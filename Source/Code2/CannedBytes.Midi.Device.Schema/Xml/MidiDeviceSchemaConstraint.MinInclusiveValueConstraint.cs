﻿using CannedBytes.Midi.Core;

namespace CannedBytes.Midi.Device.Schema.Xml;

partial class MidiDeviceSchemaConstraint
{
    private sealed class MinInclusiveValueConstraint : SchemaConstraint<long>
    {
        public MinInclusiveValueConstraint(string value)
            : base("MinInclusiveValueConstraint", ConstraintValidationTypes.All)
        {
            Value = ValueParser.ParseInt64(value);
            ConstraintType = ConstraintTypes.MinInclusive;
        }

        public override bool Validate<T>(T data)
        {
            return data.CompareTo(GetValue<T>()) >= 0;
        }
    }
}
