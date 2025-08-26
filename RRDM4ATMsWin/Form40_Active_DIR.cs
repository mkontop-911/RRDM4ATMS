
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Text;
using System.DirectoryServices.AccountManagement;

//multilingual
using System.Resources;
using System.Globalization;
using RRDM4ATMs;
using System.Configuration;

namespace RRDM4ATMsWin
{
    public partial class Form40_Active_DIR : Form
    {
        //multilingual
        CultureInfo culture;

        bool WActiveDirectory;

        string ActiveDirGroup; 

        string WOperator;
        string WBankShortName;

        string Controllers_Form;

        string Reconciliator_Form;

        bool IsBranch_001;

        string WSecLevel;

        string WSignedId;
        int WSignRecordNo;

        bool AD_ShowAll = false;
        //bool AD_Admin = false;

        bool UserFoundInRRDM;

        string Progress;

        string W_Application;

        bool IsITMX;
        bool IsBank;
        bool IsCentralBank;

        bool SuccesfulSigned = false;

        //string DecryptedPassword;

        string MSG;

        int NumberOfAttempts;

        //FormMainScreen NFormMainScreen;

        Form1ATMs NForm1ATMs;

        Form_ATMS_Controller NForm_ATMS_Controller;
        Form_ATMS_ADMIN NForm_ATMS_ADMIN;

        Form1ATMs_Reconciliator NForm1ATMs_Reconciliator;

        Form1ATMs_ADMIN_Users_Mgmt NForm1ATMs_ADMIN_Users_Mgmt;

        Form1ATMs_Reconciliator_BDC_TYPE_2 NForm1ATMs_Reconciliator_BDC_TYPE_2;

        FormMainScreenCIT NFormMainScreenCIT;

        FormMainScreenSwitch NFormMainScreenSwitch;
        Form1ITMXCBT NForm1ITMXCBT;
        Form1ITMXBANKS NForm1ITMXBANKS;

        Form41 NForm41;

        RRDMBanks Ba = new RRDMBanks();

        RRDMUsersRecords Us = new RRDMUsersRecords(); // Make class available 

        RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();

        RRDMUsers_Applications_Roles Usr = new RRDMUsers_Applications_Roles();

        RRDMEncryptPasswordOrField En = new RRDMEncryptPasswordOrField();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMActiveDirectory Ad = new RRDMActiveDirectory();

        RRDMImages Ri = new RRDMImages();

        ResourceManager LocRM = new ResourceManager("RRDM4ATMsWin.appRes", typeof(Form40).Assembly);

        public Form40_Active_DIR()
        {

            //InitializeComponent();
            bool TestClient = true;
            try
            {
                string sTestClient = ConfigurationManager.AppSettings.Get("TEST_STARTUP");
                if (sTestClient == "true")
                    TestClient = true;
                else
                {
                    TestClient = false;
                }

            }
            catch (Exception ex)
            {
                TestClient = false;
                CatchDetails(ex);
                //string MessageForSQL = ex.Message;
                //MessageBox.Show(MessageForSQL);
            }
            //      < add key = "DB_Location_Local" value = "true" />  
            //      < add key = "DB_Location_Remote" value = "false" />
            //      < add key = "DB_Location_Remote_IP" value = "\\172.17.85.25\c$\" />
            bool DB_Location_Local = false;
            bool DB_Location_Remote = false;
            string DB_Location_Calling_IP = "";
            string Version = "NotDefined";

            try
            {
                string DB_Location = ConfigurationManager.AppSettings.Get("DB_Location_Local");
                if (DB_Location == "true")
                    DB_Location_Local = true;
                else
                {
                    DB_Location_Local = false;
                }

                DB_Location = ConfigurationManager.AppSettings.Get("DB_Location_Remote");
                if (DB_Location == "true")
                {
                    DB_Location_Remote = true;
                    DB_Location_Calling_IP = ConfigurationManager.AppSettings.Get("DB_Location_Calling_IP");
                }
                else
                {
                    DB_Location_Remote = false;
                }


            }
            catch (Exception ex)
            {

                CatchDetails(ex);

            }

            if (TestClient == true) MessageBox.Show("You got into Application on client PC ..Trace 1");

            RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

            if (Ap.IsServerConnected() == true)
            {
                // Everything is good ... continue 
                if (TestClient == true) MessageBox.Show("SQL Connection is Successful ..Trace 2");
            }
            else
            {
                MessageBox.Show("SQL Data Bases Not available..Not Able to connect to SQL");
                return;  // SQL is not available 
            }

            //TEST
            if (TestClient == true) MessageBox.Show("You have passed the SQL checking part ..Trace 3");

            if (TestClient == true) MessageBox.Show("You are about to initialised Component ..Trace 4");
            //
            InitializeComponent();
            //
            if (TestClient == true) MessageBox.Show("Component has beed initialised ..Trace 5");

            txtBoxPassword.PasswordChar = '*';

            pictureBox1.BackgroundImage = appResImg.logo2;

            string DName = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;

            string PC_NotNet_DomainName = System.Environment.UserDomainName;
            // TEST
            if (TestClient == true) MessageBox.Show("Domain Name Read ..Trace 6 with Domain Name:.." + PC_NotNet_DomainName);

            // =============================================
            //
            // Find Operator
            Ba.ReadBankToGetName(99);

            if (TestClient == true) MessageBox.Show("Bank Name Read ..Trace 7");

            if (Ba.ErrorFound == true)
            {

            }

            WOperator = Ba.Operator;
            WBankShortName = Ba.ShortName;
            string WBankName = Ba.BankName;

            ActiveDirGroup = Ba.AdGroup; 

            //Controllers_Form = "";

            Reconciliator_Form = "BDC";

           
          
            // SOFTWARE Version
            Version = ConfigurationManager.AppSettings.Get("Version");
            if (Version == null)
            {
                textBoxVersion.Text = "NotDefined";
            }
            else
            {
                textBoxVersion.Text = Version;
            }

            //
            // Check if Active Directory is applied 
            //

            string ParId = "264";
            string OccurId = "1";
            //TEST

            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            if (Gp.ErrorFound == true)
            {
                MessageBox.Show(Gp.ErrorOutput);
                return;
            }

           


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

        bool checkActive;
        bool IsMakerORChecker = false;
        private void buttonLogin_Click(object sender, EventArgs e)
        {
            SuccesfulSigned = false;

            MessageBox.Show("Dina Please enter your user Id and password as you use in production");

            // Check data not to be empty 

            if (textBoxSignId.Text == "")
                {
                    MessageBox.Show("Please enter your user Id");
                    return;
                }

                if (txtBoxPassword.Text == "")
                {
                    MessageBox.Show("Please enter your password");
                    return;
                }

                checkActive = true;

                WSignedId = textBoxSignId.Text;

                string InputPassword = txtBoxPassword.Text;

                bool isValid = false;
                if (checkActive == true)
                {
                 
                    try
                    {
                    MessageBox.Show("Dina now I will check if you are in Active Directory");
                    string AD_Domain_Name = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
                    MessageBox.Show("Dina the Active Directory Domain is :.." + AD_Domain_Name);
                    // create a "principal context" - e.g. your domain (could be machine, too)
                    using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, AD_Domain_Name))
                        // using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, "YOURDOMAIN"))
                        {
                            // validate the credentials
                            isValid = pc.ValidateCredentials(WSignedId, InputPassword);
                        //isValid = pc.ValidateCredentials("myuser", "mypassword");
                        MessageBox.Show("Dina you you are a valid user of the Active Directory.");
                    }

                    }
                    catch (Exception ex)
                    {
                    MessageBox.Show("Dina you had a cancel during Active Directory checking");
                    CatchDetails(ex);
                    return;
                }
                    // TESTING ACTIVE DIRECTORY 

                }

