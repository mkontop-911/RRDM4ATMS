namespace RRDM4ATMsWin
{
    partial class Form112
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxCommnet = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.Descr = new System.Windows.Forms.Label();
            this.LabelStage = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.seqNumberDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.requestorDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.authoriserDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.originDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tranNoDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.atmNoDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.disputeNumberDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.disputeTransactionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.replCycleDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.authDecisionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.authCommentDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dateOriginatedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dateAuthorisedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.stageDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.transferedDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.transferedDateDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.reasonOfTransferDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.openRecordDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.operatorDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.authorizationTableBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.aTMSDataSet40 = new RRDM4ATMsWin.ATMSDataSet40();
            this.label22 = new System.Windows.Forms.Label();
            this.buttonNext = new System.Windows.Forms.Button();
            this.textBoxMessage = new System.Windows.Forms.TextBox();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonTransfer = new System.Windows.Forms.Button();
            this.authorizationTableTableAdapter = new RRDM4ATMsWin.ATMSDataSet40TableAdapters.AuthorizationTableTableAdapter();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.authorizationTableBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.aTMSDataSet40)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.textBoxCommnet);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.Descr);
            this.panel1.Controls.Add(this.LabelStage);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.dataGridView1);
            this.panel1.ForeColor = System.Drawing.Color.Black;
            this.panel1.Location = new System.Drawing.Point(24, 52);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(681, 407);
            this.panel1.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.label6.Location = new System.Drawing.Point(12, 366);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(81, 18);
            this.label6.TabIndex = 13;
            this.label6.Text = "Comment";
            // 
            // textBoxCommnet
            // 
            this.textBoxCommnet.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.textBoxCommnet.ForeColor = System.Drawing.Color.Black;
            this.textBoxCommnet.Location = new System.Drawing.Point(111, 345);
            this.textBoxCommnet.Multiline = true;
            this.textBoxCommnet.Name = "textBoxCommnet";
            this.textBoxCommnet.ReadOnly = true;
            this.textBoxCommnet.Size = new System.Drawing.Size(552, 51);
            this.textBoxCommnet.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.label5.Location = new System.Drawing.Point(12, 348);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(85, 18);
            this.label5.TabIndex = 11;
            this.label5.Text = "Authoriser";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.label4.Location = new System.Drawing.Point(12, 299);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(112, 18);
            this.label4.TabIndex = 9;
            this.label4.Text = "Date Created ";
            // 
            // Descr
            // 
            this.Descr.AutoSize = true;
            this.Descr.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.Descr.Location = new System.Drawing.Point(12, 233);
            this.Descr.Name = "Descr";
            this.Descr.Size = new System.Drawing.Size(53, 18);
            this.Descr.TabIndex = 8;
            this.Descr.Text = "Descr";
            // 
            // LabelStage
            // 
            this.LabelStage.AutoSize = true;
            this.LabelStage.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.LabelStage.Location = new System.Drawing.Point(12, 319);
            this.LabelStage.Name = "LabelStage";
            this.LabelStage.Size = new System.Drawing.Size(51, 18);
            this.LabelStage.TabIndex = 7;
            this.LabelStage.Text = "Stage";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.label2.Location = new System.Drawing.Point(12, 278);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 18);
            this.label2.TabIndex = 6;
            this.label2.Text = "Authoriser";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.label1.Location = new System.Drawing.Point(12, 256);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 18);
            this.label1.TabIndex = 5;
            this.label1.Text = "Requestor";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoGenerateColumns = false;
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
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.seqNumberDataGridViewTextBoxColumn,
            this.requestorDataGridViewTextBoxColumn,
            this.authoriserDataGridViewTextBoxColumn,
            this.originDataGridViewTextBoxColumn,
            this.tranNoDataGridViewTextBoxColumn,
            this.atmNoDataGridViewTextBoxColumn,
            this.disputeNumberDataGridViewTextBoxColumn,
            this.disputeTransactionDataGridViewTextBoxColumn,
            this.replCycleDataGridViewTextBoxColumn,
            this.authDecisionDataGridViewTextBoxColumn,
            this.authCommentDataGridViewTextBoxColumn,
            this.dateOriginatedDataGridViewTextBoxColumn,
            this.dateAuthorisedDataGridViewTextBoxColumn,
            this.stageDataGridViewTextBoxColumn,
            this.transferedDataGridViewCheckBoxColumn,
            this.transferedDateDataGridViewTextBoxColumn,
            this.reasonOfTransferDataGridViewTextBoxColumn,
            this.openRecordDataGridViewCheckBoxColumn,
            this.operatorDataGridViewTextBoxColumn});
            this.dataGridView1.DataSource = this.authorizationTableBindingSource;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridView1.GridColor = System.Drawing.Color.LightGray;
            this.dataGridView1.Location = new System.Drawing.Point(15, 17);
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
            this.dataGridView1.Size = new System.Drawing.Size(648, 202);
            this.dataGridView1.TabIndex = 4;
            this.dataGridView1.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_RowEnter);
            // 
            // seqNumberDataGridViewTextBoxColumn
            // 
            this.seqNumberDataGridViewTextBoxColumn.DataPropertyName = "SeqNumber";
            this.seqNumberDataGridViewTextBoxColumn.HeaderText = "SeqNumber";
            this.seqNumberDataGridViewTextBoxColumn.Name = "seqNumberDataGridViewTextBoxColumn";
            this.seqNumberDataGridViewTextBoxColumn.ReadOnly = true;
            this.seqNumberDataGridViewTextBoxColumn.Width = 80;
            // 
            // requestorDataGridViewTextBoxColumn
            // 
            this.requestorDataGridViewTextBoxColumn.DataPropertyName = "Requestor";
            this.requestorDataGridViewTextBoxColumn.HeaderText = "Requestor";
            this.requestorDataGridViewTextBoxColumn.Name = "requestorDataGridViewTextBoxColumn";
            this.requestorDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // authoriserDataGridViewTextBoxColumn
            // 
            this.authoriserDataGridViewTextBoxColumn.DataPropertyName = "Authoriser";
            this.authoriserDataGridViewTextBoxColumn.HeaderText = "Authoriser";
            this.authoriserDataGridViewTextBoxColumn.Name = "authoriserDataGridViewTextBoxColumn";
            this.authoriserDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // originDataGridViewTextBoxColumn
            // 
            this.originDataGridViewTextBoxColumn.DataPropertyName = "Origin";
            this.originDataGridViewTextBoxColumn.HeaderText = "Origin";
            this.originDataGridViewTextBoxColumn.Name = "originDataGridViewTextBoxColumn";
            this.originDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // tranNoDataGridViewTextBoxColumn
            // 
            this.tranNoDataGridViewTextBoxColumn.DataPropertyName = "TranNo";
            this.tranNoDataGridViewTextBoxColumn.HeaderText = "TranNo";
            this.tranNoDataGridViewTextBoxColumn.Name = "tranNoDataGridViewTextBoxColumn";
            this.tranNoDataGridViewTextBoxColumn.ReadOnly = true;
            this.tranNoDataGridViewTextBoxColumn.Width = 70;
            // 
            // atmNoDataGridViewTextBoxColumn
            // 
            this.atmNoDataGridViewTextBoxColumn.DataPropertyName = "AtmNo";
            this.atmNoDataGridViewTextBoxColumn.HeaderText = "AtmNo";
            this.atmNoDataGridViewTextBoxColumn.Name = "atmNoDataGridViewTextBoxColumn";
            this.atmNoDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // disputeNumberDataGridViewTextBoxColumn
            // 
            this.disputeNumberDataGridViewTextBoxColumn.DataPropertyName = "DisputeNumber";
            this.disputeNumberDataGridViewTextBoxColumn.HeaderText = "DisputeNumber";
            this.disputeNumberDataGridViewTextBoxColumn.Name = "disputeNumberDataGridViewTextBoxColumn";
            this.disputeNumberDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // disputeTransactionDataGridViewTextBoxColumn
            // 
            this.disputeTransactionDataGridViewTextBoxColumn.DataPropertyName = "DisputeTransaction";
            this.disputeTransactionDataGridViewTextBoxColumn.HeaderText = "DisputeTransaction";
            this.disputeTransactionDataGridViewTextBoxColumn.Name = "disputeTransactionDataGridViewTextBoxColumn";
            this.disputeTransactionDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // replCycleDataGridViewTextBoxColumn
            // 
            this.replCycleDataGridViewTextBoxColumn.DataPropertyName = "ReplCycle";
            this.replCycleDataGridViewTextBoxColumn.HeaderText = "ReplCycle";
            this.replCycleDataGridViewTextBoxColumn.Name = "replCycleDataGridViewTextBoxColumn";
            this.replCycleDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // authDecisionDataGridViewTextBoxColumn
            // 
            this.authDecisionDataGridViewTextBoxColumn.DataPropertyName = "AuthDecision";
            this.authDecisionDataGridViewTextBoxColumn.HeaderText = "AuthDecision";
            this.authDecisionDataGridViewTextBoxColumn.Name = "authDecisionDataGridViewTextBoxColumn";
            this.authDecisionDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // authCommentDataGridViewTextBoxColumn
            // 
            this.authCommentDataGridViewTextBoxColumn.DataPropertyName = "AuthComment";
            this.authCommentDataGridViewTextBoxColumn.HeaderText = "AuthComment";
            this.authCommentDataGridViewTextBoxColumn.Name = "authCommentDataGridViewTextBoxColumn";
            this.authCommentDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // dateOriginatedDataGridViewTextBoxColumn
            // 
            this.dateOriginatedDataGridViewTextBoxColumn.DataPropertyName = "DateOriginated";
            this.dateOriginatedDataGridViewTextBoxColumn.HeaderText = "DateOriginated";
            this.dateOriginatedDataGridViewTextBoxColumn.Name = "dateOriginatedDataGridViewTextBoxColumn";
            this.dateOriginatedDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // dateAuthorisedDataGridViewTextBoxColumn
            // 
            this.dateAuthorisedDataGridViewTextBoxColumn.DataPropertyName = "DateAuthorised";
            this.dateAuthorisedDataGridViewTextBoxColumn.HeaderText = "DateAuthorised";
            this.dateAuthorisedDataGridViewTextBoxColumn.Name = "dateAuthorisedDataGridViewTextBoxColumn";
            this.dateAuthorisedDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // stageDataGridViewTextBoxColumn
            // 
            this.stageDataGridViewTextBoxColumn.DataPropertyName = "Stage";
            this.stageDataGridViewTextBoxColumn.HeaderText = "Stage";
            this.stageDataGridViewTextBoxColumn.Name = "stageDataGridViewTextBoxColumn";
            this.stageDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // transferedDataGridViewCheckBoxColumn
            // 
            this.transferedDataGridViewCheckBoxColumn.DataPropertyName = "Transfered";
            this.transferedDataGridViewCheckBoxColumn.HeaderText = "Transfered";
            this.transferedDataGridViewCheckBoxColumn.Name = "transferedDataGridViewCheckBoxColumn";
            this.transferedDataGridViewCheckBoxColumn.ReadOnly = true;
            // 
            // transferedDateDataGridViewTextBoxColumn
            // 
            this.transferedDateDataGridViewTextBoxColumn.DataPropertyName = "TransferedDate";
            this.transferedDateDataGridViewTextBoxColumn.HeaderText = "TransferedDate";
            this.transferedDateDataGridViewTextBoxColumn.Name = "transferedDateDataGridViewTextBoxColumn";
            this.transferedDateDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // reasonOfTransferDataGridViewTextBoxColumn
            // 
            this.reasonOfTransferDataGridViewTextBoxColumn.DataPropertyName = "ReasonOfTransfer";
            this.reasonOfTransferDataGridViewTextBoxColumn.HeaderText = "ReasonOfTransfer";
            this.reasonOfTransferDataGridViewTextBoxColumn.Name = "reasonOfTransferDataGridViewTextBoxColumn";
            this.reasonOfTransferDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // openRecordDataGridViewCheckBoxColumn
            // 
            this.openRecordDataGridViewCheckBoxColumn.DataPropertyName = "OpenRecord";
            this.openRecordDataGridViewCheckBoxColumn.HeaderText = "OpenRecord";
            this.openRecordDataGridViewCheckBoxColumn.Name = "openRecordDataGridViewCheckBoxColumn";
            this.openRecordDataGridViewCheckBoxColumn.ReadOnly = true;
            // 
            // operatorDataGridViewTextBoxColumn
            // 
            this.operatorDataGridViewTextBoxColumn.DataPropertyName = "Operator";
            this.operatorDataGridViewTextBoxColumn.HeaderText = "Operator";
            this.operatorDataGridViewTextBoxColumn.Name = "operatorDataGridViewTextBoxColumn";
            this.operatorDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // authorizationTableBindingSource
            // 
            this.authorizationTableBindingSource.DataMember = "AuthorizationTable";
            this.authorizationTableBindingSource.DataSource = this.aTMSDataSet40;
            // 
            // aTMSDataSet40
            // 
            this.aTMSDataSet40.DataSetName = "ATMSDataSet40";
            this.aTMSDataSet40.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.ForeColor = System.Drawing.Color.White;
            this.label22.Location = new System.Drawing.Point(19, 20);
            this.label22.Margin = new System.Windows.Forms.Padding(5, 0, 2, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(277, 29);
            this.label22.TabIndex = 393;
            this.label22.Text = "Authorisation Records ";
            // 
            // buttonNext
            // 
            this.buttonNext.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonNext.FlatAppearance.BorderSize = 2;
            this.buttonNext.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonNext.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonNext.ForeColor = System.Drawing.Color.White;
            this.buttonNext.Location = new System.Drawing.Point(583, 476);
            this.buttonNext.Margin = new System.Windows.Forms.Padding(2);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(122, 27);
            this.buttonNext.TabIndex = 398;
            this.buttonNext.Text = "Next";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // textBoxMessage
            // 
            this.textBoxMessage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.textBoxMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.textBoxMessage.ForeColor = System.Drawing.Color.White;
            this.textBoxMessage.Location = new System.Drawing.Point(24, 476);
            this.textBoxMessage.Name = "textBoxMessage";
            this.textBoxMessage.ReadOnly = true;
            this.textBoxMessage.Size = new System.Drawing.Size(411, 24);
            this.textBoxMessage.TabIndex = 399;
            // 
            // buttonDelete
            // 
            this.buttonDelete.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonDelete.FlatAppearance.BorderSize = 2;
            this.buttonDelete.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonDelete.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonDelete.ForeColor = System.Drawing.Color.White;
            this.buttonDelete.Location = new System.Drawing.Point(419, 476);
            this.buttonDelete.Margin = new System.Windows.Forms.Padding(2);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(76, 27);
            this.buttonDelete.TabIndex = 400;
            this.buttonDelete.Text = "Delete";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonTransfer
            // 
            this.buttonTransfer.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonTransfer.FlatAppearance.BorderSize = 2;
            this.buttonTransfer.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonTransfer.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonTransfer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonTransfer.ForeColor = System.Drawing.Color.White;
            this.buttonTransfer.Location = new System.Drawing.Point(503, 476);
            this.buttonTransfer.Margin = new System.Windows.Forms.Padding(2);
            this.buttonTransfer.Name = "buttonTransfer";
            this.buttonTransfer.Size = new System.Drawing.Size(76, 27);
            this.buttonTransfer.TabIndex = 401;
            this.buttonTransfer.Text = "Transfer";
            this.buttonTransfer.UseVisualStyleBackColor = true;
            this.buttonTransfer.Click += new System.EventHandler(this.buttonTransfer_Click);
            // 
            // authorizationTableTableAdapter
            // 
            this.authorizationTableTableAdapter.ClearBeforeFill = true;
            // 
            // Form112
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(732, 514);
            this.Controls.Add(this.buttonTransfer);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.textBoxMessage);
            this.Controls.Add(this.buttonNext);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.panel1);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "Form112";
            this.Text = "Form112";
            this.Load += new System.EventHandler(this.Form112_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.authorizationTableBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.aTMSDataSet40)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxMessage;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label LabelStage;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonTransfer;
        private System.Windows.Forms.Label Descr;
        private System.Windows.Forms.Label label4;
        private ATMSDataSet40 aTMSDataSet40;
        private System.Windows.Forms.BindingSource authorizationTableBindingSource;
        private ATMSDataSet40TableAdapters.AuthorizationTableTableAdapter authorizationTableTableAdapter;
        private System.Windows.Forms.TextBox textBoxCommnet;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DataGridViewTextBoxColumn seqNumberDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn requestorDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn authoriserDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn originDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn tranNoDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn atmNoDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn disputeNumberDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn disputeTransactionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn replCycleDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn authDecisionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn authCommentDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dateOriginatedDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dateAuthorisedDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn stageDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn transferedDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn transferedDateDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn reasonOfTransferDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn openRecordDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn operatorDataGridViewTextBoxColumn;
    }
}