using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DDNHRIS.Models;

namespace DDNHRIS.Controllers
{

    [SessionTimeout]
    [Authorize]
    public class ComptAssessmentToolController : Controller
    {

        HRISDBEntities db = new HRISDBEntities();

        // GET: ComptAssessmentTool
        public ActionResult Standard()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ComptPositionByOffice(string id)
        {
            string assmntGroupCode = id;
            IEnumerable<tLNDCompetencyPositionGroup> comptPositionList = db.tLNDCompetencyPositionGroups.Where(e => e.assmntGroupCode == assmntGroupCode).OrderByDescending(o => o.salaryGrade).ToList();
            return Json(new { status = "success", comptPositionList = comptPositionList }, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public JsonResult LoadPositionStandard(tLNDCompetencyPositionGroup data)
        {

            int assmntYear = 0; //APPLICANT ASSESSMENT

            IEnumerable<vLNDCompetencyStandard> stndrd = db.vLNDCompetencyStandards.Where(e => e.comptPositionCode == data.comptPositionCode).ToList();

            int leadTag = 0;
            int techTag = 0;
            if (stndrd != null)
            {
                leadTag = Convert.ToInt16(stndrd.Where(e => e.comptGroupCode == "LEAD").Count());
                techTag = Convert.ToInt16(stndrd.Where(e => e.comptGroupCode == "TECH").Count());
            }

            //CHECK IF CORE EXIST
            //int count = db.tLNDCompetencyStandards.Where(e => e.comptPositionCode == data.comptPositionCode && e.assessmentYear == assmntYear).Count();
            var stan = db.vLNDCompetencyStandards.Where(w => w.comptPositionCode == data.comptPositionCode && w.assessmentYear == assmntYear).ToList();
            
            if (stan.Count() == 0)
            {
                IEnumerable<tLNDCompetency> comptCore = db.tLNDCompetencies.Where(e => e.comptGroupCode == "CORE" && e.assessmentYear == assmntYear).OrderBy(o => o.orderNo).ToList();
                foreach (tLNDCompetency item in comptCore)
                {
                    tLNDCompetencyStandard n = new tLNDCompetencyStandard();
                    n.comptPositionCode = data.comptPositionCode;
                    n.comptCode = item.comptCode;
                    db.tLNDCompetencyStandards.Add(n);
                }
                db.SaveChanges();
            }
            

            var coreList = stan.Where(e => e.comptGroupCode == "CORE").ToList();
            var leadList = stan.Where(e => e.comptGroupCode == "LEAD").ToList();
            var techList = stan.Where(e => e.comptGroupCode == "TECH").ToList();

            return Json(new { status = "success", gData = stan, coreList = coreList, leadList = leadList, techList = techList }, JsonRequestBehavior.AllowGet);

        }


        /// *******************************************************************************************************
        /// <returns></returns> 
        public JsonResult SaveSetStandard(tLNDCompetencyStandard data)
        {
            var b = db.tLNDCompetencyStandards
                     .SingleOrDefault(t =>
                         t.comptCode.Equals(data.comptCode) &&
                         t.comptPositionCode.Equals(data.comptPositionCode)
                      );

            if (b != null)
            {
                b.standardCode = data.standardCode;
                db.SaveChanges();
                return Json(new { status = "success", msg = "Successfully Edited!" });
            }
            else
            {
                var s = new tLNDCompetencyStandard();
                s.comptPositionCode = data.comptPositionCode;
                s.standardCode = data.standardCode;
                s.comptCode = data.comptCode;

                db.tLNDCompetencyStandards.Add(s);
                db.SaveChanges();

                return Json(new { status = "success", msg = "Successfully Saved!" });
            }
        }



        // *********************************************************************************************************************************************

        [HttpPost]
        public JsonResult CheckLeadTech(tLNDCompetencyPosition data)
        {
            string deptCode = Session["_DeptCode"].ToString();

            IEnumerable<vLNDCompetencyStandard> comptList = db.vLNDCompetencyStandards.Where(e => e.assessmentYear == DateTime.Now.Year && e.comptPositionCode == data.comptPositionCode).ToList();
            var coreList = comptList.Where(e => e.comptGroupCode == "CORE").OrderBy(o => o.orderNo).ToList();
            var leadList = comptList.Where(e => e.comptGroupCode == "LEAD").OrderBy(o => o.orderNo).ToList();
            var techList = comptList.Where(e => e.comptGroupCode == "TECH").OrderBy(o => o.orderNo).ToList();

            //tLNDCompetencyPositionGroup pGroup = db.tLNDCompetencyPositionGroups.SingleOrDefault(e => e.comptPositionCode == data.comptPositionCode && e.assessmentYear == DateTime.Now.Year);

            int leadTag = 0;
            int techTag = 0;

            if (comptList != null)
            {
                leadTag = Convert.ToInt16(leadList.Count());
                techTag = Convert.ToInt16(techList.Count());
            }

            return Json(new { status = "success", leadTag = leadTag, techTag = techTag }, JsonRequestBehavior.AllowGet);

        }


        // *********************************************************************************************************************************************
        // DEPARTMENT POSITION

        [HttpPost]
        public JsonResult SaveCompetencyPositionGroup(tLNDCompetencyPositionGroup data)
        {
            try
            {

                tLNDCompetencyPositionGroup p = new tLNDCompetencyPositionGroup();
                DateTime dt = DateTime.Now;
                string uEIC = Session["_EIC"].ToString();
                string code = "COMPTPOS" + dt.ToString("yyMMddHHmmssffff") + uEIC.Substring(0, 3);
                p.comptPositionCode = code;
                p.comptPositionTitle = data.comptPositionTitle;
                p.comptPositionTitleSub = data.comptPositionTitleSub;
                p.salaryGrade = data.salaryGrade;
                p.assmntGroupCode = data.assmntGroupCode;
                p.tag = 1;
                db.tLNDCompetencyPositionGroups.Add(p);
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
        public JsonResult SaveCompetencyMapping(tLNDCompetencyPositionMap data)
        {
            try
            {
                tLNDCompetencyPositionMap map = new tLNDCompetencyPositionMap();
                map.plantillaCode = data.plantillaCode;
                map.comptPositionCode = data.comptPositionCode;
                map.tag = 1;
                db.tLNDCompetencyPositionMaps.Add(map);
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
        public JsonResult RemoveCompetencyMapping(tLNDCompetencyPositionMap data)
        {
            try
            {
                tLNDCompetencyPositionMap rem = db.tLNDCompetencyPositionMaps.Single(e => e.comptPositionCode == data.comptPositionCode && e.plantillaCode == data.plantillaCode && e.tag == 1);
                db.tLNDCompetencyPositionMaps.Remove(rem);
                db.SaveChanges();
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        // *********************************************************************************************************************************************
        // *********************************************************************************************************************************************

        //JS
        [HttpPost]
        public JsonResult LoadCompetencyByGroup(string id, string code)
        {

            int assmntYear = 0;

            string uEIC = Session["EIC"].ToString();
            //string assmntGroupCode = db.tLNDHRDFocals.SingleOrDefault(e => e.EIC == uEIC).assmntGroupCode;
            if (id == "TECH")
            {
                var coreData = db.tLNDCompetencies.Where(e => e.assmntGroupCode == code && e.comptGroupCode == "TECH" && e.assessmentYear == assmntYear).OrderBy(e => e.orderNo).ToList();
                return Json(new { status = "success", comptList = coreData }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var coreData = db.tLNDCompetencies.Where(e => e.comptGroupCode == id && e.assessmentYear == assmntYear).OrderBy(e => e.orderNo).ToList();
                return Json(new { status = "success", comptList = coreData }, JsonRequestBehavior.AllowGet);
            }
        }



        //Kani
        public JsonResult CoreKBI(string comptCode, string tool)
        {
            int aYear = 0;
            tLNDCompetency competency = db.tLNDCompetencies.Single(e => e.comptCode == comptCode);
            if (competency.comptTool == "3") //LISTED
            {
                IEnumerable<vLNDToolList> comptList = db.vLNDToolLists.Where(e => e.comptCode == comptCode).OrderBy(o => o.orderNo).ToList();
                IEnumerable<vLNDToolList> comptMain = comptList.Where(e => e.MKBICode == null).ToList();
                var core = comptMain.Select(a => new
                {
                    a.KBICode,
                    a.KBI,
                    a.orderNo,
                    list = comptList.Where(e => e.MKBICode == a.KBICode).Select(b => new
                    {
                        b.KBICode,
                        b.KBI,
                        b.orderNo
                    }).ToList()
                }).ToList();
                return Json(new { status = "success", comptTool = competency.comptTool, KBIList = core }, JsonRequestBehavior.AllowGet);
            }
            else  //Progressive
            {
                IEnumerable<vLNDToolProgressive> list = db.vLNDToolProgressives.Where(e => e.comptCode == comptCode && e.assessmentYear == aYear).OrderBy(o => o.orderNo).ToList();
                var core = list.Select(a => new
                {
                    a.KBICode,
                    a.comptLevel,
                    a.basic,
                    a.intermediate,
                    a.advance,
                    a.superior,
                    a.orderNo
                }).ToList();
                return Json(new { status = "success", comptTool = competency.comptTool, KBIList = core }, JsonRequestBehavior.AllowGet);
            }
        }

        //SAVE COMPETENCY
        public JsonResult SaveCompetency(tLNDCompetency data)
        {
            int assmntYear = 0;

            string uEIC = Session["_EIC"].ToString();

            string tmpCode = "C" + DateTime.Now.ToString("yyMMddHHmmssfff") +  "LND" +  uEIC.Substring(0, 6);

            tLNDCompetency c = new tLNDCompetency();
            c.comptCode = tmpCode;
            c.comptName = data.comptName;
            c.comptDesc = data.comptDesc;
            c.comptGroupCode = data.comptGroupCode;
            c.assmntGroupCode = data.assmntGroupCode;
            c.comptTool = data.comptTool;
            c.orderNo = data.orderNo;
            c.assessmentYear = assmntYear;
            db.tLNDCompetencies.Add(c);
            db.SaveChanges();

            var comptList = db.tLNDCompetencies.Where(e => e.comptGroupCode == data.comptGroupCode && e.assessmentYear == assmntYear).OrderBy(e => e.orderNo).ToList();
            return Json(new { status = "success", comptList = comptList }, JsonRequestBehavior.AllowGet);

            //var comptList = db.tLNDCompetencies.OrderBy(e => e.assessmentYear == assmntYear && e.comptGroupCode == data.comptGroupCode && e.assmntGroupCode == data.assmntGroupCode).ToList();
            //return Json(new { status = "success", comptList = comptList }, JsonRequestBehavior.AllowGet);
        }


        //EDIT COMPETENCY
        [HttpPost]
        public JsonResult UpdateCompetency(tLNDCompetency data)
        {
            int assmntYear = 0;
            tLNDCompetency c = db.tLNDCompetencies.Single(e => e.comptCode == data.comptCode);
            c.comptName = data.comptName;
            c.comptDesc = data.comptDesc;
            db.SaveChanges();
            var coreData = db.tLNDCompetencies.OrderBy(e => e.assessmentYear == assmntYear).ToList();
            return Json(new { status = "success", coreList = coreData }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult SaveMainKBI(tLNDToolListed data)
        {
            string tmpCode = "MK" + DateTime.Now.ToString("yyMMddHHmmssfff");
            //tmpCode = genCodeString(tmpCode, 11);
            IEnumerable<tLNDToolListed> grpList = db.tLNDToolListeds.Where(e => e.comptCode == data.comptCode && e.MKBICode == null).OrderByDescending(e => e.orderNo).ToList();
            int orderNo = 1;
            if (grpList.Count() >= 1)
            {
                orderNo = Convert.ToInt16(grpList.First().orderNo) + 1;
            }

            tLNDToolListed k = new tLNDToolListed();
            k.KBICode = tmpCode;
            k.comptCode = data.comptCode;
            k.KBI = data.KBI;
            k.orderNo = orderNo;
            k.tag = 1;
            db.tLNDToolListeds.Add(k);
            db.SaveChanges();

            return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult GetMainKBIData(string id)
        {
            tLNDToolListed data = db.tLNDToolListeds.Single(e => e.KBICode == id);
            return Json(new { status = "success", MKData = data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateMainKBI(tLNDToolListed data)
        {
            tLNDToolListed k = db.tLNDToolListeds.Single(e => e.KBICode == data.KBICode);
            k.KBI = data.KBI;
            db.SaveChanges();
            return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveKBI(tLNDToolListed data)
        {
            string uEIC = Session["_EIC"].ToString();
            tLNDToolListed mk = db.tLNDToolListeds.Single(e => e.KBICode == data.MKBICode);

            IEnumerable<tLNDToolListed> m = db.tLNDToolListeds.Where(e => e.MKBICode == mk.KBICode).OrderByDescending(o => o.orderNo).ToList();

            int orderNo = 1;
            if (m.Count() >= 1)
            {
                orderNo = Convert.ToInt16(m.First().orderNo) + 1;
            }

            string tmpCode = "KL" + DateTime.Now.ToString("yyMMddHHmmssfff") + uEIC.Substring(0, 8).ToString();


            tLNDToolListed k = new tLNDToolListed();
            k.KBICode = tmpCode;
            k.comptCode = mk.comptCode;
            k.MKBICode = mk.KBICode;
            k.KBI = data.KBI;
            k.orderNo = orderNo;
            k.tag = 1;
            db.tLNDToolListeds.Add(k);
            db.SaveChanges();
            return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetKBIData(string id)
        {
            tLNDToolListed data = db.tLNDToolListeds.Single(e => e.KBICode == id);
            var title = db.tLNDToolListeds.Single(e => e.KBICode == data.MKBICode).KBI;
            return Json(new { status = "success", title = title, MKData = data }, JsonRequestBehavior.AllowGet);
        }

         [HttpPost]
        public JsonResult PKBIData(string id)
        {
            tLNDToolProgressive p = db.tLNDToolProgressives.Single(e => e.KBICode == id);
            return Json(new { status = "success", PKBI = p }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateKBI(tLNDToolListed data)
        {
            tLNDToolListed k = db.tLNDToolListeds.Single(e => e.KBICode == data.KBICode);
            k.KBI = data.KBI;
            db.SaveChanges();
            return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public JsonResult SavePKBI(vLNDToolProgressive data)
        {
            string uEIC = Session["_EIC"].ToString();
            string tmpCode = "KP" + DateTime.Now.ToString("yyMMddHHmmssfff") + uEIC.Substring(0, 8);

            tLNDCompetency c = db.tLNDCompetencies.Single(e => e.comptCode == data.comptCode);

            IEnumerable<tLNDToolProgressive> prog = db.tLNDToolProgressives.Where(e => e.comptCode == c.comptCode).OrderByDescending(o => o.orderNo);
            int orderNo = 1;
            if (prog.Count() >= 1)
            {
                orderNo = Convert.ToInt16(prog.First().orderNo) + 1;
            }

            tLNDToolProgressive p = new tLNDToolProgressive();
            p.KBICode = tmpCode;
            p.comptCode = c.comptCode;
            p.comptLevel = data.comptLevel;
            p.basic = data.basic;
            p.intermediate = data.intermediate;
            p.advance = data.advance;
            p.superior = data.superior;
            p.orderNo = orderNo;
            p.tag = 1;
            db.tLNDToolProgressives.Add(p);
            db.SaveChanges();
            return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdatePKBI(vLNDToolProgressive data)
        {
            tLNDToolProgressive p = db.tLNDToolProgressives.Single(e => e.KBICode == data.KBICode);
            p.comptLevel = data.comptLevel;
            p.basic = data.basic;
            p.intermediate = data.intermediate;
            p.advance = data.advance;
            p.superior = data.superior;
            db.SaveChanges();
            return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
        }



        public JsonResult GetTechComptList(tLNDCompetencyPosition data)
        {
            //string assmntGroupCode = Session["AssmntGroupCode"].ToString();
            IEnumerable<tLNDCompetency> comptList = db.tLNDCompetencies.Where(e => e.assessmentYear == 0).ToList();
            var techList = comptList.Where(e => e.comptGroupCode == "TECH").OrderBy(o => o.orderNo).ToList();
            IEnumerable<vLNDCompetencyStandardTech> curTechList = db.vLNDCompetencyStandardTeches.Where(e => e.comptPositionCode == data.comptPositionCode).ToList();

            List<vLNDCompetencyStandardTech> myList = new List<vLNDCompetencyStandardTech>();
            foreach (tLNDCompetency item in techList)
            {
                int tag = curTechList.Where(e => e.comptPositionCode == data.comptPositionCode && e.comptCode == item.comptCode).Count();
                myList.Add(new vLNDCompetencyStandardTech()
                {
                    comptCode = item.comptCode,
                    comptName = item.comptName,
                    comptDesc = item.comptDesc,
                    comptGroupCode = item.comptGroupCode,
                    comptTool = item.comptTool,
                    orderNo = item.orderNo,
                    assessmentYear = item.assessmentYear,
                    assmntGroupCode = item.assmntGroupCode,
                    tag = tag
                });
            }

            var techListCommon = myList.Where(e => e.assmntGroupCode == null).ToList();
            var techListOffice = myList.Where(e => e.assmntGroupCode == data.assmntGroupCode).ToList();

            return Json(new { status = "success", techCommon = techListCommon, techOffice = techListOffice }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddTechCompetency(tLNDCompetencyPosition pos, string comptCode)
        {

            int assmntYear = 0;
            string assmntGroupCode = pos.assmntGroupCode;
            tLNDCompetencyStandard stan = db.tLNDCompetencyStandards.SingleOrDefault(e => e.comptPositionCode.Equals(pos.comptPositionCode) && e.comptCode.Equals(comptCode));
            if (stan == null)
            {
                tLNDCompetencyStandard n = new tLNDCompetencyStandard();
                n.comptPositionCode = pos.comptPositionCode;
                n.comptCode = comptCode;
                db.tLNDCompetencyStandards.Add(n);
                db.SaveChanges();
            }

            IEnumerable<tLNDCompetency> techList = db.tLNDCompetencies.Where(e => e.assessmentYear == assmntYear && e.comptGroupCode == "TECH").ToList();
            IEnumerable<vLNDCompetencyStandardTech> curTechList = db.vLNDCompetencyStandardTeches.Where(e => e.comptPositionCode == pos.comptPositionCode).ToList();

            List<vLNDCompetencyStandardTech> myList = new List<vLNDCompetencyStandardTech>();

            foreach (tLNDCompetency item in techList)
            {
                int tag = curTechList.Where(e => e.comptPositionCode == pos.comptPositionCode && e.comptCode == item.comptCode).Count();
                myList.Add(new vLNDCompetencyStandardTech()
                {
                    comptCode = item.comptCode,
                    comptName = item.comptName,
                    comptDesc = item.comptDesc,
                    comptGroupCode = item.comptGroupCode,
                    comptTool = item.comptTool,
                    orderNo = item.orderNo,
                    assessmentYear = item.assessmentYear,
                    assmntGroupCode = item.assmntGroupCode,
                    tag = tag
                });
            }

            var techListCommon = myList.Where(e => e.assmntGroupCode == null).ToList();
            var techListOffice = myList.Where(e => e.assmntGroupCode == assmntGroupCode).ToList();

            return Json(new { status = "success", techCommon = techListCommon, techOffice = techListOffice }, JsonRequestBehavior.AllowGet);
        }

        // AddLeadershipCompetency

        public JsonResult AddLeadershipToStandard(tLNDCompetencyPosition pos)
        {
            int assmntYear = 0;
            IEnumerable<tLNDCompetency> comptList = db.tLNDCompetencies.Where(e => e.assessmentYear == assmntYear && e.comptGroupCode == "LEAD").ToList();
            foreach (tLNDCompetency item in comptList)
            {
                tLNDCompetencyStandard a = new tLNDCompetencyStandard();
                a.comptPositionCode = pos.comptPositionCode;
                a.comptCode = item.comptCode;
                a.standardCode = item.standard;
                db.tLNDCompetencyStandards.Add(a);
            }
            db.SaveChanges();
            return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
        }


        // Remove Leadership Compt from Standard

        public JsonResult RemoveLeadershipFromStandard(tLNDCompetencyPosition pos)
        {
            try
            {

                IEnumerable<vLNDCompetencyStandard> stand = db.vLNDCompetencyStandards.Where(e => e.comptPositionCode == pos.comptPositionCode && e.comptGroupCode == "LEAD").ToList();
                //TO BE VERIFY LATER
                foreach (vLNDCompetencyStandard item in stand)
                {
                    tLNDCompetencyStandard d = db.tLNDCompetencyStandards.SingleOrDefault(e => e.comptPositionCode == item.comptPositionCode && e.comptCode == item.comptCode);
                    db.tLNDCompetencyStandards.Remove(d);
                }
                db.SaveChanges();

                //List<tLNDCompetencyStandard> list = db.tLNDCompetencyStandards.Where(s => s.comptPositionCode == pos.com && s.InventoryLocationId == NewInventoryLocationId && s.LocationId == LocationId).ToList();
                //db.tLNDCompetencyStandards.RemoveRange(list);
                //db.SaveChanges();
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }

            return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
        }


        //Delete Listed
        public JsonResult DeleteKBI(string id)
        {
            tLNDToolListed p = db.tLNDToolListeds.SingleOrDefault(e => e.KBICode == id);
            if (p != null)
            {

                int counter = db.tLNDToolListeds.Where(e => e.MKBICode == p.KBICode).Count();

                if (counter > 0)
                {
                    return Json(new { status = "Unable to delete!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    try
                    {
                        db.tLNDToolListeds.Remove(p);
                        db.SaveChanges();
                        return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        return Json(new { status = "Error deleting!" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            else
            {
                return Json(new { status = "Invalid transaction!" }, JsonRequestBehavior.AllowGet);
            }

        }


        [HttpPost]
        public JsonResult DeletePKBI(string id)
        {
            tLNDToolProgressive p = db.tLNDToolProgressives.SingleOrDefault(e => e.KBICode == id);
            if (p != null)
            {
                try
                {
                    db.tLNDToolProgressives.Remove(p);
                    db.SaveChanges();
                    return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    string errMsg = ex.ToString();
                    return Json(new { status = "Error deleting record!" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { status = "Invalid transaction!" }, JsonRequestBehavior.AllowGet);
            }
        }


        ////////////////////////////////////////////////////
    }
}