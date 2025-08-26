using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Security.Cryptography;
using System.Globalization;

using RRDM4ATMs;
using System.IO;

// Alecos

namespace RRDM4ATMsWin
{
    public partial class Form502_Actions_GL : Form
    {

        RRDMMatchingSourceFiles Rs = new RRDMMatchingSourceFiles();
        RRDMMappingFileFieldsFromBankToRRDM Rff = new RRDMMappingFileFieldsFromBankToRRDM();

        RRDMUniversalTableFieldsDefinition Utd = new RRDMUniversalTableFieldsDefinition();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDM_BULK_IST_AndOthers_Records_ALL_2 Bg = new RRDM_BULK_IST_AndOthers_Records_ALL_2();

        RRDMActions_GL Ag = new RRDMActions_GL(); 

        ////int WRowIndex;
        string WSourceFileID;
        //string WReconcCategory;
        int WSeqNo;
       
        bool InternalChange;
        int WRowIndex;
        
        int WRowIndexLeft;
       
        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        int WMode; 

        public Form502_Actions_GL(string InSignedId, int SignRecordNo, string InOperator, int InMode)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WMode = InMode; // 1 equal to normal process
                            // 

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;

            labelUserId.Text = InSignedId;
            // First
            comboBoxDR_CR_1.Items.Add("DR");
            comboBoxDR_CR_1.Items.Add("CR");

            comboBoxDR_CR_1.Text = "DR";
            // Second 
            comboBoxDR_CR_2.Items.Add("DR");
            comboBoxDR_CR_2.Items.Add("CR");

            comboBoxDR_CR_2.Text = "DR";
            // First
            //comboBoxBranchId_1.Items.Add("Please Select");
            //comboBoxBranchId_1.Items.Add("ATM");
            //comboBoxBranchId_1.Items.Add("User");
            //comboBoxBranchId_1.Items.Add("Category_Branch");
            //comboBoxBranchId_1.Items.Add("CIT_Branch");

            //comboBoxBranchId_1.Text = "Please Select";
            //// Second 
            //comboBoxBranchId_2.Items.Add("Please Select");
            //comboBoxBranchId_2.Items.Add("ATM");
            //comboBoxBranchId_2.Items.Add("User");
            //comboBoxBranchId_2.Items.Add("Category_Branch");
            //comboBoxBranchId_2.Items.Add("CIT_Branch");

            //comboBoxBranchId_2.Text = "Please Select";

            // ViewInApplication
            Gp.ParamId = "942";
            comboBoxViewInApplication.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxViewInApplication.DisplayMember = "DisplayValue";

            // Action Names
            Gp.ParamId = "941";
            comboBoxActionName.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxActionName.DisplayMember = "DisplayValue";


            // Accounts Names first
            Gp.ParamId = "701";
            comboBoxAccName_1.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxAccName_1.DisplayMember = "DisplayValue";

            // Accounts Names Second 
            Gp.ParamId = "701";
            comboBoxAccName_2.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxAccName_2.DisplayMember = "DisplayValue";

            // Descriptions first
            Gp.ParamId = "943";
            comboBoxStatDesc_1.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxStatDesc_1.DisplayMember = "DisplayValue";
            // Descriptions Second
            Gp.ParamId = "943";
            comboBoxStatDesc_2.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxStatDesc_2.DisplayMember = "DisplayValue";

        }
        // Load 
        private void Form502_Load(object sender, EventArgs e)
        {
            // Source Files (Grid-ONE)
            string SelectionCriteria = " WHERE Operator = '" + WOperator + "' ";

            Ag.ReadActionsAndFillTable(SelectionCriteria); 
          
            dataGridViewActions.DataSource = Ag.ActionsDataTable.DefaultView;

            dataGridViewActions.Columns[0].Width = 60; // SeqNo
            dataGridViewActions.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewActions.Columns[0].Visible = false;

            dataGridViewActions.Columns[1].Width = 70; // 
            dataGridViewActions.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridViewActions.Columns[2].Width = 50; // 
            dataGridViewActions.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewActions.Columns[3].Width = 300; //
            dataGridViewActions.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewActions.Columns[4].Width = 50; // 
            dataGridViewActions.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewActions.Columns[4].Visible = false;

        }
        // Row Enter 
        //
        private void dataGridViewActions_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridViewActions.Rows[e.RowIndex];

            WSeqNo = (int)rowSelected.Cells[0].Value;

            Ag.ReadActionBySeqNo(WSeqNo);

            // 
            textBoxActionId.Text = Ag.ActionId ;
            textBoxOcc.Text= Ag.Occurance.ToString();

            comboBoxActionName.Text = Ag.ActionNm ;
            checkBoxIsGL.Checked = Ag.Is_GL_Action ;

