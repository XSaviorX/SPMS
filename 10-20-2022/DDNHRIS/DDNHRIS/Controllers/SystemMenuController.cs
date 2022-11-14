using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DDNHRIS.Models;

namespace DDNHRIS.Controllers
{

    [SessionTimeout]
    [Authorize(Roles = "SysAdmin")]
    public class SystemMenuController : Controller
    {
        // GET: SystemMenu
        HRISDBEntities db = new HRISDBEntities();
        public ActionResult List()
        {
            Session["GroupID"] = "MG1537147610";
            return View();
        }

        //NG
        [HttpPost]
        public JsonResult MenuList()
        {
            try
            {
                IEnumerable<vSysMenu> menuList = db.vSysMenus.Where(e => e.tag > 0).ToList();

                //MENU GROUP
                var gMenuGroup = db.tSysMenuGroups.Select(a => new
                {
                    a.menuGroupCode,
                    a.groupName,
                    a.groupOrderNo,
                    a.fontIcon
                }).OrderBy(a => a.groupOrderNo);
                //SUB GROUP

                var subMenuGroup = db.tSysMenuSubs.Where(e => e.subGroupTag > 0).Join(db.tSysMenuGroups, a => a.menuGroupCode, b => b.menuGroupCode, (a, b) => new
                {
                    a.menuSubGroupCode,
                    a.recNo,
                    a.menuGroupCode,
                    b.groupName,
                    b.fontIcon,
                    a.subGroupName,
                    a.subFontIcon,
                    a.subGroupTag,
                    a.subOrderNo
                }).OrderByDescending(a => a.recNo);


                //var subMenuGroup = db.tSysMenuSubs.Select(s => new {
                //    s.menuSubGroupCode,
                //    s.menuGroupCode,   
                //    s.subFontIcon,
                //    s.subGroupName
                //}).OrderBy(o => o.subGroupName).ToList();

                return Json(new { status = "success", list = menuList, menuGroup = gMenuGroup, menuSubGroup = subMenuGroup }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }



        }


        [HttpPost]
        public JsonResult SaveMenuData(tSysMenu data)
        {
            try
            {
                string tmp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                tmp = tmp.Replace("-", "");
                string code = "MNU" + tmp.GetHashCode().ToString();

                tSysMenu m = new tSysMenu();
                m.menuCode = code;
                m.menuGroupCode = data.menuGroupCode;
                m.menuSubGroupCode = data.menuSubGroupCode;
                m.menuName = data.menuName;
                m.controllerName = data.controllerName;
                m.methodName = data.methodName;
                m.orderNo = 100;
                m.menuTypeNo = 2;
                m.tag = 1;
                db.tSysMenus.Add(m);
                db.SaveChanges();

                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {

                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        ///////////////////////////////////////////
        // MENU GROUP
        ////////////////////////////////////////////

        public ActionResult Group()
        {
            return View();
        }

        //NG
        [HttpPost]
        public JsonResult SaveMenuGroup(tSysMenuGroup data)
        {
            try
            {

                string tmp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                tmp = tmp.Replace("-", "");
                string code = "MGRP" + tmp.GetHashCode().ToString();

                tSysMenuGroup m = new tSysMenuGroup();
                m.menuGroupCode = code;
                m.groupName = data.groupName;
                m.groupOrderNo = 10;
                m.groupTag = 1;
                db.tSysMenuGroups.Add(m);
                db.SaveChanges();

                var gMenuGroup = db.tSysMenuGroups.Select(a => new
                {
                    a.menuGroupCode,
                    a.groupName,
                    a.groupOrderNo,
                    a.fontIcon
                }).OrderBy(a => a.groupOrderNo);



                return Json(new { status = "success", menuGroup = gMenuGroup }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        //NG
        [HttpPost]
        public JsonResult SaveMenuSubGroup(tSysMenuSub data)
        {
            try
            {
                string tmp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                tmp = tmp.Replace("-", "");
                string code = "SUBGRP" + tmp.GetHashCode().ToString();
                tSysMenuSub s = new tSysMenuSub();
                s.menuSubGroupCode = code;
                s.subGroupName = data.subGroupName;
                s.menuGroupCode = data.menuGroupCode;
                s.subOrderNo = 10;
                s.subGroupTag = 1;
                db.tSysMenuSubs.Add(s);
                db.SaveChanges();

                //SUB GROUP

                var subMenuGroup = db.tSysMenuSubs.Where(e => e.subGroupTag > 0).Join(db.tSysMenuGroups, a => a.menuGroupCode, b => b.menuGroupCode, (a, b) => new
                {
                    a.menuSubGroupCode,
                    a.recNo,
                    a.menuGroupCode,
                    b.groupName,
                    b.fontIcon,
                    a.subGroupName,
                    a.subFontIcon,
                    a.subGroupTag,
                    a.subOrderNo
                }).OrderByDescending(a => a.recNo);

                return Json(new { status = "success", menuSubGroup = subMenuGroup }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        

      


        ///////////////////////////////////////////
        // SYSTEM ROLE
        ////////////////////////////////////////////

        public ActionResult Role()
        {
            return View();
        }

        //NG
        [HttpPost]
        public JsonResult RoleList()
        {
            var role = db.tSysRoles.Select(a => new
            {
                a.roleID,
                a.roleName,
                a.roleDesc
            }).OrderBy(a => a.roleName);
            return Json(new { status = "success", roleList = role }, JsonRequestBehavior.AllowGet);
        }


        //NG
        [HttpPost]
        public JsonResult RoleMenuList(string id)
        {
            try
            {
                tSysRole role = db.tSysRoles.Single(e => e.roleID == id);
                IEnumerable<vSysRoleMenu> roleMenu = db.vSysRoleMenus.Where(e => e.roleID == id).OrderBy(o => o.menuName).ToList();
                return Json(new { status = "success", role = role, roleMenuList = roleMenu }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }



        }


        //NG
        [HttpPost]
        public JsonResult MenuListByRole(string id)
        {
            try
            {
                tSysRole role = db.tSysRoles.Single(e => e.roleID == id);
                IEnumerable<vSysRoleMenu> roleMenu = db.vSysRoleMenus.Where(e => e.roleID == id).OrderBy(o => o.menuName).ToList();

                IEnumerable<vSysMenu> menuList = db.vSysMenus.ToList();
                List<vSysRoleMenu> list = new List<vSysRoleMenu>();

                foreach (vSysMenu item in menuList)
                {
                    int iTag = 0;
                    int counter = roleMenu.Where(e => e.menuCode == item.menuCode).Count();
                    if (counter >= 1)
                    {
                        iTag = 1;
                    }

                    list.Add(new vSysRoleMenu()
                    {
                        menuCode = item.menuCode,
                        menuName = item.menuName,
                        controllerName = item.controllerName,
                        methodName = item.methodName,
                        groupName = item.groupName,
                        tag = iTag
                    });
                }
                return Json(new { status = "success", role = role, menuList = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        //NG
        [HttpPost]
        public JsonResult RemoveMenuFromRole(string id, string roleID)
        {
            try
            {
                tSysRoleMenu delMenu = db.tSysRoleMenus.Single(e => e.roleID == roleID && e.menuCode == id);
                db.tSysRoleMenus.Remove(delMenu);
                db.SaveChanges();

                tSysRole role = db.tSysRoles.Single(e => e.roleID == roleID);
                IEnumerable<vSysRoleMenu> roleMenu = db.vSysRoleMenus.Where(e => e.roleID == roleID).OrderBy(o => o.menuName).ToList();
                return Json(new { status = "success", role = role, roleMenuList = roleMenu }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        //SaveMenuToRole
        //NG
        [HttpPost]
        public JsonResult SaveMenuToRole(string id, string roleID)
        {
            try
            {
                tSysRoleMenu m = new tSysRoleMenu();
                m.roleID = roleID;
                m.menuCode = id;
                db.tSysRoleMenus.Add(m);
                db.SaveChanges();


                tSysRole role = db.tSysRoles.Single(e => e.roleID == roleID);
                IEnumerable<vSysRoleMenu> roleMenu = db.vSysRoleMenus.Where(e => e.roleID == roleID).OrderBy(o => o.menuName).ToList();

                IEnumerable<vSysMenu> menuList = db.vSysMenus.ToList();
                List<vSysRoleMenu> list = new List<vSysRoleMenu>();

                foreach (vSysMenu item in menuList)
                {
                    int iTag = 0;
                    int counter = roleMenu.Where(e => e.menuCode == item.menuCode).Count();
                    if (counter >= 1)
                    {
                        iTag = 1;
                    }

                    list.Add(new vSysRoleMenu()
                    {
                        menuCode = item.menuCode,
                        menuName = item.menuName,
                        controllerName = item.controllerName,
                        methodName = item.methodName,
                        groupName = item.groupName,
                        tag = iTag
                    });
                }
                return Json(new { status = "success", role = role, menuList = list }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }



        public ActionResult UserRole()
        {
            return View();
        }

        //NG
        [HttpPost]
        public JsonResult EmployeeList()
        {
            try
            {

                IEnumerable<vRSPEmployee> empList = db.vRSPEmployees.Where(e => e.recNo > 0).OrderBy(o => o.fullNameLast);
                var emp = empList.Select(e => new { e.EIC, e.fullNameLast });

                return Json(new { status = "success", employeeList = emp }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception) { return Json(new { status = "error" }, JsonRequestBehavior.AllowGet); }
        }


        public JsonResult RoleListByEIC(string id)
        {
            try
            {
                vRSPEmployee emp = db.vRSPEmployees.Single(e => e.EIC == id);  
                IEnumerable<vSysUserRole> userRoleList = db.vSysUserRoles.Where(e => e.EIC == id).ToList().ToList();
                return Json(new { status = "success", userData = emp, userRoleList = userRoleList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception) { return Json(new { status = "error" }, JsonRequestBehavior.AllowGet); }

        }



        [HttpPost]
        public JsonResult ShowRoleList(string id)
        {
            try
            {
                IEnumerable<vSysUserRole> userRole = db.vSysUserRoles.Where(e => e.EIC == id).ToList();
                IEnumerable<tSysRole> sysRole = db.tSysRoles.Where(e => e.tag > 0).ToList();
                List<tSysRole> list = new List<tSysRole>();

                foreach (tSysRole item in sysRole)
                {
                    int counter = userRole.Where(e => e.roleID == item.roleID).Count();
                    list.Add(new tSysRole()
                    {
                        roleID = item.roleID,
                        roleName = item.roleName,
                        roleDesc = item.roleDesc,
                        tag = counter
                    });
                }
                return Json(new { status = "success", sysRoleList = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception) { return Json(new { status = "error" }, JsonRequestBehavior.AllowGet); }

        }


        [HttpPost]
        public JsonResult SaveRoleToUser(string EIC, string roleID)
        {
            try
            {
                tSysRoleUser n = new tSysRoleUser();
                n.EIC = EIC;
                n.roleID = roleID;
                db.tSysRoleUsers.Add(n);
                db.SaveChanges();

                IEnumerable<vSysUserRole> userRole = db.vSysUserRoles.Where(e => e.EIC == EIC).ToList();
                IEnumerable<tSysRole> sysRole = db.tSysRoles.Where(e => e.tag > 0).ToList();
                List<tSysRole> list = new List<tSysRole>();

                foreach (tSysRole item in sysRole)
                {
                    int counter = userRole.Where(e => e.roleID == item.roleID).Count();
                    list.Add(new tSysRole()
                    {
                        roleID = item.roleID,
                        roleName = item.roleName,
                        roleDesc = item.roleDesc,
                        tag = counter
                    });
                }

                IEnumerable<vSysUserRole> userRoleList = db.vSysUserRoles.Where(e => e.EIC == EIC).ToList().ToList();

                return Json(new { status = "success", sysRoleList = list, userRoleList = userRoleList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception) { return Json(new { status = "error" }, JsonRequestBehavior.AllowGet); }
        }


        //
        //NG
        [HttpPost]
        public JsonResult RemoveRoleFromUser(string EIC, string roleID)
        {
            try
            {
                tSysRoleUser uRole = db.tSysRoleUsers.Single(e => e.EIC == EIC && e.roleID == roleID);
                db.tSysRoleUsers.Remove(uRole);
                db.SaveChanges();
               

                IEnumerable<vSysUserRole> userRole = db.vSysUserRoles.Where(e => e.EIC == EIC).ToList();
                IEnumerable<tSysRole> sysRole = db.tSysRoles.Where(e => e.tag > 0).ToList();
                List<tSysRole> list = new List<tSysRole>();

                foreach (tSysRole item in sysRole)
                {
                    int counter = userRole.Where(e => e.roleID == item.roleID).Count();
                    list.Add(new tSysRole()
                    {
                        roleID = item.roleID,
                        roleName = item.roleName,
                        roleDesc = item.roleDesc,
                        tag = counter
                    });
                }

                IEnumerable<vSysUserRole> userRoleList = db.vSysUserRoles.Where(e => e.EIC == EIC).ToList().ToList();

                return Json(new { status = "success", sysRoleList = list, userRoleList = userRoleList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception) { return Json(new { status = "error" }, JsonRequestBehavior.AllowGet); }
        }


        ////////////////////////////////////////////////
    }
}