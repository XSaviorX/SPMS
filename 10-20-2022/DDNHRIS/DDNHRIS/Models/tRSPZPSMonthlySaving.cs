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
    
    public partial class tRSPZPSMonthlySaving
    {
        public int recNo { get; set; }
        public string plantillaCode { get; set; }
        public Nullable<decimal> monthlyRate { get; set; }
        public Nullable<decimal> annualRate { get; set; }
        public Nullable<decimal> PERA { get; set; }
        public Nullable<decimal> RA { get; set; }
        public Nullable<decimal> TA { get; set; }
        public Nullable<decimal> clothing { get; set; }
        public Nullable<decimal> hazard { get; set; }
        public Nullable<decimal> laundry { get; set; }
        public Nullable<decimal> subsistence { get; set; }
        public Nullable<decimal> midYearBonus { get; set; }
        public Nullable<decimal> yearEndBonus { get; set; }
        public Nullable<decimal> cashGift { get; set; }
        public Nullable<decimal> gsisPrem { get; set; }
        public Nullable<decimal> ECC { get; set; }
        public Nullable<decimal> hdmfPrem { get; set; }
        public Nullable<decimal> phicPrem { get; set; }
        public Nullable<decimal> totalSavings { get; set; }
        public string departmentCode { get; set; }
        public string monthCode { get; set; }
        public Nullable<System.DateTime> monthPeriod { get; set; }
    }
}
