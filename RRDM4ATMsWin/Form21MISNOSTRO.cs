using System;
using System.Data;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Data.SqlClient;
using System.Configuration;
//multilingual
using System.Resources;
using System.Drawing;
using System.Globalization;

namespace RRDM4ATMsWin
{
    public partial class Form21MISNOSTRO : Form
    {
        Form35NOSTRO NForm35NOSTRO; // Enlarge first chart 
        Form36NOSTRO NForm36NOSTRO; // Enlarge second chart 

        Form22MISNOSTRO NForm22MISNOSTRO;

        RRDMUsersRecords Us = new RRDMUsersRecords();  

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        DataTable MISNostro1 = new DataTable();

        DateTime WDtTm;

        DateTime NullFutureDate = new DateTime(2050, 11, 21);

        //TEST RANGE 
        DateTime WorkingDtA = new DateTime(2016, 11, 06); 
        DateTime WorkingDtB = new DateTime(2016, 11, 13);

        //TEST RANGE 
        DateTime WorkingDtC = new DateTime(2014, 05, 09);
        DateTime WorkingDtD = new DateTime(2014, 06, 18); 

        //multilingual
        CultureInfo culture;

        RRDMUsersRecords Xa = new RRDMUsersRecords(); // Make class availble 

        ResourceManager LocRM = new ResourceManager("RRDM4ATMsWin.appRes", typeof(Form40).Assembly);

        RRDMNVReconcCategoriesSessions Rcs = new RRDMNVReconcCategoriesSessions();

        string WUserId;
        //string WAtmNo;

        DateTime WDateOfCycle; 

        int WReconcCycleNo;

        string WJobCategory;
        string WOrigin;

        string WCurrNm;

        string WSignedId;
        int WSignRecordNo;
        string WSecLevel;
        string WOperator;
        string WSubSystem;

        int WAction;

        public Form21MISNOSTRO(string InSignedId, int SignRecordNo, string InSecLevel, string InOperator, string InSubSystem ,int InAction)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;
            WSubSystem = InSubSystem;

            WAction = InAction;  // 1 = By Period Key Indicators of Nostro Reconciliation, 
                                 // 2 = By User performance , 
                                 // 3 = By Banks grouping 
                                 // 4 = Aging Analysis for outstanding txns of Internal Stmt


            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;
            labelUserId.Text = InSignedId;
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);
            if (WSubSystem == "CardsSettlement")
            {
                WJobCategory = "CardsSettlement";
                WOrigin = "Visa Settlement"; // Used For Categories 
            }
            if (WSubSystem == "NostroReconciliation")
            {
                WJobCategory = "NostroReconciliation";
                WOrigin = "Nostro - Vostro"; // Used For Categories 
            }
          

            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostro(WOperator, WJobCategory);
            if (Rjc.RecordFound == true)
            {

                //WReconcCycleNo = Rjc.JobCycle;

                WDateOfCycle = Rjc.StartDateTm ;
            }

