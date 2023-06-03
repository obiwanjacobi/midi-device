using System;

namespace CannedBytes.Midi.Device.Schema.Xml
{
    public class MidiDeviceSchemaCompiler
    {
        private readonly MidiDeviceSchemaSet _schemas;

        public MidiDeviceSchemaCompiler(MidiDeviceSchemaSet schemas)
        {
            _schemas = schemas;
        }

        public void Compile(DeviceSchema schema)
        {
            throw new NotImplementedException();
        }
    }
}