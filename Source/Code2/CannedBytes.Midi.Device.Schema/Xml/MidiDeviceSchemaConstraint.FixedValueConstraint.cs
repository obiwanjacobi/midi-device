using CannedBytes.Midi.Core;

namespace CannedBytes.Midi.Device.Schema.Xml;

partial class MidiDeviceSchemaConstraint
{
    private sealed class FixedValueConstraint : SchemaConstraint<int>
    {
        public FixedValueConstraint(string value)
            : base("FixedValueConstraint", ConstraintValidationTypes.One)
        {
            Value = ValueParser.ParseInt32(value);
            ConstraintType = ConstraintTypes.FixedValue;
        }
    }
}
