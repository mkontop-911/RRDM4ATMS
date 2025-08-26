using System;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class UCForm271a_JCC : UserControl
    {

        //public event EventHandler ChangeBoardMessage;
        public string guidanceMsg;

        string WCcy;

        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();
        RRDMReconcCategATMsAtRMCycles RAtms = new RRDMReconcCategATMsAtRMCycles();

        RRDMErrorsClassWithActions Er = new RRDMErrorsClassWithActions();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

        RRDMReconcCateqoriesVsMatchingCategories RcMc = new RRDMReconcCateqoriesVsMatchingCategories();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        bool linkLabel1Alert, linkLabel2Alert, linkLabel3Alert, linkLabel4Alert, linkLabel5Alert, linkLabel6Alert; 

        bool ViewWorkFlow;
        string SelectionCriteria;
        int CallingMode;
        string WSortCriteria = "";
        string WBankId; 

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WCategoryId;
        int WRMCycle;

        public void UCForm271a_JCC_Par(string InSignedId, int InSignRecordNo, string InOperator, string InCategory, int InRMCycle)
        {

            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WCategoryId = InCategory;

            WRMCycle = InRMCycle;

            InitializeComponent();

            panelLightAlert.Hide(); 

            Us.ReadUsersRecord(WSignedId);
            WBankId = Us.BankId;
            RRDMBanks Ba = new RRDMBanks();
            Ba.ReadBank(WBankId); 
            WCcy = Ba.BasicCurName; 

            Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, WCategoryId, WRMCycle);
            //Rcs.ReadMatchingCategoriesSessionsByRunningJobNo(WOperator, WRMCycle);

            label4.Text = "RM CATEGORY : " + WCategoryId + "--" + Rcs.CategoryName + " ---- RM CYCLE : " + WRMCycle.ToString();

            if (WCategoryId == "EWB311")
            {
                label12.Text = "GL Balance";
            }

            // ................................
            // Handle View ONLY 
            // ''''''''''''''''''''''''''''''''
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.ProcessNo == 54 || Usi.ProcessNo == 55 || Usi.ProcessNo == 56)
            {
                ViewWorkFlow = true;
            }
            else
            {
                if (Usi.ProcessNo == 2) ViewWorkFlow = false;
            }

            if (ViewWorkFlow == false) // UPDATING  
            {
                // Matching is done but not Settled 
                SelectionCriteria = " WHERE Operator ='" + WOperator + "' AND RMCateg ='" + WCategoryId + "'"
                          + "  AND IsMatchingDone = 1 AND FastTrack = 0 "
                          + "  AND Matched = 0 AND SettledRecord = 0 " + " AND ActionType != '07' ";
                CallingMode = 2; // Updating 

                DateTime FromDt = NullPastDate;
                DateTime ToDt = NullPastDate;

                WSortCriteria = " ORDER By TerminalId";
                // The below call is not valid anymore 
                Mpa.UpdateExceptionIds(WOperator, WSignedId, CallingMode, SelectionCriteria, WSortCriteria, FromDt, ToDt,1);
            }    

        }

        // SHOW SCREEN 

        public void SetScreen()
        {
            //
            // LEFT 
            //
            Color Red = Color.Red;
            Color Black = Color.Black;

            RRDM_GL_Balances_For_Categories_And_Atms GL = new RRDM_GL_Balances_For_Categories_And_Atms();

            GL.Read_GL_Balances_And_FindTodaysAndYesterdaysBalance(WCategoryId); 

            if (GL.RecordFound == true)
            {
                label5.Text = "GL Date Time...: " + GL.Cut_Off_Date.ToString();
                label20.Text = "GL Account : " + GL.GL_AccountNo;
                label6.Text = "GL Openning Balance : " + GL.Todays_GL_Balance.ToString("#,##0.00");
                label7.Text = "GL Closing Balance : " + GL.Yesterdays_GL_Balance.ToString("#,##0.00");
            }
            else
            {
          //      MessageBox.Show("GL balances not found for this Category."); 
            }

            string SelectionCriteria = " WHERE  RMCateg ='"
                           + WCategoryId + "' AND MatchingAtRMCycle =" + WRMCycle
                           + " AND IsMatchingDone = 1 "
                           + " AND Matched = 0 ";

            //SelectionCriteria = " WHERE Operator ='" + WOperator + "' AND RMCateg ='"
            //           + WCategoryId + "' AND MatchingAtRMCycle =" + WRMCycle + " AND IsMatchingDone = 1 ";

            Mpa.ReadMatchingTxnsMasterPoolATMsTotals(SelectionCriteria,2);

            label13.Text = "Matched Trans Amt............ : " + Rcs.MatchedTransAmt.ToString("#,##0.00");
            label14.Text = "UNMatched Amt................ : " + Rcs.NotMatchedTransAmt.ToString("#,##0.00");
            //label26.Text = "Adjusted with default Actions : " + Rcs.SettledUnMatchedAmtDefault.ToString("#,##0.00");

            Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, WCategoryId, WRMCycle); 
            if (Rcs.RecordFound == true)
            {
                //MatchingCat01 = (string)rdr["MatchingCat01"];
                //MatchingCat01Updated = (bool)rdr["MatchingCat01Updated"];
                if (Rcs.MatchingCat01 != "")
                {
                    if (Rcs.MatchingCat01Updated == false)
                    {
                        labelCat1.ForeColor = Red;
                        labelNm1.ForeColor = Red;
                        linkLabel1Alert = true; 
                    }
                    labelCat1.Show();
                    labelNm1.Show();
                    linkLabel1.Show();
                    labelNm1.Text = Rcs.MatchingCat01;
                }
                if (Rcs.MatchingCat02 != "")
                {
                    if (Rcs.MatchingCat02Updated == false)
                    {
                        labelCat2.ForeColor = Red;
                        labelNm2.ForeColor = Red;
                        linkLabel2Alert = true;
                    }
                    labelCat2.Show();
                    labelNm2.Show();
                    linkLabel2.Show();
                    labelNm2.Text = Rcs.MatchingCat02;
                }

                if (Rcs.MatchingCat03 != "")
                {
                    if (Rcs.MatchingCat03Updated == false)
                    {
                        labelCat3.ForeColor = Red;
                        labelNm3.ForeColor = Red;
                        linkLabel3Alert = true;
                    }
                    labelCat3.Show();
                    labelNm3.Show();
                    linkLabel3.Show();
                    labelNm3.Text = Rcs.MatchingCat03;
                }
                if (Rcs.MatchingCat04 != "")
                {
                    if (Rcs.MatchingCat04Updated == false)
                    {
                        labelCat4.ForeColor = Red;
                        labelNm4.ForeColor = Red;
                        linkLabel4Alert = true;
                    }
                    labelCat4.Show();
                    labelNm4.Show();
                    linkLabel4.Show();
                    labelNm4.Text = Rcs.MatchingCat04;
                }
                if (Rcs.MatchingCat05 != "")
                {
                    if (Rcs.MatchingCat05Updated == false)
                    {
                        labelCat5.ForeColor = Red;
                        labelNm5.ForeColor = Red;
                        linkLabel5Alert = true;
                    }
                    labelCat5.Show();
                    labelNm5.Show();
                    linkLabel5.Show();
                    labelNm5.Text = Rcs.MatchingCat05;
                }
                if (Rcs.MatchingCat06 != "")
                {
                    if (Rcs.MatchingCat06Updated == false)
                    {
                        labelCat6.ForeColor = Red;
                        labelNm6.ForeColor = Red;
                        linkLabel6Alert = true;
                    }
                    labelCat6.Show();
                    labelNm6.Show();
                    linkLabel6.Show();
                    labelNm6.Text = Rcs.MatchingCat06;
                }


            }


            //
            // Right side
            //

            decimal TempTotal = Rcs.GlYesterdaysBalance + Rcs.MatchedTransAmt + Rcs.SettledUnMatchedAmtDefault;
            textBox1.Text = TempTotal.ToString("#,##0.00");
            textBox2.Text = Rcs.GlTodaysBalance.ToString("#,##0.00");
            textBox3.Text = (Rcs.GlTodaysBalance - TempTotal).ToString("#,##0.00");


            if ((Rcs.GlTodaysBalance - TempTotal) != 0)
            {
                label15.ForeColor = Red;
            }
            else
            {
                label15.ForeColor = Black;
            }

            textBox7.Text = (Mpa.TotalUnMatched - Mpa.TotalDefaultActionBySystem).ToString();

            textBox6.Text = Mpa.TotalAmountUnMatched.ToString("#,##0.00");

            textBoxMetaExcep.Text = Mpa.TotalActionsByUserDefaultAndManual.ToString();
            textBox4.Text = Mpa.TotalAmountByUserDefaultAndManual.ToString("#,##0.00");
            textBoxForcedMatchDisp.Text = (Mpa.TotalForcedMatched+ Mpa.TotalMoveToDisputeNumber).ToString();
            textBox5.Text = (Mpa.TotalForcedMatchedAmount + Mpa.TotalMoveToDisputeAmt).ToString("#,##0.00");

            textBoxEffect.Text = (Mpa.TotalAmountByUserDefaultAndManual + Mpa.TotalForcedMatchedAmount + Mpa.TotalMoveToDisputeAmt).ToString("#,##0.00");

            textBox8.Text = (Mpa.TotalAmountUnMatched - (Mpa.TotalAmountByUserDefaultAndManual + Mpa.TotalForcedMatchedAmount + Mpa.TotalMoveToDisputeAmt)).ToString("#,##0.00");
            
            //
            // FAST TRACK
            //

            RRDMGasParameters Gp = new RRDMGasParameters();
            Gp.ReadParametersSpecificId(WOperator, "221", "1", "", ""); // > is Red 
            int NotInJournalLimit = (int)Gp.Amount;

          
                if ((Mpa.TotalUnMatched - Mpa.TotalDefaultActionBySystem) > NotInJournalLimit)
                {
                   
                    //labelAlert.Show();
                    //pictureBox2.Show();
                    //textBoxBrokenDisc.Show();
                    //pictureBox2.BackgroundImage = appResImg.RED_LIGHT;
                }
           

            label17.Text = "FAST TRACK " ;

            //SelectionCriteria = " WHERE Operator ='" + WOperator + "' AND RMCateg ='"
            //      + WCategoryId + "' AND MatchingAtRMCycle =" + WRMCycle
            //      //+ " AND TerminalId ='" + WAtmNo + "'"
            //      + " AND IsMatchingDone = 1 ";

            //Mpa.ReadMatchingTxnsMasterPoolATMsTotals(SelectionCriteria);

            textBox11.Text = Mpa.TotalFastTrack.ToString();
            textBox13.Text = Mpa.TotalFastTrackAmount.ToString("#,##0.00");

            if (Mpa.TotalFastTrack > 0)
            {
                checkBoxFastTrack.Checked = true;
            }
            else
            {
                // Initialise Fast Track 
                checkBoxFastTrack.Checked = false;
            }

         

        }

      
        private void buttonFastTrack_Click(object sender, EventArgs e)
        {
            //Form271FastTrack
            //    (string InSignedId, int InSignRecordNo,
            //    string InOperator, string InReconcCategory, 
            //    int InWRMCycleNo, string InAtmNo)

            Form271FastTrack NForm271FastTrack;
             
            NForm271FastTrack = new Form271FastTrack(WSignedId, WSignRecordNo, WOperator, WCategoryId, WRMCycle, "ALL");
            NForm271FastTrack.FormClosed += NForm271FastTrack_FormClosed;
            NForm271FastTrack.ShowDialog();

         
        }

        void NForm271FastTrack_FormClosed(object sender, FormClosedEventArgs e)
        {
            //
            // Read all Totals By type
            //
            //string SelectionCriteria = " WHERE Operator ='" + WOperator 
            //      + "' AND RMCateg ='" + WCategoryId + "' "
            //       +" AND MatchingAtRMCycle = " + WRMCycle
            //       //+ " AND MatchingAtRMCycle = " + WRMCycle
            //       + " AND IsMatchingDone = 1 ";

            //string SelectionCriteria = " WHERE  RMCateg ='"
            //             + WCategoryId + "' AND MatchingAtRMCycle =" + WRMCycle
            //             + " AND IsMatchingDone = 1 "
            //             + " AND Matched = 0 ";


            //Mpa.ReadMatchingTxnsMasterPoolATMsTotals(SelectionCriteria);

            //textBox11.Text = Mpa.TotalFastTrack.ToString();
            //textBox13.Text = Mpa.TotalFastTrackAmount.ToString("#,##0.00");

            SetScreen();

            //dataGridView1.Rows[WRowIndex].Selected = true;
            //dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

            //dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;


        }
      
        // Show Matching Files 

        RRDMMatchingCategoriesVsSourcesFiles Mcf = new RRDMMatchingCategoriesVsSourcesFiles();
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (linkLabel1Alert == true)
            {
                MessageBox.Show("Please note that the matching of this Matching Category was not Done."); 
            }

            Form2Grid NForm2Grid;
            string Header = "Matching Files For: " + labelNm1.Text;

            string SelectionCriteria = " WHERE CategoryId ='" + labelNm1.Text + "'";
            Mcf.ReadReconcCategoryVsSourcesANDFillTableofFiles(SelectionCriteria);

            NForm2Grid = new Form2Grid(Header, Mcf.RMCategoryFilesDataFiles);
            NForm2Grid.Show();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (linkLabel2Alert == true)
            {
                MessageBox.Show("Please note that the matching of this Matching Category was not Done.");
            }
            Form2Grid NForm2Grid;
            string Header = "Matching Files For: " + labelNm2.Text;

            string SelectionCriteria = " WHERE CategoryId ='" + labelNm2.Text + "'";
            Mcf.ReadReconcCategoryVsSourcesANDFillTableofFiles(SelectionCriteria);

            NForm2Grid = new Form2Grid(Header, Mcf.RMCategoryFilesDataFiles);
            NForm2Grid.Show();
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (linkLabel3Alert == true)
            {
                MessageBox.Show("Please note that the matching of this Matching Category was not Done.");
            }
            Form2Grid NForm2Grid;
            string Header = "Matching Files For: " + labelNm3.Text;

            string SelectionCriteria = " WHERE CategoryId ='" + labelNm3.Text + "'";
            Mcf.ReadReconcCategoryVsSourcesANDFillTableofFiles(SelectionCriteria);

            NForm2Grid = new Form2Grid(Header, Mcf.RMCategoryFilesDataFiles);
            NForm2Grid.Show();
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (linkLabel4Alert == true)
            {
                MessageBox.Show("Please note that the matching of this Matching Category was not Done.");
            }
            Form2Grid NForm2Grid;
            string Header = "Matching Files For: " + labelNm4.Text;

            string SelectionCriteria = " WHERE CategoryId ='" + labelNm4.Text + "'";
            Mcf.ReadReconcCategoryVsSourcesANDFillTableofFiles(SelectionCriteria);

            NForm2Grid = new Form2Grid(Header, Mcf.RMCategoryFilesDataFiles);
            NForm2Grid.Show();
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (linkLabel5Alert == true)
            {
                MessageBox.Show("Please note that the matching of this Matching Category was not Done.");
            }
            Form2Grid NForm2Grid;
            string Header = "Matching Files For: " + labelNm5.Text;

            string SelectionCriteria = " WHERE CategoryId ='" + labelNm5.Text + "'";
            Mcf.ReadReconcCategoryVsSourcesANDFillTableofFiles(SelectionCriteria);

            NForm2Grid = new Form2Grid(Header, Mcf.RMCategoryFilesDataFiles);
            NForm2Grid.Show();
        }

        private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (linkLabel6Alert == true)
            {
                MessageBox.Show("Please note that the matching of this Matching Category was not Done.");
            }
            Form2Grid NForm2Grid;
            string Header = "Matching Files For: " + labelNm6.Text;

            string SelectionCriteria = " WHERE CategoryId ='" + labelNm6.Text + "'";
            Mcf.ReadReconcCategoryVsSourcesANDFillTableofFiles(SelectionCriteria);

            NForm2Grid = new Form2Grid(Header, Mcf.RMCategoryFilesDataFiles);
            NForm2Grid.Show();
        }
        //
        // Fast Track 
        private void checkBoxFastTrack_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxFastTrack.Checked == true)
            {
                panel5.Show();
            }
            else
            {
                panel5.Hide();
            }
        }
    
        // Test 
        private void buttonPrint_Click(object sender, EventArgs e)
        {
            // Matching is done but not Settled 
            SelectionCriteria = " WHERE Operator ='" + WOperator + "' AND RMCateg ='" + WCategoryId + "'"
                      + "  AND MatchingAtRMCycle =" + WRMCycle
                      + " AND IsMatchingDone = 1 AND Matched = 0  "
                      //+ " AND IsMatchingDone = 1 AND Matched = 0 AND SettledRecord = 0 "
                      + " AND ActionType != '07' ";

            WSortCriteria = "Order By TerminalId, SeqNo "; 

            Mpa.ReadMatchingTxnsMasterPoolAndFillTableFastTrack(WOperator, WSignedId, SelectionCriteria,
                                                                                     WSortCriteria,1);
  
            string P1 = "Transactions For Reconciliation :" + WCategoryId + " AND Cycle : " + WRMCycle.ToString();

            string P2 = "";
            string P3 = "";
            if (ViewWorkFlow == true)
            {

                RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

                Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WCategoryId, WRMCycle, "ReconciliationCat");

                if (Ap.RecordFound == true)
                {
                    Us.ReadUsersRecord(Ap.Requestor);
                    P2 = Us.UserName;
                    Us.ReadUsersRecord(Ap.Authoriser);
                    P3 = Us.UserName;
                }
                else
                {
                    //ReconciliationAuthorNoRecordYet = true;
                }

            }
            else
            {
                Us.ReadUsersRecord(WSignedId);
                P2 = Us.UserName;
                P3 = "N/A";
            }

            string P4 = WBankId;
            string P5 = WSignedId;

            Form56R55ATMS ReportATMS55 = new Form56R55ATMS(P1, P2, P3, P4, P5);
            ReportATMS55.Show();
        }
        // Show Screen
        // Show Alert Lines 
        bool ShowAlertLines;
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            ShowAlertLines = true;
            SetScreen();
        }
    }
}
