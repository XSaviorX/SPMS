using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DDNHRIS.Models
{
    public class ServiceRecord
    {
        public string EIC { get; set; }
        public string idNo { get; set; }
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string extnName { get; set; }
        public DateTime? birthDate { get; set; }
        public string birthPlace { get; set; }
        public string remarks { get; set; }
        public string remarks2 { get; set; }
        public string remarks3 { get; set; }
        public string lastRemarks { get; set; }
        public List<tRSPServiceRecord> serviceData { get; set; }
        public string GAPRemarks { get; set; }
    }


    public class ServiceData
    {
        public DateTime periodFrom { get; set; }
        public string periodTo { get; set; }
        public string designation { get; set; }
        public string empStatus { get; set; }
        public decimal salary { get; set; }
        public string salaryType { get;set; }
        public string office { get; set; }
        public string branch { get; set; }
        public string lwpay { get; set; }
        public string sepcause1 { get; set; }

    }



}