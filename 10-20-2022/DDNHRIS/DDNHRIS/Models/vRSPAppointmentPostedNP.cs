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
    
    public partial class vRSPAppointmentPostedNP
    {
        public string appointmentItemCode { get; set; }
        public string EIC { get; set; }
        public string fullNameLast { get; set; }
        public string positionTitle { get; set; }
        public string subPositionTitle { get; set; }
        public Nullable<int> salaryGrade { get; set; }
        public string salaryGradeText { get; set; }
        public Nullable<decimal> salary { get; set; }
        public string salaryType { get; set; }
        public Nullable<System.DateTime> periodFrom { get; set; }
        public Nullable<System.DateTime> periodTo { get; set; }
        public string appointmentCode { get; set; }
        public string employmentStatusCode { get; set; }
        public string employmentStatus { get; set; }
        public string employmentStatusNameShort { get; set; }
        public string fundSourceCode { get; set; }
        public string projectName { get; set; }
        public string programCode { get; set; }
        public string programName { get; set; }
        public string departmentCode { get; set; }
        public string shortDepartmentName { get; set; }
        public string departmentName { get; set; }
    }
}
