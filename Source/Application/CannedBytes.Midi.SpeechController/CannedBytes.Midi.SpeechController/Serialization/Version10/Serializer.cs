using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace CannedBytes.Midi.SpeechController.Serialization.Version10
{
    class Serializer : ISerializer
    {
        internal static XmlSerializer XmlSerializer = new XmlSerializer(typeof(Document1));

        public void Serialize(DomainModel.PresetCollection presets, XmlReader xmlReader)
        {
            var file = MapToFile(presets);
        }

        private File MapToFile(DomainModel.PresetCollection presets)
        {
            var target = new File()
            {
                Properties = new Properties()
                {
                    FileVersion = FileSchemaManager.Version10
                },
                Document = new Document1()
                {
                    Presets = MapTo(presets)
                }
            };

            return target;
        }

        private Preset[] MapTo(DomainModel.PresetCollection presets)
        {
            var targets = new List<Preset>();

            foreach (var preset in presets)
            {
                targets.Add(MapTo(preset));
            }

            return targets.ToArray();
        }

        private Preset MapTo(DomainModel.Preset preset)
        {
            var target = new Preset();

            target.Name = preset.Name;
            target.SuccessAudioFeedbackType = ConvertTo(preset.SuccessAudioFeedbackType);
            target.FailureAudioFeedbackType = ConvertTo(preset.FailureAudioFeedbackType);

            target.Patches = MapTo(preset.Patches);

            return target;
        }

        private Patch[] MapTo(DomainModel.PatchCollection patchCollection)
        {
            var targets = new List<Patch>();

            foreach (var patch in patchCollection)
            {
                targets.Add(MapTo(patch));
            }

            return targets.ToArray();
        }

        private Patch MapTo(DomainModel.Patch patch)
        {
            var target = new Patch();

            target.Name = patch.Name;
            target.TextPhrase = patch.Text;

            target.MidiCommands = MapTo(patch.MidiCommands);

            return target;
        }

        private MidiCommand[] MapTo(DomainModel.MidiCommandCollection midiCommandCollection)
        {
            var targets = new List<MidiCommand>();

            foreach (var midiCmd in midiCommandCollection)
            {
                targets.Add(MapTo(midiCmd));
            }

            return targets.ToArray();
        }

        private MidiCommand MapTo(DomainModel.MidiCommand midiCmd)
        {
            var target = new MidiCommand();

            target.Channel = midiCmd.Channel;
            target.CommandType = ConvertTo(midiCmd.Type);
            target.ControllerType = ConvertTo(midiCmd.ControllerType);
            target.ControllerTypeSpecified = true;
            target.ControllerValue = midiCmd.ControllerValue;
            target.ControllerValueSpecified = true;
            target.ProgramValue = midiCmd.ProgramValue;
            target.ProgramValueSpecified = true;

            target.Port = MapTo(midiCmd.Port);

            return target;
        }

        private MidiPort MapTo(int portId)
        {
            var target = new MidiPort();

            target.Id = portId;
            target.Name = "";

            return target;
        }

        private MidiControllerTypes ConvertTo(CannedBytes.Midi.Message.MidiControllerType midiControllerType)
        {
            int value = (int)midiControllerType;
            return (MidiControllerTypes)value;
        }

        private MidiCommandTypes ConvertTo(CannedBytes.Midi.SpeechController.DomainModel.MidiCommandType midiCommandType)
        {
            int value = (int)midiCommandType;
            return (MidiCommandTypes)value;
        }

        private AudioFeedbackTypes ConvertTo(DomainModel.AudioFeedbackType audioFeedbackType)
        {
            int value = (int)audioFeedbackType;
            return (AudioFeedbackTypes)value;
        }
    }
}