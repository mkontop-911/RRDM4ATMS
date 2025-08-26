using System;
using System.Windows.Forms;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;

using RRDM4ATMs;
using System.Drawing;
using RRDM4ATMsClasses;

namespace RRDM4ATMsWin
{
    public partial class Form20 : Form
    {

        bool WActiveDirectory;// Ad

        // FOR AUDIT TRAIL
        Bitmap SCREENinitial;
        int AuditTrailUniqueID = 0;
        // ************************

        string WPassEncrypted_1_to_8; // 1 to 8 

        string WSecLevel;
        bool ITMXUser;

        string WOperator;

        string WSignedId;
        int WSignRecordNo;

        string WChosenUserId;
        int WFunctionNo;
        int WApplication; 

        string WPassWord; 


        RRDMUsersRecords Us = new RRDMUsersRecords(); // Make class availble 

        RRDMEncryptPasswordOrField En = new RRDMEncryptPasswordOrField(); 

        RRDMComboClass Cc = new RRDMComboClass();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMBanks Ba = new RRDMBanks();

        RRDMEmailClass2 Em = new RRDMEmailClass2();

        RRDMUserVsAuthorizers Ua = new RRDMUserVsAuthorizers();

        RRDMActiveDirectory Ad = new RRDMActiveDirectory();

        RRDMBank_Branches Bb = new RRDMBank_Branches();

        RRDM_AuditTrailClass_NEW At = new RRDM_AuditTrailClass_NEW();

        public Form20(string InOperator,string InSignedId, int SignRecordNo,  
                               string InChosenUserId, int InFunctionNo, int InApplication)
        {
            WOperator = InOperator;
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WChosenUserId = InChosenUserId;
            WFunctionNo = InFunctionNo; // 1 For ADD , 2 For Update, 3 for Viewing
            WApplication = InApplication;
            // 1 ATMs + JCC
            // 2 Cashless
            // 3 NOSTRO

            InitializeComponent();

            if (WFunctionNo == 2 || WFunctionNo == 3)
            {
                textBoxUserId.ReadOnly = true; 
            }

            // Set Working Date 
            RRDMGasParameters Gp = new RRDMGasParameters();
            string ParId = "267";
            string OccurId = "1";
          
            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;

            // Read and save Encrypted Password 
            string ModelUser = "PILOT_001";  
            Us.ReadUsersRecord(ModelUser);
            WPassEncrypted_1_to_8 = Us.PassWord; 

            Us.ReadUsersRecord(WSignedId);

            //if (Us.Operator == Us.BankId & )
            //{
            //    ITMXUser = true;
            //}
            //else
            //{
            //    ITMXUser = false;
            //}

            //**********************************************************
            //********** ACTIVE DIRECTORY ******************************
            //************* START **************************************
            //**********************************************************

            ParId = "264";
            OccurId = "1";
            //TEST

            //WOperator = "ETHNCY2N"; 
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            //Gp.OccuranceNm = "YES";
            // AdDomainName

            if (Gp.OccuranceNm == "YES") // Active directory needed
            {
                // Do not create password. 
                checkBox4.Hide();
                checkBox3.Hide();
                checkBox5.Hide();

                checkBox3.Checked = false;
                checkBox4.Checked = false;
                checkBox5.Checked = false;
                
                WActiveDirectory = true;

                buttonCheckAD.Show(); 
            }
            else
            {
                WActiveDirectory = false;
                buttonCheckAD.Hide();
            }

                /*
                Ad.CheckActiveDirectory();

                if (Ad.DomainFound & Ad.ValidDomain & Ad.AdRRDMYes & Ad.UserInGroup)
                {
                    WActiveDirectory = true;

                    if (Ad.NameFromActive) textBox2.ReadOnly = true;
                    if (Ad.MobileFromActive) textBox5.ReadOnly = true;
                    if (Ad.EmailFromActive) textBox4.ReadOnly = true;

                }
                */

                //**********************************************************
                //********** ACTIVE DIRECTORY ******************************
                //************* END **************************************
                //**********************************************************

                if (WFunctionNo == 1) // New ... so create password
            {
                if (WActiveDirectory == false)
                {
                    // Create password 
                    checkBox4.Checked = true;
                    checkBox3.Checked = true;
                    checkBox5.Checked = false;
                }

                labelStep1.Text = "Add New User";
            }

            if (WFunctionNo == 2)
            {
                checkBox4.Checked = false;
                checkBox3.Checked = false;
                checkBox5.Checked = false;
                labelStep1.Text = "Update User";
            }

            if (WFunctionNo == 3)
            {
                checkBox4.Hide();
                checkBox3.Hide();
                checkBox5.Hide();
                labelStep1.Text = "View User";
            }
            // ===============================
          
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);
            //WOperator = Us.Operator;
            WSecLevel = Usi.SecLevel;

