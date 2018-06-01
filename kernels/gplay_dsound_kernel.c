#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <dsound.h>
#include "gerror.h"
#include "glog.h"
#include "gplay.h"
#include "kernels/gplay_dsound_kernel.h"

#pragma comment(lib, "dsound.lib")
#pragma comment(lib, "dxguid.lib")

///*
//* pdwCurrentPlayCursor:播放结束位置, 距离播放缓冲区起点的偏移量, 不是地址, 字节为单位, 如果不需要可以设置为NULL
//* pdwCurrentWriteCursor:当前写入的结束位置, writeCursor总是在playCursor的前面, 同上
//* 可以以writeCursor为起点写入将要播放的PCM数据, writeCursor是根据playCursor而改变的, 而不是根据写入的位置而改变
//* 为了尽可能正确的获取playCursor的值, 通常在缓冲区（DSBUFFERDESC）里设置DSBCAPS_GETCURRENTPOSITION2标志
//*/
//if ((hret = dsound->lpDirectSoundBuffer8->lpVtbl->GetCurrentPosition(dsound->lpDirectSoundBuffer8, &playCursor, &writeCursor)) != DS_OK)
//{
//	glog_error("GetCurrentPosition failed, hret=%lld", hret);
//	continue;
//}


DWORD WINAPI gplay_play_thread(LPVOID lpParam)
{
	gplay_dsound_ctx_t *dsound = (gplay_dsound_ctx_t*)lpParam;
	int offset = DEFAULT_PLAY_BUF_SIZE;
	while (dsound->state != GPLAY_STATE_IDLE)
	{
		DWORD idx = WaitForMultipleObjects(MAX_NOTIFY_POSITIONS, dsound->play_callback_evt, FALSE, INFINITE);
		if (idx >= WAIT_OBJECT_0 && idx <= WAIT_OBJECT_0 + MAX_NOTIFY_POSITIONS)
		{
			DWORD playCursor, writeCursor;
			HRESULT hret = DS_OK;

			char buf[DEFAULT_PLAY_BUF_SIZE];
			int size = fread(buf, 1, sizeof(buf), dsound->stream);
			if (size == 0)
			{
				/* 播放完了 */
			}
			else
			{
				LPVOID dataPtr1, dataPtr2;
				DWORD dataLength1, dataLength2;
				/*
				* dwOffset:锁定的内存的起点, 如果dwFlags指定了DSBLOCK_FROMWRITECURSOR标志, 那么此参数会被忽略
				* dwBytes:锁定的内存大小, 因为播放缓冲区是以圆形的形式使用的, 这个大小有可能会一直填充到播放缓冲区的结束位置, 并且会从头继续填充缓冲区
				* 其他参数与DSCapture类似
				*/
				if ((hret = dsound->lpDirectSoundBuffer8->lpVtbl->Lock(dsound->lpDirectSoundBuffer8, writeCursor, size, &dataPtr1, &dataLength1, &dataPtr2, &dataLength2, NULL)) != DS_OK)
				{
					if (hret == DSERR_BUFFERLOST)
					{
						dsound->lpDirectSoundBuffer8->lpVtbl->Restore(dsound->lpDirectSoundBuffer8);
						if ((hret = dsound->lpDirectSoundBuffer8->lpVtbl->Lock(dsound->lpDirectSoundBuffer8, writeCursor, size, &dataPtr1, &dataLength1, &dataPtr2, &dataLength2, NULL)) != DS_OK)
						{
							glog_error("GetCurrentPosition failed, hret=%lld", hret);
							continue;
						}
					}
				}

				/* fill play buffer */
				memcpy(dataPtr1, buf, dataLength1);
				if (dataLength2 > 0 && dataPtr2)
				{
					memcpy(dataPtr2, buf + dataLength1, dataLength2);
				}

				if ((hret = dsound->lpDirectSoundBuffer8->lpVtbl->Unlock(dsound->lpDirectSoundBuffer8, dataPtr1, dataLength1, dataPtr2, dataLength2)) != DS_OK)
				{
				}

				offset += DEFAULT_PLAY_BUF_SIZE;
				offset %= DEFAULT_PLAY_BUF_SIZE * MAX_NOTIFY_POSITIONS;
			}
		}
		else if (idx == WAIT_FAILED)
		{

		}
	}

	return 0;
}

int gplay_dsound_init(void *ctx)
{
	int ret = ERR_OK;
	HRESULT hret = DS_OK;
	LPDIRECTSOUND8 lpDirectSound8;
	HWND hwnd = NULL; // window handle for SetCooperativeLevel

	if ((hret = DirectSoundCreate8(NULL, &lpDirectSound8, NULL)) != DS_OK)
	{
		glog_error("DirectSoundCreate8 failed, hret=%lld\n", hret);
		ret = ERR_DSOUND_ERR;
		goto end;
	}

	if ((hret = lpDirectSound8->lpVtbl->SetCooperativeLevel(lpDirectSound8, hwnd, DSSCL_NORMAL)) != DS_OK)
	{
		glog_error("SetCooperativeLevel failed, hret=%lld\n", hret);
		ret = ERR_DSOUND_ERR;
		goto end;
	}

	gplay_dsound_ctx_t *dsound = (gplay_dsound_ctx_t*)ctx;
	dsound->lpDirectSound8 = lpDirectSound8;

end:
	return ret;
}
int gplay_dsound_release(void *ctx) {}

