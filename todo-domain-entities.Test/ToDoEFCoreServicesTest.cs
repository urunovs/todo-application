using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using todo_aspnetmvc_ui.Models.Services;
using ToDoListApplication.Tests;

namespace todo_domain_entities.Test
{
    static class InMemoryAppDbContextOptions
    {
        private static DbConnection _connection;
        private static DbContextOptions<AppDbContext> _contextOptions;

        public static DbContextOptions<AppDbContext> Options
        {
            get
            {
                if(_connection == null)
                {
                    _connection = new SqliteConnection("Filename=:memory:");
                    _connection.Open();
                }
                
                if(_contextOptions == null)
                {
                    _contextOptions = new DbContextOptionsBuilder<AppDbContext>()
                    .UseSqlite(_connection)
                    .Options;
                }

                return _contextOptions;
            }
        }
    }

    [TestFixture]
    public class ToDoEFCoreServicesTest : IDisposable
    {
        private readonly DbConnection _connection;
        private readonly DbContextOptions<AppDbContext> _contextOptions;
        private IToDoServices _todoServicesProvider;

        public ToDoEFCoreServicesTest()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            _contextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            using var context = new AppDbContext(_contextOptions);

            //Seed some initial data
            context.AddRange(
                new ToDoList
                {
                    MainTitle = "Finish EPAM courses",
                    ToDoEntries = new List<ToDoEntry>
                    {
                        new ToDoEntry {OrdinalNumber = 1, Title = "Task #1", Description = "Learn C# Basics", DueDate = new DateTime(2022, 10, 1) },
                        new ToDoEntry {OrdinalNumber = 2,  Title = "Task #2", Description = "Learn C# Advanced", DueDate = new DateTime(2022, 10, 1) },
                        new ToDoEntry {OrdinalNumber = 3,  Title = "Task #3", Description = "Learn ASP.NET Core", DueDate = new DateTime(2022, 11, 9) },
                    }
                },

                new ToDoList
                {
                    MainTitle = "Get IELTS certificate",
                    ToDoEntries = new List<ToDoEntry>
                    {
                            new ToDoEntry {OrdinalNumber = 1, Title = "Task #1", Description = "Prepare to exam", DueDate = new DateTime(2022, 6, 1) },
                            new ToDoEntry {OrdinalNumber = 2, Title = "Task #2", Description = "Learn English", DueDate = new DateTime(2022, 8, 1) },
                            new ToDoEntry {OrdinalNumber = 3, Title = "Task #3", Description = "Pass exam", DueDate = new DateTime(2022, 8, 2) },
                    }
                },

                new ToDoList
                {
                    MainTitle = "Begin new life",
                    ToDoEntries = new List<ToDoEntry>
                    {
                            new ToDoEntry {OrdinalNumber = 1, Title = "Task #1", Description = "Move to CA", DueDate = new DateTime(2022, 2, 1) },
                            new ToDoEntry {OrdinalNumber = 2, Title = "Task #2", Description = "Find a job", DueDate = new DateTime(2022, 3, 1) },
                            new ToDoEntry {OrdinalNumber = 3, Title = "Task #3", Description = "Buy a flat", DueDate = new DateTime(2022, 8, 1) },
                    }
                });

            context.SaveChanges();
        }

        [SetUp]
        public void Init()
        {
            // Base arrange
            _todoServicesProvider = new ToDoEFCoreServicesProvider(new AppDbContext(_contextOptions));
        }

        [TearDown]
        public void Cleanup()
        {
            // Checkout for IDisposable
            var servicesProviderType = _todoServicesProvider.GetType();
            var methodInfo = servicesProviderType.GetMethod("Dispose");
            var isDisposable = methodInfo?.DeclaringType.GetInterfaceMap(typeof(IDisposable)).TargetMethods.Any(x => x == methodInfo);

            if(isDisposable.HasValue && isDisposable.Value)
            {
                ((IDisposable)_todoServicesProvider).Dispose();
            }
        }

