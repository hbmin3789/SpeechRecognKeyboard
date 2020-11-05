using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechRecognKeyboard.Model
{
    public class KeyPress : BindableBase
    {
        private ObservableCollection<KeyItem> _keyItems;
        public ObservableCollection<KeyItem> KeyItems
        {
            get => _keyItems;
            set => SetProperty(ref _keyItems, value);
        }

    }
}
