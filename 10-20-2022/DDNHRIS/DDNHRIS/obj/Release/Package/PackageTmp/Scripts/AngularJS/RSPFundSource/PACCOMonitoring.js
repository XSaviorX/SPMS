

app.controller('PACCOMonitoring', ['$scope', '$http', '$document', function (s, h, d) {

    s.projList = {};
    s.programList = {};
    s.bDisable = false;

    s.detailTag = 0;
    s.viewTag = 0;

    s.fsData = {};
    s.fsDetails = {};
    s.postedList = {};
    s.appnteeList = {};
    s.appointmentList = {};
    s.balance = 0;
    s.rate = 0;

    s.apptData = {};
    s.fundData = {};
    s.empData = {};
    s.empPS = {};
    s.unPosted = {};

    s.casualTag = 0;
    s.hazardTag = 0;
 
    s.saveDisable = true;
    s.postData = {};

    s.fundItemLabel = "Add Funds";
    s.fundItemTag = 0;
    s.fundItemData = {};
    s.fundItemReadOnly = true;

    s.searchList = {};
    s.appCount = 0;

    s.tabId = 0;
    s.appointmentCode = "";

    s.notification = {};
    s.notifyItemData = {};

    s.fundSourceData = {};
    s.excludedList = {};

    s.tempClothing = [{ text: 'NOT QUALIFIED', val: 0 }, { text: 'QUALIFIED', val:1 }];
    s.tempHazard = [{ text: 'NONE', value: 0 }, { text: 'Health Worker', value: 1 }, { text: 'Social Worker', value: 2 }];

    s.data = {};

    loadInitData();

    function loadInitData() {
        h.post('~/../../RSPFundSource/ProjectMonitoring').then(function (r) {
            if (r.data.status == "success") {
                s.projList = r.data.projList;
                s.appCount = r.data.iCount;
                s.notifyCount = r.data.notifyCount;
            }
        });
    }
      
    s.viewDetail = function (id) {
       
        loadProjectData(id);
        
    }
    
    function loadProjectData(id) {
         
        h.post('~/../../RSPFundSource/ProjectItemData/' + id).then(function (r) {
            if (r.data.status == "success") {
                
                s.fundSourceData = r.data.fundSourceData;
 
                s.approList = r.data.approList;
                s.notifyCount = s.fundSourceData.empList.length;              
                s.detailTag = 1;               
            }
        });
    }

    s.backToList = function () {
        s.detailTag = 0;
    }

    function loadIncomingAppList() {        
        h.post('~/../../RSPFundSource/AppointmentList').then(function (r) {
            if (r.data.status == "success") {
                s.appCount = r.data.iCount;
                s.appointmentList = r.data.appList;
            }
        });
    }

    s.viewApptDetail = function (data) {
        s.appointmentCode = data.appointmentCode;
        s.apptData = data;
        viewAppointeeList();
    }

    //INCOMING APPOINTMENT : List
    function viewAppointeeList() {
        var id = s.appointmentCode;
        h.post('~/../../RSPFundSource/AppointeeListByFundSource/' + id).then(function (r) {
            if (r.data.status == "success") {
                s.fundData = r.data.fundData;
                s.appnteeList = r.data.list;
                s.viewTag = 1;
                s.unPosted = r.data.unPosted;
            }
        });
    }


    s.viewApptFromSearch = function(id) {        
        s.appointmentCode = id;
        h.post('~/../../RSPFundSource/AppointeeListByFundSource2/' + id).then(function (r) {
            if (r.data.status == "success") {
                s.apptData = r.data.apptData;
                s.fundData = r.data.fundData;
                s.appnteeList = r.data.list;
                s.viewTag = 1;
                s.unPosted = r.data.unPosted;
                s.tabId = 1;
            }
        });
    }

    
    s.backToAppList = function () {
        s.viewTag = 0;
    }

    s.viewAppointmentItemData = function (data) {
        var id = data.appointmentItemCode;
        s.saveDisable = true;
        s.bDisable = true;
        s.empData = data;
         
        s.data.periodFrom = new Date(moment(s.empData.periodFrom).format('YYYY,MM,DD'));
        s.data.periodTo = new Date(moment(s.empData.periodTo).format('YYYY,MM,DD'));
      
        h.post('~/../../RSPFundSource/ViewAppointmentItem/' + id).then(function (r) {
            if (r.data.status == "success") {
                s.empPS = r.data.ps;
                s.casualTag = r.data.casualTag;
                s.hazardTag = r.data.hazardTag;
                angular.element('#modalAppointmentPosting').modal('show');
                s.bDisable = false;
            }
        });       
    }




    //VIEW APPOINTMENT CONFIRMATION
    s.recalculatePS = function (data) {
         
        s.data = data;     
        var id = s.empData.appointmentItemCode;        
        if (s.hazardTag == 0) {
            s.data.hazardCode = 0;
        }

        if (s.casualTag == 1) {           
            if (s.data.hasClothing >= 0 && s.data.hasMidYear >= 0 && s.data.hasYearEnd >= 0 && s.data.hazardCode >= 0) {
                
            }
            else {
                toastr["error"]("Please full-up the required data!", "Error!");
                return;
            }
        }
        else {         
            s.data.hazardCode = 0;
            s.data.hasClothing = 0;
            s.data.hasMidYear = 0;
            s.data.hasYearEnd = 0;
        }

      
        s.bDisable = true;
        s.saveDisable = true;
      
        h.post('~/../../RSPFundSource/ReCalculatePS', {id: id, postData: s.data}).then(function (r) {
            if (r.data.status == "success") {
                s.empPS = r.data.ps;
                s.saveDisable = false;
                s.bDisable = false;
            }
            else {
                toastr["error"](r.data.status, "Opps!");
                s.saveDisable = true;
                s.bDisable = false;
            }
        });
    }

    //CONFIRM & POST APPOINTMENT ITEM
    s.confirmAppoinmentItem = function (data) {
        s.data = data;
        var id = s.empData.appointmentItemCode;
        if (s.hazardTag == 0) {
            s.data.hazardCode = 0;
        }
        if (s.casualTag == 1) {
            if (s.data.hasClothing >= 0 && s.data.hasMidYear >= 0 && s.data.hasYearEnd >= 0 && s.data.hazardCode >= 0) {

            }
            else {
                toastr["error"]("Please full-up the required data!", "Opps!");
                return;
            }
        }
        else {
            s.data.hazardCode = 0;
            s.data.hasClothing = 0;
            s.data.hasMidYear = 0;
            s.data.hasYearEnd = 0;
        }

        s.bDisable = true;
        s.saveDisable = true;
        s.saveDisable = true;
        h.post('~/../../RSPFundSource/ConfirmAppointmentItem', { id: s.empData.appointmentItemCode, postData: s.data, unPostedCount: s.unPosted }).then(function (r) {
            if (r.data.status == "success") {
                viewAppointeeList();
                s.empPS = r.data.ps;
                s.saveDisable = false;
                s.bDisable = false;

                s.unPosted = r.data.unPosted;
              
                toastr["success"]("Posting successful!", "Success");
                angular.element('#modalAppointmentPosting').modal('hide');
            }
            else {
                toastr["error"](r.data.status, "Error");
                s.saveDisable = false;
                s.bDisable = false;
            }
        });

    }





    //UPDATING MODAL
    s.modalAppointmentEdit = function (id) {
        s.bDisable = true;
        h.post('~/../../RSPFundSource/modalAppointmentEdit', { id: id }).then(function (r) {
            if (r.data.status == "success") {
                s.bDisable = false;
            }
        });
    }

    s.editFundItem = function () {
        s.fundItemReadOnly = false;
    }

    s.viewFundItem = function (data) {    
        h.post('~/../../RSPFundSource/FundItemData', { id: data.recNo }).then(function (r) {
            if (r.data.status == "success") {
                s.fundItemData = r.data.fundItem;
                s.fundItemData.amount = r.data.fundItem.amount;
                s.fundItemLabel = "Edit Funds";
                s.fundItemTag = 1;
                s.fundItemReadOnly = true;               
                angular.element('#modalFundItemDetail').modal('show');
            }
        });
    }

    //s.updateFundItem = function (data) {     
    //    h.post('~/../../RSPFundSource/UpdateFundItemData', { data: data }).then(function (r) {
    //        if (r.data.status == "success") {              
    //            s.fundItemTag = 1;
    //            s.fundItemReadOnly = true;
    //            toastr["success"]("Update successful!", "Success");                
    //            loadProjectData(s.fundData.fundSourceCode);                
    //        }
    //    });
    //}

 

    s.updateFundItem = function (data) {
        h.post('~/../../RSPFundSource/UpdateFundItemData', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                s.fundItemTag = 1;
                s.fundItemReadOnly = true;
                toastr["success"]("Update successful!", "Success");
                //loadProjectData(s.fundData.fundSourceCode);
                loadProjectData(s.fundSourceData.fundSourceCode);
            }
        });
    }


    //MODAL ADDITIONAL FUNDS
    s.modalAddFundItem = function () {
        //alert(s.fundSourceData.fundSourceCode);
        s.fundAdd = {};
        s.fundAdd.fundSourceCode = s.fundSourceData.fundSourceCode;
        s.fundItemReadOnly = false;
        angular.element('#modalAddFunds').modal('show');
    }
    //SAVING...
    s.saveAdditionalFund = function (data) {
      
        var myData = {};
        s.fundAdd.particulars = data.particulars;
        s.fundAdd.amount = data.amount;
               
        h.post('~/../../RSPFundSource/SaveAdditionalFund', { data: s.fundAdd }).then(function (r) {
            if (r.data.status == "success") {
                s.bDisable = false;
                angular.element('#modalAddFunds').modal('hide');
                toastr["success"]("Saving successful!", "Success");
                loadProjectData(s.fundSourceData.fundSourceCode);
            }
            else {
                toastr["error"]("Something went wrong...!", "Opsss");
            }
        });
    }

    s.searchAppointee = function (id) {
        s.bDisable = true;
        h.post('~/../../RSPFundSource/SearchAppointee', { id: id }).then(function (r) {
            if (r.data.status == "success") {
                s.searchList = r.data.list;
                s.bDisable = false;
                 
            }
        });
    }
      
    s.showTab = function (id) {
        s.tabId = id;
        if (id == 1) {
            loadIncomingAppList();
        }
        if (id == 3) {
            showNotification(0);
        }
        if (id == 4) {
            showExcludedList();
        }
    }

    //NOTIFICATION
    function showNotification(id) {
        if (s.notification.length == undefined || id == 1) {
            s.bDisable = true;
            h.post('~/../../RSPFundSource/GetHRNotification').then(function (r) {
                if (r.data.status == "success") {
                    s.notification = r.data.list;
                    s.notifyCount = r.notification.length;
                    s.bDisable = false;
                }
            });
        }
    }

    //SHOW NOTIFICATION MODAL
    s.notificationDetails = function (data) {
        s.notifyItemData = data;
        s.bDisable = false;
        s.casualTag = 0;
        s.postData = {};
        s.empPS = {};
        if (s.notifyItemData.employmentStatusCode == '05') {
            s.casualTag = 1;             
            s.postData.hasClothing = s.notifyItemData.hasClothing; 
            s.postData.hasMidYear = data.hasBonusMY; 
            s.postData.hasYearEnd = data.hasBonusYE;
            s.postData.hazardCode = data.hazardCode;
        }
       
        angular.element('#modalNotification').modal('show');
      
    }

    //recalculate adjustment
    s.recalculatePSAdjmnt = function (data) {
       
        if (s.casualTag == 1) {
            if (s.postData.hasClothing >= 0 && s.postData.hasMidYear >= 0 && s.postData.hasYearEnd >= 0 && s.postData.hazardCode >=0) {
               
            }
            else {
                toastr["error"]("Please full-up the required data!", "Opps...");
                return;
            }
        }
        else {
            data.hazardCode = 0;
            data.hasClothing = 0;
            data.hasMidYear = 0;
            data.hasYearEnd = 0;
        }
               
        s.bDisable = true;
        s.saveDisable = true;
        data.appointmentItemCode = s.notifyItemData.appointmentItemCode;
        data.periodFrom = formatMyDate(s.notifyItemData.periodFrom);
        data.periodTo = formatMyDate(s.notifyItemData.periodTo);
        data.termCode = s.notifyItemData.termCode;
              
        h.post('~/../../RSPFundSource/ReCalculatePSAdjsmnt', { appItemCode: s.notifyItemData.notificationCode, postData: data }).then(function (r) {
            if (r.data.status == "success") {
                s.empPS = r.data.ps;
                s.saveDisable = false;
                s.bDisable = false;              
            }
            else {
                toastr["error"]("Something went wrong...!", "Opsss");
            }
        });
    }

    //SAVE ADJUSTMENT
    s.saveAdjustment = function (data) {        

        if (s.casualTag == 1) {
            if (s.postData.hasClothing >= 0 && s.postData.hasMidYear >= 0 && s.postData.hasYearEnd >= 0 && s.postData.hazardCode >= 0) {

            }
            else {
                toastr["error"]("Please full-up the required data!", "Opps...");
                return;
            }
        }
        //var id = s.empData.appointmentItemCode;
        s.bDisable = true;
        s.saveDisable = true;
        data.appointmentItemCode = s.notifyItemData.appointmentItemCode;
        data.periodFrom = formatMyDate(s.notifyItemData.periodFrom);
        data.periodTo = formatMyDate(s.notifyItemData.effectiveDate);
        data.termCode = s.notifyItemData.termCode;
        
        h.post('~/../../RSPFundSource/SaveAppointmentAdjsmnt', {  appItemCode: s.notifyItemData.notificationCode, postData: data }).then(function (r) {
            if (r.data.status == "success") {
                s.empPS = r.data.ps;
                s.saveDisable = false;
                s.bDisable = false;                
                s.notification = r.data.notify;
                s.notifyCount = s.notification.length;
                angular.element('#modalNotification').modal('hide');
                toastr["success"]("Saving successful...", "Success");
            }
            else {
                toastr["error"]("Please full-up the required data!", "Opps...");
            }
        });
    }


    //EXCLUDED LIST
    function showExcludedList() {       
        if (s.excludedList.length == undefined) {          
            h.post('~/../../RSPFundSource/ExcludedList').then(function (r) {
                if (r.data.status == "success") {
                    s.excludedList = r.data.list;                 
                }
                else {
                    toastr["error"]("Something went wrong...!", "Opsss");
                }
            });
        }         
    }


    s.showEmployeePS = function (data) {
        s.notifyItemData = data;
        s.casualTag = 0;
        if (s.notifyItemData.employmentStatusCode == '05') {
            s.casualTag = 1;
        }
         
        h.post('~/../../RSPFundSource/ShowEmployeePS', { postData: data }).then(function (r) {
            if (r.data.status == "success") {
                s.empPS = r.data.ps;
                s.saveDisable = false;
                s.bDisable = false;
                angular.element('#modalEmployeePS').modal('show');
            }
            else {
                toastr["error"]("Something went wrong...!", "Opsss");
            }
        });      
    }

   

    //PRINT FUND SROUCE     
    s.printByFundSource = function (data) {
       
        h.post('~/../../RSPReport/ReportByFundSource', { id: data.fundSourceCode }).then(function (r) {
            if (r.data.status == "success") {
                window.open("../Reports/HRIS.aspx");
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

}]);

