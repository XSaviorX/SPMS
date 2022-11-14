using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DDNHRIS.Models;

namespace DDNHRIS.Controllers
{
    [SessionTimeout]
    [Authorize]
    public class DailyTimeRecordController : Controller
    {

        HRISDBEntities db = new HRISDBEntities();

        DateTime dtLogin1, dtLogin2, dtLogout1, dtLogout2;
        string _login1, _logout1, _login2, _logout2;
        List<myLogList> myLog = new List<myLogList>();

        List<myShiftDTR> myShiftLogs = new List<myShiftDTR>();
        DateTime dtShiftLogin, dtShiftLogOut;

        // GET: DailyTimeRecord
        public ActionResult Preparation()
        {
            return View();
        }

        //NG
        [HttpPost]
        public JsonResult PreparationData()
        {
            List<SelectListItem> empList = new List<SelectListItem>();
            empList.Add(new SelectListItem()
            {
                Text = "SAM, DONNIE M.",
                Value = Session["_EIC"].ToString(),
                Selected = true
            });

            empList.Add(new SelectListItem()
            {
                Text = "GEMENTIZA, DANILO T.",
                Value = "DG2081876717DC1DFBC1",
                Selected = false
            });

            empList.Add(new SelectListItem()
            {
                Text = "ROLANDO M. LLANES",
                Value = "RL1623739725F4D4C3E1",
                Selected = false
            });

            empList.Add(new SelectListItem()
            {
                Text = "RICARDO G. NINIEL JR",
                Value = "RN1489802862BED44E3F",
                Selected = false
            });
            empList.Add(new SelectListItem()
            {
                Text = "MAGOS, REY MARK M.",
                Value = "RM7731486493F74A60FF",
                Selected = false
            });


            List<SelectListItem> approvingOfficer = new List<SelectListItem>();
            approvingOfficer.Add(new SelectListItem()
            {
                Text = "JUBAHIB, EDWIN I.",
                Value = "EJA0770564CA4141F2BC"
            });
            approvingOfficer.Add(new SelectListItem()
            {
                Text = "RABANOZ, JOSIE JEAN R.",
                Value = "JR1942559638B931650A"
            });
            approvingOfficer.Add(new SelectListItem()
            {
                Text = "PALERO, EDWIN A.",
                Value = "EP1831954384C6C94D75"
            });
            return Json(new { status = "success", employeeList = empList, approvingOfficerList = approvingOfficer }, JsonRequestBehavior.AllowGet);
        }



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // SHIFTING B
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //NG
        [HttpPost]
        public JsonResult ViewShiftLog(DTRView data)
        {
            string uEIC = Session["_EIC"].ToString();

            tAttEmpScheme scheme = db.tAttEmpSchemes.OrderByDescending(o => o.recNo).FirstOrDefault(e => e.EIC == uEIC);
            tRSPEmployee emp = db.tRSPEmployees.Single(e => e.EIC == data.EIC);

            DateTime periodFrom = DateTime.Now;
            DateTime periodTo = DateTime.Now;
            string tmpMonth = "1 " + data.month;
            DateTime baseMonth = Convert.ToDateTime(tmpMonth);

            if (data.period == 0)
            {
                periodFrom = Convert.ToDateTime(tmpMonth);
                periodTo = periodFrom.AddMonths(1).AddDays(-1);
            }
            else if (data.period == 1)
            {
                string tmpDate = "15 " + data.month;
                periodFrom = Convert.ToDateTime(tmpMonth);
                periodTo = Convert.ToDateTime(tmpDate);
            }
            else if (data.period == 2)
            {
                string tmpDate = "16 " + data.month;
                periodFrom = Convert.ToDateTime(tmpDate);
                DateTime tempDate = Convert.ToDateTime(tmpMonth);
                periodTo = tempDate.AddMonths(1).AddDays(-1);
            }

            int userLogNo = Convert.ToInt16(emp.idNo);
            //int userLogNo = emp.idNo;
            //TEMP SCHEME CODE -> SCHM7AMTO3PM
            IEnumerable<tAttEmpSchemeSchedDetail> sched = db.tAttEmpSchemeSchedDetails.Where(e => e.schemeCode == scheme.schemeCode && e.loginDT >= periodFrom && e.loginDT <= periodTo).OrderBy(o => o.loginDT).ToList();

            tAttEmpSchemeSchedDetail startSched = sched.First();
            tAttEmpSchemeSchedDetail endSched = sched.Last();

            DateTime logStart = startSched.loginDT.Value.AddHours(-1);
            DateTime logEnd = endSched.logoutDT.Value.AddHours(1);

            IEnumerable<tAttLog> logList = db.tAttLogs.Where(e => e.userLogNo == userLogNo && e.logDT >= periodFrom && e.logDT < periodTo).ToList();

            List<myShiftDTR> shiftLog = new List<myShiftDTR>();


            foreach (tAttEmpSchemeSchedDetail s in sched)
            {
              myShiftDTR d =  GenerateShiftLog(emp.EIC, s, logList);
              shiftLog.Add(d);
            }

      
            //JUSTIFICATION
            //myShiftLogs = JustificationJusty(uEIC, myShiftLogs, logStart, logEnd);
            //List<myShiftDTR> shiftLog = new List<myShiftDTR>();

            //IEnumerable<myShiftDTR> myLogs = myShiftLogs.ToList();

            //DateTime tempoMonth = baseMonth.AddMonths(1).AddDays(-1);
            //int maxDay = tempoMonth.Day;

            //for (int i = 1; i <= maxDay; i++)
            //{
            //    DateTime runDate = Convert.ToDateTime(i.ToString() + " " + data.month);

            //    string tempLogin = "";
            //    string tempLogOut = "";
            //    string remarks = "";

            //    int tempTardyA = 0;
            //    int tempTardyB = 0;
            //    int totalTardy = 0;

            //    int underTimeA = 0;
            //    int underTimeB = 0;
            //    int totalUnder = 0;

            //    IEnumerable<myShiftDTR> dayLogs = myShiftLogs.Where(e => e.logDate.Day == i);
                
            //    if (dayLogs.Count() > 0)
            //    {
            //        myShiftDTR logA = dayLogs.First();
            //        tempLogin = logA.logIn;
            //        tempLogOut = logA.logOut;
            //        tempTardyA = logA.tardyA;
            //        underTimeA = logA.underTimeA;
            //        remarks = logA.remarks;
                    
            //        totalTardy = tempTardyA + tempTardyB;
            //        totalUnder = underTimeA + underTimeB;
            //    }

            //    int isWorkingDay = 0;

            //    if (runDate >= periodFrom && runDate <= periodTo)
            //    {
            //        isWorkingDay = 1;
            //        remarks = "";
            //    }


            //    shiftLog.Add(new myShiftDTR()
            //    {
            //        EIC = uEIC,
            //        logNo = runDate.Day,
            //        logDay = runDate.ToString("ddd"),
            //        logDate = runDate,
            //        //dtLogin1 = dtLogin1,
            //        //dtLogout1 = dtLogout1,
            //        //dtLogin2 = dtLogin2,
            //        //dtLogout2 = dtLogout2,
            //        logIn = tempLogin,
            //        logOut = tempLogOut,
            //        isWorkingDay = isWorkingDay,
            //        //tardyA = tempTardyA,
            //        //tardyB = tempTardyB,
            //        totalTardy = totalTardy,
            //        totalUndertime = totalUnder,
            //        remarks = remarks,

            //    });
                
            //}

            return Json(new { status = "success", logList = shiftLog }, JsonRequestBehavior.AllowGet);
        }

