using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using todo_aspnetmvc_ui.Models.Services;
using static todo_aspnetmvc_ui.Models.Services.IToDoServices;

namespace todo_domain_entities
{


    /// <summary>
    /// ToDoEFCoreServices that providing some actions with T0D0 lists in DB.
    /// </summary>
    public class ToDoEFCoreServicesProvider : IToDoServices, IDisposable
    {
        private readonly AppDbContext appDbContext;
        public IQueryable<ToDoList> ToDoLists => appDbContext.ToDoLists;


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

            toDoList = appDbContext.ToDoLists.Add(toDoList).Entity;
            appDbContext.SaveChanges();

            return toDoList;
        }

        public void RemoveToDoList(int toDoListId)
        {
            var toDoList = appDbContext.ToDoLists.Find(toDoListId);

            if (toDoList == null)
            {
                throw new ArgumentException("No such ToDoList instance with a given Id", nameof(toDoListId));
            }

            appDbContext.ToDoEntries.RemoveRange(toDoList.ToDoEntries);
            appDbContext.ToDoLists.Remove(toDoList);
            appDbContext.SaveChanges();
        }

        public ToDoList ModifyToDoList(int toDoListId, ToDoList updatedView)
        {
            ValidateItem(updatedView);

            var toDoListToUpdate = appDbContext.ToDoLists.Find(toDoListId);

            if (toDoListToUpdate == null)
            {
                throw new ArgumentException("No such ToDoList instance with a given Id", nameof(toDoListId));
            }

            if (toDoListToUpdate.MainTitle != updatedView.MainTitle)
            {
                toDoListToUpdate.MainTitle = updatedView.MainTitle;
            }

            if (toDoListToUpdate.IsVisible != updatedView.IsVisible)
            {
                toDoListToUpdate.IsVisible = updatedView.IsVisible;
            }

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

            var toDoList = appDbContext.ToDoLists.Find(toDoListId);

            if (toDoList == null)
            {
                throw new ArgumentException("No such ToDoList instance with a given Id", nameof(toDoListId));
            }

            var sameOrdinalNumberItem = toDoList.ToDoEntries.FirstOrDefault(item =>
                                            item.OrdinalNumber == toDoEntryToInsert.OrdinalNumber);

            if (sameOrdinalNumberItem != null)
            {
                var index = toDoList.ToDoEntries.IndexOf(sameOrdinalNumberItem);

                toDoList.ToDoEntries.Insert(index, toDoEntryToInsert);

                for (var i = index + 1; i < toDoList.ToDoEntries.Count; i++)
                {
                    toDoList.ToDoEntries[i].OrdinalNumber++;
                    appDbContext.ToDoEntries.Find(toDoList.ToDoEntries[i].Id).OrdinalNumber++;
                }
            }

            toDoEntryToInsert.ToDoList = toDoList;
            appDbContext.ToDoEntries.Add(toDoEntryToInsert);
            appDbContext.SaveChanges();

            return toDoEntryToInsert;
        }

        public ToDoEntry ModifyToDoEntry(int toDoEntryId, ToDoEntry updatedView)
        {
            ValidateItem(updatedView);

            var toDoEntryToUpdate = appDbContext.ToDoEntries.Find(toDoEntryId);

            if (toDoEntryToUpdate == null)
            {
                throw new ArgumentException("No such ToDoEntry instance with a given Id", nameof(toDoEntryId));
            }

            toDoEntryToUpdate.Title = updatedView.Title;
            toDoEntryToUpdate.Description = updatedView.Description;
            toDoEntryToUpdate.DueDate = updatedView.DueDate;
            toDoEntryToUpdate.Status = updatedView.Status;

            if (!toDoEntryToUpdate.ToDoList.Equals(updatedView.ToDoList))
            {
                appDbContext.ToDoLists.Find(toDoEntryToUpdate.ToDoList.Id)
                                      .ToDoEntries.Remove(toDoEntryToUpdate);
                appDbContext.ToDoLists.Find(updatedView.ToDoList.Id)
                                      .ToDoEntries.Add(toDoEntryToUpdate);

                toDoEntryToUpdate.ToDoList = updatedView.ToDoList;
            }

            if (toDoEntryToUpdate.OrdinalNumber != updatedView.OrdinalNumber)
            {
                // Shifting OrdinalNumber of items
                if (toDoEntryToUpdate.OrdinalNumber > updatedView.OrdinalNumber)
                {
                    var prevItems = toDoEntryToUpdate.ToDoList.ToDoEntries
                                        .Where(item => item.OrdinalNumber >= updatedView.OrdinalNumber
                                                    && item.OrdinalNumber < toDoEntryToUpdate.OrdinalNumber)
                                        .OrderBy(item => item.OrdinalNumber);

                    foreach (var item in prevItems)
                    {
                        item.OrdinalNumber++;
                    }
                }
                else
                {
                    var nextItems = toDoEntryToUpdate.ToDoList.ToDoEntries
                                        .Where(item => item.OrdinalNumber > toDoEntryToUpdate.OrdinalNumber
                                                    && item.OrdinalNumber <= updatedView.OrdinalNumber)
                                        .OrderBy(item => item.OrdinalNumber);

                    foreach (var item in nextItems)
                    {
                        item.OrdinalNumber--;
                    }
                }

                toDoEntryToUpdate.OrdinalNumber = updatedView.OrdinalNumber;
            }

            appDbContext.SaveChanges();

            return toDoEntryToUpdate;
        }

        public void RemoveAllToDoLists()
        {
            appDbContext.ToDoEntries.RemoveRange(appDbContext.ToDoEntries);
            appDbContext.ToDoLists.RemoveRange(appDbContext.ToDoLists);
            appDbContext.SaveChanges();
        }

        public ToDoList ClearToDoList(int toDoListId)
        {
            var toDoList = appDbContext.ToDoLists.Find(toDoListId);

            if (toDoList == null)
            {
                throw new ArgumentException("No such ToDoList wtih a given Id", nameof(toDoListId));
            }

            appDbContext.ToDoEntries.RemoveRange(toDoList.ToDoEntries);
            toDoList.ToDoEntries.Clear();
            appDbContext.SaveChanges();

            return toDoList;
        }

        public void RemoveToDoEntry(int toDoEntryId)
        {
            var toDoEntry = appDbContext.ToDoEntries.Find(toDoEntryId);

            if (toDoEntry == null)
            {
                throw new ArgumentException("No such ToDoEntry instance with a given Id", nameof(toDoEntryId));
            }

            toDoEntry.ToDoList.ToDoEntries.Remove(toDoEntry);
            appDbContext.ToDoEntries.Remove(toDoEntry);
            appDbContext.SaveChanges();
        }

        private void ValidateItem(IValidatableObject item)
        {
            if (item == null)
            {
                throw new ArgumentException($"{nameof(item)} is null", nameof(item));
            }

            var validationContext = new ValidationContext(item);
            var validationResult = item.Validate(validationContext);

            if (validationResult.Any())
            {
                throw new ArgumentException(validationResult.First().ErrorMessage, nameof(item));
            }
        }
    }
}
