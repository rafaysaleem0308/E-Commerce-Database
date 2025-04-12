using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Review_Page : Form
    {
        private int productId;
        private int selectedRating = 0;
        public Review_Page(int productId)
        {
            InitializeComponent();
            this.productId = productId;
            ApplyButtonAndRadioStyles();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void ApplyButtonAndRadioStyles()
        {
            // Radio Button Styling
            foreach (var control in this.Controls)
            {
                if (control is RadioButton radioButton)
                {
                    radioButton.ForeColor = Color.DarkSlateBlue;
                    radioButton.Font = new Font("Segoe UI", 10, FontStyle.Regular);
                    radioButton.FlatStyle = FlatStyle.Flat;
                    radioButton.FlatAppearance.BorderColor = Color.DarkSlateGray;
                    radioButton.FlatAppearance.CheckedBackColor = Color.LightSkyBlue;
                }
            }

            // Button Styling
            button1.BackColor = Color.CornflowerBlue;
            button1.FlatStyle = FlatStyle.Flat;
            button1.FlatAppearance.BorderSize = 1;
            button1.ForeColor = Color.White;
            button1.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            button1.Cursor = Cursors.Hand;

            button2.BackColor = Color.LightSkyBlue;
            button2.FlatStyle = FlatStyle.Flat;
            button2.FlatAppearance.BorderSize = 1;
            button2.ForeColor = Color.White;
            button2.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            button2.Cursor = Cursors.Hand;

            // Adjust Button Sizes for Consistency
            button1.Size = new Size(120, 40);
            button2.Size = new Size(120, 40);
        }



        private void button1_Click(object sender, EventArgs e) // Submit Button
        {
            string reviewComment = textBox1.Text.Trim();

            // Validate inputs
            if (string.IsNullOrEmpty(reviewComment))
            {
                MessageBox.Show("Please enter a comment.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (selectedRating == 0)
            {
                MessageBox.Show("Please select a rating using the Select button.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int userId = Sessional.UserId; // Fetch the current user's ID

            if (userId == 0)
            {
                MessageBox.Show("Unable to identify the user. Please log in again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Insert review into the database
            if (SubmitReview(productId, userId, selectedRating, reviewComment))
            {
                MessageBox.Show("Review submitted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Failed to submit the review. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Hide();
            Customer_Dashboard cd = new Customer_Dashboard();
            cd.Show();
            
        }

        private bool SubmitReview(int productId, int userId, int rating, string reviewComment)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Connection.connect))
                {
                    con.Open();
                    string query = @"
                        INSERT INTO reviews (review_id, product_id, user_id, rating, review_comment)
                        VALUES (reviews_seq.nextval, :productId, :userId, :rating, :reviewComment)";

                    using (OracleCommand cmd = new OracleCommand(query, con))
                    {
                        cmd.Parameters.Add(new OracleParameter("productId", OracleDbType.Int32)).Value = productId;
                        cmd.Parameters.Add(new OracleParameter("userId", OracleDbType.Int32)).Value = userId;
                        cmd.Parameters.Add(new OracleParameter("rating", OracleDbType.Int32)).Value = rating;
                        cmd.Parameters.Add(new OracleParameter("reviewComment", OracleDbType.Varchar2)).Value = reviewComment;

                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
           
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e) // Select Button
        {
            // Capture the selected rating
            if (radioButton1.Checked) selectedRating = 1;
            if (radioButton2.Checked) selectedRating = 2;
            if (radioButton3.Checked) selectedRating = 3;
            if (radioButton4.Checked) selectedRating = 4;
            if (radioButton5.Checked) selectedRating = 5;

            if (selectedRating == 0)
            {
                MessageBox.Show("Please select a rating before clicking Select.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show($"You have selected a rating of {selectedRating}.", "Rating Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void Review_Page_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            this.Hide();
            Customer_Dashboard customer_Dashboard = new Customer_Dashboard();
            customer_Dashboard.Show();
        }
    }
}
