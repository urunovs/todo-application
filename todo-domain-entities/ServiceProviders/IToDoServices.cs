using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using todo_domain_entities;

namespace todo_aspnetmvc_ui.Models.Services
{
    public enum ToDoItemsDueDate
    {
        [Display(Name = "Today")]
        DueDateToday,

        [Display(Name = "Tomorrow")]
        DueDateTomorrow,

        [Display(Name = "Overdue")]
        DueDateOverdue,

        [Display(Name = "This month")]
        DueDateThisMonth
    }

    public interface IToDoServices
    {
        public IQueryable<ToDoList> ToDoLists { get; }
        public IEnumerable<ToDoList> VisibleToDoLists { get; }
        public IEnumerable<ToDoList> CompletedToDoLists { get; }

        public ToDoList AddToDoList(ToDoList toDoList);
        public void RemoveToDoList(int toDoListId);
        public ToDoList ModifyToDoList(int toDoListId, ToDoList updatedView);
        public ToDoEntry AddToDoItemToList(ToDoEntry toDoEntryToInsert, int toDoListId);
        public ToDoList ClearToDoList(int toDoListId);
        public ToDoList ChangeVisiblityOfToDoList(int toDoListId);
        public string ChangeToDoItemsStatus(int toDoItemId);
        public ToDoEntry ModifyToDoEntry(int toDoEntryId, ToDoEntry updatedView);
        public (IEnumerable<IGrouping<ToDoList, ToDoEntry>>, int) GetGroupedToDoItemsByDueDate(ToDoItemsDueDate itemsDueDate, int pageSize, int page);
        public void RemoveToDoEntry(int toDoEntryId);
        public void RemoveAllToDoLists();
        public SummaryOfToDoLists GetSummaryOfToDoLists();
    }
}
