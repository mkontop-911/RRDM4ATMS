using System;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Configuration;
// Alecos
using System.Diagnostics;


namespace RRDM4ATMsWin
{
    public partial class Form200 : Form
    {
        // Variables

        Form54 NForm54;
        Form55 NForm55;

        Form22MIS NForm22MIS; 
        Form24 NForm24; 

        public bool Prive;
       
        int WAction;

        string WBankId; 

        // DATATable for Grid 
        // public DataTable GridDays = new DataTable();

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        string MsgFilter;
     //   string WAtmNo; 

        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();

      //  DepositsClass Dc = new DepositsClass();

        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate(); 

        RRDMReplStatsClass Rs = new RRDMReplStatsClass();

        RRDMBanks Ba = new RRDMBanks();

        RRDMAtmsClass Ac = new RRDMAtmsClass(); 

        RRDMUsersAccessToAtms Ua = new RRDMUsersAccessToAtms();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMCounterClass Cc = new RRDMCounterClass();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        DateTime NullFutureDate = new DateTime(2050, 11, 21);

        RRDMControllerMsgClass Cm = new RRDMControllerMsgClass();

        RRDMCaseNotes Cn = new RRDMCaseNotes();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMMatchingCategories Rc = new RRDMMatchingCategories();

        string WSignedId;
        int WSignRecordNo;
        string WSecLevel;
        string WOperator;
  
        string WCitId; 

        // Methods 
        // READ ATMs Main
        // 
        public Form200(string InSignedId, int SignRecordNo, string InSecLevel, string InOperator, string InCitId)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;
       
            WCitId = InCitId;

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

            textBoxMsgBoard.Text = "The total picture of the current ATM work is showed ";

            Us.ReadUsersRecord(WSignedId);
            WBankId = Us.BankId;

            // 
            //TEST
            // Physical Inspection results 
            // 

            // How many 
            string WParameter4 = "Physical Inspection";
            string Order = "Ascending";
            string SearchP4 = "Physical Inspection";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, SearchP4);
            if (Cn.RecordFound == true)
            {
            }
           
            // Find Total and Fill table for all Notes
            string SelectionCriteria = " WHERE Parameter2 = 'PhysicalInspection' ";
            Cn.ReadAllCaseNotesAndFillTableBySelectionCriteria(SelectionCriteria);

            textBox26.Text = Cn.TotalSelected.ToString();

            textBox27.Text = Cn.TotalNotes.ToString() ; 

            //*****************************************
            //*****************************************

            if (WCitId !="1000")
            {
                Us.ReadUsersRecord(WCitId);

                labelStep1.Text = "Today's Repl. and Reconc. Status For " + Us.UserName;
            }
            if (WCitId == "1000")
            {
                Ba.ReadBanksForOperator(WOperator);

                labelStep1.Text = "Today's Repl. and Reconc. Status For " + Ba.BankName;
            }

            // ....
            // ....
            // Call Procedures to see if serious message

        //    WAtmNo = "";

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

           
                MessageBox.Show("No Testing Data to show");
                return; 
            

           // MAIN PROGRAM 
            //TESTING
            DateTime WReplDate = DateTime.Today;
            WReplDate = new DateTime(2014, 04, 18); // IF NOT TESTING THIS IS TODAY
            
          //  Read Counted totals 

            Cc.ReadAtmsMainTotals(WReplDate, WOperator, WCitId, 0); 

            textBox4.Text = Cc.TotAtms.ToString();
            textBox1.Text = (Cc.TotRepl+Cc.TotNotRepl).ToString();
            textBox5.Text = Cc.TotRepl.ToString();
            textBox15.Text = Cc.TotNotRepl.ToString();
            textBox25.Text = Cc.TotUnderRepl.ToString();

            if ((Cc.TotRepl + Cc.TotNotRepl + Cc.TotUnderRepl) > 0)
            {
                textBox12.Text = (Cc.TotRepl * 100 / (Cc.TotRepl + Cc.TotNotRepl + Cc.TotUnderRepl)).ToString();
                textBox13.Text = (Cc.TotNotRepl * 100 / (Cc.TotRepl + Cc.TotNotRepl + Cc.TotUnderRepl)).ToString();
                textBox22.Text = (Cc.TotUnderRepl * 100 / (Cc.TotRepl + Cc.TotNotRepl + Cc.TotUnderRepl)).ToString(); 
            }
            
