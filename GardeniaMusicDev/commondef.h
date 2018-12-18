#ifndef __COMMONDEF_H__
#define __COMMONDEF_H__

#define API_EXPORT __declspec(dllexport)

#define LOGI(fmt, ...) fprintf(stdout, fmt, __VA_ARGS__)
#define LOGW(fmt, ...) fprintf(stdout, fmt, __VA_ARGS__)
#define LOGE(fmt, ...) fprintf(stdout, fmt, __VA_ARGS__)

typedef struct music_format
{
	char name[2048];
}music_format_t;

#endif // !__COMMONDEF_H__


