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
    public partial class Form502b : Form
    {
        RRDMReconcCategories Rc = new RRDMReconcCategories();
        RRDMReconcSourceFiles Rs = new RRDMReconcSourceFiles();
        RRDMReconcCategoriesVsSourcesFiles Rcs = new RRDMReconcCategoriesVsSourcesFiles();
        RRDMReconcCategStageVsMatchingFields Rsm = new RRDMReconcCategStageVsMatchingFields();

        RRDMReconcMasksVsMetaExceptions Rme = new RRDMReconcMasksVsMetaExceptions();

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

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;

            labelUserId.Text = InSignedId; 

            labelRMCategory.Text = "RM CATEGORY : " + WCategoryId; 

            // Matching Operation 

            comboBoxMatchOper.Items.Add("Equal");

            comboBoxMatchOper.Items.Add("Like");

            comboBoxMatchOper.Items.Add("Variance");

            reconcCategoryVsSourceFilesBindingSource1.Filter = "CategoryId = '" + WCategoryId + "' ";

            reconcCategoryStageVsMatchingFieldsBindingSource1.Filter = "CategoryId = '" + WCategoryId + "' ";

        }

        private void Form502b_Load(object sender, EventArgs e)
        {
            
            // TODO: This line of code loads data into the 'aTMSDataSet66.ReconcCategoryVsSourceFiles' table. You can move, or remove it, as needed.
            
            // Selected Files 
           
            this.reconcCategoryVsSourceFilesTableAdapter1.Fill(this.aTMSDataSet66.ReconcCategoryVsSourceFiles);
        

            // Category Vs Matching Fields 

            
            this.reconcCategoryStageVsMatchingFieldsTableAdapter1.Fill(this.aTMSDataSet67.ReconcCategoryStageVsMatchingFields);


            // LOAD MAsks 

            Rme.ReadReconcMasksToFillDataTable(WOperator, WCategoryId);

            dataGridView1.DataSource = Rme.DataTableMasks.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                // Create records
                Ec.CreateMaskVsExceptionsRecords(WOperator, WCategoryId);

                // Read Again for updating 
                Rme.ReadReconcMasksToFillDataTable(WOperator, WCategoryId);

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
            dataGridView1.Sort(dataGridView1.Columns[1], ListSortDirection.Ascending);

            dataGridView1.Columns[2].Width = 150; // Mask Name 
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

            if (Rcs.WithRemains == true)
            {
                checkBoxRemains.Checked = true;
            }
            else checkBoxRemains.Checked = false;

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

            if (checkBoxRemains.Checked == true)
            {
                Rcs.WithRemains = true;
            }
            if (checkBoxRemains.Checked == false)
            {
                Rcs.WithRemains = false;
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
            Rme.ReadReconcMaskBySeqNo(WSeqNoMask);

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

            textBox2.Text = Rme.MaskName;

            textBox3.Text = Rme.MetaExceptionNo.ToString(); 

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

            Rme.ReadReconcMaskRecord(WOperator, WCategoryId, Rme.MaskId);

            if (Rme.RecordFound == true)
            {
                MessageBox.Show("This Mask already exist. Please correct your action. ");

                return;
            }

            // Meta exception no
             if (int.TryParse(textBox3.Text, out Rme.MetaExceptionNo))
            {
                Ec.ReadErrorsIDRecord(Rme.MetaExceptionNo, WOperator);
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

            Rme.MaskName = textBox2.Text ;

            Rme.InsertReconcCategoryMaskRecord();

            Form502b_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex4));

        }
// UPDATE MASK RECORD 
        private void buttonUpdate_Click(object sender, EventArgs e)
        {

            WRowIndex4 = dataGridView1.SelectedRows[0].Index;

            Rme.MaskId = textBox1.Text;

            if (dataGridView2.Rows.Count != textBox1.TextLength)
            {
                MessageBox.Show("Please enter correct Mask");
                return;
            }

            if (int.TryParse(textBox3.Text, out Rme.MetaExceptionNo))
            {
            }
            else
            {
                MessageBox.Show(textBox3.Text, "Please enter a valid number!");

                return;
            }

            Rme.MaskName = textBox2.Text;

            Rme.ReadReconcMaskRecord(WOperator, WCategoryId, Rme.MaskId);

            if (Rme.RecordFound == true)
            {
                MessageBox.Show("This Mask already exist. Please correct your action. ");

                return;
            }

            Rme.UpdateReconcMaskRecord(WSeqNoMask);

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

    }
}
