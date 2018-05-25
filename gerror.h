#ifndef __GERROR_H__
#define __GERROR_H__

/* 0 - 500 generic error */
#define ERR_OK 1
#define ERR_DSOUND_ERR 2
#define ERR_OPEN_FILE_ERR 3
#define ERR_CREATE_THREAD_ERR 4
#define ERR_CONV_WIDE_CHAR_ERR 5

#define GPLAY_ERR_OK 1
#define GPLAY_ERR_FAILED 2

/* 501 - 600 gprocess error */
#define ERR_GPROC_CREATE_PIPE_FAILED 501
#define ERR_GPROC_PIPE_HANDLE_ERROR 502
#define ERR_GPROC_START_FAILED 503
#define ERR_GPROC_READ_ERR 504
#define ERR_GPROC_WRITE_ERR 505

#endif