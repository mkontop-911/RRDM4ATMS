using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

//multilingual


namespace RRDM4ATMsWin
{
    public partial class Form48 : Form
    {
        //string filter;

        Form24 NForm24;
        //Form67 NForm67;

        Form51 NForm51;
        //Form71 NForm71; 

        int WProcessMode; 

        RRDMUpdateGrids Ug = new RRDMUpdateGrids();
        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();
        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();
        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();

        string WUserBankId; 
   
        int WSesNo;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        bool AudiType;

        // string MsgFilter;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
    
        string WAtmNo;
        DateTime WDtFrom;
        DateTime WDtTo;

        public Form48(string InSignedId, int SignRecordNo, string InOperator,  string InAtmNo, DateTime InDtFrom, DateTime InDtTo)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
       
            WAtmNo = InAtmNo;
            WDtFrom = InDtFrom;
            WDtTo = InDtTo; 
            InitializeComponent();

            // Set Working Date 
            RRDMGasParameters Gp = new RRDMGasParameters();

            labelStep1.Text = "Replenishement Cycles from.." + WDtFrom.ToShortDateString() + "..TO.." + WDtTo.ToShortDateString(); 
            
            // ================USER BANK =============================
            Us.ReadUsersRecord(WSignedId); // Read USER record for the signed user
            WUserBankId = Us.Operator;
            // ========================================================

            // Find If AUDI Type 
            // If found and it is 1 is Audi Type If Zero then is normal 
            //RRDMGasParameters Gp = new RRDMGasParameters();
            AudiType = false;
            int IsAmountOneZero;
            Gp.ReadParametersSpecificId(InOperator, "945", "4", "", ""); // 
            if (Gp.RecordFound == true)
            {
                IsAmountOneZero = (int)Gp.Amount;

                if (IsAmountOneZero == 1)
                {
                    // Transactions will be done at the end 
                    AudiType = true;


                }
                else
                {
                    AudiType = false;
                }
            }
            else
            {
                AudiType = false;
            }
            //labelStep1.Text = "Replenishment Cycles for AtmNo : " + WAtmNo; 

        }

        private void Form48_Load(object sender, EventArgs e)
        {
           
            //
            //  DataGrid 
            //
            //

            Ta.ReadReplCyclesForFromToDateFillTable(WOperator, WSignedId ,WAtmNo, WDtFrom, WDtTo);

            dataGridView1.DataSource = Ta.ATMsReplCyclesSelectedPeriod.DefaultView;

            ShowGrid_ReplCycles(); 

            //if (AudiType == true)
            //{
            //    ShowGrid_ATMs_1_2();
            //}
            //else
            //{
            //    ShowGrid_ATMs_1();
            //}


        }
        int WFuid; 
        // On Row ENTER Identify WSesNo and ATM 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WSesNo = (int)rowSelected.Cells[0].Value;

            string WSelectionCriteria = " WHERE TerminalId='" + WAtmNo + "'"
                                            + " AND ReplCycleNo =" + WSesNo;
            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(WSelectionCriteria,2);
            if (Mpa.RecordFound == true)
            {
                WFuid = Mpa.FuID;

                Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, Mpa.RMCateg, Mpa.MatchingAtRMCycle);

