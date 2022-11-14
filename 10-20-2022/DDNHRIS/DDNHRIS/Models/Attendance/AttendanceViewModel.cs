using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DDNHRIS.Models.Attendance
{
    
    public class myDailyTimeRecord
    {
        public string EIC { get; set; }   
        public string logDay { get; set; }
        public DateTime logDate { get; set; }
        //public DateTime dtLogin1 { get; set; }
        //public DateTime dtLogin2 { get; set; }
        //public DateTime dtLogout1 { get; set; }
        //public DateTime dtLogout2 { get; set; }
        public string login1 { get; set; }
        public string logout1 { get; set; }
        public string login2 { get; set; }      
        public string logout2 { get; set; }
        public string schemeCode { get; set; }
        public string schemeName { get; set; }

        public int updateTag { get; set; }
      
    }



}