            // Banks available for the seed bank 
            comboBox5.DataSource = Cc.GetBanksIds(WOperator);
            comboBox5.DisplayMember = "DisplayValue";
            // ===============================
            comboBox5.Text = Us.BankId;
            if (ITMXUser == false)
            {
                comboBox5.Enabled = false;
            }
            else
            {
                comboBox5.Enabled = true;
            }
          
            //comboBoxRoles.DataSource = Gp.GetParamOccurancesNm(WOperator);
            //comboBoxRoles.DisplayMember = "DisplayValue";

            comboBox3.Items.Add("Employee"); // Employee 
            comboBox3.Items.Add("CIT Company"); // Employee 
            comboBox3.Items.Add("Operator Entity"); // CIT Company  =1000

            // CIT Companies 
            comboBox4.DataSource = Cc.GetCitIds(WOperator);
            comboBox4.DisplayMember = "DisplayValue";

            //      InsertUpdateMade = false;

            if (WFunctionNo == 2 || WFunctionNo == 3) // Show Information For Updating 
            {
                Us.ReadUsersRecord(WChosenUserId); // Read USER Chosen

                // Read Info from active directory 
                if (WActiveDirectory == true)
                {
                    //if (Ad.NameFromActive) Us.UserName = Ad.UserName;
                    //if (Ad.MobileFromActive) Us.MobileNo = Ad.UserPhone;
                    //if (Ad.EmailFromActive) Us.email = Ad.UserMail;
                }

                textBoxUserId.Text = Us.UserId;

                textBoxUserId.ReadOnly = true;

                textBoxUserName.Text = Us.UserName;

                comboBox5.Text = Us.BankId;

                textBoxBrachId.Text = Us.Branch;

                comboBox3.Text = Us.UserType;
                //comboBoxRoles.Text = Us.RoleNm;

                //textBox3.Text = Us.SecLevel.ToString();

                textBox4.Text = Us.email;
                textBox5.Text = Us.MobileNo; // String

                //checkBoxAuthoriser.Checked = Us.Authoriser;

                //checkBoxDisputeOfficer.Checked = Us.DisputeOfficer;

                //checkBoxReconcOfficer.Checked = Us.ReconcOfficer;
                //checkBoxReconcMgr.Checked = Us.ReconcMgr;

                checkBox1.Checked = Us.UserInactive;

                checkBox2.Checked = Us.UserOnLeave;

                comboBox4.Text = Us.CitId; // Leave always here = at the end 

                button1.Hide();

                textBoxMsgBoard.Text = "CHANGE INFORMATION AND PRESS Update BUTTON ";

            }
            if (WFunctionNo == 1) // Show Information For New 
            {
                textBoxUserId.ReadOnly = false;

                button4.Hide();

                textBoxMsgBoard.Text = "INPUT INFORMATION AND PRESS ADD BUTTON ";
            }

