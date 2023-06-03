using CannedBytes.Midi.Core;
using System;

namespace CannedBytes.Midi.Device.Schema.Xml
{
    public abstract class MidiDeviceSchemaConstraint : Constraint
    {
        protected MidiDeviceSchemaConstraint(string name, ConstraintValidationType validationType)
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

        internal abstract class SchemaConstraint<TValue> : MidiDeviceSchemaConstraint
            where TValue : IComparable
        {
            protected SchemaConstraint(string name, ConstraintValidationType validationType)
                : base(name, validationType)
            {
            }

            public TValue Value { get; internal protected set; }

            public override T GetValue<T>()
            {
                return (T)Convert.ChangeType(Value, typeof(T));
            }

            public override bool Validate<T>(T data)
            {
                return (data.CompareTo(GetValue<T>()) == 0);
            }
        }

        internal class MinInclusiveValueConstraint : SchemaConstraint<long>
        {
            public MinInclusiveValueConstraint(string value)
                : base("MinInclusiveValueConstraint", ConstraintValidationType.All)
            {
                //Value = Int64.Parse(value);
                Value = ValueParser.ParseInt64(value);
                ConstraintType = ConstraintType.MinInclusive;
            }

            public override bool Validate<T>(T data)
            {
                return (data.CompareTo(GetValue<T>()) >= 0);
            }
        }

        internal class MaxInclusiveValueConstraint : SchemaConstraint<long>
        {
            public MaxInclusiveValueConstraint(string value)
                : base("MaxInclusiveValueConstraint", ConstraintValidationType.All)
            {
                //Value = Int64.Parse(value);
                Value = ValueParser.ParseInt64(value);
                ConstraintType = ConstraintType.MaxInclusive;
            }

            public override bool Validate<T>(T data)
            {
                return (data.CompareTo(GetValue<T>()) <= 0);
            }
        }

        internal class EnumeratedValueConstraint : SchemaConstraint<int>
        {
            public EnumeratedValueConstraint(string value)
                : base("EnumeratedValueConstraint", ConstraintValidationType.One)
            {
                //Value = Int32.Parse(value);
                Value = ValueParser.ParseInt32(value);
                ConstraintType = ConstraintType.Enumeration;
            }
        }

        internal class LengthValueConstraint : SchemaConstraint<int>
        {
            public LengthValueConstraint(string value)
                : base("LengthValueConstraint", ConstraintValidationType.One)
            {
                //Value = Int32.Parse(value);
                Value = ValueParser.ParseInt32(value);
                ConstraintType = ConstraintType.FixedLength;
            }

            public override bool Validate<T>(T data)
            {
                // TODO: We need an interface to be able to validate multiple bytes as one value

                return true;
            }
        }

        internal class FixedValueConstraint : SchemaConstraint<int>
        {
            public FixedValueConstraint(string value)
                : base("FixedValueConstraint", ConstraintValidationType.One)
            {
                //Value = Int32.Parse(value);
                Value = ValueParser.ParseInt32(value);
                ConstraintType = ConstraintType.FixedValue;
            }
        }
    }
}