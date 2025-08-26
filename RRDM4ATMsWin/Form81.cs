using System;
using System.Data;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMsWin
{
    public partial class Form81 : Form
    {
        /// <summary>
        /// THROUGH THIS THE CONTROOLER CHECKS ITS HEALTH 
        /// AND WORK TO BE DONE
        /// </summary>
        /// 

        int I; 
        int NumberOfDays;
        int Min;
        DateTime LatestDate;
        int NeedCount;

        int NoAction;

        int ActiveActions;

        int TransOutstanding;

        int QualityRange1;
        int QualityRange2; 

        DateTime LatestHol; 

        string connectionString = ConfigurationManager.ConnectionStrings
          ["ATMSConnectionString"].ConnectionString;

        RRDMGasParameters Gp = new RRDMGasParameters();
        RRDMUsersRecords Us = new RRDMUsersRecords();

 //       string WUserOperator;

        DataTable TblMatchedDates = new DataTable();
        DataTable TblFixedDates = new DataTable();

        string WSignedId;
        int WSignRecordNo;
        string WSecLevel;
        string WOperator;
     //   bool WPrive;
        public Form81(string InSignedId, int InSignRecordNo, string InSecLevel, string InOperator)
        {         
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;
        

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

            textBoxMsgBoard.Text = "Controller sees information about the work to be done"; 
        }
        // ON LOADING 
        private void Form81_Load(object sender, EventArgs e)
        {

            //TEST
            DateTime WDate = new DateTime(2014, 03, 04);

            string SqlString1 =
                       " SELECT MatchDatesCateg AS Category , MAX(cast(NextDate AS DATE)) As LatestDate , Count(NextDate) AS NumberOfDays   " 
                     + " FROM [dbo].[MatchedDatesTable]"
                     + " WHERE Operator = @Operator AND NextDate > @NextDate " 
                     + " Group by MatchDatesCateg"
                     + " ORDER BY MatchDatesCateg ASC ";
            
            using (SqlConnection conn =
                        new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString1, conn);

                    sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", WOperator);
                    sqlAdapt.SelectCommand.Parameters.AddWithValue("@NextDate", WDate);

                    //Create a datatable that will be filled with the data retrieved from the command
                    //    DataSet MISds = new DataSet();
                    sqlAdapt.Fill(TblMatchedDates);

                    //Fill the dataGrid that will be displayed with the dataset
                    dataGridView1.DataSource = TblMatchedDates.DefaultView;

                    // Close conn
                    conn.Close();

                }

                catch (Exception ex)
                {
                    string exception = ex.ToString();
                    MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));
                }
            // SHOW DATA GRID FOR MATCHED DATES 
            dataGridView1.Show();

             I = 0;

             Min = 1000; 

             while (I < (TblMatchedDates.Rows.Count))
             {
                 LatestDate = (DateTime)TblMatchedDates.Rows[I]["LatestDate"];
                 TimeSpan Remain1 = LatestDate - WDate;

                 TblMatchedDates.Rows[I]["NumberOfDays"] = Remain1.TotalDays;

                 NumberOfDays = (int)TblMatchedDates.Rows[I]["NumberOfDays"];

                 if (NumberOfDays < Min)
                 {
                     Min = NumberOfDays; 
                 }
                
                 I++; 
             }

           

            // ASSIGN TRAFFIC LIGHTS FOR MATCHED DATES 
             //  int QualityRange1 = 15;
             //  int QualityRange2 = 20; 

             Gp.ReadParametersSpecificId(WOperator,"601", "1", "", "");
             QualityRange1 = (int)Gp.Amount ;

             Gp.ReadParametersSpecificId(WOperator,"602", "1", "", "");
             QualityRange2 = (int)Gp.Amount; 

            if (Min > QualityRange2 ) 
            {
                pictureBox3.BackgroundImage = appResImg.GREEN_LIGHT;
                textBox2.Text = "Min>"+QualityRange2.ToString();
            }
            if (Min >= QualityRange1 & Min <= QualityRange2)
            {
                pictureBox3.BackgroundImage = appResImg.YELLOW;
                textBox2.Text = QualityRange1.ToString() + "<Min<" + QualityRange2.ToString();
            }
            if (Min < QualityRange1)
            {
                pictureBox3.BackgroundImage = appResImg.RED_LIGHT;
                textBox2.Text = "Min<" + QualityRange1.ToString(); ;
            }

            // FIXED REPL DATES 
           
            //TEST
            DateTime WDate2 = new DateTime(2014, 03, 03);

            string SqlString2 =
                       " SELECT [AtmNo],MAX(CAST(NextDate AS DATE)) As LatestDate , Count(NextDate) AS NumberOfDays  "
                     + " FROM [dbo].[FixedDaysReplAtm]"
                     + " WHERE Operator = @Operator AND NextDate > @NextDate "
                     + " Group by AtmNo"
                     + " ORDER BY AtmNo ASC ";

            using (SqlConnection conn =
                        new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString2, conn);

                    sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", WOperator);
                    sqlAdapt.SelectCommand.Parameters.AddWithValue("@NextDate", WDate2);

                    //Create a datatable that will be filled with the data retrieved from the command
                    //    DataSet MISds = new DataSet();
                    sqlAdapt.Fill(TblFixedDates);

                    //Fill the dataGrid that will be displayed with the dataset
                    dataGridView2.DataSource = TblFixedDates.DefaultView;

                    // Close conn
                    conn.Close();

                }

                catch (Exception ex)
                {
                    string exception = ex.ToString();
                    MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));
                }
         
            // FIND NUMBER OF DAYS AND MINIMUM
            I = 0;

            Min = 1000;

            while (I < (TblFixedDates.Rows.Count))
            {
                LatestDate = (DateTime)TblFixedDates.Rows[I]["LatestDate"];
                TimeSpan Remain2 = LatestDate - WDate2;
               
                TblFixedDates.Rows[I]["NumberOfDays"] = Remain2.TotalDays;

                NumberOfDays = (int)TblFixedDates.Rows[I]["NumberOfDays"];

                if (NumberOfDays < Min)
                {
                    Min = NumberOfDays;
                }

                I++;

            }

            // SHOW DATA GRID FIXED REPL DATES 
            dataGridView2.Show();

            // ASSIGN TRAFFIC LIGHTS
         //   QualityRange1 = 10;
          //  QualityRange2 = 15;

            Gp.ReadParametersSpecificId(WOperator,"601", "2", "", "");
            QualityRange1 = (int)Gp.Amount;

            Gp.ReadParametersSpecificId(WOperator,"602", "2", "", "");
            QualityRange2 = (int)Gp.Amount; 

            if (Min > QualityRange2)
            {
                pictureBox4.BackgroundImage = appResImg.GREEN_LIGHT;
                textBox11.Text = "Min>" + QualityRange2.ToString();
            }
            if (Min >= QualityRange1 & Min <= QualityRange2)
            {
                pictureBox4.BackgroundImage = appResImg.YELLOW;
                textBox11.Text = QualityRange1.ToString() + "<Min<" + QualityRange2.ToString();
            }
            if (Min < QualityRange1)
            {
                pictureBox4.BackgroundImage = appResImg.RED_LIGHT;
                textBox11.Text = "Min<" + QualityRange1.ToString(); ;
            }

            // CHECK OTHER INDICATORS 
            bool Red = false; 
            // WAS THE JOB FOR ATMS IN NEED RUNNED? 
            AtmsInNeedRun() ;
            
            if (NeedCount == 0)
            {
                textBox10.Text = " NOT RUN ";
                Red = true; 
            }
            else textBox10.Text = " YES... RUN";

            // Check ATMs in Need without action
            // 
            if (NeedCount > 0)
            {
                AtmsInNeedWithNoAction();

                textBox4.Text = NoAction.ToString();
                if (NoAction > 0) Red = true; 
            }
            else textBox4.Text = "N/A";
           

            // Find the Active actions
            // 
            FindNoActiveActions();
            if (ActiveActions > 0)
            {
                textBox5.Text = ActiveActions.ToString();
                Red = true; 
            }

            // FIND THE OUTSTANDING TRANS TO BE POSTED 
            //
            FindOutstandingTranstobePosted();

            if (TransOutstanding > 0)
            {
             textBox7.Text = TransOutstanding.ToString();
             Red = true; 
            } 

            // Holidays Messages 
            //
            Gp.ReadParametersSpecificId(WOperator,"601", "3", "", ""); // < is Red 
            QualityRange1 = (int)Gp.Amount;

            Gp.ReadParametersSpecificId(WOperator,"602", "3", "", "");// > is Green 
            QualityRange2 = (int)Gp.Amount; 
            // FIND MAXIMUM HOLIDAY  
            //
            FindLatestHoliday();

            textBox8.Text = LatestHol.ToShortDateString();

            TimeSpan Remain3 = LatestHol - DateTime.Now;
            textBox9.Text = Remain3.TotalDays.ToString();

            if (Remain3.TotalDays < QualityRange1)
            {
                Red = true; 
            }
            if (Remain3.TotalDays < (QualityRange1-5))
            {
                MessageBox.Show("Critical Warning: Update Holidays table");
            }

            // ASSIGN TRAFFIC LIGHT

            if (Red == false)
            {
                pictureBox2.BackgroundImage = appResImg.GREEN_LIGHT;
            }
            if (Red == true)
            {
                pictureBox2.BackgroundImage = appResImg.RED_LIGHT;
            }
         
        }
        //
