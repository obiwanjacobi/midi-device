using System;
using System.Collections.ObjectModel;
using System.Linq;
using CannedBytes.Midi.Device;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.DeviceTestApp.Midi
{
    class DeviceLogicalData : IMidiLogicalWriter, IMidiLogicalReader
    {
        public DeviceLogicalData()
        {
            this.Values = new ObservableCollection<LogicalValue>();
        }

        public ObservableCollection<LogicalValue> Values { get; private set; }

        private void AddValue<T>(Field field, FieldPathKey key, T value)
        {
            var logValue = new LogicalValue(field, key);
            logValue.Value = value;

            this.Values.Add(logValue);
        }

        private T GetValue<T>(Field field, FieldPathKey key)
        {
            return (from logValue in this.Values
                    where logValue.Field.Name.FullName == field.Name.FullName
                    where logValue.Key.Equals(key)
                    select logValue.Value).Cast<T>().FirstOrDefault();
        }

        public void Write(MidiLogicalContext context, string data)
        {
            AddValue(context.Field, context.Key, data);
        }

        public void Write(MidiLogicalContext context, long data)
        {
            AddValue(context.Field, context.Key, data);
        }

        public void Write(MidiLogicalContext context, int data)
        {
            AddValue(context.Field, context.Key, data);
        }

        public void Write(MidiLogicalContext context, byte data)
        {
            AddValue(context.Field, context.Key, data);
        }

        public void Write(MidiLogicalContext context, bool data)
        {
            AddValue(context.Field, context.Key, data);
        }

        public bool ReadBool(MidiLogicalContext context)
        {
            return GetValue<bool>(context.Field, context.Key);
        }

        public byte ReadByte(MidiLogicalContext context)
        {
            return GetValue<byte>(context.Field, context.Key);
        }

        public int ReadInt32(MidiLogicalContext context)
        {
            return GetValue<int>(context.Field, context.Key);
        }

        public long ReadInt64(MidiLogicalContext context)
        {
            return GetValue<long>(context.Field, context.Key);
        }

        public string ReadString(MidiLogicalContext context)
        {
            return GetValue<string>(context.Field, context.Key);
        }
    }

    class LogicalValue
    {
        public LogicalValue(Field field, FieldPathKey key)
        {
            this.Field = field;
            this.Key = key;
        }

        public Field Field { get; private set; }

        public FieldPathKey Key { get; private set; }

        public object Value { get; set; }

        public T GetValue<T>()
        {
            return (T)Convert.ChangeType(this.Value, typeof(T));
        }

        public override string ToString()
        {
            return this.Field.ToString() + " [" + this.Key.ToString() + "]";
        }
    }
}