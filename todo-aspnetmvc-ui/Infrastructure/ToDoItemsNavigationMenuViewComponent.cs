using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using todo_aspnetmvc_ui.Models.Services;
using todo_domain_entities;

namespace todo_aspnetmvc_ui.Infrastructure
{
    public class ToDoItemsNavigationMenuViewComponent: ViewComponent
    {        
        public IViewComponentResult Invoke()
        {
            var dueDateTypes = new List<string>();

            foreach (var value in Enum.GetValues(typeof(ToDoItemsDueDate)))
            {
                dueDateTypes.Add(((ToDoItemsDueDate)value).GetAttribute<DisplayAttribute>().Name);
            }

            if(RouteData?.Values["duedate"] != null)
            {
                ViewBag.SelectedDueDate = RouteData?.Values["duedate"];
            }

            if(ViewBag.SelectedDueDate == null)
            {
                ViewBag.SelectedDueDate = dueDateTypes.FirstOrDefault();
            }

            return View(dueDateTypes);
        }
    }
}
