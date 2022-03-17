using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cinema
{
    public partial class EditingForm : Form
    {
        FormHome formHome;
        public EditingForm(FormHome home)
        {
            formHome = home;
            InitializeComponent();
        }
        DateTime date;
        int filmID;
        int occupancy;
        int roomID;
        private void ApplyButton_Click(object sender, EventArgs e)
        {
            formHome.EditRow(
                dateTimePicker.Value,
                filmsComboBox.SelectedIndex + 1, 
                (int)occupancyNumeric.Value,
                roomsComboBox.SelectedIndex + 1);
            formHome.LoadTable();
            Hide();
        }
        private void EditingForm_Load(object sender, EventArgs e)
        {
            labelId.Text = formHome.selectedRow.Cells[0].Value.ToString();
            roomsTableAdapter.Fill(romsBaseDataSet.Rooms);
            filmsTableAdapter.Fill(filmsDataSet.Films);
            filmID = (int)formHome.selectedRow.Cells[1].Value - 1;
            occupancy = (int)formHome.selectedRow.Cells[2].Value;
            date = (DateTime)formHome.selectedRow.Cells[3].Value;
            roomID = (int)formHome.selectedRow.Cells[4].Value - 1;

            filmsComboBox.SelectedIndex = filmID;
            occupancyNumeric.Value = occupancy;
            dateTimePicker.Value = date;
            roomsComboBox.SelectedIndex = roomID;
        }
        private void CancelButton_Click(object sender, EventArgs e) => Hide();
    }
}
