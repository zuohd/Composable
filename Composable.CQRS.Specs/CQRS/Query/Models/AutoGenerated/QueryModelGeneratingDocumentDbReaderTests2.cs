﻿using System;
using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Composable.CQRS.CQRS.EventSourcing;
using Composable.CQRS.CQRS.Query.Models.Generators;
using Composable.CQRS.EventSourcing;
using Composable.CQRS.Specs.CQRS.Query.Models.AutoGenerated.Domain;
using Composable.CQRS.Specs.CQRS.Query.Models.AutoGenerated.Domain.Events;
using Composable.CQRS.Specs.CQRS.Query.Models.AutoGenerated.Domain.Events.Implementation;
using Composable.CQRS.Specs.CQRS.Query.Models.AutoGenerated.Domain.Events.PropertyUpdated;
using Composable.CQRS.Specs.CQRS.Query.Models.AutoGenerated.Domain.UI.QueryModels;
using Composable.CQRS.Windsor;
using Composable.GenericAbstractions.Time;
using Composable.Messaging.Buses;
using Composable.Messaging.Events;
using Composable.Persistence.KeyValueStorage;
using Composable.SystemExtensions.Threading;
using Composable.UnitsOfWork;
using FluentAssertions;
using NUnit.Framework;

namespace Composable.CQRS.Specs.CQRS.Query.Models.AutoGenerated
{
    [TestFixture]
    public class QueryModelGeneratingDocumentDbReaderTests2
    {
        WindsorContainer _container;

        [SetUp]
        public void CreateContainer()
        {
            _container = new WindsorContainer();

            _container.Kernel.Resolver.AddSubResolver(new CollectionResolver(_container.Kernel));

            _container.Register(
                Component.For<IUtcTimeTimeSource,DummyTimeSource>()
                    .Instance(DummyTimeSource.Now)
                    .LifestyleSingleton(),
                Component.For<ISingleContextUseGuard>().ImplementedBy<SingleThreadUseGuard>()
                    .LifestyleScoped(),
                Component.For<IWindsorContainer>().Instance(_container),
                Component.For<IMessageHandlerRegistry, IMessageHandlerRegistrar>().ImplementedBy<MessageHandlerRegistry>().LifestyleSingleton(),
                Component.For<IServiceBus>().ImplementedBy<TestingOnlyServiceBus>(),
                Component.For<IEventStore>().ImplementedBy<InMemoryEventStore>(),
                Component.For<IEventStoreSession, IEventStoreReader, IUnitOfWorkParticipant>().ImplementedBy<EventStoreSession>()
                    .LifestyleScoped(),
                Component.For<IDocumentDbSessionInterceptor>().Instance(NullOpDocumentDbSessionInterceptor.Instance),
                Component.For<IDocumentDbReader, IVersioningDocumentDbReader>().ImplementedBy<QueryModelGeneratingDocumentDbReader>()
                    .LifestyleScoped(),
                Component.For<IQueryModelGenerator, IQueryModelGenerator<MyAccountQueryModel>>().ImplementedBy<AccountQueryModelGenerator>()
                    .LifestyleScoped()
                );
        }

        [Test]
        public void ThrowsExceptionIfInstanceDoesNotExist()
        {
            using(_container.BeginScope())
            {
                var reader = _container.Resolve<IDocumentDbReader>();
                reader.Invoking(me => me.Get<MyAccountQueryModel>(Guid.NewGuid()))
                    .ShouldThrow<Exception>();
            }
        }

        [Test]
        public void CanFetchQueryModelAfterAggregateHasBeenCreated()
        {
            using(_container.BeginScope())
            {
                var aggregates = _container.Resolve<IEventStoreSession>();
                var accountId = Guid.Parse("00000000-0000-0000-0000-000000000001");

                MyAccount registered;
                using(var transaction = _container.BeginTransactionalUnitOfWorkScope())
                {
                    registered = MyAccount.Register(aggregates, accountId, "email", "password");
                    transaction.Commit();
                }


                registered.Email.Should().Be("email");
                registered.Password.Should().Be("password");


                var reader = _container.Resolve<IDocumentDbReader>();
                var loadedModel = reader.Get<MyAccountQueryModel>(registered.Id);

                loadedModel.Should().NotBe(null);
                loadedModel.Id.Should().Be(accountId);
                loadedModel.Email.Should().Be(registered.Email);
                loadedModel.Password.Should().Be(registered.Password);
            }
        }

