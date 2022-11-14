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


app.controller('WorkGroup', ['$scope', '$http', '$document', function (s, h, d) {
    
    s.empData = {};
    s.monthList = {};
    s.workGroupList = {};
    s.schemeList = {};
    s.logList = {};
    s.tabId = 0;
    s.loadingState = false;
    s.myID = "";
    s.logDayFull = "";
    s.schemeCode = {};
    s.monthSched = {};

    s.schemes = {};

    //load data
    loadInitData();

    function loadInitData() {        
        h.post('~/../../Attendance/MyWorkGroupList').then(function (r) {
            if (r.data.status == "success") {                
                s.workGroupList = r.data.list;
                s.monthList = r.data.monthList;
                s.schemeList = r.data.schemeList;
                s.schemes = r.data.schemes;
            }
        });
    }
      
    //SELECT EMPLOYEE
    s.selectGroupMate = function (data) {
        //check type of log by employee
       
        s.monthSched = {};
        s.empData = data;
        s.tabId = 1;
        if (data.shiftTag >= 1) {
            s.tabId = 2;
        }
       
    }

    //SHOW SCHME
    s.viewAttendanceScheme = function () {         
        s.loadingState = true;
        s.code = s.monthCode;     
        h.post('~/../../Attendance/ViewSchedule', { id: s.empData.EIC, code: s.code }).then(function (r) {
            if (r.data.status == "success") {             
                s.monthSched = r.data.list;  
            }
        });
        s.loadingState = false;
    }
 

    //BACK TO LIST
    s.backToWorkGroupList = function () {
        s.monthCode = null;
        s.tabId = 0;
    }

    //MODAL : SELECT SCHEME
    s.modalViewMyLogs = function (logNo) {      
        s.loadingState = true;       
        h.post('~/../../Attendance/GetSchemeAndLogs', { id: s.empData.EIC, code: logNo }).then(function (r) {
            if (r.data.status == "success") {
                s.logList = r.data.logList;
                s.logDayFull = r.data.logDay;
                s.schemeCode = r.data.schemeCode;
                angular.element('#modalDailyLogs').modal('show');
            }
        });
        s.loadingState = false;
    }

    s.shiftTemplateId = "NONE";
    s.modalViewMyScheme = function (logNo) {
       
        s.loadingState = true;
        h.post('~/../../Attendance/GetMySchemeOfTheDay', { id: s.empData.EIC, code: logNo }).then(function (r) {
            if (r.data.status == "success") {
              
                s.logDayFull = r.data.logDay;
                s.shiftTemplateId = r.data.shiftTemplateId;
               
                angular.element('#modalSelectScheme').modal('show');
            }
        });
        s.loadingState = false;
    
    }

    s.updateDailyScheme = function () {
        s.loadingState = true;
        h.post('~/../../Attendance/UpdateSchemeOfTheDay', { id: s.empData.EIC, month: s.logDayFull, code: s.shiftTemplateId }).then(function (r) {
            if (r.data.status == "success") {
                //s.monthSched = r.data.monthSched;
                angular.element('#modalSelectScheme').modal('hide');
                toastr["success"]("Saving successful!", "Success");
            }
        });
        s.loadingState = false;
    }
    
    s.submitScheme = function () {      
        s.loadingState = true;
        h.post('~/../../Attendance/SubmitSchemeSchedule', { id: s.empData.EIC, code: s.logDayFull, tag: s.schemeCode }).then(function (r) {
            if (r.data.status == "success") {
                s.monthSched = r.data.monthSched;
                toastr["success"]("Saving successful!", "Success");
            }
        });
        s.loadingState = false;
    }

    //MOVE NEXT
    s.moveNext = function ()
    {
        s.loadingState = true;
        h.post('~/../../Attendance/MoveToNextDay', { id: s.empData.EIC, code: s.logDayFull  }).then(function (r) {
            if (r.data.status == "success") {
                s.logDayFull = r.data.logDay;
                s.logList = r.data.logList;
                s.schemeCode = r.data.schemeCode;                
            }
        });
        s.loadingState = false;
    }

    //moveBack
    s.moveBack = function () {
        s.loadingState = true;
        h.post('~/../../Attendance/MoveToPreviousDay', { id: s.empData.EIC, code: s.logDayFull }).then(function (r) {
            if (r.data.status == "success") {
                s.logDayFull = r.data.logDay;
                s.logList = r.data.logList;
                s.schemeCode = r.data.schemeCode;
            }
        });
        s.loadingState = false;
    }

    s.generateLog = function (code) {
        s.bDisable = true;       
        h.post('~/../../Attendance/GenerateDailyLog', { id: s.empData.EIC, code: code }).then(function (r) {
            if (r.data.status == "success") {               
                s.monthSched = r.data.monthSched;
                toastr["success"]("Updating successful!", "Success");
            }
            else {
                toastr["error"](r.data.status, "Opps...");
            }
        });
        s.bDisable = false;
    }

    s.formatDate = function (date) {
        if (!date) {
            return '';
        }
        return (moment(date).format('MMMM DD, YYYY'));
    };

    s.formatDateFull = function (date) {
        if (!date) {
            return '';
        }
        return (moment(date).format('MMMM DD, YYYY h:mm:ss a'));
    };


   

}]);

 