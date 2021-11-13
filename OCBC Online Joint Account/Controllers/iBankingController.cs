using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCBC_Joint_Account_Application.Controllers
{
    public class iBankingController : Controller
    {
        public ActionResult Login()
        {
            HttpContext.Session.SetString("PageType", "iBanking");
            return View();
        }
    }
}