            if (WFunctionNo == 3) // Show Information For New 
            {

                button4.Hide();
                button1.Hide();

                textBoxMsgBoard.Text = "View Only";
            }
        }
        // Load 
        private void Form20_Load(object sender, EventArgs e)
        {
            // Add Code if needed
            System.Drawing.Bitmap memoryImage;
            memoryImage = new System.Drawing.Bitmap(tableLayoutPanelMain.Width, tableLayoutPanelMain.Height);
            tableLayoutPanelMain.DrawToBitmap(memoryImage, tableLayoutPanelMain.ClientRectangle);
            SCREENinitial = memoryImage;
        }
        // ADD USER
        private void button1_Click_1(object sender, EventArgs e)
        {
            if (comboBox3.Text == "")
            {
                MessageBox.Show("Please Enter User Type!");
                return; 
            }

            // Fill In the fields
            Us.UserId = (textBoxUserId.Text).Trim();

            if (Us.UserId == "")
            {
                MessageBox.Show(textBoxUserId.Text, "Please enter a User Id!");
                return;
            }
          
            Bb.ReadBranchByBranchId(textBoxBrachId.Text);
            if (Bb.RecordFound == false)
            {
                MessageBox.Show(textBoxUserId.Text, "Please enter a valid Branch!");
                return;
            }
            // Read User - If 1 it doesnt exist if 2 exist 
            Us.ReadIfExist(WOperator, Us.UserId);

            if (Us.RecordFound == true)
            {
                MessageBox.Show("User already Exist");
                return;
            }
            // Read Info from active directory 
            if (WActiveDirectory == true)
            {
                RRDMActiveDirectoryHelper Ah = new RRDMActiveDirectoryHelper();
                //if (Ad.NameFromActive) textBox2.Text = Ad.UserName;
                //if (Ad.MobileFromActive) textBox5.Text = Ad.UserPhone;
                //if (Ad.EmailFromActive) textBox4.Text = Ad.UserMail;
             
               
                bool IsCIT = false; 
                if (Us.UserId == "1000" 
                    ||  Us.UserId != "2000" 
                    || Us.UserId != "3000"
                    || Us.UserId != "4000"
                    || Us.UserId != "5000"
                    )
                {
                    IsCIT = true; 
                }

                bool IsInActive = Ah.isUserInAD(Us.UserId);

                if (IsInActive == false & IsCIT == false)
                {
                    MessageBox.Show("User is not in Active Directory"+Environment.NewLine
                         +"Open the user in Active and then try again"
                         
                         )
                        ;
                    return;
                }

            }


            if (checkBox4.Checked == true) // Password wish generation was inputed  
            {
                if (checkBox3.Checked == false & checkBox5.Checked == false)
                {
                    MessageBox.Show("Please choose delivery method of password");
                    return;
                }
            }

            if ((checkBox3.Checked == true || checkBox5.Checked == true) & checkBox4.Checked == false)
            {
                MessageBox.Show("Do you want generation of email? Make your selection. ");
                return;
            }

            Us.UserName = textBoxUserName.Text;

            Us.Culture = "English";
            Us.BankId = comboBox5.Text;
            Us.Branch = textBoxBrachId.Text;
            Us.UserType = comboBox3.Text;
            //Us.RoleNm = comboBoxRoles.Text;

            //Us.SecLevel = textBox3.Text;

            //if ((WSecLevel == "09" & Us.SecLevel != "07") || (WSecLevel == "07" & Us.SecLevel != "06")
            //    /*|| (WSecLevel == "06" & Us.SecLevel > "05")*/)
            //{
            //    MessageBox.Show(textBox3.Text, "Please enter correct Security Level.");
            //    return;
            //}
            if (textBox4.Text == "")
            {
                MessageBox.Show(textBox4.Text, "Please enter email id.");
                return;
            }
            string email = textBox4.Text;

            if (email != "")
            {
                System.Text.RegularExpressions.Regex expr = new System.Text.RegularExpressions.Regex
                 (@"^[a-zA-Z][\w\.-]{2,28}[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$");

                if (expr.IsMatch(email))
                {
                    //   MessageBox.Show("valid email");
                }
                else
                {
                    MessageBox.Show("invalid email");
                    return;
                }
            }

            Us.email = textBox4.Text;
            Us.MobileNo = textBox5.Text;

            if (Us.UserType == "Employee")
            {
                Us.CitId = comboBox4.Text;
            }
            else
            {
                // Input of Operator entity
                Us.CitId = Us.UserId;
            }

            Us.DateTimeOpen = DateTime.Now;


            if (checkBox1.Checked == true)
            {
                Us.UserInactive = true;
            }
            else Us.UserInactive = false;

            if (checkBox2.Checked == true)
            {
                Us.UserOnLeave = true;
            }
            else Us.UserOnLeave = false;
            //========================Password============================
            // Find Min Length of passward 
            Gp.ReadParametersSpecificId(WOperator, "451", "0", "", "");
            int UAT = ((int)Gp.Amount);
            int MinLengthOfPassword; 
            // 
            if (UAT == 1)
            {
                MinLengthOfPassword = 8; 
            }
            else
            {
                Gp.ReadParametersSpecificId(WOperator, "451", "7", "", "");
                MinLengthOfPassword = ((int)Gp.Amount);
            }
            // Find Min Length of passward 
           
            //Create Password 
            WPassWord = En.GetUniqueKey(MinLengthOfPassword); // Here we insert a generated password 
            //string WPassWord2 = En.CreatePassword(8); 

            // Encrypt and Keep 
            Us.PassWord = En.EncryptField(WPassWord);

            // THIS TEMPORARY FOR BANK DE CAIRE Because communication with email doesn't exist 

            if (UAT == 1)
            {

                Us.PassWord = WPassEncrypted_1_to_8;

                MessageBox.Show("You are in UAT environment" + Environment.NewLine
                              + "Temporary Password for BDC = 1 to 8"
                                );

            }


            Gp.ReadParametersSpecificId(WOperator, "451", "1", "", "");
            int Temp = ((int)Gp.Amount);

            Us.DtChanged = DateTime.Now;
            Us.DtToBeChanged = DateTime.Now.AddDays(Temp);
            Us.ForceChangePassword = true;

            // Your Password must change 
            int TempPasswordLifeDays = 0;
            Gp.ReadParametersSpecificId(Us.Operator, "451", "11", "", "");
            TempPasswordLifeDays = ((int)Gp.Amount);

            Us.MaxDateTmForTempPassword = DateTime.Now.AddDays(TempPasswordLifeDays);

            //========================PasswordEND============================

            //   Us.AccessToBankTypes = WAccessToBankTypes;
            Us.Operator = WOperator;

            Us.InsertUser(Us.UserId);

            // DO NOT DELETE

            //// Send email with New Password
            //if (checkBox3.Checked == true)
            //{
            //    string Recipient = Us.email;

            //    string Subject = "Your Password";

            //    string EmailBody = "Your Password for using RRDM for ATMs is : " + WPassWord;

            //    Em.SendEmail(WOperator, Recipient, Subject, EmailBody);
            //}

            //if (checkBox5.Checked == true)
            //{
            //    Us.ReadUsersRecord(WSignedId);// Get email of the creator = controller

            //    string Recipient = Us.email;

            //    string Subject = "Your Password";

            //    string EmailBody = "Your Password for using RRDM for ATMs is : " + WPassWord;

            //    Em.SendEmail(WOperator, Recipient, Subject, EmailBody);
            //}
            if (WActiveDirectory == true)
            {
                MessageBox.Show("User is added !");
            }
            else
            {
                MessageBox.Show("User is added and email with password sent.");
            }
            

            textBoxMsgBoard.Text = " USER ADDED IN DATA BASES ";
            button1.Hide();

            //Us.ChangedByWhatMaker = WSignedId; // is the Designated Maker
            //Us.DtTimeOfChange = DateTime.Now; 

            //Us.UpdateTail_For_Maker_Work((textBoxUserId.Text).Trim());

            //string WMessage = "ADD OR UPDATE USER.. " + (textBoxUserId.Text).Trim();
            ////AUDIT TRAIL 
            ////AUDIT TRAIL 
            //string AuditCategory = "Audit_Trail_For_User_Mgmt";
            //string AuditSubCategory = "ADD OR UPDATE USER";
            //string AuditAction = "ADD OR UPDATE";
            //string Message = WMessage;
            //GetMainBodyImageAndStoreIt(AuditCategory, AuditSubCategory, AuditAction, WSignedId, Message);

        }
    
