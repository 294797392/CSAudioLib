#include <stdlib.h>
#include <stdio.h>
#include <string.h>

#include "stream.h"

static stream_info_t stream_info_file = 
{
	"File",
	"file",
	"Albeu",
	"based on the code from ??? (probably Arpi)",
	NULL,
	{ "file", "", "sdp", NULL },
	NULL,
	1 // Urls are an option string
};

static const stream_info_t* const auto_open_streams[] =
{
	&stream_info_file,
	NULL
};

void open_stream_full(const char* filename, int mode, char** options, int* file_format)
{
	int i, j;
	for (i = 0; auto_open_streams[i]; i++)
	{
		const stream_info_t *sinfo = auto_open_streams[i];
		for (j = 0; sinfo->protocols[j]; j++) {
			int l = strlen(sinfo->protocols[j]);
			// l == 0 => Don't do protocol matching (ie network and filenames)
			if ((l == 0 && !strstr(filename, "://")) ||
				((strncmp(sinfo->protocols[j], filename, l) == 0) &&
				(strncmp("://", filename + l, 3) == 0))) {

				fprintf(stdout, "found\n");

			}
		}
	}
}