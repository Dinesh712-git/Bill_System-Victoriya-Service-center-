using MySql.Data.MySqlClient;
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
    public partial class ClientBill : Form
    {
        MySqlConnection con = new MySqlConnection(
      "server=localhost;userid = root; password=;database=service;  ");
        int BAG_Count = 0;
        int qtyCount = 0;
        public ClientBill()
        {
            InitializeComponent();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Close();
        }

        private void ClientBill_Load(object sender, EventArgs e)
        {
            view_data();
        }

        //view table data
        public void view_data()
        {
            con.Open();
            // textBox1.Text = "you are now connect database";
            MySqlCommand sm = new MySqlCommand("select * from tbl_client_item", con);

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
            MySqlCommand sm = new MySqlCommand("select * from tbl_client_item WHERE bill_no = @bill_no OR nic=@nic OR date=@date", con);
            sm.Parameters.AddWithValue("@bill_no", txboxBillNo.Text);
            sm.Parameters.AddWithValue("@nic", txboxNic.Text);
            sm.Parameters.AddWithValue("@date", txBoxDate.Text);
            MySqlDataAdapter da = new MySqlDataAdapter(sm);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            con.Close();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            search();
        }
    }
}
