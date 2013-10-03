using System;
using ConfigInjector;
using Ninject;

namespace Sample.WithNinject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var kernel = CreateKernel())
            {
                var deepThought = kernel.Get<DeepThought>();

                deepThought.DoSomeThinking();
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to quit.");
            Console.ReadKey();
        }

        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();

            kernel.Bind<DeepThought>().ToSelf();

            ConfigurationConfigurator.RegisterConfigurationSettings()
                                     .FromAssemblies(typeof (DeepThought).Assembly)
                                     .RegisterWithContainer(configSetting => kernel.Bind(configSetting.GetType())
                                                                                   .ToConstant(configSetting))
                                     .DoYourThing();

            return kernel;
        }
    }
}