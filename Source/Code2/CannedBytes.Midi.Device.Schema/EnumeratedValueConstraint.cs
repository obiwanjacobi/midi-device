using CannedBytes.Midi.Core;

namespace CannedBytes.Midi.Device.Schema;

internal sealed class EnumeratedValueConstraint : SchemaConstraint<int>
{
    public EnumeratedValueConstraint(string value)
        : base("EnumeratedValueConstraint", ConstraintValidationTypes.OneOf)
    {
        Value = ValueParser.ParseInt32(value);
        ConstraintType = ConstraintTypes.Enumeration;
    }
}
