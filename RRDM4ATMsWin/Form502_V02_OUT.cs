using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Data;

// Alecos

namespace RRDM4ATMsWin
{
    public partial class Form502_V02_OUT : Form
    {

        RRDMMatchingSourceFiles Rs = new RRDMMatchingSourceFiles();

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs(); 
      
        RRDMOutputFileDefinition Outf = new RRDMOutputFileDefinition(); 

        RRDMOutputFileFieldsMappingDefinition Om = new RRDMOutputFileFieldsMappingDefinition(); 

        RRDMGasParameters Gp = new RRDMGasParameters();

        ////int WRowIndex;
        string WOutputFileID;
        //string WReconcCategory;
        int WSeqNoLeft;
        int WSeqNoRight;

        bool InternalChangeLeft;
        bool InternalChangeRight;
        int WRowIndex;
        //int WRowIndexMatching; 

        //int WMatchingSeqNo; 

        int WRowIndexLeft;
        int WRowIndexRight;
        string WFieldSourceType; 

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        public Form502_V02_OUT(string InSignedId, int SignRecordNo, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;

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
            //Field Type for File Fields 
            comboBoxFieldType.Items.Add("Character");
            comboBoxFieldType.Items.Add("Numeric");
            comboBoxFieldType.Items.Add("Decimal");
            comboBoxFieldType.Items.Add("Boolean");
            comboBoxFieldType.Items.Add("DateTime");
            comboBoxFieldType.Items.Add("Date");
            comboBoxFieldType.Items.Add("Time");
            comboBoxFieldType.Text = "Character";

          
            //Outf.ReadReconcOutputFilesToFillDataTable2("");
            Mpa.ReadTablePoolDataToGetFieldNames("");

            if (Mpa.OutputFieldsDataTable.Rows.Count == 0)
            {
                MessageBox.Show("No Records to show");
            }
            else
            {
                comboBoxSourceFieldNm.Items.Add("NotPresent");
                int I = 0;
             
                while (I <= (Mpa.OutputFieldsDataTable.Rows.Count - 1))
                {
            
                    string NAME = (string)Mpa.OutputFieldsDataTable.Rows[I]["FieldNm"];

                    comboBoxSourceFieldNm.Items.Add(NAME);

                    I++;

                }
            }

            comboBoxSourceFieldNm.Text = "NotPresent"; 

            // Get Source Fields  
            //comboBoxSourceFieldNm.DataSource = Outf.OutputFieldsDataTable.DefaultView;
            //comboBoxSourceFieldNm.DisplayMember = "DisplayValue";

            Gp.ParamId = "916"; // Source Field Value
            comboBoxPar916.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxPar916.DisplayMember = "DisplayValue";

            // File Id Structure
            Gp.ParamId = "917";
            comboBoxSourceTbl.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxSourceTbl.DisplayMember = "DisplayValue";

            // File Ids
            Gp.ParamId = "927";

            comboBoxOutputFile.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxOutputFile.DisplayMember = "DisplayValue";

            // Version 
            Gp.ParamId = "928";
            comboBoxOutputVersion.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxOutputVersion.DisplayMember = "DisplayValue";

            // Version 
            Gp.ParamId = "929";
            comboBoxSourceTbl.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxSourceTbl.DisplayMember = "DisplayValue";

            // File Layout type 
            Gp.ParamId = "921";
            comboBoxOutputFileType.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxOutputFileType.DisplayMember = "DisplayValue";

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
            string Filter1 = " WHERE Operator = '" + WOperator + "' ";

            Outf.ReadReconcOutputFilesToFillDataTable(Filter1); 

            //Rs.ReadReconcSourceFilesToFillDataTable(Filter1);

            if (Outf.OutputFilesDataTable.Rows.Count ==0)
            {
                MessageBox.Show("No Records to show"); 
            }
            else
            {
                dataGridViewFiles.DataSource = Outf.OutputFilesDataTable.DefaultView;

                //dataGridViewFiles.Sort(dataGridViewFiles.Columns[0], ListSortDirection.Ascending);

                dataGridViewFiles.Columns[0].Width = 60; // Id
                dataGridViewFiles.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridViewFiles.Columns[1].Width = 170; // Id
                dataGridViewFiles.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridViewFiles.Columns[2].Width = 100; // Name
                dataGridViewFiles.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }    

        }
        // Row Enter Left
        private void dataGridViewFiles_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridViewFiles.Rows[e.RowIndex];

