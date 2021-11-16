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

namespace OCBC_Joint_Account_Application.Controllers
{
    public class Account360Controller : Controller
    {
        private SingpassDAL singpassContext = new SingpassDAL();
        private CustomerDAL customerContext = new CustomerDAL();
        private ApplicationDAL applicationContext = new ApplicationDAL();
        Account360ViewModel applicants = new Account360ViewModel();

        private List<SelectListItem> Salutation = new List<SelectListItem>();
        private List<SelectListItem> CountryOfBirth = new List<SelectListItem>();
        private List<SelectListItem> Nationality = new List<SelectListItem>();
        private List<SelectListItem> Gender = new List<SelectListItem>();
        private List<SelectListItem> MaritialStatus = new List<SelectListItem>();
        private List<SelectListItem> AnnualIncome = new List<SelectListItem>();
        private List<SelectListItem> Occupation = new List<SelectListItem>();
        private List<SelectListItem> YearsInEmployment = new List<SelectListItem>();
        private List<string> singaporean = new List<string> { "I am a Singaporean Citizen/Permanent Resident", "I am a Foreigner working/studying or residing in Singapore" };

        public Account360Controller()
        {
            //Populate Salutation
            Salutation.Add(new SelectListItem { Value = "Dr", Text = "Dr" });
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

        /**==========================
              APPLYONLINE.CSHTML
         ==========================**/

        public ActionResult ApplyOnline(int? AT, string? JAC)
        {
            HttpContext.Session.SetString("PageType", "Account360");
     
            if (JAC != null)
            {
                HttpContext.Session.SetString("JAC", JAC);
                InsertQRForJointApplicant(AT, JAC);
                return RedirectToAction("ApplyOnline", "Account360");
            }
            checkJAC(HttpContext.Session.GetString("JAC"));
            if (ResponseQR() == true)
            {
                return RedirectToAction("JointApplicant", "Account360");
            }  
            return View();     
        }

        /**==========================
              IDENTITY.CSHTML
        ==========================**/

        public ActionResult Identity()
        {
            if (HttpContext.Session.GetString("Applicant") == null)
            {
                return RedirectToAction("Index", "Home");
            }

            checkJAC(HttpContext.Session.GetString("JAC"));
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
            checkJAC(HttpContext.Session.GetString("JAC"));
            HttpContext.Session.SetString("PageType", "Account360");

            if (a360.OTP == HttpContext.Session.GetInt32("OTP"))
            {
                return RedirectToAction("Form", "Account360");
            }

            ViewData["Invalid"] = "Invalid OTP";
            return View();
        }

        /**==========================
                 FORM.CSHTML
        ==========================**/

        public async Task<ActionResult> Form()
        {
            checkJAC(HttpContext.Session.GetString("JAC"));
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

            // Check for Singpass then run code to pull from singpass
            if (HttpContext.Session.GetString("ApplyMethod") == "Singpass")
            {
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
                        if (sp.Gender == "M")
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
            // Else if iBanking run code to pull from iBanking
            else if (HttpContext.Session.GetString("ApplyMethod") == "iBanking")
            {
                return View();
            }

            // Else if Scan run code to pull from Scan
            else if (HttpContext.Session.GetString("ApplyMethod") == "Scan")
            {
                return View();
            }

            // Else show some error
            else
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Form(Account360ViewModel a360)
        {
            Account360ViewModel mainApplication = new Account360ViewModel();
            mainApplication.Salutation = a360.Salutation;
            mainApplication.FullName = a360.FullName;
            mainApplication.NRIC = a360.NRIC;
            mainApplication.DateOfBirth = a360.DateOfBirth;
            mainApplication.CountryOfBirth = a360.CountryOfBirth;
            mainApplication.Nationality = a360.Nationality;
            mainApplication.Gender = a360.Gender;
            mainApplication.MaritialStatus = a360.MaritialStatus;
            mainApplication.MobileNum = a360.MobileNum;
            mainApplication.EmailAddress = a360.EmailAddress;
            mainApplication.Address = a360.Address;
            mainApplication.Employer = a360.Employer;
            mainApplication.YearsInEmployment = a360.YearsInEmployment;
            mainApplication.Occupation = a360.Occupation;
            mainApplication.AnnualIncome = a360.AnnualIncome;

            mainApplication.JointApplicantCode = $"J{DateTime.Today.Day}{DateTime.Today.Month}{mainApplication.NRIC.Substring(5, 3)}";

            HttpContext.Session.SetObjectAsJson("ApplicantsDetails", mainApplication);

            //Application mainApplication = new Application()
            //{
            //    CustNRIC = storedApplicant.CustNRIC,
            //    AccountTypeID = 2,
            //    Status = "Pending",
            //    CreationDate = DateTime.Today,
            //    JointApplicantCode = $"J{DateTime.Today.Day}{DateTime.Today.Month}{storedApplicant.CustNRIC.Substring(5, 3)}"
            //};


            return RedirectToAction("JointApplicant", "Account360");
        }

        /**==========================
                UPLOAD.CSHTML
        ==========================**/

        public ActionResult Upload()
        {
            checkJAC(HttpContext.Session.GetString("JAC"));
            HttpContext.Session.SetString("ApplyMethod", "Scan");
            ViewData["SingaporeanSelection"] = singaporean;

            CustApplication custApplication = new CustApplication
            {
                Singaporean = singaporean[0]
            };
            
            return View("Upload", custApplication);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(CustApplication custApplication)
        {
            checkJAC(HttpContext.Session.GetString("JAC"));
            ViewData["SingaporeanSelection"] = singaporean;
            CustApplication custApplication1 = new CustApplication
            {
                Singaporean = singaporean[0]
            };

            ViewData["UploadMessage"] = "File uploaded successfully.";
            if (custApplication.CustProofOfResidenceUpload != null && custApplication.CustProofOfResidenceUpload.Length > 0)
            {
                try
                {
                    string fileExt = Path.GetExtension(custApplication.CustProofOfResidenceUpload.FileName);
                    string uploadedFile = String.Format("residence_proof" + fileExt);
                    string savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\applicationdocs\\", uploadedFile);
                    using (var fileSteam = new FileStream(savePath, FileMode.Create))
                    {
                        await custApplication.CustProofOfResidenceUpload.CopyToAsync(fileSteam);
                    }
                    ViewData["UploadColor"] = "lime";
                    ViewData["UploadMessage"] = "Upload Successful!";
                }
                catch (IOException)
                {
                    ViewData["UploadColor"] = "red";
                    ViewData["UploadMessage"] = "Upload Failed!";
                    return View("Upload", custApplication);
                }
                catch (Exception ex)
                {
                    ViewData["UploadMessage"] = ex.Message;
                    return View("Upload", custApplication);
                }
            }
            if (custApplication.CustNRICFrontUpload != null && custApplication.CustNRICFrontUpload.Length > 0)
            {
                try
                {
                    string fileExt = Path.GetExtension(custApplication.CustNRICFrontUpload.FileName);
                    string uploadedFile = String.Format("nric_front" + fileExt);
                    string savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\applicationdocs\\", uploadedFile);
                    using (var fileSteam = new FileStream(savePath, FileMode.Create))
                    {
                        await custApplication.CustNRICFrontUpload.CopyToAsync(fileSteam);
                    }
                    ViewData["UploadColor"] = "lime";
                    ViewData["UploadMessage"] = "Upload Successful!";
                }
                catch (IOException)
                {
                    ViewData["UploadColor"] = "red";
                    ViewData["UploadMessage"] = "Upload Failed!";
                    return View("Upload", custApplication);
                }
                catch (Exception ex)
                {
                    ViewData["UploadMessage"] = ex.Message;
                    return View("Upload", custApplication);
                }
            }
            if (custApplication.CustNRICBackUpload != null && custApplication.CustNRICBackUpload.Length > 0)
            {
                try
                {
                    string fileExt = Path.GetExtension(custApplication.CustNRICBackUpload.FileName);
                    string uploadedFile = String.Format("nric_back" + fileExt);
                    string savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\applicationdocs\\", uploadedFile);
                    using (var fileSteam = new FileStream(savePath, FileMode.Create))
                    {
                        await custApplication.CustNRICBackUpload.CopyToAsync(fileSteam);
                    }
                    ViewData["UploadColor"] = "lime";
                    ViewData["UploadMessage"] = "Upload Successful!";
                }
                catch (IOException)
                {
                    ViewData["UploadColor"] = "red";
                    ViewData["UploadMessage"] = "Upload Failed!";
                    return View("Upload", custApplication);
                }
                catch (Exception ex)
                {
                    ViewData["UploadMessage"] = ex.Message;
                    return View("Upload", custApplication);
                }
            }

            // NRIC Front OCR API - DO NOT DELETE. COMMENTING OUT TO REDUCE API CALL USAGE.
            //var client = new RestClient("https://app.nanonets.com/api/v2/OCR/Model/3de1189e-0087-4b80-8954-813aa4b0aaac/LabelFile/");
            //var request = new RestRequest(Method.POST);
            //request.AddHeader("authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes("_9xwEpzEdi3gevc7Oy-SucehAswdtRFG:")));
            //request.AddHeader("accept", "Multipart/form-data");
            //request.AddFile("file", ".\\wwwroot\\applicationdocs\\nric_front.jpg");
            //IRestResponse response = client.Execute(request);

            //OCBC_Online_Joint_Account.Models.ClientOCR clientOCR = new OCBC_Online_Joint_Account.Models.ClientOCR();


            //Dictionary <string, object> obj = (Dictionary<string, object>)OCBC_Online_Joint_Account.Models.JSONHelper.Deserialize(response.Content);
            //foreach (var item in obj.Keys)
            //{
            //    if (item == "result")
            //    {
            //        List<object> results = (List<object>)obj[item];
            //        Dictionary<string, object> predictions = (Dictionary<string, object>)results[0];
            //        List<object> prediction = (List<object>) predictions["prediction"];
            //        foreach (var p in prediction)
            //        {
            //            Dictionary<string, object> pvalue = (Dictionary<string, object>)p;
            //            var label = (string)pvalue["label"];
            //            var ocr_text = (string)pvalue["ocr_text"];
            //            //Console.WriteLine("Label: " + label);
            //            if (label == "NRIC")
            //            {
            //                clientOCR.NRIC = ocr_text;
            //            }
            //            else if (label == "Name")
            //            {
            //                clientOCR.Name = ocr_text;
            //            }
            //            else if (label == "Sex")
            //            {
            //                clientOCR.Gender = ocr_text;
            //            }
            //            else if (label == "Date_of_Birth")
            //            {
            //                clientOCR.Date_of_Birth = ocr_text;
            //            }
            //            //Console.WriteLine("Value: " + ocr_text);
            //            // return clientOCR object to Form.cshtml to parse the data from the OCR read
            //        }
            //    }
            //}

            return View("Upload", custApplication);
        }

        /**==========================
            JOINTAPPLICANT.CSHTML
        ==========================**/

        public ActionResult JointApplicant()
        {
            if (HttpContext.Session.GetString("JAC") != null)
            {       
                return RedirectToAction("Verify", "Account360");
            }

            ResetQR();
            checkJAC(HttpContext.Session.GetString("JAC"));
            HttpContext.Session.SetString("PageType", "Account360");
            ViewData["Salutation"] = Salutation;

            Console.WriteLine(applicants.FullName);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult JointApplicant(Account360ViewModel a360)
        {


            return RedirectToAction("Verify", "Account360");
        }

        /**==========================
               VERIFY.CSHTML
       ==========================**/

        public ActionResult Verify(Account360ViewModel a360)
        {
            // Check Main or Joint
            checkJAC(HttpContext.Session.GetString("JAC"));


            // a360 object to display the data in the fields
            Account360ViewModel ac360 = new Account360ViewModel();

            ac360.Salutation = a360.Salutation;
            Console.WriteLine(TempData["Object"]);

            return View(ac360);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Verifya()
        {
            return View();
        }

        /**==========================
                    METHODS
        ==========================**/
        public void checkJAC(string JAC)
        {
            if(HttpContext.Session.GetString("JAC") != null)
            {
                foreach (Application a in applicationContext.GetApplicationByJointApplicantionCode(JAC))
                {
                    foreach (Customer c in customerContext.GetCustomerByNRIC(a.CustNRIC))
                    {
                        ViewData["MainSalutation"] = c.Salutation;
                        ViewData["MainName"] = c.CustName;
                    }
                }
            }
        }

        public void ResetQR()
        {
            //QR: Reset QR settings
            var resetQR =
                "{\"qr_data\":\"ocbc_jointacc_digital_create\"," +
                "\"custNRIC\":null," +
                "\"hasScanned\":false," +
                "\"toRedirect\":false," +
                "\"continueMobile\":false," +
                "\"isJointApplicant\":false," +
                "\"selectedAccountTypeId\":2," +
                "\"selectedAccountTypeName\":\"360 Account\"," +
                "\"mainApplicantName\":null," +
                "\"mainApplicantNRIC\":null," +
                "\"jointApplicationCode\":null," +
                "\"id\":0}";

            var client = new RestClient("https://pfdocbcdb-5763.restdb.io/rest/qr-response/6191214a9402c24f00017a99");
            var request = new RestRequest(Method.PUT);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("x-apikey", "f3e68097c1a4127f4472d8730dcb3399f2d14");
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", resetQR, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
        }

        public void InsertQRForJointApplicant(int? AT, string JAC)
        {
            //QR: Reset QR settings

            string Name = "";
            string NRIC = "";

            foreach (Application a in applicationContext.GetApplicationByJointApplicantionCode(JAC))
            {
                foreach (Customer c in customerContext.GetCustomerByNRIC(a.CustNRIC))
                {
                    Name = c.CustName;
                    NRIC = a.CustNRIC;                
                }           
            }

            var resetQR =
                "{\"qr_data\":\"ocbc_jointacc_digital_create\"," +
                "\"custNRIC\":null," +
                "\"hasScanned\":false," +
                "\"toRedirect\":false," +
                "\"continueMobile\":false," +
                "\"isJointApplicant\":true," +
                "\"selectedAccountTypeId\":" + AT + "," +
                "\"selectedAccountTypeName\":\"360 Account\"," +
                "\"mainApplicantName\":\"" + Name + "\"," +
                "\"mainApplicantNRIC\":\"" + NRIC + "\"," +
                "\"jointApplicationCode\":\"" + JAC + "\"," +
                "\"id\":0}";

            var client1 = new RestClient("https://pfdocbcdb-5763.restdb.io/rest/qr-response/6191214a9402c24f00017a99");
            var request1 = new RestRequest(Method.PUT);
            request1.AddHeader("cache-control", "no-cache");
            request1.AddHeader("x-apikey", "f3e68097c1a4127f4472d8730dcb3399f2d14");
            request1.AddHeader("content-type", "application/json");
            request1.AddParameter("application/json", resetQR, ParameterType.RequestBody);
            IRestResponse response = client1.Execute(request1);       
        }

        public bool ResponseQR()
        {
            //QR: Wait for response from iBanking App
            var client = new RestClient("https://pfdocbcdb-5763.restdb.io/rest/qr-response/6191214a9402c24f00017a99");
            var request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("x-apikey", "f3e68097c1a4127f4472d8730dcb3399f2d14");
            request.AddHeader("content-type", "application/json");
            IRestResponse response = client.Execute(request);
            QR qr = JsonConvert.DeserializeObject<QR>(response.Content);

            if (qr.hasScanned == true && qr.toRedirect == true && qr.continueMobile == false)
            {
                if(qr.custNRIC != null)
                {
                    HttpContext.Session.SetString("ApplyMethod", "QR");
                    HttpContext.Session.SetString("iBankingLogin", qr.custNRIC);
                }
                return true;
            }
            return false;
        }
    }
}

