using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Resources;
using System.Globalization;

using RRDM4ATMs;

public partial class WebForm80b : System.Web.UI.Page
{
    int WProcessMode;

    RRDMUpdateGrids Ug = new RRDMUpdateGrids();
    RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
    RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();
    RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

    RRDMUserSignedInRecords Us = new RRDMUserSignedInRecords();
    RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();

    RRDMTransAndTransToBePostedClass Tp = new RRDMTransAndTransToBePostedClass();


    DateTime NullPastDate = new DateTime(1900, 01, 01);

    string WUserBankId;

    int WSesNo;

    int WRowIndex;
    int PreviousRow;

    int Mode;
    int PageSize;
    string SelectionCriteria;
    string SortCriteria;

    // string MsgFilter;
    string WOrigin;

    string WSignedId;
    int WSignRecordNo;
    string WOperator;
    int WUniqueRecordId; 

    string WAtmNo;

    DateTime WDtFrom;
    DateTime WDtTo;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!Page.IsPostBack)
            {
                //GridView1.AllowPaging = true;
                //GridView1.PageSize = 10;

                Label11.Text = "Historical Transactions";

                Label7.Text = DateTime.Now.ToShortDateString();

                Label5.Text = WOperator;

                WOrigin = (string)Session["WOrigin"];

                WSignedId = (string)Session["WSignedId"];
                WSignRecordNo = (int)Session["WSignRecordNo"];
                WOperator = (string)Session["WOperator"];

                WAtmNo = (string)Session["WAtmNo"];

                WDtFrom = (DateTime)Session["WDtFrom"];
                WDtTo = (DateTime)Session["WDtTo"];

                WRowIndex = (int)Session["WIndex"];

                WUniqueRecordId = (int)Session["WUniqueRecordId"]; 

                if (WUniqueRecordId > 0) PopulateFields(WUniqueRecordId);


                // Ta.ReadReplCyclesForFromToDate(WOperator, WAtmNo, WDtFrom, WDtTo);
                Mode = 1;
                PageSize = 20;
                SelectionCriteria = "";
                SortCriteria = " WHERE TerminalId ='" + WAtmNo + "'";
                Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable_Paging_WEB(WOperator, WSignedId, Mode, SelectionCriteria, SortCriteria, NullPastDate, NullPastDate, PageSize);
                GridView1.DataSource = Mpa.MatchingMasterDataTableATMs.DefaultView;
                GridView1.DataBind();

                string WSessionId = (string)Session.SessionID;

                //string FilterExpression = "BankId ='" + WOperator + "' AND AtmNo ='" + WAtmNo + "' AND SesDtTimeStart >='" + WDtFrom + "'"
                //    + " AND SesDtTimeStart <='" + WDtTo + "'";
                //SqlDataSource1.FilterExpression = FilterExpression;

                MsgGuidance.Text = "Shown As Per Selection";
            }

            GridView1.HeaderRow.TableSection = TableRowSection.TableHeader;

        }
        catch (Exception ex)
        {

            string ErrorOutput = "An error occured in class Mpa ............. " + ex.Message;
        }

    }
    protected void GridView1_DataBound(object sender, EventArgs e)
    {

        WRowIndex = (int)Session["WIndex"];

        GridView1.SelectedIndex = WRowIndex;

        Session["PreviousRow"] = WRowIndex ;

        GridView1.SelectedRow.BackColor = System.Drawing.Color.GreenYellow;

        GridViewRow row = GridView1.SelectedRow;
        string WUniqueNo = row.Cells[1].Text;

        WUniqueRecordId = Int32.Parse(WUniqueNo);

        Session["WUniqueRecordId"] = WUniqueRecordId;

        PopulateFields(WUniqueRecordId);

        //NewSelectedIndex(sender, e);
    }

    //
    protected void OnSelectedIndexChanged(object sender, EventArgs e)
    {
        PreviousRow = (int)Session["PreviousRow"];

        //if (PreviousRow != null)
        //{
            GridViewRow PreviousRowcolor = GridView1.Rows[PreviousRow];
            PreviousRowcolor.BackColor = System.Drawing.Color.White;
        //}

        // Get the currently selected row using the SelectedRow property.
        GridViewRow row = GridView1.SelectedRow;

        row.BackColor = System.Drawing.Color.GreenYellow;

        Session["WIndex"] = WRowIndex = GridView1.SelectedRow.RowIndex;
        Session["PreviousRow"] = WRowIndex = GridView1.SelectedRow.RowIndex;
        string WUniqueNo = row.Cells[1].Text;

        WUniqueRecordId = Int32.Parse(WUniqueNo);

        Session["WUniqueRecordId"] = WUniqueRecordId;

        PopulateFields(WUniqueRecordId);

        GridView1.SelectedIndex = WRowIndex;

    }
    // Show 
    protected void Button6_Click(object sender, EventArgs e)
    {

    }
    // Journal Lines 
    protected void ButtonJournalLines_Click(object sender, EventArgs e)
    {

        WRowIndex = (int)Session["WIndex"];

        if (WRowIndex == 0)
        {
            PopUpMessage("Please select transaction!");
            return;
        }

        int WUniqueRecordId = (int)Session["WUniqueRecordId"];
        Mpa.ReadInPoolTransSpecificUniqueRecordId(WUniqueRecordId,2);

        int WSeqNoA = 0;
        int WSeqNoB = 0;

        RRDMJournalReadTxns_Text_Class Ej = new RRDMJournalReadTxns_Text_Class();

        Ej.ReadJournalTxnsByParameters(Mpa.Operator, Mpa.TerminalId, Mpa.AtmTraceNo, Mpa.TransAmount, Mpa.CardNumber, Mpa.TransDate.Date);

        if (Ej.RecordFound)
        {
            WSeqNoA = Ej.SeqNo;
            WSeqNoB = Ej.SeqNo;

        }
        //
        // Bank De Caire
        //

        int Mode = 5; // Specific
        string WTraceRRNumber = Mpa.TraceNoWithNoEndZero.ToString();
        if (Mpa.TraceNoWithNoEndZero == 0 & Mpa.RRNumber != "") WTraceRRNumber = Mpa.RRNumber;
        //NForm67_BDC = new Form67_BDC(WSignedId, 0, WOperator, Mpa.FuID, WTraceRRNumber, Mpa.TerminalId, WSeqNoA, WSeqNoB, Mpa.TransDate, NullPastDate, Mode);
        //NForm67_BDC.ShowDialog();


        //Session["WOperator"] = WOperator;
        //Session["WSignedId"] = WSignedId;

        //Session["WSignRecordNo"] = WSignRecordNo;

        Session["WFuid"] = Mpa.FuID;  // THIS IS THE FUI Number 
        Session["WAtmNo"] = Mpa.TerminalId;

        Session["WSeqNoA"] = WSeqNoA;
        Session["WSeqNoB"] = WSeqNoB;

        Session["WDateA"] = Mpa.TransDate;
        Session["WDateB"] = NullPastDate;

        Session["WMode"] = Mode;  // Mode 1 = single trace 
                                  // ---- Mode = 2 Whole Journal .... 
                                  // Mode = 3 working file in Journal
                                  // Mode = 4 Range of traces (from .. to) 
                                  // Mode = 5 Range of Fuid, Ruid ... It can also be used to get range 

        Session["WTraceOrRRNumber"] = WTraceRRNumber;


        Session["WOrigin"] = "WebForm80b";

        string WSessionId = (string)Session.SessionID;

        //Server.Transfer("WebForm48a.aspx");
        Server.Transfer("WebForm67_BDC.aspx");

    }
    private void PopUpMessage(string InMsg)
    {
        string ShowWindow;
        ShowWindow = "showMessage";
        string smsg = InMsg;

        ShowWindow = ShowWindow + "('" + smsg + "')";

        ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowWindow", ShowWindow, true);
    }

    private void PopulateFields(int InUniqueRecordId)
    {
        Mpa.ReadInPoolTransSpecificUniqueRecordId(InUniqueRecordId,2);

        TextBoxRMCateg.Text = Mpa.RMCateg;
        TextBoxTerminal.Text = Mpa.TerminalId;

        TextBoxRMCycle.Text = Mpa.MatchingAtRMCycle.ToString();

        TextBoxCardNo.Text = Mpa.CardNumber;
        TextBoxAccNo.Text = Mpa.AccNumber;
        TextBoxCurr.Text = Mpa.TransCurr;
        TextBoxAmnt.Text = Mpa.TransAmount.ToString("#,##0.00");
        TextBoxDateTm.Text = Mpa.TransDate.ToString();
        // TextBoxTraceNo.Text = Mpa.TraceNoWithNoEndZero.ToString();
        TextBoxUniqueId.Text = Mpa.UniqueRecordId.ToString();

        if (Mpa.TraceNoWithNoEndZero > 0 & Mpa.Origin == "Our Atms")
        {
            TextBoxTraceNo.Text = Mpa.TraceNoWithNoEndZero.ToString();
            //  textBoxLineDetails.Text = "DETAILS OF SELECTED";
        }
        if (Mpa.RRNumber != "0" & Mpa.Origin != "Our Atms")
        {
            TextBoxTraceNo.Text = Mpa.RRNumber;
        }

        TextBoxMatchingMask.Text = Mpa.MatchMask;

        TextBoxMaskedFiles.Text = Mpa.FileId01 + " , " + Mpa.FileId02;

        if (Mpa.FileId03 != "") TextBoxMaskedFiles.Text += " , " + Mpa.FileId03;

        Tp.ReadTransToBePostedSpecificByUniqueRecordId(Mpa.UniqueRecordId);
        if (Tp.RecordFound == true)
        {
            LabelTransCreated.Visible = true;
            TextBoxTransCreated.Visible = true;
            LabelPosted.Visible = true;
            TextBoxPosted.Visible = true;

            TextBoxTransCreated.Text = Tp.OpenDate.ToString();
            if (Tp.ActionDate != NullPastDate)
            {
                TextBoxPosted.Text = Tp.ActionDate.ToString();
                //Tp.ReadTransToBePostedSpecificByUniqueRecordIdForReversal(Mpa.UniqueRecordId);
                //if (Tp.IsReversal == true) labelReversal.Show();
                //else labelReversal.Hide();
            }
            else TextBoxPosted.Text = "Not Posted yet.";
        }
        else
        {
            LabelTransCreated.Visible = false;
            TextBoxTransCreated.Visible = false;
            LabelPosted.Visible = false;
            TextBoxPosted.Visible = false;
        }

        if (Mpa.ActionType == "4")
        {
            LabelForcedMatched.Visible = true;
            LabelReason.Visible = true;
            TextBoxReason.Visible = true;
            TextBoxReason.Text = Mpa.MatchedType;

        }
        else
        {
            LabelForcedMatched.Visible = false;
            LabelReason.Visible = false;
            TextBoxReason.Visible = false;
        }
    }




    protected void TextBoxTraceNo_TextChanged(object sender, EventArgs e)
    {

    }
}