using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DDNHRIS.Models;
using System.Globalization;
using System.Threading;

namespace DDNHRIS.Controllers
{

    [SessionTimeout]
    [Authorize(Roles = "APRD,PBOStaff")]
    public class RSPUtilityController : Controller
    {
        HRISDBEntities db = new HRISDBEntities();

        // POSITION TABLE //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        [Authorize(Roles = "APRD")]
        public ActionResult NOSI()
        {
            return View();
        }

        public class PostedNOSI
        {
            public string monthYear { get; set; }
            public int count { get; set; }
            public List<tRSPNOSI> list { get; set; }
        }

        [Authorize(Roles = "APRD")]
        public JsonResult NOSIInitData()
        {
            try
            {
                IEnumerable<tRSPNOSI> nosiList = db.tRSPNOSIs.OrderByDescending(o => o.effectiveDate).ToList();
                IEnumerable<tRSPNOSI> nosiPending = nosiList.Where(e => e.nosiTag == null).ToList();
                IEnumerable<tRSPNOSI> nosiPrinted = nosiList.Where(e => e.nosiTag >= 0).ToList();

                List<PostedNOSI> tempList = new List<PostedNOSI>();

                foreach (tRSPNOSI item in nosiPrinted)
                {
                    string tmp = Convert.ToDateTime(item.effectiveDate).ToString("MMMM yyyy");
                    tempList.Add(new PostedNOSI()
                    {
                        monthYear = tmp
                    });
                }
                //GROUP BY
                var myGroup = tempList.GroupBy(e => e.monthYear).Select(g => new { g.Key });

                List<PostedNOSI> postedList = new List<PostedNOSI>();
                foreach (var item in myGroup)
                {
                    DateTime tmpDate = Convert.ToDateTime("1 " + item.Key);
                    List<tRSPNOSI> myList = nosiPrinted.Where(e => e.effectiveDate.Value.Year == tmpDate.Year && e.effectiveDate.Value.Month == tmpDate.Month).OrderBy(o => o.effectiveDate).ToList();

                    if (myList.Count > 0)
                    {
                        postedList.Add(new PostedNOSI()
                        {
                            monthYear = item.Key.ToString(),
                            count = myList.Count(),
                            list = myList
                        });
                    }
                }

                PostedNOSI first = postedList.FirstOrDefault();
                nosiPending = nosiPending.OrderBy(o => o.effectiveDate).ToList();
                return Json(new { status = "success", pending = nosiPending, posted = postedList, nosiItem = first }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        public class NOSITemp
        {
            public decimal newSalary { get; set; }
            public decimal salaryAdd { get; set; }
        }

        [Authorize(Roles = "APRD")]
        public JsonResult NOSIItemData(string transCode)
        {
            try
            {
                tRSPNOSI item = db.tRSPNOSIs.Single(e => e.transCode == transCode);
                vRSPSalaryDetail newSalary = db.vRSPSalaryDetails.Single(e => e.salaryGrade == item.salaryGrade && e.step == item.stepIncTo);
                NOSITemp temp = new NOSITemp();
                temp.newSalary = Convert.ToDecimal(newSalary.rateMonth);
                temp.salaryAdd = Convert.ToDecimal(newSalary.rateMonth) - Convert.ToDecimal(item.salaryFrom);
                return Json(new { status = "success", nosiData = temp }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "APRD")]
        public JsonResult PrintNOSI(string id)
        {
            try
            {
                tRSPNOSI n = db.tRSPNOSIs.SingleOrDefault(e => e.transCode == id);
                if (n != null)
                {
                    Session["ReportType"] = "NOSI";
                    Session["PrintReport"] = id;

                    if (n.printedDate == null)
                    {
                        vRSPSalaryDetail newSalary = db.vRSPSalaryDetails.Single(e => e.salaryGrade == n.salaryGrade && e.step == n.stepIncTo);
                        n.salaryTo = Convert.ToDecimal(newSalary.rateMonth);
                        n.salaryAdd = Convert.ToDecimal(newSalary.rateMonth) - Convert.ToDecimal(n.salaryFrom);

                        tRSPPlantilla plan = db.tRSPPlantillas.Single(e => e.plantillaCode == n.plantillaCode);

                        //n.itemNo = plan.oldItemNo;
                        n.salaryDetailCode = newSalary.salaryDetailCode;
                        n.itemNo = plan.newItemNo;
                        n.printedDate = DateTime.Now;

                        tRSPEmployee emp = db.tRSPEmployees.Single(e => e.EIC == n.EIC);
                        string prefixLastName = emp.namePrefix + " " + emp.lastName;

                        if (emp.extName != null || emp.extName != "")
                        {
                            prefixLastName = prefixLastName + " " + emp.extName;
                        }
                        n.lastNamePrefix = ToPascalCase(prefixLastName.Trim());

                        n.nosiTag = 1;
                        db.SaveChanges();
                    }
                    return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult CancelNOSI(string id)
        {
            try
            {

                tRSPNOSI n = db.tRSPNOSIs.SingleOrDefault(e => e.transCode == id && e.nosiTag == null);

                if (n != null)
                {

                    n.nosiTag = -1;
                    n.remarks = "Cancelled";

                    tRSPTransactionLog log = new tRSPTransactionLog();
                    log.logCode = n.transCode;
                    log.logType = "NOSI";
                    log.logDetails = "NOSI CANCELLED";
                    log.userEIC = Session["_EIC"].ToString();
                    log.logDT = DateTime.Now;
                    db.tRSPTransactionLogs.Add(log);
                    //COMIT
                    db.SaveChanges();

                    return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        private string ToPascalCase(string text)
        {


            if (text.Contains(" "))
            {
                text = text.Replace(" ", "?");
                text = CultureInfo.InvariantCulture.TextInfo
                .ToTitleCase(text.ToLowerInvariant())
                .Replace("-", "")
                .Replace("_", "");
                text = text.Replace("?", " ");
                return text;
            }

            return CultureInfo.InvariantCulture.TextInfo
                .ToTitleCase(text.ToLowerInvariant())
                .Replace("-", "")
                .Replace("_", "");



        }


        // POSITION TABLE //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        [Authorize(Roles = "APRD")]
        public ActionResult PositionTable()
        {
            return View();
        }


        [HttpPost]
        [Authorize(Roles = "APRD")]
        public JsonResult EditCSCPosition(string id)
        {
            try
            {
                tRSPPosition position = db.tRSPPositions.Single(e => e.positionCode == id);
                return Json(new { status = "success", positionData = position }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        public class TempPositionList
        {
            public string positionCode { get; set; }
            public string positionTitle { get; set; }
            public int salaryGrade { get; set; }
            public string rateMonth { get; set; }
            public string keyCSCLevel { get; set; }
        }

        [HttpPost]
        [Authorize(Roles = "APRD")]
        public JsonResult PositionInitData()
        {
            try
            {
                IEnumerable<tRSPPosition> csc = db.tRSPPositions.Where(e => e.isActive == 1).OrderBy(o => o.positionTitle).ToList();
                IEnumerable<vRSPSalarySchedCasual> salary = db.vRSPSalarySchedCasuals.Where(e => e.step == 1).OrderBy(o => o.salaryGrade).ToList();

                List<TempPositionList> myList = new List<TempPositionList>();

                foreach (tRSPPosition item in csc)
                {
                    decimal tempSalary = Convert.ToDecimal(salary.Single(e => e.salaryGrade == item.salaryGrade).rateMonth);
                    myList.Add(new TempPositionList()
                    {
                        positionCode = item.positionCode,
                        positionTitle = item.positionTitle,
                        salaryGrade = Convert.ToInt16(item.salaryGrade),
                        rateMonth = tempSalary.ToString("#,##0.00"),
                        keyCSCLevel = item.keyCSCLevel
                    });
                }


                IEnumerable<tRSPPositionContract> con = db.tRSPPositionContracts.Where(e => e.isActive == 1).OrderBy(o => o.positionTitle).ToList();
                return Json(new { status = "success", positionCSC = myList, positionCOS = con }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string tmp = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        public class PositionTempData
        {
            public string positionCode { get; set; }
            public string positionDesc { get; set; }
            public string positionTitle { get; set; }
            public double salary { get; set; }
        }

        [HttpPost]
        [Authorize(Roles = "APRD")]
        public JsonResult SavePositionContract(PositionTempData data)
        {
            try
            {
                string tmpCode = "POSC" + DateTime.Now.ToString("yyMMddHHmmssfff");
                tRSPPositionContract c = new tRSPPositionContract();
                c.positionCode = tmpCode;
                c.positionTitle = data.positionTitle;
                c.positionDesc = data.positionDesc;
                c.salary = Convert.ToDecimal(data.salary);
                c.salaryTypeCode = "M";
                c.isActive = 1;
                db.tRSPPositionContracts.Add(c);
                db.SaveChanges();
                IEnumerable<tRSPPositionContract> con = db.tRSPPositionContracts.Where(e => e.isActive == 1).OrderBy(o => o.positionTitle).ToList();
                return Json(new { status = "success", positionCOS = con }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Authorize(Roles = "APRD")]
        public JsonResult UpdateCSCPosition(string id, string key)
        {
            try
            {
                tRSPPosition pos = db.tRSPPositions.Single(e => e.positionCode == id);
                pos.keyCSCLevel = key;
                db.SaveChanges();

                IEnumerable<tRSPPosition> csc = db.tRSPPositions.Where(e => e.isActive == 1).OrderBy(o => o.positionTitle).ToList();
                IEnumerable<vRSPSalarySchedCasual> salary = db.vRSPSalarySchedCasuals.Where(e => e.step == 1).OrderBy(o => o.salaryGrade).ToList();

                List<TempPositionList> myList = new List<TempPositionList>();

                foreach (tRSPPosition item in csc)
                {
                    decimal tempSalary = Convert.ToDecimal(salary.Single(e => e.salaryGrade == item.salaryGrade).rateMonth);
                    myList.Add(new TempPositionList()
                    {
                        positionCode = item.positionCode,
                        positionTitle = item.positionTitle,
                        salaryGrade = Convert.ToInt16(item.salaryGrade),
                        rateMonth = tempSalary.ToString("#,##0.00"),
                        keyCSCLevel = item.keyCSCLevel
                    });
                }

                return Json(new { status = "success", positionCSC = myList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //NOTICE OF SALARY ADJUSTMENT (NOSA)
        [Authorize(Roles = "APRD")]
        public ActionResult NOSA()
        {
            return View();
        }


        //NOSA INITAIL DATA
        [HttpPost]
        [Authorize(Roles = "APRD")]
        public JsonResult NOSAInitData()
        {
            try
            {
                List<TempNOSA> myList = GetNOSAList();
                IEnumerable<tOrgDepartment> dept = db.tOrgDepartments.Where(e => e.isActive == true).OrderBy(o => o.orderNo).ToList();
                List<TempOfficeList> deptList = new List<TempOfficeList>();
                foreach (tOrgDepartment d in dept)
                {
                    int counter = myList.Where(e => e.shortDepartmentName == d.shortDepartmentName && e.tag == null).Count();
                    deptList.Add(new TempOfficeList()
                    {
                        officeName = d.shortDepartmentName,
                        count = Convert.ToInt16(counter)
                    });
                }
                int pending = myList.Where(e => e.tag == null).Count();
                return Json(new { status = "success", list = myList, pendingCount = pending, deptList = deptList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "Error loading data...!" }, JsonRequestBehavior.AllowGet);
            }
        }



        public class TempNOSA
        {
            public string EIC { get; set; }
            public string NOSACode { get; set; }
            public string appointmentItemCode { get; set; }
            public string itemNo { get; set; }
            public string fullNameLast { get; set; }
            public string prefixLastName { get; set; }
            public string positionTitle { get; set; }
            public int salaryGrade { get; set; }
            public int step { get; set; }
            public string salaryDetailCode { get; set; }
            public string fullNameTitle { get; set; }
            public decimal salaryFrom { get; set; }
            public decimal salaryTo { get; set; }
            public decimal salaryAdd { get; set; }
            public string shortDepartmentName { get; set; }
            public int? tag { get; set; }

        }


        //NOSA INITAIL DATA
        private class TempOfficeList
        {
            public string departmentCode { get; set; }
            public string officeName { get; set; }
            public string departmentNameShort { get; set; }
            public Int16 count { get; set; }
        }

        private List<TempNOSA> GetNOSAList()
        {
            List<TempNOSA> myList = new List<TempNOSA>();

            //NOSA2021SSL0201P
            DateTime dt = Convert.ToDateTime("2021-01-01");
            IEnumerable<vRSPNOSA> nosa = db.vRSPNOSAs.Where(e => e.effectiveDate.Value == dt && e.itemNo > 0).OrderBy(o => o.itemNo).ToList();

            foreach (vRSPNOSA item in nosa)
            {
                string nameTitle = item.fullNameTitle;
                if (item.tag == null)
                {
                    nameTitle = item.namePrefix + " " + item.fullNameFirst.ToUpper() + " " + item.nameSuffix;
                    nameTitle = nameTitle.Trim();
                }

                if (item.fullNameTitle == null || item.fullNameTitle == "" || item.fullNameTitle.Length < 5)
                {
                    nameTitle = item.namePrefix + " " + item.fullNameFirst.ToUpper() + " " + item.nameSuffix;
                    nameTitle = nameTitle.Trim();
                }
                string tmp = item.namePrefix == null ? "" : item.namePrefix;
                string prefixLastName = ToPascalCase(tmp) + " " + ToPascalCase(item.lastName);

                myList.Add(new TempNOSA()
                {
                    EIC = item.EIC,
                    NOSACode = item.NOSACode,
                    itemNo = item.itemNo.ToString(),
                    fullNameLast = item.fullNameLast,
                    prefixLastName = prefixLastName,
                    positionTitle = item.positionTitle,
                    salaryGrade = Convert.ToInt16(item.salaryGrade),
                    step = Convert.ToInt16(item.step),
                    salaryDetailCode = item.salaryDetailCode,
                    fullNameTitle = nameTitle,
                    salaryFrom = Convert.ToDecimal(item.salaryFrom),
                    salaryTo = Convert.ToDecimal(item.salaryTo),
                    salaryAdd = Convert.ToDecimal(item.salaryAdd),
                    shortDepartmentName = item.shortDepartmentName,
                    tag = item.tag
                });
            }

            myList = myList.OrderBy(e => e.tag).ThenBy(o => o.fullNameLast).ToList();
            return myList;

        }


        //NOSA PRINT PLANTILLA
        [HttpPost]
        [Authorize(Roles = "APRD")]
        public JsonResult NOSAPrint(string id, string code, string nameTitle, string prefixLastName)
        {
            try
            {
                tRSPNOSA n = db.tRSPNOSAs.SingleOrDefault(e => e.EIC == id && e.NOSACode == code);
                if (n == null)
                {
                    return Json(new { status = "Hacking Attempt...!" }, JsonRequestBehavior.AllowGet);   //HACKER ALERT
                }
                n.prefixLastName = prefixLastName;
                n.fullNameFirst = nameTitle;
                n.tag = 0;
                n.userEIC = Session["_EIC"].ToString();
                n.printDT = DateTime.Now;
                db.SaveChanges();
                Session["ReportType"] = "NOSA";
                Session["PrintReport"] = n.recNo;

                List<TempNOSA> myList = GetNOSAList();

                IEnumerable<tOrgDepartment> dept = db.tOrgDepartments.Where(e => e.isActive == true).OrderBy(o => o.orderNo).ToList();
                List<TempOfficeList> deptList = new List<TempOfficeList>();
                foreach (tOrgDepartment d in dept)
                {
                    int counter = myList.Where(e => e.shortDepartmentName == d.shortDepartmentName && e.tag == null).Count();
                    deptList.Add(new TempOfficeList()
                    {
                        officeName = d.shortDepartmentName,
                        count = Convert.ToInt16(counter)
                    });
                }

                int pending = myList.Where(e => e.tag == null).Count();
                return Json(new { status = "success", list = myList, pendingCount = pending, deptList = deptList }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { status = "Error loading data...!" }, JsonRequestBehavior.AllowGet);
            }
        }



        //POST NOSA TO DB (PERMANENT)
        public JsonResult PostNosaToDB(string id, string code)
        {
            try
            {
                vRSPNOSA nosaData = db.vRSPNOSAs.SingleOrDefault(e => e.NOSACode == code && e.EIC == id);
                if (nosaData == null)
                {   //HACKER ALERT
                    return Json(new { status = "Invalid data!" }, JsonRequestBehavior.AllowGet);
                }

                if (nosaData.tag == 1)
                {
                    return Json(new { status = "Unable to post!" }, JsonRequestBehavior.AllowGet);
                }


                vRSPPlantillaPersonnel plantilla = db.vRSPPlantillaPersonnels.Single(e => e.EIC == nosaData.EIC);
                if (plantilla == null)
                {   //HACKER ALERT
                    return Json(new { status = "Invalid plantilla data!" }, JsonRequestBehavior.AllowGet);
                }



                tRSPNOSA nosaItem = db.tRSPNOSAs.Single(e => e.NOSACode == code && e.EIC == id);
                nosaItem.tag = 1;

                //
                tRSPPlantillaPersonnel p = new tRSPPlantillaPersonnel();
                //NOSA
                string tempCode = "NOSA" + DateTime.Now.ToString("yyMMddHHmmssfff") + nosaData.EIC.Substring(0, 5) + "P"; //25

                p.transCode = tempCode;
                p.appointmenCode = plantilla.appointmenCode;
                p.plantillaCode = plantilla.plantillaCode;
                p.EIC = nosaData.EIC;
                p.salaryDetailCode = nosaData.salaryDetailCode;
                p.effectiveDate = nosaData.effectiveDate;
                db.tRSPPlantillaPersonnels.Add(p);

                tRSPEmployeePosition pos = new tRSPEmployeePosition();
                pos.transCode = tempCode;
                pos.transType = "NOSA";
                pos.EIC = nosaData.EIC;
                pos.plantillaCode = plantilla.plantillaCode;
                pos.positionCode = plantilla.positionCode;
                pos.subPositionCode = plantilla.subPositionCode;
                pos.salaryGrade = nosaData.salaryGrade;
                pos.step = nosaData.step;
                pos.salaryDetailCode = nosaData.salaryDetailCode;
                pos.salaryRate = nosaData.salaryTo;
                pos.salaryType = "M";
                pos.employmentStatusCode = plantilla.employmentStatusCode;
                pos.effectiveDate = nosaData.effectiveDate;
                pos.appointmentCode = plantilla.appointmenCode;
                db.tRSPEmployeePositions.Add(pos);

                //comit
                db.SaveChanges();

                //Thread.Sleep(5000);

                //List<TempNOSA> myList = GetNOSAList();  
                List<TempNOSA> myList = new List<TempNOSA>();
                return Json(new { status = "success", list = myList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //NOTICE OF SALARY ADJUSTMENT (NOSA) - CASUAL
        [Authorize(Roles = "APRD")]
        [HttpPost]
        public JsonResult NOSACasualData()
        {
            try
            {
                string id = "";
                string uEIC = Session["_EIC"].ToString();
                IEnumerable<tRSPNOSA> lastFS = db.tRSPNOSAs.Where(e => e.userEIC == uEIC).OrderByDescending(o => o.printDT);
                string code = lastFS.FirstOrDefault().appointmentItemCode;

                if (code != null)
                {
                    string appItem = code;
                    id = db.vRSPAppointmentNPEmployees.SingleOrDefault(e => e.appointmentItemCode == appItem).fundSourceCode;
                }

                tRSPRefFundSource fundSourceData = db.tRSPRefFundSources.SingleOrDefault(e => e.fundSourceCode == id);

                List<TempNOSA> list = GetNOSACasual(id);
                return Json(new { status = "success", casualList = list, fundSourceData = fundSourceData }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        private List<TempNOSA> GetNOSACasual(string id)
        {

            string fundSourceCode = id;

            List<TempNOSA> myList = new List<TempNOSA>();

            //CHECK DATE OF APPOINMENT
            //FEB 1, 2021 IS JUST SAPMPLE

            string prevSked = "MSSLT12020";
            string currSked = "MSSLT12021";
            string nosaCode = "NOSA2021SLT0201C";

            //PREVIOUS
            IEnumerable<tRSPSalaryTableDetail> schedPrev = db.tRSPSalaryTableDetails.Where(e => e.salaryCode == prevSked && e.salaryGrade <= 32 && e.step == 1).OrderBy(o => o.salaryGrade).ToList();
            //CURRENT
            IEnumerable<tRSPSalaryTableDetail> schedNew = db.tRSPSalaryTableDetails.Where(e => e.salaryCode == currSked && e.salaryGrade <= 32 && e.step == 1).OrderBy(o => o.salaryGrade).ToList();
            //
            IEnumerable<vRSPAppointmentNPEmployee> casualList = db.vRSPAppointmentNPEmployees.Where(e => e.employmentStatusCode == "05" && e.fundSourceCode == fundSourceCode && e.empTag != -1).OrderBy(o => o.fullNameLast).ToList();

            IEnumerable<tRSPNOSA> nosaList = db.tRSPNOSAs.Where(e => e.NOSACode == nosaCode).ToList();

            foreach (vRSPAppointmentNPEmployee item in casualList)
            {
                decimal prevRate = Convert.ToDecimal(schedPrev.Single(e => e.salaryGrade == item.salaryGrade).rateDay);
                decimal newRate = Convert.ToDecimal(schedNew.Single(e => e.salaryGrade == item.salaryGrade).rateDay);

                tRSPNOSA ns = nosaList.LastOrDefault(e => e.EIC == item.EIC);

                dynamic t = null;
                if (ns != null)
                {
                    t = 0;
                }

                myList.Add(new TempNOSA()
                {
                    EIC = item.EIC,
                    appointmentItemCode = item.appointmentItemCode,
                    NOSACode = nosaCode,
                    itemNo = "",
                    fullNameLast = item.fullNameLast,
                    fullNameTitle = item.fullNameTitle,
                    prefixLastName = "",
                    positionTitle = item.positionTitle,
                    salaryGrade = Convert.ToInt16(item.salaryGrade),
                    step = 1,
                    salaryDetailCode = item.salaryDetailCode,
                    salaryFrom = prevRate,
                    salaryTo = newRate,
                    salaryAdd = newRate - prevRate,
                    shortDepartmentName = item.shortDepartmentName,
                    tag = t
                });

            }

            return myList;



        }

        [HttpPost]
        public JsonResult GetFundSourceList()
        {
            try
            {
                IEnumerable<tRSPRefFundSource> fs = db.tRSPRefFundSources.Where(e => e.isActive == 1).ToList();
                return Json(new { status = "success", fundSourceList = fs }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        //FUND SOURCE CASUAL
        [HttpPost]
        public JsonResult GetFundSourceCasual(string id)
        {
            try
            {
                vRSPRefFundSource fs = db.vRSPRefFundSources.Single(e => e.fundSourceCode == id);
                List<TempNOSA> list = GetNOSACasual(id);
                return Json(new { status = "success", casualList = list, fundSourceData = fs }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        //PRINT NOSA CASUAL
        [HttpPost]
        public JsonResult NOSACasualPrint(TempNOSA data)
        {
            try
            {
                string nosaCode = "NOSA2021SLT0201C";

                string prevSked = "MSSLT12020";
                string currSked = "MSSLT12021";

                vRSPAppointmentNPEmployee app = db.vRSPAppointmentNPEmployees.SingleOrDefault(e => e.EIC == data.EIC && e.appointmentItemCode == data.appointmentItemCode);

                if (app == null)
                {
                    return Json(new { status = "Hacking Alert...!" }, JsonRequestBehavior.AllowGet);   //HACKER ALERT
                }

                int sg = Convert.ToInt16(app.salaryGrade);

                //PREVIOUS
                tRSPSalaryTableDetail schedPrev = db.tRSPSalaryTableDetails.Single(e => e.salaryCode == prevSked && e.salaryGrade == sg && e.step == 1);
                //CURRENT
                tRSPSalaryTableDetail schedNew = db.tRSPSalaryTableDetails.Single(e => e.salaryCode == currSked && e.salaryGrade == sg && e.step == 1);
                decimal salaryAdd = Convert.ToDecimal(schedNew.rateDay) - Convert.ToDecimal(schedPrev.rateDay);

                tRSPNOSA n = db.tRSPNOSAs.SingleOrDefault(e => e.EIC == app.EIC && e.appointmentItemCode == app.appointmentItemCode);
                tRSPEmployee emp = db.tRSPEmployees.Single(e => e.EIC == data.EIC);

                string tmp = emp.namePrefix == null ? "" : emp.namePrefix;
                string prefixLastName = ToPascalCase(tmp) + " " + ToPascalCase(emp.lastName);
                string tempCode = "NOSAC" + DateTime.Now.ToString("yyMMddHHmmssffff") + emp.EIC.Substring(0, 5);

                if (n == null)
                {
                    //CREATE NEW 
                    tRSPNOSA a = new tRSPNOSA();
                    a.NOSAId = tempCode;
                    a.NOSACode = nosaCode;
                    a.EIC = app.EIC;
                    a.fullNameFirst = data.fullNameTitle;
                    a.prefixLastName = prefixLastName;
                    a.positionTitle = app.positionTitle;
                    a.salaryGrade = app.salaryGrade;
                    a.step = 1;
                    a.salaryFrom = Convert.ToDecimal(schedPrev.rateDay);
                    a.salaryTo = Convert.ToDecimal(schedNew.rateDay);
                    a.salaryAdd = salaryAdd;
                    a.salaryDetailCode = schedNew.salaryDetailCode;
                    a.appointmentItemCode = app.appointmentItemCode;
                    a.tag = 0;
                    a.departmentCode = app.departmentCode;
                    a.printDT = DateTime.Now;
                    a.userEIC = Session["_EIC"].ToString();
                    db.tRSPNOSAs.Add(a);
                    db.SaveChanges();
                }
                else
                {
                    tempCode = n.NOSAId;
                    n.prefixLastName = prefixLastName;
                    n.fullNameFirst = data.fullNameTitle;
                    db.SaveChanges();
                }

                Session["ReportType"] = "NOSACa";
                Session["PrintReport"] = tempCode;

                List<TempNOSA> list = GetNOSACasual(app.fundSourceCode);
                return Json(new { status = "success", casualList = list }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { status = "Error loading data...!" }, JsonRequestBehavior.AllowGet);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        public ActionResult VacantPositions()
        {

            return View();
        }


        public class TempVacantPosition
        {
            public string reportCode { get; set; }
            public string oldItemNo { get; set; }
            public string itemNo { get; set; }
            public string plantillaCode { get; set; }
            public string positionTitle { get; set; }
            public string subPositionTitle { get; set; }
            public int separationTag { get; set; }
            public int salaryGrade { get; set; }
            public int level { get; set; }
            public decimal rateMonth { get; set; }
            public string shortDepartmentName { get; set; }
            public string functionCode { get; set; }
            public string vacatedBy { get; set; }
            public string vacatedStat { get; set; }

        }

        public class TempVacantPositionSumm
        {
            public string reportCode { get; set; }
            public string functionCode { get; set; }
            public string functionName { get; set; }
            public int levelFirst { get; set; }
            public int levelSecond { get; set; }
            public int total { get; set; }
            public DateTime reportDT { get; set; }
        }

        public class TempReportGroup
        {
            public string reportCode { get; set; }
            public string reportDate { get; set; }
        }

        // ********************** VACANT BY MONTH *******************************************//
        public JsonResult VacantByMonthList()
        {
            try
            {
                DateTime dt = Convert.ToDateTime("January 1, " + DateTime.Now.Year);
                //PLANTILLA
                IEnumerable<vRSPPlantilla> list = db.vRSPPlantillas.Where(e => e.EIC == null && e.isFunded == true).OrderBy(o => o.itemNo).ToList();
                //SEPARATION
                IEnumerable<tRSPSeparation> sep = db.tRSPSeparations.Where(e => e.effectiveDate.Year == dt.Year && e.effectiveDate.Month == dt.Month).ToList();

                List<TempVacantPosition> myList = new List<TempVacantPosition>();

                foreach (vRSPPlantilla item in list)
                {
                    myList.Add(new TempVacantPosition()
                    {
                        oldItemNo = Convert.ToInt16(item.oldItemNo).ToString("0000"),
                        itemNo = Convert.ToInt16(item.itemNo).ToString("0000"),
                        plantillaCode = item.plantillaCode,
                        positionTitle = item.positionTitle,
                        subPositionTitle = item.subPositionTitle,
                        salaryGrade = Convert.ToInt16(item.salaryGrade),
                        level = Convert.ToInt16(item.positionLevel),
                        rateMonth = Convert.ToDecimal(item.rateMonth),
                        shortDepartmentName = item.shortDepartmentName,
                        functionCode = item.functionCode
                    });

                }

                //tOrgFunction
                IEnumerable<tOrgFunction> funcList = db.tOrgFunctions.Where(e => e.isActive == true).OrderBy(o => o.orderNo).ToList();
                List<TempVacantPositionSumm> summList = new List<TempVacantPositionSumm>();
                foreach (tOrgFunction item in funcList)
                {
                    int count1 = myList.Where(e => e.functionCode == item.functionCode && e.level == 1).Count();
                    int count2 = myList.Where(e => e.functionCode == item.functionCode && e.level == 2).Count();
                    summList.Add(new TempVacantPositionSumm()
                    {
                        functionCode = item.functionCode,
                        functionName = item.functionDesc,
                        levelFirst = count1,
                        levelSecond = count2,
                        total = count1 + count2
                    });
                }

                List<TempReportGroup> rpt = new List<TempReportGroup>();

                IEnumerable<tRSPVacantPositionSum> groupList = db.tRSPVacantPositionSums.Where(e => e.recNo > 0).ToList();

                IEnumerable<tRSPVacantPositionSum> tempGroup = groupList.GroupBy(g => g.reportCode).Select(group => group.First()).ToList();

                foreach (var item in tempGroup)
                {
                    rpt.Add(new TempReportGroup()
                    {
                        reportCode = item.reportCode,
                        reportDate = Convert.ToDateTime(item.reportDT).ToString("MMMM dd, yyyy")
                    });
                }


                IEnumerable<tOrgFunction> deptFunc = db.tOrgFunctions.Where(e => e.isActive == true).OrderBy(o => o.orderNo).ToList();


                return Json(new { status = "success", list = myList, summData = summList, reportList = rpt, department = deptFunc }, JsonRequestBehavior.AllowGet);




            }
            catch (Exception)
            {
                return Json(new { status = "Error loading data...!" }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult GenerateVacantPositionSummary()
        {
            try
            {
                string tempCode = "SVP" + DateTime.Now.ToString("yyMMddHHmmssfff");
                DateTime dt = Convert.ToDateTime("January 1, 2021");
                //PLANTILLA
                IEnumerable<vRSPPlantilla> list = db.vRSPPlantillas.Where(e => e.EIC == null && e.isFunded == true).OrderBy(o => o.itemNo).ToList();
                //SEPARATION
                IEnumerable<vRSPSeparation> sep = db.vRSPSeparations.Where(e => e.separationDate.Year == dt.Year && e.separationDate.Month == dt.Month).ToList();

                List<TempVacantPosition> myList = new List<TempVacantPosition>();

                foreach (vRSPPlantilla item in list)
                {
                    int sepTag = sep.Where(e => e.plantillaCode == item.plantillaCode).Count();
                    if (sepTag > 1) { sepTag = 1; }

                    myList.Add(new TempVacantPosition()
                    {
                        reportCode = tempCode,
                        oldItemNo = Convert.ToInt16(item.oldItemNo).ToString("0000"),
                        itemNo = Convert.ToInt16(item.itemNo).ToString("0000"),
                        plantillaCode = item.plantillaCode,
                        positionTitle = item.positionTitle,
                        subPositionTitle = item.subPositionTitle,
                        salaryGrade = Convert.ToInt16(item.salaryGrade),
                        level = Convert.ToInt16(item.positionLevel),
                        rateMonth = Convert.ToDecimal(item.rateMonth),
                        separationTag = sepTag,
                        shortDepartmentName = item.shortDepartmentName,
                        functionCode = item.functionCode
                    });

                    tRSPVacantPositionData n = new tRSPVacantPositionData();
                    n.reportCode = tempCode;
                    n.plantillaCode = item.plantillaCode;
                    n.salaryDetailCode = item.salaryDetailCode;
                    n.rateMonth = item.rateMonth;
                    n.separationTag = sepTag;
                    db.tRSPVacantPositionDatas.Add(n);
                }

                //tOrgFunction
                IEnumerable<tOrgFunction> funcList = db.tOrgFunctions.Where(e => e.isActive == true).OrderBy(o => o.orderNo).ToList();
                List<TempVacantPositionSumm> summList = new List<TempVacantPositionSumm>();
                foreach (tOrgFunction item in funcList)
                {
                    int count1 = myList.Where(e => e.functionCode == item.functionCode && e.level == 1).Count();
                    int count2 = myList.Where(e => e.functionCode == item.functionCode && e.level == 2).Count();
                    summList.Add(new TempVacantPositionSumm()
                    {
                        reportCode = tempCode,
                        functionCode = item.functionCode,
                        functionName = item.functionDesc,
                        levelFirst = count1,
                        levelSecond = count2,
                        total = count1 + count2
                    });

                    tRSPVacantPositionSum s = new tRSPVacantPositionSum();
                    s.reportCode = tempCode;
                    s.functionCode = item.functionCode;
                    s.functionName = item.functionDesc;
                    s.levelFirst = count1;
                    s.levelSecond = count2;
                    s.total = count1 + count2;
                    s.reportDT = DateTime.Now;
                    db.tRSPVacantPositionSums.Add(s);
                }
                db.SaveChanges();
                return Json(new { status = "success", list = myList, summData = summList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "Error loading data...!" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetVacantByFunctionCode(string id)
        {
            try
            {
                IEnumerable<vRSPPlantilla> list = db.vRSPPlantillas.Where(e => e.EIC == null && e.isFunded == true && e.functionCode == id).OrderBy(o => o.itemNo).ToList();
                IEnumerable<vSalaryTable> salaryTable = db.vSalaryTables.ToList();

                //SEPARATION
                IEnumerable<vRSPSeparation> sep = db.vRSPSeparations.ToList();
                List<TempVacantPosition> myList = new List<TempVacantPosition>();

                foreach (vRSPPlantilla item in list)
                {
                    string vacatedBy = "";
                    string vacatedStat = "";
                    vRSPSeparation separationItem = sep.LastOrDefault(e => e.plantillaCode == item.plantillaCode);
                    if (separationItem != null)
                    {
                        vacatedBy = separationItem.fullNameFirst;
                        vacatedStat = separationItem.separationType;
                    }
                    decimal salary = (decimal)salaryTable.First(e => e.salaryGrade == item.salaryGrade).rateMonth;
                    myList.Add(new TempVacantPosition()
                    {
                        //reportCode = tempCode,
                        oldItemNo = Convert.ToInt16(item.oldItemNo).ToString("0000"),
                        itemNo = Convert.ToInt16(item.itemNo).ToString("0000"),
                        plantillaCode = item.plantillaCode,
                        positionTitle = item.positionTitle,
                        subPositionTitle = item.subPositionTitle,
                        salaryGrade = Convert.ToInt16(item.salaryGrade),
                        level = Convert.ToInt16(item.positionLevel),
                        rateMonth = salary,
                        //separationTag = sepTag,
                        shortDepartmentName = item.shortDepartmentName,
                        functionCode = item.functionCode,
                        vacatedBy = vacatedBy,
                        vacatedStat = vacatedStat
                    });
                }


                return Json(new { status = "success", vacantPositions = myList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string err = ex.ToString();
                return Json(new { status = "Error loading data...!" }, JsonRequestBehavior.AllowGet);
            }
        }


          [Authorize(Roles = "APRD,PBOStaff")]
        // PERSONAL SERVICES  (P.S.) ************************************************************

        public ActionResult PSRequirement()
        {

            return View();
        }

        public class TempCurrentPS
        {
            public string plantillaCode { get; set; }
            public string itemNo { get; set; }
            public string oldItemNo { get; set; }
            public string EIC { get; set; }
            public string fullNameLast { get; set; }
            public string fullNameFirst { get; set; }
            public string positionTitle { get; set; }
            public int salaryGrade { get; set; }
            public int step { get; set; }
            public decimal monthlyRate { get; set; }
            public decimal annualRate { get; set; }
            public decimal PERA { get; set; }
            public decimal RA { get; set; }
            public decimal TA { get; set; }

            public decimal clothing { get; set; }
            public decimal hazard { get; set; }
            public decimal subsistence { get; set; }
            public decimal laundry { get; set; }
            public decimal bonusMidYear { get; set; }
            public decimal bonusYearEnd { get; set; }
            public decimal cashGift { get; set; }
            public decimal loyalty { get; set; }
            public decimal gsisPrem { get; set; }
            public decimal hdmfPrem { get; set; }
            public decimal ECC { get; set; }
            public decimal PHIC { get; set; }
            public decimal totalPS { get; set; }
            public string stepRemarks { get; set; }
            public int plantillaNo { get; set; }
        }

         

        [Authorize(Roles = "APRD,PBOStaff")]
        [HttpPost]
        public JsonResult PrintPSSetup(string id, int sy)
        {
            //string code = "T20210101NOSA" + id.Substring(0, 12);             
            string code = "T" + sy.ToString("0000") + "P" + id;
            Session["ReportType"] = "PRINTPS";
            Session["PrintReport"] = code;
            return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
        }




        [Authorize(Roles = "APRD,PBOSTAFF")]
        [HttpPost]
        public JsonResult PSRequirementByDeptCode(string id, int sy)
        {
            try
            {
                //PLANTILLA
                //IEnumerable<vRSPPlantilla> plantilla = db.vRSPPlantillas.Where(e => e.departmentCode == id && e.isFunded == true).OrderBy(o => o.itemNo).ToList();
                IEnumerable<vRSPPlantilla> plantilla = db.vRSPPlantillas.Where(e => e.functionCode == id && (e.isFunded == true || e.fundStat ==2)).OrderBy(o => o.plantillaNo).ToList();
                //SALARY TABLE

                //2021 - MSSLT12021
                //2022 - ST2022SSL05T03
                
                IEnumerable<tRSPSalaryTableDetail> salaryTable = db.tRSPSalaryTableDetails.Where(e => e.salaryCode == "ST2022SSL05T03").ToList();
                if (sy == 2021)
                {
                    salaryTable = db.tRSPSalaryTableDetails.Where(e => e.salaryCode == "MSSLT12021").ToList();
                }


                //STEP INCREMENT

                /////////// STEP INCREMENT
                string code = "T" + sy.ToString("0000") + "P" + id;

                //CURRENT NOSI NOT YET POSTED
                IEnumerable<tRSPNOSIPropBudYear> NOSIPrev = db.tRSPNOSIPropBudYears.Where(e => e.year == 2021).ToList();

                IEnumerable<tRSPNOSIPropBudYear> NOSIList = db.tRSPNOSIPropBudYears.Where(e => e.year == 2022).ToList();
                List<TempCurrentPS> list = new List<TempCurrentPS>();
                
                //LOYALTY
                IEnumerable<tRSPLoyalty> loyaltyList = db.tRSPLoyalties.Where(e => e.year == 2022).ToList();

                //HAZARD
                IEnumerable<tPayrollHazard> hazardList = db.tPayrollHazards.ToList();

                //RA TA
                IEnumerable<tPayrollRATA> rataList = db.tPayrollRATAs.Where(e => e.status == 1).ToList();

                
                

                int count = db.tReportBudgetaryReqs.Where(e => e.reportCode == code).Count();
                int counter = 0;
                foreach (vRSPPlantilla item in plantilla)
                {

                  
                    int prevStep = Convert.ToInt16(item.step);

                    if (item.plantillaCode == "PC223847464")
                    {
                        string s = "";
                    }

                    if (item.EIC != null)
                    {
                        tRSPNOSIPropBudYear currNOSI = NOSIPrev.SingleOrDefault(e => e.EIC == item.EIC);
                        if (currNOSI != null)
                        {
                            if (item.step == currNOSI.step - 1)
                            {
                                prevStep = Convert.ToInt16(currNOSI.step);
                            }
                        }
                    }

                    
                    TempCurrentPS ps = _GetPSRequirement(item, salaryTable, NOSIList, hazardList , prevStep);
                    //CHECK FOR LOYALTY
                    tRSPLoyalty loyal = loyaltyList.FirstOrDefault(e => e.EIC == item.EIC);
                    if (loyal != null)
                    {
                        decimal myLoyal = Convert.ToDecimal(loyal.loyalty);
                        ps.loyalty = myLoyal;
                        ps.totalPS = Convert.ToDecimal(ps.totalPS + myLoyal);

                    }
                    //CHECK RA & TA

                    tRSPRata rata = getMyRATA(ps.plantillaCode);

                    //IEnumerable<tPayrollRATA> rata = rataList.Where(e => e.EIC == item.EIC);
                    if (rata != null)
                    {
                        //decimal ra = Convert.ToDecimal(rata.Where(e => e.payrollClassification == "RA").Sum(e => e.amount)) * 12;
                        //decimal ta = Convert.ToDecimal(rata.Where(e => e.payrollClassification == "TA").Sum(e => e.amount)) * 12;
                        decimal ra = Convert.ToDecimal(rata.RA * 12);
                        decimal ta = Convert.ToDecimal(rata.TA) * 12;
                        ps.RA = ra;
                        ps.TA = ta;
                        decimal newTotal = Convert.ToDecimal(ps.totalPS) + ra + ta;
                        ps.totalPS = newTotal;
                    }
                    ps.plantillaNo = Convert.ToInt16(item.plantillaNo);

                    list.Add(ps);

                    if (count == 0)
                    {
                        tReportBudgetaryReq r = new tReportBudgetaryReq();
                        r.plantillaCode = item.plantillaCode;
                        r.EIC = item.EIC;
                        r.itemNo = item.itemNo;
                        r.salaryGrade = ps.salaryGrade;
                        r.stepInc = ps.step;
                        r.stepRemark = ps.stepRemarks;
                        r.monthlySalary = ps.monthlyRate;
                        r.annualSalary = ps.annualRate;
                        r.PERA = ps.PERA;
                        r.RA = ps.RA;
                        r.TA = ps.TA;
                        r.clothing = ps.clothing;
                        r.hazard = ps.hazard;
                        r.subsistence = ps.subsistence;
                        r.laundry = ps.laundry;
                        r.midYearBonus = ps.bonusMidYear;
                        r.yearEndBonus = ps.bonusYearEnd;
                        r.cashGift = ps.cashGift;
                        r.loyaltyBonus = ps.loyalty;
                        r.lifeRetirement = ps.gsisPrem;
                        r.hmdfPrem = ps.hdmfPrem;
                        r.ECC = ps.ECC;
                        r.PHIC = ps.PHIC;
                        r.total = ps.totalPS;
                        r.departmentCode = item.departmentCode;
                        r.reportCode = code;
                        db.tReportBudgetaryReqs.Add(r);
                        counter = counter + 1;
                    }

                }

                if (counter >= 1)
                {
                    //COMIT;
                    db.SaveChanges();
                }


                //IEnumerable<tOrgFunction> funcList = db.tOrgFunctions.Where(e => e.isActive == true).OrderBy(o => o.orderNo).ToList();
                ////IEnumerable<tOrgDepartment> deptList = db.tOrgDepartments.Where(e => e.isActive == true).OrderBy(o => o.orderNo).ToList();
                //List<tOrgFunction> departmentList = new List<tOrgFunction>();
                //foreach (tOrgFunction p in funcList)
                //{
                //    departmentList.Add(new tOrgFunction()
                //    {
                //        functionCode = p.functionCode,
                //        departmentName = p.functionDesc
                //    });
                //}

                list = list.OrderBy(o => o.plantillaNo).ToList();

                return Json(new { status = "success", psList = list, code = code  }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "Error loading data...!" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "APRD,PBOStaff")]

        private TempCurrentPS _GetPSRequirement(vRSPPlantilla plantilla, IEnumerable<tRSPSalaryTableDetail> salaryTable, IEnumerable<tRSPNOSIPropBudYear> NOSIList, IEnumerable<tPayrollHazard> hazardList, int newStep)
        {
            TempCurrentPS p = new TempCurrentPS();
            int i = Convert.ToInt16(plantilla.itemNo);

            if (plantilla.plantillaCode == "PC223847464")
            {
                string s = "";
            }

            try
            {
                int step = Convert.ToInt16(plantilla.step);
                if (plantilla.step == 0 || plantilla.step == null)
                {
                    step = 1;
                }
                else
                {
                    step = newStep;
                }
                
                //tRSPSalaryTableDetail salaryData = salaryTable.Single(e => e.salaryGrade == plantilla.salaryGrade && e.step == step);
                //p.monthlyRate = Convert.ToDecimal(salaryData.rateMonth);
                 
                tRSPNOSIPropBudYear hasStep = NOSIList.SingleOrDefault(e => e.EIC == plantilla.EIC && e.year == 2022);

                //tRSPNOSIPropBudYear currNOSI = NOSIPrev.SingleOrDefault(e => e.EIC == item.EIC);
                if (hasStep != null)
                {
                    if (plantilla.step == hasStep.step - 1)
                    {
                        
                    }
                    else
                    {
                        hasStep = null;
                    }
                }

                if (hasStep != null) //HAS STEP
                {
                    p = _PSRequirementWithStep(plantilla, salaryTable, hasStep);
                }
                else
                {
                    p = _PSRequirement(plantilla, salaryTable, newStep);
                }
                //CHECK FOR LOYALTY
                return p;
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return p;
            }
            
        }



           [Authorize(Roles = "APRD,PBOStaff")]
        private TempCurrentPS _PSRequirement(vRSPPlantilla plantilla, IEnumerable<tRSPSalaryTableDetail> salaryTable, int newStep)
        {

            TempCurrentPS p = new TempCurrentPS();
            int monthCount = 12;

            
            int step = Convert.ToInt16(newStep);

            if (step == 0)
            {
                step = 1;
            }
                  
            // TEMP ONLY #########################################################################
            //SALARY GRADE
            int sg = Convert.ToInt16(plantilla.salaryGrade);
            //2021-2022 ##################### BUDGETING ONLY

            if (plantilla.EIC == "CA468696777D9180D6E1")
            {
                step = 1;
            }
            //TEMP FOR LAAG STEP 15 - STEP 16
            if (plantilla.EIC == "GL1892839168A07B31DA")
            {
                sg = 16; //BUDGETING ONLY
            }
            if (plantilla.EIC == "JC1978972800A1806026")
            {
                step = 1; //BUDGETING ONLY
            }
            if (plantilla.EIC == "MG169332696865AA8153")
            {
                sg = 16;
            }
            if (plantilla.EIC == "FS252853324670E8200D")
            {
                sg = 16;
            }
               //VILLANUEVA JUVY MARIE RNx
            if (plantilla.EIC == "JV177892922777325FB1")
            {
                sg = 16;
            }
            // TEMP ONLY #########################################################################

            decimal currMonthlyRate = Convert.ToDecimal(salaryTable.Single(e => e.salaryGrade == sg && e.step == step).rateMonth);

            if (currMonthlyRate == 0)
            {
                currMonthlyRate = 1;
            }

            decimal subsistence = 0;
            decimal laundry = 0;
            decimal hazard = 0;

            int hazardCode = Convert.ToInt16(plantilla.hazardCode);

            if (hazardCode == 0) // NO HAZARD 
            {
                subsistence = 0;
                laundry = 0;
            }
            else if (hazardCode == 1) // HEALTH WORKER
            {
                double partA = CalQHazadMonthly(sg, Convert.ToDouble(currMonthlyRate), monthCount);
                hazard = Convert.ToDecimal(partA);
                subsistence = 18000;
                laundry = 1800;

            }
            else if (hazardCode == 2) // SOCIAL WORKER
            {
                double partA = CalQHazadSocialWorker(sg, Convert.ToDouble(currMonthlyRate), monthCount);
                hazard = Convert.ToDecimal(partA);
                subsistence = 0;
                tRSPRata subs = getMySubs(plantilla.plantillaCode);
                if (subs != null)
                {
                    subsistence = Convert.ToDecimal(subs.RA);
                }           
                //NO LAUDRY
                laundry = 0;
            }

            DateTime dt = DateTime.Now;
            //ANNUAL SALARY
            decimal annualSalary = 0;
            annualSalary = currMonthlyRate * monthCount;

            // GSIS PREMIUM
            decimal gsisPrem = (currMonthlyRate * Convert.ToDecimal(.12)) * monthCount;

            // PHIC CONTRIBUTION
            decimal phicGovtShareA = Convert.ToDecimal(CalQPHICGovtShare(Convert.ToDouble(currMonthlyRate), monthCount));


            p.plantillaCode = plantilla.plantillaCode;
            p.salaryGrade = sg;
            p.step = step;
            p.itemNo = Convert.ToInt16(plantilla.itemNo).ToString("0000");
            p.oldItemNo = Convert.ToInt16(plantilla.oldItemNo).ToString("0000");
            p.EIC = plantilla.EIC;
            p.fullNameLast = plantilla.fullNameLast;
            p.fullNameFirst = plantilla.fullNameFirst;
            p.positionTitle = plantilla.positionTitle;
           
           
            p.monthlyRate = currMonthlyRate;
            p.annualRate = annualSalary;
            p.PERA = 2000 * 12;
            p.clothing = 6000;

            p.hazard = hazard;
            p.subsistence = subsistence;
            p.laundry = laundry;

            p.bonusMidYear = currMonthlyRate;
            p.bonusYearEnd = currMonthlyRate;
            p.cashGift = 5000;

            p.gsisPrem = gsisPrem;
            p.ECC = 1200;
            p.hdmfPrem = 1200;
            p.PHIC = phicGovtShareA;

            decimal totalPS = 0;
            totalPS = p.annualRate + p.PERA + p.clothing + p.RA + p.TA + p.hazard + p.laundry + p.subsistence + p.bonusMidYear + p.bonusYearEnd + p.cashGift + p.gsisPrem + p.hdmfPrem + p.PHIC + p.ECC;

            p.totalPS = totalPS;

            p.stepRemarks = "";

            return p;

        }

        



        [Authorize(Roles = "APRD,PBOStaff")]
        //WITH STEP
        private TempCurrentPS _PSRequirementWithStep(vRSPPlantilla plantilla, IEnumerable<tRSPSalaryTableDetail> salaryTable, tRSPNOSIPropBudYear stepData)
        {
            TempCurrentPS p = new TempCurrentPS();

            DateTime stepEffectiveDate = Convert.ToDateTime(stepData.effectiveDate);
            
            int iMonth = stepEffectiveDate.Month;
            int startMonth = (iMonth - 1);
            int endMonth = 12 - startMonth;

            int step = Convert.ToInt16(stepData.step); //APPLY NEW STEP INC
             
            // TEMP ONLY #########################################################################
            //SALARY GRADE
            int sg = Convert.ToInt16(plantilla.salaryGrade);
            //2021-2022 ##################### BUDGETING ONLY

            if (plantilla.EIC == "CA468696777D9180D6E1")
            {
                step = 1;
            }
            //TEMP FOR LAAG STEP 15 - STEP 16
            if (plantilla.EIC == "GL1892839168A07B31DA")
            {
                sg = 16; //BUDGETING ONLY
            }
            if (plantilla.EIC == "JC1978972800A1806026")
            {
                step = 1; //BUDGETING ONLY
            }
            if (plantilla.EIC == "MG169332696865AA8153")
            {
                sg = 16;
            }
            if (plantilla.EIC == "FS252853324670E8200D")
            {
                sg = 16;
            }
            //VILLANUEVA JUVY MARIE RNx
            if (plantilla.EIC == "JV177892922777325FB1")
            {
                sg = 16;
            }
            // TEMP ONLY #########################################################################


            decimal currMonthlyRate = Convert.ToDecimal(salaryTable.Single(e => e.salaryGrade == sg && e.step == plantilla.step).rateMonth);
            decimal newMonthlyRate = Convert.ToDecimal(salaryTable.Single(e => e.salaryGrade == sg && e.step == step).rateMonth);

            decimal subsistence = 0;
            decimal laundry = 0;
            decimal hazard = 0;



           int  hazardCode = Convert.ToInt16(plantilla.hazardCode);


            if (hazardCode == 0) // NO HAZARD 
            {
                subsistence = 0;
                laundry = 0;
            }
            else if (hazardCode == 1) // HEALTH WORKER
            {
                double partA = CalQHazadMonthly(sg, Convert.ToDouble(currMonthlyRate), startMonth);
                double partB = CalQHazadMonthly(sg, Convert.ToDouble(newMonthlyRate), endMonth);
                hazard = Convert.ToDecimal(partA + partB);
                subsistence = 18000;
                laundry = 1800;
            }
            else if (hazardCode == 2) // SOCIAL WORKER
            {
                double partA = CalQHazadSocialWorker(sg, Convert.ToDouble(currMonthlyRate), startMonth);
                double partB = CalQHazadSocialWorker(sg, Convert.ToDouble(newMonthlyRate), endMonth);
                hazard = Convert.ToDecimal(partA + partB);
                
                subsistence = 0;
                tRSPRata subs = getMySubs(plantilla.plantillaCode);
                if (subs != null)
                {
                    subsistence = Convert.ToDecimal(subs.RA);
                }    

                laundry = 0;
            }

            DateTime dt = DateTime.Now.AddYears(1);
            p.bonusMidYear = currMonthlyRate;
            p.bonusYearEnd = currMonthlyRate;
            //CHECK MIDYEAR BONUS
            string midYearCutOff = "May 15, " + dt.Year;
            if (stepEffectiveDate <= Convert.ToDateTime(midYearCutOff))
            {
                p.bonusMidYear = newMonthlyRate;
            }
            //CHECK YEAR-END BONUS
            string yearEndCutOff = "October 31, " + dt.Year;
            if (stepEffectiveDate <= Convert.ToDateTime(yearEndCutOff))
            {
                p.bonusYearEnd = newMonthlyRate;
            }

            //ANNUAL SALARY
            decimal annualSalary = 0;
            annualSalary = currMonthlyRate * startMonth;
            annualSalary = annualSalary + (newMonthlyRate * endMonth);

            // GSIS PREMIUM
            decimal gsisPartA = (currMonthlyRate * Convert.ToDecimal(.12)) * startMonth;
            decimal gsisPartB = (newMonthlyRate * Convert.ToDecimal(.12)) * endMonth;
            decimal gsisPrem = gsisPartA + gsisPartB;

            // PHIC CONTRIBUTION
            decimal phicGovtShareA = Convert.ToDecimal(CalQPHICGovtShare(Convert.ToDouble(currMonthlyRate), startMonth));
            decimal phicGovtShareB = Convert.ToDecimal(CalQPHICGovtShare(Convert.ToDouble(newMonthlyRate), endMonth));

            p.plantillaCode = plantilla.plantillaCode;
            p.itemNo = Convert.ToInt16(plantilla.itemNo).ToString("0000");
            p.oldItemNo = Convert.ToInt16(plantilla.oldItemNo).ToString("0000");
            p.EIC = plantilla.EIC;
            p.fullNameLast = plantilla.fullNameLast;
            p.fullNameFirst = plantilla.fullNameFirst;
            p.positionTitle = plantilla.positionTitle;
            p.salaryGrade = sg;
            p.step = step;

            p.monthlyRate = newMonthlyRate;
            p.annualRate = annualSalary;
            p.PERA = 2000 * 12;
            p.clothing = 6000;

            p.hazard = hazard;
            p.subsistence = subsistence;
            p.laundry = laundry;
            p.RA = 0;
            p.TA = 0;
               
            p.cashGift = 5000;

            p.gsisPrem = gsisPrem;
            p.ECC = 1200;
            p.hdmfPrem = 1200;
            p.PHIC = phicGovtShareA + phicGovtShareB;

            decimal totalPS = 0;
            totalPS = p.annualRate + p.PERA + p.clothing + p.hazard + p.laundry + p.subsistence + p.bonusMidYear + p.bonusYearEnd + p.cashGift + p.gsisPrem + p.hdmfPrem + p.PHIC + p.ECC;
            p.totalPS = totalPS;

            p.stepRemarks = Convert.ToDateTime(stepData.effectiveDate).ToString("MM/dd");

            return p;

        }


        //PHIC GOVT SHARE COMPUTATION
        //private double CalQPHICGovtShare(double rateMonth, int monthCount)
        //{
        //    double monthlyPrem = 150;
        //    if (rateMonth >= 10000.01 && rateMonth < 60000)
        //    {
        //        monthlyPrem = (rateMonth * .03) / 2;
        //    }
        //    else if (rateMonth >= 60000)
        //    {
        //        monthlyPrem = 1800 / 2;
        //    }
        //    return monthlyPrem * monthCount;
        //}

        private double CalQPHICGovtShare(double rateMonth, int monthCount)
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
            return monthlyPrem * monthCount;
        }
        
        //HAZARD SOCIAL WORKER
        private double CalQHazadSocialWorker(int sg, double monthlyRate, int monthCount)
        {
            double rate =  (((monthlyRate) / 22) * .20 ) * 30;
            rate = rate * monthCount;
            return rate;
        }

        //HAZARD
        private double CalQHazadMonthly(int sg, double monthlyRate, int monthCount)
        {
            double total = 0;
            double prcntg = 0;

            if (sg <= 19)
            {
                prcntg = .25;
            }
            else if (sg == 20)
            {
                prcntg = .15;
            }
            else if (sg == 21)
            {
                prcntg = .13;
            }
            else if (sg == 22)
            {
                prcntg = .12;
            }
            else if (sg == 23)
            {
                prcntg = .11;
            }
            else if (sg == 24)
            {
                prcntg = .10;
            }
            else if (sg == 25)
            {
                prcntg = .10;
            }
            else if (sg == 26)
            {
                prcntg = .09;
            }
            else if (sg == 27)
            {
                prcntg = .08;
            }
            else if (sg == 28)
            {
                prcntg = .07;
            }
            else
            {
                prcntg = .05;
            }
            total = (monthlyRate * prcntg) * monthCount; //PER MONTH
            return total;

        }


        private tRSPRata getMySubs(string plantillaCode)
        {
            List<tRSPRata> myList = new List<tRSPRata>();
            //PRSWO
            myList.Add(new tRSPRata() { EIC = "PC632027662", RA = 18000 });
            //PG ASST
            myList.Add(new tRSPRata() { EIC = "PC1275890709", RA = 18000 });
            //SWO IV
            myList.Add(new tRSPRata() { EIC = "PC434303901", RA = 18000 });
            myList.Add(new tRSPRata() { EIC = "PC902677367", RA = 18000 });
            
            //SWO III
            myList.Add(new tRSPRata() { EIC = "PC1532784360", RA = 18000 });
            myList.Add(new tRSPRata() { EIC = "PC1747096738", RA = 18000 });
            myList.Add(new tRSPRata() { EIC = "PC77731620", RA = 18000 });
            myList.Add(new tRSPRata() { EIC = "PC866569341", RA = 18000 });
            //SWO II
            myList.Add(new tRSPRata() { EIC = "PC1566407906", RA = 18000 });
            myList.Add(new tRSPRata() { EIC = "PC2047752602", RA = 18000 });
            myList.Add(new tRSPRata() { EIC = "PC2066164358", RA = 18000 });
            myList.Add(new tRSPRata() { EIC = "PC291612443", RA = 18000 });
 
            //SWO I
            myList.Add(new tRSPRata() { EIC = "PC1070602817", RA = 18000 });
            myList.Add(new tRSPRata() { EIC = "PC2118608569", RA = 18000 });
            myList.Add(new tRSPRata() { EIC = "PC2129164102", RA = 18000 });
            myList.Add(new tRSPRata() { EIC = "PC403870139", RA = 18000 });

            myList.Add(new tRSPRata() { EIC = "PC698749903", RA = 18000 });
            myList.Add(new tRSPRata() { EIC = "PC961397810", RA = 18000 });
            //SW ASST
            myList.Add(new tRSPRata() { EIC = "PC171282946", RA = 18000 });

            tRSPRata item = myList.SingleOrDefault(e => e.EIC == plantillaCode);

            return item;
        }
      


        private tRSPRata getMyRATA(string plantillaCode)
        {
            List<tRSPRata> myList = new List<tRSPRata>();
            //GOV
            myList.Add(new tRSPRata() { EIC = "PC1395498714", RA = 11000, TA = 11000 });
            //VICE-GOV
            myList.Add(new tRSPRata() { EIC = "PC1002493235", RA = 10000, TA = 0 });
            //SP
            myList.Add(new tRSPRata() { EIC = "PC1330822667", RA = 8500, TA = 8500 });
            myList.Add(new tRSPRata() { EIC = "PC1376681184", RA = 8500, TA = 8500 });
            myList.Add(new tRSPRata() { EIC = "PC1964721781", RA = 8500, TA = 8500 });
            myList.Add(new tRSPRata() { EIC = "PC32882793", RA = 8500, TA = 8500 });
            myList.Add(new tRSPRata() { EIC = "PC401484990", RA = 8500, TA = 8500 });
            myList.Add(new tRSPRata() { EIC = "PC433938382", RA = 8500, TA = 8500 });
            myList.Add(new tRSPRata() { EIC = "PC830271536", RA = 8500, TA = 8500 });
            myList.Add(new tRSPRata() { EIC = "PC933216354", RA = 8500, TA = 8500 });
            myList.Add(new tRSPRata() { EIC = "PC949228266", RA = 8500, TA = 8500 });
            //SP EX-O
            myList.Add(new tRSPRata() { EIC = "PC1024741735", RA = 8500, TA = 8500 });
            myList.Add(new tRSPRata() { EIC = "PC447726653", RA = 8500, TA = 8500 });
            myList.Add(new tRSPRata() { EIC = "PC680134735", RA = 8500, TA = 8500 });
            myList.Add(new tRSPRata() { EIC = "PC896289307", RA = 8500, TA = 8500 });

            // PG DEPT HEAD
            myList.Add(new tRSPRata() { EIC = "PC2126664416", RA = 8500, TA = 0 });
            myList.Add(new tRSPRata() { EIC = "PC128392120", RA = 8500, TA = 0 });
            myList.Add(new tRSPRata() { EIC = "PC634339122", RA = 8500, TA = 0 });
            myList.Add(new tRSPRata() { EIC = "PC638378104", RA = 8500, TA = 0 });
            myList.Add(new tRSPRata() { EIC = "PC1400873911", RA = 8500, TA = 0 });
            myList.Add(new tRSPRata() { EIC = "PC108584726", RA = 8500, TA = 0 });
            myList.Add(new tRSPRata() { EIC = "PC1760980943", RA = 8500, TA = 0 });
            myList.Add(new tRSPRata() { EIC = "PC1141635432", RA = 8500, TA = 0 });
            myList.Add(new tRSPRata() { EIC = "PC1145456246", RA = 8500, TA = 0 });
            myList.Add(new tRSPRata() { EIC = "PC164905793", RA = 8500, TA = 0 });
            myList.Add(new tRSPRata() { EIC = "PC465459045", RA = 8500, TA = 0 });
            myList.Add(new tRSPRata() { EIC = "PC808265900", RA = 8500, TA = 0 });
            myList.Add(new tRSPRata() { EIC = "PC632027662", RA = 8500, TA = 0 });
            myList.Add(new tRSPRata() { EIC = "PC1769969647", RA = 8500, TA = 0 });
            myList.Add(new tRSPRata() { EIC = "PC2043679706", RA = 8500, TA = 0 });
            myList.Add(new tRSPRata() { EIC = "PC1105503071", RA = 8500, TA = 0 });
            myList.Add(new tRSPRata() { EIC = "PC341604431", RA = 8500, TA = 0 });
            myList.Add(new tRSPRata() { EIC = "PC1604107249", RA = 8500, TA = 0 });
            myList.Add(new tRSPRata() { EIC = "PC772294349", RA = 8500, TA = 0 });

         
            // ASST. PG DEPT.
            myList.Add(new tRSPRata() { EIC = "PC130320409", RA = 7500, TA = 7500 });
            myList.Add(new tRSPRata() { EIC = "PC118754989", RA = 7500, TA = 7500 });
            myList.Add(new tRSPRata() { EIC = "PC1194134706", RA = 7500, TA = 7500 });
            myList.Add(new tRSPRata() { EIC = "PC1275890709", RA = 7500, TA = 7500 });
            myList.Add(new tRSPRata() { EIC = "PC14917422", RA = 7500, TA = 7500 });
            myList.Add(new tRSPRata() { EIC = "PC1663268962", RA = 7500, TA = 7500 });
            myList.Add(new tRSPRata() { EIC = "PC167218590", RA = 7500, TA = 7500 });
            myList.Add(new tRSPRata() { EIC = "PC1739288485", RA = 7500, TA = 7500 });
            myList.Add(new tRSPRata() { EIC = "PC1947772203", RA = 7500, TA = 7500 });
            myList.Add(new tRSPRata() { EIC = "PC1990933061", RA = 7500, TA = 7500 });
            myList.Add(new tRSPRata() { EIC = "PC277201814", RA = 7500, TA = 7500 });
            myList.Add(new tRSPRata() { EIC = "PC622132826", RA = 7500, TA = 7500 });
            myList.Add(new tRSPRata() { EIC = "PC797488630", RA = 7500, TA = 7500 });
            myList.Add(new tRSPRata() { EIC = "PC847159161", RA = 7500, TA = 7500 });
            myList.Add(new tRSPRata() { EIC = "PC908815061", RA = 7500, TA = 7500 });
            myList.Add(new tRSPRata() { EIC = "PC654684200", RA = 7500, TA = 7500 });

            myList.Add(new tRSPRata() { EIC = "PC883315356", RA = 7500, TA = 7500 }); //PVO
            myList.Add(new tRSPRata() { EIC = "PC1621830574", RA = 7500, TA = 7500 }); //PHO
            myList.Add(new tRSPRata() { EIC = "PC1578737086", RA = 7500, TA = 7500 }); //PHO
            
            //CHIEF OF HOSPITALS
            myList.Add(new tRSPRata() { EIC = "PC1184815520", RA = 5000, TA = 5000 });
            myList.Add(new tRSPRata() { EIC = "PC1243518423", RA = 5000, TA = 5000 });
            myList.Add(new tRSPRata() { EIC = "PC2123177139", RA = 5000, TA = 5000 });
            myList.Add(new tRSPRata() { EIC = "PC552784905", RA = 5000, TA = 5000 });

            tRSPRata item = myList.SingleOrDefault(e => e.EIC == plantillaCode);
            return item;
        }

       


        /////////////////////////////////////////////////////////////////////////////////////////

        // ******************************************************************************************************
        // APPLICANT LIST
        [Authorize(Roles = "APRD")]
        public ActionResult Applicant()
        {
            return View();
        }

        [Authorize(Roles = "APRD")]
        [HttpPost]
        public JsonResult GetApplicantList()
        {
            try
            {
                var app = db.tApplicants.Select(e => new
                {
                    e.applicantCode,
                    e.fullNameLast,
                }).OrderBy(o => o.fullNameLast).ToList();
                return Json(new { status = "success", applicant = app }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string err = ex.ToString();
                return Json(new { status = "Error loading data...!" }, JsonRequestBehavior.AllowGet);
            }
        }


        [Authorize(Roles = "APRD")]
        [HttpPost]
        public JsonResult GetApplicantData(string id)
        {
            try
            {
                tApplicant data = db.tApplicants.Single(e => e.applicantCode == id);
                return Json(new { status = "success", applicantData = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string err = ex.ToString();
                return Json(new { status = "Error loading data...!" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "APRD")]
        [HttpPost]
        public JsonResult UpdateApplicantData(tApplicant data)
        {
            string hasError = _CheckForError(data);

            if (hasError == "success")
            {
                try
                {
                    tApplicant app = db.tApplicants.Single(e => e.applicantCode == data.applicantCode);
                    app.username = data.username.Trim().ToLower();
                    app.password = data.password.Trim().ToLower();
                    app.mobileNo = data.mobileNo;
                    app.email = data.email;
                    app.extName = data.extName;
                    db.SaveChanges();
                    return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    string err = ex.ToString();
                    return Json(new { status = "Error updating data...!" }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { status = hasError }, JsonRequestBehavior.AllowGet);

        }


        private string _CheckForError(tApplicant data)
        {
            string res = "success";

            if (data.username == null || data.password == null)
            {
                return "Please type username and password!";
            }

            if (data.username.Length < 6 && data.password.Length < 6)
            {
                return "Username & password must be 6 characters!";
            }

            if (data.mobileNo != null && data.mobileNo != "")
            {
                if (data.mobileNo.Length != 11 || data.mobileNo.Substring(0, 2) != "09")
                {
                    return "Invalid phone number";
                }
            }

            return res;
        }

        //
        /////////////////////////////////////////////////////////////////////////////////////////

        // ******************************************************************************************************
        // IPRC LIST
        public ActionResult PerformanceRating()
        {

            return View();
        }

        [HttpPost]
        public JsonResult GetEmployeeList()
        {
            try
            {

                string uEIC = Session["_EIC"].ToString();

                var myList = db.vRSPEmployeeLists.Select(e => new
                {
                    e.EIC,
                    e.fullNameLast
                }).OrderBy(o => o.fullNameLast).ToList();

                var rList = db.vSPMSIPCRRatings.Select(e => new
                {
                    e.controlNo,
                    e.fullNameFirst,
                    e.fullNameLast,
                    e.positionTitle,
                    e.subPositionTitle,
                    e.ratingAdj,
                    e.ratingNum,
                    e.encoderEIC,
                    e.transDT
                }).Where(e => e.encoderEIC == uEIC).OrderBy(o => o.fullNameLast).ToList();

                return Json(new { status = "success", list = myList, ratingList = rList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult SavePerformanceRating(tSPMSIPRCRating data)
        {
            try
            {
                string uEIC = Session["_EIC"].ToString();
                string code = "PM" + DateTime.Now.ToString("yyMMddHHmmssfff") + data.EIC.Substring(0, 8);
                tSPMSIPRCRating r = new tSPMSIPRCRating();
                r.controlNo = code;
                r.EIC = data.EIC;
                r.ratingNum = data.ratingNum;
                r.ratingAdj = data.ratingAdj;
                r.year = 2020;
                r.semester = 2;
                r.details = "";
                r.remarks = data.remarks;
                r.encoderEIC = uEIC;
                r.transDT = DateTime.Now;
                db.tSPMSIPRCRatings.Add(r);
                db.SaveChanges();

                var rList = db.vSPMSIPCRRatings.Select(e => new
                {
                    e.controlNo,
                    e.fullNameFirst,
                    e.fullNameLast,
                    e.positionTitle,
                    e.subPositionTitle,
                    e.ratingAdj,
                    e.ratingNum,
                    e.encoderEIC,
                    e.transDT
                }).Where(e => e.encoderEIC == uEIC).OrderBy(o => o.transDT).ToList();

                return Json(new { status = "success", ratingList = rList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult PSCalculator()
        {

            return View();
        }


        [HttpPost]
        public JsonResult PSCalculatorData()
        {
            try
            {
                var myList = db.tRSPZPSComputationMains.Select(e => new
                {
                    e.reportCode,
                    e.planName,
                    e.fundSourceCode,
                    e.recNo
                }).Where(e => e.recNo > 0).ToList();

                return Json(new { status = "success", list = myList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetListByCode(string id)
        {
            try
            {
                IEnumerable<vRSPZPSComputation> myList = db.vRSPZPSComputations.Where(e => e.reportCode == id).ToList();

                return Json(new { status = "success", psPlanItemList = myList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult PrintPSPlanByCode(string id)
        {
            try
            {
                tRSPZPSComputationMain data = db.tRSPZPSComputationMains.Single(e => e.reportCode == id);
                //string reportType = "";
                //if (data.employmentStatusCode == "05")
                //{
                //    reportType = "PSCOMPUTATION";
                //}
                //else if (data.employmentStatusCode == "06")
                //{
                //    reportType = "PSCOMPUTATIONNGS";
                //}
                //else if (data.employmentStatusCode == "07" || data.employmentStatusCode == "08")
                //{
                //    reportType = "PSCOMPUTATIONNGS";
                //}
                Session["ReportType"] = "PSCOMPUTATION";
                Session["PrintReport"] = id;
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult GetPlanStatus(string id)
        {
            string empStatus = "";
            try
            {
                tRSPZPSComputationMain data = db.tRSPZPSComputationMains.Single(e => e.reportCode == id);

                List<tRSPPosition> position = new List<tRSPPosition>();

                if (data.employmentStatusCode == "05" || data.employmentStatusCode == "06")
                {

                    IEnumerable<tRSPPosition> list = db.tRSPPositions.Where(e => e.isActive == 1).OrderBy(o => o.positionTitle).ToList();

                    foreach (tRSPPosition item in list)
                    {
                        position.Add(new tRSPPosition()
                        {
                            positionCode = item.positionCode,
                            positionTitle = item.positionTitle
                        });
                    }
                    empStatus = "JOB ORDER";
                    if (data.employmentStatusCode == "05")
                    {
                        empStatus = "CASUAL";
                    }

                }

                else if (data.employmentStatusCode == "07" || data.employmentStatusCode == "08")
                {
                    IEnumerable<tRSPPositionContract> list = db.tRSPPositionContracts.Where(e => e.isActive == 1).OrderBy(o => o.positionTitle).ToList();
                    foreach (tRSPPositionContract item in list)
                    {
                        position.Add(new tRSPPosition()
                        {
                            positionCode = item.positionCode,
                            positionTitle = item.positionTitle
                        });
                    }

                    empStatus = "HONORARIUM";
                    if (data.employmentStatusCode == "07")
                    {
                        empStatus = "CONTRACT OF SERVICE";
                    }

                }

                return Json(new { status = "success", stat = empStatus, position = position }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "APRD")]
        public ActionResult StatementALN()
        {
            return View();
        }

        public class TempSALNData
        {
            public string EIC { get; set; }
            public string batchCode { get; set; }
            public string lastName { get; set; }
            public string firstname { get; set; }
            public string middleName { get; set; }
            public string positionTitle { get; set; }
            public string TIN { get; set; }
            public decimal? netWorth { get; set; }
            public int? isGovtServSpouse { get; set; }
            public string spouseRemarks { get; set; }
            public string departmentCode { get; set; }
            public int isJointFiling { get; set; }
        }

        public JsonResult GetSALNById(string id)
        {
            IEnumerable<tOrgDepartment> dept = db.tOrgDepartments.Where(e => e.isActive == true).OrderBy(o => o.orderNo).ToList();

            if (id == null)
            {
                id = "OC191015134332380001";
            }

            try
            {

                List<TempSALNData> myList = SALNByOffice(id);

                //IEnumerable<vRSPPlantillaPersonnel> list = db.vRSPPlantillaPersonnels.Where(e => e.departmentCode == id).OrderBy(o => o.fullNameLast).ToList();
                //var list = db.vRSPPlantillaPersonnels.Select(e => new
                //{
                //    e.departmentCode,
                //    e.fullNameLast,
                //    e.EIC
                //}).Where(e => e.departmentCode == id).OrderBy(o => o.fullNameLast).ToList();

                //string tempBatchCode = "SALN202012310001";

                //IEnumerable<tRSPEmployee> employees = db.tRSPEmployees.ToList();
                //IEnumerable<tSALNData> salnData = db.tSALNDatas.Where(e => e.batchCode == tempBatchCode).ToList();




                //List<TempSALNData> myList = new List<TempSALNData>();
                //foreach (vRSPPlantillaPersonnel item in list)
                //{

                //    tRSPEmployee emp = employees.Single(e => e.EIC == item.EIC);
                //    tSALNData s = salnData.SingleOrDefault(e => e.EIC == item.EIC);

                //    if (s != null)
                //    {
                //        tSALNData n = db.tSALNDatas.Single(e => e.EIC == emp.EIC && e.batchCode == tempBatchCode);
                //        n.lastname = emp.lastName;
                //        n.firstname = emp.firstName;
                //        n.middleName = emp.middleName;
                //        n.positionTitle = item.positionTitle;
                //        n.departmentCode = item.departmentCode;
                //        db.SaveChanges();
                //    }
                //    else
                //    {
                //        tSALNData nw = new tSALNData();
                //        nw.batchCode = tempBatchCode;
                //        nw.EIC = emp.EIC;
                //        nw.lastname = emp.lastName;
                //        nw.firstname = emp.firstName;
                //        nw.middleName = emp.middleName;
                //        nw.positionTitle = item.positionTitle;
                //        nw.departmentCode = item.departmentCode;
                //        db.tSALNDatas.Add(nw);
                //        db.SaveChanges();
                //    }


                //    decimal net = 0;
                //    int isJoint = 0;                  
                //    string remarks = "";
                //    if (s != null)
                //    {
                //        net = Convert.ToDecimal(s.networth);
                //        isJoint = Convert.ToInt16(s.isJointFiling);
                //        remarks = s.spouseRemarks;                        
                //    }

                //    myList.Add(new TempSALNData()
                //    {
                //        EIC = item.EIC,
                //        fullNameLast = item.fullNameLast,
                //        TIN =  emp.TINNo,
                //        positionTitle = item.positionTitle,
                //        netWorth = net,
                //        spouseRemarks = remarks,
                //        isJointFiling = isJoint
                //    });
                //}
                return Json(new { status = "success", dept = dept, emps = myList }, JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        private List<TempSALNData> SALNByOffice(string departmentCode)
        {

            string tempBatchCode = "SALN202012310001";
            //IEnumerable<vRSPPlantillaPersonnel> list = db.vRSPPlantillaPersonnels.Where(e => e.departmentCode == departmentCode).OrderBy(o => o.fullNameLast).ToList();

            try
            {
                List<TempSALNData> res = (from s in db.tSALNDatas
                                          join emp in db.tRSPEmployees
                                            on s.EIC equals emp.EIC
                                          select new TempSALNData
                                          {
                                              EIC = s.EIC,
                                              lastName = s.lastname,
                                              firstname = s.firstname,
                                              middleName = s.middleName,
                                              positionTitle = s.positionTitle,
                                              TIN = emp.TINNo,
                                              netWorth = s.networth, // netWorth = Convert.ToDecimal(s.networth),
                                              spouseRemarks = s.spouseRemarks,
                                              isJointFiling = s.isJointFiling == 1 ? 1 : 0, //  isJointFiling = Convert.ToInt16(s.isJointFiling),
                                              departmentCode = s.departmentCode,
                                              batchCode = s.batchCode
                                          }).Where(e => e.batchCode == tempBatchCode && e.departmentCode == departmentCode).OrderBy(o => o.lastName).ThenBy(o => o.firstname).ToList();
                return res;

            }
            catch (Exception ex)
            {
                string err = ex.ToString();
                throw;
            }



            //IEnumerable<tRSPEmployee> employees = db.tRSPEmployees.ToList();
            //IEnumerable<tSALNData> salnData = db.tSALNDatas.Where(e => e.batchCode == tempBatchCode).ToList();

            //List<TempSALNData> myList = new List<TempSALNData>();
            //foreach (vRSPPlantillaPersonnel item in list)
            //{

            //    tRSPEmployee emp = employees.Single(e => e.EIC == item.EIC);                
            //    tSALNData s = salnData.SingleOrDefault(e => e.EIC == item.EIC);

            //    //if (s != null)
            //    //{
            //    //    tSALNData n = db.tSALNDatas.Single(e => e.EIC == emp.EIC && e.batchCode == tempBatchCode);
            //    //    n.lastname = emp.lastName;
            //    //    n.firstname = emp.firstName;
            //    //    n.middleName = emp.middleName;
            //    //    n.positionTitle = item.positionTitle;
            //    //    n.departmentCode = item.departmentCode;
            //    //    db.SaveChanges();
            //    //}
            //    //else
            //    //{
            //    //    tSALNData nw = new tSALNData();
            //    //    nw.batchCode = tempBatchCode;
            //    //    nw.EIC = emp.EIC;
            //    //    nw.lastname = emp.lastName;
            //    //    nw.firstname = emp.firstName;
            //    //    nw.middleName = emp.middleName;
            //    //    nw.positionTitle = item.positionTitle;
            //    //    nw.departmentCode = item.departmentCode;
            //    //    db.tSALNDatas.Add(nw);
            //    //    db.SaveChanges();
            //    //}

            //    decimal net = 0;
            //    int isJoint = 0;
            //    string remarks = "";
            //    if (s != null)
            //    {
            //        net = Convert.ToDecimal(s.networth);
            //        isJoint = Convert.ToInt16(s.isJointFiling);
            //        remarks = s.spouseRemarks;
            //    }

            //    myList.Add(new TempSALNData()
            //    {
            //        EIC = item.EIC,
            //        //fullNameLast = item.fullNameLast,
            //        TIN = emp.TINNo,
            //        positionTitle = item.positionTitle,
            //        netWorth = net,
            //        spouseRemarks = remarks,
            //        isJointFiling = isJoint
            //    });
            //}

        }

        [Authorize(Roles = "APRD")]
        public JsonResult GetSALNByEmployeeByOfficeId(string id)
        {
            List<TempSALNData> myList = SALNByOffice(id);
            return Json(new { status = "success", emps = myList }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [Authorize(Roles = "APRD")]
        public JsonResult GetSALNBatchByEmpId(string code, string id)
        {
            try
            {
                tSALNData saln = db.tSALNDatas.SingleOrDefault(e => e.batchCode == "SALN202012310001" && e.EIC == id);

                if (saln == null)
                {
                    tSALNData temp = new tSALNData();
                    temp.EIC = id;
                    temp.batchCode = code;
                    temp.spouseRemarks = "";
                    temp.isJointFiling = 0;
                    saln = temp;
                }
                return Json(new { status = "success", data = saln }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Authorize(Roles = "APRD")]
        public JsonResult UpdateEmployeeSALN(TempSALNData data, string code)
        {
            try
            {
                string uEIC = Session["_EIC"].ToString();
                tSALNData saln = db.tSALNDatas.SingleOrDefault(e => e.batchCode == "SALN202012310001" && e.EIC == data.EIC);

                if (saln == null)
                {
                    //CREATE NEW
                    tSALNData n = new tSALNData();
                    n.batchCode = data.batchCode;
                    n.EIC = data.EIC;
                    n.networth = data.netWorth;
                    n.positionTitle = data.positionTitle;
                    n.spouseRemarks = data.spouseRemarks;
                    n.isJointFiling = data.isJointFiling;
                    n.transDT = DateTime.Now;
                    n.userEIC = uEIC;
                    db.tSALNDatas.Add(n);
                    db.SaveChanges();
                }
                else
                {
                    //UPDATE
                    saln.networth = data.netWorth;
                    saln.spouseRemarks = data.spouseRemarks;
                    saln.isJointFiling = data.isJointFiling;
                    saln.positionTitle = data.positionTitle;
                    saln.userEIC = uEIC;
                    saln.transDT = DateTime.Now;
                    db.SaveChanges();
                }
                List<TempSALNData> sln = SALNByOffice(code);
                return Json(new { status = "success", emps = sln }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Authorize(Roles = "APRD")]
        public JsonResult GetEmployeeListSALN()
        {
            var myList = db.vRSPEmployeeLists.Select(e => new
            {
                e.EIC,
                e.fullNameLast
            }).OrderBy(o => o.fullNameLast).ToList();
            return Json(new { status = "success", list = myList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize(Roles = "APRD")]
        public JsonResult AddEmpToSALN(string id, string code)
        {
            try
            {
                int count = db.tSALNDatas.Where(e => e.batchCode == id && e.departmentCode == code).Count();
                if (count >= 1)
                {
                    return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
                }

                string uEIC = Session["_EIC"].ToString();
                vRSPEmployeeList emp = db.vRSPEmployeeLists.SingleOrDefault(e => e.EIC == id);
                if (emp != null)
                {
                    string batchCode = "SALN202012310001";
                    tSALNData s = new tSALNData();
                    s.batchCode = batchCode;
                    s.EIC = emp.EIC;
                    s.lastname = emp.lastName;
                    s.firstname = emp.firstName;
                    s.middleName = emp.middleName;
                    s.positionTitle = emp.positionTitle;
                    s.departmentCode = code;
                    s.userEIC = uEIC;
                    db.tSALNDatas.Add(s);
                    db.SaveChanges();

                    List<TempSALNData> sln = SALNByOffice(code);
                    return Json(new { status = "success", emps = sln }, JsonRequestBehavior.AllowGet);
                }


                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string err = ex.Message;
                throw;
            }
        }

        [HttpPost]
        [Authorize(Roles = "APRD")]
        public JsonResult PrintReportSALN(string code)
        {
            try
            {
                Session["ReportType"] = "SALN";
                Session["PrintReport"] = code;
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string err = ex.ToString();
                throw;
            }
        }


        //PS MONTHLY SAVINGS
        public ActionResult MonthlySavings()
        {

            return View();
        }


        public class TempMonthYear
        {
            public string monthYearCode { get; set; }
            public string monthYearName { get; set; }
        }

        [HttpPost]
        public JsonResult MonthlySavingsInitData()
        {
            try
            {
                int monthCount = Convert.ToInt16(DateTime.Now.ToString("MM"));
                int yearCount = DateTime.Now.Year;
                List<TempMonthYear> monthList = new List<TempMonthYear>();
                DateTime dt = Convert.ToDateTime("January 1, " + yearCount);
                for (int i = 1; i <= monthCount; i++)
                {
                    monthList.Add(new TempMonthYear()
                    {
                        monthYearCode = dt.ToString("MMMM dd, yyyy"),
                        monthYearName = dt.ToString("MMMM yyyy")
                    });
                    dt = dt.AddMonths(1);
                }

                //tOrgFunction
                IEnumerable<tOrgFunction> funcList = db.tOrgFunctions.Where(e => e.isActive == true).OrderBy(o => o.orderNo).ToList();

                //IEnumerable<tOrgDepartment> dept = db.tOrgDepartments.Where(e => e.isActive == true).OrderBy(o => o.orderNo).ToList();
                List<TempOfficeList> deptList = new List<TempOfficeList>();
                foreach (tOrgFunction d in funcList)
                {
                    deptList.Add(new TempOfficeList()
                    {
                        departmentCode = d.functionCode,
                        departmentNameShort = d.functionDesc
                    });
                }
                return Json(new { status = "success", department = deptList, monthList = monthList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }

        }



        //public class TempMonthSavings
        //{
        //    public string plantillaCode { get; set; }
        //    public string itemNo { get; set; }
        //    public string positionTitle { get; set; }
        //    public int salaryGrade { get; set; }
        //    public string monthlyRate { get; set; }
        //}


        //[HttpPost]
        //public JsonResult GenerateSavingsReport(string id, string code)
        //{
        //    try
        //    {
        //        // !!! CHECK IF CODE HAS BEEN CREATED                
        //        // !!! CHECK FOR FUNDING STATUS
        //        DateTime dt = Convert.ToDateTime(id);
        //        DateTime dtMax = dt.AddMonths(1);

        //        //SEPARATION
        //        IEnumerable<vRSPSeparation> separationList = db.vRSPSeparations.Where(e => e.separationDate.Year == dt.Date.Year).ToList();
        //        //
        //        //dt = dt.AddMonths(1);
        //        IEnumerable<tRSPAppointment> appointmentList = db.tRSPAppointments.Where(e => e.effectivityDate.Value.Year == dt.Date.Year).ToList();

        //        IEnumerable<vRSPPlantilla> list = db.vRSPPlantillas.Where(e => e.departmentCode == code && e.isFunded == true).OrderBy(o => o.itemNo).ToList();

        //        List<TempMonthSavings> myList = new List<TempMonthSavings>();

        //        foreach (vRSPPlantilla item in list)
        //        {
        //            int hasSavingsTag = 0;
        //            //tRSPSeparation sepItem = separationList.SingleOrDefault(e => e.effectiveDate.Month == dt.Month);
        //            //tRSPAppointment appItem = appointmentList.SingleOrDefault(e => e.effectivityDate.Value.Month == dt.Month);

        //            if (item.EIC == null) //IF VACANT and no separation for this month
        //            {
        //                if (item.itemNo == 171)
        //                {
        //                    hasSavingsTag = 0;
        //                }
        //                DateTime tmpDT = dt.AddMonths(1).AddDays (-1);

        //                 //check if has no appointment for this month
        //                 tRSPAppointment appItem = appointmentList.SingleOrDefault(e => e.plantillaCode == item.plantillaCode && e.effectivityDate.Value.Month == dt.Month);
        //                 vRSPSeparation sepItem = separationList.SingleOrDefault(e => e.plantillaCode == item.plantillaCode && e.separationDate > tmpDT);
        //                 if (appItem == null && sepItem == null)
        //                 {
        //                     hasSavingsTag = 1;
        //                 }                         
        //            }
        //            else if (item != null)
        //            {
        //                tRSPAppointment appItem = appointmentList.SingleOrDefault(e => e.plantillaCode == item.plantillaCode);
        //                if (appItem != null)
        //                {
        //                    DateTime tmpDT = dt.AddDays(-1);
        //                    if (appItem.effectivityDate < tmpDT)
        //                    {
        //                        hasSavingsTag = 1;
        //                    }
        //                }
        //            }

        //            if (hasSavingsTag == 1)
        //            {
        //                //add to savings list
        //                myList.Add(new TempMonthSavings()
        //                {
        //                    plantillaCode = item.plantillaCode,
        //                    itemNo = Convert.ToInt16(item.newItemNo).ToString("0000"),
        //                    monthlyRate  = Convert.ToDecimal(item.rateMonth).ToString("#,##0.00"),
        //                    positionTitle = item.positionTitle,
        //                    salaryGrade = Convert.ToInt16(item.salaryGrade) 
        //                });
        //            }                     
        //        }

        //        string monthCode = dt.ToString("yyyyMM");
        //        //if DEPARTMENT HAS NO SAVINGS COMPUTATION FOR THIS MONTH
        //        int counter = db.tRSPZPSMonthlySavings.Where(e => e.departmentCode == code && e.monthCode == monthCode).Count();

        //        if (counter == 0)
        //        {
        //            //SAVE HERE
        //        }

        //        return Json(new { status = "success", list = myList }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        string errMsg = ex.ToString();
        //        return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
        //    }
        //}



        [HttpPost]
        public JsonResult PrintSavingsReport(string id, string code)
        {
            try
            {
                DateTime dt = Convert.ToDateTime(id);
                string printCode = dt.ToString("yyyyMM") + code;
                Session["ReportType"] = "PSSAVINGS";
                Session["PrintReport"] = printCode;
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        public class TempPositionProfile
        {
            public int recNo { get; set; }
            public string positionCode { get; set; }
            public string positionTitle { get; set; }

            public int casualM { get; set; }
            public int casualF { get; set; }
            public int casualTotal { get; set; }
            public int joM { get; set; }
            public int joF { get; set; }
            public int joTotal { get; set; }
            public int cosM { get; set; }
            public int cosF { get; set; }
            public int cosTotal { get; set; }
            public int honM { get; set; }
            public int honF { get; set; }
            public int honTotal { get; set; }
            public int total { get; set; }
        }


        [HttpPost]
        public JsonResult ViewPositionProfile()
        {
            try
            {
                List<TempPositionProfile> myList = new List<TempPositionProfile>();

                IEnumerable<vRSPEmployeeList> empList = db.vRSPEmployeeLists.Where(e => e.tag == 1 && e.positionCode != null).ToList();

                List<vRSPEmployeeList> results1 = (
                   from p in empList
                   group p by p.positionCode into g
                   select new vRSPEmployeeList()
                   {
                       positionCode = g.Key
                   }
                   ).ToList();

                IEnumerable<tRSPPosition> position = db.tRSPPositions.Where(e => e.isActive == 1).OrderBy(o => o.positionTitle).ToList();
                IEnumerable<tRSPPositionContract> positionContract = db.tRSPPositionContracts.Where(e => e.isActive == 1).OrderBy(o => o.positionTitle).ToList();


                foreach (vRSPEmployeeList item in results1)
                {

                    tRSPPosition posData = position.SingleOrDefault(e => e.positionCode == item.positionCode);

                    if (posData != null)
                    {
                        IEnumerable<vRSPEmployeeList> temp = empList.Where(e => e.positionCode == item.positionCode);
                        int casualM = temp.Where(e => e.sex == "MALE" && e.employmentStatusCode == "05").Count();
                        int casualF = temp.Where(e => e.sex == "FEMALE" && e.employmentStatusCode == "05").Count();
                        int jobM = temp.Where(e => e.sex == "MALE" && e.employmentStatusCode == "06").Count();
                        int JobF = temp.Where(e => e.sex == "FEMALE" && e.employmentStatusCode == "06").Count();

                        myList.Add(new TempPositionProfile()
                        {
                            positionCode = item.positionCode,
                            positionTitle = posData.positionTitle,
                            casualM = casualM,
                            casualF = casualF,
                            casualTotal = casualM + casualF,
                            joM = jobM,
                            joF = JobF,
                            joTotal = jobM + JobF,
                            total = casualM + casualF + jobM + JobF
                        });
                    }
                    else
                    {
                        tRSPPositionContract positionCon = positionContract.SingleOrDefault(e => e.positionCode == item.positionCode);

                        if (positionCon != null)
                        {
                            IEnumerable<vRSPEmployeeList> temp = empList.Where(e => e.positionCode == item.positionCode);

                            int cosM = temp.Where(e => e.sex == "MALE" && e.employmentStatusCode == "07").Count();
                            int cosF = temp.Where(e => e.sex == "FEMALE" && e.employmentStatusCode == "07").Count();
                            int honM = temp.Where(e => e.sex == "MALE" && e.employmentStatusCode == "08").Count();
                            int honF = temp.Where(e => e.sex == "FEMALE" && e.employmentStatusCode == "08").Count();

                            myList.Add(new TempPositionProfile()
                            {
                                positionCode = item.positionCode,
                                positionTitle = positionCon.positionDesc + " ****",
                                casualM = 0,
                                casualF = 0,
                                casualTotal = 0 + 0,
                                joM = 0,
                                joF = 0,
                                joTotal = 0 + 0,
                                cosM = cosM,
                                cosF = cosF,
                                cosTotal = cosM + cosF,
                                honM = honM,
                                honF = honF,
                                honTotal = honM + honF,
                                total = cosM + cosF + honM + honF
                            });
                        }
                    }
                }




                myList = myList.Where(e => e.total > 0).OrderBy(o => o.positionTitle).ToList();


                return Json(new { status = "success", list = myList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }





        public ActionResult WorkGroup()
        {
            return View();
        }

        [HttpPost]
        public JsonResult WorkGroupInit()
        {
            var myList = (from e in db.tRSPWorkGroups
                          select new
                          {
                              e.workGroupCode,
                              e.workGroupName,
                              e.orderNo
                          }).OrderBy(o => o.orderNo).ToList();
            return Json(new { status = "success", workGroup = myList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetWorkGroupEmployee(string code, string id)
        {
            try
            {
                var myList = (from e in db.vRSPEmployeeLists
                              where e.workGroupCode == code && e.employmentStatusCode == id
                              select new
                              {
                                  e.fullNameLast,
                                  e.positionTitle,
                                  e.projectName,
                                  e.employmentStatusCode
                              }).OrderBy(o => o.fullNameLast).ToList();


                Session["ReportType"] = "EMPOFFICEWORKGROUP";
                Session["PrintReport"] = code + "|" + id;

                return Json(new { status = "success", employee = myList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        //EMPLOYEE POSTING DATA

        [Authorize(Roles = "APRD")]
        public ActionResult EmployeePosting()
        {

            return View();
        }


        public JsonResult SelectPostingPosition(string id)
        {
            vRSPPlantilla p = db.vRSPPlantillas.Single(e => e.plantillaCode == id);
            IEnumerable<tRSPEmployee> empList = db.tRSPEmployees.OrderBy(e => e.fullNameLast).ToList();

            PlantillaPosition pos = new PlantillaPosition();
            pos.plantillaCode = p.plantillaCode;
            pos.itemNo = Convert.ToInt16(p.itemNo).ToString("0000");
            pos.positionTitle = p.positionTitle;
            pos.salaryGrade = Convert.ToInt16(p.salaryGrade);
            pos.positionLevelNameShort = p.positionLevelCode;
            pos.positionStatusName = p.positionStatusName;
            pos.fullNameLast = p.fullNameLast;

            List<tRSPEmploymentStatu> empStatList = new List<tRSPEmploymentStatu>();

            if (p.positionStatusCode == "01")
            {
                empStatList.Add(new tRSPEmploymentStatu()
                {
                    employmentStatusCode = "01",
                    employmentStatus = "Elective"
                });
            }
            else if (p.positionStatusCode == "02")
            {
                empStatList.Add(new tRSPEmploymentStatu()
                {
                    employmentStatusCode = "02",
                    employmentStatus = "Co-Terminous"
                });
            }
            else if (p.positionStatusCode == "03")
            {
                empStatList.Add(new tRSPEmploymentStatu()
                {
                    employmentStatusCode = "03",
                    employmentStatus = "Permanent"
                });
                empStatList.Add(new tRSPEmploymentStatu()
                {
                    employmentStatusCode = "04",
                    employmentStatus = "Temporary"
                });
            }

            //return Json(new { status = "success", plantilla = pos, employeeList = empList, empStatList = empStatList }, JsonRequestBehavior.AllowGet);


            var jsonResult = Json(new { status = "success", plantilla = pos, employeeList = empList, empStatList = empStatList }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }

        public class TempEmployeePosting
        {
            public string plantillaCode { get; set; }
            public string EIC { get; set; }
            public string empStatusCode { get; set; }
            public int step { get; set; }
            public DateTime effectiveDate { get; set; }
            public DateTime dateLastProm { get; set; }
            public DateTime dateOrigAppt { get; set; }
            public string eligibilityCode { get; set; }

        }

        public JsonResult SaveEmployeePosting(TempEmployeePosting data)
        {

            vRSPPlantilla pos = db.vRSPPlantillas.Single(e => e.plantillaCode == data.plantillaCode);

            if (pos.EIC == null)
            {
                string sgCode = GetSalayDetailCode(Convert.ToInt16(pos.salaryGrade), Convert.ToInt16(data.step));


                DateTime dt = DateTime.Now;
                string uEIC = Session["_EIC"].ToString();
                string appCode = "T" + dt.ToString("yyMMddHHmm") + "APPT0" + uEIC.Substring(0, 4) + dt.ToString("ssfff");

                vRSPPlantilla plan = db.vRSPPlantillas.Single(e => e.plantillaCode == data.plantillaCode);

                tRSPEmployee emp = db.tRSPEmployees.Single(e => e.EIC == data.EIC);
                emp.dateLastPromoted = Convert.ToDateTime(data.dateLastProm);
                emp.dateOrigAppointment = Convert.ToDateTime(data.dateOrigAppt);


                //ADD TABLE tRSPAppointment
                tRSPAppointment app = new tRSPAppointment();
                app.appointmentCode = appCode;
                app.itemNo = plan.itemNo;
                app.positionTitle = plan.positionTitle;
                app.salaryGrade = plan.salaryGrade;
                app.step = plan.step;
                app.employmentStatusCode = data.empStatusCode;
                app.rateMonth = plan.rateMonth;
                app.rateMonthText = NumWordsWrapper(Convert.ToDouble(plan.rateMonth));
                app.effectivityDate = Convert.ToDateTime(data.effectiveDate);
                app.eligibilityCode = data.eligibilityCode;
                app.appNatureCode = "";
                app.viceOf = "";
                app.viceStatus = "";
                app.pageNo = 0;
                app.remarks = "SysINI";
                db.tRSPAppointments.Add(app);

                //ADD TABLE
                tRSPPlantillaPersonnel p = new tRSPPlantillaPersonnel();
                p.transCode = appCode;
                p.appointmenCode = appCode;
                p.plantillaCode = data.plantillaCode;
                p.EIC = data.EIC;
                p.salaryDetailCode = sgCode;
                p.effectiveDate = Convert.ToDateTime(data.effectiveDate);

                db.tRSPPlantillaPersonnels.Add(p);
                db.SaveChanges();

                var id = pos.departmentCode;
                return Json(new { status = "success", code = id }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "failed!" }, JsonRequestBehavior.AllowGet);
            }

            //vRSPPlantillaPosition p = db.vRSPPlantillaPositions.Single(e => e.plantillaCode == data.plantillaCode);

            //Is valid plantilla
            //Add Validation if position is vacant
            //

            //tServiceRecord sr = new tServiceRecord();
            //sr.EIC = data.EIC;
            //sr.dateFrom = Convert.ToDateTime(data.dateFrom);
            //sr.dateToText = "Present";
            //sr.designation = p.positionTitle;
            //sr.empStatusCode = sr.empStatusCode;
            //sr.plantillaCode = data.plantillaCode;
            //sr.step = data.step;
            //sr.isSysGen = 1;
            //sr.serviceTag = 1;
            //db.tServiceRecords.Add(sr);
            //db.SaveChanges();

        }

        private string GetSalayDetailCode(int sg, int step)
        {
            vSalaryTable st = db.vSalaryTables.Single(e => e.salaryGrade == sg && e.step == step);
            return st.salaryDetailCode;
        }


        //[HttpPost]
        // public JsonResult EmployeePostingData()
        // {
        //     IEnumerable<tRSPPosition> pos = db.tRSPPositions.OrderBy(e => e.positionTitle).ToList();

        //     string deptCode = "OC191015134332380001";
        //     tOrgDepartment dept = db.tOrgDepartments.Single(e => e.departmentCode == deptCode);

        //     IEnumerable<vOrgStructure> nodeList = db.vOrgStructures.Where(e => e.departmentCode == deptCode).OrderBy(e => e.orderNo).ToList();
        //     //IEnumerable<vRSPPlantillaPosition> positionList = db.vRSPPlantillaPositions.Where(e => e.departmentCode == deptCode).ToList();
        //     IEnumerable<vRSPPlantilla> positionList = db.vRSPPlantillas.Where(e => e.departmentCode == deptCode).ToList();
        //     positionList = positionList.Where(e => e.isFunded == true).ToList();

        //     vOrgStructure nod = nodeList.Single(e => e.strucNo == 0);
        //     string strucName = structName(nod);
        //     string parentPath = parentAddress(nod);

        //     myParentList.Add(new StructureParentViewModel()
        //     {
        //         structureCode = nod.structureCode,
        //         structureName = strucName,
        //         structurePath = parentPath + "\\" + strucName,
        //         parentPath = parentPath,
        //         parentNo = 0,
        //         levelNo = 0,
        //         orderNo = Convert.ToInt16(nod.strucNo),
        //         positionList = PositionList(nod.structureCode, positionList).ToList()
        //     });

        //     SearchMyChild(nod.structureCode, 0, nodeList, positionList);
        //     IEnumerable<StructureParentViewModel> parentList = myParentList.OrderBy(o => o.orderNo).ToList();

        //     IEnumerable<tRSPEligibility> elig = db.tRSPEligibilities.Where(e => e.isActive == 1).OrderBy(o => o.orderNo).ThenBy(o => o.eligibilityName).ToList();

        //     var deptList = db.tOrgDepartments.OrderBy(e => e.orderNo).Select(s => new { s.departmentCode, s.departmentName }).ToList();

        //     return Json(new { status = "success", parentList = parentList, eligList = elig, deptList = deptList }, JsonRequestBehavior.AllowGet);
        // }


        static String NumWordsWrapper(double n)
        {
            string words = "";
            double intPart;
            double decPart = 0;
            if (n == 0)
                return "zero";
            try
            {
                string[] splitter = n.ToString().Split('.');
                intPart = double.Parse(splitter[0]);
                decPart = double.Parse(splitter[1]);
            }
            catch
            {
                intPart = n;
            }

            words = NumWords(intPart);

            if (decPart > 0)
            {
                if (words != "")
                    words += " and ";
                int counter = decPart.ToString().Length;
                switch (counter)
                {
                    case 1: words += NumWords(decPart) + " tenths"; break;
                    case 2: words += NumWords(decPart) + " hundredths"; break;
                    case 3: words += NumWords(decPart) + " thousandths"; break;
                    case 4: words += NumWords(decPart) + " ten-thousandths"; break;
                    case 5: words += NumWords(decPart) + " hundred-thousandths"; break;
                    case 6: words += NumWords(decPart) + " millionths"; break;
                    case 7: words += NumWords(decPart) + " ten-millionths"; break;
                }
            }
            return words;
        }


        static String NumWords(double n) //converts double to words
        {
            string[] numbersArr = new string[] { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
            string[] tensArr = new string[] { "twenty", "thirty", "fourty", "fifty", "sixty", "seventy", "eighty", "ninty" };
            string[] suffixesArr = new string[] { "thousand", "million", "billion", "trillion", "quadrillion", "quintillion", "sextillion", "septillion", "octillion", "nonillion", "decillion", "undecillion", "duodecillion", "tredecillion", "Quattuordecillion", "Quindecillion", "Sexdecillion", "Septdecillion", "Octodecillion", "Novemdecillion", "Vigintillion" };
            string words = "";

            bool tens = false;

            if (n < 0)
            {
                words += "negative ";
                n *= -1;
            }

            int power = (suffixesArr.Length + 1) * 3;

            while (power > 3)
            {
                double pow = Math.Pow(10, power);
                if (n >= pow)
                {
                    if (n % pow > 0)
                    {
                        words += NumWords(Math.Floor(n / pow)) + " " + suffixesArr[(power / 3) - 1] + ", ";
                    }
                    else if (n % pow == 0)
                    {
                        words += NumWords(Math.Floor(n / pow)) + " " + suffixesArr[(power / 3) - 1];
                    }
                    n %= pow;
                }
                power -= 3;
            }
            if (n >= 1000)
            {
                if (n % 1000 > 0) words += NumWords(Math.Floor(n / 1000)) + " thousand, ";
                else words += NumWords(Math.Floor(n / 1000)) + " thousand";
                n %= 1000;
            }
            if (0 <= n && n <= 999)
            {
                if ((int)n / 100 > 0)
                {
                    words += NumWords(Math.Floor(n / 100)) + " hundred";
                    n %= 100;
                }
                if ((int)n / 10 > 1)
                {
                    if (words != "")
                        words += " ";
                    words += tensArr[(int)n / 10 - 2];
                    tens = true;
                    n %= 10;
                }

                if (n < 20 && n > 0)
                {
                    if (words != "" && tens == false)
                        words += " ";
                    words += (tens ? "-" + numbersArr[(int)n - 1] : numbersArr[(int)n - 1]);
                    n -= Math.Floor(n);
                }
            }
            return words;
        }

        [HttpPost]
        public JsonResult StepNextYear()
        {
            DateTime dt = Convert.ToDateTime("2022-02-01");
            var stepList = db.fnGetStepIncInYear(dt).ToList();
            List<tRSPNOSIPropBudYear> pbNOSI = new List<tRSPNOSIPropBudYear>();
            string rem = "";

            foreach (var item in stepList)
            {
                int sg = Convert.ToInt16(item.salaryGrade);
                int stepInc = Convert.ToInt16(item.step) + 1;
                vSalaryTable sal = GetSalaryTable(sg, stepInc);
                decimal myRate = Convert.ToDecimal(item.rateMonth);
                string stra = Convert.ToDateTime(item.effectiveDate).ToString("MM");
                string strb = Convert.ToDateTime(item.effectiveDate).ToString("dd");

                decimal monthlyInc = Convert.ToDecimal(sal.rateMonth - myRate);
                int monthCount = (12 - Convert.ToDateTime(item.effectiveDate).Month) + 1;
                decimal totInc = monthlyInc * monthCount;

                decimal propBudget = (myRate * 12) + totInc;
                rem = stra + strb + "-" + totInc.ToString("########0");
                tRSPNOSIPropBudYear n = new tRSPNOSIPropBudYear();
                n.EIC = item.EIC;
                n.year = dt.Year;
                n.propBudgetYear = propBudget;
                n.effectiveDate = Convert.ToDateTime(item.effectiveDate);
                n.step = item.step + 1;
                n.remarks = rem;
                //db.tRSPNOSIPropBudYears.Add(n);
                rem = ""; stra = ""; strb = "";
            }

            //db.SaveChanges();
            return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
        }


        private vSalaryTable GetSalaryTable(int sg, int step)
        {
            vSalaryTable sgItem = db.vSalaryTables.Single(e => e.salaryGrade == sg && e.step == step);
            return sgItem;
        }


           [Authorize(Roles = "APRD,PBOSTAFF")]
        public ActionResult PersonnelServices()
        {

            return View();
        }

           [Authorize(Roles = "APRD,PBOSTAFF")]
        [HttpPost]
        public JsonResult PSInitData()
        {
            try
            {

                IEnumerable<tRSPSalaryTable> salaryTable = db.tRSPSalaryTables.Where(e => e.tag >= 1).ToList();
                IEnumerable<tRSPRefFundSource> fundSource = db.tRSPRefFundSources.Where(e => e.isActive == 1).ToList();
                IEnumerable<tRSPPersonnelServData> comptList = db.tRSPPersonnelServDatas.Where(e => e.tag == 1).ToList();

                IEnumerable<tRSPPosition> positions = db.tRSPPositions.Where(e => e.isActive == 1).OrderBy(o => o.positionTitle).ToList();


                return Json(new { status = "success", salaryTable = salaryTable, fundSource = fundSource, comptList = comptList, positions = positions }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

           [Authorize(Roles = "APRD,PBOSTAFF")]
        [HttpPost]
        public JsonResult EmployeeListByCode(string id)
        {
            try
            {
                IEnumerable<tRSPPersonnelService> ps = db.tRSPPersonnelServices.Where(e => e.PSCode == id && e.tag >= 1).OrderBy(o => o.fullNameLast).ToList();
                return Json(new { status = "success", personnelService = ps }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

           [Authorize(Roles = "APRD,PBOSTAFF")]
        [HttpPost]
        public JsonResult AddComputationData(string name)
        {
            try
            {
                tRSPPersonnelServData data = new tRSPPersonnelServData();
                data.PSCode = "T" + DateTime.Now.ToString("yyMMddHHmmssfff");
                data.psName = name;
                data.userEIC = Session["_EIC"].ToString();
                data.transDT = DateTime.Now;
                data.tag = 1;
                db.tRSPPersonnelServDatas.Add(data);
                db.SaveChanges();

                IEnumerable<tRSPPersonnelServData> comptList = db.tRSPPersonnelServDatas.Where(e => e.tag == 1).ToList();

                return Json(new { status = "success", comptList = comptList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        public class TemptData
        {

            public string name { get; set; }
            public string positionCode { get; set; }
            public int positionCount { get; set; }
            public string psCode { get; set; }
            public string fundSourceCode { get; set; }
            public string salaryCode { get; set; }
            public string employmentStatusCode { get; set; }
            public DateTime periodFrom { get; set; }
            public DateTime periodTo { get; set; }
        }

           [Authorize(Roles = "APRD,PBOSTAFF")]
        [HttpPost]
        public JsonResult AddPosition(TemptData data)
        {
            try
            {
                RSPFundSourceController myPS = new RSPFundSourceController();
                tRSPZPersonnelService p = new tRSPZPersonnelService();
                tRSPPosition position = new tRSPPosition();

                IEnumerable<tRSPSalaryTableDetail> salaryTable = db.tRSPSalaryTableDetails.Where(e => e.salaryCode == data.salaryCode).ToList();

                tRSPSalaryTableDetail salary = new tRSPSalaryTableDetail();

                if (data.employmentStatusCode == "05" || data.employmentStatusCode == "03")
                {
                    position = db.tRSPPositions.Single(e => e.positionCode == data.positionCode);
                    int salaryGrade = Convert.ToInt16(position.salaryGrade);
                    salary = salaryTable.Single(e => e.salaryGrade == salaryGrade && e.step == 1);
                    p = myPS.GetCasualPS(Convert.ToDecimal(salary.rateMonth), salaryGrade, 0, data.periodFrom, data.periodTo, data.employmentStatusCode);
                }
                else if (data.employmentStatusCode == "06")
                {
                    position = db.tRSPPositions.Single(e => e.positionCode == data.positionCode);
                    int salaryGrade = Convert.ToInt16(position.salaryGrade);
                    salary = salaryTable.Single(e => e.salaryGrade == salaryGrade && e.step == 1);
                    p = myPS.GetJobOrderPS(Convert.ToDecimal(salary.rateMonth), data.periodFrom, data.periodTo);
                }
                else if (data.employmentStatusCode == "07" || data.employmentStatusCode == "08")
                {
                    tRSPPositionContract posContract = db.tRSPPositionContracts.Single(e => e.positionCode == data.positionCode);
                    p = myPS.GetJobContract(Convert.ToDecimal(posContract.salary), data.periodFrom, data.periodTo);
                }

                tRSPPersonnelServData psComp = db.tRSPPersonnelServDatas.Single(e => e.PSCode == data.psCode);

                tRSPPersonnelService ps = new tRSPPersonnelService();
                ps.PSCode = data.psCode;
                ps.fullNameLast = data.name;
                ps.positionTitle = position.positionTitle;
                ps.rateMonth = salary.rateMonth;
                ps.salaryDetailCode = salary.salaryDetailCode;
                ps.salaryGrade = salary.salaryGrade;
                ps.step = 1;
                ps.employmentStatusCode = data.employmentStatusCode;
                ps.periodFrom = data.periodFrom;
                ps.periodTo = data.periodTo;
                ps.annual = p.annualRate;
                ps.PERA = p.PERA;
                ps.leaveEarned = p.leaveEarned;
                ps.hazard = p.hazardPay;
                ps.subsistence = p.subsistence;
                ps.laundry = p.laundry;
                ps.midYear = p.midYearBonus;
                ps.yearEnd = p.yearEndBonus;
                ps.cashGift = p.cashGift;
                ps.loyalty = 0;
                ps.clothing = p.clothing;
                ps.lifeRetmnt = p.lifeRetmnt;
                ps.ECC = p.ECC;
                ps.HDMF = p.hdmf;
                ps.PHIC = p.phic;
                ps.totalPS = p.total;
                ps.monthCount = p.recNo;
                ps.hazardCode = 0;
                ps.tag = 1;
                ps.stat = "ACTIVE";
                db.tRSPPersonnelServices.Add(ps);

                //NEW LIST
                IEnumerable<tRSPPersonnelService> newList = db.tRSPPersonnelServices.Where(e => e.PSCode == data.psCode && e.tag >= 1).OrderBy(o => o.fullNameLast).ToList();
                //UPDATE TOTAL
                psComp.totalPS = newList.Sum(e => e.totalPS);

                db.SaveChanges();

                return Json(new { status = "success", personnelService = newList }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult GeneratePSList(TemptData data)
        {
            try
            {
                IEnumerable<vRSPEmployeeList> employee = db.vRSPEmployeeLists.Where(e => e.fundSourceCode == data.fundSourceCode && e.employmentStatusCode == data.employmentStatusCode).OrderBy(o => o.fullNameLast).ToList();

                IEnumerable<tRSPSalaryTableDetail> salaryTable = db.tRSPSalaryTableDetails.Where(e => e.salaryCode == data.salaryCode).ToList();

                List<vRSPEmployeeList> myList = new List<vRSPEmployeeList>();

                foreach (vRSPEmployeeList item in employee)
                {
                    tRSPSalaryTableDetail salary = salaryTable.FirstOrDefault(e => e.salaryGrade == item.salaryGrade && e.step == 1);
                    myList.Add(new vRSPEmployeeList()
                    {
                        fullNameLast = item.fullNameLast,
                        positionTitle = item.positionTitle,
                        salaryGrade = item.salaryGrade,
                        step = item.step,
                        salaryDetailCode = salary.salaryDetailCode,
                        salaryRate = salary.rateMonth,
                        salaryType = "M",
                        employmentStatusTag = item.employmentStatusTag
                    });
                }
                Session["_TempDATA"] = myList.ToList();
                return Json(new { status = "success", employee = myList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        // public JsonResult SavePSCompData(List<vRSPEmployeeList> employee, TemptData data)
        [HttpPost]
        public JsonResult SavePSCompData(TemptData data)
        {
            try
            {

                var tempData = Session["_TempDATA"];
                List<vRSPEmployeeList> employee = tempData as List<vRSPEmployeeList>;

                tRSPPersonnelServData psComp = db.tRSPPersonnelServDatas.Single(e => e.PSCode == data.psCode);
                
                decimal newSum = 0;

                foreach (vRSPEmployeeList item in employee)
                {
                    RSPFundSourceController myPS = new RSPFundSourceController();
                    tRSPZPersonnelService p = new tRSPZPersonnelService();
                    if (data.employmentStatusCode == "05")
                    {
                        p = myPS.GetCasualPS(Convert.ToDecimal(item.salaryRate), Convert.ToInt16(item.salaryGrade), 0, data.periodFrom, data.periodTo, data.employmentStatusCode);
                    }
                    else if (data.employmentStatusCode == "06")
                    {
                        p = myPS.GetJobOrderPS(Convert.ToDecimal(item.salaryRate), data.periodFrom, data.periodTo);
                    }
                    else if (data.employmentStatusCode == "07" || data.employmentStatusCode == "08")
                    {
                        p = myPS.GetJobOrderPS(Convert.ToDecimal(item.salaryRate), data.periodFrom, data.periodTo);
                    }
                    
                    tRSPPersonnelService ps = new tRSPPersonnelService();
                    ps.PSCode = data.psCode;
                    ps.fullNameLast = item.fullNameLast;
                    ps.positionTitle = item.positionTitle;
                    ps.salaryDetailCode = item.salaryDetailCode;
                    ps.salaryGrade = item.salaryGrade;
                    ps.rateMonth = p.monthlyRate;
                    ps.step = item.step;
                    ps.employmentStatusCode = data.employmentStatusCode;
                    ps.periodFrom = data.periodFrom;
                    ps.periodTo = data.periodTo;
                    ps.annual = p.annualRate;
                    ps.PERA = p.PERA;
                    ps.leaveEarned = p.leaveEarned;
                    ps.hazard = p.hazardPay;
                    ps.subsistence = p.subsistence;
                    ps.laundry = p.laundry;
                    ps.midYear = p.midYearBonus;
                    ps.yearEnd = p.yearEndBonus;
                    ps.cashGift = p.cashGift;
                    ps.loyalty = 0;
                    ps.clothing = p.clothing;
                    ps.lifeRetmnt = p.lifeRetmnt;
                    ps.ECC = p.ECC;
                    ps.HDMF = p.hdmf;
                    ps.PHIC = p.phic;
                    ps.totalPS = p.total;
                    ps.monthCount = p.recNo;
                    ps.hazardCode = 0;
                    ps.tag = 1;
                    ps.stat = "ACTIVE";
                    db.tRSPPersonnelServices.Add(ps);
                    newSum = newSum + Convert.ToDecimal(p.total);
                }

                //UPDATE TOTAL
                psComp.totalPS = newSum;
                //COMIT;
                db.SaveChanges();
                IEnumerable<tRSPPersonnelService> newList = db.tRSPPersonnelServices.Where(e => e.PSCode == data.psCode && e.tag >= 1).OrderBy(o => o.fullNameLast).ToList();
                return Json(new { status = "success", personnelService = newList }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        //PRINT PS MATRIX
        [HttpPost]
        public JsonResult PrintPS(string code)
        {
            Session["ReportType"] = "PSMATRIX";
            Session["PrintReport"] = code;
            return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
        }
        
        public class TempEditData
        {
            public int id { get; set; }
            public string PSCode { get; set; }
            public string fullNameTitle { get; set; }
            public string positionTitle { get; set; }
            public DateTime periodFrom { get; set; }
            public DateTime periodTo { get; set; }
            public int? hazardCode { get; set; }
            public string status { get; set; }
        }

        [HttpPost]
        public JsonResult EditPSItem(tRSPPersonnelService data)
        {
            try
            {
                tRSPPersonnelService psItem = db.tRSPPersonnelServices.SingleOrDefault(e => e.PSCode == data.PSCode && e.recNo == data.recNo);

                if (psItem != null)
                {

                    TempEditData myData = new TempEditData();
                    myData.id = psItem.recNo;
                    myData.PSCode = psItem.PSCode;
                    myData.periodFrom = psItem.periodFrom;
                    myData.periodTo = Convert.ToDateTime(psItem.periodTo);
                    myData.hazardCode = psItem.hazardCode;
                    myData.fullNameTitle = psItem.fullNameLast;
                    myData.positionTitle = psItem.positionTitle;
                    myData.status = psItem.stat;
                    return Json(new { status = "success", editData = myData }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UpdatePSItem(TempEditData data)
        {
            try
            {
                if (data.periodFrom.Year != data.periodTo.Year)
                {
                    return Json(new { status = "Invalid date" }, JsonRequestBehavior.AllowGet);
                }
                if (data.periodFrom >= data.periodTo)
                {
                    return Json(new { status = "Invalid date" }, JsonRequestBehavior.AllowGet);
                }
                if (data.hazardCode > 2 || data.hazardCode < 0)
                {
                    return Json(new { status = "Invalid data!" }, JsonRequestBehavior.AllowGet);
                }

                tRSPPersonnelService ps = db.tRSPPersonnelServices.SingleOrDefault(e => e.PSCode == data.PSCode && e.recNo == data.id);

                if (ps == null)
                {
                    return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
                }

                RSPFundSourceController myPS = new RSPFundSourceController();
                tRSPZPersonnelService p = new tRSPZPersonnelService();
                if (ps.employmentStatusCode == "05")
                {
                    p = myPS.GetCasualPS(Convert.ToDecimal(ps.rateMonth), Convert.ToInt16(ps.salaryGrade), Convert.ToInt16(data.hazardCode), data.periodFrom, data.periodTo, ps.employmentStatusCode);
                }
                else if (ps.employmentStatusCode == "06")
                {
                    p = myPS.GetJobOrderPS(Convert.ToDecimal(ps.rateMonth), data.periodFrom, data.periodTo);
                }
                else if (ps.employmentStatusCode == "07" || ps.employmentStatusCode == "08")
                {
                    p = myPS.GetJobOrderPS(Convert.ToDecimal(ps.rateMonth), data.periodFrom, data.periodTo);
                }

                ps.periodFrom = data.periodFrom;
                ps.periodTo = data.periodTo;
                ps.annual = p.annualRate;
                ps.PERA = p.PERA;
                ps.leaveEarned = p.leaveEarned;
                ps.hazard = p.hazardPay;
                ps.subsistence = p.subsistence;
                ps.laundry = p.laundry;
                ps.midYear = p.midYearBonus;
                ps.yearEnd = p.yearEndBonus;
                ps.cashGift = p.cashGift;
                ps.loyalty = 0;
                ps.clothing = p.clothing;
                ps.lifeRetmnt = p.lifeRetmnt;
                ps.ECC = p.ECC;
                ps.HDMF = p.hdmf;
                ps.PHIC = p.phic;
                ps.totalPS = p.total;
                ps.monthCount = p.recNo;
                ps.hazardCode = Convert.ToInt16(data.hazardCode);
                ps.stat = data.status;
                db.SaveChanges();

                IEnumerable<tRSPPersonnelService> newList = db.tRSPPersonnelServices.Where(e => e.PSCode == data.PSCode && e.tag >= 0).OrderBy(o => o.fullNameLast).ToList();
                return Json(new { status = "success", personnelService = newList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult DeletePSItem(TempEditData data)
        {
            try
            {
                tRSPPersonnelService psItem = db.tRSPPersonnelServices.SingleOrDefault(e => e.PSCode == data.PSCode && e.recNo == data.id);
                if (psItem != null)
                {
                    psItem.tag = -1;
                    db.SaveChanges();
                    IEnumerable<tRSPPersonnelService> newList = db.tRSPPersonnelServices.Where(e => e.PSCode == data.PSCode && e.tag >= 0).OrderBy(o => o.fullNameLast).ToList();
                    return Json(new { status = "success", personnelService = newList }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        //END OF CODE LINE

    }
}