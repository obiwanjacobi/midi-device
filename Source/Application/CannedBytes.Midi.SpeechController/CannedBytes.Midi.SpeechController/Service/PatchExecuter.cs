using System;
using CannedBytes.ComponentFramework;
using CannedBytes.Midi.Message;
using CannedBytes.Midi.SpeechController.DomainModel;

namespace CannedBytes.Midi.SpeechController.Service
{
    internal class PatchExecuter : IPatchExecuter, IServiceContainerHost
    {
        //Dictionary<int, IMidiOutPort> _midiPorts = new Dictionary<int, IMidiOutPort>();

        #region IPatchExecuter Members

        public void Prepare(Preset preset)
        {
            //_midiPorts.Clear();

            IMidiOutPortService midiSvc = ServiceContainer.GetService<IMidiOutPortService>();

            // pre-open all ports

            foreach (Patch patch in preset.Patches)
            {
                foreach (MidiCommand cmd in patch.MidiCommands)
                {
                    midiSvc.Open(cmd.Port);

                    //if (!_midiPorts.ContainsKey(cmd.Port))
                    //{
                    //    _midiPorts.Add(cmd.Port, midiSvc.Open(cmd.Port));
                    //}
                }
            }
        }

        public void Execute(Patch patch)
        {
            IMidiMessageFactory factory = ServiceContainer.GetService<IMidiMessageFactory>();
            IMidiOutPortService midiSvc = ServiceContainer.GetService<IMidiOutPortService>();

            int count = patch.MidiCommands.Count;

            // send all midi commands
            for (int n = 0; n < count; n++)
            {
                MidiCommand cmd = patch.MidiCommands[n];

                // create a midi message
                MidiMessage midiMsg = factory.CreateMidiMessage(cmd);

                //IMidiOutPort port = _midiPorts[cmd.Port];
                IMidiOutPort port = midiSvc.Open(cmd.Port);
                port.Send(midiMsg);

                OnNotifyProgress(count, n + 1);
            }
        }

        public event EventHandler<NotifyProgressEventArg> NotifyProgress;

        protected void OnNotifyProgress(int total, int current)
        {
            EventHandler<NotifyProgressEventArg> handler = NotifyProgress;

            if (handler != null)
            {
                handler(this, new NotifyProgressEventArg(total, current));
            }
        }

        #endregion IPatchExecuter Members

        #region IServiceContainerHost Members

        IServiceContainer _serviceContainer;

        public IServiceContainer ServiceContainer
        {
            get { return _serviceContainer; }
            set { _serviceContainer = value; }
        }

        #endregion IServiceContainerHost Members
    }
}