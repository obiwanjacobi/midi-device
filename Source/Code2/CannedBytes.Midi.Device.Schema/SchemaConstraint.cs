using System;

namespace CannedBytes.Midi.Device.Schema;

public abstract class SchemaConstraint<TValue> : Constraint
    where TValue : IComparable
{
    protected SchemaConstraint(string name, ConstraintValidationTypes validationType)
        : base(name, validationType)
    { }

    public TValue? Value { get; internal protected set; }

    public override T GetValue<T>()
    {
        var val = Convert.ChangeType(Value, typeof(T))
            ?? throw new DeviceSchemaException($"Cannot return value '{Value}' as type: {typeof(T)}");

        return (T)val;
    }

    public override bool Validate<T>(T data)
    {
        return data.CompareTo(GetValue<T>()) == 0;
    }
}
