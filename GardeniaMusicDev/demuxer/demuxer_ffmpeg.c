#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include <windows.h>
#include <share.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <fcntl.h>

#include "commondef.h"
#ifdef __cplusplus
extern "C"
{
#endif
#include "libavformat/avformat.h"
#include "libavformat/avio.h"
#include "libavutil/avutil.h"
#include "libavutil/avstring.h"
#include "libavutil/mathematics.h"
#include "libavutil/opt.h"
#include "libavutil/replaygain.h"
#ifdef __cplusplus
}
#endif

#pragma comment(lib, "avcodec.lib")
#pragma comment(lib, "avdevice.lib")
#pragma comment(lib, "avfilter.lib")
#pragma comment(lib, "avutil.lib")
#pragma comment(lib, "avformat.lib")
#pragma comment(lib, "postproc.lib")
#pragma comment(lib, "swresample.lib")
#pragma comment(lib, "swscale.lib")

#define FFMAX(a,b) ((a) > (b) ? (a) : (b))
#define FFMIN(a,b) ((a) > (b) ? (b) : (a))
#define FFMINMAX(c,a,b) FFMIN(FFMAX(c, a), b)
#define SMALL_MAX_PROBE_SIZE (32 * 1024)
#define PROBE_BUF_SIZE (2 * 1024 * 1024)

typedef int(__stdcall *stream_reader)(void *dest, int size);

typedef struct demux_ffmpeg_info
{
	char uri[2048];
	stream_reader read;
} demux_ffmpeg_info_t;

typedef struct demux_ffmpeg_context
{
	AVInputFormat *avif;
	AVFormatContext *avfc;
	int use_netstream;
} demux_ffmpeg_context_t;

API_EXPORT demux_ffmpeg_context_t *demux_ffmpeg_init()
{
	demux_ffmpeg_context_t *ctx = (demux_ffmpeg_context_t*)malloc(sizeof(demux_ffmpeg_context_t));
	memset(ctx, 0, sizeof(demux_ffmpeg_context_t));
	av_register_all();
	return ctx;
}

API_EXPORT int demux_ffmpeg_check(demux_ffmpeg_context_t *ctx, demux_ffmpeg_info_t info)
{
	int read_size = 2048;
	int probe_data_size = 0;
	int score = 0;
	AVProbeData avpd = { 0 };
	avpd.buf = av_mallocz(PROBE_BUF_SIZE + AV_INPUT_BUFFER_PADDING_SIZE);

	do
	{
		read_size = info.read(avpd.buf + probe_data_size, read_size);
		if (read_size < 0)
		{
			av_free(avpd.buf);
			return 0;
		}
		probe_data_size += read_size;
		avpd.filename = info.uri;
		if (!avpd.filename)
		{
			avpd.filename = "";
		}
		if (!strncmp(avpd.filename, "ffmpeg://", 9))
		{
			avpd.filename += 9;
		}
		avpd.buf_size = probe_data_size;

		score = 0;
		ctx->avif = av_probe_input_format2(&avpd, probe_data_size > 0, &score);
		read_size = FFMIN(2 * read_size, PROBE_BUF_SIZE - probe_data_size);
	} while (probe_data_size < SMALL_MAX_PROBE_SIZE &&
		score <= AVPROBE_SCORE_MAX / 4 &&
		read_size > 0 && probe_data_size < PROBE_BUF_SIZE);
	av_free(avpd.buf);

	if (!ctx->avif)
	{
		LOGW("demux_ffmpeg_check: no clue about this gibberish!\n");
		return 0;
	}
	else
	{
		LOGI("demux_ffmpeg_check: %s\n", ctx->avif->long_name);
		if (!strcmp(ctx->avif->name, "hls,applehttp"))
		{
			LOGI("demux_ffmpeg_check: network streaming with lavf\n");
			avformat_network_init();
			ctx->use_netstream = 1;
		}
	}

	return 1;
}

API_EXPORT int demux_ffmpeg_open(demux_ffmpeg_context_t *ctx, demux_ffmpeg_info_t info)
{
	int errnum = 0;
	AVDictionary *opts = NULL;
	AVFormatContext *avfc;
	AVDictionaryEntry *t = NULL;

	avfc = avformat_alloc_context();
	if (ctx->avif->flags & AVFMT_NOFILE)
	{

	}

	if ((errnum = avformat_open_input(&avfc, "D://ДњТы//gardenia-music//build//1.mp3", ctx->avif, &opts)) < 0)
	{
		char buf[1024] = { '\0' };
		av_strerror(errnum, buf, sizeof(buf));
		LOGE("demux_ffmpeg_open: av_open_input_stream() failed, %s\n", buf);
		return NULL;
	}

	ctx->avfc = avfc;

	if (avformat_find_stream_info(avfc, NULL) < 0) 
	{
		LOGE("demux_ffmpeg_open: av_find_stream_info() failed\n");
		return NULL;
	}

	return 1;
}