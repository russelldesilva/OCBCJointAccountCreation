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
        public int Add(Application application)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify an INSERT SQL statement which will
            //return the auto-generated StaffID after insertion
            cmd.CommandText = @"INSERT INTO Application (CustNRIC, AccountTypeId, Status, 
                                                   CreationDate, JointApplicationCode)
                                                OUTPUT INSERTED.ApplicationID
                                                VALUES(@nric, @typeID, @status,
                                                @date, @code)";
            //Define the parameters used in SQL statement, value for each parameter
            //is retrieved from respective class's property.
            cmd.Parameters.AddWithValue("@nric", application.CustNRIC);
            cmd.Parameters.AddWithValue("@typeID",  application.AccountTypeID);
            cmd.Parameters.AddWithValue("@status", application.Status);
            cmd.Parameters.AddWithValue("@date", application.CreationDate);
            cmd.Parameters.AddWithValue("@code", application.JointApplicantCode);
            //A connection to database must be opened before any operations made.
            conn.Open();
            //ExecuteScalar is used to retrieve the auto-generated
            //StaffID after executing the INSERT SQL statement
            application.ApplicationID = (int)cmd.ExecuteScalar();
            //A connection should be closed after operations.
            conn.Close();
            //Return id when no error occurs.
            return application.ApplicationID;
        }
    }
}
