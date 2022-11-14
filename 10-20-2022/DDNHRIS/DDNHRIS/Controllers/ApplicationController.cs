using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DDNHRIS.Models;
using DDNHRIS.Models.LnDViewModels;


namespace DDNHRIS.Controllers
{

    [SessionTimeout]
    [Authorize(Roles = "EMP,Applicant")]

    public class ApplicationController : Controller
    {
        HRISDBEntities db = new HRISDBEntities();

        // GET: Application
        public ActionResult JobApp()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ApplicationList()
        {
            try
            {
                string uEIC = Session["_EIC"].ToString();
                IEnumerable<vRSPApplication> appList = db.vRSPApplications.Where(e => e.EIC == uEIC).ToList();
                return Json(new { status = "success", applicationList = appList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }



        [HttpPost]
        public JsonResult VacantPosition()
        {
            try
            {
                DateTime cutOffDate = DateTime.Now.AddDays(-90);
                IEnumerable<vRSPPublicationItem> pubItems = db.vRSPPublicationItems.Where(e => e.publicationDate.Value >= cutOffDate).OrderBy(o => o.itemNo).ToList();
                return Json(new { status = "success", vacantPositions = pubItems }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult PositionJD(string id)
        {
            try
            {
                vRSPPublicationItem position = db.vRSPPublicationItems.Single(e => e.publicationItemCode == id);
                IEnumerable<tRSPPositionJobDesc> jobDescList = db.tRSPPositionJobDescs.Where(e => e.plantillaCode == position.plantillaCode).OrderBy(o => o.jobSeqNo).ToList();
                return Json(new { status = "success", vacantPositions = position, jobDescList = jobDescList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult ApplyThisJob(string id)
        {
            try
            {
                vRSPPublicationItem pub = db.vRSPPublicationItems.SingleOrDefault(e => e.publicationItemCode == id);
                string uEIC = Session["_EIC"].ToString();

                if (pub != null)
                {
                    //CHECK IF POSITION ALREADY APPLIED
                    tRSPApplication app = db.tRSPApplications.SingleOrDefault(e => e.publicationItemCode == id && e.EIC == uEIC);
                    if (app == null)
                    {
                        string tmpStr = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                        string tempCode = "APPL" + tmpStr + uEIC.Substring(0, 5);

                        tRSPApplication a = new tRSPApplication();
                        a.applicationCode = tempCode;
                        a.EIC = uEIC;
                        a.publicationItemCode = id;
                        a.transDT = DateTime.Now;
                        a.appTypeCode = 1;
                        a.tag = 0;
                        a.remarks = "";
                        db.tRSPApplications.Add(a);
                        db.SaveChanges();
                        return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { status = "Unable to save application!" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = "Unable to save application!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "An error occurred!" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult CheckCompetencyAssessment(string id)
        {
            try
            {
                tRSPApplicationComptAssmnt compt = db.tRSPApplicationComptAssmnts.SingleOrDefault(e => e.applicationCode == id);
                
                if (compt == null)
                {
                    tRSPApplicationComptAssmnt tmpCompt = new tRSPApplicationComptAssmnt();
                    tmpCompt.applicationCode = "";
                    tmpCompt.tag = 0;
                    return Json(new { status = "success", competency = tmpCompt }, JsonRequestBehavior.AllowGet);
                }

                //compt assessment tag
                // 0 - created
                // 1 - available for test
                // 2 - started
                // 3 - submitted

                List<LNDCompetency> myList = new List<LNDCompetency>();

                if (compt.tag == 1)
                {

                    //CHECK APPLICATION TO GET PLANTILLA
                    vRSPApplication application = db.vRSPApplications.Single(e => e.applicationCode == compt.applicationCode);
                    //CHECK COMPETENCY MAPPING
                    tLNDCompetencyPositionMap map = db.tLNDCompetencyPositionMaps.SingleOrDefault(e => e.plantillaCode == application.plantillaCode && e.tag == 1);
                    //COMPETENCY LIST FOR THIS PLANTILLACODE
                    IEnumerable<vLNDCompetencyStandard> comptList = db.vLNDCompetencyStandards.Where(e => e.comptPositionCode == map.comptPositionCode).OrderBy(o => o.orderNo).ToList();

                    //GET RESPONSE/ANSWER (check of previous answers)
                    //IEnumerable<tLNDComptRespondentAn> responseList = db.tLNDComptRespondentAns.Where(e => e.respondentCode == respondent.respondentCode);
                    IEnumerable<tLNDComptRespondentAn> responseList = db.tLNDComptRespondentAns.Where(e => e.respondentCode == application.applicationCode).ToList();

                    if (map != null)
                    {
                        IEnumerable<vLNDToolList> toolListed = db.vLNDToolLists.Where(e => e.assessmentYear == 2020).OrderBy(o => o.orderNo).ToList();
                        //TOOL Progressive
                        IEnumerable<vLNDToolProgressive> toolProg = db.vLNDToolProgressives.Where(e => e.assessmentYear == 2020).OrderBy(o => o.orderNo).ToList();

                        int i = 0;
                        foreach (vLNDCompetencyStandard item in comptList)
                        {
                            i++;
                            List<LNDIndicators> ndList = new List<LNDIndicators>();

                            if (item.comptTool == "3")
                            {
                                IEnumerable<vLNDToolList> header = toolListed.Where(e => e.comptCode == item.comptCode && e.MKBICode == null).OrderBy(o => o.orderNo).ToList();
                                foreach (vLNDToolList h in header)
                                {
                                    IEnumerable<vLNDToolList> listed = toolListed.Where(e => e.MKBICode == h.KBICode).ToList();
                                    foreach (vLNDToolList itm in listed)
                                    {
                                        int ansSA = 0; int ansDS = 0;
                                        if (responseList.Count() > 0)
                                        {
                                            tLNDComptRespondentAn resp = responseList.SingleOrDefault(e => e.respondentCode == id && e.KIBCode == itm.KBICode);
                                            if (resp != null)
                                            {
                                                ansSA = Convert.ToInt16(resp.answerSA);
                                                ansDS = Convert.ToInt16(resp.answerDS);
                                            }
                                        }
                                        i = i + 1;
                                        // ADD TO LIST
                                        ndList.Add(new LNDIndicators()
                                        {
                                            itemNo = i,
                                            header = h.KBI,
                                            KBICode = itm.KBICode,
                                            KBI = itm.KBI,
                                            answerSA = ansSA,
                                            answerDS = ansDS,
                                            orderNo = Convert.ToInt16(itm.orderNo),
                                            tag = 1
                                        });
                                    }
                                }

                            }
                            else
                            {
                                IEnumerable<vLNDToolProgressive> p = toolProg.Where(e => e.comptCode == item.comptCode).OrderBy(o => o.orderNo).ToList();
                                foreach (vLNDToolProgressive itm in p)
                                {

                                    int ansSA = 0; int ansDS = 0;
                                    if (responseList.Count() > 0)
                                    {
                                        var cod = responseList.SingleOrDefault(e => e.respondentCode == id && e.KIBCode == itm.KBICode).answerSA;
                                        if (cod >= 1)
                                        {
                                            tLNDComptRespondentAn resp = responseList.SingleOrDefault(e => e.respondentCode == id && e.KIBCode == itm.KBICode);
                                            if (resp != null)
                                            {
                                                ansSA = Convert.ToInt16(resp.answerSA);
                                                ansDS = Convert.ToInt16(resp.answerDS);
                                            }
                                        }
                                    }
                                    //add to list
                                    i = i + 1;
                                    ndList.Add(new LNDIndicators()
                                    {
                                        itemNo = i,
                                        KBICode = itm.KBICode,
                                        KBI = "Progressive",
                                        comptLevel = itm.comptLevel,
                                        basic = itm.basic,
                                        intermediate = itm.intermediate,
                                        advance = itm.advance,
                                        superior = itm.superior,
                                        answerSA = ansSA,
                                        answerDS = ansDS,
                                        orderNo = Convert.ToInt16(itm.orderNo),
                                        tag = 1
                                    });
                                }

                            }

                            myList.Add(new LNDCompetency()
                            {
                                comptCode = item.comptCode,
                                comptName = item.comptName,
                                comptDesc = item.comptDesc,
                                comptGroupCode = item.comptGroupCode,
                                comptTool = item.comptTool,
                                KBIList = ndList,
                                orderNo = Convert.ToInt16(item.orderNo)
                            });
                        }

                        var coreList = myList.Where(e => e.comptGroupCode == "CORE").ToList();
                        var leadList = myList.Where(e => e.comptGroupCode == "LEAD").ToList();
                        var techList = myList.Where(e => e.comptGroupCode == "TECH").ToList();

                        var psychoTest = db.vLNDPsychoSocialTests.Where(e => e.respondentCode == id);

                        return Json(new { status = "success", competency = compt, core = coreList, lead = leadList, tech = techList, psychoTest = psychoTest }, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        return Json(new { status = "success", competency = compt }, JsonRequestBehavior.AllowGet);
                        //return null
                    }

                    //RETURN COMPETENCY ASSESSMENT
                }
                else
                {
                    //dont return anything...
                }
                return Json(new { status = "success", competency = compt }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "An error occurred!" }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult SubmitAnswer(string applicationCode, string KBICode, int ans)
        {
            try
            {
                string res = "";
                tLNDComptRespondentAn subAns = db.tLNDComptRespondentAns.SingleOrDefault(e => e.respondentCode == applicationCode && e.KIBCode == KBICode);
                if (subAns != null)
                {
                    subAns.answerSA = ans;
                    db.SaveChanges();
                    res = "success";
                }
                else
                {
                    //LOOP AND SAVE COMPETENCY ANSWER
                    //if (SetupComptAnswerSheet(applicationCode) == 1)
                    //{
                    //    tLNDComptRespondentAn firstAns = db.tLNDComptRespondentAns.SingleOrDefault(e => e.respondentCode == applicationCode && e.KIBCode == KBICode);
                    //    firstAns.answerSA = ans;
                    //    db.SaveChanges();
                    //    res = "success";
                    //}
                    res = "An error occurred!";
                }
                return Json(new { status = res }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(new { status = "An error occurred!" }, JsonRequestBehavior.AllowGet);
            }
        }


        //Submit PSYCHO ANSWER
        [HttpPost]
        public JsonResult SubmitPsychoAnswer(string applicationCode, string psychoCode, int ans)
        {
            string res = "";
            try
            {
                tLNDPsychoSocialTest psyAns = db.tLNDPsychoSocialTests.SingleOrDefault(e => e.respondentCode == applicationCode && e.psychoCode == psychoCode);
                if (psyAns != null)
                {
                    psyAns.answer = ans;
                    db.SaveChanges();
                    res = "success";
                }
                else
                {
                    res = "Error!";
                }
                return Json(new { status = res }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //SUBMT SELFT ASSESSMENT
        public JsonResult SubmitSelfAssessment(string id)
        {

            vRSPApplication application = db.vRSPApplications.Single(e => e.applicationCode == id);

            tRSPApplicationComptAssmnt comptAssmnt = db.tRSPApplicationComptAssmnts.Single(e => e.applicationCode == id);
          
            //check if all KBI answered
            int count = db.tLNDComptRespondentAns.Where(e => e.respondentCode == application.applicationCode && e.answerSA == null).Count();
            if (count >= 1)
            {
                string errMsg = "There is 1 item unanswered in Competency Test!";
                if (count > 1)
                {
                    errMsg = "There are " + count + " items unanswered in Competency Test!";
                }

                return Json(new { status = errMsg }, JsonRequestBehavior.AllowGet);
            }

            count = db.vLNDPsychoSocialTests.Where(e => e.respondentCode == application.applicationCode && e.answer == null).Count();
            if (count >= 1)
            {
                string errMsg = "There is 1 item unanswered in Psycho-Social Test";
                if (count > 1)
                {
                    errMsg = "There are " + count + " items unanswered in Psycho-Social Test!";
                }

                return Json(new { status = errMsg }, JsonRequestBehavior.AllowGet);
            }


            if (comptAssmnt != null)
            {
                if (comptAssmnt.supervisorEIC != null)
                {
                    comptAssmnt.tag = 2;
                }
                else if (comptAssmnt.supervisorEIC == null)
                {

                    decimal rating = 0;
                    decimal rateCounter = 0;

                    //COMPETENCY ASSESSMENT RESULT
                    List<tLNDComptRespondentResult> result = AssessmentResult(comptAssmnt);
                    if (result.Count() > 0)
                    {
                        foreach (tLNDComptRespondentResult item in result)
                        {
                            rateCounter = rateCounter + 1;
                            tLNDComptRespondentResult n = new tLNDComptRespondentResult();
                            n.respondentCode = item.respondentCode;
                            n.comptCode = item.comptCode;
                            n.scoreSA = item.scoreSA;
                            n.scoreDS = item.scoreDS;
                            n.scoreAve = item.scoreAve;
                            n.scoreEqv = item.scoreEqv;
                            db.tLNDComptRespondentResults.Add(n);
                            rating = rating + Convert.ToDecimal(item.scoreAve);
                        }
                        rating = rating / rateCounter;
                    }

                    //PSYCHO-SOCIAL TEST RESULT
                    decimal pshcoTestResult = PsychoScore(comptAssmnt.applicationCode);

                    tRSPApplicationComptAssmnt psy = new tRSPApplicationComptAssmnt();
                    psy.applicationCode = comptAssmnt.applicationCode;
                    psy.assmntType = 1;
                    psy.score = Convert.ToInt16(pshcoTestResult);
                    //psy.transDT = DateTime.Now;
                    db.tRSPApplicationComptAssmnts.Add(psy);
                    
                    comptAssmnt.tag = 3;
                    comptAssmnt.rating = rating;
                }

                db.SaveChanges(); //comit changes
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "Invalid transaction!" }, JsonRequestBehavior.AllowGet);
            }
            
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private List<tLNDComptRespondentResult> AssessmentResult(tRSPApplicationComptAssmnt comptAssmnt)
        {
                     
            vRSPApplication application = db.vRSPApplications.Single(e => e.applicationCode == comptAssmnt.applicationCode);
            tLNDCompetencyPositionMap map = db.tLNDCompetencyPositionMaps.SingleOrDefault(e => e.plantillaCode == application.plantillaCode && e.tag == 1);

            string tmpComptCode = map.comptPositionCode;

            int asmntLevelNo = 1;
            if (comptAssmnt.supervisorEIC != null)
            {
                asmntLevelNo = 2;
            }

            IEnumerable<vLNDCompetencyStandard> comptStatndard = db.vLNDCompetencyStandards.Where(e => e.comptPositionCode == tmpComptCode).ToList();
            IEnumerable<tLNDComptRespondentAn> responseList = db.tLNDComptRespondentAns.Where(e => e.respondentCode == comptAssmnt.applicationCode).ToList();

            IEnumerable<tLNDComptCriteria> criteriaList = db.tLNDComptCriterias.Where(e => e.standardNo > 0).ToList();

            List<tLNDComptRespondentResult> listItem = new List<tLNDComptRespondentResult>();

            foreach (vLNDCompetencyStandard item in comptStatndard)
            {
                IEnumerable<tLNDComptRespondentAn> respnsList = responseList.Where(e => e.comptCode == item.comptCode).ToList();

                decimal myScoreSA = 0;
                decimal myScoreDS = 0;
                int levelCounter = 0;

                foreach (tLNDComptRespondentAn itm in respnsList)
                {
                    levelCounter = levelCounter + 1;

                    int tmpSA = ComptAssmntCriteria(criteriaList, Convert.ToInt16(item.standardCode), Convert.ToInt16(itm.answerSA));
                    myScoreSA = myScoreSA + tmpSA;

                    int tmpDS = ComptAssmntCriteria(criteriaList, Convert.ToInt16(item.standardCode), Convert.ToInt16(itm.answerDS));
                    myScoreDS = myScoreDS + tmpDS;
                }

                myScoreSA = myScoreSA / levelCounter;
                myScoreDS = myScoreDS / levelCounter;

                decimal scoreAve = 0;

                if (asmntLevelNo == 1)
                {
                    scoreAve = myScoreSA;
                }
                else if (asmntLevelNo == 2)
                {
                    scoreAve = (myScoreSA + myScoreDS) / 2;
                }

                listItem.Add(new tLNDComptRespondentResult()
                {
                    respondentCode = comptAssmnt.applicationCode,
                    comptCode = item.comptCode,
                    scoreSA = myScoreSA,
                    scoreAve = scoreAve,
                    scoreEqv = ComptAssmntLevel(criteriaList, Convert.ToInt16(item.standardCode), scoreAve)
                });

            }
            return listItem;

        }


        private decimal PsychoScore(string respondentCode)
        {
            IEnumerable<tLNDPsychoSocialTest> list = db.tLNDPsychoSocialTests.Where(e => e.respondentCode == respondentCode).ToList();
            decimal result = 0;
            int totalAnswer = 0;
            int totalCounter = 0;
            foreach (tLNDPsychoSocialTest item in list)
            {
                totalCounter = totalCounter + 1;
                totalAnswer = totalAnswer + Convert.ToInt16(item.answer);
            }
            result = (totalAnswer / totalCounter) * 2;
            return result;
        }


        private int ComptAssmntCriteria(IEnumerable<tLNDComptCriteria> criteriaData, int iStand, int ans)
        {
            int i = 0;

            tLNDComptCriteria item = criteriaData.Single(e => e.standardNo == iStand);
            if (ans == 1)
            {
                i = Convert.ToInt16(item.basic);
            }
            else if (ans == 2)
            {
                i = Convert.ToInt16(item.intermediate);
            }
            else if (ans == 3)
            {
                i = Convert.ToInt16(item.advanced);
            }
            else if (ans == 4)
            {
                i = Convert.ToInt16(item.superior);
            }
            return i;
        }

        private int ComptAssmntLevel(IEnumerable<tLNDComptCriteria> criteriaData, int iStand, decimal scoreAve)
        {
            int score = Convert.ToInt16(scoreAve);
            int result = 0;

            tLNDComptCriteria item = criteriaData.Single(e => e.standardNo == iStand);

            result = Convert.ToInt16(item.basic);

            if (score >= Convert.ToInt16(item.intermediate))
            {
                result = Convert.ToInt16(item.intermediate);
            }
            if (score >= Convert.ToInt16(item.advanced))
            {
                result = Convert.ToInt16(item.advanced);
            }
            if (score >= Convert.ToInt16(item.superior))
            {
                result = Convert.ToInt16(item.superior);
            }

            return result;
        }




    }
}