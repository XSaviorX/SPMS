app.controller("SPMS_Signatories", function ($scope, $window, $http, filterFilter) {
    var s = $scope;

    s.title = "hellooo";
    s.users = {};
    s.supervisor = '';
    s.currentOffice = 'OFFPHRMONZ3WT7D';
    s.currentDivision = 'DIVPROVIT8ZP';

    loadData();


    function loadData() {
        $http.post('../SPMS_Signatories/getUsers', { OfficeId: s.currentOffice }).then(function (response) {

            s.users = response.data;
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
   /* s.addSups = [];
    s.onchangeSup = function(data){
        s.addSups.push(data);
        console.log("s.addSups", s.addSups);
    }*/
    s.addManytoManySupervisor = function () {

        console.log("s.selectedUsers", s.users);
        $http.post('../SPMS_Signatories/addManytoManySupervisor', { Users: s.users, OfficeHeadId: 'OFFICEHEADID123', DivisionId: s.currentDivision }).then(function (response) {
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
    s.addManytoOneSupervisor = function () {
        console.log("s.supervisor", s.supervisor);

        $http.post('../SPMS_Signatories/addManytoOneSupervisor', {Users: s.selectedUsers, SupervisorId: s.supervisor , OfficeHeadId: 'OFFICEHEADID123', DivisionId: s.currentDivision }).then(function (response) {
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

});