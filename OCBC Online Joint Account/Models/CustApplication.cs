using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCBC_Joint_Account_Application.Models
{
    public class CustApplication
    {
        public int CustID { get; set; }
        public int ApplicationID { get; set; }
        public string CustProofOfResidence { get; set; }
        public string CustNRICFront { get; set; }
        public string CustNRICBack { get; set; }
        public string CustPassport { get; set; }
        public string CustForeignPassFront { get; set; }
        public string CustForeignPassBack { get; set; }
        public string JointApplicantName { get; set; }
        public string JointApplicantNRIC { get; set; }
    }
}
