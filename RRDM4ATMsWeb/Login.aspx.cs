using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RRDM4ATMs;

namespace RRDM4ATMsWeb
{
    public partial class Login : System.Web.UI.Page
    {

        RRDMBanks Ba = new RRDMBanks();

        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord(); // Make class availble 

        RRDMGasParameters Gp = new RRDMGasParameters();
  
        string WOperator;

        int WSecLevel;

        string WSignedId;
        int WSignRecordNo;
        string WPassword;
        // LOAD FORM 
        protected void Page_Load(object sender, EventArgs e)
        {

            if (DropDownListUsers.SelectedValue == "1005")
            {
                //       TextBoxPassword.Text = "12345678";
            }

            MessageBox.Text = " Please Enter Id and Password ";

        }

        // Login Button

        protected void Button1_Click(object sender, EventArgs e)
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

                if (TextBoxPassword.Text == "")
                {
                    MessageBox.Text = "Please enter your password";
                    return;
                }

                WSignedId = DropDownListUsers.SelectedValue;

                WPassword = TextBoxPassword.Text;

                // =============================================
                Us.ReadUsersRecord(WSignedId); // Read USER record for the signed user

                // ===========================================
                if (Us.RecordFound == false)
                {
                    MessageBox.Text = " User Not Found ";
                    return;
                }
                else if (WPassword != Us.PassWord)
                {
                    MessageBox.Text = " Wrong User or Password ";
                    return;
                }

                Gp.ReadParametersSpecificId(Us.Operator, "451", "2", "", "");
                int Temp = ((int)Gp.Amount);

                if (DateTime.Now > Us.DtToBeChanged)
                {
                    MessageBox.Text = "Your Password has expired";
                    return;
                }

                if (DateTime.Now > Us.DtToBeChanged.AddDays(-Temp))
                {
                    int TempRem = Convert.ToInt32((Us.DtToBeChanged - DateTime.Now).TotalDays);
                    MessageBox.Text = "Days reamaining to change your password : " + TempRem.ToString();
                }

                if (Us.ForceChangePassword == true)
                {
                    MessageBox.Text = "Please Change password";
                    return;
                }

                Us.DtChanged = DateTime.Now;
                Us.DtToBeChanged = DateTime.Now.AddDays(Temp);
                Us.ForceChangePassword = true;

                WOperator = Us.Operator;
                WSecLevel = Us.SecLevel;

                if (WSecLevel == 7 & Us.PassWord == "123")
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

                Ba.ReadBank(WOperator);

                Us.UserId = WSignedId;

                Us.Culture = DropDownListUsers.SelectedValue;

                Us.DtTmIn = DateTime.Now;
                Us.DtTmOut = DateTime.Now;
                Us.Replenishment = false;
                Us.Reconciliation = false;
                Us.OtherActivity = true;

                Us.InsertSignedActivity(WSignedId);

                Us.ReadSignedActivity(WSignedId); // Read to get key of record 

                WSignRecordNo = Us.SignRecordNo;

                // Prepare Session and Go to FORM1

                Session["WOrigin"] = "WebForm40";

                Session["WSignedId"] = WSignedId;

                Session["WSignRecordNo"] = WSignRecordNo;

                Session["WSecLevel"] = Us.SecLevel;

                Session["WOperator"] = WOperator;

                string WSessionId = (string)Session.SessionID;

                Server.Transfer("WebFormMain.aspx");

            }
            else
            {
                MessageBox.Text = "Page is In Valid";
                return;
            }

        }
        // Custome Validation 
        protected void CustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
        {

            //    DateTime.ParseExact(args.Value, "m/d/yyyy",
            //        System.Globalization.DateTimeFormatInfo.InvariantInfo);
            //   args.IsValid = true;

            if (args.Value == "12345678")
            {
                args.IsValid = true;
            }
            else
            {
                args.IsValid = false;
            }

        }
    }
}
