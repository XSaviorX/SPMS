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
    
    public partial class tApplicantWE
    {
        public int recNo { get; set; }
        public string wesCode { get; set; }
        public string applicantCode { get; set; }
        public string positionTitle { get; set; }
        public Nullable<System.DateTime> durationFrom { get; set; }
        public Nullable<System.DateTime> durationTo { get; set; }
        public string nameOfOffice { get; set; }
        public string nameOfSupervisor { get; set; }
        public string nameOfAgency { get; set; }
        public string actualDuties { get; set; }
        public Nullable<System.DateTime> transDT { get; set; }
    }
}