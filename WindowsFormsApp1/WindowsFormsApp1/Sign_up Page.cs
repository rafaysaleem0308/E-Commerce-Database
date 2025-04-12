using Oracle.ManagedDataAccess.Client;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    

    public partial class Sign_up_Page : Form
    {
        public Sign_up_Page()
        {
            InitializeComponent();

            // Add hover effects for buttons
            button1.MouseEnter += Button_MouseEnter;
            button1.MouseLeave += Button_MouseLeave;
            button2.MouseEnter += Button2_MouseEnter;
            button2.MouseLeave += Button2_MouseLeave;

            // Set custom font for textboxes
            textBox2.Font = new Font(textBox2.Font.FontFamily, 10, FontStyle.Bold);  // Smaller, bold font for password field
            textBox3.Font = new Font(textBox3.Font.FontFamily, 10, FontStyle.Bold);  // Smaller, bold font for confirm password field

            // Set PasswordChar to hide text in password and confirm password fields
            textBox2.PasswordChar = '*';  // Password textbox will show dots
            textBox3.PasswordChar = '*';  // Confirm password textbox will show dots
        }

        // Hover effects for buttons
        private void Button_MouseEnter(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                btn.BackColor = Color.Pink;  // Hover background color
                btn.ForeColor = Color.White; // Hover text color
            }
        }

        private void Button_MouseLeave(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                btn.BackColor = SystemColors.Control; // Default background color
                btn.ForeColor = SystemColors.ControlText; // Default text color
            }
        }

       
        private void button1_Click(object sender, EventArgs e)
        {
            // Retrieve values from textboxes
            string username = textBox1.Text.Trim();
            string password = textBox2.Text.Trim();
            string confirmPassword = textBox3.Text.Trim(); // Get the confirm password from textBox3
            string email = textBox4.Text.Trim();
            string role = comboBox1.SelectedItem?.ToString().ToLower();

            // Get the current timestamp
            DateTime created_at = DateTime.Now;

            // Validation: Check if all fields are filled
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(role) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("Please fill out all fields and select a role from the list.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check if the password and confirm password match
            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match. Please re-enter the password.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate password strength
            if (!IsPasswordStrong(password))
            {
                MessageBox.Show("Password is weak. Please include at least 8 characters, with uppercase, lowercase, a number, and a special character.", "Weak Password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Generate unique user ID
            int userID = GenerateUniqueUserID();

            // Pass the values to the connection method
            Connection cn = new Connection();
            cn.WriteData_of_users(username, password, email, role, userID, created_at);

            // Redirect to the appropriate form based on the role
            if (role == "customer")
            {   this.Hide();
                Customer_Dashboard customerDashboard = new Customer_Dashboard();
                customerDashboard.Show();
            }
            else if (role == "delivery")
            {
                this.Hide();
                Delievery deliveryDashboard = new Delievery();
                deliveryDashboard.Show();
            }
            else if (role == "manager")
            {
                this.Hide();
                Manager adminDashboard = new Manager();
                adminDashboard.Show();
            }
            else if (role == "cashier")
            {
                this.Hide();
                Cashier cashierDashboard = new Cashier();
                cashierDashboard.Show();
            }

            // Close or hide the signup form
            this.Hide();
        }



        // Method to generate a unique userID
        public int GenerateUniqueUserID()
        {
            Random random = new Random();
            int userID;

            while (true)
            {
                userID = random.Next(1000, 9999); // Generate a random 4-digit number

                using (OracleConnection con = new OracleConnection(Connection.connect))
                {
                    con.Open();
                    using (OracleCommand cmd = new OracleCommand("SELECT COUNT(*) FROM users WHERE user_id = :userID", con))
                    {
                        cmd.Parameters.Add(new OracleParameter("userID", OracleDbType.Int32)).Value = userID;

                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        if (count == 0) // Unique ID found
                        {
                            return userID;
                        }
                    }
                }
            }
        }

        // Method to check if the password is strong
        private bool IsPasswordStrong(string password)
        {
            // Check if the password meets the criteria
            if (password.Length < 8)
                return false; // Minimum length check

            if (!password.Any(char.IsUpper))
                return false; // At least one uppercase letter

            if (!password.Any(char.IsLower))
                return false; // At least one lowercase letter

            if (!password.Any(char.IsDigit))
                return false; // At least one numeric digit

            if (!password.Any(ch => "!@#$%^&*()_+|".Contains(ch)))
                return false; // At least one special character

            if (password.Contains(' '))
                return false; // No spaces allowed

            return true; // Password is strong
        }


        // Button2 click for going back to login page
        private void button2_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }

        // Toggle visibility of password field (button3 click)
        private void button3_Click(object sender, EventArgs e)
        {
            // Toggle the PasswordChar for password field
            if (textBox2.PasswordChar == '*')
            {
                textBox2.PasswordChar = '\0';  // Show password text
            }
            else
            {
                textBox2.PasswordChar = '*';  // Hide password text with dots
            }
        }

        // Toggle visibility of confirm password field (button4 click)
        private void button4_Click(object sender, EventArgs e)
        {
            // Toggle the PasswordChar for confirm password field
            if (textBox3.PasswordChar == '*')
            {
                textBox3.PasswordChar = '\0';  // Show confirm password text
            }
            else
            {
                textBox3.PasswordChar = '*';  // Hide confirm password text with dots
            }
        }

        // Hover effect for button2 (back button)
        private void Button2_MouseEnter(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                btn.BackColor = Color.Pink; // Change button background to pink
                btn.ForeColor = Color.White; // Change text color to white
            }
        }

        private void Button2_MouseLeave(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                btn.BackColor = SystemColors.Control; // Reset to default background color
                btn.ForeColor = SystemColors.ControlText; // Reset to default text color
            }
        }

        // Empty event handlers for additional UI controls (if needed)
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void textBox3_TextChanged(object sender, EventArgs e) { }
        private void textBox4_TextChanged(object sender, EventArgs e) { }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) { }

        private void Sign_up_Page_Load(object sender, EventArgs e)
        {

        }
    }


}
