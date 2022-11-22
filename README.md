# Casptone project: todo-application
This solution is based on ASP.NET Core MVC framework to demonstrate its basic features and show implementation processes on it.

## Purpose of project
The goal of the project is to create a ToDo List web application that allows users to manage their personal activities by creating to do lists 
and provides functionality that enables users to:
- view all ToDo lists
- create, edit and delete ToDo lists
- add, edit (modify) and remove ToDo items to list
- track completion progress of ToDo items
- rank ToDo items by status and due date.


## Application architecture
The solution is based on Model-View-Controller ([MVC](https://en.wikipedia.org/wiki/Model-view-controller)) software architectural pattern which 
allows to separate internal representations of information from the ways information is presented to and accepted from the user. It's achieved through the
seperation of logic into the three components (Model, View and Controller). This pattern has been selected for the following benefits:
- faster development process
- easily modifiable
- ability to provide multiple views
- supports TTD (test-driven development) and others.

Also The MVC framework is easy to implement as it offers above given numerous advantages. 
Projects that are developed with the help of the MVC model can be easily developed with lesser expenditure and within less time too. 
Above all, its power to manage multiple views makes MVC the best architecture pattern for developing web applications.


### Structure (logic separation)
The solution containing three VS projects:
1. A class library project which contains classes that represent the domain model for the ToDo list application ([todo-domain-entities](./todo-domain-entities/)).
2. A client ASP.NET Core MVC application that end users can use to interact with the ToDo list application ([todo-aspnetmvc-ui](./todo-aspnetmvc-ui/)).
3. A unit test project that provides unit tests for the ToDo list class library ([todo-domain-entities.Test](./todo-domain-entities.Test/)).


### Data access layer
To store the data about ToDo lists is used a Localdb under the management of MSSQL server. The access to the database and managing data providing 
by [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/) object-relational mapping tool which allows to create a database without designing the tables directly. This aproach is called 
Code-First helps to create a new database and gives a possibility of changing the configuration of a database upon changing the classes. 
It enables the developers to get more control through the code only.


### Domain model layer
Business logic of the application is implemented in [todo-domain-entities](./todo-domain-entities/) project. There are 2 entity types:
1. `ToDoEntry` class which consists of properties to represent data about ToDo item and have reference to `ToDoList` class.
2. `ToDoList` class which has list of ToDoEntries (ToDo items).

Between `ToDoList` and `ToDoEntry` entities is One-to-many (data model) relationship.

Interface `IToDoServices` represents methods to manage ToDo lists and ToDo items. In this case used 
[Repository pattern](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design) 
to hide the logics required to store or retreive data. Class `ToDoEFCoreServicesProvider` implements `IToDoServices` interface methods and 
using Entity Framework Core's `DbContext` class to access to the database.

Class `AppDbContext` derives `DbContext` which is the primary class that is responsible for interacting with the database. 
`AppDbContext` class contains `DbSet<ToDoList>` and `DbSet<ToDoEntry>` properties which represents the collection of entities in the context, 
or that can be queried from the database. Also it accepts context configuration options passed through constructor.


### Presentation logic layer

#### Configuration and Startup

#### Controllers

#### Views

#### Models

#### Infrastructure

## Test coverage