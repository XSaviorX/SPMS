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


app.controller('JobApplication', ['$scope', '$http', '$document', function (s, h, d) {

    s.applicationList = {};

    s.vacantPositions = {};
    s.positionData = {};
    s.jobDescList = {};

    s.loading = false;

    s.appliedPositionData = {};

    s.competency = {};
    s.comptCoreList = {};
    s.comptLeadList = {};
    s.compTechList = {};
    s.psychoTest = {};
    s.lockAnswer = false;
    s.hideLeadTag = true;
    s.hideTechTag = true;

    s.tabId = 0;  // 0 -  Application; 1-JobSearch

    s.jobAppTabId = 0;
    s.myAppTabId = 0;

    loadInitData();


    s.changeTab = function (id) {
        s.tabId = id;

        if (s.tabId == 1) {
            s.jobAppTabId = 0;
            if (s.jobVacantList == undefined) {

                h.post('~/../../Application/VacantPosition').then(function (r) {
                    if (r.data.status == "success") {
                        s.jobVacantList = r.data.vacantPositions;
                    }
                    s.bDisable = false;
                });
            }
        }
    }

    function loadInitData() {
        h.post('~/../../Application/applicationList').then(function (r) {
            if (r.data.status == "success") {
                s.applicationList = r.data.applicationList;
            }
        });
    }

    s.selectAppliedPosition = function (data) {
        s.appliedPositionData = data;
        s.myAppTabId = 1;
        s.competency = {};
        s.competency.tag = -1;
        s.competency.applicationCode = "";
    }

    s.loadCompetencyAssessment = function () {

        if (s.competency.applicationCode == "") {
            h.post('~/../../Application/CheckCompetencyAssessment/' + s.appliedPositionData.applicationCode).then(function (r) {
                if (r.data.status == "success") {
                    s.loading = false;
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
                }
                s.loading = false;
            });
        }
    }

    //SUBMIT COMPETENCY ANSWER
    s.submitAns = function (a, b) {
        if (s.lockAnswer == false) {

            h.post('~/../../Application/SubmitAnswer', { applicationCode: s.competency.applicationCode, KBICode: a, ans: b }).then(function (d) {
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
            h.post('../Application/SubmitPsychoAnswer', { applicationCode: s.competency.applicationCode, psychoCode: psychoCode, ans: key }).then(function (d) {
                if (d.data.status == "success") {
                    toastr["success"]("Saving successful!", "Success");
                }
                else {
                    toastr["error"]("An error occured, please try to refresh the page.", "Error");
                }
            });
        }
    }

    //SUBMIT SELF ASSESSMENT
    s.submitSelfAssessment = function () {
        s.lockAnswer = true;
        h.post('../Application/SubmitSelfAssessment', { id: s.competency.applicationCode }).then(function (d) {
            if (d.data.status == "success") {
                swal({
                    title: "Assessment is successfully submitted!",
                    text: "",
                    type: "success",
                    closeOnConfirm: true
                });
            }
            else {
                s.lockAnswer = false;
                toastr["error"](d.data.status, "Error");
            }
        });
    }

    /////////////////////////////////////////////////////////////////////////////////////////////


    s.backToList = function () {
        s.jobAppTabId = 0;
    }

    s.selectPosition = function (data) {
        s.positionData = data;
        h.post('~/../../Application/PositionJD/' + data.publicationItemCode).then(function (r) {
            if (r.data.status == "success") {
                s.jobDescList = r.data.jobDescList;
                s.jobAppTabId = 1;
                //s.jobVacantList = r.data.vacantPositions;
            }
            s.bDisable = false;
        });

    }


    s.applyThisJob = function () {
        h.post('~/../../Application/ApplyThisJob/' + s.positionData.publicationItemCode).then(function (r) {
            if (r.data.status == "success") {
                loadInitData();
                toastr["success"]("Saving successful!", "Success");
                s.tabId = 0;
                s.jobAppTabId = 0;
                //s.jobDescList = r.data.jobDescList;
                //s.jobAppTabId = 1;
                //s.jobVacantList = r.data.vacantPositions;
            }
            else {
                toastr["error"](r.data.status, "Opps!");
            }
            s.bDisable = false;
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
