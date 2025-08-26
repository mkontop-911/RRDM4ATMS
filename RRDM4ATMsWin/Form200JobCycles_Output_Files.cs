using System;
using System.Windows.Forms;
using System.Drawing;
using RRDM4ATMs;
using System.Configuration;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Text;


namespace RRDM4ATMsWin
{
    public partial class Form200JobCycles_Output_Files : Form
    {
        // Variables

        Form54 NForm54;
        Form55 NForm55;
        
        public bool Prive;

        //int WAction;

        //int WJobCycleNo;

        //string WCategoryId; 

        string MsgFilter;

        RRDMControllerMsgClass Cm = new RRDMControllerMsgClass();

        // NEW FORM

        RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

        RRDMOutputFileDefinition Outf = new RRDMOutputFileDefinition();

        RRDMOutputFileFieldsMappingDefinition Om = new RRDMOutputFileFieldsMappingDefinition();

        RRDMOutputFileMonitorLog Olog = new RRDMOutputFileMonitorLog();

        RRDMGasParameters Gp = new RRDMGasParameters();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        DateTime NullFutureDate = new DateTime(2050, 11, 21);

        //string WSelectionCriteria;

        string WSignedId;
        int WSignRecordNo;
        string WSecLevel;
        string WOperator;

        string WMatchingRunningGroup;

        // Methods 
        // READ ATMs Main
        // 
        public Form200JobCycles_Output_Files(string InSignedId, int SignRecordNo, string InSecLevel, string InOperator, string InMatchingRunningGroup)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;

            WMatchingRunningGroup = InMatchingRunningGroup;

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
            UserId.Text = InSignedId;

            //*****************************************
            //
            //*****************************************

            textBoxMsgBoard.Text = "Job Cycles ";

            // ....

            MsgFilter =
                  "(ReadMsg = 0 AND ToAllAtms = 1)"
              + " OR (ReadMsg = 0 AND ToUser='" + WSignedId + "')";


            Cm.ReadControlerMSGsSerious(MsgFilter);

            if (Cm.SerMsgCount > 0)
            {
                string messagesStatus = " You have " + Cm.SerMsgCount.ToString() + " personal messages from the controller.";

                toolTipMessages.SetToolTip(buttonMsgs, messagesStatus);
                toolTipMessages.ShowAlways = true;

            }
            else
            {
                toolTipMessages.SetToolTip(buttonMsgs, "Check messages from controller.");
                toolTipMessages.ShowAlways = true;
            }

            toolTipController.SetToolTip(buttonCommController, "Communicate with today's controller.");

          //  WJobCycleNo = 0; 

        }
        string WJobCategory = "ATMs";
        int WReconcCycleNo;
        // Load
   
        private void Form200JobCycles_Output_Files_Load(object sender, EventArgs e)
        {

            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

            if (Rjc.RecordFound == true)
            {
                label4.Show();

                dateTimePickerCurrentCutOff.Show();
                dateTimePickerCurrentCutOff.Value = Rjc.Cut_Off_Date.Date;

                textBox2.Text = WReconcCycleNo.ToString();
                label12.Text = Rjc.Cut_Off_Date.Date.ToShortDateString();
            }
            else
            {
                label4.Hide();
                dateTimePickerCurrentCutOff.Hide();
            }

            // Source Files (Grid-ONE)
            string Filter1 = " WHERE Operator = '" + WOperator + "' ";

            Outf.ReadReconcOutputFilesToFillDataTable(Filter1);

            //Rs.ReadReconcSourceFilesToFillDataTable(Filter1);

            if (Outf.OutputFilesDataTable.Rows.Count == 0)
            {
                MessageBox.Show("No Records to show");
            }
            else
            {
                dataGridViewFileIds.DataSource = Outf.OutputFilesDataTable.DefaultView;

                //dataGridViewFiles.Sort(dataGridViewFiles.Columns[0], ListSortDirection.Ascending);

                dataGridViewFileIds.Columns[0].Width = 60; // Id
                dataGridViewFileIds.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridViewFileIds.Columns[1].Width = 170; // Id
                dataGridViewFileIds.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridViewFileIds.Columns[2].Width = 100; // Name
                dataGridViewFileIds.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }

        }

        int SeqNoGrid2;
        string WOutputFile_Id ;
        string WOutputFile_Version ;
        string WTargetDirectory;
        string WArchiveDirectory; 
        // On ROW ENTER 
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridViewFileIds.Rows[e.RowIndex];

            label5.Hide();
            panel2.Hide();

