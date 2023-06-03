namespace CannedBytes.Midi.Device.Converters
{
    using CannedBytes.Midi.Device.Schema;

    /// <summary>
    /// A converter class that converts bits of a byte.
    /// </summary>
    public partial class BitConverter : ConverterExtension
    {
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

        private BitFlags _bitFlag;

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
        /// <param name="data">The raw input data.</param>
        /// <param name="bitLength">Receives the total bit length of the processing.</param>
        /// <returns>Returns the new data value.</returns>
        private ushort ProcessStartAndLength(ProcessPart part, ushort data, out byte bitLength)
        {
            bitLength = 0;

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

            return data;
        }

        /// <inheritdoc/>
        protected void ToLogicalInternal(MidiDeviceDataContext context, IMidiLogicalWriter writer)
        {
            Check.IfArgumentNull(context, "context");
            Check.IfArgumentNull(writer, "writer");

            var fieldData = new FieldData<ushort>(context);
            var inputStream = context.CurrentStream;

            long pos = context.PhysicalStream.Position;
            ushort data = 0;
            var carryLength = context.Carry.ReadFrom(inputStream, BitFlag, out data);

            byte bitLength = 0;
            // shift data to logical location
            data = ProcessStartAndLength(ProcessPart.LogicalStartAndLength, data, out bitLength);

            fieldData.Validate(data);

            if (fieldData.Callback)
            {
                // test for a single bit set
                if (bitLength == 1)
                {
                    // call Write(Field, Boolean)
                    writer.Write(context.CreateLogicalContext(), (data > 0));
                }
                else if (bitLength < 8)
                {
                    // call Write(Field, Byte)
                    writer.Write(context.CreateLogicalContext(), (byte)data);
                }
                else
                {
                    // call Write(Field, Int32)
                    writer.Write(context.CreateLogicalContext(), (int)data);
                }
            }

            context.DataRecords.Add(pos, data, context.CurrentFieldConverter.Field, carryLength > 0);
        }

        /// <inheritdoc/>
        protected void ToPhysicalInternal(MidiDeviceDataContext context, IMidiLogicalReader reader)
        {
            Check.IfArgumentNull(context, "context");
            Check.IfArgumentNull(reader, "reader");

            var fieldData = new FieldData<ushort>(context);
            var outputStream = context.CurrentStream;
            long pos = context.PhysicalStream.Position;
            byte bitLength;
            ushort data;

            ProcessStartAndLength(ProcessPart.Length, 0, out bitLength);

            if (fieldData.Callback)
            {
                // test for a single bit set
                if (bitLength == 1)
                {
                    data = (byte)(reader.ReadBool(context.CreateLogicalContext()) ? 1 : 0);
                }
                else if (bitLength < 8)
                {
                    data = reader.ReadByte(context.CreateLogicalContext());
                }
                else
                {
                    data = (ushort)reader.ReadInt32(context.CreateLogicalContext());
                }
            }
            else
            {
                data = (ushort)fieldData.FixedValue;
            }

            fieldData.Validate(data);

            var recordData = data;

            // shift data to physical location
            data = ProcessStartAndLength(ProcessPart.PhysicalStart, data, out bitLength);

            var carryLength = context.Carry.WriteTo(outputStream, data, BitFlag);

            context.DataRecords.Add(pos, recordData, context.CurrentFieldConverter.Field, carryLength > 0);
        }

        protected override IConverterProcess CreateProcess(MidiDeviceDataContext context)
        {
            var process = new BitProcess(context, this.BitFlag);

            return process;
        }
    }
}