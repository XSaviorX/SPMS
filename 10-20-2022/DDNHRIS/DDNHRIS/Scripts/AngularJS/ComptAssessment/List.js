app.controller('AssessmentList', ['$scope', '$http', '$document', function (s, h, d) {



    s.bDisable = true;
    s.assessmentList = {};
    s.assessmentData = {};

    s.employeeList = {};

    s.displayTag = 0;
    s.respondentList = {};

    s.data = {};
    s.data.appTypeCode = "1";
    s.appType = [{ appTypeCode: "1", text: "Insider" }, { appTypeCode: "0", text: "Outsider" }];

    loadInitData();

    function loadInitData() {
        h.post('~/../../ComptAssessment/AssessmentData').then(function (r) {
            if (r.data.status == "success") {
                s.assessmentList = r.data.list;
            }
        });
    }

    s.employee = {};
    s.applicant = {};
    s.supervisor = {};

    s.department = {};
    s.modalAddComptAssmnt = function () {
        if (s.comptPosition.length == undefined) {
            h.post('~/../../ComptAssessment/GetDepartmentList').then(function (r) {
                if (r.data.status == "success") {
                    s.comptPosition = r.data.comptPosition;
                    s.department = r.data.department;
                    angular.element('#modalAddComptAssment').modal('show');
                    s.bDisable = false;
                }
            });
        }
        else {
            angular.element('#modalAddComptAssment').modal('show');
        }
    }

    s.comptPosition = {};
    s.onSelectOffice = function (id) {        
        h.post('~/../../ComptAssessment/GetComptPositionById', {id: id}).then(function (r) {
            if (r.data.status == "success") {
                s.comptPosition = r.data.comptPosition;
                //s.department = r.data.department;
                //angular.element('#modalAddComptAssment').modal('show');
                s.bDisable = false;
            }
        });      
    }

    //SAVE ASSESSMENT
    s.assmntData = {};
    s.submitComptAssessment = function (data) {
        s.bDisable = true;
     
        data.assmntGroupCode = data.departmentCode;
        h.post('~/../../ComptAssessment/SaveComptAssessment', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                s.assessmentList = r.data.list;
                s.bDisable = false;
                s.assessmentData = data;
                s.assessmentData.assessmentCode = r.data.code;
                s.displayTag = 1;
            }
        });
    }

  
    s.addRespondent = function () {
        s.bDisable = false;
        h.post('~/../../ComptAssessment/ComptAssmntPreData').then(function (r) {
            if (r.data.status == "success") {
                s.employee = r.data.employee;
                s.applicant = r.data.applicant;
                s.supervisor = r.data.supervisor;
                angular.element('#modalAddRespondent').modal('show');
            }
            else {

            }
        });
    }

    //SaveRespondent  
    s.submitRespondentData = function (data) {
        var myData = {};
        myData.assessmentCode = s.assessmentData.assessmentCode;
        myData.EIC = data.EIC;
        myData.supervisorEIC = data.supervisorEIC;
        h.post('~/../../ComptAssessment/SaveRespondent', { data: myData }).then(function (r) {
            if (r.data.status == "success") {
                s.respondentList = r.data.respList;
                toastr["success"]("Respondent added!", "Good");
            }
            else {
                toastr["error"](r.data.status, "Opps");
            }
        });
    }

   

    s.backToList = function () {
        s.displayTag = 0;
    }

    s.viewRespondent = function (data) {
        s.respondentList = {};
        s.displayTag = 1;
        s.assessmentData = data;
        reloadRespondent();
        //h.post('~/../../ComptAssessment/ViewRespondentListByCode', { id: s.assessmentData.assessmentCode }).then(function (r) {
        //    if (r.data.status == "success") {
        //        s.respondentList = r.data.respList;
        //        s.bDisable = false;
        //    }
        //});
    }

     
    s.computeComptRating = function (data) {
        s.bDisable = true;
        h.post('~/../../ComptAssessment/ComputeRespondentResult', { id: data.respondentCode }).then(function (r) {
            if (r.data.status == "success") {
                toastr["success"]("Successful!", "Good");
                reloadRespondent();
                //s.respondentList = r.data.respList;
                s.bDisable = false;
            }
        });

    }


    function reloadRespondent() {       
        h.post('~/../../ComptAssessment/ViewRespondentListByCode', { id: s.assessmentData.assessmentCode }).then(function (r) {
            if (r.data.status == "success") {
                s.respondentList = r.data.respList;
                s.bDisable = false;
            }
        });
    }
    
    
    s.viewAssmntData = function (data) {
        var tag = 1;
        if (s.supervisor.length == undefined) {
            tag = 0;
        }        
        s.assmntData.respondentCode = data.respondentCode;
        s.assmntData.fullNameLast = data.fullNameLast;
        s.assmntData.supervisorEIC = "";        
        s.bDisable = true;

        h.post('~/../../ComptAssessment/ViewMyData', { id: s.assmntData.respondentCode, tag: tag }).then(function (r) {
                if (r.data.status == "success") {
                    if (tag == 0) {                   
                        s.supervisor = r.data.supervisors;                    
                    }              
                    s.assmntData.supervisorEIC = r.data.supervisorEIC;               
                    s.bDisable = false;
                    angular.element('#modalComptAssmentData').modal('show');
                }
        });

        //h.post('~/../../ComptAssessment/ViewMyData', { id: s.assmntData.respondentCode, tag: tag }).then(function (r) {
        //    if (r.data.status == "success") {
        //        if (tag == 0) {                   
        //            s.supervisor = r.data.supervisors;                    
        //        }              
        //        s.assmntData.supervisorEIC = r.data.supervisorEIC;               
        //        s.bDisable = false;
        //        angular.element('#modalComptAssmentData').modal('show');
        //    }
        //});

       
    }
    
    s.updateAssessmentData = function () {
        h.post('~/../../ComptAssessment/UpdateSupervisor', { data: s.assmntData }).then(function (r) {
            if (r.data.status == "success") {              
                s.bDisable = false;
                toastr["success"]("Updating successful!", "Good");
                angular.element('#modalComptAssmentData').modal('hide');
                s.bDisable = false;
            }
            else {
                toastr["error"]("Error updating...", "Opps");
            }
        });
    
    }

}]);