using System;
using System.ComponentModel.Composition;

namespace CannedBytes.Midi.Device.Converters
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class)]
    public class StreamConverterAttribute : ExportAttribute, IStreamConverterInfo
    {
        public StreamConverterAttribute(string schemaName, string recordTypeName)
            : base(typeof(StreamConverter))
        {
            Check.IfArgumentNullOrEmpty(schemaName, "schemaName");
            Check.IfArgumentNullOrEmpty(recordTypeName, "recordTypeName");

            SchemaName = schemaName;
            RecordTypeName = recordTypeName;
        }

        public string SchemaName { get; private set;}

        public string RecordTypeName { get; private set; }
    }
}