            if (Ag.Is_GL_Action == true)
            {
                panel2.Show(); 
            }
            else
            {
                panel2.Hide();
            }

            comboBoxDR_CR_1.Text= Ag.GL_Sign_1;
            textBoxAccShort_1.Text = Ag.ShortAccID_1  ;

            comboBoxAccName_1.Text= Ag.AccName_1 ;

           // comboBoxBranchId_1.Text = Ag.WhatBranch_1 ;

            comboBoxStatDesc_1.Text= Ag.StatementDesc_1 ;

            comboBoxDR_CR_2.Text = Ag.GL_Sign_2;
            textBoxAccShort_2.Text = Ag.ShortAccID_2;
            comboBoxAccName_2.Text = Ag.AccName_2;

           // comboBoxBranchId_2.Text = Ag.WhatBranch_2;

            comboBoxStatDesc_2.Text = Ag.StatementDesc_2;

            comboBoxViewInApplication.Text = Ag.ViewVersion ;

            comboBoxActionName.Enabled = false;
            textBoxOcc.ReadOnly = true;
            checkBoxIsGL.Enabled = false;

            buttonAddLeft.Hide();
            buttonUpdateLeft.Show();
            buttonDeleteLeft.Show();

        }

        // ADD 
        private void buttonAddLeft_Click(object sender, EventArgs e)
        {
          //  Ag.ReadActionByActionId(WOperator, textBoxActionId.Text);
            Ag.ReadActionByActionId_And_Occ(WOperator, textBoxActionId.Text, Int32.Parse(textBoxOcc.Text));
            if (Ag.RecordFound == true)
            {
                MessageBox.Show("Action Already exist");
                return;
            }

            Ag.ActionId = textBoxActionId.Text;
            Ag.Occurance = Int32.Parse(textBoxOcc.Text);

            Ag.ActionNm = comboBoxActionName.Text; 
            Ag.Is_GL_Action = checkBoxIsGL.Checked;

            Ag.ViewVersion = comboBoxViewInApplication.Text;

            Ag.Operator = WOperator;

            if (Ag.Is_GL_Action == true)
            {
                Ag.GL_Sign_1 = comboBoxDR_CR_1.Text;
                Ag.ShortAccID_1 = textBoxAccShort_1.Text;
                Ag.AccName_1 = comboBoxAccName_1.Text;

                Ag.WhatBranch_1 = "N/A";

                Ag.StatementDesc_1 = comboBoxStatDesc_1.Text;

                Ag.GL_Sign_2 = comboBoxDR_CR_2.Text;
                Ag.ShortAccID_2 = textBoxAccShort_2.Text;
                Ag.AccName_2 = comboBoxAccName_2.Text;

                Ag.WhatBranch_2 = "N/A";

                Ag.StatementDesc_2 = comboBoxStatDesc_2.Text;

            }
            else
            {
                Ag.GL_Sign_1 = "N/A";
                Ag.ShortAccID_1 = "N/A";
                Ag.AccName_1 = "N/A";

                Ag.WhatBranch_1 = "N/A";

                Ag.StatementDesc_1 = "N/A";

                Ag.GL_Sign_2 = "N/A";
                Ag.ShortAccID_2 = "N/A";
                Ag.AccName_2 = "N/A";

                Ag.WhatBranch_2 = "N/A";

                Ag.StatementDesc_2 = "N/A";
            }

            Ag.InsertAction_GL();

            MessageBox.Show("Action with Nm.:.." + comboBoxActionName.Text + " Inserted");
            textBoxMsgBoard.Text = "Action Inserted";

            Form502_Load(this, new EventArgs());

        }
        // Update 
        private void buttonUpdateLeft_Click(object sender, EventArgs e)
        {
            WRowIndexLeft = dataGridViewActions.SelectedRows[0].Index;

            Ag.ActionId = textBoxActionId.Text;
            Ag.Occurance = Int32.Parse(textBoxOcc.Text);

            Ag.ActionNm = comboBoxActionName.Text;
            Ag.Is_GL_Action = checkBoxIsGL.Checked;

            Ag.GL_Sign_1 = comboBoxDR_CR_1.Text;
            Ag.ShortAccID_1 = textBoxAccShort_1.Text;
            Ag.AccName_1 = comboBoxAccName_1.Text;

            Ag.WhatBranch_1 = "N/A";

            Ag.StatementDesc_1 = comboBoxStatDesc_1.Text;

            Ag.GL_Sign_2 = comboBoxDR_CR_2.Text;
            Ag.ShortAccID_2 = textBoxAccShort_2.Text;
            Ag.AccName_2 = comboBoxAccName_2.Text;

            Ag.WhatBranch_2 = "N/A";

            Ag.StatementDesc_2 = comboBoxStatDesc_2.Text;

            Ag.ViewVersion = comboBoxViewInApplication.Text;

            Ag.Operator = WOperator;

            Ag.UpdateAction_GL(WSeqNo); 

            Form502_Load(this, new EventArgs());

            dataGridViewActions.Rows[WRowIndexLeft].Selected = true;
            dataGridViewActions_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

            MessageBox.Show("Action Updated");
            textBoxMsgBoard.Text = "Action Updated";
        }
        // Delete Action
        private void buttonDeleteLeft_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Warning: Do you want to delete this Action ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                         == DialogResult.Yes)
            {
                Ag.DeleteActionRecord(WSeqNo); 

                Form502_Load(this, new EventArgs());
            }
            else
            {
                return;
            }

        }

        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        // Make A new Entry Left 

        private void checkBoxMakeNewEntry_CheckedChanged(object sender, EventArgs e)
        {
            HandleNew();
        }

        // Show New 
        private void HandleNew()
        {
            if (InternalChange == true)
            {
                InternalChange = false;
                return;
            }

            labelFileDefinition.Text = "NEW ACTION DEFINITION"; 

            if (checkBoxMakeNewEntry.Checked == true)
            {
                //buttonAdd.Enabled = true;
                buttonAddLeft.Show();
                buttonUpdateLeft.Hide();
                buttonDeleteLeft.Hide();
                panel2.Hide(); 
                
                //941
               // comboBoxActionName.Text = "Not_Defined";
                comboBoxActionName.Enabled = true;
                textBoxOcc.Enabled = true;
                checkBoxIsGL.Checked = false; 
                textBoxOcc.ReadOnly = false;
                checkBoxIsGL.Enabled = true;
                InternalChange = true;

                checkBoxMakeNewEntry.Checked = false;


            }
            else
            {
                buttonAddLeft.Hide();
                buttonUpdateLeft.Show();
                buttonDeleteLeft.Show();
                //buttonRefresh.Hide();

                //comboBoxStructureId.Enabled = false;

            }

            //Form502_Load(this, new EventArgs());
        }

        // When Index Change
        private void comboBoxFileId_SelectedIndexChanged(object sender, EventArgs e)
        {

          
          

        }
       


        // 


        private void buttonNext_Click(object sender, EventArgs e)
        {
            WSourceFileID = Rs.SourceFileId;

            if (WMode == 1)
            {

                if (WSourceFileID == "Atms_Journals_Txns")
                {
                    MessageBox.Show("Nothing to show for this file");
                    return;
                }
                //  Form78d_FileRecords NForm78d_FileRecords;
                // textBoxFileId.Text

                string WMatchingCateg = "";
                int WReconcCycleNo = 0;
                string WFullPath_01 = "";

                int WModeBulk = 9; // BULK 

                bool WCategoryOnly = false;

                // Find out if record in BULK 
                RRDM_BULK_IST_AndOthers_Records_ALL_2 Bio = new RRDM_BULK_IST_AndOthers_Records_ALL_2();

                RRDM_BULK_IST_AndOthers_Records_ALL_2 Bg = new RRDM_BULK_IST_AndOthers_Records_ALL_2();
                // Create and Load if not already present
                RRDM_LoadFiles_InGeneral_Auto La = new RRDM_LoadFiles_InGeneral_Auto();

                // Read fields of existing table
                bool WithHeaders = false;
                // 
                Bg.ReadTableToGetFieldNames_Array_List(WSourceFileID, WithHeaders, 1);
                // Bio.ReadTableToSeeIfRecords(WSourceFileID);
                if (Bg.RecordFound == true)
                {
                    // Read to see if records exist
                    Bio.ReadTableToSeeIfRecords(WSourceFileID);
                }

                // FIND FULL FILE PATH
                if (Bg.RecordFound == true & Bio.RecordFound == true)
                {
                    // DEFINITION OF BULK FILE FOUND 
                    // Also RECORDS IN FILE EXISTS
                }
                else
                {
                    // NEED 
                    // LOAD BULK TABLE IF NEEDED
                    
                    string[] specificFiles = Directory.GetFiles(Rs.SourceDirectory, WSourceFileID + "_????????.???");

                    if (specificFiles == null || specificFiles.Length == 0)
                    {
                        MessageBox.Show("No such a text file in the directory..." + Rs.SourceDirectory);

                        return;
                    }

                    string pattern = WSourceFileID + "_[0-9]{8}\\.[0-9]{3}";

                    foreach (string file in specificFiles)
                    {
                        //int FileLen = file.Length;
                        long length = new System.IO.FileInfo(file).Length;
                        if (length < 1500)
                        {
                            File.Delete(file);
                            continue;
                        }
                        if (Regex.IsMatch(file, pattern))
                        {
                            // Found a file that matches!
                            var resultLastThreeDigits = file.Substring(file.Length - 3);
                            string result1 = file.Substring(file.Length - 12);
                            string result2 = result1.Substring(0, 8);

                            DateTime FileDATEresult;

                            if (DateTime.TryParseExact(result2, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out FileDATEresult))
                            {

                            }

                            WFullPath_01 = Rs.SourceDirectory + "\\" + WSourceFileID + "_" + FileDATEresult.ToString("yyyyMMdd") + "." + resultLastThreeDigits;

                        }
                        else
                        {
                            MessageBox.Show("Not the proper file in Directory");
                            return;
                        }

                    }

                    

                }

             

            }
            else
            {
                // EXTRA ON THE RRDM FILES 
                Form502_V03_TablesOther NForm502_V03_TablesOther;

                NForm502_V03_TablesOther = new Form502_V03_TablesOther(WSignedId, WSignRecordNo,
                                                                    WOperator, WSourceFileID);

                NForm502_V03_TablesOther.ShowDialog();

            }


           
        }
        // View File 
        private void buttonViewFile_Click(object sender, EventArgs e)
        {
            WSourceFileID = Rs.SourceFileId;

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

            // Find out if record in BULK 
            RRDM_BULK_IST_AndOthers_Records_ALL_2 Bio = new RRDM_BULK_IST_AndOthers_Records_ALL_2();
            Bio.ReadTableToSeeIfRecords(WSourceFileID); 

            if (Bio.RecordFound == true)
            {
                // Records Found
                // Show file 

                NForm78d_FileRecords = new Form78d_FileRecords(WOperator, WSignedId, WSourceFileID, "", WReconcCycleNo, WMatchingCateg, WMode, WCategoryOnly);
                NForm78d_FileRecords.ShowDialog();

                return; 

            }
            else
            {
                // FIND FULL FILE PATH

                string WFullPath_01 = "";

                string[] specificFiles = Directory.GetFiles(Rs.SourceDirectory, WSourceFileID + "_????????.???");

                if (specificFiles == null || specificFiles.Length == 0)
                {
                    MessageBox.Show("No such a text file in the directory..." + Rs.SourceDirectory);

                    return;
                }

                string pattern = WSourceFileID + "_[0-9]{8}\\.[0-9]{3}";

                foreach (string file in specificFiles)
                {
                    //int FileLen = file.Length;
                    long length = new System.IO.FileInfo(file).Length;
                    if (length < 1500)
                    {
                        File.Delete(file);
                        continue;
                    }
                    if (Regex.IsMatch(file, pattern))
                    {
                        // Found a file that matches!
                        var resultLastThreeDigits = file.Substring(file.Length - 3);
                        string result1 = file.Substring(file.Length - 12);
                        string result2 = result1.Substring(0, 8);

                        DateTime FileDATEresult;

                        if (DateTime.TryParseExact(result2, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out FileDATEresult))
                        {

                        }

                        WFullPath_01 = Rs.SourceDirectory + "\\" + WSourceFileID + "_" + FileDATEresult.ToString("yyyyMMdd") + "." + resultLastThreeDigits;

                    }
                    else
                    {
                        MessageBox.Show("Not the proper file in Directory");
                        return;
                    }

                }

                // Create and Load if not already present
                RRDM_LoadFiles_InGeneral_Auto La = new RRDM_LoadFiles_InGeneral_Auto();

                
                // Show file 

                NForm78d_FileRecords = new Form78d_FileRecords(WOperator, WSignedId, WSourceFileID, "", WReconcCycleNo, WMatchingCateg, WMode, WCategoryOnly);
                NForm78d_FileRecords.ShowDialog();
            }

        }

        private void checkBoxIsGL_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxIsGL.Checked == true)
            {
                panel2.Show();
            }
            else
            {
                panel2.Hide();
            }
        }

        private void comboBoxActionName_SelectedIndexChanged(object sender, EventArgs e)
        {
            string ParamId = "941";
            string OccuranceNm = comboBoxActionName.Text;
            Gp.ReadParametersSpecificNm(WOperator, ParamId, OccuranceNm); 
            textBoxActionId.Text = Gp.OccuranceId;

            //ParId = "920";
            //OccurId = "2";
            //Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

        }
// Account Name 1 
        private void comboBoxAccName_1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string ParamId = "701";
            string OccuranceNm = comboBoxAccName_1.Text;
            Gp.ReadParametersSpecificNm(WOperator, ParamId, OccuranceNm);
            textBoxAccShort_1.Text = Gp.OccuranceId;

            //ParId = "701";
            //OccurId = "2";
            //Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            //comboBoxAccName_1.Text = Gp.OccuranceNm;
        }
        // Account Name 2 
        private void comboBoxAccName_2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string ParamId = "701";
            string OccuranceNm = comboBoxAccName_2.Text;
            Gp.ReadParametersSpecificNm(WOperator, ParamId, OccuranceNm);
            textBoxAccShort_2.Text = Gp.OccuranceId;
        }
    }
}


