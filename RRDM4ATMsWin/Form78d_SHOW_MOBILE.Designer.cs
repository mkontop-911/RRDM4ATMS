namespace RRDM4ATMsWin
{
    partial class Form78d_SHOW_MOBILE
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form78d_SHOW_MOBILE));
            this.panel1 = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.labelWhatGrid = new System.Windows.Forms.Label();
            this.labelSelected = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxTotalRec = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonXML = new System.Windows.Forms.Button();
            this.buttonSourceRecords = new System.Windows.Forms.Button();
            this.textBoxSeqNo = new System.Windows.Forms.TextBox();
            this.labelSeqNo = new System.Windows.Forms.Label();
            this.buttonExportToExcel = new System.Windows.Forms.Button();
            this.linkLabelAnalysis = new System.Windows.Forms.LinkLabel();
            this.buttonNotes2 = new System.Windows.Forms.Button();
            this.labelNumberNotes2 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.panel1.Controls.Add(this.labelNumberNotes2);
            this.panel1.Controls.Add(this.buttonNotes2);
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
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(1140, 640);
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
            // buttonXML
            // 
            this.buttonXML.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonXML.FlatAppearance.BorderSize = 2;
            this.buttonXML.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonXML.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonXML.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonXML.ForeColor = System.Drawing.Color.White;
            this.buttonXML.Location = new System.Drawing.Point(688, 691);
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
            this.buttonExportToExcel.Location = new System.Drawing.Point(831, 691);
            this.buttonExportToExcel.Margin = new System.Windows.Forms.Padding(2, 2, 10, 2);
            this.buttonExportToExcel.Name = "buttonExportToExcel";
            this.buttonExportToExcel.Size = new System.Drawing.Size(163, 32);
            this.buttonExportToExcel.TabIndex = 360;
            this.buttonExportToExcel.Text = "EXPORT TO EXCEL < 1000 ";
            this.buttonExportToExcel.UseVisualStyleBackColor = true;
            this.buttonExportToExcel.Click += new System.EventHandler(this.buttonExportToExcel_Click);
            // 
            // linkLabelAnalysis
            // 
            this.linkLabelAnalysis.AutoSize = true;
            this.linkLabelAnalysis.BackColor = System.Drawing.Color.White;
            this.linkLabelAnalysis.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.linkLabelAnalysis.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabelAnalysis.Location = new System.Drawing.Point(534, 701);
            this.linkLabelAnalysis.Name = "linkLabelAnalysis";
            this.linkLabelAnalysis.Size = new System.Drawing.Size(124, 18);
            this.linkLabelAnalysis.TabIndex = 556;
            this.linkLabelAnalysis.TabStop = true;
            this.linkLabelAnalysis.Text = "Link To Analysis";
            this.linkLabelAnalysis.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelAnalysis_LinkClicked);
            // 
            // buttonNotes2
            // 
            this.buttonNotes2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonNotes2.BackgroundImage")));
            this.buttonNotes2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonNotes2.FlatAppearance.BorderColor = System.Drawing.Color.DarkRed;
            this.buttonNotes2.FlatAppearance.BorderSize = 0;
            this.buttonNotes2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonNotes2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonNotes2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonNotes2.ForeColor = System.Drawing.Color.Black;
            this.buttonNotes2.Location = new System.Drawing.Point(1156, 26);
            this.buttonNotes2.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.buttonNotes2.Name = "buttonNotes2";
            this.buttonNotes2.Size = new System.Drawing.Size(56, 52);
            this.buttonNotes2.TabIndex = 396;
            this.buttonNotes2.UseVisualStyleBackColor = true;
            this.buttonNotes2.Click += new System.EventHandler(this.buttonNotes2_Click);
            // 
            // labelNumberNotes2
            // 
            this.labelNumberNotes2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelNumberNotes2.AutoSize = true;
            this.labelNumberNotes2.BackColor = System.Drawing.Color.Gainsboro;
            this.labelNumberNotes2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelNumberNotes2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.labelNumberNotes2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(198)))));
            this.labelNumberNotes2.Location = new System.Drawing.Point(1200, 19);
            this.labelNumberNotes2.Name = "labelNumberNotes2";
            this.labelNumberNotes2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.labelNumberNotes2.Size = new System.Drawing.Size(15, 15);
            this.labelNumberNotes2.TabIndex = 414;
            this.labelNumberNotes2.Text = "2";
            // 
            // Form78d_SHOW_MOBILE
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(1264, 730);
            this.Controls.Add(this.linkLabelAnalysis);
            this.Controls.Add(this.buttonExportToExcel);
            this.Controls.Add(this.labelSeqNo);
            this.Controls.Add(this.textBoxSeqNo);
            this.Controls.Add(this.buttonSourceRecords);
            this.Controls.Add(this.buttonXML);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxTotalRec);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelSelected);
            this.Controls.Add(this.labelWhatGrid);
            this.Controls.Add(this.panel1);
            this.Name = "Form78d_SHOW_MOBILE";
            this.Text = "Form78d_SHOW_MOBILE";
            this.Load += new System.EventHandler(this.Form78b_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
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
        private System.Windows.Forms.Button buttonXML;
        private System.Windows.Forms.Button buttonSourceRecords;
        private System.Windows.Forms.TextBox textBoxSeqNo;
        private System.Windows.Forms.Label labelSeqNo;
        private System.Windows.Forms.Button buttonExportToExcel;
        private System.Windows.Forms.LinkLabel linkLabelAnalysis;
        private System.Windows.Forms.Button buttonNotes2;
        private System.Windows.Forms.Label labelNumberNotes2;
    }
}