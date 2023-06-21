using System;
using System.IO;
using System.Text;
using CannedBytes.Midi.Core;

namespace CannedBytes.Midi.Device;

/// <summary>
/// A StreamReader that transparently reads midi bits/bytes using the <see cref="BitStreamReader"/>.
/// </summary>
public sealed class DeviceStreamReader
{
    private const int MaxBufferSize = 8;
    private readonly byte[] _buffer = new byte[MaxBufferSize];

    private readonly BitStreamReader _bitReader;

    /// <summary>
    /// Constructs a new instance.
    /// </summary>
    /// <param name="stream">The stream to read from. Must not be null.</param>
    /// <param name="carry">A reference to an existing carry.</param>
    /// <seealso cref="DeviceDataContext"/>
    internal DeviceStreamReader(Stream stream, BitStreamReader bitReader)
    {
        Assert.IfArgumentNull(stream, nameof(stream));
        Assert.IfArgumentNull(bitReader, nameof(bitReader));

        BaseStream = stream;
        _bitReader = bitReader;
    }

    /// <summary>
    /// The stream this instance was constructed with.
    /// </summary>
    public Stream BaseStream { get; }

    /// <summary>
    /// Reads individual bits from the stream.
    /// </summary>
    /// <param name="range">The range of bits to read.</param>
    /// <returns>Returns value read from the stream.</returns>
    public ushort ReadBitRange(ValueRange range)
    {
        return _bitReader.ReadBits(BaseStream, range.Start, range.Length);
    }

    /// <summary>
    /// Reads a maximum of 8 bytes from the stream.
    /// </summary>
    /// <param name="byteLength">The number of bytes to read.</param>
    /// <param name="byteOrder">To override the byte-order that is used to build the value.</param>
    /// <returns>Returns the value represented by the bytes read.</returns>
    public VarUInt64 Read(int byteLength, BitOrder byteOrder = BitOrder.LittleEndian)
    {
        Assert.IfArgumentOutOfRange(byteLength,
            (int)VarUInt64.VarTypeCode.UInt8, (int)VarUInt64.VarTypeCode.UInt64, nameof(byteLength));

        ReadIntoBuffer(byteLength);

        var value = BufferToValue<ulong>(byteLength, byteOrder);

        return new VarUInt64(value);
    }

    /// <summary>
    /// Reads the <paramref name="numOfBytes"/> from the stream.
    /// </summary>
    /// <param name="numOfBytes">Must be greater than zero and smaller than <see cref="MaxBufferSize"/>.</param>
    /// <exception cref="EndOfStreamException">Thrown when less than the specified <paramref name="numOfBytes"/> 
    /// could be read from the stream.</exception>
    private void ReadIntoBuffer(int numOfBytes)
    {
        Assert.IfArgumentOutOfRange(numOfBytes, 0, MaxBufferSize, nameof(numOfBytes));

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
    /// <param name="length">The length of the number of bytes in the buffer. 
    /// Buffer size is fixed and not an indication for target <typeparamref name="T"/>.</param>
    /// <param name="byteOrder">To override the byte-order that is used to build the value.</param>
    /// <returns>Returns the integer value.</returns>
    private T BufferToValue<T>(int length, BitOrder byteOrder)
        where T : struct, IConvertible, IComparable
    {
        var value = (ulong)ByteConverter.FromBytesToInt64(_buffer, length, byteOrder);
        return (T)Convert.ChangeType(value, typeof(T));
    }

    public byte ReadInt8()
    {
        const int size = 1;
        ReadIntoBuffer(size);
        return BufferToValue<byte>(size, BitOrder.LittleEndian);
    }

    public short ReadInt16(BitOrder byteOrder = BitOrder.LittleEndian)
    {
        const int size = 2;
        ReadIntoBuffer(size);
        return BufferToValue<short>(size, byteOrder);
    }

    public int ReadInt32(BitOrder byteOrder = BitOrder.LittleEndian)
    {
        const int size = 4;
        ReadIntoBuffer(size);
        return BufferToValue<int>(size, byteOrder);
    }

    public long ReadInt64(BitOrder byteOrder = BitOrder.LittleEndian)
    {
        const int size = 8;
        ReadIntoBuffer(size);
        return BufferToValue<long>(size, byteOrder);
    }

    public ushort ReadUInt16(BitOrder byteOrder = BitOrder.LittleEndian)
    {
        const int size = 2;
        ReadIntoBuffer(size);
        return BufferToValue<ushort>(size, byteOrder);
    }

    public uint ReadUInt24(BitOrder byteOrder = BitOrder.LittleEndian)
    {
        const int size = 3;
        ReadIntoBuffer(size);
        return BufferToValue<uint>(size, byteOrder);
    }

    public uint ReadUInt32(BitOrder byteOrder = BitOrder.LittleEndian)
    {
        const int size = 4;
        ReadIntoBuffer(size);
        return BufferToValue<uint>(size, byteOrder);
    }

    public ulong ReadUInt40(BitOrder byteOrder = BitOrder.LittleEndian)
    {
        const int size = 5;
        ReadIntoBuffer(size);
        return BufferToValue<ulong>(size, byteOrder);
    }

    public ulong ReadUInt48(BitOrder byteOrder = BitOrder.LittleEndian)
    {
        const int size = 6;
        ReadIntoBuffer(size);
        return BufferToValue<ulong>(size, byteOrder);
    }

    public ulong ReadUInt56(BitOrder byteOrder = BitOrder.LittleEndian)
    {
        const int size = 7;
        ReadIntoBuffer(size);
        return BufferToValue<ulong>(size, byteOrder);
    }

    public ulong ReadUInt64(BitOrder byteOrder = BitOrder.LittleEndian)
    {
        const int size = 8;
        ReadIntoBuffer(size);
        return BufferToValue<ulong>(size, byteOrder);
    }

    /// <summary>
    /// Reads a string of a fixed length from the stream.
    /// </summary>
    /// <param name="byteLength">The number of characters in the stream. No terminating 0.</param>
    /// <returns>Returns the string read.</returns>
    /// <exception cref="EndOfStreamException">Thrown when less than the specified <paramref name="byteLength"/> 
    /// could be read from the stream.</exception>
    public string ReadStringAscii(int byteLength)
    {
        // use own buffer to be able to read strings with larger length than MaxBufferSize.
        var buffer = new byte[byteLength];

        if (BaseStream.Read(buffer, 0, byteLength) != byteLength)
        {
            throw new EndOfStreamException();
        }

        return Encoding.ASCII.GetString(buffer).TrimEnd();
    }
}