        [Test]
        public void ThrowsExceptionWhenTryingToFetchDeletedEntity()
        {
            using (_container.BeginScope())
            {
                var aggregates = _container.Resolve<IEventStoreSession>();
                var accountId = Guid.Parse("00000000-0000-0000-0000-000000000001");

                MyAccount registered;
                using (var transaction = _container.BeginTransactionalUnitOfWorkScope())
                {
                    registered = MyAccount.Register(aggregates, accountId, "email", "password");
                    transaction.Commit();
                }

                var reader = _container.Resolve<IDocumentDbReader>();
                reader.Get<MyAccountQueryModel>(registered.Id);//Here it exists

                using (var transaction = _container.BeginTransactionalUnitOfWorkScope())
                {
                    registered.Delete();
                    transaction.Commit();
                }

                using(_container.BeginScope())
                {
                    var reader2 = _container.Resolve<IDocumentDbReader>();
                    reader2.Invoking(me => me.Get<MyAccountQueryModel>(registered.Id))
                        .ShouldThrow<Exception>();
                }
            }
        }

        [Test]
        public void ReturnsUpdatedDataAfterTransactionHasCommitted()
        {
            using(_container.BeginScope())
            {
                var accountId = Guid.Parse("00000000-0000-0000-0000-000000000001");

                MyAccount registered;

                var aggregates = _container.Resolve<IEventStoreSession>();
                using(var transaction = _container.BeginTransactionalUnitOfWorkScope())
                {
                    registered = MyAccount.Register(aggregates, accountId, "email", "password");
                    transaction.Commit();
                }

                _container.Resolve<IDocumentDbReader>()
                    .Get<MyAccountQueryModel>(registered.Id); //Make sure we read it once so caches etc get involved.

                using(var transaction = _container.BeginTransactionalUnitOfWorkScope()) //Update it.
                {
                    registered.ChangeEmail("newEmail");
                    transaction.Commit();
                }

                using(_container.BeginScope())
                {
                    var loadedModel = _container.Resolve<IDocumentDbReader>()
                        .Get<MyAccountQueryModel>(registered.Id);

                    loadedModel.Should().NotBe(null);
                    loadedModel.Email.Should().Be("newEmail");
                }
            }
        }

        [Test]
        public void CanReturnPreviousVersionsOfQueryModel()
        {
            using (_container.BeginScope())
            {
                var accountId = Guid.Parse("00000000-0000-0000-0000-000000000001");

                MyAccount registered;

                var aggregates = _container.Resolve<IEventStoreSession>();
                using (var transaction = _container.BeginTransactionalUnitOfWorkScope())
                {
                    registered = MyAccount.Register(aggregates, accountId, "originalEmail", "password");
                    transaction.Commit();
                }

                _container.Resolve<IVersioningDocumentDbReader>()
                    .GetVersion<MyAccountQueryModel>(registered.Id, registered.Version); //Make sure we read it once so caches etc get involved.

                using (var transaction = _container.BeginTransactionalUnitOfWorkScope()) //Update it.
                {
                    registered.ChangeEmail("newEmail1");
                    registered.ChangeEmail("newEmail2");
                    registered.ChangeEmail("newEmail3");
                    transaction.Commit();
                }

                using (_container.BeginScope())
                {
                    var loadedModel = _container.Resolve<IVersioningDocumentDbReader>()
                        .Get<MyAccountQueryModel>(registered.Id);

                    loadedModel.Should().NotBe(null);
                    loadedModel.Email.Should().Be("newEmail3");

                    _container.Resolve<IVersioningDocumentDbReader>()
                        .GetVersion<MyAccountQueryModel>(registered.Id, registered.Version -1)
                        .Email.Should().Be("newEmail2");

                    _container.Resolve<IVersioningDocumentDbReader>()
                        .GetVersion<MyAccountQueryModel>(registered.Id, registered.Version - 2)
                        .Email.Should().Be("newEmail1");

                    _container.Resolve<IVersioningDocumentDbReader>()
                        .GetVersion<MyAccountQueryModel>(registered.Id, registered.Version - 3)
                        .Email.Should().Be("originalEmail");
                }
            }
        }
    }

