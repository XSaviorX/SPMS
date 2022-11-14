

app.controller('DTRPreparation', ['$scope', '$http', '$document', function (s, h, d) {

    s.data = {};
    s.logList = {};
    s.employeeList = {};
    s.monthList = [{ value: 'November 2021', month: 'November 2021' }];
    s.logTypeList = [{ value: 1, logType: 'Login' }, { value: 2, logType: 'Logout' }];
    s.approvingOfficerList = {};
    s.schedList = {};
    s.justy = {};

    loadInitData();

    function loadInitData() {
        h.post('~/../../DailyTimeRecord/PreparationData').then(function (r) {
            if (r.data.status == "success") {
                s.employeeList = r.data.employeeList;
                s.data.EIC = r.data.employeeList[0].Value;
                s.approvingOfficerList = r.data.approvingOfficerList;
            }
        });
    }
        
    s.viewDTRLog = function (data) {       
        s.logList = {};
        h.post('~/../../DailyTimeRecord/ViewShiftLog', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                s.logList = r.data.logList;
            }
        });
    }
    
    s.modalJusty = function (dt) {       
        dt = formatDate(dt);
        h.post('~/../../DailyTimeRecord/Justy', { jDate: dt }).then(function (r) {
            if (r.data.status == "success") {
                s.schedList = r.data.schedList; 
                if (r.data.schedList.length == 1) {
                    s.justy.scheme = r.data.schedList[0].Value;                   
                }                 
                angular.element('#openModalJusty').modal('show');
            }
        });
    }

    s.submitJusty = function (data) {

 
        var tempDT = formatDate(data.date) + " " + formatTime(data.time);
 
        data.logDT = tempDT;
        data.schemeDT = data.scheme;

        h.post('~/../../DailyTimeRecord/SubmitJusty', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                if (r.data.schedList.length == 1) {

                }
                angular.element('#openModalJusty').modal('show');
            }
        });
    }



    function formatDate(date) {
        if (!date) {
            return '';
        }
        return (moment(date).format('MMMM DD, YYYY'));
    };


    function formatTime(time) {
        if (!time) {
            return '';
        }
        return (moment(time).format('HH:mm'));
    };


}]);