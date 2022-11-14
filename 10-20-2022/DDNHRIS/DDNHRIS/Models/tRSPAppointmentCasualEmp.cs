//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DDNHRIS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tRSPAppointmentCasualEmp
    {
        public int recNo { get; set; }
        public string appointmentItemCode { get; set; }
        public string appointmentCode { get; set; }
        public string EIC { get; set; }
        public string fullNameTitle { get; set; }
        public string positionCode { get; set; }
        public string positionTitle { get; set; }
        public string subPositionCode { get; set; }
        public string subPositionTitle { get; set; }
        public string salaryDetailCode { get; set; }
        public Nullable<int> salaryGrade { get; set; }
        public Nullable<decimal> salary { get; set; }
        public string salaryType { get; set; }
        public Nullable<System.DateTime> periodFrom { get; set; }
        public Nullable<System.DateTime> periodTo { get; set; }
        public string warmBodyGroupCode { get; set; }
        public string placeOfAssignment { get; set; }
        public string departmentName { get; set; }
        public string PGHeadName { get; set; }
        public string PGHeadPosition { get; set; }
        public Nullable<System.DateTime> assumptionDate { get; set; }
        public string presentAppropAct { get; set; }
        public string previousAppropAct { get; set; }
        public string otherCompensation { get; set; }
        public string positionSupervisor { get; set; }
        public string positionSupervisorNext { get; set; }
        public string supervisedPositions { get; set; }
        public string supervisedItemNo { get; set; }
        public string machineToolsUsed { get; set; }
        public Nullable<byte> PDFManagerial { get; set; }
        public Nullable<byte> PDFSupervisor { get; set; }
        public Nullable<byte> PDFNonSupervisor { get; set; }
        public Nullable<byte> PDFStaff { get; set; }
        public Nullable<byte> PDFGenPublic { get; set; }
        public Nullable<byte> PDFOtherAgency { get; set; }
        public string PDFOthers { get; set; }
        public Nullable<byte> PDFWorkConOffice { get; set; }
        public Nullable<byte> PDFWorkConField { get; set; }
        public string PDFGenFunc { get; set; }
        public string PDFGenFuncPosition { get; set; }
        public string govtIDName { get; set; }
        public string govtIDNo { get; set; }
        public Nullable<System.DateTime> govtIDIssued { get; set; }
        public Nullable<decimal> PS { get; set; }
        public Nullable<int> tag { get; set; }
    }
}
