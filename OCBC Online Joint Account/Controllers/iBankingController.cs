using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OCBC_Joint_Account_Application.DAL;
using OCBC_Joint_Account_Application.Models;
using OCBC_Online_Joint_Account.Models;

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
