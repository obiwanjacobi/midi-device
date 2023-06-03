using System.IO;

namespace CannedBytes.Midi.Device
{
    /// <summary>
    /// The Carry class represents a one ushort data cache that is accessible at bit level.
    /// </summary>
    public class Carry
    {
        private ushort _carry;

        /// <summary>
        /// Gets or sets the data cache ushort value.
        /// </summary>
        /// <remarks>When setting the value the <see cref="BitFlags"/> are all set
        /// to indicate that all bits are available.</remarks>
        public ushort Data
        {
            get { return _carry; }
            set
            {
                _carry = value;
                _bitFlags = BitFlags.Word;
            }
        }

        private BitFlags _bitFlags;

        /// <summary>
        /// Gets the flags that indicate which bits are available.
        /// </summary>
        public BitFlags BitFlags
        {
            get { return _bitFlags; }
        }

        /// <summary>
        /// Tests the <see cref="BitFlags"/> on one or more matches.
        /// </summary>
        /// <param name="bitFlag">The value to test for.</param>
        /// <returns>Returns true when one or more of the specified <paramref name="bitFlags"/>
        /// match with the <see cref="BitFlags"/> property value.</returns>
        public bool IsOneOfTheBitsSet(BitFlags bitFlag)
        {
            return ((_bitFlags & bitFlag) > 0);
        }

        /// <summary>
        /// Tests if the <see cref="BitFlags"/> exactly matches the <paramref name="bitFlags"/>.
        /// </summary>
        /// <param name="bitFlag">The value to test for.</param>
        /// <returns>Returns true when all of the specified <paramref name="bitFlags"/>
        /// match with the <see cref="BitFlags"/> property value.</returns>
        public bool AreAllBitsSet(BitFlags bitFlag)
        {
            return ((_bitFlags & bitFlag) == bitFlag);
        }

        /// <summary>
        /// Initializes the Carry to a new value.
        /// </summary>
        /// <param name="value">The data byte value to cache.</param>
        /// <param name="bitFlags">The corresponding bit flags.</param>
        public void Set(ushort value, BitFlags bitFlags)
        {
            _carry = value;
            _bitFlags = bitFlags;
        }

        /// <summary>
        /// Reads the bits for the Carry data indicated by the <paramref name="bitFlags"/>.
        /// </summary>
        /// <param name="bitFlags">The bits to read from the Carry.</param>
        /// <returns>Returns the data value.</returns>
        /// <remarks>The specified <paramref name="bitFlags"/> are reset
        /// - but be read again on the same Carry data.</remarks>
        public ushort Read(BitFlags bitFlags)
        {
            _bitFlags &= ~bitFlags;
            return (ushort)(_carry & (ushort)bitFlags);
        }

        /// <summary>
        /// Reads a data byte from the <paramref name="stream"/> using the <paramref name="bitFlags"/>.
        /// </summary>
        /// <param name="stream">Can be null. If null only the length (return value) is processed.</param>
        /// <param name="bitFlags">The flags indicating which bits are read.</param>
        /// <param name="value">Receives the data byte that contains valid bits at the <paramref name="bitFlags"/> positions.</param>
        /// <returns>Returns the number of bytes that are read from the <paramref name="stream"/>.</returns>
        /// <remarks>New bytes from the underlying stream are only read and stored
        /// when the <paramref name="bitFlags"/> are not available in the <see cref="BitFlags"/>.
        /// Otherwise the bits of data from the carry is returned.</remarks>
        public virtual int ReadFrom(Stream stream, BitFlags bitFlags, out ushort value)
        {
            // NOTE: stream can be null!

            int newLength = 0;
            value = 0;

            // test if we can read all bits from carry
            if (AreAllBitsSet(bitFlags))
            {
                value = Read(bitFlags);
            }
            else
            {
                var currentLength = ByteLength(BitFlags);
                newLength = ByteLength(bitFlags);
                BitFlags newFlags = BitFlags.None;

                if (newLength == 2)
                {
                    // new value is either hiByte or Word.
                    if (!IsOneOfTheBitsSet(Device.BitFlags.Word))
                    {
                        // no word flags are set
                        value = ReadWordLittleEndian(stream);
                        newFlags = BitFlags.Word;

                        Set(value, newFlags);
                    }
                    else if (!IsOneOfTheBitsSet(Device.BitFlags.HiByte))
                    {
                        // no hiByte flags are set
                        value = ReadByte(stream);

                        newFlags = BitFlags.HiByte;
                        newLength = 1;

                        // make it a hi byte
                        value = (ushort)((int)value << 8);

                        Write(value, newFlags);
                    }
                    //else if (!IsBitSet(Device.BitFlags.LoByte))
                    //{
                    //    // no loByte flags are set
                    //    value = ReadByte(stream);
                    //    newFlags = BitFlags.LoByte;
                    //    newLength = 1;

                    //    Write(value, newFlags);
                    //}
                    else
                    {
                        // current carry will be lost
                        value = ReadWordLittleEndian(stream);
                        newFlags = BitFlags.Word;

                        Set(value, newFlags);
                    }
                }
                else
                {
                    // deal with loByte
                    if (!IsOneOfTheBitsSet(Device.BitFlags.LoByte))
                    {
                        value = ReadByte(stream);
                        newFlags = BitFlags.LoByte;

                        Write(value, newFlags);
                    }
                    else
                    {
                        // current carry will be lost
                        value = ReadByte(stream);
                        newFlags = BitFlags.LoByte;

                        Set(value, newFlags);
                    }
                }

                value = Read(bitFlags);
            }

            return newLength;
        }

