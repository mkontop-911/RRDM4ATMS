using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RRDM4ATMs;

namespace RRDM4ATMsWeb
{
    public partial class WebFormMain : System.Web.UI.Page
    {

        RRDMBanks Ba = new RRDMBanks();

        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord(); // Make class availble 

        RRDMGasParameters Gp = new RRDMGasParameters();

        int WAction;


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
               
            }
         //   MessageBox.Text = " Please choose your next step ";

        }
        // GO To MY ATMs Operation 
        protected void Button4_Click(object sender, EventArgs e)
        {
            // Prepare Session and Go to FORM47

            int WSecLevel = (int)Session["WSecLevel"];

            if (WSecLevel > 4)
            {
                //   MessageBox.Text = "THIS BUTTON IS ONLY FOR OPERATIONAL OFFICERS";
                return;
            }

            WAction = 1; // Show INFO FOR ATMS 
   
            Session["WAction"] = WAction;

            //=============================================================
            Session["WOrigin"] = "WebFormMain";

            Server.Transfer("WebForm47.aspx");
            //  Response.Redirect("WebForm47.aspx");

        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            
        }
        // Sign out 
        protected void Button2_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            Session.Clear();
            Response.Redirect("~/Default.aspx");
        }
        // Replenishment 
        protected void Button5_Click(object sender, EventArgs e)
        {
            Session["WOrigin"] = "WebFormMain";

            Session["WFunction"] = "Replenishment";

            Server.Transfer("WebForm152.aspx");
        }

       
        /*
        bool ReturnValue()
        {
            return false;
        }
         */
    }
}