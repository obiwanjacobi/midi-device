using System;
using System.Runtime.InteropServices;
using CannedBytes.Midi.Core;

namespace CannedBytes.Midi.Device;

// helps in calling the right method on IMidiLogicalWriter
internal class LogicalWriteAccessor : ILogicalWriteAccessor
{
    private readonly DeviceDataContext _context;
    private readonly IMidiLogicalWriter _writer;

    public LogicalWriteAccessor(DeviceDataContext context, IMidiLogicalWriter writer)
    {
        Assert.IfArgumentNull(context, nameof(context));
        Assert.IfArgumentNull(writer, nameof(writer));

        _context = context;
        _writer = writer;
    }

    // one method decides which one to use.
    public bool Write<T>(T value, int bitLength)
        where T : IComparable
    {
        var fieldValue = _context.FieldInfo.GetDataFieldValue<T>();

        // intercept the value and set it on the current Record Entry.
        if (_context.LogManager?.CurrentEntry != null)
        {
            _context.LogManager.CurrentEntry.Data = value;
        }

        // validate against field constraints
        fieldValue.Validate(value);

        var devicePropName = _context.FieldInfo.CurrentField.ExtendedProperties.DevicePropertyName;
        
        // write device property value
        if (!String.IsNullOrEmpty(devicePropName))
        {
            _context.DeviceProperties.Add(
                _context.FieldInfo.CurrentField.Name.SchemaName, devicePropName, value);
        }

        if (fieldValue.Callback)
        {
            return DispatchValue<T>(value, bitLength);
        }

        return false;
    }

    protected virtual bool DispatchValue<T>(T value, int bitLength)
    {
        var type = typeof(T);
        var ctx = CreateLogicalContext();

        if (type == typeof(string))
        {
            return _writer.WriteString(ctx, value as string);
        }

        if (bitLength == 0)
        {
            bitLength = DetermineBitLength(type);
        }

        ctx.BitLength = bitLength;

        if (bitLength == 1)
        {
            bool nativeValue = (bool)Convert.ChangeType(value, typeof(bool));

            return _writer.WriteBool(ctx, nativeValue);
        }
        else if (bitLength <= 8)
        {
            byte nativeValue = (byte)Convert.ChangeType(value, typeof(byte));

            return _writer.WriteByte(ctx, nativeValue);
        }
        else if (bitLength <= 16)
        {
            short nativeValue = (short)Convert.ChangeType(value, typeof(short));

            return _writer.WriteShort(ctx, nativeValue);
        }
        else if (bitLength <= 32)
        {
            int nativeValue = (int)Convert.ChangeType(value, typeof(int));

            return _writer.WriteInt(ctx, nativeValue);
        }
        else if (bitLength <= 64)
        {
            long nativeValue = (long)Convert.ChangeType(value, typeof(long));

            return _writer.WriteLong(ctx, nativeValue);
        }

        throw new ArgumentException(
                "The specified type is not supported: " + type.FullName);
    }

    private LogicalContext CreateLogicalContext()
    {
        var ctx = _context.CreateLogicalContext();

        return ctx;
    }

    private int DetermineBitLength(Type type)
    {
        int length = _context.FieldInfo.CurrentNode.FieldConverterPair.Converter.ByteLength;

        if (length < 0)
        {
            return Marshal.SizeOf(type);
        }

        return length;
    }
}
