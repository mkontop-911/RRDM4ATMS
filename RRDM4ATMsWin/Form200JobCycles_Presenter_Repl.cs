using System;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Threading;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data;
//using System.Data.OleDb;
using RRDM4ATMs;
using RRDMAgent_Classes;
using System.Globalization;


namespace RRDM4ATMsWin
{
    public partial class Form200JobCycles_Presenter_Repl : Form
    {
        // Variables

        Form54 NForm54;
        Form55 NForm55;
        
        public bool Prive;

        //int WAction;

        int WJobCycleNo;

        string WCategoryId;

        DateTime WCut_Off_Date; 

        string MsgFilter;

        RRDMBanks Ba = new RRDMBanks();

        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

        RRDMUsersAccessToAtms Ua = new RRDMUsersAccessToAtms();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMControllerMsgClass Cm = new RRDMControllerMsgClass();

        RRDMGasParameters Gp = new RRDMGasParameters();

        //RRDMMatchingCategoriesSessions Mcs = new RRDMMatchingCategoriesSessions();

        RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

        RRDMMatchingCategories Mc = new RRDMMatchingCategories();

        RRDMReconcCategories Rc = new RRDMReconcCategories(); 

        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();

        RRDMMatchingCategoriesVsSourcesFiles Msf = new RRDMMatchingCategoriesVsSourcesFiles();

        RRDMPerformanceTraceClass Pt = new RRDMPerformanceTraceClass();

        RRDMReconcFileMonitorLog Flog = new RRDMReconcFileMonitorLog();

        RRDMHolidays Ho = new RRDMHolidays();

        RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();


        DateTime NullPastDate = new DateTime(1900, 01, 01);

        DateTime NullFutureDate = new DateTime(2050, 11, 21);

        //string ProcessName;
        //string Message;
        //int Mode;

        DateTime WSesDtTimeStart;

        DateTime WSesDtTimeEnd;

        string WSignedId;
       
        string WSecLevel;
        string WOperator;
        string WAtmNo;
        int WReplCycle; 

        // Methods 
        // READ ATMs Main
        // 
        public Form200JobCycles_Presenter_Repl(string InOperator,string InSignedId,  
              string InAtmNo, int InReplCycle )
        {
            WSignedId = InSignedId;
           
            WOperator = InOperator;

            WAtmNo = InAtmNo;

            WReplCycle = InReplCycle; 

           // WMatchingRunningGroup = InMatchingRunningGroup;

            InitializeComponent();

            // Set Working Date 
          
            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;
            UserId.Text = InSignedId;

            //*****************************************
            //
            //*****************************************

            labelStep1.Text = "Management Of Presenter For Cycle.."+ WReplCycle.ToString(); 

            textBoxMsgBoard.Text = "Job Cycles ";

            // ....

            Ta.ReadSessionsStatusTraces(WAtmNo, WReplCycle);

           
            WSesDtTimeStart = Ta.SesDtTimeStart;
            WSesDtTimeEnd = Ta.SesDtTimeEnd;
            //Ta.SesDtTimeStart, Ta.SesDtTimeEnd

            panel2.Hide();
            label5.Hide(); 
        
            WJobCycleNo = 0; 

        }
        string WJobCategory = "ATMs";
        // Load
        private void Form200JobCycles_Load(object sender, EventArgs e)
        {

            WTableId = "Switch_IST_Txns";

            Mgt.ReadTableAndFillTableWithPresenter_For_ReplCycle(WTableId, WSignedId,
                                               WAtmNo, WSesDtTimeStart, WSesDtTimeEnd
                                               , 2);

           

            textBoxIST_112.Text = Mgt.CountAll_IST_112.ToString();
            textBoxNonPresen.Text = Mgt.CountAll_IST_Non_Presenter.ToString();
            textBoxCountPresenter.Text = Mgt.CountPresenter.ToString();

            ShowGrid2();

            //int WReconcCycleNo;
           
            //WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

            //if (Rjc.RecordFound == true)
            //{
            //    label4.Show();
                
            //    dateTimePickerCurrentCutOff.Show(); 
            //    dateTimePickerCurrentCutOff.Value = Rjc.Cut_Off_Date.Date;
            //}
            //else
            //{
            //    label4.Hide();
            //    dateTimePickerCurrentCutOff.Hide();
            //}

            //string SelectionCriteria = " WHERE Operator='" + WOperator + "' AND JobCategory ='ATMs'";
            //Rjc.ReadReconcJobCyclesFillTable(SelectionCriteria);

            //ShowGrid1(); 
          
        }

