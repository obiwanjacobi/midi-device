using System;

namespace CannedBytes.Midi.Device.Converters
{
    public abstract class ConverterProcess<T> : IConverterProcess
        where T : IComparable
    {
        public ConverterProcess(MidiDeviceDataContext context)
        {
            this.Context = context;

            var t = typeof(T);

            if (t == typeof(bool))
            {
                DataType = LogicDataType.Boolean;
            }
            if (t == typeof(byte))
            {
                DataType = LogicDataType.Byte;
            }
            if (t == typeof(int) || t == typeof(short) || t == typeof(ushort))
            {
                DataType = LogicDataType.Int32;
            }
            if (t == typeof(long) || t == typeof(uint))
            {
                DataType = LogicDataType.Int64;
            }
            if (t == typeof(string))
            {
                DataType = LogicDataType.String;
            }
        }

        public LogicDataType DataType { get; protected set; }

        private FieldData<T> _fieldData;

        public FieldData<T> FieldData
        {
            get
            {
                if (_fieldData == null)
                {
                    _fieldData = new FieldData<T>(this.Context);
                }

                return _fieldData;
            }
        }

        public MidiDeviceDataContext Context { get; protected set; }

        private long _streamPosition;

        protected bool FlushedCarry { get; set; }

        public bool ReadFromExtension(IConverterExtension extension)
        {
            _streamPosition = this.Context.PhysicalStream.Position;

            if (extension != null)
            {
                this.Value = extension.Read<T>(Context);

                return true;
            }

            return false;
        }

        public virtual bool ReadFromContext()
        {
            _streamPosition = this.Context.PhysicalStream.Position;

            return true;
        }

        public virtual void ProcessToLogical()
        {
            this.FieldData.Validate(this.Value);
        }

        public virtual void AddDataLogRecord()
        {
            this.Context.DataRecords.Add(_streamPosition, Value, this.Context.CurrentFieldConverter.Field, FlushedCarry);
        }

        public virtual void ToLogical(IMidiLogicalWriter writer)
        {
            if (this.FieldData.Callback)
            {
                var logicalCtx = this.Context.CreateLogicalContext();

                switch (this.DataType)
                {
                    case LogicDataType.Boolean:
                        writer.Write(logicalCtx, GetValue<bool>());
                        break;
                    case LogicDataType.Byte:
                        writer.Write(logicalCtx, GetValue<byte>());
                        break;
                    case LogicDataType.Int32:
                        writer.Write(logicalCtx, GetValue<int>());
                        break;
                    case LogicDataType.Int64:
                        writer.Write(logicalCtx, GetValue<long>());
                        break;
                    case LogicDataType.String:
                        writer.Write(logicalCtx, GetValue<string>());
                        break;
                    default:
                        throw new NotSupportedException("Converter logical data type not supported.");
                }
            }
        }

        public T Value { get; protected set; }

        public TValue GetValue<TValue>()
        {
            return (TValue)Convert.ChangeType(Value, typeof(TValue));
        }

        public void SetValue<TValue>(TValue value)
        {
            Value = (T)Convert.ChangeType(value, typeof(T));
        }

        public bool WriteToExtension(IConverterExtension extension)
        {
            if (extension != null)
            {
                extension.Write(this.Context, Value);
                return true;
            }

            return false;
        }

        public virtual bool WriteToContext()
        {
            return true;
        }

        public virtual void ProcessToPhysical()
        {
            _streamPosition = this.Context.PhysicalStream.Position;

            this.FieldData.Validate(Value);
        }

        public virtual void ToPhysical(IMidiLogicalReader reader)
        {
            if (this.FieldData.Callback)
            {
                var logicalCtx = this.Context.CreateLogicalContext();

                switch (this.DataType)
                {
                    case LogicDataType.Boolean:
                        SetValue(reader.ReadBool(logicalCtx));
                        break;
                    case LogicDataType.Byte:
                        SetValue(reader.ReadByte(logicalCtx));
                        break;
                    case LogicDataType.Int32:
                        SetValue(reader.ReadInt32(logicalCtx));
                        break;
                    case LogicDataType.Int64:
                        SetValue(reader.ReadInt64(logicalCtx));
                        break;
                    case LogicDataType.String:
                        SetValue(reader.ReadString(logicalCtx));
                        break;
                    default:
                        throw new NotSupportedException("Converter logical data type not supported.");
                }
            }
            else
            {
                Value = this.FieldData.FixedValue;
            }
        }

        public enum LogicDataType
        {
            Unknown,
            Boolean,
            Byte,
            Int32,
            Int64,
            String,
        }
    }
}