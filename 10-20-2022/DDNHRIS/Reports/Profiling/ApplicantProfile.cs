namespace DDNHRIS.Reports.Profiling
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;
    using DDNHRIS.Models;
    using System.Linq;
    using System.Globalization;



    /// <summary>
    /// Summary description for ApplicantProfile.
    /// </summary>
    public partial class ApplicantProfile : Telerik.Reporting.Report
    {

        public ApplicantProfile()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        public static Decimal gethere(string applicationCode)
        {
            using (HRISDBEntities _db = new HRISDBEntities())
            {

                var data = _db.tRSPApplicationProfileExprs.Where(a=>a.applicationCode == applicationCode).ToList();
                TimeSpan? relevantyear = new TimeSpan();
                double total = 0;
                int days = 0;
               // DateTime thisDay = DateTime.Today;
                //const double ApproxDaysPerMonth = 30.4375;
               //const double ApproxDaysPerYear = 365.25;
                for (int i = 0; i < data.Count; i++)
                {
                    var span = new TimeSpan();
                    if (data[i].periodTo == null)
                    {
                        span = DateTime.Today.Subtract(data[i].periodFrom.Value);

                    }
                    else
                    {
                        span = data[i].periodTo.Value.Subtract(data[i].periodFrom.Value);
                    }
                    //DateTime b = new DateTime(data[i].periodFrom);
                   // TimeSpan? diff = (data[i].periodFrom - data[i].periodTo);
                    TimeSpan? diff = (data[i].periodFrom - data[i].periodTo);
                   // int years = (data[i].periodFrom - data[i].periodTo).Year - 1;
                    //var datefrm = data[i].periodFrom.Value == null ? DateTime.Now : data[i].periodFrom.Value;
                   // var span = data[i].periodTo.Value.Subtract(datefrm);
                    days += span.Days;
                    //int res = DateTime.Compare(data[i].periodTo , data[i].periodFrom);
                    //total += new DateTime(diff).Year - 1;
                    relevantyear += diff;
                }
                //total = relevantyear.Value.TotalDays / 365.25;
                total = days / 365.25;
                return (Math.Round(Convert.ToDecimal(total), 2));
                //return total;
            }
        }
    }
}