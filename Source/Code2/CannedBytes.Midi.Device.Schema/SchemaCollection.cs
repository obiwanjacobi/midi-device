namespace CannedBytes.Midi.Device.Schema;

using System;
using System.Collections.ObjectModel;
using CannedBytes.Midi.Core;

/// <summary>
/// Implements a basic collection for schema types.
/// </summary>
/// <typeparam name="T">A schema type derived from <see cref="SchemaObject"/>.</typeparam>
public abstract class SchemaCollection<T> : KeyedCollection<string, T>
    where T : SchemaObject
{
    private readonly bool _enforceSchema;

    protected SchemaCollection(DeviceSchema schema, bool enforceSchema = true)
    {
        Schema = schema;
        _enforceSchema = enforceSchema;
    }

    /// <summary>
    /// Gets the <see cref="DeviceSchema"/> this instance is part of.
    /// </summary>
    /// <value>Derived classes can set the value for this property. Must not be null.</value>
    public DeviceSchema Schema { get; }

    /// <summary>
    /// Adds all the <see cref="SchemaObject"/> instances in the <paramref name="items"/>
    /// collection to this collection instance.
    /// </summary>
    /// <param name="items">Must not be null.</param>
    public void AddRange(SchemaCollection<T> items)
    {
        Assert.IfArgumentNull(items, nameof(items));

        foreach (T item in items)
        {
            Add(item);
        }
    }

    /// <summary>
    /// Tries to find the <typeparamref name="T"/> instance with the
    /// specified <paramref name="fullName"/>.
    /// </summary>
    /// <param name="itemName">The full name of an item.</param>
    /// <returns>Returns null if the item could not be found.</returns>
    public T? Find(string itemName)
    {
        Assert.IfArgumentNullOrEmpty(itemName, nameof(itemName));

        if (!Contains(itemName) &&
            Schema is not null && !itemName.StartsWith(Schema.SchemaName))
        {
            itemName = Schema.FormatFullName(itemName);
        }

        if (Contains(itemName))
        {
            return base[itemName];
        }

        return null;
    }

    /// <summary>
    /// Called by the collection base class to determine the key for
    /// a <see cref="SchemaObject"/> item.
    /// </summary>
    /// <param name="item">The item from the collection.</param>
    /// <returns>Returns a unique key. Can return an empty string. Never returns null.</returns>
    /// <remarks>Override this method if you want the collection to use another property
    /// than <see cref="P:SchemaObject.FullName"/> as key.</remarks>
    protected override string GetKeyForItem(T item)
    {
        if (item is not null)
        {
            return item.Name.FullName;
        }

        return String.Empty;
    }

    /// <summary>
    /// Inserts the specified <paramref name="item"/> at the
    /// specified <paramref name="index"/> into the collection.
    /// </summary>
    /// <param name="index">Zero-based index.</param>
    /// <param name="item">Must not be null.</param>
    /// <remarks>The override makes sure the <see cref="P:SchemaObject.Schema"/> property of the
    /// <paramref name="item"/> is set.</remarks>
    protected override void InsertItem(int index, T item)
    {
        if (_enforceSchema &&
            Schema is not null &&
            item.Schema != Schema)
        {
            throw new DeviceSchemaException(
                "The item does not belong to this schema.");
        }

        base.InsertItem(index, item);
    }
}
