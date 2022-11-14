app.controller('RAI', ['$scope', '$http', '$document', function (s, h, d) {

    s.appointmentList = {};
    s.raiList = {};
    s.bDisable = false;

    loadInitData();

    function loadInitData() {
        h.post('~/../../RSPReport/AppointmentIssuedList').then(function (r) {
            if (r.data.status == "success") {
                s.appntList = r.data.appointmentList;
            }
        });
    }


    s.generateRAI = function () {
        h.post('~/../../RSPReport/GenerateRAI').then(function (r) {
            if (r.data.status == "success") {
                s.appntList = r.data.appntList;
            }
        });
    }

    s.formatDate = function (date) {
        if (!date) {
            return '';
        }
        return (moment(date).format('MM/DD/YYYY'));
    };


    s.showRAIList = function () {
        h.post('~/../../RSPReport/RAIBatchList').then(function (r) {
            if (r.data.status == "success") {
                s.raiList = r.data.appointmentList;
            }
        });
    }


    s.printRAI = function (id) {    
        h.post('~/../../RSPReport/PrintRAI', {id: id}).then(function (r) {
            if (r.data.status == "success") {
                window.open("../Reports/HRIS.aspx");
            }
        });
    }
    

}]);