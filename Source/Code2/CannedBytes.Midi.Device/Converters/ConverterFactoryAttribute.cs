using System;
using System.ComponentModel.Composition;

namespace CannedBytes.Midi.Device.Converters
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ConverterFactoryAttribute : ExportAttribute, IConverterFactoryInfo
    {
        public ConverterFactoryAttribute(string schemaName)
            : base(typeof(IConverterFactory))
        {
            Check.IfArgumentNullOrEmpty(schemaName, "schemaName");

            SchemaName = schemaName;
        }

        public string SchemaName { get; private set; }

        public static IConverterFactoryInfo FromType<T>()
        {
            return FromType(typeof(T));
        }

        public static IConverterFactoryInfo FromType(Type type)
        {
            Check.IfArgumentNull(type, "type");

            var attrs = type.GetCustomAttributes(typeof(ConverterFactoryAttribute), true);

            if (attrs != null && attrs.Length > 0)
            {
                return (ConverterFactoryAttribute)attrs[0];
            }

            return null;
        }
    }
}
