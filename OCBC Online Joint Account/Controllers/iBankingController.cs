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
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Newtonsoft.Json;
using RestSharp;
using System.IO;
using System.Threading;
using System.Text;
using OCBC_Online_Joint_Account.Models;
using System.Globalization;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Newtonsoft.Json.Linq;

namespace OCBC_Joint_Account_Application.Controllers
{
    public class iBankingController : Controller
    {
        private CustomerDAL customerContext = new CustomerDAL();

        public ActionResult Login()
        {
            HttpContext.Session.SetString("PageType", "iBanking");
            return View();
        }
        [HttpPost]
        public ActionResult Login(Customer customer)
        {
            Customer selectedCust = customerContext.GetCustomerByiBUsername(customer.iBUsername);
            if (selectedCust != null)
            {
                if (customer.iBPin == selectedCust.iBPin)
                {
                    customer = selectedCust;
                    HttpContext.Session.SetString("ApplyMethod", "iBanking");
                    HttpContext.Session.SetObjectAsJson("iBankingDetails", customer);
                    return RedirectToAction("Form","Account360");
                }
            }
            ViewData["iBankingError"] = "Access code or password incorrect!";
            return View(customer);
        }
    }
}
