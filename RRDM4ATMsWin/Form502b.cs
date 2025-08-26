using System;
using System.ComponentModel;
using System.Windows.Forms;
using RRDM4ATMs;

// Alecos

namespace RRDM4ATMsWin
{
    public partial class Form502b : Form
    {
        RRDMMatchingCategories Rc = new RRDMMatchingCategories();
        RRDMMatchingSourceFiles Rs = new RRDMMatchingSourceFiles();
        RRDMMatchingCategoriesVsSourcesFiles Rcs = new RRDMMatchingCategoriesVsSourcesFiles();
        RRDMMatchingCategStageVsMatchingFields Rsm = new RRDMMatchingCategStageVsMatchingFields();

        RRDMMatchingMasksVsMetaExceptions Rme = new RRDMMatchingMasksVsMetaExceptions();

        RRDMErrorsORExceptionsCharacteristics Ec = new RRDMErrorsORExceptionsCharacteristics(); 

        int WRowIndex;
        string WSourceFileName;
        //string WReconcCategory;
        int WSeqNo;
        int WSeqNo2;

        string WSourceFileNameX;
        string WSourceFileNameY;

        //string WMatchingField;
        string WWMatchingField;

        //string WStage;

        //bool ButtonPressForStage;

        //int tempRowIndex1;
        //int tempRowIndex2;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WCategoryId;

        public Form502b(string InSignedId, int SignRecordNo, string InOperator, string InCategoryId)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WCategoryId = InCategoryId;

            InitializeComponent();

            // Set Working Date 
            RRDMGasParameters Gp = new RRDMGasParameters();
            string ParId = "267";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            string TestingDate = Gp.OccuranceNm;
            if (TestingDate == "YES")
                labelToday.Text = new DateTime(2017, 03, 01).ToShortDateString();
            else labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;

            labelUserId.Text = InSignedId; 

            labelRMCategory.Text = "RM CATEGORY : " + WCategoryId; 

            // Matching Operation 

            comboBoxMatchOper.Items.Add("Equal");

            comboBoxMatchOper.Items.Add("Like");

            comboBoxMatchOper.Items.Add("Variance");   

            if (WOperator =="ITMX")
            {
                panel2.Hide();
                label6.Hide(); 
            }

        }

        private void Form502b_Load(object sender, EventArgs e)
        {

            // TODO: This line of code loads data into the 'aTMSDataSet66.ReconcCategoryVsSourceFiles' table. You can move, or remove it, as needed.
            // Selected Files (TWO)
            string Filter2 = "CategoryId = '" + WCategoryId + "' ";
            ShowGridCategVsSourceFiles(Filter2);

            // Category Vs Matching Fields (FOUR)
            string Filter4 = "CategoryId = '" + WCategoryId + "' ";
            ShowGridCategStageVsMatchingFields(Filter4);

            // LOAD MAsks 

            Rme.ReadMatchingMasksToFillDataTable(WOperator, WCategoryId);

            dataGridView1.DataSource = Rme.DataTableMasks.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                // Create records
                Ec.CreateMaskVsExceptionsRecords(WOperator, WCategoryId);

                // Read Again for updating 
                Rme.ReadMatchingMasksToFillDataTable(WOperator, WCategoryId);

                dataGridView1.DataSource = Rme.DataTableMasks.DefaultView;
            }
            else
            {

            }

            //// DATA TABLE ROWS DEFINITION 
            //DataTableMasks.Columns.Add("SeqNo", typeof(int));
            //DataTableMasks.Columns.Add("MaskId", typeof(string));
            //DataTableMasks.Columns.Add("MaskName", typeof(string));
            //DataTableMasks.Columns.Add("MetaExceptionId", typeof(string));

            dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);

            dataGridView1.Columns[0].Width = 60; // Error Id 
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 80; // Mask Id  
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 160; // Mask Name 
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 80; // Exception Id  
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }
        // ON ROW ENTER FOR FILES
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];
            WSeqNo2 = (int)rowSelected.Cells[0].Value;

            WSourceFileName = rowSelected.Cells[1].Value.ToString();

            labelFileId.Text = WSourceFileName;

            Rcs.ReadReconcCategoriesVsSourcebySeqNo(WSeqNo2);

            if (Rcs.PrimaryFile == true)
            {
                checkBoxPrimary.Checked = true;
            }
            else checkBoxPrimary.Checked = false;

            if (Rcs.IsTargetSystem == true)
            {
                checkBoxTargetSystem.Checked = true;
            }
            else checkBoxTargetSystem.Checked = false;

        }
        // ON ROW ENTER FOR FIELDS 
        private void dataGridView4_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView4.Rows[e.RowIndex];

            WSeqNo = (int)rowSelected.Cells[0].Value;

            Rsm.ReadReconcCategStageVsMatchingField(WSeqNo);

            WWMatchingField = Rsm.MatchingField;

            labelMatchingField.Text = " Field : " + WWMatchingField;

            comboBoxMatchOper.Text = Rsm.MatchingOperator;

            textBoxFrom.Text = Rsm.LowVarianceAmount.ToString("#,##0.00");

            textBoxTo.Text = Rsm.UpperVarianceAmount.ToString("#,##0.00");

            if (comboBoxMatchOper.Text == "Variance")
            {
                labelLow.Show();
                labelUpper.Show();
                textBoxFrom.Show();
                textBoxTo.Show();
            }
            else
            {
                labelLow.Hide();
                labelUpper.Hide();
                textBoxFrom.Hide();
                textBoxTo.Hide();

                // Nulify if values 
                Rsm.ReadReconcCategStageVsMatchingField(WSeqNo);

                Rsm.MatchingOperator = comboBoxMatchOper.Text;

                Rsm.LowVarianceAmount = 0;

                Rsm.UpperVarianceAmount = 0;

                Rsm.UpdateCategStageVsMatchingFieldsRecord(WSeqNo);

            }

        }
// Update File 
        private void buttonUpdateFile_Click(object sender, EventArgs e)
        {
            int WRowIndex2 = dataGridView2.SelectedRows[0].Index;
            Rcs.ReadReconcCategoriesVsSourcebySeqNo(WSeqNo2);

            if (checkBoxPrimary.Checked == true)
            {
                Rcs.PrimaryFile = true;
            }
            if (checkBoxPrimary.Checked == false)
            {
                Rcs.PrimaryFile = false;
            }

            if (checkBoxTargetSystem.Checked == true)
            {
                Rcs.IsTargetSystem = true;
            }
            if (checkBoxTargetSystem.Checked == false)
            {
                Rcs.IsTargetSystem = false;
            }

            Rcs.UpdateReconcCategoryVsSourceRecord(WOperator, WSeqNo2);

            //reconcCategoryStageVsMatchingFieldsBindingSource1.Filter = "CategoryId = '" + WCategoryId + "' ";

            Form502b_Load(this, new EventArgs());

            dataGridView2.Rows[WRowIndex2].Selected = true;
            dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex2));
        }
// Update Matching Field 
        private void buttonUpdateRow_Click(object sender, EventArgs e)
        {

            WRowIndex = dataGridView4.SelectedRows[0].Index;
            // Read 
            Rsm.ReadReconcCategStageVsMatchingField(WSeqNo);

            Rsm.MatchingOperator = comboBoxMatchOper.Text;

            if (comboBoxMatchOper.Text == "Equal" || comboBoxMatchOper.Text == "Like")
            {
                Rsm.LowVarianceAmount = 0;

                Rsm.UpperVarianceAmount = 0;

            }
            if (comboBoxMatchOper.Text == "Variance")
            {

                if (decimal.TryParse(textBoxFrom.Text, out Rsm.LowVarianceAmount))
                {
                }
                else
                {
                    MessageBox.Show(textBoxFrom.Text, "Please enter a valid number in field From!");
                    return;
                }
                if (decimal.TryParse(textBoxTo.Text, out Rsm.UpperVarianceAmount))
                {
                }
                else
                {
                    MessageBox.Show(textBoxFrom.Text, "Please enter a valid number in field To!");
                    return;
                }

            }

            Rsm.UpdateCategStageVsMatchingFieldsRecord(WSeqNo);

            WSourceFileNameX = Rsm.SourceFileNameA;

            WSourceFileNameY = Rsm.SourceFileNameB;

            //// Category Vs Matching Fields 
            //reconcCategoryStageVsMatchingFieldsBindingSource1.Filter = "CategoryId = '" + WCategoryId + "' "
            //                                                + " AND SourceFileNameA = '" + WSourceFileNameX + "' "
            //                                                + " AND SourceFileNameB = '" + WSourceFileNameY + "' ";

            Form502b_Load(this, new EventArgs());

            dataGridView4.Rows[WRowIndex].Selected = true;
            dataGridView4_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));
          
        }
