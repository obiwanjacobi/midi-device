using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using TestApp.Services;

namespace TestApp.DeviceView
{
    internal partial class MidiViewModel : ViewModel
    {
        // designer support
        public MidiViewModel()
        {
        }

        private readonly MidiService _midiService;

        public MidiViewModel(DeviceViewModel deviceModel)
            : base(deviceModel)
        {
            _midiService = deviceModel.Services.GetRequiredService<MidiService>();
        }

        public IEnumerable<string> MidiInPorts => _midiService.GetMidiInPorts();
        public IEnumerable<string> MidiOutPorts => _midiService.GetMidiOutPorts();

        [ObservableProperty]
        private string _selectedMidiInPort;
        [ObservableProperty]
        private string _selectedMidiOutPort;
    }
}
