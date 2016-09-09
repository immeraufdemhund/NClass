using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;

namespace NClass.DependencyResolution
{
    public interface IConfigureContainer
    {
        IConfigureContainerItem<TBase> Register<TBase>();
    }

    internal class ConfigureContainer : IConfigureContainer, IDisposable
    {
        private readonly IUnityContainer _container;
        private readonly Stack<CommitToContainer<IUnityContainer>> _items = new Stack<CommitToContainer<IUnityContainer>>();

        public ConfigureContainer(IUnityContainer container)
        {
            _container = container;
        }

        public void Dispose()
        {
            while (_items.Count > 0)
                _items.Pop().Commit(_container);
        }

        public IConfigureContainerItem<TBase> Register<TBase>()
        {
            var item = new ConfigureContainerItem<TBase>();
            _items.Push(item);

            return item;
        }
    }
}