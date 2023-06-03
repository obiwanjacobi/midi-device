using CannedBytes.Midi.Core;
using System;

namespace CannedBytes.Midi.Device.Schema.Xml
{
    public abstract partial class MidiDeviceSchemaConstraint : Constraint
    {
        protected MidiDeviceSchemaConstraint(string name, ConstraintValidationTypes validationType)
            : base(name, validationType)
        { }

        public static MidiDeviceSchemaConstraint Create(string constraintType, string value)
        {
            MidiDeviceSchemaConstraint constraint = null;

            switch (constraintType)
            {
                case "enumeration":
                    constraint = new EnumeratedValueConstraint(value);
                    break;

                case "fixed":
                case "@fixed":
                    constraint = new FixedValueConstraint(value);
                    break;

                case "length":
                    constraint = new LengthValueConstraint(value);
                    break;

                case "maximum":
                    constraint = new MaxInclusiveValueConstraint(value);
                    break;

                case "minimum":
                    constraint = new MinInclusiveValueConstraint(value);
                    break;
            }

            return constraint;
        }
    }
}