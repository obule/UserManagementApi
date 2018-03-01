using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace BTS__User__Mangement__API.Models
{
    public class RESTAuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        private const string _securityToken = "token"; // Name of the url parameter.
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (Authorize(filterContext))
            {
                return;
            }
            HandleUnauthorizedRequest(filterContext);
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);
        }
        public static bool  Authorize(AuthorizationContext actionContext)
        {
            try
            {
                HttpRequestBase request = actionContext.RequestContext.HttpContext.Request;
                string token = request.Params[_securityToken];
                bool result= NewToken.IsTokenValid(token, CommonManager.GetIP(request), request.UserAgent);
                if (result == true)
                {
                    //Increase the token Expiration date in the Database
                    return true;
                }
                else
                {
                    return false;
                }
                
                
            }
            catch (Exception)
            {
                return false;
            }
        }
    }


    public static class CommonManager
    {
        public static string GetIP(HttpRequestBase request)
        {
            string ip = request.Headers["X-Forwarded-For"]; // AWS compatibility
            if (string.IsNullOrEmpty(ip))
            {
                ip = request.UserHostAddress;
            }
            return ip;
        }
    }
    //public class IpController : Controller
    //{
    //    [HttpGet]
    //    public string Index()
    //    {
    //        return CommonManager.GetIP(Request);
    //    }
    //}
}