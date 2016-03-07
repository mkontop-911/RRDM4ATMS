using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RRDM4ATMs;

namespace RRDM4ATMsWeb
{
    public partial class WebForm152b : System.Web.UI.Page
    {

        RRDMNotesBalances Na = new RRDMNotesBalances(); // Class Notes 

        RRDMTracesReadUpdate Ta = new RRDMTracesReadUpdate(); // Class Traces 

        RRDMAtmsClass Ac = new RRDMAtmsClass();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                SetScreen();

            }

        }

        protected void SetScreen()
        {
            int WFunction = 2;
            string WAtmNo = (string)Session["WAtmNo"];
            int WSesNo = (int)Session["WReplCycle"]; 

            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, WFunction); // Read Values from NOTES

            // Cassette 1
            int temp = Convert.ToInt32(Na.Cassettes_1.FaceValue);
            lbel13.Text = "Type 1- " + temp.ToString() + " " + Na.Cassettes_1.CurName;
            txtBox11.Text = Na.Cassettes_1.RemNotes.ToString();
            lbel11.Text = "Type 1- " + temp.ToString() + " " + Na.Cassettes_1.CurName;
            txtBox22.Text = Na.Cassettes_1.RejNotes.ToString();

            txtBox1.Text = Na.Cassettes_1.CasCount.ToString();
            txtBox5.Text = Na.Cassettes_1.RejCount.ToString();

            // Cassette 2
            temp = Convert.ToInt32(Na.Cassettes_2.FaceValue);
            lbel14.Text = "Type 2 - " + temp.ToString() + " " + Na.Cassettes_2.CurName;
            txtBox13.Text = Na.Cassettes_2.RemNotes.ToString();
            lbel9.Text = "Type 2 - " + temp.ToString() + " " + Na.Cassettes_2.CurName;
            txtBox24.Text = Na.Cassettes_2.RejNotes.ToString();

            txtBox2.Text = Na.Cassettes_2.CasCount.ToString();
            txtBox6.Text = Na.Cassettes_2.RejCount.ToString();

            // Cassette 3
            temp = Convert.ToInt32(Na.Cassettes_3.FaceValue);
            lbel16.Text = "Type 3 - " + temp.ToString() + " " + Na.Cassettes_3.CurName;
            txtBox15.Text = Na.Cassettes_3.RemNotes.ToString();
            lbel8.Text = "Type 3 - " + temp.ToString() + " " + Na.Cassettes_3.CurName;
            txtBox26.Text = Na.Cassettes_3.RejNotes.ToString();

            txtBox3.Text = Na.Cassettes_3.CasCount.ToString();
            txtBox7.Text = Na.Cassettes_3.RejCount.ToString();

            // Cassette 4
            temp = Convert.ToInt32(Na.Cassettes_4.FaceValue);
            lbel17.Text = "Type 4 - " + temp.ToString() + " " + Na.Cassettes_4.CurName;
            txtBox17.Text = Na.Cassettes_4.RemNotes.ToString();
            lbel7.Text = "Type 4 - " + temp.ToString() + " " + Na.Cassettes_4.CurName;
            txtBox19.Text = Na.Cassettes_4.RejNotes.ToString();

            txtBox4.Text = Na.Cassettes_4.CasCount.ToString();
            txtBox8.Text = Na.Cassettes_4.RejCount.ToString();

            // Show differences in Notes  
            txtBox12.Text = Na.Cassettes_1.DiffCas.ToString();
            txtBox23.Text = Na.Cassettes_1.DiffRej.ToString();
            txtBox14.Text = Na.Cassettes_2.DiffCas.ToString();
            txtBox25.Text = Na.Cassettes_2.DiffRej.ToString();
            txtBox16.Text = Na.Cassettes_3.DiffCas.ToString();
            txtBox27.Text = Na.Cassettes_3.DiffRej.ToString();
            txtBox18.Text = Na.Cassettes_4.DiffCas.ToString();
            txtBox20.Text = Na.Cassettes_4.DiffRej.ToString();

            // Capture Cards 

            txtBox9.Text = Na.CaptCardsMachine.ToString();
            txtBox34.Text = Na.CaptCardsCount.ToString();

            txtBox10.Text = (Na.CaptCardsCount - Na.CaptCardsMachine).ToString(); // Captured Differences 

            if (Na.ErrJournalThisCycle > 0)
            {
            //    ButtonErrors.Show(); 
            }
           // else button2.Hide();

            ShowBalances();// SHOW BALANCES 

            // Handle request from Reconciliation 

       //     Us.ReadSignedActivityByKey(WSignRecordNo);

            //if (Us.ProcessNo == 11) // If 11 the request came from Reconciliation and not from Replenishemnt 
            //{
            //    button1.Hide();
            //    button2.Hide();
            //    button4.Hide();
            //    textBox1.ReadOnly = true;
            //    textBox2.ReadOnly = true;
            //    textBox3.ReadOnly = true;
            //    textBox4.ReadOnly = true;
            //    textBox5.ReadOnly = true;
            //    textBox6.ReadOnly = true;
            //    textBox7.ReadOnly = true;
            //    textBox8.ReadOnly = true;
            //    textBox34.ReadOnly = true;
            //}

            txtMessage.Text = " Input numbers of counted Notes And Press UPDATE - You can make corrections at wish  ";

        }

        protected void ShowBalances()
        {
            // FILL ALL FIELDS FOR ALL CURRENCIES 

            if (Na.BalSets >= 1)
            {
                lbel29.Text = Na.Balances1.CurrNm;
                txtBox28.Text = Na.Balances1.CountedBal.ToString("#,##0.00");
                txtBox32.Text = Na.Balances1.MachineBal.ToString("#,##0.00");
                txtBox33.Text = Na.BalDiff1.Machine.ToString("#,##0.00");
            }

            if (Na.BalSets >= 2)
            {
                lbel19.Text = Na.Balances2.CurrNm;
                txtBox30.Text = Na.Balances2.CountedBal.ToString("#,##0.00");
                txtBox35.Text = Na.Balances2.MachineBal.ToString("#,##0.00");
                txtBox36.Text = Na.BalDiff2.Machine.ToString("#,##0.00");
            }
            if (Na.BalSets >= 3)
            {
                lbel25.Text = Na.Balances3.CurrNm;
                txtBox31.Text = Na.Balances3.CountedBal.ToString("#,##0.00");
                txtBox37.Text = Na.Balances3.MachineBal.ToString("#,##0.00");
                txtBox38.Text = Na.BalDiff3.Machine.ToString("#,##0.00");
            }

            if (Na.BalSets == 4)
            {
                lbel28.Text = Na.Balances4.CurrNm;
                txtBox29.Text = Na.Balances4.CountedBal.ToString("#,##0.00");
                txtBox39.Text = Na.Balances4.MachineBal.ToString("#,##0.00");
                txtBox40.Text = Na.BalDiff4.Machine.ToString("#,##0.00");
            }

            // SHOW ... SHOW ... SHOW 

            //panel1.Show();
            //label24.Show();
            //label19.Show(); textBox30.Show(); textBox35.Show(); textBox36.Show();
            //label25.Show(); textBox31.Show(); textBox37.Show(); textBox38.Show();
            //label28.Show(); textBox29.Show(); textBox39.Show(); textBox40.Show();

            //if (Na.BalSets == 1)
            //{
            //    label19.Hide(); textBox30.Hide(); textBox35.Hide(); textBox36.Hide();
            //    label25.Hide(); textBox31.Hide(); textBox37.Hide(); textBox38.Hide();
            //    label28.Hide(); textBox29.Hide(); textBox39.Hide(); textBox40.Hide();
            //}
            //if (Na.BalSets == 2)
            //{
            //    label25.Hide(); textBox31.Hide(); textBox37.Hide(); textBox38.Hide();
            //    label28.Hide(); textBox29.Hide(); textBox39.Hide(); textBox40.Hide();
            //}
            //if (Na.BalSets == 3)
            //{
            //    label28.Hide(); textBox29.Hide(); textBox39.Hide(); textBox40.Hide();
            //}
            //if (Na.BalSets == 4)
            //{
            //    // HIDE Nothing
            //}
        }
      
        // USE ATM FIGURES
        protected void ButtonUseATMsFigures_Click(object sender, EventArgs e)
        {

            int WFunction = 2;
            string WAtmNo = (string)Session["WAtmNo"];
            int WSesNo = (int)Session["WReplCycle"];

            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, WFunction); // Read Values from NOTES

            // INITIALISE FORM 
            //     panel1.Hide();
            //label24.Hide();
            //label3.Hide();
            txtBox12.Text = " ";
            txtBox23.Text = " ";
            txtBox14.Text = " ";
            txtBox25.Text = " ";
            txtBox16.Text = " ";
            txtBox27.Text = " ";
            txtBox18.Text = " ";
            txtBox20.Text = " ";

            txtBox10.Text = " ";

            // SHOW 
            // Cassette 1
            txtBox1.Text = Na.Cassettes_1.RemNotes.ToString();
            txtBox5.Text = Na.Cassettes_1.RejNotes.ToString();

            // Cassette 2
            txtBox2.Text = Na.Cassettes_2.RemNotes.ToString();
            txtBox6.Text = Na.Cassettes_2.RejNotes.ToString();

            // Cassette 3
            txtBox3.Text = Na.Cassettes_3.RemNotes.ToString();
            txtBox7.Text = Na.Cassettes_3.RejNotes.ToString();

            // Cassette 4
            txtBox4.Text = Na.Cassettes_4.RemNotes.ToString();
            txtBox8.Text = Na.Cassettes_4.RejNotes.ToString();

            txtBox34.Text = Na.CaptCardsMachine.ToString();

            Na.Balances1.CountedBal = Na.Balances1.MachineBal;
            Na.Balances2.CountedBal = Na.Balances2.MachineBal;
            Na.Balances3.CountedBal = Na.Balances3.MachineBal;
            Na.Balances4.CountedBal = Na.Balances4.MachineBal;

            Na.BalDiff1.Machine = 0;
            Na.BalDiff2.Machine = 0;
            Na.BalDiff3.Machine = 0;
            Na.BalDiff4.Machine = 0;

            // Show differences in Notes  

            txtBox12.Text = "0";
            txtBox23.Text = "0";
            txtBox14.Text = "0";
            txtBox25.Text = "0";
            txtBox16.Text = "0";
            txtBox27.Text = "0";
            txtBox18.Text = "0";
            txtBox20.Text = "0";

            txtBox10.Text = "0";


            ShowBalances();// SHOW BALANCES 

            txtMessage.Text = " VERIFY FIGURES CHANGE THEM IF YOU WANT AND press UPDATE";

        }

        // Show Errors 
        protected void ButtonErrors_Click(object sender, EventArgs e)
        {
            Session["WOrigin"] = "WebForm152b";

            Server.Transfer("WebForm24.aspx");
        }
   
        // Go Back to Physical inspection 
        protected void ButtonBack_Click(object sender, EventArgs e)
        {
            Session["WOrigin"] = "WebForm152b";

            Server.Transfer("WebForm152a.aspx");
        }

        // Update 
        protected void ButtonUpdate_Click(object sender, EventArgs e)
        {
            int WFunction = 2;
            string WAtmNo = (string)Session["WAtmNo"];
            int WSesNo = (int)Session["WReplCycle"];

            // Read Session Notes 

            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, WFunction);


            if (int.TryParse(txtBox1.Text, out Na.Cassettes_1.CasCount))
            {
            }
            else
            {
                
                txtMessage.Text = "Please enter a valid number!"; 

                return;
            }


            if (int.TryParse(txtBox5.Text, out Na.Cassettes_1.RejCount))
            {
            }
            else
            {
                txtMessage.Text = "Please enter a valid number!";
                return;
            }


            if (int.TryParse(txtBox2.Text, out Na.Cassettes_2.CasCount))
            {
            }
            else
            {
                txtMessage.Text = "Please enter a valid number!";
                return;
            }


            if (int.TryParse(txtBox6.Text, out Na.Cassettes_2.RejCount))
            {
            }
            else
            {
                txtMessage.Text = "Please enter a valid number!";
                return;
            }


            if (int.TryParse(txtBox3.Text, out Na.Cassettes_3.CasCount))
            {
            }
            else
            {
                txtMessage.Text = "Please enter a valid number!";
                return;
            }


            if (int.TryParse(txtBox7.Text, out Na.Cassettes_3.RejCount))
            {
            }
            else
            {
                txtMessage.Text = "Please enter a valid number!";
                return;
            }


            if (int.TryParse(txtBox4.Text, out Na.Cassettes_4.CasCount))
            {
            }
            else
            {
                txtMessage.Text = "Please enter a valid number!";
                return;
            }


            if (int.TryParse(txtBox8.Text, out Na.Cassettes_4.RejCount))
            {
            }
            else
            {
                txtMessage.Text = "Please enter a valid number!";
                return;
            }

            // Captured Cards 

            if (int.TryParse(txtBox34.Text, out Na.CaptCardsCount))
            {
            }
            else
            {
                txtMessage.Text = "Please enter a valid number!";
                return;
            }

            // update input 

            Na.UpdateSessionsNotesAndValues(WAtmNo, WSesNo);

            // Read Session Notes to get the updated data 

            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, WFunction);

            // Show differences in Notes  

            txtBox12.Text = Na.Cassettes_1.DiffCas.ToString();
            txtBox23.Text = Na.Cassettes_1.DiffRej.ToString();
            txtBox14.Text = Na.Cassettes_2.DiffCas.ToString();
            txtBox25.Text = Na.Cassettes_2.DiffRej.ToString();
            txtBox16.Text = Na.Cassettes_3.DiffCas.ToString();
            txtBox27.Text = Na.Cassettes_3.DiffRej.ToString();
            txtBox18.Text = Na.Cassettes_4.DiffCas.ToString();
            txtBox20.Text = Na.Cassettes_4.DiffRej.ToString();

            txtBox10.Text = (Na.CaptCardsCount - Na.CaptCardsMachine).ToString();

            ShowBalances();// SHOW BALANCES 

            // Check if differences 
            if (Na.Cassettes_1.DiffCas == 0 & Na.Cassettes_1.DiffRej == 0 &
                Na.Cassettes_2.DiffCas == 0 & Na.Cassettes_2.DiffRej == 0 &
                Na.Cassettes_3.DiffCas == 0 & Na.Cassettes_3.DiffRej == 0 &
                Na.Cassettes_4.DiffCas == 0 & Na.Cassettes_4.DiffRej == 0
                )
            {
                if (Na.ErrJournalThisCycle > 0 )
                {
                    if (CheckBoxApprove.Checked == false)
                    {
                        txtMessage.Text = "You may have to recount money! Approve to continue. ";

                    return; 
                    }
                    
                
                    //if (MessageBox.Show("Warning: Balances reconcile but there are error/s. Do you want to examine errors?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    //                 == DialogResult.Yes)
                    //{
                    //    bool Replenishment = true;
                    //    string SearchFilter = "AtmNo = '" + WAtmNo + "'" + " AND SesNo =" + WSesNo + " AND (ErrType = 1) AND OpenErr =1";
                    //    NForm24 = new Form24(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo, "", Replenishment, SearchFilter);
                    //    NForm24.Show();
                    //    return;
                    //}

                }
                else
                {
                    txtMessage.Text = "ALL WELL FIGURES AT ATM LEVEL RECONCILE";

                }
            }
            else
            {
                if (Na.BalDiff1.Machine == Na.Balances1.PresenterValue)
                {
                    txtMessage.Text = "WARNING: THERE ARE DIFFERENCES and your presented errors is of the same value =" + Na.Balances1.PresenterValue;
                   
                    lbel3.Text = "Difference Same as Errors Value";
                }

                if (Na.BalDiff1.Machine != Na.Balances1.PresenterValue & CheckBoxApprove.Checked == false)
                {
                    //txtMessage.Text = "WARNING: THERE ARE DIFFERENCES. Your presented errors which are "
                    //    + Na.Balances1.PresenterValue + " are not of the same value";

                    txtMessage.Text = "WARNING: THERE ARE DIFFERENCES. If this the case please approve to update"; 

                    lbel3.Text = "Difference Not Same as Errors Value";
                
                    return; 
                    
                }
              //  button2.Show();
            }

           

            //   MessageBox.Show("Input Data has been updated.");

            //Form2MessageBox Mb = new Form2MessageBox("Input Data has been updated.");
            //Mb.StartPosition = FormStartPosition.Manual;
            //Mb.Location = new Point(300, 480);
            //Mb.ShowDialog();
            // STEPLEVEL UPDATING

            Session["Step2Status"] = "Updated";

            txtMessage.Text = "Updating completed. - Move to next step.";
           
        }
// Approval 
        protected void CheckBoxApprove_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckBoxApprove.Checked == true)
            {
                Session["Step2Status"] = "Updated";

                txtMessage.Text = "Updating completed following your approval - Move to next step.";
            }
            if (CheckBoxApprove.Checked == false)
            {
                Session["Step2Status"] = "NotUpdated";

                txtMessage.Text = "Input info and press update";
            }

        }
       
    }
}