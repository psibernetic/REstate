﻿using Nancy;
using Nancy.ModelBinding;
using REstate.Configuration;
using REstate.Engine;
using REstate.Logging;
using REstate.Web.Requests;
using REstate.Web.Responses;
using System;
using System.Linq;

namespace REstate.Web.Modules
{
    /// <summary>
    /// Machine interactions module.
    /// </summary>
    public class InstancesModule
        : SecuredModule
    {
        protected IPlatformLogger Logger { get; }

        protected StateEngineFactory StateEngineFactory { get; }

        /// <summary>
        /// Registers routes for interacting with machines.
        /// </summary>
        /// <param name="configurationRepositoryContextFactory">The repository context factory.</param>
        /// <param name="stateMachineFactory">The state machine factory.</param>
        /// <param name="logger"></param>
        public InstancesModule(
            StateEngineFactory stateEngineFactory,
            IPlatformLogger logger)
            : base("/instances", claim => claim.Type == "claim" && claim.Value == "operator")
        {
            Logger = logger;
            StateEngineFactory = stateEngineFactory;
            

            GetMachineState();

            IsInState();

            GetAvailableTriggers();

            FireTrigger();

            GetDiagramForInstance();

            GetInstanceMetadata();

            DeleteInstance();
        }

        private void GetInstanceMetadata() =>
            Get("/{MachineInstanceId}", async (parameters, ct) =>
            {
                var stateEngine = StateEngineFactory
                    .GetStateEngine(Context.CurrentUser.GetApiKey());

                string machineInstanceId = parameters.machineInstanceId;

                string metadata = await stateEngine.GetInstanceMetadataRaw(machineInstanceId, ct);

                return Response.AsText(metadata ?? "{ }", "application/json");
            });

        private void GetDiagramForInstance() =>
            Get("/{MachineInstanceId}/diagram", async (parameters, ct) =>
            {
                var stateEngine = StateEngineFactory
                    .GetStateEngine(Context.CurrentUser.GetApiKey());

                string machineInstanceId = parameters.machineInstanceId;

                IStateMachine machine = await stateEngine
                    .GetInstance(machineInstanceId, ct);

                return Response.AsText(machine.ToString(), "text/plain");
            });

        private void DeleteInstance() =>
            Delete("/{MachineInstanceId}", async (parameters, ct) =>
            {
                var stateEngine = StateEngineFactory
                    .GetStateEngine(Context.CurrentUser.GetApiKey());

                string machineInstanceId = parameters.machineInstanceId;

                await stateEngine.DeleteInstance(machineInstanceId, ct);

                return HttpStatusCode.Accepted;
            });

        private void FireTrigger() =>
            Post("/{MachineInstanceId}/fire/{TriggerName}", async (parameters, ct) =>
            {
                var stateEngine = StateEngineFactory
                    .GetStateEngine(Context.CurrentUser.GetApiKey());

                var triggerFireRequest = this.Bind<TriggerFireRequest>();
                InstanceRecord instanceRecord;

                IStateMachine machine = await stateEngine
                    .GetInstance(triggerFireRequest.MachineInstanceId, ct);

                try
                {
                    machine.Fire(new Trigger(machine.MachineDefinitionId,
                        triggerFireRequest.TriggerName),
                        triggerFireRequest.Payload);
                }
                catch (InvalidOperationException ex)
                {
                    return Negotiate
                        .WithStatusCode(400)
                        .WithReasonPhrase(ex.Message)
                        .WithModel(new ReasonPhraseResponse { ReasonPhrase = ex.Message });
                }
                catch (StateConflictException ex)
                {
                    return Negotiate
                        .WithStatusCode(409)
                        .WithReasonPhrase(ex.Message)
                        .WithModel(new ReasonPhraseResponse { ReasonPhrase = ex.Message });
                }
                catch (AggregateException ex)
                    when (ex.InnerExceptions.First().GetType() == typeof(StateConflictException))
                {
                    return Negotiate
                        .WithStatusCode(409)
                        .WithReasonPhrase(ex.InnerExceptions.First().Message)
                        .WithModel(new ReasonPhraseResponse { ReasonPhrase = ex.InnerExceptions.First().Message });
                }

                instanceRecord = await stateEngine.GetInstanceInfo(triggerFireRequest.MachineInstanceId, ct);

                return instanceRecord;
            });

        private void GetAvailableTriggers() =>
            Get("/{MachineInstanceId}/triggers", async (parameters, ct) =>
            {
                var stateEngine = StateEngineFactory
                    .GetStateEngine(Context.CurrentUser.GetApiKey());

                string machineInstanceId = parameters.MachineInstanceId;

                var machine = await stateEngine
                    .GetInstance(machineInstanceId, ct);

                return machine.PermittedTriggers.Select(trigger =>
                    new Responses.Trigger
                    {
                        MachineName = trigger.MachineDefinitionId,
                        TriggerName = trigger.TriggerName
                    }).ToList();
            });

        private void IsInState() =>
            Get("/{MachineInstanceId}/isinstate/{StateName}", async (parameters, ct) =>
            {
                var stateEngine = StateEngineFactory
                    .GetStateEngine(Context.CurrentUser.GetApiKey());

                string machineInstanceId = parameters.MachineInstanceId;
                string isInStateName = parameters.StateName;

                var machine = await stateEngine
                    .GetInstance(machineInstanceId, ct);

                var isInState = machine.IsInState(
                        new State(machine.MachineDefinitionId, isInStateName));

                return new IsInStateResponse { QueriedStateName = isInStateName, IsInState = isInState };
            });

        private void GetMachineState() =>
            Get("/{MachineInstanceId}/state", async (parameters, ct) =>
            {
                var stateEngine = StateEngineFactory
                    .GetStateEngine(Context.CurrentUser.GetApiKey());

                string machineInstanceId = parameters.MachineInstanceId;

                var instanceRecord = await stateEngine
                    .GetInstanceInfo(machineInstanceId, ct);

                if (instanceRecord == null)
                    return new Response
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        ReasonPhrase = "The machine instance requested does not exist."
                    };

                return instanceRecord;
            });
    }
}
