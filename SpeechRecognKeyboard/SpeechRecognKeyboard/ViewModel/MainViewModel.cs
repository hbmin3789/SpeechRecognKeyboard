using Interceptor;
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
        private Dictionary<string, Keys> keyBoardSettings;

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
        public DelegateCommand CloseDialogCommand { get; set; }

        #endregion

        public MainViewModel()
        {
            InitVariables();
            InitCommands();
        }

        #region Initialize

        private void InitVariables()
        {
            keyBoardSettings = new Dictionary<string, Keys>();
        }

        private void InitCommands()
        {
            OpenKeySettingDialogCommand = new DelegateCommand<string>(OpenKeySetting);
            AddKeyCommand = new DelegateCommand(AddKey);
            CloseDialogCommand = new DelegateCommand(CloseDialog);
        }


        #endregion

        #region CommandMethods

        private void OpenKeySetting(string Key)
        {
            IsOpenDialog = true;
            SelectedKey = Key;
        }

        private void AddKey()
        {
            IsOpenDialog = false;

            Keys key;
            Enum.TryParse(SelectedKey, out key);

            if (keyBoardSettings.ContainsKey(SelectedKey))
            {
                keyBoardSettings[SelectedKey] = key;
                return;
            }

            keyBoardSettings.Add(SelectedKey, key);
        }

        private void CloseDialog()
        {
            IsOpenDialog = false;
        }

        #endregion
    }
}
