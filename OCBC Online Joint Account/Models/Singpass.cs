using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace OCBC_Joint_Account_Application.Models
{
    public class Singpass
    {
        public string NRIC { get; set; }
        public string Name { get; set; }
        public string AliasName { get; set; }
        public string HanyuPinYinName { get; set; }
        public string HanyuPinYinAliasName { get; set; }
        public string MarriedName { get; set; }
        public DateTime DoB { get; set; }
        public string Gender { get; set; }
        public string Race { get; set; }
        public string Nationality { get; set; }
        public string CountryOfBirth { get; set; }
        public string EmployerName { get; set; }
        public string MobileNum { get; set; }
        public string Email { get; set; }
        public string RegisteredAddress { get; set; }
    }
}
