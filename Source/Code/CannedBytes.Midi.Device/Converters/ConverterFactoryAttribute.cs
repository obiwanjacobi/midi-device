using System;
using System.ComponentModel.Composition;

namespace CannedBytes.Midi.Device.Converters
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ConverterFactoryAttribute : ExportAttribute, IConverterFactoryInfo
    {
        public ConverterFactoryAttribute(string schemaName)
            : base(typeof(ConverterFactory))
        {
            SchemaName = schemaName;
        }

        public string SchemaName { get; private set; }
    }

    public interface IConverterFactoryInfo
    {
        string SchemaName { get; }
    }
}