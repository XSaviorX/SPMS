app.controller('EmployeeList', ['$scope', '$http', '$document', function (s, h, d) {

    s.data = {};
    s.empList = {};
    s.searchList = {};
    s.workGroupList = {};
    s.tag = 0;
    s.bDisable = false;
    s.searchText = "";

    s.tabId = 1;

    s.isReadOnly = true;
    s.isLoading = false;
    //2
    s.profileData = {};
    s.famData = {};
    s.children = {};
    s.educationData = {};
    s.educationList = {};
    s.educData = {};
    s.trainingList = {};


    s.elig = {}    
    s.eligType = {};
    s.empEligList = {};

    s.workExperience = {};
    s.trainings = {};

    s.imageURL = "http://www.w3schools.com/css/trolltunga.jpg";

    s.civilStatus = [{ civilStatCode: 'SINGLE', civilStatName: 'SINGLE' }, { civilStatCode: 'MARRIED', civilStatName: 'MARRIED' }, { civilStatCode: 'SEPARATED', civilStatName: 'SEPARATED' }];
    s.sexList = [{ code: 'MALE', name: 'MALE' }, { code: 'FEMALE', name: 'FEMALE' }];

    s.empStatusList = [{ code: '05', name: 'Casual' }, { code: '06', name: 'Job Order' }, { code: '07', name: 'Honorarium' }, { code: '08', name: 'Contract' }];
    s.fundSourceList = {};

    s.educStatus = [{ stat: 1, statName: "VALIDATE" }, { stat: 0, statName: "RETURN" }];



    loadInitData();

    function loadInitData() {
        showMyEmployeeList(0);
    }

    s.backToList = function () {
        s.tabId = 1;
    }

    s.modalAPPTCreate = function (id) {
        alert(id);
    }


    //PROFILE ***********************************************************************

    s.loadProfile = function () {
        s.isReadOnly = true;
        h.post('~/../../Employee/ProfileData/' + id.EIC).then(function (r) {
            s.profileData = r.data.profileData;            
        });
    }

    s.viewProfile = function (id) {
        s.isReadOnly = true;
        s.tabId = 2;       
        h.post('~/../../Employee/ProfileData/' + id.EIC).then(function (r) {
            s.profileData = r.data.profileData;
            s.imageURL = r.data.img;
        });
    }

    s.enableText = function (id) {
        s.profileData.birthDate = new Date(moment(s.profileData.birthDate).format('YYYY,MM,DD'));        
        if (id == 1) {
            s.isReadOnly = false;
            toastr["info"]("Edting is now allowed!", "Info");
        }
        else {
            s.isReadOnly = true;
        }
    }

    s.cancelEditing = function () {
        h.post('~/../../Employee/ProfileData/' + s.profileData.EIC).then(function (r) {
            s.profileData = r.data.profileData;
        });
        s.isReadOnly = true;
    }

    s.updateProfile = function (data) {       
        s.bDisable = true;
        h.post('~/../../Employee/ProfileUpdate', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                s.bDisable = false;
                s.isReadOnly = true;
                toastr["success"]("Updating Successful!", "Good");
            }
        });
    }

    // SEARCH / *************************************************************************
    s.searchEmployee = function () {
        s.bDisable = true;
        h.post('~/../../Employee/EmployeeSearch', { id: s.searchText }).then(function (r) {
            if (r.data.status == "success") {
                s.empList = r.data.empList;
                s.bDisable = false;
            }           
        });
    }

    //PROFILE /***********************************************************************
     
    function showMyEmployeeList(id) {
        s.tag = id;
       
        if (id == 0) {
            h.post('~/../../Employee/EmployeeList').then(function (r) {
                if (r.data.status == "success") {
                    s.empList = r.data.empList;
                }
            });
        }
        else {
            h.post('~/../../Employee/EmployeeNonPlantillaList').then(function (r) {
                if (r.data.status == "success") {
                    s.empList = r.data.empList;
                }
            });
        }
    }

    //family /***********************************************************************
    s.loadFamily = function () {
        s.bDisable = true;
        s.isReadOnly = true;
        h.post('~/../../Employee/GetFamilyData', { id: s.profileData.EIC }).then(function (r) {
            if (r.data.status == "success") {
                s.famData = r.data.famData;
                s.children = r.data.children;
            }
            s.bDisable = false;
        });
    }

    s.updateFamilyBackground = function (data) {
        s.bDisable = true;
        //s.isReadOnly = false;
        h.post('~/../../Employee/UpdateFamilyBackGround', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                //s.famData = r.data.famData;
                //s.children = r.data.children;
                toastr["success"]("Updating successful!", "Success");
                s.bDisable = false;
                s.isReadOnly = true;
            }
            s.bDisable = false;
        });
    }

    //CHILD DATA / ******************************************************************
    s.addChildModal = function () {        
        angular.element('#modalChilData').modal('show');
    }

    s.saveChildData = function (data) {
        data.EIC = s.profileData.EIC;
        h.post('~/../../Employee/SubmitChildData', {data: data}).then(function (r) {
            if (r.data.status == "success") {
                angular.element('#modalChilData').modal('hide');
                toastr["success"]("Saving successful!", "Child");
            }
        });
    }

    //EDUCATIONAL BACKGROUND //*********************************************************
    s.showEducation = function () {
        h.post('~/../../Employee/GetPDSEducation', { id: s.profileData.EIC}).then(function (r) {
            if (r.data.status == "success") {
                s.educationList = r.data.educationList;
            }
        });
    }

    s.verifyEducation = function (data) {
        s.educData = data;
        angular.element('#modalEducation').modal('show');
    }
   
    s.confirmEducationData = function (data) {
        h.post('~/../../Employee/ConfirmPDSEducation', { data: data, reason : data.reason}).then(function (r) {
            if (r.data.status == "success") {
                s.educationList = r.data.educationList;
                toastr["success"]("Saving successful!", "Success");
                angular.element('#modalEducation').modal('hide');
            }
        });
    }


    //-- EDUCATIONAL BACKGROUND //*********************************************************

    s.showNonPlantilla = function () {
        h.post('~/../../Employee/EmployeeNonPlantillaList').then(function (r) {
            if (r.data.status == "success") {
                s.empList = r.data.empList;
            }
        });
    }

    s.openMigrationModal = function () {
        angular.element('#modalMigration').modal('show');
    }

    s.searchData = function (data) {
        s.bDisable = true;
        h.post('~/../../Employee/SearchHRISTempData/' + data.name).then(function (r) {
            if (r.data.status == "success") {
                s.searchList = r.data.searchList;
                s.bDisable = false;
            }
            else {
                toastr["error"]("Searching error!", "Login");
                s.bDisable = false;
            }
        });
    }

    s.submitMigration = function (id) {
        s.bDisable = true;
        h.post('~/../../Employee/MigrateEmployeeDataByID/' + id).then(function (r) {
            if (r.data.status == "success") {
                if (r.data.stat == 1) {
                    toastr["success"]("Migration successful!", "Login");
                    s.bDisable = false;
                }
                if (r.data.stat == 0) {
                    toastr["error"]("Data already exist!", "Login");
                    s.bDisable = false;
                }
                s.data.name = "";
            }
        });
    }

  

    s.viewServiceRecord = function (id) {
        h.post('~/../../Employee/SetupEmployee/' + id.EIC).then(function (r) {
            if (r.data.status == "success") {
                window.location.href = "../Employee/ServiceRecord";
            }
        });
    }

    //MODAL WORK GROUP
    s.selectWorkGroup = function (data) {
        s.data = data;
        if (s.workGroupList.length == undefined) {
            h.post('~/../../Employee/GetWorkGroupList').then(function (r) {
                if (r.data.status == "success") {
                    s.workGroupList = r.data.workGroupList;
                    angular.element('#openModalWorkGroup').modal('show');
                }
            });
        }
        else {
            angular.element('#openModalWorkGroup').modal('show');
        }
       
    }

    //UPDATE WORK GROUP
    s.updateWorkGroup = function () {
        h.post('~/../../Employee/UpdateWorkGroup', { id: s.data.EIC, code: s.data.workGroupCode, tag: s.searchText }).then(function (r) {
            if (r.data.status == "success") {
                s.empList = r.data.empList;
                angular.element('#openModalWorkGroup').modal('hide');
                toastr["success"]("Updating successful!", "Work Group");
            }
        });
    }

    s.sendToArchive = function (data) {
        h.post('~/../../Employee/ArchiveEmployeeRecord', { id: s.data.EIC}).then(function (r) {
            if (r.data.status == "success") {                        
                toastr["success"]("Updating successful!", "Archiving");
            }
        });
    }


    //************ CIVIL SERVICE ELIGIBILITY *********************************
    //EligibilityInitData

    s.eligibilityProfile = function () {
        s.isReadOnly = true;
        s.bDisable = true;
        h.post('~/../../PersonalDataSheet/EligibilityInitData', {id: s.profileData.EIC}).then(function (r) {
            if (r.data.status == "success") {
                s.eligType = r.data.eligType;
                s.empEligList = r.data.empElig;
            }
        });
    }

    //MDOAL ADD

    s.modalAddEligibility = function () {     
        s.elig.eligibilityCode = "";
        angular.element('#modalAddEligibility').modal('show');
    }

    //MODAL EDIT
    s.modalEditEligibility = function (data) {
        s.elig = data;
        s.elig.examDate = new Date(moment(data.examDate).format('YYYY,MM,DD'));
        s.elig.validityDate = new Date(moment(data.validityDate).format('YYYY,MM,DD'));
        s.isReadOnly = false;
        //$.fn.modal.Constructor.prototype.enforceFocus = function () { };
       angular.element('#modalEditEligibility').modal('show');
        //$.fn.modal.Constructor.prototype.enforceFocus = function () { };
    }

    //SUBMIT
    s.submitEligibility = function (data) {
        s.bDisable = true;
        data.EIC = s.profileData.EIC;
        //alert(data.EIC);
        h.post('~/../../PersonalDataSheet/SaveEligibility', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                s.empEligList = r.data.empElig;
                toastr["success"]("Saving successful!", "Good");
                angular.element('#modalAddEligibility').modal('hide');
            }
            else {
                toastr["error"](r.data.status, "Opps..");
            }
        });
    }

    //UPDATE
    //s.updateEligibility = function (data) {
    //    s.bDisable = true;
    //    h.post('~/../../PersonalDataSheet/UpdateEligibility', { data: data }).then(function (r) {
    //        if (r.data.status == "success") {
    //            s.empEligList = r.data.empElig;
    //            toastr["success"]("Updating successful!", "Good");
    //            angular.element('#modalEditEligibility').modal('hide');
    //        }
    //        else {
    //            toastr["error"](r.data.status, "Opps..");
    //        }
    //    });
    //}

    s.validateEligibility = function (data) {
        s.bDisable = true;
        h.post('~/../../PersonalDataSheet/ValidateEligibility', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                //s.empEligList = r.data.empElig;
                toastr["success"]("Updating successful!", "Good");
                angular.element('#modalEditEligibility').modal('hide');
            }
            else {
                toastr["error"](r.data.status, "Opps..");
            }
        });
    }

    //PDS: Training
    s.showTraining = function () {
        //GetEmpPDSTraining
        h.post('~/../../PersonalDataSheet/GetEmpPDSTraining', { id: s.profileData.EIC }).then(function (r) {
            if (r.data.status == "success") {
                s.trainingList = r.data.trainingList;
            }
            else {
                toastr["error"](r.data.status, "Opps..");
            }
        });
    }

    s.trainingAction = function (data) {
        alert(data.controlCode);
    }

    //////////////////////////////////////
    
    //modalFoxData
    s.myIDData = {};
    s.openMigrateDataID = function () {
        h.post('~/../../PersonalDataSheet/MyDataID', { id: s.profileData.EIC }).then(function (r) {
            if (r.data.status == "success") {
                s.myIDData = r.data.dataId;
                angular.element('#modalFoxData').modal('show');
            }
            else {
                toastr["error"](r.data.status, "Opps..");
            }
        });
    }

    //TRY TO DELETE
    s.deleteEligibility = function (data) {        
        swal({
            title: data.eligibilityName,
            text: "Remove eligibility now?",
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
                    h.post('~/../../PersonalDataSheet/DeleteEligibility', { data: data }).then(function (r) {
                        if (r.data.status == "success") {
                            s.empEligList = r.data.empElig;
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

    //************ CIVIL SERVICE ELIGIBILITY *********************************

    function ParseDate(input) {
        theDate = new Date(parseInt(input.substring(6, 19)));
        return theDate.toGMTString();
    }

    s.formatDate = function (date) {
        if (!date) {
            return 'N/A';
        }
        return (moment(date).format('MM/DD/YYYY'));
    };

    //************ PERSONAL DATA SHEET (PDS) *********************************

    s.printPDS = function (id) {     
        h.post('~/../../Employee/PrintPDS', { id: s.profileData.EIC, code: id }).then(function (r) {
            if (r.data.status == "success") {               
                window.open("../Reports/HRIS.aspx");
            }
        });
    }



    //************ UPDATE DATA ID *********************************

    s.updateIDNumber = function (code, value) {
        h.post('~/../../Employee/UpdateIDNumber', { id: s.profileData.EIC, code: code, value: value }).then(function (r) {
            if (r.data.status == "success") {
                s.profileData = r.data.profileData;
                toastr["success"]("Updating successful...", "Success");
            }
        });
    }


    s.showModalByCharges = function () { 
  

        if (s.fundSourceList.length == undefined) {
            h.post('~/../../RSPFundSource/GetCurrentCharges').then(function (r) {
                if (r.data.status == "success") {
                    s.fundSourceList = r.data.fundSource;
                    angular.element('#modalListByCharges').modal('show');
                }
            });
        }
        else {
            angular.element('#modalListByCharges').modal('show');
        }

    }

    s.showModalWorkGroup = function () {
        if (s.workGroupList.length == undefined) {
            h.post('~/../../Employee/GetWorkGroupList').then(function (r) {
                if (r.data.status == "success") {
                    s.workGroupList = r.data.workGroupList;
                    angular.element('#modalListByWorkGroup').modal('show');
                }
            });
        }
        else {
            angular.element('#modalListByWorkGroup').modal('show');
        }
      
        //if (s.fundSourceList.length == undefined) {
        //    h.post('~/../../RSPFundSource/WorkGroup').then(function (r) {
        //        if (r.data.status == "success") {
        //            s.fundSourceList = r.data.fundSource;
        //             angular.element('#modalListByWorkGroup').modal('show');
        //        }
        //    });
        //}
        //else {
        //     angular.element('#modalListByWorkGroup').modal('show');
        //}

    }



    s.printPreviewEmpList = function (data, code) {
        data.printCode = code;
       h.post('~/../../Employee/PrintByEmpList', {data: data}).then(function (r) {
            if (r.data.status == "success") {
                window.open("../Reports/HRIS.aspx");
            }
       });
    }

    //EMPLOYEE REGISTRATION
    s.regData = {};
    s.checkState = false;
    s.showRegistrationModal = function () {
        s.regData = {};
        s.checkState = false;
        angular.element('#modalRegistration').modal('show');
    }

    //
    s.saveRegistration = function (data) {
        s.bDisable = true;
        h.post('~/../../Employee/SaveEmployeeRegistration', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                angular.element('#modalRegistration').modal('hide');
                toastr["success"]("Registration successful...", "Success");
                s.bDisable = false;                
            }
            else {
                s.checkState = false;
                s.bDisable = false;
                toastr["error"](r.data.status, "Opps");
            }
        });
    }


    //SERVICE DATE
    s.serviceDate = {};
    s.modalServiceDate = function () {
        s.checkState = false;
        s.bDisable = true;
        h.post('~/../../Employee/GetEmployeeServiceDate', { id: s.profileData.EIC }).then(function (r) {
            if (r.data.status == "success") {
                s.serviceDate.dateLastPromoted = new Date(moment(r.data.serviceDate.dateLastPromoted).format('YYYY,MM,DD'));
                s.serviceDate.dateOrigAppointment = new Date(moment(r.data.serviceDate.dateOrigAppointment).format('YYYY,MM,DD'));
                s.bDisable = false;
                angular.element('#modalServiceDate').modal('show');
            }
            else {
                s.checkState = false;
                s.bDisable = false;
                toastr["error"](r.data.status, "Opps");
            }
        });
    }

    //update SERVICE DATA
    s.updateServiceDate = function (data) {      
        var tmp = {};
        tmp.EIC = s.profileData.EIC;
        tmp.dateOrigAppointment = data.dateOrigAppointment;
        s.bDisable = true;
        h.post('~/../../Employee/UpdateEmployeeServiceDate', { data: tmp }).then(function (r) {
            if (r.data.status == "success") {              
                s.bDisable = false;
                toastr["success"]("Update successful...", "Success");
                angular.element('#modalServiceDate').modal('hide');
                s.checkState = false;
            }
            else {
                s.checkState = false;
                s.bDisable = false;
                toastr["error"](r.data.status, "Opps");
            }
        });
    }

    
}]);