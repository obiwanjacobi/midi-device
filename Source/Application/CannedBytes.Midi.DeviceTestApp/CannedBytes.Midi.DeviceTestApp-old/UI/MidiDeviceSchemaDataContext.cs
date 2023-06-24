using System.Collections.Generic;
using System.ComponentModel;
using CannedBytes.Midi.Device.Schema;
using CannedBytes.Midi.DeviceTestApp.Midi;
using CannedBytes.Midi.DeviceTestApp.UI.Model;

namespace CannedBytes.Midi.DeviceTestApp.UI
{
    class MidiDeviceSchemaDataContext : INotifyPropertyChanged
    {
        public IEnumerable<DeviceSchema> AllSchemas
        {
            get
            {
                var deviceMgr = App.Current.Container.GetExportedValue<DeviceManager>();
                return deviceMgr.Schemas;
            }
        }

        private DeviceSchema currentSchema;

        public DeviceSchema CurrentSchema
        {
            get { return this.currentSchema; }
            set
            {
                if (this.currentSchema != value)
                {
                    this.currentSchema = value;
                    OnPropertyChanged("CurrentSchema");
                    OnPropertyChanged("Model");
                }
            }
        }

        public SchemaModel Model
        {
            get { return this.currentSchema == null ? null : new SchemaModel(this.currentSchema); }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}