        // ROW ENTER FOR JOB CYCLE 
        string WLatestStatus;
        string WTableId; 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            //DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            //WJobCycleNo = (int)rowSelected.Cells[0].Value;

            //textBoxReconcCycle.Text = WJobCycleNo.ToString(); 

            //Flog.ReadDataTableFileMonitorLogByCycleNo_For_Not_Loaded(WOperator, WSignedId, WJobCycleNo);

            //Rjc.ReadReconcJobCyclesById(WOperator, WJobCycleNo);

            //WCut_Off_Date = Rjc.Cut_Off_Date; 

            //Rc.ReadReconcCategoriesForMatrix(WOperator, WJobCycleNo);

            //WLatestStatus = Rc.StatusCycle1;
            //// 
            ////*******
            ////
            //WTableId = "Switch_IST_Txns";

            //Mgt.ReadTableAndFillTableWithPresenter_For_ReplCycle(WTableId, WSignedId,
            //                                   WAtmNo, WSesDtTimeStart, WSesDtTimeEnd
            //                                   , 2);

            ////public void ReadTableAndFillTableWithPresenter_For_ReplCycle(string InTable,
            ////                      int InRMCycle, string InSignedId,
            ////                      string InTerminalId,
            ////                      DateTime InFromTransDate, DateTime InToTransDate, int In_DB_Mode)

            //textBoxIST_112.Text = Mgt.CountAll_IST_112.ToString();
            //textBoxNonPresen.Text = Mgt.CountAll_IST_Non_Presenter.ToString();
            //textBoxCountPresenter.Text = Mgt.CountPresenter.ToString();

            //ShowGrid2();

        }

        // Row Enter second grid
        int WUniqueRecordId = 0;
        int WSeqNo;
        
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            WSeqNo = (int)rowSelected.Cells[0].Value;

            bool WIsPresenter = (bool)rowSelected.Cells["IsPresenter"].Value;

            string WSelectionCriteria = " WHERE SeqNo="+ WSeqNo;

            Mgt.ReadTransSpecificFromBothTables_By_SelectionCriteria(WSelectionCriteria, WTableId);

            textBoxAtmNo.Text = Mgt.TerminalId;
            textBoxTraceNo.Text = Mgt.TraceNo.ToString(); 

            if (Mgt.ActionType == "112")
            {
                textBoxRecordStatus.Text = "True Presenter"; 
            }
            if (Mgt.ActionType == "113")
            {
                textBoxRecordStatus.Text = "Invalid 112";
            }
            if (Mgt.ActionType != "112" & Mgt.ActionType != "113")
            {
                textBoxRecordStatus.Text = "No Action Yet";
                buttonInvalidCase.Enabled = true;
                buttonMakeItPresenter.Enabled = true;
                buttonUndo.Enabled = false; 
            }
            else
            {
                //
                // Action taken
                //
                buttonInvalidCase.Enabled = false;
                buttonMakeItPresenter.Enabled = false;
                buttonUndo.Enabled = true;
            }

            if (WIsPresenter == true)
            {
                buttonMakeItPresenter.Enabled = false;
                buttonUndo.Enabled = true;
            }
            else
            {
                buttonMakeItPresenter.Enabled = true;
                buttonUndo.Enabled = false;
            }

