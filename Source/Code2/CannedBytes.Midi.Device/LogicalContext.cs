namespace CannedBytes.Midi.Device
{
    public class LogicalContext
    {
        public LogicalContext(ILogicalFieldInfo fieldInfo)
        {
            FieldInfo = fieldInfo;
        }

        /// <summary>
        /// Linked field info
        /// </summary>
        public ILogicalFieldInfo FieldInfo { get; private set; }

        /// <summary>
        /// Number of bits of the value (divide by 8 to get number of bytes).
        /// </summary>
        public int BitLength { get; set; }
    }
}
