using System;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Configuration;
// Alecos
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;

namespace RRDM4ATMsWin
{
    public partial class Form200cATMs : Form
    {
        // Variables

        Form54 NForm54;
        Form55 NForm55;

        string MsgFilter;

        bool IsJournalsReadFromHost;

        int WMode;

        DateTime WDateTime;

        RRDMJTMEventSchedules Js = new RRDMJTMEventSchedules();
        RRDMJTMIdentificationDetailsClass Jd = new RRDMJTMIdentificationDetailsClass();
        RRDMJTMQueue Jq = new RRDMJTMQueue();

        RRDMReconcFileMonitorLog Flog = new RRDMReconcFileMonitorLog();

        //RRDMBanks Ba = new RRDMBanks();

        RRDMUsersAccessToAtms Ua = new RRDMUsersAccessToAtms();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMControllerMsgClass Cm = new RRDMControllerMsgClass();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        DateTime NullFutureDate = new DateTime(2050, 11, 21);

        string WSelectionCriteria;

        string WSignedId;
        int WSignRecordNo;
        string WSecLevel;
        string WOperator;
        string WAtmNo;

        // Methods 
        // READ ATMs Main
        // 
        public Form200cATMs(string InSignedId, int SignRecordNo, string InSecLevel, string InOperator, string InAtmNo)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;
            WAtmNo = InAtmNo;

            InitializeComponent();

            // Set Working Date 
            RRDMGasParameters Gp = new RRDMGasParameters();
            string ParId = "267";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            string TestingDate = Gp.OccuranceNm;
            if (TestingDate == "YES")
                labelToday.Text = new DateTime(2017, 03, 01).ToShortDateString();
            labelToday.Text = DateTime.Now.ToShortDateString();

            // Are journals found in a Directory at HOST
            ParId = "265";
            OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            string Journals = Gp.OccuranceNm;

            if (Gp.OccuranceNm == "YES") IsJournalsReadFromHost = true;
            else IsJournalsReadFromHost = false;

            pictureBox1.BackgroundImage = appResImg.logo2;

            labelUserId.Text = WSignedId;

            //*****************************************
            //
            //labelStep1.Text = "E-Journal Loading Schedules";
            //
            //*****************************************
            if (WAtmNo == "")
            {
                WSelectionCriteria = "WHERE Operator ='" + WOperator + "'";
            }
            else
            {
                WSelectionCriteria = "WHERE Operator ='" + WOperator + "' AND AtmNo ='" + WAtmNo + "'";
                label10.Hide();
                textBox10.Hide();
                buttonSelectATM.Hide();
                label1.Hide();
                numericUpDownHours.Hide();
                buttonHours.Hide();
                buttonRefresh.Hide();

            }

            WMode = 1;
            WDateTime = NullPastDate;

            textBoxMsgBoard.Text = "ATMs Loading Activity.";
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

            printDocument1.PrintPage +=
            new PrintPageEventHandler(printDocument1_PrintPage);

        }
        //
        // LOAD
        //
        private void Form200bATMs_Load(object sender, EventArgs e)
        {
            // FILL DataGrid

            Jd.ReadJTMIdentificationDetailsToFillPartialTable(WSelectionCriteria, WMode, WDateTime);

            dataGridView1.DataSource = Jd.ATMsJournalDetailsTable.DefaultView;

            dataGridView1.Columns[0].Width = 80; // 
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 250; // 
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[2].Width = 80; // QueueId
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[3].Width = 120; // 
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[4].Width = 120; // 
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[5].Width = 120; // 
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[6].Width = 80; // Result Code 
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[7].Width = 130; // 
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[8].Width = 120; // 
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }
        //
        // ROW ENTER FOR DATA GRID
        //
        private void dataGridView1_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WAtmNo = (string)rowSelected.Cells[0].Value;

