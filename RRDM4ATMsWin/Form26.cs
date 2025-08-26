using System;
using System.ComponentModel;
using System.Windows.Forms;
using RRDM4ATMs;


namespace RRDM4ATMsWin
{
    public partial class Form26 : Form
    {
        //   FormMainScreen NFormMainScreen;

        // NOTES 
        string Order;

        string WParameter4;
        string WSearchP4;

        //Form67 NForm67;
        RRDMCaptureCardsClass Cc = new RRDMCaptureCardsClass();
        RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
        RRDMCaseNotes Cn = new RRDMCaseNotes();

        DateTime LongFutureDate = new DateTime(2050, 11, 21);

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        int tempRowIndex;

        int WCaptNo;
        //int WTraceNo; 
        //string Gridfilter;

        int WTraceNo;

        int WReconcCycleNo; 

        string InputCardNo;

        string WSignedId;
        //int WSignRecordNo;
        string WOperator;

        string WInputText;
       
        DateTime WDateTmFrom;
        DateTime WDateTmTo;

        int WMode;

        string WAtmNo;
        int WSesNo;

        public Form26(string InSignedId, string InOperator, string InInputText
                         , DateTime InDateTmFrom
                         , DateTime InDateTmTo
                         , int InMode
                         )
        {
            WSignedId = InSignedId;
           // WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;


            WInputText = InInputText;

            WDateTmFrom = InDateTmFrom;
            WDateTmTo = InDateTmTo;

            WMode = InMode;

            InitializeComponent();

            string WJobCategory = "ATMs";

            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

            // Set Up the combobox. 

            // Districts
            RRDMGasParameters Gp = new RRDMGasParameters(); 
            Gp.ParamId = "269"; // Reasons 
            comboBox1.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBox1.DisplayMember = "DisplayValue";

            Gp.ParamId = "270"; // Origins
            comboBox2.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBox2.DisplayMember = "DisplayValue";

            buttonAdd.Hide(); 
 
            // Call Procedures

            labelATMno.Text = WAtmNo;
           
            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;

        }

        // ON LOAD OF FORM DO 
        private void Form26_Load(object sender, EventArgs e)
        {

            if (WMode == 21)
            {
                Cc.ReadCaptureCardsTableByDatesTm_Range(WOperator, WInputText,
                                 WDateTmFrom, WDateTmTo, WMode);
                // BRANCH
                labelListOfCaptureCards.Text = "CAPTURE CARDS FOR BRANCH:."
                    + WInputText + ".From:." + WDateTmFrom.ToShortDateString()
                    + ".To:." + WDateTmTo.ToShortDateString()
                    ;

            }
            if (WMode == 22)
            {
                // BANK
                Cc.ReadCaptureCardsTableByDatesTm_Range(WOperator, WInputText,
                                  WDateTmFrom, WDateTmTo, WMode);

                labelListOfCaptureCards.Text = "CAPTURE CARDS FOR BANK."
                    + ".From:." + WDateTmFrom.ToShortDateString()
                    + ".To:." + WDateTmTo.ToShortDateString();

            }
            if (WMode == 23)
            {
                // ATM 
                // BANK
                Cc.ReadCaptureCardsTableByDatesTm_Range(WOperator, WInputText,
                                  WDateTmFrom, WDateTmTo, WMode);

                labelListOfCaptureCards.Text = "CAPTURE CARDS FOR ATM:."
                    + WInputText + ".From:." + WDateTmFrom.ToShortDateString()
                    + ".To:." + WDateTmTo.ToShortDateString()
                    ;
            }
            if (WMode == 24)
            {
                // Full Card
                Cc.ReadCaptureCardsTableByDatesTm_Range(WOperator, WInputText,
                                  WDateTmFrom, WDateTmTo, WMode);

                labelListOfCaptureCards.Text = "CAPTURE CARDS FOR CARD:."
                   + WInputText + ".From:." + WDateTmFrom.ToShortDateString()
                   + ".To:." + WDateTmTo.ToShortDateString()
                   ;
            }

            bool WithDate = false;

            //Cc.ReadCaptureCardsTable(WOperator, WAtmNo, "", 0, NullPastDate, WithDate, 11) ;          

            //**************
            ShowGrid();
            //**************

            textBoxMsgBoard.Text = "Manage capture cards. Print and scan when needed";

            if (dataGridView1.Rows.Count == 0)
            {
                button2.Enabled = false;
            }
            else button2.Enabled = true;

        }

