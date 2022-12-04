using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using todo_aspnetmvc_ui.Models.Services;
using ToDoListApplication.Tests;

namespace todo_domain_entities.Test
{
    [TestFixture]
    public class IToDoServicesTest : IDisposable
    {
        private AppDbContext _context;
        private DbConnection _connection;
        private DbContextOptions<AppDbContext> _contextOptions;
        private IToDoServices _todoServicesProvider;

        [SetUp]
        public void Init()
        {
            // Base arrange
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            _contextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new AppDbContext(_contextOptions);
            _todoServicesProvider = new ToDoEFCoreServicesProvider(_context);
            PopulateDatabaseFake();
        }

        [TearDown]
        public void Cleanup()
        {
            // Checkout for IDisposable
            var servicesProviderType = _todoServicesProvider.GetType();
            var methodInfo = servicesProviderType.GetMethod("Dispose");
            var isDisposable = methodInfo?.DeclaringType.GetInterfaceMap(typeof(IDisposable)).TargetMethods.Any(x => x == methodInfo);

            if (isDisposable.HasValue && isDisposable.Value)
            {
                ((IDisposable)_todoServicesProvider).Dispose();
            }

            _connection.Close();
        }

        [Test]
        public void GetToDoLists_ReturnsToDoLists()
        {
            // Act
            var (_, count) = _todoServicesProvider.GetToDoLists(5, 1);

            //Assert
            Assert.AreEqual(3, count);
        }


        [Test]
        public void GetVisibleToDoLists_ReturnsOnlyVisibleLists()
        {
            // Arrange
            var visibleListTitles = new string[] { "Finish EPAM courses", "Get IELTS certificate" };
            var i = 0;
            var (lists, count) = _todoServicesProvider.GetVisibleToDoLists(5, 0);

            // Act and assert
            foreach (var list in lists)
            {
                Assert.IsTrue(list.MainTitle == visibleListTitles[i++]);
            }

            Assert.IsTrue(count == 2);
        }

        [Test]
        public void GetCompletedToDoLists_ReturnsOnlyCompletedLists()
        {
            // Arrange
            var completedListTitle = "Get IELTS certificate";
            var (lists, count) = _todoServicesProvider.GetCompletedToDoLists(5, 0);

            // Act and assert
            foreach (var list in lists)
            {
                Assert.IsTrue(list.MainTitle == completedListTitle);
            }

            Assert.IsTrue(count == 1);
        }

        [Test]
        public void GetToDoListById_ReturnsExpectedObject()
        {
            // Arrange
            var firstList = _context.ToDoLists.First();

            // Act
            var list = _todoServicesProvider.GetToDoListById(firstList.Id);

            // Assert
            Assert.AreEqual(firstList, list);
        }

        [Test]
        public void GetToDoListById_ReturnsNull()
        {
            // Act
            var list = _todoServicesProvider.GetToDoListById(0);

            // Assert
            Assert.IsTrue(list == null);
        }

