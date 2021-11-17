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
                Income = reader.GetString(15)
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
                        EmployerName = reader.GetString(14),
                        Occupation = reader.GetString(15),
                        Income = reader.GetString(16)
                    }
                );
            }

            reader.Close();
            conn.Close();
            return custList;
        }

        // For new customers
        public string Add(Account360ViewModel a360)
        {
            SqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = @"INSERT INTO Customer (CustNRIC, Salutation, CustName, DateOfBirth, CountryOfBirth, Nationality, Gender, MaritalStatus, Address, Email, ContactNo, Occupation, EmployerName, Income, iBUsername, iBPin)  
                VALUES(@CustNRIC, @Salutation, @CustName, @DateOfBirth, @CountryOfBirth, @Nationality, @Gender, @MaritalStatus, @Address, @Email, @ContactNo, @Occupation, @EmployerName, @Income, @iBUsername, @iBPin)";

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

            conn.Open();

            a360.NRIC = (string)cmd.ExecuteScalar();

            conn.Close();

            return a360.NRIC;
        }

        // For existing customers

        public string Update(Account360ViewModel a360)
        {
            SqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = @"UPDATE Staff SET CustNRIC = @CustNRIC, Salutation = @Salutation, FullName = @FullName, DateOfBirth = @DateOfBirth, CountryOfBirth = @CountryOfBirth, Nationality = @Nationality, 
                Gender = @Gender, MaritialStatus = @MaritialStatus, Address = @Address, EmailAddress = @EmailAddress, ContactNo = @ContactNo, Occupation = @Occupation, EmployerName = @EmployerName, Income = @Income,
                iBUsername = @iBUsername, iBPin = @iBPin";

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

            conn.Open();

            conn.Close();

            return a360.NRIC;
        }
    }
}