int gplay_dsound_open(void *ctx, const char *uri)
{
	int ret = ERR_OK;
	HRESULT hret = DS_OK;
	/* used for play buffer description */
	LPDIRECTSOUNDBUFFER lpDirectSoundBuffer = NULL;     /* used for obtain LPDIRECTSOUNDBUFFER8 interface */
	LPDIRECTSOUNDBUFFER8 lpDirectSoundBuffer8 = NULL;
	WAVEFORMATEX fmt;
	DSBUFFERDESC bufdesc;
	/* used for play buffer notifycation */
	LPDIRECTSOUNDNOTIFY8 lpDirectSoundNotify8 = NULL;
	HANDLE play_callback_evt[MAX_NOTIFY_POSITIONS] = { NULL };
	DSBPOSITIONNOTIFY dsBufferPosNotify[MAX_NOTIFY_POSITIONS];
	/* audio file */
	FILE *stream = NULL;
	/* diret sound context */
	gplay_dsound_ctx_t *dsound = (gplay_dsound_ctx_t*)ctx;

	/*
	 * 主缓冲区:
	 *	  混合播放所有二级缓冲区的声音, 控制全局3D音效参数, 主要接口:IDirectSoundBuffer, IDirectSound3DListener8
	 *	  主缓冲区是DirectSound自动创建和管理的, 应用程序播放声音不需要获取主缓冲区对象。但是如果要使用IDirectSound3DListener8接口, 那么必须创建主缓冲区对象
	 * 二级缓冲区:
	 *	  管理流声音并且在主缓冲区中播放, 主要接口:IDirectSoundBuffer8, IDirectSound3DBuffer8, IDirectSoundNotify8
	 *	  二级缓冲区可以使用声音合成和噪声, 也有控制3D音效的能力, 全局3D音效控制需要在IDirectSound3DListener8接口进行控制
	 */

	 /* 设置缓冲区信息 */
	memset(&fmt, 0, sizeof(WAVEFORMATEX));
	fmt.wFormatTag = WAVE_FORMAT_PCM;
	fmt.nChannels = DEFAULT_CHANNELS;           /* 通道数量 */
	fmt.nSamplesPerSec = DEFAULT_SAMPLE_RATE; /* 采样率, 每秒采样次数 */
	/*
	 * 采样位数, 每个采样的位数
	 * 如果wFormatTag是WAVE_FORMAT_PCM, 必须设置为8或者16, 其他的不支持
	 * 如果wFormatTag是WAVE_FORMAT_EXTENSIBLE, 必须设置为8的倍数, 一些压缩方法不定义此值, 所以此值可以为0
	 */
	fmt.wBitsPerSample = DEFAULT_BITS_PER_SAMPLE;
	/* 以字节为单位设置块对齐。块对齐是指最小数据的原子大小，如果wFormatTag = WAVE_FORMAT_PCM, nBlockAlign为(nChannels * wBitsPerSample) / 8, 对于非PCM格式请根据厂商的说明计算 */
	fmt.nBlockAlign = DEFAULT_BLOCK_ALIGN;
	/* 设置声音数据的传输速率, 每秒平均传输的字节数, 单位byte/s, 如果wFormatTag = WAVE_FORMAT_PCM, nAvgBytesPerSec为nBlockAlign * nSamplesPerSec, 对于非PCM格式请根据厂商的说明计算 */
	fmt.nAvgBytesPerSec = DEFAULT_PLAY_BUF_SIZE;
	/* 额外信息的大小，以字节为单位，额外信息添加在WAVEFORMATEX结构的结尾。这个信息可以作为非PCM格式的wFormatTag额外属性，如果wFormatTag不需要额外的信息，此值必需为0，对于PCM格式此值被忽略。 */
	fmt.cbSize = 0;

	memset(&bufdesc, 0, sizeof(DSBUFFERDESC));
	bufdesc.dwSize = sizeof(DSBUFFERDESC);
	/*
	 * DSBCAPS_LOCHARDWARE和DSBCAPS_LOCSOFTWARE两个标志位是互斥的
	 * DSBCAPS_LOCHARDWARE:缓冲区存储在声卡里, 声卡硬件实现混音
	 * DSBCAPS_LOCSOFTWARE::缓冲区存储在内存里, 软件实现混音
	 * DSBCAPS_CTRLPOSITIONNOTIFY:缓冲区通知功能
	 * DSBCAPS_GLOBALFOCUS:失去焦点继续播放功能
	 */
	bufdesc.dwFlags = DSBCAPS_CTRLPOSITIONNOTIFY | DSBCAPS_GETCURRENTPOSITION2 | DSBCAPS_GLOBALFOCUS;
	bufdesc.lpwfxFormat = &fmt;
	bufdesc.guid3DAlgorithm = GUID_NULL;
	bufdesc.dwBufferBytes = fmt.nAvgBytesPerSec; /* 存储音频流的缓冲区大小, 如果bufferDesc的flag设置了DSBCAPS_PRIMARYBUFFER, 那么必须为0 */
	bufdesc.dwReserved = 0; /* 保留字段 */
	if ((hret = dsound->lpDirectSound8->lpVtbl->CreateSoundBuffer(dsound->lpDirectSound8, &bufdesc, &lpDirectSoundBuffer, NULL)) != DS_OK)
	{
		glog_error("CreateSoundBuffer failed, hret=%lld", hret);
		ret = ERR_DSOUND_ERR;
		goto end;
	}
	lpDirectSoundBuffer->lpVtbl->QueryInterface(lpDirectSoundBuffer, &IID_IDirectSoundBuffer8, (LPVOID *)&lpDirectSoundBuffer8);
	lpDirectSoundBuffer->lpVtbl->Release(lpDirectSoundBuffer);

	/* 设置播放缓冲区回调 */
	if ((hret = lpDirectSoundBuffer8->lpVtbl->GetFormat(lpDirectSoundBuffer8, &fmt, sizeof(WAVEFORMATEX), NULL)) != DS_OK)
	{
		glog_error("GetFormat failed, hret=%lld", hret);
		ret = ERR_DSOUND_ERR;
		goto end;
	}
	if ((hret = lpDirectSoundBuffer8->lpVtbl->QueryInterface(lpDirectSoundBuffer8, &IID_IDirectSoundNotify8, (LPVOID *)&lpDirectSoundNotify8)) != DS_OK)
	{
		glog_error("obtain IID_IDirectSoundNotify8 failed, hret=%lld", hret);
		ret = ERR_DSOUND_ERR;
		goto end;
	}
	for (int idx = 0; idx < MAX_NOTIFY_POSITIONS; idx++)
	{
		if (!(play_callback_evt[idx] = CreateEvent(NULL, 1, 0, NULL)))
		{
			glog_error("create notify evt failed, hret=%lld", hret);
			ret = ERR_DSOUND_ERR;
			goto end;
		}
		dsBufferPosNotify[idx].dwOffset = bufdesc.dwBufferBytes * idx; // 当缓冲区播放完的时候进行通知
		dsBufferPosNotify[idx].hEventNotify = play_callback_evt[idx];
	}
	if ((hret = lpDirectSoundNotify8->lpVtbl->SetNotificationPositions(lpDirectSoundNotify8, MAX_NOTIFY_POSITIONS, dsBufferPosNotify)) != DS_OK)
	{
		glog_error("SetNotificationPositions failed, hret=%lld", hret);
		ret = ERR_DSOUND_ERR;
		goto end;
	}

	/* 打开文件 */
	if (!(stream = fopen(uri, "rb+")))
	{
		glog_error("open %s failed", uri);
		ret = ERR_OPEN_FILE_ERR;
		goto end;
	}

	/* set context state */
	dsound->lpDirectSoundBuffer8 = lpDirectSoundBuffer8;
	memcpy(dsound->play_callback_evt, play_callback_evt, sizeof(play_callback_evt));
	dsound->state = GPLAY_STATE_IDLE;
	dsound->stream = stream;
	dsound->fmt = fmt;
	strncpy(dsound->uri, uri, sizeof(dsound->uri));

end:
	lpDirectSoundNotify8->lpVtbl->Release(lpDirectSoundNotify8);
	return ret;
}

int gplay_dsound_close(void *ctx) {}

int gplay_dsound_play(void *ctx)
{
	int ret = ERR_OK;
	HANDLE play_thread_handle = NULL;
	gplay_dsound_ctx_t *dsound = (gplay_dsound_ctx_t*)ctx;

	if (!(play_thread_handle = CreateThread(NULL, 0, gplay_play_thread, ctx, 0, &dsound->play_thread_id)))
	{
		glog_error("create play thread failed");
		ret = ERR_CREATE_THREAD_ERR;
		goto end;
	}

	dsound->play_thread_hwnd = play_thread_handle;

end:
	return ret;
}

int gplay_dsound_stop(void *ctx) {}
int gplay_dsound_pause(void *ctx) {}
int gplay_dsound_resume(void *ctx) {}

const static gplay_kernel_t gplay_dsound_kernel =
{
	.name = "Windows DirectSound Kernel",
	.init = gplay_dsound_init,
	.release = gplay_dsound_release,
	.play = gplay_dsound_play,
	.stop = gplay_dsound_stop,
	.pause = gplay_dsound_pause,
	.resume = gplay_dsound_resume
};