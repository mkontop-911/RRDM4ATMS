using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RRDM4ATMs; 

namespace RRDM4ATMsWin
{
    public partial class Form20 : Form
    {

      //   string WOperator;
       //  string WAccessToBankTypes;

        bool WActiveDirectory;// Ad
       
        int WSecLevel;

        string WSignedId;
        int WSignRecordNo;
       
        string WChosenUserId;
        int WFunctionNo;

        string WOperator;

        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord(); // Make class availble 

        RRDMComboClass Cc = new RRDMComboClass();

        RRDMGasParameters Gp = new RRDMGasParameters(); 

        RRDMBanks Ba = new RRDMBanks();

        RRDMEmailClass2 Em = new RRDMEmailClass2();

        RRDMUserVsAuthorizers Ua = new RRDMUserVsAuthorizers();

        RRDMActiveDirectory Ad = new RRDMActiveDirectory(); 

        public Form20(string InSignedId, int SignRecordNo,  
                               string InChosenUserId, int InFunctionNo)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WChosenUserId = InChosenUserId;
            WFunctionNo = InFunctionNo; // 1 For ADD , 2 For Update, 3 for Viewing

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;

            //**********************************************************
            //********** ACTIVE DIRECTORY ******************************
            //************* START **************************************
            //**********************************************************

            Ad.CheckActiveDirectory();

            if (Ad.DomainFound & Ad.ValidDomain & Ad.AdRRDMYes & Ad.UserInGroup)
            {
                WActiveDirectory = true;

               // Do not create password. 
               checkBox4.Hide();
               checkBox3.Hide();
               checkBox5.Hide();

               if (Ad.NameFromActive) textBox2.ReadOnly = true;
               if (Ad.MobileFromActive) textBox5.ReadOnly = true;
               if (Ad.EmailFromActive) textBox4.ReadOnly = true; 
            
            }

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
            Us.ReadUsersRecord(InSignedId); // Read USER record for the signed user
    
            WOperator = Us.Operator;
            WSecLevel = Us.SecLevel;

            // Banks available for the seed bank 
            comboBox5.DataSource = Cc.GetBanksIds(WOperator);
            comboBox5.DisplayMember = "DisplayValue";
            // ===============================

            // Role name  
            Gp.ParamId = "803";
            comboBox1.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBox1.DisplayMember = "DisplayValue";
          
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
                        //if (Ad.NameFromActive) textBox2.Text = Ad.UserName;
                        //if (Ad.MobileFromActive) textBox5.Text = Ad.UserPhone;
                        //if (Ad.EmailFromActive) textBox4.Text = Ad.UserMail;

                        if (Ad.NameFromActive) Us.UserName = Ad.UserName;
                        if (Ad.MobileFromActive) Us.MobileNo = Ad.UserPhone;
                        if (Ad.EmailFromActive) Us.email = Ad.UserMail;
                    
                }
               

                textBoxUserId.Text = Us.UserId;

                textBoxUserId.ReadOnly = true; 

                textBox2.Text = Us.UserName;

                comboBox5.Text = Us.BankId;

                textBox6.Text = Us.Branch;
              
                comboBox3.Text = Us.UserType;
                comboBox1.Text = Us.RoleNm;

                textBox3.Text = Us.SecLevel.ToString();
              
                textBox4.Text = Us.email;
                textBox5.Text = Us.MobileNo; // String

                checkBoxAuthoriser.Checked = Us.Authoriser;

                checkBoxDisputeOfficer.Checked = Us.DisputeOfficer;