        private myShiftDTR GenerateShiftLog(string EIC, tAttEmpSchemeSchedDetail scheme, IEnumerable<tAttLog> logs)
        {

            myShiftDTR myData = new myShiftDTR();

            DateTime dt = Convert.ToDateTime(scheme.loginDT);

            string tmpLogin = ShiftLogIn(logs, scheme, 0);
            string tmpLogOut = ShiftLogOut(logs, scheme, 1);
            
            //CHECK FOR PASS SLIP
            //CHECK FOR PTLOS
            //CHECK FOR JUSTIFICATION

            IEnumerable<tJustifyApp> justyList = db.tJustifyApps.Where(e => e.EIC == EIC && e.schemeDT == scheme.loginDT || e.schemeDT == scheme.logoutDT).ToList();
            
            int logInMinsTardy = 0;
            int logOutMinsTardy = 0;

            decimal tmpTardy = 0;
            if (tmpLogin == "" || tmpLogin == "")
            {
                tmpTardy = Convert.ToDecimal(scheme.schemeHour);
            }
            else
            {
                tmpTardy = 0;
                DateTime loginTime = Convert.ToDateTime(scheme.loginDT);
                DateTime empLogin = Convert.ToDateTime(dtShiftLogin.Date.ToString("yyyy-MM-dd") + " " + dtShiftLogin.ToString("HH:mm"));

                logInMinsTardy = (int)empLogin.Subtract(loginTime).TotalMinutes;
                if (logInMinsTardy < 1)
                {
                    logInMinsTardy = 0;
                }

                DateTime logOutTime = Convert.ToDateTime(scheme.logoutDT);
                DateTime empLogOut = Convert.ToDateTime(dtShiftLogOut.Date.ToString("yyyy-MM-dd") + " " + dtShiftLogOut.ToString("HH:mm"));
                logOutMinsTardy = (int)logOutTime.Subtract(empLogOut).TotalMinutes;

                if (logOutMinsTardy < 1)
                {
                    logOutMinsTardy = 0;
                }

            }


            myData.EIC = EIC;
            myData.schemeName = "7AM - 3PM";
            myData.logNo = dt.Day;
            myData.logDate = dt;
            myData.logDay = dt.ToString("ddd");
            myData.logIn = tmpLogin;
            myData.logOut = tmpLogOut;


            //myData.Add(new myShiftDTR()
            //{
            //    EIC = EIC,
            //    logNo = dt.Day,
            //    logDay = dt.ToString("ddd"),
            //    logDate = dt,
            //    loginDT = dtLogin1,
            //    logoutDT = dtLogout1,
            //    logIn = tmpLogin,
            //    logOut = tmpLogOut,
            //    remarks = "",
            //    //tardyA = Convert.ToInt16(logInMinsTardy),
            //    //underTimeA = Convert.ToInt16(logOutMinsTardy),
            //    //isWorkingDay = isWorkingDay,
            //    //hasLog = hasLogDay,
            //});
            return myData;
        }

