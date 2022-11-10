using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using todo_domain_entities;

namespace todo_aspnetmvc_ui.Models.ViewModels
{
    public class ToDoItemsViewModel
    {
        public IEnumerable<IGrouping<ToDoList, ToDoEntry>> GroupedToDoItems { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public string CurrentCategory { get; set; }
    }
}
