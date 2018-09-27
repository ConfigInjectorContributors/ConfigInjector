using System;
using Autofac;
using ConfigInjector.Configuration;
using ConfigInjector.Containers.Autofac;
using ConfigInjector.Infrastructure.Logging;
using ConfigInjector.Infrastructure.SettingsOverriders;
using ConfigInjector.Sources.DotNet;
using CoreSample.WithAutofac.Configuration;

namespace CoreSample.WithAutofac
{
    public static class Program
    {
        public static void Main()
        {
            var configuration = ConfigurationConfigurator.RegisterConfigurationSettings()
                                                         .FromDotNetConfiguration<AppSettingsRoot>()
                                                         .WithOverridesFrom<NoOpSettingsOverrider>()
                                                         .WithOverridesFrom<EnvironmentVariableSettingsOverrider>()
                                                         .WithLogger(new ConsoleLogger())
                                                         .DoYourThing();

            var builder = new ContainerBuilder();
            builder.RegisterSource(new ConfigInjectorRegistrationSource(configuration));
            builder.RegisterType<NoOpSettingsOverrider>().InstancePerLifetimeScope();
            builder.RegisterType<EnvironmentVariableSettingsOverrider>().InstancePerLifetimeScope();

            using (var container = builder.Build())
            {
                var smtpSettings = container.Resolve<SmtpSettings>();
                Console.WriteLine($"{smtpSettings.Host}:{smtpSettings.Port}");
            }
        }
    }
}