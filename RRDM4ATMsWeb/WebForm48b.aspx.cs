using System;
using System.Web.UI;

namespace RRDM4ATMsWeb
{
    public partial class WebForm48b : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    string WOrigin = (string)Session["WOrigin"];

                    string WSignedId = (string)Session["WSignedId"];
                    int WSignRecordNo = (int)Session["WSignRecordNo"];
                    string WOperator = (string)Session["WOperator"];

                    string WAtmNo = (string)Session["WAtmNo"];

                    DateTime WDtFrom = (DateTime)Session["WDtFrom"];
                    DateTime WDtTo = (DateTime)Session["WDtTo"];

                    string WSessionId = (string)Session.SessionID;

                    string FilterExpression = "BankId ='" + WOperator + "' AND AtmNo ='" + WAtmNo + "' AND SesDtTimeStart >='" + WDtFrom + "'"
                        + " AND SesDtTimeStart <='" + WDtTo + "'";
                    SqlDataSource1.FilterExpression = FilterExpression;

                    MessageBox.Text = "Be informed about Replenishment Cycles";
                }


            }
            catch (Exception ex)
            {

                string ErrorOutput = "An error occured in class ATMs Main position 270 ............. " + ex.Message;
            }

        }

        // Go back 
        protected void Button1_Click(object sender, EventArgs e)
        {
            Session["WOrigin"] = "WebForm48b";

            Server.Transfer("WebForm47.aspx");
        }
        // Selected Row
        //   protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        //  {
        //     string StrSesNo = GridView1.SelectedRow.Cells[1].Text;

        //         string FilterExpression = "SesNo = '" + StrSesNo + "'";
        //         SqlDataSource2.FilterExpression = FilterExpression; 

        //    int rowindex = int.Parse(e.CommandArgument.ToString());

        //  }
       
    }
}