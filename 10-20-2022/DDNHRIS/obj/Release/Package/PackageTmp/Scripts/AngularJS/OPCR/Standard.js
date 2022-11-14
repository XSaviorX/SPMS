
app.filter('groupBy', function () {
    return function (list, group_by) {

        var filtered = [];
        var prev_item = null;
        var group_changed = false;
        // this is a new field which is added to each item where we append "_CHANGED"
        // to indicate a field change in the list
        var new_field = group_by + '_CHANGED';

        // loop through each item in the list
        angular.forEach(list, function (item) {

            group_changed = false;

            // if not the first item
            if (prev_item !== null) {

                // check if the group by field changed
                if (prev_item[group_by] !== item[group_by]) {
                    group_changed = true;
                }

                // otherwise we have the first item in the list which is new
            } else {
                group_changed = true;
            }

            // if the group changed, then add a new field to the item
            // to indicate this
            if (group_changed) {
                item[new_field] = true;
            } else {
                item[new_field] = false;
            }

            filtered.push(item);
            prev_item = item;

        });

        return filtered;
    };
})
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


//app.factory('Session', ['$rootScope', function ($rootScope) {
//    return {
//        get: function (key) {
//            return sessionStorage.getItem(key);
//        },
//        save: function (key, data) {
//            sessionStorage.setItem(key, data);
//        }
//    };
//}]);


