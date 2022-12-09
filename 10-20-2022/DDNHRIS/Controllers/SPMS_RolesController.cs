using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DDNHRIS.Models.SPMS;

namespace DDNHRIS.Content
{
    public class SPMS_RolesController : Controller
    {
        SPMSDBEntities _db = new SPMSDBEntities();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string nums = "0123456789";
        Random random = new Random();
        string[] quan = new string[5];


        public ActionResult Roles()
        {
            return View();
        }
        [HttpGet]
        public ActionResult getRoles()
        {

            var roles = _db.tSPMS_Roles.ToList();
            var menus = _db.tSPMS_Menu.ToList();
            var rolesmenu = _db.vSPMS_RoleMenu.ToList();
            return Json(new { roles, menus, rolesmenu }, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult saveMenu(List<vSPMS_RoleMenu> rolesMenu)
        {

            foreach (var i in rolesMenu)
            {
                var isExist = _db.tSPMS_RoleMenu.Where(a => a.recNo == i.recNo).FirstOrDefault();

                if (i.recNo == 0) // SAVE
                {
                    var add = new tSPMS_RoleMenu()
                    {
                       rID = i.rID,
                       mID = i.mID
                    };
                    _db.tSPMS_RoleMenu.Add(add);
                }
                else
                {
                    isExist.mID = i.mID;
                }
            }
            _db.SaveChanges();

            return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult removedMenu(int recNo)
        {

            var remove = _db.tSPMS_RoleMenu.Where(a => a.recNo == recNo).FirstOrDefault();
            _db.tSPMS_RoleMenu.Remove(remove);
            _db.SaveChanges();

            return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

        }
    }
}