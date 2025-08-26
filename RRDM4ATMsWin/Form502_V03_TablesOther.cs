using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

// Alecos

namespace RRDM4ATMsWin
{
    public partial class Form502_V03_TablesOther : Form
    {

        RRDMMatchingSourceFiles Rs = new RRDMMatchingSourceFiles();
        RRDMMappingFileFieldsFromBankToRRDM Rff = new RRDMMappingFileFieldsFromBankToRRDM();

        RRDMUniversalTableFieldsDefinition Utd = new RRDMUniversalTableFieldsDefinition();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDM_BULK_IST_AndOthers_Records_ALL_2 Bg = new RRDM_BULK_IST_AndOthers_Records_ALL_2();

        RRDMMatchingBankToRRDMFileFields_EXTRA Me = new RRDMMatchingBankToRRDMFileFields_EXTRA(); 

        ////int WRowIndex;

        //string WReconcCategory;
        //int WSeqNoLeft;
        int WSeqNoRight;

        //bool InternalChange;
        //int WRowIndex;
        //int WRowIndexMatching; 

        //int WMatchingSeqNo; 
        bool IsDelimiterFile;
        string WSelectionCriteria;

        //int WRowIndexLeft;
        int WRowIndexRight;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WSourceFileID;

        public Form502_V03_TablesOther(string InSignedId, int SignRecordNo, string InOperator
                        , string InSourceFileID)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WSourceFileID = InSourceFileID;

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;

            if (WSourceFileID == "Atms_Journals_Txns")
            {
                WSourceFileID = "tblMatchingTxnsMasterPoolATMs"; 
            }

            labelUserId.Text = InSignedId;

            labelStep1.Text = "Extra definitions for updation for Table.." + WSourceFileID;
            //Field Type for File Fields 
         

        }
        // Load 
        private void Form502_Load(object sender, EventArgs e)
        {

            Bg.ReadTableToGetField_NAMES(WSourceFileID);
            

            if (Bg.Fields_NAMES_DataTable.Rows.Count > 0)
            {
                dataGridViewFields.DataSource = Bg.Fields_NAMES_DataTable.DefaultView;

                dataGridViewFields.Columns[0].Width = 150; // Field Name 
                dataGridViewFields.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;   
            }
            else
            {

            }

            Me.ReadExtraFieldsAndFillTable(WSourceFileID);

            if(Me.DataTableExtraFields.Rows.Count>0)
            {
                dataGridViewToBeUpdated.DataSource = Me.DataTableExtraFields.DefaultView;

                dataGridViewToBeUpdated.Columns[0].Width = 70; // SeqNo
                dataGridViewToBeUpdated.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridViewToBeUpdated.Columns[0].Visible = false; 

                dataGridViewToBeUpdated.Columns[1].Width = 140; // FieldNm
                dataGridViewToBeUpdated.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridViewToBeUpdated.Columns[2].Width = 500; // Formula
                dataGridViewToBeUpdated.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }
            else
            {

            }
            
        }

        // Row Enter 
        int WSeqNo;
        //string FieldName;
        string Formula; 
        private void dataGridViewToBeUpdated_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridViewToBeUpdated.Rows[e.RowIndex];
            WSeqNo = (int)rowSelected.Cells[0].Value;
            WFieldName = (string)rowSelected.Cells[1].Value;
            Formula = (string)rowSelected.Cells[2].Value;

            labelReformat.Text = "FORMULA FOR FIELD.." + WFieldName;

            WSelectionCriteria = " WHERE TableId ='" + WSourceFileID + "'"
                              + " AND FieldNm ='" + WFieldName + "'"
                               ;
            Me.ReadExtraFieldsBySelectionCriteria(WSignedId, WSelectionCriteria);

