using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RRDM4ATMs; 
using System.Collections;
using System.IO;

namespace RRDM4ATMsWin
{
    public partial class Form44 : Form
    {
        

        RRDMBanks Ba = new RRDMBanks();

        RRDMGasParameters Gp = new RRDMGasParameters();
        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();

  
        string WSignedId;
        int WSignRecordNo;
        string WOperator; 
   
        string WChosenBank;
        int WFunctionNo;

        public Form44(string InSignedId, int InSignRecordNo, string InOperator, string InChosenBank, int InFunctionNo)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
      
            WChosenBank = InChosenBank;
            WFunctionNo = InFunctionNo; // 1: Update , 2: Add, 3 : Delete 

            InitializeComponent();

            textBoxSenderPassword.PasswordChar = '*';

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;

            textBox5.Text = WOperator; 

            // FIRST CASSETTE

            Gp.ParamId = "201"; // Currencies first Cassette 
            comboBox1.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBox1.DisplayMember = "DisplayValue"; 

            // Update or DELETE 
            if (WFunctionNo == 1 || WFunctionNo == 3 )
            {
                Ba.ReadBank(WChosenBank);

                textBox1.Text = Ba.BankSwiftId;

                textBox6.Text = Ba.ActiveDirectoryDM;
                textBox7.Text = Ba.AdGroup;
    
                textBox2.Text = Ba.BankName;
                textBox3.Text = Ba.BankCountry;
                textBox4.Text = Ba.GroupName;
                textBox5.Text = Ba.Operator; 
     
                comboBox1.Text = Ba.BasicCurName;

                textBoxSenderEmail.Text = Ba.SenderEmail;
                textBoxSenderUserName.Text = Ba.SenderUserName ;
                textBoxSenderPassword.Text = Ba.SenderPassword ;
                textBoxSenderSmtpClient.Text = Ba.SenderSmtpClient ;
                textBoxSenderPort.Text = Ba.SenderPort.ToString(); 

                checkBox2.Checked = Ba.UsingGAS;

                //THIS IS WORKING 
                MemoryStream ms = new MemoryStream(Ba.Logo);
                System.Drawing.Image image = Image.FromStream(ms);

                pictureBox2.Image = image; 

                if (WFunctionNo == 1)
                {
                    labelStep1.Text = " Update Bank ";
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
                textBox1.Text = WChosenBank; // When seed bank is created 
                textBoxMsgBoard.Text = "Input data for the new Bank or Operator. Bank id = SwiftNo";
                button2.Show();
                button1.Hide();
                button3.Hide(); 
            }
        }
        //
        // UPDATE BANK
        //
        private void button1_Click(object sender, EventArgs e)
        {
            Ba.ActiveDirectoryDM = textBox6.Text;
            Ba.AdGroup = textBox7.Text;
          
            Ba.BankName=textBox2.Text;
            Ba.BankCountry=textBox3.Text;

            Ba.GroupName = textBox4.Text;

            Ba.BasicCurName = comboBox1.Text;

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
                    return; 
                }
            }

            Ba.SenderEmail = textBoxSenderEmail.Text;
            Ba.SenderUserName = textBoxSenderUserName.Text;
            Ba.SenderPassword = textBoxSenderPassword.Text;
            Ba.SenderSmtpClient = textBoxSenderSmtpClient.Text;

            if (int.TryParse(textBoxSenderPort.Text, out Ba.SenderPort))
            {
            }
            else
            {
                Ba.SenderPort = 0; 
            }

            Ba.UsingGAS = checkBox2.Checked;
//THIS IS WORKING 
         //   MemoryStream ms = new MemoryStream(Ba.Logo);
        //    System.Drawing.Image image = Image.FromStream(ms);

        //    pictureBox1.Image = image; 


            Bitmap memoryImage=new System.Drawing.Bitmap(pictureBox2.Image.Width,pictureBox2.Image.Height);
            pictureBox2.DrawToBitmap(memoryImage, pictureBox2.ClientRectangle);

