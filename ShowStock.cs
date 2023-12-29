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

namespace service
{
    public partial class ShowStock : Form
    {
        MySqlConnection con = new MySqlConnection(
       "server=localhost;userid = root; password=;database=service;  ");
        int BAG_Count = 0;
        int qtyCount = 0;

        public ShowStock()
        {
            InitializeComponent();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Close();
        }

        private void ShowStock_Load(object sender, EventArgs e)
        {
            DateTime currentDate = DateTime.Now;
            lblDate.Text = currentDate.ToString("yyyy-MM-dd");
            itemCount();
            view_data();
        }

        //view table data
        public void view_data()
        {
            con.Open();
            // textBox1.Text = "you are now connect database";
            MySqlCommand sm = new MySqlCommand("select * from tbl_item", con);

            MySqlDataAdapter da = new MySqlDataAdapter(sm);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            con.Close();
        }

        //search btn
        public void search()
        {
            con.Open();
            // textBox1.Text = "you are now connect database";
            MySqlCommand sm = new MySqlCommand("select * from tbl_item WHERE code = @code", con);
            sm.Parameters.AddWithValue("@code", txboxItem.Text);
         
            MySqlDataAdapter da = new MySqlDataAdapter(sm);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            con.Close();
        }

        //ITEM count
        public void itemCount()
        {
            try
            {
                con.Open();
                MySqlCommand countCommand = new MySqlCommand("SELECT SUM(quntity) FROM tbl_item", con);
                int rowCount = Convert.ToInt32(countCommand.ExecuteScalar());
                lblSlippersCount.Text = rowCount.ToString();
                //MessageBox.Show($"Number of rows in tbl_slippers: {rowCount}", "Row Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con.Close();
            }


        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            search();
        }
    }
}
