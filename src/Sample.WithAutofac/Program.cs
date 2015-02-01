using System;
using Autofac;
using ConfigInjector;
using ConfigInjector.Configuration;

namespace Sample.WithAutofac
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

        private static IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<DeepThought>();

            ConfigurationConfigurator.RegisterConfigurationSettings()
                                     .FromAssemblies(typeof (DeepThought).Assembly)
                                     .RegisterWithContainer(configSetting => builder.RegisterInstance(configSetting)
                                                                                    .AsSelf()
                                                                                    .SingleInstance())
                                     .DoYourThing();

            return builder.Build();
        }
    }
}