        //
        // Choose from Grid 
        //
        //int WRow;
        //int scrollPosition;
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            //WRow = e.RowIndex;
            //scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            WCaptNo = (int)rowSelected.Cells[0].Value;

            Cc.ReadCaptureCardBySeqNo(WCaptNo);

            if (checkBoxAddCaptured.Checked == false)
            {
                buttonNotes2.Show();
                label15.Show();
                labelNumberNotes2.Show();
            }
            else
            {
                buttonNotes2.Hide();
                label15.Hide();
                labelNumberNotes2.Hide();
            }
            
            comboBox1.Text = Cc.ReasonDesc;
            comboBox2.Text = Cc.Origin;

            checkBox1.Checked = Cc.Received ;

            dateTimePickerDateOfCaptured.Value = Cc.CaptDtTm; 


            Cc.ActionDtTm = NullPastDate;

            Cc.CustomerNm = "";

            Cc.ActionComments = "";

            Cc.ActionCode = 0;

            Cc.LoadedAtRMCycle = WReconcCycleNo;

            Cc.OpenRec = true;

            Cc.Received = checkBox1.Checked;

            if (checkBox1.Checked == true)
            {
                Cc.DateReceived = dateTimePicker1.Value;
            }
            else
            {
                Cc.DateReceived = NullPastDate;
            }

            Cc.Origin = comboBox2.Text;

            // NOTES for final comment 
            Order = "Descending";
            WParameter4 = "Capture Card No:" + WCaptNo;
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";

            ShowRow(WCaptNo); // Show Row   
        }

        //
        // TAKE ACTION
        //
        private void button2_Click(object sender, EventArgs e)
        {
            tempRowIndex = dataGridView1.SelectedRows[0].Index;

            if (radioButton4.Checked == false & radioButton5.Checked == false & radioButton7.Checked == false)
            {
                MessageBox.Show("Make your choice please.");
                return;
            }

            // IF REQUEST FOR UNDO ACTION 
            if (radioButton7.Checked == true)
            {
                //    OldPoss = Poss; 

                Cc.ReadCaptureCardBySeqNo(WCaptNo);

                Cc.ActionCode = 0;

                Cc.ActionComments = "";
                Cc.CustomerNm = "";
                Cc.ActionDtTm = LongFutureDate;
                Cc.OpenRec = true;

                Cc.UpdateCapturedCardSpecific(WCaptNo);

                ShowRow(WCaptNo); // Show Row 

                bool WithDate = false;

                Cc.ReadCaptureCardsTable(WOperator, WAtmNo, "", 0, NullPastDate, WithDate, 11);


                //**************
                ShowGrid();
                //**************


                textBoxMsgBoard.Text = " Action is cancelled ";

                return;
            }

            // DELIVERED To CUSTOMER 


            if (radioButton4.Checked == true)
            {
                // It will be delivered to customer 

                if (String.IsNullOrEmpty(textBox3usercommentold.Text) || String.IsNullOrEmpty(textBox2Name.Text)
                    || String.IsNullOrEmpty(textBox2.Text))
                {
                    MessageBox.Show("Comment , Full Card and Name must have Values");
                    return;
                }

                Cc.ReadCaptureCardBySeqNo(WCaptNo);

                Cc.CardNo = textBox2.Text; // Full Card No 
                Cc.ActionCode = 1;

                Cc.ActionComments = textBox3usercommentold.Text;
                Cc.CustomerNm = textBox2Name.Text;
                Cc.ActionDtTm = DateTime.Now;


                Cc.UpdateCapturedCardSpecific(WCaptNo);

                ShowRow(WCaptNo); // Show Row 

                PrintFormFromAction();

                DateTime NullPastDate = new DateTime(1900, 01, 01);

                bool WithDate = false;

                Cc.ReadCaptureCardsTable(WOperator, WAtmNo, "", 0, NullPastDate, WithDate, 11);


                //**************
                ShowGrid();
                //**************

                MessageBox.Show("Action Taken and captured card form printed");

                textBoxMsgBoard.Text = " Action Taken ";

                return;

            }

            // DELIVERED TO CARDS Department 

            if (radioButton5.Checked == true)
            {
                if (String.IsNullOrEmpty(textBox3usercommentold.Text) || String.IsNullOrEmpty(textBox2Name.Text))
                {
                    MessageBox.Show("Both User Comment And Name must have Values");
                    return;
                }

                Cc.ReadCaptureCardBySeqNo(WCaptNo);

                Cc.CardNo = textBox2.Text; // Full Card No 
                Cc.ActionCode = 2;

                Cc.ActionComments = textBox3usercommentold.Text;
                Cc.CustomerNm = textBox2Name.Text;
                Cc.ActionDtTm = DateTime.Now;


                Cc.UpdateCapturedCardSpecific(WCaptNo);

                ShowRow(WCaptNo); // Show Row 

                PrintFormFromAction();
                // SHOW Updated Grid

                DateTime NullPastDate = new DateTime(1900, 01, 01);

                bool WithDate = false;

                Cc.ReadCaptureCardsTable(WOperator, WAtmNo, "", 0, NullPastDate, WithDate, 11);


                //**************
                ShowGrid();
                //**************

                MessageBox.Show("Action Taken and captured card form printed");

                textBoxMsgBoard.Text = " Action Taken ";

                return;

            }

            // Handled By Branch  

            if (radioButton6.Checked == true)
            {
                if (String.IsNullOrEmpty(textBox3usercommentold.Text))
                {
                    MessageBox.Show("User Comment must have Value");
                    return;
                }

                Cc.ReadCaptureCardBySeqNo(WCaptNo);

                Cc.CardNo = textBox2.Text; // Full Card No 
                Cc.ActionCode = 3;

                Cc.ActionComments = textBox3usercommentold.Text;
                Cc.CustomerNm = "N/A";
                Cc.ActionDtTm = DateTime.Now;

                Cc.UpdateCapturedCardSpecific(WCaptNo);

                ShowRow(WCaptNo); // Show Row 

                PrintFormFromAction();

                // SHOW Updated Grid

                bool WithDate = false;

                Cc.ReadCaptureCardsTable(WOperator, WAtmNo, "", 0, NullPastDate, WithDate, 11);



                //**************
                ShowGrid();
                //**************


                MessageBox.Show("Action Taken and captured card form printed");

                textBoxMsgBoard.Text = " Branch will Act ";

                return;

            }
        }

     

