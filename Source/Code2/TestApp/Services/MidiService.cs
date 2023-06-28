using System;
using System.Collections.Generic;
using System.Linq;
using RtMidi.Core;
using RtMidi.Core.Devices;
using RtMidi.Core.Messages;

namespace TestApp.Services
{
    internal interface IMidiPort : IDisposable
    {
        string Name { get; }
        bool IsOpen { get; }
        void Open();
        void Close();
    }

    internal interface IMidiInPort : IMidiPort
    {
        event EventHandler<MidiMessage> MidiMessage;
    }

    internal interface IMidiOutPort : IMidiPort
    {
    }

    internal readonly struct MidiMessage
    {
        public MidiMessage(byte[] data)
            => Data = data;

        public byte[] Data { get; }
    }

    internal class MidiService
    {
        public IEnumerable<string> GetMidiInPorts()
        {
            return MidiDeviceManager.Default.InputDevices.Select(indev => indev.Name);
        }

        public IEnumerable<string> GetMidiOutPorts()
        {
            return MidiDeviceManager.Default.OutputDevices.Select(indev => indev.Name);
        }

        public IMidiInPort CreateMidiInPort(string name)
        {
            var inputDeviceInfo = MidiDeviceManager.Default.InputDevices.FirstOrDefault(indev => indev.Name == name)
                ?? throw new ArgumentException($"The Midi InPort '{name}' was not found.");
            
            var inputDevice = inputDeviceInfo.CreateDevice();
            return new MidiInPort(inputDevice);
        }

        public IMidiOutPort CreateMidiOutPort(string name)
        {
            var outputDeviceInfo = MidiDeviceManager.Default.OutputDevices.FirstOrDefault(indev => indev.Name == name)
                ?? throw new ArgumentException($"The Midi OutPort '{name}' was not found.");

            var outputDevice = outputDeviceInfo.CreateDevice();
            return new MidiOutPort(outputDevice);
        }

        private sealed class MidiOutPort : IMidiOutPort
        {
            private readonly IMidiOutputDevice _midiOutDevice;

            public MidiOutPort(IMidiOutputDevice midiOutDevice)
                => _midiOutDevice = midiOutDevice;

            public string Name => _midiOutDevice.Name;

            public bool IsOpen => _midiOutDevice.IsOpen;

            public void Open() => _midiOutDevice.Open();

            public void Close() => _midiOutDevice.Close();

            public void Dispose()
            {
                if (IsOpen)
                {
                    Close();
                }

                _midiOutDevice.Dispose();
            }
        }

        private sealed class MidiInPort : IMidiInPort
        {
            private readonly IMidiInputDevice _midiInDevice;

            public MidiInPort(IMidiInputDevice midiInDevice)
            {
                _midiInDevice = midiInDevice;
                _midiInDevice.SysEx += OnSystemExclusive;
            }

            private void OnSystemExclusive(IMidiInputDevice sender, in SysExMessage msg)
            {
                MidiMessage?.Invoke(this, new MidiMessage(msg.Data));
            }

            public event EventHandler<MidiMessage>? MidiMessage;

            public void Dispose()
            {
                if (IsOpen)
                {
                    Close();
                }

                _midiInDevice.SysEx -= OnSystemExclusive;
                _midiInDevice.Dispose();
            }

            public string Name => _midiInDevice.Name;

            public bool IsOpen => _midiInDevice.IsOpen;

            public void Open() => _midiInDevice.Open();

            public void Close() => _midiInDevice.Close();
        }
    }
}
