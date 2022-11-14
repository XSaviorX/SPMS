app.controller('PSBBEI', ['$scope', '$http', '$document', function (s, h, d) {
        
    s.positionList = {};
    s.pubItemData = {};
    s.applicantList = {};
    s.applicationData = {};
    s.tabId = 0;

    loadInitData();

    s.value1 = 0;
    s.min1 = 0;
    s.max1 = 30;

    s.value2 = 0;
    s.min2 = 0;
    s.max2 = 20;

    s.value3 = 0;
    s.min3 = 0;
    s.max3 = 20;

    s.value4 = 10;
    s.min4 = 0;
    s.max4 = 15;

    s.value5 = 5;
    s.min5 = 0;
    s.max5 = 10;


    function loadInitData() {       
        h.post('~/../../PHRMPSB/GetPSBSchedule').then(function (r) {
            if (r.data.status == "success") {
                s.positionList = r.data.list;
            }
        });
    }

    s.formatDateLong = function (date) {
        if (!date) {
            return '';
        }
        return (moment(date).format('MMMM DD, YYYY'));
    };

    s.showPositionData = function (data) {
        s.tabId = 1;
        s.pubItemData = data;
        s.applicantList = data.applicantList;
    }

    s.backToMain = function () {
        s.tabId = 0;
    }
     
    s.backToApplicants = function () {
        s.tabId = 1;
    }
     
    s.showRatingEntry = function (data) {        
        s.applicationData = data;
        angular.element('#modalRating').modal('show');        
    }

    s.onChangeRating = function (rate) {
        //alert(rate);
    }


    //function ParseDate(input) {
    //    theDate = new Date(parseInt(input.substring(6, 19)));
    //    return theDate.toGMTString();
    //}

    //s.formatDate = function (date) {
    //    if (!date) {
    //        return 'N/A';
    //    }
    //    return (moment(date).format('MM/DD/YYYY'));
    //};

     

}]);
