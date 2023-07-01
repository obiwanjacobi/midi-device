using System;

namespace CannedBytes.Midi.Device.IntegrationTests.Stubs;

public class ConsoleLogicalWriterStub : IMidiLogicalWriter
{
    private readonly IMidiLogicalWriter writer;

    public ConsoleLogicalWriterStub(IMidiLogicalWriter nestedWriter)
    {
        writer = nestedWriter;
    }

    private void Log<T>(LogicalContext context, T data)
    {
        Console.Write(context.FieldInfo.Field.ToString());
        Console.Write("[");
        Console.Write(context.FieldInfo.Key.ToString());
        Console.Write("] = ");
        Console.WriteLine(data);
    }

    public bool WriteBool(LogicalContext context, bool data)
    {
        Log(context, data);

        writer?.WriteBool(context, data);

        return true;
    }

    public bool WriteByte(LogicalContext context, byte data)
    {
        Log(context, data);

        writer?.WriteByte(context, data);

        return true;
    }

    public bool WriteShort(LogicalContext context, short data)
    {
        Log(context, data);

        writer?.WriteShort(context, data);

        return true;
    }

    public bool WriteInt(LogicalContext context, int data)
    {
        Log(context, data);

        writer?.WriteInt(context, data);

        return true;
    }

    public bool WriteLong(LogicalContext context, long data)
    {
        Log(context, data);

        writer?.WriteLong(context, data);

        return true;
    }

    public bool WriteString(LogicalContext context, string data)
    {
        Log(context, data);

        writer?.WriteString(context, data);

        return true;
    }
}