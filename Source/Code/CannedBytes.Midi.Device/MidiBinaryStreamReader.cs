namespace CannedBytes.Midi.Device
{
    using System.IO;
    using System.Text;

    /// <summary>
    /// The MidiBinaryStreamReader class provides operations for reading physical binary data from a <see cref="Stream"/>.
    /// </summary>
    public class MidiBinaryStreamReader
    {
        /// <summary>
        /// Constructor for derived classes.
        /// </summary>
        /// <param name="inputStream">Must not be null.</param>
        public MidiBinaryStreamReader(Stream inputStream)
        {
            Check.IfArgumentNull(inputStream, "inputStream");

            _stream = inputStream;
        }

        private Stream _stream;

        /// <summary>
        /// Gets the stream the reader uses to read from.
        /// </summary>
        public Stream BaseStream
        {
            get { return _stream; }
        }

        /// <summary>
        /// Reads an ASCII string from the underlying <see cref="BaseStream"/> with the specified <paramref name="length"/>.
        /// </summary>
        /// <param name="length">The length of the string in bytes not including any terminating characters (null).</param>
        /// <returns>Returns the ASCII string.</returns>
        public string ReadString(int length)
        {
            byte[] buffer = new byte[length];

            if (BaseStream.Read(buffer, 0, length) != length)
            {
                throw new EndOfStreamException();
            }

            return Encoding.ASCII.GetString(buffer);
        }
    }
}