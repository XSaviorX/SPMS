app.controller('PlantillaCurrent', ['$scope', '$http', '$document', function (s, h, d) {

    s.deptFunctionCode = "";
    s.functionList = {};
    s.plantilla = {};

    s.position = {};
    s.qStan = {};
    s.jobDesc = {};

    s.isLoading = false;

    s.bDisable = true;


    s.isReadOnly = true;
    s.jdEditMode = false;
    s.jobDescItem = {};

    loadInitData();

    function loadInitData() {
        h.post('~/../../Plantilla/ProposedInitData').then(function (r) {
            if (r.data.status == "success") {
                s.functionList = r.data.functionList;
            }
        });
    }

    s.functionChange = function () {
        if (s.deptFunctionCode == "" || s.deptFunctionCode == null) {
            s.bDisable = true;
        }
        else {
            showPlantilla();
            s.bDisable = false;
        }

    }

    //VIEW FUNDED
    s.viewPlantilla = function () {
        var id = s.deptFunctionCode;
        s.isLoading = true;
        s.plantilla = {};
        s.bDisable = true;
        s.isReadOnly = true;
        h.post('~/../../Plantilla/ShowCurrentPlantilla/' + id).then(function (r) {
            if (r.data.status == "success") {
                s.isLoading = false;
                s.bDisable = false;
                s.plantilla = r.data.plantilla;
            }
        });
    }

    function showPlantilla() {
        var id = s.deptFunctionCode;
        s.isLoading = true;
        s.plantilla = {};
        s.bDisable = true;
        h.post('~/../../Plantilla/ShowCurrentPlantilla/' + id).then(function (r) {
            if (r.data.status == "success") {
                s.isLoading = false;
                s.bDisable = false;
                s.plantilla = r.data.plantilla;
            }
        });
    }

    //SHOW VACANT
    s.showVacantPosition = function () {
        var id = s.deptFunctionCode;
        s.isLoading = true;
        s.plantilla = {};
        s.bDisable = true;
        h.post('~/../../Plantilla/ShowCurrentPlantillaVacant/' + id).then(function (r) {
            if (r.data.status == "success") {
                s.isLoading = false;
                s.bDisable = false;
                s.plantilla = r.data.plantilla;
            }
        });
    }


    s.fundStat = 1;
    //SHOW UNFUNDED POSITION
    s.showUnfundedPosition = function () {
        var id = s.deptFunctionCode;
        s.isLoading = true;
        s.plantilla = {};
        s.bDisable = true;
        h.post('~/../../Plantilla/ShowUnfundedPositions/' + id).then(function (r) {
            if (r.data.status == "success") {
                s.isLoading = false;
                s.bDisable = false;
                s.plantilla = r.data.plantilla;
                s.fundStat = 0;
            }
        });
    }


    s.formatDate = function (date) {
        if (!date) {
            return '';
        }
        return (moment(date).format('MM/DD/YYYY'));
    };


    s.setEditMode = function (id) {
        if (id == 1) {
            s.isReadOnly = false;
        }
        else {
            s.isReadOnly = true;
        }
    }

    s.viewQualification = function (item) {
        s.position = item;
        var id = item.plantillaCode;
        s.isReadOnly = true;
        h.post('~/../../Plantilla/PositionQS/' + id).then(function (r) {
            if (r.data.status == "success") {
                s.qStan = r.data.qs;
                angular.element('#modalQualification').modal('show');

            }
        });
    }

    s.updateQStandard = function (data) {
        h.post('~/../../Plantilla/UpdateQStandard', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                toastr["success"]("Updating successful!", "Success");
                s.isReadOnly = true;
            }
        });
    }

    s.viewJobDesc = function (item) {
        s.position = item;
        s.transTag = "";
        s.jdEditMode = false;
        var id = item.plantillaCode;
        h.post('~/../../Plantilla/ViewJobDesc/' + id).then(function (r) {
            if (r.data.status == "success") {
                s.jobDesc = r.data.jobDesc;
                angular.element('#modalJobDescription').modal('show');
            }
        });
    }

    s.JDEdit = function (jd) {
        s.transTag = "Update";
        s.jdEditMode = true;
        s.jobDescItem = jd;
        s.isReadOnly = false;
    }

    s.transTag = "";

    s.CreateJobDesc = function () {
        s.jobDescItem = {};
        s.transTag = "Save";
        s.jdEditMode = true;
        s.isReadOnly = false;
    }


    s.cancelJDEdit = function () {
        s.transTag = "";
        s.jdEditMode = false;
    }


    s.SaveUpdateJobDesc = function (data) {
        data.plantillaCode = s.position.plantillaCode;
        if (s.transTag == "Save") {
            h.post('~/../../Plantilla/SaveJobDesc', { data: data }).then(function (r) {
                if (r.data.status == "success") {
                    s.jdEditMode = false;
                    s.isReadOnly = true;
                    s.jobDesc = r.data.list;
                    toastr["success"]("Saving successful!", "Success");
                }
                else {
                    toastr["error"]("Unable to save!", "Opps...");
                }
            });
        }
        else if (s.transTag == "Update") {
            h.post('~/../../Plantilla/UpdateJobDesc', { data: data }).then(function (r) {
                if (r.data.status == "success") {
                    s.jdEditMode = false;
                    s.isReadOnly = true;
                    s.jobDesc = r.data.list;
                    toastr["success"]("Updating successful!", "Success");
                }
                else {
                    toastr["error"]("Unable to save!", "Opps...");
                }
            });
        }
    }



    s.viewPrint = function () {
        var id = s.deptFunctionCode;
        h.post('~/../../Plantilla/Current/' + id).then(function (r) {
            if (r.data.status == "success") {
                window.open("../Reports/HRIS.aspx")
                s.bDisable = false;
            }
            else {
                s.bDisable = true;
            }
        });
    }

    s.printVacantPositions = function () {
        var id = s.deptFunctionCode;
        h.post('~/../../Plantilla/Current', { id: id, printType: "VACANT" }).then(function (r) {
            if (r.data.status == "success") {
                window.open("../Reports/HRIS.aspx")
                s.bDisable = false;
            }
            else {
                s.bDisable = true;
            }
        });
    }

    //PRINT FUNDED
    s.printCSCForm = function (rep) {
        var id = s.deptFunctionCode;

        if (s.fundStat == 0) {
            h.post('~/../../Plantilla/PlantillaCSCForm/', { id: id, code: rep }).then(function (r) {
                if (r.data.status == "success") {
                    window.open("../Reports/HRIS.aspx")
                    s.bDisable = false;
                }
                else {
                    s.bDisable = true;
                }
            });
        }
        else {
            h.post('~/../../Plantilla/UnfundedPlantillaCSCForm/', { id: id, code: rep }).then(function (r) {
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

    
     

    //PRINT FUNDED
    //s.printDBMForm = function () {
    //    var id = s.deptFunctionCode;
    //    h.post('~/../../Plantilla/PlantillaDBMForm/' + id).then(function (r) {
    //        if (r.data.status == "success") {
    //            window.open("../Reports/HRIS.aspx")
    //            s.bDisable = false;
    //        }
    //        else {
    //            s.bDisable = true;
    //        }
    //    });
    //}


    //VIEW UNFUNDED
    s.viewUnFundedPlantilla = function () {
        var id = s.deptFunctionCode;
        s.isLoading = true;
        s.plantilla = {};
        s.bDisable = true;
        h.post('~/../../Plantilla/ShowUnfundedPlantilla/' + id).then(function (r) {
            if (r.data.status == "success") {

                s.isLoading = false;
                s.bDisable = false;
                s.plantilla = r.data.plantilla;
            }
        });
    }

    //PRINT FUNDED
    s.printCSCUnFundedForm = function (code) {
        var id = s.deptFunctionCode;
        h.post('~/../../Plantilla/PrintPlantillaUnfundSetup', { id: id, code: code }).then(function (r) {
            if (r.data.status == "success") {
                window.open("../Reports/HRIS.aspx")
                s.bDisable = false;
            }
            else {
                s.bDisable = true;
            }
        });
    }

    //PRINT FUNDED
    s.printVacantFunc = function () {
        var id = s.deptFunctionCode;
        h.post('~/../../Plantilla/PrintPlantillaVacant/' + id).then(function (r) {
            if (r.data.status == "success") {
                window.open("../Reports/HRIS.aspx")
                s.bDisable = false;
            }
            else {
                s.bDisable = true;
            }
        });
    }


    s.positionData = {};
    s.newItemNo = "";
    //FUNDING POSITION
    s.fundPosition = function (data) {
        s.bDisable = true;
        h.post('~/../../Plantilla/GetLastItemNo').then(function (r) {
            if (r.data.status == "success") {
                s.newItemNo = r.data.newItemNo;
                s.checkStat = false;
                s.positionData = data;
                angular.element('#modalFundingPosition').modal('show');
                s.bDisable = false;
            }
            else {
                s.bDisable = false;
            }
        });         
    }

    s.fundPosNextYear = function (data) {
        h.post('~/../../Plantilla/FundPositionNextYear', { data: data }).then(function (r) {
            data.fundStat = 2;
        });
    }

    s.submitPositionForFunding = function () {
        h.post('~/../../Plantilla/FundPosition', { data: s.positionData }).then(function (r) {
            if (r.data.status == "success") {
                angular.element('#modalFundingPosition').modal('hide');
                showPlantilla();
                toastr["success"]("Updating successful!", "Success");
            }
            else {
                s.bDisable = false;
            }
        });
    }



    //PRINT UNFUNDED DBM
    s.printUnFundedForm = function (code) {
        var id = s.deptFunctionCode;
        h.post('~/../../Plantilla/PrintPlantillaUnfundSetup', { id: id, code: code }).then(function (r) {
            if (r.data.status == "success") {
                window.open("../Reports/HRIS.aspx")
                s.bDisable = false;
            }
            else {
                s.bDisable = true;
            }
        });
    }



}]);