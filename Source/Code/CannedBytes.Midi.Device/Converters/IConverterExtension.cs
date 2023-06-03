namespace CannedBytes.Midi.Device.Converters
{
    /// <summary>
    /// The extension interface allows converters to work together in a stack to produce one (logic) value.
    /// </summary>
    public interface IConverterExtension : IConverter
    {
        /// <summary>
        /// Gets or sets the inner/base converter.
        /// </summary>
        IConverterExtension InnerConverter { get; set; }

        /// <summary>
        /// Used in ToLogical
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Read<T>(MidiDeviceDataContext context);

        /// <summary>
        /// Used in ToPhysical
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        void Write<T>(MidiDeviceDataContext context, T value);
    }
}