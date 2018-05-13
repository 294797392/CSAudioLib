#ifndef __GPLAY_H__
#define __GPLAY_H__

typedef enum
{
    GPLAY_PARAMS_KEY_CHANNEL,
    GPLAY_PARAMS_KEY_BITRATE,
    GPLAY_PARAMS_KET_FORMATE
} gplay_params_key_t;

typedef struct gplay_params
{
    int channel;
    int bitrate;
    int formate;
} gplay_params_t;

typedef struct gplay_kernel_ops
{
    const char *name;
    int (*init)(void *ctx);
    int (*release)(void *ctx);
    int (*set_params)(void *ctx, gplay_params_t params);
    int (*play)(void *ctx, const char *uri);
    int (*stop)(void *ctx);
    int (*pause)(void *ctx);
    int (*resume)(void *ctx);
} gplay_kernel_ops_t;

typedef struct gplay
{
    void *kernel_ctx;
    gplay_kernel_ops_t *kernel;
} gplay_t;

int gplay_main_loop(int argc, char *argv[]);
int gplay_new(gplay_t **gplay);
int gplay_free(gplay_t *gplay);
int gplay_select_kernel(gplay_t *gplay, char *name);
int gplay_play(gplay_t *gplay, const char *uri);
int gplay_stop(gplay_t *gplay);
int gplay_pause(gplay_t *gplay);
int gplay_resume(gplay_t *gplay);

#endif