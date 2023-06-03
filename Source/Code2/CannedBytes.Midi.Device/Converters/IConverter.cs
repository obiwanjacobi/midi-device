namespace CannedBytes.Midi.Device.Converters
{
    /// <summary>
    /// The IConverter interface represents the contract that all converters
    /// must implement to be incorporated into the mechanism of the
    /// Midi Device Schema.
    /// </summary>
    public interface IConverter
    {
        /// <summary>
        /// Gets the name of the Converter.
        /// </summary>
        /// <remarks>Usually linked to the <see cref="DataType"/> or the <see cref="RecordType"/> it serves.</remarks>
        string Name { get; }

        /// <summary>
        /// Gets the physical byte length.
        /// </summary>
        /// <remarks>Negative numbers are <see cref="BitFalgs"/> and the caller should compute how many bytes are used
        /// by subsequent BitConverters. Stream converters return the length of data they manage - not including their Fields.</remarks>
        int ByteLength { get; }
    }
}
