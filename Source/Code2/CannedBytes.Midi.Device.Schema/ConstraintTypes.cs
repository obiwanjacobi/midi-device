namespace CannedBytes.Midi.Device.Schema
{
    /// <summary>
    /// Specifies the different types of constraints that can be put on a <see cref="DataType"/>
    /// </summary>
    public enum ConstraintTypes
    {
        /// <summary>Unsupported or unknown constraint.</summary>
        Unknown,

        /// <summary>Constraint that fixes the value.</summary>
        FixedValue,

        /// <summary>Constraint for a fixed length.</summary>
        FixedLength,

        /// <summary>A maximum value, including the value specified.</summary>
        MaxInclusive,

        /// <summary>A minimum value, including the value specified.</summary>
        MinInclusive,

        /// <summary>Restricting the value to a enumeration.</summary>
        Enumeration,
    }
}