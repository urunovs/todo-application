using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using todo_aspnetmvc_ui.Models.Services;

namespace todo_domain_entities
{

    /// <summary>
    /// ToDoEFCoreServicesProvider performing some actions with T0D0 lists against DB.
    /// </summary>
    public class ToDoEFCoreServicesProvider : IToDoServices, IDisposable
    {
        private readonly AppDbContext _appDbContext;

        public (IEnumerable<ToDoList>, int) GetToDoLists(int pageSize, int page)
        {
            var dbSet = _appDbContext.Set<ToDoList>();
            return GetFetchedToDoLists(pageSize, page, dbSet);
        }

        public (IEnumerable<ToDoList>, int) GetVisibleToDoLists(int pageSize, int page)
        {
            var dbSet = _appDbContext.Set<ToDoList>().Where(list => list.IsVisible);
            return GetFetchedToDoLists(pageSize, page, dbSet);
        }

        public (IEnumerable<ToDoList>, int) GetCompletedToDoLists(int pageSize, int page)
        {
            var dbSet = _appDbContext.Set<ToDoList>().Where(list => list.ToDoEntries.All(item => item.Status == ToDoStatus.Completed));
            return GetFetchedToDoLists(pageSize, page, dbSet);
        }

        public ToDoEFCoreServicesProvider(AppDbContext appDbContext)
        {
            this._appDbContext = appDbContext;
        }

        public ToDoList GetToDoListById(int id)
        {
            return _appDbContext.ToDoLists.Find(id);
        }

        public ToDoList AddToDoList(ToDoList toDoList)
        {
            ValidateItem(toDoList);

            toDoList = _appDbContext.Set<ToDoList>().Add(toDoList).Entity;
            _appDbContext.SaveChanges();

            return toDoList;
        }

        public void RemoveToDoList(int toDoListId)
        {
            var toDoList = _appDbContext.Set<ToDoList>().Find(toDoListId);

            if (toDoList == null)
            {
                throw new ArgumentException("No such ToDoList instance with a given Id", nameof(toDoListId));
            }

            _appDbContext.Set<ToDoList>().Remove(toDoList);
            _appDbContext.SaveChanges();
        }

        public ToDoList ModifyToDoList(int toDoListId, ToDoList updatedView)
        {
            ValidateItem(updatedView);

            var toDoListToUpdate = _appDbContext.Set<ToDoList>().Find(toDoListId);

            if (toDoListToUpdate == null)
            {
                throw new ArgumentException("No such ToDoList instance with a given Id", nameof(toDoListId));
            }

            toDoListToUpdate.MainTitle = updatedView.MainTitle;
            toDoListToUpdate.IsVisible = updatedView.IsVisible;

            _appDbContext.SaveChanges();

            return toDoListToUpdate;
        }

        public ToDoList ChangeVisiblityOfToDoList(int toDoListId)
        {
            var toDoListToUpdate = _appDbContext.Set<ToDoList>().Find(toDoListId);

            if (toDoListToUpdate == null)
            {
                throw new ArgumentException("No such ToDoList instance with a given Id", nameof(toDoListId));
            }

            toDoListToUpdate.IsVisible = !toDoListToUpdate.IsVisible;

            _appDbContext.SaveChanges();

            return toDoListToUpdate;
        }

        public ToDoEntry AddToDoItemToList(ToDoEntry toDoEntryToInsert, int toDoListId)
        {
            if (toDoEntryToInsert == null)
            {
                throw new ArgumentNullException(nameof(toDoEntryToInsert), "Passed object is null");
            }

            ValidateItem(toDoEntryToInsert);

            var toDoList = _appDbContext.Set<ToDoList>().Find(toDoListId);

            if (toDoList == null)
            {
                throw new ArgumentException("No such ToDoList instance with a given Id", nameof(toDoListId));
            }

            toDoEntryToInsert.ToDoList = toDoList;
            _appDbContext.Set<ToDoEntry>().Add(toDoEntryToInsert);

            if (toDoList.ToDoEntries.Any(item => item.OrdinalNumber == toDoEntryToInsert.OrdinalNumber))
            {
                ReorderItems(toDoEntryToInsert.ToDoList, toDoEntryToInsert);
            }

            _appDbContext.SaveChanges();
            toDoList.ToDoEntries.Add(toDoEntryToInsert);

            return toDoEntryToInsert;
        }