        //public string ShiftLogIn(IEnumerable<tAttLog> logs, tAttEmpSchemeSchedDetail scheme, int code)
        //{
        //    string res = "";
        //    double mins = -60; double minsmax = 180;
        //    DateTime validLogFrom = Convert.ToDateTime(scheme.loginDT);
        //    validLogFrom = validLogFrom.AddMinutes(mins);
        //    DateTime validLogTo = Convert.ToDateTime(scheme.logoutDT);
        //    //VALID FOR RANGE FOR LOGIN
        //    IEnumerable<tAttLog> myShiftLogs = logs.Where(e => e.logDT >= validLogFrom && e.logDT < validLogTo).OrderBy(o => o.logDT).ToList();
        //    //SELECTED LOG
        //    // [7:00AM - 3:00PM]  >>>  6:00AM <-> 2:59PM  TAG == 1
        //    IEnumerable<tAttLog> tempLog = logs.Where(e => e.logDT >= validLogFrom && e.logDT < validLogTo && e.tag == 1).OrderBy(o => o.logDT).ToList();
        //    if (tempLog.Count() > 0)
        //    {
        //        var log = tempLog.First();
        //        dtShiftLogin = Convert.ToDateTime(log.logDT);
        //        res = string.Format("{0:MM/dd/yyyy hh:mm tt}", log.logDT);
        //        return res;
        //    }
        //    // MINIMUM RANGE FOR LOGIN
        //    // [7:00AM - 3:00PM]  >>>  6:00AM <-> 10:00PM  TAG == NULL
        //    validLogTo = validLogFrom.AddMinutes(minsmax); // +3hrs/ 180mins
        //    IEnumerable<tAttLog> minShiftLogs = logs.Where(e => e.logDT >= validLogFrom && e.logDT <= validLogTo && e.tag == null).OrderBy(o => o.logDT).ToList();
        //    foreach (var log in minShiftLogs)
        //    {
        //        dtShiftLogin = Convert.ToDateTime(log.logDT);
        //        res = string.Format("{0:MM/dd/yyyy hh:mm tt}", log.logDT);
        //        return res;
        //    }
        //    return res;
        //}


        public string ShiftLogIn(IEnumerable<tAttLog> logs, tAttEmpSchemeSchedDetail scheme, int code)
        {
            string res = "";
            double minsmin = -60; double minsmax = 60;
            DateTime validLogFrom = Convert.ToDateTime(scheme.loginDT);
            validLogFrom = validLogFrom.AddMinutes(minsmin);
            DateTime validLogTo = Convert.ToDateTime(scheme.loginDT);
            validLogTo = validLogTo.AddMinutes(minsmax);

            //VALID FOR RANGE FOR LOGIN
            //IEnumerable<tAttLog> myShiftLogs = logs.Where(e => e.logDT >= validLogFrom && e.logDT <= validLogTo).OrderBy(o => o.logDT).ToList();
            //SELECTED LOG
            // [7:00AM - 3:00PM]  >>>  6:00AM <-> 2:59PM  TAG == 1
            IEnumerable<tAttLog> tempLog = logs.Where(e => e.logDT >= validLogFrom && e.logDT < validLogTo).OrderBy(o => o.logDT).ToList();
            if (tempLog.Count() > 0)
            {
                var log = tempLog.First();
                dtShiftLogin = Convert.ToDateTime(log.logDT);
                res = string.Format("{0:MM/dd/yyyy hh:mm tt}", log.logDT);
                return res;
            }
            return res;
        }


        public string ShiftLogOut(IEnumerable<tAttLog> logs, tAttEmpSchemeSchedDetail scheme, int code)
        {
            string res = "";
            double minsmin = -60; double minsmax = 180;

            DateTime validLogFrom = Convert.ToDateTime(scheme.logoutDT);
            validLogFrom = validLogFrom.AddMinutes(minsmin);

            DateTime validLogTo = Convert.ToDateTime(scheme.logoutDT);
            validLogTo = validLogTo.AddMinutes(minsmax);

            // CHECK IF THERE IS SELECTED LOG
            // [7:00AM - 3:00PM]  >>>  7:01AM <-> 4:00PM  TAG == 2
            IEnumerable<tAttLog> tempLogs = logs.Where(e => e.logDT >= validLogFrom && e.logDT <= validLogTo).OrderByDescending(o => o.logDT).ToList();
            if (tempLogs.Count() > 0)
            {
                var log = tempLogs.First();
                dtShiftLogOut = Convert.ToDateTime(log.logDT);
                res = string.Format("{0:MM/dd/yyyy hh:mm tt}", log.logDT);
                return res;
            }

            return res;
        }



