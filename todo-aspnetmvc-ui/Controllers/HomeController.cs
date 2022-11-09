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
        private readonly ILogger<HomeController> _logger;
        private readonly IToDoServices _toDoServices;
        private readonly Dictionary<ToDoItemsCategory, Func<ToDoEntry, bool>> _todoItemsSelector;
        public const int PageSize = 6;

        public HomeController(ILogger<HomeController> logger, IToDoServices toDoServices)
        {
            _logger = logger;
            _toDoServices = toDoServices ?? throw new ArgumentNullException(nameof(toDoServices));

            _todoItemsSelector = new Dictionary<ToDoItemsCategory, Func<ToDoEntry, bool>>
            {
                { ToDoItemsCategory.DueDateToday, (entry) => entry.Status != ToDoStatus.Completed
                                                          && entry.DueDate.Value.Date == DateTime.Today },

                { ToDoItemsCategory.DueDateTomorrow, (entry) => entry.Status != ToDoStatus.Completed
                                                            &&  entry.DueDate.Value.Date == DateTime.Today.AddDays(1) },

                { ToDoItemsCategory.DueDateOverdue, (entry) => entry.Status != ToDoStatus.Completed
                                                            && entry.DueDate.Value.Date < DateTime.Today },

                { ToDoItemsCategory.DueDateThisMonth, (entry) => entry.Status != ToDoStatus.Completed
                                                              && entry.DueDate.Value.Date.Month == DateTime.Today.Month
                                                              && entry.DueDate.Value.Date.Year == DateTime.Today.Year }
            };
        }

        public ViewResult Index(string category, int todoItemsPage = 1)
        {
            if(category == null)
            {
                category = ToDoItemsCategory.DueDateToday.GetAttribute<DisplayAttribute>().Name;
            }

            var itemsByCategory = GetToDoItemsBySelectedCategory(category);

            return View(new ToDoItemsViewModel
            {
                ToDoItems = itemsByCategory
                    .OrderBy(list => list.Id)
                    .Skip((todoItemsPage - 1) * PageSize)
                    .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = todoItemsPage,
                    ItemsPerPage = PageSize,
                    TotalItems = itemsByCategory.Count()
                },
                CurrentCategory = category
            });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private IEnumerable<ToDoEntry> GetToDoItemsBySelectedCategory(string category)
        {
            var selectedCategory = category.GetValueFromName<ToDoItemsCategory>();

            var todoItemsToView = _toDoServices.ToDoLists
                .SelectMany(list => list.ToDoEntries)
                .Where(_todoItemsSelector[selectedCategory]);

            return todoItemsToView;
        }
    }
}
