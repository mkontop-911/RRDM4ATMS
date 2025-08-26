using System;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

// Alecos

namespace RRDM4ATMsWin
{
    public partial class Form502a : Form
    {
        RRDMMatchingCategories Mc = new RRDMMatchingCategories();

        RRDMMatchingSourceFiles Rs = new RRDMMatchingSourceFiles();
        RRDMMatchingCategoriesVsSourcesFiles Rcs = new RRDMMatchingCategoriesVsSourcesFiles();
        //RRDMMatchingFields Rmf = new RRDMMatchingFields();
        RRDMMatchingCategStageVsMatchingFields Rsm = new RRDMMatchingCategStageVsMatchingFields();
        RRDMUniversalTableFieldsDefinition Utd = new RRDMUniversalTableFieldsDefinition();

        //  int WRowIndex; 
        string WSourceFileName;
        //string WReconcCategory;
        int WSeqNo;
        int WSeqNo2;

        string WCategoryId;
        string WCategoryNm;

        string WSourceFileNameX;
        string WSourceFileNameY;

        string WMatchingField;
        string WWMatchingField;

        string WTableStructureId;

        int WSortSequence;

        string WStage;

        bool ButtonPressForStage;

        string W_Application;

        string WComboboxText;

        int WRowGrid1;
        int WRowGrid3;

        string WCategoryIdAndName;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        public Form502a(string InSignedId, int SignRecordNo, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;

            InitializeComponent();

            // Set Working Date 

            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;
            labelUserId.Text = InSignedId;

            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.RecordFound == true)
            {
                W_Application = Usi.SignInApplication;

                //W_Application = "e_MOBILE"; 

                if (W_Application == "e_MOBILE")
                {
                    if (Usi.WFieldNumeric11 == 11)
                    {
                        W_Application = "ETISALAT";
                    }
                    if (Usi.WFieldNumeric11 == 12)
                    {
                        W_Application = "QAHERA";
                    }
                    if (Usi.WFieldNumeric11 == 13)
                    {
                        W_Application = "IPN";
                    }
                    if (Usi.WFieldNumeric11 == 15)
                    {
                        W_Application = "EGATE";
                    }
                    WTableStructureId = "QAHERA";

                }
                else
                {

                    WTableStructureId = "Atms And Cards";

                }
            }

            // Categories
            if (W_Application == "ETISALAT" || W_Application == "QAHERA" || W_Application == "IPN" || W_Application == "EGATE")
            {
                comboBoxMatchingCateg.DataSource = Mc.GetCategories_e_MOBILE(WOperator);

                comboBoxMatchingCateg.DisplayMember = "DisplayValue";
            }
            else
            {
                comboBoxMatchingCateg.DataSource = Mc.GetCategories(WOperator);

                comboBoxMatchingCateg.DisplayMember = "DisplayValue";
            }


            textBoxMsgBoard.Text = "Select Category and proceed to Files and Stages definition. ";

            buttonAllStages.BackColor = Color.Red;
        }

