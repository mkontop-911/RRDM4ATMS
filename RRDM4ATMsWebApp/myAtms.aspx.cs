using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Drawing;
using System.Globalization;

using RRDM4ATMs;

public partial class myAtms : System.Web.UI.Page
{

    RRDMUpdateGrids Ug = new RRDMUpdateGrids();
    RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();
    RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();
    RRDMAtmsMainClass Am = new RRDMAtmsMainClass();

    RRDMUsersRecords Us = new RRDMUsersRecords(); // Make class availble 
   
    RRDMUsersAccessToAtms Uaa = new RRDMUsersAccessToAtms();
    RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();
    RRDMAllowedAtmsAndUpdateFromJournalΧΧΧΧ Aj = new RRDMAllowedAtmsAndUpdateFromJournalΧΧΧΧ();

    // RRDMAtmsLocation Tl = new RRDMAtmsLocation();

    string WAtmNo;
    int WSesNo;
    int WRowIndex;

    string WBankId;

    string WSignedId;
    int WSignRecordNo;
    string WOperator;
    string WCitId;
    int WAction;

    protected void Page_Load(object sender, EventArgs e)
    {
        WOperator = (string)Session["WOperator"];
        WSignedId = (string)Session["WSignedId"];

        if (!Page.IsPostBack)
        {

            //Am.ReadAtmsMainForAuthUserAndFillTable(WOperator, WSignedId, "");
            WRowIndex = 1;

            Label7.Text = DateTime.Now.ToShortDateString();

            Label5.Text = WOperator;

            Am.ReadAtmsMainForAuthUserAndFillTable(WOperator, WSignedId, "");

            GridView1.DataSource = Am.TableATMsMainSelected.DefaultView;
            GridView1.DataBind();


            //DataTable dt = new DataTable();
            //dt.Columns.AddRange(new DataColumn[5] { new DataColumn("AtmNo"), new DataColumn("ReplCycle"), new DataColumn("AtmName"), new DataColumn("RespBranch"), new DataColumn("AuthUser") });
            //dt.Rows.Add(1, "Row one", "bla bla bla", "asdasd", "asdasdad");
            //dt.Rows.Add(2, "Row two", "bla bla bla", "asdasd", "asdasdad");
            //dt.Rows.Add(3, "Row three", "bla bla bla", "asdasd", "asdasdad");
            //dt.Rows.Add(4, "Row four", "bla bla bla", "asdasd", "asdasdad");
            //GridView1.DataSource = dt;
            //GridView1.DataBind();
            //GridView1.HeaderRow.TableSection = TableRowSection.TableHeader;


            TextBoxFrom.Text = "02/01/2014";

            MsgGuidance.Text = "Select Atm and see what is needed";

            Session["CurrentPage"] = "1";
            Session["NbOfPage"] = "0";
        }
        else
        {


            //  MessageBox.Text = " This is a PostBack";
        }

        GridView1.HeaderRow.TableSection = TableRowSection.TableHeader;
    }

    private void BindGrid()
    {
      //  GridView1.DataBind();
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
      
        if ((string)Session["WOrigin"] == "WebForm48b")
        {
            WRowIndex = (int)Session["WIndex"];
        }
        else
        {
         //   WRowIndex = 1;
            Session["WIndex"] = WRowIndex;
        }

        GridView1.SelectedIndex = WRowIndex;
        //NewSelectedIndex(sender, e);

    }


