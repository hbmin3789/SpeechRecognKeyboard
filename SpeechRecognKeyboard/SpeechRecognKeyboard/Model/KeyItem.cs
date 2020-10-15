using Interceptor;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechRecognKeyboard.Model
{
    public class KeyItem : BindableBase
    {
        private Keys _keys;
        public Keys Keys
        {
            get => _keys;
            set => SetProperty(ref _keys, value);
        }

        private string _speech;
        public string Speech
        {
            get => _speech;
            set => SetProperty(ref _speech, value);
        }

        private string _keyDisplay;
        public string KeyDisplay
        {
            get => _keyDisplay;
            set => SetProperty(ref _keyDisplay, value);
        }

        public KeyItem()
        {

        }
    }
}
