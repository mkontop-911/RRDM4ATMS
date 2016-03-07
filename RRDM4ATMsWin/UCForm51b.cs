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
    public partial class UCForm51b : UserControl
    {
        
          Form24 NForm24; // Show the errors 

          public event EventHandler ChangeBoardMessage;
          public string guidanceMsg;

          bool ViewWorkFlow; 
          bool WSetScreen; 

        RRDMNotesBalances Na = new RRDMNotesBalances(); // Class Notes 
        
        RRDMTracesReadUpdate Ta = new RRDMTracesReadUpdate(); // Class Traces 

        RRDMAtmsClass Ac = new RRDMAtmsClass();
        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();

       // string WUserOperator;

        int WFunction;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
     
        string WAtmNo;
        int WSesNo;
        

        public void UCForm51bPar(string InSignedId, int InSignRecordNo, string InOperator, string InAtmNo, int InSesNo)
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
         
            WAtmNo = InAtmNo;
            WSesNo = InSesNo;

            InitializeComponent();

            // ================USER BANK =============================
     //       Us.ReadUsersRecord(WSignedId); // Read USER record for the signed user
     //       WUserOperator = Us.Operator;
            // ========================================================

            this.DoubleBuffered = true;
            SetScreen();
         //   button2.Hide();
        }

        public void SetScreen()
        {
            buttonUpdateInput.Hide();

            WSetScreen = true; 
            // ................................
            // Handle View ONLY 
            // ''''''''''''''''''''''''''''''''
            Us.ReadSignedActivityByKey(WSignRecordNo);

            if (Us.ProcessNo == 11 || Us.ProcessNo == 54 || Us.ProcessNo == 55 || Us.ProcessNo == 56)
            {
                ViewWorkFlow = true;
            }

            WFunction = 2;
            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, WFunction); // Read Values from NOTES
            
            // Cassette 1
            int temp = Convert.ToInt32(Na.Cassettes_1.FaceValue);
            label13.Text = "Type 1- " + temp.ToString() + " " + Na.Cassettes_1.CurName;
            textBox11.Text = Na.Cassettes_1.RemNotes.ToString();
            label11.Text = "Type 1- " + temp.ToString() + " " + Na.Cassettes_1.CurName;
            textBox22.Text = Na.Cassettes_1.RejNotes.ToString();

            textBox1.Text = Na.Cassettes_1.CasCount.ToString();
            textBox5.Text = Na.Cassettes_1.RejCount.ToString();

            // Cassette 2
            temp = Convert.ToInt32(Na.Cassettes_2.FaceValue);
            label14.Text = "Type 2 - " + temp.ToString() + " " + Na.Cassettes_2.CurName;
            textBox13.Text = Na.Cassettes_2.RemNotes.ToString();
            label9.Text = "Type 2 - " + temp.ToString() + " " + Na.Cassettes_2.CurName;
            textBox24.Text = Na.Cassettes_2.RejNotes.ToString();

            textBox2.Text = Na.Cassettes_2.CasCount.ToString();
            textBox6.Text = Na.Cassettes_2.RejCount.ToString();

            // Cassette 3
            temp = Convert.ToInt32(Na.Cassettes_3.FaceValue);
            label16.Text = "Type 3 - " + temp.ToString() + " " + Na.Cassettes_3.CurName;
            textBox15.Text = Na.Cassettes_3.RemNotes.ToString();
            label8.Text = "Type 3 - " + temp.ToString() + " " + Na.Cassettes_3.CurName;
            textBox26.Text = Na.Cassettes_3.RejNotes.ToString();

            textBox3.Text = Na.Cassettes_3.CasCount.ToString();
            textBox7.Text = Na.Cassettes_3.RejCount.ToString();

            // Cassette 4
            temp = Convert.ToInt32(Na.Cassettes_4.FaceValue);
            label17.Text = "Type 4 - " + temp.ToString() + " " + Na.Cassettes_4.CurName;
            textBox17.Text = Na.Cassettes_4.RemNotes.ToString();
            label7.Text = "Type 4 - " + temp.ToString() + " " + Na.Cassettes_4.CurName;
            textBox19.Text = Na.Cassettes_4.RejNotes.ToString();

            textBox4.Text = Na.Cassettes_4.CasCount.ToString();
            textBox8.Text = Na.Cassettes_4.RejCount.ToString();

            // Show differences in Notes  

            textBox12.Text = Na.Cassettes_1.DiffCas.ToString();
            textBox21.Text = (Na.Cassettes_1.FaceValue * Na.Cassettes_1.DiffCas).ToString("#,##0.00");
            textBox23.Text = Na.Cassettes_1.DiffRej.ToString();
            textBox41.Text = (Na.Cassettes_1.FaceValue * Na.Cassettes_1.DiffRej).ToString("#,##0.00");

            textBox14.Text = Na.Cassettes_2.DiffCas.ToString();
            textBox45.Text = (Na.Cassettes_2.FaceValue * Na.Cassettes_2.DiffCas).ToString("#,##0.00");
            textBox25.Text = Na.Cassettes_2.DiffRej.ToString();
            textBox42.Text = (Na.Cassettes_2.FaceValue * Na.Cassettes_2.DiffRej).ToString("#,##0.00");

            textBox16.Text = Na.Cassettes_3.DiffCas.ToString();
            textBox46.Text = (Na.Cassettes_3.FaceValue * Na.Cassettes_3.DiffCas).ToString("#,##0.00");
            textBox27.Text = Na.Cassettes_3.DiffRej.ToString();
            textBox43.Text = (Na.Cassettes_3.FaceValue * Na.Cassettes_3.DiffRej).ToString("#,##0.00");
            
            textBox18.Text = Na.Cassettes_4.DiffCas.ToString();
            textBox47.Text = (Na.Cassettes_4.FaceValue * Na.Cassettes_4.DiffCas).ToString("#,##0.00");
            textBox20.Text = Na.Cassettes_4.DiffRej.ToString();
            textBox44.Text = (Na.Cassettes_4.FaceValue * Na.Cassettes_4.DiffRej).ToString("#,##0.00");

