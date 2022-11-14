app.controller('FSProjDetail', ['$scope', '$http', '$document', function (s, h, d) {


    s.msg = "Welcome";
    s.fsData = {};
    s.fsDetails = {};
    s.appnteeList = {};
    s.balance = 0;
    s.rate = 0;
    s.fundDetail = {};
    s.bDisable = false;
    s.saveDisable = true;

    s.casualTag = 0;

    s.data = {};
    s.empPS = {};
    s.updateAlert = 0;

    s.fundSourceData = {};
    s.approList = {};
    s.termExclnTag = 0;

    s.forceUpdate = false;

    s.tempClothing = [{ text: 'NOT QUALIFIED', val: 0 }, { text: 'QUALIFIED', val: 1 }];
    s.tempHazard = [{ text: 'NONE', value: 0 }, { text: 'Health Worker', value: 1 }, { text: 'Social Worker', value: 2 }];

    s.empStatusList = [{ text: 'Casual', val: "05" }, { text: 'Job Order', val: "06" }, { text: 'Contract of Service', val: "07" }, { text: 'HONORARIUM', val: "08" }];
    
    s.termList = [
        { text: 'TERMINATION', value: "TERMINATION" },
        { text: 'CHANGESTATUS', value: "CHANGE STATUS" },
        { text: 'PROMOTION', value: "PROMOTION" },
        { text: 'RESIGNATION', value: "RESIGNATION" },
        { text: 'ADJUSTMENT', value: "ADJUSTMENT" },
        { text: 'EXCLUSION ***', value: "EXCLUSION" }
    ];

    loadInitData();
    
    function loadInitData() {
        h.post('~/../../RSPFundSource/ProjectDetailData').then(function (r) {
            if (r.data.status == "success") {

                s.fundSourceData = r.data.fundSourceData;
                s.approList = r.data.approList;                
                 
            }
        });
    }

    s.openAdddModal = function () {
        s.bDisable = false;
        s.fundDetail = {};
        angular.element('#modalAddDetail').modal('show');
    }

    s.SubmitAddtnlFund = function (data) {
        s.bDisable = true;
        h.post('~/../../RSPFundSource/SubmitAddtnlFund', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                s.fsData = r.data.fsData;
                s.fsDetails = r.data.fsDetails;
                s.appnteeList = r.data.appnteeList;
                s.balance = r.data.balance;
                s.rate = r.data.rate;
                angular.element('#modalAddDetail').modal('hide');
            }
        });
    }

    s.data = {};
    s.setAdjustment = function (data) {

        s.data.termCode = "";
      
      
        

        s.bDisable = false;
        s.data = {};
        s.data = data;
        
        s.casualTag = 0;        
        if (s.data.employmentStatusCode == "05") {
            s.casualTag = 1;
            s.data.hazardTag = 0;          
        }

        var tmp = new Date(moment(s.data.periodFrom).format('YYYY,MM,DD'));
       
        //alert(tmp.getFullYear())
        //s.data.periodFrom != undefined ||

        if ( tmp.getFullYear() > 2020) {         
            s.data.adjstmntFrom = new Date(moment(s.data.periodFrom).format('YYYY,MM,DD'));
        }

        s.data.adjstmntTo = new Date(moment(s.data.periodTo).format('YYYY,MM,DD'));
        angular.element('#modalAdjstmnt').modal('show');
        s.forceUpdate = false;
    }
    
    //ONCHANGE
    s.onChangeTermcode = function () {
        s.termExclnTag = 0;    
        if (s.data.termCode == "EXCLUSION")
        {           
            s.termExclnTag = 1;
        }
              
        if (s.data.termCode != null  && s.data.termCode != "") {
            s.saveDisable = false;
        }
    }

    //recalculate adjustment
    s.recalculatePSAdjmnt = function (data) {
       
        if (data.termCode == "" || data.termCode == undefined) {
            toastr["error"]("Please full-up the required data!", "Opps...");
            return;
        }
        if (s.casualTag == 1) {
            if (data.hasClothing >= 0 && data.hasMidYear >= 0 && data.hasYearEnd >= 0 && data.hazardCode >= 0) {
               
            }
            else {
                toastr["error"]("Please full-up the required data!", "Opps...");
                return;
            }
        }
             
        s.bDisable = true;
        s.saveDisable = true;
        data.appointmentItemCode = s.data.appointmentItemCode;
        data.periodFrom = formatMyDate(s.data.adjstmntFrom);
        data.periodTo = formatMyDate(s.data.adjstmntTo);
        data.termCode = s.data.termCode;
        
        //  h.post('~/../../RSPFundSource/ReCalculatePSAdjsmnt', { appItemCode: s.notifyItemData.notificationCode, postData: data }).then(function (r) {
        h.post('~/../../RSPFundSource/ReCalculatePSAdjsmnt', { appItemCode: "", postData: data }).then(function (r) {
            if (r.data.status == "success") {
                s.empPS = r.data.ps;
                s.saveDisable = false;
                s.bDisable = false;
            }
            else {
                toastr["error"]("Something went wrong...!", "Opps...");
            }
        });
    }

    //SAVE ADJUSTMENT
    s.saveAdjstmntRequest = function (data) {
        s.bDisable = true;
       
        if(data.termCode == null || data.termCode == "") {
            toastr["error"]("Please full-up the required data!", "Error!");
            return;
        }
        
        if (s.casualTag == 1) {
            if (data.hasClothing >= 0 && data.hasMidYear >= 0 && data.hasYearEnd >= 0 && data.hazardCode >= 0) {

            }
            else {
                toastr["error"]("Please full-up the required data!", "Opps...");
                return;
            }
        }
       
        s.bDisable = true;
        s.saveDisable = true;

        var tmpData = {};
        tmpData.appointmentItemCode = s.data.appointmentItemCode;
        tmpData.termCode = data.termCode;
        tmpData.remarks = data.remarks;
        tmpData.hasBonusMY = data.hasBonusMY;
        tmpData.hasBonusYE = data.hasBonusYE;
        tmpData.hasClothing = data.hasClothing;
        tmpData.hazardCode = data.hazardCode;
        tmpData.adjstmntFrom = formatMyDate(data.periodFrom);
        tmpData.adjstmntTo = formatMyDate(data.adjstmntTo);

        h.post('~/../../RSPFundSource/SaveAdjstmntRequest', { data: tmpData }).then(function (r) {
            if (r.data.status == "success") {
                s.fundSourceData = r.data.fundSourceData;
                angular.element('#modalAdjstmnt').modal('hide');
                toastr["success"]("Saving sucessful", "Success");
            }
        });
    }

    s.printByFundSource = function () {
        h.post('~/../../RSPReport/ReportByFundSource', { id: s.fundSourceData.fundSourceCode }).then(function (r) {
            if (r.data.status == "success") {
                window.open("../Reports/HRIS.aspx");
            }
        });
    }


    s.printByFundSourcePS = function () {
        h.post('~/../../RSPReport/ReportByFundSourcePS', { id: s.fundSourceData.fundSourceCode }).then(function (r) {
            if (r.data.status == "success") {
                window.open("../Reports/HRIS.aspx");
            }
        });
    }

    s.showAdjustmentData = function (data) {
        s.data = data;       
        angular.element('#modalAdjstmntData').modal('show');
    }
    //SHOW EMPLOYEE PS
    s.adminTag = 0;   
    s.showEmployeeData = function (data) {

        s.data = data;
        s.adminTag = 0;
        s.casualTag = 0;
        if (data.employmentStatusCode == "05") {
            s.casualTag = 1;
        }
        s.updateAlert = 0;
        h.post('~/../../RSPFundSource/GetAppointmentItemPS', { appItemCode: s.data.appointmentItemCode }).then(function (r) {
            if (r.data.status == "success") {              
                s.empPS = r.data.ps;
                s.empPS.recNo = 0;
                s.data.periodFrom = new Date(moment(s.empPS.periodFrom).format('YYYY,MM,DD'));
                s.data.periodTo = new Date(moment(s.empPS.periodTo).format('YYYY,MM,DD'));
                s.adminTag = r.data.adminTag;
                angular.element('#modalEmployeeData').modal('show');
            }
        });
        s.fData = 0;
        s.forceUpdate = false;
    }

    //admin RECALCULATE
    s.adminCalQ = function (data) {

        s.data = data;
        
        if (s.casualTag == 1) {
            if (s.data.hasClothing >= 0 && s.data.hasMidYear >= 0 && s.data.hasYearEnd >= 0 && s.data.hazardCode >= 0) {

            }
            else {
                toastr["error"]("Please full-up the required data!", "Opps...");
                return;
            }
        }
       
        s.bDisable = true;
        s.saveDisable = true;      
        s.data.appointmentItemCode = s.data.appointmentItemCode;
      
        s.data.periodFrom = new Date(moment(s.data.periodFrom).format('YYYY,MM,DD'));
        s.data.periodTo = new Date(moment(s.data.periodTo).format('YYYY,MM,DD'));
        s.updateAlert = 0;
        h.post('~/../../RSPFundSource/ReCalculatePS', { id: s.data.appointmentItemCode, postData: s.data }).then(function (r) {
            if (r.data.status == "success") {
                s.empPS = r.data.ps;
                s.saveDisable = false;
                s.bDisable = false;
                s.updateAlert = r.data.updateAlert;
                if (s.updateAlert == 0) {
                    toastr["success"]("Good...", "Success");
                }
            }
            else {
                toastr["error"]("Something went wrong...!", "Opps...");
            }
        });
    }


    //QUICK PS UPDATE (ADMIN ONLY)

    s.QuickFix = function (data) {
        s.data = data;
        if (s.casualTag == 1) {
            if (s.data.hasClothing >= 0 && s.data.hasMidYear >= 0 && s.data.hasYearEnd >= 0 && s.data.hazardCode >= 0) {

            }
            else {
                toastr["error"]("Please full-up the required data!", "Opps...");
                return;
            }
        }

        s.bDisable = true;
        s.saveDisable = true;
        s.data.appointmentItemCode = s.data.appointmentItemCode;

        s.data.periodFrom = new Date(moment(s.data.periodFrom).format('YYYY,MM,DD'));
        s.data.periodTo = new Date(moment(s.data.periodTo).format('YYYY,MM,DD'));
      
        h.post('~/../../RSPFundSource/QuickFixPS', { id: s.data.appointmentItemCode, postData: s.data }).then(function (r) {
            if (r.data.status == "success") {               
                s.bDisable = false;
                s.updateAlert = 0;
                toastr["success"]("PS updating successful!", "Success");
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


    s.generatePSData = function (data) {
        s.bDisable = true;
       
        var tempData = {};
        tempData.periodFrom = data.periodFrom;
        tempData.periodTo = data.periodTo;
        tempData.fundSourceCode = s.fundSourceData.fundSourceCode;
        tempData.employmentStatusCode = data.employmentStatusCode;
         
        h.post('~/../../AppointmentNonPlantilla/PSGenerator', { data: tempData }).then(function (r) {
            if (r.data.status == "success") {
                s.bDisable = false;
                window.open("../Reports/HRIS.aspx");
                //toastr["success"]("PS updating successful!", "Success");
            }
            else {
                toastr["error"]("Something went wrong...!", "Opps...");
            }
        });


    }



    //# force update
    //forceUpdate
    s.forceToUpdate = function (amt) {
        s.bDisable = true;
        h.post('~/../../RSPFundSource/ForceFixPS', { code: s.data.appointmentItemCode, amt: amt }).then(function (r) {
            if (r.data.status == "success") {
                s.bDisable = false;
                s.updateAlert = 0;
                s.fundSourceData = r.data.fundSourceData;
                toastr["success"]("PS updating successful!", "Success");
                angular.element('#modalEmployeeData').modal('hide');
            }
            else {
                toastr["error"]("Something went wrong...!", "Opps...");
                s.bDisable = false;
            }
        });
    }


}]);