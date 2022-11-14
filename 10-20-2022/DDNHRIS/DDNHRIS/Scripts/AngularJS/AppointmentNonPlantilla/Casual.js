app.controller("AppointmentCasual", function ($scope, $http) {


    var s = $scope;
    var h = $http;
    s.empList = {};
    s.positionList = {};
    s.subPositionList = {};
    s.appointeeList = {};
    s.apptNatureList = {};
    s.data = {};

    s.warmBodyList = {};
    s.apptCode = "";
    s.empData = {};
    
    s.fundSourceList = {};
    s.isLoading = false;
    s.bDisable = false;

    s.hazardList = [{ hazardCode: 0, hazardName: '(NONE)' }, { hazardCode: 1, hazardName: 'Health Workder' }, { hazardCode: 2, hazardName: 'Social Workder' }];

    loadEmployee();

    function loadEmployee() {   
        $http.post('~/../../AppointmentNonPlantilla/CasualApptData').then(function (r) {
            if (r.data.status == "success") {
                s.fundSourceList = r.data.fSource;
                s.apptNatureList = r.data.appNatureList;
            }
        });
    }

    s.openModalSelectEmp = function () {
        s.bDisable = true;
        if (s.data.appointmentName == undefined || s.data.appointmentName == "" ||   s.data.periodTo == undefined || s.data.fundSourceCode == undefined || s.data.appNatureCode == undefined) {
            toastr["error"]("Please fill-up the required data!", "Opps!");
            s.bDisable = false;
            return;
        }
        s.empData.hazardCode = 0;      
        if (s.empList.length == undefined) {
            $http.post('~/../../AppointmentNonPlantilla/GetAppointeeAndPosition').then(function (r) {
                if (r.data.status == "success") {                
                    s.empList = r.data.empList;
                    s.positionList = r.data.positionList;
                    s.subPositionList = r.data.subPositionList;
                    s.warmBodyList = r.data.warmBodyList;
                    angular.element('#openModalSelectEmp').modal('show');
                    s.bDisable = false;
                }
            });
        }
        else { angular.element('#openModalSelectEmp').modal('show'); s.bDisable = false; }        
    }

    //REFRESH MY LIST
    s.refreshDataList = function () {
        s.isLoading = true;
        s.bDisable = true;
        $http.post('~/../../AppointmentNonPlantilla/ReloadEmployeeList').then(function (r) {
            if (r.data.status == "success") {
                s.empList = r.data.list;
                //s.positionList = r.data.positionList;
                //s.subPositionList = r.data.subPositionList;
                //s.warmBodyList = r.data.warmBodyList;                
                s.isLoading = false;
                s.bDisable = false;
            }
        });

    }
   
    s.AddAppointee = function (data) {
        s.data.apptType = "05";
        s.data.hazardCode = data.hazardCode;       
        s.bDisable = true;
        $http.post('~/../../AppointmentNonPlantilla/AddAppointee', { data: data, app: s.data }).then(function (r) {
            if (r.data.status == "success") {
                s.empData.EIC = "";
                s.empData.positionCode = "";
                s.empData.subPositionCode = "";
                s.empData.warmBodyGroupCode = "";               
                s.appointeeList = r.data.list;
                s.empData.hazardCode = 0;
                toastr["success"]("New appointee added!", "Good");
                s.bDisable = false;
            }
            else {
                toastr["error"](r.data.status, "Error!");
                s.bDisable = false;
            }
        });
    }
    
    s.deleteEmployee = function (id) {
        s.bDisable = true;
        $http.post('~/../../AppointmentNonPlantilla/RemoveEmployeeFromList/' + id).then(function (r) {
            if (r.data.status == "success") {
                s.appointeeList = r.data.list;
                toastr["info"]("Removing successful!", "Success");
                s.bDisable = false;
            }
            else {
                toastr["error"](r.data.status, "Error!");
                s.bDisable = false;
            }
        });
    }
    
    s.PrintPreview = function () {
        var id = s.apptCode;
        $http.post('~/../../AppointmentNonPlantilla/PrintPreviewSetup/' + id).then(function (r) {
            if (r.data.status == "success") {
                angular.element('#modalPrintPreview').modal('hide');
                window.open("../Reports/HRIS.aspx");
            }
        });
    }
    
    s.submitAppointment = function (data) {
        if (s.data.appointmentName == undefined || s.data.appointmentName == "" || s.data.periodTo == undefined || s.data.fundSourceCode == undefined || s.data.appNatureCode == undefined) {
            toastr["error"]("Please fill-up required fields!", "Error!");
            return;
        }
        
        if (s.appointeeList.length >= 0 ) {
            swal({
                title: "Are you sure?",
                text: "Submit casual appointment.",
                type: "warning",
                showCancelButton: true,
                confirmButtonColor: "#2196f3",
                confirmButtonText: "Yes",
                cancelButtonText: "Cancel",
                closeOnConfirm: false
            }, function (value) {

                if (value == true) {
                    var f = data.periodFrom;
                    var t = data.periodTo;
                    var d = data;
                    d.appType = "05";

                    if (s.isLoading == false) {
                        s.isLoading = true;
                        s.bDisable = true;
                        h.post('~/../../AppointmentNonPlantilla/SaveCasualAppointment', { data: d, periodF: f, periodT: t }).then(function (r) {
                            if (r.data.status == "success") {
                                s.apptCode = r.data.apptCode;
                                s.appointeeList = {};
                                s.data = {};
                                s.data.fundSourceCode = "";
                                swal("Saving Successful", "", "success");
                                s.isLoading = false;
                                s.bDisable = false;
                            }
                        });
                    }
                    else {
                        toastr["error"]("Unable to save data!", "Error!");
                    }
                    
                }

            });
        }

    }



});

