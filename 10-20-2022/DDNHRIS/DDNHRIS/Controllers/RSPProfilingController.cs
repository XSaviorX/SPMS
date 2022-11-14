using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DDNHRIS.Models.LnDViewModels;
using DDNHRIS.Models;

namespace DDNHRIS.Controllers
{

    [SessionTimeout]
    [Authorize(Roles = "APRD")]
    public class RSPProfilingController : Controller
    {
        HRISDBEntities db = new HRISDBEntities();
        HRISEntities dbp = new HRISEntities();
        // GET: RSPProfiling


        [HttpPost]
        public JsonResult PublicationPositionList()
        {
            try
            {
                DateTime cutOffDate = DateTime.Now.AddDays(-90);
                IEnumerable<vRSPPublicationItem> pubItems = db.vRSPPublicationItems.Where(e => e.publicationDate.Value >= cutOffDate).OrderBy(o => o.itemNo).ToList();
                List<TempPSBScreening> myList = _GetPSBSchedule();
                return Json(new { status = "success", list = pubItems, positionList = myList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        private List<TempPSBScreening> _GetPSBSchedule()
        {
            string uEIC = Session["_EIC"].ToString();
            //var positionList = db.vRSPScreenings.Select(e => new
            //{
            //    e.publicationItemCode,
            //    e.transCode,
            //    e.itemCount,
            //    e.itemText,
            //    e.positionTitle,
            //    e.salaryGrade,
            //    e.rateMonth,
            //    e.departmentName,
            //    e.PSBDate,
            //    e.PSBVenue,
            //    e.tag,
            //    e.userEIC
            //}).Where(e => e.tag == 1 && e.userEIC == uEIC).OrderByDescending(o => o.PSBDate).ToList();

            var positionList = db.vRSPPSBSchedules.Select(e => new
            {
                e.publicationItemCode,
                e.transCode,
                e.itemCount,
                e.itemText,
                e.positionTitle,
                e.salaryGrade,
                e.rateMonth,
                e.departmentName,
                e.PSBDate,
                e.venue,
                e.tag,
                e.userEIC
            }).Where(e => e.tag == 1 && e.userEIC == uEIC).OrderBy(o => o.PSBDate).ToList();
           
            List<TempPSBScreening> myList = new List<TempPSBScreening>();
            foreach (var item in positionList)
            {
                IEnumerable<vRSPApplication> lst = db.vRSPApplications.Where(e => e.publicationItemCode == item.publicationItemCode).ToList();
                myList.Add(new TempPSBScreening
                {
                    transCode = item.transCode,
                    publicationItemCode = item.publicationItemCode,
                    itemText = item.itemText,
                    itemCount = Convert.ToInt16(item.itemCount),
                    positionTitle = item.positionTitle,
                    salaryGrade = Convert.ToInt16(item.salaryGrade),
                    rateMonth = item.rateMonth,
                    PSBDate = Convert.ToDateTime(item.PSBDate),
                    PSBVenue = item.venue,
                    departmentName = item.departmentName,
                    applicantList = lst
                });
            }
            return myList;
        }


        private class TempPSBScreening
        {

            public string transCode { get; set; }
            public string publicationItemCode { get; set; }
            public int itemCount { get; set; }
            public string itemText { get; set; }
            public string positionTitle { get; set; }
            public int salaryGrade { get; set; }
            public decimal? rateMonth { get; set; }
            public DateTime PSBDate { get; set; }
            public string PSBVenue { get; set; }
            public string departmentName { get; set; }
            public IEnumerable<vRSPApplication> applicantList { get; set; }
        }

        [HttpPost]
        public JsonResult ApplicantByPosition(string id)
        {
            try
            {
                vRSPPublicationItem pubData = db.vRSPPublicationItems.Single(e => e.publicationItemCode == id);
                List<vRSPApplication> list = ApplicantListByPublicationId(id);
                return Json(new { status = "success", pubItemData = pubData, applicantList = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        //APPLICANT LIST
        [HttpPost]
        public JsonResult EmployeeApplicantList()
        {
            try
            {
                var appList = db.tApplicants.Select(e => new
                {
                    e.applicantCode,
                    e.fullNameLast,
                    e.isVerified
                }).Where(e => e.isVerified == true).OrderBy(e => e.fullNameLast).ToList();

                var empList = db.vRSPEmployees.Select(e => new
                {
                    e.EIC,
                    e.fullNameLast,
                    e.positionTitle
                }).OrderBy(e => e.fullNameLast).ToList();

                return Json(new { status = "success", employeeList = empList, applicantList = appList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }



        private List<vRSPApplication> ApplicantListByPublicationId(string id)
        {
            try
            {
                vRSPPublicationItem pubData = db.vRSPPublicationItems.Single(e => e.publicationItemCode == id);
                IEnumerable<vRSPApplication> applicants = db.vRSPApplications.Where(e => e.publicationItemCode == id).OrderBy(o => o.applicantNameLast).ToList();

                List<vRSPApplication> myList = new List<vRSPApplication>();

                foreach (vRSPApplication item in applicants)
                {
                    string tempPosition = "Applicant";
                    int tempSalaryGrade = 0;
                    int tempStep = 0;
                    if (item.EIC != null)
                    {
                        tempPosition = item.positionTitle;
                        tempSalaryGrade = Convert.ToInt16(item.salaryGrade);
                        tempStep = Convert.ToInt16(item.applicantStep);
                    }

                    myList.Add(new vRSPApplication()
                    {
                        applicationCode = item.applicationCode,
                        EIC = item.EIC,
                        applicantCode = item.applicantCode,
                        applicantName = item.applicantName,
                        applicantNameLast = item.applicantNameLast,
                        applicantPositionTitle = tempPosition,
                        applicantSalaryGrade = tempSalaryGrade,
                        applicantStep = tempStep,
                        employmentStatusTag = item.employmentStatusTag
                    });
                }

                return myList;
            }
            catch (Exception)
            {
                List<vRSPApplication> list = new List<vRSPApplication>();
                return list;
            }
        }
         
        public class TempApplicantProfile
        {
            public string applicantName { get; set; }
            public string positionTitle { get; set; }
            public string sgStep { get; set; }
            public string applicantType { get; set; }
            public List<tRSPApplicationProfileEducation> educationProfile { get; set; }
            public List<tRSPApplicationProfileExpr> experienceProfile { get; set; }
            public List<tRSPApplicationProfileTraining> trainingProfile { get; set; }
            public List<tRSPApplicationProfileElig> eligibilityProfile { get; set; }
            public List<tRSPApplicationProfilePM> performanceRating { get; set; }
        }


        [HttpPost]
        public JsonResult SaveApplication(tRSPApplication data)
        {
            string uEIC = Session["_EIC"].ToString();
            string tmpCode = "APPL" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + uEIC.Substring(0, 4);

            tRSPPublicationItem positionItem = db.tRSPPublicationItems.SingleOrDefault(e => e.publicationItemCode == data.publicationItemCode);
            if (positionItem == null)
            {
                return Json(new { status = "Hacker Alert" }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                tRSPApplication a = new tRSPApplication();
                //check if exist
                if (data.appTypeCode == 0)
                {
                    tRSPApplication application = db.tRSPApplications.SingleOrDefault(e => e.publicationItemCode == data.publicationItemCode && e.applicantCode == data.applicantCode);
                    if (application != null)
                    {
                        return Json(new { status = "Application already exist!" }, JsonRequestBehavior.AllowGet);
                    }
                    a.applicantCode = data.applicantCode;
                }
                else
                {
                    tRSPApplication application = db.tRSPApplications.SingleOrDefault(e => e.publicationItemCode == data.publicationItemCode && e.EIC == data.EIC);
                    if (application != null)
                    {
                        return Json(new { status = "Application already exist!" }, JsonRequestBehavior.AllowGet);
                    }
                    a.EIC = data.EIC;
                }

                a.applicationCode = tmpCode;
                a.publicationItemCode = data.publicationItemCode;
                a.appTypeCode = data.appTypeCode;
                a.transDT = DateTime.Now;
                db.tRSPApplications.Add(a);
                db.SaveChanges();
                List<TempPSBScreening> myList = _GetPSBSchedule();
                List<vRSPApplication> list = ApplicantListByPublicationId(positionItem.publicationItemCode);
                return Json(new { status = "success", applicantList = list, positionList = myList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }



        [HttpPost]
        public JsonResult ViewApplicantProfile(string id)
        {
            try
            {
                vRSPApplication applicationData = db.vRSPApplications.SingleOrDefault(e => e.applicationCode == id);
                TempApplicantProfile profile = _GetApplicatnProfile(id);
                return Json(new { status = "success", applicationData = applicationData, profileData = profile }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        private TempApplicantProfile _GetApplicatnProfile(string id)
        {
            TempApplicantProfile profile = new TempApplicantProfile();
            try
            {
                vRSPApplication applicationData = db.vRSPApplications.SingleOrDefault(e => e.applicationCode == id);
                profile.applicantName = applicationData.applicantName;
                profile.positionTitle = applicationData.positionTitle;
                //profile.educationProfile = db.tRSPApplicationProfileEducations.Where(e => e.applicationCode == id).ToList();
                profile.experienceProfile = db.tRSPApplicationProfileExprs.Where(e => e.applicationCode == id).ToList();
                profile.trainingProfile = db.tRSPApplicationProfileTrainings.Where(e => e.applicationCode == id).ToList();
                profile.eligibilityProfile = db.tRSPApplicationProfileEligs.Where(e => e.applicationCode == id).ToList();
                profile.performanceRating = db.tRSPApplicationProfilePMs.Where(e => e.applicationCode == id).ToList();
                return profile;
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return profile;
            }
        }

        // ******** EDUCATION *********************************************************************************************

        [HttpPost]
        public JsonResult ShowApplicantPDSData(string id, string code)
        {
            try
            {
                List<tPDSEducation> educ = _GetPDSProfileData(id, code);
                return Json(new { status = "success", pdsEducation = educ }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
        }

        private List<tPDSEducation> _GetPDSProfileData(string EIC, string applicationCode)
        {
            List<tPDSEducation> list = new List<tPDSEducation>();
            IEnumerable<tPDSEducation> educ = db.tPDSEducations.Where(e => e.EIC == EIC).ToList();
            IEnumerable<tRSPApplicationProfileEducation> educProfile = db.tRSPApplicationProfileEducations.Where(e => e.applicationCode == applicationCode).ToList();

            foreach (tPDSEducation item in educ)
            {
                int tag = 0;
                if (educProfile.Where(e => e.sourceControlNo == item.controlNo).Count() >= 1)
                {
                    tag = 1;
                }
                string level = "";
                if (item.educLevelCode == "01")
                {
                    level = "ELEMENTARY";
                }
                if (item.educLevelCode == "02")
                {
                    level = "HIGH SCHOOL";
                }
                if (item.educLevelCode == "03")
                {
                    level = "VOCATIONAL";
                }
                if (item.educLevelCode == "04")
                {
                    level = "COLLEGE";
                }
                if (item.educLevelCode == "05")
                {
                    level = "GRADUATE STUDIES";
                }
                item.educLevelCode = level;
                item.isVerified = tag;
                list.Add(item);
            }

            return list;

        }



        //HRIS  v1. EDUCATIONAL BACKGROUND 
        [HttpPost]
        public JsonResult HRISPDSEducationList(string id)
        {
            try
            {
                List<TempHRIS1PDS> myList = GetHRISV1PDSEducation(id);
                return Json(new { status = "success", list = myList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        private List<TempHRIS1PDS> GetHRISV1PDSEducation(string id)
        {
            IEnumerable<tPDSEducation> PDSEducList = db.tPDSEducations.Where(e => e.EIC == id).ToList();

            IEnumerable<tapp212EducBackground> educList = dbp.tapp212EducBackground.Where(e => e.EIC == id).ToList();
            List<TempHRIS1PDS> myList = new List<TempHRIS1PDS>();
            foreach (tapp212EducBackground item in educList)
            {

                //int tag = PDSEducList.Where(e => e.versionTransNo == item.fRecNo).Count();
                int tag = 0;

                string levelName = "";
                if (item.fLevelTag == "01")
                {
                    levelName = "ELEMENTARY";
                }
                else if (item.fLevelTag == "02")
                {
                    levelName = "HIGH SCHOOL";
                }
                else if (item.fLevelTag == "03")
                {
                    levelName = "VOCATIONAL/ TRADE SCHOOL";
                }
                else if (item.fLevelTag == "04")
                {
                    levelName = "COLLEGE";
                }
                else if (item.fLevelTag == "05")
                {
                    levelName = "GRADUATE STUDIES";
                }
                myList.Add(new TempHRIS1PDS()
                {
                    EIC = item.EIC,
                    controlNo = item.fContrlNo,
                    levelTag = item.fLevelTag,
                    levelName = levelName,
                    schoolName = item.fSchoolName,
                    degree = item.fDegree,
                    yearGraduated = item.fYearGrad,
                    highestLevel = item.fHighLevel,
                    //periodYearFrom = item.fFromDT,
                    //periodYearTo = item.fToDT,
                    acadamicHonors = item.fHonors,
                    recNo = item.fRecNo,
                    tag = tag
                });
            }
            myList = myList.OrderBy(o => o.levelTag).ToList();
            return myList;
        }


        [HttpPost]
        public JsonResult SavePDSV1Migration(TempHRIS1PDS data, string appCode)
        {
            try
            {
                string id = data.EIC;
                string uEIC = Session["_EIC"].ToString();
                string code = "T" + DateTime.Now.ToString("yyMMddHHmmssfff") + "PDSE" + id.Substring(0, 5);

                tPDSEducation p = new tPDSEducation();
                p.controlNo = code;
                p.EIC = data.EIC;
                p.educLevelCode = data.levelTag;
                p.schoolName = data.schoolName;
                p.degree = data.degree;
                p.periodYearFrom = data.periodYearFrom;
                p.periodYearTo = data.periodYearTo;
                p.highestLevel = data.highestLevel;
                p.yearGraduated = data.yearGraduated;
                p.honorsReceived = data.acadamicHonors;
                //p.versionTransNo = data.recNo;
                p.encoderEIC = uEIC;
                p.transDT = DateTime.Now;
                db.tPDSEducations.Add(p);
                db.SaveChanges();

                List<TempHRIS1PDS> myList = GetHRISV1PDSEducation(id);
                List<tPDSEducation> pdsEduc = _GetPDSProfileData(data.EIC, appCode);

                return Json(new { status = "success", list = myList, pdsEducation = pdsEduc }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        public class TempHRIS1PDS
        {
            public Int64 recNo { get; set; }
            public string controlNo { get; set; }
            public string EIC { get; set; }
            public string levelTag { get; set; }
            public string levelName { get; set; }
            public string schoolName { get; set; }
            public string degree { get; set; }
            public string yearGraduated { get; set; }
            public string highestLevel { get; set; }
            public int periodYearFrom { get; set; }
            public int periodYearTo { get; set; }
            public string acadamicHonors { get; set; }
            public int tag { get; set; }

        }



        // TRAINING   *********************************************************************************************
        [HttpPost]
        public JsonResult PDSTrainingData(string id, string appCode)
        {
            try
            {
                List<TempPDSTraining> list = _GetPDSTraining(id, appCode).ToList();
                return Json(new { status = "success", trainingList = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        private List<TempPDSTraining> _GetPDSTraining(string id, string appCode)
        {
            List<TempPDSTraining> list = new List<TempPDSTraining>();
            IEnumerable<tPDSTraining> training = db.tPDSTrainings.Where(e => e.EIC == id);
            IEnumerable<tRSPApplicationProfileTraining> profileTraning = db.tRSPApplicationProfileTrainings.Where(e => e.applicationCode == appCode).ToList();

            foreach (tPDSTraining item in training)
            {
                int tag = 0;
               // if (profileTraning.Where(e => e.sourceControlNo == item.versionTransNo).Count() >= 1) { tag = 1; }
                list.Add(new TempPDSTraining()
                {
                    controlNo = item.controlNo,
                    recNo = item.recNo,
                    EIC = item.EIC,
                    trainingTitle = item.trainingTitle,
                    hoursNo = Convert.ToInt16(item.hoursNo),
                    trainingType = item.trainingTypeCode,
                    conductedBy = item.conductedBy,
                    tag = tag
                });
            }

            return list;
        }

        public class TempPDSTraining
        {
            public string controlNo { get; set; }
            public Int64 recNo { get; set; }
            public string EIC { get; set; }
            public string trainingTitle { get; set; }
            public DateTime dateFrom { get; set; }
            public DateTime dateTo { get; set; }
            public int hoursNo { get; set; }
            public string trainingType { get; set; }
            public string conductedBy { get; set; }
            public int tag { get; set; }
        }


        [HttpPost]
        public JsonResult LoadHRIS1PDSTraining(string id)
        {
            try
            {
                List<TempPDSTraining> list = _GetHRIS1TrainingList(id);
                return Json(new { status = "success", trainingList = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }

        }

        private List<TempPDSTraining> _GetHRIS1TrainingList(string id)
        {
            IEnumerable<tapp212Trainings> training = dbp.tapp212Trainings.Where(e => e.EIC == id).ToList();
            IEnumerable<tPDSTraining> pdsTrainingList = db.tPDSTrainings.Where(e => e.EIC == id).ToList();

            List<TempPDSTraining> list = new List<TempPDSTraining>();

            foreach (tapp212Trainings item in training)
            {
                int tag = 0;
                //if (pdsTrainingList.Where(e => e.versionTransNo == item.fRecNo).Count() >= 1)
                //{
                //    tag = 1;
                //}

                DateTime df = Convert.ToDateTime(item.fFromDT);
                DateTime dt = Convert.ToDateTime(item.ftoDT);

                list.Add(new TempPDSTraining()
                {
                    recNo = item.fRecNo,
                    EIC = item.EIC,
                    trainingTitle = item.fTitle,
                    dateFrom = df,
                    dateTo = dt,
                    hoursNo = Convert.ToInt16(item.fHoursNo),
                    trainingType = item.fTrainType,
                    conductedBy = item.fConducted,
                    tag = tag
                });
            }

            return list;
        }

        [HttpPost]
        public JsonResult PerformTrainingMigration(TempPDSTraining data, string appCode)
        {
            try
            {
                string uEIC = Session["_EIC"].ToString();
                string code = "T" + DateTime.Now.ToString("yyMMddHHmmssfff") + "PDST" + data.EIC.Substring(0, 5);

                //DateTime df = Convert.ToDateTime(item.fFromDT);
                //DateTime dt = Convert.ToDateTime(item.ftoDT);                 
                tPDSTraining p = new tPDSTraining();
                p.controlNo = code;
                p.EIC = data.EIC;
                p.trainingTitle = data.trainingTitle;
                p.dateFrom = data.dateFrom;
                p.dateTo = data.dateTo;
                p.hoursNo = Convert.ToInt16(data.hoursNo);
                p.trainingTypeCode = data.trainingType;
                p.conductedBy = data.conductedBy;
                p.transDT = DateTime.Now;
                //p.= uEIC;
                //p.versionTransNo = Convert.ToInt32(data.recNo);
                db.tPDSTrainings.Add(p);
                db.SaveChanges();

                List<TempPDSTraining> list = _GetHRIS1TrainingList(data.EIC);
                List<TempPDSTraining> pdsTraining = _GetPDSTraining(data.EIC, appCode).ToList();

                return Json(new { status = "success", pdsV1Training = list, pdsTraining = pdsTraining }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SaveTrainingProfile(TempPDSTraining data, string code)
        {
            try
            {
                tRSPApplicationProfileTraining t = new tRSPApplicationProfileTraining();
                t.applicationCode = code;
                t.hour = data.hoursNo;
                t.trainingTitle = data.trainingTitle;
                t.conductedBy = data.conductedBy;
                t.sourceControlNo = data.controlNo;
                db.tRSPApplicationProfileTrainings.Add(t);
                db.SaveChanges();

                List<TempPDSTraining> list = _GetPDSTraining(data.EIC, code).ToList();
                TempApplicantProfile profile = _GetApplicatnProfile(code);
                return Json(new { status = "success", trainingList = list, profileData = profile }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }



        //LoadApplicantEligibility
        // *****************************************************************************************
        [HttpPost]
        public JsonResult LoadApplicantEligibility(string id, string code)
        {
            try
            {
                List<TempEligibility> list = _GetElegibilityList(id, code);
                return Json(new { status = "success", list = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        private List<TempEligibility> _GetElegibilityList(string id, string code)
        {
            //ELIGIBILITIES
            IEnumerable<vPDSEligibility> eligList = db.vPDSEligibilities.Where(e => e.EIC == id).ToList();
            //PROFILE ELIG
            IEnumerable<tRSPApplicationProfileElig> profileElig = db.tRSPApplicationProfileEligs.Where(e => e.applicationCode == code).ToList();


            List<TempEligibility> list = new List<TempEligibility>();

            foreach (vPDSEligibility item in eligList)
            {

                int tag = 0;
                if (profileElig.Where(e => e.sourceControlNo == item.controlNo).Count() >= 1)
                {
                    tag = 1;
                }

                list.Add(new TempEligibility()
                {
                    controlNo = item.controlNo,
                    EIC = id,
                    applicationCode = code,
                    eligibilityName = item.eligibilityName,
                    tag = tag
                });
            }

            return list;

            //
            //IEnumerable<tRSPApplicationProfileElig> eligProfile = db.tRSPApplicationProfileEligs.Where(e => e.applicationCode == code);

        }

        public class TempEligibility
        {
            public string controlNo { get; set; }
            public string EIC { get; set; }
            public string applicationCode { get; set; }
            public string eligibilityName { get; set; }
            public string rating { get; set; }
            public int tag { get; set; }

        }


        //    public JsonResult SaveTrainingProfile(TempPDSTraining data, string code)
        [HttpPost]
        public JsonResult SaveEligibilityProfile(TempEligibility data)
        {
            try
            {
                tRSPApplicationProfileElig e = new tRSPApplicationProfileElig();
                e.applicationCode = data.applicationCode;
                e.eligibilityName = data.eligibilityName;
                e.sourceControlNo = data.controlNo;
                db.tRSPApplicationProfileEligs.Add(e);
                db.SaveChanges();

                List<TempEligibility> list = _GetElegibilityList(data.EIC, data.applicationCode);
                return Json(new { status = "success", list = list }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        //LoadApplicantEligibility
        // *****************************************************************************************


        [HttpPost]
        public JsonResult SPMSIPCRRatingList(string id, string code)
        {
            List<tSPMSIPRCRating> list = _GetIPCRRaint(id, code);
            return Json(new { status = "success", list = list }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public List<tSPMSIPRCRating> _GetIPCRRaint(string id, string code)
        {
            List<tSPMSIPRCRating> list = new List<tSPMSIPRCRating>();
            try
            {
                //SPMS IPCR
                IEnumerable<tSPMSIPRCRating> pmList = db.tSPMSIPRCRatings.Where(e => e.EIC == id).OrderByDescending(o => o.year).ThenByDescending(o => o.semester).ToList();
                //PROFILE PM
                IEnumerable<tRSPApplicationProfilePM> profilePM = db.tRSPApplicationProfilePMs.Where(e => e.applicationCode == code).ToList();

                foreach (tSPMSIPRCRating item in pmList)
                {
                    int tag = 0;
                    if (profilePM.Where(e => e.sourceControlNo == item.controlNo).Count() > 0)
                    {
                        tag = 1;
                    }
                    item.tag = tag;
                    list.Add(item);
                }

                return list;
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return list;
            }
        }


        [HttpPost]
        public JsonResult AddIPCRRating(tSPMSIPRCRating data)
        {
            try
            {
                string uEIC = Session["_EIC"].ToString();
                DateTime dt = DateTime.Now;
                string code = "T" + dt.ToString("yyMMddHHmm") + "PMI" + dt.ToString("ssfff") + data.EIC.Substring(0, 6);
                string detail = "";
                if (data.semester == 1)
                {
                    detail = "(" + Convert.ToDouble(data.ratingNum).ToString("#.00") + ") " + data.ratingAdj + " - 1st semester of " + data.year;
                }
                else
                {
                    detail = "(" + Convert.ToDouble(data.ratingNum).ToString("#.00") + ") " + data.ratingAdj + " - 2nd semester of " + data.year;
                }

                tSPMSIPRCRating pm = new tSPMSIPRCRating();
                pm.controlNo = code;
                pm.EIC = data.EIC;
                pm.ratingNum = data.ratingNum;
                pm.ratingAdj = data.ratingAdj;
                pm.semester = data.semester;
                pm.year = data.year;
                pm.details = detail;
                pm.tag = 1;
                pm.remarks = data.remarks;
                pm.encoderEIC = uEIC;
                pm.transDT = DateTime.Now;
                db.tSPMSIPRCRatings.Add(pm);
                db.SaveChanges();

                //RETURN LIST

                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }



        [HttpPost]
        public JsonResult AddPerformanceRatingProfile(tSPMSIPRCRating data, string code)
        {
            tRSPApplicationProfilePM pm = new tRSPApplicationProfilePM();
            pm.applicationCode = code;
            pm.period = data.details;
            pm.sourceControlNo = data.controlNo;
            db.tRSPApplicationProfilePMs.Add(pm);
            db.SaveChanges();
            List<tSPMSIPRCRating> list = _GetIPCRRaint(data.EIC, code);
            return Json(new { status = "success", list = list }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RemovePerformanceRatingItem(string id, tRSPApplicationProfilePM data)
        {
            tRSPApplicationProfilePM rem = db.tRSPApplicationProfilePMs.Single(e => e.sourceControlNo == data.sourceControlNo && e.applicationCode == data.applicationCode);
            db.tRSPApplicationProfilePMs.Remove(rem);
            db.SaveChanges();
            List<tSPMSIPRCRating> list = _GetIPCRRaint(id, data.applicationCode);
            TempApplicantProfile profile = _GetApplicatnProfile(data.applicationCode);
            return Json(new { status = "success", list = list, profileData = profile }, JsonRequestBehavior.AllowGet);
        }

        // 
        // *****************************************************************************************


        [HttpPost]
        public JsonResult ViewSPMSList(string id)
        {
            try
            {

                int code = 0;
                vRSPApplication applicationData = db.vRSPApplications.SingleOrDefault(e => e.applicationCode == id);

                if (applicationData.appTypeCode == 1)
                {
                    code = 1;
                }

                if (applicationData != null)
                {
                    List<SelectListItem> listItem = new List<SelectListItem>();
                    if (code == 0) //APPLICANT
                    {

                    }
                    else if (code == 1) //EMPLOYEE
                    {
                        IEnumerable<tSPMSIPRCRating> spms = db.tSPMSIPRCRatings.Where(e => e.EIC == applicationData.EIC).OrderByDescending(o => o.year).ThenByDescending(o => o.semester).ToList();
                        foreach (tSPMSIPRCRating item in spms)
                        {
                            string sem = "2nd Semester";
                            if (item.semester == 1)
                            {
                                sem = "1st Semester";
                            }

                            string tmp = "IPCR " + item.ratingAdj + " (" + item.ratingNum.ToString() + ") " + sem + " of " + item.year;
                            listItem.Add(new SelectListItem()
                            {
                                Text = tmp,
                                Value = "1" + item.recNo.ToString()
                            });
                        }
                    }

                    return Json(new { status = "success", list = listItem }, JsonRequestBehavior.AllowGet);
                }


                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }



        public JsonResult SaveAppointee(string id)
        {
            try
            { 
                DateTime dt = DateTime.Now;
                vRSPApplication application = db.vRSPApplications.Single(e => e.applicationCode == id);
                tRSPPSBSchedule  schedule = db.tRSPPSBSchedules.SingleOrDefault(e => e.publicationItemCode == application.publicationItemCode);
                if (schedule == null || schedule.PSBDate > dt)
                {
                    return Json(new { status = "Invalid PSB schedule date!" }, JsonRequestBehavior.AllowGet);
                }
                string tmpEIC = application.EIC;
                if (application.appTypeCode == 0)
                {
                    tmpEIC = application.applicantCode;
                }
                string code = "T" + DateTime.Now.ToString("yyMMddHHmmfff") + "APPT" + tmpEIC.Substring(0, 7);
                tRSPPSBApp app = new tRSPPSBApp();
                app.appointmentCode = code;
                app.applicationCode = id;
                app.EIC = tmpEIC;
                app.applicationCode = application.applicationCode;
                app.plantillaCode = application.plantillaCode;
                app.psbDate = schedule.PSBDate;
                app.transDT = DateTime.Now;
                db.tRSPPSBApps.Add(app);
                schedule.tag = 0;
                db.SaveChanges();
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult GetPublicationData()
        {
            try
            {
                List<SelectListItem> myList = new List<SelectListItem>();
                DateTime dt = DateTime.Now.AddDays(-270);
                IEnumerable<vRSPPublicationItem> list = db.vRSPPublicationItems.Where(e => e.CSCClosingDate >= dt).ToList();
                foreach (vRSPPublicationItem item in list)
                {
                    myList.Add(new SelectListItem()
                    {
                        Text = Convert.ToInt16(item.itemNo).ToString("0000") +  " " + item.positionTitle,
                        Value = item.publicationItemCode    
                    });
                }
                return Json(new { status = "success", pubList = myList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult SavePSBSchedule(tRSPPSBSchedule data)
        {
            try
            {
                string uEIC = Session["_EIC"].ToString();
                vRSPPublicationItem pub = db.vRSPPublicationItems.SingleOrDefault(e => e.publicationItemCode == data.publicationItemCode);
                string temp = "T" + DateTime.Now.ToString("yyMMddHHmmssfff") + "PSB" + Convert.ToInt16(pub.itemNo).ToString("0000") + uEIC.Substring(0,2);
                     
                if (pub != null)
                {
                    tRSPPSBSchedule psb = new tRSPPSBSchedule();
                    psb.transCode = temp;
                    psb.publicationItemCode = pub.publicationItemCode;
                    psb.itemCount = 1;
                    psb.itemText = Convert.ToInt16(pub.itemNo).ToString("0000");
                    psb.PSBDate = data.PSBDate;
                    psb.venue = "";
                    psb.remarks = "";
                    psb.tag = 1;
                    psb.transDT = DateTime.Now;
                    psb.userEIC = uEIC;
                    db.tRSPPSBSchedules.Add(psb);
                    db.SaveChanges();
                 
                    List<TempPSBScreening> myList = _GetPSBSchedule();
                    //return Json(new { status = "success", list = pubItems, positionList = myList }, JsonRequestBehavior.AllowGet);

                    return Json(new { status = "success", positionList = myList }, JsonRequestBehavior.AllowGet);


                }

                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }



    }
}