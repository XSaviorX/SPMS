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
    
    public partial class tRSPEmploymentStatu
    {
        public int recNo { get; set; }
        public string employmentStatusCode { get; set; }
        public string employmentStatus { get; set; }
        public string employmentStatusNameShort { get; set; }
        public string employmentStatusTag { get; set; }
        public Nullable<int> isPlantilla { get; set; }
        public Nullable<int> isGovService { get; set; }
    }
}
