using System;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

// Alecos

namespace RRDM4ATMsWin
{
    public partial class Form80bNV : Form
    {
        RRDMNVBanksNostroVostro Bnv = new RRDMNVBanksNostroVostro();
        RRDMGasParameters Gp = new RRDMGasParameters();
        RRDMAccountsClass Acc = new RRDMAccountsClass();

        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();

        RRDMNVStatement_Lines_InternalAndExternal Se = new RRDMNVStatement_Lines_InternalAndExternal();

        RRDMMatchingCategories Mc = new RRDMMatchingCategories();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMCaseNotes Cn = new RRDMCaseNotes();

        // NOTES START
        string Order;

        string WSearchP4;
        string WMode;
        // NOTES END 

        DateTime FromDt;
        DateTime ToDt;

        int Mode;

        string Ccy;
        string InternalAcc;
        string ExternalAcc;
        //
      
        string W4DigitMainCateg;

        int WSeqNo;

        string UserBank;

        bool ValidBankAccs; 

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WSubSystem; 
        string WCategoryId;
        int WReconcCycleNo;
        int WInMaskRecordId;
        string WFunction;
        DateTime WFromDt;
        DateTime WToDt;

        public Form80bNV(string InSignedId, int InSignRecordNo, string InOperator, string InSubSystem ,string InCategoryId, int InReconcCycleNo,
                                                  int InMaskRecordId, string InFunction, DateTime InFromDt, DateTime InToDt)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
            WSubSystem = InSubSystem;
            WCategoryId = InCategoryId; // Specific or ALL
            WReconcCycleNo = InReconcCycleNo;

            WInMaskRecordId = InMaskRecordId; // Coming from externally 

            WFunction = InFunction; // "Reconc", "View", "Investigation", "MainMenu"
                                    // "ViewAllAlerts", "ViewAgeingRange"
            WFromDt = InFromDt;
            WToDt = InToDt;

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;

            ValidBankAccs = false; 

            Us.ReadUsersRecord(InSignedId);

            UserBank = Us.BankId;

            //External Bank Names 
            comboBoxExternalBank.DataSource = Bnv.GetExternalBanksNames(WOperator);
            comboBoxExternalBank.DisplayMember = "DisplayValue";

            if (WCategoryId !="ALL")
            {
                W4DigitMainCateg = WCategoryId.Substring(0, 4);
                //Find Internal and External Account 
                Mc.ReadMatchingCategorybyActiveCategId(WOperator, WCategoryId);
                if (W4DigitMainCateg == "EWB8")
                {
                    Ccy = Mc.VostroCurr;
                    InternalAcc = Mc.InternalAcc;
                    ExternalAcc = Mc.VostroAcc;

                    comboBoxExternalBank.Text = Mc.VostroBank;
                    comboBoxExternalAccount.Text = ExternalAcc;
                    textBoxVostroCcy.Text = Mc.VostroCurr;
                    textBoxInternalAccount.Text = InternalAcc;

                    comboBoxExternalBank.Enabled = false;
                    comboBoxExternalAccount.Enabled = false;
                    textBoxVostroCcy.Text = Mc.VostroCurr;
                    textBoxInternalAccount.Text = InternalAcc;

                    ValidBankAccs = true; 
                }
            }
            else
            {
                // ALL CATEGORIES 
            }

            comboBoxFilter.Items.Add("Internal Statement");
            comboBoxFilter.Items.Add("External Statement");
            comboBoxFilter.Items.Add("Matched");
            comboBoxFilter.Items.Add("UnMatched");
            comboBoxFilter.Items.Add("Txns With Alerts");
            comboBoxFilter.Items.Add("ALL");

            comboBoxFilter.Text = "Internal Statement";

            //TEST
            dateTimePicker1.Value = new DateTime(2014, 02, 10);
            dateTimePicker2.Value = new DateTime(2015, 02, 25);

            if ( WFunction == "ViewAgeingRange" )
            {
                dateTimePicker1.Value = WFromDt;
                dateTimePicker2.Value = WToDt;
                comboBoxFilter.Text = "UnMatched";
            }

