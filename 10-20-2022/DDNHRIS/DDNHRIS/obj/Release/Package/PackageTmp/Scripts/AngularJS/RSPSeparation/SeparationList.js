app.controller('Separation', ['$scope', '$http', '$document', function (s, h, d) {

    s.separationList = {};
    s.separationItem = {};
    s.empList = {};
    s.empCount = 0;

    s.bDisable = false;
    s.loadingState = false;

    s.tabId = 0;
    s.groupTag = 2;

    s.sepCauseList = [{ value: 'RESIGNED', text: 'RESIGNED' }, { value: 'TRANSFERRED', text: 'TRANSFERRED' }, { value: 'RETIRED', text: 'RETIRED' }, { value: 'TERMINATED', text: 'TERMINATED' }, { value: 'END OF COTERM APPT', text: 'END OF COTERM APPT' }, { value: 'END OF TERM', text: 'END OF TERM' }, { value: 'DEATH', text: 'DEATH' }];


    s.empStatGroupType = [{ value: 'PLANTILLA', text: 'PLANTILLA' }, { value: 'NONPLANTILLA', text: 'NON-PLANTILLA' }];


    //LOAD DATA
    loadInitData();

    function loadInitData() {
        h.post('~/../../RSPSeparation/SeparationList').then(function (r) {
            if (r.data.status == "success") {
                s.separationList = r.data.list;
            }
        });
    }

    s.selectMonth = function (item) {
        s.separationItem = item;
    }

    s.tabSelect = function (id)
    {
        s.tabId = id;        
        if (id == 1) {
            loadForcast();
        }
    }

    s.forcastList = {};
    function loadForcast() {
        if (s.forcastList.length == undefined) {
            h.post('~/../../RSPSeparation/GetForecastData').then(function (r) {
                if (r.data.status == "success") {
                    s.forcastList = r.data.list;
                }
                s.bDisable = false;
            });
        }
    }

    s.modalSeparation = function () {        
        if (s.empCount == 0) {
            s.bDisable = true;
            h.post('~/../../RSPSeparation/EmployeeList').then(function (r) {
                if (r.data.status == "success") {
                    s.empList = r.data.list;
                    s.empCount = 0;
                    angular.element('#modalSeparation').modal('show');
                }
                s.bDisable = false;
            });
        }
        else {
            angular.element('#modalSeparation').modal('show');
        }       
    }
     
    //SEPARATION
    s.submitSeparation = function (data) {      
        if (data.EIC == null || data.EIC == "" || data.EIC == undefined || data.separationType == null || data.effectiveDate == null) {          
            return;
        }
        s.bDisable = true;
        h.post('~/../../RSPSeparation/SubmitSeparation', {data: data, tag: s.groupTag }).then(function (r) {
            if (r.data.status == "success") {
                loadInitData();
                angular.element('#modalSeparation').modal('hide');
            }
            s.bDisable = false;
        });
    }
    
    //EMPSTATGROUP
    s.onChangeStatusGroup = function (id) {
        s.loadingState = true;
        if (id == "PLANTILLA") {
            s.groupTag = 1;
        }
        else if (id == "NONPLANTILLA") {
            s.groupTag = 0;
        }
        s.loadingState = false;        
    }

    //FORECASTED      
    s.modalForecasted = function () {
        if (s.empCount == 0) {
            s.bDisable = true;
            h.post('~/../../RSPSeparation/EmployeeList').then(function (r) {
                if (r.data.status == "success") {
                    s.empList = r.data.list;
                    s.empCount = 1;
                    angular.element('#modalForCasting').modal('show');
                }
                s.bDisable = false;
            });
        }
        else {
            angular.element('#modalForCasting').modal('show');
        }
    }


    s.submitForcastedSep = function (data)
    {
        if (data.EIC == null || data.EIC == "" || data.EIC == undefined || data.separationType == null || data.effectiveDate == null) {
            return;
        }
        s.bDisable = true;
        h.post('~/../../RSPSeparation/SubmitForecast', { data: data}).then(function (r) {
            if (r.data.status == "success") {
                s.forcastList = r.data.list;
                angular.element('#modalForCasting').modal('hide');
                toastr["success"]("Saving successful!", "Good");
            }
            else {
                toastr["error"]("Please fill-up the required fields!", "Opps...");
            }
            s.bDisable = false;
        });
    }
    

    s.formatDate = function (date) {
        if (!date) {
            return '';
        }
        return (moment(date).format('MMMM DD, YYYY'));
    };
    
}]);