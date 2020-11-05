using Interceptor;
using Interceptor.Maple_STT;
using Microsoft.Speech.Recognition;
using Prism.Commands;
using Prism.Mvvm;
using SpeechRecognize.Core;
using SpeechRecognize.Core.Interfaces.Keyboard;
using SpeechRecognKeyboard.Common;
using SpeechRecognKeyboard.Model;
using SpeechRecognKeyboard.Properties;
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
        #region Member

        private Dictionary<string, Keys> keyBoardSettings;
        private IKeyboardManager keyboardManager;
        private SpeechManager speechManager;

        #endregion

        #region Property

        private bool _mainWindowEnabled;
        public bool MainWindowEnabled
        {
            get => _mainWindowEnabled;
            set => SetProperty(ref _mainWindowEnabled, value);
        }

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

        public DelegateCommand OnContentRenderedCommand { get; set; }

        public DelegateCommand OnSTTStartCommand { get; set; }

        #endregion

        public MainViewModel()
        {
            InitVariables();
            InitCommands();
            InitKeyItems();
        }

        #region Initialize

        private void InitVariables()
        {
            keyBoardSettings = new Dictionary<string, Keys>();
            _keyItems = new ObservableCollection<KeyItem>();
            speechManager = new SpeechManager();
        }

        private void InitCommands()
        {
            OpenKeySettingDialogCommand = new DelegateCommand<string>(OpenKeySetting);
            AddKeyCommand = new DelegateCommand(AddKey);
            CloseDialogCommand = new DelegateCommand(CloseDialog);
            OnContentRenderedCommand = new DelegateCommand(OnContentRendered);
            OnSTTStartCommand = new DelegateCommand(OnSTTStart);
        }

        private void InitSpeechManager()
        {
            speechManager.SetOnRecognized((sender, e) => 
            {
                var item = KeyItems.Where(x => x.Speech == e.Result.Text).FirstOrDefault();
                if(item != null)
                {
                    //item.Keys
                    keyboardManager.PressKey();
                }
            });
        }

        private void OnSTTStart()
        {
            if (speechManager.IsSpeechRecognizing)
            {
                speechManager.StopSTT();
            }
            else
            {
                List<string> speechList = new List<string>();
                KeyItems.ToList().ForEach((x) => 
                {
                    speechList.Add(x.Speech);
                });

                speechManager.StartSTT(speechList);
            }
        }

        private void InitKeyItems()
        {
            KeyItems = Setting.KeySetting;
            if(KeyItems.Count == 0)
            {
                var values = Enum.GetNames(typeof(Keys));
                foreach (var value in values)
                {
                    string name = "";
                    switch (value)
                    {
                        case "OpenBracketBrace":
                            name = "[";
                            break;
                        case "CloseBracketBrace":
                            name = "]";
                            break;
                        case "SemicolonColon":
                            name = ";";
                            break;
                        case "One":
                            name = 1.ToString();
                            break;
                        case "Two":
                            name = 2.ToString();
                            break;
                        case "Three":
                            name = 3.ToString();
                            break;
                        case "Four":
                            name = 4.ToString();
                            break;
                        case "Five":
                            name = 5.ToString();
                            break;
                        case "Six":
                            name = 6.ToString();
                            break;
                        case "Seven":
                            name = 7.ToString();
                            break;
                        case "Eight":
                            name = 8.ToString();
                            break;
                        case "Nine":
                            name = 9.ToString();
                            break;
                        case "Zero":
                            name = 0.ToString();
                            break;
                        case "Tilde":
                            name = "`";
                            break;
                        default:
                            name = value;
                            break;
                    }
                    KeyItems.Add(new KeyItem() { Keys = (Keys)Enum.Parse(typeof(Keys), value), KeyDisplay = name });
                }
                Setting.KeySetting = KeyItems;
            }
            
        }


        #endregion

        #region CommandMethods

        private void OnContentRendered()
        {
            //keyboardManager.GetKeyboardId();
            //keyboardManager.StartKeyboardCapture();
            MainWindowEnabled = true;
        }

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

            SaveSetting();
        }

        private void CloseDialog()
        {
            IsOpenDialog = false;
            SaveSetting();
        }

        #endregion

        #region KeyboardManager

        public void OnDestroy()
        {
            //keyboardManager.DestroyContext();
            //keyboardManager.AbortKeyboardCapture();
        }

        public void GetKeyboardId()
        {
            //keyboardManager.GetKeyboardId();
            //keyboardManager.StartKeyboardCapture();
        }

        #endregion

        #region Setting

        private void SaveSetting()
        {
            Setting.KeySetting = KeyItems;
        }

        #endregion

        
    }
}
