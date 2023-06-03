namespace CannedBytes.Midi.Device.Converters
{
    public interface IConverterProcess
    {
        T GetValue<T>();

        void SetValue<T>(T value);

        void AddDataLogRecord();

        bool ReadFromExtension(IConverterExtension extension);

        bool ReadFromContext();

        void ProcessToLogical();

        void ToLogical(IMidiLogicalWriter writer);

        bool WriteToExtension(IConverterExtension extension);

        bool WriteToContext();

        void ProcessToPhysical();

        void ToPhysical(IMidiLogicalReader reader);
    }
}