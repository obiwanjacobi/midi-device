using CannedBytes.Midi.Core;

namespace CannedBytes.Midi.Device.Schema;

internal sealed class LengthValueConstraint : SchemaConstraint<int>
{
    public LengthValueConstraint(string value)
        : base("LengthValueConstraint", ConstraintValidationTypes.OneOf)
    {
        Value = ValueParser.ParseInt32(value);
        ConstraintType = ConstraintTypes.FixedLength;
    }

    public override bool Validate<T>(T data)
    {
        if (data is string str)
        {
            return str.Length <= Value;
        }

        // TODO: We need an interface to be able to validate multiple bytes as one value

        return true;
    }
}
