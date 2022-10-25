using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using todo_domain_entities;

namespace ToDoListApplication.Tests
{
    /// <summary>
    /// Test cases class.
    /// </summary>
    public static class TestCases
    {
        /// <summary>
        /// Gets test cases for AddToDoList_PassedWrongArgs_ThrowsArgumentException method.
        /// </summary>
        public static IEnumerable<TestCaseData> AddToDoListPassedWrongArgsThrowsArgumentExceptionTestCases
        {
            get
            {
                yield return null;

                yield return new TestCaseData(
                    new ToDoList
                    {
                        PrimaryPurpose = "Learn to cook",
                        ToDoEntries = new List<ToDoEntry> //OrdinalNumber of ToDoEntry items is not initializing
                        {
                            new ToDoEntry { Title = "Stage #1", Description = "Buy ingrtdients", DueDate = new DateTime(2022, 2, 1) },
                            new ToDoEntry { Title = "Stage #2", Description = "Cook anything", DueDate = new DateTime(2022, 2, 2) },
                            new ToDoEntry { Title = "Final stage #3", Description = "Repeat Stage #1 and Stage #2", DueDate = new DateTime(2022, 2, 3) }
                        }
                    });
            }
        }

        /// <summary>
        /// Gets test cases for AddToDoList_PassedValidArgs_SuccessfulExecution method.
        /// </summary>
        public static IEnumerable<TestCaseData> AddToDoListPassedValidArgsSuccessfulExecutionTestCases
        {
            get
            {
                yield return new TestCaseData(
                    new ToDoList
                    {
                        PrimaryPurpose = "Learn to cook",
                        ToDoEntries = new List<ToDoEntry>
                        {
                            new ToDoEntry {OrdinalNumber = 1, Title = "Stage #1", Description = "Buy ingrtdients", DueDate = new DateTime(2022, 2, 1) },
                            new ToDoEntry {OrdinalNumber = 2, Title = "Stage #2", Description = "Cook anything", DueDate = new DateTime(2022, 2, 2) },
                            new ToDoEntry {OrdinalNumber = 3, Title = "Final stage #3", Description = "Repeat Stage #1 and Stage #2", DueDate = new DateTime(2022, 2, 3) }
                        }
                    });

                yield return new TestCaseData(
                    new ToDoList
                    {
                        PrimaryPurpose = "Become a web-frontend developer",
                        ToDoEntries = new List<ToDoEntry>
                        {
                            new ToDoEntry { OrdinalNumber = 1, Title = "Task #1", Description = "Learn HTML and CSS", DueDate = new DateTime(2022, 10, 1) },
                            new ToDoEntry { OrdinalNumber = 2, Title = "Task #2", Description = "Learn JavaScript", DueDate = new DateTime(2022, 11, 1) }
                        }
                    });

                yield return new TestCaseData(
                    new ToDoList
                    {
                        PrimaryPurpose = "Become a superhero"
                    });
            }
        }

        /// <summary>
        /// Gets test cases for RemoveToDoList_PassedInvalidArgs_ThrowsArgumentException method.
        /// </summary>
        public static IEnumerable<TestCaseData> RemoveToDoListPassedInvalidArgsThrowsArgumentExceptionTestCases
        {
            get
            {
                yield return null;

                yield return new TestCaseData(
                    new ToDoList
                    {
                        Id = -1,
                        PrimaryPurpose = "Get IELTS certificate"
                    });
            }
        }

