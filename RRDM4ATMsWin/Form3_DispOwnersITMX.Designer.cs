namespace RRDM4ATMsWin
{
    partial class Form3_DispOwnersITMX
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.textBoxMessage = new System.Windows.Forms.TextBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonAssign = new System.Windows.Forms.Button();
            this.label22 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.labelOfficerName = new System.Windows.Forms.Label();
            this.labelOfficerId = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.buttonHistory = new System.Windows.Forms.Button();
            this.labelCuurentOwnerNm = new System.Windows.Forms.Label();
            this.labelCurrentOwnerId = new System.Windows.Forms.Label();
            this.labelDispId = new System.Windows.Forms.Label();
            this.buttonDeAssign = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxMessage
            // 
            this.textBoxMessage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.textBoxMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.textBoxMessage.ForeColor = System.Drawing.Color.White;
            this.textBoxMessage.Location = new System.Drawing.Point(12, 505);
            this.textBoxMessage.Name = "textBoxMessage";
            this.textBoxMessage.ReadOnly = true;
            this.textBoxMessage.Size = new System.Drawing.Size(341, 24);
            this.textBoxMessage.TabIndex = 405;
            // 
            // buttonCancel
            // 
            this.buttonCancel.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonCancel.FlatAppearance.BorderSize = 2;
            this.buttonCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonCancel.ForeColor = System.Drawing.Color.White;
            this.buttonCancel.Location = new System.Drawing.Point(386, 510);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(2);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(76, 27);
            this.buttonCancel.TabIndex = 402;
            this.buttonCancel.Text = "Finish";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonAssign
            // 
            this.buttonAssign.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonAssign.FlatAppearance.BorderSize = 2;
            this.buttonAssign.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonAssign.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonAssign.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonAssign.ForeColor = System.Drawing.Color.White;
            this.buttonAssign.Location = new System.Drawing.Point(559, 510);
            this.buttonAssign.Margin = new System.Windows.Forms.Padding(2);
            this.buttonAssign.Name = "buttonAssign";
            this.buttonAssign.Size = new System.Drawing.Size(76, 27);
            this.buttonAssign.TabIndex = 404;
            this.buttonAssign.Text = "Assign";
            this.buttonAssign.UseVisualStyleBackColor = true;
            this.buttonAssign.Click += new System.EventHandler(this.buttonAssign_Click);
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.ForeColor = System.Drawing.Color.White;
            this.label22.Location = new System.Drawing.Point(14, 9);
            this.label22.Margin = new System.Windows.Forms.Padding(5, 0, 2, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(272, 29);
            this.label22.TabIndex = 403;
            this.label22.Text = "Assign Dispute Officer";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.label2.Location = new System.Drawing.Point(9, 113);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(130, 18);
            this.label2.TabIndex = 272;
            this.label2.Text = "Dispute Officers";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridView1.GridColor = System.Drawing.Color.LightGray;
            this.dataGridView1.Location = new System.Drawing.Point(7, 136);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(606, 191);
            this.dataGridView1.TabIndex = 3;
            this.dataGridView1.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_RowEnter);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(154, 73);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(158, 21);
            this.comboBox1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 76);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(122, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Reason of Assignment : ";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.labelOfficerName);
            this.panel2.Controls.Add(this.labelOfficerId);
            this.panel2.Controls.Add(this.comboBox1);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Location = new System.Drawing.Point(7, 350);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(606, 104);
            this.panel2.TabIndex = 273;
            // 
            // labelOfficerName
            // 
            this.labelOfficerName.AutoSize = true;
            this.labelOfficerName.Location = new System.Drawing.Point(16, 33);
            this.labelOfficerName.Name = "labelOfficerName";
            this.labelOfficerName.Size = new System.Drawing.Size(88, 13);
            this.labelOfficerName.TabIndex = 3;
            this.labelOfficerName.Text = "labelOfficerName";
            // 
            // labelOfficerId
            // 
            this.labelOfficerId.AutoSize = true;
            this.labelOfficerId.Location = new System.Drawing.Point(16, 9);
            this.labelOfficerId.Name = "labelOfficerId";
            this.labelOfficerId.Size = new System.Drawing.Size(69, 13);
            this.labelOfficerId.TabIndex = 2;
            this.labelOfficerId.Text = "labelOfficerId";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.dataGridView1);
            this.panel1.ForeColor = System.Drawing.Color.Black;
            this.panel1.Location = new System.Drawing.Point(19, 41);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(616, 464);
            this.panel1.TabIndex = 401;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.label4.Location = new System.Drawing.Point(6, 330);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(132, 18);
            this.label4.TabIndex = 276;
            this.label4.Text = "Assigned Officer";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.label3.Location = new System.Drawing.Point(6, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 18);
            this.label3.TabIndex = 275;
            this.label3.Text = "Dispute ";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.Controls.Add(this.buttonHistory);
            this.panel3.Controls.Add(this.labelCuurentOwnerNm);
            this.panel3.Controls.Add(this.labelCurrentOwnerId);
            this.panel3.Controls.Add(this.labelDispId);
            this.panel3.Location = new System.Drawing.Point(7, 28);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(610, 82);
            this.panel3.TabIndex = 274;
            // 
            // buttonHistory
            // 
            this.buttonHistory.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonHistory.FlatAppearance.BorderSize = 2;
            this.buttonHistory.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonHistory.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonHistory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonHistory.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonHistory.Location = new System.Drawing.Point(533, 50);
            this.buttonHistory.Name = "buttonHistory";
            this.buttonHistory.Size = new System.Drawing.Size(64, 24);
            this.buttonHistory.TabIndex = 336;
            this.buttonHistory.Text = "History";
            this.buttonHistory.UseVisualStyleBackColor = true;
            this.buttonHistory.Click += new System.EventHandler(this.buttonHistory_Click);
            // 
            // labelCuurentOwnerNm
            // 
            this.labelCuurentOwnerNm.AutoSize = true;
            this.labelCuurentOwnerNm.Location = new System.Drawing.Point(16, 56);
            this.labelCuurentOwnerNm.Name = "labelCuurentOwnerNm";
            this.labelCuurentOwnerNm.Size = new System.Drawing.Size(110, 13);
            this.labelCuurentOwnerNm.TabIndex = 2;
            this.labelCuurentOwnerNm.Text = "labelCurrentOwnerNm";
            // 
            // labelCurrentOwnerId
            // 
            this.labelCurrentOwnerId.AutoSize = true;
            this.labelCurrentOwnerId.Location = new System.Drawing.Point(16, 36);
            this.labelCurrentOwnerId.Name = "labelCurrentOwnerId";
            this.labelCurrentOwnerId.Size = new System.Drawing.Size(103, 13);
            this.labelCurrentOwnerId.TabIndex = 1;
            this.labelCurrentOwnerId.Text = "labelCurrentOwnerId";
            // 
            // labelDispId
            // 
            this.labelDispId.AutoSize = true;
            this.labelDispId.Location = new System.Drawing.Point(16, 9);
            this.labelDispId.Name = "labelDispId";
            this.labelDispId.Size = new System.Drawing.Size(59, 13);
            this.labelDispId.TabIndex = 0;
            this.labelDispId.Text = "labelDispId";
            // 
            // buttonDeAssign
            // 
            this.buttonDeAssign.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonDeAssign.FlatAppearance.BorderSize = 2;
            this.buttonDeAssign.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonDeAssign.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonDeAssign.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonDeAssign.ForeColor = System.Drawing.Color.White;
            this.buttonDeAssign.Location = new System.Drawing.Point(475, 510);
            this.buttonDeAssign.Margin = new System.Windows.Forms.Padding(2);
            this.buttonDeAssign.Name = "buttonDeAssign";
            this.buttonDeAssign.Size = new System.Drawing.Size(76, 27);
            this.buttonDeAssign.TabIndex = 406;
            this.buttonDeAssign.Text = "DeAssign";
            this.buttonDeAssign.UseVisualStyleBackColor = true;
            this.buttonDeAssign.Click += new System.EventHandler(this.buttonDeAssign_Click);
            // 
            // Form3_DispOwnersITMX
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(643, 543);
            this.Controls.Add(this.buttonDeAssign);
            this.Controls.Add(this.textBoxMessage);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonAssign);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.panel1);
            this.Name = "Form3_DispOwnersITMX";
            this.Text = "Form3_DispOwnersNAITMX";
            this.Load += new System.EventHandler(this.Form3DispOwners_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxMessage;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonAssign;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelOfficerName;
        private System.Windows.Forms.Label labelOfficerId;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label labelCuurentOwnerNm;
        private System.Windows.Forms.Label labelCurrentOwnerId;
        private System.Windows.Forms.Label labelDispId;
        private System.Windows.Forms.Button buttonHistory;
        private System.Windows.Forms.Button buttonDeAssign;
    }
}