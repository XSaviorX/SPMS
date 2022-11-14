app.controller('FundSourceProgram', ['$scope', '$http', '$document', function (s, h, d) {

    s.deptList = {};
    s.programList = {};
    s.bDisable = false;
    loadInitData();

    function loadInitData() {

        h.post('~/../../RSPFundSource/ProgramInitData').then(function (r) {
            if (r.data.status == "success") {
                s.deptList = r.data.deptList;
                s.programList = r.data.programList;
            }
        });
    }

    s.modalAdd = function () {
        angular.element('#modalAddProgram').modal('show');
    }

    s.SubmitProgram = function (data) {
        s.bDisable = true;
        h.post('~/../../RSPFundSource/SubmitProgram', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                s.programList = r.data.programList;
                angular.element('#modalAddProgram').modal('hide');
            }
            s.bDisable = false;
        });
    }

}]);