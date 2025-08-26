using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

// Alecos

namespace RRDM4ATMsWin
{
    public partial class Form502_V02 : Form
    {

        RRDMMatchingSourceFiles Rs = new RRDMMatchingSourceFiles();
        RRDMMappingFileFieldsFromBankToRRDM Rff = new RRDMMappingFileFieldsFromBankToRRDM();

        RRDMUniversalTableFieldsDefinition Utd = new RRDMUniversalTableFieldsDefinition();

        RRDMGasParameters Gp = new RRDMGasParameters();

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

        public Form502_V02(string InSignedId, int SignRecordNo, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;

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

            string field0="20180716";
            string field1="My Name kqkjkjqwkwjkqwek";
            if (field1.Length > 10)
                field1 = field1.Substring(0, 10);
            string field2="1000.00";
            string field3="ATM100";
            string field4 = "ATM10";

            String formatttedstring1 = string.Format("{0, 10}{1, -12}{2, -16}{3, -20}", field0, field1, field2, field3);
            String formatttedstring2 = string.Format("{0}{1, -12}", formatttedstring1, field4);

            string field8 = "20180716";

        }
        // Load 
        private void Form502_Load(object sender, EventArgs e)
        {
            // Source Files (Grid-ONE)
            string Filter1 = "Operator = '" + WOperator + "' ";

            Rs.ReadReconcSourceFilesToFillDataTable(Filter1);

            dataGridViewFiles.DataSource = Rs.SourceFilesDataTable.DefaultView;

            dataGridViewFiles.Columns[0].Width = 60; // SeqNo
            dataGridViewFiles.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewFiles.Columns[0].Visible = false; 

            dataGridViewFiles.Columns[1].Width = 60; // FileSeq"
            dataGridViewFiles.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridViewFiles.Columns[2].Width = 170; // SourceFile_ID
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
            WSourceFileID = rowSelected.Cells[1].Value.ToString();

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

            Rs.ReadReconcSourceFilesBySeqNo(WSeqNoLeft);
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
            textBoxSourceFileDirectory.Text = Rs.SourceDirectory;
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

            //InternalChange = true;

            //checkBoxMakeNewEntry.Checked = false;

            //HandleNew();

            buttonAddLeft.Hide();
            buttonUpdateLeft.Show();
            buttonDeleteLeft.Show();
            buttonRefresh.Hide();

            if (comboBoxFileId.Text != "Not_Defined")
            {
                comboBoxFileId.Enabled = false;
                comboBoxFileOrigin.Enabled = false;
               // comboBoxSourceFileType.Enabled = false;
            }


            comboBoxStructureId.Enabled = false;

            Rff.ReadFileFields(WSourceFileID);

            dataGridViewFields.DataSource = Rff.DataTableFileFields.DefaultView;

            // Create records

            Utd.ReadUniversalTableFieldsDefinitionToCreateBankToRRDMRecords(WSourceFileID, comboBoxStructureId.Text);

            if (Utd.RecordFound = false)
            {
                MessageBox.Show("No records found in Universal for this Structure or Operator.");
            }

            //Rmf.ReadMatchingFieldsToCreateBankToRRDMRecords(WOperator, WSourceFileName);

            // Read Again for updating 
            Rff.ReadFileFields(WSourceFileID);

            dataGridViewFields.DataSource = Rff.DataTableFileFields.DefaultView;

            //}

            string Result = Rff.ReadCheckForBrokenSequence(Rff.SourceFileId);

            if (Result != "")
            {
                label18.Show();
                textBoxMsgBoard.Text = Result;
                label18.ForeColor = Color.Red;
            }
            else
            {
                label18.Hide();
                textBoxMsgBoard.Text = Result;
            }


            dataGridViewFields.Columns[0].Width = 60; // PositionStart
            dataGridViewFields.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewFields.Columns[0].HeaderText = "Position Start"; 

            dataGridViewFields.Columns[1].Width = 60; // PositionEnd
            dataGridViewFields.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewFields.Columns[1].HeaderText = "Position End";
            //dataGridView2.Sort(dataGridView2.Columns[0], ListSortDirection.Ascending);

            dataGridViewFields.Columns[2].Width = 120; // 

            dataGridViewFields.Columns[3].Width = 70; // IsUniversal
            dataGridViewFields.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridViewFields.Columns[4].Width = 130; // TargetFieldNm
            dataGridViewFields.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewFields.Columns[5].Width = 120; // SourceFieldValue
            dataGridViewFields.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewFields.Columns[6].Width = 120; // TargetFieldValue
            dataGridViewFields.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewFields.Columns[7].Width = 60; // 
            dataGridViewFields.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewFields.Columns[7].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

        }
        // Row Enter Right 
        private void dataGridViewFields_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridViewFields.Rows[e.RowIndex];

