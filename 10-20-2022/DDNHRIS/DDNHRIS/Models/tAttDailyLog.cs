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
    
    public partial class tAttDailyLog
    {
        public int RecNo { get; set; }
        public string EIC { get; set; }
        public string ID { get; set; }
        public Nullable<System.DateTime> LogDate { get; set; }
        public Nullable<System.DateTime> In1 { get; set; }
        public Nullable<System.DateTime> Out1 { get; set; }
        public Nullable<System.DateTime> In2 { get; set; }
        public Nullable<System.DateTime> Out2 { get; set; }
        public string LastLog { get; set; }
        public string OverRide1 { get; set; }
        public string OverRide2 { get; set; }
        public string SchemeCode { get; set; }
        public Nullable<int> nonRegDay { get; set; }
        public Nullable<int> tag { get; set; }
    }
}