                checkBoxReconcOfficer.Checked = Us.ReconcOfficer;
                checkBoxReconcMgr.Checked = Us.ReconcMgr; 

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
        // ADD USER
        private void button1_Click_1(object sender, EventArgs e)
        {        

            // (WFunctionNo == 1)

            // Fill In the fields
            Us.UserId = textBoxUserId.Text;
          
            if (Us.UserId == "")
            {
                MessageBox.Show(textBoxUserId.Text, "Please enter a User Id!");
                return;
            }


            // Read User - If 1 it doesnt exist if 2 exist 
            Us.ReadUsersRecord(Us.UserId);

            if (Us.RecordFound == true & Us.Operator == WOperator)
                {
                    MessageBox.Show("User already Exist");
                    return;
                }
            // Read Info from active directory 
            if (WActiveDirectory == true)
            {
             

                    if (Ad.NameFromActive) textBox2.Text = Ad.UserName;
                    if (Ad.MobileFromActive) textBox5.Text = Ad.UserPhone;
                    if (Ad.EmailFromActive) textBox4.Text = Ad.UserMail;

                    //if (Ad.NameFromActive) Us.UserName = Ad.UserName;
                    //if (Ad.MobileFromActive) Us.MobileNo = Ad.UserPhone;
                    //if (Ad.EmailFromActive) Us.email = Ad.UserMail;
                
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

            Us.UserName = textBox2.Text;

            Us.Culture = "English";
            Us.BankId = comboBox5.Text;
            Us.Branch = textBox6.Text;
            Us.UserType = comboBox3.Text;
            Us.RoleNm = comboBox1.Text;

            Us.SecLevel = int.Parse(textBox3.Text);

            if ((WSecLevel == 9 & Us.SecLevel != 7) || (WSecLevel == 7 & Us.SecLevel != 6)
                || (WSecLevel == 6 & Us.SecLevel > 5))
            {
                MessageBox.Show(textBox3.Text, "Please enter correct Security Level.");
                return;
            }
            if (textBox4.Text == "")
            {
                MessageBox.Show(textBox3.Text, "Please enter email id.");
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

            if (checkBoxAuthoriser.Checked == true)
            {
                Us.Authoriser = true;
            }
            else Us.Authoriser = false;

            if (checkBoxDisputeOfficer.Checked == true)
            {
                Us.DisputeOfficer = true;
            }
            else Us.DisputeOfficer = false;

            if (checkBoxReconcOfficer.Checked == true)
            {
                Us.ReconcOfficer = true;
            }
            else Us.ReconcOfficer = false;

            if (checkBoxReconcMgr.Checked == true)
            {
                Us.ReconcMgr = true;
            }
            else Us.ReconcMgr = false;

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

            Us.CreatePassword(8);
            Us.PassWord = Us.CreatePassword(8); // Here we insert a generated password 

            Gp.ReadParametersSpecificId(WOperator, "451", "1", "", "");
            int Temp = ((int)Gp.Amount);

            Us.DtChanged = DateTime.Now;
            Us.DtToBeChanged = DateTime.Now.AddDays(Temp);
            Us.ForceChangePassword = true;

            
           
            //========================PasswordEND============================
           
         //   Us.AccessToBankTypes = WAccessToBankTypes;
            Us.Operator = WOperator;
            
            Us.InsertUser(Us.UserId);

            // Send email with New Password
            if (checkBox3.Checked == true)
            {
                string Recipient = Us.email;

                string Subject = "Your Password";

                string EmailBody = "Your Password for using RRDM for ATMs is : " + Us.PassWord;

                Em.SendEmail(WOperator, Recipient, Subject, EmailBody);
            }

            if (checkBox5.Checked == true)
            {
                Us.ReadUsersRecord(WSignedId);// Get email of the creator = controller

                string Recipient = Us.email;

                string Subject = "Your Password";

                string EmailBody = "Your Password for using RRDM for ATMs is : " + Us.PassWord;

                Em.SendEmail(WOperator, Recipient, Subject, EmailBody);
            }

            MessageBox.Show("User is added and email with password sent.");

            textBoxMsgBoard.Text = " USER ADDED IN DATA BASES ";

            button1.Hide();

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
            Us.UserId = textBoxUserId.Text;

            // Read User - If 1 it doesnt exist if 2 exist 
            Us.ReadUsersRecord(Us.UserId);

            Us.UserName = textBox2.Text;

            Us.Culture = "English";
            Us.BankId = comboBox5.Text;
            Us.Branch = textBox6.Text;
            Us.UserType = comboBox3.Text;
            Us.RoleNm = comboBox1.Text;

            // SECURITY LEVEL 
            if (int.TryParse(textBox3.Text, out Us.SecLevel))
            {
            }
            else
            {
                MessageBox.Show(textBox3.Text, "Please enter a valid number!");
                return;
            }

            if ((WSecLevel == 9 & Us.SecLevel != 7) || (WSecLevel == 7 & Us.SecLevel != 6)
                || (WSecLevel == 6 & Us.SecLevel > 5))
            {
                MessageBox.Show(textBox3.Text, "Please enter correct Security Level.");
                return;
            }

            if (checkBox4.Checked == true) // Password wish generation was inputed  
            {
                if (checkBox3.Checked == false & checkBox5.Checked == false)
                {
                    MessageBox.Show("Please choose delivery method of password");
                    return;
                }

                Us.CreatePassword(8);
                Us.PassWord = Us.CreatePassword(8); // Here we insert a generated password 

                Gp.ReadParametersSpecificId(WOperator, "451", "1", "", "");
                int Temp = ((int)Gp.Amount);

                Us.DtChanged = DateTime.Now;
                Us.DtToBeChanged = DateTime.Now.AddDays(Temp);
                Us.ForceChangePassword = true;

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

            if (checkBoxAuthoriser.Checked == true)
            {
                Us.Authoriser = true;
            }
            else Us.Authoriser = false;

            if (checkBoxDisputeOfficer.Checked == true)
            {
                Us.DisputeOfficer = true;
            }
            else Us.DisputeOfficer = false;

            if (checkBoxReconcOfficer.Checked == true)
            {
                Us.ReconcOfficer = true;
            }
            else Us.ReconcOfficer = false;

            if (checkBoxReconcMgr.Checked == true)
            {
                Us.ReconcMgr = true;
            }
            else Us.ReconcMgr = false;

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

                string EmailBody = "Your Password for using RRDM for ATMs is : " + Us.PassWord;

                Em.SendEmail(WOperator, Recipient, Subject, EmailBody);
            }

            if (checkBox5.Checked == true)
            {
                Us.ReadUsersRecord(WSignedId);// Get email of the creator = controller

                string Recipient = Us.email;

                string Subject = "Your Password";

                string EmailBody = "Your Password for using RRDM for ATMs is : " + Us.PassWord;

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
              
        }
             
        // ON ROLE CHANGE DO

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (comboBox1.Text == "Global GAS Master(9)") textBox3.Text = "9";
            if (comboBox1.Text == "Bank GAS Master(7)") textBox3.Text = "7";
            if (comboBox1.Text == "Maintenance Master(6)") textBox3.Text = "6";
            if (comboBox1.Text == "MIS Master(5)") textBox3.Text = "5";
            if (comboBox1.Text == "ATMs Controller(4)") textBox3.Text = "4";
            if (comboBox1.Text == "ATM/Group Officer(3)") textBox3.Text = "3"; 

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

                label9.Show();
                label2.Show();

                comboBox1.Show();
                textBox3.Show();
            }
            else
            {
                label13.Hide();
                label1.Hide();
                comboBox4.Hide();
                textBox8.Hide();

                comboBox4.Text = "";
                textBox8.Text = "";

                label9.Hide();
                label2.Hide();
              
                comboBox1.Hide();
                textBox3.Hide();

                comboBox1.Text = "";
                textBox3.Text = "0"; 
            }

        }
        // If Authoriser = goes to No Authoriser then delete
        private void checkBoxAuthoriser_CheckedChanged(object sender, EventArgs e)
        {
            bool OpenRecord; 

            if (checkBoxAuthoriser.Checked == false)
            {
                OpenRecord = false;
                Ua.UpdateUserVsAuthorisationRecordOpenRecord(WChosenUserId, OpenRecord); 
            }

            if (checkBoxAuthoriser.Checked == true) 
            {
                OpenRecord = true;
                Ua.UpdateUserVsAuthorisationRecordOpenRecord(WChosenUserId, OpenRecord);
            }
        }
// Finish
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }       

    }
}

