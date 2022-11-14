
app.controller('AppointmentJobOrder', ['$scope', '$http', '$document', function (s, h, d) {


    s.empList = {};
    s.fundSourceList = {};
    s.subPositionList = {};
    s.data = {};

    s.empData = {};
    s.appointeeList = {};
    s.warmBodyList = {};
    s.isLoading = false;
    s.bDisable = false;
    
    loadEmployee();

    function loadEmployee() {
        h.post('~/../../AppointmentNonPlantilla/CasualApptData').then(function (r) {
            if (r.data.status == "success") {
                s.fundSourceList = r.data.fSource;
            }
        });
    }

    s.openModalSelectEmp = function (data) {                
        if (s.data.appointmentName == undefined || s.data.appointmentName == "" || s.data.periodTo == undefined || s.data.fundSourceCode == undefined) { 
            toastr["error"]("Please fill-up the required data!", "Opps!");
            return;
        }
          
        if (s.empList.length == undefined) {
            h.post('~/../../AppointmentNonPlantilla/GetAppointeeAndPosition').then(function (r) {              
                if (r.data.status == "success") {                  
                    s.empList = r.data.empList;
                    s.positionList = r.data.positionList;
                    s.subPositionList = r.data.subPositionList;
                    s.warmBodyList = r.data.warmBodyList;
                    angular.element('#openModalSelectEmp').modal('show');
                }
            });
        }
        else { angular.element('#openModalSelectEmp').modal('show'); }
    }


    //REFRESH MY LIST
    s.refreshDataList = function () {
        if (s.data.appointmentName == undefined || s.data.appointmentName == "" ||  s.data.periodTo == undefined || s.data.fundSourceCode == undefined) {
            toastr["error"]("Please fill-up the required data!", "Opps!");
            return;
        }
        s.isLoading = true;
        h.post('~/../../AppointmentNonPlantilla/GetAppointeeAndPosition').then(function (r) {
            if (r.data.status == "success") {
                s.empList = r.data.empList;
                s.positionList = r.data.positionList;
                s.subPositionList = r.data.subPositionList;
                s.warmBodyList = r.data.warmBodyList;
                angular.element('#openModalSelectEmp').modal('show');
            }
        });
    }


    s.AddAppointee = function (data) {
        s.bDisable = true;
        s.data.apptType = "06";
        h.post('~/../../AppointmentNonPlantilla/AddAppointee', { data: data, app: s.data }).then(function (r) {
            if (r.data.status == "success") {
                s.empData.EIC = "";
                s.empData.positionCode = "";
                s.empData.subPositionCode = "";
                s.empData.warmBodyGroupCode = "";
                s.appointeeList = r.data.list;
                s.bDisable = false;
                toastr["success"]("New appointee added!", "Good");
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

    s.submitJobOrderAppointment = function (data) {
            
        if (s.data.appointmentName == undefined || s.data.appointmentName == "" || s.data.periodTo == undefined || s.data.fundSourceCode == undefined) {
            toastr["error"]("Please fill-up required fields!", "Error!");
            return;
        }
        s.bDisable = true;
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
                    d.appType = "06";
                
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
                                s.bDisable = false;
                            }
                        });
                    }
                }
            });
        }
       
        //var f = data.periodFrom;
        //var t = data.periodTo;
        //var d = data
        //d.appType = "06";

        //h.post('~/../../AppointmentNonPlantilla/SaveCasualAppointment', { data: d, periodF: f, periodT: t }).then(function (r) {
        //    if (r.data.status == "success") {
        //        s.apptCode = r.data.apptCode;
        //        s.appointeeList = {};
        //        s.data = {};
        //        s.data.fundSourceCode = "";
        //        angular.element('#modalPrintPreview').modal('show');
        //    }
        //});
             
    }


    s.PrintPreview = function () {
        var id = s.apptCode;
        h.post('~/../../AppointmentNonPlantilla/PrintPreviewSetup/' + id).then(function (r) {
            if (r.data.status == "success") {
                angular.element('#modalPrintPreview').modal('hide');
                window.open("../Reports/HRIS.aspx");
            }
        });
    }

}]);