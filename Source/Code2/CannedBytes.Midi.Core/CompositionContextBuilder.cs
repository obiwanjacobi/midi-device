using System.ComponentModel.Composition.Hosting;
using System.Reflection;

namespace CannedBytes.ComponentModel.Composition;

public class CompositionContextBuilder
{
    private readonly AggregateCatalog _catalog = new();

    public virtual CompositionContext ToCompositionContext()
    {
        CompositionContainer container = new(_catalog);
        return new CompositionContext(container);
    }

    public void AddDirectory(string directoryLocation)
    {
        DirectoryCatalog dirCat = new(directoryLocation);
        _catalog.Catalogs.Add(dirCat);
    }

    public void AddAssembly(Assembly assembly)
    {
        AssemblyCatalog assCat = new(assembly);
        _catalog.Catalogs.Add(assCat);
    }
}
