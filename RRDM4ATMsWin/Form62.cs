using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RRDM4ATMs; 
using System.Data.SqlClient;
using System.Configuration;
//multilingual
using System.Resources;
using System.Globalization;


namespace RRDM4ATMsWin
{
    public partial class Form62 : Form 
    {
        string Transfilter;

        Form67 NForm67;

        Form84 NForm84;

        Form56R6 NForm56R6; 

        int WTranNo;
        int WTraceNo;

        string WPrintTrace;
        string WPrintTraceDtTm;

        // DATATable for Grid 
 //       public DataTable GridDays = new DataTable();
        // DATATable for Grid 
        DataTable CardAtmsTran = new DataTable(); 

        DataTable dtAtmsTran = new DataTable();
        SqlDataAdapter daAtmsTran;

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        RRDMTracesReadUpdate Ta = new RRDMTracesReadUpdate();

        RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();

        RRDME_JournalTxtClass Ej = new RRDME_JournalTxtClass();

        //multilingual
        CultureInfo culture;

        RRDMUsersAndSignedRecord Xa = new RRDMUsersAndSignedRecord(); // Make class availble 

        ResourceManager LocRM = new ResourceManager("RRDM4ATMsWin.appRes", typeof(Form40).Assembly);

        
        
        string SQLString; 

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
   
        string WAtmNo;
        int WSesNo; 
        int WAction;
        DateTime WFromDate;
        DateTime WToDate;
        string WSingleChoice; 

        // Action = 21 is for Session Transactions, 22 for period, 23 for given card ,
        // 24 for given account, 25 for given Trace No
          

        public Form62(string InSignedId, int InSignRecordNo, string InOperator,string InAtmNo,
            int InSesNo, int InAction, DateTime FromDate, DateTime ToDate, string InSingleChoice)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
       
            WAtmNo = InAtmNo;
            WSesNo = InSesNo ;
            WFromDate = FromDate;
            WToDate = ToDate;
            WSingleChoice = InSingleChoice; 

            WAction = InAction; // 21 = Transactions For specific Session , 22 = Transactions per specific dates , 23 Trans for card

            InitializeComponent();

            if (WAction != 21)
            {
                button1.Hide(); 
            }

            Xa.ReadSignedActivityByKey(WSignRecordNo);

            if (Xa.Culture == "English")
            {
                culture = CultureInfo.CreateSpecificCulture("el-GR");
            }
            if (Xa.Culture == "Français")
            {
                culture = CultureInfo.CreateSpecificCulture("fr-FR");
            }


            if (WAction == 21) label1.Text = LocRM.GetString("Form62label1a", culture) + WSesNo.ToString();
            if (WAction == 22) label1.Text = LocRM.GetString("Form62label1b", culture) + FromDate.Date.ToString() + " -- " + ToDate.Date.ToString();
            if (WAction == 23) label1.Text = LocRM.GetString("Form62label1c", culture) + " " + WSingleChoice; // CARD
            if (WAction == 24) label1.Text = LocRM.GetString("Form62label1d", culture) + " " + WSingleChoice; // ACCOUNT
            if (WAction == 25) label1.Text = LocRM.GetString("Form62label1e", culture) + " " + WSingleChoice; // TRACE NUMBER 

            button1.Text = LocRM.GetString("Form62button1", culture);
            //button2.Text = LocRM.GetString("Form62button2", culture);
            button3.Text = LocRM.GetString("Form62button3", culture);
            button4.Text = LocRM.GetString("Form62button4", culture);
            button5.Text = LocRM.GetString("Form62button5", culture);
        }

        private void Form62_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'aTMSDataSet20.InPoolTrans' table. You can move, or remove it, as needed.
            this.inPoolTransTableAdapter.Fill(this.aTMSDataSet20.InPoolTrans);
            // TODO: This line of code loads data into the 'aTMSDataSet20.InPoolTrans' table. You can move, or remove it, as needed.
           
            // TODO: This line of code loads data into the 'aTMSDataSet28.InPoolTrans' table. You can move, or remove it, as needed.
            this.inPoolTransTableAdapter.Fill(this.aTMSDataSet20.InPoolTrans);
          