// Capture Cards 

            textBox9.Text = Na.CaptCardsMachine.ToString();
            textBox34.Text = Na.CaptCardsCount.ToString();

            textBox10.Text = (Na.CaptCardsCount - Na.CaptCardsMachine).ToString(); // Captured Differences 

            if (Na.ErrJournalThisCycle > 0)
            {
                buttonShowErrors.Show(); 
            }
            else buttonShowErrors.Hide(); 

            ShowBalances();// SHOW BALANCES 

             guidanceMsg = " Input numbers of counted Notes And Press UPDATE - You can make corrections at wish  "; 

            // View only 

               if (ViewWorkFlow == true) 
               {
                   buttonUpdateInput.Hide();
                   buttonShowErrors.Hide();
                   buttonUseAtmsFigures.Hide();
                   textBox1.ReadOnly= true;
                   textBox2.ReadOnly = true;
                   textBox3.ReadOnly = true;
                   textBox4.ReadOnly = true; 
                   textBox5.ReadOnly = true;
                   textBox6.ReadOnly = true;
                   textBox7.ReadOnly = true;
                   textBox8.ReadOnly = true;
                   textBox34.ReadOnly = true;

               //    guidanceMsg = " View only "; // Move to form51
               }

               WSetScreen = false; 

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
        
