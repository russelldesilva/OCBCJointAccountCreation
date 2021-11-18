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
                    Console.WriteLine("HttpContext: " + HttpContext.Session.GetString("CustSingpass"));
                    Console.WriteLine("ViewData: " + Convert.ToString(ViewData["CustSingpass"]));
                    Console.WriteLine("TempData: " + Convert.ToString(TempData["CustSingpass"]));
                    if (HttpContext.Session.GetString("CustSingpass") == "existingCustomer")
                    {
                        Console.WriteLine(singpassLogin.NRIC);
                        TempData["CustSingpass"] = "existingCustomer";
                        HttpContext.Session.SetString("iBankingLogin", singpassLogin.NRIC);
                    }
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
