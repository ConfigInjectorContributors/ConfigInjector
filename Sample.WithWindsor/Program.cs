using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using ConfigInjector;

namespace Sample.WithWindsor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var container = CreateContainer())
            {
                var deepThought = container.Resolve<DeepThought>();

                deepThought.DoSomeThinking();
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to quit.");
            Console.ReadKey();
        }

        private static IWindsorContainer CreateContainer()
        {
            var container = new WindsorContainer();

            container.Register(Component.For<DeepThought>());

            ConfigurationConfigurator.RegisterConfigurationSettings()
                                     .FromAssemblies(typeof (DeepThought).Assembly)
                                     .RegisterWithContainer(configSetting => container.Register(Component.For(configSetting.GetType()).Instance(configSetting).LifestyleSingleton()))
                                     .DoYourThing();

            return container;
        }
    }
}