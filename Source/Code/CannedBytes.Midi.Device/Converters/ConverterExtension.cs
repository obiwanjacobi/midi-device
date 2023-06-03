using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Converters
{
    public abstract class ConverterExtension : Converter, IConverterExtension
    {
        protected ConverterExtension(DataType dataType)
            : base(dataType)
        {
        }

        /// </inheritdoc/>
        /// <remarks>Derived classes must override and implement.</remarks>
        public override void ToPhysical(MidiDeviceDataContext context, IMidiLogicalReader reader)
        {
            var process = CreateProcess(context);

            process.ToPhysical(reader);

            ExecuteWrite(process);
        }

        /// </inheritdoc/>
        /// <remarks>Derived classes must override and implement.</remarks>
        public override void ToLogical(MidiDeviceDataContext context, IMidiLogicalWriter writer)
        {
            var process = ExecuteRead(context);

            process.ToLogical(writer);
        }

        protected IConverterProcess ExecuteRead(MidiDeviceDataContext context)
        {
            var process = CreateProcess(context);

            if (InnerConverter != null)
            {
                // read from extension
                process.ReadFromExtension(InnerConverter);
            }
            else
            {
                // read from context
                process.ReadFromContext();
            }

            process.ProcessToLogical();
            process.AddDataLogRecord();

            return process;
        }

        protected void ExecuteWrite(IConverterProcess process)
        {
            process.ProcessToPhysical();
            process.AddDataLogRecord();

            if (InnerConverter != null)
            {
                // write to extension
                process.WriteToExtension(InnerConverter);
            }
            else
            {
                // write to context
                process.WriteToContext();
            }
        }

        protected abstract IConverterProcess CreateProcess(MidiDeviceDataContext context);

        /// <summary>
        /// Gets or sets the inner converter of a stack of (Data) Converters.
        /// </summary>
        public IConverterExtension InnerConverter { get; set; }

        public T Read<T>(MidiDeviceDataContext context)
        {
            var process = ExecuteRead(context);

            return process.GetValue<T>();
        }

        public void Write<T>(MidiDeviceDataContext context, T value)
        {
            var process = CreateProcess(context);

            process.SetValue(value);

            ExecuteWrite(process);
        }
    }
}