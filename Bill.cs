using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace service
{
    public partial class Bill : Form
    {
        MySqlConnection con = new MySqlConnection(
      "server=localhost;userid = root; password=;database=service;  ");
        int Bill_Count = 0;
        double Bill_Total = 0;
        double Bille = 0;
        double priceadd=0;
        public Bill()
        {
            InitializeComponent();
            txboxQuntity.TextChanged += txboxQuntity_TextChanged;
            txboxPrice.TextChanged += txboxPrice_TextChanged;
            TxboxCash.TextChanged += TxboxCash_TextChanged;
            txboxItemName.TextChanged += txboxItemName_TextChanged;

        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Close();
        }

        private void Bill_Load(object sender, EventArgs e)
        {
            view_data();
        }

        //view table data
        public void view_data()
        {
            con.Open();
            // textBox1.Text = "you are now connect database";
            MySqlCommand sm = new MySqlCommand("select * from tbl_bill_temp", con);

            MySqlDataAdapter da = new MySqlDataAdapter(sm);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            con.Close();
        }

        private void txboxQuntity_TextChanged(object sender, EventArgs e)
        {
            CalculateTotal();
        }

        private void txboxPrice_TextChanged(object sender, EventArgs e)
        {
            CalculateTotal();
        }

        private void CalculateTotal()
        {
            try
            {
                double qut = Convert.ToDouble(txboxQuntity.Text);
                double price = Convert.ToDouble(txboxPrice.Text);
                double total = price * qut;
                Bill_Total = Bill_Total + total;
                txboxTotal.Text = total.ToString();
            }
            catch (FormatException ex)
            {
                // Handle the case where the user enters non-numeric values
                // MessageBox.Show("Invalid input. Please enter valid numeric values for quantity and price.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txboxTotal.Text = string.Empty; // Clear the total text box in case of an error
            }
        }

        private void TxboxCash_TextChanged(object sender, EventArgs e)
        {
            CalculateBalance();
        }

        private void CalculateBalance()
        {
            try
            {
                double cash = Convert.ToDouble(TxboxCash.Text);

                double balance = cash - Bill_Total;
                // lblBillTotal.Text = Bill_Total.ToString();
                //  txboxTotalBills.Text = Bill_Total.ToString();
                txboxBalance.Text = balance.ToString();
            }
            catch (FormatException ex)
            {
                // Handle the case where the user enters non-numeric values
                // MessageBox.Show("Invalid input. Please enter valid numeric values for quantity and price.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txboxTotal.Text = string.Empty; // Clear the total text box in case of an error
            }
        }

        private void txboxItemName_TextChanged(object sender, EventArgs e)
        {
            // Call getItemDetails to fill txboxPrice based on the entered item name
            getItemDetails(txboxItemName.Text);
        }

        private void getItemDetails(string itemName)
        {
            try
            {
                con.Open();

                // Assuming tables are named tbl_bags, tbl_others, tbl_slippers
                string[] tableNames = { "tbl_item" };

                foreach (string tableName in tableNames)
                {
                    MySqlCommand selectCommand = new MySqlCommand($"SELECT sell_price, name FROM {tableName} WHERE code = @code", con);
                    selectCommand.Parameters.AddWithValue("@code", itemName);

                    using (MySqlDataReader reader = selectCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Item found in the current table
                            double price = Convert.ToDouble(reader["sell_price"]);

                            // Set the price to txboxPrice
                            txboxPrice.Text = price.ToString();

                            // Break out of the loop since we found the item
                            break;
                        }
                        // If the item is not found in the current table, continue to the next table
                    }
                }

                // If the loop completes without finding the item, you can handle it here
                // MessageBox.Show("Item not found in any table.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                //  MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con.Close();
                // con.Open();
            }
        }

        private void DeleteBillByBillNo()
        {
            try
            {
                con.Open();

                MySqlCommand deleteCommand = new MySqlCommand("DELETE FROM tbl_bill_temp", con);
                //  deleteCommand.Parameters.AddWithValue("@BillNo", billNo);

                int rowsAffected = deleteCommand.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    //   MessageBox.Show($"Bill deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"Bill with BillNo not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con.Close();
                // Refresh the data in your DataGridView or wherever you display the data
                view_data();
            }
        }


        private void insertData()
        {
            try
            {
                // con.Open();
                this.billCount();
                //   con.Open();
                DateTime currentDate = DateTime.Now;

                int generatedCode = Bill_Count + 1;

                MySqlCommand insertCommand = new MySqlCommand("INSERT INTO tbl_bill_main (bill_no, date, sale, client) " +
                                                    "VALUES (@bill_no, @date, @sale, @client)", con);


                insertCommand.Parameters.AddWithValue("@bill_no", generatedCode);
                insertCommand.Parameters.AddWithValue("@date", currentDate.ToString("yyyy-MM-dd"));
                insertCommand.Parameters.AddWithValue("@sale", Convert.ToDouble(txboxTotalBills.Text));
                insertCommand.Parameters.AddWithValue("@client", txboxClientNIC.Text);

                insertCommand.ExecuteNonQuery();

                // MessageBox.Show("Data inserted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con.Close();
                // If you want to perform any additional actions after the insert, you can do it here.
            }
        }


        //remove item by code
        private void remove_items(string itemcode)
        {
            try
            {
                // Assuming tables are named tbl_bags, tbl_others, tbl_slippers
                string[] tableNames = { "tbl_item" };

                foreach (string tableName in tableNames)
                {
                    // Use UPDATE instead of DELETE to decrement the quantity
                    MySqlCommand updateCommand = new MySqlCommand($"UPDATE {tableName} SET quntity = quntity - @quantity  WHERE code = @code", con);
                    updateCommand.Parameters.AddWithValue("@code", itemcode);

                    int quantityDecrement = int.Parse(txboxQuntity.Text);
                    updateCommand.Parameters.AddWithValue("@quantity", quantityDecrement);

                    // Execute the update query
                    updateCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Close the connection if it was opened
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        public void billCount()
        {
            try
            {
                con.Open();

                MySqlCommand countCommand = new MySqlCommand("SELECT COUNT(*) FROM tbl_bill_main", con);
                int rowCount = Convert.ToInt32(countCommand.ExecuteScalar());
                Bill_Count = rowCount;
                //MessageBox.Show($"Number of rows in tbl_slippers: {rowCount}", "Row Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // con.Close();

            }


        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {

                this.billCount();
                //   con.Open();
                DateTime currentDate = DateTime.Now;

                int generatedCode = Bill_Count + 1;

                MySqlCommand sm = new MySqlCommand("INSERT INTO tbl_bill_temp (bill_no, name, price, quntity, totat, profit, date, client) " +
                                                 "VALUES (@bill_no, @name, @price, @Quntity, @totat, @profit,@date,@client)", con);


                double qut = Convert.ToDouble(txboxQuntity.Text);
                double price = Convert.ToDouble(txboxPrice.Text);
                double total = price * qut;
                Bille = Bille + total;
                txboxTotal.Text = total.ToString();
                sm.Parameters.AddWithValue("@bill_no", generatedCode);
                sm.Parameters.AddWithValue("@name", txboxItemName.Text);
                
                sm.Parameters.AddWithValue("@price", Convert.ToDouble(txboxPrice.Text));
       
                sm.Parameters.AddWithValue("@quntity", Convert.ToDouble(txboxQuntity.Text));
                getrealprice(txboxItemName.Text);
                double p = Convert.ToDouble(txboxPrice.Text);
                int qt = Convert.ToInt32(txboxQuntity.Text);
                double profit = (p - priceadd) * qt;
                sm.Parameters.AddWithValue("@profit", profit);
                sm.Parameters.AddWithValue("@totat", Convert.ToDouble(txboxTotal.Text));
                sm.Parameters.AddWithValue("@date", currentDate.ToString("yyyy-MM-dd"));
           
                sm.Parameters.AddWithValue("@client", txboxClientNIC.Text);

                sm.ExecuteNonQuery();
                remove_items(txboxItemName.Text);

                // MessageBox.Show("Item added successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
             
                txboxTotalBills.Text = Convert.ToString(Bille);
                addclient();
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

        //get price 
        public void getrealprice(string name)
        {
            try
            {


                string[] tableNames = { "tbl_item" };

                foreach (string tableName in tableNames)
                {
                    MySqlCommand selectCommand = new MySqlCommand($"SELECT price, name FROM {tableName} WHERE code = @code", con);
                    selectCommand.Parameters.AddWithValue("@code", name);

                    using (MySqlDataReader reader = selectCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Item found in the current table
                            priceadd = Convert.ToDouble(reader["price"]);

                            // Set the price to txboxPrice
                          //  txboxPrice.Text = price.ToString();

                            // Break out of the loop since we found the item
                            break;
                        }
                        // If the item is not found in the current table, continue to the next table
                    }
                }

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

        public void clear()
        {
            txboxItemName.Text = null;
            txboxPrice.Text = null;
            txboxQuntity.Text = null;
            txboxTotal.Text = null;

        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            con.Open();
            MySqlCommand sm = new MySqlCommand("select * from tbl_bill_temp", con);
            MySqlDataAdapter da = new MySqlDataAdapter(sm);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();

            // Pass 'dt' to the Print constructor
            Print p = new Print(dt);
            insertData();
            DeleteBillByBillNo();
            p.ShowDialog();
            txboxBalance.Text = null;
            TxboxCash.Text = null;
            txboxTotalBills.Text = null;
            Bille = 0.0;
            this.Refresh();
        }


        //add to client table
        public void addclient()
        {
            try
            {
                this.billCount();
                //   con.Open();
                DateTime currentDate = DateTime.Now;

                int generatedCode = Bill_Count + 1;
                // con.Open();
      
                MySqlCommand sm1 = new MySqlCommand("INSERT INTO tbl_client_item (bill_no, nic, code, quntity, date) " +
                                                  "VALUES (@bill_no, @nic, @code, @quntity, @date)", con);

                sm1.Parameters.AddWithValue("@bill_no", generatedCode);
                sm1.Parameters.AddWithValue("@nic", txboxClientNIC.Text);
                sm1.Parameters.AddWithValue("@code", txboxItemName.Text);
              
                sm1.Parameters.AddWithValue("@quntity", Convert.ToInt32(txboxQuntity.Text));
               
                sm1.Parameters.AddWithValue("@date", currentDate.ToString("yyyy-MM-dd"));

                sm1.ExecuteNonQuery();

          

                //this.clear();
            }
            catch (Exception ex)
            {
           
            }
            finally
            {
                con.Close();
                con.Open();
            
            }
        }
    }
}
