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
    
    public partial class vSPMS_CommonMFO
    {
        public int recNo { get; set; }
        public string MFOId { get; set; }
        public string indicatorId { get; set; }
        public string officeId { get; set; }
        public string divisionId { get; set; }
        public string description { get; set; }
        public Nullable<int> categoryId { get; set; }
        public string classificationId { get; set; }
        public string catTargetId { get; set; }
        public string targetId { get; set; }
        public Nullable<int> target { get; set; }
        public Nullable<int> tRemaining { get; set; }
        public string TargetOffcId { get; set; }
        public string officeName { get; set; }
        public string officeNameShort { get; set; }
        public string targetUnit { get; set; }
        public Nullable<int> targetTypeId { get; set; }
        public string indicator { get; set; }
        public string MFO { get; set; }
        public Nullable<int> isHrtgt { get; set; }
    }
}
