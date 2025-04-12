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
    public partial class Cart_Page : Form
    {
        public Cart_Page()
        {
            InitializeComponent();
            LoadCartItems();
        }

        private void CartPage_Load(object sender, EventArgs e)
        {
            dataGridView1.CellContentClick += dataGridView1_CellContentClick; // Attach the event handler
        }

        //private void LoadCartItems()
        //{
        //    int userId = Sessional.UserId; // Use the logged-in user's ID from the session

        //    try
        //    {
        //        using (OracleConnection connection = new OracleConnection(Connection.connect))
        //        {
        //            connection.Open();

        //            // Query to fetch all cart items for the logged-in user
        //            string query = @"
        //        SELECT 
        //            ci.cart_item_id AS ""Cart Item ID"",
        //            p.name AS ""Product Name"",
        //            ci.quantity AS ""Quantity"",
        //            p.price AS ""Price""
        //        FROM cart_items ci
        //        JOIN products p ON ci.product_id = p.product_id
        //        JOIN cart c ON ci.cart_id = c.cart_id
        //        WHERE c.user_id = :user_id";

        //            OracleCommand command = new OracleCommand(query, connection);
        //            command.Parameters.Add(new OracleParameter("user_id", userId));

        //            OracleDataAdapter adapter = new OracleDataAdapter(command);
        //            DataTable dataTable = new DataTable();
        //            adapter.Fill(dataTable);

        //            if (dataTable.Rows.Count == 0)
        //            {
        //                MessageBox.Show("No data found for the current user.");
        //                return; // Exit if no data found
        //            }

        //            // Bind the DataTable to the DataGridView
        //            dataGridView1.DataSource = dataTable;

        //            // Configure DataGridView
        //            dataGridView1.ReadOnly = false; // Allow interaction
        //            dataGridView1.AllowUserToAddRows = false;
        //            dataGridView1.AllowUserToDeleteRows = false;
        //            dataGridView1.RowHeadersVisible = false;
        //            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        //            dataGridView1.DefaultCellStyle.BackColor = Color.White;
        //            dataGridView1.DefaultCellStyle.ForeColor = Color.Black;
        //            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.Blue;
        //            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;

        //            // Add Add and Remove buttons if not already present
        //            AddButtonColumn("Add", "Add");
        //            AddButtonColumn("Remove", "Remove");

        //            dataGridView1.ClearSelection(); // Clear any default row selection
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        //private void AddButtonColumn(string columnName, string buttonText)
        //{
        //    if (!dataGridView1.Columns.Contains(columnName))
        //    {
        //        DataGridViewButtonColumn buttonColumn = new DataGridViewButtonColumn
        //        {
        //            Name = columnName,
        //            HeaderText = buttonText,
        //            Text = buttonText,
        //            UseColumnTextForButtonValue = true,
        //            AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
        //        };
        //        dataGridView1.Columns.Add(buttonColumn);
        //    }
        //}
        private void LoadCartItems()
        {
            int userId = Sessional.UserId; // Use the logged-in user's ID from the session

            try
            {
                using (OracleConnection connection = new OracleConnection(Connection.connect))
                {
                    connection.Open();

                    // Query to fetch all cart items for the logged-in user
                    string query = @"
                SELECT 
                    ci.cart_item_id AS ""Cart Item ID"",
                    p.name AS ""Product Name"",
                    ci.quantity AS ""Quantity"",
                    p.price AS ""Price""
                FROM cart_items ci
                JOIN products p ON ci.product_id = p.product_id
                JOIN cart c ON ci.cart_id = c.cart_id
                WHERE c.user_id = :user_id";

                    OracleCommand command = new OracleCommand(query, connection);
                    command.Parameters.Add(new OracleParameter("user_id", userId));

                    OracleDataAdapter adapter = new OracleDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    if (dataTable.Rows.Count == 0)
                    {
                        MessageBox.Show("No data found for the current user.");
                        return; // Exit if no data found
                    }

                    // Bind the DataTable to the DataGridView
                    dataGridView1.DataSource = dataTable;

                    // Configure DataGridView Appearance
                    dataGridView1.ReadOnly = false; // Allow interaction
                    dataGridView1.AllowUserToAddRows = false;
                    dataGridView1.AllowUserToDeleteRows = false;
                    dataGridView1.RowHeadersVisible = false;
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dataGridView1.DefaultCellStyle.BackColor = Color.White;
                    dataGridView1.DefaultCellStyle.ForeColor = Color.Black;
                    dataGridView1.DefaultCellStyle.SelectionBackColor = Color.Navy;
                    dataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;
                    dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 10);
                    dataGridView1.RowTemplate.Height = 30;
                    dataGridView1.DefaultCellStyle.Padding = new Padding(5);

                    // Customize Header Style
                    dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.LightSteelBlue;
                    dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
                    dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
                    dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dataGridView1.EnableHeadersVisualStyles = false;

                    // Set Alternating Row Colors
                    dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;

                    // Add Add and Remove buttons if not already present
                    AddButtonColumn("Add", "Add", "Add this item");
                    AddButtonColumn("Remove", "Remove", "Remove this item");

                    dataGridView1.ClearSelection(); // Clear any default row selection
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddButtonColumn(string columnName, string text, string toolTip)
        {
            if (!dataGridView1.Columns.Contains(columnName))
            {
                DataGridViewButtonColumn buttonColumn = new DataGridViewButtonColumn
                {
                    Name = columnName,
                    HeaderText = columnName,
                    Text = text,
                    UseColumnTextForButtonValue = true,
                    ToolTipText = toolTip,
                    FlatStyle = FlatStyle.Flat,
                    DefaultCellStyle = { BackColor = Color.DarkSlateBlue, ForeColor = Color.White }
                };
                dataGridView1.Columns.Add(buttonColumn);
            }
        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ensure the click is on a button column
            if (e.RowIndex >= 0)
            {
                string columnName = dataGridView1.Columns[e.ColumnIndex].Name;

                // Get the current row data
                int cartItemId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["Cart Item ID"].Value);
                int currentQuantity = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["Quantity"].Value);

                if (columnName == "Add")
                {
                    // Increase the quantity by 1
                    UpdateCartItemQuantity(cartItemId, currentQuantity + 1);
                }
                else if (columnName == "Remove")
                {
                    if (currentQuantity > 1)
                    {
                        // Decrease the quantity by 1
                        UpdateCartItemQuantity(cartItemId, currentQuantity - 1);
                    }
                    else
                    {
                        // Remove the item if quantity becomes 0
                        DeleteCartItem(cartItemId);
                    }
                }

                // Reload the cart items to refresh the grid
                LoadCartItems();
            }
        }


        private void UpdateCartItemQuantity(int cartItemId, int newQuantity)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(Connection.connect))
                {
                    connection.Open();

                    string query = "UPDATE cart_items SET quantity = :quantity WHERE cart_item_id = :cart_item_id";
                    OracleCommand command = new OracleCommand(query, connection);
                    command.Parameters.Add(new OracleParameter("quantity", newQuantity));
                    command.Parameters.Add(new OracleParameter("cart_item_id", cartItemId));

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating quantity: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void DeleteCartItem(int cartItemId)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(Connection.connect))
                {
                    connection.Open();

                    string query = "DELETE FROM cart_items WHERE cart_item_id = :cart_item_id";
                    OracleCommand command = new OracleCommand(query, connection);
                    command.Parameters.Add(new OracleParameter("cart_item_id", cartItemId));

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting item: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {

        }

     

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(Connection.connect))
                {
                    connection.Open();

                    // Start a transaction to ensure all changes are atomic
                    OracleTransaction transaction = connection.BeginTransaction();

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        // Skip rows without valid data
                        if (row.Cells["Cart Item ID"].Value == null || row.Cells["Quantity"].Value == null)
                            continue;

                        // Extract values from the row
                        int cartItemId = Convert.ToInt32(row.Cells["Cart Item ID"].Value);
                        int quantity = Convert.ToInt32(row.Cells["Quantity"].Value);

                        if (quantity > 0)
                        {
                            // Update the quantity in the database
                            string updateQuery = @"
                        UPDATE cart_items
                        SET quantity = :quantity
                        WHERE cart_item_id = :cart_item_id";

                            using (OracleCommand updateCommand = new OracleCommand(updateQuery, connection))
                            {
                                updateCommand.Transaction = transaction; // Set the transaction
                                updateCommand.Parameters.Add(new OracleParameter("quantity", quantity));
                                updateCommand.Parameters.Add(new OracleParameter("cart_item_id", cartItemId));
                                updateCommand.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // Delete the cart item if quantity is 0
                            string deleteQuery = @"
                        DELETE FROM cart_items
                        WHERE cart_item_id = :cart_item_id";

                            using (OracleCommand deleteCommand = new OracleCommand(deleteQuery, connection))
                            {
                                deleteCommand.Transaction = transaction; // Set the transaction
                                deleteCommand.Parameters.Add(new OracleParameter("cart_item_id", cartItemId));
                                deleteCommand.ExecuteNonQuery();
                            }
                        }
                    }

                    // Commit all changes in the transaction
                    transaction.Commit();

                    MessageBox.Show("Cart items updated successfully.", "Update Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Reload the cart items to reflect changes
                    LoadCartItems();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating cart items: {ex.Message}", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Check_out check_Out = new Check_out();
            check_Out.Show();
            this.Hide();
        }
    }



}