app.controller("OPCR", function ($scope, $http, filterFilter) {

    var s = $scope;
    s.mfoes = {};
    s.offices = {};
    s.listofmfoes = {};
    s.newMFO = {};
    s.newIndicator = [{}];
    s.indicators = {};
    s.listofOffices = {};
    s.programs = {};
    s.listOfPrograms = {};
    s.projects = {};
    s.listOfProjects = {};
    s.AppropriateProjID = {};
    s.AssignedAppropProjID = {};
    s.OPCRTableData = [{}];
    s.OPCRTableDataDisplay = [{}];
    s.geteditMFOSI = {};
    s.updateMFOSI = {};
    s.performanceData = {};
    s.getPerformance = {};
    s.addPerformance = {};
    s.getMfoInd = {}; //Show All MFO and Success Indicator
    s.getMfoIndID = {}; //Get One S
    s.assignStandard = [];
    s.count = 0;
    s.officeNameHeader = "";
    s.assignCmfo = [];
    s.spinnerCheck = 0;
    s.targetStandard = [{}, {}, {}, {}, {}]

    //loadData();
    //loadSession();
    s.print = function () {
        s.officeId = "OFFPHRMONZ3WT7D";

        $http.post('../OPCR/cookies', { officeId: s.officeId }).then(function (responses) {
            window.open('../Reports/SPMS/Opcr.aspx?type=opcr');
        }), function (err) {
            alert("error item (getPerformanceData)");
        }
    }

    s.isAssignOrUnassign = false;
    s.years = {};
    s.getYear = function () {
        $http.post('../OPCR/MFO_getYears').then(function (response) {
            
            s.years = response.data;
            console.log("dasdas: ", s.years);
        }, function (err) {
            //alert("ERROR: loadData");
            console.log("ERROR:", JSON.stringify(err));
        });
    }

    function loadData(data) {
        console.log("daasdsta", data);
        var targetid = "";
        var officeid = "";
        var opcrId = s.currentUser + "-" + s.checkyear + "-" + s.checksem;
        s.currentOffice = 'OFFPHRMONZ3WT7D';
        s.HRAccnt = "OFFPHRMONZ3WT7D";
        //PBO = OFFPBOEZ7SC4ZA9
        //COA = OFFCOAYW2FQ3EM1
        //COMELEC = OFFCOMELECXV2XW

        $http.post('../OPCR/MFO_getPerOffice', { _OfficeID: s.currentOffice, _opcrId: opcrId }).then(function (response) {
            console.log("data: ", response.data)
            //s.OPCRTableData = response.data;
            s.OPCRTableData = response.data.mfoData;
            s.cmfoesdata = response.data.cmfo;
            s.StandardPrOffc = response.data.standardData;
            console.log("StandardPrOffc: ", s.StandardPrOffc);
            console.log("cmfoesdata: ", response.data.cmfo);
            console.log("mycmfoes: ", s.mycmfoes);

            console.log("OPCRTableData: ", s.OPCRTableData);
            s.officeName = response.data.office;

            if (data == undefined) {
                angular.forEach(s.OPCRTableData, function (mfo, keyfirst) {
                    officeid = mfo.TargetOffcId;
                    var keepGoing = true;
                    if (s.StandardPrOffc.length == 0) {
                        status = false;
                        s.assignStandard.push({ status: status, MFOId: mfo.MFOId, MFO: mfo.MFO, indicator: mfo.indicator, officeId: officeid, indicatorId: mfo.indicatorId, target: mfo.target, targetId: mfo.targetId, description: mfo.description, isCMFO: mfo.isCMFO, categoryId: mfo.categoryId });
                    }
                    else {
                        angular.forEach(s.StandardPrOffc, function (standard, key) {
                            if (keepGoing) {
                                if (mfo.indicatorId == standard.indicatorId & s.currentOffice == standard.officeId) {
                                    targetid = standard.targetId;
                                    status = true;
                                    officeid = standard.officeId;
                                    keepGoing = false;
                                }
                                else {
                                    targetid = mfo.targetId;
                                    status = false;
                                    officeid = officeid;
                                }
                            }
                        });
                        s.assignStandard.push({ status: status, MFOId: mfo.MFOId, MFO: mfo.MFO, indicator: mfo.indicator, officeId: officeid, indicatorId: mfo.indicatorId, target: mfo.target, targetId: targetid, description: mfo.description, isCMFO: mfo.isCMFO, categoryId: mfo.categoryId });
                    }
                });
                console.log("assignStandard: ", s.assignStandard);

                angular.forEach(s.cmfoesdata, function (mfo, keyfirst) {
                    officeid = mfo.TargetOffcId;
                    var keepGoing2 = true;
                    if (s.StandardPrOffc.length == 0) {
                        status = false;
                        s.assignCmfo.push({ status: status, MFOId: mfo.MFOId, MFO: mfo.MFO, indicator: mfo.indicator, officeId: officeid, indicatorId: mfo.indicatorId, target: mfo.target, targetId: mfo.targetId, description: mfo.description, isCMFO: mfo.isCMFO, categoryId: mfo.categoryId, catTargetId: mfo.catTargetId, TargetOffcId: mfo.TargetOffcId });
                    }
                    else {
                        angular.forEach(s.StandardPrOffc, function (standard, key) {
                            if (keepGoing2) {
                                if (mfo.indicatorId == standard.indicatorId & mfo.officeId == s.currentOffice & s.currentOffice == standard.officeId) {
                                    targetid = standard.targetId;
                                    status = true;
                                    officeid = standard.officeId;
                                    keepGoing2 = false;
                                }
                                else {
                                    targetid = mfo.targetId;
                                    status = false;
                                    officeid = officeid;
                                }
                            }
                        });
                        s.assignCmfo.push({ status: status, MFOId: mfo.MFOId, MFO: mfo.MFO, indicator: mfo.indicator, officeId: officeid, indicatorId: mfo.indicatorId, target: mfo.target, targetId: targetid, description: mfo.description, isCMFO: mfo.isCMFO, categoryId: mfo.categoryId, catTargetId: mfo.catTargetId, TargetOffcId: mfo.TargetOffcId });

                    }
                });

                console.log("assignCmfo: ", s.assignCmfo);

            }
            s.spinnerCheck = 1;

        }, function (err) {
            //alert("ERROR: loadData");
            console.log("ERROR:", JSON.stringify(err));
        });
        //$http.post('../OPCR/GetPerformance').then(function (responses) {
        //    //alert(JSON.stringify(response.data) + "\n" + data.MFOId)

        //    s.performanceData = responses.data;
        //    console.log("performancedata: ", s.performanceData);
        //    if (s.performanceData.length === 0) {
        //        s.performanceData = [{}, {}, {}, {}, {}];
        //        //alert("dsadas");
        //    }
        //}), function (err) {
        //    alert("error item (getPerformanceData)");
        //}

    }
    //=============
    s.newAssign = function (data) {
        console.log("s.newAssign", data);
        var opcrId = s.currentUser + "-"+ s.checkyear + "-" + s.checksem;
        $http.post('../OPCR/NewAssign', { Assign: data, _OfficeID: s.currentOffice, _opcrId: opcrId }).then(function (response) {

            if (response.data.status = 1) {
                /*   s.OPCRTableData = [{}];
                   s.cmfoesdata = {};
                   s.StandardPrOffc = {};
                   s.assignStandard = [];
                   s.assignCmfo = [];*/
                loadData(true);
                const Toast = Swal.mixin({
                    toast: true,
                    position: 'top-end',
                    showConfirmButton: false,
                    timer: 3000,
                    iconColor: '#FFFFFF',
                    color: '#FFFFFF',
                    timerProgressBar: true,
                    background: '#24a0ed',
                    didOpen: (toast) => {
                        toast.addEventListener('mouseenter', Swal.stopTimer)
                        toast.addEventListener('mouseleave', Swal.resumeTimer)
                    }
                })

                Toast.fire({
                    icon: 'success',
                    title: "Successfully Added!"
                })
            }

        }), function (error) {
        }
    }

    s.newUnassign = function (data) {
        console.log(JSON.stringify(data));
        $http.post('../OPCR/NewUnassign', { Unassign: data, _OfficeID: s.currentOffice }).then(function (response) {
            if (response.data.status = 1) {
                /*s.OPCRTableData = [{}];
                s.cmfoesdata = {};
                s.StandardPrOffc = {};
                s.assignStandard = [];
                s.assignCmfo = [];*/

                loadData(true);
                const Toast = Swal.mixin({
                    toast: true,
                    position: 'top-end',
                    showConfirmButton: false,
                    timer: 3000,
                    iconColor: '#FFFFFF',
                    color: '#FFFFFF',
                    timerProgressBar: true,
                    background: '#ffc107',
                    didOpen: (toast) => {
                        toast.addEventListener('mouseenter', Swal.stopTimer)
                        toast.addEventListener('mouseleave', Swal.resumeTimer)
                    }
                })

                Toast.fire({
                    icon: 'success',
                    title: "Successfully Removed!"
                })
            }
        }), function (error) {

        }
    }

    //s.getListAssignMFO = function (_officeId) {
    //    alert(_officeId);
    //    $http.post('../OPCR/MFO_getList', { _OfficeID: _officeId }).then(function (response) {
    //        s.OPCRTableData = response.data.mfoData;
    //        console.log("OPCRTableData: ", s.OPCRTableData);


    //    }, function (err) {
    //        console.log(JSON.stringify(err));
    //    });
    //}

    //===========================================================



    //// Insert | Update Performance
    //s.add_Performance = function () {


    //    //alert(JSON.stringify(s.addPerformance));
    //    s.button = "Saving data ... "
    //    $http.post('../OPCR/AddPerformance', { tOpcrPerformance: s.addPerformance }).then(function (response) {

    //        if (response.data.isSaved == "success") {
    //            alert(response.data.isSaved);
    //            s.button = "Save changes";
    //            s.getPerformancePer();
    //        }
    //        if (response.data.isSaved == "updated") {
    //            alert("Data Updated!");
    //            s.button = "Save changes";
    //            s.getPerformancePer();
    //        }


    //    }), function (error) {
    //        alert("Error");
    //    }

    //}

    //s.getMfoIndIdToModal = function (data) {
    //    //alert(JSON.stringify(data));
    //    $http.post('../OPCR/GetPerformancePer', { tOpcrPerformance: data }).then(function (response) {
    //        s.getMfoIndID = data;
    //        s.result = response.data;
    //        //    /*alert(JSON.stringify(response.data.data));*/
    //        if (s.result.length === 0) {
    //            s.addPerformance = [{}, {}, {}, {}, {}];
    //            s.addcPerformance = [{}, {}, {}, {}, {}];
    //        }
    //        else {
    //            s.addPerformance = s.result;
    //            s.addcPerformance = s.result;
    //            //s.getMfoIndID = data;
    //        }

    //    }), function (error) {
    //        //    alert("Error");
    //    }
    //    console.log(JSON.stringify(data.indicatorId + "\n" + data.MFOId));

    //}


    //============================================================
    // Get list of MFO and SI Function per office
    //s.getOPCRTableData = function () {
    //    $http.post('../OPCR/MFO_getPerOffice', { _OfficeID: 'OFFPHRMONZ3WT7D' }).then(function (response) {
    //        console.log(response.data)
    //        s.OPCRTableData = response.data;

    //    }, function (err) {
    //        alert("ERROR: getOPCRTableData");
    //    });

    //    s.getPerformancePer();
    //}
    //============================================================
    // Get MFO and SI by MFO ID
    //s.getMFO_SIbyID = function (data) {
    //    s.updateMFOSI = data;
    //}

    //============================================================
    // Update MFO and SI by MFO ID/SI_ID
    //s.updateMFO_SIbyID = function (data) {
    //    $http.post('../OPCR/MFO_updateMFOSI', { _OPCRData: data }).then(function (response) {
    //        if (response.data.status != '') {
    //            alert('MFO Information Updated Successfully');
    //            s.getOPCRTableData(response.data.status);
    //        }
    //    }, function (err) {
    //        alert("ERROR: updateMFO_SIbyID");
    //    });
    //    //alert((data.MFOId + "\n" + data.appropProjectId))
    //    //console.log(JSON.stringify(data));
    //}


    //===================================================================
    // MFO Insert Function
    //s.insertMFO = function (_MFODesc, _AppropriateID) {
    //    $http.post('../OPCR/MFO_insert', { _MFO: _MFODesc, _AppropProjID: _AppropriateID }).then(function (response) {

    //        //alert(response.data.status);
    //        loadData();
    //        s.insertMFOInd(response.data.status);
    //    }), function (err) {
    //        alert("ERROR: insertMFO");
    //    }
    //    //console.log(_MFODesc);
    //    //console.log(_AppropriateID);


    //}
    //===================================================================
    // MFO SIs Insert Function
    //s.insertMFOInd = function (_MFO_id) {
    //    console.log(s.newIndicator);
    //    $http.post('../OPCR/MFO_SIinsert', { indicators: s.newIndicator, MFO_ID: _MFO_id }).then(function (response) {

    //        alert(response.data.status);
    //        //loadData();

    //    }), function (err) {
    //        alert("ERROR: insertMFOInd");
    //    }
    //}
    //===================================================================
    // Get List of Programs Function
    //s.getPrograms = function (_data) {
    //    $http.post('../OPCR/MFO_getPrograms', { _OfficeID: _data }).then(function (response) {
    //        console.log(response.data)
    //        s.programs = response.data;
    //    }), function (err) {
    //        alert("error item (getPrograms)");
    //    }
    //}
    //===================================================================
    //s.getProjects = function (_data) {
    //    //alert(_data);
    //    $http.post('../OPCR/MFO_getProjects', { _ProgramID: _data }).then(function (response) {
    //        console.log(response.data)
    //        s.projects = response.data;
    //    }), function (err) {
    //        alert("error item (getProjects)");
    //    }
    //}
    //==================================================================
    //

    s.addIndicatorField = function () {
        s.newIndicator.push({});
    }
    //============================================================
    // Get Performance 
    //s.getPerformancePer = function () {
    //    //alert(JSON.stringify(data));
    //    //console.log(data);
    //    $http.post('../OPCR/GetPerformance').then(function (responses) {
    //        //alert(JSON.stringify(response.data) + "\n" + data.MFOId)

    //        s.performanceData = responses.data;
    //        console.log(s.performanceData);
    //        if (s.performanceData.length === 0) {
    //            s.performanceData = [{}, {}, {}, {}, {}];
    //            // alert("dsadas");
    //        }
    //    }), function (err) {
    //        alert("error item (getPerformanceData)");
    //    }
    //}

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

    s.rowCounttarget = function (_mfoID, data) {
        s.counts = 0;
        for (var ind = 0; ind < data.length; ind++) {
            if (_mfoID == data[ind].MFOId) {
                s.counts++;
            }
        }
        s.counts = s.counts + 2;
        return s.counts;
    }



    s.removedInd = function (_indicatorId) {
        Swal.fire({
            title: 'Are you sure?',
            text: "",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes!'
        }).then((result) => {
            if (result.isConfirmed) {
                $http.post('../OPCR/removeIndicator', { indicatorId: _indicatorId }).then(function (response) {

                    Swal.fire(
                        'Removed!',
                        'The Success Indicator has been removed.',
                        'success'
                    )
                    loadData();

                }), function (error) {

                }

            }
        })
    }

    s.prt_target = function () {

        //hr
        s.officeId = "OFFPHRMONZ3WT7D";

        //pbo
        //s.officeId = "OFFPBOEZ7SC4ZA9";

        $http.post('../OPCR/cookiestarget', { officeId: "OFFPHRMONZ3WT7D" }).then(function (responses) {
            // alert(responses.data.data);
            window.open('../Reports/SPMS/Target.aspx?type=Target');
        }), function (err) {
            alert("error item (getPerformanceData)");
        }
    }


    s.dropdownClr = function () {
        s.categoryopcr = "";
    }
    s.dropdownClrModal = function () {
        s.category = "";
    }
    s.searchClrModal = function () {
        s.search = null;
    }
    s.officeNameH = function () {
        return s.officeName.officeName; 
    }
    //=============
    s.updateTarget = function (data) {

        //angular.forEach(data, function (ndata, keyfirst) {

        //    alert(ndata.MFO+"\n"+ndata.division + "\n" + ndata.MFOId);

        //})
        data = DuplicateRemover(data, 'MFOId');
        console.log("Update: ", data);
        $http.post('../OPCR/updateMFO_Division', { targetTableData: data }).then(function (responses) {
            // alert(responses.data.data);
            if (responses.data.status = 1) {
                loadData();
                const Toast = Swal.mixin({
                    toast: true,
                    position: 'top-end',
                    showConfirmButton: false,
                    timer: 3000,
                    iconColor: '#FFFFFF',
                    color: '#FFFFFF',
                    timerProgressBar: true,
                    background: '#24a0ed',
                    didOpen: (toast) => {
                        toast.addEventListener('mouseenter', Swal.stopTimer)
                        toast.addEventListener('mouseleave', Swal.resumeTimer)
                    }
                })

                Toast.fire({
                    icon: 'success',
                    title: "Successfully Updated!"
                })
            }
        }), function (err) {
            alert("error item (updateTarget)");
        }

    }

    function DuplicateRemover(collection, keyname) {
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

    s.tgt_indDetails = {};
    s.tempData = {};
    s.tgt_indStandardDetails = {};
    s.getIndicatorDetails = function (_indId, _targtId) {
        //alert(_targtId);
        $http.post('../OPCR/MFO_getIndicatorDetails', { indId: _indId, targtId: _targtId }).then(function (response) {
            s.tgt_indDetails = response.data.indDetails;
            s.tgt_indStandardDetails = response.data.standardDatas;
            console.log("here: ", s.tgt_indStandardDetails);
        }, function (err) {
            alert("ERROR: GetIndicatorsDetails");
        });
    }

    /*  ==================================================== CMFO CLICK IN MODAL ====================================*/

    s.getCMFO = function (data) {
        console.log("s.getCMFO / CMFO CLICK", data);
        s.cmfoes = {};
        s.catcmfoes = {};
        s.targetStandard = [{}, {}, {}, {}, {}];
        s.selectedunit = 'N';

        $http.post('../OPCR/MFO_showAddtgt', { CMFO: data, _OfficeID: s.currentOffice }).then(function (response) {
            console.log("response.data.cmfo", response.data.cmfo);
            if (response.data.status == 1) {
                if (response.data.cmfo != null) {
                    s.cmfoes = response.data.cmfo;
                    s.onChangeTarget(s.cmfoes.target);
                    s.selectedunit = s.cmfoes.targetUnit;
                    s.targetStandard = response.data.standard;
                    console.log("s.targetStandard", response.data.standard);


                }
                else {
                    s.cmfoes = data;
                    s.onChangeTarget(s.cmfoes.target);
                }
                s.catcmfoes = response.data.catcmfo.categoryId;
                console.log(" s.catcmfoes", s.catcmfoes);
                console.log("s.getCMFO / RESULT", s.cmfoes);
            }

        }, function (err) {
            alert("error item");
        });
    }
    /*  ==================================================== CMFO ADD TARGET & CATEGORY MODAL ====================================*/
    s.addCMFOtarget = function (data) {
        console.log("addCMFOtarget", data, s.targetStandard);
        $http.post('../OPCR/CMFO_addTargetCat', { cmfoes: data, standardData: s.targetStandard, _OfficeID: s.currentOffice }).then(function (response) {
            if (response.data.status == 1) {
                const Toast = Swal.mixin({
                    toast: true,
                    position: 'top-end',
                    showConfirmButton: false,
                    timer: 3000,
                    iconColor: '#FFFFFF',
                    color: '#FFFFFF',
                    timerProgressBar: true,
                    background: '#24a0ed',
                    didOpen: (toast) => {
                        toast.addEventListener('mouseenter', Swal.stopTimer)
                        toast.addEventListener('mouseleave', Swal.resumeTimer)
                    }
                })

                Toast.fire({
                    icon: 'success',
                    title: "Successfully Updated!"
                })
                s.OPCRTableData = {};
                s.StandardPrOffc = {};
                s.assignStandard = [];
                s.assignCmfo = [];
                loadData();
            }

        }), function (error) { }
    }
    /*   ====================================================================================== TARGET COMPUTATION ======================================================================================*/

    s.units = [
        { unit: 'N' },
        { unit: 'P' },
    ]
    s.selectedunit = 'N';
    s.target = 0;

    s.onChangeTarget = function (targetCount) {
        s.target = targetCount;
        if (s.selectedunit == 'N') {
            if (s.target == 0) {
                s.r5 = null; s.r4 = null; s.r3 = null; s.r2 = null; s.r1 = null;

            }
            if (s.target == 1) { s.r5 = 1; s.r4 = null; s.r3 = null; s.r2 = null; s.r1 = 0; }

            else {
                s.target = targetCount;
                if (/^\d+(\.\d+)?%$/.test(s.target)) {
                    s.r5 = null; s.r4 = null; s.r3 = null; s.r2 = null; s.r1 = null;
                }
                else {
                    s.r5var = ((parseFloat(s.target) * 0.3) + parseFloat(s.target)); s.r5 = Math.round(s.r5var);
                    s.r4var = ((parseFloat(s.target) * 0.15) + parseFloat(s.target)); s.r4 = Math.round(s.r4var);
                    s.r3 = s.target;
                    s.r2var = ((parseFloat(s.target) / 2) + 1); s.r2 = Math.round(s.r2var);
                    s.r1 = Math.round(s.r2 - 1);
                }
            }
        }
    }

    // ------------ TEST CODE ------------

    s.checkyear = 112321;
    s.checksem = 0;
    s.currentUser = "EMP128";
    s.saveToSession = function (_year,_sem,_uid) {
        //Session.save(_year, _year);
        //Session.save(_sem, _sem);
        //Session.save(_uid, _uid);
        //if (_year != null) {
        //    s.checkyear = (Session.get(_year));
        //    s.checksem = (Session.get(_sem));
        //    s.currentUser = Session.get(_uid);
        //    loadData();
        //}
        document.cookie = _uid + "=" + _uid + ";";
        document.cookie = _uid + "year=" + _year + ";";
        document.cookie = _uid + "sem=" + _sem + ";";
        loadData();
    }
    loadSession();
    function loadSession() {

        s.currentUser = "EMP128";
        let cUser = s.currentUser;
        console.log("cUser: ", cUser);
        if (cUser != "") {
            s.checkyear = getCookie(cUser + "year");
            s.checksem = getCookie(cUser + "sem");
            s.currentUser = getCookie(cUser);

            console.log("year: ", s.checkyear);
            console.log("sem: ", s.checksem);
            console.log("id: ", s.currentUser);
            loadData();
            return s.checkyear;
        } else {
            //currentUser = prompt("Please enter your employee ID:", "");
            //if (cUser != "" && cUser != null) {
            //    setCookie(s.currentUser, cUser, 365);
            //}
        }
        return s.checkyear;
    }

    function getCookie(cname) {
        let name = cname + "=";
        let decodedCookie = decodeURIComponent(document.cookie);
        let ca = decodedCookie.split(';');
        
        for (let i = 0; i < ca.length; i++) {
            let c = ca[i];
            while (c.charAt(0) == ' ') {
                c = c.substring(1);
            }
            if (c.indexOf(name) == 0) {
                return c.substring(name.length, c.length);
            }
        }
        return "";
    }
  

    // -------- END OF TEST CODE ---------

});