        //public string ShiftLogOut(IEnumerable<tAttLog> logs, tAttEmpSchemeSchedDetail scheme, int code)
        //{
        //    string res = "";
        //    double mins = 60; double minsMax = 180;

        //    DateTime validLogFrom = Convert.ToDateTime(scheme.loginDT);

        //    DateTime validLogTo = Convert.ToDateTime(scheme.logoutDT);
        //    validLogTo = validLogTo.AddMinutes(mins);

        //    // CHECK IF THERE IS SELECTED LOG
        //    // [7:00AM - 3:00PM]  >>>  7:01AM <-> 4:00PM  TAG == 2
        //    IEnumerable<tAttLog> tempLogs = logs.Where(e => e.logDT > validLogFrom && e.logDT <= validLogTo && e.tag == 2).OrderBy(o => o.logDT).ToList();
        //    if (tempLogs.Count() > 0)
        //    {
        //        var log = tempLogs.First();
        //        dtShiftLogOut = Convert.ToDateTime(log.logDT);
        //        res = string.Format("{0:MM/dd/yyyy hh:mm tt}", log.logDT);
        //        return res;
        //    }

        //    // [7:00AM - 3:00PM]  >>>  12:00PM <-> 4:00PM  TAG == NULL
        //    DateTime tmpMaxDate = Convert.ToDateTime(scheme.logoutDT).AddHours(minsMax);
        //    IEnumerable<tAttLog> myLogs = logs.Where(e => e.logDT >= validLogFrom && e.logDT < validLogTo && e.tag == null).OrderByDescending(o => o.logDT).ToList();
        //    foreach (var log in myLogs)
        //    {
        //        dtShiftLogOut = Convert.ToDateTime(log.logDT);
        //        res = string.Format("{0:MM/dd/yyyy hh:mm tt}", log.logDT);
        //        return res;
        //    }

        //    return res;
        //}





        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //private int ShiftingLog(tRSPEmployee emp, tAttEmpScheme scheme, DateTime dateFrom, DateTime dateTo)
        //{
        //    int i = 0;
        //    int userLogNo = Convert.ToInt16(emp.idNo);         
        //    //TEMP SCHEME CODE -> SCHM7AMTO3PM
        //    IEnumerable<tAttEmpSchemeSchedDetail> sched = db.tAttEmpSchemeSchedDetails.Where(e => e.loginDT >= dateFrom && e.loginDT <= dateTo).OrderBy(o => o.loginDT).ToList();

        //    tAttEmpSchemeSchedDetail startSched = sched.First();
        //    tAttEmpSchemeSchedDetail endSched = sched.Last();

        //    DateTime logStart = startSched.loginDT.Value.AddHours(-1);
        //    DateTime logEnd = startSched.logotDT.Value.AddHours(1);

        //    IEnumerable<tAttLog> logList = db.tAttLogs.Where(e => e.userLogNo == userLogNo && e.logDT >= logStart && e.logDT < logEnd).ToList();

        //    foreach (tAttEmpSchemeSchedDetail s in sched)
        //    {
        //        IEnumerable<tAttLog> logs = logList.Where(e => e.logDT >= s.loginDT && e.logDT <= s.logotDT).ToList();
        //        GenerateShiftLog(emp.EIC, dateFrom, s, logs);
        //    }
        //    return i;
        //}

        //  private int GenList(string EIC, DateTime dt, tAttLogScheme scheme, IEnumerable<tAttLog> logs)



        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////




        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////




        ////NG
        //[HttpPost]
        //public JsonResult ViewLog(DTRView data)
        //{
        //    string uEIC = Session["_EIC"].ToString();
        //    string empScheme = "";

        //    tAttEmpScheme scheme = db.tAttEmpSchemes.OrderByDescending(o => o.recNo).FirstOrDefault(e => e.EIC == uEIC);

        //    if (scheme != null && scheme.tag == 1)
        //    {
        //        //CHECK THE SCHEME DATE IF VALID               
        //        IEnumerable<tAttEmpSchemeSchedDetail> schedDetail = db.tAttEmpSchemeSchedDetails.Where(e => e.schemeCode == scheme.schemeCode).OrderBy(o => o.loginDT).ToList();
        //    }
        //    else
        //    {
        //        empScheme = "SCHM0001REG"; //DEFAULT SCHEME
        //    }


        //    DateTime periodFrom = DateTime.Now;
        //    DateTime periodTo = DateTime.Now;
        //    string tmpMonth = "1 " + data.month;

