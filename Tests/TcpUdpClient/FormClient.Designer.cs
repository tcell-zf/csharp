namespace TcpUdpClient
{
    partial class FormClient
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBoxProtocol = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownPort = new System.Windows.Forms.NumericUpDown();
            this.textBoxIp = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.radioButtonUdp = new System.Windows.Forms.RadioButton();
            this.radioButtonTcp = new System.Windows.Forms.RadioButton();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonStart = new System.Windows.Forms.Button();
            this.groupBoxProtocol.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPort)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBoxProtocol
            // 
            this.groupBoxProtocol.Controls.Add(this.label2);
            this.groupBoxProtocol.Controls.Add(this.numericUpDownPort);
            this.groupBoxProtocol.Controls.Add(this.textBoxIp);
            this.groupBoxProtocol.Controls.Add(this.label1);
            this.groupBoxProtocol.Controls.Add(this.radioButtonUdp);
            this.groupBoxProtocol.Controls.Add(this.radioButtonTcp);
            this.groupBoxProtocol.Location = new System.Drawing.Point(10, 9);
            this.groupBoxProtocol.Name = "groupBoxProtocol";
            this.groupBoxProtocol.Size = new System.Drawing.Size(813, 84);
            this.groupBoxProtocol.TabIndex = 0;
            this.groupBoxProtocol.TabStop = false;
            this.groupBoxProtocol.Text = "Protocol";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(579, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 29);
            this.label2.TabIndex = 7;
            this.label2.Text = "port:";
            // 
            // numericUpDownPort
            // 
            this.numericUpDownPort.Location = new System.Drawing.Point(646, 34);
            this.numericUpDownPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownPort.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownPort.Name = "numericUpDownPort";
            this.numericUpDownPort.Size = new System.Drawing.Size(118, 35);
            this.numericUpDownPort.TabIndex = 6;
            this.numericUpDownPort.Value = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            // 
            // textBoxIp
            // 
            this.textBoxIp.Location = new System.Drawing.Point(286, 31);
            this.textBoxIp.Name = "textBoxIp";
            this.textBoxIp.Size = new System.Drawing.Size(287, 35);
            this.textBoxIp.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(239, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 29);
            this.label1.TabIndex = 4;
            this.label1.Text = "IP:";
            // 
            // radioButtonUdp
            // 
            this.radioButtonUdp.AutoSize = true;
            this.radioButtonUdp.Location = new System.Drawing.Point(128, 34);
            this.radioButtonUdp.Name = "radioButtonUdp";
            this.radioButtonUdp.Size = new System.Drawing.Size(94, 33);
            this.radioButtonUdp.TabIndex = 3;
            this.radioButtonUdp.TabStop = true;
            this.radioButtonUdp.Text = "UDP";
            this.radioButtonUdp.UseVisualStyleBackColor = true;
            this.radioButtonUdp.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonTcp
            // 
            this.radioButtonTcp.AutoSize = true;
            this.radioButtonTcp.Location = new System.Drawing.Point(18, 34);
            this.radioButtonTcp.Name = "radioButtonTcp";
            this.radioButtonTcp.Size = new System.Drawing.Size(93, 33);
            this.radioButtonTcp.TabIndex = 2;
            this.radioButtonTcp.TabStop = true;
            this.radioButtonTcp.Text = "TCP";
            this.radioButtonTcp.UseVisualStyleBackColor = true;
            this.radioButtonTcp.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(963, 25);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(128, 65);
            this.buttonStop.TabIndex = 9;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(829, 25);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(128, 65);
            this.buttonStart.TabIndex = 8;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            // 
            // FormClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 29F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1103, 653);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.groupBoxProtocol);
            this.Controls.Add(this.buttonStart);
            this.Name = "FormClient";
            this.Text = "TCP/UDP Client";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormClient_FormClosed);
            this.Load += new System.EventHandler(this.FormClient_Load);
            this.groupBoxProtocol.ResumeLayout(false);
            this.groupBoxProtocol.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPort)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxProtocol;
        private System.Windows.Forms.RadioButton radioButtonUdp;
        private System.Windows.Forms.RadioButton radioButtonTcp;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxIp;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDownPort;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Button buttonStart;
    }
}