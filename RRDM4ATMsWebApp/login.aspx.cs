using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RRDM4ATMs; 

public partial class login : System.Web.UI.Page
{
    RRDMBanks Ba = new RRDMBanks();

    RRDMGasParameters Gp = new RRDMGasParameters();

    RRDMUsersRecords Us = new RRDMUsersRecords(); // Make class availble 

    RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();

    RRDMUsers_Applications_Roles Usr = new RRDMUsers_Applications_Roles();

    RRDMEncryptPasswordOrField En = new RRDMEncryptPasswordOrField();
    RRDMActiveDirectory Ad = new RRDMActiveDirectory();

    RRDMImages Ri = new RRDMImages();

    string WOperator;

    string WSecLevel;

    string WSignedId;
    int WSignRecordNo;
    string WPassword;
    // LOAD FORM 
    protected void Page_Load(object sender, EventArgs e)
    {       

        if (!Page.IsPostBack)
        {
            if (DropDownListUsers.SelectedValue == "PILOT_001")
            {
                TextBoxPassWord.Text = "12345678";
            }

        }
        else
        {
            //  MessageBox.Text = " This is a PostBack";
        }
    }
    // LOGIN
    string W_Application = "";

    protected void ButtonLogin_Click(object sender, EventArgs e)
    {
        // Check data not to be empty 
        if (Page.IsValid)
        {
            MessageBox.Text = "Page is Valid";

            if (DropDownListUsers.SelectedValue == "Blank")
            {
                MessageBox.Text = "Please enter your user Id";
                return;
            }

            if (TextBoxPassWord.Text == "")
            {
                MessageBox.Text = "Please enter your password";
                return;
            }

            WSignedId = DropDownListUsers.SelectedValue;

            WPassword = TextBoxPassWord.Text;

            // =============================================
            Us.ReadUsersRecord(WSignedId); // Read USER record for the signed user

           

                // =============================================

                try
                {
                    Us.ReadUsersRecord(WSignedId); // Read USER record for the signed user

                    if (Us.ErrorFound == true)
                    {
                    MessageBox.Text = "System Problem. Most likely SQL Not loaded yet" + Environment.NewLine
                                           + "Wait for few seconds and retry to login"
                                            ;
                        return;
                    }
                }
                catch (Exception ex)
                {

                  //CatchDetails(ex);
                    return;
                }


                // ===========================================
                if (Us.RecordFound == false)
                {
                //    MSG = LocRM.GetString("Form40MSG002", culture);
                //    MessageBox.Show(comboBox2.Text, MSG);
                ////   MessageBox.Show(" User Not Found ");
                MessageBox.Text = " User Not Found ";
                    return;
                }
                else
                {
                    bool PasswordMatched = En.CheckPassword(WPassword, Us.PassWord);

                    if (PasswordMatched == false)
                    {
                       
                    MessageBox.Text = " Wrong User or Password ";
                    return;
                    }


                }

                Gp.ReadParametersSpecificId(Us.Operator, "451", "2", "", "");
                int Temp = ((int)Gp.Amount);

                if (DateTime.Now > Us.DtToBeChanged)
                {
                // Your Password has expired
                MessageBox.Text = "Your Password has expired";
                    return;
                }

                if (DateTime.Now > Us.DtToBeChanged.AddDays(-Temp))
                {
                    // Your Password will expire in so many days
                    int TempRem = Convert.ToInt32((Us.DtToBeChanged - DateTime.Now).TotalDays);
                MessageBox.Text = "Days reamaining to change your password : " + TempRem.ToString();
                }

                if (Us.ForceChangePassword == true)
                {
                // Your Password must change 
                MessageBox.Text = "Please Change password";
                    return;
                }

                Us.DtChanged = DateTime.Now;
            Us.DtToBeChanged = DateTime.Now.AddDays(Temp);
            Us.ForceChangePassword = true;

            WOperator = Us.Operator;

            //if (radioButtonATMS_CARDS.Checked == true) W_Application = "ATMS/CARDS";
            //if (radioButtonNOSTRO.Checked == true) W_Application = "NOSTRO";
            //if (radioButtonVISA_SETTLEMENT.Checked == true) W_Application = "VISA SETTLEMENT";
            //if (radioButtonE_FINANCE_RECONCILIATION.Checked == true) W_Application = "E_FINANCE RECONCILIATION";
            //if (radioButtonFAWRY_RECONCILIATION.Checked == true) W_Application = "FAWRY RECONCILIATION";

            W_Application = "ATMS/CARDS";

            Usr.ReadUsersVsApplicationsVsRolesByApplication(Us.UserId, W_Application);
            if (Usr.RecordFound == true)
            {
                WSecLevel = Usr.SecLevel;
            }
            else
            {
                MessageBox.Text = "This user cannot access this application :.." + W_Application;
                return;
            }
           

            if (WSecLevel == "07" & Us.PassWord == "123")
            {
                // MessageBox.Show("Change your password please");

                MessageBox.Text = "Change your password please";
                return;
            }

            if (Us.PassWord == "99RESET9") // Password was reset  
            {
                // MessageBox.Show("Change your password please");
                MessageBox.Text = "Change your password please";
                return;
            }

            //Ba.ReadBank(WOperator);

            //Us.UserId = WSignedId;

            //Us.Culture = DropDownListUsers.SelectedValue;

            //Us.DtTmIn = DateTime.Now;
            //Us.DtTmOut = DateTime.Now;
            //Us.Replenishment = false;
            //Us.Reconciliation = false;
            //Us.OtherActivity = true;

            //Us.InsertSignedActivity(WSignedId);

            //Us.ReadSignedActivity(WSignedId); // Read to get key of record 

            //WSignRecordNo = Us.SignRecordNo;
            Usi.ReadSignedActivity(WSignedId); // Read to check if user is already signed in 
            //if (Usi.RecordFound == true & Usi.SignInStatus == true)
            //{
            //    if (MessageBox.Show("Our records show that you are already loged in the system. Do you want to force new login?", "Message",
            //                         MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            //                                         == DialogResult.Yes)
            //    {
            //        // Leave process to continue 
            //    }
            //    else
            //    {
            //        return;
            //    }
            //}

            Usi.UserId = WSignedId;

            Usi.UserName = Us.UserName;

            Usi.SecLevel = WSecLevel;

            Usi.Culture = "" ;

            Usi.SignInStatus = true;

            Usi.SignInApplication = W_Application;

            Usi.DtTmIn = DateTime.Now;
            Usi.DtTmOut = DateTime.Now;

            // INITIALISE 
            Usi.ATMS_Reconciliation = false;
            Usi.CARDS_Settlement = false;
            Usi.NOSTRO_Reconciliation = false;
            Usi.SWITCH_Reconciliation = false;

            // SET
            //if (radioButtonATMS_CARDS.Checked == true)
                Usi.ATMS_Reconciliation = true;
            //if (radioButtonNOSTRO.Checked == true)
            //    Usi.CARDS_Settlement = true;

            //if (radioButtonFAWRY_RECONCILIATION.Checked == true)
            //    Usi.E_FIN_Reconciliation = true;

            //if (radioButtonVISA_SETTLEMENT.Checked == true)
            //    Usi.NOSTRO_Reconciliation = true;
            //if (radioButtonE_FINANCE_RECONCILIATION.Checked == true)
            //    Usi.SWITCH_Reconciliation = true;

            Usi.Replenishment = false;
            Usi.Reconciliation = false;
            Usi.OtherActivity = true;

            WSignRecordNo = Usi.InsertSignedActivity(WSignedId);


            // Prepare Session and Go to FORM1

            Session["WOrigin"] = "WebForm40";

            Session["WSignedId"] = WSignedId;

            Session["WSignRecordNo"] = WSignRecordNo;

            Session["WSecLevel"] = WSecLevel;

            Session["WOperator"] = WOperator;

            string WSessionId = (string)Session.SessionID;

       //     Server.Transfer("WebFormMain.aspx");

            Response.Redirect("webFrom1.aspx");

        }
        else
        {
            MessageBox.Text = "Page is Invalid";
            return;
        }


        //string ShowWindow;
        //ShowWindow = "showMessage";
        //string smsg = "message to show";

        //ShowWindow = ShowWindow + "('" + smsg + "')";

        //ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowWindow", ShowWindow, true);


        //   ScriptManager.RegisterStartupScript(this, typeof(Page), "UpdateMsg", "alert('dasdsadsa')", true);

    }
}