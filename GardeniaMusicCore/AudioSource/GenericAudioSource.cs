using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GMusicCore.AudioSource
{
    internal class GenericAudioSource : IAudioSource
    {
        public override AudioSourceEnum Type
        {
            get
            {
                return AudioSourceEnum.Generic;
            }
        }
    }
}