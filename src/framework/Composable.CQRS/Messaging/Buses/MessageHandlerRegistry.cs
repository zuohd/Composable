using System;
using System.Collections.Generic;
using System.Linq;
using Composable.Messaging.Events;
using Composable.System.Collections.Collections;

namespace Composable.Messaging.Buses
{
    class MessageHandlerRegistry : IMessageHandlerRegistrar, IMessageHandlerRegistry
    {
        readonly Dictionary<Type, Action<object>> _commandHandlers = new Dictionary<Type, Action<object>>();
        readonly Dictionary<Type, List<Action<IEvent>>> _eventHandlers = new Dictionary<Type, List<Action<IEvent>>>();
        readonly Dictionary<Type, Func<object, object>> _queryHandlers = new Dictionary<Type, Func<object, object>>();
        readonly Dictionary<Type, Func<object, object>> _commandHandlersReturningResults = new Dictionary<Type, Func<object, object>>();
        readonly List<EventHandlerRegistration> _eventHandlerRegistrations = new List<EventHandlerRegistration>();

        readonly object _lock = new object();

        IMessageHandlerRegistrar IMessageHandlerRegistrar.ForEvent<TEvent>(Action<TEvent> handler)
        {
            lock(_lock)
            {
                _eventHandlers.GetOrAdd(typeof(TEvent), () => new List<Action<IEvent>>()).Add(@event => handler((TEvent)@event));
                _eventHandlerRegistrations.Add(new EventHandlerRegistration(typeof(TEvent), registrar => registrar.For(handler)));
                return this;
            }
        }

        IMessageHandlerRegistrar IMessageHandlerRegistrar.ForCommand<TCommand>(Action<TCommand> handler)
        {
            lock(_lock)
            {
                _commandHandlers.Add(typeof(TCommand), command => handler((TCommand)command));
                return this;
            }
        }

        public IMessageHandlerRegistrar ForCommand<TCommand, TResult>(Func<TCommand, TResult> handler) where TCommand : ICommand<TResult>
                                                                                      where TResult : IMessage
        {
            lock (_lock)
            {
                _commandHandlersReturningResults.Add(typeof(TCommand), command => handler((TCommand)command));
                return this;
            }
        }

        IMessageHandlerRegistrar IMessageHandlerRegistrar.ForQuery<TQuery, TResult>(Func<TQuery, TResult> handler)
        {
            lock(_lock)
            {
                _queryHandlers.Add(typeof(TQuery), query => handler((TQuery)query));
                return this;
            }
        }



        Action<object> IMessageHandlerRegistry.GetCommandHandler(ICommand message)
        {
            try
            {
                lock(_lock)
                {
                    return _commandHandlers[message.GetType()];
                }
            }
            catch(KeyNotFoundException)
            {
                throw new NoHandlerException(message.GetType());
            }
        }

        public Func<ICommand, object> GetCommandHandler(Type commandType)
        {
            if(_commandHandlers.TryGetValue(commandType, out Action<object> handler))
            {
                return (command) =>
                {
                    handler(command);
                    return null;
                };
            }

            return _commandHandlersReturningResults[commandType];
        }

        public Func<IQuery, object> GetQueryHandler(Type queryType) => _queryHandlers[queryType];

        public IReadOnlyList<Action<IEvent>> GetEventHandlers(Type eventType)
        {
            return _eventHandlers.Where(@this => @this.Key.IsAssignableFrom(eventType)).SelectMany(@this => @this.Value).ToList();
        }

        Func<IQuery<TResult>, TResult> IMessageHandlerRegistry.GetQueryHandler<TResult>(IQuery<TResult> query)
        {
            try
            {
                lock(_lock)
                {
                    var typeUnsafeQuery = _queryHandlers[query.GetType()];
                    return actualQuery => (TResult)typeUnsafeQuery(actualQuery);
                }
            }
            catch(KeyNotFoundException)
            {
                throw new NoHandlerException(query.GetType());
            }
        }

        public Func<ICommand<TResult>, TResult> GetCommandHandler<TResult>(ICommand<TResult> command) where TResult : IMessage
        {
            try
            {
                lock (_lock)
                {
                    var typeUnsafeHandler = _commandHandlersReturningResults[command.GetType()];
                    return actualCommand => (TResult)typeUnsafeHandler(command);
                }
            }
            catch (KeyNotFoundException)
            {
                throw new NoHandlerException(command.GetType());
            }
        }


        IEventDispatcher<IEvent> IMessageHandlerRegistry.CreateEventDispatcher()
        {
            var dispatcher = new CallMatchingHandlersInRegistrationOrderEventDispatcher<IEvent>();
            var registrar = dispatcher.RegisterHandlers()
                                      .IgnoreUnhandled<IEvent>();
            lock(_lock)
            {
                _eventHandlerRegistrations.ForEach(handlerRegistration => handlerRegistration.RegisterHandlerWithRegistrar(registrar));
            }

            return dispatcher;
        }

        public IEnumerable<Type> HandledTypes()
        {
            return _commandHandlers.Keys
                .Concat(_commandHandlersReturningResults.Keys).
                Concat(_queryHandlers.Keys)
                .Concat(_eventHandlerRegistrations.Select(reg => reg.Type));
        }

        internal class EventHandlerRegistration
        {
            public Type Type { get; }
            public Action<IEventHandlerRegistrar<IEvent>> RegisterHandlerWithRegistrar { get; }
            public EventHandlerRegistration(Type type, Action<IEventHandlerRegistrar<IEvent>> registerHandlerWithRegistrar)
            {
                Type = type;
                RegisterHandlerWithRegistrar = registerHandlerWithRegistrar;
            }
        }
    }
}
