
app.controller('SystemUserRoleApp', ['$scope', '$http', '$document', function (s, h, d) {

    s.empList = {};
    s.userRoleList = {};
    s.sysRoleList = {};
    s.userData = {};


    s.searchEmployee = function () {
        if (s.empList.length == undefined) {
            h.post('~/../../SystemMenu/EmployeeList').then(function (r) {
                if (r.data.status == "success") {
                    s.empList = r.data.employeeList;
                    $('#modalEmpSearch').modal('show');
                }
            });
        }
        else {
            $('#modalEmpSearch').modal('show');
        }
    }
    
    s.selectEmployee = function (data) {
        h.post('~/../../SystemMenu/RoleListByEIC/' + data.EIC).then(function (r) {
            if (r.data.status == "success") {
                s.userData = r.data.userData;
                s.userRoleList = r.data.userRoleList;
                $('#modalEmpSearch').modal('hide');
            }
        });
    }
    
    s.modalOpenAddUserRole = function () {         
        var id = s.userData.EIC;      
        h.post('~/../../SystemMenu/ShowRoleList/' + id).then(function (r) {
            if (r.data.status == "success") { 
                s.sysRoleList = r.data.sysRoleList;
                $('#modalAddUserRole').modal('show');
            }
        });    
    }

    s.saveRoleToUser = function (id) {
        h.post('~/../../SystemMenu/SaveRoleToUser', {EIC: s.userData.EIC, roleID: id}).then(function (r) {
            if (r.data.status == "success") {
                s.userRoleList = r.data.userRoleList;
                s.sysRoleList = r.data.sysRoleList;                
            }
        });
    }
    
    s.removeRoleFromUser = function (id) {
        h.post('~/../../SystemMenu/RemoveRoleFromUser', { EIC: s.userData.EIC, roleID: id }).then(function (r) {
            if (r.data.status == "success") {
                s.userRoleList = r.data.userRoleList;
                s.sysRoleList = r.data.sysRoleList;
            }
        });
    }


}]);