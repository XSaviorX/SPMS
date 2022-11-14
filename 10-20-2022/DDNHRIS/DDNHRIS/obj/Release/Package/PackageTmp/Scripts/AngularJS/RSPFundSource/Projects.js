app.controller('FundSources', ['$scope', '$http', '$document', function (s, h, d) {


    s.progList = {};
    s.projList = {};
    s.yearList = {};

    s.deptList = {};
    s.programList = {};
    s.editMode = false;

    s.fundSourceData = {};

    s.bDisable = false;

    s.budgetYear = 2022;

    loadInitData();

    function loadInitData() {     
        h.post('~/../../RSPFundSource/ProjectInitData', {year: s.budgetYear}).then(function (r) {
            if (r.data.status == "success") {
                s.projList = r.data.projList;
                s.progList = r.data.progList;
                s.yearList = r.data.yearList;
            }
        });
    }

    s.ViewDetail = function (id) {       
        h.post('~/../../RSPFundSource/SetFundSourceCode/' + id).then(function (r) {
            if (r.data.status == "success") {
                window.location.href = "../RSPFundSource/ProjectDetail";
            }
        });
    }

    s.data = {};
    s.modalAdd = function () {
        s.editMode = false;
        s.data.CY = s.budgetYear;
        angular.element('#modalAddProject').modal('show');
        //h.post('~/../../RSPFundSource/GetDeptList').then(function (r) {
        //    if (r.data.status == "success") {
        //        //s.progList = r.data.progList;               
        //        //s.yearList = r.data.yearList;
        //        angular.element('#modalAddProject').modal('show');
        //    }
        //});
    }

      
    s.SubmitFundSource = function (data) {
        s.bDisable = true;
        h.post('~/../../RSPFundSource/SubmitFundSource', {data: data} ).then(function (r) {
            if (r.data.status == "success") {
                s.progList = r.data.progList;
                s.yearList = r.data.yearList;
                s.projList = r.data.projList;
                angular.element('#modalAddProject').modal('hide');
            }
            s.bDisable = false;
        });
    }
    
    //EDIT DATA
    s.editData = function (id) {      
        h.post('~/../../RSPFundSource/EditFundSource/' + id).then(function (r) {
            if (r.data.status == "success") {
                s.editMode = true;

                s.projData = r.data.fundSource;
                s.projData.CY = s.projData.CY;
                s.editMode = true;
                angular.element('#modalProjectUpdate').modal('show');
            }
        });
    }

    //UDPATE PROJECT
    s.SubmitUpdate = function (data) {       
        h.post('~/../../RSPFundSource/UpdateFundSource', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                s.projList = r.data.projList;
                angular.element('#modalProjectUpdate').modal('hide');
                loadInitData();
                s.editMode = false;
                toastr["success"]("Updating succeful!", "Success");
            }
        });
    }
    
    s.showPrograms = function () {      
        h.post('~/../../RSPFundSource/ProgramInitData').then(function (r) {
            if (r.data.status == "success") {
                s.deptList = r.data.deptList;
                s.programList = r.data.programList;
            }
        });
    }
    
    s.modalAddProgram = function () {
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

    s.setBudgetYear = function (year) {
        if (s.budgetYear != year) {
            s.budgetYear = year;
            loadInitData();
            toastr["success"]("Budget year updated!", "Success");
        }
      
    }

}]);