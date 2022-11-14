using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DDNHRIS.Models.SPMS;

namespace DDNHRIS.Controllers
{
    public class SPMS_DPCRController : Controller
    {
        SPMSDBEntities _db = new SPMSDBEntities();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string nums = "0123456789";
        Random random = new Random();
        int[] quan = new int[5];

        // GET: SPMS_DPCR
        public ActionResult DPCR()
        {
            return View();
        }
        public ActionResult DPCR_Actual()
        {
            return View();
        }
        public ActionResult CreateCheckList()
        {
            return View();
        }
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

        public ActionResult MFO_get()
        {
            var data = _db.loadDataViews.ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MFO_getPerOffice(string _OfficeID, string _DivisionID)
        {

            var divisionMFO = _db.vSPMS_nMFO_ALL.Where(a => a.divisionId == _DivisionID).ToList(); // List of MFO created by division
                                                                                                   // var mfoData = _db.vNew_tOPCR.Where(a => a.officeId == _OfficeID & a.divisionId == null & a.isCMFO == null | a.TargetOffcId == _OfficeID & a.divisionId == null & a.isCMFO == null | a.divisionId == _DivisionID).ToList(); // List of MFO and SI per Officess
            var mfoData = _db.vSPMS_nMFO_ALL.Where(a => a.officeId == _OfficeID & a.divisionId == null | a.TargetOffcId == _OfficeID & a.divisionId == null | a.divisionId == _DivisionID).ToList(); // List of MFO and SI per Officess
            var cmfo = _db.vSPMS_CommonMFO.Where(a => a.divisionId == null | a.officeId == _OfficeID | a.isHrtgt == 1).ToList(); // List of cMFO 

            var standardData = _db.vSPMS_DPCR.Where(a => a.officeId == _OfficeID && a.divisionId == _DivisionID).OrderBy(a => a.description).ToList(); // Assigned SI per Offices and division
            var office = _db.tOffices.Where(a => a.officeId == _OfficeID).FirstOrDefault();
            var CLData = _db.vSPMS_CheckListMFO.Where(a => a.officeId == _OfficeID).ToList();

            return Json(new { mfoData, standardData, office, CLData, divisionMFO, cmfo }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MFO_updateMFOSI(vSPMS_nMFO_ALL _OPCRData)
        {
            var updateMFO = _db.tAppropriationProjMFOes.FirstOrDefault(a => a.MFOId == _OPCRData.MFOId);
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
        public ActionResult SHOW_USERS(vSPMS_nMFO_ALL IPCRDATA)
        {
            var users = _db.vSPMS_IPCRwCat.Where(a => a.i_MFOId == IPCRDATA.MFOId & a.i_indicatorId == IPCRDATA.indicatorId).ToList();
            return Json(new { status = 1, users });
        }

        [HttpPost]
        public ActionResult MFO_getByID(string _OfficeID, string _MFO_ID, string _SI_ID)
        {
            var query = from viewOPCR in _db.vSPMS_nMFO_ALL
                        where viewOPCR.officeId == _OfficeID & viewOPCR.MFOId == _MFO_ID & viewOPCR.indicatorId == _SI_ID
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
        public ActionResult edit_targetShared(vSPMS_nMFO_ALL MFOdata)
        {

            var dpcr = _db.tSPMS_DPCR.Where(a => a.targetId == MFOdata.targetId && a.divisionId == MFOdata.divisionId).FirstOrDefault();

            var data = _db.tSPMS_TargetCountSI.Where(a => a.targetId == dpcr.targetIdparent && a.officeId == dpcr.officeId).FirstOrDefault();

            return Json(data, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult update_targetShared(tSPMS_TargetCountSI ParentTgt, tSPMS_DPCR ChildTgt, int Committed)
        {
            var _parentTgt = _db.tSPMS_TargetCountSI.Where(a => a.targetId == ParentTgt.targetId & a.officeId == ParentTgt.officeId).FirstOrDefault();
            var _childTgt = _db.tSPMS_TargetCountSI.Where(a => a.targetId == ChildTgt.targetId & a.officeId == ChildTgt.officeId).FirstOrDefault();

            var totalTgt = _parentTgt.tRemaining + _childTgt.target;
            var totalRemaining = totalTgt - Committed;

            if (totalRemaining < 0)
            {
                //error
            }
            else
            {
                _parentTgt.tRemaining = totalRemaining;


                if (Committed > _childTgt.target)
                {
                    var _trem = Committed - _childTgt.target;
                    _childTgt.tRemaining = _childTgt.tRemaining + _trem;

                }
                else
                {
                    var _trem = _childTgt.target - Committed;
                    _childTgt.tRemaining = _childTgt.tRemaining - _trem;
                }
                _childTgt.target = Committed;
            }




            quan[4] = (int)Math.Round((((double)Committed * 0.3) + (double)Committed));
            quan[3] = (int)Math.Round((((double)Committed * 0.15) + (double)Committed));
            quan[2] = (int)((double)Committed);
            quan[1] = (int)Math.Round((((double)Committed / 2) + 1));
            quan[0] = (int)Math.Round((double)(quan[1] - 1));

            for (var i = 5; i >= 1; i--)
            {
                var standard = _db.tSPMS_MFOStandard.Where(a => a.targetId == _childTgt.targetId && a.rating == i.ToString()).FirstOrDefault();
                standard.quantity = quan[i - 1].ToString();
            }

            _db.SaveChanges();

            return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult save_targetShared(vSPMS_nMFO_ALL TgtData, int Committed, String _OfficeID, String _DivisionID)
        {
            string randomLetters = new string(Enumerable.Repeat(chars, 6)
     .Select(s => s[random.Next(s.Length)]).ToArray());
            string randomLNumbers = new string(Enumerable.Repeat(nums, 6)
        .Select(s => s[random.Next(s.Length)]).ToArray());

            var targetID = "TGT" + DateTime.Now.ToString("yyMMdd") + randomLetters.ToUpper() + randomLNumbers;
            var standardExist = _db.tSPMS_MFOStandard.Where(a => a.targetId == TgtData.targetId).ToList();

            var nTargetData = new tSPMS_TargetCountSI()
            {
                indicatorId = TgtData.indicatorId,
                officeId = _OfficeID,
                targetId = targetID,
                target = Committed,
                targetUnit = TgtData.targetUnit,
                tRemaining = Committed,
                targetTypeId = 0
            };
            _db.tSPMS_TargetCountSI.Add(nTargetData);

            var assignData = new tSPMS_DPCR()
            {
                indicatorId = TgtData.indicatorId,
                officeId = _OfficeID,
                divisionId = _DivisionID,
                targetId = targetID,
                targetIdparent = TgtData.targetId,
                categoryId = TgtData.categoryId,
            };
            _db.tSPMS_DPCR.Add(assignData);

            var updatedTgt = _db.tSPMS_TargetCountSI.Where(a => a.targetId == TgtData.targetId).FirstOrDefault();
            updatedTgt.tRemaining = updatedTgt.tRemaining - Committed;

            if (standardExist != null)
            {

                quan[4] = (int)Math.Round((((double)Committed * 0.3) + (double)Committed));
                quan[3] = (int)Math.Round((((double)Committed * 0.15) + (double)Committed));
                quan[2] = (int)((double)Committed);
                quan[1] = (int)Math.Round((((double)Committed / 2) + 1));
                quan[0] = (int)Math.Round((double)(quan[1] - 1));

                foreach (var ndata in standardExist)
                {
                    var newStandard = new tSPMS_MFOStandard()
                    {
                        targetId = targetID,
                        MFOId = TgtData.MFOId,
                        indicatorId = TgtData.indicatorId,
                        quantity = quan[int.Parse(ndata.rating) - 1].ToString(),
                        quality = ndata.quality,
                        timeliness = ndata.timeliness,
                        rating = ndata.rating
                    };
                    _db.tSPMS_MFOStandard.Add(newStandard);

                }
            }



            _db.SaveChanges();


            return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

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





        [HttpPost]
        public ActionResult MFO_getOPCRData(string _OfficeID)
        {
            var data = (from viewOPCR in _db.vprt_OPCR
                        where viewOPCR.officeId == _OfficeID & viewOPCR.programTypeId == 0
                        select viewOPCR).ToList();


            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]

        public JsonResult cookiestarget(string officeId, int prt_type)
        {
            string temp = "";
            HttpCookie cookiname = new HttpCookie("targetCrookie")
            {
                Value = officeId + ","

            };
            temp = cookiname.Name;
            Response.Cookies.Add(cookiname);

            return Json(new { data = "success" });
        }

        //==========================

        [HttpPost]
        public ActionResult NewAssign(vSPMS_nMFO_ALL Assign, String _OfficeID, String _DivisionID, String IsHrtgt)
        {
            var OfficeID = "";
            var canupdate = 0;
            var text = 0;
            var isExist = _db.tSPMS_DPCR.Where(a => a.indicatorId == Assign.indicatorId & a.officeId == _OfficeID & a.divisionId == _DivisionID).FirstOrDefault();

            if (isExist == null)
            {
                if (IsHrtgt == "1")
                {
                    OfficeID = _OfficeID;
                }
                else
                {
                    if (Assign.officeId == null)
                    {
                        OfficeID = _OfficeID;
                    }
                    else
                    {
                        OfficeID = Assign.officeId;
                    }
                }

                if (Assign.divisionId == null)
                {
                    var assignData = new tSPMS_DPCR()
                    {
                        indicatorId = Assign.indicatorId,
                        officeId = OfficeID,
                        divisionId = _DivisionID,
                        targetId = Assign.targetId,
                        categoryId = Assign.categoryId,
                    };
                    _db.tSPMS_DPCR.Add(assignData);
                }
                if (Assign.divisionId != null & Assign.isCMFO == null)
                {
                    var assignData = new tSPMS_DPCR()
                    {
                        indicatorId = Assign.indicatorId,
                        officeId = OfficeID,
                        divisionId = _DivisionID,
                        targetId = Assign.targetId,
                        categoryId = Assign.categoryId,
                        canUpdate = 1
                    };
                    _db.tSPMS_DPCR.Add(assignData);
                }
                if (Assign.divisionId != null & Assign.isCMFO == 1)
                {
                    var assignData = new tSPMS_DPCR()
                    {
                        indicatorId = Assign.indicatorId,
                        officeId = OfficeID,
                        divisionId = _DivisionID,
                        targetId = Assign.targetId,
                        categoryId = Assign.categoryId,
                    };
                    _db.tSPMS_DPCR.Add(assignData);
                }
                text = 1;

            }
            else
            {
                text = 2;
            }



            _db.SaveChanges();

            return Json(new { status = text }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult removeIndicator(vSPMS_nMFO_ALL indicator)
        {
            var data = _db.tSPMS_DPCR.Where(a => a.indicatorId == indicator.indicatorId & a.officeId == indicator.officeId & a.divisionId == indicator.divisionId).FirstOrDefault();
            var childTgt = _db.tSPMS_TargetCountSI.Where(a => a.targetId == indicator.targetId && a.officeId == indicator.officeId).FirstOrDefault();

            _db.tSPMS_DPCR.Remove(data);

            //UPDATE PARENT TARGET
            if (data.targetIdparent != null)
            {
                var parentTgt = _db.tSPMS_TargetCountSI.Where(a => a.targetId == data.targetIdparent && a.officeId == indicator.officeId).FirstOrDefault();
                parentTgt.tRemaining = parentTgt.tRemaining + childTgt.target;
                _db.tSPMS_TargetCountSI.Remove(childTgt);

                for (int index = 5; index >= 1; index--)
                {
                    var del = _db.tSPMS_MFOStandard.Where(a => a.targetId == indicator.targetId && a.rating == index.ToString()).FirstOrDefault();
                    _db.tSPMS_MFOStandard.Remove(del);

                }
            }

            _db.SaveChanges();
            return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult NewUnassign(vSPMS_nMFO_ALL Unassign, String _OfficeID, String _DivisionID)
        {

            var isShared = 0;
            var data = _db.tSPMS_DPCR.Where(a => a.indicatorId == Unassign.indicatorId && a.officeId == _OfficeID && a.divisionId == _DivisionID).FirstOrDefault();
            var childTgt = _db.tSPMS_TargetCountSI.Where(a => a.targetId == Unassign.targetId && a.officeId == _OfficeID).FirstOrDefault();

            //UPDATE PARENT TARGET
            if (data.targetIdparent != null)
            {
                var parentTgt = _db.tSPMS_TargetCountSI.Where(a => a.targetId == data.targetIdparent && a.officeId == _OfficeID).FirstOrDefault();
                parentTgt.tRemaining = parentTgt.tRemaining + childTgt.target;
                _db.tSPMS_TargetCountSI.Remove(childTgt);

                for (int index = 5; index >= 1; index--)
                {
                    var del = _db.tSPMS_MFOStandard.Where(a => a.targetId == Unassign.targetId && a.rating == index.ToString()).FirstOrDefault();
                    _db.tSPMS_MFOStandard.Remove(del);

                }

                isShared = 1;


            }



            _db.tSPMS_DPCR.Remove(data);


            _db.SaveChanges();

            return Json(new { status = 1, isShared }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult viewStandard(String TargetId)
        {

            var standard = _db.tSPMS_MFOStandard.Where(a => a.targetId == TargetId).ToList();

            _db.SaveChanges();
            return Json(standard, JsonRequestBehavior.AllowGet);
        }

        //=========================
        [HttpPost]
        public ActionResult MFO_addCL(String MFOID, vSPMS_nMFO_ALL newMFO, List<tSPMS_MFOStandard> newStandard, String _OfficeID)
        {
            string randomLetters = new string(Enumerable.Repeat(chars, 6)
             .Select(s => s[random.Next(s.Length)]).ToArray());
            string randomLNumbers = new string(Enumerable.Repeat(nums, 6)
             .Select(s => s[random.Next(s.Length)]).ToArray());

            //var MFO_id = "MFO" + DateTime.Now.ToString("yyMMdd") + randomLetters.ToUpper() + randomLNumbers;
            var indicator_id = "IND" + DateTime.Now.ToString("yyMMdd") + randomLetters.ToUpper() + randomLNumbers;
            var target_id = "TGT" + DateTime.Now.ToString("yyMMdd") + randomLetters.ToUpper() + randomLNumbers;
            var CL_id = "CL" + DateTime.Now.ToString("yyMMdd") + randomLetters.ToUpper() + randomLNumbers;


            var addCheckList = new tSPMS_CheckListMFO()
            {
                MFOId = MFOID,
                CLId = CL_id,
                CLDesc = newMFO.MFO

            };
            _db.tSPMS_CheckListMFO.Add(addCheckList);

            var addSIData = new tAppropriationProjMFOInd()
            {
                MFOId = CL_id,
                indicatorId = indicator_id,
                indicator = newMFO.indicator,
                //target = newSI.target,
                targetUnit = "N",
                targetTypeId = 0,
                isActive = 0,

            };
            _db.tAppropriationProjMFOInds.Add(addSIData);

            var addTargetCount = new tSPMS_TargetCountSI()
            {
                target = newMFO.target,
                tRemaining = newMFO.target,
                targetId = target_id,
                targetUnit = newMFO.targetUnit,
                indicatorId = indicator_id,
                officeId = _OfficeID
            };
            _db.tSPMS_TargetCountSI.Add(addTargetCount);

            foreach (var i in newStandard)
            {

                var addStandard = new tSPMS_MFOStandard()
                {
                    targetId = target_id,
                    MFOId = CL_id,
                    indicatorId = indicator_id,
                    rating = i.rating,
                    quantity = i.quantity,
                    quality = i.quality,
                    timeliness = i.timeliness
                };
                _db.tSPMS_MFOStandard.Add(addStandard);



            }
            _db.SaveChanges();

            var getObj = _db.vSPMS_MFOwCat.Where(a => a.MFOId == MFOID).ToList();

            return Json(new { data = 1, objCMFO = getObj }, JsonRequestBehavior.AllowGet);
            //return Json(new { data = 1 }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MFO_add(tAppropriationProjMFO newMFO, tSPMS_MFOCategory catMFO, tAppropriationProjMFOInd newSI, List<tSPMS_MFOStandard> newStandard, String _OfficeID, String _DivisionID, bool isCheck)
        {
            string randomLetters = new string(Enumerable.Repeat(chars, 6)
             .Select(s => s[random.Next(s.Length)]).ToArray());
            string randomLNumbers = new string(Enumerable.Repeat(nums, 6)
             .Select(s => s[random.Next(s.Length)]).ToArray());

            var MFO_id = "MFO" + DateTime.Now.ToString("yyMMdd") + randomLetters.ToUpper() + randomLNumbers;
            var indicator_id = "IND" + DateTime.Now.ToString("yyMMdd") + randomLetters.ToUpper() + randomLNumbers;
            var target_id = "TGT" + DateTime.Now.ToString("yyMMdd") + randomLetters.ToUpper() + randomLNumbers;

            var isExistMFO = _db.vSPMS_nMFO_ALL.Where(a => a.MFO == newMFO.MFO && a.divisionId == _DivisionID).FirstOrDefault();


            List<vSPMS_nMFO_ALL> getObj = new List<vSPMS_nMFO_ALL>();
            var status = 1;

            if (isExistMFO == null)
            {
                var newMFOData = new tAppropriationProjMFO()
                {
                    MFOId = MFO_id,
                    MFO = newMFO.MFO,
                    isActive = 0
                };
                _db.tAppropriationProjMFOes.Add(newMFOData);

                var addMFOCategory = new tSPMS_MFOCategory()
                {
                    MFOId = MFO_id,
                    categoryId = catMFO.categoryId,
                    divisionId = _DivisionID,
                    officeId = _OfficeID
                };
                _db.tSPMS_MFOCategory.Add(addMFOCategory);

                var addSIData = new tAppropriationProjMFOInd()
                {
                    MFOId = MFO_id,
                    indicatorId = indicator_id,
                    indicator = newSI.indicator,
                    //target = newSI.target,
                    targetUnit = "N",
                    targetTypeId = 0,
                    isActive = 0,

                };
                _db.tAppropriationProjMFOInds.Add(addSIData);

                var _target = newSI.target;
                if (newSI.target == null)
                {
                    _target = 0;
                }

                var addTargetCount = new tSPMS_TargetCountSI()
                {
                    target = _target,
                    tRemaining = _target,
                    targetId = target_id,
                    indicatorId = indicator_id,
                    targetUnit = newSI.targetUnit,
                    officeId = _OfficeID
                };
                _db.tSPMS_TargetCountSI.Add(addTargetCount);

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
                _db.SaveChanges();

                getObj = _db.vSPMS_nMFO_ALL.Where(a => a.MFOId == MFO_id).ToList();

                if (isCheck)
                {
                    var assign = _db.vSPMS_nMFO_ALL.Where(a => a.MFOId == MFO_id && a.indicatorId == indicator_id).FirstOrDefault();
                    NewAssign(assign, _OfficeID, _DivisionID, null);
                }


            }
            else
            {
                status = 0;
            }



            return Json(new { data = status, objMFO = getObj }, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult MFO_getMFOperId(String MFOId)
        {
            List<vSPMS_nMFO_ALL> getObj = null;
            var data = 0;
            var hasChecklist = _db.tSPMS_CheckListMFO.Where(a => a.MFOId == MFOId).FirstOrDefault();

            if (hasChecklist == null)
            {
                getObj = _db.vSPMS_nMFO_ALL.Where(a => a.MFOId == MFOId).ToList();
                data = 1;
            }

            return Json(new { status = data, objMFO = getObj }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MFO_moreSI(tAppropriationProjMFO getMFO, tAppropriationProjMFOInd newSI, List<tSPMS_MFOStandard> newStandard, String _OfficeID, bool isCheck)
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

            var addTargetCount = new tSPMS_TargetCountSI()
            {
                target = newSI.target,
                tRemaining = newSI.target,
                targetId = target_id,
                indicatorId = indicator_id,
                targetUnit = newSI.targetUnit,
                officeId = _OfficeID
            };
            _db.tSPMS_TargetCountSI.Add(addTargetCount);

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

            var getObj = _db.vSPMS_nMFO_ALL.Where(a => a.MFOId == getMFO.MFOId).ToList();

            if (isCheck)
            {
                var assign = _db.vSPMS_nMFO_ALL.Where(a => a.MFOId == getMFO.MFOId && a.indicatorId == indicator_id).FirstOrDefault();
                NewAssign(assign, _OfficeID, assign.divisionId, null);
            }

            return Json(new { data = 1, objMFO = getObj }, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult MFO_Standard(tSPMS_MFOStandard mfoindId)
        {
            var data = _db.tSPMS_MFOStandard.Where(a => a.MFOId == mfoindId.MFOId & a.indicatorId == mfoindId.indicatorId & a.targetId == mfoindId.targetId).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult MFO_UpdateStandard(List<tSPMS_MFOStandard> objStandard)
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
        public ActionResult MFO_getMFO_allSI(vSPMS_nMFO_ALL mfoData)
        {
            var data = _db.vSPMS_nMFO_ALL.Where(a => a.MFOId == mfoData.MFOId & a.officeId == mfoData.officeId).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MFO_showMFO_SI(vSPMS_nMFO_ALL mfoData)
        {
            var data = _db.vSPMS_nMFO_ALL.Where(a => a.MFOId == mfoData.MFOId & a.officeId == mfoData.officeId & a.indicatorId == mfoData.indicatorId).FirstOrDefault();
            var standard = _db.tSPMS_MFOStandard.Where(a => a.targetId == mfoData.targetId).ToList();
            return Json(new { data, standard }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult MFO_showMFO_SI2(vSPMS_nMFO_ALL mfoData, String DivisionID)
        {
            var data = _db.vSPMS_nMFO_ALL.Where(a => a.MFOId == mfoData.MFOId & a.divisionId == DivisionID & a.indicatorId == mfoData.indicatorId).FirstOrDefault();
            var standard = _db.tSPMS_MFOStandard.Where(a => a.targetId == data.targetId).ToList();
            return Json(new { data, standard }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MFO_updateMFO_allSI(vSPMS_nMFO_ALL mfoData, List<tSPMS_MFOStandard> standard)
        {

            if (mfoData.target == 0 | String.IsNullOrEmpty(mfoData.target.ToString()))
            {
                quan[4] = 0;
                quan[3] = 0;
                quan[2] = 0;
                quan[1] = 0;
                quan[0] = 0;
                var update = _db.tSPMS_DPCR.Where(a => a.indicatorId == mfoData.indicatorId & a.officeId == mfoData.officeId).FirstOrDefault();
                if (update != null)
                {
                    _db.tSPMS_DPCR.Remove(update);

                }
            }
            else
            {
                quan[4] = (int)(((double)mfoData.target * 0.3) + (double)mfoData.target);
                quan[3] = (int)(((double)mfoData.target * 0.15) + (double)mfoData.target);
                quan[2] = (int)((double)mfoData.target);
                quan[1] = (int)(((double)mfoData.target / 2) + 1);
                quan[0] = (int)(quan[1] - 1);
            }


            var updateMFO = _db.tAppropriationProjMFOes.FirstOrDefault(a => a.MFOId == mfoData.MFOId);
            updateMFO.MFO = mfoData.MFO;

            if (mfoData.target == 0 | mfoData.target.ToString() == "")
            {

                var updateTarget = _db.tSPMS_TargetCountSI.FirstOrDefault(a => a.targetId == mfoData.targetId);
                updateTarget.target = 0;
                updateTarget.tRemaining = 0;
                updateTarget.targetUnit = mfoData.targetUnit;


            }
            else
            {
                var updateTarget = _db.tSPMS_TargetCountSI.FirstOrDefault(a => a.targetId == mfoData.targetId);
                updateTarget.target = mfoData.target;
                updateTarget.tRemaining = mfoData.target;
                updateTarget.targetUnit = mfoData.targetUnit;


            }


            var updateSI = _db.tAppropriationProjMFOInds.Where(a => a.indicatorId == mfoData.indicatorId & a.MFOId == mfoData.MFOId).FirstOrDefault();
            updateSI.indicator = mfoData.indicator;


            var updateCat = _db.tSPMS_MFOCategory.Where(a => a.MFOId == mfoData.MFOId).FirstOrDefault();
            if (updateCat != null)
            {
                if (mfoData.categoryId != null)
                {
                    updateCat.categoryId = mfoData.categoryId;
                    var updateDpcrCat = _db.tSPMS_DPCR.Where(a => a.indicatorId == mfoData.indicatorId & a.officeId == mfoData.officeId & a.divisionId == mfoData.divisionId).FirstOrDefault();
                    if (updateDpcrCat != null)
                    {
                        updateDpcrCat.categoryId = mfoData.categoryId;
                    }

                }
                else
                {
                    //text = " empty ang catId";
                }
            }
            else
            {
                var addCat = new tSPMS_MFOCategory()
                {
                    MFOId = mfoData.MFOId,
                    categoryId = mfoData.categoryId
                };
                _db.tSPMS_MFOCategory.Add(addCat);
            }


            foreach (var item in standard)
            {
                var update = _db.tSPMS_MFOStandard.Where(a => a.rating == item.rating & a.MFOId == item.MFOId & a.indicatorId == item.indicatorId).ToList();
                foreach (var i in update)
                {
                    i.quantity = item.quantity;
                    i.quality = item.quality;
                    i.timeliness = item.timeliness;
                }

            }



            _db.SaveChanges();
            return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult MFO_checkList(String MFOID)
        {
            var cl = _db.tAppropriationProjMFOInds.Where(a => a.MFOId == MFOID).ToList();
            var cmfo = _db.vSPMS_CommonMFO.Where(a => a.MFOId == MFOID).ToList();
            var count = 0;
            if (cl.Count() > 1 || cmfo.Count() > 2)
            {
                count = 1;
            }
            return Json(new { status = count }, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult getCLstandard(vSPMS_CheckListMFO CLData)
        {
            var standard = _db.tSPMS_MFOStandard.Where(a => a.targetId == CLData.targetId).ToList();
            var clmfo = _db.vSPMS_CheckListMFO.Where(a => a.indicatorId == CLData.indicatorId & a.officeId == a.officeId & a.divisionId == CLData.divisionId).FirstOrDefault();
            return Json(new { standard, clmfo, }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult updateCheckList(vSPMS_CheckListMFO CLData, List<tSPMS_MFOStandard> CLStandard)
        {
            var updateCLDesc = _db.tSPMS_CheckListMFO.Where(a => a.MFOId == CLData.MFOId && a.CLId == CLData.CLId).FirstOrDefault();
            if (updateCLDesc != null)
            {
                updateCLDesc.CLDesc = CLData.CLDesc;

                var updateIndicator = _db.tAppropriationProjMFOInds.Where(a => a.indicatorId == CLData.indicatorId).FirstOrDefault();
                updateIndicator.indicator = CLData.indicator;

                var updateTarget = _db.tSPMS_TargetCountSI.Where(a => a.targetId == CLData.targetId).FirstOrDefault();
                updateTarget.target = CLData.target;
                updateTarget.tRemaining = CLData.target;
                updateTarget.targetUnit = CLData.targetUnit;

            }
            foreach (var item in CLStandard)
            {
                var update = _db.tSPMS_MFOStandard.Where(a => a.rating == item.rating & a.MFOId == item.MFOId & a.indicatorId == item.indicatorId).ToList();
                foreach (var i in update)
                {
                    i.quantity = item.quantity;
                    i.quality = item.quality;
                    i.timeliness = item.timeliness;
                }

            }

            _db.SaveChanges();
            return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MFO_showAddtgt(vSPMS_CommonMFO CMFO, String _OfficeID, String _DivisionID)
        {
            List<tSPMS_MFOStandard> standard = new List<tSPMS_MFOStandard>();

            var cmfo = _db.vSPMS_CommonMFO.Where(a => a.MFOId == CMFO.MFOId & a.indicatorId == CMFO.indicatorId & a.officeId == _OfficeID & a.divisionId == _DivisionID).FirstOrDefault();
            var catcmfo = _db.vSPMS_CommonMFO.Where(a => a.MFOId == CMFO.MFOId & a.officeId == _OfficeID & a.divisionId == _DivisionID).FirstOrDefault();

            if (cmfo != null)
            {
                standard = _db.tSPMS_MFOStandard.Where(a => a.targetId == cmfo.targetId).ToList();

            }
            return Json(new { status = 1, cmfo, catcmfo, standard }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CMFO_addTargetCat(vSPMS_CommonMFO cmfoes, List<tSPMS_MFOStandard> standardData, String _OfficeID, String _DivisionID)
        {

            var test = "";

            string randomLetters = new string(Enumerable.Repeat(chars, 6)
            .Select(s => s[random.Next(s.Length)]).ToArray());
            string randomLNumbers = new string(Enumerable.Repeat(nums, 6)
             .Select(s => s[random.Next(s.Length)]).ToArray());


            var target_id = "TGT" + DateTime.Now.ToString("yyMMdd") + randomLetters.ToUpper() + randomLNumbers;
            var isExist = _db.tSPMS_CommonMFO.Where(a => a.MFOId == cmfoes.MFOId & a.officeId == _OfficeID & a.targetId == cmfoes.catTargetId & a.divisionId == _DivisionID).FirstOrDefault();
            //  var isCreated = _db.tSPMS_TargetCountSI.FirstOrDefault(a => a.indicatorId == cmfoes.indicatorId & a.officeId == _OfficeID & a.divisionId == _DivisionID);


            if (isExist == null)
            {
                var add = new tSPMS_CommonMFO()
                {
                    MFOId = cmfoes.MFOId,
                    indicatorId = cmfoes.indicatorId,
                    officeId = _OfficeID,
                    divisionId = _DivisionID,
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
                /*  quan[4] = (int)(((double)cmfoes.target * 0.3) + (double)cmfoes.target);
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
            else
            {
                var updateTarget = _db.tSPMS_TargetCountSI.FirstOrDefault(a => a.targetId == cmfoes.catTargetId);
                updateTarget.target = cmfoes.target;
                updateTarget.tRemaining = cmfoes.target;
                updateTarget.targetUnit = cmfoes.targetUnit;



                var listCat = _db.tSPMS_CommonMFO.Where(a => a.MFOId == cmfoes.MFOId & a.officeId == _OfficeID & a.divisionId == _DivisionID).ToList();

                foreach (var i in listCat)
                {
                    i.categoryId = cmfoes.categoryId;
                }
                foreach (var i in standardData)
                {
                    var standard = _db.tSPMS_MFOStandard.Where(a => a.targetId == cmfoes.catTargetId & a.rating == i.rating).FirstOrDefault();

                    standard.quantity = i.quantity;

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
            }


            if (cmfoes.targetId == null) // insert data if new
            {
                var target = new tSPMS_TargetCountSI()
                {
                    targetId = target_id,
                    indicatorId = cmfoes.indicatorId,
                    divisionId = _DivisionID,
                    target = cmfoes.target,
                    tRemaining = cmfoes.target,
                    targetUnit = cmfoes.targetUnit,
                    officeId = _OfficeID

                };
                _db.tSPMS_TargetCountSI.Add(target);

            }
            _db.SaveChanges();

            return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);
        }

    }
}
