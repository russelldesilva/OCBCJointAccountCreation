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
                                                   CreationDate, JointApplicationCode)
                                                OUTPUT INSERTED.ApplicationID
                                                VALUES(@nric, @typeID, @status,
                                                @date, @code)";
            cmd.Parameters.AddWithValue("@nric", application.CustNRIC);
            cmd.Parameters.AddWithValue("@typeID",  application.AccountTypeID);
            cmd.Parameters.AddWithValue("@status", application.Status);
            cmd.Parameters.AddWithValue("@date", application.CreationDate);
            cmd.Parameters.AddWithValue("@code", application.JointApplicantCode);
            conn.Open();
            application.ApplicationID = (int)cmd.ExecuteScalar();
            conn.Close();
            return application.ApplicationID;
        }
    }
}
