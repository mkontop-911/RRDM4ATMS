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
    public partial class Form502_V03 : Form
    {

        RRDMMatchingSourceFiles Rs = new RRDMMatchingSourceFiles();
        RRDMMappingFileFieldsFromBankToRRDM Rff = new RRDMMappingFileFieldsFromBankToRRDM();

        RRDMUniversalTableFieldsDefinition Utd = new RRDMUniversalTableFieldsDefinition();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDM_BULK_IST_AndOthers_Records_ALL_2 Bg = new RRDM_BULK_IST_AndOthers_Records_ALL_2();

        string W_Application; 

        ////int WRowIndex;
        string WSourceFileID;
        //string WReconcCategory;
        int WSeqNoLeft;
        int WSeqNoRight;

        bool InternalChange;
        int WRowIndex;
        //int WRowIndexMatching; 

        //int WMatchingSeqNo; 
        bool IsDelimiterFile;

        int WRowIndexLeft;
        int WRowIndexRight;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        int WMode; 

        public Form502_V03(string InSignedId, int SignRecordNo, string InOperator, int InMode)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WMode = InMode; // 1 equal to normal process
                            // 2 equal to EXTRA process 

            InitializeComponent();

            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.RecordFound == true)
            {
                W_Application = Usi.SignInApplication;

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
                    labelStep1.Text = "Configuration Files Menu_" + W_Application;
             
                }
                else
                {
                    W_Application = "ATMs";
                }
            }

            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;

            labelUserId.Text = InSignedId;
            //Field Type for File Fields 

            // File Id Structure
            Gp.ParamId = "917";
            comboBoxStructureId.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxStructureId.DisplayMember = "DisplayValue";

            // File Ids
            Gp.ParamId = "918";
            comboBoxFileId.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxFileId.DisplayMember = "DisplayValue";

            // From What System 
            Gp.ParamId = "919";
            comboBoxFileOrigin.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxFileOrigin.DisplayMember = "DisplayValue";

            // POOL
            Gp.ParamId = "920";
            comboBoxSourceFileDirectory.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxSourceFileDirectory.DisplayMember = "DisplayValue";

            // Source File Layout type 
            Gp.ParamId = "921";
            comboBoxSourceFileType.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxSourceFileType.DisplayMember = "DisplayValue";

            // Delimiters
            Gp.ParamId = "922";
            comboBoxDelimiter.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxDelimiter.DisplayMember = "DisplayValue";

            //comboBoxStructureId.Text = "Atms And Cards";

            //string str = null;
            //string retString = null;
            //str = "This is substring test";
            //retString = str.Substring(8, 9);
            ////str.Substring(8, 9) = "Pan" : 
            //MessageBox.Show(retString);

            string field0 = "20180716";
            string field1 = "My Name kqkjkjqwkwjkqwek";
            if (field1.Length > 10)
                field1 = field1.Substring(0, 10);
            string field2 = "1000.00";
            string field3 = "ATM100";
            string field4 = "ATM10";

            String formatttedstring1 = string.Format("{0, 10}{1, -12}{2, -16}{3, -20}", field0, field1, field2, field3);
            String formatttedstring2 = string.Format("{0}{1, -12}", formatttedstring1, field4);

            string field8 = "20180716";

        }
        // Load 
        private void Form502_Load(object sender, EventArgs e)
        {
            // Source Files (Grid-ONE)
           // WOperator = "BCAIEGCX"; 
            string Filter1 = "Operator = '" + WOperator + "' ";

            Rs.ReadReconcSourceFilesToFillDataTable(Filter1);

            dataGridViewFiles.DataSource = Rs.SourceFilesDataTable.DefaultView;

            dataGridViewFiles.Columns[0].Width = 60; // SeqNo
            dataGridViewFiles.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewFiles.Columns[0].Visible = false;

            dataGridViewFiles.Columns[1].Width = 60; // FileSeq"
            dataGridViewFiles.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridViewFiles.Columns[2].Width = 230; // SourceFile_ID
            dataGridViewFiles.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewFiles.Columns[3].Width = 100; // OriginSystem
            dataGridViewFiles.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewFiles.Columns[4].Width = 50; // Type
            dataGridViewFiles.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewFiles.Columns[4].Visible = false;

        }
        // Row Enter Left


        private void dataGridViewFiles_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridViewFiles.Rows[e.RowIndex];

            WSeqNoLeft = (int)rowSelected.Cells[0].Value;

            Rs.ReadReconcSourceFilesBySeqNo(WSeqNoLeft);

            WSourceFileID = Rs.SourceFileId;

            if (WSourceFileID == "Atms_Journals_Txns")
            {
                labelFileDefinition.Hide();
                panelSelectFields.Hide();
            }
            else
            {
                labelFileDefinition.Show();
                panelSelectFields.Show();
            }

            labelFileDefinition.Text = "DEFINITION FOR FILE : " + WSourceFileID;

            if (Rs.Enabled == true) checkBoxEnable.Checked = true;
            else checkBoxEnable.Checked = false;
            if (Rs.LayoutId == "DelimiterFile")
            {
                IsDelimiterFile = true;


            }
            else
            {
                IsDelimiterFile = false;

            }

            comboBoxFileOrigin.Text = Rs.SystemOfOrigin;
            comboBoxFileId.Text = Rs.SourceFileId;
            string ParId ="";
            string OccurId = "";
            Gp.OccuranceNm = ""; 
            if (W_Application == "ETISALAT")
            {
                ParId = "920";
                OccurId = "15";
                Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            }
            if (W_Application == "QAHERA")
            {
                ParId = "920";
                OccurId = "16";
                Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            }
            if (W_Application == "IPN")
            {
                ParId = "920";
                OccurId = "17";
                Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            }
            if (W_Application == "EGATE")
            {
                ParId = "920";
                OccurId = "18";
                Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            }

            
            if (Gp.OccuranceNm != "")
            {
                comboBoxSourceFileDirectory.Text = Gp.OccuranceNm;
            }
            else
            {
                comboBoxSourceFileDirectory.Text = Rs.SourceDirectory;
            }

            textBoxFileNmMask.Text = Rs.FileNameMask;
            textBoxArchiveDirectory.Text = Rs.ArchiveDirectory;

            textBoxExceptionsDirectory.Text = Rs.ExceptionsDirectory;

            comboBoxSourceFileType.Text = Rs.LayoutId;
            comboBoxDelimiter.Text = Rs.Delimiter;
            textBoxFileSeq.Text = Rs.Type;
            txtLinesInHeader.Text = Rs.LinesInHeader.ToString();
            txtLinesInTrailer.Text = Rs.LinesInTrailer.ToString();
            textBoxDBtbl.Text = Rs.InportTableName;
            comboBoxStructureId.Text = Rs.TableStructureId;
            checkBoxMoveToMatched.Checked = Rs.IsMoveToMatched; 

            //InternalChange = true;

            //checkBoxMakeNewEntry.Checked = false;

            //HandleNew();

            buttonAddLeft.Hide();
            buttonUpdateLeft.Show();
            buttonDeleteLeft.Show();
            buttonRefresh.Hide();

            buttonViewFile.Show();

            if (comboBoxFileId.Text != "Not_Defined")
            {
                comboBoxFileId.Enabled = false;
                comboBoxFileOrigin.Enabled = false;
                // comboBoxSourceFileType.Enabled = false;
            }

            comboBoxStructureId.Enabled = false;

            if (WMode == 2)
            {
                checkBoxMakeNewEntry.Hide();
                buttonAddLeft.Hide();
                buttonUpdateLeft.Hide();
                buttonDeleteLeft.Hide();
                buttonViewFile.Hide();
                labelStep1.Text = "Definition of EXTRA on RRDM tables";
            }

            // Create records if missing

            Utd.ReadUniversalTableFieldsDefinitionToCreateBankToRRDMRecords(WSourceFileID, comboBoxStructureId.Text);

            if (Utd.RecordFound = false)
            {
                MessageBox.Show("No records found in Universal for this Structure or Operator.");
            }
            //
            // Delete records that were deleted from Universal fields
            //
            Rff.ReadTableAndDeleteNotInUniversal(WSourceFileID, comboBoxStructureId.Text);

        }

        // ADD Left
        private void buttonAddLeft_Click(object sender, EventArgs e)
        {
            Rs.Enabled = checkBoxEnable.Checked;

            Rs.SourceFileId = comboBoxFileId.Text;
            Rs.SystemOfOrigin = comboBoxFileOrigin.Text;

            Rs.ReadReconcSourceFilesByFileId(Rs.SourceFileId);
            if (Rs.RecordFound == true)
            {
                MessageBox.Show("File Id Already exist");
                return;
            }

            if (comboBoxFileId.Text == "Not_Defined" || comboBoxFileOrigin.Text == "Not_Defined" ||
                 comboBoxSourceFileType.Text == "Not_Defined" || textBoxFileNmMask.Text == "Not_Defined"
                 || comboBoxStructureId.Text == "Not_Defined"
                 )
            {
                MessageBox.Show("Please Fill In All Necessary Information ");
                return;
            }

            Rs.SourceDirectory = comboBoxSourceFileDirectory.Text + comboBoxFileId.Text;
            Rs.FileNameMask = textBoxFileNmMask.Text;
            Rs.ArchiveDirectory = textBoxArchiveDirectory.Text;
            Rs.ExceptionsDirectory = textBoxExceptionsDirectory.Text;
            Rs.LayoutId = comboBoxSourceFileType.Text;

            Rs.Delimiter = comboBoxDelimiter.Text;

            if (int.TryParse(txtLinesInHeader.Text, out Rs.LinesInHeader))
            {
            }
            else
            {
                MessageBox.Show(txtLinesInHeader.Text, "Please enter a valid number for header!");

                return;
            }


            if (int.TryParse(txtLinesInTrailer.Text, out Rs.LinesInTrailer))
            {
            }
            else
            {
                MessageBox.Show(txtLinesInTrailer.Text, "Please enter a valid number for trailer!");

                return;
            }

            //Rs.LinesInHeader = int.Parse(txtLinesInHeader.Text);
            //Rs.LinesInTrailer = int.Parse(txtLinesInTrailer.Text);

            Rs.Type = textBoxFileSeq.Text;

            Rs.InportTableName = textBoxDBtbl.Text;
            Rs.WorkingTableName = "";

            Rs.TableStructureId = comboBoxStructureId.Text;

            Rs.IsMoveToMatched = checkBoxMoveToMatched.Checked; 

            Rs.Operator = WOperator;

            Rs.InsertReconcSourceFileRecord();

            buttonViewFile.Show();

            string SavedFileId = Rs.SourceFileId; 

            Form502_Load(this, new EventArgs());

            MessageBox.Show("File with id: " + SavedFileId + " Inserted");
            textBoxMsgBoard.Text = "File Inserted";

        }
        // Update Left
        private void buttonUpdateLeft_Click(object sender, EventArgs e)
        {
            WRowIndexLeft = dataGridViewFiles.SelectedRows[0].Index;

            Rs.ReadReconcSourceFilesBySeqNo(WSeqNoLeft);

            Rs.Enabled = checkBoxEnable.Checked;

            Rs.SystemOfOrigin = comboBoxFileOrigin.Text;
            Rs.SourceFileId = comboBoxFileId.Text;
            Rs.SourceDirectory = comboBoxSourceFileDirectory.Text+ comboBoxFileId.Text;
            Rs.FileNameMask = textBoxFileNmMask.Text;
            Rs.ArchiveDirectory = textBoxArchiveDirectory.Text;
            Rs.ExceptionsDirectory = textBoxExceptionsDirectory.Text;
            Rs.LayoutId = comboBoxSourceFileType.Text;
            Rs.Delimiter = comboBoxDelimiter.Text;
            Rs.Type = textBoxFileSeq.Text;
            Rs.LinesInHeader = int.Parse(txtLinesInHeader.Text);
            Rs.LinesInTrailer = int.Parse(txtLinesInTrailer.Text);

            if (textBoxDBtbl.Text == "")
            {
                MessageBox.Show("Please Fill In Table Name In DB ");
                return;
            }
            Rs.InportTableName = textBoxDBtbl.Text;

            Rs.IsMoveToMatched = checkBoxMoveToMatched.Checked;

            Rs.UpdateReconcSourceFileRecord(WSeqNoLeft);

            Form502_Load(this, new EventArgs());

            dataGridViewFiles.Rows[WRowIndexLeft].Selected = true;
            dataGridViewFiles_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

            MessageBox.Show("File Updated");
            textBoxMsgBoard.Text = "File Updated";
        }
        // Delete Left 
        private void buttonDeleteLeft_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Warning: Do you want to delete this file ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                         == DialogResult.Yes)
            {
                Rs.DeleteFileFieldRecord(WSeqNoLeft, Rs.SourceFileId);

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

            labelFileDefinition.Text = "NEW FILE DEFINITION"; 

            if (checkBoxMakeNewEntry.Checked == true)
            {
                //buttonAdd.Enabled = true;
                buttonAddLeft.Show();
                buttonUpdateLeft.Hide();
                buttonDeleteLeft.Hide();

                checkBoxEnable.Checked = false;

                //918
                comboBoxFileId.Text = "Not_Defined";

                //919
                comboBoxFileOrigin.Text = "Not_Defined";

                string ParId = "920";
                string OccurId = "1";

                if (W_Application == "ATMs")
                {
                    ParId = "920";
                    OccurId = "1";
                    Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
                }
                if (W_Application == "ETISALAT")
                {
                    ParId = "920";
                    OccurId = "15";
                    Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
                }
                if (W_Application == "QAHERA")
                {
                    ParId = "920";
                    OccurId = "16";
                    Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
                }
                if (W_Application == "IPN")
                {
                    ParId = "920";
                    OccurId = "17";
                    Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
                }
                if (W_Application == "EGATE")
                {
                    ParId = "920";
                    OccurId = "18";
                    Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
                }

                comboBoxSourceFileDirectory.Text = Gp.OccuranceNm ;
                // Occurance 2
                ParId = "920";
                OccurId = "2";
                Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

                textBoxArchiveDirectory.Text = Gp.OccuranceNm + comboBoxFileId.Text;

                textBoxFileSeq.Text = "0";

                txtLinesInHeader.Text = "0";
                txtLinesInTrailer.Text = "0";

                // Occurance 3
                ParId = "920";
                OccurId = "3";
                Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

                textBoxExceptionsDirectory.Text = Gp.OccuranceNm;

                textBoxFileNmMask.Text = "_yyyymmdd.999";
                //920
                comboBoxSourceFileType.Text = "Not_Defined";
                textBoxDBtbl.Text = comboBoxFileId.Text;

                // READ 
                ParId = "918";
                //OccurId = "2";
                Gp.ReadParametersSpecificNm(WOperator, ParId, comboBoxFileId.Text);

                comboBoxStructureId.Text = Gp.GetParamOccurancesRelatedNm(WOperator, Gp.RelatedParmId, Gp.RelatedOccuranceId);

                comboBoxFileId.Enabled = true;
                comboBoxFileOrigin.Enabled = true;

                comboBoxSourceFileDirectory.Enabled = true; 

                comboBoxSourceFileType.Enabled = true;

                comboBoxDelimiter.Enabled = true;

                comboBoxStructureId.Enabled = true;

                InternalChange = true;

                checkBoxMakeNewEntry.Checked = false;

                buttonViewFile.Hide();

                checkBoxMoveToMatched.Checked = false; 

            }
            else
            {
                //buttonAddLeft.Hide();
                //buttonUpdateLeft.Show();
                //buttonDeleteLeft.Show();
                //buttonRefresh.Hide();

                //comboBoxStructureId.Enabled = false;

            }

            //Form502_Load(this, new EventArgs());
        }

        // When Index Change
        private void comboBoxFileId_SelectedIndexChanged(object sender, EventArgs e)
        {
            // POOL 
            string ParId = "920";
            string OccurId = "1";

            if (W_Application == "ATMs")
            {
                ParId = "920";
                OccurId = "1";
                Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            }
            if (W_Application == "ETISALAT")
            {
                ParId = "920";
                OccurId = "15";
                Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            }
            if (W_Application == "QAHERA")
            {
                ParId = "920";
                OccurId = "16";
                Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            }
            if (W_Application == "IPN")
            {
                ParId = "920";
                OccurId = "17";
                Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            }

            comboBoxSourceFileDirectory.Text = Gp.OccuranceNm ;

            ParId = "920";
            OccurId = "2";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            textBoxArchiveDirectory.Text = Gp.OccuranceNm + comboBoxFileId.Text;

            // Occurance 3
            ParId = "920";
            OccurId = "3";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            textBoxExceptionsDirectory.Text = Gp.OccuranceNm;

            // READ 
            ParId = "918";
            //OccurId = "2";
            Gp.ReadParametersSpecificNm(WOperator, ParId, comboBoxFileId.Text);

            comboBoxStructureId.Text = Gp.GetParamOccurancesRelatedNm(WOperator, Gp.RelatedParmId, Gp.RelatedOccuranceId);

            if (comboBoxFileId.Text == "Atms_Journals_Txns")
            {
                textBoxDBtbl.Text = "tblMatchingTxnsMasterPoolATMs";

            }
            else
            {
                textBoxDBtbl.Text = comboBoxFileId.Text;
            }

        }
        // Refresh 
        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            WRowIndex = dataGridViewFiles.SelectedRows[0].Index;

            Form502_Load(this, new EventArgs());

            dataGridViewFiles.Rows[WRowIndex].Selected = true;
            dataGridViewFiles_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));
        }


        // REformat 


        private void buttonNext_Click(object sender, EventArgs e)
        {
            if (Environment.MachineName != "RRDM-PANICOS")
            {
                MessageBox.Show("This new module is under construction");
                return; 
            }
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

                    // Create BULK table  and create RRDM Standard (STD) Table IF not created 
                    if (Bg.RecordFound == false)
                    {

                        La.CreateBulk_And_STD_RRDM_Tables(WFullPath_01, WSourceFileID, comboBoxDelimiter.Text);

                    }

                    // Load BULK from Delimiter file
                    La.LoadBulk_First_Table(WFullPath_01, WSourceFileID, comboBoxDelimiter.Text);

                }

                // Form502_V03_REFORMAT(string InSignedId, int SignRecordNo, string InOperator, string InSourceFileID)
                Form502_V03_REFORMAT Reformat = new Form502_V03_REFORMAT(WSignedId, WSignRecordNo, WOperator
                        , WSourceFileID, WFullPath_01, comboBoxDelimiter.Text);
                Reformat.Show();

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
            if (Environment.MachineName == "RRDM-PANICOS" || Environment.MachineName == "RRDM_ABE")
            {
                // OK MOVE ON
            }
            else
            {
                MessageBox.Show("This new module is under construction");
                return;
            }

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
                    MessageBox.Show("No such a text file in the directory..." + Rs.SourceDirectory+ Environment.NewLine
                           + "Format should be:"+ WSourceFileID + "_YYYYMMDD.001"
                        );

                    return;
                }

                string pattern = WSourceFileID + "_[0-9]{8}\\.[0-9]{3}";

                foreach (string file in specificFiles)
                {
                    //int FileLen = file.Length;
                    long length = new System.IO.FileInfo(file).Length;
                    if (length < 1500)
                    {
                        MessageBox.Show("File too small..." + file);
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

                // Create BULK table  and create RRDM Standard (STD) Table IF not created 
                La.CreateBulk_And_STD_RRDM_Tables(WFullPath_01, WSourceFileID, comboBoxDelimiter.Text);

                // Load BULK from Delimiter file
                La.LoadBulk_First_Table(WFullPath_01, WSourceFileID, comboBoxDelimiter.Text);

                // Show file 

                NForm78d_FileRecords = new Form78d_FileRecords(WOperator, WSignedId, WSourceFileID, "", WReconcCycleNo, WMatchingCateg, WMode, WCategoryOnly);
                NForm78d_FileRecords.ShowDialog();
            }

        }
    }
}


