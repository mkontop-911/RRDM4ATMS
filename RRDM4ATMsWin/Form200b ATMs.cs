using System;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Configuration;
// Alecos
using System.Diagnostics;

namespace RRDM4ATMsWin
{
    public partial class Form200bATMs : Form
    {
        // Variables

        Form54 NForm54;
        Form55 NForm55;

        string MsgFilter;

        //string WLoadingSchedule;

        int WSeqNo;

        RRDMJTMEventSchedules Js = new RRDMJTMEventSchedules();

        RRDMJTMIdentificationDetailsClass Jd = new RRDMJTMIdentificationDetailsClass();

        //RRDMBanks Ba = new RRDMBanks();

        RRDMUsersAccessToAtms Ua = new RRDMUsersAccessToAtms();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMControllerMsgClass Cm = new RRDMControllerMsgClass();

        RRDMGasParameters Gp = new RRDMGasParameters(); 

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        DateTime NullFutureDate = new DateTime(2050, 11, 21);

        string WSelectionCriteria;

        string WSignedId;
        int WSignRecordNo;
        string WSecLevel;
        string WOperator;

        // Methods 
        // READ ATMs Main
        // 
        public Form200bATMs(string InSignedId, int SignRecordNo, string InSecLevel, string InOperator )
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WSecLevel = InSecLevel;
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

            labelUserId.Text = WSignedId;

            //*****************************************
            //
            labelStep1.Text = "Event Schedules";
            //
            //*****************************************

            textBoxMsgBoard.Text = "Event Schedules Maintenance ";

            checkBoxTesting.Checked = false;
            checkBoxAdd.Checked = false; 

            // Event Types  
            Gp.ParamId = "241"; // 
            comboBoxEventTypeSelect.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxEventTypeSelect.DisplayMember = "DisplayValue";
            //
            // .... 
            //
            MsgFilter =
                  "(ReadMsg = 0 AND ToAllAtms = 1)"
                  + " OR (ReadMsg = 0 AND ToUser='" + WSignedId + "')";

            Cm.ReadControlerMSGsSerious(MsgFilter);

            if (Cm.SerMsgCount > 0)
            {
                string messagesStatus = " You have " + Cm.SerMsgCount.ToString() + " personal messages from the controller.";

                toolTipMessages.SetToolTip(buttonMsgs, messagesStatus);
                toolTipMessages.ShowAlways = true;
            }
            else
            {
                toolTipMessages.SetToolTip(buttonMsgs, "Check messages from controller.");
                toolTipMessages.ShowAlways = true;
            }

            toolTipController.SetToolTip(buttonCommController, "Communicate with today's controller.");
        }
        //
        // LOAD
        //
        private void Form200bATMs_Load(object sender, EventArgs e)
        {
            // FILL DataGrid
            WSelectionCriteria = "WHERE EventType ='" + comboBoxEventTypeSelect.Text + "'";

            Js.ReadJTMEventSchedulesToFillPartialTable(WSelectionCriteria);

            if (Js.EventSchedulesTable.Rows.Count > 0)
            {
                dataGridView1.DataSource = Js.EventSchedulesTable.DefaultView;

                dataGridView1.Columns[0].Width = 80; // 
                dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[1].Width = 500; // 
                dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView1.Columns[2].Width = 180; // 
                dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView1.Columns[3].Width = 80; // 
                dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }
            else
            {
                dataGridView1.DataSource = null;
                dataGridView1.Refresh();
                textBoxScheduleID.Text = "";
                dateTimePicker1.Value = DateTime.Now;
                checkBoxAdd.Checked = true; 
            }

        }

        // ROW ENTER FOR JOB CYCLE 
        //bool ChangeFromInput;
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WSeqNo = (int)rowSelected.Cells[0].Value;

            //WLoadingSchedule = (string)rowSelected.Cells[1].Value;

            //Initialise Fields

            //ChangeFromInput = false;

            radioButtonRecurDaily.Checked = false;
            radioButtonRecurWeekly.Checked = false;
            radioButtonRecurMonthly.Checked = false;
            radioButtonRecurPerMinutes.Checked = false;

            radioButtonRecurEveryDays.Checked = false;
            radioButtonRecurEveryWeekDay.Checked = false;

