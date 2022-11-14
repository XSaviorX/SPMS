using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
 
using System.ComponentModel.DataAnnotations;

namespace DDNHRIS.Models
{

    public class AppointmentNonPlantillaData
    {
        public string EIC { get; set; }
        public string fullNameLast { get; set; }
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string extName { get; set; }
        public string middleName { get; set; }
        public string positionCode { get; set; }
        public string positionTitle { get; set; }
        public string subPositionCode { get; set; }
        public string subPositionTitle { get; set; }
        public string salaryDetailCode { get; set; }
        public int salaryGrade { get; set; }
        public string salaryGradeText { get; set; }
        public decimal rateDaily { get; set; }
        public decimal rateMonthly { get; set; }
        public string salaryTypeCode { get; set; }
        public string warmBodyGroupCode { get; set; }
        public string warmBodyGroupName { get; set; }
        public string periodFrom { get; set; }
        public string periodTo { get; set; }
        public string departmentCode { get; set; }
        public int hazardCode { get; set; }
        public decimal PS { get; set; }

    }


    public class TempAppointmentList
    {
        public string appointmentCode { get; set; }
        public string appointmentName { get; set; }
        public string employmentStatus { get; set; }
        public int itemCount { get; set; }
        public string period { get; set; }
        public string projectName { get; set; }
        public string status { get; set; }
        public string departmentName { get; set; }
        public int tag { get; set; }

    }

}