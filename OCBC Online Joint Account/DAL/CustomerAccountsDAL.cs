using Microsoft.Extensions.Configuration;
using OCBC_Joint_Account_Application.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OCBC_Online_Joint_Account.DAL
{
    public class CustomerAccountsDAL
    {
        private IConfiguration Configuration { get; }
        private SqlConnection conn;

        public CustomerAccountsDAL()
        {
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            string strConn = Configuration.GetConnectionString(
            "CJP_DBConnectionString");
            conn = new SqlConnection(strConn);
        }

        public string Add(CustomerAccounts customerAccounts)
        {
            SqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = @"INSERT INTO CustomerAccounts (CustNRIC, AccountNo) VALUES (@CustNRIC, @AccountNo )";

            cmd.Parameters.AddWithValue("@AccountNo", customerAccounts.AccountNo);
            cmd.Parameters.AddWithValue("@CustNRIC", customerAccounts.CustNRIC);
            conn.Open();
            customerAccounts.CustNRIC = (string)cmd.ExecuteScalar();
            conn.Close();
            return customerAccounts.CustNRIC;
        }
    }
}