        //    if (data.period == 0)
        //    {
        //        periodFrom = Convert.ToDateTime(tmpMonth);
        //        periodTo = periodFrom.AddMonths(1).AddDays(-1);
        //    }
        //    else if (data.period == 1)
        //    {
        //        string tmpDate = "15 " + data.month;
        //        periodFrom = Convert.ToDateTime(tmpMonth);
        //        periodTo = Convert.ToDateTime(tmpDate);
        //    }
        //    else if (data.period == 2)
        //    {
        //        string tmpDate = "16 " + data.month;
        //        periodFrom = Convert.ToDateTime(tmpDate);
        //        DateTime tempDate = Convert.ToDateTime(tmpMonth);
        //        periodTo = tempDate.AddMonths(1).AddDays(-1);
        //    }


        //    tRSPEmployee emp = db.tRSPEmployees.Single(e => e.EIC == uEIC);

        //    int userLogNo = Convert.ToInt16(emp.idNo);
        //    DateTime tempDateTo = periodTo.AddDays(1);
        //    IEnumerable<tAttLog> logs = db.tAttLogs.Where(e => e.userLogNo == userLogNo && e.logDT >= periodFrom && e.logDT < tempDateTo).ToList();

        //    tAttLogScheme attScheme = db.tAttLogSchemes.Single(e => e.schemeCode == "SCHM0001REG");

        //    for (DateTime runDate = periodFrom; runDate <= periodTo; )
        //    {
        //        if (runDate >= periodFrom && runDate <= periodTo)
        //        {
        //            //FIND BEST LOG
        //            GenList(emp.EIC, runDate, attScheme, logs);
        //            //FIND MAX LOG
        //            GenMaxLog(runDate, attScheme, logs);

        //            int d = runDate.Date.Day;
        //            //if (myLog[d - 1].hasLog == 1)
        //            //{
        //            //    int counter = dailyLogs.Where(e => e.LogDate == tmpDate).Count();
        //            //    if (counter > 0)
        //            //    {
        //            //        myLog[d - 1].hasSync = 2;
        //            //    }
        //            //    else
        //            //    {
        //            //        myLog[d - 1].hasSync = 1;
        //            //    }
        //            //}
        //        }
        //        else
        //        {
        //            BlankWritter(runDate);
        //        }
        //        runDate = runDate.AddDays(1);
        //    }



        //    // SET DEFAULT SCHEME : SCHM0001REG

        //    // 1. CHECK SHIFTING SCHEME & GET SCHEDULE
        //    // 2. GET DTR LOGS
        //    // 3. POPULATE LOGS
        //    // 4. CALCULATE LOGS
        //    // 5. RETURN LOGLIST



        //    //CHECK PERIOD [FULLMONTH; 1ST HALF, 2ND HALF]
        //    //CHECK SCHEME [REGULAR, FLEXI, SHIFTING]
        //    //-- IF FLEXI AND REGULAR SAME FORMAT
        //    //-- IF SHIFTING CHECK 

        //    //List<DTRViewModel> logs = new List<DTRViewModel>();



        //    return Json(new { status = "success", logList = myLog }, JsonRequestBehavior.AllowGet);
        //}




        ///////////////////////////////////////////////////////////////////////////////////////
        //// LOGS





        //public int BlankWritter(DateTime dt)
        //{
        //    myLog.Add(new myLogList()
        //    {
        //        EIC = "",
        //        logNo = dt.Day,
        //        logDate = dt,
        //        logIn1 = "",
        //        logOut1 = "",
        //        logIn2 = "",
        //        logOut2 = "",
        //        remarks = "",
        //        isWorkingDay = 0
        //    });
        //    return 0;
        //}

        //private int GenList(string EIC, DateTime dt, tAttLogScheme scheme, IEnumerable<tAttLog> logs)
        //{
        //    string midLogs = "";

        //    _login1 = "";
        //    _logout1 = "";
        //    _login2 = "";
        //    _logout2 = "";

        //    string tmpLogDT = dt.ToString("MMMM dd, yyyy") + " " + scheme.in1;
        //    _login1 = FindMyLogIn(logs, Convert.ToDateTime(tmpLogDT), scheme, 1);

        //    tmpLogDT = dt.ToString("MMMM dd, yyyy") + " " + scheme.out1;
        //    _logout1 = FinMyLogOut(logs, Convert.ToDateTime(tmpLogDT), scheme, 1);

        //    tmpLogDT = dt.ToString("MMMM dd, yyyy") + " " + scheme.in2;
        //    midLogs = FindMyLogIn2(logs, Convert.ToDateTime(tmpLogDT), scheme, 2);

        //    string[] tmpVal = midLogs.Split(';');

        //    if (tmpVal[0] == "2")
        //    {
        //        _logout1 = tmpVal[1].ToString();
        //        _login2 = tmpVal[2].ToString();
        //    }
        //    else if (tmpVal[0] == "1")
        //    {
        //        _login2 = tmpVal[1].ToString();
        //    }

