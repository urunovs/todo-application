using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using todo_aspnetmvc_ui.Models.Services;
using static todo_aspnetmvc_ui.Models.Services.IToDoServices;

namespace todo_domain_entities
{

    /// <summary>
    /// ToDoEFCoreServicesProvider performing some actions with T0D0 lists against DB.
    /// </summary>
    public class ToDoEFCoreServicesProvider : IToDoServices, IDisposable
    {
        private readonly AppDbContext appDbContext;
        public IEnumerable<ToDoList> ToDoLists => appDbContext.Set<ToDoList>().AsEnumerable();

        public IEnumerable<ToDoList> VisibleToDoLists =>
            appDbContext.Set<ToDoList>().Where(list => list.IsVisible).AsEnumerable();

        public IEnumerable<ToDoList> CompletedToDoLists =>
            appDbContext.Set<ToDoList>().Where(list => list.ToDoEntries
                                            .All(item => item.Status == ToDoStatus.Completed));


        public ToDoEFCoreServicesProvider(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            appDbContext.Dispose();
        }

        public ToDoList AddToDoList(ToDoList toDoList)
        {
            ValidateItem(toDoList);

            toDoList = appDbContext.Set<ToDoList>().Add(toDoList).Entity;
            appDbContext.SaveChanges();

            return toDoList;
        }

        public void RemoveToDoList(int toDoListId)
        {
            var toDoList = appDbContext.Set<ToDoList>().Find(toDoListId);

            if (toDoList == null)
            {
                throw new ArgumentException("No such ToDoList instance with a given Id", nameof(toDoListId));
            }

            appDbContext.Set<ToDoList>().Remove(toDoList);
            appDbContext.SaveChanges();
        }

        public ToDoList ModifyToDoList(int toDoListId, ToDoList updatedView)
        {
            ValidateItem(updatedView);

            var toDoListToUpdate = appDbContext.Set<ToDoList>().Find(toDoListId);

            if (toDoListToUpdate == null)
            {
                throw new ArgumentException("No such ToDoList instance with a given Id", nameof(toDoListId));
            }

            toDoListToUpdate.MainTitle = updatedView.MainTitle;
            toDoListToUpdate.IsVisible = updatedView.IsVisible;

            appDbContext.SaveChanges();

            return toDoListToUpdate;
        }

        public ToDoList ChangeVisiblityOfToDoList(int toDoListId)
        {
            var toDoListToUpdate = appDbContext.Set<ToDoList>().Find(toDoListId);

            if (toDoListToUpdate == null)
            {
                throw new ArgumentException("No such ToDoList instance with a given Id", nameof(toDoListId));
            }

            toDoListToUpdate.IsVisible = !toDoListToUpdate.IsVisible;

            appDbContext.SaveChanges();

            return toDoListToUpdate;
        }

        public ToDoEntry AddToDoItemToList(ToDoEntry toDoEntryToInsert, int toDoListId)
        {
            if (toDoEntryToInsert == null)
            {
                throw new ArgumentNullException(nameof(toDoEntryToInsert), "Passed object is null");
            }

            ValidateItem(toDoEntryToInsert);

            var toDoList = appDbContext.Set<ToDoList>().Find(toDoListId);

            if (toDoList == null)
            {
                throw new ArgumentException("No such ToDoList instance with a given Id", nameof(toDoListId));
            }

            toDoEntryToInsert.ToDoList = toDoList;
            appDbContext.Set<ToDoEntry>().Add(toDoEntryToInsert);

            if (toDoList.ToDoEntries.Any(item => item.OrdinalNumber == toDoEntryToInsert.OrdinalNumber))
            {
                ReorderItems(toDoEntryToInsert.ToDoList, toDoEntryToInsert);
            }

            appDbContext.SaveChanges();
            toDoList.ToDoEntries.Add(toDoEntryToInsert);

            return toDoEntryToInsert;
        }

