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
    [Authorize]
    public class ComptAssessmentController : Controller
    {
        // GET: ComptAssessment

        HRISDBEntities db = new HRISDBEntities();

        public ActionResult Self()
        {
            return View();
        }

        public JsonResult RespondentAssessmentList()
        {
            string uEIC = Session["_EIC"].ToString();

            DateTime limitDate = DateTime.Now.AddMonths(-10);

            IEnumerable<vLNDComptRespondentApplicant> assmntList = db.vLNDComptRespondentApplicants.Where(e => e.EIC == uEIC).OrderBy(o => o.recNo).ToList();
            return Json(new { status = "success", list = assmntList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult retrieveAssList()
        {
            try
            {
                string agc = "ASSGRP21012009201APPLCNT";
                var assmntList = db.tLNDComptAssessments.Where(e => e.assmntGroupCode == agc).Select(a => new
                {
                    a.assessmentCode,
                    a.assessmentName,
                    a.assessmentDetail,
                    a.assessmentType
                });

                IEnumerable<vRSPEmployee> deptEmpList = db.vRSPEmployees.Where(e => e.fullNameLast != null).OrderBy(o => o.fullNameLast).ToList();
                IEnumerable<vRSPEmployee> pg = deptEmpList.Where(n => n.salaryGrade >= 22).ToList();

                List<vRSPEmployee> pgList = new List<vRSPEmployee>();

                foreach (vRSPEmployee itm in pg)
                {
                    pgList.Add(new vRSPEmployee()
                    {
                        EIC = itm.EIC,
                        fullNameLast = itm.fullNameLast,
                        positionTitle = itm.positionTitle
                    });
                }

                var supList = deptEmpList.Where(n => n.salaryGrade >= 15).ToList().Select(a => new
                {
                    a.EIC,
                    a.fullNameLast
                }).OrderBy(o => o.fullNameLast).ToList();

                return Json(new { status = "success", list = assmntList, empList = deptEmpList, supList = supList, deptList = pgList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { status = "Error retrieving data!" }, JsonRequestBehavior.AllowGet);
            }

        }

        //CHECK IF COMPT. ASSESSMENT SURVEY EXIST
        //IF EXIST LOAD STANDARD
        public ActionResult ShowCompetencyByCategory(string id)
        {
            int assmntYear = 0;

            string uEIC = Session["_EIC"].ToString();

            vLNDComptRespondentApplicant respondent = db.vLNDComptRespondentApplicants.SingleOrDefault(e => e.respondentCode == id);

            if (respondent == null)
            {
                return Json(new { status = "none" }, JsonRequestBehavior.AllowGet);
            }
            tLNDComptAssessment assmnt = db.tLNDComptAssessments.Single(e => e.assessmentCode == respondent.assessmentCode);

            IEnumerable<vLNDCompetencyStandard> comptList = db.vLNDCompetencyStandards.Where(e => e.comptPositionCode == assmnt.comptPositionCode).OrderBy(o => o.orderNo).ToList();

            //GET RESPONSE List
            IEnumerable<tLNDComptRespondentAn> responseList = db.tLNDComptRespondentAns.Where(e => e.respondentCode == respondent.respondentCode);
            //TOOL Listed
            IEnumerable<vLNDToolList> toolListed = db.vLNDToolLists.Where(e => e.assessmentYear == assmntYear).OrderBy(o => o.orderNo).ToList();
            //TOOL Progressive
            IEnumerable<vLNDToolProgressive> toolProg = db.vLNDToolProgressives.Where(e => e.assessmentYear == assmntYear).OrderBy(o => o.orderNo).ToList();

            List<LNDCompetency> myList = new List<LNDCompetency>();

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
                                tLNDComptRespondentAn resp = responseList.SingleOrDefault(e => e.respondentCode == respondent.respondentCode && e.KIBCode == itm.KBICode);
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
                            var cod = responseList.SingleOrDefault(e => e.respondentCode == respondent.respondentCode && e.KIBCode == itm.KBICode).answerSA;
                            if (cod >= 1)
                            {
                                tLNDComptRespondentAn resp = responseList.SingleOrDefault(e => e.respondentCode == respondent.respondentCode && e.KIBCode == itm.KBICode);
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

            var psychoTest = db.vLNDPsychoSocialTests.Where(e => e.respondentCode == respondent.respondentCode);

            return Json(new { status = "success", respondentCode = respondent.respondentCode, respondent = respondent, core = coreList, lead = leadList, tech = techList, psychoTest = psychoTest }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SubmitAnswer(string respondentCode, string KBICode, int ans)
        {
            string res = "";
            try
            {
                tLNDComptRespondentAn subAns = db.tLNDComptRespondentAns.SingleOrDefault(e => e.respondentCode == respondentCode && e.KIBCode == KBICode);
                if (subAns != null)
                {
                    subAns.answerSA = ans;
                    db.SaveChanges();
                    res = "success";
                }
                else
                {
                    tLNDComptRespondentAn selfAss = new tLNDComptRespondentAn();
                    res = "Error!";
                }
                return Json(new { status = res }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }
        //SubmitPsychoAnswer
        public JsonResult SubmitPsychoAnswer(string respondentCode, string psychoCode, int ans)
        {
            string res = "";
            try
            {
                tLNDPsychoSocialTest psyAns = db.tLNDPsychoSocialTests.SingleOrDefault(e => e.respondentCode == respondentCode && e.psychoCode == psychoCode);
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
        /// <summary>
        /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// SAVE RESPONDENT
        /// 


        public ActionResult List()
        {
            return View();
        }

        [HttpPost]
        public JsonResult AssessmentData()
        {
            string uEIC = Session["_EIC"].ToString();
            IEnumerable<tLNDComptAssessment> list = db.tLNDComptAssessments.Where(e => e.assessmentType == 1 && e.userEIC == uEIC).OrderByDescending(o => o.recNo).ToList();
            return Json(new { status = "success", list = list }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ComptAssmntPreData()
        {
            var applicant = db.tApplicants.Select(e => new
            {
                e.applicantCode,
                e.fullNameLast
            }).OrderBy(o => o.fullNameLast).ToList();

            var employee = db.vRSPEmployeeLists.Select(e => new
            {
                e.EIC,
                e.fullNameLast,
                e.salaryGrade
            }).OrderBy(o => o.fullNameLast).ToList();

            var supervisor = employee.Select(e => new
            {
                e.EIC,
                e.fullNameLast,
                e.salaryGrade
            }).Where(e => e.salaryGrade >= 12).ToList();


            return Json(new { status = "success", employee = employee, applicant = applicant, supervisor = supervisor }, JsonRequestBehavior.AllowGet);
        }


        //[HttpPost]
        //public JsonResult AssessmentEmployeeList()
        //{
        //    IEnumerable<tApplicant> applicantList = db.tApplicants.Where(e => e.isVerified == true).OrderBy(o => o.fullNameFirst).ToList();
        //    List<vRSPEmployee> empList = new List<vRSPEmployee>();
        //    foreach (tApplicant item in applicantList)
        //    {
        //        empList.Add(new vRSPEmployee()
        //        {
        //            EIC = item.applicantCode,
        //            fullNameLast = item.fullNameLast,
        //            fullNameFirst = item.fullNameFirst,
        //            positionTitle = "Applicant"
        //        });
        //    }
        //    return Json(new { status = "success", employeeList = empList }, JsonRequestBehavior.AllowGet);
        //}


        [HttpPost]
        public JsonResult GetDepartmentList()
        {
            try
            {
                var dept = db.tOrgDepartments.Select(e => new
                {
                    e.departmentCode,
                    e.shortDepartmentName,
                    e.orderNo
                }).OrderBy(o => o.orderNo).ToList();


                return Json(new { status = "success", department = dept }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult GetComptPositionById(string id)
        {
            try
            {
                var pos = db.tLNDCompetencyPositionGroups.Select(e => new
                {
                    e.comptPositionCode,
                    e.comptPositionTitle,
                    e.salaryGrade,
                    e.assmntGroupCode,
                    e.tag
                }).Where(e => e.tag == 1 && e.assmntGroupCode == id).OrderByDescending(o => o.salaryGrade).ThenBy(o => o.comptPositionTitle).ToList();
                return Json(new { status = "success", comptPosition = pos }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult SaveComptAssessment(tLNDComptAssessment data)
        {
            string uEIC = Session["_EIC"].ToString();
            string code = "ASSMNT" + DateTime.Now.ToString("yyMMddHHmmssfff") + uEIC.Substring(0, 4);
            tLNDComptAssessment a = new tLNDComptAssessment();
            a.assessmentCode = code;
            a.assessmentName = data.assessmentName;
            a.assessmentDetail = data.assessmentDetail;
            a.comptPositionCode = data.comptPositionCode;
            a.assmntGroupCode = data.assmntGroupCode;
            a.assessmentType = 1;
            a.tag = 0;
            a.transDT = DateTime.Now;
            a.userEIC = uEIC;
            db.tLNDComptAssessments.Add(a);
            db.SaveChanges();

            IEnumerable<tLNDComptAssessment> list = db.tLNDComptAssessments.Where(e => e.assessmentType == 1 && e.userEIC == uEIC).OrderByDescending(o => o.recNo).ToList();
            return Json(new { status = "success", list = list, code = code }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ViewRespondentListByCode(string id)
        {
            IEnumerable<vLNDComptRespondentApplicant> respondent = db.vLNDComptRespondentApplicants.Where(e => e.assessmentCode == id && e.tag >= 0).OrderBy(o => o.fullNameLast).ToList();
            return Json(new { status = "success", respList = respondent }, JsonRequestBehavior.AllowGet);
        }

        public class RespondentData
        {
            public string EIC { get; set; }
            public string assessmentCode { get; set; }
            public string comptPositionCode { get; set; }
            public string supervisorEIC { get; set; }
            public string deptHeadEIC { get; set; }
            public int psychoTestTag { get; set; }
            public int tag { get; set; }
        }

        //[HttpPost]
        //public JsonResult AssessmentSupervisor()
        //{

        //    var supervisors = db.vRSPEmployeeLists.Select(e => new
        //    {
        //        e.EIC,
        //        e.fullNameLast,
        //        e.salaryGrade
        //    }).Where(e => e.salaryGrade >= 12).OrderBy(o => o.fullNameLast).ToList();

        //    return Json(new { status = "success", supervisors = supervisors }, JsonRequestBehavior.AllowGet);

        //}


        [HttpPost]
        public JsonResult ViewMyData(string id, int tag)
        {

            try
            {
                tLNDComptRespondentApplicant assmnt = db.tLNDComptRespondentApplicants.Single(e => e.respondentCode == id);
                var sup = db.tLNDComptRespondentApplicants.Select(e => new
                {
                    e.respondentCode,
                    e.supervisorEIC
                }).Single(e => e.respondentCode == id);

                if (tag == 0)
                {
                    var supervisors = db.vRSPEmployeeLists.Select(e => new
                    {
                        e.EIC,
                        e.fullNameLast,
                        e.salaryGrade
                    }).Where(e => e.salaryGrade >= 12).OrderBy(o => o.fullNameLast).ToList();
                    return Json(new { status = "success", supervisors = supervisors, supervisorEIC = sup.supervisorEIC }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = "success", supervisorEIC = sup.supervisorEIC }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public JsonResult UpdateSupervisor(tLNDComptRespondentApplicant data)
        {
            try
            {
                tLNDComptRespondentApplicant assmnt = db.tLNDComptRespondentApplicants.Single(e => e.respondentCode == data.respondentCode);
                if (assmnt.tag <= 2)
                {
                    assmnt.supervisorEIC = data.supervisorEIC;
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


        public JsonResult SaveRespondent(tLNDComptRespondentApplicant data)
        {
            string uEIC = Session["_EIC"].ToString();
            //CHECK IF EIC IS ALREADY EXIST
            tLNDComptRespondent checker = db.tLNDComptRespondents.SingleOrDefault(e => e.EIC == data.EIC && e.assessmentCode == data.assessmentCode);
            if (checker != null)
            {
                return Json(new { status = "Respondent already exists." }, JsonRequestBehavior.AllowGet);
            }

            tLNDComptAssessment assmnt = db.tLNDComptAssessments.Single(e => e.assessmentCode == data.assessmentCode);

            IEnumerable<tLNDCompetencyStandard> standardList = db.tLNDCompetencyStandards.Where(e => e.comptPositionCode == assmnt.comptPositionCode);

            int count = standardList.ToList().Count();
            if (count == 0)
            {
                return Json(new { status = "Position Standard not yet profiled!" }, JsonRequestBehavior.AllowGet);
            }

            int hasUnAns = standardList.Where(e => e.standardCode == null).Count();

            if (hasUnAns >= 1)
            {
                return Json(new { status = "Please set standard in all competency!" }, JsonRequestBehavior.AllowGet);
            }

            string tmpCode = "RSPND" + DateTime.Now.ToString("yyMMddHHmmssfff") + uEIC.Substring(0, 6);
            tLNDComptRespondentApplicant r = new tLNDComptRespondentApplicant();
            r.respondentCode = tmpCode;
            r.assessmentCode = data.assessmentCode;
            r.EIC = data.EIC;
            r.applicationCode = "";
            r.supervisorEIC = data.supervisorEIC;
            r.tag = 0;
            r.transDT = DateTime.Now;
            r.userEIC = uEIC;
            db.tLNDComptRespondentApplicants.Add(r);
            db.SaveChanges();

            IEnumerable<vLNDComptRespondentApplicant> respondent = db.vLNDComptRespondentApplicants.Where(e => e.assessmentCode == data.assessmentCode).ToList();
            return Json(new { status = "success", respList = respondent }, JsonRequestBehavior.AllowGet);

            //try
            //{

            //    string tmpCode = "RSPND" + DateTime.Now.ToString("yyMMddHHmmssfff") + uEIC.Substring(0, 6);
            //    string respondentCode = tmpCode;
            //    tLNDComptRespondent resp = new tLNDComptRespondent();
            //    resp.respondentCode = respondentCode;
            //    resp.assessmentCode = data.assessmentCode;
            //    resp.comptPositionCode = data.comptPositionCode;
            //    resp.EIC = data.EIC;
            //    resp.supervisorEIC = data.supervisorEIC;
            //    resp.deptHeadEIC = data.deptHeadEIC;
            //    resp.tag = 0;

            //    //COMPETENCY LIST
            //    IEnumerable<vLNDCompetencyStandard> comptList = db.vLNDCompetencyStandards.Where(e => e.comptPositionCode == data.comptPositionCode).OrderBy(o => o.orderNo).ToList();
            //    //COMPETENCY TOOL LISTED                
            //    IEnumerable<vLNDToolList> toolListed = db.vLNDToolLists.Where(e => e.assessmentYear == 2020).OrderBy(o => o.orderNo).ToList();
            //    //COMPETENCY TOOL Progressive
            //    IEnumerable<vLNDToolProgressive> toolProg = db.vLNDToolProgressives.Where(e => e.assessmentYear == 2020).OrderBy(o => o.orderNo).ToList();

            //    List<LNDCompetency> myList = new List<LNDCompetency>();

            //    foreach (vLNDCompetencyStandard item in comptList)
            //    {
            //        if (item.comptTool == "3") //LISTED
            //        {
            //            //LISTED HEADER
            //            IEnumerable<vLNDToolList> header = toolListed.Where(e => e.comptCode == item.comptCode && e.MKBICode == null).OrderBy(o => o.orderNo).ToList();
            //            foreach (vLNDToolList h in header)
            //            {
            //                //LISTED INDICATOR
            //                IEnumerable<vLNDToolList> listed = toolListed.Where(e => e.MKBICode == h.KBICode).ToList();
            //                foreach (vLNDToolList itm in listed)
            //                {
            //                    tLNDComptRespondentAn a = new tLNDComptRespondentAn();
            //                    a.comptCode = item.comptCode;
            //                    a.respondentCode = respondentCode;
            //                    a.KIBCode = itm.KBICode;
            //                    db.tLNDComptRespondentAns.Add(a);
            //                }
            //            }
            //        }
            //        else //PROGRESSIVE
            //        {
            //            IEnumerable<vLNDToolProgressive> p = toolProg.Where(e => e.comptCode == item.comptCode).OrderBy(o => o.orderNo).ToList();
            //            foreach (vLNDToolProgressive itm in p)
            //            {
            //                tLNDComptRespondentAn a = new tLNDComptRespondentAn();
            //                a.comptCode = item.comptCode;
            //                a.respondentCode = respondentCode;
            //                a.KIBCode = itm.KBICode;
            //                db.tLNDComptRespondentAns.Add(a);
            //            }
            //        }
            //    }

            //    //ADD PSCHO TEST

            //    int groupTag = 2;
            //    if (data.psychoTestTag != 2)
            //    {
            //        groupTag = 1;
            //    }

            //    IEnumerable<tLNDPsychoSocial> psychoTest = db.tLNDPsychoSocials.Where(e => e.groupTag == groupTag).ToList();

            //    foreach (tLNDPsychoSocial item in psychoTest)
            //    {
            //        tLNDPsychoSocialTest ans = new tLNDPsychoSocialTest();
            //        ans.respondentCode = respondentCode;
            //        ans.psychoCode = item.psychoCode;
            //        db.tLNDPsychoSocialTests.Add(ans);
            //    }

            //    db.tLNDComptRespondents.Add(resp);
            //    db.SaveChanges();

            //    IEnumerable<vLNDComptRespondent> list = RespondentList(data.assessmentCode);
            //    return Json(new { status = "success", respList = list }, JsonRequestBehavior.AllowGet);

            //}
            //catch (Exception ex)
            //{
            //    return Json(new { status = "Error saving resondent..." }, JsonRequestBehavior.AllowGet);
            //}

            // 1. Check if EIC exsist
            // 2. Check if comptPosition is available
            // 3. if Yes save using storeProce
            //return Json(new { status = "Connection error!" }, JsonRequestBehavior.AllowGet);
        }



        public IEnumerable<vLNDComptRespondent> RespondentList(string code)
        {
            List<vLNDComptRespondent> myList = new List<vLNDComptRespondent>();
            IEnumerable<vLNDComptRespondent> list = db.vLNDComptRespondents.Where(e => e.assessmentCode == code).OrderBy(e => e.fullNameLast).ToList();
            foreach (vLNDComptRespondent item in list)
            {
                string tempRemarks = "";
                if (item.respondentTag == 0)
                {
                    tempRemarks = "for Assessment";
                }
                else if (item.respondentTag == 1)
                {
                    tempRemarks = "for Supervisor's Validation";
                }
                else if (item.respondentTag == 2)
                {
                    tempRemarks = "for Department Head validation";
                }
                else if (item.respondentTag == 5)
                {
                    tempRemarks = "Assessment completed...";
                }
                else if (item.respondentTag == 10)
                {
                    tempRemarks = "Assessment completed.";
                }
                myList.Add(new vLNDComptRespondent()
                {
                    respondentCode = item.respondentCode,
                    assessmentCode = item.assessmentCode,
                    assessmentName = item.assessmentName,
                    fullNameLast = item.fullNameLast,
                    positionTitle = item.positionTitle,
                    supervisorEIC = item.supervisorEIC,
                    supervisorName = item.supervisorName,
                    deptHeadEIC = item.deptHeadEIC,
                    PGHeadName = item.PGHeadName,
                    remarks = tempRemarks,
                    respondentTag = item.respondentTag
                });
            }
            return myList;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public JsonResult SubmitSelftComptAssmnt(string id)
        {
            //tLNDComptRespondent respnd = db.tLNDComptRespondents.SingleOrDefault(e => e.respondentCode == id);
            tLNDComptRespondentApplicant respnd = db.tLNDComptRespondentApplicants.SingleOrDefault(e => e.respondentCode == id);
            //check if all KBI answered
            IEnumerable<tLNDComptRespondentAn> ans = db.tLNDComptRespondentAns.Where(e => e.respondentCode == respnd.respondentCode && e.answerSA == null);

            int count = ans.Count();
            if (count >= 1)
            {
                string errMsg = "There is 1 item unanswered in Competency Test!";
                if (count > 1)
                {
                    errMsg = "There are " + count + " items unanswered in Competency Test!";
                }

                return Json(new { status = errMsg }, JsonRequestBehavior.AllowGet);
            }

            count = db.vLNDPsychoSocialTests.Where(e => e.respondentCode == respnd.respondentCode && e.answer == null).Count();
            if (count >= 1)
            {
                string errMsg = "There is 1 item unanswered in Psycho-Social Test";
                if (count > 1)
                {
                    errMsg = "There are " + count + " items unanswered in Psycho-Social Test!";
                }

                return Json(new { status = errMsg }, JsonRequestBehavior.AllowGet);
            }

            if (respnd != null)
            {
                if (respnd.supervisorEIC != null)
                {
                    respnd.tag = 2;
                }
                else if (respnd.supervisorEIC == null && respnd.supervisorEIC == null)
                {
                    respnd.tag = 4;
                }

                db.SaveChanges(); //comit changes
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "Invalid transaction!" }, JsonRequestBehavior.AllowGet);
            }
        }


        private List<tLNDComptRespondentResult> AssessmentResult(string respnCode)
        {

            vLNDComptRespondent respondent = db.vLNDComptRespondents.Single(e => e.respondentCode == respnCode);
            //   string tmpComptCode = "PCXAPP00001";
            string tmpComptCode = "COMPTPOS202002061111NURSE01A";

            int asmntLevelNo = 1;
            if (respondent.supervisorEIC != null)
            {
                asmntLevelNo = 2;
            }

            IEnumerable<vLNDCompetencyStandard> comptStatndard = db.vLNDCompetencyStandards.Where(e => e.comptPositionCode == tmpComptCode).ToList();
            IEnumerable<tLNDComptRespondentAn> responseList = db.tLNDComptRespondentAns.Where(e => e.respondentCode == respondent.respondentCode).ToList();

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
                    respondentCode = respondent.respondentCode,
                    comptCode = item.comptCode,
                    scoreSA = myScoreSA,
                    scoreAve = scoreAve,
                    scoreEqv = ComptAssmntLevel(criteriaList, Convert.ToInt16(item.standardCode), scoreAve)
                });

            }


            return listItem;

        }


        //private decimal PsychoScore(string respondentCode)
        //{
        //    IEnumerable<tLNDPsychoSocialTest> list = db.tLNDPsychoSocialTests.Where(e => e.respondentCode == respondentCode).ToList();
        //    decimal result = 0;
        //    int totalAnswer = 0;
        //    int totalCounter = 0;
        //    foreach (tLNDPsychoSocialTest item in list)
        //    {
        //        totalCounter = totalCounter + 1;
        //        totalAnswer = totalAnswer + Convert.ToInt16(item.answer);
        //    }
        //    result = (totalAnswer / totalCounter) * 2;
        //    return result;
        //}


        //private int ComptAssmntCriteria(IEnumerable<tLNDComptCriteria> criteriaData, int iStand, int ans)
        //{
        //    int i = 0;

        //    tLNDComptCriteria item = criteriaData.Single(e => e.standardNo == iStand);
        //    if (ans == 1)
        //    {
        //        i = Convert.ToInt16(item.basic);
        //    }
        //    else if (ans == 2)
        //    {
        //        i = Convert.ToInt16(item.intermediate);
        //    }
        //    else if (ans == 3)
        //    {
        //        i = Convert.ToInt16(item.advanced);
        //    }
        //    else if (ans == 4)
        //    {
        //        i = Convert.ToInt16(item.superior);
        //    }
        //    return i;
        //}

        //private int ComptAssmntLevel(IEnumerable<tLNDComptCriteria> criteriaData, int iStand, decimal scoreAve)
        //{
        //    int score = Convert.ToInt16(scoreAve);
        //    int result = 0;

        //    tLNDComptCriteria item = criteriaData.Single(e => e.standardNo == iStand);

        //    result = Convert.ToInt16(item.basic);

        //    if (score >= Convert.ToInt16(item.intermediate))
        //    {
        //        result = Convert.ToInt16(item.intermediate);
        //    }
        //    if (score >= Convert.ToInt16(item.advanced))
        //    {
        //        result = Convert.ToInt16(item.advanced);
        //    }
        //    if (score >= Convert.ToInt16(item.superior))
        //    {
        //        result = Convert.ToInt16(item.superior);
        //    }

        //    return result;
        //}



        [HttpPost]
        public JsonResult ComputeRespondentResult(string id)
        {
            try
            {
                tLNDComptRespondentApplicant comptAssmnt = db.tLNDComptRespondentApplicants.Single(e => e.respondentCode == id);
                List<tLNDComptRespondentResult> result = AssessmentResult(comptAssmnt);

                decimal rating = 0;
                decimal rateCounter = 0;


                decimal ratingEqv = 0;

                //COMPETENCY ASSESSMENT RESULT
                //List<tLNDComptRespondentResult> result = AssessmentResult(comptAssmnt);
                if (result.Count() > 0)
                {
                    foreach (tLNDComptRespondentResult item in result)
                    {
                        rateCounter = rateCounter + 1;
                        tLNDComptRespondentResult n = new tLNDComptRespondentResult();
                        n.respondentCode = item.respondentCode;
                        n.standard = item.standard;
                        n.comptCode = item.comptCode;
                        n.scoreSA = item.scoreSA;
                        n.scoreDS = item.scoreDS;
                        n.scoreAve = item.scoreAve;
                        n.scoreEqv = item.scoreEqv;
                        db.tLNDComptRespondentResults.Add(n);
                        rating = rating + Convert.ToDecimal(item.scoreAve);
                        ratingEqv = ratingEqv + Convert.ToDecimal(item.scoreEqv);
                    }
                    rating = rating / rateCounter;
                    ratingEqv = ratingEqv / rateCounter;
                }

                //PSYCHO-SOCIAL TEST RESULT
                decimal pshcoTestResult = _GetPsychoScore(comptAssmnt.applicationCode);

                tRSPApplicationComptAssmnt psy = new tRSPApplicationComptAssmnt();
                psy.applicationCode = comptAssmnt.applicationCode;
                psy.assmntType = 1;
                psy.score = Convert.ToInt16(pshcoTestResult);
                //psy.transDT = DateTime.Now;
                db.tRSPApplicationComptAssmnts.Add(psy);

                comptAssmnt.tag = 5; //assessment done!
                comptAssmnt.rating = ratingEqv;

                db.SaveChanges(); //comit changes
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "Invalid transaction!" }, JsonRequestBehavior.AllowGet);
            }
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public class TempComptRespondentResult
        {
            public string respondentCode { get; set; }
            public string comptCode { get; set; }
            public int standard { get; set; }

        }

        private List<tLNDComptRespondentResult> AssessmentResult(tLNDComptRespondentApplicant comptAssmnt)
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

            string id = "";

            try
            {
                foreach (vLNDCompetencyStandard item in comptStatndard)
                {

                    id = item.comptCode;
                    
                    IEnumerable<tLNDComptRespondentAn> respnsList = responseList.Where(e => e.comptCode == item.comptCode).ToList();

                    if (respnsList.Count() >= 1)
                    {
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

                        decimal aveSA = myScoreSA / levelCounter;
                        decimal aveDS = 0;

                        decimal scoreAve = 0;

                        if (asmntLevelNo == 1)
                        {
                            scoreAve = aveSA;
                        }
                        else if (asmntLevelNo == 2)
                        {
                            aveDS = myScoreDS / levelCounter;
                            scoreAve = (aveSA + aveDS) / 2;
                        }

                        listItem.Add(new tLNDComptRespondentResult()
                        {
                            respondentCode = comptAssmnt.applicationCode,
                            comptCode = item.comptCode,
                            standard = Convert.ToInt16(item.standardCode),
                            scoreSA = aveSA,
                            scoreDS = aveDS,
                            scoreAve = scoreAve,
                            scoreEqv = ComptAssmntLevel(criteriaList, Convert.ToInt16(item.standardCode), scoreAve)
                        });
                    }
                    else
                    {
                        string rer = "";
                    }


                }

            }
            catch (Exception)
            {

                string s = id;
            }


            return listItem;
        }


        private decimal _GetPsychoScore(string respondentCode)
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

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // SELF ASSESSMENT
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public ActionResult SelfAssessment()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetMyAssessmentList()
        {
            try
            {
                string uEIC = Session["_EIC"].ToString();
                var assmnt = db.vLNDComptRespondentApplicants.Select(e => new
                {
                    e.EIC,
                    e.respondentCode,
                    e.assessmentName,
                    e.assessmentDetail,
                    //e.comptPositionCode,
                    e.tag,
                    e.applicantCode
                    //}).OrderByDescending(o => o.respondentCode).ToList();
                }).Where(e => e.EIC == uEIC && e.tag >= 0).ToList();

                return Json(new { status = "success", assmntList = assmnt }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }



        [HttpPost]
        public JsonResult CheckCompetencyAssessment(string id)
        {

            int assmntYear = 0;
            try
            {
                vLNDComptRespondentApplicant compt = db.vLNDComptRespondentApplicants.SingleOrDefault(e => e.respondentCode == id);

                //compt assessment tag
                // 0 - created
                // 1 - available for test
                // 2 - supervisor's validation
                // 3 - pg validation
                // 4 - submited for computation
                // 5 - done

                List<LNDCompetency> myList = new List<LNDCompetency>();

                if (compt.tag == 1 || compt.tag == 0)
                {
                    //CHECK APPLICATION TO GET PLANTILLA
                    vRSPApplication application = db.vRSPApplications.Single(e => e.applicationCode == compt.respondentCode);
                    //CHECK COMPETENCY MAPPING
                    tLNDCompetencyPositionMap map = db.tLNDCompetencyPositionMaps.SingleOrDefault(e => e.plantillaCode == application.plantillaCode && e.tag == 1);
                    //COMPETENCY LIST FOR THIS PLANTILLACODE
                    IEnumerable<vLNDCompetencyStandard> comptList = db.vLNDCompetencyStandards.Where(e => e.comptPositionCode == map.comptPositionCode).OrderBy(o => o.orderNo).ToList();

                    //GET RESPONSE/ANSWER (check of previous answers)
                    IEnumerable<tLNDComptRespondentAn> responseList = db.tLNDComptRespondentAns.Where(e => e.respondentCode == application.applicationCode).ToList();

                    if (map != null)
                    {
                        IEnumerable<vLNDToolList> toolListed = db.vLNDToolLists.Where(e => e.assessmentYear == assmntYear).OrderBy(o => o.orderNo).ToList();
                        //TOOL Progressive
                        IEnumerable<vLNDToolProgressive> toolProg = db.vLNDToolProgressives.Where(e => e.assessmentYear == assmntYear).OrderBy(o => o.orderNo).ToList();

                        int i = 0;
                        int x = 0;
                        int y = 0;
                        foreach (vLNDCompetencyStandard item in comptList)
                        {
                            x++;
                            List<LNDIndicators> ndList = new List<LNDIndicators>();

                            if (item.comptTool == "3")
                            {
                                IEnumerable<vLNDToolList> header = toolListed.Where(e => e.comptCode == item.comptCode && e.MKBICode == null).OrderBy(o => o.orderNo).ToList();
                                foreach (vLNDToolList h in header)
                                {
                                    y = 1;
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
                                        y = y + 1;
                                        // ADD TO LIST
                                        ndList.Add(new LNDIndicators()
                                        {
                                            itemNo = y,
                                            header = h.KBI,
                                            KBICode = itm.KBICode,
                                            KBI = itm.KBI,
                                            answerSA = ansSA,
                                            answerDS = ansDS,
                                            orderNo = Convert.ToInt16(y),
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
                                    y = 1;
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
                                    y = y + 1;
                                    ndList.Add(new LNDIndicators()
                                    {
                                        itemNo = y,
                                        KBICode = itm.KBICode,
                                        KBI = "Progressive",
                                        comptLevel = itm.comptLevel,
                                        basic = itm.basic,
                                        intermediate = itm.intermediate,
                                        advance = itm.advance,
                                        superior = itm.superior,
                                        answerSA = ansSA,
                                        answerDS = ansDS,
                                        orderNo = Convert.ToInt16(y),
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
                                orderNo = Convert.ToInt16(x)
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
            catch (Exception ex)
            {

                string err = ex.ToString();
                return Json(new { status = "An error occurred!" }, JsonRequestBehavior.AllowGet);
            }
        }



        public JsonResult SubmitMyAnswer(string applicationCode, string KBICode, int ans)
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
        public JsonResult SubmitMyPsychoAnswer(string applicationCode, string psychoCode, int ans)
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
        //SUBMIT SELFT ASSESSMENT
        public JsonResult SubmitSelfAssessment(string id)
        {
            vRSPApplication application = db.vRSPApplications.Single(e => e.applicationCode == id);
            tLNDComptRespondentApplicant comptAssmnt = db.tLNDComptRespondentApplicants.Single(e => e.respondentCode == id);

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
                int comptTag = 4; //If no validation required!
                if (comptAssmnt.supervisorEIC != null)
                {
                    comptTag = 2;
                }

                tRSPTransactionLog log = new tRSPTransactionLog();
                log.logCode = application.applicationCode;
                log.userEIC = application.applicantCode;
                log.logType = "COMPTASS";
                log.logDetails = "Submit competency assessment";
                log.logDT = DateTime.Now;
                db.tRSPTransactionLogs.Add(log);

                comptAssmnt.tag = comptTag; //assessment done! // NEXT IS COMPURATOIN => 5             
                db.SaveChanges(); //comit changes
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "Error submitting competency!" }, JsonRequestBehavior.AllowGet);
            }
        }



        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // ASSESSMENT VALIDATION
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public ActionResult Validation()
        {
            return View();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        [HttpPost]
        public JsonResult ValidatorsInitData()
        {
            string uEIC = Session["_EIC"].ToString();
            try
            {
                var assmnt = db.vLNDComptRespondentApplicants.Select(e => new
                {
                    e.EIC,
                    e.respondentCode,
                    e.fullNameLast,
                    e.assessmentName,
                    e.tag,
                    e.supervisorEIC
                }).Where(e => e.supervisorEIC == uEIC && e.tag == 2).OrderBy(o => o.fullNameLast).ToList();

                return Json(new { status = "success", assmntList = assmnt }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }



        }

        public JsonResult ValidateAssessment(string id, string coreType)
        {

            int assmntYear = 0;
            //compt assessment tag
            // 0 - created
            // 1 - available for test
            // 2 - supervisor's validation
            // 3 - pg validation
            // 4 - submited for computation
            // 5 - done

            vLNDComptRespondentApplicant respondent = db.vLNDComptRespondentApplicants.SingleOrDefault(e => e.respondentCode == id);

            vRSPApplication application = db.vRSPApplications.Single(e => e.applicationCode == respondent.respondentCode);
            //CHECK COMPETENCY MAPPING
            tLNDCompetencyPositionMap map = db.tLNDCompetencyPositionMaps.SingleOrDefault(e => e.plantillaCode == application.plantillaCode && e.tag == 1);
            //COMPETENCY LIST FOR THIS PLANTILLACODE
            IEnumerable<vLNDCompetencyStandard> comptList = db.vLNDCompetencyStandards.Where(e => e.comptPositionCode == map.comptPositionCode).OrderBy(o => o.orderNo).ToList();


            //GET Competency List
            //IEnumerable<vLNDCompetencyStandard> comptList = db.vLNDCompetencyStandards.Where(e => e.comptPositionCode == respondent.comptPositionCode).OrderBy(o => o.orderNo).ToList();
            //GET RESPONSE List
            IEnumerable<tLNDComptRespondentAn> responseList = db.tLNDComptRespondentAns.Where(e => e.respondentCode == respondent.respondentCode);
            //TOOL Listed
            IEnumerable<vLNDToolList> toolListed = db.vLNDToolLists.Where(e => e.assessmentYear == assmntYear).OrderBy(o => o.orderNo).ToList();
            //TOOL Progressive
            IEnumerable<vLNDToolProgressive> toolProg = db.vLNDToolProgressives.Where(e => e.assessmentYear == assmntYear).OrderBy(o => o.orderNo).ToList();

            List<LNDCompetency> myList = new List<LNDCompetency>();

            int i = 0;
            int x = 0;
            foreach (vLNDCompetencyStandard item in comptList)
            {
                i++;
                List<LNDIndicators> ndList = new List<LNDIndicators>();

                if (item.comptTool == "3")
                {
                    IEnumerable<vLNDToolList> header = toolListed.Where(e => e.comptCode == item.comptCode && e.MKBICode == null).OrderBy(o => o.orderNo).ToList();
                    foreach (vLNDToolList h in header)
                    {
                        // ADD TO LIST [HEADER]

                        IEnumerable<vLNDToolList> listed = toolListed.Where(e => e.MKBICode == h.KBICode).ToList();
                        foreach (vLNDToolList itm in listed)
                        {
                            int ansSA = 0;
                            int ansDS = 0;
                            int ansPG = 0;

                            if (responseList.Count() > 0)
                            {
                                tLNDComptRespondentAn resp = responseList.SingleOrDefault(e => e.respondentCode == respondent.respondentCode && e.KIBCode == itm.KBICode);
                                if (resp != null)
                                {
                                    ansSA = Convert.ToInt16(resp.answerSA);
                                    ansDS = Convert.ToInt16(resp.answerDS);
                                    ansPG = Convert.ToInt16(resp.answerPG);
                                }
                            }
                            // ADD TO LIST
                            ndList.Add(new LNDIndicators()
                            {
                                header = h.KBI,
                                KBICode = itm.KBICode,
                                KBI = itm.KBI,
                                standard = 0,
                                answerSA = ansSA,
                                answerDS = ansDS,
                                answerPG = ansPG,
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
                        int ansSA = 0;
                        int ansDS = 0;
                        int ansPG = 0;
                        if (responseList.Count() > 0)
                        {
                            tLNDComptRespondentAn resp = responseList.SingleOrDefault(e => e.respondentCode == respondent.respondentCode && e.KIBCode == itm.KBICode);
                            if (resp != null)
                            {
                                ansSA = Convert.ToInt16(resp.answerSA);
                                ansDS = Convert.ToInt16(resp.answerDS);
                                ansPG = Convert.ToInt16(resp.answerPG);
                            }
                        }
                        //add to list
                        i = i + 1;
                        ndList.Add(new LNDIndicators()
                        {
                            KBICode = itm.KBICode,
                            KBI = "Progressive",
                            comptLevel = itm.comptLevel,
                            basic = itm.basic,
                            intermediate = itm.intermediate,
                            advance = itm.advance,
                            superior = itm.superior,
                            standard = 0,
                            answerSA = ansSA,
                            answerDS = ansDS,
                            answerPG = ansPG,
                            orderNo = Convert.ToInt16(itm.orderNo),
                            tag = 1
                        });
                    }
                }

                x = x + 1;
                myList.Add(new LNDCompetency()
                {
                    comptCode = item.comptCode,
                    comptName = item.comptName,
                    comptDesc = item.comptDesc,
                    comptGroupCode = item.comptGroupCode,
                    comptTool = item.comptTool,
                    KBIList = ndList,
                    orderNo = x
                });
            }

            var coreList = myList.Where(e => e.comptGroupCode == "CORE").ToList();
            var leadList = myList.Where(e => e.comptGroupCode == "LEAD").ToList();
            var techList = myList.Where(e => e.comptGroupCode == "TECH").ToList();
            return Json(new { status = "success", respondentCode = respondent.respondentCode, respondent = respondent, core = coreList, lead = leadList, tech = techList }, JsonRequestBehavior.AllowGet);

        }
        //SUPERVISORS ASSESSMENT
        public JsonResult SubmitSupervisorAnswer(string respondentCode, string KBICode, int ans)
        {
            string res = "";
            tLNDComptRespondentAn subAns = db.tLNDComptRespondentAns.SingleOrDefault(e => e.respondentCode == respondentCode && e.KIBCode == KBICode);

            if (subAns != null)
            {
                subAns.answerDS = ans;
                db.SaveChanges();
                res = "success";
            }
            else
            {
                res = "Error!";
            }
            return Json(new { status = res }, JsonRequestBehavior.AllowGet);
        }


        //SubmitSupervisorValidation
        public JsonResult SubmitSupervisorValidation(string respondentCode)
        {
            tLNDComptRespondentApplicant respondent = db.tLNDComptRespondentApplicants.SingleOrDefault(e => e.respondentCode == respondentCode);
            int count = db.tLNDComptRespondentAns.Where(e => e.respondentCode == respondent.respondentCode && e.answerDS == null).Count();

            if (count >= 1)
            {
                return Json(new { status = "There are " + count + " item/(s) unanswered!" }, JsonRequestBehavior.AllowGet);
            }

            if (respondent != null)
            {
                tRSPTransactionLog log = new tRSPTransactionLog();
                log.logCode = respondent.applicationCode + "_S";
                log.userEIC = respondent.applicationCode;
                log.logType = "COMPTASS";
                log.logDT = DateTime.Now;
                log.logDetails = "Submit Competency Assessment Validation";
                db.tRSPTransactionLogs.Add(log);

                respondent.tag = 4; //FOR COMPURATION
                db.SaveChanges();   //comit changes
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "Invalid transaction!" }, JsonRequestBehavior.AllowGet);
            }
        }


        [Authorize(Roles = "HRDFocal")]
        public ActionResult Result()
        {

            return View();
        }


        public class TempCompetency
        {
            public string comptCode { get; set; }
            public string comptName { get; set; }
            public string comptGroup { get; set; }
        }


        [HttpPost]
        [Authorize(Roles = "HRDFocal")]
        public JsonResult GetCompetencyByGroup()
        {
            string uEIC = Session["_EIC"].ToString();
            tLNDHRDFocal focal = db.tLNDHRDFocals.SingleOrDefault(e => e.EIC == uEIC);

            if (focal != null)
            {
                IEnumerable<tLNDCompetency> comptList = db.tLNDCompetencies.Where(e => e.assessmentYear == 2020).OrderBy(o => o.orderNo).ToList();
                IEnumerable<tLNDCompetency> comptCoreLead = comptList.Where(e => e.assmntGroupCode == null && (e.comptGroupCode == "CORE" || e.comptGroupCode == "LEAD")).ToList();
                List<TempCompetency> myList = new List<TempCompetency>();

                foreach (tLNDCompetency item in comptCoreLead)
                {
                    myList.Add(new TempCompetency()
                    {
                        comptCode = item.comptCode,
                        comptName = item.comptName,
                        comptGroup = item.comptGroupCode
                    });
                }

                IEnumerable<tLNDCompetency> comptTech = comptList.Where(e => e.assmntGroupCode == focal.assmntGroupCode).ToList();
                foreach (tLNDCompetency item in comptTech)
                {
                    myList.Add(new TempCompetency()
                    {
                        comptCode = item.comptCode,
                        comptName = item.comptName,
                        comptGroup = item.comptGroupCode
                    });
                }

                return Json(new { status = "success", competency = myList }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);

        }




        [HttpPost]
        [Authorize(Roles = "HRDFocal")]
        public JsonResult GetComptResult(string id)
        {
            try
            {
                string uEIC = Session["_EIC"].ToString();
                tLNDHRDFocal focal = db.tLNDHRDFocals.SingleOrDefault(e => e.EIC == uEIC);
                IEnumerable<vLNDComptResult> res = db.vLNDComptResults.Where(e => e.comptCode == id && e.assmntGroupCode == focal.assmntGroupCode).ToList();
                return Json(new { status = "success", list = res }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                string msg = e.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        [Authorize(Roles = "HRDFocal")]
        public JsonResult PrintComptAssmntResult(string id)
        {
            string uEIC = Session["_EIC"].ToString();
            tLNDHRDFocal focal = db.tLNDHRDFocals.SingleOrDefault(e => e.EIC == uEIC);
            tLNDCompetency compt = db.tLNDCompetencies.Single(e => e.comptCode == id);


            //Session["ReportType"] = "PrintCoreCompetency";
            //Session["comptCode"] = comptCode;
            //Session["comptGroupCode"] = comptGroupCode;
            //Session["assmntGroupCode"] = assmntGroupCode;

            Session["ReportType"] = "COMPTASSMNTRESULT";
            //assmntGroup //comptGroup

            string tmpCode = focal.assmntGroupCode + ":" + id + ":" + compt.comptGroupCode;

            Session["PrintReport"] = tmpCode;


            return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);

        }


    }
}