            Rs.ReadReplStatClassMinAndMax(WReplDate);

            textBox6.Text = Rs.MinReplMinutes.ToString();
            textBox3.Text = Rs.MaxReplMinutes.ToString();
            textBox7.Text = Rs.AvgReplMinutes.ToString();

           //
           // RECONCILIATION 
           //
            textBox21.Text = Cc.TotReconc.ToString();

            textBox20.Text = Cc.TotNotReconc1.ToString();
            textBox17.Text = Cc.TotNotReconc2.ToString();
           // Percentages 

            if ((Cc.TotReconc + Cc.TotNotReconc1 + Cc.TotNotReconc2) > 0)
            {
                textBox2.Text = (Cc.TotReconc * 100 / (Cc.TotReconc + Cc.TotNotReconc1 + Cc.TotNotReconc2)).ToString();
                textBox16.Text = (Cc.TotNotReconc1 * 100 / (Cc.TotReconc + Cc.TotNotReconc1 + Cc.TotNotReconc2)).ToString();
                textBox14.Text = (Cc.TotNotReconc2 * 100 / (Cc.TotReconc + Cc.TotNotReconc1 + Cc.TotNotReconc2)).ToString();
            }


            textBox10.Text = Cc.TotErrors.ToString();
            textBox9.Text = Cc.TotErrorsAtm.ToString();

            textBox11.Text = Cc.TotErrorsHost.ToString();
            textBox8.Text = Cc.TotHostToday.ToString();

           // Percentages 
            if (Cc.TotErrors > 0)
            {
                textBox24.Text = (Cc.TotErrorsAtm * 100 / (Cc.TotErrors)).ToString();
                textBox23.Text = (Cc.TotErrorsHost * 100 / (Cc.TotErrors)).ToString();
            }

            Ba.ReadBanksForOperator(WOperator); 

            label22.Text = label22.Text + " " + Ba.BasicCurName;
            label16.Text = label16.Text + " " + Ba.BasicCurName;

            textBox19.Text = Cc.TotDiffPlus.ToString();
            textBox18.Text = Cc.TotDiffMinus.ToString();

            Rc.ReadMatchingCategoriesForAllocation(WOperator);

            textBox35.Text = (Rc.TotalReconc + Rc.TotalNotReconc).ToString();
            textBox34.Text = Rc.TotalMatchingDone.ToString(); ;
            textBox33.Text = Rc.TotalMatchingNotDone.ToString();
     

            Gp.ReadParametersSpecificId(WOperator, "603", "9", "", ""); // < is Green 
            int QualityRange1 = (int)Gp.Amount;

            Gp.ReadParametersSpecificId(WOperator, "604", "9", "", ""); // > is Red 
            int QualityRange2 = (int)Gp.Amount;

            if (Rc.TotalMatchingNotDone <= QualityRange1)
            {
                // Green
                pictureBox2.BackgroundImage = appResImg.GREEN_LIGHT_Repl;
            }

            if (Rc.TotalMatchingNotDone >= QualityRange2)
            {
                // Red 
                pictureBox2.BackgroundImage = appResImg.RED_LIGHT_Repl;
            }
            if (Rc.TotalMatchingNotDone > QualityRange1 & Rc.TotalUnMatchedRecords < QualityRange2)
            {
                // Yellow 
                pictureBox2.BackgroundImage = appResImg.YELLOW_Repl;
            }

            textBox28.Text = (Rc.TotalReconc + Rc.TotalNotReconc).ToString();
            textBox29.Text = Rc.TotalReconc.ToString();  ;
            textBox30.Text = Rc.TotalNotReconc.ToString();
            textBox31.Text = Rc.TotalUnMatchedRecords.ToString();

            Gp.ReadParametersSpecificId(WOperator, "603", "10", "", ""); // < is Green 
            QualityRange1 = (int)Gp.Amount;

            Gp.ReadParametersSpecificId(WOperator, "604", "10", "", ""); // > is Red 
            QualityRange2 = (int)Gp.Amount;

