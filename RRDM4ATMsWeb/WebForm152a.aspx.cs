using System;
using System.Web.UI;
using RRDM4ATMs;

namespace RRDM4ATMsWeb
{
    public partial class WebForm152a : System.Web.UI.Page
    {
        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();
   //     RRDMCaseNotes Cn = new RRDMCaseNotes(); 
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // Show  Fraud Data
                string WAtmNo = (string)Session["WAtmNo"];
                int WSesNo = (int)Session["WReplCycle"];


                if (Na.ErrorFound == true)
                {
                    Session["WOrigin"] = "WebForm152a";
                    ShowError(Na.ErrorOutput);
                    return; 
                }

                //checkBoxNoChips.Checked = Na.PhysicalCheck1.NoChips;
                //checkBoxNoCameras.Checked = Na.PhysicalCheck1.NoCameras;
                //checkBoxNoSuspCards.Checked = Na.PhysicalCheck1.NoSuspCards;
                //checkBoxNoGlue.Checked = Na.PhysicalCheck1.NoGlue;
                //checkBoxNoOtherSusp.Checked = Na.PhysicalCheck1.NoOtherSusp;

                //Na.PhysicalCheck1.OtherSuspComm = TextBox1.Text;

                labelDescription.Text = "REPL.CYCLE INTRO";
                txtMessage.Text = "Do the Physical Inspection and update system";
            }
            
        }
        //
        // UPDATE PHYSICAL SECURITY CHECK
        //
        protected void ButtonUpdate_Click(object sender, EventArgs e)
        {          
            // Validation 

            if (checkBoxNoChips.Checked == false || checkBoxNoCameras.Checked == false || checkBoxNoSuspCards.Checked == false
                || checkBoxNoGlue.Checked == false || checkBoxNoOtherSusp.Checked == false)
            {
                if (String.IsNullOrEmpty(TextBox1.Text))
                {
                    txtMessage.Text = "Please fill comments if not all marked"; 
                    return;
                }
                else // There is value = something will be reported 
                {

                }
            }

            string WAtmNo = (string)Session["WAtmNo"];
            int WSesNo = (int)Session["WReplCycle"]; 

            // Read for updating Physical Data
            //Na.ReadSessionsNotesAndValues3PhyCheck(WAtmNo, WSesNo);
            if (Na.ErrorFound == true)
            {
                 Session["WOrigin"] = "WebForm152a";
                 ShowError(Na.ErrorOutput); 
            }

            //Na.PhysicalCheck1.NoChips = checkBoxNoChips.Checked;
            //Na.PhysicalCheck1.NoCameras = checkBoxNoCameras.Checked;
            //Na.PhysicalCheck1.NoSuspCards = checkBoxNoSuspCards.Checked;
            //Na.PhysicalCheck1.NoGlue = checkBoxNoGlue.Checked;
            //Na.PhysicalCheck1.NoOtherSusp = checkBoxNoOtherSusp.Checked;

            //Na.PhysicalCheck1.OtherSuspComm = TextBox1.Text;

            // Update Physical Data
            //Na.UpdateSessionsNotesAndValues3PhyCheck(WAtmNo, WSesNo);

            // STEPLEVEL UPDATING

            Session["Step1Status"] = "Updated";

            //       MessageBox.Show("Physical Inspection information is updated.");
            txtMessage.Text = "Physical Inspection information is updated for ATM. MOVE TO NEXT STEP.";

        }
        // Back Button 
       
        protected void ButtonBack_Click(object sender, EventArgs e)
        {
            Session["WOrigin"] = "WebForm152a";

            Session["WStep1Status"] = "NotUpdated";

            Server.Transfer("WebForm152.aspx");

        }
        // Go to next step 
        protected void ButtonNext_Click(object sender, EventArgs e)
        {
            Session["WOrigin"] = "WebForm152a";

            Server.Transfer("WebForm152b.aspx");
        }

        // Show error 
        private void ShowError(string InErrorOutput)
        {
            Session["WErrorOutput"] = InErrorOutput;
            Server.Transfer("WebFormErrors.aspx");  
        }
    }

}