            if (WAction == 4) chart3.Show();
                          else chart3.Hide();  
        }
       
        private void Form21MIS_Load(object sender, EventArgs e)
        {
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.Culture == "English")
            {
                culture = CultureInfo.CreateSpecificCulture("el-GR");
            }
            if (Usi.Culture == "Français")
            {
                culture = CultureInfo.CreateSpecificCulture("fr-FR");
            }

      //      if (WMode == 1) label14.Text = LocRM.GetString("Form67label14a", culture);
      //      if (WMode == 2) label14.Text = LocRM.GetString("Form67label14b", culture);

            textBoxMsgBoard.Text = " Use expand Button to see analysis of DataGrid first column";

            RRDMBanks Ba = new RRDMBanks();
            Ba.ReadBank(WOperator);
            WCurrNm = Ba.BasicCurName; 
           
            MISNostro1 = new DataTable();
            MISNostro1.Clear();

            if (WAction == 1) // By Period Quality Indicators 
            {

                labelStep1.Text = "Key Indicators of Nostro Reconciliation"; 
                label2.Text = "PERIOD vs SET OF INDICATORS";
                label3.Text = "PERIOD vs Percent Efficiency";

               string SqlString2 =
              " SELECT "
              + " CAST(StartDailyProcess AS Date) AS Date, "
              + " Count(CategoryId) As No_Categories,"
              //+ " Count(CASE WHEN [StatementLoaded]=1 THEN 1 END) As LoadedStmts, "
              //+ " Count(CASE WHEN [StatementLoaded]=0 THEN 1 END) As NotLoadedStmts, "
              + " SUM(TotalNumberProcessed) As TotalNumberProcessed, "
              + " SUM(MatchedDefault) As MatchedDefault,"
              + " SUM(AutoButToBeConfirmed) As AutoButToBeConfirmed, "
              + " PercentEfficiency = (SUM(MatchedDefault) + SUM(AutoButToBeConfirmed))*100/SUM(TotalNumberProcessed) , "
              + " SUM(OutstandingAlerts) As OutstandingAlerts,"
              + " SUM(OutstandingDisputes) As OutstandingDisputes,"
              + " SUM(ManualToBeConfirmed) As ManualToBeConfirmed,"
              + " SUM(MatchedFromAutoToBeConfirmed) As MatchedFromAutoToBeConfirmed ,"
              + " SUM(MatchedFromManualToBeConfirmed) As MatchedFromManualToBeConfirmed,"
              + " UnMatchedNo = SUM(TotalNumberProcessed - "
              + " (MatchedDefault + MatchedFromAutoToBeConfirmed + MatchedFromManualToBeConfirmed)) , "
              + " count(CASE WHEN(TotalNumberProcessed - (MatchedDefault + MatchedFromAutoToBeConfirmed + MatchedFromManualToBeConfirmed)) > 0 THEN 1 END) As CatUnMatched, "
              + " SUM(UnMatchedAmt) As UnMatchedAmt"
              + " FROM [ATMS].[dbo].[NVReconcCategoriesSessions]"
              + " WHERE Operator = @Operator AND Origin = @Origin "
              + " AND StartDailyProcess > @WorkingDtA AND StartDailyProcess < @WorkingDtB "
              + " GROUP BY CAST(StartDailyProcess AS Date) "
              + " ORDER BY CAST(StartDailyProcess AS Date) DESC "
              ;

                using (SqlConnection conn =
                            new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();

                        //Create an Sql Adapter that holds the connection and the command
                        SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString2, conn);

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Origin", WOrigin);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", WOperator);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@WorkingDtA", WorkingDtA);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@WorkingDtB", WorkingDtB);

                        //Create a datatable that will be filled with the data retrieved from the command
                        //    DataSet MISds = new DataSet();
                        sqlAdapt.Fill(MISNostro1);

                        //Fill the dataGrid that will be displayed with the dataset
                        dataGridView1.DataSource = MISNostro1.DefaultView;

                        // Close conn
                        conn.Close();

                    }

                    catch (Exception ex)
                    {

                        string exception = ex.ToString();
                        MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

                    }

                if (dataGridView1.Rows.Count == 0)
                {
                    //MessageBox.Show("No transactions to be posted");
                    Form2 MessageForm = new Form2("No Records");
                    MessageForm.ShowDialog();
                    return;
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

                // Set chart1 data source  
                chart1.DataSource = MISNostro1.DefaultView;
                chart1.Series[0].Name = "UnMatchedNo";
                chart1.Series.Add("OutstandingAlerts");
                chart1.Series.Add("OutstandingDisputes");
                // Set series members names for the X and Y values  
                chart1.Series[0].XValueMember = "Date";
                chart1.Series[0].YValueMembers = "UnMatchedNo";
                chart1.Series[1].XValueMember = "Date";
                chart1.Series[1].YValueMembers = "OutstandingAlerts";
                chart1.Series[2].XValueMember = "Date";
                chart1.Series[2].YValueMembers = "OutstandingDisputes";

                // Data bind to the selected data source  
                chart1.DataBind();

                // Set chart data source  
                chart2.DataSource = MISNostro1.DefaultView;
                chart2.Series[0].Name = "PercentEfficiency";
                // Set series members names for the X and Y values  
                chart2.Series[0].XValueMember = "Date";
                chart2.Series[0].YValueMembers = "PercentEfficiency";

                // Data bind to the selected data source  
                chart2.DataBind();
               
            }

            // PERFORMANCE BY USER 

            if (WAction == 2) // Performance per USER 
            {
               
                labelStep1.Text = "User Performance Analysis"; 
                label14.Text = "BY USER LISTING OF KEY PERFORMANCE";
           
                string SqlString2 =
            " SELECT "
            + " OwnerId AS UserId, "
            + " Count(CategoryId) As No_Categories,"
            //+ " Count(CASE WHEN [StatementLoaded]=1 THEN 1 END) As LoadedStmts, "
            //+ " Count(CASE WHEN [StatementLoaded]=0 THEN 1 END) As NotLoadedStmts, "
            + " SUM(TotalNumberProcessed) As TotalNumberProcessed, "
            + " SUM(MatchedDefault) As MatchedDefault,"
            + " SUM(AutoButToBeConfirmed) As AutoButToBeConfirmed, "
            + " PercentEfficiency = (SUM(MatchedDefault) + SUM(AutoButToBeConfirmed))*100/SUM(TotalNumberProcessed) , "
            + " SUM(OutstandingAlerts) As OutstandingAlerts,"
            + " SUM(OutstandingDisputes) As OutstandingDisputes,"
            + " SUM(ManualToBeConfirmed) As ManualToBeConfirmed,"
            + " SUM(MatchedFromAutoToBeConfirmed) As MatchedFromAutoToBeConfirmed ,"
            + " SUM(MatchedFromManualToBeConfirmed) As MatchedFromManualToBeConfirmed,"
            + " UnMatchedNo = SUM(TotalNumberProcessed - "
            + " (MatchedDefault + MatchedFromAutoToBeConfirmed + MatchedFromManualToBeConfirmed)) , "
            + " count(CASE WHEN(TotalNumberProcessed - (MatchedDefault + MatchedFromAutoToBeConfirmed + MatchedFromManualToBeConfirmed)) > 0 THEN 1 END) As CatUnMatched, "
            + " SUM(UnMatchedAmt) As UnMatchedAmt"
            + " FROM [ATMS].[dbo].[NVReconcCategoriesSessions]"
            + " WHERE Operator = @Operator AND Origin = @Origin "
            + " AND StartDailyProcess > @WorkingDtA AND StartDailyProcess < @WorkingDtB "
            + " GROUP BY OwnerId "
            + " ORDER BY OwnerId ASC "
            ;

                using (SqlConnection conn =
                            new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();

                        //Create an Sql Adapter that holds the connection and the command
                        SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString2, conn);

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Origin", WOrigin);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", WOperator);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@WorkingDtA", WorkingDtA);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@WorkingDtB", WorkingDtB);

                        //Create a datatable that will be filled with the data retrieved from the command
                 
                        sqlAdapt.Fill(MISNostro1);

                        //Fill the dataGrid that will be displayed with the dataset
                        dataGridView1.DataSource = MISNostro1.DefaultView;

                        // Close conn
                        conn.Close();

                    }

                    catch (Exception ex)
                    {
                        string exception = ex.ToString();
                        MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));
                    }


                //  MessageBox.Show(" Number of ATMS = " + I);

                if (dataGridView1.Rows.Count == 0)
                {
                    //MessageBox.Show("No transactions to be posted");
                    Form2 MessageForm = new Form2("No Records");
                    MessageForm.ShowDialog();
                    return;
                }

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


                dataGridView1.Show();


                label2.Text = "USERS VS Key Indicators";

                // Set chart1 data source  
                chart1.DataSource = MISNostro1.DefaultView;
                chart1.Series[0].Name = "UnMatchedNo";
                chart1.Series.Add("OutstandingDisputes");
                // Set series members names for the X and Y values  
                chart1.Series[0].XValueMember = "UserId";
                chart1.Series[0].YValueMembers = "UnMatchedNo";
                chart1.Series[1].XValueMember = "UserId";
                chart1.Series[1].YValueMembers = "OutstandingDisputes";

                // Data bind to the selected data source  
                chart1.DataBind();

               
                // Set chart data source  
                label3.Text = "USERS VS Total Processed.";

                chart2.ChartAreas[0].AxisX.ScaleView.Zoomable = true;

                chart2.DataSource = MISNostro1.DefaultView;
                chart2.Series[0].Name = "TotalNumberProcessed";
                // Set series members names for the X and Y values  
                chart2.Series[0].XValueMember = "UserId";
                chart2.Series[0].YValueMembers = "TotalNumberProcessed";

                // Data bind to the selected data source  
                chart2.DataBind();
            
            }

            if (WAction == 3) // By Period NOSTRO BANKS  Indicators 
            {
                labelStep1.Text = "Management information for Banks"; 
                label2.Text = "Banks vs INDICATORS FOR DISPENSED";
                label3.Text = "Banks vs EFFECTIVENESS";
                //TESt

                string SqlString2 =
                   " SELECT "
                   + " ExternalBank As Bank, "
                   + " Count(CategoryId) As No_Categories,"
                   //+ " Count(CASE WHEN [StatementLoaded]=1 THEN 1 END) As LoadedStmts, "
                   //+ " Count(CASE WHEN [StatementLoaded]=0 THEN 1 END) As NotLoadedStmts, "
                   + " SUM(TotalNumberProcessed) As TotalNumberProcessed, "
                   + " SUM(MatchedDefault) As MatchedDefault,"
                   + " SUM(AutoButToBeConfirmed) As AutoButToBeConfirmed, "
                   + " PercentEfficiency = (SUM(MatchedDefault) + SUM(AutoButToBeConfirmed))*100/SUM(TotalNumberProcessed) , "
                   + " SUM(OutstandingAlerts) As OutstandingAlerts,"
                   + " SUM(OutstandingDisputes) As OutstandingDisputes,"
                   + " SUM(ManualToBeConfirmed) As ManualToBeConfirmed,"
                   + " SUM(MatchedFromAutoToBeConfirmed) As MatchedFromAutoToBeConfirmed ,"
                   + " SUM(MatchedFromManualToBeConfirmed) As MatchedFromManualToBeConfirmed,"
                   + " UnMatchedNo = SUM(TotalNumberProcessed - "
                   + " (MatchedDefault + MatchedFromAutoToBeConfirmed + MatchedFromManualToBeConfirmed)) , "
                   + " count(CASE WHEN(TotalNumberProcessed - (MatchedDefault + MatchedFromAutoToBeConfirmed + MatchedFromManualToBeConfirmed)) > 0 THEN 1 END) As CatUnMatched, "
                   + " SUM(UnMatchedAmt) As UnMatchedAmt"
                   + " FROM [ATMS].[dbo].[NVReconcCategoriesSessions]"
                   + " WHERE Origin = @Origin AND StmtLines > 0"
                   + " GROUP BY ExternalBank "
                   + " ORDER BY ExternalBank ASC "
                   ;

                using (SqlConnection conn =
                            new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();

                        //Create an Sql Adapter that holds the connection and the command
                        SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString2, conn);

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Origin", WOrigin);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", WOperator);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@WorkingDtA", WorkingDtA);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@WorkingDtB", WorkingDtB);

                        //Create a datatable that will be filled with the data retrieved from the command
                        //    DataSet MISds = new DataSet();
                        sqlAdapt.Fill(MISNostro1);

                        //Fill the dataGrid that will be displayed with the dataset
                        dataGridView1.DataSource = MISNostro1.DefaultView;

                        // Close conn
                        conn.Close();

                    }

                    catch (Exception ex)
                    {
                        string exception = ex.ToString();
                        MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

                    }
                if (dataGridView1.Rows.Count == 0)
                {
                    //MessageBox.Show("No transactions to be posted");
                    Form2 MessageForm = new Form2("No Records");
                    MessageForm.ShowDialog();
                    return;
                }
                dataGridView1.Columns[0].Width = 85; // 
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

                // Set chart1 data source  
                chart1.DataSource = MISNostro1.DefaultView;
                chart1.Series[0].Name = "TotalNumberProcessed";
                chart1.Series.Add("MatchedDefault");
                
                // Set series members names for the X and Y values  
                chart1.Series[0].XValueMember = "Bank";
                chart1.Series[0].YValueMembers = "TotalNumberProcessed";
                chart1.Series[1].XValueMember = "Bank";
                chart1.Series[1].YValueMembers = "MatchedDefault";

                // Data bind to the selected data source  
                chart1.DataBind();

                // Set chart data source  
                chart2.DataSource = MISNostro1.DefaultView;
                chart2.Series[0].Name = "PercentEfficiency";
                //chart1.Series.Add("Avg_CrAmount");
                // Set series members names for the X and Y values  
                chart2.Series[0].XValueMember = "Bank";
                chart2.Series[0].YValueMembers = "PercentEfficiency";
                //chart1.Series[1].XValueMember = "Date";
                //chart1.Series[1].YValueMembers = "Avg_CrAmount";

                // Data bind to the selected data source  
                chart2.DataBind();

            }

            if (WAction == 4) // Ageing Analysis
            {
             

                labelStep1.Text = "Outstanding txns Ageing Analysis Per Pair";
                label14.Text = "LISTING PER PAIR - RANGES ARE IN DATES AND AMOUNTS IN LOCAL CURRENCY";
              
              
                    try
                    {
                    DateTime NewTesting = WDateOfCycle.AddDays(3); 

                    Rcs.ReadAgeingTable(WOperator, WSignedId, WReconcCycleNo, NewTesting);
                    //Fill the dataGrid that will be displayed with the dataset
                    dataGridView1.DataSource = Rcs.TableAgeingAnalysis.DefaultView;
                }

                    catch (Exception ex)
                    {

                        string exception = ex.ToString();
                        MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

                    }
             

                DataGridViewCellStyle style = new DataGridViewCellStyle();
                style.Format = "N2";

                dataGridView1.Columns[0].Width = 60; // PairID
                dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView1.Columns[1].Width = 90; // Name
                dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[1].Visible = true;

                dataGridView1.Columns[2].Width = 50; // Ccy
                dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[2].Visible = true;
                
                dataGridView1.Columns[3].Width = 100; // GrandTotal
                dataGridView1.Columns[3].DefaultCellStyle = style;
                dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns[3].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);
                dataGridView1.Columns[3].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                
                dataGridView1.Columns[4].Width = 100; // Zero_To_4
                dataGridView1.Columns[4].DefaultCellStyle = style;
                dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns[4].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);
                dataGridView1.Columns[4].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;

                dataGridView1.Columns[5].Width = 100; // Four_To_7
                dataGridView1.Columns[5].DefaultCellStyle = style;
                dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns[5].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);
                dataGridView1.Columns[5].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;

                dataGridView1.Columns[6].Width = 100; // Seven_To_15
                dataGridView1.Columns[6].DefaultCellStyle = style;
                dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns[6].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);
                dataGridView1.Columns[6].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;

                dataGridView1.Columns[7].Width = 100; // Fifteen_To_30
                dataGridView1.Columns[7].DefaultCellStyle = style;
                dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns[7].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);
                dataGridView1.Columns[7].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;

                dataGridView1.Columns[8].Width = 100; // Thirty_To_60
                dataGridView1.Columns[8].DefaultCellStyle = style;
                dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns[8].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);
                dataGridView1.Columns[8].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;

                dataGridView1.Columns[9].Width = 100; //  More_Than_60
                dataGridView1.Columns[9].DefaultCellStyle = style;
                dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns[9].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);
                dataGridView1.Columns[9].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;

                //     dataGridView1.Columns[0].HeaderText = LocRM.GetString("Form67Grd1Cl0", culture);
                //     dataGridView1.Columns[1].HeaderText = LocRM.GetString("Form67Grd1Cl01", culture);
                //     dataGridView1.Columns[2].HeaderText = LocRM.GetString("Form67Grd1Cl02", culture);


                dataGridView1.Show();

                label2.Text = "PAIR vs TOTAL ";
                

                // Set chart1 data source  
                chart1.DataSource = Rcs.TableAgeingAnalysis.DefaultView;
                chart1.Series[0].Name = "GrandTotal";
              
                // Set series members names for the X and Y values  
                chart1.Series[0].XValueMember = "PairID";
                chart1.Series[0].YValueMembers = "GrandTotal";

                // Data bind to the selected data source  
                chart1.DataBind();

                linkLabel1.Hide();
             
                chart2.Hide();

                linkLabel2.Show();
                linkLabel3.Show();
                linkLabel4.Show();
                linkLabel5.Show();
                linkLabel6.Show();
                linkLabel7.Show();
            }
          
        }


        // A ROW IS SELECTED ASSIGN VALUES 
        string WBankId;
        decimal[] yvalues3;

        string CategoryId ;
        decimal Zero_To_4 ;
        decimal Four_To_7 ;
        decimal Seven_To_15 ;
        decimal Fifteen_To_30 ;
        decimal Thirty_To_60 ;
        decimal More_Than_60 ;

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
                WBankId = (string)rowSelected.Cells[0].Value;
            }

            if (WAction == 4)
            {
                CategoryId = (string)rowSelected.Cells[0].Value;
                Zero_To_4 = (decimal)rowSelected.Cells[4].Value;
                Four_To_7 = (decimal)rowSelected.Cells[5].Value;
                Seven_To_15 = (decimal)rowSelected.Cells[6].Value;
                Fifteen_To_30 = (decimal)rowSelected.Cells[7].Value;
                Thirty_To_60 = (decimal)rowSelected.Cells[8].Value;
                More_Than_60 = (decimal)rowSelected.Cells[9].Value;

                //TEST 
                if (CategoryId== "EWB812")
                {
                    Zero_To_4 = 500;
                    Four_To_7 = 200;
                    Seven_To_15 = 100;
                    Fifteen_To_30 = 50;
                    Thirty_To_60 = 0;
                    More_Than_60 = 50;
                }

                chart3.Show();
                label3.Show(); 
                label3.Text = "PAIR ANALYSIS";

                decimal[] yvalues2 = { Zero_To_4, Four_To_7, Seven_To_15,
                                      Fifteen_To_30, Thirty_To_60, More_Than_60 };
                string[] xvalues2 =
                    { "0 - 4", "4 - 7", "7 - 15", "15 - 30","30 - 60"," > 60" };

                // Set series members names for the X and Y values 
                chart3.Series[0].Points.DataBindXY(xvalues2, yvalues2);

                yvalues3 = yvalues2 ; 
            }

        }

        // Chart 1
        private void chart1_Click_1(object sender, EventArgs e)
        {
            string Heading;

            if (WAction == 1) // By Period Replenishment  
            {
                Heading = label2.Text;
                //   WF = 1; // Means show large for Form21MIS 
                NForm35NOSTRO = new Form35NOSTRO(WSignedId, WSignRecordNo, WOperator, MISNostro1, Heading, WAction);
                NForm35NOSTRO.Show();
            }

            if (WAction == 2) // By Period for Users  
            {

                Heading = label2.Text;
                //    WF = 2; // Means show large for Form21MIS 
                NForm35NOSTRO = new Form35NOSTRO(WSignedId, WSignRecordNo, WOperator, MISNostro1, Heading, WAction);
                NForm35NOSTRO.Show();
            }

            if (WAction == 3) // By Period ATMs Turnover Indicators 
            {

                Heading = label2.Text;
                //    WF = 3; // Means show large for Form21MIS 
                NForm35NOSTRO = new Form35NOSTRO(WSignedId, WSignRecordNo, WOperator, MISNostro1, Heading, WAction);
                NForm35NOSTRO.Show();
            }

            if (WAction == 4) // By CIT providers vs Key indicators 
            {

                Heading = label2.Text;
                //   WF = 4; // Means show large for Form21MIS 
                NForm35NOSTRO = new Form35NOSTRO(WSignedId, WSignRecordNo, WOperator, Rcs.TableAgeingAnalysis, Heading, WAction);
                NForm35NOSTRO.Show();
            }

        }
        //  // Enhancement for second graph 
        private void chart2_Click_1(object sender, EventArgs e)
        {

            string Heading;

            if (WAction == 1) // By Period Replenishment  
            {
            
                Heading = label3.Text;
                //   WF = 1; // Means show large for Form21MIS 
                NForm36NOSTRO = new Form36NOSTRO(WSignedId, WSignRecordNo, WOperator, MISNostro1, yvalues3, Heading, WAction);
                NForm36NOSTRO.Show();
            }

            if (WAction == 2) // By Period for Users  
            {

                Heading = label3.Text;
                //    WF = 2; // Means show large for Form21MIS 
                NForm36NOSTRO = new Form36NOSTRO(WSignedId, WSignRecordNo, WOperator, MISNostro1, yvalues3, Heading, WAction);
                NForm36NOSTRO.Show();
            }

            if (WAction == 3) // By Period ATMs Turnover Indicators 
            {

                Heading = label3.Text;
                //    WF = 3; // Means show large for Form21MIS 
                NForm36NOSTRO = new Form36NOSTRO(WSignedId, WSignRecordNo, WOperator, MISNostro1, yvalues3, Heading, WAction);
                NForm36NOSTRO.Show();
            }

          
        }

        //Chart 3 
        private void chart3_Click(object sender, EventArgs e)
        {
            if (WAction == 4) // 
            {
                string Heading = label3.Text;
                //   WF = 4; // Means show large for Form21MIS 
                NForm36NOSTRO = new Form36NOSTRO(WSignedId, WSignRecordNo, WOperator, Rcs.TableAgeingAnalysis, yvalues3, Heading, WAction);
                NForm36NOSTRO.Show();
            }
        }

        // Double Click on Row
        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            if (WAction == 1)
            {

                NForm22MISNOSTRO = new Form22MISNOSTRO(WSignedId, WSignRecordNo, WOperator, WUserId, WDtTm, "","",21);
                NForm22MISNOSTRO.Show();
            }
            if (WAction == 2)
            {

                NForm22MISNOSTRO = new Form22MISNOSTRO(WSignedId, WSignRecordNo, WOperator, WUserId, NullFutureDate, "","", 22);
                NForm22MISNOSTRO.Show();

            }

            if (WAction == 3)
            {

                NForm22MISNOSTRO = new Form22MISNOSTRO(WSignedId, WSignRecordNo, WOperator, WUserId, WDtTm, "","", 23);
                NForm22MISNOSTRO.Show();

            }

            if (WAction == 4)
            {
                MessageBox.Show("Nothing to Show"); 
            }    

        }

