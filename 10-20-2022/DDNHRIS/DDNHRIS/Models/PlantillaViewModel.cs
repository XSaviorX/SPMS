using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DDNHRIS.Models
{
    public class StructureParentViewModel
    {
        public string structureCode { get; set; }
        public string structureName { get; set; }
        public string structurePath { get; set; }
        public string parentPath { get; set; }
        public int structNo { get; set; }
        public int parentNo { get; set; }
        public int levelNo { get; set; }
        public int orderNo { get; set; }
        public List<PlantillaPosition> positionList { get; set; }
    }


    public class PlantillaPosition
    {
        public string plantillaCode { get; set; }
        public string oldItemNo { get; set; }
        public string itemNo { get; set; }
        public string positionTitle { get; set; }
        public decimal currentYearRate { get; set; }
        public decimal proposeYearRate { get; set; }
        public int salaryGrade { get; set; }
        public int step { get; set; }
        public int positionLevel { get; set; }
        public string positionLevelNameShort { get; set; }
        public int isFunded { get; set; }
        public int fundStat { get; set; }
        public string positionStatusName { get; set; }
        public string subPositionCode { get; set; }
        public string subPositionTitle { get; set; }
        public string EIC { get; set; }
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string extName { get; set; }
        public string fullNameLast { get; set; }
        public DateTime birthDate { get; set; }
        public string eligibilityName { get; set; }
        public DateTime origApptDate { get; set; }
        public DateTime lastPromDate { get; set; }
        public string empStatus { get; set; }
        public string remarks { get; set; }
        public string clusterCode { get; set; }
        public string clusterName { get; set; }
        public string divisionCode { get; set; }
        public string divisionName { get; set; }
        public string sectionCode { get; set; }
        public string sectionName { get; set; }
        public string unitCode { get; set; }
        public string unitName { get; set; }
        public int plantillaNo { get; set; }

    }
}