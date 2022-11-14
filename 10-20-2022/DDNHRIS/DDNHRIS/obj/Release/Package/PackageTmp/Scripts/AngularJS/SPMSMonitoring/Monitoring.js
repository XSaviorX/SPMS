app.controller('SPMSMonitoring', ['$scope', '$http', '$document', function (s, h, d) {

    s.groupList = {};
    s.employeeList = {};
    s.bDisable = false;

    
     
    s.workGroupData  = {};
    s.workGroupData.workGroupCode = "WRKGRPPGO";
    s.workGroupData.workGroupName = "PGO";
   

    //LOAD DATA 
    loadInitData();

    function loadInitData() {
        h.post('~/../../SPMSMonitoring/GetWorkGroup').then(function (r) {
            if (r.data.status == "success") {
                s.groupList = r.data.groupList;
                s.ratingList = r.data.ratingList;
            }
        });
    }


    s.modalSelectDepartment = function () {
        angular.element('#modalDepartment').modal('show');
    }

    s.selectGroup = function () {
        s.bDisable = true;
        var id = s.workGroupData.workGroupCode;
        var itemData = s.groupList.filter(function (item) {
            return item.workGroupCode === id;
        })[0];
        s.workGroupData = itemData;

        h.post('~/../../SPMSMonitoring/GetRatingGroupByID', { id: s.workGroupData.workGroupCode }).then(function (r) {
            if (r.data.status == "success") {
                s.ratingList = r.data.ratingList;
                angular.element('#modalDepartment').modal('hide');
                s.bDisable = false;
                toastr["info"](s.workGroupData.workGroupName + " selected!", "Info");
            }
        });

     

    }

   

}]);