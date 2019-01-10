using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBtoFile
{
    public partial class Form1 : Form
    {
        
        private SqlConnection connection = null;
        private List<Product> products = null;
        public Form1()
        {
            InitializeComponent();
            var connectionString = @"Data Source=localhost\\sqlexpress;Initial Catalog = MMABooks;Integrated Security = True";
            connection = new SqlConnection(connectionString);
        }
        

        private void button1_Click(object sender, EventArgs e)
        {
            var Selectstatement = textBox1.Text.Trim();

            try
            {
                SqlCommand selectCommand = new SqlCommand(Selectstatement, connection);
                selectCommand.Parameters.AddWithValue("@p", textBox2.Text);
                connection.Open();

                SqlDataReader reader = selectCommand.ExecuteReader();



                products = new List<Product>();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        products.Add(new Product()
                        {
                            ProductCode = reader.GetString(0),
                            Description = reader.GetString(1),
                            UnitPrice = reader.GetDecimal(2)
                        });
                    }


                }

                var numOfRows = reader.HasRows ? products.Count : 0;
                label4.Text = $"{numOfRows.ToString()} rows added";


            }
            catch (Exception ex)
            {
                MessageBox.Show("SQL Server found an error", "sql error");
                throw;
            }
        }
        public class Product
        {
            //down here
            public string ProductCode { get; set; }
            public string Description { get; set; }
            public Decimal UnitPrice { get; set; }
        }
       
        public bool saveToFile(List<Product> products)
        {
            try
            {
                if (!Directory.Exists(@"C:\EXAM3"))
                {
                    Directory.CreateDirectory(@"C:\EXAM3");
                }

                if (File.Exists(@"C:\EXAM3\Products.txt"))
                {
                    File.WriteAllText(@"C:\EXAM3\Products.txt", String.Empty);
                }
                else
                {
                    File.Create(@"C:\EXAM3\Products.txt");
                }

                var content = products
                    .Select(x => $"{x.ProductCode}#{x.Description}#{x.UnitPrice.ToString()} \r\n").ToList();


                File.WriteAllText(@"C:\EXAM3\Products.txt", content.Any() ? String.Join("", content.ToArray()) : String.Empty);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (saveToFile(products))
            {
                MessageBox.Show($"{products.Count.ToString()} products are saved in \"C:\\EXAM3\\Products.txt", "File saved");
            }
            else
            {
                throw new Exception("File write error.");
            }
        }
    }
}
