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


        [HttpGet]
        public ActionResult Index(int page = 1)
        {
            var todoLists = bool.Parse(_configuration["ShowHiddenToDoLists"])
                ? _todoServices.ToDoLists
                : _todoServices.VisibleToDoLists;

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
                    TotalItems = todoLists.Count()
                },
                CompletedToDoListsCount = _todoServices.CompletedToDoLists.Count(),
                TotalToDoListsCount = _todoServices.ToDoLists.Count(),
                ShowHiddenToDoLists = bool.Parse(_configuration["ShowHiddenToDoLists"]),
                ShowCompletedTasks = bool.Parse(_configuration["ShowCompletedTasks"])
            });
        }

        [HttpPost]
        public ActionResult UpdateGeneralList(bool showHiddenLists, bool showCompletedTasks)
        {
            _configuration["ShowHiddenToDoLists"] = showHiddenLists.ToString();
            _configuration["ShowCompletedTasks"] = showCompletedTasks.ToString();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public ActionResult OpenToDoList(int toDoListId)
        {
            return View(_todoServices.ToDoLists.FirstOrDefault(list => list.Id == toDoListId));
        }

        [HttpPost]
        public ActionResult CreateToDoList(string newTodoListTitle)
        {
            var listId = _todoServices.AddToDoList(new ToDoList { MainTitle = newTodoListTitle }).Id;

            return RedirectToAction(nameof(OpenToDoList), new { toDoListId = listId });
        }

        [HttpPost]
        public ActionResult DeleteToDoList(ToDoList toDoList)
        {
            _todoServices.RemoveToDoList(toDoList.Id);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public ActionResult EditToDoListTitle(ToDoList toDoList)
        {
            _todoServices.ModifyToDoList(toDoList.Id, toDoList);

            return RedirectToAction(nameof(OpenToDoList), new { toDoListId = toDoList.Id });
        }

        [HttpPost]
        public void ChangeVisiblityState(int todoListId)
        {
            _todoServices.ChangeVisiblityOfToDoList(todoListId);
        }

        [HttpPost]
        public string ChangeToDoStatus(int todoItemId)
        {
            return _todoServices.ChangeToDoItemsStatus(todoItemId);
        }

        [HttpPost]
        public ActionResult AddToDoItem(ToDoEntry toDoItem, int todoListId)
        {
            _todoServices.AddToDoItemToList(toDoItem, todoListId);

            return RedirectToAction(nameof(OpenToDoList), new { toDoListId = toDoItem.ToDoList.Id });
        }

        [HttpPost]
        public ActionResult EditToDoItem(ToDoEntry toDoItem, int todoListId)
        {
            toDoItem.ToDoList = _todoServices.ToDoLists.FirstOrDefault(list => list.Id == todoListId);
            _todoServices.ModifyToDoEntry(toDoItem.Id, toDoItem);

            return RedirectToAction(nameof(OpenToDoList), new { toDoListId = toDoItem.ToDoList.Id });
        }

        [HttpPost]
        public ActionResult DeleteToDoItem(ToDoEntry toDoItem, int todoListId)
        {
            _todoServices.RemoveToDoEntry(toDoItem.Id);

            return RedirectToAction(nameof(OpenToDoList), new { toDoListId = todoListId });
        }
    }
}
