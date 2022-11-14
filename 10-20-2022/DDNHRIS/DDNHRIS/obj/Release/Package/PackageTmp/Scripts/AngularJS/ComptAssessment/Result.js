app.controller('ComptAssmntResult', ['$scope', '$http', '$document', function (s, h, d) {

    s.comptGroup = 'CORE';
    s.comptGroupName = "CORE";
    s.competency = {};
    s.assmntResult = {};


    loadInitData();

     
    function loadInitData() {
        
        h.post('~/../../ComptAssessment/GetCompetencyByGroup').then(function (r) {
            if (r.data.status == "success") {
                s.competency = r.data.competency;
            }
        });
    }

    
    s.onChangeGroup = function (id) {
        s.comptGroup = id;
        
        if (s.comptGroup == "CORE") {
            s.comptGroupName = "CORE";
        }
        else if(s.comptGroup == "LEAD") {
            s.comptGroupName = "LEADERSHIP";
        }
        else if (s.comptGroup == "TECH") {
            s.comptGroupName = "TECHNICAL";
        }
        s.assmntResult = {};
    }

    s.onChangeCompetency = function () {
        s.assmntResult = {};
    }

    s.showResultByCompt = function (id) {      
        h.post('~/../../ComptAssessment/GetComptResult', {id: id}).then(function (r) {
            if (r.data.status == "success") {
                s.assmntResult = r.data.list;
            }
        });               
    }
    
    s.printComptResult = function (id) {
        h.post('~/../../ComptAssessment/PrintComptAssmntResult', { id: id }).then(function (r) {
            if (r.data.status == "success") {
                s.bDisable = false;
                window.open("../Reports/HRIS.aspx");
            }
        });
    }

   
}]);