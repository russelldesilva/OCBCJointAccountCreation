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
using OCBC_Joint_Account_Application.Models;
using OCBC_Joint_Account_Application.DAL;
using Newtonsoft.Json;
using RestSharp;
using System.IO;
using System.Threading;
using System.Text;
using System.Globalization;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Newtonsoft.Json.Linq;
using OCBC_Online_Joint_Account.Models;

namespace OCBC_Joint_Account_Application.Controllers
{
    public class iBankingController : Controller
    {
        private CustomerDAL customerContext = new CustomerDAL();
        private ApplicationDAL applicationContext = new ApplicationDAL();
        public ActionResult Login()
        {
            HttpContext.Session.SetString("PageType", "iBanking");
            return View();
        }

        [HttpPost]
        public ActionResult Login(Customer customer)
        {
            Customer c = customerContext.GetCustomerByiBUsername(customer.iBUsername);
            if (c != null)
            {
                if (customer.iBPin == c.iBPin)
                {
                    HttpContext.Session.SetString("ApplyMethod", "iBanking");
                    HttpContext.Session.SetString("iBankingLogin", c.CustNRIC);
                    
                    if(HttpContext.Session.GetString("JAC") != null)
                    {
                        foreach (Application a in applicationContext.GetApplicationByJointApplicantionCode(HttpContext.Session.GetString("JAC")))
                        {
                            HttpContext.Session.SetString("MainApplicantNRIC", a.CustNRIC);
                        }
                    }
                    HttpContext.Session.SetString("Applicant", c.CustNRIC);
                    return RedirectToAction("Identity", "Account360");
                   
                }
            }
            ViewData["iBankingError"] = "Access code or password incorrect!";
            return View(customer);
        }
    }
}
