app.filter("unique", function () {
    return function (collection, keyname) {
        var output = [],
            keys = [];

        angular.forEach(collection, function (item) {
            var key = item[keyname];
            if (keys.indexOf(key) === -1) {
                keys.push(key);
                output.push(item);
            }
        });
        return output;
    };
});
app.controller("MFOStandard", function ($scope, $http, filterFilter) {
    var s = $scope;

    s.currentOffice = "OFFPHRMONZ3WT7D";
    s.HRAccnt = "OFFPHRMONZ3WT7D";
    loadData();

    //PBO = OFFPBOEZ7SC4ZA9
    //COA = OFFCOAYW2FQ3EM1
    //COMELEC = OFFCOMELECXV2XW
    /*   ====================================================================================== INITIALIZE DATA ======================================================================================*/
    s.cmfoes = {};
    s.TempCmfo = {};
    s.targetCreated = {};
    s.isHR;

    function loadData() {
        $http.post('../MFOStandard/CMFO_get', { _OfficeID: s.HRAccnt }).then(function (response) {

            s.cmfoes = response.data.cmfo;
            console.log("cmfo", s.cmfoes);
            /*   ====================================================================================== HIDE/SHOW FUNCTION FOR HR ONLY ======================================================================================*/
            if (s.currentOffice == s.HRAccnt) {
                s.isHR = true;
            }
            else {
                s.isHR = false;
            }

        }), function (error) {

        }

    };




    /*   ====================================================================================== GET MFOID FROM DROPDOWN AND MODAL CONFIRMATION ============================================================*/
    s.getMFOperId = function (data) {
        $http.post('../MFOStandard/CMFO_getMFOperId', { MFOId: data }).then(function (response) {
            s.objCMFO = response.data.objCMFO;
            // alert(JSON.stringify(s.objCMFO));
            //s.showBckBtn = true;
            $('#addSIForm').modal('hide');
            $('#addMoreSIForm').modal('show');
        }), function (error) {

        }


    };

    /*   ====================================================================================== ADD NEW COMMON MFO ======================================================================================*/
    s.newCMFO = {};
    s.catCMFO = {};
    s.newSI = {};
    s.addStandard = [{}, {}, {}, {}, {}];
    s.objCMFO = {};

    s.addCMFO = function (_newCMFO) {
        document.getElementById("addCMFO").disabled = true;
        if (_newCMFO.MFO != null) {
            $http.post('../MFOStandard/CMFO_add', { newCMFO: _newCMFO, newStandard: s.addStandard, _OfficeID: s.HRAccnt }).then(function (response) {
                console.log("objCMFO", response.data.objCMFO);
                if (response.data.data == 1) {
                    s.newCMFO = {}; s.catCMFO = {}; s.newSI = {}; s.r5 = null; s.r4 = null; s.r3 = null; s.r2 = null; s.r1 = null; s.addStandard = [{}, {}, {}, {}, {}];
                    s.objCMFO = response.data.objCMFO;
                    s.cmfoes = [];
                    loadData();
                    $('#addMFOForm').modal('hide');
                    document.getElementById("addCMFO").disabled = false;
                    Swal.fire({
                        title: 'Add more success indicator?',
                        text: "This is to add success indicator based on common MFO you just created.",
                        icon: 'success',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        cancelButtonText: 'No, maybe later.',
                        confirmButtonText: 'Yes, please!',
                        allowOutsideClick: false
                    }).then((result) => {
                        if (result.isConfirmed) {
                            $('#addMoreSIForm').modal('show');
                            s.showBckBtn = false;

                        }
                    });
                }
                if (response.data.data == 2) {
                    alert("Error! MFO is already exist.");
                    document.getElementById("addCMFO").disabled = false;
                }
            }), function (error) {

            }
        }
        else {
            alert("Error! MFO is empty.");
            document.getElementById("addCMFO").disabled = false;
        }

    }
    /*   ====================================================================================== ADD MORE SUCCESS INDICATOR ======================================================================================*/

    s.moreSI = function (_newSI) {
        /* alert(JSON.stringify(s.objCMFO[0])); get the first array object*/


        if (_newSI.indicator !== undefined) {
            $http.post('../MFOStandard/CMFO_moreSI', { getMFO: s.objCMFO[0], newSI: _newSI, newStandard: s.addStandard, _OfficeID: s.HRAccnt }).then(function (response) {
                if (response.data.data == 1) {
                    s.newCMFO = {}; s.catCMFO = {}; s.newSI = {}; s.r5 = null; s.r4 = null; s.r3 = null; s.r2 = null; s.r1 = null; s.addStandard = [{}, {}, {}, {}, {}];
                    s.objCMFO = response.data.objCMFO;
                    console.log(" s.objCMFO ", s.objCMFO)
                    s.cmfoes = [];
                    loadData();
                    $('#addMoreSIForm').modal('hide');


                    Swal.fire({
                        title: 'Add more success indicator?',
                        text: "This is to add success indicator based on common MFO you just created.",
                        icon: 'success',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        cancelButtonText: 'No, maybe later.',
                        confirmButtonText: 'Yes, please!',
                        allowOutsideClick: false
                    }).then((result) => {
                        if (result.isConfirmed) {
                            $('#addMoreSIForm').modal('show');


                        }
                    });
                }
            }), function (error) {
            }
        }
        else {
            alert("Error! Success Indicator field is empty.")
        }



    }

    /*   ====================================================================================== SHOW STANDARD ===================================================================================================*/
    s.standardShow = {};
    s.standard_show = {};
    s.showStandard = function (getStandard, isVisible) {
        console.log("getStandard", getStandard);
        s.copyBtn = isVisible;

        $http.post('../MFOStandard/CMFO_Standard', { mfoindId: getStandard }).then(function (response) {
            s.standard_show = response.data;
            //s.standardShow = response.data;
            console.log(JSON.stringify(s.standardShow));

        }), function (error) {

        }
    }

    /*   ====================================================================================== UPDATE STANDARD ===================================================================================================*/

    s.updateStandard = function () {
        $http.post('../MFOStandard/CMFO_UpdateStandard', { objStandard: s.standard_show }).then(function (respponse) {
            if (respponse.data.data == 1) {
                const Toast = Swal.mixin({
                    toast: true,
                    position: 'top-end',
                    showConfirmButton: false,
                    timer: 3000,
                    iconColor: '#FFFFFF',
                    color: '#FFFFFF',
                    timerProgressBar: true,
                    background: '#87EE04',
                    didOpen: (toast) => {
                        toast.addEventListener('mouseenter', Swal.stopTimer)
                        toast.addEventListener('mouseleave', Swal.resumeTimer)
                    }
                })

                Toast.fire({
                    icon: 'success',
                    title: 'Updated Successfully!'
                })
            }
        }), function (error) {

        }
    }
    /*   ====================================================================================== COPY STANDARD ===================================================================================================*/
    s.copyQT = function () {
        // alert(JSON.stringify(s.standard_show[2].quantity));
        $('#addPerformance').modal('hide');
        const Toast = Swal.mixin({
            toast: true,
            position: 'top-end',
            showConfirmButton: false,
            timer: 3000,
            iconColor: '#FFFFFF',
            color: '#FFFFFF',
            timerProgressBar: true,
            background: '#0063D9',
            didOpen: (toast) => {
                toast.addEventListener('mouseenter', Swal.stopTimer)
                toast.addEventListener('mouseleave', Swal.resumeTimer)
            }
        })

        Toast.fire({
            icon: 'success',
            title: 'Copied!'
        })
        /* if (s.standard_show[2].quantity == 0) {
             s.onChangeTarget(null);
             s.newSI.target = null;
         }
         else {
             s.onChangeTarget(s.standard_show[2].quantity);
             s.newSI.target = s.standard_show[2].quantity;
         }*/

        s.addStandard = s.standard_show;
    }
    /*   ====================================================================================== SHOW ALL MFO AND ITS SI ===================================================================================================*/
    s.getAllSIbyMFO = function (_mfoData) {
        console.log("_mfoData", _mfoData)
        $http.post('../MFOStandard/CMFO_getMFO_allSI', { mfoData: _mfoData }).then(function (response) {
            s.updateMFO_allSI = response.data.data;
            s.standard_show = response.data.standard;
            //alert(JSON.stringify(s.standard_show[2].quantity));


            console.log(s.updateMFO_allSI);
        }, function (err) {
            alert("error item");
        });
    }
    /*   ====================================================================================== UPDATE MFO, SI, Category ===================================================================================================*/
    s.updateMFO_allSIbyID = function (_mfoData) {
        console.log("_mfoData_for_save", _mfoData);
        $http.post('../MFOStandard/CMFO_updateMFO_allSI', { MFOData: _mfoData, objStandard: s.standard_show }).then(function (response) {
            if (response.data.status == 1) {

                const Toast = Swal.mixin({
                    toast: true,
                    position: 'top-end',
                    showConfirmButton: false,
                    timer: 3000,
                    iconColor: '#FFFFFF',
                    color: '#FFFFFF',
                    timerProgressBar: true,
                    background: '#87EE04',
                    didOpen: (toast) => {
                        toast.addEventListener('mouseenter', Swal.stopTimer)
                        toast.addEventListener('mouseleave', Swal.resumeTimer)
                    }
                })

                Toast.fire({
                    icon: 'success',
                    title: 'Updated Successfully!'
                })
                s.cmfoes = [];
                loadData();
            }
        }, function (err) {
            alert("error item");
        });
    }


    s.dropdownClrCat = function () {
        s.category = "";
        //alert(s.category)
    }
    s.onChangeFilterTarget = function (data) {
        if (data == 1) {
            s.targetFilter = '!!';
        }
        if (data == 2) {
            s.targetFilter = null;
        }
    }
    s.dropdownClrStat = function () {
        s.filterTarget = undefined;
    }
    s.filterShow = false;
    s.filterVisbility = function () {
        if (s.filterShow == false) {
            s.filterShow = true;
        }
        else {
            s.filterShow = false;
        }

    }
    s.reportShow = false;
    s.reportsVisbility = function () {
        if (s.reportShow == false) {
            s.reportShow = true;
        }
        else {
            s.reportShow = false;
        }

    }

    s.rowCount = function (_mfoID, data) {
        s.counts = 0;
        for (var ind = 0; ind < data.length; ind++) {
            if (_mfoID == data[ind].MFOId) {
                s.counts++;
            }
        }
        //s.counts = s.counts + (s.counts * 5);
        return s.counts;
    }
})