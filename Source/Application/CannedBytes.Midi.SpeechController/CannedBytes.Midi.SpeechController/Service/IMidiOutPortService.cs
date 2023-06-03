using System;
using CannedBytes.Midi.Message;

namespace CannedBytes.Midi.SpeechController.Service
{
    internal interface IMidiOutPortService : IDisposable
    {
        MidiOutPortCaps[] Capabilities { get; }

        IMidiOutPort Open(int portIndex);

        void CloseAll();
    }

    internal interface IMidiOutPort : IDisposable
    {
        MidiOutPortCaps Capabilities { get; }

        void Send(MidiMessage message);

        void Close();
    }
}