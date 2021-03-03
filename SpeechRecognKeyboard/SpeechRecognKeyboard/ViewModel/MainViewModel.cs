using Interceptor;
using Microsoft.Speech.Recognition;
using Prism.Commands;
using Prism.Mvvm;
using SpeechRecognize.Core;
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
        private Input keyboardManager;
        private SpeechManager speechManager;

        #endregion

        #region Property
        private bool _isOpenSpeechDialog;
        public bool IsOpenSpeechDialog
        {
            get => _isOpenSpeechDialog;
            set => SetProperty(ref _isOpenSpeechDialog, value);
        }

        private bool _isOpenSpeechManager;
        public bool IsOpenSpeechManager
        {
            get => _isOpenSpeechManager;
            set => SetProperty(ref _isOpenSpeechManager, value);
        }

        private bool _isSpeechStart;
        public bool IsSpeechStart
        {
            get => _isSpeechStart;
            set => SetProperty(ref _isSpeechStart, value);
        }

        private bool _mainWindowEnabled = false;
        public bool MainWindowEnabled
        {
            get => !_mainWindowEnabled;
            set => SetProperty(ref _mainWindowEnabled, value);
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
        public DelegateCommand OpenSpeechManagerCommand { get; set; }
        public DelegateCommand<string> OpenKeySettingDialogCommand { get; set; }
        public DelegateCommand AddKeyCommand { get; set; }
        public DelegateCommand CloseKeyManagerDialogCommand { get; set; }
        public DelegateCommand CloseKeySettingDialogCommand { get; set; }


        public DelegateCommand StartRecognizeCommand { get; set; }

        public DelegateCommand OnContentRenderedCommand { get; set; }

        #endregion

        public MainViewModel()
        {
            InitVariables();
            InitCommands();
            InitKeyItems();
            InitSpeech();
        }

        ~MainViewModel()
        {
            keyboardManager.Unload();
            speechManager.StopSTT();
        }

        #region Initialize

        private void InitVariables()
        {
            keyBoardSettings = new Dictionary<string, Keys>();
            _keyItems = new ObservableCollection<KeyItem>();
            speechManager = new SpeechManager();
            keyboardManager = new Input();
        }

        private void InitCommands()
        {
            OpenSpeechManagerCommand = new DelegateCommand(OpenSpeechManager);
            OpenKeySettingDialogCommand = new DelegateCommand<string>(OpenKeySetting);
            AddKeyCommand = new DelegateCommand(AddKey);
            CloseKeyManagerDialogCommand = new DelegateCommand(CloseKeyManagerDialog);
            OnContentRenderedCommand = new DelegateCommand(OnContentRendered);
            CloseKeySettingDialogCommand = new DelegateCommand(CloseKeySettingDialog);

            StartRecognizeCommand = new DelegateCommand(StartRecognize);
        }


        private void InitSpeech()
        {
            speechManager.SetOnRecognized((s, e) =>
            {
                if (keyBoardSettings.ContainsKey(e.Result.Text))
                {
                    keyboardManager.SendKey(keyBoardSettings[e.Result.Text], KeyState.Down);
                    keyboardManager.SendKey(keyBoardSettings[e.Result.Text], KeyState.Up);
                }
            });
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

        private void OpenSpeechManager()
        {
            IsOpenSpeechManager = !IsOpenSpeechManager;
        }

        private void OnContentRendered()
        {
            //keyboardManager.KeyboardFilterMode = KeyboardFilterMode.All;
            //keyboardManager.MouseFilterMode = MouseFilterMode.All;
            //if (keyboardManager.Load())
            //{
            //    MainWindowEnabled = true;
            //}
            //else 
            //{
            //    throw new Exception("드라이버를 확인해주세요.");
            //}
        }

        private void OpenKeySetting(string Key)
        {
            IsOpenSpeechDialog = true;
            SelectedKey = Key;
        }

        private void StartRecognize()
        {
            IsSpeechStart = !IsSpeechStart;
            if (IsSpeechStart)
            {
                speechManager.StartSTT(keyBoardSettings.Keys);
            }
            else
            {
                speechManager.StopSTT();
            }
        }

        private void AddKey()
        {
            IsOpenSpeechDialog = false;

            Keys key;
            Enum.TryParse(SelectedKey, out key);

            if (keyBoardSettings.ContainsKey(SelectedKey))
            {
                var keyItem = KeyItems.Where(x => x.Keys == keyBoardSettings[SelectedKey]).FirstOrDefault();
                keyItem.Keys = key;
                keyBoardSettings[SelectedKey] = key;
                return;
            }

            keyBoardSettings.Add(CurrentSpeech, key);
            KeyItems.Add(new KeyItem() { Keys = key, Speech = CurrentSpeech });

            SaveSetting();
        }

        private void CloseKeyManagerDialog()
        {
            IsOpenSpeechDialog = false;
            CurrentSpeech = string.Empty;
            SaveSetting();
        }

        private void CloseKeySettingDialog()
        {
            IsOpenSpeechManager = false;
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
