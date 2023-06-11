using System;
using System.Collections.ObjectModel;
using System.Linq;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.IntegrationTests.Stubs;

public class DictionaryBasedLogicalStub : KeyedCollection<string, DictionaryBasedLogicalStub.FieldInfo>, IMidiLogicalWriter, IMidiLogicalReader
{
    protected override string GetKeyForItem(FieldInfo item)
    {
        return item.Key;
    }

    public T GetValue<T>(ILogicalFieldInfo fieldInfo)
    {
        return GetValue<T>(fieldInfo.Field.Name.FullName, fieldInfo.Key.Values.Last());
    }

    public T GetValue<T>(string fieldName, int instanceIndex)
    {
        string key = BuildKey(fieldName, instanceIndex);

        var fldInfo = this[key];

        return (T)Convert.ChangeType(fldInfo.Value, typeof(T));
    }

    public FieldInfo Add(ILogicalFieldInfo logicFieldInfo, object value)
    {
        FieldInfo fldInfo = new()
        {
            InstanceIndex = logicFieldInfo.Key.Values.Last()
        };
        fldInfo.Key = BuildKey(logicFieldInfo.Field.Name.FullName, fldInfo.InstanceIndex);
        fldInfo.Field = logicFieldInfo.Field;
        fldInfo.LogicalFieldInfo = logicFieldInfo;
        fldInfo.Value = value;

        Add(fldInfo);

        return fldInfo;
    }

    public static string BuildKey(string fieldName, int instanceIndex)
    {
        return fieldName + "[" + instanceIndex + "]";
    }

    public bool WriteBool(LogicalContext context, bool data)
    {
        Add(context.FieldInfo, data);
        return true;
    }

    public bool WriteByte(LogicalContext context, byte data)
    {
        Add(context.FieldInfo, data);
        return true;
    }

    public bool WriteShort(LogicalContext context, short data)
    {
        Add(context.FieldInfo, data);
        return true;
    }

    public bool WriteInt(LogicalContext context, int data)
    {
        Add(context.FieldInfo, data);
        return true;
    }

    public bool WriteLong(LogicalContext context, long data)
    {
        Add(context.FieldInfo, data);
        return true;
    }

    public bool WriteString(LogicalContext context, string data)
    {
        Add(context.FieldInfo, data);
        return true;
    }

    public bool ReadBool(LogicalContext context, out bool value)
    {
        value = GetValue<bool>(context.FieldInfo);
        return true;
    }

    public bool ReadByte(LogicalContext context, out byte value)
    {
        value = GetValue<byte>(context.FieldInfo);
        return true;
    }

    public bool ReadShort(LogicalContext context, out short value)
    {
        value = GetValue<short>(context.FieldInfo);
        return true;
    }

    public bool ReadInt(LogicalContext context, out int value)
    {
        value = GetValue<int>(context.FieldInfo);
        return true;
    }

    public bool ReadLong(LogicalContext context, out long value)
    {
        value = GetValue<long>(context.FieldInfo);
        return true;
    }

    public bool ReadString(LogicalContext context, out string value)
    {
        value = GetValue<string>(context.FieldInfo);
        return true;
    }

    //-------------------------------------------------------------------------

    public class FieldInfo
    {
        public string Key { get; set; }
        public Field Field { get; set; }
        public int InstanceIndex { get; set; }
        public object Value { get; set; }

        public ILogicalFieldInfo LogicalFieldInfo { get; set; }
    }
}