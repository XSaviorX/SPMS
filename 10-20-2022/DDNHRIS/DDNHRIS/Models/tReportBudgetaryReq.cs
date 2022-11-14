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
    
    public partial class tReportBudgetaryReq
    {
        public int recNo { get; set; }
        public string plantillaCode { get; set; }
        public string EIC { get; set; }
        public Nullable<int> itemNo { get; set; }
        public string positionTitle { get; set; }
        public string nameOfIncumbent { get; set; }
        public Nullable<int> salaryGrade { get; set; }
        public Nullable<int> stepInc { get; set; }
        public string stepRemark { get; set; }
        public Nullable<decimal> monthlySalary { get; set; }
        public Nullable<decimal> annualSalary { get; set; }
        public Nullable<decimal> PERA { get; set; }
        public Nullable<decimal> RA { get; set; }
        public Nullable<decimal> TA { get; set; }
        public Nullable<decimal> clothing { get; set; }
        public Nullable<decimal> subsistence { get; set; }
        public Nullable<decimal> laundry { get; set; }
        public Nullable<decimal> hazard { get; set; }
        public Nullable<decimal> yearEndBonus { get; set; }
        public Nullable<decimal> cashGift { get; set; }
        public Nullable<decimal> loyaltyBonus { get; set; }
        public Nullable<decimal> midYearBonus { get; set; }
        public Nullable<decimal> lifeRetirement { get; set; }
        public Nullable<decimal> hmdfPrem { get; set; }
        public Nullable<decimal> ECC { get; set; }
        public Nullable<decimal> PHIC { get; set; }
        public Nullable<decimal> total { get; set; }
        public string departmentCode { get; set; }
        public string reportCode { get; set; }
        public Nullable<int> budgetYear { get; set; }
    }
}