        //******************
        // SHOW GRID
        //******************
        private void ShowGrid()
        {
          
            dataGridView1.DataSource = Cc.CapturedCardsDataTable.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("No Capture Cards For this selection");

                button2.Enabled = false;

                this.Dispose();

                return;
            }
            else button2.Enabled = true;

            dataGridView1.Columns[0].Width = 70; // CapturedNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Descending);

            dataGridView1.Columns[1].Width = 70; // AtmNo
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 70; // ReplCycle
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[3].Width = 100; // CardNo
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;


            if (dataGridView1.Rows.Count == 0)
            {
                button2.Enabled = false;
            }
            else button2.Enabled = true;

            //dataGridView1.Rows[WRow].Selected = true;
            //dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));

            //dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
        }

        // Show ROW
        private void ShowRow(int InCaptNo)
        {
            Cc.ReadCaptureCardBySeqNo(InCaptNo);
            WAtmNo = Cc.AtmNo; 
            label7.Text = Cc.CardNo;
            label11.Text = Cc.ReasonDesc;
            textBoxBranch.Text = Cc.BranchId; 
            textBoxCardNumber.Text = Cc.CardNo; 
            textBoxAtm.Text = Cc.AtmNo;
            dateTimePickerDateOfCaptured.Value = Cc.CaptDtTm.Date; 

            if (Cc.ActionDtTm == LongFutureDate || Cc.ActionDtTm == NullPastDate)
            {
                label17.Hide();
                label18.Hide();

                pictureBox3.Show();
            }
            else
            {
                label17.Show();
                label18.Show();
                label18.Text = Cc.ActionDtTm.ToString();
                pictureBox3.Hide();
            }

            //   WTraceNo = Cc.TraceNo;

            WTraceNo = Cc.TraceNo;


            if (Cc.ActionCode == 1) radioButton4.Checked = true;
            if (Cc.ActionCode == 2) radioButton5.Checked = true;
            if (Cc.ActionCode == 3) radioButton6.Checked = true;

            if (Cc.ActionCode == 0)
            {
                radioButton4.Checked = false;
                radioButton5.Checked = false;
                radioButton6.Checked = false;
                radioButton7.Checked = false;
            }

            textBox3usercommentold.Text = Cc.ActionComments;
            textBox2Name.Text = Cc.CustomerNm;
            textBox2.Text = Cc.CardNo;

            if (Cc.ActionDtTm == LongFutureDate)
            {
                textBoxMsgBoard.Text = " Action not taken yet for this card.";
            }
            else
            {
                textBoxMsgBoard.Text = " Action taken for this card.";
            }
        }


        //
        // Based on Transaction No show video clip
        //
        private void button3_Click(object sender, EventArgs e)
        {
            VideoWindow videoForm = new VideoWindow();
            videoForm.ShowDialog();
        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        //
        // Locate Card In EJournal 
        // 
        private void button5_Click(object sender, EventArgs e)
        {
            int WFuId = 0;
            int WSeqNoA = 0;
            int WSeqNoB = 0;

            if (WTraceNo == 0)
            {
                MessageBox.Show("There is no trace to locate the Journal information. ");
                return; 
            }

            RRDMJournalReadTxns_Text_Class Ej = new RRDMJournalReadTxns_Text_Class();
            Ej.ReadJournalTxnsByTraceAndFind_Start_End(WOperator, WAtmNo, WTraceNo
                                                  , Cc.CaptDtTm.Date);
            if (Ej.RecordFound == true)
            {
                WFuId = Ej.FuId;

                WSeqNoA = WSeqNoB = Ej.SeqNo;
            }
          
            //
            Form67_BDC NForm67_BDC;

            int Mode = 5; // Specific
            string WTraceRRNumber = WTraceNo.ToString();
          
            NForm67_BDC = new Form67_BDC(WSignedId, 0, WOperator, WFuId, WTraceRRNumber, WAtmNo, WSeqNoA, WSeqNoB, Cc.CaptDtTm.Date, NullPastDate, Mode);
            NForm67_BDC.ShowDialog();


            //String JournalId = "[ATMS_Journals].[dbo].[tblHstEjText]";

            //Mode = 5; // Specific

            //NForm67 = new Form67(WSignedId, 0 , WOperator, 0, WAtmNo, WTraceNo, WTraceNo, Cc.CaptDtTm.Date, NullPastDate, Mode);
            //NForm67.Show();
        }
        //
        // Print Captured Card Form
        //
        private void PrintFormFromAction()
        {
            RRDMAtmsClass Ac = new RRDMAtmsClass();
            Ac.ReadAtm(WAtmNo);
            RRDMUsersRecords Us = new RRDMUsersRecords();
            Us.ReadUsersRecord(WSignedId);
            RRDMBanks Ba = new RRDMBanks();
            Ba.ReadBank(WOperator);

            String P1 = Cc.CardNo;
            String P2 = Cc.CustomerNm;
            String P3 = Cc.CaptDtTm.ToString();
            String P4 = Cc.ReasonDesc;
            String P5 = WOperator;
            String P6 = Ba.BankName;
            String P7 = Ac.BranchName;
            String P8 = WAtmNo;
            String P9 = Cc.ActionComments;
            String P10 = Us.UserName;
            String P11 = "";
            String P12 = "";
            String P13 = "";
            if (Cc.ActionCode == 3)
            {
                P11 = "X";
                P12 = "";
                P13 = "";
            }
            if (Cc.ActionCode == 2)
            {
                P11 = "";
                P12 = "X";
                P13 = "";
            }
            if (Cc.ActionCode == 1)
            {
                P11 = "";
                P12 = "";
                P13 = "X";
            }

            Form56R2 ReportCaptured = new Form56R2(P1, P2, P3, P4, P5, P6, P7,
                                  P8, P9, P10, P11, P12, P13, Ba.BankId);
            ReportCaptured.Show();

        }

        // FINISH 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
// NOTES 
        private void buttonNotes2_Click(object sender, EventArgs e)
        {
           
            Form197 NForm197;
            string WParameter3 = "";
            WParameter4 = "Capture Card No:" + WCaptNo;
            string SearchP4 = "";
            //if (ViewWorkFlow == true) WMode = "Read";
            //else WMode = "Update";
            string WMode = "Update";
            NForm197 = new Form197(WSignedId, 0, WOperator, "", WParameter3, WParameter4, WMode, SearchP4);
            NForm197.ShowDialog();

            // NOTES 
            Order = "Descending";
            WParameter4 = "Capture Card No:" + WCaptNo;
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";
        }
// ADD Captured
        private void checkBoxAddCaptured_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAddCaptured.Checked == true)
            {
                buttonAdd.Show();
                buttonUpdate.Hide();
                //buttonDelete.Hide();

                textBoxCardNumber.Text = "";
                textBoxAtm.Text = "";
                checkBox1.Checked = false;

                buttonNotes2.Hide();
                label15.Hide();
                labelNumberNotes2.Hide(); 
            }
            else
            {
                buttonAdd.Hide();
                buttonUpdate.Show();
                //buttonDelete.Show();

                //dataGridView2.Enabled = true;
                int WRowIndex1 = -1;

                if (dataGridView1.Rows.Count > 0)
                {
                    WRowIndex1 = dataGridView1.SelectedRows[0].Index;
                }

                Form26_Load(this, new EventArgs());

                if (dataGridView1.Rows.Count > 0)
                {
                    dataGridView1.Rows[WRowIndex1].Selected = true;
                    dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex1));
                }

            }
        }
