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


namespace OCBC_Joint_Account_Application.Controllers
{
    public class Account360Controller : Controller
    {
        private SingpassDAL singpassContext = new SingpassDAL();
        private CustomerDAL customerContext = new CustomerDAL();
        private ApplicationDAL applicationContext = new ApplicationDAL();
        Customer storedApplicant = new Customer();

        bool hasScanned = false;
        bool toRedirect = false;
        bool continueMobile = false;

        
        public ActionResult ApplyOnline(string? JAC)
        {
            
            HttpContext.Session.SetString("PageType", "Account360");
            /*
            if(JAC != null)
            {
                //QR: Reset QR settings
                var resetQR =
                    "{\"qr_data\":\"ocbc_jointacc_digital_create\"," +
                    "\"custNRIC\":null," +
                    "\"hasScanned\":" + hasScanned +"," +
                    "\"toRedirect\":" + toRedirect + "," +
                    "\"continueMobile\":false," +
                    "\"isJointApplicant\":true," +
                    "\"id\":0}";

                var client1 = new RestClient("https://pfdocbcdb-5763.restdb.io/rest/qr-response/618de4c99402c24f00010d9b");
                var request1 = new RestRequest(Method.PUT);
                request1.AddHeader("cache-control", "no-cache");
                request1.AddHeader("x-apikey", "f3e68097c1a4127f4472d8730dcb3399f2d14");
                request1.AddHeader("content-type", "application/json");
                request1.AddParameter("application/json", resetQR, ParameterType.RequestBody);
                IRestResponse response1 = client1.Execute(request1);
            }

            //QR: Wait for response from iBanking App
            var client = new RestClient("https://pfdocbcdb-5763.restdb.io/rest/qr-response/618de4c99402c24f00010d9b");
            var request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("x-apikey", "f3e68097c1a4127f4472d8730dcb3399f2d14");
            request.AddHeader("content-type", "application/json");
            IRestResponse response = client.Execute(request);
            QR qr = JsonConvert.DeserializeObject<QR>(response.Content);

            if (qr.hasScanned == true && qr.toRedirect == true && qr.continueMobile == false)
            {
                hasScanned = true;
                toRedirect = true;
                HttpContext.Session.SetString("iBankingLogin", qr.custNRIC);
                return RedirectToAction("JointApplicant", "Account360");
            }*/
            return View();
        }
        

        public ActionResult Identity()
        {
            if (HttpContext.Session.GetString("Applicant") == null)
            {
                return RedirectToAction("Index", "Home");
            }

            HttpContext.Session.SetString("PageType", "Account360");

            //Get Mobile Number
            string mobileNum = null;
            foreach (Singpass sp in singpassContext.GetSingpassByNRIC(HttpContext.Session.GetString("Applicant")))
            {
                mobileNum = sp.MobileNum;
            }

            //Generate 6-digit OTP
            Random rnd = new Random();
            int OTP = rnd.Next(100000, 999999);

            //Disable to save money
            //OTP API by Twilio
            /*var accountSid = "AC900a65cf35b142ba9d231968f7975595";
            var authToken = "900f7cf484248daa85bccb918be28908";
            TwilioClient.Init(accountSid, authToken);
            var messageOptions = new CreateMessageOptions(new PhoneNumber("+65" + mobileNum));
            messageOptions.MessagingServiceSid = "MG9dc1a6ffbac9048864eaadfda51637fc";
            messageOptions.Body = "Your OCBC OTP is " + OTP;
            var message = MessageResource.Create(messageOptions);
            Console.WriteLine(message.Body);**/
          
            HttpContext.Session.SetInt32("OTP", OTP);

            ViewData["A"] = OTP;
            return View();
        }

        [HttpPost]
        public ActionResult Identity(Account360ViewModel a360)
        {
            HttpContext.Session.SetString("PageType", "Account360");

            if (a360.OTP == HttpContext.Session.GetInt32("OTP"))
            {
                return RedirectToAction("Form", "Account360");
            }

            ViewData["Invalid"] = "Invalid OTP";
            return View();
        }

