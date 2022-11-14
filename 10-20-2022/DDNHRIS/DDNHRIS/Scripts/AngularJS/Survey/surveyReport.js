app.controller('SurveyReport', ['$scope', '$http', '$document', function (s, h, d) {

    s.survey = {};
    s.surveyData = {};
    s.department = {};
    s.displayTag = 0;
    s.bDisable = false;
    s.result = {};
    
    //LOAD DATA 
    loadSurveyReportData();
    
    function loadSurveyReportData() {
        h.post('~/../../Survey/ReportInitData').then(function (r) {
            if (r.data.status == "success") {
                s.survey = r.data.survey;
                s.department = r.data.department;
            }
        });
    }

    s.viewResult = function (data) {
        s.surveyData = data;
        s.displayTag = 1;
    }


    s.backToList = function () {
        s.displayTag = 0;
    }

    s.onChangeDept = function () {
        s.result = {};     
        if (s.departmentCode != null) {
            h.post('~/../../Survey/ViewReportById', { id: s.surveyData.surveyId, code: s.departmentCode }).then(function (r) {
                if (r.data.status == "success") {
                    s.result = r.data.list;
                }
            });
        }      
    }

    s.printReport = function () {
        s.bDisable = true;
        h.post('~/../../Survey/PrintSurveyById', { id: s.surveyData.surveyId, code: s.departmentCode }).then(function (r) {
            if (r.data.status == "success") {
                window.open("../Reports/HRIS.aspx");
                s.bDisable = true;
            }
        });
    }


}]);