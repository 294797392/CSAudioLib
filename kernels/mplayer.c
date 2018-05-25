#include "kernels.h"
#include "gerror.h"
#include "gprocess.h"
#include "kernels/mplayer.h"

#include <stdlib.h>
#include <stdio.h>
#include <string.h>

int mplayer_ops_init(void **player)
{
	mplayer_t *mplayer = malloc(sizeof(mplayer_t));
	memset(mplayer, 0, sizeof(mplayer_t));
	(*player) = mplayer;
    return GPLAY_ERR_OK;
}

int mplayer_ops_release(void *player){ return GPLAY_ERR_OK; }

int mplayer_ops_open(void *player, const char *uri)
{
	return GPLAY_ERR_OK;
}

int mplayer_ops_close(void *player){ return GPLAY_ERR_OK; }
int mplayer_ops_play(void *player){ return GPLAY_ERR_OK; }
int mplayer_ops_stop(void *player){ return GPLAY_ERR_OK; }
int mplayer_ops_pause(void *player){ return GPLAY_ERR_OK; }
int mplayer_ops_resume(void *player){ return GPLAY_ERR_OK; }

const gplay_ops_t gplay_mplayer_ops =
{
    .name = "mplayer",
    .desc = "gplay mplayer kernel",
	.init = mplayer_ops_init,
	.release = mplayer_ops_release,
	.open = mplayer_ops_open,
	.close = mplayer_ops_close,
	.play = mplayer_ops_play,
	.stop = mplayer_ops_stop,
	.pause = mplayer_ops_pause,
	.resume = mplayer_ops_resume
};