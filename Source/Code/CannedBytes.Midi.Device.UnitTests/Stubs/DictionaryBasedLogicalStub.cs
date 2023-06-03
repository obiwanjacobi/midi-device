using System;
using System.Collections.Generic;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.UnitTests.Stubs
{
    public class DictionaryBasedLogicalStub : IMidiLogicalWriter, IMidiLogicalReader
    {
        private List<FieldInfo> fieldList = new List<FieldInfo>();

        public DictionaryBasedLogicalStub()
        {
            FieldValues = new Dictionary<string, object>();
        }

        public Dictionary<string, object> FieldValues { get; private set; }

        public void Write(MidiLogicalContext context, bool data)
        {
            AddValue(context, data);
        }

        public void Write(MidiLogicalContext context, byte data)
        {
            AddValue(context, data);
        }

        public void Write(MidiLogicalContext context, int data)
        {
            AddValue(context, data);
        }

        public void Write(MidiLogicalContext context, long data)
        {
            AddValue(context, data);
        }

        public void Write(MidiLogicalContext context, string data)
        {
            AddValue(context, data);
        }

        public IEnumerable<FieldInfo> Fields
        {
            get { return fieldList; }
        }

        private void AddValue<T>(MidiLogicalContext context, T data)
        {
            var fieldInfo = new FieldInfo();
            fieldInfo.Field = context.Field;
            fieldInfo.Key = context.Key;

            fieldList.Add(fieldInfo);

            AddValue<T>(context.Field.Name.FullName, context.Key.ToString(), data);
        }

        private T ReadValue<T>(Field field, string key)
        {
            string mapKey = BuildKey(field.Name.FullName, key);

            if (FieldValues.ContainsKey(mapKey))
            {
                object value = FieldValues[mapKey];

                if (value != null)
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
            }

            Console.WriteLine("WARNING: Logical Stub could not find a value found for: " + mapKey);
            return default(T);
        }

        private string BuildKey(string name, string key)
        {
            //if (instanceIndex > 0)
            {
                return name + "[" + key + "]";
            }

            //return name;
        }

        public void AddValue<T>(string fieldName, string key, T value)
        {
            FieldValues.Add(BuildKey(fieldName, key), value);
        }

        public void AddValue<T>(string fieldName, int instanceIndex, T value)
        {
            if (instanceIndex >= 0)
            {
                var key = new FieldPathKey();
                key.Add(instanceIndex);

                FieldValues.Add(BuildKey(fieldName, key.ToString()), value);
            }
            else
            {
                FieldValues.Add(fieldName, value);
            }
        }

        public bool ReadBool(MidiLogicalContext context)
        {
            return ReadValue<bool>(context.Field, context.Key.ToString());
        }

        public byte ReadByte(MidiLogicalContext context)
        {
            return ReadValue<byte>(context.Field, context.Key.ToString());
        }

        public int ReadInt32(MidiLogicalContext context)
        {
            return ReadValue<Int32>(context.Field, context.Key.ToString());
        }

        public long ReadInt64(MidiLogicalContext context)
        {
            return ReadValue<Int64>(context.Field, context.Key.ToString());
        }

        public string ReadString(MidiLogicalContext context)
        {
            return ReadValue<string>(context.Field, context.Key.ToString());
        }

        public class FieldInfo
        {
            public Field Field;
            public FieldPathKey Key;

            public override string ToString()
            {
                return Field.ToString() + " [" + Key.ToString() + "]";
            }
        }

        public void Clear()
        {
            this.fieldList.Clear();
            this.FieldValues.Clear();
        }
    }
}