        private List<SelectListItem> Salutation = new List<SelectListItem>();
        private List<SelectListItem> CountryOfBirth = new List<SelectListItem>();
        private List<SelectListItem> Nationality = new List<SelectListItem>();
        private List<SelectListItem> Gender = new List<SelectListItem>();
        private List<SelectListItem> MaritialStatus = new List<SelectListItem>();
        private List<SelectListItem> AnnualIncome = new List<SelectListItem>();
        private List<SelectListItem> Occupation = new List<SelectListItem>();
        private List<SelectListItem> YearsInEmployment = new List<SelectListItem>();

        public Account360Controller()
        {
            //Populate Salutation
            Salutation.Add(new SelectListItem { Value = "Dr",  Text = "Dr"});
            Salutation.Add(new SelectListItem { Value = "Mdm", Text = "Mdm" });
            Salutation.Add(new SelectListItem { Value = "Miss", Text = "Miss" });
            Salutation.Add(new SelectListItem { Value = "Mr", Text = "Mr" });
            Salutation.Add(new SelectListItem { Value = "Mrs", Text = "Mrs" });
            Salutation.Add(new SelectListItem { Value = "Ms", Text = "Ms" });

            //Populate Gender
            Gender.Add(new SelectListItem { Value = "Male", Text = "Male" });
            Gender.Add(new SelectListItem { Value = "Female", Text = "Female" });

            //Populate Maritial Status
            MaritialStatus.Add(new SelectListItem { Value = "Single", Text = "Single" });
            MaritialStatus.Add(new SelectListItem { Value = "Married", Text = "Married" });
            MaritialStatus.Add(new SelectListItem { Value = "Widowed", Text = "Widowed" });
            MaritialStatus.Add(new SelectListItem { Value = "Divorced", Text = "Divorced" });
            MaritialStatus.Add(new SelectListItem { Value = "Married but seprated", Text = "Married but seprated" });

            //Populate Annual Income
            AnnualIncome.Add(new SelectListItem { Value = "1", Text = "Less Than 30,000" });
            AnnualIncome.Add(new SelectListItem { Value = "2", Text = "30,000 - 49,000" });
            AnnualIncome.Add(new SelectListItem { Value = "3", Text = "50,000 - 99,999" });
            AnnualIncome.Add(new SelectListItem { Value = "4", Text = "100,000 - 149,000" });
            AnnualIncome.Add(new SelectListItem { Value = "5", Text = "150,000 - 199,000" });
            AnnualIncome.Add(new SelectListItem { Value = "6", Text = "Above 200,000" });

            //Populate Occupation
            Occupation.Add(new SelectListItem { Value = "1", Text = "Architect" });
            Occupation.Add(new SelectListItem { Value = "2", Text = "Doctor/Dentist" });
            Occupation.Add(new SelectListItem { Value = "3", Text = "Engineer" });
            Occupation.Add(new SelectListItem { Value = "4", Text = "IT Professional" });
            Occupation.Add(new SelectListItem { Value = "5", Text = "Legal Professional/Lawyer" });
            Occupation.Add(new SelectListItem { Value = "6", Text = "Student" });

            //Populate Years In Employment
            YearsInEmployment.Add(new SelectListItem { Value = "< 1", Text = "< 1" });
            for (int i = 1; i <= 40; i++)
            {
                YearsInEmployment.Add(new SelectListItem { Value = Convert.ToString(i), Text = Convert.ToString(i) });
            }
            YearsInEmployment.Add(new SelectListItem { Value = "> 40", Text = "> 40" });
        }

        public async Task<ActionResult> Form()
        {
            HttpContext.Session.SetString("PageType", "Account360");

            //Populate CountryOfBirth & Nationality
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://raw.githubusercontent.com/");
            HttpResponseMessage response = await client.GetAsync("Dinuks/country-nationality-list/master/countries.json");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                List<Country> country = JsonConvert.DeserializeObject<List<Country>>(data);

                foreach(Country c in country)
                {
                    CountryOfBirth.Add(new SelectListItem { Value = c.en_short_name, Text = c.en_short_name });
                    Nationality.Add(new SelectListItem { Value = c.nationality, Text = c.nationality });
                }            
            }

