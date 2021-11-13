using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCBC_Joint_Account_Application.Models
{
    public class AccountType
    {
        public int AccountTypeID { get; set; }
        public int AccountCatID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public double Interest { get; set; }
        public string Description { get; set; }
    }
}