            WSelectionCriteria = " WHERE TerminalId='" + Mgt.TerminalId + "'"
                                + " AND TraceNoWithNoEndZero =" + Mgt.TraceNo
                                + " AND TransAmount =" + Mgt.TransAmt
                                + " AND Card_Encrypted ='" + Mgt.Card_Encrypted + "'  AND TXNSRC= '1'  AND Origin = 'Our Atms' "
                               ;
            
            Mpa.ReadInPoolTransSpecificBySelectionCriteria(WSelectionCriteria, 2); 

            if (Mpa.RecordFound)
            {
                WUniqueRecordId = Mpa.UniqueRecordId;
            }
            else
            {
                WUniqueRecordId = 0; 
            }
        }

        // Show Grid1
        private void ShowGrid1()
        {

            dataGridView1.DataSource = Rjc.TableReconcJobCycles.DefaultView;
     
            dataGridView1.Columns[0].Width = 60; // JobCycle;
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[1].Width = 200; // JobCategory;
            dataGridView1.Columns[1].Visible = false;
            
            dataGridView1.Columns[2].Width = 120; // StartDateTm.ToString();
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 120; // FinishDateTm
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[4].Width = 120; //  Description
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[5].Width = 120; // "Status"
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }
        // Show Grid2
        private void ShowGrid2()
        {
            
            dataGridView2.DataSource = Mgt.DataTableAllFields.DefaultView;

            if (dataGridView2.Rows.Count == 0)
            {
                MessageBox.Show("Nothing to show! "); 
                this.Dispose(); 
                return;
            }
            else
            {
                

            }

            //DataTableAllFields.Columns.Add("SeqNo", typeof(int));
            //DataTableAllFields.Columns.Add("Done", typeof(string));
            //DataTableAllFields.Columns.Add("Amount", typeof(string));
            //DataTableAllFields.Columns.Add("DR/CR", typeof(string));

            //DataTableAllFields.Columns.Add("IsInMaster", typeof(bool));
            //DataTableAllFields.Columns.Add("IsInJournal", typeof(bool));
            //DataTableAllFields.Columns.Add("IsPresenter", typeof(bool));
            //DataTableAllFields.Columns.Add("MASK", typeof(bool));
            //DataTableAllFields.Columns.Add("TerminalId", typeof(string));
            //DataTableAllFields.Columns.Add("Descr", typeof(string));
            //DataTableAllFields.Columns.Add("Date", typeof(string));
            //DataTableAllFields.Columns.Add("Response", typeof(string));

            dataGridView2.Columns[0].Width = 60; // SeqNo;
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[1].Width = 60; // Done;
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[2].Width = 80; // Amount
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView2.Columns[3].Width = 60; // DR/CR;
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[4].Width = 60; // IsInMaster;
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[5].Width = 60; // IsInJournal;
            dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[6].Width = 60; // IsPresenter
            dataGridView2.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[7].Width = 60; // MASK
            dataGridView2.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[8].Width = 70; // TerminalId
            dataGridView2.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }

        // Show Grid RCS
        //int CountNotDone; 
      
        //// ADD A NEW CYCLE 

        //int OldReconcJobCycle;

       
        
        //Finish 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
       
       
        RRDMMatchingCategoriesVsSourcesFiles Mcf = new RRDMMatchingCategoriesVsSourcesFiles();
        // Show Matching Files 

      
        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void buttonViewFiles_Click(object sender, EventArgs e)
        {
           
        }

        // Presenter cases
        private void buttonPresenterCases_Click(object sender, EventArgs e)
        {
            string FileId = "Switch_IST_Txns";
            Form78d_FileRecords_IST_PRESENTER NForm78d_FileRecords_IST_PRESENTER;
            NForm78d_FileRecords_IST_PRESENTER = new Form78d_FileRecords_IST_PRESENTER(WOperator, WSignedId, FileId, WJobCycleNo
                                                                                 , WCut_Off_Date, 0);
            NForm78d_FileRecords_IST_PRESENTER.ShowDialog();
        }
        // source records 
        private void buttonSourceReords_Click(object sender, EventArgs e)
        {
            if (WUniqueRecordId == 0)
            {
                MessageBox.Show("Main record not found");
                return; 
            }
            
            Form78d_AllFiles NForm78d_AllFiles; // BASED ON TRACE NUMBER
            Form78d_AllFiles_BDC_3 NForm78d_AllFiles_BDC_3; // BASED ON DIFFERENT VARIABLES

            //switch (WOperator)
            //{
            //    case "CRBAGRAA":
            //        {
            //            // DEMO MODE

            //            NForm78d_AllFiles = new Form78d_AllFiles(WOperator, WSignedId, WUniqueRecordId);

            //            NForm78d_AllFiles.ShowDialog();

            //            break;
            //        }
            //    case "ETHNCY2N":
            //        {

            //            NForm78d_AllFiles = new Form78d_AllFiles(WOperator, WSignedId, WUniqueRecordId);

            //            NForm78d_AllFiles.ShowDialog();

            //            break;
            //        }
            //    case "BCAIEGCX":
            //        {
                        NForm78d_AllFiles_BDC_3 = new Form78d_AllFiles_BDC_3(WOperator, WSignedId, WUniqueRecordId, 1);

                        NForm78d_AllFiles_BDC_3.ShowDialog();
            //            break;
            //        }
            //}

        }

        // View Pending For Matching
        private void buttonOutstandings_Click_1(object sender, EventArgs e)
        {
            string WMatchingCateg ="";

            Form78d_AllFiles_Pending NForm78d_AllFiles_Pending;
            // textBoxFileId.Text
            int WMode = 2; //
                           // InMode = 1 : Not processed yet 
                           // InMode = 2 : Processed this Cycle

            NForm78d_AllFiles_Pending = new Form78d_AllFiles_Pending(WOperator, WSignedId, WMatchingCateg );
            NForm78d_AllFiles_Pending.Show();
           
        }
