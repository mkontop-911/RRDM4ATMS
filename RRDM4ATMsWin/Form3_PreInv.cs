using System;
using System.Globalization;
using System.Windows.Forms;
using RRDM4ATMs;

//multilingual

namespace RRDM4ATMsWin
{
    public partial class Form3_PreInv : Form
    {
        string WStringInput;
        int WIntInput;
        int UniqueIdType;
        string WFunction;
        string WIncludeNotMatchedYet;
        DateTime HST_DATE;

        // "1" = Include
        // "" = Not Include

        RRDMEncryptPasswordOrField En = new RRDMEncryptPasswordOrField();

        RRDMAtmsClass Ac = new RRDMAtmsClass();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        Form80b NForm80b;
        Form80b_ROM NForm80b_ROM;
        Form80b_IST NForm80b_IST;

        bool IsRomaniaVersion; 

        string WSignedId;
        int WSignRecordNo;
        string WSecLevel;
        string WOperator;

        public Form3_PreInv(string InSignedId, int InSignRecordNo, string InSecLevel, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;

            InitializeComponent();

            // Set Working Date 
            RRDMGasParameters Gp = new RRDMGasParameters();

            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;
            pictureBox2.BackgroundImage = appResImg.imageBranch;
            pictureBox3.BackgroundImage = appResImg.CallCentre1;
            pictureBox4.BackgroundImage = appResImg.BackOffice;

            UserId.Text = InSignedId;

            dateTimePicker2.Value = new DateTime(2014, 02, 01);



            // Hide Capture cards 

            string ParamId = "719";
            string OccuranceId = "2"; // Capture cards

            Gp.ReadParameterByOccuranceId(ParamId, OccuranceId);

            if (Gp.RecordFound == true & Gp.OccuranceNm == "YES")
            {
                // Show Capture card 
            }
            else
            {
                // Hide 
                labelCapture.Hide();

                radioCaptureBr.Hide();
                radioCapturedBank.Hide();
                radioCapturedFullCard.Hide();
                radioCapturedATM.Hide();
            }

            //
            // Check if ROMANIA VERSION
            //

            ParamId = "951";
            OccuranceId = "1";
            //TEST
            IsRomaniaVersion = false;
            Gp.ReadParametersSpecificId(WOperator, ParamId, OccuranceId, "", "");

            if (Gp.RecordFound == true)
            {
                if (Gp.OccuranceNm == "YES") // ROMANIA 
                {
                    IsRomaniaVersion = true;
                }
                else
                {
                    IsRomaniaVersion = false;
                }

            }


            // Hide Disputes 

            ParamId = "719";
            OccuranceId = "3"; // Hide Disputes 

            Gp.ReadParameterByOccuranceId(ParamId, OccuranceId);

            if (Gp.RecordFound == true & Gp.OccuranceNm == "YES")
            {
                // Hide Disputes 
                label14.Hide();
                radioButtonBranchOrDept.Hide();
                radioButtonFullCard.Hide();
                radioButtonPhone.Hide(); 
            }
            else
            {
                
            }

            // FIND HISTORY DATE

            ParamId = "853";
            OccuranceId = "5"; // HST

            Gp.ReadParameterByOccuranceId(ParamId, OccuranceId);

            if (Gp.RecordFound == true & Gp.OccuranceNm != "")
            {

                if (DateTime.TryParseExact(Gp.OccuranceNm, "yyyy-MM-dd", CultureInfo.InvariantCulture
                              , System.Globalization.DateTimeStyles.None, out HST_DATE))
                {
                    textBoxHST_DATE.Text = HST_DATE.ToShortDateString();

                    textBoxHST_DATE.Show();
                    label_HST.Show();

                }
            }
            else
            {
                textBoxHST_DATE.Hide();
                label_HST.Hide();
            }

            //radioButtonMaster.Checked = true; 
        }
        // Finish 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        // Go to next 
        string WRRNumber;
        int WDispMode;
        private void buttonNext_Click(object sender, EventArgs e)
        {
            //if (radioCaptureBr.Checked == true)
            //    if (radioCapturedBank.Checked == true)
            //        if (radioCapturedFullCard.Checked == true)
            //            if (radioCapturedATM.Checked == true)
            //radioButtonATM_Discrepancies
            if (radioButtonBranchOrDept.Checked == false & radioButtonCard.Checked == false
                & radioButtonAccount.Checked == false
 & radioButtonTraceNo.Checked == false & radioButtonRRNumber.Checked == false
 & radioButtonATM.Checked == false & radioButtonATM_Discrepancies.Checked == false
 & radioButtonPresenterErrors.Checked == false & radioButtonReversals.Checked == false
& radioButtonFullCard.Checked == false & radioButtonPhone.Checked == false 
& radioButtonRevDep.Checked == false & radioButtonByAction.Checked == false
 & radioCaptureBr.Checked == false & radioCapturedBank.Checked == false
  & radioCapturedFullCard.Checked == false & radioCapturedATM.Checked == false
 
    )
            {
                MessageBox.Show("Please select button");
                return;
            }

            WFunction = "Investigation";

            if (radioButtonBranchOrDept.Checked == true
                || radioButtonCard.Checked == true
                || radioButtonAccount.Checked == true
                || radioButtonATM.Checked == true
                || radioButtonATM_Discrepancies.Checked == true 
                || radioButtonPresenterErrors.Checked == true
                || radioButtonReversals.Checked == true
                || radioButtonRevDep.Checked == true
                || radioButtonByAction.Checked == true
                || radioButtonFullCard.Checked == true
                || radioButtonPhone.Checked == true
                || radioCaptureBr.Checked == true
                || radioCapturedBank.Checked == true
                || radioCapturedFullCard.Checked == true
                || radioCapturedATM.Checked == true
                )
            {
                if (radioButtonReversals.Checked == true)
                {
                    int Mode = 1;
                    string FileId = "REVERSALs_PAIRs";
                    Form78d_FileRecords_Reversals NForm78d_FileRecords_Reversals;
                    NForm78d_FileRecords_Reversals = new Form78d_FileRecords_Reversals(WOperator, WSignedId, FileId, dateTimePicker1.Value, dateTimePicker2.Value, Mode);
                    NForm78d_FileRecords_Reversals.ShowDialog();

                    return;
                }
                // REVERSAL DEPOSITS
                if (radioButtonRevDep.Checked == true)
                {
                    int Mode = 4;
                    string FileId = "REVERSALs_PAIRs";
                    Form78d_FileRecords_Reversals NForm78d_FileRecords_Reversals;
                    NForm78d_FileRecords_Reversals = new Form78d_FileRecords_Reversals
                        (WOperator, WSignedId, FileId, dateTimePicker1.Value, dateTimePicker2.Value, Mode);
                    NForm78d_FileRecords_Reversals.ShowDialog();

                    return;
                }

                if (textBoxInputField.Text == "" & radioButtonPresenterErrors.Checked == false)
                {
                    MessageBox.Show("Please input field");
                    return;
                }

                WStringInput = textBoxInputField.Text.Trim();

                // Show By Action
                if (radioButtonByAction.Checked == true)
                {
                    DateTime FromDate = dateTimePicker1.Value; 
                    DateTime ToDate = dateTimePicker2.Value;
                    if (dateTimePicker2.Value < dateTimePicker1.Value)
                    {
                        MessageBox.Show("Second date can not be less than first one");
                        return;
                    }
                    bool IsValid;
                    string allowableLetters = "0123456789',";
                    IsValid = FormatValid(WStringInput, allowableLetters); 

                    if (IsValid == true)
                    {
                        // Continue
                    }
                    else
                    {
                        // Message
                        MessageBox.Show("Invalid chardacters"+Environment.NewLine
                            + "Input in format eg '91','92'"
                            );
                        return; 
                    }
                    Form80b3 NForm80b3;

                    WFunction = "View";

                    int Type = 35; // Show actions by selection string 

                    string WCategoryId = WStringInput;

                    int TempJobCycleNumber = 0; 

                    NForm80b3 = new Form80b3(WSignedId, WSignRecordNo, WOperator, FromDate, ToDate, "", WCategoryId, TempJobCycleNumber, "", 0, Type, WFunction, "", 0, NullPastDate, "", "");
                    // NForm80b3.FormClosed += NForm80b3_FormClosed;
                    NForm80b3.ShowDialog();

                    return;
                }

               

                if (radioButtonRRNumber.Checked == true) WStringInput = textBoxInputedTrace.Text;

                if (radioButtonFullCard.Checked == true )
                {
                    if (RRDMInputValidationRoutines.IsAlfaNumeric(WStringInput))
                    {
                        //   No Problem 
                    }
                    else
                    {
                        MessageBox.Show("invalid Input");
                        return;
                    }
                }

                if (radioButtonBranchOrDept.Checked == true 
                                  || radioButtonPhone.Checked == true)
                {
                    if (RRDMInputValidationRoutines.IsNumeric(WStringInput))
                    {
                        //   No Problem 
                    }
                    else
                    {
                        MessageBox.Show("invalid Input");
                        return;
                    }
                }

                    if (radioButtonBranchOrDept.Checked == true || radioButtonFullCard.Checked == true
                || radioButtonPhone.Checked == true)
                {
                    if (radioButtonBranchOrDept.Checked == true) WDispMode = 14;  // You will show Branch Disputes 
                    if (radioButtonFullCard.Checked == true) WDispMode = 15; // You will show Full Card Disputes 
                    if (radioButtonPhone.Checked == true) WDispMode = 16; // You will show Phone Disputes 

                    Form3 NForm3;

                    NForm3 = new Form3(WSignedId, WSignRecordNo, WOperator,
                        WStringInput, dateTimePicker1.Value, dateTimePicker2.Value, WDispMode);
                    //   NForm3.FormClosed += NForm3_FormClosed;
                    NForm3.ShowDialog();
                    return;
                }
                //
                // CAPTURED CARDS
                //
                if (
                   radioCaptureBr.Checked == true
                   || radioCapturedBank.Checked == true
                   || radioCapturedFullCard.Checked == true
                   || radioCapturedATM.Checked == true)
                {
                    if (RRDMInputValidationRoutines.IsAlfaNumeric(WStringInput))
                    {
                        //   No Problem 
                    }
                    else
                    {
                        MessageBox.Show("invalid Input");
                        return;
                    }
                    if (radioCaptureBr.Checked == true) WDispMode = 21;  // You will show Branch Captured
                    if (radioCapturedBank.Checked == true) WDispMode = 22; // You will show Captured For Bank

                    if (radioCapturedATM.Checked == true) WDispMode = 23; // You will show Captured ATM 
                    if (radioCapturedFullCard.Checked == true) WDispMode = 24; // You will show Captued By Full Card

                    Form26 NForm26;

                    //Form26(string InSignedId, string InOperator, string InInputText
                    //    , DateTime InDateTmFrom
                    //    , DateTime InDateTmTo
                    //    , int InMode
                    //    )
                    // WDispMode = 23; // You will show Captured ATM 
                    NForm26 = new Form26(WSignedId, WOperator, WStringInput, dateTimePicker1.Value
                               , dateTimePicker2.Value, WDispMode);
                    //NForm26.FormClosed += NForm26_FormClosed;
                    NForm26.ShowDialog(); ;

                    //CAPTURED CARDS 

                    return;
                }

                if (radioButtonCard.Checked == true)
                {
                    if (RRDMInputValidationRoutines.IsAlfaNumeric(WStringInput))
                    {
                        //   No Problem 
                    }
                    else
                    {
                        MessageBox.Show("invalid Card");
                        return;
                    }

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

                if (radioButtonATM.Checked == true || radioButtonATM_Discrepancies.Checked == true)
                {
                    RRDMAtmsClass Ac = new RRDMAtmsClass();

                    Ac.ReadAtm(textBoxInputField.Text);
                    if (Ac.RecordFound == false)
                    {
                        MessageBox.Show("invalid ATM Number");
                        return;
                    }
                    if (radioButtonATM.Checked == true)
                    {
                        UniqueIdType = 6; // ATMNo
                    }

                    if (radioButtonATM_Discrepancies.Checked == true)
                    {
                        UniqueIdType = 7; // ATMNo Discrepancies 
                    }

                }

                if (radioButtonPresenterErrors.Checked == true)
                {
                    textBoxInputField.Text = "";
                    UniqueIdType = 14; // Presenter Error
                }


                if (checkBoxMatchingNotDone.Checked == true)
                {
                    WIncludeNotMatchedYet = "1";
                }
                else
                {
                    WIncludeNotMatchedYet = "";
                }
                // Check if they want to see IST
                if (checkBoxThroughIST_1.Checked == true)
                {
                    NForm80b_IST = new Form80b_IST(WSignedId, WSignRecordNo, WOperator, dateTimePicker1.Value, dateTimePicker2.Value, textBoxAtmNo.Text, "", 0,
                                                WStringInput, 0, UniqueIdType, WFunction, WIncludeNotMatchedYet, 0);
                    NForm80b_IST.ShowDialog();
                    return; 

                }
                // Else 
                if (IsRomaniaVersion== true)
                {
                    NForm80b_ROM = new Form80b_ROM(WSignedId, WSignRecordNo, WOperator, dateTimePicker1.Value.Date, dateTimePicker2.Value.Date, textBoxInputField.Text, "", 0,
                                                 WStringInput, 0, UniqueIdType, WFunction, WIncludeNotMatchedYet, 0);

                    NForm80b_ROM.ShowDialog();
                }
                else
                {
                    NForm80b = new Form80b(WSignedId, WSignRecordNo, WOperator, dateTimePicker1.Value.Date, dateTimePicker2.Value.Date, textBoxInputField.Text, "", 0,
                                                 WStringInput, 0, UniqueIdType, WFunction, WIncludeNotMatchedYet, 0);

                    NForm80b.ShowDialog();
                }
               


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

                    //if (textBoxInputedTrace.TextLength < 6)
                    //{
                    //    MessageBox.Show(textBoxInputedTrace.Text, "Please enter trace no less than 6 digits!");

                    //    return;
                    //}

                    UniqueIdType = 5; // TraceNo

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
                    UniqueIdType = 13; // RRNumber
                }


                if (textBoxAtmNo.Text == "")
                {
                    MessageBox.Show("Please input AtmNo and Date");
                    return;
                }

                Ac.ReadAtm(textBoxAtmNo.Text);
                if (Ac.RecordFound == true)
                {
                    // ok 
                }
                else
                {
                    MessageBox.Show("Not such ATM");
                    return;
                }

                if (checkBoxMatchingNotDone.Checked == true)
                {
                    WIncludeNotMatchedYet = "1";
                }
                else
                {
                    WIncludeNotMatchedYet = "";
                }
                if (radioButtonTraceNo.Checked == true)
                {
                    if (WOperator == "AUDBEGCA")
                    {

                    }
                    else
                    {
                         NForm80b = new Form80b(WSignedId, WSignRecordNo, WOperator,
                       dateTimePicker3.Value, dateTimePicker3.Value,
                       textBoxAtmNo.Text, "", 0, "", WIntInput, UniqueIdType, WFunction, WIncludeNotMatchedYet, 0);
                    NForm80b.ShowDialog();
                    }

                   


                }
                if (radioButtonRRNumber.Checked == true)
                {
                    // UniqueIdType = 13
                    // WFunction = "Investigation"
                    // WIncludeNotMatchedYet = true

                    NForm80b = new Form80b(WSignedId, WSignRecordNo, WOperator,
                                        dateTimePicker3.Value.AddDays(-1), dateTimePicker3.Value.AddDays(1),
                                        textBoxAtmNo.Text, "", 0, WRRNumber, 0, UniqueIdType, WFunction, WIncludeNotMatchedYet, 0);
                    NForm80b.ShowDialog();


                }

               


            }
        }
        // Card Number 
        private void radioButtonCard_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonCard.Checked == true)
            {
                labelInput.Text = "Card No";
                textBoxInputField.Text = "4375071234567892";

                buttonEncrypt.Show();
                label4.Show();
                label5.Show();
                panel3.Hide();
                panel4.Show();
                labelInput.Show();
                textBoxInputField.Show();
                checkBoxMatchingNotDone.Checked = true;
                dateTimePicker1.Value = DateTime.Now.Date;
                dateTimePicker2.Value = DateTime.Now.Date;
                checkBoxWithAmt.Checked = false;
                checkBoxThroughIST_1.Checked = false;
                checkBoxWithAmt.Hide();
                textBoxAmnt.Hide();
            }
            else
            {
                buttonEncrypt.Hide();
                label4.Hide();
                label5.Hide();
            }

        }
        // Masked Card
        private void radioButtonMaskCard_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonMaskCard.Checked == true)
            {
                labelInput.Text = "Mask Card No";
                textBoxInputField.Text = "437507******7892";

                // buttonEncrypt.Show();
                checkBoxWithAmt.Show();
                checkBoxWithAmt.Checked = false;
                checkBoxThroughIST_1.Checked = false; 
                textBoxAmnt.Hide();
                label4.Show();
                label5.Show();
                panel3.Hide();
                panel4.Show();
                labelInput.Show();
                textBoxInputField.Show();
                checkBoxMatchingNotDone.Checked = true;
                dateTimePicker1.Value = DateTime.Now.Date;
                dateTimePicker2.Value = DateTime.Now.Date;
            }
            else
            {
               // buttonEncrypt.Hide();
                label4.Hide();
                label5.Hide();
                checkBoxWithAmt.Checked = false;
                checkBoxThroughIST_1.Checked = false;
                checkBoxWithAmt.Hide();
                textBoxAmnt.Hide();
            }
        }
        // Account 
        private void radioButtonAccount_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonAccount.Checked == true)
            {
                labelInput.Text = "Account No";
                textBoxInputField.Text = "1234567892";
                panel3.Hide();
                panel4.Show();
                labelInput.Show();
                textBoxInputField.Show();
                checkBoxMatchingNotDone.Checked = true;
                dateTimePicker1.Value = DateTime.Now.Date;
                dateTimePicker2.Value = DateTime.Now.Date;
                checkBoxWithAmt.Checked = false;
                checkBoxThroughIST_1.Checked = false;
                checkBoxWithAmt.Hide();
                textBoxAmnt.Hide();
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
                textBoxAtmNo.Show();
                panel3.Show();
                panel4.Hide();

                label11.Show();
                dateTimePicker3.Show();
                checkBoxMatchingNotDone.Checked = true;
                dateTimePicker3.Value = DateTime.Now.Date;
                // dateTimePicker3.Value = new DateTime(2014, 02, 12);
                checkBoxWithAmt.Checked = false;
                checkBoxThroughIST_1.Checked = false;
                checkBoxWithAmt.Hide();
                textBoxAmnt.Hide();
            }

        }

        // RR Number
        private void radioButtonRRNumber_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonRRNumber.Checked == true)
            {

                labelInput.Text = "RR Number";
                textBoxInputedTrace.Text = "ABCDE";
                textBoxAtmNo.Text = "00000504";
                label10.Show();
                textBoxAtmNo.Show();
                panel3.Show();
                panel4.Hide();
                checkBoxMatchingNotDone.Checked = true;
                label11.Hide();
                dateTimePicker3.Hide();
                // dateTimePicker3.Value = new DateTime(2019, 01, 01);
                checkBoxWithAmt.Checked = false;
                checkBoxThroughIST_1.Checked = false;
                checkBoxWithAmt.Hide();
                textBoxAmnt.Hide();
            }
        }
        // ATM No


        // ATM
        private void radioButtonATM_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonATM.Checked == true)
            {
                labelInput.Text = "ATM No";
                textBoxInputField.Text = "RA209002";
                if (WOperator == "BCAIEGCX")
                {
                    textBoxInputField.Text = "00000504";
                }

                panel4.Show();
                labelInput.Show();
                textBoxInputField.Show();
                panel3.Hide();

                checkBoxThroughIST_1.Show();
                checkBoxThroughIST_1.Checked = false;

                checkBoxMatchingNotDone.Checked = false;

                dateTimePicker1.Value = DateTime.Now.Date;
                dateTimePicker2.Value = DateTime.Now.Date;

               // checkBoxWithAmt.Checked = false;
                checkBoxThroughIST_1.Checked = false;
              //  checkBoxWithAmt.Hide();
               // textBoxAmnt.Hide();
            }
            else
            {
                checkBoxThroughIST_1.Hide();
            }
        }
        // Show Encrypted
        private void buttonEncrypt_Click(object sender, EventArgs e)
        {

            string WCardNo = En.EncryptField(textBoxInputField.Text);

            MessageBox.Show("The Encrypted Card Is: " + Environment.NewLine
                            + WCardNo);
        }
        // RADIO button branch
        private void radioButtonBranchOrDept_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonBranchOrDept.Checked == true)
            {

                labelInput.Text = "User Branch/Dpt";
                textBoxInputField.Text = "";

                buttonEncrypt.Show();
                label4.Show();
                label5.Show();
                panel3.Hide();
                panel4.Show();
                labelInput.Show();
                textBoxInputField.Show();
                checkBoxMatchingNotDone.Checked = true;
                dateTimePicker1.Value = DateTime.Now.Date;
                dateTimePicker2.Value = DateTime.Now.Date;
            }

        }
        // Presenter Errors
        private void radioButtonPresenterErrors_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonPresenterErrors.Checked == true)
            {

                labelInput.Hide();
                textBoxInputField.Hide();

                panel4.Show();

                panel3.Hide();

                checkBoxMatchingNotDone.Checked = false;

                dateTimePicker1.Value = DateTime.Now.Date;
                dateTimePicker2.Value = DateTime.Now.Date;

                checkBoxWithAmt.Checked = false;
                checkBoxThroughIST_1.Checked = false;
                checkBoxWithAmt.Hide();
                textBoxAmnt.Hide();
            }
        }
        // Show Reversals
        private void radioButtonReversals_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonReversals.Checked == true)
            {

                labelInput.Hide();
                textBoxInputField.Hide();

                panel4.Show();

                panel3.Hide();

                checkBoxMatchingNotDone.Checked = false;

                dateTimePicker1.Value = DateTime.Now.Date;
                dateTimePicker2.Value = DateTime.Now.Date;

                checkBoxWithAmt.Checked = false;
                checkBoxThroughIST_1.Checked = false;
                checkBoxWithAmt.Hide();
                textBoxAmnt.Hide();
            }

        }
        // Dispute Full Card 
        private void radioButtonFullCard_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonFullCard.Checked == true)
            {

                labelInput.Text = "Full Card";
                textBoxInputField.Text = "";

                buttonEncrypt.Show();
                label4.Show();
                label5.Show();
                panel3.Hide();
                panel4.Show();
                labelInput.Show();
                textBoxInputField.Show();
                checkBoxMatchingNotDone.Checked = true;
                dateTimePicker1.Value = DateTime.Now.Date;
                dateTimePicker2.Value = DateTime.Now.Date;
            }
        }
        // Phone Number 
        private void radioButtonPhone_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonPhone.Checked == true)
            {

                labelInput.Text = "Phone Number";
                textBoxInputField.Text = "";

                buttonEncrypt.Show();
                label4.Show();
                label5.Show();
                panel3.Hide();
                panel4.Show();
                labelInput.Show();
                textBoxInputField.Show();
                checkBoxMatchingNotDone.Checked = true;
                dateTimePicker1.Value = DateTime.Now.Date;
                dateTimePicker2.Value = DateTime.Now.Date;
            }
        }
        // Reversals Deposits 
        private void radioButtonRevDep_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonRevDep.Checked == true)
            {

                labelInput.Hide();
                textBoxInputField.Hide();

                panel4.Show();

                panel3.Hide();

                checkBoxMatchingNotDone.Checked = false;

                dateTimePicker1.Value = DateTime.Now.Date;
                dateTimePicker2.Value = DateTime.Now.Date;

                checkBoxWithAmt.Checked = false;
                checkBoxThroughIST_1.Checked = false;
                checkBoxWithAmt.Hide();
                textBoxAmnt.Hide();
            }
        }
        // Capture card
        private void radioCaptureBr_CheckedChanged(object sender, EventArgs e)
        {
            if (radioCaptureBr.Checked == true)
            {

                labelInput.Text = "User's Branch/Dpt";
                textBoxInputField.Text = "";

                buttonEncrypt.Show();
                label4.Show();
                label5.Show();
                panel3.Hide();
                panel4.Show();
                labelInput.Show();
                textBoxInputField.Show();
                checkBoxMatchingNotDone.Checked = true;
                dateTimePicker1.Value = DateTime.Now.Date;
                dateTimePicker2.Value = DateTime.Now.Date;
            }
        }
        // Bank 
        private void radioCapturedBank_CheckedChanged(object sender, EventArgs e)
        {
            if (radioCapturedBank.Checked == true)
            {

                labelInput.Text = "Bank";
                textBoxInputField.Text = "All In Bank By Range";

                buttonEncrypt.Show();
                label4.Show();
                label5.Show();
                panel3.Hide();
                panel4.Show();
                labelInput.Show();
                textBoxInputField.Show();
                checkBoxMatchingNotDone.Checked = true;
                dateTimePicker1.Value = DateTime.Now.Date;
                dateTimePicker2.Value = DateTime.Now.Date;
            }
        }
        // FULL CARD
        private void radioCapturedFullCard_CheckedChanged(object sender, EventArgs e)
        {
            if (radioCapturedFullCard.Checked == true)
            {

                labelInput.Text = "Full Card";
                textBoxInputField.Text = "";

                buttonEncrypt.Show();
                label4.Show();
                label5.Show();
                panel3.Hide();
                panel4.Show();
                labelInput.Show();
                textBoxInputField.Show();
                checkBoxMatchingNotDone.Checked = true;
                dateTimePicker1.Value = DateTime.Now.Date;
                dateTimePicker2.Value = DateTime.Now.Date;
            }
        }
        // Captured per ATM by dates range 
        private void radioCapturedATM_CheckedChanged(object sender, EventArgs e)
        {
            if (radioCapturedATM.Checked == true)
            {

                labelInput.Text = "ATM No";
                textBoxInputField.Text = "";

                buttonEncrypt.Show();
                label4.Show();
                label5.Show();
                panel3.Hide();
                panel4.Show();
                labelInput.Show();
                textBoxInputField.Show();
                checkBoxMatchingNotDone.Checked = true;
                dateTimePicker1.Value = DateTime.Now.Date;
                dateTimePicker2.Value = DateTime.Now.Date;
            }
        }
