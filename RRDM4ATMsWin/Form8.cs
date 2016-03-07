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
    public partial class Form8 : Form
    {
        Form16 NForm16; 

        string connectionString = ConfigurationManager.ConnectionStrings
          ["ATMSConnectionString"].ConnectionString;    

        RRDMGasParameters Gp = new RRDMGasParameters();
        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();

    //    string WOperator;

        string ModelAtm; 
        string WCriticalProcess;
        string ParCriticalProcess;
        DateTime WDtTm;

        string WSignedId;
        int WSignRecordNo;
        int WSecLevel;
        string WOperator;
  //      bool WPrive;

        int WAction;

        public Form8(string InSignedId, int SignRecordNo, int InSecLevel, string InOperator, int InAction)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;
          //  WPrive = InPrive;

            WAction = InAction;  // 1 = System Performance 
           
            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;

            labelStep1.Text = "System Performance is based on Model ATM and Critical Process.";

            // Model ATMS 
            Gp.ParamId = "350"; 
            comboBox1.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBox1.DisplayMember = "DisplayValue";

            comboBox1.Text = "AB104"; 

            // Critical Process 
            Gp.ParamId = "351"; 
            comboBox2.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBox2.DisplayMember = "DisplayValue";

            comboBox2.Text = "KontoLoadJournal"; 

            textBoxMsgBoard.Text = " Make you selection and press button Show";

            label1.Hide();
            panel2.Hide(); 

        }
        // On Load form 
        private void Form8_Load(object sender, EventArgs e)
        {
           
        }
        // SHOW 
        private void button2_Click(object sender, EventArgs e)
        {
            DataTable PerformanceTbl = new DataTable();

            PerformanceTbl.Clear();

            WCriticalProcess = comboBox2.Text; 

            // System performance For Model ATM AB104
            // 
            if (WAction == 1) //  
            {
                label1.Show();
                panel2.Show();
                       
                ModelAtm = comboBox1.Text;

                label1.Text = "System Performance for ATM : " + ModelAtm + " And Critical Process: " + WCriticalProcess ;

                ParCriticalProcess = "%" + WCriticalProcess + "%"; 

                string SqlString2 =
                    " SELECT "
                    + " CAST(StartDT AS Date) As Entry_Date, "
                     + " Sum(Duration) As TotalDuration,"
                    + " Max(Duration) AS Max_Duration, Sum(Counter) As Counter, (Sum(Counter)/Sum(Duration)) As PerSec "
                    + " FROM [ATMS].[dbo].[PerformanceTrace] "
                    + " WHERE AtmNo = @AtmNo AND ProcessNm LIKE @ProcessNm"
                    + " GROUP BY CAST(StartDT AS Date) "
                    + " ORDER BY CAST(StartDT AS Date) DESC ";

                using (SqlConnection conn =
                            new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();

                        //Create an Sql Adapter that holds the connection and the command
                        SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString2, conn);

                  //      sqlAdapt.SelectCommand.Parameters.AddWithValue("@BankId", WBankId);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@AtmNo", ModelAtm);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@ProcessNm", ParCriticalProcess);

                        //Create a datatable that will be filled with the data retrieved from the command
                        //    DataSet MISds = new DataSet();
                        sqlAdapt.Fill(PerformanceTbl);

                        //Fill the dataGrid that will be displayed with the dataset
                        dataGridView1.DataSource = PerformanceTbl.DefaultView;

                        // Close conn
                        conn.Close();

                    }

                    catch (Exception ex)
                    {

                        string exception = ex.ToString();
                        MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

                    }

                dataGridView1.Columns[0].Width = 85; // 
                dataGridView1.Columns[1].Width = 85; // 

                //     dataGridView1.Columns[0].HeaderText = LocRM.GetString("Form67Grd1Cl0", culture);
                //     dataGridView1.Columns[1].HeaderText = LocRM.GetString("Form67Grd1Cl01", culture);
                //     dataGridView1.Columns[2].HeaderText = LocRM.GetString("Form67Grd1Cl02", culture);

                dataGridView1.Show();

                // Set chart data source  
                chart1.DataSource = PerformanceTbl.DefaultView;
                chart1.Series[0].Name = "Per Second Efficinecy by Date";

                // Set series members names for the X and Y values  
                chart1.Series[0].XValueMember = "Entry_Date";
                chart1.Series[0].YValueMembers = "PerSec";

                // Data bind to the selected data source  
                chart1.DataBind();

            }

        }

        // On ROW ENTER 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            WDtTm = (DateTime)rowSelected.Cells[0].Value;
        }

        // Double Click on Row - EXPAND LINE
        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            NForm16 = new Form16(ModelAtm, WCriticalProcess, WDtTm);
            NForm16.Show();

        }
        // EXPAND LINE 
        private void button1_Click(object sender, EventArgs e)
        {
            NForm16 = new Form16(ModelAtm, WCriticalProcess, WDtTm);
            NForm16.Show();
        }

    }
}