// Create file
        private void buttonCreateFile_Click(object sender, EventArgs e)
        {
            // Based on conditions read next 1 and fill a table line
            RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
            
            // read records of output fie and fill up table
            // Loop and create a line
            // Insert line in file  
        }
        // Not Settled transactions 
        private void buttonNotSettled_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("The function has been built but it works when in production");
            //return; 

            Form78d_FileRecords NForm78d_FileRecords;
            // textBoxFileId.Text
            int WMode = 5; //
                           // Not Action Taken by Maker
                           // 
            bool WCategoryOnly = false;

            string WTableId = "Atms_Journals_Txns";

            NForm78d_FileRecords = new Form78d_FileRecords(WOperator, WSignedId, WTableId, "", WJobCycleNo, "", WMode, WCategoryOnly);
            NForm78d_FileRecords.Show();
        }

// Based On IST 
        private void buttonBasedOn_IST_Click(object sender, EventArgs e)
        {
            //if (WUniqueRecordId == 0)
            //{
            //    MessageBox.Show("Main record not found");
            //    return;
            //}

            Form78d_AllFiles NForm78d_AllFiles; // BASED ON TRACE NUMBER
            Form78d_AllFiles_BDC_3 NForm78d_AllFiles_BDC_3; // BASED ON DIFFERENT VARIABLES

            //switch (WOperator)
            //{
            //    case "CRBAGRAA":
            //        {
            //            // DEMO MODE

            //            NForm78d_AllFiles = new Form78d_AllFiles(WOperator, WSignedId, WUniqueRecordId);

            //            NForm78d_AllFiles.ShowDialog();

            //            break;
            //        }
            //    case "ETHNCY2N":
            //        {

            //            NForm78d_AllFiles = new Form78d_AllFiles(WOperator, WSignedId, WUniqueRecordId);

            //            NForm78d_AllFiles.ShowDialog();

            //            break;
            //        }
            //    case "BCAIEGCX":
            //        {
                        int Mode = 2; 
                        NForm78d_AllFiles_BDC_3 = new Form78d_AllFiles_BDC_3(WOperator, WSignedId, WSeqNo, Mode);

                        NForm78d_AllFiles_BDC_3.ShowDialog();
            //            break;
            //        }
            //}

        }
