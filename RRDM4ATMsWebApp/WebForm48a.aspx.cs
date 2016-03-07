using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Resources;
using System.Globalization;

using RRDM4ATMs; 

public partial class WebForm48a : System.Web.UI.Page
{
    int WProcessMode;

    RRDMUpdateGrids Ug = new RRDMUpdateGrids();
    RRDMNotesBalances Na = new RRDMNotesBalances();
    RRDMTracesReadUpdate Ta = new RRDMTracesReadUpdate();
   
    RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();
    RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();

    string WUserBankId;

    int WSesNo;

    // string MsgFilter;

    string WSignedId;
    int WSignRecordNo;
    string WOperator;

    string WAtmNo;
    DateTime WDtFrom;
    DateTime WDtTo;

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

                Ta.ReadReplCyclesForFromToDate(WOperator, WAtmNo, WDtFrom, WDtTo);

                GridView1.DataSource = Ta.ATMsReplCyclesSelectedPeriod.DefaultView;
                GridView1.DataBind();

                string WSessionId = (string)Session.SessionID;
                

                //string FilterExpression = "BankId ='" + WOperator + "' AND AtmNo ='" + WAtmNo + "' AND SesDtTimeStart >='" + WDtFrom + "'"
                //    + " AND SesDtTimeStart <='" + WDtTo + "'";
                //SqlDataSource1.FilterExpression = FilterExpression;

                MsgGuidance.Text = "Be informed about Replenishment Cycles"; 
            }


        }
        catch (Exception ex)
        {

            string ErrorOutput = "An error occured in class ATMs Main position 270 ............. " + ex.Message;
        }

    }
    protected void GridView1_DataBound(object sender, EventArgs e)
    {

    }

    //
    protected void OnSelectedIndexChanged(object sender, EventArgs e)
    {

    }
    // Show 
    protected void Button6_Click(object sender, EventArgs e)
    {

    }
}