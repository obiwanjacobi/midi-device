using CannedBytes.Midi.Device.Schema;
using System;

namespace CannedBytes.Midi.Device.Converters
{
    /// <summary>
    /// A converter class that converts bits of a byte.
    /// </summary>
    public partial class BitConverter : DataConverter
    {
        [Flags]
        private enum ProcessPart
        {
            None = 0x00,
            Length = 0x01,
            PhysicalStart = 0x02,
            LogicalStart = 0x04,

            PhysicalStartAndLength = Length | PhysicalStart,
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
            _bitFlag = bitFlag;
        }

        private readonly BitFlags _bitFlag;

        /// <summary>
        /// Gets the bits that are used during conversion.
        /// </summary>
        public BitFlags BitFlag
        {
            get { return _bitFlag; }
        }

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
            Check.IfArgumentNull(context, "context");
            Check.IfArgumentNull(reader, "reader");

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
            Check.IfArgumentNull(context, "context");
            Check.IfArgumentNull(reader, "reader");
            Check.IfArgumentNull(writer, "writer");

            ushort value;
            
            reader.Read(this.BitFlag, out value);

            int bitLength = ProcessStartAndLength(ProcessPart.LogicalStartAndLength, ref value);

            writer.Write(value, bitLength);
        }
    }
}