        protected byte ReadByte(Stream stream)
        {
            byte value = 0;

            if (stream != null)
            {
                int temp = stream.ReadByte();

                if (temp == -1)
                {
                    throw new EndOfStreamException();
                }

                value = (byte)temp;
            }

            return value;
        }

        protected ushort ReadWordLittleEndian(Stream stream)
        {
            var lsb = ReadByte(stream);
            var msb = ReadByte(stream);
            var value = (int)msb << 8 | (int)lsb;

            return (ushort)value;
        }

        public int WriteTo(Stream stream, ushort value, BitFlags flags)
        {
            Check.IfArgumentNull(stream, "stream");

            int bytesWritten = 0;

            // test if value can be written directly into carry
            if (IsOneOfTheBitsSet(flags))
            {
                // flush what is in the carry to stream
                bytesWritten = Flush(stream);
            }

            Write(value, flags);

            return bytesWritten;
        }

        public int Flush(Stream stream)
        {
            Check.IfArgumentNull(stream, "stream");

            var length = ByteLength(BitFlags);

            if (length == 2)
            {
                var value = Read(Device.BitFlags.Word);
                WriteWordLittleEndian(stream, value);
            }
            else if (length == 1)
            {
                var value = Read(Device.BitFlags.LoByte);
                stream.WriteByte((byte)value);
            }
            else
            {
                // length = 0
                // no-op
            }

            Clear();

            return length;
        }

        private void WriteWordLittleEndian(Stream stream, ushort value)
        {
            byte lsb = (byte)value;
            byte msb = (byte)((int)value >> 8);

            stream.WriteByte(lsb);
            stream.WriteByte(msb);
        }

        /// <summary>
        /// Writes the bits of the <paramref name="value"/> into the Carry indicated by the <paramref name="bitFlags"/>.
        /// </summary>
        /// <param name="value">The data value to write.</param>
        /// <param name="bitFlags">The bits of the data value to write.</param>
        /// <returns>Returns true when successful or false when (one of) the specified
        /// <paramref name="bitFlags"/> has already been written.</returns>
        /// <remarks>When a value is written the <paramref name="bitFlags"/> are set
        /// in the <see cref="BitFlags"/> property.
        /// All bits indicated by <paramref name="bitFlags"/> of the <paramref name="value"/>
        /// are copied to the <see cref="Data"/> property.</remarks>
        public bool Write(ushort value, BitFlags bitFlags)
        {
            if (IsOneOfTheBitsSet(bitFlags)) return false;

            _bitFlags |= bitFlags;

            // clear bit positions
            _carry &= (ushort)~bitFlags;
            // copy specified bits
            _carry |= (ushort)(value & (ushort)bitFlags);

            return true;
        }

        /// <summary>
        /// Clears the carry.
        /// </summary>
        public void Clear()
        {
            _bitFlags = BitFlags.None;
            _carry = 0;
        }

        /// <summary>
        /// Retrieves how many bytes are represented by the bit <paramref name="flags"/>.
        /// </summary>
        /// <param name="flags">A set of bit flags.</param>
        /// <returns>Returns 0 if no flags are set, 1 if only <see cref="BitFlags.LoByte"/> flags are set
        /// and 2 if (also) <see cref="BitFlags.HiByte"/> flags are set.</returns>
        public static int ByteLength(BitFlags flags)
        {
            int length = 0;

            if ((flags & BitFlags.HiByte) > 0)
            {
                length = 2;
            }
            else if ((flags & BitFlags.LoByte) > 0)
            {
                length = 1;
            }

            return length;
        }
    }
}
