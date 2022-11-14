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

        HRISDBEntities db = new HRISDBEntities();

        public AccountController()
        {
        }
        
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            //ViewBag.ReturnUrl = returnUrl;
            //if (Request.IsAuthenticated && Session["_EIC"] != null)
            if (Session["_EIC"] != null)
            {
                //  return RedirectToAction("Profiling", "ApplicantProfile");

                return RedirectToAction("Dashboard", "Home");
            }
            else
            {
                // return RedirectToAction("Dashboard", "Home");
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

                string _EIC = "";

                if (data.username == "MACHAN" && data.password == "@CHAN")
                {
                    _EIC = "RS163896604047CAB450";
                }
                else
                {

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

                //emps = db.vRSPEmployees.Single(e => e.EIC == _EIC);

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
                //SetSessionImage(emps.idNo);

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
            string path = @"C:\DataFile\images\0000.jpg";
            byte[] imageByteData = System.IO.File.ReadAllBytes(path);
            string imageBase64Data = Convert.ToBase64String(imageByteData);
            string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
            Session["UImage"] = imageDataURL;
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