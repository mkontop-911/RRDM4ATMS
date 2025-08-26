using System;
using System.Drawing;
using System.Windows.Forms;
using System.Configuration;
using RRDM4ATMs;
using System.IO;
using TestImages; 

namespace RRDM4ATMsWin
{
    public partial class Form44 : Form
    {     
        RRDMBanks Ba = new RRDMBanks();

        RRDMGasParameters Gp = new RRDMGasParameters();
        RRDMUsersRecords Us = new RRDMUsersRecords();

        bool LoadingCycle;
        int WMode; 

        string WSignedId;
        int WSignRecordNo;
        string WOperator; 
   
        string WBankId;
        int WFunctionNo;

        public Form44(string InSignedId, int InSignRecordNo, string InOperator, string InChosenBank, int InFunctionNo)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
      
            WBankId = InChosenBank;
            WFunctionNo = InFunctionNo; // 1: Update , 2: Add, 3 : Delete 

            InitializeComponent();

            textBoxSenderPassword.PasswordChar = '*';

            // Set Working Date 
            RRDMGasParameters Gp = new RRDMGasParameters();
            string ParId = "267";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
          
            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;

            LoadingCycle = true;

            ImgFileName = ""; 

            textBox5.Text = WOperator;

            // Countries
            Gp.ParamId = "227";
            comboBox28.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBox28.DisplayMember = "DisplayValue";

            // FIRST CASSETTE

            Gp.ParamId = "201"; // Currencies first Cassette 
            comboBox1.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBox1.DisplayMember = "DisplayValue";

            Us.ReadUsersRecord(WSignedId);

            Ba.ReadBank(Us.BankId);
            if (Ba.SettlementBank == true)
            {
                WMode = 2; // Settlement Bank 
                textBox1.ReadOnly = true;
                textBox2.ReadOnly = true;
                textBox4.ReadOnly = true;
                textBox5.ReadOnly = true;
                checkBoxSettlementBank.Hide(); 
                buttonBrowse.Hide(); 

            }
            else
            {
                WMode = 1;
                textBox3.ReadOnly = true;
                textBox9.ReadOnly = true;
                
            }

            // Update or DELETE 
            if (WFunctionNo == 1 || WFunctionNo == 3 )
            {
                Ba.ReadBank(WBankId);

                if ( WFunctionNo == 1)
                {
                    if (Ba.Logo == null)
                    {
                        pictureBox.Image = null;
                        MessageBox.Show("No Logo assigned yet!");
                    }
                    else
                    {
                        MemoryStream ms = new MemoryStream(Ba.Logo);
                        pictureBox.Image = Image.FromStream(ms);
                    }
                }
              
                textBox1.Text = Ba.BankId;

                textBox6.Text = Ba.ActiveDirectoryDM;
                textBox7.Text = Ba.AdGroup;
                textBox8.Text = Ba.ShortName;
                textBox2.Text = Ba.BankName;
                comboBox28.Text = Ba.BankCountry;
                textBox4.Text = Ba.GroupName;
                textBox5.Text = Ba.Operator; 
     
                comboBox1.Text = Ba.BasicCurName;

                textBox3.Text = Ba.SettlementAccount ;
                checkBoxSettlementBank.Checked = Ba.SettlementBank ;
                textBox9.Text = Ba.SettlementClearingAccount ;

                if (checkBoxSettlementBank.Checked == true)
                {
                    label18.Show();
                    label19.Show();
                    textBox9.Show();
                }
                else
                {
                    label18.Hide();
                    label19.Hide();
                    textBox9.Hide();
                }

                textBoxSenderEmail.Text = Ba.SenderEmail;
                textBoxSenderUserName.Text = Ba.SenderUserName ;
                textBoxSenderPassword.Text = Ba.SenderPassword ;
                textBoxSenderSmtpClient.Text = Ba.SenderSmtpClient ;
                textBoxSenderPort.Text = Ba.SenderPort.ToString(); 

                checkBox2.Checked = Ba.UsingGAS;

                if (WFunctionNo == 1)
                {
                   if (WMode ==1)  labelStep1.Text = " Update Bank ";
                    if (WMode == 2) labelStep1.Text = " Update Settlement Bank Accounts ";
                    textBoxMsgBoard.Text = "Change fields and press Update ";
                    button1.Show();
                    button2.Hide();
                    button3.Hide(); 
                } 

                if (WFunctionNo == 3)
                {
                    labelStep1.Text = " Delete this Bank ";
                    textBoxMsgBoard.Text = "Verify and press Delete ";
                    button1.Hide();
                    button2.Hide();
                    button3.Show(); 
                } 

            }

