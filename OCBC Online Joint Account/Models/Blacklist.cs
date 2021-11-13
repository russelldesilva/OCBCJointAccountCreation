using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCBC_Joint_Account_Application.Models
{
    public class Blacklist
    {
        public int BlacklistID { get; set; }
        public string Name { get; set; }
        public string Reason { get; set; }
    }
}
