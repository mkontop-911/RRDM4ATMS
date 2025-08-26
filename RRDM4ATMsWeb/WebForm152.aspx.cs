using System;
using System.Web.UI;
using RRDM4ATMs;

namespace RRDM4ATMsWeb
{
    public partial class WebForm152 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
              //  labelDescription.Text = "REPLENISHMENT WF"; 
                string WSignedId = (string)Session["WSignedId"];

                string FilterExpression = "AUTHUSER = '" + WSignedId + "'";

                SqlDataSource1.FilterExpression = FilterExpression;

                txtMessage.Text = " This shows information only for your ATMs ";
           
            }
            else
            {
                //  MessageBox.Text = " This is a PostBack";
            }

        }
        // Selected Index for ATMs Main 
        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["WIndex1"] = GridView1.SelectedIndex;

            NewSelectedIndexGridViw1(sender, e);
        }

        private void NewSelectedIndexGridViw1(object sender, EventArgs e)
        {

            // GridView1.SelectedIndex=Indx2;

            if (GridView1.SelectedDataKey != null)
            {
                Session["WAtmNo"] = GridView1.SelectedDataKey.Value.ToString();

                Label3.Text = "STATUS OF CHOSEN : " + (string)Session["WAtmNo"];
            }
        }
        // Selected Index for Replenishment Cycles  
        protected void GridView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["WIndex2"] = GridView2.SelectedIndex;

            NewSelectedIndexGridViw2(sender, e);

        }

        private void NewSelectedIndexGridViw2(object sender, EventArgs e)
        {
            if (GridView2.SelectedDataKey != null)
            {
                Session["WReplCycle"] = GridView2.SelectedDataKey.Value;
                Label3.Text = Label3.Text + " WITH REPL CYCLE : " + (int)Session["WReplCycle"];
            }
        }

        // Set index for For ATMs 
        protected void GridView1_DataBound(object sender, EventArgs e)
        {
            int Indx;
            if ((string)Session["WOrigin"] == "WebForm51")
            {
                Indx = (int)Session["WIndex1"];
            }
            else
            {
                Indx = 0;
                Session["WIndex1"] = Indx;
            }

            GridView1.SelectedIndex = Indx;
            NewSelectedIndexGridViw1(sender, e);
        }

        // Set Index for Repl Cycles 
        protected void GridView2_DataBound(object sender, EventArgs e)
        {
            int Indx;
            if ((string)Session["WOrigin"] == "WebForm51")
            {
                Indx = (int)Session["WIndex2"];
            }
            else
            {
                Indx = 0;
                Session["WIndex2"] = Indx;
            }

            GridView2.SelectedIndex = Indx;
            NewSelectedIndexGridViw2(sender, e);

        }
        // Proceed to next steps for replenishment 
        protected void Button1_Click(object sender, EventArgs e)
        {
            ZeroData(); 

            Session["WOrigin"] = "WebForm152";

            Session["WStep1Status"] = "NotUpdated";
            Session["WStep2Status"] = "NotUpdated";
            Session["WStep3Status"] = "NotUpdated";
            Session["WStep4Status"] = "NotUpdated";
            Session["WStep5Status"] = "NotUpdated";

            Server.Transfer("WebForm152a.aspx");

        }

        // Zero DATA METHOD 
        //   
        //
        protected void ZeroData()
        {

            RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances(); // Activate Class 

            RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

            int WFunction = 2;
            string WAtmNo = (string)Session["WAtmNo"];
            int WSesNo = (int)Session["WReplCycle"];

            // Read Session Notes 

            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, WFunction);


   ////         WEBCDepositsClass Da = new WEBCDepositsClass();
           

   //         if (WSignedId == "1005")
   //         {
   //             if (WAtmNo == "AB102")
   //             {
   //                 //    CurrentSessionNo = 3144;
   //                 WSesNo = 3144;
   //             }
   //             /*
   //             if (WAtmNo == "AB104")
   //             {
   //                 CurrentSessionNo = 5174;
   //                 WSesNo = 5174;
   //             }
   //              */
   //         }

   //         if (WAtmNo == "12507")
   //         {

   //             //   CurrentSessionNo = 1122;
   //             WSesNo = 1122;
   //         }

   //         if (WSignedId == "03ServeUk")
   //         {
   //             if (WAtmNo == "ServeUk102")
   //             {

   //                 WSesNo = 6694;
   //             }

   //         }
   //         if (WSignedId == "03ServeUk")
   //         {
   //             if (WAtmNo == "ABC501")
   //             {

   //                 WSesNo = 6695;
   //             }

   //         }
            // Update Physical Data
            //Na.ReadSessionsNotesAndValues3PhyCheck(WAtmNo, WSesNo);

            //Na.PhysicalCheck1.NoChips = false;
            //Na.PhysicalCheck1.NoCameras = false;
            //Na.PhysicalCheck1.NoSuspCards = false;
            //Na.PhysicalCheck1.NoGlue = false;
            //Na.PhysicalCheck1.NoOtherSusp = false;

            //Na.PhysicalCheck1.OtherSuspComm = "";

            //Na.UpdateSessionsNotesAndValues3PhyCheck(WAtmNo, WSesNo);

            // CASSETTES COUNT AND CAPTURED CARDS 
            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);

            Na.Cassettes_1.CasCount = 0;

            Na.Cassettes_1.RejCount = 0;

            Na.Cassettes_2.CasCount = 0;

            Na.Cassettes_2.RejCount = 0;

            Na.Cassettes_3.CasCount = 0;

            Na.Cassettes_3.RejCount = 0;


            Na.Cassettes_4.CasCount = 0;

            Na.Cassettes_4.RejCount = 0;


            // Captured Cards 

            Na.CaptCardsCount = 0;

            Na.UpdateSessionsNotesAndValues(WAtmNo, WSesNo);

            //// DEPOSITS 

            //Da.ReadDepositsSessionsNotesAndValuesDeposits(WAtmNo, WSesNo);

            //Da.DepositsCount1.Trans = 0;

            //Da.DepositsCount1.Notes = 0;

            //Da.DepositsCount1.Amount = 0;

            //Da.DepositsCount1.NotesRej = 0;

            //Da.DepositsCount1.AmountRej = 0;

            //Da.DepositsCount1.Envelops = 0;

            //Da.DepositsCount1.EnvAmount = 0;

            //// CHEQUES
            ////
            //Da.ChequesCount1.Trans = 0;


            //Da.ChequesCount1.Number = 0;

            //Da.ChequesCount1.Amount = 0;

            //Da.UpdateDepositsSessionsNotesAndValuesWithCount(WAtmNo, WSesNo); // UPDATE INPUT VALUES


            //     Replenishement 

            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);

            Na.Cassettes_1.NewInUser = 0;

            Na.Cassettes_2.NewInUser = 0;

            Na.Cassettes_3.NewInUser = 0;

            Na.Cassettes_4.NewInUser = 0;

            // Update Notes balances with new in figures 

            Na.ReplMethod = 0;
            Na.InUserDate = new DateTime(2050, 11, 21);
            Na.InReplAmount = 0;

            Na.ReplAmountTotal = 0;

            Na.UpdateSessionsNotesAndValues(WAtmNo, WSesNo);

            Na.ReplUserComment = " ";

            Na.UpdateSessionsNotesAndValuesUserComment(WAtmNo, WSesNo);

            // Undo Process Mode in Ta.

            //  Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

            //  Ta.ProcessMode = 0;

            //   Ta.UpdateSessionsStatusTraces(WAtmNo, WSesNo);

        }
    }
}