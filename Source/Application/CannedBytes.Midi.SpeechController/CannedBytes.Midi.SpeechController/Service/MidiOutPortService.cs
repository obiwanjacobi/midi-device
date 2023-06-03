using System.Collections.Generic;
using System.Linq;
using CannedBytes.Midi.Components;
using CannedBytes.Midi.Message;

namespace CannedBytes.Midi.SpeechController.Service
{
    internal class MidiOutPortService : IMidiOutPortService
    {
        private Dictionary<int, IMidiOutPort> _openPorts =
            new Dictionary<int, IMidiOutPort>();

        public const short MaxSysExBufferSize = 2048;

        #region IMidiOutPortService Members

        MidiOutPortCaps[] _capabilities;

        public MidiOutPortCaps[] Capabilities
        {
            get
            {
                if (_capabilities == null)
                {
                    MidiOutPortCapsCollection caps = new MidiOutPortCapsCollection();
                    _capabilities = (from cap in caps
                                     select cap).ToArray();
                }

                return _capabilities;
            }
        }

        public IMidiOutPort Open(int portIndex)
        {
            if (!_openPorts.ContainsKey(portIndex))
            {
                MidiOutPortImpl port = new MidiOutPortImpl(portIndex);

                _openPorts[portIndex] = port;
            }

            return _openPorts[portIndex];
        }

        public void CloseAll()
        {
            foreach (IMidiOutPort port in _openPorts.Values)
            {
                try
                {
                    port.Close();
                }
                catch
                {
                    // TODO: log error
                }
            }

            _openPorts.Clear();
        }

        #endregion IMidiOutPortService Members

        #region IDisposable Members

        public void Dispose()
        {
            foreach (IMidiOutPort port in _openPorts.Values)
            {
                port.Dispose();
            }

            _openPorts.Clear();
        }

        #endregion IDisposable Members

        private class MidiOutPortImpl : IMidiOutPort
        {
            MidiOutPort _port;
            MidiOutPortChainManager _chainMgr;

            public MidiOutPortImpl(int portId)
            {
                _port = new MidiOutPort();
                _chainMgr = new MidiOutPortChainManager(_port);

                _port.Open(portId);

                _chainMgr.Initialize(4, MaxSysExBufferSize);
            }

            #region IMidiOutPort Members

            public MidiOutPortCaps Capabilities
            {
                get { return _port.Capabilities; }
            }

            public void Send(MidiMessage message)
            {
                MidiShortMessage shortMsg = message as MidiShortMessage;
                MidiLongMessage longMsg = message as MidiLongMessage;

                if (shortMsg != null)
                {
                    _chainMgr.Sender.ShortData(shortMsg.Data);
                }

                if (longMsg != null)
                {
                    MidiBufferStream buffer = _port.BufferManager.RetrieveBuffer();

                    buffer.Write(longMsg.GetData(), 0, longMsg.ByteLength);

                    _chainMgr.Sender.LongData(buffer);
                }
            }

            public void Close()
            {
                _port.Close();
                _chainMgr.Dispose();
            }

            #endregion IMidiOutPort Members

            #region IDisposable Members

            public void Dispose()
            {
                Close();
            }

            #endregion IDisposable Members
        }
    }
}