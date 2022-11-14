app.controller('EmployeePostingApp', ['$scope', '$http', '$document', function (s, h, d) {
    s.msg = "Sample";
    s.office = "PGO";
    s.deptList = {};
    s.positionList = {};
    s.structureList = [];


    s.plantilla = {};

  

    loadPlantillaData();

    

    function loadPlantillaData() {
      
        h.post('~/../../Plantilla/ShowCurrentPlantilla', { id: "1011" }).then(function (r) {
            if (r.data.status == "success") {
                s.plantilla = r.data.plantilla;
            }
        }, function (result) {
            console.log(result.stat);
        });
    }
    

    s.postEmployee = function (data) {
       
        h.post('~/../../RSPUtility/SelectPostingPosition', { id: data.plantillaCode }).then(function (r) {
            if (r.data.status == "success") {
                s.data = {};
                s.plantilla = r.data.plantilla;
                s.employeeList = r.data.employeeList;
                s.empStatList = r.data.empStatList;
                s.showOnSave = true;
                s.showOnUpdate = false;
                angular.element('#modalEmployeePosting').modal('show');
            }
        });
    }


    s.editEmployeePostedData = function (id) {
        h.post('~/../../RSPUtility/EditPostedData/' + id).then(function (r) {
            if (r.data.status == "success") {
                s.plantilla = r.data.plantilla;
                s.employeeList = r.data.employeeList;
                s.empStatList = r.data.empStatList;

                s.data = r.data.myData;

                s.showOnSave = false;
                s.showOnUpdate = true;

                angular.element('#modalEmployeePosting').modal('show');
            }
        });
    }





    s.changeOffice = function (id) {
        h.post('~/../../RSPUtility/ChangeDeptForPosting/' + id).then(function (r) {
            if (r.data.status == "success") {
                s.structureList = r.data.parentList;
                s.office = r.data.deptName;
                angular.element('#modalDeptSelect').modal('hide');
            }
        }, function (result) {
            console.log(result.stat);
        });
    }

    //Save PlantillaPosition
    s.saveEmployeePosting = function () {
        s.data.plantillaCode = s.plantilla.plantillaCode;
        var dats = s.data;
        h.post('~/../../RSPUtility/SaveEmployeePosting', { data: dats }).then(function (r) {
            if (r.data.status == "success") {
                //alert(r.data.parentList.length);
                //s.structureList = r.data.parentList;
                reaload(r.data.code);
                s.data = {};
                angular.element('#modalEmployeePosting').modal('hide');
            }
            else {

            }
        }, function (result) {
            console.log(result.stat);
        });
    }


    //UPDATE
    s.UpdateEmployeePostedData = function () {
        s.data.plantillaCode = s.plantilla.plantillaCode;
        var dats = s.data;
        h.post('~/../../RSPUtility/UpdateEmployeePostedData', { data: dats }).then(function (r) {
            if (r.data.status == "success") {
                reaload(r.data.code);
                s.data = {};
                angular.element('#modalEmployeePosting').modal('hide');
            }
            else {

            }
        });
    }

    function reaload(id) {
        h.post('~/../../RSPUtility/ChangeDeptForPosting/' + id).then(function (r) {
            if (r.data.status == "success") {
                s.structureList = r.data.parentList;
                s.office = r.data.deptName;
                angular.element('#modalDeptSelect').modal('hide');
            }
        }, function (result) {
            console.log(result.stat);
        });
    }


    s.checkStepInc = function () {
        h.post('~/../../RSPUtility/StepNextYear').then(function (r) {
            if (r.data.status == "success") {

            }
            else {

            }
        }, function (result) {
            console.log(result.stat);
        });
    }


}]);
 