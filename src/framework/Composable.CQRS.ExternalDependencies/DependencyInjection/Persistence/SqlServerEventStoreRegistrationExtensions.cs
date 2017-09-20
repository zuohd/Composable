using System;
using System.Collections.Generic;
using Castle.DynamicProxy;
using Composable.Contracts;
using Composable.GenericAbstractions.Time;
using Composable.Messaging.Buses;
using Composable.Persistence.EventStore;
using Composable.Persistence.EventStore.MicrosoftSQLServer;
using Composable.Persistence.EventStore.Refactoring.Migrations;
using Composable.Persistence.EventStore.Refactoring.Naming;
using Composable.System.Configuration;
using Composable.System.Linq;
using Composable.SystemExtensions.Threading;
using JetBrains.Annotations;

// ReSharper disable UnusedTypeParameter the type parameters allow non-ambiguous registrations in the container. They are in fact used.

namespace Composable.DependencyInjection.Persistence
{
    interface IEventStore<TSessionInterface, TReaderInterface> : IEventStore {}

    public static class SqlServerEventStoreRegistrationExtensions
    {
        interface IEventStoreUpdater<TSessionInterface, TReaderInterface> : IEventStoreUpdater {}

        class EventStore<TSessionInterface, TReaderInterface> : EventStore, IEventStore<TSessionInterface, TReaderInterface>
        {
            public EventStore(IEventStoreEventSerializer serializer,
                              IEventstorePersistenceLayer persistenceLayer,
                              ISingleContextUseGuard usageGuard,
                              EventCache<TSessionInterface> cache,
                              IEnumerable<IEventMigration> migrations) : base(persistenceLayer, serializer, usageGuard, cache, migrations:migrations) {}
        }

        class InMemoryEventStore<TSessionInterface, TReaderInterface> : InMemoryEventStore, IEventStore<TSessionInterface, TReaderInterface>
        {
            public InMemoryEventStore(IEnumerable<IEventMigration> migrations = null) : base(migrations) {}
        }

        [UsedImplicitly] class EventStoreUpdater<TSessionInterface, TReaderInterface> : EventStoreUpdater, IEventStoreUpdater<TSessionInterface, TReaderInterface>
        {
            public EventStoreUpdater(IInProcessServiceBus bus,
                                     IEventStore<TSessionInterface, TReaderInterface> store,
                                     ISingleContextUseGuard usageGuard,
                                     IUtcTimeTimeSource timeSource) : base(bus, store, usageGuard, timeSource) {}
        }

        interface IEventstorePersistenceLayer<TUpdater> : IEventstorePersistenceLayer
        {
        }

        class EventstorePersistenceLayer<TUpdaterType> : IEventstorePersistenceLayer<TUpdaterType>
        {
            public EventstorePersistenceLayer(IEventStoreSchemaManager schemaManager, IEventStoreEventReader eventReader, IEventStoreEventWriter eventWriter)
            {
                SchemaManager = schemaManager;
                EventReader = eventReader;
                EventWriter = eventWriter;
            }
            public IEventStoreSchemaManager SchemaManager { get; }
            public IEventStoreEventReader EventReader { get; }
            public IEventStoreEventWriter EventWriter { get; }
        }

        [UsedImplicitly] class EventCache<TUpdaterType> : EventCache
        {}

        public static void RegisterSqlServerEventStore<TSessionInterface, TReaderInterface>(this IDependencyInjectionContainer @this,
                                                                                            string connectionName,
                                                                                            IReadOnlyList<IEventMigration> migrations = null)
            where TSessionInterface : class, IEventStoreUpdater
            where TReaderInterface : IEventStoreReader
            => @this.RegisterSqlServerEventStoreForFlexibleTesting<TSessionInterface, TReaderInterface>(
                @this.RunMode.Mode,
                connectionName,
                migrations != null
                    ? (Func<IReadOnlyList<IEventMigration>>)(() => migrations)
                    : (() => EmptyMigrationsArray));

