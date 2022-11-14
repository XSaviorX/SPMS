using DDNHRIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DDNHRIS.Controllers
{

    [SessionTimeout]
    [Authorize]
    public class SurveyController : Controller
    {
        HRISDBEntities db = new HRISDBEntities();
        // GET: Survey
        public ActionResult Questionnaire()
        {
            return View();
        }

        public ActionResult Question()
        {
            return View();
        }


        public class TempSurvey
        {
            public string surveyId { get; set; }
            public string particulars { get; set; }
            public string details { get; set; }
            public string surveyIntro { get; set; }
            public DateTime dateFrom { get; set; }
            public DateTime dateTo { get; set; }
            public string respondentId { get; set; }
            public string EIC { get; set; }
            public int tag { get; set; }
        }

        [HttpPost]
        public JsonResult Questions()
        {
            try
            {

                string userId = Session["_EIC"].ToString();

                var res = (from r in db.tLNDSurveyRespondents
                           join s in db.tLNDSurveys on r.surveyId equals s.surveyId
                           where r.EIC == userId && r.tag == 1
                           select new { s.surveyId, s.particulars, s.details, s.surveyIntro, s.dateFrom, s.dateTo, r.respondentId, r.EIC, r.tag }).ToList();


                return Json(new { status = "success", surveys = res }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string e = ex.ToString();
                return Json(new { status = "Error loading data...!" }, JsonRequestBehavior.AllowGet);
            }
        }

        public class TempQuestion
        {
            public int questionNo { get; set; }
            public string questionId { get; set; }
            public string question { get; set; }
            public int responseCode { get; set; }
            public string answer { get; set; }
            public int hasSubQuest { get; set; }
            public string respondentId { get; set; }
            public List<TempQuestion> subQuestion { get; set; }
        }


        [HttpPost]
        public JsonResult SurveyQuestions(string id)
        {
            try
            {

                tLNDSurveyRespondent resp = db.tLNDSurveyRespondents.SingleOrDefault(e => e.respondentId == id);

                if (resp == null)
                {
                    return Json(new { status = "No survey available!" }, JsonRequestBehavior.AllowGet);
                }

                string surveyId = resp.surveyId;
                IEnumerable<tLNDSurveyQuestion> questions = db.tLNDSurveyQuestions.Where(e => e.surveyId == surveyId).OrderBy(o => o.orderNo).ToList();

                List<TempQuestion> myList = new List<TempQuestion>();

                int counter = 0;

                foreach (tLNDSurveyQuestion item in questions.Where(e => e.coreQuestId == null).ToList())
                {
                    counter = counter + 1;
                    IEnumerable<tLNDSurveyQuestion> subs = questions.Where(e => e.coreQuestId == item.questionId).ToList();

                    List<TempQuestion> mySubs = new List<TempQuestion>();
                    int hasSub = 0;
                    if (subs.Count() > 0)
                    {
                        foreach (tLNDSurveyQuestion subItm in subs)
                        {
                            mySubs.Add(new TempQuestion()
                            {
                                questionId = subItm.questionId,
                                question = subItm.question,
                                responseCode = Convert.ToInt16(subItm.reponseCode),
                                hasSubQuest = hasSub,
                                answer = ""
                            });
                        }
                        hasSub = 1;
                    }

                    myList.Add(new TempQuestion()
                    {
                        questionId = item.questionId,
                        questionNo = counter,
                        question = item.question,
                        responseCode = Convert.ToInt16(item.reponseCode),
                        hasSubQuest = hasSub,
                        subQuestion = mySubs,
                        respondentId = id,
                        answer = "false"
                    });
                }


                return Json(new { status = "success", questions = myList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string e = ex.ToString();
                return Json(new { status = "Error loading data...!" }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult SubmitSurveyResponse(List<TempQuestion> data, string id)
        {
            try
            {
                bool hasError = false;

                //data = data.Where(e => e.answer == "True").ToList();

                tLNDSurveyRespondent resp = db.tLNDSurveyRespondents.SingleOrDefault(e => e.respondentId == id && e.tag == 1);

                if (resp == null)
                {
                    return Json(new { status = "Survey not found!" }, JsonRequestBehavior.AllowGet);
                }

                string surveyId = resp.surveyId;

                IEnumerable<tLNDSurveyQuestion> questions = db.tLNDSurveyQuestions.Where(e => e.surveyId == surveyId).ToList();

                List<tLNDSurveyResponse> answer = new List<tLNDSurveyResponse>();

                foreach (TempQuestion item in data)
                {

                    List<TempQuestion> subs = item.subQuestion;

                    answer.Add(new tLNDSurveyResponse()
                    {
                        respondentId = id,
                        questionId = item.questionId,
                        answer = item.answer
                    });

                    if (item.answer == "True")
                    {

                        foreach (TempQuestion itm in subs)
                        {
                            if (itm.answer == null)
                            {
                                hasError = true;
                            }
                            else
                            {
                                answer.Add(new tLNDSurveyResponse()
                                {
                                    respondentId = id,
                                    questionId = itm.questionId,
                                    answer = itm.answer
                                });
                            }
                        }
                    }
                    
                }
                
                if (hasError == true)
                {
                    return Json(new { status = "Incomplete data!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    // 1 - int & double;  2 - text; 3 - date
                    foreach (tLNDSurveyResponse item in answer)
                    {

                        tLNDSurveyQuestion q = questions.Single(e => e.questionId == item.questionId);

                        if (q.reponseCode == 3) //date
                        {
                            DateTime dt = Convert.ToDateTime(item.answer);
                            item.answer = dt.ToString("MMMM dd, yyyy");
                        }

                        tLNDSurveyResponse n = new tLNDSurveyResponse();
                        n.questionId = item.questionId;
                        n.answer = item.answer;
                        n.respondentId = item.respondentId;
                        db.tLNDSurveyResponses.Add(n);
                    }

                    resp.tag = 0;
                    db.SaveChanges();
                    return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                string e = ex.ToString();
                return Json(new { status = "Error loading data...!" }, JsonRequestBehavior.AllowGet);
            }
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////
        // SURVEY REPORT

        public ActionResult SurveyReport()
        {

            return View();
        }

        [HttpPost]
        public JsonResult ReportInitData()
        {
            try
            {
                //DEPARTMENT LIST
                //SURVEY LIST

                var survey = (from s in db.tLNDSurveys
                              where s.tag == 1
                              select new { s.surveyId, s.particulars }).ToList();


                var dept = (from d in db.tOrgDepartments
                            where d.isActive == true
                            select new { d.departmentCode, d.shortDepartmentName, d.orderNo }).OrderBy(o => o.orderNo).ToList();

                return Json(new { status = "success", survey = survey, department = dept }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                string e = ex.ToString();
                return Json(new { status = "Error loading data...!" }, JsonRequestBehavior.AllowGet);
            }
        }



        public class RespondentList
        {
            public string EIC { get; set; }
            public string fullNameLast { get; set; }
            public string positionTitle { get; set; }
            public int salaryGrade { get; set; }
            public string trainingA { get; set; }
            public string trainingB { get; set; }
            public string trainingC { get; set; }
            public int levelNo { get; set; }
            public string remarks { get; set; }
        }


        public class ResponseList
        {
            public string questionId { get; set; }
            public string answer { get; set; }
            public string respondentId { get; set; }
            public string surveyId { get; set; }
        }


        [HttpPost]
        public JsonResult ViewReportById(string id, string code)
        {
            try
            {
                List<RespondentList> list = SurveyData(id, code);
                return Json(new { status = "success", list = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string e = ex.ToString();
                return Json(new { status = "Error loading data...!" }, JsonRequestBehavior.AllowGet);
            }
        }

        private List<RespondentList> SurveyData(string surveyId, string departmentCode)
        {

            List<RespondentList> myList = new List<RespondentList>();

            IEnumerable<vLNDSurveyRepondent> respndt = db.vLNDSurveyRepondents.Where(e => e.surveyId == surveyId && e.departmentCode == departmentCode).OrderBy(o => o.fullNameLast).ToList();
            IEnumerable<tLNDSurveyQuestion> quest = db.tLNDSurveyQuestions.Where(e => e.surveyId == surveyId).OrderBy(o => o.orderNo).ToList();
            List<ResponseList> response = (from rspn in db.tLNDSurveyResponses
                                           join resp in db.tLNDSurveyRespondents on rspn.respondentId equals resp.respondentId
                                           where resp.surveyId == surveyId
                                           select new ResponseList { questionId = rspn.questionId, answer = rspn.answer, respondentId = resp.respondentId, surveyId = resp.surveyId }).ToList();

            foreach (vLNDSurveyRepondent item in respndt)
            {
                string remarks = "No Response";
                string BSDC = "-";
                string SDCT1 = "-";
                string SDCT2 = "-";
                int levelNo = -1;

                List<ResponseList> temp = response.Where(e => e.respondentId == item.respondentId).ToList();

                if (item.tag == 0)
                {
                    levelNo = 0;
                    BSDC = "x";
                    SDCT1 = "x";
                    SDCT2 = "x";
                    remarks = "";
                    //BSDC
                    ResponseList a = temp.SingleOrDefault(e => e.questionId == "QEST202109290933001");
                    if (a != null)
                    {
                        if (a.answer.ToUpper() == "TRUE")
                        {
                            BSDC = "YES";
                            levelNo = 1;
                        }
                    }

                    ResponseList b = temp.SingleOrDefault(e => e.questionId == "QEST202109290933004");
                    if (b != null)
                    {
                        if (b.answer.ToUpper() == "TRUE")
                        {
                            SDCT1 = "YES";
                            levelNo = 2;
                        }
                    }

                    ResponseList c = temp.SingleOrDefault(e => e.questionId == "QEST202109290933007");
                    if (c != null)
                    {
                        if (c.answer.ToUpper() == "TRUE")
                        {
                            SDCT2 = "YES";
                            levelNo = 3;
                        }
                    }
                }

                myList.Add(new RespondentList()
                {
                    EIC = item.EIC,
                    fullNameLast = item.fullNameLast,
                    positionTitle = item.positionTitle,
                    salaryGrade = Convert.ToInt16(item.salaryGrade),
                    trainingA = BSDC,
                    trainingB = SDCT1,
                    trainingC = SDCT2,
                    levelNo = levelNo,
                    remarks = remarks
                });
            }            
            return myList;
        }


        [HttpPost]
        public JsonResult PrintSurveyById(string id, string code)
        {


            tOrgDepartment org = db.tOrgDepartments.FirstOrDefault(e => e.departmentCode == code);

            string userId = Session["_EIC"].ToString();
            DateTime dt = DateTime.Now;
            string tempCode = "S" + dt.ToString("yyMMddHHmm") + "LDI" + userId.Substring(0,5) + dt.ToString("ssfff");
            List<RespondentList> list = SurveyData(id, code);
            foreach (RespondentList item in list)
            {
                tLNDSurveyReport rep = new tLNDSurveyReport();
                rep.fullNameLast = item.fullNameLast;
                rep.positionTitle = item.positionTitle;
                rep.BSDC = item.trainingA;
                rep.SDCT1 = item.trainingB;
                rep.SDCT2 = item.trainingC;
                rep.levelNo = item.levelNo;
                rep.remarks = item.remarks;
                rep.reportId = tempCode;
                db.tLNDSurveyReports.Add(rep);
            }
            db.SaveChanges();

            Session["ReportType"] = "SURVEYREP";
            Session["PrintReport"] = tempCode + "|" + org.departmentName.ToUpper();

            return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////


    }
}