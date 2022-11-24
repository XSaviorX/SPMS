
app.controller("SPMS_DivisionUtility", function ($scope, $window, $http, filterFilter) {
    var s = $scope;

    s.title = "hellooo";
    s.users = {};
    s.supervisor = '';
    s.currentOffice = 'OFFPHRMONZ3WT7D';
    s.currentDivision = 'DIVPROVIT8ZP';
    s.officeRoles = {};
    s.divisions;
    s.officeRoleID = "";
    s.divisionID = "";

    s.newUsers = [];
    s.users = [];
    s.cboxlength = 0;

    loadData();


    function loadData() {
        $http.post('../SPMS_DivisionUtility/getUsers', { OfficeId: s.currentOffice }).then(function (response) {

<<<<<<< HEAD
            //s.users = response.data.users;
=======
            s.userssssssss = response.data.users;
            console.log("userss: ", s.userssssssss);
>>>>>>> 47fbc6c6d5d64869ca61323cf26e35ffc94b0c6f
            s.officeRoles = response.data.officeRoles;
            console.log("officeRoles: ", s.officeRoles);
            s.divisions = response.data.divisions;
            console.log("divisions: ", s.divisions);

            s.listUsers = response.data.users;
            console.log("listUsers: ", s.listUsers);

            angular.forEach(s.listUsers, function (user, keyfirst) {

                s.users.push({
                    recNo: user.recNo, EIC: user.EIC, F_Name: user.F_Name, R_Description: user.R_Description, division: user.division,
                    divisionName: user.divisionName, officeId: user.officeId, officeName: user.officeName, officeNameShort: user.officeNameShort,
                    RID: user.RID, officeheadId: user.officeheadId, officeheadName: user.officeheadName, positionTitle: user.positionTitle,
                    supervisorId: user.supervisorId, supervisorName: user.supervisorName, oldOffcRoleId: user.RID, oldDivision: user.division,
                    recNoEmployee: user.recNoEmployee, recNoRole: user.recNoRole
                });

            });
            console.log("users: ", s.users);

        }), function (err) {
            alert(err);
        }
    }
    s.lengthUsers = 0;
    s.GetValue = function () {
        s.selectedUsers = [];
        s.lengthUsers = 0;
        for (var i = 0; i < s.users.length; i++) {
            if (s.users[i].Selected) {

                s.lengthUsers = s.lengthUsers + 1;
                s.selectedUsers.push(s.users[i]);
            }
        }
        if (s.lengthUsers !== 0) {
            $('#addMultiSup').modal('show');
        }
        else {
            Swal.fire(
                `No user selected.`,
                'Please click the checkbox to select a user.',
                'info'
            )
        }



        console.log("selectedUsers", s.selectedUsers);
    }
    s.addSups = [];
    s.onchangeSup = function (data) {
        s.addSups.push(data);
        console.log("s.addSups", s.addSups);
    }

    s.removedRole = function (recNo) {
        Swal.fire({
            title: 'Are you sure?',
            text: "",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes!'
        }).then((result) => {
            if (result.isConfirmed) {
                $http.post('../SPMS_DivisionUtility/removeRole', { RecNo: recNo }).then(function (response) {

                    if (response.data.status == 1) {
                        Swal.fire(
                            'Removed!',
                            'The Role has been removed.',
                            'success'
                        )
                        s.users = [];
                        loadData();
                    }

                }), function (error) {

                }

            }
        })
    }

    s.addManytoManyDivision = function () {
        console.log(" addManytoManyDivision | s.selectedUsers", s.users);
        console.log("s.selectedUsers", s.users);
        $http.post('../SPMS_DivisionUtility/addManytoManyDivision', { Users: s.users, OfficeId: s.currentOffice }).then(function (response) {
            if (response.data.status == 1) {
                s.users = [];
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


    s.addManytoOneDivRole = function () {

        console.log("s.divisionID", s.divisionID);
        console.log("s.officeRoleID", s.officeRoleID);

        $http.post('../SPMS_DivisionUtility/addManytoOneDivRole', { Users: s.selectedUsers, OfficeRoleID: s.officeRoleID, DivisionID: s.divisionID }).then(function (response) {
            if (response.data.status == 1) {
                $('#addMultiSup').modal('hide');

                s.users = [];
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

    // ---------------------------SHOW ADD ROLE -------------------------------------------
    s.showAddRole = function (data) {

        s.userRole = data;
        console.log("showAddRole", s.userRole);
    }
    // --------------------------- ADD ROLE -------------------------------------------
    s.office_RoleID;
    s.Is_Empty = false;
    s.saveRole = function () {
        console.log(" s.office_RoleID", s.office_RoleID);

        if (s.office_RoleID !== undefined) {
            $http.post('../SPMS_DivisionUtility/saveRole', { OfficeRoleID: s.office_RoleID, User: s.userRole }).then(function (response) {
                s.Is_Empty = false;

                if (response.data.status == 1) {
                    s.users = [];
                    loadData();
                    const Toast = Swal.mixin({
                        toast: true,
                        position: 'top-end',
                        showConfirmButton: false,
                        timer: 3000,
                        iconColor: '#FFFFFF',
                        color: '#FFFFFF',
                        timerProgressBar: true,
                        background: '#87EE04',
                        didOpen: (toast) => {
                            toast.addEventListener('mouseenter', Swal.stopTimer)
                            toast.addEventListener('mouseleave', Swal.resumeTimer)
                        }
                    })

                    Toast.fire({
                        icon: 'success',
                        title: 'Saved Successfully!'
                    })
                }

                if (response.data.status == 0) {

                    const Toast = Swal.mixin({
                        toast: true,
                        position: 'top-end',
                        showConfirmButton: false,
                        timer: 3000,
                        iconColor: '#FFFFFF',
                        color: '#FFFFFF',
                        timerProgressBar: true,
                        background: 'red',
                        didOpen: (toast) => {
                            toast.addEventListener('mouseenter', Swal.stopTimer)
                            toast.addEventListener('mouseleave', Swal.resumeTimer)
                        }
                    })

                    Toast.fire({
                        icon: 'danger',
                        title: 'Error! Role Already Exist.'
                    })
                }




            }), function (err) {
                alert(err);
            }
        }
        else {
            s.Is_Empty = true;
            console.log(" s.Is_Empty", s.Is_Empty);

        }


    }
    // ----------------------------ADD DIVSION----------------------------------------------
    s.divname = "";
    s.divisionAdd = function (data) {
        console.log("DIVISION ADD: ", data);
        $http.post('../SPMS_DivisionUtility/addDivision', { DivName: data, Office_ID: s.currentOffice }).then(function (response) {

            if (response.data.status == 1) {
                s.users = {};
                loadData();
                const Toast = Swal.mixin({
                    toast: true,
                    position: 'top-end',
                    showConfirmButton: false,
                    timer: 3000,
                    iconColor: '#FFFFFF',
                    color: '#FFFFFF',
                    timerProgressBar: true,
                    background: '#87EE04',
                    didOpen: (toast) => {
                        toast.addEventListener('mouseenter', Swal.stopTimer)
                        toast.addEventListener('mouseleave', Swal.resumeTimer)
                    }
                })

                Toast.fire({
                    icon: 'success',
                    title: 'Saved Successfully!'
                })
            }

            if (response.data.status == 0) {

                const Toast = Swal.mixin({
                    toast: true,
                    position: 'top-end',
                    showConfirmButton: false,
                    timer: 3000,
                    iconColor: '#FFFFFF',
                    color: '#FFFFFF',
                    timerProgressBar: true,
                    background: 'red',
                    didOpen: (toast) => {
                        toast.addEventListener('mouseenter', Swal.stopTimer)
                        toast.addEventListener('mouseleave', Swal.resumeTimer)
                    }
                })

                Toast.fire({
                    icon: 'danger',
                    title: 'Error! Division Already Exist.'
                })
            }




        }), function (err) {
            alert(err);
        }


    }

    s.rowCount = function (_EIC, data) {
        s.counts = 0;
        for (var user = 0; user < data.length; user++) {
            if (_EIC == data[user].EIC) {
                s.counts++;
            }
        }
        //s.counts = s.counts + (s.counts * 5);
        return s.counts;
    }

});

app.filter("unique", function () {
    return function (collection, keyname) {
        var output = [],
            keys = [];

        angular.forEach(collection, function (item) {
            var key = item[keyname];
            if (keys.indexOf(key) === -1) {
                keys.push(key);
                output.push(item);
            }
        });
        return output;
    };
});
