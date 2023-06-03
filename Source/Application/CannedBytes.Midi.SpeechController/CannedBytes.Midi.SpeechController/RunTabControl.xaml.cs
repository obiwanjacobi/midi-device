using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using CannedBytes.ComponentFramework;
using CannedBytes.Midi.SpeechController.DomainModel;
using CannedBytes.Midi.SpeechController.Service;

namespace CannedBytes.Midi.SpeechController
{
    /// <summary>
    /// Interaction logic for RunTabControl.xaml
    /// </summary>
    public partial class RunTabControl : UserControl
    {
        private string _lastTextPhrase;

        public RunTabControl()
        {
            InitializeComponent();
        }

        private void InitializeSpeechEngine()
        {
            IServiceContainer svcContainer = BindingContextData.GetServiceContainer(DataContext);

            if (svcContainer != null)
            {
                ISpeechRecognizerService speechSvc = svcContainer.GetService<ISpeechRecognizerService>();

                if (speechSvc != null)
                {
                    speechSvc.SpeechRecognized += new EventHandler<SpeechEventArgs>(SpeechService_SpeechRecognized);
                    speechSvc.SpeechUnrecognized += new EventHandler<SpeechEventArgs>(SpeechService_SpeechUnrecognized);
                    speechSvc.StateChanged += new EventHandler<SpeechRecognizerStatesEventsArgs>(SpeechService_StateChanged);

                    // select the right panel in the UI for either speech or manual
                    SpeechPanel.Visibility = speechSvc.IsInstalled ? Visibility.Visible : Visibility.Collapsed;
                    //ManualPanel.Visibility = speechSvc.IsInstalled ? Visibility.Collapsed : Visibility.Visible;
                }

                IPatchExecuter patchExecuter = svcContainer.GetService<IPatchExecuter>();

                if (patchExecuter != null)
                {
                    patchExecuter.NotifyProgress += new EventHandler<NotifyProgressEventArg>(PatchExecuter_NotifyProgress);
                }
            }
        }

        private void UninitializeSpeechEngine()
        {
            IServiceContainer svcContainer = BindingContextData.GetServiceContainer(DataContext);

            if (svcContainer != null)
            {
                ISpeechRecognizerService speechSvc = svcContainer.GetService<ISpeechRecognizerService>();

                if (speechSvc != null)
                {
                    speechSvc.SpeechRecognized -= new EventHandler<SpeechEventArgs>(SpeechService_SpeechRecognized);
                    speechSvc.SpeechUnrecognized -= new EventHandler<SpeechEventArgs>(SpeechService_SpeechUnrecognized);
                    speechSvc.StateChanged -= new EventHandler<SpeechRecognizerStatesEventsArgs>(SpeechService_StateChanged);
                }

                IPatchExecuter patchExecuter = svcContainer.GetService<IPatchExecuter>();

                if (patchExecuter != null)
                {
                    patchExecuter.NotifyProgress -= new EventHandler<NotifyProgressEventArg>(PatchExecuter_NotifyProgress);
                }
            }
        }

        private void DisplayPatch(Patch patch)
        {
            RunningPatch.Content = patch.Name;

            PatchCommands.ItemsSource = patch.MidiCommands;

            HistoryList.Items.Insert(0, patch.Name);
        }

        private void ExecutePatch(Patch patch)
        {
            IServiceContainer svcContainer = BindingContextData.GetServiceContainer(DataContext);

            if (svcContainer != null)
            {
                IPatchExecuter patchExecuter = svcContainer.GetService<IPatchExecuter>();

                if (patchExecuter != null)
                {
                    patchExecuter.Execute(patch);
                }
            }
        }

        private void RunPatch(Patch patch)
        {
            ResetProgress();
            DisplayPatch(patch);
            ConfirmSuccess(patch);

            ExecutePatch(patch);
        }

        private void ConfirmSuccess(Patch patch)
        {
            Preset preset = (Preset)PresetList.SelectedItem;
            IServiceContainer svcContainer = BindingContextData.GetServiceContainer(DataContext);

            if (preset != null && svcContainer != null)
            {
                switch (preset.SuccessAudioFeedbackType)
                {
                    case AudioFeedbackType.Beep:
                        IAudioService audioSvc = svcContainer.GetService<IAudioService>();

                        if (audioSvc != null)
                        {
                            //audioSvc.Beep();
                            audioSvc.PlayStandardSound(WindowsSounds.Beep);
                        }
                        break;
                    case AudioFeedbackType.Speak:
                        ITextToSpeechService speakSvc = svcContainer.GetService<ITextToSpeechService>();

                        if (speakSvc != null)
                        {
                            speakSvc.Speak(patch.Name);
                        }
                        break;
                }
            }
        }

