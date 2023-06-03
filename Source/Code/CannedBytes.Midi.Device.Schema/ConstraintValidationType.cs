namespace CannedBytes.Midi.Device.Schema
{
    /// <summary>
    /// An enumeration that lists the validation type used for a <see cref="Constraint"/>.
    /// </summary>
    /// <remarks>A <see cref="Constraint"/> uses these value to indicate how to process
    /// Constraint validation results.</remarks>
    public enum ConstraintValidationType
    {
        /// <summary>All <see cref="Constraint"/>s of this type must pass.</summary>
        All,
        /// <summary>At least one <see cref="Constraint"/> of this type must pass.</summary>
        One,
    }
}