            SeqNoGrid2 = (int)rowSelected.Cells[0].Value;

            Outf.ReadOutputFilesBySeqNo(SeqNoGrid2);

            WOutputFile_Id = Outf.OutputFile_Id;
            WOutputFile_Version = Outf.OutputFile_Version;

            WTargetDirectory = Outf.TargetDirectory;

            WArchiveDirectory = Outf.ArchiveDirectory;

            Olog.ReadDataTableFileMonitorLogByFileIdAndVersion(Outf.OutputFile_Id, Outf.OutputFile_Version);

           
            //C:\BankOutput\FilePool\123_Network
            //C:\BankOutput\Archives\123_Network

            ShowGrid3();

        }

        // ON ROW ENTER THIRD GRID
        int SeqNoGrid3;
        private void dataGridView3_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridViewFileOccurances.Rows[e.RowIndex];
            SeqNoGrid3 = (int)rowSelected.Cells[0].Value;

        }



        // Show Grid OutPut Files Occurances
     
        private void ShowGrid3()
        {  
        
            dataGridViewFileOccurances.DataSource = Olog.DataTableFileMonitorLog;

            if (dataGridViewFileOccurances.Rows.Count == 0)
            {
                return;
            }
            else
            {
                
            }

       

            dataGridViewFileOccurances.Columns[0].Width = 110; // SeqNo
            dataGridViewFileOccurances.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewFileOccurances.Columns[0].Visible = false;

            dataGridViewFileOccurances.Columns[1].Width = 70; // RMCycleNo
            dataGridViewFileOccurances.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
       

            dataGridViewFileOccurances.Columns[2].Width = 80; // SystemOfOrigin
            dataGridViewFileOccurances.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewFileOccurances.Columns[2].Visible = false;

            dataGridViewFileOccurances.Columns[3].Width = 80; // OutputFile_Id
            dataGridViewFileOccurances.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //     dataGridViewFileOccurances.Columns[3].HeaderText = "Cycle " + Rc.Cycle2;
            dataGridViewFileOccurances.Columns[3].Visible = true;

            dataGridViewFileOccurances.Columns[4].Width = 80; // OutputFile_Version
            dataGridViewFileOccurances.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewFileOccurances.Columns[4].Visible = false;

            dataGridViewFileOccurances.Columns[5].Width = 110; // StatusVerbose
            dataGridViewFileOccurances.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
          
            dataGridViewFileOccurances.Columns[6].Width = 200; // FileName
            dataGridViewFileOccurances.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewFileOccurances.Columns[7].Width = 80; // FileSize
            dataGridViewFileOccurances.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewFileOccurances.Columns[7].Visible = false;

            dataGridViewFileOccurances.Columns[8].Width = 100; // DateTimeCreated
            dataGridViewFileOccurances.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        

        }

        // ADD A NEW CYCLE 

        int OldReconcJobCycle;

        // Message from Controller 
        private void buttonMsgs_Click_1(object sender, EventArgs e)
        {
            NForm55 = new Form55(MsgFilter, WSignedId);
            NForm55.ShowDialog();

            MsgFilter =
                 "(ReadMsg = 0 AND ToAllAtms = 1)"
             + " OR (ReadMsg = 0 AND ToUser='" + WSignedId + "')";


            Cm.ReadControlerMSGsSerious(MsgFilter);

            if (Cm.SerMsgCount > 0)
            {
                string messagesStatus = " You have " + Cm.SerMsgCount.ToString() + " personal messages from the controller.";

                toolTipMessages.SetToolTip(buttonMsgs, messagesStatus);
                toolTipMessages.ShowAlways = true;
            }
            else
            {
                toolTipMessages.SetToolTip(buttonMsgs, "Check messages from controller.");
                toolTipMessages.ShowAlways = true;
            }
        }
        // Todays Controller 
        private void buttonCommController_Click_1(object sender, EventArgs e)
        {
            NForm54 = new Form54(WSignedId, WSignRecordNo, WOperator);
            NForm54.ShowDialog();
        }
        //Finish 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        // SHOW 
        DataTable TableFirstLines = new DataTable();
        private void button1_Click(object sender, EventArgs e)
        {
            ShowORCreateFile("Show");

            dataGridViewFileRecords.DataSource = TableFirstLines.DefaultView;

            if (dataGridViewFileRecords.Rows.Count == 0)
            {
                label5.Hide();
                panel2.Hide();
                MessageBox.Show("No records to show"); 
                return;
            }
            else
            {
                label5.Show();
                panel2.Show();
                label2.Text = "Number of Records ...." + dataGridViewFileRecords.Rows.Count.ToString();
                label3.Text = "File to be created .." + fileName_Out_Pool; ;

                dataGridViewFileRecords.Columns[0].Width = 500; // 
            }
          
        }
 
        // Create File 
        private void buttonCreateFile_Click_1(object sender, EventArgs e)
        {
            ShowORCreateFile("Create");
        }

     

        // SHOW OR CREATE FILE
      //  bool Show = false;
        bool Create = false;
        string fileName_Out_Pool;
        string fileName_Out_Archive;
        int WLinesCount;
        DataTable TableSelectedRecords = new DataTable();
        private void ShowORCreateFile(string InRequest)
        {
            if (InRequest == "Show")
            {
               // Show = true;
                Create = false; 
            }
            if (InRequest == "Create")
            {
                Create = true;
               // Show = false;     
            }
            RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
            RRDMOutputFileFieldsMappingDefinition Om = new RRDMOutputFileFieldsMappingDefinition();

            try
            {

                TableSelectedRecords = new DataTable();
                TableSelectedRecords.Clear();

                TableFirstLines = new DataTable();
                TableFirstLines.Clear();

                TableFirstLines.Columns.Add("Lines To Be Created In Output File", typeof(string));

                // Fill Table 
                string SelectionCriteria = ""; 
           
                {
                    SelectionCriteria = " WHERE Operator ='" + WOperator + "'"
                                        + " AND MatchingAtRMCycle =" + WReconcCycleNo
                                        + " AND IsMatchingDone = 1 AND FastTrack = 0 "
                                        + " AND Matched = 1 AND ActionType != '07' ";
                }
                if (checkBoxUnmatched_UnChecked.Checked == true)
                {
                    SelectionCriteria = " WHERE Operator ='" + WOperator + "'"
                                        + " AND MatchingAtRMCycle =" + WReconcCycleNo
                                        + " AND IsMatchingDone = 1 AND FastTrack = 0 "
                                        + " AND Matched = 0 AND ActionType != '07' ";
                }
                if (checkBoxUnMatched_Checked.Checked == true)
                {
                    SelectionCriteria = " WHERE Operator ='" + WOperator + "'"
                                        + " AND MatchingAtRMCycle =" + WReconcCycleNo
                                        + " AND IsMatchingDone = 1 AND FastTrack = 0 "
                                        + " AND Matched = 0 AND ActionType != '07' ";
                }
                if (checkBoxUnMatched_Checked.Checked == true)
                {
                    Mpa.ReadTablePoolDataToGetTableByCriteria(SelectionCriteria, 1);
                }
                else
                {
                    Mpa.ReadTablePoolDataToGetTableByCriteria(SelectionCriteria, 2);
                }
                   
                //MpaTable
                TableSelectedRecords = Mpa.MpaTable;
                WLinesCount = TableSelectedRecords.Rows.Count; 

                SelectionCriteria = " WHERE OutputFile_Id ='" + WOutputFile_Id + "'"
                                          + " AND OutputFile_Version ='" + WOutputFile_Version + "'";
                Om.ReadOutputFileFieldsDefinitionToFillDataTable("");
                int MaxLength = 0; 
                int K = 0;
                while (K <= (Om.DataTableFileFields.Rows.Count - 1))
                {                    
                    MaxLength = (int)Om.DataTableFileFields.Rows[K]["TargetFieldPositionEnd"];         
                    K++; 
                }
                    //DataTableFileFields
                    string[] lineArrayOut = new string[TableSelectedRecords.Rows.Count];

                int I = 0;

                while (I <= (TableSelectedRecords.Rows.Count - 1))
                {

                    string newLine = new String(' ', MaxLength);
                    K = 0;
                    while (K <= (Om.DataTableFileFields.Rows.Count - 1))
                    {
                        string s1 = newLine;
                        string OutputFile_Id = (string)Om.DataTableFileFields.Rows[K]["OutputFile_Id"];
                        string OutputFile_Version = (string)Om.DataTableFileFields.Rows[K]["OutputFile_Version"];
                        string Source_Field_Nm = (string)Om.DataTableFileFields.Rows[K]["Source_Field_Nm"];
                        string SourceFieldType = (string)Om.DataTableFileFields.Rows[K]["SourceFieldType"];
                        string Target_Field_Nm = (string)Om.DataTableFileFields.Rows[K]["Target_Field_Nm"];
                        string TargetFieldType = (string)Om.DataTableFileFields.Rows[K]["TargetFieldType"];
                        int PosStart = (int)Om.DataTableFileFields.Rows[K]["TargetFieldPositionStart"];
                        int PosEnd = (int)Om.DataTableFileFields.Rows[K]["TargetFieldPositionEnd"];
                        string TargetDefaultValue = (string)Om.DataTableFileFields.Rows[K]["TargetDefaultValue"];
                        string TransformationRoutine = (string)Om.DataTableFileFields.Rows[K]["TransformationRoutine"];
                        string Operator = (string)Om.DataTableFileFields.Rows[K]["Operator"];
                        // 

                        string ValueOriginString;
                        int ValueOriginInt;
                        decimal ValueOriginDecimal;
                        DateTime ValueOriginDtTm;
                        bool ValueOriginBoolean;
                        string ValueTarget = "";
                        if (Source_Field_Nm != "NotPresent")
                        {
                            if (SourceFieldType == "Character")
                            {
                                ValueOriginString = (string)Mpa.MpaTable.Rows[I][Source_Field_Nm];
                                ValueTarget = ValueOriginString;
                                PosStart = PosStart;
                            }
                            if (SourceFieldType == "Numeric")
                            {
                                ValueOriginInt = (int)Mpa.MpaTable.Rows[I][Source_Field_Nm];
                                ValueTarget = ValueOriginInt.ToString();
                                PosStart = (PosEnd - ValueTarget.Length) + 1;
                            }
                            if (SourceFieldType == "Decimal")
                            {
                                ValueOriginDecimal = (decimal)Mpa.MpaTable.Rows[I][Source_Field_Nm];
                                ValueTarget = ValueOriginDecimal.ToString();
                                PosStart = (PosEnd - ValueTarget.Length) + 1;
                            }
                            if (SourceFieldType == "DateTime")
                            {
                                if (TransformationRoutine == "Rtn_Default")
                                {
                                    ValueOriginDtTm = (DateTime)Mpa.MpaTable.Rows[I][Source_Field_Nm];
                                    ValueTarget = ValueOriginDtTm.ToString();
                                    PosStart = PosStart;
                                }
                                if (TransformationRoutine == "Rtn_NBG-Date")
                                {
                                    ValueOriginDtTm = (DateTime)Mpa.MpaTable.Rows[I][Source_Field_Nm];
                                    string TempValueTarget = ValueOriginDtTm.ToString();
                                    ValueTarget = TempValueTarget.Substring(0, 10);
                                    PosStart = PosStart;
                                }
                                if (TransformationRoutine == "Rtn_NBG-Time")
                                {
                                    ValueOriginDtTm = (DateTime)Mpa.MpaTable.Rows[I][Source_Field_Nm];
                                    string TempValueTarget = ValueOriginDtTm.ToString();
                                    ValueTarget = TempValueTarget.Substring(11, 8);
                                    PosStart = PosStart;
                                }

                            }
                            if (SourceFieldType == "Boolean")
                            {
                                ValueOriginBoolean = (bool)Mpa.MpaTable.Rows[I][Source_Field_Nm];
                                if (ValueOriginBoolean == true) ValueTarget = "YES";
                                if (ValueOriginBoolean == false) ValueTarget = "NO";
                                PosStart = PosStart;
                            }
                        }
                        else
                        {
                            ValueTarget = TargetDefaultValue;
                            PosStart = PosStart;
                        }
                     

                        // Apply Transformation Routine and find new 

                        newLine = s1.Insert(PosStart - 1, ValueTarget);

                        K++;
                    }


                    // Fill Table 

                    DataRow RowSelected = TableFirstLines.NewRow();

                    RowSelected["Lines To Be Created In Output File"] = newLine;
                    // ADD ROW
                    TableFirstLines.Rows.Add(RowSelected);

                    // Fill ARRAY
                    lineArrayOut[I] = newLine;

                    I++;

                }

                FileStream fs = null;

                DateTime WDate = DateTime.Now;

                string WDay = "";
                string WMonth = "";
                if (WDate.Day < 10)
                {
                    WDay = "0" + WDate.Day;
                }
                else WDay = WDate.Day.ToString();

                if (WDate.Month < 10)
                {
                    WMonth = "0" + WDate.Month;
                }
                else WMonth = WDate.Month.ToString();

                int Id1 = DateTime.Now.Hour;
                int Id2 = DateTime.Now.Minute;
                string Id3 = Id1.ToString() + "_" + Id2.ToString();
                string DateString = WDay + "." + WMonth + "." + WDate.Year; // eg ("17.11.2017")

                WTargetDirectory = Outf.TargetDirectory;

                WArchiveDirectory = Outf.ArchiveDirectory;

                //Olog.ReadDataTableFileMonitorLogByFileIdAndVersion(Outf.OutputFile_Id, Outf.OutputFile_Version);


                //C:\\BankOutput\\FilePool\\123_Network
                //C:\\BankOutput\\Archives\\123_Network
                //   Result.Substring(0, 2) == "OK"
              
                fileName_Out_Pool = WTargetDirectory + "\\BDC_File_123_Network" + "__" + " " + DateString + Id3 + ".Txt";
                fileName_Out_Archive = WArchiveDirectory + "\\BDC_File_123_Network" + "__"  + " " + DateString + Id3 + ".Txt";

                //  fileName_Out = "C:\\BankOutput\\FilePool\\123_Network\\BDC_File_123_Network" + "__" + Id3 + " " + DateString + ".Txt";

                if (Create == true)
                {
                    
                    File.WriteAllLines(fileName_Out_Pool, lineArrayOut, Encoding.GetEncoding(1253));
                    File.WriteAllLines(fileName_Out_Archive, lineArrayOut, Encoding.GetEncoding(1253));

                    InsertLogRec(1); 

                    MessageBox.Show("File Created!");

                }
                
            }
            catch (Exception ex)
            {
                InsertLogRec(0);

                CatchDetails(ex);

            }

        }

        // Insert Log Record

            private void InsertLogRec(int InStatus)
        {
            // Fill and insert 
            try
            {
                Olog.RMCycleNo = WReconcCycleNo;
                Olog.SystemOfOrigin = Outf.SourceTableName;
                Olog.OutputFile_Id = Outf.OutputFile_Id;
                Olog.OutputFile_Version = Outf.OutputFile_Version;
                if (InStatus == 1)
                {
                    Olog.StatusVerbose = "Inserted Succesfully";
                }
                else
                {
                    Olog.StatusVerbose = "Problem with Insertion of File";
                } 
                Olog.FileName = fileName_Out_Pool;
                Olog.FileSize = 1676;
                Olog.DateTimeCreated = DateTime.Now; ;

                Olog.FileHASH = "HjHk";
                Olog.LineCount = WLinesCount;
             
                Olog.DestinationPath = WTargetDirectory;
                Olog.ArchivedPath = WArchiveDirectory;
                Olog.Status = InStatus;// 1 Created OK 
                                 // 0 Problem with creation 
                Olog.InsertFileRecordLog();

                Olog.ReadDataTableFileMonitorLogByFileIdAndVersion(Outf.OutputFile_Id, Outf.OutputFile_Version);

                ShowGrid3();
            }
            catch (Exception ex)
            {

                CatchDetails(ex);

            }
           
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        //
        // Catch Details 
        //
        private static void CatchDetails(Exception ex)
        {
            RRDMLog4Net Log = new RRDMLog4Net();

            StringBuilder WParameters = new StringBuilder();

            WParameters.Append("User : ");
            WParameters.Append("NotAssignYet");
            WParameters.Append(Environment.NewLine);

            WParameters.Append("ATMNo : ");
            WParameters.Append("NotDefinedYet");
            WParameters.Append(Environment.NewLine);

            string Logger = "RRDM4Atms";
            string Parameters = WParameters.ToString();

            Log.CreateAndInsertRRDMLog4NetMessage(ex, Logger, Parameters);

            System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
                + " . Application will be aborted! Call controller to take care. ");

            //Environment.Exit(0);
        }
// Check if 
        private void textBox2_TextChanged(object sender, EventArgs e)
        {       
            //
            //  Validation
            //
            int WCycle; 
            if (int.TryParse(textBox2.Text, out WCycle))
            {
                if (WCycle == 0)
                {
                    MessageBox.Show("Zero not allowed for Cycle");
                    return;
                }
            }
            else
            {
                MessageBox.Show(textBox2.Text, "Please enter a valid number for Cycle. ");
                return;
            }

            Rjc.ReadReconcJobCyclesById(WOperator, WCycle); 
            if (Rjc.RecordFound == true)
            {
                textBox2.Text = WCycle.ToString();
                label12.Text = "Cut Off is.." + Rjc.Cut_Off_Date.Date.ToShortDateString();
            }
            WReconcCycleNo = WCycle; 
        }
    }
}
