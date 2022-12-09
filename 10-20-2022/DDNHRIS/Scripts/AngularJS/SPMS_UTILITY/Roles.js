app.controller("SPMS_Roles", function ($scope, $window, $http, filterFilter) {
    var s = $scope;

    s.rolesmenu = [];
   
    loadData();

    function loadData() {
        $http.get('../SPMS_Roles/getRoles').then(function (response) {

            s.roles = response.data.roles;
            s.roles_menu = response.data.rolesmenu;
            s.menus = response.data.menus;

            angular.forEach(s.roles_menu, function (rm, keyfirst) {
                s.rolesmenu.push({
                    recNo: rm.recNo, rID: rm.rID, mID: rm.mID, old_mID: rm.mID, mDescription: rm.mDescription, R_Description: rm.R_Description
                });

               

            });
            console.log("roles: ", s.roles);
            console.log("rolesmenu: ", s.rolesmenu);
            console.log("menus: ", s.menus);


        }), function (err) {
            alert(err);
        }
    }

    s.addMenu = function (data) {
        console.log("Menu", data)
        s.rolesmenu.push({recNo: 0, rID: data.RID, R_Description: data.R_Description });
        console.log("rolesmenu", s.rolesmenu);
    }

    s.saveMenu = function () {
        console.log("rolesmenu: ", s.rolesmenu);
        console.log("savemenu: ", s.savemenu);

        if (s.savemenu.length > 0) {
            $http.post('../SPMS_Roles/saveMenu', { rolesMenu: s.savemenu }).then(function (response) {
                if (response.data.status == 1) {
                    s.roles_menu = {};
                    s.rolesmenu = [];
                    s.savemenu = [];
                    loadData();
                    const Toast = Swal.mixin({
                        toast: true,
                        position: 'top-end',
                        showConfirmButton: false,
                        timer: 3000,
                        iconColor: '#FFFFFF',
                        color: '#FFFFFF',
                        timerProgressBar: true,
                        background: '#24a0ed',
                        didOpen: (toast) => {
                            toast.addEventListener('mouseenter', Swal.stopTimer)
                            toast.addEventListener('mouseleave', Swal.resumeTimer)
                        }
                    })

                    Toast.fire({
                        icon: 'success',
                        title: "Saved Successfully!"
                    })
                }
            }), function (err) {
                alert(err);
            }
        }
      
    }


    s.removedMenu = function (data) {
        s.rolesmenu.splice(s.rolesmenu.findIndex(v => v.$$hashKey === data.$$hashKey), 1);
        if (data.recNo > 0) {
            $http.post('../SPMS_Roles/removedMenu', { recNo: data.recNo }).then(function (response) {
               
            }), function (err) {
                alert(err);
            }
        }
        const Toast = Swal.mixin({
            toast: true,
            position: 'top-end',
            showConfirmButton: false,
            timer: 3000,
            iconColor: '#FFFFFF',
            color: '#FFFFFF',
            timerProgressBar: true,
            background: '#24a0ed',
            didOpen: (toast) => {
                toast.addEventListener('mouseenter', Swal.stopTimer)
                toast.addEventListener('mouseleave', Swal.resumeTimer)
            }
        })

        Toast.fire({
            icon: 'success',
            title: "Removed Successfully!"
        })

        console.log("removedMenu", data)
        
    }
    s.savemenu = [];
    s.onChangeSelect = function (data) {
       // s.savemenu.find(v => v.$$hashKey === data.$$hashKey).mID = data.mID;
        s.isExist = s.savemenu.find(v => v.$$hashKey === data.$$hashKey);
       
        if (s.isExist) {
            console.log("true")
            s.isExist.mID = data.mID;
        }
        else {
            s.savemenu.push(data);

        }
        console.log(" s.savemenu", s.savemenu)

    }
});