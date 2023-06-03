using System;

using CannedBytes.Midi.SpeechController.DomainModel;

namespace CannedBytes.Midi.SpeechController.Service
{
    internal interface IPatchExecuter
    {
        void Prepare(Preset preset);

        void Execute(Patch patch);

        event EventHandler<NotifyProgressEventArg> NotifyProgress;
    }

    public class NotifyProgressEventArg : EventArgs
    {
        public NotifyProgressEventArg(int total, int current)
        {
            Total = total;
            Current = current;
        }

        public int Total { get; private set; }

        public int Current { get; private set; }
    }
}