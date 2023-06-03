using CannedBytes.Midi.Core;
using CannedBytes.Midi.Device.Converters;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Message
{
    public class AddressMapGroupConverter : DynamicGroupConverter
    {
        private FieldConverterMap orginalMap;

        public AddressMapGroupConverter(RecordType recordType)
            : base(recordType)
        {
        }

        public override void ToLogical(MidiDeviceDataContext context, IMidiLogicalWriter writer)
        {
            var msgCtx = context as MidiMessageDataContext;

            if (msgCtx != null)
            {
                var converter = Initialize(msgCtx);
                Activate(converter);
            }

            base.ToLogical(context, writer);

            Deactivate();
        }

        public override void ToPhysical(MidiDeviceDataContext context, IMidiLogicalReader reader)
        {
            var msgCtx = context as MidiMessageDataContext;

            if (msgCtx != null)
            {
                var converter = Initialize(msgCtx);
                Activate(converter);
            }

            base.ToPhysical(context, reader);

            Deactivate();
        }

        private void Deactivate()
        {
            this.FieldConverterMap = this.orginalMap;
        }

        private void Activate(GroupConverter converter)
        {
            this.orginalMap = this.FieldConverterMap;
            this.FieldConverterMap = converter.FieldConverterMap;
        }

        private GroupConverter Initialize(MidiMessageDataContext context)
        {
            Check.IfArgumentNull(context, "context");
            Check.IfArgumentNull(context.CompositionContainer, "context.CompositionContainer");
            var converterMgr = context.CompositionContainer.GetExportedValue<ConverterManager>();
            var schemaProvider = context.CompositionContainer.GetExportedValue<IDeviceSchemaProvider>();

            var addressProperty = context.DeviceProperties.Find(Constants.AddressPropertyName);
            var sizeProperty = context.DeviceProperties.Find(Constants.SizePropertyName);

            if (addressProperty == null)
            {
                throw new MidiDeviceDataException("The AddressMapGroupConverter could not find the 'address' Device Property (ToLogical). " +
                                                  "Did you tag the field that contains the address with property='address'?");
            }

            if (context.CurrentFieldConverter == null)
            {
                throw new MidiDeviceDataException("No current Field-Converter pair was found in the context." +
                                                  "An AddressMap can not be the root message RecordType.");
            }

            var address = SevenBitUInt32.FromSevenBitValue(addressProperty.GetValue<uint>());

            SevenBitUInt32 size;

            if (sizeProperty != null)
            {
                size = SevenBitUInt32.FromSevenBitValue(sizeProperty.GetValue<uint>());
            }
            else
            {
                var endNode = context.BinaryMap.Find(context.CurrentFieldConverter);

                if (endNode != null)
                {
                    // reading physical to logical
                    if (context.PhysicalStream.Length > context.PhysicalStream.Position)
                    {
                        int endSize = 0;

                        while (endNode != null)
                        {
                            if (endNode.NextSibling != null)
                            {
                                endNode = endNode.NextSibling;
                            }
                            else
                            {
                                endNode = endNode.NextNode;
                            }

                            if (endNode != null)
                            {
                                endSize += endNode.DataLength;
                            }
                        }

                        // remaining size of the physical stream minus the remain fields after the address map.
                        size = SevenBitUInt32.FromInt32((int)(context.PhysicalStream.Length - context.PhysicalStream.Position - endSize));
                    }
                    else
                    {
                        // writing physical from logical

                        //TODO:
                        size = SevenBitUInt32.FromInt32(ByteLength);
                    }
                }
                else
                {
                    //TODO:
                    size = SevenBitUInt32.FromInt32(ByteLength);
                }
            }

            var factory = new MessageTypeFactory(converterMgr, context.BinaryMap, schemaProvider);
            var converter = factory.CreateDynamicGroupConverter(address, size);

            if (converter == null)
            {
                throw new MidiDeviceDataException(
                    "The AddressMap did not result in a valid Field range for address: " + address + " and size: " + size + ".\r\n" +
                    "This usually means that the address in the SysEx message was not found in the AddressMap.");
            }

            return converter;
        }
    }
}