        // UPDATE User

        private void button4_Click(object sender, EventArgs e)
        {
            // Email validation 
            string email = textBox4.Text;

            if (email != "")
            {
                System.Text.RegularExpressions.Regex expr = new System.Text.RegularExpressions.Regex
                 (@"^[a-zA-Z][\w\.-]{2,28}[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$");

                if (expr.IsMatch(email))
                {
                    //   MessageBox.Show("valid email");
                }
                else
                {
                    MessageBox.Show("invalid email");
                    return;
                }
            }

            // Telephone Validation 
            string Telephone = textBox5.Text;

            if (Telephone != "")
            {
                System.Text.RegularExpressions.Regex expr = new System.Text.RegularExpressions.Regex
                 (@"^\s*\+?\s*([0-9][\s-]*){9,}$");
                /*
                                ^           # Start of the string
                               \s*       # Ignore leading whitespace
                               \+?       # An optional plus
                               \s*       # followed by an optional space or multiple spaces
                               (
                                [0-9]  # A digit
                                [\s-]* # followed by an optional space or dash or more than one of those
                               )
                               {9,}     # That appears nine or more times
                               $           # End of the string
                 */

                if (expr.IsMatch(Telephone))
                {
                    //   MessageBox.Show("valid telephone");
                }
                else
                {
                    MessageBox.Show("invalid telephone");
                    return;
                }
            }
            // (WFunctionNo == 2)

            // Fill In the fields
            Us.UserId = (textBoxUserId.Text).Trim();

            // Read User - If 1 it doesnt exist if 2 exist 
            Us.ReadUsersRecord(Us.UserId);

            Us.UserName = textBoxUserName.Text;

            Us.Culture = "English";
            Us.BankId = comboBox5.Text;
            Us.Branch = textBoxBrachId.Text;
            Us.UserType = comboBox3.Text;
         
            if (checkBox4.Checked == true) // Password wish generation was inputed  
            {
                if (checkBox3.Checked == false & checkBox5.Checked == false)
                {
                    MessageBox.Show("Please choose delivery method of password");
                    return;
                }

                // Create password 
                WPassWord = En.GetUniqueKey(8); // Here we insert a generated password 
                //string WPassWord2 = En.CreatePassword(8); 

                // Encrypt and Keep 
                Us.PassWord = En.EncryptField(WPassWord);

                MessageBox.Show("Temporary Password for BDC = 1 to 8"); 

                Us.PassWord = WPassEncrypted_1_to_8;


                Gp.ReadParametersSpecificId(WOperator, "451", "1", "", "");
                int Temp = ((int)Gp.Amount);

                Us.DtChanged = DateTime.Now;
                Us.DtToBeChanged = DateTime.Now.AddDays(Temp);
                Us.ForceChangePassword = true;

                // Your Password must change 
                int TempPasswordLifeDays = 0;
                Gp.ReadParametersSpecificId(Us.Operator, "451", "11", "", "");
                TempPasswordLifeDays = ((int)Gp.Amount);

                Us.MaxDateTmForTempPassword = DateTime.Now.AddDays(TempPasswordLifeDays);


            }

            Us.email = textBox4.Text;
            Us.MobileNo = textBox5.Text;

            if (Us.UserType == "Employee")
            {
                Us.CitId = comboBox4.Text;
            }
            else
            {
                // Input of Operator entity
                Us.CitId = Us.UserId;
            }

           
            if (checkBox1.Checked == true)
            {
                Us.UserInactive = true;
            }
            else Us.UserInactive = false;

            if (checkBox2.Checked == true)
            {
                Us.UserOnLeave = true;
            }
            else Us.UserOnLeave = false;

            //    Us.AccessToBankTypes = WAccessToBankTypes;
            Us.Operator = WOperator;

            Us.UpdateUser(Us.UserId);

            // Send email with New Password
            if (checkBox3.Checked == true)
            {
                string Recipient = Us.email;

                string Subject = "Your Password";

                string EmailBody = "Your Password for using RRDM for ATMs is : " + WPassWord;

                Em.SendEmail(WOperator, Recipient, Subject, EmailBody);
            }

            if (checkBox5.Checked == true)
            {
                Us.ReadUsersRecord(WSignedId);// Get email of the creator = controller

                string Recipient = Us.email;

                string Subject = "Your Password";

                string EmailBody = "Your Password for using RRDM for ATMs is : " + WPassWord;

                Em.SendEmail(WOperator, Recipient, Subject, EmailBody);
            }

            if (checkBox4.Checked == true)
            {
                MessageBox.Show("Changed information has been updated And email for password sent.");
                textBoxMsgBoard.Text = " Email with password sent ";
            }
            else
            {
                MessageBox.Show("Changed information has been updated");
                textBoxMsgBoard.Text = " Changes completed ";
            }
          
            //Us.UpdateTail((textBoxUserId.Text).Trim());
            //string WMessage = "ADD OR UPDATE USER.. "+ (textBoxUserId.Text).Trim(); 
            ////AUDIT TRAIL 
            ////AUDIT TRAIL 
            //string AuditCategory = "Audit_Trail_For_User_Mgmt";
            //string AuditSubCategory = "ADD OR UPDATE USER";
            //string AuditAction = "ADD OR UPDATE";
            //string Message = WMessage;
            //GetMainBodyImageAndStoreIt(AuditCategory, AuditSubCategory, AuditAction, WSignedId, Message);

        }



