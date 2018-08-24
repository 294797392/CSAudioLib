using GMusicCore.AudioSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GMusicCore.PlayerDrivers
{
    public interface IPlayerDriver
    {
        event Action<object, double> Progress;

        bool Initialize();

        void Release();

        int Play(IAudioSource source);

        void Stop();

        void Pause();

        void Resume();
    }
}