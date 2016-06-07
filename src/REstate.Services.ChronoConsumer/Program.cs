﻿using System;
using System.IO;
using System.Reflection;
using Autofac;
using REstate.Chrono;
using REstate.Client;
using REstate.Repositories.Chrono.Susanoo;
using AutofacSerilogIntegration;
using Newtonsoft.Json;
using Psibr.Platform;
using Psibr.Platform.Logging.Serilog;
using REstate.Platform;
using Serilog;
using Serilog.Sinks.RollingFile;
using Topshelf;

namespace REstate.Services.ChronoConsumer
{
    internal class Program
    {
        private const string ServiceName = "REstate.Services.ChronoConsumer";

        private static void Main(string[] args)
        {
            var configString = PlatformConfiguration.LoadConfigurationFile("REstateConfig.json");

            var config = JsonConvert.DeserializeObject<REstatePlatformConfiguration>(configString);
            
            var kernel = BuildAndConfigureContainer(config).Build();

            HostFactory.Run(host =>
            {
                host.UseSerilog(kernel.Resolve<ILogger>());
                host.Service<ChronoConsumerService>(svc =>
                {
                    svc.ConstructUsing(() => kernel.Resolve<ChronoConsumerService>());
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

            container.RegisterInstance(new ConsumerServiceConfiguration
            {
                ApiKey = configuration.ChronoConsumerConfig.ApiKey
            });

            container.RegisterType<ChronoConsumerService>();

            container.RegisterModule<SerilogPlatformLoggingModule>();

            container.RegisterLogger(
                new LoggerConfiguration().MinimumLevel.Verbose()
                    .Enrich.WithProperty("source", ServiceName)
                    .WriteTo.LiterateConsole()
                    .If((loggerConfig) => configuration.LoggerConfigurations.ContainsKey("rollingFile")
                        && configuration.LoggerConfigurations["rollingFile"].ContainsKey("path"), (loggerConfig) =>
                           loggerConfig.WriteTo
                               .RollingFile($"{configuration.LoggerConfigurations["rollingFile"]["path"]}\\{ServiceName}\\{{Date}}.log"))
                    .If(_ => configuration.LoggerConfigurations.ContainsKey("seq"), loggerConfig =>
                        loggerConfig.WriteTo.Seq(configuration.LoggerConfigurations["seq"]["serverUrl"],
                            apiKey: configuration.LoggerConfigurations["seq"]["apiKey"]))
                    .CreateLogger());

            container.RegisterType<ChronoRepositoryFactory>()
                .As<IChronoRepositoryFactory>();

            container.Register(context => context.Resolve<IChronoRepositoryFactory>().OpenRepository());

            container.Register(context => new REstateClientFactory(configuration.AuthHttpService.Address + "apikey"))
                .As<IREstateClientFactory>();

            container.Register(context => context.Resolve<IREstateClientFactory>()
                .GetConfigurationClient(configuration.CoreHttpService.Address))
                .As<REstateConfigurationClient>();

            return container;
        }
    }

}
