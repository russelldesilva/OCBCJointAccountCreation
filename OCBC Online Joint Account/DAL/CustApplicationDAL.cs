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
    public class CustApplicationDAL
    {
        private IConfiguration Configuration { get; }
        private SqlConnection conn;

        public CustApplicationDAL()
        {
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            string strConn = Configuration.GetConnectionString(
            "CJP_DBConnectionString");
            conn = new SqlConnection(strConn);
        }
        // INSERT INTO CustApplication (CustNRIC, ApplicationId, CustNRICFront, CustNRICBack, CustProofOfResidence, CustPassport, CustForeignPassFront, CustForeignPassBack, JointApplicantName, JointApplicantNRIC) VALUES

        public int Add(Account360ViewModel a360)
        {
            SqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = @"INSERT INTO CustApplication (CustNRIC, ApplicationId, CustNRICFront, CustNRICBack, CustProofOfResidence, CustPassport, CustForeignPassFront, CustForeignPassBack, JointApplicantName, JointApplicantNRIC) VALUES
                VALUES(@CustNRIC, @ApplicationId, @CustNRICFront, @CustNRICBack, @CustProofOfResidence, @CustPassport, @CustForeignPassFront, @CustForeignPassBack, @JointApplicantName, @JointApplicantNRIC)";

            cmd.Parameters.AddWithValue("@CustNRIC", a360.NRIC);
            cmd.Parameters.AddWithValue("@ApplicationId", a360.Salutation);
            cmd.Parameters.AddWithValue("@CustNRICFront", null);
            cmd.Parameters.AddWithValue("@CustNRICBack", null);
            cmd.Parameters.AddWithValue("@CustProofOfResidence", null);
            cmd.Parameters.AddWithValue("@CustPassport", null);
            cmd.Parameters.AddWithValue("@CustForeignPassFront", null);
            cmd.Parameters.AddWithValue("@CustForeignPassBack", null);
            cmd.Parameters.AddWithValue("@JointApplicantName", null);
            cmd.Parameters.AddWithValue("@JointApplicantNRIC", null);

            conn.Open();

            //a360.NRIC = (string)cmd.ExecuteScalar();

            conn.Close();

            return 1;
        }
    }
}
