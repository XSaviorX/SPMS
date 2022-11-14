app.controller('FILE201', ['$scope', '$http', '$document', function (s, h, d) {

    s.eFileList = {};
    s.loading = true;
    s.folderName = "--";

    loadEmp201File();

    function loadEmp201File() {
        s.loading = true;
        h.post('~/../../Employee/Load201Data').then(function (r) {
            if (r.data.status == "success") {
                s.eFileList = r.data.list;
                s.loading = false;
                s.folderName = "Appointment";
            }
        });
    }


    s.showFiles = function (id) {
        s.loading = true;
        h.post('~/../../Employee/Load201Data', { id: id}).then(function (r) {
            if (r.data.status == "success") {
                s.eFileList = r.data.list;
                s.loading = false;
                s.folderName = id;
            }
        });
    }

    s.modalAddFile = function () {
        angular.element('#modalAddFile').modal('show');
    }

}]);