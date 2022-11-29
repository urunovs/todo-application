using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using todo_aspnetmvc_ui.Models.Services;
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
                yield return new TestCaseData(
                    new ToDoList
                    {
                        ToDoEntries = new List<ToDoEntry> //OrdinalNumber of ToDoEntry items is not initializing
                        {
                            new ToDoEntry { OrdinalNumber = 1, Title = "Stage #1", Description = "Buy ingrtdients", DueDate = new DateTime(2022, 2, 1) },
                            new ToDoEntry { OrdinalNumber = 2, Title = "Stage #2", Description = "Cook anything", DueDate = new DateTime(2022, 2, 2) },
                            new ToDoEntry { OrdinalNumber = 3, Title = "Final stage #3", Description = "Repeat Stage #1 and Stage #2", DueDate = new DateTime(2022, 2, 3) }
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
                        MainTitle = "Learn to cook",
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
                        MainTitle = "Become a web-frontend developer",
                        ToDoEntries = new List<ToDoEntry>
                        {
                            new ToDoEntry { OrdinalNumber = 1, Title = "Task #1", Description = "Learn HTML and CSS", DueDate = new DateTime(2022, 10, 1) },
                            new ToDoEntry { OrdinalNumber = 2, Title = "Task #2", Description = "Learn JavaScript", DueDate = new DateTime(2022, 11, 1) }
                        }
                    });

                yield return new TestCaseData(
                    new ToDoList
                    {
                        MainTitle = "Become a superhero"
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
                yield return new TestCaseData(
                    new ToDoList
                    {
                        Id = 3
                    },
                    new ToDoList
                    {
                        //MainTitle is empty string
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
                        MainTitle = "Get skills of development", // Updated data
                    });

                yield return new TestCaseData(
                    3,
                    new ToDoList
                    {
                        MainTitle = "Get IELTS certificate",
                        IsVisible = false, // Updated data
                    });
            }
        }

        /// <summary>
        /// Gets test cases for AddToDoItemToList_ValidArgs_ReturnsAddedInstance method.
        /// </summary>
        public static IEnumerable<TestCaseData> AddToDoItemToListValidArgsReturnsAddedInstanceTestCases
        {
            get
            {
                yield return new TestCaseData(
                        new ToDoEntry { OrdinalNumber = 4, Title = "Task #4", Description = "Move to success", DueDate = new DateTime(2022, 2, 1) },
                        3);
            }
        }
    }
}
