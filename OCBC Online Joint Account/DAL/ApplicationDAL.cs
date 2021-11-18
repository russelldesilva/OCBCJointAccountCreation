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
    public class ApplicationDAL
    {
        private IConfiguration Configuration { get; }
        private SqlConnection conn;

        public ApplicationDAL()
        {
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            string strConn = Configuration.GetConnectionString(
            "CJP_DBConnectionString");
            conn = new SqlConnection(strConn);
        }

        public List<Application> GetApplicationByJointApplicantionCode(string JointApplicationCode)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Application WHERE JointApplicationCode = @selectedJointApplicationCode";
            cmd.Parameters.AddWithValue("@selectedJointApplicationCode", JointApplicationCode);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            List<Application> applicationList = new List<Application>();
            while (reader.Read())
            {
                applicationList.Add(
                    new Application
                    {
                        ApplicationID = reader.GetInt32(0),
                        CustNRIC = reader.GetString(1),
                        AccountTypeID = reader.GetInt32(2),
                        Status = reader.GetString(3),
                        CreationDate = reader.GetDateTime(4),
                        JointApplicantCode = !reader.IsDBNull(5) ? reader.GetString(5) : (string)null,
                        JointApplicantID = !reader.IsDBNull(6) ? reader.GetInt32(6) : (Int32?)null
                    }
                );
            }
            reader.Close();
            conn.Close();
            return applicationList;
        }

        public int Add(Application application)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Application (CustNRIC, AccountTypeId, Status, 
                                                   CreationDate, JointApplicationCode, JointApplicationID)
                                                OUTPUT INSERTED.ApplicationID
                                                VALUES(@nric, @typeID, @status,
                                                @date, @code, @jointID)";
            cmd.Parameters.AddWithValue("@nric", application.CustNRIC);
            cmd.Parameters.AddWithValue("@typeID",  application.AccountTypeID);
            cmd.Parameters.AddWithValue("@status", application.Status);
            cmd.Parameters.AddWithValue("@date", DateTime.Now);
            if (application.JointApplicantCode == null)
            {
                cmd.Parameters.AddWithValue("@code", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@code", application.JointApplicantCode);
            }
            
            if (application.JointApplicantID == null)
            {
                cmd.Parameters.AddWithValue("@jointID", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@jointID", application.JointApplicantID);
            }
            
            conn.Open();
            application.ApplicationID = (int)cmd.ExecuteScalar();
            conn.Close();
            return application.ApplicationID;
        }

        public int Update(Application application)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"Update Application SET CustNRIC = @nric, AccountTypeId = @typeID, Status = @status, 
                                        JointApplicationCode = @code, JointApplicationID = @jointID WHERE CustNRIC = @nric";
            cmd.Parameters.AddWithValue("@nric", application.CustNRIC);
            cmd.Parameters.AddWithValue("@typeID", application.AccountTypeID);
            cmd.Parameters.AddWithValue("@status", application.Status);
            if (application.JointApplicantCode == null)
            {
                cmd.Parameters.AddWithValue("@code", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@code", application.JointApplicantCode);
            }
            cmd.Parameters.AddWithValue("@jointID", application.JointApplicantID);
            conn.Open();
            int count = cmd.ExecuteNonQuery();
            conn.Close();
            return count;
        }

        public string GetiUsername(int jointAppID)
        {
            SqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = @"SELECT iBUsername FROM Application a INNER JOIN Customer c ON a.CustNRIC = c.CustNRIC WHERE JointApplicationID = @selectedJointApplicationID";
            cmd.Parameters.AddWithValue("@selectedJointApplicationID", jointAppID);

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            reader.Read();

            string iBusername = !reader.IsDBNull(0) ? reader.GetString(0) : null;

            reader.Close();

            conn.Close();
            
            return iBusername;
        }

        public int GetApplicationIDByNRIC(string NRIC)
        {
            SqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = @"SELECT ApplicationID FROM Application WHERE CustNRIC = @selectedNRIC";
            cmd.Parameters.AddWithValue("@selectedNRIC", NRIC);

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            reader.Read();

            int appID = reader.GetInt32(0);
            reader.Close();
            conn.Close();
            return appID;
        }
    }
}
