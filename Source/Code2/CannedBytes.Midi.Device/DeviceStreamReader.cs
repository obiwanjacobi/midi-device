using System;
using System.IO;
using System.Text;
using CannedBytes.Midi.Core;

namespace CannedBytes.Midi.Device;

/// <summary>
/// A StreamReader that transparently reads midi bits/bytes using the <see cref="Carry"/>.
/// </summary>
public class DeviceStreamReader
{
    /// <summary>
    /// Constructs a new instance.
    /// </summary>
    /// <param name="stream">The stream to read from. Must not be null.</param>
    /// <param name="carry">A reference to an existing carry.</param>
    /// <seealso cref="DeviceDataContext"/>
    public DeviceStreamReader(Stream stream, Carry carry)
    {
        Check.IfArgumentNull(stream, nameof(stream));
        Check.IfArgumentNull(carry, nameof(carry));

        BaseStream = stream;
        Carry = carry;
    }

    /// <summary>
    /// The stream this instance was constructed with.
    /// </summary>
    public Stream BaseStream { get; }

    /// <summary>
    /// The carry this instance was constructed with.
    /// </summary>
    public Carry Carry { get; }

    /// <summary>
    /// Reads a maximum of 16 individual bits.
    /// </summary>
    /// <param name="bitFlags">Flags that indicate what bits to read.</param>
    /// <param name="value">The resulting value.</param>
    /// <returns>Returns the number of bytes actually read from the stream.</returns>
    public int Read(BitFlags bitFlags, out ushort value)
    {
        return Carry.ReadFrom(BaseStream, bitFlags, out value);
    }

    /// <summary>
    /// Reads a maximum of 8 bytes from the stream.
    /// </summary>
    /// <param name="byteLength">The number of bytes to read.</param>
    /// <returns>Returns the value represented by the bytes read.</returns>
    public VarUInt64 Read(int byteLength)
    {
        Check.IfArgumentOutOfRange(byteLength,
            (int)VarUInt64.VarTypeCode.UInt8, (int)VarUInt64.VarTypeCode.UInt64, nameof(byteLength));

        FillBuffer(byteLength);

        ulong value = FormatBuffer<ulong>(_buffer, byteLength);

        return new VarUInt64(value);
    }

    // LE/BE implementation
    protected const int MaxBufferSize = 8;
    private readonly byte[] _buffer = new byte[MaxBufferSize];

    /// <summary>
    /// Reads the <paramref name="numOfBytes"/> from the stream.
    /// </summary>
    /// <param name="numOfBytes">Must be greater than zero and smaller than <see cref="MaxBufferSize"/>.</param>
    /// <exception cref="EndOfStreamException">Thrown when less than the specified <paramref name="numOfBytes"/> 
    /// could be read from the stream.</exception>
    private void FillBuffer(int numOfBytes)
    {
        Check.IfArgumentOutOfRange(numOfBytes, 0, MaxBufferSize, nameof(numOfBytes));

        int bytesRead = BaseStream.Read(_buffer, 0, numOfBytes);

        if (bytesRead < numOfBytes)
        {
            throw new EndOfStreamException();
        }
    }

    /// <summary>
    /// Implemented by derived class to implement BE/LE formatting
    /// </summary>
    /// <typeparam name="T">The data type to return.</typeparam>
    /// <param name="buffer">The buffer with the read bytes.</param>
    /// <param name="length">The length of the number of bytes in the buffer. 
    /// Buffer size is fixed and not an indication for target <typeparamref name="T"/>.</param>
    /// <returns>Returns the integer value.</returns>
    protected virtual T FormatBuffer<T>(byte[] buffer, int length)
        where T : struct, IConvertible, IComparable
    {
        Type type = typeof(T);
        int shift = 0;
        ulong value = 0;

        for (int i = 0; i < length; i++)
        {
            ulong data = (ulong)buffer[i] << shift;

            value |= data;

            shift += 8;
        }

        return (T)Convert.ChangeType(value, type);
    }

    public byte ReadInt8()
    {
        const int size = 1;

        FillBuffer(size);

        return FormatBuffer<byte>(_buffer, size);
    }

    public short ReadInt16()
    {
        const int size = 2;

        FillBuffer(size);

        return FormatBuffer<short>(_buffer, size);
    }

    public int ReadInt32()
    {
        const int size = 4;

        FillBuffer(size);

        return FormatBuffer<int>(_buffer, size);
    }

    public long ReadInt64()
    {
        const int size = 8;

        FillBuffer(size);

        return FormatBuffer<long>(_buffer, size);
    }

    public ushort ReadUInt16()
    {
        const int size = 2;

        FillBuffer(size);

        return FormatBuffer<ushort>(_buffer, size);
    }

    public uint ReadUInt24()
    {
        const int size = 3;

        FillBuffer(size);

        return FormatBuffer<uint>(_buffer, size);
    }

    public uint ReadUInt32()
    {
        const int size = 4;

        FillBuffer(size);

        return FormatBuffer<uint>(_buffer, size);
    }

    public ulong ReadUInt40()
    {
        const int size = 5;

        FillBuffer(size);

        return FormatBuffer<ulong>(_buffer, size);
    }

    public ulong ReadUInt48()
    {
        const int size = 6;

        FillBuffer(size);

        return FormatBuffer<ulong>(_buffer, size);
    }

    public ulong ReadUInt56()
    {
        const int size = 7;

        FillBuffer(size);

        return FormatBuffer<ulong>(_buffer, size);
    }

    public ulong ReadUInt64()
    {
        const int size = 8;

        FillBuffer(size);

        return FormatBuffer<ulong>(_buffer, size);
    }

    /// <summary>
    /// Reads a string of a fixed length from the stream.
    /// </summary>
    /// <param name="byteLength">The number of characters in the stream. No terminating 0.</param>
    /// <returns>Returns the string read.</returns>
    /// <exception cref="EndOfStreamException">Thrown when less than the specified <paramref name="byteLength"/> 
    /// could be read from the stream.</exception>
    /// <remarks>Uses UTF7 encoding.</remarks>
    public string ReadString(int byteLength)
    {
        // use own buffer to be able to read strings with larger length than MaxBufferSize.
        byte[] buffer = new byte[byteLength];

        if (BaseStream.Read(buffer, 0, byteLength) != byteLength)
        {
            throw new EndOfStreamException();
        }

        return Encoding.UTF7.GetString(buffer);
    }
}
