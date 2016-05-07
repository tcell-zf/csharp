using System;
using System.Windows.Forms;

using TCell.IO;

namespace SystemInfo
{
    public partial class FormSystemInfo : Form
    {
        public FormSystemInfo()
        {
            InitializeComponent();
        }

        private void FormSystemInfo_Load(object sender, EventArgs e)
        {
            textBoxLocalMac.Text = Management.GetLocalHostMac().ToString();
            textBoxCpuId.Text = Management.GetCPUProcessorIds().ToString();
            textBoxHDD.Text = Management.GetHDDSerialNumbers().ToString();
        }
    }
}
