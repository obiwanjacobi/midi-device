using CannedBytes.ComponentFramework;
using CannedBytes.Midi.Message;
using CannedBytes.Midi.SpeechController.DomainModel;

namespace CannedBytes.Midi.SpeechController.Service
{
    internal class MidiMessageFactory : IMidiMessageFactory, IServiceContainerHost
    {
        private Message.MidiMessageFactory factory = new Message.MidiMessageFactory();

        #region IMidiMessageFactory Members

        public MidiMessage CreateMidiMessage(MidiCommand midiCmd)
        {
            MidiMessage midiMsg = null;

            switch (midiCmd.Type)
            {
                case MidiCommandType.ProgramChange:
                    midiMsg = CreateProgramChange(midiCmd);
                    break;
                case MidiCommandType.ControlChange:
                    midiMsg = CreateControlChange(midiCmd);
                    break;
                //case MidiCommandType.SystemExclusive:
                //    midiMsg = CreateSysEx(midiCmd);
                //    break;
            }

            return midiMsg;
        }

        #endregion IMidiMessageFactory Members

        private MidiMessage CreateProgramChange(MidiCommand midiCmd)
        {
            return this.factory.CreateChannelMessage(
                MidiChannelCommand.ProgramChange, (byte)midiCmd.Channel, (byte)midiCmd.ControllerValue, 0);
        }

        private MidiControllerMessage CreateControlChange(MidiCommand midiCmd)
        {
            return this.factory.CreateControllerMessage(
                (byte)midiCmd.Channel, midiCmd.ControllerType, (byte)midiCmd.ControllerValue);
        }

        //private MidiSysExMessage CreateSysEx(MidiCommand midiCmd)
        //{
        //    byte[] data = ParseSysEx(midiCmd.SysExData);

        //    return this.factory.CreateSysExMessage(data);
        //}

        private byte[] ParseSysEx(string sysex)
        {
            IBinaryTextService binaryTxtSvc =
                ServiceContainer.GetService<IBinaryTextService>();

            return binaryTxtSvc.Parse(sysex);
        }

        #region IServiceContainerHost Members

        private IServiceContainer _serviceContainer;

        public IServiceContainer ServiceContainer
        {
            get { return _serviceContainer; }
            set { _serviceContainer = value; }
        }

        #endregion IServiceContainerHost Members
    }
}