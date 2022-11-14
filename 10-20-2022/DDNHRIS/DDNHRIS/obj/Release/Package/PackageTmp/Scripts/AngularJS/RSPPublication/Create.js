app.controller('PublicationCreate', ['$scope', '$http', '$document', function (s, h, d) {


    s.msg = "Welcome";
    s.data = {};
    s.vacantList = {};
    s.pubItemList = {};
    s.bDisable = false;

    loadInitData();
    
    function loadInitData() {
        h.post('~/../../RSPPublication/PlantillaVacant').then(function (r) {
            if (r.data.status == "success") {
                s.vacantList = r.data.vacantList;
                s.data.publicationDate = r.data.pubDate;
            }
        });
    }


    s.addToList = function (id) {
        if (id == null || id == "") {
            return;
        }
        s.bDisable = true;
        h.post('~/../../RSPPublication/AddPublicationItem', { id: id }).then(function (r) {
            if (r.data.status == "success") {
                s.data.plantillaCode = "";
                s.pubItemList = r.data.list;
                //angular.element('#modalAddProgram').modal('hide');
            }
            s.bDisable = false;
        });
    }

    s.submitPublication = function (data) {      
        h.post('~/../../RSPPublication/SubmitPublication', { data: data }).then(function (r) {
                if (r.data.status == "success") {
                   
                }
            });
    }
        
   
}]);