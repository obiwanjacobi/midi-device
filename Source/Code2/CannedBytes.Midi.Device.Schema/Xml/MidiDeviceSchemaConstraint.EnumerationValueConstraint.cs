using CannedBytes.Midi.Core;

namespace CannedBytes.Midi.Device.Schema.Xml;

partial class MidiDeviceSchemaConstraint
{
    private sealed class EnumeratedValueConstraint : SchemaConstraint<int>
    {
        public EnumeratedValueConstraint(string value)
            : base("EnumeratedValueConstraint", ConstraintValidationTypes.One)
        {
            Value = ValueParser.ParseInt32(value);
            ConstraintType = ConstraintTypes.Enumeration;
        }
    }
}
