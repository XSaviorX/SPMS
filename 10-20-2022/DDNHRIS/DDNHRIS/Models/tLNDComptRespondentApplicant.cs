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
    
    public partial class tLNDComptRespondentApplicant
    {
        public int recNo { get; set; }
        public string respondentCode { get; set; }
        public string assessmentCode { get; set; }
        public string EIC { get; set; }
        public string applicationCode { get; set; }
        public string supervisorEIC { get; set; }
        public Nullable<int> tag { get; set; }
        public Nullable<System.DateTime> transDT { get; set; }
        public string userEIC { get; set; }
        public Nullable<decimal> score { get; set; }
        public Nullable<decimal> rating { get; set; }
    }
}
