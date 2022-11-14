app.controller('PSCalculator', ['$scope', '$http', '$document', function (s, h, d) {

    s.tempClothing = [{ text: 'NOT QUALIFIED', val: 0 }, { text: 'QUALIFIED', val: 1 }];
    s.tempHazard = [{ text: 'NONE', value: 0 }, { text: 'Health Worker', value: 1 }, { text: 'Social Worker', value: 2 }];
    s.empStatusList = [{ text: '(ALL)', val: "00" }, { text: 'Casual', val: "05" }, { text: 'Job Order', val: "06" }, { text: 'Contract of Service', val: "07" }, { text: 'Honorarium', val: "08" }];

    s.initStatData = [{ text: 'Auto add appointee data', val: 1 }];

    s.fundSourceList = {};

    s.psPlanList = {};
    s.psPlanItem = {};
    s.psPlanData = {};
    s.data = {};

    s.tabId = 0;

    loadInitData();

    function loadInitData() {
        h.post('~/../../RSPUtility/PSCalculatorData').then(function (r) {
            if (r.data.status == "success") {
                s.psPlanList = r.data.list;
            }
        });
    }

    s.openModalPSPlan = function () {     
        if (s.fundSourceList.length == undefined) {
            h.post('~/../../RSPUtility/GetFundSourceList').then(function (r) {
                if (r.data.status == "success") {
                    s.fundSourceList = r.data.fundSourceList;
                    angular.element('#modalPSPlan').modal('show');
                }
            });
        }
        else {
            angular.element('#modalPSPlan').modal('show');
        }         
    }
    
    s.submitPlan = function (data) {
        s.bDisable = true;
        h.post('~/../../AppointmentNonPlantilla/SubmitPSPlan', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                s.bDisable = false;
                toastr["success"]("Saving successful!", "Success");
                angular.element('#modalPSPlan').modal('hide');
                s.tab = 0;
                loadInitData();  
            }
        });
    }

    s.backToList = function () {
        s.tabId = 0;
    }

    s.viewPSPlanList = function (data) {
        s.psPlanData = data;
        viewPSPlanItemList();
    }

    function viewPSPlanItemList() {
        h.post('~/../../RSPUtility/GetListByCode', { id: s.psPlanData.reportCode }).then(function (r) {
            if (r.data.status == "success") {
                s.psPlanItemList = r.data.psPlanItemList;
                s.tabId = 1;
            }
        });
    }
   
    s.editItemData = function (data) {
        s.psPlanItem = data;
        angular.element('#modalEdit').modal('show');
    }

    s.printPSPlan = function (data) {
        h.post('~/../../RSPUtility/PrintPSPlanByCode', { id: data.reportCode }).then(function (r) {
            if (r.data.status == "success") {
                s.bDisable = false;
                window.open("../Reports/HRIS.aspx");
            }
            else {
                toastr["error"]("Something went wrong...!", "Opps...");
            }
        });

    }

    s.stat = "";
    s.position = {};
    s.openModalAddName = function () {
        s.bDisable = true;
        h.post('~/../../RSPUtility/GetPlanStatus', { id: s.psPlanData.reportCode }).then(function (r) {
            if (r.data.status == "success") {
                s.bDisable = false;
                s.stat = r.data.stat;
                s.position = r.data.position;
                s.bDisable = false;
            }
            else {
                toastr["error"]("Something went wrong...!", "Opps...");
            }
        });
        angular.element('#modalAddName').modal('show');
    }


    s.submitTempName = function (data) {
        s.bDisable = true;
        data.reportCode = s.psPlanData.reportCode;
        h.post('~/../../AppointmentNonPlantilla/AddTempNameItem', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                s.bDisable = false;
                viewPSPlanItemList();
                toastr["success"]("PS updating successful!", "Success");
                angular.element('#modalAddName').modal('hide');
            }
            else {
                toastr["error"]("Something went wrong...!", "Opps...");
            }
        });
    }


    s.updateItemData = function (data) {       
        s.bDisable = true;
        data.reportCode = s.psPlanData.reportCode;    
        h.post('~/../../AppointmentNonPlantilla/EditPSComputation', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                s.bDisable = false;
                viewPSPlanItemList();
                toastr["success"]("PS updating successful!", "Success");
                angular.element('#modalAddName').modal('hide');
            }
            else {
                toastr["error"]("Something went wrong...!", "Opps...");
            }
        });
    }
    
    function formatMyDate(date) {
        return (moment(date).format('MM/DD/YYYY'));
    }

    s.formatDate = function (date) {
        if (!date) {
            return '(TBD)';
        }
        return (moment(date).format('MM/DD/YYYY'));
    };

    s.formatDateText = function (date) {
        if (!date) {
            return '';
        }
        return (moment(date).format('MMMM DD, YYYY'));
    };

    s.openModalSetupCalQ = function () {
        angular.element('#modalSetupCalQ').modal('show');
    }


    //s.generatePSData = function (data) {
    //    s.bDisable = true;

    //    var tempData = {};
    //    tempData.periodFrom = data.periodFrom;
    //    tempData.periodTo = data.periodTo;
    //    tempData.fundSourceCode = s.fundSourceData.fundSourceCode;
    //    tempData.employmentStatusCode = data.employmentStatusCode;

    //    h.post('~/../../AppointmentNonPlantilla/PSGenerator', { data: tempData }).then(function (r) {
    //        if (r.data.status == "success") {
    //            s.bDisable = false;
    //            window.open("../Reports/HRIS.aspx");
    //            //toastr["success"]("PS updating successful!", "Success");
    //        }
    //        else {
    //            toastr["error"]("Something went wrong...!", "Opps...");
    //        }
    //    });


    //}

}]);