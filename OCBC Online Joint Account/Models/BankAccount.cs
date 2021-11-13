using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCBC_Joint_Account_Application.Models
{
    public class BankAccount
    {
        public string AccountNo{ get; set; }
        public int AccountTypeID { get; set; }
        public double Balance { get; set; }
    }
}
