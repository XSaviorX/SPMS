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
    
    public partial class tRSPNOSI
    {
        public int recNo { get; set; }
        public string transCode { get; set; }
        public string plantillaCode { get; set; }
        public string EIC { get; set; }
        public Nullable<int> itemNo { get; set; }
        public string fullNameFirst { get; set; }
        public string lastNamePrefix { get; set; }
        public string positionTitle { get; set; }
        public Nullable<int> salaryGrade { get; set; }
        public Nullable<int> stepIncFrom { get; set; }
        public Nullable<decimal> salaryFrom { get; set; }
        public string salaryDetailCode { get; set; }
        public Nullable<int> stepIncTo { get; set; }
        public Nullable<decimal> salaryTo { get; set; }
        public Nullable<decimal> salaryAdd { get; set; }
        public Nullable<System.DateTime> effectiveDate { get; set; }
        public Nullable<int> fy { get; set; }
        public Nullable<System.DateTime> printedDate { get; set; }
        public Nullable<int> nosiTag { get; set; }
        public string remarks { get; set; }
    }
}
