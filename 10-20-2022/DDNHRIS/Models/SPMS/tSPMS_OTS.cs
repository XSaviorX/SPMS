//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DDNHRIS.Models.SPMS
{
    using System;
    using System.Collections.Generic;
    
    public partial class tSPMS_OTS
    {
        public int recNo { get; set; }
        public string ipcrID { get; set; }
        public string EIC { get; set; }
        public string MFOId { get; set; }
        public string indicatorId { get; set; }
        public string officeId { get; set; }
        public string divisionId { get; set; }
        public string output { get; set; }
        public Nullable<int> r_quantity { get; set; }
        public Nullable<int> r_quality { get; set; }
        public Nullable<int> r_timeliness { get; set; }
        public Nullable<System.DateTime> dateCreated { get; set; }
        public string approvedby { get; set; }
        public Nullable<int> day { get; set; }
        public Nullable<int> month { get; set; }
        public Nullable<int> status { get; set; }
    }
}
