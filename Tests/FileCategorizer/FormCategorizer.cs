using System;
using System.IO;
using System.Windows.Forms;

using TCell.IO;

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
            listViewFiles.Items.Clear();

            if (string.IsNullOrEmpty(textBoxFolder.Text) || !Directory.Exists(textBoxFolder.Text))
                return;

            string[] paths = Directory.GetFiles(textBoxFolder.Text);
            if (paths == null || paths.Length == 0)
                return;

            foreach (string path in paths)
            {
                string typeString;
                FileType type = TCell.IO.File.GetFileType(path, out typeString);
                FileCategory category = TCell.IO.File.GetFileCategory(path);

                ListViewItem item = new ListViewItem()
                {
                    Text = path
                };
                item.SubItems.Add(new ListViewItem.ListViewSubItem()
                {
                    Text = typeString
                });
                item.SubItems.Add(new ListViewItem.ListViewSubItem()
                {
                    Text = type.ToString()
                });
                item.SubItems.Add(new ListViewItem.ListViewSubItem()
                {
                    Text = category.ToString()
                });

                listViewFiles.Items.Add(item);
            }
        }
    }
}
