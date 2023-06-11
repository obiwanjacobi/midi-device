using System;
using System.Collections.Generic;

namespace CannedBytes.Midi.Device;

partial class DeviceDataContext
{
    public sealed class ConverterState
    {
        private readonly Dictionary<string, object> _stateMap = new();

        public void Clear()
        {
            _stateMap.Clear();
        }

        public IEnumerable<string> Names
        {
            get { return _stateMap.Keys; }
        }

        public bool Contains(string name)
        {
            return _stateMap.ContainsKey(name);
        }

        public T Get<T>(string name)
        {

            if (_stateMap.TryGetValue(name, out object value))
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }

            return default;
        }

        public T Set<T>(string name, T value)
        {
            T oldValue = Get<T>(name);

            _stateMap[name] = value;

            return oldValue;
        }

        public T Remove<T>(string name)
        {
            T oldValue = Get<T>(name);

            _stateMap.Remove(name);

            return oldValue;
        }
    }
}
