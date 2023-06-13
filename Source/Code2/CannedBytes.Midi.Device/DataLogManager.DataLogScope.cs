using System;

namespace CannedBytes.Midi.Device;

partial class DataLogManager
{
    private sealed class DataLogScope : IDisposable
    {
        private readonly DataLogManager _manager;

        public DataLogScope(DataLogManager manager)
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
