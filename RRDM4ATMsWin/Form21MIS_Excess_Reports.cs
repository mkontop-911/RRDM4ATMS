using System;
using System.Data;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Data.SqlClient;
using System.Configuration;
//multilingual
using System.Resources;
using System.Globalization;
using System.ComponentModel;
using System.Drawing;

using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form21MIS_Excess_Reports : Form
    {
        Form35 NForm35; // Enlarge chart 
        Form22MIS NForm22MIS;
        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        DataTable MISRepl1 = new DataTable();

        DataTable QuarterEXCESS_1 = new DataTable();
        DataTable QuarterEXCESS_2 = new DataTable();

        DataTable QuarterSHORTAGE_1 = new DataTable();
        DataTable QuarterSHORTAGE_2 = new DataTable();


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

        RRDMUsersRecords Xa = new RRDMUsersRecords(); // Make class availble 

        ResourceManager LocRM = new ResourceManager("RRDM4ATMsWin.appRes", typeof(Form40).Assembly);

        string WCitId;

        decimal Total_Excess = 0;
        decimal Total_Refund = 0;
        decimal Total_Shortage = 0;
        decimal Total_Recovered = 0;

        DateTime QuarterFrom;
        DateTime QuarterTo;

        //   int WF=0;

        string WUserId;
        string WAtmNo;

        string WCurrNm;

        //    string WOperator;

        string WSignedId;
        string WOperator;
        DateTime WDtFrom;
        DateTime WDtTo;
        int WAction;

        public Form21MIS_Excess_Reports(string InSignedId, string InOperator, DateTime InDtFrom, DateTime InDtTo, int InAction)
        {
            WSignedId = InSignedId;
            WOperator = InOperator;
            WDtFrom = InDtFrom;
            WDtTo = InDtTo;

            WAction = InAction;  // 1 = By Period Quality of Repl and Reconc, 
                                 // 2 = By User performance , 
                                 // 3 = By ATMs Daily Business 
                                 // 4 = Cit Analysis
                                 // 5 = Dispute Analysis 
                                 // 6 = By ATM Profitability 

            // 12 = Quarter Report for Excess
            // 14 = Quearter Report for Shortage 

            InitializeComponent();


            pictureBox1.BackgroundImage = appResImg.logo2;
            labelToday.Text = DateTime.Now.ToShortDateString();
        }

        private void Form21MIS_Load(object sender, EventArgs e)
        {
            //RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            //Usi.ReadSignedActivityByKey(WSignRecordNo);

            //if (Usi.Culture == "English")
            //{
            //    culture = CultureInfo.CreateSpecificCulture("el-GR");
            //}
            //if (Usi.Culture == "Français")
            //{
            //    culture = CultureInfo.CreateSpecificCulture("fr-FR");
            //}

            //      if (WMode == 1) label14.Text = LocRM.GetString("Form67label14a", culture);
            //      if (WMode == 2) label14.Text = LocRM.GetString("Form67label14b", culture);

            textBoxMsgBoard.Text = " Use expand Button to see analysis of DataGrid first column";

            RRDMBanks Ba = new RRDMBanks();
            Ba.ReadBank(WOperator);
            WCurrNm = Ba.BasicCurName;

            string DataBase ;

            if ( Environment.MachineName == "RRDM-PANICOS")
            {
                DataBase = "ATMS_SAMIH_For_Osman";
            }
            else
            {
                DataBase = "ATMS";
            }

            MISRepl1 = new DataTable();
            MISRepl1.Clear();

            if (WAction == 12) // Excess Or SHORTAGE BOTH OF THEM 
            {

                labelStep1.Text = "Management information for Excess and Shortage Per Quarter ";

                

                // START WITH EXCESS
                QuarterEXCESS_1 = new DataTable();
                QuarterEXCESS_1.Clear();

                QuarterEXCESS_2 = new DataTable();
                QuarterEXCESS_2.Clear();

                string SelectionCriteria_Excess = " WHERE(ShortAccID_1 = '40') and Stage = '03' ";


                string SqlString2 =
                   " SELECT DATEPART(yyyy, ActionAtDateTime) AS W_Year, DATEPART(qq, ActionAtDateTime) AS W_Quarter, "
     + " SUM(CR_AMOUNT) AS CR_AMOUNT , SUM(Dr_AMOUNT) AS DR_AMOUNT, SUM(CR_AMOUNT - Dr_AMOUNT) AS NET "
     + " FROM " + DataBase + ".[dbo].[Actions_Occurances] "
     + SelectionCriteria_Excess
     + " AND(CAST(ActionAtDateTime As DATE) >= @WDtFrom  AND CAST(ActionAtDateTime As DATE) <= @WDtTo ) "
     + " GROUP BY DATEPART(yyyy, ActionAtDateTime), DATEPART(qq, ActionAtDateTime) "
     + " ORDER BY DATEPART(yyyy, ActionAtDateTime), DATEPART(qq, ActionAtDateTime) ";

                using (SqlConnection conn =
                            new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();

                        //Create an Sql Adapter that holds the connection and the command
                        SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString2, conn);

                        //sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", WOperator);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@WDtFrom", WDtFrom);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@WDtTo", WDtTo);

                        //Create a datatable that will be filled with the data retrieved from the command
                        //    DataSet MISds = new DataSet();
                        sqlAdapt.Fill(QuarterEXCESS_1);

                        // Close conn
                        conn.Close();

                    }

                    catch (Exception ex)
                    {
                        conn.Close();
                        string exception = ex.ToString();
                        MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

                    }


                // Read QuarterEXCESS_Shortage and CREATE QuarterEXCESS_Shortage_2

                // DATA TABLE ROWS DEFINITION 

                QuarterEXCESS_2.Columns.Add("YYYY-Quarter", typeof(string));

                QuarterEXCESS_2.Columns.Add("From_Dt", typeof(DateTime)); // From Date
                QuarterEXCESS_2.Columns.Add("To_Dt", typeof(DateTime)); // To Date

                QuarterEXCESS_2.Columns.Add("TOTAL_EXCESS", typeof(decimal));
                QuarterEXCESS_2.Columns.Add("TOTAL_Refunded", typeof(decimal));
                QuarterEXCESS_2.Columns.Add("Net_Amount", typeof(decimal));


                try
                {
                    Total_Excess = 0;
                    Total_Refund = 0;


                    int I = 0;
                    int K = QuarterEXCESS_1.Rows.Count - 1;

                    while (I <= K)
                    {
                        int W_YYYY = (int)QuarterEXCESS_1.Rows[I]["W_Year"];
                        int WW_Quarter = (int)QuarterEXCESS_1.Rows[I]["W_Quarter"];
                        // Read the rest 

                        Total_Excess = (decimal)QuarterEXCESS_1.Rows[I]["CR_AMOUNT"];
                        Total_Refund = (decimal)QuarterEXCESS_1.Rows[I]["DR_AMOUNT"];

                        // FILL TABLE

                        //NEW ROW
                        DataRow RowSelected = QuarterEXCESS_2.NewRow();

                        RowSelected["YYYY-Quarter"] = W_YYYY.ToString() + "-" + WW_Quarter.ToString();
                        //  RowSelected["QUARTER"] = WW_Quarter ;
                        if (WW_Quarter == 1)
                        {
                            string QuarterFrom_S = W_YYYY + "-01-01";

                            QuarterFrom = DateTime.ParseExact(QuarterFrom_S, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                            string QuarterTo_S = W_YYYY + "-03-31";

                            QuarterTo = DateTime.ParseExact(QuarterTo_S, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                            RowSelected["From_Dt"] = QuarterFrom.Date;
                            RowSelected["To_Dt"] = QuarterTo.Date;
                        }

                        if (WW_Quarter == 2)
                        {
                            string QuarterFrom_S = W_YYYY + "-04-01";

                            QuarterFrom = DateTime.ParseExact(QuarterFrom_S, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                            string QuarterTo_S = W_YYYY + "-06-30";

                            QuarterTo = DateTime.ParseExact(QuarterTo_S, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                            RowSelected["From_Dt"] = QuarterFrom.Date;
                            RowSelected["To_Dt"] = QuarterTo.Date;
                        }
                        if (WW_Quarter == 3)
                        {
                            string QuarterFrom_S = W_YYYY + "-07-01";

                            QuarterFrom = DateTime.ParseExact(QuarterFrom_S, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                            string QuarterTo_S = W_YYYY + "-09-30";

                            QuarterTo = DateTime.ParseExact(QuarterTo_S, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                            RowSelected["From_Dt"] = QuarterFrom.Date;
                            RowSelected["To_Dt"] = QuarterTo.Date;
                        }
                        if (WW_Quarter == 4)
                        {
                            string QuarterFrom_S = W_YYYY + "-10-01";

                            QuarterFrom = DateTime.ParseExact(QuarterFrom_S, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                            string QuarterTo_S = W_YYYY + "-12-31";

                            QuarterTo = DateTime.ParseExact(QuarterTo_S, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                            RowSelected["From_Dt"] = QuarterFrom.Date;
                            RowSelected["To_Dt"] = QuarterTo.Date;
                        }

                        RowSelected["Total_EXCESS"] = Total_Excess;
                        RowSelected["Total_Refunded"] = Total_Refund;
                        RowSelected["Net_Amount"] = Total_Excess - Total_Refund;

                        // ADD ROW
                        QuarterEXCESS_2.Rows.Add(RowSelected);

                        I++; // Read Next entry of the table 

                    }

                }
                catch (Exception ex)
                {

                    string exception = ex.ToString();
                    MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));
                    return;

                }


                //Fill the dataGrid that will be displayed with the dataset
                dataGridView1.DataSource = QuarterEXCESS_2.DefaultView;

                DataGridViewCellStyle style = new DataGridViewCellStyle();
                style.Format = "N2";

                dataGridView1.Columns[0].Width = 80; // YYYY and Quarter 
                dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[1].Width = 100; // Date From 
                dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dataGridView1.Columns[2].Width = 100; //  Date TO
                dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                //dataGridView1.Columns[2].DefaultCellStyle.Format = "dd/MM/yyyy";

                dataGridView1.Columns[3].Width = 100; // Excess 
                dataGridView1.Columns[3].DefaultCellStyle.Format = "#,##0.00";
                dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dataGridView1.Columns[4].Width = 100; // Returned
                dataGridView1.Columns[4].DefaultCellStyle.Format = "#,##0.00";
                dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dataGridView1.Columns[5].Width = 100; //  Net 
                dataGridView1.Columns[5].DefaultCellStyle.Format = "#,##0.00";
                dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dataGridView1.Show();


                labelFirstGraph.Text = "EXCESS ANALYSIS vs QUARTERS";

                // Set chart1 data source  FIRST ON the top 
                chart1.DataSource = QuarterEXCESS_2.DefaultView;
                chart1.Series[0].Name = "TOTAL_EXCESS";
                chart1.Series.Add("TOTAL_Refunded");
                chart1.Series.Add("Net_Amount");
                // Set series members names for the X and Y values  
                chart1.Series[0].XValueMember = "YYYY-Quarter";
                chart1.Series[0].YValueMembers = "TOTAL_EXCESS";
                chart1.Series[1].XValueMember = "YYYY-Quarter";
                chart1.Series[1].YValueMembers = "TOTAL_Refunded";
                chart1.Series[2].XValueMember = "YYYY-Quarter";
                chart1.Series[2].YValueMembers = "Net_Amount";

                // Data bind to the selected data source  
                chart1.DataBind();

                // CONTINUE WITH SHORTAGE 

                QuarterSHORTAGE_1 = new DataTable();
                QuarterSHORTAGE_1.Clear();

                QuarterSHORTAGE_2 = new DataTable();
                QuarterSHORTAGE_2.Clear();

                //   + " WHERE (A.ShortAccID_1 = '50' OR A.ShortAccID_2 = '50') and A.Stage = '03' "
                string SelectionCriteria_Shortage = " WHERE (ShortAccID_1 = '50' OR ShortAccID_2 = '50') and Stage = '03' ";

                string SqlString3 =
                   " SELECT DATEPART(yyyy, ActionAtDateTime) AS W_Year, DATEPART(qq, ActionAtDateTime) AS W_Quarter, "
     + " SUM(CR_AMOUNT) AS CR_AMOUNT , SUM(Dr_AMOUNT) AS DR_AMOUNT, SUM(CR_AMOUNT - Dr_AMOUNT) AS NET "
     + " FROM " + DataBase + ".[dbo].[Actions_Occurances] "
     + SelectionCriteria_Shortage
     + " AND(CAST(ActionAtDateTime As DATE) >= @WDtFrom  AND CAST(ActionAtDateTime As DATE) <= @WDtTo ) "
     + " GROUP BY DATEPART(yyyy, ActionAtDateTime), DATEPART(qq, ActionAtDateTime) "
     + " ORDER BY DATEPART(yyyy, ActionAtDateTime), DATEPART(qq, ActionAtDateTime) ";

                using (SqlConnection conn =
                            new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();

                        //Create an Sql Adapter that holds the connection and the command
                        SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString3, conn);

                        //sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", WOperator);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@WDtFrom", WDtFrom);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@WDtTo", WDtTo);

                        //Create a datatable that will be filled with the data retrieved from the command
                        //    DataSet MISds = new DataSet();
                        sqlAdapt.Fill(QuarterSHORTAGE_1);

                        // Close conn
                        conn.Close();

                    }

                    catch (Exception ex)
                    {
                        conn.Close();
                        string exception = ex.ToString();
                        MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

                    }


                // Read QuarterEXCESS_Shortage and CREATE QuarterEXCESS_Shortage_2

                // DATA TABLE ROWS DEFINITION 

                QuarterSHORTAGE_2.Columns.Add("YYYY-Quarter", typeof(string));

                QuarterSHORTAGE_2.Columns.Add("From_Dt", typeof(DateTime)); // From Date
                QuarterSHORTAGE_2.Columns.Add("To_Dt", typeof(DateTime)); // To Date


                QuarterSHORTAGE_2.Columns.Add("TOTAL_SHORTAGE", typeof(decimal));
                QuarterSHORTAGE_2.Columns.Add("TOTAL_Recovered", typeof(decimal));
                QuarterSHORTAGE_2.Columns.Add("Net_Amount", typeof(decimal));

                try
                {

                    Total_Shortage = 0;
                    Total_Recovered = 0;


                    int I = 0;
                    int K = QuarterSHORTAGE_1.Rows.Count - 1;

                    while (I <= K)
                    {
                        int W_YYYY = (int)QuarterSHORTAGE_1.Rows[I]["W_Year"];
                        int WW_Quarter = (int)QuarterSHORTAGE_1.Rows[I]["W_Quarter"];

                        Total_Shortage = (decimal)QuarterSHORTAGE_1.Rows[I]["DR_AMOUNT"]; // DR FOR SHORTAGE
                        Total_Recovered = (decimal)QuarterSHORTAGE_1.Rows[I]["CR_AMOUNT"];


                        // FILL TABLE

                        //NEW ROW
                        DataRow RowSelected = QuarterSHORTAGE_2.NewRow();

                        RowSelected["YYYY-Quarter"] = W_YYYY.ToString() + "-" + WW_Quarter.ToString();
                        //  RowSelected["QUARTER"] = WW_Quarter ;
                        if (WW_Quarter == 1)
                        {
                            string QuarterFrom_S = W_YYYY + "-01-01";

                            QuarterFrom = DateTime.ParseExact(QuarterFrom_S, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                            string QuarterTo_S = W_YYYY + "-03-31";

                            QuarterTo = DateTime.ParseExact(QuarterTo_S, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                            RowSelected["From_Dt"] = QuarterFrom.Date;
                            RowSelected["To_Dt"] = QuarterTo.Date;
                        }

                        if (WW_Quarter == 2)
                        {
                            string QuarterFrom_S = W_YYYY + "-04-01";

                            QuarterFrom = DateTime.ParseExact(QuarterFrom_S, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                            string QuarterTo_S = W_YYYY + "-06-30";

                            QuarterTo = DateTime.ParseExact(QuarterTo_S, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                            RowSelected["From_Dt"] = QuarterFrom.Date;
                            RowSelected["To_Dt"] = QuarterTo.Date;
                        }
                        if (WW_Quarter == 3)
                        {
                            string QuarterFrom_S = W_YYYY + "-07-01";

                            QuarterFrom = DateTime.ParseExact(QuarterFrom_S, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                            string QuarterTo_S = W_YYYY + "-09-30";

                            QuarterTo = DateTime.ParseExact(QuarterTo_S, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                            RowSelected["From_Dt"] = QuarterFrom.Date;
                            RowSelected["To_Dt"] = QuarterTo.Date;
                        }
                        if (WW_Quarter == 4)
                        {
                            string QuarterFrom_S = W_YYYY + "-10-01";

                            QuarterFrom = DateTime.ParseExact(QuarterFrom_S, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                            string QuarterTo_S = W_YYYY + "-12-31";

                            QuarterTo = DateTime.ParseExact(QuarterTo_S, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                            RowSelected["From_Dt"] = QuarterFrom.Date;
                            RowSelected["To_Dt"] = QuarterTo.Date;
                        }

                        RowSelected["Total_SHORTAGE"] = Total_Shortage;
                        RowSelected["Total_Recovered"] = Total_Recovered;
                        RowSelected["Net_Amount"] = Total_Shortage - Total_Recovered;


                        // ADD ROW
                        QuarterSHORTAGE_2.Rows.Add(RowSelected);

                        I++; // Read Next entry of the table 

                    }

                }
                catch (Exception ex)
                {

                    string exception = ex.ToString();
                    MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));
                    return;

                }


                //Fill the dataGrid that will be displayed with the dataset
                dataGridView2.DataSource = QuarterSHORTAGE_2.DefaultView;

                //DataGridViewCellStyle style = new DataGridViewCellStyle();
                //style.Format = "N2";

                dataGridView2.Columns[0].Width = 80; // YYYY and Quarter 
                dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView2.Columns[1].Width = 100; // Date From 
                dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dataGridView2.Columns[2].Width = 100; //  Date TO
                dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;


                dataGridView2.Columns[3].Width = 100; // SHORTAGE
                dataGridView2.Columns[3].DefaultCellStyle.Format = "#,##0.00";
                dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dataGridView2.Columns[4].Width = 100; // 
                dataGridView2.Columns[4].DefaultCellStyle.Format = "#,##0.00";
                dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dataGridView2.Columns[5].Width = 100; //  Net 
                dataGridView2.Columns[5].DefaultCellStyle.Format = "#,##0.00";
                dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dataGridView2.Show();

                labelSecondGraph.Text = "SHORTAGE ANALYSIS VS QUARTERS";

                //QuarterSHORTAGE_2.Columns.Add("YYYY-Quarter", typeof(string));

                //QuarterSHORTAGE_2.Columns.Add("From_Dt", typeof(DateTime)); // From Date
                //QuarterSHORTAGE_2.Columns.Add("To_Dt", typeof(DateTime)); // To Date


                //QuarterSHORTAGE_2.Columns.Add("TOTAL_SHORTAGE", typeof(decimal));
                //QuarterSHORTAGE_2.Columns.Add("TOTAL_Recovered", typeof(decimal));
                //QuarterSHORTAGE_2.Columns.Add("Net_Amount", typeof(decimal));
                // Set chart2 data source  FIRST ON the top 
                chart2.DataSource = QuarterSHORTAGE_2.DefaultView;
                chart2.Series[0].Name = "TOTAL_SHORTAGE";
                chart2.Series.Add("TOTAL_Recovered");
                chart2.Series.Add("Net_Amount");
                // Set series members names for the X and Y values  
                chart2.Series[0].XValueMember = "YYYY-Quarter";
                chart2.Series[0].YValueMembers = "TOTAL_SHORTAGE";
                chart2.Series[1].XValueMember = "YYYY-Quarter";
                chart2.Series[1].YValueMembers = "TOTAL_Recovered";
                chart2.Series[2].XValueMember = "YYYY-Quarter";
                chart2.Series[2].YValueMembers = "Net_Amount";

                // Data bind to the selected data source  
                chart2.DataBind();


            }


            if (WAction == 1) // By Period Quality Indicators 
            {

                labelStep1.Text = "Management information for Replenish and Reconciliation";
                labelFirstGraph.Text = "PERIOD vs SET OF INDICATORS";
                labelSecondGraph.Text = "PERIOD vs CASH UTILIZATION";



                string SqlString2 =
                    "SELECT CAST(ReplDate AS Date) AS Date, Count(AtmNo) As No_Of_Atms, SUM(ReplMinutes) AS RepMin, Avg(ReplMinutes) AS Avg_Repl,"
            + " SUM(ErrorsAtm) As ErrATM,SUM(ErrorsHost) AS ErrHost, TotErr = SUM(ErrorsAtm + ErrorsHost), "
            + "  AbsDiff = SUM(DiffPlus + DiffMinus),"
            + " CashUtil=(Sum(RemainMoney)/Sum(InMoneyLast)), SUM(NotReconc) As NotReconc"
            + " FROM [ATMS].[dbo].[ReplStatsTable] "
            + " WHERE Operator = @Operator AND ReplDate > @WorkingDtA AND ReplDate < @WorkingDtB"
            + " GROUP BY CAST(ReplDate AS Date) "
            + " ORDER BY CAST(ReplDate AS Date) DESC ";

                //           string SqlString2 =
                //              " SELECT DATEPART(yyyy, ActionAtDateTime) AS W_Year, DATEPART(qq, ActionAtDateTime) AS W_Quarter, "
                //+ " SUM(CR_AMOUNT) AS EXCESS, SUM(Dr_AMOUNT) AS Refund, SUM(CR_AMOUNT - Dr_AMOUNT) AS NET "
                //+ " FROM  DataBase.[dbo].[Actions_Occurances] "
                //+ " WHERE(ShortAccID_1 = '40') and Stage = '03' "
                //+ " AND(CAST(ActionAtDateTime As DATE) >= '2023-11-01'  AND CAST(ActionAtDateTime As DATE) <= '2024-11-30') "
                //+ " GROUP BY DATEPART(yyyy, ActionAtDateTime), DATEPART(qq, ActionAtDateTime) "
                //+ " ORDER BY DATEPART(yyyy, ActionAtDateTime), DATEPART(qq, ActionAtDateTime) ";

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

                DataGridViewCellStyle style = new DataGridViewCellStyle();
                style.Format = "N2";

                dataGridView1.Columns[0].Width = 70; // Date
                dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView1.Columns[1].Width = 60; // No_Of_Atms
                dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[2].Width = 65; //  RepMin
                dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[3].Width = 65; // Avg_Repl
                dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[4].Width = 65; // ErrATM
                dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[5].Width = 65; // ErrHost
                dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[6].Width = 65; // TotErr
                dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[7].Width = 70; // AbsDiff
                dataGridView1.Columns[7].DefaultCellStyle = style;
                dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[8].Width = 70; // CashUtil
                dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns[8].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

                dataGridView1.Columns[9].Width = 50; // NotReconc
                dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;


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


                DataGridViewCellStyle style = new DataGridViewCellStyle();
                style.Format = "N2";

                dataGridView1.Columns[0].Width = 70; // UserId
                dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView1.Columns[1].Width = 60; //RepMin
                dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[2].Width = 65; //  RepMax
                dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[3].Width = 65; // Avg_Repl
                dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[4].Width = 65; // ErrATM
                dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[5].Width = 65; // ErrHost
                dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[6].Width = 65; // TotErr
                dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[7].Width = 70; // AbsDiff
                dataGridView1.Columns[7].DefaultCellStyle = style;
                dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dataGridView1.Columns[8].Width = 75; // TotInMoney
                dataGridView1.Columns[8].DefaultCellStyle = style;
                dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns[8].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

                dataGridView1.Columns[9].Width = 70; //TotUsed
                dataGridView1.Columns[9].DefaultCellStyle = style;
                dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dataGridView1.Columns[10].Width = 70; //CashUtil
                dataGridView1.Columns[10].DefaultCellStyle = style;
                dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dataGridView1.Columns[11].Width = 70; //NotReconc
                dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;


                //dataGridView1.Columns[0].Width = 60; // 
                //dataGridView1.Columns[1].Width = 60; // 
                //dataGridView1.Columns[2].Width = 60; //
                //dataGridView1.Columns[3].Width = 60; // 
                //dataGridView1.Columns[4].Width = 60; // 
                //dataGridView1.Columns[5].Width = 60; // 
                //dataGridView1.Columns[6].Width = 60; // 
                //dataGridView1.Columns[7].Width = 65; // 
                //dataGridView1.Columns[8].Width = 65; // 
                //dataGridView1.Columns[9].Width = 65; // 
                //dataGridView1.Columns[10].Width = 65; // 
                //dataGridView1.Columns[11].Width = 65; // 

                //     dataGridView1.Columns[0].HeaderText = LocRM.GetString("Form67Grd1Cl0", culture);
                //     dataGridView1.Columns[1].HeaderText = LocRM.GetString("Form67Grd1Cl01", culture);
                //     dataGridView1.Columns[2].HeaderText = LocRM.GetString("Form67Grd1Cl02", culture);


                dataGridView1.Show();


                labelFirstGraph.Text = "USERS VS ERRORS AND NOT RECONC";

                // Set chart2 data source  
                // This is the first one on the top
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
                labelSecondGraph.Text = "USERS VS AVER TIME OF REPL.";

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
                labelFirstGraph.Text = "PERIOD vs INDICATORS FOR DISPENSED";
                labelSecondGraph.Text = "PERIOD vs INDICATORS FOR DEPOSITS";
                //TEST
                // SELECT STATEMENT TO GET DATA BY MONTH PERIOD INSTEAD OF DATE

                string SqlStringDD =
                    "SELECT CAST(DtTm AS Date) AS Date, Count(AtmNo) As No_Of_Atms,"
                    + " SUM(DrTransactions) AS DR_TRANS,SUM(DispensedAmt) AS DR_Amount,"
                    + " Avg(DrTransactions) AS Avg_DrTrans, Avg(DispensedAmt) AS Avg_DrAmount,"
                    + " SUM(CrTransactions) AS CR_TRANS,SUM(DepAmount) AS CR_Amount,"
                     + " Avg(CrTransactions) AS Avg_CrTrans, Avg(DepAmount) AS Avg_CrAmount"
            + " FROM [ATMS].[dbo].[AtmDispAmtsByDay] "
            + " WHERE Operator = @Operator AND Year =@Year"
            + " GROUP BY CAST(DtTm AS Date) "
            + " ORDER BY CAST(DtTm AS Date) DESC ";

                using (SqlConnection conn =
                            new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();


                        //Create an Sql Adapter that holds the connection and the command
                        SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlStringDD, conn);

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", WOperator);
                        if (WOperator == "CRBAGRAA")
                        {
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@Year", 2014);
                        }
                        else
                        {
                            int WYear = DateTime.Now.Year;
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@Year", WYear);
                        }


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


                DataGridViewCellStyle style = new DataGridViewCellStyle();
                style.Format = "N2";

                //      "SELECT CAST(DtTm AS Date) AS Date, Count(AtmNo) As No_Of_Atms,"
                //        + " SUM(DrTransactions) AS DR_TRANS,SUM(DispensedAmt) AS DR_Amount,"
                //        + " Avg(DrTransactions) AS Avg_DrTrans, Avg(DispensedAmt) AS Avg_DrAmount,"
                //        + " SUM(CrTransactions) AS CR_TRANS,SUM(DepAmount) AS CR_Amount,"
                //         + " Avg(CrTransactions) AS Avg_CrTrans, Avg(DepAmount) AS Avg_CrAmount"
                //+ " FROM [ATMS].[dbo].[AtmDispAmtsByDay] "

                dataGridView1.Columns[0].Width = 70; // Date
                dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView1.Columns[1].Width = 60; //No_Of_Atms
                dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[2].Width = 65; //  DR_TRANS
                dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[3].Width = 80; // DR_Amount
                dataGridView1.Columns[3].DefaultCellStyle = style;
                dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dataGridView1.Columns[4].Width = 65; // Avg_DrTrans
                dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[5].Width = 80; // Avg_DrAmount
                dataGridView1.Columns[5].DefaultCellStyle = style;
                dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dataGridView1.Columns[6].Width = 65; // CR_TRANS
                dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[7].Width = 80; // CR_Amount
                dataGridView1.Columns[7].DefaultCellStyle = style;
                dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dataGridView1.Columns[8].Width = 65; // Avg_CrTrans
                dataGridView1.Columns[8].DefaultCellStyle = style;
                dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                //dataGridView1.Columns[8].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

                dataGridView1.Columns[9].Width = 80; // Avg_CrAmount
                dataGridView1.Columns[9].DefaultCellStyle = style;
                dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

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

                labelFirstGraph.Text = "CITs GROUP vs KEY INDICATORS";
                labelSecondGraph.Text = "CITs vs ERRORS";

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
                labelFirstGraph.Text = "PERIOD vs NO OF DISPUTES";
                labelSecondGraph.Text = "PERIOD vs DISPUTED AMOUNT";

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


                labelFirstGraph.Text = "ATMs VS COST AND REVENUE AND PROFIT";

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
                labelSecondGraph.Text = "ANALYSIS OF ATM PROFIT";

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
        DateTime WExcess_From_Dt;
        DateTime WExcess_To_Dt;
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            // Get DAtes 
            WExcess_From_Dt = (DateTime)rowSelected.Cells[1].Value;
            WExcess_To_Dt = (DateTime)rowSelected.Cells[2].Value;

        }
        // SECOND GRID
        DateTime WShortage_From_Dt;
        DateTime WShortage_To_Dt;
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected2 = dataGridView2.Rows[e.RowIndex];
            // Get DAtes 
            WShortage_From_Dt = (DateTime)rowSelected2.Cells[1].Value;
            WShortage_To_Dt = (DateTime)rowSelected2.Cells[2].Value;
        }
        // BASED ON Selected Row Expand 
        private void button2_Click(object sender, EventArgs e)
        {
            if (WAction == 1)
            {

                //  NForm22MIS = new Form22MIS(WSignedId, WSignRecordNo, WOperator, WUserId, WDtTm ,"","",21);
                NForm22MIS.Show();
            }
            if (WAction == 2)
            {

                //  NForm22MIS = new Form22MIS(WSignedId, WSignRecordNo, WOperator, WUserId, NullFutureDate,  "","", 22);
                NForm22MIS.Show();

            }

            if (WAction == 3)
            {

                //  NForm22MIS = new Form22MIS(WSignedId, WSignRecordNo, WOperator, WUserId, WDtTm,  "","", 23);
                NForm22MIS.Show();

            }

            if (WAction == 4)
            {

                // NForm22MIS = new Form22MIS(WSignedId, WSignRecordNo, WOperator, WUserId, WDtTm,  WCitId,"", 24);
                NForm22MIS.Show();

            }

            if (WAction == 5)
            {

                // NForm22MIS = new Form22MIS(WSignedId, WSignRecordNo, WOperator, WUserId, WDtTm, "" ,"", 25);
                NForm22MIS.Show();

            }

            if (WAction == 6)
            {
                // NForm22MIS = new Form22MIS(WSignedId, WSignRecordNo, WOperator, WUserId, WDtTm, "", WAtmNo, 26);
                NForm22MIS.Show();

            }
        }
        // Enchanced chart2 
        private void chart2_Click(object sender, EventArgs e)
        {

            string Heading;

            if (WAction == 1) // By Period Replenishment  
            {
                labelFirstGraph.Text = "Period vs Indicators for Replenish";
                Heading = labelFirstGraph.Text;
                //   WF = 1; // Means show large for Form21MIS 
                //   NForm35 = new Form35(WSignedId, WSignRecordNo, WOperator, MISRepl1, Heading, WAction);
                NForm35.Show();
            }

            if (WAction == 2) // By Period for Users  
            {
                labelFirstGraph.Text = "Users vs Errors and ones not Reconciled";
                Heading = labelFirstGraph.Text;
                //    WF = 2; // Means show large for Form21MIS 
                //    NForm35 = new Form35(WSignedId, WSignRecordNo, WOperator, MISRepl1, Heading, WAction);
                NForm35.Show();
            }

            if (WAction == 3) // By Period ATMs Turnover Indicators 
            {
                labelFirstGraph.Text = "Period vs Indicators for Dispensed";
                Heading = labelFirstGraph.Text;
                //    WF = 3; // Means show large for Form21MIS 
                //    NForm35 = new Form35(WSignedId, WSignRecordNo, WOperator, MISRepl1, Heading, WAction);
                NForm35.Show();
            }

            if (WAction == 4) // By CIT providers vs Key indicators 
            {
                labelFirstGraph.Text = "CITs Vs Key Indicators";
                Heading = labelFirstGraph.Text;
                //   WF = 4; // Means show large for Form21MIS 
                //    NForm35 = new Form35(WSignedId, WSignRecordNo, WOperator, MISRepl1, Heading, WAction);
                NForm35.Show();
            }

            if (WAction == 5) // Disputes by period  
            {
                labelFirstGraph.Text = "Period vs No of Disputes";
                Heading = labelFirstGraph.Text;
                //     WF = 5; // Means show large for Form21MIS 
                //  NForm35 = new Form35(WSignedId, WSignRecordNo, WOperator, MISRepl1, Heading, WAction);
                NForm35.Show();
            }

            if (WAction == 6) // Profitability   
            {
                labelFirstGraph.Text = "Profitability Per Atm";
                Heading = labelFirstGraph.Text;
                //  WF = 6; // Means show large for Form21MIS 
                //    NForm35 = new Form35(WSignedId, WSignRecordNo, WOperator, MISRepl1, Heading, WAction);
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

                //    NForm22MIS = new Form22MIS(WSignedId, WSignRecordNo, WOperator, WUserId, WDtTm, "","",21);
                NForm22MIS.Show();
            }
            if (WAction == 2)
            {

                //   NForm22MIS = new Form22MIS(WSignedId, WSignRecordNo, WOperator, WUserId, NullFutureDate, "","", 22);
                NForm22MIS.Show();

            }

            if (WAction == 3)
            {

                //     NForm22MIS = new Form22MIS(WSignedId, WSignRecordNo, WOperator, WUserId, WDtTm, "","", 23);
                NForm22MIS.Show();

            }

            if (WAction == 4)
            {

                //   NForm22MIS = new Form22MIS(WSignedId, WSignRecordNo, WOperator, WUserId, WDtTm, WCitId,"", 24);
                NForm22MIS.Show();

            }
            if (WAction == 5)
            {

                //     NForm22MIS = new Form22MIS(WSignedId, WSignRecordNo, WOperator, WUserId, WDtTm, WCitId,"", 25);
                NForm22MIS.Show();

            }
            if (WAction == 6)
            {

                //   NForm22MIS = new Form22MIS(WSignedId, WSignRecordNo, WOperator, WUserId, WDtTm, WCitId, WAtmNo, 26);
                NForm22MIS.Show();

            }

        }
        // FINISH 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        // Link to Excess 
        private void linkLabelAnalysisForExcess_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            int Mode = 12;
            //DateTime WExcess_From_Dt;
            //DateTime WExcess_To_Dt;

            Form67_Cycle_Rich_Picture_2 NForm67_Cycle_Rich_Picture_2;
            int WReconcCycleNo = 0;
            string WAtmNo = "";
            int WSesNo = 0;
            NForm67_Cycle_Rich_Picture_2 = new Form67_Cycle_Rich_Picture_2(WSignedId, WOperator, WAtmNo, WSesNo,
                WReconcCycleNo, WExcess_From_Dt, WExcess_To_Dt, Mode);
            NForm67_Cycle_Rich_Picture_2.ShowDialog();

            return;
        }
// GO to ANALYSIS
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            
            int Mode = 14; // for SHORTAGE 
            
            Form67_Cycle_Rich_Picture_2 NForm67_Cycle_Rich_Picture_2;
            int WReconcCycleNo = 0;
            string WAtmNo = "";
            int WSesNo = 0;
            NForm67_Cycle_Rich_Picture_2 = new Form67_Cycle_Rich_Picture_2(WSignedId, WOperator, WAtmNo, WSesNo,
                WReconcCycleNo, WShortage_From_Dt, WShortage_To_Dt, Mode);
            NForm67_Cycle_Rich_Picture_2.ShowDialog();

            return;
        }


        //public void CatchDetails(Exception ex)
        //{
        //    HasErrors = true;

        //    //if (Errors == null)
        //    //{
        //    //    Errors = new List<string>();
        //    //}
        //    RRDMLog4Net Log = new RRDMLog4Net();

        //    StringBuilder WParameters = new StringBuilder();

        //    string WDatetime = DateTime.Now.ToString();

        //    WParameters.Append("User : ");
        //    WParameters.Append("NotAssignYet");
        //    WParameters.Append(Environment.NewLine);

        //    WParameters.Append("DtTm : ");
        //    WParameters.Append(WDatetime);
        //    WParameters.Append(Environment.NewLine);

        //    string Logger = "RRDM4Atms";
        //    string Parameters = WParameters.ToString();

        //    Log.CreateAndInsertRRDMLog4NetMessage(ex, Logger, Parameters);

        //    LogErrorNo = Log.ErrorNo;
        //    if (Environment.UserInteractive)
        //    {
        //        System.Windows.Forms.MessageBox.Show("There is an issue to be reported to the helpdesk " + Environment.NewLine
        //                                                 + "Issue reference number: " + Log.ErrorNo.ToString());
        //    }
        //    ErrorDetails = ("There is an issue to be reported to the helpdesk " + Environment.NewLine
        //                                                 + "Issue reference number: " + Log.ErrorNo.ToString());
        //    //    Environment.Exit(0);}
        //}
    }
}

