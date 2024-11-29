using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace CQRS.Core.Events
{
    [BsonNoId]
    [BsonIgnoreExtraElements]
    //represent the schema of event store and each instance of the event model represent the record in event store.
    public class EventModel
    {
        //in mongo db, we have _id field or object id field which is primary field of GUID type. 
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public DateTime Timestamp { get; set; }
        public Guid AggregateIdentifier { get; set; }
        public string AggregateType { get; set; }
        public int Version { get; set; }
        public string EventType { get; set; }
        public BaseEvent EventData { get; set; }

    }
}