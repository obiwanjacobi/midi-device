using System;

namespace CannedBytes.Midi.Core;

public static class ByteConverter
{
    private const uint IntMaskByte1 = 0x0000007F;
    private const uint IntMaskByte2 = 0x00003F80;
    private const uint IntMaskByte3 = 0x001FC000;
    private const uint IntMaskByte4 = 0x0FE00000;

    public static byte[] FromUint32ToSevenBitBytes(uint value, Ordering ordering)
    {
        var bytes = new byte[4];

        bytes[0] = (byte)((value & IntMaskByte1) >> 0);
        bytes[1] = (byte)((value & IntMaskByte2) >> 7);
        bytes[2] = (byte)((value & IntMaskByte3) >> 14);
        bytes[3] = (byte)((value & IntMaskByte4) >> 21);

        if (ordering == Ordering.BigEndian)
        {
            Array.Reverse(bytes);
        }

        return bytes;
    }

    private const uint ByteMask = 0x7F;

    public static uint FromSevenBitBytesToUInt32(byte[] bytes, Ordering ordering)
    {
        uint internalValue = 0;

        if (ordering == Ordering.LittleEndian)
        {
            for (int i = 0; i < Math.Min(bytes.Length, 4); i++)
            {
                internalValue |= ((uint)bytes[i] & ByteMask) << (i * 7);
            }
        }
        else
        {
            int s = 0;
            for (int i = Math.Min(bytes.Length, 4) - 1; i >= 0; i--)
            {
                internalValue |= ((uint)bytes[i] & ByteMask) << (s * 7);
                s++;
            }
        }

        return internalValue;
    }

    public static int FromBytesToInt32(byte[] bytes, Ordering ordering)
    {
        int value = 0;

        if (ordering == Ordering.LittleEndian)
        {
            for (int i = 0; i < Math.Min(bytes.Length, 4); i++)
            {
                value |= ((int)bytes[i]) << (i * 8);
            }
        }
        else
        {
            int s = 0;
            for (int i = Math.Min(bytes.Length, 4) - 1; i >= 0; i--)
            {
                value |= ((int)bytes[i]) << (s * 8);
                s++;
            }
        }

        return value;
    }

    public static long FromBytesToInt64(byte[] bytes, Ordering ordering)
    {
        long value = 0;

        if (ordering == Ordering.LittleEndian)
        {
            for (int i = 0; i < Math.Min(bytes.Length, 8); i++)
            {
                value |= ((long)bytes[i]) << (i * 8);
            }
        }
        else
        {
            int s = 0;
            for (int i = Math.Min(bytes.Length, 8) - 1; i >= 0; i--)
            {
                value |= ((long)bytes[i]) << (s * 8);
                s++;
            }
        }

        return value;
    }

    private const ulong LowByteMask = 0xFF;

    public static byte[] FromUInt64ToBytes(ulong value, Ordering ordering)
    {
        var bytes = new byte[8];

        bytes[0] = (byte)((value >> 0) & LowByteMask);
        bytes[1] = (byte)((value >> 8) & LowByteMask);
        bytes[2] = (byte)((value >> 16) & LowByteMask);
        bytes[3] = (byte)((value >> 24) & LowByteMask);
        bytes[4] = (byte)((value >> 32) & LowByteMask);
        bytes[5] = (byte)((value >> 40) & LowByteMask);
        bytes[6] = (byte)((value >> 48) & LowByteMask);
        bytes[7] = (byte)((value >> 56) & LowByteMask);

        if (ordering == Ordering.BigEndian)
        {
            Array.Reverse(bytes);
        }

        return bytes;
    }

    //public static byte[] TrimEnd(byte[] bytes)
    //{
    //    // TODO: trim empty slots
    //    return bytes;
    //}
}