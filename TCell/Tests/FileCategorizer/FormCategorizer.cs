using System;
using System.IO;
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

        private void buttonCheck_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxFolder.Text) || !Directory.Exists(textBoxFolder.Text))
                return;
        }
    }
}
