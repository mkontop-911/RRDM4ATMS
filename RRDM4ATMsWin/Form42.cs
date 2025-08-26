using System;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form42 : Form
    {
        RRDMReplOrdersClass Ra = new RRDMReplOrdersClass(); 

        int WReplActNo;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WAtmNo;
        DateTime WDtFrom;
        DateTime WDtTo;

        public Form42(string InSignedId, int SignRecordNo, string InOperator, string InAtmNo, DateTime InDtFrom, DateTime InDtTo)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;

            WAtmNo = InAtmNo;
            WDtFrom = InDtFrom;
            WDtTo = InDtTo.AddDays(1);
            
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

            labelStep1.Text = "Actions Cycles for AtmNo : " + WAtmNo;

            textBoxMsgBoard.Text = "Select line and view details!"; 
        }

        private void Form42_Load(object sender, EventArgs e)
        {

            string filter = "BankId ='" + WOperator + "' AND AtmNo ='" + WAtmNo + "' " ; 

            Ra.ReadReplActionsAndFillTable(WOperator, filter, WDtFrom, WDtTo, 2);

            dataGridView1.DataSource = Ra.TableReplOrders.DefaultView;

            dataGridView1.Columns[0].Width = 40; // ActNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 40; // ActId
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 40; // AtmNo

            dataGridView1.Columns[3].Width = 50; // CycleNo

            dataGridView1.Columns[4].Width = 70; // AmountWas

            dataGridView1.Columns[5].Width = 70; // LastReplDt
            dataGridView1.Columns[5].Width = 70; // NewEstReplDt
            dataGridView1.Columns[5].Width = 70; // PassReplCycleDate

            dataGridView1.Columns[5].Width = 70; // EstAmount
            dataGridView1.Columns[5].Width = 70; // InMoneyReal

            if (dataGridView1.Rows.Count == 0 )
            {
                //MessageBox.Show("No transactions to be posted");
                Form2 MessageForm = new Form2("No Actions within this dates range! ");
                MessageForm.ShowDialog();

                this.Dispose();
                return;
            }

            //TableReplActions.Columns.Add("ActNo", typeof(int));
            //TableReplActions.Columns.Add("ActId", typeof(int));
            //TableReplActions.Columns.Add("AtmNo", typeof(string));
            //TableReplActions.Columns.Add("CycleNo", typeof(int));
            //TableReplActions.Columns.Add("AmountWas", typeof(string));
            //TableReplActions.Columns.Add("LastReplDt", typeof(DateTime));
            //TableReplActions.Columns.Add("NewEstReplDt", typeof(DateTime));
            //TableReplActions.Columns.Add("PassReplCycleDate", typeof(DateTime));
            //TableReplActions.Columns.Add("EstAmount", typeof(string));
            //TableReplActions.Columns.Add("InMoneyReal", typeof(string));
        }

// DATA GRID ROW ENTER 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WReplActNo = (int)rowSelected.Cells[0].Value;

            Ra.ReadReplActionsForAtm(WAtmNo, WReplActNo);

            textBox1.Text = Ra.ReplOrderNo.ToString();

            textBox2.Text = Ra.AtmNo;

            textBox3.Text = Ra.ReplCycleNo.ToString();

            textBoxCurrCassettes.Text = Ra.CurrCassettes.ToString("#,##0.00");
            textBoxCurrentDeposits.Text = Ra.CurrentDeposits.ToString("#,##0.00");

            textBoxLastReplDt.Text = Ra.LastReplDt.ToString();
            textBoxDateInsert.Text = Ra.DateInsert.ToString();

            textBoxNewEstReplDt.Text = Ra.NewEstReplDt.ToString();

            if (Ra.AuthorisedRecord == true)
            {
                textBoxAuthorisedDate.Text = Ra.AuthorisedDate.ToString();
            }
            else
            {
                textBoxAuthorisedDate.Text = "Not Authorised yet!";
            }

            if (Ra.PassReplCycle == true)
            {
                textBoxPassReplCycleDate.Text = Ra.PassReplCycleDate.ToString();
            }
            else
            {
                textBoxPassReplCycleDate.Text = "No Repl Yet!";
            }

            textBoxNewAmount.Text = Ra.NewAmount.ToString("#,##0.00");
            textBoxCashInAmount.Text = Ra.CashInAmount.ToString("#,##0.00");
            textBoxInMoneyReal.Text = Ra.InMoneyReal.ToString("#,##0.00");
            textBoxCashDifference.Text = (Ra.CashInAmount - Ra.InMoneyReal).ToString("#,##0.00");

            textBoxReplActId.Text = Ra.ReplOrderId.ToString();
            textBoxAuthUser.Text = Ra.AuthUser;
            textBoxCitId.Text = Ra.CitId; 

        }

// Finish 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

    }
}
