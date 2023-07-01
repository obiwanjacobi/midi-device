using System;
using System.Collections.ObjectModel;
using System.Linq;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.IntegrationTests.Stubs;

public class DictionaryBasedLogicalStub : KeyedCollection<string, DictionaryBasedLogicalStub.FieldInfo>,
    IMidiLogicalWriter, IMidiLogicalReader
{
    protected override string GetKeyForItem(FieldInfo item)
    {
        return item.Key!;
    }

    public T GetValue<T>(ILogicalFieldInfo fieldInfo)
    {
        string key = BuildKey(fieldInfo);

        var fldInfo = this[key];

        return ConvertTo.ChangeType<object?, T>(fldInfo.Value);
    }

    public FieldInfo Add(ILogicalFieldInfo logicFieldInfo, object value)
    {
        var fldInfo = new FieldInfo()
        {
            InstanceIndex = logicFieldInfo.Key.Values.Last(),
            Key = BuildKey(logicFieldInfo),
            Field = logicFieldInfo.Field,
            LogicalFieldInfo = logicFieldInfo,
            Value = value
        };

        Add(fldInfo);

        return fldInfo;
    }

    public FieldInfo AddStub(string fullFieldName, object value)
    {
        var fldInfo = new FieldInfo()
        {
            InstanceIndex = 0,
            Key = BuildKey(fullFieldName, "0"),
            Value = value
        };

        Add(fldInfo);

        return fldInfo;
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

    private static string BuildKey(ILogicalFieldInfo fieldInfo)
    {
        return $"{fieldInfo.Field.Name.FullName}[{fieldInfo.Key}]";
    }

    private static string BuildKey(string fullFieldName, string instanceKey)
    {
        return $"{fullFieldName}[{instanceKey}]";
    }

    //-------------------------------------------------------------------------

    public class FieldInfo
    {
        public string? Key { get; set; }
        public Field? Field { get; set; }
        public int InstanceIndex { get; set; }
        public object? Value { get; set; }

        public ILogicalFieldInfo? LogicalFieldInfo { get; set; }

        public override string ToString()
        {
            return $"{Field}[{InstanceIndex}] = {Value}";
        }
    }
}