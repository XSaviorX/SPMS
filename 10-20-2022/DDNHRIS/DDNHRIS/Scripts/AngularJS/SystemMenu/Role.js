
app.controller('SystemMenuRoleApp', ['$scope', '$http', '$document', function (s, h, d) {
    
    s.role = {};
    s.roleList = {};
    s.roleMenuList = {};

    s.menuList = {};
    
    loadData();

    function loadData() {
        h.post('~/../../SystemMenu/RoleList').then(function (r) {
            if (r.data.status == "success") {
                s.roleList = r.data.roleList;                
            }
        });
    }
    
    s.showRoleMenuList = function (roleID) {
        s.role = [];
        s.roleMenuList = {};      
        h.post('~/../../SystemMenu/RoleMenuList', {id: roleID}).then(function (r) {
            if (r.data.status == "success") {
                s.role = r.data.role;
                s.roleMenuList = r.data.roleMenuList;
            }
        });
    }
    
    s.openAddMenuToRole = function () {
        var roleID = s.role.roleID;       
        if (roleID == null || roleID ==undefined) {
           
        }
        else {
            h.post('~/../../SystemMenu/MenuListByRole', { id: roleID }).then(function (r) {
                if (r.data.status == "success") {
                    s.menuList = r.data.menuList;
                    $('#modalAddMenuToRole').modal('show');
                }
            });
        }
    }


    s.saveMenuToRole = function (id) {        
        h.post('~/../../SystemMenu/SaveMenuToRole', { id: id, roleID: s.role.roleID }).then(function (r) {
            if (r.data.status == "success") {
                s.menuList = r.data.menuList;
            }
        });
    }


    s.removeMenuFromRole = function (id) {        
        h.post('~/../../SystemMenu/RemoveMenuFromRole', { id: id, roleID: s.role.roleID }).then(function (r) {
            if (r.data.status == "success") {
                s.roleMenuList = r.data.roleMenuList;
            }
        });
    }

}]);