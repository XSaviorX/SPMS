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
    
    public partial class tRSPWarmBody
    {
        public int recNo { get; set; }
        public string warmBodyGroupCode { get; set; }
        public string warmBodyGroupName { get; set; }
        public string departmentCode { get; set; }
        public string structureCode { get; set; }
        public Nullable<int> orderNo { get; set; }
        public Nullable<int> isDDN { get; set; }
        public Nullable<int> tag { get; set; }
    }
}