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
    public partial class Manager : Form
    {
        public class Category
        {
            public int CategoryId { get; set; }
            public string Name { get; set; }
        }
      


        public Manager()
        {
            InitializeComponent();
            dataGridView1.CellContentClick += DataGridView1_CellContentClick; // Attach event for cell content click
            button1.MouseEnter += button1_MouseEnter;
            button1.MouseLeave += button1_MouseLeave;
            button2.MouseEnter += button2_MouseEnter;
            button2.MouseLeave += button2_MouseLeave;

        }

        // Add these event handlers in your form's constructor or `Load` method:

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            button1.BackColor = Color.CornflowerBlue;  // Hover color
            button1.ForeColor = Color.White;  // Text color on hover
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.BackColor = Color.FromArgb(0, 100, 255);  // Original color
            button1.ForeColor = Color.White;  // Text color
        }

        // Repeat similar methods for other buttons like button2, button3, button4

        private void button2_MouseEnter(object sender, EventArgs e)
        {
            button2.BackColor = Color.CornflowerBlue;
            button2.ForeColor = Color.White;
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            button2.BackColor = Color.FromArgb(0, 100, 255);
            button2.ForeColor = Color.White;
        }

        private void StyleButtons()
        {
            // Custom button styling for all buttons
            foreach (Control control in this.Controls)
            {
                if (control is Button)
                {
                    Button btn = (Button)control;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.BackColor = Color.FromArgb(0, 100, 255);
                    btn.ForeColor = Color.White;
                    btn.Font = new Font("Arial", 10, FontStyle.Bold);

                    // Hover effect
                    btn.MouseEnter += (s, e) =>
                    {
                        btn.BackColor = Color.CornflowerBlue;
                    };

                    btn.MouseLeave += (s, e) =>
                    {
                        btn.BackColor = Color.FromArgb(0, 100, 255);
                    };
                }
            }
        }



        private void Admin_Load(object sender, EventArgs e)
        {
           
        }
      


        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Search button functionality: Display all products in the data grid
            LoadAllProducts();
        }

      
        private void LoadAllProducts()
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Connection.connect))
                {
                    con.Open();
                    string query = @"SELECT p.product_id, p.name, p.description, p.price, p.stock_quantity, c.name AS category_name
                             FROM products p
                             JOIN categories c ON p.category_id = c.category_id
                             ORDER BY p.product_id ASC";

                    OracleDataAdapter da = new OracleDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;

                    // Ensure AutoGenerateColumns is true if columns are generated from DataTable
                    dataGridView1.AutoGenerateColumns = true;

                    // Styling DataGridView
                    dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);  // Light grey for alternating rows
                    dataGridView1.DefaultCellStyle.BackColor = Color.White;  // White for regular rows
                    dataGridView1.DefaultCellStyle.ForeColor = Color.FromArgb(33, 37, 41);  // Dark text for clarity
                    dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 123, 255);  // Blue for selection
                    dataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;  // White text on selection

                    // Column headers styling with smaller font size
                    dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);  // Reduced font size to 9
                    dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 85, 85);  // Red background for headers
                    dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;  // White text in headers
                    dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;  // Center align headers

                    // Customize border and gridlines
                    dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
                    dataGridView1.BorderStyle = BorderStyle.FixedSingle;
                    dataGridView1.BackgroundColor = Color.White;

                    // Adjusting column widths
                    foreach (DataGridViewColumn column in dataGridView1.Columns)
                    {
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;  // Automatically adjusts columns to fit content
                    }

                    // You can manually set column width if needed:
                    // Example:
                    // dataGridView1.Columns["name"].Width = 150;  // Set column "name" width to 150 pixels
                    // dataGridView1.Columns["description"].Width = 300;  // Set column "description" width to 300 pixels

                    // Add Delete button dynamically if not already added
                    if (!dataGridView1.Columns.Contains("Delete"))
                    {
                        DataGridViewButtonColumn deleteButton = new DataGridViewButtonColumn
                        {
                            Name = "Delete",
                            Text = "Delete",
                            UseColumnTextForButtonValue = true
                        };
                        dataGridView1.Columns.Add(deleteButton);
                    }

                    // Refresh the DataGridView to apply styling changes
                    dataGridView1.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading products: " + ex.Message);
            }
        }



        private void button3_Click(object sender, EventArgs e)
        {
            Form1 f1 = new Form1();
            f1.Show();
        }



        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                // Handle the case of adding a new product
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.IsNewRow) continue;

                    int productId = Convert.ToInt32(row.Cells["product_id"].Value);
                    string name = row.Cells["name"].Value.ToString();
                    string description = row.Cells["description"].Value.ToString();
                    decimal price = Convert.ToDecimal(row.Cells["price"].Value);
                    int stockQuantity = Convert.ToInt32(row.Cells["stock_quantity"].Value);
                    string selectedCategory = row.Cells["category_name"].Value.ToString();

                 
                    // Check if the product already exists by product_id
                    if (ProductExists(productId))
                    {
                        MessageBox.Show("Product ID already exists. Please use a unique product ID.");
                        continue;
                    }

                    // Get the category ID based on the category name
                    int categoryId = GetCategoryIdByName(selectedCategory);

                    if (categoryId == -1)
                    {
                        // If category does not exist, insert a new category and get its ID
                        if (CategoryExists(selectedCategory))
                        {
                            MessageBox.Show("Category already exists.");
                        }
                        else
                        {
                            using (OracleConnection con = new OracleConnection(Connection.connect))
                            {
                                con.Open();
                                string insertCategoryQuery = "INSERT INTO categories (name) VALUES (:category_name)";
                                OracleCommand cmd = new OracleCommand(insertCategoryQuery, con);
                                cmd.Parameters.Add(new OracleParameter(":category_name", selectedCategory));
                                cmd.ExecuteNonQuery();
                            }

                            // After inserting, get the newly inserted category ID
                            categoryId = GetCategoryIdByName(selectedCategory);
                        }
                    }

                    // Insert the new product with the correct category ID
                    using (OracleConnection con = new OracleConnection(Connection.connect))
                    {
                        con.Open();
                        string insertProductQuery = @"
                    INSERT INTO products (product_id, name, description, price, stock_quantity, category_id)
                    VALUES (:product_id, :name, :description, :price, :stock_quantity, :category_id)";

                        OracleCommand cmd = new OracleCommand(insertProductQuery, con);
                        cmd.Parameters.Add(new OracleParameter(":product_id", productId));
                        cmd.Parameters.Add(new OracleParameter(":name", name));
                        cmd.Parameters.Add(new OracleParameter(":description", description));
                        cmd.Parameters.Add(new OracleParameter(":price", price));
                        cmd.Parameters.Add(new OracleParameter(":stock_quantity", stockQuantity));
                        cmd.Parameters.Add(new OracleParameter(":category_id", categoryId));

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("New product added successfully!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding product: " + ex.Message);
            }
        }







        private bool ProductExists(int productId)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Connection.connect))
                {
                    con.Open();
                    string query = "SELECT COUNT(*) FROM products WHERE product_id = :product_id";
                    OracleCommand cmd = new OracleCommand(query, con);
                    cmd.Parameters.Add(new OracleParameter(":product_id", productId));

                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    return count > 0; // Returns true if the product ID exists
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error checking product existence: " + ex.Message);
                return false;
            }
        }




        private int GetCategoryIdByName(string categoryName)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Connection.connect))
                {
                    con.Open();
                    string query = "SELECT category_id FROM categories WHERE name = :category_name";
                    OracleCommand cmd = new OracleCommand(query, con);
                    cmd.Parameters.Add(new OracleParameter(":category_name", categoryName));

                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        return Convert.ToInt32(result);
                    }
                    else
                    {
                        return -1;  // Category not found
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching category ID: " + ex.Message);
                return -1;
            }
        }

        private bool CategoryExists(string categoryName)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(Connection.connect))
                {
                    con.Open();
                    string query = "SELECT COUNT(*) FROM categories WHERE name = :category_name";
                    OracleCommand cmd = new OracleCommand(query, con);
                    cmd.Parameters.Add(new OracleParameter(":category_name", categoryName));

                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    return count > 0; // Returns true if the category already exists
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error checking category existence: " + ex.Message);
                return false;
            }
        }








        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns["Delete"].Index)
            {
                if (MessageBox.Show("Are you sure you want to delete this product?", "Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    int productId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["product_id"].Value);

                    using (OracleConnection con = new OracleConnection(Connection.connect))
                    {
                        con.Open();
                        string query = "DELETE FROM products WHERE product_id = :product_id";
                        OracleCommand cmd = new OracleCommand(query, con);
                        cmd.Parameters.Add(new OracleParameter(":product_id", productId));
                        cmd.ExecuteNonQuery();
                    }

                    LoadAllProducts();  // Refresh product list
                    MessageBox.Show("Product deleted successfully.");
                }
            }
        }

      

        // Add button functionality: Add new product to the database
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                // Add button functionality: Insert a new row in the DataGridView for new product
                int product_Id = 0; // Auto-generated by the database
                string name = "New Product"; // Replace with TextBox.Text for actual user input
                string description = "Description"; // Replace with TextBox.Text for actual user input
                decimal price = 100.00m; // Replace with TextBox.Text for actual user input
                int stockQuantity = 10; // Replace with TextBox.Text for actual user input
                string category = "Category"; // Replace with TextBox.Text for actual user input

                // Create a new DataRow for the new product
                DataTable dt = (DataTable)dataGridView1.DataSource;
                DataRow newRow = dt.NewRow();
                newRow["product_id"] = product_Id;
                newRow["name"] = name;
                newRow["description"] = description;
                newRow["price"] = price;
                newRow["stock_quantity"] = stockQuantity;
                newRow["category_name"] = category;

                dt.Rows.Add(newRow);



                MessageBox.Show("New product row added to the DataGridView!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding product row: " + ex.Message);
            }
        }





    }
}