        // If ComboSelected 

        private void comboBox4_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            string TempValue = comboBox4.Text;

            Us.ReadUsersRecord(TempValue);
            textBox8.Text = Us.UserName;
            return;

        }
             
        // Show or hide based on choice
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.Text == "Employee")
            {
                label13.Show();
                label1.Show();
                comboBox4.Show();
                textBox8.Show();

            }
            else
            {
                label13.Hide();
                label1.Hide();
                comboBox4.Hide();
                textBox8.Hide();

                comboBox4.Text = "";
                textBox8.Text = "";

          
            }

        }
        // If Authoriser = goes to No Authoriser then delete
        private void checkBoxAuthoriser_CheckedChanged(object sender, EventArgs e)
        {
           
            bool OpenRecord;

           

         
        }
// Finish
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
// Banks combobox 
        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (ITMXUser == true)
            //{
            //    if (Us.BankId != comboBox5.Text)
            //    {
            //        comboBoxRoles.Text = "Maintenance Master(6)";
            //        comboBoxRoles.Enabled = false;
            //    }
            //    else
            //    {
            //        comboBoxRoles.Enabled = true;
            //    }
            //}
            //if (ITMXUser == false) 
            //{
               
            //}
        }


// Maintain ATMS
        private void buttonMaintainAtms_Click(object sender, EventArgs e)
        {
            if (Us.UserInactive == false)
            {
                Form119 NForm119; 
                //WRowIndex = dataGridView1.SelectedRows[0].Index;
                NForm119 = new Form119(WOperator, WSignedId, WSignRecordNo, WChosenUserId, textBoxUserId.Text);
                NForm119.FormClosed += NForm119_FormClosed;
                NForm119.ShowDialog();
            }
            else
            {
                MessageBox.Show("User is Inactive. ");
                return;
            }
        }
        void NForm119_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Form20_Load(this, new EventArgs());

        }

