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
    
    public partial class tApplicantTraining
    {
        public int recNo { get; set; }
        public string trainingCode { get; set; }
        public string applicantCode { get; set; }
        public string trainingTitle { get; set; }
        public Nullable<System.DateTime> fromDate { get; set; }
        public Nullable<System.DateTime> toDate { get; set; }
        public Nullable<decimal> hours { get; set; }
        public string conductedBy { get; set; }
        public string venue { get; set; }
        public Nullable<System.DateTime> transDT { get; set; }
        public Nullable<bool> isActive { get; set; }
    }
}
