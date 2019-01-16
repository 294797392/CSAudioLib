using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAudioLib.AudioPlayer
{
    public interface IAudioPlayer
    {
        int Initialize();

        int Release();

        /// <summary>
        /// 打开音频源
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        int Open(IAudioSource source);

        /// <summary>
        /// 关闭音频源
        /// </summary>
        /// <returns></returns>
        int Close();

        int Play();

        int Stop();
    }
}