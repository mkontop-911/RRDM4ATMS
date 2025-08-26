using System;
using System.Web.UI;

namespace RRDM4ATMsWeb
{
    public partial class WebForm47 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                
                string WSignedId = (string)Session["WSignedId"];
               
                string FilterExpression = "AUTHUSER = '" + WSignedId + "'";

                SqlDataSource1.FilterExpression = FilterExpression;

                TxtMessage.Text = " This shows information only for your ATMs ";

                //TEST
                Calendar1.SelectedDate = new DateTime(2014, 02, 01);
                TxtFromDt.Text = Calendar1.SelectedDate.ToShortDateString();
                Session["WDtFrom"] = Calendar1.SelectedDate;

            }
            else
            {
                //  MessageBox.Text = " This is a PostBack";
            }

        }

        // Data bound for index 
        protected void GridView1_DataBound(object sender, EventArgs e)
        {
            int Indx;
            if ((string)Session["WOrigin"] == "WebForm48b")
            {
                Indx = (int)Session["WIndex"];
            }
            else
            {
                Indx = 0;
                Session["WIndex"] = Indx;
            }

            GridView1.SelectedIndex = Indx;
            NewSelectedIndex(sender, e);

        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {

            Session["WIndex"] = GridView1.SelectedIndex;

            NewSelectedIndex(sender, e);
        }

        private void NewSelectedIndex(object sender, EventArgs e)
        {

            // GridView1.SelectedIndex=Indx2;

            if (GridView1.SelectedDataKey != null)
            {
                Session["WAtmNo"] = GridView1.SelectedDataKey.Value.ToString();
            }
        }


        // Date 1
        protected void Calendar1_SelectionChanged(object sender, EventArgs e)
        {
            TxtFromDt.Text = Calendar1.SelectedDate.ToShortDateString();
            Session["WDtFrom"] = Calendar1.SelectedDate;
        }
        // Date 2
        protected void Calendar2_SelectionChanged(object sender, EventArgs e)
        {
            TxtToDt.Text = Calendar2.SelectedDate.ToShortDateString();
            Session["WDtTo"] = Calendar2.SelectedDate;
        }

        // Go to Replenishment 
        protected void ButtonShow_Click(object sender, EventArgs e)
        {

            TxtMessage.Visible = false;

            if (TxtFromDt.Text == "" || (DateTime)Session["WDtFrom"] > DateTime.Now)
            {
                TxtMessage.Text = " Please Enter A Valid Date for From Date";
                TxtMessage.Visible = true;
                return;
            }

            if (TxtToDt.Text == "" || (DateTime)Session["WDtTo"] > DateTime.Now)
            {
                TxtMessage.Text = " Please Enter A Valid Date for To Date";
                TxtMessage.Visible = true;
                return;
            }

            Session["WOrigin"] = "WebForm47";

            string WSessionId = (string)Session.SessionID;

            Server.Transfer("WebForm48b.aspx");
        }

        // Home 

        protected void ButtonHome_Click(object sender, EventArgs e)
        {
            Session["WOrigin"] = "WebForm47";

            Server.Transfer("WebFormMain.aspx");

        }
    }
}