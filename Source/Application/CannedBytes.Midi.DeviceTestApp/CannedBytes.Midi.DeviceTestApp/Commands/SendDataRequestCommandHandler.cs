using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CannedBytes.Midi.Device;
using CannedBytes.Midi.Device.Converters;
using CannedBytes.Midi.Device.Message;
using CannedBytes.Midi.Device.Schema;
using CannedBytes.Midi.DeviceTestApp.Midi;
using CannedBytes.Midi.DeviceTestApp.UI;
using CannedBytes.Midi.DeviceTestApp.UI.Model;

namespace CannedBytes.Midi.DeviceTestApp.Commands
{
    class SendDataRequestCommandHandler : CommandHandler
    {
        private AppData appData;

        public SendDataRequestCommandHandler(AppData appData)
        {
            this.appData = appData;
            Command = AppCommands.SendDataRequest;
        }

        protected override void CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            base.CanExecute(sender, e);
            var field = e.Parameter as SchemaField;

            e.CanExecute = e.CanExecute & field != null;
        }

        protected override void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var field = e.Parameter as SchemaField;

            if (field != null)
            {
                Execute(field);
                e.Handled = true;
            }

            base.Execute(sender, e);
        }

        private void Execute(SchemaField field)
        {
            var deviceMgr = App.Current.Container.GetExportedValue<DeviceManager>();
            var deviceProvider = deviceMgr.Find(field.Field.Schema.Name);

            var rqMsgPair = deviceProvider.RootTypes.Find(field.Field.Name.SchemaName + ":RQ1");
            var dtMsgPair = deviceProvider.RootTypes.Find(field.Field.Name.SchemaName + ":DT1");
            var binaryMap = deviceProvider.FindBinaryMap(dtMsgPair);
            var converterMgr = App.Current.Container.GetExportedValue<ConverterManager>();
            var schemaProvider = App.Current.Container.GetExportedValue<IDeviceSchemaProvider>();
            var msgTypeFactory = new MessageTypeFactory(converterMgr, binaryMap, schemaProvider);

            var key = PromptInstanceIndex(field);
            if (key == null) return;

            SevenBitUInt32 address;
            SevenBitUInt32 size;

            if (msgTypeFactory.FindAddressRange(field.Field, key, field.Field, key, out address, out size))
            {
                try
                {
                    var ctx = new MidiMessageDataContext(rqMsgPair);
                    ctx.CompositionContainer = App.Current.Container;
                    ctx.BinaryMap = binaryMap;

                    ctx.DeviceProperties.Add(Constants.MidiTypesNamespace, "address", address);
                    ctx.DeviceProperties.Add(Constants.MidiTypesNamespace, "size", size);

                    MidiSysExBuffer buffer = new MidiSysExBuffer();
                    ctx.PhysicalStream = buffer.Stream;

                    ctx.ToPhysical(new DeviceLogicalReader());

                    this.appData.LogicalData.Values.Clear();
                    this.appData.CurrentFieldSize = size;
                    this.appData.CurrentField = field;
                    this.appData.SysExSender.Send(buffer);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString(), "Error - To Physical", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("No address and size could be determined.", "Error - To Physical", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private FieldPathKey PromptInstanceIndex(SchemaField field)
        {
            int count = field.MultiFields.Count();
            bool? result = true;

            if (count > 0)
            {
                var editCtrl = new EditFieldKeyPathControl();
                var frame = new WindowFrame();
                frame.DataContext = field;

                frame.ContentPlaceholder.Content = editCtrl;
                result = frame.ShowDialog();
            }

            if (result.GetValueOrDefault())
            {
                var key = new FieldPathKey();

                foreach (var fld in field.ParentFields)
                {
                    key.Add(fld.InstanceIndex);
                }

                return key;
            }

            return null;
        }
    }
}