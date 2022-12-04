using System.Collections.Generic;
using todo_domain_entities;

namespace todo_aspnetmvc_ui.Models.ViewModels
{
    public class ToDoListsViewModel
    {
        public IEnumerable<ToDoList> ToDoLists { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public SummaryOfToDoLists SummaryOfToDoLists { get; set; }
        public bool ShowHiddenToDoLists { get; set; } = true;
        public bool ShowCompletedTasks { get; set; } = true;
    }
}
