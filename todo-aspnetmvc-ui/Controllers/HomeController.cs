using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using todo_aspnetmvc_ui.Models;
using todo_aspnetmvc_ui.Models.Repo;
using todo_aspnetmvc_ui.Models.ViewModels;

namespace todo_aspnetmvc_ui.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IToDoRepository _toDoRepository;
        public const int PageSize = 2;

        public HomeController(ILogger<HomeController> logger, IToDoRepository repository)
        {
            _logger = logger;
            _toDoRepository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public ViewResult Index(int todoListPage = 1)
            => View(new ToDoListsViewModel
            {
                ToDoLists = _toDoRepository.ToDoLists
                    .OrderBy(list => list.Id)
                    .Skip((todoListPage - 1) * PageSize)
                    .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = todoListPage,
                    ItemsPerPage = PageSize,
                    TotalItems = _toDoRepository.ToDoLists.Count()
                }
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
    }
}
