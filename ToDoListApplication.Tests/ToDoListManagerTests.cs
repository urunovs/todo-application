using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ToDoListDomainModel;

#pragma warning disable CA1707

namespace ToDoListApplication.Tests
{
    /// <summary>
    /// Class to test ToDoListDomainModel.ToDoListManager.
    /// </summary>
    [TestFixture]
    public sealed class ToDoListManagerTests : IDisposable
    {
        private readonly ToDoListManager toDoListManager = new ToDoListManager();

        /// <summary>
        /// Finalizes an instance of the <see cref="ToDoListManagerTests"/> class.
        /// Releases memory of toDoListManager field.
        /// </summary>
        ~ToDoListManagerTests()
        {
            this.Dispose();
        }

        /// <summary>
        /// Implementation of IDisposable.
        /// </summary>
        public void Dispose()
        {
            this.toDoListManager.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Testing AddToDoList method for Null.
        /// </summary>
        /// <param name="toDoList">ToDoList entity.</param>
        [TestCase(null)]
        public void AddToDoList_NullArg_ThrowsArgumentException(ToDoList toDoList)
        {
            // Act and assert
            Assert.Throws<ArgumentException>(() => this.toDoListManager.AddToDoList(toDoList));
        }

        /// <summary>
        /// Testing AddToDoList method for valid values.
        /// </summary>
        /// <param name="toDoList">ToDoList entity.</param>
        [TestCaseSource(typeof(TestCases), nameof(TestCases.AddToDoListValidArgNoReturnTestCases))]
        public void AddToDoList_ValidArg_ReturnInsertedInstance(ToDoList toDoList)
        {
            // Arange
            // Clearing database to prevent data collision
            this.toDoListManager.RemoveAllToDoLists();

            // Act and assert
            Assert.AreEqual(this.toDoListManager.AddToDoList(toDoList), this.toDoListManager.ToDoLists.First());
        }

        /// <summary>
        /// Testing RemoveToDoList method for Null and invalid object.
        /// </summary>
        /// <param name="toDoList">ToDoList entity.</param>
        [TestCaseSource(typeof(TestCases), nameof(TestCases.NullAndInvalidDataThrowsArgumentExceptionTestCases))]
        public void RemoveToDoList_NullAndInvalidArg_ThrowsArgumentException(ToDoList toDoList)
        {
            // Arange
            this.ArrangeGeneralData();

            // Act and assert
            Assert.Throws<ArgumentException>(() => this.toDoListManager.RemoveToDoList(toDoList));
        }

        /// <summary>
        /// Testing RemoveToDoList method for valid data.
        /// </summary>
        /// <param name="toDoList">ToDoList entity.</param>
        [TestCaseSource(typeof(TestCases), nameof(TestCases.RemoveToDoListValidArgNoReturnTestCases))]
        public void RemoveToDoList_ValidArg_NoReturn(ToDoList toDoList)
        {
            // Arange
            this.ArrangeGeneralData();
            var instanceToRemove = this.toDoListManager.ToDoLists.First(list => list.PrimaryGoal == toDoList.PrimaryGoal);

            // Act
            this.toDoListManager.RemoveToDoList(instanceToRemove);

            // Assert
            Assert.IsNull(this.toDoListManager.ToDoLists.FirstOrDefault(list => list.PrimaryGoal == toDoList.PrimaryGoal));
        }

        /// <summary>
        /// Testing HideToDoList method for Null and invalid object.
        /// </summary>
        /// <param name="toDoList">ToDoList entity.</param>
        [TestCaseSource(typeof(TestCases), nameof(TestCases.NullAndInvalidDataThrowsArgumentExceptionTestCases))]
        public void HideToDoList_NullAndInvalidArg_ThrowsArgumentException(ToDoList toDoList)
        {
            // Arange
            this.ArrangeGeneralData();

            // Act and assert
            Assert.Throws<ArgumentException>(() => this.toDoListManager.HideToDoList(toDoList));
        }

        /// <summary>
        /// Testing HideToDoList method for valid data.
        /// </summary>
        [Test]
        public void HideToDoList_NoArgs_NoReturn()
        {
            // Arange
            this.ArrangeGeneralData();

            var listToHide = this.toDoListManager.ToDoLists.First();

            // Act
            this.toDoListManager.HideToDoList(listToHide);

            // Assert
            Assert.IsTrue(!this.toDoListManager.ToDoLists.Contains(listToHide));
            Assert.IsTrue(this.toDoListManager.HidedToDoLists.Contains(listToHide));
        }

        /// <summary>
        /// Testing MakeVisibleToDoList method for Null and invalid object.
        /// </summary>
        /// <param name="toDoList">ToDoList entity.</param>
        [TestCaseSource(typeof(TestCases), nameof(TestCases.NullAndInvalidDataThrowsArgumentExceptionTestCases))]
        public void MakeVisibleToDoList_NullAndInvalidArg_ThrowsArgumentException(ToDoList toDoList)
        {
            // Arange
            this.ArrangeGeneralData();

            // Act and assert
            Assert.Throws<ArgumentException>(() => this.toDoListManager.MakeVisibleToDoList(toDoList));
        }

        /// <summary>
        /// Testing MakeVisibleToDoList method for valid data.
        /// </summary>
        [Test]
        public void MakeVisibleToDoList_NoArgs_NoReturn()
        {
            // Arange
            this.ArrangeGeneralData();
            var instanceToChangeVisiblity = this.toDoListManager.ToDoLists.First();
            this.toDoListManager.HideToDoList(instanceToChangeVisiblity);

            // Act
            this.toDoListManager.MakeVisibleToDoList(instanceToChangeVisiblity);

            // Assert
            Assert.IsTrue(this.toDoListManager.ToDoLists.Contains(instanceToChangeVisiblity));
            Assert.IsTrue(!this.toDoListManager.HidedToDoLists.Contains(instanceToChangeVisiblity));
        }

        /// <summary>
        /// Testing ModifyToDoList method for Null and invalid object.
        /// </summary>
        /// <param name="listToUpdate">Existing ToDoList entity.</param>
        /// /// <param name="updatedViewList">ToDoList entity with updated data.</param>
        [TestCaseSource(typeof(TestCases), nameof(TestCases.ModifyToDoListNullAndInvalidArgsThrowsArgumentExceptionTestCases))]
        public void ModifyToDoList_NullAndInvalidArgs_ThrowsArgumentException(ToDoList listToUpdate, ToDoList updatedViewList)
        {
            // Arange
            this.ArrangeGeneralData();

            // Act and assert
            Assert.Throws<ArgumentException>(() => this.toDoListManager.ModifyToDoList(listToUpdate, updatedViewList));
        }

        /// <summary>
        /// Testing ModifyToDoList method for valid data.
        /// </summary>
        /// <param name="listToUpdate">Existing ToDoList entity.</param>
        /// <param name="updatedViewList">ToDoList entity with updated data.</param>
        [TestCaseSource(typeof(TestCases), nameof(TestCases.ModifyToDoListValidArgsReturnUpdatedInstanceTestCases))]
        public void ModifyToDoList_ValidArgs_ReturnUpdatedInstance(ToDoList listToUpdate, ToDoList updatedViewList)
        {
            // Arange
            this.ArrangeGeneralData();
            var instanceToUpdate = this.toDoListManager.ToDoLists.First(list => list.PrimaryGoal == listToUpdate.PrimaryGoal);

            // Act
            this.toDoListManager.ModifyToDoList(instanceToUpdate, updatedViewList);

            if (updatedViewList == null)
            {
                return;
            }

            // Assert
            Assert.IsTrue(instanceToUpdate.PrimaryGoal == updatedViewList.PrimaryGoal &&
                          instanceToUpdate.IsVisible == updatedViewList.IsVisible &&
                          instanceToUpdate.ToDoEntries.SequenceEqual(updatedViewList.ToDoEntries));
        }

        /// <summary>
        /// Testing AssignToDoEntriesToList method for Null and invalid object.
        /// </summary>
        /// <param name="toDoEntries">ToDoEntries list.</param>
        /// /// <param name="toDoList">ToDoList entity.</param>
        [TestCaseSource(typeof(TestCases), nameof(TestCases.AssignToDoEntriesToListNullAndInvalidArgsThrowsArgumentExceptionTestCases))]
        public void AssignToDoEntriesToList_NullAndInvalidArgs_ThrowsArgumentException(List<ToDoEntry> toDoEntries, ToDoList toDoList)
        {
            // Arange
            this.ArrangeGeneralData();

            // Act and assert
            Assert.Throws<ArgumentException>(() => this.toDoListManager.AssignToDoEntriesToList(toDoEntries, toDoList));
        }

        /// <summary>
        /// Testing AssignToDoEntriesToList method for valid data.
        /// </summary>
        /// <param name="toDoEntries">ToDoEntries list.</param>
        /// /// <param name="toDoList">ToDoList entity.</param>
        [TestCaseSource(typeof(TestCases), nameof(TestCases.AssignToDoEntriesToListValidArgsNoReturnTestCases))]
        public void AssignToDoEntriesToList_ValidArgs_NoReturn(List<ToDoEntry> toDoEntries, ToDoList toDoList)
        {
            // Arange
            this.ArrangeGeneralData();
            var listToAssign = this.toDoListManager.ToDoLists.First(list => list.PrimaryGoal == toDoList.PrimaryGoal);

            // Act
            this.toDoListManager.AssignToDoEntriesToList(toDoEntries, listToAssign);

            // Assert
            Assert.IsTrue(listToAssign.ToDoEntries.SequenceEqual(toDoEntries));
        }

        /// <summary>
        /// Testing ModifyToDoEntry method for Null and invalid objects.
        /// </summary>
        /// <param name="toDoEntryToUpdate">Existing ToDoList entity.</param>
        /// <param name="updatedView">ToDoList entity with updated data.</param>
        [TestCaseSource(typeof(TestCases), nameof(TestCases.ModifyToDoEntryToListNullAndInvalidArgsThrowsArgumentExceptionTestCases))]
        public void ModifyToDoEntry_NullAndInvalidArgs_ThrowsArgumentException(ToDoEntry toDoEntryToUpdate, ToDoEntry updatedView)
        {
            // Arange
            this.ArrangeGeneralData();

            // Act and assert
            Assert.Throws<ArgumentException>(() => this.toDoListManager.ModifyToDoEntry(toDoEntryToUpdate, updatedView));
        }

        /// <summary>
        /// Testing ModifyToDoEntry method for valid data.
        /// </summary>
        /// <param name="toDoEntryToUpdate">Existing ToDoList entity.</param>
        /// <param name="updatedView">ToDoList entity with updated data.</param>
        [TestCaseSource(typeof(TestCases), nameof(TestCases.ModifyToDoEntryValidArgsReturnUpdatedInstanceTestCases))]
        public void ModifyToDoEntry_ValidArgs_ReturnUpdatedInstance(ToDoEntry toDoEntryToUpdate, ToDoEntry updatedView)
        {
            // Arange
            this.ArrangeGeneralData();
            var toUpdateInstance = this.toDoListManager.ToDoLists.Select(list => list.ToDoEntries.First(entry => entry.Title == toDoEntryToUpdate.Title))
                                                                 .First();

            if (updatedView == null)
            {
                return;
            }

            updatedView.ToDoList = toUpdateInstance.ToDoList;

            // Act
            var updatedInstance = this.toDoListManager.ModifyToDoEntry(toUpdateInstance, updatedView);

            // Assert
            Assert.IsTrue(updatedInstance.Title == updatedView.Title &&
                          updatedInstance.Description == updatedView.Description &&
                          updatedInstance.Status == updatedView.Status &&
                          updatedInstance.ToDoList.Equals(updatedView.ToDoList));
        }

        /// <summary>
        /// Testing SetToDoEntryStatus method for Null and invalid objects.
        /// </summary>
        /// <param name="toDoEntryToUpdate">Existing ToDoList entity.</param>
        /// <param name="status">ToDoStatus value.</param>
        [TestCaseSource(typeof(TestCases), nameof(TestCases.SetToDoEntryStatusNullAndInvalidArgsThrowsArgumentExceptionTestCases))]
        public void SetToDoEntryStatus_NullAndInvalidArgs_ThrowsArgumentException(ToDoEntry toDoEntryToUpdate, ToDoStatus status)
        {
            // Arange
            this.ArrangeGeneralData();

            // Act and assert
            Assert.Throws<ArgumentException>(() => this.toDoListManager.SetToDoEntryStatus(toDoEntryToUpdate, status));
        }

        /// <summary>
        /// Testing SetToDoEntryStatus method for valid data.
        /// </summary>
        /// <param name="toDoEntryToUpdate">Existing ToDoList entity.</param>
        /// <param name="status">ToDoStatus value.</param>
        [TestCaseSource(typeof(TestCases), nameof(TestCases.SetToDoEntryStatusValidArgsReturnUpdatedInstanceTestCases))]
        public void SetToDoEntryStatus_ValidArgs_ReturnUpdatedInstance(ToDoEntry toDoEntryToUpdate, ToDoStatus status)
        {
            // Arange
            this.ArrangeGeneralData();
            toDoEntryToUpdate = this.toDoListManager.ToDoLists.First(list => list.PrimaryGoal == toDoEntryToUpdate.ToDoList.PrimaryGoal)
                                                              .ToDoEntries.First(entry => entry.Title == toDoEntryToUpdate.Title);

            // Act
            var updatedInstance = this.toDoListManager.SetToDoEntryStatus(toDoEntryToUpdate, status);

            // Assert
            Assert.IsTrue(updatedInstance.Status == status);
        }

        /// <summary>
        /// Prepare database to test method.
        /// </summary>
        private void ArrangeGeneralData()
        {
            var todoLists = new List<ToDoList>
            {
                new ToDoList
                {
                    PrimaryGoal = "Finish EPAM courses",
                    ToDoEntries = new List<ToDoEntry>
                    {
                        new ToDoEntry { Title = "Task #1", Description = "Learn C# Basics", DueDate = new DateTime(2022, 10, 1) },
                        new ToDoEntry { Title = "Task #2", Description = "Learn C# Advanced", DueDate = new DateTime(2022, 10, 1) },
                        new ToDoEntry { Title = "Task #3", Description = "Learn ASP.NET Core", DueDate = new DateTime(2022, 11, 9) },
                    },
                },

                new ToDoList
                {
                    PrimaryGoal = "Get IELTS certificate",
                    ToDoEntries = new List<ToDoEntry>
                    {
                        new ToDoEntry { Title = "Task #1", Description = "Prepare to exam", DueDate = new DateTime(2022, 6, 1) },
                        new ToDoEntry { Title = "Task #2", Description = "Learn English", DueDate = new DateTime(2022, 8, 1) },
                        new ToDoEntry { Title = "Task #3", Description = "Pass exam", DueDate = new DateTime(2022, 8, 2) },
                    },
                },
            };

            // Clearing database to prevent data collision
            this.toDoListManager.RemoveAllToDoLists();

            // Adding new data into database
            todoLists.ForEach(list => this.toDoListManager.AddToDoList(list));
        }
    }
}
