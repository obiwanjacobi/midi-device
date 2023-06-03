using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CannedBytes.Midi.SpeechController.DomainModel
{
    public class PresetCollection : ObservableCollection<Preset>
    {
        private Dictionary<string, Preset> _map =
            new Dictionary<string, Preset>();

        protected override void ClearItems()
        {
            base.ClearItems();
            _map.Clear();
        }

        protected override void InsertItem(int index, Preset item)
        {
            base.InsertItem(index, item);
            //_map.Add(item.Name, item);
        }

        protected override void RemoveItem(int index)
        {
            //Preset item = this[index];
            base.RemoveItem(index);

            //_map.Remove(item.Name);
        }

        protected override void SetItem(int index, Preset item)
        {
            //Preset preset = this[index];
            base.SetItem(index, item);

            //_map.Remove(preset.Name);
            //_map.Add(item.Name, item);
        }

        //public Preset this[string name]
        //{
        //    get
        //    {
        //        return _map[name];
        //    }
        //}

        public bool Remove(string name)
        {
            //if (_map.ContainsKey(name))
            //{
            //    Preset item = _map[name];
            //    return base.Remove(item);
            //}

            foreach (var preset in this)
            {
                if (string.Compare(preset.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    Remove(preset);
                    return true;
                }
            }

            return false;
        }
    }
}