namespace CannedBytes.Midi.Device
{
    using System;
    using System.IO;
    using System.Text;

    /// <summary>
    /// The MidiBinaryStreamWriterBase class provides a base class for writing into the physical stream.
    /// </summary>
    public class MidiBinaryStreamWriter
    {
        /// <summary>
        /// Constructor for derived classes.
        /// </summary>
        /// <param name="outputStream">Must not be null.</param>
        public MidiBinaryStreamWriter(Stream outputStream)
        {
            Check.IfArgumentNull(outputStream, "outputStream");

            _stream = outputStream;
        }

        private Stream _stream;

        /// <summary>
        /// Gets the <see cref="Stream"/> the writer uses to write data to.
        /// </summary>
        public Stream BaseStream
        {
            get { return _stream; }
        }

        /// <summary>
        /// Writes the specified string <paramref name="data"/> to the underlying <see cref="BaseStream"/>.
        /// </summary>
        /// <param name="data">Must not be null.</param>
        public void Write(string data, int length)
        {
            var bytes = Encoding.ASCII.GetBytes(data);

            if (bytes.Length != length)
            {
                if (bytes.Length > length)
                {
                    throw new ArgumentOutOfRangeException("data", "The resulting byte buffer is too large.");
                }

                throw new ArgumentOutOfRangeException("data", "The resulting byte buffer it too small.");
            }

            //if (bytes.Length < length)
            //{
            //    // enlarge the byte array to full length
            //    byte[] temp = new byte[length];

            //    Array.Copy(bytes, temp, bytes.Length);

            //    bytes = temp;
            //}

            BaseStream.Write(bytes, 0, length);
        }

        /// <summary>
        /// Flushes any buffered data into the actual physical stream.
        /// </summary>
        public virtual void Flush()
        {
            BaseStream.Flush();
        }
    }
}