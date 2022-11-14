app.controller('NOSI', ['$scope', '$http', '$document', function (s, h, d) {

    s.nosiPending = {};
    s.nosiPrinted = {};
    s.pendingCount = 0;
    s.nosiItem = {};
    s.nosiEmp = {};
    s.bDisable = false;

    //LOAD DATA
    loadInitData();

    function loadInitData() {
        h.post('~/../../RSPUtility/NOSIInitData').then(function (r) {
            if (r.data.status == "success") {
                s.nosiPending = r.data.pending;
                s.pendingCount = s.nosiPending.length;
             
                s.nosiPrinted = r.data.posted;
                s.nosiItem = r.data.nosiItem;
            }
        });
    }

    s.selectMonth = function (item) {
        s.nosiItem = item;
    }
    
    s.formatDate = function (date) {
        if (!date) {
            return '';
        }
        return (moment(date).format('MMMM DD, YYYY'));
    };

    s.formatDate2 = function (date) {
        if (!date) {
            return '';
        }
        return (moment(date).format('MM/DD/YYYY'));
    };

    s.printSetup = function (data) {
        s.nosiEmp = data;
        s.bDisable = true;

        h.post('~/../../RSPUtility/NOSIItemData', { transcode: data.transCode }).then(function (r) {
            if (r.data.status == "success") {
                s.nosiEmp.salaryTo = r.data.nosiData.newSalary;
                s.nosiEmp.salaryAdd = r.data.nosiData.salaryAdd;
                angular.element('#modalNOSIPosting').modal('show');
            }
        });
        s.bDisable = false;
    }

    s.printPreview = function (id) {
       // alert("Print NOSI")
        s.bDisable = true;
        h.post('~/../../RSPUtility/PrintNOSI/' + id).then(function (r) {
            if (r.data.status == "success") {
                    angular.element('#modalNOSIPosting').modal('hide');
                    loadInitData();
                    toastr["success"]("NOSI CONFIRMED!", "Success");
                    window.open("../Reports/HRIS.aspx");   
                }
        });
        s.bDisable = false;
    }

    //CANCEL NOSI
    s.cancelNOSI = function (data) {
        s.nosiEmp = data;         
        s.bDisable = true;
        h.post('~/../../RSPUtility/CancelNOSI', { id: data.transCode }).then(function (r) {
            if (r.data.status == "success") {                
                loadInitData();
                toastr["warning"]("NOSI cacelled!", "Success");
            }
        });
        s.bDisable = false;
    }

}]);