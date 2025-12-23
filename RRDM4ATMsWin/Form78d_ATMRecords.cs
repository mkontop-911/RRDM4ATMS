using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.ComponentModel;

namespace RRDM4ATMsWin
{
    public partial class Form78d_ATMRecords : Form
    {
        //RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();
        
       // RRDMMatchingDiscrepancies Md = new RRDMMatchingDiscrepancies();

        RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();
        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
        RRDMRepl_SupervisorMode_Details Sm = new RRDMRepl_SupervisorMode_Details();

        RRDMJournalReadTxns_Text_Class RBal = new RRDMJournalReadTxns_Text_Class();

        RRDM_CIT_G4S_And_Bank_Repl_Entries G4 = new RRDM_CIT_G4S_And_Bank_Repl_Entries();


        // int SavedMode;
        //  bool InternalUse; 
        DateTime NullPastDate = new DateTime(1900, 01, 01);

        int WSeqNo;

        string WOperator;
        string WSignedId; 
        string WTableId;
        string WAtmNo;
        
        DateTime WDtFrom;
        DateTime WDtTo;
        int WSesNo;
        DateTime WCap_Date;
        int WMode; 

        public Form78d_ATMRecords(string InOperator, string InSignedId, string InTableId
                                          , string InAtmNo , DateTime InDtFrom, DateTime InDtTo
                                                     , int InSesNo, DateTime InCap_Date, int InMode)
        {
            WOperator = InOperator;
            WSignedId = InSignedId;
            WTableId = InTableId; // eg InTable == "Atms_Journals_Txns" OR IST 
            WAtmNo = InAtmNo;
            WDtFrom = InDtFrom;
            WDtTo = InDtTo;
            WSesNo = InSesNo;
            WCap_Date = InCap_Date;
            WMode = InMode;


            // In Mode = 1 = both debits and credits 
            // In Mode = 2 Only withdrawls  
            // In Mode = 3 Only Deposits
            // 11 : Gives from start of date and time of last replenishment till the end of last complete Cap_Date for DEBITS       
            // 12 : Gives from start of date and time of last replenishment till the end of last complete Cap_Date for Credits   

            //******************************************************************

            // 4: Gives from start of Cap_Date till WDtTo for Debits and Credits
            // 5 : Same as 4 but for debits only
            // 6 : Same as 4 but for credits only 
            // 7 : Gives Figures from SM
            // 8 : Gives ALerts for late replenishments

            // 14 : Gives NCR FOREX TXNS FOR A REPL CYCLE 
            // 15 : Gives NCR FOREX TXNS TOTALS FOR A REPL CYCLE 

            // 18 : Gives times bettween transactions in SWITCH bettween two days 

            // 20 : TXNS From [ATM_MT_Journals_AUDI].[dbo].[TransactionSummary] between two days (cycle dates )  

            // 21 : TXNS From [ATM_MT_Journals_AUDI].[dbo].[tblHstAtmTxnsROM] between two days (cycle dates )  

            // 24 : ALPHA Master Daily Reconciliation per ATM

            InitializeComponent();

            if ( WMode == 8)
            {
                labelWhatGrid.Text = "ALERTS FOR CIT FEEDING OF ATMS";
            }
            else
            {
                labelWhatGrid.Text = "RECORDS FOR.." + WTableId + " FROM.." + WDtFrom.ToString() + "..TO.." + WDtTo.ToString();
            }

            if (WMode == 14)
            {
                labelWhatGrid.Text = "RECORDS FOR FOREX." + " FROM.." + WDtFrom.ToString() + "..TO.." + WDtTo.ToString();

                labelDebits.Hide();
                textBoxTotalDebit.Hide();
                labelCredits.Hide();
                textBoxTotalCredit.Hide();
                buttonShowSM.Hide();
                buttonShowTerm.Hide();
                buttonRefresh.Hide();
                //buttonPrint.Hide();

            }
            if (WMode == 15)
            {
                labelWhatGrid.Text = "TOTALS FOR FOREX." + " FROM.." + WDtFrom.ToString() + "..TO.." + WDtTo.ToString();

                labelDebits.Hide();
                textBoxTotalDebit.Hide();
                labelCredits.Hide();
                textBoxTotalCredit.Hide();
                buttonShowSM.Hide();
                buttonShowTerm.Hide();
                buttonRefresh.Hide();
                buttonPrint.Hide();

            }

            if (WMode == 18)
            {
                labelWhatGrid.Text = "Analysis of times for Switch." + " FROM.." + WDtFrom.ToString() + "..TO.." + WDtTo.ToString();

                labelDebits.Hide();
                textBoxTotalDebit.Hide();
                labelCredits.Hide();
                textBoxTotalCredit.Hide();
                buttonShowSM.Hide();
                buttonShowTerm.Hide();
                buttonRefresh.Hide();
                buttonPrint.Hide();

            }

            if (WMode == 20)
            {
                labelWhatGrid.Text = "Transaction Balancing " + " FROM.." + WDtFrom.ToString() + "..TO.." + WDtTo.ToString();

                labelDebits.Hide();
                textBoxTotalDebit.Hide();
                labelCredits.Hide();
                textBoxTotalCredit.Hide();
                buttonShowSM.Hide();
                buttonShowTerm.Hide();
                buttonRefresh.Hide();
                buttonPrint.Hide();

            }

            if (WMode == 21)
            {
                labelWhatGrid.Text = "Transaction Orininal Errors " + " FROM.." + WDtFrom.ToString() + "..TO.." + WDtTo.ToString();

                labelDebits.Hide();
                textBoxTotalDebit.Hide();
                labelCredits.Hide();
                textBoxTotalCredit.Hide();
                buttonShowSM.Hide();
                buttonShowTerm.Hide();
                buttonRefresh.Hide();
                buttonPrint.Hide();

            }
        }

