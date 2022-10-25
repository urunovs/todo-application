using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using todo_aspnetmvc_ui.Models.Repo;

namespace todo_domain_entities
{
    /// <summary>
    /// EFToDoListRepository that providing some actions with T0D0 lists in DB.
    /// </summary>
    public class EFToDoListRepository : IToDoRepository, IDisposable
    {
        private readonly AppDbContext appDbContext;

        public IQueryable<ToDoList> ToDoLists { get; private set; }


        public EFToDoListRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
            ToDoLists = appDbContext.ToDoLists;
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

            ToDoLists = appDbContext.ToDoLists;

            return toDoList;
        }

        public void RemoveToDoList(ToDoList toDoList)
        {
            if (!appDbContext.ToDoLists.Contains(toDoList))
            {
                throw new ArgumentException("No such ToDoList", nameof(toDoList));
            }

            appDbContext.ToDoEntries.RemoveRange(toDoList.ToDoEntries);
            appDbContext.ToDoLists.Remove(toDoList);
            appDbContext.SaveChanges();
            ToDoLists = appDbContext.ToDoLists;
        }

        public ToDoList ModifyToDoList(ToDoList toDoListToUpdate, ToDoList updatedView)
        {
            ValidateItem(updatedView);

            toDoListToUpdate = appDbContext.ToDoLists.FirstOrDefault(list => list.Equals(toDoListToUpdate));

            if (toDoListToUpdate == null)
            {
                throw new ArgumentException("No such ToDoList", nameof(toDoListToUpdate));
            }

            AddToDoEntriesToList(updatedView.ToDoEntries, toDoListToUpdate);

            if (toDoListToUpdate.PrimaryPurpose != updatedView.PrimaryPurpose)
            {
                toDoListToUpdate.PrimaryPurpose = updatedView.PrimaryPurpose;
                appDbContext.SaveChanges();
            }

            ToDoLists = appDbContext.ToDoLists;

            return toDoListToUpdate;
        }

        public void AddToDoEntriesToList(List<ToDoEntry> toDoEntries, ToDoList toDoList)
        {
            if (toDoEntries == null || toDoEntries.Count == 0)
            {
                throw new ArgumentException("ToDoEntries list is empty", nameof(toDoEntries));
            }

            toDoEntries.ForEach(item => ValidateItem(item));

            toDoList = appDbContext.ToDoLists.FirstOrDefault(list => list.Equals(toDoList));

            if (toDoList == null)
            {
                throw new ArgumentException("No such ToDoList", nameof(toDoEntries));
            }

            if (!toDoList.ToDoEntries.SequenceEqual(toDoEntries))
            {
                var entriesToRemove = toDoList.ToDoEntries.Except(toDoEntries);
                var entriesToAdd = toDoEntries.Except(toDoList.ToDoEntries);

                foreach (var entry in entriesToAdd)
                {
                    entry.ToDoList = toDoList;
                }

                appDbContext.ToDoEntries.RemoveRange(entriesToRemove);
                appDbContext.ToDoEntries.AddRange(entriesToAdd);
                toDoList.ToDoEntries = toDoEntries;
                appDbContext.SaveChanges();
                ToDoLists = appDbContext.ToDoLists;
            }
        }

        public ToDoEntry ModifyToDoEntry(ToDoEntry toDoEntryToUpdate, ToDoEntry updatedView)
        {
            ValidateItem(updatedView);

            toDoEntryToUpdate = appDbContext.ToDoEntries.FirstOrDefault(entry => entry.Equals(toDoEntryToUpdate));

            if (toDoEntryToUpdate == null)
            {
                throw new ArgumentException("No such ToDoEntry", nameof(toDoEntryToUpdate));
            }

            toDoEntryToUpdate.OrdinalNumber = updatedView.OrdinalNumber;
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
                ToDoLists = appDbContext.ToDoLists;
            }

            appDbContext.SaveChanges();

            return toDoEntryToUpdate;
        }

        public ToDoEntry SetToDoEntryStatus(ToDoEntry toDoEntry, ToDoStatus status)
        {
            toDoEntry = appDbContext.ToDoEntries.FirstOrDefault(entry => entry.Equals(toDoEntry));

            if (toDoEntry == null)
            {
                throw new ArgumentException("No such ToDoEntry", nameof(toDoEntry));
            }

            toDoEntry.Status = status;
            appDbContext.SaveChanges();

            return toDoEntry;
        }

        public void RemoveAllToDoLists()
        {
            appDbContext.ToDoEntries.RemoveRange(appDbContext.ToDoEntries);
            appDbContext.ToDoLists.RemoveRange(appDbContext.ToDoLists);
            appDbContext.SaveChanges();

            ToDoLists = appDbContext.ToDoLists;
        }

        public ToDoList ClearToDoList(ToDoList toDoList)
        {
            toDoList = appDbContext.ToDoLists.FirstOrDefault(list => list.Equals(toDoList));

            if (toDoList == null)
            {
                throw new ArgumentException("No such ToDoList", nameof(toDoList));
            }

            appDbContext.ToDoEntries.RemoveRange(toDoList.ToDoEntries);
            toDoList.ToDoEntries.Clear();
            appDbContext.SaveChanges();

            return toDoList;
        }

        public void RemoveToDoEntry(ToDoEntry toDoEntry)
        {
            if (toDoEntry == null)
            {
                throw new ArgumentException(nameof(toDoEntry));
            }

            toDoEntry = appDbContext.ToDoEntries.FirstOrDefault(entry => entry.Equals(toDoEntry));

            if (toDoEntry == null)
            {
                throw new ArgumentException("No such ToDoEntry", nameof(toDoEntry));
            }

            appDbContext.ToDoEntries.Remove(toDoEntry);
            toDoEntry.ToDoList.ToDoEntries.Remove(toDoEntry);
            appDbContext.SaveChanges();
        }

        private void ValidateItem(IValidatableObject item)
        {
            if(item == null)
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
