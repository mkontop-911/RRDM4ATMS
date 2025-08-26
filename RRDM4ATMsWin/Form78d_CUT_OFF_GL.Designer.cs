namespace RRDM4ATMsWin
{
    partial class Form78d_CUT_OFF_GL
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.labelWhatGrid = new System.Windows.Forms.Label();
            this.labelSelected = new System.Windows.Forms.Label();
            this.labelAtmNo = new System.Windows.Forms.Label();
            this.textBoxAtmNo = new System.Windows.Forms.TextBox();
            this.buttonShow = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxTotalRec = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonPrint = new System.Windows.Forms.Button();
            this.buttonExcel = new System.Windows.Forms.Button();
            this.textBoxCateg = new System.Windows.Forms.TextBox();
            this.labelCateg = new System.Windows.Forms.Label();
            this.buttonShowCateg = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.buttonExtractToExcel = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.panel1.Controls.Add(this.dataGridView1);
            this.panel1.Location = new System.Drawing.Point(12, 40);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1240, 647);
            this.panel1.TabIndex = 0;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.GridColor = System.Drawing.Color.LightGray;
            this.dataGridView1.Location = new System.Drawing.Point(11, 3);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(1216, 640);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_RowEnter);
            // 
            // labelWhatGrid
            // 
            this.labelWhatGrid.AutoSize = true;
            this.labelWhatGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.labelWhatGrid.ForeColor = System.Drawing.Color.White;
            this.labelWhatGrid.Location = new System.Drawing.Point(263, 11);
            this.labelWhatGrid.Name = "labelWhatGrid";
            this.labelWhatGrid.Size = new System.Drawing.Size(104, 26);
            this.labelWhatGrid.TabIndex = 1;
            this.labelWhatGrid.Text = "WhatGrid";
            // 
            // labelSelected
            // 
            this.labelSelected.AutoSize = true;
            this.labelSelected.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.labelSelected.ForeColor = System.Drawing.Color.White;
            this.labelSelected.Location = new System.Drawing.Point(12, 698);
            this.labelSelected.Name = "labelSelected";
            this.labelSelected.Size = new System.Drawing.Size(0, 20);
            this.labelSelected.TabIndex = 2;
            // 
            // labelAtmNo
            // 
            this.labelAtmNo.AutoSize = true;
            this.labelAtmNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAtmNo.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.labelAtmNo.Location = new System.Drawing.Point(14, 698);
            this.labelAtmNo.Name = "labelAtmNo";
            this.labelAtmNo.Size = new System.Drawing.Size(51, 13);
            this.labelAtmNo.TabIndex = 350;
            this.labelAtmNo.Text = "ATM no";
            this.labelAtmNo.Click += new System.EventHandler(this.labelAtmNo_Click);
            // 
            // textBoxAtmNo
            // 
            this.textBoxAtmNo.Location = new System.Drawing.Point(71, 699);
            this.textBoxAtmNo.Name = "textBoxAtmNo";
            this.textBoxAtmNo.Size = new System.Drawing.Size(100, 20);
            this.textBoxAtmNo.TabIndex = 349;
            this.textBoxAtmNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // buttonShow
            // 
            this.buttonShow.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonShow.FlatAppearance.BorderSize = 2;
            this.buttonShow.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonShow.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonShow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonShow.ForeColor = System.Drawing.Color.White;
            this.buttonShow.Location = new System.Drawing.Point(187, 691);
            this.buttonShow.Margin = new System.Windows.Forms.Padding(2, 2, 10, 2);
            this.buttonShow.Name = "buttonShow";
            this.buttonShow.Size = new System.Drawing.Size(110, 32);
            this.buttonShow.TabIndex = 351;
            this.buttonShow.Text = "Show";
            this.buttonShow.UseVisualStyleBackColor = true;
            this.buttonShow.Click += new System.EventHandler(this.buttonShow_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label1.Location = new System.Drawing.Point(312, 698);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 354;
            this.label1.Text = "Total Records";
            // 
            // textBoxTotalRec
            // 
            this.textBoxTotalRec.Location = new System.Drawing.Point(400, 696);
            this.textBoxTotalRec.Name = "textBoxTotalRec";
            this.textBoxTotalRec.Size = new System.Drawing.Size(100, 20);
            this.textBoxTotalRec.TabIndex = 353;
            this.textBoxTotalRec.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(310, 698);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 20);
            this.label2.TabIndex = 352;
            // 
            // buttonPrint
            // 
            this.buttonPrint.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonPrint.FlatAppearance.BorderSize = 2;
            this.buttonPrint.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonPrint.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonPrint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPrint.ForeColor = System.Drawing.Color.White;
            this.buttonPrint.Location = new System.Drawing.Point(1163, 691);
            this.buttonPrint.Margin = new System.Windows.Forms.Padding(2, 2, 10, 2);
            this.buttonPrint.Name = "buttonPrint";
            this.buttonPrint.Size = new System.Drawing.Size(84, 32);
            this.buttonPrint.TabIndex = 355;
            this.buttonPrint.Text = "Print";
            this.buttonPrint.UseVisualStyleBackColor = true;
            this.buttonPrint.Click += new System.EventHandler(this.buttonPrint_Click);
            // 
            // buttonExcel
            // 
            this.buttonExcel.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonExcel.FlatAppearance.BorderSize = 2;
            this.buttonExcel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonExcel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonExcel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonExcel.ForeColor = System.Drawing.Color.White;
            this.buttonExcel.Location = new System.Drawing.Point(1168, 5);
            this.buttonExcel.Margin = new System.Windows.Forms.Padding(2, 2, 10, 2);
            this.buttonExcel.Name = "buttonExcel";
            this.buttonExcel.Size = new System.Drawing.Size(84, 32);
            this.buttonExcel.TabIndex = 356;
            this.buttonExcel.Text = "Excel";
            this.buttonExcel.UseVisualStyleBackColor = true;
            // 
            // textBoxCateg
            // 
            this.textBoxCateg.Location = new System.Drawing.Point(607, 695);
            this.textBoxCateg.Name = "textBoxCateg";
            this.textBoxCateg.Size = new System.Drawing.Size(100, 20);
            this.textBoxCateg.TabIndex = 357;
            this.textBoxCateg.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // labelCateg
            // 
            this.labelCateg.AutoSize = true;
            this.labelCateg.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCateg.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.labelCateg.Location = new System.Drawing.Point(544, 698);
            this.labelCateg.Name = "labelCateg";
            this.labelCateg.Size = new System.Drawing.Size(57, 13);
            this.labelCateg.TabIndex = 358;
            this.labelCateg.Text = "Category";
            // 
            // buttonShowCateg
            // 
            this.buttonShowCateg.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonShowCateg.FlatAppearance.BorderSize = 2;
            this.buttonShowCateg.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonShowCateg.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonShowCateg.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonShowCateg.ForeColor = System.Drawing.Color.White;
            this.buttonShowCateg.Location = new System.Drawing.Point(714, 692);
            this.buttonShowCateg.Margin = new System.Windows.Forms.Padding(2, 2, 10, 2);
            this.buttonShowCateg.Name = "buttonShowCateg";
            this.buttonShowCateg.Size = new System.Drawing.Size(148, 32);
            this.buttonShowCateg.TabIndex = 359;
            this.buttonShowCateg.Text = "Show Categ";
            this.buttonShowCateg.UseVisualStyleBackColor = true;
            this.buttonShowCateg.Click += new System.EventHandler(this.buttonShowCateg_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(12, 13);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(171, 21);
            this.comboBox1.TabIndex = 360;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // buttonExtractToExcel
            // 
            this.buttonExtractToExcel.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonExtractToExcel.FlatAppearance.BorderSize = 2;
            this.buttonExtractToExcel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonExtractToExcel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonExtractToExcel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonExtractToExcel.ForeColor = System.Drawing.Color.White;
            this.buttonExtractToExcel.Location = new System.Drawing.Point(1041, 691);
            this.buttonExtractToExcel.Margin = new System.Windows.Forms.Padding(2, 2, 10, 2);
            this.buttonExtractToExcel.Name = "buttonExtractToExcel";
            this.buttonExtractToExcel.Size = new System.Drawing.Size(110, 32);
            this.buttonExtractToExcel.TabIndex = 361;
            this.buttonExtractToExcel.Text = "Extract to Excel";
            this.buttonExtractToExcel.UseVisualStyleBackColor = true;
            this.buttonExtractToExcel.Click += new System.EventHandler(this.buttonExtractToExcel_Click);
            // 
            // Form78d_CUT_OFF_GL
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(1264, 730);
            this.Controls.Add(this.buttonExtractToExcel);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.buttonShowCateg);
            this.Controls.Add(this.labelCateg);
            this.Controls.Add(this.textBoxCateg);
            this.Controls.Add(this.buttonExcel);
            this.Controls.Add(this.buttonPrint);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxTotalRec);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonShow);
            this.Controls.Add(this.labelAtmNo);
            this.Controls.Add(this.textBoxAtmNo);
            this.Controls.Add(this.labelSelected);
            this.Controls.Add(this.labelWhatGrid);
            this.Controls.Add(this.panel1);
            this.Name = "Form78d_CUT_OFF_GL";
            this.Text = "Form78d_CUT_OFF_GL";
            this.Load += new System.EventHandler(this.Form78b_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label labelWhatGrid;
        private System.Windows.Forms.Label labelSelected;
        private System.Windows.Forms.Label labelAtmNo;
        private System.Windows.Forms.TextBox textBoxAtmNo;
        private System.Windows.Forms.Button buttonShow;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxTotalRec;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonPrint;
        private System.Windows.Forms.Button buttonExcel;
        private System.Windows.Forms.TextBox textBoxCateg;
        private System.Windows.Forms.Label labelCateg;
        private System.Windows.Forms.Button buttonShowCateg;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button buttonExtractToExcel;
    }
}