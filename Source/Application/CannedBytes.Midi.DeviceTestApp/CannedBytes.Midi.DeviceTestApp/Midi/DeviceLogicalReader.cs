using CannedBytes.Midi.Device;

namespace CannedBytes.Midi.DeviceTestApp.Midi
{
    class DeviceLogicalReader : IMidiLogicalReader
    {
        public bool ReadBool(MidiLogicalContext context)
        {
            return false;
        }

        public byte ReadByte(MidiLogicalContext context)
        {
            // TODO: remove hardcoded
            if (context.Field.Name.Name == "DeviceId")
            {
                return 0x10;
            }

            return 0;
        }

        public int ReadInt32(MidiLogicalContext context)
        {
            return 0;
        }

        public long ReadInt64(MidiLogicalContext context)
        {
            return 0;
        }

        public string ReadString(MidiLogicalContext context)
        {
            return null;
        }
    }
}