            WSeqNoRight = (int)rowSelected.Cells[7].Value;

            Rff.ReadReadFileFieldsbySeqNo(WSeqNoRight);

            textBoxSourceFieldNm.Text = Rff.SourceFieldNm;
            textBoxTargetFieldName.Text = Rff.TargetFieldNm;
            textBoxFrom.Text = Rff.SourceFieldPositionStart.ToString();
            textBoxTo.Text = Rff.SourceFieldPositionEnd.ToString();

            if (IsDelimiterFile == true)
            {
                textBoxTo.ReadOnly = true;
            }
            else
            {
                textBoxTo.ReadOnly = false;
            }

            comboBoxFieldType.Text = Rff.TargetFieldType;

            comboBoxPar916.Text = Rff.SourceFieldValue;
            textBoxTargetFieldValue.Text = Rff.TargetFieldValue;

            checkBoxIsUniversal.Checked = Rff.IsUniversal;

            if (checkBoxIsUniversal.Checked == true)
            {
                textBoxTargetFieldName.ReadOnly = true;
                comboBoxFieldType.Enabled = false;
            }
            else
            {
                textBoxTargetFieldName.ReadOnly = false;
                comboBoxFieldType.Enabled = true;
            }

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
                 comboBoxSourceFileType.Text == "Not_Defined" || textBoxFileNmMask.Text == "Not_Defined")
            {
                MessageBox.Show("Please Fill In All Necessary Information ");
                return;
            }

            Rs.SourceDirectory = textBoxSourceFileDirectory.Text;
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

            Rs.Operator = WOperator;

            Rs.InsertReconcSourceFileRecord();

            Form502_Load(this, new EventArgs());

