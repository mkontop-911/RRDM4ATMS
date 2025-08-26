namespace RRDM4ATMsWin
{
    partial class Form14b_All_Actions
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
            this.labelFormHeader = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonExportToExcel = new System.Windows.Forms.Button();
            this.textBoxTotal = new System.Windows.Forms.TextBox();
            this.labelTotal = new System.Windows.Forms.Label();
            this.labelGridHeader = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.dataGrid1 = new System.Windows.Forms.DataGridView();
            this.button2 = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
            this.SuspendLayout();
            // 
            // labelFormHeader
            // 
            this.labelFormHeader.AutoSize = true;
            this.labelFormHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFormHeader.ForeColor = System.Drawing.Color.White;
            this.labelFormHeader.Location = new System.Drawing.Point(12, 9);
            this.labelFormHeader.Margin = new System.Windows.Forms.Padding(5, 0, 2, 0);
            this.labelFormHeader.Name = "labelFormHeader";
            this.labelFormHeader.Size = new System.Drawing.Size(142, 29);
            this.labelFormHeader.TabIndex = 268;
            this.labelFormHeader.Text = "All Actions ";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.panel1.Controls.Add(this.buttonExportToExcel);
            this.panel1.Controls.Add(this.labelGridHeader);
            this.panel1.Controls.Add(this.panel5);
            this.panel1.ForeColor = System.Drawing.Color.Black;
            this.panel1.Location = new System.Drawing.Point(14, 41);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1258, 606);
            this.panel1.TabIndex = 270;
            // 
            // buttonExportToExcel
            // 
            this.buttonExportToExcel.BackColor = System.Drawing.Color.Transparent;
            this.buttonExportToExcel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonExportToExcel.FlatAppearance.BorderSize = 2;
            this.buttonExportToExcel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonExportToExcel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonExportToExcel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonExportToExcel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonExportToExcel.Location = new System.Drawing.Point(1134, 4);
            this.buttonExportToExcel.Name = "buttonExportToExcel";
            this.buttonExportToExcel.Size = new System.Drawing.Size(102, 26);
            this.buttonExportToExcel.TabIndex = 342;
            this.buttonExportToExcel.Text = "Export To Excel";
            this.buttonExportToExcel.UseVisualStyleBackColor = false;
            this.buttonExportToExcel.Click += new System.EventHandler(this.buttonExportToExcel_Click);
            // 
            // textBoxTotal
            // 
            this.textBoxTotal.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.textBoxTotal.Location = new System.Drawing.Point(135, 657);
            this.textBoxTotal.Name = "textBoxTotal";
            this.textBoxTotal.ReadOnly = true;
            this.textBoxTotal.Size = new System.Drawing.Size(100, 20);
            this.textBoxTotal.TabIndex = 341;
            this.textBoxTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // labelTotal
            // 
            this.labelTotal.AutoSize = true;
            this.labelTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.labelTotal.ForeColor = System.Drawing.Color.White;
            this.labelTotal.Location = new System.Drawing.Point(34, 657);
            this.labelTotal.Name = "labelTotal";
            this.labelTotal.Size = new System.Drawing.Size(88, 18);
            this.labelTotal.TabIndex = 340;
            this.labelTotal.Text = "Total No : ";
            // 
            // labelGridHeader
            // 
            this.labelGridHeader.AutoSize = true;
            this.labelGridHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.labelGridHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.labelGridHeader.Location = new System.Drawing.Point(20, 8);
            this.labelGridHeader.Name = "labelGridHeader";
            this.labelGridHeader.Size = new System.Drawing.Size(250, 18);
            this.labelGridHeader.TabIndex = 339;
            this.labelGridHeader.Text = "ACCOUNTING TRANSACTIONS";
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.White;
            this.panel5.Controls.Add(this.dataGrid1);
            this.panel5.Location = new System.Drawing.Point(18, 29);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(1225, 574);
            this.panel5.TabIndex = 338;
            // 
            // dataGrid1
            // 
            this.dataGrid1.AllowUserToAddRows = false;
            this.dataGrid1.AllowUserToDeleteRows = false;
            this.dataGrid1.BackgroundColor = System.Drawing.Color.White;
            this.dataGrid1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGrid1.GridColor = System.Drawing.Color.LightGray;
            this.dataGrid1.Location = new System.Drawing.Point(5, 7);
            this.dataGrid1.MultiSelect = false;
            this.dataGrid1.Name = "dataGrid1";
            this.dataGrid1.ReadOnly = true;
            this.dataGrid1.RowHeadersVisible = false;
            this.dataGrid1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGrid1.Size = new System.Drawing.Size(1217, 564);
            this.dataGrid1.TabIndex = 3;
            this.dataGrid1.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGrid1_RowEnter);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.button2.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.button2.FlatAppearance.BorderSize = 2;
            this.button2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.button2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.ForeColor = System.Drawing.Color.White;
            this.button2.Location = new System.Drawing.Point(1187, 653);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(70, 26);
            this.button2.TabIndex = 337;
            this.button2.Text = "Finish";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form14b_All_Actions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(1285, 684);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.labelTotal);
            this.Controls.Add(this.textBoxTotal);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.labelFormHeader);
            this.Name = "Form14b_All_Actions";
            this.Text = "Form14b_All_Actions";
            this.Load += new System.EventHandler(this.Form14b_All_Actions_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelFormHeader;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label labelGridHeader;
        private System.Windows.Forms.DataGridView dataGrid1;
        private System.Windows.Forms.TextBox textBoxTotal;
        private System.Windows.Forms.Label labelTotal;
        private System.Windows.Forms.Button buttonExportToExcel;
    }
}