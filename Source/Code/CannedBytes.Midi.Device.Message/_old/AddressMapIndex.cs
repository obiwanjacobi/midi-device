using System.Collections.Generic;
using System.Diagnostics;
using CannedBytes.Midi.Device.Converters;

namespace CannedBytes.Midi.Device.Message
{
    /// <summary>
    /// Indexes an address map to quickly find converters by address. NOT USED.
    /// </summary>
    public class AddressMapIndex
    {
        private Dictionary<int, IndexEntry> _index = new Dictionary<int, IndexEntry>();
        private const string AddressAttributeName = "http://schemas.cannedbytes.com/midi-device-schema/message-types/10:address";

        public AddressMapIndex(GroupConverter groupConverter)
        {
            Initialize(groupConverter);
        }

        private void Initialize(GroupConverter groupConverter)
        {
            foreach (var pair in groupConverter.FieldConverterMap)
            {
                Debug.Assert(pair.Converter is GroupConverter, "AddressMap must contain only RecordTypes.");

                var addressAttr = pair.Field.Attributes.Find(AddressAttributeName);

                SevenBitUInt32 baseAddress;

                if (addressAttr != null)
                {
                    baseAddress = new AddressProperty(addressAttr.Value).Address;
                }
                else
                {
                    baseAddress = new SevenBitUInt32(0);
                }

                for (int i = 0; i < pair.Field.MaxOccurs; i++)
                {
                    SevenBitUInt32 address = baseAddress + (i * pair.Converter.ByteLength);

                    var entry = new IndexEntry(i, pair, address);

                    Add(entry);
                }
            }
        }

        private void Add(IndexEntry entry)
        {
            this._index.Add(entry.Address.ToInt32(), entry);
        }

        public IndexEntry Find(int address)
        {
            IndexEntry entry = null;

            if (this._index.ContainsKey(address))
            {
                entry = this._index[address];
            }
            else
            {
                IndexEntry lastEntry = null;
                foreach (var dictPair in this._index)
                {
                    if (dictPair.Key > address)
                    {
                        entry = lastEntry;
                        break;
                    }

                    lastEntry = dictPair.Value;
                }
            }

            return entry;
        }

        public class IndexEntry
        {
            internal IndexEntry(int instanceIndex, FieldConverterPair pair, SevenBitUInt32 address)
            {
                InstanceIndex = instanceIndex;
                Pair = pair;
                Address = address;
            }

            public int InstanceIndex { get; private set; }

            public FieldConverterPair Pair { get; private set; }

            public SevenBitUInt32 Address { get; private set; }
        }
    }
}