            MessageBox.Show("File with id: " + comboBoxFileId.Text + " Inserted");
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
            Rs.SourceDirectory = textBoxSourceFileDirectory.Text;
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
        // Add Right 
        private void buttonAddRight_Click(object sender, EventArgs e)
        {
            WRowIndexLeft = dataGridViewFiles.SelectedRows[0].Index;
            //
            //  Validation
            //

            // Check if source already exists 
            //
            Rff.ReadFileFieldsBySourceField(WSourceFileID, textBoxSourceFieldNm.Text);

            if (Rff.RecordFound == true)
            {
                MessageBox.Show("Same Source Field Already exist");
                return;
            }

            // Check if target already exists
            //
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

            // Check Valid Range 

            int FromPos;
            int ToPos;

            //
            //  Validation
            //
            if (int.TryParse(textBoxFrom.Text, out FromPos))
            {
            }
            else
            {
                MessageBox.Show(comboBoxFileOrigin.Text, "Please enter a valid number in field Start. ");
                return;
            }

            if (int.TryParse(textBoxTo.Text, out ToPos))
            {
            }
            else
            {
                MessageBox.Show(textBoxTo.Text, "Please enter a valid number in field End. ");
                return;
            }

            // Check Valid Range 

            string Result = Rff.ReadCheckWithinOtherRange(WSourceFileID, textBoxSourceFieldNm.Text, FromPos, ToPos);

            if (Result != "")
            {
                MessageBox.Show(Result);
                return;
            }

            Rff.SourceFileId = WSourceFileID;
            Rff.SourceFieldNm = textBoxSourceFieldNm.Text;

            Rff.SourceFieldPositionStart = FromPos;
            Rff.SourceFieldPositionEnd = ToPos;

            if (checkBoxIsUniversal.Checked == true)
            {
                Rff.IsUniversal = true;
            }
            else
            {
                Rff.IsUniversal = false;
            }

            Rff.TargetFieldNm = textBoxTargetFieldName.Text;

            Rff.TargetFieldType = comboBoxFieldType.Text;
            Rff.SourceFieldValue = comboBoxPar916.Text;
            Rff.TargetFieldValue = textBoxTargetFieldValue.Text;

            Rff.InsertFileFieldRecord();

            Form502_Load(this, new EventArgs());

            dataGridViewFiles.Rows[WRowIndexLeft].Selected = true;
            dataGridViewFiles_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

        }
        // Update Right 
        bool NotPresent;
        private void buttonUpdateRight_Click(object sender, EventArgs e)
        {
            WRowIndexLeft = dataGridViewFiles.SelectedRows[0].Index;
            WRowIndexRight = dataGridViewFields.SelectedRows[0].Index;

            if (textBoxSourceFieldNm.Text == "NotPresent")
            {
                NotPresent = true;
                textBoxFrom.Text = "0";
                textBoxTo.Text = "0";
            }
            else
            {
                NotPresent = false;
            }

            Rff.ReadReadFileFieldsbySeqNo(WSeqNoRight);

            if (Rff.IsUniversal == true & checkBoxIsUniversal.Checked == false)
            {
                MessageBox.Show("You cannot change the Universal Atribute for this field");
                return;
            }
            if (Rff.IsUniversal == false & checkBoxIsUniversal.Checked == true)
            {
                MessageBox.Show("You cannot change the Universal Atribute for this field");
                return;
            }

            string OldSourceFieldNm = Rff.SourceFieldNm;
            string OldTargetFieldNm = Rff.TargetFieldNm;

            if (OldSourceFieldNm != textBoxSourceFieldNm.Text)
            {
                // New Source to be updated 
                Rff.ReadFileFieldsBySourceField(WSourceFileID, textBoxSourceFieldNm.Text);

                if (Rff.RecordFound == true & NotPresent == false)
                {
                    MessageBox.Show("Same Source Field Already exist");
                    return;
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

            int FromPos;
            int ToPos;

            //
            //  Validation
            //
            if (int.TryParse(textBoxFrom.Text, out FromPos))
            {
            }
            else
            {
                MessageBox.Show(comboBoxFileOrigin.Text, "Please enter a valid number in field Start. ");
                return;
            }

            if (int.TryParse(textBoxTo.Text, out ToPos))
            {
            }
            else
            {
                MessageBox.Show(textBoxTo.Text, "Please enter a valid number in field End. ");
                return;
            }

            // CHECK if exist in other range
            // 
            string Result = Rff.ReadCheckWithinOtherRange(Rff.SourceFileId, OldSourceFieldNm, FromPos, ToPos);

            if (Result != "" & NotPresent == false)
            {
                MessageBox.Show(Result);
                return;
            }

            // Read Again to position record 

            Rff.ReadReadFileFieldsbySeqNo(WSeqNoRight);

            Rff.SourceFileId = WSourceFileID;

            Rff.SourceFieldNm = textBoxSourceFieldNm.Text;

            Rff.SourceFieldPositionStart = FromPos;
            Rff.SourceFieldPositionEnd = ToPos;

            if (checkBoxIsUniversal.Checked == true)
            {
                Rff.IsUniversal = true;
            }
            else
            {
                Rff.IsUniversal = false;
            }

            Rff.TargetFieldNm = textBoxTargetFieldName.Text;

            Rff.TargetFieldType = comboBoxFieldType.Text;
            Rff.SourceFieldValue = comboBoxPar916.Text;
            Rff.TargetFieldValue = textBoxTargetFieldValue.Text;


            // UPDATE 
            Rff.UpdateFileFieldRecord(WSeqNoRight);

            Form502_Load(this, new EventArgs());

            dataGridViewFiles.Rows[WRowIndexLeft].Selected = true;
            dataGridViewFiles_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

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

            dataGridViewFiles.Rows[WRowIndexLeft].Selected = true;
            dataGridViewFiles_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

        }


        // Is Universal 
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxIsUniversal.Checked == true)
            {
                textBoxTargetFieldName.ReadOnly = true;
                comboBoxFieldType.Enabled = false;
            }
            else
            {
                textBoxTargetFieldName.ReadOnly = false;
                comboBoxFieldType.Enabled = true;
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
                Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

                textBoxSourceFileDirectory.Text = Gp.OccuranceNm + comboBoxFileId.Text;
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

                textBoxExceptionsDirectory.Text = Gp.OccuranceNm ;

                textBoxFileNmMask.Text = "Not_Defined";
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

                comboBoxSourceFileType.Enabled = true;

                comboBoxDelimiter.Enabled = true;

                comboBoxStructureId.Enabled = true;

                InternalChange = true;

                checkBoxMakeNewEntry.Checked = false;

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

            string ParId = "920";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            textBoxSourceFileDirectory.Text = Gp.OccuranceNm + comboBoxFileId.Text;

            ParId = "920";
            OccurId = "2";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            textBoxArchiveDirectory.Text = Gp.OccuranceNm + comboBoxFileId.Text;

            // Occurance 3
            ParId = "920";
            OccurId = "3";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            textBoxExceptionsDirectory.Text = Gp.OccuranceNm ;

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
// PositionFrom Change => change the two as well
        private void textBoxFrom_TextChanged(object sender, EventArgs e)
        {
            if (IsDelimiterFile == true)
            {
                textBoxTo.Text = textBoxFrom.Text; 
            }
        }
    }
}
