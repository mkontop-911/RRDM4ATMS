namespace RRDM4ATMsWin
{
    partial class Form271FastTrack
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.buttonSourceRecords = new System.Windows.Forms.Button();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.textBoxTotalChecked = new System.Windows.Forms.TextBox();
            this.textBox13 = new System.Windows.Forms.TextBox();
            this.textBoxTotalUnchecked = new System.Windows.Forms.TextBox();
            this.label33 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox11 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.buttonUnDo = new System.Windows.Forms.Button();
            this.radioButtonCreateDefault = new System.Windows.Forms.RadioButton();
            this.Update = new System.Windows.Forms.Button();
            this.comboBoxReason = new System.Windows.Forms.ComboBox();
            this.labelReason = new System.Windows.Forms.Label();
            this.radioButtonForceMatching = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.checkBoxAll = new System.Windows.Forms.CheckBox();
            this.checkBoxUnAll = new System.Windows.Forms.CheckBox();
            this.checkBoxAllWithFirstZero = new System.Windows.Forms.CheckBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.labelFastTrack = new System.Windows.Forms.Label();
            this.buttonFinish = new System.Windows.Forms.Button();
            this.labelView = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Location = new System.Drawing.Point(12, 54);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1260, 628);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.buttonSourceRecords);
            this.panel2.Controls.Add(this.panel5);
            this.panel2.Controls.Add(this.button1);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.label18);
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Controls.Add(this.linkLabel1);
            this.panel2.Controls.Add(this.dataGridView1);
            this.panel2.Location = new System.Drawing.Point(6, 6);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1251, 618);
            this.panel2.TabIndex = 2;
            // 
            // buttonSourceRecords
            // 
            this.buttonSourceRecords.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonSourceRecords.FlatAppearance.BorderSize = 2;
            this.buttonSourceRecords.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonSourceRecords.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonSourceRecords.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSourceRecords.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonSourceRecords.Location = new System.Drawing.Point(1123, 136);
            this.buttonSourceRecords.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.buttonSourceRecords.Name = "buttonSourceRecords";
            this.buttonSourceRecords.Size = new System.Drawing.Size(75, 40);
            this.buttonSourceRecords.TabIndex = 446;
            this.buttonSourceRecords.Text = "Source Records";
            this.buttonSourceRecords.UseVisualStyleBackColor = true;
            this.buttonSourceRecords.Click += new System.EventHandler(this.buttonSourceRecords_Click);
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.label1);
            this.panel5.Controls.Add(this.label21);
            this.panel5.Controls.Add(this.textBoxTotalChecked);
            this.panel5.Controls.Add(this.textBox13);
            this.panel5.Controls.Add(this.textBoxTotalUnchecked);
            this.panel5.Controls.Add(this.label33);
            this.panel5.Controls.Add(this.label2);
            this.panel5.Controls.Add(this.textBox11);
            this.panel5.Location = new System.Drawing.Point(845, 516);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(353, 67);
            this.panel5.TabIndex = 445;
            this.panel5.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 433;
            this.label1.Text = "Total Checked";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.ForeColor = System.Drawing.Color.Black;
            this.label21.Location = new System.Drawing.Point(204, 44);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(25, 13);
            this.label21.TabIndex = 443;
            this.label21.Text = "Amt";
            // 
            // textBoxTotalChecked
            // 
            this.textBoxTotalChecked.Location = new System.Drawing.Point(112, 9);
            this.textBoxTotalChecked.Name = "textBoxTotalChecked";
            this.textBoxTotalChecked.ReadOnly = true;
            this.textBoxTotalChecked.Size = new System.Drawing.Size(70, 20);
            this.textBoxTotalChecked.TabIndex = 431;
            // 
            // textBox13
            // 
            this.textBox13.Location = new System.Drawing.Point(244, 41);
            this.textBox13.Name = "textBox13";
            this.textBox13.ReadOnly = true;
            this.textBox13.Size = new System.Drawing.Size(78, 20);
            this.textBox13.TabIndex = 444;
            this.textBox13.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBoxTotalUnchecked
            // 
            this.textBoxTotalUnchecked.Location = new System.Drawing.Point(112, 41);
            this.textBoxTotalUnchecked.Name = "textBoxTotalUnchecked";
            this.textBoxTotalUnchecked.ReadOnly = true;
            this.textBoxTotalUnchecked.Size = new System.Drawing.Size(70, 20);
            this.textBoxTotalUnchecked.TabIndex = 434;
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.ForeColor = System.Drawing.Color.Black;
            this.label33.Location = new System.Drawing.Point(204, 15);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(34, 13);
            this.label33.TabIndex = 441;
            this.label33.Text = "TXNs";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 13);
            this.label2.TabIndex = 435;
            this.label2.Text = "Total Un-Checked";
            // 
            // textBox11
            // 
            this.textBox11.Location = new System.Drawing.Point(244, 12);
            this.textBox11.Name = "textBox11";
            this.textBox11.ReadOnly = true;
            this.textBox11.Size = new System.Drawing.Size(60, 20);
            this.textBox11.TabIndex = 442;
            this.textBox11.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Transparent;
            this.button1.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.button1.FlatAppearance.BorderSize = 2;
            this.button1.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.button1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.button1.Location = new System.Drawing.Point(845, 482);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(107, 28);
            this.button1.TabIndex = 440;
            this.button1.Text = "Refresh Totals";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.label4.Location = new System.Drawing.Point(842, 170);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 16);
            this.label4.TabIndex = 439;
            this.label4.Text = "Action";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.label18.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.label18.Location = new System.Drawing.Point(842, 22);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(73, 16);
            this.label18.TabIndex = 438;
            this.label18.Text = "Selection";
            // 
            // panel4
            // 
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.buttonUnDo);
            this.panel4.Controls.Add(this.radioButtonCreateDefault);
            this.panel4.Controls.Add(this.Update);
            this.panel4.Controls.Add(this.comboBoxReason);
            this.panel4.Controls.Add(this.labelReason);
            this.panel4.Controls.Add(this.radioButtonForceMatching);
            this.panel4.Location = new System.Drawing.Point(845, 189);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(353, 238);
            this.panel4.TabIndex = 438;
            // 
            // buttonUnDo
            // 
            this.buttonUnDo.BackColor = System.Drawing.Color.Transparent;
            this.buttonUnDo.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonUnDo.FlatAppearance.BorderSize = 2;
            this.buttonUnDo.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.buttonUnDo.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonUnDo.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonUnDo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonUnDo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonUnDo.Location = new System.Drawing.Point(213, 193);
            this.buttonUnDo.Name = "buttonUnDo";
            this.buttonUnDo.Size = new System.Drawing.Size(107, 28);
            this.buttonUnDo.TabIndex = 428;
            this.buttonUnDo.Text = "Undo";
            this.buttonUnDo.UseVisualStyleBackColor = false;
            this.buttonUnDo.Click += new System.EventHandler(this.buttonUnDo_Click);
            // 
            // radioButtonCreateDefault
            // 
            this.radioButtonCreateDefault.AutoSize = true;
            this.radioButtonCreateDefault.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.radioButtonCreateDefault.Location = new System.Drawing.Point(14, 41);
            this.radioButtonCreateDefault.Name = "radioButtonCreateDefault";
            this.radioButtonCreateDefault.Size = new System.Drawing.Size(199, 17);
            this.radioButtonCreateDefault.TabIndex = 426;
            this.radioButtonCreateDefault.TabStop = true;
            this.radioButtonCreateDefault.Text = "Create Default Meta Exception";
            this.radioButtonCreateDefault.UseVisualStyleBackColor = true;
            // 
            // Update
            // 
            this.Update.BackColor = System.Drawing.Color.Transparent;
            this.Update.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.Update.FlatAppearance.BorderSize = 2;
            this.Update.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.Update.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.Update.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.Update.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Update.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.Update.Location = new System.Drawing.Point(213, 159);
            this.Update.Name = "Update";
            this.Update.Size = new System.Drawing.Size(107, 28);
            this.Update.TabIndex = 357;
            this.Update.Text = "Proceed";
            this.Update.UseVisualStyleBackColor = false;
            this.Update.Click += new System.EventHandler(this.Update_Click);
            // 
            // comboBoxReason
            // 
            this.comboBoxReason.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxReason.FormattingEnabled = true;
            this.comboBoxReason.Location = new System.Drawing.Point(10, 106);
            this.comboBoxReason.Name = "comboBoxReason";
            this.comboBoxReason.Size = new System.Drawing.Size(221, 21);
            this.comboBoxReason.TabIndex = 423;
            this.comboBoxReason.Visible = false;
            // 
            // labelReason
            // 
            this.labelReason.AutoSize = true;
            this.labelReason.Location = new System.Drawing.Point(11, 90);
            this.labelReason.Name = "labelReason";
            this.labelReason.Size = new System.Drawing.Size(71, 13);
            this.labelReason.TabIndex = 424;
            this.labelReason.Text = "Reason (714)";
            this.labelReason.Visible = false;
            // 
            // radioButtonForceMatching
            // 
            this.radioButtonForceMatching.AutoSize = true;
            this.radioButtonForceMatching.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.radioButtonForceMatching.Location = new System.Drawing.Point(14, 17);
            this.radioButtonForceMatching.Name = "radioButtonForceMatching";
            this.radioButtonForceMatching.Size = new System.Drawing.Size(113, 17);
            this.radioButtonForceMatching.TabIndex = 427;
            this.radioButtonForceMatching.TabStop = true;
            this.radioButtonForceMatching.Text = "Force Matching";
            this.radioButtonForceMatching.UseVisualStyleBackColor = true;
            this.radioButtonForceMatching.CheckedChanged += new System.EventHandler(this.radioButtonForceMatching_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.label3.Location = new System.Drawing.Point(22, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(167, 18);
            this.label3.TabIndex = 437;
            this.label3.Text = "UN MATCHED TXNS";
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.checkBoxAll);
            this.panel3.Controls.Add(this.checkBoxUnAll);
            this.panel3.Controls.Add(this.checkBoxAllWithFirstZero);
            this.panel3.Location = new System.Drawing.Point(845, 41);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(353, 91);
            this.panel3.TabIndex = 436;
            // 
            // checkBoxAll
            // 
            this.checkBoxAll.AutoSize = true;
            this.checkBoxAll.Location = new System.Drawing.Point(18, 25);
            this.checkBoxAll.Name = "checkBoxAll";
            this.checkBoxAll.Size = new System.Drawing.Size(71, 17);
            this.checkBoxAll.TabIndex = 428;
            this.checkBoxAll.Text = "Check All";
            this.checkBoxAll.UseVisualStyleBackColor = true;
            this.checkBoxAll.CheckedChanged += new System.EventHandler(this.checkBoxAll_CheckedChanged);
            // 
            // checkBoxUnAll
            // 
            this.checkBoxUnAll.AutoSize = true;
            this.checkBoxUnAll.Location = new System.Drawing.Point(154, 25);
            this.checkBoxUnAll.Name = "checkBoxUnAll";
            this.checkBoxUnAll.Size = new System.Drawing.Size(88, 17);
            this.checkBoxUnAll.TabIndex = 429;
            this.checkBoxUnAll.Text = "Un-Check All";
            this.checkBoxUnAll.UseVisualStyleBackColor = true;
            this.checkBoxUnAll.CheckedChanged += new System.EventHandler(this.checkBoxUnAll_CheckedChanged);
            // 
            // checkBoxAllWithFirstZero
            // 
            this.checkBoxAllWithFirstZero.AutoSize = true;
            this.checkBoxAllWithFirstZero.Location = new System.Drawing.Point(18, 48);
            this.checkBoxAllWithFirstZero.Name = "checkBoxAllWithFirstZero";
            this.checkBoxAllWithFirstZero.Size = new System.Drawing.Size(279, 17);
            this.checkBoxAllWithFirstZero.TabIndex = 430;
            this.checkBoxAllWithFirstZero.Text = "Check All with Not In Journal But OK in other (eg 011)";
            this.checkBoxAllWithFirstZero.UseVisualStyleBackColor = true;
            this.checkBoxAllWithFirstZero.CheckedChanged += new System.EventHandler(this.checkBoxAllWithFirstZero_CheckedChanged);
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(692, 25);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(101, 13);
            this.linkLabel1.TabIndex = 356;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Matched This Cycle";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.GridColor = System.Drawing.Color.LightGray;
            this.dataGridView1.Location = new System.Drawing.Point(19, 41);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(765, 574);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_RowEnter);
            // 
            // labelFastTrack
            // 
            this.labelFastTrack.AutoSize = true;
            this.labelFastTrack.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.labelFastTrack.ForeColor = System.Drawing.Color.White;
            this.labelFastTrack.Location = new System.Drawing.Point(13, 13);
            this.labelFastTrack.Name = "labelFastTrack";
            this.labelFastTrack.Size = new System.Drawing.Size(257, 26);
            this.labelFastTrack.TabIndex = 1;
            this.labelFastTrack.Text = "FAST TRACK ENABLER";
            // 
            // buttonFinish
            // 
            this.buttonFinish.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonFinish.FlatAppearance.BorderSize = 2;
            this.buttonFinish.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonFinish.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonFinish.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonFinish.ForeColor = System.Drawing.Color.White;
            this.buttonFinish.Location = new System.Drawing.Point(1201, 689);
            this.buttonFinish.Margin = new System.Windows.Forms.Padding(2, 2, 10, 2);
            this.buttonFinish.Name = "buttonFinish";
            this.buttonFinish.Size = new System.Drawing.Size(71, 32);
            this.buttonFinish.TabIndex = 344;
            this.buttonFinish.Text = "Finish";
            this.buttonFinish.UseVisualStyleBackColor = true;
            this.buttonFinish.Click += new System.EventHandler(this.buttonFinish_Click);
            // 
            // labelView
            // 
            this.labelView.AutoSize = true;
            this.labelView.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.labelView.ForeColor = System.Drawing.Color.White;
            this.labelView.Location = new System.Drawing.Point(15, 697);
            this.labelView.Name = "labelView";
            this.labelView.Size = new System.Drawing.Size(103, 18);
            this.labelView.TabIndex = 446;
            this.labelView.Text = "VIEW ONLY ";
            this.labelView.Visible = false;
            // 
            // Form271FastTrack
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(1284, 729);
            this.Controls.Add(this.labelView);
            this.Controls.Add(this.buttonFinish);
            this.Controls.Add(this.labelFastTrack);
            this.Controls.Add(this.panel1);
            this.Name = "Form271FastTrack";
            this.Text = "Form271FastTrack";
            this.Load += new System.EventHandler(this.Form271FastTrack_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label labelFastTrack;
        private System.Windows.Forms.Button buttonFinish;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button Update;
        private System.Windows.Forms.Label labelReason;
        private System.Windows.Forms.ComboBox comboBoxReason;
        private System.Windows.Forms.RadioButton radioButtonForceMatching;
        private System.Windows.Forms.RadioButton radioButtonCreateDefault;
        private System.Windows.Forms.CheckBox checkBoxAll;
        private System.Windows.Forms.CheckBox checkBoxUnAll;
        private System.Windows.Forms.CheckBox checkBoxAllWithFirstZero;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxTotalUnchecked;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxTotalChecked;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button buttonUnDo;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox textBox13;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.TextBox textBox11;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label labelView;
        private System.Windows.Forms.Button buttonSourceRecords;
    }
}