﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace REstate.Engine.Services
{
    public interface IConnector
    {
        string ConnectorKey { get; }

        Func<CancellationToken, Task> ConstructAction(IStateMachine machineInstance, State state, string contentType, string payload, IDictionary<string, string> configuration);

        Func<CancellationToken, Task<bool>> ConstructPredicate(IStateMachine machineInstance, IDictionary<string, string> configuration);
    }
}