        //    tmpLogDT = dt.ToString("MMMM dd, yyyy") + " " + scheme.out2;
        //    _logout2 = FinMyLogOut(logs, Convert.ToDateTime(tmpLogDT), scheme, 2);

        //    //CHECK ALSO HOLIDAY
        //    int isWorkingDay = 1;
        //    if (Convert.ToInt16(dt.DayOfWeek) == 0 || Convert.ToInt16(dt.DayOfWeek) == 6)
        //    {
        //        int s = Convert.ToInt16(dt.DayOfWeek);
        //        isWorkingDay = 0;
        //    }

        //    int hasLogDay = 0;
        //    if (_login1.Length > 0 || _login2.Length > 0 || _logout1.Length > 0 || _logout2.Length > 0)
        //    {
        //        hasLogDay = 1;
        //    }

        //    myLog.Add(new myLogList()
        //    {
        //        EIC = EIC,
        //        logNo = dt.Day,
        //        logDay = dt.ToString("ddd"),
        //        logDate = dt,
        //        dtLogin1 = dtLogin1,
        //        dtLogout1 = dtLogout1,
        //        dtLogin2 = dtLogin2,
        //        dtLogout2 = dtLogout2,
        //        logIn1 = _login1,
        //        logOut1 = _logout1,
        //        logIn2 = _login2,
        //        logOut2 = _logout2,
        //        remarks = "",
        //        isWorkingDay = isWorkingDay,
        //        hasLog = hasLogDay,
        //        hasSync = 0
        //    });

        //    return 0;
        //}



        //public string FindMyLogIn(IEnumerable<tAttLog> logs, DateTime logDT, tAttLogScheme scheme, int code)
        //{
        //    string res = "";
        //    double minValidF = 0; double minValidT = 0;

        //    if (code == 1)
        //    {
        //        minValidF = Convert.ToDouble(scheme.validIn1_minsF);
        //        minValidT = Convert.ToDouble(scheme.validIn1_minsT);
        //    }
        //    else
        //    {
        //        minValidF = Convert.ToDouble(scheme.validIn2_minsF);
        //        minValidT = Convert.ToDouble(scheme.validIn2_minsT);
        //    }

        //    //MIN LOGS SEARCH
        //    DateTime validLogFrom = logDT.AddMinutes(minValidF);
        //    DateTime validLogTo = logDT.AddMinutes(minValidT + 1);
        //    IEnumerable<tAttLog> myLogs = logs.Where(e => e.logDT >= validLogFrom && e.logDT < validLogTo).ToList();

        //    foreach (var log in myLogs)
        //    {
        //        dtLogin1 = Convert.ToDateTime(log.logDT);
        //        res = string.Format("{0:hh:mm:ss tt}", log.logDT);
        //        return res;
        //    }

        //    return res;
        //}


        //public string FindMyLogIn2(IEnumerable<tAttLog> logs, DateTime logDT, tAttLogScheme scheme, int code)
        //{
        //    string res = "";
        //    double minValidF = 0; double minValidT = 0;

        //    if (code == 1)
        //    {
        //        minValidF = Convert.ToDouble(scheme.validIn1_minsF);
        //        minValidT = Convert.ToDouble(scheme.validIn1_minsT);
        //    }
        //    else
        //    {
        //        minValidF = Convert.ToDouble(scheme.validIn2_minsF);
        //        minValidT = Convert.ToDouble(scheme.validIn2_minsT);
        //    }

        //    //MIN LOGS SEARCH
        //    DateTime validLogFrom = logDT.AddMinutes(minValidF);
        //    DateTime validLogTo = logDT.AddMinutes(minValidT + 1);
        //    IEnumerable<tAttLog> myLogs = logs.Where(e => e.logDT >= validLogFrom && e.logDT < validLogTo).ToList();

        //    string logA = "";
        //    string logB = "";
        //    DateTime dtLogA = DateTime.Now;
        //    DateTime dtLogB = DateTime.Now;

        //    foreach (var log in myLogs)
        //    {
        //        res = string.Format("{0:hh:mm:ss tt}", log.logDT);

        //        if (_logout1.Length > 0)
        //        {
        //            logA = res;
        //            res = "1;" + logA + ";";
        //            dtLogout1 = Convert.ToDateTime(log.logDT);
        //            return res;
        //        }
        //        else
        //        {
        //            if (logA == "" || logB == "")
        //            {
        //                if (logA == "")
        //                {
        //                    logA = res;
        //                    dtLogA = Convert.ToDateTime(log.logDT);
        //                }
        //                else
        //                {
        //                    logB = res;
        //                    dtLogB = Convert.ToDateTime(log.logDT);
        //                }
        //            }
        //        }


        //        if (logA.Length > 0 && logB.Length > 0)
        //        {
        //            res = "2;" + logA + ";" + logB;
        //            dtLogout1 = Convert.ToDateTime(dtLogA);
        //            dtLogin2 = Convert.ToDateTime(dtLogB);
        //            return res;
        //        }
        //    }

        //    res = "1;" + logA + ";";
        //    return res;
        //}



