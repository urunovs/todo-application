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
    public enum ToDoItemsCategory
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

    public class ToDoItemsNavigationMenuViewComponent: ViewComponent
    {        
        public ToDoItemsNavigationMenuViewComponent()
        {
        }

        public IViewComponentResult Invoke()
        {
            var categories = new List<string>();

            foreach (var value in Enum.GetValues(typeof(ToDoItemsCategory)))
            {
                categories.Add(((ToDoItemsCategory)value).GetAttribute<DisplayAttribute>().Name);
            }

            if(RouteData?.Values["category"] != null)
            {
                ViewBag.SelectedCategory = RouteData?.Values["category"];
            }

            if(ViewBag.SelectedCategory == null)
            {
                ViewBag.SelectedCategory = categories.FirstOrDefault();
            }

            return View(categories);
        }
    }
}
