using System;

namespace CannedBytes.Midi.Device.Schema;

/// <summary>
/// The Constraint class provides an abstract base class for concrete constraint
/// implementations.
/// </summary>
public abstract class Constraint
{
    /// <summary>
    /// Constructs an instance.
    /// </summary>
    /// <param name="name">The name of the constraint.</param>
    /// <param name="validationType">A value indicating how to interpret the validation results.</param>
    protected Constraint(string name,
        ConstraintValidationTypes validationType)
    {
        Name = name;
        ValidationType = validationType;
    }

    /// <summary>
    /// Gets the value indicating how to interpret the validation results.
    /// </summary>
    /// <value>Derived classes can set this property. Must not be empty.</value>
    public ConstraintValidationTypes ValidationType { get; }

    /// <summary>
    /// Gets the value indicating what standard type of constraint this is.
    /// </summary>
    /// <value>Derived classes can set this property. Must not be empty.</value>
    public ConstraintTypes ConstraintType { get; internal protected set; }

    /// <summary>
    /// Gets the name of the Constraint.
    /// </summary>
    /// <value>Derived classes can set this property. Must not be null or empty.</value>
    public string Name { get; }

    /// <summary>
    /// Gets the value used in validation.
    /// </summary>
    /// <remarks>Derived classes must implement this method.</remarks>
    public abstract T GetValue<T>();

    /// <summary>
    /// Validates the <paramref name="data"/> byte against the value.
    /// </summary>
    /// <param name="data">A 'unit' of midi data.</param>
    /// <returns>Returns true if the <paramref name="data"/> passes validation
    /// otherwise false is returned.</returns>
    public abstract bool Validate<T>(T data)
        where T : IComparable;

    public override string ToString()
    {
        return Name;
    }
}