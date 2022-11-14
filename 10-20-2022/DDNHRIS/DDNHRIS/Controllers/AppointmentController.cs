using DDNHRIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DDNHRIS.Controllers
{

    [SessionTimeout]
    [Authorize(Roles = "APRD")]

    public class AppointmentController : Controller
    {
        HRISDBEntities db = new HRISDBEntities();
        // GET: Appointment
        public ActionResult List()
        {
            return View();
        }

        public JsonResult AppointmentInitData()
        {
            try
            {
                IEnumerable<vRSPPSBApp> incomingApp = db.vRSPPSBApps.OrderBy(o => o.fullNameFirst).ToList(); //PENDING
                IEnumerable<tRSPAppointment> list = db.tRSPAppointments.Where(e => e.tag >= 0).ToList();
                IEnumerable<tRSPAppointment> pending = list.Where(e => e.tag == 0).ToList();
                IEnumerable<tRSPAppointment> posted = list.Where(e => e.tag == 2).OrderByDescending(o => o.effectivityDate).ToList();
                IEnumerable<tRSPEligibility> elig = db.tRSPEligibilities.Where(e => e.isActive == 1).OrderBy(o => o.orderNo).ToList();
                IEnumerable<tRSPAppointmentNature> appNatureList = db.tRSPAppointmentNatures.Where(e => e.recNo > 0).OrderBy(o => o.orderNo).ToList();

                incomingApp = incomingApp.Where(e => e.recNo == null).ToList();

                var tempo = db.vRSPPlantillas.Select(e => new
                {
                    e.appointmenCode,
                    e.oldItemNo,
                    e.itemNo,
                    e.shortDepartmentName,
                    e.positionTitle,
                    e.salaryGrade,
                    e.fullNameLast,
                    e.employmentStatusCode,
                    e.employmentStatus
                }).Where(e => e.employmentStatusCode == "04").OrderBy(e => e.itemNo).ToList();

                return Json(new { status = "success", appIncoming = incomingApp, appPosted = posted, appPending = pending, appNatureList = appNatureList, eligList = elig, temporary = tempo }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        public class TempAppointmentData
        {
            public string appointmentCode { get; set; }
            public string EIC { get; set; }
            public string fullName { get; set; }
            public string fullNameTitle { get; set; }
            public string positionTitle { get; set; }
            public string namePrefix { get; set; }
            public string nameSuffix { get; set; }
            public int salaryGrade { get; set; }
            public int step { get; set; }
            public string itemNo { get; set; }
            public string officeAssignment { get; set; }
            public string appNatureCode { get; set; }
            public string employmentStatusCode { get; set; }
            public string viceOf { get; set; }
            public string viceStatus { get; set; }
            public int pageNo { get; set; }
            public string eligibilityCode { get; set; }
            public DateTime CSCPostedDate { get; set; }
            public DateTime CSCClosingDate { get; set; }
            public DateTime PSBDate { get; set; }
            public string plantillaCode { get; set; }
        }


        public JsonResult GetAppointmentData(vRSPPSBApp data)
        {
            try
            {
                TempAppointmentData app = new TempAppointmentData();
                string viceOf = "";
                string viceRemarks = "";
                //GET LAST VICE DATA

                vRSPPSBApp psb = db.vRSPPSBApps.SingleOrDefault(e => e.appointmentCode == data.appointmentCode);
                IEnumerable<vRSPSeparation> sepList = db.vRSPSeparations.Where(e => e.plantillaCode == psb.plantillaCode).OrderByDescending(e => e.separationDate).ToList();
                if (sepList.Count() > 0)
                {
                    vRSPSeparation sep = sepList.First();
                    if (sep.fullNameFirst != null)
                    {
                        viceOf = sep.namePrefix + " " + sep.fullNameFirst.Trim();
                        if (sep.nameSuffix != null && sep.nameSuffix != "")
                        {
                            viceOf = viceOf + " " + sep.nameSuffix;
                        }
                        viceRemarks = sep.separationType;
                    }
                }

                //TEMP ONLY
                if (data.EIC == "MAF8ADDBB6E1BE4EB7A7")
                {
                    data.appTypeCode = 1;
                }


                if (data.appTypeCode == 0)
                {
                    tApplicant applicant = db.tApplicants.Single(e => e.applicantCode == data.EIC);
                    if (applicant.EIC == null)
                    {
                        tRSPEmployee newEmp = new tRSPEmployee();
                        newEmp.EIC = applicant.applicantCode;
                        newEmp.lastName = applicant.lastName;
                        newEmp.firstName = applicant.firstName;
                        newEmp.middleName = applicant.middleName;
                        newEmp.extName = applicant.extName;
                    }
                    else if (applicant.EIC != null)
                    {
                        data.EIC = applicant.EIC;
                    }
                }



                //PERMANENT
                if (data.appTypeCode == 0 || data.appTypeCode == 1)
                {
                    tRSPEmployee emp = db.tRSPEmployees.Single(e => e.EIC == data.EIC);
                    vRSPPSBApp a = db.vRSPPSBApps.Single(e => e.appointmentCode == data.appointmentCode);
                    app.appointmentCode = a.appointmentCode;
                    app.EIC = emp.EIC;
                    app.fullName = emp.fullNameFirst;
                    app.fullNameTitle = emp.fullNameTitle;
                    app.positionTitle = a.positionTitle;
                    app.itemNo = Convert.ToInt16(a.itemNo).ToString("0000");

                    app.namePrefix = emp.namePrefix;
                    app.nameSuffix = emp.nameSuffix;

                    app.viceOf = viceOf;
                    app.viceStatus = viceRemarks;
                    app.PSBDate = Convert.ToDateTime(a.psbDate);
                    app.employmentStatusCode = "03";
                    app.officeAssignment = a.departmentName;
                    vRSPPSBApplication application = db.vRSPPSBApplications.SingleOrDefault(e => e.appointmentCode == a.appointmentCode);
                    app.salaryGrade = Convert.ToInt16(application.salaryGrade);
                    if (application != null)
                    {
                        app.CSCPostedDate = Convert.ToDateTime(application.CSCPostedDate);
                        app.CSCClosingDate = Convert.ToDateTime(application.CSCClosingDate);
                        //app.PSBDate = Convert.ToDateTime(application.PSBDate);
                        app.plantillaCode = application.plantillaCode;
                        app.pageNo = Convert.ToInt16(application.pageNo);
                    }
                }
                else if (data.appTypeCode == 2) // COTERM
                {

                    tRSPEmployee emp = db.tRSPEmployees.Single(e => e.EIC == data.EIC);
                    vRSPPSBAppCoTerm a = db.vRSPPSBAppCoTerms.Single(e => e.appointmentCode == data.appointmentCode);
                    vRSPPlantilla plantilla = db.vRSPPlantillas.Single(e => e.plantillaCode == a.plantillaCode);
                    app.appointmentCode = a.appointmentCode;
                    app.EIC = a.EIC;
                    app.fullName = emp.fullNameFirst;
                    app.fullNameTitle = emp.fullNameFirst;
                    app.namePrefix = emp.namePrefix;
                    app.nameSuffix = emp.nameSuffix;
                    app.positionTitle = a.positionTitle;
                    app.itemNo = Convert.ToInt16(a.itemNo).ToString("0000");
                    app.salaryGrade = Convert.ToInt16(a.salaryGrade);
                    app.officeAssignment = plantilla.departmentName;
                    app.pageNo = Convert.ToInt16(plantilla.pageNo);

                    app.viceOf = viceOf;
                    app.viceStatus = viceRemarks;

                    app.employmentStatusCode = "02";

                }
                return Json(new { status = "success", appData = app }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        //SAVE APPOINTMENT
        [HttpPost]
        public JsonResult SaveAppointment(TempAppointmentData data)
        {
            try
            {
                //vRSPPSBApplication app = new vRSPPSBApplication();
                //vRSPPSBApplication a = db.vRSPPSBApplications.Single(e => e.appointmentCode == data.appointmentCode);                                                
                tRSPEmployee emp = db.tRSPEmployees.Single(e => e.EIC == data.EIC);

                string tmpFullName = emp.fullNameFirst.Trim();
                string tmpFullNameSig = "";
                string fullNameTitle = "";

                if (data.namePrefix != null && data.namePrefix.Length > 0)
                {
                    fullNameTitle = data.namePrefix.Trim() + " " + tmpFullName;
                }

                if (data.nameSuffix != null && data.nameSuffix.Length > 0)
                {
                    tmpFullName = tmpFullName + ", " + data.nameSuffix;
                    tmpFullNameSig = tmpFullName + ", " + data.nameSuffix;
                    fullNameTitle = fullNameTitle + ", " + data.nameSuffix;
                    fullNameTitle = fullNameTitle.Trim();
                }

                vRSPPSBApp psb = db.vRSPPSBApps.Single(e => e.appointmentCode == data.appointmentCode);

                int appTypeCode = psb.appTypeCode;
                if (appTypeCode == 1 || appTypeCode == 0)
                {
                    vRSPPSBApplication a = db.vRSPPSBApplications.Single(e => e.appointmentCode == data.appointmentCode);

                    tRSPAppointment appt = new tRSPAppointment();
                    appt.EIC = emp.EIC;
                    appt.appointmentCode = data.appointmentCode;
                    appt.fullName = tmpFullName.Trim();
                    appt.fullNameTitle = fullNameTitle;
                    appt.fullNameSign = tmpFullNameSig.Trim();
                    appt.positionTitle = a.positionTitle;
                    appt.prefixLastName = data.namePrefix + " " + emp.lastName.Trim();
                    appt.namePrefix = data.namePrefix;
                    appt.nameSuffix = data.nameSuffix;
                    appt.salaryGrade = a.salaryGrade;
                    appt.step = data.step;
                    appt.itemNo = a.itemNo;
                    appt.employmentStatusCode = data.employmentStatusCode;
                    appt.eligibilityCode = data.eligibilityCode;
                    appt.officeAssignment = data.officeAssignment;

                    appt.rateMonth = a.rateMonth;
                    appt.rateMonthText = NumWords(Convert.ToDouble(a.rateMonth));
                    appt.appNatureCode = data.appNatureCode;
                    appt.viceOf = data.viceOf;
                    appt.viceStatus = data.viceStatus;
                    appt.pageNo = data.pageNo;

                    appt.CSCPostedDate = a.CSCPostedDate;
                    appt.CSCClosingDate = a.CSCClosingDate;
                    appt.PSBDate = psb.psbDate;

                    appt.plantillaCode = a.plantillaCode;
                    appt.publicationItemCode = a.publicationItemCode;
                    appt.tag = 0;

                    db.tRSPAppointments.Add(appt);

                    tRSPAppointmentData apptData = new tRSPAppointmentData();
                    apptData.appointmentCode = data.appointmentCode;
                    db.tRSPAppointmentDatas.Add(apptData);
                    db.SaveChanges();
                }
                else if (appTypeCode == 2)
                {
                    vRSPPSBAppCoTerm cot = db.vRSPPSBAppCoTerms.Single(e => e.appointmentCode == data.appointmentCode);
                    tRSPPlantilla plantilla = db.tRSPPlantillas.Single(e => e.plantillaCode == cot.plantillaCode);

                    vRSPSalaryDetailCurrent rate = db.vRSPSalaryDetailCurrents.Single(e => e.salaryGrade == cot.salaryGrade);

                    tRSPAppointment appt = new tRSPAppointment();
                    appt.appointmentCode = data.appointmentCode;
                    appt.EIC = emp.EIC;
                    appt.fullName = tmpFullName;
                    appt.fullNameTitle = fullNameTitle;
                    appt.fullNameSign = tmpFullNameSig;
                    appt.positionTitle = cot.positionTitle;
                    appt.prefixLastName = data.namePrefix + " " + emp.lastName;
                    appt.namePrefix = data.namePrefix;
                    appt.nameSuffix = data.nameSuffix;
                    appt.salaryGrade = cot.salaryGrade;
                    appt.step = data.step;
                    appt.itemNo = cot.itemNo;
                    appt.employmentStatusCode = data.employmentStatusCode;
                    appt.eligibilityCode = data.eligibilityCode;
                    appt.officeAssignment = data.officeAssignment;

                    appt.rateMonth = rate.rateMonth;
                    appt.rateMonthText = NumWords(Convert.ToDouble(rate.rateMonth));
                    appt.appNatureCode = data.appNatureCode;
                    appt.viceOf = data.viceOf;
                    appt.viceStatus = data.viceStatus;
                    appt.pageNo = plantilla.pageNo;
                    appt.plantillaCode = cot.plantillaCode;
                    appt.tag = 0;
                    db.tRSPAppointments.Add(appt);

                    tRSPAppointmentData apptData = new tRSPAppointmentData();
                    apptData.appointmentCode = data.appointmentCode;
                    db.tRSPAppointmentDatas.Add(apptData);
                    db.SaveChanges();

                }
                
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }



        public JsonResult EditFormData(string id)
        {
            try
            {
                tRSPAppointmentData data = db.tRSPAppointmentDatas.Single(e => e.appointmentCode == id);
                tRSPAppointment app = db.tRSPAppointments.Single(e => e.appointmentCode == id);
                return Json(new { status = "success", formApp = app, formData = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult UpdateFormData(tRSPAppointment app, tRSPAppointmentData data)
        {
            try
            {
                tRSPAppointment myApp = db.tRSPAppointments.Single(e => e.appointmentCode == app.appointmentCode);
                tRSPEmployee employee = db.tRSPEmployees.Single(e => e.EIC == myApp.EIC);
                tRSPAppointmentData myData = db.tRSPAppointmentDatas.Single(e => e.appointmentCode == app.appointmentCode);

                string fullNameTitle = "";
                string fullName = employee.fullNameFirst.Trim();
                string fullNameSig = "";

                if (app.namePrefix != null && app.namePrefix.Length > 0)
                {
                    fullNameTitle = app.namePrefix.Trim() + " " + fullName;
                }

                if (app.nameSuffix != null && app.nameSuffix.Length > 0)
                {
                    fullName = fullName + ", " + app.nameSuffix;
                    fullNameSig = fullName + ", " + app.nameSuffix;
                    fullNameTitle = fullNameTitle + ", " + app.nameSuffix;
                    fullNameTitle = fullNameTitle.Trim();
                }

                myApp.fullName = fullName;
                myApp.fullNameSign = fullNameSig;
                myApp.fullNameTitle = fullNameTitle;

                myApp.namePrefix = app.namePrefix;
                myApp.nameSuffix = app.nameSuffix;
                myApp.appNatureCode = app.appNatureCode;
                myApp.employmentStatusCode = app.employmentStatusCode;
                myApp.eligibilityCode = app.eligibilityCode;
                myApp.viceOf = app.viceOf;
                myApp.viceStatus = app.viceStatus;

                myData.presentAppAct = data.presentAppAct;
                myData.previousAppAct = data.previousAppAct;
                myData.otherCompensation = data.otherCompensation;
                myData.workStation = data.workStation;

                myData.positionSupervisorImm = data.positionSupervisorImm;
                myData.positionSupervisorNxt = data.positionSupervisorNxt;
                myData.positionSupervise = data.positionSupervise;
                myData.positionSuperviseItemNo = data.positionSuperviseItemNo;
                myData.machineToolsUsed = data.machineToolsUsed;
                myData.departmentHead = data.departmentHead;

                myData.CCSMangerial = data.CCSMangerial;
                myData.CCSSupervisors = data.CCSSupervisors;
                myData.CCSNonSupervisors = data.CCSNonSupervisors;
                myData.CCSSTaff = data.CCSSTaff;
                myData.CCSGenPublic = data.CCSGenPublic;
                myData.CCSOtherAgencies = data.CCSOtherAgencies;
                myData.CCSOthers = data.CCSOthers;

                myData.workConOffice = data.workConOffice;
                myData.workConField = data.workConField;
                myData.descriptUnitSec = data.descriptUnitSec;

                db.SaveChanges();

                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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
            string[] tensArr = new string[] { "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };
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

            string res = words.Replace(",", "");

            return res;

        }




        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //APPOINTMENT POSTING
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public class TempPublicationItem
        {
            public string publicationItemCode { get; set; }
            public string publicationCode { get; set; }
            public string publicationDate { get; set; }
            public string CSCPostedDate { get; set; }
            public string CSCClosingDate { get; set; }
            public string positionTitle { get; set; }
            public int salaryGrade { get; set; }
            public double rateMonth { get; set; }

        }

        [HttpPost]
        public JsonResult AppointmentPostingData()
        {
            try
            {
                var empList = db.tRSPEmployees.Select(e => new
                {
                    e.EIC,
                    e.idNo,
                    e.fullNameLast,
                    e.tag
                }).Where(e => e.tag >= 0).OrderBy(o => o.fullNameLast).ToList();


                DateTime cutOffDate = DateTime.Now.AddDays(-190);


                IEnumerable<vRSPPublicationItem> pubItems = db.vRSPPublicationItems.Where(e => e.CSCClosingDate >= cutOffDate.Date).OrderBy(o => o.itemNo).ThenByDescending(o => o.publicationDate).ToList();


                List<TempPublicationItem> myList = new List<TempPublicationItem>();

                foreach (vRSPPublicationItem item in pubItems)
                {
                    myList.Add(new TempPublicationItem()
                    {
                        publicationItemCode = item.publicationItemCode,
                        publicationCode = item.publicationCode,
                        publicationDate = Convert.ToDateTime(item.publicationDate).ToString("MM/dd/yyyy"),
                        CSCPostedDate = "",
                        CSCClosingDate = "",
                        positionTitle = String.Format("{0:0000}", Convert.ToInt16(item.itemNo)) + " " + item.positionTitle,
                        salaryGrade = Convert.ToInt16(item.salaryGrade),
                        rateMonth = Convert.ToDouble(item.rateMonth)

                    });
                }

                List<TempPublicationItem> myListCoTerm = new List<TempPublicationItem>();
                IEnumerable<vRSPPlantilla> coterm = db.vRSPPlantillas.Where(e => e.EIC == null && e.positionStatusCode == "02" && e.isFunded == true).ToList();
                foreach (vRSPPlantilla item in coterm)
                {
                    myListCoTerm.Add(new TempPublicationItem()
                    {
                        publicationItemCode = item.plantillaCode,
                        publicationCode = item.plantillaCode,
                        positionTitle = String.Format("{0:0000}", Convert.ToInt16(item.itemNo)) + " " + item.positionTitle,
                        salaryGrade = Convert.ToInt16(item.salaryGrade),
                        rateMonth = Convert.ToDouble(item.rateMonth)
                    });
                }
                return Json(new { status = "success", employeeList = empList, publicationItemList = myList, cotermList = myListCoTerm }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        public class TempAppointeePosting
        {
            public string EIC { get; set; }
            public string publicationItemCode { get; set; }
            public DateTime psbDate { get; set; }
        }

        public JsonResult PostAppointeeToAppList(TempAppointeePosting data)
        {
            try
            {
                vRSPPublicationItem pubItem = db.vRSPPublicationItems.Single(e => e.publicationItemCode == data.publicationItemCode);
                if (pubItem == null)
                {
                    return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
                }
                tRSPEmployee emp = db.tRSPEmployees.SingleOrDefault(e => e.EIC == data.EIC);
                if (emp == null)
                {
                    return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
                }
                DateTime cutOff = DateTime.Now.AddDays(-15);
                if (cutOff <= data.psbDate)
                {
                    return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
                }

                string uEIC = Session["_EIC"].ToString();
                string tmpStr = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                string tmpApptCode = "APPT" + tmpStr + uEIC.Substring(0, 5);
                string tmpApplCode = "APPL" + tmpStr + uEIC.Substring(0, 5);
                //AUTO CREATE APPLICATION 
                tRSPApplication a = new tRSPApplication();
                a.applicationCode = tmpApplCode;
                a.EIC = data.EIC;
                a.publicationItemCode = data.publicationItemCode;
                a.appTypeCode = 1;
                a.transDT = DateTime.Now;
                a.tag = 1;
                a.remarks = "";
                db.tRSPApplications.Add(a);
                //CREATE FOR APPOINTMENT
                tRSPPSBApp p = new tRSPPSBApp();
                p.appointmentCode = tmpApptCode;
                p.applicationCode = tmpApplCode;
                p.plantillaCode = pubItem.plantillaCode;
                p.EIC = data.EIC;

                db.tRSPPSBApps.Add(p);
                db.SaveChanges();

                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }



        public class TempAssumptionData
        {
            public string appointmentCode { get; set; }
            public DateTime assumptionDate { get; set; }
            public string fullNameFirst { get; set; }
            public string namePrefix { get; set; }
            public string nameSuffix { get; set; }
            public string departmentHead { get; set; }
            public string departmentHeadPosition { get; set; }
            public string officeAssignment { get; set; }
        }

        //CheckAssumptionReport
        public JsonResult CheckAssumptionReport(string id)
        {
            try
            {
                tRSPAppointment app = db.tRSPAppointments.Single(e => e.appointmentCode == id);
                tRSPEmployee emp = db.tRSPEmployees.Single(e => e.EIC == app.EIC);
                tRSPAppointmentData data = db.tRSPAppointmentDatas.Single(e => e.appointmentCode == id);

                TempAssumptionData assumptData = new TempAssumptionData();
                assumptData.appointmentCode = app.appointmentCode;
                assumptData.assumptionDate = Convert.ToDateTime(app.effectivityDate);
                assumptData.fullNameFirst = emp.fullNameFirst;
                assumptData.namePrefix = emp.namePrefix;
                assumptData.nameSuffix = emp.nameSuffix;
                assumptData.departmentHead = data.departmentHead;
                assumptData.departmentHeadPosition = data.departmentHeadPosition;
                assumptData.officeAssignment = app.officeAssignment;
                return Json(new { status = "success", data = assumptData }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UpdateAssumption(TempAssumptionData data)
        {
            try
            {
                tRSPAppointment appointment = db.tRSPAppointments.Single(e => e.appointmentCode == data.appointmentCode);
                if (appointment.effectivityDate == null)
                {
                    appointment.effectivityDate = Convert.ToDateTime(data.assumptionDate);
                }
                appointment.officeAssignment = data.officeAssignment;
                tRSPAppointmentData appData = db.tRSPAppointmentDatas.Single(e => e.appointmentCode == appointment.appointmentCode);
                appData.departmentHead = data.departmentHead;
                appData.departmentHeadPosition = data.departmentHeadPosition;

                db.SaveChanges();

                //tRSPAppointment app = db.tRSPAppointments.Single(e => e.appointmentCode == id);
                tRSPEmployee emp = db.tRSPEmployees.Single(e => e.EIC == appointment.EIC);
                //tRSPAppointmentData appData = db.tRSPAppointmentDatas.Single(e => e.appointmentCode == id);

                TempAssumptionData assumptData = new TempAssumptionData();
                assumptData.appointmentCode = appointment.appointmentCode;
                assumptData.assumptionDate = Convert.ToDateTime(appointment.effectivityDate);
                assumptData.fullNameFirst = emp.fullNameFirst;
                assumptData.namePrefix = emp.namePrefix;
                assumptData.nameSuffix = emp.nameSuffix;
                assumptData.departmentHead = data.departmentHead;
                assumptData.departmentHeadPosition = data.departmentHeadPosition;
                assumptData.officeAssignment = appointment.officeAssignment;

                return Json(new { status = "success", data = assumptData }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        public class TempOathOfOffice
        {
            public string appointmentCode { get; set; }
            public string fullNameFirst { get; set; }
            public string positionTitle { get; set; }
            public string fullNameSign { get; set; }
            public string completeAddress { get; set; }
            public string govtIDName { get; set; }
            public string govtIDNo { get; set; }
            public string govtIDIssued { get; set; }
            public string officeAssignment { get; set; }
        }


        [HttpPost]
        public JsonResult CheckOathOfOffice(string id)
        {
            try
            {

                tRSPAppointment app = db.tRSPAppointments.Single(e => e.appointmentCode == id);
                tRSPAppointmentData appData = db.tRSPAppointmentDatas.Single(e => e.appointmentCode == app.appointmentCode);
                tRSPEmployee employee = db.tRSPEmployees.Single(e => e.EIC == app.EIC);

                TempOathOfOffice oath = new TempOathOfOffice();
                oath.appointmentCode = id;
                oath.fullNameFirst = employee.fullNameFirst;
                oath.completeAddress = appData.presentAddress;

                oath.govtIDName = appData.govtID;
                oath.govtIDNo = appData.govtIDNo;
                oath.govtIDIssued = appData.govtIDDateIssued;


                if (appData.govtID == null || appData.govtID == "")
                {
                    oath.govtIDName = "";
                    oath.govtIDNo = "";
                    oath.govtIDIssued = "";
                }

                return Json(new { status = "success", data = oath }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UpdateOathData(TempOathOfOffice data)
        {
            try
            {
                tRSPAppointment app = db.tRSPAppointments.Single(e => e.appointmentCode == data.appointmentCode);
                tRSPAppointmentData appData = db.tRSPAppointmentDatas.Single(e => e.appointmentCode == data.appointmentCode);
                appData.govtID = data.govtIDName;
                appData.govtIDNo = data.govtIDNo;
                appData.govtIDDateIssued = data.govtIDIssued;
                appData.presentAddress = data.completeAddress;
                db.SaveChanges();

                TempOathOfOffice oath = new TempOathOfOffice();
                oath.appointmentCode = data.appointmentCode;
                oath.fullNameFirst = app.fullName;
                oath.completeAddress = appData.presentAddress;

                oath.govtIDName = data.govtIDName;
                oath.govtIDNo = data.govtIDNo;
                oath.govtIDIssued = data.govtIDIssued;
                if (appData.govtID == null || appData.govtID == "")
                {
                    oath.govtIDName = "";
                    oath.govtIDNo = "";
                    oath.govtIDIssued = "";
                }
                return Json(new { status = "success", data = oath }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }




        [HttpPost]
        public JsonResult CheckCSCLimitation(string id)
        {
            try
            {
                vRSPApplication app = db.vRSPApplications.Single(e => e.applicationCode == id);
                return Json(new { status = "success", data = app }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult AppointmentPrintSetup(tRSPZPrintReport data, tRSPAppointmentData appData)
        {
            try
            {
                string uEIC = Session["_EIC"].ToString();
                int iStat = 0;
                string tmpPrintID = "PRN" + DateTime.Now.ToString("yyyyMMddHHmssfff") + uEIC.Substring(0, 5);

                tRSPAppointmentData appointmentData = db.tRSPAppointmentDatas.Single(e => e.appointmentCode == data.printCode);
                if (appointmentData == null)
                {
                    return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
                }

                if (data.printType == "FUNDS")
                {
                    data.printType = "AVAILABILITYOFUNDS";
                    data.printCode = data.printCode;
                }
                else if (data.printType == "CSCLIMIT")
                {
                    data.printType = "AVAILABILITYOFUNDS";
                    data.printCode = data.printCode;
                }
                else if (data.printType == "ASSUMPTION")
                {
                    data.printType = "ASSUMPTION";
                    data.printCode = data.printCode;
                }
                else if (data.printType == "OATH")
                {
                    data.printType = "OATH";
                    data.printCode = data.printCode;
                }
                else
                {
                    iStat = 1;
                }

                if (iStat == 0)
                {
                    tRSPZPrintReport r = new tRSPZPrintReport();
                    r.printID = tmpPrintID;
                    r.printCode = data.printCode;
                    r.printType = data.printType;
                    r.printDetail = data.printDetail;
                    r.userEIC = Session["_EIC"].ToString();
                    r.transDT = DateTime.Now;
                    db.tRSPZPrintReports.Add(r);
                    db.SaveChanges();
                    return Json(new { status = "success", printID = tmpPrintID, stat = iStat }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult PostPlantillaAppointment(string id, DateTime assumptDate)
        {
            try
            {
                string transCode = "";
                string userEIC = Session["_EIC"].ToString();
                tRSPAppointment appointment = db.tRSPAppointments.Single(e => e.appointmentCode == id);
                int appTag = 1; //CHANGE LAST DATE OF PROMOTION

                vRSPEmployee emp = db.vRSPEmployees.SingleOrDefault(e => e.EIC == appointment.EIC);
                transCode = "T" + DateTime.Now.ToString("yyMMddHHmmssfff") + "APPT" + emp.EIC.Substring(0, 5);

                if (emp != null)
                {
                    if (emp.salaryGrade == appointment.salaryGrade)
                    {
                        appTag = 0; // SAME SG MEANS NO PROMOTION
                    }
                }
                if (appointment.tag == 0 || appointment.tag == 1)
                {
                    db.spAppointmentPosting(transCode, appointment.appointmentCode, assumptDate, appTag, userEIC);
                    return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // APPOINTEE POSTING

        public ActionResult AppointeePosting()
        {
            return View();
        }

        [HttpPost]
        public JsonResult PostAppointeePerm(TempAppointeePosting data)
        {
            try
            {
                //tRSPPSBApp p = new tRSPPSBApp();
                //p.applicationCode = "";
                //p.appointmentCode = "";
                //p.plantillaCode = "";
                //p.EIC = "";
                //p.transDT = DateTime.Now;

                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult PostAppointeeCoTerm(TempAppointeePosting data)
        {
            try
            {
                string uEIC = Session["_EIC"].ToString();
                string tmpStr = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                string tmpApptCode = "APPT" + tmpStr + uEIC.Substring(0, 5);

                //NORMALIZE
                string plantillaCode = data.publicationItemCode;

                //CREATE FOR APPOINTMENT
                tRSPPSBApp p = new tRSPPSBApp();
                p.appointmentCode = tmpApptCode;
                p.plantillaCode = plantillaCode;              
                p.EIC = data.EIC;
                p.transDT = DateTime.Now;
                db.tRSPPSBApps.Add(p);
                db.SaveChanges();

                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // ********************************************************************************************************************************************
        public JsonResult PrintPreviewSetup(string rptType, string id)
        {
            try
            {

                vRSPAppointment appt = db.vRSPAppointments.SingleOrDefault(e => e.appointmentCode == id);

                if (appt != null)
                {
                    string tmpType = "";

                    if (rptType == "APPT")
                    {
                        tmpType = "Appointment";
                    }
                    else if (rptType == "PDF")
                    {
                        tmpType = "PDF";
                    }
                    else if (rptType == "PDFBACK")
                    {
                        id = id + ":" + appt.plantillaCode;
                        //CHECK IF THERES SUB POSITION                  
                        int count = db.vRSPPositionJobDescSubs.Where(e => e.plantillaCode == appt.plantillaCode).Count();
                        tmpType = "PDFBACK";
                        if (count >= 1)
                        {
                            tmpType = "PDFBACKB";
                        }
                    }
                    else if (rptType == "LIMITATION")
                    {
                        tmpType = "CSCLIMITATION";
                    }
                    else if (rptType == "ASSUMPTION")
                    {
                        tmpType = "ASSUMPTDUTY";
                    }
                    else if (rptType == "OATH")
                    {
                        tmpType = "OATHOFFICE";
                    }
                    else if (rptType == "FUNDS")
                    {
                        tmpType = "FUNDSAVAILABILITY";
                    }
                    else if (rptType == "RAI") //RAI PLANTILLA
                    {
                        if (appt.effectivityDate == null)
                        {
                            return Json(new { status = "Unable to create RAI for unposted appointment!" }, JsonRequestBehavior.AllowGet);
                        }
                        tmpType = "RAIPLANTILLA";
                    }
                    Session["ReportType"] = tmpType;
                    Session["PrintReport"] = id;
                    return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { status = "Crital Error!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetRAIList()
        {
            try
            {
                string uEIC = Session["_EIC"].ToString();
                IEnumerable<tRSPRPTRAI> raiList = db.tRSPRPTRAIs.Where(e => e.tag == 1 && e.userEIC == uEIC).OrderByDescending(o => o.transDT).ToList();

                DateTime minDate = Convert.ToDateTime("May 1, 2021");
                var postedApptList = (from a in db.vRSPAppointments
                                      where a.effectivityDate >= minDate && a.RAITag == null
                                      select new
                                      {
                                          a.appointmentCode,
                                          a.fullName,
                                          a.itemNo,
                                          a.positionTitle,
                                          a.effectivityDate,
                                      }).ToList();

                return Json(new { status = "success", rai = raiList, postedApptList = postedApptList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string err = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult GetRAIAppointment(string id)
        {
            try
            {
                string uEIC = Session["_EIC"].ToString();

                var myList = (from e in db.tRSPRPTRAIDatas
                              join a in db.vRSPAppointments on e.appointmentCode equals a.appointmentCode
                              where e.RAICode == id
                              select new
                              {
                                  e.RAICode,
                                  e.appointmentCode,
                                  e.dateIssued,
                                  e.EIC,
                                  e.lastName,
                                  e.firstName,
                                  e.extnName,
                                  e.middleName,
                                  e.positionTitle,
                                  a.salaryGrade,
                                  a.step,
                                  e.employmentStatus,
                                  e.natureOfAppointment,
                                  a.CSCPostedDate,
                                  a.CSCClosingDate,

                              }).ToList();


                return Json(new { status = "success", raiAppList = myList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string err = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UpdateRAICheckList(string id, int tag)
        {
            try
            {
                tRSPRPTRAI r = db.tRSPRPTRAIs.Single(e => e.RAICode == id);

                if (tag == 1)
                {
                    if (r.chkList_ApptForms == 1)
                    {
                        r.chkList_ApptForms = 0;
                    }
                    else
                    {
                        r.chkList_ApptForms = 1;
                    }
                }
                else if (tag == 2)
                {
                    if (r.chkList_Casual == 1)
                    {
                        r.chkList_Casual = 0;
                    }
                    else
                    {
                        r.chkList_Casual = 1;
                    }
                }
                else if (tag == 3)
                {
                    if (r.chkList_PDS == 1)
                    {
                        r.chkList_PDS = 0;
                    }
                    else
                    {
                        r.chkList_PDS = 1;
                    }
                }
                else if (tag == 4)
                {
                    if (r.chkList_Eligibility == 1)
                    {
                        r.chkList_Eligibility = 0;
                    }
                    else
                    {
                        r.chkList_Eligibility = 1;
                    }
                }
                else if (tag == 5)
                {
                    if (r.chkList_PDF == 1)
                    {
                        r.chkList_PDF = 0;
                    }
                    else
                    {
                        r.chkList_PDF = 1;
                    }
                }
                else if (tag == 6)
                {
                    if (r.chkList_Oath == 1)
                    {
                        r.chkList_Oath = 0;
                    }
                    else
                    {
                        r.chkList_Oath = 1;
                    }
                }
                else if (tag == 7)
                {
                    if (r.chkList_Assumption == 1)
                    {
                        r.chkList_Assumption = 0;
                    }
                    else
                    {
                        r.chkList_Assumption = 1;
                    }
                }
                db.SaveChanges();
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string err = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult CreateRAI(tRSPRPTRAI data)
        {
            try
            {
                if (data.RAIName == null || data.RAIDate == null || data.RAIDate > DateTime.Now)
                {
                    return Json(new { status = "Please fill-up the form correctly!" }, JsonRequestBehavior.AllowGet);
                }
                string uEIC = Session["_EIC"].ToString();
                DateTime dt = DateTime.Now;
                string tmp = "T" + dt.ToString("yyMMddHHmm") + "RAI" + dt.ToString("ss") + uEIC.Substring(0, 6) + dt.ToString("fff");
                tRSPRPTRAI r = new tRSPRPTRAI();
                r.RAICode = tmp;
                r.RAIName = data.RAIName.Trim();
                r.RAIDate = data.RAIDate;
                r.transDT = dt;
                r.tag = 1;
                r.userEIC = uEIC;
                db.tRSPRPTRAIs.Add(r);
                db.SaveChanges();
                return Json(new { status = "success", raiData = r }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string err = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult AddAPPTToRAI(string id, string code)
        {
            try
            {
                vRSPAppointment app = db.vRSPAppointments.SingleOrDefault(e => e.appointmentCode == id);
                if (app.RAITag != null)
                {
                    return Json(new { status = "Duplication now allowed!" }, JsonRequestBehavior.AllowGet);
                }

                if (code == null || code == "")
                {
                    return Json(new { status = "Invalid RAI!" }, JsonRequestBehavior.AllowGet);
                }

                if (app.effectivityDate != null)
                {
                    tRSPEmployee emp = db.tRSPEmployees.Single(e => e.EIC == app.EIC);
                    int pos = Convert.ToInt16(db.tRSPPositions.Single(e => e.positionCode == app.positionCode).positionLevel);

                    int tmpPositionLevel = Convert.ToInt16(pos);
                    if (app.salaryGrade >= 25)
                    { tmpPositionLevel = 3; }

                    tRSPRPTRAIData d = new tRSPRPTRAIData();
                    d.RAICode = code;
                    d.EIC = app.EIC;
                    d.lastName = emp.lastName;
                    d.firstName = emp.firstName;
                    d.extnName = emp.extName;
                    d.middleName = emp.middleName;
                    d.appointmentCode = app.appointmentCode;
                    d.dateIssued = app.effectivityDate;
                    d.positionTitle = app.positionTitle;
                    d.positionLevel = tmpPositionLevel;
                    d.itemNo = Convert.ToInt16(app.itemNo).ToString("0000");
                    d.salaryGrade = app.salaryGrade + "/1";
                    d.salaryRate = app.rateMonth;
                    d.employmentStatus = app.employmentStatus;
                    d.natureOfAppointment = app.appNatureName.Replace("*", "");
                    d.transDT = DateTime.Now;

                    if (app.employmentStatusCode == "02")
                    {   
                        d.period = "N/A";
                        d.publicationMode = "N/A";                        
                    }
                    else
                    {
                        string pubPeriod = Convert.ToDateTime(app.CSCPostedDate).ToString("MM/dd/yyyy") + " - " + Convert.ToDateTime(app.CSCClosingDate).ToString("MM/dd/yyyy");
                        d.period = pubPeriod;
                        d.publicationMode = "CSC Bulletin of Vacant Positions, Agency Websites";
                    }

                    db.tRSPRPTRAIDatas.Add(d);
                    db.SaveChanges();

                    var myList = (from e in db.tRSPRPTRAIDatas
                                  join a in db.vRSPAppointments on e.appointmentCode equals a.appointmentCode
                                  where e.RAICode == code
                                  select new
                                  {
                                      e.RAICode,
                                      e.appointmentCode,
                                      e.dateIssued,
                                      e.EIC,
                                      e.lastName,
                                      e.firstName,
                                      e.extnName,
                                      e.middleName,
                                      e.positionTitle,
                                      a.salaryGrade,
                                      a.step,
                                      e.employmentStatus,
                                      e.natureOfAppointment,
                                      a.CSCPostedDate,
                                      a.CSCClosingDate
                                  }).ToList();

                    return Json(new { status = "success", raiAppList = myList }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { status = "Unpostd appointment!" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                string err = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        //Remove RAI
        [HttpPost]
        public JsonResult RemoveRAI(tRSPRPTRAI data)
        {
            try
            {
                string uEIC = Session["_EIC"].ToString();
                tRSPRPTRAI r = db.tRSPRPTRAIs.Single(e => e.RAICode == data.RAICode);
                int counter = db.tRSPRPTRAIDatas.Where(e => e.RAICode == r.RAICode).Count();
                if (counter == 0)
                {
                    db.tRSPRPTRAIs.Remove(r);
                    db.SaveChanges();

                    IEnumerable<tRSPRPTRAI> raiList = db.tRSPRPTRAIs.Where(e => e.tag == 1 && e.userEIC == uEIC).OrderByDescending(o => o.transDT).ToList();

                    return Json(new { status = "success", rai = raiList }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = "Unable to cancel RAI!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                string err = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }
        //Remove RAI ITEM
        [HttpPost]
        public JsonResult RemoveRAIItem(tRSPRPTRAIData data)
        {
            try
            {
                tRSPRPTRAIData r = db.tRSPRPTRAIDatas.Single(e => e.appointmentCode == data.appointmentCode);
                DateTime tempDate = Convert.ToDateTime(r.transDT);
                tempDate = tempDate.AddHours(1);
                if (tempDate > DateTime.Now)
                {
                    db.tRSPRPTRAIDatas.Remove(r);
                    db.SaveChanges();
                    return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = "Unable to remove appointment!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                string err = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult PrintRAIPlantilla(string id, int tag)
        {
            try
            {
                string repType = "RAIPLANTILLA";

                if (tag == 1)
                {
                    repType = "RAIPLANTILLAXLS";
                }

               
                Session["ReportType"] = repType;
                Session["PrintReport"] = id;

                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string err = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult PrintRAIPlantillaBack(string id)
        {
            try
            {
                Session["ReportType"] = "RAIPLANTILLABACK";
                Session["PrintReport"] = id;
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string err = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // ********************************************************************************************************************************************
        public JsonResult CheckDataForPrintPlantilla(string type, string id)
        {
            int iStat = 0;
            string tmp = "";

            string code = "PRN" + DateTime.Now.ToString("yyMMddHHmmssfff");
            if (type == "ASSUMPTION")
            {
                //tRSPAppointmentCasualEmp appItem = db.tRSPAppointmentCasualEmps.Single(e => e.appointmentItemCode == id);

                vRSPAppointment appData = db.vRSPAppointments.Single(e => e.appointmentCode == id);
                vRSPPlantilla plantilla = db.vRSPPlantillas.SingleOrDefault(e => e.plantillaCode == appData.plantillaCode);
                tRSPEmployee emp = db.tRSPEmployees.Single(e => e.EIC == appData.EIC);
                 
                tOrgDepartmentHead dept = db.tOrgDepartmentHeads.Single(e => e.departmentCode == plantilla.departmentCode);

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


        public class TemporaryAppointment
        {
            public string appointmentCode { get; set; }
            public string appointeeTitleName { get; set; }
            public string namePrefix { get; set; }
            public string nameSufix { get; set; }
            public string itemNo { get; set; }
            public string positionTitle { get; set; }
            public int salaryGrade { get; set; }
            public int step { get; set; }
            public string employmentStatusCode { get; set; }
            public string employmentStatus { get; set; }
            public string appNatureCode { get; set; }
            public string eligibilityCode { get; set; }
            public string viceOf { get; set; }
            public string viceStatus { get; set; }
            public string pageNo { get; set; }

        }


        [HttpPost]
        public JsonResult TemporaryAppointmentData(string id, int tag)
        {
            try
            {
                vRSPPlantilla p = db.vRSPPlantillas.SingleOrDefault(e => e.appointmenCode == id);

                if (p != null)
                {

                    tRSPEmployee emp = db.tRSPEmployees.Single(e => e.EIC == p.EIC);

                    TemporaryAppointment appt = new TemporaryAppointment();
                    appt.appointeeTitleName = emp.namePrefix + " " + p.fullNameFirst + " " + emp.nameSuffix;
                    appt.appointeeTitleName = appt.appointeeTitleName.Trim();
                    appt.itemNo = Convert.ToInt16(p.itemNo).ToString("0000");
                    appt.positionTitle = p.positionTitle;
                    appt.salaryGrade = Convert.ToInt16(p.salaryGrade);
                    appt.step = Convert.ToInt16(p.step);
                    appt.pageNo = Convert.ToInt16(p.pageNo).ToString("000");

                    appt.employmentStatusCode = "04";
                    appt.employmentStatus = "TEMPORARY";
                    if (tag == 2)
                    {
                        appt.employmentStatusCode = "03";
                        appt.employmentStatus = "PERMANENT";
                    }

                    tRSPAppointment app = db.tRSPAppointments.SingleOrDefault(e => e.appointmentCode == id);
                    if (app != null)
                    {
                        appt.appNatureCode = app.appNatureCode;
                        appt.viceOf = app.viceOf;
                        appt.viceStatus = app.viceStatus;
                    }
                    return Json(new { status = "success", appointmentData = appt }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveTemporaryApptRenewal(TemporaryAppointment data)
        {
            try
            {

                tRSPAppointment prev = db.tRSPAppointments.SingleOrDefault(e => e.appointmentCode == data.appointmentCode);

                if (prev != null)
                {
                    DateTime dt = DateTime.Now;
                    string tmpCode = "T" + dt.ToString("yyMMddHHmm") + "APPT" + dt.ToString("ssfff");
                                        

                    tRSPAppointment appt = new tRSPAppointment();
                    appt.EIC = prev.EIC;
                    appt.appointmentCode = tmpCode; //NEW APPTCODE
                    //appt.fullName = tmpFullName.Trim();
                    //appt.fullNameTitle = fullNameTitle;
                    //appt.fullNameSign = tmpFullNameSig.Trim();
                    //appt.positionTitle = a.positionTitle;
                    //appt.prefixLastName = data.namePrefix + " " + emp.lastName.Trim();
                    //appt.namePrefix = data.namePrefix;
                    //appt.nameSuffix = data.nameSuffix;
                    //appt.salaryGrade = a.salaryGrade;
                    //appt.step = data.step;
                    //appt.itemNo = a.itemNo;
                    //appt.employmentStatusCode = data.employmentStatusCode;
                    //appt.eligibilityCode = data.eligibilityCode;
                    //appt.officeAssignment = data.officeAssignment;

                    //appt.rateMonth = a.rateMonth;
                    //appt.rateMonthText = NumWords(Convert.ToDouble(a.rateMonth));
                    //appt.appNatureCode = data.appNatureCode;
                    //appt.viceOf = data.viceOf;
                    //appt.viceStatus = data.viceStatus;
                    //appt.pageNo = data.pageNo;

                    //appt.CSCPostedDate = a.CSCPostedDate;
                    //appt.CSCClosingDate = a.CSCClosingDate;
                    //appt.PSBDate = psb.psbDate;

                    appt.plantillaCode = prev.plantillaCode;
                    appt.publicationItemCode = prev.publicationItemCode;
                    appt.tag = 0;

                    tRSPAppointmentData apptData = new tRSPAppointmentData();
                    apptData.appointmentCode = data.appointmentCode;
                    //db.tRSPAppointmentDatas.Add(apptData);

                    //db.tRSPAppointments.Add(appt);
                    //db.SaveChanges();

                    return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);

                }



             
             
                

                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }



    }
}