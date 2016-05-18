﻿using System;
using System.Collections.Generic;
using Autofac;
using AutofacSerilogIntegration;
using Nancy.Bootstrapper;
using Newtonsoft.Json;
using Psibr.Platform;
using Psibr.Platform.Logging;
using Psibr.Platform.Logging.Serilog;
using Psibr.Platform.Nancy;
using Psibr.Platform.Nancy.Service;
using Psibr.Platform.Serialization;
using Psibr.Platform.Serialization.NewtonsoftJson;
using REstate.Connectors.Decorators.Task;
using REstate.Platform;
using REstate.Repositories.Configuration;
using REstate.Repositories.Core.Susanoo;
using REstate.Stateless;
using REstate.Web.Core;
using Serilog;
using Topshelf;

namespace REstate.Services.Core
{
    class Program
    {
        const string ServiceName = "REstate.Services.Core";

        static void Main(string[] args)
        {
            var configString = PlatformConfiguration.LoadConfigurationFile("REstateConfig.json");

            var config = JsonConvert.DeserializeObject<REstatePlatformConfiguration>(configString);

            var kernel = BuildAndConfigureContainer(config).Build();
            var x = kernel.Resolve<IEnumerable<IConnectorFactory>>();
            HostFactory.Run(host =>
            {
                host.UseSerilog(kernel.Resolve<ILogger>());
                host.Service<PlatformApiService<REstatePlatformConfiguration>>(svc =>
                {
                    svc.ConstructUsing(() => kernel.Resolve<PlatformApiService<REstatePlatformConfiguration>>());
                    svc.WhenStarted(service => service.Start());
                    svc.WhenStopped(service => service.Stop());
                });

                if (config.ServiceCredentials.Username.Equals("NETWORK SERVICE", StringComparison.OrdinalIgnoreCase))
                    host.RunAsNetworkService();
                else
                    host.RunAs(config.ServiceCredentials.Username, config.ServiceCredentials.Password);

                host.StartAutomatically();

                host.SetServiceName(ServiceName);
            });
        }

        private static ContainerBuilder BuildAndConfigureContainer(REstatePlatformConfiguration configuration)
        {
            var container = new ContainerBuilder();

            container.Register(ctx => configuration)
                .As<IPlatformConfiguration, PlatformConfiguration, REstatePlatformConfiguration>();

            container.RegisterInstance(new ApiServiceConfiguration<REstatePlatformConfiguration>(
                configuration, configuration.CoreHttpService));

            container.RegisterType<PlatformNancyBootstrapper>()
                .As<INancyBootstrapper>();

            container.RegisterType<PlatformApiService<REstatePlatformConfiguration>>();

            container.RegisterModule<SerilogPlatformLoggingModule>();

            container.RegisterLogger(
                new LoggerConfiguration().MinimumLevel.Verbose()
                    .Enrich.WithProperty("source", ServiceName)
                    .WriteTo.LiterateConsole()
                    .If(_ => configuration.LoggerConfigurations.ContainsKey("rollingFile") 
                        && configuration.LoggerConfigurations["rollingFile"].ContainsKey("path"),  (loggerConfig) =>
                            loggerConfig.WriteTo
                                .RollingFile($"{configuration.LoggerConfigurations["rollingFile"]["path"]}\\{ServiceName}\\{{Date}}.log"))
                    .If(_ => configuration.LoggerConfigurations.ContainsKey("seq"), loggerConfig =>
                        loggerConfig.WriteTo.Seq(configuration.LoggerConfigurations["seq"]["serverUrl"],
                            apiKey: configuration.LoggerConfigurations["seq"]["apiKey"]))
                    .CreateLogger());

            container.RegisterType<NewtonsoftJsonSerializer>()
                .As<IStringSerializer, IByteSerializer>();

            container.RegisterType<ConfigurationRepositoryContextFactory>()
                .As<IConfigurationRepositoryContextFactory>();

            container.RegisterConnectors(configuration);

            container.RegisterType<StatelessStateMachineFactory>()
                .As<IStateMachineFactory>();

            container.RegisterType<DecoratingConnectorFactoryResolver>()
                .UsingConstructor(() => new DecoratingConnectorFactoryResolver(
                    null, (ConnectorDecoratorAssociations)null))
                .As<IConnectorFactoryResolver>();

            container.Register(ctx => new TaskConnectorDecorator(ctx.Resolve<IPlatformLogger>()))
                .AsSelf()
                .Named<IConnectorDecorator>("REstate.Connectors.Decorators.Task")
                .As<IConnectorDecorator>();

            container.Register(ctx => new ConnectorDecoratorAssociations
            {
                Associations = new Dictionary<string, IEnumerable<IConnectorDecorator>>
                {
                    { "REstate.Connectors.RabbitMq", new []
                    {
                        ctx.ResolveNamed<IConnectorDecorator>("REstate.Connectors.Decorators.Task")
                    }}
                }
            });

            return container;
        }
    }
}