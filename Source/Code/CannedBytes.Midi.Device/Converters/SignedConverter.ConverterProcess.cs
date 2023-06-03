using System;

namespace CannedBytes.Midi.Device.Converters
{
    partial class SignedConverter
    {
        private abstract class SignedProcess<T> : ConverterProcess<T>
            where T : IComparable
        {
            protected int _valueOffset;

            public SignedProcess(MidiDeviceDataContext context, int valueOffset)
                : base(context)
            {
                _valueOffset = valueOffset;
            }

            public override bool ReadFromContext()
            {
                var success = base.ReadFromContext();

                if (success)
                {
                    this.Context.Carry.Clear();
                    FlushedCarry = true;

                    var reader = new LittleEndianStreamReader(this.Context.CurrentStream);
                    Value = ReadValue(reader);
                }

                return success;
            }

            protected abstract T ReadValue(LittleEndianStreamReader reader);

            public override void ProcessToLogical()
            {
                if (_valueOffset != 0)
                {
                    SetValue<long>(GetValue<long>() + _valueOffset);
                }

                base.ProcessToLogical();
            }

            public override bool WriteToContext()
            {
                var success = base.WriteToContext();

                if (success)
                {
                    var carryLength = this.Context.Carry.Flush(this.Context.CurrentStream);
                    FlushedCarry = carryLength > 0;

                    var writer = new LittleEndianStreamWriter(this.Context.CurrentStream);

                    WriteValue(writer, Value);
                }

                return success;
            }

            protected abstract void WriteValue(LittleEndianStreamWriter writer, T value);

            public override void ProcessToPhysical()
            {
                if (_valueOffset != 0)
                {
                    SetValue<ulong>((ulong)(GetValue<long>() - _valueOffset));
                }

                base.ProcessToPhysical();
            }
        }

        private class Int16Process : SignedProcess<Int16>
        {
            public Int16Process(MidiDeviceDataContext context, int valueOffset)
                : base(context, valueOffset)
            {
            }

            protected override short ReadValue(LittleEndianStreamReader reader)
            {
                return reader.ReadInt16();
            }

            protected override void WriteValue(LittleEndianStreamWriter writer, short value)
            {
                writer.WriteInt16(value);
            }
        }

        private class Int32Process : SignedProcess<Int32>
        {
            public Int32Process(MidiDeviceDataContext context, int valueOffset)
                : base(context, valueOffset)
            {
            }

            protected override int ReadValue(LittleEndianStreamReader reader)
            {
                return reader.ReadInt32();
            }

            protected override void WriteValue(LittleEndianStreamWriter writer, int value)
            {
                writer.WriteInt32(value);
            }
        }

        private class Int64Process : SignedProcess<Int64>
        {
            public Int64Process(MidiDeviceDataContext context, int valueOffset)
                : base(context, valueOffset)
            {
            }

            protected override long ReadValue(LittleEndianStreamReader reader)
            {
                return reader.ReadInt64();
            }

            protected override void WriteValue(LittleEndianStreamWriter writer, long value)
            {
                writer.WriteInt64(value);
            }
        }
    }
}