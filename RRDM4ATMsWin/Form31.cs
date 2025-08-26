using System;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Configuration;
//multilingual

namespace RRDM4ATMsWin
{
    public partial class Form31 : Form
    {

        // // DATATable for Grid 
        //DataTable CardAtmsTran = new DataTable(); 

        //DataTable dtAtmsTran = new DataTable();
    //    SqlDataAdapter daAtmsTran;

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;
        
        RRDMAccountsClass Acc = new RRDMAccountsClass();
        RRDMPostedTrans Pt = new RRDMPostedTrans(); 

        string WFilter; 

        //bool RecordFound ; 

        //decimal LineBal;
        //decimal OldLineBal;
        //decimal TotalCr;
        //decimal TotalDr;
        int WFunction; 

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WAccNo; 
    
        string WUserId;
        string WAccName;
        string WCurrDesc; 
        int WAction;
        DateTime WFromDate;
        DateTime WToDate;

        string WAtmNo; 

        public Form31(string InSignedId, int InSignRecordNo, string InOperator, string InUserId, string InAccName,
                     string InCurrDesc, int InAction, DateTime InFromDate, DateTime InToDate, string InAtmNo )
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
     
            WUserId = InUserId; // this the cit providers
            WAccName = InAccName;
            WCurrDesc = InCurrDesc;
            WAction = InAction;
            WFromDate = InFromDate;
            WToDate = InToDate;

            WAtmNo = InAtmNo;
          
            InitializeComponent();

            if (WAction == 1)
            {
                label14.Text = "Account Statement for the Last Seven days";
            }
            if (WAction == 2)
            {
                label14.Text = "Account Statement for Last Month";
            }
            if (WAction == 3)
            {
                label14.Text = "Account Statement for current month";
            }

            if (WAction == 4 & WUserId != "")
            {
                label14.Text = "Account Statement for CIT : "+ WUserId + 
                    " From : " + WFromDate.ToShortDateString() + "  To: " + WToDate.ToShortDateString(); 
            }

            if (WAction == 4 & WUserId == "")
            {
                label14.Text = "Account Statement for ATM : " + WAtmNo +
                    " From : " + WFromDate.ToShortDateString() + "  To: " + WToDate.ToShortDateString();
                labelAtmNo.Hide();
                textBoxAtmNo.Hide();
                buttonShow.Hide();
                buttonShowAll.Hide(); 
            }

            if (WAction == 5)
            {
                label14.Text = "All Open Ready For Posting Transactions";
                labelAtmNo.Hide();
                textBoxAtmNo.Hide();
                buttonShow.Hide();
                buttonShowAll.Hide();
            }
            else // Not 5 
            {
                if (WUserId == "")
                {
                    string UserId = "1000";
                    Acc.ReadAndFindAccount(UserId, "", "", WOperator, WAtmNo, WCurrDesc, WAccName);
                }
                else
                {
                    Acc.ReadAndFindAccountForUserId(WUserId, WOperator, WCurrDesc, WAccName);
                }

                WAccNo = Acc.AccNo;

                label12.Text = "TRANS FOR ACCOUNT: " + WAccNo + " ACC NAME: " + WAccName + " In: " + WCurrDesc;
            }        

        }

        // Show for All
        private void Form31_Load(object sender, EventArgs e)
        {
            if (WAction == 5) // ALL WAITING FOR POSTING 
            {
                WFilter = " OpenRecord = 1 ";

                WFunction = 3; // No days to be taken into consideration 

                WToDate = DateTime.Now;

                Pt.ReadPostedTransAndFillTheTable(WOperator, WAtmNo, WAccNo, WCurrDesc, WFromDate, WToDate, WFunction);

                ShowTrans(WFilter, "", WFunction);
            }
            else
            {
                if (WUserId != "") // REQUEST BY USER LIKE CIT then you use only the account no. It is unique 
                {

                    //WFilter = " AccNo ='" + WAccNo + "' AND CurrDesc ='" + WCurrDesc + "' AND OpenRecord = 1 ";

                    WFunction = 1;

                    WToDate = DateTime.Now;

                    Pt.ReadPostedTransAndFillTheTable(WOperator, WAtmNo, WAccNo, WCurrDesc, WFromDate, WToDate, WFunction);

                    ShowTrans(WFilter, "", WFunction);
                }
                if (WUserId == "") // REQUEST BY ATM - ACC no and ATM 
                {
                    //WFilter = " AtmNo ='" + WAtmNo + "' AND AccNo ='" + WAccNo + "' AND CurrDesc ='" + WCurrDesc + "' AND OpenRecord = 1 ";

                    WFunction = 2;

                    WToDate = DateTime.Now;

                    Pt.ReadPostedTransAndFillTheTable(WOperator, WAtmNo, WAccNo, WCurrDesc, WFromDate, WToDate, WFunction);

                    ShowTrans(WFilter, WAtmNo, WFunction);
                }
            }
            
        }

        // SHOW
        private void ShowTrans(string InSqlString, string InAtmNo, int InFunction)
        {
            //TEST
            

            if (Pt.RecordFound == false)
            {
                MessageBox.Show("No transactions for this request");
                this.Dispose();
                return;

            }

            textBox1.Text = Pt.TotalDr.ToString("#,##0.00");
            textBox2.Text = Pt.TotalCr.ToString("#,##0.00");
           
            dataGridView1.DataSource = Pt.TablePostedTrans.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("No Rows for the selection");
                return; 
            }
            //else
            //{
            //    MessageBox.Show("No of ROWS" + dataGridView1.Rows.Count.ToString());
             
