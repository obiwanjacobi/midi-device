using System;
using System.Windows.Input;
using CannedBytes.Midi.Device.Message;
using CannedBytes.Midi.Device.Schema;
using CannedBytes.Midi.DeviceTestApp.Midi;

namespace CannedBytes.Midi.DeviceTestApp.Commands
{
    class SendDataSetCommandHandler : CommandHandler
    {
        private AppData appData;

        public SendDataSetCommandHandler(AppData appData)
        {
            this.appData = appData;
            Command = AppCommands.SendDataSet;
        }

        protected override void CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            base.CanExecute(sender, e);

            e.CanExecute = e.CanExecute &&
                this.appData.LogicalData != null &&
                this.appData.LogicalData.Values.Count > 0;
        }

        protected override void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            SendData(this.appData.LogicalData);

            base.Execute(sender, e);
        }

        private void SendData(DeviceLogicalData deviceLogicalData)
        {
            string schemaName;

            if (this.appData.DeviceSchemaData.CurrentSchema != null)
            {
                schemaName = this.appData.DeviceSchemaData.CurrentSchema.Name;
            }
            else
            {
                throw new InvalidOperationException();
            }

            var deviceMgr = App.Current.Container.GetExportedValue<DeviceManager>();
            var deviceProvider = deviceMgr.Find(schemaName);

            var dtMsgPair = deviceProvider.RootTypes.Find(schemaName + ":DT1");
            var binaryMap = deviceProvider.FindBinaryMap(dtMsgPair);

            var ctx = new MidiMessageDataContext(dtMsgPair);
            ctx.CompositionContainer = App.Current.Container;
            ctx.BinaryMap = binaryMap;

            MidiSysExBuffer buffer = new MidiSysExBuffer();
            ctx.PhysicalStream = buffer.Stream;

            if (this.appData.CurrentFieldSize > 0)
            {
                ctx.DeviceProperties.Add(Constants.MidiTypesNamespace, "size", this.appData.CurrentFieldSize);
            }

            ctx.ToPhysical(deviceLogicalData);

            this.appData.SysExSender.Send(buffer);
        }
    }
}