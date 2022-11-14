using DDNHRIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DDNHRIS.Models.LnDViewModels;

using System.Net;
using System.Net.Mail;

namespace DDNHRIS.Controllers
{

    [SessionTimeout]
    [Authorize(Roles = "APRD")]

    public class RSPApplicationController : Controller
    {

        HRISDBEntities db = new HRISDBEntities();
        // GET: RSPApplication

        //[HttpPost]
        //public JsonResult PublicationPositionList()
        //{
        //    try
        //    {
        //        DateTime cutOffDate = DateTime.Now.AddDays(-90);
        //        IEnumerable<vRSPPublicationItem> pubItems = db.vRSPPublicationItems.Where(e => e.publicationDate.Value >= cutOffDate).OrderBy(o => o.itemNo).ToList();                 
        //        List<TempPSBScreening> myList = _GetPSBSchedule();               
        //        return Json(new { status = "success", list = pubItems, positionList = myList }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        string error = ex.ToString();
        //        return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
        //    }
        //}


        //private List<TempPSBScreening> _GetPSBSchedule()
        //{
        //    var positionList = db.vRSPScreenings.Select(e => new
        //    {
        //        e.publicationItemCode,
        //        e.transCode,
        //        e.itemCount,
        //        e.itemText,
        //        e.positionTitle,
        //        e.salaryGrade,
        //        e.rateMonth,
        //        e.departmentName,
        //        e.PSBDate,
        //        e.PSBVenue
        //    }).ToList();

        //    List<TempPSBScreening> myList = new List<TempPSBScreening>();
        //    foreach (var item in positionList)
        //    {
        //        IEnumerable<vRSPApplication> lst = db.vRSPApplications.Where(e => e.publicationItemCode == item.publicationItemCode).ToList();
        //        myList.Add(new TempPSBScreening
        //        {
        //            transCode = item.transCode,
        //            publicationItemCode = item.publicationItemCode,
        //            itemText = item.itemText,
        //            itemCount = item.itemCount,
        //            positionTitle = item.positionTitle,
        //            salaryGrade = Convert.ToInt16(item.salaryGrade),
        //            rateMonth = item.rateMonth,
        //            PSBDate = Convert.ToDateTime(item.PSBDate),
        //            PSBVenue = item.PSBVenue,
        //            departmentName = item.departmentName,
        //            applicantList = lst
        //        });
        //    }

        //    return myList;
        //}


        //private class TempPSBScreening
        //{

        //    public string transCode { get; set; }
        //    public string publicationItemCode { get; set; }
        //    public string itemCount { get; set; }
        //    public string itemText { get; set; }
        //    public string positionTitle { get; set; }
        //    public int salaryGrade { get; set; }
        //    public decimal? rateMonth { get; set; }
        //    public DateTime PSBDate { get; set; }
        //    public string PSBVenue { get; set; }
        //    public string departmentName { get; set; }
        //    public IEnumerable<vRSPApplication> applicantList { get; set; }
        //}


