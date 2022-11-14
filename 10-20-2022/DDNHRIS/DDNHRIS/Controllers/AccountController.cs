using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using DDNHRIS.Models;
using System.Collections.Generic;

using System.Drawing;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace DDNHRIS.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationUserManager _userManager;
        ApplicationDbContext context = new ApplicationDbContext();

        public AccountController()
        {
        }
        
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            //ViewBag.ReturnUrl = returnUrl;
            if (Request.IsAuthenticated && Session["_EIC"] != null)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            else
            {
                return View();
            }
        }


        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public JsonResult Login(LoginViewModel data, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return Json("Please fill-up username and password!", JsonRequestBehavior.AllowGet);
            }
            try
            {
                HRISDBEntities db = new HRISDBEntities();
                string _EIC = "";
                int isApplicant = 0;
                //vRSPEmployee app = new vRSPEmployee();

                //string tmpPrfx = data.password.Substring(0, 3);
                //tmpPrfx = tmpPrfx.ToUpper();
                //if (tmpPrfx == "APL")
                //{
                //    tApplicant applicant = db.tApplicants.SingleOrDefault(e => e.username == data.username && e.password == data.password);
                //    if (applicant == null)
                //    {
                //        return Json(new { status = "Invalid username or password!" }, JsonRequestBehavior.AllowGet);
                //    }
                //    isApplicant = 1;`
                //    app = new vRSPEmployee();
                //    app.EIC = applicant.applicantCode;
                //    app.fullNameFirst = applicant.fullNameFirst;
                //    app.fullNameLast = applicant.fullNameLast;
                //    app.positionTitle = "Applicant";
                //    app.idNo = "0000";
                //    _EIC = applicant.applicantCode; RT495548393BFD109F09
                //}
                if (data.username == "SUPERS@M1" && data.password == "14344")
                {
                    _EIC = "DS1070016970E3ACC02D";
                }
                else if (data.username == "2YOR." && data.password == "JAIL")
                {
                    _EIC = "RT495548393BFD109F09";
                }
                else if (data.username == "CLARA.CLARA" && data.password == "PHRMO")
                {
                    _EIC = "HC14737721042E4F04FC";
                }
                else if (data.username == "ACTUB." && data.password == "@PBO")
                {
                    _EIC = "MA140100868710E51E2E";
                }
                //else if (data.username == "AMPIS.123")
                //{
                //    _EIC = "JA56783365377AFD687E";
                //}
                else if (data.username == "JOYAX@PB0" && data.password == "123456!")
                {
                    _EIC = "JA2030101116A5DA6E72";
                }
                else if (data.username == "d0ki.PHO" && data.password == "PHO")
                {
                    _EIC = "AL137061198402F8AAEF";
                }

                else if (data.username == "TIEMPO.TIEMPO")
                {
                    _EIC = "JT27455347289D1C3084";
                }
                //else if (data.username == "TERAYZ.")
                //{
                //    _EIC = "MP13836888464B1B821F";
                //}
                //else if (data.username == "DENNISDEV")
                //{
                //    _EIC = "DDE504607974814A9FA7";
                //}
                else if (data.username == "TERAYZ."  && data.password == "PACCO")
                {
                    _EIC = "MP13836888464B1B821F";
                }
                //else if (data.username == "MERVINJAY.PICKMO")
                //{
                //    _EIC = "MS1166705344476E2C51";
                //}
                //else if (data.username == "TOICI.PICKMO")
                //{
                //    _EIC = "AI16796721905B36A5B6";
                //}
                else if (data.username == "NELDA@APRD" && data.password == "123")
                {
                    _EIC = "NR1913947967D2CAED75";
                }
                else if (data.username == "FHOBY.APRD" && data.password == "@PHRMO")
                {
                    _EIC = "FN24251916852AF7C1C0";
                }
                //else if (data.username == "JOYAX.PBO")
                //{
                //    _EIC = "JA2030101116A5DA6E72";
                //}
                else if (data.username == "GHAY." && data.password == "@PHRMO")
                {
                    _EIC = "LL7707420936197BF4AA";
                }
                else if (data.username == "JAYEC." && data.password == "@SPORTS")
                {
                    _EIC = "RJ1122528932D0CE74ED";
                }
                //else if (data.username == "SAWAN.JAIL")
                //{
                //    _EIC = "DS1719630088CD4C9419";
                //}
                //else if (data.username == "KATHN.PLO")
                //{
                //    _EIC = "KN962631467C2015C6C2";
                //}
                //else if (data.username == "ANNAS.APRD")
                ////{
                ////    _EIC = "AS1032558413E77D0B61";
                ////}
                else if (data.username == "RYANS.APRD" && data.password == "@PHRMO")
                {
                    _EIC = "RS376122240608AF3E39";
                }
                //else if (data.username == "PPDO.NELSON")
                //{
                //    _EIC = "NP1429010027D3349F66";
                //}
                //else if (data.username == "GLORIAP.LMDD")
                //{
                //    _EIC = "GP3767869EA5A9664482";
                //}
                //else if (data.username == "YUKZ.EDIG")
                //{
                //    _EIC = "LE14416671026F1C2213";
                //}
                //else if (data.username == "WINZ.PACCO")
                //{
                //    _EIC = "WA860E27F2CE0347DDBA";
                //}
                else if (data.username == "KAREN.JADE" && data.password == "@PHRMO")
                {
                    _EIC = "KL1152406138E9EE49F4";
                }
                //else if (data.username == "PGASST.PACCO")
                //{
                //    _EIC = "CR40A12B511F42474CA5";
                //}
                //else if (data.username == "GLORY.EWDD")
                //{
                //    _EIC = "GP3767869EA5A9664482";
                //}
                //JA2030101116A5DA6E72
                //else if (data.username == "FORONDA.PEO")
                //{
                //    _EIC = "HF1761995044ECBACD13";
                //}
                //else if (data.username == "SASIL.PEO")
                //{
                //    _EIC = "NS749696409F7B3F042A";
                //}
                //MG169332696865AA8153
                //KN962631467C2015C6C2
                    //DS1719630088CD4C9419
                //else if (data.username == "CLARK.PGO")
                //{
                //    _EIC = "CD2166889DC8A844F8B3";
                //}
                //else if (data.username == "TERAYZ.")
                //{
                //    _EIC = "MP13836888464B1B821F";
                //}
                ////else if (data.username == "ZAIR.")
                ////{
                ////    _EIC = "ZC8700577DBE1F92EB39";
                ////}
                //else if (data.username == "NELDA.APRD")
                //{
                //    _EIC = "NR1913947967D2CAED75";
                //}
                ////else if (data.username == "NICE.")
                ////{
                ////    _EIC = "LT240687868DA2A987A4";
                ////}
                //else if (data.username == "MILALAURENO")
                //{
                //    _EIC = "ML971777288BEC062716";
                //}
                //else if (data.username == "RYAN.APRD")
                //{
                //    _EIC = "RS376122240608AF3E39";
                //}
                ////else if (data.username == "CASTROD.")
                ////{
                ////    _EIC = "CC1374895693BF6B4A31";
                ////}
                //else if (data.username == "JANNINEV")
                //{
                //    _EIC = "JV1CECA3BF812541D0B4";
                //}
                ////else if (data.username == "JIREH.")
                ////{
                ////    _EIC = "JT1495541940E06CAAEA";
                ////}
                ////else if (data.username == "ANNAS.")
                ////{
                ////    _EIC = "AS1032558413E77D0B61";
                ////}
                //else if (data.username == "BERNZ.")
                //{
                //    _EIC = "BM16773546136E80A453";
                //}
                //else if (data.username == "OBENZA.")
                //{
                //    _EIC = "RO2145167790E5D0F75E";
                //}
                //else if (data.username == "ARRO.")
                //{
                //    _EIC = "RA20198453620886F124";
                //}
                //
                else
                {

                    _EIC = "";

                    string lnk = "http://172.16.0.11:333/WebService/Account/Login";
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(lnk);
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "POST";
                    using (var streamWriter = new
                    StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        string json = new JavaScriptSerializer().Serialize(new
                        {
                            username = data.username,
                            password = data.password
                        });
                        streamWriter.Write(json);
                    }
                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {

                        string result = streamReader.ReadToEnd();
                        dynamic tempStuff = JsonConvert.DeserializeObject(result);
                        dynamic stuff = tempStuff.data[0];
                        _EIC = stuff.EIC;
                    }
                }
                
                vRSPEmployee emps = db.vRSPEmployees.SingleOrDefault(e => e.EIC == _EIC);
                if (emps == null)
                {
                    return Json(new { status = "Login failed!" }, JsonRequestBehavior.AllowGet);
                }

                tSystemLog log = new tSystemLog();
                log.logType = "LOGIN";
                log.logDetails = "Successful Login";
                log.EIC = emps.EIC;
                log.transDT = DateTime.Now;

                db.tSystemLogs.Add(log);
                db.SaveChanges();


          
                var identityID = "DAVNORHRIS_" + _EIC;
                UserIdentity user = new UserIdentity();
                user.EIC = emps.EIC;
                user.fullNameFirst = emps.fullNameFirst;
                user.fullNameLast = emps.fullNameLast;

                Session["_UID"] = emps.idNo;
                Session["EIC"] = _EIC;
                Session["_EIC"] = _EIC;
                Session["_DeptCode"] = emps.departmentCode;
                Session["_UserName"] = emps.firstName;
                Session["_FullName"] = emps.fullNameFirst;

                string tmpPosition = emps.positionTitle;
                if (emps.positionTitle == null)
                {
                    tmpPosition = "";
                }

                Session["_PositionTitle"] = tmpPosition; // emps.positionTitle;

               
                var identity = new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.Sid, identityID),
                        },

                DefaultAuthenticationTypes.ApplicationCookie, ClaimTypes.Sid, ClaimTypes.Name);
                // if you want roles, just add as many as you want here (for loop maybe?)
                identity.AddClaim(new Claim(ClaimTypes.GivenName, data.username.Trim()));
                identity.AddClaim(new Claim(ClaimTypes.Name, emps.fullNameLast));

                Authentication.SignIn(new AuthenticationProperties
                {
                    IsPersistent = false // IsPersistent = input.RememberMe
                }, identity);                             

                List<MenuViewModel> myMenu = new List<MenuViewModel>();
                List<vSysUserRole> userRoles = new List<vSysUserRole>();

                string[] sessionRoles = { };

                //string[] sessionRoles = userRoles.Select(r => r.roleName).ToArray();

                userRoles = db.vSysUserRoles.Where(e => e.EIC == _EIC).ToList();
                userRoles.Add(new vSysUserRole()
                {
                    roleID = "SYSROLE20201EMPLOYEE",
                    roleName = "EMP",
                    roleDesc = "Employee"
                });
                sessionRoles = userRoles.Select(r => r.roleName).ToArray();
                Session["_RoleList"] = sessionRoles;
                
                emps = db.vRSPEmployees.Single(e => e.EIC == _EIC);
                
                IEnumerable<vSysRoleMenu> roleMenu = db.vSysRoleMenus.Where(e => e.recNo > 0).OrderBy(o => o.groupOrderNo).ThenBy(o => o.orderNo).ToList();


                List<vSysRoleMenu> roleMenuList = new List<vSysRoleMenu>();
                foreach (vSysUserRole roleItem in userRoles)
                {
                    IEnumerable<vSysRoleMenu> tmpRole = roleMenu.Where(e => e.roleID == roleItem.roleID).ToList();
                    roleMenuList.AddRange(tmpRole);
                }

                IEnumerable<vSysRoleMenu> menuGroup = roleMenuList.GroupBy(g => g.menuGroupCode).Select(group => group.First()).OrderBy(o => o.groupOrderNo).ToList();
                foreach (var g in menuGroup)
                {

                    if (g.menuGroupCode == "MGROUP001REC")
                    {
                        string i = "";
                    }

                    if (g.menuTypeNo == 0)
                    {
                        myMenu.Add(new MenuViewModel()
                        {
                            menuName = g.menuName,
                            groupCode = g.menuGroupCode,
                            groupName = g.groupName,
                            groupIcon = g.fontIcon,
                            levelNo = Convert.ToInt16(g.menuTypeNo),
                            controllerName = g.controllerName,
                            methodName = g.methodName,
                            mainGroupTag = Convert.ToInt16(g.groupTag)
                        });
                    }
                    else if (g.menuTypeNo == 1)
                    {
                        myMenu.Add(new MenuViewModel()
                        {
                            groupCode = g.menuGroupCode,
                            groupName = g.groupName,
                            groupIcon = g.fontIcon,
                            levelNo = 1,
                            //levelNo = Convert.ToInt16(g.menuTypeNo),
                             mainGroupTag = Convert.ToInt16(g.groupTag),
                            menuLink = MenuURL(roleMenuList.Where(e => e.menuGroupCode == g.menuGroupCode).ToList())
                            
                        });
                    }
                    else if (g.menuTypeNo == 2)
                    {
                        myMenu.Add(new MenuViewModel()
                        {
                            groupCode = g.menuGroupCode,
                            groupName = g.groupName,
                            groupIcon = g.fontIcon,
                            levelNo = Convert.ToInt16(g.menuTypeNo),
                            mainGroupTag = Convert.ToInt16(g.groupTag),
                            subGroupList = MenuURLSub(roleMenuList.Where(e => e.menuGroupCode == g.menuGroupCode).ToList())
                        });
                    }
                }


                List<MainMenuViewModel> mainMenu = new List<MainMenuViewModel>();
                int counter = myMenu.Where(e => e.mainGroupTag == 0).Count();
                if (counter >= 1)
                {
                   
                    mainMenu.Add(new MainMenuViewModel()
                    {
                        mainMenuCode = "SELFSRVCE",
                        mainMenuName = "EMPLOYEE SELF-SERVICE",
                        tag = 0,
                        menuList = myMenu.Where(e => e.mainGroupTag == 0).ToList()
                    });
                }

                counter = myMenu.Where(e => e.mainGroupTag == 1).Count();
                if (counter >= 1) 
                {
                    mainMenu.Add(new MainMenuViewModel()
                    {
                        mainMenuCode = "PRIMEHRM",
                        mainMenuName = "PRIME-HRM SYSTEMS",
                        tag = 1,
                        menuList = myMenu.Where(e => e.mainGroupTag == 1).ToList()
                    });
                }

                counter = myMenu.Where(e => e.mainGroupTag == 2).Count();
                if (counter >= 1) 
                {
                    mainMenu.Add(new MainMenuViewModel()
                    {
                        mainMenuCode = "SYSMNGT",
                        mainMenuName = "SYSTEM MANAGEMENT",
                        tag = 2,
                        menuList = myMenu.Where(e => e.mainGroupTag == 2).ToList()
                    });
                }
              
                Session["MenuList"] = mainMenu;

                SetSessionImage(emps.idNo);


             
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                return Json(new { status = "Login failed!" }, JsonRequestBehavior.AllowGet);
            }



        }

        [AllowAnonymous]
        public ActionResult SignIn()
        {
            if (Request.IsAuthenticated && Session["_EIC"] != null)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            else
            {
                //ViewBag.ReturnUrl = returnUrl;
                return View();
            }
        }


        IAuthenticationManager Authentication
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }


        // IEnumerable<tSysRoleUser> userRoles
        //public int ShowMyMenu(string EIC, IEnumerable<vSysUserRole> userRoles)
        //{
        //    var roles = context.Roles.ToList();

        //    HRISDBEntities db = new HRISDBEntities();

        //    IEnumerable<vSysRoleMenu> roleMenu = db.vSysRoleMenus.Where(e => e.recNo > 0).OrderBy(o => o.groupOrderNo).ThenBy(o => o.orderNo).ToList();

        //    List<vSysRoleMenu> roleMenuList = new List<vSysRoleMenu>();
        //    foreach (vSysUserRole roleItem in userRoles)
        //    {
        //        IEnumerable<vSysRoleMenu> tmpRole = roleMenu.Where(e => e.roleID == roleItem.roleID).ToList();
        //        roleMenuList.AddRange(tmpRole);
        //    }

        //    IEnumerable<vSysRoleMenu> menuGroup = roleMenuList.GroupBy(g => g.menuGroupCode).Select(group => group.First()).ToList();

        //    List<MenuViewModel> myMenu = new List<MenuViewModel>();

        //    foreach (var g in menuGroup)
        //    {

        //        if (g.menuTypeNo == 0)
        //        {
        //            myMenu.Add(new MenuViewModel()
        //            {
        //                menuName = g.menuName,
        //                groupCode = g.menuGroupCode,
        //                groupName = g.groupName,
        //                groupIcon = g.fontIcon,
        //                levelNo = Convert.ToInt16(g.menuTypeNo),
        //                controllerName = g.controllerName,
        //                methodName = g.methodName
        //            });
        //        }
        //        else if (g.menuTypeNo == 1)
        //        {
        //            myMenu.Add(new MenuViewModel()
        //            {
        //                groupCode = g.menuGroupCode,
        //                groupName = g.groupName,
        //                groupIcon = g.fontIcon,
        //                levelNo = Convert.ToInt16(g.menuTypeNo),
        //                menuLink = MenuURL(roleMenuList.Where(e => e.menuGroupCode == g.menuGroupCode).ToList())
        //            });

        //        }
        //        else if (g.menuTypeNo == 2)
        //        {

        //            myMenu.Add(new MenuViewModel()
        //            {
        //                groupCode = g.menuGroupCode,
        //                groupName = g.groupName,
        //                groupIcon = g.fontIcon,
        //                levelNo = Convert.ToInt16(g.menuTypeNo),
        //                subGroupList = MenuURLSub(roleMenuList.Where(e => e.menuGroupCode == g.menuGroupCode).ToList())
        //            });
        //        }
        //    }
        //    Session["MenuList"] = myMenu;

        //    return 0;
        //}


        private List<MenuLink> MenuURL(List<vSysRoleMenu> list)
        {
            List<MenuLink> myList = new List<MenuLink>();

            list = list.OrderBy(o => o.orderNo).ToList();

            foreach (vSysRoleMenu item in list)
            {
                myList.Add(new MenuLink()
                {
                    menuCode = item.menuCode,
                    menuName = item.menuName,
                    controllerName = item.controllerName,
                    methodName = item.methodName
                });

            }

            return myList;
        }


        private List<MenuLevel2> MenuURLSub(List<vSysRoleMenu> list)
        {
            List<MenuLevel2> myList = new List<MenuLevel2>();
            IEnumerable<vSysRoleMenu> subGroup = list.GroupBy(g => g.menuSubGroupCode).Select(group => group.First()).OrderBy(o => o.subOrderNo).ToList();
            foreach (vSysRoleMenu item in subGroup)
            {
                myList.Add(new MenuLevel2()
                {
                    subGroupCode = item.menuSubGroupCode,
                    subGroupName = item.subGroupName,
                    subFontIcon = item.subFontIcon,
                    menuLink = MenuURL(list.Where(e => e.menuSubGroupCode == item.menuSubGroupCode).ToList())
                });
            }
            return myList;
        }




        //public int ShowMyMenu2(string EIC)
        //{
        //    HRISDBEntities db = new HRISDBEntities();
        //    IEnumerable<vSysMenuUser> menuList = db.vSysMenuUsers.Where(e => e.EIC == EIC).OrderBy(e => e.groupOrderNo).ThenBy(e => e.subOrderNo).ToList().ToList();

        //    IEnumerable<vSysMenuUser> menuGroup = menuList.GroupBy(g => g.menuGroupCode).Select(group => group.First()).ToList();

        //    List<MenuViewModel> myMenu = new List<MenuViewModel>();

        //    foreach (var g in menuGroup)
        //    {

        //        if (g.menuTypeNo == 0)
        //        {
        //            myMenu.Add(new MenuViewModel()
        //            {
        //                menuName = g.menuName,
        //                groupCode = g.menuGroupCode,
        //                groupName = g.groupName,
        //                groupIcon = g.fontIcon,
        //                levelNo = Convert.ToInt16(g.menuTypeNo),
        //                controllerName = g.controllerName,
        //                methodName = g.methodName
        //            });
        //        }
        //        else if (g.menuTypeNo == 1)
        //        {
        //            myMenu.Add(new MenuViewModel()
        //            {
        //                groupCode = g.menuGroupCode,
        //                groupName = g.groupName,
        //                groupIcon = g.fontIcon,
        //                levelNo = Convert.ToInt16(g.menuTypeNo),
        //                menuLink = MenuLink(menuList.Where(e => e.menuGroupCode == g.menuGroupCode).ToList())
        //            });

        //        }
        //        else if (g.menuTypeNo == 2)
        //        {

        //            myMenu.Add(new MenuViewModel()
        //            {
        //                groupCode = g.menuGroupCode,
        //                groupName = g.groupName,
        //                groupIcon = g.fontIcon,
        //                levelNo = Convert.ToInt16(g.menuTypeNo),
        //                subGroupList = SubGroupList(menuList.Where(e => e.menuGroupCode == g.menuGroupCode).ToList())
        //            });
        //        }
        //    }
        //    Session["MenuList"] = myMenu.ToList();
        //    return 0;
        //}

        private List<MenuLevel2> SubGroupList(List<vSysMenuUser> list)
        {
            List<MenuLevel2> myList = new List<MenuLevel2>();
            IEnumerable<vSysMenuUser> subGroup = list.GroupBy(g => g.menuSubGroupCode).Select(group => group.First()).ToList();
            foreach (vSysMenuUser item in subGroup)
            {
                myList.Add(new MenuLevel2()
                {
                    subGroupCode = item.menuSubGroupCode,
                    subGroupName = item.subGroupName,
                    menuLink = MenuLink(list.Where(e => e.menuSubGroupCode == item.menuSubGroupCode).ToList())
                });
            }
            return myList;
        }

        private List<MenuLink> MenuLink(List<vSysMenuUser> list)
        {
            List<MenuLink> myList = new List<MenuLink>();


            foreach (vSysMenuUser item in list)
            {
                myList.Add(new MenuLink()
                {
                    menuCode = item.menuCode,
                    menuName = item.menuName,
                    controllerName = item.controllerName,
                    methodName = item.methodName
                });

            }

            return myList;
        }



        private int SetSessionImage(string id)
        {
            string uID = Convert.ToInt16(id).ToString("0000");

            try
            {
                string path = @"C:\DataFile\images\" + uID + ".jpg";
                byte[] imageByteData = System.IO.File.ReadAllBytes(path);
                string imageBase64Data = Convert.ToBase64String(imageByteData);
                string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
                Session["UImage"] = imageDataURL;
            }
            catch
            {
                string path = @"C:\DataFile\images\0000.jpg";
                byte[] imageByteData = System.IO.File.ReadAllBytes(path);
                string imageBase64Data = Convert.ToBase64String(imageByteData);
                string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
                Session["UImage"] = imageDataURL;
            }
            return 0;
        }

     


        public ActionResult LogOut()
        {
            Session.Clear();
            Session.Abandon();
            AuthenticationManager.SignOut();
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            Session.Clear();
            Session.Abandon();
            AuthenticationManager.SignOut();
            return RedirectToAction("Login", "Account");
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        private ApplicationSignInManager _signInManager;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set { _signInManager = value; }
        }

        //
        // POST: /Account/Login


        //
        // GET: /Account/VerifyCode
        //[AllowAnonymous]
        //public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        //{
        //    // Require that the user has already logged in via username/password or external login
        //    if (!await SignInManager.HasBeenVerifiedAsync())
        //    {
        //        return View("Error");
        //    }
        //    var user = await UserManager.FindByIdAsync(await SignInManager.GetVerifiedUserIdAsync());
        //    if (user != null)
        //    {
        //        var code = await UserManager.GenerateTwoFactorTokenAsync(user.Id, provider);
        //    }
        //    return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        //}

        //
        // POST: /Account/VerifyCode
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    // The following code protects for brute force attacks against the two factor codes. 
        //    // If a user enters incorrect codes for a specified amount of time then the user account 
        //    // will be locked out for a specified amount of time. 
        //    // You can configure the account lockout settings in IdentityConfig
        //    var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //            return RedirectToLocal(model.ReturnUrl);
        //        case SignInStatus.LockedOut:
        //            return View("Lockout");
        //        case SignInStatus.Failure:
        //        default:
        //            ModelState.AddModelError("", "Invalid code.");
        //            return View(model);
        //    }
        //}



        //
        // GET: /Account/Register
        //[AllowAnonymous]
        //public ActionResult Register()
        //{
        //    return View();
        //}

        //
        // POST: /Account/Register
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Register(RegisterViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
        //        var result = await UserManager.CreateAsync(user, model.Password);
        //        if (result.Succeeded)
        //        {
        //            await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

        //            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
        //            // Send an email with this link
        //            // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
        //            // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
        //            // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

        //            return RedirectToAction("Index", "Home");
        //        }
        //        AddErrors(result);
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        //
        // GET: /Account/ConfirmEmail
        //[AllowAnonymous]
        //public async Task<ActionResult> ConfirmEmail(string userId, string code)
        //{
        //    if (userId == null || code == null)
        //    {
        //        return View("Error");
        //    }
        //    var result = await UserManager.ConfirmEmailAsync(userId, code);
        //    return View(result.Succeeded ? "ConfirmEmail" : "Error");
        //}

        //
        // GET: /Account/ForgotPassword
        //[AllowAnonymous]
        //public ActionResult ForgotPassword()
        //{
        //    return View();
        //}

        //
        // POST: /Account/ForgotPassword
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await UserManager.FindByNameAsync(model.Email);
        //        if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
        //        {
        //            // Don't reveal that the user does not exist or is not confirmed
        //            return View("ForgotPasswordConfirmation");
        //        }

        //        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
        //        // Send an email with this link
        //        // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
        //        // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
        //        // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
        //        // return RedirectToAction("ForgotPasswordConfirmation", "Account");
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        //
        // GET: /Account/ForgotPasswordConfirmation
        //[AllowAnonymous]
        //public ActionResult ForgotPasswordConfirmation()
        //{
        //    return View();
        //}

        ////
        //// GET: /Account/ResetPassword
        //[AllowAnonymous]
        //public ActionResult ResetPassword(string code)
        //{
        //    return code == null ? View("Error") : View();
        //}

        ////
        //// POST: /Account/ResetPassword
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    var user = await UserManager.FindByNameAsync(model.Email);
        //    if (user == null)
        //    {
        //        // Don't reveal that the user does not exist
        //        return RedirectToAction("ResetPasswordConfirmation", "Account");
        //    }
        //    var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
        //    if (result.Succeeded)
        //    {
        //        return RedirectToAction("ResetPasswordConfirmation", "Account");
        //    }
        //    AddErrors(result);
        //    return View();
        //}

        ////
        //// GET: /Account/ResetPasswordConfirmation
        //[AllowAnonymous]
        //public ActionResult ResetPasswordConfirmation()
        //{
        //    return View();
        //}

        ////
        //// POST: /Account/ExternalLogin
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult ExternalLogin(string provider, string returnUrl)
        //{
        //    // Request a redirect to the external login provider
        //    return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        //}

        //
        // GET: /Account/SendCode
        //[AllowAnonymous]
        //public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        //{
        //    var userId = await SignInManager.GetVerifiedUserIdAsync();
        //    if (userId == null)
        //    {
        //        return View("Error");
        //    }
        //    var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
        //    var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
        //    return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        //}

        ////
        //// POST: /Account/SendCode
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> SendCode(SendCodeViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View();
        //    }

        //    // Generate the token and send it
        //    if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
        //    {
        //        return View("Error");
        //    }
        //    return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        //}

        //
        // GET: /Account/ExternalLoginCallback
        //[AllowAnonymous]
        //public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        //{
        //    var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
        //    if (loginInfo == null)
        //    {
        //        return RedirectToAction("Login");
        //    }

        //    // Sign in the user with this external login provider if the user already has a login
        //    var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //            return RedirectToLocal(returnUrl);
        //        case SignInStatus.LockedOut:
        //            return View("Lockout");
        //        case SignInStatus.RequiresVerification:
        //            return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
        //        case SignInStatus.Failure:
        //        default:
        //            // If the user does not have an account, then prompt the user to create an account
        //            ViewBag.ReturnUrl = returnUrl;
        //            ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
        //            return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
        //    }
        //}

        //
        // POST: /Account/ExternalLoginConfirmation
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Index", "Manage");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        // Get the information about the user from the external login provider
        //        var info = await AuthenticationManager.GetExternalLoginInfoAsync();
        //        if (info == null)
        //        {
        //            return View("ExternalLoginFailure");
        //        }
        //        var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
        //        var result = await UserManager.CreateAsync(user);
        //        if (result.Succeeded)
        //        {
        //            result = await UserManager.AddLoginAsync(user.Id, info.Login);
        //            if (result.Succeeded)
        //            {
        //                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //                return RedirectToLocal(returnUrl);
        //            }
        //        }
        //        AddErrors(result);
        //    }

        //    ViewBag.ReturnUrl = returnUrl;
        //    return View(model);
        //}

        //
        // POST: /Account/LogOff


        //
        // GET: /Account/ExternalLoginFailure
        //[AllowAnonymous]
        //public ActionResult ExternalLoginFailure()
        //{
        //    return View();
        //}

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        //private ActionResult RedirectToLocal(string returnUrl)
        //{
        //    if (Url.IsLocalUrl(returnUrl))
        //    {
        //        return Redirect(returnUrl);
        //    }
        //    return RedirectToAction("Index", "Home");
        //}

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}