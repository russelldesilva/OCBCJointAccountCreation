using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCBC_Joint_Account_Application.Models
{
    public class BankTransaction
    {
        public int TransctionID { get; set; }
        public DateTime Date { get; set; }
        public int TotalAmount { get; set; }
        public string Type { get; set; }
        public string AccountNo { get; set; }
    }
}
