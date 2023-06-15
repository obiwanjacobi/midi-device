using System;
using CannedBytes.Midi.Core;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Converters;

/// <summary>
/// The DataConverter class provides a base class for (field) converter classes.
/// </summary>
/// <remarks>
/// All converters are created on a <see cref="DataType"/> which provides the
/// converter with the necessary meta (and context) information.
/// The <see cref="ConverterFacory"/> determines what converter is created for which <see cref="DataType"/>.
/// </remarks>
public abstract partial class DataConverter : IConverter
{
    /// <summary>
    /// A constructor for derived classes.
    /// </summary>
    /// <param name="dataType">The data type the converter represents at runtime. Must not be null.</param>
    protected DataConverter(DataType dataType)
    {
        Assert.IfArgumentNull(dataType, nameof(dataType));

        DataType = dataType;
    }

    public DeviceSchema Schema => DataType.Schema;

    /// <summary>
    /// Gets the data type on which this converter was constructed.
    /// </summary>
    public DataType DataType { get; private set; }

    /// <summary>
    /// Gets the name of the converter.
    /// </summary>
    /// <remarks>
    /// By default this implementation returns the full name of the <see cref="DataType"/> attached to this converter.
    /// </remarks>
    public virtual string Name
    {
        get { return DataType.Name.FullName; }
    }

    /// <inheritdoc/>
    public virtual int ByteLength
    {
        get { return 1; }
    }

    /// <inheritdoc/>
    /// <remarks>Derived classes must override and implement.</remarks>
    public abstract void ToPhysical(DeviceDataContext context, IMidiLogicalReader reader);

    // decorator pattern
    /// <summary>
    /// The converter called when used in extension.
    /// </summary>
    public DataConverter InnerConverter { get; set; }

    public virtual void ToLogical(DeviceDataContext context, DeviceStreamReader reader, ILogicalWriteAccessor writer)
    {
        Assert.IfArgumentNull(context, nameof(context));
        Assert.IfArgumentNull(reader, nameof(reader));
        Assert.IfArgumentNull(writer, nameof(writer));

        if (InnerConverter != null)
        {
            ReadFromInnerConverter(context, reader, writer);
        }
        else
        {
            ReadFromReader(context, reader, writer);
        }
    }

    protected virtual void ReadFromInnerConverter(DeviceDataContext context, DeviceStreamReader reader, ILogicalWriteAccessor writer)
    {
        if (InnerConverter == null)
        {
            throw new InvalidOperationException("The InnerConverter property is not set.");
        }

        InnerConverter.ToLogical(context, reader, writer);
    }

    /// <summary>
    /// As part of the <see cref="ToLogical"/> operation this method reads the physical data 
    /// from <paramref name="reader"/>, converts it into a logical value that is written to
    /// the <paramref name="writer"/>.
    /// </summary>
    /// <param name="context">Is never null.</param>
    /// <param name="reader">Is never null.</param>
    /// <param name="writer">Is never null.</param>
    protected abstract void ReadFromReader(DeviceDataContext context, DeviceStreamReader reader, ILogicalWriteAccessor writer);
}
