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
    
    public partial class tRSPAppointmentNotify
    {
        public int recNo { get; set; }
        public string notificationCode { get; set; }
        public string appointmentItemCode { get; set; }
        public string termCode { get; set; }
        public string remarks { get; set; }
        public Nullable<System.DateTime> adjstmntFrom { get; set; }
        public Nullable<System.DateTime> adjstmntTo { get; set; }
        public Nullable<int> hasClothing { get; set; }
        public Nullable<int> hasBonusMY { get; set; }
        public Nullable<int> hasBonusYE { get; set; }
        public Nullable<int> hazardCode { get; set; }
        public string userEIC { get; set; }
        public Nullable<System.DateTime> transDT { get; set; }
        public string postedByEIC { get; set; }
        public Nullable<System.DateTime> postedDT { get; set; }
        public Nullable<int> tag { get; set; }
    }
}
