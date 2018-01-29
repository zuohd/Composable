﻿using System;
using Composable.DependencyInjection;
using Composable.DependencyInjection.Persistence;
using Composable.GenericAbstractions.Time;
using Composable.Messaging;
using Composable.Messaging.Buses;
using Composable.Persistence.EventStore;
using Composable.Persistence.EventStore.Aggregates;
using Composable.Testing.Threading;
using FluentAssertions;

namespace Composable.Tests.Messaging.ServiceBusSpecification.Given_a_backend_endpoint_with_a_command_event_and_query_handler
{
    public class Fixture : IDisposable
    {
        internal readonly ITestingEndpointHost Host;
        internal readonly IThreadGate CommandHandlerThreadGate = ThreadGate.CreateOpenWithTimeout(1.Seconds());
        internal readonly IThreadGate CommandHandlerWithResultThreadGate = ThreadGate.CreateOpenWithTimeout(1.Seconds());
        internal readonly IThreadGate MyModifyAgregateCommandHandlerThreadGate = ThreadGate.CreateOpenWithTimeout(1.Seconds());
        internal readonly IThreadGate EventHandlerThreadGate = ThreadGate.CreateOpenWithTimeout(1.Seconds());
        internal readonly IThreadGate QueryHandlerThreadGate = ThreadGate.CreateOpenWithTimeout(5.Seconds());

        protected readonly TestingTaskRunner TaskRunner = TestingTaskRunner.WithTimeout(1.Seconds());
        protected readonly IEndpoint DomainEndpoint;
        protected readonly IEndpoint ClientEndpoint;

        protected Fixture()
        {
            Host = EndpointHost.Testing.CreateHost(DependencyInjectionContainer.Create);
            DomainEndpoint = Host.RegisterAndStartEndpoint(
                "Backend",
                new EndpointId(Guid.Parse("DDD0A67C-D2A2-4197-9AF8-38B6AEDF8FA6")),
                builder =>
                {
                    builder.Container.RegisterSqlServerEventStore("Backend");
                    builder.RegisterHandlers
                           .ForCommand((MyExactlyOnceCommand command) => CommandHandlerThreadGate.AwaitPassthrough())
                           .ForCommand((MyModifyAggregateCommand command, ILocalApiNavigatorSession navigator) =>
                            {
                                MyModifyAgregateCommandHandlerThreadGate.AwaitPassthrough();
                            })
                           .ForEvent((MyExactlyOnceEvent myEvent) => EventHandlerThreadGate.AwaitPassthrough())
                           .ForQuery((MyQuery query) => QueryHandlerThreadGate.AwaitPassthroughAndReturn(new MyQueryResult()))
                           .ForCommandWithResult((MyAtMostOnceCommandWithResult command) => CommandHandlerWithResultThreadGate.AwaitPassthroughAndReturn(new MyCommandResult()));

                    builder.TypeMapper.Map<MyExactlyOnceCommand>("0ddefcaa-4d4d-48b2-9e1a-762c0b835275")
                           .Map<MyAtMostOnceCommandWithResult>("24248d03-630b-4909-a6ea-e7fdaf82baa2")
                           .Map<MyExactlyOnceEvent>("2fdde21f-c6d4-46a2-95e5-3429b820dfc3")
                           .Map<MyQuery>("b9d62f22-514b-4e3c-9ac1-66940a7a8144")
                           .Map<MyModifyAggregateCommand>("86bf04d8-8e6d-4e21-a95e-8af237f69f0f");
                });

            ClientEndpoint = Host.ClientEndpoint;
        }

        public virtual void Dispose()
        {
            OpenGates();
            TaskRunner.Dispose();
            Host.Dispose();
        }

        protected void CloseGates()
        {
            EventHandlerThreadGate.Close();
            CommandHandlerThreadGate.Close();
            CommandHandlerWithResultThreadGate.Close();
            QueryHandlerThreadGate.Close();
        }

        protected void OpenGates()
        {
            EventHandlerThreadGate.Open();
            CommandHandlerThreadGate.Open();
            CommandHandlerWithResultThreadGate.Open();
            QueryHandlerThreadGate.Open();
        }

        protected class MyAggregateEvent
        {
            public interface IRoot : IAggregateEvent{}
            public interface Created : IRoot{}
            public class Implementation
            {
                public class Root : AggregateEvent, IRoot
                {
                    protected Root() {}
                    protected Root(Guid aggregateId) : base(aggregateId) {}
                }
            }
        }

        protected class MyAggregate : Aggregate<MyAggregate, MyAggregateEvent.Implementation.Root, MyAggregateEvent.IRoot>
        {
            public MyAggregate():base(new DateTimeNowTimeSource()) {}
        }



        protected class MyModifyAggregateCommand : BusApi.Remotable.ICommand{}
        protected class MyExactlyOnceCommand : BusApi.Remotable.ExactlyOnce.Command {}
        protected class MyExactlyOnceEvent : AggregateEvent {}
        protected class MyQuery : BusApi.Remotable.NonTransactional.Queries.Query<MyQueryResult> {}
        protected class MyQueryResult {}
        protected class MyAtMostOnceCommand : BusApi.Remotable.AtMostOnce.Command<MyCommandResult> {}
        protected class MyAtMostOnceCommandWithResult : BusApi.Remotable.AtMostOnce.Command<MyCommandResult> {}
        protected class MyCommandResult {}
    }
}