            WSeqNoLeft = (int)rowSelected.Cells[0].Value;

            Outf.ReadOutputFilesBySeqNo(WSeqNoLeft); 

            WOutputFileID = Outf.OutputFile_Id;

            labelFieldsDefinition.Text = "FIELDS DEFINITION FOR FILE : " + WOutputFileID;
          
            comboBoxOutputFile.Text = Outf.OutputFile_Id; 
            comboBoxOutputVersion.Text = Outf.OutputFile_Version;

            dateTimePickerEffectiveDt.Value = Outf.EffectiveDate;
            textBoxFileNmMask.Text = Outf.FileNameMask;

            comboBoxOutputFileType.Text = Outf.LayoutType;

            txtLinesInHeader.Text = Outf.LinesHeader.ToString();
            txtLinesInTrailer.Text = Outf.LinesTrailer.ToString();

            textBoxTargetDir.Text = Outf.TargetDirectory;

            comboBoxSourceTbl.Text = Outf.SourceTableName;

            textBoxArchiveDirectory.Text = Outf.ArchiveDirectory;

            textBoxExceptionsDirectory.Text = Outf.ExceptionsDirectory;


            //InternalChange = true;

            //checkBoxMakeNewEntry.Checked = false;

            //HandleNew();

            buttonAddLeft.Hide();
            buttonUpdateLeft.Show();
            buttonDeleteLeft.Show();
            buttonRefresh.Hide();

            if (comboBoxOutputFile.Text != "Not_Defined")
            {
                comboBoxOutputFile.Enabled = false;
                comboBoxOutputVersion.Enabled = false;
                comboBoxOutputFileType.Enabled = false;
                comboBoxSourceTbl.Enabled = false;
            }

            
            //Rff.ReadFileFields(WOutputFileID);
            string SelectionCriteria = " WHERE OutputFile_Id ='"+ Outf.OutputFile_Id + "'"
                                      + " AND OutputFile_Version ='" + Outf.OutputFile_Version + "'"; 

            Om.ReadOutputFileFieldsDefinitionToFillDataTable(SelectionCriteria); 

