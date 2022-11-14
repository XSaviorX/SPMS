﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DDNHRIS.Models.SPMS;

namespace DDNHRIS.Controllers
{
    public class SPMS_DivisionUtilityController : Controller
    {
        // GET: SPMS_Signatories
        SPMSDBEntities _db = new SPMSDBEntities();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string nums = "0123456789";
        Random random = new Random();
        string[] quan = new string[5];

        public ActionResult DivisionUtility()
        {
            return View();
        }

        [HttpPost]
        public ActionResult getUsers(String OfficeId)
        {

            var users = _db.vSPMS_Employees.Where(a => a.officeId == OfficeId).ToList();
            var divisions = _db.tOfficeDivisions.Where(a => a.officeId == OfficeId).ToList();
            var officeRoles = _db.tSPMS_OfficeRole.ToList();
            return Json(new { users, divisions, officeRoles }, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult addManytoManyDivision(List<vSPMS_Employees> Users, String OfficeId)
        {

            foreach (var i in Users)
            {
                var update = _db.tSPMS_Employees.Where(a => a.EIC == i.EIC).FirstOrDefault();

                if(i.officeRoleId == "ORCKHKLG582947") //IS OFFICE HEAD
                {
                    update.officeRoleId = i.officeRoleId;
                    update.division = OfficeId;
                }
                else
                {
                    update.officeRoleId = i.officeRoleId;
                    update.division = i.division;
                }

               
            }
            _db.SaveChanges();

            return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult addManytoOneDivRole(List<tSPMS_Employees> Users, String OfficeRoleID, String DivisionID)
        {

            foreach (var i in Users)
            {
                var update = _db.tSPMS_Employees.Where(a => a.EIC == i.EIC).FirstOrDefault();

                update.officeRoleId = OfficeRoleID;
                update.division = DivisionID;
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

    }
}