            if (Rc.TotalUnMatchedRecords <= QualityRange1)
            {
                // Green
                pictureBox3.BackgroundImage = appResImg.GREEN_LIGHT_Repl;
            }

            if (Rc.TotalUnMatchedRecords >= QualityRange2)
            {
                // Red 
                pictureBox3.BackgroundImage = appResImg.RED_LIGHT_Repl;
            }
            if (Rc.TotalUnMatchedRecords > QualityRange1 & Rc.TotalUnMatchedRecords < QualityRange2)
            {
                // Yellow 
                pictureBox3.BackgroundImage = appResImg.YELLOW_Repl;
            }
            //
            //
            // REPLENISHED - Chart1

            int[] yvalues1 = { Cc.TotRepl, Cc.TotNotRepl };
            string[] xvalues1 = { "Yes", "No" };

            // Set series members names for the X and Y values 
            chart1.Series[0].Points.DataBindXY(xvalues1, yvalues1);

           // RECONCILED - Chart2

            int[] yvalues2 = { Cc.TotReconc, Cc.TotNotReconc1, Cc.TotNotReconc2 };
            string[] xvalues2 = { "Yes", "No Today", "No old" };

            // Set series members names for the X and Y values 
            chart2.Series[0].Points.DataBindXY(xvalues2, yvalues2);

        }
     
        // SHOW ALL ABOVE AVERAGE
        private void button10_Click(object sender, EventArgs e)
        {
            //
            // SELECT ProductName, Price FROM Products
            // WHERE Price>(SELECT AVG(Price) FROM Products); 
            //
        }

        // Go to not REPL LISTING 

        private void button2_Click(object sender, EventArgs e)
        {
            WAction = 1;
            NForm22MIS = new Form22MIS(WSignedId, WSignRecordNo, WOperator, "", NullFutureDate , WCitId,"",WAction);
            NForm22MIS.ShowDialog();

        }

        // SHOW THE REPLENISHED BY USER 

        private void button3_Click(object sender, EventArgs e)
        {
            WAction = 2;
            NForm22MIS = new Form22MIS(WSignedId, WSignRecordNo, WOperator, "", NullFutureDate, WCitId,"", WAction);
            NForm22MIS.ShowDialog();
           
        }
        //
        // SHOW THE ONES CURRENTLY UNDER REPLENISHMENT 
        //
        private void button1_Click(object sender, EventArgs e)
        {
            WAction = 3;
            NForm22MIS = new Form22MIS(WSignedId, WSignRecordNo, WOperator, "", NullFutureDate, WCitId, "", WAction);
            NForm22MIS.ShowDialog();
        }

        // Not Reconciled = 1

        private void button4_Click(object sender, EventArgs e)
        {
            WAction = 11;
            NForm22MIS = new Form22MIS(WSignedId, WSignRecordNo, WOperator, "", NullFutureDate, WCitId,"", WAction);
            NForm22MIS.ShowDialog();

        }

        // Not Reconciled > 1


        private void button9_Click(object sender, EventArgs e)
        {
            WAction = 12;
            NForm22MIS = new Form22MIS(WSignedId, WSignRecordNo, WOperator, "", NullFutureDate,WCitId,"", WAction);
            NForm22MIS.ShowDialog();

        }
     
        // ATM ERRORS 
        private void button6_Click(object sender, EventArgs e)
        {
            bool Replenishment = false;
            string InFilter = "Operator ='" + WOperator + "'" + " AND (ErrType = 1) AND OpenErr =1 AND CitId ='" + WCitId + "'";
            NForm24 = new Form24(WSignedId, WSignRecordNo, WOperator, "", 0,"", Replenishment, InFilter);
            NForm24.ShowDialog();

       //     (int InSignedId, int InSignRecordNo, string InBankId, bool InPrive, string InAtmNo, int InSesNo,
        //    string CurrNm, bool Replenishment, string InFilter)
        }
        // HOST ERRORS 
        private void button7_Click(object sender, EventArgs e)
        {
            bool Replenishment = false;
            string InFilter = "Operator ='" + WOperator + "'" + " AND (ErrType = 2) AND OpenErr =1 AND CitId ='" + WCitId + "'";
            NForm24 = new Form24(WSignedId, WSignRecordNo, WOperator,"", 0, "", Replenishment, InFilter);
            NForm24.ShowDialog();
        }
        

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
        // Show probelmatic atms during physical inspection. 
        private void button5_Click(object sender, EventArgs e)
        {
            //
            // Check for Google License 
            //
            string ParId = "263";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
#if DEBUG
#endif
            if (Gp.OccuranceNm == "YES") // Electronic needed 
            {
              
            }
            else
            {
                MessageBox.Show("Need  Of Google Maps Lisence ");
                return; 
            }

            RRDMTempAtmsLocation Tl = new RRDMTempAtmsLocation();

            // READ NOTES TABLE AND Insert in Temp ATMs Location
            int WGroupNo = 25 ; 
            Tl.DeleteTempAtmLocationGroup(WSignedId, WGroupNo); 

            int I = 0;
            string PreviousAtm = ""; 
            
            while (I <= (Cn.NotesDataTable.Rows.Count - 1))
            {
                int SeqNo = (int)Cn.NotesDataTable.Rows[I]["SeqNo"];
                string Parameter3 = (string)Cn.NotesDataTable.Rows[I]["Parameter3"];
                string Parameter4 = (string)Cn.NotesDataTable.Rows[I]["Parameter4"];

                string WAtmNo = Parameter3;

                if (WAtmNo != PreviousAtm) // Create Entry for sictict ATM
                {

                    RRDMAtmsClass Ac = new RRDMAtmsClass();
                    Ac.ReadAtm(WAtmNo);

                    Tl.UserId = WSignedId; 
                    Tl.AtmNo = WAtmNo; 
                    Tl.BankId = WOperator;
                    Tl.Mode = 2;

                    Tl.GroupNo = WGroupNo;
                    Tl.GroupDesc = "Show ATMs With Physical Problems ";

                    Tl.DtTmCreated = DateTime.Now;

                    Tl.Street = Ac.Street;
                    Tl.Town = Ac.Town;
                    Tl.District = Ac.District;
                    Tl.PostalCode = Ac.PostalCode;
                    Tl.Country = Ac.Country;

                    Tl.Latitude = Ac.Latitude;
                    Tl.Longitude = Ac.Longitude;

                    Tl.ColorId = "2";
                    Tl.ColorDesc = "Normal Color";

                    // Insert In Maps 

                    Tl.SeqNo = Tl.InsertTempAtmLocationRecord();

                    PreviousAtm = WAtmNo; 
                }

                I++; // Read Next entry of the table 

            }

            //TEST
            /* New code for RRDMMaps... */
            // Read the URL from app.config
            string ATMGroupNoURL = ConfigurationManager.AppSettings["RRDMMapsGroupNoURL"];
            // Format the URL with the query string (ATMId=#)
            string GrpDescr = "ATMs with Problems!";
            string QueryURL = ATMGroupNoURL + "?GroupNo=" + WGroupNo +"&UserId="+ WSignedId + "&GroupDescr=" + GrpDescr;

            // Invoke default browser
            ProcessStartInfo sInfo = new ProcessStartInfo(QueryURL);
            Process.Start(sInfo);

            // MessageBox.Show("Shows on Google Maps the problematic ATMs. ");
            return; 
        }

        // Read all Notes with physical Inspection Notes 
        private void button8_Click(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = ""; 
            string WParameter4 = "Physical Inspection";
            string SearchP4 = "Physical Inspection";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, "Read", SearchP4);
            NForm197.ShowDialog();
        }
// SHOW DETAILS OF MATCHING STATUS OF RM CATEGORIES 
        private void button10_Click_1(object sender, EventArgs e)
        {
            Form200b NForm200b;

            string WFunction = "MATCHINGSTATUS";
            NForm200b = new Form200b(WSignedId, WSignRecordNo, WOperator, WFunction);
            NForm200b.ShowDialog();
        }
// Show Reconciliation Details 
        private void button11_Click(object sender, EventArgs e)
        {
           Form80a NForm80aITMX;
            string WFunction = "View";
            string Category = "All";

            string WhatBank = WBankId;

            NForm80aITMX = new Form80a(WSignedId, WSignRecordNo, WOperator, WFunction, Category, WhatBank);

            NForm80aITMX.ShowDialog();
        }
// Finish
        private void buttonNext_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }
    }
}
