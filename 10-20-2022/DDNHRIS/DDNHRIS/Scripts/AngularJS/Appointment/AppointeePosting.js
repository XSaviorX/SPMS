
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
    "timeOut": "5000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
}

app.controller('AppointeePosting', ['$scope', '$http', '$document', function (s, h, d) {


    s.bDisable = false;

    s.incomingCount = 0;
    s.employeeList = {};
    s.publicationItemList = {};
    s.coTermPositionList = {};

    loadInitData();

    function loadInitData() {
        loadEmployeeList();
    }


    //AppointeePostingTab
    function loadEmployeeList() {
        h.post('~/../../Appointment/AppointmentPostingData').then(function (r) {
            if (r.data.status == "success") {
                s.employeeList = r.data.employeeList;
                s.publicationItemList = r.data.publicationItemList;
                s.coTermPositionList = r.data.cotermList;
            }
        });
    }
    
    //POSTING PERMANENT
    s.submitAppointeePermanent = function (data) {

        if (data.publicationItemCode == null || data.publicationItemCode == undefined || data.EIC == undefined || data.EIC == "" || data.psbDate == undefined || data.psbDate == "") {
            toastr["error"]("Please fill-up the required fields!", "Opps...");
            return;
        }

        h.post('~/../../Appointment/PostAppointeeToAppList', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                s.employeeList = r.data.employeeList;
                s.publicationItemList = r.data.publicationItemList;
                toastr["success"]("Posting successful!", "Success")
            }
        });
    }

    //POSTING COTERM
    s.submitAppointeeCoTerm = function (data) {
     
        if (data.publicationItemCode == undefined || data.EIC == undefined) {
            toastr["error"]("Please fill-up the required fields!", "Opps...");
            return;
        }

        h.post('~/../../Appointment/PostAppointeeCoTerm', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                s.formApp = {};
                s.formApp.EIC = "";
                s.employeeList = r.data.employeeList;
                s.publicationItemList = r.data.publicationItemList;
                toastr["success"]("Saving successful!", "Success");
                loadEmployeeList();
            }
        });
    }


}]);