            panel5.Hide();

        }
        // Load 
        private void Form80b_Load(object sender, EventArgs e)
        {
            if (WFunction == "ViewAllAlerts" & WCategoryId == "ALL")
            {
                label16.Hide(); 
                panel4.Hide();
                labelStep1.Text = "All Outstanding Alerts";
                comboBoxFilter.Text = "Txns With Alerts";
                if (comboBoxFilter.Text == "Txns With Alerts")
                {
                    FromDt = dateTimePicker1.Value.AddDays(-1);
                    ToDt = dateTimePicker2.Value.AddDays(1);
                    Mode = 16;
                    Se.ReadNVStatements_LinesByRangeAndDates(WOperator, WSignedId, Mode,
                                    ExternalAcc, InternalAcc, FromDt, ToDt, 0);

                    if (Se.TableNVStatement_Lines_Both.Rows.Count == 0)
                    {
                        // Do 
                        MessageBox.Show("No Entries to show");
                        return;
                    }
                    else
                    {
                        ShowGridUnMatched();
                    }
                }

                return; 
            }

            if (ValidBankAccs == false)
            {
                Bnv.ReadBank(comboBoxExternalBank.Text);
                if (Bnv.RecordFound == true)
                {
                    Acc.ReadAndFindAccountSpecificForNostroVostro(comboBoxExternalAccount.Text, comboBoxExternalBank.Text);
                    if (Acc.RecordFound == true)
                    {
                        ValidBankAccs = true;
                        Ccy = textBoxVostroCcy.Text;
                        InternalAcc = textBoxInternalAccount.Text;
                        ExternalAcc = comboBoxExternalAccount.Text;
                    }
                    else ValidBankAccs = false;
                }
                else ValidBankAccs = false;
            }
           
            if (ValidBankAccs == true)
            {
                if (checkBoxUnique.Checked == false) // Not Unique 
                {
                    if (comboBoxFilter.Text == "Internal Statement")
                    {
                        Mode = 11;
                        FromDt = dateTimePicker1.Value.AddDays(-1);
                        ToDt = dateTimePicker2.Value.AddDays(1);
                        Se.ReadNVStatements_LinesByRangeAndDates(WOperator, WSignedId, WSubSystem ,Mode,
                                        ExternalAcc, InternalAcc, FromDt, ToDt, "", 0, 0);
                        if (Se.TableNVStatement_Lines_Single.Rows.Count == 0)
                        {
                            return;
                        }
                        else
                        {
                            ShowGridStatement();
                        }
                    }

                    if (comboBoxFilter.Text == "External Statement")
                    {
                        Mode = 12;
                        FromDt = dateTimePicker1.Value.AddDays(-1);
                        ToDt = dateTimePicker2.Value.AddDays(1);
                        Se.ReadNVStatements_LinesByRangeAndDates(WOperator, WSignedId, WSubSystem ,Mode,
                                        ExternalAcc, InternalAcc, FromDt, ToDt, "", 0, 0);

                        if (Se.TableNVStatement_Lines_Single.Rows.Count == 0)
                        {
                            MessageBox.Show("No Entries to show");
                            return;
                        }
                        else
                        {
                            ShowGridStatement();
                        }

                    }

                    if (comboBoxFilter.Text == "Matched")
                    {

                        FromDt = dateTimePicker1.Value.AddDays(-1);
                        ToDt = dateTimePicker2.Value.AddDays(1);
                        Mode = 13;
                        Se.ReadNVStatements_LinesByRangeAndDates(WOperator, WSignedId, Mode,
                                        ExternalAcc, InternalAcc, FromDt, ToDt, 0);

                        if (Se.TableNVStatement_Lines_Both.Rows.Count == 0)
                        {
                            MessageBox.Show("No Entries to show");
                            return;
                        }
                        else
                        {
                            ShowGridMatched();
                        }

                    }

                    if (comboBoxFilter.Text == "UnMatched")
                    {
                        if (WFunction == "ViewAgeingRange")
                        {
                            FromDt = dateTimePicker1.Value.AddDays(-1);

                            ToDt = dateTimePicker2.Value.AddDays(1);
                            Mode = 27;
                            Se.ReadNVStatements_LinesByRangeAndDates(WOperator, WSignedId, Mode,
                                            ExternalAcc, InternalAcc, FromDt, ToDt, 0);

                            if (Se.TableNVStatement_Lines_Both.Rows.Count == 0)
                            {
                                // Do 
                                MessageBox.Show("No Entries to show");
                                return;
                            }
                            else
                            {
                                ShowGridUnMatched();
                            }
                        }
                        else
                        {
                            FromDt = dateTimePicker1.Value.AddDays(-1);
                            ToDt = dateTimePicker2.Value.AddDays(1);
                            Mode = 14;
                            Se.ReadNVStatements_LinesByRangeAndDates(WOperator, WSignedId, Mode,
                                            ExternalAcc, InternalAcc, FromDt, ToDt, 0);

                            if (Se.TableNVStatement_Lines_Both.Rows.Count == 0)
                            {
                                // Do 
                                MessageBox.Show("No Entries to show");
                                return;
                            }
                            else
                            {
                                ShowGridUnMatched();
                            }
                        }
                      

                    }

                    if (comboBoxFilter.Text == "Txns With Alerts" & WCategoryId != "ALL")
                    {
                        FromDt = dateTimePicker1.Value.AddDays(-1);
                        ToDt = dateTimePicker2.Value.AddDays(1);

                        Mode = 15;
                      
                        Se.ReadNVStatements_LinesByRangeAndDates(WOperator, WSignedId, Mode,
                                        ExternalAcc, InternalAcc, FromDt, ToDt, 0);

                        if (Se.TableNVStatement_Lines_Both.Rows.Count == 0)
                        {
                            // Do 
                            MessageBox.Show("No Entries to show");
                            return;
                        }
                        else
                        {
                            ShowGridUnMatched();
                        }
                    }

                }
                else
                {
                    
                }
            }
            else
            {
                // Refresh Grid 
                dataGridView1.DataSource = null;
                dataGridView1.Refresh();
                textBoxMsgBoard.Text = "Make a selection please of Bank and accounts"; 
            }
           
        }

