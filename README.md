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

# FAQ
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