namespace CannedBytes.Midi.Device
{
    // implemented by a StreamConverter will be called if its RecordType is being processed
    public interface INavigationEvents
    {
        void OnBeforeRecord(DeviceDataContext context);

        void OnBeforeField(DeviceDataContext context);

        void OnAfterField(DeviceDataContext context);

        void OnAfterRecord(DeviceDataContext context);
    }
}
