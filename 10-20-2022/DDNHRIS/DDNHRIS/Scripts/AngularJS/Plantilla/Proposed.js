app.controller('PlantillaProposed', ['$scope', '$http', '$document', function (s, h, d) {


    s.deptFunctionCode = "";
    s.functionList = {};
    s.bDisable = true;
    loadInitData();

    function loadInitData() {
        h.post('~/../../Plantilla/ProposedInitData').then(function (r) {
            if (r.data.status == "success") {
                s.functionList = r.data.functionList;
            }
        });
    }

    s.onSelectOffice = function () {
        if (s.deptFunctionCode != undefined && s.deptFunctionCode.length) {
            s.bDisable = false;
        }
    }


    s.printPreview = function () {        
        if (s.deptFunctionCode != undefined && s.deptFunctionCode.length) {
            s.bDisable = true;
            var id = s.deptFunctionCode;
            h.post('~/../../Plantilla/PrintPlantillaSetup', {id : id, type : "PROPOSED" }).then(function (r) {
                if (r.data.status == "success") {
                    window.open("../Reports/HRIS.aspx")
                    s.bDisable = false;
                }
            });
        }
    }
             

    s.printLPBForm = function () {
        if (s.deptFunctionCode != undefined && s.deptFunctionCode.length) {
            s.bDisable = true;
            var id = s.deptFunctionCode;
            h.post('~/../../Plantilla/PrintPlantillaSetup', { id: id, type: "LBPFORM3" }).then(function (r) {
                if (r.data.status == "success") {
                    window.open("../Reports/HRIS.aspx")
                    s.bDisable = false;
                }
            });
        }
    }


    s.printLPBForm3A = function () {
        if (s.deptFunctionCode != undefined && s.deptFunctionCode.length) {
            s.bDisable = true;
            var id = s.deptFunctionCode;
            h.post('~/../../Plantilla/PrintPlantillaSetup', { id: id, type: "LBPFORM3A" }).then(function (r) {
                if (r.data.status == "success") {
                    window.open("../Reports/HRIS.aspx")
                    s.bDisable = false;
                }
            });
        }
    }


    

    s.printPreviewUnfud = function () {
        
        if (s.deptFunctionCode != undefined && s.deptFunctionCode.length) {

            s.bDisable = true;
            var id = s.deptFunctionCode;

            h.post('~/../../Plantilla/PrintPlantillaUnfundSetup/' + id).then(function (r) {
                if (r.data.status == "success") {
                    window.open("../Reports/HRIS.aspx")
                    s.bDisable = false;
                }
                else {
                    s.bDisable = true;
                }
            });
            
        }

    }


}]);