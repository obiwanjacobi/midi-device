using System;

namespace CannedBytes.Midi.Device.Converters
{
    partial class UnsignedConverter
    {
        private class UInt16Process : ConverterProcess<UInt16>
        {
            public UInt16Process(MidiDeviceDataContext context)
                : base(context)
            {
            }

            public override bool ReadFromContext()
            {
                var success = base.ReadFromContext();

                if (success)
                {
                    this.Context.Carry.Clear();
                    FlushedCarry = true;

                    var reader = new LittleEndianStreamReader(this.Context.CurrentStream);
                    Value = reader.ReadUInt16();
                }

                return success;
            }

            public override bool WriteToContext()
            {
                var success = base.WriteToContext();

                if (success)
                {
                    var carryLength = this.Context.Carry.Flush(this.Context.CurrentStream);
                    FlushedCarry = carryLength > 0;

                    var writer = new LittleEndianStreamWriter(this.Context.CurrentStream);

                    writer.WriteUInt16(Value);
                }

                return success;
            }
        }

        private class UInt24Process : ConverterProcess<UInt32>
        {
            public UInt24Process(MidiDeviceDataContext context)
                : base(context)
            {
            }

            public override bool ReadFromContext()
            {
                var success = base.ReadFromContext();

                if (success)
                {
                    this.Context.Carry.Clear();
                    FlushedCarry = true;

                    var reader = new LittleEndianStreamReader(this.Context.CurrentStream);
                    Value = reader.ReadUInt24();
                }

                return success;
            }

            public override bool WriteToContext()
            {
                var success = base.WriteToContext();

                if (success)
                {
                    var carryLength = this.Context.Carry.Flush(this.Context.CurrentStream);
                    FlushedCarry = carryLength > 0;

                    var writer = new LittleEndianStreamWriter(this.Context.CurrentStream);

                    writer.WriteUInt24(Value);
                }

                return success;
            }
        }

        private class UInt32Process : ConverterProcess<UInt32>
        {
            public UInt32Process(MidiDeviceDataContext context)
                : base(context)
            {
            }

            public override bool ReadFromContext()
            {
                var success = base.ReadFromContext();

                if (success)
                {
                    this.Context.Carry.Clear();
                    FlushedCarry = true;

                    var reader = new LittleEndianStreamReader(this.Context.CurrentStream);
                    Value = reader.ReadUInt32();
                }

                return success;
            }

            public override bool WriteToContext()
            {
                var success = base.WriteToContext();

                if (success)
                {
                    var carryLength = this.Context.Carry.Flush(this.Context.CurrentStream);
                    FlushedCarry = carryLength > 0;

                    var writer = new LittleEndianStreamWriter(this.Context.CurrentStream);

                    writer.WriteUInt32(Value);
                }

                return success;
            }
        }

        private class UInt40Process : ConverterProcess<UInt64>
        {
            public UInt40Process(MidiDeviceDataContext context)
                : base(context)
            {
            }

            public override bool ReadFromContext()
            {
                var success = base.ReadFromContext();

                if (success)
                {
                    this.Context.Carry.Clear();
                    FlushedCarry = true;

                    var reader = new LittleEndianStreamReader(this.Context.CurrentStream);
                    Value = reader.ReadUInt40();
                }

                return success;
            }

            public override bool WriteToContext()
            {
                var success = base.WriteToContext();

                if (success)
                {
                    var carryLength = this.Context.Carry.Flush(this.Context.CurrentStream);
                    FlushedCarry = carryLength > 0;

                    var writer = new LittleEndianStreamWriter(this.Context.CurrentStream);

                    writer.WriteUInt40(Value);
                }

                return success;
            }
        }

        private class UInt48Process : ConverterProcess<UInt64>
        {
            public UInt48Process(MidiDeviceDataContext context)
                : base(context)
            {
            }

            public override bool ReadFromContext()
            {
                var success = base.ReadFromContext();

                if (success)
                {
                    this.Context.Carry.Clear();
                    FlushedCarry = true;

                    var reader = new LittleEndianStreamReader(this.Context.CurrentStream);
                    Value = reader.ReadUInt48();
                }

                return success;
            }

            public override bool WriteToContext()
            {
                var success = base.WriteToContext();

                if (success)
                {
                    var carryLength = this.Context.Carry.Flush(this.Context.CurrentStream);
                    FlushedCarry = carryLength > 0;

                    var writer = new LittleEndianStreamWriter(this.Context.CurrentStream);

                    writer.WriteUInt48(Value);
                }

                return success;
            }
        }

        private class UInt56Process : ConverterProcess<UInt64>
        {
            public UInt56Process(MidiDeviceDataContext context)
                : base(context)
            {
            }

            public override bool ReadFromContext()
            {
                var success = base.ReadFromContext();

                if (success)
                {
                    this.Context.Carry.Clear();
                    FlushedCarry = true;

                    var reader = new LittleEndianStreamReader(this.Context.CurrentStream);
                    Value = reader.ReadUInt56();
                }

                return success;
            }

            public override bool WriteToContext()
            {
                var success = base.WriteToContext();

                if (success)
                {
                    var carryLength = this.Context.Carry.Flush(this.Context.CurrentStream);
                    FlushedCarry = carryLength > 0;

                    var writer = new LittleEndianStreamWriter(this.Context.CurrentStream);

                    writer.WriteUInt56(Value);
                }

                return success;
            }
        }

        private class UInt64Process : ConverterProcess<UInt64>
        {
            public UInt64Process(MidiDeviceDataContext context)
                : base(context)
            {
            }

            public override bool ReadFromContext()
            {
                var success = base.ReadFromContext();

                if (success)
                {
                    this.Context.Carry.Clear();
                    FlushedCarry = true;

                    var reader = new LittleEndianStreamReader(this.Context.CurrentStream);
                    Value = reader.ReadUInt64();
                }

                return success;
            }

            public override bool WriteToContext()
            {
                var success = base.WriteToContext();

                if (success)
                {
                    var carryLength = this.Context.Carry.Flush(this.Context.CurrentStream);
                    FlushedCarry = carryLength > 0;

                    var writer = new LittleEndianStreamWriter(this.Context.CurrentStream);

                    writer.WriteUInt64(Value);
                }

                return success;
            }
        }
    }
}