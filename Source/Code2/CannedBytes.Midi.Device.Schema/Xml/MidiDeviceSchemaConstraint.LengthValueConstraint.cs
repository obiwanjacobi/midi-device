using CannedBytes.Midi.Core;

namespace CannedBytes.Midi.Device.Schema.Xml
{
    partial class MidiDeviceSchemaConstraint
    {
        private class LengthValueConstraint : SchemaConstraint<int>
        {
            public LengthValueConstraint(string value)
                : base("LengthValueConstraint", ConstraintValidationTypes.One)
            {
                Value = ValueParser.ParseInt32(value);
                ConstraintType = ConstraintTypes.FixedLength;
            }

            public override bool Validate<T>(T data)
            {
                string str = data as string;
                if (str != null)
                {
                    return str.Length <= Value;
                }

                // TODO: We need an interface to be able to validate multiple bytes as one value

                return true;
            }
        }
    }
}
