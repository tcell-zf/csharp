namespace TcpUdpServer
{
    partial class FormServer
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
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownPort = new System.Windows.Forms.NumericUpDown();
            this.radioButtonUdp = new System.Windows.Forms.RadioButton();
            this.radioButtonTcp = new System.Windows.Forms.RadioButton();
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.textBoxResults = new System.Windows.Forms.TextBox();
            this.groupBoxProtocol.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPort)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBoxProtocol
            // 
            this.groupBoxProtocol.Controls.Add(this.label1);
            this.groupBoxProtocol.Controls.Add(this.numericUpDownPort);
            this.groupBoxProtocol.Controls.Add(this.radioButtonUdp);
            this.groupBoxProtocol.Controls.Add(this.radioButtonTcp);
            this.groupBoxProtocol.Location = new System.Drawing.Point(10, 9);
            this.groupBoxProtocol.Name = "groupBoxProtocol";
            this.groupBoxProtocol.Size = new System.Drawing.Size(412, 80);
            this.groupBoxProtocol.TabIndex = 1;
            this.groupBoxProtocol.TabStop = false;
            this.groupBoxProtocol.Text = "Protocol";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(220, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 29);
            this.label1.TabIndex = 3;
            this.label1.Text = "port:";
            // 
            // numericUpDownPort
            // 
            this.numericUpDownPort.Location = new System.Drawing.Point(278, 32);
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
            this.numericUpDownPort.TabIndex = 2;
            this.numericUpDownPort.Value = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            // 
            // radioButtonUdp
            // 
            this.radioButtonUdp.AutoSize = true;
            this.radioButtonUdp.Location = new System.Drawing.Point(106, 32);
            this.radioButtonUdp.Name = "radioButtonUdp";
            this.radioButtonUdp.Size = new System.Drawing.Size(94, 33);
            this.radioButtonUdp.TabIndex = 1;
            this.radioButtonUdp.TabStop = true;
            this.radioButtonUdp.Text = "UDP";
            this.radioButtonUdp.UseVisualStyleBackColor = true;
            this.radioButtonUdp.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonTcp
            // 
            this.radioButtonTcp.AutoSize = true;
            this.radioButtonTcp.Location = new System.Drawing.Point(15, 32);
            this.radioButtonTcp.Name = "radioButtonTcp";
            this.radioButtonTcp.Size = new System.Drawing.Size(93, 33);
            this.radioButtonTcp.TabIndex = 0;
            this.radioButtonTcp.TabStop = true;
            this.radioButtonTcp.Text = "TCP";
            this.radioButtonTcp.UseVisualStyleBackColor = true;
            this.radioButtonTcp.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(428, 25);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(128, 65);
            this.buttonStart.TabIndex = 2;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(562, 25);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(128, 65);
            this.buttonStop.TabIndex = 3;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // textBoxResults
            // 
            this.textBoxResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxResults.Location = new System.Drawing.Point(10, 96);
            this.textBoxResults.Multiline = true;
            this.textBoxResults.Name = "textBoxResults";
            this.textBoxResults.ReadOnly = true;
            this.textBoxResults.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxResults.Size = new System.Drawing.Size(949, 545);
            this.textBoxResults.TabIndex = 4;
            // 
            // FormServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 29F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(966, 653);
            this.Controls.Add(this.textBoxResults);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.groupBoxProtocol);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "FormServer";
            this.Text = "TCP/UDP Server";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormServer_FormClosed);
            this.Load += new System.EventHandler(this.FormServer_Load);
            this.groupBoxProtocol.ResumeLayout(false);
            this.groupBoxProtocol.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPort)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxProtocol;
        private System.Windows.Forms.RadioButton radioButtonUdp;
        private System.Windows.Forms.RadioButton radioButtonTcp;
        private System.Windows.Forms.NumericUpDown numericUpDownPort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.TextBox textBoxResults;
    }
}