using CannedBytes.ComponentFramework;
using CannedBytes.Midi.SpeechController.DomainModel;
using CannedBytes.Midi.SpeechController.Service;

namespace CannedBytes.Midi.SpeechController
{
    internal class BindingContextData : IServiceContainerHost
    {
        public MidiOutPortCaps[] MidiOutPortCapabilities
        {
            get
            {
                IMidiOutPortService midiSvc = ServiceContainer.GetService<IMidiOutPortService>();

                if (midiSvc != null)
                {
                    return midiSvc.Capabilities;
                }

                // return an empty array
                return new MidiOutPortCaps[] { };
            }
        }

        private PresetCollection _presets;

        public PresetCollection Presets
        {
            get { return _presets; }
            set { _presets = value; }
        }

        public static PresetCollection GetPresets(object dataCtx)
        {
            BindingContextData ctxData = dataCtx as BindingContextData;

            if (ctxData != null)
            {
                return ctxData.Presets;
            }

            return null;
        }

        #region IServiceContainerHost Members

        private IServiceContainer _svcContainer;

        public IServiceContainer ServiceContainer
        {
            get { return _svcContainer; }
            set { _svcContainer = value; }
        }

        #endregion IServiceContainerHost Members

        public static IServiceContainer GetServiceContainer(object dataCtx)
        {
            BindingContextData ctxData = dataCtx as BindingContextData;

            if (ctxData != null)
            {
                return ctxData.ServiceContainer;
            }

            return null;
        }
    }
}