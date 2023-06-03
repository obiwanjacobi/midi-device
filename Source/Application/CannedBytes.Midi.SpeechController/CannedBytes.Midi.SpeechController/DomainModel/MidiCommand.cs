using CannedBytes.Midi.Message;

namespace CannedBytes.Midi.SpeechController.DomainModel
{
    public class MidiCommand : ObservableObject
    {
        public void CopyTo(MidiCommand target)
        {
            target.Port = Port;
            target.Channel = Channel;
            target.Type = Type;
            target.ControllerType = ControllerType;
            target.ControllerValue = ControllerValue;
            target.ProgramValue = ProgramValue;
            //target.SysExData = SysExData;
        }

        private int _port;

        public int Port
        {
            get { return _port; }
            set { SetPropertyValue(ref _port, value, "Port"); }
        }

        private int _channel;

        public int Channel
        {
            get { return _channel; }
            set { SetPropertyValue(ref _channel, value, "Channel"); }
        }

        private MidiCommandType _type;

        public MidiCommandType Type
        {
            get { return _type; }
            set { SetPropertyValue(ref _type, value, "Type"); }
        }

        private MidiControllerType _controllerType;

        public MidiControllerType ControllerType
        {
            get { return _controllerType; }
            set { SetPropertyValue(ref _controllerType, value, "ControllerType"); }
        }

        private int _programValue;

        public int ProgramValue
        {
            get { return _programValue; }
            set { SetPropertyValue(ref _programValue, value, "ProgramValue"); }
        }

        private int _controllerValue;

        public int ControllerValue
        {
            get { return _controllerValue; }
            set { SetPropertyValue(ref _controllerValue, value, "ControllerValue"); }
        }
    }
}