        /// <summary>
        /// Gets test cases for ModifyToDoList_InvalidArgs_ThrowsArgumentException method.
        /// </summary>
        public static IEnumerable<TestCaseData> ModifyToDoListInvalidArgsThrowsArgumentExceptionTestCases
        {
            get
            {
                yield return new TestCaseData(null, null);

                yield return new TestCaseData(
                    new ToDoList
                    {
                        PrimaryPurpose = "Begin new life",
                        ToDoEntries = new List<ToDoEntry>
                        {
                            new ToDoEntry {OrdinalNumber = 1, Title = "Task #1", Description = "Move to CA", DueDate = new DateTime(2022, 2, 1) },
                            new ToDoEntry {OrdinalNumber = 2, Title = "Task #2", Description = "Find a job", DueDate = new DateTime(2022, 3, 1) },
                            new ToDoEntry {OrdinalNumber = 3, Title = "Task #3", Description = "Buy a flat", DueDate = new DateTime(2022, 8, 1) }
                        },
                    },
                    null);

                yield return new TestCaseData(
                    null,
                    new ToDoList
                    {
                        PrimaryPurpose = "Begin new life",
                        ToDoEntries = new List<ToDoEntry>
                        {
                            new ToDoEntry {OrdinalNumber = 1, Title = "Task #1", Description = "Move to CA", DueDate = new DateTime(2022, 2, 1) },
                            new ToDoEntry {OrdinalNumber = 2, Title = "Task #2", Description = "Find a job", DueDate = new DateTime(2022, 3, 1) },
                            new ToDoEntry {OrdinalNumber = 3, Title = "Task #3", Description = "Buy a flat", DueDate = new DateTime(2022, 8, 1) }
                        },
                    });

                yield return new TestCaseData(
                    new ToDoList
                    {
                        Id = 3,
                        PrimaryPurpose = "Begin new life",
                        ToDoEntries = new List<ToDoEntry>
                        {
                            new ToDoEntry {OrdinalNumber = 1, Title = "Task #1", Description = "Move to CA", DueDate = new DateTime(2022, 2, 1) },
                            new ToDoEntry {OrdinalNumber = 2, Title = "Task #2", Description = "Find a job", DueDate = new DateTime(2022, 3, 1) },
                            new ToDoEntry {OrdinalNumber = 3, Title = "Task #3", Description = "Buy a flat", DueDate = new DateTime(2022, 8, 1) }
                        }
                    },
                    new ToDoList
                    {
                        PrimaryPurpose = "Begin new life",
                        ToDoEntries = new List<ToDoEntry>
                        {
                            new ToDoEntry { Title = "Task #1", Description = "Move to CA", DueDate = new DateTime(2022, 2, 1) },
                            new ToDoEntry { Title = "Task #2", Description = "Find a job", DueDate = new DateTime(2022, 3, 1) },
                            new ToDoEntry { Title = "Final task", Description = "Buy a flat", DueDate = new DateTime(2022, 8, 1) }
                        }
                    });
            }
        }

        /// <summary>
        /// Gets test cases for ModifyToDoList_ValidArgs_ReturnsUpdatedInstance method.
        /// </summary>
        public static IEnumerable<TestCaseData> ModifyToDoListValidArgsReturnsUpdatedInstanceTestCases
        {
            get
            {
                yield return new TestCaseData(
                    2,
                    new ToDoList
                    {
                        PrimaryPurpose = "Get skills of development", // Updated data
                        ToDoEntries = new List<ToDoEntry>
                        {
                            new ToDoEntry { OrdinalNumber = 1, Title = "Task #1", Description = "Learn C# Basics", DueDate = new DateTime(2022, 10, 1) },
                            new ToDoEntry { OrdinalNumber = 2, Title = "Task #2", Description = "Learn C# Advanced", DueDate = new DateTime(2022, 10, 1) },
                            new ToDoEntry { OrdinalNumber = 3, Title = "Task #3", Description = "Learn ASP.NET Core", DueDate = new DateTime(2022, 11, 9) },
                        }
                    });

                yield return new TestCaseData(
                    3,
                    new ToDoList
                    {
                        PrimaryPurpose = "Get IELTS certificate",
                        ToDoEntries = new List<ToDoEntry>
                        {
                            new ToDoEntry { OrdinalNumber = 1, Title = "Task #1", Description = "Prepare to exam", DueDate = new DateTime(2022, 6, 1) },
                            new ToDoEntry { OrdinalNumber = 2, Title = "Task #2", Description = "Learn English", DueDate = new DateTime(2022, 8, 1) },
                            new ToDoEntry { OrdinalNumber = 3, Title = "Final task", Description = "Pass exams", DueDate = new DateTime(2022, 8, 15) }, // Updated data
                        }
                    });
            }
        }

        /// <summary>
        /// Gets test cases for AddToDoEntriesToList_InvalidArgs_ThrowsArgumentException method.
        /// </summary>
        public static IEnumerable<TestCaseData> AddToDoEntriesToListInvalidArgsThrowsArgumentException
        {
            get
            {
                yield return new TestCaseData(null, 1);

                yield return new TestCaseData(
                    new List<ToDoEntry>
                    {
                        new ToDoEntry { OrdinalNumber = 1, Title = "Task #1", Description = "Move to CA", DueDate = new DateTime(2022, 2, 1) },
                        new ToDoEntry { OrdinalNumber = 2, Title = "Task #2", Description = "Find a job", DueDate = new DateTime(2022, 3, 1) },
                        new ToDoEntry { OrdinalNumber = 3, Title = "Task #3", Description = "Buy a flat", DueDate = new DateTime(2022, 8, 1) }
                    },
                    -1);
            }
        }

