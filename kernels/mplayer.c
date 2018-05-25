#include "kernels.h"
#include "gerror.h"
#include "gprocess.h"
#include "kernels/mplayer.h"

#include <stdlib.h>
#include <stdio.h>
#include <string.h>

void mplayer_pause_and_resume(mplayer_t *mplayer)
{
	gprocess_write(mplayer->mplay_proc, "pause");
}

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
	mplayer_t *mplayer = (mplayer_t*)player;
	strncpy(mplayer->uri, uri, strlen(uri));

	return GPLAY_ERR_OK;
}

int mplayer_ops_close(void *player){ return GPLAY_ERR_OK; }

int mplayer_ops_play(void *player)
{
	mplayer_t *mplayer = (mplayer_t*)player;
	int ret = ERR_OK;
	gprocess_t *proc = NULL;
	char cmd[1024] = { '\0' };
	char line[1024] = { '\0' };
	int len = 0;

	sprintf_s(cmd, sizeof(cmd), "-slave -quiet %s", mplayer->uri);
	if ((ret = gprocess_open(DEFAULT_MPLAYER_EXE_PATH, cmd, &proc)) != ERR_OK)
	{
		goto end;
	}

end:
	return ret;
}

int mplayer_ops_stop(void *player){ return GPLAY_ERR_OK; }

int mplayer_ops_pause(void *player)
{
	mplayer_pause_and_resume((mplayer_t*)player);
	return GPLAY_ERR_OK; 
}

int mplayer_ops_resume(void *player)
{
	mplayer_pause_and_resume((mplayer_t*)player);
	return GPLAY_ERR_OK; 
}

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