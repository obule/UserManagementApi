using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BTS__User__Mangement__API.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        public ActionResult Verify()
        {
            return View();
        }

        public ActionResult Verify_Fail()
        {
            return View();
        }

        public ActionResult Adminverify()
        {
            return View();
        }

        public ActionResult AdminVerify_Fail()
        {
            return View();
        }
    }
}