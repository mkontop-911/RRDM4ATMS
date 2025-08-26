using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

// Alecos

namespace RRDM4ATMsWin
{
    public partial class Form502_V03_REFORMAT : Form
    {

        RRDMMatchingSourceFiles Rs = new RRDMMatchingSourceFiles();
        RRDMMappingFileFieldsFromBankToRRDM Rff = new RRDMMappingFileFieldsFromBankToRRDM();

        RRDMUniversalTableFieldsDefinition Utd = new RRDMUniversalTableFieldsDefinition();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDM_BULK_IST_AndOthers_Records_ALL_2 Bg = new RRDM_BULK_IST_AndOthers_Records_ALL_2();

        ////int WRowIndex;
        
        //string WReconcCategory;
      //  int WSeqNoLeft;
        int WSeqNoRight;

       // bool InternalChange;
//int WRowIndex;
        //int WRowIndexMatching; 

        //int WMatchingSeqNo; 
        bool IsDelimiterFile;

       // int WRowIndexLeft;
        int WRowIndexRight;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WSourceFileID;
        string WFullPath;
        string WDelimiter; 

        public Form502_V03_REFORMAT(string InSignedId, int SignRecordNo, string InOperator
                        , string InSourceFileID, string InFullPath, string InDelimiter)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WSourceFileID = InSourceFileID;
            WFullPath = InFullPath;
            WDelimiter = InDelimiter; 

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;

            labelUserId.Text = InSignedId;
            //Field Type for File Fields 
            comboBoxFieldType.Items.Add("Character");
            comboBoxFieldType.Items.Add("Numeric");
            comboBoxFieldType.Items.Add("Decimal");
            comboBoxFieldType.Items.Add("Boolean");
            comboBoxFieldType.Items.Add("DateTime");
            comboBoxFieldType.Items.Add("Date");
            comboBoxFieldType.Items.Add("Time");
            comboBoxFieldType.Text = "Character";

