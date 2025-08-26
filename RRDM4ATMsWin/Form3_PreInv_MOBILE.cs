using System;
using System.Windows.Forms;
using RRDM4ATMs;

//multilingual
namespace RRDM4ATMsWin
{
    public partial class Form3_PreInv_MOBILE : Form
    {
        string WStringInput;
        int WIntInput;
        int UniqueIdType;
        string WFunction;
        string WIncludeNotMatchedYet;
        // "1" = Include
        // "" = Not Include

        RRDMEncryptPasswordOrField En = new RRDMEncryptPasswordOrField();

       // RRDMAtmsClass Ac = new RRDMAtmsClass();

        RRDMGasParameters Gp = new RRDMGasParameters(); 

        DateTime NullPastDate = new DateTime(1900, 01, 01);



        Form80b NForm80b;
        Form80b_IST NForm80b_IST;

        string WSignedId;
        //int WSignRecordNo;
        string W_Application;
        int WRMCycle;
        string WOperator;

        public Form3_PreInv_MOBILE(string InOperator, string InSignedId, string InApplication, int InRMCycle)
        {
            WSignedId = InSignedId;
            //WSignRecordNo = InSignRecordNo;
            W_Application = InApplication;
            WRMCycle = InRMCycle;
            WOperator = InOperator;

            InitializeComponent();

            // Set Working Date 

            labelStep1.Text = "Investigation.." + W_Application;

            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;
            pictureBox2.BackgroundImage = appResImg.imageBranch;
            pictureBox3.BackgroundImage = appResImg.CallCentre1;
            pictureBox4.BackgroundImage = appResImg.BackOffice;

            UserId.Text = InSignedId;

            dateTimePicker2.Value = new DateTime(2014, 02, 01);

            radioButtonMaster.Checked = true; 

            //comboBoxTXN_TYPE.Items.Add("BDC272");
            //comboBoxTXN_TYPE.Items.Add("BDC273");
            //comboBoxTXN_TYPE.Items.Add("BDC231");
            //comboBoxTXN_TYPE.Items.Add("BDC233");

            //comboBoxTXN_TYPE.Text = "BDC272";



        }
        // Finish 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        // Go to next 
        string WRRNumber;
        string Description = "";
        string WSelectionCriteria = "";
        string FileId = "";
        bool IsWithDates = false;
        bool IsFromMaster = false;
        bool IsFromMeeza = false;
        private void buttonNext_Click(object sender, EventArgs e)
        {
            if (radioButtonMaster.Checked == true)
            {
                if (radioButtonRRN_Master.Checked == false
                    & radioButtonReference_TPF_Master.Checked == false
                       & radioButtonSender_Master.Checked == false
                       & radioButtonReceiver_Master.Checked == false
                       & radioButtonCustId_Master.Checked == false

                       & radioButtonAmount_Master.Checked == false
                       & radioButtonAuth_Master.Checked == false

                       & radioButtonTxnType_Master.Checked == false

                       & radioButtonReversals_Master.Checked == false
                    )
                {
                    MessageBox.Show("Please select button");
                    return;
                }
                else
                {
                    IsFromMaster = true;

                    if (textBoxInputField.Text == "" & radioButtonReversals_Master.Checked == false & radioButtonTxnType_Master.Checked == false)
                    {
                        MessageBox.Show("Please input data");
                        return;
                    }
                }

                //}

                WFunction = "Investigation";

                //if (IsFromMaster == true)
                //    {
                if (radioButtonTxnType_Master.Checked == true)
                {
                    WStringInput = comboBoxTXN_TYPE.Text; 
                }
                else
                {
                    WStringInput = textBoxInputField.Text.Trim();
                }
                

                FileId = W_Application + "_TPF_TXNS_MASTER";

                if (radioButtonRRN_Master.Checked == true)
                {
                    Description = "Search Master By RRN.." + WStringInput;
                    WSelectionCriteria = " WHERE Reference_TPF ='" + WStringInput + "' OR RRNumber ='" + WStringInput+"'"; // Here we get RRN from the Reference PTF kept as a second field in Master  

                    // It is the Master File 

                    IsWithDates = false;

                }
                // radioButtonReference_TPF_Master
                if (radioButtonReference_TPF_Master.Checked == true)
                {
                    // Check length of Input
                    int WLength = WStringInput.Length; 
                    if (WLength != 12)
                    {
                        MessageBox.Show("The length of input is not equal to 12 " + Environment.NewLine
                                         + "Maybe you have to insert zeros at beginning? "
                                       );
                        return; 
                    }
                    Description = "Search Master By Reference_TPF.." + WStringInput;
                    WSelectionCriteria = " WHERE Reference_TPF ='" + WStringInput + "' ";  // Reference PTF kept as a second field in Master  

                    // It is the Master File 

                    IsWithDates = false;

                }

                if (radioButtonSender_Master.Checked == true)
                {
                    Description = "Search Master By Sender.." + WStringInput;
                    WSelectionCriteria = " WHERE SenderTelephone='" + WStringInput + "'";

                    // It is the Master File 
                    //FileId = W_Application + "_TPF_TXNS_MASTER";

                    IsWithDates = true;

                }

                if (radioButtonReceiver_Master.Checked == true)
                {
                    Description = "Search Master By Receiver.." + WStringInput;
                    WSelectionCriteria = " WHERE ReceivedTelephone ='" + WStringInput + "'";

                    // It is the Master File 
                    // FileId = W_Application + "_TPF_TXNS_MASTER";

                    IsWithDates = true;

                }

                if (radioButtonCustId_Master.Checked == true)
                {
                    Description = "Search Master By CustId.." + WStringInput;
                    WSelectionCriteria = " WHERE CustomerID ='" + WStringInput + "'";

                    // It is the Master File 
                    // FileId = W_Application + "_TPF_TXNS_MASTER";

                    IsWithDates = true;

                }

                if (radioButtonAmount_Master.Checked == true)
                {
                    // convert character to decimal 
                    decimal Input_Amt;
                    if (decimal.TryParse(WStringInput, out Input_Amt))
                    {
                        //  MessageBox.Show(textBox10.Text, "The input number is correct!");
                    }
                    else
                    {
                        MessageBox.Show(textBoxInputField.Text.Trim(), " Please enter a valid number!");
                        return;
                    }

                    Description = "Search Master By Amount.." + WStringInput;
                    WSelectionCriteria = " WHERE TransAmount=" + Input_Amt;

                    // It is the Master File 
                    // FileId = W_Application + "_TPF_TXNS_MASTER";

                    IsWithDates = true;

                }

                if (radioButtonAuth_Master.Checked == true)
                {
                    Description = "Search Master By AUTHNUM.." + WStringInput;
                    WSelectionCriteria = " WHERE AUTHNUM='" + WStringInput + "'";

                    // It is the Master File 
                    // FileId = W_Application + "_TPF_TXNS_MASTER";

                    IsWithDates = true;

                }

                if (radioButtonTxnType_Master.Checked == true)
                {
                    if (checkBoxOnlyDiscrepancies.Checked == true)
                    {
                        Description = "Search Master Unmatched By Txn Type  .." + WStringInput;
                        WSelectionCriteria = " WHERE TransType ='" + WStringInput + "' AND IsMatchingDone = 1 and Matched = 0 ";
                    }
                    else
                    {
                        Description = "Search Master By Txn Type.." + WStringInput;
                        WSelectionCriteria = " WHERE TransType ='" + WStringInput + "'";
                    }

                    // It is the Master File 
                    // FileId = W_Application + "_TPF_TXNS_MASTER";

                    IsWithDates = true;

                }
                if (radioButtonReversals_Master.Checked == true)
                {
                    Description = "Search Master By Reversals.." + WStringInput;
                    WSelectionCriteria = " WHERE IsReversal = 1 ";

                    // It is the Master File 
                    // FileId = W_Application + "_TPF_TXNS_MASTER";

                    IsWithDates = true;

                }



            }


            if (radioButtonMEEZA.Checked == true)
            {
                if (radioButtonRRN_Meeza.Checked == false
                       & radioButtonSender_Meeza.Checked == false
                       & radioButtonReceiver_Meeza.Checked == false

                       //& radioButtonTxn_ID_Meeza.Checked == false

                       & radioButtonAmount_Meeza.Checked == false
                       // & radioButtonAuth_Master.Checked == false

                       & radioButtonTxnType_Meeza.Checked == false

                       & radioButtonReversals_Meeza.Checked == false
                    )
                {
                    MessageBox.Show("Please select button");
                    return;
                }
                else
                {
                    IsFromMeeza = true;

                    if (textBoxInputField.Text == "" & radioButtonReversals_Meeza.Checked == false)
                    {
                        MessageBox.Show("Please input data");
                        return;
                    }
                }



                FileId = W_Application + "_MEEZA_TXNS";
                //     [ETISALAT_MATCHED_TXNS].[dbo].[ETISALAT_MEEZA_TXNS]

                if (radioButtonTxnType_Meeza.Checked == true)
                {
                    WStringInput = comboBoxTXN_TYPE.Text;
                }
                else
                {
                    WStringInput = textBoxInputField.Text.Trim();
                }

                

                if (radioButtonRRN_Meeza.Checked == true)
                {
                    Description = "Search Meeza By RRN.." + WStringInput;
                    WSelectionCriteria = " WHERE Reference ='" + WStringInput + "'"; // Here we get RRN from the Reference PTF kept as a second field in Master  

                    // It is the Master File 


                    IsWithDates = false;

                }

                if (radioButtonSender_Meeza.Checked == true)
                {
                    Description = "Search Meeza By Sender.." + WStringInput;
                    WSelectionCriteria = " WHERE SenderTelephone='" + WStringInput + "'";

                    IsWithDates = true;

                }

                if (radioButtonReceiver_Meeza.Checked == true)
                {
                    Description = "Search Meeza By Receiver.." + WStringInput;
                    WSelectionCriteria = " WHERE ReceivedTelephone ='" + WStringInput + "'";

                    // It is the Master File 
                    // FileId = W_Application + "_TPF_TXNS_MASTER";

                    IsWithDates = true;

                }

                //if (radioButtonTxn_ID_Meeza.Checked == true)
                //{
                //    Description = "Search Meeza By Txn_ID.." + WStringInput;
                //    WSelectionCriteria = " WHERE ReceivedTelephone ='" + WStringInput + "'";

                //    // It is the Master File 
                //    // FileId = W_Application + "_TPF_TXNS_MASTER";

                //    IsWithDates = true;

                //}

                if (radioButtonAmount_Meeza.Checked == true)
                {
                    // convert character to decimal 
                    decimal Input_Amt;
                    if (decimal.TryParse(WStringInput, out Input_Amt))
                    {
                        //  MessageBox.Show(textBox10.Text, "The input number is correct!");
                    }
                    else
                    {
                        MessageBox.Show(textBoxInputField.Text.Trim(), " Please enter a valid number!");
                        return;
                    }

                    Description = "Search Meeza By Amount.." + WStringInput;
                    WSelectionCriteria = " WHERE TransAmount=" + Input_Amt;

                    // It is the Master File 
                    // FileId = W_Application + "_TPF_TXNS_MASTER";

                    IsWithDates = true;

                }

               

                if (radioButtonTxnType_Master.Checked == true)
                {
                    //if (checkBoxOnlyDiscrepancies.Checked == true)
                    //{
                    //    Description = "Search Master Unmatched By Txn Type  .." + WStringInput;
                    //    WSelectionCriteria = " WHERE TransType ='" + WStringInput + "' AND IsMatchingDone = 1 and Matched = 0 ";
                    //}
                    //else
                    //{
                    //    Description = "Search Master By Txn Type.." + WStringInput;
                    //    WSelectionCriteria = " WHERE TransType ='" + WStringInput + "'";
                    //}

                       Description = "Search Meeza By Txn Type.." + WStringInput;
                       WSelectionCriteria = " WHERE TransType ='" + WStringInput + "'";

                    // It is the Master File 
                    // FileId = W_Application + "_TPF_TXNS_MASTER";

                    IsWithDates = true;

                }
                if (radioButtonReversals_Meeza.Checked == true)
                {
                    Description = "Search Master By Reversals.." + WStringInput;
                    WSelectionCriteria = " WHERE IsReversal = 1 ";

                    // It is the Master File 
                    // FileId = W_Application + "_TPF_TXNS_MASTER";

                    IsWithDates = true;

                }



            }

            // Validate the dates
            if (CheckForDatesRange == true)
            {
                if ((dateTimePicker2.Value - dateTimePicker1.Value).TotalDays > 5 || dateTimePicker2.Value < dateTimePicker1.Value)
                {
                    if ((dateTimePicker2.Value - dateTimePicker1.Value).TotalDays > 5)
                    {
                        MessageBox.Show("For the processed only range of 5 days is allowed");
                    }
                    if (dateTimePicker2.Value < dateTimePicker1.Value)
                    {
                        MessageBox.Show("Second date must be equal or greater than first Date");
                    }

                    return;
                }
            }
           
            if (IsWithDates == true)
            {
                Form78d_SHOW_MOBILE NForm78d_SHOW_MOBILE;
                NForm78d_SHOW_MOBILE = new Form78d_SHOW_MOBILE(WSignedId, Description, WSelectionCriteria,
                                               dateTimePicker1.Value, dateTimePicker2.Value, FileId, IsFromMaster, W_Application);
                NForm78d_SHOW_MOBILE.ShowDialog();
            }
            else
            {
                // No Dates 
                Form78d_SHOW_MOBILE NForm78d_SHOW_MOBILE;
                NForm78d_SHOW_MOBILE = new Form78d_SHOW_MOBILE(WSignedId, Description, WSelectionCriteria,
                                               NullPastDate, NullPastDate, FileId, IsFromMaster, W_Application);
                NForm78d_SHOW_MOBILE.ShowDialog();
            }


        }



