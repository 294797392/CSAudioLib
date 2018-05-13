#include <dsound.h>
#include "gplay.h"
#include "kernels/gplay_dsound_kernel.h"

int gplay_dsound_init(void *ctx){}
int gplay_dsound_release(void *ctx){}
int gplay_dsound_set_params(void *ctx, gplay_params_t params){}
int gplay_dsound_play(void *ctx, const char *uri){}
int gplay_dsound_stop(void *ctx){}
int gplay_dsound_pause(void *ctx){}
int gplay_dsound_resume(void *ctx){}

const static gplay_kernel_ops_t gplay_dsound_kernel_ops = 
{
    .name = "Windows DirectSound Kernel",
    .init = gplay_dsound_init,
    .release = gplay_dsound_release,
    .set_params = gplay_dsound_set_params,
    .play = gplay_dsound_play,
    .stop = gplay_dsound_stop,
    .pause = gplay_dsound_pause,
    .resume = gplay_dsound_resume
};