using System;
using System.Text;
using System.Windows.Forms;

//multilingual

using RRDMEncrypt;

using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form41 : Form
    {
        string WPassWord; 

        RRDMUsersRecords Us = new RRDMUsersRecords(); // Make class availble 
        RRDMGasParameters Gp = new RRDMGasParameters(); 

        string WSignedId;
        string WOperator;

        public Form41(string InSignedId, string InOperator)
        {
            WSignedId = InSignedId;
            WOperator = InOperator;

            InitializeComponent();

            textBox1.Text = InSignedId; 

            pictureBox1.BackgroundImage = appResImg.logo2;
        }
        // Change Password 
        private void button1_Click(object sender, EventArgs e)
        {
            // Make validation of input data 
            if (textBox3.Text == "" || textBox4.Text == "")
            {
                MessageBox.Show("Please fill Data");
                return;
            }

           
                string InputPassword = textBox3.Text;
                // Minimum Length Of Password
                Gp.ReadParametersSpecificId(WOperator, "451", "7", "", "");
                int MinLengthOfPassword = ((int)Gp.Amount);

                if (InputPassword.Length < MinLengthOfPassword)
                {
                    MessageBox.Show("Password is less than allow length " + Environment.NewLine
                                     + "Allowed length is.." + MinLengthOfPassword.ToString()
                                        );
                    return;
                }



                string str = InputPassword;
                char[] aPassword = str.ToCharArray();
                char[] aDigits = new char[aPassword.Length];
                char[] aCharacters = new char[aPassword.Length];
                char[] aSpecialChars = new char[aPassword.Length];

                int iDigitsCounter = 0;
                int iCharacterCounter = 0;
                int iSpecialCharsCounter = 0;

                for (int i = 0; i < aPassword.Length; i++)
                {
                    if (!Char.IsLetterOrDigit(aPassword[i]))
                    {
                        aSpecialChars[iSpecialCharsCounter] = aPassword[i];
                        iSpecialCharsCounter++;
                    }

                    if (Char.IsLetter(aPassword[i]))
                    {
                        aCharacters[iCharacterCounter] = aPassword[i];
                        iCharacterCounter++;
                    }

                    if (Char.IsDigit(aPassword[i]))
                    {
                        aDigits[iDigitsCounter] = aPassword[i];
                        iDigitsCounter++;
                    }


                }

                //Array.Resize(ref aDigits, iDigitsCounter);
                //Array.Resize(ref aCharacters, iCharacterCounter);
                //Array.Resize(ref aSpecialChars, iSpecialCharsCounter);


                // Minimum number of Alpha Characters
                Gp.ReadParametersSpecificId(WOperator, "451", "4", "", "");
                int MinNumberOfAlphanumeric = ((int)Gp.Amount);

                if (iCharacterCounter < MinNumberOfAlphanumeric)
                {
                    MessageBox.Show("Invalid Password." + Environment.NewLine
                                    + "You must have at least.." + MinNumberOfAlphanumeric.ToString() + "..Alpha"
                                       );
                    return;
                }

                // Minimum number of Special Characters 
                Gp.ReadParametersSpecificId(WOperator, "451", "5", "", "");
                int MinNumberOfSpecialCharacters = ((int)Gp.Amount);

                if (iSpecialCharsCounter < MinNumberOfSpecialCharacters)
                {
                    MessageBox.Show("Invalid Password." + Environment.NewLine
                                    + "You must have at least.." + MinNumberOfSpecialCharacters.ToString() + "..Special Characters"
                                       );
                    return;
                }

                // Name In Password
                Gp.ReadParametersSpecificId(WOperator, "451", "6", "", "");
                int NameInPassword = ((int)Gp.Amount);

                if (InputPassword.Contains(textBox1.Text))
                {
                    MessageBox.Show("Password contains user id" + Environment.NewLine
                                  + "This is not allowed."
                                     );
                    return;
                }

            


            if (textBox3.Text != textBox4.Text)
            {
                MessageBox.Show("Data do not match");
                return;
            }
// =======================Update New Password ==========
            Us.ReadUsersRecord(WSignedId);

            ////Password parameters
            ////
            //string passPhrase = "Pas5pr@seAthos11Aramis11Porthos11";
            //string initVectorOrg = "@1B09FgHH8#"; // must be up to 16 bytes
            //byte[] initVector = Encoding.ASCII.GetBytes(initVectorOrg);
            //string initVectorTable = string.Empty;
            //string EncryptedString = string.Empty;
            //string DecryptedString = string.Empty;

            //RRDM_Encryption cEncryption = new RRDM_Encryption(passPhrase, Convert.ToBase64String(initVector));
            //this.WPassWord = cEncryption.Encrypt(this.textBox3.Text);
            // Us.PassWord = WPassWord;


            RRDMEncryptPasswordOrField En = new RRDMEncryptPasswordOrField();
            string hashPwd1 = RRDM_Encryption.ComputeHash(this.textBox3.Text, "SHA256", null);
            WPassWord = En.EncryptField(hashPwd1);

            Us.PassWord = WPassWord;
            

            Gp.ReadParametersSpecificId(WOperator, "451", "1", "", "");
            if (Gp.RecordFound == false)
            {
                MessageBox.Show("Parameter Not Updated");
                return; 
            }
            int Temp = ((int)Gp.Amount);

            Us.DtChanged = DateTime.Now;
            Us.DtToBeChanged = DateTime.Now.AddDays(Temp);
            Us.ForceChangePassword = false;

            Us.UpdateUser(WSignedId);

            MessageBox.Show("New Password has been updated");

            this.Dispose(); 
        }
// Cancel
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }
    }
}
