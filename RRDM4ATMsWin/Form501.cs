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
    public partial class Form501 : Form
    {
        RRDMGasParameters Gp = new RRDMGasParameters();
        RRDMReconcCategoriesVsSourcesFiles Rcs = new RRDMReconcCategoriesVsSourcesFiles();
        RRDMReconcSourceFiles Rs = new RRDMReconcSourceFiles();
        RRDMReconcCategVsMatchingFields Rm = new RRDMReconcCategVsMatchingFields(); 

        string WSourceFileName;
        //string WReconcCategory;

        string WSourceFileNameX ;
        string WSourceFileNameY ;

        string WMatchingField;
        string WWMatchingField; 

        string WTechnicalSourceNameA ;         
        string WTechnicalSourceNameB ; 

        bool ButtonPress ; 

        string WCategory; 
        
        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        public Form501(string InSignedId, int SignRecordNo, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;

            InitializeComponent();

            pictureBox1.BackgroundImage = Properties.Resources.logo2;

            // Districts
            Gp.ParamId = "219";
            comboBox1.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBox1.DisplayMember = "DisplayValue";
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void Form501_Load(object sender, EventArgs e)
        {
            // LOAD Selected Category Files (TWO)
            WCategory = comboBox1.Text; 

            // MANAGE WHAT TO SHOW 

            // MATCHING FLOW
            Rcs.ReadReconcCategoriesVsSourcesAll(WCategory);
            if (Rcs.RecordFound == true)
            {
                label4.Show();
                panel6.Show();
                if (Rcs.TotalRecords == 1)
                {
                    textBoxFileA.Show();
                    buttonStageA.Hide();
                    textBoxFileB.Hide();
                    buttonStageB.Hide();
                    textBoxFileC.Hide();
                    buttonStageC.Hide();
                    textBoxFileD.Hide();
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
                }

                if (Rcs.TotalRecords == 3)
                {
                    textBoxFileA.Show();
                    buttonStageA.Show();
                    textBoxFileB.Show();
                    buttonStageB.Show();
                    textBoxFileC.Show();
                    buttonStageC.Hide();
                    textBoxFileD.Hide();
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
                }
            }
            else
            {
                label4.Hide();
                panel6.Hide(); 
            }

            // SELECT MATCHING FIELDS 
            Rm.ReadReconcCategVsMatchingFieldsAll(WCategory);
            if (Rm.RecordFound == true)
            {
                label5.Show();
                panel4.Show();
                button1.Show();
                button2.Show();
                label6.Show();
                panel5.Show();
            }
            else
            {
                label5.Hide();
                panel4.Hide();
                button1.Hide();
                button2.Hide();
                label6.Hide();
                panel5.Hide();
            }

            // Source Files (ONE)
            this.reconcSourceFilesTableAdapter.Fill(this.aTMSDataSet24.ReconcSourceFiles);
            
            reconcCategoryVsSourceFilesBindingSource.Filter = "ReconcCategory = '" + WCategory + "' "; 
            this.reconcCategoryVsSourceFilesTableAdapter.Fill(this.aTMSDataSet35.ReconcCategoryVsSourceFiles);

            // Matching Fields (THREE)

            this.reconcMatchingFieldsTableAdapter.Fill(this.aTMSDataSet46.ReconcMatchingFields);

            // Category Vs Matching Fields (FOUR)
            reconcCategoryVsSourceFilesBindingSource.Filter = "ReconcCategory = '" + WCategory + "' "; 
            this.reconcCategVsMatchingFieldsTableAdapter.Fill(this.aTMSDataSet51.ReconcCategVsMatchingFields);

        }


// Row Enter for Source Files 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            WSourceFileName = rowSelected.Cells[1].Value.ToString();

        }
// Row enter for Category vs Source files 
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];
       
            WSourceFileName = rowSelected.Cells[1].Value.ToString();

        }

// Row Enter For Matching fields 
        private void dataGridView3_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView3.Rows[e.RowIndex];
            WMatchingField = rowSelected.Cells[1].Value.ToString();

        }

