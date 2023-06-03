using System;

namespace CannedBytes.Midi.SpeechController.Service
{
    internal class SpeechEventArgs : EventArgs
    {
        public SpeechEventArgs(string text)
        {
            _text = text;
        }

        private string _text;

        public string Text
        {
            get { return _text; }
        }
    }

    internal interface ISpeechRecognizerService
    {
        bool IsInstalled { get; }

        bool Enabled { get; set; }

        void Reset();

        void RegisterText(string text);

        event EventHandler<SpeechEventArgs> SpeechRecognized;
        event EventHandler<SpeechEventArgs> SpeechUnrecognized;
        event EventHandler<SpeechRecognizerStatesEventsArgs> StateChanged;
    }

    internal class SpeechRecognizerStatesEventsArgs : EventArgs
    {
        public SpeechRecognizerStatesEventsArgs(SpeechRecognizerStates state)
        {
            _state = state;
        }

        private SpeechRecognizerStates _state;

        public SpeechRecognizerStates State
        {
            get { return _state; }
        }
    }

    internal enum SpeechRecognizerStates
    {
        Stopped,
        Recognized,
        NotRecognized,
        Speech,
        NoSignal,
        TooFast,
        TooSlow,
        TooLoud,
        TooSoft,
        TooNoisy,
        Silence,
    }
}