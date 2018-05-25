#ifndef __GPLAY_MPLAYER_KERNEL_H__
#define __GPLAY_MPLAYER_KERNEL_H__

#include "gprocess.h"
#include "gplay.h"

#define DEFAULT_MPLAYER_EXE_PATH "mplayer.exe"

typedef struct mplayer
{
	char uri[MAX_URI_LEN];
	gprocess_t *mplay_proc;
	int status;
} mplayer_t;

int mplayer_ops_init(void **player);
int mplayer_ops_release(void *player);
int mplayer_ops_open(void *player, const char *uri);
int mplayer_ops_close(void *player);
int mplayer_ops_play(void *player);
int mplayer_ops_stop(void *player);
int mplayer_ops_pause(void *player);
int mplayer_ops_resume(void *player);
int mplayer_ops_status(void *player, int *state);

#endif