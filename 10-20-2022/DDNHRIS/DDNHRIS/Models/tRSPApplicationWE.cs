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
    
    public partial class tRSPApplicationWE
    {
        public int recNo { get; set; }
        public string applicationCode { get; set; }
        public string wesCode { get; set; }
        public string EIC { get; set; }
        public string applicantCode { get; set; }
        public Nullable<bool> isVerified { get; set; }
        public string verEIC { get; set; }
    }
}