        /// <summary>
        /// Gets test cases for AddToDoEntriesToList_ValidArgs_SuccessfulExecution method.
        /// </summary>
        public static IEnumerable<TestCaseData> AddToDoEntriesToListValidArgsSuccessfulExecutionTestCases
        {
            get
            {
                yield return new TestCaseData(
                    new List<ToDoEntry>
                    {
                        new ToDoEntry { OrdinalNumber = 1, Title = "Task #1", Description = "Move to CA", DueDate = new DateTime(2022, 2, 1) },
                        new ToDoEntry { OrdinalNumber = 2, Title = "Task #2", Description = "Find a job", DueDate = new DateTime(2022, 3, 1) },
                        new ToDoEntry { OrdinalNumber = 3, Title = "Task #3", Description = "Buy a flat", DueDate = new DateTime(2022, 8, 1) }
                    },
                    3);
            }
        }

        /// <summary>
        /// Gets test cases for ModifyToDoEntry_InvalidArgs_ThrowsArgumentException method.
        /// </summary>
        public static IEnumerable<TestCaseData> ModifyToDoEntryToListInvalidArgsThrowsArgumentExceptionTestCases
        {
            get
            {
                yield return new TestCaseData(null, null);

                yield return new TestCaseData(
                        new ToDoEntry { Id = 1, OrdinalNumber = 1, Title = "Task #1", Description = "Learn C# Basics", DueDate = new DateTime(2022, 10, 1) },
                        null);

                yield return new TestCaseData(
                        null,
                        new ToDoEntry { Id = 1, OrdinalNumber = 1, Title = "Task #1", Description = "Learn C# Basics", DueDate = new DateTime(2022, 9, 1) });

                yield return new TestCaseData(
                        new ToDoEntry { Id = 1, OrdinalNumber = 1, Title = "Task #1", Description = "Learn C# Basics", DueDate = new DateTime(2022, 10, 1) },
                        new ToDoEntry { Title = "Task #1", Description = "Move to NY", DueDate = new DateTime(2022, 2, 1) });
            }
        }

        /// <summary>
        /// Gets test cases for ModifyToDoEntry_ValidArgs_ReturnsUpdatedInstance method.
        /// </summary>
        public static IEnumerable<TestCaseData> ModifyToDoEntryValidArgsReturnsUpdatedInstanceTestCases
        {
            get
            {
                yield return new TestCaseData(
                        1,
                        new ToDoEntry { OrdinalNumber = 1, Title = "Task #1", Description = "Learn IT Fundamentals", DueDate = new DateTime(2022, 10, 1) });
            }
        }

        /// <summary>
        /// Gets test cases for RemoveToDoEntry_PassedInvalidArgs_ThrowsArgumentException method.
        /// </summary>
        public static IEnumerable<TestCaseData> RemoveToDoEntryPassedInvalidArgsThrowsArgumentExceptionTestCases
        {
            get
            {
                yield return null;

                yield return new TestCaseData(
                    new ToDoEntry
                    {
                        Id = -1
                    });
            }
        }

        /// <summary>
        /// Gets test cases for SetToDoEntryStatus_InvalidArgs_ThrowsArgumentException method.
        /// </summary>
        public static IEnumerable<TestCaseData> SetToDoEntryStatusInvalidArgsThrowsArgumentExceptionTestCases
        {
            get
            {
                yield return new TestCaseData(
                        new ToDoEntry { Title = "Task #1", Description = "Move to CA", DueDate = new DateTime(2022, 2, 1) },
                        ToDoStatus.Completed);

                yield return new TestCaseData(
                        null,
                        ToDoStatus.Completed);
            }
        }

        /// <summary>
        /// Gets test cases for SetToDoEntryStatus_ValidArgs_ReturnUpdatedInstance method.
        /// </summary>
        public static IEnumerable<TestCaseData> SetToDoEntryStatusValidArgsReturnsUpdatedInstanceTestCases
        {
            get
            {
                yield return new TestCaseData(
                        1,
                        1,
                        ToDoStatus.Completed);

                yield return new TestCaseData(
                        1,
                        1,
                        ToDoStatus.InProgress);
            }
        }
    }
}
