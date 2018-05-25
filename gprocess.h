#ifndef __GPROCESS_H__
#define __GPROCESS_H__

typedef struct gprocess gprocess_t;

int gprocess_open(char *path, char *args, gprocess_t **proc);
int gprocess_write(gprocess_t *proc, const char *data);
int gprocess_read(gprocess_t *proc, char *data, int size, int *read);
int gprocess_close(gprocess_t *proc);

#endif