// FIND WHETHER THE ATMS IN NEED WAS RUN
        //
        public void AtmsInNeedRun()
        {
            NeedCount = 0;

            string SqlString = 
           " SELECT Count(AtmNo) AS NeedCount"
        + " FROM [ATMS].[dbo].[AtmsMain]"
        + "  WHERE Cast(LastInNeedReview AS DATE) = @Today "; 

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Today", DateTime.Today);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            NeedCount = (int)rdr["NeedCount"];

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
                    MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

                }
        }
        //
        // FIND THE ATMS IN NEED WITHOUT ACTION
        //
        public void AtmsInNeedWithNoAction()
        {
            NoAction = 0; 

            string SqlString =
           " SELECT Count(AtmNo) AS NoAction"
        + " FROM [ATMS].[dbo].[AtmsMain]"
        + "  WHERE Cast(LastInNeedReview AS DATE) = @Today And NeedType > 10 AND ActionNo = 0 ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Today", DateTime.Today);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            NoAction = (int)rdr["NoAction"];

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
                    MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

                }
        }

        //
        // FIND THE ACTIVE ACTIONS ( NOT FINALISED )
        //
        public void FindNoActiveActions()
        {
            ActiveActions = 0; 

            string SqlString =
           " SELECT COUNT(ReplActNo) AS ActiveActions"
                + " FROM [dbo].[ReplActionsTable]"
                + "  WHERE ActiveRecord = 1";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                     //   cmd.Parameters.AddWithValue("@Today", DateTime.Today);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            ActiveActions = (int)rdr["ActiveActions"];

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
                    MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

                }
        }

      
         //
        // FIND THE OUTSTANDING TRANS TO BE POSTED 
        //
        public void FindOutstandingTranstobePosted()
        {
            TransOutstanding = 0; 

            string SqlString =
           " SELECT COUNT ( PostedNo) AS TransOutstanding "
       + " FROM [ATMS].[dbo].[TransToBePosted]"
       + " WHERE [ActionCd2] = 0 OR [ActionCd2] = 2 OR [ActionCd2] = 3 "; 

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                     //   cmd.Parameters.AddWithValue("@Today", DateTime.Today);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            TransOutstanding = (int)rdr["TransOutstanding"];

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
                    MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

                }
        }

        //
        // FIND MAXIMUM HOLIDAY  
        //
        public void FindLatestHoliday() 
        {   
            string SqlString =
                 " SELECT MAX(SpecialDay) AS LatestHol "
                  + " FROM [ATMS].[dbo].[HolidaysAndSpecialDays]"; 

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                     //   cmd.Parameters.AddWithValue("@Today", DateTime.Today);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            LatestHol = (DateTime)rdr["LatestHol"];

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
                    MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

                }
        }
// Finish 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }
    }
}
