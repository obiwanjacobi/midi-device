using System;
using System.Collections.Specialized;
using System.Windows;
using CannedBytes.Midi.Device.Message;

namespace CannedBytes.Midi.DeviceTestApp.Midi
{
    class MidiToLogicalProcess
    {
        private AppData appData;

        public MidiToLogicalProcess(AppData appData)
        {
            this.appData = appData;
            this.appData.SysExBuffers.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(SysExBuffers_CollectionChanged);
        }

        void SysExBuffers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (MidiSysExBuffer item in e.NewItems)
            {
                Process(item);
            }
        }

        private void Process(MidiSysExBuffer buffer)
        {
            try
            {
                var container = App.Current.Container;
                var deviceManager = container.GetExportedValue<DeviceManager>();
                var deviceProvider = deviceManager.Find(this.appData.DeviceSchemaData.CurrentSchema.Name);

                var dtMsgPair = deviceProvider.RootTypes.Find(this.appData.DeviceSchemaData.CurrentSchema.Name + ":DT1");

                var ctx = new MidiMessageDataContext(dtMsgPair);
                ctx.CompositionContainer = container;
                ctx.BinaryMap = deviceProvider.FindBinaryMap(dtMsgPair);
                ctx.PhysicalStream = buffer.Stream;

                ctx.ToLogical(this.appData.LogicalData);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error - To Logical", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}