        private void Form78b_Load(object sender, EventArgs e)
        {


           // Mgt.ReadTableAndFillTable(WTableId, WAtmNo, WRMCycle, WMatchingCateg, WMode, WCategoryOnly);
           if (WMode == 1 )
            {
                Mgt.ReadTrans_TXNS_FromSpecificTableBetween_Dates_Short(WSignedId, WTableId, WAtmNo, WDtFrom, WDtTo, WMode,2);
                textBoxTotalDebit.Text = Mgt.TotalDebit.ToString("#,##0.00");
                textBoxTotalCredit.Text = Mgt.TotalCredit.ToString("#,##0.00");

                buttonShowSM.Hide();
                dataGridView1.DataSource = Mgt.DataTableAllFields.DefaultView;
                int rows = Mgt.DataTableAllFields.Rows.Count;
                textBoxRecords.Text = rows.ToString();
            }
            if (WMode == 2 || WMode == 3 )
            {
                Mgt.ReadTrans_TXNS_FromSpecificTableBetween_Dates_Short(WSignedId, WTableId, WAtmNo, WDtFrom, WDtTo, WMode, 2);

                if (WMode == 2)
                {
                    textBoxTotalDebit.Text = Mgt.TotalDebit.ToString("#,##0.00");
                }

                if (WMode == 3 )
                {
                    textBoxTotalCredit.Text = Mgt.TotalCredit.ToString("#,##0.00");
                }

                buttonShowSM.Hide();
                dataGridView1.DataSource = Mgt.DataTableAllFields.DefaultView;
                int rows = Mgt.DataTableAllFields.Rows.Count;
                textBoxRecords.Text = rows.ToString();
            }
        
            if (WMode == 11)
            {
                Mgt.ReadTrans_TXNS_FromSpecificTableBetween_Dates_Short(WSignedId, WTableId, WAtmNo, WDtFrom, WDtTo.Date, WMode, 2);
                textBoxTotalDebit.Text = Mgt.TotalDebit.ToString("#,##0.00");

                buttonShowSM.Hide();
                dataGridView1.DataSource = Mgt.DataTableAllFields.DefaultView;
                int rows = Mgt.DataTableAllFields.Rows.Count;
                textBoxRecords.Text = rows.ToString();
            }
            if (WMode == 12)
            {
                Mgt.ReadTrans_TXNS_FromSpecificTableBetween_Dates_Short(WSignedId, WTableId, WAtmNo, WDtFrom, WDtTo.Date, WMode, 2);
                textBoxTotalCredit.Text = Mgt.TotalCredit.ToString("#,##0.00");

                buttonShowSM.Hide();
                dataGridView1.DataSource = Mgt.DataTableAllFields.DefaultView;
                int rows = Mgt.DataTableAllFields.Rows.Count;
                textBoxRecords.Text = rows.ToString();
            }
            if (WMode == 5)
            {
                Mgt.ReadTrans_TXNS_FromSpecificTableForCurrent_Cap_Date_UpTo_Date_Time(WSignedId, WTableId, WAtmNo, WCap_Date
                    , WDtTo, WMode,2);
                buttonShowSM.Hide();
                dataGridView1.DataSource = Mgt.DataTableAllFields.DefaultView;
                int rows = Mgt.DataTableAllFields.Rows.Count;
                textBoxRecords.Text = rows.ToString();
            }
            if (WMode == 6)
            {
                Mgt.ReadTrans_TXNS_FromSpecificTableForCurrent_Cap_Date_UpTo_Date_Time(WSignedId, WTableId, WAtmNo, WCap_Date
                    , WDtTo, WMode,2);
                buttonShowSM.Hide();
                dataGridView1.DataSource = Mgt.DataTableAllFields.DefaultView;
                int rows = Mgt.DataTableAllFields.Rows.Count;
                textBoxRecords.Text = rows.ToString();
            }
            
            if (WMode == 7)
            {  
                Sm.ReadSMLineByReplCycleFillTable(WSignedId, WTableId, WAtmNo, WSesNo,2);
                dataGridView1.DataSource = Sm.DataTable_SM.DefaultView;
                int rows = Sm.DataTable_SM.Rows.Count;
                textBoxRecords.Text = rows.ToString();
                textBoxTotalDebit.Hide();
                textBoxTotalCredit.Hide();
                labelDebits.Hide();
                labelCredits.Hide();
                label2.Hide();
                textBoxTerm.Hide();
                buttonShowTerm.Hide();
                buttonShowSM.Show();
            }

            if (WMode == 8)
            {
               
                G4.ReadCIT_G4S_Repl_EntriesToFillDataTable_Feeding_Alerts(WOperator); 
                if (G4.DataTableG4SEntries.Rows.Count > 0)
                {
                    dataGridView1.DataSource = G4.DataTableG4SEntries.DefaultView;
                }
                    
                int rows = G4.DataTableG4SEntries.Rows.Count;
                textBoxRecords.Text = rows.ToString();
                textBoxTotalDebit.Hide();
                textBoxTotalCredit.Hide();
                labelDebits.Hide();
                labelCredits.Hide();
                label2.Hide();
                textBoxTerm.Hide();
                buttonShowTerm.Hide();
                buttonShowSM.Show();
            }

            if (WMode == 14)
            {
                Mgt.ReadTrans_TXNS_FromSpecificTableBetween_Dates_Short(WSignedId, WTableId, WAtmNo, WDtFrom, WDtTo, WMode, 2);
                //textBoxTotalDebit.Text = Mgt.TotalDebit.ToString("#,##0.00");
                //textBoxTotalCredit.Text = Mgt.TotalCredit.ToString("#,##0.00");

                //buttonShowSM.Hide();
                dataGridView1.DataSource = Mgt.DataTableAllFields.DefaultView;
                int rows = Mgt.DataTableAllFields.Rows.Count;
                textBoxRecords.Text = rows.ToString();
            }
            if (WMode == 15)
            {
                Mgt.ReadTrans_TXNS_FromSpecificTableBetween_Dates_Short(WSignedId, WTableId, WAtmNo, WDtFrom, WDtTo, WMode, 2);
                //textBoxTotalDebit.Text = Mgt.TotalDebit.ToString("#,##0.00");
                //textBoxTotalCredit.Text = Mgt.TotalCredit.ToString("#,##0.00");

                //buttonShowSM.Hide();
                dataGridView1.DataSource = Mgt.DataTableAllFields.DefaultView;
                int rows = Mgt.DataTableAllFields.Rows.Count;
                textBoxRecords.Text = rows.ToString();
            }

            if (WMode == 18)
            {
                RRDM_CIT_EXCEL_TO_BANK Ce = new RRDM_CIT_EXCEL_TO_BANK(); 
               
                Ce.ReadSWITCH_ShortTableToFindTimeSpaces(WSignedId, WTableId, WAtmNo, WDtFrom, WDtTo, WMode, 1);
                
                buttonShowSM.Hide();
                dataGridView1.DataSource = Ce.DataTableSwitchTimers.DefaultView;
                int rows = Ce.DataTableAllFields.Rows.Count;
                textBoxRecords.Text = rows.ToString();
            }

            if (WMode == 20)
            {
                
                RBal.ReadTXN_BALANCING(WAtmNo, WDtFrom, WDtTo); 

                buttonShowSM.Hide();
                dataGridView1.DataSource = RBal.DataTableTXN_Balancing.DefaultView;
                int rows = RBal.DataTableTXN_Balancing.Rows.Count;
                textBoxRecords.Text = rows.ToString();
            }

            if (WMode == 21)
            {

                RBal.ReadTXN_OriginalInError(WAtmNo, WDtFrom, WDtTo);

                buttonShowSM.Hide();
                dataGridView1.DataSource = RBal.DataTableTXN_Balancing.DefaultView;
                int rows = RBal.DataTableTXN_Balancing.Rows.Count;
                textBoxRecords.Text = rows.ToString();
            }
        }

