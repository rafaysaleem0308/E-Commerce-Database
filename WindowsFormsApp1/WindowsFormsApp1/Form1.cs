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
    public partial class Form1 : Form
    {
        public static Form1 Instance;
        public Form1()
        {
            InitializeComponent();
            Instance = this;
        }

        private void button1_Click(object sender, EventArgs e)
        {   this.Hide();
            Login_Page login_Page = new Login_Page();
            login_Page.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();

            Sign_up_Page sign_Up_Page = new Sign_up_Page();
            sign_Up_Page.Show();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
