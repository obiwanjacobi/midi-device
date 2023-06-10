using System;

namespace CannedBytes.Midi.Device;

partial class DataRecordManager
{
    private sealed class DataRecordScope : IDisposable
    {
        private readonly DataRecordManager _manager;

        public DataRecordScope(DataRecordManager manager)
        {
            _manager = manager;
        }

        public void Dispose()
        {
            if (_manager.CurrentEntry?.IsEmpty == false)
            {
                _manager.SaveCurrentEntry();
            }
            else
            {
                _manager.CancelCurrentEntry();
            }
        }
    }
}
