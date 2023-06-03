using System;

namespace CannedBytes.ComponentFramework
{
    [Obsolete]
    public interface IServiceProvider
    {
        T GetService<T>() where T : class;
    }
}