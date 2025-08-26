namespace RRDM4ATMsWin
{
    partial class UCForm276a_NBG
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel6 = new System.Windows.Forms.Panel();
            this.labelHeader = new System.Windows.Forms.Label();
            this.buttonReverse = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.textBoxCount = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBoxExcelName = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.buttonInportExcel = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.White;
            this.panel6.Controls.Add(this.labelHeader);
            this.panel6.Controls.Add(this.buttonReverse);
            this.panel6.Controls.Add(this.button8);
            this.panel6.Controls.Add(this.textBoxCount);
            this.panel6.Controls.Add(this.label12);
            this.panel6.Controls.Add(this.dataGridView1);
            this.panel6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.panel6.Location = new System.Drawing.Point(19, 93);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(1249, 537);
            this.panel6.TabIndex = 338;
            // 
            // labelHeader
            // 
            this.labelHeader.AutoSize = true;
            this.labelHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.labelHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.labelHeader.Location = new System.Drawing.Point(3, 0);
            this.labelHeader.Name = "labelHeader";
            this.labelHeader.Size = new System.Drawing.Size(138, 18);
            this.labelHeader.TabIndex = 463;
            this.labelHeader.Text = "EXCEL ENTRIES";
            // 
            // buttonReverse
            // 
            this.buttonReverse.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonReverse.FlatAppearance.BorderSize = 2;
            this.buttonReverse.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonReverse.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonReverse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonReverse.Location = new System.Drawing.Point(632, 445);
            this.buttonReverse.Name = "buttonReverse";
            this.buttonReverse.Size = new System.Drawing.Size(187, 35);
            this.buttonReverse.TabIndex = 462;
            this.buttonReverse.Text = "Reverse Entries and Restart";
            this.buttonReverse.UseVisualStyleBackColor = true;
            this.buttonReverse.Visible = false;
            this.buttonReverse.Click += new System.EventHandler(this.buttonReverse_Click);
            // 
            // button8
            // 
            this.button8.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.button8.FlatAppearance.BorderSize = 2;
            this.button8.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.button8.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.button8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button8.Location = new System.Drawing.Point(858, 444);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(170, 36);
            this.button8.TabIndex = 461;
            this.button8.Text = "Populate RRDM";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // textBoxCount
            // 
            this.textBoxCount.Location = new System.Drawing.Point(85, 451);
            this.textBoxCount.Name = "textBoxCount";
            this.textBoxCount.ReadOnly = true;
            this.textBoxCount.Size = new System.Drawing.Size(68, 20);
            this.textBoxCount.TabIndex = 459;
            this.textBoxCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(3, 456);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(56, 13);
            this.label12.TabIndex = 458;
            this.label12.Text = "Excel Size";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.GridColor = System.Drawing.Color.LightGray;
            this.dataGridView1.Location = new System.Drawing.Point(6, 21);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(1224, 417);
            this.dataGridView1.TabIndex = 357;
            this.dataGridView1.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_RowEnter);
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.SystemColors.Control;
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.textBox2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.textBox2.Location = new System.Drawing.Point(721, -21);
            this.textBox2.Margin = new System.Windows.Forms.Padding(7, 3, 7, 0);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(440, 19);
            this.textBox2.TabIndex = 342;
            this.textBox2.Text = "LINE DETAILS FOR ATM";
            // 
            // textBoxExcelName
            // 
            this.textBoxExcelName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxExcelName.Location = new System.Drawing.Point(189, 2);
            this.textBoxExcelName.Multiline = true;
            this.textBoxExcelName.Name = "textBoxExcelName";
            this.textBoxExcelName.ReadOnly = true;
            this.textBoxExcelName.Size = new System.Drawing.Size(393, 55);
            this.textBoxExcelName.TabIndex = 466;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(607, 11);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(156, 13);
            this.label9.TabIndex = 465;
            this.label9.Text = "Input Excel Date for Verification";
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(769, 8);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker1.TabIndex = 464;
            // 
            // buttonInportExcel
            // 
            this.buttonInportExcel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonInportExcel.FlatAppearance.BorderSize = 2;
            this.buttonInportExcel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonInportExcel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonInportExcel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonInportExcel.Location = new System.Drawing.Point(1052, 5);
            this.buttonInportExcel.Name = "buttonInportExcel";
            this.buttonInportExcel.Size = new System.Drawing.Size(93, 25);
            this.buttonInportExcel.TabIndex = 463;
            this.buttonInportExcel.Text = "Load Excel";
            this.buttonInportExcel.UseVisualStyleBackColor = true;
            this.buttonInportExcel.Click += new System.EventHandler(this.buttonInportExcel_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.Color.White;
            this.label11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.label11.Location = new System.Drawing.Point(150, 5);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(33, 13);
            this.label11.TabIndex = 462;
            this.label11.Text = "Excel";
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonBrowse.FlatAppearance.BorderSize = 2;
            this.buttonBrowse.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonBrowse.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonBrowse.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonBrowse.Location = new System.Drawing.Point(3, 3);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(141, 27);
            this.buttonBrowse.TabIndex = 461;
            this.buttonBrowse.Text = "Browse for Excel";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // UCForm276a_NBG
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.Controls.Add(this.textBoxExcelName);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.buttonInportExcel);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.buttonBrowse);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.panel6);
            this.Name = "UCForm276a_NBG";
            this.Size = new System.Drawing.Size(1280, 647);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBoxExcelName;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Button buttonInportExcel;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.Button buttonReverse;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.TextBox textBoxCount;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label labelHeader;
    }
}
