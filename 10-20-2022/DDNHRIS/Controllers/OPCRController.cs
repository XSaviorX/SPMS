using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DDNHRIS.Models.SPMS;


namespace DDNHRIS.Controllers
{
    public class OPCRController : Controller
    {
        SPMSDBEntities _db = new SPMSDBEntities();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string nums = "0123456789";
        Random random = new Random();
        int[] quan = new int[5];




        //
        // GET: /OPCR/

        [HttpPost]

        public JsonResult cookies(string officeId)
        {
            HttpCookie cookiname = new HttpCookie("opcrCrookie")
            {
                Value = officeId + ","

            };
            Response.Cookies.Add(cookiname);
            return Json(new { data = "success" });

        }

        public ActionResult Standard()
        {
            return View();
        }
        public ActionResult Target()
        {

            return View();
        }

        public ActionResult Actual()
        {

            return View();
        }

        public ActionResult MFO_get()
        {
            var data = _db.loadDataViews.ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MFO_getPerOffice(string _OfficeID, String opcrId)
        {

            // List of MFO and SI per Officess

            var cmfo = _db.vSPMS_CommonMFO.Where(a => a.officeId == _OfficeID & a.divisionId == null | a.officeId == null & a.divisionId == null | a.isHrtgt == 1).ToList(); // List of cMFO 
            var mfoData = _db.vSPMS_nMFO_ALL.Where(a => a.officeId == _OfficeID).ToList();
            var standardData = _db.vSPMS_nOPCR.Where(a => a.officeId == _OfficeID).OrderBy(a => a.categoryId).ToList(); // Assigned SI per Officess
            var office = _db.tOffices.Where(a => a.officeId == _OfficeID).FirstOrDefault();

            return Json(new { cmfo, standardData, office, mfoData }, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult MFO_getList(string _OfficeID)
        {
            var mfoData = _db.vSPMS_nMFO_ALL.Where(a => a.officeId == _OfficeID & a.isCMFO == null).ToList();
            return Json(new { mfoData }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MFO_updateMFOSI(view_OPCRTable _OPCRData)
        {
            var updateMFO = _db.tAppropriationProjMFOes.FirstOrDefault(a => a.appropProjectId == _OPCRData.appropProjectId & a.MFOId == _OPCRData.MFOId);
            updateMFO.MFO = _OPCRData.MFO;
            _db.SaveChanges();

            var updateSI = _db.tAppropriationProjMFOInds.FirstOrDefault(a => a.indicatorId == _OPCRData.indicatorId & a.MFOId == _OPCRData.MFOId);
            updateSI.indicator = _OPCRData.indicator;
            updateSI.target = _OPCRData.target;
            _db.SaveChanges();

            quan[4] = (int)(((double)_OPCRData.target * 0.3) + (double)_OPCRData.target);
            quan[3] = (int)(((double)_OPCRData.target * 0.15) + (double)_OPCRData.target);
            quan[2] = (int)((double)_OPCRData.target);
            quan[1] = (int)(((double)_OPCRData.target / 2) + 1);
            quan[0] = (int)(quan[1] - 1);

            var newSI_performance = _db.tSPMS_MFOStandard.FirstOrDefault(a => a.indicatorId == _OPCRData.indicatorId & a.MFOId == _OPCRData.MFOId);

            if (newSI_performance == null)
            {
                for (var num = 5; num >= 1; num--)
                {
                    var newData = new tSPMS_MFOStandard()
                    {
                        indicatorId = _OPCRData.indicatorId,
                        MFOId = _OPCRData.MFOId,
                        rating = num.ToString(),
                        quantity = quan[num - 1].ToString(),
                    };
                    _db.tSPMS_MFOStandard.Add(newData);
                }
                _db.SaveChanges();
            }
            else
            {
                for (var limit = 5; limit >= 1; limit--)
                {
                    var updateData = _db.tSPMS_MFOStandard.FirstOrDefault(a => a.indicatorId == _OPCRData.indicatorId & a.MFOId == _OPCRData.MFOId & a.rating == limit.ToString());
                    {
                        updateData.quantity = quan[limit - 1].ToString();
                    }
                }
                _db.SaveChanges();
            }
            return Json(new { status = _OPCRData.officeId }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MFO_getByID(string _OfficeID, string _MFO_ID, string _SI_ID)
        {
            var query = from viewOPCR in _db.view_OPCRTable
                        where viewOPCR.officeId == _OfficeID & viewOPCR.programTypeId == 0 & viewOPCR.MFOId == _MFO_ID & viewOPCR.indicatorId == _SI_ID
                        select viewOPCR;
            var data = query.ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult MFO_getoffices()
        {
            var data = _db.tOffices.ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult MFO_getPrograms(string _OfficeID)
        {
            var query = from offID in _db.tPrograms where offID.officeId == _OfficeID & offID.programTypeId == 0 select offID;
            var data = query.ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MFO_getProjects(string _ProgramID)
        {
            var query = (from MFOProjects in _db.appropprojectids
                         where MFOProjects.programId == _ProgramID & MFOProjects.budgetYear == 2022
                         select MFOProjects).ToList();
            //var data = query.ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MFO_insert(tAppropriationProjMFO _MFO, appropprojectid _AppropProjID)
        {
            string randomLetters = new string(Enumerable.Repeat(chars, 6)
        .Select(s => s[random.Next(s.Length)]).ToArray());
            string randomLNumbers = new string(Enumerable.Repeat(nums, 6)
        .Select(s => s[random.Next(s.Length)]).ToArray());

            var MFO_id = "MFO" + DateTime.Now.ToString("yyMMdd") + randomLetters.ToUpper() + randomLNumbers;

            var data = _db.tAppropriationProjMFOes.FirstOrDefault(a => a.MFOId == MFO_id);

            var newMFOData = new tAppropriationProjMFO()
            {
                MFOId = MFO_id,
                MFO = _MFO.MFO,
                appropProjectId = _AppropProjID.appropProjectId1,
                isActive = 0
            };
            _db.tAppropriationProjMFOes.Add(newMFOData);
            _db.SaveChanges();

            return Json(new { status = MFO_id }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult MFO_SIinsert(List<tAppropriationProjMFOInd> indicators, string MFO_ID)
        {
            string randomLetters = new string(Enumerable.Repeat(chars, 6)
        .Select(s => s[random.Next(s.Length)]).ToArray());
            string randomLNumbers = new string(Enumerable.Repeat(nums, 6)
        .Select(s => s[random.Next(s.Length)]).ToArray());
            foreach (var ndata in indicators)
            {
                var si_id = "IND" + DateTime.Now.ToString("yyMMdd") + randomLetters.ToUpper() + randomLNumbers;
                var newMFO_SI_Data = new tAppropriationProjMFOInd()
                {
                    indicatorId = si_id,
                    MFOId = MFO_ID,
                    indicator = ndata.indicator,
                    target = ndata.target,
                    targetUnit = "N",
                    targetTypeId = 0,
                    isActive = 0
                };
                _db.tAppropriationProjMFOInds.Add(newMFO_SI_Data);
            }
            _db.SaveChanges();

            return Json(new { status = MFO_ID }, JsonRequestBehavior.AllowGet);
            //return Json(new { status = "MFO ID: " + MFO_ID + "\n" + indicators.ElementAt(0).indicator + "\n" + indicators.ElementAt(0).indicatorId + "\n" + indicators.ElementAt(0).isActive + "\n" + indicators.ElementAt(0).MFOId + "\n" + indicators.ElementAt(0).recNo + "\n" + indicators.ElementAt(0).target + "\n" + indicators.ElementAt(0).targetTypeId + "\n" + indicators.ElementAt(0).targetUnit }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult GetPerformance()
        {
            var data = _db.tSPMS_MFOStandard.ToList();
            return Json(data, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult GetPerformancePer(tSPMS_MFOStandard tOpcrPerformance)
        {
            var data = _db.tSPMS_MFOStandard.Where(a => a.MFOId == tOpcrPerformance.MFOId & a.indicatorId == tOpcrPerformance.indicatorId).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);

        }

        //[HttpPost]
        //public ActionResult AddPerformance(List<tSPMS_MFOStandard> tOpcrPerformance)
        //{
        //    var isSaved = "";


        //        foreach (var i in tOpcrPerformance)
        //        {
        //            var ifExist = _db.tSPMS_MFOStandard.Where(a => a.MFOId == i.MFOId & a.indicatorId == i.indicatorId).FirstOrDefault();
        //            if (ifExist == null)
        //            {
        //                _db.tSPMS_MFOStandard.Add(i);
        //                isSaved = "success";

        //            }
        //            else
        //            {
        //                var update = _db.tSPMS_MFOStandard.Where(a => a.recNo == i.recNo & a.MFOId == i.MFOId & a.indicatorId == i.indicatorId).FirstOrDefault();
        //                update.quantity = i.quantity;
        //                update.quality = i.quality;
        //                update.timeliness = i.timeliness;
        //                isSaved = "updated";
        //            }

        //        }

        //        _db.SaveChanges();
        //        return Json(new { isSaved }, JsonRequestBehavior.AllowGet);

        //}

        [HttpPost]
        public ActionResult updateAssigned(view_OPCRTable ndata)
        {
            var text = "1";
            var update = _db.tAppropriationProjMFOInds.Where(a => a.MFOId == ndata.MFOId & a.indicatorId == ndata.indicatorId).FirstOrDefault();
            update.isActive = ndata.isActive;
            if (ndata.isActive == 0)
            {
                text = "0"; //removed
            }

            _db.SaveChanges();
            return Json(new { isSaved = text }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult removeIndicator(String indicatorId)
        {
            var update = _db.tAppropriationProjMFOInds.Where(a => a.indicatorId == indicatorId).FirstOrDefault();
            update.isActive = 0;
            _db.SaveChanges();
            return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MFO_getOPCRData(string _OfficeID)
        {
            var data = (from viewOPCR in _db.vprt_OPCR
                        where viewOPCR.officeId == _OfficeID & viewOPCR.programTypeId == 0
                        select viewOPCR).ToList();


            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]

        public JsonResult cookiestarget(string officeId)
        {
            HttpCookie cookiname = new HttpCookie("targetCrookie")
            {
                Value = officeId + ","

            };
            Response.Cookies.Add(cookiname);

            return Json(new { data = officeId });
        }

        //==========================

        [HttpPost]
        public ActionResult NewAssign(vSPMS_nMFO_ALL Assign, String _OfficeID, String _opcrId, String IsHrtgt)
        {
            var OfficeID = "";
            if (IsHrtgt == "1")
            {
                OfficeID = _OfficeID;
            }
            else
            {
                if (Assign.TargetOffcId == null)
                {
                    OfficeID = _OfficeID;
                }
                else
                {
                    OfficeID = Assign.TargetOffcId;
                }
            }

            var assignData = new tSPMS_OPCR()
            {
                opcrID = _opcrId,
                indicatorId = Assign.indicatorId,
                officeId = OfficeID,
                categoryId = Assign.categoryId,
                targetId = Assign.targetId
            };
            _db.tSPMS_OPCR.Add(assignData);

            var upData = _db.tAppropriationProjMFOes.Where(a => a.MFOId == Assign.MFOId).FirstOrDefault();
            if (upData != null)
            {
                upData.isActive = 1;
            }
            _db.SaveChanges();

            return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult NewUnassign(vSPMS_nMFO_ALL Unassign, String _OfficeID)
        {

            var data = _db.tSPMS_OPCR.Where(a => a.indicatorId == Unassign.indicatorId && a.officeId == _OfficeID).FirstOrDefault();
            _db.tSPMS_OPCR.Remove(data);

            var upData = _db.tAppropriationProjMFOes.Where(a => a.MFOId == Unassign.MFOId).FirstOrDefault();
            if (upData != null)
            {
                upData.isActive = 0;
            }
            _db.SaveChanges();

            return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);
        }

        //=========================
        [HttpPost]
        public ActionResult updateMFO_Division(List<vSPMS_nOPCR> targetTableData)
        {
            var msg = 0;
            if (targetTableData != null)
            {
                msg = 1;
                foreach (var ndata in targetTableData)
                {
                    var updateDivision = _db.tMFO_perDivision.Where(a => a.MFOId == ndata.MFOId).FirstOrDefault();
                    if (updateDivision != null)
                    {
                        //updateDivision.division = ndata.division;
                        _db.SaveChanges();
                    }

                    else
                    {
                        var insertMFO_Division = new tMFO_perDivision()
                        {
                            MFOId = ndata.MFOId,
                            //  division = ndata.division
                        };
                        _db.tMFO_perDivision.Add(insertMFO_Division);
                        _db.SaveChanges();
                    }
                }

            }
            return Json(new { status = msg }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MFO_getIndicatorDetails(string indId, string targtId)
        {
            var indDetails = _db.vSPMS_nOPCR.Where(a => a.indicatorId == indId).FirstOrDefault();
            var standardDatas = _db.tSPMS_MFOStandard.Where(a => a.targetId == targtId).ToList();
            return Json(new { indDetails, standardDatas }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CMFO_addTargetCat(vSPMS_CommonMFO cmfoes, List<tSPMS_MFOStandard> standardData, String _OfficeID)
        {

            var test = "";

            string randomLetters = new string(Enumerable.Repeat(chars, 6)
            .Select(s => s[random.Next(s.Length)]).ToArray());
            string randomLNumbers = new string(Enumerable.Repeat(nums, 6)
             .Select(s => s[random.Next(s.Length)]).ToArray());


            var target_id = "TGT" + DateTime.Now.ToString("yyMMdd") + randomLetters.ToUpper() + randomLNumbers;
            var isExist = _db.tSPMS_CommonMFO.Where(a => a.MFOId == cmfoes.MFOId & a.officeId == _OfficeID & a.targetId == cmfoes.catTargetId).FirstOrDefault();
            var isCreated = _db.tSPMS_TargetCountSI.FirstOrDefault(a => a.indicatorId == cmfoes.indicatorId & a.officeId == _OfficeID);

            if (isCreated == null)
            {
                if (isExist == null)
                {
                    var add = new tSPMS_CommonMFO()
                    {
                        MFOId = cmfoes.MFOId,
                        indicatorId = cmfoes.indicatorId,
                        officeId = _OfficeID,
                        categoryId = cmfoes.categoryId,
                        targetId = target_id,

                    };
                    _db.tSPMS_CommonMFO.Add(add);

                    foreach (var i in standardData)
                    {
                        var standard = _db.tSPMS_MFOStandard.Where(a => a.targetId == cmfoes.catTargetId & a.rating == i.rating).FirstOrDefault();

                        var addstandard = new tSPMS_MFOStandard()
                        {

                            rating = i.rating,
                            quantity = i.quantity,
                            timeliness = standard.timeliness,
                            quality = standard.quality,
                            targetId = target_id,
                            MFOId = cmfoes.MFOId,
                            indicatorId = cmfoes.indicatorId,

                        };
                        _db.tSPMS_MFOStandard.Add(addstandard);

                    }

                    /* quan[4] = (int)(((double)cmfoes.target * 0.3) + (double)cmfoes.target);
                     quan[3] = (int)(((double)cmfoes.target * 0.15) + (double)cmfoes.target);
                     quan[2] = (int)((double)cmfoes.target);
                     quan[1] = (int)(((double)cmfoes.target / 2) + 1);
                     quan[0] = (int)(quan[1] - 1);

                     for (var limit = 5; limit >= 1; limit--)
                     {
                         var standard = _db.tSPMS_MFOStandard.Where(a => a.targetId == cmfoes.catTargetId & a.rating == limit.ToString()).FirstOrDefault();
                         var addstandard = new tSPMS_MFOStandard()
                         {

                             rating = limit.ToString(),
                             quantity = quan[limit - 1].ToString(),
                             timeliness = standard.timeliness,
                             quality = standard.quality,
                             targetId = target_id,
                             MFOId = cmfoes.MFOId,
                             indicatorId = cmfoes.indicatorId,

                         };
                         _db.tSPMS_MFOStandard.Add(addstandard);

                     }*/



                }


                if (cmfoes.targetId == null) // insert data if new
                {
                    var target = new tSPMS_TargetCountSI()
                    {
                        targetId = target_id,
                        indicatorId = cmfoes.indicatorId,
                        target = cmfoes.target,
                        tRemaining = cmfoes.target,
                        officeId = _OfficeID,
                        targetUnit = cmfoes.targetUnit

                    };
                    _db.tSPMS_TargetCountSI.Add(target);

                }

            }
            else
            {
                var updateTarget = _db.tSPMS_TargetCountSI.FirstOrDefault(a => a.targetId == cmfoes.targetId);
                updateTarget.target = cmfoes.target;
                updateTarget.tRemaining = cmfoes.target;
                updateTarget.targetUnit = cmfoes.targetUnit;

                var listCat = _db.tSPMS_CommonMFO.Where(a => a.MFOId == cmfoes.MFOId & a.officeId == _OfficeID).ToList();

                foreach (var i in listCat)
                {
                    i.categoryId = cmfoes.categoryId;
                }
                /*isExist.categoryId = cmfoes.cmfo_catId;*/
                /*  quan[4] = (int)(((double)cmfoes.target * 0.3) + (double)cmfoes.target);
                  quan[3] = (int)(((double)cmfoes.target * 0.15) + (double)cmfoes.target);
                  quan[2] = (int)((double)cmfoes.target);
                  quan[1] = (int)(((double)cmfoes.target / 2) + 1);
                  quan[0] = (int)(quan[1] - 1);

                  for (var limit = 5; limit >= 1; limit--)
                  {
                      var performance = _db.tSPMS_MFOStandard.FirstOrDefault(a => a.targetId == cmfoes.targetId & a.rating == limit.ToString());
                      {
                          performance.quantity = quan[limit - 1].ToString();
                      }

                  }*/

                foreach (var i in standardData)
                {
                    var standard = _db.tSPMS_MFOStandard.Where(a => a.targetId == cmfoes.catTargetId & a.rating == i.rating).FirstOrDefault();

                    standard.quantity = i.quantity;

                }
            }


            _db.SaveChanges();

            return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MFO_showAddtgt(vSPMS_CommonMFO CMFO, String _OfficeID)
        {


            List<tSPMS_MFOStandard> standard = new List<tSPMS_MFOStandard>();

            var cmfo = _db.vSPMS_CommonMFO.Where(a => a.MFOId == CMFO.MFOId & a.indicatorId == CMFO.indicatorId & a.officeId == _OfficeID).FirstOrDefault();
            var catcmfo = _db.vSPMS_CommonMFO.Where(a => a.MFOId == CMFO.MFOId & a.officeId == _OfficeID).FirstOrDefault();

            if (cmfo != null)
            {
                standard = _db.tSPMS_MFOStandard.Where(a => a.targetId == cmfo.targetId).ToList();

            }

            return Json(new { status = 1, cmfo, catcmfo, standard }, JsonRequestBehavior.AllowGet);
        }

        // TEST CODDE HERE ======================

        [HttpPost]
        public ActionResult MFO_getYears()
        {

            var years = _db.tSPMS_MFOCategory.Select(a => a.year).Distinct().ToList();

            return Json(years, JsonRequestBehavior.AllowGet);
        }



        // TEST CODE END =========================

    }

}