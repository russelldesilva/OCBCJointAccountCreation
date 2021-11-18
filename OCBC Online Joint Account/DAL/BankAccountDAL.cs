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
    public class BankAccountDAL
    {
        private IConfiguration Configuration { get; }
        private SqlConnection conn;

        public BankAccountDAL()
        {
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            string strConn = Configuration.GetConnectionString(
            "CJP_DBConnectionString");
            conn = new SqlConnection(strConn);
        }

        public string Add(BankAccount bankAccount)
        {
            SqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = @"INSERT INTO BankAccount (AccountNo, AccountTypeID, Balance) VALUES (@AccountNo, @AccountTypeID, @Balance)";

            cmd.Parameters.AddWithValue("@AccountNo", bankAccount.AccountNo);
            cmd.Parameters.AddWithValue("@AccountTypeID", bankAccount.AccountTypeID);
            cmd.Parameters.AddWithValue("@Balance", bankAccount.Balance);
            conn.Open();
            bankAccount.AccountNo = (string)cmd.ExecuteScalar();
            conn.Close();
            return bankAccount.AccountNo;
        }
    }
}
