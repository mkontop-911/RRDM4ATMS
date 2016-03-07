namespace RRDM4ATMsWin
{
    partial class Form110
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.authoriserDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.authorNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.typeOfAuthDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.openRecordDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.userIdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.seqNumberDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dateOfInsertDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dateOfCloseDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.operatorDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.userVsAuthorizersBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.aTMSDataSet36 = new RRDM4ATMsWin.ATMSDataSet36();
            this.label22 = new System.Windows.Forms.Label();
            this.buttonRemote = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.userVsAuthorizersTableAdapter = new RRDM4ATMsWin.ATMSDataSet36TableAdapters.UserVsAuthorizersTableAdapter();
            this.textBoxMessage = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.userVsAuthorizersBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.aTMSDataSet36)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.dataGridView1);
            this.panel1.ForeColor = System.Drawing.Color.Black;
            this.panel1.Location = new System.Drawing.Point(12, 51);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(509, 300);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.comboBox1);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Location = new System.Drawing.Point(7, 250);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(499, 47);
            this.panel2.TabIndex = 273;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(163, 13);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Reason of Transfer : ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.label2.Location = new System.Drawing.Point(4, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(126, 18);
            this.label2.TabIndex = 272;
            this.label2.Text = "AUTHORISERS";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.authoriserDataGridViewTextBoxColumn,
            this.authorNameDataGridViewTextBoxColumn,
            this.typeOfAuthDataGridViewTextBoxColumn,
            this.openRecordDataGridViewCheckBoxColumn,
            this.userIdDataGridViewTextBoxColumn,
            this.seqNumberDataGridViewTextBoxColumn,
            this.dateOfInsertDataGridViewTextBoxColumn,
            this.dateOfCloseDataGridViewTextBoxColumn,
            this.operatorDataGridViewTextBoxColumn});
            this.dataGridView1.DataSource = this.userVsAuthorizersBindingSource;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.GridColor = System.Drawing.Color.LightGray;
            this.dataGridView1.Location = new System.Drawing.Point(7, 38);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(499, 208);
            this.dataGridView1.TabIndex = 3;
            this.dataGridView1.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_RowEnter);
            // 
            // authoriserDataGridViewTextBoxColumn
            // 
            this.authoriserDataGridViewTextBoxColumn.DataPropertyName = "Authoriser";
            this.authoriserDataGridViewTextBoxColumn.HeaderText = "Authoriser";
            this.authoriserDataGridViewTextBoxColumn.Name = "authoriserDataGridViewTextBoxColumn";
            this.authoriserDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // authorNameDataGridViewTextBoxColumn
            // 
            this.authorNameDataGridViewTextBoxColumn.DataPropertyName = "AuthorName";
            this.authorNameDataGridViewTextBoxColumn.HeaderText = "AuthorName";
            this.authorNameDataGridViewTextBoxColumn.Name = "authorNameDataGridViewTextBoxColumn";
            this.authorNameDataGridViewTextBoxColumn.ReadOnly = true;
            this.authorNameDataGridViewTextBoxColumn.Width = 120;
            // 
            // typeOfAuthDataGridViewTextBoxColumn
            // 
            this.typeOfAuthDataGridViewTextBoxColumn.DataPropertyName = "TypeOfAuth";
            this.typeOfAuthDataGridViewTextBoxColumn.HeaderText = "TypeOfAuth";
            this.typeOfAuthDataGridViewTextBoxColumn.Name = "typeOfAuthDataGridViewTextBoxColumn";
            this.typeOfAuthDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // openRecordDataGridViewCheckBoxColumn
            // 
            this.openRecordDataGridViewCheckBoxColumn.DataPropertyName = "OpenRecord";
            this.openRecordDataGridViewCheckBoxColumn.HeaderText = "OpenRecord";
            this.openRecordDataGridViewCheckBoxColumn.Name = "openRecordDataGridViewCheckBoxColumn";
            this.openRecordDataGridViewCheckBoxColumn.ReadOnly = true;
            // 
            // userIdDataGridViewTextBoxColumn
            // 
            this.userIdDataGridViewTextBoxColumn.DataPropertyName = "UserId";
            this.userIdDataGridViewTextBoxColumn.HeaderText = "UserId";
            this.userIdDataGridViewTextBoxColumn.Name = "userIdDataGridViewTextBoxColumn";
            this.userIdDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // seqNumberDataGridViewTextBoxColumn
            // 
            this.seqNumberDataGridViewTextBoxColumn.DataPropertyName = "SeqNumber";
            this.seqNumberDataGridViewTextBoxColumn.HeaderText = "SeqNumber";
            this.seqNumberDataGridViewTextBoxColumn.Name = "seqNumberDataGridViewTextBoxColumn";
            this.seqNumberDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // dateOfInsertDataGridViewTextBoxColumn
            // 
            this.dateOfInsertDataGridViewTextBoxColumn.DataPropertyName = "DateOfInsert";
            this.dateOfInsertDataGridViewTextBoxColumn.HeaderText = "DateOfInsert";
            this.dateOfInsertDataGridViewTextBoxColumn.Name = "dateOfInsertDataGridViewTextBoxColumn";
            this.dateOfInsertDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // dateOfCloseDataGridViewTextBoxColumn
            // 
            this.dateOfCloseDataGridViewTextBoxColumn.DataPropertyName = "DateOfClose";
            this.dateOfCloseDataGridViewTextBoxColumn.HeaderText = "DateOfClose";
            this.dateOfCloseDataGridViewTextBoxColumn.Name = "dateOfCloseDataGridViewTextBoxColumn";
            this.dateOfCloseDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // operatorDataGridViewTextBoxColumn
            // 
            this.operatorDataGridViewTextBoxColumn.DataPropertyName = "Operator";
            this.operatorDataGridViewTextBoxColumn.HeaderText = "Operator";
            this.operatorDataGridViewTextBoxColumn.Name = "operatorDataGridViewTextBoxColumn";
            this.operatorDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // userVsAuthorizersBindingSource
            // 
            this.userVsAuthorizersBindingSource.DataMember = "UserVsAuthorizers";
            this.userVsAuthorizersBindingSource.DataSource = this.aTMSDataSet36;
            // 
            // aTMSDataSet36
            // 
            this.aTMSDataSet36.DataSetName = "ATMSDataSet36";
            this.aTMSDataSet36.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.ForeColor = System.Drawing.Color.White;
            this.label22.Location = new System.Drawing.Point(14, 7);
            this.label22.Margin = new System.Windows.Forms.Padding(5, 0, 2, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(172, 29);
            this.label22.TabIndex = 392;
            this.label22.Text = "Authorisation ";
            // 
            // buttonRemote
            // 
            this.buttonRemote.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonRemote.FlatAppearance.BorderSize = 2;
            this.buttonRemote.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonRemote.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonRemote.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonRemote.ForeColor = System.Drawing.Color.White;
            this.buttonRemote.Location = new System.Drawing.Point(442, 368);
            this.buttonRemote.Margin = new System.Windows.Forms.Padding(2);
            this.buttonRemote.Name = "buttonRemote";
            this.buttonRemote.Size = new System.Drawing.Size(76, 27);
            this.buttonRemote.TabIndex = 393;
            this.buttonRemote.Text = "Remote";
            this.buttonRemote.UseVisualStyleBackColor = true;
            this.buttonRemote.Click += new System.EventHandler(this.buttonRemote_Click);
            // 
            // button1
            // 
            this.button1.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.button1.FlatAppearance.BorderSize = 2;
            this.button1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(358, 368);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(76, 27);
            this.button1.TabIndex = 247;
            this.button1.Text = "Local";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // userVsAuthorizersTableAdapter
            // 
            this.userVsAuthorizersTableAdapter.ClearBeforeFill = true;
            // 
            // textBoxMessage
            // 
            this.textBoxMessage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.textBoxMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.textBoxMessage.ForeColor = System.Drawing.Color.White;
            this.textBoxMessage.Location = new System.Drawing.Point(12, 368);
            this.textBoxMessage.Name = "textBoxMessage";
            this.textBoxMessage.ReadOnly = true;
            this.textBoxMessage.Size = new System.Drawing.Size(341, 24);
            this.textBoxMessage.TabIndex = 400;
            // 
            // Form110
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(548, 400);
            this.Controls.Add(this.textBoxMessage);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.buttonRemote);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.panel1);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "Form110";
            this.Text = "Form110";
            this.Load += new System.EventHandler(this.Form110_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.userVsAuthorizersBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.aTMSDataSet36)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Button buttonRemote;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label2;
        private ATMSDataSet36 aTMSDataSet36;
        private System.Windows.Forms.BindingSource userVsAuthorizersBindingSource;
        private ATMSDataSet36TableAdapters.UserVsAuthorizersTableAdapter userVsAuthorizersTableAdapter;
        private System.Windows.Forms.DataGridViewTextBoxColumn authoriserDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn authorNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn typeOfAuthDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn openRecordDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn userIdDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn seqNumberDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dateOfInsertDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dateOfCloseDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn operatorDataGridViewTextBoxColumn;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxMessage;
    }
}