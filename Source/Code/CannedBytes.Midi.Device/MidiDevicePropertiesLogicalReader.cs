namespace CannedBytes.Midi.Device
{
    public class MidiDevicePropertiesLogicalReader : IMidiLogicalReader
    {
        public MidiDevicePropertiesLogicalReader(MidiDeviceDataContext context, IMidiLogicalReader nestedReader)
        {
            Check.IfArgumentNull(context, "context");
            Check.IfArgumentNull(nestedReader, "nestedReader");

            DeviceContext = context;
            NestedReader = nestedReader;
        }

        public MidiDeviceDataContext DeviceContext { get; protected set; }

        public IMidiLogicalReader NestedReader { get; protected set; }

        protected void AddProperty(MidiLogicalContext context, object value)
        {
            if (!string.IsNullOrEmpty(context.Field.DevicePropertyName))
            {
                this.DeviceContext.DeviceProperties.Add(context.Field.Schema.SchemaName, context.Field.DevicePropertyName, value);
            }
        }

        public bool ReadBool(MidiLogicalContext context)
        {
            var data = NestedReader.ReadBool(context);

            AddProperty(context, data);

            return data;
        }

        public byte ReadByte(MidiLogicalContext context)
        {
            var data = NestedReader.ReadByte(context);

            AddProperty(context, data);

            return data;
        }

        public int ReadInt32(MidiLogicalContext context)
        {
            var data = NestedReader.ReadInt32(context);

            AddProperty(context, data);

            return data;
        }

        public long ReadInt64(MidiLogicalContext context)
        {
            var data = NestedReader.ReadInt64(context);

            AddProperty(context, data);

            return data;
        }

        public string ReadString(MidiLogicalContext context)
        {
            var data = NestedReader.ReadString(context);

            AddProperty(context, data);

            return data;
        }
    }
}