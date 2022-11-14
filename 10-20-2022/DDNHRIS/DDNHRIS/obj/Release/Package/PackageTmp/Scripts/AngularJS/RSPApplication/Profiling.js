app.controller('ApplicantProfiling', ['$scope', '$http', '$document', function (s, h, d) {

    s.data = {};

    s.pubItemList = {};
    s.pubItemData = {};
    s.positionData = {};

    s.applicantList = {};
    s.employeeList = {};

    s.bDisable = false;
    s.applicationData = {};
    s.outsiderList = {};
    s.pmList = {};

    s.profileList = {};
    s.profileData = {};
    s.assmntResultData = {};

    s.applicantData = {};

    s.comptAssmntTag = 0;

    s.tabId = 0;

    s.positionList = {};

    s.appType = "INSIDER";

    loadInitData();

    s.applicantType = [appTypeCode = 1, text = 'INSIDER', appTypeCode = 0, text = 'OUTSIDER']

    s.schoolLevel = [
        { schoolLevelCode: "01", schoolLevelName: 'ELEMENTARY' },
        { schoolLevelCode: "02", schoolLevelName: 'HIGH SCHOOL' },
        { schoolLevelCode: "03", schoolLevelName: 'VOCATIONAL/TRADE COURSE' },
        { schoolLevelCode: "04", schoolLevelName: 'COLLEGE' },
        { schoolLevelCode: "05", schoolLevelName: 'GRADUATE STUDIES' }
    ];

    s.triningTypeList = [
          { trainingTypeCode: "Managerial", trainingTypeName: 'Managerial' },
            { trainingTypeCode: "Technical", trainingTypeName: 'Technical' },
              { trainingTypeCode: "Supervisory", trainingTypeName: 'Supervisory' },
                 { trainingTypeCode: "Others", trainingTypeName: 'Others' },
    ];

    s.spmsSemester = [
         { code: "1", semester: '1st Semester' },
            { code: "2", semester: '2nd Semester' }
    ];

    s.spmsRating = [
          { code: "Outstanding", text: 'Outstanding' },
          { code: "Very Satisfactory", text: 'Very Satisfactory' },
          { code: "Satisfactory", text: 'Satisfactory' },
          { code: "Unsatisfactory", text: 'Unsatisfactory' },
          { code: "Poor", text: 'Poor' }
    ];

    s.spmsYear = [
        { code: 2020, text: '2020' },
        { code: 2019, text: '2019' },
        { code: 2018, text: '2018' },
        { code: 2017, text: '2017' },
        { code: 2016, text: '2016' }
    ];

    function loadInitData() {
        h.post('~/../../RSPProfiling/PublicationPositionList').then(function (r) {
            if (r.data.status == "success") {
                s.pubItemList = r.data.list;
                s.positionList = r.data.positionList;
            }
        });
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    s.showPositionData = function (data) {
        s.pubItemData = data;
        h.post('~/../../RSPProfiling/ApplicantByPosition', { id: data.publicationItemCode }).then(function (r) {
            if (r.data.status == "success") {
                s.tabId = 1;
                s.positionData = r.data.pubItemData;
                s.applicantList = r.data.applicantList;
            }
            s.bDisable = false;
        });
    }

    //ADD APPLICANT
    s.addApplicant = function (data) {
        s.pubItemData = data;
        if (s.employeeList.length == undefined) {
            s.bDisable = true;
            h.post('~/../../RSPProfiling/EmployeeApplicantList').then(function (r) {
                if (r.data.status == "success") {
                    s.employeeList = r.data.employeeList;
                    s.outsiderList = r.data.applicantList;
                    angular.element('#modalAddApplication').modal('show');
                }
                s.bDisable = false;
            });
        }
        else {
            angular.element('#modalAddApplication').modal('show');
        }

    }


    //SAVE APPLICANT
    s.saveApplicant = function (data) {
        s.bDisable = true;

        if (s.data.appTypeCode == undefined) {
            s.data.appTypeCode = 1;
        }

        data.publicationItemCode = s.pubItemData.publicationItemCode;
        data.appTypeCode = s.data.appTypeCode;
        data.EIC = data.EIC;
        data.applicantCode = data.applicantCode;
        //alert("save applicant");
        h.post('~/../../RSPProfiling/SaveApplication', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                s.applicantList = r.data.applicantList;
                angular.element('#modalAddApplication').modal('hide');
                toastr["success"]("Application added!", "Success");

                s.positionList = r.data.positionList;

            }
            else {
                toastr["error"](r.data.status, "Opps...");
            }
            s.bDisable = false;
        });
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //PROFILING
    s.openProfileEducation = function () {
        angular.element('#modalProfileEduc').modal('show');
    }


    //APPLICANT PROFILE
    s.viewApplicantProfile = function (id) {
        s.bDisable = true;
        h.post('~/../../RSPApplication/ViewApplicantProfile', { id: id }).then(function (r) {
            if (r.data.status == "success") {
                s.tabId = 2;
                s.applicationData = r.data.applicationData;
                s.profileData = r.data.profileData;
                ViewAssessmentData();
            }
            s.bDisable = false;
        });
    }



    s.addProfileItem = function () {
        s.bDisable = false;
        angular.element('#modalAddProfile').modal('show');
    }

    s.saveProfileItem = function (data) {
        s.bDisable = false;
        data.applicationCode = s.applicationData.applicationCode;
        h.post('~/../../RSPApplication/SaveProfileData', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                angular.element('#modalAddProfile').modal('hide');
            }
            s.bDisable = false;
        });
    }

    s.comptAssmntTag = 0;
    s.competency = {};
    s.viewAsessmentResult = function () {
        ViewAssessmentData();
    }

    function ViewAssessmentData() {
        s.assmntResultData = {};
        s.comptAssmntTag = 0;
        var id = s.applicationData.applicationCode;
        h.post('~/../../RSPApplication/ViewAssessmentResult', { id: id }).then(function (r) {
            if (r.data.status == "success") {
                s.assmntResultData = r.data.assmntResultData;
                s.comptAssmntTag = r.data.comptAssmntTag;
                s.competency = r.data.competency;
            }
            s.bDisable = false;
        });
    }

    s.submitRating = function (id, answer) {
        h.post('~/../../RSPApplication/SubmitRating', { applicationCode: s.applicationData.applicationCode, id: id, answer: answer }).then(function (r) {
            if (r.data.status == "success") {
                s.assmntResultData = r.data.assmntResultData;
            }
            s.bDisable = false;
        });
    }



    s.supervisors = {};
    s.supervirsorEIC = "";
    s.appUser = "";
    s.appCode = "";
    s.email = "";
    s.checkApplicantCompetency = function () {
        s.bDisable = true;
        s.appUser = "";
        s.appCode = "";
        var id = s.applicationData.applicationCode;
        var hasList = 1;
        if (s.supervisors.length == undefined) {
            hasList = 0;
        }

        h.post('~/../../RSPApplication/CheckCompetencyStatus', { id: id, hasList: hasList }).then(function (r) {
            if (r.data.status == "success") {
                s.appType = r.data.appType;
                s.appUser = r.data.appUser;
                s.appCode = r.data.appCode;
                s.email = r.data.email;
                if (hasList == 0) {
                    s.supervisors = r.data.supervisors;
                }
                angular.element('#modalCompetencyAllow').modal('show');
            }
            else {
                toastr["error"](r.data.status, "Opps...");
            }
            s.bDisable = false;
        });
    }

    s.comptAssmntAllow = function () {
        var id = s.applicationData.applicationCode;
        s.bDisable = true;
        h.post('~/../../RSPApplication/ComptAssmntTestAllow', { id: id, supervisorEIC: s.supervisorEIC, email: s.email }).then(function (r) {
            if (r.data.status == "success") {
                ViewAssessmentData();
                s.assmntResultData = r.data.assmntResultData;
                toastr["success"](r.data.status, "Success");
                angular.element('#modalCompetencyAllow').modal('hide');
                s.comptAssmntTag = code;
            }
            else {
                toastr["error"](r.data.status, "Opps...");
            }
            s.bDisable = false;
        });
    }

    s.emailStat = "";
    s.checkComptAssmntEmail = function () {
        var id = s.applicationData.applicationCode;
        s.bDisable = true;
        h.post('~/../../RSPApplication/CheckComptAssmntEMail', { id: id }).then(function (r) {
            if (r.data.status == "success") {
                s.emailStat = r.data.remarks;
                angular.element('#modalComptAssmntEmail').modal('show');                
            }
            else {
                toastr["error"](r.data.status, "Opps...");
            }
            s.bDisable = false;
        });
    }

    s.SendComptAssmntEmail = function () {
        var id = s.applicationData.applicationCode;
        s.bDisable = true;
        h.post('~/../../RSPApplication/SendEmailComptAssmntEmail', { id: id }).then(function (r) {
            if (r.data.status == "success") {
                angular.element('#modalComptAssmntEmail').modal('hide');
                toastr["success"]("Email sent!", "Good");
            }           
            else {
                toastr["error"](r.data.status, "Opps...");
            }
            s.bDisable = false;
        });
    }

    s.backToList = function () {
        s.tabId = 0;
    }

    s.backToLevel1 = function () {
        s.tabId = 1;
    }

    //s.SubmitProgram = function (data) {
    //    s.bDisable = true;
    //    h.post('~/../../RSPApplication/SubmitProgram', { data: data }).then(function (r) {
    //        if (r.data.status == "success") {
    //            s.programList = r.data.programList;
    //            angular.element('#modalAddProgram').modal('hide');
    //        }
    //        s.bDisable = false;
    //    });
    //}


    //MODAL : EDUCATION



    s.pdsEducation = {};
    s.modalProfileEducation = function () {
        h.post('~/../../RSPProfiling/ShowApplicantPDSData', { id: s.applicationData.EIC, code: s.applicationData.applicationCode }).then(function (r) {
            if (r.data.status == "success") {
                s.pdsEducation = r.data.pdsEducation;
                angular.element('#modalEducationProfile').modal('show');
            }
            s.bDisable = false;
        });
    }


    //ADD TO PROFILE LIST
    s.addEducProfileItem = function (data) {
        var datx = {};
        datx.applicationCode = s.applicationData.applicationCode;
        datx.sourceControlNo = data.controlNo;
        h.post('~/../../RSPApplication/AddEducationProfile', { data: datx }).then(function (r) {
            if (r.data.status == "success") {
                s.pdsEducation = r.data.pdsEducation;
                s.profileData = r.data.profileData;
                //angular.element('#modalEducationProfile').modal('show');
            }
            s.bDisable = false;
        });
    }

    s.pdsData = {};
    s.savePDSEntry = function (data) {
        data.EIC = s.applicationData.EIC;
        h.post('~/../../RSPApplication/SavePDSEntry', { data: data, code: s.applicationData.applicationCode }).then(function (r) {
            if (r.data.status == "success") {
                s.pdsEducation = r.data.pdsEducation;
                s.profileData = r.data.profileData;
                s.pdsData = {};
            }
            s.bDisable = false;
        });
    }

    s.removeEducItem = function (data) {
        h.post('~/../../RSPApplication/DeleteEducationItemData', { data: data, code: s.applicationData.applicationCode }).then(function (r) {
            if (r.data.status == "success") {
                s.pdsEducation = r.data.pdsEducation;
                s.profileData = r.data.profileData;
            }
            s.bDisable = false;
        });
    }




    //EDUCATION
    s.hrisPDSV2EducList = {};

    s.showHRISPDSV1 = function () {
        h.post('~/../../RSPProfiling/HRISPDSEducationList', { id: s.applicationData.EIC }).then(function (r) {
            if (r.data.status == "success") {
                s.hrisPDSV2EducList = r.data.list;
            }
            s.bDisable = false;
        });
    }

    s.migratePDSV1ToV2 = function (data) {
        s.bDisable = true;
        h.post('~/../../RSPProfiling/SavePDSV1Migration', { data: data, appCode: s.applicationData.applicationCode }).then(function (r) {
            if (r.data.status == "success") {
                s.hrisPDSV2EducList = r.data.list;
                s.pdsEducation = r.data.pdsEducation;
                //s.hrisPDSV2EducList = r.data.list;
                toastr["success"]("Importing successful!", "Success");
            }
            s.bDisable = false;
        });
    }

    //TRAINING *****************************************************************
    s.pdsTrainingList = {};
    s.openModalTraining = function () {
        s.bDisable = true;
        h.post('~/../../RSPProfiling/PDSTrainingData', { id: s.applicationData.EIC, appCode: s.applicationData.applicationCode }).then(function (r) {
            if (r.data.status == "success") {
                s.pdsTrainingList = r.data.trainingList;
                angular.element('#modalTrainingProfile').modal('show');
            }
            s.bDisable = false;
        });
    }

    s.hris1PDSTraining = {};
    s.showHRISPDSV1Training = function () {
        if (s.hris1PDSTraining.length == undefined) {
            s.bDisable = true;
            h.post('~/../../RSPProfiling/LoadHRIS1PDSTraining', { id: s.applicationData.EIC }).then(function (r) {
                if (r.data.status == "success") {
                    s.hris1PDSTraining = r.data.trainingList;
                }
                s.bDisable = false;
            });
        }
    }

    //migrage training
    s.migrateTrainingFromPDS1 = function (data) {
        s.bDisable = true;
        h.post('~/../../RSPProfiling/PerformTrainingMigration', { data: data, appCode: s.applicationData.applicationCode }).then(function (r) {
            if (r.data.status == "success") {
                s.hris1PDSTraining = r.data.pdsV1Training;
                s.pdsTrainingList = r.data.pdsTraining;
                toastr["success"]("Importing successful!", "Success");
            }
            s.bDisable = false;
        });
    }


    //add to Training Profile   
    s.addTrainingProfile = function (data) {
        s.bDisable = true;
        h.post('~/../../RSPProfiling/SaveTrainingProfile', { data: data, code: s.applicationData.applicationCode }).then(function (r) {
            if (r.data.status == "success") {
                s.pdsTrainingList = r.data.trainingList;

                s.profileData = r.data.profileData;
                toastr["success"]("Importing successful!", "Success");
            }
            s.bDisable = false;
        });
    }

    //TRAINING *****************************************************************





    //ELIGIBILITY ************************************************************** 



    s.pdsEligibility = {};
    s.openModalEligibility = function () {
        s.bDisable = true;
        h.post('~/../../RSPProfiling/LoadApplicantEligibility', { id: s.applicationData.EIC, code: s.applicationData.applicationCode }).then(function (r) {
            if (r.data.status == "success") {
                s.pdsEligibility = r.data.list;
                angular.element('#modalEligibility').modal('show');
            }
            s.bDisable = false;
        });
    }

    //ETNRY FORM
    s.eligFormTag = 1;
    s.eligibilityForm = function () {
        s.eligFormTag = 2;
    }

    s.eligFormBack = function () {
        s.eligFormTag = 1;
    }

    s.saveEligibilityProfile = function (data) {
        s.bDisable = true;
        h.post('~/../../RSPProfiling/SaveEligibilityProfile', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                s.pdsEligibility = r.data.list;
            }
            s.bDisable = false;
        });
    }

    //PERFORMANCE MANAGEMENT ***************************************************
    //s.openModalPerformanceRating = function () {
    //    angular.element('#modalPerformanceMngt').modal('show');
    //}


    s.openProfilePM = function (data) {
        s.applicationData = data;

        //SPMSIPCRRatingList



        h.post('~/../../RSPProfiling/SPMSIPCRRatingList', { id: s.applicationData.EIC, code: s.applicationData.applicationCode }).then(function (r) {
            if (r.data.status == "success") {
                s.pmList = r.data.list;
                angular.element('#modalPerformanceMngt').modal('show');
            }
            s.bDisable = false;
        });

        //h.post('~/../../RSPProfiling/ViewSPMSList', { id: data.applicationCode }).then(function (r) {
        //    if (r.data.status == "success") {
        //        s.pmList = r.data.list;
        //        angular.element('#modalPerformanceMngt').modal('show');
        //    }
        //    s.bDisable = false;
        //});
    }

    s.spmsTag = 1;
    s.spmsForm = function () {
        s.spmsTag = 2;
    }

    s.spmsFormBack = function () {
        s.spmsTag = 1;
    }


    s.addIPCRRating = function (data) {
        data.EIC = s.applicationData.EIC;
        h.post('~/../../RSPProfiling/AddIPCRRating', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                s.spmsTag = 1;
                //angular.element('#modalProfilePM').modal('hide');
            }
            s.bDisable = false;
        });
    }

    s.addPMProfile = function (data) {
        s.bDisable = true;
        h.post('~/../../RSPProfiling/AddPerformanceRatingProfile', { data: data, code: s.applicationData.applicationCode }).then(function (r) {
            if (r.data.status == "success") {
                s.pmList = r.data.list;
                //angular.element('#modalProfilePM').modal('hide');
            }
            s.bDisable = false;
        });
    }

    s.removePerformanceRating = function (data) {
        s.bDisable = true;
        h.post('~/../../RSPProfiling/RemovePerformanceRatingItem', { id: s.applicationData.EIC, data: data }).then(function (r) {
            if (r.data.status == "success") {
                toastr["success"]("Removing successful!", "Success");
                s.pmList = r.data.list;
                s.profileData = r.data.profileData;
            }
            s.bDisable = false;
        });
    }

    //**************************************************************************

    s.printProfile = function () {
        h.post('~/../../RSPApplication/PrintApplicantProfile', { id: s.applicationData.applicationCode }).then(function (r) {
            if (r.data.status == "success") {
                window.open("../Reports/HRIS.aspx");
            }
            s.bDisable = false;
        });
    }

    s.formatDateLong = function (date) {
        if (!date) {
            return '';
        }
        return (moment(date).format('MMMM DD, YYYY'));
    };


    s.formatDate = function (date) {
        if (!date) {
            return '';
        }
        return (moment(date).format('MM/DD/YYYY'));
    };


    s.modalTag = 0;
    s.modalHeading = "Select Applicant";

    s.addOutsiderApplicant = function () {
        s.modalTag = 1;
        s.applData = {};
        s.modalHeading = "New Applicant";
    }

    s.backToApplicantSelection = function () {
        s.modalTag = 0;
        s.modalHeading = "Select Applicant";
    }

    s.data = {};
    s.applData = {};
    s.registerApplicant = function (data) {
        var tempDate = new Date(moment(data.birthDate).format('YYYY,MM,DD'));
        data.birthDate = tempDate;
        h.post('~/../../RSPApplication/RegisterApplicant', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                s.outsiderList = r.data.applicantList;
                s.data.applicantCode = r.applicantCode;
                toastr["success"]("Saving successful!", "Success");
                s.modalTag = 0;
            }
            s.bDisable = false;
        });
    }

    s.selectApplicant = function (data) {
        s.applicantData = data;
        angular.element('#modalApplicantProfile').modal('show');
    }

    //SAVE AS APPOINTEE
    s.saveAppointee = function () {
        s.bDisable = true;
        h.post('~/../../RSPProfiling/SaveAppointee', { id: s.applicantData.applicationCode }).then(function (r) {
            if (r.data.status == "success") {
                s.outsiderList = r.data.applicantList;
                s.data.applicantCode = r.applicantCode;
                toastr["success"]("Saving successful!", "Success");
                s.modalTag = 0;
            }
            else {
                toastr["error"](r.data.status, "Error");
            }
            s.bDisable = false;
        });
    }
        
    //modalCreatePSBSchedule
    s.pubList = {};
    s.modalOpenPSBSchedule = function () {        
        if (s.pubList.length == undefined) {
            h.post('~/../../RSPProfiling/GetPublicationData').then(function (r) {
                if (r.data.status == "success") {
                    s.pubList = r.data.pubList;
                    angular.element('#modalCreatePSBSchedule').modal('show');
                }
                s.bDisable = false;
            });
        } else {
            angular.element('#modalCreatePSBSchedule').modal('show');
        }      
    }

    //
    s.submitPSBSchedule = function (data) {      
        h.post('~/../../RSPProfiling/SavePSBSchedule', { data: data }).then(function (r) {
            if (r.data.status == "success") {
                s.positionList = r.data.positionList;
                toastr["success"]("Saving successful!", "Success");
                angular.element('#modalCreatePSBSchedule').modal('hide');
            }
            s.bDisable = false;
        });
    }


}]);