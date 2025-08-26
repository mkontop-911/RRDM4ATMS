
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
    public partial class Form40 : Form
    {
        //multilingual
        CultureInfo culture;

        bool WActiveDirectory;

        bool IsAdmin;
        bool IsController;

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

        public Form40()
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

            Controllers_Form = "";

           // Reconciliator_Form = "BDC";

            if (Environment.MachineName == "DESKTOP-77PU6PG" || Environment.MachineName == "RRDM-PANICOS" || Environment.MachineName == "RRDM_ABE")
            {
                if (Environment.MachineName == "DESKTOP-77PU6PG" || Environment.MachineName == "RRDM-PANICOS")
                {
                    Reconciliator_Form = "BDC";
                    //   MessageBox.Show("You Are Signing to..." + WBankName);
                    MessageBox.Show("You Are Signing to the Primary Bank..");

                    //panelUserMgmt.Show(); 
                }
                else
                {

                }


                //if (Environment.MachineName == "RRDM_ABE")
                //{
                //    Reconciliator_Form = "ABE";
                //    MessageBox.Show("You Are Signing to..." + WBankShortName);
                //}
            }

            if (WBankShortName == "BDC")
            {
                Reconciliator_Form = "BDC";
                radioButton_T24.Show();
                Controllers_Form = "BDC";
                //  MessageBox.Show("You Are Signing to BOC Bank..");
            }
            else
            {
                 radioButton_T24.Checked = false;
                 panelMobileApl.Hide();
            }
            //else
            //{

            //}

            if (WBankShortName == "ABE")
            {
                Reconciliator_Form = "ABE";
                MessageBox.Show("You Are Signing to..." + WBankShortName);
            }

            // ALLOW AUDI TO GO TO FULL RECONCILIATION
            if (WBankShortName == "AUD" || WBankShortName == "BT")
            {
                Reconciliator_Form = "EMR";
                Controllers_Form = "EMR";
                radioButton_T24.Checked = false;
                radioButton_T24.Hide();
            }

           
            if (WBankShortName == "EGA")
            {
                Reconciliator_Form = "EGA";
                Controllers_Form = "EGA";
                MessageBox.Show("You Are Signing to EGATE");
                radioButton_T24.Checked = false;
                radioButton_T24.Hide();
            }
            radioButtonController.Checked = false;

            radioButtonReconciliator.Checked = false;
            // SOFTWARE Version
            Version = ConfigurationManager.AppSettings.Get("Version");
            if (Version == null)
            {
                textBoxVersion.Text = "NotDefined";
            }
            else
            {
                textBoxVersion.Text = Version+"DEV";
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

            //Gp.OccuranceNm = "YES";
            // AdDomainName
            
            //PANICOS


            if (Gp.OccuranceNm == "YES") // Active directory needed
            {
                if (TestClient == true) MessageBox.Show("Active Directory Is YES About to check it ..Trace 8");
                Ad.CheckActiveDirectory();

                if (Ad.ErrorFound == true)
                {
                    if (TestClient == true) MessageBox.Show("Error In Active Directory ..Trace 9");
                    MessageBox.Show(Ad.ErrorOutput);

                    panelAplications.Hide();
                    panelRole.Hide();
                    buttonLogin.Hide();
                    buttonChange.Hide();

                    label1_User_ID.Hide();
                    label_password.Hide();
                    label_Prompt_to_Login.Hide();

                    comboBoxLanguage.Hide();

                    checkBoxShowPassword.Hide();

                    comboBoxUser.Hide();
                    txtBoxPassword.Hide();

                    panel_AD.Show();

                    label4.Text = "Maybe User not in Active Directory";
                    label5.Text = " not in RRDM.";
                    label6.Text = "Ask Admin to open User ";
                    label7.Text = "with the exact User Id";

                    return;
                }


                if (Ad.DomainFound & Ad.ValidDomain & Ad.UserInGroup & Ad.UserFoundInRRDM)
                {
                    // Success in Active Directory
                    if (TestClient == true) MessageBox.Show("Success in Active Directory..Trace 9");
                    WActiveDirectory = true;
                }
                else
                {
                    panelAplications.Hide();
                    panelRole.Hide();
                    buttonLogin.Hide();
                    buttonChange.Hide();

                    label1_User_ID.Hide();
                    label_password.Hide();
                    label_Prompt_to_Login.Hide();

                    comboBoxLanguage.Hide();

                    checkBoxShowPassword.Hide();

                    comboBoxUser.Hide();
                    txtBoxPassword.Hide();

                    panel_AD.Show();

                    if (Ad.ValidDomain == false)
                    {
                        label4.Text = "You try to sign on not in valid";
                        label5.Text = "Active Directory Domain";
                        label6.Text = "";
                        label7.Text = "";
                        return;
                    }

                    if (Ad.UserInGroup == false)
                    {
                        textBoxAD_User.Show();
                        textBoxAD_User.Text = Ad.AdUserName;

                        label4.Text = "User in Active Directory";
                        label5.Text = "But Not in Active RRDM group";
                        label6.Text = "You cannot sign to RRDM";
                        label7.Text = "unless IT includes you in group";
                        return;
                    }

                    if (Ad.UserFoundInRRDM == false)
                    {
                        textBoxAD_User.Show();
                        textBoxAD_User.Text = Ad.AdUserName;

                        label4.Text = "User with ID:" + Ad.AdUserId;
                        label5.Text = " not in RRDM.";
                        label6.Text = "Ask Admin to open User ";
                        label7.Text = "with the exact User Id";
                        return;
                    }

                    return;
                }


                if (WActiveDirectory == true)
                {
                    label1_User_ID.Hide();
                    label_password.Hide();
                    label_Prompt_to_Login.Hide();

                    comboBoxUser.Hide();
                    txtBoxPassword.Hide();

                    panel_AD.Show();

                    buttonChange.Hide();

                    WOperator = Ad.Operator;

                    WSignedId = Ad.AdUserId.Trim();

                    // ===========================================
                    // Assign User fields 
                    //*********************************************
                    Us.ReadUsersRecord(WSignedId);

                    if (Ad.NameFromActive) Us.UserName = Ad.AdUserName;

                    Us.UpdateUser(WSignedId); // Update User with details from active directory 

                    buttonLogin.Text = "Proceed";

                    panelRole.Hide();

                    checkBoxShowPassword.Hide();

                    panelAplications.Hide();

                    W_Application = "ATMS/CARDS";
                    if (WBankShortName == "BDC")
                    {
                        string WParamId = "805"; // Show ALL
                        string WOccuranceNm = WSignedId;
                        Gp.ReadParametersSpecificNm(WOperator, WParamId, WOccuranceNm);
                        // MAKE SECOND CHECK
                        //RRDMUsers_Applications_Roles Ar = new RRDMUsers_Applications_Roles();

                        //// GET THE ACCESS RIGHTS and Update the history 

                        //Ar.ReadUsersVsApplicationsVsRolesByUserAndAccessRights(WSignedId);

                        //if (Gp.RecordFound == true || Ar.W_Counter > 1)
                        //{
                        //    AD_ShowAll = true;

                        //    panelRole.Show();

                        //    radioButtonController.Checked = true;

                        //    if (Ar.W_Counter > 1)
                        //    {
                        //        radioButtonAdmin.Enabled = false;
                        //        radioButtonController.Enabled = false;
                        //        radioButtonReconciliator.Enabled = false;

                        //        if (Ar.W_Admin == true)
                        //        {
                        //            IsAdmin = true;
                        //            radioButtonAdmin.Enabled = true;
                        //        }
                        //        if (Ar.W_Controller == true)
                        //        {
                        //            IsController = true;
                        //            radioButtonController.Enabled = true;
                        //        }
                        //        if (Ar.W_ReconcOfficer == true)
                        //        {
                        //            // IsController = true;
                        //            radioButtonReconciliator.Enabled = true;
                        //        }

                        //    }

                        //}
                        //else
                        //{
                        //    AD_ShowAll = false;
                        //    radioButtonReconciliator.Checked = true;
                        //}

                        if (Gp.RecordFound == true)
                        {
                            AD_ShowAll = true;

                            panelRole.Show();

                            radioButtonController.Checked = true;
                        }
                        else
                        {
                            AD_ShowAll = false;
                            radioButtonReconciliator.Checked = true;
                        }

                    }

                    textBoxAD_User.Show();
                    textBoxAD_User.Text = Us.UserName;

                    label4.Text = "Welcome!";
                    label5.Text = "You are now signed on to RRDM.";
                    label6.Text = "Press Proceed ";
                    label7.Text = "";

                }
                else
                {
                    WActiveDirectory = false;

                    //else
                    //{
                    //    AD_ShowAll = false;
                    //    radioButtonReconciliator.Checked = true;

                    W_Application = "ATMS/CARDS";

                    if (WBankShortName == "BDC")
                    {

                        string WParamId = "805"; // Show ALL
                        string WOccuranceNm = WSignedId;
                        Gp.ReadParametersSpecificNm(WOperator, WParamId, WOccuranceNm);

                        if (Gp.RecordFound == true)
                        {
                            AD_ShowAll = true;

                            panelRole.Show();

                            radioButtonController.Checked = true;
                        }
                        else
                        {
                            AD_ShowAll = false;
                            radioButtonReconciliator.Checked = true;
                        }

                    }
                    //}

                    panel_AD.Hide();

                    

                }
            }
            else
            {
                WActiveDirectory = false;
                panel_AD.Hide();

                panelRole.Hide();

            }

            //TEST
            if (WBankShortName == "EGA")
            {
                panelAplications.Hide();
                panelRole.Hide();
                panelMobileApl.Hide();
                radioButton_T24.Hide(); 
              
                comboBoxLanguage.Hide();

                //radioButtonATMS_CARDS.Checked = true;
                radioButton_e_MOBILE.Checked = true; 

            }

            if (WBankShortName == "AUD")
            {
                comboBoxUser.Items.Add("ADMIN-" + "FAB");
                comboBoxUser.Items.Add("CONTROLLER-" + "FAB");
            }
            else
            {
                comboBoxUser.Items.Add("ADMIN-" + WBankShortName);
                comboBoxUser.Items.Add("CONTROLLER-" + WBankShortName);
            }

            comboBoxUser.Items.Add("Reconc_Total");

            //  comboBox2.Items.Add("ADMIN-NBG-E-FIN");
            comboBoxUser.Items.Add("ahm.osman");
            comboBoxUser.Items.Add("PILOT_001");
            comboBoxUser.Items.Add("PILOT_002");
            comboBoxUser.Items.Add("PILOT_004");
            comboBoxUser.Items.Add("PILOT_005");
            comboBoxUser.Items.Add("UAT_014");
            comboBoxUser.Items.Add("UAT_013");
            comboBoxUser.Items.Add("UAT_011");
            //comboBox2.Items.Add("E_FIN_Central");
            comboBoxUser.Items.Add("PILOT_7_NOSTRO");
            comboBoxUser.Items.Add("PILOT_8 NV Auther");
            //comboBox2.Items.Add("POS_USER1");

            //comboBox2.Items.Add("999"); // Gas Master 

            SetValues();


        }
        bool MoreThanOneRole;
        private void SetValues()
        {
            radioButtonATMS_CARDS.Checked = true;
            if (WBankShortName == "EGA")
            {
                radioButton_e_MOBILE.Checked = true;
            }
            else
            {
                radioButton_e_MOBILE.Checked = false;
            }
                
            //radioButtonPOS_SETTLEMENT.Checked = false;
            //radioButtonE_FINANCE_RECONCILIATION.Checked = false;
            //radioButtonFAWRY_RECONCILIATION.Checked = false;

            if (comboBoxUser.Text == "1005" || comboBoxUser.Text == "1006" || comboBoxUser.Text == "487116"
                || comboBoxUser.Text == "NBG_001" || comboBoxUser.Text == "NBG_002"
                || comboBoxUser.Text == "NBG_004" || comboBoxUser.Text == "NBG_005"
                || comboBoxUser.Text == "PILOT_001" || comboBoxUser.Text == "PILOT_002"
                || comboBoxUser.Text == "PILOT_004" || comboBoxUser.Text == "PILOT_005"

                || comboBoxUser.Text == "UAT_014"
                || comboBoxUser.Text == "ADMIN-ATMS"
                || comboBoxUser.Text == "ADMIN-NBG"
                || comboBoxUser.Text == "ADMIN-" + WBankShortName
                || comboBoxUser.Text == "CONTROLLER-" + WBankShortName
                || comboBoxUser.Text == "ahm.osman"
                || comboBoxUser.Text == "ADMIN-" + "FAB"
                || comboBoxUser.Text == "CONTROLLER-" + "FAB"
                || comboBoxUser.Text == "Reconc_Total"
                || comboBoxUser.Text == "1007_BDO"
                || comboBoxUser.Text == "Admin-Level10" || comboBoxUser.Text == "Admin-Level11"
                               || comboBoxUser.Text == "NOSTROUser" || comboBoxUser.Text == "NOSTROAuther"
                               || comboBoxUser.Text == "VISAUSER1"
                               || comboBoxUser.Text == "UAT_013"
                               || comboBoxUser.Text == "UAT_011"
                               || comboBoxUser.Text == "E_FIN_Central"
                                || comboBoxUser.Text == "PILOT_7_NOSTRO"
                               || comboBoxUser.Text == "PILOT_8 NV Auther"
                               || comboBoxUser.Text == "POS_USER1"
                               )
            {
                txtBoxPassword.Text = "12345678";
            }

            if (comboBoxUser.Text == "999")
            {
                //txtBoxPassword.Text = "hLGxlR51";
                txtBoxPassword.Text = "12345678";
            }

            if (comboBoxUser.Text == "ITMXUser1")
            {
                txtBoxPassword.Text = "12345678";
            }

            if (comboBoxUser.Text == "BBLUser1")
            {
                txtBoxPassword.Text = "12345678";
            }

            if (comboBoxUser.Text == "BBLUser2")
            {
                txtBoxPassword.Text = "12345678";
            }

            if (comboBoxUser.Text == "CBTUser1")
            {
                txtBoxPassword.Text = "12345678";
            }

            if (comboBoxUser.Text == "CBTUser2")
            {
                txtBoxPassword.Text = "12345678";
            }

            if (comboBoxUser.Text == "1032-Level2")
            {
                txtBoxPassword.Text = "12345678";
            }
            if (comboBoxUser.Text == "1033-Level3")
            {
                txtBoxPassword.Text = "12345678";
            }
            if (comboBoxUser.Text == "1034-Level4")
            {
                txtBoxPassword.Text = "12345678";
            }
            if (comboBoxUser.Text == "1035-Level5")
            {
                txtBoxPassword.Text = "12345678";
            }
            // ***********************
            //RRDMUsers_Applications_Roles Ar = new RRDMUsers_Applications_Roles();

            //// GET THE ACCESS RIGHTS and Update the history 

            //Ar.ReadUsersVsApplicationsVsRolesByUserAndAccessRights(comboBoxUser.Text);

            //if (Ar.W_Counter > 1)
            //{
            //    if (Ar.W_Admin == true || Ar.W_Controller == true)
            //    {
            //        MoreThanOneRole = true;

            //        panelRole.Show();

            //        radioButtonController.Checked = true;

            //        radioButtonAdmin.Enabled = false;
            //        radioButtonController.Enabled = false;
            //        radioButtonReconciliator.Enabled = false;

            //        if (Ar.W_Admin == true)
            //        {
            //            IsAdmin = true;
            //            radioButtonAdmin.Enabled = true;
            //        }
            //        if (Ar.W_Controller == true)
            //        {
            //            IsController = true;
            //            radioButtonController.Enabled = true;
            //        }
            //        if (Ar.W_ReconcOfficer == true)
            //        {
            //            // IsController = true;
            //            radioButtonReconciliator.Enabled = true;
            //        }
            //    }

            //}
            //else
            //{
            //    MoreThanOneRole = false;

            //    panelRole.Hide();

            //    radioButtonAdmin.Enabled = false;
            //    radioButtonController.Enabled = false;
            //    radioButtonReconciliator.Enabled = false;
            //}
            W_Application = "ATMS/CARDS";

            if (WBankShortName == "BDC")
            {
                string WParamId = "805"; // Show ALL
                string WOccuranceNm = comboBoxUser.Text;
                Gp.ReadParametersSpecificNm(WOperator, WParamId, WOccuranceNm);

                if (Gp.RecordFound == true)
                {
                    AD_ShowAll = true;

                    panelRole.Show();

                    radioButtonController.Checked = true;

                    MoreThanOneRole = true;
                }
                else
                {
                    AD_ShowAll = false;
                    radioButtonReconciliator.Checked = true;
                }
            }
            
            comboBoxLanguage.Text = "English";

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
            label_Prompt_to_Login.Text = LocRM.GetString("LoginInstruction", culture);
            label1_User_ID.Text = LocRM.GetString("UserIdCapital", culture);
            label_password.Text = LocRM.GetString("PasswordCapital", culture);

            if (WActiveDirectory) buttonLogin.Text = "Proceed";

            this.Text = LocRM.GetString("Login", culture);
        }
        // LOGIN

        bool checkActive;
        bool IsMakerORChecker = false;
        private void buttonLogin_Click(object sender, EventArgs e)
        {
            SuccesfulSigned = false;
           if (WBankShortName == "EGA")
            {
                radioButton_e_MOBILE.Checked = true;
            }
            if (radioButtonATMS_CARDS.Checked == false
                 & radioButton_e_MOBILE.Checked == false
                 //  & radioButtonNOSTRO.Checked == false
                 //& radioButtonPOS_SETTLEMENT.Checked == false
                 //& radioButtonE_FINANCE_RECONCILIATION.Checked == false
                 //& radioButtonFAWRY_RECONCILIATION.Checked == false
                 )
            {
                MessageBox.Show("Please select Application ");
                return;
            }

            if (radioButton_e_MOBILE.Checked == true
                & radioButtonETI.Checked == false & radioButtonQAH.Checked == false
                & radioButtonIPL.Checked == false & WBankShortName != "EGA"
                 )
            {
                MessageBox.Show("Please select e-wallet type");
                return;
            }

            string ParId_2 = "102";
            string OccurId_2 = "2";
            //TEST


            Gp.ReadParametersSpecificId(WOperator, ParId_2, OccurId_2, "", "");

            if (Gp.RecordFound == true & Gp.OccuranceNm == "YES") // ...
            {
                checkActive = true;
            }
            else
            {
                checkActive = false;


            }

            //WActiveDirectory = true;

            //WSignedId = comboBox2.Text;

            if (WActiveDirectory == false)
            {
                // THIS IS NOT ACTIVE DIRECTORY
                //
                // Check data not to be empty 

                if (comboBoxUser.Text == "")
                {
                    MessageBox.Show("Please enter your user Id");
                    return;
                }

                if (txtBoxPassword.Text == "")
                {
                    MessageBox.Show("Please enter your password");
                    return;
                }

                WSignedId = comboBoxUser.Text;

                string InputPassword = txtBoxPassword.Text;

                bool Panicos = false;
                if (Panicos == true)
                {
                    bool isValid = false;
                    try
                    {
                        string AD_Domain_Name = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;

                        // create a "principal context" - e.g. your domain (could be machine, too)
                        using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, AD_Domain_Name))
                        // using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, "YOURDOMAIN"))
                        {
                            // validate the credentials
                            isValid = pc.ValidateCredentials(WSignedId, InputPassword);
                            //isValid = pc.ValidateCredentials("myuser", "mypassword");
                        }

                    }
                    catch (Exception ex)
                    {
                        CatchDetails(ex);
                    }
                    // TESTING ACTIVE DIRECTORY 

                }


                // All Validations must be made before allowed to change password
                //*************************************************************
                UserPasswordValidation(WSignedId, InputPassword, "Login");
                //*************************************************************

                if (SuccesfulSigned == true)
                {
                    if (Us.ForceChangePassword == true)
                    {
                        return;
                    }
                    // OK

                }
                else
                {
                    return;
                }
                // =============================================

                // We have here the id 
                // READ USER TAIL 
                // Check For Maker or Checker 
                //if (Environment.MachineName == "RRDM-PANICOS")
                //{
                IsMakerORChecker = false;
                // Read the tail
                if (radioButtonATMS_CARDS.Checked == true)
                {
                    Us.ReadUsersRecord_FULL(WSignedId);
                    if (Us.User_Is_Maker == true || Us.User_Is_Checker == true)
                    {
                        IsMakerORChecker = true;
                    }
                }
                else
                {
                    Us.ReadUsersRecord(WSignedId);
                }

                //}

                if (IsMakerORChecker == true)
                {
                    // Fill What it is needed
                    W_Application = "ATMS/CARDS";
                    WSecLevel = "10"; // Leave here We neeed it when we create sign one record  
                }
                else
                {
                    // Follow the not Active directory direction
                    Us.ReadUsersRecord(WSignedId);

                    //radioButton_e_MOBILE.Checked = true;

                    WOperator = Us.Operator;

                    if (radioButtonATMS_CARDS.Checked == true) W_Application = "ATMS/CARDS";
                    if (radioButton_e_MOBILE.Checked == true) W_Application = "e_MOBILE";
                    if (radioButtonNOSTRO.Checked == true) W_Application = "NOSTRO";

                    Usr.ReadUsersVsApplicationsVsRolesByApplication(Us.UserId, W_Application);
                    if (Usr.RecordFound == true)
                    {
                        if (MoreThanOneRole == true & WBankShortName == "BDC")
                        {
                            // Check if Dina is signed in as  a controller and upgrade her access rights. 
                            if (radioButtonController.Checked == true)
                            {
                                // Make-Upgrate  Security level to 08 as Controller 
                                WSecLevel = "08";

                            }
                            // Check if Dina is signed in as  a Admin and upgrade her access rights. 
                            if (radioButtonAdmin.Checked == true)
                            {
                                // Make-Upgrate  Security level to 10 as Admin
                                WSecLevel = "10";
                            }

                            if (radioButtonReconciliator.Checked == true)
                            {
                                // Make-Upgrate  Security level to 10 as Admin
                                WSecLevel = "03";
                            }
                        }
                        else
                        {
                            WSecLevel = Usr.SecLevel;
                        }

                    }
                    else
                    {
                        MessageBox.Show("This user cannot access this application :.." + W_Application);
                        return;
                    }

                    if (WSecLevel == "01")
                    {
                        MessageBox.Show("This user has security level that is allowed access only from branches");
                        return;
                    }

                    if (WSecLevel == "07" & Us.PassWord == "123")
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

            }
            else
            {
                // THIS IS ACTIVE DIRECTORY 
                if (Environment.MachineName == "RRDM-PANICOS")
                {
                    // Read the tail
                    Us.ReadUsersRecord_FULL(WSignedId);

                    if (Us.Is_Approved == false)
                    {
                        MessageBox.Show("User Under Checker Process");
                        return;
                    }

                    if (Us.User_Is_Maker == true || Us.User_Is_Checker == true)
                    {
                        IsMakerORChecker = true;
                    }
                }

                if (IsMakerORChecker == true)
                {
                    // Fill What it is needed
                    W_Application = "ATMS/CARDS";
                    WSecLevel = "10"; // leave here needed when we insert the sign on record
                }
                else
                {
                    WSecLevel = "03";
                    comboBoxLanguage.Text = "English";
                    if (radioButtonATMS_CARDS.Checked == true) W_Application = "ATMS/CARDS";
                    if (radioButton_e_MOBILE.Checked == true) W_Application = "e_MOBILE";
                    if (radioButtonNOSTRO.Checked == true) W_Application = "NOSTRO";
                    Us.ReadUsersRecord(WSignedId);

                    if (Us.RecordFound == true)
                    {
                        Usr.ReadUsersVsApplicationsVsRolesByApplication(Us.UserId, W_Application);
                        if (Usr.RecordFound == true)
                        {
                            WSecLevel = Usr.SecLevel;


                        }
                        else
                        {
                            MessageBox.Show("Security level not found in RRDM");
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("This User Not found in RRDM");

                        return;
                    }

                    // Check if Dina is signed in as  a controller and upgrade her access rights. 
                    if (AD_ShowAll == true & radioButtonController.Checked == true)
                    {
                        // Make-Upgrate  Security level to 08 as Controller 
                        WSecLevel = "08";

                    }
                    // Check if Dina is signed in as  a Admin and upgrade her access rights. 
                    if (AD_ShowAll == true & radioButtonAdmin.Checked == true)
                    {
                        // Make-Upgrate  Security level to 10 as Admin
                        WSecLevel = "10";
                    }

                    if (AD_ShowAll == true & radioButtonReconciliator.Checked == true)
                    {
                        // Make-Upgrate  Security level to 10 as Admin
                        WSecLevel = "03";
                    }



                }

            }


            // Check for Is Allowed to get in 
            if (IsMakerORChecker == true)
            {
                // Continue
            }
            else
            {
                string ParId = "105";
                string OccurId = "1";

                bool IsAllowedToSignIn = true;
                Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

                if (Gp.OccuranceNm == "YES")
                {
                    IsAllowedToSignIn = true;
                }
                else
                {
                    // Occurance is NO => not allowed to signed in
                    if (WSecLevel == "08" || WSecLevel == "10" || (Usi.RecordFound == true & Usi.SignInStatus == true))
                    {
                        // 08 is the controller which is allowed at any moment
                        // The other condition is related with the ones with half signed in 
                        IsAllowedToSignIn = true;
                    }
                    else
                    {
                        IsAllowedToSignIn = false;
                    }

                }

                if (IsAllowedToSignIn == false)
                {
                    MessageBox.Show("You are not allowed to sign in at this moment" + Environment.NewLine
                        + "Loading and Matching of files takes place"
                        );
                    return;
                }

            }

            Usi.ReadSignedActivity(WSignedId); // Read to check if user is already signed in 
            if (Usi.RecordFound == true & Usi.SignInStatus == true)
            {
                if (MessageBox.Show("Our records show that you are already logged in the system. Do you want to force new login?", "Message",
                                     MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                                     == DialogResult.Yes)
                {
                    // Leave process to continue 
                    string Test = "105";
                }
                else
                {
                    return;
                }
            }

            if (Us.Branch == "001" || Us.Branch == "0001")
            {
                // Cards Department 
                IsBranch_001 = true;
            }
            else
            {
                IsBranch_001 = false;
            }

            Usi.UserId = WSignedId;

            Usi.UserName = Us.UserName;

            Usi.SecLevel = WSecLevel;

            Usi.Culture = comboBoxLanguage.Text;

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
            if (radioButtonATMS_CARDS.Checked == true)
                Usi.ATMS_Reconciliation = true;

            if (radioButtonNOSTRO.Checked == true)
                Usi.NOSTRO_Reconciliation = true;

            Usi.Replenishment = false;
            Usi.Reconciliation = false;
            Usi.OtherActivity = true;

            // ProcessNo

            if (radioButton_e_MOBILE.Checked == true)
            {
                if (Us.UserId == "CONTROLLER-EGA" || Us.UserId == "ADMIN-EGA" || WOperator == "EGATE")
                {
                    Usi.WFieldNumeric11 = 15; // Set 15 for EGATE
                }
                if (radioButtonETI.Checked == true)
                {
                    Usi.WFieldNumeric11 = 11; // Set 11 for ETISALAT

                    // Valid for UAT 
                    
                    //DateTime Today = DateTime.Now;
                    //DateTime LimitDate = new DateTime(2025, 09, 31);

                    //if (Today >= LimitDate)
                    //{
                    //    MessageBox.Show("Communicate with RRDM for new version to extend UAT period");
                    //    return;
                    //}

                }
                if (radioButtonQAH.Checked == true)
                {
                    Usi.WFieldNumeric11 = 12; // Set 2 for QAHERA
                }
                if (radioButtonIPL.Checked == true)
                {
                    Usi.WFieldNumeric11 = 13; // Set 3 for IPN
                }
            }

            if (radioButtonATMS_CARDS.Checked == true)
            {
                if (radioButton_T24.Checked == true)
                {
                    Usi.WFieldNumeric11 = 10; // This is a T24 Version 


                    // UPDATE the entry of parameter 950 to YES 
                    string ParId = "950";
                    string OccurId = "1";
                    //TEST

                    Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

                    if (Gp.RecordFound == true)
                    {
                        Gp.OccuranceNm = "YES";

                        Gp.UpdateGasParamByKey(WOperator, Gp.RefKey);
                    }
                    else
                    {
                        MessageBox.Show("950 parameter not present");
                        return;
                    }

                }
                else
                {
                    Usi.WFieldNumeric11 = 0; // This is not a T24 Version
                                             // UPDATE the entry of parameter 950 to NO

                    string ParId = "950";
                    string OccurId = "1";
                    //TEST

                    Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

                    if (Gp.RecordFound == true)
                    {
                        Gp.OccuranceNm = "NO";

                        Gp.UpdateGasParamByKey(WOperator, Gp.RefKey);
                    }
                    else
                    {
                        MessageBox.Show("950 parameter not present");
                        return;
                    }

                }
            }

            WSignRecordNo = Usi.InsertSignedActivity(WSignedId);

            if (IsMakerORChecker == true)
            {
                // GO TO USERS MANAGEMENT 
                NForm1ATMs_ADMIN_Users_Mgmt = new Form1ATMs_ADMIN_Users_Mgmt(WSignedId, WSignRecordNo, WSecLevel, WOperator);
                NForm1ATMs_ADMIN_Users_Mgmt.LoggingOut += NForm1ATMs_LoggingOut;
                NForm1ATMs_ADMIN_Users_Mgmt.Show();

                this.Hide();
                return;
            }

            if (WActiveDirectory == true)
            {
                if (AD_ShowAll == true & radioButtonController.Checked == true) // Coming from Dina or Osman
                {

                    // Make-Upgrate  Security level to 08 as Controller 
                    WSecLevel = "08";

                    if (Environment.MachineName != "RRDM-PROD")
                    {
                        MessageBox.Show("Warning: " + Environment.NewLine
                                        + "You are signing as Controller not to the production machine." + Environment.NewLine
                                         + "Why? Make sure you know what you are doing" + Environment.NewLine
                                        + "Also Ensure that you are not sign in in the production machine too" + Environment.NewLine
                                        + "You might create problems to the system"
                                       );
                    }

                    //if (checkActive == true)
                    //{

                    //    Progress = Progress + "007_Progress_Before Shown" + Environment.NewLine
                    //           + "Operator.. " + WOperator + Environment.NewLine
                    //             + "SignedId.. " + WSignedId + Environment.NewLine
                    //               + "WSecLevel .. " + WSecLevel + Environment.NewLine
                    //           ;
                    //    MessageBox.Show(Progress);
                    //}

                    NForm_ATMS_Controller = new Form_ATMS_Controller(WSignedId, WSignRecordNo, WSecLevel, WOperator);
                    // NForm_ATMS_ADMIN.LoggingOut += NForm_ATMS_ADMIN_LoggingOut;
                    NForm_ATMS_Controller.LoggingOut += NForm_ATMS_ADMIN_LoggingOut;
                    NForm_ATMS_Controller.Show();

                    this.Hide();
                    return;

                    //if (checkActive == true)
                    //{

                    //    Progress = Progress + "008_Progress_After Shown" + Environment.NewLine;
                    //    MessageBox.Show(Progress);
                    //}

                    //return; 
                    //this.Hide();
                    // return;
                }
                // 
                if (AD_ShowAll == true & radioButtonReconciliator.Checked == true)
                {
                    if (Environment.MachineName == "RRDM-PROD")
                    {
                        MessageBox.Show("Warning: " + Environment.NewLine
                                        + "You are signing as Reconciliator to the production machine." + Environment.NewLine
                                         + "Why?" + Environment.NewLine
                                        + "Ensure that you are not sign in in your computer too " + Environment.NewLine
                                        + "You might create problems to the system"
                                       );
                    }

                    if (IsBranch_001 == true)
                    {
                        // Cards Department 
                        NForm1ATMs_Reconciliator_BDC_TYPE_2 = new Form1ATMs_Reconciliator_BDC_TYPE_2(WSignedId, WSignRecordNo, WSecLevel, WOperator);
                        NForm1ATMs_Reconciliator_BDC_TYPE_2.LoggingOut += NForm1ATMs_LoggingOut;
                        NForm1ATMs_Reconciliator_BDC_TYPE_2.Show();
                        this.Hide();
                        return;

                    }
                    else
                    {
                        if (W_Application == "e_MOBILE")
                        {
                            Form1ATMs_Reconciliator_EWALLET NForm1ATMs_Reconciliator_EWALLET;

                            NForm1ATMs_Reconciliator_EWALLET = new Form1ATMs_Reconciliator_EWALLET(WSignedId, WSignRecordNo, WSecLevel, WOperator);
                            NForm1ATMs_Reconciliator_EWALLET.LoggingOut += NForm1ATMs_LoggingOut;
                            NForm1ATMs_Reconciliator_EWALLET.Show();
                            this.Hide();
                            return;
                        }
                        else
                        {
                            NForm1ATMs_Reconciliator = new Form1ATMs_Reconciliator(WSignedId, WSignRecordNo, WSecLevel, WOperator);
                            NForm1ATMs_Reconciliator.LoggingOut += NForm1ATMs_LoggingOut;
                            NForm1ATMs_Reconciliator.Show();
                            this.Hide();
                            return;
                        }

                    }


                }

                if (AD_ShowAll == true & radioButtonAdmin.Checked == true)
                {

                    // Make-Upgrate  Security level to 10 as Controller 
                    WSecLevel = "10";

                    NForm_ATMS_ADMIN = new Form_ATMS_ADMIN(WSignedId, WSignRecordNo, WSecLevel, WOperator);
                    NForm_ATMS_ADMIN.LoggingOut += NForm_ATMS_ADMIN_LoggingOut;
                    NForm_ATMS_ADMIN.Show();
                    this.Hide();
                    return;
                }

                if (AD_ShowAll == false & radioButtonReconciliator.Checked == true)
                {
                    if (IsBranch_001 == true)
                    {
                        NForm1ATMs_Reconciliator_BDC_TYPE_2 = new Form1ATMs_Reconciliator_BDC_TYPE_2(WSignedId, WSignRecordNo, WSecLevel, WOperator);
                        NForm1ATMs_Reconciliator_BDC_TYPE_2.LoggingOut += NForm1ATMs_LoggingOut;
                        NForm1ATMs_Reconciliator_BDC_TYPE_2.Show();
                        this.Hide();
                        return;
                    }
                    else
                    {
                        if (W_Application == "e_MOBILE")
                        {
                            Form1ATMs_Reconciliator_EWALLET NForm1ATMs_Reconciliator_EWALLET;

                            NForm1ATMs_Reconciliator_EWALLET = new Form1ATMs_Reconciliator_EWALLET(WSignedId, WSignRecordNo, WSecLevel, WOperator);
                            NForm1ATMs_Reconciliator_EWALLET.LoggingOut += NForm1ATMs_LoggingOut;
                            NForm1ATMs_Reconciliator_EWALLET.Show();
                            this.Hide();
                            return;
                        }
                        else
                        {
                            NForm1ATMs_Reconciliator = new Form1ATMs_Reconciliator(WSignedId, WSignRecordNo, WSecLevel, WOperator);
                            NForm1ATMs_Reconciliator.LoggingOut += NForm1ATMs_LoggingOut;
                            NForm1ATMs_Reconciliator.Show();
                            this.Hide();
                            return;
                        }
                    }

                }
            }
            else
            {
                // NOT ACTIVE DIRECTORY 
                if (WSignedId == "500")
                {
                    // READ DETAILS OF ACCESS RIGHTS AND GO TO FORM1 
                    NFormMainScreenCIT = new FormMainScreenCIT(WSignedId, WSignRecordNo, WSecLevel, WOperator);
                    //NFormMainScreenCIT.LoggingOut
                    NFormMainScreenCIT.LoggingOut += NFormMainScreenCIT_LoggingOut;
                    NFormMainScreenCIT.Show();
                    this.Hide();
                    return;
                }

                if (WOperator == "ITMX")
                {

                    if (Us.BankId == "ITMX")
                    {
                        IsITMX = true;
                    }
                    else
                    {
                        if (Us.BankId == "CBT")
                        {
                            IsCentralBank = true;
                        }
                        else
                        {
                            IsBank = true;
                        }
                    }

                    if (IsITMX)
                    {
                        //This is ITMX and other Banks 
                        // READ DETAILS OF ACCESS RIGHTS AND GO TO FORM1 
                        NFormMainScreenSwitch = new FormMainScreenSwitch(WSignedId, WSignRecordNo, WSecLevel, WOperator);
                        //NFormMainScreenCIT.LoggingOut
                        NFormMainScreenSwitch.LoggingOut += NFormMainScreenSwitch_LoggingOut;
                        NFormMainScreenSwitch.Show();
                        this.Hide();
                        return;
                    }

                    if (IsBank)
                    {
                        // THIS IS ANY MEMBER BANK in THAILAND 
                        // READ DETAILS OF ACCESS RIGHTS AND GO TO FORM1 
                        NForm1ITMXBANKS = new Form1ITMXBANKS(WSignedId, WSignRecordNo, WSecLevel, WOperator);
                        //NFormMainScreenCIT.LoggingOut
                        NForm1ITMXBANKS.LoggingOut += NForm1ITMXBANKS_LoggingOut;
                        NForm1ITMXBANKS.Show();
                        this.Hide();
                        return;
                    }

                    if (IsCentralBank)
                    {
                        // THIS IS CENTRAL BANK THAILAND 
                        // READ DETAILS OF ACCESS RIGHTS AND GO TO FORM1 
                        NForm1ITMXCBT = new Form1ITMXCBT(WSignedId, WSignRecordNo, WSecLevel, WOperator);
                        //NFormMainScreenCIT.LoggingOut
                        NForm1ITMXCBT.LoggingOut += NForm1ITMXCBT_LoggingOut;
                        NForm1ITMXCBT.Show();
                        this.Hide();
                        return;
                    }
                }
                else
                {
                    RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
                    Usi.ReadSignedActivityByKey(WSignRecordNo);

                    if ((Usi.SecLevel == "10" & W_Application == "ATMS/CARDS")
                        || (Usi.SecLevel == "10" & W_Application == "e_MOBILE")
                        )
                    {
                        Form_ATMS_ADMIN NForm_ATMS_ADMIN;

                        NForm_ATMS_ADMIN = new Form_ATMS_ADMIN(WSignedId, WSignRecordNo, WSecLevel, WOperator);
                        NForm_ATMS_ADMIN.LoggingOut += NForm_ATMS_ADMIN_LoggingOut;
                        NForm_ATMS_ADMIN.Show();
                        //return; 
                        this.Hide();
                        return;
                    }
                    if ((Usi.SecLevel == "08" & W_Application == "ATMS/CARDS") // show controller scre
                        || (Usi.SecLevel == "08" & W_Application == "e_MOBILE") // show controller scre
                       )
                    {

                        if (Controllers_Form == "EMR")
                        {
                            Form_ATMS_Controller_EMR NForm_ATMS_Controller_EMR;

                            NForm_ATMS_Controller_EMR = new Form_ATMS_Controller_EMR(WSignedId, WSignRecordNo, WSecLevel, WOperator);
                            // NForm_ATMS_ADMIN.LoggingOut += NForm_ATMS_ADMIN_LoggingOut;
                            NForm_ATMS_Controller_EMR.LoggingOut += NForm_ATMS_ADMIN_LoggingOut;
                            NForm_ATMS_Controller_EMR.Show();
                        }
                        else
                        {
                            if (Controllers_Form == "EGA")
                            {
                                Form_ATMS_Controller_EGA NForm_ATMS_Controller_EGA;

                                NForm_ATMS_Controller_EGA = new Form_ATMS_Controller_EGA(WSignedId, WSignRecordNo, WSecLevel, WOperator);
                                // NForm_ATMS_ADMIN.LoggingOut += NForm_ATMS_ADMIN_LoggingOut;
                                NForm_ATMS_Controller_EGA.LoggingOut += NForm_ATMS_ADMIN_LoggingOut;
                                NForm_ATMS_Controller_EGA.Show();
                            }
                            else
                            {
                                // BDC ALL APPLICATIONS 
                                Form_ATMS_Controller NForm_ATMS_Controller;

                                NForm_ATMS_Controller = new Form_ATMS_Controller(WSignedId, WSignRecordNo, WSecLevel, WOperator);
                                // NForm_ATMS_ADMIN.LoggingOut += NForm_ATMS_ADMIN_LoggingOut;
                                NForm_ATMS_Controller.LoggingOut += NForm_ATMS_ADMIN_LoggingOut;
                                NForm_ATMS_Controller.Show();
                            }
                            
                        }

                        this.Hide();
                        return;
                    }

                    if ((Usi.SecLevel == "11" & W_Application == "ATMS/CARDS") // SHOW Total Rich Screen
                       )
                    {
                        NForm1ATMs = new Form1ATMs(WSignedId, WSignRecordNo, WSecLevel, WOperator);
                        NForm1ATMs.LoggingOut += NForm1ATMs_LoggingOut;
                        NForm1ATMs.Show();
                        this.Hide();
                        return;
                    }

                    if (WSecLevel == "02"
                        || WSecLevel == "03"
                        || WSecLevel == "04"
                        || WSecLevel == "05"
                        || WSecLevel == "06"
                        || WSecLevel == "07"
                        // || WSecLevel == "08"
                        || WSecLevel == "09"
                         || WSecLevel == "10" & W_Application == "NOSTRO"
                         || WSecLevel == "10" & W_Application == "E_FINANCE RECONCILIATION"
                        )
                    {
                        //
                        // W_Application = "ATMS/CARDS"; =>
                        // W_Application = "NOSTRO";
                        // W_Application = "POS SETTLEMENT";
                        // W_Application = "E_FINANCE RECONCILIATION";
                        // W_Application = "FAWRY RECONCILIATION";
                        //

                        if (W_Application == "ATMS/CARDS"
                            || W_Application == "e_MOBILE"
                            )
                        {
                            if (Reconciliator_Form == "BDC")
                            {
                                if (IsBranch_001 == true)
                                {
                                    NForm1ATMs_Reconciliator_BDC_TYPE_2 = new Form1ATMs_Reconciliator_BDC_TYPE_2(WSignedId, WSignRecordNo, WSecLevel, WOperator);
                                    NForm1ATMs_Reconciliator_BDC_TYPE_2.LoggingOut += NForm1ATMs_LoggingOut;
                                    NForm1ATMs_Reconciliator_BDC_TYPE_2.Show();
                                    this.Hide();
                                    return;
                                }
                                else
                                {
                                    if (W_Application == "e_MOBILE")
                                    {
                                       
                                            Form1ATMs_Reconciliator_EWALLET NForm1ATMs_Reconciliator_EWALLET;

                                            NForm1ATMs_Reconciliator_EWALLET = new Form1ATMs_Reconciliator_EWALLET(WSignedId, WSignRecordNo, WSecLevel, WOperator);
                                            NForm1ATMs_Reconciliator_EWALLET.LoggingOut += NForm1ATMs_LoggingOut;
                                            NForm1ATMs_Reconciliator_EWALLET.Show();
                                            this.Hide();
                                            return;
                                    }
                                    else
                                    {
                                        NForm1ATMs_Reconciliator = new Form1ATMs_Reconciliator(WSignedId, WSignRecordNo, WSecLevel, WOperator);
                                        NForm1ATMs_Reconciliator.LoggingOut += NForm1ATMs_LoggingOut;
                                        NForm1ATMs_Reconciliator.Show();
                                        this.Hide();
                                        return;
                                    }
                                }

                            }
                            if (Reconciliator_Form == "EGA")
                            {
                                Form1ATMs_Reconciliator_EGATE NForm1ATMs_Reconciliator_EGATE;

                                NForm1ATMs_Reconciliator_EGATE = new Form1ATMs_Reconciliator_EGATE(WSignedId, WSignRecordNo, WSecLevel, WOperator);
                                NForm1ATMs_Reconciliator_EGATE.LoggingOut += NForm1ATMs_LoggingOut;
                                NForm1ATMs_Reconciliator_EGATE.Show();
                                this.Hide();
                                return;
                            }
                            if (Reconciliator_Form == "ABE")
                            {
                                Form1ATMs_Reconciliator_ABE NForm1ATMs_Reconciliator_ABE;
                                NForm1ATMs_Reconciliator_ABE = new Form1ATMs_Reconciliator_ABE(WSignedId, WSignRecordNo, WSecLevel, WOperator);
                                NForm1ATMs_Reconciliator_ABE.LoggingOut += NForm1ATMs_LoggingOut;
                                NForm1ATMs_Reconciliator_ABE.Show();
                                this.Hide();
                                return;
                            }
                            if (Reconciliator_Form == "EMR")
                            {
                                Form1ATMs_Reconciliator_EMR NForm1ATMs_Reconciliator_EMR;
                                NForm1ATMs_Reconciliator_EMR = new Form1ATMs_Reconciliator_EMR(WSignedId, WSignRecordNo, WSecLevel, WOperator);
                                NForm1ATMs_Reconciliator_EMR.LoggingOut += NForm1ATMs_LoggingOut;
                                NForm1ATMs_Reconciliator_EMR.Show();
                                this.Hide();
                                return;
                            }

                        }

                        if (W_Application == "NOSTRO"
                             || W_Application == "POS SETTLEMENT"
                            || (Usi.SecLevel == "10" & W_Application == "NOSTRO")
                            )
                        {
                            Form_NOSTRO_OPERATIONAL NForm_NOSTRO_OPERATIONAL;

                            NForm_NOSTRO_OPERATIONAL = new Form_NOSTRO_OPERATIONAL(WSignedId, WSignRecordNo, WSecLevel, WOperator);
                            NForm_NOSTRO_OPERATIONAL.LoggingOut += NForm_NOSTRO_OPERATIONAL_LoggingOut;
                            NForm_NOSTRO_OPERATIONAL.Show();
                            this.Hide();
                            return;
                        }
                        // Form1_E_FINANCE
                        if (
                           (Usi.SecLevel == "10" & W_Application == "E_FINANCE RECONCILIATION")

                           || W_Application == "E_FINANCE RECONCILIATION"
                           )
                        {
                            Form1_E_FINANCE NForm1_E_FINANCE;

                            NForm1_E_FINANCE = new Form1_E_FINANCE(WSignedId, WSignRecordNo, WSecLevel, WOperator);
                            NForm1_E_FINANCE.LoggingOut += NForm_NOSTRO_OPERATIONAL_LoggingOut;
                            NForm1_E_FINANCE.Show();
                            this.Hide();
                            return;
                        }
                    }

                }

            }


        }

        void NForm_ATMS_ADMIN_LoggingOut(object sender, EventArgs e)
        {

            Usi.ReadSignedActivityByKey(WSignRecordNo);

            Usi.SignInStatus = false;

            Usi.DtTmOut = DateTime.Now;

            Usi.UpdateSignedInTableDateTmOut(WSignRecordNo);

            //   SetValues();
            this.Dispose();
        }

        void NForm_ATMS_OPERATIONAL_LoggingOut(object sender, EventArgs e)
        {

            Usi.ReadSignedActivityByKey(WSignRecordNo);

            Usi.SignInStatus = false;

            Usi.DtTmOut = DateTime.Now;

            Usi.UpdateSignedInTableDateTmOut(WSignRecordNo);

            //   SetValues();
            this.Dispose();
        }
        void NForm_NOSTRO_OPERATIONAL_LoggingOut(object sender, EventArgs e)
        {

            Usi.ReadSignedActivityByKey(WSignRecordNo);

            Usi.SignInStatus = false;

            Usi.DtTmOut = DateTime.Now;

            Usi.UpdateSignedInTableDateTmOut(WSignRecordNo);

            //   SetValues();
            this.Dispose();
        }
        void NForm1ATMs_LoggingOut(object sender, EventArgs e)
        {

            Usi.ReadSignedActivityByKey(WSignRecordNo);

            Usi.SignInStatus = false;

            Usi.DtTmOut = DateTime.Now;

            Usi.UpdateSignedInTableDateTmOut(WSignRecordNo);

            //   SetValues();
            this.Dispose();
        }

        void NFormMainScreenCIT_LoggingOut(object sender, EventArgs e)
        {


            Usi.ReadSignedActivityByKey(WSignRecordNo);

            Usi.SignInStatus = false;

            Usi.DtTmOut = DateTime.Now;

            Usi.UpdateSignedInTableDateTmOut(WSignRecordNo);

            //   SetValues();
            this.Dispose();
        }

        void NForm1ITMXBANKS_LoggingOut(object sender, EventArgs e)
        {


            Usi.ReadSignedActivityByKey(WSignRecordNo);

            Usi.SignInStatus = false;

            Usi.DtTmOut = DateTime.Now;

            Usi.UpdateSignedInTableDateTmOut(WSignRecordNo);

            //   SetValues();
            this.Dispose();
        }

        void NFormMainScreenSwitch_LoggingOut(object sender, EventArgs e)
        {
            NFormMainScreenSwitch.Dispose();

            Usi.ReadSignedActivityByKey(WSignRecordNo);

            Usi.SignInStatus = false;

            Usi.DtTmOut = DateTime.Now;

            Usi.UpdateSignedInTableDateTmOut(WSignRecordNo);

            //   SetValues();
            this.Dispose();
        }

        void NForm1ITMXCBT_LoggingOut(object sender, EventArgs e)
        {
            //NForm1ITMXCBT.Dispose();

            Usi.ReadSignedActivityByKey(WSignRecordNo);

            Usi.SignInStatus = false;

            Usi.DtTmOut = DateTime.Now;

            Usi.UpdateSignedInTableDateTmOut(WSignRecordNo);

            //   SetValues();
            this.Dispose();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxLanguage.SelectedItem.ToString().Equals("English"))
            {
                culture = CultureInfo.CreateSpecificCulture("el-GR");
            }
            if (comboBoxLanguage.SelectedItem.ToString().Equals("Français"))
            {
                culture = CultureInfo.CreateSpecificCulture("fr-FR");
            }

            SetCulture();
        }
        // Change password 

        private void buttonChange_Click_1(object sender, EventArgs e)
        {
            // HERE user had insert the user id and password 

            if (comboBoxUser.Text == "")
            {
                MessageBox.Show("Please enter your user Id");
                return;
            }

            if (txtBoxPassword.Text == "")
            {
                MessageBox.Show("Please enter your password");
                return;
            }

            WSignedId = comboBoxUser.Text;
            string InputPassword = txtBoxPassword.Text;

            // All Validations must be made before allowed to change password

            UserPasswordValidation(WSignedId, InputPassword, "Change");

            if (SuccesfulSigned == true)
            {
                // Validations had passed
                // Inititilize password 
                txtBoxPassword.Text = "";
                NForm41 = new Form41(WSignedId, WOperator);
                NForm41.Show();

            }
            else
            {
                return;
            }

        }
        // If change 
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkBoxShowPassword.Checked = false;
            SetValues();
        }
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

                WSignedId = comboBoxUser.Text;

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
                    MessageBox.Show(comboBoxUser.Text, MSG);
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

        private void UserPasswordValidation(string InUserId, string InPassword, string InMode)
        {
            // InMode = Login OR Change 
            SuccesfulSigned = false;
            try
            {
                Us.ReadUsersRecord_FULL(InUserId); // Read USER record for the signed user

                if (Us.ErrorFound == true)
                {
                    MessageBox.Show("System Problem. Most likely SQL Not loaded yet" + Environment.NewLine
                                       + "Wait for few seconds and retry to login"
                                        );
                    return;
                }
            }
            catch (Exception ex)
            {

                CatchDetails(ex);
                return;
            }
            // ===========================================
            if (Us.RecordFound == false)
            {
                MSG = LocRM.GetString("Form40MSG002", culture);
                MessageBox.Show(comboBoxUser.Text, MSG);
                //   MessageBox.Show(" User Not Found ");
                return;
            }
            else
            {
                // Check if User 
                // Inactive

                if (Us.UserInactive == true)
                {
                    MessageBox.Show("User is Inactive" + Environment.NewLine
                        + "No action allowed"
                        );
                    return;
                }

                // Check if User 
                // Inactive

                if (Us.UserInactive == false & Us.Is_Approved == false)
                {
                    MessageBox.Show("User is Active But Not Approved By Checker" + Environment.NewLine
                        + "No action allowed"
                        );
                    return;
                }

                // On Leave
                Gp.ReadParametersSpecificId(WOperator, "451", "8", "", "");
                int OnLeave = ((int)Gp.Amount);

                if (OnLeave != 0)
                {
                    // Make Check
                    if (Us.UserOnLeave == true)
                    {
                        MessageBox.Show("User is on leave" + Environment.NewLine
                            + "No action allowed"
                            );
                        return;
                    }
                }

                // Password check 
                bool PasswordMatched = En.CheckPassword(InPassword, Us.PassWord);

                if (PasswordMatched == false)
                {
                    MSG = LocRM.GetString("Form40MSG003", culture);
                    MessageBox.Show("For User: " + comboBoxUser.Text, MSG);
                    // MessageBox.Show(" Wrong User or Password ");

                    NumberOfAttempts = NumberOfAttempts + 1;

                    // Attempts
                    Gp.ReadParametersSpecificId(WOperator, "451", "3", "", "");
                    int MaximunNumberOfAttempts = ((int)Gp.Amount);

                    // Register All Unsucceful attempts
                    Gp.ReadParametersSpecificId(WOperator, "451", "9", "", "");
                    int RegisterAttempts = ((int)Gp.Amount);

                    if (RegisterAttempts == 1)
                    {
                        // Insert record in table 
                        bool OverMaximumAttempts = false;
                        if (NumberOfAttempts > MaximunNumberOfAttempts)
                        {
                            OverMaximumAttempts = true;
                        }
                        Usi.InsertUnsuccesfulAttempts(WSignedId, Us.UserName, NumberOfAttempts, OverMaximumAttempts);
                    }

                    if (NumberOfAttempts > MaximunNumberOfAttempts)
                    {
                        MessageBox.Show("You have pass the number of attempts " + Environment.NewLine
                                        + "Your Account will be blocked."
                                           );
                        //
                        // Block User
                        //
                        Gp.ReadParametersSpecificId(WOperator, "451", "10", "", "");
                        int BlockUser = ((int)Gp.Amount);

                        if (BlockUser == 1)
                        {
                            Us.ReadUsersRecord(WSignedId);
                            Us.UserInactive = true;
                            Us.UpdateUser(WSignedId);
                        }

                        return;
                    }

                    return;
                }
                else
                {
                    // Password Matched
                    //SuccesfulSigned = true;
                }
            }

            // Expired password

            if (DateTime.Now > Us.DtToBeChanged)
            {
                // Your Password has expired
                MessageBox.Show("Your Password has expired"
                               + "Your administrator must generate a new one."
                                 );

                return;
            }

            // WARNING 
            if (InMode == "Login")
            {

                Gp.ReadParametersSpecificId(Us.Operator, "451", "2", "", "");
                int WarningDays = ((int)Gp.Amount);

                if (DateTime.Now > Us.DtToBeChanged.AddDays(-WarningDays))
                {
                    // Your Password will expire in so many days
                    int TempRem = Convert.ToInt32((Us.DtToBeChanged - DateTime.Now).TotalDays);
                    MessageBox.Show("Days remaining to change your password : " + TempRem.ToString());
                }

            }

            if (Us.ForceChangePassword == true)
            {
                // Your Password must change 
                int TempPasswordLifeDays = 0;
                Gp.ReadParametersSpecificId(Us.Operator, "451", "11", "", "");
                TempPasswordLifeDays = ((int)Gp.Amount);

                if (TempPasswordLifeDays != 0)
                {
                    // Check is Temporary password has expired
                    if (DateTime.Now > Us.MaxDateTmForTempPassword)
                    {
                        MessageBox.Show("Your Temporary Password has expired" + Environment.NewLine
                            + "Request a new temporary password"
                            );
                        return;
                    }
                }
                if (InMode == "Login")
                {
                    // Your Password must change 
                    MessageBox.Show("Please Change password");
                    // return;  Leave it here 
                }
            }

            SuccesfulSigned = true;

        }

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
        // e_Mobile
        private void radioButton_e_MOBILE_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_e_MOBILE.Checked == true)
            {
                if (WBankShortName == "EGA")
                {
                    panelMobileApl.Hide();
                }
                else
                {
                    panelMobileApl.Show();
                }
               
                radioButton_T24.Checked = false;
                radioButton_T24.Hide();
            }
            else
            {
                panelMobileApl.Hide();
            }
        }
        // ATMS AND CARDS 
        private void radioButtonATMS_CARDS_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonATMS_CARDS.Checked == true)
            {
                panelMobileApl.Hide();
               
                if (WBankShortName == "BDC")
                {
                    radioButton_T24.Show();
                }
                else
                {
                    radioButton_T24.Checked = false;
                    panelMobileApl.Hide();
                }
           
            }
            else
            {
                //radioButton_T24.Show();
                //radioButton_T24.Checked = false;
               // panelMobileApl.Hide();
            }
        }
        // NOSTRO 
        private void radioButtonNOSTRO_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonNOSTRO.Checked == true)
            {
                panelMobileApl.Hide();
            }
            else
            {
                panelMobileApl.Hide();
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
    }
}