// Discrepancies 
        private void radioButtonATM_Discrepancies_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonATM_Discrepancies.Checked == true)
            {
                labelInput.Text = "ATM No";
                textBoxInputField.Text = "AB104";
               
                panel4.Show();
                labelInput.Show();
                textBoxInputField.Show();
                panel3.Hide();

                checkBoxMatchingNotDone.Checked = false;

                dateTimePicker1.Value = DateTime.Now.Date;
                dateTimePicker2.Value = DateTime.Now.Date;

                checkBoxWithAmt.Checked = false;
                checkBoxThroughIST_1.Checked = false;
                checkBoxWithAmt.Hide();
                textBoxAmnt.Hide();
            }
        }
// Show by action id 
        private void radioButtonByAction_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonByAction.Checked == true)
            {
                labelInput.Text = "Action/s";
                textBoxInputField.Text = "'10','11','91','92' etc";
                MessageBox.Show("You place Actions IDs as shown"+ Environment.NewLine
                    + "'10' and '11' are related to Dispute postponed or cancel"
                    + "Separate each action with a comma(,) "
                    );
                buttonEncrypt.Show();
                label4.Show();
                label5.Show();
                panel3.Hide();
                panel4.Show();
                labelInput.Show();
                textBoxInputField.Show();
                checkBoxMatchingNotDone.Checked = true;
                dateTimePicker1.Value = DateTime.Now.Date;
                dateTimePicker2.Value = DateTime.Now.Date;

                checkBoxWithAmt.Checked = false;
                checkBoxThroughIST_1.Checked = false;
                checkBoxWithAmt.Hide();
                textBoxAmnt.Hide();
            }
            else
            {
                buttonEncrypt.Hide();
                label4.Hide();
                label5.Hide();
            }
        }


        bool FormatValid(string format, string allowableLetters)
        {
            
            foreach (char c in format)
            {
                // This is using String.Contains for .NET 2 compat.,
                //   hence the requirement for ToString()
                if (!allowableLetters.Contains(c.ToString()))
                    return false;
            }

            return true;
        }
// With Amount 
        private void checkBoxWithAmt_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxWithAmt.Checked == true)
            {
                textBoxAmnt.Show(); 
            }
            else
            {
                textBoxAmnt.Hide();
            }
        }
        // 
    }
}
