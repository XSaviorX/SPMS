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
    
    public partial class tAppropriationProjMFOInd
    {
        public int recNo { get; set; }
        public string indicatorId { get; set; }
        public string indicator { get; set; }
        public Nullable<int> target { get; set; }
        public string targetUnit { get; set; }
        public Nullable<int> targetTypeId { get; set; }
        public string MFOId { get; set; }
        public Nullable<int> isActive { get; set; }
    }
}
