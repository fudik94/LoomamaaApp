using System;
using System.Collections.Generic;

namespace LoomamaaApp
{
    public class ServiceLocator
    {
        private static ServiceLocator instance;
        private readonly Dictionary<Type, object> services = new Dictionary<Type, object>();
        private readonly Dictionary<Type, Func<object>> factories = new Dictionary<Type, Func<object>>();

        private ServiceLocator() { }

        public static ServiceLocator Instance
        {
            get
            {
                if (instance == null)
                    instance = new ServiceLocator();
                return instance;
            }
        }

        public void Register<TInterface, TImplementation>() where TImplementation : TInterface, new()
        {
            factories[typeof(TInterface)] = () => new TImplementation();
        }

        public void Register<TInterface>(Func<TInterface> factory)
        {
            factories[typeof(TInterface)] = () => factory();
        }

        public void RegisterSingleton<TInterface>(TInterface instance)
        {
            services[typeof(TInterface)] = instance;
        }

        public T Resolve<T>()
        {
            var type = typeof(T);

            // Check if singleton is registered
            if (services.ContainsKey(type))
                return (T)services[type];

            // Check if factory is registered
            if (factories.ContainsKey(type))
                return (T)factories[type]();

            throw new InvalidOperationException($"Service of type {type.Name} is not registered.");
        }
    }
}
