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

app.filter('currentdate', ['$filter', function ($filter) {
    return function () {
        return $filter('date')(new Date(), 'MMM d, y');
    };
}]);


app.controller("MFOStandard", function ($scope, $http, filterFilter) {
    var s = $scope;
    s.mfoes = {};
    /*  s.cmfoes = {};*/
    s.classifications = {};
    s.offices = {};
    s.nIndicators = [];
    s.listofmfoes = {};
    s.newMFO = {};
    s.newIndicator = [];
    s.editIndicator = [{}];
    s.newCMFO = {};
    s.newCIndicator = [{}];
    s.indicators = {};
    s.listofOffices = {};
    s.programs = {};
    s.listOfPrograms = {};
    s.projects = {};
    s.listOfProjects = {};
    s.AppropriateProjID = {};
    s.AssignedAppropProjID = {};
    s.OPCRTableData = {};
    s.geteditMFOSI = {};
    s.updateMFOSI = {};
    s.updateMFO_allSI = [{}];
    s.catMFO = {};
    s.updatecatMFO = {};
    s.index = 0;
    s.performanceData = {};
    s.disRequest = {};
    s.requestContent = {};
    s.reqcontent_standard = {};

    var indicatorCount = 1;
    var newIndicatorCount = 1;
    s.currentOffice = '';
    loadData();
    function loadData(data) {
        console.log("offid : ", data);
        //============================================================
        // Get list of MFO and SI Function
        // s.currentOffice = "OFFPHRMONZ3WT7D";
        s.HRAccnt = "OFFPHRMONZ3WT7D";
        //PBO = OFFPBOEZ7SC4ZA9
        //COA = OFFCOAYW2FQ3EM1
        //COMELEC = OFFCOMELECXV2XW
        $http.post('../MFOStandard/MFO_get', { Office_ID: data }).then(function (response) {
            console.log("MFO DATA: ", response.data)
            s.mfoes = response.data;
        }, function (err) {
            console.log(JSON.stringify(err));
        });
        //=============================================================
        // Get List of Offices Function
        $http.post('../MFOStandard/MFO_getoffices').then(function (response) {
            //console.log(response.data)
            s.offices = response.data;

        }, function (err) {
            //alert("error item (offices)");
        });
        //=============================================================
        // Get List of Divisions Function
        $http.post('../MFOStandard/MFO_getDivisions', { _offId: data }).then(function (response) {
            console.log("divisions: ", response.data);
            s.divisions = response.data;

        }, function (err) {
            //alert("error item (offices)");
        });

        $http.post('../MFOStandard/MFO_getClassifications', { _offId: data}).then(function (response) {
            console.log("dasdsads: ", response.data)
            s.classifications = response.data;

        }, function (err) {
            //alert("error item (dasdsas)");
        });

    }
    s.officeName = function () {
        return s.mfoes[0].officeName;
    }

    //----------------- SET OFFICE -------------------

    s.setOffice = function (data) {
        //  alert(data);
        s.currentOffice = data;
        s.mfoes = {};
        loadData(data);
    }
    //============================= ADD CLASSIFICATION ===================================

    s.classification = "";
    s.addClassification = function (data) {
        console.log("s.classification:", data);
        $http.post('../MFOStandard/addClassification', { Classification: data, _OfficeID: s.currentOffice }).then(function (response) {

            if (response.data.data == 1) {
                s.classifications = {};

                $http.post('../MFOStandard/MFO_getClassifications', { _offId: response.data._OfficeID }).then(function (response) {
                    console.log("dasdsads: ", response.data)
                    s.classifications = response.data;

                }, function (err) {
                    //alert("error item (dasdsas)");
                });
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
                    title: 'Saved Successfully!'
                })
            }
            else {
                const Toast = Swal.mixin({
                    toast: true,
                    position: 'top-end',
                    showConfirmButton: false,
                    timer: 3000,
                    iconColor: '#FFFFFF',
                    color: '#FFFFFF',
                    timerProgressBar: true,
                    background: 'red',
                    didOpen: (toast) => {
                        toast.addEventListener('mouseenter', Swal.stopTimer)
                        toast.addEventListener('mouseleave', Swal.resumeTimer)
                    }
                })

                Toast.fire({
                    icon: 'error',
                    title: 'Error! Data already exist!'
                })
            }

        }), function (err) {
            alert(err);
        }



    }

    // ----------------------------ADD DIVSION----------------------------------------------
    s.divname = "";
    s.divisionAdd = function (data) {
        console.log("DIVISION ADD: ", data);
        $http.post('../MFOStandard/addDivision', { DivName: data, Office_ID: s.currentOffice }).then(function (response) {

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
                    title: 'Saved Successfully!'
                })
            }

            if (response.data.status == 0) {

                const Toast = Swal.mixin({
                    toast: true,
                    position: 'top-end',
                    showConfirmButton: false,
                    timer: 3000,
                    iconColor: '#FFFFFF',
                    color: '#FFFFFF',
                    timerProgressBar: true,
                    background: 'red',
                    didOpen: (toast) => {
                        toast.addEventListener('mouseenter', Swal.stopTimer)
                        toast.addEventListener('mouseleave', Swal.resumeTimer)
                    }
                })

                Toast.fire({
                    icon: 'danger',
                    title: 'Error! Division Already Exist.'
                })
            }




        }), function (err) {
            alert(err);
        }


    }
    //============================= UPDATE CLASSIFICATION ===================================

    s.classification = "";
    s.updateClassification = function (data) {
        console.log("update classification:", data);
        $http.post('../MFOStandard/updateClassification', { ClassData: data, _OfficeID: s.currentOffice }).then(function (response) {

            if (response.data.status == 1) {
                s.classifications = {};

                $http.post('../MFOStandard/MFO_getClassifications').then(function (response) {
                    console.log("dasdsads: ", response.data)
                    s.classifications = response.data;

                }, function (err) {
                    //alert("error item (dasdsas)");
                });
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


        }), function (err) {
            alert(err);
        }



    }

    //===================================================================================

    s.indicators = {};
    s.loadIDS = function () {
        $http.post('../MFOStandard/addMFO_setInd').then(function (response) {
            s.newIndicator.push(response.data);
        }), function (err) {
            alert("error item (getProjects)");
        }

    }

    s.loadAddnIndicators = function () {
        newIndicatorCount = 1;
        s.nIndicators = [];
        $http.post('../MFOStandard/addMFO_setInd').then(function (response) {
            s.nIndicators.push(response.data);
        }), function (err) {
            alert("error item (getProjects)");
        }
    }

    s.getCMFO = function () {
        $http.get('../MFOStandard/CMFO_get').then(function (response) {
            //console.log(response.data)
            s.cmfoes = response.data;

        }, function (err) {
            alert("error item");
        });
    }
    //============================================================
    // Get list of MFO and SI Function per office
    s.getOPCRTableData = function (OfficeID) {
        $http.post('../MFOStandard/MFO_getPerOffice', { _OfficeID: OfficeID }).then(function (response) {
            console.log(response.data)
            s.OPCRTableData = response.data;

        }, function (err) {
            alert("error item");
        });

        // s.getPerformancePer();
    }
    //============================================================
    // Get MFO and SI by MFO ID
    s.getMFO_SIbyID = function (data) {
        s.updateMFOSI = data;
    }

    //============================================================
    // Update MFO and SI by MFO ID/SI_ID
    s.updateMFO_SIbyID = function (data) {
        //$http.post('../OPCR/MFO_updateMFOSI', { _OPCRData: data }).then(function (response) {
        //    if (response.data.status == "success") {
        //        alert(response.data.status);
        //        s.getOPCRTableData('OFFPHRMONZ3WT7D');
        //    }
        //}, function (err) {
        //    alert("error item");
        //});
        //alert((data.MFOId + "\n" + data.appropProjectId))
        console.log(JSON.stringify(data));
    }

    //===================================================================
    // MFO Insert Function
    s.btnSaveTapOnce = false;
    s.insertMFO = function (_MFODesc, _catId) {

        if (s.btnSaveTapOnce) {
            alert("REMINDER: BUTTON SAVE HAVE ALREADY BEEN CLICKED." + "\n" + " PLEASE REOPEN THE FORM");
        }
        else {
            console.log("MFO DATA: ", _MFODesc);
            console.log("CAT DATA: ", _catId);
            _catId.OfficeID = "OFFPHRMONZ3WT7D";
            s.btnSaveTapOnce = true;

            $http.post('../MFOStandard/MFO_insert', { _MFO: _MFODesc, MFOCat: _catId }).then(function (response) {

                //alert(response.data.status);
                s.insertMFOInd(response.data.status, _catId); //return mfoid
                //s.insertMFOInd("dummy test");
            }), function (err) {
                alert(err.toString());
            }
        }
    }

    s.setbtnSaveTapOnce = function () {
        s.btnSaveTapOnce = false;

    }
    //===================================================================
    // MFO SIs Insert Function
    var targetID = "";
    s.addPerformance = [];
    function getTargetID(tgtID, _MFO_id, _noOfIndicator) {
        var temp = 0.0;
        var _quan = 0.0;
        var target = 0.0;

        //alert(tgtID + "\n" + _MFO_id + "\n" + _noOfIndicator);
        for (var i = 0; i < 5; i++) {
            var rating = s.newIndicator[_noOfIndicator][i].rating;
            var target = document.getElementById(s.newIndicator[_noOfIndicator][i].indicatorId + "quantity3").value;
            if (rating == "5") {
                temp = Number(target) + Number((target * 0.3));
            }
            else if (rating == "4") {
                temp = Number(target) + Number((target * 0.15));
            }
            else if (rating == "3") {
                temp = target;
            }
            else if (rating == "2") {
                temp = (target / 2) + 1;
            }
            else if (rating == "1") {
                temp = (target / 2);
            }

            s.newIndicator[_noOfIndicator][i].targetId = tgtID;
            s.newIndicator[_noOfIndicator][i].quantity = parseInt(temp);
            s.newIndicator[_noOfIndicator][i].target = document.getElementById(s.newIndicator[_noOfIndicator][i].indicatorId + "quantity3").value;

            // alert(JSON.stringify(s.newIndicator[_noOfIndicator][i]));

            s.addPerformance.push(s.newIndicator[_noOfIndicator][i]);

        }
        $http.post('../MFOStandard/addPerformances', { indicators: s.addPerformance, MFO_ID: _MFO_id }).then(function (response) {
            testResponse = response.data;
            if (testResponse == "0") {
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
                    title: 'Saved Successfully!'
                })
                loadData();
            }
            else {
                const Toast = Swal.mixin({
                    toast: true,
                    position: 'top-end',
                    showConfirmButton: false,
                    timer: 3000,
                    iconColor: '#EE7504',
                    color: '#6A6A6A',
                    timerProgressBar: true,
                    background: '#ffffff',
                    didOpen: (toast) => {
                        toast.addEventListener('mouseenter', Swal.stopTimer)
                        toast.addEventListener('mouseleave', Swal.resumeTimer)
                    }
                })

                Toast.fire({
                    icon: 'warning',
                    title: 'Error! There is an empty fields.'
                })
            }
        }), function (err) {
            alert("error item");
        }
        s.resetAddMFO();
    }

    s.insertMFOInd = function (_MFO_id, _officeID) {
        //s.newIndicator.forEach(prnt);
        var testResponse = "test";

        for (var _noOfIndicator = 0; _noOfIndicator < indicatorCount; _noOfIndicator++) {
            console.log("ind:  ", s.newIndicator[_noOfIndicator][0]);
            //s.newIndicator[_noOfIndicator][0].target = document.getElementById(s.newIndicator[_noOfIndicator][0].indicatorId + "quantity3").value;

            $http.post('../MFOStandard/MFO_SIinsert', { indicators: s.newIndicator[_noOfIndicator][0], MFO_ID: _MFO_id, officeId: _officeID }).then(function (response) {
                var tgtID = response.data;
                //alert(targetID);
                _noOfIndicator--;
                getTargetID(tgtID, _MFO_id, _noOfIndicator);
            }), function (err) {
                alert("error item");
            }
        }


    }
    //===================================================================

    //===================================================================
    // Get List of Programs Function
    //s.getPrograms = function (_data) {
    //    $http.post('../MFOStandard/MFO_getPrograms', { _OfficeID: _data }).then(function (response) {
    //        console.log(response.data)
    //        s.programs = response.data;
    //    }), function (err) {
    //        alert("error item (getPrograms)");
    //    }
    //}
    //===================================================================
    //s.getProjects = function (_data) {
    //    //alert(_data);
    //    $http.post('../MFOStandard/MFO_getProjects', { _ProgramID: _data }).then(function (response) {
    //        console.log(response.data)
    //        s.projects = response.data;
    //    }), function (err) {
    //        alert("error item (getProjects)");
    //    }
    //}
    //==================================================================

    s.addIndicatorField = function () {
        indicatorCount++;
        $http.post('../MFOStandard/addMFO_setInd').then(function (response) {

            s.newIndicator.push(response.data);
            console.log("IDs: ", s.newIndicator)

        }), function (err) {
            alert("error item (getProjects)");
        }
    }
    s.removeIndicatorField = function () {
        indicatorCount--;
        s.newIndicator.pop();
        console.log("IDs: ", s.newIndicator)
    }

    s.addIndicatorField_update = function () {
        newIndicatorCount++;
        $http.post('../MFOStandard/addMFO_setInd').then(function (response) {

            s.nIndicators.push(response.data);
            console.log("ind: ", s.newindicators)
        }), function (err) {
            alert("error item (getProjects)");
        }
    }

    s.insertCMFO = function (_CMFODesc) {
        $http.post('../MFOStandard/CMFO_insert', { _CMFO: _CMFODesc }).then(function (response) {

            //alert(response.data.status);
            loadData();
            s.insertCMFOInd(response.data.status);
        }), function (err) {
            alert("error item");
        }
    }
    //===================================================================
    // MFO SIs Insert Function
    s.insertCMFOInd = function (_CMFO_id) {
        console.log(s.newIndicator);
        $http.post('../MFOStandard/CMFO_SIinsert', { indicators: s.newIndicator, CMFO_ID: _CMFO_id }).then(function (response) {

            alert(response.data.status);
            //loadData();

        }), function (err) {
            alert("error item");
        }
    }
    //============================================================
    // Update MFO and all SI by MFO ID/SI_ID
    s.updateMFO_allSIbyID = function (pdata, mfodata) {

        //var _mfodesc = $('#updateAll_MFODesc').val();

        //console.log("PDATA: ", pdata);
        //console.log("MFODATA: ", mfodata);
        for (var _NewnoOfIndicator = 0; _NewnoOfIndicator < newIndicatorCount; _NewnoOfIndicator++) {
            console.log("ind:  ", pdata[_NewnoOfIndicator], mfodata.MFOId, mfodata.officeId);
            s.newdata = pdata[_NewnoOfIndicator];
            $http.post('../MFOStandard/MFO_updateMFO_allSI', { _Pdata: s.newdata, MFOdata: mfodata }).then(function (response) {
                if (response.data.status != '') {
                    /*alert("MFO and SI data have been updated successfully");*/
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
                        title: _NewnoOfIndicator + ' Success Indicator/s Added Successfully!'
                    })
                    loadData();
                }
            }, function (err) {
                alert("error item");
            });
        }
    }
    //$http.post('../MFOStandard/MFO_SIinsert', { _Pdata: pdata, MFOdata: mfodata }).then(function (response) {
    //alert(response.data.status);
    //if (response.data.status != '') {
    //    /*alert("MFO and SI data have been updated successfully");*/
    //    const Toast = Swal.mixin({
    //        toast: true,
    //        position: 'top-end',
    //        showConfirmButton: false,
    //        timer: 3000,
    //        iconColor: '#FFFFFF',
    //        color: '#FFFFFF',
    //        timerProgressBar: true,
    //        background: '#87EE04',
    //        didOpen: (toast) => {
    //            toast.addEventListener('mouseenter', Swal.stopTimer)
    //            toast.addEventListener('mouseleave', Swal.resumeTimer)
    //        }
    //    })

    //    Toast.fire({
    //        icon: 'success',
    //        title: 'Updated Successfully!'
    //    })
    //    loadData();
    //}
    //}, function (err) {
    //    alert("error item");
    //});

    s.origData = {};
    s.p_origData = {};
    s.dateToday = new Date();
    s.sendRequest = function (mfoData, indicatorData, standardData, todayDate) {

        console.log("req MFO SI: ", indicatorData);

        $http.post('../MFOStandard/send_requestChanges', { mfoData: mfoData, reqChangeData: indicatorData, reqDataStandard: standardData, _DateToday: todayDate }).then(function (response) {
            if (response.data.status != '') {
                /*  *//*alert("MFO and SI data have been updated successfully");*/
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
                loadData(s.currentOffice);
            }
        }, function (err) {
            console.log(JSON.stringify(err));
        });
    }

    var req_mfo = 0;
    var req_year = 0;
    var req_sem = 0;
    var req_class = 0;
    function checkChanges(data, _todayDate) {
        $http.post('../MFOStandard/GetPerformance', { MFO_ID: data[0].MFOId }).then(function (response) {

            s.performanceData = response.data;
            sendRequestData(data, s.performanceData, _todayDate);
        }, function (err) {
            alert("error item");
        });
    }


    s.origDatasforsave = {};
    function sendRequestData(data, req_pdata, _Date) {
        s.origDatasforsave = data;
        console.log("Original Data: ", req_pdata);
        console.log("Request: ", s.editIndicator);
        var ind = 0;
        for (var _cnt = 0; _cnt < data.length; _cnt++) {
            s.updateMFO_allSI[_cnt].MFO = "";
            s.updateMFO_allSI[_cnt].categoryId = "";
            for (var _ind = 0; _ind < data.length; _ind++) {
                if (data[_cnt].MFOId == s.updateMFO_allSI[_ind].MFOId && data[_cnt].indicatorId == s.updateMFO_allSI[_ind].indicatorId) {
                    //checking for changes
                    //mfo
                    if (data[_cnt].MFO != s.update_allSI.MFO && req_mfo == 0) {
                        req_mfo = 1;
                        //alert("MFO Update" + "\n" + "FROM: " + data[_cnt].MFO + "\n TO: " + s.update_allSI.MFO);
                        s.updateMFO_allSI[_ind].MFO = s.update_allSI.MFO;
                        s.updateMFO_allSI[_ind].o_mfo = data[_cnt].MFO;

                    }
                    if (data[0].categoryId != s.update_allSI.categoryId) {
                        s.updateMFO_allSI[0].categoryId = s.update_allSI.categoryId;
                        s.updateMFO_allSI[_ind].o_category = data[_cnt].categoryId;
                    } else {
                        s.updateMFO_allSI[_ind].categoryId = "";
                    }
                    //MFO year
                    if (data[_cnt].year != s.update_allSI.year && req_year == 0) {
                        req_year = 1;
                        //alert("MFO Update" + "\n" + "FROM: " + data[_cnt].year + "\n TO: " + s.update_allSI.year);
                        s.updateMFO_allSI[_ind].year = s.update_allSI.year;
                        s.updateMFO_allSI[_ind].o_year = data[_cnt].year;
                    } else {
                        s.updateMFO_allSI[_ind].year = "";
                    }
                    //MFO Semester
                    if (data[_cnt].semester != s.update_allSI.semester && req_sem == 0) {
                        req_sem = 1;
                        //alert("MFO Update" + "\n" + "FROM: " + data[_cnt].MFO + "\n TO: " + s.update_allSI.MFO);
                        s.updateMFO_allSI[_ind].semester = s.update_allSI.semester;
                        s.updateMFO_allSI[_ind].o_semester = data[_cnt].semester;
                    } else {
                        s.updateMFO_allSI[_ind].semester = "";
                    }
                    //MFO Classification
                    if (data[_cnt].classificationId != s.update_allSI.classificationId && req_class == 0) {
                        req_class = 1;
                        //alert("MFO Update" + "\n" + "FROM: " + data[_cnt].MFO + "\n TO: " + s.update_allSI.MFO);
                        s.updateMFO_allSI[_ind].classificationId = s.update_allSI.classificationId;
                        s.updateMFO_allSI[_ind].o_classificationId = data[_cnt].classificationId;
                    } else {
                        s.updateMFO_allSI[_ind].classificationId = "";
                    }
                    //target
                    if (data[_cnt].target != s.updateMFO_allSI[_ind].target) {

                        s.updateMFO_allSI[_ind].o_target = data[_cnt].target;
                        //alert("MFO Update" + "\n" + "FROM: " + data[_cnt].target + "\n TO: " + s.updateMFO_allSI[_ind].target);
                    } else {
                        s.updateMFO_allSI[_ind].target = "";
                    }
                    //indicator
                    if (data[_cnt].indicator != s.updateMFO_allSI[_ind].indicator) {
                        s.updateMFO_allSI[_ind].o_indicator = data[_cnt].indicator;
                        // alert("MFO Update" + "\n" + "FROM: " + data[_cnt].indicator + "\n TO: " + s.updateMFO_allSI[_ind].indicator);
                    } else {
                        s.updateMFO_allSI[_ind].indicator = "";
                    }

                    var ratingcnt = 5;
                    if (ind < s.editIndicator.length) {
                        while (ratingcnt > 0) {
                            // alert(data[_ind].MFOId + " - " + s.editIndicator[ind].MFOId + "\n" + data[_ind].indicatorId + " - " + s.editIndicator[ind].indicatorId)
                            if (data[_ind].MFOId == s.editIndicator[ind].MFOId && data[_ind].indicatorId == s.editIndicator[ind].indicatorId && req_pdata[ind].rating == ratingcnt) {
                                //console.log("Before: ", s.editIndicator[ind].rating, " - ",s.editIndicator[ind].quality, ", ", s.editIndicator[ind].timeliness);
                                s.editIndicator[ind].quantity = "";

                                if (req_pdata[ind].quality == s.editIndicator[ind].quality && req_pdata[ind].timeliness == s.editIndicator[ind].timeliness) {
                                    s.editIndicator[ind].rating = "";
                                }
                                if (req_pdata[ind].quality != s.editIndicator[ind].quality) {

                                    s.editIndicator[ind].o_quality = req_pdata[ind].quality;

                                } else {
                                    s.editIndicator[ind].quality = "";
                                }
                                if (req_pdata[ind].timeliness != s.editIndicator[ind].timeliness) {

                                    s.editIndicator[ind].o_timeliness = req_pdata[ind].timeliness;

                                } else {
                                    s.editIndicator[ind].timeliness = "";
                                }
                                //  console.log("After: ", s.editIndicator[ind].rating," - ",s.editIndicator[ind].quality, ", ", s.editIndicator[ind].timeliness);

                                ratingcnt--;
                            }
                            ind++;
                            //   console.log(ratingcnt, " - " , ind);
                        }
                    }
                }
            }
        }
        console.log("here: ", s.updateMFO_allSI);
        console.log("orig: ", data);
        console.log("edit: ", s.editIndicator);
        $http.post('../MFOStandard/send_requestChanges', { reqChangeData: s.updateMFO_allSI, _DateToday: _Date }).then(function (response) {
            if (response.data.status != '') {
                /*  *//*alert("MFO and SI data have been updated successfully");*/
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
                loadData();
            }
            req_mfo = 0;
            req_year = 0;
            req_sem = 0;
            req_class = 0;
        }, function (err) {
            console.log(JSON.stringify(err));
        });
    }

    //===========================================================
    s.year = [];
    s.semester = [];
    s.update_allSI = {};
    s.getAllSIbyMFO = function (data) {
        //alert(JSON.stringify(data));
        s.update_allSI = data;
        //alert(data.MFOId);
        $http.post('../MFOStandard/MFO_getMFO_allSI', { _OPCRData: data.MFOId }).then(function (response) {
            s.updateMFO_allSI = response.data;
            console.log("here: ", s.updateMFO_allSI);
            //alert(JSON.stringify(s.updateMFO_allSI[0].MFO));

            if (s.updateMFO_allSI[0].categoryId == 1) {
                document.getElementById("category1").checked = true;
            }
            else {
                document.getElementById("category1").checked = false;

            }
            if (s.updateMFO_allSI[0].categoryId == 2) {
                document.getElementById("category2").checked = true;
            }
            else {
                document.getElementById("category2").checked = false;


            }
            if (s.updateMFO_allSI[0].categoryId == 3) {
                document.getElementById("category3").checked = true;
            }
            else {
                document.getElementById("category3").checked = false;


            }
            s.semester = response.data[0].semester;
            s.year = [];
            var curYear = 0;
            if (response.data[0].year != null && response.data[0].year != 0) {
                curYear = response.data[0].year;
            } else {
                curYear = 2022;
            }
            for (var i = 3; i > 0; i--) {
                var temp = Number(curYear) - i;
                s.year.push({ val: temp, selected: false });
            }
            s.year.push({ val: curYear, selected: true });
            for (var i = 1; i < 5; i++) {
                var temp = Number(curYear) + i;
                s.year.push({ val: temp, selected: false });
            }
            //alert(JSON.stringify(s.year));
            //s.f_editIndicators(update_allSI.MFOId);
            //console.log(s.updateMFO_allSI);
        }, function (err) {
            alert("error item");
        });
    }

    s.checkSem = function (sem) {
        console.log("dads");
        if (sem == 1) {
            alert('First');
            $('#opt_selectFirstSem').selected = "selected";
        } else if (sem == 2) {
            alert('Second');
            $('#opt_selectSecndSem').selected = "selected";
        }
    }


    //===================================================================
    s.autoComputeStandard = function (targetCount, _id, _unit) {
        //alert(targetCount);
        console.log("this is unit: ", _unit, " - ", targetCount);
        if (_unit == "%") {
            document.getElementById(_id + 'quantity5').readOnly = false;
            document.getElementById(_id + 'quantity4').readOnly = false;
            document.getElementById(_id + 'quantity3').readOnly = false;
            document.getElementById(_id + 'quantity2').readOnly = false;
            document.getElementById(_id + 'quantity1').readOnly = false;
        } else {
            if (targetCount != '' && targetCount != null) {
                if (targetCount == 1) {
                    document.getElementById(_id + 'quantity5').value = 1;
                    document.getElementById(_id + 'quantity4').value = "";
                    document.getElementById(_id + 'quantity3').value = "";
                    document.getElementById(_id + 'quantity2').value = "";
                    document.getElementById(_id + 'quantity1').value = 0;
                } else {
                    document.getElementById(_id + 'quantity5').value = parseInt(Number(targetCount * 0.3) + Number(targetCount));
                    document.getElementById(_id + 'quantity4').value = parseInt(Number(targetCount) + Number((targetCount * 0.15)));
                    document.getElementById(_id + 'quantity3').value = targetCount;
                    document.getElementById(_id + 'quantity2').value = parseInt(1 + (targetCount / 2));
                    document.getElementById(_id + 'quantity1').value = parseInt(targetCount / 2);
                }
            } else {
                document.getElementById(_id + 'quantity5').value = "";
                document.getElementById(_id + 'quantity4').value = "";
                document.getElementById(_id + 'quantity3').value = "";
                document.getElementById(_id + 'quantity2').value = "";
                document.getElementById(_id + 'quantity1').value = "";
            }
        }

    }

    s.trySave = function (data) {

        data.forEach(prnt);

        //alert(JSON.stringify(data))
        //console.log(data);
    }
    var temp = "";
    function prnt(s_data) {

        if (s_data.target != null) {
            temp = s_data.target;
        } else {
            s_data.target = temp;
        }

        //console.log(s_data);
    }

    s.add_Performance = function (data) {

        $http.post('../OPCR/AddPerformance', { tOpcrPerformance: s.addPerformance }).then(function (response) {

        }), function (error) {
            alert("Error");
        }

    }
    //======================= EDIT MODAL ======================================

    s.f_editIndicators = function (mfoId) {
        //alert('d:' + mfoId);
        $http.post('../MFOStandard/GetPerformance', { MFO_ID: mfoId }).then(function (response) {
            console.log(response.data)
            s.editIndicator = response.data;
        }), function (err) {
            alert("error item (getProjects)");
        }
    }



    //========================== REQUEST CHANGES =============================


    s.display_requests = function () {
        $http.post('../MFOStandard/Get_AllRequest').then(function (response) {
            //console.log("dasdsa:" ,response.data)
            s.disRequest = response.data;
            console.log("dasd: ", s.disRequest);
        }), function (err) {
            alert("error item (Get_AllRequest)");
        }
    }

    s.getStatus = function (statusNumber, rqId) {

        var _status = "";
        if (statusNumber == 0) {
            _status = "PENDING";
            document.getElementById("r" + rqId).style.color = "#FF7733";
        } else if (statusNumber == 1) {
            _status = "APPROVED";
            document.getElementById("r" + rqId).style.color = "#27A95E";
        } else {
            _status = "CANCELLED";
            document.getElementById("r" + rqId).style.color = "#CC4400";
        }
        return _status;
    }

    s.tryAll = function (data) {
        alert(data);
    }

    s.getRequestContent = function (reqId) {
        //alert(s.disRequest[0].MFO);
        $http.post('../MFOStandard/Get_RequestContent', { reqId: reqId }).then(function (response) {
            //console.log("dasdsa:" ,response.data)
            s.requestContent = response.data;
            modalHeaderBGColor(s.requestContent[0].Status);
        }), function (err) {
            alert("error item (Get_RequestContent)");
        }
    }
    s.cntt = 0;
    s.alert = function () {
        s.cntt++;
        alert(s.cntt);
    }
    s.checkNull = function (checkData, returnData) {
        //alert(checkData  +" - " + returnData);
        if (checkData == null || checkData == 0) {
            return "";
        } else {
            return returnData;
        }
    }
    s.checkDescription = function (checkData) {
        //alert(checkData  +" - " + returnData);
        var returnDesc = "";
        if (checkData == null) {
            return "";
        } else {
            if (checkData == "1") {
                returnDesc = "Strategic Function";
            }
            else if (checkData == "2") {
                returnDesc = "Core Function";
            }
            else if (checkData == "3") {
                returnDesc = "Support Function";
            }
            return returnDesc;
        }
    }

    s.checkSem = function (checkData) {
        //alert(checkData  +" - " + returnData);
        var returnDesc = "";
        if (checkData == null) {
            return "";
        } else {
            if (checkData == 1) {
                returnDesc = "First Semester";
            }
            else if (checkData == 2) {
                returnDesc = "Second Semester";
            }
            return returnDesc;
        }
    }

    function modalHeaderBGColor(status) {
        //btn btn - primary
        if (status == 0) {//pending
            document.getElementById("modalheader").style.backgroundColor = "#FF7733";
            $('#btn_approve').removeClass('hidden');
            $('#btn_reject').removeClass('hidden');
        } else if (status == 1) {//approved
            document.getElementById("modalheader").style.backgroundColor = "#27A95E";
            $('#btn_approve').addClass('hidden');
            $('#btn_reject').addClass('hidden');
        } else {//cancelled / rejected
            document.getElementById("modalheader").style.backgroundColor = "#CC4400";
            $('#btn_approve').addClass('hidden');
            $('#btn_reject').addClass('hidden');
        }
    }


    s.updateRequest = function (reqidd, statusVal, _reqData, _reqDataStandard) {
        console.log("req_content: ", _reqData);
        console.log("req standard: ", _reqDataStandard);
        $http.post('../MFOStandard/Update_statusRequest', { reqId: reqidd, statusChange: statusVal, reqData: _reqData, reqDataStandard: _reqDataStandard }).then(function (response) {
            //console.log("dasdsa:" ,response.data)
            if (response.data.data == "successfully") {
                /*  *//*alert("MFO and SI data have been updated successfully");*/
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
                display_requests();
            }
        }), function (err) {
            alert("error item (Get_RequestContent)");
        }
    }

    s.rejectRequest = function (reqidd) {
        $http.post('../MFOStandard/Reject_statusRequest', { reqID: reqidd }).then(function (response) {
            //console.log("dasdsa:" ,response.data)
            if (response.data.data != "") {
                /*  *//*alert("MFO and SI data have been updated successfully");*/
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
                display_requests();
            }
        }), function (err) {
            alert("error item (Get_RequestContent)");
        }
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
    s.dropdownClrCat = function () {
        s.category = "";
        //alert(s.category)
    }

    s.rowColor = function (data) {
        if ((data % 2 == 1)) {
            document.getElementById("row_indicator").style.backgroundColor = "#EFF1F1";
        } else {
            document.getElementById("row_indicator").style.backgroundColor = "white";
        }
    }

    s.resetAddMFO = function () {
        s.newMFO = {};
        s.newIndicator = [];
        s.loadIDS();
        loadData();
    }
    //========================================================================
})