using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Converters;

partial class ConverterManager
{
    [Export]
    public class AttributedConverterFactory : ConverterFactory
    {
        public const string MultipleSchemaNames = "*";

        private readonly IEnumerable<Lazy<DataConverter, IDataConverterInfo>> _dataConverters;
        private readonly IEnumerable<Lazy<StreamConverter, IStreamConverterInfo>> _streamConverters;

        [ImportingConstructor]
        public AttributedConverterFactory(
                    [ImportMany]
                    IEnumerable<Lazy<DataConverter, IDataConverterInfo>> dataConverters,
                    [ImportMany]
                    IEnumerable<Lazy<StreamConverter, IStreamConverterInfo>> streamConverters)
            : base(MultipleSchemaNames)
        {
            Check.IfArgumentNull(dataConverters, "dataConverters");
            Check.IfArgumentNull(streamConverters, "streamConverters");

            _dataConverters = dataConverters;
            _streamConverters = streamConverters;

            SchemaNames = (from dc in _dataConverters
                           select dc.Metadata.SchemaName).Concat(
                           from sc in _streamConverters
                           select sc.Metadata.SchemaName).Distinct();
        }

        public IEnumerable<string> SchemaNames { get; }

        public override DataConverter Create(DataType matchType, DataType constructType)
        {
            Check.IfArgumentNull(matchType, "matchType");
            Check.IfArgumentNull(constructType, "constructType");

            Lazy<DataConverter, IDataConverterInfo> dataConverter = (from dc in _dataConverters
                                 where dc.Metadata.SchemaName == matchType.Name.SchemaName
                                 where dc.Metadata.DataTypeName == matchType.Name.Name
                                 select dc).FirstOrDefault();

            return dataConverter?.Value;
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
