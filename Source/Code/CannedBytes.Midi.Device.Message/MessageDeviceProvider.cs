using System;
using System.Collections.Generic;
using System.Linq;
using CannedBytes.Midi.Device.Converters;

namespace CannedBytes.Midi.Device.Message
{
    [DeviceProvider(typeof(MessageDeviceProvider), "", "", 0, 0)]
    public class MessageDeviceProvider : MidiDeviceProvider
    {
        public MessageDeviceProvider()
        {
            IsMessageProvider = true;
        }

        private FieldConverterPair addressMap;

        public FieldConverterPair AddressMap
        {
            get
            {
                if (this.addressMap == null)
                {
                    var maps = FindAllAddressMaps();

                    this.addressMap = maps.FirstOrDefault();

                    if (maps.Count() > 1)
                    {
                        throw new InvalidOperationException("Multiple address maps where found in the schema.");
                    }
                }

                return this.addressMap;
            }
        }

        private Dictionary<string, MidiDeviceBinaryMap> binaryRootMaps;

        public MidiDeviceBinaryMap FindBinaryMap(FieldConverterPair rootPair)
        {
            if (!this.RootTypes.Contains(rootPair))
            {
                throw new ArgumentException("Specified Field-Converter pair was not found in the RootTypes collection.", "rootPair");
            }
            if (rootPair.GroupConverter == null)
            {
                throw new ArgumentException("Specified Field-Converter pair does not represent a Record.", "rootPair");
            }

            if (this.binaryRootMaps == null)
            {
                this.binaryRootMaps = new Dictionary<string, MidiDeviceBinaryMap>();

                foreach (var pair in RootTypes)
                {
                    var map = new MidiDeviceBinaryMap(pair.GroupConverter);
                    this.binaryRootMaps.Add(pair.Field.Name.FullName, map);
                }
            }

            if (this.binaryRootMaps.ContainsKey(rootPair.Field.Name.FullName))
            {
                return this.binaryRootMaps[rootPair.Field.Name.FullName];
            }

            return null;
        }

        protected IEnumerable<FieldConverterPair> FindAllAddressMaps()
        {
            return (from pair in RootTypes
                    let am = FindAddressMap(pair)
                    select am).Distinct();
        }

        public FieldConverterPair FindAddressMap(FieldConverterPair rootPair)
        {
            var groupConverter = rootPair.Converter as GroupConverter;

            if (groupConverter != null)
            {
                var addressMapType = this.SchemaProvider.FindRecordType(Constants.MessageNamespace, Constants.AddressMapTypeName);

                return groupConverter.FieldConverterMap.FindByType(addressMapType, true);
            }

            return null;
        }
    }
}