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
    
    public partial class tApplicantFile
    {
        public int recNo { get; set; }
        public string applicantCode { get; set; }
        public string applicationCode { get; set; }
        public string fileTypeCode { get; set; }
        public string fileName { get; set; }
        public string fileCode { get; set; }
        public Nullable<System.DateTime> uploadDT { get; set; }
        public Nullable<int> tag { get; set; }
    }
}
