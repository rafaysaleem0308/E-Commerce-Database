using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

using System;
using System.Windows.Forms;
using System.Windows.Markup;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Data;

namespace WindowsFormsApp1
{

    public static class Sessional
    {
        public static int UserId { get; set; }
        public static string Username { get; set; }
    }


    public class Connection
    {
        // Corrected connection string
        public static string connect = "User Id=PROJECT;Password=Rafey3420;Data Source=localhost:1521/xe;";


        public void WriteData_of_users(string username, string password, string email, string role, int userID, DateTime created_at)
        {
            using (OracleConnection con = new OracleConnection(connect))
            {
                try
                {
                    con.Open();

                    // Check if the username already exists
                    using (OracleCommand checkCmd = new OracleCommand("SELECT COUNT(*) FROM users WHERE USERNAME = :USERNAME", con))
                    {
                        checkCmd.Parameters.Add(new OracleParameter("USERNAME", OracleDbType.Varchar2)).Value = username;

                        int userCount = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (userCount > 0)
                        {
                            // Username already exists
                            MessageBox.Show("This username already exists. Please choose a different username.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    
                   
                    // Insert new user data into the database
                    using (OracleCommand cmd = new OracleCommand("INSERT INTO users(USER_ID, USERNAME, PASSWORD, EMAIL, ROLE, CREATED_AT) VALUES(:USER_ID, :USERNAME, :PASSWORD, :EMAIL, :ROLE, :CREATED_AT)", con))
                    {
                        cmd.Parameters.Add(new OracleParameter("USER_ID", OracleDbType.Int32)).Value = userID;
                        cmd.Parameters.Add(new OracleParameter("USERNAME", OracleDbType.Varchar2)).Value = username;
                        cmd.Parameters.Add(new OracleParameter("PASSWORD", OracleDbType.Varchar2)).Value = password;
                        cmd.Parameters.Add(new OracleParameter("EMAIL", OracleDbType.Varchar2)).Value = email;
                        cmd.Parameters.Add(new OracleParameter("ROLE", OracleDbType.Varchar2)).Value = role;
                        cmd.Parameters.Add(new OracleParameter("CREATED_AT", OracleDbType.Date)).Value = created_at;

                        // Execute the query
                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            MessageBox.Show("User registered successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Failed to register user. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (OracleException ex)
                {
                    MessageBox.Show($"Database Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //public List<(string Username, string Password, string Role, int UserId)> ReadLoginData(string username, string password, string role)
        //{
        //    var loginData = new List<(string Username, string Password, string Role, int UserId)>();

        //    using (OracleConnection con = new OracleConnection(connect))
        //    {
        //        try
        //        {
        //            con.Open();

        //            // Use a parameterized query to fetch data, including user_id
        //            using (OracleCommand cmd = new OracleCommand(
        //                "SELECT USERNAME, PASSWORD, ROLE, USER_ID FROM users WHERE USERNAME = :username AND PASSWORD = :password AND ROLE = :role", con))
        //            {
        //                cmd.Parameters.Add(new OracleParameter("username", OracleDbType.Varchar2)).Value = username;
        //                cmd.Parameters.Add(new OracleParameter("password", OracleDbType.Varchar2)).Value = password;
        //                cmd.Parameters.Add(new OracleParameter("role", OracleDbType.Varchar2)).Value = role;

        //                using (OracleDataReader reader = cmd.ExecuteReader())
        //                {
        //                    while (reader.Read())
        //                    {
        //                        loginData.Add((
        //                            reader["USERNAME"].ToString(),
        //                            reader["PASSWORD"].ToString(),
        //                            reader["ROLE"].ToString(),
        //                            Convert.ToInt32(reader["USER_ID"])
        //                        ));

        //                    }
        //                }
        //            }
        //        }
        //        catch (OracleException ex)
        //        {
        //            MessageBox.Show($"Database Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        }
        //    }

        //    return loginData;
        //} 
        public List<(string Username, string Password, string Role, int UserId)> ReadLoginData( string username, string password, string role)
        {
            var loginData = new List<(string Username, string Password, string Role, int UserId)>();

            using (OracleConnection con = new OracleConnection(connect))
            {
                try
                {
                    con.Open();

                    // Use a parameterized query to fetch data, including user_id
                    using (OracleCommand cmd = new OracleCommand(
                        "SELECT username, password, role, user_id FROM users WHERE username = :username AND password = :password AND role = :role", con))
                    {
                        cmd.Parameters.Add(new OracleParameter("username", OracleDbType.Varchar2)).Value = username;
                        cmd.Parameters.Add(new OracleParameter("password", OracleDbType.Varchar2)).Value = password;
                        cmd.Parameters.Add(new OracleParameter("role", OracleDbType.Varchar2)).Value = role;

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var userRecord = (
                                    Username: reader["username"].ToString(),
                                    Password: reader["password"].ToString(),
                                    Role: reader["role"].ToString(),
                                    UserId: Convert.ToInt32(reader["user_id"])
                                );

                                loginData.Add(userRecord);

                                // Store in sessional variables
                                Sessional.UserId = userRecord.UserId;
                                Sessional.Username = userRecord.Username;
                            }
                        }
                    }
                }
                catch (OracleException ex)
                {
                    MessageBox.Show($"Database Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return loginData;
        }




        public DataTable SearchProducts(string productName, string categoryName)
        {
            DataTable dataTable = new DataTable();

            using (OracleConnection con = new OracleConnection(connect))
            {
                try
                {
                    con.Open();

                    // SQL Query to fetch products by name and category
                    string query = @"
                SELECT p.product_id, p.name AS PRODUCT_NAME, p.price, p.stock_quantity, c.name AS CATEGORY_NAME
                FROM products p
                INNER JOIN categories c ON p.category_id = c.category_id
                WHERE LOWER(p.name) LIKE :productName 
                AND LOWER(c.name) = :categoryName";

                    using (OracleCommand cmd = new OracleCommand(query, con))
                    {
                        // Add parameters to avoid SQL injection
                        cmd.Parameters.Add(new OracleParameter("productName", OracleDbType.Varchar2)).Value = $"%{productName.ToLower()}%";
                        cmd.Parameters.Add(new OracleParameter("categoryName", OracleDbType.Varchar2)).Value = categoryName.ToLower();

                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            // Fill the DataTable with the search results
                            adapter.Fill(dataTable);
                        }
                    }
                }
                catch (OracleException ex)
                {
                    MessageBox.Show($"Database Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return dataTable;
        }



    }



    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}