using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Newtonsoft.Json;
using OCBC_Joint_Account_Application.Models;
using OCBC_Joint_Account_Application.DAL;

namespace OCBC_Joint_Account_Application.Controllers
{
    public class SingpassController : Controller
    {
        private SingpassDAL singpassContext = new SingpassDAL();

        public ActionResult Login(string customer)
        {
            TempData["CustSingpass"] = customer;
            HttpContext.Session.SetString("PageType", "Singpass");
            HttpContext.Session.Remove("Applicant");
            return View();
        }

        [HttpPost]
        public ActionResult Login(SingpassViewModel singpassLogin)
        {
            foreach (Singpass sp in singpassContext.GetSingpassByNRIC(singpassLogin.NRIC))
            {
                if(sp.NRIC == singpassLogin.NRIC)
                {
                    HttpContext.Session.SetString("Applicant", singpassLogin.NRIC);
                    return RedirectToAction("Auth", "Singpass");
                }
            }
            ViewData["Incorrect"] = "You have entered an invalid Singpass ID or password.";
            return View();
        }

        public ActionResult Auth()
        {
            HttpContext.Session.Remove("PageType");
            return View();
        }

        [HttpPost]
        public ActionResult Auth(Singpass sp)
        {
            // Set apply method
            HttpContext.Session.SetString("ApplyMethod", "Singpass");
            return RedirectToAction("Identity", "Account360");
        }
    }
}
