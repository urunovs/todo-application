using System.Collections.Generic;
using System.Linq;
using todo_domain_entities;

namespace todo_aspnetmvc_ui.Models.ViewModels
{
    public class ToDoItemsViewModel
    {
        public IEnumerable<IGrouping<ToDoList, ToDoEntry>> GroupedToDoItems { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public string SelectedDueDate { get; set; }
    }
}
