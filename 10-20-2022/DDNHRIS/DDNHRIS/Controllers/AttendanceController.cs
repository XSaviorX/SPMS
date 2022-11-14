using DDNHRIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DDNHRIS.Models.Attendance;

namespace DDNHRIS.Controllers
{
    [SessionTimeout]
    [Authorize(Roles = "GROUPADMIN")]
    public class AttendanceController : Controller
    {

        HRISDBEntities db = new HRISDBEntities();

        public ActionResult WorkGroup()
        {
            return View();
        }

        [HttpPost]
        public JsonResult MyWorkGroupList()
        {
            try
            {
                string uEIC = Session["_EIC"].ToString();
                tRSPWorkGroupAdmin wgAdm = db.tRSPWorkGroupAdmins.SingleOrDefault(e => e.EIC == uEIC && e.tag >= 1);

                if (wgAdm == null)
                {
                    return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
                }

                string groupCode = wgAdm.workGroupCode;
                List<vRSPWorkGroupEmp> list = new List<vRSPWorkGroupEmp>();

                if (wgAdm.tag == 2)
                {
                    list = db.vRSPWorkGroupEmps.Where(e => e.workGroupCode != null).OrderBy(o => o.fullNameLast).ToList();
                }
                else
                {
                    list = db.vRSPWorkGroupEmps.Where(e => e.workGroupCode == groupCode).OrderBy(o => o.fullNameLast).ToList();
                }

                DateTime dt = DateTime.Now;
                List<SelectListItem> myList = new List<SelectListItem>();
                for (int i = 0; i < 3; i++)
                {
                    string tmp = dt.ToString("MMMM") + " " + dt.Year;
                    myList.Add(new SelectListItem() { Text = tmp, Value = tmp });
                    dt = dt.AddMonths(-1);
                }

                var myScheme = db.tAttLogSchemes.Select(e => new
                {
                    e.schemeCode,
                    e.schemeName,
                    e.schemeDetails,
                    e.isActive
                }).Where(e => e.isActive == 1).ToList();


                IEnumerable<tAttWorkSchedTemplate> sList = db.tAttWorkSchedTemplates.ToList();
                List<tAttWorkSchedTemplate> temp = new List<tAttWorkSchedTemplate>();

                temp.Add(new tAttWorkSchedTemplate()
                {
                    shiftTemplateId = "NONE",
                    shiftName = "(NONE)"
                });

                foreach (tAttWorkSchedTemplate item in sList)
                {
                    temp.Add(new tAttWorkSchedTemplate()
                    {
                        shiftTemplateId = item.shiftTemplateId,
                        shiftName = item.shiftName
                    });
                }


                return Json(new { status = "success", list = list, monthList = myList, schemeList = myScheme, schemes = temp }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        public List<myDailyTimeRecord> MySchemeList(string id, string code, int userLogNo, string EIC)
        {
            try
            {
                string[] text = code.Split(' ');
                DateTime baseMonth = Convert.ToDateTime(text[0].Trim() + " 1, " + text[1].Trim());
                DateTime lastDay = new DateTime(baseMonth.Year, baseMonth.Month, 1).AddMonths(1).AddDays(-1);

                IEnumerable<vAttEmpSchemeDaily> dailyScheme = db.vAttEmpSchemeDailies.Where(e => e.EIC == id && e.logDate.Value.Year == baseMonth.Year & e.logDate.Value.Month == baseMonth.Month).ToList();

                List<myDailyTimeRecord> schedule = new List<myDailyTimeRecord>();

                //HRISv1 tAttDailyLog
                HRISEntities hrDB = new HRISEntities();
                IEnumerable<tAttDailyLog> dailyLog = hrDB.tAttDailyLogs.Where(e => e.EIC == id && e.LogDate.Value.Year == baseMonth.Year && e.LogDate.Value.Month == baseMonth.Month).ToList();
                //HRISv1 
                IEnumerable<tAttShiftingEmpScheme> dayScheme = hrDB.tAttShiftingEmpSchemes.Where(e => e.EIC == id && e.entryDate.Value.Year == baseMonth.Year && e.entryDate.Value.Month == baseMonth.Month).OrderBy(o => o.entryDate).ToList();
                //HRISv2 
                IEnumerable<tAttLog> monthLogs = db.tAttLogs.Where(e => e.userLogNo == userLogNo || e.EIC == EIC).Where(e => e.logDT.Value.Year == baseMonth.Year && e.logDT.Value.Month == baseMonth.Month && e.isActive != 0).ToList();

                //IEnumerable<tAttLog> monthLogs = db.tAttLogs.Where(e => e.userLogNo == userLogNo && e.logDT.Value.Year == baseMonth.Year && e.logDT.Value.Month == baseMonth.Month).ToList();

                //HRISv2 REGULAR LOG SCHME
                tAttLogScheme regScheme = db.tAttLogSchemes.Single(e => e.schemeCode == "SCHM0001REG");


                for (DateTime tmpDate = baseMonth; tmpDate <= lastDay; )
                {
                    vAttEmpSchemeDaily schm = dailyScheme.SingleOrDefault(e => e.logDate.Value.Day == tmpDate.Day);

                    string tmpSchmCode = "";
                    string tmpSchmName = "";

                    if (schm != null)
                    {
                        tmpSchmCode = schm.schemeCode;
                        tmpSchmName = schm.schemeName;
                    }

                    string tmpLogin1 = "", tmpLogin2 = "", tmpLogOut1 = "", tmpLogOut2 = "";
                    //CHECK IF THERE IS SCHEME OF THE DAY
                    //HRISv1
                    tAttShiftingEmpScheme tmpSchm = dayScheme.FirstOrDefault(e => e.entryDate.Value.Day == tmpDate.Day);

                    tAttDailyLog attLog = dailyLog.SingleOrDefault(e => e.LogDate.Value.Day == tmpDate.Day);

                    //CHECK IF HAS LOG OF THE DAY
                    IEnumerable<tAttLog> logs = monthLogs.Where(e => e.logDT.Value.Day == tmpDate.Day).ToList();

                    if (tmpDate.Day == 12)
                    {
                        string gi = "";
                    }

                    int updateTag = 0;
                    vAttEmpSchemeDaily scheme = new vAttEmpSchemeDaily();

                    //************* TEMPORARY ******************************
                    if ((logs.Count() == 0) && tmpDate.Day == 21 && tmpDate.Month == 2 && tmpDate.Year == 2021)
                    {
                        List<tAttLog> temp = new List<tAttLog>();
                        tAttLog n = new tAttLog();
                        n.logDT = tmpDate;
                        temp.Add(n);
                        logs = temp.ToList();
                    }

                    if (logs.Count() > 0)
                    {
                        string myTempScheme = "";
                        if (tmpSchm == null) //NO SCHEME OF THE DAY
                        {
                            myTempScheme = "REGULAR (8AM-12NN && 1PM-5PM)";
                            scheme = RegularSchemeConverter(regScheme);
                            scheme.schemeCode = "SCHM0001REG";
                        }
                        else
                        {
                            myTempScheme = "SHIFTING";
                            myTempScheme = GetMyScheme(Convert.ToDateTime(tmpSchm.In1), Convert.ToDateTime(tmpSchm.Out1), Convert.ToDateTime(tmpSchm.In2), Convert.ToDateTime(tmpSchm.Out2));
                            scheme = SchemeConverter(tmpSchm);
                            scheme.schemeCode = myTempScheme;
                            scheme.schemeName = myTempScheme;
                        }

                        if (tmpDate.Day == 20)
                        {
                            updateTag = 0;
                        }

                        if (tmpDate.Day == 21)
                        {
                            updateTag = 1;
                        }

                        //LOG TO DTR : DAILY LOG
                        //MY DAILY LOG
                        TempDailyLog dayLog = DailyLog(tmpDate, logs, scheme);
                        tmpLogin1 = dayLog.login1.Year == 1 ? "" : string.Format("{0:hh:mm:ss tt}", dayLog.login1);
                        tmpLogOut1 = dayLog.logout1.Year == 1 ? "" : string.Format("{0:hh:mm:ss tt}", dayLog.logout1);
                        tmpLogin2 = dayLog.login2.Year == 1 ? "" : string.Format("{0:hh:mm:ss tt}", dayLog.login2);
                        tmpLogOut2 = dayLog.logout2.Year == 1 ? "" : string.Format("{0:hh:mm:ss tt}", dayLog.logout2);


                        updateTag = 0;

                        //string testLogin1 = string.Format("{0:hh:mm:ss tt}", attLog.In1);
                        //string testLogOut1 = string.Format("{0:hh:mm:ss tt}", attLog.Out1);
                        //string testLogin2 = string.Format("{0:hh:mm:ss tt}", attLog.In2);
                        //string testLogOut2 = string.Format("{0:hh:mm:ss tt}", attLog.Out2);


                        //if (testLogin1 != tmpLogin1)
                        //{
                        //    updateTag = 1;
                        //}
                        //if (testLogOut1 != tmpLogOut1)
                        //{
                        //    updateTag = 1;
                        //}
                        //if (testLogin2 != tmpLogin2)
                        //{
                        //    updateTag = 1;
                        //}
                        //if (testLogOut2 != tmpLogOut2)
                        //{
                        //    updateTag = 1;
                        //}

                        if (attLog != null)
                        {
                            updateTag = 0;
                            string testLogin1 = string.Format("{0:hh:mm:ss tt}", attLog.In1);
                            string testLogOut1 = string.Format("{0:hh:mm:ss tt}", attLog.Out1);
                            string testLogin2 = string.Format("{0:hh:mm:ss tt}", attLog.In2);
                            string testLogOut2 = string.Format("{0:hh:mm:ss tt}", attLog.Out2);
                            if (testLogin1 != tmpLogin1)
                            {
                                updateTag = 1;
                            }
                            if (testLogOut1 != tmpLogOut1)
                            {
                                updateTag = 1;
                            }
                            if (testLogin2 != tmpLogin2)
                            {
                                updateTag = 1;
                            }
                            if (testLogOut2 != tmpLogOut2)
                            {
                                updateTag = 1;
                            }
                        }
                        else
                        {
                            updateTag = 1;
                        }
                    }
                    else
                    {
                        scheme.schemeCode = "";
                        scheme.schemeName = "";
                    }

                    schedule.Add(new myDailyTimeRecord()
                    {
                        EIC = id,
                        logDate = tmpDate.Date,
                        logDay = tmpDate.ToString("ddd"),
                        login1 = tmpLogin1,
                        logout1 = tmpLogOut1,
                        login2 = tmpLogin2,
                        logout2 = tmpLogOut2,
                        schemeCode = scheme.schemeCode,
                        schemeName = scheme.schemeName,
                        updateTag = updateTag
                    });
                    tmpDate = tmpDate.AddDays(1);
                }
                return schedule;

            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                List<myDailyTimeRecord> tmp = new List<myDailyTimeRecord>();
                return tmp;
            }

        }


        public List<myDailyTimeRecord> MyShiftSchemeList(string id, string code, int userLogNo, string EIC)
        {

            List<myDailyTimeRecord> schedule = new List<myDailyTimeRecord>();

            try
            {
                string[] text = code.Split(' ');
                DateTime baseMonth = Convert.ToDateTime(text[0].Trim() + " 1, " + text[1].Trim());
                DateTime lastDay = new DateTime(baseMonth.Year, baseMonth.Month, 1).AddMonths(1).AddDays(-1);

                //WORKS SCHEDULE
                IEnumerable<tAttWorkSchedDetail> schemes = db.tAttWorkSchedDetails.
                    Where(e => e.EIC == EIC && e.login.Value.Year == baseMonth.Year && e.login.Value.Month == baseMonth.Month).ToList();

                var temp = (from d in db.tAttWorkSchedDetails
                            join t in db.tAttWorkSchedTemplates on d.shiftTemplateId equals t.shiftTemplateId
                            select new
                            {
                                recNo = d.recNo,
                                EIC = d.EIC,
                                workSchedId = d.workSchedId,
                                login = d.login,
                                logout = d.logout,
                                shiftHour = d.shiftHour,
                                shiftName = t.shiftName
                            }).Where(e => e.EIC == EIC && e.login.Value.Year == baseMonth.Year && e.login.Value.Month == baseMonth.Month).ToList();

                IEnumerable<tAttLog> monthLogs = db.tAttLogs.
                    Where(e => e.userLogNo == userLogNo || e.EIC == EIC).Where(e => e.logDT.Value.Year == baseMonth.Year && e.logDT.Value.Month == baseMonth.Month && e.isActive != 0).ToList();



                for (DateTime tmpDate = baseMonth; tmpDate <= lastDay; )
                {


                    // vAttEmpSchemeDaily schm = temp.SingleOrDefault(e => e.logDate.Value.Day == tmpDate.Day);
                    var tempScheme = temp.SingleOrDefault(e => e.login.Value.Date == tmpDate.Date);

                    string tmpSchmCode = "";
                    string tmpSchmName = "";

                    if (tempScheme != null)
                    {
                        tmpSchmCode = tempScheme.shiftName;
                        tmpSchmName = tempScheme.shiftName;
                    }

                    IEnumerable<tAttLog> logs = monthLogs.Where(e => e.logDT.Value.Day == tmpDate.Day).ToList();

                    string tmpLogin = "", tmpLogout = "";

                    tAttWorkSchedDetail logScheme = new tAttWorkSchedDetail();



                    if (tempScheme != null)
                    {

                        logScheme.login = Convert.ToDateTime(tempScheme.login);
                        logScheme.logout = Convert.ToDateTime(tempScheme.logout);

                        TempDailyLog dayLog = ShiftingLog(tmpDate, logs, logScheme);

                        if (dayLog.login1.Year > 2000)
                        {
                            tmpLogin = dayLog.login1.ToString("MM/dd/yyyy hh:mm tt");
                        }

                        if (dayLog.logout1.Year > 2000)
                        {
                            tmpLogout = dayLog.logout1.ToString("MM/dd/yyyy hh:mm tt");
                        }


                    }


                    schedule.Add(new myDailyTimeRecord()
                    {
                        EIC = EIC,
                        logDate = tmpDate.Date,
                        logDay = tmpDate.ToString("ddd"),
                        login1 = tmpLogin,
                        logout1 = tmpLogout,

                        //schemeCode = scheme.schemeCode,
                        schemeName = tmpSchmName,
                        //updateTag = updateTag
                    });
                    tmpDate = tmpDate.AddDays(1);


                }



                return schedule;
            }
            catch (Exception)
            {

                return schedule;
            }
        }



        public List<myDailyTimeRecord> MyShiftSchemeList2(string id, string code, int userLogNo, string EIC)
        {
            try
            {
                string[] text = code.Split(' ');
                DateTime baseMonth = Convert.ToDateTime(text[0].Trim() + " 1, " + text[1].Trim());
                DateTime lastDay = new DateTime(baseMonth.Year, baseMonth.Month, 1).AddMonths(1).AddDays(-1);

                IEnumerable<vAttEmpSchemeDaily> dailyScheme = db.vAttEmpSchemeDailies.Where(e => e.EIC == id && e.logDate.Value.Year == baseMonth.Year & e.logDate.Value.Month == baseMonth.Month).ToList();

                List<myDailyTimeRecord> schedule = new List<myDailyTimeRecord>();

                //HRISv1 tAttDailyLog
                //HRISEntities hrDB = new HRISEntities();
                //IEnumerable<tAttDailyLog> dailyLog = hrDB.tAttDailyLogs.Where(e => e.EIC == id && e.LogDate.Value.Year == baseMonth.Year && e.LogDate.Value.Month == baseMonth.Month).ToList();
                IEnumerable<tAttWorkSchedDetail> dailyLog = db.tAttWorkSchedDetails.Where(e => e.EIC == id && e.login.Value.Year == baseMonth.Year && e.login.Value.Month == baseMonth.Month).ToList();

                //HRISv1 
                //IEnumerable<tAttShiftingEmpScheme> dayScheme = hrDB.tAttShiftingEmpSchemes.Where(e => e.EIC == id && e.entryDate.Value.Year == baseMonth.Year && e.entryDate.Value.Month == baseMonth.Month).OrderBy(o => o.entryDate).ToList();
                //HRISv2 
                IEnumerable<tAttLog> monthLogs = db.tAttLogs.Where(e => e.userLogNo == userLogNo || e.EIC == EIC).Where(e => e.logDT.Value.Year == baseMonth.Year && e.logDT.Value.Month == baseMonth.Month && e.isActive != 0).ToList();

                //IEnumerable<tAttLog> monthLogs = db.tAttLogs.Where(e => e.userLogNo == userLogNo && e.logDT.Value.Year == baseMonth.Year && e.logDT.Value.Month == baseMonth.Month).ToList();

                //HRISv2 REGULAR LOG SCHME
                tAttLogScheme regScheme = db.tAttLogSchemes.Single(e => e.schemeCode == "SCHM0001REG");


                for (DateTime tmpDate = baseMonth; tmpDate <= lastDay; )
                {
                    vAttEmpSchemeDaily schm = dailyScheme.SingleOrDefault(e => e.logDate.Value.Day == tmpDate.Day);

                    string tmpSchmCode = "";
                    string tmpSchmName = "";

                    if (schm != null)
                    {
                        tmpSchmCode = schm.schemeCode;
                        tmpSchmName = schm.schemeName;
                    }

                    string tmpLogin1 = "", tmpLogin2 = "", tmpLogOut1 = "", tmpLogOut2 = "";
                    //CHECK IF THERE IS SCHEME OF THE DAY                   
                    tAttShiftingEmpScheme tmpSchm = new tAttShiftingEmpScheme(); //dayScheme.FirstOrDefault(e => e.entryDate.Value.Day == tmpDate.Day);






                    tAttWorkSchedDetail attLog = dailyLog.SingleOrDefault(e => e.login.Value.Day == tmpDate.Day);

                    //CHECK IF HAS LOG OF THE DAY
                    IEnumerable<tAttLog> logs = monthLogs.Where(e => e.logDT.Value.Day == tmpDate.Day).ToList();

                    if (tmpDate.Day == 12)
                    {
                        string gi = "";
                    }

                    int updateTag = 0;
                    vAttEmpSchemeDaily scheme = new vAttEmpSchemeDaily();

                    //************* TEMPORARY ******************************
                    //if ((logs.Count() == 0) && tmpDate.Day == 21 && tmpDate.Month == 2 && tmpDate.Year == 2021)
                    //{
                    //    List<tAttLog> temp = new List<tAttLog>();
                    //    tAttLog n = new tAttLog();
                    //    n.logDT = tmpDate;
                    //    temp.Add(n);
                    //    logs = temp.ToList();
                    //}

                    if (logs.Count() > 0)
                    {
                        string myTempScheme = "";
                        if (tmpSchm == null) //NO SCHEME OF THE DAY
                        {
                            myTempScheme = "REGULAR (8AM-12NN && 1PM-5PM)";
                            scheme = RegularSchemeConverter(regScheme);
                            scheme.schemeCode = "SCHM0001REG";
                        }
                        else
                        {
                            myTempScheme = "SHIFTING";
                            myTempScheme = GetMyScheme(Convert.ToDateTime(tmpSchm.In1), Convert.ToDateTime(tmpSchm.Out1), Convert.ToDateTime(tmpSchm.In2), Convert.ToDateTime(tmpSchm.Out2));
                            scheme = SchemeConverter(tmpSchm);
                            scheme.schemeCode = myTempScheme;
                            scheme.schemeName = myTempScheme;
                        }

                        if (tmpDate.Day == 20)
                        {
                            updateTag = 0;
                        }

                        if (tmpDate.Day == 21)
                        {
                            updateTag = 1;
                        }

                        //LOG TO DTR : DAILY LOG
                        //MY DAILY LOG
                        TempDailyLog dayLog = DailyLog(tmpDate, logs, scheme);
                        tmpLogin1 = dayLog.login1.Year == 1 ? "" : string.Format("{0:hh:mm:ss tt}", dayLog.login1);
                        tmpLogOut1 = dayLog.logout1.Year == 1 ? "" : string.Format("{0:hh:mm:ss tt}", dayLog.logout1);
                        tmpLogin2 = dayLog.login2.Year == 1 ? "" : string.Format("{0:hh:mm:ss tt}", dayLog.login2);
                        tmpLogOut2 = dayLog.logout2.Year == 1 ? "" : string.Format("{0:hh:mm:ss tt}", dayLog.logout2);


                        updateTag = 0;

                        //string testLogin1 = string.Format("{0:hh:mm:ss tt}", attLog.In1);
                        //string testLogOut1 = string.Format("{0:hh:mm:ss tt}", attLog.Out1);
                        //string testLogin2 = string.Format("{0:hh:mm:ss tt}", attLog.In2);
                        //string testLogOut2 = string.Format("{0:hh:mm:ss tt}", attLog.Out2);


                        //if (testLogin1 != tmpLogin1)
                        //{
                        //    updateTag = 1;
                        //}
                        //if (testLogOut1 != tmpLogOut1)
                        //{
                        //    updateTag = 1;
                        //}
                        //if (testLogin2 != tmpLogin2)
                        //{
                        //    updateTag = 1;
                        //}
                        //if (testLogOut2 != tmpLogOut2)
                        //{
                        //    updateTag = 1;
                        //}

                        if (attLog != null)
                        {
                            updateTag = 0;
                            //string testLogin1 = string.Format("{0:hh:mm:ss tt}", attLog.In1);
                            //string testLogOut1 = string.Format("{0:hh:mm:ss tt}", attLog.Out1);
                            //string testLogin2 = string.Format("{0:hh:mm:ss tt}", attLog.In2);
                            //string testLogOut2 = string.Format("{0:hh:mm:ss tt}", attLog.Out2);
                            //if (testLogin1 != tmpLogin1)
                            //{
                            //    updateTag = 1;
                            //}
                            //if (testLogOut1 != tmpLogOut1)
                            //{
                            //    updateTag = 1;
                            //}
                            //if (testLogin2 != tmpLogin2)
                            //{
                            //    updateTag = 1;
                            //}
                            //if (testLogOut2 != tmpLogOut2)
                            //{
                            //    updateTag = 1;
                            //}
                        }
                        else
                        {
                            updateTag = 1;
                        }
                    }
                    else
                    {
                        scheme.schemeCode = "";
                        scheme.schemeName = "";
                    }

                    schedule.Add(new myDailyTimeRecord()
                    {
                        EIC = id,
                        logDate = tmpDate.Date,
                        logDay = tmpDate.ToString("ddd"),
                        login1 = tmpLogin1,
                        logout1 = tmpLogOut1,
                        login2 = tmpLogin2,
                        logout2 = tmpLogOut2,
                        schemeCode = scheme.schemeCode,
                        schemeName = scheme.schemeName,
                        updateTag = updateTag
                    });
                    tmpDate = tmpDate.AddDays(1);
                }
                return schedule;

            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                List<myDailyTimeRecord> tmp = new List<myDailyTimeRecord>();
                return tmp;
            }

        }




        private vAttEmpSchemeDaily SchemeConverter(tAttShiftingEmpScheme scheme)
        {
            vAttEmpSchemeDaily tempSchme = new vAttEmpSchemeDaily();

            if (scheme.In1 != null)
            {
                tempSchme.in1 = Convert.ToDateTime(scheme.In1).ToString("hh:mm tt");
                tempSchme.validIn1_minsF = -60;
                tempSchme.validIn1_minsT = 0;
                //-----------------------------
                tempSchme.maxLogIn1_minF = -180;
                tempSchme.maxLogIn1_minT = 180;
            }
            if (scheme.Out1 != null)
            {
                tempSchme.out1 = Convert.ToDateTime(scheme.Out1).ToString("hh:mm tt");
                tempSchme.validOut1_minsF = 0;
                tempSchme.validOut1_minsT = 120;
                //-----------------------------
                tempSchme.maxLogOut1_minF = -180;
                tempSchme.maxLogOut1_minT = 300;
            }
            if (scheme.In2 != null)
            {
                tempSchme.in2 = Convert.ToDateTime(scheme.In2).ToString("hh:mm tt");
                tempSchme.validIn2_minsF = -29;
                tempSchme.validIn2_minsT = 0;
                //-----------------------------
                tempSchme.maxLogIn2_minF = -180;
                tempSchme.maxLogIn2_minT = 120;
            }
            if (scheme.Out2 != null)
            {
                tempSchme.out2 = Convert.ToDateTime(scheme.Out2).ToString("hh:mm tt");
                tempSchme.validOut2_minsF = 0;
                tempSchme.validOut2_minsT = 120;
                //-----------------------------
                tempSchme.maxLogOut2_minF = -200;
                tempSchme.maxLogOut2_minT = 300;
            }
            tempSchme.schemeTypeCode = 1;
            return tempSchme;
        }


        private vAttEmpSchemeDaily RegularSchemeConverter(tAttLogScheme regScheme)
        {
            vAttEmpSchemeDaily scheme = new vAttEmpSchemeDaily();

            scheme.schemeCode = regScheme.schemeDetails;
            scheme.schemeName = regScheme.schemeName;
            //REG IN1
            scheme.in1 = regScheme.in1;
            scheme.validIn1_minsF = regScheme.validIn1_minsF;
            scheme.validIn1_minsT = regScheme.validIn1_minsT;
            //MAX IN1
            scheme.maxLogIn1_minF = regScheme.maxLogIn1_minF;
            scheme.maxLogIn1_minT = regScheme.maxLogIn1_minT;

            //OUT1
            scheme.out1 = regScheme.out1;
            scheme.validOut1_minsF = regScheme.validOut1_minsF;
            scheme.validOut1_minsT = regScheme.validOut1_minsT;
            //MAX OUT1
            scheme.maxLogOut1_minF = regScheme.maxLogOut1_minF;
            scheme.maxLogOut1_minT = regScheme.maxLogOut1_minT;

            //IN2
            scheme.in2 = regScheme.in2;
            scheme.validIn2_minsF = regScheme.validIn2_minsF;
            scheme.validIn2_minsT = regScheme.validIn2_minsT;
            //MAX IN2
            scheme.maxLogIn2_minF = regScheme.maxLogIn2_minF;
            scheme.maxLogIn2_minT = regScheme.maxLogIn2_minT;

            //OUT2
            scheme.out2 = regScheme.out2;
            scheme.validOut2_minsF = regScheme.validOut2_minsF;
            scheme.validOut2_minsT = regScheme.validOut2_minsT;

            //MAX OUT2
            scheme.maxLogOut2_minF = regScheme.maxLogOut2_minF;
            scheme.maxLogOut2_minT = regScheme.maxLogOut2_minT;
            scheme.schemeTypeCode = 0;

            return scheme;

        }

        private string GetMyScheme(DateTime login1, DateTime logout1, DateTime login2, DateTime logout2)
        {
            string scheme = "";
            string schemeA = "";
            string schemeB = "";
            if (login1.Year != 1 && logout1.Year != 1)
            {
                schemeA = login1.ToString("hh:mm tt") + " - " + logout1.ToString("hh:mm tt");
            }
            if (login2.Year != 1 && logout2.Year != 1)
            {
                schemeB = login2.ToString("hh:mm tt") + " - " + logout2.ToString("hh:mm tt");
            }
            if (schemeA != "")
            {
                scheme = schemeA;
            }
            if (schemeB != "")
            {
                scheme = schemeA + " " + schemeB;
            }
            return scheme.ToUpper().Trim();
        }


        [HttpPost]
        public JsonResult ViewSchedule(string id, string code)
        {
            try
            {
                tRSPEmployee emp = db.tRSPEmployees.Single(e => e.EIC == id);

                int userLogNo = Convert.ToInt16(emp.idNo);

                vRSPWorkGroupEmp empGroup = db.vRSPWorkGroupEmps.Single(e => e.EIC == id);

                if (empGroup.shiftTag >= 1)
                {
                    List<myDailyTimeRecord> myList = MyShiftSchemeList(id, code, userLogNo, emp.EIC);
                    return Json(new { status = "success", list = myList }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    List<myDailyTimeRecord> myList = MySchemeList(id, code, userLogNo, emp.EIC);
                    return Json(new { status = "success", list = myList }, JsonRequestBehavior.AllowGet);
                }


            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult GetSchemeAndLogs(string id, string code)
        {
            try
            {
                tRSPEmployee emp = db.tRSPEmployees.Single(e => e.EIC == id);
                int userLogNo = Convert.ToInt16(emp.idNo);
                DateTime dt = Convert.ToDateTime(code);

                string tmpDay = dt.ToString("dddd, dd MMMM yyyy");

                var logs = db.tAttLogs.Select(e => new
                {
                    e.userLogNo,
                    e.logDT
                }).Where(e => e.userLogNo == userLogNo && e.logDT.Value.Year == dt.Year & e.logDT.Value.Month == dt.Month && e.logDT.Value.Day == dt.Day).OrderBy(o => o.logDT).ToList();

                tAttEmpSchemeDaily schm = db.tAttEmpSchemeDailies.SingleOrDefault(e => e.EIC == id && e.logDate.Value.Year == dt.Year && e.logDate.Value.Month == dt.Month && e.logDate.Value.Day == dt.Day);

                string scheme = null;
                if (schm != null)
                {
                    scheme = schm.schemeCode;
                }

                return Json(new { status = "success", logDay = tmpDay, logList = logs, schemeCode = scheme }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetMySchemeOfTheDay(string id, string code)
        {
            try
            {
                DateTime dt = Convert.ToDateTime(code);
                string tmpDay = dt.ToString("dddd, dd MMMM yyyy");

                //CHECK IF 
                tAttWorkSchedDetail temp = db.tAttWorkSchedDetails.SingleOrDefault(e => e.login.Value.Year == dt.Year && e.login.Value.Month == dt.Month && e.login.Value.Day == dt.Day);

                string templateId = "NONE";
                if (temp != null)
                {
                    templateId = temp.shiftTemplateId;
                }


                return Json(new { status = "success", logDay = tmpDay, shiftTemplateId = templateId }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UpdateSchemeOfTheDay(string id, string month, string code)
        {
            try
            {


                DateTime dt = Convert.ToDateTime(month);
                string tmpDay = dt.ToString("dddd, dd MMMM yyyy");

                tAttWorkSchedDetail temp = db.tAttWorkSchedDetails.SingleOrDefault(e => e.EIC == id && e.login.Value.Year == dt.Year && e.login.Value.Month == dt.Month && e.login.Value.Day == dt.Day);

                if (temp == null)
                {
                    if (code.ToUpper() != "NONE")
                    {
                        tAttWorkSchedTemplate schm = db.tAttWorkSchedTemplates.Single(e => e.shiftTemplateId == code);
                        DateTime loginDT = Convert.ToDateTime(dt.ToString("MM/dd/yyyy") + " " + schm.loginTime);
                        DateTime logoutDT = loginDT.AddHours(Convert.ToInt16(schm.hours));
                        tAttWorkSchedDetail s = new tAttWorkSchedDetail();
                        //s.workSchedId = data.workSchedId;
                        s.EIC = id;
                        s.login = loginDT;
                        s.logout = logoutDT;
                        s.shiftHour = schm.hours;
                        s.shiftTemplateId = schm.shiftTemplateId;
                        db.tAttWorkSchedDetails.Add(s);
                        db.SaveChanges();                      
                    }

                    return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (code.ToUpper() == "NONE")
                    {
                        db.tAttWorkSchedDetails.Remove(temp);
                        db.SaveChanges();
                        return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        tAttWorkSchedTemplate schm = db.tAttWorkSchedTemplates.Single(e => e.shiftTemplateId == code);
                        DateTime loginDT = Convert.ToDateTime(dt.ToString("MM/dd/yyyy") + " " + schm.loginTime);
                        DateTime logoutDT = loginDT.AddHours(Convert.ToInt16(schm.hours));                      
                        //s.workSchedId = data.workSchedId;
                        temp.login = loginDT;
                        temp.logout = logoutDT;
                        temp.shiftHour = schm.hours;
                        temp.shiftTemplateId = schm.shiftTemplateId;
                        db.SaveChanges();
                        return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
                    }
                }




              
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult SubmitSchemeSchedule(string id, string code, string tag)
        {
            try
            {
                DateTime dt = Convert.ToDateTime(code);
                tAttEmpSchemeDaily schm = db.tAttEmpSchemeDailies.SingleOrDefault(e => e.EIC == id && e.logDate.Value.Year == dt.Year && e.logDate.Value.Month == dt.Month && e.logDate.Value.Day == dt.Day);
                if (schm == null)
                {
                    tAttEmpSchemeDaily newSchm = new tAttEmpSchemeDaily();
                    newSchm.EIC = id;
                    newSchm.logDate = dt;
                    newSchm.schemeCode = tag;
                    newSchm.userEIC = Session["_EIC"].ToString();
                    newSchm.transDT = DateTime.Now;
                    db.tAttEmpSchemeDailies.Add(newSchm);
                    db.SaveChanges();
                }
                else
                {
                    schm.schemeCode = tag;
                    schm.userEIC = Session["_EIC"].ToString();
                    schm.transDT = DateTime.Now;
                    db.SaveChanges();
                }
                string tmpCode = dt.ToString("MMMM") + " " + dt.Year;
                //List<myDailyTimeRecord> list = MySchemeList(id, tmpCode); //code format: January 2020
                //return Json(new { status = "success", monthSched = list }, JsonRequestBehavior.AllowGet);
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }



        [HttpPost]
        public JsonResult MoveToNextDay(string id, string code)
        {
            try
            {
                tRSPEmployee emp = db.tRSPEmployees.Single(e => e.EIC == id);
                int userLogNo = Convert.ToInt16(emp.idNo);

                DateTime dt = Convert.ToDateTime(code);
                DateTime dtTemp = dt;
                dt = dt.AddDays(1);

                if (dt.Month != dtTemp.Month)
                {
                    return Json(new { status = "Invalid month!" }, JsonRequestBehavior.AllowGet);
                }
                string tmpDay = dt.ToString("dddd, dd MMMM yyyy");
                var logs = db.tAttLogs.Select(e => new
                {
                    e.userLogNo,
                    e.logDT
                }).Where(e => e.userLogNo == userLogNo && e.logDT.Value.Year == dt.Year & e.logDT.Value.Month == dt.Month && e.logDT.Value.Day == dt.Day).OrderBy(o => o.logDT).ToList();

                tAttEmpSchemeDaily schm = db.tAttEmpSchemeDailies.SingleOrDefault(e => e.EIC == id && e.logDate.Value.Year == dt.Year && e.logDate.Value.Month == dt.Month && e.logDate.Value.Day == dt.Day);

                string scheme = null;
                if (schm != null)
                {
                    scheme = schm.schemeCode;
                }

                return Json(new { status = "success", logDay = tmpDay, logList = logs, schemeCode = scheme }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult MoveToPreviousDay(string id, string code)
        {
            try
            {
                tRSPEmployee emp = db.tRSPEmployees.Single(e => e.EIC == id);
                int userLogNo = Convert.ToInt16(emp.idNo);

                DateTime dt = Convert.ToDateTime(code);
                DateTime dtTemp = dt;
                dt = dt.AddDays(-1);

                if (dt.Month != dtTemp.Month)
                {
                    return Json(new { status = "Invalid month!" }, JsonRequestBehavior.AllowGet);
                }
                string tmpDay = dt.ToString("dddd, dd MMMM yyyy");
                var logs = db.tAttLogs.Select(e => new
                {
                    e.userLogNo,
                    e.logDT
                }).Where(e => e.userLogNo == userLogNo && e.logDT.Value.Year == dt.Year & e.logDT.Value.Month == dt.Month && e.logDT.Value.Day == dt.Day).OrderBy(o => o.logDT).ToList();

                tAttEmpSchemeDaily schm = db.tAttEmpSchemeDailies.SingleOrDefault(e => e.EIC == id && e.logDate.Value.Year == dt.Year && e.logDate.Value.Month == dt.Month && e.logDate.Value.Day == dt.Day);

                string scheme = null;
                if (schm != null)
                {
                    scheme = schm.schemeCode;
                }

                return Json(new { status = "success", logDay = tmpDay, logList = logs, schemeCode = scheme }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        //REFRESH LOG

        [HttpPost]
        public JsonResult GenerateDailyLog(string id, string code)
        {
            try
            {

                HRISEntities hrDB = new HRISEntities();
                DateTime dt = Convert.ToDateTime(code);
                DateTime tmpCutOff = Convert.ToDateTime("January 11, 2021");

                if (dt.Date < tmpCutOff.Date)
                {
                    return Json(new { status = "Invalid date!" }, JsonRequestBehavior.AllowGet);
                }

                tRSPEmployee emp = db.tRSPEmployees.Single(e => e.EIC == id);

                vAttEmpSchemeDaily scheme = new vAttEmpSchemeDaily();
                tAttShiftingEmpScheme tmpSchm = hrDB.tAttShiftingEmpSchemes.FirstOrDefault(e => e.EIC == id && e.entryDate.Value.Year == dt.Year && e.entryDate.Value.Month == dt.Month && e.entryDate.Value.Day == dt.Day);

                if (tmpSchm != null)
                {
                    if (tmpSchm.In1 != null && tmpSchm.In2 != null)
                    {
                        tAttLogScheme regScheme = db.tAttLogSchemes.Single(e => e.schemeCode == "SCHM0001REG");
                        scheme = RegularSchemeConverter(regScheme);
                    }
                    else
                    {
                        scheme = SchemeConverter(tmpSchm);
                    }
                }
                else
                {
                    tAttLogScheme regScheme = db.tAttLogSchemes.Single(e => e.schemeCode == "SCHM0001REG");
                    scheme = RegularSchemeConverter(regScheme);
                }

                int userLogNo = Convert.ToInt16(emp.idNo);
                IEnumerable<tAttLog> logs = db.tAttLogs.Where(e => e.userLogNo == userLogNo || e.EIC == emp.EIC).Where(e => e.logDT.Value.Year == dt.Year && e.logDT.Value.Month == dt.Month && e.logDT.Value.Day == dt.Day && e.isActive == null).ToList();

                //CHECK IF THERE WHERE WORK FROM HOME TAG
                int wfhLogCount = logs.Where(e => e.tag == 0).Count();

                //FIND MY LOG *****************************************************
                TempDailyLog myLog = DailyLog(dt, logs, scheme);
                myLog.EIC = emp.EIC;

                tAttDailyLog dailyLog = hrDB.tAttDailyLogs.SingleOrDefault(e => e.EIC == emp.EIC && e.LogDate.Value.Year == dt.Year && e.LogDate.Value.Month == dt.Month && e.LogDate.Value.Day == dt.Day);

                if (dailyLog != null)
                {
                    //UPDATE                    
                    dailyLog.In1 = null;
                    dailyLog.Out1 = null;
                    dailyLog.In2 = null;
                    dailyLog.Out2 = null;
                    dailyLog.tag = null;
                    if (myLog.login1.Year > 1)
                    {
                        dailyLog.In1 = myLog.login1;
                    }
                    if (myLog.logout1.Year > 1)
                    {
                        dailyLog.Out1 = myLog.logout1;
                    }
                    if (myLog.login2.Year > 1)
                    {
                        dailyLog.In2 = myLog.login2;
                    }
                    if (myLog.logout2.Year > 1)
                    {
                        dailyLog.Out2 = myLog.logout2;
                    }
                    if (wfhLogCount >= 1)
                    {
                        dailyLog.tag = 0;
                    }

                    dailyLog.SchemeCode = scheme.schemeCode; //SCHEME CODE OF VERSION 1 : SCHEME DETAILS IN VERSION 2
                    hrDB.SaveChanges();
                }
                else
                {
                    tAttDailyLog n = new tAttDailyLog();
                    n.EIC = emp.EIC;
                    n.LogDate = dt.Date;
                    if (myLog.login1.Year > 1)
                    {
                        n.In1 = myLog.login1;
                    }
                    if (myLog.logout1.Year > 1)
                    {
                        n.Out1 = myLog.logout1;
                    }
                    if (myLog.login2.Year > 1)
                    {
                        n.In2 = myLog.login2;
                    }
                    if (myLog.logout2.Year > 1)
                    {
                        n.Out2 = myLog.logout2;
                    }
                    n.SchemeCode = scheme.schemeDetails;
                    if (wfhLogCount >= 1)
                    {
                        n.tag = 0;
                    }
                    hrDB.tAttDailyLogs.Add(n);
                    hrDB.SaveChanges();
                }
                string tmpCode = dt.ToString("MMMM") + " " + dt.Year;

                //   List<myDailyTimeRecord> myList = MySchemeList(id, code, userLogNo);    
                List<myDailyTimeRecord> list = MySchemeList(id, tmpCode, Convert.ToInt16(emp.idNo), emp.EIC); //code format: January 2020
                return Json(new { status = "success", monthSched = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        public class TempDailyLog
        {
            public string EIC { get; set; }
            public DateTime logDate { get; set; }
            public DateTime login1 { get; set; }
            public DateTime logout1 { get; set; }
            public DateTime login2 { get; set; }
            public DateTime logout2 { get; set; }
        }

        public TempDailyLog ShiftingLog(DateTime LogDate, IEnumerable<tAttLog> logs, tAttWorkSchedDetail scheme)
        {
            TempDailyLog myLog = new TempDailyLog();

            if (LogDate.Day == 3)
            {
                string s = "";
            }

            //IF NO LOGS
            if (logs.Count() == 0)
            {
                return myLog;
            }

            logs = logs.OrderBy(o => o.logDT).ToList();

            string tmpLogIn = "";
            tmpLogIn = FindMyShiftLogIn(LogDate, logs, scheme);

            if (tmpLogIn != "")
            {
                myLog.login1 = Convert.ToDateTime(tmpLogIn);
            }

            string tmpLogOut = "";
            tmpLogOut = FindMyShiftLogOut(LogDate, logs, scheme);
            if (tmpLogOut != "")
            {
                myLog.logout1 = Convert.ToDateTime(tmpLogOut);
            }





            return myLog;

        }

        public string FindMyShiftLogIn(DateTime logDate, IEnumerable<tAttLog> logs, tAttWorkSchedDetail scheme)
        {
            string res = "";


            DateTime validLogFrom = Convert.ToDateTime(scheme.login).AddMinutes(-40);
            DateTime validLogTo = Convert.ToDateTime(scheme.login).AddMinutes(60);



            IEnumerable<tAttLog> myLogs = logs.Where(e => e.logDT >= validLogFrom && e.logDT < validLogTo).ToList();

            foreach (var log in myLogs)
            {
                res = string.Format("{0:MM/dd/yyyy hh:mm:ss tt}", log.logDT);
                return res;
            }

            return res;

        }

        public string FindMyShiftLogOut(DateTime logDate, IEnumerable<tAttLog> logs, tAttWorkSchedDetail scheme)
        {
            string res = "";


            DateTime validLogFrom = Convert.ToDateTime(scheme.logout).AddMinutes(-40);
            DateTime validLogTo = Convert.ToDateTime(scheme.logout).AddMinutes(60);



            IEnumerable<tAttLog> myLogs = logs.Where(e => e.logDT >= validLogFrom && e.logDT < validLogTo).OrderByDescending(o => o.logDT).ToList();

            foreach (var log in myLogs)
            {
                res = string.Format("{0:MM/dd/yyyy hh:mm:ss tt}", log.logDT);
                return res;
            }

            return res;

        }


        public TempDailyLog DailyLog(DateTime LogDate, IEnumerable<tAttLog> logs, vAttEmpSchemeDaily scheme)
        {
            TempDailyLog myLog = new TempDailyLog();

            if (LogDate.Day == 3)
            {
                string s = "";
            }

            //IF NO LOGS
            if (logs.Count() == 0)
            {
                return myLog;
            }

            logs = logs.OrderBy(o => o.logDT).ToList();

            string tmpLog = "";
            if (scheme.in1 != null && scheme.in1 != "")
            {
                //CHECK LOGIN 
                //REGULAR LOGIN
                tmpLog = FindMyLogIn(LogDate, logs, scheme, 1);
                //FIND MAX LOGIN
                if (tmpLog == "")
                {
                    tmpLog = FindMaxLogIn(LogDate, logs, scheme, 1);
                }
                if (tmpLog != "")
                {
                    myLog.login1 = Convert.ToDateTime(tmpLog);
                }
            }
            if (scheme.out1 != null && scheme.out1 != "")
            {
                //CHECK LOGOUT
                tmpLog = FinMyLogOut(LogDate, logs, scheme, 1);
                if (tmpLog == "")
                {
                    tmpLog = FindMaxLogOut(LogDate, logs, scheme, 1);
                }
                //LOG DETECTED FOR SPECIFIC SCHEM
                if (tmpLog != "")
                {
                    myLog.logout1 = Convert.ToDateTime(tmpLog);
                }
            }

            if (scheme.in2 != null && scheme.in2 != "")
            {
                //CHECK LOGIN
                tmpLog = FindMyLogIn(LogDate, logs, scheme, 2);

                if (tmpLog != "")
                {
                    string[] tmpVal = tmpLog.Split(';');
                    if (tmpVal.Count() == 1)
                    {
                        string a = tmpVal[0].ToString();
                        myLog.login2 = Convert.ToDateTime(a);

                    }
                    else if (tmpVal.Count() == 2)
                    {
                        string a = tmpVal[0].ToString();
                        string b = tmpVal[1].ToString();

                        if (myLog.logout1.Year == 1 || myLog.logout1 == null)
                        {
                            myLog.logout1 = Convert.ToDateTime(a);
                            myLog.login2 = Convert.ToDateTime(b);
                        }
                        else
                        {
                            myLog.login2 = Convert.ToDateTime(a);
                        }

                    }
                }

                if (scheme.schemeTypeCode == 0)
                {
                    //FIND MAX LOG
                    if (scheme.in2 != null)
                    {
                        tmpLog = ""; //{}
                        if (myLog.login2.Year == 1 || myLog.login2 == null)
                        {
                            tmpLog = FindMaxLogIn2(LogDate, logs, scheme, 2);
                        }
                    }

                    if (tmpLog != "")
                    {
                        myLog.login2 = Convert.ToDateTime(tmpLog);
                    }
                }
                else
                {
                    if (scheme.schemeTypeCode == 1)
                    {
                        //FIND MAX LOG
                        if (scheme.in2 != null)
                        {
                            tmpLog = FindMaxLogIn2(LogDate, logs, scheme, 2);
                        }

                        if (tmpLog != "")
                        {
                            myLog.login2 = Convert.ToDateTime(tmpLog);
                        }
                    }
                }
            }

            if (scheme.out2 != null && scheme.out2 != "")
            {
                //CHECK LOGOUT
                tmpLog = FinMyLogOut(LogDate, logs, scheme, 2);
                if (tmpLog == "")
                {
                    tmpLog = FindMaxLogOut(LogDate, logs, scheme, 2);
                }
                if (tmpLog != "")
                {
                    myLog.logout2 = Convert.ToDateTime(tmpLog);
                }
            }


            // NO AM LOGIN & WITH AM SHIFTING SCHEDULE
            if (myLog.login1.Year == 1 && scheme.in1 != null && scheme.out1 != null)
            {
                //TARDY CHECKER
                if (myLog.login1.Year == 1 && myLog.logout1.Year > 2000)
                {
                    //TARDY                     
                    tmpLog = FinMyTardyLogin(LogDate, logs, scheme, myLog.logout1, "AM");
                    if (tmpLog != "")
                    {
                        myLog.login1 = Convert.ToDateTime(tmpLog);
                    }
                }
            }

            // NO PM LOGIN & WITH AM SHIFTING SCHEDULE
            if (myLog.login2.Year == 1 && scheme.in2 != null && scheme.out2 != null)
            {
                //TARDY CHECKER
                if (myLog.login2.Year == 1 && myLog.logout2.Year > 2000)
                {
                    //TARDY                   
                    tmpLog = FinMyTardyLogin(LogDate, logs, scheme, myLog.logout2, "PM");
                    if (tmpLog != "")
                    {

                        string tmpMaxLogin2 = Convert.ToDateTime(tmpLog).ToString("MM/dd/yyyy HH:mm");
                        string tmpMaxLogOut2 = Convert.ToDateTime(myLog.logout2).ToString("MM/dd/yyyy HH:mm");

                        if (tmpMaxLogin2 != tmpMaxLogOut2)
                        {
                            myLog.login2 = Convert.ToDateTime(tmpLog);
                        }

                    }
                }
            }

            // NO AM LOGOUT W/ AM SCHEDULE
            if (myLog.logout1.Year == 1 && scheme.in1 != null && scheme.out1 != null)
            {
                if (myLog.login1.Year != 1 && myLog.logout1.Year == 1)
                {
                    tmpLog = FinMyUnderTimeLogOut(LogDate, logs, scheme, myLog.login1, "AM");
                    if (tmpLog != "")
                    {
                        myLog.logout1 = Convert.ToDateTime(tmpLog);
                    }
                }
            }

            // NO AM LOGOUT W/ AM SCHEDULE
            if (myLog.logout2.Year == 1 && scheme.in2 != null && scheme.out2 != null)
            {
                if (myLog.login2.Year != 1 && myLog.logout2.Year == 1)
                {
                    tmpLog = FinMyUnderTimeLogOut(LogDate, logs, scheme, myLog.login2, "PM");
                    if (tmpLog != "")
                    {
                        myLog.logout2 = Convert.ToDateTime(tmpLog);
                    }
                }
            }

            return myLog;
        }

        public string FindMyLogIn(DateTime logDate, IEnumerable<tAttLog> logs, vAttEmpSchemeDaily scheme, int schemeTag)
        {
            string res = "";

            string schemeLog = "";
            double validMinsFrom = 0;
            double validMinsTo = 0;
            if (schemeTag == 1)
            {
                schemeLog = scheme.in1;
                validMinsFrom = Convert.ToDouble(scheme.validIn1_minsF);
                validMinsTo = Convert.ToDouble(scheme.validIn1_minsT);
            }
            else if (schemeTag == 2)
            {
                schemeLog = scheme.in2;
                validMinsFrom = Convert.ToDouble(scheme.validIn2_minsF);
                validMinsTo = Convert.ToDouble(scheme.validIn2_minsT);
            }

            DateTime validLogFrom = Convert.ToDateTime(logDate.ToString("MMMM dd, yyyy") + " " + schemeLog);
            DateTime validLogTo = Convert.ToDateTime(logDate.ToString("MMMM dd, yyyy") + " " + schemeLog);

            validLogFrom = validLogFrom.AddMinutes(validMinsFrom);
            validLogTo = validLogTo.AddMinutes(validMinsTo);

            string tempLogA = "";
            string tempLogB = "";

            IEnumerable<tAttLog> myLogs = logs.Where(e => e.logDT >= validLogFrom && e.logDT < validLogTo).ToList();
            foreach (var log in myLogs)
            {

                if (schemeTag == 1)
                {
                    res = string.Format("{0:MM/dd/yyyy hh:mm:ss tt}", log.logDT);
                    return res;
                }

                if (schemeTag == 2)
                {
                    if (scheme.schemeTypeCode == 0) //MEANS 8-5
                    {
                        res = string.Format("{0:MM/dd/yyyy hh:mm:ss tt}", log.logDT);

                        if (tempLogA == "")
                        {
                            tempLogA = res;
                        }
                        else
                        {
                            tempLogB = res;
                        }
                    }
                    else
                    {
                        res = string.Format("{0:MM/dd/yyyy hh:mm:ss tt}", log.logDT);
                        return res;
                    }
                }

                if (tempLogA.Length > 0 && tempLogB.Length > 0)
                {
                    res = tempLogA + ";" + tempLogB;
                    return res;
                }



            }
            return res;
        }

        public string FinMyLogOut(DateTime logDate, IEnumerable<tAttLog> logs, vAttEmpSchemeDaily scheme, int schemeTag)
        {
            string res = "";


            if (logDate.Day == 3)
            {
                res = "";
            }

            string schemeLog = "";
            double validMinsFrom = 0;
            double validMinsTo = 0;
            if (schemeTag == 1)
            {
                schemeLog = scheme.out1;
                validMinsFrom = Convert.ToDouble(scheme.validOut1_minsF);
                validMinsTo = Convert.ToDouble(scheme.validOut1_minsT);
            }
            else if (schemeTag == 2)
            {
                schemeLog = scheme.out2;
                validMinsFrom = Convert.ToDouble(scheme.validOut2_minsF);
                validMinsTo = Convert.ToDouble(scheme.validOut2_minsT);
            }

            DateTime validLogFrom = Convert.ToDateTime(logDate.ToString("MMMM dd, yyyy") + " " + schemeLog);
            DateTime validLogTo = Convert.ToDateTime(logDate.ToString("MMMM dd, yyyy") + " " + schemeLog);

            validLogFrom = validLogFrom.AddMinutes(validMinsFrom);
            validLogTo = validLogTo.AddMinutes(validMinsTo + 1);

            IEnumerable<tAttLog> myLogs = logs.Where(e => e.logDT >= validLogFrom && e.logDT < validLogTo).ToList();

            foreach (var log in myLogs)
            {
                res = string.Format("{0:MM/dd/yyyy hh:mm:ss tt}", log.logDT);
                return res;
            }
            return res;
        }


        public string FindTardyLogin_1(DateTime logDate, IEnumerable<tAttLog> logs, vAttEmpSchemeDaily scheme, DateTime logout)
        {
            string res = "";

            DateTime validLogFrom = Convert.ToDateTime(logDate.ToString("MMMM dd, yyyy") + " " + scheme.in1);
            DateTime validLogTo = Convert.ToDateTime(logDate.ToString("MMMM dd, yyyy") + " " + scheme.out1);



            if (logout > validLogTo)
            {
                validLogTo = logout;
                validLogTo = validLogTo.AddMinutes(-5);
            }
            else
            {
                validLogTo = validLogTo.AddMinutes(-5);
            }



            IEnumerable<tAttLog> myLogs = logs.Where(e => e.logDT > validLogFrom && e.logDT < validLogTo).ToList();
            foreach (var log in myLogs)
            {
                res = string.Format("{0:MM/dd/yyyy hh:mm:ss tt}", log.logDT);
                return res;
            }
            return res;

        }


        public string FinMyTardyLogin(DateTime logDate, IEnumerable<tAttLog> logs, vAttEmpSchemeDaily scheme, DateTime uLogFrom, string shift)
        {
            string res = "";

            if (logDate.Day == 12)
            {
                res = "";
            }

            string schm_Login = "";
            string schm_Logout = "";


            if (shift == "AM")
            {
                schm_Login = scheme.in1;
                schm_Logout = scheme.out1;
            }
            else if (shift == "PM")
            {
                schm_Login = scheme.in2;
                schm_Logout = scheme.out2;
            }

            DateTime validLogFrom = Convert.ToDateTime(logDate.ToString("MMMM dd, yyyy") + " " + schm_Login);
            DateTime validLogTo = Convert.ToDateTime(logDate.ToString("MMMM dd, yyyy") + " " + schm_Logout);


            if (shift == "AM")
            {
                if (uLogFrom > validLogTo)
                {
                    validLogTo = uLogFrom;
                    validLogTo = validLogTo.AddMinutes(-30);
                }
                else
                {
                    validLogTo = uLogFrom.AddMinutes(-30);
                }
            }
            else if (shift == "PM")
            {
                if (uLogFrom > validLogTo)
                {
                    validLogTo = uLogFrom;
                    validLogTo = validLogTo.AddMinutes(-30);
                }
                else
                {
                    validLogFrom = uLogFrom.AddMinutes(-30);
                }
            }


            IEnumerable<tAttLog> myLogs = logs.Where(e => e.logDT > validLogFrom && e.logDT < validLogTo).ToList();
            foreach (var log in myLogs)
            {
                res = string.Format("{0:MM/dd/yyyy hh:mm:ss tt}", log.logDT);
                return res;
            }
            return res;
        }


        public string FinMyUnderTimeLogOut(DateTime logDate, IEnumerable<tAttLog> logs, vAttEmpSchemeDaily scheme, DateTime uLogFrom, string shift)
        {
            string res = "";

            string schm_Login = "";
            string schm_Logout = "";

            if (shift == "AM")
            {
                schm_Login = scheme.in1;
                schm_Logout = scheme.out1;
            }
            else if (shift == "PM")
            {
                schm_Login = scheme.in2;
                schm_Logout = scheme.out2;
            }

            DateTime validLogFrom = Convert.ToDateTime(logDate.ToString("MMMM dd, yyyy") + " " + schm_Login);
            DateTime validLogTo = Convert.ToDateTime(logDate.ToString("MMMM dd, yyyy") + " " + schm_Logout);


            if (uLogFrom > validLogFrom)
            {
                validLogFrom = uLogFrom;
                validLogFrom = validLogFrom.AddMinutes(1);
            }
            else
            {
                validLogFrom = validLogFrom.AddMinutes(1);
            }

            IEnumerable<tAttLog> myLogs = logs.Where(e => e.logDT > validLogFrom && e.logDT < validLogTo).ToList();
            foreach (var log in myLogs)
            {
                res = string.Format("{0:MM/dd/yyyy hh:mm:ss tt}", log.logDT);
                return res;
            }
            return res;
        }

        public string FinMyUnderTimeLogOut_2(DateTime logDate, IEnumerable<tAttLog> logs, vAttEmpSchemeDaily scheme, DateTime uLogFrom)
        {
            string res = "";

            DateTime validLogFrom = Convert.ToDateTime(logDate.ToString("MMMM dd, yyyy") + " " + scheme.in2);
            DateTime validLogTo = Convert.ToDateTime(logDate.ToString("MMMM dd, yyyy") + " " + scheme.out2);

            if (uLogFrom > validLogFrom)
            {
                validLogFrom = uLogFrom;
                validLogFrom = validLogFrom.AddMinutes(1);
            }
            else
            {
                validLogFrom = validLogFrom.AddMinutes(1);
            }

            IEnumerable<tAttLog> myLogs = logs.Where(e => e.logDT > validLogFrom && e.logDT < validLogTo).ToList();
            foreach (var log in myLogs)
            {
                res = string.Format("{0:MM/dd/yyyy hh:mm:ss tt}", log.logDT);
                return res;
            }
            return res;
        }


        //MAX LOGIN 1
        public string FindMaxLogIn(DateTime logDate, IEnumerable<tAttLog> logs, vAttEmpSchemeDaily scheme, int schemeTag)
        {
            string res = "";

            string schemeLog = "";
            double validMinsFrom = 0;
            double validMinsTo = 0;
            if (schemeTag == 1)
            {
                schemeLog = scheme.in1;
                validMinsFrom = Convert.ToDouble(scheme.maxLogIn1_minF);
                validMinsTo = Convert.ToDouble(scheme.maxLogIn1_minT);
            }
            else if (schemeTag == 2)
            {
                schemeLog = scheme.in2;
                validMinsFrom = Convert.ToDouble(scheme.maxLogIn2_minF);
                validMinsTo = Convert.ToDouble(scheme.maxLogIn2_minT);
            }

            DateTime validLogFrom = Convert.ToDateTime(logDate.ToString("MMMM dd, yyyy") + " " + schemeLog);
            DateTime validLogTo = Convert.ToDateTime(logDate.ToString("MMMM dd, yyyy") + " " + schemeLog);

            validLogFrom = validLogFrom.AddMinutes(validMinsFrom);
            validLogTo = validLogTo.AddMinutes(validMinsTo);

            string tempLogA = "";
            string tempLogB = "";

            IEnumerable<tAttLog> myLogs = logs.Where(e => e.logDT >= validLogFrom && e.logDT < validLogTo).ToList();
            foreach (var log in myLogs)
            {

                if (schemeTag == 1)
                {
                    res = string.Format("{0:MM/dd/yyyy hh:mm:ss tt}", log.logDT);
                    return res;
                }

                if (schemeTag == 2)
                {
                    if (scheme.schemeTypeCode == 0) //MEANS 8-5
                    {
                        res = string.Format("{0:MM/dd/yyyy hh:mm:ss tt}", log.logDT);

                        if (tempLogA == "")
                        {
                            tempLogA = res;
                        }
                        else
                        {
                            tempLogB = res;
                        }
                    }
                    else
                    {
                        res = string.Format("{0:MM/dd/yyyy hh:mm:ss tt}", log.logDT);
                        return res;
                    }
                }

                if (tempLogA.Length > 0 && tempLogB.Length > 0)
                {
                    res = tempLogA + ";" + tempLogB;
                    return res;
                }



            }
            return res;
        }

        //MAX LOGIN
        public string FindMaxLogIn2(DateTime logDate, IEnumerable<tAttLog> logs, vAttEmpSchemeDaily scheme, int schemeTag)
        {
            string res = "";

            string schemeLog = "";
            double validMinsFrom = 0;
            double validMinsTo = 0;

            schemeLog = scheme.in2;
            validMinsFrom = Convert.ToDouble(scheme.maxLogIn2_minF);
            validMinsTo = Convert.ToDouble(scheme.maxLogIn2_minT);

            DateTime validLogFrom = Convert.ToDateTime(logDate.ToString("MMMM dd, yyyy") + " " + schemeLog);
            DateTime validLogTo = Convert.ToDateTime(logDate.ToString("MMMM dd, yyyy") + " " + schemeLog);

            validLogFrom = validLogFrom.AddMinutes(validMinsFrom);
            validLogTo = validLogTo.AddMinutes(validMinsTo);

            IEnumerable<tAttLog> myLogs = logs.Where(e => e.logDT >= validLogFrom && e.logDT < validLogTo).ToList();
            foreach (var log in myLogs)
            {
                res = string.Format("{0:MM/dd/yyyy hh:mm:ss tt}", log.logDT);
                return res;
            }
            return res;
        }




        ////MAX LOGIN
        //public string FindMaxLogOut(DateTime logDate, IEnumerable<tAttLog> logs, vAttEmpSchemeDaily scheme, int schemeTag)
        //{
        //    string res = "";

        //    string schemeLog = "";
        //    double validMinsFrom = 0;
        //    double validMinsTo = 0;
        //    if (schemeTag == 1)
        //    {
        //        schemeLog = scheme.out1;
        //        validMinsFrom = Convert.ToDouble(scheme.maxLogOut1_minF);
        //        validMinsTo = Convert.ToDouble(scheme.maxLogOut1_minT);
        //    }
        //    else if (schemeTag == 2)
        //    {
        //        schemeLog = scheme.out1;
        //        validMinsFrom = Convert.ToDouble(scheme.maxLogOut2_minF);
        //        validMinsTo = Convert.ToDouble(scheme.maxLogOut2_minT);
        //    }

        //    DateTime validLogFrom = Convert.ToDateTime(logDate.ToString("MMMM dd, yyyy") + " " + schemeLog);
        //    DateTime validLogTo = Convert.ToDateTime(logDate.ToString("MMMM dd, yyyy") + " " + schemeLog);

        //    validLogFrom = validLogFrom.AddMinutes(validMinsFrom);
        //    validLogTo = validLogTo.AddMinutes(validMinsTo);

        //    string tempLogA = "";
        //    string tempLogB = "";

        //    IEnumerable<tAttLog> myLogs = logs.Where(e => e.logDT >= validLogFrom && e.logDT < validLogTo).ToList();
        //    foreach (var log in myLogs)
        //    {

        //        if (schemeTag == 1)
        //        {
        //            res = string.Format("{0:MM/dd/yyyy hh:mm:ss tt}", log.logDT);
        //            return res;
        //        }

        //        if (schemeTag == 2)
        //        {
        //            if (scheme.schemeTypeCode == 0) //MEANS 8-5
        //            {
        //                return "";
        //                //res = string.Format("{0:MM/dd/yyyy hh:mm:ss tt}", log.logDT);

        //                //if (tempLogA == "")
        //                //{
        //                //    tempLogA = res;
        //                //}
        //                //else
        //                //{
        //                //    tempLogB = res;
        //                //}
        //            }
        //            else
        //            {
        //                res = string.Format("{0:MM/dd/yyyy hh:mm:ss tt}", log.logDT);
        //                return res;
        //            }
        //        }

        //        if (tempLogA.Length > 0 && tempLogB.Length > 0)
        //        {
        //            res = tempLogA + ";" + tempLogB;
        //            return res;
        //        }



        //    }
        //    return res;
        //}

        //MAX LOGIN
        public string FindMaxLogOut(DateTime logDate, IEnumerable<tAttLog> logs, vAttEmpSchemeDaily scheme, int schemeTag)
        {
            string res = "";

            string schemeLog = "";
            double validMinsFrom = 0;
            double validMinsTo = 0;
            if (schemeTag == 1)
            {
                schemeLog = scheme.out1;
                validMinsFrom = Convert.ToDouble(scheme.maxLogOut1_minF);
                validMinsTo = Convert.ToDouble(scheme.maxLogOut1_minT);
            }
            else if (schemeTag == 2)
            {
                schemeLog = scheme.out2;
                validMinsFrom = Convert.ToDouble(scheme.maxLogOut2_minF);
                validMinsTo = Convert.ToDouble(scheme.maxLogOut2_minT);
            }

            DateTime validLogFrom = Convert.ToDateTime(logDate.ToString("MMMM dd, yyyy") + " " + schemeLog);
            DateTime validLogTo = Convert.ToDateTime(logDate.ToString("MMMM dd, yyyy") + " " + schemeLog);

            validLogFrom = validLogFrom.AddMinutes(validMinsFrom);
            validLogTo = validLogTo.AddMinutes(validMinsTo + 1);

            IEnumerable<tAttLog> myLogs = logs.Where(e => e.logDT >= validLogFrom && e.logDT < validLogTo).ToList();
            foreach (var log in myLogs)
            {

                res = string.Format("{0:MM/dd/yyyy hh:mm:ss tt}", log.logDT);
                return res;

                //if (schemeTag == 1)
                //{
                //    res = string.Format("{0:MM/dd/yyyy hh:mm:ss tt}", log.logDT);
                //    return res;
                //}

                //if (schemeTag == 2)
                //{
                //    if (scheme.schemeTypeCode == 0) //MEANS 8-5
                //    {
                //        res = string.Format("{0:MM/dd/yyyy hh:mm:ss tt}", log.logDT);

                //        if (tempLogA == "")
                //        {
                //            tempLogA = res;
                //        }
                //        else
                //        {
                //            tempLogB = res;
                //        }
                //    }
                //    else
                //    {
                //        res = string.Format("{0:MM/dd/yyyy hh:mm:ss tt}", log.logDT);
                //        return res;
                //    }
                //}

                //if (tempLogA.Length > 0 && tempLogB.Length > 0)
                //{
                //    res = tempLogA + ";" + tempLogB;
                //    return res;
                //}



            }
            return res;
        }



        public ActionResult Schedule()
        {
            return View();
        }


    }
}