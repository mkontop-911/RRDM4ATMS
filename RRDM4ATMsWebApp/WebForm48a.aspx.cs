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
    RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();
    RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

    RRDMUserSignedInRecords Us = new RRDMUserSignedInRecords();
    RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();

    string WUserBankId;

    int WSesNo;

    // string MsgFilter;
    string WOrigin ;

    string WSignedId ;
    int WSignRecordNo ;
    string WOperator ;

    string WAtmNo;

    DateTime WDtFrom ;
    DateTime WDtTo ;


    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!Page.IsPostBack)
            {
                WOrigin = (string)Session["WOrigin"];

                WSignedId = (string)Session["WSignedId"];
                WSignRecordNo = (int)Session["WSignRecordNo"];
                WOperator = (string)Session["WOperator"];

                WAtmNo = (string)Session["WAtmNo"];

                WDtFrom = (DateTime)Session["WDtFrom"];
                WDtTo = (DateTime)Session["WDtTo"];

               // Ta.ReadReplCyclesForFromToDate(WOperator, WAtmNo, WDtFrom, WDtTo);

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