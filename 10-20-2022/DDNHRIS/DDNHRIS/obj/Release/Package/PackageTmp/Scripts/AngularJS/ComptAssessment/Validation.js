
$(document).ready(function () {
    toastr.options = {
        "closeButton": false,
        "debug": false,
        "progressBar": true,
        "preventDuplicates": true,
        "positionClass": "toast-top-right",
        "onclick": null,
        "showDuration": "400",
        "hideDuration": "3000",
        "timeOut": "800",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }
});


app.controller('ComptAssessmentValidation', function ($scope, $http, $filter, $anchorScroll) {

    var s = $scope;
    var h = $http;
    
    s.displayTag = 0;
   

    s.supervisoryList = {};
    s.departmentHeadList = {};


    s.comptCoreList = {};
    s.comptLeadList = {};
    s.compTechList = {};

    s.respondent = {};
    s.showLoadingState = false;
    s.lockAnswer = true;
    s.formTag = 0;   
    s.hideLeadTag = true;    
    s.hideTechTag = true;
    
    loadValidationList();

    s.assessmentList = {};
        
    function loadValidationList() {
        h.post('../ComptAssessment/ValidatorsInitData').then(function (r) {
            if (r.data.status == "success") {
                s.assessmentList = r.data.assmntList;
            }
        });
    }
    
    s.gotoValidation = function (data)
    {      
        s.displayTag = 1;
        s.respondent = data;
        s.showLoadingState = true;
        h.post('../ComptAssessment/ValidateAssessment', {id: data.respondentCode, coreType: "CORE"}).then(function (r) {
            if (r.data.status == "success") {
                s.supervisoryList = r.data.supList;
                s.departmentHeadList = r.data.pghList;
                s.comptCoreList = r.data.core;
                s.comptLeadList = r.data.lead;
                s.compTechList = r.data.tech;

                s.formTag = 1;

                if (s.comptLeadList.length > 0) {
                    s.hideLeadTag = false;
                }
                if (s.compTechList.length > 0) {
                    s.hideTechTag = false;
                }

                if (r.data.respondent.tag > 2) {
                    s.lockAnswer = true;
                }
                else {
                    s.lockAnswer = false;
                }
                
            }
            s.showLoadingState = false;
        });
    }

    s.submitSupAns = function (a, b) {        
        h.post('../ComptAssessment/SubmitSupervisorAnswer', { respondentCode: s.respondent.respondentCode, KBICode: a, ans: b }).then(function (d) {
            if (d.data.status == "success") {
                toastr["success"]("Saving successful!", "Success");
            }
            else {
                toastr["error"]("Error sending answer, please reload your page.", "Error");              
            }
        });
    }
    
    s.backToList = function () {
        s.displayTag = 0;
    }


    s.submitSupervisorsValidation = function () {
        s.lockAnswer = true;
        h.post('../ComptAssessment/SubmitSupervisorValidation', { respondentCode: s.respondent.respondentCode }).then(function (r) {
            if (r.data.status == "success") {
                swal({ title: "Validation successfully saved!", text: "", type: "success", closeOnConfirm: true });
            }
            else {
                s.lockAnswer = false;
                toastr["error"](r.data.status, "Error");
                //var s = r.data.status;              
                //swal({ title: s, text: "Assessment Validation", type: "error", closeOnConfirm: true });
                //s.lockAnswer = false;
            }
        });
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

