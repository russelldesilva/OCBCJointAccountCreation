using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCBC_Joint_Account_Application.Models
{
    public class Application
    {
        public int ApplicationID { get; set; }
        public string CustNRIC { get; set; }
        public int AccountTypeID { get; set; }
        public string Status { get; set; }
        public DateTime CreationDate { get; set; }
        public string JointApplicantCode { get; set; }
        public int? JointApplicantID { get; set; }
    }
}
