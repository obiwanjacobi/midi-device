namespace CannedBytes.Midi.Console.UI.Controls
{
    /// <summary>
    /// Implemented on Value Controls to publish capabilities.
    /// </summary>
    interface IValueControlInfo
    {
        /// <summary>
        /// Gets an info object describing the Control's capabilities. Used for control matching.
        /// </summary>
        ValueControlInfo Info { get; }
    }
}