        // On ROW ENTER 
       
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (WMode == 18 )
            {
                DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
                WSeqNo = (int)dataGridView1.Rows[e.RowIndex].Cells["SeqNo"].Value;
                return; 
            }
            if (WMode == 20)
            {
                DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
                string ATMNO = (string)dataGridView1.Rows[e.RowIndex].Cells["AtmNo"].Value;
                return;
            }
            if (WMode == 21)
            {
                DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
                WSeqNo = (int)dataGridView1.Rows[e.RowIndex].Cells["SeqNo"].Value;

                buttonJournalLines.Show(); 
                return;
            }
            if ((WMode == 7 || WMode == 8))
            {
                DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
                WSeqNo = (int)dataGridView1.Rows[e.RowIndex].Cells["SeqNo"].Value;
            }
            else
            {
                if (WMode== 15)
                {
                    // THESE ARE FOREX TOTALS For the ATM and period 
                    textBoxTerm.Text = WAtmNo;  
                }
                else
                {
                    DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
                    WSeqNo = (int)dataGridView1.Rows[e.RowIndex].Cells["SeqNo"].Value;
                    string SelectionCriteria = " WHERE SeqNo =" + WSeqNo;
                    if (WTableId == "Atms_Journals_Txns")
                    {
                        Mpa.ReadInPoolTransSpecificBySelectionCriteria(SelectionCriteria, 2);
                        textBoxTerm.Text = Mpa.TerminalId;
                    }
                    else
                    {
                        //if ((WMode == 7)
                        Mgt.ReadTransSpecificFromSpecificTable_Order_By_Date(SelectionCriteria, WTableId, 2);
                        textBoxTerm.Text = Mgt.TerminalId;
                    }

                }

            }


        }

