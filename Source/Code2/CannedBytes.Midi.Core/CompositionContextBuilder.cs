using System.ComponentModel.Composition.Hosting;
using System.Reflection;

namespace CannedBytes.ComponentModel.Composition
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public class CompositionContextBuilder
    {
        private readonly AggregateCatalog _catalog = new AggregateCatalog();

        public virtual CompositionContext ToCompositionContext()
        {
            var container = new CompositionContainer(_catalog);

            return new CompositionContext(container);
        }

        public void AddDirectory(string directoryLocation)
        {
            var dirCat = new DirectoryCatalog(directoryLocation);
            _catalog.Catalogs.Add(dirCat);
        }

        public void AddAssembly(Assembly assembly)
        {
            var assCat = new AssemblyCatalog(assembly);
            _catalog.Catalogs.Add(assCat);
        }
    }
}
