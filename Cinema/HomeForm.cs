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

namespace Cinema
{
    public partial class FormHome : Form
    {
        public FormHome()
        {
            InitializeComponent();
            flowLayoutPanel1.BackColor =    Properties.Settings.Default.Header;
            dataGridView1.BackgroundColor = Properties.Settings.Default.GridBackground;
            tableLayoutPanel1.BackColor =   Properties.Settings.Default.Menu;
        }
        AddingForm addingForm;
        EditingForm editingForm;
        SearchForm searchForm;
        public DataGridViewRow selectedRow;
        string connectionString = Properties.Settings.Default.BaseConnectionString;
        string selectString = "SELECT * FROM [Session]";
        private void FormHome_Load(object sender, EventArgs e)
        {
            LoadTable();
        }
        public void LoadTable()
        {
            DataSet data = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(selectString, connectionString);
            adapter.Fill(data);
            dataGridView1.DataSource = data.Tables[0];
        }
        private void AddingButton_Click(object sender, EventArgs e)
        {
            addingForm = new AddingForm(this);
            addingForm.Owner = this;
            addingForm.Show();
        }
        private void EditingButton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows is null)
                MessageBox.Show("Error!");
            else 
            {
                selectedRow = dataGridView1.SelectedRows[0];
                editingForm = new EditingForm(this);
                editingForm.Owner = this;
                editingForm.ShowDialog();
            }
        }
        private void RemovingButton_Click(object sender, EventArgs e)
        {
            selectedRow = dataGridView1.SelectedRows[0];
            int id;
            if (selectedRow is null)
            {
                MessageBox.Show(text: "Вы не выбрали строку для удаления!", caption: "Удаление", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Stop);
                return;
            }
            id = (int)selectedRow.Cells[0].Value;
            RemoveRow(id);
        }
        private void ReportingButton_Click(object sender, EventArgs e)
        {

        }
        private void SearchFilterButton_Click(object sender, EventArgs e)
        {
            searchForm = new SearchForm(this);
            searchForm.Owner = this;
            searchForm.ShowDialog();
        }
        public void AddRow(DateTime date,int filmID, int occupancy, int roomID)
        {
            string requestString = @"INSERT INTO [dbo].[Session] ([FilmID], [Occupancy], [Date], [RoomID])" +
                                   @"VALUES (@film, @occ, @date, @room)";
            using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.BaseConnectionString))
            {
                connection.Open();

                SqlTransaction transaction = connection.BeginTransaction();
                SqlCommand addingCommand = new SqlCommand(requestString, connection);
                addingCommand.Parameters.Add("@film", SqlDbType.Int).Value = filmID;
                addingCommand.Parameters.Add("@occ", SqlDbType.Int).Value = occupancy;
                addingCommand.Parameters.Add("@date", SqlDbType.DateTime).Value = date;
                addingCommand.Parameters.Add("@room", SqlDbType.Int).Value = roomID;
                addingCommand.Transaction = transaction;
                try
                {
                    addingCommand.ExecuteNonQuery();
                    transaction.Commit();
                    MessageBox.Show("Данные добавлены в базу данных", caption: "Транзакция");
                    connection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, caption: "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    transaction.Rollback();
                    throw;
                }
            }
        }
        public void EditRow(DateTime date, int filmID, int occupancy, int roomID)
        {
            string requestString = @"UPDATE Session SET FilmID = @film, Occupancy = @occ, Date = @date, RoomID = @room WHERE Id = @id";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                SqlCommand sqlCommand = new SqlCommand(requestString, connection);
                sqlCommand.Parameters.Add("@film",SqlDbType.Int).Value = filmID;
                sqlCommand.Parameters.Add("@date",SqlDbType.DateTime).Value = date;
                sqlCommand.Parameters.Add("@room",SqlDbType.Int).Value = roomID;
                sqlCommand.Parameters.Add("@occ", SqlDbType.Int).Value = occupancy;
                sqlCommand.Parameters.Add("@id",SqlDbType.Int).Value = (int)selectedRow.Cells[0].Value;
                sqlCommand.Transaction = transaction;
                try
                {
                    sqlCommand.ExecuteNonQuery();
                    transaction.Commit();
                    MessageBox.Show("Данные изменены", caption: "Транзакция");
                    connection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, caption: "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    transaction.Rollback();
                    throw;
                }
            }
            //UPDATE Session SET FilemID = '', Occupancy = '', Date = '', RoomID = '' WHERE Id = ;
        }
        private void RemoveRow(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                string requestString = "DELETE FROM Session WHERE Id = @id";
                SqlCommand sqlCommand = new SqlCommand(requestString,connection);
                sqlCommand.Parameters.Add("@id", SqlDbType.Int).Value = id;
                sqlCommand.Transaction = transaction;
                try
                {
                    DialogResult result = MessageBox.Show(text: $"Вы действительно хотите удалить строку под номером {id}?",
                                    caption: "Удаление строки",
                                    buttons: MessageBoxButtons.YesNo,
                                    icon:MessageBoxIcon.Warning
                                    );
                    if(result == DialogResult.Yes)
                    {
                        sqlCommand.ExecuteNonQuery();
                        transaction.Commit();
                        MessageBox.Show("Данные изменены", caption: "Транзакция");
                        LoadTable();
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, caption: "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}
