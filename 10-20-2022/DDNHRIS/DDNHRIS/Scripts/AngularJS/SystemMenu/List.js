

//app.controller("SystemMenuListApp", function ($scope, $http) {


$(document).ready(function () {
    toastr.options = {
        "closeButton": true,
        "debug": false,
        "newestOnTop": true,
        "progressBar": true,
        "positionClass": "toast-top-right",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }
});



app.controller('SystemMenuListApp', ['$scope', '$http', '$document', function (s, h, d) {


    s.menuList = {};
    s.menuGroup = {};
    s.menuSubGroup = {};

    loadData();

    function loadData() {
      
        h.post('~/../../SystemMenu/MenuList').then(function (r) {
            if (r.data.status == "success") {
                s.menuList = r.data.list;
                s.menuGroup = r.data.menuGroup;
                s.menuSubGroup = r.data.menuSubGroup;
            }
        });
    }

    s.openModal_AddMenu = function () {
        $('#modal_AddMenu').modal('show');
    }

    s.saveMenu = function (data) {
        h.post('~/../../SystemMenu/SaveMenuData', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                s.menuList = r.data.list;
                $('#modal_AddMenu').modal('hide');
                swal({
                    title: "Success!",
                    text: "Successfull submission was made",
                    type: "success",
                    showCancelButton: false,
                    confirmButtonText: "OK, Good!",
                    closeOnConfirm: true
                });
            }
        });
    }


}]);