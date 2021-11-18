using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCBC_Joint_Account_Application.Models
{
    public class QR
    {
        public string _id { get; set; }
        public string custNRIC { get; set; }
        public bool hasScanned { get; set; }
        public bool toRedirect { get; set; }
        public bool continueMobile { get; set; }
        public int id { get; set; }
        public string qr_data { get; set; }
        public bool isJointApplicant { get; set; }
        public string mainApplicantName { get; set; }
        public string mainApplicantNRIC { get; set; }
        public int custId { get; set; }
        public string jointApplicationCode { get; set; }
    }
}
