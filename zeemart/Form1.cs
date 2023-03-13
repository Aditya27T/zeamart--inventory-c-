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
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace zeemart
{
    public partial class Form1 : Form
    {
        MySqlConnection con = new MySqlConnection("server=localhost;user=root;password=admin1234;database=zeemart");
        MySqlCommand cmd;
        MySqlDataReader dr;
        DataTable dt = new DataTable();
        int i = 0;

        public Form1()
        {
            InitializeComponent();
        }

        public DataTable getAll()
        {
            dt.Reset();
            dt = new DataTable();
            con.Open();
            cmd = new MySqlCommand("select * from product", con);
            dr = cmd.ExecuteReader();
            dt.Load(dr);
            con.Close();
            return dt;
        }

        public void id_Increment()
        {
            MySqlScript script = new MySqlScript(con, "SET  @id := 0;\r\nUPDATE product SET id = @id := (@id+1);\r\nALTER TABLE `product` AUTO_INCREMENT = 1;");
            script.Execute();
        }
        

        public void resetdata_Table()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = getAll();
            dataGridView1.RowTemplate.Height = 80;

            id.DataPropertyName = "id";
            product.DataPropertyName = "product";
            quantity.DataPropertyName = "qty";
            price.DataPropertyName = "price";
            Column5.DataPropertyName = "image";
            edit.DataPropertyName = "edit";
            delete.DataPropertyName = "delete";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MemoryStream ms = new MemoryStream();
            pictureBox1.Image.Save(ms, pictureBox1.Image.RawFormat);
            byte[] img = ms.ToArray();


            if ((textBox1.Text == string.Empty | textBox2.Text == string.Empty | textBox3.Text == string.Empty ))
            {
                MessageBox.Show("Please fill all fields");
                return;
            }
            else
            {
                try
                {
                    id_Increment();
                    cmd = new MySqlCommand("insert into product (product,qty,price,image) values (@product,@qty,@price,@image)", con);
                    con.Open();
                    cmd.Parameters.AddWithValue("@product", textBox1.Text);
                    cmd.Parameters.AddWithValue("@qty", textBox2.Text);
                    cmd.Parameters.AddWithValue("@price", textBox3.Text);
                    cmd.Parameters.AddWithValue("@image",img );
                    cmd.ExecuteNonQuery();
                    con.Close();
                    MessageBox.Show("Product added successfully");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    con.Close();
                    resetdata_Table();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog openimage = new OpenFileDialog();
            openimage.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
            if (openimage.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = new Bitmap(openimage.FileName);
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MemoryStream ms = new MemoryStream();
            pictureBox1.Image.Save(ms, pictureBox1.Image.RawFormat);
            byte[] img = ms.ToArray();


            if ((textBox10.Text == string.Empty | textBox9.Text == string.Empty | textBox8.Text == string.Empty | textBox7.Text == string.Empty))
            {
                MessageBox.Show("Please fill all fields");
                return;
            }
            else
            {
                try
                {
                    cmd = new MySqlCommand("update product set product=@product,qty=@qty,price=@price,image=@image where id=@id", con);
                    con.Open();
                    cmd.Parameters.AddWithValue("@id", textBox10.Text);
                    cmd.Parameters.AddWithValue("@product", textBox9.Text);
                    cmd.Parameters.AddWithValue("@qty", textBox8.Text);
                    cmd.Parameters.AddWithValue("@price", textBox7.Text);
                    cmd.Parameters.AddWithValue("@image", img);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Product updated successfully");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    con.Close();
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 5)
            {
                string id = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                cmd = new MySqlCommand("select * from product where id=@id", con);
                con.Open();
                cmd.Parameters.AddWithValue("@id", id);
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    textBox10.Text = dr["id"].ToString();
                    textBox9.Text = dr["product"].ToString();
                    textBox8.Text = dr["qty"].ToString();
                    textBox7.Text = dr["price"].ToString();
                    byte[] img = (byte[])(dr["image"]);

                    con.Close();
                }
                
            }
            else if (e.ColumnIndex == 6)
            {
                string id = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete this product?", "Delete", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    try
                    {
                        id_Increment();
                        cmd = new MySqlCommand("delete from product where id=@id", con);
                        con.Open();
                        cmd.Parameters.AddWithValue("@id", id);
                        i = cmd.ExecuteNonQuery();
                        /*MySqlScript script = new MySqlScript(con, "ALTER TABLE product AUTO_INCREMENT = 1");
                        script.Execute();*/
                        
                        if (i > 0)
                        {
                            MessageBox.Show("Product deleted successfully");
                        }
                        else
                        {
                            MessageBox.Show("Error");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    con.Close();
                }
                else if (dialogResult == DialogResult.No)
                {
                    return;
                }
            }
            resetdata_Table();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            resetdata_Table();
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            MySqlDataAdapter ad = new MySqlDataAdapter("select * from product where product like '" + textBox11.Text + "%'", con);
            ad.Fill(dt);
            dataGridView1.DataSource = dt;
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            dataGridView1.SelectAll();
            DataObject copydata = dataGridView1.GetClipboardContent();
            if (copydata != null) Clipboard.SetDataObject(copydata);
            Microsoft.Office.Interop.Excel.Application xlapp = new Microsoft.Office.Interop.Excel.Application();
            xlapp.Visible = true;
            Microsoft.Office.Interop.Excel.Workbook xlWbook;
            Microsoft.Office.Interop.Excel.Worksheet xlsheet;
            object miseddata = System.Reflection.Missing.Value;
            xlWbook = xlapp.Workbooks.Add(miseddata);

            xlsheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWbook.Worksheets.get_Item(1);
            Microsoft.Office.Interop.Excel.Range xlr = (Microsoft.Office.Interop.Excel.Range)xlsheet.Cells[1, 1];
            xlr.Select();

            xlsheet.PasteSpecial(xlr, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, true);

        }
    }
}
