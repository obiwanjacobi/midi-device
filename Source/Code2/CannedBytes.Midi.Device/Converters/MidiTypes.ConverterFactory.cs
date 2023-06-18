using CannedBytes.Midi.Core;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Converters;

/// <summary>
/// The MidiTypesConverterFactory creates converters for the predefined type system
/// of midi types.
/// </summary>
public sealed class MidiTypesConverterFactory : ConverterFactory
{
    public MidiTypesConverterFactory()
        : base(MidiTypes.MidiTypesSchemaName)
    { }

    /// <summary>
    /// Creates a field converter instance on the <paramref name="constructType"/>
    /// that supports the specified <paramref name="matchType"/>.
    /// </summary>
    /// <param name="matchType">The data type that is used to match the converter. Must not be null.</param>
    /// <param name="constructType">The data type that is passed to the converter when it is created.</param>
    /// <returns>Returns null if the factory could not find a converter that matched the <paramref name="matchType"/>.</returns>
    public override DataConverter Create(DataType matchType, DataType constructType)
    {
        Assert.IfArgumentNull(matchType, nameof(matchType));
        Assert.IfArgumentNull(constructType, nameof(constructType));

        System.Diagnostics.Debug.Assert(matchType.Schema.SchemaName == SchemaName);

        DataConverter converter = null;

        switch (matchType.Name.Name)
        {
            case "midiByte": // bit0-bit7
                converter = new BitConverter(constructType, new ValueRange(0, 7));
                break;
            case "midiData": // bit0-bit6
                converter = new BitConverter(constructType, new ValueRange(0, 6));
                break;
            case "midiBit0":
                converter = new BitConverter(constructType, new ValueRange(0));
                break;
            case "midiBit1":
                converter = new BitConverter(constructType, new ValueRange(1));
                break;
            case "midiBit2":
                converter = new BitConverter(constructType, new ValueRange(2));
                break;
            case "midiBit3":
                converter = new BitConverter(constructType, new ValueRange(3));
                break;
            case "midiBit4":
                converter = new BitConverter(constructType, new ValueRange(4));
                break;
            case "midiBit5":
                converter = new BitConverter(constructType, new ValueRange(5));
                break;
            case "midiBit6":
                converter = new BitConverter(constructType, new ValueRange(6));
                break;
            case "midiBit7":
                converter = new BitConverter(constructType, new ValueRange(7));
                break;
            case "midiBit8":
                converter = new BitConverter(constructType, new ValueRange(8));
                break;
            case "midiBit9":
                converter = new BitConverter(constructType, new ValueRange(9));
                break;
            case "midiBit10":
                converter = new BitConverter(constructType, new ValueRange(10));
                break;
            case "midiBit11":
                converter = new BitConverter(constructType, new ValueRange(11));
                break;
            case "midiBit12":
                converter = new BitConverter(constructType, new ValueRange(12));
                break;
            case "midiBit13":
                converter = new BitConverter(constructType, new ValueRange(13));
                break;
            case "midiBit14":
                converter = new BitConverter(constructType, new ValueRange(14));
                break;
            case "midiBit15":
                converter = new BitConverter(constructType, new ValueRange(15));
                break;
            case "midiBitRange":
                converter = new BitConverter(constructType);
                break;
            case "midiString":
                converter = new StringConverter(constructType);
                break;
            //case "midiNull":
            //    converter = new NullByteConverter(constructType);
            //    break;
            //case "midiChecksum":
            //    converter = new ChecksumConverter(constructType);
            //    break;
            //case "midiUnsigned":
            //    converter = new UnsignedConverter(constructType);
            //    break;
            //case "midiSigned":
            //    converter = new SignedConverter(constructType);
            //    break;
            default:
                if (matchType.IsUnion)
                {
                    throw new DeviceSchemaException("Unions are not supported.");
                }

                //if (converter == null)
                //{
                //    // it is our schema, but dataType is not recognized.
                //    if (SchemaName == matchType.Schema.SchemaName)
                //    {
                //        throw new DeviceSchemaException(
                //            string.Format("The MidiTypesConverterFactory could not create a converter for '{0}' while constructing '{1}'. It does not own that dataType.",
                //                matchType.Name.FullName, constructType.Name.FullName));
                //    }
                //}
                break;
        }

        return converter;
    }

    /// <summary>
    /// Creates a stream converter instance on the specified <paramref name="constructType"/>
    /// that supports the <paramref name="matchType"/>.
    /// </summary>
    /// <param name="matchType">The record type used to match the stream converter. Must not be null.</param>
    /// <param name="constructType">The record type passed to the converter when it is created. Must not be null.</param>
    /// <returns>Returns null when the factory could not find a suitable stream converter that matched the <paramref name="matchType"/>.</returns>
    public override StreamConverter Create(RecordType matchType, RecordType constructType)
    {
        Assert.IfArgumentNull(matchType, nameof(matchType));
        Assert.IfArgumentNull(constructType, nameof(constructType));

        StreamConverter converter;
        switch (matchType.Name.Name)
        {
            //case "midiSplitNibbleLE":
            //    converter = new SplitNibbleLEStreamConverter(constructType);
            //    break;
            //case "midiSplitNibbleBE":
            //    converter = new SplitNibbleBEStreamConverter(constructType);
            //    break;
            case "midiBigEndian":
                converter = new EndianStreamConverter(constructType);
                break;
            //case "midiSevenByte":
            //    converter = new SevenByteShift56StreamConverter(constructType);
            //    break;
            case "midiSysEx":
                converter = new SysExStreamConverter(constructType);
                break;
            case "midiAddressMap":
                converter = new AddressMapConverter(constructType);
                break;
            case "midiChecksum":
                converter = new ChecksumStreamConverter(constructType);
                break;
            default:
                converter = new StreamConverter(constructType);
                break;
        }

        return converter;
    }
}