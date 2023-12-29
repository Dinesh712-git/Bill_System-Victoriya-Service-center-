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
using System.Data;

namespace service
{
    public partial class Print : Form
    {
        MySqlConnection con = new MySqlConnection(
     "server=localhost;userid = root; password=;database=service;  ");
        string name;
        private DataTable printData;
        int Bill_Count = 0;
        public Print(DataTable dt)
        {
            InitializeComponent();
            printData = dt;
        }

        private void Print_Load(object sender, EventArgs e)
        {

            this.billCount();
            int generatedCode = Bill_Count;
            DateTime currentDate = DateTime.Now;

            List<string> dataList = new List<string>();
            richTextBox1.Clear();
            richTextBox1.Text += "-------------------------------------------------------\n";
            richTextBox1.Text += "         VICTORIYA CLEANING PARK                       \n";
            richTextBox1.Text += "                         YAMAHA                        \n";
            richTextBox1.Text += "                  287/2 Kandy Road,                    \n";
            richTextBox1.Text += "                          Millawa,                     \n";
            richTextBox1.Text += "                       Kurunegala                     \n";
            richTextBox1.Text += "                   Reg No : 20/1662                   \n";
            richTextBox1.Text += " Hot Line: 071 2823780  Tel: 0767823780      \n";
            richTextBox1.Text += "-------------------------------------------------------\n";
            richTextBox1.Text += $"  BILL NO : {generatedCode}         DATE : {currentDate.ToString("yyyy-MM-dd")}   \n";
            richTextBox1.Text += "-------------------------------------------------------\n";

            // Iterate through the rows of the DataTable and add product names to the RichTextBox
            double subtot = 0;
            foreach (DataRow row in printData.Rows)
            {
                string name = "";
                string productName = row["name"].ToString();

                con.Open();

                // Assuming tables are named tbl_bags, tbl_others, tbl_slippers
                string[] tableNames = { "tbl_item" };

                foreach (string tableName in tableNames)
                {
                    MySqlCommand selectCommand = new MySqlCommand($"SELECT sell_price, name FROM {tableName} WHERE code = @code", con);
                    selectCommand.Parameters.AddWithValue("@code", productName);

                    using (MySqlDataReader reader = selectCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Item found in the current table
                            name = reader["name"].ToString();

                            // Set the price to txboxPrice
                            //  txboxPrice.Text = price.ToString();

                            // Break out of the loop since we found the item
                            break;
                        }
                        // If the item is not found in the current table, continue to the next table
                    }
                }

                con.Close();




                string price = row["price"].ToString();
                string qut = row["quntity"].ToString();
                string tot = row["totat"].ToString();
                subtot = subtot + Convert.ToDouble(row["totat"]);
                richTextBox1.Text += $"{name,-30} {price,-8} * {qut,-5} {tot,-10}\n "; // Adjust the format as needed
            }

            richTextBox1.Text += "-------------------------------------------------------\n";
            richTextBox1.Text += $"    SUB TOTAL                   {subtot,-20:F2}          \n";
            richTextBox1.Text += "-------------------------------------------------------\n";
            richTextBox1.Text += $"  No Of Items : {printData.Rows.Count,-46}\n"; // Assuming you want to display the total number of items
            richTextBox1.Text += "-------------------------------------------------------\n";
            richTextBox1.Text += "                  Authorized Dealers                   \n";
            richTextBox1.Text += "                 Service & SpareParts                  \n";
            richTextBox1.Text += "                All Indian Bike Service                \n";
            richTextBox1.Text += "       Open Hours Friday To Wednsday                   \n";
            richTextBox1.Text += "-------------------------------------------------------\n";
            richTextBox1.Text += "-------------------------------------------------------\n";
            richTextBox1.Text += "                Thank You Come Again                   \n";
            richTextBox1.Text += "-------------------------------------------------------\n";
            richTextBox1.Text += "  System By Dinesh Mobile: 070 4544527              \n";

            print();
            this.Close();
        }
        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            e.Graphics.DrawString(richTextBox1.Text, new Font("Microsoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, new Point(10, 10));
        }

        public void print()
        {  // Set custom page size
            printDocument1.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("Custom", (int)(4 * 100), 0); // Width and Height in hundredths of an inch

            // Optional: Set other print settings, e.g., margins
            printDocument1.DefaultPageSettings.Margins.Left = 50;
            printDocument1.DefaultPageSettings.Margins.Right = 50;
            printDocument1.DefaultPageSettings.Margins.Top = 50;
            printDocument1.DefaultPageSettings.Margins.Bottom = 50;

            // printPreviewDialog1.Document = printDocument1;
            // printPreviewDialog1.ShowDialog();
            printDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(printDocument1_PrintPage);

            // Print without showing the print preview dialog
            printDocument1.Print();
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
                con.Close();

            }


        }
    }
}
