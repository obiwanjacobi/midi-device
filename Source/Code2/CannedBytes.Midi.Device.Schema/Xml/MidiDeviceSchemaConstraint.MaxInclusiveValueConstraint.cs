using CannedBytes.Midi.Core;

namespace CannedBytes.Midi.Device.Schema.Xml;

partial class MidiDeviceSchemaConstraint
{
    private sealed class MaxInclusiveValueConstraint : SchemaConstraint<long>
    {
        public MaxInclusiveValueConstraint(string value)
            : base("MaxInclusiveValueConstraint", ConstraintValidationTypes.All)
        {
            Value = ValueParser.ParseInt64(value);
            ConstraintType = ConstraintTypes.MaxInclusive;
        }

        public override bool Validate<T>(T data)
        {
            return data.CompareTo(GetValue<T>()) <= 0;
        }
    }
}