        private void Form502a_Load(object sender, EventArgs e)
        {
            Mc.ReadMatchingCategorybyActiveCategId(WOperator, WCategoryId);
            if (Mc.RecordFound == false)
            {
                MessageBox.Show("No Reconciliation Matching Categories created yet");
                return;
            }



            // MATCHING FLOW
            Rcs.ReadReconcCategoriesVsSourcesAll(WCategoryId);
            if (Rcs.RecordFound == true)
            {
                labelFlow.Show();
                labelFlow.Text = "MATCHING FLOW FOR " + WCategoryNm;

                panelFlow.Show();
                labelFileA.Text = Rcs.SourceFileNameA;
                labelFileB.Text = Rcs.SourceFileNameB;
                labelFileC.Text = Rcs.SourceFileNameC;
                labelFileD.Text = Rcs.SourceFileNameD;
                labelFileE.Text = Rcs.SourceFileNameE;

                if (Rcs.TotalRecords == 1)
                {
                    textBoxFileA.Show();
                    buttonStageA.Hide();
                    textBoxFileB.Hide();
                    buttonStageB.Hide();
                    textBoxFileC.Hide();
                    buttonStageC.Hide();
                    textBoxFileD.Hide();
                    buttonStageD.Hide();
                    textBoxFileE.Hide();
                    buttonAllStages.Hide();
                }

                if (Rcs.TotalRecords == 2)
                {
                    textBoxFileA.Show();
                    buttonStageA.Show();
                    textBoxFileB.Show();

                    buttonStageB.Hide();
                    textBoxFileC.Hide();
                    buttonStageC.Hide();
                    textBoxFileD.Hide();
                    buttonStageD.Hide();
                    textBoxFileE.Hide();
                    buttonAllStages.Hide();

                }

                if (Rcs.TotalRecords == 3)
                {
                    textBoxFileA.Show();

                    textBoxFileB.Show();
                    buttonStageB.Show();
                    textBoxFileC.Show();

                    buttonStageC.Hide();
                    textBoxFileD.Hide();
                    buttonStageD.Hide();
                    textBoxFileE.Hide();
                    buttonAllStages.Show();
                }
                if (Rcs.TotalRecords == 4)
                {
                    textBoxFileA.Show();
                    buttonStageA.Show();
                    textBoxFileB.Show();
                    buttonStageB.Show();
                    textBoxFileC.Show();
                    buttonStageC.Show();
                    textBoxFileD.Show();
                    buttonStageD.Hide();
                    textBoxFileE.Hide();
                    buttonAllStages.Show();
                }
                if (Rcs.TotalRecords == 5)
                {
                    textBoxFileA.Show();
                    buttonStageA.Show();
                    textBoxFileB.Show();
                    buttonStageB.Show();
                    textBoxFileC.Show();
                    buttonStageC.Show();
                    textBoxFileD.Show();
                    buttonStageD.Show();
                    textBoxFileE.Show();
                    buttonAllStages.Show();
                }
            }
            else
            {
                labelFlow.Hide();
                labelMatching.Hide();
                labelSelectedMatch.Hide();
                panelFlow.Hide();
                panelMatchFields.Hide();
                panelSelectFields.Hide();
                button1.Hide();
                button2.Hide();
            }

            // SELECT MATCHING FIELDS 
            Rsm.ReadReconcCategVsMatchingFieldsAll(WCategoryId);
            if (Rsm.RecordFound == true)
            {

                labelMatching.Show();
                labelSelectedMatch.Show();

                panelMatchFields.Show();
                panelSelectFields.Show();
                button1.Show();
                button2.Show();
            }
            else
            {
                labelMatching.Hide();
                panelSelectFields.Hide();
                labelSelectedMatch.Hide();
                panelMatchFields.Hide();
                button1.Hide();
                button2.Hide();
            }


            // Source Files (Grid-ONE)
            string Filter1 = "";
            if (W_Application == "ETISALAT" || W_Application == "QAHERA" || W_Application == "IPN" || W_Application == "EGATE")
            {
                //Filter1 = "Operator = '" + WOperator + "'  AND TableStructureId = '" + WTableStructureId + "' AND (Enabled = 1 OR Enabled = 0) " +
                //    "   ";
                Filter1 = "Operator = '" + WOperator + "' AND SystemOfOrigin='" + W_Application + "' AND (Enabled = 1 OR Right(SourceFileId, 5) = 'TWINS') ";
                ShowGridSourceFiles(Filter1);
            }
            else
            {
                Filter1 = "Operator = '" + WOperator + "' AND TableStructureId = '" + WTableStructureId + "'" + " AND (Enabled = 1 OR SourceFileId = 'Switch_IST_Txns_TWIN')";
                ShowGridSourceFiles(Filter1);
            }

            // Selected Files (TWO)
            string Filter2 = "CategoryId = '" + WCategoryId + "' ";
            ShowGridCategVsSourceFiles(Filter2);

            // Matching Fields (THREE)
            string Filter3 = "";
            if (W_Application == "ETISALAT" || W_Application == "QAHERA" || W_Application == "IPN" || W_Application == "EGATE")
            {
                Filter3 = "Operator = '" + WOperator + "' ";
                ShowGridMatchingFields(Filter3);
            }
            else
            {
                Filter3 = "Operator = '" + WOperator + "' ";
                ShowGridMatchingFields(Filter3);
            }
           

            // Category Vs Matching Fields (FOUR)
            string Filter4 = "CategoryId = '" + WCategoryId + "' ";
            ShowGridCategStageVsMatchingFields(Filter4);


        }


