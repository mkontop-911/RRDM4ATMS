using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RRDM4ATMs;

// Alecos
using System.Configuration;
using System.Diagnostics;

namespace RRDM4ATMsWin
{
    public partial class Form502a : Form
    {
        RRDMReconcCategories Rc = new RRDMReconcCategories();
        RRDMReconcSourceFiles Rs = new RRDMReconcSourceFiles();
        RRDMReconcCategoriesVsSourcesFiles Rcs = new RRDMReconcCategoriesVsSourcesFiles();   
        RRDMReconcCategStageVsMatchingFields Rsm = new RRDMReconcCategStageVsMatchingFields(); 

      //  int WRowIndex; 
        string WSourceFileName;
        //string WReconcCategory;
        int WSeqNo;
        int WSeqNo2;

        string WCategoryId; 

        string WSourceFileNameX;
        string WSourceFileNameY;

        string WMatchingField;
        string WWMatchingField;

        string WStage;

        bool ButtonPressForStage;

        string WComboboxText; 

        int tempRowIndex1;
        int tempRowIndex2;

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

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;
            labelUserId.Text = InSignedId; 

            // Categories
            comboBox1.DataSource = Rc.GetCategories(WOperator);
            comboBox1.DisplayMember = "DisplayValue";

            buttonAllStages.BackColor = Color.Red; 
           
        }


        private void Form502a_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'aTMSDataSet68.ReconcMatchingFields' table. You can move, or remove it, as needed.
            this.reconcMatchingFieldsTableAdapter.Fill(this.aTMSDataSet68.ReconcMatchingFields);
            // TODO: This line of code loads data into the 'aTMSDataSet63.ReconcSourceFiles' table. You can move, or remove it, as needed.
            this.reconcSourceFilesTableAdapter.Fill(this.aTMSDataSet63.ReconcSourceFiles);
           
            
            this.reconcCategoryStageVsMatchingFieldsTableAdapter.Fill(this.aTMSDataSet53.ReconcCategoryStageVsMatchingFields);
           
            this.reconcCategoryVsSourceFilesTableAdapter.Fill(this.aTMSDataSet35.ReconcCategoryVsSourceFiles);
          
            // MATCHING FLOW
            Rcs.ReadReconcCategoriesVsSourcesAll(WCategoryId);
            if (Rcs.RecordFound == true)
            {
                labelFlow.Show();
                labelFlow.Text = "MATCHING FLOW FOR " + WCategoryIdAndName;
                panelFlow.Show();
                labelFileA.Text = Rcs.SourceFileNameA;
                labelFileB.Text = Rcs.SourceFileNameB;
                labelFileC.Text = Rcs.SourceFileNameC;
                labelFileD.Text = Rcs.SourceFileNameD;

                if (Rcs.TotalRecords == 1)
                {
                    textBoxFileA.Show();
                    buttonStageA.Hide();
                    textBoxFileB.Hide();
                    buttonStageB.Hide();
                    textBoxFileC.Hide();
                    buttonStageC.Hide();
                    textBoxFileD.Hide();
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

            // Source Files (ONE)
            this.reconcSourceFilesTableAdapter.Fill(this.aTMSDataSet63.ReconcSourceFiles);

            dataGridView1.Rows[tempRowIndex1].Selected = true;
            dataGridView1_RowEnter_1(this, new DataGridViewCellEventArgs(1, tempRowIndex1));

            // Selected Files (TWO)

            reconcCategoryVsSourceFilesBindingSource.Filter = "CategoryId = '" + WCategoryId + "' ";
            this.reconcCategoryVsSourceFilesTableAdapter.Fill(this.aTMSDataSet35.ReconcCategoryVsSourceFiles);

            // Matching Fields (THREE)

            this.reconcMatchingFieldsTableAdapter.Fill(this.aTMSDataSet68.ReconcMatchingFields);

            dataGridView3.Rows[tempRowIndex2].Selected = true;
            dataGridView3_RowEnter_1(this, new DataGridViewCellEventArgs(1, tempRowIndex2));

            // Category Vs Matching Fields (FOUR)
            reconcCategoryStageVsMatchingFieldsBindingSource.Filter = "CategoryId = '" + WCategoryId + "' ";
            this.reconcCategoryStageVsMatchingFieldsTableAdapter.Fill(this.aTMSDataSet53.ReconcCategoryStageVsMatchingFields);

        }


        private void dataGridView1_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            WSourceFileName = rowSelected.Cells[1].Value.ToString();

        }
        // Row enter for Category vs Source files 
        private void dataGridView2_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];
            WSeqNo2 = (int)rowSelected.Cells[0].Value;
           
            WSourceFileName = rowSelected.Cells[1].Value.ToString();

            Rcs.ReadReconcCategoriesVsSourcebySeqNo(WSeqNo2);

        }     
       
        // Row Enter For Matching fields 
        private void dataGridView3_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView3.Rows[e.RowIndex];
            WMatchingField = rowSelected.Cells[1].Value.ToString();
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
            Rcs.ReadReconcCategoriesVsSources(WCategoryId, WSourceFileName);
            if (Rcs.RecordFound == true)
            {
                MessageBox.Show("Record already exist in Cat vs Source");
                return;
            }
            tempRowIndex1 = dataGridView1.SelectedRows[0].Index;

            Rcs.CategoryId = WCategoryId;
            Rcs.SourceFileName = WSourceFileName;

            Rcs.InsertReconcCategoryVsSourceRecord();

            Form502a_Load(this, new EventArgs()); // Load Grid        

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
                MessageBox.Show("Press Button Of Pairs");
                return;
            }

            tempRowIndex2 = dataGridView3.SelectedRows[0].Index;

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


            Rsm.InsertReconcCategVsMatchingFieldsRecord();

            Form502a_Load(this, new EventArgs());

            // Category Vs Matching Fields 
            reconcCategoryStageVsMatchingFieldsBindingSource.Filter = "CategoryId = '" + WCategoryId + "' "
                                                            + " AND SourceFileNameA = '" + WSourceFileNameX + "' "
                                                            + " AND SourceFileNameB = '" + WSourceFileNameY + "' ";
            this.reconcCategoryStageVsMatchingFieldsTableAdapter.Fill(this.aTMSDataSet53.ReconcCategoryStageVsMatchingFields);
        }

