#ifndef __GPLAY_MPLAYER_KERNEL_H__
#define __GPLAY_MPLAYER_KERNEL_H__

typedef struct gplay_mplayer_kernel_ctx
{
    
} gplay_mplayer_kernel_ctx_t;

int gplay_kernel_mplager_init(void **ctx);
int gplay_kernel_mplayer_release(void *ctx);
int gplay_kernel_mplayer_open(void *ctx, const char *uri);
int gplay_kernel_mplayer_close(void *ctx);
int gplay_kernel_mplayer_play(void *ctx);
int gplay_kernel_mplayer_stop(void *ctx);
int gplay_kernel_mplayer_pause(void *ctx);
int gplay_kernel_mplayer_resume(void *ctx);

#endif