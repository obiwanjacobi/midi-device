using System;
using System.Collections.Generic;
using System.Speech.Recognition;

namespace CannedBytes.Midi.SpeechController.Service
{
    internal class SpeechRecognizerService : ISpeechRecognizerService, IDisposable
    {
        private SpeechRecognitionEngine _recognizer;
        private List<String> _phrases = new List<string>();

        public SpeechRecognizerService()
        {
            try
            {
                foreach (RecognizerInfo config in SpeechRecognitionEngine.InstalledRecognizers())
                {
                    _recognizer = new SpeechRecognitionEngine(config);
                    break;
                }

                Enabled = false;

                _recognizer.SetInputToDefaultAudioDevice();

                _recognizer.SpeechRecognized +=
                    new EventHandler<SpeechRecognizedEventArgs>(_recognizer_SpeechRecognized);
                _recognizer.SpeechRecognitionRejected +=
                    new EventHandler<SpeechRecognitionRejectedEventArgs>(_recognizer_SpeechRecognitionRejected);

                _recognizer.AudioSignalProblemOccurred +=
                    new EventHandler<AudioSignalProblemOccurredEventArgs>(_recognizer_AudioSignalProblemOccurred);
                _recognizer.AudioStateChanged +=
                    new EventHandler<AudioStateChangedEventArgs>(_recognizer_AudioStateChanged);

                _isInstalled = true;
            }
            catch
            {
                _isInstalled = false;
            }
        }

        private void _recognizer_AudioStateChanged(object sender, AudioStateChangedEventArgs e)
        {
            SpeechRecognizerStates state = SpeechRecognizerStates.Stopped;

            switch (e.AudioState)
            {
                case AudioState.Speech:
                    state = SpeechRecognizerStates.Speech;
                    break;
                case AudioState.Silence:
                    state = SpeechRecognizerStates.Silence;
                    break;
            }

            RaiseStateChangedEvent(state);
        }

        private void _recognizer_AudioSignalProblemOccurred(object sender, AudioSignalProblemOccurredEventArgs e)
        {
            SpeechRecognizerStates state = SpeechRecognizerStates.Stopped;

            switch (e.AudioSignalProblem)
            {
                case AudioSignalProblem.NoSignal:
                    state = SpeechRecognizerStates.NoSignal;
                    break;
                case AudioSignalProblem.TooFast:
                    state = SpeechRecognizerStates.TooFast;
                    break;
                case AudioSignalProblem.TooLoud:
                    state = SpeechRecognizerStates.TooLoud;
                    break;
                case AudioSignalProblem.TooNoisy:
                    state = SpeechRecognizerStates.TooNoisy;
                    break;
                case AudioSignalProblem.TooSlow:
                    state = SpeechRecognizerStates.TooSlow;
                    break;
                case AudioSignalProblem.TooSoft:
                    state = SpeechRecognizerStates.TooSoft;
                    break;
            }

            if (state != SpeechRecognizerStates.Stopped)
            {
                RaiseStateChangedEvent(state);
            }
        }

        private void _recognizer_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            if (Enabled)
            {
                RaiseSpeechUnrecognizedEvent(e.Result.Text);
            }
        }

        private void _recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (Enabled)
            {
                RaiseSpeechRecognizedEvent(e.Result.Text);
            }
        }

        #region ISpeechRecognizerService Members

        private bool _isInstalled;

        public bool IsInstalled
        {
            get { return _isInstalled; }
        }

        private bool _enabled;

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_recognizer == null) return;

                if (value)
                {
                    if (_phrases.Count > 0)
                    {
                        Choices choices = new Choices(_phrases.ToArray());
                        _recognizer.LoadGrammar(new Grammar(choices.ToGrammarBuilder()));

                        _recognizer.RecognizeAsync(RecognizeMode.Multiple);
                        _enabled = true;
                    }
                }
                else
                {
                    _recognizer.RecognizeAsyncCancel();
                    _recognizer.UnloadAllGrammars();

                    _enabled = false;
                }
            }
        }

        public void Reset()
        {
            //Enabled = false;
            _phrases.Clear();
        }

        public void RegisterText(string text)
        {
            _phrases.Add(text);
        }

        public event EventHandler<SpeechEventArgs> SpeechRecognized;

        protected void RaiseSpeechRecognizedEvent(string text)
        {
            var handler = SpeechRecognized;

            if (handler != null)
            {
                handler(this, new SpeechEventArgs(text));
            }
        }

        public event EventHandler<SpeechEventArgs> SpeechUnrecognized;

        protected void RaiseSpeechUnrecognizedEvent(string text)
        {
            var handler = SpeechUnrecognized;

            if (handler != null)
            {
                handler(this, new SpeechEventArgs(text));
            }
        }

        public event EventHandler<SpeechRecognizerStatesEventsArgs> StateChanged;

        protected void RaiseStateChangedEvent(SpeechRecognizerStates state)
        {
            var handler = StateChanged;

            if (handler != null)
            {
                handler(this, new SpeechRecognizerStatesEventsArgs(state));
            }
        }

        #endregion ISpeechRecognizerService Members

        #region IDisposable Members

        public void Dispose()
        {
            if (_recognizer != null)
            {
                _recognizer.Dispose();
                _recognizer = null;
            }

            _phrases = null;
        }

        #endregion IDisposable Members
    }
}