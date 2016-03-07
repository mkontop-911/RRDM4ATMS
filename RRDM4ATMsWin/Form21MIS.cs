using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using RRDM4ATMs; 
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
//multilingual
using System.Resources;
using System.Globalization;

namespace RRDM4ATMsWin
{
    public partial class Form21MIS : Form
    {
        Form35 NForm35; // Enlarge chart 
          Form22MIS NForm22MIS; 
        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        DataTable MISRepl1 = new DataTable();

        DateTime WDtTm;

        DateTime NullFutureDate = new DateTime(2050, 11, 21);

        //TEST RANGE 
        DateTime WorkingDtA = new DateTime(2014, 04, 12); 
        DateTime WorkingDtB = new DateTime(2014, 04, 19);

        //TEST RANGE 
        DateTime WorkingDtC = new DateTime(2014, 05, 09);
        DateTime WorkingDtD = new DateTime(2014, 06, 18); 

        //multilingual
        CultureInfo culture;

        RRDMUsersAndSignedRecord Xa = new RRDMUsersAndSignedRecord(); // Make class availble 

        ResourceManager LocRM = new ResourceManager("RRDM4ATMsWin.appRes", typeof(Form40).Assembly);

        string WCitId;

     //   int WF=0;

        string WUserId;
        string WAtmNo; 
  
        string WCurrNm;

    //    string WOperator;

        string WSignedId;
        int WSignRecordNo;
        int WSecLevel;
        string WOperator;
 
        int WAction;

        public Form21MIS(string InSignedId, int SignRecordNo, int InSecLevel, string InOperator, int InAction)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;
    
            WAction = InAction;  // 1 = By Period Quality of Repl and Reconc, 
                                 // 2 = By User performance , 
                                 // 3 = By ATMs Daily Business 
                                 // 4 = Cit Analysis
                                 // 5 = Dispute Analysis 
                                 // 6 = By ATM Profitability 

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;
        }
       
        private void Form21MIS_Load(object sender, EventArgs e)
        {
            
            Xa.ReadSignedActivityByKey(WSignRecordNo);

            if (Xa.Culture == "English")
            {
                culture = CultureInfo.CreateSpecificCulture("el-GR");
            }
            if (Xa.Culture == "Français")
            {
                culture = CultureInfo.CreateSpecificCulture("fr-FR");
            }

      //      if (WMode == 1) label14.Text = LocRM.GetString("Form67label14a", culture);
      //      if (WMode == 2) label14.Text = LocRM.GetString("Form67label14b", culture);

            textBoxMsgBoard.Text = " Use expand Button to see analysis of DataGrid first column";

            RRDMBanks Ba = new RRDMBanks();
            Ba.ReadBank(WOperator);
            WCurrNm = Ba.BasicCurName; 
           
            MISRepl1 = new DataTable();
            MISRepl1.Clear();

            if (WAction == 1) // By Period Quality Indicators 
            {

                labelStep1.Text = "Management information for Replenish and Reconciliation"; 
                label2.Text = "PERIOD vs SET OF INDICATORS";
                label1.Text = "PERIOD vs CASH UTILIZATION";

            

                string SqlString2 =
                    "SELECT CAST(ReplDate AS Date) AS Date, Count(AtmNo) As No_Of_Atms, SUM(ReplMinutes) AS RepMin, Avg(ReplMinutes) AS Avg_Repl,"
            + " SUM(ErrorsAtm) As ErrATM,SUM(ErrorsHost) AS ErrHost, TotErr = SUM(ErrorsAtm + ErrorsHost), "
            + "  AbsDiff = SUM(DiffPlus + DiffMinus),"
            + " CashUtil=(Sum(RemainMoney)/Sum(InMoneyLast)), SUM(NotReconc) As NotReconc"
            + " FROM [ATMS].[dbo].[ReplStatsTable] "
            + " WHERE Operator = @Operator AND ReplDate > @WorkingDtA AND ReplDate < @WorkingDtB"
            + " GROUP BY CAST(ReplDate AS Date) "
            + " ORDER BY CAST(ReplDate AS Date) DESC ";

                using (SqlConnection conn =
                            new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();

                        //Create an Sql Adapter that holds the connection and the command
                        SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString2, conn);

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", WOperator);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@WorkingDtA", WorkingDtA);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@WorkingDtB", WorkingDtB);

                        //Create a datatable that will be filled with the data retrieved from the command
                        //    DataSet MISds = new DataSet();
                        sqlAdapt.Fill(MISRepl1);

                        //Fill the dataGrid that will be displayed with the dataset
                        dataGridView1.DataSource = MISRepl1.DefaultView;

                        // Close conn
                        conn.Close();

                    }

