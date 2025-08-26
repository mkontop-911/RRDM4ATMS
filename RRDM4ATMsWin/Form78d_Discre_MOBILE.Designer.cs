namespace RRDM4ATMsWin
{
    partial class Form78d_Discre_MOBILE
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
            this.textBoxTotalRec = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonPrint = new System.Windows.Forms.Button();
            this.buttonXML = new System.Windows.Forms.Button();
            this.buttonSourceRecords = new System.Windows.Forms.Button();
            this.textBoxSeqNo = new System.Windows.Forms.TextBox();
            this.labelSeqNo = new System.Windows.Forms.Label();
            this.buttonExportToExcel = new System.Windows.Forms.Button();
            this.textBoxCust_ID = new System.Windows.Forms.TextBox();
            this.labelCustId = new System.Windows.Forms.Label();
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
            this.panel1.Size = new System.Drawing.Size(1312, 647);
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
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(1298, 640);
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
            this.label1.Location = new System.Drawing.Point(8, 698);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 354;
            this.label1.Text = "Total Records";
            // 
            // textBoxTotalRec
            // 
            this.textBoxTotalRec.Location = new System.Drawing.Point(113, 696);
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
            this.buttonPrint.Visible = false;
            this.buttonPrint.Click += new System.EventHandler(this.buttonPrint_Click);
            // 
            // buttonXML
            // 
            this.buttonXML.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonXML.FlatAppearance.BorderSize = 2;
            this.buttonXML.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonXML.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonXML.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonXML.ForeColor = System.Drawing.Color.White;
            this.buttonXML.Location = new System.Drawing.Point(859, 691);
            this.buttonXML.Margin = new System.Windows.Forms.Padding(2, 2, 10, 2);
            this.buttonXML.Name = "buttonXML";
            this.buttonXML.Size = new System.Drawing.Size(108, 32);
            this.buttonXML.TabIndex = 356;
            this.buttonXML.Text = "EXPORT TO Txt ";
            this.buttonXML.UseVisualStyleBackColor = true;
            this.buttonXML.Click += new System.EventHandler(this.buttonXML_Click);
            // 
            // buttonSourceRecords
            // 
            this.buttonSourceRecords.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonSourceRecords.FlatAppearance.BorderSize = 2;
            this.buttonSourceRecords.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonSourceRecords.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonSourceRecords.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSourceRecords.ForeColor = System.Drawing.Color.White;
            this.buttonSourceRecords.Location = new System.Drawing.Point(413, 692);
            this.buttonSourceRecords.Margin = new System.Windows.Forms.Padding(2, 2, 10, 2);
            this.buttonSourceRecords.Name = "buttonSourceRecords";
            this.buttonSourceRecords.Size = new System.Drawing.Size(108, 32);
            this.buttonSourceRecords.TabIndex = 357;
            this.buttonSourceRecords.Text = "SourceRecord/s";
            this.buttonSourceRecords.UseVisualStyleBackColor = true;
            this.buttonSourceRecords.Click += new System.EventHandler(this.buttonSourceRecords_Click);
            // 
            // textBoxSeqNo
            // 
            this.textBoxSeqNo.Location = new System.Drawing.Point(307, 695);
            this.textBoxSeqNo.Name = "textBoxSeqNo";
            this.textBoxSeqNo.Size = new System.Drawing.Size(100, 20);
            this.textBoxSeqNo.TabIndex = 358;
            this.textBoxSeqNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // labelSeqNo
            // 
            this.labelSeqNo.AutoSize = true;
            this.labelSeqNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSeqNo.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.labelSeqNo.Location = new System.Drawing.Point(229, 698);
            this.labelSeqNo.Name = "labelSeqNo";
            this.labelSeqNo.Size = new System.Drawing.Size(76, 13);
            this.labelSeqNo.TabIndex = 359;
            this.labelSeqNo.Text = "Seq Number";
            // 
            // buttonExportToExcel
            // 
            this.buttonExportToExcel.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonExportToExcel.FlatAppearance.BorderSize = 2;
            this.buttonExportToExcel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonExportToExcel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonExportToExcel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonExportToExcel.ForeColor = System.Drawing.Color.White;
            this.buttonExportToExcel.Location = new System.Drawing.Point(987, 691);
            this.buttonExportToExcel.Margin = new System.Windows.Forms.Padding(2, 2, 10, 2);
            this.buttonExportToExcel.Name = "buttonExportToExcel";
            this.buttonExportToExcel.Size = new System.Drawing.Size(163, 32);
            this.buttonExportToExcel.TabIndex = 360;
            this.buttonExportToExcel.Text = "EXPORT TO EXCEL < 1000 ";
            this.buttonExportToExcel.UseVisualStyleBackColor = true;
            this.buttonExportToExcel.Click += new System.EventHandler(this.buttonExportToExcel_Click);
            // 
            // textBoxCust_ID
            // 
            this.textBoxCust_ID.Location = new System.Drawing.Point(621, 696);
            this.textBoxCust_ID.Name = "textBoxCust_ID";
            this.textBoxCust_ID.Size = new System.Drawing.Size(153, 20);
            this.textBoxCust_ID.TabIndex = 361;
            this.textBoxCust_ID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // labelCustId
            // 
            this.labelCustId.AutoSize = true;
            this.labelCustId.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCustId.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.labelCustId.Location = new System.Drawing.Point(539, 699);
            this.labelCustId.Name = "labelCustId";
            this.labelCustId.Size = new System.Drawing.Size(74, 13);
            this.labelCustId.TabIndex = 362;
            this.labelCustId.Text = "Customer Id";
            // 
            // Form78d_Discre_MOBILE
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(1326, 735);
            this.Controls.Add(this.labelCustId);
            this.Controls.Add(this.textBoxCust_ID);
            this.Controls.Add(this.buttonExportToExcel);
            this.Controls.Add(this.labelSeqNo);
            this.Controls.Add(this.textBoxSeqNo);
            this.Controls.Add(this.buttonSourceRecords);
            this.Controls.Add(this.buttonXML);
            this.Controls.Add(this.buttonPrint);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxTotalRec);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelSelected);
            this.Controls.Add(this.labelWhatGrid);
            this.Controls.Add(this.panel1);
            this.Name = "Form78d_Discre_MOBILE";
            this.Text = "Form78d_Discre_MOBILE";
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
        private System.Windows.Forms.TextBox textBoxTotalRec;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonPrint;
        private System.Windows.Forms.Button buttonXML;
        private System.Windows.Forms.Button buttonSourceRecords;
        private System.Windows.Forms.TextBox textBoxSeqNo;
        private System.Windows.Forms.Label labelSeqNo;
        private System.Windows.Forms.Button buttonExportToExcel;
        private System.Windows.Forms.TextBox textBoxCust_ID;
        private System.Windows.Forms.Label labelCustId;
    }
}