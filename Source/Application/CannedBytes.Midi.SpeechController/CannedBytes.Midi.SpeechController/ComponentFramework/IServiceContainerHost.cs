using System;

namespace CannedBytes.ComponentFramework
{
    [Obsolete]
    public interface IServiceContainerHost
    {
        IServiceContainer ServiceContainer { get; set; }
    }
}