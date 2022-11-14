app.controller('VacantPosition', ['$scope', '$http', '$document', function (s, h, d) {


    s.isLoading = false;
    s.vacantList = {};
    s.vacantPositions = {};
    s.summData = {};
    s.reportList = {};
    s.department = {};

    //LOAD DATA
    loadInitData();
     
    function loadInitData() {        
        h.post('~/../../RSPUtility/VacantByMonthList').then(function (r) {
            if (r.data.status == "success") {           
                s.vacantList = r.data.list;
                s.summData = r.data.summData;
                s.reportList = r.data.reportList;
                s.department = r.data.department;
            }
        });
    }

    s.generateReport = function () {
        h.post('~/../../RSPUtility/GenerateVacantPositionSummary').then(function (r) {
            if (r.data.status == "success") {
                s.vacantList = r.data.list;
                s.summData = r.data.summData;
            }
        });
    }
     
    s.printSummary = function (data) {        
        h.post('~/../../RSPReport/VacantPositionSummary', { id: data.reportCode }).then(function (r) {
            if (r.data.status == "success") {                
                window.open("../Reports/HRIS.aspx");
            }
        });
    }
     

    s.showPrintModal = function () {     
        angular.element('#modalPrint').modal('show');
    }

    s.onSelectDepartment = function (id) {
        //alert(id);
        s.isLoading = true;
        s.vacantPositions = {};
        h.post('~/../../RSPUtility/GetVacantByFunctionCode', {id: id}).then(function (r) {
            if (r.data.status == "success") {
                s.vacantPositions = r.data.vacantPositions;
                s.isLoading = false;
                //s.vacantList = r.data.list;
                //s.summData = r.data.summData;
            }
        });
    }

    s.printVacantByOffice = function (data) {
      
        h.post('~/../../Plantilla/PrintPlantillaVacant', { id: data.functionCode }).then(function (r) {
            if (r.data.status == "success") {
                window.open("../Reports/HRIS.aspx");
            }
        });
    }

}]);