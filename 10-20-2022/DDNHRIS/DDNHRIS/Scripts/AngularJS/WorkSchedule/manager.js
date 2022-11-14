app.controller('WORKSCHEDULE', ['$scope', '$http', '$document', function (s, h, d) {


    s.schedule = {};
    s.schedules = {};
    s.workGroups = {};
    s.months = {};
    s.schemes = {};

    s.employee = {};

    loadInitData();
    s.tabId = 1;

    s.period = [{ text: 'Full month', value: 0 }, { text: '1st Period', value: 1 }, { text: '2nd Period', value: 2 }];

    function loadInitData() {
        h.post('~/../../WorkSchedule/ManagerInitData').then(function (r) {
            if (r.data.status == "success") {
                s.months = r.data.monthList;
                s.schedules = r.data.schedules;
                s.schemes = r.data.schemes;
                s.workGroups = r.data.workGroups;
            }
        });

    }

     
    s.createSchedule = function () {
        angular.element('#modalWorkSchedMonth').modal('show');
    };

    s.submitWorkSched = function (data) {
        h.post('~/../../WorkSchedule/SaveWorksSchedule', {data: data}).then(function (r) {
            if (r.data.status == "success") {
                s.schedule = r.data.schedule;
                angular.element('#modalWorkSchedMonth').modal('hide');
                s.tabId = 2;
            }
        });
    }

    s.viewAttendanceScheme = function () {
        
    }

    //s.scheduleSchemes = {};
    //s.selectSchedule = function (id) {
    //    h.post('~/../../WorkSchedule/SelectWorkSchedule', { id: id }).then(function (r) {
    //        if (r.data.status == "success") {
    //            s.schedule = r.data.schedule;
    //            s.scheduleSchemes = r.data.scheduleSchemes;
    //            s.tabId = 2;
    //        }
    //    });
    //}

    //s.backToList = function () {
    //    s.tabId = 1;
    //}


    //s.schemeDaily = function () {
    //    angular.element('#modalSchemeDaily').modal('show');
    //}

    //s.schemeLogs = {};
    //s.submitSchemeData = function (data) {
    //    data.workSchedId = s.schedule.workSchedId;
    //    h.post('~/../../WorkSchedule/SaveSchemeData', { data: data }).then(function (r) {
    //        if (r.data.status == "success") {
    //           // s.schemes = r.data.schemeLogs;
    //        }
    //    });
    //}

    s.selectGroupMate = function (data) {
        s.employee = data;
        s.tabId = 2;
    }

    s.formatDate = function (date) {
        if (!date) {
            return 'N/A';
        }
        return (moment(date).format('MM/DD/YYYY HH:mm a'));
    };

     
    


}]);