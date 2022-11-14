
app.controller("ApptNonPlanList", ['$scope', '$http', '$document', function (s, h, d) {

    s.appList = {};
    s.appData = {};
    s.appEmpList = {};
    s.panelData = "list";
    s.postData = {};
    s.hazardTag = 0;
    s.casualTag = 0;


    s.isReadOnly = true;
    s.bDisable = false;
    s.isLoading = false;

    s.employmentStat = "";
    s.apptData = {};
    s.appointeeList = {};
    s.tabId = 1;
    s.contentId = 1;
    s.formData = {};
    s.workGroupList = {};

    s.empData = {};
    s.empList = {};

    s.raiList = {};
    s.postingTag = 0;
    s.isGroupRenewal = false;

    s.viewTab = 0;

    //ARCHIVE
    s.viewRemarks = "APPOINTMENT";
    s.archiveApp = {};
    s.appTag = 0;
    s.dataIdx = 0;

    s.workGroupList = {};


    s.data = {};
    s.data.CY = 2022;

    s.budgetYear = [{ text: "CY 2022", value: 2022 }, { text: "CY 2021", value: 2021 }]

    loadData();

    function loadData() {
        s.isLoading = true;
        s.appList = {};
        s.viewRemarks = "APPOINTMENT";
        h.post('~/../../AppointmentNonPlantilla/ApptListData').then(function (r) {
            if (r.data.status == "success") {
                s.apptNatureList = r.data.appNatureList;
                s.appList = r.data.list;
                s.fundSourceList = r.data.fundSource;
                s.workGroupList = r.data.workGroupList;
            }
            s.isLoading = false;
        });
    }
    
    s.changeBudgetYear = function () {
        s.fundSourceList = {};
        h.post('~/../../AppointmentNonPlantilla/ChangeFundSource', { cy: s.data.CY }).then(function (r) {
            if (r.data.status == "success") {               
                s.fundSourceList = r.data.fundSource;
                toastr["info"]("Budget year selected!", s.data.CY);
            }
            s.isLoading = false;
        });
    }


    //APPOINTMENT CREATE
    s.submitAppointmentMainData = function (data) {
        s.bDisable = true;
        if (data.periodTo == undefined || data.periodTo == "" || data.fundSourceCode == undefined || data.fundSourceCode == "") {
            toastr["error"]("Please fill-up the required fields!", "Opps..");
            s.bDisable = false;
            return;
        }
        if (s.employmentStatusCode == "05") {
            if (data.appNatureCode == undefined || data.appNatureCode == null) {
                toastr["error"]("Please fill-up the required fields!", "Opps...");
                s.bDisable = false;
                return;
            }
        }

        data.employmentStatusCode = s.employmentStatusCode;
        data.workGroupCode = "";
        h.post('~/../../AppointmentNonPlantilla/SaveAppointmentMasterData', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                s.appointeeList = {};
                s.apptData = r.data.appointment;
                s.apptData.employmentStatus = s.employmentStat;
                s.contentId = 2;
                angular.element('#modalAppointmentCreate').modal('hide');
                toastr["success"]("New appointment added!", "Success");
                s.bDisable = false;
            }
        });
    }

    s.viewApptDetails = function (data, idx) {
        s.apptData = data;
        s.dataIdx = idx;
        h.post('~/../../AppointmentNonPlantilla/AppointeeList', { id: s.apptData.appointmentCode }).then(function (r) {
            if (r.data.status == "success") {
                s.appointeeList = r.data.appointeeList;
                s.contentId = 2;
                s.apptData.tag = r.data.appTag;
            }
        });
    }

    s.addAppointee = function (data) {
        s.bDisable = true;
        data.appointmentCode = s.apptData.appointmentCode;

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

    //ARCHIVE 
    s.viewAppointment = function (id) {
        if (id == 0) {
            if (id == 0 && s.viewRemarks == "APPOINTMENT") {
                return;
            }
            s.viewRemarks = "APPOINTMENT";
        }
        else {
            if (id == 1 && s.viewRemarks == "APPOINTMENT (ARCHIVE)") {
                return;
            }
            s.viewRemarks = "APPOINTMENT (ARCHIVE)";
        }
        s.isLoading = true;
        h.post('~/../../AppointmentNonPlantilla/GetAppointmentList', { id: id }).then(function (r) {
            if (r.data.status == "success") {
                s.appList = r.data.appointment;
            }
            s.isLoading = false;
        });

    }

    //SEND TO ARCHIVE
    s.sendToArchive = function (data, idx) {
        s.isLoading = true;
        h.post('~/../../AppointmentNonPlantilla/SendToArchive', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                toastr["success"]("Archiving successful", "Success");
                s.appList.splice(s.dataIdx, 1);
                s.contentId = 1;
            }
            s.isLoading = false;
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
                    s.workGroupList = r.data.workGroupList;
                    if (s.selectTag == 1) {
                        s.subPositionList = r.data.subPositionList;
                    }
                    angular.element('#openModalSelectEmp').modal('show');
                }
            });
        }
        else { angular.element('#openModalSelectEmp').modal('show'); }
    }

    s.viewApptList = function (data) {
        s.panelData = "list";
    }

    s.backToMain = function () {
        s.contentId = 1;
    }

    s.modalAPPTCreate = function (code) {

        if (code == "CASUAL") {
            s.employmentStatusCode = "05";
            s.employmentStat = "CASUAL";
        }
        else if (code == "JO") {
            s.employmentStatusCode = "06";
            s.employmentStat = "JOB ORDER";
        }
        else if (code == "COS") {
            s.employmentStatusCode = "07";
            s.employmentStat = "CONTRACT OF SERVICE";
        }
        else if (code == "HON") {
            s.employmentStatusCode = "08";
            s.employmentStat = "HONORARIUM";
        }
        angular.element('#modalAppointmentCreate').modal('show');
    }



    s.myDateFormat = function (date) {
        if (!date) {
            return '';
        }
        return (moment(date).format('MMMM DD, YYYY'));
    };


    s.viewPrintableForms = function (data) {
        s.bDisable = true;
        h.post('~/../../AppointmentNonPlantilla/SetupApptNPPrintingData', { id: data.appointmentCode }).then(function (r) {
            if (r.data.status == "success") {
                s.bDisable = false;
                window.location.href = "../AppointmentNonPlantilla/Printing";
            }
        });
    }


    s.deleteAppointment = function (data, idx) {
        h.post('~/../../AppointmentNonPlantilla/CheckApptDelete', { data: data }).then(function (r) {
            if (r.data.status == "success") {

                swal({
                    title: "Are you sure?",
                    text: "Delete Appointment",
                    type: "warning",
                    showCancelButton: true,
                    confirmButtonColor: "#2196f3",
                    confirmButtonText: "Yes",
                    cancelButtonText: "Cancel",
                    closeOnConfirm: false
                }, function (value) {
                    if (value == true) {
                        if (s.isLoading == false) {
                            s.isLoading = true;
                            h.post('~/../../AppointmentNonPlantilla/DeleteAppointment', { data: data }).then(function (r) {
                                if (r.data.status == "success") {
                                    s.isLoading = false;
                                    s.apptCode = r.data.apptCode;
                                    s.appointeeList = {};
                                    s.data = {};
                                    s.data.fundSourceCode = "";
                                    swal("Deleting successful", "", "success");
                                    s.appList.splice(idx, 1);
                                }
                                else {
                                    toastr["error"]("Your are not allowed to delete this appointment!", "Opps!");
                                    s.isLoading = false;
                                }
                            });
                        }
                    }
                });

            }
            else {
                toastr["error"]("Your are not allowed to delete this appointment!", "Opps!");
            }
        });
    }

    s.markAsFinal = function (data) {
        alert(data.appointmentCode);     
        swal({
            title: "Are you sure?",
            text: "Mark appointment as final",
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#2196f3",
            confirmButtonText: "Yes",
            cancelButtonText: "Cancel",
            closeOnConfirm: false
        }, function (value) {
            if (value == true) {
                if (s.isLoading == false) {
                    s.isLoading = true;
                    h.post('~/../../AppointmentNonPlantilla/FinalizeAppointment', { id: data.appointmentCode }).then(function (r) {
                        if (r.data.status == "success") {
                            swal("Finalizing successful!", "", "success");
                            s.apptData.tag = 1;
                            s.contentId = 1;
                            s.apptCode = r.data.apptCode;
                            //s.appointeeList = {};
                            //s.data = {};
                            //s.data.fundSourceCode = "";
                            loadData();
                            s.isLoading = false;
                        }
                        else {
                            s.isLoading = false;
                            swal("Error", r.data.status, "error");
                        }
                    });
                }
            }
        });
    }

    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
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

    //PRINT APPOINTMENT
    s.printAppointment = function (page) {       
        var id = s.apptData.appointmentCode;
        if (s.apptData.tag == 0) { //NOT YET FINAL
            s.isLoading = false;
            swal({
                title: "Draft Only?",
                text: "Appointment not yet final!",
                type: "warning",
                showCancelButton: true,
                confirmButtonColor: "#2196f3",
                confirmButtonText: "Yes",
                cancelButtonText: "Cancel",
                closeOnConfirm: true
            }, function (value) {               
                if (value == true) {
                    s.isLoading = true;
                    h.post('~/../../AppointmentNonPlantilla/PrintPreviewSetup', { id: id, page: page }).then(function (r) {
                        if (r.data.status == "success") {
                            s.isLoading = false;
                            window.open("../Reports/HRIS.aspx");
                        }
                    });
                }
            });
        }
        else { //FINALIZE
            s.isLoading = true;
            h.post('~/../../AppointmentNonPlantilla/PrintPreviewSetup', { id: id, page: page }).then(function (r) {
                if (r.data.status == "success") {
                    s.isLoading = false;
                    window.open("../Reports/HRIS.aspx");
                }
            });
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

    //UPDATING...
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

    s.refreshDataList = function () {
        s.isLoading = true;
        s.bDisable = true;
        h.post('~/../../AppointmentNonPlantilla/ReloadEmployeeList').then(function (r) {
            if (r.data.status == "success") {
                s.empList = r.data.list;
                s.bDisable = false;
                s.isLoading = false;
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
        s.postingTag = 0;
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
        h.post('~/../../AppointmentNonPlantilla/GenerateAppointmentRAI', { id: id, dateIssued: data.dateIssued }).then(function (r) {
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
                s.bDisable = false;
                angular.element('#modalPosting').modal('show');
            }
            else {
                toastr["error"](r.data.status, "Error!");
                s.bDisable = true;
            }
        });
    }

    //SUBMIT POSTING
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
    s.postAppointmentItem = function (data) {
        if (data.periodFrom == undefined || data.periodFrom == "" || s.workGroupCode == undefined || s.workGroupCode == "") {
            toastr["error"]("Please full-up the required data!", "Error!");
            return;
        }
        s.bDisable = true;
        s.tmpWorkGroupCode = s.workGroupCode;
        h.post('~/../../AppointmentNonPlantilla/POSTAppointmentItem', { data: data, workGroupCode: s.workGroupCode }).then(function (r) {
            if (r.data.status == "success") {
                s.bDisable = false;
                toastr["success"]("Posting successful!", "Success");
                angular.element('#modalPosting').modal('hide');
                s.appointeeList = r.data.appointeeList;
            }
            else {
                s.bDisable = false;
                toastr["error"](r.data.status, "Error!");
            }
        });
    }

    s.openModalSearchAppointee = function () {
        angular.element('#modalSearchAppointee').modal('show');
    }
    
    s.searchApptList = {};
    s.searchAppointeeByName = function (str) {
        s.searchApptList = {};
        h.post('~/../../AppointmentNonPlantilla/SearchAppointeeByName', { id: str }).then(function (r) {
            if (r.data.status == "success") {
                s.searchApptList = r.data.myList;
                s.bDisable = false;
            }
            else {
                toastr["error"](r.data.status, "Error!");
            }
        });
    }


    //SHOW APPOINTEE LIST
    s.ShowAppointeeList = function (data) {
        if (data.fundSourceCode == "" || data.fundSourceCode == null || data.workGroupCode == "" || data.workGroupCode == null) {
            toastr["error"]("Please fill-up all required data!", "Error!");
            return;
        }

        s.isLoading = true;
        data.employmentStatusCode = s.employmentStatusCode;

        h.post('~/../../AppointmentNonPlantilla/ViewAppointeeByGroup', { data: data, workGroupCode: data.workGroupCode }).then(function (r) {
            if (r.data.status == "success") {
                s.appointeeList = r.data.appointeeList;
                s.bDisable = false;
                s.viewTab = 1;
            }
            else {
                toastr["error"](r.data.status, "Error!");
            }
        });
        s.isLoading = false;
    }

    //BACK TO FORM
    s.backToFormTab = function () {
        s.viewTab = 0;
    }


    //SUBMIT RENEWAL BY GROUP
    //s.submitGroupRenewal = function (data) {
    //    if (data.fundSourceCode == "" || data.fundSourceCode == null || data.workGroupCode == "" || data.workGroupCode == null) {
    //        toastr["error"]("Please fill-up all required data!", "Error!");
    //        return;
    //    }

    //    s.bDisable = true;
    //    s.isLoading = true;
    //    data.employmentStatusCode = s.employmentStatusCode;
    //    h.post('~/../../AppointmentNonPlantilla/SubmitRenewalGroup', { data: data, workGroupCode: data.workGroupCode }).then(function (r) {
    //        if (r.data.status == "success") {
    //            s.bDisable = false;
    //            s.contentId = 2;
    //            s.apptData = r.data.appData;
    //            s.appointeeList = r.data.appointeeList;
    //            angular.element('#modalAppointmentCreate').modal('hide');
    //            toastr["success"]("Saving successful!", "Success");
    //            s.bDisable = false;
    //            s.isLoading = false;
    //            s.data = {};
    //        }
    //        else {
    //            toastr["error"](r.data.status, "Error!");
    //            s.bDisable = false;
    //            s.isLoading = false;
    //        }
    //    }); 
    //}



}]);