using DDNHRIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DDNHRIS.Controllers
{

    [SessionTimeout]
    [Authorize(Roles = "APRD, PACCOSTAFF")]

    public class RSPReportController : Controller
    {
        HRISDBEntities db = new HRISDBEntities();
        // GET: RSPReport
        public ActionResult List()
        {
            return View();
        }

        public ActionResult AppointmentIssued()
        {
            return View();
        }

        [HttpPost]
        public JsonResult AppointmentIssuedList()
        {
            try
            {
                string uEIC = Session["_EIC"].ToString();


                string tmpTo = DateTime.Now.ToString("MMMM") + " 1, " + DateTime.Now.Year.ToString();
                DateTime dateTo = Convert.ToDateTime(tmpTo).AddDays(-1);

                string tmpFrom = dateTo.ToString("MMMM") + " 1, " + dateTo.Year.ToString();
                DateTime dateFrom = Convert.ToDateTime(tmpFrom);
                IEnumerable<vRSPAppointmentNPEmployee> casualList = db.vRSPAppointmentNPEmployees.Where(e => e.dateIssued >= dateFrom && e.dateIssued <= dateTo && e.tag == 2).ToList();


                IEnumerable<vRSPAppointmentNonPlantilla> appointment = db.vRSPAppointmentNonPlantillas.Where(e => e.tag == 2 && e.employmentStatusCode == "05" && e.userEIC == uEIC).ToList();

                var apptList = db.vRSPAppointmentNonPlantillas.Select(e => new
                {
                    e.appointmentCode,
                    e.appointmentName,
                    e.projectName,
                    e.employmentStatus,
                    e.employmentStatusCode,
                    e.tag,
                    e.userEIC
                }).Where(e => e.tag == 1 && e.employmentStatusCode == "05" && e.userEIC == uEIC).OrderBy(e => e.appointmentName).ToList();


                //string batchCode = dateFrom.Year + "" + dateFrom.Month;


                //IEnumerable<tRSPRPTRAIData> raiList = db.tRSPRPTRAIDatas.Where(e => e.batchCode == batchCode).ToList();

                //List<tRSPRPTRAIData> myList = new List<tRSPRPTRAIData>();

                //foreach (vRSPAppointmentNPEmployee item in casualList)
                //{

                //    int tmpPositionLevel = Convert.ToInt16(item.positionLevel);
                //    if (item.salaryGrade >= 25)
                //    { tmpPositionLevel = 3; }

                //    int count = raiList.Where(e => e.appointmentItemCode == item.appointmentItemCode).Count();

                //    if (count == 0)
                //    {
                //        myList.Add(new tRSPRPTRAIData()
                //        {
                //            EIC = item.EIC,
                //            appointmentItemCode = item.appointmentItemCode,
                //            lastName = item.lastName,
                //            firstName = item.firstName,
                //            middleName = item.middleName,
                //            extnName = item.extName,
                //            itemNo = "N/A",
                //            salaryGrade = "SG " + item.salaryGrade + "/1",
                //            salaryRate = item.salary * 22,
                //            period = Convert.ToDateTime(item.periodFrom).ToString("MM/dd/yyyy") + " - " + Convert.ToDateTime(item.periodTo).ToString("MM/dd/yyyy"),
                //            natureOfAppointment = item.appNatureName,
                //            employmentStatus = "CASUAL",
                //            positionTitle = item.positionTitle,
                //            positionLevel = tmpPositionLevel, // 1:FIRST, 2:SECOND, 3:MANAGERIAL
                //            dateIssued = item.dateIssued
                //        });
                //    }

                //}

                //myList = myList.OrderBy(o => o.lastName).ThenBy(o => o.firstName).ToList();

                //Session["RSPREPORT"] = myList;

                return Json(new { status = "success", appointmentList = apptList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult RAIBatchList()
        {
            try
            {

                var myList = db.tRSPRPTRAIs.Select(e => new
                {
                    e.RAICode,
                    e.RAIName,
                    e.itemCount,
                    e.transDT
                }).OrderBy(e => e.RAICode).ToList();

                return Json(new { status = "success", list = myList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult PrintRAI(string id)
        {
            try
            {
                tRSPRPTRAI rai = db.tRSPRPTRAIs.SingleOrDefault(e => e.RAICode == id);
                if (rai != null)
                {
                    Session["ReportType"] = "RAI";
                    Session["PrintReport"] = id;
                    return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //ADD -HACKERALERT-
                    return Json(new { status = "HACKER ALERT!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ReportByFundSource(string id)
        {
            try
            {
                tRSPRefFundSource fs = db.tRSPRefFundSources.SingleOrDefault(e => e.fundSourceCode == id);
                if (fs != null)
                {
                    Session["ReportType"] = "LISTBYFUNDSOURCE";
                    Session["PrintReport"] = id;
                    return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //ADD -HACKERALERT-
                    return Json(new { status = "--HACKER ALERT--" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        public class TempMonthlyPS
        {
            public string EIC { get; set; }
            public int month { get; set; }
            public string positionTitle { get; set; }
            public decimal dailyRate { get; set; }
            public decimal monthlyRate { get; set; }
            public decimal PERA { get; set; }
            public decimal leaveEarned { get; set; }
            public decimal hazard { get; set; }
            public decimal subsistence { get; set; }
            public decimal laundry { get; set; }
            public decimal cashGift { get; set; }
            public decimal lifeRetmnt { get; set; }
            public decimal ECC { get; set; }
            public decimal hdmf { get; set; }
            public decimal PHIC { get; set; }
            public decimal clothing { get; set; }
            public decimal midYearBonus { get; set; }
            public decimal yearEndBonus { get; set; }
            public decimal total { get; set; }
            public string remarks { get; set; }
        }

        [HttpPost]
        public JsonResult ReportByFundSourcePS(string id)
        {
            string appCode = "";
            string checkCode = "";
            try
            {
               
                string userId = Session["_EIC"].ToString();
                IEnumerable<vRSPAppointmentNPEmployee> empList = db.vRSPAppointmentNPEmployees.Where(e => e.fundSourceCode == id && e.PSTag == 1).OrderBy(o => o.employmentStatusCode).ThenBy(o => o.periodFrom).ThenBy(o => o.EIC).ToList();

                List<TempMonthlyPS> mps = new List<TempMonthlyPS>();

                IEnumerable<vRSPSalarySchedCasual> schedList = db.vRSPSalarySchedCasuals.Where(e => e.step == 1);
                 
                var results = empList.GroupBy(p => p.EIC, (key) => new { EIC = key });
                string tmpRemarks = "";
                string tmpRun = "";
                foreach (var itm in results)
                {
                    string tmpEIC = itm.Key;
                    List<vRSPAppointmentNPEmployee> myList = empList.Where(e => e.EIC == tmpEIC).ToList();

                    TempMonthlyPS[] monthList = new TempMonthlyPS[13];
                    decimal monthRate = 0;
                    tmpRemarks = "";
                    tmpRun = "";

                 

                    foreach (vRSPAppointmentNPEmployee item in myList)
                    {

                        appCode = item.appointmentCode;
                        checkCode = item.appointmentItemCode;
                         
                        DateTime df = Convert.ToDateTime(item.periodFrom);
                        DateTime dt = Convert.ToDateTime(item.periodTo);

                        if (item.positionTitle != tmpRun)
                        {
                            tmpRun = item.positionTitle;
                            tmpRemarks = tmpRemarks + item.positionTitle + ";";
                        }
                                                                       
                        int from = df.Month;
                        int dto = dt.Month;

                        monthRate = Convert.ToDecimal(item.salary);

                        TempMonthlyPS monthPS = new TempMonthlyPS();
                        decimal clothing = 0;
                        if (item.employmentStatusCode == "05")
                        {
                            tRSPPosition position = db.tRSPPositions.Single(e => e.positionCode == item.positionCode);
                            vRSPSalarySchedCasual sched = schedList.FirstOrDefault(e => e.salaryGrade == item.salaryGrade);
                            monthPS = GetMonthlyRate(sched, position, 0);
                            monthRate = monthPS.monthlyRate;
                            clothing = 6000;

                        }
                        else if (item.employmentStatusCode == "06")
                        {
                            monthPS.monthlyRate = Convert.ToInt16(item.salary) * 22 ;
                        }
                        else
                        {
                            monthPS.monthlyRate = Convert.ToInt16(item.salary);
                        } 

                        for (int i = from; i <= dto; i++)
                        {       
                            TempMonthlyPS newMP = new TempMonthlyPS();
                            newMP = monthPS;
                            newMP.clothing = clothing;
                            newMP.EIC = item.EIC;
                            newMP.monthlyRate = monthPS.monthlyRate;
                            newMP.month = i;
                            newMP.remarks = tmpRemarks;
                            monthList[i] = newMP;
                            
                            clothing = 0;
                        }
                    }

                    //SAVE DATA
                    for (int i = 1; i < monthList.Length; i++)
                    {
                        TempMonthlyPS item = monthList[i];
                        if (item != null)
                        {
                            item.remarks = tmpRemarks;
                            mps.Add(item);
                        }
                    }                      
                }

                string tmpCode = "T" + DateTime.Now.ToString("yyMMddHHmmssfff") + userId.Substring(0, 4);

                IEnumerable<vRSPZPersonnelService> tempPS = db.vRSPZPersonnelServices.Where(e => e.fundSourceCode == id).ToList();

                 
                List<tRSPZPSByCharge> ps = new List<tRSPZPSByCharge>();                  
                foreach (var itm in results)
                {
                    string tmpEIC = itm.Key;

                    decimal midYear = 0;
                    decimal yearEnd = 0;
                    decimal cashGift = 0;
                    decimal loyal = 0;
                    decimal clothing = 0;


                    List<TempMonthlyPS> myMP = mps.Where(e => e.EIC == tmpEIC).ToList();
                    vRSPAppointmentNPEmployee tempData = empList.Where(e => e.EIC == tmpEIC).OrderByDescending(o => o.periodTo).LastOrDefault();


                    string tmpstr = "";

                    IEnumerable<vRSPZPersonnelService> myTempPS = tempPS.Where(e => e.EIC == tmpEIC).ToList();
                    if (myTempPS.Count() > 0)
                    {

                       
                        if(myTempPS.Sum(e => e.midYearBonus) > 0) {
                            midYear = myMP.First().monthlyRate;
                        }
                        if (myTempPS.Sum(e => e.yearEndBonus) > 0)
                        {
                            yearEnd = myMP.First().monthlyRate;
                            cashGift = 5000;
                        }
                        if (myTempPS.Sum(e => e.clothing) > 0)
                        {                           
                            clothing = 6000;
                        }
                    }


                    appCode = tmpEIC;

                    tRSPZPSByCharge n = new tRSPZPSByCharge();
                    n.EIC = tmpEIC;
                    n.monthCount = myMP.Count();
                    n.dailyRate = myMP.First().dailyRate;
                    n.monthlyRate = myMP.First().monthlyRate;
                    n.positionTitle = tempData.positionTitle;
                    n.salaryGrade = tempData.salaryGrade;
                    n.PERA = myMP.Sum(e => e.PERA);
                    n.annualRate = myMP.Sum(e => e.monthlyRate);
                    n.leaveEarned = myMP.Sum(e => e.leaveEarned);
                    n.hazard = myMP.Sum(e => e.hazard);
                    n.subsistence = myMP.Sum(e => e.subsistence);
                    n.laundry = myMP.Sum(e => e.laundry);
                    n.lifeRetmnt = myMP.Sum(e => e.lifeRetmnt);
                    n.ECC = myMP.Sum(e => e.ECC);
                    n.hdmf = myMP.Sum(e => e.hdmf);
                    n.PHIC = myMP.Sum(e => e.PHIC);
                    n.clothing = clothing;
                    
                    n.loyalty = 0;    
                    n.midYearBonus = midYear;
                    n.yearEndBonus = yearEnd;
                    n.cashGift = cashGift;

                    n.total = n.annualRate + n.PERA + n.leaveEarned + n.hazard + n.subsistence + n.laundry + n.lifeRetmnt + n.ECC + n.hdmf + n.PHIC + n.clothing + n.loyalty + n.midYearBonus +n.yearEndBonus + n.cashGift;
                    n.remarks = myMP.First().remarks;

           
                    n.reportCode = tmpCode;
                    n.fundSourceCode = id;
                    db.tRSPZPSByCharges.Add(n);                    
                    db.SaveChanges();
                }
                  
                if (empList != null)
                {
                    Session["ReportType"] = "PSBYFUNDSOURCE";
                    Session["PrintReport"] = tmpCode;
                    return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //ADD -HACKERALERT-
                    return Json(new { status = "--HACKER ALERT--" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                string appItem = checkCode;
                string app = appCode;

                string err = ex.ToString();

                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        private TempMonthlyPS GetMonthlyRate(vRSPSalarySchedCasual salarySched, tRSPPosition position, int hazardCode)
        {
            TempMonthlyPS res = new TempMonthlyPS();

            decimal monthlyRate = Convert.ToDecimal(salarySched.rateMonth);
            decimal dailyRate = Convert.ToDecimal(salarySched.rateDay);

            decimal hazardPay = 0;
            decimal subsistence = 0;
            decimal laundry = 0;
            if (hazardCode == 1)
            {
                //HEALTH SERVICES
                hazardPay = CalQHazardForHealthMonthly(Convert.ToInt16(position.salaryGrade), Convert.ToDecimal(dailyRate));
                //SUBS. & Luandry
                subsistence = 1500;
                laundry = 150;
            }
            else if (hazardCode == 2)
            {
                //SOCIAL WORKER
                hazardPay = CalQHazardForSocialWorkerMonthly(dailyRate);
                //SUBS ONLY
                subsistence = 1500;
            }


            decimal leaveEarned = (monthlyRate / 8);
            decimal lifeAndRetire = (Convert.ToDecimal(monthlyRate) * Convert.ToDecimal(.12));
            decimal PHIC = CalQPHICGovtShare(Convert.ToDouble(monthlyRate), 1);
            decimal PERA = 2000;
            decimal HDMF = 100;
            decimal ECC = 100;

            decimal clothing = 6000;

            decimal total = monthlyRate + PERA + HDMF + lifeAndRetire + ECC + PHIC + laundry + subsistence + hazardPay + clothing;

            res.dailyRate = dailyRate;
            res.monthlyRate = monthlyRate;
            res.PERA = PERA;
            res.hdmf = 100;
            res.ECC = 100;
            res.PHIC = PHIC;
            res.leaveEarned = leaveEarned;
            res.lifeRetmnt = lifeAndRetire;
            res.laundry = laundry;
            res.hazard = hazardPay;
            res.subsistence = subsistence; 
            res.subsistence = subsistence;
            res.clothing = clothing;
            res.total = total;
            return res;

        }


        //
        private decimal CalQPHICGovtShare(double rateMonth, int monthCount)
        {
            double monthlyPrem = 350;
            if (rateMonth >= 10000.01 && rateMonth < 70000)
            {
                monthlyPrem = (rateMonth * .035) / 2;
            }
            else if (rateMonth >= 70000)
            {
                monthlyPrem = 2450 / 2;
            }
            return Convert.ToDecimal(monthlyPrem * monthCount);
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

        [HttpPost]
        public JsonResult VacantPositionSummary(string id)
        {
            try
            {
                Session["ReportType"] = "VACANTPOSITIONSUMM";
                Session["PrintReport"] = id;
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}