using System.Collections.Generic;
using CannedBytes.Collections;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Converters;

/// <summary>
/// The StreamConverter class implements a basic RecordType converter without any additional (special) logic.
/// </summary>
public class StreamConverter : IConverter
{
    /// <summary>
    /// Constructs an instance based on the specified <paramref name="recordType"/>.
    /// </summary>
    /// <param name="recordType">Must not be null.</param>
    public StreamConverter(RecordType recordType)
    {
        Check.IfArgumentNull(recordType, "recordType");

        RecordType = recordType;
    }

    /// <summary>Indicates if the contents (Fields) was constructed dynamically (content determined at runtime).</summary>
    //public virtual bool IsDynamic
    //{
    //    get { return RecordType.IsDynamic; }
    //}

    /// <summary>
    /// Indicates if the converter represents an Address Map
    /// </summary>
    public bool IsAddressMap { get; protected set; }

    /// <summary>
    /// Gets the record type this group converter represents.
    /// </summary>
    public RecordType RecordType { get; }

    /// <inheritdoc/>
    public virtual string Name
    {
        get { return RecordType.Name.Name; }
    }

    /// <summary>
    /// Usually 0 for a StreamConverter unless it injects data (for instance checksum).
    /// </summary>
    public int ByteLength { get; protected set; }

    /// <summary>
    /// This is the place for a StreamConverter to intercept the actual nodes that are being processed.
    /// </summary>
    /// <param name="context">context.FieldInfo.CurrentNode contains thisNode.</param>
    /// <returns>Must never return null.</returns>
    public virtual IEnumerable<SchemaNode> GetChildNodeIterator(DeviceDataContext context)
    {
        Check.IfArgumentNull(context, "context");

        AggregateEnumerator<SchemaNode> aggEnum = new();

        for (int i = 0; i < context.FieldInfo.CurrentNode.InstanceCount; i++)
        {
            aggEnum.Add(context.FieldInfo.CurrentNode.RepeatedChildren(i));
        }

        return aggEnum;
    }

    /// <summary>
    /// No-op method for converters that implement <see cref="INavigationEvents"/>.
    /// </summary>
    /// <param name="context">Must not be null.</param>
    public virtual void OnBeforeRecord(DeviceDataContext context)
    {
        Check.IfArgumentNull(context, "context");
    }

    /// <summary>
    /// No-op method for converters that implement <see cref="INavigationEvents"/>.
    /// </summary>
    /// <param name="context">Must not be null.</param>
    public virtual void OnBeforeField(DeviceDataContext context)
    {
        Check.IfArgumentNull(context, "context");
    }

    /// <summary>
    /// No-op method for converters that implement <see cref="INavigationEvents"/>.
    /// </summary>
    /// <param name="context">Must not be null.</param>
    public virtual void OnAfterField(DeviceDataContext context)
    {
        Check.IfArgumentNull(context, "context");
    }

    /// <summary>
    /// No-op method for converters that implement <see cref="INavigationEvents"/>.
    /// </summary>
    /// <param name="context">Must not be null.</param>
    public virtual void OnAfterRecord(DeviceDataContext context)
    {
        Check.IfArgumentNull(context, "context");
    }
}
