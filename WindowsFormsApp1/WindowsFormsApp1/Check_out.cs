using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
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
    public partial class Check_out : Form
    {
        public Check_out()
        {
            InitializeComponent();
            LoadCartItems(); // Load cart items when the form is loaded
        }

        


        private void LoadCartItems()
        {
            int userId = Sessional.UserId; // Get logged-in user ID

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

                    // Add a new column for the total price in the DataGridView
                    if (!dataTable.Columns.Contains("Total Price"))
                    {
                        dataTable.Columns.Add("Total Price", typeof(decimal));
                    }

                    // Calculate the total price for each row and add it to the "Total Price" column
                    foreach (DataRow row in dataTable.Rows)
                    {
                        decimal quantity = Convert.ToDecimal(row["Quantity"]);
                        decimal price = Convert.ToDecimal(row["Price"]);
                        row["Total Price"] = quantity * price;
                    }

                    // Bind the DataTable to the DataGridView
                    dataGridView1.DataSource = dataTable;

                    // Customize DataGridView appearance
                    CustomizeDataGridView();

                    // Calculate and display the overall total price
                    decimal totalPrice = 0;
                    foreach (DataRow row in dataTable.Rows)
                    {
                        totalPrice += Convert.ToDecimal(row["Total Price"]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CustomizeDataGridView()
        {
            // Set header style
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Black; // Black background for headers
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White; // White font color
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridView1.ColumnHeadersHeight = 40; // Increase header height for better readability
            dataGridView1.EnableHeadersVisualStyles = false;

            // Set alternating row colors for better readability
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245); // Light gray
            dataGridView1.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(144, 202, 249); // Light blue

            // Set default row style
            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dataGridView1.DefaultCellStyle.ForeColor = Color.FromArgb(33, 33, 33); // Dark gray text
            dataGridView1.DefaultCellStyle.BackColor = Color.White;
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(100, 181, 246); // Blue for selection
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;

            // Add padding to cells for better readability
            dataGridView1.DefaultCellStyle.Padding = new Padding(5);

            // Set grid style
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridView1.GridColor = Color.FromArgb(224, 224, 224); // Subtle gray gridlines

            // Remove row headers for a cleaner look (optional)
            dataGridView1.RowHeadersVisible = false;

            // Adjust row height for better alignment
            dataGridView1.RowTemplate.Height = 35;

            // Automatically resize columns to fill available space
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Optional: Add hover effect on rows
            dataGridView1.CellMouseEnter += DataGridView1_CellMouseEnter;
            dataGridView1.CellMouseLeave += DataGridView1_CellMouseLeave;
        }

        // Hover effect on rows
        private void DataGridView1_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(230, 240, 255); // Light blue
            }
        }

        private void DataGridView1_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Reset to alternating color or white
                dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = e.RowIndex % 2 == 0
                    ? Color.White
                    : Color.FromArgb(245, 245, 245); // Light gray
            }
        }





        private void button1_Click(object sender, EventArgs e)
        {
            int userId = Sessional.UserId; // Get logged-in user ID
            string paymentMethod = comboBox1.SelectedItem?.ToString(); // Get selected payment method

            // Validate inputs
            if (userId <= 0)
            {
                MessageBox.Show("Invalid user. Please log in again.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(paymentMethod))
            {
                MessageBox.Show("Please select a payment method.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (OracleConnection connection = new OracleConnection(Connection.connect))
                {
                    connection.Open();
                    using (OracleTransaction transaction = connection.BeginTransaction()) // Begin transaction
                    {
                        try
                        {
                            // Step 1: Insert into 'orders' table and get the generated order_id
                            string insertOrderQuery = @"
                        INSERT INTO orders (order_id, user_id, status, total_price)
                        VALUES (orders_seq.NEXTVAL, :user_id, 'Processing', 0)
                        RETURNING order_id INTO :order_id";

                            OracleCommand insertOrderCommand = new OracleCommand(insertOrderQuery, connection);
                            insertOrderCommand.Transaction = transaction; // Assign the transaction
                            insertOrderCommand.Parameters.Add(new OracleParameter("user_id", userId));
                            OracleParameter orderIdParameter = new OracleParameter("order_id", OracleDbType.Decimal)
                            {
                                Direction = ParameterDirection.Output
                            };
                            insertOrderCommand.Parameters.Add(orderIdParameter);
                            insertOrderCommand.ExecuteNonQuery();

                            // Convert OracleDecimal to .NET type
                            int orderId = ((OracleDecimal)orderIdParameter.Value).ToInt32();

                            // Step 2: Insert into 'order_items' table and calculate total price
                            decimal totalPrice = 0;

                            foreach (DataGridViewRow row in dataGridView1.Rows)
                            {
                                if (row.Cells["Cart Item ID"].Value == null || row.Cells["Quantity"].Value == null)
                                    continue;

                                int cartItemId = Convert.ToInt32(row.Cells["Cart Item ID"].Value);
                                int quantity = Convert.ToInt32(row.Cells["Quantity"].Value);
                                decimal price = Convert.ToDecimal(row.Cells["Price"].Value);
                                totalPrice += quantity * price;

                                // Insert into 'order_items' table
                                string insertOrderItemsQuery = @"
                            INSERT INTO order_items (order_item_id, order_id, product_id, quantity, price)
                            SELECT order_items_seq.NEXTVAL, :order_id, ci.product_id, :quantity, :price
                            FROM cart_items ci
                            WHERE ci.cart_item_id = :cart_item_id";

                                OracleCommand insertOrderItemsCommand = new OracleCommand(insertOrderItemsQuery, connection);
                                insertOrderItemsCommand.Transaction = transaction; // Assign the transaction
                                insertOrderItemsCommand.Parameters.Add(new OracleParameter("order_id", orderId));
                                insertOrderItemsCommand.Parameters.Add(new OracleParameter("quantity", quantity));
                                insertOrderItemsCommand.Parameters.Add(new OracleParameter("price", price));
                                insertOrderItemsCommand.Parameters.Add(new OracleParameter("cart_item_id", cartItemId));
                                insertOrderItemsCommand.ExecuteNonQuery();
                            }

                            // Step 3: Update total price in 'orders' table
                            string updateOrderQuery = @"
                        UPDATE orders
                        SET total_price = :total_price
                        WHERE order_id = :order_id";

                            OracleCommand updateOrderCommand = new OracleCommand(updateOrderQuery, connection);
                            updateOrderCommand.Transaction = transaction; // Assign the transaction
                            updateOrderCommand.Parameters.Add(new OracleParameter("total_price", totalPrice));
                            updateOrderCommand.Parameters.Add(new OracleParameter("order_id", orderId));
                            updateOrderCommand.ExecuteNonQuery();

                            // Step 4: Insert into 'payments' table with total price as amount
                            string insertPaymentQuery = @"
                        INSERT INTO payments (payment_id, order_id, amount, payment_method)
                        VALUES (payments_seq.NEXTVAL, :order_id, :amount, :payment_method)";

                            OracleCommand insertPaymentCommand = new OracleCommand(insertPaymentQuery, connection);
                            insertPaymentCommand.Transaction = transaction; // Assign the transaction
                            insertPaymentCommand.Parameters.Add(new OracleParameter("order_id", orderId));
                            insertPaymentCommand.Parameters.Add(new OracleParameter("amount", totalPrice));
                            insertPaymentCommand.Parameters.Add(new OracleParameter("payment_method", paymentMethod));
                            insertPaymentCommand.ExecuteNonQuery();

                            // Commit the transaction
                            transaction.Commit();
                            MessageBox.Show("Order placed successfully!", "Order Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Clear the cart
                            ClearCart(userId);
                            MessageBox.Show("Cart cleared successfully!", "Cart Cleared", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception innerEx)
                        {
                            transaction.Rollback(); // Rollback on error
                            MessageBox.Show("Transaction failed: " + innerEx.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Database error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Hide();
            Customer_Dashboard customer_Dashboard = new Customer_Dashboard();
            customer_Dashboard.Show();
        }



        // Calculate the total price for the order using the stored function
        private decimal CalculateOrderTotal(int orderId)
        {
            decimal total = 0;

            try
            {
                using (OracleConnection connection = new OracleConnection(Connection.connect))
                {
                    connection.Open();

                    string query = "SELECT calculate_order_total(:order_id) FROM dual";

                    OracleCommand command = new OracleCommand(query, connection);
                    command.Parameters.Add(new OracleParameter("order_id", orderId));

                    total = Convert.ToDecimal(command.ExecuteScalar());  // Ensure you handle null or invalid result
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error calculating total: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return total;
        }


        // Clear the cart after a successful order
        private void ClearCart(int userId)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(Connection.connect))
                {
                    connection.Open();

                    string clearCartQuery = @"
                        DELETE FROM cart_items
                        WHERE cart_id IN (SELECT cart_id FROM cart WHERE user_id = :user_id)";

                    OracleCommand command = new OracleCommand(clearCartQuery, connection);
                    command.Parameters.Add(new OracleParameter("user_id", userId));

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error clearing cart: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Check_out_Load(object sender, EventArgs e)
        {
            
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
