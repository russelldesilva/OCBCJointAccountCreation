using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCBC_Joint_Account_Application.Models
{
    public class Country
    {
        public int num_code { get; set; }
        public string alpha_2_code { get; set; }
        public string alpha_3_code { get; set; }
        public string en_short_name { get; set; }
        public string nationality { get; set; }
    }
}
