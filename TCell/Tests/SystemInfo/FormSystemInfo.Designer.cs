namespace SystemInfo
{
    partial class FormSystemInfo
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxLocalMac = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxCpuId = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxHDD = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(186, 29);
            this.label1.TabIndex = 0;
            this.label1.Text = "Local host MAC:";
            // 
            // textBoxLocalMac
            // 
            this.textBoxLocalMac.Location = new System.Drawing.Point(232, 10);
            this.textBoxLocalMac.Name = "textBoxLocalMac";
            this.textBoxLocalMac.ReadOnly = true;
            this.textBoxLocalMac.Size = new System.Drawing.Size(578, 35);
            this.textBoxLocalMac.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(212, 29);
            this.label2.TabIndex = 2;
            this.label2.Text = "CPU processor ID:";
            // 
            // textBoxCpuId
            // 
            this.textBoxCpuId.Location = new System.Drawing.Point(232, 69);
            this.textBoxCpuId.Name = "textBoxCpuId";
            this.textBoxCpuId.ReadOnly = true;
            this.textBoxCpuId.Size = new System.Drawing.Size(578, 35);
            this.textBoxCpuId.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 131);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(173, 29);
            this.label3.TabIndex = 4;
            this.label3.Text = "HDD serial No:";
            // 
            // textBoxHDD
            // 
            this.textBoxHDD.Location = new System.Drawing.Point(232, 128);
            this.textBoxHDD.Name = "textBoxHDD";
            this.textBoxHDD.ReadOnly = true;
            this.textBoxHDD.Size = new System.Drawing.Size(578, 35);
            this.textBoxHDD.TabIndex = 5;
            // 
            // FormSystemInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 29F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(974, 477);
            this.Controls.Add(this.textBoxHDD);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxCpuId);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxLocalMac);
            this.Controls.Add(this.label1);
            this.Name = "FormSystemInfo";
            this.Text = "System Info Reader";
            this.Load += new System.EventHandler(this.FormSystemInfo_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxLocalMac;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxCpuId;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxHDD;
    }
}