using System;
using System.Windows.Forms;
using RRDM4ATMs;

//multilingual
namespace RRDM4ATMsWin
{
    public partial class Form3_PreInv_IST : Form
    {
        string WStringInput;
        int WIntInput;
        int UniqueIdType;
        string WFunction;
        string WIncludeNotMatchedYet;
        // "1" = Include
        // "" = Not Include

        RRDMEncryptPasswordOrField En = new RRDMEncryptPasswordOrField();

        RRDMAtmsClass Ac = new RRDMAtmsClass();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        Form80b NForm80b;
        Form80b_IST NForm80b_IST;

        string WSignedId;
        int WSignRecordNo;
        string WSecLevel;
        int WRMCycle; 
        string WOperator;

        public Form3_PreInv_IST(string InSignedId, int InSignRecordNo, string InSecLevel, string InOperator, int InRMCycle)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WSecLevel = InSecLevel;
            WRMCycle = InRMCycle; 
            WOperator = InOperator;

            InitializeComponent();

            // Set Working Date 

            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;
            pictureBox2.BackgroundImage = appResImg.imageBranch;
            pictureBox3.BackgroundImage = appResImg.CallCentre1;
            pictureBox4.BackgroundImage = appResImg.BackOffice;

            UserId.Text = InSignedId;

            dateTimePicker2.Value = new DateTime(2014, 02, 01);

            comboBoxPOSCateg.Items.Add("BDC272");
            comboBoxPOSCateg.Items.Add("BDC273");
            comboBoxPOSCateg.Items.Add("BDC231");
            comboBoxPOSCateg.Items.Add("BDC233");

            comboBoxPOSCateg.Text = "BDC272";

            //if (Environment.MachineName == "RRDM-PANICOS")
            //{
            //    // OK pass through 
            //}
            //else
            //{
            //    MessageBox.Show("To Be Delelop");
            //    // 
            //}

