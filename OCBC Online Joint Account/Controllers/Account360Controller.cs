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
    public class Account360Controller : Controller
    {
        private SingpassDAL singpassContext = new SingpassDAL();
        private CustomerDAL customerContext = new CustomerDAL();
        private ApplicationDAL applicationContext = new ApplicationDAL();

        private List<SelectListItem> Salutation = new List<SelectListItem>();
        private List<SelectListItem> CountryOfBirth = new List<SelectListItem>();
        private List<SelectListItem> Nationality = new List<SelectListItem>();
        private List<SelectListItem> Gender = new List<SelectListItem>();
        private List<SelectListItem> MaritialStatus = new List<SelectListItem>();
        private List<SelectListItem> AnnualIncome = new List<SelectListItem>();
        private List<SelectListItem> Occupation = new List<SelectListItem>();
        private List<SelectListItem> YearsInEmployment = new List<SelectListItem>();
        private List<TaxResidency> TaxResidencyList = new List<TaxResidency>();
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
            Occupation.Add(new SelectListItem { Value = "7", Text = "Teacher" });

            //Populate Years In Employment
            YearsInEmployment.Add(new SelectListItem { Value = "< 1", Text = "< 1" });
            for (int i = 1; i <= 40; i++)
            {
                YearsInEmployment.Add(new SelectListItem { Value = Convert.ToString(i), Text = Convert.ToString(i) });
            }
            YearsInEmployment.Add(new SelectListItem { Value = "> 40", Text = "> 40" });

            //Populate Tax Residency list
            TaxResidencyList.Add(new TaxResidency
            {
                Country = "Singapore",
                Selected = false
            });
            TaxResidencyList.Add(new TaxResidency
            {
                Country = "United States",
                Selected = false
            });
            TaxResidencyList.Add(new TaxResidency
            {
                Country = "Other country(s)",
                Selected = false
            });
        }

        /**==========================
              APPLYONLINE.CSHTML
         ==========================**/

        public ActionResult ApplyOnline(int? AT, string? JAC)
        {
            HttpContext.Session.SetString("PageType", "Account360");
            HttpContext.Session.SetInt32("AccountTypeID", 2);
     
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

          /**
            //OTP API by Twilio
            var accountSid = "AC900a65cf35b142ba9d231968f7975595";
            var authToken = "900f7cf484248daa85bccb918be28908";
            TwilioClient.Init(accountSid, authToken);
            var messageOptions = new CreateMessageOptions(new PhoneNumber("+65" + mobileNum));
            messageOptions.MessagingServiceSid = "MG9dc1a6ffbac9048864eaadfda51637fc";
            messageOptions.Body = "Your OCBC OTP is " + OTP;
            var message = MessageResource.Create(messageOptions);
            Console.WriteLine(message.Body);
          **/
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
            ac360.TaxResidencyList = TaxResidencyList;
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
                Customer iBankingDetails = HttpContext.Session.GetObjectFromJson<Customer>("iBankingDetails");
                ac360.NRIC = iBankingDetails.CustNRIC;
                ac360.Salutation = iBankingDetails.Salutation;
                ac360.FullName = iBankingDetails.CustName;
                ac360.EmailAddress = iBankingDetails.Email;
                ac360.MobileNum = iBankingDetails.ContactNo;
                ac360.Gender = iBankingDetails.Gender;
                ac360.MaritialStatus = iBankingDetails.MaritialStatus;
                ac360.Address = iBankingDetails.Address;
                ac360.CountryOfBirth = iBankingDetails.CountryOfBirth;
                ac360.Nationality = iBankingDetails.Nationality;
                ac360.DateOfBirth = iBankingDetails.DateOfBirth;
                ac360.Employer = iBankingDetails.EmployerName;
                ac360.Occupation = iBankingDetails.Occupation;
                int tempIncome = Convert.ToInt32(iBankingDetails.Income);
                if (tempIncome < 30000)
                {
                    ac360.AnnualIncome = "1";
                }
                else if (tempIncome >= 30000 && tempIncome <= 49999)
                {
                    ac360.AnnualIncome = "2";
                }
                else if (tempIncome >= 50000 && tempIncome <= 99999)
                {
                    ac360.AnnualIncome = "3";
                }
                else if (tempIncome >= 100000 && tempIncome <= 149999)
                {
                    ac360.AnnualIncome = "4";
                }
                else if (tempIncome >= 150000 && tempIncome <= 199999)
                {
                    ac360.AnnualIncome = "5";
                }
                else if (tempIncome >= 200000)
                {
                    ac360.AnnualIncome = "6";
                }
                return View(ac360);
            }

            // Else if Scan run code to pull from Scan
            else if (HttpContext.Session.GetString("ApplyMethod") == "Scan")
            {
                // Get data from OCR
                Account360ViewModel OCRDetails = new Account360ViewModel();
                OCRDetails = HttpContext.Session.GetObjectFromJson<Account360ViewModel>("Scan");
                ac360.FullName = OCRDetails.FullName;
                if (OCRDetails.Gender == null)
                {
                    ac360.Gender = "";
                }
                else if (OCRDetails.Gender.Contains("F"))
                {
                    ac360.Gender = "Female";
                }
                else if (OCRDetails.Gender.Contains("M"))
                {
                    ac360.Gender = "Male";
                }
                ac360.CountryOfBirth = OCRDetails.CountryOfBirth;
                ac360.DateOfBirth = OCRDetails.DateOfBirth;
                ac360.NRIC = OCRDetails.NRIC;
                ac360.Address = OCRDetails.Address;

                return View(ac360);
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
            mainApplication.SelfEmployeed = a360.SelfEmployeed;
            mainApplication.TaxResidence = "";

            string temp = "";
            foreach (TaxResidency country in a360.TaxResidencyList)
            {
                if (country.Selected)
                {
                    temp += country.Country + ", ";
                }
            }
            temp = temp.Substring(0, temp.Length - 2);
            mainApplication.TaxResidence = temp;

            HttpContext.Session.SetObjectAsJson("ApplicantsDetails", mainApplication);

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
            HttpContext.Session.SetString("ApplyMethod", "Scan");
            ViewData["SingaporeanSelection"] = singaporean;

            string uploadedNRICFront = "";
            string uploadedNRICBack = "";
            string uploadedResidentialProof = "";

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
                    uploadedResidentialProof = String.Format("residence_proof" + fileExt);
                    string savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\applicationdocs\\", uploadedResidentialProof);
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
                    uploadedNRICFront = String.Format("nric_front" + fileExt);
                    string savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\applicationdocs\\", uploadedNRICFront);
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
                    uploadedNRICBack = String.Format("nric_back" + fileExt);
                    string savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\applicationdocs\\", uploadedNRICBack);
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
            var client = new RestClient("https://app.nanonets.com/api/v2/OCR/Model/96fa0936-a5dd-4e70-96dd-0bae04e9d8f4/LabelFile/");
            var request = new RestRequest(Method.POST);
            request.AddHeader("authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes("1xpmJsen1wvDnk5v-50NMcSF4uO5qCZp:")));
            request.AddHeader("accept", "Multipart/form-data");
            request.AddFile("file", ".\\wwwroot\\applicationdocs\\" + uploadedNRICFront);
            IRestResponse response = client.Execute(request);

            Account360ViewModel clientOCR = new Account360ViewModel();

            Dictionary<string, object> obj = (Dictionary<string, object>)OCBC_Online_Joint_Account.Models.JSONHelper.Deserialize(response.Content);
            foreach (var item in obj.Keys)
            {
                if (item == "result")
                {
                    List<object> results = (List<object>)obj[item];
                    Dictionary<string, object> predictions = (Dictionary<string, object>)results[0];
                    List<object> prediction = (List<object>)predictions["prediction"];
                    foreach (var p in prediction)
                    {
                        Dictionary<string, object> pvalue = (Dictionary<string, object>)p;
                        var label = (string)pvalue["label"];
                        var ocr_text = (string)pvalue["ocr_text"];
                        //Console.WriteLine("Label: " + label);
                        if (label == "NRIC")
                        {
                            clientOCR.NRIC = ocr_text;
                        }
                        else if (label == "Name")
                        {
                            clientOCR.FullName = ocr_text;
                        }
                        else if (label == "Sex")
                        {
                            clientOCR.Gender = ocr_text;
                        }
                        else if (label == "Date_of_Birth")
                        {
                            clientOCR.DateOfBirth = Convert.ToDateTime(ocr_text);
                        }
                        else if (label == "Country_Of_Birth")
                        {
                            clientOCR.CountryOfBirth = ocr_text;
                        }
                        //Console.WriteLine("Value: " + ocr_text);

                    }
                }
            }


            // NRIC BACK OCR API - DO NOT DELETE. COMMENTING OUT TO REDUCE API CALL USAGE.
            var client2 = new RestClient("https://app.nanonets.com/api/v2/OCR/Model/8ee8790a-92db-48d7-adf0-c9512997b60a/LabelFile/");
            var request2 = new RestRequest(Method.POST);
            request2.AddHeader("authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes("_9xwEpzEdi3gevc7Oy-SucehAswdtRFG:")));
            request2.AddHeader("accept", "Multipart/form-data");
            request2.AddFile("file", ".\\wwwroot\\applicationdocs\\" + uploadedNRICBack);
            IRestResponse response2 = client2.Execute(request2);
            
            
            Dictionary<string, object> obj2 = (Dictionary<string, object>)OCBC_Online_Joint_Account.Models.JSONHelper.Deserialize(response2.Content);

            foreach (var item in obj2.Keys)
            {
                if (item == "result")
                {
                    List<object> results = (List<object>)obj2[item];
                    Dictionary<string, object> predictions = (Dictionary<string, object>)results[0];
                    List<object> prediction = (List<object>)predictions["prediction"];

                    foreach (var p in prediction)
                    {
                        Dictionary<string, object> pvalue = (Dictionary<string, object>)p;
                        clientOCR.Address = (string)pvalue["ocr_text"];
                        break;
                    }
                    break;
                }
            }

            ////Set clientOCR object to the "Scan" string to be used in Form.cshtml to parse the data from the OCR read
            HttpContext.Session.SetObjectAsJson("Scan", clientOCR);

            return RedirectToAction("Form");
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

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult JointApplicant(Account360ViewModel a360)
        {
            Account360ViewModel ac360 = new Account360ViewModel();

            if (HttpContext.Session.GetString("ApplyMethod") == "QR" || HttpContext.Session.GetString("ApplyMethod") == "iBanking")
            {
                foreach (Customer c in customerContext.GetCustomerByNRIC(HttpContext.Session.GetString("iBankingLogin")))
                {
                    ac360.NRIC = c.CustNRIC;
                    ac360.Salutation = c.Salutation;
                    ac360.FullName = c.CustName;
                    ac360.EmailAddress = c.Email;
                    ac360.MobileNum = c.ContactNo;
                    ac360.Gender = c.Gender;
                    ac360.MaritialStatus = c.MaritialStatus;
                    ac360.Address = c.Address;
                    ac360.CountryOfBirth = c.CountryOfBirth;
                    ac360.Nationality = c.Nationality;
                    ac360.DateOfBirth = c.DateOfBirth;
                    ac360.Employer = c.EmployerName;
                    ac360.Occupation = c.Occupation;
                    ac360.AnnualIncome = c.Income;
                }

                HttpContext.Session.SetObjectAsJson("ApplicantsDetails", ac360);
            }

            ac360 = HttpContext.Session.GetObjectFromJson<Account360ViewModel>("ApplicantsDetails");
            ac360.SalutationJoint = a360.SalutationJoint;
            ac360.JointApplicantName = a360.JointApplicantName;
            ac360.Email = a360.Email;
            ac360.ContactNo = a360.ContactNo;
            HttpContext.Session.SetObjectAsJson("ApplicantsDetails", ac360);

            return RedirectToAction("Verify", "Account360");
        }

        /**==========================
               VERIFY.CSHTML
       ==========================**/

        public ActionResult Verify()
        {
            ResetQR();
            HttpContext.Session.SetString("PageType", "Account360");
            checkJAC(HttpContext.Session.GetString("JAC")); // Check Main or Joint       

            Account360ViewModel ac360 = new Account360ViewModel();
            ac360 = HttpContext.Session.GetObjectFromJson<Account360ViewModel>("ApplicantsDetails");
            if (HttpContext.Session.GetString("ApplyMethod") != "QR" && HttpContext.Session.GetString("ApplyMethod") != "iBanking")
            {
                ac360.Occupation = Occupation[(Convert.ToInt32(ac360.Occupation) - 1)].Text;
                ac360.AnnualIncome = AnnualIncome[(Convert.ToInt32(ac360.AnnualIncome) - 1)].Text;

            }
            ViewData["DateOfBirth"] = ac360.DateOfBirth.Date.ToString("d");

            HttpContext.Session.SetObjectAsJson("ApplicantsDetails", ac360);
            return View(ac360);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Verify(Account360ViewModel a360)
        {
            a360 = HttpContext.Session.GetObjectFromJson<Account360ViewModel>("ApplicantsDetails");

            // Only scan and new singpass is add, rest update
            // Convert gender to single char
            if (a360.Gender == "Male")
            {
                a360.Gender = "M";
            }
            else
            {
                a360.Gender = "F";
            }
            // if not scan || not new singpass
            if (HttpContext.Session.GetString("ApplyMethod") != "Scan" && HttpContext.Session.GetString("CustSingpass") != "newCustomer")
            {
                customerContext.Update(a360);
            }
            else
            {
                customerContext.Add(a360);
            }

            // Add application table
            Application newApplication = new Application();
            newApplication.CustNRIC = a360.NRIC;
            newApplication.AccountTypeID = (int)HttpContext.Session.GetInt32("AccountTypeID");

            // Main applicant
            if (HttpContext.Session.GetString("JAC") == null)
            {
                Random rnd = new Random();
                int rndNum1 = rnd.Next(100000000, 999999999);
                int rndNum2 = rnd.Next(100000000, 999999999);
                int rndNum3 = rnd.Next(10, 99);
                string JAC = "J" + DateTime.Today.Day + rndNum1 + rndNum2 + rndNum3 + a360.NRIC.Substring(5, 3);
                //Email API
                RunAsync(a360.Salutation, a360.FullName, a360.Email, JAC, a360.SalutationJoint, a360.JointApplicantName).Wait();

                newApplication.Status = "Pending";
                newApplication.JointApplicantID = null;
            }
            // Joint applicant
            else
            {
                newApplication.Status = "Successful";
            }

            // Create Bank Account && CustomerAccounts once status = successful.

            return RedirectToAction("Success", "Account360");
        }

        public ActionResult Success()
        {
            HttpContext.Session.SetString("PageType", "Account360");
            return View();
        }

        /**==========================
                    METHODS
        ==========================**/

        static async Task RunAsync(string sal, string name, string email, string jac, string sj, string jan)
        {
            MailjetClient client = new MailjetClient("883c10fe26db15ef52b5ff8f0a4965fb", "2f42f3a81ad1fa32fe50ebf5274be5e0");
            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            }
               .Property(Send.FromEmail, "s10208193@connect.np.edu.sg")
               .Property(Send.FromName, "OCBC Bank")
               .Property(Send.Subject, "360 Account Joint-Account Application")
               .Property(Send.TextPart, "")
               .Property(Send.HtmlPart, "<div style='text-align: center; margin: 0 20% 0 20%'><img style='height: 150px' src='https://i.ibb.co/X28mfrZ/ocbc-logo-330-160.png'><h1><b>360 Account Joint-Account Application</b></h1><hr><div style='text-align: left; font-weight: lighter;'><h3 style='font-weight: lighter; margin-top: 40px'>Dear "+ sj + " " + jan +"</h3><h3 style='font-weight: lighter; margin-top: 40px'>"+ sal + " " + name +" has initiated a Joint-Account application and is requesting you to complete it. Simply click on this <a href='https://localhost:44381/Account360/ApplyOnline?AT=2&JAC="+ jac + "'>link</a> to complete your application.</h3><h3 style='font-weight: lighter; margin-top: 40px'>If you do not know this person, call 1800 363 333 at once.</h3><h3 style='font-weight: lighter; margin-top: 40px'>Yours sincerely<br><b>OCBC Bank</b></h3></div></div>")
               .Property(Send.Recipients, new JArray { new JObject { {"Email", email} } });
            MailjetResponse response = await client.PostAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine(string.Format("Total: {0}, Count: {1}\n", response.GetTotal(), response.GetCount()));
                Console.WriteLine(response.GetData());
            }
            else
            {
                Console.WriteLine(string.Format("StatusCode: {0}\n", response.StatusCode));
                Console.WriteLine(string.Format("ErrorInfo: {0}\n", response.GetErrorInfo()));
                Console.WriteLine(string.Format("ErrorMessage: {0}\n", response.GetErrorMessage()));
                Console.ReadLine();
            }
        }

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

