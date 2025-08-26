namespace RRDM4ATMsWin
{
    partial class Form78d_FileRecords_IST_PRESENTER
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
            this.buttonSourceRecords = new System.Windows.Forms.Button();
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
            this.label1.Location = new System.Drawing.Point(21, 704);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 13);
            this.label1.TabIndex = 348;
            this.label1.Text = "Number of Records";
            // 
            // textBoxRecords
            // 
            this.textBoxRecords.Location = new System.Drawing.Point(138, 704);
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
            this.label2.Location = new System.Drawing.Point(733, 700);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 351;
            this.label2.Text = "Terminal Id";
            // 
            // textBoxTerm
            // 
            this.textBoxTerm.Location = new System.Drawing.Point(801, 700);
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
            this.buttonShowTerm.Size = new System.Drawing.Size(84, 32);
            this.buttonShowTerm.TabIndex = 352;
            this.buttonShowTerm.Text = "Show";
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
            // buttonSourceRecords
            // 
            this.buttonSourceRecords.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonSourceRecords.FlatAppearance.BorderSize = 2;
            this.buttonSourceRecords.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonSourceRecords.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonSourceRecords.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSourceRecords.ForeColor = System.Drawing.Color.White;
            this.buttonSourceRecords.Location = new System.Drawing.Point(545, 693);
            this.buttonSourceRecords.Margin = new System.Windows.Forms.Padding(2, 2, 10, 2);
            this.buttonSourceRecords.Name = "buttonSourceRecords";
            this.buttonSourceRecords.Size = new System.Drawing.Size(135, 32);
            this.buttonSourceRecords.TabIndex = 354;
            this.buttonSourceRecords.Text = "Show Source Records";
            this.buttonSourceRecords.UseVisualStyleBackColor = true;
            this.buttonSourceRecords.Click += new System.EventHandler(this.buttonSourceRecords_Click);
            // 
            // Form78d_FileRecords_IST_PRESENTER
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(1264, 730);
            this.Controls.Add(this.buttonSourceRecords);
            this.Controls.Add(this.buttonRefresh);
            this.Controls.Add(this.buttonShowTerm);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxTerm);
            this.Controls.Add(this.buttonPrint);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxRecords);
            this.Controls.Add(this.labelSelected);
            this.Controls.Add(this.labelWhatGrid);
            this.Controls.Add(this.panel1);
            this.Name = "Form78d_FileRecords_IST_PRESENTER";
            this.Text = "Form78d_FileRecords_IST_PRESENTER";
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
        private System.Windows.Forms.Button buttonSourceRecords;
    }
}