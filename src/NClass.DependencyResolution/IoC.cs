using System;
using Microsoft.Practices.Unity;

namespace NClass.DependencyResolution
{
    public class IoC
    {
        private static IUnityContainer CONTAINER = new UnityContainer();

        public static void Reset()
        {
            CONTAINER = new UnityContainer();
        }

        public static T Get<T>()
        {
            return CONTAINER.Resolve<T>();
        }

        public static void Configure(Action<IConfigureContainer> configurationAction)
        {
            using (var configurationContainer = new ConfigureContainer(CONTAINER))
            {
                configurationAction(configurationContainer);
            }
        }
    }
}
