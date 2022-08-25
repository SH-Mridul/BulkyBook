﻿var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#producstTable').DataTable({
        "ajax": {
            "url": "/Admin/Product/GetAll"
        },
        "columns": [
                    { "data": "title", "width": "15%" },
                    { "data": "price", "width": "15%" },
                    { "data": "isbn", "width": "15%" },
                    { "data": "author", "width": "15%" },
                    { "data": "category.name", "width": "15%" },
                    {
                        "data": "id",
                        "render": function (data) {
                            return `
                             <a href="/Admin/Product/Upsert?id=${data}" class="btn btn-success btn-sm"><i class="bi bi-pencil-square"></i></a>
                             <a onclick=Delete('/Admin/Product/Delete/${data}') class="btn btn-danger btn-sm"><i class="bi bi-trash3-fill"></i></a>
                            `
                        },
                        "width": "15%" 
                    },
               ]
    });
}



function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "product will be deleted!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();
                        toastr.success(data.message);
                    } else {
                        toastr.warning(data.message);
                    }
                }
            })
        }
    })
}