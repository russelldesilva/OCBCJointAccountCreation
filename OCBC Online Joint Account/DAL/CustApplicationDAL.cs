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

        public int Add(CustApplication custApp)
        {
            SqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = @"INSERT INTO CustApplication (CustNRIC, ApplicationId, CustNRICFront, CustNRICBack, CustProofOfResidence, JointApplicantName, JointApplicantNRIC) VALUES (@CustNRIC, @ApplicationId, @CustNRICFront, @CustNRICBack, @CustProofOfResidence, @JointApplicantName, @JointApplicantNRIC)";

            cmd.Parameters.AddWithValue("@CustNRIC", custApp.CustNRIC);
            cmd.Parameters.AddWithValue("@ApplicationId", custApp.ApplicationID);
            cmd.Parameters.AddWithValue("@CustProofOfResidence", !string.IsNullOrEmpty(custApp.CustProofOfResidence) ? custApp.CustProofOfResidence : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CustNRICFront", !string.IsNullOrEmpty(custApp.CustNRICFront) ? custApp.CustNRICFront : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CustNRICBack", !string.IsNullOrEmpty(custApp.CustNRICBack) ? custApp.CustNRICBack : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CustForeignPassFront", !string.IsNullOrEmpty(custApp.CustForeignPassFront) ? custApp.CustForeignPassFront : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CustForeignPassBack", !string.IsNullOrEmpty(custApp.CustForeignPassBack) ? custApp.CustForeignPassBack : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CustPassport", !string.IsNullOrEmpty(custApp.CustPassport) ? custApp.CustPassport : (object)DBNull.Value);

            cmd.Parameters.AddWithValue("@JointApplicantName", !string.IsNullOrEmpty(custApp.JointApplicantName) ? custApp.JointApplicantName : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@JointApplicantNRIC", !string.IsNullOrEmpty(custApp.JointApplicantNRIC) ? custApp.JointApplicantNRIC : (object)DBNull.Value);

            conn.Open();

            custApp.ApplicationID = (int)cmd.ExecuteScalar();

            conn.Close();

            return custApp.ApplicationID;
        }
    }
}
