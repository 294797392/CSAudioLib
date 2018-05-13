#ifndef __GPLAY_H__
#define __GPLAY_H__

#define MAX_URI_LEN 1024

typedef enum
{
    GPLAY_PARAMS_KEY_CHANNEL,
    GPLAY_PARAMS_KEY_BITRATE,
    GPLAY_PARAMS_KET_FORMATE
};

typedef enum 
{
    GPLAY_OPT_NONE,          /* 无选项 */
    GPLAY_OPT_BUFFERED_PLAY, /* 缓冲播放 */
} gplay_opt_t;

typedef struct gplay_params
{
    int channel;        /* 通道数 */
    int bitrate;        /* 比特率 */
    int formate;        /* 文件格式 */
    int sample_rate;    /* 采样率 */
} gplay_params_t;

typedef struct gplay_kernel
{
    const char *name;
    int (*init)(void *ctx);
    int (*release)(void *ctx);
    int (*set_params)(void *ctx, gplay_params_t params);
    int (*play)(void *ctx, const char *uri);
    int (*stop)(void *ctx);
    int (*pause)(void *ctx);
    int (*resume)(void *ctx);
} gplay_kernel_t;

typedef struct gplay
{
    void *kernel_ctx;
    gplay_kernel_t *kernel;
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