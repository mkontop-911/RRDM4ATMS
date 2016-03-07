using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Drawing;

using RRDM4ATMs;

public partial class replenishmentCyclesForAtm : System.Web.UI.Page
{
    string filter;

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
        if (!Page.IsPostBack)
        {
            WOperator = (string)Session["WOperator"];
            WSignedId = (string)Session["WSignedId"];

            WDtFrom = (DateTime)Session["WdtFrom"];
            WDtTo = (DateTime)Session["WdtTo"];
            WAtmNo = (string)Session["WAtmNo"];

            //Am.ReadAtmsMainForAuthUserAndFillTable(WOperator, WSignedId, "");

            //GridView1.DataSource = Am.ATMsMainSelected.DefaultView;
            //GridView1.DataBind();

            //TextBoxFrom.Text = "02/01/2014";

            //MsgGuidance.Text = "Select Atm and see what is needed";

            Ta.ReadReplCyclesForFromToDate(WOperator, WAtmNo, WDtFrom, WDtTo);

            GridView1.DataSource = Ta.ATMsReplCyclesSelectedPeriod.DefaultView;
            GridView1.DataBind();

            Session["CurrentPage"] = "1";
            Session["NbOfPage"] = "0";
        }
        else
        {


            //  MessageBox.Text = " This is a PostBack";
        }

        //GridView1.HeaderRow.TableSection = TableRowSection.TableHeader;
    }

    private void BindGrid()
    {

    }

    protected void btnFirstPage_Click(object sender, EventArgs e)
    {
        Session["CurrentPage"] = "1";

        BindGrid();
        GridView1.HeaderRow.TableSection = TableRowSection.TableHeader;
    }

    protected void btnPreviousPage_Click(object sender, EventArgs e)
    {
        int icurpage = Convert.ToInt32(Session["CurrentPage"].ToString());

        icurpage--;

        if (icurpage <= 0)
            Session["CurrentPage"] = "1";
        else
            Session["CurrentPage"] = icurpage;

        BindGrid();
        GridView1.HeaderRow.TableSection = TableRowSection.TableHeader;
    }

    protected void btnNextPage_Click(object sender, EventArgs e)
    {
        int icurpage = Convert.ToInt32(Session["CurrentPage"].ToString());

        icurpage++;

        if (icurpage > Convert.ToInt32(Session["NbOfPage"].ToString()))
            Session["CurrentPage"] = Session["NbOfPage"].ToString();
        else
            Session["CurrentPage"] = icurpage;

        BindGrid();
        GridView1.HeaderRow.TableSection = TableRowSection.TableHeader;
    }

    protected void btnLastPage_Click(object sender, EventArgs e)
    {
        Session["CurrentPage"] = Session["NbOfPage"];

        BindGrid();
        GridView1.HeaderRow.TableSection = TableRowSection.TableHeader;
    }

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
        //NewSelectedIndex(sender, e);

    }

    protected void OnSelectedIndexChanged(object sender, EventArgs e)
    {
        GridViewRow row = GridView1.SelectedRow;
        
        WSesNo = int.Parse(row.Cells[1].Text);

        WAtmNo = (string)Session["WAtmNo"];
        WOperator = (string)Session["WOperator"];
        //WSesNo = InSesNo;
        Label2.Text = "REPL CYCLE INFORMATION FOR : " + WSesNo.ToString();

        Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

        if (Ta.RecordFound == false)
        {
            PopUpMessage("REPL CYCLE NUMBER NOT FOUND ");
            return;
        }

        WProcessMode = Ta.ProcessMode;

        int Function = 2;
        Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, Function);

        TextBox2.Text = WAtmNo;
        TextBox5.Text = WSesNo.ToString();

        TextBox6.Text = Ta.FirstTraceNo.ToString();
        TextBox7.Text = Ta.LastTraceNo.ToString();

        TextBox8.Text = (Ta.Stats1.NoOfTranCash + Ta.Stats1.NoOfTranDepCash + Ta.Stats1.NoOfTranDepCheq).ToString();
        TextBox9.Text = (Na.Balances1.OpenBal - Na.Balances1.MachineBal).ToString("#,##0.00");
        TextBox10.Text = Na.Balances1.MachineBal.ToString("#,##0.00");

        if (WProcessMode == -1)
        {
            TextBox4.Text = "IN PROCESS Repl Cycle";
        }
        if (WProcessMode >= 1)
        {
            TextBox4.Text = "COMPLETED Repl Cycle";
            TextBox1.Text = Ta.Repl1.ReplStartDtTm.ToString();
            TextBox11.Text = Ta.Repl1.ReplFinDtTm.ToString();
        }
        if (WProcessMode >= 2)
        {
            TextBox4.Text = "COMPLETED Reconciliation and Repl Cycle";
            TextBox12.Text = Ta.Recon1.RecStartDtTm.ToString();
            TextBox13.Text = Ta.Recon1.RecFinDtTm.ToString();
        }

        TextBox3.Text = Ta.Diff1.CurrNm1;
        TextBox14.Text = Ta.Diff1.DiffCurr1.ToString("#,##0.00");
        TextBox15.Text = Ta.SessionsInDiff.ToString();

        Ec.ReadAllErrorsTableForCounterReplCycle(WOperator, WAtmNo, WSesNo);

        TextBox17.Text = (Ec.NumOfErrors - Ec.ErrUnderAction).ToString();

        TextBox16.Text = Ec.NumOfErrors.ToString();

        if (Ec.NumOfErrors > 0)
        {
            Button1.Visible=true;
        }
        else
        {
            Button1.Visible = false;
        }

        Us.ReadUsersRecord(Ta.Repl1.SignIdRepl);
        TextBox18.Text = Us.UserName;

        Us.ReadUsersRecord(Ta.Recon1.SignIdReconc);
        TextBox19.Text = Us.UserName;
    }

    private void PopUpMessage(string InMsg)
    {
        string ShowWindow;
        ShowWindow = "showMessage";
        string smsg = InMsg;

        ShowWindow = ShowWindow + "('" + smsg + "')";

        ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowWindow", ShowWindow, true);
    }
}