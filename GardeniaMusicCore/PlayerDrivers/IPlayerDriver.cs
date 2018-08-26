using GMusicCore.AudioSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GMusicCore.PlayerDrivers
{
    public interface IPlayerDriver
    {
        /// <summary>
        /// 回调事件
        /// </summary>
        event Action<object, PlayerDriverEventType, double> EventCallback;

        int Volume { get; }

        PlayerDriverStatus Status { get; }

        bool Initialize();

        void Release();

        int Play(IAudioSource source);

        void Stop();

        void Pause();

        void Resume();

        /// <summary>
        /// 获取总时长
        /// </summary>
        /// <returns></returns>
        int GetDuration();

        /// <summary>
        /// 设置音量大小（1-100）
        /// </summary>
        /// <param name="volume"></param>
        void SetVolume(int volume);
    }
}