        public ToDoEntry ModifyToDoEntry(int toDoEntryId, ToDoEntry updatedView)
        {
            ValidateItem(updatedView);

            var toDoEntryToUpdate = _appDbContext.Set<ToDoEntry>().Find(toDoEntryId);

            if (toDoEntryToUpdate == null)
            {
                throw new ArgumentException("No such ToDoEntry instance with a given Id", nameof(toDoEntryId));
            }

            toDoEntryToUpdate.Title = updatedView.Title;
            toDoEntryToUpdate.Description = updatedView.Description;
            toDoEntryToUpdate.DueDate = updatedView.DueDate;
            toDoEntryToUpdate.Status = updatedView.Status;
            toDoEntryToUpdate.OrdinalNumber = updatedView.OrdinalNumber;
            toDoEntryToUpdate.Notes = updatedView.Notes;

            if (toDoEntryToUpdate.ToDoList.ToDoEntries.Count(item => item.OrdinalNumber == updatedView.OrdinalNumber) > 1)
            {
                ReorderItems(toDoEntryToUpdate.ToDoList, toDoEntryToUpdate);
            }

            _appDbContext.SaveChanges();

            return toDoEntryToUpdate;
        }

        public void RemoveAllToDoLists()
        {
            _appDbContext.Set<ToDoList>().RemoveRange(_appDbContext.Set<ToDoList>());
            _appDbContext.SaveChanges();
        }

        public ToDoList ClearToDoList(int toDoListId)
        {
            var toDoList = _appDbContext.Set<ToDoList>().Find(toDoListId);

            if (toDoList == null)
            {
                throw new ArgumentException("No such ToDoList wtih a given Id", nameof(toDoListId));
            }

            toDoList.ToDoEntries.Clear();
            _appDbContext.SaveChanges();

            return toDoList;
        }

        public string ChangeToDoItemsStatus(int toDoItemId)
        {
            var toDoEntry = _appDbContext.Set<ToDoEntry>().Find(toDoItemId);

            if (toDoEntry == null)
            {
                throw new ArgumentException("No such ToDoEntry instance with a given Id", nameof(toDoItemId));
            }

            if (toDoEntry.Status == ToDoStatus.NotStarted)
            {
                toDoEntry.Status = ToDoStatus.InProgress;
            }
            else
            {
                toDoEntry.Status = ToDoStatus.Completed;
            }

            _appDbContext.SaveChanges();
            return toDoEntry.Status.ToString();
        }

        public void RemoveToDoEntry(int toDoEntryId)
        {
            var toDoEntry = _appDbContext.Set<ToDoEntry>().Find(toDoEntryId);

            if (toDoEntry == null)
            {
                throw new ArgumentException("No such ToDoEntry instance with a given Id", nameof(toDoEntryId));
            }

            toDoEntry.ToDoList.ToDoEntries.Remove(toDoEntry);
            _appDbContext.Set<ToDoEntry>().Remove(toDoEntry);
            ReorderItems(toDoEntry.ToDoList);
            _appDbContext.SaveChanges();
        }

