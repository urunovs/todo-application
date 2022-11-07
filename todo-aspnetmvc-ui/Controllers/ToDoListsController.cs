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
                },
                CompletedToDoListsCount = _toDoRepository.ToDoLists.Count(
                                            list => list.ToDoEntries.All(
                                                item => item.Status == ToDoStatus.Completed)),
                TotalToDoListsCount = _toDoRepository.ToDoLists.Count()
            });
        }


        [HttpGet]
        public ActionResult CreateToDoList(ToDoList toDoList)
        {
            return View(_toDoRepository.ToDoLists.FirstOrDefault(list => list.Id == toDoList.Id));
        }


        // POST: ToDoListsController/CreateToDoList
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateToDoList(string newTodoListTitle)
        {
            try
            {
                var listId = _toDoRepository.AddToDoList(new ToDoList { MainTitle = newTodoListTitle }).Id;

                return RedirectToAction(nameof(OpenToDoList), new { toDoListId = listId });
            }
            catch
            {
                return View();
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddToDoItem(ToDoEntry toDoItem)
        {
            try
            {
                toDoItem = _toDoRepository.AddToDoItemToList(toDoItem, toDoItem.ToDoList);

                return RedirectToAction(nameof(OpenToDoList), toDoItem.ToDoList.Id);
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
                _toDoRepository.RemoveToDoList(_toDoRepository.ToDoLists.FirstOrDefault(
                                                list => list.Id == toDoList.Id));

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
                var listToUpdate = _toDoRepository.ToDoLists.FirstOrDefault(list => list.Id == toDoList.Id);

                _toDoRepository.ModifyToDoList(listToUpdate, toDoList);

                return RedirectToAction(nameof(OpenToDoList), new { toDoListId = toDoList.Id });
            }
            catch
            {
                return View();
            }
        }

        // POST: ToDoListsController/EditToDoListTitle/obj
        [HttpPost]
        public ActionResult EditToDoItem(ToDoEntry toDoItem, int todoListId)
        {
            try
            {
                toDoItem.ToDoList = _toDoRepository.ToDoLists.FirstOrDefault(list => list.Id == todoListId);
                _toDoRepository.ModifyToDoEntry(toDoItem, toDoItem);

                return RedirectToAction(nameof(OpenToDoList), new { toDoListId = toDoItem.ToDoList.Id });
            }
            catch
            {
                return View();
            }
        }

        // GET: ToDoListsController/Edit/5
        public ActionResult OpenToDoList(int toDoListId)
        {
            try
            {
                return View(_toDoRepository.ToDoLists.FirstOrDefault(list => list.Id == toDoListId));
            }
            catch
            {
                return View();
            }
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
