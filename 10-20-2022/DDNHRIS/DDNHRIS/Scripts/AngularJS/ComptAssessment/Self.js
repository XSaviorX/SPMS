
$(document).ready(function () {
    toastr.options = {
        "closeButton": false,
        "debug": false,
        "progressBar": true,
        "preventDuplicates": false,
        "positionClass": "toast-top-right",
        "onclick": null,
        "showDuration": "400",
        "hideDuration": "1000",
        "timeOut": "800",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }
});


app.controller('ComptAssessmentSelf', function ($scope, $http, $filter, $anchorScroll) {

    var s = $scope;
    var h = $http;

    s.respondentCode = "";

    s.competency = null;

    s.myAssessmentList = {};

    s.hideLeadTag = true;
    s.hideTechTag = true;

    s.comptCoreList = {};
    s.comptLeadList = {};
    s.comptTechList = {};
    s.psychoTest = {};

    s.comptAssmntTag = 0;
    s.lockAnswer = false;

    s.displayTag = 0;
    s.bDisable = false;
    s.buttonText = "Wait..."
   
    checkMyAssessmentList();

    function checkMyAssessmentList() {        
        h.post('../ComptAssessment/RespondentAssessmentList').then(function (r) {
            if (r.data.status == "success") {
                s.myAssessmentList = r.data.list;
            }
        }); 
    }

    s.takeAssessment = function (id) {
        loadAssesment(id);
    }
 

    function loadAssesment(id) {

        s.comptAssmntTag = 0;      

        s.hideLeadTag = true;
        s.hideTechTag = true;
        s.bDisable = true;
        h.post('../ComptAssessment/ShowCompetencyByCategory', { id: id }).then(function (r) {

            if (r.data.status == "none") {
                s.comptAssmntTag = 1; //NO COMPT ASSESSTMENT
            }
            else if (r.data.status == "success") {
                s.displayTag = 1;
                s.comptAssmntTag = 2; //WITH ASSESSTMENT SURVERY
                s.respondentCode = r.data.respondentCode;
                s.standardStatus = r.data;
                s.comptCoreList = r.data.core;
                s.comptLeadList = r.data.lead;
                s.compTechList = r.data.tech;
                s.psychoTest = r.data.psychoTest;

                if (s.comptLeadList.length > 0) {
                    s.hideLeadTag = false;
                }
                if (s.compTechList.length > 0) {
                    s.hideTechTag = false;
                }

                if (r.data.respondent.respondentTag >= 1) {
                    s.lockAnswer = true;
                }
                else {
                    s.lockAnswer = false;
                }

        
            }
            else if (r.data.status == "error") {
                //to be used soon...
            }
        });
    }

    //SUBMIT ANSWER
    s.submitAns = function (a, b) {
        if (s.lockAnswer == false) {
            h.post('../ComptAssessment/SubmitAnswer', { respondentCode: s.respondentCode, KBICode: a, ans: b }).then(function (d) {
                if (d.data.status == "success") {
                    toastr["success"]("Saving successful!", "Success");
                }
                else {
                    toastr["error"]("An error occured, please try to refresh the page.", "Error");
                }
            });
        }
    }

    //SUBMIT ANSWER
    s.submitPsychoAns = function (psychoCode, key) {
      
        if (s.lockAnswer == false) {         
            h.post('../ComptAssessment/SubmitPsychoAnswer', { respondentCode: s.respondentCode, psychoCode: psychoCode, ans: key }).then(function (d) {
                if (d.data.status == "success") {
                    toastr["success"]("Saving successful!", "Success");
                }
                else {
                    toastr["error"]("An error occured, please try to refresh the page.", "Error");
                }
            });
        }
    }


    s.submitAssessmentSurvey = function () {
        s.lockAnswer = true;
        h.post('../ComptAssessment/SubmitSelftComptAssmnt', { id: s.respondentCode }).then(function (d) {
            if (d.data.status == "success") {
                swal({
                    title: "Assessment is successfully saved!",
                    text: "",
                    type: "success",
                    closeOnConfirm: true
                });               
            }
            else {
                
                s.lockAnswer = false;

                toastr["error"](d.data.status, "Error");

                //swal({
                //    title: s,
                //    text: "Assessment Survey",
                //    type: "error",
                //    closeOnConfirm: true
                //});              
            }            
        });
    }

    s.backToList = function () {
        s.comptAssmntTag = 0;
    }






});


app.directive('ngDisabled', function () {
    return {
        controller: function ($scope, $attrs) {
            var self = this;
            $scope.$watch($attrs.ngDisabled, function (isDisabled) {
                self.isDisabled = isDisabled;
            });
        }
    };
});

function reactToParentNgDisabled(tagName) {
    app.directive(tagName, function () {
        return {
            restrict: 'E',
            require: '?^ngDisabled',
            link: function (scope, element, attrs, ngDisabledController) {
                if (!ngDisabledController) return;
                scope.$watch(function () {
                    return ngDisabledController.isDisabled;
                }, function (isDisabled) {
                    element.prop('disabled', isDisabled);
                });
            }
        };
    });
}

['input', 'select', 'button', 'textarea'].forEach(reactToParentNgDisabled);