        [ TestCase(null), Order(0)]
        public void AddToDoList_PassedNull_ThrowsArgumentNullException(ToDoList toDoList)
        {
            // Act and assert
            Assert.Throws<ArgumentNullException>(() => _todoServicesProvider.AddToDoList(toDoList));
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.AddToDoListPassedWrongArgsThrowsArgumentExceptionTestCases)), Order(1)]
        public void AddToDoList_PassedWrongArgs_ThrowsArgumentException(ToDoList toDoList)
        {
            // Act and assert
            Assert.Throws<ArgumentException>(() => _todoServicesProvider.AddToDoList(toDoList));
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.AddToDoListPassedValidArgsSuccessfulExecutionTestCases)), Order(2)]
        public void AddToDoList_PassedValidObject_ReturnsInsertedInstance(ToDoList toDoList)
        {
            // Act
            var resultItem = _todoServicesProvider.AddToDoList(toDoList);

            // Assert
            Assert.IsTrue(_todoServicesProvider.ToDoLists.Contains(resultItem));
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.AddToDoListPassedValidArgsSuccessfulExecutionTestCases)), Order(3)]
        public void AddToDoList_PassedValidArgs_SuccessfulExecution(ToDoList toDoList)
        {
            // Arrange
            var todoListItemsCount = _todoServicesProvider.ToDoLists.Count();

            // Act
            _todoServicesProvider.AddToDoList(toDoList);

            // Assert
            Assert.AreEqual(todoListItemsCount + 1, _todoServicesProvider.ToDoLists.Count());
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.RemoveToDoListPassedInvalidArgsThrowsArgumentExceptionTestCases)), Order(4)]
        public void RemoveToDoList_PassedInvalidArgs_ThrowsArgumentException(ToDoList toDoList)
        {
            // Act and assert
            Assert.Throws<ArgumentException>(() => _todoServicesProvider.RemoveToDoList(toDoList.Id));
        }

        [TestCase, Order(5)]
        public void RemoveToDoList_PassedValidObject_SuccessfulExecution()
        {
            // Arrange
            var existingInstance = _todoServicesProvider.ToDoLists.First(list => list.MainTitle == "Become a superhero");
            var todoListItemsCount = _todoServicesProvider.ToDoLists.Count();

            // Act
            _todoServicesProvider.RemoveToDoList(existingInstance.Id);

            // Assert
            Assert.AreEqual(todoListItemsCount - 1, _todoServicesProvider.ToDoLists.Count());
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.ModifyToDoListNullThrowsArgumentNullExceptionTestCases))]
        public void ModifyToDoList_PassedNull_ThrowsArgumentNullException(ToDoList listToUpdate, ToDoList updatedListView)
        {
            // Act and assert
            Assert.Throws<ArgumentNullException>(() => _todoServicesProvider.ModifyToDoList(listToUpdate.Id, updatedListView));
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.ModifyToDoListInvalidArgsThrowsArgumentExceptionTestCases)), Order(6)]
        public void ModifyToDoList_InvalidArgs_ThrowsArgumentException(ToDoList listToUpdate, ToDoList updatedListView)
        {
            // Act and assert
            Assert.Throws<ArgumentException>(() => _todoServicesProvider.ModifyToDoList(listToUpdate.Id, updatedListView));
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.ModifyToDoListValidArgsReturnsUpdatedInstanceTestCases)), Order(7)]
        public void ModifyToDoList_ValidArgs_ReturnsUpdatedInstance(int listToUpdateId, ToDoList updatedListView)
        {
            // Arange
            var listToUpdate = _todoServicesProvider.ToDoLists.First(list => list.Id == listToUpdateId);
            updatedListView.ToDoEntries = listToUpdate.ToDoEntries;

            // Act
            var updatedInstance = _todoServicesProvider.ModifyToDoList(listToUpdate.Id, updatedListView);

            // Assert
            Assert.IsTrue(updatedInstance.IsTheSame(updatedListView));
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.AddToDoItemToListInvalidArgsThrowsArgumentException)), Order(8)]
        public void AddToDoItemToList_InvalidArgs_ThrowsArgumentException(ToDoEntry toDoEntry, int toDoListId)
        {
            // Act and assert
            Assert.Throws<ArgumentException>(() => _todoServicesProvider.AddToDoItemToList(toDoEntry, toDoListId));
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.AddToDoItemToListValidArgsSuccessfulExecutionTestCases)), Order(9)]
        public void AddToDoItemToList_ValidArgs_SuccessfulExecution(ToDoEntry toDoEntry, int toDoListId)
        {
            // Arrange
            var toDoList = _todoServicesProvider.ToDoLists.First(list => list.Id == toDoListId);
            toDoEntry.ToDoList = toDoList;

            // Act
            var returnedItem = _todoServicesProvider.AddToDoItemToList(toDoEntry, toDoList.Id);

            // Assert
            Assert.AreEqual(returnedItem, toDoEntry);
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.ModifyToDoEntryToListPassedNullThrowsArgumentNullExceptionTestCases)), Order(10)]
        public void ModifyToDoEntry_PassedNull_ThrowsArgumentNullException(ToDoEntry toDoEntryToUpdate, ToDoEntry updatedView)
        {
            // Act and assert
            Assert.Throws<ArgumentNullException>(() => _todoServicesProvider.ModifyToDoEntry(toDoEntryToUpdate.Id, updatedView));
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.ModifyToDoEntryToListInvalidArgsThrowsArgumentExceptionTestCases)), Order(10)]
        public void ModifyToDoEntry_InvalidArgs_ThrowsArgumentException(ToDoEntry toDoEntryToUpdate, ToDoEntry updatedView)
        {
            // Act and assert
            Assert.Throws<ArgumentException>(() => _todoServicesProvider.ModifyToDoEntry(toDoEntryToUpdate.Id, updatedView));
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.ModifyToDoEntryValidArgsReturnsUpdatedInstanceTestCases)), Order(11)]
        public void ModifyToDoEntry_ValidArgs_ReturnsUpdatedInstance(int toDoEntryToUpdateId, ToDoEntry updatedView)
        {
            // Arrange
            var toDoEntryToUpdate = _todoServicesProvider.ToDoLists.SelectMany(list => list.ToDoEntries)
                                                          .First(entry => entry.Id == toDoEntryToUpdateId);
            updatedView.ToDoList = toDoEntryToUpdate.ToDoList;

            // Act
            var updatedInstance = _todoServicesProvider.ModifyToDoEntry(toDoEntryToUpdate.Id, updatedView);

            // Assert
            Assert.IsTrue(updatedInstance.IsTheSame(updatedView));
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.RemoveToDoEntryPassedInvalidArgsThrowsArgumentExceptionTestCases)), Order(12)]
        public void RemoveToDoEntry_PassedInvalidArgs_ThrowsArgumentException(ToDoEntry toDoEntry)
        {
            // Act and assert
            Assert.Throws<ArgumentException>(() => _todoServicesProvider.RemoveToDoEntry(toDoEntry.Id));
        }

        [TestCase, Order(13)]
        public void RemoveToDoEntry_PassedValidObject_SuccessfulExecution()
        {
            // Arrange
            var existingInstance = _todoServicesProvider.ToDoLists.First();
            var todoListItemsCount = existingInstance.ToDoEntries.Count;

            // Act
            _todoServicesProvider.RemoveToDoEntry(existingInstance.ToDoEntries.Last().Id);

            // Assert
            Assert.AreEqual(todoListItemsCount - 1, existingInstance.ToDoEntries.Count);
        }


        [TestCase, Order(16)]
        public void ClearToDoList_NoArgs_ReturnsUpdatedToDoListItem()
        {
            // Arrange
            var toDoList = _todoServicesProvider.ToDoLists.First(list => list.Id == 3);

            // Act
            var clearedToDoList = _todoServicesProvider.ClearToDoList(toDoList.Id);

            // Assert
            Assert.IsTrue(clearedToDoList.ToDoEntries.Count == 0);
        }

        [TestCase, Order(17)]
        public void RemoveAllToDoLists_NoArgs_SuccessfulExecution()
        {
            // Act
            _todoServicesProvider.RemoveAllToDoLists();

            // Assert
            Assert.IsTrue(_todoServicesProvider.ToDoLists.Count() == 0);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}