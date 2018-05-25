#ifndef __GPLAY_H__
#define __GPLAY_H__

#define MAX_URI_LEN 1024

typedef struct gplay_ops
{
    const char *name;
    const char *desc;
    int (*init)(void **kernel);
	int(*release)(void *kernel);
	int(*open)(void *kernel, const char *uri);
	int(*close)(void *kernel);
	int(*play)(void *kernel);
	int(*stop)(void *kernel);
	int(*pause)(void *kernel);
	int(*resume)(void *kernel);
} gplay_ops_t;

typedef struct gplay
{
    void *kernel_ctx;
	gplay_ops_t *kernel;
} gplay_t;

int gplay_main_loop(int argc, char *argv[]);
int gplay_create(gplay_t **gplay);
int gplay_init(gplay_t *gplay, const char *kernel_name);
int gplay_release(gplay_t *gplay);
int gplay_open(gplay_t *gplay, const char *uri);
int gplay_close(gplay_t *gplay);
int gplay_play(gplay_t *gplay);
int gplay_stop(gplay_t *gplay);
int gplay_pause(gplay_t *gplay);
int gplay_resume(gplay_t *gplay);

#endif