﻿using Interceptor;
using Interceptor.Maple_STT;
using Microsoft.Speech.Recognition;
using Prism.Commands;
using Prism.Mvvm;
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
        private Dictionary<string, Keys> keyBoardSettings;
        private SpeechKeyboardManager keyboardManager;
        private SpeechRecognitionEngine speech;

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
        }

        private void InitCommands()
        {
            OpenKeySettingDialogCommand = new DelegateCommand<string>(OpenKeySetting);
            AddKeyCommand = new DelegateCommand(AddKey);
            CloseDialogCommand = new DelegateCommand(CloseDialog);
            OnContentRenderedCommand = new DelegateCommand(OnContentRendered);
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
            keyboardManager.GetKeyboardId();
            keyboardManager.StartKeyboardCapture();
            MainWindowEnabled = false;
        }

        private void OpenKeySetting(string Key)
        {
            StopSTT();
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
            StartSTT();
            SaveSetting();
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

        private void SaveSetting()
        {
            Setting.KeySetting = KeyItems;
        }
    }
}
