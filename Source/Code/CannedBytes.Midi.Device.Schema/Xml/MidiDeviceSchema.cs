using System.Collections.Generic;
using CannedBytes.Midi.Device.Schema.Xml.Model1;

namespace CannedBytes.Midi.Device.Schema.Xml
{
    public class MidiDeviceSchema : DeviceSchema
    {
        public List<import> Imports { get; set; }

        public void SetVersion(string value)
        {
            Version = value;
        }
    }
}