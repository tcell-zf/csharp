using System;
using System.Windows.Forms;

namespace FileCategorizer
{
    public partial class FormCategorizer : Form
    {
        public FormCategorizer()
        {
            InitializeComponent();
        }

        private void buttonSelect_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                textBoxFolder.Text = folderBrowserDialog.SelectedPath;
            }
        }
    }
}