        //public string FinMyLogOut(IEnumerable<tAttLog> logs, DateTime logDT, tAttLogScheme scheme, int code)
        //{
        //    string res = "";
        //    double minValidF = 0; double minValidT = 0;

        //    if (code == 1)
        //    {
        //        minValidF = Convert.ToDouble(scheme.validOut1_minsF);
        //        minValidT = Convert.ToDouble(scheme.validOut1_minsT) + 1;
        //    }
        //    else
        //    {
        //        minValidF = Convert.ToDouble(scheme.validOut2_minsF);
        //        minValidT = Convert.ToDouble(scheme.validOut2_minsT) + 1;
        //    }
        //    DateTime validLogFrom = logDT.AddMinutes(minValidF);
        //    DateTime validLogTo = logDT.AddMinutes(minValidT);
        //    IEnumerable<tAttLog> myLogs = logs.Where(e => e.logDT >= validLogFrom && e.logDT < validLogTo).ToList();

        //    foreach (var log in myLogs)
        //    {
        //        res = string.Format("{0:hh:mm:ss tt}", log.logDT);
        //        if (code == 1)
        //        {
        //            dtLogout1 = Convert.ToDateTime(log.logDT);
        //        }
        //        else
        //        {
        //            dtLogout2 = Convert.ToDateTime(log.logDT);
        //        }
        //        return res;
        //    }


        //    return res;
        //}


        //public string GenMaxLog(DateTime dt, tAttLogScheme scheme, IEnumerable<tAttLog> logs)
        //{
        //    string myMaxLogIn1, myMaxLogOut1, myMaxLogin2, myMaxLogOut2;

        //    //LOGIN1 IS EMPTY
        //    string myTempLogin1 = myLog[dt.Day - 1].logIn1;
        //    string myTempLogout1 = myLog[dt.Day - 1].logOut1;

        //    string myTempLogin2 = myLog[dt.Day - 1].logIn2;
        //    string myTempLogout2 = myLog[dt.Day - 1].logOut2;

        //    //IF [LOGIN 1 IS BLANK]
        //    if (myTempLogin1.Length == 0)
        //    {
        //        string tmpLogDT = dt.ToString("MMMM dd, yyyy") + " " + scheme.in1;
        //        DateTime tempLogDT = Convert.ToDateTime(tmpLogDT);
        //        GetMaxLogin1(logs, tempLogDT, scheme, 1);
        //    }
        //    //IF [LOGOUT 1 IS BLANK]
        //    if (myTempLogout1.Length == 0)
        //    {
        //        string tmpLogDT = dt.ToString("MMMM dd, yyyy") + " " + scheme.out1;
        //        DateTime tempLogDT = Convert.ToDateTime(tmpLogDT);
        //        GetMaxLogout1(logs, tempLogDT, scheme, 1);
        //    }
        //    //IF [LOGIN 2 IS BLANK]
        //    if (myTempLogin2.Length == 0)
        //    {
        //        string tmpLogDT = dt.ToString("MMMM dd, yyyy") + " " + scheme.in2;
        //        DateTime tempLogDT = Convert.ToDateTime(tmpLogDT);
        //        GetMaxLogin2(logs, tempLogDT, scheme, 2);
        //    }
        //    //IF [LOGOUT 1 IS BLANK]
        //    if (myTempLogout2.Length == 0)
        //    {
        //        string tmpLogDT = dt.ToString("MMMM dd, yyyy") + " " + scheme.out2;
        //        DateTime tempLogDT = Convert.ToDateTime(tmpLogDT);
        //        GetMaxLogout2(logs, tempLogDT, scheme, 2);
        //    }

        //    return "";
        //}


        //private string GetMaxLogin1(IEnumerable<tAttLog> logs, DateTime logDT, tAttLogScheme scheme, int code)
        //{
        //    string res;
        //    DateTime maxLogFrom = logDT.AddMinutes(Convert.ToDouble(scheme.maxLogIn1_minF));
        //    DateTime maxLogTo = logDT.AddMinutes(Convert.ToDouble(scheme.maxLogIn1_minT) + 1);
        //    IEnumerable<tAttLog> myLogs = logs.Where(e => e.logDT >= maxLogFrom && e.logDT <= maxLogTo).ToList();
        //    foreach (var log in myLogs)
        //    {
        //        res = string.Format("{0:hh:mm:ss tt}", log.logDT);
        //        myLog[log.logDT.Value.Day - 1].logIn1 = res;
        //        myLog[log.logDT.Value.Day - 1].dtLogin1 = Convert.ToDateTime(log.logDT);
        //        return res;
        //    }
        //    return "";
        //}


