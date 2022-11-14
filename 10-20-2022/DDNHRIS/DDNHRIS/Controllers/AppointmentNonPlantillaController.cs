using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DDNHRIS.Models;

namespace DDNHRIS.Controllers
{

    [SessionTimeout]
    [Authorize(Roles = "APRD")]

    public class AppointmentNonPlantillaController : Controller
    {

        HRISDBEntities db = new HRISDBEntities();

        // GET: AppointmentNonPlantilla
        public ActionResult List()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ApptListData()
        {
            string uEIC = Session["_EIC"].ToString();
            IEnumerable<vRSPAppointmentNonPlantilla> list = db.vRSPAppointmentNonPlantillas.Where(e => e.tag >= 0 && e.tag <= 3 && e.userEIC == uEIC).OrderByDescending(o => o.recNo).ToList();
            IEnumerable<tRSPAppointmentCasualEmp> empList = db.tRSPAppointmentCasualEmps.Where(e => e.recNo > 0).ToList();
            List<TempAppointmentList> myList = new List<TempAppointmentList>();
            foreach (vRSPAppointmentNonPlantilla item in list)
            {
                IEnumerable<tRSPAppointmentCasualEmp> groupList = empList.Where(e => e.appointmentCode == item.appointmentCode).ToList();
                string stat = "";
                if (item.tag == 1)
                {
                    stat = "Forwarded";
                }
                else if (item.tag == 2)
                {
                    stat = "for Posting";
                }
                else if (item.tag == 3)
                {
                    stat = "POSTED";
                }

                myList.Add(new TempAppointmentList()
                {
                    appointmentCode = item.appointmentCode,
                    appointmentName = item.appointmentName,
                    employmentStatus = item.employmentStatusNameShort,
                    itemCount = groupList.Count(),
                    //itemCount = 0,
                    projectName = item.projectName,
                    departmentName = item.departmentName,
                    status = stat,
                    period = AppointmentPeriod(Convert.ToDateTime(item.periodFrom), Convert.ToDateTime(item.periodTo)),
                    tag = Convert.ToInt16(item.tag)
                });

            }

            int cy = 2022;
            var fundSource = from f in db.vRSPRefFundSources
                             where f.CY == cy
                             select new { f.fundSourceCode, f.projectName };

            IEnumerable<tRSPAppointmentNature> appNatureList = db.tRSPAppointmentNatures.OrderBy(o => o.orderNo).ToList();

            var workGroupList = db.tRSPWorkGroups.Select(e => new
            {
                e.workGroupCode,
                e.workGroupName,
                e.orderNo
            }).OrderBy(e => e.orderNo).ToList();

            return Json(new { status = "success", list = myList, appNatureList = appNatureList, fundSource = fundSource, workGroupList = workGroupList }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult ChangeFundSource(int cy)
        {
            try
            {
                var fundSource = from f in db.vRSPRefFundSources
                                 where f.CY == cy
                                 select new { f.fundSourceCode, f.projectName };

                return Json(new { status = "success", fundSource = fundSource.OrderBy(o => o.projectName).ToList() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult GetAppointmentList(int id)
        {
            try
            {
                string uEIC = Session["_EIC"].ToString();
                int cy = 2021;

                IEnumerable<vRSPAppointmentNonPlantilla> list;

                if (id == 0)
                {
                    list = db.vRSPAppointmentNonPlantillas.Where(e => e.tag >= 0 && e.tag <= 3 && e.userEIC == uEIC).OrderByDescending(o => o.recNo).ToList();
                }
                else
                {
                    list = db.vRSPAppointmentNonPlantillas.Where(e => e.tag == 4 && e.userEIC == uEIC).OrderByDescending(o => o.recNo).ToList();
                }

                IEnumerable<tRSPAppointmentCasualEmp> empList = db.tRSPAppointmentCasualEmps.Where(e => e.recNo > 0).ToList();
                List<TempAppointmentList> myList = new List<TempAppointmentList>();
                foreach (vRSPAppointmentNonPlantilla item in list)
                {
                    IEnumerable<tRSPAppointmentCasualEmp> groupList = empList.Where(e => e.appointmentCode == item.appointmentCode).ToList();
                    string stat = "";
                    if (item.tag == 1)
                    {
                        stat = "Forwarded";
                    }
                    else if (item.tag == 2)
                    {
                        stat = "for Posting";
                    }
                    else if (item.tag == 3)
                    {
                        stat = "POSTED";
                    }

                    myList.Add(new TempAppointmentList()
                    {
                        appointmentCode = item.appointmentCode,
                        appointmentName = item.appointmentName,
                        employmentStatus = item.employmentStatusNameShort,
                        itemCount = groupList.Count(),
                        projectName = item.projectName,
                        departmentName = item.departmentName,
                        status = stat,
                        period = AppointmentPeriod(Convert.ToDateTime(item.periodFrom), Convert.ToDateTime(item.periodTo)),
                        tag = Convert.ToInt16(item.tag)
                    });
                }
                return Json(new { status = "success", appointment = myList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        //SEND TO ARCHIVE
        [HttpPost]
        public JsonResult SendToArchive(tRSPAppointmentCasual data)
        {
            try
            {
                string uEIC = Session["_EIC"].ToString();
                tRSPAppointmentCasual app = db.tRSPAppointmentCasuals.SingleOrDefault(e => e.appointmentCode == data.appointmentCode && e.tag == 3 && e.userEIC == uEIC);
                if (app == null)
                {
                    return Json(new { status = "Failed!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    app.tag = 4;
                    db.SaveChanges();
                    return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        private string AppointmentPeriod(DateTime f, DateTime t)
        {
            if (f == null)
            {
                return "TBD";
            }
            try
            {
                //JAN-MAR, 2021
                string res;
                res = f.ToString("MMM") + "-" + t.ToString("MMM") + ", " + t.ToString("yyyy");
                return res.ToUpper();
            }
            catch (Exception)
            {
                return "";
            }
        }

        //CASUAL APPOINTMENT
        //public ActionResult Casual()
        //{
        //    Session["AppointeeList"] = null;
        //    return View();
        //}

        //SAVE APPOINTMENT MAIN DATA
        [HttpPost]
        public JsonResult SaveAppointmentMasterData(tRSPAppointmentCasual data)
        {
            try
            {
                string uEIC = Session["_EIC"].ToString();
                tRSPAppointmentCasual app = new tRSPAppointmentCasual();
                DateTime dt = DateTime.Now;
                string tmpCode = "T" + dt.ToString("yyMMddHHmm") + "APPT" + data.employmentStatusCode + uEIC.Substring(0, 3) + dt.ToString("ssfff");
                app.appointmentCode = tmpCode;
                app.appointmentName = data.appointmentName;
                app.fundSourceCode = data.fundSourceCode;
                app.employmentStatusCode = data.employmentStatusCode;
                app.appNatureCode = data.appNatureCode;
                app.periodFrom = data.periodFrom;
                app.periodTo = data.periodTo;
                app.userEIC = uEIC;
                app.transDT = dt;
                app.tag = 0;
                db.tRSPAppointmentCasuals.Add(app);
                db.SaveChanges();
                return Json(new { status = "success", appointment = app }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        //NG
        [HttpPost]
        public JsonResult CasualApptData()
        {
            IEnumerable<vRSPRefFundSource> fundSource = db.vRSPRefFundSources.OrderBy(o => o.projectName).ToList();
            IEnumerable<tRSPAppointmentNature> appNatureList = db.tRSPAppointmentNatures.OrderBy(o => o.orderNo).ToList();
            return Json(new { status = "success", fundSourceList = fundSource, appNatureList = appNatureList }, JsonRequestBehavior.AllowGet);
        }

        //NG
        [HttpPost]
        public JsonResult GetAppointeeAndPosition()
        {
            //IEnumerable<tRSPEmployee> empList = db.tRSPEmployees.Where(e => e.tag != 0).OrderBy(o => o.fullNameLast).ToList();
            List<tRSPEmployee> newList = new List<tRSPEmployee>();

            IEnumerable<vRSPPlantilla> plantillaEmp = db.vRSPPlantillas.Where(e => e.isFunded == true && e.EIC != null).ToList();

            IEnumerable<vRSPEmployeeList> empList = db.vRSPEmployeeLists.Where(e => e.positionCode == null).ToList();

            foreach (vRSPEmployeeList item in empList)
            {
                int iCount = plantillaEmp.Where(e => e.EIC == item.EIC).Count();
                if (iCount == 0)
                {
                    newList.Add(new tRSPEmployee()
                    {
                        EIC = item.EIC,
                        fullNameLast = item.fullNameLast
                    });
                }
            }
            newList = newList.ToList();

            var myPositionList = db.tRSPPositions.Select(e => new
            {
                e.positionTitle,
                e.positionCode,
            }).OrderBy(e => e.positionTitle).ToList();

            var mySubList = db.tRSPPositionSubs.Select(e => new
            {
                e.subPositionTitle,
                e.subPositionCode,
            }).OrderBy(e => e.subPositionTitle).ToList();

            var wbList = db.tRSPWarmBodies.Select(e => new
            {
                e.warmBodyGroupCode,
                e.warmBodyGroupName,
                e.orderNo
            }).OrderBy(e => e.orderNo).ToList();


            return Json(new { status = "success", empList = newList, positionList = myPositionList, subPositionList = mySubList, warmBodyList = wbList }, JsonRequestBehavior.AllowGet);


        }



        [HttpPost]
        public JsonResult ReloadEmployeeList()
        {
            try
            {
                IEnumerable<tRSPEmployee> empList = db.tRSPEmployees.Where(e => e.tag == 1).OrderBy(o => o.fullNameLast).ToList();
                List<tRSPEmployee> newList = new List<tRSPEmployee>();

                IEnumerable<vRSPPlantilla> plantillaEmp = db.vRSPPlantillas.Where(e => e.isFunded == true && e.EIC != null).ToList();

                foreach (tRSPEmployee item in empList)
                {
                    int iCount = plantillaEmp.Where(e => e.EIC == item.EIC).Count();
                    if (iCount == 0)
                    {
                        newList.Add(new tRSPEmployee()
                        {
                            EIC = item.EIC,
                            fullNameLast = item.fullNameLast
                        });
                    }
                }
                newList = newList.ToList();
                //var temp = newList.Select(e => new { 
                //    e.EIC,
                //    e.fullNameLast
                //}).OrderBy(o => o.fullNameLast);
                return Json(new { status = "success", MaxJsonLength = int.MaxValue, list = newList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "Error!" }, JsonRequestBehavior.AllowGet);
            }
        }

        public class TempApptData
        {
            public string fundSourceCode { get; set; }
            public string periodFrom { get; set; }
            public string periodTo { get; set; }
            public string apptType { get; set; }
            public int hazardCode { get; set; }
        }

        //NG
        [HttpPost]
        public JsonResult AddAppointee(tRSPAppointmentCasualEmp data, TempApptData app)
        {

            if (data.EIC == null || data.positionCode == null || data.warmBodyGroupCode == null)
            {
                return Json(new { status = "Please fill-up the required data!" }, JsonRequestBehavior.AllowGet);
            }

            if (Convert.ToDateTime(app.periodFrom) >= Convert.ToDateTime(app.periodTo))
            {
                return Json(new { status = "Invalid period range!" }, JsonRequestBehavior.AllowGet);
            }


            tRSPEmployee employee = db.tRSPEmployees.SingleOrDefault(e => e.EIC == data.EIC);
            tRSPPosition position = new tRSPPosition();
            tRSPPositionSub subPosition = new tRSPPositionSub();

            vRSPSalaryDetailCurrent salary = new vRSPSalaryDetailCurrent();

            decimal rateDaily = 0;
            decimal rateMonthly = 0;
            decimal appointmentPS = 0;
            string salaryGradeText = "";
            string warmBodyGroupName = "";
            string departmentCode = "";

            tRSPWarmBody warmBody = db.tRSPWarmBodies.SingleOrDefault(e => e.warmBodyGroupCode == data.warmBodyGroupCode);
            departmentCode = warmBody.departmentCode;

            // 05-Casual 06-JobOrder 07-ContractOfService 08-Honorarium
            if (app.apptType == "05")
            {
                position = db.tRSPPositions.SingleOrDefault(e => e.positionCode == data.positionCode);
                subPosition = db.tRSPPositionSubs.SingleOrDefault(e => e.subPositionCode == data.subPositionCode);
                vRSPSalarySchedCasual sched = db.vRSPSalarySchedCasuals.Single(e => e.salaryGrade == position.salaryGrade);

                rateDaily = Convert.ToDecimal(sched.rateDay);
                rateMonthly = Convert.ToDecimal(sched.rateMonth);
                salary.salaryGrade = Convert.ToInt16(position.salaryGrade);
                salaryGradeText = "SG " + position.salaryGrade + "/1";
            }
            else if (app.apptType == "06")
            {
                position = db.tRSPPositions.SingleOrDefault(e => e.positionCode == data.positionCode);
                subPosition = db.tRSPPositionSubs.SingleOrDefault(e => e.subPositionCode == data.subPositionCode);
                vRSPSalarySchedJO sched = db.vRSPSalarySchedJOes.Single(e => e.salaryGrade == position.salaryGrade);
                rateDaily = Convert.ToDecimal(sched.rateDay);
                rateMonthly = Convert.ToDecimal(sched.rateMonth);
                salary.salaryGrade = Convert.ToInt16(position.salaryGrade);
                salaryGradeText = "SG " + position.salaryGrade + "/1";
            }
            else if (app.apptType == "07" || app.apptType == "08")
            {

                tRSPPositionContract posContract = db.tRSPPositionContracts.SingleOrDefault(e => e.positionCode == data.positionCode);
                position.positionCode = posContract.positionCode;
                position.positionTitle = posContract.positionTitle;
                position.salaryGrade = 0;
                rateDaily = Convert.ToDecimal(posContract.salary / 22);
                rateMonthly = Convert.ToDecimal(posContract.salary);
                warmBodyGroupName = warmBody.warmBodyGroupName;
                salary.salaryDetailCode = "";
                salary.salaryGrade = 0;
                salary.rateMonth = posContract.salary;
            }

            if (employee == null || position.positionCode == null)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }

            //MAKE VALIDATION HERE ----------------
            List<AppointmentNonPlantillaData> finalList = new List<AppointmentNonPlantillaData>();
            if (data.EIC != null && data.positionCode != null)
            {

                //vRSPSalaryDetailCurrent salary = db.vRSPSalaryDetailCurrents.Single(e => e.salaryGrade == position.salaryGrade && e.step == 1);

                var tempList = Session["AppointeeList"];
                List<AppointmentNonPlantillaData> list = tempList as List<AppointmentNonPlantillaData>;

                string tmpPositionTitle = position.positionTitle;
                string tmpSubPositionCode = "";
                string tmpSubPositionTitle = "";

                if (subPosition != null)
                {
                    if (subPosition.subPositionCode != null)
                    {
                        tmpPositionTitle = tmpPositionTitle + " (" + subPosition.subPositionTitle + ")";
                        tmpSubPositionCode = subPosition.subPositionCode;
                        tmpSubPositionTitle = subPosition.subPositionTitle;
                    }
                }

                if (list != null)
                {
                    int countChecker = list.Where(e => e.EIC == employee.EIC).Count();
                    if (countChecker > 0)
                    {
                        return Json(new { status = "Employee already exists." }, JsonRequestBehavior.AllowGet);
                    }
                }

                if (list == null)
                {
                    List<AppointmentNonPlantillaData> temp = new List<AppointmentNonPlantillaData>();
                    temp.Add(new AppointmentNonPlantillaData()
                    {
                        EIC = employee.EIC,
                        fullNameLast = employee.fullNameLast,
                        lastName = employee.lastName,
                        firstName = employee.firstName,
                        extName = employee.extName,
                        middleName = employee.middleName,
                        positionCode = position.positionCode,
                        positionTitle = tmpPositionTitle,
                        subPositionCode = tmpSubPositionCode,
                        subPositionTitle = tmpSubPositionTitle,
                        salaryDetailCode = salary.salaryDetailCode,
                        salaryGrade = salary.salaryGrade,
                        salaryGradeText = "SG " + position.salaryGrade + "/1",
                        rateDaily = rateDaily,
                        rateMonthly = rateMonthly,
                        warmBodyGroupCode = data.warmBodyGroupCode,
                        warmBodyGroupName = warmBodyGroupName,
                        hazardCode = app.hazardCode,
                        PS = appointmentPS,
                    });
                    finalList = temp;
                    Session["AppointeeList"] = temp;
                }
                else
                {
                    list.Add(new AppointmentNonPlantillaData()
                    {
                        EIC = employee.EIC,
                        fullNameLast = employee.fullNameLast,
                        lastName = employee.lastName,
                        firstName = employee.firstName,
                        extName = employee.extName,
                        middleName = employee.middleName,
                        positionCode = position.positionCode,
                        positionTitle = tmpPositionTitle,
                        subPositionCode = tmpSubPositionCode,
                        subPositionTitle = tmpSubPositionTitle,
                        salaryDetailCode = salary.salaryDetailCode,
                        salaryGrade = salary.salaryGrade,
                        salaryGradeText = salaryGradeText,
                        rateDaily = rateDaily,
                        rateMonthly = rateMonthly,
                        warmBodyGroupCode = data.warmBodyGroupCode,
                        warmBodyGroupName = warmBodyGroupName,
                        hazardCode = app.hazardCode,
                        PS = appointmentPS,
                    });
                    finalList = list;
                    finalList = finalList.OrderBy(o => o.fullNameLast).ToList();
                    Session["AppointeeList"] = finalList;
                }
            }
            return Json(new { status = "success", list = finalList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RemoveEmployeeFromList(string id)
        {
            try
            {
                var tempList = Session["AppointeeList"];
                List<AppointmentNonPlantillaData> list = tempList as List<AppointmentNonPlantillaData>;
                list = list.Where(e => e.EIC != id).ToList();
                Session["AppointeeList"] = list;
                return Json(new { status = "success", list = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        // CALCULATOR OF PERSONNEL SERVICES
        private decimal CALQCasualPS(tRSPPosition position, vRSPSalarySchedCasual salarySched, int hazardCode, string periodFrom, string periodTo)
        {
            try
            {

                DateTime pFrom;
                pFrom = Convert.ToDateTime(periodFrom);
                DateTime pTo = Convert.ToDateTime(periodTo);

                // = db.tRSPPositions.Single(e => e.positionCode == positionCode);
                // = db.vRSPSalaryDetailCurrents.Single(e => e.salaryGrade == position.salaryGrade);

                int monthCount = Monthcounter(pFrom, pTo);

                decimal dailyRate = Convert.ToDecimal(salarySched.rateMonth) / 22;
                decimal monthlyRate = Convert.ToDecimal(salarySched.rateMonth);
                decimal annualSalary = monthlyRate * monthCount;
                decimal PERA = 2000 * monthCount;

                decimal earnedLeave = (monthlyRate / 8) * monthCount;

                decimal hazardPay = 0;
                decimal subsistence = 0;
                decimal laundry = 0;
                if (hazardCode == 1)
                {
                    //tRSPPosition position = db.tRSPPositions.Single(e => e.positionCode == data.positionCode);
                    //HEALTH SERVICES
                    hazardPay = CalQHazardForHealthMonthly(Convert.ToInt16(position.salaryGrade), Convert.ToDecimal(dailyRate)) * monthCount;
                    //SUBS
                    subsistence = 1500 * monthCount;
                    laundry = 150 * monthCount;
                }
                else if (hazardCode == 2)
                {
                    //SOCIAL WORKER
                    hazardPay = CalQHazardForSocialWorkerMonthly(dailyRate) * monthCount;
                    subsistence = 1500 * monthCount;
                }


                DateTime clothingCutOff = Convert.ToDateTime("March 30, " + pFrom.Year);
                decimal clothing = 0;
                if (pFrom <= clothingCutOff)  //MARCH 30 but if HEALTH PERSONNEL NO CUT-OFF
                {
                    clothing = 6000;
                }

                DateTime midYearCutOff = Convert.ToDateTime("May 15, " + pFrom.Year);
                decimal midYearBonus = 0;
                if (pFrom <= midYearCutOff)
                {
                    midYearBonus = monthlyRate;
                }

                DateTime yearEndCutOff = Convert.ToDateTime("October 31, " + pFrom.Year);

                decimal yearEndBonus = 0;
                decimal cashGift = 0;

                if (pFrom <= yearEndCutOff)
                {
                    yearEndBonus = monthlyRate;
                    cashGift = 5000;
                }

                decimal lifeAndRetire = (Convert.ToDecimal(monthlyRate) * Convert.ToDecimal(.12)) * monthCount;
                decimal PHIC = CalQPHICGovtShare(Convert.ToDouble(monthlyRate), monthCount);

                decimal HDMF = 100 * monthCount;
                decimal ECC = 100 * monthCount;

                decimal totalPS = annualSalary + PERA + earnedLeave + hazardPay + subsistence + laundry + midYearBonus + yearEndBonus + cashGift + lifeAndRetire + ECC + PHIC + clothing;

                return totalPS;

            }
            catch (Exception)
            {
                return 0;
            }

        }


        // CALCULATOR OF PERSONNEL SERVICES (SAVING MODE)
        private decimal CALQCasualPSSaving(string positionCode, int hazardCode, string periodFrom, string periodTo)
        {
            try
            {

                DateTime pFrom;
                pFrom = Convert.ToDateTime(periodFrom);
                DateTime pTo = Convert.ToDateTime(periodTo);

                tRSPPosition position = db.tRSPPositions.Single(e => e.positionCode == positionCode);
                vRSPSalaryDetailCurrent salarySched = db.vRSPSalaryDetailCurrents.Single(e => e.salaryGrade == position.salaryGrade);

                int monthCount = Monthcounter(pFrom, pTo);

                decimal dailyRate = Convert.ToDecimal(salarySched.rateMonth) / 22;
                decimal monthlyRate = Convert.ToDecimal(salarySched.rateMonth);
                decimal annualSalary = monthlyRate * monthCount;
                decimal PERA = 2000 * monthCount;

                decimal earnedLeave = (monthlyRate / 8) * monthCount;

                decimal hazardPay = 0;
                decimal subsistence = 0;
                decimal laundry = 0;
                if (hazardCode == 1)
                {
                    hazardPay = CalQHazardForHealthMonthly(Convert.ToInt16(position.salaryGrade), Convert.ToDecimal(dailyRate)) * monthCount;
                    //SUBS
                    subsistence = 1500 * monthCount;
                    laundry = 150 * monthCount;
                }
                else if (hazardCode == 2)
                {
                    //SOCIAL WORKER
                    hazardPay = CalQHazardForSocialWorkerMonthly(dailyRate) * monthCount;
                    subsistence = 1500 * monthCount;
                }


                DateTime clothingCutOff = Convert.ToDateTime("March 30, " + pFrom.Year);
                decimal clothing = 0;
                if (pFrom <= clothingCutOff)  //MARCH 30 but if HEALTH PERSONNEL NO CUT-OFF
                {
                    clothing = 6000;
                }

                DateTime midYearCutOff = Convert.ToDateTime("May 15, " + pFrom.Year);
                decimal midYearBonus = 0;
                if (pFrom <= midYearCutOff)
                {
                    midYearBonus = dailyRate * 12;
                }


                DateTime yearEndCutOff = Convert.ToDateTime("October 31, " + pFrom.Year);

                decimal yearEndBonus = 0;
                decimal cashGift = 0;

                if (pFrom <= yearEndCutOff)
                {
                    yearEndBonus = dailyRate * 12;
                    cashGift = 5000;
                }

                decimal lifeAndRetire = (Convert.ToDecimal(monthlyRate) * Convert.ToDecimal(.12)) * monthCount;
                decimal PHIC = CalQPHICGovtShare(Convert.ToDouble(monthlyRate), monthCount);

                decimal HDMF = 100 * monthCount;
                decimal ECC = 100 * monthCount;

                decimal totalPS = annualSalary + PERA + earnedLeave + hazardPay + subsistence + laundry + midYearBonus + yearEndBonus + cashGift + lifeAndRetire + ECC + PHIC + clothing;

                return totalPS;

            }
            catch (Exception)
            {
                return 0;
            }

        }




        //NG
        [HttpPost]
        public JsonResult SaveCasualAppointment(tRSPAppointmentCasual data, string periodF, string periodT)
        {
            if (data.fundSourceCode == null || data.appointmentName == null)
            {
                return Json(new { status = "Incomplete data!" }, JsonRequestBehavior.AllowGet);
            }

            if (data.appType == "05" && data.appNatureCode == null)
            {
                return Json(new { status = "Incomplete data!" }, JsonRequestBehavior.AllowGet);
            }

            var tempList = Session["AppointeeList"];
            List<AppointmentNonPlantillaData> list = tempList as List<AppointmentNonPlantillaData>;

            string apptCode = "APPT" + DateTime.Now.ToString("yyMMddHHmmssfff");
            string itemCode = "APPI";

            DateTime periodFrom = Convert.ToDateTime(periodF);
            DateTime periodTo = Convert.ToDateTime(periodT);


            string apptType = "";

            List<vRSPSalarySchedCasual> salaryTableCasual = new List<vRSPSalarySchedCasual>();
            List<vRSPSalarySchedJO> salaryTableJobOrder = new List<vRSPSalarySchedJO>();

            if (data.appType == "05")
            {
                salaryTableCasual = db.vRSPSalarySchedCasuals.OrderBy(o => o.salaryGrade).ThenBy(s => s.step).ToList();
            }
            else if (data.appType == "06")
            {
                salaryTableJobOrder = db.vRSPSalarySchedJOes.OrderBy(o => o.salaryGrade).ThenBy(s => s.step).ToList();
            }

            try
            {
                int counter = 0;
                foreach (AppointmentNonPlantillaData item in list)
                {
                    counter = counter + 1;
                    decimal salary = 0;
                    string salaryType = "";
                    decimal itemPS = 0;

                    tRSPZPersonnelService personnelService = new tRSPZPersonnelService();


                    tRSPPosition position = new tRSPPosition();
                    string salaryDetailCode = "";

                    int hazardCode = 0;


                    if (data.appType == "05")
                    {
                        hazardCode = item.hazardCode;
                        vRSPSalarySchedCasual sched = salaryTableCasual.Single(e => e.salaryGrade == item.salaryGrade);
                        TempAppointmentNPPosting postData = new TempAppointmentNPPosting();
                        personnelService = GetPositionCasualPS(sched, position, hazardCode, periodFrom, periodTo, postData);

                        tRSPZPersonnelService p = personnelService;

                        itemPS = Convert.ToDecimal(p.annualRate) + Convert.ToDecimal(p.PERA) + Convert.ToDecimal(p.leaveEarned) + Convert.ToDecimal(p.hazardPay) + Convert.ToDecimal(p.subsistence) + Convert.ToDecimal(p.laundry);
                        itemPS = itemPS + Convert.ToDecimal(p.midYearBonus) + Convert.ToDecimal(p.yearEndBonus) + Convert.ToDecimal(p.cashGift) + Convert.ToDecimal(p.lifeRetmnt) + Convert.ToDecimal(p.ECC) + Convert.ToDecimal(p.hdmf) + Convert.ToDecimal(p.phic) + Convert.ToDecimal(p.clothing);

                        apptType = "CAS";
                        position.positionCode = item.positionCode;
                        position.salaryGrade = item.salaryGrade;
                        salaryDetailCode = sched.salaryDetailCode;

                        salaryType = "D";
                        salary = Convert.ToDecimal(personnelService.dailyRate);
                    }
                    else if (data.appType == "06")
                    {
                        apptType = "JOB";
                        vRSPSalarySchedJO sched = salaryTableJobOrder.Single(e => e.salaryGrade == item.salaryGrade);
                        personnelService = GetPositionJobOrderPS(sched, periodFrom, periodTo);
                        itemPS = Convert.ToDecimal(personnelService.annualRate);
                        salaryDetailCode = sched.salaryDetailCode;
                        position.salaryGrade = item.salaryGrade;
                        salaryType = "D";
                        salary = Convert.ToDecimal(personnelService.dailyRate);
                    }
                    else if (data.appType == "07")
                    {
                        apptType = "COS";
                        tRSPPositionContract posContract = db.tRSPPositionContracts.SingleOrDefault(e => e.positionCode == item.positionCode);
                        position.positionCode = item.positionCode;
                        personnelService = GetPositionContractHon(posContract, periodFrom, periodTo);
                        itemPS = Convert.ToDecimal(personnelService.annualRate);
                        salaryType = "M";
                        salary = item.rateMonthly;
                    }
                    else if (data.appType == "08")
                    {
                        apptType = "HON";
                        tRSPPositionContract posContract = db.tRSPPositionContracts.SingleOrDefault(e => e.positionCode == item.positionCode);
                        position.positionCode = item.positionCode;
                        personnelService = GetPositionContractHon(posContract, periodFrom, periodTo);
                        itemPS = Convert.ToDecimal(personnelService.annualRate);
                        salaryType = "M";
                        salary = item.rateMonthly;
                    }

                    string code = itemCode + apptCode.Substring(4, 15) + counter.ToString("000");
                    tRSPAppointmentCasualEmp a = new tRSPAppointmentCasualEmp();
                    a.appointmentItemCode = code;
                    a.appointmentCode = apptCode;
                    a.EIC = item.EIC;
                    a.positionCode = item.positionCode;
                    a.positionTitle = item.positionTitle;
                    a.subPositionCode = item.subPositionCode;
                    a.subPositionTitle = item.subPositionTitle;
                    a.salaryGrade = item.salaryGrade;
                    a.salaryDetailCode = salaryDetailCode;
                    a.salary = salary;
                    a.salaryType = salaryType;
                    a.tag = 0;

                    if (periodF != null || periodF != "" || periodFrom.Year == 1)
                    {
                        a.periodFrom = periodFrom;
                    }
                    a.warmBodyGroupCode = item.warmBodyGroupCode;
                    a.periodTo = Convert.ToDateTime(periodTo);
                    a.PS = itemPS;

                    db.tRSPAppointmentCasualEmps.Add(a);

                    tRSPZPersonnelService ps = new tRSPZPersonnelService();
                    ps = personnelService;
                    ps.appointmentCode = apptCode;
                    ps.appointmentItemCode = code;
                    ps.positionTitle = item.positionTitle;
                    ps.userID = Session["_EIC"].ToString();
                    ps.transDT = DateTime.Now;
                    ps.total = itemPS;
                    ps.tag = 0;
                    db.tRSPZPersonnelServices.Add(ps);
                }
                tRSPAppointmentCasual appt = new tRSPAppointmentCasual();
                appt.appointmentCode = apptCode;
                appt.appNatureCode = data.appNatureCode;
                appt.appointmentName = data.appointmentName;
                appt.fundSourceCode = data.fundSourceCode;
                appt.userEIC = Session["_EIC"].ToString();
                appt.transDT = DateTime.Now;
                appt.employmentStatusCode = data.appType;
                appt.tag = 0;
                db.tRSPAppointmentCasuals.Add(appt);
                db.SaveChanges();

                Session["AppointeeList"] = null;

                return Json(new { status = "success", apptCode = apptCode }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
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
            }

            if (checkDateFrom <= cuttOffFrom && monthCount >= 4)
            {
                midYearBonus = monthlyRate;
            }

            //!IMPORTANT => MIDYEAR BONUS
            if (postData.hasMidYear == 1)
            {
                midYearBonus = monthlyRate;
            }


            decimal yearEndBonus = 0;
            decimal cashGift = 0;
            DateTime cuttOffTo = Convert.ToDateTime("October 31, " + periodTo.Year.ToString());
            if (checkDateFrom <= cuttOffTo && periodTo >= cuttOffTo)
            {
                if (monthCount >= 4)
                {
                    yearEndBonus = monthlyRate;
                    cashGift = 5000;
                }
            }

            //!IMPORTANT => YEAR-END BONUS
            if (postData != null && postData.hasYearEnd == 1)
            {
                yearEndBonus = monthlyRate;
                cashGift = 5000;
            }

            decimal lifeAndRetire = (Convert.ToDecimal(monthlyRate) * Convert.ToDecimal(.12)) * monthCount;

            // PHIC
            //PersonnelServiceViewModel pscomp = new PersonnelServiceViewModel();
            //decimal PHIC = pscomp.ComputePHICGovtShare(Convert.ToDouble(monthlyRate), monthCount);                        
            decimal PHIC = CalQPHICGovtShare(Convert.ToDouble(monthlyRate), monthCount);

            decimal HDMF = 100 * monthCount;
            decimal ECC = 100 * monthCount;

            decimal clothing = 0;
            //MARCH 30 but if HEALTH PERSONNEL NO CUT-OFF
            clothing = 6000;

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
                ps.phic = 0;
                ps.clothing = 0;
                ps.total = totalPS;
                ps.periodFrom = Convert.ToDateTime(periodFrom);
                ps.periodTo = periodTo;
                //ps.PSTag = 0;
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
                ps.phic = 0;
                ps.clothing = 0;
                ps.total = totalPS;
                ps.periodFrom = Convert.ToDateTime(periodFrom);
                ps.periodTo = periodTo;
                //ps.PSTag = 0;
                return ps;
            }
            catch (Exception)
            {
                return ps;
            }
        }


        /// <summary>
        /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult PrintPreviewSetup(string id, string page)
        {
            try
            {
                tRSPAppointmentCasual app = db.tRSPAppointmentCasuals.SingleOrDefault(e => e.appointmentCode == id);

                if (app == null)
                {
                    return Json(new { status = "Invalid appointment!" }, JsonRequestBehavior.AllowGet);
                }

                string repType = "";

                if (app.employmentStatusCode == "05") //CASUAL
                {
                    repType = "APPTCasual_34F";
                    if (app.appNatureCode == "APN01" || app.appNatureCode == "APN05" || app.appNatureCode == "APN06" || app.appNatureCode == "APN07") //NEW APPOINTMENT
                    {
                        repType = "APPTCasual_34D";
                    }
                    if (page == "R")
                    {
                        repType = repType + "_R";
                    }
                }
                else if (app.employmentStatusCode == "06") //JO
                {
                    repType = "APPTJOBORDER";
                    if (page == "R")
                    {
                        repType = "APPTJOBORDER_R";
                    }
                }
                else if (app.employmentStatusCode == "07") //COS
                {
                    repType = "APPTHonorarium";
                    if (page == "R")
                    {
                        repType = "APPTHonorarium_R";
                    }
                }
                else if (app.employmentStatusCode == "08") //HON
                {
                    repType = "APPTHonorarium";
                    if (page == "R")
                    {
                        repType = "APPTHonorarium_R";
                    }
                }

                if (repType == "")
                {
                    return Json(new { status = "Invalid report!" }, JsonRequestBehavior.AllowGet);
                }

                Session["ReportType"] = repType;
                Session["PrintReport"] = id;

                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }

        }

        ////NG
        [HttpPost]
        public JsonResult ViewAppDetails(string code)
        {
            try
            {
                IEnumerable<vRSPAppointmentNPEmployee> empList = db.vRSPAppointmentNPEmployees.Where(e => e.appointmentCode == code).OrderBy(o => o.fullNameLast).ToList();
                IEnumerable<tRSPZPersonnelService> posted = db.tRSPZPersonnelServices.ToList();
                List<vRSPAppointmentNPEmployee> list = new List<vRSPAppointmentNPEmployee>();
                foreach (vRSPAppointmentNPEmployee item in empList)
                {
                    //int iCount = posted.Where(e => e.appointmentItemCode == item.appointmentItemCode).Count();
                    list.Add(new vRSPAppointmentNPEmployee()
                    {
                        appointmentItemCode = item.appointmentItemCode,
                        EIC = item.EIC,
                        fullNameLast = item.fullNameLast,
                        fullNameFirst = item.fullNameFirst,
                        positionTitle = item.positionTitle,
                        salaryGrade = item.salaryGrade,
                        salary = item.salary,
                        salaryType = item.salaryType,
                        periodFrom = item.periodFrom,
                        periodTo = item.periodTo,
                        departmentCode = item.departmentCode,
                        empTag = item.empTag,
                        tag = item.tag
                    });
                }
                return Json(new { status = "success", appointeeList = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }


        //NG
        [HttpPost]
        public JsonResult ViewCasualAppointmentItem(string code)
        {
            try
            {
                vRSPAppointmentNPEmployee appItem = db.vRSPAppointmentNPEmployees.Single(e => e.appointmentItemCode == code);
                int hasHazard = 0;
                int casualTag = 0;
                if (appItem.employmentStatusCode == "05")
                {
                    casualTag = 1;
                    if (appItem.departmentCode == "OC191017163920527001" || appItem.departmentCode == "OC191017164319324001" || appItem.departmentCode == "OC191017163949719001")
                    {
                        hasHazard = 1;
                    }
                }
                else
                {
                    hasHazard = 0;
                }

                //var workGroupList = db.tRSPWorkGroups.Select(e => new
                //{
                //    e.workGroupCode,
                //    e.workGroupName,
                //    e.orderNo
                //}).OrderBy(e => e.orderNo).ToList();
                tRSPWorkGroupEmp wrkGrpCode = db.tRSPWorkGroupEmps.SingleOrDefault(e => e.EIC == appItem.EIC);
                string workGroup = wrkGrpCode == null ? "" : wrkGrpCode.workGroupCode;
                // workGroupList = workGroupList,

                if (appItem.periodFrom != null)
                {
                    if (appItem.periodFrom.Value.Year == 1)
                    {
                        appItem.periodFrom = null;
                    }
                }



                return Json(new { status = "success", appData = appItem, hazardTag = hasHazard, casualTag = casualTag, workGroupCode = workGroup }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
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
        }


        //NG 
        [HttpPost]
        public JsonResult POSTAppointmentItem(TempAppointmentNPPosting data, string workGroupCode)
        {
            decimal stat = 0;
            string uEIC = Session["_EIC"].ToString();
            if (data.appointmentItemCode == null || data.periodFrom == null)
            {
                return Json(new { status = "Please fill-up the required data!" }, JsonRequestBehavior.AllowGet);
            }

            if (workGroupCode == null || workGroupCode == "")
            {
                return Json(new { status = "Please fill-up the required data!" }, JsonRequestBehavior.AllowGet);
            }

            vRSPAppointmentNPEmployee item = db.vRSPAppointmentNPEmployees.SingleOrDefault(e => e.appointmentItemCode == data.appointmentItemCode);


            if (Convert.ToDateTime(data.periodFrom).Year != item.periodTo.Value.Year)
            {
                return Json(new { status = "Invalid date!" }, JsonRequestBehavior.AllowGet);
            }

            if (item == null)
            {
                //HACKER ALERT
                return Json(new { status = "Please fill-up the required data!" }, JsonRequestBehavior.AllowGet);
            }

            tRSPWorkGroupEmp workGroupData = db.tRSPWorkGroupEmps.SingleOrDefault(e => e.EIC == item.EIC);
            if (workGroupData == null)
            {
                tRSPWorkGroupEmp w = new tRSPWorkGroupEmp();
                w.EIC = item.EIC;
                w.workGroupCode = workGroupCode;
                w.userEIC = uEIC;
                w.transDT = DateTime.Now;
                db.tRSPWorkGroupEmps.Add(w);
                db.SaveChanges();
            }
            else
            {
                workGroupData.workGroupCode = workGroupCode;
                db.SaveChanges();
            }

            if (item.employmentStatusCode == "05")
            {
                tRSPPosition position = db.tRSPPositions.Single(e => e.positionCode == item.positionCode);
                vRSPSalarySchedCasual sched = db.vRSPSalarySchedCasuals.Single(e => e.salaryGrade == item.salaryGrade);

                DateTime assmptDate = Convert.ToDateTime(data.periodFrom);
                string code = item.appointmentItemCode;
                //var res = db.spAppointmentPostingNP(code, assmptDate, workGroupCode, uEIC);
                var res = db.spAppointmentPostingNonPlan(code, assmptDate, workGroupCode, uEIC);
                stat = 1;
            }
            else if (item.employmentStatusCode == "06")
            {
                // JOB ORDER
                vRSPSalarySchedJO sched = db.vRSPSalarySchedJOes.Single(e => e.salaryGrade == item.salaryGrade);
                tRSPZPersonnelService p = GetPositionJobOrderPS(sched, Convert.ToDateTime(data.periodFrom), Convert.ToDateTime(item.periodTo));

                DateTime assmptDate = Convert.ToDateTime(data.periodFrom);
                var res = db.spAppointmentPostingNonPlan(item.appointmentItemCode, assmptDate, workGroupCode, uEIC);
                stat = 1;
            }
            else if (item.employmentStatusCode == "07" || item.employmentStatusCode == "08")
            {
                // C.O.S. & Honorarium
                tRSPPositionContract posContract = db.tRSPPositionContracts.SingleOrDefault(e => e.positionCode == item.positionCode);
                tRSPZPersonnelService p = GetPositionContractHon(posContract, Convert.ToDateTime(data.periodFrom), Convert.ToDateTime(item.periodTo));
                DateTime assmptDate = Convert.ToDateTime(data.periodFrom);
                var res = db.spAppointmentPostingNonPlan(item.appointmentItemCode, assmptDate, workGroupCode, uEIC);
                stat = 1;
            }

            if (stat != 1)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                IEnumerable<vRSPAppointmentNPEmployee> empList = db.vRSPAppointmentNPEmployees.Where(e => e.appointmentCode == item.appointmentCode).OrderBy(o => o.fullNameLast).ToList();
                //IEnumerable<tRSPZPersonnelService> posted = db.tRSPZPersonnelServices.ToList();
                List<vRSPAppointmentNPEmployee> list = new List<vRSPAppointmentNPEmployee>();
                //int appPostCounter = 0;
                foreach (vRSPAppointmentNPEmployee itm in empList)
                {
                    int tmpTag = Convert.ToInt16(itm.tag);
                    int tmpPostNo = Convert.ToInt32(itm.postNo);
                    if (itm.appointmentItemCode == item.appointmentItemCode)
                    {
                        tmpTag = 2;
                        tmpPostNo = 1;
                    }
                    list.Add(new vRSPAppointmentNPEmployee()
                    {
                        appointmentItemCode = itm.appointmentItemCode,
                        EIC = itm.EIC,
                        fullNameLast = itm.fullNameLast,
                        fullNameFirst = itm.fullNameFirst,
                        positionTitle = itm.positionTitle,
                        salaryGrade = itm.salaryGrade,
                        salary = itm.salary,
                        salaryType = itm.salaryType,
                        periodFrom = itm.periodFrom,
                        periodTo = itm.periodTo,
                        departmentCode = itm.departmentCode,
                        empTag = item.empTag,
                        tag = tmpTag,
                        postNo = tmpPostNo

                    });
                }
                return Json(new { status = "success", appointeeList = list }, JsonRequestBehavior.AllowGet);
            }
        }


        //private int POSTJobConHonAppointment(TempAppointmentNPPosting data, vRSPAppointmentNPEmployee item)
        //{
        //    try
        //    {

        //        string periodFrom = data.periodFrom;
        //        int hazardCode = data.hazardCode;
        //        int hasMidYear = data.hasMidYear;
        //        int hasYearEnd = data.hasYearEnd;
        //        int hasClothing = data.hasClothing;

        //        DateTime pFrom;
        //        pFrom = Convert.ToDateTime(periodFrom);
        //        DateTime pTo = Convert.ToDateTime(item.periodTo);

        //        int monthCount = Monthcounter(pFrom, pTo);

        //        decimal rate = Convert.ToDecimal(item.salary);
        //        decimal monthlyRate = 0;
        //        decimal dailyRate = 0;
        //        if (item.salaryType == "D")
        //        {
        //            monthlyRate = rate * 22;
        //            dailyRate = rate;
        //        }
        //        else if (item.salaryType == "M")
        //        {
        //            dailyRate = rate / 22;
        //            monthlyRate = rate;
        //        }


        //        decimal annualSalary = monthlyRate * monthCount;
        //        decimal totalPS = annualSalary;

        //        tRSPZPersonnelService ps = new tRSPZPersonnelService();
        //        ps.appointmentItemCode = item.appointmentItemCode;
        //        ps.positionTitle = item.positionTitle;
        //        ps.dailyRate = dailyRate;
        //        ps.monthlyRate = monthlyRate;
        //        ps.annualRate = annualSalary;
        //        ps.PERA = 0;
        //        ps.leaveEarned = 0;
        //        ps.midYearBonus = 0;
        //        ps.yearEndBonus = 0;
        //        ps.cashGift = 0;
        //        ps.lifeRetmnt = 0;
        //        ps.ECC = 0;
        //        ps.phic = 0;
        //        ps.clothing = 0;
        //        ps.total = totalPS;
        //        ps.periodFrom = Convert.ToDateTime(periodFrom);
        //        ps.periodTo = item.periodTo;
        //        ps.userID = Session["_EIC"].ToString();
        //        ps.transDT = DateTime.Now;
        //        ps.PSTag = 1;
        //        db.tRSPZPersonnelServices.Add(ps);
        //        db.SaveChanges();


        //        IEnumerable<vRSPAppointmentNPEmployee> empList = db.vRSPAppointmentNPEmployees.Where(e => e.appointmentCode == item.appointmentCode).OrderBy(o => o.fullNameLast).ToList();
        //        IEnumerable<tRSPZPersonnelService> posted = db.tRSPZPersonnelServices.ToList();
        //        List<vRSPAppointmentNPEmployee> list = new List<vRSPAppointmentNPEmployee>();

        //        int appPostCounter = 0;

        //        foreach (vRSPAppointmentNPEmployee itm in empList)
        //        {
        //            int iCount = posted.Where(e => e.appointmentItemCode == itm.appointmentItemCode).Count();
        //            //
        //            if (iCount > 0) { appPostCounter = appPostCounter + 1; }

        //            list.Add(new vRSPAppointmentNPEmployee()
        //            {
        //                appointmentItemCode = itm.appointmentItemCode,
        //                EIC = itm.EIC,
        //                fullNameLast = itm.fullNameLast,
        //                fullNameFirst = itm.fullNameFirst,
        //                positionTitle = itm.positionTitle,
        //                salaryGrade = itm.salaryGrade,
        //                salary = itm.salary,
        //                salaryType = itm.salaryType,
        //                periodFrom = itm.periodFrom,
        //                periodTo = itm.periodTo,
        //                departmentCode = itm.departmentCode,
        //                tag = iCount
        //            });
        //        }

        //        //if (appPostCounter >= empList.Count())
        //        //{
        //        //    tRSPAppointmentCasual app = db.tRSPAppointmentCasuals.Single(e => e.appointmentCode == item.appointmentCode);
        //        //    app.tag = 2;
        //        //    db.SaveChanges();
        //        //}
        //        return 1;
        //    }
        //    catch (Exception)
        //    {
        //        return 0;
        //    }

        //}



        //private int POSTCasualAppointment(TempAppointmentNPPosting data, vRSPAppointmentNPEmployee item)
        //{
        //    try
        //    {
        //        string periodFrom = data.periodFrom;
        //        int hazardCode = data.hazardCode;
        //        int hasMidYear = data.hasMidYear;
        //        int hasYearEnd = data.hasYearEnd;
        //        int hasClothing = data.hasClothing;

        //        DateTime pFrom;
        //        pFrom = Convert.ToDateTime(periodFrom);
        //        DateTime pTo = Convert.ToDateTime(item.periodTo);
        //        int monthCount = Monthcounter(pFrom, pTo);

        //        decimal dailyRate = Convert.ToDecimal(item.salary);
        //        decimal monthlyRate = dailyRate * 22;
        //        decimal annualSalary = monthlyRate * monthCount;
        //        decimal PERA = 2000 * monthCount;
        //        decimal earnedLeave = (monthlyRate / 8) * monthCount;

        //        decimal hazardPay = 0;
        //        decimal subsistence = 0;
        //        decimal laundry = 0;
        //        if (hazardCode == 1)
        //        {
        //            tRSPPosition position = db.tRSPPositions.Single(e => e.positionCode == item.positionCode);
        //            //HEALTH SERVICES
        //            hazardPay = CalQHazardForHealthMonthly(Convert.ToInt16(position.salaryGrade), Convert.ToDecimal(dailyRate)) * monthCount;
        //            //SUBS
        //            subsistence = 1500 * monthCount;
        //            laundry = 150 * monthCount;
        //        }
        //        else if (hazardCode == 2)
        //        {
        //            //SOCIAL WORKER
        //            hazardPay = CalQHazardForSocialWorkerMonthly(dailyRate) * monthCount;
        //            subsistence = 1500 * monthCount;
        //        }

        //        decimal midYearBonus = 0;
        //        if (hasMidYear == 1)
        //        {
        //            midYearBonus = monthlyRate;
        //        }

        //        decimal yearEndBonus = 0;
        //        decimal cashGift = 0;
        //        if (hasYearEnd == 1)
        //        {
        //            yearEndBonus = monthlyRate;
        //            cashGift = 5000;
        //        }

        //        decimal lifeAndRetire = (Convert.ToDecimal(monthlyRate) * Convert.ToDecimal(.12)) * monthCount;
        //        decimal PHIC = CalQPHICGovtShare(Convert.ToDouble(monthlyRate), monthCount);

        //        decimal HDMF = 100 * monthCount;
        //        decimal ECC = 100 * monthCount;

        //        decimal clothing = 0;
        //        if (hasClothing == 1)  //MARCH 30 but if HEALTH PERSONNEL NO CUT-OFF
        //        {
        //            clothing = 6000;
        //        }

        //        decimal totalPS = annualSalary + PERA + earnedLeave + hazardPay + subsistence + laundry + midYearBonus + yearEndBonus + cashGift + lifeAndRetire + ECC + PHIC + clothing;

        //        tRSPZPersonnelService ps = new tRSPZPersonnelService();
        //        ps.appointmentItemCode = item.appointmentItemCode;
        //        ps.positionTitle = item.positionTitle;
        //        ps.dailyRate = dailyRate;
        //        ps.monthlyRate = monthlyRate;
        //        ps.annualRate = annualSalary;
        //        ps.PERA = PERA;
        //        ps.leaveEarned = earnedLeave;
        //        ps.hazardPay = hazardPay;
        //        ps.subsistence = subsistence;
        //        ps.laundry = laundry;
        //        ps.midYearBonus = midYearBonus;
        //        ps.yearEndBonus = yearEndBonus;
        //        ps.cashGift = cashGift;
        //        ps.lifeRetmnt = lifeAndRetire;
        //        ps.ECC = ECC;
        //        ps.phic = PHIC;
        //        ps.clothing = clothing;
        //        ps.total = totalPS;
        //        ps.periodFrom = Convert.ToDateTime(periodFrom);
        //        ps.periodTo = item.periodTo;
        //        ps.userID = Session["_EIC"].ToString();
        //        ps.transDT = DateTime.Now;
        //        ps.PSTag = 1;
        //        db.tRSPZPersonnelServices.Add(ps);

        //        tRSPNONPlantilla np = new tRSPNONPlantilla();
        //        np.appointmentItemCode = item.appointmentItemCode;
        //        np.EIC = item.EIC;
        //        np.positionCode = item.positionCode;
        //        np.positionTitle = item.positionTitle;
        //        np.salaryGrade = item.salaryGrade;
        //        np.salary = item.salary;
        //        np.salaryType = item.salaryType;
        //        np.salaryDetailCode = "";
        //        np.periodFrom = item.periodFrom;
        //        np.periodTo = item.periodTo;
        //        np.fundSourceCode = item.fundSourceCode;
        //        np.projectName = item.projectName;
        //        np.programName = item.programName;
        //        np.employmentStatusCode = item.employmentStatusCode;
        //        np.warmBodyGroupCode = item.warmBodyGroupCode;
        //        np.userEIC = "";
        //        np.appTag = 1;
        //        np.transDT = DateTime.Now;
        //        db.tRSPNONPlantillas.Add(np);

        //        db.SaveChanges();

        //        IEnumerable<vRSPAppointmentNPEmployee> empList = db.vRSPAppointmentNPEmployees.Where(e => e.appointmentCode == item.appointmentCode).OrderBy(o => o.fullNameLast).ToList();
        //        IEnumerable<tRSPZPersonnelService> posted = db.tRSPZPersonnelServices.ToList();
        //        List<vRSPAppointmentNPEmployee> list = new List<vRSPAppointmentNPEmployee>();

        //        int appPostCounter = 0;

        //        foreach (vRSPAppointmentNPEmployee itm in empList)
        //        {
        //            int iCount = posted.Where(e => e.appointmentItemCode == itm.appointmentItemCode).Count();
        //            //
        //            if (iCount > 0) { appPostCounter = appPostCounter + 1; }

        //            list.Add(new vRSPAppointmentNPEmployee()
        //            {
        //                appointmentItemCode = itm.appointmentItemCode,
        //                EIC = itm.EIC,
        //                fullNameLast = itm.fullNameLast,
        //                fullNameFirst = itm.fullNameFirst,
        //                positionTitle = itm.positionTitle,
        //                salaryGrade = itm.salaryGrade,
        //                salary = itm.salary,
        //                salaryType = itm.salaryType,
        //                periodFrom = itm.periodFrom,
        //                periodTo = itm.periodTo,
        //                departmentCode = itm.departmentCode,
        //                tag = iCount
        //            });
        //        }

        //        //if (appPostCounter >= empList.Count())
        //        //{
        //        //    tRSPAppointmentCasual app = db.tRSPAppointmentCasuals.Single(e => e.appointmentCode == item.appointmentCode);
        //        //    app.tag = 2;
        //        //    db.SaveChanges();
        //        //}

        //        return 1;
        //    }
        //    catch (Exception)
        //    {
        //        return 0;
        //    }

        //}



        ////NG
        //[HttpPost]
        //public JsonResult PostCasualAppointmentItem_old(TempAppointmentNPPosting data)
        //{
        //    try
        //    {

        //        string periodFrom = data.periodFrom;
        //        int hazardCode = data.hazardCode;
        //        int hasMidYear = data.hasMidYear;
        //        int hasYearEnd = data.hasYearEnd;
        //        int hasClothing = data.hasClothing;


        //        vRSPAppointmentNPEmployee item = db.vRSPAppointmentNPEmployees.Single(e => e.appointmentItemCode == data.appointmentItemCode);

        //        DateTime pFrom;
        //        pFrom = Convert.ToDateTime(periodFrom);



        //        DateTime pTo = Convert.ToDateTime(item.periodTo);

        //        int monthCount = Monthcounter(pFrom, pTo);

        //        decimal dailyRate = Convert.ToDecimal(item.salary);
        //        decimal monthlyRate = dailyRate * monthCount;
        //        decimal annualSalary = monthlyRate * monthCount;
        //        decimal PERA = 2000 * monthCount;

        //        decimal earnedLeave = (monthlyRate / 8) * monthlyRate;

        //        decimal hazardPay = 0;
        //        decimal subsistence = 0;
        //        decimal laundry = 0;
        //        if (hazardCode == 1)
        //        {
        //            tRSPPosition position = db.tRSPPositions.Single(e => e.positionCode == item.positionCode);
        //            //HEALTH SERVICES
        //            hazardPay = CalQHazardForHealthMonthly(Convert.ToInt16(position.salaryGrade), Convert.ToDecimal(dailyRate)) * monthCount;
        //            //SUBS
        //            subsistence = 1500 * monthCount;
        //            laundry = 150 * monthCount;
        //        }
        //        else if (hazardCode == 2)
        //        {
        //            //SOCIAL WORKER
        //            hazardPay = CalQHazardForSocialWorkerMonthly(dailyRate) * monthCount;
        //            subsistence = 1500 * monthCount;
        //        }

        //        decimal midYearBonus = 0;
        //        if (hasMidYear == 1)
        //        {
        //            midYearBonus = dailyRate * 12;
        //        }

        //        decimal yearEndBonus = 0;
        //        decimal cashGift = 0;
        //        if (hasYearEnd == 1)
        //        {
        //            yearEndBonus = dailyRate * 12;
        //            cashGift = 5000;
        //        }


        //        decimal lifeAndRetire = (Convert.ToDecimal(monthlyRate) * Convert.ToDecimal(.12)) * monthCount;
        //        decimal PHIC = CalQPHICGovtShare(Convert.ToDouble(monthlyRate), monthCount);


        //        decimal HDMF = 100 * monthCount;
        //        decimal ECC = 100 * monthCount;

        //        decimal clothing = 0;
        //        if (hasClothing == 1)  //MARCH 30 but if HEALTH PERSONNEL NO CUT-OFF
        //        {
        //            clothing = 6000;
        //        }

        //        decimal totalPS = annualSalary + PERA + earnedLeave + hazardPay + subsistence + laundry + midYearBonus + yearEndBonus + cashGift + lifeAndRetire + ECC + PHIC + clothing;


        //        tRSPRefP ps = new tRSPRefP();
        //        ps.appointmentItemCode = item.appointmentItemCode;
        //        ps.positionTitle = item.positionTitle;
        //        ps.dailyRate = dailyRate;
        //        ps.monthlyRate = monthCount;
        //        ps.annualRate = annualSalary;
        //        ps.PERA = PERA;
        //        ps.leaveEarned = earnedLeave;
        //        ps.midYearBonus = midYearBonus;
        //        ps.yearEndBonus = yearEndBonus;
        //        ps.cashGift = cashGift;
        //        ps.lifeRetmnt = lifeAndRetire;
        //        ps.ECC = ECC;
        //        ps.phic = PHIC;
        //        ps.clothing = clothing;
        //        ps.total = totalPS;
        //        ps.periodFrom = Convert.ToDateTime(periodFrom);
        //        ps.periodTo = item.periodTo;
        //        ps.userID = Session["_EIC"].ToString();
        //        ps.transDT = DateTime.Now;
        //        ps.PSTag = 1;
        //        db.tRSPRefPS.Add(ps);
        //        db.SaveChanges();


        //        IEnumerable<vRSPAppointmentNPEmployee> empList = db.vRSPAppointmentNPEmployees.Where(e => e.appointmentCode == item.appointmentCode).OrderBy(o => o.fullNameLast).ToList();
        //        IEnumerable<tRSPRefP> posted = db.tRSPRefPS.ToList();
        //        List<vRSPAppointmentNPEmployee> list = new List<vRSPAppointmentNPEmployee>();

        //        int appPostCounter = 0;

        //        foreach (vRSPAppointmentNPEmployee itm in empList)
        //        {
        //            int iCount = posted.Where(e => e.appointmentItemCode == itm.appointmentItemCode).Count();
        //            //
        //            if (iCount > 0) { appPostCounter = appPostCounter + 1; }

        //            list.Add(new vRSPAppointmentNPEmployee()
        //            {
        //                appointmentItemCode = itm.appointmentItemCode,
        //                EIC = itm.EIC,
        //                fullNameLast = itm.fullNameLast,
        //                fullNameFirst = itm.fullNameFirst,
        //                positionTitle = itm.positionTitle,
        //                salaryGradeInt = itm.salaryGradeInt,
        //                salaryGrade = itm.salaryGrade,
        //                salary = itm.salary,
        //                salaryType = itm.salaryType,
        //                periodFrom = itm.periodFrom,
        //                periodTo = itm.periodTo,
        //                departmentCode = itm.departmentCode,
        //                tag = iCount
        //            });
        //        }

        //        if (appPostCounter >= empList.Count())
        //        {
        //            tRSPAppointmentCasual app = db.tRSPAppointmentCasuals.Single(e => e.appointmentCode == item.appointmentCode);
        //            app.tag = 2;
        //            db.SaveChanges();
        //        }


        //        return Json(new { status = "success", appointeeList = list }, JsonRequestBehavior.AllowGet);

        //    }
        //    catch (Exception)
        //    {

        //        return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
        //    }
        //}


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

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // JOB ORDER
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ActionResult JobOrder()
        {
            Session["AppointeeList"] = null;
            return View();
        }


        //NG
        //[HttpPost]
        //public JsonResult AddAppointeeJobOrder(tRSPAppointmentCasualEmp data, string periodFrom, string periodTo)
        //{
        //    if (data.EIC == null || data.positionCode == null)
        //    {
        //        return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
        //    }

        //    tRSPEmployee employee = db.tRSPEmployees.SingleOrDefault(e => e.EIC == data.EIC);
        //    tRSPPosition position = db.tRSPPositions.SingleOrDefault(e => e.positionCode == data.positionCode);

        //    if (employee == null || position == null)
        //    {
        //        return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
        //    }

        //    //MAKE VALIDATION HERE ----------------

        //    decimal ps = CalQPSJobOrer(position.positionCode, periodFrom, periodTo);

        //    List<AppointmentNonPlantillaData> finalList = new List<AppointmentNonPlantillaData>();
        //    if (data.EIC != null && data.positionCode != null)
        //    {

        //        vRSPSalaryDetailCurrent salary = db.vRSPSalaryDetailCurrents.Single(e => e.salaryGrade == position.salaryGrade && e.step == 1);

        //        var tempList = Session["AppointeeList"];
        //        List<AppointmentNonPlantillaData> list = tempList as List<AppointmentNonPlantillaData>;

        //        if (list == null)
        //        {
        //            List<AppointmentNonPlantillaData> temp = new List<AppointmentNonPlantillaData>();
        //            temp.Add(new AppointmentNonPlantillaData()
        //            {
        //                EIC = employee.EIC,
        //                fullNameLast = employee.fullNameLast,
        //                lastName = employee.lastName,
        //                firstName = employee.firstName,
        //                extName = employee.extName,
        //                middleName = employee.middleName,
        //                positionCode = position.positionCode,
        //                positionTitle = position.positionTitle,
        //                salaryDetailCode = salary.salaryDetailCode,
        //                salaryGrade = "SG " + position.salaryGrade + "/1",
        //                salary = Convert.ToDecimal(salary.rateDay),
        //                warmBodyGroupCode = data.warmBodyGroupCode,
        //                PS = ps,
        //            });
        //            finalList = temp;
        //            Session["AppointeeList"] = temp;
        //        }
        //        else
        //        {
        //            list.Add(new AppointmentNonPlantillaData()
        //            {
        //                EIC = employee.EIC,
        //                fullNameLast = employee.fullNameLast,
        //                lastName = employee.lastName,
        //                firstName = employee.firstName,
        //                extName = employee.extName,
        //                middleName = employee.middleName,
        //                positionCode = position.positionCode,
        //                positionTitle = position.positionTitle,
        //                salaryDetailCode = salary.salaryDetailCode,
        //                salaryGrade = "SG " + position.salaryGrade + "/1",
        //                salary = Convert.ToDecimal(salary.rateDay),
        //                warmBodyGroupCode = data.warmBodyGroupCode,
        //                PS = ps,
        //            });
        //            finalList = list;
        //            finalList = finalList.OrderBy(o => o.fullNameLast).ToList();
        //            Session["AppointeeList"] = finalList;
        //        }
        //    }
        //    return Json(new { status = "success", list = finalList }, JsonRequestBehavior.AllowGet);
        //}


        //NG
        [HttpPost]
        public JsonResult SaveJobOrderAppointment(tRSPAppointmentCasual data, string periodF, string periodT)
        {
            //string tmpPeriod = Session["ApptDate"].ToString();
            if (data.fundSourceCode == null || data.appointmentName == null)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }

            var tempList = Session["AppointeeList"];
            List<AppointmentNonPlantillaData> list = tempList as List<AppointmentNonPlantillaData>;

            string apptCode = "APPT" + DateTime.Now.ToString("yyMMddHHmmssfff");
            string itemCode = "APPI";

            DateTime periodFrom = Convert.ToDateTime(periodF);
            DateTime periodTo = Convert.ToDateTime(periodT);

            try
            {
                int counter = 0;
                foreach (AppointmentNonPlantillaData item in list)
                {
                    counter = counter + 1;
                    string code = itemCode + apptCode.Substring(4, 15) + counter.ToString("00");
                    tRSPAppointmentCasualEmp a = new tRSPAppointmentCasualEmp();
                    a.appointmentItemCode = code;
                    a.appointmentCode = apptCode;
                    a.EIC = item.EIC;
                    a.positionCode = item.positionCode;
                    a.positionTitle = item.positionTitle;
                    //a.salary = item.salary;
                    a.salaryType = "D";
                    if (periodF != null || periodF != "")
                    {
                        a.periodFrom = periodFrom;
                    }
                    a.periodTo = Convert.ToDateTime(periodTo);
                    a.PS = item.PS;
                    a.warmBodyGroupCode = item.warmBodyGroupCode;
                    db.tRSPAppointmentCasualEmps.Add(a);
                }
                tRSPAppointmentCasual appt = new tRSPAppointmentCasual();
                appt.appointmentCode = apptCode;
                appt.appNatureCode = data.appNatureCode;
                appt.appointmentName = data.appointmentName;
                appt.fundSourceCode = data.fundSourceCode;
                appt.userEIC = Session["EIC"].ToString();
                appt.transDT = DateTime.Now;
                appt.employmentStatusCode = "06"; //JOBORDER
                appt.tag = 0;
                db.tRSPAppointmentCasuals.Add(appt);

                db.SaveChanges();
                return Json(new { status = "success", apptCode = apptCode }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }

        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // HONORARIUM
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public ActionResult Honorarium()
        {
            Session["AppointeeList"] = null;
            return View();
        }


        //NG
        [HttpPost]
        public JsonResult HonorariumApptData()
        {
            IEnumerable<vRSPRefFundSource> fundSource = db.vRSPRefFundSources.OrderBy(o => o.projectName).ToList();
            return Json(new { status = "success", fSource = fundSource }, JsonRequestBehavior.AllowGet);
        }


        //NG
        [HttpPost]
        public JsonResult GetAppointeeAndPositionHonorarium()
        {

            IEnumerable<tRSPEmployee> empList = db.tRSPEmployees.Where(e => e.tag == 1).OrderBy(o => o.fullNameLast).ToList();

            List<tRSPEmployee> newList = new List<tRSPEmployee>();
            foreach (tRSPEmployee item in empList)
            {
                newList.Add(new tRSPEmployee()
                {
                    EIC = item.EIC,
                    fullNameLast = item.fullNameLast
                });
            }

            IEnumerable<tRSPPositionContract> positionList = db.tRSPPositionContracts.Where(e => e.isActive == 1).OrderBy(o => o.positionTitle).ToList();
            List<tRSPPositionContract> posList = new List<tRSPPositionContract>();

            foreach (tRSPPositionContract item in positionList)
            {
                posList.Add(new tRSPPositionContract()
                {
                    positionCode = item.positionCode,
                    positionTitle = item.positionDesc
                });
            }


            IEnumerable<tRSPWarmBody> wbList = db.tRSPWarmBodies.Where(e => e.tag == 1).OrderBy(o => o.orderNo).ToList();

            List<tRSPWarmBody> newWarmBodyList = new List<tRSPWarmBody>();

            foreach (tRSPWarmBody item in wbList)
            {
                newWarmBodyList.Add(new tRSPWarmBody()
                {
                    warmBodyGroupCode = item.warmBodyGroupCode,
                    warmBodyGroupName = item.warmBodyGroupName
                });
            }


            return Json(new { status = "success", empList = newList, positionList = posList, warmBodyList = newWarmBodyList }, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public JsonResult GetAppointeeAndPositionContract()
        {
            IEnumerable<tRSPEmployee> empList = db.tRSPEmployees.Where(e => e.recNo >= 1000).OrderBy(o => o.fullNameLast).ToList();
            List<tRSPEmployee> newList = new List<tRSPEmployee>();

            IEnumerable<vRSPPlantilla> plantillaEmp = db.vRSPPlantillas.Where(e => e.isFunded == true && e.EIC != null).ToList();

            foreach (tRSPEmployee item in empList)
            {

                int iCount = plantillaEmp.Where(e => e.EIC == item.EIC).Count();
                if (iCount == 0)
                {
                    newList.Add(new tRSPEmployee()
                    {
                        EIC = item.EIC,
                        fullNameLast = item.fullNameLast
                    });
                }
            }

            IEnumerable<tRSPPositionContract> positionList = db.tRSPPositionContracts.Where(e => e.isActive == 1).OrderBy(o => o.positionTitle).ToList();
            List<tRSPPositionContract> posList = new List<tRSPPositionContract>();

            foreach (tRSPPositionContract item in positionList)
            {
                posList.Add(new tRSPPositionContract()
                {
                    positionCode = item.positionCode,
                    positionTitle = item.positionDesc
                });
            }

            IEnumerable<tRSPWarmBody> wbList = db.tRSPWarmBodies.Where(e => e.tag == 1).OrderBy(o => o.orderNo).ToList();
            List<tRSPWarmBody> newWarmBodyList = new List<tRSPWarmBody>();
            foreach (tRSPWarmBody item in wbList)
            {
                newWarmBodyList.Add(new tRSPWarmBody()
                {
                    warmBodyGroupCode = item.warmBodyGroupCode,
                    warmBodyGroupName = item.warmBodyGroupName
                });
            }
            return Json(new { status = "success", empList = newList, positionList = posList, warmBodyList = newWarmBodyList }, JsonRequestBehavior.AllowGet);
        }



        //NG
        [HttpPost]
        public JsonResult AddContractAppointee(tRSPAppointmentCasualEmp data)
        {

            if (data.EIC == null || data.positionCode == null)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }

            tRSPEmployee employee = db.tRSPEmployees.SingleOrDefault(e => e.EIC == data.EIC);
            tRSPPositionContract position = db.tRSPPositionContracts.SingleOrDefault(e => e.positionCode == data.positionCode);
            tRSPWarmBody warmBody = db.tRSPWarmBodies.SingleOrDefault(e => e.warmBodyGroupCode == data.warmBodyGroupCode);

            if (employee == null || position == null)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }

            //MAKE VALIDATION HERE ----------------
            List<AppointmentNonPlantillaData> finalList = new List<AppointmentNonPlantillaData>();
            if (data.EIC != null && data.positionCode != null)
            {

                //vRSPSalaryDetailCurrent salary = db.vRSPSalaryDetailCurrents.Single(e => e.salaryGrade == position.salaryGrade && e.step == 1);
                decimal salary = Convert.ToDecimal(position.salary);

                var tempList = Session["AppointeeList"];
                List<AppointmentNonPlantillaData> list = tempList as List<AppointmentNonPlantillaData>;

                if (list == null)
                {
                    List<AppointmentNonPlantillaData> temp = new List<AppointmentNonPlantillaData>();
                    temp.Add(new AppointmentNonPlantillaData()
                    {
                        EIC = employee.EIC,
                        fullNameLast = employee.fullNameLast,
                        lastName = employee.lastName,
                        firstName = employee.firstName,
                        extName = employee.extName,
                        middleName = employee.middleName,
                        positionCode = position.positionCode,
                        positionTitle = position.positionTitle,
                        warmBodyGroupCode = data.warmBodyGroupCode,
                        warmBodyGroupName = warmBody.warmBodyGroupName,
                        //salary = Convert.ToDecimal(salary),
                        salaryTypeCode = "M",
                        PS = 0,
                    });
                    finalList = temp;
                    Session["AppointeeList"] = temp;
                }
                else
                {
                    list.Add(new AppointmentNonPlantillaData()
                    {
                        EIC = employee.EIC,
                        fullNameLast = employee.fullNameLast,
                        lastName = employee.lastName,
                        firstName = employee.firstName,
                        extName = employee.extName,
                        middleName = employee.middleName,
                        positionCode = position.positionCode,
                        positionTitle = position.positionTitle,
                        warmBodyGroupCode = data.warmBodyGroupCode,
                        warmBodyGroupName = warmBody.warmBodyGroupName,
                        //salary = Convert.ToDecimal(salary),
                        salaryTypeCode = "M",
                        PS = 0,
                    });
                    finalList = list;
                    finalList = finalList.OrderBy(o => o.fullNameLast).ToList();
                    Session["AppointeeList"] = finalList;
                }
            }
            return Json(new { status = "success", list = finalList }, JsonRequestBehavior.AllowGet);
        }

        ////NG
        //[HttpPost]
        //public JsonResult SaveHonorariumAppointment(tRSPAppointmentCasual data, string periodF, string periodT)
        //{
        //    //string tmpPeriod = Session["ApptDate"].ToString();
        //    if (data.fundSourceCode == null || data.appointmentName == null)
        //    {
        //        return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
        //    }

        //    var tempList = Session["AppointeeList"];
        //    List<AppointmentNonPlantillaData> list = tempList as List<AppointmentNonPlantillaData>;

        //    string apptCode = "APPT" + DateTime.Now.ToString("yyMMddHHmmssfff");
        //    string itemCode = "APPI";

        //    DateTime periodFrom = Convert.ToDateTime(periodF);
        //    DateTime periodTo = Convert.ToDateTime(periodT);

        //    try
        //    {
        //        int counter = 0;
        //        foreach (AppointmentNonPlantillaData item in list)
        //        {
        //            counter = counter + 1;
        //            string code = itemCode + apptCode.Substring(4, 15) + counter.ToString("00");
        //            tRSPAppointmentCasualEmp a = new tRSPAppointmentCasualEmp();
        //            a.appointmentItemCode = code;
        //            a.appointmentCode = apptCode;
        //            a.EIC = item.EIC;
        //            a.positionCode = item.positionCode;
        //            a.positionTitle = item.positionTitle;
        //            a.salaryGrade = Convert.ToInt16(item.salaryGrade);
        //            // a.salary = item.salary;
        //            a.salaryType = "M";
        //            if (periodF != null || periodF != "")
        //            {
        //                a.periodFrom = periodFrom;
        //            }
        //            a.periodTo = Convert.ToDateTime(periodTo);
        //            a.warmBodyGroupCode = item.warmBodyGroupCode;
        //            db.tRSPAppointmentCasualEmps.Add(a);
        //        }
        //        tRSPAppointmentCasual appt = new tRSPAppointmentCasual();
        //        appt.appointmentCode = apptCode;
        //        appt.appNatureCode = data.appNatureCode;
        //        appt.appointmentName = data.appointmentName;
        //        appt.fundSourceCode = data.fundSourceCode;
        //        appt.userEIC = Session["EIC"].ToString();
        //        appt.transDT = DateTime.Now;
        //        appt.employmentStatusCode = "08"; //HONORARIUM
        //        appt.tag = 0;
        //        db.tRSPAppointmentCasuals.Add(appt);

        //        db.SaveChanges();
        //        return Json(new { status = "success", apptCode = apptCode }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception)
        //    {
        //        return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
        //    }

        //}


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // CONTRACT OF SERVER
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ActionResult Contract()
        {
            Session["AppointeeList"] = null;
            return View();
        }


        //[HttpPost]
        //public JsonResult SaveContractAppointment(tRSPAppointmentCasual data, string periodF, string periodT)
        //{
        //    //string tmpPeriod = Session["ApptDate"].ToString();
        //    if (data.fundSourceCode == null || data.appointmentName == null)
        //    {
        //        return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
        //    }

        //    var tempList = Session["AppointeeList"];
        //    List<AppointmentNonPlantillaData> list = tempList as List<AppointmentNonPlantillaData>;

        //    string apptCode = "APPT" + DateTime.Now.ToString("yyMMddHHmmssfff");
        //    string itemCode = "APPI";

        //    DateTime periodFrom = Convert.ToDateTime(periodF);
        //    DateTime periodTo = Convert.ToDateTime(periodT);

        //    try
        //    {
        //        int counter = 0;
        //        foreach (AppointmentNonPlantillaData item in list)
        //        {
        //            counter = counter + 1;
        //            string code = itemCode + apptCode.Substring(4, 15) + counter.ToString("00");
        //            tRSPAppointmentCasualEmp a = new tRSPAppointmentCasualEmp();
        //            a.appointmentItemCode = code;
        //            a.appointmentCode = apptCode;
        //            a.EIC = item.EIC;
        //            a.positionCode = item.positionCode;
        //            a.positionTitle = item.positionTitle;
        //            //a.salaryGrade = Convert.ToInt16(item.salaryGrade);
        //            //a.salary = item.salary;
        //            a.salaryType = "M";
        //            if (periodF != null || periodF != "")
        //            {
        //                a.periodFrom = periodFrom;
        //            }
        //            a.periodTo = Convert.ToDateTime(periodTo);
        //            a.warmBodyGroupCode = item.warmBodyGroupCode;
        //            db.tRSPAppointmentCasualEmps.Add(a);
        //        }
        //        tRSPAppointmentCasual appt = new tRSPAppointmentCasual();
        //        appt.appointmentCode = apptCode;
        //        appt.appNatureCode = data.appNatureCode;
        //        appt.appointmentName = data.appointmentName;
        //        appt.fundSourceCode = data.fundSourceCode;
        //        appt.userEIC = Session["EIC"].ToString();
        //        appt.transDT = DateTime.Now;
        //        appt.employmentStatusCode = "07"; //HONORARIUM
        //        appt.tag = 0;
        //        db.tRSPAppointmentCasuals.Add(appt);

        //        db.SaveChanges();
        //        return Json(new { status = "success", apptCode = apptCode }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception)
        //    {
        //        return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
        //    }

        //}



        ////NG
        //[HttpPost]
        //public JsonResult AddContractAppointee(tRSPAppointmentCasualEmp data)
        //{

        //    if (data.EIC == null || data.positionCode == null)
        //    {
        //        return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
        //    }


        //    tRSPEmployee employee = db.tRSPEmployees.SingleOrDefault(e => e.EIC == data.EIC);
        //    tRSPPosition position = db.tRSPPositionContracts.Single(e => e.positionCode == data.positionCode);

        //    if (employee == null || position == null)
        //    {
        //        return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
        //    }

        //    //MAKE VALIDATION HERE ----------------
        //    List<AppointmentNonPlantillaData> finalList = new List<AppointmentNonPlantillaData>();
        //    if (data.EIC != null && data.positionCode != null)
        //    {

        //        vRSPSalaryDetailCurrent salary = db.vRSPSalaryDetailCurrents.Single(e => e.salaryGrade == position.salaryGrade && e.step == 1);

        //        var tempList = Session["AppointeeList"];
        //        List<AppointmentNonPlantillaData> list = tempList as List<AppointmentNonPlantillaData>;

        //        if (list == null)
        //        {
        //            List<AppointmentNonPlantillaData> temp = new List<AppointmentNonPlantillaData>();
        //            temp.Add(new AppointmentNonPlantillaData()
        //            {
        //                EIC = employee.EIC,
        //                fullNameLast = employee.fullNameLast,
        //                lastName = employee.lastName,
        //                firstName = employee.firstName,
        //                extName = employee.extName,
        //                middleName = employee.middleName,
        //                positionCode = position.positionCode,
        //                positionTitle = position.positionTitle,
        //                salaryDetailCode = salary.salaryDetailCode,
        //                salaryGrade = "SG " + position.salaryGrade + "/1",
        //                salary = Convert.ToDecimal(salary.rateDay),
        //                warmBodyGroupCode = data.warmBodyGroupCode,
        //                budgetAlloc = 0,
        //            });
        //            finalList = temp;
        //            Session["AppointeeList"] = temp;
        //        }
        //        else
        //        {
        //            list.Add(new AppointmentNonPlantillaData()
        //            {
        //                EIC = employee.EIC,
        //                fullNameLast = employee.fullNameLast,
        //                lastName = employee.lastName,
        //                firstName = employee.firstName,
        //                extName = employee.extName,
        //                middleName = employee.middleName,
        //                positionCode = position.positionCode,
        //                positionTitle = position.positionTitle,
        //                salaryDetailCode = salary.salaryDetailCode,
        //                salaryGrade = "SG " + position.salaryGrade + "/1",
        //                salary = Convert.ToDecimal(salary.rateDay),
        //                warmBodyGroupCode = data.warmBodyGroupCode,
        //                budgetAlloc = 0,
        //            });
        //            finalList = list;
        //            finalList = finalList.OrderBy(o => o.fullNameLast).ToList();
        //            Session["AppointeeList"] = finalList;
        //        }
        //    }
        //    return Json(new { status = "success", list = finalList }, JsonRequestBehavior.AllowGet);
        //}


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // POSTING 1.) JOB ORDER 2.) COS 3.) HONORARIUM
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private decimal CalQPSJobOrder(tRSPPosition position, string pFrom, string pTo)
        {
            try
            {
                vRSPSalarySchedJO salary = db.vRSPSalarySchedJOes.Single(e => e.salaryGrade == position.salaryGrade);
                DateTime periodFrom = Convert.ToDateTime(pFrom);
                DateTime periodTo = Convert.ToDateTime(pTo);
                int monthCount = Monthcounter(periodFrom, periodTo);
                decimal monthlyRate = Convert.ToDecimal(salary.rateMonth);
                decimal annualSalary = monthlyRate * monthCount;
                decimal totalPS = annualSalary;
                return totalPS;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private decimal CalQPSContract(tRSPPositionContract position, string pFrom, string pTo)
        {
            try
            {
                DateTime periodFrom = Convert.ToDateTime(pFrom);
                DateTime periodTo = Convert.ToDateTime(pTo);
                int monthCount = Monthcounter(periodFrom, periodTo);
                decimal monthlyRate = Convert.ToDecimal(position.salary);
                decimal annualSalary = monthlyRate * monthCount;
                decimal totalPS = annualSalary;
                return totalPS;
            }
            catch (Exception)
            {
                return 0;
            }
        }


        //NG
        [HttpPost]
        public JsonResult PostContractAppointmentItem(TempAppointmentNPPosting data)
        {
            try
            {

                string periodFrom = data.periodFrom;
                vRSPAppointmentNPEmployee item = db.vRSPAppointmentNPEmployees.Single(e => e.appointmentItemCode == data.appointmentItemCode);
                decimal dailyRate = 0;
                if (item.salaryType == "M")
                {
                    dailyRate = Convert.ToDecimal(item.salary) / 22;
                }
                else if (item.salaryType == "D")
                {
                    dailyRate = Convert.ToDecimal(item.salary);
                }

                DateTime pFrom;
                pFrom = Convert.ToDateTime(periodFrom);

                DateTime pTo = Convert.ToDateTime(item.periodTo);

                int monthCount = Monthcounter(pFrom, pTo);

                decimal monthlyRate = Convert.ToDecimal(item.salary) * monthCount;
                decimal annualSalary = monthlyRate * monthCount;
                decimal totalPS = annualSalary;


                tRSPZPersonnelService ps = new tRSPZPersonnelService();
                ps.appointmentItemCode = item.appointmentItemCode;
                ps.positionTitle = item.positionTitle;
                ps.dailyRate = dailyRate;
                ps.monthlyRate = monthCount;
                ps.annualRate = annualSalary;
                ps.PERA = 0;
                ps.leaveEarned = 0;
                ps.midYearBonus = 0;
                ps.yearEndBonus = 0;
                ps.cashGift = 0;
                ps.lifeRetmnt = 0;
                ps.ECC = 0;
                ps.phic = 0;
                ps.clothing = 0;
                ps.total = totalPS;
                ps.periodFrom = Convert.ToDateTime(periodFrom);
                ps.periodTo = item.periodTo;
                ps.userID = Session["_EIC"].ToString();
                ps.transDT = DateTime.Now;
                ps.PSTag = 1;
                db.tRSPZPersonnelServices.Add(ps);
                db.SaveChanges();

                IEnumerable<vRSPAppointmentNPEmployee> empList = db.vRSPAppointmentNPEmployees.Where(e => e.appointmentCode == item.appointmentCode).OrderBy(o => o.fullNameLast).ToList();
                IEnumerable<tRSPZPersonnelService> posted = db.tRSPZPersonnelServices.ToList();
                List<vRSPAppointmentNPEmployee> list = new List<vRSPAppointmentNPEmployee>();

                int appPostCounter = 0;

                foreach (vRSPAppointmentNPEmployee itm in empList)
                {
                    int iCount = posted.Where(e => e.appointmentItemCode == itm.appointmentItemCode).Count();
                    if (iCount > 0) { appPostCounter = appPostCounter + 1; }

                    list.Add(new vRSPAppointmentNPEmployee()
                    {
                        appointmentItemCode = itm.appointmentItemCode,
                        EIC = itm.EIC,
                        fullNameLast = itm.fullNameLast,
                        fullNameFirst = itm.fullNameFirst,
                        positionTitle = itm.positionTitle,
                        salaryGrade = itm.salaryGrade,
                        salary = itm.salary,
                        salaryType = itm.salaryType,
                        periodFrom = itm.periodFrom,
                        periodTo = itm.periodTo,
                        departmentCode = itm.departmentCode,
                        tag = iCount
                    });
                }

                if (appPostCounter >= empList.Count())
                {
                    tRSPAppointmentCasual app = db.tRSPAppointmentCasuals.Single(e => e.appointmentCode == item.appointmentCode);
                    app.tag = 2;
                    db.SaveChanges();
                }

                return Json(new { status = "success", appointeeList = list }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {

                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        [HttpPost]
        public JsonResult SetupApptNPPrintingData(string id)
        {
            try
            {
                Session["AppointmentCode"] = id;
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }


        }

        public ActionResult Printing()
        {
            return View();
        }


        public JsonResult LoadAppointeeList()
        {
            try
            {
                string code = Session["AppointmentCode"].ToString();


                IEnumerable<vRSPAppointmentNPEmployee> empList = db.vRSPAppointmentNPEmployees.Where(e => e.appointmentCode == code).OrderBy(o => o.fullNameLast).ToList();
                IEnumerable<tRSPZPersonnelService> posted = db.tRSPZPersonnelServices.ToList();
                List<vRSPAppointmentNPEmployee> list = new List<vRSPAppointmentNPEmployee>();
                foreach (vRSPAppointmentNPEmployee item in empList)
                {
                    int iCount = posted.Where(e => e.appointmentItemCode == item.appointmentItemCode).Count();
                    list.Add(new vRSPAppointmentNPEmployee()
                    {
                        appointmentItemCode = item.appointmentItemCode,
                        EIC = item.EIC,
                        fullNameLast = item.fullNameLast,
                        fullNameFirst = item.fullNameFirst,
                        positionTitle = item.positionTitle,
                        salaryGrade = item.salaryGrade,
                        salary = item.salary,
                        salaryType = item.salaryType,
                        periodFrom = item.periodFrom,
                        periodTo = item.periodTo,
                        departmentCode = item.departmentCode,
                        tag = iCount
                    });
                }
                return Json(new { status = "success", appointeeList = list, appCode = code }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult PrintFormPreviewSetup(string type, string id)
        {
            try
            {
                string tmp = "";
                OATHDATATemp oData = new OATHDATATemp();
                AssumptionDataTemp aData = new AssumptionDataTemp();
                PDFData pdfData = new PDFData();
                bool isReadOnlyTag = true;
                if (type == "ASSUMPTION")
                {
                    tRSPAppointmentCasualEmp appItem = db.tRSPAppointmentCasualEmps.Single(e => e.appointmentItemCode == id);
                    tRSPEmployee emp = db.tRSPEmployees.Single(e => e.EIC == appItem.EIC);
                    aData.fullNameTitle = emp.fullNameTitle;

                    vRSPAppointmentNonPlantilla app = db.vRSPAppointmentNonPlantillas.Single(e => e.appointmentCode == appItem.appointmentCode);

                    if (emp.namePrefix == null || emp.namePrefix.Length < 2 || emp.nameSuffix == null)
                    {
                        isReadOnlyTag = true;
                    }

                    if (appItem.assumptionDate == null || appItem.assumptionDate.Value.Year == 1)
                    {
                        isReadOnlyTag = true;
                    }
                    else
                    {
                        aData.assumptionDate = Convert.ToDateTime(appItem.assumptionDate);
                    }

                    aData.namePrefix = emp.namePrefix;
                    aData.nameSuffix = emp.nameSuffix;
                    aData.fullNameFirst = emp.fullNameFirst;

                    aData.fullNameTitle = emp.namePrefix + " " + emp.fullNameFirst;
                    if (emp.nameSuffix != null && emp.nameSuffix.Length >= 2)
                    {
                        aData.fullNameTitle = emp.namePrefix + " " + emp.fullNameFirst + ", " + emp.nameSuffix;
                    }

                    aData.PGHeadName = appItem.PGHeadName;
                    aData.PGHeadPosition = appItem.PGHeadPosition;

                    aData.officeAssignment = app.departmentName;

                    if (appItem.departmentName != null)
                    {
                        aData.officeAssignment = appItem.departmentName;
                    }

                    aData.namePrefix = emp.namePrefix;
                    if (appItem.PGHeadName == null || appItem.PGHeadName.Length <= 3)
                    {
                        DepartmentTemplate dept = DepartmentProfile(app.departmentCode);
                        aData.PGHeadName = dept.deptHeadPGName;
                        aData.PGHeadPosition = dept.deptHeadPGPos;
                        isReadOnlyTag = false;
                    }


                    tmp = "ASSUMPTION";
                }
                else if (type == "OATH")
                {

                    tRSPAppointmentCasualEmp appItem = db.tRSPAppointmentCasualEmps.Single(e => e.appointmentItemCode == id);
                    tRSPEmployee emp = db.tRSPEmployees.SingleOrDefault(e => e.EIC == appItem.EIC);

                    if (appItem.govtIDNo == null || appItem.govtIDNo.Length < 4)
                    {
                        oData.govtIDName = "PGDDN ID";
                        oData.govtIDNo = emp.idNo;
                        oData.govtIDIssued = emp.idDDNIssuedDate;
                        isReadOnlyTag = false;
                    }
                    else
                    {
                        oData.govtIDName = appItem.govtIDName;
                        oData.govtIDNo = appItem.govtIDNo;
                        oData.govtIDIssued = appItem.govtIDIssued;
                        isReadOnlyTag = true;
                    }
                    oData.completeAddress = emp.completeAddress;


                    tmp = "OATH";
                }

                else if (type == "PDF")
                {
                    tRSPAppointmentCasualEmp appItem = db.tRSPAppointmentCasualEmps.Single(e => e.appointmentItemCode == id);
                    pdfData.PGHeadName = appItem.PGHeadName;
                    pdfData.PGHeadPosition = appItem.PGHeadPosition;
                    pdfData.presentAppropAct = appItem.presentAppropAct;
                    pdfData.previousAppropAct = appItem.previousAppropAct;
                    pdfData.otherCompensation = appItem.otherCompensation;
                    pdfData.positionSupervisor = appItem.positionSupervisor;
                    pdfData.positionSupervisorNext = appItem.positionSupervisorNext;
                    pdfData.supervisedPositions = appItem.supervisedPositions;
                    pdfData.supervisedItemNo = appItem.supervisedItemNo;
                    pdfData.machineToolsUsed = appItem.machineToolsUsed;
                    pdfData.PDFManagerial = appItem.PDFManagerial >= 0 ? Convert.ToInt16(appItem.PDFManagerial) : -1;
                    pdfData.PDFSupervisor = appItem.PDFSupervisor >= 0 ? Convert.ToInt16(appItem.PDFSupervisor) : -1;
                    pdfData.PDFNonSupervisor = appItem.PDFNonSupervisor >= 0 ? Convert.ToInt16(appItem.PDFNonSupervisor) : -1;
                    pdfData.PDFStaff = appItem.PDFStaff >= 0 ? Convert.ToInt16(appItem.PDFStaff) : -1;
                    pdfData.PDFGenPublic = appItem.PDFGenPublic >= 0 ? Convert.ToInt16(appItem.PDFGenPublic) : -1; ;
                    pdfData.PDFOtherAgency = appItem.PDFOtherAgency >= 0 ? Convert.ToInt16(appItem.PDFOtherAgency) : -1;
                    pdfData.PDFOthers = appItem.PDFOthers;
                    tmp = "PDF";
                }

                else if (type == "FUNDS")
                {
                    tmp = "FUNDS";
                }

                return Json(new { status = "success", oData = oData, aData = aData, pdfData = pdfData, isReadOnlyTag = isReadOnlyTag }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {

                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult CheckDataForPrint(string type, string id)
        {
            int iStat = 0;
            string tmp = "";
            string tmpDetail = "";
            string code = "PRN" + DateTime.Now.ToString("yyMMddHHmmssfff");
            if (type == "ASSUMPTION")
            {
                tRSPAppointmentCasualEmp appItem = db.tRSPAppointmentCasualEmps.Single(e => e.appointmentItemCode == id);
                tRSPEmployee emp = db.tRSPEmployees.Single(e => e.EIC == appItem.EIC);
                vRSPAppointmentNonPlantilla app = db.vRSPAppointmentNonPlantillas.Single(e => e.appointmentCode == appItem.appointmentCode);
                //if (app.employmentStatusNameShort .employmentStatusCode == "06" || app.employmentStatusCode == "07" || app.employmentStatusCode == "08") { tmpDetail = "NONCSC"; }
                DepartmentTemplate dept = DepartmentProfile(app.departmentCode);
                tmp = "ASSUMPTION";
            }
            else if (type == "OATH")
            {

                tRSPAppointmentCasualEmp appItem = db.tRSPAppointmentCasualEmps.Single(e => e.appointmentItemCode == id);

                if (appItem.govtIDName == null || appItem.govtIDName == "")
                {
                    iStat = 1;
                }

                tRSPEmployee emp = db.tRSPEmployees.SingleOrDefault(e => e.EIC == appItem.EIC);
                if (emp != null)
                {
                    if (emp.completeAddress == null || emp.completeAddress.Length < 5)
                    {
                        iStat = 1;
                    }
                }

                tmp = "OATH";
            }

            else if (type == "PDF")
            {

            }
            else if (type == "PDFBACK")
            {

            }

            if (iStat == 0)
            {
                tRSPZPrintReport r = new tRSPZPrintReport();
                r.printID = code;
                r.printCode = id;
                r.printType = tmp;
                r.printDetail = "";
                r.userEIC = Session["_EIC"].ToString();
                r.transDT = DateTime.Now;
                db.tRSPZPrintReports.Add(r);
                db.SaveChanges();
            }

            return Json(new { status = "success", type = type, id = code, stat = iStat }, JsonRequestBehavior.AllowGet);

        }

        public class OATHDATATemp
        {
            public string appItemCode { get; set; }
            public string completeAddress { get; set; }
            public string govtIDName { get; set; }
            public string govtIDNo { get; set; }
            public DateTime? govtIDIssued { get; set; }
        }

        public class AssumptionDataTemp
        {
            public string appItemCode { get; set; }
            public DateTime? assumptionDate { get; set; }
            public string fullNameFirst { get; set; }
            public string namePrefix { get; set; }
            public string nameSuffix { get; set; }
            public string fullNameTitle { get; set; }
            public string PGHeadName { get; set; }
            public string PGHeadPosition { get; set; }
            public string officeAssignment { get; set; }
        }

        public class PDFData
        {
            public string appItemCode { get; set; }
            public string presentAppropAct { get; set; }
            public string previousAppropAct { get; set; }
            public string otherCompensation { get; set; }
            public string positionSupervisor { get; set; }
            public string positionSupervisorNext { get; set; }
            public string supervisedPositions { get; set; }
            public string supervisedItemNo { get; set; }
            public string machineToolsUsed { get; set; }
            public int PDFManagerial { get; set; }
            public int PDFSupervisor { get; set; }
            public int PDFNonSupervisor { get; set; }
            public int PDFStaff { get; set; }
            public int PDFGenPublic { get; set; }
            public int PDFOtherAgency { get; set; }
            public string PDFOthers { get; set; }
            public string PGHeadName { get; set; }
            public string PGHeadPosition { get; set; }
            public string page { get; set; }
        }

        [HttpPost]
        public JsonResult SaveAssumptionPrintData(AssumptionDataTemp data)
        {
            try
            {
                string code = "PRN" + DateTime.Now.ToString("yyMMddHHmmssfff");
                tRSPAppointmentCasualEmp appItem = db.tRSPAppointmentCasualEmps.Single(e => e.appointmentItemCode == data.appItemCode);

                appItem.assumptionDate = data.assumptionDate;
                appItem.fullNameTitle = data.fullNameTitle;
                appItem.PGHeadName = data.PGHeadName;
                appItem.PGHeadPosition = data.PGHeadPosition;
                appItem.departmentName = data.officeAssignment;

                vRSPAppointmentNPEmployee itemData = db.vRSPAppointmentNPEmployees.SingleOrDefault(e => e.appointmentItemCode == appItem.appointmentItemCode);

                tRSPEmployee emp = db.tRSPEmployees.Single(e => e.EIC == appItem.EIC);
                emp.fullNameTitle = data.fullNameTitle;
                emp.namePrefix = data.namePrefix;
                emp.nameSuffix = data.nameSuffix;

                db.SaveChanges();
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult SaveOathPrintData(OATHDATATemp data)
        {
            try
            {
                string code = "PRN" + DateTime.Now.ToString("yyMMddHHmmssfff");
                tRSPAppointmentCasualEmp appItem = db.tRSPAppointmentCasualEmps.Single(e => e.appointmentItemCode == data.appItemCode);

                appItem.govtIDName = data.govtIDName;
                appItem.govtIDNo = data.govtIDNo;
                appItem.govtIDIssued = data.govtIDIssued;

                tRSPEmployee emp = db.tRSPEmployees.Single(e => e.EIC == appItem.EIC);
                emp.completeAddress = data.completeAddress;
                if (emp.idNo == data.govtIDNo && emp.idDDNIssuedDate == null)
                {
                    emp.idDDNIssuedDate = data.govtIDIssued;
                }

                db.SaveChanges();

                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }




        [HttpPost]
        public JsonResult SavePDFPrintData(PDFData data)
        {
            try
            {
                tRSPAppointmentCasualEmp a = db.tRSPAppointmentCasualEmps.Single(e => e.appointmentItemCode == data.appItemCode);
                a.presentAppropAct = data.presentAppropAct;
                a.previousAppropAct = data.previousAppropAct;
                a.otherCompensation = data.otherCompensation;
                a.positionSupervisor = data.positionSupervisor;
                a.positionSupervisorNext = data.positionSupervisorNext;
                a.supervisedPositions = data.supervisedPositions;
                a.supervisedItemNo = data.supervisedItemNo;
                a.machineToolsUsed = data.machineToolsUsed;
                a.PGHeadName = data.PGHeadName;
                a.PDFManagerial = Convert.ToByte(data.PDFManagerial);
                a.PDFSupervisor = Convert.ToByte(data.PDFSupervisor);
                a.PDFNonSupervisor = Convert.ToByte(data.PDFNonSupervisor);
                a.PDFStaff = Convert.ToByte(data.PDFStaff);
                a.PDFGenPublic = Convert.ToByte(data.PDFGenPublic);
                a.PDFOtherAgency = Convert.ToByte(data.PDFOtherAgency);
                a.PDFOthers = data.PDFOthers;
                db.SaveChanges();

                //string code = "PRN" + DateTime.Now.ToString("yyMMddHHmmssfff");
                //tRSPZPrintReport r = new tRSPZPrintReport();
                //r.printID = code;
                //r.printCode = data.appItemCode;
                //r.printType = "PDF";
                //r.printDetail = "BACK";
                //r.userEIC = Session["_EIC"].ToString();
                //r.transDT = DateTime.Now;
                //db.tRSPZPrintReports.Add(r);
                db.SaveChanges();

                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public class DepartmentTemplate
        {
            public string departmentCode { get; set; }
            public string departmentName { get; set; }
            public string deptHeadPGName { get; set; }
            public string deptHeadPGPos { get; set; }

        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private DepartmentTemplate DepartmentProfile(string deptCode)
        {

            List<DepartmentTemplate> deptList = new List<DepartmentTemplate>();

            deptList.Add(new DepartmentTemplate()
            {
                departmentCode = "OC191015134332380001",
                departmentName = "Provincial Governor's Office",
                deptHeadPGName = "EDWIN I. JUBAHIB",
                deptHeadPGPos = "Governor"
            });
            deptList.Add(new DepartmentTemplate()
            {
                departmentCode = "OC191015134453048001",
                departmentName = "Provincial Administrator's Office",
                deptHeadPGName = "JOSIE JEAN R. RABANOZ, CE, MPA, EnP",
                deptHeadPGPos = "Provincial Administrator"
            });
            deptList.Add(new DepartmentTemplate()
            {
                departmentCode = "OC191015134636261001",
                departmentName = "Provincial Human Resource Management Office",
                deptHeadPGName = "EDWIN A. PALERO, MPA, MHRM",
                deptHeadPGPos = "Provincial Administrator"
            });

            deptList.Add(new DepartmentTemplate()
            {
                departmentCode = "OC191015134702339001",
                departmentName = "Provincial Information, Communication and Knowledge Management Office",
                deptHeadPGName = "MERVIN JAY Z. SUAYBAGUIO, Ph.D, DDM",
                deptHeadPGPos = "Provincial Information Officer"
            });
            deptList.Add(new DepartmentTemplate()
            {
                departmentCode = "OC191015152447314001",
                departmentName = "Provincial Planning and Development Office",
                deptHeadPGName = "NELSON F. PLATA, MPA, En.P.",
                deptHeadPGPos = "Provincial Planning and Development Coordinator"
            });
            deptList.Add(new DepartmentTemplate()
            {
                departmentCode = "OC191015152503876001",
                departmentName = "Provincial General Services Office",
                deptHeadPGName = "JOSEPH NILO F. PARREÑAS, MD",
                deptHeadPGPos = "Provincial General Services Officer"
            });
            deptList.Add(new DepartmentTemplate()
            {
                departmentCode = "OC191015152544060001",
                departmentName = "Provincial Budget Office",
                deptHeadPGName = "EMELIA C. PALERO, CPA, MSLRG",
                deptHeadPGPos = "Provincial Budget Officer"
            });
            deptList.Add(new DepartmentTemplate()
            {
                departmentCode = "OC191015152600833001",
                departmentName = "Provincial Accountant`s Office",
                deptHeadPGName = "WINONA J. AVENIDO, CPA, MPA",
                deptHeadPGPos = "Provincial Accountant"
            });
            deptList.Add(new DepartmentTemplate()
            {
                departmentCode = "OC191017163038011001",
                departmentName = "Provincial Legal Office",
                deptHeadPGName = "CHARINA C. CABRERA, CPA, REA, REB",
                deptHeadPGPos = "Provincial Legal Officer"
            });
            deptList.Add(new DepartmentTemplate()
            {
                departmentCode = "OC191017163348136001",
                departmentName = "Provincial Treasurer's Office",
                deptHeadPGName = "EVELYN G. ESPRA, MPA",
                deptHeadPGPos = "Provincial Treasurer"
            });
            deptList.Add(new DepartmentTemplate()
            {
                departmentCode = "OC191017163416304001",
                departmentName = "Provincial Assessor's Office",
                deptHeadPGName = "JOYCE T. GUALBERTO, CE, MPA, REA",
                deptHeadPGPos = "Provincial Assessor"
            });
            deptList.Add(new DepartmentTemplate()
            {
                departmentCode = "OC191017163920527001",
                departmentName = "Provincial Health Office",
                deptHeadPGName = "ALFREDO A. LACERONA, MD",
                deptHeadPGPos = "Provincial Health Officer II"
            });
            deptList.Add(new DepartmentTemplate()
            {
                departmentCode = "OC191017163949719001",
                departmentName = "Provincial Social Welfare and Development Office",
                deptHeadPGName = "ROSALINDA O. RAPISTA, RSW, MPA",
                deptHeadPGPos = "Provincial Social Welfare and Development Officer"
            });
            deptList.Add(new DepartmentTemplate()
            {
                departmentCode = "OC191017164019805001",
                departmentName = "Provincial Agriculturist's Office",
                deptHeadPGName = "JOSE L. ANDAMON, RA",
                deptHeadPGPos = "Provincial Agriculturist"
            });
            deptList.Add(new DepartmentTemplate()
            {
                departmentCode = "OC191017164040738001",
                departmentName = "Provincial Veterinarian's Office",
                deptHeadPGName = "RENATO R. EMBATE, DVM",
                deptHeadPGPos = "Provincial Veterinarian"
            });
            deptList.Add(new DepartmentTemplate()
            {
                departmentCode = "OC191017164103801001",
                departmentName = "Provincial Environment and Natural Resources Office",
                deptHeadPGName = "ROMULO D. TAGALO, Ph.D",
                deptHeadPGPos = "Provincial Environment and Natural Resources Officer"
            });
            deptList.Add(new DepartmentTemplate()
            {
                departmentCode = "OC191017164206284001",
                departmentName = "Provincial Engineer's Office",
                deptHeadPGName = "GLENN A. OLANDRIA, CE",
                deptHeadPGPos = "Provincial Engineer"
            });
            deptList.Add(new DepartmentTemplate()
            {
                departmentCode = "OC191017164319324001",
                departmentName = "Provincial Economic Enterprise Development Office",
                deptHeadPGName = "DENNIS B. DEVILLERES, Ll.B",
                deptHeadPGPos = "Provincial Government Department Head"
            });
            deptList.Add(new DepartmentTemplate()
            {
                departmentCode = "OC191017164344755001",
                departmentName = "Provincial Sports and Youth Development Office",
                deptHeadPGName = "GIOVANNI I. GULANES",
                deptHeadPGPos = "Provincial Government Department Head"
            });
            deptList.Add(new DepartmentTemplate()
            {
                departmentCode = "OC191017164406966001",
                departmentName = "Office of the Secretary to the Sangunian",
                deptHeadPGName = "DENNIS DEAN T. CASTILLO, MPA",
                deptHeadPGPos = "Provincial Government Department Head"
            });
            deptList.Add(new DepartmentTemplate()
            {
                departmentCode = "OC191017164422254001",
                departmentName = "Sangguniang Panlalawigan Office",
                deptHeadPGName = "REY T. UY",
                deptHeadPGPos = "Provincial Vice-Governor"
            });
            deptList.Add(new DepartmentTemplate()
            {
                departmentCode = "OC191017164437082001",
                departmentName = "Vice Governor's Office",
                deptHeadPGName = "REY T. UY",
                deptHeadPGPos = "Provincial Vice-Governor"
            });
            DepartmentTemplate dept = deptList.Single(e => e.departmentCode == deptCode);
            return dept;

        }

        public JsonResult PrintPDFCasual(string id, string type)
        {
            try
            {
                string repType = "PDFCASUAL";
                if (type == "BACK")
                {
                    repType = "PDFCASUALBACK";
                }
                Session["ReportType"] = repType;
                Session["PrintReport"] = id;
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult PrintPDFBackCasual(string id)
        {
            try
            {
                Session["ReportType"] = "PDFCASUALBACK";
                Session["PrintReport"] = id;
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public JsonResult ApptSelected(string id)
        {
            try
            {
                Session["_APPTCODE"] = id;
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult ViewDetail()
        {
            return View();
        }

        //public JsonResult ViewDetailInitData()
        //{
        //    try
        //    {
        //        string apptCode = Session["_APPTCODE"].ToString();
        //        vRSPAppointmentNonPlantilla data = db.vRSPAppointmentNonPlantillas.Single(e => e.appointmentCode == apptCode);
        //        IEnumerable<vRSPAppointmentNPEmployee> empList = db.vRSPAppointmentNPEmployees.Where(e => e.appointmentCode == apptCode).OrderBy(o => o.fullNameLast).ToList();
        //        IEnumerable<tRSPZPersonnelService> posted = db.tRSPZPersonnelServices.ToList();
        //        List<vRSPAppointmentNPEmployee> list = new List<vRSPAppointmentNPEmployee>();
        //        foreach (vRSPAppointmentNPEmployee item in empList)
        //        {
        //            //int iCount = posted.Where(e => e.appointmentItemCode == item.appointmentItemCode).Count();
        //            list.Add(new vRSPAppointmentNPEmployee()
        //            {
        //                appointmentItemCode = item.appointmentItemCode,
        //                EIC = item.EIC,
        //                fullNameLast = item.fullNameLast,
        //                fullNameFirst = item.fullNameFirst,
        //                positionTitle = item.positionTitle,
        //                salaryGrade = item.salaryGrade,
        //                salary = item.salary,
        //                salaryType = item.salaryType,
        //                periodFrom = item.periodFrom,
        //                periodTo = item.periodTo,
        //                departmentCode = item.departmentCode,
        //                empTag = item.empTag,
        //                tag = item.tag,
        //                postNo = item.postNo
        //            });
        //        }

        //        var workGroupList = db.tRSPWorkGroups.Select(e => new  {
        //            e.workGroupCode,
        //            e.workGroupName,
        //            e.orderNo
        //        }).OrderBy(e => e.orderNo).ToList() ;

        //        //CHECK IF ALL APPT. EMP ARE POSTED
        //        if (data.tag == 1)
        //        {
        //            int count = empList.Where(e => e.PSTag != 1).Count();
        //            if (count == 0)
        //            {
        //                tRSPAppointmentCasual appt = db.tRSPAppointmentCasuals.Single(e => e.appointmentCode == data.appointmentCode);
        //                appt.tag = 2; // IF ALL PS POSTED 
        //                db.SaveChanges();
        //                data.tag = 2;
        //            }
        //        }

        //        return Json(new { status = "success", apptData = data, appointeeList = list, workGroupList = workGroupList }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        string errMsg = ex.ToString();
        //        return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
        //    }

        //}

        [HttpPost]
        public JsonResult AppointeeList(string id)
        {
            try
            {
                //vRSPAppointmentNonPlantilla data = db.vRSPAppointmentNonPlantillas.Single(e => e.appointmentCode == apptCode);
                IEnumerable<vRSPAppointmentNPEmployee> empList = db.vRSPAppointmentNPEmployees.Where(e => e.appointmentCode == id).OrderBy(o => o.fullNameLast).ToList();
                //IEnumerable<tRSPZPersonnelService> posted = db.tRSPZPersonnelServices.ToList();
                List<vRSPAppointmentNPEmployee> list = new List<vRSPAppointmentNPEmployee>();
                foreach (vRSPAppointmentNPEmployee item in empList)
                {
                    //int iCount = posted.Where(e => e.appointmentItemCode == item.appointmentItemCode).Count();
                    list.Add(new vRSPAppointmentNPEmployee()
                    {
                        appointmentItemCode = item.appointmentItemCode,
                        EIC = item.EIC,
                        fullNameLast = item.fullNameLast,
                        fullNameFirst = item.fullNameFirst,
                        positionTitle = item.positionTitle,
                        salaryGrade = item.salaryGrade,
                        salary = item.salary,
                        salaryType = item.salaryType,
                        periodFrom = item.periodFrom,
                        periodTo = item.periodTo,
                        departmentCode = item.departmentCode,
                        empTag = item.empTag,
                        tag = item.tag,
                        postNo = item.postNo
                    });
                }


                tRSPAppointmentCasual app = db.tRSPAppointmentCasuals.Single(e => e.appointmentCode == id);
                if (app.tag == 2)
                {
                    int postedCounter = empList.Where(e => e.postNo > 0).Count();
                    if (empList.Count() == postedCounter)
                    {
                        app.tag = 3;
                        db.SaveChanges();
                    }
                }

                return Json(new { status = "success", appointeeList = list, appTag = app.tag }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }

        }




        public JsonResult DeleteAppointee(string id)
        {
            try
            {

                tRSPAppointmentCasualEmp delItem = db.tRSPAppointmentCasualEmps.Single(e => e.appointmentItemCode == id);


                db.tRSPAppointmentCasualEmps.Remove(delItem);
                db.SaveChanges();

                IEnumerable<vRSPAppointmentNPEmployee> empList = db.vRSPAppointmentNPEmployees.Where(e => e.appointmentCode == delItem.appointmentCode).OrderBy(o => o.fullNameLast).ToList();
                IEnumerable<tRSPZPersonnelService> posted = db.tRSPZPersonnelServices.ToList();
                List<vRSPAppointmentNPEmployee> list = new List<vRSPAppointmentNPEmployee>();
                foreach (vRSPAppointmentNPEmployee item in empList)
                {
                    int iCount = posted.Where(e => e.appointmentItemCode == item.appointmentItemCode).Count();
                    list.Add(new vRSPAppointmentNPEmployee()
                    {

                        appointmentItemCode = item.appointmentItemCode,
                        EIC = item.EIC,
                        fullNameLast = item.fullNameLast,
                        fullNameFirst = item.fullNameFirst,
                        positionTitle = item.positionTitle,
                        salaryGrade = item.salaryGrade,
                        salary = item.salary,
                        salaryType = item.salaryType,
                        periodFrom = item.periodFrom,
                        periodTo = item.periodTo,
                        departmentCode = item.departmentCode,
                        empTag = item.empTag,
                        tag = item.tag

                    });
                }
                return Json(new { status = "success", appointeeList = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult AddNewAppointee(tRSPAppointmentCasualEmp data)
        {
            try
            {
                if (data.EIC == null || data.positionCode == null)
                {
                    return Json(new { status = "Please fill-up the required data!" }, JsonRequestBehavior.AllowGet);
                }

                string apptCode = data.appointmentCode;

                vRSPAppointmentNonPlantilla app = db.vRSPAppointmentNonPlantillas.Single(e => e.appointmentCode == apptCode);

                tRSPEmployee employee = db.tRSPEmployees.SingleOrDefault(e => e.EIC == data.EIC);
                tRSPPosition position = new tRSPPosition();
                tRSPPositionSub subPosition = new tRSPPositionSub();
                vRSPSalaryDetailCurrent salary = new vRSPSalaryDetailCurrent();

                decimal rateDaily = 0;
                decimal rateMonthly = 0;
                decimal rateSalary = 0;
                decimal appointmentPS = 0;
                string salaryType = "";
                string salaryGradeText = "";
                string warmBodyGroupName = "";
                string salaryDetailCode = "";

                int hazardCode = 0;

                TempAppointmentNPPosting postData = new TempAppointmentNPPosting();
                postData.hazardCode = Convert.ToInt16(data.tag);

                tRSPZPersonnelService personnelService = new tRSPZPersonnelService();
                tRSPRefFundPersonnelService nps = new tRSPRefFundPersonnelService();

                string emptStat = app.employmentStatusCode;
                // 05-Casual 06-JobOrder 07-ContractOfService 08-Honorarium
                if (emptStat == "05")
                {

                    DateTime periodFrom = Convert.ToDateTime(app.periodFrom);
                    DateTime periodTo = Convert.ToDateTime("December 31 " + app.periodTo.Value.Year);

                    hazardCode = Convert.ToInt16(data.tag);
                    position = db.tRSPPositions.SingleOrDefault(e => e.positionCode == data.positionCode);
                    subPosition = db.tRSPPositionSubs.SingleOrDefault(e => e.subPositionCode == data.subPositionCode);

                    if (periodTo.Year == 2022)
                    {
                        tRSPSalaryTableDetail newSalary = db.tRSPSalaryTableDetails
                            .Single(e => e.salaryCode == "ST2022SSL05T03" && e.salaryGrade == position.salaryGrade && e.step == 1);


                        decimal rateMonth = Convert.ToDecimal(newSalary.rateMonth);
                        decimal rateDay = Convert.ToDecimal(newSalary.rateDay);
                        if (rateDaily == 0)
                        {
                            rateDay = rateMonth / 22;
                        }

                        int sg = Convert.ToInt16(position.salaryGrade);

                        PersonnelServiceViewModel psc = new PersonnelServiceViewModel();
                        //NEW P.S.
                        nps = psc.GetCasualFullYearPS(rateMonth, rateDay, sg, periodFrom, periodTo, 0);
                        nps.positionCode = data.positionCode;
                        nps.EIC = data.EIC;
                        nps.employmentStatusCode = "05";
                        nps.fundSourceCode = app.fundSourceCode;
                        nps.remarks = "";
                        nps.transDT = DateTime.Now;
                    }
                    else {
                        vRSPSalarySchedCasual sched = db.vRSPSalarySchedCasuals.Single(e => e.salaryGrade == position.salaryGrade);
                        personnelService = GetPositionCasualPS(sched, position, hazardCode, periodFrom, periodTo, postData);
                        rateSalary = Convert.ToDecimal(sched.rateDay);
                        rateDaily = Convert.ToDecimal(sched.rateDay);
                        rateMonthly = Convert.ToDecimal(sched.rateMonth);
                        salaryGradeText = "SG " + position.salaryGrade + "/1";
                        salaryDetailCode = sched.salaryDetailCode;
                        salaryType = "D";
                    }

                 

                }
                else if (emptStat == "06")
                {
                    position = db.tRSPPositions.SingleOrDefault(e => e.positionCode == data.positionCode);
                    subPosition = db.tRSPPositionSubs.SingleOrDefault(e => e.subPositionCode == data.subPositionCode);

                    vRSPSalarySchedJO sched = db.vRSPSalarySchedJOes.Single(e => e.salaryGrade == position.salaryGrade);
                    personnelService = GetPositionJobOrderPS(sched, Convert.ToDateTime(app.periodFrom), Convert.ToDateTime(app.periodTo));

                    rateSalary = Convert.ToDecimal(sched.rateDay);
                    rateDaily = Convert.ToDecimal(sched.rateDay);
                    rateMonthly = Convert.ToDecimal(sched.rateMonth);
                    salaryGradeText = "SG " + position.salaryGrade + "/1";
                    salaryDetailCode = sched.salaryDetailCode;
                    salaryType = "D";
                }
                else if (emptStat == "07" || emptStat == "08")
                {
                    tRSPPositionContract posContract = db.tRSPPositionContracts.SingleOrDefault(e => e.positionCode == data.positionCode);
                    position.positionCode = posContract.positionCode;
                    position.positionTitle = posContract.positionTitle;
                    position.salaryGrade = 0;
                    subPosition = null;
                    personnelService = GetPositionContractHon(posContract, Convert.ToDateTime(app.periodFrom), Convert.ToDateTime(app.periodTo));

                    rateDaily = Convert.ToDecimal(posContract.salary / 22);
                    rateMonthly = Convert.ToDecimal(posContract.salary);

                    tRSPWarmBody warmBody = db.tRSPWarmBodies.SingleOrDefault(e => e.warmBodyGroupCode == data.warmBodyGroupCode);
                    if (warmBody != null)
                    {
                        warmBodyGroupName = warmBody.warmBodyGroupName;
                    }

                    salary.salaryDetailCode = "";
                    salary.salaryGrade = 0;
                    salary.rateMonth = posContract.salary;
                    rateSalary = Convert.ToDecimal(posContract.salary);
                    salaryType = "M";
                }

                if (employee == null || position.positionCode == null)
                {
                    return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
                }

                string tmpPositionTitle = position.positionTitle;
                string tmpSubPositionCode = "";
                string tmpSubPositionTitle = "";

                if (subPosition != null)
                {
                    tmpPositionTitle = tmpPositionTitle + " (" + subPosition.subPositionTitle + ")";
                    tmpSubPositionCode = subPosition.subPositionCode;
                    tmpSubPositionTitle = subPosition.subPositionTitle;
                }

                int empCount = db.tRSPAppointmentCasualEmps.Where(e => e.appointmentCode == apptCode && e.EIC == employee.EIC).Count();

                if (empCount >= 1)
                {
                    return Json(new { status = "Employee already exists." }, JsonRequestBehavior.AllowGet);
                }

                string tempPlaceOfAssignment = data.placeOfAssignment;
                if (data.warmBodyGroupCode != null)
                {
                    //tRSPWarmBody wb = db.tRSPWarmBodies.SingleOrDefault(e => e.warmBodyGroupCode == data.warmBodyGroupCode);
                    tRSPWorkGroup wg = db.tRSPWorkGroups.SingleOrDefault(e => e.workGroupCode == data.warmBodyGroupCode);
                    if (wg != null)
                    {
                        tempPlaceOfAssignment = wg.workGroupDesc;
                    }
                }

                //MAKE VALIDATION HERE ----------------
                if (data.EIC != null && data.positionCode != null)
                {

                    DateTime dt = DateTime.Now;
                    string eEIC = employee.EIC;
                    string tmpCode = "T" + dt.ToString("yyMMddHHmm") + "APPI" + eEIC.Substring(0, 5) + dt.ToString("ssfff");

                    if (app.periodTo.Value.Year == 2022)
                    {
                        //NEW PS TABLE
                        tRSPRefFundPersonnelService myPS = new tRSPRefFundPersonnelService();
                        myPS = nps;
                        myPS.PSId = "T" + dt.ToString("yyMMddHHmm") + "PS" + dt.ToString("ssfff") + GetMyRandom(7);
                        //tRSPRefFundPersonnelService 
                    }
                    else
                    {
                        tRSPZPersonnelService ps = new tRSPZPersonnelService();
                        ps = personnelService;
                        ps.appointmentCode = apptCode;
                        ps.appointmentItemCode = tmpCode;
                        ps.positionTitle = tmpPositionTitle;
                        ps.userID = Session["_EIC"].ToString();
                        ps.transDT = DateTime.Now;
                        ps.tag = 0;
                        db.tRSPZPersonnelServices.Add(ps);
                    }

                    tRSPAppointmentCasualEmp a = new tRSPAppointmentCasualEmp();
                    a.appointmentItemCode = tmpCode;
                    a.appointmentCode = apptCode;
                    a.EIC = employee.EIC;
                    a.positionCode = position.positionCode;
                    a.positionTitle = tmpPositionTitle;
                    a.subPositionCode = tmpSubPositionCode;
                    a.subPositionTitle = tmpSubPositionTitle;
                    a.salaryGrade = position.salaryGrade;
                    a.salaryDetailCode = salaryDetailCode;
                    a.salary = rateSalary;
                    a.salaryType = salaryType;
                    a.placeOfAssignment = tempPlaceOfAssignment;
                    a.warmBodyGroupCode = data.warmBodyGroupCode;

                    if (app.periodFrom != null)
                    {
                        if (app.periodFrom.Value.Year > 2020)
                        {
                            a.periodFrom = Convert.ToDateTime(app.periodFrom);
                        }
                    }

                    a.periodTo = Convert.ToDateTime(app.periodTo);
                    a.PS = appointmentPS; //
                    a.tag = 0;
                    db.tRSPAppointmentCasualEmps.Add(a);
                    db.SaveChanges();
                }

                IEnumerable<vRSPAppointmentNPEmployee> empList = db.vRSPAppointmentNPEmployees.Where(e => e.appointmentCode == apptCode).OrderBy(o => o.fullNameLast).ToList();
                //IEnumerable<tRSPZPersonnelService> posted = db.tRSPZPersonnelServices.ToList();
                List<vRSPAppointmentNPEmployee> list = new List<vRSPAppointmentNPEmployee>();
                foreach (vRSPAppointmentNPEmployee item in empList)
                {
                    //int iCount = posted.Where(e => e.appointmentItemCode == item.appointmentItemCode).Count();
                    list.Add(new vRSPAppointmentNPEmployee()
                    {
                        appointmentItemCode = item.appointmentItemCode,
                        EIC = item.EIC,
                        fullNameLast = item.fullNameLast,
                        fullNameFirst = item.fullNameFirst,
                        positionTitle = item.positionTitle,
                        salaryGrade = item.salaryGrade,
                        salary = item.salary,
                        salaryType = item.salaryType,
                        periodFrom = item.periodFrom,
                        periodTo = item.periodTo,
                        departmentCode = item.departmentCode,
                        empTag = item.empTag,
                        tag = item.tag
                    });
                }

                list = list.OrderBy(o => o.fullNameLast).ToList();

                return Json(new { status = "success", appointeeList = list }, JsonRequestBehavior.AllowGet);


                //return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }


        public class TempPosition
        {
            public string positionCode { get; set; }
            public string positionTitle { get; set; }
        }

        //NG
        [HttpPost]
        public JsonResult GetAppointeeAndPositionByEmpStat(string code)
        {
            IEnumerable<tRSPEmployee> empList = db.tRSPEmployees.Where(e => e.tag != 0).OrderBy(o => o.fullNameLast).ToList();

            List<tRSPEmployee> newList = new List<tRSPEmployee>();
            IEnumerable<vRSPPlantilla> plantillaEmp = db.vRSPPlantillas.Where(e => e.isFunded == true && e.EIC != null).ToList();
            foreach (tRSPEmployee item in empList)
            {
                int iCount = plantillaEmp.Where(e => e.EIC == item.EIC).Count();
                if (iCount == 0)
                {
                    newList.Add(new tRSPEmployee()
                    {
                        EIC = item.EIC,
                        fullNameLast = item.fullNameLast
                    });
                }
            }

            List<tRSPPosition> posList = new List<tRSPPosition>();
            tRSPAppointmentCasual appointment = db.tRSPAppointmentCasuals.Single(e => e.appointmentCode == code);
            int iTag = 0;

            List<TempPosition> tempPos = new List<TempPosition>();

            if (appointment.employmentStatusCode == "05" || appointment.employmentStatusCode == "06")
            {
                IEnumerable<tRSPPosition> positionList = db.tRSPPositions.Where(e => e.isActive == 1).OrderBy(o => o.positionTitle).ToList();
                foreach (tRSPPosition pos in positionList)
                {
                    tempPos.Add(new TempPosition()
                    {
                        positionCode = pos.positionCode,
                        positionTitle = pos.positionTitle
                    });
                }
                iTag = 1;
            }
            else
            {
                IEnumerable<tRSPPositionContract> conList = db.tRSPPositionContracts.Where(e => e.isActive == 1).OrderBy(o => o.positionTitle).ToList();
                foreach (tRSPPositionContract pos in conList)
                {
                    tempPos.Add(new TempPosition()
                    {
                        positionCode = pos.positionCode,
                        positionTitle = pos.positionDesc
                    });
                }
            }

            var mySubList = db.tRSPPositionSubs.Select(e => new
            {
                e.subPositionTitle,
                e.subPositionCode,
            }).OrderBy(e => e.subPositionTitle).ToList();

            //var wbList = db.tRSPWarmBodies.Select(e => new
            //{
            //    e.warmBodyGroupCode,
            //    e.warmBodyGroupName,
            //    e.orderNo
            //}).OrderBy(e => e.orderNo).ToList();

            var workGroup = db.tRSPWorkGroups.Select(e => new
            {
                e.workGroupCode,
                e.workGroupName,
                e.orderNo
            }).OrderBy(o => o.orderNo).ToList();

            //return Json(new { status = "success", empList = newList, positionList = tempPos, subPositionList = mySubList, warmBodyList = wbList, tag = iTag }, JsonRequestBehavior.AllowGet);

            var jsonResult = Json(new { status = "success", empList = newList, positionList = tempPos, subPositionList = mySubList, workGroupList = workGroup, tag = iTag }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        [HttpPost]
        public JsonResult CheckApptDelete(tRSPAppointmentCasual data)
        {
            try
            {
                string uEIC = Session["_EIC"].ToString();
                tRSPAppointmentCasual delItem = db.tRSPAppointmentCasuals.Single(e => e.appointmentCode == data.appointmentCode);
                if (delItem.userEIC.ToUpper() == uEIC.ToUpper() && delItem.tag == 0)
                {
                    return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = "Not Allowed!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult DeleteAppointment(tRSPAppointmentCasual data)
        {
            try
            {
                string uEIC = Session["_EIC"].ToString();
                tRSPAppointmentCasual delItem = db.tRSPAppointmentCasuals.Single(e => e.appointmentCode == data.appointmentCode && e.userEIC == uEIC);
                if (delItem.userEIC.ToUpper() == uEIC.ToUpper())
                {


                    int count = db.tRSPAppointmentCasualEmps.Where(e => e.appointmentCode == delItem.appointmentCode).Count();

                    if (count > 0)
                    {
                        return Json(new { status = "Unable to delete appointment!" }, JsonRequestBehavior.AllowGet);
                    }


                    if (delItem.tag == 0)
                    {
                        //DELETE PS
                        //DELETE EMPLOEE
                        //DELETE APPOINTMENT
                        var res = db.spAppointmentDeleteNP(delItem.appointmentCode);
                        return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { status = "Not Allowed!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }





        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public JsonResult ViewAppointmentPS()
        {
            string apptCode = Session["_APPTCODE"].ToString();
            try
            {
                IEnumerable<vRSPZPersonnelService> ps = db.vRSPZPersonnelServices.Where(e => e.appointmentCode == apptCode).OrderBy(o => o.fullNameLast).ToList();
                return Json(new { status = "success", list = ps }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        //MARK AS FINAL
        public JsonResult FinalizeAppointment(string id)
        {
            try
            {
                //1 check if there is appointee
                int count = db.tRSPAppointmentCasualEmps.Where(e => e.appointmentCode == id).Count();
                if (count <= 0)
                {
                    return Json(new { status = "No appointee!" }, JsonRequestBehavior.AllowGet);
                }
                tRSPAppointmentCasual app = db.tRSPAppointmentCasuals.SingleOrDefault(e => e.appointmentCode == id);
                if (app == null)
                {
                    return Json(new { status = "Hacking alert, please dont do that again!" }, JsonRequestBehavior.AllowGet);
                }

                //****************************************************************************************************************************************
                //2 check the fund source balance against then appointment requirements
                //string fundSrcCode = app.fundSourceCode;
                //vRSPRefFundSource fundSource = db.vRSPRefFundSources.Single(e => e.fundSourceCode == fundSrcCode);
                ////PS
                //IEnumerable<vRSPAppointmentNPP> empApptList = db.vRSPAppointmentNPPS.Where(e => e.fundSourceCode == fundSrcCode).ToList();

                //decimal postedPS = Convert.ToDecimal(empApptList.Where(e => e.fundSourceCode == fundSrcCode).Sum(e => e.total));
                //decimal balance = Convert.ToDecimal(fundSource.amount) - postedPS;
                ////APPOINTMENT PS 
                //if (balance <= 0)
                //{
                //    return Json(new { status = "Insufficient balance!" }, JsonRequestBehavior.AllowGet);
                //}
                //****************************************************************************************************************************************



                if (app.tag == 0)
                {
                    app.tag = 1;
                    db.SaveChanges();
                    return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        /////////////////////////////////////////////////////////////////////////////////////
        //CSC RA

        [HttpPost]
        public JsonResult CheckAppointmentRAI(string id)
        {
            try
            {
                IEnumerable<tRSPRPTRAI> raiList = db.tRSPRPTRAIs.Where(e => e.appointmentCode == id).ToList();
                if (raiList.Count() >= 1)
                {
                    return Json(new { status = "success", raiList = raiList }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    List<tRSPRPTRAI> temp = new List<tRSPRPTRAI>();
                    return Json(new { status = "success", raiList = temp }, JsonRequestBehavior.AllowGet);
                }
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
                Session["ReportType"] = "RAI";
                Session["PrintReport"] = id;
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult PrintRAIALL(string id)
        {
            try
            {
                Session["ReportType"] = "RAIALL";
                Session["PrintReport"] = id;
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult GenerateAppointmentRAI(string id, DateTime dateIssued)
        {
            try
            {
                tRSPAppointmentCasual app = db.tRSPAppointmentCasuals.SingleOrDefault(e => e.appointmentCode == id && e.employmentStatusCode == "05");
                if (app == null)
                {
                    return Json(new { status = "Unable to generate RAI!" }, JsonRequestBehavior.AllowGet);
                }
                tRSPRPTRAI appt = db.tRSPRPTRAIs.SingleOrDefault(e => e.appointmentCode == id);
                if (appt == null)
                {
                    int i = GenerateRAI(id, dateIssued);
                }
                IEnumerable<tRSPRPTRAI> raiList = db.tRSPRPTRAIs.Where(e => e.appointmentCode == id).ToList();
                return Json(new { status = "success", raiList = raiList }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        private int GenerateRAI(string appointmentCode, DateTime dateIssued)
        {
            try
            {
                string uEIC = Session["_EIC"].ToString();

                DateTime dt = DateTime.Now;

                IEnumerable<vRSPAppointmentNPEmployee> list = db.vRSPAppointmentNPEmployees.Where(e => e.appointmentCode == appointmentCode).OrderBy(o => o.fullNameLast).ToList();

                int groupCounter = 1;
                int runGCounter = 1;
                int counter = 1;

                foreach (vRSPAppointmentNPEmployee item in list)
                {
                    int tmpPositionLevel = Convert.ToInt16(item.positionLevel);
                    if (item.salaryGrade >= 25)
                    { tmpPositionLevel = 3; }
                    tRSPRPTRAIData r = new tRSPRPTRAIData();
                    string tmpRAICode = item.appointmentCode + "00" + groupCounter.ToString();
                    if (runGCounter == groupCounter)
                    {
                        //CREATE MAIN RAI                       
                        tRSPRPTRAI m = new tRSPRPTRAI();
                        m.RAICode = tmpRAICode;
                        m.itemCount = list.Count();
                        m.appointmentCode = item.appointmentCode;
                        m.RAIName = "Part " + groupCounter.ToString();
                        m.RAIDate = dateIssued;
                        m.userEIC = uEIC;
                        m.transDT = dt;
                        db.tRSPRPTRAIs.Add(m);
                        runGCounter = runGCounter + 1;
                    }
                    //RAI ITEM DATA
                    r.RAICode = tmpRAICode;
                    //r.batchCode = item.appointmentCode;
                    r.dateIssued = dateIssued;
                    r.EIC = item.EIC;
                    r.appointmentItemCode = item.appointmentItemCode;
                    r.positionTitle = item.positionTitle;
                    r.period = Convert.ToDateTime(dateIssued).ToString("MM/dd/yyyy") + " - " + Convert.ToDateTime(item.periodTo).ToString("MM/dd/yyyy");
                    r.natureOfAppointment = item.appNatureName;
                    r.itemNo = "N/A";
                    r.salaryGrade = "SG " + item.salaryGrade + "/1";
                    r.salaryRate = Convert.ToDecimal(item.salary * 22);
                    r.employmentStatus = item.employmentStatus;
                    r.positionLevel = tmpPositionLevel; // 1:FIRST, 2:SECOND, 3:MANAGERIAL

                    if (counter >= 10)
                    {
                        groupCounter = groupCounter + 1;
                        counter = 0;
                    }
                    counter = counter + 1;
                    db.tRSPRPTRAIDatas.Add(r);
                }

                db.SaveChanges();

                return 1;
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                return 0;
            }
        }

        //PS CALCULATOR
        public class TempModel
        {
            public string fundSourceCode { get; set; }
            public DateTime periodFrom { get; set; }
            public DateTime periodTo { get; set; }
            public string employmentStatusCode { get; set; }
        }

        public class TempCASUALHazard
        {
            public string id { get; set; }
            public double hazardPay { get; set; }
            public double laundry { get; set; }
            public double subsistence { get; set; }
        }

        public class TempLoyalty
        {
            public string id { get; set; }
            public string name { get; set; }
            public decimal amount { get; set; }
        }

        private IEnumerable<TempLoyalty> LoyaltyList()
        {
            List<TempLoyalty> loyalty = new List<TempLoyalty>();
            loyalty.Add(new TempLoyalty() { id = "1563", name = "LOZANO, FRANCISCO JR. G.", amount = 5000 });
            loyalty.Add(new TempLoyalty() { id = "7871", name = "CHATTO, ALLAN M.", amount = 10000 });
            loyalty.Add(new TempLoyalty() { id = "5137", name = "MORADA, DANNY", amount = 5000 });
            loyalty.Add(new TempLoyalty() { id = "5373", name = "DAGANATO, FEDERICO JR. A.", amount = 10000 });
            loyalty.Add(new TempLoyalty() { id = "5692", name = "LUMANGTAD, ARTURO NIÑO JR C.", amount = 10000 });
            loyalty.Add(new TempLoyalty() { id = "1809", name = "VARGAS, JUNNY D.", amount = 10000 });
            loyalty.Add(new TempLoyalty() { id = "4820", name = "NUNALA, SANDRA MAE B. ", amount = 10000 });
            loyalty.Add(new TempLoyalty() { id = "5773", name = "ACASO, EDGARLITO L.", amount = 10000 });
            loyalty.Add(new TempLoyalty() { id = "3831", name = "ANGELIA, RONILO R.", amount = 10000 });
            loyalty.Add(new TempLoyalty() { id = "1702", name = "BAÑADOS, LEONARDO M.", amount = 10000 });
            loyalty.Add(new TempLoyalty() { id = "5205", name = "CAGAS, JAIME M. ", amount = 10000 });
            loyalty.Add(new TempLoyalty() { id = "5195", name = "HAGONOS, DIONISIO S.", amount = 5000 });
            loyalty.Add(new TempLoyalty() { id = "5602", name = "PALBAN, EDGAR N.", amount = 10000 });
            loyalty.Add(new TempLoyalty() { id = "5308", name = "PALMA, ANTONIO P. ", amount = 5000 });
            loyalty.Add(new TempLoyalty() { id = "5395", name = "TAMPOS, MANOLITO", amount = 5000 });
            loyalty.Add(new TempLoyalty() { id = "2274", name = "VALIENTE, CORNELIO F.", amount = 10000 });
            loyalty.Add(new TempLoyalty() { id = "5710", name = "CABALLERO, JOSEPHINE N.", amount = 10000 });
            loyalty.Add(new TempLoyalty() { id = "1835", name = "NONAN, WELLIE M.", amount = 10000 });
            loyalty.Add(new TempLoyalty() { id = "5370", name = "AGUIRRE, DANILO SR T.", amount = 5000 });
            return loyalty;
        }

        //PSGenerator
        [HttpPost]
        public JsonResult SubmitPSPlan(tRSPZPSComputationMain data)
        {

            List<TempCASUALHazard> hazardList = new List<TempCASUALHazard>();

            hazardList.Add(new TempCASUALHazard() { id = "4435", hazardPay = 34652.64, laundry = 18000, subsistence = 1800 });
            hazardList.Add(new TempCASUALHazard() { id = "5673", hazardPay = 37802.88, laundry = 0, subsistence = 0 });
            hazardList.Add(new TempCASUALHazard() { id = "2418", hazardPay = 39056.82, laundry = 18000, subsistence = 1800 });
            hazardList.Add(new TempCASUALHazard() { id = "2172", hazardPay = 39056.82, laundry = 18000, subsistence = 1800 });
            hazardList.Add(new TempCASUALHazard() { id = "9675", hazardPay = 46571.58, laundry = 18000, subsistence = 1800 });
            hazardList.Add(new TempCASUALHazard() { id = "0333", hazardPay = 73033.92, laundry = 18000, subsistence = 0 });
            hazardList.Add(new TempCASUALHazard() { id = "8410", hazardPay = 41420.94, laundry = 18000, subsistence = 1800 });
            hazardList.Add(new TempCASUALHazard() { id = "4621", hazardPay = 41420.94, laundry = 18000, subsistence = 1800 });
            hazardList.Add(new TempCASUALHazard() { id = "1010", hazardPay = 37802.88, laundry = 0, subsistence = 0 });
            hazardList.Add(new TempCASUALHazard() { id = "0872", hazardPay = 73033.92, laundry = 18000, subsistence = 0 });
            hazardList.Add(new TempCASUALHazard() { id = "4737", hazardPay = 96752.7, laundry = 18000, subsistence = 1800 });

            string uEIC = Session["_EIC"].ToString();

            try
            {

                //string uEIC = Session["_EIC"].ToString();
                //string code = "PS" + DateTime.Now.ToString("yyMMddHHmmssfff") + GetHashCode();
                string code = "T" + DateTime.Now.ToString("yyMMddHHmmssfff") + uEIC.Substring(0, 5);


                IEnumerable<vRSPAppointmentNPEmployee> empList = db.vRSPAppointmentNPEmployees.Where(e => e.fundSourceCode == data.fundSourceCode && e.PSTag == 1).OrderBy(o => o.fullNameLast).ToList();
                if (data.employmentStatusCode != "00")
                {
                    empList = empList.Where(e => e.employmentStatusCode == data.employmentStatusCode);
                }

                IEnumerable<TempLoyalty> loyalty = LoyaltyList();

                List<vRSPSalarySchedCasual> salaryTableCasual = new List<vRSPSalarySchedCasual>();
                List<vRSPSalarySchedJO> salaryTableJobOrder = new List<vRSPSalarySchedJO>();

                //CASUAL SALARY SCHEDULE
                salaryTableCasual = db.vRSPSalarySchedCasuals.OrderBy(o => o.salaryGrade).ThenBy(s => s.step).ToList();
                //JOB ORDER SALARY SCHEDULE
                salaryTableJobOrder = db.vRSPSalarySchedJOes.OrderBy(o => o.salaryGrade).ThenBy(s => s.step).ToList();


                TempAppointmentNPPosting postData = new TempAppointmentNPPosting();
                int hazardCode = 0;

                IEnumerable<tRSPAppointmentNotify> separationList = db.tRSPAppointmentNotifies.ToList();
                decimal loyalAmt = 0;
                List<tRSPZPSComputation> myList = new List<tRSPZPSComputation>();
                foreach (vRSPAppointmentNPEmployee item in empList)
                {

                    tRSPPosition position = new tRSPPosition();
                    position.positionTitle = item.positionTitle;
                    position.salaryGrade = item.salaryGrade;

                    DateTime periodFrom = Convert.ToDateTime(item.PSPeriodFrom); //APPOINTMENT START DATE
                    DateTime periodTo = Convert.ToDateTime("2021-12-31");
                    //DateTime periodTo = Convert.ToDateTime(data.periodTo);     //SELECTED PERIOD DATE


                    int itemTag = 1;
                    tRSPAppointmentNotify sepItem = separationList.LastOrDefault(e => e.appointmentItemCode == item.appointmentItemCode);

                    if (sepItem != null)
                    {
                        if (sepItem.termCode == "EXCLUSION")
                        {
                            itemTag = 0;
                        }
                        if (sepItem.termCode == "ADJUSTMENT")
                        {
                            itemTag = 1;
                        }
                        else
                        {
                            periodTo = Convert.ToDateTime(sepItem.adjstmntTo);
                        }
                    }


                    tRSPZPersonnelService ps = new tRSPZPersonnelService();
                    data.employmentStatusCode = item.employmentStatusCode;

                    if (data.employmentStatusCode == "05")
                    {
                        vRSPSalarySchedCasual sched = salaryTableCasual.Single(e => e.salaryGrade == position.salaryGrade);
                        ps = GetPositionCasualPS(sched, position, hazardCode, periodFrom, periodTo, postData);
                        TempCASUALHazard haz = hazardList.SingleOrDefault(e => e.id == item.idNo);

                        if (haz != null)
                        {
                            ps.hazardPay = Convert.ToDecimal(haz.hazardPay);
                            ps.laundry = Convert.ToDecimal(haz.laundry);
                            ps.subsistence = Convert.ToDecimal(haz.subsistence);
                            decimal tot = Convert.ToDecimal(ps.total) + Convert.ToDecimal(haz.hazardPay) + Convert.ToDecimal(haz.laundry) + Convert.ToDecimal(haz.subsistence);
                            ps.total = Convert.ToDecimal(tot);
                        }

                        loyalAmt = 0;
                        TempLoyalty loyal = loyalty.SingleOrDefault(e => e.id == item.idNo);
                        if (loyal != null)
                        {
                            loyalAmt = Convert.ToDecimal(loyal.amount);
                            ps.total = Convert.ToDecimal(ps.total + loyalAmt);
                        }

                    }
                    else if (data.employmentStatusCode == "06")
                    {
                        vRSPSalarySchedJO mySched = salaryTableJobOrder.Single(e => e.salaryGrade == item.salaryGrade);
                        ps = GetPositionJobOrderPS(mySched, periodFrom, periodTo);
                    }
                    else if (data.employmentStatusCode == "07" || data.employmentStatusCode == "08")
                    {
                        tRSPPositionContract posContract = db.tRSPPositionContracts.SingleOrDefault(e => e.positionCode == item.positionCode);
                        position.positionCode = item.positionCode;
                        ps = GetPositionContractHon(posContract, periodFrom, periodTo);
                    }




                    if (itemTag == 1)
                    {
                        int monthCount = Monthcounter(periodFrom, periodTo);
                        decimal monthly = Convert.ToDecimal(ps.monthlyRate);
                        decimal daily = monthly / 22;
                        decimal annual = monthly * monthCount;

                        tRSPZPSComputation s = new tRSPZPSComputation();
                        s.reportCode = code;
                        s.EIC = item.EIC;
                        s.positionTitle = item.positionTitle;
                        s.subPositionTitle = "";
                        s.employmentStatusCode = data.employmentStatusCode;
                        s.salaryGrade = item.salaryGrade;
                        s.periodFrom = periodFrom;
                        s.periodTo = periodTo;
                        s.dailyRate = daily;
                        s.monthlyRate = monthly;
                        s.annualRate = annual;
                        s.PERA = ps.PERA;
                        s.leaveEarned = ps.leaveEarned;
                        s.hazardPay = ps.hazardPay;
                        s.laundry = ps.laundry;
                        s.subsistence = ps.subsistence;
                        s.midYear = ps.midYearBonus;
                        s.yearEnd = ps.yearEndBonus;
                        s.cashGift = ps.cashGift;
                        s.loyalty = loyalAmt;
                        s.lifeRetiremnt = ps.lifeRetmnt;
                        s.ECC = ps.ECC;
                        s.HDMF = ps.hdmf;
                        s.PHIC = ps.phic;
                        s.clothing = ps.clothing;
                        s.totalPS = ps.total;
                        s.monthCount = monthCount;
                        s.fundSourceCode = item.fundSourceCode;
                        db.tRSPZPSComputations.Add(s);
                    }
                }


                tRSPZPSComputationMain p = new tRSPZPSComputationMain();
                p.reportCode = code;
                p.planName = data.planName;
                p.fundSourceCode = data.fundSourceCode;
                p.employmentStatusCode = data.employmentStatusCode;
                p.transDT = DateTime.Now;
                p.userEIC = uEIC;
                db.tRSPZPSComputationMains.Add(p);

                db.SaveChanges();



                return Json(new { status = "success", reportCode = code }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }


        public class TempNamePosting
        {
            public int recNo { get; set; }
            public string reportCode { get; set; }
            public string appointeeName { get; set; }
            public string positionCode { get; set; }
            public int hazardCode { get; set; }
            public DateTime periodFrom { get; set; }
            public DateTime periodTo { get; set; }

        }

        [HttpPost]
        public JsonResult AddTempNameItem(TempNamePosting data)
        {
            try
            {

                tRSPZPSComputationMain main = db.tRSPZPSComputationMains.Single(e => e.reportCode == data.reportCode);

                tRSPZPersonnelService ps = new tRSPZPersonnelService();

                DateTime periodFrom = Convert.ToDateTime(data.periodFrom);
                DateTime periodTo = Convert.ToDateTime("December 31, 2021");

                int hazardCode = Convert.ToInt16(data.hazardCode);

                List<vRSPSalarySchedCasual> salaryTableCasual = new List<vRSPSalarySchedCasual>();
                List<vRSPSalarySchedJO> salaryTableJobOrder = new List<vRSPSalarySchedJO>();

                TempAppointmentNPPosting postData = new TempAppointmentNPPosting();
                postData.hazardCode = hazardCode;
                postData.periodFrom = periodFrom.ToString("MMM. dd, yyyy");

                tRSPPosition positionTemp = new tRSPPosition();

                if (main.employmentStatusCode == "05")
                {

                    tRSPPosition position = db.tRSPPositions.Single(e => e.positionCode == data.positionCode);
                    positionTemp = position;
                    //salaryTableCasual = db.vRSPSalarySchedCasuals.OrderBy(o => o.salaryGrade).ThenBy(s => s.step).ToList();
                    vRSPSalarySchedCasual sched = db.vRSPSalarySchedCasuals.Single(e => e.salaryGrade == position.salaryGrade);
                    ps = GetPositionCasualPS(sched, position, hazardCode, periodFrom, periodTo, postData);
                }
                else if (main.employmentStatusCode == "06")
                {

                    tRSPPosition position = db.tRSPPositions.Single(e => e.positionCode == data.positionCode);
                    positionTemp = position;
                    vRSPSalarySchedJO mySched = db.vRSPSalarySchedJOes.Single(e => e.salaryGrade == position.salaryGrade);
                    ps = GetPositionJobOrderPS(mySched, periodFrom, periodTo);
                }
                else if (main.employmentStatusCode == "07" || main.employmentStatusCode == "08")
                {

                    tRSPPositionContract posContract = db.tRSPPositionContracts.SingleOrDefault(e => e.positionCode == data.positionCode);
                    positionTemp.positionTitle = posContract.positionTitle;
                    positionTemp.salaryGrade = 0;
                    ps = GetPositionContractHon(posContract, periodFrom, periodTo);

                }

                int monthCount = Monthcounter(periodFrom, periodTo);
                decimal monthly = Convert.ToDecimal(ps.monthlyRate);
                decimal daily = monthly / 22;
                decimal annual = monthly * monthCount;

                tRSPZPSComputation s = new tRSPZPSComputation();
                s.reportCode = main.reportCode;
                //s.EIC = item.EIC;
                s.appointeeName = data.appointeeName;
                s.positionTitle = positionTemp.positionTitle;
                s.subPositionTitle = "";
                s.employmentStatusCode = main.employmentStatusCode;
                s.salaryGrade = positionTemp.salaryGrade;
                s.periodFrom = periodFrom;
                s.periodTo = periodTo;
                s.dailyRate = daily;
                s.monthlyRate = monthly;
                s.annualRate = annual;
                s.PERA = ps.PERA;
                s.leaveEarned = ps.leaveEarned;
                s.hazardPay = ps.hazardPay;
                s.laundry = ps.laundry;
                s.subsistence = ps.subsistence;
                s.midYear = ps.midYearBonus;
                s.yearEnd = ps.yearEndBonus;
                s.cashGift = ps.cashGift;
                s.lifeRetiremnt = ps.lifeRetmnt;
                s.ECC = ps.ECC;
                s.HDMF = ps.hdmf;
                s.PHIC = ps.phic;
                s.clothing = ps.clothing;
                s.totalPS = ps.total;
                s.monthCount = monthCount;
                s.fundSourceCode = main.fundSourceCode;
                db.tRSPZPSComputations.Add(s);
                db.SaveChanges();

                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult EditPSComputation(TempNamePosting data)
        {
            try
            {

                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SearchAppointeeByName(string id)
        {
            try
            {
                var list = db.vRSPAppointmentNPEmployees.Select(e => new
                {
                    e.EIC,
                    e.fullNameLast,
                    e.projectName,
                    e.periodFrom,
                    e.periodTo,
                    e.appointmentName,
                    e.recNo
                }).Where(e => e.fullNameLast.Contains(id)).OrderBy(o => o.fullNameLast).ThenByDescending(o => o.recNo).ToList();
                return Json(new { status = "success", myList = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult ViewAppointeeByGroup(tRSPAppointmentCasual data, string workGroupCode)
        {
            try
            {
                var myList = db.vRSPEmployeeLists.Where(e => e.workGroupCode == workGroupCode && e.fundSourceCode == data.fundSourceCode && e.employmentStatusCode == data.employmentStatusCode).Select(e => new
                          {
                              e.EIC,
                              e.fullNameLast,
                              e.positionTitle
                          }).OrderBy(o => o.fullNameLast).ToList();

                return Json(new { status = "success", appointeeList = myList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }


        //SAVE RENEWAL 
        // public JsonResult AddAppointee(tRSPAppointmentCasualEmp data, TempApptData app) 
        [HttpPost]
        public JsonResult SubmitRenewalGroup(tRSPAppointmentCasual data, string workGroupCode)
        {

            try
            {

                tRSPRefFundSource fs = db.tRSPRefFundSources.SingleOrDefault(e => e.fundSourceCode == data.fundSourceCode);
                if (fs == null)
                {
                    return Json(new { status = "Please fill-up the required field!" }, JsonRequestBehavior.AllowGet);
                }

                TempAppointmentNPPosting postData = new TempAppointmentNPPosting();
                postData.hazardCode = 0;
                postData.hasYearEnd = 1;

                DateTime periodFrom = Convert.ToDateTime(data.periodFrom);
                DateTime periodTo = Convert.ToDateTime(data.periodTo);

                IEnumerable<vRSPEmployeeList> myList = db.vRSPEmployeeLists.Where(e => e.workGroupCode == workGroupCode && e.fundSourceCode == data.fundSourceCode && e.employmentStatusCode == data.employmentStatusCode).ToList();

                //SALARY SCHEDULE FOR CASUAL
                IEnumerable<vRSPSalarySchedCasual> salaryCasual = db.vRSPSalarySchedCasuals.Where(e => e.step == 1).ToList();
                IEnumerable<vRSPSalarySchedJO> salayJObOrder = db.vRSPSalarySchedJOes.Where(e => e.step == 1).ToList();

                string uEIC = Session["_EIC"].ToString();

                tRSPZPersonnelService ps = new tRSPZPersonnelService();
                DateTime dt = DateTime.Now;

                string tmpCode = "T" + dt.ToString("yyMMddHHmm") + "APPT" + dt.ToString("ssfff");
                string appItemCode = "T" + dt.ToString("yyMMddHHmm") + "APPTI" + dt.ToString("ssfff");
                string apptCode = tmpCode + uEIC.Substring(0, 5);
                string tempStat = "";

                //HAZARD CODE 
                int counter = 0;
                foreach (var item in myList)
                {
                    counter = counter + 1;

                    decimal rateDaily = 0;
                    decimal rateMonthly = 0;
                    decimal rateSalary = 0;
                    decimal appointmentPS = 0;
                    string salaryType = "";
                    string salaryGradeText = "";
                    string salaryDetailCode = "";

                    int hazardCode = 0;

                    tRSPPosition position = new tRSPPosition();
                    tRSPPositionSub positionSub = new tRSPPositionSub();

                    string tmpPositionTitle = "";

                    if (data.employmentStatusCode == "05")
                    {
                        hazardCode = Convert.ToInt16(data.tag);
                        position = db.tRSPPositions.SingleOrDefault(e => e.positionCode == item.positionCode);
                        positionSub = db.tRSPPositionSubs.SingleOrDefault(e => e.subPositionCode == item.subPositionCode);
                        tmpPositionTitle = position.positionTitle;

                        if (positionSub != null)
                        {
                            tmpPositionTitle = tmpPositionTitle + " (" + positionSub.subPositionTitle + ")";
                        }

                        vRSPSalarySchedCasual sched = salaryCasual.Single(e => e.salaryGrade == position.salaryGrade);
                        ps = GetPositionCasualPS(sched, position, hazardCode, Convert.ToDateTime(periodFrom), Convert.ToDateTime(periodTo), postData);
                        rateSalary = Convert.ToDecimal(sched.rateDay);
                        rateDaily = Convert.ToDecimal(sched.rateDay);
                        rateMonthly = Convert.ToDecimal(sched.rateMonth);
                        salaryGradeText = "SG " + position.salaryGrade + "/1";
                        salaryDetailCode = sched.salaryDetailCode;
                        salaryType = "D";
                        tempStat = "CASUAL";
                    }
                    else if (data.employmentStatusCode == "06")
                    {
                        position = db.tRSPPositions.Single(e => e.positionCode == item.positionCode);
                        positionSub = db.tRSPPositionSubs.SingleOrDefault(e => e.subPositionCode == item.subPositionCode);
                        if (positionSub != null)
                        {
                            tmpPositionTitle = tmpPositionTitle + " (" + positionSub.subPositionTitle + ")";
                        }

                        vRSPSalarySchedJO mySched = salayJObOrder.Single(e => e.salaryGrade == position.salaryGrade);
                        ps = GetPositionJobOrderPS(mySched, periodFrom, periodTo);
                        rateSalary = Convert.ToDecimal(mySched.rateDay);
                        rateDaily = Convert.ToDecimal(mySched.rateDay);
                        salaryGradeText = "SG " + position.salaryGrade;
                        salaryDetailCode = mySched.salaryDetailCode;
                        tmpPositionTitle = position.positionTitle;
                        salaryType = "D";
                        tempStat = "JO";
                    }
                    else if (data.employmentStatusCode == "07" || data.employmentStatusCode == "08")
                    {
                        tRSPPositionContract posContract = new tRSPPositionContract();
                        posContract = db.tRSPPositionContracts.SingleOrDefault(e => e.positionCode == item.positionCode);
                        position.positionCode = posContract.positionCode;
                        position.positionTitle = posContract.positionTitle;
                        ps = GetPositionContractHon(posContract, periodFrom, periodTo);
                        tmpPositionTitle = position.positionTitle;
                        rateSalary = Convert.ToDecimal(posContract.salary);
                        salaryGradeText = "";
                        salaryType = "M";
                        tempStat = "HON";
                        if (data.employmentStatusCode == "07")
                        {
                            tempStat = "COS";
                        }
                    }

                    if (item.EIC != null && item.positionCode != null)
                    {
                        string codeItem = appItemCode + counter.ToString("0000");
                        tRSPZPersonnelService myps = new tRSPZPersonnelService();
                        myps = ps;
                        myps.appointmentCode = apptCode;
                        myps.appointmentItemCode = codeItem;
                        myps.positionTitle = position.positionTitle;
                        myps.userID = Session["_EIC"].ToString();
                        myps.transDT = DateTime.Now;
                        myps.tag = 0;

                        db.tRSPZPersonnelServices.Add(myps);

                        tRSPAppointmentCasualEmp a = new tRSPAppointmentCasualEmp();
                        a.appointmentItemCode = codeItem;
                        a.appointmentCode = apptCode;
                        a.EIC = item.EIC;
                        a.positionCode = position.positionCode;
                        a.positionTitle = tmpPositionTitle;
                        a.warmBodyGroupCode = workGroupCode;

                        if (positionSub != null)
                        {
                            a.subPositionCode = positionSub.subPositionCode;
                            a.subPositionTitle = positionSub.subPositionTitle;
                        }

                        a.salaryGrade = position.salaryGrade;
                        a.salaryDetailCode = salaryDetailCode;
                        a.salary = rateSalary;
                        a.salaryType = salaryType;
                        a.periodFrom = Convert.ToDateTime(periodFrom);
                        a.periodTo = Convert.ToDateTime(periodTo);
                        a.PS = appointmentPS; //
                        a.tag = 0;
                        db.tRSPAppointmentCasualEmps.Add(a);
                    }
                }

                tRSPAppointmentCasual app = new tRSPAppointmentCasual();
                app.appointmentCode = apptCode;
                app.appointmentName = data.appointmentName;
                app.fundSourceCode = data.fundSourceCode;
                app.employmentStatusCode = data.employmentStatusCode;
                app.appNatureCode = data.appNatureCode;
                app.periodFrom = periodFrom;
                app.periodTo = periodTo;
                app.userEIC = uEIC;
                app.transDT = DateTime.Now;
                app.tag = 0;
                db.tRSPAppointmentCasuals.Add(app);
                db.SaveChanges();

                IEnumerable<vRSPAppointmentNPEmployee> empList = db.vRSPAppointmentNPEmployees.Where(e => e.appointmentCode == apptCode).OrderBy(o => o.fullNameLast).ToList();
                List<vRSPAppointmentNPEmployee> list = new List<vRSPAppointmentNPEmployee>();
                foreach (vRSPAppointmentNPEmployee item in empList)
                {
                    //int iCount = posted.Where(e => e.appointmentItemCode == item.appointmentItemCode).Count();
                    list.Add(new vRSPAppointmentNPEmployee()
                    {
                        appointmentItemCode = item.appointmentItemCode,
                        EIC = item.EIC,
                        fullNameLast = item.fullNameLast,
                        fullNameFirst = item.fullNameFirst,
                        positionTitle = item.positionTitle,
                        salaryGrade = item.salaryGrade,
                        salary = item.salary,
                        salaryType = item.salaryType,
                        periodFrom = item.periodFrom,
                        periodTo = item.periodTo,
                        departmentCode = item.departmentCode,
                        empTag = item.empTag,
                        tag = item.tag,
                        postNo = item.postNo
                    });
                }

                TempAppointmentList myApp = new TempAppointmentList();
                myApp.appointmentCode = app.appointmentCode;
                myApp.appointmentName = app.appointmentName;
                myApp.employmentStatus = tempStat;
                myApp.itemCount = empList.Count();
                myApp.projectName = fs.projectName;

                return Json(new { status = "success", appData = myApp, appointeeList = list }, JsonRequestBehavior.AllowGet);

                //return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }


        private string GetMyRandom(int len)
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var stringChars = new char[len];
            var random = new Random();
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            var finalString = new String(stringChars);
            return finalString;

        }




    }

}