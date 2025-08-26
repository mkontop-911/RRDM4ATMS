using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RRDM4ATMs;

public partial class webFrom1 : System.Web.UI.Page
{

    RRDMBanks Ba = new RRDMBanks();

    RRDMUsersRecords Us = new RRDMUsersRecords();
  
    RRDMGasParameters Gp = new RRDMGasParameters();

    int WAction;

    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void lnkToATM_Click(object sender, EventArgs e)
    {
        // Prepare Session and Go to FORM47

        string WSecLevel = (string)Session["WSecLevel"];

        if (WSecLevel != "03")
        {
            //   MessageBox.Text = "THIS BUTTON IS ONLY FOR OPERATIONAL OFFICERS";
            return;
        }

        WAction = 1; // Show INFO FOR ATMS 

        Session["WAction"] = WAction;

        //=============================================================
        Session["WOrigin"] = "webFrom1";

        //Server.Transfer("WebForm47.aspx");
        //  Response.Redirect("WebForm47.aspx");
        Response.Redirect("myAtms.aspx");
    }


    // Link To Diputes 
    protected void lnkToDISP_Click(object sender, EventArgs e)
    {
        string WSecLevel = (string)Session["WSecLevel"];

        if (WSecLevel != "03")
        {
            //   MessageBox.Text = "THIS BUTTON IS ONLY FOR OPERATIONAL OFFICERS";
            return;
        }

        WAction = 1; // Show INFO FOR ATMS 

        Session["WAction"] = WAction;

        //=============================================================
        Session["WOrigin"] = "webFrom1";

        //Server.Transfer("WebForm47.aspx");
        //  Response.Redirect("WebForm47.aspx");
        Response.Redirect("disputes_Pre_Inv.aspx");
    }
}