            Ba.Logo = ImageToByte2(memoryImage);

            Bitmap ReportLogo = BytesToBitmap( Ba.Logo);

            Ba.UpdateBank(Ba.BankSwiftId);

            MessageBox.Show("The Bank data has been updated "); 

            textBoxMsgBoard.Text = " The Bank data has been updated "; 

        }

        //
        // ADD A NEW BANK 
        //
        private void button2_Click(object sender, EventArgs e)
        {         
            Ba.BankSwiftId = textBox1.Text;

            Ba.ActiveDirectoryDM = textBox6.Text;
            Ba.AdGroup = textBox7.Text;
      
            Ba.BankName = textBox2.Text;
            Ba.BankCountry = textBox3.Text;

            Ba.GroupName = textBox4.Text; 

            Ba.BasicCurName = comboBox1.Text; 
            Ba.DtTmCreated = DateTime.Now; 
            Ba.UsingGAS = checkBox2.Checked;

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
                    return;
                }
            }

            Ba.SenderEmail = textBoxSenderEmail.Text;
            Ba.SenderUserName = textBoxSenderUserName.Text;
            Ba.SenderPassword = textBoxSenderPassword.Text;
            Ba.SenderSmtpClient = textBoxSenderSmtpClient.Text;

            if (int.TryParse(textBoxSenderPort.Text, out Ba.SenderPort))
            {
            }
            else
            {
                Ba.SenderPort = 0;
            }

            Ba.Operator = WOperator; 

            Ba.ReadBank(Ba.BankSwiftId);

            if (Ba.RecordFound == true)
            {
                MessageBox.Show(" BANK ALREADY EXIST ");
                return; 
            }

            Bitmap memoryImage = new System.Drawing.Bitmap(pictureBox2.Image.Width, pictureBox2.Image.Height);
            pictureBox2.DrawToBitmap(memoryImage, pictureBox2.ClientRectangle);

            Ba.Logo = ImageToByte2(memoryImage);

            Ba.InsertBank(Ba.BankSwiftId);

            // ==============Create Parameters and Errors Ids====================
            string BankA = "ModelBak";
            string BankB = Ba.BankSwiftId;
            // ==============CopyParameters and holidays from ModelBak========================
            // ======================To Added Bank if Prive =====================
            if (Ba.BankSwiftId == Ba.Operator) // When Operator is created 
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
            Ec.CopyErrorIds(BankA, BankB);
            // ==================================================================

            if (Ba.BankSwiftId == Ba.Operator)
            {
                MessageBox.Show("A new Bank has been created, With new Parameters and Error Ids sets."); 
            }
            if (Ba.BankSwiftId != Ba.Operator)
            {
                MessageBox.Show("A new Bank has been created, With Error Ids sets. Parameters are from: " +  WOperator );
            } 

            textBoxMsgBoard.Text = " A new Bank has been created ";

            button2.Enabled = false;
        }
        // DELETE 
        private void button3_Click(object sender, EventArgs e)
        {
            /*
            AtmsClass Ac = new AtmsClass();
            Ac.ReadAtm 
            UsersAccessToAtms Us = new UsersAccessToAtms();
             */

            MessageBox.Show("Future Development for checking if open ATMs and users =" + 
                                           "not allowed to delete and also delete parameters and errors Ids ");
            return; 
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

        // Image to Byte 
        private static byte[] ImageToByte2(System.Drawing.Bitmap img)
        {
            byte[] byteArray = new byte[0];
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                stream.Close();

                byteArray = stream.ToArray();
            }
            return byteArray;
        }
        // Byte to Bitmap 
        public static Bitmap BytesToBitmap(byte[] byteArray)
        {
            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                Bitmap img = (Bitmap)Image.FromStream(ms);
                return img;
            }
        }

        private void buttonLogo_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";
           
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox2.Image = Image.FromFile(openFileDialog1.FileName);

            }  
        }

    }
}
