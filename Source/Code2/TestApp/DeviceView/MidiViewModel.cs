using System.Collections.Generic;
using TestApp.Commands;

namespace TestApp.DeviceView
{
    internal partial class MidiViewModel : ViewModel
    {
        public IEnumerable<string> MidiInPorts { get; }
        public IEnumerable<string> MidiOutPorts { get; }

        public Command StartStopCommand { get; }
    }
}
