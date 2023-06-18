using System.Collections.Generic;

namespace CannedBytes.Midi.Device;

partial class DeviceDataContext
{
    public sealed class ConverterState
    {
        private readonly Dictionary<string, object> _stateMap = new();
        private readonly DeviceDataContext _context;

        internal ConverterState(DeviceDataContext context)
        {
            _context = context;
        }

        public void Clear()
        {
            _stateMap.Clear();
        }

        public bool Contains(string name)
        {
            var fullName = GetFullName(name);
            return _stateMap.ContainsKey(fullName);
        }

        public T Get<T>(string name)
        {
            var fullName = GetFullName(name);
            return GetInternal<T>(fullName);
        }

        public T Set<T>(string name, T value)
        {
            var fullName = GetFullName(name);
            T oldValue = GetInternal<T>(fullName);

            _stateMap[fullName] = value;

            return oldValue;
        }

        public T Remove<T>(string name)
        {
            var fullName = GetFullName(name);
            T oldValue = GetInternal<T>(fullName);

            _stateMap.Remove(fullName);

            return oldValue;
        }

        public T GetInternal<T>(string fullName)
        {
            if (_stateMap.TryGetValue(fullName, out object value))
            {
                //return (T)Convert.ChangeType(value, typeof(T));
                return (T)value;
            }

            return default;
        }

        private string GetFullName(string name)
        {
            // this does not work when state is written/read
            // at different levels in the Schema(NodeMap).

            //var key = _context.FieldInfo.CurrentNode.Key;
            //var field = _context.FieldInfo.CurrentField.Name.FullName;
            //return $"{field}[{key}]:{name}";
            
            return name;
        }
    }
}
