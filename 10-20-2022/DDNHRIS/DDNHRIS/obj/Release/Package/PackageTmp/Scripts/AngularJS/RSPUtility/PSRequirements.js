app.controller('PSRequirement', ['$scope', '$http', '$document', function (s, h, d) {


    s.year = 2021;
    s.psList = {};    
    s.deptData = {};
    s.deptData.functionCode = "";
    s.deptData.departmentName = "-- Please select Department -- ";
   
    s.departmentList = [
           { functionCode: "OC191015134332380001", departmentName: 'PGO' }
    ];
    
    //LOAD DATA
    //loadInitData();
    loadInitialData();
    s.code = "";
    
    function loadInitialData() {
        h.post('~/../../Plantilla/ProposedInitData').then(function (r) {
            if (r.data.status == "success") {
                s.departmentList = r.data.functionList;              
            }
        });
    }


    function loadInitData() {
        s.psList = {};
        h.post('~/../../RSPUtility/PSRequirementByDeptCode', { id: s.deptData.functionCode, sy: s.year }).then(function (r) {
            if (r.data.status == "success") {
                s.psList = r.data.psList;
                s.code = r.data.code;               
            }
        });
    }
    
    s.setYear = function (id) {
        //alert(id);
        s.year = id;
        loadInitData(); //PrintPSSetup
    }

    s.modalSelectDepartment = function () {
        $('#modalDepartment').modal('show');
    }
    
    s.selectedDepartment = function () {       
        var id = s.deptData.functionCode;
        var itemData = s.departmentList.filter(function (item) {
            return item.functionCode === id;
        })[0];        
        s.deptData = itemData;
        s.psList = [];     
        loadInitData();
        $('#modalDepartment').modal('hide');
    }
     
    s.printPS = function () {

        h.post('~/../../RSPUtility/PrintPSSetup', { id: s.deptData.functionCode, sy: s.year }).then(function (r) {
            if (r.data.status == "success") {
                window.open("../Reports/HRIS.aspx");
            }
        });
    }


}]);