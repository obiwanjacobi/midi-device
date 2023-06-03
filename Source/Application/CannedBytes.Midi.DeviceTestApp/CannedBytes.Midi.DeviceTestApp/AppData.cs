using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Threading;
using CannedBytes.Midi.DeviceTestApp.Midi;
using CannedBytes.Midi.DeviceTestApp.UI;
using CannedBytes.Midi.DeviceTestApp.UI.Model;

namespace CannedBytes.Midi.DeviceTestApp
{
    internal class AppData : DisposableBase, INotifyPropertyChanged
    {
        private MidiToLogicalProcess toLogical;

        public AppData(Dispatcher dispatcher)
        {
            Dispatcher = dispatcher;

            MidiInPorts = new MidiInPortCapsCollection();
            MidiOutPorts = new MidiOutPortCapsCollection();

            SysExBuffers = new ObservableCollection<MidiSysExBuffer>();
            SysExReceiver = new MidiSysExReceiver(this);
            SysExSender = new MidiSysExSender();

            this.DeviceSchemaData = new MidiDeviceSchemaDataContext();
            this.toLogical = new MidiToLogicalProcess(this);
            this.LogicalData = new DeviceLogicalData();
        }

        public int CurrentFieldSize { get; set; }

        public SchemaField CurrentField { get; set; }

        public DeviceLogicalData LogicalData { get; private set; }

        public MidiDeviceSchemaDataContext DeviceSchemaData { get; private set; }

        public Dispatcher Dispatcher { get; private set; }

        public MidiInPortCapsCollection MidiInPorts { get; private set; }

        public MidiOutPortCapsCollection MidiOutPorts { get; private set; }

        public MidiInPortCaps SelectedMidiInPort { get; set; }

        public MidiOutPortCaps SelectedMidiOutPort { get; set; }

        public ObservableCollection<MidiSysExBuffer> SysExBuffers { get; private set; }

        public MidiSysExSender SysExSender { get; private set; }

        public MidiSysExReceiver SysExReceiver { get; private set; }

        private long recvErrCnt;

        public long ReceiveErrorCount
        {
            get { return this.recvErrCnt; }
            set
            {
                if (this.recvErrCnt != value)
                {
                    this.recvErrCnt = value;
                    OnPropertyChanged("ReceiveErrorCount");
                }
            }
        }

        #region INotifyPropertyChanged Members

        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged Members

        protected override void Dispose(DisposeObjectKind disposeKind)
        {
            this.SysExReceiver.Dispose();
            this.SysExSender.Dispose();
        }
    }
}