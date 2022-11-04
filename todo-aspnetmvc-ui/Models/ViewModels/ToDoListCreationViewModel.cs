using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using todo_domain_entities;

namespace todo_aspnetmvc_ui.Models.ViewModels
{
    public class ToDoListCreationViewModel
    {
        public ToDoList ToDoList { get; set; }
        public ToDoEntry ToDoItemToAdd { get; set; }
    }
}
