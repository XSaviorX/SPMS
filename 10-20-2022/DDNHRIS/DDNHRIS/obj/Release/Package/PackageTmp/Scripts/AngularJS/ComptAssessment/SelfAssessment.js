$(document).ready(function () {
    toastr.options = {
        "closeButton": false,
        "debug": false,
        "progressBar": true,
        "preventDuplicates": false,
        "positionClass": "toast-top-right",
        "onclick": null,
        "showDuration": "400",
        "hideDuration": "800",
        "timeOut": "800",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }
});

app.controller('SelfAssessment', ['$scope', '$http', '$document', function (s, h, d) {


    s.assessmentList = {};
    s.assessmentData = {};
    s.tabId = 0;
    s.testLoading = false;

    s.competency = {};
    s.comptCoreList = {};
    s.comptLeadList = {};
    s.compTechList = {};

    s.psychoTest = {};
    s.lockAnswer = false;
    s.hideLeadTag = true;
    s.hideTechTag = true;

    s.tabId = 0;  // 0 -  Application; 1-JobSearch

    loadInitData();

    function loadInitData() {
        h.post('~/../../ComptAssessment/GetMyAssessmentList').then(function (r) {
            if (r.data.status == "success") {
                s.assessmentList = r.data.assmntList;
            }
        });
    }

    s.takeMyTest = function (data) {
        s.assessmentData = data;
        angular.element('#modalReminder').modal('show');
    }

    s.backToList = function () {
        s.tabId = 0;
    }

    s.competency = {};
    s.startComptAssessment = function (data) {

        if (s.competency.length == undefined) {
            h.post('~/../../ComptAssessment/CheckCompetencyAssessment/' + s.assessmentData.respondentCode).then(function (r) {
                if (r.data.status == "success") {

                    s.competency = r.data.competency;
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

                    if (r.data.competency.tag > 1) {
                        s.lockAnswer = true;
                    }
                    else {
                        s.lockAnswer = false;
                    }
                    angular.element('#modalReminder').modal('hide');
                }
                s.tabId = 1;
                s.loading = false;
            });
        }
    }

     
    //SUBMIT COMPETENCY ANSWER
    s.submitAns = function (a, b) {
        if (s.lockAnswer == false) {
            h.post('~/../../ComptAssessment/SubmitMyAnswer', { applicationCode: s.assessmentData.respondentCode, KBICode: a, ans: b }).then(function (d) {
                if (d.data.status == "success") {
                    toastr["success"]("Saving successful!", "Success");
                }
                else {
                    toastr["error"]("An error occured, please try to refresh the page.", "Error");
                }
            });
        }
    }


    //SUBMIT PSYCHO ANSWER
    s.submitPsychoAns = function (psychoCode, key) {
        if (s.lockAnswer == false) {
            h.post('../ComptAssessment/SubmitMyPsychoAnswer', { applicationCode: s.assessmentData.respondentCode, psychoCode: psychoCode, ans: key }).then(function (d) {
                if (d.data.status == "success") {
                    toastr["success"]("Saving successful!", "Success");
                }
                else {
                    toastr["error"]("An error occured, please try to refresh the page.", "Error");
                }
            });
        }
    }


    //SUBMIT SELF MY ASSESSMENT FOR COMPUTRATION / VALIDATION (IF ANY)
    s.submitSelfAssessment = function () {
        s.lockAnswer = true;
        h.post('../ComptAssessment/SubmitSelfAssessment', { id: s.assessmentData.respondentCode }).then(function (d) {
            if (d.data.status == "success") {
                swal({
                    title: "Assessment submitted!",
                    text: "",
                    type: "success",
                    closeOnConfirm: true
                });
                loadInitData();
            }
            else {
                s.lockAnswer = false;
                toastr["error"](d.data.status, "Error");
            }
        });
    }



}]);




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
