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
        protected override IEnumerable<string> ValidationErrors(int value)
        {
            if (value <= 0) yield return "TCP port numbers cannot be negative.";
            if (value > 65535) yield return "TCP port numbers cannot be greater than 65535.";
        }
    }

Here's how we set them in our [web|app].config:

    <?xml version="1.0" encoding="utf-8" ?>
    <configuration>
      <appSettings>
        <add key="SmtpHostConfigurationSetting" value="localhost" />
        <add key="SmtpPortConfigurationSetting" value="25" />
      </appSettings>
    </configuration>

... and here's how we provide mock values for them in our unit tests:

    var smtpHost = new SmtpHostConfigurationSetting {Value = "smtp.example.com"};
    var smtpPort = new SmtpPortConfigurationSetting {Value = 25};
    
    var emailSender = new EmailSender(smtpHost, smtpPort);
    
    emailSender.Send(someTestMessage);

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
                             .FromAssemblies(ThisAssembly)
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

# FAQ
## What naming conventions should I use for the configuration settings?

There are two naming conventions that are applied by default. With the first, `DefaultSettingKeyConvention`, the name of the configuration setting class should match the key used in the `appSettings` section of your configuration file. So given a key called `StorageConnectionString` the corresponding configuration setting class should be called `StorageConnectionString`. The second, `WithSuffixSettingKeyConvention` strips 'Setting' from the name of the configuration class, so given a key called `StorageConnectionString` the corresponding configuration setting class could be called `StorageConnectionStringSetting`.

Additional naming conventions can be configured when registering ConfigInjector by providing your own implementations of `ISettingKeyConvention`, using the `WithSettingKeyConventions()` method before calling `DoYourThing()`:

    ConfigurationConfigurator.RegisterConfigurationSettings()
                             .FromAssemblies(...)
                             .RegisterWithContainer(...)
                             .WithSettingKeyConventions(new MyCustomKeyConvention())
                             .DoYourThing();

## What types can I use?

* Any type that has a public static .Parse(string someValue) method.
    * This includes TimeSpan, DateTime and a bunch of others.
    * Yes, you can use your own custom types here if you wish. Just don't get too tricky...
* Enums
    * Just remember that they're case-sensitive - and they *should* be case-sensitive, because SomeEnum.Foo and SomeEnum.FOO are different.
* Uris
* Any type that Convert.ChangeType can convert from a string.
* Any types for which you provide a custom parser.

## Can I provide my own type converters?

Yep. Just provide your custom value parsers to the WithCustomValueParsers configuration method.

    ConfigurationConfigurator.RegisterConfigurationSettings()
                             .FromAssemblies(/* TODO: Provide a list of assemblies to scan for configuration settings here  */)
                             .RegisterWithContainer(configSetting => /* TODO: Register this instance with your container here */ )
                             .WithCustomValueParsers(new PersonNameValueParser())
                             .DoYourThing();

Each of your parsers should implement the IValueParser interface:

    public class EnumValueParser : IValueParser
    {
        public int SortOrder
        {
            get { return int.MaxValue - 2; }
        }

        public bool CanParse(Type settingValueType)
        {
            return settingValueType.BaseType == typeof (Enum);
        }

        public object Parse(Type settingValueType, string settingValueString)
        {
            return Enum.Parse(settingValueType, settingValueString);
        }
    }

## How can I exclude certain settings that are causing an ExtraneousSettingsException?

Some external packages add configuration that your application doesn't care about, such as Microsoft.AspNet.Mvc. These will cause an ExtraneousSettingsException because you have not created classes for them. **This is desired behaviour**. We want to discourage any sneaky references to ConfigurationManager.AppSettings[...] and an easy way to do that is to simply not permit any settings that we haven't wrapped in setting classes.

