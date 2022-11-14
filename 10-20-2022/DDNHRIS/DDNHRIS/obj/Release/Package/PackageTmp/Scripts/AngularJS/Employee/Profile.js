app.controller('EmployeeProfile', ['$scope', '$http', '$document', function (s, h, d) {

    s.data = {};
    s.searchList = {};
    s.dobText = "";
    s.tempDataHolder = {};
    s.isReadOnly = true;

    s.civilStatus = [{ civilStatCode: 'SINGLE', civilStatName: 'SINGLE' }, { civilStatCode: 'MARRIED', civilStatName: 'MARRIED' }, { civilStatCode: 'SEPARATED', civilStatName: 'SEPARATED' }];
    s.sexList = [{ code: 'MALE', name: 'MALE' }, { code: 'FEMALE', name: 'FEMALE' }];

    s.bDisable = false;
    var eligType = {};
    var empEligList = {};
    s.elig = {};
    s.isLoading = false;

    

    s.eligibilityCode = "";

    loadInitData();

    function loadInitData() {
        h.post('~/../../Employee/ProfileData').then(function (r) {
            if (r.data.status == "success") {
                s.data = r.data.empData;                
                //s.tempDataHolder = r.data.tempData;
                s.data.birthDate = new Date(moment(s.data.birthDate).format('YYYY,MM,DD'));
                s.dobText = new Date(moment(s.data.birthDate).format('YYYY,MM,DD'));
            }
        });
    }

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


    s.enableText = function (id) { 
        s.data.birthDate = new Date(moment(s.data.birthDate).format('YYYY,MM,DD'));
        if (id == 1) {
            s.isReadOnly = false;
            toastr["info"]("Edting is now allowed!", "Info");
        }
        else {
            s.isReadOnly = true;           
        }
    }

    s.cancelEditing = function () { 
        var temp = s.tempDataHolder;       
        s.data = temp;
        s.isReadOnly = true;      
    }
    
    s.updateProfile = function (data) {
        s.bDisable = true;
        h.post('~/../../Employee/ProfileUpdate', { data: s.data }).then(function (r) {
            if (r.data.status == "success") {                
                s.bDisable = false;
                s.isReadOnly = true;
                toastr["success"]("Updating Successful!", "Good");
            }
        });
    }
    

    //************ CIVIL SERVICE ELIGIBILITY *********************************
    //EligibilityInitData
       
    s.eligibilityProfile = function () {
        s.bDisable = true;
        h.post('~/../../PersonalDataSheet/EligibilityInitData').then(function (r) {
            if (r.data.status == "success") {
                s.eligType = r.data.eligType;
                s.empEligList = r.data.empElig;                
            }
        });
    }
    
    //MDOAL ADD
   
    s.modalAddEligibility = function () {
        s.elig = {};
        s.elig.eligibilityCode = "";      
        angular.element('#modalAddEligibility').modal('show');
    }

    //MODAL EDIT
    s.modalEditEligibility = function (data) {       
        s.elig = data;
        s.elig.examDate = new Date(moment(data.examDate).format('YYYY,MM,DD'));
        s.elig.validityDate = new Date(moment(data.validityDate).format('YYYY,MM,DD'));   
        s.isReadOnly = false;
        $.fn.modal.Constructor.prototype.enforceFocus = function () { };
        angular.element('#modalEditEligibility').modal('show');
        $.fn.modal.Constructor.prototype.enforceFocus = function () { };
    }


    //SUBMIT
    s.submitEligibility = function (data) {
        s.bDisable = true;
      
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
    s.updateEligibility = function (data) {
        s.bDisable = true;
        h.post('~/../../PersonalDataSheet/UpdateEligibility', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                s.empEligList = r.data.empElig;
                toastr["success"]("Updating successful!", "Good");
                angular.element('#modalEditEligibility').modal('hide');
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



}]);
 