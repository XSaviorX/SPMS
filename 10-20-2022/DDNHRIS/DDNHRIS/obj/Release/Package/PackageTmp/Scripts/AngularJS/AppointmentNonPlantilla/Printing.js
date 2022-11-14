app.controller('AppointmentNPPrinting', ['$scope', '$http', '$document', function (s, h, d) {
        

    s.appCode = "";
    s.apptData = {};
    s.oathData = {};
    s.assumptData = {};
    s.pdfData = {};
    s.appointeeList = {};
    s.bDisable = false;

    

    loadInitData();

    s.isReadOnly = true;

    function loadInitData() {
        h.post('~/../../AppointmentNonPlantilla/LoadAppointeeList').then(function (r) {
            if (r.data.status == "success") {
                s.appointeeList = r.data.appointeeList;
                s.appCode = r.data.appCode;
            }
        });
    }
    
    s.modalApptForm = function (data, type) {
        s.apptData = data;
        s.bDisable = true;
        s.isReadOnly = true;
        h.post('~/../../AppointmentNonPlantilla/PrintFormPreviewSetup', { type: type, id: data.appointmentItemCode }).then(function (r) {
            if (r.data.status == "success") {
                if (type == "OATH") {
                    s.oathData = r.data.oData;
                    s.oathData.govtIDIssued = new Date(moment(r.data.oData.govtIDIssued).format('YYYY,MM,DD'));
                    angular.element('#modalFormDataOath').modal('show');
                    s.isReadOnly = r.data.isReadOnlyTag;
                }
                else if (type == "ASSUMPTION") {
                    s.assumptData = r.data.aData;
                    s.assumptData.assumptionDate = new Date(moment(r.data.aData.assumptionDate).format('YYYY,MM,DD'));
                    angular.element('#modalFormDataAssumption').modal('show');
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
    

    //printing
    s.printApptForms = function (data, type) {
        s.bDisable = true;
        s.isReadOnly = true;
        h.post('~/../../AppointmentNonPlantilla/CheckDataForPrint', { type: type, id:  s.apptData.appointmentItemCode }).then(function (r) {
            if (r.data.status == "success") {
                s.bDisable = false;
                if (r.data.stat == 0) {
                    var link = "http://superdesk/ReportCenter/Home/Report?id=" + r.data.id;
                    window.open(link);
                }
                else {
                    toastr["error"]("Please fill-up the required data!", "Error");
                }                                               
                 
            }
        });
    }

    s.editFormData = function()  {       
        s.isReadOnly = false;      
    }
    
    s.changePreSuffix = function () {
        s.appointeeTitleName = "";
        s.assumptData.fullNameTitle = s.assumptData.namePrefix + " " + s.assumptData.fullNameFirst;
        if (s.assumptData.nameSuffix != undefined && s.assumptData.nameSuffix.length >= 2) {
            s.assumptData.fullNameTitle = s.assumptData.fullNameTitle.trim() + ", " + s.assumptData.nameSuffix;
        }
    }


    s.updateAssumptionData = function (data) {
        s.bDisable = true;
        data.appItemCode = s.apptData.appointmentItemCode;
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
        data.appItemCode = s.apptData.appointmentItemCode;
        h.post('~/../../AppointmentNonPlantilla/SaveOathPrintData', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                s.isReadOnly = true;
                toastr["success"]("Updating succeful!", "Success");
            }
            s.bDisable = false;
        });
    }

    s.updatePDFData = function (data) {
        data.appItemCode = s.apptData.appointmentItemCode;
        h.post('~/../../AppointmentNonPlantilla/SavePDFPrintData', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                s.isReadOnly = true;
                toastr["success"]("Updating succeful!", "Success");
                s.bDisable = false;
            }
        });
    }
    
    s.printPDF = function (type) {            
        h.post('~/../../AppointmentNonPlantilla/PrintPDFCasual', { id: s.apptData.appointmentItemCode, type: type }).then(function (r) {
            if (r.data.status == "success") {
                window.open("../Reports/HRIS.aspx");
            }
        });
    }

    //PRINT APPOINTMENT
    s.printPreview = function () {
        var id = s.appCode;       
        h.post('~/../../AppointmentNonPlantilla/PrintPreviewSetup/' + id).then(function (r) {
            if (r.data.status == "success") {
                window.open("../Reports/HRIS.aspx");
            }
        });
    }


}]);