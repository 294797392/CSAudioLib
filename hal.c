#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "hal.h"

#ifdef __WIN32__
#include <Windows.h>
#endif

#ifdef __LINUX__
int hal_readline(FILE *stream, char *line, int len)
{
	char *pline = NULL;
	int line_len = 0;
	size_t buf_len = 0;

	line_len = getline(&pline, &buf_len, stream);
	if(line_len <= 0)
	{
		goto end;
	}

	if (pline[line_len - 1] == '\n')
	{
		pline[line_len - 1] = '\0';
		line_len--;
	}
	if (pline[line_len - 1] == '\r')
	{
		pline[line_len - 1] = '\0';
		line_len--;
	}

	if(strlen(pline) == 0)
	{
		goto end;
	}

	strncpy(line, pline, len);

end:
	if(pline != NULL)
	{
		free(pline);
		pline = NULL;
	}
    return line_len;
}
#elif defined __WIN32__
int hal_readline(FILE *stream, char *line, int len)
{
	return 0;
}
#endif

int hal_strcmp(const char *str1, const char *str2)
{
	if(strlen(str1) != strlen(str2))
	{
		return 0;
	}

	return strncmp(str1, str2, strlen(str1)) == 0 ? 1 : 0;
}

#ifdef __LINUX__
int hal_ascii2wchar(const char *ascii, wchar_t *out)
{
	return -1;
}
#elif defined __WIN32__
int hal_ascii2wchar(const char *ascii, WCHAR **wstr)
{
	int wstr_len = 0;		// 字符串长度
	int wstr_bytes_len = 0; // 字符串所占字节长度

	if (!ascii)
	{
		return 0;
	}

	// 获取转换成宽字符所需要的字节数
	if ((wstr_len = MultiByteToWideChar(CP_ACP, 0, ascii, -1, NULL, 0)) == 0)
	{
		return 0;
	}

	// 执行真正的转换
	wstr_bytes_len = wstr_len * sizeof(WCHAR);
	(*wstr) = (WCHAR*)malloc(wstr_bytes_len);
	memset((*wstr), 0, wstr_bytes_len);
	MultiByteToWideChar(CP_ACP, 0, ascii, -1, (*wstr), wstr_len);

	return 1;
}
#endif