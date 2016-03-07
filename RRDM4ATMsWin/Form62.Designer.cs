namespace RRDM4ATMsWin
{
    partial class Form62
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonPrintTrace = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxTraceNo = new System.Windows.Forms.TextBox();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.aTMSDataSet20 = new RRDM4ATMsWin.ATMSDataSet20();
            this.inPoolTransBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.inPoolTransTableAdapter = new RRDM4ATMsWin.ATMSDataSet20TableAdapters.InPoolTransTableAdapter();
            this.tranNoDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.originNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.atmTraceNoDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.eJournalTraceNoDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.atmNoDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sesNoDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bankIdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.branchIdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.atmDtTimeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.maskRecordIdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.systemTargetDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.transTypeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.transDescDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cardNoDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cardOriginDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.accNoDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.currDescDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tranAmountDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.authCodeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.refNumbDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.remNoDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.transMsgDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.atmMsgDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.errNoDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.startTrxnDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.endTrxnDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.depCountDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.commissionCodeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.commissionAmountDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.succTranDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.matchingMaskDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.operatorDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.aTMSDataSet20)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inPoolTransBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.panel1.Controls.Add(this.buttonPrintTrace);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.textBoxTraceNo);
            this.panel1.Controls.Add(this.button6);
            this.panel1.Controls.Add(this.button5);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.button4);
            this.panel1.Controls.Add(this.button1);
            this.panel1.ForeColor = System.Drawing.Color.Black;
            this.panel1.Location = new System.Drawing.Point(5, 64);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(904, 550);
            this.panel1.TabIndex = 0;
            // 
            // buttonPrintTrace
            // 
            this.buttonPrintTrace.BackColor = System.Drawing.Color.Transparent;
            this.buttonPrintTrace.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonPrintTrace.FlatAppearance.BorderSize = 2;
            this.buttonPrintTrace.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonPrintTrace.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonPrintTrace.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPrintTrace.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonPrintTrace.Location = new System.Drawing.Point(750, 518);
            this.buttonPrintTrace.Name = "buttonPrintTrace";
            this.buttonPrintTrace.Size = new System.Drawing.Size(125, 24);
            this.buttonPrintTrace.TabIndex = 339;
            this.buttonPrintTrace.Text = "Print Trace Journal";
            this.buttonPrintTrace.UseVisualStyleBackColor = false;
            this.buttonPrintTrace.Click += new System.EventHandler(this.buttonPrintTrace_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.label2.Location = new System.Drawing.Point(542, 520);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 338;
            this.label2.Text = "Trace Number";
            // 
            // textBoxTraceNo
            // 
            this.textBoxTraceNo.Location = new System.Drawing.Point(634, 520);
            this.textBoxTraceNo.Name = "textBoxTraceNo";
            this.textBoxTraceNo.Size = new System.Drawing.Size(100, 20);
            this.textBoxTraceNo.TabIndex = 337;
            // 
            // button6
            // 
            this.button6.BackColor = System.Drawing.Color.Transparent;
            this.button6.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.button6.FlatAppearance.BorderSize = 2;
            this.button6.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.button6.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.button6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.button6.Location = new System.Drawing.Point(750, 488);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(125, 24);
            this.button6.TabIndex = 336;
            this.button6.Text = "Dates Trail";
            this.button6.UseVisualStyleBackColor = false;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button5
            // 
            this.button5.BackColor = System.Drawing.Color.Transparent;
            this.button5.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.button5.FlatAppearance.BorderSize = 2;
            this.button5.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.button5.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.button5.Location = new System.Drawing.Point(184, 488);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(125, 24);
            this.button5.TabIndex = 335;
            this.button5.Text = "Journal For Repl.Cycle";
            this.button5.UseVisualStyleBackColor = false;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.dataGridView1);
            this.panel2.Location = new System.Drawing.Point(7, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(911, 465);
            this.panel2.TabIndex = 1;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.tranNoDataGridViewTextBoxColumn,
            this.originNameDataGridViewTextBoxColumn,
            this.atmTraceNoDataGridViewTextBoxColumn,
            this.eJournalTraceNoDataGridViewTextBoxColumn,
            this.atmNoDataGridViewTextBoxColumn,
            this.sesNoDataGridViewTextBoxColumn,
            this.bankIdDataGridViewTextBoxColumn,
            this.branchIdDataGridViewTextBoxColumn,
            this.atmDtTimeDataGridViewTextBoxColumn,
            this.maskRecordIdDataGridViewTextBoxColumn,
            this.systemTargetDataGridViewTextBoxColumn,
            this.transTypeDataGridViewTextBoxColumn,
            this.transDescDataGridViewTextBoxColumn,
            this.cardNoDataGridViewTextBoxColumn,
            this.cardOriginDataGridViewTextBoxColumn,
            this.accNoDataGridViewTextBoxColumn,
            this.currDescDataGridViewTextBoxColumn,
            this.tranAmountDataGridViewTextBoxColumn,
            this.authCodeDataGridViewTextBoxColumn,
            this.refNumbDataGridViewTextBoxColumn,
            this.remNoDataGridViewTextBoxColumn,
            this.transMsgDataGridViewTextBoxColumn,
            this.atmMsgDataGridViewTextBoxColumn,
            this.errNoDataGridViewTextBoxColumn,
            this.startTrxnDataGridViewTextBoxColumn,
            this.endTrxnDataGridViewTextBoxColumn,
            this.depCountDataGridViewTextBoxColumn,
            this.commissionCodeDataGridViewTextBoxColumn,
            this.commissionAmountDataGridViewTextBoxColumn,
            this.succTranDataGridViewCheckBoxColumn,
            this.matchingMaskDataGridViewTextBoxColumn,
            this.operatorDataGridViewTextBoxColumn});
            this.dataGridView1.DataSource = this.inPoolTransBindingSource;
            this.dataGridView1.GridColor = System.Drawing.Color.LightGray;
            this.dataGridView1.Location = new System.Drawing.Point(5, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(889, 446);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_RowEnter);
            this.dataGridView1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.dataGridView1_MouseDoubleClick);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.Transparent;
            this.button3.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.button3.FlatAppearance.BorderSize = 2;
            this.button3.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.button3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.button3.Location = new System.Drawing.Point(468, 488);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(125, 24);
            this.button3.TabIndex = 332;
            this.button3.Text = "Video Clip";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.Color.Transparent;
            this.button4.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.button4.FlatAppearance.BorderSize = 2;
            this.button4.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.button4.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.button4.Location = new System.Drawing.Point(324, 488);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(125, 24);
            this.button4.TabIndex = 333;
            this.button4.Text = "Journal For Chosen";
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Transparent;
            this.button1.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.button1.FlatAppearance.BorderSize = 2;
            this.button1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.button1.Location = new System.Drawing.Point(609, 488);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(125, 24);
            this.button1.TabIndex = 331;
            this.button1.Text = "Print All";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(245, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 24);
            this.label1.TabIndex = 336;
            this.label1.Text = "label1";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold);
            this.label14.ForeColor = System.Drawing.Color.White;
            this.label14.Location = new System.Drawing.Point(12, 9);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(227, 26);
            this.label14.TabIndex = 328;
            this.label14.Text = "List of transactions  ";
            // 
            // aTMSDataSet20
            // 
            this.aTMSDataSet20.DataSetName = "ATMSDataSet20";
            this.aTMSDataSet20.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // inPoolTransBindingSource
            // 
            this.inPoolTransBindingSource.DataMember = "InPoolTrans";
            this.inPoolTransBindingSource.DataSource = this.aTMSDataSet20;
            // 
            // inPoolTransTableAdapter
            // 
            this.inPoolTransTableAdapter.ClearBeforeFill = true;
            // 
            // tranNoDataGridViewTextBoxColumn
            // 
            this.tranNoDataGridViewTextBoxColumn.DataPropertyName = "TranNo";
            this.tranNoDataGridViewTextBoxColumn.HeaderText = "TranNo";
            this.tranNoDataGridViewTextBoxColumn.Name = "tranNoDataGridViewTextBoxColumn";
            this.tranNoDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // originNameDataGridViewTextBoxColumn
            // 
            this.originNameDataGridViewTextBoxColumn.DataPropertyName = "OriginName";
            this.originNameDataGridViewTextBoxColumn.HeaderText = "OriginName";
            this.originNameDataGridViewTextBoxColumn.Name = "originNameDataGridViewTextBoxColumn";
            this.originNameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // atmTraceNoDataGridViewTextBoxColumn
            // 
            this.atmTraceNoDataGridViewTextBoxColumn.DataPropertyName = "AtmTraceNo";
            this.atmTraceNoDataGridViewTextBoxColumn.HeaderText = "AtmTraceNo";
            this.atmTraceNoDataGridViewTextBoxColumn.Name = "atmTraceNoDataGridViewTextBoxColumn";
            this.atmTraceNoDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // eJournalTraceNoDataGridViewTextBoxColumn
            // 
            this.eJournalTraceNoDataGridViewTextBoxColumn.DataPropertyName = "EJournalTraceNo";
            this.eJournalTraceNoDataGridViewTextBoxColumn.HeaderText = "EJournalTraceNo";
            this.eJournalTraceNoDataGridViewTextBoxColumn.Name = "eJournalTraceNoDataGridViewTextBoxColumn";
            this.eJournalTraceNoDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // atmNoDataGridViewTextBoxColumn
            // 
            this.atmNoDataGridViewTextBoxColumn.DataPropertyName = "AtmNo";
            this.atmNoDataGridViewTextBoxColumn.HeaderText = "AtmNo";
            this.atmNoDataGridViewTextBoxColumn.Name = "atmNoDataGridViewTextBoxColumn";
            this.atmNoDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // sesNoDataGridViewTextBoxColumn
            // 
            this.sesNoDataGridViewTextBoxColumn.DataPropertyName = "SesNo";
            this.sesNoDataGridViewTextBoxColumn.HeaderText = "SesNo";
            this.sesNoDataGridViewTextBoxColumn.Name = "sesNoDataGridViewTextBoxColumn";
            this.sesNoDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // bankIdDataGridViewTextBoxColumn
            // 
            this.bankIdDataGridViewTextBoxColumn.DataPropertyName = "BankId";
            this.bankIdDataGridViewTextBoxColumn.HeaderText = "BankId";
            this.bankIdDataGridViewTextBoxColumn.Name = "bankIdDataGridViewTextBoxColumn";
            this.bankIdDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // branchIdDataGridViewTextBoxColumn
            // 
            this.branchIdDataGridViewTextBoxColumn.DataPropertyName = "BranchId";
            this.branchIdDataGridViewTextBoxColumn.HeaderText = "BranchId";
            this.branchIdDataGridViewTextBoxColumn.Name = "branchIdDataGridViewTextBoxColumn";
            this.branchIdDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // atmDtTimeDataGridViewTextBoxColumn
            // 
            this.atmDtTimeDataGridViewTextBoxColumn.DataPropertyName = "AtmDtTime";
            this.atmDtTimeDataGridViewTextBoxColumn.HeaderText = "AtmDtTime";
            this.atmDtTimeDataGridViewTextBoxColumn.Name = "atmDtTimeDataGridViewTextBoxColumn";
            this.atmDtTimeDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // maskRecordIdDataGridViewTextBoxColumn
            // 
            this.maskRecordIdDataGridViewTextBoxColumn.DataPropertyName = "MaskRecordId";
            this.maskRecordIdDataGridViewTextBoxColumn.HeaderText = "MaskRecordId";
            this.maskRecordIdDataGridViewTextBoxColumn.Name = "maskRecordIdDataGridViewTextBoxColumn";
            this.maskRecordIdDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // systemTargetDataGridViewTextBoxColumn
            // 
            this.systemTargetDataGridViewTextBoxColumn.DataPropertyName = "SystemTarget";
            this.systemTargetDataGridViewTextBoxColumn.HeaderText = "SystemTarget";
            this.systemTargetDataGridViewTextBoxColumn.Name = "systemTargetDataGridViewTextBoxColumn";
            this.systemTargetDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // transTypeDataGridViewTextBoxColumn
            // 
            this.transTypeDataGridViewTextBoxColumn.DataPropertyName = "TransType";
            this.transTypeDataGridViewTextBoxColumn.HeaderText = "TransType";
            this.transTypeDataGridViewTextBoxColumn.Name = "transTypeDataGridViewTextBoxColumn";
            this.transTypeDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // transDescDataGridViewTextBoxColumn
            // 
            this.transDescDataGridViewTextBoxColumn.DataPropertyName = "TransDesc";
            this.transDescDataGridViewTextBoxColumn.HeaderText = "TransDesc";
            this.transDescDataGridViewTextBoxColumn.Name = "transDescDataGridViewTextBoxColumn";
            this.transDescDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // cardNoDataGridViewTextBoxColumn
            // 
            this.cardNoDataGridViewTextBoxColumn.DataPropertyName = "CardNo";
            this.cardNoDataGridViewTextBoxColumn.HeaderText = "CardNo";
            this.cardNoDataGridViewTextBoxColumn.Name = "cardNoDataGridViewTextBoxColumn";
            this.cardNoDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // cardOriginDataGridViewTextBoxColumn
            // 
            this.cardOriginDataGridViewTextBoxColumn.DataPropertyName = "CardOrigin";
            this.cardOriginDataGridViewTextBoxColumn.HeaderText = "CardOrigin";
            this.cardOriginDataGridViewTextBoxColumn.Name = "cardOriginDataGridViewTextBoxColumn";
            this.cardOriginDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // accNoDataGridViewTextBoxColumn
            // 
            this.accNoDataGridViewTextBoxColumn.DataPropertyName = "AccNo";
            this.accNoDataGridViewTextBoxColumn.HeaderText = "AccNo";
            this.accNoDataGridViewTextBoxColumn.Name = "accNoDataGridViewTextBoxColumn";
            this.accNoDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // currDescDataGridViewTextBoxColumn
            // 
            this.currDescDataGridViewTextBoxColumn.DataPropertyName = "CurrDesc";
            this.currDescDataGridViewTextBoxColumn.HeaderText = "CurrDesc";
            this.currDescDataGridViewTextBoxColumn.Name = "currDescDataGridViewTextBoxColumn";
            this.currDescDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // tranAmountDataGridViewTextBoxColumn
            // 
            this.tranAmountDataGridViewTextBoxColumn.DataPropertyName = "TranAmount";
            this.tranAmountDataGridViewTextBoxColumn.HeaderText = "TranAmount";
            this.tranAmountDataGridViewTextBoxColumn.Name = "tranAmountDataGridViewTextBoxColumn";
            this.tranAmountDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // authCodeDataGridViewTextBoxColumn
            // 
            this.authCodeDataGridViewTextBoxColumn.DataPropertyName = "AuthCode";
            this.authCodeDataGridViewTextBoxColumn.HeaderText = "AuthCode";
            this.authCodeDataGridViewTextBoxColumn.Name = "authCodeDataGridViewTextBoxColumn";
            this.authCodeDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // refNumbDataGridViewTextBoxColumn
            // 
            this.refNumbDataGridViewTextBoxColumn.DataPropertyName = "RefNumb";
            this.refNumbDataGridViewTextBoxColumn.HeaderText = "RefNumb";
            this.refNumbDataGridViewTextBoxColumn.Name = "refNumbDataGridViewTextBoxColumn";
            this.refNumbDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // remNoDataGridViewTextBoxColumn
            // 
            this.remNoDataGridViewTextBoxColumn.DataPropertyName = "RemNo";
            this.remNoDataGridViewTextBoxColumn.HeaderText = "RemNo";
            this.remNoDataGridViewTextBoxColumn.Name = "remNoDataGridViewTextBoxColumn";
            this.remNoDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // transMsgDataGridViewTextBoxColumn
            // 
            this.transMsgDataGridViewTextBoxColumn.DataPropertyName = "TransMsg";
            this.transMsgDataGridViewTextBoxColumn.HeaderText = "TransMsg";
            this.transMsgDataGridViewTextBoxColumn.Name = "transMsgDataGridViewTextBoxColumn";
            this.transMsgDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // atmMsgDataGridViewTextBoxColumn
            // 
            this.atmMsgDataGridViewTextBoxColumn.DataPropertyName = "AtmMsg";
            this.atmMsgDataGridViewTextBoxColumn.HeaderText = "AtmMsg";
            this.atmMsgDataGridViewTextBoxColumn.Name = "atmMsgDataGridViewTextBoxColumn";
            this.atmMsgDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // errNoDataGridViewTextBoxColumn
            // 
            this.errNoDataGridViewTextBoxColumn.DataPropertyName = "ErrNo";
            this.errNoDataGridViewTextBoxColumn.HeaderText = "ErrNo";
            this.errNoDataGridViewTextBoxColumn.Name = "errNoDataGridViewTextBoxColumn";
            this.errNoDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // startTrxnDataGridViewTextBoxColumn
            // 
            this.startTrxnDataGridViewTextBoxColumn.DataPropertyName = "StartTrxn";
            this.startTrxnDataGridViewTextBoxColumn.HeaderText = "StartTrxn";
            this.startTrxnDataGridViewTextBoxColumn.Name = "startTrxnDataGridViewTextBoxColumn";
            this.startTrxnDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // endTrxnDataGridViewTextBoxColumn
            // 
            this.endTrxnDataGridViewTextBoxColumn.DataPropertyName = "EndTrxn";
            this.endTrxnDataGridViewTextBoxColumn.HeaderText = "EndTrxn";
            this.endTrxnDataGridViewTextBoxColumn.Name = "endTrxnDataGridViewTextBoxColumn";
            this.endTrxnDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // depCountDataGridViewTextBoxColumn
            // 
            this.depCountDataGridViewTextBoxColumn.DataPropertyName = "DepCount";
            this.depCountDataGridViewTextBoxColumn.HeaderText = "DepCount";
            this.depCountDataGridViewTextBoxColumn.Name = "depCountDataGridViewTextBoxColumn";
            this.depCountDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // commissionCodeDataGridViewTextBoxColumn
            // 
            this.commissionCodeDataGridViewTextBoxColumn.DataPropertyName = "CommissionCode";
            this.commissionCodeDataGridViewTextBoxColumn.HeaderText = "CommissionCode";
            this.commissionCodeDataGridViewTextBoxColumn.Name = "commissionCodeDataGridViewTextBoxColumn";
            this.commissionCodeDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // commissionAmountDataGridViewTextBoxColumn
            // 
            this.commissionAmountDataGridViewTextBoxColumn.DataPropertyName = "CommissionAmount";
            this.commissionAmountDataGridViewTextBoxColumn.HeaderText = "CommissionAmount";
            this.commissionAmountDataGridViewTextBoxColumn.Name = "commissionAmountDataGridViewTextBoxColumn";
            this.commissionAmountDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // succTranDataGridViewCheckBoxColumn
            // 
            this.succTranDataGridViewCheckBoxColumn.DataPropertyName = "SuccTran";
            this.succTranDataGridViewCheckBoxColumn.HeaderText = "SuccTran";
            this.succTranDataGridViewCheckBoxColumn.Name = "succTranDataGridViewCheckBoxColumn";
            this.succTranDataGridViewCheckBoxColumn.ReadOnly = true;
            // 
            // matchingMaskDataGridViewTextBoxColumn
            // 
            this.matchingMaskDataGridViewTextBoxColumn.DataPropertyName = "MatchingMask";
            this.matchingMaskDataGridViewTextBoxColumn.HeaderText = "MatchingMask";
            this.matchingMaskDataGridViewTextBoxColumn.Name = "matchingMaskDataGridViewTextBoxColumn";
            this.matchingMaskDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // operatorDataGridViewTextBoxColumn
            // 
            this.operatorDataGridViewTextBoxColumn.DataPropertyName = "Operator";
            this.operatorDataGridViewTextBoxColumn.HeaderText = "Operator";
            this.operatorDataGridViewTextBoxColumn.Name = "operatorDataGridViewTextBoxColumn";
            this.operatorDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // Form62
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(921, 626);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label14);
            this.Name = "Form62";
            this.Text = "Form62";
            this.Load += new System.EventHandler(this.Form62_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.aTMSDataSet20)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inPoolTransBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxTraceNo;
        private System.Windows.Forms.Button buttonPrintTrace;
        private ATMSDataSet20 aTMSDataSet20;
        private System.Windows.Forms.BindingSource inPoolTransBindingSource;
        private ATMSDataSet20TableAdapters.InPoolTransTableAdapter inPoolTransTableAdapter;
        private System.Windows.Forms.DataGridViewTextBoxColumn tranNoDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn originNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn atmTraceNoDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn eJournalTraceNoDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn atmNoDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn sesNoDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn bankIdDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn branchIdDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn atmDtTimeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn maskRecordIdDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn systemTargetDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn transTypeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn transDescDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn cardNoDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn cardOriginDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn accNoDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn currDescDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn tranAmountDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn authCodeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn refNumbDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn remNoDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn transMsgDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn atmMsgDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn errNoDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn startTrxnDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn endTrxnDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn depCountDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn commissionCodeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn commissionAmountDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn succTranDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn matchingMaskDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn operatorDataGridViewTextBoxColumn;

    }
}