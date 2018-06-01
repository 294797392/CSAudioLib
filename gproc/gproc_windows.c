#include <stdio.h>
#include <stdlib.h>
#include <windows.h>

#include "hal.h"
#include "gproc.h"
#include "gerror.h"

struct gproc
{
    HANDLE in;
    HANDLE out;
};

int gproc_open(const char *path, char **args, gproc_t **proc)
{
    int ret = ERR_OK;
    HANDLE g_hChildStd_IN_Rd = NULL;
    HANDLE g_hChildStd_IN_Wr = NULL;
    HANDLE g_hChildStd_OUT_Rd = NULL;
    HANDLE g_hChildStd_OUT_Wr = NULL;
    SECURITY_ATTRIBUTES saAttr; 
    STARTUPINFO siStartInfo;
    BOOL bSuccess = FALSE;
	PROCESS_INFORMATION piProcInfo;
	LPWSTR wpath = NULL;
	LPWSTR wargs = NULL;

	// Set the bInheritHandle flag so pipe handles are inherited. 
    saAttr.nLength = sizeof(SECURITY_ATTRIBUTES);
    saAttr.bInheritHandle = TRUE;
    saAttr.lpSecurityDescriptor = NULL;

    if (!CreatePipe(&g_hChildStd_OUT_Rd, &g_hChildStd_OUT_Wr, &saAttr, 0))
    {
        ret = ERR_GPROC_CREATE_PIPE_FAILED;
        goto end;
    }

    // Ensure the read handle to the pipe for STDOUT is not inherited.
    if (!SetHandleInformation(g_hChildStd_OUT_Rd, HANDLE_FLAG_INHERIT, 0))
    {
        ret = ERR_GPROC_PIPE_HANDLE_ERROR;
        goto end;
    }

    // Create a pipe for the child process's STDIN. 
    if (!CreatePipe(&g_hChildStd_IN_Rd, &g_hChildStd_IN_Wr, &saAttr, 0))
    {
        ret = ERR_GPROC_CREATE_PIPE_FAILED;
        goto end;
    }

    // Ensure the write handle to the pipe for STDIN is not inherited. 
   	if (!SetHandleInformation(g_hChildStd_IN_Wr, HANDLE_FLAG_INHERIT, 0))
   	{
		ret = ERR_GPROC_PIPE_HANDLE_ERROR;
		goto end;
   	}

    // Set up members of the PROCESS_INFORMATION structure. 
    memset(&piProcInfo, 0, sizeof(PROCESS_INFORMATION));

	// Set up members of the STARTUPINFO structure. 
    // This structure specifies the STDIN and STDOUT handles for redirection.
    ZeroMemory(&siStartInfo, sizeof(STARTUPINFO));
    siStartInfo.cb = sizeof(STARTUPINFO); 
    siStartInfo.hStdError = g_hChildStd_OUT_Wr;
    siStartInfo.hStdOutput = g_hChildStd_OUT_Wr;
    siStartInfo.hStdInput = g_hChildStd_IN_Rd;
    siStartInfo.dwFlags |= STARTF_USESTDHANDLES;

	hal_ascii2wchar(path, &wpath);
	hal_ascii2wchar(args, &wargs);

    // Create the child process. 
	if (!CreateProcess(wpath,
		wargs,		   // command line 
        NULL,          // process security attributes 
        NULL,          // primary thread security attributes 
        TRUE,          // handles are inherited 
        0,             // creation flags 
        NULL,          // use parent's environment 
        NULL,          // use parent's current directory 
        &siStartInfo,  // STARTUPINFO pointer 
        &piProcInfo))  // receives PROCESS_INFORMATION
    {
		DWORD err = GetLastError();
        ret = ERR_GPROC_START_FAILED;
        goto end;
    }

    (*proc) = (gproc_t*)malloc(sizeof(gproc_t));
    memset((*proc), 0, sizeof(gproc_t));
    (*proc)->in = g_hChildStd_IN_Wr;
    (*proc)->out = g_hChildStd_OUT_Rd;

end:
	if (wpath){ free(wpath); wpath = NULL; }
	if (wargs){ free(wargs); wargs = NULL; }
    return ret;
}

int gproc_write(gproc_t *proc, const char *data, int len)
{
    int ret = ERR_OK;
	DWORD dwWritten;
	if (WriteFile(proc->in, data, len, &dwWritten, NULL) == FALSE)
	{
		ret = ERR_GPROC_WRITE_ERR;
		goto end;
	}

end:
    return ret;
}

int gproc_read(gproc_t *proc, char *data, int size, int *read)
{
    int ret = ERR_OK;
	if (ReadFile(proc->out, data, size, read, NULL) == FALSE)
	{
		ret = ERR_GPROC_READ_ERR;
		goto end;
	}

end:
    return ret;
}

int gproc_close(gproc_t *proc)
{
    int ret = ERR_OK;

end:
    return ret;
}