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
    loadData();


    function loadData() {
        $http.post('../SPMS_DivisionUtility/getUsers', { OfficeId: s.currentOffice }).then(function (response) {

            s.users = response.data.users;
            console.log("users: ", s.users);
            s.officeRoles = response.data.officeRoles;
            console.log("officeRoles: ", s.officeRoles);
            s.divisions = response.data.divisions;
            console.log("divisions: ", s.divisions);

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

    s.addManytoManyDivision = function () {
        console.log(" addManytoManyDivision | s.selectedUsers", s.users);
        console.log("s.selectedUsers", s.users);
        $http.post('../SPMS_DivisionUtility/addManytoManyDivision', { Users: s.users, OfficeId: s.currentOffice}).then(function (response) {
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

        $http.post('../SPMS_DivisionUtility/addManytoOneDivRole', { Users: s.selectedUsers, OfficeRoleID: s.officeRoleID, DivisionID: s.divisionID}).then(function (response) {
            if (response.data.status == 1) {
                $('#addMultiSup').modal('hide');

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


});