using System;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.Windows;

namespace CannedBytes.Midi.DeviceTestApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Container = InitContainer();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            Container.Dispose();
        }

        public CompositionContainer Container { get; private set; }

        public new static App Current
        {
            get { return (App)Application.Current; }
        }

        private CompositionContainer InitContainer()
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new DirectoryCatalog(Environment.CurrentDirectory));

            //catalog.Catalogs.Add(new AssemblyCatalog("CannedBytes.Midi.Device.dll"));
            //catalog.Catalogs.Add(new AssemblyCatalog("CannedBytes.Midi.Device.Schema.Xml.dll"));
            //catalog.Catalogs.Add(new AssemblyCatalog("CannedBytes.Midi.Device.Message.dll"));
            //catalog.Catalogs.Add(new AssemblyCatalog("CannedBytes.Midi.Device.Roland.dll"));
            //catalog.Catalogs.Add(new AssemblyCatalog("CannedBytes.Midi.Device.Roland.U220.dll"));
            catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetEntryAssembly()));

            var container = new CompositionContainer(catalog);

            return container;
        }
    }
}