using System;
using System.Collections.Generic;

namespace CannedBytes.ComponentFramework
{
    [Obsolete]
    public class ServiceContainer : DisposableBase, IServiceContainer
    {
        private Dictionary<Type, Object> _services = new Dictionary<Type, object>();

        internal protected IDictionary<Type, Object> Services
        {
            get { return _services; }
        }

        #region IServiceContainer Members

        public void AddService<T>(T serviceInstance)
            where T : class
        {
            _services.Add(typeof(T), serviceInstance);

            IServiceContainerHost host = serviceInstance as IServiceContainerHost;

            if (host != null)
            {
                host.ServiceContainer = this;
            }
        }

        public void RemoveService<T>()
            where T : class
        {
            T instance = GetServiceInternal<T>();

            if (instance != null)
            {
                IDisposable disposable = instance as IDisposable;

                if (disposable != null)
                {
                    disposable.Dispose();
                }

                _services.Remove(typeof(T));
            }
        }

        #endregion IServiceContainer Members

        #region IServiceProvider Members

        public T GetService<T>()
            where T : class
        {
            T instance = GetServiceInternal<T>();

            return instance;
        }

        protected T GetServiceInternal<T>()
            where T : class
        {
            T instance = null;

            Type serviceType = typeof(T);
            if (_services.ContainsKey(serviceType))
            {
                instance = (T)_services[serviceType];
            }

            return instance;
        }

        #endregion IServiceProvider Members

        protected override void Dispose(DisposeObjectKind disposeKind)
        {
            if (disposeKind == DisposeObjectKind.ManagedAndUnmanagedResources)
            {
                foreach (object svc in _services.Values)
                {
                    IDisposable disposable = svc as IDisposable;

                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }

                _services.Clear();
                _services = null;
            }
        }
    }
}