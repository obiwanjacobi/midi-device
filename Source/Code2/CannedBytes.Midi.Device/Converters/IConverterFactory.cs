using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Converters
{
    public interface IConverterFactory
    {
        string SchemaName { get; }

        DataConverter Create(DataType matchType, DataType constructType);

        StreamConverter Create(RecordType matchType, RecordType constructType);
    }
}
