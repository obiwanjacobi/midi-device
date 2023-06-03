namespace CannedBytes.Midi.Device
{
    public class MidiLogicalStreamWriter
    {
        public MidiLogicalStreamWriter(IMidiLogicalWriter writer)
        {
            _writer = writer;
        }

        private IMidiLogicalWriter _writer;

        public IMidiLogicalWriter MidiLogicalWriter
        {
            get { return _writer; }
        }
    }
}