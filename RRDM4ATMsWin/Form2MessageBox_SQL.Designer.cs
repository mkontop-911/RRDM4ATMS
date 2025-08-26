namespace RRDM4ATMsWin
{
    partial class Form2MessageBox_SQL
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.OK = new System.Windows.Forms.Button();
            this.textBoxSQLCmd = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(8, 1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(789, 44);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "label1";
            // 
            // OK
            // 
            this.OK.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.OK.Location = new System.Drawing.Point(741, 687);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(56, 23);
            this.OK.TabIndex = 1;
            this.OK.Text = "OK";
            this.OK.UseVisualStyleBackColor = false;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // textBoxSQLCmd
            // 
            this.textBoxSQLCmd.Location = new System.Drawing.Point(8, 51);
            this.textBoxSQLCmd.Multiline = true;
            this.textBoxSQLCmd.Name = "textBoxSQLCmd";
            this.textBoxSQLCmd.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxSQLCmd.Size = new System.Drawing.Size(789, 630);
            this.textBoxSQLCmd.TabIndex = 2;
            // 
            // Form2MessageBox_SQL
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(809, 717);
            this.Controls.Add(this.textBoxSQLCmd);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.panel1);
            this.Name = "Form2MessageBox_SQL";
            this.Text = "RRDMMessageBox";
            this.Load += new System.EventHandler(this.Form2MessageBox_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.TextBox textBoxSQLCmd;
    }
}