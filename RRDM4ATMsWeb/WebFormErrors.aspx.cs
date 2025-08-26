using System;

namespace WebApplication52_Alex
{
    public partial class WebFormErrors : System.Web.UI.Page
    {
        // THIS IS A SYSTEM ERROR FORM 

        string WErrorOutput; 

        protected void Page_Load(object sender, EventArgs e)
        {
            WErrorOutput = (string)Session["WErrorOutput"];
            ErrMessageBody.Text = WErrorOutput; 
        }
        // GO BACK TO MAIN MENU 
        protected void Button1_Click(object sender, EventArgs e)
        {
            Session["WOrigin"] = "WebFormErrors";

            string WSessionId = (string)Session.SessionID;

            Server.Transfer("WebFormMain.aspx");  

        }
    }
}