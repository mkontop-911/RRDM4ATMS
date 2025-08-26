namespace RRDM4ATMsWin
{
    partial class Form55
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label14 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.controlerMSGsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.aTMSDataSet11 = new RRDM4ATMsWin.ATMSDataSet11();
            this.button2 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.ReadMsg2 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.controlerMSGsTableAdapter = new RRDM4ATMsWin.ATMSDataSet11TableAdapters.ControlerMSGsTableAdapter();
            this.ReadMsg = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.MesNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FromUser = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ToUser = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SeriousMsg = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Message = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.readMsgDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.BankId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BranchId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ToAllAtms = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.AtmNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DtTm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ExpDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OpenMsg = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Operator = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.controlerMSGsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.aTMSDataSet11)).BeginInit();
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.SuspendLayout();
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.label14.ForeColor = System.Drawing.Color.White;
            this.label14.Location = new System.Drawing.Point(12, 9);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(379, 18);
            this.label14.TabIndex = 328;
            this.label14.Text = "MESSAGES EXCHANGED WITH CONTROLLER ";
            this.label14.Click += new System.EventHandler(this.label14_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ReadMsg,
            this.MesNo,
            this.FromUser,
            this.ToUser,
            this.Type,
            this.SeriousMsg,
            this.Message,
            this.readMsgDataGridViewCheckBoxColumn,
            this.BankId,
            this.BranchId,
            this.ToAllAtms,
            this.AtmNo,
            this.DtTm,
            this.ExpDate,
            this.OpenMsg,
            this.Operator});
            this.dataGridView1.DataSource = this.controlerMSGsBindingSource;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.GridColor = System.Drawing.Color.LightGray;
            this.dataGridView1.Location = new System.Drawing.Point(3, 3);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(867, 271);
            this.dataGridView1.TabIndex = 329;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            this.dataGridView1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellDoubleClick);
            // 
            // controlerMSGsBindingSource
            // 
            this.controlerMSGsBindingSource.DataMember = "ControlerMSGs";
            this.controlerMSGsBindingSource.DataSource = this.aTMSDataSet11;
            // 
            // aTMSDataSet11
            // 
            this.aTMSDataSet11.DataSetName = "ATMSDataSet11";
            this.aTMSDataSet11.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
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
            this.button2.Location = new System.Drawing.Point(838, 369);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(62, 24);
            this.button2.TabIndex = 330;
            this.button2.Text = "Finish";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.tabControl1);
            this.panel1.ForeColor = System.Drawing.Color.Black;
            this.panel1.Location = new System.Drawing.Point(-3, 36);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(918, 327);
            this.panel1.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(18, 13);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(881, 303);
            this.tabControl1.TabIndex = 331;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dataGridView1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(873, 277);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Received";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dataGridView2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(873, 277);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Sent";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // dataGridView2
            // 
            this.dataGridView2.AllowUserToAddRows = false;
            this.dataGridView2.AllowUserToDeleteRows = false;
            this.dataGridView2.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView2.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ReadMsg2});
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView2.DefaultCellStyle = dataGridViewCellStyle8;
            this.dataGridView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView2.GridColor = System.Drawing.Color.LightGray;
            this.dataGridView2.Location = new System.Drawing.Point(3, 3);
            this.dataGridView2.MultiSelect = false;
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.ReadOnly = true;
            this.dataGridView2.RowHeadersVisible = false;
            this.dataGridView2.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView2.Size = new System.Drawing.Size(867, 271);
            this.dataGridView2.TabIndex = 330;
            this.dataGridView2.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView2_CellContentClick);
            this.dataGridView2.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView2_CellDoubleClick);
            this.dataGridView2.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView2_RowEnter);
            // 
            // ReadMsg2
            // 
            this.ReadMsg2.DataPropertyName = "ReadMsg";
            this.ReadMsg2.HeaderText = "ReadMsg";
            this.ReadMsg2.Name = "ReadMsg2";
            this.ReadMsg2.ReadOnly = true;
            this.ReadMsg2.Visible = false;
            // 
            // controlerMSGsTableAdapter
            // 
            this.controlerMSGsTableAdapter.ClearBeforeFill = true;
            // 
            // ReadMsg
            // 
            this.ReadMsg.DataPropertyName = "ReadMsg";
            this.ReadMsg.HeaderText = "ReadMsg";
            this.ReadMsg.Name = "ReadMsg";
            this.ReadMsg.ReadOnly = true;
            this.ReadMsg.Visible = false;
            this.ReadMsg.Width = 50;
            // 
            // MesNo
            // 
            this.MesNo.DataPropertyName = "MesNo";
            this.MesNo.HeaderText = "MesNo";
            this.MesNo.Name = "MesNo";
            this.MesNo.ReadOnly = true;
            this.MesNo.Width = 50;
            // 
            // FromUser
            // 
            this.FromUser.DataPropertyName = "FromUser";
            this.FromUser.HeaderText = "FromUser";
            this.FromUser.Name = "FromUser";
            this.FromUser.ReadOnly = true;
            this.FromUser.Width = 70;
            // 
            // ToUser
            // 
            this.ToUser.DataPropertyName = "ToUser";
            this.ToUser.HeaderText = "ToUser";
            this.ToUser.Name = "ToUser";
            this.ToUser.ReadOnly = true;
            this.ToUser.Width = 70;
            // 
            // Type
            // 
            this.Type.DataPropertyName = "Type";
            this.Type.HeaderText = "Type";
            this.Type.Name = "Type";
            this.Type.ReadOnly = true;
            this.Type.Width = 90;
            // 
            // SeriousMsg
            // 
            this.SeriousMsg.DataPropertyName = "SeriousMsg";
            this.SeriousMsg.HeaderText = "SeriousMsg";
            this.SeriousMsg.Name = "SeriousMsg";
            this.SeriousMsg.ReadOnly = true;
            this.SeriousMsg.Width = 65;
            // 
            // Message
            // 
            this.Message.DataPropertyName = "Message";
            this.Message.HeaderText = "Message";
            this.Message.Name = "Message";
            this.Message.ReadOnly = true;
            this.Message.Width = 150;
            // 
            // readMsgDataGridViewCheckBoxColumn
            // 
            this.readMsgDataGridViewCheckBoxColumn.DataPropertyName = "ReadMsg";
            this.readMsgDataGridViewCheckBoxColumn.HeaderText = "ReadMsg";
            this.readMsgDataGridViewCheckBoxColumn.Name = "readMsgDataGridViewCheckBoxColumn";
            this.readMsgDataGridViewCheckBoxColumn.ReadOnly = true;
            // 
            // BankId
            // 
            this.BankId.DataPropertyName = "BankId";
            this.BankId.HeaderText = "BankId";
            this.BankId.Name = "BankId";
            this.BankId.ReadOnly = true;
            this.BankId.Visible = false;
            // 
            // BranchId
            // 
            this.BranchId.DataPropertyName = "BranchId";
            this.BranchId.HeaderText = "BranchId";
            this.BranchId.Name = "BranchId";
            this.BranchId.ReadOnly = true;
            this.BranchId.Width = 60;
            // 
            // ToAllAtms
            // 
            this.ToAllAtms.DataPropertyName = "ToAllAtms";
            this.ToAllAtms.HeaderText = "ToAllAtms";
            this.ToAllAtms.Name = "ToAllAtms";
            this.ToAllAtms.ReadOnly = true;
            this.ToAllAtms.Width = 60;
            // 
            // AtmNo
            // 
            this.AtmNo.DataPropertyName = "AtmNo";
            this.AtmNo.HeaderText = "AtmNo";
            this.AtmNo.Name = "AtmNo";
            this.AtmNo.ReadOnly = true;
            this.AtmNo.Width = 50;
            // 
            // DtTm
            // 
            this.DtTm.DataPropertyName = "DtTm";
            this.DtTm.HeaderText = "DtTm";
            this.DtTm.Name = "DtTm";
            this.DtTm.ReadOnly = true;
            // 
            // ExpDate
            // 
            this.ExpDate.DataPropertyName = "ExpDate";
            this.ExpDate.HeaderText = "ExpDate";
            this.ExpDate.Name = "ExpDate";
            this.ExpDate.ReadOnly = true;
            // 
            // OpenMsg
            // 
            this.OpenMsg.DataPropertyName = "OpenMsg";
            this.OpenMsg.HeaderText = "OpenMsg";
            this.OpenMsg.Name = "OpenMsg";
            this.OpenMsg.ReadOnly = true;
            // 
            // Operator
            // 
            this.Operator.DataPropertyName = "Operator";
            this.Operator.HeaderText = "Operator";
            this.Operator.Name = "Operator";
            this.Operator.ReadOnly = true;
            // 
            // Form55
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(908, 400);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.button2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Form55";
            this.Text = "Form55";
            this.Load += new System.EventHandler(this.Form55_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.controlerMSGsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.aTMSDataSet11)).EndInit();
            this.panel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView dataGridView2;
        private ATMSDataSet11 aTMSDataSet11;
        private System.Windows.Forms.BindingSource controlerMSGsBindingSource;
        private ATMSDataSet11TableAdapters.ControlerMSGsTableAdapter controlerMSGsTableAdapter;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ReadMsg2;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ReadMsg;
        private System.Windows.Forms.DataGridViewTextBoxColumn MesNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn FromUser;
        private System.Windows.Forms.DataGridViewTextBoxColumn ToUser;
        private System.Windows.Forms.DataGridViewTextBoxColumn Type;
        private System.Windows.Forms.DataGridViewCheckBoxColumn SeriousMsg;
        private System.Windows.Forms.DataGridViewTextBoxColumn Message;
        private System.Windows.Forms.DataGridViewCheckBoxColumn readMsgDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn BankId;
        private System.Windows.Forms.DataGridViewTextBoxColumn BranchId;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ToAllAtms;
        private System.Windows.Forms.DataGridViewTextBoxColumn AtmNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn DtTm;
        private System.Windows.Forms.DataGridViewTextBoxColumn ExpDate;
        private System.Windows.Forms.DataGridViewCheckBoxColumn OpenMsg;
        private System.Windows.Forms.DataGridViewTextBoxColumn Operator;
    }
}