            if (Om.DataTableFileFields.Rows.Count == 0)
            {
                MessageBox.Show("No Records for this file");
                return; 
            }
            else
            {
                //dataGridViewFields.DataSource = Rff.DataTableFileFields.DefaultView;
                dataGridViewFields.DataSource = Om.DataTableFileFields.DefaultView;


                string Result = Om.ReadCheckForBrokenSequence(Outf.OutputFile_Id, Outf.OutputFile_Version);

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

                dataGridViewFields.Columns[0].Width = 60; // SeqNo
                dataGridViewFields.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridViewFields.Columns[0].Visible = false;


                dataGridViewFields.Columns[1].Width = 60; // OutputFile_Id 
                dataGridViewFields.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                //dataGridView2.Sort(dataGridView2.Columns[0], ListSortDirection.Ascending);
                dataGridViewFields.Columns[1].Visible = false; 

                dataGridViewFields.Columns[2].Width = 120; // OutputFile_Version
                dataGridViewFields.Columns[2].Visible = false;

                dataGridViewFields.Columns[3].Width = 110; // Source_Field_Nm
                dataGridViewFields.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridViewFields.Columns[3].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
                
                dataGridViewFields.Columns[4].Width = 60; // SourceFieldType
                dataGridViewFields.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                
                dataGridViewFields.Columns[5].Width = 110; // Target_Field_Nm
                dataGridViewFields.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridViewFields.Columns[5].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

                dataGridViewFields.Columns[6].Width = 60; // TargetFieldType
                dataGridViewFields.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridViewFields.Columns[7].Width = 60; // TargetFieldPositionStart
                dataGridViewFields.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridViewFields.Columns[7].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
                dataGridViewFields.Columns[7].HeaderText = "Position Start";
                
                dataGridViewFields.Columns[8].Width = 60; // TargetFieldPositionEnd
                dataGridViewFields.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridViewFields.Columns[8].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
                dataGridViewFields.Columns[8].HeaderText = "Position End";

                dataGridViewFields.Columns[9].Width = 60; // TargetDefaultValue
                dataGridViewFields.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                
                dataGridViewFields.Columns[10].Width = 120; // TransformationRoutine
                dataGridViewFields.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridViewFields.Columns[11].Width = 60; // Operator
                dataGridViewFields.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridViewFields.Columns[11].Visible = false;

            }

        }
        // Row Enter Right 
        private void dataGridViewFields_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridViewFields.Rows[e.RowIndex];

            WSeqNoRight = (int)rowSelected.Cells[0].Value;

            Om.ReadReadFileFieldsbySeqNo(WSeqNoRight);

            //Rff.ReadReadFileFieldsbySeqNo(WSeqNoRight);
           

            comboBoxSourceFieldNm.Text = Om.Source_Field_Nm;

            textBoxFieldSourceType.Text = WFieldSourceType; 

            textBoxTargetFieldNm.Text = Om.Target_Field_Nm;
            textBoxFrom.Text = Om.TargetFieldPositionStart.ToString();
            textBoxTo.Text = Om.TargetFieldPositionEnd.ToString();

            comboBoxFieldType.Text = Om.TargetFieldType;

            comboBoxPar916.Text = Om.TransformationRoutine;
            textBoxTargetFieldValue.Text = Om.TargetDefaultValue;

