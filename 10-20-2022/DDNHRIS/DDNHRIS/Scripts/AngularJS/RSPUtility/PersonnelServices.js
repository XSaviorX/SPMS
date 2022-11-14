app.controller('PersonnelServiceApp', ['$scope', '$http', '$document', '$filter', function (s, h, d, f) {
      
    
    s.data = {};
    s.workGroup = {};
    s.fundSource = {};
    s.salaryTable = {};
    s.psList = {};
    s.data = {};

    s.comptList = {};
    s.comptData = {};

    s.personnelService = {};
    s.editData = {};
    s.totalPS = 0;
    s.tab = 0;
    s.step = 1;
    s.bDisable = false;
    s.empStatFilter = "";
    
    s.emptStatus = [{ employmentStatus: 'PLANTILLA', employmentStatusCode: '03', employmentStatusShort: 'P' }, { employmentStatus: 'CASUAL', employmentStatusCode: '05', employmentStatusShort: 'CA' }, { employmentStatus: 'JOB ORDER', employmentStatusCode: '06', employmentStatusShort: 'JO' }, { employmentStatus: 'CONTRACT OF SERVICE', employmentStatusCode: '07', employmentStatusShort: 'COS' }, { employmentStatus: 'HONORARIUM', employmentStatusCode: '08', employmentStatusShort: 'HON' }];
   
    s.hazardList = [{ text: 'NONE', id: 0 }, { text: 'HEALTH WORKER', id: 1 }, { text: 'SOCIAL WORKER', id: 2 }];

    s.statusList = [{ text: 'ACTIVE', id: "ACTIVE" }, { text: 'PROMOTED', id: "PROMOTED" }, { text: 'RESIGNED', id: "RESIGNED" }, { text: 'SEPARATED', id: 'SEPARATED' }];

    s.deleteMode = false;
    
    loadInitData();

    function loadInitData() {
       s.bDisable = true;
       h.post('~/../../RSPUtility/PSInitData').then(function (r) {
           if (r.data.status == "success") {
               s.fundSource = r.data.fundSource;
               s.salaryTable = r.data.salaryTable;
               s.comptList = r.data.comptList;
               s.positions = r.data.positions;
               s.bDisable = false;
           }
       });
    }
     
     
   s.setEmpStatus = function (id) {
       return f('filter')(s.emptStatus, { employmentStatusCode: id })[0].employmentStatusShort;
   }
 
   s.createComputationName = function () {
       s.comptData.name = "";
       angular.element('#modalCreate').modal('show');
    }


   s.addNewCompName = function (str) {
       s.bDisable = true;
       h.post('~/../../RSPUtility/AddComputationData', { name: str }).then(function (r) {
           if (r.data.status == "success") {
               s.bDisable = false;
               s.comptList = r.data.comptList;
               angular.element('#modalCreate').modal('hide');
           }
       });
   }

    //add (1) Position
   s.addPosition = function (data) {
       data.PSCode = s.comptData.PSCode;
       s.bDisable = true;
       h.post('~/../../RSPUtility/AddPosition', { data: data }).then(function (r) {
           if (r.data.status == "success") {
               s.bDisable = false;
               s.personnelService = r.data.personnelService;
               angular.element('#modalAddAppointee').modal('hide');
           }
       });
   }

    //s.addPSComputation = function () {
    //    h.post('~/../../RSPUtility/AddComputationData', { data: s.comptData }).then(function (r) {
    //        if (r.data.status == "success") {
    //            angular.element('#modalCreate').modal('hide');
    //        }
    //    });
    //}
    //

   s.viewList = function (data) {
       s.empStatFilter = "";
        s.bDisable = true;
        s.comptData = data;
        h.post('~/../../RSPUtility/EmployeeListByCode', { id: s.comptData.PSCode }).then(function (r) {
            if (r.data.status == "success") {
                s.tab = 1;
                s.personnelService = r.data.personnelService;
                s.bDisable = false;
                s.totalPS = s.getTotalSum();
            }
        });
    }
     

   s.getTotalSum = function () {
       s.totalPS = 0;
       var total = 0;
       for (var count = 0; count < s.personnelService.length; count++) {           
           total += s.personnelService[count].totalPS;           
       }
       return total;
   }


    s.addTag = 1;
    s.modalOpenAppointee = function (tag) {
        s.addTag = tag;
        s.employee = {};
        s.data.fundSourceCode = "";
        s.data.employmentStatusCode = "";
        s.data.salaryCode = "";
        s.data = {};
        s.step = 1;
        angular.element('#modalAddAppointee').modal('show');
    }
    
    s.moveNext = function (data) {
        s.bDisable = true;
        h.post('~/../../RSPUtility/GeneratePSList', {data: data}).then(function (r) {
            if (r.data.status == "success") {               
                if (r.data.employee.length >= 1) {
                    s.step = 2;
                    s.employee = r.data.employee;
                    s.bDisable = false;
                }
                else {
                    alert("No data found!");
                    s.step = 1;
                    s.bDisable = false;
                }
            }
        });
    }

    //SAVE PS BY GROUP (CHARGES)
    s.savePSData = function (data) {     
        s.bDisable = true;
        s.data = data;
        s.data.PSCode = s.comptData.PSCode;       
        h.post('~/../../RSPUtility/SavePSCompData', { data: s.data, }).then(function (r) {
            if (r.data.status == "success") {
                angular.element('#modalAddAppointee').modal('hide');
                s.step = 1;
                s.bDisable = false;
                s.personnelService = r.data.personnelService;
                s.totalPS = s.getTotalSum();
                toastr["success"]("Saving succeful!", "Success");
            }
        });
    }

    s.backToComptList = function () {
        s.employee = {};
        s.data = {};
        s.step = 1;
        s.tab = 0;
    }

    s.backToList = function () {
        s.employee = {};
        s.data = {};
        s.step = 1;
        angular.element('#modalAddAppointee').modal('hide');
    }

    s.printMyPS = function () {
        s.bDisable = true;
        h.post('~/../../RSPUtility/PrintPS', { code:  s.comptData.PSCode }).then(function (r) {
            if (r.data.status == "success") {
                window.open("../Reports/HRIS.aspx");
                s.bDisable = false;
            }
        });
    }
    
    s.editRow = function (data) {
        s.bDisable = true;
        s.deleteMode = false;
        h.post('~/../../RSPUtility/EditPSItem', {  data: data}).then(function (r) {
            if (r.data.status == "success") {
                s.editData = r.data.editData;
                s.editData.periodFrom = new Date(moment(s.editData.periodFrom).format('YYYY,MM,DD'));
                s.editData.periodTo = new Date(moment(s.editData.periodTo).format('YYYY,MM,DD'));
                angular.element('#modalEdit').modal('show');
                s.bDisable = false;
            }
        });
    }

    s.updateRow = function (data) {
        s.bDisable = true;
        h.post('~/../../RSPUtility/UpdatePSItem', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                s.personnelService = r.data.personnelService;
                s.totalPS = s.getTotalSum();
                angular.element('#modalEdit').modal('hide');
                s.bDisable = false;
                toastr["success"]("Updating succeful!", "Success");
            }
        });
    }
     
    s.deletePSItem = function () {
        h.post('~/../../RSPUtility/DeletePSItem', { data: s.editData }).then(function (r) {
            if (r.data.status == "success") {
                s.personnelService = r.data.personnelService;
                s.totalPS = s.getTotalSum();
                angular.element('#modalEdit').modal('hide');
                s.bDisable = false;
                toastr["success"]("Record deleted!", "Success");
            }
        });
    }

}]);


