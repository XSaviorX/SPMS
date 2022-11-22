
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

app.filter('currentdate', ['$filter', function ($filter) {
    return function () {
        return $filter('date')(new Date(), 'MMM d, y');
    };
}]);

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

app.controller("SPMS_IPCR", function ($scope, $http, filterFilter) {

    var s = $scope;

    s.tbl_ipcr = {};
    s.currentUser = "EMP131";
    s.s_month = 0;
    loadData();
    s.ipcrtemp = [];
    s.ipcr = [];
    s.otsData = {};
    s.show_standard = {};
    s.show_ots = {};
    s.mfoData = {};

    function loadData() {
        s.officeId = "OFFPHRMONZ3WT7D";
        s.curDiv = "DIVPHRMO0003";
        $http.post('../SPMS_IPCR/ipcr_getList', { officeId: s.officeId, UserId: s.currentUser, _divId: s.curDiv }).then(function (response) {
            //console.log("ipcr:", response.data.dpcr);
            //console.log("cl:", response.data.cldata);
            s.tbl_ipcr = response.data.dpcr;

            s.dpcrmfo = response.data.dpcr;
            console.log("dpcrmfo:", s.dpcrmfo);
            s.clmfo = response.data.cldata;
            s.list_ipcr = response.data.ipcr;
            s.list_ipcrwCat = response.data.ipcrwCat;
            s.coreTotal = response.data.coreTotal;
            s.suppTotal = response.data.suppTotal;
            console.log("list_ipcr:", s.list_ipcr);
            console.log(" s.clmfo:", s.clmfo);

            console.log("list_ipcrwCat:", s.list_ipcrwCat);
            angular.forEach(s.dpcrmfo, function (dpcr, keyfirst) {

                var hasCL = false;

                angular.forEach(s.clmfo, function (cl, key) {

                    if (dpcr.MFOId == cl.MFOId & dpcr.officeId == cl.officeId) {
                        s.ipcrtemp.push({ MFOId: dpcr.MFOId, MFO: dpcr.MFO, indicatorId: dpcr.indicatorId, indicator: dpcr.indicator, target: dpcr.target, targetId: dpcr.targetId, categoryId: dpcr.categoryId, description: dpcr.description, officeId: dpcr.officeId, tRemaining: dpcr.tRemaining, isCLroot: 1 });

                        s.ipcrtemp.push({ MFOId: cl.CLId, MFO: cl.CLDesc, indicatorId: cl.indicatorId, indicator: cl.indicator, target: cl.target, targetId: cl.targetId, categoryId: cl.categoryId, description: cl.description, officeId: cl.officeId, tRemaining: cl.tRemaining, isCL: 1 });
                        hasCL = true;
                    }
                });

                if (hasCL == false) {

                    s.ipcrtemp.push({ MFOId: dpcr.MFOId, MFO: dpcr.MFO, indicatorId: dpcr.indicatorId, indicator: dpcr.indicator, target: dpcr.target, targetId: dpcr.targetId, categoryId: dpcr.categoryId, description: dpcr.description, officeId: dpcr.officeId, tRemaining: dpcr.tRemaining });
                }
            });

            console.log("ipcrtemp", s.ipcrtemp);

            angular.forEach(s.ipcrtemp, function (temp, key) {
                var hasCommitted = false;
                angular.forEach(s.list_ipcr, function (ipcrAll, key) {
                    if (temp.MFOId == ipcrAll.i_MFOId & temp.indicatorId == ipcrAll.i_indicatorId & s.currentUser == ipcrAll.i_EIC) {
                        s.ipcr.push({ MFOId: temp.MFOId, MFO: temp.MFO, indicatorId: temp.indicatorId, indicator: temp.indicator, target: temp.target, targetId: temp.targetId, categoryId: temp.categoryId, description: temp.description, officeId: temp.officeId, tRemaining: temp.tRemaining, isCL: temp.isCL, isCLroot: temp.isCLroot, committed: ipcrAll.i_target, i_EIC: ipcrAll.i_EIC, status: true, committedTgtId: ipcrAll.targetId });
                        hasCommitted = true;
                    }
                });

                if (hasCommitted == false) {
                    s.ipcr.push({ MFOId: temp.MFOId, MFO: temp.MFO, indicatorId: temp.indicatorId, indicator: temp.indicator, target: temp.target, targetId: temp.targetId, categoryId: temp.categoryId, description: temp.description, officeId: temp.officeId, tRemaining: temp.tRemaining, isCL: temp.isCL, isCLroot: temp.isCLroot, committed: null, i_EIC: null, status: false });
                }
            });
            console.log("IPCR-MFO", s.ipcr);



        }), function (err) {
            alert("error item (ipcr_getList)");
        }
    }
    s.refreshModal = function () {
        s.clmfo = {};
        s.list_ipcr = {};
        s.list_ipcrwCat = {};
        s.tbl_ipcr = {};
        s.dpcrmfo = {};
        s.ipcrtemp = [];
        s.ipcr = [];
        loadData();
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
    //========================================================================================================= Click Committed ====================================================================================================================
    s.focus = false;
    s.committed;
    s.addminus = 0;
    s.o_committed = 0;
    s.inputCommitted = function () {

    }
    //========================================================================================================= Save IPCR ====================================================================================================================
    s.saveIPCR = function (data, _committed) {
        console.log("saveIPCR", data);
        console.log("input committed", _committed);
        // alert(data.i_EIC);

        $http.post('../SPMS_IPCR/SAVE_IPCR', { IPCRData: data, Committed: _committed, UserId: s.currentUser, EIC: data.i_EIC, CommittedTgtId: data.committedTgtId }).then(function (response) {
            console.log("obj", response.data.obj);
            if (response.data.data == 1) {
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
                //   alert(JSON.stringify(response.data.obj));
                // s.ipcrtemp = [];
                // s.ipcr = []
                s.refreshModal();
            }
        }), function (error) { }
    }

    s.resetval = function (data, index, remaining) {
        document.getElementById("reset" + "" + index).value = data;
        document.getElementById("tRemaining" + "" + index).value = remaining;
        /* document.getElementById("resetBtn" + "" + index).style.display = "none";*/
    }
    //========================================================================================================= Remove MFO in Modal ====================================================================================================================

    s.newUnassign = function (data) {
        console.log("remove", data.committedTgtId);

        if (data.committedTgtId != undefined) {
            $http.post('../SPMS_IPCR/REMOVE_IPCR', { IPCRData: data, UserId: s.currentUser, EIC: data.i_EIC, CommittedTgtId: data.committedTgtId }).then(function (response) {
                //  console.log("obj", response.data.obj);
                if (response.data.status == 1) {
                    // alert(JSON.stringify(response.data.remove))
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
                        title: 'Removed Successfully!'
                    })
                    //   alert(JSON.stringify(response.data.obj));
                    s.refreshModal();
                }
            }), function (error) { }
        }
        else {
            alert("Please try again!");
            s.refreshModal();

            console.log(" s.ipcr.committedTgtId", s.ipcr.committedTgtId);
        }

    }
    //========================================================================================================= Remove MFO in IPCR ====================================================================================================================

    s.removeSI = function (data) {
        console.log("removeSI", data);

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


                $http.post('../SPMS_IPCR/REMOVE_IPCRMAIN', { IPCRData: data, targetIdparent: data.targetIdparent }).then(function (response) {
                    //  console.log("obj", response.data.obj);
                    if (response.data.status == 1) {
                        // alert(JSON.stringify(response.data.remove))
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

                        Swal.fire(
                            'Removed!',
                            'The Success Indicator has been removed.',
                            'success'
                        );
                        s.refreshModal();
                    }
                }), function (error) { }

            }
        })





    }
    //========================================================================================================= Show users per mfo in modal ====================================================================================================================

    s.users = {};
    s.getUsers = function (data) {
        console.log("getUsers", data);
        $http.post('../SPMS_IPCR/SHOW_USERS', { IPCRData: data }).then(function (response) {
            if (response.data.status == 1) {
                s.users = response.data.users;
                console.log("s.users", s.users);

                //alert(JSON.stringify(response.data.users));

            }

        }), function (error) { }
    }

    s.categ = "hi";
    s.catChange = function (data) {
        alert(data);
        s.categ = data;


    }
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
    //========================================================================================================= ADD OTS ====================================================================================================================
    s.otstempdata = {};
    s.getData = function (data) {
        s.otstempdata = data;
        console.log("ipct_get", s.otstempdata);
        //IND230425PHRBKH57351
        getOTSStandard(data.i_indicatorId);
    }
    function getOTSStandard(indicatorId) {
        $http.post('../SPMS_IPCR/GET_OTSStandardData', { indicatorId: indicatorId }).then(function (response) {
            s.ots_standard = response.data;
            console.log(s.ots_standard);
        }), function (err) {
            alert("error item (getOTSStandard)");
        }

    }
    s.otsData = {};
    s.addOTS = function (otsdata, otsMFOdata, dateToday) {
        console.log("addOTS", otsdata, "\n", otsMFOdata, "\n", dateToday);

        $http.post('../SPMS_IPCR/ADD_OTS', { OTS: otsdata, MFOData: otsMFOdata, dateCreated: dateToday, cUser: s.currentUser }).then(function (response) {
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
        }), function (error) {

        }
    }
    //========================================================================================================= SHOW OTS ====================================================================================================================

    s.showOTS = function (data) {
        console.log("showOTS", data);
        $http.post('../SPMS_IPCR/SHOW_OTS', { OTS: data }).then(function (response) {
            if (response.data.status == 1) {
                s.s_month = response.data.curMonth;
                s.show_ots = response.data.ots;
                s.mfoData = response.data.mfoData;
                console.log("getOTS", s.mfoData);

            }

        }), function (error) {

        }
    }


    //convert date
    s.convertToDate = function (stringDate) {
        var dateOut = new Date(stringDate);
        dateOut.setDate(dateOut.getDate() + 1);
        return dateOut;
    };
    var tempdate = "";
    s.formatDateToString = function (stringDate) {
        tempdate = stringDate;

        var returnDate = tempdate.substring(6, 19);
        return (returnDate);
    }

    //======================= AUTO COMPUTE FOR TOTALS AND GET STANDARD EQUIVALENT =================
    s.total_qty = 0;
    s.equiv_qty = 0;
    s.equiv_qly = 0;
    s.equiv_time = 0;
    var subTotal_qty = 0;

    s.getTotalQty = function (tempQty) {
        var temp = Number(tempQty);
        subTotal_qty = subTotal_qty + temp;
        s.total_qty = subTotal_qty;
        console.log("total qty: ", s.total_qty);
    }
    //var countlimt = 0;
    //s.getStandardEquivalent = function (_MFOdata, totalVal, increm, limit) {
    //    countlimt = countlimt + increm;
    //    if (countlimt == limit) {
    //        $http.post('../SPMS_IPCR/get_StandardQTYEquiv', { MFOdata: _MFOdata, qtyVal: totalVal }).then(function (response) {
    //            if (response.data != null) {
    //                s.equiv_qty = response.data;
    //            }
    //            console.log("this israting: ", s.equiv_qty);
    //            countlimt = 0;
    //        }), function (error) { }
    //    }
    //}

    s.getStandard = function (_targetId) {
        $http.post('../SPMS_IPCR/getStandard', { targetId: _targetId }).then(function (response) {
            if (response.data != null) {
                s.show_standard = response.data;
            }
        }), function (error) { }
    }

    s.setDefault = function () {
        subTotal_qty = 0;
        s.total_qty = 0;
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

    //=================== PRINT =========================
    s.prt_ipcrtarget = function () {

        //hr
        s.officeId = "OFFPHRMONZ3WT7D";
        s.currentU = "EMP131";

        //pbo
        //s.officeId = "OFFPBOEZ7SC4ZA9";

        $http.post('../SPMS_IPCR/cookiestarget', { officeId: s.officeId, EIC: s.currentU }).then(function (responses) {
            console.log("ds: " + JSON.stringify(responses.data));
            window.open('../Reports/SPMS/IPCR_TARGET.aspx?type=Target');
        }), function (err) {
            alert("error item (getPerformanceData)");
        }
    }

    s.prt_ipcrstndrd = function () {
        //hr
        s.officeId = "OFFPHRMONZ3WT7D";

        //pbo
        //s.officeId = "OFFPBOEZ7SC4ZA9";

        $http.post('../SPMS_IPCR/stndCookies', { officeId: s.officeId, EIC: s.currentUser }).then(function (responses) {
            console.log("ds: " + JSON.stringify(responses.data));
            window.open('../Reports/SPMS/IPCR_STANDARD.aspx?type=stnd');
        }), function (err) {
            alert("error item (getPerformanceData)");
        }
    }

    s.prt_actual = function () {
        //hr
        s.officeId = "OFFPHRMONZ3WT7D";
        s.currentU = "EMP131";

        //pbo
        //s.officeId = "OFFPBOEZ7SC4ZA9";

        $http.post('../SPMS_IPCR/actualCookies', { officeId: s.officeId, EIC: s.currentU }).then(function (responses) {
            console.log("ds: " + JSON.stringify(responses.data));
            window.open('../Reports/SPMS/IPCR_ACTUAL.aspx?type=actual');
        }), function (err) {
            alert("error item (getPerformanceData)");
        }
    }

    s.prt_mpor = function (_monthId,_year,_semester) {
        //hr
        s.officeId = "OFFPHRMONZ3WT7D";
        s.currentU = "EMP131";

        //pbo
        //s.officeId = "OFFPBOEZ7SC4ZA9";

        $http.post('../SPMS_IPCR/mporCookies', { officeId: s.officeId, EIC: s.currentU, monthId: _monthId, month: s.getMonthString(_monthId) , year: _year, semester: _semester}).then(function (responses) {
            console.log("mpor: " + JSON.stringify(responses.data));
            window.open('../Reports/SPMS/IPCR_MPOR.aspx?type=mpor');
        }), function (err) {
            alert("error item (getPerformanceData)");
        }
    }

    s.getCurDate = function () {
        document.getElementById('input_date').valueAsDate = new Date();
    }

    // ================= MPOR ==========================
    const d = new Date();
    s.month = Number(d.getMonth()) + 1;
    s.mporData = {};
    s.loadDataMPOR = function (_month) {
        var _curMonth = Number(_month);
        console.log("month :", _month);
        $http.post('../SPMS_IPCR/mpor_getData', { officeId: s.officeId, EIC: s.currentUser, _curMonth: _curMonth }).then(function (response) {
            if (response.data != null) {
                s.mporData = response.data;
                console.log("MPOR: ", s.mporData);
            }
        }), function (err) {
            alert("error item (ipcr_getList)");
        }
    }

    s.isVisible = function (data) {
        var res = false;
        console.log("this is: ", data);
        if (data == 2) {
            res = true;
        }

        return res;
    }

    s.showSubmitButton = function (data) {
        var res = true;
        console.log("this is: ", data);
        if (data == 2) {
            res = false;
        }

        return res;
    }

    s.submitRequest = function (data) {
        console.log("User: ", s.currentUser, " + ", data);
        $http.post('../SPMS_IPCR/ipcr_submitRequest', { _reqId: data, userId: s.currentUser }).then(function (response) {
            if (response.data == 1) {
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
                    title: 'Submit Request Successfully!'
                })

            } else {
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
                    title: 'Already has a request pending'
                })
            }
        }), function (err) {
            alert("error item (ipcr_getList)");
        }
    }
    s.requestsIPCR = {};
    s.getListRequest = function () {

        $http.post('../SPMS_IPCR/ipcr_getListRequest').then(function (response) {
            if (response.data != null) {
                s.requestsIPCR = response.data;
                console.log("datas: ", s.requestsIPCR);
            }
        }), function (err) {
            alert("error item (ipcr_getList)");
        }
    }

    s.getStatus = function (data) {
        var returnText = "";
        if (data == 0) {
            returnText = "PENDING";
        } else if (data == 1) {
            returnText = "RECOMMENDED";
        } else if (data == 2) {
            returnText = "APPROVED";
        }

        console.log("returntext: ", returnText);
        return returnText;
    }

    s.ipcr_ListItems = {};
    s.getList = function (r_ipcrId) {

        $http.post('../SPMS_IPCR/req_ipcr_getList', { _ipcrId: r_ipcrId }).then(function (response) {
            if (response.data != null) {
                s.ipcr_ListItems = response.data;
            }
        }), function (err) {
            alert("error item (ipcr_getList)");
        }
    }

    s.getSem = function (sem) {
        var returnSem = "";
        if (sem == 1) {
            returnSem = "First";
        } else if (sem == 2) {
            returnSem = "Second";
        }
        return returnSem;
    }

    s.rejectRequest = function (_RipcrId, _remarks) {
        $http.post('../SPMS_IPCR/req_reject', { _ipcrId: _RipcrId, _rmarks: _remarks }).then(function (response) {
            if (response.data != null) {
                s.ipcr_ListItems = response.data;
            }
        }), function (err) {
            alert("error item (ipcr_getList)");
        }
    }
    // totals variables
    s.stratTotal = 0.0;
    s.coreTotal = 0.0;
    s.suppTotal = 0.0;

    s.getAverageRate = function (data) {

        var averageRate = 0;
        var temp = data.i_Rquantity + data.i_Rquality + data.i_Rtimeliness;

        if (temp > 0) {
            averageRate = temp / 3;
            averageRate = averageRate.toFixed(2);
        }

        return averageRate;
    }

    s.getCoreTotal = function () {

        s.coreTotal = (Number(s.coreTotal).toFixed(2)).toString();
        return s.coreTotal;
    }

    s.getSuppTotal = function () {

        s.suppTotal = (Number(s.suppTotal).toFixed(2)).toString();
        return s.suppTotal;
    }

    s.updateRequest = function (data, _status) {
        console.log("updateRequestData: ", data);
        $http.post('../SPMS_IPCR/updateRequestStatus', { data: data, status: _status}).then(function (response) {
            if (response.data == 1 || response.data == 2) {
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
                    title: 'Update Request Status Successfully!'
                })

            } else if (response.data == 3) {
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
                    title: 'Cancelled Request Successfully!'
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
                    title: 'No changes were made during the updating of request status'
                })
            }
        }), function (err) {
            alert("error item (ipcr_getList)");
        }

    }

    s.smporData = {};
    s.loadDataSMPOR = function (year,sem) {
        $http.post('../SPMS_IPCR/smpor_getData', { EIC: s.currentUser, _year: year  ,_sem: sem }).then(function (response) {
            if (response.data != null) {
                s.smporData = response.data;
                console.log("SMPOR: ", s.smporData);
            }
        }), function (err) {
            alert("error item (smpor_getList)");
        }
    }

    s.checkStatus = function (data) {
        var returnRes = false;

        if (data == 2) {
            returnRes = true;
        }

        return returnRes;
    }

    s.checkNull = function (data) {
        var returnRes = false;

        if (data != '' && data != null) {
            returnRes = true;
        }

        return returnRes;
    }

    s.getMonthString = function (monthId) {
        var returnMonthString = '';

        if (monthId == 1) {
            returnMonthString = 'January';
        } else if (monthId == 2) {
            returnMonthString = 'February';
        } else if (monthId == 3) {
            returnMonthString = 'March';
        } else if (monthId == 4) {
            returnMonthString = 'April';
        } else if (monthId == 5) {
            returnMonthString = 'May';
        } else if (monthId == 6) {
            returnMonthString = 'June';
        } else if (monthId == 7) {
            returnMonthString = 'July';
        } else if (monthId == 8) {
            returnMonthString = 'August';
        } else if (monthId == 9) {
            returnMonthString = 'September';
        } else if (monthId == 10) {
            returnMonthString = 'October';
        } else if (monthId == 11) {
            returnMonthString = 'November';
        } else if (monthId == 12) {
            returnMonthString = 'December';
        }

        return returnMonthString;
    }

    s.tgt_indDetails = {};
    s.tempData = {};
    s.tgt_indStandardDetails = {};
    s.getIndicatorDetails = function (_indId, _targtId) {
        //alert(_targtId);
        $http.post('../SPMS_IPCR/MFO_getIndicatorDetails', { indId: _indId, targtId: _targtId }).then(function (response) {
            s.tgt_indDetails = response.data.indDetails;
            s.tgt_indStandardDetails = response.data.standardDatas;
            console.log("here: ", s.tgt_indDetails);
        }, function (err) {
            alert("ERROR: GetIndicatorsDetails");
        });
    }
})