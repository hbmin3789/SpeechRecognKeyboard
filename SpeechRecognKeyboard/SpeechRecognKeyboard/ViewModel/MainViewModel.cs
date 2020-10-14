using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechRecognKeyboard.ViewModel
{
    public class MainViewModel : BindableBase
    {
        #region Property

        private bool _isOpenDialog = false;
        public bool IsOpenDialog
        {
            get => _isOpenDialog;
            set => SetProperty(ref _isOpenDialog, value);
        }

        private string _selectedKey;
        public string SelectedKey
        {
            get => _selectedKey;
            set => SetProperty(ref _selectedKey, value);
        }

        private string _currentSpeech;
        public string CurrentSpeech
        {
            get => _currentSpeech;
            set => SetProperty(ref _currentSpeech, value);
        }

        #endregion

        #region Commands

        public DelegateCommand<string> OpenKeySettingDialogCommand { get; set; }
        public DelegateCommand AddKeyCommand { get; set; }

        #endregion

        public MainViewModel()
        {
            InitCommands();
        }

        #region Initialize

        private void InitCommands()
        {
            OpenKeySettingDialogCommand = new DelegateCommand<string>(OpenKeySetting);
        }

        #endregion

        #region CommandMethods

        private void OpenKeySetting(string Key)
        {
            IsOpenDialog = true;
            SelectedKey = Key;
        }

        #endregion
    }
}
