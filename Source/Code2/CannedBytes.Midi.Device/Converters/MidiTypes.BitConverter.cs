using System;
using CannedBytes.Midi.Core;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Converters;

/// <summary>
/// A converter class that converts bits of a byte.
/// </summary>
internal sealed class BitConverter : DataConverter
{
    [Flags]
    private enum ProcessPart
    {
        None = 0x00,
        Length = 0x01,
        PhysicalStart = 0x02,
        PhysicalStartAndLength = Length | PhysicalStart,
        LogicalStart = 0x04,
        LogicalStartAndLength = Length | LogicalStart,
    }

    /// <summary>
    /// Construct an instance based on the specified <paramref name="dataType"/>
    /// and the <paramref name="bitFlags"/>.
    /// </summary>
    /// <param name="dataType">Must not be null.</param>
    /// <param name="bitFlag">Indicates the bits this converter will use out of byte.</param>
    public BitConverter(DataType dataType, BitFlags bitFlag)
        : base(dataType)
    {
        BitFlag = bitFlag;
    }

    /// <summary>
    /// Gets the bits that are used during conversion.
    /// </summary>
    public BitFlags BitFlag { get; }

    /// <summary>
    /// Returns the <see cref="BitFlags"/> property as a negative value.
    /// </summary>
    public override int ByteLength
    {
        get { return -(int)BitFlag; }
    }

    /// <summary>
    /// Processes the data according to the specified <paramref name="part"/>s.
    /// </summary>
    /// <param name="part">Specifies the operations to perform on the <paramref name="data"/>.</param>
    /// <param name="data">[in] The raw input data. [out] the adjusted data (shifted).</param>
    /// <returns>Returns the total bit length of the processing..</returns>
    private byte ProcessStartAndLength(ProcessPart part, ref ushort data)
    {
        byte bitLength = 0;

        // sanity check
        if (BitFlag == BitFlags.None || part == ProcessPart.None) return 0;

        ushort mask = 0x01;

        // find the start of the data
        while (((int)BitFlag & (int)mask) == 0 && mask > 0)
        {
            if ((part & ProcessPart.LogicalStart) > 0)
            {
                // shift the physical data down
                data >>= 1;
            }
            else if ((part & ProcessPart.PhysicalStart) > 0)
            {
                // shift the logical data up
                data <<= 1;
            }

            // shift the mask up
            mask <<= 1;
        }

        if ((part & ProcessPart.Length) > 0)
        {
            // find the length of the data
            while (((int)BitFlag & (int)mask) == mask && mask > 0)
            {
                // increment the bitLength
                bitLength++;

                // shift the mask up
                mask <<= 1;
            }
        }

        return bitLength;
    }

    /// <inheritdoc/>
    public override void ToPhysical(DeviceDataContext context, IMidiLogicalReader reader)
    {
        Assert.IfArgumentNull(context, nameof(context));
        Assert.IfArgumentNull(reader, nameof(reader));

        //var fieldData = new FieldData<ushort>(context);
        //var outputStream = context.CurrentStream;
        //long pos = context.PhysicalStream.Position;
        //byte bitLength;
        //ushort data;

        //ProcessStartAndLength(ProcessPart.Length, 0, out bitLength);

        //if (fieldData.Callback)
        //{
        //    // test for a single bit set
        //    if (bitLength == 1)
        //    {
        //        data = (byte)(reader.ReadBool(context.CreateLogicalContext()) ? 1 : 0);
        //    }
        //    else if (bitLength < 8)
        //    {
        //        data = reader.ReadByte(context.CreateLogicalContext());
        //    }
        //    else
        //    {
        //        data = (ushort)reader.ReadInt32(context.CreateLogicalContext());
        //    }
        //}
        //else
        //{
        //    data = (ushort)fieldData.FixedValue;
        //}

        //fieldData.Validate(data);

        //var recordData = data;

        //// shift data to physical location
        //data = ProcessStartAndLength(ProcessPart.PhysicalStart, data, out bitLength);

        //var carryLength = context.Carry.WriteTo(outputStream, data, BitFlag);

        //context.DataRecords.Add(pos, recordData, context.CurrentFieldConverter.Field, carryLength > 0);
    }

    protected override void ReadFromReader(DeviceDataContext context, DeviceStreamReader reader, ILogicalWriteAccessor writer)
    {
        reader.ReadBits(BitFlag, out ushort value);

        int bitLength = ProcessStartAndLength(ProcessPart.LogicalStartAndLength, ref value);

        writer.Write(value, bitLength);
    }
}