                ShowSessionInfo(WAtmNo, WSesNo);
            }  
            else
            {
                ShowSessionInfo(WAtmNo, WSesNo);
            }

        }

        // Show Grid 1
        private void ShowGrid_ReplCycles()
        {

            dataGridView1.Columns[0].Width = 70; // SesNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            // dataGridView2.Sort(dataGridView2.Columns[0], ListSortDirection.Descending);

            dataGridView1.Columns[1].Width = 125; // SesDtTimeStart
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[2].Width = 125; // SesDtTimeEnd
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 160; // ProcessMode
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[4].Width = 65; // Mode_2
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[5].Width = 150; // ExcelStatus
            //dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }

        // Show Session Information 
        private void ShowSessionInfo(string InAtmNo, int InSesNo)
        {
            WAtmNo = InAtmNo;
            WSesNo = InSesNo;

            label13.Text = "REPL CYCLE INFORMATION FOR : " + WSesNo.ToString();

            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

            if (Ta.RecordFound == false)
            {
                MessageBox.Show("REPL CYCLE NUMBER NOT FOUND ");
                return;
            }

            WProcessMode = Ta.ProcessMode; 

            int Function = 2;
            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, Function);
           

            textBox2.Text = WAtmNo;
            textBox3.Text = WSesNo.ToString();

            textBox5.Text = Ta.FirstTraceNo.ToString();
            textBox7.Text = Ta.LastTraceNo.ToString();

            textBox10.Text = (Ta.Stats1.NoOfTranCash + Ta.Stats1.NoOfTranDepCash + Ta.Stats1.NoOfTranDepCheq).ToString();
            textBox9.Text = (Na.Balances1.OpenBal - Na.Balances1.MachineBal).ToString("#,##0.00");
            textBox14.Text = Na.Balances1.MachineBal.ToString("#,##0.00");
            textBox20.Text = Na.Balances1.CountedBal.ToString("#,##0.00");

            textBox19.Text = "N/A";
            textBox4.Text = "N/A";
            textBox6.Text = "N/A";

            if (WProcessMode == -1)
            {
                textBox19.Text = "Serving Stage";
            }
            if (WProcessMode == 0)
            {
                textBox19.Text = "Waiting For RRDM Workflow";
            }
            if (WProcessMode >= 1)
            {
                textBox19.Text = "COMPLETED Repl Cycle";
                textBox4.Text = Ta.Repl1.ReplStartDtTm.ToString();
                textBox6.Text = Ta.Repl1.ReplFinDtTm.ToString(); 
            }
            if (WProcessMode >= 2)
            {
                textBox19.Text = "COMPLETED Reconciliation and Repl Cycle";
                textBox17.Text = Ta.Recon1.RecStartDtTm.ToString();
                textBox18.Text = Ta.Recon1.RecFinDtTm.ToString(); 
            }

            textBox8.Text = Ta.Diff1.CurrNm1;
            textBox11.Text = Ta.Diff1.DiffCurr1.ToString("#,##0.00");
            textBox12.Text = Ta.SessionsInDiff.ToString();

            Ec.ReadAllErrorsTableForCounterReplCycle(WOperator, WAtmNo, WSesNo);

            textBox13.Text = (Ec.NumOfErrors - Ec.ErrUnderAction).ToString();

            textBox1.Text = Ec.NumOfErrors.ToString();

            if (Ec.NumOfErrors  > 0)
            {
                button1.Show();
            }
            else
            {
                button1.Hide();
            }

            Us.ReadUsersRecord(Ta.Repl1.SignIdRepl);
            textBox15.Text = Us.UserName;

            Us.ReadUsersRecord(Ta.Recon1.SignIdReconc);
            textBox16.Text = Us.UserName;

            if (WSesNo == 3144 || WSesNo == 9051)
            {
                button2.Enabled = true;
                button2.BackColor = Color.White;
            }
            else
            {
                button2.Enabled = false;
                button2.BackColor = Color.Silver;
            }

            if (WProcessMode > 0)
            {        
                buttonVewWorkFlow.Enabled = true;
                buttonVewWorkFlow.BackColor = Color.White;
            }
            else
            {
                buttonVewWorkFlow.Enabled = false;
                buttonVewWorkFlow.BackColor = Color.Silver;
                //MessageBox.Show("Not allowed operation. Repl Workflow not done yet");
            }

        }
        // Show Errors 
        private void button1_Click(object sender, EventArgs e)
        {
         
                bool Mode = true;
                string SearchFilter = "AtmNo = '" + WAtmNo + "' AND SesNo=" + WSesNo;
                NForm24 = new Form24(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo, "", Mode, SearchFilter);
                NForm24.ShowDialog();
       
        }
        // Show Journal 
        private void button3_Click(object sender, EventArgs e)
        {
            if (WAtmNo == "AB102" & WSesNo == 3144)
            {
                MessageBox.Show("For this ATM do not use the Replenishement Cycle 3144. Use another replenishment Cycle.");
                return; 
            }
            Form67 NForm67;
            int Mode = 3;
            NForm67 = new Form67(WSignedId, WSignRecordNo, WOperator, WFuid, "", 0, 0, NullPastDate, NullPastDate, Mode);
            NForm67.Show();

        }
    