// Link to line info 
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (WAction == 1)
            {

                NForm22MISNOSTRO = new Form22MISNOSTRO(WSignedId, WSignRecordNo, WOperator, WUserId, WDtTm, "", "", 21);
                NForm22MISNOSTRO.Show();
            }
            if (WAction == 2)
            {

                NForm22MISNOSTRO = new Form22MISNOSTRO(WSignedId, WSignRecordNo, WOperator, WUserId, NullFutureDate, "", "", 22);
                NForm22MISNOSTRO.Show();

            }

            if (WAction == 3)
            {

                NForm22MISNOSTRO = new Form22MISNOSTRO(WSignedId, WSignRecordNo, WOperator, WUserId, WDtTm, "", "", 23);
                NForm22MISNOSTRO.Show();

            }

            if (WAction == 4)
            {

            //

            }

          
        }

        // Finish 
        private void buttonNext_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
// Cell Click 
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            MessageBox.Show("A Cell was chosen"); 
        }
// Cell double Click 
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            MessageBox.Show("A Cell was chosen with double click ");
        }
        //
        // LINK 0 to 4 
        //
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Check if value before proceeding 
        
            if (Zero_To_4 == 0)
            {
                MessageBox.Show("No Transactions to show.");
                return; 
            }
            DateTime NewTesting = WDateOfCycle.AddDays(3);
            Form80bNV NForm80bNV;
            
            DateTime FromDt = NewTesting.Date.AddDays(-4);
            DateTime ToDt = NewTesting;
            NForm80bNV = new Form80bNV(WSignedId, WSignRecordNo, WOperator, WSubSystem ,CategoryId , 0, 0, "ViewAgeingRange", FromDt, ToDt);
            NForm80bNV.Show(); 
        
        }
//
// Link to > 60 
//
        private void linkLabel7_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Check if value before proceeding 

            if (More_Than_60 == 0)
            {
                MessageBox.Show("No Transactions to show.");
                return;
            }
            DateTime NewTesting = WDateOfCycle.Date.AddDays(3);
            Form80bNV NForm80bNV;

            DateTime FromDt = NewTesting.Date.AddDays(-1000);
            DateTime ToDt = NewTesting.Date.AddDays(-60);
            NForm80bNV = new Form80bNV(WSignedId, WSignRecordNo, WOperator, WSubSystem, CategoryId, 0, 0, "ViewAgeingRange", FromDt, ToDt);
            NForm80bNV.Show();
        }
    }
}