        private void dataGridView1_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WSeqNo = (int)rowSelected.Cells[0].Value;

            Rs.ReadReconcSourceFilesBySeqNo(WSeqNo);

            WSourceFileName = Rs.SourceFileId;

        }
        // Row enter for Category vs Source files 
        private void dataGridView2_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];
            WSeqNo2 = (int)rowSelected.Cells[0].Value;

            Rcs.ReadReconcCategoriesVsSourcebySeqNo(WSeqNo2);

            WSourceFileName = Rcs.SourceFileName;

        }

        // Row Enter For Matching fields 
        private void dataGridView3_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView3.Rows[e.RowIndex];

            WSeqNo = (int)rowSelected.Cells[0].Value;

            //Rmf.ReadReconcMatchingFieldsSeqNo(WSeqNo);
            Utd.ReadUniversalTableFieldsDefinitionSeqNo(WSeqNo);

            WMatchingField = Utd.FieldName;
            WSortSequence = Utd.SortSequence; 
        }
        // ROW ENTER for Cat matching 
        private void dataGridView4_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView4.Rows[e.RowIndex];

            WSeqNo = (int)rowSelected.Cells[0].Value;

            Rsm.ReadReconcCategStageVsMatchingField(WSeqNo);

            WWMatchingField = Rsm.MatchingField;

        }
        // Add Sources to category 
        private void buttonAdd_Click_1(object sender, EventArgs e)
        {
            if (WOperator != "ITMX")
            {
                Rcs.ReadReconcCategoriesVsSources(WCategoryId, WSourceFileName);
                if (Rcs.RecordFound == true)
                {
                    MessageBox.Show("Record already exist in Cat vs Source");
                    return;
                }
            }

            WRowGrid1 = dataGridView1.SelectedRows[0].Index;

            Rcs.CategoryId = WCategoryId;
            Rcs.SourceFileName = WSourceFileName;

            Rcs.InsertReconcCategoryVsSourceRecord();

            WRowGrid1 = dataGridView1.SelectedRows[0].Index;

            Form502a_Load(this, new EventArgs()); // Load Grid    

            //// Selected Files (TWO)
            //string Filter2 = "CategoryId = '" + WCategoryId + "' ";
            //ShowGridCategVsSourceFiles(Filter2);

            textBoxMsgBoard.Text = "Source File Added to Reconciliation Category. ";
        }

        // Remove 
        private void buttonRemove_Click_1(object sender, EventArgs e)
        {

            Rcs.DeleteReconcCategoryVsSourceRecord(WSeqNo2);

            Rsm.DeleteAllReconcCategVsMatchingFieldsForSourceFile(WCategoryId, WSourceFileName);

            Form502a_Load(this, new EventArgs());

            textBoxMsgBoard.Text = "Source File Removed from Reconciliation Category. ";

        }
        // Add to categories / matching fields 
        private void button1_Click_1(object sender, EventArgs e)
        {
            if (ButtonPressForStage == false)
            {
                MessageBox.Show("Press Button Of Stage");
                return;
            }

            WRowGrid3 = dataGridView3.SelectedRows[0].Index;

            Rsm.ReadReconcCategVsMatchingFields(WCategoryId, WSourceFileNameX, WSourceFileNameY, WMatchingField);

            if (Rsm.RecordFound == true)
            {
                MessageBox.Show("Record already exist in Cat vs Matching Field");
                return;
            }

            Rsm.CategoryId = WCategoryId;
            Rsm.Stage = WStage;
            Rsm.SourceFileNameA = WSourceFileNameX;

            Rsm.SourceFileNameB = WSourceFileNameY;

            Rsm.MatchingField = WMatchingField;

            Rsm.SortSequence = WSortSequence; 

            Rsm.InsertReconcCategVsMatchingFieldsRecord();

            //Form502a_Load(this, new EventArgs());

            // Category Vs Matching Fields 
            string Filter = "CategoryId = '" + WCategoryId + "' "
                                                            + " AND SourceFileNameA = '" + WSourceFileNameX + "' "
                                                            + " AND SourceFileNameB = '" + WSourceFileNameY + "' ";

            ShowGridCategStageVsMatchingFields(Filter);


        }

        // Remove from categories / matching fields 
        private void button2_Click_1(object sender, EventArgs e)
        {
            if (ButtonPressForStage == false)
            {
                MessageBox.Show("Press Button Of Stage ");
                return;
            }

            Rsm.DeleteReconcCategVsMatchingFieldsRecord(WCategoryId, WSourceFileNameX, WSourceFileNameY, WWMatchingField);

            WRowGrid3 = dataGridView3.SelectedRows[0].Index;
            Form502a_Load(this, new EventArgs());

            // Category Vs Matching Fields 
            string Filter = "CategoryId = '" + WCategoryId + "' "
                                                            + " AND SourceFileNameA = '" + WSourceFileNameX + "' "
                                                            + " AND SourceFileNameB = '" + WSourceFileNameY + "' ";

            ShowGridCategStageVsMatchingFields(Filter);


        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            WCategoryIdAndName = comboBoxMatchingCateg.Text;

            string s = WCategoryIdAndName;
            // Split string on spaces.
            // ... This will separate all the words.
            string[] words = s.Split('*');

            WCategoryId = words[0]; // Get the Category from string 
            WCategoryNm = words[1];

            Form502a_Load(this, new EventArgs());

        }


        // Stage 
        private void buttonStageA_Click_1(object sender, EventArgs e)
        {

            WStage = "Stage A";
            // MATCHING FLOW
            Rcs.ReadReconcCategoriesVsSourcesAll(WCategoryId);

            WSourceFileNameX = Rcs.SourceFileNameA;

            WSourceFileNameY = Rcs.SourceFileNameB;

            // Category Vs Matching Fields 
            string Filter = "CategoryId = '" + WCategoryId + "' "
                                                            + " AND SourceFileNameA = '" + WSourceFileNameX + "' "
                                                            + " AND SourceFileNameB = '" + WSourceFileNameY + "' ";

            ShowGridCategStageVsMatchingFields(Filter);

            ButtonPressForStage = true;

            buttonStageA.BackColor = Color.Red;
            buttonStageB.BackColor = Color.White;
            buttonStageC.BackColor = Color.White;
            buttonStageD.BackColor = Color.White;
            buttonAllStages.BackColor = Color.White;

            labelMatching.Show();
            labelSelectedMatch.Show();

            panelMatchFields.Show();
            panelSelectFields.Show();
            button1.Show();
            button2.Show();

        }
        // Stage B
        private void buttonStageB_Click(object sender, EventArgs e)
        {
            WStage = "Stage B";
            // MATCHING FLOW
            Rcs.ReadReconcCategoriesVsSourcesAll(WCategoryId);

            WSourceFileNameX = Rcs.SourceFileNameB;

            WSourceFileNameY = Rcs.SourceFileNameC;

            // Category Vs Matching Fields 
            string Filter = "CategoryId = '" + WCategoryId + "' "
                                                            + " AND SourceFileNameA = '" + WSourceFileNameX + "' "
                                                            + " AND SourceFileNameB = '" + WSourceFileNameY + "' ";

            ShowGridCategStageVsMatchingFields(Filter);

            ButtonPressForStage = true;

            buttonStageA.BackColor = Color.White;
            buttonStageB.BackColor = Color.Red;
            buttonStageC.BackColor = Color.White;
            buttonStageD.BackColor = Color.White;
            buttonAllStages.BackColor = Color.White;

            labelMatching.Show();
            labelSelectedMatch.Show();

            panelMatchFields.Show();
            panelSelectFields.Show();
            button1.Show();
            button2.Show();

        }

        // Stage C 
        private void buttonStageC_Click(object sender, EventArgs e)
        {

            WStage = "Stage C";
            // MATCHING FLOW
            Rcs.ReadReconcCategoriesVsSourcesAll(WCategoryId);

            WSourceFileNameX = Rcs.SourceFileNameC;

            WSourceFileNameY = Rcs.SourceFileNameD;

            // Category Vs Matching Fields 
            string Filter = "CategoryId = '" + WCategoryId + "' "
                                                            + " AND SourceFileNameA = '" + WSourceFileNameX + "' "
                                                            + " AND SourceFileNameB = '" + WSourceFileNameY + "' ";

            ShowGridCategStageVsMatchingFields(Filter);

            ButtonPressForStage = true;

            buttonStageA.BackColor = Color.White;
            buttonStageB.BackColor = Color.White;
            buttonStageC.BackColor = Color.Red;
            buttonStageD.BackColor = Color.White;
            buttonAllStages.BackColor = Color.White;

            labelMatching.Show();
            labelSelectedMatch.Show();

            panelMatchFields.Show();
            panelSelectFields.Show();
            button1.Show();
            button2.Show();


        }
        // Stage D
        private void buttonStageD_Click(object sender, EventArgs e)
        {
            WStage = "Stage D";
            // MATCHING FLOW
            Rcs.ReadReconcCategoriesVsSourcesAll(WCategoryId);

            WSourceFileNameX = Rcs.SourceFileNameD;

            WSourceFileNameY = Rcs.SourceFileNameE;

            // Category Vs Matching Fields 
            string Filter = "CategoryId = '" + WCategoryId + "' "
                                                            + " AND SourceFileNameA = '" + WSourceFileNameX + "' "
                                                            + " AND SourceFileNameB = '" + WSourceFileNameY + "' ";

            ShowGridCategStageVsMatchingFields(Filter);

            ButtonPressForStage = true;

            buttonStageA.BackColor = Color.White;
            buttonStageB.BackColor = Color.White;
            buttonStageC.BackColor = Color.White;
            buttonStageD.BackColor = Color.Red;
            buttonAllStages.BackColor = Color.White;

            labelMatching.Show();
            labelSelectedMatch.Show();

            panelMatchFields.Show();
            panelSelectFields.Show();
            button1.Show();
            button2.Show();

        }

        // SHOW ALL STAGES 
        string Filter;
        private void buttonAllStages_Click(object sender, EventArgs e)
        {
            buttonStageA.BackColor = Color.White;
            buttonStageB.BackColor = Color.White;
            buttonStageC.BackColor = Color.White;
            buttonStageD.BackColor = Color.White;
            buttonAllStages.BackColor = Color.Red;



            ButtonPressForStage = false;

            // Category Vs Matching Fields 
            Filter = "CategoryId = '" + WCategoryId + "' ";

            ShowGridCategStageVsMatchingFields(Filter);

        }
        // Go to Next 
        private void button4_Click(object sender, EventArgs e)
        {
            WComboboxText = comboBoxMatchingCateg.Text;

            Form502b NForm502b;

            NForm502b = new Form502b(WSignedId, WSignRecordNo, WOperator, WCategoryId);

            NForm502b.FormClosed += NForm502b_FormClosed;
            NForm502b.ShowDialog();
        }

        void NForm502b_FormClosed(object sender, FormClosedEventArgs e)
        {

            comboBoxMatchingCateg.Text = WComboboxText;
            Form502a_Load(this, new EventArgs());
        }

        //**********************************************************************
        // END NOTES 
        //********************************************************************** 

        //******************
        // SHOW GRID dataGridView1
        //******************
        private void ShowGridSourceFiles(string InFilter)
        {
            // Keep Scroll position 
            int scrollPosition = 0;
            if (WRowGrid1 > 0)
            {
                scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;
            }

            Rs.ReadReconcSourceFilesToFillDataTable(InFilter);
            //this.reconcSourceFilesTableAdapter.Fill(this.aTMSDataSet63.ReconcSourceFiles);

            dataGridView1.DataSource = Rs.SourceFilesDataTable.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                return;
            }
            dataGridView1.Columns[0].Width = 60; // Id
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[0].Visible = false;

            dataGridView1.Columns[1].Width = 60; // Id
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 170; // SourceFileName
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 110; // Origin of File 
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[4].Width = 50; // Type
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Sort(dataGridView1.Columns[2], ListSortDirection.Ascending);

            dataGridView1.Columns[5].Width = 100; // SourceDirectory
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Rows[WRowGrid1].Selected = true;
            dataGridView1_RowEnter_1(this, new DataGridViewCellEventArgs(1, WRowGrid1));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;

            // DATA TABLE ROWS DEFINITION 
            //SourceFilesDataTable.Columns.Add("SeqNo", typeof(int));
            //SourceFilesDataTable.Columns.Add("SourceFileName", typeof(string));
            //RowSelected["OriginSystem"] = SystemOfOrigin;
            //SourceFilesDataTable.Columns.Add("Type", typeof(string));
            //SourceFilesDataTable.Columns.Add("SourceDirectory", typeof(string));
        }

        //******************
        // SHOW GRID dataGridView2
        //******************
        private void ShowGridCategVsSourceFiles(string InFilter)
        {

            Rcs.ReadReconcCategoryVsSourcesANDFillTable(InFilter);

            dataGridView2.DataSource = Rcs.RMCategoryFilesDataFiles.DefaultView;

            if (dataGridView2.Rows.Count == 0)
            {
                return;
            }
            dataGridView2.Columns[0].Width = 70; // SeqNo
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView2.Columns[1].Width = 110; // RMCategId
            dataGridView2.Columns[1].Visible = false;
            //dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[2].Width = 500; // FileName
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[3].Visible = false; // ProcessMode
            dataGridView2.Columns[4].Visible = false; // LastInFileDtTm
            dataGridView2.Columns[5].Visible = false; // LastMatchingDtTm

            // DATA TABLE ROWS DEFINITION 
            //RMCategoryFilesDataFiles.Columns.Add("SeqNo", typeof(int));
            //RMCategoryFilesDataFiles.Columns.Add("RMCategId", typeof(string));
            //RMCategoryFilesDataFiles.Columns.Add("FileName", typeof(string));
            //RMCategoryFilesDataFiles.Columns.Add("ProcessMode", typeof(string));
            //RMCategoryFilesDataFiles.Columns.Add("LastInFileDtTm", typeof(DateTime));
            //RMCategoryFilesDataFiles.Columns.Add("LastMatchingDtTm", typeof(DateTime));
        }

        //******************
        // SHOW GRID dataGridView3
        //******************
        private void ShowGridMatchingFields(string InFilter)
        {
            // Keep Scroll position 
            int scrollPosition = 0;
            if (WRowGrid1 > 0)
            {
                scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;
            }

            string WTableStructureId = Rs.TableStructureId;
            // = "Atms And Cards"; 
            Utd.ReadUniversalTableMatchingFieldsToFillDataTable(WTableStructureId);

            dataGridView3.DataSource = Utd.MatchingFieldsDataTable.DefaultView;

            if (WOperator == "ITMX")
            {

                WTableStructureId = "Cashless";
                Utd.ReadUniversalTableMatchingFieldsToFillDataTable(WTableStructureId);

                dataGridView3.DataSource = Utd.MatchingFieldsDataTable.DefaultView;

            }

            //if (WSignedId == "ADMIN-NOSTRO")
            //{

            //    RRDMUniversalTableFieldsDefinition Utd = new RRDMUniversalTableFieldsDefinition();
            //    string WTableStructureId = "NOSTRO";
            //    Utd.ReadUniversalTableMatchingFieldsToFillDataTable(WOperator, WTableStructureId);

            //    dataGridView3.DataSource = Utd.MatchingFieldsDataTable.DefaultView;
            //}
            //if (WSignedId == "VISAUSER1")
            //{

            //    RRDMUniversalTableFieldsDefinition Utd = new RRDMUniversalTableFieldsDefinition();
            //    string WTableStructureId = "VISA";
            //    Utd.ReadUniversalTableMatchingFieldsToFillDataTable(WOperator, WTableStructureId);

            //    dataGridView3.DataSource = Utd.MatchingFieldsDataTable.DefaultView;
            //}     

            if (dataGridView3.Rows.Count == 0)
            {
                return;
            }
            dataGridView3.Columns[0].Width = 70; // SeqNo
            dataGridView3.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView3.Columns[1].Width = 190; // Field Name
            dataGridView3.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


            dataGridView3.Columns[2].Width = 90; // Field Type
            dataGridView3.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView3.Rows[WRowGrid3].Selected = true;
            dataGridView3_RowEnter_1(this, new DataGridViewCellEventArgs(1, WRowGrid3));

            dataGridView3.FirstDisplayedScrollingRowIndex = scrollPosition;

            // DATA TABLE ROWS DEFINITION 
            //MatchingFieldsDataTable.Columns.Add("SeqNo", typeof(int));
            //MatchingFieldsDataTable.Columns.Add("Field Name", typeof(string));
            //MatchingFieldsDataTable.Columns.Add("Field Type", typeof(string));
        }

        //******************
        // SHOW GRID dataGridView4
        //******************
        private void ShowGridCategStageVsMatchingFields(string InFilter)
        {
            Rsm.ReadReconcCategVsMatchingFieldsDataTable(InFilter, WTableStructureId);

            dataGridView4.DataSource = Rsm.ReconcCategStagesDataTable.DefaultView;

            if (dataGridView4.Rows.Count == 0)
            {
                return;
            }
            dataGridView4.Columns[0].Width = 70; // SeqNo
            dataGridView4.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView4.Columns[1].Width = 70; // Stage
            dataGridView4.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView4.Columns[2].Width = 100; // MatchingOperator
            dataGridView4.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView4.Columns[3].Width = 170; // MatchingField
            dataGridView4.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //dataGridView4.Rows[tempRowIndex1].Selected = true;
            //dataGridView4_RowEnter_1(this, new DataGridViewCellEventArgs(1, tempRowIndex1));

            // DATA TABLE ROWS DEFINITION 
            //ReconcCategStagesDataTable.Columns.Add("SeqNo", typeof(int));
            //ReconcCategStagesDataTable.Columns.Add("Stage", typeof(int));
            //ReconcCategStagesDataTable.Columns.Add("MatchingOperator", typeof(DateTime));
            //ReconcCategStagesDataTable.Columns.Add("MatchingField", typeof(string));
            //ReconcCategStagesDataTable.Columns.Add("CategoryId", typeof(string));
        }

        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Updating Done. Press Button Next For Additional.");
        }

    }
}