// Remove from categories / matching fields 
        private void button2_Click_1(object sender, EventArgs e)
        {
            if (ButtonPressForStage == false)
            {
                MessageBox.Show("Press Button Of Pairs");
                return;
            }

            Rsm.DeleteReconcCategVsMatchingFieldsRecord(WCategoryId, WSourceFileNameX, WSourceFileNameY, WWMatchingField);

            Form502a_Load(this, new EventArgs());

            // Category Vs Matching Fields 
            reconcCategoryStageVsMatchingFieldsBindingSource.Filter = "CategoryId = '" + WCategoryId + "' "
                                                            + " AND SourceFileNameA = '" + WSourceFileNameX + "' "
                                                            + " AND SourceFileNameB = '" + WSourceFileNameY + "' ";
            this.reconcCategoryStageVsMatchingFieldsTableAdapter.Fill(this.aTMSDataSet53.ReconcCategoryStageVsMatchingFields);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            WCategoryIdAndName = comboBox1.Text;

            WCategoryId = WCategoryIdAndName.Substring(0, 6);
            // Change Of Category // Reload 
            //Rc.ReadReconcCategorybyCategName(WOperator, WCategoryName);
            //WCategoryId = Rc.CategoryId;
        
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
            reconcCategoryStageVsMatchingFieldsBindingSource.Filter = "CategoryId = '" + WCategoryId + "' "
                                                            + " AND SourceFileNameA = '" + WSourceFileNameX + "' "
                                                            + " AND SourceFileNameB = '" + WSourceFileNameY + "' ";
            this.reconcCategoryStageVsMatchingFieldsTableAdapter.Fill(this.aTMSDataSet53.ReconcCategoryStageVsMatchingFields);

            ButtonPressForStage = true;

            buttonStageA.BackColor = Color.Red;
            buttonStageB.BackColor = Color.White;
            buttonStageC.BackColor = Color.White;
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
            reconcCategoryStageVsMatchingFieldsBindingSource.Filter = "CategoryId = '" + WCategoryId + "' "
                                                            + " AND SourceFileNameA = '" + WSourceFileNameX + "' "
                                                            + " AND SourceFileNameB = '" + WSourceFileNameY + "' ";
            this.reconcCategoryStageVsMatchingFieldsTableAdapter.Fill(this.aTMSDataSet53.ReconcCategoryStageVsMatchingFields);

            ButtonPressForStage = true;

            buttonStageA.BackColor = Color.White;
            buttonStageB.BackColor = Color.Red;
            buttonStageC.BackColor = Color.White;
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
            reconcCategoryStageVsMatchingFieldsBindingSource.Filter = "CategoryId = '" + WCategoryId + "' "
                                                            + " AND SourceFileNameA = '" + WSourceFileNameX + "' "
                                                            + " AND SourceFileNameB = '" + WSourceFileNameY + "' ";
            this.reconcCategoryStageVsMatchingFieldsTableAdapter.Fill(this.aTMSDataSet53.ReconcCategoryStageVsMatchingFields);

            ButtonPressForStage = true;

            buttonStageA.BackColor = Color.White;
            buttonStageB.BackColor = Color.White;
            buttonStageC.BackColor = Color.Red; 
            buttonAllStages.BackColor = Color.White;

            labelMatching.Show();
            labelSelectedMatch.Show();

            panelMatchFields.Show();
            panelSelectFields.Show();
            button1.Show();
            button2.Show();
          

        }

        // SHOW ALL STAGES 
        private void buttonAllStages_Click(object sender, EventArgs e)
        {
            buttonStageA.BackColor = Color.White;
            buttonStageB.BackColor = Color.White;
            buttonStageC.BackColor = Color.White;

            buttonAllStages.BackColor = Color.Red;

       

            ButtonPressForStage = false;

            // Category Vs Matching Fields 
            reconcCategoryStageVsMatchingFieldsBindingSource.Filter = "CategoryId = '" + WCategoryId + "' ";

            this.reconcCategoryStageVsMatchingFieldsTableAdapter.Fill(this.aTMSDataSet53.ReconcCategoryStageVsMatchingFields);

        }


        // FINISH
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
// Go to Next 
        private void button4_Click(object sender, EventArgs e)
        {
            WComboboxText  = comboBox1.Text;

            Form502b NForm502b; 

            NForm502b = new Form502b(WSignedId, WSignRecordNo, WOperator, WCategoryId);
           
            NForm502b.FormClosed += NForm502b_FormClosed;
            NForm502b.ShowDialog();
        }

        void NForm502b_FormClosed(object sender, FormClosedEventArgs e)
        {

            comboBox1.Text = WComboboxText; 
            Form502a_Load(this, new EventArgs());
        }

    }
}
