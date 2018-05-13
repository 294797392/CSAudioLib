#ifndef __DS_DRIVER_H__
#define __DS_DRIVER_H__

#include "gplay.h"

int gplay_dsound_init(void *ctx);
int gplay_dsound_release(void *ctx);
int gplay_dsound_set_params(void *ctx, gplay_params_t params);
int gplay_dsound_play(void *ctx, const char *uri);
int gplay_dsound_stop(void *ctx);
int gplay_dsound_pause(void *ctx);
int gplay_dsound_resume(void *ctx);

/*
 * 存储播放器内核上下文信息
 */
typedef struct gplay_dsound_ctx
{

} gplay_dsound_ctx_t;

#endif