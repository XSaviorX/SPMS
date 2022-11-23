using System;
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

                var updateRole = _db.tSPMS_EmployeeRole.Where(a => a.EIC == i.EIC & a.recNo == i.recNoRole).FirstOrDefault();
                var updateDivision = _db.tSPMS_Employees.Where(a => a.EIC == i.EIC).FirstOrDefault();

                if (updateRole == null) // ADD ROLE
                {
                    var addRole = new tSPMS_EmployeeRole()
                    {
                        EIC = i.EIC,
                        RID = i.RID

                    };
                    _db.tSPMS_EmployeeRole.Add(addRole);

                    if (i.RID == "ORCKHKLG582947") //IS OFFICE HEAD
                    {
                        updateDivision.division = OfficeId;
                    }
                    else
                    {

                        updateDivision.division = i.division;
                    }

                }
                else // UPDATE ROLE
                {
                    if (i.RID == "ORCKHKLG582947") //IS OFFICE HEAD
                    {
                        updateRole.RID = i.RID;
                        updateDivision.division = OfficeId;
                    }
                    else
                    {
                        updateRole.RID = i.RID;

                        updateDivision.division = i.division;
                    }
                }

            }
            _db.SaveChanges();

            return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult addManytoOneDivRole(List<vSPMS_Employees> Users, String OfficeRoleID, String DivisionID)
        {

            foreach (var i in Users)
            {
                var updateDivision = _db.tSPMS_Employees.Where(a => a.EIC == i.EIC).FirstOrDefault();
                var updateRole = _db.tSPMS_EmployeeRole.Where(a => a.EIC == i.EIC & a.recNo == i.recNoRole).FirstOrDefault();



                if (updateRole == null) // ADD ROLE
                {
                    var addRole = new tSPMS_EmployeeRole()
                    {
                        EIC = i.EIC,
                        RID = OfficeRoleID

                    };
                    _db.tSPMS_EmployeeRole.Add(addRole);


                    updateDivision.division = DivisionID;

                }
                else // UPDATE ROLE
                {

                    updateRole.RID = OfficeRoleID;
                    updateDivision.division = DivisionID;
                }
                /* try
                 {
                     updateRole.RID = OfficeRoleID;
                     updateDivision.division = DivisionID;
                 }
                 catch (NullReferenceException e)
                 {
                     //Code to do something with e
                 }*/

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

        [HttpPost]
        public ActionResult removeRole(int RecNo)
        {
            var deleteRole = _db.tSPMS_EmployeeRole.Where(a => a.recNo == RecNo).FirstOrDefault();

            _db.tSPMS_EmployeeRole.Remove(deleteRole);

            _db.SaveChanges();
            return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult saveRole(String OfficeRoleID, vSPMS_Employees User)
        {
            var saveRole = _db.tSPMS_EmployeeRole.Where(a => a.EIC == User.EIC && a.RID == OfficeRoleID).FirstOrDefault();
            var data = 0;
            if (saveRole == null)
            {
                var addRole = new tSPMS_EmployeeRole()
                {
                    EIC = User.EIC,
                    RID = OfficeRoleID

                };
                data = 1;
                _db.tSPMS_EmployeeRole.Add(addRole);

            }

            _db.SaveChanges();
            return Json(new { status = data }, JsonRequestBehavior.AllowGet);
        }

    }
}