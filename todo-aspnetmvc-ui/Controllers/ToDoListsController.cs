using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using todo_aspnetmvc_ui.Infrastructure;
using todo_aspnetmvc_ui.Models.Services;
using todo_aspnetmvc_ui.Models.ViewModels;
using todo_domain_entities;

namespace todo_aspnetmvc_ui.Controllers
{
    public class ToDoListsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IToDoServices _todoServices;
        public const int PageSize = 3;

        public ToDoListsController(IConfiguration configuration, IToDoServices todoServices)
        {
            _configuration = configuration;
            _todoServices = todoServices ?? throw new ArgumentNullException(nameof(todoServices));
        }


        // GET: ToDoListsController
        [HttpGet]
        public ActionResult Index(int page = 1)
        {
            var todoLists = bool.Parse(_configuration["ShowHiddenToDoLists"])
                ? _todoServices.ToDoLists.ToList()
                : _todoServices.VisibleToDoLists.ToList();

            return View(new ToDoListsViewModel
            {
                ToDoLists = todoLists
                    .OrderBy(list => list.Id)
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = todoLists.Count
                },
                CompletedToDoListsCount = _todoServices.CompletedToDoLists.Count(),
                TotalToDoListsCount = _todoServices.ToDoLists.Count(),
                ShowHiddenToDoLists = bool.Parse(_configuration["ShowHiddenToDoLists"]),
                ShowCompletedTasks = bool.Parse(_configuration["ShowCompletedTasks"])
            });
        }

        [HttpPost]
        public ActionResult UpdateList(bool showHiddenLists, bool showCompletedTasks)
        {
            _configuration["ShowHiddenToDoLists"] = showHiddenLists.ToString();
            _configuration["ShowCompletedTasks"] = showCompletedTasks.ToString();

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public ActionResult CreateToDoList(ToDoList toDoList)
        {
            return View(_todoServices.ToDoLists.FirstOrDefault(list => list.Id == toDoList.Id));
        }


        // POST: ToDoListsController/CreateToDoList
        [HttpPost]
        public ActionResult CreateToDoList(string newTodoListTitle)
        {
            try
            {
                var listId = _todoServices.AddToDoList(new ToDoList { MainTitle = newTodoListTitle }).Id;

                return RedirectToAction(nameof(OpenToDoList), new { toDoListId = listId });
            }
            catch
            {
                return View();
            }
        }


        [HttpPost]
        public ActionResult AddToDoItem(ToDoEntry toDoItem, int todoListId)
        {
            try
            {
                _todoServices.AddToDoItemToList(toDoItem, todoListId);

                return RedirectToAction(nameof(OpenToDoList), new { toDoListId = toDoItem.ToDoList.Id });
            }
            catch
            {
                return View();
            }
        }

        // POST: ToDoListsController/DeleteToDoList/5
        [HttpPost]
        public ActionResult DeleteToDoList(ToDoList toDoList)
        {
            try
            {
                _todoServices.RemoveToDoList(toDoList.Id);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // POST: ToDoListsController/EditToDoListTitle/obj
        [HttpPost]
        public ActionResult EditToDoListTitle(ToDoList toDoList)
        {
            try
            {
                _todoServices.ModifyToDoList(toDoList.Id, toDoList);

                return RedirectToAction(nameof(OpenToDoList), new { toDoListId = toDoList.Id });
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public void ChangeVisiblityState(int todoListId)
        {
            _todoServices.ChangeVisiblityOfToDoList(todoListId);
        }

        [HttpPost]
        public string ChangeToDoStatus(int todoItemId)
        {
            return _todoServices.ChangeToDoStatus(todoItemId);
        }

        // POST: ToDoListsController/EditToDoListTitle/obj
        [HttpPost]
        public ActionResult EditToDoItem(ToDoEntry toDoItem, int todoListId)
        {
            try
            {
                toDoItem.ToDoList = _todoServices.ToDoLists.FirstOrDefault(list => list.Id == todoListId);
                _todoServices.ModifyToDoEntry(toDoItem.Id, toDoItem);

                return RedirectToAction(nameof(OpenToDoList), new { toDoListId = toDoItem.ToDoList.Id });
            }
            catch
            {
                return View();
            }
        }


        [HttpPost]
        public ActionResult DeleteToDoItem(ToDoEntry toDoItem, int todoListId)
        {
            try
            {
                _todoServices.RemoveToDoEntry(toDoItem.Id);

                return RedirectToAction(nameof(OpenToDoList), new { toDoListId = todoListId });
            }
            catch
            {
                return View();
            }
        }

        // GET: ToDoListsController/Edit/5
        [HttpGet]
        public ActionResult OpenToDoList(int toDoListId)
        {
            try
            {
                return View(_todoServices.ToDoLists.FirstOrDefault(list => list.Id == toDoListId));
            }
            catch
            {
                return View();
            }
        }
    }
}
