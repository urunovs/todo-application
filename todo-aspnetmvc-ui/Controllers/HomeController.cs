using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using todo_aspnetmvc_ui.Infrastructure;
using todo_aspnetmvc_ui.Models;
using todo_aspnetmvc_ui.Models.Services;
using todo_aspnetmvc_ui.Models.ViewModels;
using todo_domain_entities;

namespace todo_aspnetmvc_ui.Controllers
{
    public class HomeController : Controller
    {
        private readonly IToDoServices _toDoServices;
        public const int PageSize = 5;

        public HomeController(IToDoServices toDoServices)
        {
            _toDoServices = toDoServices ?? throw new ArgumentNullException(nameof(toDoServices));
        }

        public ViewResult Index(string duedate, int page = 1)
        {
            if (duedate == null)
            {
                duedate = ToDoItemsDueDate.DueDateToday.GetAttribute<DisplayAttribute>().Name;
            }

            var dueDateAsEnum = duedate.GetValueFromName<ToDoItemsDueDate>();
            var (selectedItems, count) = _toDoServices.GetGroupedToDoItemsByDueDate(
                dueDateAsEnum,
                PageSize,
                page);

            return View(new ToDoItemsViewModel
            {
                GroupedToDoItems = selectedItems,
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = count,
                },
                SelectedDueDate = dueDateAsEnum.GetAttribute<DisplayAttribute>().Name
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
