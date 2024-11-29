# Microservices Udemy Training
- Event Sourcing and Kafka Sample project
- https://www.udemy.com/course/net-microservices-cqrs-event-sourcing-with-kafka/learn/lecture/29894624?start=0#overview


## Key Principl of Microservices
- should not share code or data    
- donot communicate directly with each other as this will couple microservices which will result in failure of one effecting other
- communicate through asynchronous event bus.

## CQRS
- software design pattern
- seggregate beyween commands(write) and query (read)
- it helps in scale command and query api's independently.
- helps in fewer lock contentions which is generally as a result of executing command and query operations on the same model.
- allows to optimize read and write data schemas.
- allows to separate concerns
- imporve data security

## Event Sourcing
- software design pattern combined with CQRS
- approach where all changes made to an object or entity are stored as sequence of immutable events to an event store.
- Benefits
    - contains complete auditable log
    - state of an object usually the aggregate, can be recreated by replaying the event store.
    - improves write performance.
    - in cas eof failure, event store can be used to restore the entire read DB.

## Apache Kafka
- open source event stream platform that enables the creation of real time event driven applications
- can be used as event bus.

## Docker 
- create a network
    - docker network create --attachable -d bridge myDockerNetwork

- list all networks
    - docker network ls

- for running kafka and zookeeper with docker
    - create docker-compose.yaml file
    - run the same in stated directory in docker terminal as below
        - docker-compose up -d

    - we can then see kafka and zookeeper containers in docker
        - docker ps
    
- to run Mongo DB in docker
    - docker run -it -d --name mongo-container -p 27017:27017 --network myDockerNetwork --restart always -v mongodb_data_container:/data/db mongo:latest

- to run MS SQL in docker 
    - docker run -d --name sql-container --network myDockerNetwork --restart always -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=$tr0ngS@P@ssw0rd02' -e 'MSSQL_PID=Express' -p 1433:1433 mcr.microsoft.com/mssql/server:2017-latest-ubuntu 

## Message Types in CQRS
- Command
    - combination of expresses interest
    - describes an action that u want to be performed
    - contains information that is required to undertake the desired action
- Event
    - describees something that has occured in application
    - typical source of event is aggregate -> when something important happens in aggreagte, it will raise an event
- Query
    - Read 

## Mediatr pattern
- Behavioral Design pattern
- Promotes loose coupling by preventing objects from referring to each other.
- simplifies communication btween objects by introducing a single object known as Mediator that manages the distribution of messages amoung other objects
- messages are Commands.

- Mediatr(ICommandDispatcher) -> defines interface for communicating with collegue objects
- ConcreteMediatr(CommandDispatcher) -> class that implements the mediatr interface, which implements coorporative behavior  by coordinating  collegue objects
- Collegue(ICommandHandler) - each class is aware of its mediatr object and can communicate with its mediatr rather than communicating with other collegue
- ConcreteCollegueObjets (CommandHandlers)

## Aggregate
- entity or group of entities that is always kept in a consistent state
- command that creates an instance of aggregate should always be handled in constructor of the aggregate

## Aggregate Root 
- entity with in aggreagte which is responsible for maintaining the state.
- manages which apply method is involked on the concrete aggregate based on the event type.
- commits the changes that have been applied to the aggregate.
- maintains list of uncommitted changes in form of events, that need to be applied to the aggregate and be persisted to the event store.

## Event store
- write database
- data is stored as a sequence of immutable events
- key component of event Sourcing
    - append only store
    - saved event should represent the satte or version of an aggreagte at any given time
    - event should be stored in chromological order and new event should be appended to previous event
    - state of aggregate should be recrratable by replaying the event store
    - implement optimistic concurrency control

## Event Model 
- represent the schema of event store and each instance of the evnt model represent the record in event store.

## Repository pattern
- used to create an abstrction layer between data access and business logic layer

## ConfigureAwait
- avoid forcing the callback to be invoked on the original context or scheduler
- it helps in improving performance and avoiding deadlocks

## Kafka Producer (in our code it is IEventProducer)
- allows us to produce or send messages to one or more Kafka topics
- serialize compress and load balances data amoung kafka brokers through partioning
- Kafka broker
    - server running in a kafka cluster, usually in form of a container.

## Kafka cluster
- made up of one or more brokers
- multiple brokers allows for load balancing reducndancy and reliable fail over brokers stateless and rely on Apache zookeeper to manage the state of the cluster
- minimum 3 brokers

![image](https://github.com/user-attachments/assets/6bf66999-eb96-4da5-94b3-5987985b524a)


                                                                                **Kafka Cluster which included below**
                                                                                        Broker 1
                                                                                        Broker 2
    producer (produce message and retrive kafka broker Id from zookeeper) --->          Broker 3                           ---------> consumer(Consume message and Update message offset and send to zookeeper)
                                                                                    Apache Zookeeper




                                                                                    Broker includes below partitions
    Producer A    ---------->                                                           Tolpic 1                                ----------> Consumer A
    Producer B    ---------->                                                           Tolpic 2                                ----------> Consumer B
    Producer C    ---------->                                                           Tolpic 3                                ----------> Consumer C


## DDD 
- approach to structure and model software in a way that it matches the business domain.
- primary focus is on core domain
- refers to the problems as domain
- describes independent problem areas as Bounded Contexts.

## Bounded Contexts
- independent problem area
- logical bounday within which a particular model is defined and applicable.
- each bounded context correlates to a microservice.

## Kafka Consumer
- every time a producer produces a new event message to kafka, it append the kafka log
- Kafka tracks a separate consumer offset for each consumer group that subscribes to topic so that multiple consumers can consume the same event messages and use them in different way
- example -Post Query API consumes POSTCREATEDEVENT to create a new entry in read DB. but another cosumer can use it tos end a email/notification ti your social media followers  that you have created a new social Post
- The order of events in kakfa commit log is important

## Complete flow of CQRS and Event sourcing project

![image](https://github.com/user-attachments/assets/76f45b78-c316-4297-9a96-e8025f1aee34)


    Commands (SocialMedia Post Command API)
        HTTP Request (.NET Core) <-> ICommandDispatcher <-> IComamndHandler    <- IEventSourcingHandler (gets called in ICommandHandler and also uses ds PostAggregate) -> IEventStore <-> IEventStoreRepository <-> Event Store/write DB
                                                         -> Post Aggregate      -> Post Aggregate -> aggregateRoot                                                                      -> IEventPublisher -> Kafka
    Queries (SocialMedia Post Query API)
        HTTP Request (.Net Core) <-> IQueryDispatcher <-> IQueryHandler <-> Postentity <-> IPostRepository <-> Read DB
                                                                                                            <- IEventHandler <- IEventConsumer <- Kafka
