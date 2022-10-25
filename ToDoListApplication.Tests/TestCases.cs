using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using ToDoListDomainModel;

namespace ToDoListApplication.Tests
{
    /// <summary>
    /// Test cases class.
    /// </summary>
    public static class TestCases
    {
        /// <summary>
        /// Gets test cases for AddToDoList_ValidArg_ReturnInsertedInstance method.
        /// </summary>
        public static IEnumerable<TestCaseData> AddToDoListValidArgNoReturnTestCases
        {
            get
            {
                yield return new TestCaseData(
                    new ToDoList
                    {
                        PrimaryGoal = "Finish EPAM courses",
                        ToDoEntries = new List<ToDoEntry>
                        {
                            new ToDoEntry { Title = "Task #1", Description = "Learn C# Basics", DueDate = new DateTime(2022, 10, 1) },
                            new ToDoEntry { Title = "Task #2", Description = "Learn C# Advanced", DueDate = new DateTime(2022, 10, 1) },
                            new ToDoEntry { Title = "Task #3", Description = "Learn ASP.NET Core", DueDate = new DateTime(2022, 11, 9) },
                        },
                    });

                yield return new TestCaseData(
                    new ToDoList
                    {
                        PrimaryGoal = "Get IELTS certificate",
                        ToDoEntries = new List<ToDoEntry>
                        {
                            new ToDoEntry { Title = "Task #1", Description = "Prepare to exam", DueDate = new DateTime(2022, 6, 1) },
                            new ToDoEntry { Title = "Task #2", Description = "Learn English", DueDate = new DateTime(2022, 8, 1) },
                            new ToDoEntry { Title = "Task #3", Description = "Pass exam", DueDate = new DateTime(2022, 8, 2) },
                        },
                    });

                yield return new TestCaseData(
                    new ToDoList
                    {
                        PrimaryGoal = "Begin new life",
                        ToDoEntries = new List<ToDoEntry>
                        {
                            new ToDoEntry { Title = "Task #1", Description = "Move to CA", DueDate = new DateTime(2022, 2, 1) },
                            new ToDoEntry { Title = "Task #2", Description = "Find a job", DueDate = new DateTime(2022, 3, 1) },
                            new ToDoEntry { Title = "Task #3", Description = "Buy a flat", DueDate = new DateTime(2022, 8, 1) },
                        },
                    });
            }
        }

        /// <summary>
        /// Gets test cases for RemoveToDoList_ValidArg_NoReturn method.
        /// </summary>
        public static IEnumerable<TestCaseData> RemoveToDoListValidArgNoReturnTestCases
        {
            get
            {
                yield return new TestCaseData(
                    new ToDoList
                    {
                        PrimaryGoal = "Finish EPAM courses",
                        ToDoEntries = new List<ToDoEntry>
                        {
                            new ToDoEntry { Title = "Task #1", Description = "Learn C# Basics", DueDate = new DateTime(2022, 10, 1) },
                            new ToDoEntry { Title = "Task #2", Description = "Learn C# Advanced", DueDate = new DateTime(2022, 10, 1) },
                            new ToDoEntry { Title = "Task #3", Description = "Learn ASP.NET Core", DueDate = new DateTime(2022, 11, 9) },
                        },
                    });

                yield return new TestCaseData(
                    new ToDoList
                    {
                        PrimaryGoal = "Get IELTS certificate",
                        ToDoEntries = new List<ToDoEntry>
                        {
                            new ToDoEntry { Title = "Task #1", Description = "Prepare to exam", DueDate = new DateTime(2022, 6, 1) },
                            new ToDoEntry { Title = "Task #2", Description = "Learn English", DueDate = new DateTime(2022, 8, 1) },
                            new ToDoEntry { Title = "Task #3", Description = "Pass exam", DueDate = new DateTime(2022, 8, 2) },
                        },
                    });
            }
        }

