using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Events;

namespace CQRS.Core.Domain
{
    //to interact with event db
    // it won't have any update or delete
    public interface IEventStoreRepository
    {
        Task SaveAsync(EventModel @event);
        Task<List<EventModel>> FindByAggregateId(Guid aggregateId);// used  to retrieve all of events from event store
        Task<List<EventModel>> FindAllAsync(); // for restore read DB
    }
}