using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
//multilingual
using System.Resources;
using System.Globalization;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class UCForm51d2 : UserControl
    {
        Decimal CashInAmount;

        //    DateTime WReplDate;

        //   decimal WCurrentBal;

        public event EventHandler ChangeBoardMessage;
        public string guidanceMsg;

        //string WHolidaysVersion;
        bool ViewWorkFlow; 

        RRDMAtmsClass Ac = new RRDMAtmsClass(); 
        RRDMNotesBalances Na = new RRDMNotesBalances(); // Class Notes 
        RRDMGasParameters Gp = new RRDMGasParameters();
        RRDMHolidays Ch = new RRDMHolidays();
        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();
        RRDMReplActionsClass Ra = new RRDMReplActionsClass(); 

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WAtmNo;
        int WSesNo;

        public void UCForm51d2Par(string InSignedId, int InSignRecordNo, string InOperator, string InAtmNo, int InSesNo)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WAtmNo = InAtmNo;
            WSesNo = InSesNo;

            InitializeComponent();     

            try
            {
                // ................................
                // Handle View ONLY 
                // ''''''''''''''''''''''''''''''''
                Us.ReadSignedActivityByKey(WSignRecordNo);

                if (Us.ProcessNo == 54 || Us.ProcessNo == 55 || Us.ProcessNo == 56)
                {
                    ViewWorkFlow = true;

                    buttonUpdate.Hide(); 
                }
                //TEST
                if (WAtmNo == "AB104")
                {
                    Ra.ReadReplActionsForAtmReplCycleNo(WAtmNo, 7760);
                }
                else Ra.ReadReplActionsForAtmReplCycleNo(WAtmNo, WSesNo);
                if (Ra.RecordFound)
                {
                    textBoxOrderNo.Text = Ra.ReplActNo.ToString();
                    textBoxOrderDate.Text = Ra.AuthorisedDate.ToString();
                    textBoxIssueById.Text = Ra.AuthUser;
                    Us.ReadUsersRecord(Ra.AuthUser);
                    textBoxIssueByName.Text = Us.UserName ;
                    textBoxMoneyIn.Text = Ra.NewAmount.ToString("#,##0.00");
                }
                else
                {
                    MessageBox.Show("No Action Record Available. Complete repenishment with Applying Overrride.");
                }

            

                int WFunction = 2; //  BALANCES 
                Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, WFunction); // CALL TO MAKE BALANCES AVAILABLE 

                if (Na.ReplAmountTotal > 0 & (Na.ReplAmountTotal != Na.ReplAmountSuggest || Ra.RecordFound == false))
                {
                    // We had override 
                    textBoxOverrideAmt.Text = Na.ReplAmountTotal.ToString("#,##0.00");
                }
                
            }
            catch (Exception ex)
            {

                string exception = ex.ToString();
                //       MessageBox.Show(ex.ToString());
                MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

            }
    
            //TEST
            guidanceMsg = "Push Update and Move to Next step or Use Override!";

            if (ViewWorkFlow == true) guidanceMsg = "View Only!";
         
        }
       

// Update 
     
        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (Decimal.TryParse(textBoxOverrideAmt.Text, out CashInAmount) || textBoxOverrideAmt.Text == "")
            {
                if (textBoxOverrideAmt.Text == "")
                {
                    CashInAmount = 0;
                }
                else
                {
                    Ac.ReadAtm(WAtmNo);
                    if (CashInAmount > Ac.InsurOne)
                    {
                        MessageBox.Show("Insured amount is : " + Ac.InsurOne.ToString("#,##0.00") + "Your input is greater than this amount");
                    }

                    textBoxOverrideAmt.Text = CashInAmount.ToString("#,##0.00");
                } 
            }
            else
            {
                MessageBox.Show(textBoxOverrideAmt.Text, "Please enter a valid number For CashInAmount!");
                return;
            }
          

            if (CashInAmount == 0 & Ra.NewAmount == 0)
            {
                MessageBox.Show("No Money to Update!");
                return;
            }

            if (CashInAmount == 0)
            {
                CashInAmount = Ra.NewAmount; 
            }

            // Update Na data Bases with money in 

            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);

            Na.InUserDate = DateTime.Now;

            Na.InReplAmount = CashInAmount;

            Na.ReplAmountTotal = CashInAmount;

            if (Ra.NewAmount == 0) // In case there is no Order 
            {
                Na.ReplAmountSuggest = CashInAmount;
            }
            else
            {
                Na.ReplAmountSuggest = Ra.NewAmount;
            }       

            Na.UpdateSessionsNotesAndValues(WAtmNo, WSesNo);

            //
            // Update Actions table 
            // 
             Ra.ReadReplActionsForAtmReplCycleNo(WAtmNo, WSesNo);
             if (Ra.RecordFound)
             {
                 Ra.PassReplCycle = true;
                 Ra.PassReplCycleDate = DateTime.Now;
                 Ra.CashInAmount = CashInAmount; 

                 Ra.UpdateReplActionsForAtm(WAtmNo, Ra.ReplActNo); 
             }            

            //textBoxMoneyIn.Text = Na.InReplAmount.ToString("#,##0.00");

            // STEPLEVEL

            // Update STEP

            Us.ReadSignedActivityByKey(WSignRecordNo);

            Us.ReplStep4_Updated = true;

            Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            guidanceMsg = "Input data has been updated. Move to Next step.";

            ChangeBoardMessage(this, new EventArgs());
        }
    }
}
