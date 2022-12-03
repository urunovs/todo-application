// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function copyToClipboard(todoList) {
    const todoItems = todoList.ToDoEntries;
    todoItems.sort((a, b) => { return a.OrdinalNumber - b.OrdinalNumber });

    let listData = `ToDo list title: '${todoList.MainTitle}'\n`;

    for (let i = 0; i < todoItems.length; i++) {
        listData = listData.concat("\n",
            `${todoItems[i].OrdinalNumber}. ${todoItems[i].Title}\n` +
            `Due date: ${todoItems[i].DueDate}\n` +
            `Status: ${todoItems[i].Status}\n` +
            `Description: ${todoItems[i].Description}\n`,
            `Notes: ${todoItems[i].Notes}\n`);
    }

    navigator.clipboard.writeText(listData);

    const toastElem = document.getElementById('copyToast')
    const toast = new bootstrap.Toast(toastElem)

    toast.show()
}

function updateList() {
    const updateForm = document.getElementById('listsUpdateForm');
    updateForm.submit();
}

function updateToDoListsVisiblityState(todoListId, url_action) {
    $.ajax({
        type: "POST",
        url: url_action,
        data: {
            todoListId
        }
    })
}

function processState(todoItemId, url_action) {
    $.ajax({
        type: "POST",
        url: url_action,
        data: {
            todoItemId
        },
        dataType: "text",
        success: function (value) {
            const button = document.getElementById(`statusBtn${todoItemId}`);
            const statusSpan = document.getElementById(`statusSpan${todoItemId}`);
            const statusImg = document.getElementById(`statusImg${todoItemId}`);

            if (value == 'InProgress') {
                button.value = "Complete"
                statusSpan.textContent = "In progress"
                statusImg.src = "/img/icons/status-in-progress.png"
            }
            else {
                button.style.display = "none"
                statusSpan.textContent = "Completed"
                statusImg.src = "/img/icons/status-completed.png"
            }
        }
    })
}