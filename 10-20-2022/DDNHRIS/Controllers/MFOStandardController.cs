using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DDNHRIS.Models.SPMS;

namespace DDNHRIS.Controllers
{
    public class MFOStandardController : Controller
    {
        SPMSDBEntities7 _db = new SPMSDBEntities7();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string nums = "0123456789";
        Random random = new Random();
        string[] quan = new string[5];
        // GET: MFO

        /* ============================================ COMMON MFO ====================================================================================*/
        public ActionResult CommonMFO()
        {
            return View();
        }
        public ActionResult AddMFO()
        {
            return View();
        }

        /* ============================================ OFFICE ====================================================================================*/
        public ActionResult Office()
        {
            return View();
        }

        public ActionResult Index()
        {
            return Redirect("~/MFOStandard/Office");
        }

        [HttpPost]
        public ActionResult addClassification(String Classification, String _OfficeID)
        {
            string randomLetters = new string(Enumerable.Repeat(chars, 6)
            .Select(s => s[random.Next(s.Length)]).ToArray());
            string randomLNumbers = new string(Enumerable.Repeat(nums, 6)
             .Select(s => s[random.Next(s.Length)]).ToArray());

            var class_id = "CFN" + DateTime.Now.ToString("yyMMdd") + randomLetters.ToUpper() + randomLNumbers;

            var isExist = _db.tSPMS_Classification.Where(a => a.classification == Classification & a.officeId == _OfficeID).FirstOrDefault();

            var data = 0;
            if (isExist == null)
            {
                var addData = new tSPMS_Classification()
                {

                    classificationId = class_id,
                    classification = Classification,
                    officeId = _OfficeID,
                };

                _db.tSPMS_Classification.Add(addData);
                data = 1;
            }
            _db.SaveChanges();
            return Json(new { data, _OfficeID }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult updateClassification(tSPMS_Classification ClassData, String _OfficeID)
        {


            var update = _db.tSPMS_Classification.Where(a => a.classificationId == ClassData.classificationId & a.officeId == _OfficeID).FirstOrDefault();

            update.classification = ClassData.classification;
            _db.SaveChanges();
            return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MFO_get(string Office_ID)
        {
            var data = _db.vSPMS_nMFO_ALL.Where(a=> a.officeId == Office_ID & a.isOPCR == 1).ToList();
            //var data = _db.Database.SqlQuery<vSPMS_nMFO_ALL>("");

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult MFO_getPrograms(string _OfficeID)
        {
            var query = from offID in _db.tPrograms where offID.officeId == _OfficeID & offID.programTypeId == 0 select offID;
            var data = query.ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /*  [HttpPost]
          public ActionResult MFO_getProjects(string _ProgramID)
          {
              var query = (from MFOProjects in _db.appropprojectids
                           where MFOProjects.programId == _ProgramID & MFOProjects.budgetYear == 2022
                           select MFOProjects).ToList();
              //var data = query.ToList();
              return Json(query, JsonRequestBehavior.AllowGet);
          }*/
        [HttpPost]
        public ActionResult MFO_getoffices()
        {
            var data = _db.tOffices.ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MFO_getDivisions(string _offId)
        {
            var data = _db.tOfficeDivisions.Where(a=> a.officeId == _offId).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MFO_getClassifications(string _offId)
        {
            var data = _db.tSPMS_Classification.Where(a => a.officeId == _offId).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public ActionResult MFO_getAppropriateProjID(string _ProjID)
        //{
        //    var query = from MFOProjects in _db.tAppropriationProjs where MFOProjects.projectId == _ProjID & MFOProjects.budgetYear == 2022 select MFOProjects;
        //    var data = query.ToList();
        //    //return Json(new { appropProjID = data[0].appropProjectId}, JsonRequestBehavior.AllowGet);
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        public ActionResult MFO_insert(tAppropriationProjMFO _MFO, tSPMS_MFOCategory MFOCat)
        {
            string randomLetters = new string(Enumerable.Repeat(chars, 6)
        .Select(s => s[random.Next(s.Length)]).ToArray());
            string randomLNumbers = new string(Enumerable.Repeat(nums, 6)
        .Select(s => s[random.Next(s.Length)]).ToArray());

            var MFO_id = "MFO" + DateTime.Now.ToString("yyMMdd") + randomLetters.ToUpper() + randomLNumbers;

            var newMFOData = new tAppropriationProjMFO()
            {
                MFOId = MFO_id,
                MFO = _MFO.MFO,
                isActive = 0
            };
            _db.tAppropriationProjMFOes.Add(newMFOData);

            var addMFOCategory = new tSPMS_MFOCategory()
            {
                MFOId = MFO_id,
                categoryId = MFOCat.categoryId,
                officeId = MFOCat.officeId,
                year = MFOCat.year,
                semester = MFOCat.semester,
                classificationId = MFOCat.classificationId
            };
            _db.tSPMS_MFOCategory.Add(addMFOCategory);
            _db.SaveChanges();

            return Json(new { status = MFO_id }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MFO_SIinsert(vSPMS_nMFO_ALL indicators, string MFO_ID, tSPMS_MFOCategory officeId)
        {
            var data = "";
            var target = 0;

            target = int.Parse(indicators.target.ToString());
            var newMFO_SI_Data = new tAppropriationProjMFOInd()
            {
                indicatorId = indicators.indicatorId,
                MFOId = MFO_ID,
                indicator = indicators.indicator,
                target = target,
                targetUnit = indicators.targetUnit,
                targetTypeId = 0,
                isActive = 0
            };
            _db.tAppropriationProjMFOInds.Add(newMFO_SI_Data);

            string randomLetters = new string(Enumerable.Repeat(chars, 6)
        .Select(s => s[random.Next(s.Length)]).ToArray());
            string randomLNumbers = new string(Enumerable.Repeat(nums, 6)
        .Select(s => s[random.Next(s.Length)]).ToArray());

            var targetID = "TGT" + DateTime.Now.ToString("yyMMdd") + randomLetters.ToUpper() + randomLNumbers;

            var nTargetData = new tSPMS_TargetCountSI()
            {
                indicatorId = indicators.indicatorId,
                officeId = officeId.officeId,
                targetId = targetID,
                target = target,
                targetUnit = indicators.targetUnit,
                tRemaining = target,
                targetTypeId = 0,
                divisionId = indicators.divisionId,
                isShared = indicators.isShared,
                isOPCR = 1
            };
            _db.tSPMS_TargetCountSI.Add(nTargetData);


            _db.SaveChanges();
            data = targetID;


            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult addPerformances(List<tSPMS_MFOStandard> indicators, string MFO_ID)
        {
            var data = "";
            foreach (var ndata in indicators)
            {
                var newPerformanceData = new tSPMS_MFOStandard()
                {
                    indicatorId = ndata.indicatorId,
                    MFOId = MFO_ID,
                    rating = ndata.rating,
                    quantity = ndata.quantity,
                    quality = ndata.quality,
                    timeliness = ndata.timeliness,
                    targetId = ndata.targetId
                };
                _db.tSPMS_MFOStandard.Add(newPerformanceData);
                _db.SaveChanges();
            }

            data = "0";
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MFO_updateMFO_allSI(List<vSPMS_rOPCRStandard> _Pdata, vSPMS_MFOwCat MFOdata)
        {
            var text = "";
            foreach (var items in _Pdata)
            {
                if (items.indicator != null && items.indicator != "")
                {
                    string randomLetters = new string(Enumerable.Repeat(chars, 6)
             .Select(s => s[random.Next(s.Length)]).ToArray());
                    string randomLNumbers = new string(Enumerable.Repeat(nums, 6)
                     .Select(s => s[random.Next(s.Length)]).ToArray());

                    var target_id = "TGT" + DateTime.Now.ToString("yyMMdd") + randomLetters.ToUpper() + randomLNumbers;
                    if (items.target == 1)
                    {
                        quan[4] = "1";
                        quan[3] = "";
                        quan[2] = "";
                        quan[1] = "";
                        quan[0] = "0";
                    }
                    else
                    {
                        quan[4] = (((double)items.target * 0.3) + (double)items.target).ToString();
                        quan[3] = (((double)items.target * 0.15) + (double)items.target).ToString();
                        quan[2] = ((double)items.target).ToString();
                        quan[1] = (((double)items.target / 2) + 1).ToString();
                        quan[0] = ((double)(items.target / 2)).ToString();
                    }
                    var newMFO_SI_Data = new tAppropriationProjMFOInd()
                    {
                        indicatorId = items.indicatorId,
                        MFOId = MFOdata.MFOId,
                        indicator = items.indicator,
                        target = items.target,
                        targetUnit = items.targetUnit,
                        targetTypeId = 0,
                        isActive = 0
                    };
                    _db.tAppropriationProjMFOInds.Add(newMFO_SI_Data);

                    var newTargetData = new tSPMS_TargetCountSI()
                    {
                        indicatorId = items.indicatorId,
                        officeId = MFOdata.officeId,
                        target = items.target,
                        targetId = target_id,
                        tRemaining = items.target,
                        targetTypeId = 0,
                        targetUnit = items.targetUnit,
                        isShared = items.isShared
                    };
                    _db.tSPMS_TargetCountSI.Add(newTargetData);

                    for (var numRate = 5; numRate >= 1; numRate--)
                    {
                        foreach (var ndata in _Pdata)
                        {
                            if (ndata.indicatorId == items.indicatorId && ndata.rating == numRate.ToString())
                            {
                                var newSI_performance = new tSPMS_MFOStandard()
                                {
                                    indicatorId = ndata.indicatorId,
                                    MFOId = MFOdata.MFOId,
                                    rating = ndata.rating,
                                    quantity = quan[numRate - 1].ToString(),
                                    quality = ndata.quality,
                                    timeliness = ndata.timeliness,
                                    targetId = target_id
                                };
                                _db.tSPMS_MFOStandard.Add(newSI_performance);
                            }
                        }
                    }
                    text = "success";
                }
            }
            _db.SaveChanges();
            return Json(new { status = text }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MFO_getMFO_allSI(string _OPCRData)
        {
            var query = from viewOPCR in _db.vSPMS_MFOwCat
                        where viewOPCR.MFOId == _OPCRData & viewOPCR.officeId != null
                        select viewOPCR;
            var data = query.ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /*========================================== ADD MFO ===================================================================*/
        public ActionResult AddCommonMFO()
        {
            return View();
        }

        [HttpPost]
        public ActionResult addMFO_setInd()
        {

            List<vSPMS_rOPCRStandard> ids = new List<vSPMS_rOPCRStandard>();
            //for (int cnt = 0; cnt < limit; cnt++)
            //{
            string randomLetters = new string(Enumerable.Repeat(chars, 6)
    .Select(s => s[random.Next(s.Length)]).ToArray());
            string randomLNumbers = new string(Enumerable.Repeat(nums, 6)
        .Select(s => s[random.Next(s.Length)]).ToArray());

            var si_id = "IND" + DateTime.Now.ToString("yyMMdd") + randomLetters.ToUpper() + randomLNumbers;

            for (int i = 5; i >= 1; i--)
            {
                var newIDS = new vSPMS_rOPCRStandard()
                {
                    rating = i.ToString(),
                    indicatorId = si_id
                };
                ids.Add(newIDS);
            }
            //}
            var data = ids.ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult editMFO_setInd(int limit)
        {

            List<vSPMS_rOPCRStandard> ids = new List<vSPMS_rOPCRStandard>();
            for (int cnt = 0; cnt < limit; cnt++)
            {
                string randomLetters = new string(Enumerable.Repeat(chars, 6)
        .Select(s => s[random.Next(s.Length)]).ToArray());
                string randomLNumbers = new string(Enumerable.Repeat(nums, 6)
            .Select(s => s[random.Next(s.Length)]).ToArray());

                var si_id = "IND" + DateTime.Now.ToString("yyMMdd") + randomLetters.ToUpper() + randomLNumbers;

                for (int i = 5; i >= 1; i--)
                {
                    var newIDS = new vSPMS_rOPCRStandard()
                    {
                        rating = i.ToString(),
                        indicatorId = si_id
                    };
                    ids.Add(newIDS);
                }
            }
            var data = ids.ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult AddPerformance(List<tSPMS_MFOStandard> tOpcrPerformance)
        {
            var isSaved = "";
            foreach (var i in tOpcrPerformance)
            {
                var ifExist = _db.tSPMS_MFOStandard.Where(a => a.MFOId == i.MFOId & a.indicatorId == i.indicatorId).FirstOrDefault();
                if (ifExist == null)
                {
                    _db.tSPMS_MFOStandard.Add(i);
                    isSaved = "success";

                }
                else
                {
                    var update = _db.tSPMS_MFOStandard.Where(a => a.recNo == i.recNo & a.MFOId == i.MFOId & a.indicatorId == i.indicatorId).FirstOrDefault();
                    update.quantity = i.quantity;
                    update.quality = i.quality;
                    update.timeliness = i.timeliness;
                    isSaved = "updated";
                }
            }
            _db.SaveChanges();
            return Json(new { isSaved }, JsonRequestBehavior.AllowGet);
        }
        //========================================================================================================================
        //============================================== EDIT MFO / SIs ==========================================================
        [HttpPost]
        public ActionResult send_requestChanges(vSPMS_nOPCR mfoData, List<vSPMS_nMFO_ALL> reqChangeData, List<tSPMS_MFOStandard> reqDataStandard, string _DateToday)
        {
            var isSaved = "";
            string randomLetters = new string(Enumerable.Repeat(chars, 6)
        .Select(s => s[random.Next(s.Length)]).ToArray());
            string randomLNumbers = new string(Enumerable.Repeat(nums, 6)
        .Select(s => s[random.Next(s.Length)]).ToArray());

            var req_id = "REQ" + DateTime.Now.ToString("yyMMdd") + randomLetters.ToUpper() + randomLNumbers;
            foreach (var i in reqChangeData)
            {
                //MFO Update
                var updateMFO = _db.tAppropriationProjMFOes.Where(a => a.MFOId == mfoData.MFOId).FirstOrDefault();
                if (updateMFO != null && mfoData.MFO != updateMFO.MFO)
                {
                    updateMFO.MFO = mfoData.MFO;
                }

                //Indicator Update
                var updateIND = _db.tAppropriationProjMFOInds.Where(a => a.MFOId == i.MFOId && a.indicatorId == i.indicatorId).FirstOrDefault();
                if (updateIND != null && updateIND.indicator != i.indicator)
                {
                    updateIND.indicator = i.indicator;
                }
                //cateqgory update
                var updatecat = _db.tSPMS_MFOCategory.Where(a => a.MFOId == mfoData.MFOId).FirstOrDefault();

                if (updatecat != null && mfoData.categoryId != updatecat.categoryId)
                {
                    updatecat.categoryId = mfoData.categoryId;
                }

                //year
                var updateYear = _db.tSPMS_MFOCategory.Where(a => a.MFOId == mfoData.MFOId).FirstOrDefault();
                if (updateYear != null && mfoData.year != updateYear.year)
                {
                    updateYear.year = mfoData.year;
                }

                //semester
                var updateSem = _db.tSPMS_MFOCategory.Where(a => a.MFOId == mfoData.MFOId).FirstOrDefault();

                if (updateSem != null && mfoData.semester != updateSem.semester)
                {
                    updateSem.semester = mfoData.semester;
                }

                //class
                var updateClass = _db.tSPMS_MFOCategory.Where(a => a.MFOId == mfoData.MFOId).FirstOrDefault();

                if (updateClass != null && updateClass.classificationId != mfoData.classificationId)
                {
                    updateClass.classificationId = mfoData.classificationId;
                }

                //Standard Update
                if (reqDataStandard != null)
                {
                    foreach (var sdata in reqDataStandard)
                    {
                        var updateSta = _db.tSPMS_MFOStandard.Where(a => a.MFOId == sdata.MFOId && a.indicatorId == sdata.indicatorId && a.rating == sdata.rating).FirstOrDefault();
                        if (updateSta != null)
                        {
                            updateSta.quality = sdata.quality;
                            updateSta.timeliness = sdata.timeliness;
                        }
                    }
                }

                _db.SaveChanges();
                var origData = _db.tSPMS_TargetCountSI.Where(a => a.indicatorId == i.indicatorId).FirstOrDefault();

                if (origData != null)
                {
                    if (i.isShared != null)
                    {
                        origData.isShared = i.isShared;
                    }
                    if (i.tgt_divisionId != null)
                    {
                        origData.divisionId = i.tgt_divisionId;
                    }

                    if (origData.target != i.target)
                    {
                        var reqData = new tSPMS_RequestChange()
                        {
                            MFOId = i.MFOId,
                            indicatorId = i.indicatorId,
                            reqId = req_id,
                            target = i.target,
                            targetId = i.targetId,
                            date = _DateToday,
                            o_target = origData.target,
                            Status = 0, //0 is pending, 1 is approved

                        };

                        _db.tSPMS_RequestChange.Add(reqData);
                    }

                }
                _db.SaveChanges();
            }
            return Json(new { isSaved }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetPerformance(string MFO_ID)
        {
            var data = _db.tSPMS_MFOStandard.Where(a => a.MFOId == MFO_ID).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //========================================================================================================================

        //Request Changes
        public ActionResult RequestChanges()
        {
            var pending = (from reqdata in _db.tSPMS_RequestChange
                           where reqdata.Status == 0
                           select reqdata.reqId).Distinct().ToList();
            ViewBag.pending = pending.Count();

            var approved = (from reqdata in _db.tSPMS_RequestChange
                            where reqdata.Status == 1
                            select reqdata.reqId).Distinct().ToList();
            ViewBag.approved = approved.Count();

            var cancelled = (from reqdata in _db.tSPMS_RequestChange
                             where reqdata.Status == 2
                             select reqdata.reqId).Distinct().ToList();
            ViewBag.cancelled = cancelled.Count();

            var totalrequest = (from reqdata in _db.tSPMS_RequestChange
                                select reqdata.reqId).Distinct().ToList();
            ViewBag.totalrequest = totalrequest.Count();

            return View();
        }
        [HttpPost]
        public ActionResult Get_AllRequest()
        {

            var data = (from reqdata in _db.vSPMS_RequestTable
                        select reqdata).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult Get_RequestContent(string reqId)
        {
            var data = (from reqdata in _db.vSPMS_RequestTable
                        where reqdata.reqId == reqId
                        select reqdata).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult Get_ClassificationDesc(string classId)
        {
            var data = (from reqdata in _db.tSPMS_Classification
                        where reqdata.classificationId.ToString() == classId
                        select reqdata.classification).FirstOrDefault();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Update_statusRequest(string reqId, int statusChange, List<tSPMS_RequestChange> reqData)
        {
            var data = "successfully";
            var update = _db.tSPMS_RequestChange.Where(a => a.reqId == reqId).FirstOrDefault();

            if (update != null)
            {
                if (statusChange == 1)
                {
                    foreach (var rdata in reqData)
                    {
                        //MFO Update
                        //if (rdata.req_MFO != null && rdata.req_MFO !="")
                        //{
                        //    var updateMFO = _db.tAppropriationProjMFOes.Where(a => a.MFOId == rdata.MFOId).FirstOrDefault();
                        //    updateMFO.MFO = rdata.req_MFO;
                        //}

                        ////Indicator Update
                        //if (rdata.req_indicator != null && rdata.req_indicator != "")
                        //{
                        //    var updateIND = _db.tAppropriationProjMFOInds.Where(a => a.MFOId == rdata.MFOId && a.indicatorId == rdata.indicatorId).FirstOrDefault();
                        //    updateIND.indicator = rdata.req_indicator;

                        //}

                        //Target Update
                        if (rdata.target != null)
                        {
                            var updatetarg = _db.tAppropriationProjMFOInds.Where(a => a.MFOId == rdata.MFOId && a.indicatorId == rdata.indicatorId).FirstOrDefault();
                            updatetarg.target = rdata.target;

                            var updatetar = _db.tSPMS_TargetCountSI.Where(a => a.targetId == rdata.targetId).FirstOrDefault();
                            updatetar.target = rdata.target;
                            updatetar.tRemaining = rdata.target;

                            if (rdata.target == 1)
                            {
                                quan[4] = "1";
                                quan[3] = "";
                                quan[2] = "";
                                quan[1] = "";
                                quan[0] = "0";
                            }
                            else
                            {
                                quan[4] = (((double)rdata.target * 0.3) + (double)rdata.target).ToString();
                                quan[3] = (((double)rdata.target * 0.15) + (double)rdata.target).ToString();
                                quan[2] = ((double)rdata.target).ToString();
                                quan[1] = (((double)rdata.target / 2) + 1).ToString();
                                quan[0] = ((double)rdata.target / 2).ToString();
                            }
                            var update_performance = _db.tSPMS_MFOStandard.FirstOrDefault(a => a.indicatorId == rdata.indicatorId & a.MFOId == rdata.MFOId);
                            if (update_performance != null)
                            {
                                for (var limit = 5; limit >= 1; limit--)
                                {
                                    var updateData = _db.tSPMS_MFOStandard.FirstOrDefault(a => a.indicatorId == rdata.indicatorId & a.MFOId == rdata.MFOId & a.rating == limit.ToString());
                                    {
                                        updateData.quantity = quan[limit - 1].ToString();
                                    }
                                }
                            }
                        }

                        ////cateqgory update
                        //if (rdata.req_categoryId !=null && rdata.req_categoryId != 0)
                        //{
                        //    var updatetar = _db.tMFOCategories.Where(a => a.MFOId == rdata.MFOId).FirstOrDefault();
                        //    updatetar.categoryId = rdata.req_categoryId;

                        //}

                        ////year
                        //if (rdata.year != null && rdata.year != 0)
                        //{
                        //    var updatetar = _db.tMFOCategories.Where(a => a.MFOId == rdata.MFOId).FirstOrDefault();
                        //    updatetar.year = rdata.year;

                        //}

                        ////semester
                        //if (rdata.semester != null && rdata.semester != 0)
                        //{
                        //    var updatetar = _db.tMFOCategories.Where(a => a.MFOId == rdata.MFOId).FirstOrDefault();
                        //    updatetar.semester = rdata.semester;

                        //}

                        ////class
                        //if (rdata.classificationId != null && rdata.classificationId != "")
                        //{
                        //    var updatetar = _db.tMFOCategories.Where(a => a.MFOId == rdata.MFOId).FirstOrDefault();
                        //    updatetar.classificationId = rdata.classificationId;

                        //}

                        //////Standard Update
                        //if (reqDataStandard != null)
                        //{
                        //    foreach (var sdata in reqDataStandard)
                        //    {
                        //        if (sdata.req_Rating != null)
                        //        {
                        //            var updateSta = _db.tOpcrPerformances.Where(a => a.MFOId == rdata.MFOId && a.indicatorId == rdata.indicatorId && a.rating == sdata.req_Rating.ToString()).FirstOrDefault();
                        //            if (updateSta != null)
                        //            {
                        //                if (sdata.req_quality != null && sdata.req_quality != "")
                        //                {
                        //                    updateSta.quality = sdata.req_quality;

                        //                }
                        //                if (sdata.req_timeliness != null && sdata.req_timeliness != "")
                        //                {
                        //                    updateSta.timeliness = sdata.req_timeliness;
                        //                }
                        //                _db.SaveChanges();
                        //            }
                        //        }
                        //    }
                        //}
                        var upStatus = _db.tSPMS_RequestChange.Where(a => a.recNo == rdata.recNo).FirstOrDefault();
                        upStatus.Status = statusChange;
                        _db.SaveChanges();
                    }
                }
            }


            return Json(new { data }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Reject_statusRequest(string reqID)
        {
            var data = "successfully";
            var upStatus = _db.tSPMS_RequestChange.Where(a => a.reqId == reqID).ToList();

            foreach (var upItems in upStatus)
            {
                upItems.Status = 2;
            }
            _db.SaveChanges();

            return Json(new { data }, JsonRequestBehavior.AllowGet);
        }



        //============================ COMMON MFO =======================================
        [HttpPost]
        public ActionResult CMFO_get(String _OfficeID)
        {

            var cmfo = _db.vSPMS_CommonMFO.Where(a => a.officeId == null).ToList();
            return Json(new { cmfo }, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult CMFO_getMFOperId(String MFOId)
        {
            var objCMFO = _db.vSPMS_CommonMFO.Where(a => a.MFOId == MFOId & a.officeId == null).ToList();

            return Json(new { data = 1, objCMFO }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult CMFO_add(vSPMS_CommonMFO newCMFO, List<tSPMS_MFOStandard> newStandard, String _OfficeID, bool isCheck)
        {
            string randomLetters = new string(Enumerable.Repeat(chars, 6)
             .Select(s => s[random.Next(s.Length)]).ToArray());
            string randomLNumbers = new string(Enumerable.Repeat(nums, 6)
             .Select(s => s[random.Next(s.Length)]).ToArray());

            var MFO_id = "MFO" + DateTime.Now.ToString("yyMMdd") + randomLetters.ToUpper() + randomLNumbers;
            var indicator_id = "IND" + DateTime.Now.ToString("yyMMdd") + randomLetters.ToUpper() + randomLNumbers;
            var target_id = "TGT" + DateTime.Now.ToString("yyMMdd") + randomLetters.ToUpper() + randomLNumbers;


            var isExistCMFO = (from c in _db.tAppropriationProjMFOes
                               where c.MFO.Contains(newCMFO.MFO)
                               select c);

            var data = 0;
            var _t_quality = "";
            var _t_timeliness = "";
            // List<vSPMS_ListAllCMFO> getObj = null;

            if (isExistCMFO.Count() == 0)
            {
                var newMFOData = new tAppropriationProjMFO()
                {
                    MFOId = MFO_id,
                    MFO = newCMFO.MFO,
                    isActive = 0
                };
                _db.tAppropriationProjMFOes.Add(newMFOData);

                if (isCheck)
                {
                    var addCMFO = new tSPMS_CommonMFO()
                    {
                        MFOId = MFO_id,
                        indicatorId = indicator_id,
                        targetId = target_id,
                        officeId = _OfficeID,
                        isHrtgt = 1

                    };

                    _db.tSPMS_CommonMFO.Add(addCMFO);

                    var addtoTarget_t = new tSPMS_TargetCountSI()
                    {
                        targetId = target_id,
                        officeId = _OfficeID,
                        indicatorId = indicator_id,
                        t_quality = _t_quality,
                        t_timeliness = _t_timeliness
                    };

                    _db.tSPMS_TargetCountSI.Add(addtoTarget_t);
                }
                else
                {
                    var addCMFO = new tSPMS_CommonMFO()
                    {
                        MFOId = MFO_id,
                        indicatorId = indicator_id,
                        targetId = target_id,

                    };

                    _db.tSPMS_CommonMFO.Add(addCMFO);

                    var addtoTarget_t = new tSPMS_TargetCountSI()
                    {
                        targetId = target_id,
                        indicatorId = indicator_id,
                        t_quality = _t_quality,
                        t_timeliness = _t_timeliness
                    };

                    _db.tSPMS_TargetCountSI.Add(addtoTarget_t);
                }



                if (newStandard[2].quality != null && newStandard[2].timeliness != null)
                {
                    _t_quality = newStandard[2].quality;
                    _t_timeliness = newStandard[2].timeliness;
                }
                else
                {
                    _t_quality = newStandard[4].quality;
                    _t_timeliness = newStandard[4].timeliness;
                }



                var addSIData = new tAppropriationProjMFOInd()
                {
                    MFOId = MFO_id,
                    indicatorId = indicator_id,
                    indicator = newCMFO.indicator,
                    //target = newSI.target,
                    targetUnit = "N",
                    targetTypeId = 0,
                    isActive = 0,

                };
                _db.tAppropriationProjMFOInds.Add(addSIData);

                foreach (var i in newStandard)
                {
                    var addStandard = new tSPMS_MFOStandard()
                    {
                        targetId = target_id,
                        MFOId = MFO_id,
                        indicatorId = indicator_id,
                        rating = i.rating,
                        quantity = i.quantity,
                        quality = i.quality,
                        timeliness = i.timeliness
                    };
                    _db.tSPMS_MFOStandard.Add(addStandard);
                }
                data = 1;
            }
            else
            {
                data = 2;

            }

            _db.SaveChanges();

            var getObj = _db.vSPMS_CommonMFO.Where(a => a.MFOId == MFO_id & a.officeId == null).ToList();

            return Json(new { data, objCMFO = getObj }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult CMFO_moreSI(tAppropriationProjMFO getMFO, tAppropriationProjMFOInd newSI, List<tSPMS_MFOStandard> newStandard, String _OfficeID)
        {
            string randomLetters = new string(Enumerable.Repeat(chars, 6)
     .Select(s => s[random.Next(s.Length)]).ToArray());
            string randomLNumbers = new string(Enumerable.Repeat(nums, 6)
             .Select(s => s[random.Next(s.Length)]).ToArray());

            var indicator_id = "IND" + DateTime.Now.ToString("yyMMdd") + randomLetters.ToUpper() + randomLNumbers;
            var target_id = "TGT" + DateTime.Now.ToString("yyMMdd") + randomLetters.ToUpper() + randomLNumbers;



            var addSIData = new tAppropriationProjMFOInd()
            {
                MFOId = getMFO.MFOId,
                indicatorId = indicator_id,
                indicator = newSI.indicator,
                targetUnit = "N",
                targetTypeId = 0,
                isActive = 0,

            };
            _db.tAppropriationProjMFOInds.Add(addSIData);

            var addCMFO = new tSPMS_CommonMFO()
            {
                MFOId = getMFO.MFOId,
                indicatorId = indicator_id,
                targetId = target_id,
            };
            _db.tSPMS_CommonMFO.Add(addCMFO);


            foreach (var i in newStandard)
            {
                var addStandard = new tSPMS_MFOStandard()
                {
                    targetId = target_id,
                    MFOId = getMFO.MFOId,
                    indicatorId = indicator_id,
                    rating = i.rating,
                    quantity = i.quantity,
                    quality = i.quality,
                    timeliness = i.timeliness
                };
                _db.tSPMS_MFOStandard.Add(addStandard);
            }
            _db.SaveChanges();

            var getObj = _db.vSPMS_CommonMFO.Where(a => a.MFOId == getMFO.MFOId & a.officeId == null).ToList();

            return Json(new { data = 1, objCMFO = getObj }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult CMFO_editTarget(int TargetCount, String IndicatorID, String MFOID, String _OfficeID, String TargetId, List<tSPMS_MFOStandard> newStandard)
        {
            if (TargetCount == 0)
            {

                var updateTarget = _db.tSPMS_TargetCountSI.FirstOrDefault(a => a.targetId == TargetId);
                updateTarget.target = null;
                updateTarget.tRemaining = null;

                var removeSI = _db.tSPMS_OPCR.FirstOrDefault(a => a.indicatorId == IndicatorID & a.officeId == _OfficeID);
                _db.tSPMS_OPCR.Remove(removeSI);
            }
            else
            {
                var updateTarget = _db.tSPMS_TargetCountSI.FirstOrDefault(a => a.targetId == TargetId);
                updateTarget.target = TargetCount;
                updateTarget.tRemaining = TargetCount;
            }

            /*var updateTarget = _db.tTargetCountSIs.Where(a => a.targetId == TargetId).FirstOrDefault();
            updateTarget.target = TargetCount;*/


            foreach (var i in newStandard)
            {

                var update = _db.tSPMS_MFOStandard.Where(a => a.recNo == i.recNo & a.MFOId == i.MFOId & a.indicatorId == i.indicatorId).FirstOrDefault();
                update.quantity = i.quantity;

            }
            _db.SaveChanges();
            return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult CMFO_Standard(vSPMS_CMFO mfoindId)
        {
            var data = _db.tSPMS_MFOStandard.Where(a => a.targetId == mfoindId.catTargetId).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult CMFO_UpdateStandard(List<tSPMS_MFOStandard> objStandard)
        {

            foreach (var item in objStandard)
            {
                var update = _db.tSPMS_MFOStandard.Where(a => a.rating == item.rating & a.MFOId == item.MFOId & a.indicatorId == item.indicatorId).ToList();
                foreach (var i in update)
                {
                    i.quality = item.quality;
                    i.timeliness = item.timeliness;
                }

            }
            _db.SaveChanges();
            return Json(new { data = 1 }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CMFO_getMFO_allSI(vSPMS_CommonMFO mfoData)
        {
            var data = _db.vSPMS_CommonMFO.Where(a => a.MFOId == mfoData.MFOId & a.indicatorId == mfoData.indicatorId & a.catTargetId == mfoData.catTargetId).FirstOrDefault();
            var standard = _db.tSPMS_MFOStandard.Where(a => a.targetId == mfoData.catTargetId).ToList();
            return Json(new { data, standard }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult CMFO_updateMFO_allSI(vSPMS_CommonMFO MFOData, List<tSPMS_MFOStandard> objStandard)
        {
            var updateMFO = _db.tAppropriationProjMFOes.FirstOrDefault(a => a.MFOId == MFOData.MFOId);
            updateMFO.MFO = MFOData.MFO;

            var updateSI = _db.tAppropriationProjMFOInds.Where(a => a.indicatorId == MFOData.indicatorId & a.MFOId == MFOData.MFOId).FirstOrDefault();
            updateSI.indicator = MFOData.indicator;

            foreach (var item in objStandard)
            {
                var update = _db.tSPMS_MFOStandard.Where(a => a.rating == item.rating & a.MFOId == item.MFOId & a.indicatorId == item.indicatorId).ToList();
                foreach (var i in update)
                {

                    i.quality = item.quality;
                    i.timeliness = item.timeliness;
                }

            }
            _db.SaveChanges();
            return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult addDivision(String DivName, String Office_ID)
        {
            string randomLetters = new string(Enumerable.Repeat(chars, 3)
            .Select(s => s[random.Next(s.Length)]).ToArray());
            string randomLNumbers = new string(Enumerable.Repeat(nums, 3)
             .Select(s => s[random.Next(s.Length)]).ToArray());

            String cutDiv = DivName.Length > 4 ? DivName.Substring(0, 4).ToUpper() : DivName.Substring(0, 3).ToUpper();

            var div_id = "DIV" + cutDiv.Trim() + randomLetters.ToUpper() + randomLNumbers;

            var isExist = _db.tOfficeDivisions.Where(a => a.divisionName == DivName).FirstOrDefault();
            var data = 0;
            if (isExist == null)
            {
                var addDiv = new tOfficeDivision()
                {
                    divisionId = div_id,
                    divisionName = DivName,
                    officeId = Office_ID,
                    tag = 1

                };
                _db.tOfficeDivisions.Add(addDiv);
                data = 1;
            }

            _db.SaveChanges();
            return Json(new { status = data }, JsonRequestBehavior.AllowGet);
        }
        //===============================================================================
    }
}