﻿using CannedBytes.Midi.Device.Schema.Xml.Model1;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace CannedBytes.Midi.Device.Schema.Xml
{
    public static class MidiDeviceSchemaReader
    {
        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(deviceSchema));
        private static readonly XmlReaderSettings _settings = new XmlReaderSettings();

        static MidiDeviceSchemaReader()
        {
            _settings.IgnoreComments = true;
            _settings.IgnoreProcessingInstructions = true;
            _settings.IgnoreWhitespace = true;
            _settings.XmlResolver = new XmlResourceResolver();
        }

        public static deviceSchema Read(Stream stream)
        {
            var reader = XmlReader.Create(stream);

            if (_serializer.CanDeserialize(reader))
            {
                return (deviceSchema)_serializer.Deserialize(reader);
            }

            return null;
        }
    }
}
