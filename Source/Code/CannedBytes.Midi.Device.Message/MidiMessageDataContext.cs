using System.Collections.Generic;
using CannedBytes.Midi.Device.Converters;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Message
{
    public class MidiMessageDataContext : MidiDeviceDataContext
    {
        public MidiMessageDataContext(FieldConverterPair rootPair)
            : base(rootPair)
        {
        }

        public MidiMessageDataContext(GroupConverter rootConverter)
            : base(rootConverter)
        {
        }

        public MidiMessageDataContext(RecordType recordType, GroupConverter rootConverter)
            : base(recordType, rootConverter)
        {
        }

        private MidiDeviceBinaryMap binaryMap;

        public MidiDeviceBinaryMap BinaryMap
        {
            get
            {
                if (this.binaryMap == null)
                {
                    this.BinaryMap = new MidiDeviceBinaryMap(this.RootConverter);
                }

                return this.binaryMap;
            }
            set
            {
                Check.IfArgumentNull(value, "BinaryMap");
                this.binaryMap = value;
            }
        }

        public override MidiLogicalContext CreateLogicalContext()
        {
            var fieldInfos = new List<MidiLogicalContext.FieldInfo>();

            foreach (var pairEnum in this.EnumeratorStack)
            {
                var dynamicPair = pairEnum.Current as DynamicFieldConverterPair;

                if (dynamicPair != null)
                {
                    fieldInfos.Add(new MidiLogicalContext.FieldInfo(dynamicPair.Field, dynamicPair.InstanceIndex));
                }
                else
                {
                    fieldInfos.Add(new MidiLogicalContext.FieldInfo(pairEnum.Current.Field, pairEnum.InstanceIndex));
                }
            }

            return new MidiLogicalContext(RecordType, CurrentFieldConverter.Field, fieldInfos);
        }
    }
}