using System.Text;
using CannedBytes.Midi.Device.Converters;
using CannedBytes.Midi.Device.Schema;
using CannedBytes.Midi.Core;

namespace CannedBytes.Midi.Device.Message
{
    public class DynamicFieldConverterPair : FieldConverterPair
    {
        public DynamicFieldConverterPair(Field field, IConverter converter, int instanceIndex, SevenBitUInt32 address)
            : base(field, converter)
        {
            InstanceIndex = instanceIndex;
            Address = address;
        }

        public int InstanceIndex { get; protected set; }

        public SevenBitUInt32 Address { get; protected set; }

        public void Stamp(int address, int size)
        {
            ((DynamicField)this.Field).Stamp(address, size);
            //((DynamicGroupConverter)this.GroupConverter).Stamp(address, size);
        }

        public DynamicField DynamicField
        {
            get { return base.Field as DynamicField; }
        }

        public DynamicGroupConverter DynamicGroupConverter
        {
            get { return base.GroupConverter as DynamicGroupConverter; }
        }

        public override string ToString()
        {
            StringBuilder text = new StringBuilder();

            text.Append(Address.ToString("X"));
            text.Append(": ");
            text.Append(Field.ToString());
            text.Append("[");
            text.Append(InstanceIndex);
            text.Append("] - ");

            if (DataConverter != null)
            {
                text.Append(DataConverter.ToString());
            }
            else if (GroupConverter != null)
            {
                text.Append(GroupConverter.ToString());
            }

            return text.ToString();
        }
    }
}