using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DDNHRIS.Models;

namespace DDNHRIS.Controllers
{

    [SessionTimeout]
    [Authorize(Roles = "APRD,PACCOSTAFF,PBOSTAFF")]
    public class RSPFundSourceController : Controller
    {

        HRISDBEntities db = new HRISDBEntities();
        // GET: RSPFundSource


        public ActionResult Programs()
        {
            return View();
        }


        public ActionResult Projects()
        {
            return View();
        }


        //NG
        public JsonResult ProgramInitData()
        {
            try
            {
                IEnumerable<vRSPRefProgram> prog = db.vRSPRefPrograms.Where(e => e.isActive == 1).ToList().OrderBy(o => o.programName).ToList();
                IEnumerable<tOrgDepartment> dept = db.tOrgDepartments.Where(e => e.isActive == true).OrderBy(o => o.orderNo).ToList();

                return Json(new { status = "success", programList = prog, deptList = dept }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }



        public JsonResult SubmitProgram(tRSPRefProgram data)
        {
            try
            {

                string tmp = DateTime.Now.ToString("yyMMddHHmmssfff");
                string code = "FSPROG" + tmp;

                tRSPRefProgram p = new tRSPRefProgram();
                p.programCode = code;
                p.programName = data.programName;
                p.departmentCode = data.departmentCode;
                p.isActive = 1;
                db.tRSPRefPrograms.Add(p);
                db.SaveChanges();


                IEnumerable<vRSPRefProgram> prog = db.vRSPRefPrograms.Where(e => e.isActive == 1).ToList().OrderBy(o => o.programName).ToList();

                return Json(new { status = "success", programList = prog }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        //NG

        [HttpPost]
        public JsonResult ProjectInitData(int year)
        {
            //int cYear = 2021;


            List<vRSPRefFundSource> projList = ProjectList(year);


            //IEnumerable<vRSPRefFundSource> sourceList = db.vRSPRefFundSources.Where(e => e.CY >= 2021).ToList();
            //IEnumerable<vRSPAppointmentNPEmployee> empApptList = db.vRSPAppointmentNPEmployees.Where(e => e.CY.Value == cYear).ToList();

            //List<vRSPRefFundSource> list = new List<vRSPRefFundSource>();

            //foreach (vRSPRefFundSource item in sourceList)
            //{
            //    decimal totalApptPS = Convert.ToDecimal(empApptList.Where(e => e.fundSourceCode == item.fundSourceCode).Sum(e => e.PS));
            //    decimal balance = Convert.ToDecimal(item.amount) - totalApptPS;

            //    list.Add(new vRSPRefFundSource()
            //    {
            //        fundSourceCode = item.fundSourceCode,
            //        projectName = item.projectName,
            //        departmentName = item.departmentName,
            //        amount = item.amount,
            //        programName = balance.ToString("#,##0.00")
            //    });

            //}

            List<SelectListItem> yearList = new List<SelectListItem>();
            int counter = DateTime.Now.Year;
            for (int i = counter + 1; i >= counter - 1; i--)
            {
                yearList.Add(new SelectListItem()
                {
                    Text = i.ToString(),
                    Value = i.ToString()
                });
            }

            IEnumerable<tRSPRefProgram> prog = db.tRSPRefPrograms.Where(e => e.isActive == 1).OrderBy(o => o.programName).ToList();
            return Json(new { status = "success", projList = projList, yearList = yearList, progList = prog }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetDeptList()
        {
            List<SelectListItem> yearList = new List<SelectListItem>();
            int counter = DateTime.Now.Year;
            for (int i = counter + 1; i >= counter - 1; i--)
            {
                yearList.Add(new SelectListItem()
                {
                    Text = i.ToString(),
                    Value = i.ToString()
                });
            }
            //IEnumerable<tOrgDepartment> dept = db.tOrgDepartments.Where(e => e.isActive == true).OrderBy(o => o.orderNo).ToList();

            IEnumerable<tRSPRefProgram> prog = db.tRSPRefPrograms.Where(e => e.isActive == 1).OrderBy(o => o.programName).ToList();
            return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult SubmitFundSource(vRSPRefFundSource data)
        {
            try
            {
                string appropId = "APPR20192020";
                if (data.CY == 2022)
                {
                    appropId = "APPR2022";
                }

                string tmp = DateTime.Now.ToString("yyMMddHHmmssfff");
                string code = "FSPROJ" + tmp;

                tRSPRefFundSource fSource = new tRSPRefFundSource();
                fSource.fundSourceCode = code;
                fSource.programCode = data.programCode;
                fSource.projectName = data.projectName;
                fSource.appropriationCode = appropId;
                fSource.isActive = 1;
                db.tRSPRefFundSources.Add(fSource);

                tRSPRefFundSourceDetail f = new tRSPRefFundSourceDetail();
                f.fundSourceCode = code;
                f.particulars = "Reg. Appr.";
                f.amount = Convert.ToDecimal(data.amount);
                f.userID = Session["_EIC"].ToString();
                f.transDT = DateTime.Now;
                db.tRSPRefFundSourceDetails.Add(f);

                db.SaveChanges();

                IEnumerable<vRSPRefFundSource> sourceList = db.vRSPRefFundSources.Where(e => e.CY >= 2021).ToList();

                List<vRSPRefFundSource> list = new List<vRSPRefFundSource>();
                foreach (vRSPRefFundSource item in sourceList)
                {
                    list.Add(new vRSPRefFundSource()
                    {
                        fundSourceCode = item.fundSourceCode,
                        projectName = item.projectName,
                        departmentName = item.departmentName,
                        amount = item.amount
                    });
                }
                return Json(new { status = "success", projList = list }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        //EDIT FUND SOURCE

        public JsonResult EditFundSource(string id)
        {
            try
            {
                vRSPRefFundSource fSource = db.vRSPRefFundSources.SingleOrDefault(e => e.fundSourceCode == id);
                if (fSource != null)
                {
                    return Json(new { status = "success", fundSource = fSource }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }



        public JsonResult UpdateFundSource(vRSPRefFundSource data)
        {
            try
            {
                tRSPRefFundSource fSource = db.tRSPRefFundSources.SingleOrDefault(e => e.fundSourceCode == data.fundSourceCode);
                fSource.programCode = data.programCode;
                fSource.projectName = data.projectName;

                tRSPRefFundSourceDetail l = db.tRSPRefFundSourceDetails.Single(e => e.fundSourceCode == data.fundSourceCode && e.particulars == "AB");

                l.amount = Convert.ToDecimal(data.amount);

                db.SaveChanges();
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult SetFundSourceCode(string id)
        {
            try
            {
                Session["FunSourceCode"] = id;
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult ProjectDetail()
        {
            return View();
        }

        public class TempFundSourceData
        {
            public string fundSourceCode { get; set; }
            public string fundSourceName { get; set; }
            public decimal amountAlloc { get; set; }
            public decimal amountExpns { get; set; }
            public decimal amountBalance { get; set; }
            public string rate { get; set; }
            public List<TempFundSourceEmp> empList { get; set; }
        }

        public class TempFundSourceEmp
        {
            public string notificationCode { get; set; }
            public string appointmentItemCode { get; set; }
            public string EIC { get; set; }
            public string fullNameLast { get; set; }
            public string fullNameFirst { get; set; }
            public string positionTitle { get; set; }
            public decimal salary { get; set; }
            public string salaryType { get; set; }
            public string period { get; set; }
            public DateTime periodFrom { get; set; }
            public DateTime periodTo { get; set; }
            public decimal totalPS { get; set; }
            public string employmentStatusCode { get; set; }
            public string employmentStatus { get; set; }
            public string employmentStatusTag { get; set; }
            public int hasClothing { get; set; }
            public int hasBonusMY { get; set; }
            public int hasBonusYE { get; set; }
            public int hazardCode { get; set; }
            public DateTime adjstmntFrom { get; set; }
            public DateTime adjstmntTo { get; set; }
            public int empTag { get; set; }
            public int notifyTag { get; set; }
            public string termCode { get; set; }
            public string notifyRemarks { get; set; }
            public string projectName { get; set; }
        }

        //FUNDSOURCE : CHARGESDATA
        public TempFundSourceData EmployeeByCharges(string code)
        {
            TempFundSourceData myData = new TempFundSourceData();
            List<TempFundSourceEmp> empData = new List<TempFundSourceEmp>();

            vRSPRefFundSource fs = db.vRSPRefFundSources.Single(e => e.fundSourceCode == code);
            IEnumerable<tRSPRefFundSourceDetail> fsDetails = db.tRSPRefFundSourceDetails.Where(e => e.fundSourceCode == code).OrderBy(e => e.recNo).ToList();

            IEnumerable<vRSPAppointmentNPEmployee> empList = db.vRSPAppointmentNPEmployees.Where(e => e.fundSourceCode == code && e.PSTag == 1).OrderBy(o => o.fullNameLast).ThenBy(o => o.periodFrom).ToList();
            List<vRSPAppointmentNPEmployee> myList = new List<vRSPAppointmentNPEmployee>();
            IEnumerable<tRSPAppointmentNotify> separationList = db.tRSPAppointmentNotifies.ToList();
            foreach (vRSPAppointmentNPEmployee item in empList)
            {
                string tmpRemarks = "";
                tRSPAppointmentNotify sepItem = separationList.LastOrDefault(e => e.appointmentItemCode == item.appointmentItemCode);
                int sepTag = 0;

                string termCode = "";
                if (sepItem != null)
                {
                    if (sepItem.tag == 1)
                    {
                        sepTag = 1;
                        termCode = sepItem.termCode;
                        tmpRemarks = sepItem.remarks;
                    }
                }

                string tmpDF = ""; string tmpDT = "";
                string periodText = "";
                if (item.periodFrom != null && item.periodFrom.Value.Year > 1)
                {
                    tmpDF = Convert.ToDateTime(item.periodFrom).ToString("MM/dd/yyyy");
                }
                if (item.periodTo != null && item.periodTo.Value.Year > 1)
                {
                    tmpDT = Convert.ToDateTime(item.periodTo).ToString("MM/dd/yyyy");
                }
                if (item.empTag != -1) //mean active
                {
                    periodText = GetPeriodDisplayText(Convert.ToDateTime(item.periodFrom), Convert.ToDateTime(item.periodTo));
                }
                else if (item.empTag == -1)
                {
                    if (sepItem != null)
                    {
                        termCode = sepItem.termCode;
                        tmpRemarks = sepItem.remarks;
                        periodText = GetPeriodText(item.appointmentItemCode);
                    }
                }

                empData.Add(new TempFundSourceEmp()
                {
                    appointmentItemCode = item.appointmentItemCode,
                    EIC = item.EIC,
                    fullNameLast = item.fullNameLast,
                    fullNameFirst = item.fullNameFirst,
                    positionTitle = item.positionTitle,
                    salary = Convert.ToDecimal(item.salary),
                    salaryType = item.salaryType,
                    totalPS = Convert.ToDecimal(item.totalPS),
                    periodFrom = Convert.ToDateTime(item.periodFrom),
                    periodTo = Convert.ToDateTime(item.periodTo),
                    employmentStatusCode = item.employmentStatusCode,
                    employmentStatus = item.employmentStatus,
                    employmentStatusTag = item.employmentStatusTag,
                    empTag = Convert.ToInt16(item.empTag),
                    period = periodText,
                    termCode = termCode,
                    notifyTag = sepTag,
                    notifyRemarks = tmpRemarks
                });
            }

            decimal bal = Convert.ToDecimal(fs.amount - empList.Sum(e => e.totalPS).Value);
            decimal pRate = 0;
            if (bal > 0)
            {
                pRate = Convert.ToDecimal((bal / fs.amount) * 100);
            }

            string tmpRate = "";
            if (pRate < 1)
            {
                tmpRate = pRate.ToString("0.00");
            }
            else
            {
                tmpRate = pRate.ToString("#0");
            }
            myData.fundSourceCode = fs.fundSourceCode;
            myData.fundSourceName = fs.projectName;

            myData.amountAlloc = Convert.ToDecimal(fs.amount);
            myData.amountExpns = Convert.ToDecimal(empList.Sum(e => e.totalPS).Value);
            myData.amountBalance = bal;
            myData.empList = empData;
            myData.rate = tmpRate;

            return myData;
        }

        private string GetPeriodText(string appItemCode)
        {
            tRSPZPersonnelService p = db.tRSPZPersonnelServices.Single(e => e.appointmentItemCode == appItemCode);
            string res = "";
            if (p.periodTo != null && p.periodFrom != null)
            {
                DateTime df = Convert.ToDateTime((p.periodFrom));
                DateTime dt = Convert.ToDateTime((p.periodTo));
                if (df.Month == dt.Month)
                {
                    res = Convert.ToDateTime(p.periodFrom).ToString("MMM/dd") + " - " + dt.ToString("dd/yyy");
                }
                else
                {
                    res = df.ToString("MMMdd") + "-" + dt.ToString("MMMdd, yy").ToString();
                }
            }
            else
            {
                return "";
            }
            return res;
        }

        private string GetPeriodDisplayText(DateTime df, DateTime dt)
        {
            string res = "";

            if (df.Month == dt.Month)
            {
                res = Convert.ToDateTime(df).ToString("MMM/dd") + " - " + dt.ToString("dd/yyy");
            }
            else
            {
                res = df.ToString("MMMdd") + "-" + dt.ToString("MMMdd, yy").ToString();
            }


            return res;
        }

        //APPOINMENT ITEM PS
        [HttpPost]
        public JsonResult GetAppointmentItemPS(string appItemCode)
        {
            try
            {
                tRSPZPersonnelService psData = db.tRSPZPersonnelServices.Single(e => e.appointmentItemCode == appItemCode);

                string uEIC = Session["_EIC"].ToString();
                int adm = 0;
                if (uEIC == "DS1070016970E3ACC02D" || uEIC == "FN24251916852AF7C1C0")
                {
                    adm = 1;
                }

                return Json(new { status = "success", ps = psData, adminTag = adm }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        //NG
        [HttpPost]
        public JsonResult ProjectDetailData()
        {
            string code = Session["FunSourceCode"].ToString();

            TempFundSourceData fundSourceData = EmployeeByCharges(code);
            IEnumerable<tRSPRefFundSourceDetail> approList = db.tRSPRefFundSourceDetails.Where(e => e.fundSourceCode == code).OrderBy(e => e.recNo).ToList();
            return Json(new { status = "success", fundSourceData = fundSourceData, approList = approList }, JsonRequestBehavior.AllowGet);
        }





        [HttpPost]
        public JsonResult SaveAdjstmntRequest(tRSPAppointmentNotify data)
        {
            try
            {
                if (data.adjstmntFrom.Value.Year == 1 || data.adjstmntFrom == null || data.adjstmntTo.Value.Year == 1 || data.adjstmntTo == null)
                {
                    return Json(new { status = "Please fill-up the required data!" }, JsonRequestBehavior.AllowGet);
                }

                vRSPAppointmentNPEmployee app = db.vRSPAppointmentNPEmployees.SingleOrDefault(e => e.appointmentItemCode == data.appointmentItemCode);
                if (app != null)
                {
                    string tmpCode = DateTime.Now.ToString("yyMMddHHmmss") + app.appointmentItemCode.Substring(0, 13);
                    tRSPAppointmentNotify n = new tRSPAppointmentNotify();
                    n.notificationCode = tmpCode;
                    n.appointmentItemCode = data.appointmentItemCode;
                    n.termCode = data.termCode;
                    n.remarks = data.remarks;
                    n.adjstmntFrom = data.adjstmntFrom;
                    n.adjstmntTo = data.adjstmntTo;
                    n.hasBonusMY = data.hasBonusMY;
                    n.hasBonusYE = data.hasBonusYE;
                    n.hasClothing = data.hasClothing;
                    n.hazardCode = data.hazardCode;
                    n.userEIC = Session["_EIC"].ToString();
                    n.transDT = DateTime.Now;
                    n.tag = 1;
                    db.tRSPAppointmentNotifies.Add(n);
                    db.SaveChanges();

                    TempFundSourceData fundSourceData = EmployeeByCharges(app.fundSourceCode);

                    return Json(new { status = "success", fundSourceData = fundSourceData }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = "HACKER ALERT" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        //NG
        [HttpPost]
        public JsonResult SubmitAddtnlFund(tRSPRefFundSourceDetail data)
        {
            try
            {
                string code = Session["FunSourceCode"].ToString();
                tRSPRefFundSourceDetail f = new tRSPRefFundSourceDetail();
                f.fundSourceCode = code;
                f.particulars = data.particulars;
                f.amount = data.amount;
                f.userID = Session["_EIC"].ToString();
                f.transDT = DateTime.Now;
                db.tRSPRefFundSourceDetails.Add(f);
                db.SaveChanges();

                vRSPRefFundSource fs = db.vRSPRefFundSources.Single(e => e.fundSourceCode == code);
                IEnumerable<tRSPRefFundSourceDetail> fsDetails = db.tRSPRefFundSourceDetails.Where(e => e.fundSourceCode == code).OrderBy(e => e.recNo).ToList();
                IEnumerable<vRSPAppointmentNPEmployee> empList = db.vRSPAppointmentNPEmployees.Where(e => e.fundSourceCode == code).ToList();
                decimal bal = Convert.ToDecimal(fs.amount - empList.Sum(e => e.PS).Value);
                int pRate = Convert.ToInt16((bal / fs.amount) * 100);
                return Json(new { status = "success", fsData = fs, fsDetails = fsDetails, appnteeList = empList, balance = bal, rate = pRate }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// PACCO CHARGES MONITOING //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <returns></returns>

        public JsonResult FundItemData(int id)
        {
            try
            {
                tRSPRefFundSourceDetail fsd = db.tRSPRefFundSourceDetails.Single(e => e.recNo == id);
                return Json(new { status = "success", fundItem = fsd }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "PACCOSTAFF")]
        public JsonResult UpdateFundItemData(tRSPRefFundSourceDetail data)
        {
            try
            {
                tRSPRefFundSourceDetail fs = db.tRSPRefFundSourceDetails.Single(e => e.recNo == data.recNo);
                fs.particulars = data.particulars;
                fs.amount = data.amount;
                db.SaveChanges();
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [Authorize]
        public JsonResult SaveAdditionalFund(tRSPRefFundSourceDetail data)
        {
            try
            {
                tRSPRefFundSource src = db.tRSPRefFundSources.SingleOrDefault(e => e.fundSourceCode == data.fundSourceCode);
                if (src != null)
                {
                    string uEIC = Session["_EIC"].ToString();
                    tRSPRefFundSourceDetail f = new tRSPRefFundSourceDetail();
                    f.fundSourceCode = data.fundSourceCode;
                    f.particulars = data.particulars;
                    f.amount = data.amount;
                    f.userID = uEIC;
                    f.transDT = DateTime.Now;
                    db.tRSPRefFundSourceDetails.Add(f);
                    db.SaveChanges();
                    return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = "error_fundsource" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "PACCOSTAFF")]
        public JsonResult SearchAppointee(string id)
        {
            try
            {
                //IEnumerable<vRSPAppointmentNPEmployee> empList = db.vRSPAppointmentNPEmployees.Where(e => e.fullNameLast.Contains(id)).ToList();
                var list = db.vRSPAppointmentNPEmployees.Select(e => new
                {
                    e.recNo,
                    e.EIC,
                    e.idNo,
                    e.fullNameLast,
                    e.positionTitle,
                    e.salaryGrade,
                    e.salary,
                    e.salaryType,
                    e.fundSourceCode,
                    e.projectName,
                    e.departmentName,
                    e.employmentStatusCode,
                    e.CY,
                    e.PSTag,
                    e.tag,
                    e.appointmentCode
                }).Where(e => e.fullNameLast.Contains(id) && e.tag >= 0).OrderByDescending(e => e.fullNameLast).ThenByDescending(e => e.recNo).ToList();

                return Json(new { status = "success", list = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [Authorize(Roles = "PACCOSTAFF")]
        public ActionResult PACCOMonitoring()
        {
            return View();
        }

        public JsonResult ProjectMonitoring()
        {
            int cYear = 2021;

            IEnumerable<vRSPRefFundSource> sourceList = db.vRSPRefFundSources.Where(e => e.CY == cYear).ToList();
            //IEnumerable<vRSPAppointmentNPEmployee> empApptList = db.vRSPAppointmentNPEmployees.Where(e => e.CY.Value == cYear).ToList();

            List<vRSPRefFundSource> list = ProjectList(2021);

            //INCOMING APPOINTMENT
            int count = db.vRSPAppointmentNonPlantillas.Where(e => e.tag == 1).Count();
            //INCOMING HR NOTIFICATION
            int notify = db.tRSPAppointmentNotifies.Where(e => e.tag == 1).Count();

            return Json(new { status = "success", projList = list, iCount = count, notifyCount = notify }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetCurrentCharges()
        {
            //List<vRSPRefFundSource> list = ProjectList(2021);
            IEnumerable<vRSPRefFundSource> sourceList = db.vRSPRefFundSources.Where(e => e.CY >= 2021).ToList();
            return Json(new { status = "success", fundSource = sourceList }, JsonRequestBehavior.AllowGet);
        }


        //LIST PROJECTS
        private List<vRSPRefFundSource> ProjectList(int cy)
        {
            IEnumerable<vRSPRefFundSource> sourceList = db.vRSPRefFundSources.Where(e => e.CY >= cy).ToList();

            IEnumerable<vRSPAppointmentNPP> empApptList = db.vRSPAppointmentNPPS.Where(e => e.cy == cy).ToList();

            List<vRSPRefFundSource> list = new List<vRSPRefFundSource>();

            foreach (vRSPRefFundSource item in sourceList)
            {
                decimal postedPS = Convert.ToDecimal(empApptList.Where(e => e.fundSourceCode == item.fundSourceCode && e.PSTag == 1).Sum(e => e.total));
                decimal balance = Convert.ToDecimal(item.amount) - postedPS;

                int prcntg = 100;
                if (postedPS > 0 && balance > 0)
                {
                    prcntg = Convert.ToInt16((balance / Convert.ToDecimal(item.amount)) * 100);
                }

                if (balance < 0)
                {
                    prcntg = -100;
                }

                list.Add(new vRSPRefFundSource()
                {
                    fundSourceCode = item.fundSourceCode,
                    projectName = item.projectName,
                    departmentName = item.departmentName,
                    amount = item.amount,
                    branch = balance.ToString("#,##0.00"),
                    programCode = prcntg.ToString("#0"),
                    programName = balance.ToString("#,##0.00")
                });
            }
            return list;
        }


        [Authorize(Roles = "PACCOSTAFF")]
        [HttpPost]
        public JsonResult ProjectItemData(string id)
        {
            string code = id;

            vRSPRefFundSource fs = db.vRSPRefFundSources.Single(e => e.fundSourceCode == code);

            //vRSPRefFundSource fs = db.vRSPRefFundSources.Single(e => e.fundSourceCode == code);
            IEnumerable<tRSPRefFundSourceDetail> fsDetails = db.tRSPRefFundSourceDetails.Where(e => e.fundSourceCode == code).OrderBy(e => e.recNo).ToList();


            IEnumerable<vRSPAppointmentNPEmployee> list = db.vRSPAppointmentNPEmployees.Where(e => e.fundSourceCode == code && e.PSTag == 1).OrderBy(o => o.fullNameLast).ToList();
            TempFundSourceApp fund = new TempFundSourceApp();
            fund.fundSourceCode = fs.fundSourceCode;
            fund.projectName = fs.projectName;
            fund.departmentName = fs.departmentName;
            if (list.Count() > 0)
            {
                var item = list.First();
                decimal totPosted = Convert.ToDecimal(list.Where(e => e.PSTag == 1).Sum(s => s.totalPS));
                decimal balance = Convert.ToDecimal(fs.amount) - totPosted;

                int percentage = 0;

                if (balance > 0 && Convert.ToDecimal(fs.amount) > 0)
                {
                    percentage = Convert.ToInt16((balance / Convert.ToDecimal(fs.amount)) * 100);
                }

                if (balance < 0)
                {
                    percentage = -100;
                }


                fund.fundSourceCode = fs.fundSourceCode;
                fund.amount = Convert.ToDecimal(fs.amount);
                fund.balance = Convert.ToDecimal(fs.amount) - totPosted;
                fund.percentage = Convert.ToInt16(percentage);
            }
            else
            {
                fund.amount = Convert.ToDecimal(fs.amount);
                fund.balance = Convert.ToDecimal(fs.amount);
                fund.percentage = Convert.ToInt16(100);
            }

            //vRSPRefFundSource fs = db.vRSPRefFundSources.Single(e => e.fundSourceCode == item.fundSourceCode);          
            //return Json(new { status = "success", fsData = fs, fsDetails = fsDetails, appnteeList = empList, balance = bal, rate = pRate }, JsonRequestBehavior.AllowGet);

            TempFundSourceData fundSourceData = EmployeeByCharges(code);
            IEnumerable<tRSPRefFundSourceDetail> approList = db.tRSPRefFundSourceDetails.Where(e => e.fundSourceCode == code).OrderBy(e => e.recNo).ToList();
            return Json(new { status = "success", fundSourceData = fundSourceData, approList = approList }, JsonRequestBehavior.AllowGet);

            //return Json(new { status = "success", list = list, appnteeList = list, fundData = fund, fsDetails = fsDetails }, JsonRequestBehavior.AllowGet);

        }



        [Authorize(Roles = "PACCOSTAFF")]
        [HttpPost]
        public JsonResult AppointmentList()
        {
            try
            {

                int cy = 2021;
                IEnumerable<vRSPAppointmentNonPlantilla> list = db.vRSPAppointmentNonPlantillas.Where(e => e.tag == 1).ToList();
                //IEnumerable<vRSPAppointmentNPEmployee> empList = db.vRSPAppointmentNPEmployees.Where(e => e.CY == cy).ToList();
                IEnumerable<vRSPAppointmentNPP> empList = db.vRSPAppointmentNPPS.Where(e => e.cy == cy).ToList();


                List<TempAppointmentList> myList = new List<TempAppointmentList>();
                foreach (vRSPAppointmentNonPlantilla item in list)
                {

                    //IEnumerable<vRSPAppointmentNPEmployee> groupList = empList.Where(e => e.appointmentCode == item.appointmentCode).OrderBy(o => o.lastName).ThenBy(o => o.firstName).ToList();

                    IEnumerable<vRSPAppointmentNPP> groupList = empList.Where(e => e.appointmentCode == item.appointmentCode).OrderBy(o => o.lastName).ThenBy(o => o.firstName).ToList();


                    if (groupList.Count() > 0)
                    {
                        vRSPAppointmentNPP firstEmp = groupList.First();
                        string tmpName = firstEmp.lastName + ", " + firstEmp.firstName;
                        if (groupList.Count() > 1)
                        {
                            tmpName = tmpName + " et al.,";
                        }
                        myList.Add(new TempAppointmentList()
                        {
                            appointmentCode = item.appointmentCode,
                            appointmentName = tmpName,
                            employmentStatus = item.employmentStatusNameShort,
                            itemCount = groupList.Count(),
                            projectName = item.projectName,
                            departmentName = item.departmentName,
                            //period = AppointmentPeriod(Convert.ToDateTime(firstEmp), Convert.ToDateTime(firstEmp.periodTo)),
                            tag = Convert.ToInt16(item.tag)
                        });
                    }
                }

                int count = myList.Count();
                myList = myList.OrderBy(o => o.appointmentName).ToList();

                return Json(new { status = "success", appList = myList, iCount = count }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "PACCOSTAFF")]
        [HttpPost]
        public JsonResult GetHRNotification()
        {
            try
            {
                IEnumerable<vRSPAppointmentNotify> list = db.vRSPAppointmentNotifies.Where(e => e.tag == 1).OrderBy(o => o.fullNameFirst).ToList();
                //ACTIVE NOTIFICATION
                IEnumerable<tRSPAppointmentNotify> separationList = db.tRSPAppointmentNotifies.Where(e => e.tag == 1).ToList();
                List<TempFundSourceEmp> myList = new List<TempFundSourceEmp>();

                foreach (vRSPAppointmentNotify item in list)
                {

                    string tmpRemarks = "";
                    tRSPAppointmentNotify sepItem = separationList.SingleOrDefault(e => e.notificationCode == item.notificationCode);
                    int sepTag = 0;

                    string termCode = "";
                    if (sepItem != null)
                    {
                        if (sepItem.tag == 1)
                        {
                            sepTag = 1;
                            termCode = sepItem.termCode;
                            tmpRemarks = sepItem.remarks;
                        }
                    }

                    myList.Add(new TempFundSourceEmp()
                    {
                        notificationCode = item.notificationCode,
                        appointmentItemCode = item.appointmentItemCode,
                        fullNameLast = item.fullNameLast,
                        fullNameFirst = item.fullNameFirst,
                        positionTitle = item.positionTitle,
                        salary = Convert.ToDecimal(item.salary),
                        salaryType = item.salaryType,
                        periodFrom = Convert.ToDateTime(item.periodFrom),
                        periodTo = Convert.ToDateTime(item.periodTo),
                        employmentStatusCode = item.employmentStatusCode,
                        employmentStatus = item.employmentStatus,
                        hasClothing = Convert.ToInt16(item.hasClothing),
                        hasBonusMY = Convert.ToInt16(item.hasBonusMY),
                        hasBonusYE = Convert.ToInt16(item.hasBonusYE),
                        hazardCode = Convert.ToInt16(item.hazardCode),
                        adjstmntFrom = Convert.ToDateTime(item.adjstmntFrom),
                        adjstmntTo = Convert.ToDateTime(item.adjstmntTo),
                        projectName = item.projectName,
                        termCode = termCode,
                        notifyRemarks = item.remarks
                    });
                }

                return Json(new { status = "success", list = myList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }


        [Authorize(Roles = "PACCOSTAFF")]
        [HttpPost]
        public JsonResult ExcludedList()
        {
            try
            {
                IEnumerable<vRSPAppointmentNotify> excldList = db.vRSPAppointmentNotifies.Where(e => e.tag == 0 && e.termCode == "EXCLUSION");
                return Json(new { status = "success", list = excldList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }

        private int monthCounter(DateTime f, DateTime t)
        {


            DateTime r = f;
            for (int i = 0; i < 13; i++)
            {
                if (r.Year == t.Year && r.Month == t.Month)
                {
                    return i;
                }
                r = r.AddMonths(1);
            }
            return 0;
        }


        private string AppointmentPeriod(DateTime f, DateTime t)
        {

            if (f == null)
            {
                return "TBD";
            }

            try
            {
                string res;
                //JAN-MAR, 2021
                res = f.ToString("MMM") + "-" + t.ToString("MMM") + ", " + t.ToString("yyyy");
                return res.ToUpper();
            }
            catch (Exception)
            {
                return "";
            }
        }

        public class TempFundSourceApp
        {
            public string fundSourceCode { get; set; }
            public string projectName { get; set; }
            public string departmentName { get; set; }
            public decimal amount { get; set; }
            public decimal balance { get; set; }
            public decimal percentage { get; set; }

        }


        [HttpPost]
        public JsonResult AppointeeListByFundSource(string id)
        {
            try
            {
                int cYear = 2021;
                //IEnumerable<vRSPAppointmentNPEmployee> empApptList = db.vRSPAppointmentNPEmployees.Where(e => e.CY.Value == cYear).ToList();
                IEnumerable<vRSPAppointmentNPP> empApptList = db.vRSPAppointmentNPPS.Where(e => e.cy == cYear).ToList();


                IEnumerable<vRSPAppointmentNPEmployee> list = db.vRSPAppointmentNPEmployees.Where(e => e.appointmentCode == id && e.tag >= 0).OrderBy(o => o.lastName).ThenBy(o => o.firstName).ToList();
                var item = list.First();
                vRSPRefFundSource fs = db.vRSPRefFundSources.Single(e => e.fundSourceCode == item.fundSourceCode);

                List<vRSPAppointmentNPEmployee> myList = new List<vRSPAppointmentNPEmployee>();
                foreach (vRSPAppointmentNPEmployee itm in list)
                {
                    DateTime tmpFrom = Convert.ToDateTime(itm.periodFrom);
                    if (tmpFrom.Year < 2000)
                    {
                        itm.periodFrom = null;
                    }

                    myList.Add(new vRSPAppointmentNPEmployee()
                    {
                        appointmentItemCode = itm.appointmentItemCode,
                        appointmentCode = itm.appointmentCode,
                        fullNameLast = itm.fullNameLast,
                        positionTitle = itm.positionTitle,
                        periodFrom = itm.periodFrom,
                        periodTo = itm.periodTo,
                        salary = itm.salary,
                        salaryType = itm.salaryType,
                        PSTag = itm.PSTag
                    });
                }


                IEnumerable<vRSPAppointmentNPP> tempPosted = empApptList.Where(e => e.fundSourceCode == item.fundSourceCode && e.PSTag == 1).ToList();
                decimal totPosted = Convert.ToDecimal(tempPosted.Sum(t => t.total));

                decimal balance = Convert.ToDecimal(fs.amount) - totPosted;

                decimal tempBalance = 0;
                decimal percentage = 0;

                if (Convert.ToDecimal(fs.amount) > 0)
                {
                    tempBalance = balance / Convert.ToDecimal(fs.amount);
                    percentage = Convert.ToDecimal(tempBalance) * 100;
                }

                TempFundSourceApp fund = new TempFundSourceApp();
                fund.fundSourceCode = fs.fundSourceCode;
                fund.amount = Convert.ToDecimal(fs.amount);
                fund.balance = Convert.ToDecimal(fs.amount) - totPosted;
                fund.percentage = percentage;
                fund.projectName = fs.projectName;

                int unPostedCount = list.Where(e => e.PSTag < 1 || e.PSTag == null).Count();

                if (unPostedCount == 0)
                {
                    tRSPAppointmentCasual appt = db.tRSPAppointmentCasuals.Single(e => e.appointmentCode == id);
                    appt.tag = 2;
                    db.SaveChanges();
                }

                return Json(new { status = "success", list = myList, fundData = fund, unPosted = unPostedCount }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }




        public JsonResult AppointeeListByFundSource2(string id)
        {
            try
            {
                int cYear = 2021;
                //IEnumerable<vRSPAppointmentNPEmployee> empApptList = db.vRSPAppointmentNPEmployees.Where(e => e.CY.Value == cYear).ToList();
                IEnumerable<vRSPAppointmentNPP> empApptList = db.vRSPAppointmentNPPS.Where(e => e.cy == cYear).ToList();


                IEnumerable<vRSPAppointmentNPEmployee> list = db.vRSPAppointmentNPEmployees.Where(e => e.appointmentCode == id && e.tag >= 0).OrderBy(o => o.lastName).ThenBy(o => o.firstName).ToList();

                var item = list.First();
                vRSPRefFundSource fs = db.vRSPRefFundSources.Single(e => e.fundSourceCode == item.fundSourceCode);

                List<vRSPAppointmentNPEmployee> myList = new List<vRSPAppointmentNPEmployee>();
                foreach (vRSPAppointmentNPEmployee itm in list)
                {
                    DateTime tmpFrom = Convert.ToDateTime(itm.periodFrom);
                    if (tmpFrom.Year < 2000)
                    {
                        itm.periodFrom = null;
                    }

                    myList.Add(new vRSPAppointmentNPEmployee()
                    {
                        appointmentItemCode = itm.appointmentItemCode,
                        appointmentCode = itm.appointmentCode,
                        fullNameLast = itm.fullNameLast,
                        positionTitle = itm.positionTitle,
                        periodFrom = itm.periodFrom,
                        periodTo = itm.periodTo,
                        salary = itm.salary,
                        salaryType = itm.salaryType,
                        PSTag = itm.PSTag
                    });
                }

                IEnumerable<vRSPAppointmentNPP> tempPosted = empApptList.Where(e => e.fundSourceCode == item.fundSourceCode && e.PSTag == 1).ToList();
                decimal totPosted = Convert.ToDecimal(tempPosted.Sum(t => t.total));
                decimal balance = Convert.ToDecimal(fs.amount) - totPosted;

                decimal tempBalance = 0;
                decimal percentage = 0;
                if (Convert.ToDecimal(fs.amount) > 0)
                {
                    tempBalance = balance / Convert.ToDecimal(fs.amount);
                    percentage = Convert.ToDecimal(tempBalance) * 100;
                }

                // *** 

                decimal postedPS = Convert.ToDecimal(empApptList.Where(e => e.fundSourceCode == item.fundSourceCode && e.PSTag == 1).Sum(e => e.total));
                //decimal balance = Convert.ToDecimal(fs.amount) - postedPS;

                // ***


                TempFundSourceApp fund = new TempFundSourceApp();
                fund.fundSourceCode = fs.fundSourceCode;
                fund.amount = Convert.ToDecimal(fs.amount);
                fund.balance = Convert.ToDecimal(fs.amount) - totPosted;
                fund.percentage = percentage;
                fund.projectName = fs.projectName;

                int unPostedCount = list.Where(e => e.PSTag != 1).Count();

                string tmpName = item.fullNameLast;
                if (list.Count() > 1)
                {
                    tmpName = tmpName + " et al.,";
                }

                TempAppointmentList data = new TempAppointmentList();
                data.projectName = item.programName;
                data.itemCount = list.Count();
                data.employmentStatus = item.employmentStatus;
                data.appointmentName = tmpName;
                data.period = AppointmentPeriod(Convert.ToDateTime(item.periodFrom), Convert.ToDateTime(item.periodTo));

                return Json(new { status = "success", list = myList, apptData = data, fundData = fund, unPosted = unPostedCount }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Authorize(Roles = "PACCOSTAFF")]
        public JsonResult ViewAppointmentItem(string id)
        {

            TempAppointmentNPPosting postingData = new TempAppointmentNPPosting();

            tRSPZPersonnelService personnelService = new tRSPZPersonnelService();
            postingData.hazardCode = 0;

            try
            {
                int hazardTag = 0;
                int casualTag = 0;
                vRSPAppointmentNPEmployee app = db.vRSPAppointmentNPEmployees.Single(e => e.appointmentItemCode == id);

                if (app.employmentStatusCode == "05") //CASUAL
                {
                    casualTag = 1;
                    if (app.departmentCode == "OC191017163920527001" || app.departmentCode == "OC191017164319324001" || app.departmentCode == "OC191017163949719001")
                    {
                        hazardTag = 1;
                    }
                }
                else if (app.employmentStatusCode == "06") // J.O.
                {
                    hazardTag = 0;
                    casualTag = 0;
                }
                else if (app.employmentStatusCode == "07" || app.employmentStatusCode == "08") // C.O.S. & HON.
                {

                }

                return Json(new { status = "success", ps = personnelService, hazardTag = hazardTag, casualTag = casualTag }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Authorize(Roles = "PACCOSTAFF,APRD")]
        public JsonResult ReCalculatePS(string id, TempAppointmentNPPosting postData)
        {

            int monthCount = Monthcounter(Convert.ToDateTime(postData.periodFrom), Convert.ToDateTime(postData.periodTo));



            if (monthCount > 12 || monthCount <= 0 || Convert.ToDateTime(postData.periodFrom).Year != Convert.ToDateTime(postData.periodTo).Year)
            {
                return Json(new { status = "Invalid employment period!" }, JsonRequestBehavior.AllowGet);
            }


            try
            {
                tRSPZPersonnelService personnelService = new tRSPZPersonnelService();
                //postData.hazardCode = 0;
                vRSPAppointmentNPEmployee app = db.vRSPAppointmentNPEmployees.Single(e => e.appointmentItemCode == id);

                if (app.employmentStatusCode == "05") //CASUAL      
                {
                    tRSPPosition position = db.tRSPPositions.Single(e => e.positionCode == app.positionCode);
                    vRSPSalarySchedCasual sched = db.vRSPSalarySchedCasuals.Single(e => e.salaryGrade == app.salaryGrade);
                    tRSPZPersonnelService p = new tRSPZPersonnelService();
                    personnelService = GetPositionCasualPS(sched, position, Convert.ToInt16(postData.hazardCode), Convert.ToDateTime(postData.periodFrom), Convert.ToDateTime(postData.periodTo), postData);
                }
                else if (app.employmentStatusCode == "06") // J.O.
                {
                    vRSPSalarySchedJO sched = db.vRSPSalarySchedJOes.Single(e => e.salaryGrade == app.salaryGrade);
                    personnelService = GetPositionJobOrderPS(sched, Convert.ToDateTime(postData.periodFrom), Convert.ToDateTime(postData.periodTo));
                }
                else if (app.employmentStatusCode == "07" || app.employmentStatusCode == "08") // C.O.S. & HON.
                {
                    tRSPPositionContract posContract = db.tRSPPositionContracts.SingleOrDefault(e => e.positionCode == app.positionCode);
                    personnelService = GetPositionContractHon(posContract, Convert.ToDateTime(postData.periodFrom), Convert.ToDateTime(postData.periodTo));
                }

                int updateAlert = 0;
                tRSPZPersonnelService currentPS = db.tRSPZPersonnelServices.SingleOrDefault(e => e.appointmentItemCode == id);
                if (currentPS != null)
                {
                    if (currentPS.total != personnelService.total)
                    {
                        updateAlert = 1;
                    }
                }

                return Json(new { status = "success", ps = personnelService, updateAlert = updateAlert }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Authorize(Roles = "PACCOSTAFF,APRD")]

        //QuickFixPS
        public JsonResult QuickFixPS(string id, TempAppointmentNPPosting postData)
        {
            try
            {
                tRSPZPersonnelService p = new tRSPZPersonnelService();
                vRSPAppointmentNPEmployee app = db.vRSPAppointmentNPEmployees.Single(e => e.appointmentItemCode == id);

                if (app.employmentStatusCode == "05") //CASUAL      
                {
                    tRSPPosition position = db.tRSPPositions.Single(e => e.positionCode == app.positionCode);
                    vRSPSalarySchedCasual sched = db.vRSPSalarySchedCasuals.Single(e => e.salaryGrade == app.salaryGrade);
                    //tRSPZPersonnelService p = new tRSPZPersonnelService();
                    p = GetPositionCasualPS(sched, position, Convert.ToInt16(postData.hazardCode), Convert.ToDateTime(postData.periodFrom), Convert.ToDateTime(postData.periodTo), postData);
                }
                else if (app.employmentStatusCode == "06") // J.O.
                {
                    vRSPSalarySchedJO sched = db.vRSPSalarySchedJOes.Single(e => e.salaryGrade == app.salaryGrade);
                    p = GetPositionJobOrderPS(sched, Convert.ToDateTime(postData.periodFrom), Convert.ToDateTime(postData.periodTo));
                }
                else if (app.employmentStatusCode == "07" || app.employmentStatusCode == "08") // C.O.S. & HON.
                {
                    tRSPPositionContract posContract = db.tRSPPositionContracts.SingleOrDefault(e => e.positionCode == app.positionCode);
                    p = GetPositionContractHon(posContract, Convert.ToDateTime(postData.periodFrom), Convert.ToDateTime(postData.periodTo));
                }


                tRSPZPersonnelService n = db.tRSPZPersonnelServices.Single(e => e.appointmentItemCode == id);
                n.periodFrom = Convert.ToDateTime(postData.periodFrom);
                n.periodTo = Convert.ToDateTime(postData.periodTo);

                n.annualRate = p.annualRate;
                n.PERA = p.PERA;
                n.leaveEarned = p.leaveEarned;
                n.hazardPay = p.hazardPay;
                n.laundry = p.laundry;
                n.subsistence = p.subsistence;

                n.lifeRetmnt = p.lifeRetmnt;
                n.ECC = p.ECC;
                n.hdmf = p.hdmf;
                n.phic = p.phic;

                n.clothing = p.clothing;
                n.midYearBonus = p.midYearBonus;
                n.yearEndBonus = p.yearEndBonus;
                n.cashGift = p.cashGift;
                n.total = p.total;

                db.SaveChanges();




                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult ForceFixPS(string code, decimal amt)
        {
            try
            {
                tRSPZPersonnelService n = db.tRSPZPersonnelServices.Single(e => e.appointmentItemCode == code);
                n.total = amt;
                db.SaveChanges();
                vRSPAppointmentNPEmployee app = db.vRSPAppointmentNPEmployees.Single(e => e.appointmentItemCode == code);
                TempFundSourceData fundSourceData = EmployeeByCharges(app.fundSourceCode);
                return Json(new { status = "success", fundSourceData = fundSourceData }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Authorize(Roles = "PACCOSTAFF")]
        public JsonResult ConfirmAppointmentItem(string id, TempAppointmentNPPosting postData, int unPostedCount)
        {
            try
            {

                DateTime df = Convert.ToDateTime(postData.periodFrom);
                DateTime dt = Convert.ToDateTime(postData.periodTo);

                tRSPZPersonnelService ps = db.tRSPZPersonnelServices.SingleOrDefault(e => e.appointmentItemCode == id && e.tag == 0);
                if (ps != null)
                {
                    vRSPAppointmentNPEmployee app = db.vRSPAppointmentNPEmployees.Single(e => e.appointmentItemCode == id);
                    tRSPZPersonnelService p = new tRSPZPersonnelService();

                    if (app.employmentStatusCode == "05") //CASUAL      
                    {
                        tRSPPosition position = db.tRSPPositions.Single(e => e.positionCode == app.positionCode);
                        vRSPSalarySchedCasual sched = db.vRSPSalarySchedCasuals.Single(e => e.salaryGrade == app.salaryGrade);
                        p = GetPositionCasualPS(sched, position, Convert.ToInt16(postData.hazardCode), df, dt, postData);
                    }
                    else if (app.employmentStatusCode == "06") // J.O.
                    {
                        vRSPSalarySchedJO sched = db.vRSPSalarySchedJOes.Single(e => e.salaryGrade == app.salaryGrade);
                        p = GetPositionJobOrderPS(sched, df, dt);
                    }
                    else if (app.employmentStatusCode == "07" || app.employmentStatusCode == "08") // C.O.S. & HON.
                    {
                        tRSPPositionContract posContract = db.tRSPPositionContracts.SingleOrDefault(e => e.positionCode == app.positionCode);
                        p = GetPositionContractHon(posContract, df, dt);
                    }

                    int cYear = 2021;
                    //IEnumerable<vRSPAppointmentNPEmployee> empApptList = db.vRSPAppointmentNPEmployees.Where(e => e.CY.Value == cYear && e.fundSourceCode == app.fundSourceCode).ToList();
                    IEnumerable<vRSPAppointmentNPP> empApptList = db.vRSPAppointmentNPPS.Where(e => e.cy == cYear && e.fundSourceCode == app.fundSourceCode).ToList();

                    vRSPRefFundSource fs = db.vRSPRefFundSources.Single(e => e.fundSourceCode == app.fundSourceCode);
                    IEnumerable<vRSPAppointmentNPP> tempPosted = empApptList.Where(e => e.PSTag == 1).ToList();
                    decimal totPosted = Convert.ToDecimal(tempPosted.Sum(t => t.total));
                    decimal balance = Convert.ToDecimal(fs.amount) - totPosted;

                    decimal currentBalance = balance;
                    decimal appItemPS = Convert.ToDecimal(p.total);

                    int isAllowed = 0;
                    if (fs.hasLimit == 0)
                    {
                        isAllowed = 1;
                    }
                    else
                    {
                        if (currentBalance >= appItemPS)
                        {
                            isAllowed = 1;
                        }
                    }

                    if (isAllowed == 1)
                    {
                        ps.periodFrom = df;
                        ps.periodTo = dt;
                        ps.annualRate = p.annualRate;
                        ps.PERA = p.PERA;
                        ps.leaveEarned = p.leaveEarned;
                        ps.hazardPay = p.hazardPay;
                        ps.laundry = p.laundry;
                        ps.subsistence = p.subsistence;

                        ps.lifeRetmnt = p.lifeRetmnt;
                        ps.ECC = p.ECC;
                        ps.hdmf = p.hdmf;
                        ps.phic = p.phic;

                        ps.clothing = p.clothing;
                        ps.midYearBonus = p.midYearBonus;
                        ps.yearEndBonus = p.yearEndBonus;
                        ps.cashGift = p.cashGift;
                        ps.total = p.total;
                        ps.PSTag = 1;
                        unPostedCount = unPostedCount - 1;
                        if (unPostedCount <= 0)
                        {
                            tRSPAppointmentCasual appt = db.tRSPAppointmentCasuals.Single(e => e.appointmentCode == ps.appointmentCode);
                            appt.tag = 2;
                            //0 created
                            //1 FORWARDED TO PACCO
                            //2 ALL ITEM POSTED BY PACCO
                            //3 PHRMO POSTING
                        }
                        else
                        {
                            unPostedCount = unPostedCount - 1;
                        }

                        //tRSPAppointmentCasualEmp empApp = db.tRSPAppointmentCasualEmps.Single(e => e.appointmentItemCode == id);
                        //empApp.periodFrom = dt;
                        db.SaveChanges();


                        IEnumerable<vRSPAppointmentNPEmployee> list = db.vRSPAppointmentNPEmployees.Where(e => e.appointmentCode == ps.appointmentCode && e.empTag >= 0).OrderBy(o => o.fullNameLast).ToList();

                        return Json(new { status = "success", list = list, unPosted = unPostedCount }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { status = "Insufficient balance!" }, JsonRequestBehavior.AllowGet);
                    }


                }

                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }


        //RECALCULATE
        [HttpPost]
        [Authorize(Roles = "PACCOSTAFF,APRDHEAD,APRD")]
        public JsonResult ReCalculatePSAdjsmnt(string appItemCode, TempAppointmentNPPosting postData)
        {
            DateTime df = DateTime.Now;
            DateTime dt = DateTime.Now;
            tRSPZPersonnelService personnelService = new tRSPZPersonnelService();

            if (postData.termCode == null)
            {
                postData.termCode = "";
            }

            if (postData.termCode.ToUpper() == "EXCLUSION")
            {
                tRSPZPersonnelService res = new tRSPZPersonnelService();
                res.dailyRate = 0;
                res.monthlyRate = 0;
                res.annualRate = 0;
                res.PERA = 0;
                res.leaveEarned = 0;
                res.hazardPay = 0;
                res.subsistence = 0;
                res.laundry = 0;
                res.midYearBonus = 0;
                res.yearEndBonus = 0;
                res.cashGift = 0;
                res.lifeRetmnt = 0;
                res.ECC = 0;
                res.hdmf = 0;
                res.phic = 0;
                res.clothing = 0;
                res.total = 0;
                res.recNo = 0;
                return Json(new { status = "success", ps = res }, JsonRequestBehavior.AllowGet);
            }

            tRSPAppointmentNotify sep = new tRSPAppointmentNotify();
            if (appItemCode == null || appItemCode == "")
            {
                df = Convert.ToDateTime(postData.periodFrom);
                dt = Convert.ToDateTime(postData.periodTo);
            }
            else
            {
                sep = db.tRSPAppointmentNotifies.Single(e => e.notificationCode == appItemCode);
                df = Convert.ToDateTime(sep.adjstmntFrom);
                dt = Convert.ToDateTime(sep.adjstmntTo);
            }

            try
            {
                //postData.hazardCode = 0;
                vRSPAppointmentNPEmployee app = db.vRSPAppointmentNPEmployees.Single(e => e.appointmentItemCode == postData.appointmentItemCode);

                if (app.employmentStatusCode == "05") //CASUAL      
                {
                    tRSPPosition position = db.tRSPPositions.Single(e => e.positionCode == app.positionCode);
                    vRSPSalarySchedCasual sched = db.vRSPSalarySchedCasuals.Single(e => e.salaryGrade == app.salaryGrade);
                    tRSPZPersonnelService p = new tRSPZPersonnelService();
                    //app.periodTo = Convert.ToDateTime(postData.periodTo); //ADJUSTMENT PERIOD TO
                    personnelService = GetPositionCasualPS(sched, position, Convert.ToInt16(postData.hazardCode), df, dt, postData);
                }
                else if (app.employmentStatusCode == "06") // J.O.
                {
                    vRSPSalarySchedJO sched = db.vRSPSalarySchedJOes.Single(e => e.salaryGrade == app.salaryGrade);
                    personnelService = GetPositionJobOrderPS(sched, df, dt);
                }
                else if (app.employmentStatusCode == "07" || app.employmentStatusCode == "08") // C.O.S. & HON.
                {
                    tRSPPositionContract posContract = db.tRSPPositionContracts.SingleOrDefault(e => e.positionCode == app.positionCode);
                    personnelService = GetPositionContractHon(posContract, df, dt);
                }

                return Json(new { status = "success", ps = personnelService }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        [Authorize(Roles = "PACCOSTAFF")]
        public JsonResult SaveAppointmentAdjsmnt(string appItemCode, TempAppointmentNPPosting postData)
        {
            try
            {
                tRSPAppointmentNotify sep = db.tRSPAppointmentNotifies.Single(e => e.notificationCode == appItemCode && e.tag == 1);
                if (sep == null)
                {
                    return Json(new { status = "HACKER ALERT" }, JsonRequestBehavior.AllowGet);
                }
                int ps_Tag = 0;
                int ps_PSTag = 1;

                tRSPZPersonnelService p = new tRSPZPersonnelService();
                tRSPZPersonnelService ps = db.tRSPZPersonnelServices.SingleOrDefault(e => e.appointmentItemCode == sep.appointmentItemCode);
                vRSPAppointmentNPEmployee app = db.vRSPAppointmentNPEmployees.Single(e => e.appointmentItemCode == sep.appointmentItemCode);
                DateTime df = Convert.ToDateTime(sep.adjstmntFrom);
                DateTime dt = Convert.ToDateTime(sep.adjstmntTo);

                if (sep.termCode == "EXCLUSION")
                {
                    tRSPZPersonnelService res = new tRSPZPersonnelService();
                    res.dailyRate = 0;
                    res.monthlyRate = 0;
                    res.annualRate = 0;
                    res.PERA = 0;
                    res.leaveEarned = 0;
                    res.hazardPay = 0;
                    res.subsistence = 0;
                    res.laundry = 0;
                    res.midYearBonus = 0;
                    res.yearEndBonus = 0;
                    res.cashGift = 0;
                    res.lifeRetmnt = 0;
                    res.ECC = 0;
                    res.hdmf = 0;
                    res.phic = 0;
                    res.clothing = 0;
                    res.total = 0;
                    res.periodFrom = null;
                    res.periodTo = null;
                    p = res;
                    ps_Tag = -1;
                    ps_PSTag = -1;

                }
                else
                {
                    df = Convert.ToDateTime(sep.adjstmntFrom);
                    dt = Convert.ToDateTime(sep.adjstmntTo);
                    ps.periodFrom = df;
                    ps.periodTo = df;
                    if (app.employmentStatusCode == "05") //CASUAL      
                    {
                        tRSPPosition position = db.tRSPPositions.Single(e => e.positionCode == app.positionCode);
                        vRSPSalarySchedCasual sched = db.vRSPSalarySchedCasuals.Single(e => e.salaryGrade == app.salaryGrade);
                        p = GetPositionCasualPS(sched, position, Convert.ToInt16(postData.hazardCode), df, dt, postData);

                    }
                    else if (app.employmentStatusCode == "06") // J.O.
                    {
                        vRSPSalarySchedJO sched = db.vRSPSalarySchedJOes.Single(e => e.salaryGrade == app.salaryGrade);
                        p = GetPositionJobOrderPS(sched, df, dt);
                    }
                    else if (app.employmentStatusCode == "07" || app.employmentStatusCode == "08") // C.O.S. & HON.
                    {
                        tRSPPositionContract posContract = db.tRSPPositionContracts.SingleOrDefault(e => e.positionCode == app.positionCode);
                        p = GetPositionContractHon(posContract, df, dt);
                    }
                }

                tRSPAppointmentCasualEmp appt = db.tRSPAppointmentCasualEmps.Single(e => e.appointmentItemCode == app.appointmentItemCode);

                int emptTag = -1; //TERMINATED
                if (postData.termCode.ToUpper() == "ADJUSTMENT")
                {
                    emptTag = 1;
                }



                appt.tag = emptTag;
                appt.PS = p.total;

                ps.annualRate = p.annualRate;
                ps.PERA = p.PERA;
                ps.leaveEarned = p.leaveEarned;
                ps.hazardPay = p.hazardPay;
                ps.laundry = p.laundry;
                ps.subsistence = p.subsistence;

                ps.lifeRetmnt = p.lifeRetmnt;
                ps.ECC = p.ECC;
                ps.hdmf = p.hdmf;
                ps.phic = p.phic;

                ps.clothing = p.clothing;
                ps.midYearBonus = p.midYearBonus;
                ps.yearEndBonus = p.yearEndBonus;
                ps.cashGift = p.cashGift;



                ps.periodFrom = p.periodFrom;
                ps.periodTo = p.periodTo;

                ps.tag = ps_Tag;
                ps.PSTag = ps_PSTag;
                ps.total = p.total;

                sep.tag = 0;
                sep.postedByEIC = Session["_EIC"].ToString();
                sep.postedDT = DateTime.Now;
                db.SaveChanges();

                IEnumerable<vRSPAppointmentNotify> notify = db.vRSPAppointmentNotifies.Where(e => e.tag == 1).OrderBy(o => o.fullNameFirst).ToList();
                return Json(new { status = "success", ps = ps, notify = notify }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }




        public class TempAppointmentNPPosting
        {
            public string appointmentItemCode { get; set; }
            public string periodFrom { get; set; }
            public int hazardCode { get; set; }
            public int hasClothing { get; set; }
            public int hasMidYear { get; set; }
            public int hasYearEnd { get; set; }
            public string periodTo { get; set; }
            public string termCode { get; set; }
        }


        //ShowEmployeePS
        //SHOW EMPLOYEE APPOINTMENT PS
        [HttpPost]
        public JsonResult ShowEmployeePS(TempAppointmentNPPosting postData)
        {
            try
            {
                tRSPZPersonnelService psData = db.tRSPZPersonnelServices.Single(e => e.appointmentItemCode == postData.appointmentItemCode);
                psData.recNo = 0;
                return Json(new { status = "success", ps = psData }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }

        private tRSPZPersonnelService GetPositionCasualPS(vRSPSalarySchedCasual salarySched, tRSPPosition position, int hazardCode, DateTime periodFrom, DateTime periodTo, TempAppointmentNPPosting postData)
        {
            tRSPZPersonnelService res = new tRSPZPersonnelService();

            int monthCount = Monthcounter(periodFrom, periodTo);
            decimal dailyRate = Convert.ToDecimal(salarySched.rateDay);
            decimal monthlyRate = Convert.ToDecimal(salarySched.rateMonth);
            decimal annualSalary = monthlyRate * monthCount;
            decimal PERA = 2000 * monthCount;
            decimal earnedLeave = (monthlyRate / 8) * monthCount;

            decimal hazardPay = 0;
            decimal subsistence = 0;
            decimal laundry = 0;
            if (hazardCode == 1)
            {
                //HEALTH SERVICES
                hazardPay = CalQHazardForHealthMonthly(Convert.ToInt16(position.salaryGrade), Convert.ToDecimal(dailyRate)) * monthCount;
                //SUBS. & Luandry
                subsistence = 1500 * monthCount;
                laundry = 150 * monthCount;
            }
            else if (hazardCode == 2)
            {
                //SOCIAL WORKER
                hazardPay = CalQHazardForSocialWorkerMonthly(dailyRate) * monthCount;
                //SUBS ONLY
                subsistence = 1500 * monthCount;
            }


            decimal midYearBonus = 0;
            DateTime checkDateFrom = periodFrom;
            DateTime cuttOffFrom = Convert.ToDateTime("January 15, " + periodTo.Year.ToString());
            //
            if (checkDateFrom == null || checkDateFrom.Date.Year == 1)
            {
                if (DateTime.Now.Year == periodTo.Year)
                {
                    checkDateFrom = DateTime.Now;
                }
                else
                {
                    checkDateFrom = Convert.ToDateTime("January 11, " + periodTo.Year.ToString());
                }
            }
            if (checkDateFrom <= cuttOffFrom)
            {
                midYearBonus = monthlyRate;
            }

            //!IMPORTANT => MIDYEAR BONUS
            if (postData.hasMidYear == 1)
            {
                midYearBonus = monthlyRate;
            }
            else
            {
                midYearBonus = 0;
            }

            decimal yearEndBonus = 0;
            decimal cashGift = 0;
            DateTime cuttOffTo = Convert.ToDateTime("October 31, " + periodTo.Year.ToString());
            if (checkDateFrom <= cuttOffTo)
            {
                yearEndBonus = monthlyRate;
                cashGift = 5000;
            }

            //!IMPORTANT => YEAR-END BONUS
            if (postData != null && postData.hasYearEnd == 1)
            {
                yearEndBonus = monthlyRate;
                cashGift = 5000;
            }
            else
            {
                yearEndBonus = 0;
                cashGift = 0;
            }

            decimal lifeAndRetire = (Convert.ToDecimal(monthlyRate) * Convert.ToDecimal(.12)) * monthCount;
            decimal PHIC = CalQPHICGovtShare(Convert.ToDouble(monthlyRate), monthCount);

            decimal HDMF = 100 * monthCount;
            decimal ECC = 100 * monthCount;

            decimal clothing = 0;

            //MARCH 30 but if HEALTH PERSONNEL NO CUT-OFF
            if (hazardCode == 1)
            {
                clothing = 6000;
            }
            else
            {
                DateTime tmpCutOffClothing = Convert.ToDateTime("MARCH 30, " + periodTo.Year.ToString());
                if (checkDateFrom <= tmpCutOffClothing)
                {
                    clothing = 6000;
                }
            }
            //!IMPORTANT => CLOTHING
            if (postData.hasClothing == 1)
            {
                clothing = 6000;
            }
            else
            {
                clothing = 0;
            }

            decimal totalPS = annualSalary + PERA + earnedLeave + hazardPay + subsistence + laundry + midYearBonus + yearEndBonus + cashGift + lifeAndRetire + ECC + PHIC + HDMF + clothing;

            res.dailyRate = dailyRate;
            res.monthlyRate = monthlyRate;
            res.annualRate = annualSalary;
            res.PERA = PERA;
            res.leaveEarned = earnedLeave;
            res.hazardPay = hazardPay;
            res.subsistence = subsistence;
            res.laundry = laundry;
            res.midYearBonus = midYearBonus;
            res.yearEndBonus = yearEndBonus;
            res.cashGift = cashGift;
            res.lifeRetmnt = lifeAndRetire;
            res.ECC = ECC;
            res.hdmf = HDMF;
            res.phic = PHIC;
            res.clothing = clothing;
            res.total = totalPS;
            res.periodFrom = Convert.ToDateTime(periodFrom);
            res.periodTo = periodTo;
            res.recNo = monthCount;
            return res;

        }




        private tRSPZPersonnelService GetPositionJobOrderPS(vRSPSalarySchedJO salarySched, DateTime periodFrom, DateTime periodTo)
        {
            tRSPZPersonnelService ps = new tRSPZPersonnelService();
            try
            {
                int monthCount = Monthcounter(periodFrom, periodTo);
                decimal rate = Convert.ToDecimal(salarySched.rateDay);
                decimal monthlyRate = Convert.ToDecimal(salarySched.rateMonth);
                decimal annualSalary = monthlyRate * monthCount;
                decimal totalPS = annualSalary;
                ps.appointmentItemCode = "";
                ps.positionTitle = "";
                ps.dailyRate = salarySched.rateDay;
                ps.monthlyRate = salarySched.rateMonth;
                ps.annualRate = annualSalary;
                ps.PERA = 0;
                ps.leaveEarned = 0;
                ps.midYearBonus = 0;
                ps.yearEndBonus = 0;
                ps.cashGift = 0;
                ps.hazardPay = 0;
                ps.laundry = 0;
                ps.subsistence = 0;
                ps.lifeRetmnt = 0;
                ps.ECC = 0;
                ps.hdmf = 0;
                ps.phic = 0;
                ps.clothing = 0;
                ps.total = totalPS;
                ps.periodFrom = periodFrom;
                ps.periodTo = periodTo;
                ps.PSTag = 0;
                ps.recNo = monthCount;
                return ps;
            }
            catch (Exception)
            {
                return ps;
            }
        }


        private tRSPZPersonnelService GetPositionContractHon(tRSPPositionContract position, DateTime periodFrom, DateTime periodTo)
        {
            tRSPZPersonnelService ps = new tRSPZPersonnelService();
            try
            {
                int monthCount = Monthcounter(periodFrom, periodTo);

                decimal rateMonthly = Convert.ToDecimal(position.salary);
                decimal rateDaily = Convert.ToDecimal(rateMonthly / 22);

                decimal annualSalary = rateMonthly * monthCount;
                decimal totalPS = annualSalary;

                ps.appointmentItemCode = "";
                ps.positionTitle = "";
                ps.dailyRate = rateDaily;
                ps.monthlyRate = rateMonthly;
                ps.annualRate = annualSalary;
                ps.PERA = 0;
                ps.hazardPay = 0;
                ps.laundry = 0;
                ps.subsistence = 0;
                ps.leaveEarned = 0;
                ps.midYearBonus = 0;
                ps.yearEndBonus = 0;
                ps.cashGift = 0;
                ps.lifeRetmnt = 0;
                ps.ECC = 0;
                ps.hdmf = 0;
                ps.phic = 0;
                ps.clothing = 0;
                ps.total = totalPS;
                ps.periodFrom = Convert.ToDateTime(periodFrom);
                ps.periodTo = periodTo;
                ps.PSTag = 0;
                ps.recNo = monthCount;
                return ps;
            }
            catch (Exception)
            {
                return ps;
            }
        }

        private int Monthcounter(DateTime dFrom, DateTime dTo)
        {
            int res = 0;
            for (int i = 0; i < 12; i++)
            {
                res = res + 1;
                if (dFrom.Month == dTo.Month && dFrom.Year == dTo.Year)
                {
                    return res;
                }
                dFrom = dFrom.AddMonths(1);
            }
            return res;
        }


        private decimal CalQHazardForSocialWorkerMonthly(decimal dailyRate)
        {
            decimal dailyHazard = dailyRate * Convert.ToDecimal(.20);
            decimal hazardPay = (dailyHazard * 30);
            return hazardPay;
        }


        //HAZARD - Health
        private decimal CalQHazardForHealthMonthly(int SG, decimal dailyRate)
        {
            double prcntg = 0;

            if (SG <= 19)
            {
                prcntg = .25;
            }
            else if (SG == 20)
            {
                prcntg = .15;
            }
            else if (SG == 21)
            {
                prcntg = .13;
            }
            else if (SG == 22)
            {
                prcntg = .12;
            }
            else if (SG == 23)
            {
                prcntg = .11;
            }
            else if (SG == 24)
            {
                prcntg = .10;
            }
            else if (SG == 25)
            {
                prcntg = .10;
            }
            else if (SG == 26)
            {
                prcntg = .09;
            }
            else if (SG == 27)
            {
                prcntg = .08;
            }
            else if (SG == 28)
            {
                prcntg = .07;
            }
            else
            {
                prcntg = .05;
            }

            decimal monthlyHazard = 0;
            decimal monthlyRate = Convert.ToDecimal(dailyRate * 22);
            monthlyHazard = (monthlyRate * Convert.ToDecimal(prcntg));
            return monthlyHazard;
        }

        //
        private decimal CalQPHICGovtShare(double rateMonth, int monthCount)
        {
            double monthlyPrem = 400;
            if (rateMonth >= 10000.01 && rateMonth < 80000)
            {
                monthlyPrem = (rateMonth * .04) / 2;
            }
            else if (rateMonth >= 80000)
            {
                monthlyPrem = 3200 / 2;
            }
            return Convert.ToDecimal(monthlyPrem * monthCount);
        }



        [HttpPost]
        [Authorize(Roles = "PACCOSTAFF")]
        public JsonResult ViewAppoinmentItemData(string id)
        {
            try
            {
                tRSPZPersonnelService ps = db.tRSPZPersonnelServices.SingleOrDefault(e => e.appointmentItemCode == id && e.tag == 0);
                if (ps != null)
                {

                }

                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }




        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ActionResult PSChecker()
        {
            try
            {
                string id = "FSPROJ201118093919390";
                TempAppointmentNPPosting postData = new TempAppointmentNPPosting();
                postData.hasClothing = 1;
                postData.hasMidYear = 1;
                postData.hasYearEnd = 0;
                postData.hazardCode = 0;

                tRSPZPersonnelService personnelService = new tRSPZPersonnelService();
                IEnumerable<vRSPAppointmentNPEmployee> empList = db.vRSPAppointmentNPEmployees.Where(e => e.fundSourceCode == id && e.PSTag == 1).ToList();
                IEnumerable<tRSPZPersonnelService> psList = db.tRSPZPersonnelServices.Where(e => e.tag >= 0).ToList();

                foreach (vRSPAppointmentNPEmployee item in empList)
                {
                    //postData.hazardCode = 0;
                    vRSPAppointmentNPEmployee app = db.vRSPAppointmentNPEmployees.Single(e => e.appointmentItemCode == item.appointmentItemCode);
                    //CURRENT PS
                    tRSPZPersonnelService psItem = psList.Single(e => e.appointmentItemCode == item.appointmentItemCode);

                    DateTime df = Convert.ToDateTime(psItem.periodFrom);
                    DateTime dt = Convert.ToDateTime(psItem.periodTo);

                    if (app.employmentStatusCode == "05") //CASUAL      
                    {
                        tRSPPosition position = db.tRSPPositions.Single(e => e.positionCode == app.positionCode);
                        vRSPSalarySchedCasual sched = db.vRSPSalarySchedCasuals.Single(e => e.salaryGrade == app.salaryGrade);
                        tRSPZPersonnelService p = new tRSPZPersonnelService();
                        personnelService = GetPositionCasualPS(sched, position, Convert.ToInt16(postData.hazardCode), df, dt, postData);
                    }
                    else if (app.employmentStatusCode == "06") // J.O.
                    {
                        vRSPSalarySchedJO sched = db.vRSPSalarySchedJOes.Single(e => e.salaryGrade == app.salaryGrade);
                        personnelService = GetPositionJobOrderPS(sched, df, dt);
                    }
                    else if (app.employmentStatusCode == "07" || app.employmentStatusCode == "08") // C.O.S. & HON.
                    {
                        tRSPPositionContract posContract = db.tRSPPositionContracts.SingleOrDefault(e => e.positionCode == app.positionCode);
                        personnelService = GetPositionContractHon(posContract, df, dt);
                    }

                    if (psItem.total != personnelService.total)
                    {
                        tRSPZPSChecker c = new tRSPZPSChecker();
                        c.appointmentItemCode = item.appointmentItemCode;
                        c.currentPS = psItem.total;
                        c.counterPS = personnelService.total;
                        db.tRSPZPSCheckers.Add(c);
                    }
                }

                //COMIT;
                db.SaveChanges();


                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        //USE In PERSONNEL SERVICES OF RSPUtility
        public tRSPZPersonnelService GetCasualPS(decimal rateMonth, int salaryGrade, int hazard, DateTime periodFrom, DateTime periodTo, string empStatusCode)
        {
            tRSPZPersonnelService res = new tRSPZPersonnelService();

            int monthCount = Monthcounter(periodFrom, periodTo);
            decimal dailyRate = Convert.ToDecimal(rateMonth / 22);
            decimal monthlyRate = Convert.ToDecimal(rateMonth);
            decimal annualSalary = monthlyRate * monthCount;
            decimal PERA = 2000 * monthCount;
            decimal earnedLeave = (monthlyRate / 8) * monthCount;


            decimal hazardPay = 0;
            decimal subsistence = 0;
            decimal laundry = 0;
            if (hazard == 1)
            {
                //HEALTH SERVICES
                hazardPay = CalQHazardForHealthMonthly(Convert.ToInt16(salaryGrade), Convert.ToDecimal(dailyRate)) * monthCount;
                //SUBS. & Luandry
                subsistence = 1500 * monthCount;
                laundry = 150 * monthCount;
            }
            else if (hazard == 2)
            {
                //SOCIAL WORKER
                hazardPay = CalQHazardForSocialWorkerMonthly(dailyRate) * monthCount;
                //SUBS ONLY
                subsistence = 1500 * monthCount;
            }


            decimal midYearBonus = 0;
            DateTime checkDateFrom = periodFrom;
            DateTime cuttOffFrom = Convert.ToDateTime("January 15, " + periodTo.Year.ToString());
            //
            if (checkDateFrom == null || checkDateFrom.Date.Year == 1)
            {
                if (DateTime.Now.Year == periodTo.Year)
                {
                    checkDateFrom = DateTime.Now;
                }
                else
                {
                    checkDateFrom = Convert.ToDateTime("January 11, " + periodTo.Year.ToString());
                }
            }
            if (checkDateFrom <= cuttOffFrom)
            {
                midYearBonus = monthlyRate;
            }

            //!IMPORTANT => MIDYEAR BONUS


            decimal yearEndBonus = 0;
            decimal cashGift = 0;
            DateTime cuttOffTo = Convert.ToDateTime("October 31, " + periodTo.Year.ToString());
            if (checkDateFrom <= cuttOffTo)
            {
                yearEndBonus = monthlyRate;
                cashGift = 5000;
            }



            decimal lifeAndRetire = (Convert.ToDecimal(monthlyRate) * Convert.ToDecimal(.12)) * monthCount;
            decimal PHIC = CalQPHICGovtShare(Convert.ToDouble(monthlyRate), monthCount);

            decimal HDMF = 100 * monthCount;
            decimal ECC = 100 * monthCount;

            decimal clothing = 0;

            //MARCH 30 but if HEALTH PERSONNEL NO CUT-OFF
            if (hazard == 1)
            {
                clothing = 6000;
            }
            else
            {
                DateTime tmpCutOffClothing = Convert.ToDateTime("MARCH 30, " + periodTo.Year.ToString());
                if (checkDateFrom <= tmpCutOffClothing)
                {
                    clothing = 6000;
                }
            }


            if (empStatusCode == "03")
            {
                earnedLeave = 0;
            }

            decimal totalPS = annualSalary + PERA + earnedLeave + hazardPay + subsistence + laundry + midYearBonus + yearEndBonus + cashGift + lifeAndRetire + ECC + PHIC + HDMF + clothing;

            res.dailyRate = dailyRate;
            res.monthlyRate = monthlyRate;
            res.annualRate = annualSalary;
            res.PERA = PERA;
            res.leaveEarned = earnedLeave;
            res.hazardPay = hazardPay;
            res.subsistence = subsistence;
            res.laundry = laundry;
            res.midYearBonus = midYearBonus;
            res.yearEndBonus = yearEndBonus;
            res.cashGift = cashGift;
            res.lifeRetmnt = lifeAndRetire;
            res.ECC = ECC;
            res.hdmf = HDMF;
            res.phic = PHIC;
            res.clothing = clothing;
            res.total = totalPS;
            res.periodFrom = Convert.ToDateTime(periodFrom);
            res.periodTo = periodTo;
            res.recNo = monthCount;
            return res;

        }



        //RSP UTILITY
        public tRSPZPersonnelService GetJobOrderPS(decimal rateMonth, DateTime periodFrom, DateTime periodTo)
        {
            tRSPZPersonnelService ps = new tRSPZPersonnelService();
            try
            {
                int monthCount = Monthcounter(periodFrom, periodTo);
                decimal rate = Convert.ToDecimal(rateMonth);
                decimal monthlyRate = Convert.ToDecimal(rateMonth);
                decimal annualSalary = monthlyRate * monthCount;
                decimal totalPS = annualSalary;
                ps.appointmentItemCode = "";
                ps.positionTitle = "";
                ps.dailyRate = rateMonth / 22;
                ps.monthlyRate = rateMonth;
                ps.annualRate = annualSalary;
                ps.PERA = 0;
                ps.leaveEarned = 0;
                ps.midYearBonus = 0;
                ps.yearEndBonus = 0;
                ps.cashGift = 0;
                ps.hazardPay = 0;
                ps.laundry = 0;
                ps.subsistence = 0;
                ps.lifeRetmnt = 0;
                ps.ECC = 0;
                ps.hdmf = 0;
                ps.phic = 0;
                ps.clothing = 0;
                ps.total = totalPS;
                ps.periodFrom = periodFrom;
                ps.periodTo = periodTo;
                ps.PSTag = 0;
                ps.recNo = monthCount;
                return ps;
            }
            catch (Exception)
            {
                return ps;
            }
        }



        //RSP UTILITY
        public tRSPZPersonnelService GetJobContract(decimal rateMonth, DateTime periodFrom, DateTime periodTo)
        {
            tRSPZPersonnelService ps = new tRSPZPersonnelService();
            try
            {
                int monthCount = Monthcounter(periodFrom, periodTo);
                decimal rate = Convert.ToDecimal(rateMonth);
                decimal monthlyRate = Convert.ToDecimal(rateMonth);
                decimal annualSalary = monthlyRate * monthCount;
                decimal totalPS = annualSalary;
                ps.appointmentItemCode = "";
                ps.positionTitle = "";
                ps.dailyRate = rateMonth / 22;
                ps.monthlyRate = rateMonth;
                ps.annualRate = annualSalary;
                ps.PERA = 0;
                ps.leaveEarned = 0;
                ps.midYearBonus = 0;
                ps.yearEndBonus = 0;
                ps.cashGift = 0;
                ps.hazardPay = 0;
                ps.laundry = 0;
                ps.subsistence = 0;
                ps.lifeRetmnt = 0;
                ps.ECC = 0;
                ps.hdmf = 0;
                ps.phic = 0;
                ps.clothing = 0;
                ps.total = totalPS;
                ps.periodFrom = periodFrom;
                ps.periodTo = periodTo;
                ps.PSTag = 0;
                ps.recNo = monthCount;
                return ps;
            }
            catch (Exception)
            {
                return ps;
            }
        }



        // PS TABLE 2022 ###############################################################################################################
        // 1840
        public ActionResult Project()
        {
            return View();
        }

        public JsonResult ProjectByYear()
        {
            try
            {

                int budgetYear = 2022;
                var projects = from p in db.vRSPRefFundSources
                               where p.CY == budgetYear
                               select new { p.fundSourceCode, p.projectName, p.amount };

                var programs = from p in db.vRSPRefPrograms
                               where p.isActive == 1
                               select new { p.programCode, p.programName };

                return Json(new { status = "success", programs = programs, projects = projects }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }

        }






    }
}