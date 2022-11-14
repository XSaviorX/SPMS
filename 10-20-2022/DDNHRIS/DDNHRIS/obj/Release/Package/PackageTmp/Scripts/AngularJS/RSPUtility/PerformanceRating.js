app.controller('PerformanceRating', ['$scope', '$http', '$document', function (s, h, d) {

    s.data = {};
    s.employeeList = {};
    s.bDisable = false;

    s.ratingList = {};

    //LOAD DATA 
    loadInitData();

    function loadInitData() {
        h.post('~/../../RSPUtility/GetEmployeeList').then(function (r) {
            if (r.data.status == "success") {
                s.employeeList = r.data.list;
                s.ratingList = r.data.ratingList;
            }
        });
    }

   
    s.modalPerformanceRating = function () {
        angular.element('#ModalPRating').modal('show');
    }


    s.saveRating = function (data) {
        s.bDisable = true;
        h.post('~/../../RSPUtility/SavePerformanceRating', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                s.data = {};
                s.data.EIC = "";
                s.ratingList = r.data.ratingList;
                toastr["success"]("Saving successful!", "Success");
                s.bDisable = false;
            }
        });
    }
   
  

}]);