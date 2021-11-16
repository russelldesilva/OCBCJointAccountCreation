using Microsoft.Extensions.Configuration;
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

        public int Add()
        {
            SqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = @"INSERT INTO CustApplication (CustNRIC, ApplicationId, CustNRICFront, CustNRICBack, CustProofOfResidence, CustPassport, CustForeignPassFront, CustForeignPassBack, JointApplicantName, JointApplicantNRIC) VALUES
                VALUES(@CustNRIC, @ApplicationId, @CustNRICFront, @CustNRICBack, @CustProofOfResidence, @CustPassport, @CustForeignPassFront, @CustForeignPassBack, @JointApplicantName, @JointApplicantNRIC)";
            /*
            cmd.Parameters.AddWithValue("@CustNRIC", a360.NRIC);
            cmd.Parameters.AddWithValue("@Salutation", a360.Salutation);
            cmd.Parameters.AddWithValue("@CustName", a360.FullName);
            cmd.Parameters.AddWithValue("@DateOfBirth", a360.DateOfBirth);
            cmd.Parameters.AddWithValue("@CountryOfBirth", a360.CountryOfBirth);
            cmd.Parameters.AddWithValue("@Nationality", a360.Nationality);
            cmd.Parameters.AddWithValue("@Gender", a360.Gender);
            cmd.Parameters.AddWithValue("@MaritalStatus", a360.MaritialStatus);
            cmd.Parameters.AddWithValue("@Address", a360.Address);
            cmd.Parameters.AddWithValue("@Email", a360.EmailAddress);
            cmd.Parameters.AddWithValue("@ContactNo", a360.MobileNum);
            cmd.Parameters.AddWithValue("@Occupation", a360.Occupation);
            cmd.Parameters.AddWithValue("@EmployerName", a360.Employer);
            cmd.Parameters.AddWithValue("@Income", a360.AnnualIncome);
            cmd.Parameters.AddWithValue("@iBUsername", null);
            cmd.Parameters.AddWithValue("@iBPin", null);
            */
            conn.Open();

            //a360.NRIC = (string)cmd.ExecuteScalar();

            conn.Close();

            return 1;
        }
    }
}
