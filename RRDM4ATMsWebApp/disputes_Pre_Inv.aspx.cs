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

public partial class disputes_Pre_Inv : System.Web.UI.Page
{

   // RRDMUpdateGrids Ug = new RRDMUpdateGrids();
  //  RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();
 //   RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();
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

            //Am.ReadAtmsMainForAuthUserAndFillTable(WOperator, WSignedId, "");

            //GridView1.DataSource = Am.TableATMsMainSelected.DefaultView;
            //GridView1.DataBind();


            //DataTable dt = new DataTable();
            //dt.Columns.AddRange(new DataColumn[5] { new DataColumn("AtmNo"), new DataColumn("ReplCycle"), new DataColumn("AtmName"), new DataColumn("RespBranch"), new DataColumn("AuthUser") });
            //dt.Rows.Add(1, "Row one", "bla bla bla", "asdasd", "asdasdad");
            //dt.Rows.Add(2, "Row two", "bla bla bla", "asdasd", "asdasdad");
            //dt.Rows.Add(3, "Row three", "bla bla bla", "asdasd", "asdasdad");
            //dt.Rows.Add(4, "Row four", "bla bla bla", "asdasd", "asdasdad");
            //GridView1.DataSource = dt;
            //GridView1.DataBind();
            //GridView1.HeaderRow.TableSection = TableRowSection.TableHeader;

           
            Label7.Text = DateTime.Now.ToShortDateString();

            Label5.Text = WOperator;
       
            MsgGuidance.Text = "MAKE SELECTION";

            Session["CurrentPage"] = "1";
            Session["NbOfPage"] = "0";
        }
        else
        {


            //  MessageBox.Text = " This is a PostBack";
        }

       // GridView1.HeaderRow.TableSection = TableRowSection.TableHeader;
    }

  

    


    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
   



    //
    // SHOW 
    //
    protected void Button6_Click(object sender, EventArgs e)
    {
        if (RadioButtonTrace.Checked == false & RadioButtonRRN.Checked == false & RadioButtonATM.Checked == false)
        {
            PopUpMessage("Please Make Selection for type of search");
            return; 
        }
        if (RadioButtonTrace.Checked == true )
        {
            if (TextBoxInput.Text == "" || TextBoxATM.Text == "")
            {
                PopUpMessage("Please input both. Trace number and ATM number");
                return;
            }
        }
        if (RadioButtonRRN.Checked == true)
        {
            if (TextBoxInput.Text == "")
            {
                PopUpMessage("Please input RRN.");
                return;
            }
            if (TextBoxATM.Text == "")
            {
                PopUpMessage("Please do not input ATM no.");
                return;
            }
        }
        if (RadioButtonTrace.Checked == true || RadioButtonRRN.Checked == true || RadioButtonATM.Checked == true)
        {
            // We need dates
            if (TextBoxFromDt.Text == "" || TextBoxToDt.Text == "")
            {
                PopUpMessage("Please set both dates");
                return;
            }
            if (RadioButtonTrace.Checked == true || RadioButtonRRN.Checked == true)
            {
                if (TextBoxInput.Text!="" || TextBoxATM.Text != "")
                {
                    PopUpMessage("Please input both. Trace number and ATM number");
                    return;
                }
            }
            if (RadioButtonATM.Checked == true)
            {
                if (TextBoxInput.Text == "" || TextBoxATM.Text != "")
                {
                    // OK
                    // Check if ATM Exists
                    Am.ReadAtmsMainSpecific(TextBoxATM.Text);
                    if (Am.RecordFound == true)
                    {
                        // OK
                    }
                    else
                    {
                        // Not Found 
                        PopUpMessage("Input ATM Not Found!");
                        return;
                    }

                }
                else
                {
                    PopUpMessage("Please input a valid ATM Only!");
                    return;
                }
            }

        }

        // DATES Validation
        DateTime dtFrom;
        DateTime dtTo; 
        if (DateTime.TryParse(TextBoxFromDt.Text, out dtFrom))
        {
        }
        else
        {
            PopUpMessage("Please input Correct Date From!");
            return;
        }
        if (DateTime.TryParse(TextBoxToDt.Text, out dtTo))
        {
        }
        else
        {
            PopUpMessage("Please input Correct Date To!");
            return;
        }
        //  DateTime dtFrom = DateTime.Parse(TextBoxFromDt.Text);
       // DateTime dtTo = DateTime.Parse(TextBoxToDt.Text);
        Session["WAtmNo"] = TextBoxATM.Text; 

        Session["WdtFrom"] = dtFrom;
        Session["WdtTo"] = dtTo;

        Session["WIndex"] = 0; // Initialise Index.

        Session["PreviousRow"] = 0; // Initialise Index.

        Session["WOrigin"] = "disputes_Pre_Inv";

        Session["WUniqueRecordId"] = 0 ;

        string WSessionId = (string)Session.SessionID;

        //Server.Transfer("WebForm48a.aspx");
        Server.Transfer("WebForm80b.aspx");

     //   PopUpMessage("INPUT DATA PLEASE"); 
    }
    private void PopUpMessage(string InMsg)
    {
        string ShowWindow;
        ShowWindow = "showMessage";
        string smsg = InMsg;

        ShowWindow = ShowWindow + "('" + smsg + "')";

        ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowWindow", ShowWindow, true);
    }

    protected void TextBoxTo_TextChanged(object sender, EventArgs e)
    {

    }
    // Diputes 
    protected void RadioButtonDisputes_CheckedChanged(object sender, EventArgs e)
    {
        //if (RadioButtonDisputes.Checked == true)
        //{
        //    RadioButtonATMs.Checked = false; 
        //}
    }
// ATMS 
    protected void RadioButtonATMs_CheckedChanged(object sender, EventArgs e)
    {
        //if (RadioButtonATMs.Checked == true)
        //{
        //    RadioButtonDisputes.Checked = false;
        //}
    }


}