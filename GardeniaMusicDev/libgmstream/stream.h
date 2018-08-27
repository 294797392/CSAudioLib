#ifndef __GMSTREAM_H__
#define __GMSTREAM_H__

#define MAX_STREAM_PROTOCOLS 10

typedef struct stream_info_st {
	const char *info;
	const char *name;
	const char *author;
	const char *comment;
	/// mode isn't used atm (ie always READ) but it shouldn't be ignored
	/// opts is at least in it's defaults settings and may have been
	/// altered by url parsing if enabled and the options string parsing.
	int(*open)(struct stream* st, int mode, void* opts, int* file_format);
	const char* protocols[MAX_STREAM_PROTOCOLS];
	const void* opts;
	int opts_url; /* If this is 1 we will parse the url as an option string
				  * too. Otherwise options are only parsed from the
				  * options string given to open_stream_plugin */
} stream_info_t;

void open_stream_full(const char* filename, int mode, char** options, int* file_format);

#endif