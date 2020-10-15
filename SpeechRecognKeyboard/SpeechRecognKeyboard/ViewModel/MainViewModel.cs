using Interceptor;
using Interceptor.Maple_STT;
using Microsoft.Speech.Recognition;
using Prism.Commands;
using Prism.Mvvm;
using SpeechRecognKeyboard.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechRecognKeyboard.ViewModel
{
    public class MainViewModel : BindableBase
    {
        private Dictionary<string, Keys> keyBoardSettings;
        private SpeechKeyboardManager keyboardManager;
        private SpeechRecognitionEngine speech;

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

        private ObservableCollection<KeyItem> _keyItems;
        public ObservableCollection<KeyItem> KeyItems
        {
            get => _keyItems;
            set => SetProperty(ref _keyItems, value);
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
            _keyItems = new ObservableCollection<KeyItem>();
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
                var keyItem = KeyItems.Where(x => x.Keys == keyBoardSettings[SelectedKey]).FirstOrDefault();
                keyItem.Keys = key;
                keyBoardSettings[SelectedKey] = key;
                return;
            }

            keyBoardSettings.Add(SelectedKey, key);
            KeyItems.Add(new KeyItem() { Keys = key, Speech = CurrentSpeech });
        }

        private void CloseDialog()
        {
            IsOpenDialog = false;
        }

        #endregion

        #region KeyboardManager

        public void OnDestroy()
        {
            keyboardManager.DestroyContext();
            keyboardManager.AbortKeyboardCapture();
        }

        public void GetKeyboardId()
        {
            keyboardManager.GetKeyboardId();
            keyboardManager.StartKeyboardCapture();
        }

        #endregion

        #region STT

        public void StartSTT()
        {
            try
            {
                speech = new SpeechRecognitionEngine();
                Choices choices = GetChoicesFromList(KeyItems);
                GrammarBuilder gb = new GrammarBuilder();
                gb.Append(choices);

                Grammar g = new Grammar(gb);

                speech.LoadGrammar(g);
                speech.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(Speech_SpeechRecognized);
                speech.SetInputToDefaultAudioDevice();
                speech.RecognizeAsync(RecognizeMode.Multiple);
            }
            catch (Exception e)
            {
                string msg = e.Message;
            }
        }

        public void StopSTT()
        {
            speech.RecognizeAsyncStop();
        }

        private Choices GetChoicesFromList(ObservableCollection<KeyItem> skillKeyItems)
        {
            Choices retval = new Choices();
            for (int i = 0; i < skillKeyItems.Count; i++)
            {
                retval.Add(skillKeyItems[i].Speech);
            }

            return retval;
        }

        private void Speech_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            var keyItem = KeyItems.Where(x => x.Speech.Equals(e.Result.Text)).FirstOrDefault();
            keyboardManager.PressKey(keyItem.Keys);
        }

        #endregion
    }
}
