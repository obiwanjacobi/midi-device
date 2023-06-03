using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using CannedBytes.ComponentFramework;
using CannedBytes.Midi.SpeechController.Dependencies;
using CannedBytes.Midi.SpeechController.DomainModel;
using CannedBytes.Midi.SpeechController.Service;

namespace CannedBytes.Midi.SpeechController
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>

    public partial class App : System.Windows.Application
    {
        /// <summary>
        /// Application Entry Point.
        /// </summary>
        [STAThreadAttribute()]
        public static void Main()
        {
            DependencyLoader.Register();

            var app = new App();
            app.InitializeComponent();
            app.Run();
        }

        public App()
        {
            _dataCtx = new BindingContextData();

            _dataCtx.ServiceContainer = InitializeServices();

#if DEBUG
            //_dataCtx.Presets = new PresetCollection();
            _dataCtx.Presets = InitializeTestData();
#else
            _dataCtx.Presets = new PresetCollection();
#endif
        }

        private BindingContextData _dataCtx;

        internal BindingContextData DataContext
        {
            get { return _dataCtx; }
        }

        public static new App Current
        {
            get { return (App)Application.Current; }
        }

        private IServiceContainer InitializeServices()
        {
            IServiceContainer svcContainer = new ServiceContainer();

            // add services to the container.
            svcContainer.AddService<ISpeechRecognizerService>(new SpeechRecognizerService());
            svcContainer.AddService<ISpeechInitializionService>(new SpeechInitializationService());
            svcContainer.AddService<IBinaryTextService>(new BinaryTextService());
            svcContainer.AddService<IMidiMessageFactory>(new MidiMessageFactory());
            svcContainer.AddService<IMidiOutPortService>(new MidiOutPortService());
            svcContainer.AddService<IPatchExecuter>(new PatchExecuter());
            svcContainer.AddService<IAudioService>(new AudioService());
            svcContainer.AddService<ITextToSpeechService>(new TextToSpeechService());

            return svcContainer;
        }

#if DEBUG

        private PresetCollection InitializeTestData()
        {
            try
            {
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CannedBytes.Midi.SpeechController.TestDictionary.xaml"))
                {
                    // cast it anyway to ensure we get the type we expect.
                    return (PresetCollection)XamlReader.Load(stream);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }

            return null;
        }

#endif
    }
}