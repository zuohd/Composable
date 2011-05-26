﻿using System;
using System.Collections.Generic;
using System.Linq;
using Composable.System.Linq;

namespace Composable.CQRS.EventSourcing
{
    public abstract class EventStoreSession : IEventStoreSession
    {
        protected IDictionary<Guid, IEventStored> _idMap = new Dictionary<Guid, IEventStored>();

        protected abstract IEnumerable<IAggregateRootEvent> GetHistoryUnSafe(Guid aggregateId);
        private IEnumerable<IAggregateRootEvent> GetHistory(Guid aggregateId)
        {
            var history = GetHistoryUnSafe(aggregateId);
            if (history.None())
            {
                throw new Exception(string.Format("Aggregate root with Id: {0} not found", aggregateId));
            }
            return history;
        }

        public TAggregate Load<TAggregate>(Guid aggregateId) where TAggregate : IEventStored
        {
            IEventStored existing;
            if(_idMap.TryGetValue(aggregateId, out existing))
            {
                return (TAggregate)existing;
            }
            
            var aggregate = Activator.CreateInstance<TAggregate>();
            aggregate.LoadFromHistory(GetHistory(aggregateId));
            _idMap.Add(aggregateId, aggregate);
            return aggregate;
        }

        public TAggregate LoadSpecificVersion<TAggregate>(Guid aggregateId, int version) where TAggregate : IEventStored
        {
            var aggregate = Activator.CreateInstance<TAggregate>();
            aggregate.LoadFromHistory(GetHistory(aggregateId).Where(e => e.AggregateRootVersion <= version));
            return aggregate;
        }        

        public void Save<TAggregate>(TAggregate aggregate) where TAggregate : IEventStored
        {
            var changes = aggregate.GetChanges();
            if (aggregate.Version > 0 && changes.None() || changes.Any() && changes.Min(e => e.AggregateRootVersion) > 1)
            {
                throw new AttemptToSaveAlreadyPersistedAggregateException(aggregate);
            }
            _idMap.Add(aggregate.Id, aggregate);
        }

        public void SaveChanges()
        {
            SaveEvents(_idMap.SelectMany(p => p.Value.GetChanges()));
            _idMap.Select(p => p.Value).ForEach(p => p.AcceptChanges());
        }

        protected abstract void SaveEvents(IEnumerable<IAggregateRootEvent> events);
        public abstract void Dispose();
    }
}