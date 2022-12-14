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
    
    public partial class tRSPAppointmentData
    {
        public string appointmentCode { get; set; }
        public string positionSupervisorImm { get; set; }
        public string positionSupervisorNxt { get; set; }
        public string positionSupervise { get; set; }
        public string positionSuperviseItemNo { get; set; }
        public string machineToolsUsed { get; set; }
        public Nullable<int> CCSMangerial { get; set; }
        public Nullable<int> CCSSupervisors { get; set; }
        public Nullable<int> CCSNonSupervisors { get; set; }
        public Nullable<int> CCSSTaff { get; set; }
        public Nullable<int> CCSGenPublic { get; set; }
        public Nullable<int> CCSOtherAgencies { get; set; }
        public string CCSOthers { get; set; }
        public Nullable<int> workConOffice { get; set; }
        public Nullable<int> workConField { get; set; }
        public string presentAddress { get; set; }
        public string govtID { get; set; }
        public string govtIDNo { get; set; }
        public string govtIDDateIssued { get; set; }
        public Nullable<System.DateTime> effectiveMonth { get; set; }
        public Nullable<decimal> availableFunds { get; set; }
        public string availableFundsText { get; set; }
        public string presentAppAct { get; set; }
        public string previousAppAct { get; set; }
        public string workStation { get; set; }
        public string otherCompensation { get; set; }
        public string descriptUnitSec { get; set; }
        public string departmentHead { get; set; }
        public string departmentHeadPosition { get; set; }
    }
}
