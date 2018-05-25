#include <stdlib.h>
#include <stdio.h>
#include <stddef.h>
#include <windows.h>

#include "gprocess.h"

int main(int argc, char *argv[])
{
	gprocess_t *proc;
	gprocess_open("F:\\gardenia-media\\windows\\build\\mplayer.exe", "-slave -quiet F:\\gardenia-media\\windows\\build\\test.mp3", &proc);

	while (1)
	{
		char line[1024] = { '\0' };
		int read;
		gprocess_read(proc, line, sizeof(line), &read);
		printf("%s\n", line);
		Sleep(1000);
	}

    return 0;
}


