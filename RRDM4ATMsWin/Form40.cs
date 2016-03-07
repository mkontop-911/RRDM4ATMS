using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Drawing.Printing;

using System.DirectoryServices;

using System.DirectoryServices.AccountManagement; 

//multilingual
using System.Resources;
using System.Globalization;
using RRDM4ATMs; 


namespace RRDM4ATMsWin
{
    public partial class Form40 : Form
    {
        //multilingual
        CultureInfo culture;

        bool WActiveDirectory;
        //string AdDomain;
        //string AdGroup;
        //string ParId ;
        //string OccurId ;

        string WOperator;
    
        int WSecLevel;
   
        string WSignedId;
        int WSignRecordNo;
        string WPassword;

        string MSG; 

        FormMainScreen NFormMainScreen;

        FormMainScreenCIT NFormMainScreenCIT;

        Form41 NForm41; 

        RRDMBanks Ba = new RRDMBanks(); 

        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord(); // Make class availble 

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMActiveDirectory Ad = new RRDMActiveDirectory(); 

        ResourceManager LocRM = new ResourceManager("RRDM4ATMsWin.appRes", typeof(Form40).Assembly);
 
        public Form40()
        {

            InitializeComponent();

            txtBoxPassword.PasswordChar = '*';

            pictureBox1.BackgroundImage = Properties.Resources.logo2;

            string DName = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;

            string PC_NotNet_DomainName = System.Environment.UserDomainName; 

            //
            // Check if Active Directory is applied 
            //
            // =============================================

            //public bool DomainFound; // Through .Net command
            //public bool ValidDomain; // Searching in Banks
            //public bool AdRRDMYes; // It was defined in Parameters that Active Directory was needed 
            //public bool UserInGroup; // User was checked and belongs to active directory group. 

            //
            // Check for electronic authorisation need
            //
            string ParId = "264";
            string OccurId = "1";
            //TEST
            WOperator = "CRBAGRAA"; 
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            if (Gp.OccuranceNm == "YES") // Active directory needed
            {
                Ad.CheckActiveDirectory();

                if (Ad.ErrorFound == true)
                {
                    MessageBox.Show(Ad.ErrorOutput);
                    return;
                }

                if (Ad.DomainFound & Ad.ValidDomain & Ad.AdRRDMYes & Ad.UserInGroup)
                {
                    WActiveDirectory = true;
                }

                if (WActiveDirectory == true)
                {
                    label1.Hide();
                    label2.Hide();
                    label3.Hide();

                    comboBox2.Hide();
                    txtBoxPassword.Hide();


                    panel1.Show();

                    buttonLogin.Text = "Proceed";
                    buttonChange.Hide();

                    WOperator = Ad.Operator;

                    WSignedId = Ad.UserId;

                    Us.ReadUsersRecord(Ad.UserId);
                    if (Us.RecordFound == true)
                    {
                        labelName.Text = Us.UserName;

                        // ===========================================
                        // Assign User fields 
                        //*********************************************

                        if (Ad.NameFromActive) Us.UserName = Ad.UserName;
                        if (Ad.MobileFromActive) Us.MobileNo = Ad.UserPhone;
                        if (Ad.EmailFromActive) Us.email = Ad.UserMail;

                        Us.UpdateUser(Ad.UserId); // Update User with details from active directory 
                    }
                    else
                    {
                        MessageBox.Show("Active Directory user not found in RRDM. Ask RRDM administrator to examine! ");
                        return;
                    }

                }
                else
                {
                    WActiveDirectory = false;

                    panel1.Hide();
                }
            }
            else
            {
                WActiveDirectory = false;
                panel1.Hide();
            }
        
            SetValues();
        }

        private void SetValues() 
        {
           
            //TEST
            comboBox2.Items.Add("1005");
            comboBox2.Items.Add("487116");
            comboBox2.Items.Add("500");
            comboBox2.Items.Add("03ServeUk");
            comboBox2.Items.Add("06ServeUk");
            comboBox2.Items.Add("BankMasterServeUk");
            
            comboBox2.Items.Add("999"); // Gas Master 

            comboBox2.Text = "1005";
            txtBoxPassword.Text = "123";

            if (comboBox2.Text == "1005")
            {
                txtBoxPassword.Text = "12345678";
            }

            comboBox1.Text = "English";

            //multilingual
            culture = CultureInfo.CurrentCulture;
            //culture = CultureInfo.CreateSpecificCulture("fr-FR");
            SetCulture();
        }