        public (IEnumerable<IGrouping<ToDoList, ToDoEntry>>, int) GetGroupedToDoItemsByDueDate(
            ToDoItemsDueDate itemsDueDate,
            int pageSize,
            int page)
        {
            var dbset = _appDbContext.Set<ToDoEntry>();
            IQueryable<ToDoEntry> filterQuery;

            switch (itemsDueDate)
            {
                case ToDoItemsDueDate.DueDateTomorrow:
                    filterQuery = dbset.Where((entry) => entry.Status != ToDoStatus.Completed
                                                   && entry.DueDate.Value.Date == DateTime.Today.AddDays(1));
                    break;
                case ToDoItemsDueDate.DueDateOverdue:
                    filterQuery = dbset.Where((entry) => entry.Status != ToDoStatus.Completed
                                                   && entry.DueDate.Value.Date < DateTime.Today);
                    break;
                case ToDoItemsDueDate.DueDateThisMonth:
                    filterQuery = dbset.Where((entry) => entry.Status != ToDoStatus.Completed
                                                   && entry.DueDate.Value.Date.Month == DateTime.Today.Month
                                                   && entry.DueDate.Value.Date.Year == DateTime.Today.Year);
                    break;
                default:
                    filterQuery = dbset.Where((entry) => entry.Status != ToDoStatus.Completed
                                                   && entry.DueDate.Value.Date == DateTime.Today);
                    break;
            }


            var itemsCount = filterQuery.Count();
            var totalPages = (int)Math.Ceiling((decimal)itemsCount / pageSize);

            if (totalPages != 0 && page > totalPages)
            {
                page = totalPages;
            }

            var result = filterQuery.OrderBy(item => item.ToDoList)
                                    .ThenBy(item => item.DueDate)
                                    .Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToList();
            var grouped = result.GroupBy(item => item.ToDoList);


            return (grouped, itemsCount);
        }

        public SummaryOfToDoLists GetSummaryOfToDoLists()
        {
            var summary = new SummaryOfToDoLists
            {
                TotalListsCount = _appDbContext.ToDoLists.Count(),
                NotStartedListsCount = _appDbContext.ToDoLists.Count(list => list.ToDoEntries.All(list => list.Status == ToDoStatus.NotStarted)),
                CompletedListsCount = _appDbContext.ToDoLists.Count(list => list.ToDoEntries.Any() && list.ToDoEntries.All(list => list.Status == ToDoStatus.Completed)),
                HiddenToDoListsCount = _appDbContext.ToDoLists.Count(list => !list.IsVisible)
            };

            summary.InProgressListsCount = summary.TotalListsCount - summary.CompletedListsCount - summary.NotStartedListsCount;

            return summary;
        }

        public void EnsurePopulatedWithDemoData()
        {
            if (!_appDbContext.Set<ToDoList>().Any())
            {
                _appDbContext.Set<ToDoList>().AddRange(
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

                _appDbContext.SaveChanges();
            }
        }

        private void ValidateItem(IValidatableObject item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), $"{nameof(item)} is null");
            }

            var validationContext = new ValidationContext(item);
            var validationResult = item.Validate(validationContext);

            if (validationResult.Any())
            {
                throw new ArgumentException(validationResult.First().ErrorMessage, nameof(item));
            }
        }

        public (IEnumerable<ToDoList>, int) GetFetchedToDoLists(int pageSize, int page, IQueryable<ToDoList> dbSet)
        {
            var totalPages = (int)Math.Ceiling((decimal)dbSet.Count() / pageSize);

            if (page > totalPages)
            {
                page = totalPages;
            }
            else if(page == 0)
            {
                page = 1;
            }


            var items = dbSet.OrderBy(list => list.Id)
                             .Skip((page - 1) * pageSize)
                             .Take(pageSize)
                             .ToList();
            var itemsCount = dbSet.Count();
            return (items, itemsCount);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _appDbContext.Dispose();
        }

        private void ReorderItems(ToDoList toDoList, ToDoEntry originalItem = null)
        {
            var orderedItems = _appDbContext.Set<ToDoList>()
                                           .Find(toDoList.Id).ToDoEntries
                                           .AsEnumerable()
                                           .OrderBy(item => item.OrdinalNumber);

            for (var i = 1; i <= orderedItems.Count(); ++i)
            {
                if (!orderedItems.ElementAt(i - 1).Equals(originalItem))
                {
                    orderedItems.ElementAt(i - 1).OrdinalNumber = i;
                }
            }

            if (originalItem != null && orderedItems.Count(item => item.OrdinalNumber == originalItem.OrdinalNumber) == 2)
            {
                orderedItems.First(item => item.OrdinalNumber == originalItem.OrdinalNumber)
                                .OrdinalNumber = originalItem.OrdinalNumber + 1;
            }
        }
    }
}
