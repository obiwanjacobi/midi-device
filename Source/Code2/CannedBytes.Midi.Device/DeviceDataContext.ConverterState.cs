using System;
using System.Collections.Generic;

namespace CannedBytes.Midi.Device
{
    partial class DeviceDataContext
    {
        public class ConverterState
        {
            private readonly Dictionary<string, object> _stateMap = new Dictionary<string, object>();

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
                object value;

                if (_stateMap.TryGetValue(name, out value))
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }

                return default(T);
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
}
