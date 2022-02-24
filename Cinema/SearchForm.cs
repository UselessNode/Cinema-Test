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
    public partial class SearchForm : Form
    {
        FormHome home;
        public SearchForm(FormHome formHome)
            {
                formHome = home;
                InitializeComponent();
                BackColor = Properties.Settings.Default.Menu;
            }

        private void SearchForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
        }
    }
}
