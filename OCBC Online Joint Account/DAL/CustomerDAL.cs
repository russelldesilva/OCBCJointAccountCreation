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
    public class CustomerDAL
    {
        private IConfiguration Configuration { get; }
        private SqlConnection conn;

        public CustomerDAL()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            string strConn = Configuration.GetConnectionString("CJP_DBConnectionString");
            conn = new SqlConnection(strConn);
        }

        public Customer GetCustomerByiBUsername(string iBUsername)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Customer WHERE iBUsername = @selectedUsername";
            cmd.Parameters.AddWithValue("@selectedUsername", iBUsername);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            Customer cust = new Customer()
            {
                CustNRIC = reader.GetString(0),
                CustName = reader.GetString(1),
                Email = reader.GetString(2),
                ContactNo = reader.GetString(3),
                Gender = reader.GetString(4),
                MaritialStatus = reader.GetString(5),
                iBUsername = iBUsername,
                iBPin = reader.GetString(7),
            };
            reader.Close();
            conn.Close();
            return cust;
        }

        public List<Customer> GetCustomerByNRIC(string CustNRIC)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Customer WHERE CustNRIC = @selectedNRIC";
            cmd.Parameters.AddWithValue("@selectedNRIC", CustNRIC);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            List<Customer> custList = new List<Customer>();
            while (reader.Read())
            {
                custList.Add(
                    new Customer
                    {
                        CustNRIC = reader.GetString(0),
                        Salutation = reader.GetString(1),
                        CustName = reader.GetString(2),
                        Email = reader.GetString(3),
                        ContactNo = reader.GetString(4),
                        Gender = reader.GetString(5),
                        MaritialStatus = reader.GetString(6),
                        iBUsername = reader.GetString(7),
                        iBPin = reader.GetString(8),
                        Address = reader.GetString(9),
                        CountryOfBirth = reader.GetString(10),
                        Nationality = reader.GetString(11),
                        DateOfBirth = reader.GetDateTime(12),
                        EmployerName = reader.GetString(13),
                        Occupation = reader.GetString(14),
                        Income = reader.GetString(15),
                    }
                );
            }

            reader.Close();
            conn.Close();
            return custList;
        }


    }
}
