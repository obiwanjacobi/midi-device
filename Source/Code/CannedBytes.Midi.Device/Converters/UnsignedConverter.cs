namespace CannedBytes.Midi.Device.Converters
{
    using System;
    using CannedBytes.Midi.Device.Schema;

    public partial class UnsignedConverter : ConverterExtension
    {
        public UnsignedConverter(DataType dataType)
            : base(dataType)
        {
            var constraint = dataType.FindConstraint(ConstraintType.FixedLength);

            if (constraint != null)
            {
                _byteLength = constraint.GetValue<int>();
            }
            else
            {
                _byteLength = 2;
            }

            if (_byteLength < 2 || _byteLength > 8)
            {
                throw new ArgumentOutOfRangeException("dataType", _byteLength,
                    "The length constraint of the specified DataType '" + dataType.Name.FullName + "' is out of range (2-8).");
            }
        }

        private int _byteLength;

        public override int ByteLength
        {
            get { return _byteLength; }
        }

        public void _ToPhysical(MidiDeviceDataContext context, IMidiLogicalReader reader)
        {
            var pos = context.PhysicalStream.Position;
            var carryLength = context.Carry.Flush(context.CurrentStream);

            var logicalContext = context.CreateLogicalContext();
            var writer = new LittleEndianStreamWriter(context.CurrentStream);

            switch (_byteLength)
            {
                case 2:
                    {
                        var fieldData = new FieldData<UInt16>(context);
                        UInt16 data;

                        if (fieldData.Callback)
                        {
                            data = (UInt16)reader.ReadInt32(logicalContext);
                        }
                        else
                        {
                            data = fieldData.FixedValue;
                        }

                        writer.WriteUInt16(data);

                        context.DataRecords.Add(pos, data, context.CurrentFieldConverter.Field, carryLength > 0);
                    }
                    break;
                case 3:
                    {
                        var data = ReadUInt32(context, logicalContext, reader);

                        writer.WriteUInt24(data);

                        context.DataRecords.Add(pos, data, context.CurrentFieldConverter.Field, carryLength > 0);
                    }
                    break;
                case 4:
                    {
                        var data = ReadUInt32(context, logicalContext, reader);

                        writer.WriteUInt32(data);

                        context.DataRecords.Add(pos, data, context.CurrentFieldConverter.Field, carryLength > 0);
                    }
                    break;
                case 5:
                    {
                        var data = ReadUInt64(context, logicalContext, reader);

                        writer.WriteUInt40(data);

                        context.DataRecords.Add(pos, data, context.CurrentFieldConverter.Field, carryLength > 0);
                    }
                    break;
                case 6:
                    {
                        var data = ReadUInt64(context, logicalContext, reader);

                        writer.WriteUInt48(data);

                        context.DataRecords.Add(pos, data, context.CurrentFieldConverter.Field, carryLength > 0);
                    }
                    break;
                case 7:
                    {
                        var data = ReadUInt64(context, logicalContext, reader);

                        writer.WriteUInt56(data);

                        context.DataRecords.Add(pos, data, context.CurrentFieldConverter.Field, carryLength > 0);
                    }
                    break;
                case 8:
                    {
                        var data = ReadUInt64(context, logicalContext, reader);

                        writer.WriteUInt64(data);

                        context.DataRecords.Add(pos, data, context.CurrentFieldConverter.Field, carryLength > 0);
                    }
                    break;
            }
        }

        private UInt32 ReadUInt32(MidiDeviceDataContext context, MidiLogicalContext logicalContext, IMidiLogicalReader reader)
        {
            var fieldData = new FieldData<UInt32>(context);
            UInt32 data;

            if (fieldData.Callback)
            {
                data = (UInt32)reader.ReadInt64(logicalContext);
            }
            else
            {
                data = fieldData.FixedValue;
            }

            return data;
        }

        private UInt64 ReadUInt64(MidiDeviceDataContext context, MidiLogicalContext logicalContext, IMidiLogicalReader reader)
        {
            var fieldData = new FieldData<UInt64>(context);
            UInt64 data;

            if (fieldData.Callback)
            {
                // TODO: How to support UInt64??
                data = (UInt64)reader.ReadInt64(logicalContext);
            }
            else
            {
                data = fieldData.FixedValue;
            }

            return data;
        }

        public void _ToLogical(MidiDeviceDataContext context, IMidiLogicalWriter writer)
        {
            var pos = context.PhysicalStream.Position;
            context.Carry.Clear();

            var logicalContext = context.CreateLogicalContext();
            var reader = new LittleEndianStreamReader(context.CurrentStream);

            switch (_byteLength)
            {
                case 2:
                    {
                        var fieldData = new FieldData<UInt16>(context);
                        UInt16 data = reader.ReadUInt16();

                        if (fieldData.Callback)
                        {
                            writer.Write(logicalContext, data);
                        }

                        context.DataRecords.Add(pos, data, context.CurrentFieldConverter.Field);
                    }
                    break;
                case 3:
                    {
                        var fieldData = new FieldData<UInt32>(context);
                        UInt32 data = reader.ReadUInt24();

                        if (fieldData.Callback)
                        {
                            writer.Write(logicalContext, data);
                        }

                        context.DataRecords.Add(pos, data, context.CurrentFieldConverter.Field);
                    }
                    break;
                case 4:
                    {
                        var fieldData = new FieldData<UInt32>(context);
                        UInt32 data = reader.ReadUInt32();

                        if (fieldData.Callback)
                        {
                            writer.Write(logicalContext, data);
                        }

                        context.DataRecords.Add(pos, data, context.CurrentFieldConverter.Field);
                    }
                    break;
                case 5:
                    {
                        var fieldData = new FieldData<UInt64>(context);
                        // TODO: How to support UInt64??
                        UInt64 data = reader.ReadUInt40();

                        if (fieldData.Callback)
                        {
                            writer.Write(logicalContext, (long)data);
                        }

                        context.DataRecords.Add(pos, data, context.CurrentFieldConverter.Field);
                    }
                    break;
                case 6:
                    {
                        var fieldData = new FieldData<UInt64>(context);
                        // TODO: How to support UInt64??
                        UInt64 data = reader.ReadUInt48();

                        if (fieldData.Callback)
                        {
                            writer.Write(logicalContext, (long)data);
                        }

                        context.DataRecords.Add(pos, data, context.CurrentFieldConverter.Field);
                    }
                    break;
                case 7:
                    {
                        var fieldData = new FieldData<UInt64>(context);
                        // TODO: How to support UInt64??
                        UInt64 data = reader.ReadUInt56();

                        if (fieldData.Callback)
                        {
                            writer.Write(logicalContext, (long)data);
                        }

                        context.DataRecords.Add(pos, data, context.CurrentFieldConverter.Field);
                    }
                    break;
                case 8:
                    {
                        var fieldData = new FieldData<UInt64>(context);
                        // TODO: How to support UInt64??
                        UInt64 data = reader.ReadUInt64();

                        if (fieldData.Callback)
                        {
                            writer.Write(logicalContext, (long)data);
                        }

                        context.DataRecords.Add(pos, data, context.CurrentFieldConverter.Field);
                    }
                    break;
            }
        }

        protected override IConverterProcess CreateProcess(MidiDeviceDataContext context)
        {
            IConverterProcess process = null;

            switch (_byteLength)
            {
                case 2:
                    process = new UInt16Process(context);
                    break;
                case 3:
                    process = new UInt24Process(context);
                    break;
                case 4:
                    process = new UInt32Process(context);
                    break;
                case 5:
                    process = new UInt40Process(context);
                    break;
                case 6:
                    process = new UInt48Process(context);
                    break;
                case 7:
                    process = new UInt56Process(context);
                    break;
                case 8:
                    process = new UInt64Process(context);
                    break;
                default:
                    throw new NotSupportedException("The specified byte length " + _byteLength + " is not supported.");
            }

            return process;
        }
    }
}