        static readonly IEventMigration[] EmptyMigrationsArray = new IEventMigration[0];
        internal static void RegisterSqlServerEventStoreForFlexibleTesting<TSessionInterface, TReaderInterface>(this IDependencyInjectionContainer @this,
                                                                                                                TestingMode mode,
                                                                                                                string connectionName,
                                                                                                                Func<IReadOnlyList<IEventMigration>> migrations)
            where TSessionInterface : class, IEventStoreUpdater
            where TReaderInterface : IEventStoreReader
        {
            Contract.Argument(() => connectionName)
                    .NotNullEmptyOrWhiteSpace();
            migrations = migrations ?? (() => EmptyMigrationsArray);

            GeneratedLowLevelInterfaceInspector.InspectInterfaces(Seq.OfTypes<TSessionInterface, TReaderInterface>());


            @this.Register(Component.For<EventCache<TSessionInterface>>()
                                    .ImplementedBy<EventCache<TSessionInterface>>()
                                    .LifestyleSingleton());

            if (@this.RunMode.IsTesting && mode == TestingMode.InMemory)
            {
                @this.Register(Component.For<InMemoryEventStore<TSessionInterface, TReaderInterface>>()
                                        .UsingFactoryMethod(sl => new InMemoryEventStore<TSessionInterface, TReaderInterface>(migrations: migrations()))
                                        .LifestyleSingleton()
                                        .DelegateToParentServiceLocatorWhenCloning());

                @this.Register(Component.For<IEventStore<TSessionInterface, TReaderInterface>>()
                                        .UsingFactoryMethod(sl =>
                                                            {
                                                                var store = sl.Resolve<InMemoryEventStore<TSessionInterface, TReaderInterface>>();
                                                                store.TestingOnlyReplaceMigrations(migrations());
                                                                return store;
                                                            })
                                        .LifestyleScoped());
            } else
            {
                @this.Register(
                    Component.For<IEventstorePersistenceLayer<TSessionInterface>>()
                                .UsingFactoryMethod(sl =>
                                                    {
                                                        var connectionProvider = sl.Resolve<ISqlConnectionProvider>()
                                                                                                            .GetConnectionProvider(connectionName);

                                                        IEventNameMapper nameMapper = new DefaultEventNameMapper();
                                                        var connectionManager = new SqlServerEventStoreConnectionManager(connectionProvider);
                                                        var schemaManager = new SqlServerEventStoreSchemaManager(connectionProvider, nameMapper);
                                                        var eventReader = new SqlServerEventStoreEventReader(connectionManager, schemaManager);
                                                        var eventWriter = new SqlServerEventStoreEventWriter(connectionManager, schemaManager);
                                                        return new EventstorePersistenceLayer<TSessionInterface>(schemaManager, eventReader, eventWriter);
                                                    })
                                .LifestyleScoped());


                @this.Register(Component.For<IEventStore<TSessionInterface, TReaderInterface>>()
                                        .UsingFactoryMethod(sl => new EventStore<TSessionInterface, TReaderInterface>(
                                                                persistenceLayer: sl.Resolve<IEventstorePersistenceLayer<TSessionInterface>>(),
                                                                serializer: sl.Resolve<IEventStoreEventSerializer>(),
                                                                migrations: migrations(),
                                                                cache: sl.Resolve<EventCache<TSessionInterface>>(),
                                                                usageGuard: new SingleThreadUseGuard()))
                                        .LifestyleScoped());
            }

            @this.Register(Component.For<IEventStoreUpdater<TSessionInterface, TReaderInterface>>()
                                    .ImplementedBy<EventStoreUpdater<TSessionInterface, TReaderInterface>>()
                                    .LifestyleScoped());

            @this.Register(Component.For<TSessionInterface>(Seq.OfTypes<TReaderInterface>())
                                    .UsingFactoryMethod(locator => CreateProxyFor<TSessionInterface, TReaderInterface>(locator.Resolve<IEventStoreUpdater<TSessionInterface, TReaderInterface>>()))
                                    .LifestyleScoped());
        }

        static TSessionInterface CreateProxyFor<TSessionInterface, TReaderInterface>(IEventStoreUpdater updater)
            where TSessionInterface : IEventStoreUpdater
            where TReaderInterface : IEventStoreReader
        {
            var sessionType = EventStoreSessionProxyFactory<TSessionInterface, TReaderInterface>.ProxyType;
            return (TSessionInterface)Activator.CreateInstance(sessionType, new IInterceptor[] {}, updater);
        }

        //Using a generic class this way allows us to bypass any need for dictionary lookups or similar giving us excellent performance.
        static class EventStoreSessionProxyFactory<TSessionInterface, TReaderInterface>
            where TSessionInterface : IEventStoreUpdater
            where TReaderInterface : IEventStoreReader
        {
            internal static readonly Type ProxyType = new DefaultProxyBuilder().CreateInterfaceProxyTypeWithTargetInterface(
                interfaceToProxy: typeof(IEventStoreUpdater),
                additionalInterfacesToProxy: new[]
                                             {
                                                 typeof(TSessionInterface),
                                                 typeof(TReaderInterface)
                                             },
                options: ProxyGenerationOptions.Default);
        }
    }
}