        /// <summary>
        /// Gets test cases for ToDoList visiblity changing testing methods.
        /// </summary>
        public static IEnumerable<TestCaseData> NullAndInvalidDataThrowsArgumentExceptionTestCases
        {
            get
            {
                yield return null;

                yield return new TestCaseData(
                    new ToDoList
                    {
                        PrimaryGoal = "Begin new life",
                        ToDoEntries = new List<ToDoEntry>
                        {
                            new ToDoEntry { Title = "Task #1", Description = "Move to CA", DueDate = new DateTime(2022, 2, 1) },
                            new ToDoEntry { Title = "Task #2", Description = "Find a job", DueDate = new DateTime(2022, 3, 1) },
                            new ToDoEntry { Title = "Task #3", Description = "Buy a flat", DueDate = new DateTime(2022, 8, 1) },
                        },
                    });
            }
        }

        /// <summary>
        /// Gets test cases for ModifyToDoList_NullAndInvalidArgs_ThrowsArgumentException method.
        /// </summary>
        public static IEnumerable<TestCaseData> ModifyToDoListNullAndInvalidArgsThrowsArgumentExceptionTestCases
        {
            get
            {
                yield return new TestCaseData(null, null);
                yield return new TestCaseData(
                    new ToDoList
                    {
                        PrimaryGoal = "Begin new life",
                        ToDoEntries = new List<ToDoEntry>
                        {
                            new ToDoEntry { Title = "Task #1", Description = "Move to CA", DueDate = new DateTime(2022, 2, 1) },
                            new ToDoEntry { Title = "Task #2", Description = "Find a job", DueDate = new DateTime(2022, 3, 1) },
                            new ToDoEntry { Title = "Task #3", Description = "Buy a flat", DueDate = new DateTime(2022, 8, 1) },
                        },
                    },
                    null);
                yield return new TestCaseData(
                    null,
                    new ToDoList
                    {
                        PrimaryGoal = "Begin new life",
                        ToDoEntries = new List<ToDoEntry>
                        {
                            new ToDoEntry { Title = "Task #1", Description = "Move to CA", DueDate = new DateTime(2022, 2, 1) },
                            new ToDoEntry { Title = "Task #2", Description = "Find a job", DueDate = new DateTime(2022, 3, 1) },
                            new ToDoEntry { Title = "Final task", Description = "Buy a flat", DueDate = new DateTime(2022, 8, 1) },
                        },
                    });
                yield return new TestCaseData(
                    new ToDoList
                    {
                        PrimaryGoal = "Begin new life",
                        ToDoEntries = new List<ToDoEntry>
                        {
                            new ToDoEntry { Title = "Task #1", Description = "Move to CA", DueDate = new DateTime(2022, 2, 1) },
                            new ToDoEntry { Title = "Task #2", Description = "Find a job", DueDate = new DateTime(2022, 3, 1) },
                            new ToDoEntry { Title = "Task #3", Description = "Buy a flat", DueDate = new DateTime(2022, 8, 1) },
                        },
                    },
                    new ToDoList
                    {
                        PrimaryGoal = "Begin new life",
                        ToDoEntries = new List<ToDoEntry>
                        {
                            new ToDoEntry { Title = "Task #1", Description = "Move to CA", DueDate = new DateTime(2022, 2, 1) },
                            new ToDoEntry { Title = "Task #2", Description = "Find a job", DueDate = new DateTime(2022, 3, 1) },
                            new ToDoEntry { Title = "Final task", Description = "Buy a flat", DueDate = new DateTime(2022, 8, 1) },
                        },
                    });
            }
        }