    namespace Domain
    {
        namespace UI
        {
            namespace QueryModels
            {
                public class MyAccountQueryModel : ISingleAggregateQueryModel
                {
                    public Guid Id { get; private set; }
                    internal string Email { get; set; }
                    internal string Password { get; set; }

                    public void SetId(Guid id)
                    {
                        Id = id;
                    }
                }

                public class AccountQueryModelGenerator : SingleAggregateQueryModelGenerator<AccountQueryModelGenerator, MyAccountQueryModel, IAccountEvent, IEventStoreReader>
                {
                    public AccountQueryModelGenerator(IEventStoreReader session) : base(session)
                    {
                        RegisterHandlers()
                            .For<IAccountEmailPropertyUpdatedEvent>(e => Model.Email = e.Email)
                            .For<IAccountPasswordPropertyUpdatedEvent>(e => Model.Password = e.Password);
                    }
                }
            }
        }


        class MyAccount : AggregateRoot<MyAccount, AccountEvent, IAccountEvent>
        {
            MyAccount():base(new DateTimeNowTimeSource())
            {
                RegisterEventAppliers()
                    .For<IAccountEmailPropertyUpdatedEvent>(e => Email = e.Email)
                    .For<IAccountPasswordPropertyUpdatedEvent>(e => Password = e.Password)
                    .For<IAccountDeletedEvent>(e => { });
            }

            public string Email { get; private set; }
            public string Password { get; private set; }

            public void ChangeEmail(string newEmail)
            {
                RaiseEvent(new EmailChangedEvent(newEmail));
            }

            public static MyAccount Register(IEventStoreSession aggregates, Guid accountId, string email, string password)
            {
                var registered = new MyAccount();
                registered.RaiseEvent(new AccountRegisteredEvent(accountId, email, password));
                aggregates.Save(registered);
                return registered;
            }

            public void Delete()
            {
                RaiseEvent(new AccountDeletedEvent());
            }
        }

        namespace Events
        {
            public interface IAccountEvent : IAggregateRootEvent {}

            abstract class AccountEvent : AggregateRootEvent, IAccountEvent
            {
                protected AccountEvent() {}
                protected AccountEvent(Guid aggregateRootId) : base(aggregateRootId) {}
            }

            interface IAccountRegisteredEvent
                : IAggregateRootCreatedEvent,
                    IAccountEmailPropertyUpdatedEvent,
                    IAccountPasswordPropertyUpdatedEvent {}

            interface IEmailChangedEvent : IAccountEvent,
                IAccountEmailPropertyUpdatedEvent {}

            interface IAccountDeletedEvent : IAccountEvent,
                IAggregateRootDeletedEvent
            {

            }

            namespace PropertyUpdated
            {
                interface IAccountEmailPropertyUpdatedEvent : IAccountEvent
                {
                    string Email { get; }
                }

                interface IAccountPasswordPropertyUpdatedEvent : IAccountEvent
                {
                    string Password { get; }
                }
            }

            namespace Implementation
            {
                class AccountRegisteredEvent : AccountEvent, IAccountRegisteredEvent
                {
                    public AccountRegisteredEvent(Guid accountId, String email, string password) : base(accountId)
                    {
                        Email = email;
                        Password = password;
                    }

                    public string Email { get; private set; }
                    public string Password { get; private set; }
                }

                class EmailChangedEvent : AccountEvent, IEmailChangedEvent
                {
                    public EmailChangedEvent(string newEmail)
                    {
                        Email = newEmail;
                    }

                    public string Email { get; private set; }
                }

                class AccountDeletedEvent : AccountEvent, IAccountDeletedEvent
                {

                }
            }
        }
    }
}
