namespace RRDM4ATMsWin
{
    partial class Form76
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
            this.toolTipController = new System.Windows.Forms.ToolTip(this.components);
            this.toolTipMessages = new System.Windows.Forms.ToolTip(this.components);
            this.textBoxMsgBoard = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonNext = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.panelMain = new System.Windows.Forms.Panel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.panelFilters = new System.Windows.Forms.Panel();
            this.checkBoxDateRange = new System.Windows.Forms.CheckBox();
            this.buttonFilter = new System.Windows.Forms.Button();
            this.labelTo = new System.Windows.Forms.Label();
            this.dateTimePickerEnd = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerStart = new System.Windows.Forms.DateTimePicker();
            this.textBoxUserId = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.comboBoxType = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBoxSubCategory = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxCategory = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.auditTrailBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.aTMSDataSet17 = new RRDM4ATMsWin.ATMSDataSet17();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label47 = new System.Windows.Forms.Label();
            this.tableLayoutPanelHeader = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.labelStep1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.fillByToolStrip = new System.Windows.Forms.ToolStrip();
            this.fillByToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.auditTrailTableAdapter = new RRDM4ATMsWin.ATMSDataSet17TableAdapters.AuditTrailTableAdapter();
            this.uniqueIDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.categoryDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.subCategoryDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.typeOfChangeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.userIDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dateTimeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.screenshotDataGridViewImageColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.Action = new System.Windows.Forms.DataGridViewLinkColumn();
            this.ScreenshotPriorChange = new System.Windows.Forms.DataGridViewImageColumn();
            this.Message = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanelMain.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.panelFilters.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.auditTrailBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.aTMSDataSet17)).BeginInit();
            this.tableLayoutPanelHeader.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.fillByToolStrip.SuspendLayout();
            this.SuspendLayout();
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
            this.textBoxMsgBoard.Size = new System.Drawing.Size(647, 30);
            this.textBoxMsgBoard.TabIndex = 242;
            this.textBoxMsgBoard.Text = "No guidance information available.";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 659F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 83F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 83F));
            this.tableLayoutPanel2.Controls.Add(this.buttonNext, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.textBoxMsgBoard, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.Cancel, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(2, 480);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(909, 36);
            this.tableLayoutPanel2.TabIndex = 242;
            // 
            // buttonNext
            // 
            this.buttonNext.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonNext.FlatAppearance.BorderSize = 2;
            this.buttonNext.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonNext.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonNext.Location = new System.Drawing.Point(828, 2);
            this.buttonNext.Margin = new System.Windows.Forms.Padding(2, 2, 10, 2);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(71, 27);
            this.buttonNext.TabIndex = 244;
            this.buttonNext.Text = "Close";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // Cancel
            // 
            this.Cancel.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.Cancel.FlatAppearance.BorderSize = 2;
            this.Cancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.Cancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.Cancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Cancel.Location = new System.Drawing.Point(669, 2);
            this.Cancel.Margin = new System.Windows.Forms.Padding(10, 2, 2, 2);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(72, 27);
            this.Cancel.TabIndex = 245;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Visible = false;
            // 
            // tableLayoutPanelMain
            // 
            this.tableLayoutPanelMain.ColumnCount = 1;
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanelMain.Controls.Add(this.panelMain, 0, 0);
            this.tableLayoutPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelMain.ForeColor = System.Drawing.Color.White;
            this.tableLayoutPanelMain.Location = new System.Drawing.Point(0, 82);
            this.tableLayoutPanelMain.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            this.tableLayoutPanelMain.RowCount = 2;
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelMain.Size = new System.Drawing.Size(913, 518);
            this.tableLayoutPanelMain.TabIndex = 257;
            // 
            // panelMain
            // 
            this.panelMain.AutoScroll = true;
            this.panelMain.BackColor = System.Drawing.SystemColors.Control;
            this.panelMain.Controls.Add(this.tableLayoutPanel3);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(2, 2);
            this.panelMain.Margin = new System.Windows.Forms.Padding(2);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(909, 474);
            this.panelMain.TabIndex = 245;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 7F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 7F));
            this.tableLayoutPanel3.Controls.Add(this.panelFilters, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.panel1, 1, 3);
            this.tableLayoutPanel3.Controls.Add(this.label12, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.label47, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 5;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 143F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(909, 474);
            this.tableLayoutPanel3.TabIndex = 394;
            // 
            // panelFilters
            // 
            this.panelFilters.BackColor = System.Drawing.Color.White;
            this.panelFilters.Controls.Add(this.checkBoxDateRange);
            this.panelFilters.Controls.Add(this.buttonFilter);
            this.panelFilters.Controls.Add(this.labelTo);
            this.panelFilters.Controls.Add(this.dateTimePickerEnd);
            this.panelFilters.Controls.Add(this.dateTimePickerStart);
            this.panelFilters.Controls.Add(this.textBoxUserId);
            this.panelFilters.Controls.Add(this.label9);
            this.panelFilters.Controls.Add(this.comboBoxType);
            this.panelFilters.Controls.Add(this.label5);
            this.panelFilters.Controls.Add(this.comboBoxSubCategory);
            this.panelFilters.Controls.Add(this.label4);
            this.panelFilters.Controls.Add(this.label2);
            this.panelFilters.Controls.Add(this.comboBoxCategory);
            this.panelFilters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFilters.ForeColor = System.Drawing.Color.Black;
            this.panelFilters.Location = new System.Drawing.Point(9, 28);
            this.panelFilters.Margin = new System.Windows.Forms.Padding(2);
            this.panelFilters.Name = "panelFilters";
            this.panelFilters.Size = new System.Drawing.Size(891, 139);
            this.panelFilters.TabIndex = 0;
            this.panelFilters.Paint += new System.Windows.Forms.PaintEventHandler(this.panelFilters_Paint);
            // 
            // checkBoxDateRange
            // 
            this.checkBoxDateRange.AutoSize = true;
            this.checkBoxDateRange.Location = new System.Drawing.Point(87, 81);
            this.checkBoxDateRange.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxDateRange.Name = "checkBoxDateRange";
            this.checkBoxDateRange.Size = new System.Drawing.Size(106, 17);
            this.checkBoxDateRange.TabIndex = 407;
            this.checkBoxDateRange.Text = "Use Date Range";
            this.checkBoxDateRange.UseVisualStyleBackColor = true;
            this.checkBoxDateRange.CheckedChanged += new System.EventHandler(this.checkBoxDateRange_CheckedChanged);
            // 
            // buttonFilter
            // 
            this.buttonFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonFilter.BackColor = System.Drawing.Color.White;
            this.buttonFilter.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonFilter.FlatAppearance.BorderSize = 2;
            this.buttonFilter.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonFilter.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonFilter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonFilter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonFilter.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonFilter.Location = new System.Drawing.Point(769, 108);
            this.buttonFilter.Name = "buttonFilter";
            this.buttonFilter.Size = new System.Drawing.Size(98, 24);
            this.buttonFilter.TabIndex = 406;
            this.buttonFilter.Text = "Filter";
            this.buttonFilter.UseVisualStyleBackColor = false;
            this.buttonFilter.Click += new System.EventHandler(this.buttonFilter_Click);
            // 
            // labelTo
            // 
            this.labelTo.AutoSize = true;
            this.labelTo.Enabled = false;
            this.labelTo.Location = new System.Drawing.Point(261, 107);
            this.labelTo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelTo.Name = "labelTo";
            this.labelTo.Size = new System.Drawing.Size(16, 13);
            this.labelTo.TabIndex = 403;
            this.labelTo.Text = "to";
            // 
            // dateTimePickerEnd
            // 
            this.dateTimePickerEnd.Enabled = false;
            this.dateTimePickerEnd.Location = new System.Drawing.Point(288, 104);
            this.dateTimePickerEnd.Margin = new System.Windows.Forms.Padding(2);
            this.dateTimePickerEnd.Name = "dateTimePickerEnd";
            this.dateTimePickerEnd.Size = new System.Drawing.Size(135, 20);
            this.dateTimePickerEnd.TabIndex = 401;
            // 
            // dateTimePickerStart
            // 
            this.dateTimePickerStart.Enabled = false;
            this.dateTimePickerStart.Location = new System.Drawing.Point(87, 103);
            this.dateTimePickerStart.Margin = new System.Windows.Forms.Padding(2);
            this.dateTimePickerStart.Name = "dateTimePickerStart";
            this.dateTimePickerStart.Size = new System.Drawing.Size(151, 20);
            this.dateTimePickerStart.TabIndex = 400;
            // 
            // textBoxUserId
            // 
            this.textBoxUserId.Location = new System.Drawing.Point(87, 48);
            this.textBoxUserId.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxUserId.Name = "textBoxUserId";
            this.textBoxUserId.Size = new System.Drawing.Size(151, 20);
            this.textBoxUserId.TabIndex = 399;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(19, 50);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(43, 13);
            this.label9.TabIndex = 398;
            this.label9.Text = "User ID";
            // 
            // comboBoxType
            // 
            this.comboBoxType.FormattingEnabled = true;
            this.comboBoxType.Location = new System.Drawing.Point(592, 16);
            this.comboBoxType.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxType.Name = "comboBoxType";
            this.comboBoxType.Size = new System.Drawing.Size(151, 21);
            this.comboBoxType.TabIndex = 397;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(507, 18);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 13);
            this.label5.TabIndex = 396;
            this.label5.Text = "Type of Change";
            // 
            // comboBoxSubCategory
            // 
            this.comboBoxSubCategory.FormattingEnabled = true;
            this.comboBoxSubCategory.Location = new System.Drawing.Point(332, 16);
            this.comboBoxSubCategory.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxSubCategory.Name = "comboBoxSubCategory";
            this.comboBoxSubCategory.Size = new System.Drawing.Size(151, 21);
            this.comboBoxSubCategory.TabIndex = 395;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(260, 18);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 13);
            this.label4.TabIndex = 394;
            this.label4.Text = "SubCategory";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 18);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 393;
            this.label2.Text = "Category";
            // 
            // comboBoxCategory
            // 
            this.comboBoxCategory.FormattingEnabled = true;
            this.comboBoxCategory.Location = new System.Drawing.Point(87, 16);
            this.comboBoxCategory.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxCategory.Name = "comboBoxCategory";
            this.comboBoxCategory.Size = new System.Drawing.Size(151, 21);
            this.comboBoxCategory.TabIndex = 392;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.tableLayoutPanel6);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.ForeColor = System.Drawing.Color.Black;
            this.panel1.Location = new System.Drawing.Point(9, 197);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(891, 265);
            this.panel1.TabIndex = 392;
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 1;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 13F));
            this.tableLayoutPanel6.Controls.Add(this.dataGridView1, 0, 1);
            this.tableLayoutPanel6.Controls.Add(this.label13, 0, 0);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel6.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 2;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(891, 265);
            this.tableLayoutPanel6.TabIndex = 2;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.uniqueIDDataGridViewTextBoxColumn,
            this.categoryDataGridViewTextBoxColumn,
            this.subCategoryDataGridViewTextBoxColumn,
            this.typeOfChangeDataGridViewTextBoxColumn,
            this.userIDDataGridViewTextBoxColumn,
            this.dateTimeDataGridViewTextBoxColumn,
            this.screenshotDataGridViewImageColumn,
            this.Action,
            this.ScreenshotPriorChange,
            this.Message});
            this.dataGridView1.DataSource = this.auditTrailBindingSource;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(10, 29);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(10, 3, 10, 10);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 28;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(871, 226);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            this.dataGridView1.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentDoubleClick);
            this.dataGridView1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellDoubleClick);
            // 
            // auditTrailBindingSource
            // 
            this.auditTrailBindingSource.DataMember = "AuditTrail";
            this.auditTrailBindingSource.DataSource = this.aTMSDataSet17;
            this.auditTrailBindingSource.CurrentChanged += new System.EventHandler(this.auditTrailBindingSource_CurrentChanged);
            // 
            // aTMSDataSet17
            // 
            this.aTMSDataSet17.DataSetName = "ATMSDataSet17";
            this.aTMSDataSet17.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(10, 10);
            this.label13.Margin = new System.Windows.Forms.Padding(10, 10, 10, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(377, 13);
            this.label13.TabIndex = 1;
            this.label13.Text = "Press \"View\" or double click a row to view the screenshot if the change made.";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.BackColor = System.Drawing.Color.Transparent;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.label12.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.label12.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label12.Location = new System.Drawing.Point(10, 175);
            this.label12.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(101, 16);
            this.label12.TabIndex = 393;
            this.label12.Text = "AUDIT TRAIL";
            // 
            // label47
            // 
            this.label47.AutoSize = true;
            this.label47.BackColor = System.Drawing.Color.Transparent;
            this.label47.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.label47.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.label47.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label47.Location = new System.Drawing.Point(10, 6);
            this.label47.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(70, 16);
            this.label47.TabIndex = 391;
            this.label47.Text = "FILTERS";
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
            this.tableLayoutPanelHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 84F));
            this.tableLayoutPanelHeader.Size = new System.Drawing.Size(913, 82);
            this.tableLayoutPanelHeader.TabIndex = 258;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 3;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 91F));
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
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(909, 80);
            this.tableLayoutPanel4.TabIndex = 249;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.labelStep1);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(93, 6);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(2, 6, 2, 2);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(579, 63);
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
            this.labelStep1.Size = new System.Drawing.Size(162, 36);
            this.labelStep1.TabIndex = 242;
            this.labelStep1.Text = "Audit Trail";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 63F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 292F));
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(768, 2);
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
            this.tableLayoutPanel5.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 67F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(78, 67);
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
            // fillByToolStrip
            // 
            this.fillByToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fillByToolStripButton});
            this.fillByToolStrip.Location = new System.Drawing.Point(0, 82);
            this.fillByToolStrip.Name = "fillByToolStrip";
            this.fillByToolStrip.Size = new System.Drawing.Size(913, 25);
            this.fillByToolStrip.TabIndex = 259;
            this.fillByToolStrip.Text = "fillByToolStrip";
            // 
            // fillByToolStripButton
            // 
            this.fillByToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.fillByToolStripButton.Name = "fillByToolStripButton";
            this.fillByToolStripButton.Size = new System.Drawing.Size(39, 22);
            this.fillByToolStripButton.Text = "FillBy";
            this.fillByToolStripButton.Click += new System.EventHandler(this.fillByToolStripButton_Click);
            // 
            // auditTrailTableAdapter
            // 
            this.auditTrailTableAdapter.ClearBeforeFill = true;
            // 
            // uniqueIDDataGridViewTextBoxColumn
            // 
            this.uniqueIDDataGridViewTextBoxColumn.DataPropertyName = "UniqueID";
            this.uniqueIDDataGridViewTextBoxColumn.HeaderText = "UniqueID";
            this.uniqueIDDataGridViewTextBoxColumn.Name = "uniqueIDDataGridViewTextBoxColumn";
            this.uniqueIDDataGridViewTextBoxColumn.ReadOnly = true;
            this.uniqueIDDataGridViewTextBoxColumn.Visible = false;
            // 
            // categoryDataGridViewTextBoxColumn
            // 
            this.categoryDataGridViewTextBoxColumn.DataPropertyName = "Category";
            this.categoryDataGridViewTextBoxColumn.HeaderText = "Category";
            this.categoryDataGridViewTextBoxColumn.Name = "categoryDataGridViewTextBoxColumn";
            this.categoryDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // subCategoryDataGridViewTextBoxColumn
            // 
            this.subCategoryDataGridViewTextBoxColumn.DataPropertyName = "SubCategory";
            this.subCategoryDataGridViewTextBoxColumn.HeaderText = "Sub Category";
            this.subCategoryDataGridViewTextBoxColumn.Name = "subCategoryDataGridViewTextBoxColumn";
            this.subCategoryDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // typeOfChangeDataGridViewTextBoxColumn
            // 
            this.typeOfChangeDataGridViewTextBoxColumn.DataPropertyName = "TypeOfChange";
            this.typeOfChangeDataGridViewTextBoxColumn.HeaderText = "Type Of Change";
            this.typeOfChangeDataGridViewTextBoxColumn.Name = "typeOfChangeDataGridViewTextBoxColumn";
            this.typeOfChangeDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // userIDDataGridViewTextBoxColumn
            // 
            this.userIDDataGridViewTextBoxColumn.DataPropertyName = "UserId";
            this.userIDDataGridViewTextBoxColumn.HeaderText = "User Id";
            this.userIDDataGridViewTextBoxColumn.Name = "userIDDataGridViewTextBoxColumn";
            this.userIDDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // dateTimeDataGridViewTextBoxColumn
            // 
            this.dateTimeDataGridViewTextBoxColumn.DataPropertyName = "DateTime";
            this.dateTimeDataGridViewTextBoxColumn.HeaderText = "Date of Change";
            this.dateTimeDataGridViewTextBoxColumn.Name = "dateTimeDataGridViewTextBoxColumn";
            this.dateTimeDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // screenshotDataGridViewImageColumn
            // 
            this.screenshotDataGridViewImageColumn.DataPropertyName = "Screenshot";
            this.screenshotDataGridViewImageColumn.HeaderText = "Screenshot";
            this.screenshotDataGridViewImageColumn.Name = "screenshotDataGridViewImageColumn";
            this.screenshotDataGridViewImageColumn.ReadOnly = true;
            this.screenshotDataGridViewImageColumn.Visible = false;
            // 
            // Action
            // 
            this.Action.HeaderText = "Action";
            this.Action.Name = "Action";
            this.Action.ReadOnly = true;
            this.Action.Text = "View";
            this.Action.UseColumnTextForLinkValue = true;
            // 
            // ScreenshotPriorChange
            // 
            this.ScreenshotPriorChange.DataPropertyName = "ScreenshotPriorChange";
            this.ScreenshotPriorChange.HeaderText = "ScreenshotPriorChange";
            this.ScreenshotPriorChange.Name = "ScreenshotPriorChange";
            this.ScreenshotPriorChange.ReadOnly = true;
            this.ScreenshotPriorChange.Visible = false;
            // 
            // Message
            // 
            this.Message.DataPropertyName = "Message";
            this.Message.HeaderText = "Message";
            this.Message.Name = "Message";
            this.Message.ReadOnly = true;
            this.Message.Visible = false;
            // 
            // Form76
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(913, 600);
            this.Controls.Add(this.fillByToolStrip);
            this.Controls.Add(this.tableLayoutPanelMain);
            this.Controls.Add(this.tableLayoutPanelHeader);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "Form76";
            this.Text = "Form76";
            this.Load += new System.EventHandler(this.Form76_Load);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.panelMain.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.panelFilters.ResumeLayout(false);
            this.panelFilters.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.auditTrailBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.aTMSDataSet17)).EndInit();
            this.tableLayoutPanelHeader.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.tableLayoutPanel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.fillByToolStrip.ResumeLayout(false);
            this.fillByToolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTipController;
        private System.Windows.Forms.ToolTip toolTipMessages;
        private System.Windows.Forms.TextBox textBoxMsgBoard;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelHeader;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label labelStep1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Panel panelFilters;
        private System.Windows.Forms.Label label47;
        private System.Windows.Forms.Label labelTo;
        private System.Windows.Forms.DateTimePicker dateTimePickerEnd;
        private System.Windows.Forms.DateTimePicker dateTimePickerStart;
        private System.Windows.Forms.TextBox textBoxUserId;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox comboBoxType;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBoxSubCategory;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxCategory;
        private System.Windows.Forms.Button buttonFilter;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label13;
        private ATMSDataSet17 aTMSDataSet17;
        private System.Windows.Forms.BindingSource auditTrailBindingSource;
        private ATMSDataSet17TableAdapters.AuditTrailTableAdapter auditTrailTableAdapter;
    //    private System.Windows.Forms.DataGridViewTextBoxColumn typeOfChageDataGridViewTextBoxColumn;
        private System.Windows.Forms.CheckBox checkBoxDateRange;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.ToolStrip fillByToolStrip;
        private System.Windows.Forms.ToolStripButton fillByToolStripButton;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.DataGridViewTextBoxColumn uniqueIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn categoryDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn subCategoryDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn typeOfChangeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn userIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dateTimeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewImageColumn screenshotDataGridViewImageColumn;
        private System.Windows.Forms.DataGridViewLinkColumn Action;
        private System.Windows.Forms.DataGridViewImageColumn ScreenshotPriorChange;
        private System.Windows.Forms.DataGridViewTextBoxColumn Message;
    }
}