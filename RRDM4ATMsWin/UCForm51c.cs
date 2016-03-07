using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class UCForm51c : UserControl
    {
        // Working variables

        public event EventHandler ChangeBoardMessage;
        public string guidanceMsg;


        Form38 NForm38;

        Form24 NForm24;

        bool ViewWorkFlow; 

        bool WSetScreen; 

    //    int Process;

    //    NotesBalances Na = new NotesBalances(); // Class Notes 

        RRDMTracesReadUpdate Ta = new RRDMTracesReadUpdate(); // Class Traces 

        RRDMDepositsClass Da = new RRDMDepositsClass(); // Contains all Deposits and Cheques 

        RRDMNotesBalances Na = new RRDMNotesBalances(); // see if deposits errors 

        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();


    //    int WFunction;
       
        string WSignedId;
        int WSignRecordNo;
        string WOperator;
    
        string WAtmNo;
        int WSesNo;
        

        public void UCForm51cPar(string InSignedId, int InSignRecordNo, string InOperator,  string InAtmNo, int InSesNo)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
   
            WAtmNo = InAtmNo;
            WSesNo = InSesNo;

            InitializeComponent();

            tableLayoutPanel11.Dock = DockStyle.Top;

            WSetScreen = true; 

            // ................................
            // Handle View ONLY 
            // ''''''''''''''''''''''''''''''''
            Us.ReadSignedActivityByKey(WSignRecordNo);

            if (Us.ProcessNo == 12 || Us.ProcessNo == 54 || Us.ProcessNo == 55 || Us.ProcessNo == 56)
            {
                ViewWorkFlow = true;
            }

            Na.ReadAllErrorsTable(WAtmNo, WSesNo);

            if (Na.NumberOfErrDep > 0)
            {
                button3.Show(); 
            }
            else button3.Hide();

            Da.ReadDepositsSessionsNotesAndValuesDeposits(WAtmNo, WSesNo);

            labelCashDep.Text = labelCashDep.Text + " - " + Da.DepositsMachine1.CurrNm;
            labelEnvelops.Text = labelEnvelops.Text + " - " + Da.DepositsMachine1.CurrNm;
            labelCheques.Text = labelCheques.Text +" - "+ Da.DepositsMachine1.CurrNm;
            labelMoneyTotals.Text = labelMoneyTotals.Text + " - " + Da.DepositsMachine1.CurrNm;

            // Deposits 

            textBoxDepositsMachine1Trans.Text = Da.DepositsMachine1.Trans.ToString();
            textBoxDepositsMachine1Notes.Text = Da.DepositsMachine1.Notes.ToString();
            textBoxDepositsMachine1Amount.Text = Da.DepositsMachine1.Amount.ToString("#,##0.00");
            textBoxDepositsMachine1NotesRej.Text = Da.DepositsMachine1.NotesRej.ToString();
            textBoxDepositsMachine1AmountRej.Text = Da.DepositsMachine1.AmountRej.ToString("#,##0.00");
            // Envelops 
            textBoxDepositsMachine1Envelops.Text = Da.DepositsMachine1.Envelops.ToString();
            textBoxDepositsMachine1EnvAmount.Text = Da.DepositsMachine1.EnvAmount.ToString("#,##0.00");
/*
            labelCheqATM1.Text = Da.ChequesMachine1.Trans.ToString();
            labelCheqATM2.Text = Da.ChequesMachine1.Number.ToString();
            labelCheqATM3.Text = Da.ChequesMachine1.Amount.ToString();
 */
            // Cheques 
            textBoxChequesMachine1Trans.Text = Da.ChequesMachine1.Trans.ToString();
            textBoxChequesMachine1Number.Text = Da.ChequesMachine1.Number.ToString();
            textBoxChequesMachine1Amount.Text = Da.ChequesMachine1.Amount.ToString("#,##0.00");
            //
            // COUNT
            // 
            textBoxDepositsCount1Trans.Text = Da.DepositsCount1.Trans.ToString();
            textBoxDepositsCount1Notes.Text = Da.DepositsCount1.Notes.ToString();
            textBoxDepositsCount1Amount.Text = Da.DepositsCount1.Amount.ToString("#,##0.00");
            textBoxDepositsCount1NotesRej.Text = Da.DepositsCount1.NotesRej.ToString();
            textBoxDepositsCount1AmountRej.Text = Da.DepositsCount1.AmountRej.ToString("#,##0.00");

            textBoxDepositsCount1Envelops.Text = Da.DepositsCount1.Envelops.ToString();
            textBoxDepositsCount1EnvAmount.Text = Da.DepositsCount1.EnvAmount.ToString("#,##0.00");

            textBoxChequesCount1Trans.Text = Da.ChequesCount1.Trans.ToString();
            textBoxChequesCount1Number.Text = Da.ChequesCount1.Number.ToString();
            textBoxChequesCount1Amount.Text = Da.ChequesCount1.Amount.ToString("#,##0.00");

            // Differences 

            textBoxDepositsDiff1Trans.Text = Da.DepositsDiff1.Trans.ToString();
            textBoxDepositsDiff1Notes.Text = Da.DepositsDiff1.Notes.ToString();
            textBoxDepositsDiff1Amount.Text = Da.DepositsDiff1.Amount.ToString("#,##0.00");
            textBoxDepositsDiff1NotesRej.Text = Da.DepositsDiff1.NotesRej.ToString();
            textBoxDepositsDiff1AmountRej.Text = Da.DepositsDiff1.AmountRej.ToString("#,##0.00");

            textBoxDepositsDiff1Envelops.Text = Da.DepositsDiff1.Envelops.ToString();
            textBoxDepositsDiff1EnvAmount.Text = Da.DepositsDiff1.EnvAmount.ToString("#,##0.00");

            textBoxChequesDiff1Trans.Text = Da.ChequesDiff1.Trans.ToString();
            textBoxChequesDiff1Number.Text = Da.ChequesDiff1.Number.ToString();
            textBoxChequesDiff1Amount.Text = Da.ChequesDiff1.Amount.ToString("#,##0.00");


            // Show Total Balances 

            textBoxDepositsBNACount.Text = (Da.DepositsCount1.Amount + Da.DepositsCount1.AmountRej).ToString("#,##0.00");
            textBoxEnvelopsCount.Text = (Da.DepositsCount1.EnvAmount).ToString("#,##0.00");
            textBoxChequesCount.Text = (Da.ChequesCount1.Amount).ToString("#,##0.00");

            textBoxDepositsBNAMachine.Text = (Da.DepositsMachine1.Amount + Da.DepositsMachine1.AmountRej).ToString("#,##0.00");
            textBoxEnvelopsMachine.Text = (Da.DepositsMachine1.EnvAmount).ToString("#,##0.00");
            textBoxChequesMachine.Text = (Da.ChequesMachine1.Amount).ToString("#,##0.00");

            textBoxBNADiff.Text = ((Da.DepositsCount1.Amount + Da.DepositsCount1.AmountRej) - (Da.DepositsMachine1.Amount + Da.DepositsMachine1.AmountRej)).ToString("#,##0.00");
            textBoxEnvelopsDiff.Text = ((Da.DepositsCount1.EnvAmount) - (Da.DepositsMachine1.EnvAmount)).ToString("#,##0.00");
            textBoxChequesDiff.Text = ((Da.ChequesCount1.Amount) - (Da.ChequesMachine1.Amount)).ToString("#,##0.00");

            guidanceMsg = " INPUT DATA AND UPDATE";

            // Handle request from Reconciliation 

            Us.ReadSignedActivityByKey(WSignRecordNo);

            if (ViewWorkFlow == true)  // If 13 the request came from Reconciliation and not from Replenishemnt 
            {
                button6.Hide();
                button5.Hide();
                button3.Hide();
                button1.Hide();

                textBoxDepositsCount1Trans.ReadOnly = true;
                textBoxDepositsCount1Notes.ReadOnly = true;
                textBoxDepositsCount1Amount.ReadOnly = true;
                textBoxDepositsCount1AmountRej.ReadOnly = true;
                textBoxDepositsCount1Envelops.ReadOnly = true;
                textBoxDepositsCount1EnvAmount.ReadOnly = true;
                textBoxChequesCount1Trans.ReadOnly = true;
                textBoxChequesCount1Number.ReadOnly = true;
                textBoxChequesCount1Amount.ReadOnly = true;

            //    guidanceMsg = " View only "; // Moved to form51
            }

            WSetScreen = false;   

        }
        // 
        // SHOW ATMs FIGURES AS COUNTED 
        //
        private void button6_Click(object sender, EventArgs e)
        {
            textBoxDepositsDiff1Trans.Text = "0";
            textBoxDepositsDiff1Notes.Text = "0";
            textBoxDepositsDiff1Amount.Text = "0.00";
            textBoxDepositsDiff1NotesRej.Text = "0";
            textBoxDepositsDiff1AmountRej.Text = "0.00";
            textBoxDepositsDiff1Envelops.Text = "0";
            textBoxDepositsDiff1EnvAmount.Text = "0.00";


            textBoxChequesDiff1Trans.Text = "0";
            textBoxChequesDiff1Number.Text = "0";
            textBoxChequesDiff1Amount.Text = "0.00"; 

            textBoxDepositsCount1Trans.Text = Da.DepositsMachine1.Trans.ToString();
            textBoxDepositsCount1Notes.Text = Da.DepositsMachine1.Notes.ToString();
            textBoxDepositsCount1Amount.Text = Da.DepositsMachine1.Amount.ToString("#,##0.00");
            textBoxDepositsCount1NotesRej.Text = Da.DepositsMachine1.NotesRej.ToString();
            textBoxDepositsCount1AmountRej.Text = Da.DepositsMachine1.AmountRej.ToString("#,##0.00");

            textBoxDepositsCount1Envelops.Text = Da.DepositsMachine1.Envelops.ToString();
            textBoxDepositsCount1EnvAmount.Text = Da.DepositsMachine1.EnvAmount.ToString("#,##0.00");

            textBoxChequesCount1Trans.Text = Da.ChequesMachine1.Trans.ToString();
            textBoxChequesCount1Number.Text = Da.ChequesMachine1.Number.ToString();
            textBoxChequesCount1Amount.Text = Da.ChequesMachine1.Amount.ToString("#,##0.00");

            // Show Total Balances 
            // COUNT FIGURES WILL BE THE SAME AS PER MACHINE 

            Da.DepositsCount1.Amount = Da.DepositsMachine1.Amount;
            Da.DepositsCount1.AmountRej = Da.DepositsMachine1.AmountRej;
            Da.DepositsCount1.EnvAmount = Da.DepositsMachine1.EnvAmount;
            Da.ChequesCount1.Amount = Da.ChequesMachine1.Amount ;

            textBoxDepositsBNACount.Text = (Da.DepositsCount1.Amount + Da.DepositsCount1.AmountRej).ToString("#,##0.00");
            textBoxEnvelopsCount.Text = (Da.DepositsCount1.EnvAmount).ToString("#,##0.00");
            textBoxChequesCount.Text = (Da.ChequesCount1.Amount).ToString("#,##0.00");

            textBoxDepositsBNAMachine.Text = (Da.DepositsMachine1.Amount + Da.DepositsMachine1.AmountRej).ToString("#,##0.00");
            textBoxEnvelopsMachine.Text = (Da.DepositsMachine1.EnvAmount).ToString("#,##0.00");
            textBoxChequesMachine.Text = (Da.ChequesMachine1.Amount).ToString("#,##0.00");

            textBoxBNADiff.Text = ((Da.DepositsCount1.Amount + Da.DepositsCount1.AmountRej) - (Da.DepositsMachine1.Amount + Da.DepositsMachine1.AmountRej)).ToString("#,##0.00");
            textBoxEnvelopsDiff.Text = ((Da.DepositsCount1.EnvAmount) - (Da.DepositsMachine1.EnvAmount)).ToString("#,##0.00");
            textBoxChequesDiff.Text = ((Da.ChequesCount1.Amount) - (Da.ChequesMachine1.Amount)).ToString("#,##0.00");

            guidanceMsg = " VERIFY FIGURES, CHANGE THEM IF NEEDED AND PRESS UPDATE";

            ChangeBoardMessage(this, e);

        }
        

        //
        // With UPDATE BUTTOM Update ONLY AND SHOW DIFF AND BALANCES 
        //

        private void button5_Click(object sender, EventArgs e)
        {
            // Update Session Notes with Input data

            // DEPOSITS 

            if (int.TryParse(textBoxDepositsCount1Trans.Text, out Da.DepositsCount1.Trans))
            {
              //  MessageBox.Show(textBox33.Text, "The input number is correct!");
            }
            else
            {
              MessageBox.Show(textBoxDepositsCount1Trans.Text, "Please enter a valid number!");
              return;
                
            }
            //
            if (int.TryParse(textBoxDepositsCount1Notes.Text, out Da.DepositsCount1.Notes))
            {
               // MessageBox.Show(textBox9.Text, "The input number is correct!"); 
                
            }
            else
            {
                MessageBox.Show(textBoxDepositsCount1Notes.Text, "Please enter a valid number!");
                return;
            }
            //
            if (decimal.TryParse(textBoxDepositsCount1Amount.Text, out Da.DepositsCount1.Amount))
            {
              //  MessageBox.Show(textBox10.Text, "The input number is correct!");
            }
            else
            {
                MessageBox.Show(textBoxDepositsCount1Amount.Text, "Please enter a valid number!");
                return;
            }
            // 
            if (int.TryParse(textBoxDepositsCount1NotesRej.Text, out Da.DepositsCount1.NotesRej))
            {
              //  MessageBox.Show(textBox11.Text, "The input number is correct!");
            }
            else
            {
                MessageBox.Show(textBoxDepositsCount1NotesRej.Text, "Please enter a valid number!");
                return;
            }

            if (decimal.TryParse(textBoxDepositsCount1AmountRej.Text, out Da.DepositsCount1.AmountRej))
            {
              //  MessageBox.Show(textBox1.Text, "The input number is correct!");
            }
            else
            {
                MessageBox.Show(textBoxDepositsCount1AmountRej.Text, "Please enter a valid number!");
                return;
            }
            //ENVELOPS
            // 
            if (int.TryParse(textBoxDepositsCount1Envelops.Text, out Da.DepositsCount1.Envelops))
            {
                //  MessageBox.Show(textBox11.Text, "The input number is correct!");
            }
            else
            {
                MessageBox.Show(textBoxDepositsCount1Envelops.Text, "Please enter a valid number!");
                return;
            }

            if (decimal.TryParse(textBoxDepositsCount1EnvAmount.Text, out Da.DepositsCount1.EnvAmount))
            {
                //  MessageBox.Show(textBox1.Text, "The input number is correct!");
            }
            else
            {
                MessageBox.Show(textBoxDepositsCount1EnvAmount.Text, "Please enter a valid number!");
                return;
            }

            // 
            // CHEQUES
            //
            if (int.TryParse(textBoxChequesCount1Trans.Text, out Da.ChequesCount1.Trans))
            {
                //  MessageBox.Show(textBox34.Text, "The input number is correct!");
            }
            else
            {
                MessageBox.Show(textBoxChequesCount1Trans.Text, "Please enter a valid number!");
                return;
            }

            if (int.TryParse(textBoxChequesCount1Number.Text, out Da.ChequesCount1.Number))
            {
                //  MessageBox.Show(textBox12.Text, "The input number is correct!");
            }
            else
            {
                MessageBox.Show(textBoxChequesCount1Number.Text, "Please enter a valid number!");
                return;
            }

            if (decimal.TryParse(textBoxChequesCount1Amount.Text, out Da.ChequesCount1.Amount))
            {
                //  MessageBox.Show(textBox13.Text, "The input number is correct!");
            }
            else
            {
                MessageBox.Show(textBoxChequesCount1Amount.Text, "Please enter a valid number!");
                return;
            }

            Da.UpdateDepositsSessionsNotesAndValuesWithCount(WAtmNo, WSesNo); // UPDATE INPUT VALUES

            Da.ReadDepositsSessionsNotesAndValuesDeposits(WAtmNo, WSesNo); // READ UPDATED DATA 
 
            // Show differences 

            textBoxDepositsDiff1Trans.Text = Da.DepositsDiff1.Trans.ToString();
            textBoxDepositsDiff1Notes.Text = Da.DepositsDiff1.Notes.ToString();
            textBoxDepositsDiff1Amount.Text = Da.DepositsDiff1.Amount.ToString("#,##0.00");
            textBoxDepositsDiff1NotesRej.Text = Da.DepositsDiff1.NotesRej.ToString();
            textBoxDepositsDiff1AmountRej.Text = Da.DepositsDiff1.AmountRej.ToString("#,##0.00");

            textBoxDepositsDiff1Envelops.Text = Da.DepositsDiff1.Envelops.ToString();
            textBoxDepositsDiff1EnvAmount.Text = Da.DepositsDiff1.EnvAmount.ToString("#,##0.00");

            textBoxChequesDiff1Trans.Text = Da.ChequesDiff1.Trans.ToString();
            textBoxChequesDiff1Number.Text = Da.ChequesDiff1.Number.ToString();
            textBoxChequesDiff1Amount.Text = Da.ChequesDiff1.Amount.ToString("#,##0.00");

           
            // Show Total Balances 

            textBoxDepositsBNACount.Text = (Da.DepositsCount1.Amount + Da.DepositsCount1.AmountRej).ToString("#,##0.00");
            textBoxEnvelopsCount.Text = (Da.DepositsCount1.EnvAmount).ToString("#,##0.00");
            textBoxChequesCount.Text = (Da.ChequesCount1.Amount).ToString("#,##0.00");

            textBoxDepositsBNAMachine.Text = (Da.DepositsMachine1.Amount + Da.DepositsMachine1.AmountRej).ToString("#,##0.00");
            textBoxEnvelopsMachine.Text = (Da.DepositsMachine1.EnvAmount).ToString("#,##0.00");
            textBoxChequesMachine.Text = (Da.ChequesMachine1.Amount).ToString("#,##0.00");

            textBoxBNADiff.Text = ((Da.DepositsCount1.Amount + Da.DepositsCount1.AmountRej) - (Da.DepositsMachine1.Amount + Da.DepositsMachine1.AmountRej)).ToString("#,##0.00");
            textBoxEnvelopsDiff.Text = ((Da.DepositsCount1.EnvAmount) - (Da.DepositsMachine1.EnvAmount)).ToString("#,##0.00");
            textBoxChequesDiff.Text = ((Da.ChequesCount1.Amount) - (Da.ChequesMachine1.Amount)).ToString("#,##0.00");

// Check if data inputed 
            if ((Da.DepositsMachine1.Amount + Da.DepositsMachine1.AmountRej) > 0 || (Da.DepositsMachine1.EnvAmount) > 0 || (Da.ChequesMachine1.Amount) > 0)
            {
                // Check if input was made 
                if ((Da.DepositsCount1.Amount + Da.DepositsCount1.AmountRej) == 0 & (Da.DepositsCount1.EnvAmount) == 0
                    & (Da.ChequesCount1.Amount) == 0 )
                {
                    MessageBox.Show("Please enter counted data");
                    return;
                }
            }

            // Update STEP

            Us.ReadSignedActivityByKey(WSignRecordNo);

            Us.ReplStep3_Updated = true;

            Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

         //   MessageBox.Show("Input Data is updated.");

            //Form2MessageBox Mb = new Form2MessageBox("Input Data has been updated.");
            //Mb.StartPosition = FormStartPosition.Manual;
            //Mb.Location = new Point(300, 480);
            //Mb.ShowDialog();


            button5.Hide(); 

            if (Da.DiffInDeposits == true || Da.DiffInCheques == true)
            {
                guidanceMsg = " Updating done but with differences! - Move to next step. ";
            }
            else
            {
                guidanceMsg = " Updating done .. No Differences !- Move to next step. ";
            }
            
            ChangeBoardMessage(this, e);

           
        }

        protected override CreateParams CreateParams
        {

            get
            {

                CreateParams handleParam = base.CreateParams;

                handleParam.ExStyle |= 0x02000000;   // WS_EX_COMPOSITED        

                return handleParam;

            }

        }
        // SHOW ERRORS 
        private void button3_Click(object sender, EventArgs e)
        {
            bool Replenishment = true;
       //     string SearchFilter = "AtmNo = '" + WAtmNo + "' AND SesNo=" + WSesNo + " AND (ErrId > 200 AND ErrId < 299) AND OpenErr =1";
            string SearchFilter = "AtmNo = '" + WAtmNo + "' AND SesNo=" + WSesNo + " AND (ErrId > 200 AND ErrId < 299)";
            NForm24 = new Form24(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo, "", Replenishment, SearchFilter);
            NForm24.Show();
        }

        private void tableLayoutPanel11_Paint(object sender, PaintEventArgs e)
        {

        }
        // Show Deposits 
        private void button1_Click(object sender, EventArgs e)
        {
            NForm38 = new Form38(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
            NForm38.Show();
        }
        //
// My Count Change
        // 
        private void textBox33_TextChanged(object sender, EventArgs e)
        {
            SetSteplevel(); 
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            SetSteplevel(); 
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            SetSteplevel(); 
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            SetSteplevel(); 
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            SetSteplevel(); 
        }

        private void textBox41_TextChanged(object sender, EventArgs e)
        {
            SetSteplevel(); 
        }

        private void textBox42_TextChanged(object sender, EventArgs e)
        {
            SetSteplevel(); 
        }

        private void textBox34_TextChanged(object sender, EventArgs e)
        {
            SetSteplevel(); 
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            SetSteplevel(); 
        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {
            SetSteplevel(); 
        }
        // SET step level to need update if changes 
        private void SetSteplevel()
        {
            if (WSetScreen == false)
            {
                // Update STEP

                Us.ReadSignedActivityByKey(WSignRecordNo);

                Us.ReplStep3_Updated = false;

                Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                button5.Show();
            }
        }
    }
}