        // Category Non Processed

        private void radioButtonCategoryNonProcessed_CheckedChanged(object sender, EventArgs e)
        {
            // Leave it here 
        }

        // MASTER RADIO BUTTON
        private void radioButtonMaster_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonMaster.Checked == true)
            {
                panel_Master.Show();
                panel_Meeza.Hide();

                labelInput.Text = "Input Text";
                textBoxInputField.Text = "****";

                // Initialise Meeza Radio
                radioButtonRRN_Meeza.Checked = false;
                radioButtonSender_Meeza.Checked = false;
                radioButtonReceiver_Meeza.Checked = false;
                //radioButtonTxn_ID_Meeza.Checked = false;
                radioButtonAmount_Meeza.Checked = false;
                radioButtonTxnType_Meeza.Checked = false;

                radioButtonReversals_Meeza.Checked = false;


                // Initialise Master Radio 
                radioButtonRRN_Master.Checked = false;
                radioButtonSender_Master.Checked = false;
                radioButtonReceiver_Master.Checked = false;
                radioButtonCustId_Master.Checked = false;

                radioButtonAmount_Meeza.Checked = false;
                radioButtonAuth_Master.Checked = false;

                radioButtonTxnType_Meeza.Checked = false;
                checkBoxOnlyDiscrepancies.Checked = false;

                radioButtonReversals_Meeza.Checked = false;

                labelTransType.Hide();

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
        // MEEZA BUTTON
        private void radioButtonMEEZA_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonMEEZA.Checked == true)
            {
                panel_Meeza.Show();
                panel_Master.Hide();

                labelInput.Text = "Input Text";
                textBoxInputField.Text = "****";

                labelInput.Show();
                textBoxInputField.Show();

                labelTransType.Hide();

                // labelCateg.Hide();
                comboBoxTXN_TYPE.Hide();

                panel4.Show();

                //checkBoxMatchingNotDone.Checked = true;
                dateTimePicker1.Value = DateTime.Now.Date;
                dateTimePicker2.Value = DateTime.Now.Date;

                // Initialise Meeza Radio
                radioButtonRRN_Meeza.Checked = false;
                radioButtonSender_Meeza.Checked = false;
                radioButtonReceiver_Meeza.Checked = false;
                //radioButtonTxn_ID_Meeza.Checked = false;
                radioButtonAmount_Meeza.Checked = false;
                radioButtonTxnType_Meeza.Checked = false;

                radioButtonReversals_Meeza.Checked = false;


                // Initialise Master Radio 
                radioButtonRRN_Master.Checked = false;
                radioButtonSender_Master.Checked = false;
                radioButtonReceiver_Master.Checked = false;
                radioButtonCustId_Master.Checked = false;

                radioButtonAmount_Master.Checked = false;
                radioButtonAuth_Master.Checked = false;

                radioButtonTxnType_Master.Checked = false;
                checkBoxOnlyDiscrepancies.Checked = false;

                radioButtonReversals_Master.Checked = false;
            }
        }
        // RRN 
        bool CheckForDatesRange;
        private void radioButtonRRN_Master_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonRRN_Master.Checked == true)
            {
                labelTransType.Hide();
                comboBoxTXN_TYPE.Hide();
                labelFromDt.Hide();
                labelTo_Dt.Hide();
                dateTimePicker1.Hide();
                dateTimePicker2.Hide();

                CheckForDatesRange = false; 

                labelInput.Show();
                textBoxInputField.Show();
                textBoxInputField.Text = "";
     
            }
        }
        private void radioButtonReference_TPF_Master_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonReference_TPF_Master.Checked == true)
            {
                labelTransType.Hide();
                comboBoxTXN_TYPE.Hide();
                labelFromDt.Hide();
                labelTo_Dt.Hide();
                dateTimePicker1.Hide();
                dateTimePicker2.Hide();

                CheckForDatesRange = false;

                labelInput.Show();
                textBoxInputField.Show();
                textBoxInputField.Text = "";

            }
        }
        // Send Mobile
        private void radioButtonSender_Master_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonSender_Master.Checked == true)
            {
                labelTransType.Hide();
                comboBoxTXN_TYPE.Hide();
                labelFromDt.Show();
                labelTo_Dt.Show();
                dateTimePicker1.Show();
                dateTimePicker2.Show();

                CheckForDatesRange = true; 

                labelInput.Show();
                textBoxInputField.Show();
                textBoxInputField.Text = "";
            }
        }
        // Receiver
        private void radioButtonReceiver_Master_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonReceiver_Master.Checked == true)
            {
                labelTransType.Hide();
                comboBoxTXN_TYPE.Hide();
                labelFromDt.Show();
                labelTo_Dt.Show();
                dateTimePicker1.Show();
                dateTimePicker2.Show();

                CheckForDatesRange = true;

                labelInput.Show();
                textBoxInputField.Show();
                textBoxInputField.Text = "";
            }
        }
        // Cust ID
        private void radioButtonCustId_Master_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonCustId_Master.Checked == true)
            {
                labelTransType.Hide();
                comboBoxTXN_TYPE.Hide();
                labelFromDt.Show();
                labelTo_Dt.Show();
                dateTimePicker1.Show();
                dateTimePicker2.Show();

                CheckForDatesRange = true;

                labelInput.Show();
                textBoxInputField.Show();
                textBoxInputField.Text = "";
            }
        }

        // Amount 
        private void radioButtonAmount_Master_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonAmount_Master.Checked == true)
            {
                labelTransType.Hide();
                comboBoxTXN_TYPE.Hide();
                labelFromDt.Show();
                labelTo_Dt.Show();
                dateTimePicker1.Show();
                dateTimePicker2.Show();

                CheckForDatesRange = true;

                labelInput.Show();
                textBoxInputField.Show();
                textBoxInputField.Text = "";
            }
        }

        // Auth Number
        private void radioButtonAuth_Master_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonAuth_Master.Checked == true)
            {
                labelTransType.Hide();
                comboBoxTXN_TYPE.Hide();
                labelFromDt.Show();
                labelTo_Dt.Show();
                dateTimePicker1.Show();
                dateTimePicker2.Show();

                CheckForDatesRange = true;

                labelInput.Show();
                textBoxInputField.Show();
                textBoxInputField.Text = "";
            }
        }

        // TRANS TYPE
        private void radioButtonTxnType_Master_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonTxnType_Master.Checked == true)
            {
                labelTransType.Show();
                comboBoxTXN_TYPE.Show();
                labelFromDt.Show();
                labelTo_Dt.Show();
                dateTimePicker1.Show();
                dateTimePicker2.Show();

                CheckForDatesRange = true;

                labelInput.Hide();
                textBoxInputField.Hide();
                //textBoxInputField.Text = "";
                // TXN TYPES
                Gp.ParamId = "938";
                comboBoxTXN_TYPE.DataSource = Gp.GetParamOccurancesNm(WOperator);
                comboBoxTXN_TYPE.DisplayMember = "DisplayValue";
            }
        }

        // REVERSALS
        private void radioButtonReversals_Master_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonReversals_Master.Checked == true)
            {
                labelTransType.Hide();
                comboBoxTXN_TYPE.Hide();
                labelFromDt.Show();
                labelTo_Dt.Show();
                dateTimePicker1.Show();
                dateTimePicker2.Show();

                CheckForDatesRange = true;

                labelInput.Hide();
                textBoxInputField.Hide();
            }
        }


        // MEEZA RRN 
        private void radioButtonRRN_Meeza_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonRRN_Meeza.Checked == true)
            {
                labelTransType.Hide();
                comboBoxTXN_TYPE.Hide();
                labelFromDt.Hide();
                labelTo_Dt.Hide();
                dateTimePicker1.Hide();
                dateTimePicker2.Hide();

                labelInput.Show();
                textBoxInputField.Show();
                textBoxInputField.Text = "";
            }
        }
        // MEEZA
        private void radioButtonSender_Meeza_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonSender_Meeza.Checked == true)
            {
                labelTransType.Hide();
                comboBoxTXN_TYPE.Hide();
                labelFromDt.Show();
                labelTo_Dt.Show();
                dateTimePicker1.Show();
                dateTimePicker2.Show();

                CheckForDatesRange = true;

                labelInput.Show();
                textBoxInputField.Show();
                textBoxInputField.Text = "";
            }
        }
