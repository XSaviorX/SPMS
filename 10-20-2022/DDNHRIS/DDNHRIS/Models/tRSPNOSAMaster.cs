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
    
    public partial class tRSPNOSAMaster
    {
        public int recNo { get; set; }
        public string NOSCode { get; set; }
        public Nullable<System.DateTime> NOSADate { get; set; }
        public string LBCNo { get; set; }
        public Nullable<System.DateTime> LBCDate { get; set; }
        public string orderNo { get; set; }
        public Nullable<System.DateTime> orderDate { get; set; }
        public Nullable<System.DateTime> effectiveDate { get; set; }
    }
}