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
    
    public partial class tJustifyApp
    {
        public long recNo { get; set; }
        public string controlNo { get; set; }
        public string EIC { get; set; }
        public Nullable<System.DateTime> logDT { get; set; }
        public Nullable<System.DateTime> schemeDT { get; set; }
        public string reason { get; set; }
        public Nullable<int> logType { get; set; }
        public string approveEIC { get; set; }
        public Nullable<int> statusTag { get; set; }
        public Nullable<int> returnTag { get; set; }
        public string userEIC { get; set; }
        public Nullable<System.DateTime> transDT { get; set; }
    }
}