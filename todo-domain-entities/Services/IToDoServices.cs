using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using todo_domain_entities;

namespace todo_aspnetmvc_ui.Models.Services
{
    public interface IToDoServices
    {
        public IQueryable<ToDoList> ToDoLists { get; }

        public ToDoList AddToDoList(ToDoList toDoList);
        public void RemoveToDoList(ToDoList toDoList);
        public ToDoList ModifyToDoList(ToDoList toDoListToUpdate, ToDoList updatedView);
        public ToDoEntry AddToDoItemToList(ToDoEntry toDoEntry, ToDoList toDoList);
        public void AddToDoEntriesToList(List<ToDoEntry> toDoEntries, ToDoList toDoList);
        public ToDoList ClearToDoList(ToDoList toDoList);
        public ToDoEntry ModifyToDoEntry(ToDoEntry toDoEntryToUpdate, ToDoEntry updatedView);
        public void RemoveToDoEntry(ToDoEntry toDoEntry);
        public ToDoEntry SetToDoEntryStatus(ToDoEntry toDoEntry, ToDoStatus status);
        public void RemoveAllToDoLists();
    }
}
