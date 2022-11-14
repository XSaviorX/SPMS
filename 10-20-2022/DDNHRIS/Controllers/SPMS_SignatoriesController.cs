using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DDNHRIS.Models.SPMS;

namespace DDNHRIS.Controllers
{
    public class SPMS_SignatoriesController : Controller
    {
        // GET: SPMS_Signatories
        SPMSDBEntities _db = new SPMSDBEntities();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string nums = "0123456789";
        Random random = new Random();
        string[] quan = new string[5];

        public ActionResult Signatories()
        {
            return View();
        }

        [HttpPost]    
        public ActionResult getUsers(String OfficeId)
        {

            var data = _db.vSPMS_Employees.Where(a => a.officeId == OfficeId).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult addManytoManySupervisor(List<tSPMS_Signatories> Users, String OfficeHeadId, String DivisionId)
        {

            foreach (var i in Users)
            {
                var isExist = _db.tSPMS_Signatories.Where(a => a.EIC == i.EIC).FirstOrDefault();

                if (isExist == null) // SAVE
                {
                    var add = new tSPMS_Signatories()
                    {
                        EIC = i.EIC,
                        supervisorId = i.supervisorId,
                        officeheadId = OfficeHeadId
                    };
                    _db.tSPMS_Signatories.Add(add);
                }
                else
                {
                    isExist.supervisorId = i.supervisorId;
                }
            }
            _db.SaveChanges();

            return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult addManytoOneSupervisor(List<tSPMS_Employees> Users, String SupervisorId, String OfficeHeadId, String DivisionId)
        {

            foreach(var i in Users)
            {
                var isExist = _db.tSPMS_Signatories.Where(a => a.EIC == i.EIC).FirstOrDefault();

                if(isExist == null) // SAVE
                {
                    var add = new tSPMS_Signatories()
                    {
                        EIC = i.EIC,
                        supervisorId = SupervisorId,
                        officeheadId = OfficeHeadId
                    };
                    _db.tSPMS_Signatories.Add(add);
                }
                else
                {
                    isExist.supervisorId = SupervisorId;
                }
            }
            _db.SaveChanges();

            return Json(new { status = 1}, JsonRequestBehavior.AllowGet);

        }

        
    }
}