// Create and Show input as per ATM
        // 
        private void buttonUseAtmsFigures_Click(object sender, EventArgs e)
        {
            // INITIALISE FORM 
            //     panel1.Hide();
            label24.Hide();
            label3.Hide();
            textBox12.Text = " ";
            textBox23.Text = " ";
            textBox14.Text = " ";
            textBox25.Text = " ";
            textBox16.Text = " ";
            textBox27.Text = " ";
            textBox18.Text = " ";
            textBox20.Text = " ";

            textBox10.Text = " ";

            // SHOW 
            // Cassette 1
            textBox1.Text = Na.Cassettes_1.RemNotes.ToString();
            textBox5.Text = Na.Cassettes_1.RejNotes.ToString();

            // Cassette 2
            textBox2.Text = Na.Cassettes_2.RemNotes.ToString();
            textBox6.Text = Na.Cassettes_2.RejNotes.ToString();

            // Cassette 3
            textBox3.Text = Na.Cassettes_3.RemNotes.ToString();
            textBox7.Text = Na.Cassettes_3.RejNotes.ToString();

            // Cassette 4
            textBox4.Text = Na.Cassettes_4.RemNotes.ToString();
            textBox8.Text = Na.Cassettes_4.RejNotes.ToString();

            textBox34.Text = Na.CaptCardsMachine.ToString();

            Na.Balances1.CountedBal = Na.Balances1.MachineBal;
            Na.Balances2.CountedBal = Na.Balances2.MachineBal;
            Na.Balances3.CountedBal = Na.Balances3.MachineBal;
            Na.Balances4.CountedBal = Na.Balances4.MachineBal;

            Na.BalDiff1.Machine = 0;
            Na.BalDiff2.Machine = 0;
            Na.BalDiff3.Machine = 0;
            Na.BalDiff4.Machine = 0;

            // Show differences in Notes  

            textBox12.Text = "0";
            textBox23.Text = "0";
            textBox14.Text = "0";
            textBox25.Text = "0";
            textBox16.Text = "0";
            textBox27.Text = "0";
            textBox18.Text = "0";
            textBox20.Text = "0";

            textBox10.Text = "0";

            textBox21.Text = "0.00";
            textBox45.Text = "0.00";
            textBox46.Text = "0.00";
            textBox47.Text = "0.00";
            textBox41.Text = "0.00";
            textBox42.Text = "0.00";
            textBox43.Text = "0.00";
            textBox44.Text = "0.00";

            ShowBalances();// SHOW BALANCES 

            guidanceMsg = " VERIFY FIGURES CHANGE THEM IF YOU WANT AND press UPDATE";

            ChangeBoardMessage(this, e);
        }

        //
        // With UPDATE BUTTOM Update ONLY AND SHOW DIFF AND BALANCES 
        //

        private void buttonUpdateInput_Click(object sender, EventArgs e)
        {
            // VALIDATION 
            if (textBox1.Text == "0" & textBox2.Text == "0" & textBox3.Text == "0" & textBox4.Text == "0"
                 & textBox5.Text == "0" & textBox6.Text == "0" & textBox7.Text == "0" & textBox8.Text == "0")
            {
                MessageBox.Show("Please enter counted values !");
                return;
            }

            if (int.TryParse(textBox1.Text, out Na.Cassettes_1.CasCount))
            {
            }
            else
            {
                MessageBox.Show(textBox1.Text, "Please enter a valid number!");

                return;
            }


            if (int.TryParse(textBox5.Text, out Na.Cassettes_1.RejCount))
            {
            }
            else
            {
                MessageBox.Show(textBox5.Text, "Please enter a valid number!");
                return;
            }


            if (int.TryParse(textBox2.Text, out Na.Cassettes_2.CasCount))
            {
            }
            else
            {
                MessageBox.Show(textBox2.Text, "Please enter a valid number!");
                return;
            }


            if (int.TryParse(textBox6.Text, out Na.Cassettes_2.RejCount))
            {
            }
            else
            {
                MessageBox.Show(textBox6.Text, "Please enter a valid number!");
                return;
            }


            if (int.TryParse(textBox3.Text, out Na.Cassettes_3.CasCount))
            {
            }
            else
            {
                MessageBox.Show(textBox3.Text, "Please enter a valid number!");
                return;
            }


            if (int.TryParse(textBox7.Text, out Na.Cassettes_3.RejCount))
            {
            }
            else
            {
                MessageBox.Show(textBox7.Text, "Please enter a valid number!");
                return;
            }


            if (int.TryParse(textBox4.Text, out Na.Cassettes_4.CasCount))
            {
            }
            else
            {
                MessageBox.Show(textBox4.Text, "Please enter a valid number!");
                return;
            }



            if (int.TryParse(textBox8.Text, out Na.Cassettes_4.RejCount))
            {
            }
            else
            {
                MessageBox.Show(textBox8.Text, "Please enter a valid number!");
                return;
            }

            // Captured Cards 

            if (int.TryParse(textBox34.Text, out Na.CaptCardsCount))
            {
            }
            else
            {
                MessageBox.Show(textBox34.Text, "Please enter a valid number!");
                return;
            }

            //
            // UPDATING 
            //
            Na.UpdateSessionsNotesAndValues(WAtmNo, WSesNo);

            // Read Session Notes to get the updated data 

            WFunction = 2;
            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, WFunction);

            // Show differences in Notes  

            textBox12.Text = Na.Cassettes_1.DiffCas.ToString();
            textBox21.Text = (Na.Cassettes_1.FaceValue * Na.Cassettes_1.DiffCas).ToString("#,##0.00");
            textBox23.Text = Na.Cassettes_1.DiffRej.ToString();
            textBox41.Text = (Na.Cassettes_1.FaceValue * Na.Cassettes_1.DiffRej).ToString("#,##0.00");

            textBox14.Text = Na.Cassettes_2.DiffCas.ToString();
            textBox45.Text = (Na.Cassettes_2.FaceValue * Na.Cassettes_2.DiffCas).ToString("#,##0.00");
            textBox25.Text = Na.Cassettes_2.DiffRej.ToString();
            textBox42.Text = (Na.Cassettes_2.FaceValue * Na.Cassettes_2.DiffRej).ToString("#,##0.00");

            textBox16.Text = Na.Cassettes_3.DiffCas.ToString();
            textBox46.Text = (Na.Cassettes_3.FaceValue * Na.Cassettes_3.DiffCas).ToString("#,##0.00");
            textBox27.Text = Na.Cassettes_3.DiffRej.ToString();
            textBox43.Text = (Na.Cassettes_3.FaceValue * Na.Cassettes_3.DiffRej).ToString("#,##0.00");

            textBox18.Text = Na.Cassettes_4.DiffCas.ToString();
            textBox47.Text = (Na.Cassettes_4.FaceValue * Na.Cassettes_4.DiffCas).ToString("#,##0.00");
            textBox20.Text = Na.Cassettes_4.DiffRej.ToString();
            textBox44.Text = (Na.Cassettes_4.FaceValue * Na.Cassettes_4.DiffRej).ToString("#,##0.00");

            textBox10.Text = (Na.CaptCardsCount - Na.CaptCardsMachine).ToString();

            ShowBalances();// SHOW BALANCES 

            // Check if differences 
            if (Na.Cassettes_1.DiffCas == 0 & Na.Cassettes_1.DiffRej == 0 &
                Na.Cassettes_2.DiffCas == 0 & Na.Cassettes_2.DiffRej == 0 &
                Na.Cassettes_3.DiffCas == 0 & Na.Cassettes_3.DiffRej == 0 &
                Na.Cassettes_4.DiffCas == 0 & Na.Cassettes_4.DiffRej == 0
                )
            {
                if (Na.ErrJournalThisCycle > 0)
                {
                    guidanceMsg = "You may have to recount money! ";
                    buttonShowErrors.Show();


                    if (MessageBox.Show("Warning: Balances reconcile but there are error/s. Do you want to examine errors?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                     == DialogResult.Yes)
                    {
                        bool Replenishment = true;
                        string SearchFilter = "AtmNo = '" + WAtmNo + "'" + " AND SesNo =" + WSesNo + " AND (ErrType = 1) AND OpenErr =1";
                        NForm24 = new Form24(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo, "", Replenishment, SearchFilter);
                        NForm24.Show();
                        return;
                    }

                }
                else
                {
                    guidanceMsg = "ALL WELL FIGURES AT ATM LEVEL RECONCILE";

                }
            }
            else
            {
                if (Na.BalDiff1.Machine == Na.Balances1.PresenterValue)
                {
                    guidanceMsg = "WARNING: THERE ARE DIFFERENCES but your presented errors is of the same value =" + Na.Balances1.PresenterValue;
                    label3.Show();
                    Color Black = Color.Black;
                    label3.ForeColor = Black;
                    label3.Text = "Difference Same as Errors Value";
                }

                if (Na.BalDiff1.Machine != Na.Balances1.PresenterValue)
                {
                    guidanceMsg = "WARNING: THERE ARE DIFFERENCES. Your presented errors which are "
                        + Na.Balances1.PresenterValue + " are not of the same value";
                    label3.Show();
                    Color Red = Color.Red;
                    label3.ForeColor = Red;
                    label3.Text = "Difference Not Same as Errors Value";
                }
                buttonShowErrors.Show();
            }

            //// STEPLEVEL

            //Us.ReadSignedActivityByKey(WSignRecordNo);

            //if (Us.StepLevel < 2)
            //{
            //    Ac.ReadAtm(WAtmNo);

            //    if (Ac.ChequeReader == false & Ac.DepoReader == false & Ac.EnvelopDepos == false)
            //    {
            //        Us.StepLevel = 3;
            //    }
            //    else Us.StepLevel = 2;

            //    Us.UpdateSignedInTableStepLevel(WSignRecordNo);
            //}

            // Update STEP

            Us.ReadSignedActivityByKey(WSignRecordNo);

            Us.ReplStep2_Updated = true;

            Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            ////   MessageBox.Show("Input Data has been updated.");

            //   Form2MessageBox Mb = new Form2MessageBox("Input Data has been updated.");
            //   Mb.StartPosition = FormStartPosition.Manual;
            //   Mb.Location = new Point(300, 480);
            //   Mb.ShowDialog();

            buttonUpdateInput.Hide();

            guidanceMsg = "Updating completed. - Move to next step.";
            ChangeBoardMessage(this, e);

        }        

public void ShowBalances()
{
    // FILL ALL FIELDS FOR ALL CURRENCIES 

    if (Na.BalSets >= 1)
    {
        label29.Text = Na.Balances1.CurrNm;
        textBox28.Text = Na.Balances1.CountedBal.ToString("#,##0.00");
        textBox32.Text = Na.Balances1.MachineBal.ToString("#,##0.00");
        textBox33.Text = Na.BalDiff1.Machine.ToString("#,##0.00");
    }

    if (Na.BalSets >= 2)
    {
        label19.Text = Na.Balances2.CurrNm;
        textBox30.Text = Na.Balances2.CountedBal.ToString("#,##0.00");
        textBox35.Text = Na.Balances2.MachineBal.ToString("#,##0.00");
        textBox36.Text = Na.BalDiff2.Machine.ToString("#,##0.00");
    }
    if (Na.BalSets >= 3)
    {
        label25.Text = Na.Balances3.CurrNm;
        textBox31.Text = Na.Balances3.CountedBal.ToString("#,##0.00");
        textBox37.Text = Na.Balances3.MachineBal.ToString("#,##0.00");
        textBox38.Text = Na.BalDiff3.Machine.ToString("#,##0.00");
    }

    if (Na.BalSets == 4)
    {
        label28.Text = Na.Balances4.CurrNm;
        textBox29.Text = Na.Balances4.CountedBal.ToString("#,##0.00");
        textBox39.Text = Na.Balances4.MachineBal.ToString("#,##0.00");
        textBox40.Text = Na.BalDiff4.Machine.ToString("#,##0.00");
    }

    // SHOW ... SHOW ... SHOW 

    panel1.Show();
    label24.Show();
    label19.Show(); textBox30.Show(); textBox35.Show(); textBox36.Show();
    label25.Show(); textBox31.Show(); textBox37.Show(); textBox38.Show();
    label28.Show(); textBox29.Show(); textBox39.Show(); textBox40.Show();

    if (Na.BalSets == 1)
    {
        label19.Hide(); textBox30.Hide(); textBox35.Hide(); textBox36.Hide();
        label25.Hide(); textBox31.Hide(); textBox37.Hide(); textBox38.Hide();
        label28.Hide(); textBox29.Hide(); textBox39.Hide(); textBox40.Hide();
    }
    if (Na.BalSets == 2)
    {
        label25.Hide(); textBox31.Hide(); textBox37.Hide(); textBox38.Hide();
        label28.Hide(); textBox29.Hide(); textBox39.Hide(); textBox40.Hide();
    }
    if (Na.BalSets == 3)
    {
        label28.Hide(); textBox29.Hide(); textBox39.Hide(); textBox40.Hide();
    }
    if (Na.BalSets == 4)
    {
        // HIDE Nothing
    }
}

// Show Errors 

private void buttonShowErrors_Click(object sender, EventArgs e)
{
    bool Replenishment = true;
    string SearchFilter = "AtmNo = '" + WAtmNo + "' AND SesNo=" + WSesNo + " AND (ErrType = 1) AND OpenErr =1";
    NForm24 = new Form24(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo, "", Replenishment, SearchFilter);
    NForm24.ShowDialog();
}
// My Counts
private void textBox1_TextChanged(object sender, EventArgs e)
{
    SetSteplevel(); 
}

private void textBox2_TextChanged(object sender, EventArgs e)
{
    SetSteplevel(); 
}

private void textBox3_TextChanged(object sender, EventArgs e)
{
    SetSteplevel(); 
}

private void textBox4_TextChanged(object sender, EventArgs e)
{
    SetSteplevel(); 
}

private void textBox5_TextChanged(object sender, EventArgs e)
{
    SetSteplevel(); 
}

private void textBox6_TextChanged(object sender, EventArgs e)
{
    SetSteplevel(); 
}

private void textBox7_TextChanged(object sender, EventArgs e)
{
    SetSteplevel(); 
}

private void textBox8_TextChanged(object sender, EventArgs e)
{
    SetSteplevel(); 
}

private void textBox34_TextChanged(object sender, EventArgs e)
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

        Us.ReplStep2_Updated = false;

        Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

        buttonUpdateInput.Show(); 
    }
}
               
    }
}
