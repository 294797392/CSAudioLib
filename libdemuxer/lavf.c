#include <stdlib.h>
#include <stdio.h>
#include <string.h>

#include "lavf.h"

int avcodec_initialized = 0;
int avformat_initialized = 0;

void demux_lavf_init()
{
	if (avformat_initialized == 0)
	{
		av_register_all();
		avformat_initialized = 1;
	}
}