        [TestCase(null)]
        public void AddToDoList_PassedNull_ThrowsArgumentNullException(ToDoList toDoList)
        {
            // Act and assert
            Assert.Throws<ArgumentNullException>(() => _todoServicesProvider.AddToDoList(toDoList));
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.AddToDoListPassedWrongArgsThrowsArgumentExceptionTestCases))]
        public void AddToDoList_PassedWrongArgs_ThrowsArgumentException(ToDoList toDoList)
        {
            // Act and assert
            Assert.Throws<ArgumentException>(() => _todoServicesProvider.AddToDoList(toDoList));
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.AddToDoListPassedValidArgsSuccessfulExecutionTestCases))]
        public void AddToDoList_PassedValidObject_ReturnsInsertedInstance(ToDoList toDoList)
        {
            // Act
            var resultItem = _todoServicesProvider.AddToDoList(toDoList);
            
            // Assert
            Assert.IsTrue(_context.ToDoLists.Contains(resultItem));
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void RemoveToDoList_PassedInvalidArgs_ThrowsArgumentException(int toDoListId)
        {
            // Act and assert
            Assert.Throws<ArgumentException>(() => _todoServicesProvider.RemoveToDoList(toDoListId));
        }

        [Test]
        public void RemoveToDoList_PassedValidObject_RemovesObject()
        {
            // Arrange
            var existingInstance = _context.ToDoLists.First();
            var todoListItemsCount = _context.ToDoLists.Count();

            // Act
            _todoServicesProvider.RemoveToDoList(existingInstance.Id);

            // Assert
            Assert.AreEqual(todoListItemsCount - 1, _context.ToDoLists.Count());
            Assert.IsFalse(_context.ToDoLists.Contains(existingInstance));
        }

        [TestCase(1, null)]
        public void ModifyToDoList_PassedNull_ThrowsArgumentNullException(int listToUpdateId, ToDoList updatedListView)
        {
            // Act and assert
            Assert.Throws<ArgumentNullException>(() => _todoServicesProvider.ModifyToDoList(listToUpdateId, updatedListView));
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.ModifyToDoListInvalidArgsThrowsArgumentExceptionTestCases))]
        public void ModifyToDoList_InvalidArgs_ThrowsArgumentException(ToDoList listToUpdate, ToDoList updatedListView)
        {
            // Act and assert
            Assert.Throws<ArgumentException>(() => _todoServicesProvider.ModifyToDoList(listToUpdate.Id, updatedListView));
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.ModifyToDoListValidArgsReturnsUpdatedInstanceTestCases))]
        public void ModifyToDoList_ValidArgs_ReturnsUpdatedInstance(int listToUpdateId, ToDoList updatedListView)
        {
            // Arange
            var listToUpdate = _context.ToDoLists.First(list => list.Id == listToUpdateId);
            updatedListView.ToDoEntries = listToUpdate.ToDoEntries;

            // Act
            var updatedInstance = _todoServicesProvider.ModifyToDoList(listToUpdate.Id, updatedListView);

            // Assert
            Assert.IsTrue(updatedInstance.IsTheSame(updatedListView));
        }

        [Test]
        public void AddToDoItemToList_PassedInvalidItem_ThrowsArgumentException()
        {
            // Arrange
            var todoItem = new ToDoEntry
            {
                OrdinalNumber = 1,
                Title = "Task #1",
                Description = "Move to CA",
                DueDate = new DateTime(2022, 2, 1)
            };

            // Act and assert
            Assert.Throws<ArgumentException>(() => _todoServicesProvider.AddToDoItemToList(todoItem, -1));
        }

        [Test]
        public void AddToDoItemToList_PassedNull_ThrowsArgumentNullException()
        {
            // Act and assert
            Assert.Throws<ArgumentNullException>(() => _todoServicesProvider.AddToDoItemToList(null, 1));
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.AddToDoItemToListValidArgsReturnsAddedInstanceTestCases))]
        public void AddToDoItemToList_ValidArgs_ReturnsAddedInstance(ToDoEntry toDoEntry, int toDoListId)
        {
            // Arrange
            var toDoList = _context.ToDoLists.First(list => list.Id == toDoListId);
            toDoEntry.ToDoList = toDoList;

            // Act
            var returnedItem = _todoServicesProvider.AddToDoItemToList(toDoEntry, toDoList.Id);

            // Assert
            Assert.AreEqual(returnedItem, toDoEntry);
            Assert.IsTrue(toDoList.ToDoEntries.Contains(returnedItem));
        }

        [Test]
        public void ModifyToDoEntry_PassedNull_ThrowsArgumentNullException()
        {
            // Act and assert
            Assert.Throws<ArgumentNullException>(() => _todoServicesProvider.ModifyToDoEntry(1, null));
        }

        [Test]
        public void ModifyToDoEntry_InvalidArgs_ThrowsArgumentException()
        {
            // Arrange
            var notExistingItemId = -1;

            // Act and assert
            Assert.Throws<ArgumentException>(() => _todoServicesProvider.ModifyToDoEntry(notExistingItemId, new ToDoEntry()));
        }

        [Test]
        public void ModifyToDoEntry_ValidArgs_ReturnsUpdatedInstance()
        {
            // Arrange
            var toDoEntryToUpdate = _context.ToDoLists.First().ToDoEntries.First();
            var newTitle = "New title";
            toDoEntryToUpdate.Title = newTitle;

            // Act
            var updatedInstance = _todoServicesProvider.ModifyToDoEntry(toDoEntryToUpdate.Id, toDoEntryToUpdate);

            // Assert
            Assert.AreEqual(newTitle, updatedInstance.Title);
            Assert.IsTrue(_context.ToDoLists.First().ToDoEntries.First().Title == newTitle);
        }

        [Test]
        public void RemoveToDoEntry_PassedInvalidArgs_ThrowsArgumentException()
        {
            // Act and assert
            Assert.Throws<ArgumentException>(() => _todoServicesProvider.RemoveToDoEntry(-1));
        }

        [Test]
        public void RemoveToDoEntry_PassedValidObject_SuccessfulExecution()
        {
            // Arrange
            var toDoList = _context.ToDoLists.First();
            var todoListItemsCount = toDoList.ToDoEntries.Count;
            var lastToDoItem = toDoList.ToDoEntries.Last();

            // Act
            _todoServicesProvider.RemoveToDoEntry(lastToDoItem.Id);

            // Assert
            Assert.AreEqual(todoListItemsCount - 1, toDoList.ToDoEntries.Count);
            Assert.IsFalse(toDoList.ToDoEntries.Contains(lastToDoItem));
        }

        [TestCase(ToDoItemsDueDate.DueDateToday)]
        [TestCase(ToDoItemsDueDate.DueDateTomorrow)]
        [TestCase(ToDoItemsDueDate.DueDateOverdue)]
        [TestCase(ToDoItemsDueDate.DueDateThisMonth)]
        public void GetGroupedToDoItemsByDueDate_PassedDueDateType_ReturnsGroupedItems(ToDoItemsDueDate dueDate)
        {
            // Act
            var (groupedItems, count) = _todoServicesProvider.GetGroupedToDoItemsByDueDate(dueDate, 10, 1);

            // Assert
            switch(dueDate)
            {
                case ToDoItemsDueDate.DueDateTomorrow:
                    foreach (var items in groupedItems)
                    {
                        Assert.IsTrue(items.All(item => item.Status != ToDoStatus.Completed
                                                     && item.DueDate.Value.Date == DateTime.Today.AddDays(1)));
                    }

                    
                    break;

                case ToDoItemsDueDate.DueDateOverdue:
                    foreach (var items in groupedItems)
                    {
                        Assert.IsTrue(items.All(item => item.Status != ToDoStatus.Completed
                                                     && item.DueDate.Value.Date < DateTime.Today));
                    }
                    break;

                case ToDoItemsDueDate.DueDateThisMonth:
                    foreach (var items in groupedItems)
                    {
                        Assert.IsTrue(items.All(item => item.Status != ToDoStatus.Completed
                                                     && item.DueDate.Value.Date.Month == DateTime.Today.Month
                                                     && item.DueDate.Value.Date.Year == DateTime.Today.Year));
                    }
                    break;

                default:
                    foreach (var items in groupedItems)
                    {
                        Assert.IsTrue(items.All(item => item.Status != ToDoStatus.Completed
                                                     && item.DueDate.Value.Date == DateTime.Today));
                    }
                    break;
            }

            var expectedItemsCount = groupedItems.Sum(items => items.Count());
            Assert.AreEqual(expectedItemsCount, count);
        }

        [Test]
        public void ClearToDoList_PassedWrongListId_ThrowsArgumentException()
        {
            // Act and sssert
            Assert.Throws<ArgumentException>(() => _todoServicesProvider.ClearToDoList(-1));
        }

        [Test]
        public void ClearToDoList_PassedFirstListId_ReturnsUpdatedToDoListItem()
        {
            // Arrange
            var toDoList = _context.ToDoLists.First();

            // Act
            var clearedToDoList = _todoServicesProvider.ClearToDoList(toDoList.Id);

            // Assert
            Assert.IsTrue(clearedToDoList.ToDoEntries.Count == 0);
            Assert.IsTrue(!_context.ToDoLists.First(list => list.Id == toDoList.Id).ToDoEntries.Any());
        }

        [Test]
        public void ChangeVisiblityOfToDoList_PassedWrongId_ThrowsArgumentException()
        {
            // Act and assert
            Assert.Throws<ArgumentException>(() => _todoServicesProvider.ChangeVisiblityOfToDoList(-1));
        }

        [Test]
        public void ChangeVisiblityOfToDoList_PassedFirstListId_ReturnsUpdatedListObject()
        {
            // Act and assert
            Assert.IsFalse(_todoServicesProvider.ChangeVisiblityOfToDoList(1).IsVisible);
            Assert.IsFalse(_context.ToDoLists.First().IsVisible);
        }

        [Test]
        public void ChangeToDoItemsStatus_PassedWrongId_ThrowsArgumentException()
        {
            // Act and assert
            Assert.Throws<ArgumentException>(() => _todoServicesProvider.ChangeToDoItemsStatus(-1));
        }

        [Test]
        public void ChangeToDoItemsStatus_PassedFirstItemId_ReturnsNewStatusAsString()
        {
            // Act and assert
            Assert.AreEqual("InProgress", _todoServicesProvider.ChangeToDoItemsStatus(1));
            Assert.AreEqual("InProgress", _context.ToDoLists.First().ToDoEntries.First().Status.ToString());
        }

        [Test]
        public void RemoveAllToDoLists_NoArgs_SuccessfulExecution()
        {
            // Act
            _todoServicesProvider.RemoveAllToDoLists();

            // Assert
            Assert.IsTrue(!_context.ToDoLists.Any());
        }

        [Test]
        public void GetSummaryOfToDoLists_ReturnsSummaryObject()
        {
            // Act
            var summary = _todoServicesProvider.GetSummaryOfToDoLists();

            // Assert
            Assert.AreEqual(3, summary.TotalListsCount);
            Assert.AreEqual(1, summary.NotStartedListsCount);
            Assert.AreEqual(1, summary.InProgressListsCount);
            Assert.AreEqual(1, summary.CompletedListsCount);
            Assert.AreEqual(1, summary.HiddenToDoListsCount);
        }
                
        private void PopulateDatabaseFake()
        {
            _context.Database.EnsureDeleted();
            _context.AddRange(
                new ToDoList
                {
                    MainTitle = "Finish EPAM courses",
                    ToDoEntries = new List<ToDoEntry>
                    {
                        new ToDoEntry {OrdinalNumber = 1, Title = "Task #1", Description = "Learn C# Basics", DueDate = new DateTime(2022, 10, 1) },
                        new ToDoEntry {OrdinalNumber = 2,  Title = "Task #2", Description = "Learn C# Advanced", DueDate = new DateTime(2022, 10, 1) },
                        new ToDoEntry {OrdinalNumber = 3,  Title = "Task #3", Description = "Learn ASP.NET Core", DueDate = new DateTime(2022, 11, 30) },
                    },
                    IsVisible = true
                },

                new ToDoList
                {
                    MainTitle = "Get IELTS certificate",
                    ToDoEntries = new List<ToDoEntry>
                    {
                            new ToDoEntry {OrdinalNumber = 1, Title = "Task #1", Description = "Prepare to exam", DueDate = new DateTime(2022, 10, 1), Status = ToDoStatus.Completed },
                            new ToDoEntry {OrdinalNumber = 2, Title = "Task #2", Description = "Learn English", DueDate = new DateTime(2022, 11, 28), Status = ToDoStatus.Completed },
                            new ToDoEntry {OrdinalNumber = 3, Title = "Task #3", Description = "Pass exam", DueDate = new DateTime(2022, 12, 20), Status = ToDoStatus.Completed },
                    },
                    IsVisible = true
                },

                new ToDoList
                {
                    MainTitle = "Begin new life",
                    ToDoEntries = new List<ToDoEntry>
                    {
                            new ToDoEntry {OrdinalNumber = 1, Title = "Task #1", Description = "Move to CA", DueDate = new DateTime(2022, 11, 20) },
                            new ToDoEntry {OrdinalNumber = 2, Title = "Task #2", Description = "Find a job", DueDate = new DateTime(2022, 11, 29) },
                            new ToDoEntry {OrdinalNumber = 3, Title = "Task #3", Description = "Buy a flat", DueDate = new DateTime(2022, 11, 30), Status = ToDoStatus.InProgress },
                    },
                    IsVisible = false
                });
            
            _context.SaveChanges();
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