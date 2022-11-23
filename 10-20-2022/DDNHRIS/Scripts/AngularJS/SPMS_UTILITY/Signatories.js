app.controller("SPMS_Signatories", function ($scope, $window, $http, filterFilter) {
    var s = $scope;

    s.title = "hellooo";
    s.users = [];
    s.signatories = [];
    s.supervisor = '';
    s.currentOffice = 'OFFPHRMONZ3WT7D';
    s.currentDivision = 'DIVPROVIT8ZP';

    // Signatories
    s._officeHeadId = "ORCKHKLG582947";
    s._divisionHeadId = "ORHKWPAO195038";
    s._supervisorId = "ORGKTPDO206931";


    loadData();


    function loadData() {
        $http.post('../SPMS_Signatories/getUsers', { OfficeId: s.currentOffice }).then(function (response) {

            s.listUsers = response.data.users;
            angular.forEach(s.listUsers, function (user, keyfirst) {
                s.users.push({
                    recNo: user.recNo, EIC: user.EIC, F_Name: user.F_Name, R_Description: user.R_Description, division: user.division,
                    divisionName: user.divisionName, officeId: user.officeId, officeName: user.officeName, officeNameShort: user.officeNameShort,
                    officeRoleId: user.officeRoleId, officeheadId: user.officeheadId, officeheadName: user.officeheadName, positionTitle: user.positionTitle,
                    supervisorId: user.supervisorId, supervisorName: user.supervisorName, oldsupervisorId: user.supervisorId
                });

                if (user.RID == s._officeHeadId || user.RID == s._divisionHeadId || user.RID == s._supervisorId) {
                    s.signatories.push({
                        recNo: user.recNo, EIC: user.EIC, F_Name: user.F_Name, R_Description: user.R_Description, division: user.division,
                        divisionName: user.divisionName, officeId: user.officeId, officeName: user.officeName, officeNameShort: user.officeNameShort,
                        officeRoleId: user.officeRoleId, officeheadId: user.officeheadId, officeheadName: user.officeheadName, positionTitle: user.positionTitle,
                        supervisorId: user.supervisorId, supervisorName: user.supervisorName, oldsupervisorId: user.supervisorId
                    });
                }

            });
            console.log("users: ", s.users);
            console.log("signatories: ", s.signatories);


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
                s.users = [];
                // s.signatories = []
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

        $http.post('../SPMS_Signatories/addManytoOneSupervisor', { Users: s.selectedUsers, SupervisorId: s.supervisor, OfficeHeadId: 'OFFICEHEADID123', DivisionId: s.currentDivision }).then(function (response) {
            if (response.data.status == 1) {
                $('#addMultiSup').modal('hide');

                s.users = [];
                s.signatories = []
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
