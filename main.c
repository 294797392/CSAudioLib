#include <stdlib.h>
#include <stdio.h>
#include <stddef.h>

#include "gproc.h"

void test(int *c)
{
	(*c) = 1;
}

int main(int argc, char *argv[])
{
	int c = 2;
	test(&c);

	gproc_t *proc;
	gproc_open("F:\\gardenia-media\\windows\\build\\mplayer.exe", "-slave -quiet F:\\gardenia-media\\windows\\build\\test.mp3", &proc);

	while (1)
	{
		char line[1024] = { '\0' };
		int read;
		gproc_read(proc, line, sizeof(line), &read);
		printf("%s\n", line);
		Sleep(1000);
	}

    return 0;
}


