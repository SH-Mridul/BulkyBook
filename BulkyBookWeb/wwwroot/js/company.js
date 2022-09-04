var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#companiesTable').DataTable({
        "ajax": {
            "url": "/Admin/Company/GetAll"
        },
        "columns": [
                    { "data": "name", "width": "15%" },
                    { "data": "streetAddress", "width": "15%" },
                    { "data": "city", "width": "15%" },
                    { "data": "state", "width": "15%" },
                    { "data": "postalCode", "width": "15%" },
                    {
                        "data": "id",
                        "render": function (data) {
                            return `
                             <a href="/Admin/Company/Upsert?id=${data}" class="btn btn-success btn-sm"><i class="bi bi-pencil-square"></i></a>
                             <a onclick=Delete('/Admin/Company/Delete/${data}') class="btn btn-danger btn-sm"><i class="bi bi-trash3-fill"></i></a>
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
        text: "company will be deleted!",
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