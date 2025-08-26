using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

//multilingual

namespace RRDM4ATMsWin
{
    public partial class Form53 : Form
    {

        Form68 NForm68; 

        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();

        RRDMJournalAndAllowUpdate Aj = new RRDMJournalAndAllowUpdate();

        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        string AtmNo;
        string FromFunction;
        string WCitId;

        DateTime FromDate ;
        DateTime ToDate;

        DateTime WDtFrom;// Needed for repl cycles
        DateTime WDtTo;

        int SelModeA;
        int SelModeB;
        string EnteredNumber;
        int WTraceNo;

        string WAtmNo;
        int WSesNo;

        string WSignedId;
        int WSignRecordNo;
        string WOperator; 
  
        public Form53(string InSignedId, int InSignRecordNo, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator; 

            InitializeComponent();

            // Set Working Date 
            RRDMGasParameters Gp = new RRDMGasParameters();
            string ParId = "267";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            string TestingDate = Gp.OccuranceNm;
            if (TestingDate == "YES")
                labelToday.Text = new DateTime(2017, 03, 01).ToShortDateString();
            else labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;

            //-----------------ACCESS CONTROL TO WHAT ATMS TO SEE---------------//

            // Create table with the ATMs this user can access
            Us.ReadUsersRecord(WSignedId);
            if (Us.CitId != "1000")
            {
                // CIT USER 
                AtmNo = "";
                FromFunction = "FromCit";
                WCitId = Us.CitId;
            }
            else
            {
                AtmNo = "";
                FromFunction = "General";
                WCitId = "";
            }

            // Create table with the ATMs this user can access
            Am.ReadViewAtmsMainForAuthUserAndFillTable(WOperator, WSignedId, WSignRecordNo, AtmNo, FromFunction, WCitId);

            //-----------------UPDATE LATEST TRANSACTIONS----------------------//
            // Update latest transactions from Journal 
            if (WOperator == "CRBAGRAA")
            {
                Aj.UpdateLatestEjStatusVersion2(WSignedId, WSignRecordNo, WOperator, Am.TableATMsMainSelected);
            }
            //
            //-----------------------------------------------------------------// 
            if (WOperator == "CRBAGRAA")
            {
                //TEST
                dateTimePickerStartDt.Value = new DateTime(2014, 02, 12);
                dateTimePickerEndDt.Value = new DateTime(2014, 02, 13);
            }
            else
            {
                // EG ETHNIKI 
                dateTimePickerStartDt.Value = new DateTime(2017, 02, 12);
                dateTimePickerEndDt.Value = DateTime.Now;
            }
            
            radioButton1.Checked = true; // Single ATM

            radioButton3.Checked = true; // All for period 

            textBoxMsgBoard.Text = "Make your selection for Drilling ";
             
        }
        // Load 
        private void Form53_Load(object sender, EventArgs e)
        {
            //Load Datagrid with what is allowed
            dataGridViewMyATMS.DataSource = Am.TableATMsMainSelected.DefaultView;

            dataGridViewMyATMS.Columns[0].Width = 110; // AtmNo
            dataGridViewMyATMS.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewMyATMS.Columns[1].Width = 70; // ReplCycle
            dataGridViewMyATMS.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridViewMyATMS.Columns[2].Width = 120; // AtmName

            dataGridViewMyATMS.Columns[3].Width = 120; // RespBranch

            dataGridViewMyATMS.Columns[4].Width = 70; // Auth User 

            dataGridViewMyATMS.Columns[0].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridViewMyATMS.Columns[4].DefaultCellStyle.ForeColor = Color.LightSlateGray;

            if (dataGridViewMyATMS.Rows.Count == 0)
            {
                Form2 MessageForm = new Form2("You are not the owner of any ATM.");
                MessageForm.ShowDialog();

                this.Dispose();
                return;
            }

                //TEST
                // SET ROW Selection POSITIONING 
                //dataGridViewMyATMS.Rows[1].Selected = true;
                //dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, 1));

        }
        // ROW ENTER 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridViewMyATMS.Rows[e.RowIndex];

            WAtmNo = (string)rowSelected.Cells[0].Value;

            label17.Text = "REPL. CYCLE/s FOR ATM : " + WAtmNo;
            WDtFrom = new DateTime(1900, 01, 01);
            WDtTo = DateTime.Today;

            Ta.ReadReplCyclesForFromToDateFillTable(WOperator, WSignedId, WAtmNo, WDtFrom, WDtTo);

            dataGridView2.DataSource = Ta.ATMsReplCyclesSelectedPeriod.DefaultView;

            dataGridView2.Columns[0].Width = 70; // SesNo
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            // dataGridView2.Sort(dataGridView2.Columns[0], ListSortDirection.Descending);

            dataGridView2.Columns[1].Width = 130; // SesDtTimeStart
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[2].Width = 130; // SesDtTimeEnd
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[3].Width = 180; // ProcessMode
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[4].Width = 65; // Mode_2
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[5].Width = 100; // AtmNo 
            dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            textBox2.Text = WAtmNo;
          
