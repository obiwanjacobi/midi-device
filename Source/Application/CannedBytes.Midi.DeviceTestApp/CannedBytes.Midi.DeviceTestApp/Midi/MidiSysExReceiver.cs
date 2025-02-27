﻿using System;
using System.Diagnostics;

namespace CannedBytes.Midi.DeviceTestApp.Midi
{
    class MidiSysExReceiver : DisposableBase, IMidiDataReceiver, IMidiDataErrorReceiver
    {
        private AppData appData;
        private MidiInPort inPort;

        public MidiSysExReceiver(AppData appData)
        {
            this.appData = appData;
            this.inPort = new MidiInPort();

            this.inPort.Successor = this;
            // also hookup error notification
            this.inPort.NextErrorReceiver = this;

            this.inPort.BufferManager.Initialize(10, 1024);
        }

        public void Start(int portId)
        {
            this.inPort.Open(portId);
            this.inPort.Start();
        }

        public void Stop()
        {
            this.inPort.Stop();
            this.inPort.Close();
        }

        private void ScheduleAddBuffer(MidiSysExBuffer buffer)
        {
            this.appData.Dispatcher.Invoke(new Action(() => DispatchedAddBuffer(buffer)));
        }

        private void DispatchedAddBuffer(MidiSysExBuffer buffer)
        {
            this.appData.SysExBuffers.Add(buffer);
        }

        #region IMidiDataReceiver Members

        public void LongData(MidiBufferStream buffer, long timestamp)
        {
            Trace.WriteLine("Receiving buffer: " + buffer.BytesRecorded);

            var sysExBuffer = MidiSysExBuffer.From(buffer);

            ScheduleAddBuffer(sysExBuffer);
        }

        public void ShortData(int data, long timestamp)
        {
            // no op
        }

        #endregion IMidiDataReceiver Members

        #region IMidiDataErrorReceiver Members

        public void LongError(MidiBufferStream buffer, long timestamp)
        {
            this.appData.ReceiveErrorCount++;
        }

        public void ShortError(int data, long timestamp)
        {
        }

        #endregion IMidiDataErrorReceiver Members

        protected override void Dispose(DisposeObjectKind disposeKind)
        {
            this.inPort.Dispose();
        }
    }
}