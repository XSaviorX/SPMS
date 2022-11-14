app.controller('Publication', ['$scope', '$http', '$document', function (s, h, d) {

    s.data = {};
    s.pubList = {};
    s.pubItemList = {};

    s.pubData = {};
    s.positionVacant = {};

    s.bDisable = false;
    s.pubDate = {};
    s.pubDateText = "";
    s.tab = 1;
    s.checkChanges = false;
    s.expiredCount = 0;

    //LOAD DATA
    loadInitData();

    function loadInitData() {
        h.post('~/../../RSPPublication/PublicationInitData').then(function (r) {
            if (r.data.status == "success") {
                s.pubList = r.data.pubList;
            }
        });
    }

    s.setTab = function (id) {
        s.tab = id;
    }

    //MODAL NEW PUBLICATION
    s.modalCreatePublication = function () {
        angular.element('#modalPubCreate').modal('show');
    }
    //SAVE PUBLICATGION
    s.createPublication = function (data) {      
        s.bDisable = true;
        h.post('~/../../RSPPublication/SavePublication', { data: data}).then(function (r) {
            if (r.data.status == "success") {
                //s.pubList = r.data.pubList;
                s.tab = 2;
                s.pubDateText = r.data.pubDate;
                angular.element('#modalPubCreate').modal('hide');
                toastr["success"]("New publication created", "Success");
            }
            else {
                toastr["error"]("Unable to create!", "Opps");
            }
            s.bDisable = false;
        });
    }

    s.showPublicationData = function (data) {
        s.pubData = data;      
        s.bDisable = true;
        h.post('~/../../RSPPublication/PublicationData', { id: data.publicationCode }).then(function (r) {
            if (r.data.status == "success") {
                s.data = r.data.pubData;
                s.data.CSCPostedDate = new Date(moment(r.data.pubData.CSCPostedDate).format('YYYY,MM,DD'));
                s.data.CSCClosingDate = new Date(moment(r.data.pubData.CSCClosingDate).format('YYYY,MM,DD'));
                s.pubItemList = r.data.pubItemList;
                s.pubDateText = r.data.pubDate;
                s.tab = 2;
            }
            s.bDisable = false;
        });
    }

    s.changeChecker = function()
    {
        s.checkChanges = true;       
    }

    //SAVE CHANGES
    s.saveDataChanges = function ()
    {        
        s.bDisable = true;
        s.pubData.CSCPostedDate = s.data.CSCPostedDate;
        s.pubData.CSCClosingDate = s.data.CSCClosingDate;
        h.post('~/../../RSPPublication/UpdatePublicationData', { data: s.pubData }).then(function (r) {
            if (r.data.status == "success") {
                toastr["success"]("Updating successful!", "Success");
                s.checkChanges = false;
            }
            else {
                toastr["error"]("Error updating...", "Opps");
            }           
            s.bDisable = false;
        });
    }

    s.formatDate = function (date) {
        if (!date) {
            return '';
        }
        return (moment(date).format('MMMM DD, YYYY'));
    };


    //MODAL VACANT POSITIONS
    s.modalVacant = function () {
        //positionVacant
        s.bDisable = true;
        if (s.positionVacant.length == undefined) {
            h.post('~/../../RSPPublication/PlantillaVacant').then(function (r) {
                if (r.data.status == "success") {
                    s.positionVacant = r.data.vacantList;
                    s.expiredCount = r.data.expiredCount;
                    angular.element('#modalVacantPosition').modal('show');
                }
                s.bDisable = false;
            });
        }
        else {
            angular.element('#modalVacantPosition').modal('show');
            s.bDisable = false;
        }       
    }

    //addToPublication
    s.addToPublication = function (data) {
        h.post('~/../../RSPPublication/AddPositionToPub', { id: s.pubData.publicationCode, code: data.plantillaCode }).then(function (r) {
            if (r.data.status == "success") {
                toastr["success"](data.positionTitle + " added!", "Success");
                //angular.element('#modalVacantPosition').modal('hide');
            }
            s.bDisable = false;
        });

    }

    s.addExpiredToList = function () {
        //SaveExpiredPublication
        s.bDisable = true;
        h.post('~/../../RSPPublication/SaveExpiredPublication', { id: s.pubData.publicationCode }).then(function (r) {
            if (r.data.status == "success") {
                angular.element('#modalVacantPosition').modal('hide');
            }
            s.bDisable = false;
        });
    }

    //PRINT PUBLICATION    
    s.printPublication = function () {
        s.bDisable = true;
        h.post('~/../../RSPPublication/PrintPublication', { id: s.pubData.publicationCode }).then(function (r) {
            if (r.data.status == "success") {
                window.open("../Reports/HRIS.aspx");
            }
            s.bDisable = false;
        });
    }

    //PRINT PUBLICATION    
    s.printPublicationJD = function () {
        s.bDisable = true;
        h.post('~/../../RSPPublication/PrintPublicationJD', { id: s.pubData.publicationCode }).then(function (r) {
            if (r.data.status == "success") {
                window.open("../Reports/HRIS.aspx");
            }
            s.bDisable = false;
        });
    }

    s.isReadOnly = true;
    s.positionQS = {};
    s.positionData = {};
    s.openPositionQS = function (data) {
        s.bDisable = true;
        s.positionData = data;
        h.post('~/../../RSPPublication/ViewPositionQS', { id: data.plantillaCode }).then(function (r) {
            if (r.data.status == "success") {
                s.positionQS = r.data.QSData;
                angular.element('#modalPositionQS').modal('show');              
            }
            s.bDisable = false;
        });
    }


    s.setEditMode = function (id) {
        if (id == 1) {
            s.isReadOnly = false;
            toastr["info"]("Editing is now allowed!", "Info");
        }
        else {
            s.isReadOnly = true;
        }
    }
    
    s.updateQStandard = function (data)
    {
        s.bDisable = true;
        h.post('~/../../RSPPublication/UpdatePositionQS', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                s.isReadOnly = true;
                toastr["success"]("Updating successful!", "Success");
            }
            s.bDisable = false;
        });
    }

    s.jdTab = 0;
    s.positionJD = {};
    s.openModalJobDesc = function (data) {      
        s.bDisable = true;
        s.positionData = data;
        h.post('~/../../RSPPublication/ViewPositionJobDesc', { id: data.plantillaCode }).then(function (r) {
            if (r.data.status == "success") {
                s.positionJD = r.data.list;
                angular.element('#modalJobDescription').modal('show');
            }
            s.bDisable = false;
        });
    }

    s.deletePositionItem = function (data) {
        data.plantillaCode = s.positionData.plantillaCode;
        h.post('~/../../RSPPublication/DeletePublicationItem', { id: data.publicationItemCode }).then(function (r) {
            if (r.data.status == "success") {
                s.pubItemList = r.data.pubItemList;
                toastr["success"]("Deleting successful!", "Success");
            }
            s.bDisable = false;
        });
    }


    s.editJobDesc = function (data) {
        s.jdTab = 1;
    }


    s.AddJobDesc = function (data) {
        s.jdTab = 1;
    }
   
    s.cancelJobDescAdding = function (data) {
        s.jdTab = 0;
    }
    
    s.saveJobDescription = function (data) {
        data.plantillaCode = s.positionData.plantillaCode;
        h.post('~/../../RSPPublication/SaveJobDescription', {  data: data }).then(function (r) {
            if (r.data.status == "success") {
                s.positionJD = r.data.list;
                s.jdTab = 0;
                angular.element('#modalJobDescription').modal('show');
                toastr["success"]("Saving successful!", "Success");
            }
            s.bDisable = false;
        });
    }


}]);