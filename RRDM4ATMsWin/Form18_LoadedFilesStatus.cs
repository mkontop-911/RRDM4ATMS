using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

using System.Text;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

using System.Configuration;
// Alecos
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;

namespace RRDM4ATMsWin
{
    public partial class Form18_LoadedFilesStatus : Form
    {

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMReconcFileMonitorLog Flog = new RRDMReconcFileMonitorLog();

        RRDMAtmsClass Ac = new RRDMAtmsClass();

        RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();

        DataTable TableNotLoadedJournals = new DataTable();

        DateTime NullPastDate = new DateTime(1900, 01, 01);
        //DateTime WDateTime;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        int WJobCycleNo;
        DateTime WCut_Off_Date;
        int WMode;

        public Form18_LoadedFilesStatus(string InSignedId, int InSignRecordNo, string InOperator, int InJobCycleNo, DateTime InCut_Off_Date, int InMode)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
            WJobCycleNo = InJobCycleNo;
            WCut_Off_Date = InCut_Off_Date;

            WMode = InMode; // InMode = 12 all
                            // In Mode = 0 only the loaded with problems
                            // InMode = 15 Not Loaded Journals 
                            // InMode = 16 ATMs with Gaps in dates 

            InitializeComponent();

            // Set Working Date 

            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;

            labelUserId.Text = InSignedId;

            labelStep1.Text = "Loaded Files Status for Cut Off.. " + WCut_Off_Date.ToShortDateString();

            if (WMode == 0)
            {
                labelStep1.Text = "Journals with problems for Cut Off.. " + WCut_Off_Date.ToShortDateString();
             //   labelHeader.Text = "CASES WITH GAPS";

            }

            if (WMode == 16)
            {
                labelStep1.Text = "Gaps in Journals ";
                labelHeader.Text = "CASES WITH GAPS";
                
            }

            if (WMode == 15 )
            {
                labelArchivePath.Hide();
                textBoxArchivePath.Hide();
                buttonShowJournal.Hide();
                buttonPrintCutOff.Hide();
                buttonLoadedJournals.Hide();
                buttonMoveToInPool.Hide();
               
            }
            if ( WMode == 16)
            {
                labelArchivePath.Hide();
                textBoxArchivePath.Hide();
                buttonShowJournal.Hide();
                buttonPrintCutOff.Hide();
                buttonLoadedJournals.Hide();
                buttonMoveToInPool.Hide();
                textBoxstpErrorText.Hide();
                label3.Hide();
                buttonPrintCycle.Hide();
                buttonGapDates.Show();
            }
            else
            {
                buttonGapDates.Hide();
            }


            //buttonAddNew.Hide(); 

            //buttonAddNew.Show();

        }
        // Load 
        DateTime TempDate;

