using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.Configuration;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data.SqlClient;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form273_AUDI : Form
    {

        int StepNumber;

        // Working Fields 
        bool WViewFunction; // 54
        bool WAuthoriser; // 55
        bool WRequestor;  // 56

        //   int WOrdersCycle;
        DateTime WOutputDate;

        DateTime WDTm;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WSelectionCriteria;

        bool ReconciliationAuthorNoRecordYet;
        bool ReconciliationAuthorDone;

        bool ReconciliationAuthorOutstanding;

        RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();

        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();

        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

        RRDMReplOrdersClass Ro = new RRDMReplOrdersClass();

        RRDM_Cit_ExcelOutputCycles Coc = new RRDM_Cit_ExcelOutputCycles();

        RRDMAtmsClass Ac = new RRDMAtmsClass();

        string WCategory;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WCitId;
        int WOrdersCycle;
        string WFunction;

        public Form273_AUDI(string InSignedId, int InSignRecordNo, string InOperator, string InCitId, int InOrdersCycle, string InFunction)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WCategory = WCitId = InCitId;
            WOrdersCycle = InOrdersCycle;

            WFunction = InFunction;

            if (InFunction == "")
            {
                Coc.ReadExcelOutputCyclesBySeqNo(WOrdersCycle);

                WFunction = Coc.OrdersFunction; 
            }

            InitializeComponent();

           if (WFunction =="PrepareMoneyIn")
            {
                labelStep1.Text = "My ATM/s Money Status"; 
            }

            // Set Working Date 

            pictureBox1.BackgroundImage = appResImg.logo2;

            StepNumber = 1;

            labelCitId.Text = WCitId;

            // STEPLEVEL
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            Usi.WFieldChar1 = WCategory;
            Usi.WFieldNumeric1 = WOrdersCycle;

            //Usi.StepLevel = 0;
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            labelExcelCycle.Text = WOrdersCycle.ToString();

            Coc.ReadExcelOutputCyclesBySeqNo(WOrdersCycle);

            labelExcelDate.Text = Coc.CreatedDateTm.ToShortDateString();

            if (WOperator == "CRBAGRAA")
            {
                WDTm = new DateTime(2014, 02, 28);
            }
            else
            {
                RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

                string WJobCategory = "ATMs";
                int WReconcCycleNo;

                WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);
                if (WReconcCycleNo == 0)
                {
                    MessageBox.Show("Cut Off Cycle is Zero. Start a new Cycle.");
                    return;
                }
                DateTime WCut_Off_Date = Rjc.Cut_Off_Date;
                WDTm = WCut_Off_Date;
            }

            //************************************************************
            //************************************************************
            // AUTHOR PART 
            // Comes from FORM112 = Author management 
            //************************************************************
            WViewFunction = false; // 54
            WAuthoriser = false;   // 55
            WRequestor = false;    // 56 

            // RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.ProcessNo == 54) WViewFunction = true;// ViewOnly 
            if (Usi.ProcessNo == 55) WAuthoriser = true;// Authoriser from author management 
            if (Usi.ProcessNo == 56) WRequestor = true; // Requestor

            //------------------------------------------------------------

            bool Reject = false;

          
            // Requestor 
            if (WRequestor == true)  // 56: Coming from requestor 
            {
                Reject = false;
                Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WCategory, WOrdersCycle, "ReplOrders");

                if (Ap.RecordFound == true & Ap.OpenRecord == true
                    & Ap.Stage == 4 & Ap.AuthDecision == "NO")
                {
                    Reject = true;

                    Usi.ReadSignedActivityByKey(WSignRecordNo);

                    Usi.ProcessNo = 2;

                    WRequestor = false;

                    Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                    MessageBox.Show("Authorisation has been rejected." + Environment.NewLine
                        + "Authorisation Comment is below: " + Environment.NewLine
                        + Ap.AuthComment
                                   );
                }
                else
                {
                    if (Ap.RecordFound == true & Ap.OpenRecord == true
                         & Ap.Stage == 4)
                    {
                        MessageBox.Show("Go through the workflow." + Environment.NewLine
                         + "See Authoriser action at the last step." + Environment.NewLine
                         + "At the last step press finish to complete."
                     );
                    }
                }
            }
           

            if (Usi.ProcessNo == 2 //Follow Reconciliation Workflow - NORMAL 2
                || Usi.ProcessNo == 54 // ViewOnly 
                || Usi.ProcessNo == 55 // Authoriser
                || Usi.ProcessNo == 56 // Requestor   
                ) 
            {
                //Start with STEP 1


                //   tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                UCForm273a_AUDI Step273a_AUDI = new UCForm273a_AUDI();

                //  Step273a_AUDI.ChangeBoardMessage += Step273a_AUDI_ChangeBoardMessage;
                tableLayoutPanelMain.Controls.Add(Step273a_AUDI, 0, 0);
                Step273a_AUDI.Dock = DockStyle.Fill;
                Step273a_AUDI.UCForm273a_AUDI_Par(WSignedId, WSignRecordNo, WOperator, WCitId, WOrdersCycle, WFunction);
                Step273a_AUDI.SetScreen();
                textBoxMsgBoard.Text = Step273a_AUDI.guidanceMsg;

                if (Usi.ProcessNo == 2) textBoxMsgBoard.Text = "Review and go to Next";
                if (WViewFunction == true || WAuthoriser == true || WRequestor == true) textBoxMsgBoard.Text = "View only";

                buttonNext.Visible = true;
            }
        }

        // NEXT STEP 
        //
        private void buttonNext_Click(object sender, EventArgs e)
        {
            //************************************************************
            //************************************************************
            // AUTHOR PART 
            // Comes from FORM112 = Author management 
            //************************************************************
            WViewFunction = false; // 54
            WAuthoriser = false;   // 55
            WRequestor = false;    // 56 

            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.ProcessNo == 54) WViewFunction = true;// ViewOnly 
            if (Usi.ProcessNo == 55) WAuthoriser = true;// Authoriser from author management 
            if (Usi.ProcessNo == 56) WRequestor = true; // Requestor

            bool Jump_Step2 = false;
            if (StepNumber == 1)
            {
                // Check if Orders have been created
                // Move to authorisation if not created

                // Read Orders Counters
                Ro.ReadReplActionsForCounters(WCitId, WOrdersCycle);

                if (Ro.NumberOfActiveOrders == 0)
                {
                    // Move one step 
                    StepNumber = 2;
                    Jump_Step2 = true;
                }
                else
                {
                    Jump_Step2 = false;
                }
            }


            //
            // SWITCH StepNumber
            //
            switch (StepNumber)
            {
                case 1:
                    {


                        UCForm273b_AUDI Step273b_AUDI = new UCForm273b_AUDI();

                        Step273b_AUDI.Dock = DockStyle.Fill;

                        tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                        tableLayoutPanelMain.Controls.Add(Step273b_AUDI, 0, 0);
                        Step273b_AUDI.UCForm273b_AUDI_Par(WSignedId, WSignRecordNo, WOperator, WCitId, WOrdersCycle, WFunction);
                        Step273b_AUDI.SetScreen();
                        textBoxMsgBoard.Text = Step273b_AUDI.guidanceMsg;

                        if (Usi.ProcessNo == 2) textBoxMsgBoard.Text = "Examine And Act on Orders if needed.";
                        if (WViewFunction == true || WAuthoriser == true || WRequestor == true) textBoxMsgBoard.Text = "View only";

                        labelStep1.ForeColor = labelStep3.ForeColor;
                        labelStep1.Font = new Font(labelStep1.Font.FontFamily, 10, labelStep1.Font.Style);
                        labelStep2.ForeColor = Color.White;
                        labelStep2.Font = new Font(labelStep2.Font.FontFamily, 16, FontStyle.Bold);

                        // Step Becomes 2
                        StepNumber++;
                        buttonBack.Visible = true;
                        buttonNext.Visible = true;

                        break;
                    }

                case 2:
                    {
                        //tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                        UCForm273c_AUDI Step273c_AUDI = new UCForm273c_AUDI();

                        Step273c_AUDI.Dock = DockStyle.Fill;

                        tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                        tableLayoutPanelMain.Controls.Add(Step273c_AUDI, 0, 0);
                        Step273c_AUDI.UCForm273c_AUDI_Par(WSignedId, WSignRecordNo, WOperator, WCitId, WOrdersCycle, WFunction);
                        Step273c_AUDI.SetScreen();
                        textBoxMsgBoard.Text = Step273c_AUDI.guidanceMsg;

                        if (Usi.ProcessNo == 2) textBoxMsgBoard.Text = "Examine And Act on Orders if needed.";
                        if (WViewFunction == true || WAuthoriser == true || WRequestor == true) textBoxMsgBoard.Text = "View only";

                        if (Jump_Step2 == false)
                        {
                            labelStep2.ForeColor = labelStep1.ForeColor;
                            labelStep2.Font = new Font(labelStep1.Font.FontFamily, 10, labelStep1.Font.Style);
                            labelStep3.ForeColor = Color.White;
                            labelStep3.Font = new Font(labelStep2.Font.FontFamily, 16, FontStyle.Bold);
                        };

                        if (Jump_Step2 == true)
                        {
                            // We are at the first and we move to the third
                            labelStep1.ForeColor = labelStep2.ForeColor;
                            labelStep1.Font = new Font(labelStep2.Font.FontFamily, 10, labelStep2.Font.Style);
                            labelStep3.ForeColor = Color.White;
                            labelStep3.Font = new Font(labelStep2.Font.FontFamily, 16, FontStyle.Bold);

                        };

                        // Step Becomes 3
                        StepNumber++;
                        buttonBack.Visible = true;
                        buttonNext.Text = "Finish";
                        buttonNext.Visible = true;

                        if (WViewFunction == true) buttonNext.Enabled = false;

                        break;
                    }
                case 3:
                    {
                        //**************************************************
                        //FINISH BUTTON .... HANDLE AUTHORISATION VALIDATION 
                        //**************************************************

                        if (WViewFunction == true) // Coming from View only 
                        {
                            this.Dispose();
                            return;
                        }

                        // FINISH - Make validationsfor Authorisations  


                        //  ReconciliationAuthorNeeded = true;

                        Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WCategory, WOrdersCycle, "ReplOrders");
                        if (Ap.RecordFound == true)
                        {
                            ReconciliationAuthorNoRecordYet = false;
                           
                            if (Ap.Stage == 3 || Ap.Stage == 4 )
                            {
                                ReconciliationAuthorDone = true;
                            }
                            else
                            {
                                ReconciliationAuthorOutstanding = true;
                            }
                        }
                        else
                        {
                            ReconciliationAuthorNoRecordYet = true;
                        }


                        if (WAuthoriser == true & ReconciliationAuthorDone == true) // Coming from authoriser and authoriser done  
                        {
                            this.Dispose();
                            return;
                        }

                        if (Ap.AuthDecision == "NO" & ReconciliationAuthorDone == true)
                        {
                            this.Dispose();
                            return;
                        }

                        if (Usi.ProcessNo == 2 & ReconciliationAuthorDone == true) //  
                        {
                            this.Dispose();
                            return;
                        }

                        if (WRequestor == true & ReconciliationAuthorDone == false) // Coming from authoriser and authoriser not done 
                        {
                            this.Dispose();
                            return;
                        }

                        if (Usi.ProcessNo == 2 & ReconciliationAuthorNoRecordYet == true) // Cancel by originator without request for authorisation 
                        {
                            if (MessageBox.Show("Warning: Authorisation outstanding " + ". Do You want to abort?  ", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                                         == DialogResult.Yes)
                            {
                                this.Dispose();
                                return;
                            }
                            else
                            {
                                return;
                            }

                        }

                        if (Usi.ProcessNo == 2 & ReconciliationAuthorOutstanding == true) // Cancel with repl outstanding 
                        {

                            if (MessageBox.Show("Warning: Authorisation outstanding " + ". Do You want to abort?  ", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                                          == DialogResult.Yes)
                            {
                                this.Dispose();
                                return;
                            }
                            else
                            {
                                return;
                            }

                        }

                        if (WAuthoriser == true & ReconciliationAuthorOutstanding == true) // Cancel by authoriser without making authorisation.
                        {
                            MessageBox.Show("MSG946 - Authorisation outstanding");
                            this.Dispose();
                            return;
                        }

                        if (WRequestor == true & ReconciliationAuthorDone == true & Ap.Stage == 4
                             & Ap.AuthDecision == "YES") // Everything is fined .
                        {
                            // Everything OK to move to transactions
                        }
                        else
                        {
                            MessageBox.Show("MSG946 - Authorisation outstanding");
                            this.Dispose();
                            return;
                        }
                        //
                        //**************************************************************************
                        //**************************************************************************
                        // IF YOU CAME TILL HERE THEN RECONCILIATION WILL BE COMPLETED WITH UPDATING 
                        //**************************************************************************
                        //***********************************************************************
                        //**************** USE TRANSACTION SCOPE
                        //***********************************************************************

                        // create a connection object
                        //   using (var scope = new System.Transactions.TransactionScope())
                        try
                        {
                            // Update authorisation record  

                            if (Ap.RecordFound == true & Ap.Stage == 4)
                            {
                                // Update stage 
                                //
                                Ap.Stage = 5;
                                Ap.OpenRecord = false;

                                Ap.UpdateAuthorisationRecord(Ap.SeqNumber);

                                // Update all orders as Authorised 
                                Ro.ReadReplActionsForCounters(WCitId, WOrdersCycle);
                                if (Ro.NumberOfActiveOrders > 0)
                                {
                                    Ro.UpdateReplActionsForAuthorised(Ap.Authoriser, WCitId, WOrdersCycle);
                                }

                                if (WFunction == "ATMsInNeed" || WFunction == "PrepareMoneyIn")
                                {
                                    // Read Orders Counters

                                    if (Ro.NumberOfActiveOrders == 0)
                                    {
                                        // No Orders 
                                        // Do not create excel
                                    }
                                    else
                                    {
                                        //
                                        // WE HAVE ORDERS
                                        //
                                        // Create Excel 
                                        // 
                                        // For each Order create the needed transactions
                                        // Create the Excel
                                        // Send the Excel By email
                                        // Update the order Cycle
                                        // Update the Order status

                                        ///
                                        /// Create GL ENTRIES FOR CIT AND BANK CASH
                                        /// 


                                        Ro.ReadReplActionsForCITAndUpdate(WCitId, WOrdersCycle, WOperator, WSignedId);


                                        // Update Orders Cycle as completed 

                                        WSelectionCriteria = " WHERE CitId ='" + WCitId + "' AND SeqNo =" + WOrdersCycle;
                                        Coc.ReadExcelOutputCyclesBySelectionCriteria(WSelectionCriteria);

                                        // Cec.ExcelIdAndLocation = strFullFilePathNoExt;
                                        if (WFunction == "ATMsInNeed")
                                        {
                                            Coc.ExcelIdAndLocation = "Not Ready";
                                        }
                                        else
                                        {
                                            Coc.ExcelIdAndLocation = "N/A";
                                        }

                                        Coc.SendByEmail = true;
                                        Coc.SendDateTm = DateTime.Now;

                                        Coc.MakerId = WSignedId;
                                        Coc.AuthorisedDateTm = DateTime.Now;
                                        Coc.AuthoriserId = Ap.Authoriser;

                                        Coc.ProcessStage = 3;

                                        Coc.Update_Cit_ExcelOutputCycles(WCitId, WOrdersCycle);

                                        // WE HAVE ORDERS ... CREATE EXCEL

                                        if (WFunction == "ATMsInNeed")
                                        {
                                            //
                                            // Create Excel
                                            //
                                            CREATE_Excel();
                                            //

                                            // UPDATE Cycles 
                                            //
                                            WSelectionCriteria = " WHERE CitId ='" + WCitId + "' AND SeqNo =" + WOrdersCycle;
                                            Coc.ReadExcelOutputCyclesBySelectionCriteria(WSelectionCriteria);

                                            Coc.ExcelIdAndLocation = strFullFilePathNoExt;
                                            Coc.ExcelRecords = WExcelRecords;
                                            Coc.ExcelAmount = WExcelAmt;

                                            Coc.Update_Cit_ExcelOutputCycles(WCitId, WOrdersCycle);

                                            PrintOrders();

                                        }
                                        else
                                        {
                                            // Is WFunction == "PrepareMoneyIn"
                                            // Create the GL Entries
                                            // Print for each Order/ATM the voucher
                                            // DISPLAY CREATED ORDERS
                                            //
                                            string WSelectionCriteria = "  CitId ='" + WCitId + "' AND OrdersCycleNo = " + WOrdersCycle;

                                            Ro.ReadReplActionsAndFillTable(WOperator, WSelectionCriteria, WDTm, WDTm, 1);

                                            int I = 0;

                                            while (I <= (Ro.TableReplOrders.Rows.Count - 1))
                                            {
                                                int WReplOrderNo = (int)Ro.TableReplOrders.Rows[I]["OrderNo"];

                                                string NeedStatus = (string)Ro.TableReplOrders.Rows[I]["NeedStatus"];

                                                Ro.ReadReplActionsSpecific(WReplOrderNo);

                                                string P1 = "Notes to be Loaded For ATM : " + Ro.AtmNo;
                                                string P2 = Ro.AtmNo;
                                                string P3 = NeedStatus;
                                                string P4 = WOperator;
                                                string P5 = WSignedId;
                                                string P6 = Ro.ReplOrderNo.ToString();

                                                Form56R60ATMS ReportATMS60 = new Form56R60ATMS(P1, P2, P3, P4, P5, P6);
                                                ReportATMS60.Show();

                                                // Create Transactions 

                                                //Am.ReadAtmsMainSpecific(WAtmNo);

                                                RRDMTransAndTransToBePostedClass Tp = new RRDMTransAndTransToBePostedClass();
                                                Tp.DeleteOldTransToBePostedByATMNoAndSession(Ro.AtmNo, Ro.ReplCycleNo, "Cash moved to Atm Supervisor from Vaults");

                                                Ec.CreateTransTobepostedfromCashManagement(WOperator, Ro.AtmNo, Ro.ReplCycleNo, WSignedId,
                                                                                           Ro.NewAmount);

                                                I++; // Read Next entry of the table 

                                            }

                                        }

                                        //
                                        // Send Email
                                        // Email 
                                        //Form2_EMailContent NForm2_EMailContent; 
                                        //NForm2_EMailContent = new Form2_EMailContent(WSignedId, WSignRecordNo, WOperator, WCitId, WOrdersCycle, Ro.PublicCitString);
                                        ////NForm2_EMailContent.FormClosed += NForm2_EMailContent_FormClosed;
                                        //NForm2_EMailContent.ShowDialog();

                                    }

                                    // END for finish here
                                    this.Dispose();

                                }
                                else
                                {
                                    // MoneyIn ???

                                    // END for finish here
                                    this.Dispose();

                                }

                                // COMPLETE SCOPE
                                //
                                //scope.Complete();

                                //this.Close();

                            }

                        }
                        catch (Exception ex)
                        {
                            CatchDetails(ex);
                        }
                        //finally
                        //{
                        //    scope.Dispose();
                        //}
                        // END for finish here

                        break;
                    }

                default:
                    {

                        break;
                    }
            }


        }

        // Stavros
        // EVENT HANDLER FOR MESSAGE

        //void Step273c_AUDI_ChangeBoardMessage(object sender, EventArgs e)
        //{
        //    textBoxMsgBoard.Text = ((UCForm271a)tableLayoutPanelMain.Controls["UCForm271a"]).guidanceMsg;
        //}

        //
        // GO BACK BACK... BACK
        // GO BACK .. BACK ... BACK 
        private void buttonBack_Click(object sender, EventArgs e)
        {
            //************************************************************
            //************************************************************
            // AUTHOR PART 
            // Comes from FORM112 = Author management 
            //************************************************************
            WViewFunction = false; // 54
            WAuthoriser = false;   // 55
            WRequestor = false;    // 56 

            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.ProcessNo == 54) WViewFunction = true;// ViewOnly 
            if (Usi.ProcessNo == 55) WAuthoriser = true;// Authoriser from author management 
            if (Usi.ProcessNo == 56) WRequestor = true; // Requestor

            bool Jump_Step2 = false;
            if (StepNumber == 3)
            {
                // Check if Orders have been created

                // Read Orders Counters
                Ro.ReadReplActionsForCounters(WCitId, WOrdersCycle);

                if (Ro.NumberOfActiveOrders == 0)
                {
                    // Move one step 
                    StepNumber = 2;
                    Jump_Step2 = true;
                }
                else
                {
                    Jump_Step2 = false;
                }

            }

            //-----------------------------------------------------------
            switch (StepNumber)
            {
                case 2:
                    {
                        tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                        UCForm273a_AUDI Step273a_AUDI = new UCForm273a_AUDI();

                        //  Step273a_AUDI.ChangeBoardMessage += Step273a_AUDI_ChangeBoardMessage;
                        tableLayoutPanelMain.Controls.Add(Step273a_AUDI, 0, 0);
                        Step273a_AUDI.Dock = DockStyle.Fill;
                        Step273a_AUDI.UCForm273a_AUDI_Par(WSignedId, WSignRecordNo, WOperator, WCitId, WOrdersCycle, WFunction);
                        Step273a_AUDI.SetScreen();
                        textBoxMsgBoard.Text = Step273a_AUDI.guidanceMsg;

                        if (Usi.ProcessNo == 2) textBoxMsgBoard.Text = "Review and go to Next";
                        if (WViewFunction == true || WAuthoriser == true || WRequestor == true) textBoxMsgBoard.Text = "View only";

                        if (Jump_Step2 == false)
                        {
                            labelStep2.ForeColor = labelStep3.ForeColor;
                            labelStep2.Font = new Font(labelStep2.Font.FontFamily, 10, labelStep2.Font.Style);
                            labelStep1.ForeColor = Color.White;
                            labelStep1.Font = new Font(labelStep1.Font.FontFamily, 16, FontStyle.Bold);

                        };

                        if (Jump_Step2 == true)
                        {
                            labelStep3.ForeColor = labelStep2.ForeColor;
                            labelStep3.Font = new Font(labelStep2.Font.FontFamily, 10, labelStep2.Font.Style);
                            labelStep1.ForeColor = Color.White;
                            labelStep1.Font = new Font(labelStep1.Font.FontFamily, 16, FontStyle.Bold);


                        };

                        buttonNext.Visible = true;

                        buttonBack.Visible = false;

                        StepNumber--;

                        break;
                    }
                case 3:
                    {

                        UCForm273b_AUDI Step273b_AUDI = new UCForm273b_AUDI();

                        Step273b_AUDI.Dock = DockStyle.Fill;

                        tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                        tableLayoutPanelMain.Controls.Add(Step273b_AUDI, 0, 0);
                        Step273b_AUDI.UCForm273b_AUDI_Par(WSignedId, WSignRecordNo, WOperator, WCitId, WOrdersCycle, WFunction);
                        Step273b_AUDI.SetScreen();
                        textBoxMsgBoard.Text = Step273b_AUDI.guidanceMsg;

                        if (Usi.ProcessNo == 2) textBoxMsgBoard.Text = "Examine And Act on Orders if needed.";
                        if (WViewFunction == true || WAuthoriser == true || WRequestor == true) textBoxMsgBoard.Text = "View only";

                        labelStep3.ForeColor = labelStep2.ForeColor;
                        labelStep3.Font = new Font(labelStep3.Font.FontFamily, 10, labelStep3.Font.Style);
                        labelStep2.ForeColor = Color.White;
                        labelStep2.Font = new Font(labelStep2.Font.FontFamily, 16, FontStyle.Bold);

                        StepNumber--;
                        buttonBack.Visible = true;
                        buttonNext.Text = "Next >";

                        if (WViewFunction == true)
                        {
                            buttonBack.Visible = false;
                            buttonNext.Enabled = true;
                        }

                        break;
                    }
                default:
                    {

                        break;
                    }

            }

        }


        // Cancel button 

        private void Cancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit reconciliation process?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                              == DialogResult.Yes)
            {
                this.Close();
            }
        }

        // THIS IMPROVES PERFORMANCE 
        // 
        protected override CreateParams CreateParams
        {

            get
            {

                CreateParams handleParam = base.CreateParams;

                handleParam.ExStyle |= 0x02000000;   // WS_EX_COMPOSITED        

                return handleParam;

            }

        }


        ////
        //// METHOD THAT IS NEEDED FOR ORDERS
        ////

        //int WProcessStage;
        //private void CreateNewOrdersCycle(string InCitId, string InFunction)
        //{
        //    RRDM_Cit_ExcelOutputCycles Coc = new RRDM_Cit_ExcelOutputCycles();
        //    // If not already exist and Open create a new one
        //    string WSelectionCriteria = " Where CitId='" + WCitId + "' AND OrdersFunction='"+ InFunction +"' AND ProcessStage != 3 ";
        //    Coc.ReadExcelOutputCyclesBySelectionCriteria(WSelectionCriteria);
        //    if (Coc.RecordFound == true)
        //    {
        //        // There is an open Cycle
        //        WOrdersCycle = Coc.SeqNo;
        //        WProcessStage = Coc.ProcessStage;
        //        WOutputDate = Coc.CreatedDateTm;
        //    }
        //    else
        //    {
        //        // Create a new cycle

        //        Coc.CitId = WCitId;
        //        Coc.Description = "Orders Cycle for CIT.." + WCitId;
        //        Coc.OrdersFunction = InFunction;
        //        Coc.CreatedDateTm = WOutputDate = DateTime.Now;
        //        Coc.MakerId = WSignedId;
        //        Coc.ProcessStage = 1;
        //        Coc.Operator = WOperator;

        //        WOrdersCycle = Coc.InsertExcelOutputCycle();

        //        WProcessStage = 1;

        //    }
        //}

        //
        // CREATE EXCEL METHOD
        //
        string strFullFilePathNoExt;
        int WExcelRecords;
        decimal WExcelAmt;
        private void CREATE_Excel()
        {

            // There are entries to create Excel      

            Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();

            if (xlApp == null)
            {
                MessageBox.Show("Excel is not properly installed!!");
                return;
            }

            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;

            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            //
            // FILL IN EXCEL LINES 
            //
            int I = 1;

            try
            {
                // These Are the headings 

                xlWorkSheet.Cells[I, 1] = "Order No";
                xlWorkSheet.Cells[I, 2] = "Atm No";
                xlWorkSheet.Cells[I, 3] = "Location";

                xlWorkSheet.Cells[I, 4] = "No Of Bills1";
                xlWorkSheet.Cells[I, 5] = "Denom_1";

                xlWorkSheet.Cells[I, 6] = "No Of Bills2";
                xlWorkSheet.Cells[I, 7] = "Denom_2";

                xlWorkSheet.Cells[I, 8] = "No Of Bills3";
                xlWorkSheet.Cells[I, 9] = "Denom_3";

                xlWorkSheet.Cells[I, 10] = "No Of Bills4";
                xlWorkSheet.Cells[I, 11] = "Denom_4";

                xlWorkSheet.Cells[I, 12] = "Total Cash";

                xlWorkSheet.Cells[I, 13] = "Dept";
                xlWorkSheet.Cells[I, 14] = "Date";
                xlWorkSheet.Cells[I, 15] = "Bank";


                // Read actions and create the table 

                string WSelectionCriteria = "  ActiveRecord = 1 AND CitId ='" + WCitId
                              + "' AND OrdersCycleNo =" + WOrdersCycle;

                Ro.ReadReplActionsAndFillTable(WOperator, WSelectionCriteria, WDTm, WDTm, 1);

                WExcelRecords = Ro.ExcelRecords;
                WExcelAmt = Ro.ExcelAmount;

                int L = 0;

                int K = 2;

                while (L <= (Ro.TableReplOrders.Rows.Count - 1))
                {

                    int OrderNo = (int)Ro.TableReplOrders.Rows[L]["OrderNo"];

                    Ro.ReadReplActionsSpecific(OrderNo);

                    // Read ATMs Details 
                    Ac.ReadAtm(Ro.AtmNo);
                    // 
                    // public int CasNo_11;
                    //public string CurName_11;
                    //public int FaceValue_11;
                    //public int CasCapacity_11;

                    xlWorkSheet.Cells[K, 1] = Ro.ReplOrderNo; //  

                    xlWorkSheet.Cells[K, 2] = Ro.AtmNo; //  "Atm No"
                    xlWorkSheet.Cells[K, 3] = Ro.AtmName; // "ATM Name"
                    // Cassette 1
                    xlWorkSheet.Cells[K, 4].NumberFormat = "#,##0";
                    xlWorkSheet.Cells[K, 4] = Ro.Cassette_1; // CassetteOneNotes
                    xlWorkSheet.Cells[K, 5] = Ac.FaceValue_11; // Denom1
                    // Cassette 2
                    xlWorkSheet.Cells[K, 6].NumberFormat = "#,##0";
                    xlWorkSheet.Cells[K, 6] = Ro.Cassette_2; // CassettetwoNotes
                    xlWorkSheet.Cells[K, 7] = Ac.FaceValue_12; // Denom2
                    // Cassette 3
                    xlWorkSheet.Cells[K, 8].NumberFormat = "#,##0";
                    xlWorkSheet.Cells[K, 8] = Ro.Cassette_3; // CassettethreeNotes 
                    xlWorkSheet.Cells[K, 9] = Ac.FaceValue_13; // Denom3
                    // Cassette 4
                    xlWorkSheet.Cells[K, 10].NumberFormat = "#,##0";
                    xlWorkSheet.Cells[K, 10] = Ro.Cassette_4; // CassettefourNotes 
                    xlWorkSheet.Cells[K, 11] = Ac.FaceValue_14; // Denom4

                    xlWorkSheet.Cells[K, 12].NumberFormat = "#,##0.00";
                    xlWorkSheet.Cells[K, 12] = Ro.NewAmount; // Total cash

                    xlWorkSheet.Cells[K, 13] = "To Be Defined"; //  Department
                    xlWorkSheet.Cells[K, 14] = DateTime.Now.Date; // Date 
                    xlWorkSheet.Cells[K, 15] = Ro.Operator; // Bank

                    K++;
                    //}

                    L++;

                }

                DateTime WDate = WDTm;

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


                string DateString = WDay + "." + WMonth + "." + WDate.Year; // eg ("17.11.2017")

                int Id1 = DateTime.Now.Hour;
                int Id2 = DateTime.Now.Minute;
                string Id3 = Id1.ToString() + "_" + Id2.ToString();

                // THIS is an XLS version 
                //xlWorkBook.SaveAs("d:\\NBG_GL_"+ Id3 + " "+ DateString + ".xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue,
                //                           misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue,
                //                           misValue, misValue, misValue, misValue);

                // This is a xlsx version

                strFullFilePathNoExt = "C:\\EXCELS_For_CIT\\AUDI_CIT_" + Id3 + " " + DateString + ".xlsx";
                xlWorkBook.SaveAs(strFullFilePathNoExt, Excel.XlFileFormat.xlOpenXMLWorkbook, misValue,
                misValue, false, false, Excel.XlSaveAsAccessMode.xlNoChange,
                Excel.XlSaveConflictResolution.xlUserResolution, true,
                misValue, misValue, misValue);

                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();

                Marshal.ReleaseComObject(xlWorkSheet);
                Marshal.ReleaseComObject(xlWorkBook);
                Marshal.ReleaseComObject(xlApp);

                MessageBox.Show("Excel file created , you can find the file in ..." + Environment.NewLine
                    + "...C:\\EXCELS_For_CIT\\AUDI_CIT_" + Id3 + " " + DateString + ".xlsx"
                    );
            }
            catch (Exception ex)
            {
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();

                Marshal.ReleaseComObject(xlWorkSheet);
                Marshal.ReleaseComObject(xlWorkBook);
                Marshal.ReleaseComObject(xlApp);

                CatchDetails(ex);
            }
        }
        // Print orders
        private void PrintOrders()
        {
            string P1 = "REPLENISHMENT ORDERS CYCLE:" + WOrdersCycle.ToString() + " TO CIT:" + WCitId;

            string P2 = WCitId;
            string P3 = WOrdersCycle.ToString();
            string P4 = WOperator;
            string P5 = WSignedId;

            Form56R69ATMS_Repl_Orders Report_Repl_Orders = new Form56R69ATMS_Repl_Orders(P1, P2, P3, P4, P5);
            Report_Repl_Orders.Show();
        }

        // Cancel
        private void Cancel_Click_1(object sender, EventArgs e)
        {
            this.Dispose();
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
    }
}
