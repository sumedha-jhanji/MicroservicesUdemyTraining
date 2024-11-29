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


# Microservices Interview Questions Udemy course
- https://www.udemy.com/course/microservices-interview-questions-passsing-guarranteed/learn/lecture/28797268?start=0#overview

## COHESION
- degree to which the elements of a module. code or software are related.
- degree to which all elements of a module, software or code are directed towards performing a single task.
- high COHESION is a good
- Lowe cohesion example - helpre classes

## ALTERNATIVES TO MONOTLITHIC apps
- component based monolithic app
    - made up of multiple binaries(dll)
    - all libraries run as a part of single process and app.
- SOA   
    -  app is broken down into services
    - same technology stack
    - communicate via SOAP or RESTful apis 
    - high coupling
    - sync calls
    - use shared DB
- Microservices architecture
    - breaks into independent services
    - low coupluing
    - can be async manner.
    - no shared DB.
    - mix and match technology stack.

## BLAST radius
- degree to which entrier system is affectec if micro service fails or shut down.

- Patterns of resiliancy to reduce balst radius
    - Independent database for each service
    - Timeouts
    - Backoff Strategy (retries)
    - Fallback pattern
    - Circuit Breaker Pattern
    - BulkHead Pattern
    - Asynchronous communication

## CIRCUIT BREAKER Pattern
- we set threshold for failing API calls example 3. so it can fail 3 times and then it needs to wait
- if calls execeed the threshold, then we reject api calls
- it is a microservice which acts as proxy or can be used as built in feature in microservice
- 3 stages: 
    - open - we reject calls here
    - closed - we accept calls. It is good state
    - half-open - we sometimes accept calls and sometime reject calls.

- Closed state
    client ---> request to circuit breaker ---> request to microservice ---> result to circuit breaker ---> result to client

- Open state
      client ---> request to circuit breaker ---> returns error to client

## Ways to build microservices
- single-tenant : one microservice on one virtual machine
- containerized : we need docker to create container and kubernates for orchestration
- serverless: small microservices run without server in cloud environment.

## EVENT Vs Message
- Events are something that has a;ready happened in past. Order cannot be changed. Handled by even streaming platform like apache kafka
- Messages are also called as commands -> we ask them to happend. Order can be changed. Theyc an be made either by making call to API or through message broker.

## Distributed transaction
- each service has its own database, so managing transactions accross Multiple services is called distrubuted transactions.
- It is also called as Eventual Consistency
- the handle can be doen using either of 2 ways
    - Two-Phase Commit (2PC) pattern    
        - Prepare Phase
            - Microservice is asked to prepare for change
        - Commit Phase
            asked to make final and actual change
        example -> in first phase say we create a transatcion with status = 0 (this means it cannot be read for further processings).
                -> in second phase, we will change the status to =1

    - Saga Pattern
        - asynchronous
        - each microservice is for its own transaction and there is no waiting for microservices to finish the transaction.
        - 2 Patterns    
            Choreography Pattern -> if something goes wrong, the microservice that has a failed operation has to notify the upsteam(previous microservice) to rollback
            Orchestration Pattern -> here central microservices that delivers the messages for rollback.

## Bounded Context
- concept in DDD (framework of anlayzing and modelling large problems).
- boundary with in a domain where a particular domain model appies i.e the way we deal with model is a bounded context eg product can be handled in one way in placing order, another way in return order etc
- one bounded context represnets one microservice

## Consumer Defined Contract (CDC)
- consumer  is consumer of an api
- it is a kind of test to ensure constant changees in microservice will not break the dependent microservice.

## Continuous Monitoring
- automated process to ensure the security checks and extrenal compliance issues in application.

## Idempotence
- concept that ensures, same output for same input.

## Side Car Pattern
- deploying components of an app or service into a separate process or container to provide isolation and encapsulation.
- core functionality will be in main microservice and other functionalities lile logging, configuration, proxy to remote service etc in side car micro service.

## Types of tests
- Functional tetsing -> is overall system working
- Load tetsing -> does service scale if load goes up.
- Resiliance tetsing -> how app reacts to failures (infrastructure, network failure, other microservice failure.)

## DOCKER's role
- create image -> binar file which ahs app and its dependencies
- run container -> running instance of docker image
- provides networking -> containers can connect to each other.

## How can u deploy containers
- deploy to server that have docker -> only for local deployment
- deploy to server less container services lile Azure Conatiner Registry (ACR) in Azure
- deploy to a container orcehstration service like kubernates - provide n/wing, loggong, monitoirng etc

## Scaling Cube
- ways of scaling
    - x-axis -> more resources (auto scaling)
    - y-axis -> split system by function microservices
    - z-axis -> partioning (handle customers of each country by different servers closer to them)

## Strangler application
- pattern where new app is built around existing one, gradually replacing its functionalities.
- contributes to migartion process by allowing the monilith to be decomposed into microservices incrementally, reducing the risk and complexity of the migration.


## Queries (SocialMedia Post Query API)
        HTTP Request (.Net Core) <-> IQueryDispatcher <-> IQueryHandler <-> Postentity <-> IPostRepository <-> Read DB
                                                                                                            <- IEventHandler <- IEventConsumer <- Kafka