            // READ LINE IN GRID AND SET UP SCREEN
            WSelectionCriteria = " WHERE SeqNo = " + WSeqNo;

            Js.ReadJTMEventSchedulesToGetRecord(WSelectionCriteria);

            if (Js.RecordFound == true)
            {
                dateTimePicker1.Value = Js.EffectiveDateTmFrom;
                dateTimePicker4.Value = Js.EffectiveDateTmTo;

                textBoxScheduleID.Text = Js.ScheduleID;

                if (Js.RecurDaily == true) radioButtonRecurDaily.Checked = true;
                if (Js.RecurWeekly == true) radioButtonRecurWeekly.Checked = true;
                if (Js.RecurMonthly == true) radioButtonRecurMonthly.Checked = true;
                if (Js.RecurPerMinutes == true) radioButtonRecurPerMinutes.Checked = true;

                if (Js.RecurEveryDays == true) radioButtonRecurEveryDays.Checked = true;
                if (Js.RecurEveryWeekDay == true) radioButtonRecurEveryWeekDay.Checked = true;

                textBoxNumberOfDays.Text = Js.NumberOfDays.ToString();

                numericUpDownRecurPerMinutes.Text = Js.RecurPerMinutesValue.ToString(); 

                ShowPanelsDetails();

            }

            //ChangeFromInput = true;

        }
        // Show Panel Details 
        private void ShowPanelsDetails()
        {
            //
            // RECURRENCE
            //
            panel4.Hide();
            panel5.Hide();
            panel6.Hide();
            panel10.Hide();
            panel9.Hide();
            panel7.Hide();

         
                label9.Show();
                //label21.Show();

                if (radioButtonRecurDaily.Checked == true)
                {
                    panel4.Show();
                  
                    // Daily selection 
                }
              
                if (radioButtonRecurWeekly.Checked == true)
                {
                    panel5.Show();
                  
                    // Weekly Selection
                }
          
                if (radioButtonRecurMonthly.Checked == true)
                {
                    panel6.Show();
                   
                    // Monthly Selection
                    if (radioButtonMDay.Checked == true)
                    {

                    }
                    if (radioButtonMSpecific.Checked == true)
                    {

                    }

                }
        
                if (radioButtonRecurPerMinutes.Checked == true)
                {
                    numericUpDownRecurPerMinutes.Show();
                }
                else
                {
                    numericUpDownRecurPerMinutes.Hide();
                }

                // DELETE JOURNAL 
                if (radioButtonDeleteDaily.Checked == true)
                {
                
                    panel10.Show();
                    // Daily selection 
                }

                if (radioButtonDeleteWeekly.Checked == true)
                {
               
                    panel9.Show();
                    // Weekly Selection
                }

                if (radioButtonDeleteMonthly.Checked == true)
                {
                   
                    panel7.Show();
                    // Monthly Selection
                    if (radioButtonMDay.Checked == true)
                    {

                    }
                    if (radioButtonMSpecific.Checked == true)
                    {

                    }

                }

                if (radioButtonDeletePerMinutes.Checked == true)
                {
                    numericUpDownDelete.Show();
                }
                else
                {
                    numericUpDownDelete.Hide();
                }
          
            //else
            //{
            //    label9.Hide();
            //    label21.Hide();

            //    panel4.Hide();
            //    panel5.Hide();
            //    panel6.Hide();
            //}
        }

        //
        // Message from Controller 
        //
        private void buttonMsgs_Click_1(object sender, EventArgs e)
        {
            NForm55 = new Form55(MsgFilter, WSignedId);
            NForm55.ShowDialog();

            MsgFilter =
                 "(ReadMsg = 0 AND ToAllAtms = 1)"
             + " OR (ReadMsg = 0 AND ToUser='" + WSignedId + "')";


            Cm.ReadControlerMSGsSerious(MsgFilter);

            if (Cm.SerMsgCount > 0)
            {
                string messagesStatus = " You have " + Cm.SerMsgCount.ToString() + " personal messages from the controller.";

                toolTipMessages.SetToolTip(buttonMsgs, messagesStatus);
                toolTipMessages.ShowAlways = true;
            }
            else
            {

                toolTipMessages.SetToolTip(buttonMsgs, "Check messages from controller.");
                toolTipMessages.ShowAlways = true;
            }
        }
        //
        // Todays Controller 
        //
        private void buttonCommController_Click_1(object sender, EventArgs e)
        {
            NForm54 = new Form54(WSignedId, WSignRecordNo, WOperator);
            NForm54.ShowDialog();
        }
        //
        //Finish 
        //
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        //
        // CheckBox Recurrence
        //
        private void checkBoxRecurrence_CheckedChanged(object sender, EventArgs e)
        {
            ShowPanelsDetails();
        }
        //
        // RECURRENCE RADIO
        //
        private void radioButtonRecurDaily_CheckedChanged(object sender, EventArgs e)
        {
            ShowPanelsDetails();
        }

        private void radioButtonRecurWeekly_CheckedChanged(object sender, EventArgs e)
        {
            ShowPanelsDetails();
        }

        private void radioButtonRecurMonthly_CheckedChanged(object sender, EventArgs e)
        {
            ShowPanelsDetails();
        }

        private void radioButtonRecurPerMinutes_CheckedChanged(object sender, EventArgs e)
        {
            ShowPanelsDetails();
        }

        private void radioButtonMDay_CheckedChanged(object sender, EventArgs e)
        {
            ShowPanelsDetails();
        }

        private void radioButtonMSpecific_CheckedChanged(object sender, EventArgs e)
        {
            ShowPanelsDetails();
        }
        //
        // ADD
        //

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            // Initialise fields

            if (Js.ScheduleID == "")
            {
                MessageBox.Show("Please enter Loading Schedule ID!");
                return;
            }
            Js.DateLastUpdated = DateTime.Now;
            Js.UserId = WSignedId;

            Js.EffectiveDateTmFrom = DateTime.Now;

            Js.Recurrence = false;
            Js.RecurDaily = false;
            Js.RecurWeekly = false;
            Js.RecurMonthly = false;
            Js.RecurPerMinutes = false;
            Js.RecurEveryDays = false;
            Js.NumberOfDays = 0;
            Js.RecurEveryWeekDay = false;
            Js.RecurPerMinutesValue = 0;
            Js.Operator = WOperator;
            //
            // Set Standard Input 
            //       
            if (dateTimePicker1.Value < DateTime.Now)
            {
                MessageBox.Show("Please enter correct date and time for From! Must be greater than now.");
                return;
            }
            if (dateTimePicker4.Value < dateTimePicker1.Value)
            {
                MessageBox.Show("Please enter correct date and time for To! It must be greater than than the from.");
                return;
            }
            Js.EffectiveDateTmFrom = dateTimePicker1.Value;
            Js.EffectiveDateTmTo = dateTimePicker4.Value;

            Js.ScheduleID = textBoxScheduleID.Text;
            Js.EventType = comboBoxEventTypeSelect.Text; 
            
            //
            // RECURRENCE
            //
         
                Js.Recurrence = true;

                if (radioButtonRecurDaily.Checked == true)
                {
                    // Daily selection 
                    Js.RecurDaily = true;

                    if (radioButtonRecurEveryDays.Checked == true)
                    {
                        Js.RecurEveryDays = true;
                        if (int.TryParse(textBoxNumberOfDays.Text, out Js.NumberOfDays))
                        {
                        }
                        else
                        {
                            MessageBox.Show(textBoxNumberOfDays.Text, "Please enter a valid number for number of days!");
                            return;
                        }
                        Js.RecurEveryWeekDay = false;
                    }

                    if (radioButtonRecurEveryWeekDay.Checked == true)
                    {
                        Js.RecurEveryWeekDay = true;
                    }

                }

                if (radioButtonRecurPerMinutes.Checked == true)
                {
                    // Read minutes 
                    Js.RecurPerMinutes = true; 

                    if (int.TryParse(numericUpDownRecurPerMinutes.Text, out Js.RecurPerMinutesValue))
                    {
                    }
                    else
                    {
                        MessageBox.Show(numericUpDownRecurPerMinutes.Text, "Please enter a valid number for number of days!");
                        return;
                    }
                }

            int InsertSeqNo = Js.InsertNewRecordInJTMEventSchedules();

            checkBoxAdd.Checked = false; 

            Form200bATMs_Load(this, new EventArgs());
        }
        // UPDATE 
        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            // Initialise fields

            if (Js.ScheduleID == "")
            {
                MessageBox.Show("Please enter Event Schedule ID!");
                return;
            }

            Js.DateLastUpdated = DateTime.Now;
            Js.UserId = WSignedId;

            Js.Recurrence = false;
            Js.RecurDaily = false;
            Js.RecurWeekly = false;
            Js.RecurMonthly = false;
            Js.RecurPerMinutes = false;
            Js.RecurEveryDays = false;
            Js.NumberOfDays = 0;
            Js.RecurEveryWeekDay = false;
            Js.RecurPerMinutesValue = 0;
            Js.Operator = WOperator;
            //
            // Set Standard Input 
            //       
            if (dateTimePicker1.Value < DateTime.Now)
            {
                MessageBox.Show("Please enter correct date and time for From!");
                return;
            }
            if (dateTimePicker4.Value < dateTimePicker1.Value)
            {
                MessageBox.Show("Please enter correct date and time for To!");
                return;
            }
            Js.EffectiveDateTmFrom = dateTimePicker1.Value;
            Js.EffectiveDateTmTo = dateTimePicker4.Value;

            Js.ScheduleID = textBoxScheduleID.Text;
            Js.EventType = comboBoxEventTypeSelect.Text;

            //
            // RECURRENCE
            //
          
                Js.Recurrence = true;

                if (radioButtonRecurDaily.Checked == true)
                {
                    // Daily selection 
                    Js.RecurDaily = true;

                    if (radioButtonRecurEveryDays.Checked == true)
                    {
                        Js.RecurEveryDays = true;
                        if (int.TryParse(textBoxNumberOfDays.Text, out Js.NumberOfDays))
                        {
                        }
                        else
                        {
                            MessageBox.Show(textBoxNumberOfDays.Text, "Please enter a valid number for number of days!");
                            return;
                        }
                        Js.RecurEveryWeekDay = false;
                    }
                    if (radioButtonRecurEveryWeekDay.Checked == true)
                    {
                        Js.RecurEveryWeekDay = true;
                    }

                }

                if (radioButtonRecurPerMinutes.Checked == true)
                {
                    // Read minutes 

                    Js.RecurPerMinutes = true; 

                    if (int.TryParse(numericUpDownRecurPerMinutes.Text, out Js.RecurPerMinutesValue))
                    {
                    }
                    else
                    {
                        MessageBox.Show(numericUpDownRecurPerMinutes.Text, "Please enter a valid number for number of days!");
                        return;
                    }
                }
           
            Js.UpdateRecordInJTMEventSchedules(WSeqNo);

            MessageBox.Show("Loading Schedule Updated!");

            // Read All ATMs with this schedule and update them 

            DateTime WDate; 

            RRDMJTMIdentificationDetailsClass Jd = new RRDMJTMIdentificationDetailsClass();

            string SelectionCriteria = " WHERE LoadingScheduleID ='" + Js.ScheduleID + "'";

            Jd.ReadJTMIdentificationDetailsToFillPartialTable(SelectionCriteria, 1, NullPastDate);

            int I = 0;

            while (I <= (Jd.ATMsJournalDetailsTable.Rows.Count - 1))
            {
                //ATMsJournalDetailsTable.Columns.Add("ATMNo", typeof(string));
                //ATMsJournalDetailsTable.Columns.Add("LoadingScheduleID", typeof(string));

                //ATMsJournalDetailsTable.Columns.Add("QueueRecId", typeof(string));
                //ATMsJournalDetailsTable.Columns.Add("FileUploadRequestDt", typeof(DateTime));
                //ATMsJournalDetailsTable.Columns.Add("FileParseEnd", typeof(DateTime));
                //ATMsJournalDetailsTable.Columns.Add("LoadingCompleted", typeof(DateTime));

                //ATMsJournalDetailsTable.Columns.Add("ResultCode", typeof(int));
                //ATMsJournalDetailsTable.Columns.Add("ResultMessage", typeof(string));
                //ATMsJournalDetailsTable.Columns.Add("NextLoadingDtTm", typeof(DateTime));
                //    RecordFound = true;
                string ATMNo = (string)Jd.ATMsJournalDetailsTable.Rows[I]["ATMNo"];

                Jd.ReadJTMIdentificationDetailsByAtmNo(ATMNo);

                Jd.DateLastUpdated = DateTime.Now;
                Jd.UserId = WSignedId;

                // Find Next Loading Date 

                // UPDATE NEXT LOADING 
                // Last loaded Date not available then insert current date 
                //
                if (Js.EffectiveDateTmFrom > DateTime.Now)
                {
                    WDate = Js.EffectiveDateTmFrom.AddDays(-1); 
                }
                else
                {
                    WDate = DateTime.Now;
                }

                
                //RRDMJTMEventSchedules Js = new RRDMJTMEventSchedules();
                DateTime NextLoading = Js.ReadCalculatedNextEventDateTm(WOperator, Jd.LoadingScheduleID,
                                                                           WDate, WDate);
                Jd.NextLoadingDtTm = NextLoading;

                // Update 

                Jd.UpdateRecordInJTMIdentificationDetailsByAtmNo(ATMNo); 

                I++; // Read Next entry of the table 

            }


            int WRowIndex = dataGridView1.SelectedRows[0].Index;

            Form200bATMs_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));
       
        }
        // DELETE 
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Warning: Do you want to delete this Loading Scedule?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                         == DialogResult.Yes)
            {
                Js.DeleteRecordInJTMIdentificationDetailsByAtmNo(WSeqNo);

                Form200bATMs_Load(this, new EventArgs());
            }
            else
            {
                return;
            }
        }
