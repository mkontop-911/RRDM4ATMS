using System;
using System.Data;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;

namespace RRDM4ATMsWin
{
    public partial class Form68_Atms_Main : Form
    {

        RRDMAtmsMainClass Am = new RRDMAtmsMainClass(); 

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WSignedId;
        int WSignRecordNo;

        string WOperator;

        int WGroupNo;

        int WMode; 

        //string JournalString;

        public Form68_Atms_Main(string InSignedId, int InSignRecordNo, string InOperator, int InGroupNo, int InMode)
        {

            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WGroupNo = InGroupNo;

            WMode = InMode; // 10: All ATMs of this Group
                            // 11: ALL ATMs irrespective of group 

            InitializeComponent();

        }
        // ON LOAD 
        private void Form68_Load(object sender, EventArgs e)
        {
            Am.ReadAtmsMainForAuthUserAndFillTableFor_GL_Cash(WOperator, WSignedId, WMode, WGroupNo);

            dataGridView1.DataSource = Am.TableATMsMainSelected.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("No entries Available.");

                return;
            }
            else
            {
                Grid_1_Fields();
            }

        }

        // on Row enter 
      
        string WAtmNo; 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
        
            WAtmNo = (string)rowSelected.Cells[0].Value;

            Am.ReadAtmsMainSpecific(WAtmNo);

            //textBox2.Text = Am.AtmNo;
            //textBox3.Text = Am.AtmName;

            //textBox4.Text = Am.GL_Balance_At_CutOff.ToString("#,##0.00");
            //textBox5.Text = (Am.GL_Balance_At_CutOff- Am.GL_At_Repl).ToString("#,##0.00"); ;
            //textBox6.Text = Am.GL_At_Repl.ToString("#,##0.00");
            //textBox7.Text = (Am.GL_At_Repl-Am.GL_Counted).ToString("#,##0.00");

            //if (Am.GL_ReplenishmentDt.Date == NullPastDate)
            //{
            //    textBox8.Text = "Not Available";
            
            //}
            //else
            //{
            //    textBox8.Text = Am.GL_ReplenishmentDt.ToString();
              
            //}

            //if (Am.GL_ReconcDate.Date == NullPastDate)
            //{
            //    textBox9.Text = "Not Available";
            //}
            //else
            //{
            //    textBox9.Text = Am.GL_ReconcDate.ToString();
            //}

        }

        // Grid 1  Fields 
        private void Grid_1_Fields()
        {

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";          

            dataGridView1.Columns[0].Width = 90; // AtmNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[1].Width = 130; // Last Repl Dt
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Columns[1].Visible = false;

            dataGridView1.Columns[2].Width = 130; //  GL_ReconcDate
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 80; // GL At Cut Off
            dataGridView1.Columns[3].DefaultCellStyle = style;
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
          
            dataGridView1.Columns[4].Width = 80; // GL At Repl
            dataGridView1.Columns[4].DefaultCellStyle = style;
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[5].Width = 80; // Cash Unloaded
            dataGridView1.Columns[5].DefaultCellStyle = style;
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[6].Width = 130; //GL_ReplenishmentDt
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
         
            dataGridView1.Columns[7].Width = 60; // GL_ReplCycle
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[7].HeaderText = "Repl Cycle";

            dataGridView1.Columns[8].Width = 60; //GL Diff
            dataGridView1.Columns[8].DefaultCellStyle = style;
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[9].Width = 60; //GL_OutStandingErrors
            dataGridView1.Columns[9].DefaultCellStyle = style;
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[9].HeaderText = "Pending Errors";

            dataGridView1.Columns[10].Width = 80; // Maker
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Columns[9].HeaderText = "Is Deposit";

            dataGridView1.Columns[11].Width = 80; // Authoriser
            dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Columns[10].HeaderText = "Opening Balance";

            dataGridView1.Columns[12].Width = 90; //Pending Errors Amt
            dataGridView1.Columns[12].DefaultCellStyle = style;
            dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

        }

        // Print Full 
        private void buttonPrintFull_Click(object sender, EventArgs e)
        {

            string P1 = "STATUS OF ATMS GL RECONCILIATION";

            string P2 = "Second Par";
            string P3 = "Third Par";
            string P4 = WOperator;
            string P5 = WSignedId;

            Form56R74 ReportATMS74 = new Form56R74(P1, P2, P3, P4, P5);
            ReportATMS74.Show();
        }
        //Finish 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        // Catch Details 
        //
        private static void CatchDetails(Exception ex)
        {
            RRDMLog4Net Log = new RRDMLog4Net();

            StringBuilder WParameters = new StringBuilder();

            WParameters.Append("User : ");
            WParameters.Append("NotAssignYet");
            WParameters.Append(Environment.NewLine);

            WParameters.Append("ATMNo : ");
            WParameters.Append("NotDefinedYet");
            WParameters.Append(Environment.NewLine);

            string Logger = "RRDM4Atms";
            string Parameters = WParameters.ToString();

            Log.CreateAndInsertRRDMLog4NetMessage(ex, Logger, Parameters);

            System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
                + " . Application will be aborted! Call controller to take care. ");

          //  Environment.Exit(0);
        }
    }
}