            ViewData["CountryOfBirth"] = CountryOfBirth;
            ViewData["Nationality"] = Nationality;
            ViewData["Salutation"] = Salutation;
            ViewData["Gender"] = Gender;
            ViewData["MaritialStatus"] = MaritialStatus;
            ViewData["AnnualIncome"] = AnnualIncome;
            ViewData["Occupation"] = Occupation;
            ViewData["YearsInEmployment"] = YearsInEmployment;

            Account360ViewModel ac360 = new Account360ViewModel();

            ac360.DateOfBirth = DateTime.Today;

            if (HttpContext.Session.GetString("Applicant") != "")
            {
                foreach (Singpass sp in singpassContext.GetSingpassByNRIC(HttpContext.Session.GetString("Applicant")))
                {
                    ac360.FullName = sp.Name;
                    ac360.NRIC = sp.NRIC;
                    ac360.DateOfBirth = sp.DoB;
                    ac360.CountryOfBirth = sp.CountryOfBirth;
                    ac360.Nationality = sp.Nationality;
                    if(sp.Gender == "M")
                    {
                        ac360.Gender = "Male";
                    }
                    else
                    {
                        ac360.Gender = "Female";
                    }
                    ac360.EmailAddress = sp.Email;
                    ac360.MobileNum = sp.MobileNum;
                    ac360.Address = sp.RegisteredAddress;
                }              
            }

            return View(ac360);
        }

        [HttpPost]
        public ActionResult Form(Account360ViewModel a360)
        { 
            storedApplicant.CustNRIC = a360.NRIC;
            storedApplicant.Salutation = a360.Salutation;
            storedApplicant.CustName = a360.FullName;
            storedApplicant.Email = a360.EmailAddress;
            storedApplicant.ContactNo = a360.MobileNum;
            storedApplicant.Gender = a360.Gender;
            storedApplicant.MaritialStatus = null;
            storedApplicant.iBUsername = null;
            storedApplicant.iBPin = null;
            storedApplicant.Address = a360.Address;
            storedApplicant.ContactNo = a360.CountryOfBirth;
            storedApplicant.Nationality = a360.Nationality;
            storedApplicant.DateOfBirth = a360.DateOfBirth;
            storedApplicant.EmployerName = a360.Employer;
            storedApplicant.Occupation = a360.Occupation;
            storedApplicant.Income = a360.AnnualIncome;

            Application mainApplication = new Application()
            {
                CustNRIC = storedApplicant.CustNRIC,
                AccountTypeID = 2,
                Status = "Pending",
                CreationDate = DateTime.Today,
                JointApplicantCode = $"J{DateTime.Today.Day}{DateTime.Today.Month}{storedApplicant.CustNRIC.Substring(5, 3)}"
            };

            //applicationContext.Add(mainApplication);

            TempData["Code"] = mainApplication.JointApplicantCode;
            return RedirectToAction("JointApplicant", "Account360", mainApplication);
        }

        public ActionResult Upload()
        {
            return View();
        }

        public ActionResult JointApplicant()
        {
            //QR: Reset QR settings
            var resetQR =
                "{\"qr_data\":\"ocbc_jointacc_digital_create\"," +
                "\"custNRIC\":null," +
                "\"hasScanned\":false," +
                "\"toRedirect\":false," +
                "\"continueMobile\":false," +
                "\"isJointApplicant\":false," +
                "\"id\":0}";

            var client = new RestClient("https://pfdocbcdb-5763.restdb.io/rest/qr-response/618de4c99402c24f00010d9b");
            var request = new RestRequest(Method.PUT);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("x-apikey", "f3e68097c1a4127f4472d8730dcb3399f2d14");
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", resetQR, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            HttpContext.Session.SetString("PageType", "Account360");
            ViewData["Salutation"] = Salutation;
            return View();
        }

        [HttpPost]
        public ActionResult JointApplicant(JointApplicantViewModel jointApplicant)
        {
            jointApplicant.MainApplicantName = storedApplicant.CustName;
            //Send SMS to joint applicant
            return RedirectToAction("Verify", "Account360");
        }
        public ActionResult Verify()
        {
            return View();
        }
    }
}