            textBoxMsgBoard.Text = "Make your selection for Drilling ";

        }
        //
        // On Row ENTER 
        //
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            WSesNo = (int)rowSelected.Cells[0].Value;

            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

            dateTimePickerStartDt.Value = Ta.SesDtTimeStart;
            dateTimePickerEndDt.Value = Ta.SesDtTimeEnd;

            textBox4.Text = WSesNo.ToString();

        }
        // Show 
        private void button7_Click(object sender, EventArgs e)
        {
            //
            // Validation for invalid characters 
            //
            System.Text.RegularExpressions.Regex expr = new System.Text.RegularExpressions.Regex
              (@"^[a-zA-Z0-9]*$");

            if (expr.IsMatch(textBoxInputField.Text))
            {
                //   MessageBox.Show("field");
            }
            else
            {
                MessageBox.Show("invalid Characters In Input Field");
                return;
            }

            //TEST
            if (WAtmNo == "AB102" || WAtmNo == "ABC502")
            {

            }
            //else
            //{
            //    MessageBox.Show("For testing Please enter only AB104 or ABC502 ATM ... ");
            //    return;
            //}

            FromDate = dateTimePickerStartDt.Value;
            ToDate = dateTimePickerEndDt.Value;

            if (radioButton1.Checked == true) SelModeA = 1;
            if (radioButton2.Checked == true) SelModeA = 2;
            if (radioButton3.Checked == true) SelModeB = 3;
            if (radioButton4.Checked == true) SelModeB = 4;
            if (radioButton5.Checked == true) SelModeB = 5;
            if (radioButton6.Checked == true) SelModeB = 6;
            if (radioButton7.Checked == true) SelModeB = 7;
            if (radioButton8.Checked == true) SelModeB = 8;
            if (radioButton9.Checked == true) SelModeB = 9;
            if (radioButton10.Checked == true) SelModeB = 10;

            if (SelModeA == 2 & SelModeB == 3 || SelModeA == 2 & SelModeB == 4 || SelModeA == 2 & SelModeB == 6
                || SelModeA == 2 & SelModeB == 7 || SelModeA == 2 & SelModeB == 10)
            {
                MessageBox.Show("This option is only available when one Atm is selected ");
                return; 
            }

            if (SelModeB == 8 || SelModeB == 9 || SelModeB == 10)
            {
                // Check if Number is entered 
                if (String.IsNullOrEmpty(textBoxInputField.Text))
                {
                    if (SelModeB == 8)
                    {
                        MessageBox.Show("Enter Data for card such as 4506531111117072");
                        textBoxInputField.Text = "4506531111117072";
                    }
                    if (SelModeB == 9)
                    {
                        MessageBox.Show("Enter Data for Account such as 012801038482");
                        textBoxInputField.Text = "012801038482";
                    }
                    if (SelModeB == 10)
                    {
                        MessageBox.Show("Enter Data for TraceNumber such as 10042990 ");
                        textBoxInputField.Text = "10042990";
                    } 
                      
                    return;
                }
                else // There is value = something will be reported 
                {
                    EnteredNumber = textBoxInputField.Text; 
                }
            }
            if (SelModeB == 10)
            {
                
                if (int.TryParse(EnteredNumber, out WTraceNo))
                {
                    EnteredNumber = WTraceNo.ToString();
                }
                else
                {
                    MessageBox.Show(textBoxInputField.Text, "Please enter a valid Trace number!");
                    return;
                }
            }

            //TEST
            if (WAtmNo == "AB102" )
            {
                WAtmNo = "AB104";
                WSesNo = 9051; 
            }
            String JournalId = ""; 
            if (WOperator == "ETHNCY2N")
            {
                JournalId = "[ATM_MT_Journals].[dbo].[tblHstAtmTxns]";
            }
            else
            {
                JournalId = "[ATMS_Journals].[dbo].[tblHstEjText]";
            }
            

            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

            NForm68 = new Form68(WSignedId, WSignRecordNo, WOperator, JournalId, FromDate.Date,ToDate.Date, WAtmNo, WSesNo, 
                Ta.FirstTraceNo, Ta.LastTraceNo, EnteredNumber, WTraceNo, SelModeA, SelModeB);
            NForm68.ShowDialog(); 

        }
        // On Change 
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            radioButton3.Visible = false;
            radioButton4.Visible = false;
            radioButton6.Visible = false;
            radioButton7.Visible = false;
            radioButton10.Visible = false;
            radioButton5.Checked = true; 
        }
        // on change 
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            radioButton3.Visible = true;
            radioButton4.Visible = true;
            radioButton6.Visible = true;
            radioButton7.Visible = true;
            radioButton10.Visible = true;
            radioButton3.Checked = true; 

        }
        // Empty textBox1 
        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {
            textBoxInputField.Text = ""; 
        }

        private void radioButton9_CheckedChanged(object sender, EventArgs e)
        {
            textBoxInputField.Text = ""; 
        }

        private void radioButton10_CheckedChanged(object sender, EventArgs e)
        {
            textBoxInputField.Text = "";
        }
        // Finish
        private void buttonNext_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }
    }
}