                    catch (Exception ex)
                    {

                        string exception = ex.ToString();
                        MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

                    }


                //  MessageBox.Show(" Number of ATMS = " + I);


                dataGridView1.Columns[0].Width = 65; // Count ATM no
                dataGridView1.Columns[1].Width = 65; // Repl Date
                dataGridView1.Columns[2].Width = 65; // Total Repl Minutes 
                dataGridView1.Columns[3].Width = 65; // Average Reple
                dataGridView1.Columns[4].Width = 65; // Errors ATm 
                dataGridView1.Columns[5].Width = 65; // Errors Host
                dataGridView1.Columns[6].Width = 65; // Total Errors
                dataGridView1.Columns[7].Width = 65; // aBSOLUTE Difference
                dataGridView1.Columns[8].Width = 65; // Cash utilization 
                dataGridView1.Columns[9].Width = 65; // Not Reconciled 


                //     dataGridView1.Columns[0].HeaderText = LocRM.GetString("Form67Grd1Cl0", culture);
                //     dataGridView1.Columns[1].HeaderText = LocRM.GetString("Form67Grd1Cl01", culture);
                //     dataGridView1.Columns[2].HeaderText = LocRM.GetString("Form67Grd1Cl02", culture);


                dataGridView1.Show();

                // Set chart2 data source  
                chart2.DataSource = MISRepl1.DefaultView;
                chart2.Series[0].Name = "Avg_Repl";
                chart2.Series.Add("TotErr");
                chart2.Series.Add("NotReconc");
                // Set series members names for the X and Y values  
                chart2.Series[0].XValueMember = "Date";
                chart2.Series[0].YValueMembers = "Avg_Repl";
                chart2.Series[1].XValueMember = "Date";
                chart2.Series[1].YValueMembers = "TotErr";
                chart2.Series[2].XValueMember = "Date";
                chart2.Series[2].YValueMembers = "NotReconc";

                // Data bind to the selected data source  
                chart2.DataBind();

                // Set chart data source  
                chart1.DataSource = MISRepl1.DefaultView;
                chart1.Series[0].Name = "CashUtil";
                // Set series members names for the X and Y values  
                chart1.Series[0].XValueMember = "Date";
                chart1.Series[0].YValueMembers = "CashUtil";