            if (Me.RecordFound == true)
            {
                textBoxRTN.Text = Me.FieldValueFormula;
            }
            else
            {
                textBoxRTN.Text = "No Formula";
            }
        }

        // Row Enter fields 
        string WFieldName;
        private void dataGridViewFields_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridViewFields.Rows[e.RowIndex];

            WFieldName = (string)rowSelected.Cells[0].Value;
           
            dataGridViewFIELDValues.Show();
            Bg.ReadTableToGetFieldValues(WSourceFileID, WFieldName);
            if (Bg.FieldsValueDataTable.Rows.Count > 0)
            {
                dataGridViewFIELDValues.DataSource = Bg.FieldsValueDataTable.DefaultView;
                //textBoxResult.Text = "";

                dataGridViewFIELDValues.Columns[0].Width = 300; // Field value
                dataGridViewFIELDValues.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }
            
            //labelReformat.Text = "FORMULA FOR FIELD.." + WFieldName; 

            //WSelectionCriteria = " WHERE TableId ='" + WSourceFileID + "'"
            //                  + " AND FieldNm ='" + WFieldName + "'"
            //                   ;
            //Me.ReadExtraFieldsBySelectionCriteria(WSignedId, WSelectionCriteria);

            //if (Me.RecordFound == true)
            //{
            //    textBoxRTN.Text = Me.FieldValueFormula;
            //}
            //else
            //{
            //    textBoxRTN.Text = "No Formula";
            //}
            
        }

     
        // Row ENTER FOR FIELDS VALUES 
        string WFieldValue;
        private void dataGridViewFIELDValues_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridViewFIELDValues.Rows[e.RowIndex];
            // WFieldValue = (string)rowSelected.Cells[0].Value;
        }

        // Apply 
        private void buttonApply_Click(object sender, EventArgs e)
        {
            bool Is_Date = false;
            if (checkBoxIsDate.Checked == true) Is_Date = true; 
            comboBoxResult.DataSource = Bg.ReadTableToGetFormulaValues_Array_List(WSourceFileID, textBoxRTN.Text, 2, Is_Date);
            comboBoxResult.DisplayMember = "DisplayResult";
            
            if (Bg.RecordFound)
            {
                //
            }
            else
            {
                MessageBox.Show("Reformat not successful" + Environment.NewLine
                    + "Error Message:..." + Bg.ErrorOutput
                    );
            }

        }

      
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        // Print 
        private void buttonPrint_Click(object sender, EventArgs e)
        {
            string P1 = "InFile Fields Mapping To RRDM Tables";

            string P2 = Rs.InportTableName;
            string P3 = "Par3";
            string P4 = WOperator;
            string P5 = WSignedId;

            Form56R_In_Mapping ReportATMS_In_Mapping = new Form56R_In_Mapping(P1, P2, P3, P4, P5);
            ReportATMS_In_Mapping.Show();
        }
        
        // Information of how the file should be
        private void linkLabelINFO_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("The input file can be delimitered type or with fixed field lenght fields" + Environment.NewLine
                + "Map the fields towards the RRDM standard record fields" + Environment.NewLine
                  + "Use routines for transformation" + Environment.NewLine
                    + "Select the right routine for date and time" + Environment.NewLine
                      + "By default system accepts date time as YYYY-MM-DD 00:00:00" + Environment.NewLine
                        + "Input value for response code = 0 success " + Environment.NewLine
                          + "Input 11 for debit and 21 for credit " + Environment.NewLine
                           + "If for the same details we have one record with 11 and one with 21 then we consider that reversal is made.  " + Environment.NewLine
                );

            return;
        }
        //
        // View Input File 
        private void buttonViewFile_Click(object sender, EventArgs e)
        {
            // WSourceFileID = Rs.SourceFileId;

            if (WSourceFileID == "Atms_Journals_Txns")
            {
                MessageBox.Show("Nothing to show for this file");
                return;
            }

            Form78d_FileRecords NForm78d_FileRecords;
            // textBoxFileId.Text

            string WMatchingCateg = "";
            int WReconcCycleNo = 0;

            int WMode = 9; // BULK 

            bool WCategoryOnly = false;
            // Show file 

            NForm78d_FileRecords = new Form78d_FileRecords(WOperator, WSignedId, WSourceFileID, "", WReconcCycleNo, WMatchingCateg, WMode, WCategoryOnly);
            NForm78d_FileRecords.ShowDialog();
        }
       
// Add Formula
        private void buttonAddFormula_Click(object sender, EventArgs e)
        {
           
            Me.TableId = WSourceFileID;
            Me.FieldNm = WFieldName;
            WSelectionCriteria = " WHERE TableId ='" + WSourceFileID + "'"
                                  + " AND FieldNm = '" + WFieldName + "'"; 
            Me.ReadExtraFieldsBySelectionCriteria(WSignedId, WSelectionCriteria);
            if (Me.RecordFound== true)
            {
                MessageBox.Show("Record already exist");
                return; 
            }
              

            if (textBoxRTN.Text != "No Formula" & textBoxRTN.Text.Length>10)
            {
                Me.FieldValueFormula = textBoxRTN.Text;
            }
            else
            {
                MessageBox.Show("Please enter valid Formula"); 
            }
            
            Me.FieldDefaultValue = "";
            Me.MatchingProduct = ""; 

            Me.InsertRecordInTable();

            Form502_Load(this, new EventArgs());

            //dataGridViewFields.Rows[WRowIndexRight].Selected = true;
            //dataGridViewFields_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexRight));
        }
// Update Formula
         
        private void buttonUpdateFormula_Click(object sender, EventArgs e)
        {
            WSelectionCriteria = " WHERE TableId ='" + WSourceFileID + "'"
                             + " AND FieldNm ='" + WFieldName + "'"
                              ;
            Me.ReadExtraFieldsBySelectionCriteria(WSignedId, WSelectionCriteria);

            Me.TableId = WSourceFileID;
            Me.FieldNm = WFieldName;

            if (textBoxRTN.Text != "No Formula" & textBoxRTN.Text.Length > 10)
            {
                Me.FieldValueFormula = textBoxRTN.Text;
            }
            else
            {
                MessageBox.Show("Please enter valid Formula");
            }

            Me.FieldDefaultValue = "";
            Me.MatchingProduct = "";

            Me.UpdateRecordInTable(Me.SeqNo);

            WRowIndexRight = dataGridViewFields.SelectedRows[0].Index;

            Form502_Load(this, new EventArgs());

            dataGridViewFields.Rows[WRowIndexRight].Selected = true;
            dataGridViewFields_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexRight));

        }
        // DELETE 
        private void buttonDeleteFormula_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Warning: Do you want to delete this field ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                      == DialogResult.Yes)
            {
                WSelectionCriteria = " WHERE TableId ='" + WSourceFileID + "'"
                            + " AND FieldNm ='" + WFieldName + "'"
                             ;
                Me.ReadExtraFieldsBySelectionCriteria(WSignedId, WSelectionCriteria);

                Me.DeleteRecordInTable(Me.SeqNo);

                Form502_Load(this, new EventArgs());
            }
            else
            {
                return;
            }
           
        }
// View Command 
        private void buttonViewCommand_Click(object sender, EventArgs e)
        {
            string SQLCmd;
          
            Me.ReadTable_EXTRA_AND_CREATE_COMMAND(WSourceFileID);
            //
            // Assign created command
            //
            SQLCmd = Me.FullCreatedSqlCommand;

            // SHOW COMMAND
            //
            string Message1 = "THE FULL SQL COMMAND IS :";
            string Message2 = SQLCmd;

            Form2MessageBox_SQL NShowMessage = new Form2MessageBox_SQL(Message1, Message2);
            NShowMessage.ShowDialog();
        }

    }
}
