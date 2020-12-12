using Microsoft.Speech.Recognition;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechRecognize.Core
{
    public class SpeechManager
    {
        private SpeechRecognitionEngine speech;
<<<<<<< HEAD
        private Action<object, SpeechRecognizedEventArgs> _onRecognized;
=======
        private bool _isSpeechRecognizing = false;
        public bool IsSpeechRecognizing
        {
            get => _isSpeechRecognizing;
            set => _isSpeechRecognizing = value;
        }

>>>>>>> 6319f0a19a9a5e546d31195c75989599b6b22675

        public SpeechManager()
        {
            speech = new SpeechRecognitionEngine();
        }

        public void SetOnRecognized(Action<object, SpeechRecognizedEventArgs> onRecognized)
        {
            _onRecognized += onRecognized;
        }

        #region STT

        public void StartSTT(IEnumerable<string> speechList)
        {
            try
            {
                speech = new SpeechRecognitionEngine();
                speech.SpeechRecognized += (s, e) =>
                {
                    _onRecognized?.Invoke(s, e);
                };
                Choices choices = StringListToChoices(speechList);
                GrammarBuilder gb = new GrammarBuilder();
                gb.Append(choices);

                Grammar g = new Grammar(gb);

                speech.LoadGrammar(g);                
                speech.SetInputToDefaultAudioDevice();
                speech.RecognizeAsync(RecognizeMode.Multiple);
            }
            catch (Exception e)
            {
                string msg = e.Message;
            }
        }

        private Choices StringListToChoices(IEnumerable<string> speechList)
        {
            Choices choices = new Choices();

            speechList.ToList().ForEach((x) => 
            {
                choices.Add(x);
            });

            return choices;
        }

        public void StopSTT()
        {
            speech.RecognizeAsyncStop();
        }

        #endregion

    }
}
