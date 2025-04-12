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
    public partial class Delievery : Form
    {
        public Delievery()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //string connectionString = "User Id=your_user;Password=your_password;Data Source=your_data_source";

            using (OracleConnection connection = new OracleConnection(Connection.connect))
            {
                try
                {
                    connection.Open();

                    // Use Sessional.UserId for the current user's ID
                    int userId = Sessional.UserId;

                    // Fetch orders with product details and status
                    string getOrdersQuery = @"
                SELECT o.order_id, 
                       p.product_id, 
                       p.name AS product_name, 
                       o.status
                FROM orders o
                JOIN order_items oi ON o.order_id = oi.order_id
                JOIN products p ON oi.product_id = p.product_id
                WHERE o.user_id = :user_id";

                    OracleCommand ordersCommand = new OracleCommand(getOrdersQuery, connection);
                    ordersCommand.Parameters.Add(new OracleParameter("user_id", userId));

                    OracleDataAdapter adapter = new OracleDataAdapter(ordersCommand);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Bind results to DataGridView
                    dataGridView1.DataSource = dataTable;
                    AddRadioButtonColumn();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void AddRadioButtonColumn()
        {
            DataGridViewCheckBoxColumn radioButtonColumn = new DataGridViewCheckBoxColumn
            {
                Name = "Select",
                HeaderText = "Done",
                Width = 50,
                ReadOnly = false
            };

            // Add the column to the DataGridView if not already added
            if (!dataGridView1.Columns.Contains("Select"))
            {
                dataGridView1.Columns.Add(radioButtonColumn);
            }
        }



        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

     

        private void button1_Click(object sender, EventArgs e)
        {
            //string connectionString = "User Id=your_user;Password=your_password;Data Source=your_data_source";

            using (OracleConnection connection = new OracleConnection(Connection.connect))
            {
                try
                {
                    connection.Open();

                    // Find the selected row
                    DataGridViewRow selectedRow = null;
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (Convert.ToBoolean(row.Cells["Select"].Value) == true)
                        {
                            selectedRow = row;
                            break;
                        }
                    }

                    if (selectedRow != null)
                    {
                        int orderId = Convert.ToInt32(selectedRow.Cells["order_id"].Value);

                        // Update the order status to 'Delivered'
                        string updateStatusQuery = "UPDATE orders SET status = 'Delivered' WHERE order_id = :order_id";
                        OracleCommand updateCommand = new OracleCommand(updateStatusQuery, connection);
                        updateCommand.Parameters.Add(new OracleParameter("order_id", orderId));

                        int rowsAffected = updateCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Order status updated to 'Delivered'.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Refresh DataGridView to reflect the updated status
                            button2_Click(sender, e);
                        }
                        else
                        {
                            MessageBox.Show("Failed to update order status.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please select an order to update.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns["Select"].Index)
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Index != e.RowIndex)
                    {
                        row.Cells["Select"].Value = false; // Uncheck other rows
                    }
                }
            }

        }
    }
}
