namespace RRDM4ATMsWin
{
    partial class Form2_MessageContent
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.labelStep1 = new System.Windows.Forms.Label();
            this.buttonProceed = new System.Windows.Forms.Button();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.buttonPrint = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Location = new System.Drawing.Point(12, 52);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(778, 598);
            this.panel1.TabIndex = 0;
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(24, 14);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(736, 573);
            this.textBox1.TabIndex = 0;
            this.textBox1.TabStop = false;
            // 
            // labelStep1
            // 
            this.labelStep1.AutoSize = true;
            this.labelStep1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelStep1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStep1.ForeColor = System.Drawing.Color.White;
            this.labelStep1.Location = new System.Drawing.Point(0, 0);
            this.labelStep1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelStep1.Name = "labelStep1";
            this.labelStep1.Size = new System.Drawing.Size(221, 26);
            this.labelStep1.TabIndex = 243;
            this.labelStep1.Text = "Details Of Matching";
            // 
            // buttonProceed
            // 
            this.buttonProceed.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonProceed.FlatAppearance.BorderSize = 2;
            this.buttonProceed.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonProceed.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonProceed.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonProceed.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonProceed.ForeColor = System.Drawing.Color.White;
            this.buttonProceed.Location = new System.Drawing.Point(611, 655);
            this.buttonProceed.Margin = new System.Windows.Forms.Padding(2, 2, 10, 2);
            this.buttonProceed.Name = "buttonProceed";
            this.buttonProceed.Size = new System.Drawing.Size(160, 27);
            this.buttonProceed.TabIndex = 245;
            this.buttonProceed.Text = "Send Email";
            this.buttonProceed.UseVisualStyleBackColor = true;
            this.buttonProceed.Click += new System.EventHandler(this.buttonProceed_Click);
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.ButtonCancel.FlatAppearance.BorderSize = 2;
            this.ButtonCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.ButtonCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.ButtonCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ButtonCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ButtonCancel.ForeColor = System.Drawing.Color.White;
            this.ButtonCancel.Location = new System.Drawing.Point(417, 655);
            this.ButtonCancel.Margin = new System.Windows.Forms.Padding(2, 2, 10, 2);
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.Size = new System.Drawing.Size(71, 27);
            this.ButtonCancel.TabIndex = 246;
            this.ButtonCancel.Text = "Cancel";
            this.ButtonCancel.UseVisualStyleBackColor = true;
            this.ButtonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
            // 
            // buttonPrint
            // 
            this.buttonPrint.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonPrint.FlatAppearance.BorderSize = 2;
            this.buttonPrint.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonPrint.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonPrint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPrint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPrint.ForeColor = System.Drawing.Color.White;
            this.buttonPrint.Location = new System.Drawing.Point(517, 655);
            this.buttonPrint.Margin = new System.Windows.Forms.Padding(2, 2, 10, 2);
            this.buttonPrint.Name = "buttonPrint";
            this.buttonPrint.Size = new System.Drawing.Size(71, 27);
            this.buttonPrint.TabIndex = 247;
            this.buttonPrint.Text = "Print";
            this.buttonPrint.UseVisualStyleBackColor = true;
            this.buttonPrint.Click += new System.EventHandler(this.buttonPrint_Click);
            // 
            // Form2_MessageContent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(814, 694);
            this.Controls.Add(this.buttonPrint);
            this.Controls.Add(this.ButtonCancel);
            this.Controls.Add(this.buttonProceed);
            this.Controls.Add(this.labelStep1);
            this.Controls.Add(this.panel1);
            this.Name = "Form2_MessageContent";
            this.Text = "Form2_MessageContent";
            this.Load += new System.EventHandler(this.Form2_MessageContent_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label labelStep1;
        private System.Windows.Forms.Button buttonProceed;
        private System.Windows.Forms.Button ButtonCancel;
        private System.Windows.Forms.Button buttonPrint;
    }
}