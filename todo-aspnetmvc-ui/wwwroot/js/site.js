// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function copyToClipboard(todoItem, todoListTitle) {
    const itemData = [
        `Item from '${todoListTitle}'`,
        `Ordinal number: ${todoItem.OrdinalNumber}`,
        `Title: ${todoItem.Title}`,
        `Due date: ${todoItem.DueDate}`,
        `Status: ${todoItem.Status}`,
        `Description: ${todoItem.Description}`,
    ].join("\n");

    navigator.clipboard.writeText(itemData);

    const toastElem = document.getElementById('copyToast')
    const toast = new bootstrap.Toast(toastElem)

    toast.show()
}
