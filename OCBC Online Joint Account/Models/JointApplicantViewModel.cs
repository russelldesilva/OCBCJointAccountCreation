using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCBC_Joint_Account_Application.Models
{
    public class JointApplicantViewModel
    {
        public string Salutation { get; set; }
        public string JointApplicantName { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public string MainApplicantName { get; set; }
        public string JointApplicantCode { get; set; }
        public int JointApplicantID { get; set; }
    }
}
