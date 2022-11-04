using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using todo_aspnetmvc_ui.Infrastructure;
using todo_aspnetmvc_ui.Models;
using todo_aspnetmvc_ui.Models.Repo;
using todo_aspnetmvc_ui.Models.ViewModels;
using todo_domain_entities;

namespace todo_aspnetmvc_ui.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IToDoRepository _toDoRepository;
        private readonly Dictionary<ToDoItemsCategory, Func<ToDoEntry, bool>> _todoItemsSelector;
        public const int PageSize = 3;

        public HomeController(ILogger<HomeController> logger, IToDoRepository repository)
        {
            _logger = logger;
            _toDoRepository = repository ?? throw new ArgumentNullException(nameof(repository));

            _todoItemsSelector = new Dictionary<ToDoItemsCategory, Func<ToDoEntry, bool>>
            {
                { ToDoItemsCategory.DueDateToday, (entry) => entry.DueDate.Value.Date == DateTime.Today },
                { ToDoItemsCategory.DueDateTomorrow, (entry) => entry.DueDate.Value.Date == DateTime.Today.AddDays(1) },
                { ToDoItemsCategory.DueDateOverdue, (entry) => entry.DueDate.Value.Date > DateTime.Today },
                { ToDoItemsCategory.DueDateThisMonth, (entry) => { return entry.DueDate.Value.Date.Month == DateTime.Today.Month &&
                                                                    entry.DueDate.Value.Date.Year == DateTime.Today.Year; } }
            };
        }

        public ViewResult Index(string category = "DueDateToday", int todoListPage = 1)
            => View(new ToDoItemsViewModel
            {
                ToDoItems = GetToDoItemsBySelectedCategory(category)
                    .OrderBy(list => list.Id)
                    .Skip((todoListPage - 1) * PageSize)
                    .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = todoListPage,
                    ItemsPerPage = PageSize,
                    TotalItems = _toDoRepository.ToDoLists.Count()
                },
                CurrentCategory = category
            });

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
            Enum.TryParse(category, out ToDoItemsCategory selectedCategory);

            var todoItemsToView = _toDoRepository.ToDoLists
                .SelectMany(list => list.ToDoEntries)
                .Where(_todoItemsSelector[selectedCategory])
                .OrderBy(entry => entry.Id);

            return todoItemsToView;
        }
    }
}
