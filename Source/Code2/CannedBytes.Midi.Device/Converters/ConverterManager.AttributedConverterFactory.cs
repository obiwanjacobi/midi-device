using System;
using System.Collections.Generic;
using System.Linq;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Converters;

partial class ConverterManager
{
    //[Export]
    public class AttributedConverterFactory : ConverterFactory
    {
        public const string MultipleSchemaNames = "*";

        private readonly IEnumerable<DataConverter> _dataConverters;
        private readonly IEnumerable<StreamConverter> _streamConverters;

        //[ImportingConstructor]
        public AttributedConverterFactory(
                    //[ImportMany]
                    IEnumerable<DataConverter> dataConverters,
                    //[ImportMany]
                    IEnumerable<StreamConverter> streamConverters)
            : base(MultipleSchemaNames)
        {
            Check.IfArgumentNull(dataConverters, "dataConverters");
            Check.IfArgumentNull(streamConverters, "streamConverters");

            _dataConverters = dataConverters;
            _streamConverters = streamConverters;

            SchemaNames = (from dc in _dataConverters
                           select dc.Schema.Name.FullName).Concat(
                           from sc in _streamConverters
                           select sc.Schema.Name.FullName).Distinct();
        }

        public IEnumerable<string> SchemaNames { get; }

        public override DataConverter Create(DataType matchType, DataType constructType)
        {
            Check.IfArgumentNull(matchType, "matchType");
            Check.IfArgumentNull(constructType, "constructType");

            DataConverter dataConverter = (from dc in _dataConverters
                where dc.Schema.Name.FullName == matchType.Name.SchemaName
                where dc.DataTypeName == matchType.Name.Name
                select dc).FirstOrDefault();

            return dataConverter;
        }

        public override StreamConverter Create(RecordType matchType, RecordType constructType)
        {
            Check.IfArgumentNull(matchType, "matchType");
            Check.IfArgumentNull(constructType, "constructType");

            Lazy<StreamConverter, IStreamConverterInfo> streamConverter = (from sc in _streamConverters
                                   where sc.Metadata.SchemaName == matchType.Name.SchemaName
                                   where sc.Metadata.RecordTypeName == matchType.Name.Name
                                   select sc).FirstOrDefault();

            return streamConverter?.Value;
        }
    }
}