// Branch 
        private void textBoxBrachId_TextChanged(object sender, EventArgs e)
        {
            if (textBoxBrachId.Text != "")
            {
                Bb.ReadBranchByBranchId(textBoxBrachId.Text); 

                if (Bb.RecordFound)
                {

                    textBoxBranchName.Text =Bb.BranchName ; 
                }
                else
                {
                    textBoxBranchName.Text = "NOT VALID BRANCH";
                }
            }
        }
// Check Ad
        private void buttonCheckAD_Click(object sender, EventArgs e)
        {
            if (WActiveDirectory == true)
            {
                RRDMActiveDirectoryHelper Ah = new RRDMActiveDirectoryHelper();
                //if (Ad.NameFromActive) textBox2.Text = Ad.UserName;
                //if (Ad.MobileFromActive) textBox5.Text = Ad.UserPhone;
                //if (Ad.EmailFromActive) textBox4.Text = Ad.UserMail;


                bool IsCIT = false;
                if (Us.UserId == "1000"
                    || Us.UserId != "2000"
                    || Us.UserId != "3000"
                    || Us.UserId != "4000"
                    || Us.UserId != "5000"
                    )
                {
                    IsCIT = true;
                }

                bool IsInActive = Ah.isUserInAD(Us.UserId);

                if (IsInActive == false & IsCIT == false)
                {
                    MessageBox.Show("User is not in Active Directory" + Environment.NewLine
                         + "Open the user in Active and then try again"

                         )
                        ;
                    return;
                }

                if (IsInActive == true & IsCIT == false)
                {
                    MessageBox.Show("User is in Active Directory" + Environment.NewLine
                         + ""
                         )
                        ;
                    textBoxUserName.Text = Ah.usrDisplayName; 
                    return;
                }

            }
        
            }
        //AUDIT TRAIL : GET IMAGE AND INSERT IT IN AUDIT TRAIL 
        private void GetMainBodyImageAndStoreIt(string InCategory, string InSubCategory,
            string InTypeOfChange, string InUser, string Message)
        {

            Bitmap SCREENb;
            System.Drawing.Bitmap memoryImage;
            memoryImage = new System.Drawing.Bitmap(tableLayoutPanelMain.Width, tableLayoutPanelMain.Height);
            tableLayoutPanelMain.DrawToBitmap(memoryImage, tableLayoutPanelMain.ClientRectangle);
            SCREENb = memoryImage;

            RRDM_AuditTrailClass_NEW At = new RRDM_AuditTrailClass_NEW();
            //AuditTrailUniqueID = 6; 
            if (AuditTrailUniqueID.Equals(0))
            {
                AuditTrailUniqueID = At.InsertRecord(InCategory, InSubCategory, InTypeOfChange, InUser, SCREENb, SCREENinitial, Message);
                AuditTrailUniqueID = 0;
            }
            else
            {
                At.UpdateRecord(AuditTrailUniqueID, InCategory, InSubCategory, InTypeOfChange, InUser, SCREENb, SCREENinitial, Message);
            }

        }
    }
}

