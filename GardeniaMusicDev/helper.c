#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdint.h>

#include "commondef.h"

API_EXPORT uint32_t helper_mp3_frame_head(unsigned char hubf1, unsigned char hubf2, unsigned char hubf3, unsigned char hubf4)
{
	uint32_t result =	hubf1 << 24 |
						hubf2 << 16 |
						hubf3 << 8 |
						hubf4;

	return result;
}