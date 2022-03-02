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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Cinema
{
    public partial class SearchForm : Form
    {
        FormHome home;
        public SearchForm(FormHome formHome)
            {
                home = formHome;
                InitializeComponent();
                BackColor = Properties.Settings.Default.Menu;
            }

        private void SearchForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
        }
        string connectionString = Properties.Settings.Default.BaseConnectionString;
        bool filterOccu = false;
        bool filterRoom = false;
        bool filterDate = false;
        bool filterFilm = false;

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown2.Minimum = numericUpDown1.Value;
        }

        private void TurnOnFilter(object sender, EventArgs e)
        {
            Control c = sender as Control;
            switch (c.Tag)
            {
                case "Occupancy":
                    filterOccu = !filterOccu;
                    numericUpDown1.Visible = !numericUpDown1.Visible;
                    numericUpDown2.Visible = !numericUpDown2.Visible;
                    label2.Visible = !label2.Visible;
                    break;

                case "Room":
                    filterRoom = !filterRoom;
                    radioButton1.Visible = !radioButton1.Visible;
                    radioButton2.Visible = !radioButton2.Visible;
                    radioButton3.Visible = !radioButton3.Visible;
                    radioButton4.Visible = !radioButton4.Visible;
                    break;

                case "Date":
                    filterDate = !filterDate;
                    dateTimePicker1.Visible = !dateTimePicker1.Visible;
                    dateTimePicker2.Visible = !dateTimePicker2.Visible;
                    label3.Visible = !label3.Visible;
                    break;

                case "Film":
                    filterFilm = !filterFilm;
                    filmsComboBox.Visible = !filmsComboBox.Visible;
                    break;

            }
            if ((filterDate || filterFilm || filterOccu || filterRoom))
            {
                button1.Enabled = !button1.Enabled;
                defaultButton.Enabled = !defaultButton.Enabled;
            }
            else
            {
                button1.Enabled = !button1.Enabled;
                defaultButton.Enabled = !defaultButton.Enabled;
            }
        }

        private void DefaultButton_Click(object sender, EventArgs e)
        {

            home.LoadTable();
        }
        DataSet data;
        SqlDataAdapter adapter;
        private void FilterButton_Click(object sender, EventArgs e)
        {
            if (filterOccu)
            {
                DataSet data = new DataSet();


                SqlDataAdapter adapter = new SqlDataAdapter("session_occupancy_filter", connectionString);
                SqlParameter parameterV1 = new SqlParameter
                {
                    ParameterName = "@VALUE1",
                    Value = numericUpDown1.Value
                };
                SqlParameter parameterV2 = new SqlParameter
                {
                    ParameterName = "@VALUE2",
                    Value = numericUpDown2.Value
                };
                adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                adapter.SelectCommand.Parameters.Add(parameterV1);
                adapter.SelectCommand.Parameters.Add(parameterV2);
                adapter.Fill(data);
                home.dataGridView1.DataSource = data.Tables[0];

            }
        }

        private void searchBox_TextChanged(object sender, EventArgs e)
        {
            int rows = home.dataGridView1.RowCount;
int columns = home.dataGridView1.ColumnCount;
            string searchText = $"Id Like '%{searchBox.Text.Trim()}%'";

            DataRowCollection collection = ((DataTable)home.dataGridView1.DataSource).Rows;
            DataRow[] dataRows = ((DataTable)home.dataGridView1.DataSource).Select(searchText);
            int rowIndex = collection.IndexOf(dataRows[0]);
            home.dataGridView1.CurrentCell = home.dataGridView1[0, rowIndex];

            //for (int i = 0; i < rows; i++)
            //{
            //    for (int j = 0; j < columns; j++)
            //    {
            //        home.dataGridView1.Rows[i].Selected = false;
            //        if (home.dataGridView1.Rows[i].Cells[j].Value != null)
            //            if (home.dataGridView1.Rows[i].Cells[j].Value.ToString().Contains(searchText))
            //            {
            //                home.dataGridView1.Rows[i].Selected = true;
            //                break;
            //            }
            //    }
            //}
        }
    }
}
