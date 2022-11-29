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
The UI part of application is located in ASP.NET Core MVC framework based [todo-aspnetmvc-ui](./todo-aspnetmvc-ui/) project and using domain model through reference to model project.

#### Configuration and Startup
In `Startup` class configuring services to work with Controllers/Views and loading database access settings from [appsettings.json](./todo-aspnetmvc-ui/appsettings.json) file. Also in `Startup` class adding middleware components and mapping routing templates, populating the database with initial data.

#### Controllers
There are 2 controller classes with corresponding action methods:
- `HomeController` - generating response of application home page.
- `ToDoListsController` - generating responses of actions on ToDoList and ToDoEntry objects.
These controllers performing interaction logic between UI and domain services.

#### Views
Views is a razor page based files with implementaion of ToDo list ojbects presentation. There are 2 main folders corresponding to controller names and common folder with shared view files. As UI styled forms template used bootstrap library.

## Test coverage
To cover domain model with unit tests used NUnit unit-testing framework and created 32 test cases for `IToDoServices` interface implementation methods of `ToDoEFCoreServicesProvider` class. As a testing strategy used [SQLite with in-memory feature](https://learn.microsoft.com/en-us/ef/core/testing/choosing-a-testing-strategy#sqlite-as-a-database-fake) as a database fake approach because of Entity Framework Core 5 which provides database access [does not supports proper mocking functionality](https://learn.microsoft.com/en-us/ef/core/testing/choosing-a-testing-strategy#mocking-or-stubbing-dbcontext-and-dbset) of `DbSet` classes.
