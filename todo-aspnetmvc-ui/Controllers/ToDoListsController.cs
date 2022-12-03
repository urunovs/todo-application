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
        private readonly IToDoServices _todoServices;
        public const int PageSize = 3;

        public ToDoListsController(IConfiguration configuration, IToDoServices todoServices)
        {
            _todoServices = todoServices ?? throw new ArgumentNullException(nameof(todoServices));
        }


        [HttpGet]
        public ActionResult Index(int page = 1)
        {
            var showHiddenToDoLists = bool.Parse(HttpContext.Request.Cookies["ShowHiddenToDoLists"] ?? "True");
            var showCompletedTasks = bool.Parse(HttpContext.Request.Cookies["ShowCompletedTasks"] ?? "True");
            var (todoLists, count) = showHiddenToDoLists
                            ? _todoServices.GetToDoLists(PageSize, page)
                            : _todoServices.GetVisibleToDoLists(PageSize, page);

            return View(new ToDoListsViewModel
            {
                ToDoLists = todoLists,
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = count,
                },
                SummaryOfToDoLists = _todoServices.GetSummaryOfToDoLists(),
                ShowHiddenToDoLists = showHiddenToDoLists,
                ShowCompletedTasks = showCompletedTasks
            });
        }

        [HttpPost]
        public ActionResult UpdateGeneralList(bool showHiddenLists, bool showCompletedTasks)
        {
            HttpContext.Response.Cookies.Append("ShowHiddenToDoLists", showHiddenLists.ToString());
            HttpContext.Response.Cookies.Append("ShowCompletedTasks", showCompletedTasks.ToString());

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public ActionResult OpenToDoList(int toDoListId)
        {
            var todoList = _todoServices.GetToDoListById(toDoListId);

            if (todoList != null)
            {
                return View(todoList);
            }

            return NotFound();
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
            toDoItem.ToDoList = _todoServices.GetToDoListById(todoListId);
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
