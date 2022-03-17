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
        FormHome formHome;
        public SearchForm(FormHome home)
        {
            formHome = home;
            InitializeComponent();
            BackColor = Properties.Settings.Default.Menu;
        }

        private void SearchForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
        }
        bool filterRoom = false;
        bool filterDate = false;
        bool bothFilters = false;
        string filterText;
        enum Filter
        {
            Room,
            Date,
            Both,
            None
        }
        Filter currientFulter;
        int room;

        private void TurnOnFilter(object sender, EventArgs e)
        {
            Control c = sender as Control;
            switch (c.Tag)
            {
                case "Room":
                    currientFulter = Filter.Room;
                    radioButton1.Visible = !radioButton1.Visible;
                    radioButton2.Visible = !radioButton2.Visible;
                    radioButton3.Visible = !radioButton3.Visible;
                    radioButton4.Visible = !radioButton4.Visible;
                    break;

                case "Date":
                    currientFulter = Filter.Date;
                    dateTimePicker1.Visible = !dateTimePicker1.Visible;
                    dateTimePicker2.Visible = !dateTimePicker2.Visible;
                    label3.Visible = !label3.Visible;
                    break;

            }
            bothFilters = false;
            if(filterDate && filterRoom)
            {
                currientFulter = Filter.Both;
                filterRoom = false;
                filterDate = false;
                bothFilters = true;
            }
        }

        private void DefaultButton_Click(object sender, EventArgs e)
        {
            formHome.LoadTable();
            filterText = "";
        }
        private void FilterButton_Click(object sender, EventArgs e)
        {
            //SELECT * FROM [Session] WHERE ([Occupancy] BETWEEN '20' AND '60') AND [RoomID] = '1'
            // Только один параметр | SELECT * FROM [Session] WHERE @text
            // Два и более | SELECT * FROM [Session] WHERE (@text) AND () AND ()

            //SELECT * FROM [Session] WHERE ([Date] BETWEEN '2022-01-01' AND '2022-03-1') AND [RoomID] = '1'
            filterText = "";
            if (currientFulter == Filter.Both)
            {
                filterText = $"SELECT * FROM [Session] WHERE ([Date] " +
                             $"BETWEEN '{dateTimePicker1.Value.ToShortDateString()}' " +
                             $"AND '{dateTimePicker2.Value.ToShortDateString()}' " +
                             $"AND [RoomID] = '{room}'";
            }
            if (currientFulter == Filter.Room)
            {
                filterText += $"SELECT * FROM [Session] WHERE " +
                              $"[RoomID] = '{room}'";
            }
            if (currientFulter == Filter.Date)
            {
                filterText += $"SELECT * FROM [Session] WHERE" +
                              $"[Date] BETWEEN '{dateTimePicker1.Value.ToShortDateString()}' " +
                              $"AND '{dateTimePicker2.Value.ToShortDateString()}'";
            }

            DataSet set = new DataSet();
            if (!String.IsNullOrEmpty(filterText))
            {
                if (room != 0)
                {
                    set = Filtration(filterText);
                    formHome.sessionTable.DataSource = set.Tables[0];
                }
            }
            else   
                MessageBox.Show($"{filterText} is empty!");

            //if (filterOccu)
            //{
            //    DataSet data = new DataSet();
            //    SqlDataAdapter adapter = new SqlDataAdapter("session_occupancy_filter", connectionString);
            //    SqlParameter parameterV1 = new SqlParameter
            //    {
            //        ParameterName = "@VALUE1",
            //        Value = numericUpDown1.Value
            //    };
            //    SqlParameter parameterV2 = new SqlParameter
            //    {
            //        ParameterName = "@VALUE2",
            //        Value = numericUpDown2.Value
            //    };
            //    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            //    adapter.SelectCommand.Parameters.Add(parameterV1);
            //    adapter.SelectCommand.Parameters.Add(parameterV2);
            //    adapter.Fill(data);
            //    formHome.sessionTable.DataSource = data.Tables[0];

            //}
        }

        private static DataSet Filtration(string filterText)
        {
            DataSet set = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(filterText,Properties.Settings.Default.BaseConnectionString);
            adapter.Fill(set);
            return set;
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            formHome.sessionTable.ClearSelection();
            int rows = formHome.sessionTable.RowCount;
            int columns = formHome.sessionTable.ColumnCount;
            string searchText = searchBox.Text;
            formHome.sessionTable.MultiSelect = true;
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                {
                    if (formHome.sessionTable.Rows[i].Cells[j].Value != null)
                        if (formHome.sessionTable.Rows[i].Cells[j].Value.ToString().Contains(searchText))
                        {
                            formHome.sessionTable.Rows[i].Selected = true;
                            return;
                        }
                }
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            room = Convert.ToInt32((sender as RadioButton).Text);
        }
    }
}