            if (IsJournalsReadFromHost == false)
            {
                EJFromATMs();
            }
            else
            {
                EJFromHOST();
            }
        }

        // ROW ENTER 
        int WSeqNo;
        int WFuid;
        string WJournalTxtFile;
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            WSeqNo = (int)rowSelected.Cells[0].Value;

            Flog.GetRecordBySeqNo(WSeqNo);

            WFuid = Flog.stpFuid;

            if (Flog.SystemOfOrigin == "ATMs")
            {
                WJournalTxtFile = Flog.ArchivedPath;
                buttonJournal.Show();
            }
            else
            {
                buttonJournal.Hide();
            }
        }

        private void EJFromATMs()
        {
            Jq.ReadJTMQueueByATMAndFillTable(WAtmNo);

            if (Jq.QueueJournalTable.Rows.Count > 0)
            {

                dataGridView2.DataSource = Jq.QueueJournalTable.DefaultView;

                dataGridView2.Columns[0].Width = 60; // 
                dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView2.Columns[1].Width = 60; // 
                dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView2.Columns[2].Width = 120; // 
                dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView2.Columns[3].Width = 80; // RequestorID
                dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView2.Columns[4].Width = 80; // 
                dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView2.Columns[5].Width = 70; // Type Of Journal 
                dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView2.Columns[6].Width = 60; // Stage
                dataGridView2.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView2.Columns[7].Width = 60; // 
                dataGridView2.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView2.Columns[8].Width = 120; // Result Message
                dataGridView2.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView2.Columns[8].Visible = false;

                dataGridView2.Columns[9].Width = 120; // date 1
                dataGridView2.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView2.Columns[10].Width = 120; // date 2
                dataGridView2.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView2.Columns[11].Width = 120; // date 3
                dataGridView2.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView2.Columns[12].Width = 120; // date 4
                dataGridView2.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView2.Columns[13].Width = 120; // date 5
                dataGridView2.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView2.Columns[14].Width = 120; // date 6
                dataGridView2.Columns[14].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            }
            else
            {
                dataGridView2.DataSource = null;
                dataGridView2.Refresh();
            }
        }

        private void EJFromHOST()
        {
            string WSelectionCriteria = "where FileName Like '%" + WAtmNo + "%' AND Status = 1 ";
            Flog.ReadDataTableFileMonitorLogByAtmNo(WOperator, WSignedId, WSelectionCriteria);

            dataGridView2.DataSource = Flog.DataTableFileMonitorLog.DefaultView;

            if (dataGridView2.Rows.Count > 0)
            {
                // Show Grid 
                buttonFromSource.Show();
                buttonJournal.Show();

            }
            else
            {
                dataGridView2.DataSource = null;
                dataGridView2.Refresh();

                buttonFromSource.Hide();
                buttonJournal.Hide(); 
            }
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
        // AN ATM WAS SELECTED
        private void buttonSelectATM_Click(object sender, EventArgs e)
        {
            WSelectionCriteria = " WHERE AtmNo ='" + textBox10.Text + "' AND Operator ='" + WOperator + "'";
            WMode = 1;
            WDateTime = NullPastDate;

            Form200bATMs_Load(this, new EventArgs());

        }
        // Refresh
        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            WSelectionCriteria = " WHERE Operator ='" + WOperator + "'";
            WMode = 1;
            WDateTime = NullPastDate;

            textBox10.Text = "";

            Form200bATMs_Load(this, new EventArgs());
        }
        // View Loaded Journals Before Houre 
        private void buttonHours_Click(object sender, EventArgs e)
        {
            int TempHours;

            if (int.TryParse(numericUpDownHours.Text, out TempHours))
            {
            }
            else
            {
                return;
            }

            WSelectionCriteria = " WHERE LoadingCompleted < @Date AND Operator ='" + WOperator + "'";
            WMode = 2;
            WDateTime = DateTime.Now.AddHours(-TempHours);

            Form200bATMs_Load(this, new EventArgs());
        }
        private Button printButton;
        private PrintDocument printDocument1 = new PrintDocument();
        private string stringToPrint;
        string CopiedFile;

        // SHOW Journal 
        private void buttonJournal_Click(object sender, EventArgs e)
        {

            Form67_BDC NForm67_BDC;
            int Mode = 3;
            Mode = 3; // Specific Journal 
            if (WFuid > 0)
            {
                NForm67_BDC = new Form67_BDC(WSignedId, 0, WOperator, WFuid, "", WAtmNo, 0, 0, NullPastDate, NullPastDate, Mode);
                NForm67_BDC.ShowDialog();
            }
            else
            {
                MessageBox.Show("Not Able to show Journal");
            }


        }

        // Journal From SOURCE file 
        private void buttonFromSource_Click(object sender, EventArgs e)
        {
          //  RRDMJournalReadTxns_Text_Class Jt = new RRDMJournalReadTxns_Text_Class();

            RRDMGasParameters Gp = new RRDMGasParameters();

            string ParId = "920";
            string OccurId = "7";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            string WorkingDirectory = Gp.OccuranceNm;

            RRDM_EXCEL_AND_Directories Ed = new RRDM_EXCEL_AND_Directories();

            CopiedFile = Ed.CopyFileFromOneDirectoryToAnother(WJournalTxtFile, WorkingDirectory);

            MessageBox.Show("New File is copied to working directory.." + Environment.NewLine
                              + CopiedFile);

            //if (MessageBox.Show("Do you want to print the Journal form the source??" + Environment.NewLine
            //                   + CopiedFile + Environment.NewLine
            //                 + "?  "
            //                 , "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            //          == DialogResult.Yes)
            //{
            //    // YES Proceed
            //    ReadFile(CopiedFile);
            //    printDocument1.Print();

            //}
            //else
            //{
            //    return;
            //}

        }

        private void ReadFile(string InCopiedFile)
        {

            using (FileStream stream = new FileStream(InCopiedFile, FileMode.Open))
            using (StreamReader reader = new StreamReader(stream))
            {
                stringToPrint = reader.ReadToEnd();
            }
        }

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            int charactersOnPage = 0;
            int linesPerPage = 0;

            // Sets the value of charactersOnPage to the number of characters 
            // of stringToPrint that will fit within the bounds of the page.
            e.Graphics.MeasureString(stringToPrint, this.Font,
                e.MarginBounds.Size, StringFormat.GenericTypographic,
                out charactersOnPage, out linesPerPage);

            // Draws the string within the bounds of the page
            e.Graphics.DrawString(stringToPrint, this.Font, Brushes.Black,
                e.MarginBounds, StringFormat.GenericTypographic);

            // Remove the portion of the string that has been printed.
            stringToPrint = stringToPrint.Substring(charactersOnPage);

            // Check to see if more pages are to be printed.
            e.HasMorePages = (stringToPrint.Length > 0);
        }
    }
}

