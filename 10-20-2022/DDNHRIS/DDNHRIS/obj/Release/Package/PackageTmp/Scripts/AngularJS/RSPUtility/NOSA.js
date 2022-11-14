app.controller('NOSA', ['$scope', '$http', '$document', function (s, h, d) {


    s.nosaList = {};
    s.nosaPrinted = {};
    s.pendingCount = 0;
    s.nosaData = {};
    s.nosaEmp = {};
    s.bDisable = false;
    s.officeList = {};

    s.officeName = "PGO";
    s.office = {};
    s.nosaType = "";
    
    //LOAD DATA
    loadInitData();


    function loadInitData() {
        h.post('~/../../RSPUtility/NOSAInitData').then(function (r) {
            if (r.data.status == "success") {
                s.nosaList = r.data.list;
                s.pendingCount = r.data.pendingCount;
                s.officeList = r.data.deptList;
            }
        });
        s.office.officeName = "PGO";

    }

    s.modalNOSAData = function (data) {
        s.nosaType == "";
        s.bDisable = true;
        s.nosaData = data;
        angular.element('#modalNOSAPrint').modal('show');
        s.bDisable = false;
    }

    s.printPreview = function (data) {
        s.bDisable = true;      
        if (s.nosaType == "") {
            //plantilla
            h.post('~/../../RSPUtility/NOSAPrint', { id: data.EIC, code: data.NOSACode, nameTitle: data.fullNameTitle, prefixLastName: data.prefixLastName }).then(function (r) {
                if (r.data.status == "success") {
                    s.nosaList = r.data.list;
                    s.pendingCount = r.data.pendingCount;
                    s.officeList = r.data.deptList;
                    angular.element('#modalNOSAPrint').modal('hide');
                    window.open("../Reports/HRIS.aspx");
                    s.bDisable = false;
                }
                else {
                    s.bDisable = false;
                }
            });
        }
        else if (s.nosaType == "05") {
            //CASUAL            
            var datx = data;        
            h.post('~/../../RSPUtility/NOSACasualPrint', { data: datx }).then(function (r) {
                if (r.data.status == "success") {
                    //s.nosaList = r.data.list;
                    s.casualList = r.data.casualList;
                    //s.pendingCount = r.data.pendingCount;
                    // s.officeList = r.data.deptList;
                    angular.element('#modalNOSAPrint').modal('hide');
                    window.open("../Reports/HRIS.aspx");
                    s.bDisable = false;
                }
                else {
                    s.bDisable = false;
                }
            });
        }
    }

    s.selectOffice = function (data) {
        s.office = data;
        s.officeName = data.officeName;
    }

    s.postNosaToDB = function (data) {
        s.bDisable = true;
        h.post('~/../../RSPUtility/PostNosaToDB', { id: data.EIC, code: data.NOSACode }).then(function (r) {
            if (r.data.status == "success") {
                loadInitData();
                //s.nosaList = r.data.list;
                angular.element('#modalNOSAPrint').modal('hide');
                toastr["success"]("Saving successful!", "Success");                
                //s.pendingCount = r.data.pendingCount;
                //s.officeList = r.data.deptList; 
                s.bDisable = false;
            }
            else {
                s.bDisable = false;
            }
        });
    }

    //**************************************************************************************************************

    s.casualList = {};

    s.showNOSACasual = function () {
        s.bDisable = true;
        if (s.casualList.length == undefined) {
            h.post('~/../../RSPUtility/NOSACasualData').then(function (r) {
                if (r.data.status == "success") {
                    s.fundSourceData = r.data.fundSourceData;
                    s.casualList = r.data.casualList;
                }
                else {
                    s.bDisable = false;
                }
            });
        }
    }


    s.modalNOSACasualData = function (data) {       
        s.bDisable = true;
        s.nosaData = data;
        s.nosaType = "05"; //Ca
        angular.element('#modalNOSAPrint').modal('show');
        s.bDisable = false;
    }

    s.fundSourceList = {};
    s.selectFundSource = function () {

        if (s.fundSourceList.length == undefined)
        {
            h.post('~/../../RSPUtility/GetFundSourceList').then(function (r) {
                if (r.data.status == "success") {
                    //s.fundSourceData = r.data.fundSourceData;
                    s.fundSourceList = r.data.fundSourceList;
                    angular.element('#modalSeletFundSource').modal('show');
                }               
            });
        }
        else {
            angular.element('#modalSeletFundSource').modal('show');
        }
     
    }

    s.fundSourceData = {};
    s.fundSourceSelected = function (id) {      
        s.bDisable = true;
        h.post('~/../../RSPUtility/GetFundSourceCasual', {id: id}).then(function (r) {
            if (r.data.status == "success") {
                s.fundSourceData = r.data.fundSourceData;
                s.casualList = r.data.casualList;             
                angular.element('#modalSeletFundSource').modal('hide');
                s.bDisable = false;
            }
        });
    }

    //s.printPreview = function (data) { 
    //    s.bDisable = true;
    //    h.post('~/../../RSPUtility/NOSAPrint', { id: data.EIC, code: data.NOSACode, nameTitle: data.fullNameTitle, prefixLastName: data.prefixLastName }).then(function (r) {
    //        if (r.data.status == "success") {
    //            s.nosaList = r.data.list;
    //            s.pendingCount = r.data.pendingCount;
    //            s.officeList = r.data.deptList;
    //            angular.element('#modalNOSAPrint').modal('hide');
    //            window.open("../Reports/HRIS.aspx");
    //            s.bDisable = false;
    //        }
    //        else {
    //            s.bDisable = false;
    //        }
    //    });
    //}

}]);