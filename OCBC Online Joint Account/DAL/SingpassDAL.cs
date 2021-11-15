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
    public class SingpassDAL
    {
        private IConfiguration Configuration { get; }
        private SqlConnection conn;

        public SingpassDAL()
        {
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            string strConn = Configuration.GetConnectionString("CJP_DBConnectionString");
            conn = new SqlConnection(strConn);
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
