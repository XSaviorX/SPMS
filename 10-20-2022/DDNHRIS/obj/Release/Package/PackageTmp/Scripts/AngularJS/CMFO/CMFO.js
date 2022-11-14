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
app.controller("CMFO", function ($scope, $http, filterFilter) {
    var s = $scope;
    s.cmfoes = {};
    s.cmfoesPerOffice = [{}];
    s.newIndicator = [{}];
    s.newCMFO = {};
    s.cmfoestoModal = {}; 
    s.addPerformance = [{}, {}, {}, {}, {}];
    loadData();

    function loadData() {
        $http.post('../CMFO/CMFO_getperOffice', { officeID: 'OFFPBOEZ7SC4ZA9'}).then(function (response) {
            console.log(response.data)
            s.cmfoesPerOffice = response.data;
        }, function (err) {
            alert("error item");
        });

        $http.post('../CMFO/GetPerformance').then(function (responses) {
            //alert(JSON.stringify(response.data) + "\n" + data.MFOId)

            s.performanceData = responses.data;
            console.log(s.performanceData);
            if (s.performanceData.length === 0) {
                s.performanceData = [{}, {}, {}, {}, {}];
                alert("dsadas");
            }
        }), function (err) {
            alert("error item (getPerformanceData)");
            }
        
    }
    s.getCMFO = function () {
        $http.get('../CMFO/CMFO_get').then(function (response) {
            console.log(response.data)
            s.cmfoes = response.data;

        }, function (err) {
            alert("error item");
        });
    }
    s.addIndicatorField = function () {
        s.newIndicator.push({});
        s.cmfoestoModal.push({});
    }
    s.insertCMFO = function (_CMFODesc) {
        $http.post('../CMFO/CMFO_insert', { _CMFO: _CMFODesc }).then(function (response) {

            /*  alert(response.data.status);*/
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
            s.insertCMFOInd(response.data.status);
        }), function (err) {
            alert("error item");
        }
    }
    //===================================================================
    s.updateMFO_allSIbyID = function (data) {
        var _mfodesc = $('#updateAll_MFODesc').val();
        $http.post('../CMFO/MFO_updateMFO_allSI', { _cmfoindicators: data, _mfoDesc: _mfodesc }).then(function (response) {
            if (response.data.status != '') {
                /*  alert("count: " + response.data.status);*/
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
        }, function (err) {
            alert("error item");
        });
    }

    //===================================================================
    //CMFO SIs Insert Function
    s.insertCMFOInd = function (_CMFO_id) {
        console.log(s.newIndicator);
        $http.post('../CMFO/CMFO_SIinsert', { indicators: s.newIndicator, CMFO_ID: _CMFO_id }).then(function (response) {

            //alert(response.data.status);
            loadData();

        }), function (err) {
            alert("error item");
        }
    }

    s.editCmfoIndIdToModal = function (data)
    {
        //alert(JSON.stringify(data))
        $http.post('../CMFO/CMFOInd_edit', { CMFOId: data }).then(function (response) {
            s.cmfoestoModal = response.data;
            //alert(JSON.stringify(s.cmfoestoModal))

        }), function (err) {
            alert("error");
        }

    }

    s.getCmfoIndIdToModal = function (performanceId, _officeId) {
       /* alert(JSON.stringify(_officeId));*/
        if (_officeId !== "OFFPBOEZ7SC4ZA9")
        {
            document.getElementById('addPrfmc').style.visibility = 'hidden';
        }
        else {
            document.getElementById('addPrfmc').style.visibility = 'visible';
        }
        $http.post('../CMFO/GetPerformancePer', { performanceId: performanceId }).then(function (response) {
           
            s.result = response.data;
            //    /*alert(JSON.stringify(response.data.data));*/
            if (s.result.length === 0) {
               
                s.addPerformance = [{}, {}, {}, {}, {}];
                s.getPfmcID = performanceId;
              /*  alert(s.getPfmcID);*/

            }
            else {
                //alert(s.result);
                s.addPerformance = s.result;
                s.getPfmcID = performanceId;

            }

        }), function (error) {
            //    alert("Error");
        }

    }

    // Insert | Update Performance
    s.add_Performance = function () {
        //alert(JSON.stringify(s.addPerformance));
        s.button = "Saving data ... "
        $http.post('../CMFO/AddPerformance', { tCMFOPerformance: s.addPerformance }).then(function (response) {

            if (response.data.isSaved == "success") {
                alert(response.data.isSaved);
                s.button = "Save changes";

               /* s.getPerformancePer();*/
            }
            if (response.data.isSaved == "updated") {
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
               /* alert("Data Updated!");*/
                s.button = "Save changes";
            //    s.getPerformancePer();
            }
            loadData();


        }), function (error) {
            alert("Error");
        }

    }
  
    s.rowCount = function (_mfoID, data) {
        s.counts = 0;
        for (var ind = 0; ind < data.length; ind++) {
            if (_mfoID == data[ind].MFOId) {
                s.counts++;
            }
        }
        s.counts = s.counts;
        return s.counts;
    }


    /* FOR ASSIGNING MFO*/

    s.vals = 0;
    s.valss = 0;
    s.text = 'REMOVE';
    s.firstLoad = 0;

    s.returnSliderStatuss = function (status) {
    
        /* alert(JSON.stringify(status))*/
        console.log("assign - " + JSON.stringify(status));
        s.try = status;
        s.vals = document.getElementById("i" + status.indicatorId + status.officeId).value;
        //alert(JSON.stringify(status.officeId));
        //console.log(s.vals);
        if (s.vals == '1' && status.officeId == "OFFPBOEZ7SC4ZA9") {
            
            s.text = 'ADD';
            s.vals = 0;
            $('#' + status.officeId + status.indicatorId).removeClass('btn-warning');
            $('#' + status.officeId + status.indicatorId).addClass('btn-primary');
        } else {
            s.text = 'REMOVE';
            s.vals = 1;
            $('#' + status.officeId + status.indicatorId).removeClass('btn-primary');
            $('#' + status.officeId + status.indicatorId).addClass('btn-warning');
        }
        s.firstLoad = 1;
        document.getElementById("i" + status.indicatorId + status.officeId).value = s.vals;
        document.getElementById("" + status.officeId + status.indicatorId).innerHTML = s.text;
        status.isActive = document.getElementById("i" + status.indicatorId + status.officeId).value = s.vals;

        $http.post('../CMFO/updateAssigned', { ndata: status }).then(function (responses) {
            //alert(JSON.stringify(response.data) + "\n" + data.MFOId)
            s.uAssignedMFO_ndata = responses.data;
            console.log(s.uAssignedMFO_ndata);
           
            if (responses.data.isSaved == "1") {
                const Toast = Swal.mixin({
                    toast: true,
                    position: 'top-end',
                    showConfirmButton: false,
                    timer: 3000,
                    iconColor: '#FFFFFF',
                    color: '#FFFFFF',
                    timerProgressBar: true,
                    background: '#EE3D17',
                    didOpen: (toast) => {
                        toast.addEventListener('mouseenter', Swal.stopTimer)
                        toast.addEventListener('mouseleave', Swal.resumeTimer)
                    }
                })

                Toast.fire({
                    icon: 'error',
                    title: 'CMFO and Success Indicator is already exist in your data.'
                })
                s.text = 'ADD';
                s.vals = 0;
                $('#' + status.officeId + status.indicatorId).removeClass('btn-warning');
                $('#' + status.officeId + status.indicatorId).addClass('btn-primary');
                document.getElementById("i" + status.indicatorId + status.officeId).value = s.vals;
                document.getElementById("" + status.officeId + status.indicatorId).innerHTML = s.text;
            }
            else {
                loadData();
                var bg_colorPopup = '#24a0ed ';
                var statusSave = "Successfully Added!";

                if (responses.data.isSaved == "3") {

                    bg_colorPopup = '#ffc107'; //removed
                    statusSave = "Successfully Removed!";
                }
                else
                {
                }
                const Toast = Swal.mixin({
                    toast: true,
                    position: 'top-end',
                    showConfirmButton: false,
                    timer: 3000,
                    iconColor: '#FFFFFF',
                    color: '#FFFFFF',
                    timerProgressBar: true,
                    background: bg_colorPopup,
                    didOpen: (toast) => {
                        toast.addEventListener('mouseenter', Swal.stopTimer)
                        toast.addEventListener('mouseleave', Swal.resumeTimer)
                    }
                })

                Toast.fire({
                    icon: 'success',
                    title: statusSave
                })
            }
        }), function (err) {
            alert("error item (updateAssigned)");
        }
    }

    s.isactiveStatus = function (data) {

        s.valss = data.isActive;
        if (s.valss == '1' && data.officeId == "OFFPBOEZ7SC4ZA9") {
            s.text = 'REMOVE';
            if (s.firstLoad == 0) {
                $('#' + data.officeId + data.indicatorId).removeClass('btn-primary');
                $('#' + data.officeId + data.indicatorId).addClass('btn-warning');
            }
        } else {

            if (data.officeId !== "OFFPBOEZ7SC4ZA9") {
                s.text = 'ADD';
               
                $('#' + data.officeId + data.indicatorId).removeClass('btn-warning');
                    $('#' + data.officeId + data.indicatorId).addClass('btn-primary');
                    // console.log('dasdsad');
                
            }
            else {
                s.text = 'ADD';
                if (s.firstLoad == 0) {
                    $('#' + data.officeId + data.indicatorId).removeClass('btn-warning');
                    $('#' + data.officeId + data.indicatorId).addClass('btn-primary');
                    // console.log('dasdsad');
                }
            }
           
        }
        return s.text;
    }
    s.classs = "";
    s.buttonColr = function (isActive) {

        if (isActive == "1") {
            s.classs = "btn btn-warning";
        } else if (isActive == "0") {
            s.classs = "btn btn-primary";
        }
        return s.classs;
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
                $http.post('../CMFO/removeIndicator', { indicatorId: _indicatorId }).then(function (response) {

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

   
})