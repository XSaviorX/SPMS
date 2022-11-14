
toastr.options = {
    "closeButton": true,
    "debug": false,
    "newestOnTop": false,
    "progressBar": true,
    "positionClass": "toast-top-right",
    "preventDuplicates": false,
    "onclick": null,
    "showDuration": "300",
    "hideDuration": "1000",
    "timeOut": "3000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
}

app.controller('AppointmentList', ['$scope', '$http', '$document', function (s, h, d) {

    s.appPending = {};
    s.appPosted = {};
    s.appIncoming = {};
    s.appNatureList = {};
    s.eligList = {};
    s.appointmentData = {};

    s.appointmentTempoData = {}; //temporary appointment


    s.appData = {};

    s.formApp = {};
    s.formData = {};

    s.isReadOnly = false;
    s.employmentStatus = [{ employmentStatusCode: '02', employmentStatus: 'CO-TERMINOUS' }, { employmentStatusCode: '03', employmentStatus: 'PERMANENT' }, { employmentStatusCode: '04', employmentStatus: 'TEMPORARY' }];
    s.stepList = [{ code: 1, name: '1' }, { code: 2, name: '2' }, { code: 3, name: '3' }, { code: 4, name: '4' }, { code: 5, name: '5' }, { code: 6, name: '6' }, { code: 7, name: '7' }, { code: 8, name: '8' }];

    s.bDisable = false;

    s.incomingCount = 0;
    s.employeeList = {};

    s.raiAppt = {};
    s.raiTab = 0;
    s.raiData = {};
    s.temporary = {};
    s.tempCounter = 0;

    loadInitData();

    function loadInitData() {
        h.post('~/../../Appointment/AppointmentInitData').then(function (r) {
            if (r.data.status == "success") {
                s.appPending = r.data.appPending;
                s.appPosted = r.data.appPosted;
                s.appIncoming = r.data.appIncoming;
                s.incomingCount = s.appIncoming.length;
                s.appNatureList = r.data.appNatureList;
                s.eligList = r.data.eligList;
                s.temporary = r.data.temporary;
                s.tempCounter = s.temporary.length;
            }
        });
    }

    s.createAppointment = function (data) {
        s.isReadOnly = true;
        s.bDisable = true;
        h.post('~/../../Appointment/GetAppointmentData', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                s.isReadOnly = false;
                s.appointmentData = r.data.appData;
                s.appointmentData.fullNameTitle = s.appointmentData.fullName;
                //s.appointmentData.emp =
                s.appointmentData.step = 1;
                angular.element('#modalAppointment').modal('show');
                s.bDisable = false;
            }
        });
    }

    //RAI
    s.RAIAppList = {};
    s.postedApptList = {};
    s.showRAIAppt = function () {
        s.raiTab = 0;
        h.post('~/../../Appointment/GetRAIList').then(function (r) {
            if (r.data.status == "success") {
                s.RAIAppList = r.data.rai;
                s.postedApptList = r.data.postedApptList;               
            }
        });
    }

    //RAI MODAL    
    s.modalRAI = function () { 
        s.raiData = {};
        angular.element('#modalRAI').modal('show');
    }

    //CREATE RAI
    s.createRAI = function (data) {
        s.bDisable = true;
        if (data.RAIName == null || data.RAIDate == null) {
            toastr["error"]("Please fill-up the form completely!", "Opps");
            s.bDisable = false;
            return;
        }
        h.post('~/../../Appointment/CreateRAI', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                s.raiData = r.data.raiData;
                s.raiTab = 1;
                s.raiAppList = {};
                toastr["success"]("New RAI created!", "Success");
                angular.element('#modalRAI').modal('hide');
                s.bDisable = false;
            }
            else {
                toastr["error"](r.data.status, "Opps");
                s.bDisable = false;
            }
        });
    }

    //VIEW Appointment list of RAI
    s.viewRAIData = function (data) {
        h.post('~/../../Appointment/GetRAIAppointment', { id: data.RAICode }).then(function (r) {
            if (r.data.status == "success") {
                s.raiData = data;
                s.raiTab = 1;
                s.raiAppList = r.data.raiAppList;
                s.bDisable = false;
            }            
        });       
    }

    //Remove RAI
    s.removeRAI = function (data) {
        s.bDisable = true;
        h.post('~/../../Appointment/RemoveRAI', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                s.raiTab = 0;
                s.RAIAppList = r.data.rai;
                s.bDisable = false;
                toastr["success"]("Removing successful!", "Success");
            }
            else {
                toastr["error"](r.data.status, "Opps");
                s.bDisable = false;
            }
        });
    }


    //Remove RAI ITEM
    s.removeRAIItem = function (data) {
        h.post('~/../../Appointment/RemoveRAIItem', { data : data }).then(function (r) {
            if (r.data.status == "success") {
                //s.raiData = data;
                //s.raiTab = 1;
                s.raiAppList = r.data.raiAppList;
                s.bDisable = false;
            }
            else {
                toastr["error"](r.data.status, "Opps");
                s.bDisable = false;
            }
        });
    }

    s.modalOpenRAIAPPPTAdd = function () {
        angular.element('#modalRAIAPPPTAdd').modal('show');
    }

    s.addToRAI = function (data) {
        h.post('~/../../Appointment/AddAPPTToRAI', { id: data.appointmentCode, code: s.raiData.RAICode }).then(function (r) {
            if (r.data.status == "success") {
                toastr["success"]("Adding successful!", "Success");
                s.raiAppList = r.data.raiAppList;
                s.bDisable = false;
            }
            else {
                toastr["error"](r.data.status, "Opps");
                s.bDisable = false;
            }
        });
    }

    s.backToRAIList = function () {
        s.raiTab = 0;
    }

    s.updateCheckList = function (tag) {
        s.bDisable = true;
        h.post('~/../../Appointment/UpdateRAICheckList', { id: s.raiData.RAICode, tag: tag }).then(function (r) {
            if (r.data.status == "success") {
                toastr["success"]("Updating successful!", "Success");
               
                if (tag == 1) {
                    if (s.raiData.chkList_ApptForms == 1) {
                        s.raiData.chkList_ApptForms = 0;
                    }
                    else {
                        s.raiData.chkList_ApptForms = 1;
                    }
                }
                else if (tag == 2) {
                    if (s.raiData.chkList_Casual == 1) {
                        s.raiData.chkList_Casual = 0;
                    }
                    else {
                        s.raiData.chkList_Casual = 1;
                    }
                }
                else if (tag == 3) {
                    if (s.raiData.chkList_PDS == 1) {
                        s.raiData.chkList_PDS = 0;
                    }
                    else {
                        s.raiData.chkList_PDS = 1;
                    }
                }
                else if (tag == 4) {
                    if (s.raiData.chkList_Eligibility == 1) {
                        s.raiData.chkList_Eligibility = 0;
                    }
                    else {
                        s.raiData.chkList_Eligibility = 1;
                    }
                }
                else if (tag == 5) {
                    if (s.raiData.chkList_PDF == 1) {
                        s.raiData.chkList_PDF = 0;
                    }
                    else {
                        s.raiData.chkList_PDF = 1;
                    }
                }
                else if (tag == 6) {
                    if (s.raiData.chkList_Oath == 1) {
                        s.raiData.chkList_Oath = 0;
                    }
                    else {
                        s.raiData.chkList_Oath = 1;
                    }
                }
                else if (tag == 7) {
                    if (s.raiData.chkList_Assumption == 1) {
                        s.raiData.chkList_Assumption = 0;
                    }
                    else {
                        s.raiData.chkList_Assumption = 1;
                    }
                }
                
            }
            else {
                toastr["error"](r.data.status, "Opps");
                //s.bDisable = false;
            }
            s.bDisable = false;
        });
    }
    
    //PRINT RAI Report
    s.printRAI = function (code) {
        s.bDisable = true;
        h.post('~/../../Appointment/PrintRAIPlantilla', { id: s.raiData.RAICode, tag: code }).then(function (r) {
            if (r.data.status == "success") {
                window.open("../Reports/HRIS.aspx");
                s.bDisable = false;
            }
        });
    }
            

    //PRINT RAI Report
    s.printRAIBACK = function () {
        s.bDisable = true;
        h.post('~/../../Appointment/PrintRAIPlantillaBack', { id: s.raiData.RAICode }).then(function (r) {
            if (r.data.status == "success") {
                window.open("../Reports/HRIS.aspx");
                s.bDisable = false;
            }
        });
    }

    
    s.submitAppointment = function (data) {
        if (data.appNatureCode == null || data.appNatureCode == undefined || data.eligibilityCode == undefined || data.eligibilityCode == "") {
            toastr["error"]("Please fill-up the required fields!", "Opps...");
            return;
        }
        if (data.step == null || data.step == "" || data.step == undefined) {
            toastr["error"]("Please fill-up the required fields!", "Opps...");
            return;
        }
        h.post('~/../../Appointment/SaveAppointment', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                loadInitData();
                angular.element('#modalAppointment').modal('hide');
                toastr["success"]("New appointment created!", "Success");
            }
        });
    }

    s.formatDate = function (date) {
        if (!date) {
            return '';
        }
        var tmp = moment(date).format('MM/DD/YYYY');
        if (tmp == "01/01/0001") {
            return '-'
        }
        return tmp;
    };

    //SHOW EDIT MODAL
    s.viewAppointmentData = function (data) {
        s.appData = data;
        h.post('~/../../Appointment/EditFormData', { id: data.appointmentCode }).then(function (r) {
            if (r.data.status == "success") {
                s.isReadOnly = true;
                s.formApp = r.data.formApp;
                s.formData = r.data.formData;
                angular.element('#modalApptFormEdit').modal('show');
            }
        });
    }

    //ALLOW EDIT
    s.editAppointment = function () {
        toastr["info"]("Editing is now allowed.", "Edit");
        s.isReadOnly = false;
        angular.element('#modalApptFormEdit').modal('show');
    }

    //UDATING APPOINTMENT DATA
    s.updateAppData = function (app, data) {
        s.bDisable = true;
        h.post('~/../../Appointment/UpdateFormData', { app: app, data: data }).then(function (r) {
            if (r.data.status == "success") {
                s.isReadOnly = true;
                toastr["success"]("Updating successful.", "Update")
                s.bDisable = false;
            }
        });
    }


    //AppointeePostingTab
    s.AppointeePostingTab = function () {
        if (s.employeeList) {
            h.post('~/../../Appointment/AppointmentPostingData').then(function (r) {
                if (r.data.status == "success") {
                    s.employeeList = r.data.employeeList;
                    s.publicationItemList = r.data.publicationItemList;
                }
            });
        }
    }

    s.pubSelectedItem = {};
    s.seletedPosition = function (id) {
        var item73 = publicationItemList.filter(function (id) {
            return item.publicationItemCode === id;
        })[0]; //alert(item73);
    }

    s.submitAppointeeToList = function (data) {
        h.post('~/../../Appointment/PostAppointeeToAppList', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                s.employeeList = r.data.employeeList;
                s.publicationItemList = r.data.publicationItemList;
            }
        });
    }

    s.PrintCrystalReport = function (type, id) {
        printApptCrystal(type, id)
    }

    s.printApptTelerik = function (type, id) {
        h.post('~/../../Appointment/PrintPreviewSetup', { rptType: type, id: id }).then(function (r) {
            if (r.data.status == "success") {
                window.open("../Reports/HRIS.aspx");
            }
            else {
                toastr["error"](r.data.status, "Opps!")
            }
        });
    }

    s.editFormData = function () {
        s.isReadOnly = false;
    }

    //ASSUMPTION
    s.assumptData = {};
    s.CheckAssumption = function (data) {
        s.formData = data;
        h.post('~/../../Appointment/CheckAssumptionReport', { id: data.appointmentCode }).then(function (r) {
            if (r.data.status == "success") {
                s.isReadOnly = true;
                s.assumptData = r.data.data;
                angular.element('#modalFormDataAssumption').modal('show');
            }
        });
    }

    s.PrintAssumptionReport = function (data) {
        var myData = {};
        myData.printCode = data.appointmentCode;
        myData.printType = "ASSUMPTION";
        h.post('~/../../Appointment/AppointmentPrintSetup', { data: myData }).then(function (r) {
            if (r.data.status == "success") {
                s.bDisable = false;
                if (r.data.stat == 0) {
                    //var link = "http://localhost:2438/Appointment/Plantilla?id=" + r.data.printID;
                    var link = "http://davnorsystems.gov.ph:1967/HRReport/Appointment/Plantilla?id=" + r.data.printID;
                    window.open(link);
                }
                else {
                    toastr["error"]("Please fill-up the required data!", "Error");
                }
            }
        });
    }

     

    s.updateAssumptionData = function (data) {
        h.post('~/../../Appointment/UpdateAssumption', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                s.isReadOnly = true;
                toastr["success"]("Updating successful!", "Success");
            }
        });
    }


    //OATH OF OFFICE
    s.oathData = {};
    s.CheckOathOffice = function (data) {
        s.formData = data;
        h.post('~/../../Appointment/CheckOathOfOffice', { id: data.appointmentCode }).then(function (r) {
            if (r.data.status == "success") {
                s.isReadOnly = true;
                s.oathData = r.data.data;
                angular.element('#modalFormDataOath').modal('show');
            }
        });
    }

    //updateOathData
    s.updateOathData = function (data) {
        h.post('~/../../Appointment/UpdateOathData', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                s.isReadOnly = true;
                s.oathData = r.data.data;
                toastr["success"]("Updating successful!", "Success");
            }
        });
    }

    s.PrintOathOfOffice = function (data) {
        var myData = {};
        myData.printCode = data.appointmentCode;
        myData.printType = "OATH";
        h.post('~/../../Appointment/AppointmentPrintSetup', { data: myData }).then(function (r) {
            if (r.data.status == "success") {
                s.bDisable = false;
                if (r.data.stat == 0) {
                    //var link = "http://localhost:2438/Appointment/Plantilla?id=" + r.data.printID;
                    var link = "http://davnorsystems.gov.ph:1967/HRReport/Appointment/Plantilla?id=" + r.data.printID;
                    window.open(link);
                }
                else {
                    toastr["error"]("Please fill-up the required data!", "Error");
                }
            }
        });
    }

    s.CSCLimitData = {};
    //CSC LIMITATION 
    s.CheckCSCLimit = function (data) {
        s.formData = data;
        h.post('~/../../Appointment/CheckCSCLimitation', { id: data.appointmentCode }).then(function (r) {
            if (r.data.status == "success") {
                var myData = {};
                myData.printCode = data.appointmentCode;
                myData.printType = "OATH";
                var link = "http://localhost:2438/Appointment/Plantilla?id=" + r.data.printID;
            }
        });
    }

    //PRINTING
    function printApptCrystal(type, id) {
        if (type == "ASSUMPTION") {
            var data = {};
            data.printCode = id;
            data.printType = type;
            h.post('~/../../Appointment/AppointmentPrintSetup', { data: data }).then(function (r) {
                if (r.data.status == "success") {
                    s.bDisable = false;
                    if (r.data.stat == 0) {
                        //var link = "http://localhost:2438/Appointment/AppointmentReport?id=" + r.data.printID;
                        var link = "http://davnorsystems.gov.ph:1967/HRReport/Appointment/Plantilla?id=" + r.data.id;
                        window.open(link);
                    }
                    else {
                        toastr["error"]("Please fill-up the required data!", "Error");
                    }
                }
            });
        }
        else if (type == "OATH") {
            var data = {};
            data.printCode = id;
            data.printType = type;
            h.post('~/../../Appointment/AppointmentPrintSetup', { data: data }).then(function (r) {
                if (r.data.status == "success") {
                    s.bDisable = false;
                    if (r.data.stat == 0) {
                        //var link = "http://localhost:2438/Appointment/AppointmentReport?id=" + r.data.printID;
                        var link = "http://davnorsystems.gov.ph:1967/HRReport/Appointment/Plantilla?id=" + r.data.id;
                        window.open(link);
                    }
                    else {
                        toastr["error"]("Please fill-up the required data!", "Error");
                    }
                }
            });
        }

    }


    //PRINTING
    s.printHRISReport = function (type, id) {
        id = s.appData.appointmentCode;

        var data = {};
        data.printType = type;
        data.printCode = id;

        h.post('~/../../Appointment/AppointmentPrintSetup', { data: data }).then(function (r) {
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


    s.postingModal = function (data) {
        s.appointmentData = data;
        s.appointmentData.effectivityDate = null;
        angular.element('#modalAppointmentPosting').modal('show');
    }

    s.postingAppointment = function () {
        var id = s.appointmentData.appointmentCode;

        if (s.appointmentData.effectivityDate == null || s.appointmentData.effectivityDate == undefined) {
            return;
        }

        h.post('~/../../Appointment/PostPlantillaAppointment', { id: id, assumptDate: s.appointmentData.effectivityDate }).then(function (r) {
            if (r.data.status == "success") {
                s.bDisable = false;
                toastr["success"]("Posting successful!", "Success");
                angular.element('#modalAppointmentPosting').modal('hide');
            }
            else {
                toastr["error"](r.data.status, "Error");
            }
        });
    }
    
    s.temporaryRenewal = function (data, tag) {      
        s.bDisable = true;
        h.post('~/../../Appointment/TemporaryAppointmentData', { id: data.appointmenCode, tag: tag }).then(function (r) {
            if (r.data.status == "success") {
                s.bDisable = false;
                s.appointmentTempoData = r.data.appointmentData;
                angular.element('#modalAppointmentTemporary').modal('show');
               
            }
            else {
                s.bDisable = false;
                toastr["error"](r.data.status, "Error");
            }
        }); 
    }

    s.sumbitTempoparyRenewal = function (data) {
        s.bDisable = true;
        h.post('~/../../Appointment/SaveTemporaryApptRenewal', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                s.bDisable = false;
               
                angular.element('#modalAppointmentTemporary').modal('hide');
            }
            else {
                s.bDisable = false;
                toastr["error"](r.data.status, "Error");
            }
        });
    }

}]);
