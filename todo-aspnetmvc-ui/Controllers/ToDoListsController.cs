using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using todo_aspnetmvc_ui.Infrastructure;
using todo_aspnetmvc_ui.Models.Repo;
using todo_aspnetmvc_ui.Models.ViewModels;
using todo_domain_entities;

namespace todo_aspnetmvc_ui.Controllers
{
    public class ToDoListsController : Controller
    {
        private readonly ILogger<ToDoListsController> _logger;
        private readonly IToDoRepository _toDoRepository;
        public const int PageSize = 3;

        public ToDoListsController(ILogger<ToDoListsController> logger, IToDoRepository repository)
        {
            _logger = logger;
            _toDoRepository = repository ?? throw new ArgumentNullException(nameof(repository));
        }


        // GET: ToDoListsController
        public ActionResult Index(int selectedToDolistId, int todoListPage = 1)
        {
            return View(new ToDoListsViewModel
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
        }


        // GET: ToDoListsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }


        [HttpGet]
        public ActionResult CreateToDoList()
        {
            var todoCreationViewModel = new ToDoListCreationViewModel
            {
                ToDoList = HttpContext.Session.GetJson<ToDoList>("ToDoList")
            };

            return View(todoCreationViewModel);
        }


        // POST: ToDoListsController/CreateToDoList
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateToDoList(string newTodoListTitle)
        {
            var todoCreationViewModel = new ToDoListCreationViewModel
            {
                ToDoList = new ToDoList { MainTitle = newTodoListTitle }
            };

            HttpContext.Session.SetJson("ToDoList", todoCreationViewModel.ToDoList);

            return View(todoCreationViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddToDoItem(ToDoEntry toDoItem)
        {
            try
            {
                var bufferedToDoList = HttpContext.Session.GetJson<ToDoList>("ToDoList");
                bufferedToDoList.ToDoEntries.Add(toDoItem);
                HttpContext.Session.SetJson("ToDoList", bufferedToDoList);

                return RedirectToAction(nameof(CreateToDoList));
            }
            catch
            {
                return View();
            }
        }


        // GET: ToDoListsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }


        // POST: ToDoListsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


        // GET: ToDoListsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }


        // POST: ToDoListsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
