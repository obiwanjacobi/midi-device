using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using Caliburn.Micro;
using CannedBytes.Midi.Console.UI.ViewModels;
using Xceed.Wpf.Toolkit;

namespace CannedBytes.Midi.Console
{
    class AppBootstrapper : Bootstrapper<AppViewModel>
    {
        private CompositionContainer _container;

        protected override void Configure()
        {
            var cat = CreateCompositionCatalog();
            _container = new CompositionContainer(cat);

            PopulateContainer(_container);
        }

        private void PopulateContainer(CompositionContainer container)
        {
            var batch = new CompositionBatch();

            batch.AddExportedValue<IWindowManager>(new WindowManager());
            batch.AddExportedValue<IEventAggregator>(new EventAggregator());
            batch.AddExportedValue(container);

            container.Compose(batch);
        }

        private AggregateCatalog CreateCompositionCatalog()
        {
            var cat = new AggregateCatalog(
                AssemblySource.Instance.Select(x => new AssemblyCatalog(x)).OfType<ComposablePartCatalog>());

            return cat;
        }

        protected override object GetInstance(Type service, string key)
        {
            string contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(service) : key;

            var exports = _container.GetExportedValues<object>(contract);

            if (exports.Count() > 0)
                return exports.First();

            throw new Exception(string.Format("Could not locate any instances of contract '{0}'.", contract));
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetExportedValues<object>(AttributedModelServices.GetContractName(service));
        }

        protected override void BuildUp(object instance)
        {
            _container.SatisfyImportsOnce(instance);
        }

        protected override void OnUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString(), "ERROR - please copy this text (Ctrl+C) into an email and send to obiwanjacobi@hotmail.com");

            base.OnUnhandledException(sender, e);
        }
    }
}