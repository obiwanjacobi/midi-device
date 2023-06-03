namespace CannedBytes.Midi.Device
{
    public class MidiDevicePropertiesLogicalWriter : IMidiLogicalWriter
    {
        public MidiDevicePropertiesLogicalWriter(MidiDeviceDataContext context, IMidiLogicalWriter nestedWriter)
        {
            Check.IfArgumentNull(context, "context");
            //Check.IfArgumentNull(nestedWriter, "nestedWriter");

            DeviceContext = context;
            NestedWriter = nestedWriter;
        }

        public MidiDeviceDataContext DeviceContext { get; protected set; }

        public IMidiLogicalWriter NestedWriter { get; protected set; }

        protected void AddProperty(MidiLogicalContext context, object value)
        {
            if (!string.IsNullOrEmpty(context.Field.DevicePropertyName))
            {
                this.DeviceContext.DeviceProperties.Add(context.Field.Schema.SchemaName, context.Field.DevicePropertyName, value);
            }
        }

        public void Write(MidiLogicalContext context, bool data)
        {
            AddProperty(context, data);

            if (NestedWriter != null)
            {
                NestedWriter.Write(context, data);
            }
        }

        public void Write(MidiLogicalContext context, byte data)
        {
            AddProperty(context, data);

            if (NestedWriter != null)
            {
                NestedWriter.Write(context, data);
            }
        }

        public void Write(MidiLogicalContext context, int data)
        {
            AddProperty(context, data);

            if (NestedWriter != null)
            {
                NestedWriter.Write(context, data);
            }
        }

        public void Write(MidiLogicalContext context, long data)
        {
            AddProperty(context, data);

            if (NestedWriter != null)
            {
                NestedWriter.Write(context, data);
            }
        }

        public void Write(MidiLogicalContext context, string data)
        {
            AddProperty(context, data);

            if (NestedWriter != null)
            {
                NestedWriter.Write(context, data);
            }
        }
    }
}