app.controller('AppointmentContract', ['$scope', '$http', '$document', function (s, h, d) {

    s.empList = {};
    s.fundSourceList = {};
    s.appointeeList = {};
    s.data = {};

    s.warmBodyList = {};
    s.isLoading = false;
    loadInitData();

    s.bDisable = false;

    function loadInitData() {
        h.post('~/../../AppointmentNonPlantilla/HonorariumApptData').then(function (r) {
            if (r.data.status == "success") {
                s.fundSourceList = r.data.fSource;
                //s.apptNatureList = r.data.appNatureList;
            }
        });
    }


    s.openModalSelectEmp = function () {
        //validation
        if (s.data.appointmentName == undefined || s.data.appointmentName == "" || s.data.periodTo == undefined || s.data.fundSourceCode == undefined) {
            toastr["error"]("Please fill-up the required data!", "Opps!");
            return;
        }
        s.bDisable = true;
        if (s.empList.length == undefined) {
          
            h.post('~/../../AppointmentNonPlantilla/GetAppointeeAndPositionContract').then(function (r)
            {              
                if (r.data.status == "success") {                  
                    s.empList = r.data.empList;
                    s.positionList = r.data.positionList;
                    s.warmBodyList = r.data.warmBodyList;                  
                    angular.element('#openModalSelectEmp').modal('show');
                    s.bDisable = false;
                }
            });
        }
        else {
            angular.element('#openModalSelectEmp').modal('show');
            s.bDisable = false;
        }
    }

    //REFRESH MY LIST
    s.refreshDataList = function () {
        if (s.data.appointmentName == undefined || s.data.appointmentName == "" || s.data.periodTo == undefined || s.data.fundSourceCode == undefined) {
            toastr["error"]("Please fill-up the required data!", "Opps!");
            return;
        }
        s.isLoading = true;
        h.post('~/../../AppointmentNonPlantilla/GetAppointeeAndPositionHonorarium').then(function (r) {
            if (r.data.status == "success") {
                s.empList = r.data.empList;
                s.positionList = r.data.positionList;
                s.warmBodyList = r.data.warmBodyList;
                angular.element('#openModalSelectEmp').modal('show');
            }
        });
    }

    s.AddContractAppointee = function (data) {
        s.data.apptType = "07";
        s.bDisable = true;
        h.post('~/../../AppointmentNonPlantilla/AddAppointee', { data: data, app: s.data }).then(function (r) {
            if (r.data.status == "success") {
                s.empData.EIC = "";
                s.empData.positionCode = "";
                s.empData.warmBodyGroupCode = "";
                s.appointeeList = r.data.list;
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
        h.post('~/../../AppointmentNonPlantilla/RemoveEmployeeFromList/' + id).then(function (r) {
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

    s.submitContractAppointment = function (data) {

        //validation
        if (s.data.appointmentName == undefined || s.data.appointmentName == "" || s.data.periodFrom == undefined || s.data.periodTo == undefined || s.data.fundSourceCode == undefined) {
            toastr["error"]("Please fill-up required fields!", "Error!");
            return;
        }

        if (s.appointeeList.length >= 0) {
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
                    d.appType = "07";
                    if (s.isLoading == false) {
                        s.isLoading = true;
                        h.post('~/../../AppointmentNonPlantilla/SaveCasualAppointment', { data: d, periodF: f, periodT: t }).then(function (r) {
                            if (r.data.status == "success") {
                                s.apptCode = r.data.apptCode;
                                s.appointeeList = {};
                                s.data = {};
                                s.data.fundSourceCode = "";
                                swal("Saving Successful", "", "success");
                                s.isLoading = false;
                            }
                        });
                    }

                }

            });
        }


    }

}]);