        private void ConfirmFailure()
        {
            Preset preset = (Preset)PresetList.SelectedItem;
            IServiceContainer svcContainer = BindingContextData.GetServiceContainer(DataContext);

            if (preset != null && svcContainer != null)
            {
                switch (preset.FailureAudioFeedbackType)
                {
                    case AudioFeedbackType.Beep:
                        IAudioService audioSvc = svcContainer.GetService<IAudioService>();

                        if (audioSvc != null)
                        {
                            //audioSvc.Beep();
                            audioSvc.PlayStandardSound(WindowsSounds.Beep);
                        }
                        break;
                    case AudioFeedbackType.Speak:
                        ITextToSpeechService speakSvc = svcContainer.GetService<ITextToSpeechService>();

                        if (speakSvc != null)
                        {
                            speakSvc.Speak("Not Recognized");
                        }
                        break;
                }
            }
        }

        private void HandleSpeechRecognized()
        {
            TextPhrase.Content = _lastTextPhrase;

            Preset preset = (Preset)PresetList.SelectedItem;

            if (preset != null)
            {
                Patch patch = preset.CreatePatchView()[_lastTextPhrase];

                if (patch != null)
                {
                    RunPatch(patch);
                }
            }
        }

        private void ResetProgress()
        {
            CommandProgress.Minimum = 0;
            CommandProgress.Maximum = 100;
            CommandProgress.Value = 0;
        }

        private void SpeechService_StateChanged(object sender, SpeechRecognizerStatesEventsArgs e)
        {
            string text = null;

            switch (e.State)
            {
                case SpeechRecognizerStates.NoSignal:
                    text = "No Signal";
                    break;
                case SpeechRecognizerStates.NotRecognized:
                    text = "Not Recognized";
                    break;
                case SpeechRecognizerStates.Recognized:
                    text = "Recognized";
                    break;
                case SpeechRecognizerStates.Silence:
                    text = "Silence";
                    break;
                case SpeechRecognizerStates.Speech:
                    text = "Speech Detected";
                    break;
                case SpeechRecognizerStates.Stopped:
                    text = "Engine Stopped";
                    break;
                case SpeechRecognizerStates.TooFast:
                    text = "Too Fast";
                    break;
                case SpeechRecognizerStates.TooLoud:
                    text = "Too Loud";
                    break;
                case SpeechRecognizerStates.TooNoisy:
                    text = "Too Noisy";
                    break;
                case SpeechRecognizerStates.TooSlow:
                    text = "Too Slow";
                    break;
                case SpeechRecognizerStates.TooSoft:
                    text = "Too Soft";
                    break;
            }

            Dispatcher.Invoke(new SetStateTextHandler(SetStateText), text);
        }

        private delegate void SetStateTextHandler(string text);

        private void SetStateText(string text)
        {
            State.Content = text;
        }

        private delegate void SpeechRecognizedHandler();

        private void SpeechService_SpeechRecognized(object sender, SpeechEventArgs e)
        {
            _lastTextPhrase = e.Text;

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                new SpeechRecognizedHandler(HandleSpeechRecognized));
        }

        private delegate void SpeechUnrecognizedHandler();

        private void SpeechService_SpeechUnrecognized(object sender, SpeechEventArgs e)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                new SpeechUnrecognizedHandler(ConfirmFailure));
        }

        private void PatchExecuter_NotifyProgress(object sender, NotifyProgressEventArg e)
        {
            CommandProgress.Minimum = 0;
            CommandProgress.Maximum = e.Total;
            CommandProgress.Value = e.Current;

            if (e.Current == e.Total)
            {
                ResetProgress();
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeSpeechEngine();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            UninitializeSpeechEngine();
        }

        private void StartStopButton_Click(object sender, RoutedEventArgs e)
        {
            CannedBytes.ComponentFramework.IServiceProvider svcProvider =
                BindingContextData.GetServiceContainer(DataContext);

            if (svcProvider != null)
            {
                ISpeechRecognizerService speechSvc =
                    svcProvider.GetService<ISpeechRecognizerService>();

                if (speechSvc != null)
                {
                    if (speechSvc.Enabled)
                    {
                        speechSvc.Enabled = false;
                        speechSvc.Reset();

                        StartStopButton.Content = "Start";
                        // reset progress bar
                        CommandProgress.Value = 0;
                        // clear command list
                        PatchCommands.ItemsSource = null;
                        // clear text phrase and running patch
                        TextPhrase.Content = String.Empty;
                        RunningPatch.Content = String.Empty;
                    }
                    else
                    {
                        Preset preset = (Preset)PresetList.SelectedItem;

                        if (preset != null)
                        {
                            ISpeechInitializionService initSvc =
                                svcProvider.GetService<ISpeechInitializionService>();

                            if (initSvc != null)
                            {
                                initSvc.Initialize(preset, speechSvc);

                                IPatchExecuter patchExecuter =
                                    svcProvider.GetService<IPatchExecuter>();

                                if (patchExecuter != null)
                                {
                                    // prepare patches in preset
                                    patchExecuter.Prepare(preset);
                                }

                                speechSvc.Enabled = true;
                            }
                        }

                        // will remain false if something is not right
                        if (speechSvc.Enabled)
                        {
                            StartStopButton.Content = "Stop";
                        }
                    }
                }
            }
        }

        private void ClearHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            HistoryList.Items.Clear();
        }

        private void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            Patch patch = (Patch)PatchList.SelectedItem;

            if (patch != null)
            {
                RunPatch(patch);
            }
        }
    }
}