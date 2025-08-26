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

public partial class WebForm67_BDC : System.Web.UI.Page
{

    int Fuid_A;
    int Ruid_A;
    int Fuid_B;
    int Ruid_B;

    public bool FoundRecord;

    string WPrintTraceDtTm;

    RRDMJournalReadTxns_Text_Class Ej = new RRDMJournalReadTxns_Text_Class();

    string WSignedId;
    int WSignRecordNo;
    string WOperator;

    //  string WJournalId;
    int WFuid;
    string WAtmNo;

    int WSeqNoA;
    int WSeqNoB;

    DateTime WDate_A;
    DateTime WDate_B;

    int WMode;

    string WTraceOrRRNumber;

    protected void Page_Load(object sender, EventArgs e)
    {
       
        if (!Page.IsPostBack)
        {

            WOperator = (string)Session["WOperator"];
            WSignedId = (string)Session["WSignedId"];
            
            WSignRecordNo = (int)Session["WSignRecordNo"];
       
            WFuid = (int)Session["WFuid"];  // THIS IS THE FUI Number 
            WAtmNo = (string)Session["WAtmNo"];

            WSeqNoA = (int)Session["WSeqNoA"];
            WSeqNoB = (int)Session["WSeqNoB"];

            WDate_A = (DateTime)Session["WDateA"];
            WDate_B = (DateTime)Session["WDateB"];

            WMode = (int)Session["WMode"];  // Mode 1 = single trace 
                                            // ---- Mode = 2 Whole Journal .... 
                                            // Mode = 3 working file in Journal
                                            // Mode = 4 Range of traces (from .. to) 
                                            // Mode = 5 Range of Fuid, Ruid ... It can also be used to get range 

            WTraceOrRRNumber = (string)Session["WTraceOrRRNumber"];



            Label7.Text = DateTime.Now.ToShortDateString();

            Label5.Text = WOperator;

            FoundRecord = false;

            if (WMode == 2)
            {
              
                Label1.Text = "JOURNAL LINES for ATM : " + WAtmNo;
               
            }
            if (WMode == 3)
            {

                Ej.CreateJournalLinesBasedonGivenFuid(WOperator, WSignedId, WAtmNo, WFuid);

                Ej.ReadJournalAndFillTableFrom_Fuid_Short(WOperator, WSignedId, WAtmNo
                                                      , WFuid, WMode);

                if (Ej.JournalLines.Rows.Count > 0)
                {
                    FoundRecord = true;

                    GridView1.DataSource = Ej.JournalLines.DefaultView;
                  
                }
                else
                {
                    FoundRecord = false;
                    MsgGuidance.Text = "No Data to show for this selection. ";
                    return;
                }

                Label1.Text = "Journal Lines for ATM : " + WAtmNo + " For File ID:.." + WFuid.ToString();
                // label1.Hide();
            }

            if (WMode == 5)
            {

                Ej.ReadJournalTxnsBySeqNoAndFind_Start_End(WOperator, WSeqNoA);

                //Ej.ReadJournalTxnsByTraceAndFind_Start_End(WOperator, WAtmNo, WTraceStart
                //                                   , WDate_A);
                if (Ej.RecordFound == true)
                {
                    Fuid_A = Ej.FuId;
                    Ruid_A = Ej.Sessionstart;
                    //
                    Ej.CreateJournalLinesBasedonGivenFuid(WOperator, WSignedId, WAtmNo, Fuid_A);
                    if (Ej.RecordFound == true)
                    {
                        // OK
                    }
                    else
                    {
                        MsgGuidance.Text = "Journal Is not created";
                       
                        return;
                    }
                }
                if (WSeqNoA != WSeqNoB)
                {
                    // Different Trace 
                    Ej.ReadJournalTxnsBySeqNoAndFind_Start_End(WOperator, WSeqNoB);
                    if (Ej.RecordFound == true)
                    {
                        Fuid_B = Ej.FuId;
                        Ruid_B = Ej.SessionEnd;
                        //
                        if (Fuid_B != Fuid_A)
                            Ej.CreateJournalLinesBasedonGivenFuid(WOperator, WSignedId, WAtmNo, Fuid_B);
                    }
                }
                else
                {
                    // Same Trace
                    // Get Info from previous read 
                    Fuid_B = Ej.FuId;
                    Ruid_B = Ej.SessionEnd;
                }
                //

                if (WSeqNoA != WSeqNoB)
                {
                    Label1.Text = "Journal Lines for ATM : " + WAtmNo + "..And FileId.." + Fuid_A.ToString()
                        + " And from line " + (Ruid_A).ToString() + " To line " + Ruid_B.ToString();
                    //    label1.Hide();
                }
                else
                {
                    Label1.Text = "Journal Lines for ATM : " + WAtmNo
                        + ".. And File Id .." + Fuid_A.ToString() + " And lines start.." + Ruid_A.ToString();
                    // label1.Hide();
                }

                // Fill UP TABLE AND REPORT 
                //
                Ej.ReadJournalAndFillTableFrom_Fuid_Ruid_To_Fuid_Ruid(WOperator, WSignedId, WAtmNo
                     , Fuid_A, Ruid_A, Fuid_B, Ruid_B, WMode);

                if (Ej.JournalLines.Rows.Count > 0)
                {
                    FoundRecord = true;

                    GridView1.DataSource = Ej.JournalLines.DefaultView;
                 
                }
                else
                {
                    FoundRecord = false;
                    MsgGuidance.Text = "No Data to show for this selection. "; 
                  
                    return; 
                }

            }


            // GridView1.DataSource = Am.TableATMsMainSelected.DefaultView;
            GridView1.DataBind();

            MsgGuidance.Text = "VIEW detail of Lines";

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
      
        //if ((string)Session["WOrigin"] == "WebForm48b")
        //{
        //    WRowIndex = (int)Session["WIndex"];
        //}
        //else
        //{
        // //   WRowIndex = 1;
        //    Session["WIndex"] = WRowIndex;
        //}

        //GridView1.SelectedIndex = WRowIndex;
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




            //Session["WdtFrom"] = dtFrom;
            //Session["WdtTo"] = dtTo;

            Session["WOrigin"] = "myAtms";

            string WSessionId = (string)Session.SessionID;

            //Server.Transfer("WebForm48a.aspx");
            Server.Transfer("replenishmentCyclesForAtm.aspx");
        }



    
// FINISH => GO BACK TO 
    protected void ButtonFinish_Click(object sender, EventArgs e)
    {
        Session["WOrigin"] = "WebForm67_BDC.aspx";

        string WSessionId = (string)Session.SessionID;

        Server.Transfer("WebForm80b.aspx");
       
    }
}