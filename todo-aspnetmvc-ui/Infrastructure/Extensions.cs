using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using todo_aspnetmvc_ui.Models.Services;
using todo_domain_entities;

namespace todo_aspnetmvc_ui.Infrastructure
{
    public static class Extensions
    {
        public static TAttribute GetAttribute<TAttribute>(this Enum enumValue)
            where TAttribute : Attribute
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<TAttribute>();
        }

        public static T GetValueFromName<T>(this string name)
            where T : Enum
        {
            var type = typeof(T);

            foreach (var field in type.GetFields())
            {
                if (Attribute.GetCustomAttribute(field, typeof(DisplayAttribute)) 
                    is DisplayAttribute attribute && attribute.Name == name)
                {
                    return (T)field.GetValue(null);
                }

                if (field.Name == name)
                {
                    return (T)field.GetValue(null);
                }
            }

            return default;
        }

        public static void EnsurePopulatedWithDemoData(this AppDbContext appDbContext)
        {
            if (!appDbContext.Set<ToDoList>().Any())
            {
                appDbContext.Set<ToDoList>().AddRange(
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
                            new ToDoEntry {OrdinalNumber = 1, Title = "Prepare to exam", DueDate = new DateTime(2022, 6, 1) },
                            new ToDoEntry {OrdinalNumber = 2, Title = "Learn English", DueDate = new DateTime(2022, 8, 1) },
                            new ToDoEntry {OrdinalNumber = 3, Title = "Pass exam", DueDate = new DateTime(2022, 8, 2) }
                        }
                    },

                    new ToDoList
                    {
                        MainTitle = "Become fullstack web-developer",
                        ToDoEntries = new List<ToDoEntry>
                        {
                            new ToDoEntry {OrdinalNumber = 1, Title = "Examine web-dev principles", DueDate = new DateTime(2022, 2, 1) },
                            new ToDoEntry {OrdinalNumber = 2, Title = "Learn HTML & CSS", DueDate = new DateTime(2022, 2, 15) },
                            new ToDoEntry {OrdinalNumber = 3, Title = "Learn JavaScript & jQuery", DueDate = new DateTime(2022, 3, 15) },
                            new ToDoEntry {OrdinalNumber = 4, Title = "Learn SQL & DBMS", DueDate = new DateTime(2022, 4, 1) }
                        }
                    },

                    new ToDoList
                    {
                        MainTitle = "Plans for the weekend",
                        ToDoEntries = new List<ToDoEntry>
                        {
                            new ToDoEntry {OrdinalNumber = 1, Title = "Go to shopping mall", DueDate = new DateTime(2022, 12, 11, 10, 0, 0) },
                            new ToDoEntry {OrdinalNumber = 2, Title = "Go to the cinema", DueDate = new DateTime(2022, 12, 11, 11, 0, 0) },
                            new ToDoEntry {OrdinalNumber = 3, Title = "Have a lunch", DueDate = new DateTime(2022, 12, 11, 13, 0, 0) },
                            new ToDoEntry {OrdinalNumber = 4, Title = "Meet with friends", DueDate = new DateTime(2022, 12, 11, 14, 0, 0)},
                            new ToDoEntry {OrdinalNumber = 5, Title = "Go to the home", DueDate = new DateTime(2022, 12, 11, 17, 0, 0)},
                            new ToDoEntry {OrdinalNumber = 6, Title = "Spend time with a family", DueDate = new DateTime(2022, 12, 11, 22, 0, 0)}
                        }
                    },

                    new ToDoList
                    {
                        MainTitle = "Begin new life",
                        ToDoEntries = new List<ToDoEntry>
                        {
                            new ToDoEntry {OrdinalNumber = 1, Title = "Move to CA", DueDate = new DateTime(2022, 2, 1) },
                            new ToDoEntry {OrdinalNumber = 2, Title = "Find a job", DueDate = new DateTime(2022, 3, 1) },
                            new ToDoEntry {OrdinalNumber = 3, Title = "Buy a flat", DueDate = new DateTime(2022, 8, 1) },
                            new ToDoEntry {OrdinalNumber = 4, Title = "Buy a car", DueDate = new DateTime(2022, 12, 1) }
                        }
                    },

                    new ToDoList
                    {
                        MainTitle = "Cook a plov",
                        ToDoEntries = new List<ToDoEntry>
                        {
                            new ToDoEntry {OrdinalNumber = 1, Title = "Buy groceries", Description = "A rice, carrot, oil, meat", DueDate = DateTime.Today.AddHours(16) },
                            new ToDoEntry {OrdinalNumber = 2, Title = "Find the recipe", Description = "Google plov's recipe", DueDate = DateTime.Today.AddHours(16.5) },
                            new ToDoEntry {OrdinalNumber = 3, Title = "Prepare the ingredients", Description = "Cut up the carrot, wash a rice and meat", DueDate = DateTime.Today.AddHours(17) },
                            new ToDoEntry {OrdinalNumber = 4, Title = "Cook a plov", Description = "Cook and follow a recipe", DueDate = DateTime.Today.AddHours(19.5) }
                        }
                    }
                );

                appDbContext.SaveChanges();
            }
        }
    }
}
