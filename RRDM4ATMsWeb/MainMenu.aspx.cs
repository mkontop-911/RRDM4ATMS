using System;

namespace RRDM4ATMsWeb
{
    public partial class MainMenu : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["UserID"] = "Testing...";

        }

 
    }
}