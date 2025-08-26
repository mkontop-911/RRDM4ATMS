namespace RRDM4ATMsWin
{
    partial class Form67_Cycle_Rich_Picture_2
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
            this.panel4 = new System.Windows.Forms.Panel();
            this.buttonExportForAtm = new System.Windows.Forms.Button();
            this.dataGridView3 = new System.Windows.Forms.DataGridView();
            this.linkLabelUnmatched = new System.Windows.Forms.LinkLabel();
            this.linkLabelSM_LINES = new System.Windows.Forms.LinkLabel();
            this.buttonALL_Actions = new System.Windows.Forms.Button();
            this.buttonALL_ACoounting = new System.Windows.Forms.Button();
            this.labelCyclesPerATM = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.labelTRecovered = new System.Windows.Forms.Label();
            this.textBoxTRecovered = new System.Windows.Forms.TextBox();
            this.labelTRefund = new System.Windows.Forms.Label();
            this.textBoxTRefund = new System.Windows.Forms.TextBox();
            this.labelTShortage = new System.Windows.Forms.Label();
            this.textBoxTShortage = new System.Windows.Forms.TextBox();
            this.labelTExcess = new System.Windows.Forms.Label();
            this.textBoxTExcess = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.textBoxTotal = new System.Windows.Forms.TextBox();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.buttonExportToExcel = new System.Windows.Forms.Button();
            this.labelHeaderLeft = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxTotalLinesATM = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.panel1.Controls.Add(this.panel4);
            this.panel1.Controls.Add(this.labelCyclesPerATM);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.labelHeaderLeft);
            this.panel1.Location = new System.Drawing.Point(4, 11);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1272, 705);
            this.panel1.TabIndex = 0;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.White;
            this.panel4.Controls.Add(this.label1);
            this.panel4.Controls.Add(this.textBoxTotalLinesATM);
            this.panel4.Controls.Add(this.buttonExportForAtm);
            this.panel4.Controls.Add(this.dataGridView3);
            this.panel4.Controls.Add(this.linkLabelUnmatched);
            this.panel4.Controls.Add(this.linkLabelSM_LINES);
            this.panel4.Controls.Add(this.buttonALL_Actions);
            this.panel4.Controls.Add(this.buttonALL_ACoounting);
            this.panel4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.panel4.Location = new System.Drawing.Point(609, 31);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(660, 661);
            this.panel4.TabIndex = 454;
            // 
            // buttonExportForAtm
            // 
            this.buttonExportForAtm.BackColor = System.Drawing.Color.Transparent;
            this.buttonExportForAtm.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonExportForAtm.FlatAppearance.BorderSize = 2;
            this.buttonExportForAtm.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonExportForAtm.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonExportForAtm.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonExportForAtm.Location = new System.Drawing.Point(559, 603);
            this.buttonExportForAtm.Name = "buttonExportForAtm";
            this.buttonExportForAtm.Size = new System.Drawing.Size(95, 30);
            this.buttonExportForAtm.TabIndex = 504;
            this.buttonExportForAtm.Text = "Export to Excel";
            this.buttonExportForAtm.UseVisualStyleBackColor = false;
            this.buttonExportForAtm.Click += new System.EventHandler(this.buttonExportForAtm_Click);
            // 
            // dataGridView3
            // 
            this.dataGridView3.AllowUserToAddRows = false;
            this.dataGridView3.AllowUserToDeleteRows = false;
            this.dataGridView3.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView3.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView3.GridColor = System.Drawing.Color.LightGray;
            this.dataGridView3.Location = new System.Drawing.Point(5, 3);
            this.dataGridView3.MultiSelect = false;
            this.dataGridView3.Name = "dataGridView3";
            this.dataGridView3.ReadOnly = true;
            this.dataGridView3.RowHeadersVisible = false;
            this.dataGridView3.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView3.Size = new System.Drawing.Size(652, 568);
            this.dataGridView3.TabIndex = 503;
            this.dataGridView3.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView3_RowEnter);
            // 
            // linkLabelUnmatched
            // 
            this.linkLabelUnmatched.AutoSize = true;
            this.linkLabelUnmatched.Location = new System.Drawing.Point(275, 579);
            this.linkLabelUnmatched.Name = "linkLabelUnmatched";
            this.linkLabelUnmatched.Size = new System.Drawing.Size(156, 13);
            this.linkLabelUnmatched.TabIndex = 502;
            this.linkLabelUnmatched.TabStop = true;
            this.linkLabelUnmatched.Text = "link to Show UnMatched TXNS";
            this.linkLabelUnmatched.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelUnmatched_LinkClicked);
            // 
            // linkLabelSM_LINES
            // 
            this.linkLabelSM_LINES.AutoSize = true;
            this.linkLabelSM_LINES.Location = new System.Drawing.Point(275, 600);
            this.linkLabelSM_LINES.Name = "linkLabelSM_LINES";
            this.linkLabelSM_LINES.Size = new System.Drawing.Size(84, 13);
            this.linkLabelSM_LINES.TabIndex = 499;
            this.linkLabelSM_LINES.TabStop = true;
            this.linkLabelSM_LINES.Text = "Show SM Lines ";
            this.linkLabelSM_LINES.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelSM_LINES_LinkClicked);
            // 
            // buttonALL_Actions
            // 
            this.buttonALL_Actions.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonALL_Actions.FlatAppearance.BorderSize = 2;
            this.buttonALL_Actions.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonALL_Actions.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonALL_Actions.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonALL_Actions.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonALL_Actions.Location = new System.Drawing.Point(5, 576);
            this.buttonALL_Actions.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.buttonALL_Actions.Name = "buttonALL_Actions";
            this.buttonALL_Actions.Size = new System.Drawing.Size(128, 27);
            this.buttonALL_Actions.TabIndex = 440;
            this.buttonALL_Actions.Text = "All Actions This Repl";
            this.buttonALL_Actions.UseVisualStyleBackColor = true;
            this.buttonALL_Actions.Click += new System.EventHandler(this.buttonALL_Actions_Click);
            // 
            // buttonALL_ACoounting
            // 
            this.buttonALL_ACoounting.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonALL_ACoounting.FlatAppearance.BorderSize = 2;
            this.buttonALL_ACoounting.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonALL_ACoounting.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonALL_ACoounting.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonALL_ACoounting.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonALL_ACoounting.Location = new System.Drawing.Point(137, 575);
            this.buttonALL_ACoounting.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.buttonALL_ACoounting.Name = "buttonALL_ACoounting";
            this.buttonALL_ACoounting.Size = new System.Drawing.Size(129, 27);
            this.buttonALL_ACoounting.TabIndex = 441;
            this.buttonALL_ACoounting.Text = "All Accounting This Repl";
            this.buttonALL_ACoounting.UseVisualStyleBackColor = true;
            this.buttonALL_ACoounting.Click += new System.EventHandler(this.buttonALL_ACoounting_Click);
            // 
            // labelCyclesPerATM
            // 
            this.labelCyclesPerATM.AutoSize = true;
            this.labelCyclesPerATM.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.labelCyclesPerATM.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.labelCyclesPerATM.Location = new System.Drawing.Point(609, 8);
            this.labelCyclesPerATM.Name = "labelCyclesPerATM";
            this.labelCyclesPerATM.Size = new System.Drawing.Size(187, 18);
            this.labelCyclesPerATM.TabIndex = 453;
            this.labelCyclesPerATM.Text = "ALL CYCLES THIS ATM";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.labelTRecovered);
            this.panel2.Controls.Add(this.textBoxTRecovered);
            this.panel2.Controls.Add(this.labelTRefund);
            this.panel2.Controls.Add(this.textBoxTRefund);
            this.panel2.Controls.Add(this.labelTShortage);
            this.panel2.Controls.Add(this.textBoxTShortage);
            this.panel2.Controls.Add(this.labelTExcess);
            this.panel2.Controls.Add(this.textBoxTExcess);
            this.panel2.Controls.Add(this.label25);
            this.panel2.Controls.Add(this.textBoxTotal);
            this.panel2.Controls.Add(this.dataGridView2);
            this.panel2.Controls.Add(this.buttonExportToExcel);
            this.panel2.Location = new System.Drawing.Point(3, 28);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(606, 664);
            this.panel2.TabIndex = 0;
            // 
            // labelTRecovered
            // 
            this.labelTRecovered.AutoSize = true;
            this.labelTRecovered.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.labelTRecovered.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.labelTRecovered.Location = new System.Drawing.Point(203, 609);
            this.labelTRecovered.Name = "labelTRecovered";
            this.labelTRecovered.Size = new System.Drawing.Size(69, 13);
            this.labelTRecovered.TabIndex = 481;
            this.labelTRecovered.Text = "Recovered";
            // 
            // textBoxTRecovered
            // 
            this.textBoxTRecovered.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxTRecovered.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.textBoxTRecovered.ForeColor = System.Drawing.Color.Black;
            this.textBoxTRecovered.Location = new System.Drawing.Point(276, 606);
            this.textBoxTRecovered.Name = "textBoxTRecovered";
            this.textBoxTRecovered.ReadOnly = true;
            this.textBoxTRecovered.Size = new System.Drawing.Size(87, 20);
            this.textBoxTRecovered.TabIndex = 480;
            this.textBoxTRecovered.TabStop = false;
            this.textBoxTRecovered.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // labelTRefund
            // 
            this.labelTRefund.AutoSize = true;
            this.labelTRefund.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.labelTRefund.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.labelTRefund.Location = new System.Drawing.Point(211, 584);
            this.labelTRefund.Name = "labelTRefund";
            this.labelTRefund.Size = new System.Drawing.Size(48, 13);
            this.labelTRefund.TabIndex = 479;
            this.labelTRefund.Text = "Refund";
            // 
            // textBoxTRefund
            // 
            this.textBoxTRefund.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxTRefund.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.textBoxTRefund.ForeColor = System.Drawing.Color.Black;
            this.textBoxTRefund.Location = new System.Drawing.Point(276, 581);
            this.textBoxTRefund.Name = "textBoxTRefund";
            this.textBoxTRefund.ReadOnly = true;
            this.textBoxTRefund.Size = new System.Drawing.Size(87, 20);
            this.textBoxTRefund.TabIndex = 478;
            this.textBoxTRefund.TabStop = false;
            this.textBoxTRefund.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // labelTShortage
            // 
            this.labelTShortage.AutoSize = true;
            this.labelTShortage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.labelTShortage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.labelTShortage.Location = new System.Drawing.Point(12, 608);
            this.labelTShortage.Name = "labelTShortage";
            this.labelTShortage.Size = new System.Drawing.Size(91, 13);
            this.labelTShortage.TabIndex = 473;
            this.labelTShortage.Text = "Total Shortage";
            // 
            // textBoxTShortage
            // 
            this.textBoxTShortage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxTShortage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.textBoxTShortage.ForeColor = System.Drawing.Color.Black;
            this.textBoxTShortage.Location = new System.Drawing.Point(105, 605);
            this.textBoxTShortage.Name = "textBoxTShortage";
            this.textBoxTShortage.ReadOnly = true;
            this.textBoxTShortage.Size = new System.Drawing.Size(87, 20);
            this.textBoxTShortage.TabIndex = 472;
            this.textBoxTShortage.TabStop = false;
            this.textBoxTShortage.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // labelTExcess
            // 
            this.labelTExcess.AutoSize = true;
            this.labelTExcess.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.labelTExcess.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.labelTExcess.Location = new System.Drawing.Point(8, 584);
            this.labelTExcess.Name = "labelTExcess";
            this.labelTExcess.Size = new System.Drawing.Size(80, 13);
            this.labelTExcess.TabIndex = 471;
            this.labelTExcess.Text = "Total Excess";
            // 
            // textBoxTExcess
            // 
            this.textBoxTExcess.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxTExcess.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.textBoxTExcess.ForeColor = System.Drawing.Color.Black;
            this.textBoxTExcess.Location = new System.Drawing.Point(107, 581);
            this.textBoxTExcess.Name = "textBoxTExcess";
            this.textBoxTExcess.ReadOnly = true;
            this.textBoxTExcess.Size = new System.Drawing.Size(87, 20);
            this.textBoxTExcess.TabIndex = 470;
            this.textBoxTExcess.TabStop = false;
            this.textBoxTExcess.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.label25.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.label25.Location = new System.Drawing.Point(456, 581);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(70, 13);
            this.label25.TabIndex = 465;
            this.label25.Text = "Total Lines";
            // 
            // textBoxTotal
            // 
            this.textBoxTotal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.textBoxTotal.ForeColor = System.Drawing.Color.Red;
            this.textBoxTotal.Location = new System.Drawing.Point(532, 577);
            this.textBoxTotal.Name = "textBoxTotal";
            this.textBoxTotal.ReadOnly = true;
            this.textBoxTotal.Size = new System.Drawing.Size(60, 20);
            this.textBoxTotal.TabIndex = 464;
            this.textBoxTotal.TabStop = false;
            this.textBoxTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // dataGridView2
            // 
            this.dataGridView2.AllowUserToAddRows = false;
            this.dataGridView2.AllowUserToDeleteRows = false;
            this.dataGridView2.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.GridColor = System.Drawing.Color.LightGray;
            this.dataGridView2.Location = new System.Drawing.Point(5, 3);
            this.dataGridView2.MultiSelect = false;
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.ReadOnly = true;
            this.dataGridView2.RowHeadersVisible = false;
            this.dataGridView2.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView2.Size = new System.Drawing.Size(587, 572);
            this.dataGridView2.TabIndex = 455;
            this.dataGridView2.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView2_RowEnter);
            // 
            // buttonExportToExcel
            // 
            this.buttonExportToExcel.BackColor = System.Drawing.Color.Transparent;
            this.buttonExportToExcel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonExportToExcel.FlatAppearance.BorderSize = 2;
            this.buttonExportToExcel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonExportToExcel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonExportToExcel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonExportToExcel.Location = new System.Drawing.Point(497, 599);
            this.buttonExportToExcel.Name = "buttonExportToExcel";
            this.buttonExportToExcel.Size = new System.Drawing.Size(95, 30);
            this.buttonExportToExcel.TabIndex = 454;
            this.buttonExportToExcel.Text = "Export to Excel";
            this.buttonExportToExcel.UseVisualStyleBackColor = false;
            this.buttonExportToExcel.Click += new System.EventHandler(this.buttonExportToExcel_Click);
            // 
            // labelHeaderLeft
            // 
            this.labelHeaderLeft.AutoSize = true;
            this.labelHeaderLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.labelHeaderLeft.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.labelHeaderLeft.Location = new System.Drawing.Point(3, 7);
            this.labelHeaderLeft.Name = "labelHeaderLeft";
            this.labelHeaderLeft.Size = new System.Drawing.Size(467, 18);
            this.labelHeaderLeft.TabIndex = 450;
            this.labelHeaderLeft.Text = "ATMS REPLENISHMENT CYCLES WITH CIT DIFFERENCES";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.label1.Location = new System.Drawing.Point(518, 575);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 506;
            this.label1.Text = "Total Lines";
            // 
            // textBoxTotalLinesATM
            // 
            this.textBoxTotalLinesATM.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxTotalLinesATM.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.textBoxTotalLinesATM.ForeColor = System.Drawing.Color.Red;
            this.textBoxTotalLinesATM.Location = new System.Drawing.Point(594, 572);
            this.textBoxTotalLinesATM.Name = "textBoxTotalLinesATM";
            this.textBoxTotalLinesATM.ReadOnly = true;
            this.textBoxTotalLinesATM.Size = new System.Drawing.Size(60, 20);
            this.textBoxTotalLinesATM.TabIndex = 505;
            this.textBoxTotalLinesATM.TabStop = false;
            this.textBoxTotalLinesATM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // Form67_Cycle_Rich_Picture_2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(1288, 723);
            this.Controls.Add(this.panel1);
            this.Name = "Form67_Cycle_Rich_Picture_2";
            this.Text = "Form67_Cycle_Rich_Picture_2";
            this.Load += new System.EventHandler(this.Form67_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label labelHeaderLeft;
        private System.Windows.Forms.Button buttonExportToExcel;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.TextBox textBoxTotal;
        private System.Windows.Forms.Label labelTShortage;
        private System.Windows.Forms.TextBox textBoxTShortage;
        private System.Windows.Forms.Label labelTExcess;
        private System.Windows.Forms.TextBox textBoxTExcess;
        private System.Windows.Forms.Label labelTRefund;
        private System.Windows.Forms.TextBox textBoxTRefund;
        private System.Windows.Forms.Label labelTRecovered;
        private System.Windows.Forms.TextBox textBoxTRecovered;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.DataGridView dataGridView3;
        private System.Windows.Forms.LinkLabel linkLabelUnmatched;
        private System.Windows.Forms.LinkLabel linkLabelSM_LINES;
        private System.Windows.Forms.Button buttonALL_Actions;
        private System.Windows.Forms.Button buttonALL_ACoounting;
        private System.Windows.Forms.Label labelCyclesPerATM;
        private System.Windows.Forms.Button buttonExportForAtm;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxTotalLinesATM;
    }
}