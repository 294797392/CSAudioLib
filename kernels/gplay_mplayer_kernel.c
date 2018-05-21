
#include "kernels.h"
#include "gerror.h"
#include "kernels/gplay_mplayer_kernel.h"

int gplay_kernel_mplager_init(void **ctx){return GPLAY_ERR_OK;}
int gplay_kernel_mplayer_release(void *ctx){return GPLAY_ERR_OK;}
int gplay_kernel_mplayer_open(void *ctx, const char *uri){return GPLAY_ERR_OK;}
int gplay_kernel_mplayer_close(void *ctx){return GPLAY_ERR_OK;}
int gplay_kernel_mplayer_play(void *ctx){return GPLAY_ERR_OK;}
int gplay_kernel_mplayer_stop(void *ctx){return GPLAY_ERR_OK;}
int gplay_kernel_mplayer_pause(void *ctx){return GPLAY_ERR_OK;}
int gplay_kernel_mplayer_resume(void *ctx){return GPLAY_ERR_OK;}

const gplay_kernel_t gplay_kernel_mplayer = 
{
    .name = "mplayer",
    .desc = "gplay mplayer kernel",
    .init = gplay_kernel_mplager_init,
    .release = gplay_kernel_mplayer_release,
    .open = gplay_kernel_mplayer_open,
    .close = gplay_kernel_mplayer_close,
    .play = gplay_kernel_mplayer_play,
    .stop = gplay_kernel_mplayer_stop,
    .pause = gplay_kernel_mplayer_pause,
    .resume = gplay_kernel_mplayer_resume
};