        private void Form502_Load(object sender, EventArgs e)
        {
            // 
            if (WMode == 12)
                Flog.ReadDataTableFileMonitorLogByCycleNo(WOperator, WSignedId, WJobCycleNo);

            if (WMode == 0)
                Flog.ReadDataTableFileMonitorLogByCycleNo_For_Not_Loaded(WOperator, WSignedId, WJobCycleNo);

            if (WMode == 12 || WMode == 0)
            {
                ShowGrid1();
            }

            if (WMode == 15 )
            {
                labelStep1.Text = "Journals not loaded during Cycle.." + WJobCycleNo.ToString();
                labelHeader.Text = "Journals for the below ATMs not loaded";

                TableNotLoadedJournals = new DataTable();
                TableNotLoadedJournals.Clear();

                // DATA TABLE ROWS DEFINITION 
                TableNotLoadedJournals.Columns.Add("AtmNo", typeof(string));
                TableNotLoadedJournals.Columns.Add("AtmsReconcGroup", typeof(string));
                TableNotLoadedJournals.Columns.Add("Model", typeof(string));
                TableNotLoadedJournals.Columns.Add("LastLoadedDate", typeof(string));
                TableNotLoadedJournals.Columns.Add("Comment", typeof(string));

                // READ ALL ATMS THAT EXISTS IN IST 
                // IST IS the base 

                Mgt.ReadTable_AndFindAll_Banks_Atms(WOperator, WCut_Off_Date);

                string AtmNo;
                int AtmsReconcGroup;
                string Model;
                bool NeverLoaded;
                bool NonLoadedThisCycle;
                bool Loaded;
                int I = 0;
                if (WMode == 15)
                {
                    I = 0;

                    while (I <= (Mgt.ATMsDataTable.Rows.Count - 1))
                    {
                        NeverLoaded = false;
                        Loaded = false;
                        NonLoadedThisCycle = false;
                        TempDate = NullPastDate;

                        AtmNo = (string)Mgt.ATMsDataTable.Rows[I]["AtmNo"];


                        Flog.ReadFileMonitorLogBy_ATM_DateOfFile(WOperator, WSignedId, WCut_Off_Date, AtmNo);

                        if (Flog.RecordFound == true)
                        {
                            // Then is OK 
                            Loaded = true;
                            // Read If missing sequence

                        }
                        else
                        {
                            // No record at all 
                            Flog.ReadFileMonitorLogBy_ATM(WOperator, WSignedId, AtmNo);
                            if (Flog.RecordFound == true)
                            {
                                NonLoadedThisCycle = true;
                            }
                            else
                            {
                                NeverLoaded = true;
                                Loaded = false;
                            }

                        }

                        if (NeverLoaded == true || NonLoadedThisCycle == true)
                        {
                            int Records;
                            //FILL TABLE 
                            DataRow RowSelected = TableNotLoadedJournals.NewRow();

                            RowSelected["Model"] = "";

                            RowSelected["AtmNo"] = AtmNo;

                            if (NeverLoaded == true)
                            {

                                Ac.ReadAtm(AtmNo);
                                if (Ac.RecordFound == true)
                                {
                                    Records = Mgt.ReadTable_AndFindNumberOfTxnsInIST(AtmNo);
                                    RowSelected["LastLoadedDate"] = "Never any journal loaded - Records:.." + Records.ToString();
                                    RowSelected["AtmsReconcGroup"] = Ac.AtmsReconcGroup.ToString();
                                    if (Ac.Model == null)
                                    {
                                        RowSelected["Model"] = "No Model";
                                    }
                                    else
                                    {
                                        RowSelected["Model"] = Ac.Model;
                                    }
                                }
                                else
                                {

                                    Records = Mgt.ReadTable_AndFindNumberOfTxnsInIST(AtmNo);
                                    RowSelected["LastLoadedDate"] = "Never any journal loaded - Records:.." + Records.ToString();
                                    RowSelected["AtmsReconcGroup"] = "Not Defined";
                                    if (Ac.Model == null)
                                    {
                                        RowSelected["Model"] = "No Model";
                                    }
                                    else
                                    {
                                        RowSelected["Model"] = Ac.Model;
                                    }
                                }
                                RowSelected["Comment"] = "Never Loaded";
                            }

                            if (NonLoadedThisCycle == true)
                            {
                                RowSelected["AtmsReconcGroup"] = Ac.AtmsReconcGroup.ToString();
                                if (Ac.Model == null)
                                {
                                    RowSelected["Model"] = "No Model";
                                }
                                else
                                {
                                    RowSelected["Model"] = Ac.Model;
                                }

                                RowSelected["LastLoadedDate"] = "Not Available";
                                RowSelected["Comment"] = "";
                            }
                            // ADD ROW
                            TableNotLoadedJournals.Rows.Add(RowSelected);
                        }

                        I++; // Read Next entry of the table 

                    }

                    textBoxNumberShown.Text = TableNotLoadedJournals.Rows.Count.ToString();
                    // At this point we have a table with all not loaded Journals

                    // Make a working table for the report

                    InsertWReport82(WSignedId);

                    dataGridView1.DataSource = TableNotLoadedJournals.DefaultView;


                    ShowGrid2();

                }
            }
            if (WMode == 16)
            {
                Flog.ReadFileMonitorLogBy_ATM_MissingSequence(WOperator, WSignedId, WCut_Off_Date);


                textBoxNumberShown.Text = Flog.DataTableAtmsWithGaps_1.Rows.Count.ToString();
                // At this point we have a table with all not loaded Journals

                // Make a working table for the report

                //InsertWReport82(WSignedId);
                dataGridView1.DataSource = Flog.DataTableAtmsWithGaps_1.DefaultView;

                ShowGrid3();

            }

        }

