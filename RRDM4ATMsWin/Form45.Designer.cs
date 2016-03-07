namespace RRDM4ATMsWin
{
    partial class Form45
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
            this.label2 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.groupNoDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.descriptionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.moreThanOneBankDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.statsDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.replenishmentDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.reconciliationDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dtTmCreatedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bankIdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.inactiveDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.operatorDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.aTMSDataSet52 = new RRDM4ATMsWin.ATMSDataSet52();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.textBox9 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button7 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.tableLayoutPanelHeader = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.labelStep1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label8 = new System.Windows.Forms.Label();
            this.labelToday = new System.Windows.Forms.Label();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.textBoxMsgBoard = new System.Windows.Forms.TextBox();
            this.groupsTableAdapter = new RRDM4ATMsWin.ATMSDataSet52TableAdapters.GroupsTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.aTMSDataSet52)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tableLayoutPanelHeader.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanelMain.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.label2.Location = new System.Drawing.Point(12, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(265, 18);
            this.label2.TabIndex = 277;
            this.label2.Text = "LIST AND ACTIONS ON GROUPS";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.groupNoDataGridViewTextBoxColumn,
            this.descriptionDataGridViewTextBoxColumn,
            this.moreThanOneBankDataGridViewCheckBoxColumn,
            this.statsDataGridViewCheckBoxColumn,
            this.replenishmentDataGridViewCheckBoxColumn,
            this.reconciliationDataGridViewCheckBoxColumn,
            this.dtTmCreatedDataGridViewTextBoxColumn,
            this.bankIdDataGridViewTextBoxColumn,
            this.inactiveDataGridViewCheckBoxColumn,
            this.operatorDataGridViewTextBoxColumn});
            this.dataGridView1.DataSource = this.groupsBindingSource;
            this.dataGridView1.GridColor = System.Drawing.Color.LightGray;
            this.dataGridView1.Location = new System.Drawing.Point(8, 41);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(926, 317);
            this.dataGridView1.TabIndex = 270;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            this.dataGridView1.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_RowEnter);
            // 
            // groupNoDataGridViewTextBoxColumn
            // 
            this.groupNoDataGridViewTextBoxColumn.DataPropertyName = "GroupNo";
            this.groupNoDataGridViewTextBoxColumn.HeaderText = "GroupNo";
            this.groupNoDataGridViewTextBoxColumn.Name = "groupNoDataGridViewTextBoxColumn";
            this.groupNoDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // descriptionDataGridViewTextBoxColumn
            // 
            this.descriptionDataGridViewTextBoxColumn.DataPropertyName = "Description";
            this.descriptionDataGridViewTextBoxColumn.HeaderText = "Description";
            this.descriptionDataGridViewTextBoxColumn.Name = "descriptionDataGridViewTextBoxColumn";
            this.descriptionDataGridViewTextBoxColumn.ReadOnly = true;
            this.descriptionDataGridViewTextBoxColumn.Width = 130;
            // 
            // moreThanOneBankDataGridViewCheckBoxColumn
            // 
            this.moreThanOneBankDataGridViewCheckBoxColumn.DataPropertyName = "MoreThanOneBank";
            this.moreThanOneBankDataGridViewCheckBoxColumn.HeaderText = "MoreThanOneBank";
            this.moreThanOneBankDataGridViewCheckBoxColumn.Name = "moreThanOneBankDataGridViewCheckBoxColumn";
            this.moreThanOneBankDataGridViewCheckBoxColumn.ReadOnly = true;
            // 
            // statsDataGridViewCheckBoxColumn
            // 
            this.statsDataGridViewCheckBoxColumn.DataPropertyName = "Stats";
            this.statsDataGridViewCheckBoxColumn.HeaderText = "Stats";
            this.statsDataGridViewCheckBoxColumn.Name = "statsDataGridViewCheckBoxColumn";
            this.statsDataGridViewCheckBoxColumn.ReadOnly = true;
            // 
            // replenishmentDataGridViewCheckBoxColumn
            // 
            this.replenishmentDataGridViewCheckBoxColumn.DataPropertyName = "Replenishment";
            this.replenishmentDataGridViewCheckBoxColumn.HeaderText = "Replenishment";
            this.replenishmentDataGridViewCheckBoxColumn.Name = "replenishmentDataGridViewCheckBoxColumn";
            this.replenishmentDataGridViewCheckBoxColumn.ReadOnly = true;
            // 
            // reconciliationDataGridViewCheckBoxColumn
            // 
            this.reconciliationDataGridViewCheckBoxColumn.DataPropertyName = "Reconciliation";
            this.reconciliationDataGridViewCheckBoxColumn.HeaderText = "Reconciliation";
            this.reconciliationDataGridViewCheckBoxColumn.Name = "reconciliationDataGridViewCheckBoxColumn";
            this.reconciliationDataGridViewCheckBoxColumn.ReadOnly = true;
            // 
            // dtTmCreatedDataGridViewTextBoxColumn
            // 
            this.dtTmCreatedDataGridViewTextBoxColumn.DataPropertyName = "DtTmCreated";
            this.dtTmCreatedDataGridViewTextBoxColumn.HeaderText = "DtTmCreated";
            this.dtTmCreatedDataGridViewTextBoxColumn.Name = "dtTmCreatedDataGridViewTextBoxColumn";
            this.dtTmCreatedDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // bankIdDataGridViewTextBoxColumn
            // 
            this.bankIdDataGridViewTextBoxColumn.DataPropertyName = "BankId";
            this.bankIdDataGridViewTextBoxColumn.HeaderText = "BankId";
            this.bankIdDataGridViewTextBoxColumn.Name = "bankIdDataGridViewTextBoxColumn";
            this.bankIdDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // inactiveDataGridViewCheckBoxColumn
            // 
            this.inactiveDataGridViewCheckBoxColumn.DataPropertyName = "Inactive";
            this.inactiveDataGridViewCheckBoxColumn.HeaderText = "Inactive";
            this.inactiveDataGridViewCheckBoxColumn.Name = "inactiveDataGridViewCheckBoxColumn";
            this.inactiveDataGridViewCheckBoxColumn.ReadOnly = true;
            // 
            // operatorDataGridViewTextBoxColumn
            // 
            this.operatorDataGridViewTextBoxColumn.DataPropertyName = "Operator";
            this.operatorDataGridViewTextBoxColumn.HeaderText = "Operator";
            this.operatorDataGridViewTextBoxColumn.Name = "operatorDataGridViewTextBoxColumn";
            this.operatorDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // groupsBindingSource
            // 
            this.groupsBindingSource.DataMember = "Groups";
            this.groupsBindingSource.DataSource = this.aTMSDataSet52;
            // 
            // aTMSDataSet52
            // 
            this.aTMSDataSet52.DataSetName = "ATMSDataSet52";
            this.aTMSDataSet52.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // textBox8
            // 
            this.textBox8.Location = new System.Drawing.Point(5, 364);
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new System.Drawing.Size(99, 20);
            this.textBox8.TabIndex = 271;
            this.textBox8.Text = "Chosen Group No";
            // 
            // textBox9
            // 
            this.textBox9.Location = new System.Drawing.Point(107, 364);
            this.textBox9.Name = "textBox9";
            this.textBox9.ReadOnly = true;
            this.textBox9.Size = new System.Drawing.Size(75, 20);
            this.textBox9.TabIndex = 272;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(12, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 13);
            this.label1.TabIndex = 278;
            this.label1.Text = "Choose Group No ";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(107, 14);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(60, 20);
            this.textBox1.TabIndex = 279;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Transparent;
            this.button1.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.button1.FlatAppearance.BorderSize = 2;
            this.button1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(711, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(62, 24);
            this.button1.TabIndex = 281;
            this.button1.Text = "Delete";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.ForeColor = System.Drawing.Color.Black;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(955, 588);
            this.panel1.TabIndex = 283;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.textBox1);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.textBox9);
            this.panel2.Controls.Add(this.textBox8);
            this.panel2.Controls.Add(this.dataGridView1);
            this.panel2.Location = new System.Drawing.Point(10, 41);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(936, 516);
            this.panel2.TabIndex = 283;
            // 
            // button7
            // 
            this.button7.BackColor = System.Drawing.Color.Transparent;
            this.button7.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.button7.FlatAppearance.BorderSize = 2;
            this.button7.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.button7.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.button7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button7.ForeColor = System.Drawing.Color.White;
            this.button7.Location = new System.Drawing.Point(877, 3);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(62, 24);
            this.button7.TabIndex = 284;
            this.button7.Text = "Add";
            this.button7.UseVisualStyleBackColor = false;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button6
            // 
            this.button6.BackColor = System.Drawing.Color.Transparent;
            this.button6.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.button6.FlatAppearance.BorderSize = 2;
            this.button6.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.button6.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.button6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button6.ForeColor = System.Drawing.Color.White;
            this.button6.Location = new System.Drawing.Point(794, 3);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(62, 24);
            this.button6.TabIndex = 283;
            this.button6.Text = "Update";
            this.button6.UseVisualStyleBackColor = false;
            this.button6.Click += new System.EventHandler(this.button6_Click);
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
            this.tableLayoutPanelHeader.RowCount = 2;
            this.tableLayoutPanelHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 84F));
            this.tableLayoutPanelHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            this.tableLayoutPanelHeader.Size = new System.Drawing.Size(961, 96);
            this.tableLayoutPanelHeader.TabIndex = 286;
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
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(957, 80);
            this.tableLayoutPanel4.TabIndex = 249;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.labelStep1);
            this.flowLayoutPanel1.Controls.Add(this.label5);
            this.flowLayoutPanel1.Controls.Add(this.label4);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(294, 6);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(2, 6, 2, 2);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(461, 48);
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
            this.labelStep1.Size = new System.Drawing.Size(294, 36);
            this.labelStep1.TabIndex = 242;
            this.labelStep1.Text = "Group Maintenance";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(300, 8);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(27, 20);
            this.label5.TabIndex = 245;
            this.label5.Text = ">>";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(331, 8);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(27, 20);
            this.label4.TabIndex = 246;
            this.label4.Text = ">>";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 63F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 292F));
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(816, 2);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 49F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 13F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 13F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(139, 76);
            this.tableLayoutPanel1.TabIndex = 247;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 76F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 232F));
            this.tableLayoutPanel5.Controls.Add(this.pictureBox1, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel3, 1, 0);
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
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 67F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 161F));
            this.tableLayoutPanel3.Controls.Add(this.label8, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.labelToday, 1, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(78, 2);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(228, 63);
            this.tableLayoutPanel3.TabIndex = 248;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(2, 0);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 3, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(30, 13);
            this.label8.TabIndex = 250;
            this.label8.Text = "Date";
            // 
            // labelToday
            // 
            this.labelToday.AutoSize = true;
            this.labelToday.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelToday.Location = new System.Drawing.Point(69, 0);
            this.labelToday.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelToday.Name = "labelToday";
            this.labelToday.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.labelToday.Size = new System.Drawing.Size(42, 13);
            this.labelToday.TabIndex = 117;
            this.labelToday.Text = "Today";
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(649, -14);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker1.TabIndex = 284;
            // 
            // tableLayoutPanelMain
            // 
            this.tableLayoutPanelMain.ColumnCount = 1;
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanelMain.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelMain.Location = new System.Drawing.Point(0, 96);
            this.tableLayoutPanelMain.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            this.tableLayoutPanelMain.RowCount = 2;
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelMain.Size = new System.Drawing.Size(961, 634);
            this.tableLayoutPanelMain.TabIndex = 285;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 83F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 83F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 83F));
            this.tableLayoutPanel2.Controls.Add(this.button1, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.textBoxMsgBoard, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.button6, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.button7, 3, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(2, 596);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(957, 36);
            this.tableLayoutPanel2.TabIndex = 242;
            // 
            // textBoxMsgBoard
            // 
            this.textBoxMsgBoard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.textBoxMsgBoard.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxMsgBoard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxMsgBoard.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxMsgBoard.ForeColor = System.Drawing.Color.White;
            this.textBoxMsgBoard.Location = new System.Drawing.Point(10, 2);
            this.textBoxMsgBoard.Margin = new System.Windows.Forms.Padding(10, 2, 2, 2);
            this.textBoxMsgBoard.Multiline = true;
            this.textBoxMsgBoard.Name = "textBoxMsgBoard";
            this.textBoxMsgBoard.ReadOnly = true;
            this.textBoxMsgBoard.Size = new System.Drawing.Size(696, 32);
            this.textBoxMsgBoard.TabIndex = 242;
            this.textBoxMsgBoard.Text = "No guidance information available.";
            // 
            // groupsTableAdapter
            // 
            this.groupsTableAdapter.ClearBeforeFill = true;
            // 
            // Form45
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(961, 730);
            this.Controls.Add(this.tableLayoutPanelMain);
            this.Controls.Add(this.tableLayoutPanelHeader);
            this.Controls.Add(this.dateTimePicker1);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "Form45";
            this.Text = "Form45";
            this.Load += new System.EventHandler(this.Form45_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.aTMSDataSet52)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tableLayoutPanelHeader.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.tableLayoutPanel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox textBox8;
        private System.Windows.Forms.TextBox textBox9;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelHeader;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label labelStep1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label labelToday;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private System.Windows.Forms.TextBox textBoxMsgBoard;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button6;
        private ATMSDataSet52 aTMSDataSet52;
        private System.Windows.Forms.BindingSource groupsBindingSource;
        private ATMSDataSet52TableAdapters.GroupsTableAdapter groupsTableAdapter;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.DataGridViewTextBoxColumn groupNoDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn descriptionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn moreThanOneBankDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn statsDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn replenishmentDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn reconciliationDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dtTmCreatedDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn bankIdDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn inactiveDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn operatorDataGridViewTextBoxColumn;

    }
}