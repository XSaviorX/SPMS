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
    
    public partial class tRSPAppointment
    {
        public int recNo { get; set; }
        public string appointmentCode { get; set; }
        public string EIC { get; set; }
        public string nameOfAppointee { get; set; }
        public string fullName { get; set; }
        public string fullNameTitle { get; set; }
        public string fullNameSign { get; set; }
        public string positionTitle { get; set; }
        public string namePrefix { get; set; }
        public string nameSuffix { get; set; }
        public string prefixLastName { get; set; }
        public Nullable<int> salaryGrade { get; set; }
        public Nullable<int> step { get; set; }
        public Nullable<int> itemNo { get; set; }
        public string employmentStatusCode { get; set; }
        public string employmentStatusName { get; set; }
        public string officeAssignment { get; set; }
        public Nullable<decimal> rateMonth { get; set; }
        public string rateMonthText { get; set; }
        public string appNatureCode { get; set; }
        public string appNatureName { get; set; }
        public string viceOf { get; set; }
        public string viceStatus { get; set; }
        public Nullable<System.DateTime> effectivityDate { get; set; }
        public Nullable<int> pageNo { get; set; }
        public string remarks { get; set; }
        public string eligibilityCode { get; set; }
        public string empEligibilityCode { get; set; }
        public Nullable<System.DateTime> CSCPostedDate { get; set; }
        public Nullable<System.DateTime> CSCClosingDate { get; set; }
        public Nullable<System.DateTime> PSBDate { get; set; }
        public string publicationItemCode { get; set; }
        public string plantillaCode { get; set; }
        public Nullable<int> tag { get; set; }
    }
}