        // On ROW ENTER 
        string WOrigin;
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WSeqNo = (int)rowSelected.Cells[0].Value;

            if (comboBoxFilter.Text == "Matched" || comboBoxFilter.Text == "UnMatched" 
                                         || comboBoxFilter.Text == "Txns With Alerts")
            {
                WOrigin = (string)rowSelected.Cells[3].Value;

                string SelectionCriteria = " WHERE SeqNo =" + WSeqNo + " AND Origin ='" + WOrigin + "'";

                Se.ReadNVStatements_Lines_BySelectionCriteria(SelectionCriteria);

                int WColorNo = (int)rowSelected.Cells[1].Value;

                if (WColorNo == 11)
                {
                    dataGridView1.DefaultCellStyle.SelectionBackColor = Color.DimGray;
                }
                else if (WColorNo == 12)
                {
                    dataGridView1.DefaultCellStyle.SelectionBackColor = Color.DodgerBlue;
                }

                if (Se.IsException == true)
                {
                    dataGridView1.DefaultCellStyle.SelectionBackColor = Color.DarkOrange;
                    AlertDetails(); // Show Alert Details 
                }
                else
                {
                    textBoxAlert.Hide();
                    linkLabelDispute.Hide();
                }
            }

            if (comboBoxFilter.Text == "Internal Statement" 
                || comboBoxFilter.Text == "External Statement"
                  || comboBoxFilter.Text == "ALL"
                )
            {
               
                WOrigin = (string)rowSelected.Cells[4].Value;

                string SelectionCriteria = " WHERE SeqNo =" + WSeqNo + " AND Origin ='" + WOrigin + "'";

                Se.ReadNVStatements_Lines_BySelectionCriteria(SelectionCriteria);
                
                string WStatus = (string)rowSelected.Cells[1].Value;

                if (WStatus == "M")
                {
                    dataGridView1.DefaultCellStyle.SelectionBackColor = Color.DimGray;
                }
                else if (WStatus == "U")
                {
                    dataGridView1.DefaultCellStyle.SelectionBackColor = Color.DodgerBlue;
                }

                if (Se.IsException == true)
                {
                    dataGridView1.DefaultCellStyle.SelectionBackColor = Color.DarkOrange;
                    AlertDetails(); // Show Alert Details 
                }
                else
                {
                    textBoxAlert.Hide();
                    linkLabelDispute.Hide();
                }
            }
        }

        // Alert Details
        int WDispNo; 
        private void AlertDetails()
        {
            textBoxAlert.Show();


            if (Se.ExceptionNo > 0)
            {
                WDispNo = Se.ExceptionNo;
                linkLabelDispute.Show();
            }
            else
            {
                linkLabelDispute.Hide();
            }

            if (Se.ExceptionId == 12)
            {
                //int DiffInEntryDays = Convert.ToInt32((Se.StmtLineEntryDate - WWorkingDate).TotalDays);
                //int AbsDiffInEntryDays = Math.Abs(DiffInEntryDays);
                if (WDispNo > 0)
                {
                    textBoxAlert.Text = "There Is Alert on Value date for this transaction." + Environment.NewLine
                                       + "A Dispute was opened.";
                }
                else
                {
                    textBoxAlert.Text = "There Is Alert on Value date for this transaction. No dispute is opened yet. ";
                }
            }
            if (Se.ExceptionId == 47)
            {
                textBoxAlert.Text = "Difference In Amt";
            }
            if (Se.ExceptionId == 14)
            {
                textBoxAlert.Text = "Fantom txn";
            }
            if (Se.ExceptionId == 15)
            {
                textBoxAlert.Text = "Transaction omited by teller";
            }
            if (Se.ExceptionId == 88)
            {
                textBoxAlert.Text = "No reply yet from External Bank - Account ";
            }

            // 01 We appear not to have debited so far
            // 02 We appear not to have credited so far  
            // 06 This transaction does not appear in your statement of account 
            // 47 Difference in amount - open Dispute and sent 995  
            // 12 Difference in Value Date 
            // 14 Fantom txn - open Dispute and sent 995 
            // 15 Transaction omited by teller - Open Dispute and send email to teller 
        }

        // Show Grid MATCHED - MERGE EXTERNAL INTERNAL  
        public void ShowGridMatched()
        {
            dataGridView1.DataSource = null;
            dataGridView1.Refresh();

            dataGridView1.DataSource = Se.TableNVStatement_Lines_Both.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                //MessageBox.Show("No transactions to be posted");
                //Form2 MessageForm = new Form2("No Matched");
                //MessageForm.ShowDialog();

                //this.Dispose();
                return;
            }

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView1.Columns[0].Width = 50; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 40; // ColorNo
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[1].Visible = false;

            dataGridView1.Columns[2].Width = 70; // MatchingNo
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[2].Visible = true;

            dataGridView1.Columns[3].Width = 85;  // Origin
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[3].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[3].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[3].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[4].Width = 40;  // Code 
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[5].Width = 90; //  ValueDate
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[5].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[5].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[5].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[6].Width = 90; // EntryDate
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[7].Width = 40; // DR/CR
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[8].Width = 40; // Ccy
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[9].Width = 100; // Amt
            dataGridView1.Columns[9].DefaultCellStyle = style;
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[9].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[9].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[9].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[10].Width = 130; // OurRef
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[10].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[10].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[10].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[11].Width = 100; // TheirRef
            dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[12].Width = 95; // OtherDetails
            dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[13].Width = 95; // CcyRate
            dataGridView1.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[13].Visible = true;

            dataGridView1.Columns[14].Width = 95; // GLAccount
            dataGridView1.Columns[14].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[14].Visible = true;
            AlertFound = false;

            int LineSeqNo;
            string LineOrigin;
            int WColorNo;
            string SelectionCriteria;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                LineSeqNo = (int)row.Cells[0].Value;
                LineOrigin = (string)row.Cells[3].Value;
                WColorNo = (int)row.Cells[1].Value;

                if (WColorNo == 11)
                {
                    row.DefaultCellStyle.BackColor = Color.Gainsboro;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
                else if (WColorNo == 12)
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }

                SelectionCriteria = " WHERE SeqNo =" + LineSeqNo + " AND Origin ='" + LineOrigin + "'";

                Se.ReadNVStatements_Lines_BySelectionCriteria(SelectionCriteria);

                if (Se.IsException == true)
                {
                    AlertFound = true;
                    //dataGridView1.Columns[1].DefaultCellStyle.BackColor = Color.SaddleBrown;
                    //dataGridView1.Columns[1].DefaultCellStyle.ForeColor = Color.White;
                    row.DefaultCellStyle.BackColor = Color.SaddleBrown;
                    row.DefaultCellStyle.ForeColor = Color.White;
                }
                else
                {

                }
            }

            if (AlertFound == true)
            {
                labelAlertFound.Show();
            }
            else
            {
                labelAlertFound.Hide();
                linkLabelDispute.Hide();
            }
        }

        // Show Grid UN MATCHED MERGE EXTERNAL INTERNAL  
        public void ShowGridUnMatched()
        {
            dataGridView1.DataSource = null;
            dataGridView1.Refresh();

            dataGridView1.DataSource = Se.TableNVStatement_Lines_Both.DefaultView;
            if (dataGridView1.Rows.Count == 0)
            {
                //MessageBox.Show("No transactions to be posted");
                Form2 MessageForm = new Form2("No Un Matched");
                MessageForm.ShowDialog();

                this.Dispose();
                return;
            }

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView1.Columns[0].Width = 50; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 40; // ColorNo
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[1].Visible = false;

            dataGridView1.Columns[2].Width = 70; // MatchingNo
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[2].Visible = false;

            dataGridView1.Columns[3].Width = 85;  // Origin
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[3].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[3].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[3].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[4].Width = 40;  // Code 
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[5].Width = 90; //  ValueDate
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[5].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[5].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[5].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[6].Width = 90; // EntryDate
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[7].Width = 40; // DR/CR
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[8].Width = 40; // Ccy
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[9].Width = 100; // Amt
            dataGridView1.Columns[9].DefaultCellStyle = style;
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[9].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[9].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[9].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[10].Width = 130; // OurRef
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[10].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[10].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[10].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[11].Width = 100; // TheirRef
            dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[12].Width = 95; // OtherDetails
            dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[13].Width = 95; // CcyRate
            dataGridView1.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[13].Visible = true;

            dataGridView1.Columns[14].Width = 95; // GLAccount
            dataGridView1.Columns[14].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[14].Visible = true;

            AlertFound = false;

            int LineSeqNo; 
            string LineOrigin; 
            int WColorNo;
            string SelectionCriteria; 

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                LineSeqNo = (int)row.Cells[0].Value;
                LineOrigin = (string)row.Cells[3].Value;
                WColorNo = (int)row.Cells[1].Value;

                if (WColorNo == 11)
                {
                    row.DefaultCellStyle.BackColor = Color.Gainsboro;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
                else if (WColorNo == 12)
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }

                SelectionCriteria = " WHERE SeqNo =" + LineSeqNo + " AND Origin ='" + LineOrigin + "'";

                Se.ReadNVStatements_Lines_BySelectionCriteria(SelectionCriteria);

                if (Se.IsException == true)
                {
                    AlertFound = true;
                    //dataGridView1.Columns[1].DefaultCellStyle.BackColor = Color.SaddleBrown;
                    //dataGridView1.Columns[1].DefaultCellStyle.ForeColor = Color.White;
                    row.DefaultCellStyle.BackColor = Color.SaddleBrown;
                    row.DefaultCellStyle.ForeColor = Color.White;
                }
                else
                {
                   
                }
            }

            if (AlertFound == true)
            {
                labelAlertFound.Show();
            }
            else
            {
                labelAlertFound.Hide();
                linkLabelDispute.Hide();
            }

        }

        // Show Grid STATEMENT
        bool AlertFound; 
        public void ShowGridStatement()
        {
            dataGridView1.DataSource = null;
            dataGridView1.Refresh();
            dataGridView1.DataSource = Se.TableNVStatement_Lines_Single.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                ////MessageBox.Show("No transactions to be posted");
                //Form2 MessageForm = new Form2("No Matched");
                //MessageForm.ShowDialog();

                return;

            }

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";
            //// DATA TABLE ROWS DEFINITION 

            dataGridView1.Columns[0].Width = 50; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 50; // Status
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 50; // Setl
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[3].Width = 70;  // MatchingNo
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[4].Width = 70;  // Origin
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[5].Width = 40; //  Code 
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[6].Width = 90; //  ValueDate
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[7].Width = 90; // EntryDate
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[8].Width = 40; // DR/CR
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[9].Width = 40; // Ccy
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[10].Width = 110; // Amt
            dataGridView1.Columns[10].DefaultCellStyle = style;
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[10].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[10].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[10].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[11].Width = 110; // StmtBal
            dataGridView1.Columns[11].DefaultCellStyle = style;
            dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[11].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[11].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[11].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[12].Width = 130; // OurRef
            dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[12].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[13].Width = 95; // TheirRef
            dataGridView1.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[14].Width = 95; // OtherDetails
            dataGridView1.Columns[14].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            AlertFound = false; 

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                string WStatus = row.Cells[1].Value.ToString();

                if (WStatus == "M")
                {
                    row.DefaultCellStyle.BackColor = Color.Gainsboro;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
                else if (WStatus == "U" )
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
                if (WStatus == "A")
                {
                    AlertFound = true; 
                    //dataGridView1.Columns[1].DefaultCellStyle.BackColor = Color.SaddleBrown;
                    //dataGridView1.Columns[1].DefaultCellStyle.ForeColor = Color.White;
                    row.DefaultCellStyle.BackColor = Color.SaddleBrown;
                    row.DefaultCellStyle.ForeColor = Color.White;
                }
            }
            if (AlertFound == true)
            {
                labelAlertFound.Show();
            }
            else
            {
                labelAlertFound.Hide();
                linkLabelDispute.Hide();
            }
        }

        // Notes 
        private void buttonNotes2_Click(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "";
            string WParameter4 = "UniqueRecordId: ";
            string SearchP4 = "";
            //if (ViewWorkFlow == true) WMode = "Read";
            //else WMode = "Update";
            WMode = "Update";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WMode, SearchP4);
            NForm197.ShowDialog();

            // NOTES 
            Order = "Descending";
            WParameter4 = "UniqueRecordId: ";
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";
            //SetScreen();
        }

        private void buttonPrint_Click(object sender, EventArgs e)
        {
            if (comboBoxFilter.Text == "Internal Statement" || comboBoxFilter.Text == "External Statement")
            {
                Us.ReadUsersRecord(WSignedId); // Get the Bank for Bank Logo

                string P1 = "TRANSACTIONS FOR :" + comboBoxFilter.Text + " " + InternalAcc;
                string P2 = "Second Par";
                string P3 = "Third Par";
                string P4 = Us.BankId;
                string P5 = WSignedId;
                string P6 = FromDt.ToShortDateString();
                string P7 = ToDt.ToShortDateString();
                //string P5 = "1005";

                Form56R65NOSTRO ReportNOSTRO65 = new Form56R65NOSTRO(P1, P2, P3, P4, P5, P6, P7);
                ReportNOSTRO65.Show();
            }
            else
            {
                Us.ReadUsersRecord(WSignedId); // Get the Bank for Bank Logo

                string P1 = "TRANSACTIONS FOR :" + comboBoxFilter.Text;
                string P2 = "Second Par";
                string P3 = "Third Par";
                string P4 = Us.BankId;
                string P5 = WSignedId;
                string P6 = FromDt.ToShortDateString();
                string P7 = ToDt.ToShortDateString();

                //string P5 = "1005";
                Form56R64NOSTRO ReportNOSTRO64 = new Form56R64NOSTRO(P1, P2, P3, P4, P5, P6, P7);
                ReportNOSTRO64.Show();
            }
        }
        //
        // SHOW Selection 
        //
        int WUniqueNo;
        decimal WUniqueAmt; 
        private void buttonShowSelection_Click(object sender, EventArgs e)
        {
            //
            // Unique
            //
            if (radioButtonMatchingId.Checked == false & radioButtonAmt.Checked == false & radioButtonReferenceNo.Checked == false)
            {
                MessageBox.Show("Please select and Continue ");
                return;
            }
            if (textBoxInputField.Text == "")
            {

                MessageBox.Show("Please enter value!");
                return;
            }

            if (radioButtonMatchingId.Checked == true) // Matching Id
            {
                if (int.TryParse(textBoxInputField.Text, out WUniqueNo))
                {
                }
                else
                {
                    MessageBox.Show("Please enter a valid Unique Matching No!");
                    return;
                }
                Mode = 17;
                FromDt = dateTimePicker1.Value.AddDays(-1);
                ToDt = dateTimePicker2.Value.AddDays(1);
                Se.ReadNVStatements_LinesByRangeAndDates(WOperator, WSignedId,WSubSystem ,Mode,
                                ExternalAcc, InternalAcc, FromDt, ToDt, "", WUniqueNo, 0);

                if (Se.TableNVStatement_Lines_Single.Rows.Count == 0)
                {
                    MessageBox.Show("No Entries to show");
                    return;
                }
                // Check table 
                ShowGridStatement();

            }

            if (radioButtonAmt.Checked == true) // Amt
            {
                if (decimal.TryParse(textBoxInputField.Text, out WUniqueAmt))
                {
                }
                else
                {
                    MessageBox.Show("Please enter a valid Unique Absolute Amt!");
                    return;
                }

                if (WUniqueAmt <= 0)
                {
                    MessageBox.Show("Please enter a positive Amt!");
                    return;
                }

                Mode = 18;
                FromDt = dateTimePicker1.Value.AddDays(-1);
                ToDt = dateTimePicker2.Value.AddDays(1);
                Se.ReadNVStatements_LinesByRangeAndDates(WOperator, WSignedId, WSubSystem, Mode,
                                ExternalAcc, InternalAcc, FromDt, ToDt, "", 0, WUniqueAmt);

                if (Se.TableNVStatement_Lines_Single.Rows.Count == 0)
                {
                    MessageBox.Show("No Entries to show");
                    return;
                }
                // Check table 
                ShowGridStatement();


            }

            if (radioButtonReferenceNo.Checked == true) // Reference No 
            {

                Mode = 19;
                FromDt = dateTimePicker1.Value.AddDays(-1);
                ToDt = dateTimePicker2.Value.AddDays(1);
                Se.ReadNVStatements_LinesByRangeAndDates(WOperator, WSignedId, WSubSystem, Mode,
                                ExternalAcc, InternalAcc, FromDt, ToDt, textBoxInputField.Text, 0, 0);

                if (Se.TableNVStatement_Lines_Single.Rows.Count == 0)
                {
                    MessageBox.Show("No Entries to show");
                    return;
                }
                // Check table 
                ShowGridStatement();
            }

            //ShowGridMatched();
            Form80b_Load(this, new EventArgs());

        }
        // Finish 
        private void buttonNext_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        // ON COMBO CHANGE LOAD 
        private void comboBoxFilter_SelectedIndexChanged(object sender, EventArgs e)
        {

            Form80b_Load(this, new EventArgs());

        }


        // Unique search 
        private void checkBoxUnique_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxUnique.Checked == true)
            {
                panel5.Show();
                buttonShowSelection.Show();
                comboBoxFilter.Text = "ALL";
                Form80b_Load(this, new EventArgs());
            }
            else
            {
                panel5.Hide();
                buttonShowSelection.Hide();

                radioButtonMatchingId.Checked = false;
                radioButtonAmt.Checked = false;
                radioButtonReferenceNo.Checked = false;

                textBoxInputField.Text = "";
                comboBoxFilter.Text = "Internal Statement";
                Form80b_Load(this, new EventArgs());
            }
        }

        // EXPAND GRID
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //Form78b NForm78b;
            //string WHeader = "LIST OF TRANSACTIONS";
            MessageBox.Show("This will show Origin of Transaction at Branch and from Swift");
            //NForm78b = new Form78b(WSignedId, WSignRecordNo, WOperator, Mp.MatchingMasterDataTableITMX, WHeader, "Form80bITMX");
            //NForm78b.FormClosed += NForm78b_FormClosed;
            //NForm78b.ShowDialog();
        }
