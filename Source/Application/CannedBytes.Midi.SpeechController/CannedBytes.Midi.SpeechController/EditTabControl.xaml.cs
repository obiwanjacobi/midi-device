using System.Windows;
using System.Windows.Controls;

using CannedBytes.Midi.SpeechController.DomainModel;

namespace CannedBytes.Midi.SpeechController
{
    /// <summary>
    /// Interaction logic for EditTabControl.xaml
    /// </summary>
    public partial class EditTabControl : UserControl
    {
        public EditTabControl()
        {
            InitializeComponent();
        }

        private Preset AddPreset(string name)
        {
            PresetCollection presets = BindingContextData.GetPresets(DataContext);

            if (presets != null)
            {
                Preset newPreset = new Preset();
                newPreset.Name = name;

                presets.Add(newPreset);

                return newPreset;
            }

            return null;
        }

        //---------------------------------------------------------------------
        //
        // Event Handlers
        //
        //---------------------------------------------------------------------

        private void AddPresetButton_Click(object sender, RoutedEventArgs e)
        {
            Preset preset = AddPreset("New Preset");

            PresetList.SelectedItem = preset;

            PresetName.Focus();
            PresetName.SelectAll();
        }

        private void DeletePresetButton_Click(object sender, RoutedEventArgs e)
        {
            Preset currentPreset = PresetList.SelectedItem as Preset;

            if (currentPreset != null)
            {
                PresetCollection presets = BindingContextData.GetPresets(DataContext);

                presets.Remove(currentPreset);
            }
        }

        private void AddPatchButton_Click(object sender, RoutedEventArgs e)
        {
            Preset currentPreset = PresetList.SelectedItem as Preset;

            if (currentPreset != null)
            {
                Patch newPatch = new Patch();
                newPatch.Name = "New Patch";

                currentPreset.Patches.Add(newPatch);

                // set selection in list
                PatchList.SelectedItem = newPatch;
                // select patch name in textbox
                PatchName.Focus();
                PatchName.SelectAll();
            }
        }

        private void DeletePatchButton_Click(object sender, RoutedEventArgs e)
        {
            Preset currentPreset = PresetList.SelectedItem as Preset;
            Patch currentPatch = PatchList.SelectedItem as Patch;

            if (currentPreset != null && currentPatch != null)
            {
                currentPreset.Patches.Remove(currentPatch);
            }
        }

        private void AddCommand_Click(object sender, RoutedEventArgs e)
        {
            Patch currentPatch = PatchList.SelectedItem as Patch;

            if (currentPatch != null)
            {
                MidiCommand newCommand = new MidiCommand();

                currentPatch.MidiCommands.Add(newCommand);

                // set selection in list
                MidiCommandList.SelectedItem = newCommand;
            }
        }

        private void DeleteCommand_Click(object sender, RoutedEventArgs e)
        {
            Patch currentPatch = PatchList.SelectedItem as Patch;
            MidiCommand currentCommand = MidiCommandList.SelectedItem as MidiCommand;

            if (currentPatch != null && currentCommand != null)
            {
                currentPatch.MidiCommands.Remove(currentCommand);
            }
        }

        private void MidiCommandTypeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var sel = this.MidiCommandList.ItemTemplateSelector;
            //this.MidiCommandList.ItemTemplateSelector = sel;
            //this.MidiCommandList.InvalidateProperty(ListBox.ItemTemplateProperty);

            this.MidiCommandList.InvalidateProperty(ListBox.ItemsSourceProperty);
            this.MidiCommandList.InvalidateVisual();
        }
    }
}