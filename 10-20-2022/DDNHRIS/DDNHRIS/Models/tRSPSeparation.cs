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
    
    public partial class tRSPSeparation
    {
        public int recNo { get; set; }
        public string recordTransCode { get; set; }
        public string EIC { get; set; }
        public string fullNameLast { get; set; }
        public Nullable<System.DateTime> dateOfBirth { get; set; }
        public string positionTitle { get; set; }
        public string statusOfAppt { get; set; }
        public Nullable<int> salaryGrade { get; set; }
        public string separationType { get; set; }
        public System.DateTime effectiveDate { get; set; }
        public Nullable<System.DateTime> reportDate { get; set; }
        public string remarks { get; set; }
        public Nullable<int> tag { get; set; }
        public string statusGroup { get; set; }
        public string userEIC { get; set; }
        public Nullable<System.DateTime> dateEncoded { get; set; }
    }
}