            if (WAction == 21) // SHOW TRANSACTIONS PER SESSION
            {
                Transfilter = "SesNo=" + WSesNo;
                inPoolTransBindingSource.Filter = Transfilter;
              //  dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Descending);
                this.inPoolTransTableAdapter.Fill(this.aTMSDataSet20.InPoolTrans);

                dataGridView1.Columns[0].HeaderText = LocRM.GetString("Form62Grd1Cl0", culture);
            }

            if (WAction == 22) // SHOW RANGE 
            {
                //  SQLString = "Select [AtmNo],[CurrDescr],[TranAmount]  FROM [dbo].[InPoolTrans] WHERE AtmNo= '" + WAtmNo + "'"
                //   + " AND AtmDtTime BETWEEN '" + WFromDate + "' AND '" + WToDate +"'";
                //    SQLString = "Select [AtmNo],[CurrDesc],[TranAmount]  FROM [dbo].[InPoolTrans]"
                SQLString = "Select * FROM [dbo].[InPoolTrans]"
                    + " WHERE AtmNo=@AtmNo AND AtmDtTime BETWEEN @WFromDate AND @WToDate";


                try
                {
                    SqlConnection conn =
                          new SqlConnection(connectionString);
                    using (
                        daAtmsTran = new SqlDataAdapter(SQLString, conn))
                    {
                        daAtmsTran.SelectCommand.Parameters.AddWithValue("@AtmNo", WAtmNo);
                        daAtmsTran.SelectCommand.Parameters.AddWithValue("@WFromDate", WFromDate);
                        daAtmsTran.SelectCommand.Parameters.AddWithValue("@WToDate", WToDate);

                        SqlCommandBuilder cmdBldr = new SqlCommandBuilder(daAtmsTran);

                        daAtmsTran.Fill(dtAtmsTran); // ATMs Numbers are now in data set table
                    }

                }

                catch (Exception ex)
                {

                    string exception = ex.ToString();
                    // MessageBox.Show(ex.ToString());
                    MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

                }

                int DtSize = dtAtmsTran.Rows.Count;
                string DtsSize = DtSize.ToString();
                //           MessageBox.Show(" Table Loaded ", DtsSize);

                dataGridView1.DataSource = dtAtmsTran.DefaultView;
            }

