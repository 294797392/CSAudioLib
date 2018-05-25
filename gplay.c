#include "errno.h"
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stddef.h>

#include "gerror.h"
#include "kernels\kernels.h"
#include "gplay.h"

int gplay_main_loop(int argc, char *argv[])
{
    return GPLAY_ERR_OK;
}

int gplay_create(gplay_t **gplay)
{
    (*gplay) = (gplay_t*)malloc(sizeof(gplay_t));
    memset((*gplay), 0, sizeof(gplay_t));
    return GPLAY_ERR_OK;
}

int gplay_init(gplay_t *gplay, const char *kernel_name)
{
    gplay_ops_t *kernel = NULL;
    void *ctx = NULL;
    int ret = GPLAY_ERR_OK;
	int len = sizeof(gplay_kernels) / sizeof(gplay_ops_t);

    for(int i = 0; i < len; i++)
    {
        kernel = gplay_kernels[i];
        if(strncmp(kernel_name, kernel->name, sizeof(kernel_name)) == 0)
        {
            if((ret = kernel->init(&ctx)) == GPLAY_ERR_OK)
            {
                ret = GPLAY_ERR_FAILED;
                goto end;
            }
        }
    }

    gplay->kernel = kernel;
    gplay->kernel_ctx = ctx;

end:
    return ret;
}

int gplay_release(gplay_t *gplay)
{
	gplay_ops_t *kernel = (gplay_ops_t*)gplay->kernel;
    kernel->release(gplay->kernel_ctx);
    return GPLAY_ERR_OK;
}

int gplay_open(gplay_t *gplay, const char *uri)
{
	gplay_ops_t *kernel = (gplay_ops_t*)gplay->kernel;
    kernel->open(gplay->kernel_ctx, uri);
    return GPLAY_ERR_OK;
}

int gplay_close(gplay_t *gplay)
{
	gplay_ops_t *kernel = (gplay_ops_t*)gplay->kernel;
    kernel->close(gplay->kernel_ctx);
    return GPLAY_ERR_OK;
}

int gplay_play(gplay_t *gplay)
{
	gplay_ops_t *kernel = (gplay_ops_t*)gplay->kernel;
    kernel->play(gplay->kernel_ctx);
    return GPLAY_ERR_OK;
}

int gplay_stop(gplay_t *gplay)
{
	gplay_ops_t *kernel = (gplay_ops_t*)gplay->kernel;
    kernel->stop(gplay->kernel_ctx);
    return GPLAY_ERR_OK;
}

int gplay_pause(gplay_t *gplay)
{
	gplay_ops_t *kernel = (gplay_ops_t*)gplay->kernel;
    kernel->pause(gplay->kernel_ctx);
    return GPLAY_ERR_OK;
}

int gplay_resume(gplay_t *gplay)
{
	gplay_ops_t *kernel = (gplay_ops_t*)gplay->kernel;
    kernel->resume(gplay->kernel_ctx);
    return GPLAY_ERR_OK;
}
