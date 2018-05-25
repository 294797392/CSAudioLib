#ifndef __KERNELS_H__
#define __KERNELS_H__

#include "gplay.h"

const gplay_ops_t gplay_mplayer_ops;

static const gplay_ops_t *gplay_kernels[] =
{
    &gplay_mplayer_ops
};

#endif