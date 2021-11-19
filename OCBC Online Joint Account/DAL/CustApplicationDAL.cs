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

        public string Add(CustApplication custApp)
        {
            SqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = @"INSERT INTO CustApplication (CustNRIC, ApplicationId, CustNRICFront, CustNRICBack, CustProofOfResidence, CustPassport, CustForeignPassFront, CustForeignPassBack, JointApplicantName, JointApplicantNRIC) VALUES (@CustNRIC, @ApplicationId, @CustNRICFront, @CustNRICBack, @CustProofOfResidence, @CustPassport, @CustForeignPassFront, @CustForeignPassBack, @JointApplicantName, @JointApplicantNRIC)";
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
            custApp.CustNRIC = (string)cmd.ExecuteScalar();
            conn.Close();
            return custApp.CustNRIC;
        }

        public List<CustApplication> GetCustApplicationByNRIC(string NRIC)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM CustApplication WHERE CustNRIC = @selectedNRIC";
            cmd.Parameters.AddWithValue("@selectedNRIC", NRIC);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            List<CustApplication> custApplicationsList = new List<CustApplication>();

            while (reader.Read())
            {
                custApplicationsList.Add(
                    new CustApplication
                    {
                        CustNRIC = reader.GetString(0),
                        ApplicationID = reader.GetInt32(1),
                        CustProofOfResidence = !reader.IsDBNull(2) ? reader.GetString(2) : (string)null,
                        CustNRICFront = !reader.IsDBNull(3) ? reader.GetString(3) : (string)null,
                        CustNRICBack = !reader.IsDBNull(4) ? reader.GetString(4) : (string)null,
                        CustPassport = !reader.IsDBNull(5) ? reader.GetString(5) : (string)null,
                        CustForeignPassFront = !reader.IsDBNull(6) ? reader.GetString(6) : (string)null,
                        CustForeignPassBack = !reader.IsDBNull(7) ? reader.GetString(7) : (string)null,
                        JointApplicantName = !reader.IsDBNull(8) ? reader.GetString(8) : (string)null,
                        JointApplicantNRIC = !reader.IsDBNull(9) ? reader.GetString(9) : (string)null,
                    }
                );
            }
            reader.Close();
            conn.Close();
            return custApplicationsList;
        }

        public List<Singpass> GetSingpassByNRIC(string NRIC)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Singpass WHERE NRIC = @selectedNRIC";
            cmd.Parameters.AddWithValue("@selectedNRIC", NRIC);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            List<Singpass> singpassList = new List<Singpass>();
            while (reader.Read())
            {
                singpassList.Add(
                    new Singpass
                    {
                        NRIC = reader.GetString(0),
                        Name = reader.GetString(1),
                        AliasName = !reader.IsDBNull(2) ? reader.GetString(2) : (string)null,
                        HanyuPinYinName = !reader.IsDBNull(3) ? reader.GetString(3) : (string)null,
                        HanyuPinYinAliasName = !reader.IsDBNull(4) ? reader.GetString(4) : (string)null,
                        MarriedName = !reader.IsDBNull(5) ? reader.GetString(5) : (string)null,
                        DoB = reader.GetDateTime(6),
                        Gender = reader.GetString(7),
                        Race = reader.GetString(8),
                        Nationality = reader.GetString(9),
                        CountryOfBirth = reader.GetString(10),
                        EmployerName = reader.GetString(11),
                        MobileNum = reader.GetString(12),
                        Email = reader.GetString(13),
                        RegisteredAddress = reader.GetString(14)
                    }
                );
            }
            reader.Close();
            conn.Close();
            return singpassList;
        }
    }
}
