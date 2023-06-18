using CannedBytes.Midi.Core;

namespace CannedBytes.Midi.Device.Schema;

internal sealed class FixedValueConstraint : SchemaConstraint<int>
{
    public FixedValueConstraint(string value)
        : base("FixedValueConstraint", ConstraintValidationTypes.OneOf)
    {
        Value = ValueParser.ParseInt32(value);
        ConstraintType = ConstraintTypes.FixedValue;
    }
}
