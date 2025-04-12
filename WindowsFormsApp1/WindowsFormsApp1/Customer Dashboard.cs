using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Customer_Dashboard : Form
    {
        public Customer_Dashboard()
        {
            InitializeComponent();
        }

      

        private void CustomizeDataGridView()
        {
            // Set the background color of the DataGridView
            dataGridView1.BackgroundColor = Color.WhiteSmoke;

            // Set grid lines
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.Single;

            // Set font and header style
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Teal;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.ColumnHeadersHeight = 35;

            // Set default row cell style (font, selection colors)
            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(64, 224, 208); // Light teal
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;

            // Set alternating row colors for better readability
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240); // Light gray

            // Row height adjustment for better readability
            dataGridView1.RowTemplate.Height = 30;

            // Auto-size columns based on content
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Set width of the select checkbox column
            dataGridView1.Columns["select"].Width = 50;

            // Hide columns that are not necessary (like product_id or cart_item_id)
            dataGridView1.Columns["product_id"].Visible = false;

            // Style quantity and price columns (optional)
            dataGridView1.Columns["quantity"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns["quantity"].DefaultCellStyle.BackColor = Color.LightYellow;

            dataGridView1.Columns["price"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns["price"].DefaultCellStyle.Format = "C";  // Currency format

            // Ensure last column fills the remaining space
            dataGridView1.Columns[dataGridView1.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void Search_Click(object sender, EventArgs e)
        {
            string productName = textBox1.Text.Trim(); // Get the product name entered by the user
            string categoryName = comboBox1.SelectedItem?.ToString(); // Get the selected category

            if (string.IsNullOrWhiteSpace(categoryName))
            {
                MessageBox.Show("Please select a category.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Create an instance of the Connection class
            Connection connection = new Connection();

            // Fetch search results using the updated SearchProducts method
            DataTable results = connection.SearchProducts(productName, categoryName);

            if (results.Rows.Count == 0)
            {
                MessageBox.Show("No products found for the given criteria.", "Search Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Bind results to the DataGridView
            dataGridView1.DataSource = results;

            // Ensure the quantity column is added if not already present
            AddColumnIfNotExists("quantity", "Quantity", typeof(int), DataGridViewContentAlignment.MiddleCenter);

            // Ensure the select column is added if not already present
            AddCheckBoxColumnIfNotExists("select", "Select");

            // Customize the DataGridView appearance
            CustomizeDataGridView();
        }




        private void AddColumnIfNotExists(string columnName, string headerText, Type valueType, DataGridViewContentAlignment alignment)
        {
            if (!dataGridView1.Columns.Contains(columnName))
            {
                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn
                {
                    Name = columnName,
                    HeaderText = headerText,
                    ValueType = valueType,
                    DefaultCellStyle = new DataGridViewCellStyle { Alignment = alignment }
                };
                dataGridView1.Columns.Add(column);
            }
        }

        private void AddCheckBoxColumnIfNotExists(string columnName, string headerText)
        {
            if (!dataGridView1.Columns.Contains(columnName))
            {
                DataGridViewCheckBoxColumn column = new DataGridViewCheckBoxColumn
                {
                    Name = columnName,
                    HeaderText = headerText,
                    TrueValue = true,
                    FalseValue = false
                };
                dataGridView1.Columns.Add(column);
            }
        }
        private void AddRadioButtonColumnIfNotExists()
        {
            if (!dataGridView1.Columns.Contains("select"))
            {
                DataGridViewCheckBoxColumn radioButtonColumn = new DataGridViewCheckBoxColumn
                {
                    Name = "select",
                    HeaderText = "Select",
                    FalseValue = false,
                    TrueValue = true,
                    CellTemplate = new DataGridViewCheckBoxCell { Style = { Alignment = DataGridViewContentAlignment.MiddleCenter } }
                };
                dataGridView1.Columns.Insert(0, radioButtonColumn);
            }
        }


        //private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        //{
        //    if (e.ColumnIndex == dataGridView1.Columns["select"].Index && e.RowIndex >= 0)
        //    {
        //        DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];

        //        // Retrieve the cart_id for the user
        //        int cartId = GetCartId();
        //        if (cartId == 0)
        //        {
        //            MessageBox.Show("Unable to identify the cart. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            return;
        //        }

        //        // Get product details from the selected row
        //        int productId = Convert.ToInt32(selectedRow.Cells["product_id"].Value); // Replace with actual column name
        //        int quantity;
        //        if (!int.TryParse(Convert.ToString(selectedRow.Cells["quantity"].Value), out quantity) || quantity <= 0)
        //        {
        //            MessageBox.Show("Please enter a valid quantity greater than 0.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //            return;
        //        }

        //        // Add the product to the cart
        //        if (AddProductToCart(cartId, productId, quantity))
        //        {
        //            MessageBox.Show("Product successfully added to the cart!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        }
        //    }
        //}

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "select" && e.RowIndex >= 0)
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    row.Cells["select"].Value = false; // Deselect all other rows
                }
                dataGridView1.Rows[e.RowIndex].Cells["select"].Value = true; // Select the clicked row
            }
        }



        private bool AddProductToCart(int cartId, int productId, int quantity)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Connection.connect))
                {
                    con.Open();
                    string query = @"
                INSERT INTO cart_items (cart_item_id, cart_id, product_id, quantity)
                VALUES (cart_items_seq.nextval, :cartId, :productId, :quantity)";

                    using (OracleCommand cmd = new OracleCommand(query, con))
                    {
                        cmd.Parameters.Add(new OracleParameter("cartId", OracleDbType.Int32)).Value = cartId;
                        cmd.Parameters.Add(new OracleParameter("productId", OracleDbType.Int32)).Value = productId;
                        cmd.Parameters.Add(new OracleParameter("quantity", OracleDbType.Int32)).Value = quantity;
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Retrieve the cart_id for the user
            int cartId = GetCartId();
            if (cartId == 0)
            {
                MessageBox.Show("Cart not found. Please ensure you are logged in.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validate and gather selected products
            var selectedProducts = GetSelectedProducts();
            if (selectedProducts.Count == 0)
            {
                MessageBox.Show("No products selected. Please select products and try again.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Add selected products to the cart
            foreach (var (productId, quantity) in selectedProducts)
            {
                if (!AddProductToCart(cartId, productId, quantity))
                {
                    return;
                }
            }

            MessageBox.Show("Products successfully added to the cart!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private List<(int ProductId, int Quantity)> GetSelectedProducts()
        {
            List<(int ProductId, int Quantity)> selectedProducts = new List<(int ProductId, int Quantity)>();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["select"].Value is bool isSelected && isSelected)
                {
                    int productId = Convert.ToInt32(row.Cells["product_id"].Value); // Replace with actual column name
                    int quantity;
                    if (!int.TryParse(Convert.ToString(row.Cells["quantity"].Value), out quantity) || quantity <= 0)
                    {
                        MessageBox.Show($"Invalid quantity for product ID: {productId}. Please correct it.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return new List<(int ProductId, int Quantity)>();
                    }
                    selectedProducts.Add((productId, quantity));
                }
            }

            return selectedProducts;
        }

        private int GetCartId()
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Connection.connect))
                {
                    con.Open();
                    string query = "SELECT cart_id FROM cart WHERE ROWNUM = 1"; // Retrieve the first cart_id
                    using (OracleCommand cmd = new OracleCommand(query, con))
                    {
                        var result = cmd.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : 0;
                    }
                }
            }
            catch
            {
                return 0;
            }
        }









        private void button2_Click(object sender, EventArgs e)
        {
           
            Cart_Page cart_Page = new Cart_Page();
            cart_Page.Show();
            this.Hide();
        }

       

        private void Customer_Dashboard_Load(object sender, EventArgs e)
        {
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DataGridViewRow selectedRow = null;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["select"].Value is bool isSelected && isSelected)
                {
                    selectedRow = row;
                    break;
                }
            }

            if (selectedRow == null)
            {
                MessageBox.Show("Please select a product to review.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Retrieve product details from the selected row
            int productId = Convert.ToInt32(selectedRow.Cells["product_id"].Value); // Replace with the correct column name

            // Navigate to the Review_Page with the productId
            Review_Page reviewPage = new Review_Page(productId);
            reviewPage.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Get the current userId (assuming it's stored in Sessional.UserId)
            int userId = Sessional.UserId;

            // Pass the userId to the Order_history form
            Order_history orderHistory = new Order_history(userId);
            orderHistory.Show();
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        { this.Hide();
            Login_Page loginPage = new Login_Page();
            loginPage.Show();
        }
    }

}