// ROW ENTER for Cat matching 
        private void dataGridView4_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView4.Rows[e.RowIndex];
            WWMatchingField = rowSelected.Cells[1].Value.ToString();

        }

        // ADD /  Move to Category 
        private void buttonAdd_Click(object sender, EventArgs e)
        {
          
            Rcs.ReadReconcCategoriesVsSources(WCategory, WSourceFileName);
            if (Rcs.RecordFound == true)
            {
                MessageBox.Show("Record already exist in Cat vs Source");
                return;
            }

            Rcs.ReconcCategory = WCategory;
            Rcs.SourceFileName = WSourceFileName;

            Rcs.InsertReconcCategoryVsSourceRecord();

            Form501_Load(this, new EventArgs());

            textBoxMsgBoard.Text = "Source File Added to Reconciliation Category. "; 

        }

        // Delete 
        private void buttonRemove_Click(object sender, EventArgs e)
        {
            Rcs.DeleteReconcCategoryVsSourceRecord(WCategory, WSourceFileName);

            Form501_Load(this, new EventArgs());

            //reconcCategoryVsSourceFilesBindingSource.Filter = "ReconcCategory = '" + WCategory + "' ";
            //this.reconcCategoryVsSourceFilesTableAdapter.Fill(this.aTMSDataSet35.ReconcCategoryVsSourceFiles);

            textBoxMsgBoard.Text = "Source File Removed from Reconciliation Category. "; 

        }
// Change Of Category // Reload 
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Form501_Load(this, new EventArgs());
        }


// ADD to Category Matching field 
        private void button1_Click(object sender, EventArgs e)
        {
            if (ButtonPress == false)
            {
                MessageBox.Show("Press Button Of Pairs");
                return;
            }

            Rm.ReadReconcCategVsMatchingFields(WCategory, WSourceFileNameX, WSourceFileNameY, WMatchingField); 
         
            if (Rm.RecordFound == true)
            {
                MessageBox.Show("Record already exist in Cat vs Matching Field" );
                return;
            }

            Rm.ReconcCategory = WCategory;
            Rm.SourceFileNameA = WSourceFileNameX;
          
            Rm.SourceFileNameB = WSourceFileNameY;
           
            Rm.MatchingField = WMatchingField;
          

            Rm.InsertReconcCategVsMatchingFieldsRecord();

            // Category Vs Matching Fields 
            reconcCategVsMatchingFieldsBindingSource.Filter = "ReconcCategory = '" + WCategory + "' "
                                                            + " AND SourceFileNameA = '" + WSourceFileNameX + "' "
                                                            + " AND SourceFileNameB = '" + WSourceFileNameY + "' "; 
            this.reconcCategVsMatchingFieldsTableAdapter.Fill(this.aTMSDataSet51.ReconcCategVsMatchingFields);

        }
// DELETE From Category Matching 
        private void button2_Click(object sender, EventArgs e)
        {
            if (ButtonPress == false)
            {
                MessageBox.Show("Press Button Of Pairs");
                return;
            }

            Rm.DeleteReconcCategVsMatchingFieldsRecord(WCategory, WSourceFileNameX, WSourceFileNameY, WWMatchingField);

            // Category Vs Matching Fields 
            reconcCategVsMatchingFieldsBindingSource.Filter = "ReconcCategory = '" + WCategory + "' "
                                                            + " AND SourceFileNameA = '" + WSourceFileNameX + "' "
                                                            + " AND SourceFileNameB = '" + WSourceFileNameY + "' ";
            this.reconcCategVsMatchingFieldsTableAdapter.Fill(this.aTMSDataSet51.ReconcCategVsMatchingFields);

        }

        // SHow FileA and FileB Matching Fields 
      

        private void buttonStageA_Click(object sender, EventArgs e)
        {


            // MATCHING FLOW
            Rcs.ReadReconcCategoriesVsSourcesAll(WCategory);

            WSourceFileNameX = Rcs.SourceFileNameA;

            WSourceFileNameY = Rcs.SourceFileNameB;

            // Category Vs Matching Fields 
            reconcCategVsMatchingFieldsBindingSource.Filter = "ReconcCategory = '" + WCategory + "' "
                                                            + " AND SourceFileNameA = '" + WSourceFileNameX + "' "
                                                            + " AND SourceFileNameB = '" + WSourceFileNameY + "' ";
            this.reconcCategVsMatchingFieldsTableAdapter.Fill(this.aTMSDataSet51.ReconcCategVsMatchingFields);

            ButtonPress = true;

            buttonStageA.BackColor = Color.Red;

            label5.Show();
            panel4.Show();
            button1.Show();
            button2.Show();
            label6.Show();
            panel5.Show(); 
        }


       
    }
}