            buttonAddRight.Hide();
            buttonUpdateRight.Show();
            buttonDeleteRight.Show();

        }
        // ADD Left
        private void buttonAddLeft_Click(object sender, EventArgs e)
        {
           

            if (comboBoxOutputFile.Text == "Not_Defined" || comboBoxOutputVersion.Text == "Not_Defined" ||
                 comboBoxOutputFileType.Text == "Not_Defined" || textBoxFileNmMask.Text == "Not_Defined")
            {
                MessageBox.Show("Please Fill In All Necessary Information ");
                return;
            }

            Outf.ReadReconcOutputFilesByFileIdAndVersion(comboBoxOutputFile.Text, comboBoxOutputVersion.Text); 
          
            if (Outf.RecordFound == true)
            {
                MessageBox.Show("File Id Already exist");
                return;
            }

            if (textBoxTargetDir.Text == "")
            {
                MessageBox.Show("Please Fill In Table Name In DB ");
                return;
            }

            //WRowIndexLeft = dataGridViewFiles.SelectedRows[0].Index;

            Outf.ReadOutputFilesBySeqNo(WRowIndexLeft);

            Outf.OutputFile_Id = comboBoxOutputFile.Text;
            Outf.OutputFile_Version = comboBoxOutputVersion.Text;

            Outf.EffectiveDate = dateTimePickerEffectiveDt.Value;
            Outf.FileNameMask = textBoxFileNmMask.Text;

            Outf.LayoutType = comboBoxOutputFileType.Text;
            int TempNo;
            if (int.TryParse(txtLinesInHeader.Text, out TempNo))
            {
                Outf.LinesHeader = TempNo;
            }
            else
            {
                MessageBox.Show(txtLinesInHeader, "Please enter a valid number for header!");

                return;
            }
            if (int.TryParse(txtLinesInTrailer.Text, out TempNo))
            {
                Outf.LinesTrailer = TempNo;
            }
            else
            {
                MessageBox.Show(txtLinesInHeader, "Please enter a valid number for trailer!");

                return;
            }


            Outf.TargetDirectory = textBoxTargetDir.Text;

            Outf.SourceTableName = comboBoxSourceTbl.Text;

            Outf.ArchiveDirectory = textBoxArchiveDirectory.Text;

            Outf.ExceptionsDirectory = textBoxExceptionsDirectory.Text;

            Outf.Operator = WOperator;

            //
            // ADD
            //
            Outf.InsertReconcOutputFileRecord();

            Form502_Load(this, new EventArgs());

            MessageBox.Show("File with id: " + comboBoxOutputFile.Text + " Inserted");
            textBoxMsgBoard.Text = "File Inserted";

        }
        // Update Left
        private void buttonUpdateLeft_Click(object sender, EventArgs e)
        {
            if (textBoxTargetDir.Text == "") 
            {
                MessageBox.Show("Please Fill In Table Name In DB ");
                return;
            }

            WRowIndexLeft = dataGridViewFiles.SelectedRows[0].Index;

            Outf.ReadOutputFilesBySeqNo(WRowIndexLeft);

            Outf.OutputFile_Id = comboBoxOutputFile.Text ;
            Outf.OutputFile_Version = comboBoxOutputVersion.Text ;

            Outf.EffectiveDate = dateTimePickerEffectiveDt.Value ;
            Outf.FileNameMask=textBoxFileNmMask.Text ;

            Outf.LayoutType=comboBoxOutputFileType.Text ;
            int TempNo; 
            if (int.TryParse(txtLinesInHeader.Text, out TempNo))
            {
                Outf.LinesHeader = TempNo;
            }
            else
            {
                MessageBox.Show(txtLinesInHeader, "Please enter a valid number for header!");

                return;
            }
            if (int.TryParse(txtLinesInTrailer.Text, out TempNo))
            {
                Outf.LinesTrailer = TempNo;
            }
            else
            {
                MessageBox.Show(txtLinesInHeader, "Please enter a valid number for trailer!");

                return;
            }

            Outf.TargetDirectory=textBoxTargetDir.Text;

            Outf.SourceTableName=comboBoxSourceTbl.Text;

            Outf.ArchiveDirectory = textBoxArchiveDirectory.Text ;

            Outf.ExceptionsDirectory=textBoxExceptionsDirectory.Text;

            Outf.UpdateReconcOutputFileRecord(WSeqNoLeft);
          
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
                Outf.DeleteFileFieldRecord(WSeqNoLeft, Outf.OutputFile_Id);

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

            // Check if Target already exists 
            //
            Om.ReadTableFieldsByTargetField(Outf.OutputFile_Id, Outf.OutputFile_Version, textBoxTargetFieldNm.Text);
          //  Rff.ReadFileFieldsBySourceField(WOutputFileID, comboBoxSourceFieldNm.Text);

            if (Om.RecordFound == true)
            {
                MessageBox.Show("Target Field Already exist");
                return;
            }

            // Check if source already exists
            //
            //Rff.ReadTableFieldsByTargetField(WOutputFileID, textBoxTargetFieldNm.Text);
            Om.ReadFileFieldsBySourceField(Outf.OutputFile_Id, Outf.OutputFile_Version, comboBoxSourceFieldNm.Text);

            if (Om.RecordFound == true)
            {
                if (MessageBox.Show("Warning: Same SOURCE Field ALready Exist. Do you want to continue ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
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
                if (FromPos == 0)
                {
                    MessageBox.Show("Zero not allowed for FromPos");
                    return; 
                }
            }
            else
            {
                MessageBox.Show(comboBoxOutputVersion.Text, "Please enter a valid number in field Start. ");
                return;
            }

            if (int.TryParse(textBoxTo.Text, out ToPos))
            {
                if (ToPos == 0)
                {
                    MessageBox.Show("Zero not allowed for ToPos");
                    return;
                }
            }
            else
            {
                MessageBox.Show(textBoxTo.Text, "Please enter a valid number in field End. ");
                return;
            }

            // Check Valid Range 

           // string Result = Rff.ReadCheckWithinOtherRange(WOutputFileID, comboBoxSourceFieldNm.Text, FromPos, ToPos);
            string Result = Om.ReadCheckWithinOtherRange(Outf.OutputFile_Id, Outf.OutputFile_Version,  textBoxTargetFieldNm.Text, FromPos, ToPos);

            if (Result != "")
            {
                MessageBox.Show(Result);
                return;
            }

            Om.OutputFile_Id = Outf.OutputFile_Id;
            Om.OutputFile_Version = Outf.OutputFile_Version;

            Om.Source_Field_Nm = comboBoxSourceFieldNm.Text;
            Om.SourceFieldType = textBoxFieldSourceType.Text; 
            Om.Target_Field_Nm = textBoxTargetFieldNm.Text;
            Om.TargetFieldPositionStart = FromPos;
            Om.TargetFieldPositionEnd = ToPos;

            Om.TargetFieldType = comboBoxFieldType.Text;

            Om.TransformationRoutine = comboBoxPar916.Text;
            Om.TargetDefaultValue = textBoxTargetFieldValue.Text;
            Om.Operator = WOperator; 

            if (Om.Source_Field_Nm=="NotPresent")
            {
                if (Om.TargetDefaultValue == "")
                {
                    MessageBox.Show("If Source field is NotPresent then TargetDefault nust have value");
                    return; 
                }
            }

            Om.InsertFileFieldRecord(); 
           
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

            if (comboBoxSourceFieldNm.Text == "NotPresent")
            {
                NotPresent = true;
                //textBoxFrom.Text = "0";
                //textBoxTo.Text = "0";
            }
            else
            {
                NotPresent = false;
            }

            Om.ReadReadFileFieldsbySeqNo(WSeqNoRight);


            string OldSourceFieldNm = Om.Source_Field_Nm;
            string OldTargetFieldNm = Om.Target_Field_Nm;

            if (OldTargetFieldNm != textBoxTargetFieldNm.Text)
            {
                // New Target to be updated 
                //Rff.ReadFileFieldsBySourceField(WOutputFileID, comboBoxSourceFieldNm.Text);
                Om.ReadTableFieldsByTargetField(Outf.OutputFile_Id, Outf.OutputFile_Version, textBoxTargetFieldNm.Text);
                if (Om.RecordFound == true & NotPresent == false)
                {
                    MessageBox.Show("Same Target Field Already exist");
                    return;
                }
            }

            if (OldSourceFieldNm != comboBoxSourceFieldNm.Text)
            {
                // New source  to be updated 
                Om.ReadFileFieldsBySourceField(Outf.OutputFile_Id, Outf.OutputFile_Version, comboBoxSourceFieldNm.Text);

                if (Om.RecordFound == true)
                {
                    if (MessageBox.Show("Warning: Same Source Field ALready Exist. Do you want to continue ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
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
                MessageBox.Show(comboBoxOutputVersion.Text, "Please enter a valid number in field Start. ");
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
            //string Result = Rff.ReadCheckWithinOtherRange(Rff.SourceFileId, OldSourceFieldNm, FromPos, ToPos);
          
            string Result = Om.ReadCheckWithinOtherRange(Om.OutputFile_Id, Om.OutputFile_Version, OldSourceFieldNm, FromPos, ToPos);

            if (Result != "" & NotPresent == false)
            {
                MessageBox.Show(Result);
                return;
            }

            // Read Again to position record 

            Om.ReadReadFileFieldsbySeqNo(WSeqNoRight);

            //Rff.ReadReadFileFieldsbySeqNo(WSeqNoRight);
            //Om.OutputFile_Id = Outf.OutputFile_Id;
            //Om.OutputFile_Version = Outf.OutputFile_Version;

            Om.Source_Field_Nm = comboBoxSourceFieldNm.Text;
            Om.SourceFieldType = textBoxFieldSourceType.Text;
            Om.Target_Field_Nm = textBoxTargetFieldNm.Text ;
            Om.TargetFieldPositionStart = FromPos;
            Om.TargetFieldPositionEnd = ToPos;

            Om.TargetFieldType = comboBoxFieldType.Text ;

            Om.TransformationRoutine = comboBoxPar916.Text ;
            Om.TargetDefaultValue = textBoxTargetFieldValue.Text ;

            Om.UpdateFileFieldRecord(WSeqNoRight);

            // UPDATE 
            //Rff.UpdateFileFieldRecord(WSeqNoRight);

            Form502_Load(this, new EventArgs());

            dataGridViewFiles.Rows[WRowIndexLeft].Selected = true;
            dataGridViewFiles_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

            dataGridViewFields.Rows[WRowIndexRight].Selected = true;
            dataGridViewFields_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexRight));

        }
        // Delete Right 
        private void buttonDeleteRight_Click(object sender, EventArgs e)
        {

          

            if (MessageBox.Show("Warning: Do you want to delete this field ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == DialogResult.Yes)
            {
                Om.DeleteFileFieldRecord(WSeqNoRight);

                Form502_Load(this, new EventArgs());
            }
            else
            {
                return;
            }

            dataGridViewFiles.Rows[WRowIndexLeft].Selected = true;
            dataGridViewFiles_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

        }



        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        // Make A new Entry Left 

        private void checkBoxMakeNewEntry_CheckedChanged(object sender, EventArgs e)
        {
            HandleNewFile();
        }

        // Show New for File
        private void HandleNewFile()
        {
            if (InternalChangeLeft == true)
            {
                InternalChangeLeft = false;
                return;
            }

            if (checkBoxMakeNewEntry.Checked == true)
            {
                //buttonAdd.Enabled = true;
                buttonAddLeft.Show();
                buttonUpdateLeft.Hide();
                buttonDeleteLeft.Hide();

                //927
                comboBoxOutputFile.Text = "Not_Defined";

                //928
                comboBoxOutputVersion.Text = "Not_Defined";

                string ParId = "927";
                string OccurId = "4";
                Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

                textBoxTargetDir.Text = Gp.OccuranceNm + comboBoxOutputFile.Text;
                // Occurance 5
                ParId = "920";
                OccurId = "5";
                Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

                textBoxArchiveDirectory.Text = Gp.OccuranceNm + comboBoxOutputFile.Text;

                // Occurance 6
                ParId = "920";
                OccurId = "6";
                Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

                textBoxExceptionsDirectory.Text = Gp.OccuranceNm ;

                textBoxFileNmMask.Text = "Not_Defined";
                //920
                comboBoxOutputFileType.Text = "Not_Defined";
                textBoxTargetDir.Text = comboBoxOutputFile.Text;

                // READ 
                ParId = "927";
                //OccurId = "2";
                Gp.ReadParametersSpecificNm(WOperator, ParId, comboBoxOutputFile.Text);

                comboBoxSourceTbl.Text = Gp.GetParamOccurancesRelatedNm(WOperator, Gp.RelatedParmId, Gp.RelatedOccuranceId);

                comboBoxOutputFile.Enabled = true;
                comboBoxOutputVersion.Enabled = true;
                comboBoxOutputFileType.Enabled = true;

                comboBoxSourceTbl.Enabled = true;

                InternalChangeLeft = true;

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
        // New Field 
        private void checkBoxAddNewField_CheckedChanged(object sender, EventArgs e)
        {
            HandleNewRight(); 
        }
        // Show New for File
        private void HandleNewRight()
        {
            if (InternalChangeRight == true)
            {
                InternalChangeRight = false;
                return;
            }

            if (checkBoxAddNewField.Checked == true)
            {
                //buttonAdd.Enabled = true;
                buttonAddRight.Show();
                buttonUpdateRight.Hide();
                buttonDeleteRight.Hide();

                comboBoxSourceFieldNm.Text = "NotPresent" ;
                textBoxTargetFieldNm.Text = "Please Fill";

                textBoxFieldSourceType.Text = ""; 

                textBoxFrom.Text = "0";
                textBoxTo.Text = "0";

                textBoxTargetFieldValue.Text = ""; 

                InternalChangeRight = true;

                checkBoxAddNewField.Checked = false;

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
        private void comboBoxOutputFile_SelectedIndexChanged(object sender, EventArgs e)
        {
            string ParId = "920";
            string OccurId = "4";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            textBoxTargetDir.Text = Gp.OccuranceNm + comboBoxOutputFile.Text;

            ParId = "920";
            OccurId = "5";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            textBoxArchiveDirectory.Text = Gp.OccuranceNm + comboBoxOutputFile.Text;

            // Occurance 3
            ParId = "920";
            OccurId = "6";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            textBoxExceptionsDirectory.Text = Gp.OccuranceNm;

          
        }

        // Input Field Nm and find Datatype, and else  
        private void FindDataTypeOfThefield(string InFieldNm)
        {
            // Presuming the DataTable has a column named Date.
            string expression;
            expression = "FieldNm ='" + InFieldNm + "'";

            DataRow[] LayoutR = null;

            // Use the Select method to find all rows matching the filter.
            LayoutR = Mpa.OutputFieldsDataTable.Select(expression);

            // string Zero = LayoutR[0]["SeqNo"].ToString();
            string Zero = LayoutR[0]["column_id"].ToString();
            string TypeNo = LayoutR[0]["system_type_id"].ToString(); // eg gives number 106 which decimal
            string precision = LayoutR[0]["precision"].ToString(); // eg gives number 18 for the decimal 
            string scale = LayoutR[0]["scale"].ToString(); // eg Gives Decimal points 2 

            if (TypeNo == "231") WFieldSourceType = "Character";
            if (TypeNo == "56") WFieldSourceType = "Numeric";
            //if (TypeNo == "106") WFieldSourceType = "Decimal/Precision - "+scale+" Decimals" ;
            if (TypeNo == "106") WFieldSourceType = "Decimal";
            if (TypeNo == "61") WFieldSourceType = "DateTime";
            if (TypeNo == "104") WFieldSourceType = "Boolean";

        }

        // Refresh 
        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            WRowIndex = dataGridViewFiles.SelectedRows[0].Index;

            Form502_Load(this, new EventArgs());

            dataGridViewFiles.Rows[WRowIndex].Selected = true;
            dataGridViewFiles_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));
        }
// If Change reconstruct 
        private void comboBoxSourceFieldNm_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxSourceFieldNm.Text != "NotPresent")
            {
                FindDataTypeOfThefield(comboBoxSourceFieldNm.Text);
                textBoxFieldSourceType.Text = WFieldSourceType;
            }
            else
            {
                textBoxFieldSourceType.Text = "";
            }
         
        }
// Print 
        private void buttonPrint_Click(object sender, EventArgs e)
        {
            string P1 = "Output File Fields Mapping";

            string P2 = Outf.OutputFile_Id;
            string P3 = Outf.OutputFile_Version;
            string P4 = WOperator;
            string P5 = WSignedId;

            Form56R_Out_Mapping ReportATMS_Out_Mapping = new Form56R_Out_Mapping(P1, P2, P3, P4, P5);
            ReportATMS_Out_Mapping.Show();
        }
    }
}
