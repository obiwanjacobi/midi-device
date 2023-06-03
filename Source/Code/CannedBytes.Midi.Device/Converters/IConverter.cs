namespace CannedBytes.Midi.Device.Converters
{
    /// <summary>
    /// The IConverter interfaces represents the contract that all converters
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
        /// by subsequent BitConverters. Group converters return the sum of the length of all their fields.</remarks>
        int ByteLength { get; }

        /// <summary>
        /// Reads in the logical data and writes to the physical stream.
        /// </summary>
        /// <param name="context">The context used in the conversion process. Must not be null.</param>
        /// <param name="reader">The logical reader that provides the mechanism with logical data input. Must not be null.</param>
        void ToPhysical(MidiDeviceDataContext context, IMidiLogicalReader reader);

        /// <summary>
        /// Reads in the physical stream and writes out the logical data.
        /// </summary>
        /// <param name="context">The context used in the conversion process. Must not be null.</param>
        /// <param name="writer">The logical writer that writes the converted values into the application. Must not be null.</param>
        void ToLogical(MidiDeviceDataContext context, IMidiLogicalWriter writer);
    }
}