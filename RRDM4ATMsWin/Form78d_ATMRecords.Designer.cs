namespace RRDM4ATMsWin
{
    partial class Form78d_ATMRecords
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxRecords = new System.Windows.Forms.TextBox();
            this.buttonPrint = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxTerm = new System.Windows.Forms.TextBox();
            this.buttonShowTerm = new System.Windows.Forms.Button();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.buttonExportToExcel = new System.Windows.Forms.Button();
            this.labelDebits = new System.Windows.Forms.Label();
            this.textBoxTotalDebit = new System.Windows.Forms.TextBox();
            this.labelCredits = new System.Windows.Forms.Label();
            this.textBoxTotalCredit = new System.Windows.Forms.TextBox();
            this.buttonShowSM = new System.Windows.Forms.Button();
            this.buttonJournalLines = new System.Windows.Forms.Button();
            this.buttonCreateTxn = new System.Windows.Forms.Button();
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
            this.dataGridView1.Location = new System.Drawing.Point(4, 6);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(1216, 627);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_RowEnter);
            // 
            // labelWhatGrid
            // 
            this.labelWhatGrid.AutoSize = true;
            this.labelWhatGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.labelWhatGrid.ForeColor = System.Drawing.Color.White;
            this.labelWhatGrid.Location = new System.Drawing.Point(13, 13);
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label1.Location = new System.Drawing.Point(209, 701);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 13);
            this.label1.TabIndex = 348;
            this.label1.Text = "Number of Records";
            // 
            // textBoxRecords
            // 
            this.textBoxRecords.Location = new System.Drawing.Point(331, 698);
            this.textBoxRecords.Name = "textBoxRecords";
            this.textBoxRecords.Size = new System.Drawing.Size(100, 20);
            this.textBoxRecords.TabIndex = 347;
            this.textBoxRecords.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // buttonPrint
            // 
            this.buttonPrint.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonPrint.FlatAppearance.BorderSize = 2;
            this.buttonPrint.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonPrint.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonPrint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPrint.ForeColor = System.Drawing.Color.White;
            this.buttonPrint.Location = new System.Drawing.Point(1146, 691);
            this.buttonPrint.Margin = new System.Windows.Forms.Padding(2, 2, 10, 2);
            this.buttonPrint.Name = "buttonPrint";
            this.buttonPrint.Size = new System.Drawing.Size(84, 32);
            this.buttonPrint.TabIndex = 349;
            this.buttonPrint.Text = "Print";
            this.buttonPrint.UseVisualStyleBackColor = true;
            this.buttonPrint.Click += new System.EventHandler(this.buttonPrint_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label2.Location = new System.Drawing.Point(13, 701);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 351;
            this.label2.Text = "Terminal Id";
            // 
            // textBoxTerm
            // 
            this.textBoxTerm.Location = new System.Drawing.Point(103, 698);
            this.textBoxTerm.Name = "textBoxTerm";
            this.textBoxTerm.Size = new System.Drawing.Size(100, 20);
            this.textBoxTerm.TabIndex = 350;
            this.textBoxTerm.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // buttonShowTerm
            // 
            this.buttonShowTerm.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonShowTerm.FlatAppearance.BorderSize = 2;
            this.buttonShowTerm.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonShowTerm.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonShowTerm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonShowTerm.ForeColor = System.Drawing.Color.White;
            this.buttonShowTerm.Location = new System.Drawing.Point(914, 691);
            this.buttonShowTerm.Margin = new System.Windows.Forms.Padding(2, 2, 10, 2);
            this.buttonShowTerm.Name = "buttonShowTerm";
            this.buttonShowTerm.Size = new System.Drawing.Size(106, 32);
            this.buttonShowTerm.TabIndex = 352;
            this.buttonShowTerm.Text = "Show Term";
            this.buttonShowTerm.UseVisualStyleBackColor = true;
            this.buttonShowTerm.Click += new System.EventHandler(this.buttonShowTerm_Click);
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonRefresh.FlatAppearance.BorderSize = 2;
            this.buttonRefresh.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonRefresh.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonRefresh.ForeColor = System.Drawing.Color.White;
            this.buttonRefresh.Location = new System.Drawing.Point(1041, 691);
            this.buttonRefresh.Margin = new System.Windows.Forms.Padding(2, 2, 10, 2);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(84, 32);
            this.buttonRefresh.TabIndex = 353;
            this.buttonRefresh.Text = "Refresh All";
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // buttonExportToExcel
            // 
            this.buttonExportToExcel.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonExportToExcel.FlatAppearance.BorderSize = 2;
            this.buttonExportToExcel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonExportToExcel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonExportToExcel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonExportToExcel.ForeColor = System.Drawing.Color.White;
            this.buttonExportToExcel.Location = new System.Drawing.Point(1156, 3);
            this.buttonExportToExcel.Name = "buttonExportToExcel";
            this.buttonExportToExcel.Size = new System.Drawing.Size(95, 30);
            this.buttonExportToExcel.TabIndex = 438;
            this.buttonExportToExcel.Text = "Export to Excel";
            this.buttonExportToExcel.UseVisualStyleBackColor = true;
            this.buttonExportToExcel.Click += new System.EventHandler(this.buttonExportToExcel_Click);
            // 
            // labelDebits
            // 
            this.labelDebits.AutoSize = true;
            this.labelDebits.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDebits.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.labelDebits.Location = new System.Drawing.Point(441, 701);
            this.labelDebits.Name = "labelDebits";
            this.labelDebits.Size = new System.Drawing.Size(70, 13);
            this.labelDebits.TabIndex = 440;
            this.labelDebits.Text = "Total Debit";
            // 
            // textBoxTotalDebit
            // 
            this.textBoxTotalDebit.Location = new System.Drawing.Point(517, 698);
            this.textBoxTotalDebit.Name = "textBoxTotalDebit";
            this.textBoxTotalDebit.Size = new System.Drawing.Size(100, 20);
            this.textBoxTotalDebit.TabIndex = 439;
            this.textBoxTotalDebit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // labelCredits
            // 
            this.labelCredits.AutoSize = true;
            this.labelCredits.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCredits.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.labelCredits.Location = new System.Drawing.Point(619, 701);
            this.labelCredits.Name = "labelCredits";
            this.labelCredits.Size = new System.Drawing.Size(73, 13);
            this.labelCredits.TabIndex = 442;
            this.labelCredits.Text = "Total Credit";
            // 
            // textBoxTotalCredit
            // 
            this.textBoxTotalCredit.Location = new System.Drawing.Point(696, 698);
            this.textBoxTotalCredit.Name = "textBoxTotalCredit";
            this.textBoxTotalCredit.Size = new System.Drawing.Size(100, 20);
            this.textBoxTotalCredit.TabIndex = 441;
            this.textBoxTotalCredit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // buttonShowSM
            // 
            this.buttonShowSM.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonShowSM.FlatAppearance.BorderSize = 2;
            this.buttonShowSM.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonShowSM.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonShowSM.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonShowSM.ForeColor = System.Drawing.Color.White;
            this.buttonShowSM.Location = new System.Drawing.Point(813, 691);
            this.buttonShowSM.Margin = new System.Windows.Forms.Padding(2, 2, 10, 2);
            this.buttonShowSM.Name = "buttonShowSM";
            this.buttonShowSM.Size = new System.Drawing.Size(84, 32);
            this.buttonShowSM.TabIndex = 443;
            this.buttonShowSM.Text = "Show SM";
            this.buttonShowSM.UseVisualStyleBackColor = true;
            this.buttonShowSM.Click += new System.EventHandler(this.buttonShowSM_Click);
            // 
            // buttonJournalLines
            // 
            this.buttonJournalLines.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonJournalLines.FlatAppearance.BorderSize = 2;
            this.buttonJournalLines.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonJournalLines.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonJournalLines.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonJournalLines.ForeColor = System.Drawing.Color.White;
            this.buttonJournalLines.Location = new System.Drawing.Point(1041, 4);
            this.buttonJournalLines.Name = "buttonJournalLines";
            this.buttonJournalLines.Size = new System.Drawing.Size(95, 30);
            this.buttonJournalLines.TabIndex = 444;
            this.buttonJournalLines.Text = "Journal Lines";
            this.buttonJournalLines.UseVisualStyleBackColor = true;
            this.buttonJournalLines.Click += new System.EventHandler(this.buttonJournalLines_Click);
            // 
            // buttonCreateTxn
            // 
            this.buttonCreateTxn.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonCreateTxn.FlatAppearance.BorderSize = 2;
            this.buttonCreateTxn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonCreateTxn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonCreateTxn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonCreateTxn.ForeColor = System.Drawing.Color.White;
            this.buttonCreateTxn.Location = new System.Drawing.Point(925, 4);
            this.buttonCreateTxn.Name = "buttonCreateTxn";
            this.buttonCreateTxn.Size = new System.Drawing.Size(95, 30);
            this.buttonCreateTxn.TabIndex = 445;
            this.buttonCreateTxn.Text = "Create TXN";
            this.buttonCreateTxn.UseVisualStyleBackColor = true;
            this.buttonCreateTxn.Click += new System.EventHandler(this.buttonCreateTxn_Click);
            // 
            // Form78d_ATMRecords
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(1264, 730);
            this.Controls.Add(this.buttonCreateTxn);
            this.Controls.Add(this.buttonJournalLines);
            this.Controls.Add(this.buttonShowSM);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxTerm);
            this.Controls.Add(this.labelCredits);
            this.Controls.Add(this.textBoxTotalCredit);
            this.Controls.Add(this.labelDebits);
            this.Controls.Add(this.textBoxTotalDebit);
            this.Controls.Add(this.buttonExportToExcel);
            this.Controls.Add(this.buttonRefresh);
            this.Controls.Add(this.buttonShowTerm);
            this.Controls.Add(this.buttonPrint);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxRecords);
            this.Controls.Add(this.labelSelected);
            this.Controls.Add(this.labelWhatGrid);
            this.Controls.Add(this.panel1);
            this.Name = "Form78d_ATMRecords";
            this.Text = "Form78d_ATMRecords";
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
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxRecords;
        private System.Windows.Forms.Button buttonPrint;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxTerm;
        private System.Windows.Forms.Button buttonShowTerm;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.Button buttonExportToExcel;
        private System.Windows.Forms.Label labelDebits;
        private System.Windows.Forms.TextBox textBoxTotalDebit;
        private System.Windows.Forms.Label labelCredits;
        private System.Windows.Forms.TextBox textBoxTotalCredit;
        private System.Windows.Forms.Button buttonShowSM;
        private System.Windows.Forms.Button buttonJournalLines;
        private System.Windows.Forms.Button buttonCreateTxn;
    }
}