using System;

namespace CannedBytes.Midi.Device
{
    public partial class DeviceDataContext
    {
        public DeviceDataContext(ConversionDirection dir)
        {
            Carry = new Carry();
            FieldInfo = new FieldContext(this);
            DeviceProperties = new DevicePropertyCollection();
            StateMap = new ConverterState();

            ConversionDirection = dir;
            RecordManager = new DataRecordManager(this);
        }

        /// <summary>
        /// Gets or sets a value indicating whether to always call the logical reader or writer
        /// even when the Converter/DataType would otherwise not (true).
        /// </summary>
        public bool ForceLogicCall { get; set; }

        /// <summary>
        /// Indicates the direction of conversion.
        /// </summary>
        public ConversionDirection ConversionDirection { get; protected set; }

        /// <summary>
        /// Gets the current bit carry for 'physical' stream operations.
        /// </summary>
        public Carry Carry { get; private set; }
        
        /// <summary>
        /// Contains the runtime device properties retrieved from the message.
        /// </summary>
        public DevicePropertyCollection DeviceProperties { get; private set; }

        // Streams
        public StreamManager StreamManager { get; set; }

        // log records
        public DataRecordManager RecordManager { get; set; }

        // schema
        public SchemaNode RootNode { get; set; }

        /// <summary>
        /// Contains field-specific context information.
        /// </summary>
        public FieldContext FieldInfo { get; private set; }

        /// <summary>
        /// A place for converters to store state during a conversion run.
        /// </summary>
        public ConverterState StateMap { get; private set; }

        // returns a reader for the current node/converter (LE/BE).
        public DeviceStreamReader CreateReader()
        {
            var reader = new DeviceStreamReader(
                this.StreamManager.CurrentStream, this.Carry);

            return reader;
        }

        /// <summary>
        /// Contains an object that writes logical values to the application.
        /// </summary>
        /// <remarks>Only set when ToLogical.</remarks>
        public ILogicalWriteAccessor LogicalWriteAccessor { get; set; }

        /// <summary>
        /// Contains an object that reads logical values from the application.
        /// </summary>
        /// <remarks>Only set when ToPhysical.</remarks>
        public ILogicalReadAccessor LogicalReadAccessor { get; set; }

        /// <summary>
        /// Creates the context information that is passed to the application
        /// </summary>
        /// <returns>Never returns null.</returns>
        public virtual LogicalContext CreateLogicalContext()
        {
            if (this.FieldInfo.CurrentNode == null)
            {
                throw new InvalidOperationException(
                    "No current node is set");
            }

            var ctx = new LogicalContext(this.FieldInfo.CurrentNode);

            return ctx;
        }
    }
}
