﻿using System;
using System.Threading.Tasks;
using Composable.Messaging.Buses;

namespace Composable.Messaging
{
    public abstract partial class RemoteNavigationSpecification<TResult>
    {
        internal static class Remote
        {
            internal class StartQuery : RemoteNavigationSpecification<TResult>
            {
                readonly IQuery<TResult> _start;

                internal StartQuery(IQuery<TResult> start) => _start = start;

                public override TResult ExecuteRemoteOn(IRemoteServiceBusSession busSession) => busSession.GetRemote(_start);
                public override Task<TResult> ExecuteRemoteAsyncOn(IRemoteServiceBusSession busSession) => busSession.GetRemoteAsync(_start);
            }

            internal class StartCommand : RemoteNavigationSpecification<TResult>
            {
                readonly IExactlyOnceCommand<TResult> _start;

                internal StartCommand(IExactlyOnceCommand<TResult> start) => _start = start;

                public override TResult ExecuteRemoteOn(IRemoteServiceBusSession busSession) => busSession.PostRemote(_start);
                public override Task<TResult> ExecuteRemoteAsyncOn(IRemoteServiceBusSession busSession) => busSession.PostRemoteAsync(_start);
            }

            internal class ContinuationQuery<TPrevious> : RemoteNavigationSpecification<TResult>
            {
                readonly RemoteNavigationSpecification<TPrevious> _previous;
                readonly Func<TPrevious, IQuery<TResult>> _nextQuery;

                internal ContinuationQuery(RemoteNavigationSpecification<TPrevious> previous, Func<TPrevious, IQuery<TResult>> nextQuery)
                {
                    _previous = previous;
                    _nextQuery = nextQuery;
                }

                public override TResult ExecuteRemoteOn(IRemoteServiceBusSession busSession)
                {
                    var previousResult = _previous.ExecuteRemoteOn(busSession);
                    var currentQuery = _nextQuery(previousResult);
                    return busSession.GetRemote(currentQuery);
                }

                public override async Task<TResult> ExecuteRemoteAsyncOn(IRemoteServiceBusSession busSession)
                {
                    var previousResult = await _previous.ExecuteRemoteAsyncOn(busSession);
                    var currentQuery = _nextQuery(previousResult);
                    return await busSession.GetRemoteAsync(currentQuery);
                }
            }

            internal class PostCommand<TPrevious> : RemoteNavigationSpecification<TResult>
            {
                readonly RemoteNavigationSpecification<TPrevious> _previous;
                readonly Func<TPrevious, IExactlyOnceCommand<TResult>> _next;
                internal PostCommand(RemoteNavigationSpecification<TPrevious> previous, Func<TPrevious, IExactlyOnceCommand<TResult>> next)
                {
                    _previous = previous;
                    _next = next;
                }

                public override TResult ExecuteRemoteOn(IRemoteServiceBusSession busSession)
                {
                    var previousResult = _previous.ExecuteRemoteOn(busSession);
                    var currentCommand = _next(previousResult);
                    return busSession.PostRemote(currentCommand);
                }

                public override async Task<TResult> ExecuteRemoteAsyncOn(IRemoteServiceBusSession busSession)
                {
                    var previousResult = await _previous.ExecuteRemoteAsyncOn(busSession);
                    var currentCommand = _next(previousResult);
                    return await busSession.PostRemoteAsync(currentCommand);
                }
            }

            internal class PostVoidCommand<TPrevious> : RemoteNavigationSpecification
            {
                readonly RemoteNavigationSpecification<TPrevious> _previous;
                readonly Func<TPrevious, IExactlyOnceCommand> _next;
                internal PostVoidCommand(RemoteNavigationSpecification<TPrevious> previous, Func<TPrevious, IExactlyOnceCommand> next)
                {
                    _previous = previous;
                    _next = next;
                }

                public override void ExecuteRemoteOn(IRemoteServiceBusSession busSession)
                {
                    var previousResult = _previous.ExecuteRemoteOn(busSession);
                    var currentCommand = _next(previousResult);
                    busSession.PostRemote(currentCommand);
                }

                public override async Task ExecuteRemoteAsyncOn(IRemoteServiceBusSession busSession)
                {
                    var previousResult = await _previous.ExecuteRemoteAsyncOn(busSession);
                    var currentCommand = _next(previousResult);
                    busSession.PostRemote(currentCommand);
                }
            }
        }
    }
}