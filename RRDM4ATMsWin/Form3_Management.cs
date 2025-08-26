using System;
using System.Globalization;
using System.Windows.Forms;
using RRDM4ATMs;

//multilingual

namespace RRDM4ATMsWin
{
    public partial class Form3_Management : Form
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
        Form80b_IST NForm80b_IST;

        string WSignedId;
        int WSignRecordNo;
        string WSecLevel;
        string WOperator;

        public Form3_Management(string InSignedId, int InSignRecordNo, string InSecLevel, string InOperator)
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
            //pictureBox2.BackgroundImage = appResImg.imageBranch;
            //pictureBox3.BackgroundImage = appResImg.CallCentre1;
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
                
            }

            // Hide Disputes 

            ParamId = "719";
            OccuranceId = "3"; // Hide Disputes 

            Gp.ReadParameterByOccuranceId(ParamId, OccuranceId);

            if (Gp.RecordFound == true & Gp.OccuranceNm == "YES")
            {
                // Hide Disputes 
                
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
            if (
                radioButtonExcessReports.Checked == false
                & radioButtonShortageReports.Checked == false
                & radioButtonQuartellyByRange.Checked == false
               )
            {
                MessageBox.Show("Please select button");
                return;
            }

            WFunction = "Investigation";

            if (
                 radioButtonExcessReports.Checked == true
                || radioButtonShortageReports.Checked == true
                || radioButtonQuartellyByRange.Checked == true
                )
            {
                
                // Excess Reports
                if (radioButtonExcessReports.Checked == true)
                {
                    int Mode = 12;
                    DateTime FromDate = dateTimePicker1.Value;
                    DateTime ToDate = dateTimePicker2.Value;
                    if (dateTimePicker2.Value < dateTimePicker1.Value)
                    {
                        MessageBox.Show("Second date can not be less than first one");
                        return;
                    }

                    DateTime NullPastDate = new DateTime(1900, 01, 01);

                    Form67_Cycle_Rich_Picture_2 NForm67_Cycle_Rich_Picture_2;
                    int WReconcCycleNo = 0; 
                    string WAtmNo = "";
                    int WSesNo = 0;
                    NForm67_Cycle_Rich_Picture_2 = new Form67_Cycle_Rich_Picture_2(WSignedId, WOperator, WAtmNo, WSesNo,
                        WReconcCycleNo, FromDate.Date, ToDate.Date, Mode);
                    NForm67_Cycle_Rich_Picture_2.ShowDialog();

                   
                    return;
                }

                // Shortage Reports
                if (radioButtonShortageReports.Checked == true)
                {
                    int Mode = 14;
                    DateTime FromDate = dateTimePicker1.Value;
                    DateTime ToDate = dateTimePicker2.Value;
                    if (dateTimePicker2.Value < dateTimePicker1.Value)
                    {
                        MessageBox.Show("Second date can not be less than first one");
                        return;
                    }

                    Form67_Cycle_Rich_Picture_2 NForm67_Cycle_Rich_Picture_2;
                    int WReconcCycleNo = 0;
                    string WAtmNo = "";
                    int WSesNo = 0;
                    NForm67_Cycle_Rich_Picture_2 = new Form67_Cycle_Rich_Picture_2(WSignedId, WOperator, WAtmNo, WSesNo,
                        WReconcCycleNo, FromDate.Date, ToDate.Date, Mode);
                    NForm67_Cycle_Rich_Picture_2.ShowDialog();


                    return;
                }

                // Quarter Reports
                if (radioButtonQuartellyByRange.Checked == true)
                {
                    int Mode = 12;
                    DateTime FromDate = dateTimePicker1.Value;
                    DateTime ToDate = dateTimePicker2.Value;
                    if (dateTimePicker2.Value < dateTimePicker1.Value)
                    {
                        MessageBox.Show("Second date can not be less than first one");
                        return;
                    }

                    Form21MIS_Excess_Reports NForm21MIS_Excess_Reports;
                    NForm21MIS_Excess_Reports = new Form21MIS_Excess_Reports(WSignedId, WOperator, FromDate.Date, ToDate.Date, Mode);
                    NForm21MIS_Excess_Reports.ShowDialog();

                    return;
                }

                WStringInput = textBoxInputField.Text.Trim();

               
                }

        }

// Excess Reports 
        private void radioButtonExcessReports_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonExcessReports.Checked == true)
            {

                labelInput.Hide();
                textBoxInputField.Hide();

                panel4.Show();

                panel3.Hide();

                dateTimePicker1.Value = DateTime.Now.Date;
                dateTimePicker2.Value = DateTime.Now.Date;
            }
        }

        // Shortage Report 
        private void radioButtonShortageReports_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonShortageReports.Checked == true)
            {

                labelInput.Hide();
                textBoxInputField.Hide();

                panel4.Show();

                panel3.Hide();

                dateTimePicker1.Value = DateTime.Now.Date;
                dateTimePicker2.Value = DateTime.Now.Date;
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
// 
        private void radioButtonQuartellyByRange_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonQuartellyByRange.Checked == true)
            {

                labelInput.Hide();
                textBoxInputField.Hide();

                panel4.Show();

                panel3.Hide();


                dateTimePicker1.Value = DateTime.Now.Date;
                dateTimePicker2.Value = DateTime.Now.Date;
            }
        }
    }
}
