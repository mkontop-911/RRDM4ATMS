using System;
using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Collections;

//using System.Windows.Forms;
//using System.Drawing;

namespace RRDM4ATMs
{
    public class RRDMReconcJobCycles : Logger
    {
        public RRDMReconcJobCycles() : base() { }
        //        FinishedDateTm datetime    Unchecked

        //InSuspense  bit Unchecked

        public int JobCycle;
        public string JobCategory; // 
        // An Installation can start one category for matching and then the next 
        public DateTime Cut_Off_Date; 
        public DateTime StartDateTm;
        public DateTime FinishDateTm;

        public string Description;

        public bool InSuspense;

        public int ProcessMode; // -1 : Under currect process
                                // 0  : Closed Cycle ... Ready for reconciliation 
                                // -1 : Under currect process
                                // -2  : Deleted Cycle // future implementation

        public int NumberOfLoadingAndMatching;

        public bool SpareBool_1;
        public bool SpareBool_2;

        public int SpareInt_1;
        public int SpareInt_2;

        public string SpareString_1;
        public string SpareString_2; 

        public string Operator;

        // Define the data table 
        public DataTable TableReconcJobCycles = new DataTable();

        public int TotalSelected;

        string SqlString; // Do not delete 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        readonly string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        // Read Table Fields
        private void ReadTableFields(SqlDataReader rdr)
        {
            JobCycle = (int)rdr["JobCycle"];
            JobCategory = (string)rdr["JobCategory"];

            Cut_Off_Date = (DateTime)rdr["Cut_Off_Date"];

            StartDateTm = (DateTime)rdr["StartDateTm"];
            FinishDateTm = (DateTime)rdr["FinishDateTm"];

            Description = (string)rdr["Description"];

            InSuspense = (bool)rdr["InSuspense"];

            ProcessMode = (int)rdr["ProcessMode"];

            NumberOfLoadingAndMatching = (int)rdr["NumberOfLoadingAndMatching"];

            SpareBool_1 = (bool)rdr["SpareBool_1"];
            SpareBool_2 = (bool)rdr["SpareBool_2"];

            SpareInt_1 = (int)rdr["SpareInt_1"];
            SpareInt_2 = (int)rdr["SpareInt_2"];

            SpareString_1 = (string)rdr["SpareString_1"];
            SpareString_2 = (string)rdr["SpareString_2"];

           Operator = (string)rdr["Operator"];
        }

        //
        // Methods 
        // READ  
        // 
        //
        public void ReadReconcJobCyclesById(string InOperator, int InJobCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[ReconcJobCycles] "
                      + " WHERE Operator = @Operator AND JobCycle = @JobCycle ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@JobCycle", InJobCycle);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadTableFields(rdr);
                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }

        //
        // Methods 
        // READ ITMXReconcJobCycles
        // FILL UP A TABLE
        //
        public void ReadReconcJobCyclesFillTable(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableReconcJobCycles = new DataTable();
            TableReconcJobCycles.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 

            TableReconcJobCycles.Columns.Add("JobCycle", typeof(int));
            TableReconcJobCycles.Columns.Add("JobCategory", typeof(string));
            TableReconcJobCycles.Columns.Add("Cut_Off_Date", typeof(string));
            TableReconcJobCycles.Columns.Add("StartDateTm", typeof(string));
            TableReconcJobCycles.Columns.Add("FinishDateTm", typeof(string));
            
            TableReconcJobCycles.Columns.Add("Description", typeof(string));

            TableReconcJobCycles.Columns.Add("Status", typeof(string));

            SqlString = "SELECT * "
                    + " FROM [ATMS].[dbo].[ReconcJobCycles] "
                    +  InSelectionCriteria 
                    + " ORDER BY JobCycle DESC ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        //cmd.Parameters.AddWithValue("@Operator", InOperator);
                     
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadTableFields(rdr);

                            DataRow RowSelected = TableReconcJobCycles.NewRow();

                            RowSelected["JobCycle"] = JobCycle;
                            RowSelected["JobCategory"] = JobCategory;

                            RowSelected["Cut_Off_Date"] = Cut_Off_Date.ToString();
                            
                            RowSelected["StartDateTm"] = StartDateTm.ToString();
                            if (FinishDateTm.Date == NullPastDate.Date)
                            {
                                RowSelected["FinishDateTm"] = "Not Ended yet";
                            }
                            else
                            {
                                RowSelected["FinishDateTm"] = FinishDateTm.ToString();
                            }
                            
                            RowSelected["Description"] = Description;

                            if (ProcessMode == 0)
                            {
                                RowSelected["Status"] = "Ready For Reconciliation";
                            }
                            if (ProcessMode == -1)
                            {
                                RowSelected["Status"] = "Currently Open";
                            }

                            // ADD ROW
                            TableReconcJobCycles.Rows.Add(RowSelected);

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }

        //
        // Methods 
        // READ ITMXReconcJobCycles
        // FILL UP A TABLE
        //
        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs(); 
        public void ReadReconcJobCyclesFillTable_2(string InMatchingCateg, string W_Application)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMMatchingTxns_InGeneralTables_MOBILE Mmob = new RRDMMatchingTxns_InGeneralTables_MOBILE(); 


            TableReconcJobCycles = new DataTable();
            TableReconcJobCycles.Clear();

            TotalSelected = 0;

            string SelectionCriteria = "";
            // DATA TABLE ROWS DEFINITION 

            TableReconcJobCycles.Columns.Add("JobCycle", typeof(int));
            TableReconcJobCycles.Columns.Add("JobCategory", typeof(string));
            TableReconcJobCycles.Columns.Add("Cut_Off_Date", typeof(string));
            TableReconcJobCycles.Columns.Add("StartDateTm", typeof(string));
            TableReconcJobCycles.Columns.Add("FinishDateTm", typeof(string));

            TableReconcJobCycles.Columns.Add("MatchedNo", typeof(int));

            SqlString = "SELECT * "
                    + " FROM [ATMS].[dbo].[ReconcJobCycles] "
                    + " WHERE JobCategory = '"+ W_Application +"'"
                    + " ORDER BY JobCycle DESC ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        //cmd.Parameters.AddWithValue("@Operator", InOperator);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadTableFields(rdr);

                            int TotalCounted = 0; 

                            if (W_Application == "ETISALAT" || W_Application == "QAHERA" || W_Application == "EGATE")
                            {
                                SelectionCriteria = " WHERE MatchingCateg='" + InMatchingCateg + "' "
                                              + " AND MatchingAtRMCycle =" + JobCycle
                                                ;

                                TotalCounted = Mmob.ReadInPoolTransBySelectionCriteria_Find_Total_MOBILE(SelectionCriteria, 2, W_Application);
                            }
                            else
                            {
                                SelectionCriteria = " WHERE MatchingCateg='" + InMatchingCateg + "' "
                                              + " AND MatchingAtRMCycle =" + JobCycle
                                                ;

                                TotalCounted = Mpa.ReadInPoolTransBySelectionCriteria_Find_Total(SelectionCriteria, 2);
                            }

