using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
 
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace DDNHRIS.Models
{
    public class SessionTimeoutAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;
            try
            {
                if (HttpContext.Current.Session["_EIC"] == null)
                {
                    filterContext.Result = new RedirectResult("~/Account/LogOut");
                    return;
                }
            }

            catch (Exception)
            {
                filterContext.Result = new RedirectResult("~/Account/LogOut");
                return;
            }
            base.OnActionExecuting(filterContext);
        }

    }
}