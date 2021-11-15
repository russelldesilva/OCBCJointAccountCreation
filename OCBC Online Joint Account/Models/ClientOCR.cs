using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCBC_Online_Joint_Account.Models
{
    public class ClientOCR
    {
        public string NRIC { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public string Date_of_Birth { get; set; }

        public string Country_of_Birth { get; set; }
    }
}