// Receiver 
        private void radioButtonReceiver_Meeza_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonReceiver_Meeza.Checked == true)
            {
                labelTransType.Hide();
                comboBoxTXN_TYPE.Hide();
                labelFromDt.Show();
                labelTo_Dt.Show();
                dateTimePicker1.Show();
                dateTimePicker2.Show();

                CheckForDatesRange = true;

                labelInput.Show();
                textBoxInputField.Show();
                textBoxInputField.Text = "";
            }
        }
// AMT
        private void radioButtonAmount_Meeza_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonAmount_Meeza.Checked == true)
            {
                labelTransType.Hide();
                comboBoxTXN_TYPE.Hide();
                labelFromDt.Show();
                labelTo_Dt.Show();
                dateTimePicker1.Show();
                dateTimePicker2.Show();

                CheckForDatesRange = true;

                labelInput.Show();
                textBoxInputField.Show();
                textBoxInputField.Text = "";
            }
        }
// TXN TYPE
        private void radioButtonTxnType_Meeza_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonTxnType_Meeza.Checked == true)
            {
                labelTransType.Show();
                comboBoxTXN_TYPE.Show();
                labelFromDt.Show();
                labelTo_Dt.Show();
                dateTimePicker1.Show();
                dateTimePicker2.Show();

                CheckForDatesRange = true;

                labelInput.Hide();
                textBoxInputField.Hide();
                //textBoxInputField.Text = "";
                // TXN TYPES
                Gp.ParamId = "940";
                comboBoxTXN_TYPE.DataSource = Gp.GetParamOccurancesNm(WOperator);
                comboBoxTXN_TYPE.DisplayMember = "DisplayValue";
            }
        }
// Reversals 
        private void radioButtonReversals_Meeza_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonReversals_Meeza.Checked == true)
            {
                labelTransType.Hide();
                comboBoxTXN_TYPE.Hide();
                labelFromDt.Show();
                labelTo_Dt.Show();
                dateTimePicker1.Show();
                dateTimePicker2.Show();

                CheckForDatesRange = true;

                labelInput.Hide();
                textBoxInputField.Hide();
            }
        }

       
    }
}
