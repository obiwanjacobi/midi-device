using System.IO;
using System.Xml;
using CannedBytes.Midi.SpeechController.DomainModel;

namespace CannedBytes.Midi.SpeechController.Serialization.Version10
{
    class Deserializer : IDeserializer
    {
        public PresetCollection Deserialize(XmlReader xmlReader)
        {
            if (!Serializer.XmlSerializer.CanDeserialize(xmlReader))
            {
                throw new InvalidDataException("Version 1.0 cannot deserialize.");
            }

            var document = (Document1)Serializer.XmlSerializer.Deserialize(xmlReader);

            // TODO: validate the MidiPortIds

            return MapTo(document.Presets);
        }

        private DomainModel.PresetCollection MapTo(Preset[] presets)
        {
            var targets = new DomainModel.PresetCollection();

            foreach (var preset in presets)
            {
                targets.Add(MapTo(preset));
            }

            return targets;
        }

        private DomainModel.Preset MapTo(Preset preset)
        {
            var target = new DomainModel.Preset();

            target.Name = preset.Name;
            target.SuccessAudioFeedbackType = ConvertTo(preset.SuccessAudioFeedbackType);
            target.FailureAudioFeedbackType = ConvertTo(preset.FailureAudioFeedbackType);

            MapTo(target.Patches, preset.Patches);

            return target;
        }

        private void MapTo(PatchCollection patchCollection, Patch[] patches)
        {
            foreach (var patch in patches)
            {
                patchCollection.Add(MapTo(patch));
            }
        }

        private DomainModel.Patch MapTo(Patch patch)
        {
            var target = new DomainModel.Patch();

            target.Name = patch.Name;
            target.Text = patch.TextPhrase;

            MapTo(target.MidiCommands, patch.MidiCommands);

            return target;
        }

        private void MapTo(MidiCommandCollection midiCommandCollection, MidiCommand[] midiCommands)
        {
            foreach (var midiCmd in midiCommands)
            {
                midiCommandCollection.Add(MapTo(midiCmd));
            }
        }

        private DomainModel.MidiCommand MapTo(MidiCommand midiCmd)
        {
            var target = new DomainModel.MidiCommand();

            target.Channel = midiCmd.Channel;
            target.Type = ConvertTo(midiCmd.CommandType);
            target.Port = midiCmd.Port.Id;

            if (midiCmd.ProgramValueSpecified)
                target.ProgramValue = midiCmd.ProgramValue;

            if (midiCmd.ControllerValueSpecified)
                target.ControllerValue = midiCmd.ControllerValue;

            if (midiCmd.ControllerTypeSpecified)
                target.ControllerType = ConvertTo(midiCmd.ControllerType);

            return target;
        }

        private DomainModel.MidiCommandType ConvertTo(MidiCommandTypes midiCommandTypes)
        {
            int value = (int)midiCommandTypes;
            return (DomainModel.MidiCommandType)value;
        }

        private CannedBytes.Midi.Message.MidiControllerType ConvertTo(MidiControllerTypes midiControllerTypes)
        {
            int value = (int)midiControllerTypes;
            return (CannedBytes.Midi.Message.MidiControllerType)value;
        }

        private DomainModel.AudioFeedbackType ConvertTo(AudioFeedbackTypes audioFeedbackType)
        {
            int value = (int)audioFeedbackType;
            return (DomainModel.AudioFeedbackType)value;
        }
    }
}