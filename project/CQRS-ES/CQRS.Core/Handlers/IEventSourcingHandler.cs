using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;

namespace CQRS.Core.Handlers
{
    //it will provide interafce abstraction through which the command handler will obtain the latest state of the aggregate 
    //and through which it will persist the uncommitted changes on the aggregate as events to the event store
    public interface IEventSourcingHandler<T>
    {
        Task SaveAsync(AggregateRoot aggregate);
        Task<T> GetByIdAsync(Guid aggregateId);

        Task RepublishEventsAsync();
    }
}