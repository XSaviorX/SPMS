using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DDNHRIS.Models
{
    public class DTRViewModel
    {
        public string EIC { get; set; }
        public int logNo { get; set; }
        public DateTime in1 { get; set; }
        public DateTime out1 { get; set; }
        public DateTime in2 { get; set; }
        public DateTime out2 { get; set; }
        public string remarks { get; set; }
        public string tardiness { get; set; }
        public string undertime { get; set; }        
    }


    public class DTRView
    {
        public string EIC { get; set; }
        public int period { get; set; }    
        public string month { get; set; }
    }



    public class myLogList
    {
        public string EIC { get; set; }
        public int logNo { get; set; }
        public string logDay { get; set; }

        public DateTime logDate { get; set; }
        public DateTime dtLogin1 { get; set; }
        public DateTime dtLogin2 { get; set; }
        public DateTime dtLogout1 { get; set; }
        public DateTime dtLogout2 { get; set; }

        public string logIn1 { get; set; }
        public string logOut1 { get; set; }
        public string logIn2 { get; set; }
        public string logOut2 { get; set; }
        public string remarks { get; set; }
        public int isWorkingDay { get; set; }
        public int hasLog { get; set; }
        public int hasSync { get; set; }

    }


    public class myShiftDTR
    {
        public string EIC { get; set; }
        public int logNo { get; set; }
        public string logDay { get; set; }
        public string schemeName { get; set; }
        public DateTime logDate { get; set; }
        public DateTime loginDT { get; set; }
        public DateTime logoutDT { get; set; }
        public string logIn { get; set; }
        public string logOut { get; set; }
        public string remarks { get; set; }
        public int isWorkingDay { get; set; }     
        public int tardyA { get; set; }
        public int tardyB { get; set; }
        public int totalTardy { get; set; }
        public int underTimeA { get; set; }
        public int underTimeB { get; set; }
        public int totalUndertime { get; set; }
        public int hasLog { get; set; }
    }


}