namespace RRDM4ATMsWin
{
    partial class Form5ITMX
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form5ITMX));
            this.dataColumn4 = new System.Data.DataColumn();
            this.dataColumn2 = new System.Data.DataColumn();
            this.dataColumn1 = new System.Data.DataColumn();
            this.dtTransactions = new System.Data.DataTable();
            this.dataColumn3 = new System.Data.DataColumn();
            this.dsTransactions = new System.Data.DataSet();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.labelStep1 = new System.Windows.Forms.Label();
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tbId = new System.Windows.Forms.TextBox();
            this.labelID = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.buttonShowMaps = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.labelNumberNotes2 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.buttonNotes2 = new System.Windows.Forms.Button();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.label13 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.radioButtonReconcDiff = new System.Windows.Forms.RadioButton();
            this.radioButtonDepositDiff = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.radioButtonOther = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.tbComments = new System.Windows.Forms.TextBox();
            this.labelComments = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.comboBoxVisitType = new System.Windows.Forms.ComboBox();
            this.dateTimePickerTo = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerFrom = new System.Windows.Forms.DateTimePicker();
            this.buttonTransactions = new System.Windows.Forms.Button();
            this.label30 = new System.Windows.Forms.Label();
            this.tbCustEmail = new System.Windows.Forms.TextBox();
            this.label29 = new System.Windows.Forms.Label();
            this.tbCustPhone = new System.Windows.Forms.TextBox();
            this.tbAccNo = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelEmail = new System.Windows.Forms.Label();
            this.labelPhone = new System.Windows.Forms.Label();
            this.labelAccount = new System.Windows.Forms.Label();
            this.tbCustName = new System.Windows.Forms.TextBox();
            this.labelName = new System.Windows.Forms.Label();
            this.tbCustomerUniqueId = new System.Windows.Forms.TextBox();
            this.labelCard = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.textBoxMsgBoard = new System.Windows.Forms.TextBox();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.buttonFinish = new System.Windows.Forms.Button();
            this.toolTipController = new System.Windows.Forms.ToolTip(this.components);
            this.toolTipMessages = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanelHeader = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.labelToday = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.labelUserId = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dtTransactions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsTransactions)).BeginInit();
            this.tableLayoutPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.tableLayoutPanelMain.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanelHeader.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataColumn4
            // 
            this.dataColumn4.AutoIncrement = true;
            this.dataColumn4.ColumnName = "ID";
            this.dataColumn4.DataType = typeof(int);
            // 
            // dataColumn2
            // 
            this.dataColumn2.ColumnName = "Amount";
            this.dataColumn2.DataType = typeof(double);
            this.dataColumn2.DefaultValue = 0D;
            // 
            // dataColumn1
            // 
            this.dataColumn1.Caption = "Date";
            this.dataColumn1.ColumnName = "Date";
            this.dataColumn1.DataType = typeof(System.DateTime);
            // 
            // dtTransactions
            // 
            this.dtTransactions.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn1,
            this.dataColumn2,
            this.dataColumn3,
            this.dataColumn4});
            this.dtTransactions.TableName = "Transactions";
            // 
            // dataColumn3
            // 
            this.dataColumn3.ColumnName = "Currency";
            // 
            // dsTransactions
            // 
            this.dsTransactions.DataSetName = "TransactionsDataSet";
            this.dsTransactions.Tables.AddRange(new System.Data.DataTable[] {
            this.dtTransactions});
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 76F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 232F));
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel3, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.pictureBox1, 0, 0);
            this.tableLayoutPanel5.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 67F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(288, 67);
            this.tableLayoutPanel5.TabIndex = 251;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(2, 2);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(72, 63);
            this.pictureBox1.TabIndex = 251;
            this.pictureBox1.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 63F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 292F));
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(863, 2);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 49F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 13F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 13F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(139, 80);
            this.tableLayoutPanel1.TabIndex = 247;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.labelStep1);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(294, 6);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(2, 6, 2, 2);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(553, 63);
            this.flowLayoutPanel1.TabIndex = 243;
            // 
            // labelStep1
            // 
            this.labelStep1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelStep1.AutoSize = true;
            this.labelStep1.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStep1.ForeColor = System.Drawing.Color.White;
            this.labelStep1.Location = new System.Drawing.Point(2, 0);
            this.labelStep1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelStep1.Name = "labelStep1";
            this.labelStep1.Size = new System.Drawing.Size(314, 36);
            this.labelStep1.TabIndex = 242;
            this.labelStep1.Text = "Input of Dispute Data";
            // 
            // tableLayoutPanelMain
            // 
            this.tableLayoutPanelMain.ColumnCount = 1;
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMain.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelMain.ForeColor = System.Drawing.Color.White;
            this.tableLayoutPanelMain.Location = new System.Drawing.Point(0, 88);
            this.tableLayoutPanelMain.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            this.tableLayoutPanelMain.RowCount = 2;
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelMain.Size = new System.Drawing.Size(1008, 642);
            this.tableLayoutPanelMain.TabIndex = 259;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.panel1.Controls.Add(this.tbId);
            this.panel1.Controls.Add(this.labelID);
            this.panel1.Controls.Add(this.panel4);
            this.panel1.Controls.Add(this.label13);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label21);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.ForeColor = System.Drawing.Color.Black;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1002, 596);
            this.panel1.TabIndex = 245;
            // 
            // tbId
            // 
            this.tbId.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.tbId.ForeColor = System.Drawing.Color.White;
            this.tbId.Location = new System.Drawing.Point(105, 24);
            this.tbId.Margin = new System.Windows.Forms.Padding(2);
            this.tbId.Name = "tbId";
            this.tbId.ReadOnly = true;
            this.tbId.Size = new System.Drawing.Size(118, 26);
            this.tbId.TabIndex = 292;
            this.tbId.TabStop = false;
            // 
            // labelID
            // 
            this.labelID.AutoSize = true;
            this.labelID.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.labelID.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.labelID.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelID.Location = new System.Drawing.Point(12, 26);
            this.labelID.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelID.Name = "labelID";
            this.labelID.Size = new System.Drawing.Size(78, 13);
            this.labelID.TabIndex = 291;
            this.labelID.Text = "DISPUTE ID";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.White;
            this.panel4.Controls.Add(this.buttonShowMaps);
            this.panel4.Controls.Add(this.label3);
            this.panel4.Controls.Add(this.labelNumberNotes2);
            this.panel4.Controls.Add(this.label9);
            this.panel4.Controls.Add(this.buttonNotes2);
            this.panel4.Controls.Add(this.textBox5);
            this.panel4.Controls.Add(this.dataGridView1);
            this.panel4.ForeColor = System.Drawing.Color.Black;
            this.panel4.Location = new System.Drawing.Point(467, 80);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(526, 513);
            this.panel4.TabIndex = 266;
            // 
            // buttonShowMaps
            // 
            this.buttonShowMaps.BackColor = System.Drawing.Color.Transparent;
            this.buttonShowMaps.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonShowMaps.FlatAppearance.BorderSize = 2;
            this.buttonShowMaps.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.buttonShowMaps.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonShowMaps.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonShowMaps.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonShowMaps.Location = new System.Drawing.Point(359, 437);
            this.buttonShowMaps.Name = "buttonShowMaps";
            this.buttonShowMaps.Size = new System.Drawing.Size(58, 45);
            this.buttonShowMaps.TabIndex = 399;
            this.buttonShowMaps.Text = "Show Maps";
            this.buttonShowMaps.UseVisualStyleBackColor = false;
            this.buttonShowMaps.Click += new System.EventHandler(this.buttonShowMaps_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.label3.Location = new System.Drawing.Point(442, 418);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 398;
            this.label3.Text = "Attach";
            // 
            // labelNumberNotes2
            // 
            this.labelNumberNotes2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelNumberNotes2.AutoSize = true;
            this.labelNumberNotes2.BackColor = System.Drawing.Color.Gainsboro;
            this.labelNumberNotes2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelNumberNotes2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.labelNumberNotes2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(198)))));
            this.labelNumberNotes2.Location = new System.Drawing.Point(492, 435);
            this.labelNumberNotes2.Name = "labelNumberNotes2";
            this.labelNumberNotes2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.labelNumberNotes2.Size = new System.Drawing.Size(15, 15);
            this.labelNumberNotes2.TabIndex = 397;
            this.labelNumberNotes2.Text = "2";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 437);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(61, 13);
            this.label9.TabIndex = 261;
            this.label9.Text = "Total Trans";
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
            this.buttonNotes2.Location = new System.Drawing.Point(448, 437);
            this.buttonNotes2.Margin = new System.Windows.Forms.Padding(2);
            this.buttonNotes2.Name = "buttonNotes2";
            this.buttonNotes2.Size = new System.Drawing.Size(56, 52);
            this.buttonNotes2.TabIndex = 396;
            this.buttonNotes2.UseVisualStyleBackColor = true;
            this.buttonNotes2.Click += new System.EventHandler(this.buttonNotes2_Click);
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(81, 437);
            this.textBox5.Name = "textBox5";
            this.textBox5.ReadOnly = true;
            this.textBox5.Size = new System.Drawing.Size(53, 20);
            this.textBox5.TabIndex = 260;
            // 
            // dataGridView1
            // 
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.GridColor = System.Drawing.Color.LightGray;
            this.dataGridView1.Location = new System.Drawing.Point(3, 8);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(520, 398);
            this.dataGridView1.TabIndex = 259;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.label13.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.label13.Location = new System.Drawing.Point(467, 59);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(135, 18);
            this.label13.TabIndex = 267;
            this.label13.Text = "TRANSACTIONS";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.Controls.Add(this.radioButtonReconcDiff);
            this.panel3.Controls.Add(this.radioButtonDepositDiff);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.textBox1);
            this.panel3.Controls.Add(this.radioButtonOther);
            this.panel3.Controls.Add(this.radioButton3);
            this.panel3.Controls.Add(this.radioButton2);
            this.panel3.Controls.Add(this.radioButton1);
            this.panel3.Controls.Add(this.textBox4);
            this.panel3.Controls.Add(this.tbComments);
            this.panel3.Controls.Add(this.labelComments);
            this.panel3.ForeColor = System.Drawing.Color.Black;
            this.panel3.Location = new System.Drawing.Point(12, 345);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(438, 248);
            this.panel3.TabIndex = 262;
            // 
            // radioButtonReconcDiff
            // 
            this.radioButtonReconcDiff.AutoSize = true;
            this.radioButtonReconcDiff.Location = new System.Drawing.Point(231, 77);
            this.radioButtonReconcDiff.Name = "radioButtonReconcDiff";
            this.radioButtonReconcDiff.Size = new System.Drawing.Size(121, 17);
            this.radioButtonReconcDiff.TabIndex = 311;
            this.radioButtonReconcDiff.TabStop = true;
            this.radioButtonReconcDiff.Text = "Reconc. Difference ";
            this.radioButtonReconcDiff.UseVisualStyleBackColor = true;
            // 
            // radioButtonDepositDiff
            // 
            this.radioButtonDepositDiff.AutoSize = true;
            this.radioButtonDepositDiff.Location = new System.Drawing.Point(3, 77);
            this.radioButtonDepositDiff.Name = "radioButtonDepositDiff";
            this.radioButtonDepositDiff.Size = new System.Drawing.Size(116, 17);
            this.radioButtonDepositDiff.TabIndex = 310;
            this.radioButtonDepositDiff.TabStop = true;
            this.radioButtonDepositDiff.Text = "Deposit Difference ";
            this.radioButtonDepositDiff.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(394, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 309;
            this.label1.Text = "times.";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(364, 7);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(28, 20);
            this.textBox1.TabIndex = 307;
            // 
            // radioButtonOther
            // 
            this.radioButtonOther.AutoSize = true;
            this.radioButtonOther.Location = new System.Drawing.Point(3, 100);
            this.radioButtonOther.Name = "radioButtonOther";
            this.radioButtonOther.Size = new System.Drawing.Size(210, 17);
            this.radioButtonOther.TabIndex = 300;
            this.radioButtonOther.TabStop = true;
            this.radioButtonOther.Text = "OTHERS: Please specify detailes here:";
            this.radioButtonOther.UseVisualStyleBackColor = true;
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(3, 54);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(266, 17);
            this.radioButton3.TabIndex = 302;
            this.radioButton3.TabStop = true;
            this.radioButton3.Text = "My account was not credited with social insurance ";
            this.radioButton3.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(3, 31);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(235, 17);
            this.radioButton2.TabIndex = 303;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "Customer says was debited with more money";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(3, 8);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(250, 17);
            this.radioButton1.TabIndex = 304;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Duplicate Billing: My account was debited twice";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(9, 115);
            this.textBox4.Margin = new System.Windows.Forms.Padding(2);
            this.textBox4.Multiline = true;
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(402, 26);
            this.textBox4.TabIndex = 297;
            // 
            // tbComments
            // 
            this.tbComments.Location = new System.Drawing.Point(7, 160);
            this.tbComments.Margin = new System.Windows.Forms.Padding(2);
            this.tbComments.Multiline = true;
            this.tbComments.Name = "tbComments";
            this.tbComments.Size = new System.Drawing.Size(404, 44);
            this.tbComments.TabIndex = 298;
            // 
            // labelComments
            // 
            this.labelComments.AutoSize = true;
            this.labelComments.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.labelComments.Location = new System.Drawing.Point(13, 145);
            this.labelComments.Name = "labelComments";
            this.labelComments.Size = new System.Drawing.Size(68, 13);
            this.labelComments.TabIndex = 299;
            this.labelComments.Text = "Comments ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.label5.Location = new System.Drawing.Point(21, 323);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(154, 18);
            this.label5.TabIndex = 264;
            this.label5.Text = "TYPE OF DISPUTE";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.label21.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.label21.Location = new System.Drawing.Point(9, 61);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(150, 18);
            this.label21.TabIndex = 265;
            this.label21.Text = "CUSTOMER DATA";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.button2);
            this.panel2.Controls.Add(this.textBox2);
            this.panel2.Controls.Add(this.label10);
            this.panel2.Controls.Add(this.comboBoxVisitType);
            this.panel2.Controls.Add(this.dateTimePickerTo);
            this.panel2.Controls.Add(this.dateTimePickerFrom);
            this.panel2.Controls.Add(this.buttonTransactions);
            this.panel2.Controls.Add(this.label30);
            this.panel2.Controls.Add(this.tbCustEmail);
            this.panel2.Controls.Add(this.label29);
            this.panel2.Controls.Add(this.tbCustPhone);
            this.panel2.Controls.Add(this.tbAccNo);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.labelEmail);
            this.panel2.Controls.Add(this.labelPhone);
            this.panel2.Controls.Add(this.labelAccount);
            this.panel2.Controls.Add(this.tbCustName);
            this.panel2.Controls.Add(this.labelName);
            this.panel2.Controls.Add(this.tbCustomerUniqueId);
            this.panel2.Controls.Add(this.labelCard);
            this.panel2.ForeColor = System.Drawing.Color.Black;
            this.panel2.Location = new System.Drawing.Point(12, 80);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(438, 236);
            this.panel2.TabIndex = 263;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.Transparent;
            this.button2.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.button2.FlatAppearance.BorderSize = 2;
            this.button2.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.button2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.button2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.button2.Location = new System.Drawing.Point(283, 58);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(146, 28);
            this.button2.TabIndex = 293;
            this.button2.Text = "Previous Disputes";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(323, 136);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(110, 20);
            this.textBox2.TabIndex = 294;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(278, 139);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(31, 13);
            this.label10.TabIndex = 295;
            this.label10.Text = "RRN";
            // 
            // comboBoxVisitType
            // 
            this.comboBoxVisitType.FormattingEnabled = true;
            this.comboBoxVisitType.Location = new System.Drawing.Point(96, 5);
            this.comboBoxVisitType.Name = "comboBoxVisitType";
            this.comboBoxVisitType.Size = new System.Drawing.Size(332, 21);
            this.comboBoxVisitType.TabIndex = 9;
            this.comboBoxVisitType.SelectedIndexChanged += new System.EventHandler(this.comboBoxVisitType_SelectedIndexChanged);
            // 
            // dateTimePickerTo
            // 
            this.dateTimePickerTo.Location = new System.Drawing.Point(44, 212);
            this.dateTimePickerTo.Name = "dateTimePickerTo";
            this.dateTimePickerTo.Size = new System.Drawing.Size(170, 20);
            this.dateTimePickerTo.TabIndex = 8;
            // 
            // dateTimePickerFrom
            // 
            this.dateTimePickerFrom.Location = new System.Drawing.Point(44, 186);
            this.dateTimePickerFrom.Name = "dateTimePickerFrom";
            this.dateTimePickerFrom.Size = new System.Drawing.Size(170, 20);
            this.dateTimePickerFrom.TabIndex = 7;
            // 
            // buttonTransactions
            // 
            this.buttonTransactions.BackColor = System.Drawing.Color.Transparent;
            this.buttonTransactions.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonTransactions.FlatAppearance.BorderSize = 2;
            this.buttonTransactions.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.buttonTransactions.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonTransactions.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonTransactions.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonTransactions.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonTransactions.Location = new System.Drawing.Point(281, 200);
            this.buttonTransactions.Name = "buttonTransactions";
            this.buttonTransactions.Size = new System.Drawing.Size(146, 28);
            this.buttonTransactions.TabIndex = 258;
            this.buttonTransactions.Text = "Search transactions";
            this.buttonTransactions.UseVisualStyleBackColor = false;
            this.buttonTransactions.Click += new System.EventHandler(this.btTransactions_Click_1);
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(18, 215);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(20, 13);
            this.label30.TabIndex = 292;
            this.label30.Text = "To";
            // 
            // tbCustEmail
            // 
            this.tbCustEmail.Location = new System.Drawing.Point(61, 132);
            this.tbCustEmail.Name = "tbCustEmail";
            this.tbCustEmail.Size = new System.Drawing.Size(209, 20);
            this.tbCustEmail.TabIndex = 5;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(8, 190);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(30, 13);
            this.label29.TabIndex = 293;
            this.label29.Text = "From";
            // 
            // tbCustPhone
            // 
            this.tbCustPhone.Location = new System.Drawing.Point(61, 106);
            this.tbCustPhone.Name = "tbCustPhone";
            this.tbCustPhone.Size = new System.Drawing.Size(146, 20);
            this.tbCustPhone.TabIndex = 4;
            // 
            // tbAccNo
            // 
            this.tbAccNo.Location = new System.Drawing.Point(61, 84);
            this.tbAccNo.Name = "tbAccNo";
            this.tbAccNo.Size = new System.Drawing.Size(146, 20);
            this.tbAccNo.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 163);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(216, 13);
            this.label4.TabIndex = 280;
            this.label4.Text = "Transaction(s) was/were done at the period:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 280;
            this.label2.Text = "Contact method";
            // 
            // labelEmail
            // 
            this.labelEmail.AutoSize = true;
            this.labelEmail.Location = new System.Drawing.Point(8, 135);
            this.labelEmail.Name = "labelEmail";
            this.labelEmail.Size = new System.Drawing.Size(35, 13);
            this.labelEmail.TabIndex = 280;
            this.labelEmail.Text = "E-mail";
            // 
            // labelPhone
            // 
            this.labelPhone.AutoSize = true;
            this.labelPhone.Location = new System.Drawing.Point(8, 109);
            this.labelPhone.Name = "labelPhone";
            this.labelPhone.Size = new System.Drawing.Size(38, 13);
            this.labelPhone.TabIndex = 280;
            this.labelPhone.Text = "Phone";
            // 
            // labelAccount
            // 
            this.labelAccount.AutoSize = true;
            this.labelAccount.Location = new System.Drawing.Point(8, 87);
            this.labelAccount.Name = "labelAccount";
            this.labelAccount.Size = new System.Drawing.Size(47, 13);
            this.labelAccount.TabIndex = 280;
            this.labelAccount.Text = "Account";
            // 
            // tbCustName
            // 
            this.tbCustName.Location = new System.Drawing.Point(61, 32);
            this.tbCustName.Name = "tbCustName";
            this.tbCustName.Size = new System.Drawing.Size(368, 20);
            this.tbCustName.TabIndex = 3;
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Location = new System.Drawing.Point(8, 35);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(35, 13);
            this.labelName.TabIndex = 280;
            this.labelName.Text = "Name";
            // 
            // tbCustomerUniqueId
            // 
            this.tbCustomerUniqueId.Location = new System.Drawing.Point(61, 58);
            this.tbCustomerUniqueId.Name = "tbCustomerUniqueId";
            this.tbCustomerUniqueId.Size = new System.Drawing.Size(146, 20);
            this.tbCustomerUniqueId.TabIndex = 1;
            // 
            // labelCard
            // 
            this.labelCard.AutoSize = true;
            this.labelCard.Location = new System.Drawing.Point(8, 61);
            this.labelCard.Name = "labelCard";
            this.labelCard.Size = new System.Drawing.Size(50, 13);
            this.labelCard.TabIndex = 280;
            this.labelCard.Text = "UniqueId";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 5;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel2.Controls.Add(this.textBoxMsgBoard, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.buttonAdd, 4, 0);
            this.tableLayoutPanel2.Controls.Add(this.button1, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.buttonFinish, 2, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(2, 604);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1004, 36);
            this.tableLayoutPanel2.TabIndex = 242;
            // 
            // textBoxMsgBoard
            // 
            this.textBoxMsgBoard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.textBoxMsgBoard.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxMsgBoard.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxMsgBoard.ForeColor = System.Drawing.Color.White;
            this.textBoxMsgBoard.Location = new System.Drawing.Point(10, 2);
            this.textBoxMsgBoard.Margin = new System.Windows.Forms.Padding(10, 2, 2, 2);
            this.textBoxMsgBoard.Multiline = true;
            this.textBoxMsgBoard.Name = "textBoxMsgBoard";
            this.textBoxMsgBoard.ReadOnly = true;
            this.textBoxMsgBoard.Size = new System.Drawing.Size(632, 30);
            this.textBoxMsgBoard.TabIndex = 242;
            this.textBoxMsgBoard.Text = "No guidance information available.";
            // 
            // buttonAdd
            // 
            this.buttonAdd.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonAdd.FlatAppearance.BorderSize = 2;
            this.buttonAdd.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonAdd.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonAdd.ForeColor = System.Drawing.Color.White;
            this.buttonAdd.Location = new System.Drawing.Point(916, 2);
            this.buttonAdd.Margin = new System.Windows.Forms.Padding(2, 2, 10, 2);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(78, 27);
            this.buttonAdd.TabIndex = 245;
            this.buttonAdd.Text = "Add/Update";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Transparent;
            this.button1.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.button1.FlatAppearance.BorderSize = 2;
            this.button1.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.button1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(827, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(84, 28);
            this.button1.TabIndex = 271;
            this.button1.Text = "Print application";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonFinish
            // 
            this.buttonFinish.BackColor = System.Drawing.Color.Transparent;
            this.buttonFinish.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonFinish.FlatAppearance.BorderSize = 2;
            this.buttonFinish.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.buttonFinish.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonFinish.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonFinish.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonFinish.ForeColor = System.Drawing.Color.White;
            this.buttonFinish.Location = new System.Drawing.Point(737, 3);
            this.buttonFinish.Name = "buttonFinish";
            this.buttonFinish.Size = new System.Drawing.Size(84, 28);
            this.buttonFinish.TabIndex = 272;
            this.buttonFinish.Text = "Finish ";
            this.buttonFinish.UseVisualStyleBackColor = false;
            this.buttonFinish.Click += new System.EventHandler(this.buttonFinish_Click);
            // 
            // toolTipController
            // 
            this.toolTipController.IsBalloon = true;
            this.toolTipController.ShowAlways = true;
            this.toolTipController.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTipController.ToolTipTitle = "Communication";
            // 
            // toolTipMessages
            // 
            this.toolTipMessages.AutomaticDelay = 100;
            this.toolTipMessages.AutoPopDelay = 9000;
            this.toolTipMessages.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.toolTipMessages.InitialDelay = 100;
            this.toolTipMessages.IsBalloon = true;
            this.toolTipMessages.ReshowDelay = 10;
            this.toolTipMessages.ShowAlways = true;
            this.toolTipMessages.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTipMessages.ToolTipTitle = "Messages";
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 3;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 292F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 147F));
            this.tableLayoutPanel4.Controls.Add(this.flowLayoutPanel1, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel1, 2, 0);
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel5, 0, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 84F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(1004, 84);
            this.tableLayoutPanel4.TabIndex = 249;
            // 
            // tableLayoutPanelHeader
            // 
            this.tableLayoutPanelHeader.ColumnCount = 1;
            this.tableLayoutPanelHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelHeader.Controls.Add(this.tableLayoutPanel4, 0, 0);
            this.tableLayoutPanelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanelHeader.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelHeader.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanelHeader.Name = "tableLayoutPanelHeader";
            this.tableLayoutPanelHeader.RowCount = 1;
            this.tableLayoutPanelHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 84F));
            this.tableLayoutPanelHeader.Size = new System.Drawing.Size(1008, 88);
            this.tableLayoutPanelHeader.TabIndex = 260;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 67F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 161F));
            this.tableLayoutPanel3.Controls.Add(this.labelToday, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.label8, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.label6, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.labelUserId, 1, 1);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(78, 2);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(228, 63);
            this.tableLayoutPanel3.TabIndex = 252;
            // 
            // labelToday
            // 
            this.labelToday.AutoSize = true;
            this.labelToday.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelToday.ForeColor = System.Drawing.Color.White;
            this.labelToday.Location = new System.Drawing.Point(69, 0);
            this.labelToday.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelToday.Name = "labelToday";
            this.labelToday.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.labelToday.Size = new System.Drawing.Size(42, 13);
            this.labelToday.TabIndex = 117;
            this.labelToday.Text = "Today";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(2, 0);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 3, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(30, 13);
            this.label8.TabIndex = 250;
            this.label8.Text = "Date";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(2, 23);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 3, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 13);
            this.label6.TabIndex = 252;
            this.label6.Text = "User Id";
            // 
            // labelUserId
            // 
            this.labelUserId.AutoSize = true;
            this.labelUserId.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelUserId.Location = new System.Drawing.Point(69, 23);
            this.labelUserId.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelUserId.Name = "labelUserId";
            this.labelUserId.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.labelUserId.Size = new System.Drawing.Size(48, 13);
            this.labelUserId.TabIndex = 253;
            this.labelUserId.Text = "User Id";
            // 
            // Form5ITMX
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(1008, 730);
            this.Controls.Add(this.tableLayoutPanelMain);
            this.Controls.Add(this.tableLayoutPanelHeader);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "Form5ITMX";
            this.Text = "Form5ITMX";
            ((System.ComponentModel.ISupportInitialize)(this.dtTransactions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsTransactions)).EndInit();
            this.tableLayoutPanel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanelHeader.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Data.DataColumn dataColumn4;
        private System.Data.DataColumn dataColumn2;
        private System.Data.DataColumn dataColumn1;
        private System.Data.DataTable dtTransactions;
        private System.Data.DataColumn dataColumn3;
        private System.Data.DataSet dsTransactions;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label labelStep1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private System.Windows.Forms.TextBox textBoxMsgBoard;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.ToolTip toolTipController;
        private System.Windows.Forms.ToolTip toolTipMessages;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelHeader;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.RadioButton radioButtonDepositDiff;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.RadioButton radioButtonOther;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox tbComments;
        private System.Windows.Forms.Label labelComments;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ComboBox comboBoxVisitType;
        private System.Windows.Forms.DateTimePicker dateTimePickerTo;
        private System.Windows.Forms.DateTimePicker dateTimePickerFrom;
        private System.Windows.Forms.Button buttonTransactions;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.TextBox tbCustEmail;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.TextBox tbCustPhone;
        private System.Windows.Forms.TextBox tbAccNo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelEmail;
        private System.Windows.Forms.Label labelPhone;
        private System.Windows.Forms.Label labelAccount;
        private System.Windows.Forms.TextBox tbCustName;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.TextBox tbCustomerUniqueId;
        private System.Windows.Forms.Label labelCard;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label labelID;
        private System.Windows.Forms.TextBox tbId;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button buttonNotes2;
        private System.Windows.Forms.Label labelNumberNotes2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton radioButtonReconcDiff;
        private System.Windows.Forms.Button buttonFinish;
        private System.Windows.Forms.Button buttonShowMaps;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label labelToday;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label labelUserId;
    }
}