using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace OCBC_Joint_Account_Application.Models
{
    public class CustApplication
    {
        public string CustNRIC { get; set; }
        public int ApplicationID { get; set; }
        public string? CustProofOfResidence { get; set; }
        public string? CustNRICFront { get; set; }
        public string? CustNRICBack { get; set; }
        public string? CustPassport { get; set; }
        public string? CustForeignPassFront { get; set; }
        public string? CustForeignPassBack { get; set; }
        public string JointApplicantName { get; set; }
        public string JointApplicantNRIC { get; set; }
        public string Singaporean { get; set; }
        public IFormFile CustProofOfResidenceUpload { get; set; }
        public IFormFile CustNRICFrontUpload { get; set; }
        public IFormFile CustNRICBackUpload { get; set; }
        public IFormFile CustPassportUpload { get; set; }
        public IFormFile CustForeignPassFrontUpload { get; set; }
        public IFormFile CustForeignPassBackUpload { get; set; }
    }
}
