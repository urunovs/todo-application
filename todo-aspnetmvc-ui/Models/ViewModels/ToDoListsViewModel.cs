using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using todo_domain_entities;

namespace todo_aspnetmvc_ui.Models.ViewModels
{
    public class ToDoListsViewModel
    {
        public IEnumerable<ToDoList> ToDoLists { get; set; }
        public PagingInfo PagingInfo { get; set; }

        public int CompletedToDoListsCount
        { 
            get
            {
                return ToDoLists.Count(list => list.ToDoEntries
                                .All(entry => entry.Status == ToDoStatus.Completed));
            } 
        }
    }
}
