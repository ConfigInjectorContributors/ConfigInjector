using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using Autofac.Core.Activators.Delegate;
using Autofac.Core.Lifetime;
using Autofac.Core.Registration;

namespace ConfigInjector.Containers.Autofac
{
    public class ConfigInjectorRegistrationSource : IRegistrationSource
    {
        private readonly ConfigInjectorInstance _configuration;

        public ConfigInjectorRegistrationSource(ConfigInjectorInstance configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<IComponentRegistration> RegistrationsFor(Service service, Func<Service, IEnumerable<IComponentRegistration>> registrationAccessor)
        {
            var typedService = service as TypedService;
            if (typedService == null) yield break;

            if (!typedService.ServiceType.IsAssignableTo<IConfigurationSetting>()) yield break;

            var registration = new ComponentRegistration(Guid.NewGuid(),
                                                         new DelegateActivator(typedService.ServiceType,
                                                                               (c, p) =>
                                                                               {
                                                                                   var dependencyResolver = new AutofacDependencyResolver(c);
                                                                                   var instance = _configuration.Get(typedService.ServiceType, dependencyResolver);
                                                                                   return instance;
                                                                               }),
                                                         new CurrentScopeLifetime(),
                                                         InstanceSharing.None,
                                                         InstanceOwnership.OwnedByLifetimeScope,
                                                         new[] {service},
                                                         new Dictionary<string, object>());

            yield return registration;
        }

        public bool IsAdapterForIndividualComponents => false;
    }
}