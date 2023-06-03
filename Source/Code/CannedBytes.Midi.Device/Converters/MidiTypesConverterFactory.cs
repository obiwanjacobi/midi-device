namespace CannedBytes.Midi.Device.Converters
{
    using System.Linq;
    using CannedBytes.Midi.Device.Schema;

    /// <summary>
    /// The MidiTypeConverterFactory creates converters for the predefined type system
    /// of midi types.
    /// </summary>
    [ConverterFactory(DeviceConstants.MidiTypesSchemaName)]
    public class MidiTypesConverterFactory : ConverterFactory
    {
        /// <summary>
        /// Constructs a new instance that handles the <see cref="Constants.MidiTypesNamespace"/> namespace.
        /// </summary>
        public MidiTypesConverterFactory()
            : base(DeviceConstants.MidiTypesSchemaName)
        { }

        /// <summary>
        /// Creates a field converter instance on the <paramref name="constructType"/>
        /// that supports the specified <paramref name="matchType"/>.
        /// </summary>
        /// <param name="matchType">The data type that is used to match the converter. Must not be null.</param>
        /// <param name="constructType">The data type that is passed to the converter when it is created.</param>
        /// <returns>Returns null if the factory could not find a converter that matched the <paramref name="matchType"/>.</returns>
        public override Converter Create(DataType matchType, DataType constructType)
        {
            Check.IfArgumentNull(matchType, "matchType");
            Check.IfArgumentNull(constructType, "constructType");

            // Not true (anymore) !!
            // Custom types were no converter is created for are run through the default factory.
            //Debug.Assert(matchType.Schema.Name == base.SchemaName);

            Converter converter = null;

            switch (matchType.Name.Name)
            {
                case "midiByte": // bit0-bit7
                    converter = new BitConverter(constructType, BitFlags.LoByte);
                    break;
                case "midiData": // bit0-bit6
                    converter = new BitConverter(constructType, BitFlags.DataByte);
                    break;
                case "midiNibble": // 0-15 (logical)
                    converter = new BitConverter(constructType, BitFlags.LoByteLoNibble);
                    break;
                case "midiLSNibble":	// bit0-bit3
                    converter = new BitConverter(constructType, BitFlags.LoByteLoNibble);
                    break;
                case "midiMSNibble": // bit4-bit7
                    converter = new BitConverter(constructType, BitFlags.LoByteHiNibble);
                    break;
                case "midiBit0":
                    converter = new BitConverter(constructType, BitFlags.Bit0);
                    break;
                case "midiBit1":
                    converter = new BitConverter(constructType, BitFlags.Bit1);
                    break;
                case "midiBit2":
                    converter = new BitConverter(constructType, BitFlags.Bit2);
                    break;
                case "midiBit3":
                    converter = new BitConverter(constructType, BitFlags.Bit3);
                    break;
                case "midiBit4":
                    converter = new BitConverter(constructType, BitFlags.Bit4);
                    break;
                case "midiBit5":
                    converter = new BitConverter(constructType, BitFlags.Bit5);
                    break;
                case "midiBit6":
                    converter = new BitConverter(constructType, BitFlags.Bit6);
                    break;
                case "midiBit7":
                    converter = new BitConverter(constructType, BitFlags.Bit7);
                    break;
                case "midiBit8":
                    converter = new BitConverter(constructType, BitFlags.Bit8);
                    break;
                case "midiBit9":
                    converter = new BitConverter(constructType, BitFlags.Bit9);
                    break;
                case "midiBit10":
                    converter = new BitConverter(constructType, BitFlags.Bit10);
                    break;
                case "midiBit11":
                    converter = new BitConverter(constructType, BitFlags.Bit11);
                    break;
                case "midiBit12":
                    converter = new BitConverter(constructType, BitFlags.Bit12);
                    break;
                case "midiBit13":
                    converter = new BitConverter(constructType, BitFlags.Bit13);
                    break;
                case "midiBit14":
                    converter = new BitConverter(constructType, BitFlags.Bit14);
                    break;
                case "midiBit15":
                    converter = new BitConverter(constructType, BitFlags.Bit15);
                    break;
                case "midiString":
                    converter = new StringConverter(constructType);
                    break;
                case "midiNull":
                    converter = new NullByteConverter(constructType);
                    break;
                case "midiChecksum":
                    converter = new ChecksumConverter(constructType);
                    break;
                case "midiUnsigned":
                    converter = new UnsignedConverter(constructType);
                    break;
                case "midiSigned":
                    converter = new SignedConverter(constructType);
                    break;
                default:
                    if (matchType.IsUnion)
                    {
                        converter = CreateUnion(matchType, constructType);
                    }
                    break;
            }

            return converter;
        }

        private Converter CreateUnion(DataType matchType, DataType constructType)
        {
            var bitTypes = from dataType in matchType.BaseTypes
                           where dataType.Name.SchemaName == this.SchemaName
                           where dataType.Name.Name.StartsWith("midiBit") || dataType.Name.Name.EndsWith("Nibble")
                           select dataType;

            int count = bitTypes.Count();
            if (count != matchType.BaseTypes.Count)
            {
                if (count != 0)
                {
                    throw new MidiDeviceDataException("Incorrect/unsupported base types specified for union. " +
                                                      "Use only the midiBit0-midiBit15 types.");
                }
            }
            else
            {
                BitFlags flags = BitFlags.None;

                foreach (var dataType in bitTypes)
                {
                    switch (dataType.Name.Name)
                    {
                        case "midiNibble":
                        case "midiLSNibble":	// bit0-bit3
                            flags |= BitFlags.LoByteLoNibble;
                            break;
                        case "midiMSNibble": // bit4-bit7
                            flags |= BitFlags.LoByteHiNibble;
                            break;
                        case "midiBit0":
                            flags |= BitFlags.Bit0;
                            break;
                        case "midiBit1":
                            flags |= BitFlags.Bit1;
                            break;
                        case "midiBit2":
                            flags |= BitFlags.Bit2;
                            break;
                        case "midiBit3":
                            flags |= BitFlags.Bit3;
                            break;
                        case "midiBit4":
                            flags |= BitFlags.Bit4;
                            break;
                        case "midiBit5":
                            flags |= BitFlags.Bit5;
                            break;
                        case "midiBit6":
                            flags |= BitFlags.Bit6;
                            break;
                        case "midiBit7":
                            flags |= BitFlags.Bit7;
                            break;
                        case "midiBit8":
                            flags |= BitFlags.Bit8;
                            break;
                        case "midiBit9":
                            flags |= BitFlags.Bit9;
                            break;
                        case "midiBit10":
                            flags |= BitFlags.Bit10;
                            break;
                        case "midiBit11":
                            flags |= BitFlags.Bit11;
                            break;
                        case "midiBit12":
                            flags |= BitFlags.Bit12;
                            break;
                        case "midiBit13":
                            flags |= BitFlags.Bit13;
                            break;
                        case "midiBit14":
                            flags |= BitFlags.Bit14;
                            break;
                        case "midiBit15":
                            flags |= BitFlags.Bit15;
                            break;
                    }
                }

                return new BitConverter(constructType, flags);
            }

            return null;
        }

        /// <summary>
        /// Creates a group converter instance on the specified <paramref name="constructType"/>
        /// that supports the <paramref name="matchType"/>.
        /// </summary>
        /// <param name="matchType">The record type used to match the group converter. Must not be null.</param>
        /// <param name="constructType">The record type passed to the converter when it is created. Must not be null.</param>
        /// <returns>Returns null when the factory could not find a suitable group converter that matched the <paramref name="matchType"/>.</returns>
        public override GroupConverter Create(RecordType matchType, RecordType constructType)
        {
            Check.IfArgumentNull(matchType, "matchType");
            Check.IfArgumentNull(constructType, "constructType");

            // check for a checksum DataType. We need the BufferedGroupConverter for it.
            if (matchType.Fields.Count > 0)
            {
                // TODO: do we need to use FlattenedFields here?
                // is the last field a checksum?
                Field field = matchType.Fields[matchType.Fields.Count - 1];

                if (field.DataType != null &&
                    field.DataType.IsType(DeviceConstants.MidiTypesSchema_Checksum, true))
                {
                    return new BufferedGroupConverter(constructType);
                }
            }

            GroupConverter converter = null;

            switch (matchType.Name.Name)
            {
                case "midiSplitNibbleLE":
                    converter = new SplitNibbleLEGroupConverter(constructType);
                    break;
                case "midiSplitNibbleBE":
                    converter = new SplitNibbleBEGroupConverter(constructType);
                    break;
                case "midiBigEndian":
                    converter = new BigEndianGroupConverter(constructType);
                    break;
                case "midiSevenByte":
                    converter = new SevenByteShift56GroupConverter(constructType);
                    break;

                default:
                    converter = new GroupConverter(constructType);
                    break;
            }

            return converter;
        }
    }
}