            if (WFunctionNo == 2)
            {
                labelStep1.Text = " Add Bank ";
                textBox1.Text = WBankId; // When seed bank is created 
                textBoxMsgBoard.Text = "Input data for the new Bank or Operator. Bank id = SwiftNo";
                button2.Show();
                button1.Hide();
                button3.Hide();

                //TEST
                Ba.ReadBank("CRBAGRAA"); // This is the model 

                textBoxSenderEmail.Text = Ba.SenderEmail;
                textBoxSenderUserName.Text = Ba.SenderUserName;
                textBoxSenderPassword.Text = Ba.SenderPassword;
                textBoxSenderSmtpClient.Text = Ba.SenderSmtpClient;
                textBoxSenderPort.Text = Ba.SenderPort.ToString();
            }

            LoadingCycle = false;
        }
        //
        // UPDATE BANK
        //
        private void button1_Click(object sender, EventArgs e)
        {
            if (Ba.BankId != textBox1.Text)
            {
                MessageBox.Show("Bank Id not allowed to change"); 
            }
            Ba.ActiveDirectoryDM = textBox6.Text;
            Ba.AdGroup = textBox7.Text;

            Ba.ShortName = textBox8.Text;
            Ba.BankName=textBox2.Text;
            Ba.BankCountry= comboBox28.Text;

            Ba.GroupName = textBox4.Text;

            Ba.BasicCurName = comboBox1.Text;

            Ba.SettlementAccount = textBox3.Text;
            Ba.SettlementBank = checkBoxSettlementBank.Checked;
            Ba.SettlementClearingAccount = textBox9.Text;

            // Check if full information is inserted 
            if (textBoxSenderEmail.Text != "" || textBoxSenderUserName.Text != "" || textBoxSenderPassword.Text != ""
                || textBoxSenderSmtpClient.Text != "" || textBoxSenderPort.Text != "")
            {
                if (textBoxSenderEmail.Text != "" & textBoxSenderUserName.Text != "" & textBoxSenderPassword.Text != ""
                & textBoxSenderSmtpClient.Text != "" & textBoxSenderPort.Text != "")
                {
                }
                else
                {
                    MessageBox.Show("Insert full email information please");
                    //return; 
                }
            }

            Ba.SenderEmail = textBoxSenderEmail.Text;
            Ba.SenderUserName = textBoxSenderUserName.Text;
            Ba.SenderPassword = textBoxSenderPassword.Text;
            Ba.SenderSmtpClient = textBoxSenderSmtpClient.Text;
            int SenderPort; 
            if (int.TryParse(textBoxSenderPort.Text, out SenderPort))
            {
                Ba.SenderPort = SenderPort; 
            }
            else
            {
                Ba.SenderPort = 0; 
            }

            Ba.UsingGAS = checkBox2.Checked;

            Ba.UpdateBank(Ba.BankId);

            if (ImgFileName == "" )
            {
                // No Image by Browsing 
            }
            else
            {
                byte[] img = null;
                FileStream fs = new FileStream(ImgFileName, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                img = br.ReadBytes((int)fs.Length);

                Ba.ReadBank(WBankId);
                Ba.Logo = img;
                Ba.UpdateBank(WBankId);
            }
            

            MessageBox.Show("The Bank data has been updated "); 

            textBoxMsgBoard.Text = " The Bank data has been updated "; 

        }

        //
        // ADD A NEW BANK 
        //
        private void button2_Click(object sender, EventArgs e)
        {         
            Ba.BankId = textBox1.Text;
            
            Ba.ActiveDirectoryDM = textBox6.Text;
            Ba.AdGroup = textBox7.Text;
            Ba.ShortName = textBox8.Text;
            Ba.BankName = textBox2.Text;
            Ba.BankCountry = comboBox28.Text;

            Ba.GroupName = textBox4.Text; 

            Ba.BasicCurName = comboBox1.Text;

            Ba.SettlementAccount = textBox3.Text;
            Ba.SettlementBank = checkBoxSettlementBank.Checked; 
            Ba.SettlementClearingAccount = textBox9.Text; 

            Ba.DtTmCreated = DateTime.Now; 
            Ba.UsingGAS = checkBox2.Checked;

            // Check if full information is inserted 
         
                if (textBoxSenderEmail.Text != "" & textBoxSenderUserName.Text != "" & textBoxSenderPassword.Text != ""
                & textBoxSenderSmtpClient.Text != "" & textBoxSenderPort.Text != "")
                {
                    // ALL INFO INPUTED 
                }
                else
                {
                    MessageBox.Show("Insert full email information please");
                    //return;
                }
          

            Ba.SenderEmail = textBoxSenderEmail.Text;
            Ba.SenderUserName = textBoxSenderUserName.Text;
            Ba.SenderPassword = textBoxSenderPassword.Text;
            Ba.SenderSmtpClient = textBoxSenderSmtpClient.Text;

            int SenderPort;
            if (int.TryParse(textBoxSenderPort.Text, out SenderPort))
            {
                Ba.SenderPort = SenderPort;
            }
            else
            {
                Ba.SenderPort = 0;
            }

            Ba.Operator = textBox5.Text; 

            Ba.ReadBank(Ba.BankId);

            if (Ba.RecordFound == true)
            {
                MessageBox.Show(" BANK ALREADY EXIST ");
                //return; 
            }

            Ba.InsertBank(Ba.BankId);

            WBankId = Ba.BankId; 

            if (ImgFileName == "")
            {
                // No Image by Browsing 
            }
            else
            {
                byte[] img = null;
                FileStream fs = new FileStream(ImgFileName, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                img = br.ReadBytes((int)fs.Length);

                Ba.ReadBank(WBankId);
                Ba.Logo = img;
                Ba.UpdateBank(WBankId);
            }

            // ==============Create Parameters and Errors Ids====================
            string BankA = "CRBAGRAA"; // This is the Model Bank 
            string BankB = Ba.BankId;
            // ==============CopyParameters and holidays from ModelBak========================
            // ======================To Added Bank if Prive =====================
            if (Ba.BankId == Ba.Operator) // When Operator is created 
                                  // If we have the situation of NoBank then parameters are taken from No Bank
            {
            //    bool Prive = Ba.Prive;
                Gp.CopyParameters(BankA, BankB);

                RRDMHolidays Ch = new RRDMHolidays();

                // Create Holidays 

                Ch.CopyHolidays(BankA, BankB, DateTime.Now.Year);

            }
            // =========================End======================================
            // ==============CopyErrorsId from ModelBak==========================
            // ======================To Added Bank Errors Ids====================
            RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();
            Ec.CopyErrorIds(BankA, BankB, textBox5.Text);
            // ==================================================================

            if (Ba.BankId == Ba.Operator)
            {
                MessageBox.Show("A new Bank has been created, With new Parameters and Error Ids sets."); 
            }
            if (Ba.BankId != Ba.Operator)
            {
                MessageBox.Show("A new Bank has been created, With Error Ids sets. Parameters are from: " +  WOperator );
            } 

            textBoxMsgBoard.Text = " A new Bank has been created ";

            button2.Enabled = false;
        }
        // DELETE 
        private void button3_Click(object sender, EventArgs e)
        {
           
            RRDMAtmsClass Ac = new RRDMAtmsClass();
            
            RRDMUsersAccessToAtms Us = new RRDMUsersAccessToAtms();

            Ba.DeleteBankEntry(WBankId);

            this.Dispose(); 
            //MessageBox.Show("Future Development for checking if open ATMs and users =" + 
            //                               "not allowed to delete and also delete parameters and errors Ids ");
            //return; 
            /*
            if (MessageBox.Show("Warning: Do you want to delete bank?", "Message",MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                             == DialogResult.Yes)
            {
                Ba.DeleteBankEntry(Ba.BankSwiftId);
                textBoxMsgBoard.Text = " The Bank" + Ba.BankSwiftId + " has been deleted "; 
            }
            else
            {
            }
             */
         
        }

//Finish 
        private void button4_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }
// 
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox8.Text = textBox1.Text; 
        }
//Show Settlement Acccount 
        private void checkBoxSettlementBank_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxSettlementBank.Checked == true)
            {
                if (LoadingCycle == false)
                {
                    //Validate that no other Bank is a Settlement Bank

                    Ba.ReadSettlementBank(WOperator);
                    if (Ba.RecordFound == true)
                    {
                        MessageBox.Show("Bank Id : " + Ba.BankId + " Is the current Settlement Bank. UNDO it first.");
                        checkBoxSettlementBank.Checked = false; 
                        //return; 
                    }
                }

                label18.Show();
                label19.Show();
                textBox9.Show();
            }
            else
            {
                label18.Hide();
                label19.Hide();
                textBox9.Text = "";
                textBox9.Hide();
            }
        }
        //Browse for LOGO 

        string ImgFileName;
        string ImgBrowseDir = ConfigurationManager.AppSettings["BrowseImageDir"];
        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.InitialDirectory = ImgBrowseDir;
                //dlg.Filter = "JPG Files (*.jpg)|*.jpg|BMP Files (*.bmp)|*.bmp|GIF Files (*.gif)|*.gif";
                dlg.Filter = "PNG Files (*.png)|*.png|GIF Files (*.gif)|*.gif";
                dlg.Title = "Select an Image";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    ImgFileName = dlg.FileName.ToString();
                    pictureBox.ImageLocation = ImgFileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            
        }
    }
}
