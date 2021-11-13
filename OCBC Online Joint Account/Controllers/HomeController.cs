using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using RestSharp;
using Newtonsoft.Json;
using OCBC_Joint_Account_Application.Models;
using OCBC_Joint_Account_Application.DAL;


namespace OCBC_Joint_Account_Application.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public ActionResult Index()
        {
            HttpContext.Session.SetString("PageType", "OCBC");
            HttpContext.Session.SetString("Applicant", "");

            return View();      
        }

        public ActionResult Privacy()
        {
            return View();
        }
    }
}