        /// <summary>
        /// Gets test cases for ModifyToDoList_NoArgs_ReturnUpdatedInstance method.
        /// </summary>
        public static IEnumerable<TestCaseData> ModifyToDoListValidArgsReturnUpdatedInstanceTestCases
        {
            get
            {
                yield return new TestCaseData(
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
                        PrimaryGoal = "Get skills of development", // Updated data
                        ToDoEntries = new List<ToDoEntry>
                        {
                            new ToDoEntry { Title = "Task #1", Description = "Learn C# Basics", DueDate = new DateTime(2022, 10, 1) },
                            new ToDoEntry { Title = "Task #2", Description = "Learn C# Advanced", DueDate = new DateTime(2022, 10, 1) },
                            new ToDoEntry { Title = "Task #3", Description = "Learn ASP.NET Core", DueDate = new DateTime(2022, 11, 9) },
                        },
                    });

                yield return new TestCaseData(
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
                    new ToDoList
                    {
                        PrimaryGoal = "Get IELTS certificate",
                        ToDoEntries = new List<ToDoEntry>
                        {
                            new ToDoEntry { Title = "Task #1", Description = "Prepare to exam", DueDate = new DateTime(2022, 6, 1) },
                            new ToDoEntry { Title = "Task #2", Description = "Learn English", DueDate = new DateTime(2022, 8, 1) },
                            new ToDoEntry { Title = "Final task", Description = "Pass exams", DueDate = new DateTime(2022, 8, 15) }, // Updated data
                        },
                    });
            }
        }

        /// <summary>
        /// Gets test cases for AssignToDoEntriesToList_NullAndInvalidArgs_ThrowsArgumentException method.
        /// </summary>
        public static IEnumerable<TestCaseData> AssignToDoEntriesToListNullAndInvalidArgsThrowsArgumentExceptionTestCases
        {
            get
            {
                yield return new TestCaseData(null, null);

                yield return new TestCaseData(
                    new List<ToDoEntry>
                    {
                        new ToDoEntry { Title = "Task #1", Description = "Move to CA", DueDate = new DateTime(2022, 2, 1) },
                        new ToDoEntry { Title = "Task #2", Description = "Find a job", DueDate = new DateTime(2022, 3, 1) },
                        new ToDoEntry { Title = "Task #3", Description = "Buy a flat", DueDate = new DateTime(2022, 8, 1) },
                    },
                    null);

                yield return new TestCaseData(
                    null,
                    new ToDoList
                    {
                        PrimaryGoal = "Begin new life",
                        ToDoEntries = new List<ToDoEntry>
                        {
                            new ToDoEntry { Title = "Task #1", Description = "Move to CA", DueDate = new DateTime(2022, 2, 1) },
                            new ToDoEntry { Title = "Task #2", Description = "Find a job", DueDate = new DateTime(2022, 3, 1) },
                            new ToDoEntry { Title = "Final task", Description = "Buy a flat", DueDate = new DateTime(2022, 8, 1) },
                        },
                    });

                yield return new TestCaseData(
                    new List<ToDoEntry>
                    {
                        new ToDoEntry { Title = "Task #1", Description = "Move to CA", DueDate = new DateTime(2022, 2, 1) },
                        new ToDoEntry { Title = "Task #2", Description = "Find a job", DueDate = new DateTime(2022, 3, 1) },
                        new ToDoEntry { Title = "Task #3", Description = "Buy a flat", DueDate = new DateTime(2022, 8, 1) },
                    },
                    new ToDoList
                    {
                        PrimaryGoal = "Begin new life",
                        ToDoEntries = new List<ToDoEntry>
                        {
                            new ToDoEntry { Title = "Task #1", Description = "Move to CA", DueDate = new DateTime(2022, 2, 1) },
                            new ToDoEntry { Title = "Task #2", Description = "Find a job", DueDate = new DateTime(2022, 3, 1) },
                            new ToDoEntry { Title = "Final task", Description = "Buy a flat", DueDate = new DateTime(2022, 8, 1) },
                        },
                    });
            }
        }

