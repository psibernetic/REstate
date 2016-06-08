﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nancy;
using REstate.Platform;
using REstate.Services;

namespace REstate.Web.Core.Modules
{
    /// <summary>
    /// Machine interactions module.
    /// </summary>
    public sealed class SchemaModule
        : NancyModule
    {

        public SchemaModule(REstatePlatformConfiguration configuration,
            IEnumerable<IConnectorFactory> connectorFactories)
        {
            this.After += (ctx, ct) =>
            {
                ctx.Response.Headers.Add("Cache-Control", "public, max-age=7200");

                return Task.FromResult(ctx.Response);
            };

            Get("/trigger", (parameters) =>
                Response.AsText(Schemas.Trigger
                    .Replace("{host}", configuration.CoreHttpService.Address)
                    .Replace(":/", "://"), "application/schema+json"));

            Get("/state", (parameters) =>
                Response.AsText(Schemas.State
                    .Replace("{host}", configuration.CoreHttpService.Address)
                    .Replace("//", "/")
                    .Replace(":/", "://"), "application/schema+json"));

            Get("/code", (parameters) =>
                Response.AsText(Schemas.Code
                    .Replace("{body}", Schemas.Body) //Default body schema
                    .Replace("{connectorKey}", "null")
                    .Replace("{host}", configuration.CoreHttpService.Address)
                    .Replace("//", "/")
                    .Replace(":/", "://"), "application/schema+json"));

            Get("/on-entry-from", (parameters) =>
                Response.AsText(Schemas.OnEntryFrom
                    .Replace("{host}", configuration.CoreHttpService.Address)
                    .Replace("//", "/")
                    .Replace(":/", "://"), "application/schema+json"));


            const string connectorDefintion =
                @"{""type"": ""object"", ""title"": ""{connectorName}"", ""$ref"": ""{host}connectors/{connectorName}"" }";

            var connectorDefinitions = new List<string>();

            foreach (var connectorFactory in connectorFactories)
            {
                var connectorName = connectorFactory.ConnectorKey.Replace("REstate.Connectors.", "");

                Get("/connectors/" + connectorName, (parameters) =>
                    Response.AsText(Schemas.Code
                        .Replace("{body}", connectorName == "Chrono" ? Schemas.ChronoConnector : Schemas.Body)
                        //Default body schema
                        .Replace("{connectorKey}", $"\"{connectorFactory.ConnectorKey}\"")
                        .Replace("{connectorName}", connectorName)
                        .Replace("{host}", configuration.CoreHttpService.Address)
                        .Replace("//", "/")
                        .Replace(":/", "://"), "application/schema+json"));

                connectorDefinitions.Add(
                    connectorDefintion
                        .Replace("{host}", configuration.CoreHttpService.Address)
                        .Replace("{connectorName}", connectorName));
            }

            var flattened =
                connectorDefinitions.Aggregate((prev, curr) => (prev ?? "") + (prev != null ? ", " : "") + curr);

            Get("/machine", (parameters) =>
                Response.AsText(Schemas.Machine
                    .Replace("{connectors}", flattened)
                    .Replace("{host}", configuration.CoreHttpService.Address)
                    .Replace("//", "/")
                    .Replace(":/", "://"), "application/schema+json"));
        }

    }
}