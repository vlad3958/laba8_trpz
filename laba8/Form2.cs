using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace laba8
{
    public partial class Form2 : Form
    {
        string connectionString;
        private DataTable dishCompositionTable = new DataTable();


        public Form2(string str)
        {
        InitializeComponent();
           
        connectionString = str;
            LoadData();
            LoadComboBoxes();
        }

        private void LoadData()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"SELECT DC.DishCompositionId, P.ProductName, D.DishName, DC.Quantity, DC.Measurment 
                                 FROM DishComposition DC
                                 JOIN Product P ON DC.ProductId = P.ProductId
                                 JOIN Dish D ON DC.DishId = D.DishId";

                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                dishCompositionTable.Clear();
                adapter.Fill(dishCompositionTable);
                gridDataView.DataSource = dishCompositionTable;
            }
        }

        private void LoadComboBoxes()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Заповнення ComboBox для продуктів
                string productQuery = "SELECT ProductId, ProductName FROM Product";
                SqlDataAdapter productAdapter = new SqlDataAdapter(productQuery, connection);
                DataTable products = new DataTable();
                productAdapter.Fill(products);

                productComboBox.DataSource = products;
                productComboBox.DisplayMember = "ProductName";
                productComboBox.ValueMember = "ProductId";

                // Заповнення ComboBox для страв
                string dishQuery = "SELECT DishId, DishName FROM Dish";
                SqlDataAdapter dishAdapter = new SqlDataAdapter(dishQuery, connection);
                DataTable dishes = new DataTable();
                dishAdapter.Fill(dishes);

                dishComboBox.DataSource = dishes;
                dishComboBox.DisplayMember = "DishName";
                dishComboBox.ValueMember = "DishId";
            }
        }

        private void AddRecord()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Отримати максимальний ID
                string getMaxIdQuery = "SELECT ISNULL(MAX(DishCompositionId), 0) + 1 FROM DishComposition";
                SqlCommand getMaxIdCommand = new SqlCommand(getMaxIdQuery, connection);
                int newId = (int)getMaxIdCommand.ExecuteScalar();

                // Додати новий запис
                string query = @"INSERT INTO DishComposition (DishCompositionId, ProductId, DishId, Quantity, Measurment) 
                         VALUES (@DishCompositionId, @ProductId, @DishId, @Quantity, @Measurment)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@DishCompositionId", newId);
                command.Parameters.AddWithValue("@ProductId", productComboBox.SelectedValue);
                command.Parameters.AddWithValue("@DishId", dishComboBox.SelectedValue);
                command.Parameters.AddWithValue("@Quantity", int.Parse(quantityTextBox.Text));
                command.Parameters.AddWithValue("@Measurment", measurmentTextBox.Text);

                command.ExecuteNonQuery();
                LoadData();
            }
        }


        private void UpdateRecord()
        {
            if (gridDataView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Будь ласка, виберіть рядок для оновлення.");
                return;
            }

            int id = int.Parse(gridDataView.SelectedRows[0].Cells["DishCompositionId"].Value.ToString());

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"UPDATE DishComposition 
                         SET ProductId = @ProductId, DishId = @DishId, Quantity = @Quantity, Measurment = @Measurment 
                         WHERE DishCompositionId = @DishCompositionId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@DishCompositionId", id);
                command.Parameters.AddWithValue("@ProductId", productComboBox.SelectedValue);
                command.Parameters.AddWithValue("@DishId", dishComboBox.SelectedValue);
                command.Parameters.AddWithValue("@Quantity", int.Parse(quantityTextBox.Text));
                command.Parameters.AddWithValue("@Measurment", measurmentTextBox.Text);

                connection.Open();
                command.ExecuteNonQuery();
                LoadData();
            }
        }



        private void DeleteRecord()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                int id = int.Parse(gridDataView.SelectedRows[0].Cells["DishCompositionId"].Value.ToString());

                string query = "DELETE FROM DishComposition WHERE DishCompositionId = @DishCompositionId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@DishCompositionId", id);

                connection.Open();
                command.ExecuteNonQuery();
                LoadData();
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            AddRecord();
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            UpdateRecord();
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            DeleteRecord();
        }

        private void buttonTransaction_Click_1(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))

                {
                    connection.Open();

                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            SqlCommand
 command = new SqlCommand("sp_GetEmployeeServedTables", connection, transaction);
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@StartDate", dateTimePicker1.Value);
                            command.Parameters.AddWithValue("@EndDate", dateTimePicker2.Value);

                            SqlDataAdapter
 adapter = new SqlDataAdapter(command);
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);

                            Form3 formResults = new Form3();
                            formResults.SetDataSource(dataTable);
                            formResults.Show();

                            transaction.Commit();
                            MessageBox.Show("Дані оновлено успішно");
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show("Виникла помилка: " + ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка підключення до бази даних: " + ex.Message);
            }
        }

       
    

    private void Form2_FormClosed(object sender, FormClosedEventArgs
        e)
        {
        Form form = Application.OpenForms[0];
        form.Close();
        }
    }
}