                // Data bind to the selected data source  
                chart1.DataBind();
               
            }

            // PERFORMANCE BY USER 

            if (WAction == 2) // Performance per USER 
            {
               
                labelStep1.Text = "User Performance Analysis"; 
                label14.Text = "BY USER LISTING OF KEY PERFORMANCE"; 

                string SqlString2 =
                    "SELECT UserId, Min(ReplMinutes) AS RepMin, Max(ReplMinutes) AS RepMax, Avg(ReplMinutes) AS Avg_Repl,"
            + " SUM(ErrorsAtm) As ErrATM,SUM(ErrorsHost) AS ErrHost,TotErr = SUM(ErrorsAtm + ErrorsHost), "
            + "  AbsDiff = SUM(DiffPlus + DiffMinus),"
            + " Sum(InMoneyLast) As TotInMoney, Sum(InMoneyLast-RemainMoney) As TotUsed,"
            + " CashUtil=(Sum(InMoneyLast-RemainMoney)/Sum(InMoneyLast)), SUM(NotReconc) As NotReconc"
            + " FROM [ATMS].[dbo].[ReplStatsTable]"
            + " WHERE Operator = @Operator"
            + " GROUP BY UserId"
            + " ORDER BY Avg_Repl ASC ";

                using (SqlConnection conn =
                            new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();

                        //Create an Sql Adapter that holds the connection and the command
                        SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString2, conn);

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", WOperator);

                        //Create a datatable that will be filled with the data retrieved from the command
                        //    DataSet MISds = new DataSet();
                        sqlAdapt.Fill(MISRepl1);

                        //Fill the dataGrid that will be displayed with the dataset
                        dataGridView1.DataSource = MISRepl1.DefaultView;

                        // Close conn
                        conn.Close();

                    }

                    catch (Exception ex)
                    {
                        string exception = ex.ToString();
                        MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));
                    }


                //  MessageBox.Show(" Number of ATMS = " + I);

                dataGridView1.Columns[0].Width = 60; // 
                dataGridView1.Columns[1].Width = 60; // 
                dataGridView1.Columns[2].Width = 60; //
                dataGridView1.Columns[3].Width = 60; // 
                dataGridView1.Columns[4].Width = 60; // 
                dataGridView1.Columns[5].Width = 60; // 
                dataGridView1.Columns[6].Width = 60; // 
                dataGridView1.Columns[7].Width = 65; // 
                dataGridView1.Columns[8].Width = 65; // 
                dataGridView1.Columns[9].Width = 65; // 
                dataGridView1.Columns[10].Width = 65; // 
                dataGridView1.Columns[11].Width = 65; // 

                //     dataGridView1.Columns[0].HeaderText = LocRM.GetString("Form67Grd1Cl0", culture);
                //     dataGridView1.Columns[1].HeaderText = LocRM.GetString("Form67Grd1Cl01", culture);
                //     dataGridView1.Columns[2].HeaderText = LocRM.GetString("Form67Grd1Cl02", culture);


                dataGridView1.Show();


                label2.Text = "USERS VS ERRORS AND NOT RECONC";

                // Set chart2 data source  
                chart2.DataSource = MISRepl1.DefaultView;
                chart2.Series[0].Name = "TotErr";
                chart2.Series.Add("NotReconc");
                // Set series members names for the X and Y values  
                chart2.Series[0].XValueMember = "UserId";
                chart2.Series[0].YValueMembers = "TotErr";
                chart2.Series[1].XValueMember = "UserId";
                chart2.Series[1].YValueMembers = "NotReconc";

                // Data bind to the selected data source  
                chart2.DataBind();

               
                // Set chart data source  
                label1.Text = "USERS VS AVER TIME OF REPL.";

                chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;

                chart1.DataSource = MISRepl1.DefaultView;
                chart1.Series[0].Name = "Avg_Repl";
                // Set series members names for the X and Y values  
                chart1.Series[0].XValueMember = "UserId";
                chart1.Series[0].YValueMembers = "Avg_Repl";

                // Data bind to the selected data source  
                chart1.DataBind();
            
            }

     

            if (WAction == 3) // By Period ATMs Turnover Indicators 
            {
                labelStep1.Text = "Management information for money turnover"; 
                label2.Text = "PERIOD vs INDICATORS FOR DISPENSED";
                label1.Text = "PERIOD vs INDICATORS FOR DEPOSITS";
                //TEST
                // SELECT STATEMENT TO GET DATA BY MONTH PERIOD INSTEAD OF DATE
           /*     string SqlStringMM = " SELECT " 
                    + " DATEPART(Year, DtTm) Year,"
                    + " DATEPART(Month, DtTm) Month, SUM(DrTransactions) [TotalAmount]"
                    + " FROM [ATMS].[dbo].[AtmDispAmtsByDay]  "
                    + " GROUP BY DATEPART(Year, DtTm), DATEPART(Month, DtTm)"
                    + " ORDER BY Year, Month "; 
*/
                string SqlStringDD =
                    "SELECT CAST(DtTm AS Date) AS Date, Count(AtmNo) As No_Of_Atms,"
                    +" SUM(DrTransactions) AS DR_TRANS,SUM(DispensedAmt) AS DR_Amount,"
                    +" Avg(DrTransactions) AS Avg_DrTrans, Avg(DispensedAmt) AS Avg_DrAmount,"
                    +" SUM(CrTransactions) AS CR_TRANS,SUM(DepAmount) AS CR_Amount,"
                     +" Avg(CrTransactions) AS Avg_CrTrans, Avg(DepAmount) AS Avg_CrAmount"
            + " FROM [ATMS].[dbo].[AtmDispAmtsByDay] "
            + " WHERE Operator = @Operator AND Year =@Year"
            + " GROUP BY CAST(DtTm AS Date) "
            + " ORDER BY CAST(DtTm AS Date) DESC ";

                /*
                SELECT CAST(DispDtTm AS Date), 
Count(DispTranNo) As No_Of_Tran_Disp,
                    SUM(TranAmount) AS Total_Disputed,
                     Avg(TranAmount) AS Avg_TranAmount
                    FROM [ATMS].[dbo].[DisputesTransTable] 
                    WHERE Operator = 'CRBAGRAA' 
					GROUP BY CAST(DispDtTm AS Date)
                    ORDER BY CAST(DispDtTm AS Date) DESC
 */

                using (SqlConnection conn =
                            new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();


                        //Create an Sql Adapter that holds the connection and the command
                        SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlStringDD, conn);

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", WOperator);

                         sqlAdapt.SelectCommand.Parameters.AddWithValue("@Year", 2014);

                        //Create a datatable that will be filled with the data retrieved from the command
                        //    DataSet MISds = new DataSet();
                        sqlAdapt.Fill(MISRepl1);

                        //Fill the dataGrid that will be displayed with the dataset
                        dataGridView1.DataSource = MISRepl1.DefaultView;

                        // Close conn
                        conn.Close();

                    }

                    catch (Exception ex)
                    {

                        string exception = ex.ToString();
                        MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

                    }

                dataGridView1.Columns[0].Width = 65; // 
                dataGridView1.Columns[1].Width = 65; // 
                dataGridView1.Columns[2].Width = 65; // 
                dataGridView1.Columns[3].Width = 65; // 
                dataGridView1.Columns[4].Width = 65; // 
                dataGridView1.Columns[5].Width = 65; // 
                dataGridView1.Columns[6].Width = 65; // 
                dataGridView1.Columns[7].Width = 65; // 
                dataGridView1.Columns[8].Width = 65; // 
                dataGridView1.Columns[9].Width = 65; //  


                //     dataGridView1.Columns[0].HeaderText = LocRM.GetString("Form67Grd1Cl0", culture);
                //     dataGridView1.Columns[1].HeaderText = LocRM.GetString("Form67Grd1Cl01", culture);
                //     dataGridView1.Columns[2].HeaderText = LocRM.GetString("Form67Grd1Cl02", culture);


                dataGridView1.Show();

                // Set chart2 data source  
                chart2.DataSource = MISRepl1.DefaultView;
                chart2.Series[0].Name = "Avg_DrTrans";
                chart2.Series.Add("Avg_DrAmount");
                
                // Set series members names for the X and Y values  
                chart2.Series[0].XValueMember = "Date";
                chart2.Series[0].YValueMembers = "Avg_DrTrans";
                chart2.Series[1].XValueMember = "Date";
                chart2.Series[1].YValueMembers = "Avg_DrAmount";

                // Data bind to the selected data source  
                chart2.DataBind();

                // Set chart data source  
                chart1.DataSource = MISRepl1.DefaultView;
                chart1.Series[0].Name = "Avg_CrTrans";
                chart1.Series.Add("Avg_CrAmount");
                // Set series members names for the X and Y values  
                chart1.Series[0].XValueMember = "Date";
                chart1.Series[0].YValueMembers = "Avg_CrTrans";
                chart1.Series[1].XValueMember = "Date";
                chart1.Series[1].YValueMembers = "Avg_CrAmount";

                // Data bind to the selected data source  
                chart1.DataBind();

            }

            if (WAction == 4) // FOR REPLENISHMENT CIT 
            {
                labelStep1.Text = "Status Analysis By Repl CIT providers";
                label14.Text = "BY REPL CIT LISTING OF KEY PERFORMANCE"; 

                label2.Text = "CITs GROUP vs KEY INDICATORS";
                label1.Text = "CITs vs ERRORS";

                string SqlString2 =
                     "SELECT CitId, Count(DISTINCT AtmNo) As No_Of_Atms, SUM(ReplMinutes) AS TotRepMin, Avg(ReplMinutes) AS Avg_Repl,"
            + " SUM(ErrorsAtm) As ErrATM,SUM(ErrorsHost) AS ErrHost, TotErr = SUM(ErrorsAtm + ErrorsHost), "
            + "  AbsDiff = SUM(DiffPlus + DiffMinus),"
            + " CashUtil=(Sum(RemainMoney)/Sum(InMoneyLast)), SUM(NotReconc) As NotReconc"
            + " FROM [ATMS].[dbo].[ReplStatsTable] "
            + " WHERE Operator = @Operator"
            + " GROUP BY CitId "
            + " ORDER BY CitId ASC ";

                using (SqlConnection conn =
                            new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();


                        //Create an Sql Adapter that holds the connection and the command
                        SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString2, conn);

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", WOperator);

                  //      sqlAdapt.SelectCommand.Parameters.AddWithValue("@Year", 2014);

                        //Create a datatable that will be filled with the data retrieved from the command
                        //    DataSet MISds = new DataSet();
                        sqlAdapt.Fill(MISRepl1);

                        //Fill the dataGrid that will be displayed with the dataset
                        dataGridView1.DataSource = MISRepl1.DefaultView;

                        // Close conn
                        conn.Close();

                    }

                    catch (Exception ex)
                    {

                        string exception = ex.ToString();
                        MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

                    }

                dataGridView1.Columns[0].Width = 75; // 
                dataGridView1.Columns[1].Width = 75; // 
                dataGridView1.Columns[2].Width = 75; // 
                dataGridView1.Columns[3].Width = 75; // 
                dataGridView1.Columns[4].Width = 75; // 
                dataGridView1.Columns[5].Width = 75; // 
                dataGridView1.Columns[6].Width = 75; // 
                dataGridView1.Columns[7].Width = 75; // 
                dataGridView1.Columns[8].Width = 75; // 
                dataGridView1.Columns[9].Width = 75; //  


                //     dataGridView1.Columns[0].HeaderText = LocRM.GetString("Form67Grd1Cl0", culture);
                //     dataGridView1.Columns[1].HeaderText = LocRM.GetString("Form67Grd1Cl01", culture);
                //     dataGridView1.Columns[2].HeaderText = LocRM.GetString("Form67Grd1Cl02", culture);


                dataGridView1.Show();

                // Set chart2 data source  
                chart2.DataSource = MISRepl1.DefaultView;
                chart2.Series[0].Name = "Avg_Repl";
                chart2.Series.Add("TotErr");
                chart2.Series.Add("NotReconc");
                // Set series members names for the X and Y values  
                chart2.Series[0].XValueMember = "CitId";
                chart2.Series[0].YValueMembers = "Avg_Repl";
                chart2.Series[1].XValueMember = "CitId";
                chart2.Series[1].YValueMembers = "TotErr";
                chart2.Series[2].XValueMember = "CitId";
                chart2.Series[2].YValueMembers = "NotReconc";

                // Data bind to the selected data source  
                chart2.DataBind();

                // Set chart data source  
                chart1.DataSource = MISRepl1.DefaultView;
                chart1.Series[0].Name = "CashUtil";
                // Set series members names for the X and Y values  
                chart1.Series[0].XValueMember = "CitId";
                chart1.Series[0].YValueMembers = "CashUtil";

                // Data bind to the selected data source  
                chart1.DataBind();

            }
            // 
            // Disputes by period 
            // 
            if (WAction == 5) // Diputes by period 
            {
                labelStep1.Text = "Management information for Disputes"; 
                label2.Text = "PERIOD vs NO OF DISPUTES";
                label1.Text = "PERIOD vs DISPUTED AMOUNT";

                string SqlString2 =
                    "SELECT "
                    + " CAST(DispDtTm AS Date) As Disp_Date,  Count(DispTranNo) As No_Of_Tran_Disp,"
                    + " SUM(TranAmount) AS Total_Disputed,"
                    + " Avg(TranAmount) AS Avg_TranAmount"
                    + " FROM [ATMS].[dbo].[DisputesTransTable] "
                    + " WHERE Operator = @Operator AND CurrencyNm=@CurrencyNm AND DispDtTm > @WorkingDtC AND DispDtTm < @WorkingDtD"
                    + " GROUP BY CAST(DispDtTm AS Date) "
                    + " ORDER BY CAST(DispDtTm AS Date) DESC ";
/*
                SELECT CAST(DispDtTm AS Date), 
Count(DispTranNo) As No_Of_Tran_Disp,
                    SUM(TranAmount) AS Total_Disputed,
                     Avg(TranAmount) AS Avg_TranAmount
                    FROM [ATMS].[dbo].[DisputesTransTable] 
                    WHERE Operator = 'CRBAGRAA' 
					GROUP BY CAST(DispDtTm AS Date)
                    ORDER BY CAST(DispDtTm AS Date) DESC
 */

                using (SqlConnection conn =
                            new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();


                        //Create an Sql Adapter that holds the connection and the command
                        SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString2, conn);

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", WOperator);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@CurrencyNm", WCurrNm);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@WorkingDtC", WorkingDtC);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@WorkingDtD", WorkingDtD);

                        //Create a datatable that will be filled with the data retrieved from the command
                        //    DataSet MISds = new DataSet();
                        sqlAdapt.Fill(MISRepl1);

                        //Fill the dataGrid that will be displayed with the dataset
                        dataGridView1.DataSource = MISRepl1.DefaultView;

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
                dataGridView1.Columns[2].Width = 85; // 
                dataGridView1.Columns[3].Width = 85; // 
             


                //     dataGridView1.Columns[0].HeaderText = LocRM.GetString("Form67Grd1Cl0", culture);
                //     dataGridView1.Columns[1].HeaderText = LocRM.GetString("Form67Grd1Cl01", culture);
                //     dataGridView1.Columns[2].HeaderText = LocRM.GetString("Form67Grd1Cl02", culture);


                dataGridView1.Show();

                // Set chart2 data source  
                chart2.DataSource = MISRepl1.DefaultView;
                chart2.Series[0].Name = "No_Of_Tran Disp";
                // Set series members names for the X and Y values  
                chart2.Series[0].XValueMember = "Disp_Date";
                chart2.Series[0].YValueMembers = "No_Of_Tran_Disp";
                // Data bind to the selected data source  
                chart2.DataBind();

                // Set chart data source  
                chart1.DataSource = MISRepl1.DefaultView;
                chart1.Series[0].Name = "Total_Disputed";
                
                // Set series members names for the X and Y values  
                chart1.Series[0].XValueMember = "Disp_Date";
                chart1.Series[0].YValueMembers = "Total_Disputed";
               

                // Data bind to the selected data source  
                chart1.DataBind();

            }

            // PROFITABILITY BY ATM  

            if (WAction == 6) // PROFITABILITY BY ATM  
            {

                labelStep1.Text = "ATM profitability Analysis";
                label14.Text = "BY ATM LISTING OF KEY PROFITABILITY CONTRIBUTORS";

                string SqlString2 =
                " SELECT [ATMNO]"
               + " , Sum ([C301DailyMaintAmount] + [C303ReplTimeCost] +[C307OverheadCost] + [C308CostOfMoney]+ [C309CostOfInvest]) As COST"
               + " , Sum ([R401CommTran] + [R402CommTran] + [R403CommTran] + [R404CommTran] + [R405CommTran]) As REVENUE"
               + " , Sum (([R401CommTran] + [R402CommTran] + [R403CommTran] + [R404CommTran] + [R405CommTran])"
                          + "- ([C301DailyMaintAmount] + [C303ReplTimeCost] +[C307OverheadCost] + [C308CostOfMoney]+ [C309CostOfInvest])) AS PROFIT "
               + " , Sum([C301DailyMaintAmount]) As MaintCost, Sum([C303ReplTimeCost]) As ReplCost"
               + ", Sum([C307OverheadCost]) As Overheads, Sum([C308CostOfMoney]) As CostOfMoney "
               + ", Sum([C309CostOfInvest]) As InvestCost"
               + ", Sum([R401CommTran]) As Comm1Tran , Sum([R401CommAmount]) As Comm1Amount"
               + ", Sum([R402CommTran]) As Comm2Tran , Sum([R402CommAmount]) As Comm2Amount"
               + ", Sum([R403CommTran]) As Comm3Tran , Sum([R403CommAmount]) As Comm3Amount"
               + ", Sum([R404CommTran]) As Comm4Tran , Sum([R404CommAmount]) As Comm4Amount"
               + ", Sum([R405CommTran]) As Comm5Tran , Sum([R405CommAmount]) As Comm5Amount"
               + " FROM [ATMS].[dbo].[AtmDispAmtsByDay] "
               + " WHERE Operator = @Operator "
               + " GROUP by [AtmNo] "
               + " ORDER by AtmNo ";

                using (SqlConnection conn =
                            new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();

                        //Create an Sql Adapter that holds the connection and the command
                        SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString2, conn);

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", WOperator);

                        //Create a datatable that will be filled with the data retrieved from the command
                        //    DataSet MISds = new DataSet();
                        sqlAdapt.Fill(MISRepl1);

                        //Fill the dataGrid that will be displayed with the dataset
                        dataGridView1.DataSource = MISRepl1.DefaultView;

                        // Close conn
                        conn.Close();

                    }

                    catch (Exception ex)
                    {
                        string exception = ex.ToString();
                        MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));
                    }


                //  MessageBox.Show(" Number of ATMS = " + I);

                dataGridView1.Columns[0].Width = 65; // 
                dataGridView1.Columns[1].Width = 65; // 
                dataGridView1.Columns[2].Width = 65; //
                dataGridView1.Columns[3].Width = 65; // 
                dataGridView1.Columns[4].Width = 65; // 
                dataGridView1.Columns[5].Width = 65; // 
                dataGridView1.Columns[6].Width = 65; // 
                dataGridView1.Columns[7].Width = 65; // 
                dataGridView1.Columns[8].Width = 65; // 
                dataGridView1.Columns[9].Width = 65; // 

                dataGridView1.Columns[10].Width = 65; // 

                dataGridView1.Columns[11].Width = 65; // 
                dataGridView1.Columns[12].Width = 65; //
                dataGridView1.Columns[13].Width = 65; // 
                dataGridView1.Columns[14].Width = 65; // 
                dataGridView1.Columns[15].Width = 65; // 
                dataGridView1.Columns[16].Width = 65; // 
                dataGridView1.Columns[17].Width = 65; // 
                dataGridView1.Columns[18].Width = 65; // 

                //     dataGridView1.Columns[0].HeaderText = LocRM.GetString("Form67Grd1Cl0", culture);
                //     dataGridView1.Columns[1].HeaderText = LocRM.GetString("Form67Grd1Cl01", culture);
                //     dataGridView1.Columns[2].HeaderText = LocRM.GetString("Form67Grd1Cl02", culture);


                dataGridView1.Show();


                label2.Text = "ATMs VS COST AND REVENUE AND PROFIT";

                // Set chart2 data source  
                chart2.DataSource = MISRepl1.DefaultView;
                chart2.Series[0].Name = "COST";
                chart2.Series.Add("REVENUE");
                chart2.Series.Add("PROFIT");
                // Set series members names for the X and Y values  
                chart2.Series[0].XValueMember = "ATMNO";
                chart2.Series[0].YValueMembers = "COST";
                chart2.Series[1].XValueMember = "ATMNO";
                chart2.Series[1].YValueMembers = "REVENUE";
                chart2.Series[2].XValueMember = "ATMNO";
                chart2.Series[2].YValueMembers = "PROFIT";

                // Data bind to the selected data source  
                chart2.DataBind();


                // Set chart data source  
                label1.Text = "ANALYSIS OF ATM PROFIT";

                chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;

                chart1.DataSource = MISRepl1.DefaultView;
                chart1.Series[0].Name = "PROFIT";
                // Set series members names for the X and Y values  
                chart1.Series[0].XValueMember = "ATMNO";
                chart1.Series[0].YValueMembers = "PROFIT";

                // Data bind to the selected data source  
                chart1.DataBind();
                
            }

            // 
           
        }
     

        // A ROW IS SELECTED ASSIGN VALUES 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            if (WAction == 1)
            {
                WDtTm = (DateTime)rowSelected.Cells[0].Value;
            }
            if (WAction == 2)
            {
                WUserId = (string)rowSelected.Cells[0].Value;
            }

            if (WAction == 3)
            {
                WDtTm = (DateTime)rowSelected.Cells[0].Value;
            }

            if (WAction == 4)
            {
                WCitId = (string)rowSelected.Cells[0].Value;
            }

            if (WAction == 5)
            {
                WDtTm = (DateTime)rowSelected.Cells[0].Value;
            }

            if (WAction == 6)
            {
                WAtmNo = (string)rowSelected.Cells[0].Value;

            }



        }
        // BASED ON Selected Row Expand 
        private void button2_Click(object sender, EventArgs e)
        {
            if (WAction == 1)
            {

                NForm22MIS = new Form22MIS(WSignedId, WSignRecordNo, WOperator, WUserId, WDtTm ,"","",21);
                NForm22MIS.Show();
            }
            if (WAction == 2)
            {

                NForm22MIS = new Form22MIS(WSignedId, WSignRecordNo, WOperator, WUserId, NullFutureDate,  "","", 22);
                NForm22MIS.Show();
                 
            }

            if (WAction == 3)
            {

                NForm22MIS = new Form22MIS(WSignedId, WSignRecordNo, WOperator, WUserId, WDtTm,  "","", 23);
                NForm22MIS.Show();

            }

            if (WAction == 4)
            {

                NForm22MIS = new Form22MIS(WSignedId, WSignRecordNo, WOperator, WUserId, WDtTm,  WCitId,"", 24);
                NForm22MIS.Show();

            }

            if (WAction == 5)
            {

                NForm22MIS = new Form22MIS(WSignedId, WSignRecordNo, WOperator, WUserId, WDtTm, "" ,"", 25);
                NForm22MIS.Show();

            }

            if (WAction == 6)
            {
                NForm22MIS = new Form22MIS(WSignedId, WSignRecordNo, WOperator, WUserId, WDtTm, "", WAtmNo, 26);
                NForm22MIS.Show();

            }
        }
        // Enchanced chart2 
        private void chart2_Click(object sender, EventArgs e)
        {
        
            string Heading; 

            if (WAction == 1) // By Period Replenishment  
            {
                label2.Text = "Period vs Indicators for Replenish";
                Heading = label2.Text;
             //   WF = 1; // Means show large for Form21MIS 
                NForm35 = new Form35(WSignedId, WSignRecordNo, WOperator, MISRepl1, Heading, WAction);
                NForm35.Show();
            }

            if (WAction == 2) // By Period for Users  
            {
                label2.Text = "Users vs Errors and ones not Reconciled";
                Heading = label2.Text;
            //    WF = 2; // Means show large for Form21MIS 
                NForm35 = new Form35(WSignedId, WSignRecordNo, WOperator, MISRepl1, Heading, WAction);
                NForm35.Show();
            }

            if (WAction == 3) // By Period ATMs Turnover Indicators 
            {
                label2.Text = "Period vs Indicators for Dispensed";
                Heading = label2.Text; 
            //    WF = 3; // Means show large for Form21MIS 
                NForm35 = new Form35(WSignedId, WSignRecordNo, WOperator, MISRepl1, Heading, WAction);
                NForm35.Show();
            }

            if (WAction == 4) // By CIT providers vs Key indicators 
            {
                label2.Text = "CITs Vs Key Indicators";
                Heading = label2.Text;
             //   WF = 4; // Means show large for Form21MIS 
                NForm35 = new Form35(WSignedId, WSignRecordNo, WOperator, MISRepl1, Heading, WAction);
                NForm35.Show();
            }
         
            if (WAction == 5) // Disputes by period  
            {
                label2.Text = "Period vs No of Disputes";
                Heading = label2.Text;
           //     WF = 5; // Means show large for Form21MIS 
                NForm35 = new Form35(WSignedId, WSignRecordNo, WOperator, MISRepl1, Heading, WAction);
                NForm35.Show();
            }

            if (WAction == 6) // Profitability   
            {
                label2.Text = "Profitability Per Atm";
                Heading = label2.Text;
              //  WF = 6; // Means show large for Form21MIS 
                NForm35 = new Form35(WSignedId, WSignRecordNo, WOperator, MISRepl1, Heading, WAction);
                NForm35.Show();
            }

        }

        // Enhancement for second graph 
        private void chart1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Zoom is not availble for this chart"); 
        }


        // Double Click on Row
        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            if (WAction == 1)
            {

                NForm22MIS = new Form22MIS(WSignedId, WSignRecordNo, WOperator, WUserId, WDtTm, "","",21);
                NForm22MIS.Show();
            }
            if (WAction == 2)
            {

                NForm22MIS = new Form22MIS(WSignedId, WSignRecordNo, WOperator, WUserId, NullFutureDate, "","", 22);
                NForm22MIS.Show();

            }

            if (WAction == 3)
            {

                NForm22MIS = new Form22MIS(WSignedId, WSignRecordNo, WOperator, WUserId, WDtTm, "","", 23);
                NForm22MIS.Show();

            }

            if (WAction == 4)
            {

                NForm22MIS = new Form22MIS(WSignedId, WSignRecordNo, WOperator, WUserId, WDtTm, WCitId,"", 24);
                NForm22MIS.Show();

            }
            if (WAction == 5)
            {

                NForm22MIS = new Form22MIS(WSignedId, WSignRecordNo, WOperator, WUserId, WDtTm, WCitId,"", 25);
                NForm22MIS.Show();

            }
            if (WAction == 6)
            {

                NForm22MIS = new Form22MIS(WSignedId, WSignRecordNo, WOperator, WUserId, WDtTm, WCitId, WAtmNo, 26);
                NForm22MIS.Show();

            }

        }
    }
}

