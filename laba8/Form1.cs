using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace laba8
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string connectionString = @"Data Source=DESKTOP-VMK6SS9\MSVLAD;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;" +
"User ID=" + textBox1.Text + ";" +
"Password=" + textBox2.Text + ";";
            using (SqlConnection connection = new
            SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    Form2 form = new
                    Form2(connectionString);
                    form.Show();
                    this.Hide();
                    MessageBox.Show("Удалось успешно подключиться к " +
                    connection.Database);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }
        }
    }
}