        // Insert 
        public void InsertWReport82(string InSignedId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ATMSConnectionString"].ConnectionString;

            if (TableNotLoadedJournals.Rows.Count > 0)
            {
                RRDMTempTablesForReportsATMS Tr = new RRDMTempTablesForReportsATMS();

                //Clear Table 
                Tr.DeleteReport82();

                // RECORDS READ AND PROCESSED 
                //TableMpa
                using (SqlConnection conn =
                               new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();

                        using (SqlBulkCopy s = new SqlBulkCopy(conn))
                        {
                            s.DestinationTableName = "[ATMS].[dbo].[WReport82]";

                            foreach (var column in TableNotLoadedJournals.Columns)
                                s.ColumnMappings.Add(column.ToString(), column.ToString());

                            s.WriteToServer(TableNotLoadedJournals);
                        }

                    }
                    catch (Exception ex)
                    {
                        conn.Close();

                        CatchDetails(ex);
                    }
            }
        }


        // Row Enter 
        int WSeqNo;
        int WFuid;
        string WAtmNo;
        string WFileName;
        string WExceptionPath;
        int WStatus;
        DateTime WPreviousFileDate;
        DateTime WCurrentFileDate; 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            if (WMode == 12 || WMode == 0)
            {
                WSeqNo = (int)rowSelected.Cells[0].Value;

                Flog.GetRecordBySeqNo(WSeqNo);

                WFileName = Flog.FileName;
                WExceptionPath = Flog.ExceptionPath;
                WStatus = Flog.Status;

                textBoxstpErrorText.Text = Flog.stpErrorText;

                //if (Flog.SystemOfOrigin == "ATMs")
                //{
                //    WJournalTxtFile = Flog.ArchivedPath;
                //    buttonJournal.Show();
                //}
                //else
                //{
                //    buttonJournal.Hide();
                //}
                //   FileName = Flog.FileName;



                textBoxArchivePath.Text = Flog.ArchivedPath;
                //    WPrefix = WOperator.Substring(0, 3);
                //Processed Successfully!00000563_20190101_EJ_NCR_BDC_2250.DAT
                //        WAtmNo = Flog.ArchivedPath.Substring(36, 8);

                WFuid = Flog.stpFuid;

                if (WFuid > 0)
                {
                    WAtmNo = Flog.StatusVerbose.Substring(24, 8);
                }
                else
                {
                    // WAtmNo = Flog.StatusVerbose.Substring(24, 8);
                }

                if (Flog.SystemOfOrigin == "ATMs")
                {
                    buttonShowJournal.Show();
                }
                else
                {
                    buttonShowJournal.Hide();
                }

            }

