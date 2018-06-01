#ifndef __GPROC_H__
#define __GPROC_H__

typedef struct gproc gproc_t;

int gproc_create(gproc_t **proc);
int gproc_release(gproc_t *proc);
int gproc_open(const char *path, char **args, gproc_t *proc);
int gproc_close(gproc_t *proc);
int gproc_write(gproc_t *proc, const char *data);
int gproc_readline(gproc_t *proc, char *data, int size, int *read);

#endif