app.controller('AppointmentViewData', ['$scope', '$http', '$document', function (s, h, d) {

    s.apptData = {};
    s.appointeeList = {};
    s.subPositionList = {};
    s.empList = {};
    s.formData = {};

    s.psList = {};
    s.bDisable = false;
    s.isLoading = false;
    s.selectTag = 0;
    s.workGroupList = {};
    s.workGroupCode = [];
    s.tmpWorkGroupCode = "";

    s.empStatCode = "";
    s.postingTag = 1;
    s.raiList = {};


    s.isReadOnly = true;
    s.hazardList = [{ hazardCode: 0, hazardName: '(NONE)' }, { hazardCode: 1, hazardName: 'Health Workder' }, { hazardCode: 2, hazardName: 'Social Workder' }];
    loadInitData();

    s.empData = {};

    function loadInitData() {
        h.post('~/../../AppointmentNonPlantilla/ViewDetailInitData').then(function (r) {
            if (r.data.status == "success") {
                s.apptData = r.data.apptData;
                s.appointeeList = r.data.appointeeList;
                s.workGroupList = r.data.workGroupList;               
            }
        });
    }

    s.selectAppointee = function () {
        s.empData.hazardCode = 0;
        if (s.empList.length == undefined) {        
            h.post('~/../../AppointmentNonPlantilla/GetAppointeeAndPositionByEmpStat', { code: s.apptData.appointmentCode }).then(function (r) {
                if (r.data.status == "success") {
                    s.selectTag = r.data.tag;
                    s.empList = r.data.empList;
                    s.positionList = r.data.positionList;
                    s.warmBodyList = r.data.warmBodyList;
                    if (s.selectTag == 1) {
                        s.subPositionList = r.data.subPositionList;
                    }                   
                    angular.element('#openModalSelectEmp').modal('show');
                }
            });
        }
        else { angular.element('#openModalSelectEmp').modal('show'); }
    }


    //REFRESH MY LIST
    //s.refreshDataList = function () {
    //    s.bDisable = true;
    //    h.post('~/../../AppointmentNonPlantilla/GetAppointeeAndPositionByEmpStat', { code: s.apptData.appointmentCode }).then(function (r) {
    //        if (r.data.status == "success") {
    //            s.empList = r.data.empList;
    //            s.positionList = r.data.positionList;
    //            s.subPositionList = r.data.subPositionList;
    //            s.warmBodyList = r.data.warmBodyList;
    //            angular.element('#openModalSelectEmp').modal('show');
    //            s.bDisable = false;
    //        }
    //    });
    //}
    
    
    s.refreshDataList = function () {
        s.isLoading = true;
        s.bDisable = true;
        h.post('~/../../AppointmentNonPlantilla/ReloadEmployeeList').then(function (r) {
            if (r.data.status == "success") {
                s.empList = r.data.list;
                s.bDisable = false;
            }
        });

    }


    s.addAppointee = function (data) {
        s.bDisable = true;
        data.tag = data.hazardCode;
        h.post('~/../../AppointmentNonPlantilla/AddNewAppointee', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                s.appointeeList = r.data.appointeeList;
                s.empData.EIC = "";
                s.empData.positionCode = "";
                s.empData.warmBodyGroupCode = "";              
                s.empData.hazardCode = 0;
                angular.element('#openModalSelectEmp').modal('hide');
                toastr["success"]("New appointee added!", "Success");
                s.empData.hazardCode = 0;
                s.bDisable = false;
            }
            else {
                toastr["error"](r.data.status, "Opps...");
                s.bDisable = false;
            }
        });
    }


    //s.deleteAppointee = function (data) {
    //    //alert(data.appointmentItemCode)
    //    h.post('~/../../AppointmentNonPlantilla/DeleteAppointee', { id: data.appointmentItemCode }).then(function (r) {
    //        if (r.data.status == "success") {
    //            s.appointeeList = r.data.appointeeList;
    //            toastr["success"]("Deleting successful...", "Success");
    //        }
    //        else {
    //            toastr["error"]("Please fill-up the required data!", "Opps");
    //        }
    //    });
    //}

    //DELETE
    s.deleteAppointee = function (data) {
        swal({
            title: data.fullNameFirst,
            text: "Remove employee now?",
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#2196f3",
            confirmButtonText: "Yes",
            cancelButtonText: "Cancel",
            closeOnConfirm: true
        }, function (value) {

            if (value == true) {
                if (s.isLoading == false) {
                    s.isLoading = true;
                    s.bDisable = true;
                    h.post('~/../../AppointmentNonPlantilla/DeleteAppointee', { id: data.appointmentItemCode }).then(function (r) {
                        if (r.data.status == "success") {
                            s.appointeeList = r.data.appointeeList;
                            toastr["success"]("Deleting successful...", "Success");
                            s.bDisable = false;
                        }
                        else {
                            toastr["error"]("Please fill-up the required data!", "Opps");
                            s.bDisable = false;
                        }
                        s.isLoading = false;
                    });
                }
                else {
                    toastr["error"]("Unable to save data!", "Error!");
                    s.bDisable = false;
                }

            }

        });
    }




    //////////////////////////////////
    //FORM PRINTING

    s.editFormData = function () {
        s.isReadOnly = false;
    }

    s.changePreSuffix = function () {
        s.appointeeTitleName = "";
        s.assumptData.fullNameTitle = s.assumptData.namePrefix + " " + s.assumptData.fullNameFirst;
        if (s.assumptData.nameSuffix != undefined && s.assumptData.nameSuffix.length >= 2) {
            s.assumptData.fullNameTitle = s.assumptData.fullNameTitle.trim() + ", " + s.assumptData.nameSuffix;
        }
    }


    s.modalApptForm = function (data, type) {
        s.formData = data;
        s.bDisable = true;
        s.isReadOnly = true;

        h.post('~/../../AppointmentNonPlantilla/PrintFormPreviewSetup', { type: type, id: data.appointmentItemCode }).then(function (r) {
            if (r.data.status == "success") {
                if (type == "ASSUMPTION") {

                    s.assumptData = r.data.aData;
                    s.assumptData.assumptionDate = new Date(moment(r.data.aData.assumptionDate).format('YYYY,MM,DD'));
                    angular.element('#modalFormDataAssumption').modal('show');
                    s.isReadOnly = r.data.isReadOnlyTag;
                }
                else if (type == "OATH") {
                    s.oathData = r.data.oData;
                    s.oathData.govtIDIssued = new Date(moment(r.data.oData.govtIDIssued).format('YYYY,MM,DD'));
                    angular.element('#modalFormDataOath').modal('show');
                    s.isReadOnly = r.data.isReadOnlyTag;
                }

                else if (type == "PDF") {
                    s.pdfData = r.data.pdfData;
                    angular.element('#modalFormDataPDF').modal('show');
                    s.isReadOnly = r.data.isReadOnlyTag;
                }
            }
            s.bDisable = false;
        });
    }

    s.updateAssumptionData = function (data) {
        s.bDisable = true;
        data.appItemCode = s.formData.appointmentItemCode;
        h.post('~/../../AppointmentNonPlantilla/SaveAssumptionPrintData', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                s.isReadOnly = true;
                toastr["success"]("Updating succeful!", "Success");
            }
            s.bDisable = false;
        });
    }

    s.updateOathData = function (data) {
        s.bDisable = true;
        data.appItemCode = s.formData.appointmentItemCode;
        h.post('~/../../AppointmentNonPlantilla/SaveOathPrintData', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                s.isReadOnly = true;
                toastr["success"]("Updating succeful!", "Success");
            }
            s.bDisable = false;
        });
    }

    s.updatePDFData = function (data) {
        data.appItemCode = s.formData.appointmentItemCode;
        h.post('~/../../AppointmentNonPlantilla/SavePDFPrintData', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                s.isReadOnly = true;
                toastr["success"]("Updating succeful!", "Success");
                s.bDisable = false;
            }
        });
    }
    
    //PRINT APPOINTMENT
    s.printAppointment = function () {
        var id = s.apptData.appointmentCode;
        h.post('~/../../AppointmentNonPlantilla/PrintPreviewSetup/' + id).then(function (r) {
            if (r.data.status == "success") {
                window.open("../Reports/HRIS.aspx");
            }
        });
    }
    //PRINT FORMS
    s.printApptForms = function (data, type) {       
        s.bDisable = true;
        s.isReadOnly = true;
  
        h.post('~/../../AppointmentNonPlantilla/CheckDataForPrint', { type: type, id: s.formData.appointmentItemCode }).then(function (r) {
            if (r.data.status == "success") {
                s.bDisable = false;
                if (r.data.stat == 0) {
                    var link = "http://davnorsystems.gov.ph:1967/HRReport/Appointment/Report?id=" + r.data.id;
                    window.open(link);
                }
                else {
                    toastr["error"]("Please fill-up the required data!", "Error");
                }
            }
        });
    }

    
    //APPOINTMENT PS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
    s.viewApptPS = function () {
        s.psList = {};
        h.post('~/../../AppointmentNonPlantilla/ViewAppointmentPS').then(function (r) {
            if (r.data.status == "success") {
                s.psList = r.data.list;
            }
            else {
                toastr["error"]("Error loading data!", "Opps");
            }
        });
    }
    
    //CSC RAI TAB
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
    s.viewRAI = function () {    
        if (s.raiList.length == undefined) {           
            h.post('~/../../AppointmentNonPlantilla/CheckAppointmentRAI', { id: s.apptData.appointmentCode }).then(function (r) {
                if (r.data.status == "success") {
                    s.raiList = r.data.raiList;
                    s.postingTag = s.raiList.length; 
                }
                else {
                    toastr["error"]("Error loading data!", "Opps");
                }
            });
        }
       
    }
    
    //GENERATE RAI
    //GenerateAppointmentRAI
    s.generateRAI = function (data) {
        var id = s.apptData.appointmentCode;
        h.post('~/../../AppointmentNonPlantilla/GenerateAppointmentRAI', { id: id }).then(function (r) {
            if (r.data.status == "success") {
                s.raiList = r.data.raiList;
                s.postingTag = s.raiList.length;
                toastr["success"]("RAI generated!", "Success");
            }
            else {
                toastr["error"]("Error loading data!", "Opps");
            }
        });
    }

    //PRINT RAI
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
    s.printRAIReport = function (data) {
        s.bDisable = true;
        var id = data.RAICode;
        h.post('~/../../AppointmentNonPlantilla/PrintRAI', { id: id }).then(function (r) {
            if (r.data.status == "success") {
                s.bDisable = false;
                window.open("../Reports/HRIS.aspx");
            }
        });
    }

    //PRINT RAI
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
    s.printRAIReportAll = function () {        
        // alert(s.apptData.appointmentCode);
        s.bDisable = true;
        h.post('~/../../AppointmentNonPlantilla/PrintRAIALL', { id: s.apptData.appointmentCode }).then(function (r) {
            if (r.data.status == "success") {
                s.bDisable = false;
                window.open("../Reports/HRIS.aspx");
            }
        });
    }
    
    //SHOW MODAL POSTING
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
    s.modalPosting = function (data) {      
        s.casualTag = 0;
        s.bDisable = true;       
        h.post('~/../../AppointmentNonPlantilla/ViewCasualAppointmentItem', { code: data.appointmentItemCode }).then(function (r) {
            if (r.data.status == "success") {
                s.postData = r.data.appData;
                s.workGroupCode = r.data.workGroupCode;
                if (s.workGroupCode == null || s.workGroupCode == "") {
                    s.workGroupCode = s.tmpWorkGroupCode;
                }                 
                s.postData.periodFrom = new Date(moment(s.postData.periodFrom).format('YYYY,MM,DD'));
                angular.element('#modalPosting').modal('show');
                s.bDisable = false;
            }
        });
        var x = new Date(moment(data.periodFrom).format('YYYY,MM,DD'));       
    }


    //SUBMIT POSTING
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
    s.postAppointmentItem = function (data) {        
        if (data.periodFrom == undefined || data.periodFrom == "" || s.workGroupCode == undefined || s.workGroupCode == ""  ) {
            toastr["error"]("Please full-up the required data!", "Error!");
            return;
        }        
        s.bDisable = true;
        s.tmpWorkGroupCode = s.workGroupCode;
        h.post('~/../../AppointmentNonPlantilla/POSTAppointmentItem', { data: data, workGroupCode: s.workGroupCode }).then(function (r) {
            if (r.data.status == "success") {
                toastr["success"]("Posting successful!", "Success");
                s.appointeeList = r.data.appointeeList;
                angular.element('#modalPosting').modal('hide');              
                s.bDisable = false;
            }
            else {
                toastr["error"](r.data.status, "Error!");
            }
        });
    }

}]);