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
    public partial class Order_history : Form
    {

        public Order_history(int userId)
        {
            InitializeComponent();

            
        }

        private void Order_history_Load(object sender, EventArgs e)
        {
            
        }

    
        private void button2_Click(object sender, EventArgs e)
        {
            int currentUserId = Sessional.UserId;
            MessageBox.Show("Current User ID: " + currentUserId); // Check User ID

            using (OracleConnection con = new OracleConnection(Connection.connect))
            {
                try
                {
                    con.Open();

                    // Query to fetch the order history with product name, payment info, and status
                    string query = @"
            SELECT o.order_id, o.order_date, o.status, o.total_price, 
                   oi.quantity, p.name AS product_name, oi.price AS product_price,
                   p.product_id,
                   (SELECT MAX(payment_method) FROM payments WHERE order_id = o.order_id) AS payment_method
            FROM orders o
            JOIN order_items oi ON o.order_id = oi.order_id
            JOIN products p ON oi.product_id = p.product_id
            WHERE o.user_id = :user_id
            ORDER BY o.order_date DESC";

                    using (OracleCommand cmd = new OracleCommand(query, con))
                    {
                        cmd.Parameters.Add("user_id", currentUserId);

                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            // If no data found
                            if (dt.Rows.Count == 0)
                            {
                                MessageBox.Show("No data found for this user.");
                            }

                            // Bind the result to the DataGridView
                            dataGridView1.DataSource = dt;

                            // Configure columns in DataGridView
                            ConfigureGridViewColumns();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading order history: " + ex.Message);
                }
            }

            // Apply styling to the DataGridView
            StyleDataGridView();

            ConfigureGridViewColumns();
        }

        private void ConfigureGridViewColumns()
        {
            // Set headers for the DataGridView columns based on the query result
            dataGridView1.Columns[0].HeaderText = "ORDER_ID";           // Order ID
            dataGridView1.Columns[1].HeaderText = "ORDER_DATE";         // Order Date
            dataGridView1.Columns[2].HeaderText = "STATUS";             // Status
            dataGridView1.Columns[3].HeaderText = "TOTAL_PRICE";        // Total Price
            dataGridView1.Columns[4].HeaderText = "QUANTITY";           // Quantity
            dataGridView1.Columns[5].HeaderText = "PRODUCT_NAME";       // Product Name
            dataGridView1.Columns[6].HeaderText = "PRODUCT_PRICE";      // Product Price
            dataGridView1.Columns[7].HeaderText = "PRODUCT_ID";         // Product ID (previously Payment Method)

            // Align columns appropriately
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // Order ID
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // Order Date
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // Status
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;  // Total Price
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // Quantity
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;   // Product Name
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;  // Product Price
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // Product ID (formerly Payment Method)

            // Format the numeric columns (Total Price, Product Price) to 2 decimal places
            dataGridView1.Columns[3].DefaultCellStyle.Format = "C2"; // Total Price - currency format
            dataGridView1.Columns[6].DefaultCellStyle.Format = "C2"; // Product Price - currency format

            // Format the Order Date column to a readable format (MM/dd/yyyy)
            dataGridView1.Columns[1].DefaultCellStyle.Format = "MM/dd/yyyy hh:mm:ss tt"; // Order Date with time
        }




        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Handle cell content click if necessary
        }

        private void Order_history_Load_1(object sender, EventArgs e)
        {

        }
        private void StyleDataGridView()
        {
            // Basic styling for the DataGridView
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(238, 239, 249);
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(55, 125, 255);
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;
            dataGridView1.DefaultCellStyle.BackColor = Color.White;
            dataGridView1.DefaultCellStyle.Font = new Font("Calibri", 12);
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(20, 25, 72);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Calibri", 13, FontStyle.Bold);
            dataGridView1.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridView1.GridColor = Color.FromArgb(193, 199, 206);

            // Adjust column widths automatically based on content
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            // Optional: Add a horizontal scroll bar if necessary (for wide tables)
            dataGridView1.ScrollBars = ScrollBars.Both;

            // Adjust the text alignment for different columns
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // Order ID
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // Order Date
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // Status
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight; // Total Price
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // Quantity
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft; // Product Name
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight; // Product Price
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft; // Payment Method

            // Format the numeric columns (Total Price, Product Price) to 2 decimal places
            dataGridView1.Columns[3].DefaultCellStyle.Format = "C2"; // Total Price - currency format
            dataGridView1.Columns[6].DefaultCellStyle.Format = "C2"; // Product Price - currency format

            // Format the Order Date column to a readable format
            dataGridView1.Columns[1].DefaultCellStyle.Format = "MM/dd/yyyy"; // Order Date
        }



        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Customer_Dashboard cd = new Customer_Dashboard();
            cd.Show();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
    }
}