        // Show Grid 03 
        public void ShowGrid03()
        {

            //dataGridView1.Columns[0].Width = 70; // WhatFile
            //dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //dataGridView1.Columns[1].Width = 60; // SeqNo
            //dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }

// FINISH 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }

        private void buttonPrint_Click(object sender, EventArgs e)
        {
            // Print
            string P1 = "";


            P1 = "Records for Replenishement Cycle..." + WSesNo; 
            

            //if (WMode == 2)
            //{
            //    P1 = "Processed Records of File..." + WTableId + "..At Cycle.." + WRMCycle.ToString();
            //}

            //if (WMode == 3)
            //{
            //    P1 = "Discrepancies for AtmNo.." + WAtmNo + "..At Cycle.." + WRMCycle.ToString();
            //}
            //if (WMode == 3 & InternalUse == true)
            //{
            //    P1 = "Records for AtmNo.." + WAtmNo + "..At Cycle.." + WRMCycle.ToString();
            //}

            string P2 = "";  // Category 
            string P3 = "";
            string P4 = WOperator;
            string P5 = WSignedId;

            
            if (WTableId == "Atms_Journals_Txns")
            {
                Form56R68ATMS_W_Pool_Table ReportATMS_Pool_Table = new Form56R68ATMS_W_Pool_Table(P1, P2, P3, P4, P5);
                ReportATMS_Pool_Table.Show();
            }
            else
            {
                Form56R68ATMS_W_General_Files ReportATMS_General_Files = new Form56R68ATMS_W_General_Files(P1, P2, P3, P4, P5);
                ReportATMS_General_Files.Show();
            }

           
        }
// Show Terminal
        private void buttonShowTerm_Click(object sender, EventArgs e)
        {
           // InternalUse = true; 
          //  WMode = 3;
            WAtmNo = textBoxTerm.Text;
            Form78b_Load(this, new EventArgs());
        }
// Refresh 
        private void buttonRefresh_Click(object sender, EventArgs e)
        {

            WAtmNo = ""; 
           // WAtmNo = textBoxTerm.Text;
            Form78b_Load(this, new EventArgs());
        }
//
// EXPORT TO EXCEL 
//
        private void buttonExportToExcel_Click(object sender, EventArgs e)
        {
            RRDM_EXCEL_AND_Directories XL = new RRDM_EXCEL_AND_Directories();  
            
            string YYYY = DateTime.Now.Year.ToString();
            string MM = DateTime.Now.Month.ToString();
            string DD = DateTime.Now.Day.ToString();
            string Min = DateTime.Now.Minute.ToString();
            if (WMode == 20)
            {
                //MessageBox.Show("Excel will be created in RRDM Working Directory");
                string Id = WAtmNo + "_" + WSesNo.ToString() + "_" + Min;

                string ExcelPath = "C:\\RRDM\\Working\\" + Id + ".xlsx";
                string WorkingDir = "C:\\RRDM\\Working\\";
                XL.ExportToExcel(RBal.DataTableTXN_Balancing, WorkingDir, ExcelPath);
            }
            else
            {
                //MessageBox.Show("Excel will be created in RRDM Working Directory");
                string Id = WAtmNo + "_" + WSesNo.ToString() + "_" + Min;

                string ExcelPath = "C:\\RRDM\\Working\\" + Id + ".xlsx";
                string WorkingDir = "C:\\RRDM\\Working\\";
                XL.ExportToExcel(Mgt.DataTableAllFields, WorkingDir, ExcelPath);
            }
            
        }
// Show SM Lines 
        private void buttonShowSM_Click(object sender, EventArgs e)
        {
            RRDMRepl_SupervisorMode_Details SM = new RRDMRepl_SupervisorMode_Details();
            SM.Read_SM_Record_Specific(WSeqNo);

            if (SM.RecordFound == true)
            {
                Form67_BDC NForm67_BDC;

                int Mode = 7; // Given Fuid and Ruid 
                string WTraceRRNumber = "";
                NForm67_BDC = new Form67_BDC(WSignedId, 0, WOperator, SM.Fuid, WTraceRRNumber, WAtmNo
                    , SM.sessionstart_ruid, SM.sessionend_ruid, NullPastDate, NullPastDate, Mode);
                NForm67_BDC.ShowDialog();
            }
            else
            {
                MessageBox.Show("Not found records");
            }

        }
// Show Journal Lines 
        private void buttonJournalLines_Click(object sender, EventArgs e)
        {
           
            int WSeqNoA = 0;
            int WSeqNoB = 0;

            WSeqNoA = WSeqNo;
            WSeqNoB = WSeqNo;
            //
            // Bank De Caire
            //
            Form67_ROM NForm67_ROM;

            int Mode = 5; // Specific
            //string WTraceRRNumber = Mpa.TraceNoWithNoEndZero.ToString();
            //if (Mpa.TraceNoWithNoEndZero == 0 & Mpa.RRNumber != "") WTraceRRNumber = Mpa.RRNumber;

            NForm67_ROM = new Form67_ROM(WSignedId, 0, WOperator,0,"", WAtmNo, WSeqNoA, WSeqNoB, Mpa.TransDate, NullPastDate, Mode);
            NForm67_ROM.ShowDialog();
        }
// Create 
        private void buttonCreateTxn_Click(object sender, EventArgs e)
        {
            RRDMUsers_Applications_Roles Uar = new RRDMUsers_Applications_Roles();

            Uar.ReadUsersVsApplicationsVsRolesByUser(WSignedId);
            Uar.Authoriser = true;
            if (Uar.Authoriser == true)
            {
                RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
                string WJobCategory = "ATMs";
                int WReconcCycleNo;
                WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);
                //if (WReconcCycleNo != 0)
                //{
                //    WCut_Off_Date = Rjc.Cut_Off_Date;
                //}
                //else
                //{
                //    WCut_Off_Date = NullPastDate;
                //}



                Form503_Insert_Manual_To_Mpa NForm503_Insert_Manual_To_Mpa;

                int WMode = 1;
                bool T24Version = false;
                NForm503_Insert_Manual_To_Mpa = new Form503_Insert_Manual_To_Mpa(WSignedId, 898, WOperator, WReconcCycleNo, WMode, T24Version);
                NForm503_Insert_Manual_To_Mpa.ShowDialog();
            }
            else
            {
                MessageBox.Show("Not Allowed Operation" + Environment.NewLine
                    + "Only Checkers are allowed. "
                    );
                return;
            }

        }
    }
}
