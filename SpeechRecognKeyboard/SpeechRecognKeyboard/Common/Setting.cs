using Newtonsoft.Json;
using SpeechRecognKeyboard.Model;
using SpeechRecognKeyboard.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechRecognKeyboard.Common
{
    public static class Setting
    {
        public static ObservableCollection<KeyItem> KeySetting 
        {
            get => JsonConvert.DeserializeObject<ObservableCollection<KeyItem>>(Settings.Default.KeySetting);
            set
            {
                Settings.Default.KeySetting = JsonConvert.SerializeObject(value);
                Settings.Default.Save();
            }
        }
    }
}