        //private string GetMaxLogout1(IEnumerable<tAttLog> logs, DateTime logDT, tAttLogScheme scheme, int code)
        //{
        //    string res;
        //    //MAX LOGS SEARCH          
        //    DateTime maxLogFrom = logDT.AddMinutes(Convert.ToDouble(scheme.maxLogOut1_minF));
        //    DateTime maxLogTo = logDT.AddMinutes(Convert.ToDouble(scheme.maxLogOut1_minT) + 1);
        //    IEnumerable<tAttLog> myLogs = logs.Where(e => e.logDT >= maxLogFrom && e.logDT < maxLogTo).OrderByDescending(o => o.logDT).ToList();
        //    foreach (var log in myLogs)
        //    {
        //        res = string.Format("{0:hh:mm:ss tt}", log.logDT);
        //        myLog[log.logDT.Value.Day - 1].logOut1 = res;
        //        myLog[log.logDT.Value.Day - 1].dtLogout1 = Convert.ToDateTime(log.logDT);
        //        return res;
        //    }
        //    return "";
        //}


        //private string GetMaxLogin2(IEnumerable<tAttLog> logs, DateTime logDT, tAttLogScheme scheme, int code)
        //{
        //    string res;
        //    //MAX LOGS SEARCH          
        //    DateTime maxLogFrom = logDT.AddMinutes(Convert.ToDouble(scheme.maxLogIn2_minF));
        //    DateTime maxLogTo = logDT.AddMinutes(Convert.ToDouble(scheme.maxLogIn2_minT) + 1);

        //    IEnumerable<tAttLog> myLogs = logs.Where(e => e.logDT >= maxLogFrom && e.logDT < maxLogTo).ToList();
        //    foreach (var log in myLogs)
        //    {
        //        res = string.Format("{0:hh:mm:ss tt}", log.logDT);
        //        myLog[log.logDT.Value.Day - 1].logIn2 = res;
        //        myLog[log.logDT.Value.Day - 1].dtLogin2 = Convert.ToDateTime(log.logDT);
        //        return res;
        //    }

        //    return "";
        //}

        //private string GetMaxLogout2(IEnumerable<tAttLog> logs, DateTime logDT, tAttLogScheme scheme, int code)
        //{
        //    string res;
        //    //MAX LOGS SEARCH          
        //    DateTime maxLogFrom = logDT.AddMinutes(Convert.ToDouble(scheme.maxLogOut2_minF));
        //    DateTime maxLogTo = logDT.AddMinutes(Convert.ToDouble(scheme.maxLogOut2_minT) + 1);
        //    IEnumerable<tAttLog> myLogs = logs.Where(e => e.logDT >= maxLogFrom && e.logDT < maxLogTo).OrderByDescending(o => o.logDT).ToList();

        //    foreach (var log in myLogs)
        //    {
        //        res = string.Format("{0:hh:mm:ss tt}", log.logDT);
        //        myLog[log.logDT.Value.Day - 1].logOut2 = res;
        //        myLog[log.logDT.Value.Day - 1].dtLogout2 = Convert.ToDateTime(log.logDT);
        //        return res;
        //    }
        //    return "";
        //}


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //// JUSTIFICATION
        //[HttpPost]
        //public JsonResult Justy(string jDate)
        //{
        //    try
        //    {


        //        DateTime dt;
        //        dt = Convert.ToDateTime(jDate);
        //        IEnumerable<tAttEmpSchemeSchedDetail> sched = db.tAttEmpSchemeSchedDetails.Where(e => e.loginDT.Value.Year == dt.Year && e.loginDT.Value.Month == dt.Month && e.loginDT.Value.Day == dt.Day).ToList();

        //        List<SelectListItem> list = new List<SelectListItem>();
        //        foreach (tAttEmpSchemeSchedDetail item in sched)
        //        {
        //            string tmpText = Convert.ToDateTime(item.loginDT).ToString("hh:mm tt") + " - " + Convert.ToDateTime(item.logoutDT).ToString("hh:mm tt");
        //            string tmpValue = Convert.ToDateTime(item.loginDT).ToString("MMMM dd, yyyy HH:mm");
        //            list.Add(new SelectListItem()
        //            {
        //                Text = tmpText,
        //                Value = tmpValue
        //            });
        //        }

        //        return Json(new { status = "success", schedList = list }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception)
        //    {

        //        return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
        //    }

        //}

        ////SubmitJusty
        //public JsonResult SubmitJusty(tJustifyApp data)
        //{
        //    try
        //    {

        //        string tmp = DateTime.Now.ToString("yyMMddHHmmssfff");
        //        string code = "JTFN" + tmp;


        //        tJustifyApp app = new tJustifyApp();
        //        app.controlNo = code;
        //        app.EIC = Session["_EIC"].ToString();
        //        app.logDT = data.logDT;
        //        app.schemeDT = data.schemeDT;
        //        app.reason = data.reason;
        //        app.logType = data.logType;
        //        app.approveEIC = data.approveEIC;
        //        app.statusTag = 0;
        //        app.returnTag = 0;
        //        app.userEIC = "";
        //        app.transDT = DateTime.Now;
        //        db.tJustifyApps.Add(app);
        //        db.SaveChanges();

        //        return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception)
        //    {

        //        return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
        //    }
        //}


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}