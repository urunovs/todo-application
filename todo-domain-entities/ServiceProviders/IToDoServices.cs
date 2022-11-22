using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using todo_domain_entities;

namespace todo_aspnetmvc_ui.Models.Services
{
    public interface IToDoServices
    {
        public IEnumerable<ToDoList> ToDoLists { get; }

        public ToDoList AddToDoList(ToDoList toDoList);
        public void RemoveToDoList(int toDoListId);
        public ToDoList ModifyToDoList(int toDoListId, ToDoList updatedView);
        public ToDoEntry AddToDoItemToList(ToDoEntry toDoEntryToInsert, int toDoListId);
        public ToDoList ClearToDoList(int toDoListId);
        public ToDoEntry ModifyToDoEntry(int toDoEntryId, ToDoEntry updatedView);
        public void RemoveToDoEntry(int toDoEntryId);
        public void RemoveAllToDoLists();
    }
}
