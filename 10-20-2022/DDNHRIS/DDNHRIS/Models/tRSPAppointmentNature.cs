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
    
    public partial class tRSPAppointmentNature
    {
        public int recNo { get; set; }
        public string appNatureCode { get; set; }
        public string appNatureName { get; set; }
        public string appNatureDesc { get; set; }
        public Nullable<int> orderNo { get; set; }
        public Nullable<int> probationTag { get; set; }
        public string appNatureDescCasual { get; set; }
    }
}