            //  radioButton_IST.Checked = true; 
        }
        // Finish 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        // Go to next 
        string WRRNumber;
        private void buttonNext_Click(object sender, EventArgs e)
        {
            if (radioButtonCard.Checked == false & radioButtonAccount.Checked == false
                 & radioButtonTraceNo.Checked == false & radioButtonRRNumber.Checked == false
                 & radioButtonATM.Checked == false & radioButtonCategoryProcessed.Checked == false
                 & radioButtonCategoryNonProcessed.Checked == false
                 & radioButtonEncryptedCard.Checked == false
                 & radioButtonForceMatch.Checked == false
                 )
            {
                MessageBox.Show("Please select button");
                return;
            }

            if (radioButtonCategoryProcessed.Checked == true)
            {
                // Check Dates
                //(futurDate - TodayDate).TotalDays;
                if ((dateTimePicker2.Value-dateTimePicker1.Value).TotalDays > 100)
                {
                    MessageBox.Show("For the processed only range of two days is allowed");
                    return; 
                }
            }

            WFunction = "Investigation";

            if (radioButtonCard.Checked == true
                || radioButtonEncryptedCard.Checked == true
                || radioButtonAccount.Checked == true
                || radioButtonATM.Checked == true
                || radioButtonCategoryProcessed.Checked == true
                || radioButtonCategoryNonProcessed.Checked == true
                || radioButtonForceMatch.Checked == true
                )
            {

                if (textBoxInputField.Text == "" & radioButtonCategoryProcessed.Checked == false)
                {
                    MessageBox.Show("Please input field");
                    return;
                }

                WStringInput = textBoxInputField.Text;

                if (radioButtonRRNumber.Checked == true) WStringInput = textBoxInputedTrace.Text;

                if (radioButtonEncryptedCard.Checked == true)
                {
                    UniqueIdType = 1; // Card Encrypted
                }

                if (radioButtonCard.Checked == true)
                {
                    UniqueIdType = 2; // Card
                }

                if (radioButtonAccount.Checked == true)
                {
                    if (RRDMInputValidationRoutines.IsAlfaNumeric(WStringInput))
                    {
                        //   No Problem 
                    }
                    else
                    {
                        MessageBox.Show("invalid Account Number");
                        return;
                    }
                    UniqueIdType = 3; // Account
                }

                if (radioButtonATM.Checked == true)
                {
                    RRDMAtmsClass Ac = new RRDMAtmsClass();

                    Ac.ReadAtm(textBoxInputField.Text);
                    if (Ac.RecordFound == false)
                    {
                        MessageBox.Show("invalid ATM Number");
                        return;
                    }
                    UniqueIdType = 6; // ATMNo
                }

                if (radioButtonCategoryProcessed.Checked == true)
                {
                    //textBoxInputField.Text = "";
                    WStringInput = comboBoxPOSCateg.Text; 
                    UniqueIdType = 7; // 
                }

                if (radioButtonCategoryNonProcessed.Checked == true)
                {
                    //textBoxInputField.Text = "";
                    WStringInput = comboBoxPOSCateg.Text;
                    UniqueIdType = 8; // 
                }

                if (radioButtonForceMatch.Checked == true)
                {
                    //textBoxInputField.Text = "";
                    if (comboBoxPOSCateg.Text != "BDC272")
                    {
                        MessageBox.Show("This selection is only for Category BDC272");
                        return; 
                    }
                    WStringInput = comboBoxPOSCateg.Text;
                    UniqueIdType = 9; // 
                }


                NForm80b_IST = new Form80b_IST(WSignedId, WSignRecordNo, WOperator, dateTimePicker1.Value, dateTimePicker2.Value, textBoxAtmNo.Text, "", WRMCycle,
                                                 WStringInput, 0, UniqueIdType, WFunction, WIncludeNotMatchedYet, 0);

                //(string InSignedId, int InSignRecordNo, string InOperator,
                //                                 DateTime InDateTimeA,
                //                                 DateTime InDateTimeB,
                //                                 string InAtmNo, string InCategoryId, int InRMCycleNo,
                //                                 string InStringUniqueId, int InIntUniqueId, int InUniqueIdType,
                //                                 string InFunction, string InIncludeNotMatchedYet, int InReplCycle)

                NForm80b_IST.ShowDialog();

            }
            if (radioButtonTraceNo.Checked == true || radioButtonRRNumber.Checked == true)
            {
                if (radioButtonTraceNo.Checked == true)
                {
                    if (int.TryParse(textBoxInputedTrace.Text, out WIntInput))
                    {

                    }
                    else
                    {
                        MessageBox.Show(textBoxInputedTrace.Text, "Please enter a valid number!");

                        return;
                    }

                    UniqueIdType = 4; // TraceNo

                }

                if (radioButtonRRNumber.Checked == true)
                {
                    if (RRDMInputValidationRoutines.IsAlfaNumeric(textBoxInputedTrace.Text))
                    {
                        //   No Problem 
                    }
                    else
                    {
                        MessageBox.Show("invalid RRNumber ");
                        return;
                    }
                    WRRNumber = textBoxInputedTrace.Text;
                    UniqueIdType = 5; // RRNumber
                }

                if (textBoxAtmNo.Text == "")
                {
                    MessageBox.Show("Please input AtmNo and Date");
                    return;
                }

                //Ac.ReadAtm(textBoxAtmNo.Text);
                //if (Ac.RecordFound == true)
                //{
                //    // ok 
                //}
                //else
                //{
                //    MessageBox.Show("Not such ATM");
                //    return;
                //}


                if (radioButtonTraceNo.Checked == true)
                {

                    NForm80b_IST = new Form80b_IST(WSignedId, WSignRecordNo, WOperator,
                      dateTimePicker3.Value.AddDays(-1), dateTimePicker3.Value.AddDays(1),
                      textBoxAtmNo.Text, "", WRMCycle, "", WIntInput, UniqueIdType, WFunction, WIncludeNotMatchedYet, 0);
                    NForm80b_IST.ShowDialog();

                }
                if (radioButtonRRNumber.Checked == true)
                {
                    // UniqueIdType = 13
                    // WFunction = "Investigation"
                    // WIncludeNotMatchedYet = true

                    NForm80b_IST = new Form80b_IST(WSignedId, WSignRecordNo, WOperator,
                                      dateTimePicker3.Value.AddDays(-1), dateTimePicker3.Value.AddDays(1),
                                      textBoxAtmNo.Text, "", WRMCycle, WRRNumber, 0, UniqueIdType, WFunction, WIncludeNotMatchedYet, 0);
                    NForm80b_IST.ShowDialog();

                }

            }
        }
        // Card Number 
        private void radioButtonCard_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonCard.Checked == true)
            {
                labelInput.Text = "Mask Card No";
                textBoxInputField.Text = "666666******4444";

                labelCateg.Hide();
                comboBoxPOSCateg.Hide(); 

                //buttonEncrypt.Show();
                //label4.Show();
                //label5.Show();
                panel3.Hide();
                panel4.Show();
                labelInput.Show();
                textBoxInputField.Show();
                //checkBoxMatchingNotDone.Checked = true;
                dateTimePicker1.Value = DateTime.Now.Date;
                dateTimePicker2.Value = DateTime.Now.Date;
            }
            else
            {
               // buttonEncrypt.Hide();
                //label4.Hide();
                //label5.Hide();
            }

        }
        // Account 
        private void radioButtonAccount_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonAccount.Checked == true)
            {
                labelInput.Text = "Account No";
                textBoxInputField.Text = "04135010025929";
                labelCateg.Hide();
                comboBoxPOSCateg.Hide();
                panel3.Hide();
                panel4.Show();
                labelInput.Show();
                textBoxInputField.Show();
                //checkBoxMatchingNotDone.Checked = true;
                dateTimePicker1.Value = DateTime.Now.Date;
                dateTimePicker2.Value = DateTime.Now.Date;
            }
        }

        // ATM Trace Number 
        private void radioButtonTraceNo_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonTraceNo.Checked == true)
            {
                labelInput.Text = "Trace Number";
                textBoxInputedTrace.Text = "999999";
                textBoxAtmNo.Text = "00000508";
                label10.Show();
                label10.Text = "ATM No";
                textBoxAtmNo.Show();
                panel3.Show();
                panel4.Hide();

                label11.Show();
                dateTimePicker3.Show();
                //checkBoxMatchingNotDone.Checked = true;
                dateTimePicker3.Value = new DateTime(2014, 02, 12);
            }

        }

        // RR Number
        private void radioButtonRRNumber_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonRRNumber.Checked == true)
            {

                labelInput.Text = "RR Number";
                textBoxInputedTrace.Text = "ABCDE";
                textBoxAtmNo.Text = "BDC210";
                label10.Show();
                label10.Text = "Category";
                textBoxAtmNo.Show();
                panel3.Show();
                panel4.Hide();
                //checkBoxMatchingNotDone.Checked = true;
                label11.Hide();
                dateTimePicker3.Hide();
                // dateTimePicker3.Value = new DateTime(2019, 01, 01);
            }
        }

        // ATM
        private void radioButtonATM_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonATM.Checked == true)
            {
                labelInput.Text = "ATM No";
                textBoxInputField.Text = "AB104";
                if (WOperator == "BCAIEGCX")
                {
                    textBoxInputField.Text = "00000504";
                }

                panel4.Show();
                labelInput.Show();
                textBoxInputField.Show();
                panel3.Hide();
                labelCateg.Hide();
                comboBoxPOSCateg.Hide(); 


                //checkBoxMatchingNotDone.Checked = false;

                dateTimePicker1.Value = new DateTime(2013, 02, 12);
                dateTimePicker2.Value = DateTime.Now.Date;
            }
        }
        // Show Encrypted
        private void buttonEncrypt_Click(object sender, EventArgs e)
        {

            string WCardNo = En.EncryptField(textBoxInputField.Text);

            MessageBox.Show("The Encrypted Card Is: " + Environment.NewLine
                            + WCardNo);
        }

        // Category Processed
        private void radioButtonCategoryProcessed_CheckedChanged(object sender, EventArgs e)
        {

            if (radioButtonCategoryProcessed.Checked == true)
            {

                labelInput.Hide();
                textBoxInputField.Hide();

                labelCateg.Show();
                comboBoxPOSCateg.Show();

                labelCateg.Text = "Category";

                panel4.Show();

                panel3.Hide();

                //checkBoxMatchingNotDone.Checked = false;

                dateTimePicker1.Value = DateTime.Now.Date;
                dateTimePicker2.Value = DateTime.Now.Date;
            }
            else
            {
                comboBoxPOSCateg.Hide();
            }
        }
        // Category Non Processed

        private void radioButtonCategoryNonProcessed_CheckedChanged(object sender, EventArgs e)
        {
            // Leave it here 
        }

        private void radioButtonCategoryNonProcessed_CheckedChanged_1(object sender, EventArgs e)
        {
            if (radioButtonCategoryNonProcessed.Checked == true)
            {

                labelInput.Hide();
                textBoxInputField.Hide();

                labelCateg.Show();
                comboBoxPOSCateg.Show();

                labelCateg.Text = "Category";

                panel4.Show();

                panel3.Hide();

                //checkBoxMatchingNotDone.Checked = false;

                dateTimePicker1.Value = DateTime.Now.Date;
                dateTimePicker2.Value = DateTime.Now.Date;
            }
        }
        // Encrypted Card
        private void radioButtonEncryptedCard_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonEncryptedCard.Checked == true)
            {
                labelInput.Show();
                textBoxInputField.Show();
                labelInput.Text = "Encrypted";
                textBoxInputField.Text = "ab666666jk444d1w";

                labelCateg.Hide();
                comboBoxPOSCateg.Hide();

                //buttonEncrypt.Show();
                //label4.Show();
                //label5.Show();
                panel3.Hide();
                panel4.Show();
                
                //checkBoxMatchingNotDone.Checked = true;
                dateTimePicker1.Value = DateTime.Now.Date;
                dateTimePicker2.Value = DateTime.Now.Date;
            }
        }

        private void radioButtonForceMatch_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonForceMatch.Checked == true)
            {
                labelInput.Hide();
                textBoxInputField.Hide();

                labelCateg.Show();
                comboBoxPOSCateg.Show();

                labelCateg.Text = "ForceMatched";

                panel4.Show();

                panel3.Hide();

                //checkBoxMatchingNotDone.Checked = false;

                dateTimePicker1.Value = DateTime.Now.Date;
                dateTimePicker2.Value = DateTime.Now.Date;
            }
            
        }
    }
}