// Calculate Next Date 
        private void button1_Click(object sender, EventArgs e)
        {
            //
            // Last loaded Date not available then insert current date 
            //
            DateTime WDate;
            DateTime LastParsed; 

            if (dateTimePicker2.Value<Js.EffectiveDateTmFrom)
            {
                MessageBox.Show("Please enter date greater or equal than effective date");
                return ; 
            }
             
            if (Js.EffectiveDateTmFrom > DateTime.Now)
            {
                WDate = Js.EffectiveDateTmFrom.AddDays(-1);
            }
            else
            {
                WDate = DateTime.Now;
            }

            LastParsed = dateTimePicker2.Value; 

            //RRDMJTMEventSchedules Js = new RRDMJTMEventSchedules();
            DateTime NextLoading = Js.ReadCalculatedNextEventDateTm(WOperator, Js.ScheduleID,
                                                                       WDate, LastParsed);

        
            //DateTime NextLoading = Js.ReadCalculatedNextEventDateTm(WOperator, Js.ScheduleID, 
            //                                            DateTime.Now, dateTimePicker2.Value);
            textBox4.Text = NextLoading.ToString(); 
        }
//
// DELETE DAILY 
//
        private void radioButtonDeleteDaily_CheckedChanged(object sender, EventArgs e)
        {
            ShowPanelsDetails();
        }

        private void radioButtonDeleteWeekly_CheckedChanged(object sender, EventArgs e)
        {
            ShowPanelsDetails();
        }

        private void radioButtonDeleteMonthly_CheckedChanged(object sender, EventArgs e)
        {
            ShowPanelsDetails();
        }

        private void radioButtonDeletePerMinutes_CheckedChanged(object sender, EventArgs e)
        {
            ShowPanelsDetails();
        }
// ComboBox change 
        private void comboBoxEventTypeSelect_SelectedIndexChanged(object sender, EventArgs e)
        {      
            Form200bATMs_Load(this, new EventArgs());
        }
// Testing 
        private void checkBoxTesting_CheckedChanged(object sender, EventArgs e)
        {
           if (checkBoxTesting.Checked == true)
            {
                panel8.Show();
            }
           else
            {
                panel8.Hide(); 
            }
           
        }
// Check Box ADD
        private void checkBoxAdd_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAdd.Checked == true)
            {
                buttonAdd.Show();
                buttonUpdate.Hide();
                buttonDelete.Hide();
                textBoxScheduleID.Text = "";
                dateTimePicker1.Value = DateTime.Now;

            }
            else
            {
                buttonAdd.Hide();
                buttonUpdate.Show();
                buttonDelete.Show();
            }
        }
    }
}
