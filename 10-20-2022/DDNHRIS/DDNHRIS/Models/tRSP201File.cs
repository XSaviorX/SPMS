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
    
    public partial class tRSP201File
    {
        public int recNo { get; set; }
        public string docID { get; set; }
        public string EIC { get; set; }
        public string docType { get; set; }
        public string docName { get; set; }
        public Nullable<System.DateTime> expiryDate { get; set; }
        public string docFileID { get; set; }
        public string userEIC { get; set; }
        public Nullable<System.DateTime> transDT { get; set; }
    }
}