            if (WMode == 16 )
            {
                string ATMNo = (string)rowSelected.Cells[0].Value;
                //                +"  ATMNO  "
                //+ ", DateOfFile  " 1
                //+ ", CurFile AS CurrentFileDateJul   " 2
                //+ ", PrevFile AS PreviousFileDateJul  " 3
                //+ ", CurFile - PrevFile As DiffFile  " 4
                //+ ", ATMS.dbo.fnJulToDate(CurFile) AS CurrentFileDate  " 5
                //+ ", ATMS.dbo.fnJulToDate(PrevFile) AS PreviousFileDate  " 6

                WPreviousFileDate = (DateTime)rowSelected.Cells[5].Value;
                WCurrentFileDate = (DateTime)rowSelected.Cells[6].Value;
                RRDMAtmsClass Ac = new RRDMAtmsClass();
                Ac.ReadAtm(ATMNo);
                string Supplier = Ac.Supplier;
                textBoxArchivePath.Show();
                textBoxArchivePath.Text = "ATM Supplier is :.." + Ac.Supplier; 
            }
            }

            private void ShowGrid1()
        {

            dataGridView1.DataSource = Flog.DataTableFileMonitorLog.DefaultView;

            textBoxNumberShown.Text = Flog.DataTableFileMonitorLog.Rows.Count.ToString();

            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("No Records Exist For This Cycle!");
                this.Dispose();

                // return;
            }
            else
            {
                dataGridView1.Columns[0].Width = 60; // SeqNo
                dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[0].Visible = true;

                dataGridView1.Columns[1].Width = 70; // RMCycleNo
                dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[1].Visible = true;

                dataGridView1.Columns[2].Width = 90; // SystemOfOrigin
                dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[2].Visible = true;

                dataGridView1.Columns[3].Width = 150; // SourceFileID
                dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[3].Visible = true;

                dataGridView1.Columns[4].Width = 500; //  StatusVerbose
                dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[4].Visible = true;

                dataGridView1.Columns[5].Width = 80; // FileName
                dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[5].Visible = false;

                dataGridView1.Columns[6].Width = 80; // FileSize
                dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[6].Visible = true;

                dataGridView1.Columns[7].Width = 80; // DateTimeReceived
                dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[7].Visible = true;

                dataGridView1.Columns[8].Width = 80; // DateExpected
                dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView1.Columns[9].Width = 80; // DateOfFile
                dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[9].Visible = true;

                dataGridView1.Columns[10].Width = 80; // FileHASH
                dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[10].Visible = false;

                dataGridView1.Columns[11].Width = 80; // LineCount
                dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[11].Visible = true;

                dataGridView1.Columns[12].Width = 80; // ArchivedPath
                dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[12].Visible = true;

                dataGridView1.Columns[13].Width = 80; // Status
                dataGridView1.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[13].Visible = true;
            }

        }

        private void ShowGrid2()
        {


            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("No Records Exist For This Cycle!");
                //this.Dispose();

                // return;
            }
            else
            {
//                +"  ATMNO  "
//+ ", DateOfFile  "
//+ ", CurFile AS CurrentFileDateJul   "
//+ ", PrevFile AS PreviousFileDateJul  "
//+ ", CurFile - PrevFile As DiffFile  "
//+ ", dbo.fnJulToDate(CurFile) AS CurrentFileDate  "
//+ ", dbo.fnJulToDate(PrevFile) AS PreviousFileDate  "
//+ ", DATEDIFF(day,dbo.fnJulToDate(CurFile), dbo.fnJulToDate(PrevFile)) AS DaysDiff  "
                dataGridView1.Columns[0].Width = 100; // AtmNo
                dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[0].Visible = true;

                dataGridView1.Columns[1].Width = 150; // AtmsReconcGroup
                dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[1].Visible = true;

                dataGridView1.Columns[2].Width = 150; // Model
                dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[2].Visible = true;

                dataGridView1.Columns[3].Width = 250; // LastLoadedDate
                dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[3].Visible = true;

            }

        }

        private void ShowGrid3()
        {


            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("No Records Exist For This Cycle!");
                //this.Dispose();

                // return;
            }
            else
            {
                //DataTableAtmsWithGaps_2.Columns.Add("AtmNo", typeof(string));
                //DataTableAtmsWithGaps_2.Columns.Add("PreviousDate", typeof(DateTime));
                //DataTableAtmsWithGaps_2.Columns.Add("CurrentDate", typeof(DateTime));
                //DataTableAtmsWithGaps_2.Columns.Add("Difference", typeof(int));
                //DataTableAtmsWithGaps_2.Columns.Add("LastDtTmInJournals", typeof(DateTime));
                //DataTableAtmsWithGaps_2.Columns.Add("NextDtTmInJournals", typeof(DateTime));
                //DataTableAtmsWithGaps_2.Columns.Add("RecordsInIST", typeof(int));
                //DataTableAtmsWithGaps_2.Columns.Add("Comment", typeof(string));

                //dataGridView1.Columns[0].Width = 60; // ATMNO
                //dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                //dataGridView1.Columns[0].Visible = true;

                //dataGridView1.Columns[1].Width = 80; //  CurrentDate
                //dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                //dataGridView1.Columns[1].Visible = true;

                //dataGridView1.Columns[2].Width = 80; // PreviousDate
                //dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                //dataGridView1.Columns[2].Visible = true;

                //dataGridView1.Columns[3].Width = 60; // Difference
                //dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                //dataGridView1.Columns[3].Visible = true;

                //dataGridView1.Columns[4].Width = 140; // LastDtTmInJournals
                //dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                //dataGridView1.Columns[4].Visible = true;

                //dataGridView1.Columns[5].Width = 140; // NextDtTmInJournals
                //dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                //dataGridView1.Columns[5].Visible = true;

                //dataGridView1.Columns[6].Width = 80; // RecordsInIST
                //dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                //dataGridView1.Columns[6].Visible = true;

                //dataGridView1.Columns[7].Width = 150; // Comment
                //dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                //dataGridView1.Columns[7].Visible = true;


            }

        }



        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        //
        // print Loading cycles
        //
        private void buttonPrintCycle_Click(object sender, EventArgs e)
        {
            if (WMode == 15)
            {
                string P1 = "NOT LOADED JOURNALS For Cycle " + WJobCycleNo.ToString();

                string P2 = "";
                string P3 = "Third Par";
                string P4 = WOperator;
                string P5 = WSignedId;
                Form56R82 ReportATMS82 = new Form56R82(P1, P2, P3, P4, P5);
                ReportATMS82.Show();
            }
            else
            {
                string P1 = "Loaded Files For Cycle " + WJobCycleNo.ToString();

                string P2 = "";
                string P3 = "Third Par";
                string P4 = WOperator;
                string P5 = WSignedId;
                Form56R72 ReportATMS72 = new Form56R72(P1, P2, P3, P4, P5);
                ReportATMS72.Show();
            }


        }
        // Journal Loading Status 
        private void buttonLoadedJournals_Click(object sender, EventArgs e)
        {
            Form200cATMs NForm200cATMs;

            NForm200cATMs = new Form200cATMs(WSignedId, WSignRecordNo, "5", WOperator, "");
            NForm200cATMs.ShowDialog();
        }

        // SHOW JOURNAL
        private void buttonShowJournal_Click(object sender, EventArgs e)
        {
            Form67_BDC NForm67_BDC;
            int Mode = 3;
            Mode = 3; // Specific Journal 
            if (WFuid > 0)
            {
                NForm67_BDC = new Form67_BDC(WSignedId, 0, WOperator, WFuid, "", WAtmNo, 0, 0, NullPastDate, NullPastDate, Mode);
                NForm67_BDC.ShowDialog();
            }
            else
            {
                MessageBox.Show("Not Able to show Journal");
            }

            //NForm67 = new Form67(WSignedId, WSignRecordNo, WOperator, WFuid, "", 0, 0, NullPastDate, NullPastDate, Mode);
            //NForm67.Show();

        }
        // Print Last traces 
        private void buttonPrintCutOff_Click(object sender, EventArgs e)
        {
            string P1 = "Cut Off Last Traces For Cycle " + WJobCycleNo.ToString();

            string P2 = WJobCycleNo.ToString();
            string P3 = "Third Par";
            string P4 = WOperator;
            string P5 = WSignedId;
            Form56R69ATMS_Last_Traces ReportATMS_Last_Traces = new Form56R69ATMS_Last_Traces(P1, P2, P3, P4, P5);
            ReportATMS_Last_Traces.Show();
        }
        // MOVE TO INPOOL 
        private Button printButton;
        private PrintDocument printDocument1 = new PrintDocument();
        private string stringToPrint;

        private void buttonMoveToInPool_Click(object sender, EventArgs e)
        {

            MessageBox.Show("This moves the journal automatically for Re-loading"
                + Environment.NewLine
                + "Now it is considered as not needed functionality"
                + Environment.NewLine
                + "It will be developed if proved that is needed"
                );
            return;

            //RRDMJournalReadTxns_Text_Class Ed = new RRDMJournalReadTxns_Text_Class();
            RRDM_EXCEL_AND_Directories Ed = new RRDM_EXCEL_AND_Directories();

            RRDMGasParameters Gp = new RRDMGasParameters();


            string ParId = "920";
            string OccurId = "7";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");



            string WorkingDirectory = Gp.OccuranceNm;

            WFileName = Flog.FileName;
            WExceptionPath = Flog.ExceptionPath;
            WStatus = Flog.Status;

            string CopiedFile;
            string WJournalTxtFile = "";
            CopiedFile = Ed.CopyFileFromOneDirectoryToAnother(WJournalTxtFile, WorkingDirectory);

            MessageBox.Show("New File is copied to working directory.." + Environment.NewLine
                              + CopiedFile);

            if (MessageBox.Show("Do you want to print the Journal form the source??" + Environment.NewLine
                               + CopiedFile + Environment.NewLine
                             + "?  "
                             , "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                      == DialogResult.Yes)
            {
                // YES Proceed
                ReadFile(CopiedFile);
                printDocument1.Print();

            }
            else
            {
                return;
            }
        }

        private void ReadFile(string InCopiedFile)
        {

            using (FileStream stream = new FileStream(InCopiedFile, FileMode.Open))
            using (StreamReader reader = new StreamReader(stream))
            {
                stringToPrint = reader.ReadToEnd();
            }
        }

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            int charactersOnPage = 0;
            int linesPerPage = 0;

            // Sets the value of charactersOnPage to the number of characters 
            // of stringToPrint that will fit within the bounds of the page.
            e.Graphics.MeasureString(stringToPrint, this.Font,
                e.MarginBounds.Size, StringFormat.GenericTypographic,
                out charactersOnPage, out linesPerPage);

            // Draws the string within the bounds of the page
            e.Graphics.DrawString(stringToPrint, this.Font, Brushes.Black,
                e.MarginBounds, StringFormat.GenericTypographic);

            // Remove the portion of the string that has been printed.
            stringToPrint = stringToPrint.Substring(charactersOnPage);

            // Check to see if more pages are to be printed.
            e.HasMorePages = (stringToPrint.Length > 0);
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

            if (Environment.UserInteractive)
            {
                System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
                    + " . Application will be aborted! Call controller to take care. ");
            }
        }
        // Export to Excel
        private void buttonExcel_Click(object sender, EventArgs e)
        {

            RRDM_EXCEL_AND_Directories XL = new RRDM_EXCEL_AND_Directories();

            // string ExcelPath = "C:\\_KONTO\\CreateXL\\Files_" + DateTime.Now + ".xls";
            string ExcelDATE = DateTime.Now.Year.ToString()
                       + DateTime.Now.Month.ToString()
                       + DateTime.Now.Day.ToString()
                       + "_"
                       + DateTime.Now.Hour.ToString()
                        + DateTime.Now.Minute.ToString()
                        + DateTime.Now.Second.ToString()
                        ;

            string ExcelPath;
            string WorkingDir;


            if (WMode == 15)
            {
                ExcelPath = "C:\\RRDM\\Working\\Non_Loaded_Journals_" + ExcelDATE + ".xls";
                WorkingDir = "C:\\RRDM\\Working\\";
                XL.ExportToExcel(TableNotLoadedJournals, WorkingDir, ExcelPath);
            }
            if (WMode == 16)
            {
                ExcelPath = "C:\\RRDM\\Working\\Gaps_In_Journals_" + ExcelDATE + ".xls";
                WorkingDir = "C:\\RRDM\\Working\\";
                XL.ExportToExcel(Flog.DataTableAtmsWithGaps_1, WorkingDir, ExcelPath);
            }

            if (WMode == 12)
            {
                ExcelPath = "C:\\RRDM\\Working\\Loaded_Journals_" + ExcelDATE + ".xls";
                WorkingDir = "C:\\RRDM\\Working\\";
                XL.ExportToExcel(Flog.DataTableFileMonitorLog, WorkingDir, ExcelPath);
            }
            if (WMode == 0)
            {
                ExcelPath = "C:\\RRDM\\Working\\Problem_Journals_" + ExcelDATE + ".xls";
                WorkingDir = "C:\\RRDM\\Working\\";
                XL.ExportToExcel(Flog.DataTableFileMonitorLog, WorkingDir, ExcelPath);
            }

        }