If you do not want to create classes for them since you will not be using them, while running up the Configurator, just call "ExcludeSettingKeys" passing in the string array of keys to ignore. E.g:

    ConfigurationConfigurator.RegisterConfigurationSettings()
                             .FromAssemblies(/* TODO: Provide a list of assemblies to scan for configuration settings here  */)
                             .RegisterWithContainer(configSetting => /* TODO: Register this instance with your container here */ )
                             .ExcludeSettingKeys(new[] { "ExampleSettingKey1", "webpages:Version", "webpages:Enabled" })
                             .DoYourThing();

IF you genuinely don't want to assert that nobody is using silly/dangerous inline configuration settings, you can use the following:

    ConfigurationConfigurator.RegisterConfigurationSettings()
                             .FromAssemblies(/* TODO: Provide a list of assemblies to scan for configuration settings here  */)
                             .RegisterWithContainer(configSetting => /* TODO: Register this instance with your container here */ )
                             .AllowConfigurationEntriesThatDoNotHaveSettingsClasses(true)
                             .DoYourThing();
                             
This approach is also useful for environments like Windows Azure, where 1) enumerating all configuration settings is not supported and 2) the hosting environment may randomly add unexpected settings to your application's configuration.

You can also ignore the reverse of this, i.e. settings classes that don't have a corresponding configuration entry, by using the following:

    ConfigurationConfigurator.RegisterConfigurationSettings()
                             .FromAssemblies(/* TODO: Provide a list of assemblies to scan for configuration settings here  */)
                             .RegisterWithContainer(configSetting => /* TODO: Register this instance with your container here */ )
                             .AllowSettingsClassesThatDoNotHaveConfigurationEntries(true)
                             .DoYourThing();

This can be useful if you have a shared set of configuration settings, but some applications don't need to specify or resolve values for all of the settings. The configuration settings that don't exist in the `[web|app].config` file won't be registered with the container.


## Can I load settings directly without configuring my container first?

You can do this but I'd give some serious thought to whether it's a good idea in your particular case.

    var setting = DefaultSettingsReader.Get<SimpleIntSetting>();

If you genuinely need access to settings before your container is wired up, go ahead. If you're using ConfigInjector as a settings service locator across your entire app, you're holding it wrong :)

ConfigInjector will make an intelligent guess at defaults. It will, for instance, walk the call stack that invoked it and look for assemblies that contain settings and value parsers. If you have custom value parsers it will pick those up, too, provided that they're not off in a satellite assembly somewhere.

If you need to globally change the default behaviour, create a class that implements IStaticSettingReaderStrategy:

    public class MyCustomSettingsReaderStrategy : IStaticSettingReaderStrategy
    {
        // ...
    }

and use use this to wire it up:

    DefaultSettingsReader.SetStrategy(new MyCustomSettingsReaderStrategy());

## What if I want to programmatically mess with settings values after I've read them?

There are a couple of cases where you'd want to do this. Let's compare SQL Server versus Windows Service Bus as examples.

With SQL Server, you can happily use a connection string containing a . for localhost:

    <add key="DatabaseConnectionString" value="Data Source=.\SQLEXPRESS;Initial Catalog=SecretDatabase-Dev;Integrated Security=SSPI;" />

With something like Windows/Azure Service Bus, however, it will have an opinion about this because it uses TLS and the certificate CN won't match, so something like this won't work:

    <add key="ServiceBusConnectionString" value="Endpoint=sb://localhost/SecretServiceBus-Dev;StsEndpoint=https://localhost:9355/SecretServiceBus-Dev;RuntimePort=9354;ManagementPort=9355" />

What we can do instead is this:

    <add key="ServiceBusConnectionString" value="Endpoint=sb://{MachineName}/SecretServiceBus-Dev;StsEndpoint=https://{MachineName}:9355/SecretServiceBus-Dev;RuntimePort=9354;ManagementPort=9355" />

and then override that configuration setting's value on the way out:

    public class ServiceBusConnectionString : ConfigurationSetting<string>
    {
        public override string Value
        {
            get { return base.Value.Replace("{MachineName}", Environment.MachineName); }
            set { base.Value = value; }
        }
    }

