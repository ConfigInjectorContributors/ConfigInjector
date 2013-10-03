# ConfigInjector

## How application settings *should* look:

Here's a class that needs some configuration settings:

    public class EmailSender : IEmailSender
    {
        private readonly SmtpHostConfigurationSetting _smtpHost;
        private readonly SmtpPortConfigurationSetting _smtpPort;

        public EmailSender(SmtpHostConfigurationSetting smtpHost,
                           SmtpPortConfigurationSetting smtpPort)
        {
            _smtpHost = smtpHost;
            _smtpPort = smtpPort;
        }

        public void Send(MailMessage message)
        {
            // NOTE the way we can use our strongly-typed settings directly as a string and int respectively
            using (var client = new SmtpClient(_smtpHost, _smtpPort))
            {
                client.Send(message);
            }
        }
    }

Here's how we declare the settings:
    
    // This will give us a strongly-typed string setting.
    public class SmtpHostConfigurationSetting : ConfigurationSetting<string>
    {
    }
    
    // This will give us a strongly-typed int setting.
    public class SmtpPortConfigurationSetting : ConfigurationSetting<int>
    {
    }

... and here's how we set them in our [web|app].config:

    <?xml version="1.0" encoding="utf-8" ?>
    <configuration>
      <appSettings>
        <add key="SmtpHostConfigurationSetting" value="localhost" />
        <add key="SmtpPortConfigurationSetting" value="25" />
      </appSettings>
    </configuration>

## Getting started

In the NuGet Package Manager Console, type:

    Install-Package ConfigInjector
    
then run up the configurator like this:

    ConfigurationConfigurator.RegisterConfigurationSettings()
                             .FromAssemblies(/* TODO: Provide a list of assemblies to scan for configuration settings here  */)
                             .RegisterWithContainer(configSetting => /* TODO: Register this instance with your container here */ )
                             .DoYourThing();

You can pick your favourite container from the list below or roll your own.

### Getting started with Autofac

    var builder = new ContainerBuilder();

    builder.RegisterType<DeepThought>();

    ConfigurationConfigurator.RegisterConfigurationSettings()
                             .FromAssemblies(typeof (DeepThought).Assembly)
                             .RegisterWithContainer(configSetting => builder.RegisterInstance(configSetting)
                                                                            .AsSelf()
                                                                            .SingleInstance())
                             .DoYourThing();

    return builder.Build();

### Getting started with Castle Windsor

    var container = new WindsorContainer();

    container.Register(Component.For<DeepThought>());

    ConfigurationConfigurator.RegisterConfigurationSettings()
                             .FromAssemblies(typeof (DeepThought).Assembly)
                             .RegisterWithContainer(configSetting => container.Register(Component.For(configSetting.GetType())
                                                                                                 .Instance(configSetting)
                                                                                                 .LifestyleSingleton()))
                             .DoYourThing();

    return container;

### Getting started with Ninject

    var kernel = new StandardKernel();

    kernel.Bind<DeepThought>().ToSelf();

    ConfigurationConfigurator.RegisterConfigurationSettings()
                             .FromAssemblies(typeof (DeepThought).Assembly)
                             .RegisterWithContainer(configSetting => kernel.Bind(configSetting.GetType())
                                                                           .ToConstant(configSetting))
                             .DoYourThing();

    return kernel;


