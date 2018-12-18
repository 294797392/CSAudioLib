#ifndef __MPLAYER_H__
#define __MPLAYER_H__

/*
 * ��������£����������������¼����˳���ʽ��
 * 1.�������Ž������Զ��˳�
 *		�Ƿ�֪ͨ����
 * 2.�ֶ�����stop�˳�
 *		�Ƿ�֪ͨ����
 * 3.����̷���stop�����˳�
 *		�Ƿ�֪ͨ����
 */

#ifdef WINDOWS
#define HAVE_STRUCT_TIMESPEC		/* �������pthreadͷ�ļ����벻�������⣨��timespec�� : ��struct�������ض��壩 */
#pragma comment(lib, "pthreadVC2.lib") /* ����pthread�� */
#endif

#include <pthread.h>

#ifdef WINDOWS
#define MPLAYER_PATH "D:/MPlayer/mplayer/mplayer.exe"
#define SLEEP(dwMilliseconds) Sleep(dwMilliseconds)
#define API_EXPORT __declspec(dllexport)
#else
#define MPLAYER_PATH "mplayer"
typedef int BOOL;
#ifndef FALSE
#define FALSE 0
#endif
#ifndef TRUE
#define TRUE 1
#endif
#define SLEEP(dwMilliseconds) sleep(dwMilliseconds / 1000)
#define API_EXPORT
#endif
#define DEFAULT_SOURCE_SIZE 1024
#define MP_SAFE_FREE(FUNC, PTR) if(PTR) { FUNC(*PTR); *PTR = NULL; }

/* ������س��� */
#define VOLUME_DEFAULT 70
#define VOLUME_MAX 100
#define VOLUME_MIN 0
#define VOLUME_DELTA 10

/* ����ֵ */
#define MP_FAILED -1
#define MP_SUCCESS 0
#define MP_CREATE_PROCESS_FAILED 1
#define MP_SEND_COMMAND_FAILED 2
#define MP_READ_DATA_FAILED 3

/* mplayer���� */
typedef struct tagMPOPT
{
	char mplayer_path[1024];
}mplayer_opt_t;

/* �������¼����Ͷ��� */
typedef enum tagMPEVENT
{
	MPEVT_STATUS_CHANGED
}mplayer_event_enum;

/* ������״̬ */
typedef enum tagMPSTATUS
{
    MPSTAT_PLAYING,
    MPSTAT_PAUSED,
	MPSTAT_STOPPED
}mplayer_status_enum;

/* mplayer�¼��ص� */
typedef int(*mp_event_handler)(mplayer_event_enum evt, void *evt_data, void *userdata);

/* �¼������� */
typedef struct tagMPLISTENER
{
	mp_event_handler handler;
	void *userdata;
} mplayer_listener_t;

/* ��ͬƽ̨�µĲ������ڲ�����ָ�� */
typedef struct tagMPLAYER_PRIV mplayer_priv_t;

/* ��������� */
typedef struct tagMPLAYER mplayer_t;

/* ��ͬƽ̨����ͬ�Ĳ��������� */
typedef struct tagMPLAYER_OPS mplayer_ops_t;

/* ������ʵ�� */
struct tagMPLAYER{
	char source[DEFAULT_SOURCE_SIZE];
	int volume;
	mplayer_opt_t *opt;
	mplayer_listener_t *listener;
	mplayer_status_enum status;
	pthread_t monitor_thread;
	mplayer_priv_t *priv;
	mplayer_ops_t *ops;
};


/* ��װ��ͬƽ̨�£��Բ�������ִ�е���ͬ�Ĳ��� */
struct tagMPLAYER_OPS
{
	/* ��mplayer���Ž��̲���ʼ���� */
	int(*mpops_open_player_process)(mplayer_t *mp);

	/* �ȴ�mplayer���Ž��̽��� */
	int(*mpops_wait_process_exit)(mplayer_t *mp);

	/* �رս��� */
	void(*mpops_close_player_process)(mplayer_t *mp);

	/* �ж�mplayer�����Ƿ��Ѿ��˳� */
	int(*mpops_process_is_exit)(mplayer_t *mp);

	/* �ͷ�mplayer������ռ�õ���Դ */
	void(*mpops_release_process_resource)(mplayer_t *mp);

	/* ��mplayer���Ž��̷�����Ϣ */
	int(*mpops_send_command)(mplayer_t *mp, const char *cmd, int size);

	/* ��mplayer���̶�ȡ���� */
	int(*mpops_read_data)(mplayer_t *mp, char *buff, int size);
};

API_EXPORT mplayer_t* mplayer_create_instance(mplayer_opt_t opt);
API_EXPORT void mplayer_free_instance(mplayer_t *mp);
API_EXPORT void mplayer_open(mplayer_t *mp, const char *source, int source_size);
API_EXPORT void mplayer_close(mplayer_t *mp);
API_EXPORT int mplayer_play(mplayer_t *mp);
API_EXPORT void mplayer_stop(mplayer_t *mp);
API_EXPORT int mplayer_pause(mplayer_t *mp);
API_EXPORT int mplayer_resume(mplayer_t *mp);
API_EXPORT void mplayer_increase_volume(mplayer_t *mp);
API_EXPORT void mplayer_decrease_volume(mplayer_t *mp);
/* ������ʱ��������Ϊ��λ */
API_EXPORT int mplayer_get_duration(mplayer_t *mp);
/* ���ص�ǰ���Ž��ȣ�����Ϊ��λ */
API_EXPORT int mplayer_get_position(mplayer_t *mp);
API_EXPORT void mplayer_listen_event(mplayer_t *mp, mplayer_listener_t listener);
API_EXPORT mplayer_status_enum mplayer_get_status(mplayer_t *mp);

#endif
