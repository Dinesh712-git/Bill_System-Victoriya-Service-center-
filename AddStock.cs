using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using MySql.Data.MySqlClient;

namespace service
{
    public partial class AddStock : Form
    {
        MySqlConnection con = new MySqlConnection(
        "server=localhost;userid = root; password=;database=service;  ");
        int BAG_Count = 0;
        int qtyCount = 0;

        
        public AddStock()
        {
            InitializeComponent();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                DateTime currentDate = DateTime.Now;

                MySqlCommand sm = new MySqlCommand("INSERT INTO tbl_item (code, name, price, sell_price, quntity, profit, update_date) " +
                                                  "VALUES (@code, @name, @price, @sell_price, @quntity, @profit ,@update_date)", con);

                sm.Parameters.AddWithValue("@code", txboxItemCode.Text);
                sm.Parameters.AddWithValue("@name", txboxItemName.Text);
                sm.Parameters.AddWithValue("@price", Convert.ToDouble(txboxPrice.Text));
                sm.Parameters.AddWithValue("@sell_price", Convert.ToDouble(txboxSellPrice.Text));
                sm.Parameters.AddWithValue("@quntity", Convert.ToInt32(txboxQut.Text));
                int qnt = Convert.ToInt32(txboxQut.Text);
                double profit = (Convert.ToDouble(txboxSellPrice.Text) - Convert.ToDouble(txboxPrice.Text)) * qnt;
                sm.Parameters.AddWithValue("@profit", profit);
                sm.Parameters.AddWithValue("@update_date", currentDate.ToString("yyyy-MM-dd"));

                sm.ExecuteNonQuery();

                MessageBox.Show("Item added successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
              
               this.clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con.Close();
                this.view_data();
            }
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

        private void AddStock_Load(object sender, EventArgs e)
        {
            DateTime currentDate = DateTime.Now;
            lblDate.Text = currentDate.ToString("yyyy-MM-dd");
            view_data();
        }

        public void clear()
        {
            txboxItemName.Text = null;
            txboxPrice.Text = null;
            txboxQut.Text = null;
            txboxSellPrice.Text = null;
            txboxItemCode.Text = null;

        }

        private void btnbagUpdate_Click(object sender, EventArgs e)
        {
            try
            {

                con.Open();
                DateTime currentDate = DateTime.Now;
                string itemNameToUpdate = txboxItemCode.Text;

                // Check if the item with the given name exists
                using (MySqlCommand checkCmd = new MySqlCommand("SELECT COUNT(*) FROM tbl_item WHERE code = @code", con))
                {
                    checkCmd.Parameters.AddWithValue("@code", itemNameToUpdate);
                    int itemCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (itemCount == 0)
                    {
                        MessageBox.Show("Item not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                // Perform the update
                using (MySqlCommand updateCmd = new MySqlCommand("UPDATE tbl_item SET price = @price, sell_price = @sell_price, quntity = @quntity, update_date = @update_date, name= @name, profit=@profit WHERE code = @code", con))
                {
                    updateCmd.Parameters.AddWithValue("@code", itemNameToUpdate);
                    updateCmd.Parameters.AddWithValue("@name", txboxItemName.Text);
                    updateCmd.Parameters.AddWithValue("@price", Convert.ToDouble(txboxPrice.Text));
                    updateCmd.Parameters.AddWithValue("@sell_price", Convert.ToDouble(txboxSellPrice.Text));
                    bagCountUpdate();
                    updateCmd.Parameters.AddWithValue("@quntity", qtyCount);

                    double profit = (Convert.ToDouble(txboxSellPrice.Text) - Convert.ToDouble(txboxPrice.Text)) * qtyCount;
                    updateCmd.Parameters.AddWithValue("@profit", profit);
                    updateCmd.Parameters.AddWithValue("@update_date", currentDate.ToString("yyyy-MM-dd"));

                    updateCmd.ExecuteNonQuery();
                }


                MessageBox.Show("Item updated successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
              
                this.clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con.Close();
                this.view_data();
            }
        }

        //bag count
        public void bagCountUpdate()
        {
            try
            {

                MySqlCommand countCommand = new MySqlCommand("SELECT SUM(quntity) FROM tbl_item WHERE code = @code", con);
                countCommand.Parameters.AddWithValue("@code", txboxItemCode.Text);
                int qutCount = Convert.ToInt32(txboxQut.Text);
                int rowCount = Convert.ToInt32(countCommand.ExecuteScalar());
                qtyCount = rowCount + qutCount;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con.Close();
                con.Open();
            }


        }

      
    }
}
