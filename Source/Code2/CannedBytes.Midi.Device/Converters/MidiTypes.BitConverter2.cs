using CannedBytes.Midi.Core;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Converters;

/// <summary>
/// A converter class that converts bits of a byte.
/// </summary>
/// <remarks>A simpler implementation using BitStreamReader replacing the Carry.</remarks>
internal sealed class BitConverter2 : DataConverter
{
    /// <summary>
    /// Construct an instance based on the specified <paramref name="dataType"/>
    /// and the <paramref name="bitFlags"/>.
    /// </summary>
    /// <param name="dataType">Must not be null.</param>
    /// <param name="range">Indicates the bits this converter will use out of byte.</param>
    public BitConverter2(DataType dataType)
        : base(dataType)
    { }

    /// <summary>
    /// Gets the bits that are used during conversion.
    /// </summary>
    //public ValueRange Range { get; }

    //private static BitFlags RangeToBitFlags(int start, int end)
    //{
    //    BitFlags flags = BitFlags.None;
            
    //    for (int f = start; f <= end; f++)
    //    {
    //        flags |= (BitFlags)(1 << f);
    //    }

    //    return flags;
    //}

    /// <summary>
    /// Returns the <see cref="BitFlags"/> property as a negative value.
    /// </summary>
    public override int ByteLength
    {
        // can't really know the data length until the field is known - which has the range pproperty.
        get { return -1; } //(int)RangeToBitFlags(Range.Start, Range.End); }
    }

    /// <inheritdoc/>
    public override void ToPhysical(DeviceDataContext context, IMidiLogicalReader reader)
    {
        Assert.IfArgumentNull(context, nameof(context));
        Assert.IfArgumentNull(reader, nameof(reader));
    }

    protected override void ReadFromReader(DeviceDataContext context, DeviceStreamReader reader, ILogicalWriteAccessor writer)
    {
        var range = context.FieldInfo.CurrentField.ExtendedProperties.Range;
        var value = reader.ReadBitRange(range);

        writer.Write(value, range.Length);
    }
}
