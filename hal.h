#ifndef __HAL_H__
#define __HAL_H__

#ifdef _MSC_VER
#define __WIN32__
#endif // _MSC_VER

#ifdef __GNUC__
#define __LINUX__
#endif

#include <stdio.h>
#include <stdlib.h>
#include <string.h>

int hal_readline(FILE *stream, char *line, int len);
int hal_strcmp(const char *str1, const char *str2);
int hal_ascii2wchar(const char *ascii, wchar_t *out);

#endif