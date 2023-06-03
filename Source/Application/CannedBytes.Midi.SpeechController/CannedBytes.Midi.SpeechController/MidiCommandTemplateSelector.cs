using System;
using System.Windows;
using System.Windows.Controls;
using CannedBytes.Midi.SpeechController.DomainModel;

namespace CannedBytes.Midi.SpeechController
{
    class MidiCommandTemplateSelector : DataTemplateSelector
    {
        private string _controlTempl;
        private string _programTempl;
        //private string _sysexTempl;

        public MidiCommandTemplateSelector()
            : this("ControlTypeTemplate", "ProgramTypeTemplate" /*, "SysExTypeTemplate"*/)
        { }

        public MidiCommandTemplateSelector(string controlTemplate, string programTemplate/*, string sysexTemplate*/)
        {
            _controlTempl = controlTemplate;
            _programTempl = programTemplate;
            //_sysexTempl = sysexTemplate;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            MidiCommand midiCmd = item as MidiCommand;

            if (midiCmd != null)
            {
                string templateName = null;

                switch (midiCmd.Type)
                {
                    case MidiCommandType.ControlChange:
                        templateName = _controlTempl;
                        break;
                    case MidiCommandType.ProgramChange:
                        templateName = _programTempl;
                        break;
                    //case MidiCommandType.SystemExclusive:
                    //    templateName = _sysexTempl;
                    //    break;
                }

                if (!String.IsNullOrEmpty(templateName))
                {
                    FrameworkElement fxElement = container as FrameworkElement;

                    if (fxElement != null)
                    {
                        return fxElement.FindResource(templateName) as DataTemplate;
                    }
                }
            }

            return null;
        }
    }
}