// EXPORT TO XML
        private void buttonXML_Click(object sender, EventArgs e)
        {
            if (WMode == 15)
            {
               
                string XMLDoc;

                //XMLDoc = ToXml(Ac.ATMsDetailsDataTable, 0);
                string sFileName = "C:\\RRDM\\Working\\TEXT.XML";
                StreamWriter outputFile = new StreamWriter(@sFileName);

                DataSet dS = new DataSet();
                dS.DataSetName = "RecordSet";
                dS.Tables.Add(TableNotLoadedJournals);
                //StringWriter sw = new StringWriter();
                dS.WriteXml(outputFile, XmlWriteMode.IgnoreSchema);

                MessageBox.Show("An XML File is created in RRDM working directory");
            }
            if (WMode == 16)
            {
              
                string XMLDoc;

                //XMLDoc = ToXml(Ac.ATMsDetailsDataTable, 0);
                string sFileName = "C:\\RRDM\\Working\\TEXT.XML";
                StreamWriter outputFile = new StreamWriter(@sFileName);

                DataSet dS = new DataSet();
                dS.DataSetName = "RecordSet";
                dS.Tables.Add(Flog.DataTableAtmsWithGaps_1);
                //StringWriter sw = new StringWriter();
                dS.WriteXml(outputFile, XmlWriteMode.IgnoreSchema);

                MessageBox.Show("An XML File is created in RRDM working directory");
            }

            if (WMode == 12)
            {
                
                string XMLDoc;

                //XMLDoc = ToXml(Ac.ATMsDetailsDataTable, 0);
                string sFileName = "C:\\RRDM\\Working\\TEXT.XML";
                StreamWriter outputFile = new StreamWriter(@sFileName);

                DataSet dS = new DataSet();
                dS.DataSetName = "RecordSet";
                dS.Tables.Add(Flog.DataTableFileMonitorLog);
                //StringWriter sw = new StringWriter();
                dS.WriteXml(outputFile, XmlWriteMode.IgnoreSchema);

                MessageBox.Show("An XML File is created in RRDM working directory");
            }
            if (WMode == 0)
            {
                string XMLDoc;

                //XMLDoc = ToXml(Ac.ATMsDetailsDataTable, 0);
                string sFileName = "C:\\RRDM\\Working\\TEXT.XML";
                StreamWriter outputFile = new StreamWriter(@sFileName);

                DataSet dS = new DataSet();
                dS.DataSetName = "RecordSet";
                dS.Tables.Add(Flog.DataTableFileMonitorLog);
                //StringWriter sw = new StringWriter();
                dS.WriteXml(outputFile, XmlWriteMode.IgnoreSchema);

                MessageBox.Show("An XML File is created in RRDM working directory");
            }
           
        }
    }
}

