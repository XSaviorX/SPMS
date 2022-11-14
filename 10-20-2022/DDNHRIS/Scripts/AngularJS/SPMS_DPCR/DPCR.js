
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

app.controller("SPMS_DPCR", function ($scope, $http, filterFilter) {

    var s = $scope;
    s.IndexCnt = 0;
    s.mfoes = {};
    s.newIndicator = [{}];
    s.indicators = {};
    s.listofOffices = {};
    s.AssignedAppropProjID = {};
    s.OPCRTableData = [{}];
    s.geteditMFOSI = {};
    s.updateMFOSI = {};
    s.performanceData = {};
    s.getPerformance = {};
    s.addPerformance = {};
    s.getMfoInd = {}; //Show All MFO and Success Indicator
    s.getMfoIndID = {}; //Get One S
    s.assignStandard = [];
    s.count = 0;
    s.assignCmfo = [];
    s.assignOpcrCmfo = [];

    s.targetStandard = [{}, {}, {}, {}, {}];
    s.addStandard = [{}, {}, {}, {}, {}];

    s.currentOffice = "OFFPHRMONZ3WT7D";
    s.currentDivision = '';
    loadData();
    s.officeId = "OFFPBOEZ7SC4ZA9";
    s.print = function () {
        $http.post('../SPMS_DPCR/cookies', { officeId: s.officeId }).then(function (responses) {
            window.open('../Reports/SPMS/Opcr.aspx?type=opcr');
        }), function (err) {
            alert("error item (getPerformanceData)");
        }
    }
    // s.currentDiv = "";

    s.cmfoes = [];

    function loadData(data) {
        console.log("data", data);

        s.currentOffice = 'OFFPHRMONZ3WT7D';
        // s.currentDivision = 'DIVAPRD';
        s.HRAccnt = "OFFPHRMONZ3WT7D";
        //PBO = OFFPBOEZ7SC4ZA9
        //COA = OFFCOAYW2FQ3EM1
        //COMELEC = OFFCOMELECXV2XW

        $http.post('../SPMS_DPCR/MFO_getPerOffice', { _OfficeID: s.currentOffice, _DivisionID: s.currentDivision }).then(function (response) {
            console.log("data: ", response.data)
            //s.OPCRTableData = response.data;
            s.OPCRTableData = response.data.mfoData; // Office mfo
            s.cmfoesdata = response.data.cmfo; // cmfoes
            s.StandardPrOffc = response.data.standardData; // assigned mfo
            s.CheckListData = response.data.CLData; // checklist mfo
            s.officeName = response.data.office; // current office
            // s.cmfoes = response.data.divisionMFO; // division mf o

            console.log("StandardPrOffc: ", s.StandardPrOffc);
            console.log("cmfoesdata: ", response.data.cmfo);
            console.log("CheckListData: ", s.CheckListData);
            console.log("OPCRTableData: ", s.OPCRTableData);
            s.officeName = response.data.office;

            if (data == undefined) {

                //-------------------------------------------------------------------------- UNIQUE MFO --------------------------------------------------------------------------------------------------------------------------------------------
                angular.forEach(s.OPCRTableData, function (mfo, keyfirst) {
                    officeid = mfo.TargetOffcId;
                    var keepGoing = true;
                    if (s.StandardPrOffc.length == 0) {
                        status = false;
                        s.assignStandard.push({ status: status, MFOId: mfo.MFOId, MFO: mfo.MFO, indicator: mfo.indicator, divisionId: mfo.divisionId, officeId: officeid, indicatorId: mfo.indicatorId, target: mfo.target, tRemaining: mfo.tRemaining, targetId: mfo.targetId, description: mfo.description, isCMFO: mfo.isCMFO, categoryId: mfo.categoryId, isShared: mfo.isShared });
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
                        s.assignStandard.push({ status: status, MFOId: mfo.MFOId, MFO: mfo.MFO, indicator: mfo.indicator, officeId: officeid, divisionId: mfo.divisionId, indicatorId: mfo.indicatorId, target: mfo.target, tRemaining: mfo.tRemaining, targetId: targetid, description: mfo.description, isCMFO: mfo.isCMFO, categoryId: mfo.categoryId, isShared: mfo.isShared });
                    }
                });
                console.log("assignStandard: ", s.assignStandard);

                //-------------------------------------------------------------------------- Office COMMON MFO --------------------------------------------------------------------------------------------------------------------------------------------
                angular.forEach(s.cmfoesdata, function (mfo, keyfirst) {
                    officeid = mfo.TargetOffcId;
                    divisionid = mfo.divisionId;
                    var keepGoing2 = true;
                    if (s.StandardPrOffc.length == 0) {
                        status = false;
                        s.assignOpcrCmfo.push({ status: status, MFOId: mfo.MFOId, MFO: mfo.MFO, indicator: mfo.indicator, officeId: officeid, divisionId: divisionid, indicatorId: mfo.indicatorId, target: mfo.target, tRemaining: mfo.tRemaining, targetId: mfo.targetId, description: mfo.description, categoryId: mfo.categoryId, TargetOffcId: mfo.TargetOffcId });
                    }
                    else {
                        angular.forEach(s.StandardPrOffc, function (standard, key) {
                            if (keepGoing2) {
                                if (mfo.indicatorId == standard.indicatorId & mfo.officeId == s.currentOffice & s.currentOffice == standard.officeId & s.currentDivision == standard.divisionId & mfo.targetId == standard.targetIdparent) {
                                    targetid = standard.targetId;
                                    status = true;
                                    officeid = standard.officeId;
                                    divisionid = null;
                                    keepGoing2 = false;
                                }
                                else {
                                    targetid = mfo.targetId;
                                    status = false;
                                    officeid = officeid;
                                    divisionid = divisionid;
                                }
                            }
                        });
                        if (mfo.officeId != null & mfo.divisionId == null | mfo.officeId == null & mfo.divisionId == null) {
                            s.assignOpcrCmfo.push({ status: status, MFOId: mfo.MFOId, MFO: mfo.MFO, indicator: mfo.indicator, officeId: officeid, divisionId: divisionid, indicatorId: mfo.indicatorId, target: mfo.target, tRemaining: mfo.tRemaining, targetId: targetid, description: mfo.description, categoryId: mfo.categoryId, TargetOffcId: mfo.TargetOffcId });

                        }

                    }
                });
                console.log("assignOpcrCmfo: ", s.assignOpcrCmfo);


                //-------------------------------------------------------------------------- DIVISION COMMON MFO --------------------------------------------------------------------------------------------------------------------------------------------
                angular.forEach(s.cmfoesdata, function (mfo, keyfirst) {
                    officeid = mfo.TargetOffcId;
                    divisionid = mfo.divisionId;
                    _isCMFO = 1;

                    var keepGoing2 = true;
                    if (s.StandardPrOffc.length == 0) {
                        status = false;
                        s.assignCmfo.push({ status: status, isCMFO: _isCMFO, MFOId: mfo.MFOId, MFO: mfo.MFO, indicator: mfo.indicator, officeId: officeid, divisionId: mfo.divisionId, indicatorId: mfo.indicatorId, target: mfo.target, targetId: mfo.targetId, catTargetId: mfo.catTargetId, description: mfo.description, categoryId: mfo.categoryId, TargetOffcId: mfo.TargetOffcId });
                    }
                    else {
                        angular.forEach(s.StandardPrOffc, function (standard, key) {
                            if (keepGoing2) {
                                if (mfo.indicatorId == standard.indicatorId & mfo.officeId == s.currentOffice & s.currentOffice == standard.officeId & s.currentDivision == standard.divisionId & mfo.targetId == standard.targetId) {
                                    targetid = standard.targetId;
                                    status = true;
                                    officeid = standard.officeId;
                                    divisionid = standard.divisionId;
                                    keepGoing2 = false;
                                }
                                else {
                                    targetid = mfo.targetId;
                                    status = false;
                                    officeid = officeid;
                                    divisionid = divisionid;
                                }
                            }
                        });
                        if (mfo.officeId != null & mfo.divisionId != null | mfo.officeId == null & mfo.divisionId == null) {
                            s.assignCmfo.push({ status: status, isCMFO: _isCMFO, MFOId: mfo.MFOId, MFO: mfo.MFO, indicator: mfo.indicator, officeId: officeid, divisionId: divisionid, indicatorId: mfo.indicatorId, target: mfo.target, targetId: targetid, catTargetId: mfo.catTargetId, description: mfo.description, categoryId: mfo.categoryId, TargetOffcId: mfo.TargetOffcId });

                        }

                    }
                });
                console.log("assignCmfo: ", s.assignCmfo);

                document.querySelector('.loadview').style.display = 'block';
            }

        }, function (err) {
            alert("error item");
        });
        $http.post('../SPMS_DPCR/GetPerformance').then(function (responses) {
            //alert(JSON.stringify(response.data) + "\n" + data.MFOId)

            s.performanceData = responses.data;
            // console.log("performancedata: ", s.performanceData);
            if (s.performanceData.length === 0) {
                s.performanceData = [{}, {}, {}, {}, {}];
                //alert("dsadas");
            }
        }), function (err) {
            console.log(JSON.stringify(err));
        }

    }
    //========================================================================================================= Show users per mfo in modal ====================================================================================================================

    s.users = {};
    s.getUsers = function (data) {
        s.mfoTgtdata = data;
        console.log("getUsers", data);
        $http.post('../SPMS_DPCR/SHOW_USERS', { IPCRData: data }).then(function (response) {
            if (response.data.status == 1) {
                s.users = response.data.users;
                console.log("s.users", s.users);



            }

        }), function (error) { }
    }


    // --------------- set division ------------
    s.setDivivion = function (data) {
        s.currentDivision = data;
        loadData();


    }
    //------------------------------ ADD TARGET TO SHARED MFO ------------------------------------------
    s.showTgtData = {};
    s.committed;
    s.show_addTargetbtn = function (data) {
        s.committed = undefined;
        s.showTgtData = data;
        console.log("addTarget", s.showTgtData)
    }
    //------------------------------ EDIT TARGET  SHARED MFO ------------------------------------------

    s.editTgtData = {};
    s.show_edtTargetbtn = function (data) {
        s.editTgtData = data;
        console.log("editTarget", s.editTgtData);

        s.committed = s.editTgtData.target;
        s.committedSrc = s.editTgtData.target; //for display purposes
        s.rt_remaining; ////for display purposes

        $http.post('../SPMS_DPCR/edit_targetShared', { MFOdata: data }).then(function (response) {
            s.showTgtData = response.data;
            console.log("showTgtData", s.showTgtData);

        }), function (error) {
        }

    }
    //------------------------------ UPDATE TARGET  SHARED MFO ------------------------------------------


    s.update_sharedTgt = function () {

        $http.post('../SPMS_DPCR/update_targetShared', { ParentTgt: s.showTgtData, ChildTgt: s.editTgtData, Committed: s.committed }).then(function (response) {
            if (response.data.status == 1) {
                s.OPCRTableData = {};
                s.StandardPrOffc = {};
                s.assignStandard = [];
                s.assignCmfo = [];
                s.assignOpcrCmfo = [];

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
                    title: "Successfully Submited!"
                })
            }


        }), function (error) {
        }
    }

    //------------------------------ SAVE TARGET TO SHARED MFO ------------------------------------------
    s.save_targetShared = function () {
        console.log("save_targetShared", s.showTgtData)
        document.getElementById("submit_target").disabled = true;

        $http.post('../SPMS_DPCR/save_targetShared', { TgtData: s.showTgtData, Committed: s.committed, _OfficeID: s.currentOffice, _DivisionID: s.currentDivision }).then(function (response) {
            if (response.data.status == 1) {
                $('#show_addTarget').modal('hide');

                document.getElementById("submit_target").disabled = false;

                s.OPCRTableData = [{}];
                s.cmfoesdata = {};
                s.StandardPrOffc = {};
                s.assignStandard = [];
                s.assignCmfo = [];
                s.assignOpcrCmfo = [];
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
                    title: "Successfully Submited!"
                })
            }


        }), function (error) {
        }
    }


    //=============
    s.newAssign = function (data) {
        alert(JSON.stringify(data));
        $http.post('../SPMS_DPCR/NewAssign', { Assign: data, _OfficeID: s.currentOffice, _DivisionID: s.currentDivision }).then(function (response) {
            if (response.data.status == 1) {

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
            if (response.data.status == 2) {
                s.assignStandard = [];
                s.assignCmfo = [];
                s.assignOpcrCmfo = [];
                loadData();
                Swal.fire(
                    'This Success Indicator is already added.',
                    'Please check it in other Tab.',
                    'error'
                )
            }

        }), function (error) {
        }
    }
    s.newUnassign = function (data) {
        console.log("remove:", JSON.stringify(data));
        $http.post('../SPMS_DPCR/NewUnassign', { Unassign: data, _OfficeID: s.currentOffice, _DivisionID: s.currentDivision }).then(function (response) {
            if (response.data.status == 1) {

                if (response.data.isShared == 0 || response.data.isShared == null) {
                    loadData(true);
                }
                else {
                    s.OPCRTableData = {};
                    s.StandardPrOffc = {};
                    s.assignStandard = [];
                    s.assignCmfo = [];
                    s.assignOpcrCmfo = [];

                    loadData();
                }

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

    /*  ==================================================== CMFO CLICK IN MODAL ====================================*/
    s.getCMFO = function (data) {
        console.log("s.getCMFO / CMFO CLICK", data);
        s.cmfoes = {};
        s.catcmfoes = {};
        s.targetStandard = [{}, {}, {}, {}, {}];
        s.selectedunit = '#';

        $http.post('../SPMS_DPCR/MFO_showAddtgt', { CMFO: data, _OfficeID: s.currentOffice, _DivisionID: s.currentDivision }).then(function (response) {
            console.log("response.data.cmfo", response.data.cmfo);
            if (response.data.status == 1) {
                if (response.data.cmfo != null) {
                    s.cmfoes = response.data.cmfo;
                    s.selectedunit = s.cmfoes.targetUnit;
                    s.targetStandard = response.data.standard;
                }
                else {
                    s.cmfoes = data;
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
    s.isValidcateg;
    s.isValidtgt;
    s.addCMFOtarget = function (data) {
        console.log("addCMFOtarget", data, s.targetStandard);

        $http.post('../SPMS_DPCR/CMFO_addTargetCat', { cmfoes: data, standardData: s.targetStandard, _OfficeID: s.currentOffice, _DivisionID: s.currentDivision }).then(function (response) {
            if (response.data.status == 1) {
                $('#addTargetCMFO').modal('hide');
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
                s.assignOpcrCmfo = [];

                loadData();
            }

        }), function (error) { }
    }

    //===========================================================
    // Insert | Update Performance
    s.add_Performance = function () {


        //alert(JSON.stringify(s.addPerformance));
        s.button = "Saving data ... "
        $http.post('../SPMS_DPCR/AddPerformance', { tOpcrPerformance: s.addPerformance }).then(function (response) {

            if (response.data.isSaved == "success") {
                alert(response.data.isSaved);
                s.button = "Save changes";
                s.getPerformancePer();
            }
            if (response.data.isSaved == "updated") {
                alert("Data Updated!");
                s.button = "Save changes";
                s.getPerformancePer();
            }


        }), function (error) {
            alert("Error");
        }

    }

    s.getMfoIndIdToModal = function (data) {
        //alert(JSON.stringify(data));
        $http.post('../SPMS_DPCR/GetPerformancePer', { tOpcrPerformance: data }).then(function (response) {
            s.getMfoIndID = data;
            s.result = response.data;
            //    /*alert(JSON.stringify(response.data.data));*/
            if (s.result.length === 0) {
                s.addPerformance = [{}, {}, {}, {}, {}];
                s.addcPerformance = [{}, {}, {}, {}, {}];
            }
            else {
                s.addPerformance = s.result;
                s.addcPerformance = s.result;
                //s.getMfoIndID = data;
            }

        }), function (error) {
            //    alert("Error");
        }
        console.log(JSON.stringify(data.indicatorId + "\n" + data.MFOId));

    }


    //============================================================
    // Get list of MFO and SI Function per office
    s.getOPCRTableData = function () {
        $http.post('../SPMS_DPCR/MFO_getPerOffice', { _OfficeID: 'OFFPHRMONZ3WT7D' }).then(function (response) {
            console.log(response.data)
            s.OPCRTableData = response.data;

        }, function (err) {
            console.log(JSON.stringify(err));
        });

        s.getPerformancePer();
    }
    //============================================================
    // Get MFO and SI by MFO ID
    s.getMFO_SIbyID = function (data) {
        s.updateMFOSI = data;
    }

    //============================================================
    // Update MFO and SI by MFO ID/SI_ID
    s.updateMFO_SIbyID = function (data) {
        $http.post('../SPMS_DPCR/MFO_updateMFOSI', { _OPCRData: data }).then(function (response) {
            if (response.data.status != '') {
                alert('MFO Information Updated Successfully');
                s.getOPCRTableData(response.data.status);
            }
        }, function (err) {
            alert("error item");
        });
        //alert((data.MFOId + "\n" + data.appropProjectId))
        //console.log(JSON.stringify(data));
    }


    //===================================================================
    // MFO Insert Function
    s.insertMFO = function (_MFODesc, _AppropriateID) {
        $http.post('../SPMS_DPCR/MFO_insert', { _MFO: _MFODesc, _AppropProjID: _AppropriateID }).then(function (response) {

            //alert(response.data.status);
            loadData();
            s.insertMFOInd(response.data.status);
        }), function (err) {
            alert("error item");
        }
        //console.log(_MFODesc);
        //console.log(_AppropriateID);


    }
    //===================================================================
    // MFO SIs Insert Function
    s.insertMFOInd = function (_MFO_id) {
        console.log(s.newIndicator);
        $http.post('../SPMS_DPCR/MFO_SIinsert', { indicators: s.newIndicator, MFO_ID: _MFO_id }).then(function (response) {

            alert(response.data.status);
            //loadData();

        }), function (err) {
            alert("error item");
        }
    }
    //===================================================================
    // Get List of Programs Function
    s.getPrograms = function (_data) {
        $http.post('../SPMS_DPCR/MFO_getPrograms', { _OfficeID: _data }).then(function (response) {
            console.log(response.data)
            s.programs = response.data;
        }), function (err) {
            alert("error item (getPrograms)");
        }
    }
    //===================================================================
    s.getProjects = function (_data) {
        //alert(_data);
        $http.post('../SPMS_DPCR/MFO_getProjects', { _ProgramID: _data }).then(function (response) {
            console.log(response.data)
            s.projects = response.data;
        }), function (err) {
            alert("error item (getProjects)");
        }
    }
    //==================================================================
    //

    s.addIndicatorField = function () {
        s.newIndicator.push({});
    }
    //============================================================
    // Get Performance 
    s.getPerformancePer = function () {
        //alert(JSON.stringify(data));
        //console.log(data);
        $http.post('../SPMS_DPCR/GetPerformance').then(function (responses) {
            //alert(JSON.stringify(response.data) + "\n" + data.MFOId)

            s.performanceData = responses.data;
            console.log(s.performanceData);
            if (s.performanceData.length === 0) {
                s.performanceData = [{}, {}, {}, {}, {}];
                // alert("dsadas");
            }
        }), function (err) {
            alert("error item (getPerformanceData)");
        }
    }
    s.rowCOUNT = 0;
    s.countCL = 0;
    s.changeCount = function (data) {
        if (s.rowCOUNT >= 3) {
            s.rowCOUNT = 2;
        }
        else {
            s.rowCOUNT = data + 1;
            //alert(s.rowCOUNT);
        }


    }
    s.mfoidNow = "";
    s.countInd = 1;
    s.rowCount = function (_mfoID, data) {

        s.counts = 0;
        for (var ind = 0; ind < data.length; ind++) {
            if (_mfoID == data[ind].MFOId) {
                s.counts++;
            }
        }
        s.counts = s.counts + (s.counts * 5);
        // s.counts - s.countCL;
        //  console.log("COUNT" + s.counts);

        return s.counts;
    }
    s.rowCount2 = function (_mfoID) {
        alert(_mfoID);
        s.countInd = _mfoID;

        return _mfoID;
    }


    s.removedInd = function (_indicator) {
        console.log("removedInd", JSON.stringify(_indicator));
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
                $http.post('../SPMS_DPCR/removeIndicator', { indicator: _indicator }).then(function (response) {

                    Swal.fire(
                        'Removed!',
                        'The Success Indicator has been removed.',
                        'success'
                    )
                    s.OPCRTableData = [{}];
                    s.cmfoesdata = {};
                    s.StandardPrOffc = {};
                    s.assignStandard = [];
                    s.assignCmfo = [];
                    s.assignOpcrCmfo = [];
                    loadData();

                }), function (error) {

                }

            }
        })


    }




    s.dropdownClrModal = function () {

        s.category = "";
    }
    s.searchClrModal = function () {
        s.search = null;
    }


    /*   ====================================================================================== TARGET COMPUTATION ======================================================================================*/



    s.units = [
        { unit: '#' },
        { unit: '%' },
    ]
    s.selectedunit = '#';
    s.target = 0;

    s.onChangeTarget = function (targetCount) {
        s.target = targetCount;
        if (s.selectedunit == '#') {
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
        if (s.standard_show[2].quantity == 0) {
            s.onChangeTarget(null);
            s.newSI.target = null;
        }
        else {
            s.onChangeTarget(s.standard_show[2].quantity);
            s.newSI.target = s.standard_show[2].quantity;
        }

        s.addStandard = s.standard_show;
    }

    /*   ====================================================================================== ADD CHECKLIST BUTTON ======================================================================================*/
    s.objMFO = {};
    s.addCheckListBtn = function (data) {
        $http.post('../SPMS_DPCR/MFO_checkList', { MFOID: data.MFOId }).then(function (response) {
            if (response.data.status == 1) {
                Swal.fire(
                    `Can't add activity.`,
                    'This MFO has multiple success indicators.',
                    'info'
                )
            }
            else {
                $('#addCheckList').modal('show');

                s.selectedunit = "#";
                s.addStandard = [{}, {}, {}, {}, {}];
                s.objMFO = data;

            }


        }), function (err) {

        }

    }
    /*   ====================================================================================== ADD CHECKLIST  MFO ======================================================================================*/
    s.newMFO = {};
    s.addStandard = [{}, {}, {}, {}, {}];

    s.addCLMFO = function (_newMFO) {


        if (_newMFO.MFO != null && _newMFO.indicator != null && _newMFO.target != null) {
            document.getElementById("addcl").disabled = true;


            $http.post('../SPMS_DPCR/MFO_addCL', { MFOID: s.objMFO.MFOId, newMFO: _newMFO, newStandard: s.addStandard, _OfficeID: s.currentOffice }).then(function (response) {

                if (response.data.data == 1) {
                    document.getElementById("addcl").disabled = false;

                    s.newMFO = {}; s.r5 = null; s.r4 = null; s.r3 = null; s.r2 = null; s.r1 = null; s.addStandard = [{}, {}, {}, {}, {}];
                    s.objCMFO = response.data.objCMFO;

                    loadData();
                    $('#addCheckList').modal('hide');
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
            }), function (error) {

            }
        }
        else {
            alert("Error! Required Field is empty.")
        }

    }
    /*   ====================================================================================== ADD NEW MFO ======================================================================================*/
    s.newMFO = {};
    s.catMFO = {};
    s.newSI = {};
    s.addStandards = [{}, {}, {}, {}, {}];
    s.objMFO = {};
    s.isCheck = false;

    s.addMFO = function (_newMFO, _catMFO, _newSI) {
        console.log("s.addStandard", s.addStandards)

        if (_newMFO.MFO != null && _newSI.indicator != null && _newSI.target != null && _catMFO.categoryId != null) {
            $http.post('../SPMS_DPCR/MFO_add', { newMFO: _newMFO, catMFO: _catMFO, newSI: _newSI, newStandard: s.addStandards, _OfficeID: s.currentOffice, _DivisionID: s.currentDivision, isCheck: s.isCheck }).then(function (response) {

                if (response.data.data == 1) {
                    s.newMFO = {}; s.catMFO = {}; s.newSI = {}; s.r5 = null; s.r4 = null; s.r3 = null; s.r2 = null; s.r1 = null; s.addStandard = [{}, {}, {}, {}, {}];
                    s.objMFO = response.data.objMFO;
                    s.cmfoes = [];
                    loadData();
                    s.isCheck = false;
                    $('#addMFOForm').modal('hide');


                    Swal.fire({
                        title: 'Add more success indicator?',
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
                else if (response.data.data == 0) {
                    alert("Error! MFO already exist in this division.")
                }

            }), function (error) {

            }
        }
        else {
            alert("Error! Required field is empty.")
        }

    }
    /*   ====================================================================================== DROPDOWN FUNCTION ======================================================================================*/
    s.show = 0;
    s.selectedMFO = "";
    s.onChangeShow = function (data) {
        s.show = data;

    }
    s.mfoid = null;
    s.inputChangeVal = function (mfoid, mfo, close) {

        s.mfoid = mfoid;
        s.selectedMFO = mfo;
        s.show = close;

    }

    /*   ====================================================================================== GET MFOID FROM DROPDOWN AND MODAL CONFIRMATION ============================================================*/
    s.getMFOperId = function (data) {
        s.mfoid = data;
        if (s.mfoid != null) {
            $http.post('../SPMS_DPCR/MFO_getMFOperId', { MFOId: s.mfoid }).then(function (response) {
                if (response.data.status == 1) {
                    s.objMFO = response.data.objMFO;
                    $('#addMoreSIForm').modal('show');
                }
                else {
                    Swal.fire(
                        `Can't add Success Indicator`,
                        'This MFO has already set of activities.',
                        'info'
                    )
                }

                /* s.showBckBtn = true;
                 $('#addSIForm').modal('hide');
                */
            }), function (error) {

            }
        }

    };

    /*   ====================================================================================== ADD MORE SUCCESS INDICATOR ======================================================================================*/

    s.moreSI = function (_newSI) {
        /* alert(JSON.stringify(s.objCMFO[0])); get the first array object*/
        if (_newSI.indicator != null && _newSI.target != null) {
            $http.post('../SPMS_DPCR/MFO_moreSI', { getMFO: s.objMFO[0], newSI: _newSI, newStandard: s.addStandard, _OfficeID: s.currentOffice, isCheck: s.isCheck }).then(function (response) {
                if (response.data.data == 1) {
                    s.newMFO = {}; s.catCMFO = {}; s.newSI = {}; s.r5 = null; s.r4 = null; s.r3 = null; s.r2 = null; s.r1 = null; s.addStandard = [{}, {}, {}, {}, {}];
                    s.objMFO = response.data.objMFO;
                    console.log("s.objMFO", s.objMFO);
                    s.cmfoes = [];
                    loadData();
                    s.isCheck = false;
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
            alert("Error! Required field is empty.");
        }

    }

    /*   ====================================================================================== SHOW Selected MFO AND ITS SI ===================================================================================================*/
    s.categorymfo = {};
    s.showSIbyMFO = function (_mfoData) {
        console.log("showSIbyMFO", _mfoData)

        $http.post('../SPMS_DPCR/MFO_showMFO_SI', { mfoData: _mfoData }).then(function (response) {
            s.showMFO = response.data.data;
            s.selectedunit = s.showMFO.targetUnit;
            s.standard_show = response.data.standard;
            s.onChangeTarget(s.showMFO.target);
            if (s.showMFO.categoryId == 2) {
                document.getElementById("showcategory2").checked = true;

            }
            else {
                document.getElementById("showcategory2").checked = false;

            }
            if (s.showMFO.categoryId == 3) {
                document.getElementById("showcategory3").checked = true;
            }
            else {
                document.getElementById("showcategory3").checked = false;
            }
            console.log(s.updateMFO_allSI);
        }, function (err) {
            alert("error item");
        });
    }
    /*   ====================================================================================== SHOW Selected MFO AND ITS SI FROM ASSIGN MODAL ===================================================================================================*/
    s.showSIbyMFO2 = function (data) {

        console.log("showSIbyMFO2", data);
        $http.post('../SPMS_DPCR/MFO_showMFO_SI2', { mfoData: data, DivisionID: s.currentDivision }).then(function (response) {
            s.showMFO = response.data.data;
            s.standard_show = response.data.standard;
            console.log("s.showMFO", s.showMFO);
            s.selectedunit = s.showMFO.targetUnit;
            s.onChangeTarget(s.showMFO.target);
            if (s.showMFO.categoryId == 2) {
                document.getElementById("showcategory2").checked = true;

            }
            else {
                document.getElementById("showcategory2").checked = false;

            }
            if (s.showMFO.categoryId == 3) {
                document.getElementById("showcategory3").checked = true;
            }
            else {
                document.getElementById("showcategory3").checked = false;
            }
            console.log(s.updateMFO_allSI);
        }, function (err) {
            alert("error item");
        });
    }
    /*   ====================================================================================== UPDATE MFO, SI, Category ===================================================================================================*/
    s.updateMFO_allSIbyID = function (_mfoData) {
        console.log(" s.updateMFO_allSIbyID", _mfoData);
        $http.post('../SPMS_DPCR/MFO_updateMFO_allSI', { mfoData: _mfoData, standard: s.standard_show }).then(function (response) {
            if (response.data.status == 1) {
                s.assignStandard = [];
                s.cmfoes = [];
                loadData();
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
        }, function (err) {
            console.log(JSON.stringify(err));
        });
    }
    /*   ====================================================================================== SHOW STANDARD ===================================================================================================*/
    s.standardShow = {};
    s.standard_show = {};
    s.showStandard = function (getStandard, isVisible) {

        s.copyBtn = isVisible;

        $http.post('../SPMS_DPCR/MFO_Standard', { mfoindId: getStandard }).then(function (response) {
            s.standard_show = response.data;
            //s.standardShow = response.data;
            console.log(JSON.stringify(s.standardShow));

        }), function (error) {

        }
    }
    /*   ====================================================================================== TARGET COMPUTATION ======================================================================================*/
    /* s.target = 0;
     s.onChangeTarget = function (targetCount) {
 
         s.target = targetCount;
        // alert(s.target);
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
     }*/
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
        if (s.standard_show[2].quantity == 0) {
            s.onChangeTarget(null);
            s.newSI.target = null;
        }
        else {
            s.onChangeTarget(s.standard_show[2].quantity);
            s.newSI.target = s.standard_show[2].quantity;
        }

        s.addStandard = s.standard_show;
    }
    /*   ====================================================================================== UPDATE STANDARD ===================================================================================================*/

    s.updateStandard = function () {
        $http.post('../SPMS_DPCR/MFO_UpdateStandard', { objStandard: s.standard_show }).then(function (respponse) {
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
    /*   ====================================================================================== SHOW CHECKLIST MFO ===================================================================================================*/
    s.getChecklist = function (clData) {
        $http.post('../SPMS_DPCR/getCLstandard', { CLData: clData }).then(function (response) {
            s.clData = response.data.clmfo;
            console.log("clData", clData);

            console.log("targetUnit", clData.targetUnit);
            s.selectedunit = s.clData.targetUnit;
            s.clStandard = response.data.standard;
            s.onChangeTarget(s.clData.target);
            //  alert(JSON.stringify(response.data));
            //alert(s.updateMFO_allSI[0].categoryId); //getcatId


        }
        ), function (error) {

        }
    }
    /*   ====================================================================================== UPDATE CHECKLIST MFO ===================================================================================================*/
    s.updateCL = function (data) {

        $http.post('../SPMS_DPCR/updateCheckList', { CLData: data, CLStandard: s.clStandard }).then(function (response) {
            if (response.data.status == 1) {
                loadData();
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
    s.divisioncat;
    s.getdivision = function () {
        if (s.divisioncat != null) {
            s.divisioncat = undefined;
        }
        else {
            s.divisioncat = s.currentDivision;
        }

        //alert(s.divisioncat);
    }

    s.clr_assignStandard = function () {
        s.OPCRTableData = [{}];
        s.cmfoesdata = {};
        s.StandardPrOffc = {};
        s.assignStandard = [];
        s.assignCmfo = [];
        s.assignOpcrCmfo = [];
        loadData();
    }
    s.isVisibleStandard = false;
    s.standardVisibility = function () {
        s.isVisibleStandard = true;
    }

    s.addMFOclr = function () {
        s.selectedunit = "#";
        s.r5 = null; s.r4 = null; s.r3 = null; s.r2 = null; s.r1 = null; s.addStandard = [{}, {}, {}, {}, {}];
    }
    //========================== view standard =============================
    s.standardShow = [];
    s.viewStandard = function (data) {

        $http.post('../SPMS_DPCR/viewStandard', { TargetId: data }).then(function (response) {

            console.log("viewStandard", response.data);

            s.standardShow = response.data;
        }), function (error) {

        }
    }
});