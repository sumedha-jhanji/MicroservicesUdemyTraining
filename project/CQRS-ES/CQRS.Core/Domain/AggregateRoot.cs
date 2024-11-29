using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using CQRS.Core.Commands;
using CQRS.Core.Events;

namespace CQRS.Core.Domain
{
    public abstract class AggregateRoot
    {
        protected Guid _id;
        private readonly List<BaseEvent> _changes = new(); // list of events
        public Guid Id { 
            get{
                return _id;
            }
        }
        public int Version{ get; set; } = -1;

        // get list of uncommitted changes
        public IEnumerable<BaseEvent> GetUncommittedChanges(){
            return _changes;
        }

        //mock changes as committed. In this case we nee to clear te list of events
        public void MockChangesAsCommitted(){
            _changes.Clear();
        }

        // will actually apply changes. If isNew  = true, it menas new event has been raised.
        private void ApplyChanges(BaseEvent @event, bool isNew){
            var method = this.GetType().GetMethod("Apply", new Type[] { @event.GetType()}); // this will refer to concrete aggregate root
            if(method == null){
                throw new ArgumentNullException(nameof(method), $"The Apply method was not found in the aggregate for {@event.GetType().Name}!");
            }
            method.Invoke(this, new object[] {@event});

            if(isNew){
                _changes.Add(@event);
            }
        }

        // will raise the event
        protected void RaiseEvent(BaseEvent @event){
            ApplyChanges(@event, true);
        }

        // replay the events. In this case isNew = false will not add the event to the list of events.
        public void ReplayEvents(IEnumerable<BaseEvent> events){
            foreach(var @event in events){
                ApplyChanges(@event, false);
            }
        }
    }
}