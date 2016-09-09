using Microsoft.Practices.Unity;

namespace NClass.DependencyResolution
{
    public interface IConfigureContainerItem<in TBase>
    {
        void To<TImplementation>() where TImplementation : class, TBase;
        void With(TBase instance);
    }

    internal class ConfigureContainerItem<TBase> : CommitToContainer<IUnityContainer>, IConfigureContainerItem<TBase>
    {
        public void To<TImplementation>() where TImplementation : class, TBase
        {
            _commitAction = (x) => { x.RegisterType<TBase, TImplementation>(); };
        }

        public void With(TBase instance)
        {
            _commitAction = (x) => { x.RegisterInstance(instance); };

        }
    }
}