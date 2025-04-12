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
    //public class RoundedButton : Button
    //{
    //    protected override void OnPaint(PaintEventArgs pevent)
    //    {
    //        base.OnPaint(pevent);
    //        Graphics g = pevent.Graphics;
    //        Rectangle bounds = new Rectangle(0, 0, this.Width, this.Height);

    //        // Create rounded rectangle path
    //        System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
    //        int radius = 20; // Corner radius
    //        path.AddArc(bounds.Left, bounds.Top, radius, radius, 180, 90);
    //        path.AddArc(bounds.Right - radius, bounds.Top, radius, radius, 270, 90);
    //        path.AddArc(bounds.Right - radius, bounds.Bottom - radius, radius, radius, 0, 90);
    //        path.AddArc(bounds.Left, bounds.Bottom - radius, radius, radius, 90, 90);
    //        path.CloseAllFigures();

    //        this.Region = new Region(path);
    //    }
    //}

    public partial class Cashier : Form
    {
        public Cashier()
        {
            InitializeComponent();
            button1.MouseEnter += Button_MouseEnter;
            button1.MouseLeave += Button_MouseLeave;
        }
        private void Button_MouseEnter(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                btn.BackColor = Color.FromArgb(255, 153, 153); // Light pink
                btn.ForeColor = Color.White; // Text color on hover
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderColor = Color.DarkRed;
                btn.FlatAppearance.BorderSize = 1; // Add subtle border
            }
        }

        private void Button_MouseLeave(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                btn.BackColor = SystemColors.Control; // Default background color
                btn.ForeColor = SystemColors.ControlText; // Default text color
                btn.FlatStyle = FlatStyle.Standard;
            }
        }
        private void StyleDataGridView()
        {
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(238, 239, 249); // Alternate row color
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(55, 125, 255); // Highlight color
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.White; // Highlighted text color
            dataGridView1.DefaultCellStyle.BackColor = Color.White; // Default cell background color
            dataGridView1.DefaultCellStyle.Font = new Font("Calibri", 12); // Cell font
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(20, 25, 72); // Header background color
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White; // Header text color
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Calibri", 13, FontStyle.Bold); // Header font
            dataGridView1.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridView1.GridColor = Color.FromArgb(193, 199, 206); // Gridline color

            // Adjust column widths
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }
        // Handle Search Button Click
        //private void button1_Click(object sender, EventArgs e)
        //{
        //    if (comboBox1.SelectedItem == null)
        //    {
        //        MessageBox.Show("Please select a delivery user.");
        //        return;
        //    }

        //    ComboBoxItem selectedUser = (ComboBoxItem)comboBox1.SelectedItem;
        //    int? selectedOrderId = null;

        //    foreach (DataGridViewRow row in dataGridView1.Rows)
        //    {
        //        if (row.Cells["Select"] is DataGridViewCheckBoxCell cell && Convert.ToBoolean(cell.Value) == true)
        //        {
        //            selectedOrderId = Convert.ToInt32(row.Cells["order_id"].Value);
        //            break;
        //        }
        //    }

        //    if (selectedOrderId == null)
        //    {
        //        MessageBox.Show("Please select an order.");
        //        return;
        //    }

        //    AssignOrderToDelivery(selectedUser.Value, selectedOrderId.Value);
        //}
        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Please select a delivery user.");
                return;
            }

            ComboBoxItem selectedUser = (ComboBoxItem)comboBox1.SelectedItem;
            int? selectedOrderId = null;

            // Loop through rows to get the selected order
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["Select"] is DataGridViewCheckBoxCell cell && Convert.ToBoolean(cell.Value) == true)
                {
                    selectedOrderId = Convert.ToInt32(row.Cells["ORDER_ID"].Value);
                    break;
                }
            }

            // Check if an order is selected
            if (selectedOrderId == null)
            {
                MessageBox.Show("Please select an order.");
                return;
            }

            // Pass the selected delivery user ID and order ID to the function
            AssignOrderToDelivery(selectedUser.Value, selectedOrderId.Value);
        }

        private void AssignOrderToDelivery(int deliveryUserId, int orderId)
        {
            using (OracleConnection con = new OracleConnection(Connection.connect))
            {
                con.Open();

                try
                {
                    // Check if order exists and its status
                    string checkStatusQuery = @"
                SELECT status 
                FROM ORDERS
                WHERE ORDER_ID = :order_id";

                    using (OracleCommand checkCmd = new OracleCommand(checkStatusQuery, con))
                    {
                        checkCmd.Parameters.Add("ORDER_ID", OracleDbType.Int32).Value = orderId;

                        string status = checkCmd.ExecuteScalar() as string;
                        if (status == "Assigned to Delivery")
                        {
                            MessageBox.Show($"Order {orderId} is already assigned to a delivery user.");
                            LoadOrdersIntoDataGridView();
                            return;
                        }
                    }

                    // Update order status
                    string updateOrderQuery = @"
                UPDATE orders
                SET status = 'Assigned to Delivery'
                WHERE order_id = :order_id";

                    using (OracleCommand updateOrderCmd = new OracleCommand(updateOrderQuery, con))
                    {
                        updateOrderCmd.Parameters.Add("ORDER_ID", OracleDbType.Int32).Value = orderId;
                        updateOrderCmd.ExecuteNonQuery();
                    }

                    // Insert into delivery
                    string insertDeliveryQuery = @"
                INSERT INTO delivery (delivery_id, user_id, order_id)
                VALUES (delivery_seq.NEXTVAL, :delivery_user_id, :order_id)";

                    using (OracleCommand insertDeliveryCmd = new OracleCommand(insertDeliveryQuery, con))
                    {
                        insertDeliveryCmd.Parameters.Add("delivery_user_id", OracleDbType.Int32).Value = deliveryUserId;
                        insertDeliveryCmd.Parameters.Add("ORDER_ID", OracleDbType.Int32).Value = orderId;
                        insertDeliveryCmd.ExecuteNonQuery();
                    }

                    MessageBox.Show($"Order {orderId} assigned successfully.");
                    LoadOrdersIntoDataGridView();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            LoadOrdersIntoDataGridView();
        }




        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
     

        private void Cashier_Load(object sender, EventArgs e)
        {
            FillDeliveryComboBox();
            LoadOrdersIntoDataGridView();

        }
         private void FillDeliveryComboBox()
        {
            using (OracleConnection con = new OracleConnection(Connection.connect))
            {
                try
                {
                    con.Open();
                    string query = "SELECT user_id, username FROM users WHERE role = 'delivery'";
                    using (OracleCommand cmd = new OracleCommand(query, con))
                    {
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                comboBox1.Items.Add(new ComboBoxItem(reader["username"].ToString(), Convert.ToInt32(reader["user_id"])));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching delivery users: " + ex.Message);
                }
            }
        }
        private void LoadOrdersIntoDataGridView()
        {
            using (OracleConnection con = new OracleConnection(Connection.connect))
            {
                try
                {
                    con.Open();
                    string query = @"
SELECT o.order_id, o.user_id AS customer_id, o.order_date, o.status, o.total_price,
       oi.product_id, p.name AS product_name, oi.quantity, oi.price
FROM orders o
JOIN order_items oi ON o.order_id = oi.order_id
JOIN products p ON oi.product_id = p.product_id";

                    using (OracleDataAdapter adapter = new OracleDataAdapter(query, con))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        dataGridView1.DataSource = dt;

                        // Ensure Select column exists only once
                        if (dataGridView1.Columns["Select"] == null)
                        {
                            DataGridViewCheckBoxColumn selectColumn = new DataGridViewCheckBoxColumn
                            {
                                Name = "Select",
                                HeaderText = "Select",
                                Width = 50
                            };
                            dataGridView1.Columns.Insert(0, selectColumn);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading orders: " + ex.Message);
                }
            }

            StyleDataGridView();
        }

        public class ComboBoxItem
        {
            public string Text { get; set; }
            public int Value { get; set; }

            public ComboBoxItem(string text, int value)
            {
                Text = text;
                Value = value;
            }

            public override string ToString()
            {
                return Text;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Login_Page loginPage = new Login_Page();
            loginPage.Show();
        }
    }
}
