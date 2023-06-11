using System.IO;
using CannedBytes.Midi.Core;
using CannedBytes.Midi.Device.Converters;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Roland;

public class RolandChecksumStreamConverter : ChecksumStreamConverter
{
    public RolandChecksumStreamConverter(RecordType recordType)
        : base(recordType)
    { }

    protected override VarUInt64 CalculateChecksum(Stream stream)
    {
        var total = base.CalculateChecksum(stream);

        int checksum = total.ConvertTo((VarUInt64.VarTypeCode)ByteLength) % 0x80;

        return 0x80 - checksum;
    }
}
