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
    public class EFToDoListRepositoryTest : IDisposable
    {
        private readonly DbConnection _connection;
        private readonly DbContextOptions<AppDbContext> _contextOptions;
        private EFToDoListRepository _efToDoListRepository;

        public EFToDoListRepositoryTest()
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
            _efToDoListRepository = new EFToDoListRepository(new AppDbContext(_contextOptions));
        }

        [TearDown]
        public void Cleanup()
        {
            _efToDoListRepository.Dispose();
        }


        [TestCaseSource(typeof(TestCases), nameof(TestCases.AddToDoListPassedWrongArgsThrowsArgumentExceptionTestCases)), Order(1)]
        public void AddToDoList_PassedWrongArgs_ThrowsArgumentException(ToDoList toDoList)
        {
            // Act and assert
            Assert.Throws<ArgumentException>(() => _efToDoListRepository.AddToDoList(toDoList));
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.AddToDoListPassedValidArgsSuccessfulExecutionTestCases)), Order(2)]
        public void AddToDoList_PassedValidObject_ReturnsInsertedInstance(ToDoList toDoList)
        {
            // Act
            var resultItem = _efToDoListRepository.AddToDoList(toDoList);

            // Assert
            Assert.IsTrue(_efToDoListRepository.ToDoLists.Contains(resultItem));
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.AddToDoListPassedValidArgsSuccessfulExecutionTestCases)), Order(3)]
        public void AddToDoList_PassedValidArgs_SuccessfulExecution(ToDoList toDoList)
        {
            // Arrange
            var todoListItemsCount = _efToDoListRepository.ToDoLists.Count();

            // Act
            _efToDoListRepository.AddToDoList(toDoList);

            // Assert
            Assert.AreEqual(todoListItemsCount + 1, _efToDoListRepository.ToDoLists.Count());
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.RemoveToDoListPassedInvalidArgsThrowsArgumentExceptionTestCases)), Order(4)]
        public void RemoveToDoList_PassedInvalidArgs_ThrowsArgumentException(ToDoList toDoList)
        {
            // Act and assert
            Assert.Throws<ArgumentException>(() => _efToDoListRepository.RemoveToDoList(toDoList));
        }

        [TestCase, Order(5)]
        public void RemoveToDoList_PassedValidObject_SuccessfulExecution()
        {
            // Arrange
            var existingInstance = _efToDoListRepository.ToDoLists.First(list => list.MainTitle == "Become a superhero");
            var todoListItemsCount = _efToDoListRepository.ToDoLists.Count();

            // Act
            _efToDoListRepository.RemoveToDoList(existingInstance);

            // Assert
            Assert.AreEqual(todoListItemsCount - 1, _efToDoListRepository.ToDoLists.Count());
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.ModifyToDoListInvalidArgsThrowsArgumentExceptionTestCases)), Order(6)]
        public void ModifyToDoList_InvalidArgs_ThrowsArgumentException(ToDoList listToUpdate, ToDoList updatedListView)
        {
            // Act and assert
            Assert.Throws<ArgumentException>(() => _efToDoListRepository.ModifyToDoList(listToUpdate, updatedListView));
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.ModifyToDoListValidArgsReturnsUpdatedInstanceTestCases)), Order(7)]
        public void ModifyToDoList_ValidArgs_ReturnsUpdatedInstance(int listToUpdateId, ToDoList updatedListView)
        {
            // Arange
            var listToUpdate = _efToDoListRepository.ToDoLists.First(list => list.Id == listToUpdateId);
            updatedListView.ToDoEntries.ForEach(entry => entry.ToDoList = updatedListView);

            // Act
            var updatedInstance = _efToDoListRepository.ModifyToDoList(listToUpdate, updatedListView);

            // Assert
            Assert.IsTrue(updatedInstance.IsTheSame(updatedListView));
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.AddToDoEntriesToListInvalidArgsThrowsArgumentException)), Order(8)]
        public void AddToDoEntriesToList_InvalidArgs_ThrowsArgumentException(List<ToDoEntry> toDoEntries, int toDoListId)
        {
            // Arrange
            var toDoList = _efToDoListRepository.ToDoLists.FirstOrDefault(list => list.Id == toDoListId);

            // Act and assert
            Assert.Throws<ArgumentException>(() => _efToDoListRepository.AddToDoEntriesToList(toDoEntries, toDoList));
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.AddToDoEntriesToListValidArgsSuccessfulExecutionTestCases)), Order(9)]
        public void AddToDoEntriesToList_ValidArgs_SuccessfulExecution(List<ToDoEntry> toDoEntries, int toDoListId)
        {
            // Arrange
            var toDoList = _efToDoListRepository.ToDoLists.First(list => list.Id == toDoListId);
            toDoEntries.ForEach(entry => entry.ToDoList = toDoList);

            // Act
            _efToDoListRepository.AddToDoEntriesToList(toDoEntries, toDoList);

            // Assert
            Assert.IsTrue(toDoList.ToDoEntries.SequenceEqual(toDoEntries));
        }               

        [TestCaseSource(typeof(TestCases), nameof(TestCases.ModifyToDoEntryToListInvalidArgsThrowsArgumentExceptionTestCases)), Order(10)]
        public void ModifyToDoEntry_InvalidArgs_ThrowsArgumentException(ToDoEntry toDoEntryToUpdate, ToDoEntry updatedView)
        {
            // Act and assert
            Assert.Throws<ArgumentException>(() => _efToDoListRepository.ModifyToDoEntry(toDoEntryToUpdate, updatedView));
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.ModifyToDoEntryValidArgsReturnsUpdatedInstanceTestCases)), Order(11)]
        public void ModifyToDoEntry_ValidArgs_ReturnsUpdatedInstance(int toDoEntryToUpdateId, ToDoEntry updatedView)
        {
            // Arrange
            var toDoEntryToUpdate = _efToDoListRepository.ToDoLists.SelectMany(list => list.ToDoEntries)
                                                          .First(entry => entry.Id == toDoEntryToUpdateId);
            updatedView.ToDoList = toDoEntryToUpdate.ToDoList;

            // Act
            var updatedInstance = _efToDoListRepository.ModifyToDoEntry(toDoEntryToUpdate, updatedView);

            // Assert
            Assert.IsTrue(updatedInstance.IsTheSame(updatedView));
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.RemoveToDoEntryPassedInvalidArgsThrowsArgumentExceptionTestCases)), Order(12)]
        public void RemoveToDoEntry_PassedInvalidArgs_ThrowsArgumentException(ToDoEntry toDoEntry)
        {
            // Act and assert
            Assert.Throws<ArgumentException>(() => _efToDoListRepository.RemoveToDoEntry(toDoEntry));
        }

        [TestCase, Order(13)]
        public void RemoveToDoEntry_PassedValidObject_SuccessfulExecution()
        {
            // Arrange
            var existingInstance = _efToDoListRepository.ToDoLists.First();
            var todoListItemsCount = existingInstance.ToDoEntries.Count;

            // Act
            _efToDoListRepository.RemoveToDoEntry(existingInstance.ToDoEntries.Last());

            // Assert
            Assert.AreEqual(todoListItemsCount - 1, existingInstance.ToDoEntries.Count);
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.SetToDoEntryStatusInvalidArgsThrowsArgumentExceptionTestCases)), Order(14)]
        public void SetToDoEntryStatus_InvalidArgs_ThrowsArgumentException(ToDoEntry toDoEntryToUpdate, ToDoStatus status)
        {
            // Act and assert
            Assert.Throws<ArgumentException>(() => _efToDoListRepository.SetToDoEntryStatus(toDoEntryToUpdate, status));
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.SetToDoEntryStatusValidArgsReturnsUpdatedInstanceTestCases)), Order(15)]
        public void SetToDoEntryStatus_ValidArgs_ReturnsUpdatedInstance(int todoListId, int toDoEntryToUpdateId, ToDoStatus status)
        {
            // Arrange
            var todoList = _efToDoListRepository.ToDoLists.Single(list => list.Id == todoListId);
            var toDoEntryToUpdate = todoList.ToDoEntries.Single(entry => entry.Id == toDoEntryToUpdateId);

            // Act
            var updatedInstance = _efToDoListRepository.SetToDoEntryStatus(toDoEntryToUpdate, status);

            // Assert
            Assert.IsTrue(updatedInstance.Status == status);
        }

        [TestCase, Order(16)]
        public void ClearToDoList_NoArgs_ReturnsUpdatedToDoListItem()
        {
            // Arrange
            var toDoList = _efToDoListRepository.ToDoLists.First(list => list.Id == 3);

            // Act
            var listWithoutToDoItems = _efToDoListRepository.ClearToDoList(toDoList);

            // Assert
            Assert.IsTrue(listWithoutToDoItems.ToDoEntries.Count == 0);
        }

        [TestCase, Order(17)]
        public void RemoveAllToDoLists_NoArgs_SuccessfulExecution()
        {
            // Act
            _efToDoListRepository.RemoveAllToDoLists();

            // Assert
            Assert.IsTrue(_efToDoListRepository.ToDoLists.Count() == 0);
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