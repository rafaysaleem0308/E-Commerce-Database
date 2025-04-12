using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Login_Page : Form
    {
        public Login_Page()
        {
            InitializeComponent();

            // Add hover effects for buttons
            button1.MouseEnter += Button_MouseEnter;
            button1.MouseLeave += Button_MouseLeave;
            button2.MouseEnter += Button2_MouseEnter;
            button2.MouseLeave += Button2_MouseLeave;
        }

        private void Login_Page_Load(object sender, EventArgs e)
        {
            // Optional: Set initial appearance for ComboBox
            comboBox1.BackColor = Color.White;
            comboBox1.ForeColor = Color.Black;
            comboBox1.Font = new Font("Arial", 12, FontStyle.Regular);

            // Set custom font for password textboxes
            textBox2.Font = new Font(textBox2.Font.FontFamily, 10, FontStyle.Bold);
            textBox2.PasswordChar = '*';
        }

        // Button hover effects
        private void Button_MouseEnter(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                btn.BackColor = Color.Pink;
                btn.ForeColor = Color.White;
            }
        }

        private void Button_MouseLeave(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                btn.BackColor = SystemColors.Control;
                btn.ForeColor = SystemColors.ControlText;
            }
        }

        private void Button2_MouseEnter(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                btn.BackColor = Color.Pink;
                btn.ForeColor = Color.White;
            }
        }

        private void Button2_MouseLeave(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                btn.BackColor = SystemColors.Control;
                btn.ForeColor = SystemColors.ControlText;
            }
        }

        // Method to create a new cart for a user



        // Button click event to log in
        private void button1_Click(object sender, EventArgs e)
        {
            // Retrieve username, password, and role
            string username = textBox1.Text.Trim();
            string password = textBox2.Text.Trim();
            string role = comboBox1.SelectedItem?.ToString().ToLower();

            // Validation: Ensure both username and password are provided
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter both username and password.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Create an instance of Connection and fetch login data
            Connection cn = new Connection();
            var loginData = cn.ReadLoginData(username, password, role);

            // Check if login data is returned
            if (loginData.Count > 0)
            {
                // Get the first matched user data
                var user = loginData.FirstOrDefault();

                // Create a new cart for the user
                CreateCartForUser(user.UserId);

                // Redirect based on role
                if (user.Role.Equals("customer", StringComparison.OrdinalIgnoreCase))
                {
                    Customer_Dashboard dashboard = new Customer_Dashboard();
                    dashboard.Show();
                    this.Hide();
                }
                else if (user.Role.Equals("manager", StringComparison.OrdinalIgnoreCase))
                {
                    Manager adminDashboard = new Manager();
                    adminDashboard.Show();
                    this.Hide();
                }
                else if (user.Role.Equals("delivery", StringComparison.OrdinalIgnoreCase))
                {
                    Delievery deliveryDashboard = new Delievery();
                    deliveryDashboard.Show();
                    this.Hide();
                }
                else if (user.Role.Equals("cashier", StringComparison.OrdinalIgnoreCase))
                {
                    Cashier cashierDashboard = new Cashier();
                    cashierDashboard.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Your role is not recognized. Access denied.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //private void CreateCartForUser(int userId)
        //{
        //    using (OracleConnection con = new OracleConnection(Connection.connect))
        //    {
        //        try
        //        {
        //            con.Open();

        //            // Validate the user_id
        //            string validateUserQuery = "SELECT COUNT(*) FROM users WHERE user_id = :userId";
        //            using (OracleCommand validateCmd = new OracleCommand(validateUserQuery, con))
        //            {
        //                validateCmd.Parameters.Add(new OracleParameter("userId", OracleDbType.Int32)).Value = userId;
        //                int userExists = Convert.ToInt32(validateCmd.ExecuteScalar());
        //                if (userExists == 0)
        //                {
        //                    MessageBox.Show($"Invalid user_id: {userId}. Cannot create cart.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //                    return;
        //                }
        //            }

        //            // Check if the user already has a cart
        //            string checkCartQuery = "SELECT COUNT(*) FROM cart WHERE user_id = :userId";
        //            using (OracleCommand checkCartCmd = new OracleCommand(checkCartQuery, con))
        //            {
        //                checkCartCmd.Parameters.Add(new OracleParameter("userId", OracleDbType.Int32)).Value = userId;
        //                int cartExists = Convert.ToInt32(checkCartCmd.ExecuteScalar());
        //                if (cartExists > 0)
        //                {
        //                    MessageBox.Show($"CartID already exists for user_id {userId}.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //                    return;
        //                }
        //            }

        //            // Insert the new cart entry
        //            string insertCartQuery = @"
        //        INSERT INTO cart (cart_id, user_id)
        //        VALUES (cart_seq.nextval, :userId)";

        //            using (OracleCommand cmd = new OracleCommand(insertCartQuery, con))
        //            {
        //                cmd.Parameters.Add(new OracleParameter("userId", OracleDbType.Int32)).Value = userId;
        //                cmd.ExecuteNonQuery();
        //                MessageBox.Show($"Cart successfully created for user_id {userId}.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        //} 
        private void CreateCartForUser(int userId)
        {
            using (OracleConnection con = new OracleConnection(Connection.connect))
            {
                try
                {
                    con.Open();

                    // Validate the user_id and ensure the user is a customer
                    string validateUserQuery = "SELECT role FROM users WHERE user_id = :userId";
                    using (OracleCommand validateCmd = new OracleCommand(validateUserQuery, con))
                    {
                        validateCmd.Parameters.Add(new OracleParameter("userId", OracleDbType.Int32)).Value = userId;
                        string role = validateCmd.ExecuteScalar()?.ToString();

                        if (string.IsNullOrEmpty(role))
                        {
                            MessageBox.Show($"Invalid user_id: {userId}. Cannot create cart.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        if (!role.Equals("customer", StringComparison.OrdinalIgnoreCase))
                        {
                            MessageBox.Show($"Cart creation is only allowed for customers. User_id {userId} has role '{role}'.",
                                            "Access Denied",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    // Check if the user already has a cart
                    string checkCartQuery = "SELECT COUNT(*) FROM cart WHERE user_id = :userId";
                    using (OracleCommand checkCartCmd = new OracleCommand(checkCartQuery, con))
                    {
                        checkCartCmd.Parameters.Add(new OracleParameter("userId", OracleDbType.Int32)).Value = userId;
                        int cartExists = Convert.ToInt32(checkCartCmd.ExecuteScalar());
                        if (cartExists > 0)
                        {
                            MessageBox.Show($"Cart already exists for user_id {userId}.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }

                    // Insert the new cart entry
                    string insertCartQuery = @"
                INSERT INTO cart (cart_id, user_id)
                VALUES (cart_seq.nextval, :userId)";

                    using (OracleCommand cmd = new OracleCommand(insertCartQuery, con))
                    {
                        cmd.Parameters.Add(new OracleParameter("userId", OracleDbType.Int32)).Value = userId;
                        cmd.ExecuteNonQuery();
                        MessageBox.Show($"Cart successfully created for user_id {userId}.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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




        // Button to navigate back to Form1
        private void button2_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }

        // Toggle password visibility
        private void button3_Click(object sender, EventArgs e)
        {
            textBox2.PasswordChar = textBox2.PasswordChar == '*' ? '\0' : '*';
        }
    }


    
}