                if (isValid == true)
                {
                // Check user if it is in Group
                MessageBox.Show("Dina now we will check if you are in the group."+ ActiveDirGroup);
                bool DinaIsMember = IsMemberOfGroup(ActiveDirGroup, WSignedId);
                if (DinaIsMember==true)
                {
                    MessageBox.Show("Dina you are a member of the group..." + ActiveDirGroup);
                }
                else
                {
                    MessageBox.Show("Dina you are not a member of the group..." + ActiveDirGroup);
                }
                }
                else
            {
                return;
            }

            

        }

        //This is a method I use in a WCF web service I created
        //userName is the sAMAccount name of the user
        //groupName is the AD group 
        public bool IsMemberOfGroup(string groupName, string userName)
        {
            try
            {
                PrincipalContext context = new PrincipalContext(ContextType.Domain);

                UserPrincipal user = UserPrincipal.FindByIdentity(context, userName);

                GroupPrincipal group = GroupPrincipal.FindByIdentity(context, groupName);

                if (group == null)
                    return false;

                if (user != null)
                    return group.Members.Contains(user);
            }
            catch (System.Exception ex)
            {
                //Log exception
            }

            return false;
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


        //Show Characters 
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

            if (checkBoxShowPassword.Checked == true)
            {
                if (txtBoxPassword.Text == "")
                {
                    MessageBox.Show("Please enter your password");
                    return;
                }

                WSignedId = textBoxAD_User.Text;

                // =============================================
                Us.ReadUsersRecord(WSignedId); // Read USER record for the signed user

                if (Us.ErrorFound == true)
                {
                    MessageBox.Show("System Problem. Most likely SQL Not loaded yet" + Environment.NewLine
                                       + "Wait for few seconds and retry to login"
                                        );
                    return;
                }

                // ===========================================
                if (Us.RecordFound == false)
                {
                    MSG = LocRM.GetString("Form40MSG002", culture);
                    MessageBox.Show(textBoxAD_User.Text, MSG);
                    //   MessageBox.Show(" User Not Found ");
                    return;
                }
                else
                {
                    // User Found 
                    bool PasswordMatched = En.CheckPassword(txtBoxPassword.Text, Us.PassWord);

                    if (PasswordMatched == true)
                    {

                        MessageBox.Show("Correct Password. Cannot be shown.");

                        return;
                    }
                }

                //Enable to see characters of entered password
                txtBoxPassword.PasswordChar = checkBoxShowPassword.Checked ? '\0' : '*';

            }
            else
            {
                // go back to hide characters of entered password 
                txtBoxPassword.PasswordChar = '*';
            }
        }
        // On password change 
        private void txtBoxPassword_TextChanged(object sender, EventArgs e)
        {
            checkBoxShowPassword.Checked = false;
        }

        // Make Password Validations


        private static void CatchDetails(Exception ex)
        {
            //  ProgressText = ProgressText + "We_Had_Cancel";
            RRDMLog4Net Log = new RRDMLog4Net();

            StringBuilder WParameters = new StringBuilder();

            WParameters.Append("User : ");
            WParameters.Append("NotAssignYet");
            WParameters.Append(Environment.NewLine);

            WParameters.Append("ATMNo : ");
            WParameters.Append("NotDefinedYet");
            WParameters.Append(Environment.NewLine);

            string Logger = "RRDM4Atms";
            string Parameters = WParameters.ToString();

            Log.CreateAndInsertRRDMLog4NetMessage(ex, Logger, Parameters);

            if (Environment.UserInteractive)
            {
                System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
                    + " . Application will be aborted! Call controller to take care. ");
            }

            ////  Environment.Exit(0);
        }



    }
}