// External Bank Change 
        private void comboBoxExternalBank_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Read all Accounts for this Bank 

            comboBoxExternalAccount.DataSource = Acc.GetExternalAccountsForExternalBank(WOperator, comboBoxExternalBank.Text);
            comboBoxExternalAccount.DisplayMember = "DisplayValue";
            ValidBankAccs = false; 
            Form80b_Load(this, new EventArgs());
        }

        private void comboBoxExternalAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidBankAccs = false;
            Acc.ReadAndFindAccountSpecificForNostroVostro(comboBoxExternalAccount.Text, comboBoxExternalBank.Text);
            if (Acc.RecordFound == true)
            {
                textBoxVostroCcy.Text = Acc.CurrNm;
                textBoxInternalAccount.Text = Acc.AccNoInternal;
            }
            else
            {
                textBoxVostroCcy.Text = "";
                textBoxInternalAccount.Text = "";
            }

            Form80b_Load(this, new EventArgs());
        }
// Show Dispute 
        private void linkLabelDispute_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form3_NOSTRO NForm3_NOSTRO;
            string Mode = "VIEW";
            NForm3_NOSTRO = new Form3_NOSTRO(WSignedId, WSignRecordNo, WOperator,
                            Mode, WDispNo);
            NForm3_NOSTRO.FormClosed += NForm3_NOSTRO_FormClosed;
            NForm3_NOSTRO.Show();
        }

        private void NForm3_NOSTRO_FormClosed(object sender, FormClosedEventArgs e)
        {

            //int WRowIndex = dataGridView1.SelectedRows[0].Index;

            //Form503_Load(this, new EventArgs());

            //dataGridView1.Rows[WRowIndex].Selected = true;
            //dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));
        }
    }
}
