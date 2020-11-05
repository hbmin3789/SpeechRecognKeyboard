using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechRecognize.Core.Interfaces.Keyboard
{
    public interface IKeyboardManager
    {
        void PressKey();
        void ReleaseKey();
    }
}
