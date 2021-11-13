using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace OCBC_Joint_Account_Application.Models
{
    public class Account360ViewModel
    {
        public int OTP { get; set; } 
        public string Salutation { get; set; }
        public string FullName { get; set; }
        public string NRIC { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-YYYY}")]
        public DateTime DateOfBirth { get; set; }
        public string CountryOfBirth { get; set; }
        public string Nationality { get; set; }
        public string Gender { get; set; }
        public string MaritialStatus { get; set; }
        public string MobileNum { get; set; }
        public string EmailAddress { get; set; }
        public string Address { get; set; }
        public string Employer { get; set; }
        public int YearsInEmployment { get; set; }
        public string Occupation { get; set; }
        public string AnnualIncome { get; set; }

       
    }
}