        /// <summary>
        /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// PROFILING
        /// </summary>
        public ActionResult Profiling()
        {
            return View();
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

        public JsonResult CheckCompetencyStatus(string id, int hasList)
        {
            try
            {
                string userCode = "";
                string passCode = "";
                string email = "";
                string appType = "OUTSIDER";
                tRSPApplication app = db.tRSPApplications.Single(e => e.applicationCode == id);
                if (app.appTypeCode == 1)
                {
                    appType = "INSIDER";
                    tRSPEmployee emp = db.tRSPEmployees.Single(e => e.EIC == app.EIC);
                    email = emp.emailAddress;
                }
                else
                {

                    tApplicant applicant = db.tApplicants.Single(e => e.applicantCode == app.applicantCode);
                    userCode = applicant.username;
                    passCode = applicant.password;
                    email = applicant.email;
                    if (applicant.username == null || applicant.password == null)
                    {
                        string tmpU = applicant.firstName.Trim();
                        string[] chunk = tmpU.Split(' ');

                        if (chunk.Length > 1)
                        {
                            userCode = chunk[0] + "." + chunk[1];
                        }
                        else
                        {
                            userCode = chunk[0] + Convert.ToDateTime(applicant.birthDate).ToString("MMdd");
                        }
                        System.Random random = new System.Random();
                        passCode = random.Next(1000, 9999) + "" + random.Next(1000, 9999);
                        applicant.username = userCode;
                        applicant.password = passCode;
                        db.SaveChanges();
                    }

                }

                if (hasList == 0)
                {
                    var supervisor = db.vRSPEmployeeLists.Select(e => new
                    {
                        e.EIC,
                        e.fullNameLast,
                        e.salaryGrade
                    }).Where(e => e.salaryGrade >= 12).OrderBy(o => o.fullNameLast).ToList();
                    return Json(new { status = "success", appType = appType, supervisors = supervisor, appUser = userCode, appCode = passCode, email = email }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = "success", appType = appType, appUser = userCode, appCode = passCode, email = email }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
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
                profile.educationProfile = db.tRSPApplicationProfileEducations.Where(e => e.applicationCode == id).ToList();
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


        [HttpPost]
        public JsonResult AddIPCRRating(tSPMSIPRCRating data)
        {
            try
            {

                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult SavePerformanceRating(string id, string code)
        {
            try
            {
                string uEIC = Session["_EIC"].ToString();
                string type = code.Substring(0, 1).ToString();
                string item = code.Substring(1, code.Length - 1);
                if (type == "1")
                {

                    int recNo = Convert.ToInt16(item);
                    tSPMSIPRCRating spms = db.tSPMSIPRCRatings.Single(e => e.recNo == recNo);


                    //IPCR PROFILE 
                    //tRSPApplicationProfileElig profileElig  =  db.tRSPApplicationProfileEligs.SingleOrDefault(e => e.sourceControlNo == spms.co)

                    string tmp = "IPCR " + spms.ratingAdj + " (" + spms.ratingNum.ToString() + ") " + spms.details;

                    tRSPApplicationProfile pro = new tRSPApplicationProfile();
                    pro.applicationCode = id;
                    pro.itemLevel = "";
                    pro.particulars = tmp;
                    pro.rating = spms.ratingNum;
                    pro.profileCode = "PM";
                    pro.orderNo = 1;
                    pro.userEIC = uEIC;
                    db.tRSPApplicationProfiles.Add(pro);

                    //SCORE
                    int pmScore = PerformanceScore(Convert.ToDouble(spms.ratingNum));
                    tRSPApplicationAssessment assmnt = db.tRSPApplicationAssessments.SingleOrDefault(e => e.applicationCode == id);
                    if (assmnt == null)
                    {
                        tRSPApplicationAssessment tmpAssmnt = new tRSPApplicationAssessment();
                        tmpAssmnt.applicationCode = id;
                        tmpAssmnt.performanceRating = spms.ratingNum;
                        tmpAssmnt.performanceScore = pmScore;
                        db.tRSPApplicationAssessments.Add(tmpAssmnt);

                    }
                    else
                    {
                        assmnt.applicationCode = id;
                        assmnt.performanceRating = spms.ratingNum;
                        assmnt.performanceScore = pmScore;
                    }

                    db.SaveChanges();
                }
                else if (type == "0")
                {

                }

                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        public int PerformanceScore(double rating)
        {
            int res = 0;
            if (rating >= 4.9)
            {
                return 20;
            }
            else if (rating >= 4.70)
            {
                return 19;
            }
            else if (rating >= 4.50)
            {
                return 18;
            }
            else if (rating >= 4.31)
            {
                return 17;
            }
            else if (rating >= 4.00)
            {
                return 16;
            }
            return res;
        }

        /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////        
        //[HttpPost]
        //public JsonResult SaveProfileData(tApplicantProfile data)
        //{
        //    try
        //    {
        //        tApplicantProfile p = new tApplicantProfile();
        //        p.applicationCode = data.applicationCode;
        //        p.education = data.education;
        //        p.experience = data.experience;
        //        p.training = data.training;
        //        p.performanceRating = data.performanceRating;
        //        p.tag = 1;
        //        db.tApplicantProfiles.Add(p);
        //        db.SaveChanges();
        //        return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception)
        //    {
        //        return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        public class TempComptGroup
        {
            public List<vLNDComptRespondentResult> core { get; set; }
            public List<vLNDComptRespondentResult> lead { get; set; }
            public List<vLNDComptRespondentResult> tech { get; set; }
        }



        [HttpPost]
        public JsonResult ViewAssessmentResult(string id)
        {
            try
            {
                //CHECK Competency Assessment Test
                int comptTag = 0;
                TempComptGroup compt = new TempComptGroup();


                tRSPApplicationAssessment assmnt = db.tRSPApplicationAssessments.SingleOrDefault(e => e.applicationCode == id);
                if (assmnt == null)
                {
                    tRSPApplicationAssessment res = new tRSPApplicationAssessment();
                    res.applicationCode = id;
                    res.performanceRating = null;
                    res.comptAssmentRating = null;
                    res.beiRating = null;
                    res.psychoRating = null;

                    res.experienceScore = null;
                    res.educationScore = null;
                    res.trainingScore = null;
                    res.eligibilityScore = null;
                    res.awardScore = null;
                    res.totalScore = 0;
                    db.tRSPApplicationAssessments.Add(res);
                    //ADD RECORD APPLICATION ASSESSMENT RESULT
                    tRSPApplicationAssessment tempData = new tRSPApplicationAssessment();
                    assmnt = res;

                    //ADD COMPETENCY ASSESSMENT 

                    tRSPApplicationComptAssmnt comptAssmnt = new tRSPApplicationComptAssmnt();
                    comptAssmnt.applicationCode = id;
                    comptAssmnt.tag = 0;

                    db.tRSPApplicationComptAssmnts.Add(comptAssmnt);

                    //COMIT;
                    db.SaveChanges();
                }
                else
                {
                    int hasChanges = 0;
                    //PERFORMANCE RATING
                    if (assmnt.performanceRating == null)
                    {
                        tApplicantIPCR peformance = db.tApplicantIPCRs.SingleOrDefault(e => e.applicantCode == id);
                        if (peformance != null)
                        {
                            int tmpPM = PerformanceScore(Convert.ToDecimal(peformance.NR));

                            assmnt.performanceRating = peformance.NR;
                            //assmnt.performanceScore = tmpPM;
                            hasChanges = 1;
                        }
                    }

                    //COMPETENCY ASSESSMENT
                    //tRSPApplicationComptAssmnt compt = db.tRSPApplicationComptAssmnts.Single(e => e.applicationCode == id);
                    tLNDComptRespondentApplicant comptAssmnt = db.tLNDComptRespondentApplicants.SingleOrDefault(e => e.applicationCode == id);




                    if (comptAssmnt.tag == null)
                    {
                        comptTag = 0;
                    }
                    else
                    {
                        comptTag = Convert.ToInt16(comptAssmnt.tag);
                    }

                    // AUTO COMPUTE


                    //5 DONE
                    if (comptTag == 5)
                    {

                         
                        IEnumerable<vLNDComptRespondentResult> results = db.vLNDComptRespondentResults.Where(e => e.respondentCode == assmnt.applicationCode).ToList();


                        int hasSup = 0;
                        if (comptAssmnt.supervisorEIC != null)
                        {
                            hasSup = 1;
                        }

                        List<vLNDComptRespondentResult> res = _GenerateScoreSheet(results, hasSup);

                        compt.core = res.Where(e => e.comptGroupCode == "CORE").ToList();
                        compt.lead = res.Where(e => e.comptGroupCode == "LEAD").ToList();
                        compt.tech = res.Where(e => e.comptGroupCode == "TECH").ToList();

                        double tempScore = 0;
                        int counter = 0;
                        double comptAve = 0;

                        foreach (vLNDComptRespondentResult itm in res)
                        {
                            counter = counter + 1;
                            tempScore = tempScore + Convert.ToInt16(itm.scoreEqv);
                        }

                        comptAve = tempScore / Convert.ToDouble(counter);

                        double myRating = TruncateDecimal(Convert.ToDouble(comptAve), Convert.ToInt16(2));

                        // decimal myScore = comptAve / counter;

                        assmnt.comptAssmentRating = Convert.ToDecimal(myRating);
                        assmnt.comptAssmentScore = Convert.ToDecimal(myRating);

                        //PSYCHO-SOCIAL TEST
                        tRSPApplicationComptAssmnt psy = db.tRSPApplicationComptAssmnts.SingleOrDefault(e => e.applicationCode == id && e.assmntType == 1);
                        if (psy != null)
                        {
                            //int myScore = Convert.ToInt16(_GetPsychoScore(id));
                            assmnt.psychoRating = psy.score;
                            assmnt.psychoScore = psy.score;
                        }
                        hasChanges = 1;
                    }
                    if (comptTag == 5)
                    {

                    }

                    if (hasChanges == 1)
                    {
                        assmnt.totalScore = Convert.ToInt16(assmnt.performanceRating) + Convert.ToInt16(assmnt.comptAssmentScore) + Convert.ToInt16(assmnt.beiScore) + Convert.ToInt16(assmnt.psychoScore) + Convert.ToInt16(assmnt.experienceScore) + Convert.ToInt16(assmnt.educationScore) + Convert.ToInt16(assmnt.trainingScore) + Convert.ToInt16(assmnt.eligibilityScore) + Convert.ToInt16(assmnt.awardScore);
                        db.SaveChanges();
                    }

                }

                //vRSPApplication applicationData = db.vRSPApplications.SingleOrDefault(e => e.applicationCode == id);
                return Json(new { status = "success", assmntResultData = assmnt, comptAssmntTag = comptTag, competency = compt }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }

        }

        private double TruncateDecimal(double value, int precision)
        {
            double step = (double)Math.Pow(10, precision);
            double tmp = Math.Truncate(step * value);
            return tmp / step;
        }

        private List<vLNDComptRespondentResult> _GenerateScoreSheet(IEnumerable<vLNDComptRespondentResult> result, int hasSupervisor)
        {
            List<vLNDComptRespondentResult> myList = new List<vLNDComptRespondentResult>();
            foreach (vLNDComptRespondentResult item in result)
            {
                decimal tempAve = 0;
                decimal tempScore = Convert.ToDecimal(item.scoreSA);
                tempAve = tempScore;

                if (hasSupervisor == 1)
                {
                    tempScore = tempScore + Convert.ToDecimal(item.scoreDS);
                    tempAve = tempScore / 2;
                }
                int s = (int)tempAve;
                int scoreEquivalent = Convert.ToInt16(_GetScore(Convert.ToDecimal(s), Convert.ToInt16(item.standard)));

                myList.Add(new vLNDComptRespondentResult()
                {
                    respondentCode = item.respondentCode,
                    comptCode = item.comptCode,
                    comptName = item.comptName,
                    comptGroupCode = item.comptGroupCode,
                    standard = item.standard,
                    scoreSA = item.scoreSA,
                    scoreDS = item.scoreDS,
                    scoreAve = tempAve,
                    scoreEqv = scoreEquivalent
                });
            }
            return myList;
        }

        private class StandardTable
        {
            public int standard { get; set; }
            public int basic { get; set; }
            public int interm { get; set; }
            public int advance { get; set; }
            public int superior { get; set; }
        }


        private int _GetScore(decimal rating, int standard)
        {
            int res = 0;
            List<StandardTable> table = new List<StandardTable>();
            table.Add(new StandardTable { standard = 1, basic = 6, interm = 7, advance = 8, superior = 10 });
            table.Add(new StandardTable { standard = 2, basic = 0, interm = 6, advance = 8, superior = 10 });
            table.Add(new StandardTable { standard = 3, basic = 0, interm = 0, advance = 6, superior = 10 });
            table.Add(new StandardTable { standard = 4, basic = 0, interm = 0, advance = 0, superior = 10 });



            StandardTable tab = table.Single(e => e.standard == standard);
            if (Convert.ToInt16(rating) >= Convert.ToInt16(tab.basic))
            {
                res = Convert.ToInt16(tab.basic);
            }
            if (Convert.ToInt16(rating) >= Convert.ToInt16(tab.interm))
            {
                res = Convert.ToInt16(tab.interm);
            }
            if (Convert.ToInt16(rating) >= Convert.ToInt16(tab.advance))
            {
                res = Convert.ToInt16(tab.advance);
            }
            if (Convert.ToInt16(rating) >= Convert.ToInt16(tab.superior))
            {
                res = Convert.ToInt16(tab.superior);
            }
            return res;
        }


        ////private decimal _GetPsychoScore(string respondentCode)
        ////{
        ////    IEnumerable<tLNDPsychoSocialTest> list = db.tLNDPsychoSocialTests.Where(e => e.respondentCode == respondentCode).ToList();
        ////    decimal result = 0;
        ////    int totalAnswer = 0;
        ////    int totalCounter = 0;
        ////    foreach (tLNDPsychoSocialTest item in list)
        ////    {
        ////        totalCounter = totalCounter + 1;
        ////        totalAnswer = totalAnswer + Convert.ToInt16(item.answer);
        ////    }
        ////    result = (totalAnswer / totalCounter) * 2;
        ////    return result;
        ////}

        [HttpPost]
        public JsonResult SubmitRating(string applicationCode, string id, int answer)
        {
            try
            {
                tRSPApplicationAssessment assmnt = db.tRSPApplicationAssessments.SingleOrDefault(e => e.applicationCode == applicationCode);

                if (id == "5")
                {
                    assmnt.experienceScore = answer;
                }
                else if (id == "6")
                {
                    assmnt.educationScore = answer;
                }
                else if (id == "7")
                {
                    assmnt.trainingScore = answer;
                }
                else if (id == "8")
                {
                    assmnt.eligibilityScore = answer;
                }
                else if (id == "9")
                {
                    assmnt.awardScore = answer;
                }

                assmnt.totalScore = Convert.ToInt16(assmnt.performanceScore) + Convert.ToInt16(assmnt.experienceScore) + Convert.ToInt16(assmnt.beiScore) + Convert.ToInt16(assmnt.psychoScore) + Convert.ToInt16(assmnt.experienceScore) + Convert.ToInt16(assmnt.educationScore) + Convert.ToInt16(assmnt.trainingScore) + Convert.ToInt16(assmnt.eligibilityScore) + Convert.ToInt16(assmnt.awardScore);

                db.SaveChanges();
                return Json(new { status = "success", assmntResultData = assmnt }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        private int PerformanceScore(decimal rating)
        {
            int res = 0;

            if (rating >= Convert.ToDecimal(4.90))
            {
                return 20;
            }
            else if (rating >= Convert.ToDecimal(4.70))
            {
                return 19;
            }
            else if (rating >= Convert.ToDecimal(4.50))
            {
                return 18;
            }
            else if (rating >= Convert.ToDecimal(4.31))
            {
                return 17;
            }
            else if (rating >= Convert.ToDecimal(4.00))
            {
                return 16;
            }
            return res;
        }

        public JsonResult ComptAssmntTestAllow(string id, string supervisorEIC, string email)
        {
            try
            {

                //if (IsValidEmail(email) == false)
                //{
                //    return Json(new { status = "Invalid email address!" }, JsonRequestBehavior.AllowGet);
                //}

                vRSPApplication application = db.vRSPApplications.SingleOrDefault(e => e.applicationCode == id);

                //CHECK COMPETENCY MAPPING
                tLNDCompetencyPositionMap map = db.tLNDCompetencyPositionMaps.SingleOrDefault(e => e.plantillaCode == application.plantillaCode && e.tag == 1);
                if (map == null)
                {
                    return Json(new { status = "Position not yet profiled!" }, JsonRequestBehavior.AllowGet);
                }
                //COMPETENCY LIST FOR THIS PLANTILLACODE
                IEnumerable<vLNDCompetencyStandard> comptList = db.vLNDCompetencyStandards.Where(e => e.comptPositionCode == map.comptPositionCode).OrderBy(o => o.orderNo).ToList();

                if (comptList.Count() == 0)
                {
                    return Json(new { status = "Position not yet profiled!" }, JsonRequestBehavior.AllowGet);
                }

                string uEIC = Session["_EIC"].ToString();
                string tempCode = "ASSMNT" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + uEIC.Substring(0, 4);

                tLNDComptAssessment comptAssessment = db.tLNDComptAssessments.SingleOrDefault(e => e.publicationItemCode == application.publicationItemCode);
                tApplicant applicant = db.tApplicants.SingleOrDefault(e => e.applicantCode == application.applicantCode);

                if (application.appTypeCode == 0)
                {
                    if (applicant.email == null || applicant.email == "")
                    {
                        if (IsValidEmail(email) == false)
                        {
                            return Json(new { status = "Invalid email address!" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }


                if (applicant != null && IsValidEmail(email) == true)
                {
                    applicant.email = email;
                }

                if (comptAssessment == null)
                {
                    //create competency assessmenttempCodetempCode
                    tLNDComptAssessment assmnt = new tLNDComptAssessment();
                    assmnt.assessmentCode = tempCode;
                    assmnt.assessmentName = "Competency Assessment for " +  Convert.ToInt16(application.itemNo).ToString("0000") + " - " + application.positionTitle;
                    assmnt.assessmentDetail = application.placeOfAssignment;
                    assmnt.assessmentType = 1;
                    assmnt.assmntGroupCode = application.departmentCode;
                    assmnt.publicationItemCode = application.publicationItemCode;

                    assmnt.tag = 0;
                    assmnt.userEIC = uEIC;
                    assmnt.transDT = DateTime.Now;
                    db.tLNDComptAssessments.Add(assmnt);
                }
                else
                {
                    tempCode = comptAssessment.assessmentCode;
                }

                tLNDComptRespondentApplicant resp = db.tLNDComptRespondentApplicants.SingleOrDefault(e => e.applicationCode == application.applicationCode);

                if (resp == null)
                {

                    tLNDComptRespondentApplicant app = new tLNDComptRespondentApplicant();
                    app.respondentCode = id;
                    app.assessmentCode = tempCode;
                    app.applicationCode = application.applicationCode;
                    app.transDT = DateTime.Now;

                    if (application.EIC != null)
                    {
                        app.supervisorEIC = supervisorEIC;
                    }

                    app.tag = 0;
                    app.userEIC = uEIC;
                    db.tLNDComptRespondentApplicants.Add(app);
                    db.SaveChanges();
                }

                //CHECK IF THERE IS AN ANSWER SHEET
                int count = db.tLNDComptRespondentAns.Where(e => e.respondentCode == application.applicationCode).Count();
                //CREATE ANSWER SHEET
                if (count == 0)
                {
                    int i = SetupComptAnswerSheet(application);
                    if (i == 0)
                    {
                        return Json(new { status = "Unable to update assessment status!" }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }


        [HttpPost]
        public JsonResult CheckComptAssmntEMail(string id)
        {
            try
            {
                vRSPApplication application = db.vRSPApplications.Single(e => e.applicationCode == id);
                tSystemEmailLog log = db.tSystemEmailLogs.Where(e => e.controlNo == application.applicationCode).OrderByDescending(o => o.recNo).Take(1).ToList().FirstOrDefault();

                string stat = "Send";
                if (log != null)
                {
                    stat = "Resend";
                }

                string status = "success";

                if (application.appTypeCode == 1)
                {
                    tRSPEmployee employee = db.tRSPEmployees.Single(e => e.EIC == application.EIC);
                    if (employee.emailAddress != null && IsValidEmail(employee.emailAddress))
                    {
                        status = "success";
                    }
                    else
                    {
                        status = "Invalid employee email address";
                    }

                }
                if (application.appTypeCode == 0)
                {
                    tApplicant applicant = db.tApplicants.Single(e => e.applicantCode == application.applicantCode);
                    if (applicant.email != null && IsValidEmail(applicant.email))
                    {
                        status = "success";
                    }
                    else
                    {
                        status = "Invalid applicant email address";
                    }
                }

                return Json(new { status = status, remarks = stat }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult SendEmailComptAssmntEmail(string id)
        {
            try
            {
                vRSPApplication application = db.vRSPApplications.SingleOrDefault(e => e.applicationCode == id);

                if (application != null)
                {

                    //CHECK FOR EMAIL

                    tApplicant applicant = new tApplicant();
                    tRSPEmployee employee = new tRSPEmployee();
                    string appEmail = applicant.email;
                    string lastNameTitle = "";
                    string username = "";
                    string password = "";
                    string url = "https://davaodelnorte.ph/JobPortal";
                    if (application.appTypeCode == 0)
                    {
                        applicant = db.tApplicants.Single(e => e.applicantCode == application.applicantCode);
                        appEmail = applicant.email;
                        username = applicant.username;
                        password = applicant.password;
                        lastNameTitle = applicant.lastName;
                        if (applicant.sex == "MALE")
                        {
                            lastNameTitle = "Mr. " + applicant.lastName;
                        }
                        else if (applicant.sex == "FEMALE")
                        {
                            lastNameTitle = "Ms. " + applicant.lastName;
                        }
                    }
                    else
                    {                       
                        url = "https://davaodelnorte.ph/HRMIS";
                        employee = db.tRSPEmployees.Single(e => e.EIC == application.EIC);
                        lastNameTitle = employee.lastName;
                        appEmail = employee.emailAddress;
                        if (employee.sex == "MALE")
                        {
                            lastNameTitle = "Mr. " + employee.lastName;
                        }
                        else if (employee.sex == "FEMALE")
                        {
                            lastNameTitle = "Ms. " + employee.lastName;
                        }
                    }
                    
                    string msg = "";
                    using (MailMessage mail = new MailMessage())
                    {                       
                        mail.From = new MailAddress("info@davaodelnorte.gov.ph", "DAVNOR Recruitment");
                        mail.To.Add(appEmail);
                        mail.Subject = "Competency Assessment";

                        if (application.appTypeCode == 0)
                        {
                            mail.Body = string.Format("Dear {0}, <BR/><BR/>As part of our screening process, you are requested to take part in the online competency assessment. Kindly follow the instructions stated below, to wit:  <BR/><BR/> <b>Step 1:  </b> Access this link: {1} <BR/><BR/> <b>Step 2:  </b> Log in using the given username and password. <BR/>  Username: <strong>{2}</strong> <BR/> Password: <strong>{3}</strong>  <BR/><BR/> <b>Step 3:  </b> Click “Take the Test” on the upper right side of the screen <BR/><BR/> <b>Step 4:  </b> Click <b>Option</b> at the upper right side of the screen then click <strong>Submit Assessment</strong>. <BR/><BR/>  *** Make sure you answered all categories of the examination before submitting. <BR/> *** Kindly comply with the said online competency assessment examination prior to the scheduled interview.<BR/><BR/><BR/><BR/> Very truly yours,<BR/><BR/> <b>EDWIN A. PALERO, MPA, MHRM (SGD)</b> <BR/> PG Department Head <BR/> Provincial Human Resource Management Office <BR/><BR/> <strong> *** THIS IS AN AUTOMATED MESSAGE, PLEASE DO NOT REPLY *** </strong>", lastNameTitle, url, username, password);
                        }
                        else if (application.appTypeCode == 1)
                        {                         
                            mail.Body = string.Format("Dear {0}, <BR/><BR/>As part of our screening process, you are requested to take part in the online competency assessment. Kindly follow the instructions stated below, to wit:  <BR/><BR/> <b>Step 1:  </b> Access this link: {1} <BR/><BR/> <b>Step 2:  </b> Log in using your HRIS Account then go to LEARNING & DEV \\ Assessment \\ Self Assessment <BR/><BR/> <b>Step 3:  </b>  Click “Take the Test” on the upper right side of the screen <BR/><BR/> <b>Step 4:  </b> Click <b>Option</b> at the upper right side of the screen then click <strong>Submit Assessment</strong>. <BR/><BR/>  *** Make sure you answered all  categories of the examination before submitting. <BR/> *** Kindly comply with the said online competency assessment examination prior to the scheduled interview.<BR/><BR/><BR/><BR/> Very truly yours,<BR/><BR/> <b>EDWIN A. PALERO, MPA, MHRM (SGD)</b> <BR/> PG Department Head <BR/> Provincial Human Resource Management Office <BR/><BR/> <strong> *** THIS IS AN AUTOMATED MESSAGE, PLEASE DO NOT REPLY *** </strong>", lastNameTitle, url);
                        }
                         
                        mail.IsBodyHtml = true;
                        ////mail.Attachments.Add(new Attachment("C:\\file.zip"));

                        using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                        {
                            smtp.UseDefaultCredentials = false;
                            smtp.Credentials = new NetworkCredential("info@davaodelnorte.gov.ph", "ddnInf0@hrm");
                            smtp.EnableSsl = true;
                            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                            smtp.Send(mail);
                        }

                        msg = mail.Body.ToString();
                        Random _random = new Random();
                        string code = "T" + DateTime.Now.ToString("yyMMddHHmmssfff") + "EMAIL" + _random.Next(1000, 9999); 

                        tSystemEmailLog log = new tSystemEmailLog();
                        log.tranCode = code;
                        log.controlNo = application.applicationCode;
                        log.recepient = appEmail;
                        log.message = msg;
                        log.transDT = DateTime.Now;
                        db.tSystemEmailLogs.Add(log);
                        db.SaveChanges();
                    }
                }
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
             
        }
        

        private int SetupComptAnswerSheet(vRSPApplication application)
        {
            //vRSPApplication application = db.vRSPApplications.Single(e => e.applicationCode == id);
            //CHECK COMPETENCY MAPPING
            tLNDCompetencyPositionMap map = db.tLNDCompetencyPositionMaps.SingleOrDefault(e => e.plantillaCode == application.plantillaCode && e.tag == 1);

            int assmntYear = 0;

            try
            {
                IEnumerable<vLNDCompetencyStandard> comptList = db.vLNDCompetencyStandards.Where(e => e.comptPositionCode == map.comptPositionCode).OrderBy(o => o.orderNo).ToList();
                //COMPETENCY TOOL LISTED                
                IEnumerable<vLNDToolList> toolListed = db.vLNDToolLists.Where(e => e.assessmentYear == assmntYear).OrderBy(o => o.orderNo).ToList();
                //COMPETENCY TOOL Progressive
                IEnumerable<vLNDToolProgressive> toolProg = db.vLNDToolProgressives.Where(e => e.assessmentYear == assmntYear).OrderBy(o => o.orderNo).ToList();

                List<LNDCompetency> myList = new List<LNDCompetency>();

                foreach (vLNDCompetencyStandard item in comptList)
                {
                    if (item.comptTool == "3") //LISTED
                    {
                        //LISTED HEADER
                        IEnumerable<vLNDToolList> header = toolListed.Where(e => e.comptCode == item.comptCode && e.MKBICode == null).OrderBy(o => o.orderNo).ToList();
                        foreach (vLNDToolList h in header)
                        {
                            //LISTED INDICATOR
                            IEnumerable<vLNDToolList> listed = toolListed.Where(e => e.MKBICode == h.KBICode).ToList();
                            foreach (vLNDToolList itm in listed)
                            {
                                tLNDComptRespondentAn a = new tLNDComptRespondentAn();
                                a.comptCode = item.comptCode;
                                a.respondentCode = application.applicationCode;
                                a.KIBCode = itm.KBICode;
                                db.tLNDComptRespondentAns.Add(a);
                            }
                        }
                    }
                    else //PROGRESSIVE
                    {
                        IEnumerable<vLNDToolProgressive> p = toolProg.Where(e => e.comptCode == item.comptCode).OrderBy(o => o.orderNo).ToList();
                        foreach (vLNDToolProgressive itm in p)
                        {
                            tLNDComptRespondentAn a = new tLNDComptRespondentAn();
                            a.comptCode = item.comptCode;
                            a.respondentCode = application.applicationCode;
                            a.KIBCode = itm.KBICode;
                            db.tLNDComptRespondentAns.Add(a);
                        }
                    }
                }

                //PSYCHO TEST
                IEnumerable<tLNDPsychoSocial> psychoTest = db.tLNDPsychoSocials.Where(e => e.groupTag == 1).ToList();
                foreach (tLNDPsychoSocial item in psychoTest)
                {
                    tLNDPsychoSocialTest ans = new tLNDPsychoSocialTest();
                    ans.respondentCode = application.applicationCode;
                    ans.psychoCode = item.psychoCode;
                    db.tLNDPsychoSocialTests.Add(ans);
                }
                tLNDComptRespondentApplicant resp = db.tLNDComptRespondentApplicants.Single(e => e.respondentCode == application.applicationCode);
                resp.tag = 1;
                db.SaveChanges();
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }


        // ***************************************************************************************************************************************
        //PRINT PROFILE

        //SAVE EDUCATION PROFILE
        [HttpPost]
        public JsonResult AddEducationProfile(tRSPApplicationProfileEducation data)
        {
            try
            {
                tPDSEducation pds = db.tPDSEducations.Single(e => e.controlNo == data.sourceControlNo);

                tRSPApplicationProfileEducation educ = new tRSPApplicationProfileEducation();
                educ.applicationCode = data.applicationCode;
                educ.education = pds.degree;
                educ.highestLevel = pds.highestLevel;
                educ.sourceControlNo = pds.controlNo;
                db.tRSPApplicationProfileEducations.Add(educ);
                db.SaveChanges();

                List<tPDSEducation> list = _GetPDSProfileData(pds.EIC, data.applicationCode);
                TempApplicantProfile profile = _GetApplicatnProfile(data.applicationCode);

                return Json(new { status = "success", pdsEducation = list, profileData = profile }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
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



        [HttpPost]
        public JsonResult SavePDSEntry(tPDSEducation data, string code)
        {
            try
            {
                string uEIC = Session["_EIC"].ToString();
                string temp = "T" + DateTime.Now.ToString("yyMMddHHmmssfff") + "PDSE" + data.EIC.Substring(0, 5);

                tPDSEducation e = new tPDSEducation();
                e.controlNo = temp;
                e.EIC = uEIC;
                e.educLevelCode = data.educLevelCode;
                e.schoolName = data.schoolName;
                e.degree = data.degree;
                e.periodYearFrom = data.periodYearFrom;
                e.periodYearTo = data.periodYearTo;
                e.highestLevel = data.highestLevel;
                e.yearGraduated = data.yearGraduated;
                e.honorsReceived = data.honorsReceived;
                e.encoderEIC = "";
                e.transDT = DateTime.Now;
                db.tPDSEducations.Add(e);
                db.SaveChanges();

                List<tPDSEducation> list = _GetPDSProfileData(data.EIC, code);
                TempApplicantProfile profile = _GetApplicatnProfile(code);

                return Json(new { status = "success", pdsEducation = list, profileData = profile }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult DeleteEducationItemData(tRSPApplicationProfileEducation data)
        {
            try
            {
                tPDSEducation pds = db.tPDSEducations.Single(e => e.controlNo == data.sourceControlNo);
                tRSPApplicationProfileEducation del = db.tRSPApplicationProfileEducations.Single(e => e.applicationCode == data.applicationCode && e.sourceControlNo == data.sourceControlNo);
                db.tRSPApplicationProfileEducations.Remove(del);
                db.SaveChanges();

                List<tPDSEducation> list = _GetPDSProfileData(pds.EIC, data.applicationCode);
                TempApplicantProfile profile = _GetApplicatnProfile(data.applicationCode);

                return Json(new { status = "success", pdsEducation = list, profileData = profile }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult PrintApplicantProfile(string id)
        {
            try
            {
                Session["ReportType"] = "APPLCNTPROFILE";
                Session["PrintReport"] = id;
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        //RSPApplication
        public class TempApplicantRegistration
        {
            public string lastName { get; set; }
            public string firstName { get; set; }
            public string middleName { get; set; }
            public string extName { get; set; }
            public DateTime birthDate { get; set; }
            public string sex { get; set; }
        }

        [HttpPost]
        public JsonResult RegisterApplicant(TempApplicantRegistration data)
        {
            try
            {
                string uEIC = Session["_EIC"].ToString();
                //ADD VALIDATION

                DateTime dob = Convert.ToDateTime(data.birthDate);
                data.birthDate = dob;
                string code = HashName(data);

                code = code = code + "0";

                string fullNameLast = data.lastName + ", " + data.firstName;
                string fullNameFirst = data.firstName;
                if (data.middleName != null && data.middleName.Length >= 1)
                {
                    fullNameFirst = fullNameFirst + " " + data.middleName.Substring(0, 1) + ". ";
                    fullNameLast = fullNameLast + " " + data.middleName.Substring(0, 1) + ".";
                }
                fullNameFirst = fullNameFirst + data.lastName;

                if (data.extName != null && data.extName.Length >= 1)
                {
                    fullNameFirst = fullNameFirst + " " + data.extName;
                    fullNameLast = fullNameLast + " " + data.extName;
                }


                tApplicant app = new tApplicant();
                app.applicantCode = code;
                app.lastName = data.lastName.Trim();
                app.firstName = data.firstName.Trim();
                app.fullNameFirst = fullNameFirst;
                app.fullNameLast = fullNameLast;
                app.middleName = data.middleName.Trim();
                app.birthDate = dob;
                app.sex = data.sex;
                app.isVerified = true;
                app.userEIC = uEIC;

                db.tApplicants.Add(app);
                db.SaveChanges();

                var appList = db.tApplicants.Select(e => new
                {
                    e.applicantCode,
                    e.fullNameLast,
                    e.isVerified
                }).Where(e => e.isVerified == true).OrderBy(e => e.fullNameLast).ToList();
                return Json(new { status = "success", applicantList = appList, applicantCode = code }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }



        //////////////////////////////////////////////////////////////////////////////////////////

        private string HashName(TempApplicantRegistration data)
        {
            string hash = "";

            string lastname = data.lastName.Trim().ToUpper();
            lastname = stringTrim(lastname, 1);


            string firstname = data.firstName.Trim().ToUpper();
            firstname = stringTrim(firstname, 1);


            string midIni = data.middleName;
            midIni = stringTrim(midIni, 0);

            string L = lastname.Substring(0, 1);   //Last L
            string F = firstname.Substring(0, 1);  //first F


            string midL23 = lastname.Substring(1, 2);   //last 2 & 3
            string midF23 = firstname.Substring(1, 2);  //first 2 & 3

            string SX = data.sex.Substring(0, 1);


            string M = "";
            int iMidLen = 0;
            string lenMid = "";
            if (midIni == null || midIni == "")
            {
                M = "0";
                lenMid = "00";
            }
            else
            {
                M = midIni.Substring(0, 1);
                iMidLen = Convert.ToInt32(midIni.Length);
                lenMid = iMidLen.ToString("00");
            }

            // d - last 3 of lastname
            string L3 = lastname.Substring(lastname.Length - 2);
            string F3 = firstname.Substring(firstname.Length - 3);

            //mid if lastname & firstname

            int iMidL = (int)lastname.Length / 2;
            int iMIdF = (int)firstname.Length / 2;

            string midL = lastname.Substring(iMidL, 1);
            string midF = firstname.Substring(iMIdF, 1);

            // f & g Len
            int x = Convert.ToInt32(lastname.Length);
            string lenL = x.ToString("00");
            int y = Convert.ToInt32(firstname.Length);
            string lenF = y.ToString("00");
            string dobYYYY = data.birthDate.ToString("yyyy");
            string dobMM = data.birthDate.ToString("MM");
            string dobDD = data.birthDate.ToString("dd");
            string revF3 = RevereseString(F3);
            string revL3 = RevereseString(L3);
            string LF3 = lastname.Substring(0, 3);
            dobYYYY = dobYYYY.Substring(0, 3);
            dobYYYY = RevereseString(dobYYYY);
            string revDOB = RevereseString(dobDD + dobMM);

            hash = F + L + M + SX + dobYYYY + lenL + midL + revL3 + lenF + midF + revF3 + LF3 + revDOB;
            return hash;
        }

        private string RevereseString(string myStr)
        {
            char[] myArr = myStr.ToArray();
            Array.Reverse(myArr);
            return new string(myArr);
        }

        private string stringTrim(string str, int tag)
        {
            if (str == null)
            {
                return "";
            }
            str = str.Replace("  ", " ");
            str = str.Replace("  ", " ");
            str = str.Replace(" ", "");
            str = str.Trim();
            if (tag == 1)
            {
                if (str.Length < 3)
                {
                    do
                    {
                        str = str + "0";
                    } while (str.Length < 3);
                }
            }

            return str;
        }

    }
}