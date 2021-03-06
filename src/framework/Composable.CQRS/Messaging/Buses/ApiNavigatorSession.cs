﻿using System;
using System.Threading.Tasks;
using Composable.Messaging.Buses.Implementation;
using Composable.SystemExtensions.Threading;
using JetBrains.Annotations;

namespace Composable.Messaging.Buses
{
    //Todo: Build a pipeline to handle things like command validation, caching layers etc. Don't explicitly check for rules and optimization here with duplication across the class.
    [UsedImplicitly] class ApiNavigatorSession : IServiceBusSession, ILocalApiNavigatorSession
    {
        readonly IInterprocessTransport _transport;
        readonly CommandScheduler _commandScheduler;
        readonly IMessageHandlerRegistry _handlerRegistry;
        readonly IRemoteApiNavigatorSession _remoteNavigator;
        readonly ISingleContextUseGuard _contextGuard;

        public ApiNavigatorSession(IInterprocessTransport transport, CommandScheduler commandScheduler, IMessageHandlerRegistry handlerRegistry, IRemoteApiNavigatorSession remoteNavigator)
        {
            _contextGuard = new CombinationUsageGuard(new SingleTransactionUsageGuard());
            _transport = transport;
            _commandScheduler = commandScheduler;
            _handlerRegistry = handlerRegistry;
            _remoteNavigator = remoteNavigator;
        }

        void IIntegrationBusSession.Send(BusApi.Remotable.ExactlyOnce.ICommand command)
        {
            _contextGuard.AssertNoContextChangeOccurred(this);
            MessageInspector.AssertValidToSendRemote(command);
            CommandValidator.AssertCommandIsValid(command);
            _transport.DispatchIfTransactionCommits(command);
        }

        void IIntegrationBusSession.ScheduleSend(DateTime sendAt, BusApi.Remotable.ExactlyOnce.ICommand command)
        {
            MessageInspector.AssertValidToSendRemote(command);
            _contextGuard.AssertNoContextChangeOccurred(this);
            CommandValidator.AssertCommandIsValid(command);
            _commandScheduler.Schedule(sendAt, command);
        }

        TResult ILocalApiNavigatorSession.Execute<TResult>(BusApi.StrictlyLocal.ICommand<TResult> command)
        {
            MessageInspector.AssertValidToSendLocal(command);
            _contextGuard.AssertNoContextChangeOccurred(this);
            CommandValidator.AssertCommandIsValid(command);
            return _handlerRegistry.GetCommandHandler(command).Invoke(command);
        }

        void ILocalApiNavigatorSession.Execute(BusApi.StrictlyLocal.ICommand command)
        {
            MessageInspector.AssertValidToSendLocal(command);
            _contextGuard.AssertNoContextChangeOccurred(this);
            CommandValidator.AssertCommandIsValid(command);
            _handlerRegistry.GetCommandHandler(command).Invoke(command);
        }

        TResult ILocalApiNavigatorSession.Execute<TResult>(BusApi.StrictlyLocal.IQuery<TResult> query)
        {
            MessageInspector.AssertValidToSendLocal(query);
            _contextGuard.AssertNoContextChangeOccurred(this);
            // ReSharper disable once SuspiciousTypeConversion.Global
            //Todo: Test and stop disabling resharper warning
            return query is BusApi.ICreateMyOwnResultQuery<TResult> selfCreating
                       ? selfCreating.CreateResult()
                       : _handlerRegistry.GetQueryHandler(query).Invoke(query);
        }

        Task IRemoteApiNavigatorSession.PostAsync(BusApi.Remotable.AtMostOnce.ICommand command) => _remoteNavigator.PostAsync(command);
        void IRemoteApiNavigatorSession.Post(BusApi.Remotable.AtMostOnce.ICommand command) => _remoteNavigator.Post(command);
        Task<TResult> IRemoteApiNavigatorSession.PostAsync<TResult>(BusApi.Remotable.AtMostOnce.ICommand<TResult> command) => _remoteNavigator.PostAsync(command);
        TResult IRemoteApiNavigatorSession.Post<TResult>(BusApi.Remotable.AtMostOnce.ICommand<TResult> command) => _remoteNavigator.Post(command);
        Task<TResult> IRemoteApiNavigatorSession.GetAsync<TResult>(BusApi.Remotable.NonTransactional.IQuery<TResult> query) => _remoteNavigator.GetAsync(query);
        TResult IRemoteApiNavigatorSession.Get<TResult>(BusApi.Remotable.NonTransactional.IQuery<TResult> query) => _remoteNavigator.Get(query);
    }
}
