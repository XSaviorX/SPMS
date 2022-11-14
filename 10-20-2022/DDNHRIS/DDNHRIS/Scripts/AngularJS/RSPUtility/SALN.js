app.controller('SALN', ['$scope', '$http', '$document', function (s, h, d) {
        

    s.bDisable = false;
    s.deptData = {};
    s.dept = {};
    s.emps = {};
    s.empData = {};
    s.data = {};
    s.empSALNData = {};
    s.batchCode = "SALN202012310001";

    s.empList = {};

    //LOAD DATA 
   
    s.deptData.departmentCode = "OC191015134332380001";
    s.shortDepartmentName = "PGO";
    
    loadInitData();

    function loadInitData() {     
        h.post('~/../../RSPUtility/GetSALNById', { id: s.deptData.departmentCode }).then(function (r) {            
            if (r.data.status == "success") {          
                s.dept = r.data.dept;
                s.emps = r.data.emps; 
            }
        });
    }
    
    s.modalSelectDepartment = function () {
        angular.element('#modalDepartment').modal('show');
    }

    s.departmentSelected = function (data) {         
        h.post('~/../../RSPUtility/GetSALNByEmployeeByOfficeId', { id: data.departmentCode }).then(function (r) {
            if (r.data.status == "success") {
                s.emps = r.data.emps;
                s.deptData = data;
                toastr["info"]("New Office selected!", "Info");
                angular.element('#modalDepartment').modal('hide');
            }
        }); 
    }

    s.selectEmployee = function (data) {      
        s.empData = data; 
        s.data.isSpouseGovtService = false;
        s.data.isJointFiling = false;
        s.data.networth = "";
        s.data.spouseData = "";
        h.post('~/../../RSPUtility/GetSALNBatchByEmpId', { code: s.batchCode, id: data.EIC }).then(function (r) {
            if (r.data.status == "success") {
                s.data = r.data.data;
                
                if (s.data.spouseRemarks != null) {
                    if (s.data.spouseRemarks.length > 5) {
                        s.data.isSpouseGovtService = true;
                        if (s.data.isJointFiling == 1) {
                            s.data.isJointFiling = true;
                        }
                    }
                }

                angular.element('#modalEntryForm').modal('show');
            }
        });      
    }
    
    s.submitSALNData = function (data) {       
        var tempData = {};
        tempData.EIC = s.empData.EIC;
        tempData.batchCode = s.batchCode;
        tempData.networth = data.networth;
        tempData.isJointFiling = data.isJointFiling;
        tempData.spouseRemarks = data.spouseRemarks;
        tempData.positionTitle = s.empData.positionTitle;

        h.post('~/../../RSPUtility/UpdateEmployeeSALN', { data: tempData, code: s.deptData.departmentCode }).then(function (r) {
            if (r.data.status == "success") {
                s.emps = r.data.emps;
                angular.element('#modalEntryForm').modal('hide');
            }
        });       
    }
    
    s.printSALNReport = function () {
        h.post('~/../../RSPUtility/PrintReportSALN', { code: s.deptData.departmentCode }).then(function (r) {
            if (r.data.status == "success") {
                //s.bDisable = false;
                window.open("../Reports/HRIS.aspx");
            }
        });
    }
    
    s.modalAddEmployee = function () {
        if (s.empList.length == undefined) {
            h.post('~/../../RSPUtility/GetEmployeeList').then(function (r) {
                if (r.data.status == "success") {
                    s.empList = r.data.list;
                    angular.element('#modalAddForm').modal('show');
                }
            });
        }
        else {
            angular.element('#modalAddForm').modal('show');
        }
    }
    
    s.addEmployee = function (data) {
        h.post('~/../../RSPUtility/AddEmpToSALN', { id: data.EIC, code: s.deptData.departmentCode }).then(function (r) {
            if (r.data.status == "success") {
                s.emps = r.data.emps;
                angular.element('#modalAddForm').modal('hide');
            }
        });         
    }

}]);