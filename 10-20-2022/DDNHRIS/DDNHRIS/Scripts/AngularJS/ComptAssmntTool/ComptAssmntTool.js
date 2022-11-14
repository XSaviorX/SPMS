$(document).ready(function () {
    toastr.options = {
        "closeButton": true, "progressBar": true, "positionClass": "toast-top-right", "showDuration": "400", "hideDuration": "1000", "timeOut": "1000", "showEasing": "swing", "hideEasing": "linear", "showMethod": "fadeIn", "hideMethod": "fadeOut"
    }
});

app.controller('ComptAssmntTool', ['$scope', '$http', '$document', function (s, h, d) {

    s.comptPositionList = {};
    s.positionData = {};

    s.newComp = {};
    s.titleComp = {};

    s.addLevelComp = {};
    s.levelComp = {};

    s.yeaR = 2021;

    s.coreTitle = {};


    s.techCommon = {};
    s.techOffice = {};

    s.bDisable = false;
    s.disabled = false;
    s.myTechTag = false;
    s.myLeadTag = false;

    s.plantilla = {};
    s.isLoading = false;


    s.comtPosGroupTag = 1;

    s.departmentData = {};
    s.departmentData.departmentCode = "OC191015134636261001";
    s.departmentData.shortDepartmentName = "PHRMO";

    s.tabNo = 1;

    //STANDARD
    s.standards = [
             { value: 1, standardName: 'Basic (1)' }, { value: 2, standardName: 'Intermediate (2)' }, { value: 3, standardName: 'Advance (3)' },
             { value: 4, standardName: 'Superior (4)' }, { value: 0, standardName: 'N/A' }
    ];
    
    s.departmentList = [
             { departmentCode: "OC191015134332380001", shortDepartmentName: 'PGO' },
             { departmentCode: "OC191015134453048001", shortDepartmentName: 'PADO' },
             { departmentCode: "OC191015134636261001", shortDepartmentName: 'PHRMO' },
             { departmentCode: "OC191015134702339001", shortDepartmentName: 'PICKMO' },
             { departmentCode: "OC191015152447314001", shortDepartmentName: 'PPDO' },
             { departmentCode: "OC191015152503876001", shortDepartmentName: 'PGSO' },
             { departmentCode: "OC191015152544060001", shortDepartmentName: 'PBO' },
             { departmentCode: "OC191015152600833001", shortDepartmentName: 'PACCO' },
             { departmentCode: "OC191017163038011001", shortDepartmentName: 'PLO' },
             { departmentCode: "OC191017163348136001", shortDepartmentName: 'PTO' },
             { departmentCode: "OC191017163416304001", shortDepartmentName: 'PASSO' },
             { departmentCode: "OC191017163920527001", shortDepartmentName: 'PHO' },
             { departmentCode: "OC191017163949719001", shortDepartmentName: 'PSWDO' },
             { departmentCode: "OC191017164019805001", shortDepartmentName: 'PAGRO' },
             { departmentCode: "OC191017164040738001", shortDepartmentName: 'PVO' },
             { departmentCode: "OC191017164103801001", shortDepartmentName: 'PENRO' },
             { departmentCode: "OC191017164206284001", shortDepartmentName: 'PEO' },
             { departmentCode: "OC191017164319324001", shortDepartmentName: 'PEEDO' },
             { departmentCode: "OC191017164344755001", shortDepartmentName: 'PSYDO' },
             { departmentCode: "OC191017164437082001", shortDepartmentName: 'VGO' },
             { departmentCode: "OC191017164422254001", shortDepartmentName: 'SPO' },
             { departmentCode: "OC191017164406966001", shortDepartmentName: 'OSS' }
    ];

    s.standardID = [];

    loadPositionList();

    function loadPositionList() {
        h.post('~/../../ComptAssessmentTool/ComptPositionByOffice', { id: s.departmentData.departmentCode }).then(function (r) {
            if (r.data.status == "success") {
                s.comptPositionList = r.data.comptPositionList;
            }
        });
    }


    //LOAD POSITION STANDARD
    s.loadComptPositionStandard = function (data) {
        LoadStandard(data);
    }

    s.viewPlantillaPosition = function () {
        if (s.plantilla.length == undefined) {
            s.isLoading = true;
            viewPlantilla();
        }
    }
   
    s.modalSelectDepartment = function () {     
         $('#modalDepartment').modal('show');
    }
 
    s.selectedDepartment = function (data) {
        var id = s.departmentData.departmentCode;
       
        var itemData = s.departmentList.filter(function (item) {
            return item.departmentCode === id;
        })[0];

     
        s.departmentData = itemData;
       

        s.plantilla = {};
        loadPositionList();
        //viewPlantilla();
        $('#modalDepartment').modal('hide');
    }
    //CREATE COMPT POSITION
    s.openModalCreateComptPosition = function () {
        s.comtPosGroupTag = 2;
        $('#modalComptPositionGroup').modal('show');
    }

    function viewPlantilla() {
        var id = s.departmentData.departmentCode;              
            h.post('~/../../Plantilla/ShowDepartmentPlantillaById', { id: id }).then(function (r) {
                if (r.data.status == "success") {
                    s.plantilla = r.data.plantilla;
                }
                s.bDisable = false;
                s.isLoading = false;
            });
          
    }

    s.myLeadTag = false;
    s.myTechTag = false;
     
    function LoadStandard(data) {
       
        s.positionData = data;
        var id = data.comptPositionCode;
        s.comptPositionCode = id;

        s.tabNo = 1;

        h.post('~/../../ComptAssessmentTool/LoadPositionStandard', { data: s.positionData }).then(function (result) {
            if (result.data.status == "success") {

                s.coreTitle = result.data.coreList;
                s.leadTitle = result.data.leadList;
                s.techTitle = result.data.techList;

                if (s.leadTitle.length > 0) {
                    s.myLeadTag = true;
                }
                else {
                    s.myLeadTag = false;
                }
                if (s.techTitle.length > 0) {
                    s.myTechTag = true;
                }
                else {
                    s.myTechTag = false;
                }
                //LOADING STANDARD
                angular.forEach(result.data.gData, function (item) {
                    s.standardID[item.comptCode] = item.standardCode;
                });

            }
        });
    }


    //SUBMIT Compt Standard
    s.competencyStandard = function (standardCode, comptCode) {

        if (s.positionData.comptPositionCode == null || s.positionData.comptPositionCode.length == 0) {
            s.standardCode = "";
            alert("Please Select Position First!")
        }
        else {
            s.stanData = {};
            s.stanData.comptPositionCode = s.positionData.comptPositionCode;
            s.stanData.comptCode = comptCode;
            s.stanData.positionCode = s.positionData.positionCode;
            s.stanData.subPositionCode = s.positionData.subPositionCode;
            s.stanData.standardCode = standardCode;
            h.post('~/../../ComptAssessmentTool/SaveSetStandard', { data: s.stanData }).then(function (r) {
                if (r.data.status == "success") {
                    toastr["success"](r.data.msg, "Good");
                }
            });
        }
    }

    s.openTechComptList = function () {
        h.post('~/../../Competency/GetTechComptList', { data: s.positionData }).then(function (result) {
            if (result.data.status == "success") {
                s.techCommon = result.data.techCommon;
                s.techOffice = result.data.techOffice;
                $('#modalTechComptList').modal('show');
            }
        });
    }


    s.openModalLeadership = function () {     
        h.post('~/../../ComptAssessmentTool/CheckLeadTech', { data: s.positionData }).then(function (result) {
            if (result.data.status == "success") {
                $('#modalAddLeadership').modal('show');
            }
        });
    }

    s.addTechCompetency = function (id) {
        s.bDisable = true;
        h.post('~/../../Competency/AddTechCompetency', { pos: s.positionData, comptCode: id }).then(function (result) {
            if (result.data.status == "success") {
                s.techCommon = result.data.techCommon;
                s.techOffice = result.data.techOffice;
                LoadStandard(s.positionData);
                s.bDisable = false;
            }
        });
        s.bDisable = false;
    }

    //Add LeadershipCompetency To Standard
    //s.AddLeadershipCompt = function () {
    //    alert("")
    //    s.bDisable = true;
    //    h.post('~/../../Competency/AddLeadershipToStandard', { pos: s.positionData }).then(function (result) {
    //        if (result.data.status == "success") {
    //            LoadStandard(s.positionData);
    //            $('#modalAddLeadership').modal('hide');
    //            s.bDisable = false;
    //        }
    //    });
    //    s.bDisable = false;
    //}


    //s.openModalLeadership = function () {     
    //    h.post('~/../../Competency/CheckLeadTech', { data: s.positionData }).then(function (result) {
    //        if (result.data.status == "success") {
    //            //if (result.data.leadTag > 0) {
    //            //    s.myLeadTag = true;
    //            //}
    //            //else {
    //            //    s.myLeadTag = false;
    //            //}

    //            //if (result.data.techTag > 0) {
    //            //    s.myTechTag = true;
    //            //}
    //            //else {
    //            //    s.myTechTag = false;
    //            //}
    //            $('#modalAddLeadership').modal('show');
    //        }
    //    });
    //}



    //REMOVE Leadership Competency from Standard
    s.RemoveLeadershipCompt = function () {
        s.bDisable = true;
        h.post('~/../../ComptAssessmentTool/RemoveLeadershipFromStandard', { pos: s.positionData }).then(function (result) {
            if (result.data.status == "success") {
                LoadStandard(s.positionData);
                $('#modalAddLeadership').modal('hide');
                s.bDisable = false;
                toastr["success"]("Removing successful!", "Good");
            }
            else {
                toastr["error"]("Error removing leadership", "Opps...");
            }
        });
    }

    s.changeTab = function (i) {
        s.tabNo = i;
    }

    s.comptData = {};
    s.plantillaData = {};
    s.openModalSelectCompetency = function (data) {
        s.plantillaData = data;
        s.comptData.comptPositionTitle = data.positionTitle;
        s.comptData.salaryGrade = data.salaryGrade;
        $('#modalComptPositionGroup').modal('show');
        //h.post('~/../../Competency/RemoveLeadershipFromStandard', { pos: s.positionData }).then(function (result) {
        //    if (result.data.status == "success") {
        //        LoadStandard(s.positionData);
        //        $('#modalAddLeadership').modal('hide');
        //        s.bDisable = false;
        //    }
        //});
    }

    s.assmntGroupCode = "";
    s.saveComptPositionGroup = function (data) {
        s.bDisable = true;
        data.comptPositionTitle = data.comptPositionTitle;
        data.assmntGroupCode = s.departmentData.departmentCode;
        h.post('~/../../ComptAssessmentTool/SaveCompetencyPositionGroup', { data: data }).then(function (result) {
            if (result.data.status == "success") {                
                s.comtPosGroupTag = 1;
                loadPositionList();
                s.bDisable = false;
            }
        });
    }

    s.mapComptPosition = function (data) {
        data.plantillaCode =  s.plantillaData.plantillaCode;
      
        h.post('~/../../ComptAssessmentTool/SaveCompetencyMapping', { data: data }).then(function (result) {
            if (result.data.status == "success") {
                viewPlantilla();
                $('#modalComptPositionGroup').modal('hide');
                toastr["success"]("Mapping successful!", "Good");
                s.bDisable = false;
            }
        });
    }

    s.removeComptMapping = function (data) {
        s.bDisable = true;
        data.comptPositionCode = data.eligibilityName;
        data.plantillaCode = data.plantillaCode;
    
        h.post('~/../../ComptAssessmentTool/RemoveCompetencyMapping', { data: data }).then(function (result) {
            if (result.data.status == "success") {
                toastr["success"]("Unmapping successful!", "Good");
                s.comtPosGroupTag = 1;
                viewPlantilla();
                s.bDisable = false;
            }
        });
    }


    s.plantillaData = {};
    s.showComptForm = function () {
        s.comtPosGroupTag = 2;
    }

    s.backToComptList = function () {
        s.comtPosGroupTag = 1;
    }


    //function addStandards(gData) {
    //    angular.forEach(gData, function (item) {
    //        s.standardID[item.comptCode] = item.standardCode;
    //    });
    //}

    //function getCompetencyData() {

    //    //s.designation = ""; s.recNo = ""; s.statusName = "";
    //    h.post('GetCompetencyData', { data: s.positionData }).then(function (d) {

    //        s.coreTitle = d.data.coreTitle;
    //        angular.forEach(s.coreTitle, function (item) {
    //            s.standardID[item.comptCode] = "";
    //        });

    //        s.leadTitle = d.data.leadTitle;
    //        angular.forEach(s.leadTitle, function (item) {
    //            s.standardID[item.comptCode] = "";
    //        });

    //        s.techTitle = d.data.techTitle;
    //        angular.forEach(s.techTitle, function (item) {
    //            s.standardID[item.comptCode] = "";
    //        });
    //    })
    //}



    //function clearStandards() {
    //    angular.forEach(s.coreTitle, function (item) {
    //        s.standardID[item.comptCode] = "";
    //    });
    //    angular.forEach(s.leadTitle, function (item) {
    //        s.standardID[item.comptCode] = "";
    //    });
    //}



    //s.LeadTechTag = function () {
    //    h.post('~/../../Competency/UpdateLeadTech', { pos: s.positionData, leadTag: s.LeadTag, techTag: s.TechTag }).then(function (result) {
    //        if (result.data.result == "success") {
    //            $('#modalCoreLeadTech').modal('hide');
    //        }
    //    });
    //}


    ////////////////////////////////////////////////////////////
    s.checkings = function (of) {
        s.bDisable = true;
        h.post('CheckingData', { subPositionCode: s.subPositionCode, positionCode: of.positionCode }).then(function (d) {
            clearStandards();
            addStandards(d.data.gData)
            s.bDisable = false;
        });
        s.bDisable = false;
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //GET PLANTILLA STRUCTURE



    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    s.toolGroup = 'TECH';

    s.loadCompetencyByGroup = function (id) {
        if (id == 1) {
            loadCoreLeadCompt(s.toolGroup);
        }

    }

    s.comptList = {};
    function loadCoreLeadCompt(id) {
        //var id = s.toolGroup;
        h.post('~/../../ComptAssessmentTool/LoadCompetencyByGroup', { id: id, code: s.departmentData.departmentCode}).then(function (r) {
            if (r.data.status == "success") {
                s.comptList = r.data.comptList;
            }
        }, function (r) {
            console.log(r.stat);
        });
    }

    s.loadIndicators = function (of) {     
        competency(of);
    }

    s.KBIList = {};
    s.KBIProg = {};
    s.comptTool = "";

    function competency(of) {
        s.li = '';
        s.comptTool = of.comptTool;
        s.comptCode = of.comptName;
        s.coreDesc = of.comptDesc;
        s.competency = of;
     
        h.post('~/../../ComptAssessmentTool/CoreKBI/', { comptCode: of.comptCode, tool: of.comptTool }).then(function (r) {
            if (r.data.status == "success") {             
             
                if (r.data.comptTool == "3") {              
                    s.KBIList = r.data.KBIList;
                    s.KBIProg = {};
                }
                else if (r.data.comptTool != "3") {                   
                    s.KBIProg = r.data.KBIList;
                    s.KBIList = {};
                }
            }
        });
    }

    s.selectToolGroup = function () {
        angular.element('#modalGroupSelect').modal('show');
    }

    s.setToolGroup = function (id) {
        s.toolGroup = id;
        if (id == "CORE") {
            s.toolGroupName = "CORE";
        }
        else if (id == "LEAD") {
            s.toolGroupName = "LEADERSHIP";
        }
        else if (id == "TECH") {
            s.toolGroupName = "TECHNICAL";
        }
        s.comptTool = "";
        s.coreList = {};
        loadCoreLeadCompt(id);
        angular.element('#modalGroupSelect').modal('hide');

    }
 

    s.showOnSave = true;
    s.showOnUpdate = false;

    s.addCompetency = function () {       
        s.showOnSave = true;
        s.showOnUpdate = false;
        s.modalTitle = "Add Competency";
        angular.element('#modalCompetencyAdd').modal('show');
    }

    /* Save Competency */
    s.saveCompetency = function (data) {       
        s.data.comptGroupCode = s.toolGroup;
        s.data.assmntGroupCode = s.departmentData.departmentCode;
        h.post('~/../../ComptAssessmentTool/SaveCompetency', { data: data }).then(function (result) {
            if (result.data.status == "success") {
                s.comptList = result.data.comptList;
                //loadCore();
                angular.element('#modalCompetencyAdd').modal('hide');
            }
        });
    }


    s.addKBIProg = function (id) {     
        s.showOnSave = true;
        s.showOnUpdate = false;
        s.modalTitle = "Add :";
        s.PKBI = {};
        angular.element('#modalProgAdd').modal('show');
    }

    s.MKData = {};
    /* Add Main Indicator */
    s.addBehaviorMain = function (id) {
      
        s.showOnSave = true;
        s.showOnUpdate = false;
        s.MKData.comptName = s.competency.comptName;
        s.MKData.KBI = "";
        s.modalTitle = "Add Main Indicator";
        angular.element('#modalKBIMainAdd').modal('show');
    }

    //SAVE MAIN INDICATOR
    s.saveMainIndicator = function (data) {       
        s.MKData.comptCode = s.competency.comptCode;
        s.MKData.conptName = data.KBI;
         
        h.post('~/../../ComptAssessmentTool/SaveMainKBI', { data: s.MKData }).then(function (result) {
            if (result.data.status == "success") {
                s.MKData = {};
                competency(s.competency);
                angular.element('#modalKBIMainAdd').modal('hide');
            }
        });
    }

    /* EDIT Main Indicator */
    s.editIndicatorMain = function (id) {
        s.showOnSave = false;
        s.showOnUpdate = true;
        s.modalTitle = "Edit Main Indicator";
        s.checked = false;
        h.post('~/../../ComptAssessmentTool/GetMainKBIData', { id: id }).then(function (result) {
            if (result.data.status == "success") {
                s.MKData = result.data.MKData;
                s.MKData.comptName = s.competency.comptName;
                angular.element('#modalKBIMainAdd').modal('show');
            }
        });
    }

    s.updateMainIndicator = function (data) {
        s.MKData.comptCode = s.competency.comptCode;
        s.MKData.conptName = data.KBI;
        h.post('~/../../ComptAssessmentTool/UpdateMainKBI', { data: s.MKData }).then(function (result) {
            if (result.data.status == "success") {
                s.MKData = {};
                competency(s.competency);
                angular.element('#modalKBIMainAdd').modal('hide');
            }
        });
    }

    s.addIndicator = function (id) {
        s.showOnSave = true;
        s.showOnUpdate = false;
        s.modalTitle = "Add Behavioral Indicator";
 
        h.post('~/../../ComptAssessmentTool/GetMainKBIData', { id: id }).then(function (result) {
            if (result.data.status == "success") {
                s.MKData = result.data.MKData;
                angular.element('#modalKBIAdd').modal('show');
            }
        });
    }

    s.editIndicator = function (id) {
        s.showOnSave = false;
        s.showOnUpdate = true;
        s.modalTitle = "Edit Behavioral Indicator";
        s.checked = false;
        h.post('~/../../ComptAssessmentTool/GetKBIData', { id: id }).then(function (result) {
            if (result.data.status == "success") {
                s.MKData.KBI = result.data.title;
                s.KData = result.data.MKData;
                angular.element('#modalKBIAdd').modal('show');
            }
        });
    }

    s.saveIndicator = function (data) {
        s.KData.MKBICode = s.MKData.KBICode;
        s.KData.KBI = data.KBI;
        h.post('~/../../ComptAssessmentTool/SaveKBI', { data: s.KData }).then(function (result) {
            if (result.data.status == "success") {
                competency(s.competency);
                s.KData = {};
                angular.element('#modalKBIAdd').modal('hide');
            }
        });
    }

    s.updateIndicator = function (data) {
        s.KData.MKBICode = s.MKData.KBICode;
        s.KData.KBI = data.KBI;
        h.post('~/../../ComptAssessmentTool/UpdateKBI', { data: s.KData }).then(function (result) {
            if (result.data.status == "success") {
                competency(s.competency);
                s.KData = {};
                angular.element('#modalKBIAdd').modal('hide');
            }
        });
    }

    s.selectToolGroup = function () {
        angular.element('#modalGroupSelect').modal('show');
    }


    s.savePKBI = function (data) {
        s.PKBI.comptCode = s.competency.comptCode;
        s.PKBI.comptLevel = data.comptLevel;
        s.PKBI.basic = data.basic;
        s.PKBI.intermediate = data.intermediate;
        s.PKBI.advance = data.advance;
        s.PKBI.superior = data.superior;
        h.post('~/../../ComptAssessmentTool/SavePKBI', { data: s.PKBI }).then(function (result) {
            if (result.data.status == "success") {
                competency(s.competency);
                angular.element('#modalProgAdd').modal('hide');
            }
        });
    }

    s.editKBIProg = function (id) {
        s.showOnSave = false;
        s.showOnUpdate = true;
        s.modalTitle = "Edit :";
        s.checked = false;
        h.post('~/../../ComptAssessmentTool/PKBIData/' + id).then(function (result) {
            if (result.data.status == "success") {
                s.PKBI = result.data.PKBI;
                angular.element('#modalProgAdd').modal('show');
            }
        });

    }

    s.updatePKBI = function (data) {
        s.PKBI.KBICode = s.PKBI.KBICode;
        s.PKBI.comptCode = s.competency.comptCode;
        s.PKBI.comptLevel = data.comptLevel;
        s.PKBI.basic = data.basic;
        s.PKBI.intermediate = data.intermediate;
        s.PKBI.advance = data.advance;
        s.PKBI.superior = data.superior;
        h.post('~/../../ComptAssessmentTool/UpdatePKBI', { data: s.PKBI }).then(function (result) {
            if (result.data.status == "success") {
                competency(s.competency);
                angular.element('#modalProgAdd').modal('hide');
            }
        });
    }
    
    s.deletePKBI = function (id) {
        h.post('~/../../ComptAssessmentTool/DeletePKBI/' + id).then(function (result) {
            if (result.data.status == "success") {
                competency(s.competency);
                angular.element('#modalProgAdd').modal('hide');
            }
        });
    }


    s.techCommon = {};
    s.techOffice = {};

    s.openTechComptList = function () {
        h.post('~/../../ComptAssessmentTool/GetTechComptList', { data: s.positionData }).then(function (result) {
            if (result.data.status == "success") {
                s.techCommon = result.data.techCommon;
                s.techOffice = result.data.techOffice;
                $('#modalTechComptList').modal('show');
            }
        });
    }

    //ADD Technical Competency
    s.addTechCompetency = function (id) {
        s.bDisable = true;
        h.post('~/../../ComptAssessmentTool/AddTechCompetency', { pos: s.positionData, comptCode: id }).then(function (result) {
            if (result.data.status == "success") {
                s.techCommon = result.data.techCommon;
                s.techOffice = result.data.techOffice;
                //LoadStandard(s.positionData);
                s.bDisable = false;
            }
        });
        s.bDisable = false;
    }

    //Add LeadershipCompetency To Standard
    s.AddLeadershipCompt = function () {
        s.bDisable = true;
        h.post('~/../../ComptAssessmentTool/AddLeadershipToStandard', { pos: s.positionData }).then(function (result) {
            if (result.data.status == "success") {
                LoadStandard(s.positionData);
                $('#modalAddLeadership').modal('hide');
                s.bDisable = false;
            }
        });
        s.bDisable = false;
    }



    /* Edit Competency */
    s.editCompetency = function (id) {
        var d = s.competency;
        s.data.comptName = d.comptName;
        s.data.comptDesc = d.comptDesc;
        s.data.comptTool = d.comptTool;
        s.modalTitle = "Edit Competency";
        s.showOnSave = false;
        s.showOnUpdate = true;
        s.checked = false;
        angular.element('#modalCompetencyAdd').modal('show');
    }

    /* Update Competency */
    s.updateCompetency = function (data) {
        s.data.comptCode = s.competency.comptCode;
        s.data.comptGroupCode = s.competency.comptGroupCode;
        h.post('~/../../ComptAssessmentTool/UpdateCompetency', { data: data }).then(function (result) {
            if (result.data.status == "success") {
                s.competency.comptName = s.data.comptName;
                s.competency.comptDesc = s.data.comptDesc;
                //s.coreList = result.data.coreList;
                //competency(s.competency);
                s.data = {};
                angular.element('#modalCompetencyAdd').modal('hide');
            }
        });
    }

    s.deleteIndicator = function (id) {
        h.post('~/../../ComptAssessmentTool/DeleteKBI', { id: id }).then(function (result) {
            if (result.data.status == "success") {
                competency(s.competency);
                angular.element('#modalKBIAdd').modal('hide');
            }
        });
    }

    //EOCL
}]);