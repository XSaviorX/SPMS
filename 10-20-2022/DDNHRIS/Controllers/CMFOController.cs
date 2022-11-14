using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DDNHRIS.Models.SPMS;

namespace DDNHRIS.Controllers
{
    public class CMFOController : Controller
    {
        SPMSDBEntities7 _db = new SPMSDBEntities7();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string nums = "0123456789";
        Random random = new Random();

        int[] quan = new int[5];
        // GET: CMFO
        public ActionResult Index()
        {
            return View();
        }
        //public ActionResult CMFO_get()
        //{
        //    var data = _db.vCMFOInds.ToList();

        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}
        //[HttpPost]
        //public ActionResult CMFO_getperOffice(string officeID)
        //{
        //    var data = _db.vCMFOInds.Where(a => a.officeId == officeID).ToList();

        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}
        //[HttpPost]
        //public ActionResult CMFO_insert(tCommonMFO _CMFO)
        //{
        //    string randomLetters = new string(Enumerable.Repeat(chars, 6)
        //.Select(s => s[random.Next(s.Length)]).ToArray());
        //    string randomLNumbers = new string(Enumerable.Repeat(nums, 6)
        //.Select(s => s[random.Next(s.Length)]).ToArray());

        //    var CMFO_id = "CMFO" + DateTime.Now.ToString("yyMMdd") + randomLetters.ToUpper() + randomLNumbers;

        //    var data = _db.tCommonMFOes.FirstOrDefault(a => a.CMFOId == CMFO_id);

        //    var newMFOData = new tCommonMFO()
        //    {
        //        CMFOId = CMFO_id,
        //        CMFO = _CMFO.CMFO,

        //        isActive = 0
        //    };
        //    _db.tCommonMFOes.Add(newMFOData);
        //    _db.SaveChanges();

        //    return Json(new { status = CMFO_id }, JsonRequestBehavior.AllowGet);
        //}
        //[HttpPost]
        //public ActionResult CMFO_SIinsert(List<tCommonMFOInd> indicators, string CMFO_ID)
        //{

        //    foreach (var ndata in indicators)
        //    {
        //        string randomLetters = new string(Enumerable.Repeat(chars, 6)
        //        .Select(s => s[random.Next(s.Length)]).ToArray());
        //        string randomLNumbers = new string(Enumerable.Repeat(nums, 6)
        //        .Select(s => s[random.Next(s.Length)]).ToArray());

        //        var si_id = "IND" + DateTime.Now.ToString("yyMMdd") + randomLetters.ToUpper() + randomLNumbers;
        //        var pfmc_id = "PFMC" + DateTime.Now.ToString("yyMMdd") + randomLetters.ToUpper() + randomLNumbers;
        //        var newCMFO_SI_Data = new tCommonMFOInd()
        //        {
        //            indicatorId = si_id,
        //            CMFOId = CMFO_ID,
        //            indicator = ndata.indicator,
        //            target = ndata.target,
        //            targetUnit = "N",
        //            targetTypeId = 0,
        //            officeId = "OFFPBOEZ7SC4ZA9",
        //            performanceId = pfmc_id,
        //            isActive = 1
        //        };
            
        //        tCommonMFOInd tCommonMFOInd = _db.tCommonMFOInds.Add(newCMFO_SI_Data);

        //        quan[4] = (int)(((double)ndata.target * 0.3) + (double)ndata.target);
        //        quan[3] = (int)(((double)ndata.target * 0.15) + (double)ndata.target);
        //        quan[2] = (int)((double)ndata.target);
        //        quan[1] = (int)(((double)ndata.target / 2) + 1);
        //        quan[0] = (int)(quan[1] - 1);

        //        for (var num = 5; num >= 1; num--)
        //        {
        //            var newSI_performance = new tCMFOPerformance()
        //            {
        //                performanceId = pfmc_id,
        //                rating = num.ToString(),
        //                quantity = quan[num - 1].ToString(),
        //            };
        //            _db.tCMFOPerformances.Add(newSI_performance);
        //        }
        //    }
        //    _db.SaveChanges();

        //    return Json(new { status = CMFO_ID }, JsonRequestBehavior.AllowGet);
        //}
        //[HttpPost]
        //public ActionResult CMFOInd_edit(String CMFOId) 
        //{
        //    var data = _db.vCMFOInds.Where(a => a.CMFOId == CMFOId).ToList();
        //    return Json(data, JsonRequestBehavior.AllowGet);

        //}
        //[HttpPost]
        //public ActionResult GetPerformancePer(String performanceId)
        //{
        //    var data = _db.tCMFOPerformances.Where(a => a.performanceId == performanceId).ToList();
        //    return Json(data, JsonRequestBehavior.AllowGet);

        //}

        //[HttpPost]
        //public ActionResult AddPerformance(List<tCMFOPerformance> tCMFOPerformance)
        //{
        //    var isSaved = "";


        //    foreach (var i in tCMFOPerformance)
        //    {
        //        var ifExist = _db.tCMFOPerformances.Where(a => a.performanceId == i.performanceId).FirstOrDefault();
        //        if (ifExist == null)
        //        {
        //            _db.tCMFOPerformances.Add(i);
        //            isSaved = "success";

        //        }
        //        else
        //        {
        //            var update = _db.tCMFOPerformances.Where(a => a.recNo == i.recNo & a.performanceId == i.performanceId).FirstOrDefault();
        //            update.quantity = i.quantity;
        //            update.quality = i.quality;
        //            update.timeliness = i.timeliness;
        //            isSaved = "updated";
        //        }

        //    }

        //    _db.SaveChanges();
        //    return Json(new { isSaved }, JsonRequestBehavior.AllowGet);

        //}

        //[HttpPost]
        //public ActionResult GetPerformance()
        //{
        //    var data = _db.tCMFOPerformances.ToList();
        //    return Json(data, JsonRequestBehavior.AllowGet);

        //}

        //[HttpPost]
        //public ActionResult updateAssigned(vCMFOInd ndata)
        //{
        //    var text = "sdd";

        //    string randomLetters = new string(Enumerable.Repeat(chars, 6)
        //   .Select(s => s[random.Next(s.Length)]).ToArray());
        //    string randomLNumbers = new string(Enumerable.Repeat(nums, 6)
        //    .Select(s => s[random.Next(s.Length)]).ToArray());

        //    var CMFO_id = "CMFO" + DateTime.Now.ToString("yyMMdd") + randomLetters.ToUpper() + randomLNumbers;
        //    var si_id = "IND" + DateTime.Now.ToString("yyMMdd") + randomLetters.ToUpper() + randomLNumbers;
        //    var pfmc_id = "PFMC" + DateTime.Now.ToString("yyMMdd") + randomLetters.ToUpper() + randomLNumbers;

         
        //    var isExist = _db.vCMFOInds.Where(a => a.CMFO == ndata.CMFO & a.indicator == ndata.indicator & a.officeId == "OFFPBOEZ7SC4ZA9").FirstOrDefault();
        //    var isExistCMFO_Offc = _db.vCMFOInds.Where(a => a.CMFO == ndata.CMFO & a.officeId == "OFFPBOEZ7SC4ZA9").FirstOrDefault();
        //    var isExistPfrmc = _db.tCMFOPerformances.Where(a => a.performanceId == ndata.performanceId).ToList();
        //    var isExistCMFOIND = _db.tCommonMFOInds.Where(a => a.CMFOId == ndata.CMFOId & a.indicatorId == ndata.indicatorId & a.officeId == "OFFPBOEZ7SC4ZA9").FirstOrDefault();
         


        //    if (isExist == null)
        //    {
        //        if (isExistCMFO_Offc == null)
        //        {
        //            var newCMFOData = new tCommonMFO()
        //            {
        //                CMFOId = CMFO_id,
        //                CMFO = ndata.CMFO,
        //                isActive = 0
        //            };
        //            _db.tCommonMFOes.Add(newCMFOData);

        //            var newSIdata = new tCommonMFOInd()
        //            {
        //                CMFOId = CMFO_id,
        //                indicatorId = si_id,
        //                indicator = ndata.indicator,
        //                target = ndata.target,
        //                targetUnit = "N",
        //                targetTypeId = 0,
        //                officeId = "OFFPBOEZ7SC4ZA9",
        //                performanceId = pfmc_id,
        //                isActive = 1,

        //            };
        //            _db.tCommonMFOInds.Add(newSIdata);

        //            if(isExistPfrmc != null) //performance not exist
        //            {
        //                foreach(var items in isExistPfrmc)
        //                {
        //                    var add_pfrmnc = new tCMFOPerformance()
        //                    {
        //                        performanceId = pfmc_id,
        //                        rating = items.rating,
        //                        quantity = items.quantity,
        //                        quality = items.quality,
        //                        timeliness = items.timeliness
        //                    };
        //                    _db.tCMFOPerformances.Add(add_pfrmnc);

        //                }
                        
                       
        //            }
        //            else
        //            {
        //                quan[4] = (int)(((double)ndata.target * 0.3) + (double)ndata.target);
        //                quan[3] = (int)(((double)ndata.target * 0.15) + (double)ndata.target);
        //                quan[2] = (int)((double)ndata.target);
        //                quan[1] = (int)(((double)ndata.target / 2) + 1);
        //                quan[0] = (int)(quan[1] - 1);

        //                for (var num = 5; num >= 1; num--)
        //                {
        //                    var newSI_performance = new tCMFOPerformance()
        //                    {
        //                        performanceId = pfmc_id,
        //                        rating = num.ToString(),
        //                        quantity = quan[num - 1].ToString(),
        //                    };
        //                    _db.tCMFOPerformances.Add(newSI_performance);
        //                }
        //            }
                 
        //            text = "2";
        //        }
        //        else
        //        {


        //            var newSIdata = new tCommonMFOInd()
        //            {
        //                CMFOId = isExistCMFO_Offc.CMFOId,
        //                indicatorId = si_id,
        //                indicator = ndata.indicator,
        //                target = ndata.target,
        //                targetUnit = "N",
        //                targetTypeId = 0,
        //                officeId = "OFFPBOEZ7SC4ZA9",
        //                performanceId = pfmc_id,
        //                isActive = 1,

        //            };
        //            _db.tCommonMFOInds.Add(newSIdata);

        //            if (isExistPfrmc != null)  //performance not exist
        //            {
        //                foreach (var items in isExistPfrmc)
        //                {
        //                    var add_pfrmnc = new tCMFOPerformance()
        //                    {
        //                        performanceId = pfmc_id,
        //                        rating = items.rating,
        //                        quantity = items.quantity,
        //                        quality = items.quality,
        //                        timeliness = items.timeliness
        //                    };
        //                    _db.tCMFOPerformances.Add(add_pfrmnc);

        //                }


        //            }
        //            else
        //            {
        //                quan[4] = (int)(((double)ndata.target * 0.3) + (double)ndata.target);
        //                quan[3] = (int)(((double)ndata.target * 0.15) + (double)ndata.target);
        //                quan[2] = (int)((double)ndata.target);
        //                quan[1] = (int)(((double)ndata.target / 2) + 1);
        //                quan[0] = (int)(quan[1] - 1);

        //                for (var num = 5; num >= 1; num--)
        //                {
        //                    var newSI_performance = new tCMFOPerformance()
        //                    {
        //                        performanceId = pfmc_id,
        //                        rating = num.ToString(),
        //                        quantity = quan[num - 1].ToString(),
        //                    };
        //                    _db.tCMFOPerformances.Add(newSI_performance);
        //                }
        //            }
        //            text = "2";
        //        }

        //    }
        //    else
        //    {
        //        if (ndata.officeId == "OFFPBOEZ7SC4ZA9")
        //        {
        //            isExistCMFOIND.isActive = ndata.isActive;
        //            if(ndata.isActive == 0)
        //            {
        //                text = "3"; //is removed
        //            }
        //            else
        //            {
        //                text = "2";//is Added
        //            }
                   
        //        }
        //        else
        //        {
        //            text = "1"; //CMFO AND SI is already exist
        //        }

        //    }



        //    _db.SaveChanges();
        //    return Json(new { isSaved = text }, JsonRequestBehavior.AllowGet);

        //}


        //[HttpPost]
        //public ActionResult MFO_updateMFO_allSI(List<vCMFOInd> _cmfoindicators, string _mfoDesc)
        //{
        //    var offId = _cmfoindicators[0].officeId;
        //    foreach (var items in _cmfoindicators)
        //    {
        //        string randomLetters = new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());
        //        string randomLNumbers = new string(Enumerable.Repeat(nums, 6).Select(s => s[random.Next(s.Length)]).ToArray());
        //        var si_id = "IND" + DateTime.Now.ToString("yyMMdd") + randomLetters.ToUpper() + randomLNumbers;
        //        var pfmc_id = "PFMC" + DateTime.Now.ToString("yyMMdd") + randomLetters.ToUpper() + randomLNumbers;

        //        if (items.indicatorId == null || items.indicatorId == "")
        //        {
        //            var newMFO_SI_Data = new tCommonMFOInd()
        //            {
        //                indicatorId = si_id,
        //                CMFOId = _cmfoindicators[0].CMFOId,
        //                indicator = items.indicator,
        //                target = items.target,
        //                officeId = offId,
        //                performanceId = pfmc_id,
        //                targetUnit = "N",
        //                targetTypeId = 0,
        //                isActive = 1
        //            };
        //            _db.tCommonMFOInds.Add(newMFO_SI_Data);

        //            quan[4] = (int)(((double)items.target * 0.3) + (double)items.target);
        //            quan[3] = (int)(((double)items.target * 0.15) + (double)items.target);
        //            quan[2] = (int)((double)items.target);
        //            quan[1] = (int)(((double)items.target / 2) + 1);
        //            quan[0] = (int)(quan[1] - 1);

        //            for (var num = 5; num >= 1; num--)
        //            {
        //                var newSI_performance = new tCMFOPerformance()
        //                {
        //                    performanceId = pfmc_id,
        //                    rating = num.ToString(),
        //                    quantity = quan[num - 1].ToString(),
        //                };
        //                _db.tCMFOPerformances.Add(newSI_performance);
        //            }
        //        }
        //        else
        //        {
        //            var updateMFO = _db.tCommonMFOes.FirstOrDefault(a => a.CMFOId == items.CMFOId);
        //            updateMFO.CMFO = _mfoDesc;

        //            quan[4] = (int)(((double)items.target * 0.3) + (double)items.target);
        //            quan[3] = (int)(((double)items.target * 0.15) + (double)items.target);
        //            quan[2] = (int)((double)items.target);
        //            quan[1] = (int)(((double)items.target / 2) + 1);
        //            quan[0] = (int)(quan[1] - 1);

        //            var updateSI = _db.tCommonMFOInds.FirstOrDefault(a => a.indicatorId == items.indicatorId & a.CMFOId == items.CMFOId);
        //            updateSI.indicator = items.indicator;
        //            updateSI.target = items.target;

        //            var update_performance = _db.tCMFOPerformances.FirstOrDefault(a => a.performanceId == items.performanceId);

        //            if (update_performance == null)
        //            {
        //                for (var num = 5; num >= 1; num--)
        //                {
        //                    var newData = new tCMFOPerformance()
        //                    {
        //                        performanceId = items.performanceId,
        //                        rating = num.ToString(),
        //                        quantity = quan[num - 1].ToString(),
        //                    };
        //                    _db.tCMFOPerformances.Add(newData);
        //                }
        //                _db.SaveChanges();
        //            }
        //            else
        //            {
                        

        //                for (var limit = 5; limit >= 1; limit--)
        //                {
        //                    var updateData = _db.tCMFOPerformances.FirstOrDefault(a => a.performanceId == items.performanceId & a.rating == limit.ToString());
        //                    {
        //                        updateData.quantity = quan[limit - 1].ToString();
        //                    }
        //                }
        //                _db.SaveChanges();
        //            }

        //        }
        //    }
        //    _db.SaveChanges();
        //    return Json(new { status = _cmfoindicators[0].officeId }, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public ActionResult removeIndicator(String indicatorId)
        //{
        //    var update = _db.tCommonMFOInds.Where(a => a.indicatorId == indicatorId).FirstOrDefault();
        //    update.isActive = 0;
        //    _db.SaveChanges();
        //    return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);
        //}
    }
}