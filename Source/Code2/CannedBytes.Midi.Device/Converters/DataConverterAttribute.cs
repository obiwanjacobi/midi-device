using System;
using System.ComponentModel.Composition;

namespace CannedBytes.Midi.Device.Converters
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class)]
    public class DataConverterAttribute : ExportAttribute, IDataConverterInfo
    {
        public DataConverterAttribute(string schemaName, string dataTypeName)
            : base(typeof(DataConverter))
        {
            Check.IfArgumentNullOrEmpty(schemaName, "schemaName");
            Check.IfArgumentNullOrEmpty(dataTypeName, "dataTypeName");

            SchemaName = schemaName;
            DataTypeName = dataTypeName;
        }

        public string SchemaName { get; private set; }
        public string DataTypeName { get; private set; }
    }
}
