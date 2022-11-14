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
    
    public partial class vRSPZPSComputation
    {
        public int recNo { get; set; }
        public string reportCode { get; set; }
        public string EIC { get; set; }
        public string fullNameFirst { get; set; }
        public string fullNameLast { get; set; }
        public string appointeeName { get; set; }
        public string positionTitle { get; set; }
        public Nullable<int> salaryGrade { get; set; }
        public Nullable<System.DateTime> periodFrom { get; set; }
        public Nullable<System.DateTime> periodTo { get; set; }
        public Nullable<decimal> dailyRate { get; set; }
        public Nullable<decimal> monthlyRate { get; set; }
        public Nullable<decimal> annualRate { get; set; }
        public Nullable<decimal> PERA { get; set; }
        public Nullable<decimal> leaveEarned { get; set; }
        public Nullable<decimal> hazardPay { get; set; }
        public Nullable<decimal> laundry { get; set; }
        public Nullable<decimal> subsistence { get; set; }
        public Nullable<decimal> midYear { get; set; }
        public Nullable<decimal> yearEnd { get; set; }
        public Nullable<decimal> cashGift { get; set; }
        public Nullable<decimal> lifeRetiremnt { get; set; }
        public Nullable<decimal> ECC { get; set; }
        public Nullable<decimal> HDMF { get; set; }
        public Nullable<decimal> PHIC { get; set; }
        public Nullable<decimal> clothing { get; set; }
        public Nullable<decimal> totalPS { get; set; }
        public Nullable<int> monthCount { get; set; }
        public string employmentStatusCode { get; set; }
        public string employmentStatusNameShort { get; set; }
        public string fundSourceCode { get; set; }
        public string projectName { get; set; }
        public Nullable<decimal> loyalty { get; set; }
    }
}
