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
    
    public partial class tLNDComptRespondent
    {
        public int recNo { get; set; }
        public string respondentCode { get; set; }
        public string assessmentCode { get; set; }
        public string comptPositionCode { get; set; }
        public string EIC { get; set; }
        public string supervisorEIC { get; set; }
        public string deptHeadEIC { get; set; }
        public Nullable<decimal> rating { get; set; }
        public string comptLevel { get; set; }
        public string remarks { get; set; }
        public Nullable<int> tag { get; set; }
    }
}
