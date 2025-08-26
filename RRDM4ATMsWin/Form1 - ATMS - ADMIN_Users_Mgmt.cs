using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Threading;

//24-04-2015 Alecos
using System.Diagnostics;
using System.Text;

//multilingual
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form1ATMs_ADMIN_Users_Mgmt : Form
    {
        public EventHandler LoggingOut;

        Form13_NEW NForm13_NEW; // Users FORM
     
        string WBankId;

       Form54 NForm54; // DO not Delete
       Form55 NForm55; // Do not Delete

        Form112 NForm112;

        bool CheckBoxAllowMsgsWithin;

        string W_Application; 

        string MsgFilter;

        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMGasParameters Gp = new RRDMGasParameters();
        RRDMControllerMsgClass Cm = new RRDMControllerMsgClass();
        RRDMCaseNotes Cn = new RRDMCaseNotes();
        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();
     
        string WOrigin = "Our Atms";
        string WApplication; 

        // NOTES 
        string Order = "Descending";
        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WParameter4 = "Main Menu - Issues For System";
        string WSearchP4 = "";

        string WSelectionCriteria;

        bool IsMaker;
        bool IsChecker;

        int WAction;

        string WSignedId;
        int WSignRecordNo;
        string WSecLevel;
        string WOperator;

        public Form1ATMs_ADMIN_Users_Mgmt(string InSignedId, int SignRecordNo, string InSecLevel, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;

            InitializeComponent();

            //labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;

            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.RecordFound == true)
            {
                W_Application = Usi.SignInApplication;

                if (W_Application == "e_MOBILE")
                {
                    if (Usi.WFieldNumeric11 == 11)
                    {
                        W_Application = "ETISALAT";
                    }
                    if (Usi.WFieldNumeric11 == 12)
                    {
                        W_Application = "QAHERA";
                    }
                    if (Usi.WFieldNumeric11 == 13)
                    {
                        W_Application = "IPN";
                    }
                    labelStep1.Text = "Configuration Menu-Mobile_" + W_Application;
                   
                }
                else
                {
                    //buttonCatForMobile.Hide();
                    //buttonMatchingCateg.Show();
                }
            }

          

            Us.ReadUsersRecord_Tail_Info(WSignedId); 

            if (WSignedId == "ADMIN-BDC")
            {
                labelStep1.Text = "Creation of Maker and Checker by :.." + WSignedId;
                ForMakerAndChecker.Hide(); 
            }

            if (Us.User_Is_Maker == true)
            {
                IsMaker = true;
                labelStep1.Text = "User Management by Maker:.."+ WSignedId ;
                panelForAdmin.Hide(); 
            }
            if (Us.User_Is_Checker == true)
            {
                IsChecker = true;
                labelStep1.Text = "MENU For User Management by Checker:.." + WSignedId;
                panelForAdmin.Hide();
            }

            Us.ReadUsersRecord(WSignedId);
            WBankId = Us.BankId;

            

            MsgFilter =
                   "(ReadMsg = 0 AND ToAllAtms = 1)"
               + " OR (ReadMsg = 0 AND ToUser='" + WSignedId + "')";


            Cm.ReadControlerMSGsSerious(MsgFilter);

            if (Cm.SerMsgCount > 0)
            {
                string messagesStatus = " You have " + Cm.SerMsgCount.ToString() + " personal messages from the controller.";

                //toolTipMessages.SetToolTip(button51, messagesStatus);
                //toolTipMessages.ShowAlways = true;

            }
            else
            {
                //toolTipMessages.SetToolTip(button51, "Check messages from controller.");
                //toolTipMessages.ShowAlways = true;
            }

         //   toolTipController.SetToolTip(button52, "Communicate with today's controller.");

            checkBox1.Checked = false;
         
        }
      
        DateTime WCut_Off_Date;
        string WJobCategory = "ATMs";
        int WReconcCycleNo;
        private void FormMainScreen_Load(object sender, EventArgs e)
        {
            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);
            WCut_Off_Date = Rjc.Cut_Off_Date;

            labelCycleNo.Text = WReconcCycleNo.ToString();
            labelCutOff.Text = Rjc.Cut_Off_Date.ToShortDateString();
            
            // ****************************************
            // Records for authorisation for this user
            // ****************************************
            //Ap.ReadAuthorizationsUserTotal(WSignedId);
            //if (Ap.RecordFound == true)
            //{
            //    //labelAuthRecords.Text = Ap.TotalNumberforUser.ToString();
            //    //checkBoxAuthRecords.Show();
            //    //checkBoxAuthRecords.Checked = true;
            //}
            //else checkBoxAuthRecords.Hide();

            
            string AtmNo = "";
            string FromFunction = "";

            FromFunction = "General";

        
        }

        // Log Out 
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Go to FORM40
            //LoggingOut(sender, e);
            if (WSignedId == "ADMIN-BDC")
            {
                this.Dispose();
            }
            else
            {
                LoggingOut(sender, e);
            }
            
        }

        // Form Closed

        private void FormMainScreen_FormClosed(object sender, FormClosedEventArgs e)
        {
            LoggingOut(sender, e);
        }
        // Disable / enable Ballon Info 
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

      

        void NForm152_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormMainScreen_Load(this, new EventArgs());
        }


        // AUthorize Decisions 

        private void button75_Click(object sender, EventArgs e)
        {
            string TempAtmNo = "";
            int TempSesNo = 0;
            int WDisputeNo = 0;
            int WDisputeTranNo = 0;
            NForm112 = new Form112(WSignedId, WSignRecordNo, WOperator, "Normal", TempAtmNo, TempSesNo, WDisputeNo, WDisputeTranNo, "", 0);
            NForm112.FormClosed += NForm112_FormClosed;
            NForm112.ShowDialog();
        }

        void NForm112_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormMainScreen_Load(this, new EventArgs());
        }

        void NForm63_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormMainScreen_Load(this, new EventArgs());
        }

        // Message from Controller 

        private void button51_Click(object sender, EventArgs e)
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

                //toolTipMessages.SetToolTip(button51, messagesStatus);
                //toolTipMessages.ShowAlways = true;

            }
            else
            {

                //toolTipMessages.SetToolTip(button51, "Check messages from controller.");
                //toolTipMessages.ShowAlways = true;
            }
        }
        
        // Todays Controller 

        private void button52_Click(object sender, EventArgs e)
        {
            NForm54 = new Form54(WSignedId, WSignRecordNo, WOperator);
            NForm54.ShowDialog();
        }


    
        private void tableLayoutPanel4_Paint(object sender, PaintEventArgs e)
        {

        }


        int NumberMsgs = 0;

      
        private void labelNumberMsgs_Click(object sender, EventArgs e)
        {

        }
    
     
      
        // EXIT 
        private void button72_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        void NForm19_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormMainScreen_Load(this, new EventArgs());
        }

        
        // Software version details 
        private void button6_Click(object sender, EventArgs e)
        {

            AboutBox1 about = new AboutBox1();
            about.ShowDialog();

        }
       
       
     

        // Timer two 
        private void timer2_Tick(object sender, EventArgs e)
        {
            // It operates every ten seconds 

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
   
     

// All Outstanding 
        private void linkLabelOutstanding_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

            string WJobCategory = "ATMs";
            int WReconcCycleNo;

            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);
            DateTime WCut_Off_Date = Rjc.Cut_Off_Date;

            DateTime NullPastDate = new DateTime(1900, 01, 01);
            Form67_Cycle_Rich_Picture NForm67_Cycle_Rich_Picture;

            int Mode = 4; // All Outstanding for Repl 
            string WAtmNo = "";
            int WSesNo = 0;
            NForm67_Cycle_Rich_Picture = new Form67_Cycle_Rich_Picture(WSignedId, WOperator, WAtmNo, WSesNo
                                   , WReconcCycleNo, NullPastDate, NullPastDate, Mode);
            NForm67_Cycle_Rich_Picture.ShowDialog();
        }