// WHEN SELECTION CHANGES 
        private void comboBoxMatchOper_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxMatchOper.Text == "Variance")
            {
                labelLow.Show();
                labelUpper.Show();
                textBoxFrom.Show();
                textBoxTo.Show();
            }
            else
            {

                labelLow.Hide();
                labelUpper.Hide();
                textBoxFrom.Hide();
                textBoxTo.Hide();
            }
        }
 // ON ROW ENTER FOR MASKS 
        int WSeqNoMask;
        int WRowIndex4; 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            WSeqNoMask = (int)rowSelected.Cells[0].Value;
            Rme.ReadMatchingMaskBySeqNo(WSeqNoMask);

            textBox1.Text = Rme.MaskId;

            if (Rme.MaskId == "")
            {
                label10.Text = "Fill in Mask or delete record";
                label10.Show();
            }
            else
            {
                label10.Hide();
            }

         
            if (Rme.TransType == 11)
            {
                radioButtonForDebit.Checked = true;
            }

            if (Rme.TransType == 21)
            {
                radioButtonForCredit.Checked = true;
            }
          

            textBox2.Text = Rme.MaskName;

            textBox3.Text = Rme.MetaExceptionId.ToString(); 

        }
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Close();
        }
// ADD MAsk 
        private void buttonAdd_Click(object sender, EventArgs e)
        {

            WRowIndex4 = dataGridView1.SelectedRows[0].Index;

            Rme.MaskId = textBox1.Text ;

            if (dataGridView2.Rows.Count != textBox1.TextLength)
            {
                MessageBox.Show("Please enter correct length Mask");
                return; 
            }

            if (radioButtonForDebit.Checked == false & radioButtonForCredit.Checked == false)
            {
                MessageBox.Show("Please select Debit or Credit");
                return;
            }

            if (radioButtonForDebit.Checked ==true)
            {
                Rme.ReadMatchingMaskRecordbyMaskId(WOperator, WCategoryId, Rme.MaskId, 11);
            }
            if (radioButtonForCredit.Checked == true)
            {
                Rme.ReadMatchingMaskRecordbyMaskId(WOperator, WCategoryId, Rme.MaskId, 21);
            }


            if (Rme.RecordFound == true)
            {
                MessageBox.Show("This Mask already exist. Please correct your action. ");

                return;
            }

            // Meta exception no
             if (int.TryParse(textBox3.Text, out Rme.MetaExceptionId))
            {
                Ec.ReadErrorsIDRecord(Rme.MetaExceptionId, WOperator);
                 if (Ec.RecordFound == true)
                 {
                     if (textBox2.Text.Length >0)
                     {
                         MessageBox.Show("You Have insert Mask Name. " + Environment.NewLine
                             + "It will be overwritten by the metaexception description." + Environment.NewLine
                             + "You will be able to change it with the Update option ");
                     }
                     textBox2.Text = Ec.ErrDesc; 
                 }
                 else
                 {
                     MessageBox.Show("Not Such Meta Id");
                     return; 
                 }
            }
            else
            {
                MessageBox.Show(textBox3.Text, "Please enter a valid number!");

                return;
            }
             if (radioButtonForDebit.Checked == true)
            {
                Rme.TransType = 11;
            }
            if (radioButtonForCredit.Checked == true)
            {
                Rme.TransType = 21;
            }
         
            Rme.MaskName = textBox2.Text ;

            Rme.InsertMatchingCategoryMaskRecord();

            Form502b_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex4));

        }
