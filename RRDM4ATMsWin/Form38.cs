using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Data.SqlClient;
using System.Configuration;
//multilingual


namespace RRDM4ATMsWin
{
    public partial class Form38 : Form
    {
       
        Form24 NForm24; // Errors 

        Form67 NForm67; // Journal 

        Form5 NForm5; // Dispute form 

        DataTable DepositsTran = new DataTable();

        int RowSelected; 

   //     int TotDepCash;
    //    int TotDepCheques;
   //     int TotDepEnvel; 

        int TransType;

        int WMasterTraceNo; 

        int UniqueRecordId;
        int TraceNo ;
        string  Card ;
        string Account ;
        string CurrNm ;

        decimal Amount ;
        string TransDesc ;
        DateTime DateTm ;
        decimal Counted ;

        decimal Differ ;
        bool Matched ;
        bool Error;

        string Comments; 

        int ErrNo; 

        int TotNoCa ;
        decimal TotValueCa ;
        decimal TotCountedCa ;
        decimal TotDiffCa ;

        int TotNoCh;
        decimal TotValueCh;
        decimal TotCountedCh;
        decimal TotDiffCh;

        int TotNoEnv;
        decimal TotValueEnv;
        decimal TotCountedEnv;
        decimal TotDiffEnv;

        string WBankId; 

        string SQLString;

        string connectionString = ConfigurationManager.ConnectionStrings
          ["ATMSConnectionString"].ConnectionString;

        RRDMAtmsClass Ac = new RRDMAtmsClass();
        RRDMDisputeTransactionsClass Dt = new RRDMDisputeTransactionsClass();
        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
        RRDMDepositsClass Da = new RRDMDepositsClass(); 

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
     //   bool WPrive;
        string WAtmNo;
        int WSesNo;

        public Form38(string InSignedId, int InSignRecordNo, string InOperator,  string InAtmNo, int InSesNo)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
   
            WAtmNo = InAtmNo;
            WSesNo = InSesNo; 

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

            labelATMno.Text = WAtmNo;

            Ac.ReadAtm(WAtmNo);
            WBankId = Ac.BankId;
            
            labelSessionNo.Text = WSesNo.ToString();

            textBoxMsgBoard.Text = "Review data and take necessary actions. Press Finish when all actions are taken. "; 
        }

