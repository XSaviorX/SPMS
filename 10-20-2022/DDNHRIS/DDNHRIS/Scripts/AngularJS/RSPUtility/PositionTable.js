app.controller('PositionTable', ['$scope', '$http', '$document', function (s, h, d) {

    s.tabNo = 1;
    s.data = {};
    s.positionCSC = {};
    s.positionCOS = {};
    s.positionSub = {};


    s.positionData = {};
    s.keyCSCLevel = "";

    s.bDisable = false;

    //LOAD DATA
    loadInitData();

    function loadInitData() {
        h.post('~/../../RSPUtility/PositionInitData').then(function (r) {
            if (r.data.status == "success") {
                s.positionCSC = r.data.positionCSC;
                s.positionCOS = r.data.positionCOS;              
            }
        });
    }



    s.editCSCPositionData = function (data) {        
        s.positionData = data;
        s.bDisable = true;
        h.post('~/../../RSPUtility/EditCSCPosition', { id: s.positionData.positionCode }).then(function (r) {
            if (r.data.status == "success") {
                s.CSCPositionKey = r.data.positionData.CSCPositionKey;
                angular.element('#modalPositionCSCEdit').modal('show');
                s.bDisable = false;
            }
        });
    }

    s.updateCSCPosition = function (id, key) {
        s.bDisable = true;
        h.post('~/../../RSPUtility/UpdateCSCPosition', { id: id, key: key }).then(function (r) {
            if (r.data.status == "success") {
                s.positionCSC = r.data.positionCSC;
                angular.element('#modalPositionCSCEdit').modal('hide');
                s.bDisable = false;
            }
        });

    }

    s.changeCode = function (i) {
        s.tabNo = i;
    }
     
    s.createPosition = function () {
        if (s.tabNo == 2) {
            angular.element('#modalPositionContract').modal('show');
        }
    }

    s.submitPositionContract = function (data) {
        s.bDisable = true;
        h.post('~/../../RSPUtility/SavePositionContract', {data: data}).then(function (r) {
            if (r.data.status == "success") {
                s.data.positionTitle = "";
                s.data.positionDesc = "";
                s.data.salary = "";
                s.positionCOS = r.data.positionCOS;
                s.bDisable = false;
            }
        });
    }

    

    s.formatDate = function (date) {
        if (!date) {
            return '';
        }
        return (moment(date).format('MMMM DD, YYYY'));
    };


    s.positionProfile = {};
    s.viewPositionProfile = function () {       
        s.bDisable = true;
        h.post('~/../../RSPUtility/ViewPositionProfile').then(function (r) {
            if (r.data.status == "success") {
                s.positionProfile = r.data.list;
            }
        });
    }

}]);