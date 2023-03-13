using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace zeemart
{
    public partial class Login : Form
    {
        MySqlConnection con = new MySqlConnection("server=localhost;user=root;password=admin1234;database=zeemart");
        MySqlCommand cmd;
        MySqlDataReader dr;
        public Login()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == string.Empty || textBox2.Text == string.Empty)
            {
                MessageBox.Show("Please enter username and password");
            }
            else
            {
                con.Open();
                cmd = new MySqlCommand("select * from user where username='" + textBox1.Text + "' and password='" + textBox2.Text + "'", con);
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    Form1 f1 = new Form1();
                    f1.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Incorrect Username and Password");
                }
                con.Close();
            }
        }
    }
}
