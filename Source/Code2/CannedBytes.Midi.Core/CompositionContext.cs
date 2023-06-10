using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace CannedBytes.ComponentModel.Composition;

public class CompositionContext : DisposableBase
{
    private readonly CompositionContainer _container;

    internal CompositionContext(CompositionContainer container)
    {
        _container = container;
        AddInstance(this);
    }

    public T GetInstance<T>()
    {
        return _container.GetExportedValue<T>();
    }

    public IEnumerable<T> GetInstances<T>()
    {
        return _container.GetExportedValues<T>();
    }

    public T GetInstanceOrDefault<T>()
    {
        return _container.GetExportedValueOrDefault<T>();
    }

    public void SatisfyImports(object instance)
    {
        _container.SatisfyImportsOnce(instance);
    }

    public void AddInstance<T>(T instance)
    {
        var batch = new CompositionBatch();
        batch.AddExportedValue<T>(instance);

        _container.Compose(batch);
    }

    protected override void Dispose(DisposeObjectKind disposeKind)
    {
        if (disposeKind == DisposeObjectKind.ManagedAndUnmanagedResources)
        {
            _container.Dispose();
        }
    }
}
