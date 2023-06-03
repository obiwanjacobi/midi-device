using System.IO;
using System.Xml;
using CannedBytes.Midi.SpeechController.DomainModel;

namespace CannedBytes.Midi.SpeechController.Serialization
{
    class FileReader
    {
        private FileSchemaManager _fileSchemaMgr = new FileSchemaManager();

        public FileReader(Stream inStream)
        {
            BaseStream = inStream;
        }

        public Stream BaseStream { get; private set; }

        private IDeserializer OpenDeserializer(XmlReader xmlReader)
        {
            xmlReader.MoveToContent();
            xmlReader.ReadStartElement();

            // <File>
            if (!xmlReader.IsStartElement("File"))
            {
                throw new System.InvalidOperationException("No or unknown root element found.");
            }

            // <Properties>
            xmlReader.ReadStartElement();
            if (!xmlReader.IsStartElement("Properties"))
            {
                throw new System.InvalidOperationException("Expected Properties element not found.");
            }

            // <FileVersion>
            xmlReader.ReadStartElement();
            var version = xmlReader.ReadString();
            xmlReader.ReadEndElement();

            // </Properties>
            xmlReader.ReadEndElement();

            // <Dcoument>
            xmlReader.ReadStartElement();

            var deserializer = _fileSchemaMgr.FindDeserializer(version);

            if (deserializer == null)
            {
                throw new InvalidDataException("Unrecognized file version.");
            }

            return deserializer;
        }

        public PresetCollection ReadAll()
        {
            var xmlReader = XmlReader.Create(BaseStream);

            var deserializer = OpenDeserializer(xmlReader);

            return deserializer.Deserialize(xmlReader);
        }
    }
}