// Update Capture Card
        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            Cc.ReadCaptureCardBySeqNo(WCaptNo);

            if (textBoxCardNumber.Text == "")
            {
                MessageBox.Show("Please input the Card Number"); 
                return;
            }
            Cc.CardNo = textBoxCardNumber.Text;

            Cc.AtmNo = textBoxAtm.Text;

            Cc.BranchId = textBoxBranch.Text;

            Cc.ReasonDesc = comboBox1.Text;

            Cc.Received = checkBox1.Checked;

            if (checkBox1.Checked == true)
            {
                Cc.DateReceived = dateTimePicker1.Value;
            }
            else
            {
                Cc.DateReceived = NullPastDate;
            }

            Cc.Origin = comboBox2.Text;


            Cc.UpdateCapturedCardSpecific(WCaptNo);

            Form26_Load(this, new EventArgs());

        }
// ADD CARD
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (textBoxCardNumber.Text == "")
            {
                MessageBox.Show("Please input the Card Number");
                return;
            }

            Cc.ReadCaptureCardByCardNo(textBoxCardNumber.Text); 

            if (Cc.RecordFound == true)
            {
                MessageBox.Show("This Card Already Exist");
                return;
            }

            Cc.CardNo = textBoxCardNumber.Text;

            Cc.AtmNo = textBoxAtm.Text;

            Cc.ReasonDesc = comboBox1.Text;
            
            Cc.BankId = WOperator; 
            Cc.Operator = WOperator;

            Cc.SesNo = 0;
            Cc.BranchId = textBoxBranch.Text;

            Cc.TraceNo = 0;

            Cc.MasterTraceNo = 0;

            Cc.CaptDtTm = dateTimePickerDateOfCaptured.Value.AddHours(18);

            Cc.CaptureCd = 0;

            Cc.ReasonDesc = comboBox1.Text;

            Cc.ActionDtTm = NullPastDate;

            Cc.CustomerNm = "";

            Cc.ActionComments = "";

            Cc.ActionCode = 0;

            Cc.LoadedAtRMCycle = WReconcCycleNo ;

            Cc.OpenRec = true;

            Cc.Received = checkBox1.Checked; 

            if (checkBox1.Checked == true)
            {
                Cc.DateReceived = dateTimePicker1.Value; 
            }
            else
            {
                Cc.DateReceived = NullPastDate;
            }

            Cc.Origin = comboBox2.Text; 

            //cmd.Parameters.AddWithValue("@Received", Received);
            //cmd.Parameters.AddWithValue("@DateReceived", DateReceived);
            //cmd.Parameters.AddWithValue("@Origin", Origin);

            Cc.InsertCapturedCard_From_Form26(textBoxAtm.Text);

            MessageBox.Show("Card Added ... Add Note if you want "); 

            checkBoxAddCaptured.Checked = false;

            //Form26_Load(this, new EventArgs());
        }
    }
}
