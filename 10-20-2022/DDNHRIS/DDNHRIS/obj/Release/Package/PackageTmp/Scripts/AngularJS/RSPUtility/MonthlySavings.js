app.controller('MonthlySavings', ['$scope', '$http', '$document', function (s, h, d) {

    
    //LOAD DATA 
    loadInitData();
    
    s.bDisable = false;
    s.bPrintDisable = true;

    s.department = {};
    s.monthList = {};
    s.savingsList = {};
     
    function loadInitData() {
        h.post('~/../../RSPUtility/MonthlySavingsInitData').then(function (r) {
            if (r.data.status == "success") {
                s.department = r.data.department;
                s.monthList = r.data.monthList;
            }
        });
    }
    
    s.openModalMonthlySaving = function () {
        angular.element('#modalSelectMonth').modal('show');
    }

    s.onChangeData = function () {
        s.savingsList = {};
        s.bPrintDisable = true;
    }

    s.createSavingsReport = function () {
        s.bDisable = true;
        s.savingsList = {};
        h.post('~/../../Plantilla/GenerateSavingsReport', { id: s.monthYearCode, code: s.departmentCode }).then(function (r) {
            if (r.data.status == "success") {
                s.savingsList = r.data.list;
                s.bDisable = false;
                s.bPrintDisable = false;
            }
        });
    }
       
    s.printPreview = function () {
        h.post('~/../../RSPUtility/PrintSavingsReport', { id: s.monthYearCode, code: s.departmentCode }).then(function (r) {
            if (r.data.status == "success") {
                window.open("../Reports/HRIS.aspx");
                //s.savingsList = r.data.list;
                //s.bDisable = false;
                //s.bPrintDisable = false;
            }
        });
    }

    s.formatDate = function (date) {
        if (!date) {
            return 'N/A';
        }
        return (moment(date).format('MM/DD/YYYY'));
    };


}]);