            if (WAction == 23) // SHOW TRANSACTIONS FOR CARD NUMBER 
            {
                Transfilter = "CardNo ='" + WSingleChoice + "'";
                inPoolTransBindingSource.Filter = Transfilter;
                //   dataGridView1.Sort(dataGridView1.Columns[6], ListSortDirection.Ascending);
                dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Descending);
                this.inPoolTransTableAdapter.Fill(this.aTMSDataSet20.InPoolTrans);
            }

            if (WAction == 24) // SHOW TRANSACTIONS FOR ACC Number  
            {
           //     AccNo LIKE '%013600004883%'
                Transfilter = "AccNo LIKE '%" + WSingleChoice + "%'";
                inPoolTransBindingSource.Filter = Transfilter;
                //   dataGridView1.Sort(dataGridView1.Columns[6], ListSortDirection.Ascending);
                dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Descending);
                this.inPoolTransTableAdapter.Fill(this.aTMSDataSet20.InPoolTrans);

                
            }

            if (WAction == 25) // SHOW TRANSACTION FOR TRACE Number  
            {
                int WAtmTraceNo = int.Parse(WSingleChoice);
                Transfilter = "AtmNo ='" + WAtmNo + "' AND EJournalTraceNo =" + WAtmTraceNo;
                inPoolTransBindingSource.Filter = Transfilter;
                //   dataGridView1.Sort(dataGridView1.Columns[6], ListSortDirection.Ascending);
                this.inPoolTransTableAdapter.Fill(this.aTMSDataSet20.InPoolTrans);

                
            }

            if (WAction == 26) // SHOW TRANSACTION FOR TRANNO
            {
                int WTranNo = int.Parse(WSingleChoice);
                Transfilter = "AtmNo ='" + WAtmNo + "' AND TranNo =" + WTranNo;
                inPoolTransBindingSource.Filter = Transfilter;
                //   dataGridView1.Sort(dataGridView1.Columns[6], ListSortDirection.Ascending);
                this.inPoolTransTableAdapter.Fill(this.aTMSDataSet20.InPoolTrans);
            }
        }
        //
        // ON ROW ENTER DEFINE TRANSACTION NO 
        //
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WTranNo = (int)rowSelected.Cells[0].Value;
            Tc.ReadInPoolTransSpecific(WTranNo);
            //WAtmNo = Tc.AtmNo; 
            WTraceNo = Tc.AtmTraceNo;
            textBoxTraceNo.Text = WTraceNo.ToString(); 

        }
        //
        // Show Video Clip
        //
        private void button3_Click(object sender, EventArgs e)
        {
            /*
             // Based on trace Number and start of transaction find the seconds. 
             
             SELECT SUBSTRING(LTRIM(RTRIM(TxtLine)), 6, 8) As TranDate
                   ,SUBSTRING(LTRIM(RTRIM(TxtLine)), 23, 8) As TransTime
                    FROM [ATMS_Journals].[dbo].[tblHstEjText]
                    where TraceNumber = 10042920 and Ruid = (44+3) 
                    order by traceNumber, RuId 
             */

            // Based on Transaction No show video clip 

            VideoWindow videoForm = new VideoWindow();
            videoForm.ShowDialog(); 
        }
        //
        // SHOW JOURNAL PART FOR THE CHOSEN TRANSACTION 
        //
        private void button4_Click(object sender, EventArgs e)
        {
            String JournalId = "[ATMS_Journals].[dbo].[tblHstEjText]";

            int Mode = 1; // Specific

            NForm67 = new Form67(WSignedId, WSignRecordNo, WOperator ,JournalId, 0, WAtmNo, WTraceNo, WTraceNo, Mode);
            NForm67.ShowDialog();

        }

        // FULL Journal 

        private void button5_Click(object sender, EventArgs e)
        {

            String JournalId = "[ATMS_Journals].[dbo].[tblHstEjText]";

            int Mode = 2 ; // FULL
 
            Tc.ReadInPoolTransSpecific(WTranNo);

            Ta.ReadSessionsStatusTraces(Tc.AtmNo, Tc.SesNo);

            RRDME_JournalTxtClass Jt = new RRDME_JournalTxtClass();

            Jt.ReadJournalTextByTrace(WOperator, Tc.AtmNo, Ta.FirstTraceNo);

            int FileInJournal = Jt.FuId; 

            // WE SHOULD FIND OUT THE START AND OF THIS REPL. CYCLE 
            NForm67 = new Form67(WSignedId, WSignRecordNo, WOperator, JournalId, FileInJournal, Tc.AtmNo, Ta.FirstTraceNo, Ta.LastTraceNo, Mode);
            NForm67.Show();
        }
       
        // ROW DOUBLE CLICK 
        private void dataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            String JournalId = "[ATMS_Journals].[dbo].[tblHstEjText]";

            int Mode = 1; // Specific

       //     Tc.ReadTranSpecific(WTranNo);

            NForm67 = new Form67(WSignedId, WSignRecordNo, WOperator, JournalId, 0, WAtmNo,  WTraceNo, WTraceNo, Mode);
            NForm67.Show();
        }
        // Print All 
        private void button1_Click(object sender, EventArgs e)
        {
            if (WAction == 21) // Print TRANSACTIONS PER SESSION
            {
                Transfilter = "SesNo=" + WSesNo;
                NForm56R6 = new Form56R6(WOperator, WAtmNo, "356", WSesNo, 11);
                NForm56R6.Show();
                
            }
        }
        // DATES TRAIL 
        private void button6_Click(object sender, EventArgs e)
        {
            NForm84 = new Form84(WSignedId, WSignRecordNo, WOperator, WAtmNo, WTraceNo, WTranNo);
            NForm84.Show();
        }
// Print Journal Trace 
        private void buttonPrintTrace_Click(object sender, EventArgs e)
        {
            Ej.ReadJournalTextByTrace(WOperator, WAtmNo, WTraceNo);

            WPrintTraceDtTm = Ej.TransDate.ToString();
            WPrintTrace = WTraceNo.ToString();


            // TRACE
            // Show all lines for a TRACE NUMBER  for a specific ATM

            Form56R16 PrintJournal = new Form56R16(WOperator, WPrintTrace, WPrintTraceDtTm, WAtmNo);
            PrintJournal.Show();
        }
                    
         }
    }
 
    

