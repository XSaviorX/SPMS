app.controller('FundProject', ['$scope', '$http', '$document', function (s, h, d) {


   

    s.programs = {};

    s.projects = {};
    s.projectData = {};


    s.viewTab = 0;
      
    s.bDisable = false;

    loadInitData();


    s.yearList = [{ text: "2022", value: 2022 }];

    function loadInitData() {     
        h.post('~/../../RSPFundSource/ProjectByYear').then(function (r) {
            if (r.data.status == "success") {
                s.projects = r.data.projects;
                s.programs = r.data.programs;
            }
        });
    }

    s.ViewDetail = function (data) {
        s.projectData = data;
        s.viewTab = 1;
        //h.post('~/../../RSPFundSource/SetFundSourceCode/' + id).then(function (r) {
        //    if (r.data.status == "success") {
        //        window.location.href = "../RSPFundSource/ProjectDetail";
        //    }
        //});
    }

    s.backToList = function () {
        s.viewTab = 0;
    }

    s.modalAddProject = function () {
        s.editMode = false;
        angular.element('#modalAddProject').modal('show');     
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

}]);