        public ToDoEntry ModifyToDoEntry(int toDoEntryId, ToDoEntry updatedView)
        {
            ValidateItem(updatedView);

            var toDoEntryToUpdate = appDbContext.Set<ToDoEntry>().Find(toDoEntryId);

            if (toDoEntryToUpdate == null)
            {
                throw new ArgumentException("No such ToDoEntry instance with a given Id", nameof(toDoEntryId));
            }

            toDoEntryToUpdate.Title = updatedView.Title;
            toDoEntryToUpdate.Description = updatedView.Description;
            toDoEntryToUpdate.DueDate = updatedView.DueDate;
            toDoEntryToUpdate.Status = updatedView.Status;
            toDoEntryToUpdate.OrdinalNumber = updatedView.OrdinalNumber;

            if (toDoEntryToUpdate.ToDoList.ToDoEntries.Count(item => item.OrdinalNumber == updatedView.OrdinalNumber) > 1)
            {
                ReorderItems(toDoEntryToUpdate.ToDoList, toDoEntryToUpdate);
            }

            appDbContext.SaveChanges();

            return toDoEntryToUpdate;
        }

        public void RemoveAllToDoLists()
        {
            appDbContext.Set<ToDoList>().RemoveRange(appDbContext.Set<ToDoList>());
            appDbContext.SaveChanges();
        }

        public ToDoList ClearToDoList(int toDoListId)
        {
            var toDoList = appDbContext.Set<ToDoList>().Find(toDoListId);

            if (toDoList == null)
            {
                throw new ArgumentException("No such ToDoList wtih a given Id", nameof(toDoListId));
            }

            toDoList.ToDoEntries.Clear();
            appDbContext.SaveChanges();

            return toDoList;
        }

        public string ChangeToDoItemsStatus(int toDoItemId)
        {
            var toDoEntry = appDbContext.Set<ToDoEntry>().Find(toDoItemId);

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

            appDbContext.SaveChanges();
            return toDoEntry.Status.ToString();
        }

        public void RemoveToDoEntry(int toDoEntryId)
        {
            var toDoEntry = appDbContext.Set<ToDoEntry>().Find(toDoEntryId);

            if (toDoEntry == null)
            {
                throw new ArgumentException("No such ToDoEntry instance with a given Id", nameof(toDoEntryId));
            }

            toDoEntry.ToDoList.ToDoEntries.Remove(toDoEntry);
            appDbContext.Set<ToDoEntry>().Remove(toDoEntry);
            ReorderItems(toDoEntry.ToDoList);
            appDbContext.SaveChanges();
        }

        public (IEnumerable<IGrouping<ToDoList, ToDoEntry>>, int) GetGroupedToDoItemsByDueDate(
            ToDoItemsDueDate itemsDueDate,
            int pageSize,
            int page)
        {
            IQueryable<ToDoEntry> filteredItems;

            switch (itemsDueDate)
            {

                case ToDoItemsDueDate.DueDateTomorrow:
                    filteredItems = appDbContext.ToDoEntries
                                    .Where((entry) => entry.Status != ToDoStatus.Completed
                                                               && entry.DueDate.Value.Date == DateTime.Today.AddDays(1));
                    break;

                case ToDoItemsDueDate.DueDateOverdue:
                    filteredItems = appDbContext.ToDoEntries
                                    .Where((entry) => entry.Status != ToDoStatus.Completed
                                                   && entry.DueDate.Value.Date < DateTime.Today);
                    break;

                case ToDoItemsDueDate.DueDateThisMonth:
                    filteredItems = appDbContext.ToDoEntries
                                    .Where((entry) => entry.Status != ToDoStatus.Completed
                                                   && entry.DueDate.Value.Date.Month == DateTime.Today.Month
                                                   && entry.DueDate.Value.Date.Year == DateTime.Today.Year);
                    break;

                default:
                    filteredItems = appDbContext.ToDoEntries
                                                .Where((entry) => entry.Status != ToDoStatus.Completed
                                                               && entry.DueDate.Value.Date == DateTime.Today);
                    break;
            }


            var groupedItems = filteredItems.OrderBy(item => item.Id)
                                            .Skip((page - 1) * pageSize)
                                            .Take(pageSize)
                                            .AsEnumerable()
                                            .GroupBy(item => item.ToDoList);

            return (groupedItems, filteredItems.Count());
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

        private void ReorderItems(ToDoList toDoList, ToDoEntry originalItem = null)
        {
            var orderedItems = appDbContext.Set<ToDoList>()
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