        private void Form38_Load(object sender, EventArgs e)
        {
            DepositsTran = new DataTable();
            DepositsTran.Clear();
            bool TranFound = false;
            // DATA TABLE ROWS DEFINITION 
            DepositsTran.Columns.Add("UniqueId", typeof(int));
            DepositsTran.Columns.Add("TraceNo", typeof(int));
            DepositsTran.Columns.Add("Card", typeof(string));
            DepositsTran.Columns.Add("Account", typeof(string));
            DepositsTran.Columns.Add("CurrNm", typeof(string));

            DepositsTran.Columns.Add("Amount", typeof(decimal));
            DepositsTran.Columns.Add("TransDesc", typeof(string));
            DepositsTran.Columns.Add("DateTm", typeof(DateTime));
            DepositsTran.Columns.Add("Counted", typeof(decimal));

            DepositsTran.Columns.Add("Differ", typeof(decimal));

            DepositsTran.Columns.Add("Matched", typeof(bool));
            DepositsTran.Columns.Add("Error", typeof(bool));

            DepositsTran.Columns.Add("Comments", typeof(string));

            TotNoCa = 0;
            TotValueCa = 0;
            TotCountedCa = 0;
            TotDiffCa = 0;

            TotNoCh = 0;
            TotValueCh = 0;
            TotCountedCh = 0;
            TotDiffCh = 0;

            TotNoEnv = 0;
            TotValueEnv = 0;
            TotCountedEnv = 0;
            TotDiffEnv = 0;
         

            SQLString = "Select * "
                + "FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                + " WHERE TerminalId = @AtmNo AND ReplCycleNo = @SesNo "
                + " AND (TransType = 23 OR TransType = 24 OR TransType = 25)"; 

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLString, conn))
                    {
                      cmd.Parameters.AddWithValue("@AtmNo", WAtmNo);
                      cmd.Parameters.AddWithValue("@SesNo", WSesNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            TranFound = true;

                            DataRow RowGrid = DepositsTran.NewRow();

                            RowGrid["UniqueId"] = (int)rdr["UniqueRecordId"];
                            RowGrid["TraceNo"] = (int)rdr["AtmTraceNo"];
                            RowGrid["Card"] = (string)rdr["CardNumber"];
                            RowGrid["Account"] = (string)rdr["AccNumber"];
                            RowGrid["CurrNm"] = (string)rdr["TransCurr"];

                            Amount = (decimal)rdr["TransAmount"];
                            RowGrid["Amount"] = Amount;
                            TransDesc = (string)rdr["TransDescr"];
                            RowGrid["TransDesc"] = TransDesc; 
                            RowGrid["DateTm"] = (DateTime)rdr["TransDate"];

                            TransType = (int)rdr["TransType"];

                            Counted = (decimal)rdr["DepCount"];

                            RowGrid["Counted"] = Counted;

                            Differ = Counted - Amount;

                            RowGrid["Differ"] = Differ;

                            if (Differ == 0)
                            {
                                RowGrid["Matched"] = true;
                            }
                            else RowGrid["Matched"] = false;

                            ErrNo = (int)rdr["MetaExceptionNo"];

                            if (ErrNo > 0)
                            {
                                RowGrid["Error"] = true; 
                            }
                            else RowGrid["Error"] = false;

                            RowGrid["Comments"] = (string)rdr["Comments"];

                            if (TransType == 23) // "DEPOSIT BNA"
                            {
                                TotNoCa = TotNoCa + 1;
                                TotValueCa = TotValueCa + Amount;
                                TotCountedCa = TotCountedCa + Counted;
                                TotDiffCa = TotCountedCa - TotValueCa; 
                            }
                            if ( TransType == 24) //TransDesc == "DEP CHEQUES")
                                {
                                TotNoCh = TotNoCh + 1;
                                TotValueCh = TotValueCh + Amount;
                                TotCountedCh = TotCountedCh + Counted;
                                TotDiffCh = TotCountedCh - TotValueCh;
                            }

                            if (TransType == 25) // Envelope Deposit                            
                            {
                                TotNoEnv = TotNoEnv + 1;
                                TotValueEnv = TotValueEnv + Amount;
                                TotCountedEnv = TotCountedEnv + Counted;
                                TotDiffEnv = TotCountedEnv - TotValueEnv;
                            }
                                                      
                            DepositsTran.Rows.Add(RowGrid);
                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {

                    string exception = ex.ToString();
                    //       MessageBox.Show(ex.ToString());
                    MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

                }

            if (TranFound == false)
            {
                MessageBox.Show(" No Deposits for this Replenish Cycle!");
                return;
            }

            dataGridView1.DataSource = DepositsTran.DefaultView;
      
            dataGridView1.Columns[0].Name = "UniqueId";
            dataGridView1.Columns[1].Name = "TraceNo";
            dataGridView1.Columns[2].Name = "Card";
            dataGridView1.Columns[3].Name = "Account";
            dataGridView1.Columns[4].Name = "CurrNm";

            dataGridView1.Columns[5].Name = "Amount";
            dataGridView1.Columns[6].Name = "TransDesc";
            dataGridView1.Columns[7].Name = "DateTm";
            dataGridView1.Columns[8].Name = "Counted";

            dataGridView1.Columns[9].Name = "Differ";
            dataGridView1.Columns[10].Name = "Matched";

            dataGridView1.Columns[11].Name = "Error";

            dataGridView1.Columns[12].Name = "Comments";

            // SIZE
            dataGridView1.Columns["UniqueId"].Width = 50; //
            dataGridView1.Columns["TraceNo"].Width = 60; //
            dataGridView1.Columns["Card"].Width = 100;
            dataGridView1.Columns["Account"].Width = 70;
            dataGridView1.Columns["CurrNm"].Width = 60;

            dataGridView1.Columns["Amount"].Width = 70;
            dataGridView1.Columns["TransDesc"].Width = 120;
            dataGridView1.Columns["DateTm"].Width = 100;
            dataGridView1.Columns["Counted"].Width = 85;

            dataGridView1.Columns["Differ"].Width = 60;
            dataGridView1.Columns["Matched"].Width = 60;

            dataGridView1.Columns["Error"].Width = 60;

            dataGridView1.Columns["Comments"].Width = 100;

            dataGridView1.Sort(dataGridView1.Columns["TransDesc"], ListSortDirection.Ascending);

            // Show Cash Deposits 
            textBox5.Text = TotNoCa.ToString();
            textBox1.Text = TotValueCa.ToString("#,##0.00");
            textBox12.Text = TotCountedCa.ToString("#,##0.00");
            textBox4.Text = TotDiffCa.ToString("#,##0.00");
            // Show Cheques Deposits
            textBox2.Text = TotNoCh.ToString();
            textBox3.Text = TotValueCh.ToString("#,##0.00");
            textBox13.Text = TotCountedCh.ToString("#,##0.00");
            textBox6.Text = TotDiffCh.ToString("#,##0.00");
            // Show Envelop Deposits
            textBox15.Text = TotNoEnv.ToString();
            textBox16.Text = TotValueEnv.ToString("#,##0.00");
            textBox14.Text = TotCountedEnv.ToString("#,##0.00");
            textBox17.Text = TotDiffEnv.ToString("#,##0.00");
      
        }

        // ON Row Enter Fill up the fields 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            RowSelected = e.RowIndex;

            UniqueRecordId = (int)rowSelected.Cells["UniqueId"].Value;
            TraceNo = (int)rowSelected.Cells["TraceNo"].Value;
            Card = (string)rowSelected.Cells["Card"].Value;
            Amount = (decimal)rowSelected.Cells["Amount"].Value;
            Counted = (decimal)rowSelected.Cells["Counted"].Value;
            Differ = (decimal)rowSelected.Cells["Differ"].Value;
            Comments = (string)rowSelected.Cells["Comments"].Value;
            Error = (bool)rowSelected.Cells["Error"].Value;

            if (Error == true)
            {
                button5.Show();
                textBox18.Show();
                textBox18.Text = "Suspected Notes";

            }
            else
            {
                textBox18.Hide();
                button5.Hide();
            }

            textBox8.Text = TraceNo.ToString();
            textBox7.Text = Amount.ToString("#,##0.00");
            textBox9.Text = Counted.ToString("#,##0.00");
            textBox10.Text = Differ.ToString("#,##0.00");
            textBoxMyComments.Text = Comments;

            // Show Dispute 

            Dt.ReadDisputeTranByUniqueRecordId(UniqueRecordId);
            if (Dt.RecordFound == true)
            {
                labelDisputeId.Show();
                textBoxDisputeId.Show();
                buttonMoveToDispute.Hide();
                textBoxDisputeId.Text = Dt.DisputeNumber.ToString();
            }
            else
            {
                labelDisputeId.Hide();
                textBoxDisputeId.Hide();
                buttonMoveToDispute.Show();
            }

            //Tc.ReadInPoolTransSpecific(UniqueRecordId);
            string SelectionCriteria = " Where UniqueRecordId =" + UniqueRecordId;
            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria,1);

            WMasterTraceNo = Mpa.MasterTraceNo;

        }

        // FINISH = UPDATE ALL ACTIONS 
        // Update In Pool Transactions with data grid data
        private void buttonNext_Click(object sender, EventArgs e)
        {
            TotNoCa = 0;
            TotValueCa = 0;
            TotCountedCa = 0;
            TotDiffCa = 0;

            TotNoCh = 0;
            TotValueCh = 0;
            TotCountedCh = 0;
            TotDiffCh = 0;

            TotNoEnv = 0;
            TotValueEnv = 0;
            TotCountedEnv = 0;
            TotDiffEnv = 0;
            int K = 0;

            while (K <= (dataGridView1.Rows.Count - 1))
            {
                UniqueRecordId = (int)dataGridView1.Rows[K].Cells["UniqueId"].Value;
                TraceNo = (int)dataGridView1.Rows[K].Cells["TraceNo"].Value;
                Card = (string)dataGridView1.Rows[K].Cells["Card"].Value;
                Account = (string)dataGridView1.Rows[K].Cells["Account"].Value;
                CurrNm = (string)dataGridView1.Rows[K].Cells["CurrNm"].Value;

                Amount = (decimal)dataGridView1.Rows[K].Cells["Amount"].Value;
                TransDesc = (string)dataGridView1.Rows[K].Cells["TransDesc"].Value;
                DateTm = (DateTime)dataGridView1.Rows[K].Cells["DateTm"].Value;
                Counted = (decimal)dataGridView1.Rows[K].Cells["Counted"].Value;

                Differ = (decimal)dataGridView1.Rows[K].Cells["Differ"].Value;
                Matched = (bool)dataGridView1.Rows[K].Cells["Matched"].Value;

                Error = (bool)dataGridView1.Rows[K].Cells["Error"].Value;

                Comments = (string)dataGridView1.Rows[K].Cells["Comments"].Value;

                // Read Transaction
                string SelectionCriteria = " Where UniqueRecordId =" + UniqueRecordId; 
                Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria,1); 

                //Tc.ReadInPoolTransSpecific(UniqueRecordId);

                Mpa.DepCount = Counted;

                Mpa.Comments = Comments;

                Mpa.UpdateTransforDeposits(UniqueRecordId,2);

                if (Mpa.TransType == 23) // "DEPOSIT BNA"
                {
                    TotNoCa = TotNoCa + 1;
                    TotValueCa = TotValueCa + Amount;
                    TotCountedCa = TotCountedCa + Counted;
                    TotDiffCa = TotCountedCa - TotValueCa;
                }
                if (Mpa.TransType == 24) //TransDesc == "DEP CHEQUES")
                {
                    TotNoCh = TotNoCh + 1;
                    TotValueCh = TotValueCh + Amount;
                    TotCountedCh = TotCountedCh + Counted;
                    TotDiffCh = TotCountedCh - TotValueCh;
                }

                if (Mpa.TransType == 25) // Envelope Deposit                            
                {
                    TotNoEnv = TotNoEnv + 1;
                    TotValueEnv = TotValueEnv + Amount;
                    TotCountedEnv = TotCountedEnv + Counted;
                    TotDiffEnv = TotCountedEnv - TotValueEnv;
                }

                //if (Mpa.TransType == 23 ) // "DEPOSIT_BNA"
                //{
                //    TotNoCa = TotNoCa + 1;
                //    TotValueCa = TotValueCa + Amount;
                //    TotCountedCa = TotCountedCa + Counted;
                //    TotDiffCa = TotCountedCa - TotValueCa;
                //}
                //if (TransDesc == "DEPOSIT")
                //{
                //    TotNoCh = TotNoCh + 1;
                //    TotValueCh = TotValueCh + Amount;
                //    TotCountedCh = TotCountedCh + Counted;
                //    TotDiffCh = TotCountedCh - TotValueCh;
                //}

                //if (TransDesc == "DEP CHEQUES")
                //{
                //    TotNoEnv = TotNoEnv + 1;
                //    TotValueEnv = TotValueEnv + Amount;
                //    TotCountedEnv = TotCountedEnv + Counted;
                //    TotDiffEnv = TotCountedEnv - TotValueEnv;
                //}

                K++; // Read Next entry of the table 
            }

            // Update deposits in Na. 
            // New Count figures must update Na
            //     Da.ReadDepositsSessionsNotesAndValuesDeposits(WAtmNo, WSesNo);
            //    Da.DepositsCount1.Amount = TotCountedCa; 

            //    Da.UpdateDepositsSessionsNotesAndValuesWithCount(WAtmNo, WSesNo ); 

            // Show Cash Deposits 
            textBox5.Text = TotNoCa.ToString();
            textBox1.Text = TotValueCa.ToString("#,##0.00");
            textBox12.Text = TotCountedCa.ToString("#,##0.00");
            textBox4.Text = TotDiffCa.ToString("#,##0.00");
            // Show Cheques Deposits
            textBox2.Text = TotNoCh.ToString();
            textBox3.Text = TotValueCh.ToString("#,##0.00");
            textBox13.Text = TotCountedCh.ToString("#,##0.00");
            textBox6.Text = TotDiffCh.ToString("#,##0.00");
            // Show Envelop Deposits
            textBox15.Text = TotNoEnv.ToString();
            textBox16.Text = TotValueEnv.ToString("#,##0.00");
            textBox14.Text = TotCountedEnv.ToString("#,##0.00");
            textBox17.Text = TotDiffEnv.ToString("#,##0.00");

            //Form38_Load(this, new EventArgs());

            MessageBox.Show("All lines in Grid have been updated");

            this.Dispose(); 
        }
       
  
        // Text Change 
        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            //  Counted to be > 0

            if (decimal.TryParse(textBox9.Text, out  Counted))
            {
                // Take the correct action 
            }
            else
            {
                MessageBox.Show(textBox9.Text, "Please enter a valid number!");
                return;
            }

           Differ = Counted - Amount;
            textBox10.Text = Differ.ToString("#,##0.00");

            dataGridView1.Rows[RowSelected].Cells["Counted"].Value = Counted;

            dataGridView1.Rows[RowSelected].Cells["Differ"].Value = Differ;
            if (Differ != 0)
            {
                dataGridView1.Rows[RowSelected].Cells["Matched"].Value = false;
            }
            else dataGridView1.Rows[RowSelected].Cells["Matched"].Value = true;

            dataGridView1.Rows[RowSelected].Cells["Differ"].Value = Differ;

            dataGridView1.Refresh(); 
        }

        // text change 
        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows[RowSelected].Cells["Comments"].Value = textBoxMyComments.Text;
            dataGridView1.Refresh(); 
        }

        // Show Error 
        private void button5_Click(object sender, EventArgs e)
        {
            string SelectionCriteria = " Where UniqueRecordId =" + UniqueRecordId;
            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria,2);

            bool Deposits = true;
            string SearchFilter = "ErrNo = " + Mpa.MetaExceptionNo ;

            NForm24 = new Form24(WSignedId, WSignRecordNo, WBankId,  WAtmNo, WSesNo, "", Deposits, SearchFilter);
            NForm24.ShowDialog(); ;
        }
        // Move transaction to dispute 
        private void button1_Click(object sender, EventArgs e)
        {
            if (Amount == Counted)
            {
                MessageBox.Show("No difference to be disputed");
                return;
            }
            int From = 3; // From Deposit 
            NForm5 = new Form5(WSignedId, WSignRecordNo, WOperator, Card, UniqueRecordId, Counted, 0 , textBoxMyComments.Text, From, "ATM");
            NForm5.FormClosed += NForm5_FormClosed;
            NForm5.ShowDialog(); 
        }

        void NForm5_FormClosed(object sender, FormClosedEventArgs e)
        {
            Dt.ReadDisputeTranByUniqueRecordId(UniqueRecordId);
            if (Dt.RecordFound == true)
            {
                labelDisputeId.Show();
                textBoxDisputeId.Show();
                buttonMoveToDispute.Hide();
                textBoxDisputeId.Text = Dt.DisputeNumber.ToString();
            }
            else
            {
                labelDisputeId.Hide();
                textBoxDisputeId.Hide();
                buttonMoveToDispute.Show();
            }
        }
        // View Journal Part for this deposit 
        private void button3_Click(object sender, EventArgs e)
        {
            if (WAtmNo == "AB102")
            {
                MessageBox.Show("Not available for ATM102. Only available for ATM104");
                return; 
            }
            //
     
            int Mode = 5; // Specific
            DateTime NullPastDate = new DateTime(1900, 01, 01);
            NForm67 = new Form67(WSignedId, WSignRecordNo, WOperator,  0, WAtmNo, WMasterTraceNo, TraceNo, Mpa.TransDate.Date, NullPastDate, Mode);
            //   DateTime Tempdt = new DateTime(2014, 02, 12);
            //   NForm67 = new Form67("1005", 70513, "CRBAGRAA",  0, "AB104", 10044990, 10044990, Tempdt, NullPastDate, Mode);
            NForm67.ShowDialog(); 
      
        }
       
        
    }
}
