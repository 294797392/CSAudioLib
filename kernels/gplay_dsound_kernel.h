#ifndef __GPLAY_DSOUND_KERNEL_H__
#define __GPLAY_DSOUND_KERNEL_H__

#include <stdio.h>
#include <stdlib.h>
#include <Windows.h>
#include <dsound.h>

#include "gplay.h"

#define MAX_NOTIFY_POSITIONS 4 /* 缓冲区通知位置数量 */
#define DEFAULT_CHANNELS 2
#define DEFAULT_SAMPLE_RATE 44100
#define DEFAULT_BITS_PER_SAMPLE 16
#define DEFAULT_BLOCK_ALIGN DEFAULT_CHANNELS * DEFAULT_BITS_PER_SAMPLE / 8
#define DEFAULT_PLAY_BUF_SIZE DEFAULT_BLOCK_ALIGN * DEFAULT_SAMPLE_RATE

typedef enum
{
	GPLAY_STATE_IDLE,
	GPLAY_STATE_PLAY,
	GPLAY_STATE_PAUSE
};

int gplay_dsound_init(void *ctx);
int gplay_dsound_release(void *ctx);
int gplay_dsound_open(void *ctx, const char *uri);
int gplay_dsound_close(void *ctx);
int gplay_dsound_play(void *ctx);
int gplay_dsound_stop(void *ctx);
int gplay_dsound_pause(void *ctx);
int gplay_dsound_resume(void *ctx);

/*
 * 存储播放器内核上下文信息
 */
typedef struct gplay_dsound_ctx
{
    FILE *stream;
    int state;
	HANDLE play_callback_evt[MAX_NOTIFY_POSITIONS];
    LPDIRECTSOUND8 lpDirectSound8;
    LPDIRECTSOUNDBUFFER8 lpDirectSoundBuffer8;
    char uri[MAX_URI_LEN];
	HANDLE play_thread_hwnd;
	DWORD play_thread_id;
	WAVEFORMATEX fmt;
} gplay_dsound_ctx_t;

#endif