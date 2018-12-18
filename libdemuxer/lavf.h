#ifndef __DEMUX_LAVF_H__
#define __DEMUX_LAVF_H__

#include "libavformat/avformat.h"
#include "libavformat/avio.h"
#include "libavutil/avutil.h"
#include "libavutil/avstring.h"
#include "libavutil/mathematics.h"
#include "libavutil/opt.h"
#include "base.h"

typedef struct lavf_priv {
	AVInputFormat *avif;
	AVFormatContext *avfc;
	AVIOContext *pb;
	int audio_streams;
	int video_streams;
	int sub_streams;
	int64_t last_pts;
	int astreams[MAX_A_STREAMS];
	int vstreams[MAX_V_STREAMS];
	int sstreams[MAX_S_STREAMS];
	int cur_program;
	int nb_streams_last;
}lavf_priv_t;

void demux_lavf_init();

#endif