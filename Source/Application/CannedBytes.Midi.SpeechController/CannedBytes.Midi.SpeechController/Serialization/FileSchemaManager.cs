using System.Collections.Generic;

namespace CannedBytes.Midi.SpeechController.Serialization
{
    internal class FileSchemaManager
    {
        private Dictionary<string, ISerializer> _serializerMap = new Dictionary<string, ISerializer>();
        private Dictionary<string, IDeserializer> _deserializerMap = new Dictionary<string, IDeserializer>();

        public const string Version10 = "1.0.0.0";

        public FileSchemaManager()
        {
            // serializers
            _serializerMap.Add(Version10, new Version10.Serializer());

            // deserializers
            _deserializerMap.Add(Version10, new Version10.Deserializer());
        }

        public ISerializer FindSerializer(string version)
        {
            if (_serializerMap.ContainsKey(version))
            {
                return _serializerMap[version];
            }

            return null;
        }

        public IDeserializer FindDeserializer(string version)
        {
            if (_deserializerMap.ContainsKey(version))
            {
                return _deserializerMap[version];
            }

            return null;
        }
    }
}