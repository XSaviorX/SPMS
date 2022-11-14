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
    /// Summary description for applicationList.
    /// </summary>
    public partial class applicationList : Telerik.Reporting.Report
    {
        public applicationList()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            

        }
        public static string getName(string applicantCode)
        {
            using (HRISDBEntities _db = new HRISDBEntities())
            {
                var data = _db.tApplicants.FirstOrDefault(a => a.applicantCode == applicantCode);
                var result = data == null ? "" : data.fullNameLast;
                return result;
            }
        }
        public static string getPosition(string applicantCode)
        {
            using (HRISDBEntities _db = new HRISDBEntities())
            {
                
                var data = _db.tApplicants.FirstOrDefault(a => a.applicantCode == applicantCode);
                var result = data.EIC == null ? "Outsider" : _db.vRSPEmployeeLists.FirstOrDefault(a=>a.EIC == data.EIC).positionTitle;
                return result;
            }
        }
        public static string getApplicantOffice(string applicationCode)
        {
            using (HRISDBEntities _db = new HRISDBEntities())
            {

                var data = _db.tRSPApplications.FirstOrDefault(a => a.applicationCode == applicationCode).publicationItemCode;
                var result = data == null ? "" : _db.vRSPPublicationItems.FirstOrDefault(a => a.publicationItemCode == data).departmentName;
                return result;
            }
        }
        public static string getPositionApplied(string applicationCode)
        {
            using (HRISDBEntities _db = new HRISDBEntities())
            {

                var data = _db.tRSPApplications.FirstOrDefault(a => a.applicationCode == applicationCode).publicationItemCode;
                var result = data == null ? "" : _db.vRSPPublicationItems.FirstOrDefault(a=>a.publicationItemCode == data).positionTitle;
                return result;
            }
        }
    }
}