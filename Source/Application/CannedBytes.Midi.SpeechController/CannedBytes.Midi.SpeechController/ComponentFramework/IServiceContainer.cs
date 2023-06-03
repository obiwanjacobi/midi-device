using System;

namespace CannedBytes.ComponentFramework
{
    [Obsolete]
    public interface IServiceContainer : IServiceProvider
    {
        void AddService<T>(T serviceInstance) where T : class;

        void RemoveService<T>() where T : class;
    }
}