// UPDATE MASK RECORD 
        private void buttonUpdate_Click(object sender, EventArgs e)
        {

            WRowIndex4 = dataGridView1.SelectedRows[0].Index;

            Rme.MaskId = textBox1.Text;

            if (dataGridView2.Rows.Count != textBox1.TextLength) // If number of files different that MASK
            {
                MessageBox.Show("Please enter correct Mask");
                return;
            }

            if (int.TryParse(textBox3.Text, out Rme.MetaExceptionId))
            {
            }
            else
            {
                MessageBox.Show(textBox3.Text, "Please enter a valid number!");

                return;
            }

            if (radioButtonForDebit.Checked == true)
            {
                Rme.TransType = 11;
            }
            if (radioButtonForCredit.Checked == true)
            {
                Rme.TransType = 21;
            }

            Rme.MaskName = textBox2.Text;

            if (radioButtonForDebit.Checked == true)
            {
                Rme.ReadMatchingMaskRecordbyMaskId(WOperator, WCategoryId, Rme.MaskId, 11);
            }
            if (radioButtonForCredit.Checked == true)
            {
                Rme.ReadMatchingMaskRecordbyMaskId(WOperator, WCategoryId, Rme.MaskId, 21);
            }

            if (Rme.RecordFound == true)
            {
                MessageBox.Show("This Mask already exist. Please correct your action. ");

                return;
            }

            Rme.UpdateMatchingMaskRecord(WSeqNoMask);

            Form502b_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex4));

        }
// DElete Mask Record 
        private void buttonDelete_Click(object sender, EventArgs e)
        {
          
            if (MessageBox.Show("Warning: Do you want to delete this row ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == DialogResult.Yes)
            {
                Rme.DeleteMaskRecord(WSeqNoMask);
            }
            else
            {
                return;
            }

            Form502b_Load(this, new EventArgs());

        }
// Show Options of Meta Ids
        private void buttonShowMetaIds_Click(object sender, EventArgs e)
        {
            Ec.ReadErrorsIDRecordsInTableDistict(WOperator);

            //dataGridView1.DataSource = Ec.ExceptionsCharacteristicsTable.DefaultView;
            Form78b NForm78b;

            string WHeader = "Meta Exceptions Ids " ;

            NForm78b = new Form78b(WSignedId, WSignRecordNo, WOperator, Ec.ExceptionsCharacteristicsTable, WHeader, "Form502b");
            //NForm78b.FormClosed += NForm78b_FormClosed;
            NForm78b.ShowDialog();

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

            dataGridView2.Columns[1].Width = 130; // RMCategId
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[2].Width = 160; // FileName
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[3].Width = 50; // ProcessMode
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Sort(dataGridView1.Columns[2], ListSortDirection.Ascending);

            dataGridView2.Columns[4].Width = 100; // LastInFileDtTm
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //dataGridView2.Rows[tempRowIndex1].Selected = true;
            //dataGridView2_RowEnter_1(this, new DataGridViewCellEventArgs(1, tempRowIndex1));


            // DATA TABLE ROWS DEFINITION 
            //RMCategoryFilesDataFiles.Columns.Add("SeqNo", typeof(int));
            //RMCategoryFilesDataFiles.Columns.Add("RMCategId", typeof(string));
            //RMCategoryFilesDataFiles.Columns.Add("FileName", typeof(string));
            //RMCategoryFilesDataFiles.Columns.Add("ProcessMode", typeof(string));
            //RMCategoryFilesDataFiles.Columns.Add("LastInFileDtTm", typeof(DateTime));
            //RMCategoryFilesDataFiles.Columns.Add("LastMatchingDtTm", typeof(DateTime));
        }



        //******************
        // SHOW GRID dataGridView4
        //******************
        string WTableStructureId = "Atms And Cards"; 
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

            dataGridView4.Columns[2].Width = 100; // MatchingField
            dataGridView4.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView4.Rows[tempRowIndex1].Selected = true;
            //dataGridView4_RowEnter_1(this, new DataGridViewCellEventArgs(1, tempRowIndex1));

            // DATA TABLE ROWS DEFINITION 
            //ReconcCategStagesDataTable.Columns.Add("SeqNo", typeof(int));
            //ReconcCategStagesDataTable.Columns.Add("Stage", typeof(int));
            //ReconcCategStagesDataTable.Columns.Add("MatchingOperator", typeof(DateTime));
            //ReconcCategStagesDataTable.Columns.Add("MatchingField", typeof(string));
            //ReconcCategStagesDataTable.Columns.Add("CategoryId", typeof(string));
        }

    }
}