        private void SetCulture()
        {
            // Assign the string for the "strMessage" key to a message box.
            labelStep1.Text = LocRM.GetString("Login", culture);
            buttonLogin.Text = LocRM.GetString("Login", culture);
            label3.Text = LocRM.GetString("LoginInstruction", culture);
            label1.Text = LocRM.GetString("UserIdCapital", culture);
            label2.Text = LocRM.GetString("PasswordCapital", culture);

            if (WActiveDirectory) buttonLogin.Text = "Proceed";

            this.Text = LocRM.GetString("Login", culture);
        }
        // LOGIN

        private void buttonLogin_Click(object sender, EventArgs e)
        {

            if (WActiveDirectory == false)
            {
                // Check data not to be empty 

                if (comboBox2.Text == "")
                {
                    MessageBox.Show("Please enter your user Id");
                    return;
                }

                if (txtBoxPassword.Text == "")
                {
                    MessageBox.Show("Please enter your password");
                    return;
                }

                WSignedId = comboBox2.Text;

                WPassword = txtBoxPassword.Text;

                // =============================================
                Us.ReadUsersRecord(WSignedId); // Read USER record for the signed user

                if (Us.ErrorFound == true)
                {
                    MessageBox.Show("System Problem. Most likely SQL Not loaded yet"+ Environment.NewLine
                                       + "Wait for few seconds and retry to login"
                                        );
                    return;
                }

                // ===========================================
                if (Us.RecordFound == false)
                {
                    MSG = LocRM.GetString("Form40MSG002", culture);
                    MessageBox.Show(comboBox2.Text, MSG);
                    //   MessageBox.Show(" User Not Found ");
                    return;
                }
                else if (WPassword != Us.PassWord)
                {
                    MSG = LocRM.GetString("Form40MSG003", culture);
                    MessageBox.Show("For User: " + comboBox2.Text, MSG);
                    // MessageBox.Show(" Wrong User or Password ");
                    return;
                }

                Gp.ReadParametersSpecificId(Us.Operator, "451", "2", "", "");
                int Temp = ((int)Gp.Amount);

                if (DateTime.Now > Us.DtToBeChanged)
                {
                    // Your Password has expired
                    MessageBox.Show("Your Password has expired");
                    return;
                }

                if (DateTime.Now > Us.DtToBeChanged.AddDays(-Temp))
                {
                    // Your Password will expire in so many days
                    int TempRem = Convert.ToInt32((Us.DtToBeChanged - DateTime.Now).TotalDays);
                    MessageBox.Show("Days reamaining to change your password : " + TempRem.ToString());
                }

                if (Us.ForceChangePassword == true)
                {
                    // Your Password must change 
                    MessageBox.Show("Please Change password");
                    return;
                }

                Us.DtChanged = DateTime.Now;
                Us.DtToBeChanged = DateTime.Now.AddDays(Temp);
                Us.ForceChangePassword = true;

                WOperator = Us.Operator;
                WSecLevel = Us.SecLevel;

                if (WSecLevel == 7 & Us.PassWord == "123")
                {
                    MessageBox.Show("Change your password please");
                    return;
                }

                if (Us.PassWord == "99RESET9") // Password was reset  
                {
                    MessageBox.Show("Change your password please");
                    return;
                }
            }
         

            Ba.ReadBank(WOperator);

            /*
            if (DateTime.Today > Ba.LastMatchingDtTm.Date)
            {
                // Call the method in class for matching 
                HostMatchingClassGeneralLedger Hm = new HostMatchingClassGeneralLedger();
                bool HostMatched = false; // find the unmatched 
                Hm.MakeMatching(WBankId, HostMatched);
                Ba.ReadBank(WBankId); 
                Ba.LastMatchingDtTm = DateTime.Now; // Assign new value 
                Ba.UpdateBank(WBankId); 
            }
             */

            Us.ReadSignedActivity(WSignedId); // Read to check if user is already signed in 
            if (Us.RecordFound == true & Us.SignInStatus == true)
            {
                if (MessageBox.Show("Our records show that you are already loged in the system. Do you want to force new login?", "Message",
                                     MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                                     == DialogResult.Yes)
                {
                    // Leave process to continue 
                }
                else
                {
                    return;
                }
            }

            Us.UserId = WSignedId;

            Us.Culture = comboBox1.Text;

            Us.SignInStatus = true;

            Us.DtTmIn = DateTime.Now;
            Us.DtTmOut = DateTime.Now;
            Us.Replenishment = false;
            Us.Reconciliation = false;
            Us.OtherActivity = true;

            Us.InsertSignedActivity(WSignedId);

            Us.ReadSignedActivity(WSignedId); // Read to get key of record 

            WSignRecordNo = Us.SignRecordNo;

            if (WSignedId == "1005" || WSignedId == "487116")
            {
                // READ DETAILS OF ACCESS RIGHTS AND GO TO FORM1 
                NFormMainScreen = new FormMainScreen(WSignedId, WSignRecordNo, Us.SecLevel, WOperator);
                NFormMainScreen.LoggingOut += NFormMainScreen_LoggingOut;
                NFormMainScreen.Show();
                this.Hide();
            }

            if (WSignedId == "500")
            {
                // READ DETAILS OF ACCESS RIGHTS AND GO TO FORM1 
                NFormMainScreenCIT = new FormMainScreenCIT(WSignedId, WSignRecordNo, Us.SecLevel, WOperator);
                //NFormMainScreenCIT.LoggingOut
                NFormMainScreenCIT.LoggingOut += NFormMainScreenCIT_LoggingOut;
                NFormMainScreenCIT.Show();
                this.Hide();
            }       

        }  
   

        void NFormMainScreen_LoggingOut(object sender, EventArgs e)
        {
            NFormMainScreen.Dispose();

            Us.ReadSignedActivityByKey(WSignRecordNo);

            Us.SignInStatus = false; 

            Us.DtTmOut = DateTime.Now;

            Us.UpdateSignedInTableDateTmOut(WSignRecordNo);

         //   SetValues();
            this.Dispose();
        }

        void NFormMainScreenCIT_LoggingOut(object sender, EventArgs e)
        {
            NFormMainScreenCIT.Dispose();

            Us.ReadSignedActivityByKey(WSignRecordNo);

            Us.SignInStatus = false;

            Us.DtTmOut = DateTime.Now;

            Us.UpdateSignedInTableDateTmOut(WSignRecordNo);

            //   SetValues();
            this.Dispose();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem.ToString().Equals("English"))
            {
                culture = CultureInfo.CreateSpecificCulture("el-GR");
            }
            if (comboBox1.SelectedItem.ToString().Equals("Français"))
            {
                culture = CultureInfo.CreateSpecificCulture("fr-FR");
            }

            SetCulture();
        }
        // Change password 

        private void buttonChange_Click_1(object sender, EventArgs e)
        {
            // Check data not to be empty 

            if (comboBox2.Text == "")
            {
                MessageBox.Show("Please enter your user Id");
                return;
            }

            if (txtBoxPassword.Text == "")
            {
                MessageBox.Show("Please enter your password");
                return;
            }

            WSignedId = comboBox2.Text;

            WPassword = txtBoxPassword.Text;

            // =============================================
            Us.ReadUsersRecord(WSignedId); // Read USER record for the signed user

            WOperator = Us.Operator;

            // ===========================================
            if (Us.RecordFound == false)
            {
                MSG = LocRM.GetString("Form40MSG002", culture);
                MessageBox.Show(comboBox2.Text, MSG);
                //   MessageBox.Show(" User Not Found ");
                return;
            }
            else if (WPassword != Us.PassWord)
            {
                MSG = LocRM.GetString("Form40MSG003", culture);
                MessageBox.Show("For User: " + comboBox2.Text, MSG);
                // MessageBox.Show(" Wrong User or Password ");
                return;
            }
            // Inititilize password 
            txtBoxPassword.Text = "";
            NForm41 = new Form41(WSignedId, WOperator);
            NForm41.Show();
        }

  

    }
}

