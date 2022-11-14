app.controller('Applicant', ['$scope', '$http', '$document', function (s, h, d) {

    s.data = {};
    s.applicant = {};
    s.applicantData = {};
    s.bDisable = false;

    //LOAD DATA 
    loadInitData();

    function loadInitData() {
        h.post('~/../../RSPUtility/GetApplicantList').then(function (r) {
            if (r.data.status == "success") {
                s.applicant = r.data.applicant;
            }
        });
    }

    s.viewApplicantData = function (data) {
        s.applicantData = data;
        s.bDisable = true;
        h.post('~/../../RSPUtility/GetApplicantData', { id: s.applicantData.applicantCode }).then(function (r) {
            if (r.data.status == "success") {
                s.data = r.data.applicantData;
                angular.element('#modalApplicantData').modal('show');
                s.bDisable = false;
                //s.applicant = r.data.applicant;
            }
        });
    }

    s.updateApplicantData = function (data) {
        h.post('~/../../RSPUtility/UpdateApplicantData', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                angular.element('#modalApplicantData').modal('hide');
                s.bDisable = false;
                toastr["success"]("Applicant information updated!", "Success");
            }
            else {
                toastr["error"](r.data.status, "Opps...");
            }
        });
    }


    s.formatDate = function (date) {
        if (!date) {
            return 'N/A';
        }
        return (moment(date).format('MM/DD/YYYY'));
    };


}]);