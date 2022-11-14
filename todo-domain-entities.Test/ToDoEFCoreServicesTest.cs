using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using ToDoListApplication.Tests;

namespace todo_domain_entities.Test
{
    [TestFixture]
    public class ToDoEFCoreServicesTest : IDisposable
    {
        private readonly DbConnection _connection;
        private readonly DbContextOptions<AppDbContext> _contextOptions;
        private ToDoEFCoreServicesProvider _todoEfCoreServicesProvider;

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
            _todoEfCoreServicesProvider = new ToDoEFCoreServicesProvider(new AppDbContext(_contextOptions));
        }

        [TearDown]
        public void Cleanup()
        {
            _todoEfCoreServicesProvider.Dispose();
        }


        [TestCaseSource(typeof(TestCases), nameof(TestCases.AddToDoListPassedWrongArgsThrowsArgumentExceptionTestCases)), Order(1)]
        public void AddToDoList_PassedWrongArgs_ThrowsArgumentException(ToDoList toDoList)
        {
            // Act and assert
            Assert.Throws<ArgumentException>(() => _todoEfCoreServicesProvider.AddToDoList(toDoList));
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.AddToDoListPassedValidArgsSuccessfulExecutionTestCases)), Order(2)]
        public void AddToDoList_PassedValidObject_ReturnsInsertedInstance(ToDoList toDoList)
        {
            // Act
            var resultItem = _todoEfCoreServicesProvider.AddToDoList(toDoList);

            // Assert
            Assert.IsTrue(_todoEfCoreServicesProvider.ToDoLists.Contains(resultItem));
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.AddToDoListPassedValidArgsSuccessfulExecutionTestCases)), Order(3)]
        public void AddToDoList_PassedValidArgs_SuccessfulExecution(ToDoList toDoList)
        {
            // Arrange
            var todoListItemsCount = _todoEfCoreServicesProvider.ToDoLists.Count();

            // Act
            _todoEfCoreServicesProvider.AddToDoList(toDoList);

            // Assert
            Assert.AreEqual(todoListItemsCount + 1, _todoEfCoreServicesProvider.ToDoLists.Count());
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.RemoveToDoListPassedInvalidArgsThrowsArgumentExceptionTestCases)), Order(4)]
        public void RemoveToDoList_PassedInvalidArgs_ThrowsArgumentException(ToDoList toDoList)
        {
            // Act and assert
            Assert.Throws<ArgumentException>(() => _todoEfCoreServicesProvider.RemoveToDoList(toDoList.Id));
        }

        [TestCase, Order(5)]
        public void RemoveToDoList_PassedValidObject_SuccessfulExecution()
        {
            // Arrange
            var existingInstance = _todoEfCoreServicesProvider.ToDoLists.First(list => list.MainTitle == "Become a superhero");
            var todoListItemsCount = _todoEfCoreServicesProvider.ToDoLists.Count();

            // Act
            _todoEfCoreServicesProvider.RemoveToDoList(existingInstance.Id);

            // Assert
            Assert.AreEqual(todoListItemsCount - 1, _todoEfCoreServicesProvider.ToDoLists.Count());
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.ModifyToDoListInvalidArgsThrowsArgumentExceptionTestCases)), Order(6)]
        public void ModifyToDoList_InvalidArgs_ThrowsArgumentException(ToDoList listToUpdate, ToDoList updatedListView)
        {
            // Act and assert
            Assert.Throws<ArgumentException>(() => _todoEfCoreServicesProvider.ModifyToDoList(listToUpdate.Id, updatedListView));
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.ModifyToDoListValidArgsReturnsUpdatedInstanceTestCases)), Order(7)]
        public void ModifyToDoList_ValidArgs_ReturnsUpdatedInstance(int listToUpdateId, ToDoList updatedListView)
        {
            // Arange
            var listToUpdate = _todoEfCoreServicesProvider.ToDoLists.First(list => list.Id == listToUpdateId);
            updatedListView.ToDoEntries = listToUpdate.ToDoEntries;

            // Act
            var updatedInstance = _todoEfCoreServicesProvider.ModifyToDoList(listToUpdate.Id, updatedListView);

            // Assert
            Assert.IsTrue(updatedInstance.IsTheSame(updatedListView));
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.AddToDoItemToListInvalidArgsThrowsArgumentException)), Order(8)]
        public void AddToDoItemToList_InvalidArgs_ThrowsArgumentException(ToDoEntry toDoEntry, int toDoListId)
        {
            // Act and assert
            Assert.Throws<ArgumentException>(() => _todoEfCoreServicesProvider.AddToDoItemToList(toDoEntry, toDoListId));
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.AddToDoItemToListValidArgsSuccessfulExecutionTestCases)), Order(9)]
        public void AddToDoItemToList_ValidArgs_SuccessfulExecution(ToDoEntry toDoEntry, int toDoListId)
        {
            // Arrange
            var toDoList = _todoEfCoreServicesProvider.ToDoLists.First(list => list.Id == toDoListId);
            toDoEntry.ToDoList = toDoList;

            // Act
            var returnedItem = _todoEfCoreServicesProvider.AddToDoItemToList(toDoEntry, toDoList.Id);

            // Assert
            Assert.AreEqual(returnedItem, toDoEntry);
        }               

        [TestCaseSource(typeof(TestCases), nameof(TestCases.ModifyToDoEntryToListInvalidArgsThrowsArgumentExceptionTestCases)), Order(10)]
        public void ModifyToDoEntry_InvalidArgs_ThrowsArgumentException(ToDoEntry toDoEntryToUpdate, ToDoEntry updatedView)
        {
            // Act and assert
            Assert.Throws<ArgumentException>(() => _todoEfCoreServicesProvider.ModifyToDoEntry(toDoEntryToUpdate.Id, updatedView));
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.ModifyToDoEntryValidArgsReturnsUpdatedInstanceTestCases)), Order(11)]
        public void ModifyToDoEntry_ValidArgs_ReturnsUpdatedInstance(int toDoEntryToUpdateId, ToDoEntry updatedView)
        {
            // Arrange
            var toDoEntryToUpdate = _todoEfCoreServicesProvider.ToDoLists.SelectMany(list => list.ToDoEntries)
                                                          .First(entry => entry.Id == toDoEntryToUpdateId);
            updatedView.ToDoList = toDoEntryToUpdate.ToDoList;

            // Act
            var updatedInstance = _todoEfCoreServicesProvider.ModifyToDoEntry(toDoEntryToUpdate.Id, updatedView);

            // Assert
            Assert.IsTrue(updatedInstance.IsTheSame(updatedView));
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.RemoveToDoEntryPassedInvalidArgsThrowsArgumentExceptionTestCases)), Order(12)]
        public void RemoveToDoEntry_PassedInvalidArgs_ThrowsArgumentException(ToDoEntry toDoEntry)
        {
            // Act and assert
            Assert.Throws<ArgumentException>(() => _todoEfCoreServicesProvider.RemoveToDoEntry(toDoEntry.Id));
        }

        [TestCase, Order(13)]
        public void RemoveToDoEntry_PassedValidObject_SuccessfulExecution()
        {
            // Arrange
            var existingInstance = _todoEfCoreServicesProvider.ToDoLists.First();
            var todoListItemsCount = existingInstance.ToDoEntries.Count;

            // Act
            _todoEfCoreServicesProvider.RemoveToDoEntry(existingInstance.ToDoEntries.Last().Id);

            // Assert
            Assert.AreEqual(todoListItemsCount - 1, existingInstance.ToDoEntries.Count);
        }


        [TestCase, Order(16)]
        public void ClearToDoList_NoArgs_ReturnsUpdatedToDoListItem()
        {
            // Arrange
            var toDoList = _todoEfCoreServicesProvider.ToDoLists.First(list => list.Id == 3);

            // Act
            var clearedToDoList = _todoEfCoreServicesProvider.ClearToDoList(toDoList.Id);

            // Assert
            Assert.IsTrue(clearedToDoList.ToDoEntries.Count == 0);
        }

        [TestCase, Order(17)]
        public void RemoveAllToDoLists_NoArgs_SuccessfulExecution()
        {
            // Act
            _todoEfCoreServicesProvider.RemoveAllToDoLists();

            // Assert
            Assert.IsTrue(_todoEfCoreServicesProvider.ToDoLists.Count() == 0);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _connection.Dispose();
        }
    }
}