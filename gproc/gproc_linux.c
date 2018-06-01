#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <signal.h>
#include <sys/types.h>

#include "glog.h"
#include "hal.h"
#include "gproc.h"
#include "gerror.h"

struct gproc
{
    FILE *input;
    FILE *output;
	int in_pipe_fd;
	int out_pipe_fd;
	int pid;
};

int gproc_create(gproc_t **proc){
	*proc = malloc(sizeof(gproc_t));
	memset(*proc, 0, sizeof(gproc_t));
	return ERR_OK;
}

int gproc_release(gproc_t *proc){
	free(proc);
	return ERR_OK;
}

int gproc_open(const char *path, char **args, gproc_t *proc){
	int ret = ERR_OK;
	int fk = -1;
	int input[2] = { 0 };
	int output[2] = { 0 };
	FILE *finput = NULL;
	FILE *foutput = NULL;

	if(pipe(input) == -1 || pipe(output) == -1){
		glog_error("create pipe failed, errno = %i", errno);
		goto err;
	}

	fk = fork();
	if(fk == -1){
		glog_error("fork failed, errno = %i", errno);
		goto err;
	}

	if(fk == 0){
		close(input[0]); /* 关闭pipe写端 */
		input[0] = 0;
		dup2(input[1], STDIN_FILENO); /* 将子进程的标准输入重定向到管道的读端 */

		close(output[1]); /* 关闭pipe读端 */
		output[1] = 0;
		dup2(output[0], STDOUT_FILENO); /*  将子进程的标准输出重定向到写端 */

		execv(path, args); /*  把要启动的程序载入子进程 */
	}
	else{
		close(input[1]); /* 关闭pipe读端 */
		input[1] = 0;

		close(output[0]); /* 关闭pipe写端 */
		output[0] = 0;

		if(!(finput = fdopen(input[0], "w"))){
			glog_error("fdopen input failed, errno = %i", errno);
			goto err;
		}
		if(!(foutput = fdopen(output[1], "r"))){
			glog_error("fdopen output failed, error = %i", errno);
			goto err;
		}
	}

	*proc->input = finput;
	*proc->output = foutput;
	*proc->in_pipe_fd = input[0];
	*proc->out_pipe_fd = output[1];
	*proc->pid = fk;

end:
	return ret;
err:
	if(finput){ fclose(finput); }
	if(foutput){ fclose(foutput); }
	ret = errno;
	goto end;
}

int gproc_close(gproc_t *proc){
	int ret = kill(proc->pid, 0);
	if(ret != ESRCH){
		kill(proc->pid, SIGINT);
	}
	if(proc->input){ fclose(proc->input); }
	if(proc->output){ fclose(proc->output); }
	memset(proc, 0, sizeof(gproc_t));
	return ERR_OK;
}

int gproc_write(gproc_t *proc, const char *data){
	fwrite(data, 1, strlen(data), proc->input);
	fflush(proc->input);
	return ERR_OK;
}

int gproc_readline(gproc_t *proc, char *data, int size, int *read){
	int readed = 0;
	while(1){
		if(readed > size){
			return ERR_OK;
		}
		fread(data + readed, 1, 1, proc->output);
		if(strncmp(data + readed, "\n", 1) == 0){
			return ERR_OK;
		}
		readed++;
		*read = readed;
	}
	return ERR_OK;
}