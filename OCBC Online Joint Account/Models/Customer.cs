using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;


namespace OCBC_Joint_Account_Application.Models
{
    public class Customer
    {
        public string CustNRIC { get; set; }
        public string Salutation { get; set; }
        public string CustName { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public string Gender { get; set; }
        public string MaritialStatus { get; set; }
        public string iBUsername { get; set; }
        public string iBPin { get; set; }
        public string Address { get; set; }
        public string CountryOfBirth { get; set; }
        public string Nationality { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string EmployerName { get; set; }
        public string Occupation { get; set; }
        public string Income { get; set; }
    }
}
