app.controller('ServiceRecord', ['$scope', '$http', '$document', function (s, h, d) {

    s.serviceRecord = {};
    s.empList = {};
    s.serviceID = "";
    s.bDisable = false;
    s.serviceTag = 0;

    s.record = {};

    s.name = "";
    s.detail = "";

    s.SRPresentTag = 0;
    s.closeAfterSave = true;

    s.employmentStatus = [{ employmentStatusCode: '01', employmentStatus: 'ELECTIVE' }, { employmentStatusCode: '02', employmentStatus: 'CO-TERMINOUS' }, { employmentStatusCode: '03', employmentStatus: 'PERMANENT' }, { employmentStatusCode: '04', employmentStatus: 'TEMPORARY' }];
    s.salaryTypeList = [{ code: 'A', text: 'Annual' }, { code: 'M', text: 'Monthly' }, { code: 'D', text: 'Daily' }];
    s.branchTypeList = [{ code: 'PROVL', text: 'PROVINCIAL' }, { code: 'NATNL', text: 'NATIONAL' }, { code: 'CITY', text: 'CITY' }, { code: 'MUN', text: 'MUNICIPALITY' }]

    s.statusTypeList = [{ code: 'PERMANENT', text: 'PERMANENT' }, { code: 'CASUAL', text: 'CASUAL' }, { code: 'TEMPORARY', text: 'TEMPORARY' }, { code: 'CO-TERMINOUS', text: 'CO-TERMINOUS' }, { code: 'ELECTIVE', text: 'ELECTIVE' }, { code: 'EMERGENCY', text: 'EMERGENCY' }]
 
    loadInitData();

    function loadInitData() {
        s.name = "WAIT";
        s.bDisable = true;
        s.detail = "Fetching employee information . . .";
        h.post('~/../../Employee/ServiceRecordData/' + s.serviceID).then(function (r) {
            if (r.data.status == "success") {
                s.serviceRecord = r.data.serviceRecord;
                s.serviceTag = r.data.tag;
                s.name = s.serviceRecord.lastName + ", " + s.serviceRecord.firstName + " " + s.serviceRecord.middleName;
                var tmp = moment(s.serviceRecord.birthDate).format('MM/DD/YYYY');
                s.detail = tmp + " - " + s.serviceRecord.birthPlace;
                s.bDisable = false;               
            }            
        });
    }
    
    s.findEmployee = function () {
        if (s.empList.length == undefined) {
            s.bDisable = true;
            h.post('~/../../Employee/EmployeeList').then(function (r) {
                if (r.data.status == "success") {
                    s.empList = r.data.empList;
                    angular.element('#modalSelectEmployee').modal('show');
                }
            });
            s.bDisable = false;
        }
        else {
            angular.element('#modalSelectEmployee').modal('show');
            s.bDisable = false;
        }
              
    }

    s.selectEmployee = function (id) {
        s.bDisable = true;
        s.serviceRecord = {};
        h.post('~/../../Employee/ServiceRecordData/', {id: id}).then(function (r) {
            if (r.data.status == "success") {
                s.serviceRecord = r.data.serviceRecord;
                s.name = s.serviceRecord.lastName + ", " + s.serviceRecord.firstName + " " + s.serviceRecord.middleName;
                var tmp = moment(s.serviceRecord.birthDate).format('MM/DD/YYYY');
                s.detail = tmp + " - " + s.serviceRecord.birthPlace;
                s.serviceTag = r.data.tag;
                angular.element('#modalSelectEmployee').modal('hide');
                s.bDisable = false;
            }
        });
    }
    
    //CHECK MySQL Database
    s.checkMySQLRecord = function () {
        s.bDisable = true;
        h.post('~/../../Employee/ChecMySQLServiceRecord').then(function (r) {
            if (r.data.status == "success") {
                s.bDisable = false;
                s.serviceRecord = r.data.serviceRecord;
                toastr["info"]("Checking successful!", "Success");
                s.serviceTag = 2;
                s.bDisable = false;
                //angular.element('#modalMigrationRecord').modal('show');                
            }
        });
    }

    //START SR Migration
    s.migrateServiceRecord = function () {
        s.bDisable = true;
        h.post('~/../../Employee/MigrateServiceRecord').then(function (r) {
            if (r.data.status == "success") {
                s.bDisable = false;
                s.serviceTag = 1;
                toastr["success"]("Migration successful!", "Success");
                s.bDisable = false;
            }
        });
    }

    s.printServiceRecord = function (id) {       
        s.bDisable = true;      
        h.post('~/../../Employee/PrintServiceRecord', { id: id } ).then(function (r) {
            if (r.data.status == "success") {
                s.bDisable = false;
                window.open("../Reports/HRIS.aspx");
                s.bDisable = false;
            }
        });
    }

    s.modalSRCreate = function () {
        s.record = {};
        s.SRPresentTag = false;
        angular.element('#modalSRCreate').modal('show');
        s.bDisable = false;
    }

    s.saveNewRecord = function () {
        s.bDisable = true;        
        h.post('~/../../Employee/SaveNewRecord', { data: s.record, presentTag: s.SRPresentTag }).then(function (r) {
            if (r.data.status == "success") {
                toastr["success"]("Saving successful!", "Success");
                s.record = {};
                s.serviceRecord.serviceData = r.data.serviceRecord;
                s.SRPresentTag = false;
                if (s.closeAfterSave == true) {
                    angular.element('#modalSRCreate').modal('hide');
                }
            }
            else {
                toastr["error"](r.data.status, "Opps");
            }
            s.bDisable = false;
        });
    }
    
    //UPDATE SERVICE RECORD MODAL
    s.openModalSRItem = function (data) {
        s.record = {};
        s.record = data;
            
        if (data.dateToText == "PRESENT") {
            s.SRPresentTag = true;
        }
        else {
            s.record.dateTo = new Date(moment(data.dateTo).format('YYYY,MM,DD'));
        }
        angular.element('#modalSRData').modal('show');
    }

    //UPDATE SERVICE RECORD 
    s.udateServiceRecod = function () {
        s.bDisable = true;
        s.record.dateTo = moment(s.record.dateTo).format('MM/DD/YYYY');
        h.post('~/../../Employee/udateServiceRecod', { data: s.record, presentTag: s.SRPresentTag }).then(function (r) {
            if (r.data.status == "success") {
                toastr["success"]("Saving successful!", "Success");
                s.record = {};
                s.serviceRecord.serviceData = r.data.serviceRecord;
                if (s.closeAfterSave == true) {
                    angular.element('#modalSRData').modal('hide');
                }
            }
            else {
                toastr["error"](r.data.status, "Opps");
            }
            s.bDisable = false;
        });
    }

   
    s.formatDate = function (date) {
        if (!date) {
            return '';
        }
        return (moment(date).format('MM/DD/YYYY'));
    };

}]);