            //}

            dataGridView1.Columns[0].Width = 70; // 
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[1].Width = 60; // 
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            
            dataGridView1.Columns[2].Width = 150; // 
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[3].Width = 280; // 
            dataGridView1.Columns[4].Width = 60; //

            dataGridView1.Columns[5].Width = 80; //
            dataGridView1.Columns[6].Width = 80; //
            dataGridView1.Columns[7].Width = 95; //
            dataGridView1.Columns[8].Width = 65; //
            dataGridView1.Columns[9].Width = 65; //
            dataGridView1.Columns[10].Width = 65; //

            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[11].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            
            dataGridView1.Columns[8].DefaultCellStyle.ForeColor = Color.LightSlateGray;
            dataGridView1.Columns[9].DefaultCellStyle.ForeColor = Color.LightSlateGray;
            dataGridView1.Columns[10].DefaultCellStyle.ForeColor = Color.LightSlateGray;
            dataGridView1.Columns[11].DefaultCellStyle.ForeColor = Color.LightSlateGray;
            dataGridView1.Columns[12].DefaultCellStyle.ForeColor = Color.LightSlateGray;
            dataGridView1.Columns[13].DefaultCellStyle.ForeColor = Color.LightSlateGray;


            dataGridView1.Columns[5].DefaultCellStyle.ForeColor = Color.Red; 

          
            //dataGridView1.Show();

            if (InFunction == 1 ) 
            {
                // Set chart data source 
                //     chart1.Series.Clear();

                chart1.DataSource = Pt.TablePostedTrans.DefaultView;

                chart1.Series[0].Name = ("Balance");

                chart1.Series[0].XValueMember = "TranDtTime";
                chart1.Series[0].YValueMembers = "BalDecimal";

                //chart1.Series[0].Name = ("Debits");
                //chart1.Series.Add("Credits");
                //chart1.Series.Add("Balance");

                // Set series members names for the X and Y values  

                //chart1.Series[0].XValueMember = "TranDtTime";
                //chart1.Series[0].YValueMembers = "Debits(-)";
                //chart1.Series[1].XValueMember = "TranDtTime";
                //chart1.Series[1].YValueMembers = "Credits(+)";
                //chart1.Series[2].XValueMember = "TranDtTime";
                //chart1.Series[2].YValueMembers = "BalDecimal";

                // Data bind to the selected data source  
                chart1.DataBind();

                // PREPARE ENLARGE GRAPH 

                //         chart2.Series.Clear();
                chart2.DataSource = Pt.TablePostedTrans.DefaultView;

                chart2.Series[0].Name = ("Balance");

                chart2.Series[0].XValueMember = "TranDtTime";
                chart2.Series[0].YValueMembers = "BalDecimal";

                //chart2.Series[0].Name = ("Debits");
                //chart2.Series.Add("Credits");
                //chart2.Series.Add("Balance");

                // Set series members names for the X and Y values  

                //chart2.Series[0].XValueMember = "TranDtTime";
                //chart2.Series[0].YValueMembers = "Debits(-)";
                //chart2.Series[1].XValueMember = "TranDtTime";
                //chart2.Series[1].YValueMembers = "Credits(+)";
                //chart2.Series[2].XValueMember = "TranDtTime";
                //chart2.Series[2].YValueMembers = "BalDecimal";

                // Data bind to the selected data source  
                chart2.DataBind();
            }
            else
            {
                chart1.DataSource = Pt.TablePostedTrans.DefaultView;
                chart1.DataBind();

                chart2.DataSource = Pt.TablePostedTrans.DefaultView;
                chart2.DataBind();
            }
/*
          
 */
        }

        
        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void chart1_MouseEnter(object sender, EventArgs e)
        {
            chart2.BringToFront();
        }

        private void chart1_MouseLeave(object sender, EventArgs e)
        {
            chart2.SendToBack();
        }
//finish 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }
     


        // Show for one ATM 
        private void button2_Click(object sender, EventArgs e)
        {
            string WAtmNo;
            if (textBoxAtmNo.TextLength == 0)
            {
                MessageBox.Show("Please enter AtmNo");
                return;
            }
            else
            {
                WAtmNo = textBoxAtmNo.Text;
            }

            string SqlString = "SELECT *"
                + " FROM [dbo].[PostedTrans] "
                + " WHERE AtmNo = @AtmNo AND AccNo = @AccNo AND CurrDesc = @CurrDesc AND OpenRecord = 1 "
                + "  Order By TranDtTime";
            WFunction = 2;
            ShowTrans(SqlString, WAtmNo, WFunction);
        }

        // Refresh SHow ALL 
        private void button1_Click(object sender, EventArgs e)
        {

            string SqlString = "SELECT *"
               + " FROM [dbo].[PostedTrans] "
               + " WHERE AccNo = @AccNo AND CurrDesc = @CurrDesc AND OpenRecord = 1 "
               + "  Order By TranDtTime";
            WFunction = 3;
            ShowTrans(SqlString, "", WFunction);

            textBoxAtmNo.Text = "";

        }
    }
}
