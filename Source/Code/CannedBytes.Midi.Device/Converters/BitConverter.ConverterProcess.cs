using System;

namespace CannedBytes.Midi.Device.Converters
{
    partial class BitConverter
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

        private class BitProcess : ConverterProcess<ushort>
        {
            public BitProcess(MidiDeviceDataContext context, BitFlags flags)
                : base(context)
            {
                this.Flags = flags;
            }

            public BitFlags Flags { get; set; }

            private byte _bitLength;

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
                if (Flags == BitFlags.None || part == ProcessPart.None) return 0;

                ushort mask = 0x01;

                // find the start of the data
                while (((int)Flags & (int)mask) == 0 && mask > 0)
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
                    while (((int)Flags & (int)mask) == mask && mask > 0)
                    {
                        // increment the bitLength
                        bitLength++;

                        // shift the mask up
                        mask <<= 1;
                    }
                }

                return data;
            }

            public override bool ReadFromContext()
            {
                var success = base.ReadFromContext();

                if (success)
                {
                    ushort data;
                    var carryLength = this.Context.Carry.ReadFrom(Context.CurrentStream, Flags, out data);

                    Value = data;
                    FlushedCarry = carryLength > 0;
                }

                return success;
            }

            public override void ProcessToLogical()
            {
                // shift data to logical location
                Value = ProcessStartAndLength(ProcessPart.LogicalStartAndLength, Value, out _bitLength);

                base.ProcessToLogical();
            }

            public override void ToLogical(IMidiLogicalWriter writer)
            {
                if (this.FieldData.Callback)
                {
                    // test for a single bit set
                    if (_bitLength == 1)
                    {
                        // call Write(Field, Boolean)
                        writer.Write(this.Context.CreateLogicalContext(), (Value > 0));
                    }
                    else if (_bitLength < 8)
                    {
                        // call Write(Field, Byte)
                        writer.Write(this.Context.CreateLogicalContext(), GetValue<byte>());
                    }
                    else
                    {
                        // call Write(Field, Int32)
                        writer.Write(this.Context.CreateLogicalContext(), GetValue<int>());
                    }
                }
            }

            public override void ToPhysical(IMidiLogicalReader reader)
            {
                ProcessStartAndLength(ProcessPart.Length, 0, out _bitLength);

                if (this.FieldData.Callback)
                {
                    // test for a single bit set
                    if (_bitLength == 1)
                    {
                        Value = (byte)(reader.ReadBool(this.Context.CreateLogicalContext()) ? 1 : 0);
                    }
                    else if (_bitLength < 8)
                    {
                        Value = reader.ReadByte(this.Context.CreateLogicalContext());
                    }
                    else
                    {
                        Value = (ushort)reader.ReadInt32(this.Context.CreateLogicalContext());
                    }
                }
                else
                {
                    Value = this.FieldData.FixedValue;
                }
            }

            public override void ProcessToPhysical()
            {
                base.ProcessToPhysical();

                // shift data to physical location
                Value = ProcessStartAndLength(ProcessPart.PhysicalStart, Value, out _bitLength);
            }

            public override bool WriteToContext()
            {
                var carryLength = this.Context.Carry.WriteTo(this.Context.CurrentStream, Value, Flags);

                FlushedCarry = carryLength > 0;

                return true;
            }
        }
    }
}