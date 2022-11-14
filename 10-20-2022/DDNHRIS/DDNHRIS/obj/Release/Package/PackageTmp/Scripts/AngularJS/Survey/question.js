app.controller('SurveyQuestion', ['$scope', '$http', '$document', function (s, h, d) {


    s.surveys = {};
    s.surveyData = {};
    s.questions = {};

    //LOAD DATA 
    loadInitData();
    s.bDisable = false;
    s.displayTag = 0;

    function loadInitData() {
        h.post('~/../../Survey/Questions').then(function (r) {
            if (r.data.status == "success") {
                s.surveys = r.data.surveys;
            }
        });
    }

    s.setType = function (id) {
        if (id == 1) {
            return "number"
        }
        else if (id == 2) {
            return "text"
        }
        else if (id == 3) {
            return "date"
        }
    }

    s.takeMySurvey = function (data) {
        s.surveyData = data;
        s.bDisable = true;
        h.post('~/../../Survey/SurveyQuestions', { id: data.respondentId }).then(function (r) {
            if (r.data.status == "success") {
                s.questions = r.data.questions;
                s.displayTag = 1;
                s.bDisable = false;
            }
            else {
                s.bDisable = false;
            }
        });
    }

    s.backToList = function () {
        s.displayTag = 0;
    }



    s.formatDate = function (date) {
        if (!date) {
            return 'N/A';
        }
        return (moment(date).format('MM/DD/YYYY'));
    };


    function comfirmedSubmit() {
        s.bDisable = true;
        h.post('~/../../Survey/SubmitSurveyResponse', { data: s.questions, id: s.surveyData.respondentId }).then(function (r) {
            if (r.data.status == "success") {
                s.displayTag = 0;
                s.bDisable = false;
                loadInitData();
            }
            else {
                toastr["error"](r.data.status, "Info");
                s.bDisable = false;
            }
        });
    }

    s.confirmSubmit = function () {         
        s.isLoading = true;
        //s.bDisable = true;
        swal({
            title: 'Are you sure?',
            text: "Submit Survey",
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#2196f3",
            confirmButtonText: "Yes",
            cancelButtonText: "Cancel",
            closeOnConfirm: true
        }, function (value) {
            if (value == true) {             
                comfirmedSubmit();
            }          
        });
    }


}]);