                            if (TotalCounted > 0)
                            {
                                DataRow RowSelected = TableReconcJobCycles.NewRow();

                                RowSelected["JobCycle"] = JobCycle;
                                RowSelected["JobCategory"] = JobCategory;

                                RowSelected["Cut_Off_Date"] = Cut_Off_Date.ToShortDateString();

                                RowSelected["StartDateTm"] = StartDateTm.ToString();
                                if (FinishDateTm.Date == NullPastDate.Date)
                                {
                                    RowSelected["FinishDateTm"] = "Not Ended yet";
                                }
                                else
                                {
                                    RowSelected["FinishDateTm"] = FinishDateTm.ToString();
                                }

                                RowSelected["MatchedNo"] = TotalCounted;

                                // ADD ROW
                                TableReconcJobCycles.Rows.Add(RowSelected);
                            }
                            
                            

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }

        //
        // Methods 
        // READ ReconcJobCycles to find the latest one 
        // 
        //
        public void ReadLastReconcJobCycle(string InOperator)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            JobCycle = 0 ; 

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[ReconcJobCycles] "
                      + " WHERE Operator = @Operator "
                      + " ORDER BY JobCycle DESC";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        //cmd.Parameters.AddWithValue("@JobCategory", InJobCategory);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadTableFields(rdr);

                            break;

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
        }

        //
        public void ReadLastReconcJobCycle_2(string InOperator, string InJobCategory)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            JobCycle = 0;

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[ReconcJobCycles] "
                      + " WHERE Operator = @Operator AND JobCategory =@JobCategory "
                      + " ORDER BY JobCycle DESC";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@JobCategory", InJobCategory);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadTableFields(rdr);

                            break;

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
        }
        //
        // Methods 
        // READ ReconcJobCycle that Exist than date of replenishment 
        // 
        //
        public int Counter;
        public void Find_GL_Cut_Off_Before_GivenDate(string InOperator, DateTime InGivenDate)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            Counter = 0; 

            RRDMGasParameters Gp = new RRDMGasParameters();

            Gp.ReadParametersSpecificId(InOperator, "718", "1", "", "");

            if (Gp.OccuranceNm == "ZERO")
            {
                Counter = 1; 
            }
            if (Gp.OccuranceNm == "ONE")
            {
                Counter = 2;
            }

            SqlString = "SELECT * "
                      + " FROM [ATMS].[dbo].[ReconcJobCycles] "
                      + " WHERE Operator = @Operator and Cut_Off_Date < @Cut_Off_Date"
                      + " ORDER BY JobCycle DESC";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@Cut_Off_Date", InGivenDate);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadTableFields(rdr);

                            Counter = Counter - 1; 

                            if (Counter == 0 ) break;

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
        }

        // GET DISTICT Cycle + Cut Off Date  TO SHOW IN COMBO 
        public ArrayList GetCut_Off_Dates_List(string InOperator)
        {
            ArrayList Cut_Off_Dates_List = new ArrayList();
            //Cut_Off_Dates_List.Add("No Value");

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            //string Cycle_And_Date;

            SqlString = "SELECT *"
                 + " FROM [ATMS].[dbo].[ReconcJobCycles] "
                 + " WHERE Operator = @Operator "
                 + " ORDER BY JobCycle DESC";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;
                            
                            JobCycle = (int)rdr["JobCycle"];
                            Cut_Off_Date = (DateTime)rdr["Cut_Off_Date"];

                            //Cycle_And_Date = "ID: "+JobCycle + "..Cut OFF..: " + Cut_Off_Date.ToShortDateString();

                            Cut_Off_Dates_List.Add(Cut_Off_Date);

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }

            return Cut_Off_Dates_List;
        }
        //
        // Methods 
        // READ ReconcJobCycles to find the latest one 
        // 
        //
        public void ReadLastReconcJobCycle_Closed_Cycle(string InOperator, string InJobCategory)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
           // string WJobCategory = "ATMs";
            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[ReconcJobCycles] "
                      + " WHERE Operator = @Operator AND ProcessMode = 0 AND  JobCategory = @JobCategory"
                      + " ORDER BY JobCycle DESC";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@JobCategory", InJobCategory);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadTableFields(rdr);

                            break;

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
        }

        //
        // Methods 
        // READ ReconcJobCycles to find the latest Five 
        // 
        //
        public int Cycle1;
        public int Cycle2;
        public int Cycle3;
        public int Cycle4;
        public int Cycle5;

        public void ReadLastReconcJobCycleLastFive(string InOperator, int InJobCycle, string InJobCategory)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalSelected = 0; 

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[ReconcJobCycles] "
                      + " WHERE Operator = @Operator AND JobCycle <= @JobCycle AND JobCategory = @JobCategory"
                      + " ORDER BY JobCycle DESC";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@JobCycle", InJobCycle);
                        cmd.Parameters.AddWithValue("@JobCategory", InJobCategory);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadTableFields(rdr);

                            if (TotalSelected == 1) Cycle1 = JobCycle;
                            if (TotalSelected == 2) Cycle2 = JobCycle;
                            if (TotalSelected == 3) Cycle3 = JobCycle;
                            if (TotalSelected == 4) Cycle4 = JobCycle;
                            if (TotalSelected == 5)
                            {
                                Cycle5 = JobCycle;
                                break;
                            }

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
        }
        //
        // Methods 
        // READ ReconcJobCycles
        // 
        //
        public void ReadReconcJobCyclesByJobCategory(string InOperator, string InJobCategory)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[ReconcJobCycles] "
                      + " WHERE Operator = @Operator AND JobCategory = @JobCategory "
                      + " ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@JobCategory", InJobCategory);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadTableFields(rdr);

                            Operator = (string)rdr["Operator"];

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
        }

        //
        // Methods 
        // Find Next Cycle
        // 
        //
        public void FindNextCycle(string InOperator, int InCurrentCycleNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[ReconcJobCycles] "
                      + " WHERE Operator = @Operator AND JobCycle > @JobCycle "
                      + " Order By JobCycle ASC ";
       
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@JobCycle", InCurrentCycleNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadTableFields(rdr);

                            // Nex is read 

                            break; 

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
        }

        //
        // Methods 
        // READ ReconcJobCycles to find the last Close Cycle
        // 
        //
        public int ReadLastReconcJobCycleATMsAndNostro(string InOperator, string InJobCategory)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[ReconcJobCycles] "
                      + " WHERE Operator = @Operator AND JobCategory = @JobCategory "
                      + " ORDER BY JobCycle DESC";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@JobCategory", InJobCategory);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadTableFields(rdr);

                            //if (ProcessMode == 0)
                            //{
                                break;
                            //}
                                                
                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
            return JobCycle; 
        }

        //
        // Methods 
        // READ ReconcJobCycles to find the Last OPEN 
        // 
        //
        public int ReadLastReconcJobCycleATMsAndNostroWithMinusOne(string InOperator, string InJobCategory)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

          
            int JobCycleMinusOne = 0; 

            SqlString = "SELECT * "
                      + " FROM [ATMS].[dbo].[ReconcJobCycles] "
                      + " WHERE Operator = @Operator AND JobCategory = @JobCategory "
                      + " AND ProcessMode = -1  "
                      + " ORDER BY JobCycle ASC";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@JobCategory", InJobCategory);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadTableFields(rdr);

                            //if (ProcessMode == -1)
                            //{
                            //    JobCycleMinusOne = JobCycle; 
                            //    break;
                            //}
                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
            return JobCycle;
        }

        //
        //
        public void ReadLastReconcJobCycleATMsAndNostroWithMinusOne_Second_version(string InOperator, string InJobCategory)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            int JobCycleMinusOne = 0;

            SqlString = "SELECT * "
                      + " FROM [ATMS].[dbo].[ReconcJobCycles] "
                      + " WHERE Operator = @Operator AND JobCategory = @JobCategory "
                      + " AND ProcessMode = -1  "
                      + " ORDER BY JobCycle ASC";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@JobCategory", InJobCategory);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadTableFields(rdr);

                            //if (ProcessMode == -1)
                            //{
                            //    JobCycleMinusOne = JobCycle; 
                            //    break;
                            //}
                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
           // return JobCycle;
        }

        //
        // Methods 
        // READ ReconcJobCycles to find as per counter
        // 
        //
        public bool LimitFound; 
        public DateTime ReadPastJobCyclesByInCounter(string InOperator, string InJobCategory, int InCounter)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalSelected = 0; 

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[ReconcJobCycles] "
                      + " WHERE Operator = @Operator AND JobCategory = @JobCategory "
                      + " ORDER BY JobCycle DESC";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@JobCategory", InJobCategory);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadTableFields(rdr);

                            if (InCounter == TotalSelected )
                            {
                                LimitFound = true;
                                break;
                            }
                            else
                            {
                                LimitFound = false;
                            }
                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
            return Cut_Off_Date;
        }
        //
        // Methods 
        // READ First JobCycles 
        // 
        //
        public int ReadFirstReconcJobCycle(string InOperator, string InJobCategory)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            JobCycle = 0;

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[ReconcJobCycles] "
                      + " WHERE Operator = @Operator AND JobCategory = @JobCategory "
                      + " ORDER BY JobCycle ASC ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@JobCategory", InJobCategory);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadTableFields(rdr);

                            break;
                            
                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
            return JobCycle;
        }
      
        //
        // Methods 
        // READ  by Selection Criteria 
        // 
        //
        public void ReadReconcJobCyclesBySelectionCriteria(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            // Read the last Cycle = the current one
            SqlString = "SELECT * "
                      + " FROM [ATMS].[dbo].[ReconcJobCycles] "
                      + InSelectionCriteria 
                      + " Order by JobCycle ASC "
                      ;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        //cmd.Parameters.AddWithValue("@Operator", InOperator);
                        //cmd.Parameters.AddWithValue("@JobCycle", InJobCycle);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadTableFields(rdr);
                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }
        // Find Limit RMCycle based of Cut off Date
        public void ReadReconcJobCyclesByCutOffDate(DateTime InCut_Off_Date)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = " SELECT TOP (1) * "
                      + " FROM [ATMS].[dbo].[ReconcJobCycles] "
                      + " WHERE Cut_Off_Date <= @Cut_Off_Date AND JobCategory = 'ATMs' "
                      + " Order By JobCycle Desc "
                      ;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Cut_Off_Date", InCut_Off_Date);
                        //cmd.Parameters.AddWithValue("@JobCycle", InJobCycle);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadTableFields(rdr);
                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }
        // First Cycle 
        public void ReadReconcJobCyclesAndFindFirstCycle(DateTime InCut_Off_Date)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = " SELECT TOP (1) * "
                      + " FROM [ATMS].[dbo].[ReconcJobCycles] "
                      + " WHERE JobCategory = 'ATMs' "
                      + " Order By JobCycle ASC "
                      ;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Cut_Off_Date", InCut_Off_Date);
                        //cmd.Parameters.AddWithValue("@JobCycle", InJobCycle);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadTableFields(rdr);
                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }

        public void ReadReconcJobCyclesAndFindFirstCycle_MOBILE(string InW_Application)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = " SELECT TOP (1) * "
                      + " FROM [ATMS].[dbo].[ReconcJobCycles] "
                      + " WHERE JobCategory = @JobCategory "
                      + " Order By JobCycle ASC "
                      ;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@JobCategory", InW_Application);
                        //cmd.Parameters.AddWithValue("@JobCycle", InJobCycle);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadTableFields(rdr);
                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }


        public void ReadReconcJobCyclesByCutOffDateEqualOrLess(DateTime InCut_Off_Date)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[ReconcJobCycles] "
                      + " WHERE Cut_Off_Date <= @Cut_Off_Date " 
                      +  " ORDER BY Cut_Off_Date DESC" 
                      + "";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Cut_Off_Date", InCut_Off_Date);
                        //cmd.Parameters.AddWithValue("@JobCycle", InJobCycle);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadTableFields(rdr);

                            break; 
                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }
        // Create A New ReconcJobCycle
        public int OldReconcJobCycle;
        public int WReconcCycleNo; 
        public int Create_A_New_ReconcJobCycle(string InOperator, string InSignedId,  DateTime InCut_Off_Date)
        {
            RRDMReconcCategories Rc = new RRDMReconcCategories();

            RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();

            RRDMMatchingCategoriesVsSourcesFiles Msf = new RRDMMatchingCategoriesVsSourcesFiles();

            try
            {
                string SelectionCriteria = " WHERE ProcessMode = -1 AND JobCategory = 'ATMs' ";
                ReadReconcJobCyclesBySelectionCriteria(SelectionCriteria);

                if (RecordFound == true)
                {
                    InSuspense = true;

                    FinishDateTm = DateTime.Now; // So it less than the new one created 

                    OldReconcJobCycle = JobCycle;

                    UpdateDailyJobCycle(JobCycle);

                }
                else
                {
                    OldReconcJobCycle = 0;
                }
                //**************************
                // Insert new Cycle 
                //**************************
                JobCategory = "ATMs";

                Cut_Off_Date = InCut_Off_Date;

                StartDateTm = DateTime.Now;
                FinishDateTm = NullPastDate;

                Description = "ATMs Reconciliation";

                InSuspense = false;
                //
                // Default Value 
                // Rjc.ProcessMode = -1 Through the Data Base Definition; 
                //
                Operator = InOperator;

                // CREATE NEW CYCLE 
                WReconcCycleNo = InsertNewReconcJobCycle();
                //
                // Update files the date to be expected to be loaded. 
                // 
                Msf.AssignNextLoadingDate(InOperator, WReconcCycleNo, Cut_Off_Date);
                // 
              
                Rc.CreateReconciliationSessionsForAtms_AND_JCC(InOperator, InSignedId,
                                                  WReconcCycleNo);
                //**********************************
                int TempProcessMode = 0;
                // Update old Cycle
                Rcs.UpdateReconcCategorySessionAtOpeningNewCycle(InOperator, OldReconcJobCycle, TempProcessMode);
                //
                // Finalise Updating of the old one 
                //
                SelectionCriteria = " WHERE JobCycle = " + OldReconcJobCycle;
                ReadReconcJobCyclesBySelectionCriteria(SelectionCriteria);

                if (RecordFound == true)
                {
                    InSuspense = false;

                    ProcessMode = 0;

                    UpdateDailyJobCycle(OldReconcJobCycle);
                }
            }
            catch (Exception ex)
            {
       
                CatchDetails(ex);

            }
            return WReconcCycleNo; 
        }


        public int Create_A_New_ReconcJobCycle_MOBILE(string InJobCategory, string InOperator, string InSignedId, DateTime InCut_Off_Date)
        {
            RRDMReconcCategories Rc = new RRDMReconcCategories();

            RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();

            RRDMMatchingCategoriesVsSourcesFiles Msf = new RRDMMatchingCategoriesVsSourcesFiles();

            try
            {
                string SelectionCriteria = " WHERE ProcessMode = -1 AND JobCategory ='"+ InJobCategory + "'";
                ReadReconcJobCyclesBySelectionCriteria(SelectionCriteria);

                if (RecordFound == true)
                {
                    InSuspense = true;

                    FinishDateTm = DateTime.Now; // So it less than the new one created 

                    OldReconcJobCycle = JobCycle;

                    UpdateDailyJobCycle(JobCycle);

                }
                else
                {
                    OldReconcJobCycle = 0;
                }
                //**************************
                // Insert new Cycle 
                //**************************
                JobCategory = InJobCategory;

                Cut_Off_Date = InCut_Off_Date;

                StartDateTm = DateTime.Now;
                FinishDateTm = NullPastDate;

                Description = "Cycle for.." + InJobCategory;

                InSuspense = false;
                //
                // Default Value 
                // Rjc.ProcessMode = -1 Through the Data Base Definition; 
                //
                Operator = InOperator;

                // CREATE NEW CYCLE 
                WReconcCycleNo = InsertNewReconcJobCycle();
                //
                // Update files the date to be expected to be loaded. 
                // 
                Msf.AssignNextLoadingDate_MOBILE(InJobCategory, InOperator, WReconcCycleNo, Cut_Off_Date);
                // 

                Rc.CreateReconciliationSessionsFor_MOBILE(InJobCategory, InOperator, InSignedId,
                                                  WReconcCycleNo);
                //**********************************
                int TempProcessMode = 0;
                // Update old Cycle
                Rcs.UpdateReconcCategorySessionAtOpeningNewCycle(InOperator, OldReconcJobCycle, TempProcessMode);
                //
                // Finalise Updating of the old one 
                //
                SelectionCriteria = " WHERE JobCycle = " + OldReconcJobCycle;
                ReadReconcJobCyclesBySelectionCriteria(SelectionCriteria);

                if (RecordFound == true)
                {
                    InSuspense = false;

                    ProcessMode = 0;

                    UpdateDailyJobCycle(OldReconcJobCycle);
                }
            }
            catch (Exception ex)
            {

                CatchDetails(ex);

            }
            return WReconcCycleNo;
        }



        // Insert NEW Daily Job Cycle 
        public int InsertNewReconcJobCycle()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[ReconcJobCycles] "
                + " ([JobCategory],"
                 + " [Cut_Off_Date],"
                + " [StartDateTm],"
                + " [FinishDateTm]," 
                + " [Description],"
                + " [InSuspense],"
                + " [Operator] )"
                + " VALUES"
                + " (@JobCategory,"
                + " @Cut_Off_Date,"
                + " @StartDateTm,"
                + " @FinishDateTm,"
                + " @Description,"
                + " @InSuspense,"
                + " @Operator ) ;"
                + " SELECT CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@JobCategory", JobCategory);

                        cmd.Parameters.AddWithValue("@Cut_Off_Date", Cut_Off_Date);

                        cmd.Parameters.AddWithValue("@StartDateTm", StartDateTm);
                        cmd.Parameters.AddWithValue("@FinishDateTm", FinishDateTm);
                        cmd.Parameters.AddWithValue("@Description", Description);
                        cmd.Parameters.AddWithValue("@InSuspense", InSuspense); 
                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        //rows number of record got updated
                        JobCycle = (int)cmd.ExecuteScalar();

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex); 
                }

            return JobCycle;
        }

        // UPDATE  Daily Job Cycle 
        // 
        public void UpdateDailyJobCycle(int InJobCycle)
        {
            
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[ReconcJobCycles] SET "
                            + " FinishDateTm = @FinishDateTm, "
                            + " InSuspense = @InSuspense,  "
                            + " ProcessMode = @ProcessMode  "
                            + " WHERE JobCycle = @JobCycle", conn))
                    {

                        cmd.Parameters.AddWithValue("@JobCycle", InJobCycle);
                        cmd.Parameters.AddWithValue("@FinishDateTm", FinishDateTm);
                        cmd.Parameters.AddWithValue("@InSuspense", InSuspense);
                        cmd.Parameters.AddWithValue("@ProcessMode", ProcessMode);
                        //cmd.Parameters.AddWithValue("@Operator", Operator);

                        //rows number of record got updated

                        cmd.ExecuteNonQuery();
                      

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
        }

        // UPDATE  Special Fields
        // 
        public void UpdateSpecialFields(int InJobCycle)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[ReconcJobCycles] SET "
                            + " SpareBool_1 = @SpareBool_1 "
                            + ", SpareBool_2 = @SpareBool_2 "
                              + ", SpareInt_1 = @SpareInt_1 "
                                + ", SpareInt_2 = @SpareInt_2 "
                                  + ", SpareString_1 = @SpareString_1 "
                                    + ", SpareString_2 = @SpareString_2 "
                            + " WHERE JobCycle = @JobCycle", conn))
                    {

                        cmd.Parameters.AddWithValue("@JobCycle", InJobCycle);

                        cmd.Parameters.AddWithValue("@SpareBool_1", SpareBool_1);
                        cmd.Parameters.AddWithValue("@SpareBool_2", SpareBool_2);
                        cmd.Parameters.AddWithValue("@SpareInt_1", SpareInt_1);
                        cmd.Parameters.AddWithValue("@SpareInt_2", SpareInt_2);
                        cmd.Parameters.AddWithValue("@SpareString_1", SpareString_1);
                        cmd.Parameters.AddWithValue("@SpareString_2", SpareString_2);
                        cmd.ExecuteNonQuery();


                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
        }

        //
        // DELETE current Cycle 
        // Change Process of the last one to -1
        //
        public void DeleteCycleNo(string InOperator, int InJobCycle, string InJobCategory)
        {


            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    // ATM FIELDS 
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE [ATMS].[dbo].[ReconcJobCycles]  "
                            + " WHERE JobCycle = @JobCycle ", conn))
                    {
                        cmd.Parameters.AddWithValue("@JobCycle", InJobCycle);

                        cmd.ExecuteNonQuery();

                    }

                    conn.Close();

                    RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions(); 

                    Rcs.DeleteCycleNo(InOperator, InJobCycle); 

                    // Read Last remaining and update it with -1
                    ReadLastReconcJobCycle_2(InOperator, InJobCategory);

                    ProcessMode = -1; 

                    UpdateDailyJobCycle(JobCycle);

                    RRDMMatchingCategoriesVsSourcesFiles Msf = new RRDMMatchingCategoriesVsSourcesFiles();

                    Msf.UpdateReconcCategoryVsSourceRecordForDeleteRmCycleNo(InJobCycle, JobCycle, Cut_Off_Date); 


                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }

      
    }
}