    public void GetRegistrationDateByDealer(int spageno, int snbofrows)
    {

        string SQL = "SELECT Top " + snbofrows + " * FROM (";

        SQL += "SELECT Fields to return ";

        SQL += " ROW_NUMBER() OVER (ORDER BY LoginName) AS num ";

        SQL += " FROM VdlPLSCategory ";

        SQL += ") AS a ";
        SQL += " Where num > " + (spageno - 1) * snbofrows;
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Button5_Click(object sender, EventArgs e)
    {

    }

    //protected void OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    //{
    //    if (e.Row.RowType == DataControlRowType.DataRow)
    //    {
    //        e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(GridView1, "Select$" + e.Row.RowIndex);
    //        //e.Row.ToolTip = "Click to select this row.";
    //    }
    //}

    protected void OnSelectedIndexChanged(object sender, EventArgs e)
    {
        // Get the currently selected row using the SelectedRow property.
        GridViewRow row = GridView1.SelectedRow;

      
        WRowIndex = GridView1.SelectedRow.RowIndex;

        WAtmNo = row.Cells[1].Text;

        Session["WAtmNo"] = WAtmNo;

        LabelHeadingRight.Text = "DETAILS FOR THE ATM No..:"+WAtmNo;


        Am.ReadAtmsMainSpecific(WAtmNo);

        Label1.Text = "CURRENT INFO FOR ALL ATMS" ;

        // Set up the Bank for this ATM
        WBankId = Am.BankId;

        TextBoxBank.Text = WBankId;

        if (Am.ProcessMode == -2)
        {
            if (Am.ProcessMode == -2)
            {
                //label26.Hide();
                //panel8.Hide();

                //label23.Hide();
                //panel4.Hide();

                PopUpMessage("This Atm is not active yet");

            }
            return;

        }
        else
        {
            //label26.Show();
            //panel8.Show();
            //if (WAction != 2)
            //{
            //    label23.Show();
            //    panel4.Show();
            //}

        }

        TextBoxBranch.Text = Am.BranchName;

        if (WAction == 3) // This is coming from a CIT company 
        {
            Us.ReadUsersRecord(WCitId);
            TextBoxOwnerUser.Text = Us.UserId;
            TextBoxName.Text = Us.UserName;
            TextBoxEmail.Text = Us.email;
            TextBoxMobile.Text = Us.MobileNo;

        }
        else
        {
            Uaa.FindUserForRepl(WAtmNo, 0);
            if (Us.RecordFound == true)
            {
                Us.ReadUsersRecord(Uaa.UserId); // Get Info for User 

            }
            else
            {
                Us.ReadUsersRecord(WSignedId);
                Us.RecordFound = false; // Initialise Record Found 
            }

            TextBoxOwnerUser.Text = Us.UserId;
            TextBoxName.Text = Us.UserName;
            TextBoxEmail.Text = Us.email;
            TextBoxMobile.Text = Us.MobileNo;
        }


        TextBoxReplCycleNo.Text = Am.CurrentSesNo.ToString();
        TextBoxLastReplDt.Text = Am.LastReplDt.ToString();
        TextBoxNextReplDt.Text = Am.NextReplDt.ToString();
        TextBoxCassettesAmnt.Text = Am.CurrCassettes.ToString("#,##0.00");
        TextBoxDepositedAmnt.Text = Am.CurrentDeposits.ToString("#,##0.00");

        TextBoxLastReconcDt.Text = Am.ReconcDt.ToString();
        //if (Am.ReconcDiff == true)
        //{
        //    TextBoxReconcDiff.Text = "YES";

        //}
        //else
        //{

        TextBoxReconcDiff.Text = "NO";

        //}

        TextBoxCurrency.Text = "";
        TextBoxAmountInDiff.Text = "";
        TextBoxSessionsInDiff.Text = Am.SessionsInDiff.ToString();
        TextBoxOutstandingErr.Text = Am.ErrOutstanding.ToString();

        Ec.ReadAllErrorsTableForCounters(WBankId, "EWB101", WAtmNo, 0, "");


        TextBoxInProcessForAction.Text = Ec.ErrUnderAction.ToString();


        if (Am.ErrOutstanding > 0 & WAction != 2)
        {
            //button10.Show();
        }
        else
        {
            //button10.Hide();
        }

        if (Ec.ErrUnderAction > 0)
        {
            //button11.Show();
        }
        else
        {
            //button11.Hide();
        }
        WSesNo = Am.CurrentSesNo;

        if (Am.ProcessMode == -1)
        {
            TextAreaStatus.Text = "Atm is currently serving customers";
            if (WAtmNo == "AB104") TextAreaStatus.Text = "Atm is currently serving customers." + " Examine if Replenishment or reconciliation is needed";
        }
        if (Am.ProcessMode == 0)
        {
            TextAreaStatus.Text = "Atm is currently ready for replenishment";
        }
        if (Am.ProcessMode == 1)
        {
            TextAreaStatus.Text = "Atm has been replenished";
        }
        if (Am.ProcessMode == 2)
        {
            TextAreaStatus.Text = "Atm has been fully reconciled";
        }
        if (Am.ProcessMode == 3)
        {
            TextAreaStatus.Text = "Atm has NOT been fully reconciled";
        }

        if (WAction == 2)
        {
            // READ ALL ERRORS AND SET COUNTER 

            Ec.ReadAllErrorsTableForCounters(WBankId, "EWB101", WAtmNo, 0, "");

            //textBox18.Text = Ec.NumOfErrors.ToString();
            //textBox17.Text = Ec.ErrUnderAction.ToString();
            //textBox5.Text = Ec.ErrUnderManualAction.ToString();

        }

        //BindGrid();
        //GridView1.HeaderRow.TableSection = TableRowSection.TableHeader;
    }


    //
    // SHOW 
    //
    protected void Button6_Click(object sender, EventArgs e)
    {
        //TxtMessage.Visible = false;

        //if (TxtFromDt.Text == "" || (DateTime)Session["WDtFrom"] > DateTime.Now)
        //{
        //    TxtMessage.Text = " Please Enter A Valid Date for From Date";
        //    TxtMessage.Visible = true;
        //    return;
        //}

        //if (TxtToDt.Text == "" || (DateTime)Session["WDtTo"] > DateTime.Now)
        //{
        //    TxtMessage.Text = " Please Enter A Valid Date for To Date";
        //    TxtMessage.Visible = true;
        //    return;
        //}



        if (RadioButton1.Checked == false & RadioButton2.Checked == false & RadioButton3.Checked == false)
        {

            PopUpMessage("Please Make your choice");

            return;
        }

        if (RadioButton1.Checked == true)
        {

            if (TextBoxTo.Text == "")
            {
                PopUpMessage("Please Enter A Valid Date for To Date");

                return;
            }

            string DateFrom = TextBoxFrom.Text;
            string DateTo = TextBoxTo.Text;

            // Format "02/01/2014" , "mm/dd/yyyy"

            DateTime dtFrom = DateTime.Parse(TextBoxFrom.Text);
            DateTime dtTo = DateTime.Parse(TextBoxTo.Text);

            Session["WdtFrom"] = dtFrom;
            Session["WdtTo"] = dtTo;

            Session["WOrigin"] = "myAtms";

            string WSessionId = (string)Session.SessionID;

            //Server.Transfer("WebForm48a.aspx");
            Server.Transfer("replenishmentCyclesForAtm.aspx");
        }






        //string ShowWindow;
        //ShowWindow = "showMessage";
        //string smsg = "message to show sdfvsd f sdf sd fsd ";

        //ShowWindow = ShowWindow + "('" + smsg + "')";

        //ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowWindow", ShowWindow, true);




        //   ScriptManager.RegisterStartupScript(this, typeof(Page), "UpdateMsg", "alert('dasdsadsa')", true);
    }
    private void PopUpMessage(string InMsg)
    {
        string ShowWindow;
        ShowWindow = "showMessage";
        string smsg = InMsg;

        ShowWindow = ShowWindow + "('" + smsg + "')";

        ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowWindow", ShowWindow, true);
    }

    // ON GO SHOW ONLY ONE ATM 
    protected void ButtonGo_Click(object sender, EventArgs e)
    {
        WAtmNo = TextBox1.Text;

        //Am.ReadAtmsMainForAuthUserAndFillTable(WOperator, WSignedId, WAtmNo);
        Am.ReadAtmsMainForAuthUserAndFillTable(WOperator, WSignedId, WAtmNo);

        GridView1.DataSource = Am.TableATMsMainSelected.DefaultView;
        GridView1.DataBind();

    }
    protected void Button4_Click(object sender, EventArgs e)
    {

    }
    protected void TextBoxTo_TextChanged(object sender, EventArgs e)
    {

    }
}