// Link Replenished and Authorised by this person 
        private void linkLabelAuthorised_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

            string WJobCategory = "ATMs";
            int WReconcCycleNo;

            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);
            DateTime WCut_Off_Date = Rjc.Cut_Off_Date;

            DateTime NullPastDate = new DateTime(1900, 01, 01);

            Form67_Cycle_Rich_Picture NForm67_Cycle_Rich_Picture;

            int Mode = 7; // All Repl this cycle by user 
            string WAtmNo = "";
            int WSesNo = 0;
            NForm67_Cycle_Rich_Picture = new Form67_Cycle_Rich_Picture(WSignedId, WOperator, WAtmNo, WSesNo
                , WReconcCycleNo, NullPastDate, NullPastDate, Mode);
            NForm67_Cycle_Rich_Picture.ShowDialog();
        }
// this cycle only
        private void linkLabelThisCycleOnly_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

            string WJobCategory = "ATMs";
            int WReconcCycleNo;

            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);
            DateTime WCut_Off_Date = Rjc.Cut_Off_Date;

            DateTime NullPastDate = new DateTime(1900, 01, 01);
            Form67_Cycle_Rich_Picture NForm67_Cycle_Rich_Picture;

            int Mode = 9; // All Outstanding for Repl 
            string WAtmNo = "";
            int WSesNo = 0;
            NForm67_Cycle_Rich_Picture = new Form67_Cycle_Rich_Picture(WSignedId, WOperator, WAtmNo, WSesNo
                                   , WReconcCycleNo, NullPastDate, NullPastDate, Mode);
            NForm67_Cycle_Rich_Picture.ShowDialog();
        }
// GO to USERS
        private void buttonUsers_Click(object sender, EventArgs e)
        {
            // Sixth which is zero is CitId
            DateTime LimitDate = new DateTime(2025, 05, 10);

            if (DateTime.Now > LimitDate)
            {
                MessageBox.Show("Limit date 0f 2025_05_10 has been reach");
                return;
            }
            bool IsForCIT = false; 
            NForm13_NEW = new Form13_NEW(WSignedId, WSignRecordNo, WOperator, "1000", 1, IsForCIT);
            NForm13_NEW.ShowDialog();
        }
// AUDIT TRAIL 
        private void buttonAuditTrail_Click(object sender, EventArgs e)
        {
            Form76_NEW NForm76_NEW = new Form76_NEW(WSignedId, WOperator,"");
            NForm76_NEW.ShowDialog(); 
        }
// ADMIN WORK
        private void buttonAdminWork_Click(object sender, EventArgs e)
        {
            DateTime LimitDate = new DateTime(2025, 05, 10);

            if (DateTime.Now > LimitDate)
            {
                MessageBox.Show("Limit date 0f 2025_05_10 has been reach");
                return;
            }
            bool IsForCIT = false;
            NForm13_NEW = new Form13_NEW(WSignedId, WSignRecordNo, WOperator, "1000", 1, IsForCIT);
            NForm13_NEW.ShowDialog();
        }

        private void buttonCITs_Click(object sender, EventArgs e)
        {
            DateTime LimitDate = new DateTime(2025, 05, 10);

            if (DateTime.Now > LimitDate)
            {
                MessageBox.Show("Limit date 0f 2025_05_10 has been reach");
                return;
            }
            bool IsForCIT = true;
            NForm13_NEW = new Form13_NEW(WSignedId, WSignRecordNo, WOperator, "1000", 1, IsForCIT);
            NForm13_NEW.ShowDialog();
        }
    }
}