            Gp.ParamId = "916"; // Source Field Value
            comboBoxPar916.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxPar916.DisplayMember = "DisplayValue";
         
        }
        // Load 
        private void Form502_Load(object sender, EventArgs e)
        {
            Rs.ReadReconcSourceFilesByFileId(WSourceFileID);

            if (WSourceFileID == "Atms_Journals_Txns")
            {
                labelFieldsDefinition.Hide();
                panelSelectFields.Hide();
            }
            else
            {
                labelFieldsDefinition.Show();
                panelSelectFields.Show();
            }

            labelFieldsDefinition.Text = "FIELDS DEFINITION FOR FILE : " + WSourceFileID;


            if (Rs.LayoutId == "DelimiterFile")
            {
                IsDelimiterFile = true;
                
                comboBoxSourceFieldNm.Show();
                bool WithHeaders = true; 
                comboBoxSourceFieldNm.DataSource = Bg.ReadTableToGetFieldNames_Array_List(WSourceFileID, WithHeaders,1);
                comboBoxSourceFieldNm.DisplayMember = "DisplayValue";

            }
            else
            {
                IsDelimiterFile = false;
               
                comboBoxSourceFieldNm.Hide();
            }       

            Rff.ReadFileFields(WSourceFileID);

            dataGridViewFields.DataSource = Rff.DataTableFileFields.DefaultView;

            // Create records

            Utd.ReadUniversalTableFieldsDefinitionToCreateBankToRRDMRecords(WSourceFileID, Rs.TableStructureId);

            if (Utd.RecordFound = false)
            {
                MessageBox.Show("No records found in Universal for this Structure or Operator.");
            }

            //Rmf.ReadMatchingFieldsToCreateBankToRRDMRecords(WOperator, WSourceFileName);

            // Read Again for updating 
            Rff.ReadFileFields(WSourceFileID);

            dataGridViewFields.DataSource = Rff.DataTableFileFields.DefaultView;

            string Result = Rff.ReadCheckForBrokenSequence(Rff.SourceFileId);
           
            dataGridViewFields.Columns[0].Width = 60; // PositionStart
            dataGridViewFields.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewFields.Columns[0].HeaderText = "Position Start";
            dataGridViewFields.Columns[0].Visible = false; 

            dataGridViewFields.Columns[1].Width = 60; // PositionEnd
            dataGridViewFields.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewFields.Columns[1].HeaderText = "Position End";
            dataGridViewFields.Columns[1].Visible = false;
            //dataGridView2.Sort(dataGridView2.Columns[0], ListSortDirection.Ascending);

            dataGridViewFields.Columns[2].Width = 150; // source field NM

            dataGridViewFields.Columns[3].Width = 70; // IsDONE 
            dataGridViewFields.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridViewFields.Columns[4].Width = 130; // TargetFieldNm
            dataGridViewFields.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewFields.Columns[5].Width = 500; // SourceFieldValue
            dataGridViewFields.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewFields.Columns[6].Width = 120; // TargetFieldValue
            dataGridViewFields.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewFields.Columns[7].Width = 60; // 
            dataGridViewFields.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewFields.Columns[7].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);


        }
               // Row Enter fields 
        private void dataGridViewFields_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridViewFields.Rows[e.RowIndex];

            WSeqNoRight = (int)rowSelected.Cells[7].Value;

            Rff.ReadReadFileFieldsbySeqNo(WSeqNoRight);

            if (IsDelimiterFile == true)
            {
                comboBoxSourceFieldNm.Text = Rff.SourceFieldNm;
            }
            else
            {
               // textBoxSourceFieldNm.Text = Rff.SourceFieldNm;
            }

            textBoxResult.Text = ""; 

            textBoxRTN.Text = Rff.SourceFieldValue; 

            textBoxTargetFieldName.Text = Rff.TargetFieldNm;
           
            comboBoxFieldType.Text = Rff.TargetFieldType;
            if (Rff.SourceFieldNm == "Not_Present_but_has_value")
            {
                textBoxInValue.Text = textBoxInputValue.Text = Rff.SourceFieldValue;
                textBoxRTN.Text = "Not_Present_but_has_value";
                textBoxInputValue.Show();
                dataGridViewFIELDValues.Hide();
            }
            else
            {
                comboBoxPar916.Text = Rff.SourceFieldValue;
                textBoxInValue.Hide();
                dataGridViewFIELDValues.Show();
            }
            
           

            if (comboBoxSourceFieldNm.Text == "Not_Present_but_has_value"
               || comboBoxSourceFieldNm.Text == "Not_Present_but_has_Formula"
               || comboBoxSourceFieldNm.Text == "Please Fill"
               )
            {
                // Dont Call
               // comboBoxValues.Hide();
                dataGridViewFIELDValues.Hide(); 
                if (comboBoxSourceFieldNm.Text == "Not_Present_but_has_value")
                {
                    textBoxInputValue.Show();
                    textBoxInValue.Show();
                    labelValue.Show();
                }
                else
                {
                    // Please Fill OR "Not_Present_but_has_Formula"
                    textBoxInputValue.Hide();
                    textBoxInValue.Hide();
                    labelValue.Hide();
                    if (comboBoxSourceFieldNm.Text == "Not_Present_but_has_Formula")
                    {
                        // do nothing 
                    }
                    else
                    {
                        textBoxRTN.Text = "";
                    }
                     
                }
            }
            else
            {
                dataGridViewFIELDValues.Show();
                string WTable = "BULK_" + WSourceFileID;
                Bg.ReadTableToGetFieldValues(WTable, Rff.SourceFieldNm);
                dataGridViewFIELDValues.DataSource = Bg.FieldsValueDataTable.DefaultView;
            }

            if (textBoxRTN.Text =="")
            {
                textBoxRTN.Text = comboBoxPar916.Text; 
            }
        }

        // Row ENTER FOR FIELDS VALUES 
        string WFieldValue;
        private void dataGridViewFIELDValues_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridViewFIELDValues.Rows[e.RowIndex];
            WFieldValue = (string)rowSelected.Cells[0].Value;
        }

        // Apply 
        private void buttonApply_Click(object sender, EventArgs e)
        {
            textBoxResult.Text = ""; 
            if (Rff.SourceFieldNm == "Not_Present_but_has_value" 
                ||( Rff.SourceFieldNm == "Please Fill" & comboBoxSourceFieldNm.Text =="Please Fill")
                || ((Rff.SourceFieldNm == "Please Fill" & comboBoxSourceFieldNm.Text != "Please Fill") 
                        & textBoxRTN.Text == "Rtn_NONE")
                || ( Rff.SourceFieldNm != "Please Fill"  & textBoxRTN.Text == "Rtn_NONE")

                || (Rff.SourceFieldNm == "Please Fill" & comboBoxSourceFieldNm.Text == "Not_Present_but_has_value")
                )
            {
                //MessageBox.Show("Not allowed operation" + Environment.NewLine
                //       + ""
                if (Rff.SourceFieldNm == "Please Fill" & comboBoxSourceFieldNm.Text == "Please Fill")
                {
                    MessageBox.Show("Please select Source Field");
                    return; 
                }

                if ((Rff.SourceFieldNm == "Please Fill" & comboBoxSourceFieldNm.Text != "Please Fill")
                                       & textBoxRTN.Text == "Rtn_NONE"
                                       )
                {

                    textBoxResult.Text = WFieldValue; 
                    return;
                }

                if ((Rff.SourceFieldNm != "Please Fill" & textBoxRTN.Text == "Rtn_NONE")
                                       )
                {

                    textBoxResult.Text = WFieldValue;
                    return;
                }

                if (comboBoxSourceFieldNm.Text == "Not_Present_but_has_value")
                {
                    textBoxResult.Text = textBoxInValue.Text; 
                }
                else
                {
                    textBoxResult.Text = textBoxRTN.Text;
                }
                 
            }
            else
            {

                Bg.ReadTableToGetReformatForSpecific(WSourceFileID, comboBoxSourceFieldNm.Text,
                                                    WFieldValue, textBoxRTN.Text);

                if (Bg.RecordFound)
                {
                    textBoxResult.Text = Bg.ReformatedField;
                }
                else
                {
                    MessageBox.Show("Reformat not successful" + Environment.NewLine
                        + "Error Message:..." + Bg.ErrorOutput
                        );
                }

            }
            
        }

        //// Update Right 
        bool NotPresent;
        string InputSourceField;
        private void buttonUpdateRight_Click(object sender, EventArgs e)
        {
           // WRowIndexLeft = dataGridViewFiles.SelectedRows[0].Index;
            WRowIndexRight = dataGridViewFields.SelectedRows[0].Index;

            if (IsDelimiterFile == true)
            {
                InputSourceField = comboBoxSourceFieldNm.Text;
            }
            else
            {
              //  InputSourceField = textBoxSourceFieldNm.Text;
            }

            if (InputSourceField == "Please Fill")
            {
                NotPresent = true;
                //textBoxFrom.Text = "0";
                //textBoxTo.Text = "0";
            }
            else
            {
                NotPresent = false;
            }

            if (InputSourceField == "Not_Present_but_has_value")
            {
                if (textBoxInputValue.Text == "")
                {
                    MessageBox.Show("Please fill in the value!");
                    return;
                }
            }
            

            Rff.ReadReadFileFieldsbySeqNo(WSeqNoRight);

            //if (Rff.IsUniversal == true & checkBoxIsUniversal.Checked == false)
            //{
            //    MessageBox.Show("You cannot change the Universal Atribute for this field");
            //    return;
            //}
          

            string OldSourceFieldNm = Rff.SourceFieldNm;
            string OldTargetFieldNm = Rff.TargetFieldNm;
            ////            Character
            ////Character
            ////Numeric
            ////Character
            ////Character
            ////Character
            ////Character
            ////Decimal
            ////Decimal
            ////Date
            ///
            decimal OutAmount;

            if (Rff.TargetFieldType == "Decimal")
            {

                if (decimal.TryParse(WFieldValue, out OutAmount))
                {
                    // Take the correct action 
                    MessageBox.Show("Value..." + WFieldValue + Environment.NewLine + " Becomes .." + OutAmount.ToString("#,##0.00"));
                }
                else
                {
                    MessageBox.Show("Value..." + WFieldValue, " The entered value cannot be converted to decimal");
                    return;
                }

            }
            int IntNumber;
            if (Rff.TargetFieldType == "Numeric")
            {

                if (int.TryParse(WFieldValue, out IntNumber))
                {
                    // Take the correct action 
                    MessageBox.Show("Value..." + WFieldValue + Environment.NewLine + " Becomes .." + IntNumber);
                }
                else
                {
                    MessageBox.Show("Value..." + WFieldValue, " The enter value cannot be converted to decimal");
                    return;
                }

            }

            if (OldSourceFieldNm != InputSourceField)
            {
                // New Source to be updated 
                Rff.ReadFileFieldsBySourceField(WSourceFileID, InputSourceField);

                if (Rff.RecordFound == true & NotPresent == false)
                {
                    //MessageBox.Show("Same Source Field Already exist");
                    //return;
                }
            }

            if (OldTargetFieldNm != textBoxTargetFieldName.Text)
            {
                // New target to be updated 
                Rff.ReadTableFieldsByTargetField(WSourceFileID, textBoxTargetFieldName.Text);

                if (Rff.RecordFound == true)
                {
                    if (MessageBox.Show("Warning: Same Target Field ALready Exist. Do you want to continue ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                         == DialogResult.Yes)
                    {
                        // Continue 
                    }
                    else
                    {
                        // NOT Continue 
                        return;
                    }
                }
            }

            // Check Valid Range 

            int FromPos = 0;
            int ToPos = 0;

            //

            if (IsDelimiterFile == false)
            {

                // CHECK if exist in other range
                // 
                string Result = Rff.ReadCheckWithinOtherRange(Rff.SourceFileId, OldSourceFieldNm, FromPos, ToPos);

                if (Result != "" & NotPresent == false)
                {
                    MessageBox.Show(Result);
                    return;
                }

            }

            // Read Again to position record 

            Rff.ReadReadFileFieldsbySeqNo(WSeqNoRight);

            Rff.SourceFileId = WSourceFileID;

            Rff.SourceFieldNm = InputSourceField;
            if (IsDelimiterFile == true)
            {
                Rff.SourceFieldPositionStart = 0;
                Rff.SourceFieldPositionEnd = 0;
            }
            else
            {
                Rff.SourceFieldPositionStart = FromPos;
                Rff.SourceFieldPositionEnd = ToPos;
            }

          
                Rff.IsUniversal = true;
          

            Rff.TargetFieldNm = textBoxTargetFieldName.Text;

            Rff.TargetFieldType = comboBoxFieldType.Text;

            if (InputSourceField == "Not_Present_but_has_value"
                || InputSourceField == "Not_Present_but_has_Formula"
                || InputSourceField == "Rtn_NONE")
            {
                
                if (InputSourceField == "Not_Present_but_has_Formula")
                {
                    if ( textBoxRTN.Text == "")
                    {
                        MessageBox.Show("Please enter formula");
                        return; 
                    }
                    Rff.SourceFieldValue = textBoxRTN.Text;
                }
                else
                {
                    Rff.SourceFieldValue = textBoxInputValue.Text;
                    Rff.TargetFieldValue = textBoxInputValue.Text;
                }
            }
            else
            {
               // WWMask.Substring(2, 1) == "0"
                if (textBoxRTN.Text != "" & textBoxRTN.Text != "Rtn_NONE")
                {
                    string WTable = WSourceFileID;
                    Bg.ReadTableToGetReformatForSpecific(WTable, Rff.SourceFieldNm,
                                                     WFieldValue, textBoxRTN.Text);

                    if (Bg.RecordFound)
                    {
                        textBoxResult.Text = Bg.ReformatedField;
                    }
                    else
                    {
                        MessageBox.Show("Reformat not successful" + Environment.NewLine
                            + "Error Message:..." + Bg.ErrorOutput
                                                           );
                        return;
                    }
                    Rff.SourceFieldValue = textBoxRTN.Text;
                }
                else
                {
                    Rff.SourceFieldValue = comboBoxPar916.Text;
                  //  Rff.TargetFieldValue = textBoxTargetFieldValue.Text;
                }
                
            }
           
            // UPDATE 
            Rff.UpdateFileFieldRecord(WSeqNoRight);

            Form502_Load(this, new EventArgs());

            dataGridViewFields.Rows[WRowIndexRight].Selected = true;
            dataGridViewFields_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexRight));

        }
        // Delete Right 
        private void buttonDeleteRight_Click(object sender, EventArgs e)
        {

            Rff.ReadReadFileFieldsbySeqNo(WSeqNoRight);

            if (Rff.IsUniversal == true)
            {
                MessageBox.Show("Not Allowed operation. You cannot delete a Universal Field");
                return;
            }

            if (MessageBox.Show("Warning: Do you want to delete this field ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == DialogResult.Yes)
            {
                Rff.DeleteFileFieldRecord(WSeqNoRight);

                Form502_Load(this, new EventArgs());
            }
            else
            {
                return;
            }   
        }

        // Is Universal 
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
           
                textBoxTargetFieldName.ReadOnly = true;
                comboBoxFieldType.Enabled = false;
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
        
        // FIND VALUES  
        private void comboBoxSourceFieldNm_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (comboBoxSourceFieldNm.Text == "Not_Present_but_has_value"
                || comboBoxSourceFieldNm.Text == "Not_Present_but_has_Formula"
                || comboBoxSourceFieldNm.Text == "Please Fill"
                )
            {
                //comboBoxValues.Hide();
                if (comboBoxSourceFieldNm.Text == "Not_Present_but_has_value")
                {
                    textBoxInputValue.Show();
                    labelValue.Show();
                    dataGridViewFIELDValues.Hide();
                    textBoxInputValue.Text = ""; 
                }
                else
                {
                    // Please Fill
                    textBoxInputValue.Hide();
                    labelValue.Hide();
                    textBoxRTN.Text = "";
                    dataGridViewFIELDValues.Hide();
                }
                
            }
            else
            {
                textBoxInputValue.Hide();
                labelValue.Hide();
                dataGridViewFIELDValues.Show();
                string WTable = "BULK_" + WSourceFileID;
                Bg.ReadTableToGetFieldValues(WTable, comboBoxSourceFieldNm.Text);

                dataGridViewFIELDValues.DataSource = Bg.FieldsValueDataTable.DefaultView;

                if (Bg.FieldsValueDataTable.Rows.Count>0)
                {
                    dataGridViewFIELDValues.Columns[0].Width = 150; // 
                    dataGridViewFIELDValues.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    //dataGridViewFIELDValues.Columns[0].HeaderText = "Position Start";

                }
                
                textBoxResult.Text = ""; 
            }

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
        
// If change set up 
        private void comboBoxPar916_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBoxRTN.Text = comboBoxPar916.Text; 
        }
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
//
        private void textBoxInputValue_TextChanged(object sender, EventArgs e)
        {
            textBoxInValue.Text = textBoxInputValue.Text; 
        }

        // test loading of Data to target file 
        int WReconcCycleNo = 999999;
        private void buttonTestLoading_Click(object sender, EventArgs e)
        {
            // If File exist delete all records loaded before
            // Load the source to target 
            // Show the first 1000
            // If error the system will show error. 
            // Move Just loaded to BULK ALL

            RRDMMappingFileFieldsFromBankToRRDM Mfr = new RRDMMappingFileFieldsFromBankToRRDM();
            RRDM_LoadFiles_InGeneral_Auto La = new RRDM_LoadFiles_InGeneral_Auto();
            RRDMMatchingSourceFiles Msf = new RRDMMatchingSourceFiles(); 
            string SQLCmd;
            WReconcCycleNo = 999999;
            string WFileSeqNo = "999999";
            string WOrigin = WSourceFileID;
            string WTerminalType = "20";

            // Read fields of existing table
            RRDM_BULK_IST_AndOthers_Records_ALL_2 Bio = new RRDM_BULK_IST_AndOthers_Records_ALL_2();

            // MAKE VALIDATION for the possible matching fields. 
            //    +" A.TerminalId = B.TerminalId "
            //+ " AND A.CardNumber = B.CardNumber "
            //+ " AND A.TransAmt = B.TransAmt "
            //+ " AND A.RRNumber = B.RRNumber "
            //+ " AND A.Net_TransDate = B.Net_TransDate "
            //+ TransType
            bool MissingMapping = false; 

            Msf.ReadReconcSourceFilesByFileId(WSourceFileID); 
            if (Msf.TableStructureId == "Atms And Cards")
            {
                string WTargetFieldName = "RRNumber";
                Rff.ReadTableFieldsByTargetField(WSourceFileID, WTargetFieldName);
                if (Rff.RecordFound == true & Rff.SourceFieldNm == "Please Fill")
                {
                    // RRN not defined 
                    MessageBox.Show("RRNumber mapping is not defined" + Environment.NewLine
                        + "You cannot proceed without it" 
                      );
                    MissingMapping = true; 
                }
                WTargetFieldName = "TransDate";
                Rff.ReadTableFieldsByTargetField(WSourceFileID, WTargetFieldName);
                if (Rff.RecordFound == true & Rff.SourceFieldNm == "Please Fill")
                {
                    // TransDate not defined 
                    MessageBox.Show("Date mapping is not defined" + Environment.NewLine
                        + "You cannot proceed without it"
                      );
                    MissingMapping = true;
                }
                WTargetFieldName = "TransAmt";
                Rff.ReadTableFieldsByTargetField(WSourceFileID, WTargetFieldName);
                if (Rff.RecordFound == true & Rff.SourceFieldNm == "Please Fill")
                {
                    // TransAmt not defined 
                    MessageBox.Show("TransAmt mapping is not defined" + Environment.NewLine
                        + "You cannot proceed without it"
                      );
                    MissingMapping = true;
                }
                WTargetFieldName = "TransType";
                Rff.ReadTableFieldsByTargetField(WSourceFileID, WTargetFieldName);
                if (Rff.RecordFound == true & Rff.SourceFieldNm == "Please Fill")
                {
                    // TransType not defined 
                    MessageBox.Show("TransType mapping is not defined" + Environment.NewLine
                        + "You cannot proceed without it"
                        + "Set 11 for debit and 21 for reversal"
                      );
                    MissingMapping = true;
                }

            }
            // 
            // If any of the mandatory fields are missing then return 
            //
            if (MissingMapping == true) return; 

            // 
            // CLEAN TABLES 
            Bio.CleanTables(WSourceFileID, WReconcCycleNo);

            // MOVE 
            La.MoveBULK_From_First_To_ALL(WSourceFileID, WDelimiter, WReconcCycleNo);

            // Load from BULK TO RRDM STD table COMMAND And Creation 
            string BulkTable = "BULK_" + WSourceFileID;
            Mfr.ReadTableFieldsBySourceFile_Reformat_AND_CREATE_COMMAND(BulkTable, WSourceFileID, WReconcCycleNo);
            //
            // Assign created command
            //
            SQLCmd = Mfr.FullCreatedSqlCommand_ITMX;

            // INSERT IN FILE 
            //
            string Message1 = "THE FULL SQL COMMAND IS :" ;
            string Message2 = SQLCmd;
            
            Form2MessageBox_SQL NShowMessage = new Form2MessageBox_SQL(Message1, Message2);
            NShowMessage.ShowDialog();

            La.Insert_Records_RRDM_STD_Table(WFileSeqNo, WOrigin, WTerminalType, WOperator, WReconcCycleNo, 
                           SQLCmd);

            // Create Reversals 
            //
            if (Msf.TableStructureId == "Atms And Cards")
            {
                La.Create_RRDM_STD_Table_Reversals(WSourceFileID, WReconcCycleNo);
            }
                

            MessageBox.Show("Data where loaded to SQL Table :.. " + WSourceFileID); 

        }
// View RRDM TABLE 
        private void buttonViewRRDMTable_Click(object sender, EventArgs e)
        {
            // string WMatchingCateg = comboBoxMatchingCateg.Text.Substring(0, 6);
            WReconcCycleNo = 999999;
            Form78d_FileRecords NForm78d_FileRecords;
            // textBoxFileId.Text
            int WMode = 10; //
                           // InMode = 1 : Not processed yet 
                           // InMode = 2 : Processed this Cycle
            bool WCategoryOnly = false;
          
            NForm78d_FileRecords = new Form78d_FileRecords(WOperator, WSignedId, WSourceFileID, "", WReconcCycleNo, "", WMode, WCategoryOnly);
            NForm78d_FileRecords.Show();
        }
    }
}
