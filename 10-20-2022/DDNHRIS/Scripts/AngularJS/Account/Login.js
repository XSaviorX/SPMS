

toastr.options = { "closeButton": true, "debug": false, "newestOnTop": false, "progressBar": true, "positionClass": "toast-top-right", "preventDuplicates": true, "onclick": null, "showDuration": "300", "hideDuration": "1000", "timeOut": "3000", "extendedTimeOut": "1000", "showEasing": "swing", "hideEasing": "linear", "showMethod": "fadeIn", "hideMethod": "fadeOut" }

app.controller("LoginApp", function ($scope, $http) {

    var s = $scope;
    s.msg = "Login";
    s.bDisable = false;

    s.LogMeIn = function (data) {
        if (data.username == undefined || data.username.length <= 5 || data.password == undefined || data.password <= 5) {
            toastr["error"]("Invalid username or password!", "Login");
            return;
        }
        login(); 
    }

    s.getkeys = function (event) {
        if (event.keyCode == 13) {
            login();
        }
    }

    function login() {
        s.bDisable = true;
        var data = {};
        data.username = s.data.username;
        data.password = s.data.password;
        s.msg = "Wait...";
        $http.post('~/../../Account/Login', { data: data }).then(function (result) {
            if (result.data.status == "success") {
                window.location.href = "../../Home/Dashboard";
            }
            else {
                s.bDisable = false;
                s.msg = "Login";
                toastr["error"]("Invalid username or password", "Login");
            }
        }, function (result) {
            s.bDisable = false;
            toastr["error"]("Invalid username or password", "Login");
        });
    }


});