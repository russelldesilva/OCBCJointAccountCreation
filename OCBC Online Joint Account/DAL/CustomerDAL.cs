using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Data.SqlClient;
using OCBC_Joint_Account_Application.Models;

namespace OCBC_Joint_Account_Application.DAL
{
    public class CustomerDAL
    {
        private IConfiguration Configuration { get; }
        private SqlConnection conn;

        public CustomerDAL()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            string strConn = Configuration.GetConnectionString("CJP_DBConnectionString");
            conn = new SqlConnection(strConn);
        }
        public Customer GetCustomerByiBUsername(string iBUsername)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Customer WHERE iBUsername = @selectedUsername";
            cmd.Parameters.AddWithValue("@selectedUsername", iBUsername);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            Customer cust = new Customer()
            {
                CustNRIC = reader.GetString(0),
                CustName = reader.GetString(1),
                Email = reader.GetString(2),
                ContactNo = reader.GetString(3),
                Gender = reader.GetString(4),
                MaritialStatus = reader.GetString(5),
                iBUsername = iBUsername,
                iBPin = reader.GetString(7),
            };
            reader.Close();
            conn.Close();
            return cust;
        }
    }
}
