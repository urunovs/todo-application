// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function copyToClipboard(todoList) {
    const todoItems = todoList.ToDoEntries;
    todoItems.sort((a, b) => { return a.OrdinalNumber - b.OrdinalNumber });

    let listData = `ToDo list title: '${todoList.MainTitle}'\n`;

    for (let i = 0; i < todoItems.length; i++) {
        listData = listData.concat("\n", `
            ${todoItems[i].OrdinalNumber}. ${todoItems[i].Title}\n
            Due date: ${todoItems[i].DueDate}\n
            Status: ${todoItems[i].Status}\n
            Description: ${todoItems[i].Description}\n`);
    }

    navigator.clipboard.writeText(listData);

    const toastElem = document.getElementById('copyToast')
    const toast = new bootstrap.Toast(toastElem)

    toast.show()
}
