using CannedBytes.Midi.Core;

namespace CannedBytes.Midi.Device.Schema;

/// <summary>
/// The DataType class describes one type of logical midi data.
/// </summary>
public sealed class DataType : AttributedSchemaObject
{
    /// <summary>
    /// Constructs a new instance.
    /// </summary>
    /// <param name="fullName">The long (and unique) name. Must not be null.</param>
    public DataType(string fullName)
    {
        Name = new SchemaObjectName(fullName);
    }

    public DataType(DeviceSchema schema, SchemaObjectName name)
    {
        Schema = schema;
        Name = name;
    }

    protected override void OnSchemaChanged()
    {
        base.OnSchemaChanged();

        if (baseTypes != null)
        {
            baseTypes.Schema = Schema;
        }
    }

    /// <summary>
    /// Gets an indication if the DataType can be instantiated.
    /// </summary>
    public bool IsAbstract { get; internal set; }

    public int ValueOffset { get; internal set; }

    public BitOrder BitOrder { get; internal set; }

    public ValueRange Range { get; internal set; }

    /// <summary>
    /// Gets a value indicating if this DataType derives from one or more other DataTypes.
    /// </summary>
    /// <remarks>Use this property, instead of accessing the <see cref="BaseTypes"/> property
    /// to find out if there are any base DataTypes.</remarks>
    public bool HasBaseTypes
    {
        get { return baseTypes?.Count > 0; }
    }

    /// <summary>
    /// Gets an indication if the data type is a union of (multiple) other data types.
    /// </summary>
    public bool IsUnion { get; internal set; }

    /// <summary>
    /// Gets an indication if the data type is an extension of (multiple) other data types.
    /// </summary>
    public bool IsExtension { get; internal set; }

    private DataTypeCollection baseTypes;

    /// <summary>
    /// Gets the collection of <see cref="DataType"/>s this definition is based on.
    /// </summary>
    /// <value>Derived classes can set this property. Must not be null.</value>
    public DataTypeCollection BaseTypes
    {
        get { return baseTypes ??= new DataTypeCollection { Schema = Schema }; }
    }

    /// <summary>
    /// Gets the base DataType if there is one (and only one).
    /// </summary>
    /// <remarks>If there are no or more than one base DataTypes this property returns null.
    /// Refer to the <see cref="HasBaseTypes"/> and <see cref="BaseTypes"/> property.</remarks>
    public DataType BaseType
    {
        get
        {
            if (baseTypes?.Count == 1)
            {
                return baseTypes[0];
            }

            return null;
        }
    }

    private ConstraintCollection constraints;

    /// <summary>
    /// Gets the collection of <see cref="Constraint"/>s for this <see cref="DataType"/> definition.
    /// </summary>
    /// <value>Derived classes can set this property. Must not be null.</value>
    /// <remarks>The collection contains only the Constraints declared in this DataType instance.</remarks>
    public ConstraintCollection Constraints
    {
        get { return constraints ??= new ConstraintCollection(); }
    }

    /// <summary>
    /// Searches the type hierarchy for a specific <paramref name="constraintType"/>.
    /// </summary>
    /// <param name="constraintType">The type of constraint to look for.</param>
    /// <returns>Returns null when no suitable constraint could be found.</returns>
    public Constraint FindConstraint(ConstraintTypes constraintType)
    {
        var dataType = this;

        Constraint constraint;
        do
        {
            constraint = dataType.Constraints.Find(constraintType);
            dataType = dataType.BaseType;
        }
        while (constraint == null && dataType != null);

        return constraint;
    }

    /// <summary>
    /// Determines if this data type is or derives from the specified <paramref name="fullDataTypeName"/>.
    /// </summary>
    /// <param name="fullDataTypeName">The full name of the DataType to test against.</param>
    /// <param name="recursive">If false only this type and its immediate base types are checked.
    /// If true than all base type are checked up the hierarchy.</param>
    /// <returns>Returns true if the DataType is or derives from the specified <paramref name="fullDataTypeName"/>.</returns>
    public bool IsType(string fullDataTypeName, bool recursive)
    {
        var success = Name.FullName == fullDataTypeName;

        if (!success && HasBaseTypes)
        {
            if (recursive)
            {
                foreach (DataType baseType in BaseTypes)
                {
                    success = baseType.IsType(fullDataTypeName, true);

                    if (success)
                    {
                        break;
                    }
                }
            }
            else
            {
                success = BaseTypes.Find(fullDataTypeName) != null;
            }
        }

        return success;
    }
}