// View workflow 
        private void buttonVewWorkFlow_Click(object sender, EventArgs e)
        {
            //Ta.Stats1.NoOfCheques = 1
            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

            if (Ta.Stats1.NoOfCheques == 1)
            {
                if (WProcessMode > 0)
                {
                    RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
                    Usi.ReadSignedActivityByKey(WSignRecordNo);
                    Usi.ProcessNo = 54; // View only for replenishment already done  
                    Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);


                    // CALL THE SAME If Recycle or not 
                    bool IsFromExcel = false;
                    Form51_Repl_For_IST NForm51_Repl_For_IST;
                    NForm51_Repl_For_IST = new Form51_Repl_For_IST(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo, IsFromExcel);
                    NForm51_Repl_For_IST.FormClosed += NForm51_FormClosed;
                    NForm51_Repl_For_IST.ShowDialog();

                }
                else
                {
                    MessageBox.Show("Not allowed operation. Repl Workflow not done yet");
                }

                return; 
            }

            RRDMGasParameters Gp = new RRDMGasParameters(); 
            // Find If AUDI Type 
            // If found and it is 1 is Audi Type If Zero then is normal 
            bool AudiType = false;
            int IsAmountOneZero;
            Gp.ReadParametersSpecificId(WOperator, "945", "4", "", ""); // 
            if (Gp.RecordFound == true)
            {
                IsAmountOneZero = (int)Gp.Amount;

                if (IsAmountOneZero == 1)
                {
                    // Transactions will be done at the end 
                    AudiType = true;
                }
                else
                {
                    AudiType = false;
                }
            }
            else
            {
                AudiType = false;
            }
            if (WProcessMode > 0 )
            {
                RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
                Usi.ReadSignedActivityByKey(WSignRecordNo);
                Usi.ProcessNo = 54; // View only for replenishment already done  
                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                if (AudiType == true)
                {
                    Form51_FAB_Type NForm51_AUDI_TYPE; 
                    NForm51_AUDI_TYPE = new Form51_FAB_Type(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                    NForm51_AUDI_TYPE.ShowDialog();
                }
                else
                {
                    //
                    // Find out if ATM is Recycling 
                    //
                    bool IsRecycle = false;

                    string ParId2 = "948";
                    string OccurId2 = "1"; // 
                                           //RRDMGasParameters Gp = new RRDMGasParameters(); 
                    Gp.ReadParametersSpecificId(WOperator, ParId2, OccurId2, "", "");
                    if (Gp.RecordFound & Gp.OccuranceNm == "YES")
                    {
                        RRDMRepl_SupervisorMode_Details_Recycle SM = new RRDMRepl_SupervisorMode_Details_Recycle();
                        SM.Read_SM_Record_Specific_By_ATMno_ReplCycle(WAtmNo, WSesNo);
                        if (SM.RecordFound == true)
                        {
                            // Check if Reccyle 
                            if (SM.is_recycle == "Y")
                            {
                                IsRecycle = true;
                            }
                        }
                    }

                    if (IsRecycle == true)
                    {
                        // Recycle Type 
                        Form51_Recycle NForm51_Recycle;
                        NForm51_Recycle = new Form51_Recycle(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                       // NForm51_Recycle.FormClosed += NForm51_FormClosed;
                        NForm51_Recycle.ShowDialog();
                    }
                    else
                    {
                        // Current Bank De Caire Type 
                        NForm51 = new Form51(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                      ///  NForm51.FormClosed += NForm51_FormClosed;
                        NForm51.ShowDialog();
                    }

                   
                }
               
            }
            else
            {
                MessageBox.Show("Not allowed operation. Repl Workflow not done yet"); 
            }

        }
        // Show reconciliation workflow 
        private void button2_Click(object sender, EventArgs e)
        {
            


            if (WProcessMode > 0)
            {
                RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
                Usi.ReadSignedActivityByKey(WSignRecordNo);
                Usi.ProcessNo = 54; // View only for reconciliation already done  
                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);


                Form271 NForm271;

                NForm271 = new Form271(WSignedId, WSignRecordNo, WOperator, Mpa.RMCateg, Mpa.MatchingAtRMCycle);
                //NForm271.FormClosed += NForm271_FormClosed;
                NForm271.ShowDialog();
            }
            else
            {
                if (WAtmNo == "AB102")
                {
                    MessageBox.Show("For this Demo Select ReplCycle No = 3144");
                }
                if (WAtmNo == "AB104")
                {
                    MessageBox.Show("For this Demo Select ReplCycle No = 9051");
                }
            }
        }
        void NForm51_FormClosed(object sender, FormClosedEventArgs e)
        {

            //int WRow1 = dataGridViewMyATMS.SelectedRows[0].Index;

            //int scrollPosition1 = dataGridViewMyATMS.FirstDisplayedScrollingRowIndex;

            ////int WRow2 = dataGridView2.SelectedRows[0].Index;

            //int scrollPosition2 = dataGridView2.FirstDisplayedScrollingRowIndex;

            Form48_Load(this, new EventArgs());
            // First
            //dataGridViewMyATMS.Rows[WRow1].Selected = true;
            //dataGridViewMyATMS_RowEnter(this, new DataGridViewCellEventArgs(1, WRow1));

            //dataGridViewMyATMS.FirstDisplayedScrollingRowIndex = scrollPosition1;

            // second

            //dataGridView2.Rows[WRow2].Selected = true;
            //dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRow2));

            //dataGridView2.FirstDisplayedScrollingRowIndex = scrollPosition2;

        }
        // Finish 
        private void buttonNext_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
// Print Replenishemnt Report
        private void buttonPrint_Click(object sender, EventArgs e)
        {
            string P1 = "Replenishment Cycles for :" + WAtmNo ; 

            string P2 = "Second Par";
            string P3 = WAtmNo;
            string P4 = WOperator;
            string P5 = WSignedId;

            Form56R69ATMS_Repl_Cycles ReportATMS_Repl_Cycles = new Form56R69ATMS_Repl_Cycles(P1, P2, P3, P4, P5);
            ReportATMS_Repl_Cycles.Show();
        }
// View only
        private void buttonCreatedExcelCycles_Click(object sender, EventArgs e)
        {
            Form18_CIT_ExcelOutputCycles NForm18_CIT_ExcelOutputCycles;
            string WCitId = "1000";
            int Mode = 2; // 

            NForm18_CIT_ExcelOutputCycles = new Form18_CIT_ExcelOutputCycles(WSignedId, WSignRecordNo, WOperator, WCitId);
            NForm18_CIT_ExcelOutputCycles.ShowDialog();
        }

    }
}