// Make it presenter 
        private void buttonMakeItPresenter_Click(object sender, EventArgs e)
        {
            // 
            RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();
            RRDMAtmsClass Ac = new RRDMAtmsClass();

            if (WUniqueRecordId == 0)
            {
                MessageBox.Show("No Journal Record Available");
                return;
            }

            Mpa.ReadInPoolTransSpecificUniqueRecordId(WUniqueRecordId, 2);

            if (Mpa.RecordFound)
            {
                if (Mpa.MetaExceptionId == 55)
                {
                    MessageBox.Show("Record already define as presenter errror");
                    return;
                }
            }

            // Insert Error and update Master Exception Id and Number 
            string PresError = "PresenterError"; 
            string SuspectDesc = "";

            if (PresError == "PresenterError" || SuspectDesc == "SUSPECT FOUND")
            {
                // THERE IS ERROR 
                // INSERT ERROR

                if (PresError == "PresenterError")
                {
                    Ec.ErrId = 55;

                }
                if (SuspectDesc == "SUSPECT FOUND")
                {
                    Ec.ErrId = 225;
                }

                Ec.BankId = WOperator;
                Ec.ReadErrorsIDRecord(Ec.ErrId, WOperator); // READ TO GET THE CHARACTERISTICS

                Ac.ReadAtm(Mpa.TerminalId); 

                // INITIALISED WHAT IS NEEDED 

                Ec.CategoryId = Mpa.MatchingCateg;

                Ec.RMCycle = WJobCycleNo;
                Ec.UniqueRecordId = Mpa.UniqueRecordId;

                Ec.AtmNo = Mpa.TerminalId;
                Ec.SesNo = Mpa.SeqNo;
                Ec.DateInserted = DateTime.Now;
                Ec.DateTime = Mpa.TransDate;
                Ec.BranchId = Ac.Branch;
                Ec.ByWhom = WSignedId;

                Ec.CurDes = Mpa.TransCurr;
                Ec.ErrAmount = Mpa.TransAmount;

                Ec.TraceNo = Mpa.TraceNoWithNoEndZero;
                Ec.CardNo = Mpa.CardNumber;

                Ec.TransType = Mpa.TransType;
                Ec.TransDescr = Mpa.TransDescr;

                Ec.CustAccNo = Mpa.AccNumber;

                Ec.DatePrinted = NullPastDate;

                Ec.OpenErr = true;

                Ec.CitId = Ac.CitId;

                Ec.Operator = WOperator;

                int ErrorNoInserted = Ec.InsertError(); // INSERT ERROR     

                // Update Mpa
              
                Mpa.MetaExceptionNo = ErrorNoInserted;
                Mpa.MetaExceptionId = Ec.ErrId;
                Mpa.SettledRecord = false;

                Mpa.UpdateMatchingTxnsMasterPoolATMsFooter(WOperator, WUniqueRecordId, 2);

                RRDM_Copy_Txns_From_IST_ToMatched_And_Vice Ft = new RRDM_Copy_Txns_From_IST_ToMatched_And_Vice();
                Ft.MOVE_MATCHED_TXNS_TO_ITMX_tblMatchingTxnsMasterPoolATMs_Unique(WUniqueRecordId); 
                //// UPDATE AS DONE
                //int WSeqNo06 = 99; 
                //Mpa.UpdateMatchingTxnsMasterPoolATMs_SeqNo06(WUniqueRecordId, WSeqNo06, 2);

                // Update IST AS presenter
                string ActionType = "112";
                string PhysicalFiledID = "[RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns]";

                Mgt.UpdateRecordForISTSeqNumber(PhysicalFiledID, WSeqNo, ActionType);

                ActionType = "112";
                PhysicalFiledID = "[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[Switch_IST_Txns]";

                Mgt.UpdateRecordForISTSeqNumber(PhysicalFiledID, WSeqNo, ActionType);

                //int WRow1 = dataGridView1.SelectedRows[0].Index;

                //int scrollPosition1 = dataGridView1.FirstDisplayedScrollingRowIndex;

                //int WRow2 = dataGridView2.SelectedRows[0].Index;

                //int scrollPosition2 = dataGridView2.FirstDisplayedScrollingRowIndex;

                Form200JobCycles_Load(this, new EventArgs());
                // First
                //dataGridView1.Rows[WRow1].Selected = true;
                //dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow1));

                //dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition1;

                //// second

                //dataGridView2.Rows[WRow2].Selected = true;
                //dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRow2));

                //dataGridView2.FirstDisplayedScrollingRowIndex = scrollPosition2;

            }
        }

        private void buttonUndo_Click(object sender, EventArgs e)
        {
            RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();
            RRDMAtmsClass Ac = new RRDMAtmsClass();
            if (Mgt.ActionType== "112")
            {
                Mpa.ReadInPoolTransSpecificUniqueRecordId(WUniqueRecordId, 2);

                Ec.DeleteErrorRecordByErrNo(Mpa.MetaExceptionNo);

                Mpa.MetaExceptionNo = 0;
                Mpa.MetaExceptionId = 0;

                if (Mpa.MatchMask == "11" || Mpa.MatchMask == "111")
                {
                    Mpa.SettledRecord = true;
                }
                else
                {
                    Mpa.SettledRecord = false;
                }

                Mpa.UpdateMatchingTxnsMasterPoolATMsFooter(WOperator, WUniqueRecordId, 2);
            }
            

            // UPDATE AS NOT DONE
            // Update IST AS presenter
            string ActionType = "00";
            string PhysicalFiledID = "[RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns]";

            Mgt.UpdateRecordForISTSeqNumber(PhysicalFiledID, WSeqNo, ActionType);

            ActionType = "00";
            PhysicalFiledID = "[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[Switch_IST_Txns]";

            Mgt.UpdateRecordForISTSeqNumber(PhysicalFiledID, WSeqNo, ActionType);

            //int WRow1 = dataGridView1.SelectedRows[0].Index;

            //int scrollPosition1 = dataGridView1.FirstDisplayedScrollingRowIndex;

            //int WRow2 = dataGridView2.SelectedRows[0].Index;

            //int scrollPosition2 = dataGridView2.FirstDisplayedScrollingRowIndex;

            Form200JobCycles_Load(this, new EventArgs());

            // First
            //dataGridView1.Rows[WRow1].Selected = true;
            //dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow1));

            //dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition1;

            //// second

            //dataGridView2.Rows[WRow2].Selected = true;
            //    dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRow2));

            //    dataGridView2.FirstDisplayedScrollingRowIndex = scrollPosition2;
      

        }
// Make this case invalid
        private void buttonInvalidCase_Click(object sender, EventArgs e)
        {
            // Update IST with Actiontype 113 ... Invalid
            // FOR ITMX and MATCHED TXNS
            string ActionType = "113";
            string PhysicalFiledID = "[RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns]"; 

            Mgt.UpdateRecordForISTSeqNumber(PhysicalFiledID, WSeqNo, ActionType );

            ActionType = "113";
            PhysicalFiledID = "[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[Switch_IST_Txns]";

            Mgt.UpdateRecordForISTSeqNumber(PhysicalFiledID, WSeqNo, ActionType);


            int WRow = dataGridView2.SelectedRows[0].Index;

            int scrollPosition = dataGridView2.FirstDisplayedScrollingRowIndex;

            Form200JobCycles_Load(this, new EventArgs());

            dataGridView2.Rows[WRow].Selected = true;
            dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));

            dataGridView2.FirstDisplayedScrollingRowIndex = scrollPosition;

        }
    }
}
