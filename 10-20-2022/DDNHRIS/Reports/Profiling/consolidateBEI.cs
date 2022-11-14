namespace DDNHRIS.Reports.Profiling
{
    using DDNHRIS.Models;
    using System.Linq;
    using System.Globalization;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for consolidateBEI.
    /// </summary>
    public partial class consolidateBEI : Telerik.Reporting.Report
    {
        public consolidateBEI()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
        public static Decimal? totalAverage(string applicationCode)
        {
            using (HRISDBEntities _db = new HRISDBEntities())
            {
                decimal? totalAverage = 0;
                var data = _db.tRSPPSBRatings.Where(a => a.applicationCode == applicationCode).ToList();
                for (int i = 0; i < data.Count; i++)
                {
                    var score = data[i].score;
                    totalAverage += data[i].score;
                }

                return totalAverage / data.Count;
            }
        }
    }
}