using System;
using System.Text;
using CannedBytes.Midi.Device.Converters;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Message
{
    public class DynamicGroupConverter : GroupConverter
    {
        public DynamicGroupConverter(RecordType recordType)
            : base(recordType)
        {
            IsDynamic = true;
            base.FieldConverterMap = new DuplicateFieldConverterMap();
        }

        public override string ToString()
        {
            StringBuilder text = new StringBuilder();
            text.AppendLine("DynamicGroupConverter: " + Name);

            foreach (var pair in FieldConverterMap)
            {
                text.AppendLine(pair.ToString());
            }

            return text.ToString();
        }

        private class DuplicateFieldConverterMap : FieldConverterMap
        {
            protected override string GetKeyForItem(FieldConverterPair item)
            {
                // put a limit to it
                for (int index = 1; index < 65535; index++)
                {
                    string key = String.Format("{0}_[{1}]", base.GetKeyForItem(item), index);

                    if (!base.Contains(key))
                    {
                        return key;
                    }
                }

                return null;
            }
        }
    }
}