        /// <summary>
        /// Gets test cases for AssignToDoEntriesToList_ValidArgs_NoReturn method.
        /// </summary>
        public static IEnumerable<TestCaseData> AssignToDoEntriesToListValidArgsNoReturnTestCases
        {
            get
            {
                yield return new TestCaseData(
                    new List<ToDoEntry>
                    {
                        new ToDoEntry { Title = "Task #1", Description = "Move to CA", DueDate = new DateTime(2022, 2, 1) },
                        new ToDoEntry { Title = "Task #2", Description = "Find a job", DueDate = new DateTime(2022, 3, 1) },
                        new ToDoEntry { Title = "Task #3", Description = "Buy a flat", DueDate = new DateTime(2022, 8, 1) },
                    },
                    new ToDoList
                    {
                        PrimaryGoal = "Finish EPAM courses",
                        ToDoEntries = new List<ToDoEntry>
                        {
                            new ToDoEntry { Title = "Task #1", Description = "Learn C# Basics", DueDate = new DateTime(2022, 10, 1) },
                            new ToDoEntry { Title = "Task #2", Description = "Learn C# Advanced", DueDate = new DateTime(2022, 10, 1) },
                            new ToDoEntry { Title = "Task #3", Description = "Learn ASP.NET Core", DueDate = new DateTime(2022, 11, 9) },
                        },
                    });
            }
        }

        /// <summary>
        /// Gets test cases for ModifyToDoEntry_NullAndInvalidArgs_ThrowsArgumentException method.
        /// </summary>
        public static IEnumerable<TestCaseData> ModifyToDoEntryToListNullAndInvalidArgsThrowsArgumentExceptionTestCases
        {
            get
            {
                yield return new TestCaseData(null, null);

                yield return new TestCaseData(
                        new ToDoEntry { Title = "Task #1", Description = "Move to CA", DueDate = new DateTime(2022, 2, 1) },
                        null);

                yield return new TestCaseData(
                        null,
                        new ToDoEntry { Title = "Task #1", Description = "Move to CA", DueDate = new DateTime(2022, 2, 1) });

                yield return new TestCaseData(
                        new ToDoEntry { Title = "Task #1", Description = "Move to CA", DueDate = new DateTime(2022, 2, 1) },
                        new ToDoEntry { Title = "Task #1", Description = "Move to NY", DueDate = new DateTime(2022, 2, 1) });
            }
        }

        /// <summary>
        /// Gets test cases for ModifyToDoEntry_ValidArgs_ReturnUpdatedInstance method.
        /// </summary>
        public static IEnumerable<TestCaseData> ModifyToDoEntryValidArgsReturnUpdatedInstanceTestCases
        {
            get
            {
                yield return new TestCaseData(
                        new ToDoEntry { Title = "Task #1", Description = "Learn C# Basics", DueDate = new DateTime(2022, 10, 1) },
                        new ToDoEntry { Title = "Task #1", Description = "Learn IT Fundamentals", DueDate = new DateTime(2022, 10, 1) });
            }
        }

        /// <summary>
        /// Gets test cases for SetToDoEntryStatus_NullAndInvalidArgs_ThrowsArgumentException method.
        /// </summary>
        public static IEnumerable<TestCaseData> SetToDoEntryStatusNullAndInvalidArgsThrowsArgumentExceptionTestCases
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
        public static IEnumerable<TestCaseData> SetToDoEntryStatusValidArgsReturnUpdatedInstanceTestCases
        {
            get
            {
                yield return new TestCaseData(
                        new ToDoEntry 
                        { 
                            Title = "Task #1", 
                            Description = "Learn C# Basics", 
                            DueDate = new DateTime(2022, 10, 1),
                            ToDoList = new ToDoList { PrimaryGoal = "Finish EPAM courses" },
                        },
                        ToDoStatus.Completed);

                yield return new TestCaseData(
                        new ToDoEntry 
                        { 
                            Title = "Task #1", 
                            Description = "Prepare to exam", 
                            DueDate = new DateTime(2022, 6, 1),
                            ToDoList = new ToDoList { PrimaryGoal = "Get IELTS certificate" },
                        },
                        ToDoStatus.Completed);
            }
        }
    }
}
